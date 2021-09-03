Imports System.Collections.Generic
Imports org.deeplearning4j.spark.api.worker
Imports org.deeplearning4j.spark.api.worker
Imports BaseEventStats = org.deeplearning4j.spark.stats.BaseEventStats
Imports EventStats = org.deeplearning4j.spark.stats.EventStats
Imports ExampleCountEventStats = org.deeplearning4j.spark.stats.ExampleCountEventStats
Imports TimeSource = org.deeplearning4j.spark.time.TimeSource
Imports TimeSourceProvider = org.deeplearning4j.spark.time.TimeSourceProvider

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

Namespace org.deeplearning4j.spark.api.stats


	Public Class StatsCalculationHelper
		Private methodStartTime As Long
		Private returnTime As Long
		Private initalModelBefore As Long
		Private initialModelAfter As Long
		Private lastDataSetBefore As Long
		Private lastProcessBefore As Long
		Private totalExampleCount As Long
		Private dataSetGetTimes As IList(Of EventStats) = New List(Of EventStats)()
		Private processMiniBatchTimes As IList(Of EventStats) = New List(Of EventStats)()

		Private timeSource As TimeSource = TimeSourceProvider.Instance

		Public Overridable Sub logMethodStartTime()
			methodStartTime = timeSource.currentTimeMillis()
		End Sub

		Public Overridable Sub logReturnTime()
			returnTime = timeSource.currentTimeMillis()
		End Sub

		Public Overridable Sub logInitialModelBefore()
			initalModelBefore = timeSource.currentTimeMillis()
		End Sub

		Public Overridable Sub logInitialModelAfter()
			initialModelAfter = timeSource.currentTimeMillis()
		End Sub

		Public Overridable Sub logNextDataSetBefore()
			lastDataSetBefore = timeSource.currentTimeMillis()
		End Sub

		Public Overridable Sub logNextDataSetAfter(ByVal numExamples As Long)
			Dim now As Long = timeSource.currentTimeMillis()
			Dim duration As Long = now - lastDataSetBefore
			dataSetGetTimes.Add(New BaseEventStats(lastDataSetBefore, duration))
			totalExampleCount += numExamples
		End Sub

		Public Overridable Sub logProcessMinibatchBefore()
			lastProcessBefore = timeSource.currentTimeMillis()
		End Sub

		Public Overridable Sub logProcessMinibatchAfter()
			Dim now As Long = timeSource.currentTimeMillis()
			Dim duration As Long = now - lastProcessBefore
			processMiniBatchTimes.Add(New BaseEventStats(lastProcessBefore, duration))
		End Sub

		Public Overridable Function build(ByVal masterSpecificStats As SparkTrainingStats) As CommonSparkTrainingStats

			Dim totalTime As IList(Of EventStats) = New List(Of EventStats)()
			totalTime.Add(New ExampleCountEventStats(methodStartTime, returnTime - methodStartTime, totalExampleCount))
			Dim initTime As IList(Of EventStats) = New List(Of EventStats)()
			initTime.Add(New BaseEventStats(initalModelBefore, initialModelAfter - initalModelBefore))

			Return (New CommonSparkTrainingStats.Builder()).trainingMasterSpecificStats(masterSpecificStats).workerFlatMapTotalTimeMs(totalTime).workerFlatMapGetInitialModelTimeMs(initTime).workerFlatMapDataSetGetTimesMs(dataSetGetTimes).workerFlatMapProcessMiniBatchTimesMs(processMiniBatchTimes).build()
		End Function
	End Class

End Namespace