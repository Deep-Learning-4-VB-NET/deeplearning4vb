Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Threading
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports IteratorDataSetIterator = org.deeplearning4j.datasets.iterator.IteratorDataSetIterator
Imports IteratorMultiDataSetIterator = org.deeplearning4j.datasets.iterator.IteratorMultiDataSetIterator
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.evaluation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports DeviceLocalNDArray = org.nd4j.linalg.util.DeviceLocalNDArray

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.deeplearning4j.spark.impl.evaluation


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class EvaluationRunner
	Public Class EvaluationRunner

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New EvaluationRunner()

		Public Shared ReadOnly Property Instance As EvaluationRunner
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Private ReadOnly workerCount As New AtomicInteger(0)
		Private queue As LinkedList(Of Eval) = New ConcurrentLinkedQueue(Of Eval)()
		'parameters map for device local parameters for a given broadcast
		'Note: byte[] doesn't override Object.equals hence this is effectively an *identity* weak hash map, which is what we want here
		'i.e., DeviceLocal<INDArray> can be GC'd once the Broadcast<byte[]> is no longer referenced anywhere
		'This approach relies on the fact that a single Broadcast object's *content* will be shared by all of Spark's threads,
		' even though the Broadcast object itself mayb not be
		'Also by storing params as a byte[] (i.e., in serialized form), we sidestep a lot of the thread locality issues
		Private paramsMap As IDictionary(Of SByte(), DeviceLocalNDArray) = New WeakHashMap(Of SByte(), DeviceLocalNDArray)()


		Private Sub New()
		End Sub

		''' <summary>
		''' Evaluate the data using the specified evaluations </summary>
		''' <param name="evals">         Evaluations to perform </param>
		''' <param name="evalWorkers">   Number of concurrent workers </param>
		''' <param name="evalBatchSize"> Evaluation batch size to use </param>
		''' <param name="ds">            DataSet iterator </param>
		''' <param name="mds">           MultiDataSet iterator </param>
		''' <param name="isCG">          True if ComputationGraph, false otherwise </param>
		''' <param name="json">          JSON for the network </param>
		''' <param name="params">        Parameters for the network </param>
		''' <returns> Future for the results </returns>
		Public Overridable Function execute(ByVal evals() As IEvaluation, ByVal evalWorkers As Integer, ByVal evalBatchSize As Integer, ByVal ds As IEnumerator(Of DataSet), ByVal mds As IEnumerator(Of MultiDataSet), ByVal isCG As Boolean, ByVal json As Broadcast(Of String), ByVal params As Broadcast(Of SByte())) As Future(Of IEvaluation())
			Preconditions.checkArgument(evalWorkers > 0, "Invalid number of evaluation workers: must be > 0. Got: %s", evalWorkers)
			Preconditions.checkState(ds IsNot Nothing OrElse mds IsNot Nothing, "No data provided - both DataSet and MultiDataSet iterators were null")

			'For multi-GPU we'll use a round robbin approach for worker thread/GPU affinity
			Dim numDevices As Integer = Nd4j.AffinityManager.NumberOfDevices
			If numDevices <= 0 Then
				numDevices = 1
			End If

			'Create the device local params if required
			Dim deviceLocalParams As DeviceLocalNDArray
			SyncLock Me
				If Not paramsMap.ContainsKey(params.getValue()) Then
					'Due to singleton pattern, this block should execute only once (first thread)
					'Initially put on device 0. For CPU, this means we only have a single copy of the params INDArray shared by
					' all threads, which is both safe and uses the least amount of memory
					'For CUDA, we can't share threads otherwise arrays will be continually relocated, causing a crash
					'Nd4j.getMemoryManager().releaseCurrentContext();
					'NativeOpsHolder.getInstance().getDeviceNativeOps().setDevice(0);
					'Nd4j.getAffinityManager().attachThreadToDevice(Thread.currentThread(), 0);
					Dim pBytes() As SByte = params.getValue()
					Dim p As INDArray
					Try
						p = Nd4j.read(New MemoryStream(pBytes))
					Catch e As Exception
						Throw New Exception(e) 'Should never happen
					End Try
					Dim dlp As New DeviceLocalNDArray(p)
					paramsMap(params.getValue()) = dlp
					'log.info("paramsMap: size {}", paramsMap.size());
				End If
				deviceLocalParams = paramsMap(params.getValue())
			End SyncLock

			Dim currentWorkerCount As Integer
			currentWorkerCount = workerCount.get()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while((currentWorkerCount = workerCount.get()) < evalWorkers)
			Do While currentWorkerCount < evalWorkers
				'For load balancing: we're relying on the fact that threads are mapped to devices in a round-robbin approach
				' the first time they touch an INDArray. If we assume this method is called by new threads,
				' then the first N workers will be distributed evenly across available devices.

					If workerCount.compareAndSet(currentWorkerCount, currentWorkerCount + 1) Then
						log.debug("Starting evaluation in thread {}", Thread.CurrentThread.getId())
						'This thread is now a worker
						Dim f As New EvaluationFuture()
						f.setResult(evals)
						Try
							Dim m As Model
							If isCG Then
								Dim conf As ComputationGraphConfiguration = ComputationGraphConfiguration.fromJson(json.getValue())
								Dim cg As New ComputationGraph(conf)
								cg.init(deviceLocalParams.get(), False)
								m = cg
							Else
								Dim conf As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json.getValue())
								Dim net As New MultiLayerNetwork(conf)
								net.init(deviceLocalParams.get(), False)
								m = net
							End If

							'Perform eval on this thread's data
							Try
								doEval(m, evals, ds, mds, evalBatchSize)
							Catch t As Exception
								f.setException(t)
							Finally
								f.getSemaphore().release(1)
							End Try

							'Perform eval on other thread's data
							Do While queue.Count > 0
								Dim e As Eval = queue.RemoveFirst() 'Use poll not remove to avoid race condition on last element
								If e Is Nothing Then
									Continue Do
								End If
								Try
									doEval(m, evals, e.getDs(), e.getMds(), evalBatchSize)
								Catch t As Exception
									e.getFuture().setException(t)
								Finally
									e.getFuture().getSemaphore().release(1)
								End Try
							Loop
						Finally
							workerCount.decrementAndGet()
							log.debug("Finished evaluation in thread {}", Thread.CurrentThread.getId())
						End Try

						Nd4j.Executioner.commit()
						Return f
					End If
					currentWorkerCount = workerCount.get()
			Loop

			'At this point: not a worker thread (otherwise, would have returned already)
			log.debug("Submitting evaluation from thread {} for processing in evaluation thread", Thread.CurrentThread.getId())
			Dim f As New EvaluationFuture()
			queue.AddLast(New Eval(ds, mds, evals, f))
			Return f
		End Function

		Private Shared Sub doEval(ByVal m As Model, ByVal e() As IEvaluation, ByVal ds As IEnumerator(Of DataSet), ByVal mds As IEnumerator(Of MultiDataSet), ByVal evalBatchSize As Integer)
			If TypeOf m Is MultiLayerNetwork Then
				Dim mln As MultiLayerNetwork = DirectCast(m, MultiLayerNetwork)
				If ds IsNot Nothing Then
					mln.doEvaluation(New IteratorDataSetIterator(ds, evalBatchSize), e)
				Else
					mln.doEvaluation(New IteratorMultiDataSetIterator(mds, evalBatchSize), e)
				End If
			Else
				Dim cg As ComputationGraph = DirectCast(m, ComputationGraph)
				If ds IsNot Nothing Then
					cg.doEvaluation(New IteratorDataSetIterator(ds, evalBatchSize), e)
				Else
					cg.doEvaluation(New IteratorMultiDataSetIterator(mds, evalBatchSize), e)
				End If
			End If
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data private static class Eval
		Private Class Eval
			Friend ds As IEnumerator(Of DataSet)
			Friend mds As IEnumerator(Of MultiDataSet)
			Friend evaluations() As IEvaluation
			Friend future As EvaluationFuture
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter @Getter private static class EvaluationFuture implements Future<org.nd4j.evaluation.IEvaluation[]>
		Private Class EvaluationFuture
			Implements Future(Of IEvaluation())

			Friend semaphore As New Semaphore(0)
			Friend result() As IEvaluation
			Friend exception As Exception

			Public Overrides Function cancel(ByVal mayInterruptIfRunning As Boolean) As Boolean
				Throw New System.NotSupportedException("Not supported")
			End Function

			Public Overrides ReadOnly Property Cancelled As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property Done As Boolean
				Get
					Return semaphore.availablePermits() > 0
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.evaluation.IEvaluation[] get() throws InterruptedException, ExecutionException
			Public Overrides Function get() As IEvaluation()
				If result Is Nothing AndAlso exception Is Nothing Then
					semaphore.acquire() 'Block until completion (or failure) is reported
				End If
				If exception IsNot Nothing Then
					Throw New ExecutionException(exception)
				End If
				Return result
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.evaluation.IEvaluation[] get(long timeout, @NonNull TimeUnit unit)
			Public Overrides Function get(ByVal timeout As Long, ByVal unit As TimeUnit) As IEvaluation()
				Throw New System.NotSupportedException()
			End Function
		End Class
	End Class

End Namespace