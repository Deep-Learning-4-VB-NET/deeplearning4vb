Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports AsyncMultiDataSetIterator = org.nd4j.linalg.dataset.AsyncMultiDataSetIterator
Imports IteratorMultiDataSetIterator = org.deeplearning4j.datasets.iterator.IteratorMultiDataSetIterator
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports TrainingResult = org.deeplearning4j.spark.api.TrainingResult
Imports org.deeplearning4j.spark.api
Imports WorkerConfiguration = org.deeplearning4j.spark.api.WorkerConfiguration
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
Imports StatsCalculationHelper = org.deeplearning4j.spark.api.stats.StatsCalculationHelper
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.spark.api.worker


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class ExecuteWorkerMultiDataSetFlatMap<R extends org.deeplearning4j.spark.api.TrainingResult> implements org.apache.spark.api.java.function.FlatMapFunction<java.util.Iterator<org.nd4j.linalg.dataset.api.MultiDataSet>, R>
	Public Class ExecuteWorkerMultiDataSetFlatMap(Of R As org.deeplearning4j.spark.api.TrainingResult)
		Implements FlatMapFunction(Of IEnumerator(Of MultiDataSet), R)

		Private ReadOnly worker As TrainingWorker(Of R)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<R> call(java.util.Iterator<org.nd4j.linalg.dataset.api.MultiDataSet> dataSetIterator) throws Exception
		Public Overrides Function [call](ByVal dataSetIterator As IEnumerator(Of MultiDataSet)) As IEnumerator(Of R)
			Dim dataConfig As WorkerConfiguration = worker.DataConfiguration

			Dim stats As Boolean = dataConfig.isCollectTrainingStats()
			Dim s As StatsCalculationHelper = (If(stats, New StatsCalculationHelper(), Nothing))
			If stats Then
				s.logMethodStartTime()
			End If

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not dataSetIterator.hasNext() Then
				If stats Then
					s.logReturnTime()
				End If
				'TODO return the results...
				Return Collections.emptyIterator() 'Sometimes: no data
			End If

			Dim batchSize As Integer = dataConfig.getBatchSizePerWorker()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int prefetchCount = dataConfig.getPrefetchNumBatches();
			Dim prefetchCount As Integer = dataConfig.getPrefetchNumBatches()

			Dim batchedIterator As MultiDataSetIterator = New IteratorMultiDataSetIterator(dataSetIterator, batchSize)
			If prefetchCount > 0 Then
				batchedIterator = New AsyncMultiDataSetIterator(batchedIterator, prefetchCount)
			End If

			Try
				If stats Then
					s.logInitialModelBefore()
				End If
				Dim net As ComputationGraph = worker.InitialModelGraph
				If stats Then
					s.logInitialModelAfter()
				End If

				Dim miniBatchCount As Integer = 0
				Dim maxMinibatches As Integer = (If(dataConfig.getMaxBatchesPerWorker() > 0, dataConfig.getMaxBatchesPerWorker(), Integer.MaxValue))

'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: while (batchedIterator.hasNext() && miniBatchCount++ < maxMinibatches)
				Do While batchedIterator.MoveNext() AndAlso miniBatchCount++ < maxMinibatches
					If stats Then
						s.logNextDataSetBefore()
					End If
					Dim [next] As MultiDataSet = batchedIterator.Current

					If stats Then
						s.logNextDataSetAfter([next].getFeatures(0).size(0))
					End If

					If stats Then
						s.logProcessMinibatchBefore()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						Dim result As Pair(Of R, SparkTrainingStats) = worker.processMinibatchWithStats([next], net, Not batchedIterator.hasNext())
						s.logProcessMinibatchAfter()
						If result IsNot Nothing Then
							'Terminate training immediately
							s.logReturnTime()
							Dim workerStats As SparkTrainingStats = result.Second
							Dim returnStats As SparkTrainingStats = s.build(workerStats)
							result.First.setStats(returnStats)

							Return Collections.singletonList(result.First).GetEnumerator()
						End If
					Else
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						Dim result As R = worker.processMinibatch([next], net, Not batchedIterator.hasNext())
						If result IsNot Nothing Then
							'Terminate training immediately
							Return Collections.singletonList(result).GetEnumerator()
						End If
					End If
				Loop

				'For some reason, we didn't return already. Normally this shouldn't happen
				If stats Then
					s.logReturnTime()
					Dim pair As Pair(Of R, SparkTrainingStats) = worker.getFinalResultWithStats(net)
					pair.First.setStats(s.build(pair.Second))
					Return Collections.singletonList(pair.First).GetEnumerator()
				Else
					Return Collections.singletonList(worker.getFinalResult(net)).GetEnumerator()
				End If
			Finally
				Nd4j.Executioner.commit()

				'Make sure we shut down the async thread properly...
				If TypeOf batchedIterator Is AsyncMultiDataSetIterator Then
					DirectCast(batchedIterator, AsyncMultiDataSetIterator).shutdown()
				End If
			End Try
		End Function
	End Class

End Namespace