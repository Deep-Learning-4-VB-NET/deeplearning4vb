Imports System.Collections.Generic
Imports Function2 = org.apache.spark.api.java.function.Function2
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StorageMetaData = org.deeplearning4j.core.storage.StorageMetaData
Imports ThresholdAlgorithmReducer = org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithmReducer
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.spark.parameterserver.accumulation


	Public Class SharedTrainingAccumulationFunction
		Implements Function2(Of SharedTrainingAccumulationTuple, SharedTrainingAccumulationTuple, SharedTrainingAccumulationTuple)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public SharedTrainingAccumulationTuple call(SharedTrainingAccumulationTuple tuple1, SharedTrainingAccumulationTuple tuple2) throws Exception
		Public Overrides Function [call](ByVal tuple1 As SharedTrainingAccumulationTuple, ByVal tuple2 As SharedTrainingAccumulationTuple) As SharedTrainingAccumulationTuple
			' if one of tuples is null - return other one
			If tuple1 Is Nothing Then
				Return tuple2
			ElseIf tuple2 Is Nothing Then
				Return tuple1
			End If

			Dim score As Double = 0.0
			Dim stateView As INDArray = Nothing
			Dim aggregationsCount As Integer = 0
			If tuple1.getUpdaterStateArray() IsNot Nothing AndAlso tuple2.getUpdaterStateArray() IsNot Nothing Then
				' we have multiple state views here. average them
				stateView = tuple1.getUpdaterStateArray().addi(tuple2.getUpdaterStateArray())
			ElseIf tuple1.getUpdaterStateArray() IsNot Nothing OrElse tuple2.getUpdaterStateArray() IsNot Nothing Then
				' only one of state views exists. just use it
				stateView = If(tuple1.getUpdaterStateArray() IsNot Nothing, tuple1.getUpdaterStateArray(), tuple2.getUpdaterStateArray())
			End If

			' we assume that aggregationsCount field is set only for entries that hold updaters state
			aggregationsCount = tuple1.getAggregationsCount() + tuple2.getAggregationsCount()
			score = tuple1.getScoreSum() + tuple2.getScoreSum()

			' aggregating spark stats
			Dim stats As SparkTrainingStats = tuple1.getSparkTrainingStats()
			If tuple2.getSparkTrainingStats() IsNot Nothing Then
				If stats Is Nothing Then
					stats = tuple2.getSparkTrainingStats()
				Else
					stats.addOtherTrainingStats(tuple2.getSparkTrainingStats())
				End If
			End If

			Nd4j.Executioner.commit()

			Dim listenerMetaData As ICollection(Of StorageMetaData) = tuple1.getListenerMetaData()
			If listenerMetaData Is Nothing Then
				listenerMetaData = tuple2.getListenerMetaData()
			Else
				Dim newMeta As ICollection(Of StorageMetaData) = tuple2.getListenerMetaData()
				If newMeta IsNot Nothing Then
					listenerMetaData.addAll(newMeta)
				End If
			End If

			Dim listenerStaticInfo As ICollection(Of Persistable) = tuple1.getListenerStaticInfo()
			If listenerStaticInfo Is Nothing Then
				listenerStaticInfo = tuple2.getListenerStaticInfo()
			Else
				Dim newStatic As ICollection(Of Persistable) = tuple2.getListenerStaticInfo()
				If newStatic IsNot Nothing Then
					listenerStaticInfo.addAll(newStatic)
				End If
			End If

			Dim listenerUpdates As ICollection(Of Persistable) = tuple1.getListenerUpdates()
			If listenerUpdates Is Nothing Then
				listenerUpdates = tuple2.getListenerUpdates()
			Else
				Dim listenerUpdates2 As ICollection(Of Persistable) = tuple2.getListenerUpdates()
				If listenerUpdates2 IsNot Nothing Then
					listenerUpdates.addAll(listenerUpdates2)
				End If
			End If

			Dim minibatchesPerExecutor As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()
			If tuple1.getMinibatchesPerExecutor() IsNot Nothing Then
				For Each e As KeyValuePair(Of String, Integer) In tuple1.getMinibatchesPerExecutor().entrySet()
					minibatchesPerExecutor(e.Key) = e.Value
				Next e
			End If
			If tuple2.getMinibatchesPerExecutor() IsNot Nothing Then
				For Each e As KeyValuePair(Of String, Integer) In tuple2.getMinibatchesPerExecutor().entrySet()
					If minibatchesPerExecutor.ContainsKey(e.Key) Then
						minibatchesPerExecutor(e.Key) = minibatchesPerExecutor(e.Key) + e.Value
					Else
						minibatchesPerExecutor(e.Key) = e.Value
					End If
				Next e
			End If

			Dim thresholdAlgorithmReducer As ThresholdAlgorithmReducer = Nothing
			If tuple1.getThresholdAlgorithmReducer() IsNot Nothing Then
				thresholdAlgorithmReducer = tuple1.getThresholdAlgorithmReducer()
			End If
			If tuple2.getThresholdAlgorithmReducer() IsNot Nothing Then
				If thresholdAlgorithmReducer Is Nothing Then
					thresholdAlgorithmReducer = tuple2.getThresholdAlgorithmReducer()
				Else
					'Merge threshold algorithm reducers
					thresholdAlgorithmReducer = thresholdAlgorithmReducer.merge(tuple2.getThresholdAlgorithmReducer())
				End If
			End If

			Return SharedTrainingAccumulationTuple.builder().scoreSum(score).updaterStateArray(stateView).aggregationsCount(aggregationsCount).sparkTrainingStats(stats).listenerMetaData(listenerMetaData).listenerUpdates(listenerUpdates).listenerStaticInfo(listenerStaticInfo).minibatchesPerExecutor(minibatchesPerExecutor).thresholdAlgorithmReducer(thresholdAlgorithmReducer).build()
		End Function
	End Class

End Namespace