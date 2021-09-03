Imports System.Collections.Generic
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports AsyncDataSetIterator = org.nd4j.linalg.dataset.AsyncDataSetIterator
Imports IteratorDataSetIterator = org.deeplearning4j.datasets.iterator.IteratorDataSetIterator
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports TrainingResult = org.deeplearning4j.spark.api.TrainingResult
Imports org.deeplearning4j.spark.api
Imports WorkerConfiguration = org.deeplearning4j.spark.api.WorkerConfiguration
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
Imports StatsCalculationHelper = org.deeplearning4j.spark.api.stats.StatsCalculationHelper
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
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


	Public Class ExecuteWorkerFlatMap(Of R As org.deeplearning4j.spark.api.TrainingResult)
		Implements FlatMapFunction(Of IEnumerator(Of DataSet), R)

		Private ReadOnly worker As TrainingWorker(Of R)

		Public Sub New(ByVal worker As TrainingWorker(Of R))
			Me.worker = worker
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<R> call(java.util.Iterator<org.nd4j.linalg.dataset.DataSet> dataSetIterator) throws Exception
		Public Overrides Function [call](ByVal dataSetIterator As IEnumerator(Of DataSet)) As IEnumerator(Of R)
			Dim dataConfig As WorkerConfiguration = worker.DataConfiguration
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final boolean isGraph = dataConfig.isGraphNetwork();
			Dim isGraph As Boolean = dataConfig.isGraphNetwork()

			Dim stats As Boolean = dataConfig.isCollectTrainingStats()
			Dim s As StatsCalculationHelper = (If(stats, New StatsCalculationHelper(), Nothing))
			If stats Then
				s.logMethodStartTime()
			End If

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not dataSetIterator.hasNext() Then
				If stats Then
					s.logReturnTime()

					Dim pair As Pair(Of R, SparkTrainingStats) = worker.getFinalResultNoDataWithStats()
					pair.First.setStats(s.build(pair.Second))
					Return Collections.singletonList(pair.First).GetEnumerator()
				Else
					Return Collections.singletonList(worker.FinalResultNoData).GetEnumerator()
				End If
			End If

			Dim batchSize As Integer = dataConfig.getBatchSizePerWorker()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int prefetchCount = dataConfig.getPrefetchNumBatches();
			Dim prefetchCount As Integer = dataConfig.getPrefetchNumBatches()

			Dim batchedIterator As DataSetIterator = New IteratorDataSetIterator(dataSetIterator, batchSize)
			If prefetchCount > 0 Then
				batchedIterator = New AsyncDataSetIterator(batchedIterator, prefetchCount)
			End If

			Try
				Dim net As MultiLayerNetwork = Nothing
				Dim graph As ComputationGraph = Nothing
				If stats Then
					s.logInitialModelBefore()
				End If
				If isGraph Then
					graph = worker.InitialModelGraph
				Else
					net = worker.InitialModel
				End If
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
					Dim [next] As DataSet = batchedIterator.Current
					If stats Then
						s.logNextDataSetAfter([next].numExamples())
					End If

					If stats Then
						s.logProcessMinibatchBefore()
						Dim result As Pair(Of R, SparkTrainingStats)
						If isGraph Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							result = worker.processMinibatchWithStats([next], graph, Not batchedIterator.hasNext())
						Else
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							result = worker.processMinibatchWithStats([next], net, Not batchedIterator.hasNext())
						End If
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
						Dim result As R
						If isGraph Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							result = worker.processMinibatch([next], graph, Not batchedIterator.hasNext())
						Else
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							result = worker.processMinibatch([next], net, Not batchedIterator.hasNext())
						End If
						If result IsNot Nothing Then
							'Terminate training immediately
							Return Collections.singletonList(result).GetEnumerator()
						End If
					End If
				Loop

				'For some reason, we didn't return already. Normally this shouldn't happen
				If stats Then
					s.logReturnTime()
					Dim pair As Pair(Of R, SparkTrainingStats)
					If isGraph Then
						pair = worker.getFinalResultWithStats(graph)
					Else
						pair = worker.getFinalResultWithStats(net)
					End If
					pair.First.setStats(s.build(pair.Second))
					Return Collections.singletonList(pair.First).GetEnumerator()
				Else
					If isGraph Then
						Return Collections.singletonList(worker.getFinalResult(graph)).GetEnumerator()
					Else
						Return Collections.singletonList(worker.getFinalResult(net)).GetEnumerator()
					End If
				End If
			Finally
				'Make sure we shut down the async thread properly...
				Nd4j.Executioner.commit()

				If TypeOf batchedIterator Is AsyncDataSetIterator Then
					DirectCast(batchedIterator, AsyncDataSetIterator).shutdown()
				End If
			End Try
		End Function
	End Class

End Namespace