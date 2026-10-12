Imports System.Collections.Generic
Imports Function2 = org.apache.spark.api.java.function.Function2
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StorageMetaData = org.deeplearning4j.core.storage.StorageMetaData
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

Namespace org.deeplearning4j.spark.impl.paramavg.aggregator


	Public Class ParameterAveragingElementCombineFunction
		Implements Function2(Of ParameterAveragingAggregationTuple, ParameterAveragingAggregationTuple, ParameterAveragingAggregationTuple)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public ParameterAveragingAggregationTuple call(ParameterAveragingAggregationTuple v1, ParameterAveragingAggregationTuple v2) throws Exception
		Public Overrides Function [call](ByVal v1 As ParameterAveragingAggregationTuple, ByVal v2 As ParameterAveragingAggregationTuple) As ParameterAveragingAggregationTuple
			If v1 Is Nothing Then
				Return v2
			ElseIf v2 Is Nothing Then
				Return v1
			End If

			'Handle edge case of less data than executors: in this case, one (or both) of v1 and v2 might not have any contents...
			If v1.getParametersSum() Is Nothing Then
				Return v2
			ElseIf v2.getParametersSum() Is Nothing Then
				Return v1
			End If

			Dim newParams As INDArray = v1.getParametersSum().addi(v2.getParametersSum())
			Dim updaterStateSum As INDArray
			If v1.getUpdaterStateSum() Is Nothing Then
				updaterStateSum = v2.getUpdaterStateSum()
			Else
				updaterStateSum = v1.getUpdaterStateSum()
				If v2.getUpdaterStateSum() IsNot Nothing Then
					updaterStateSum.addi(v2.getUpdaterStateSum())
				End If
			End If


			Dim scoreSum As Double = v1.getScoreSum() + v2.getScoreSum()
			Dim aggregationCount As Integer = v1.getAggregationsCount() + v2.getAggregationsCount()

			Dim stats As SparkTrainingStats = v1.getSparkTrainingStats()
			If v2.getSparkTrainingStats() IsNot Nothing Then
				If stats Is Nothing Then
					stats = v2.getSparkTrainingStats()
				Else
					stats.addOtherTrainingStats(v2.getSparkTrainingStats())
				End If
			End If

			Nd4j.Executioner.commit()

			Dim listenerMetaData As ICollection(Of StorageMetaData) = v1.getListenerMetaData()
			If listenerMetaData Is Nothing Then
				listenerMetaData = v2.getListenerMetaData()
			Else
				Dim newMeta As ICollection(Of StorageMetaData) = v2.getListenerMetaData()
				If newMeta IsNot Nothing Then
					listenerMetaData.addAll(newMeta)
				End If
			End If

			Dim listenerStaticInfo As ICollection(Of Persistable) = v1.getListenerStaticInfo()
			If listenerStaticInfo Is Nothing Then
				listenerStaticInfo = v2.getListenerStaticInfo()
			Else
				Dim newStatic As ICollection(Of Persistable) = v2.getListenerStaticInfo()
				If newStatic IsNot Nothing Then
					listenerStaticInfo.addAll(newStatic)
				End If
			End If

			Dim listenerUpdates As ICollection(Of Persistable) = v1.getListenerUpdates()
			If listenerUpdates Is Nothing Then
				listenerUpdates = v2.getListenerUpdates()
			Else
				Dim listenerUpdates2 As ICollection(Of Persistable) = v2.getListenerUpdates()
				If listenerUpdates2 IsNot Nothing Then
					listenerUpdates.addAll(listenerUpdates2)
				End If
			End If

			Return New ParameterAveragingAggregationTuple(newParams, updaterStateSum, scoreSum, aggregationCount, stats, listenerMetaData, listenerStaticInfo, listenerUpdates)
		End Function
	End Class

End Namespace