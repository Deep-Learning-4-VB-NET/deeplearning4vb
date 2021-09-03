Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports Data = lombok.Data
Imports SparkContext = org.apache.spark.SparkContext
Imports CommonSparkTrainingStats = org.deeplearning4j.spark.api.stats.CommonSparkTrainingStats
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
Imports BaseEventStats = org.deeplearning4j.spark.stats.BaseEventStats
Imports EventStats = org.deeplearning4j.spark.stats.EventStats
Imports ExampleCountEventStats = org.deeplearning4j.spark.stats.ExampleCountEventStats
Imports StatsUtils = org.deeplearning4j.spark.stats.StatsUtils
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

Namespace org.deeplearning4j.spark.impl.paramavg.stats


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class ParameterAveragingTrainingWorkerStats implements org.deeplearning4j.spark.api.stats.SparkTrainingStats
	<Serializable>
	Public Class ParameterAveragingTrainingWorkerStats
		Implements SparkTrainingStats

		Public Const DEFAULT_DELIMITER As String = CommonSparkTrainingStats.DEFAULT_DELIMITER
		Public Const FILENAME_BROADCAST_GET_STATS As String = "parameterAveragingWorkerBroadcastGetValueTimeMs.txt"
		Public Const FILENAME_INIT_STATS As String = "parameterAveragingWorkerInitTimeMs.txt"
		Public Const FILENAME_FIT_STATS As String = "parameterAveragingWorkerFitTimesMs.txt"

		Private parameterAveragingWorkerBroadcastGetValueTimeMs As IList(Of EventStats)
		Private parameterAveragingWorkerInitTimeMs As IList(Of EventStats)
		Private parameterAveragingWorkerFitTimesMs As IList(Of EventStats)

		Public Const PARAMETER_AVERAGING_WORKER_BROADCAST_GET_VALUE_TIME_MS As String = "ParameterAveragingWorkerBroadcastGetValueTimeMs"
		Public Const PARAMETER_AVERAGING_WORKER_INIT_TIME_MS As String = "ParameterAveragingWorkerInitTimeMs"
		Public Const PARAMETER_AVERAGING_WORKER_FIT_TIMES_MS As String = "ParameterAveragingWorkerFitTimesMs"
		Private Shared columnNames As ISet(Of String) = Collections.unmodifiableSet(New LinkedHashSet(Of String)(java.util.Arrays.asList(PARAMETER_AVERAGING_WORKER_BROADCAST_GET_VALUE_TIME_MS, PARAMETER_AVERAGING_WORKER_INIT_TIME_MS, PARAMETER_AVERAGING_WORKER_FIT_TIMES_MS)))

		Public Sub New(ByVal parameterAveragingWorkerBroadcastGetValueTimeMs As IList(Of EventStats), ByVal parameterAveragingWorkerInitTimeMs As IList(Of EventStats), ByVal parameterAveragingWorkerFitTimesMs As IList(Of EventStats))
			Me.parameterAveragingWorkerBroadcastGetValueTimeMs = parameterAveragingWorkerBroadcastGetValueTimeMs
			Me.parameterAveragingWorkerInitTimeMs = parameterAveragingWorkerInitTimeMs
			Me.parameterAveragingWorkerFitTimesMs = parameterAveragingWorkerFitTimesMs
		End Sub

		Public Overridable ReadOnly Property KeySet As ISet(Of String)
			Get
				Return columnNames
			End Get
		End Property

		Public Overridable Function getValue(ByVal key As String) As IList(Of EventStats)
			Select Case key
				Case PARAMETER_AVERAGING_WORKER_BROADCAST_GET_VALUE_TIME_MS
					Return parameterAveragingWorkerBroadcastGetValueTimeMs
				Case PARAMETER_AVERAGING_WORKER_INIT_TIME_MS
					Return parameterAveragingWorkerInitTimeMs
				Case PARAMETER_AVERAGING_WORKER_FIT_TIMES_MS
					Return parameterAveragingWorkerFitTimesMs
				Case Else
					Throw New System.ArgumentException("Unknown key: """ & key & """")
			End Select
		End Function

		Public Overridable Function getShortNameForKey(ByVal key As String) As String Implements SparkTrainingStats.getShortNameForKey
			Select Case key
				Case PARAMETER_AVERAGING_WORKER_BROADCAST_GET_VALUE_TIME_MS
					Return "BroadcastGet"
				Case PARAMETER_AVERAGING_WORKER_INIT_TIME_MS
					Return "ModelInit"
				Case PARAMETER_AVERAGING_WORKER_FIT_TIMES_MS
					Return "Fit"
				Case Else
					Throw New System.ArgumentException("Unknown key: """ & key & """")
			End Select
		End Function

		Public Overridable Function defaultIncludeInPlots(ByVal key As String) As Boolean Implements SparkTrainingStats.defaultIncludeInPlots
			Select Case key
				Case PARAMETER_AVERAGING_WORKER_BROADCAST_GET_VALUE_TIME_MS, PARAMETER_AVERAGING_WORKER_INIT_TIME_MS, PARAMETER_AVERAGING_WORKER_FIT_TIMES_MS
					Return True
				Case Else
					Throw New System.ArgumentException("Unknown key: """ & key & """")
			End Select
		End Function

		Public Overridable Sub addOtherTrainingStats(ByVal other As SparkTrainingStats)
			If Not (TypeOf other Is ParameterAveragingTrainingWorkerStats) Then
				Throw New System.ArgumentException("Cannot merge ParameterAveragingTrainingWorkerStats with " & (If(other IsNot Nothing, other.GetType(), Nothing)))
			End If

			Dim o As ParameterAveragingTrainingWorkerStats = DirectCast(other, ParameterAveragingTrainingWorkerStats)

			CType(Me.parameterAveragingWorkerBroadcastGetValueTimeMs, List(Of EventStats)).AddRange(o.parameterAveragingWorkerBroadcastGetValueTimeMs)
			CType(Me.parameterAveragingWorkerInitTimeMs, List(Of EventStats)).AddRange(o.parameterAveragingWorkerInitTimeMs)
			CType(Me.parameterAveragingWorkerFitTimesMs, List(Of EventStats)).AddRange(o.parameterAveragingWorkerFitTimesMs)
		End Sub

		Public Overridable ReadOnly Property NestedTrainingStats As SparkTrainingStats
			Get
				Return Nothing
			End Get
		End Property

		Public Overridable Function statsAsString() As String Implements SparkTrainingStats.statsAsString
			Dim sb As New StringBuilder()
			Dim f As String = SparkTrainingStats.DEFAULT_PRINT_FORMAT

			sb.Append(String.format(f, PARAMETER_AVERAGING_WORKER_BROADCAST_GET_VALUE_TIME_MS))
			If parameterAveragingWorkerBroadcastGetValueTimeMs Is Nothing Then
				sb.Append("-" & vbLf)
			Else
				sb.Append(StatsUtils.getDurationAsString(parameterAveragingWorkerBroadcastGetValueTimeMs, ",")).Append(vbLf)
			End If

			sb.Append(String.format(f, PARAMETER_AVERAGING_WORKER_INIT_TIME_MS))
			If parameterAveragingWorkerInitTimeMs Is Nothing Then
				sb.Append("-" & vbLf)
			Else
				sb.Append(StatsUtils.getDurationAsString(parameterAveragingWorkerInitTimeMs, ",")).Append(vbLf)
			End If

			sb.Append(String.format(f, PARAMETER_AVERAGING_WORKER_FIT_TIMES_MS))
			If parameterAveragingWorkerFitTimesMs Is Nothing Then
				sb.Append("-" & vbLf)
			Else
				sb.Append(StatsUtils.getDurationAsString(parameterAveragingWorkerFitTimesMs, ",")).Append(vbLf)
			End If

			Return sb.ToString()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void exportStatFiles(String outputPath, org.apache.spark.SparkContext sc) throws java.io.IOException
		Public Overridable Sub exportStatFiles(ByVal outputPath As String, ByVal sc As SparkContext) Implements SparkTrainingStats.exportStatFiles
			Dim d As String = DEFAULT_DELIMITER

			'Broadcast get time:
			StatsUtils.exportStats(parameterAveragingWorkerBroadcastGetValueTimeMs, outputPath, FILENAME_BROADCAST_GET_STATS, d, sc)

			'Network init time:
			StatsUtils.exportStats(parameterAveragingWorkerInitTimeMs, outputPath, FILENAME_INIT_STATS, d, sc)

			'Network fit time:
			StatsUtils.exportStats(parameterAveragingWorkerFitTimesMs, outputPath, FILENAME_FIT_STATS, d, sc)
		End Sub

		Public Class ParameterAveragingTrainingWorkerStatsHelper
			Friend broadcastStartTime As Long
			Friend broadcastEndTime As Long
			Friend initEndTime As Long
			Friend lastFitStartTime As Long
			'TODO replace with fast int collection (no boxing)
			Friend fitTimes As IList(Of EventStats) = New List(Of EventStats)()

			Friend ReadOnly timeSource As TimeSource = TimeSourceProvider.Instance


			Public Overridable Sub logBroadcastGetValueStart()
				broadcastStartTime = timeSource.currentTimeMillis()
			End Sub

			Public Overridable Sub logBroadcastGetValueEnd()
				broadcastEndTime = timeSource.currentTimeMillis()
			End Sub

			Public Overridable Sub logInitEnd()
				initEndTime = timeSource.currentTimeMillis()
			End Sub

			Public Overridable Sub logFitStart()
				lastFitStartTime = timeSource.currentTimeMillis()
			End Sub

			Public Overridable Sub logFitEnd(ByVal numExamples As Long)
				Dim now As Long = timeSource.currentTimeMillis()
				fitTimes.Add(New ExampleCountEventStats(lastFitStartTime, now - lastFitStartTime, numExamples))
			End Sub

			Public Overridable Function build() As ParameterAveragingTrainingWorkerStats
				'Using ArrayList not Collections.singletonList() etc so we can add to them later (during merging)
				Dim bList As IList(Of EventStats) = New List(Of EventStats)()
				bList.Add(New BaseEventStats(broadcastStartTime, broadcastEndTime - broadcastStartTime))
				Dim initList As IList(Of EventStats) = New List(Of EventStats)()
				initList.Add(New BaseEventStats(broadcastEndTime, initEndTime - broadcastEndTime)) 'Init starts at same time that broadcast ends

				Return New ParameterAveragingTrainingWorkerStats(bList, initList, fitTimes)
			End Function
		End Class
	End Class

End Namespace