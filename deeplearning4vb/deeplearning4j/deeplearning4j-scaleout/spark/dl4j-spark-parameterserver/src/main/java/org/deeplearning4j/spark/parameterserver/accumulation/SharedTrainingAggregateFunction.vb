Imports System.Collections.Generic
Imports Function2 = org.apache.spark.api.java.function.Function2
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StorageMetaData = org.deeplearning4j.core.storage.StorageMetaData
Imports ThresholdAlgorithmReducer = org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithmReducer
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
Imports SharedTrainingResult = org.deeplearning4j.spark.parameterserver.training.SharedTrainingResult
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


	Public Class SharedTrainingAggregateFunction
		Implements Function2(Of SharedTrainingAccumulationTuple, SharedTrainingResult, SharedTrainingAccumulationTuple)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public SharedTrainingAccumulationTuple call(SharedTrainingAccumulationTuple tuple, org.deeplearning4j.spark.parameterserver.training.SharedTrainingResult result) throws Exception
		Public Overrides Function [call](ByVal tuple As SharedTrainingAccumulationTuple, ByVal result As SharedTrainingResult) As SharedTrainingAccumulationTuple
			If tuple Is Nothing Then
				Dim tar As ThresholdAlgorithmReducer = Nothing
				If result.getThresholdAlgorithm() IsNot Nothing Then
					tar = result.getThresholdAlgorithm().newReducer()
					tar.add(result.getThresholdAlgorithm())
				End If

				Return SharedTrainingAccumulationTuple.builder().updaterStateArray(result.getUpdaterStateArray()).scoreSum(result.getScoreSum()).listenerStaticInfo(result.getListenerStaticInfo()).listenerUpdates(result.getListenerUpdates()).listenerMetaData(result.getListenerMetaData()).sparkTrainingStats(result.getSparkTrainingStats()).aggregationsCount(result.getAggregationsCount()).minibatchesPerExecutor(result.getMinibatchesPerExecutor()).thresholdAlgorithmReducer(tar).build()
			End If


			Dim updaterStateSum As INDArray = Nothing
			Dim aggregationsCount As Integer = 0
			Dim score As Double = 0.0
			If tuple.getUpdaterStateArray() IsNot Nothing Then
				If result.getUpdaterStateArray() IsNot Nothing Then
					updaterStateSum = tuple.getUpdaterStateArray().addi(result.getUpdaterStateArray())
					aggregationsCount = tuple.getAggregationsCount() + 1
					score = tuple.getScoreSum() + result.getScoreSum()
				End If
			Else
				If result.getUpdaterStateArray() IsNot Nothing Then
					updaterStateSum = result.getUpdaterStateArray()
					aggregationsCount = 1
					score = result.getScoreSum()
				End If
			End If

			Dim stats As SparkTrainingStats = tuple.getSparkTrainingStats()
			If result.getSparkTrainingStats() IsNot Nothing Then
				If stats Is Nothing Then
					stats = result.getSparkTrainingStats()
				Else
					stats.addOtherTrainingStats(result.getSparkTrainingStats())
				End If
			End If

			Nd4j.Executioner.commit()

			Dim listenerMetaData As ICollection(Of StorageMetaData) = tuple.getListenerMetaData()
			If listenerMetaData Is Nothing Then
				listenerMetaData = result.getListenerMetaData()
			Else
				Dim newMeta As ICollection(Of StorageMetaData) = result.getListenerMetaData()
				If newMeta IsNot Nothing Then
					listenerMetaData.addAll(newMeta)
				End If
			End If

			Dim listenerStaticInfo As ICollection(Of Persistable) = tuple.getListenerStaticInfo()
			If listenerStaticInfo Is Nothing Then
				listenerStaticInfo = result.getListenerStaticInfo()
			Else
				Dim newStatic As ICollection(Of Persistable) = result.getListenerStaticInfo()
				If newStatic IsNot Nothing Then
					listenerStaticInfo.addAll(newStatic)
				End If
			End If

			Dim listenerUpdates As ICollection(Of Persistable) = tuple.getListenerUpdates()
			If listenerUpdates Is Nothing Then
				listenerUpdates = result.getListenerUpdates()
			Else
				Dim listenerUpdates2 As ICollection(Of Persistable) = result.getListenerUpdates()
				If listenerUpdates2 IsNot Nothing Then
					listenerUpdates.addAll(listenerUpdates2)
				End If
			End If

			Dim minibatchesPerExecutor As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()
			If tuple.getMinibatchesPerExecutor() IsNot Nothing Then
				For Each e As KeyValuePair(Of String, Integer) In tuple.getMinibatchesPerExecutor().entrySet()
					minibatchesPerExecutor(e.Key) = e.Value
				Next e
			End If
			If result.getMinibatchesPerExecutor() IsNot Nothing Then
				For Each e As KeyValuePair(Of String, Integer) In result.getMinibatchesPerExecutor().entrySet()
					If minibatchesPerExecutor.ContainsKey(e.Key) Then
						minibatchesPerExecutor(e.Key) = minibatchesPerExecutor(e.Key) + e.Value
					Else
						minibatchesPerExecutor(e.Key) = e.Value
					End If
				Next e
			End If

			Dim thresholdAlgorithmReducer As ThresholdAlgorithmReducer = tuple.getThresholdAlgorithmReducer()
			If thresholdAlgorithmReducer Is Nothing AndAlso result.getThresholdAlgorithm() IsNot Nothing Then
				thresholdAlgorithmReducer = result.getThresholdAlgorithm().newReducer()
			End If
			If thresholdAlgorithmReducer IsNot Nothing Then
				thresholdAlgorithmReducer.add(result.getThresholdAlgorithm())
			End If

			Return SharedTrainingAccumulationTuple.builder().scoreSum(score).updaterStateArray(updaterStateSum).aggregationsCount(aggregationsCount).sparkTrainingStats(stats).listenerMetaData(listenerMetaData).listenerUpdates(listenerUpdates).listenerStaticInfo(listenerStaticInfo).minibatchesPerExecutor(minibatchesPerExecutor).thresholdAlgorithmReducer(thresholdAlgorithmReducer).build()
		End Function
	End Class

End Namespace