Imports System.Collections.Generic
Imports Function2 = org.apache.spark.api.java.function.Function2
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StorageMetaData = org.deeplearning4j.core.storage.StorageMetaData
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
Imports ParameterAveragingTrainingResult = org.deeplearning4j.spark.impl.paramavg.ParameterAveragingTrainingResult
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

Namespace org.deeplearning4j.spark.impl.paramavg.aggregator


	Public Class ParameterAveragingElementAddFunction
		Implements Function2(Of ParameterAveragingAggregationTuple, ParameterAveragingTrainingResult, ParameterAveragingAggregationTuple)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public ParameterAveragingAggregationTuple call(ParameterAveragingAggregationTuple tuple, org.deeplearning4j.spark.impl.paramavg.ParameterAveragingTrainingResult result) throws Exception
		Public Overrides Function [call](ByVal tuple As ParameterAveragingAggregationTuple, ByVal result As ParameterAveragingTrainingResult) As ParameterAveragingAggregationTuple
			If tuple Is Nothing Then
				Return ParameterAveragingAggregationTuple.builder().parametersSum(result.getParameters()).updaterStateSum(result.getUpdaterState()).scoreSum(result.getScore()).aggregationsCount(1).sparkTrainingStats(result.getSparkTrainingStats()).listenerMetaData(result.getListenerMetaData()).listenerStaticInfo(result.getListenerStaticInfo()).listenerUpdates(result.getListenerUpdates()).build()
			End If

			Dim params As INDArray = tuple.getParametersSum().addi(result.getParameters())
			Dim updaterStateSum As INDArray
			If tuple.getUpdaterStateSum() Is Nothing Then
				updaterStateSum = result.getUpdaterState()
			Else
				updaterStateSum = tuple.getUpdaterStateSum()
				If result.getUpdaterState() IsNot Nothing Then
					updaterStateSum.addi(result.getUpdaterState())
				End If
			End If

			Dim scoreSum As Double = tuple.getScoreSum() + result.getScore()
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
				Dim newStatic As ICollection(Of Persistable) = tuple.getListenerStaticInfo()
				If newStatic IsNot Nothing Then
					listenerStaticInfo.addAll(newStatic)
				End If
			End If

			Dim listenerUpdates As ICollection(Of Persistable) = tuple.getListenerUpdates()
			If listenerUpdates Is Nothing Then
				listenerUpdates = result.getListenerUpdates()
			Else
				Dim newUpdates As ICollection(Of Persistable) = result.getListenerUpdates()
				If newUpdates IsNot Nothing Then
					listenerUpdates.addAll(newUpdates)
				End If
			End If



			Return New ParameterAveragingAggregationTuple(params, updaterStateSum, scoreSum, tuple.getAggregationsCount() + 1, stats, listenerMetaData, listenerStaticInfo, listenerUpdates)
		End Function
	End Class

End Namespace