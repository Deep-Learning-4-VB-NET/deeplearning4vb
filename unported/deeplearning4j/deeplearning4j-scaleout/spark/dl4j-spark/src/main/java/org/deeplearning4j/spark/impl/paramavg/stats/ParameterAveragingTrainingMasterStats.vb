Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports Data = lombok.Data
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports SparkContext = org.apache.spark.SparkContext
Imports CommonSparkTrainingStats = org.deeplearning4j.spark.api.stats.CommonSparkTrainingStats
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
Imports org.deeplearning4j.spark.stats
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
'ORIGINAL LINE: @Data public class ParameterAveragingTrainingMasterStats implements org.deeplearning4j.spark.api.stats.SparkTrainingStats
	<Serializable>
	Public Class ParameterAveragingTrainingMasterStats
		Implements SparkTrainingStats

		Public Const DEFAULT_DELIMITER As String = CommonSparkTrainingStats.DEFAULT_DELIMITER
		Public Const FILENAME_EXPORT_RDD_TIME As String = "parameterAveragingMasterExportTimesMs.txt"
		Public Const FILENAME_COUNT_RDD_SIZE As String = "parameterAveragingMasterCountRddSizeTimesMs.txt"
		Public Const FILENAME_BROADCAST_CREATE As String = "parameterAveragingMasterBroadcastCreateTimesMs.txt"
		Public Const FILENAME_FIT_TIME As String = "parameterAveragingMasterFitTimesMs.txt"
		Public Const FILENAME_SPLIT_TIME As String = "parameterAveragingMasterSplitTimesMs.txt"
		Public Const FILENAME_MAP_PARTITIONS_TIME As String = "parameterAveragingMasterMapPartitionsTimesMs.txt"
		Public Const FILENAME_AGGREGATE_TIME As String = "parameterAveragingMasterAggregateTimesMs.txt"
		Public Const FILENAME_PROCESS_PARAMS_TIME As String = "parameterAveragingMasterProcessParamsUpdaterTimesMs.txt"
		Public Const FILENAME_REPARTITION_STATS As String = "parameterAveragingMasterRepartitionTimesMs.txt"

		Public Const PARAMETER_AVERAGING_MASTER_EXPORT_RDD_TIMES_MS As String = "parameterAveragingMasterExportTimesMs"
		Public Const PARAMETER_AVERAGING_MASTER_COUNT_RDD_TIMES_MS As String = "ParameterAveragingMasterCountRddSizeTimesMs"
		Public Const PARAMETER_AVERAGING_MASTER_BROADCAST_CREATE_TIMES_MS As String = "ParameterAveragingMasterBroadcastCreateTimesMs"
		Public Const PARAMETER_AVERAGING_MASTER_FIT_TIMES_MS As String = "ParameterAveragingMasterFitTimesMs"
		Public Const PARAMETER_AVERAGING_MASTER_SPLIT_TIMES_MS As String = "ParameterAveragingMasterSplitTimesMs"
		Public Const PARAMETER_AVERAGING_MASTER_MAP_PARTITIONS_TIMES_MS As String = "ParameterAveragingMasterMapPartitionsTimesMs"
		Public Const PARAMETER_AVERAGING_MASTER_AGGREGATE_TIMES_MS As String = "ParameterAveragingMasterAggregateTimesMs"
		Public Const PARAMETER_AVERAGING_MASTER_PROCESS_PARAMS_UPDATER_TIMES_MS As String = "ParameterAveragingMasterProcessParamsUpdaterTimesMs"
		Public Const PARAMETER_AVERAGING_MASTER_REPARTITION_TIMES_MS As String = "ParameterAveragingMasterRepartitionTimesMs"

		Private Shared columnNames As ISet(Of String) = Collections.unmodifiableSet(New LinkedHashSet(Of String)(java.util.Arrays.asList(PARAMETER_AVERAGING_MASTER_EXPORT_RDD_TIMES_MS, PARAMETER_AVERAGING_MASTER_COUNT_RDD_TIMES_MS, PARAMETER_AVERAGING_MASTER_BROADCAST_CREATE_TIMES_MS, PARAMETER_AVERAGING_MASTER_FIT_TIMES_MS, PARAMETER_AVERAGING_MASTER_SPLIT_TIMES_MS, PARAMETER_AVERAGING_MASTER_MAP_PARTITIONS_TIMES_MS, PARAMETER_AVERAGING_MASTER_AGGREGATE_TIMES_MS, PARAMETER_AVERAGING_MASTER_PROCESS_PARAMS_UPDATER_TIMES_MS, PARAMETER_AVERAGING_MASTER_REPARTITION_TIMES_MS)))

		Private workerStats As SparkTrainingStats
		Private parameterAveragingMasterExportTimesMs As IList(Of EventStats)
		Private parameterAveragingMasterCountRddSizeTimesMs As IList(Of EventStats)
		Private parameterAveragingMasterBroadcastCreateTimesMs As IList(Of EventStats)
		Private parameterAveragingMasterFitTimesMs As IList(Of EventStats)
		Private parameterAveragingMasterSplitTimesMs As IList(Of EventStats)
		Private parameterAveragingMasterMapPartitionsTimesMs As IList(Of EventStats)
		Private paramaterAveragingMasterAggregateTimesMs As IList(Of EventStats)
		Private parameterAveragingMasterProcessParamsUpdaterTimesMs As IList(Of EventStats)
		Private parameterAveragingMasterRepartitionTimesMs As IList(Of EventStats)


		Public Sub New(ByVal workerStats As SparkTrainingStats, ByVal parameterAveragingMasterExportTimesMs As IList(Of EventStats), ByVal parameterAveragingMasterCountRddSizeTimesMs As IList(Of EventStats), ByVal parameterAveragingMasterBroadcastCreateTimeMs As IList(Of EventStats), ByVal parameterAveragingMasterFitTimeMs As IList(Of EventStats), ByVal parameterAveragingMasterSplitTimeMs As IList(Of EventStats), ByVal parameterAveragingMasterMapPartitionsTimesMs As IList(Of EventStats), ByVal parameterAveragingMasterAggregateTimesMs As IList(Of EventStats), ByVal parameterAveragingMasterProcessParamsUpdaterTimesMs As IList(Of EventStats), ByVal parameterAveragingMasterRepartitionTimesMs As IList(Of EventStats))
			Me.workerStats = workerStats
			Me.parameterAveragingMasterExportTimesMs = parameterAveragingMasterExportTimesMs
			Me.parameterAveragingMasterCountRddSizeTimesMs = parameterAveragingMasterCountRddSizeTimesMs
			Me.parameterAveragingMasterBroadcastCreateTimesMs = parameterAveragingMasterBroadcastCreateTimeMs
			Me.parameterAveragingMasterFitTimesMs = parameterAveragingMasterFitTimeMs
			Me.parameterAveragingMasterSplitTimesMs = parameterAveragingMasterSplitTimeMs
			Me.parameterAveragingMasterMapPartitionsTimesMs = parameterAveragingMasterMapPartitionsTimesMs
			Me.paramaterAveragingMasterAggregateTimesMs = parameterAveragingMasterAggregateTimesMs
			Me.parameterAveragingMasterProcessParamsUpdaterTimesMs = parameterAveragingMasterProcessParamsUpdaterTimesMs
			Me.parameterAveragingMasterRepartitionTimesMs = parameterAveragingMasterRepartitionTimesMs
		End Sub


		Public Overridable ReadOnly Property KeySet As ISet(Of String)
			Get
				Dim [out] As ISet(Of String) = New LinkedHashSet(Of String)(columnNames)
				If workerStats IsNot Nothing Then
					[out].addAll(workerStats.getKeySet())
				End If
				Return [out]
			End Get
		End Property

		Public Overridable Function getValue(ByVal key As String) As IList(Of EventStats)
			Select Case key
				Case PARAMETER_AVERAGING_MASTER_EXPORT_RDD_TIMES_MS
					Return parameterAveragingMasterExportTimesMs
				Case PARAMETER_AVERAGING_MASTER_COUNT_RDD_TIMES_MS
					Return parameterAveragingMasterCountRddSizeTimesMs
				Case PARAMETER_AVERAGING_MASTER_BROADCAST_CREATE_TIMES_MS
					Return parameterAveragingMasterBroadcastCreateTimesMs
				Case PARAMETER_AVERAGING_MASTER_FIT_TIMES_MS
					Return parameterAveragingMasterFitTimesMs
				Case PARAMETER_AVERAGING_MASTER_SPLIT_TIMES_MS
					Return parameterAveragingMasterSplitTimesMs
				Case PARAMETER_AVERAGING_MASTER_MAP_PARTITIONS_TIMES_MS
					Return parameterAveragingMasterMapPartitionsTimesMs
				Case PARAMETER_AVERAGING_MASTER_AGGREGATE_TIMES_MS
					Return paramaterAveragingMasterAggregateTimesMs
				Case PARAMETER_AVERAGING_MASTER_PROCESS_PARAMS_UPDATER_TIMES_MS
					Return parameterAveragingMasterProcessParamsUpdaterTimesMs
				Case PARAMETER_AVERAGING_MASTER_REPARTITION_TIMES_MS
					Return parameterAveragingMasterRepartitionTimesMs
				Case Else
					If workerStats IsNot Nothing Then
						Return workerStats.getValue(key)
					End If
					Throw New System.ArgumentException("Unknown key: """ & key & """")
			End Select
		End Function

		Public Overridable Function getShortNameForKey(ByVal key As String) As String Implements SparkTrainingStats.getShortNameForKey
			Select Case key
				Case PARAMETER_AVERAGING_MASTER_EXPORT_RDD_TIMES_MS
					Return "Export"
				Case PARAMETER_AVERAGING_MASTER_COUNT_RDD_TIMES_MS
					Return "CountRDD"
				Case PARAMETER_AVERAGING_MASTER_BROADCAST_CREATE_TIMES_MS
					Return "CreateBroadcast"
				Case PARAMETER_AVERAGING_MASTER_FIT_TIMES_MS
					Return "Fit"
				Case PARAMETER_AVERAGING_MASTER_SPLIT_TIMES_MS
					Return "Split"
				Case PARAMETER_AVERAGING_MASTER_MAP_PARTITIONS_TIMES_MS
					Return "MapPart"
				Case PARAMETER_AVERAGING_MASTER_AGGREGATE_TIMES_MS
					Return "Aggregate"
				Case PARAMETER_AVERAGING_MASTER_PROCESS_PARAMS_UPDATER_TIMES_MS
					Return "ProcessParams"
				Case PARAMETER_AVERAGING_MASTER_REPARTITION_TIMES_MS
					Return "Repartition"
				Case Else
					If workerStats IsNot Nothing Then
						Return workerStats.getShortNameForKey(key)
					End If
					Throw New System.ArgumentException("Unknown key: """ & key & """")
			End Select
		End Function

		Public Overridable Function defaultIncludeInPlots(ByVal key As String) As Boolean Implements SparkTrainingStats.defaultIncludeInPlots
			Select Case key
				Case PARAMETER_AVERAGING_MASTER_FIT_TIMES_MS, PARAMETER_AVERAGING_MASTER_MAP_PARTITIONS_TIMES_MS
					Return False
				Case PARAMETER_AVERAGING_MASTER_EXPORT_RDD_TIMES_MS, PARAMETER_AVERAGING_MASTER_COUNT_RDD_TIMES_MS, PARAMETER_AVERAGING_MASTER_SPLIT_TIMES_MS, PARAMETER_AVERAGING_MASTER_BROADCAST_CREATE_TIMES_MS, PARAMETER_AVERAGING_MASTER_AGGREGATE_TIMES_MS, PARAMETER_AVERAGING_MASTER_PROCESS_PARAMS_UPDATER_TIMES_MS, PARAMETER_AVERAGING_MASTER_REPARTITION_TIMES_MS
					Return True
				Case Else
					If workerStats IsNot Nothing Then
						Return workerStats.defaultIncludeInPlots(key)
					End If
					Return False
			End Select
		End Function

		Public Overridable Sub addOtherTrainingStats(ByVal other As SparkTrainingStats)
			If Not (TypeOf other Is ParameterAveragingTrainingMasterStats) Then
				Throw New System.ArgumentException("Expected ParameterAveragingTrainingMasterStats, got " & (If(other IsNot Nothing, other.GetType(), Nothing)))
			End If

			Dim o As ParameterAveragingTrainingMasterStats = DirectCast(other, ParameterAveragingTrainingMasterStats)

			If workerStats IsNot Nothing Then
				If o.workerStats IsNot Nothing Then
					workerStats.addOtherTrainingStats(o.workerStats)
				End If
			Else
				If o.workerStats IsNot Nothing Then
					workerStats = o.workerStats
				End If
			End If

			CType(Me.parameterAveragingMasterExportTimesMs, List(Of EventStats)).AddRange(o.parameterAveragingMasterExportTimesMs)
			CType(Me.parameterAveragingMasterCountRddSizeTimesMs, List(Of EventStats)).AddRange(o.parameterAveragingMasterCountRddSizeTimesMs)
			CType(Me.parameterAveragingMasterBroadcastCreateTimesMs, List(Of EventStats)).AddRange(o.parameterAveragingMasterBroadcastCreateTimesMs)
			CType(Me.parameterAveragingMasterRepartitionTimesMs, List(Of EventStats)).AddRange(o.parameterAveragingMasterRepartitionTimesMs)
			CType(Me.parameterAveragingMasterFitTimesMs, List(Of EventStats)).AddRange(o.parameterAveragingMasterFitTimesMs)
			If parameterAveragingMasterRepartitionTimesMs Is Nothing Then
				If o.parameterAveragingMasterRepartitionTimesMs IsNot Nothing Then
					parameterAveragingMasterRepartitionTimesMs = o.parameterAveragingMasterRepartitionTimesMs
				End If
			Else
				If o.parameterAveragingMasterRepartitionTimesMs IsNot Nothing Then
					CType(parameterAveragingMasterRepartitionTimesMs, List(Of EventStats)).AddRange(o.parameterAveragingMasterRepartitionTimesMs)
				End If
			End If
		End Sub

		Public Overridable ReadOnly Property NestedTrainingStats As SparkTrainingStats
			Get
				Return workerStats
			End Get
		End Property

		Public Overridable Function statsAsString() As String Implements SparkTrainingStats.statsAsString
			Dim sb As New StringBuilder()
			Dim f As String = SparkTrainingStats.DEFAULT_PRINT_FORMAT

			sb.Append(String.format(f, PARAMETER_AVERAGING_MASTER_EXPORT_RDD_TIMES_MS))
			If parameterAveragingMasterExportTimesMs Is Nothing Then
				sb.Append("-" & vbLf)
			Else
				sb.Append(StatsUtils.getDurationAsString(parameterAveragingMasterExportTimesMs, ",")).Append(vbLf)
			End If

			sb.Append(String.format(f, PARAMETER_AVERAGING_MASTER_COUNT_RDD_TIMES_MS))
			If parameterAveragingMasterCountRddSizeTimesMs Is Nothing Then
				sb.Append("-" & vbLf)
			Else
				sb.Append(StatsUtils.getDurationAsString(parameterAveragingMasterCountRddSizeTimesMs, ",")).Append(vbLf)
			End If

			sb.Append(String.format(f, PARAMETER_AVERAGING_MASTER_BROADCAST_CREATE_TIMES_MS))
			If parameterAveragingMasterBroadcastCreateTimesMs Is Nothing Then
				sb.Append("-" & vbLf)
			Else
				sb.Append(StatsUtils.getDurationAsString(parameterAveragingMasterBroadcastCreateTimesMs, ",")).Append(vbLf)
			End If

			sb.Append(String.format(f, PARAMETER_AVERAGING_MASTER_REPARTITION_TIMES_MS))
			If parameterAveragingMasterRepartitionTimesMs Is Nothing Then
				sb.Append("-" & vbLf)
			Else
				sb.Append(StatsUtils.getDurationAsString(parameterAveragingMasterRepartitionTimesMs, ",")).Append(vbLf)
			End If

			sb.Append(String.format(f, PARAMETER_AVERAGING_MASTER_FIT_TIMES_MS))
			If parameterAveragingMasterFitTimesMs Is Nothing Then
				sb.Append("-" & vbLf)
			Else
				sb.Append(StatsUtils.getDurationAsString(parameterAveragingMasterFitTimesMs, ",")).Append(vbLf)
			End If

			sb.Append(String.format(f, PARAMETER_AVERAGING_MASTER_SPLIT_TIMES_MS))
			If parameterAveragingMasterSplitTimesMs Is Nothing Then
				sb.Append("-" & vbLf)
			Else
				sb.Append(StatsUtils.getDurationAsString(parameterAveragingMasterSplitTimesMs, ",")).Append(vbLf)
			End If

			sb.Append(String.format(f, PARAMETER_AVERAGING_MASTER_MAP_PARTITIONS_TIMES_MS))
			If parameterAveragingMasterMapPartitionsTimesMs Is Nothing Then
				sb.Append("-" & vbLf)
			Else
				sb.Append(StatsUtils.getDurationAsString(parameterAveragingMasterMapPartitionsTimesMs, ",")).Append(vbLf)
			End If

			sb.Append(String.format(f, PARAMETER_AVERAGING_MASTER_AGGREGATE_TIMES_MS))
			If paramaterAveragingMasterAggregateTimesMs Is Nothing Then
				sb.Append("-" & vbLf)
			Else
				sb.Append(StatsUtils.getDurationAsString(paramaterAveragingMasterAggregateTimesMs, ",")).Append(vbLf)
			End If

			sb.Append(String.format(f, PARAMETER_AVERAGING_MASTER_PROCESS_PARAMS_UPDATER_TIMES_MS))
			If parameterAveragingMasterProcessParamsUpdaterTimesMs Is Nothing Then
				sb.Append("-" & vbLf)
			Else
				sb.Append(StatsUtils.getDurationAsString(parameterAveragingMasterProcessParamsUpdaterTimesMs, ",")).Append(vbLf)
			End If

			If workerStats IsNot Nothing Then
				sb.Append(workerStats.statsAsString())
			End If

			Return sb.ToString()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void exportStatFiles(String outputPath, org.apache.spark.SparkContext sc) throws java.io.IOException
		Public Overridable Sub exportStatFiles(ByVal outputPath As String, ByVal sc As SparkContext) Implements SparkTrainingStats.exportStatFiles
			Dim d As String = DEFAULT_DELIMITER

			'Export times
			Dim exportRddPath As String = FilenameUtils.concat(outputPath, FILENAME_EXPORT_RDD_TIME)
			StatsUtils.exportStats(parameterAveragingMasterExportTimesMs, exportRddPath, d, sc)

			'Count RDD times:
			Dim countRddPath As String = FilenameUtils.concat(outputPath, FILENAME_COUNT_RDD_SIZE)
			StatsUtils.exportStats(parameterAveragingMasterCountRddSizeTimesMs, countRddPath, d, sc)

			'broadcast create time:
			Dim broadcastTimePath As String = FilenameUtils.concat(outputPath, FILENAME_BROADCAST_CREATE)
			StatsUtils.exportStats(parameterAveragingMasterBroadcastCreateTimesMs, broadcastTimePath, d, sc)

			'repartition
			Dim repartitionTime As String = FilenameUtils.concat(outputPath, FILENAME_REPARTITION_STATS)
			StatsUtils.exportStats(parameterAveragingMasterRepartitionTimesMs, repartitionTime, d, sc)

			'Fit time:
			Dim fitTimePath As String = FilenameUtils.concat(outputPath, FILENAME_FIT_TIME)
			StatsUtils.exportStats(parameterAveragingMasterFitTimesMs, fitTimePath, d, sc)

			'Split time:
			Dim splitTimePath As String = FilenameUtils.concat(outputPath, FILENAME_SPLIT_TIME)
			StatsUtils.exportStats(parameterAveragingMasterSplitTimesMs, splitTimePath, d, sc)

			'Map partitions:
			Dim mapPartitionsPath As String = FilenameUtils.concat(outputPath, FILENAME_MAP_PARTITIONS_TIME)
			StatsUtils.exportStats(parameterAveragingMasterMapPartitionsTimesMs, mapPartitionsPath, d, sc)

			'Aggregate time:
			Dim aggregatePath As String = FilenameUtils.concat(outputPath, FILENAME_AGGREGATE_TIME)
			StatsUtils.exportStats(paramaterAveragingMasterAggregateTimesMs, aggregatePath, d, sc)

			'broadcast create time:
			Dim processParamsPath As String = FilenameUtils.concat(outputPath, FILENAME_PROCESS_PARAMS_TIME)
			StatsUtils.exportStats(parameterAveragingMasterProcessParamsUpdaterTimesMs, processParamsPath, d, sc)

			'Repartition
			If parameterAveragingMasterRepartitionTimesMs IsNot Nothing Then
				Dim repartitionPath As String = FilenameUtils.concat(outputPath, FILENAME_REPARTITION_STATS)
				StatsUtils.exportStats(parameterAveragingMasterRepartitionTimesMs, repartitionPath, d, sc)
			End If

			If workerStats IsNot Nothing Then
				workerStats.exportStatFiles(outputPath, sc)
			End If
		End Sub

		Public Class ParameterAveragingTrainingMasterStatsHelper

			Friend lastExportStartTime As Long
			Friend lastCountStartTime As Long
			Friend lastBroadcastStartTime As Long
			Friend lastRepartitionStartTime As Long
			Friend lastFitStartTime As Long
			Friend lastSplitStartTime As Long
			Friend lastMapPartitionsStartTime As Long
			Friend lastAggregateStartTime As Long
			Friend lastProcessParamsUpdaterStartTime As Long

			Friend workerStats As SparkTrainingStats

			Friend exportTimes As IList(Of EventStats) = New List(Of EventStats)() 'Starts for exporting data
			Friend countTimes As IList(Of EventStats) = New List(Of EventStats)()
			Friend broadcastTimes As IList(Of EventStats) = New List(Of EventStats)()
			Friend repartitionTimes As IList(Of EventStats) = New List(Of EventStats)()
			Friend fitTimes As IList(Of EventStats) = New List(Of EventStats)()
			Friend splitTimes As IList(Of EventStats) = New List(Of EventStats)()
			Friend mapPartitions As IList(Of EventStats) = New List(Of EventStats)()
			Friend aggregateTimes As IList(Of EventStats) = New List(Of EventStats)()
			Friend processParamsUpdaterTimes As IList(Of EventStats) = New List(Of EventStats)()

			Friend ReadOnly timeSource As TimeSource = TimeSourceProvider.Instance

			Public Overridable Sub logExportStart()
				Me.lastExportStartTime = timeSource.currentTimeMillis()
			End Sub

			Public Overridable Sub logExportEnd()
				Dim now As Long = timeSource.currentTimeMillis()

				exportTimes.Add(New BaseEventStats(lastExportStartTime, now - lastExportStartTime))
			End Sub

			Public Overridable Sub logCountStart()
				Me.lastCountStartTime = timeSource.currentTimeMillis()
			End Sub

			Public Overridable Sub logCountEnd()
				Dim now As Long = timeSource.currentTimeMillis()

				countTimes.Add(New BaseEventStats(lastCountStartTime, now - lastCountStartTime))
			End Sub

			Public Overridable Sub logBroadcastStart()
				Me.lastBroadcastStartTime = timeSource.currentTimeMillis()
			End Sub

			Public Overridable Sub logBroadcastEnd()
				Dim now As Long = timeSource.currentTimeMillis()

				broadcastTimes.Add(New BaseEventStats(lastBroadcastStartTime, now - lastBroadcastStartTime))
			End Sub

			Public Overridable Sub logRepartitionStart()
				lastRepartitionStartTime = timeSource.currentTimeMillis()
			End Sub

			Public Overridable Sub logRepartitionEnd()
				Dim now As Long = timeSource.currentTimeMillis()
				repartitionTimes.Add(New BaseEventStats(lastRepartitionStartTime, now - lastRepartitionStartTime))
			End Sub

			Public Overridable Sub logFitStart()
				lastFitStartTime = timeSource.currentTimeMillis()
			End Sub

			Public Overridable Sub logFitEnd(ByVal examplesCount As Integer)
				Dim now As Long = timeSource.currentTimeMillis()
				fitTimes.Add(New ExampleCountEventStats(lastFitStartTime, now - lastFitStartTime, examplesCount))
			End Sub

			Public Overridable Sub logSplitStart()
				lastSplitStartTime = timeSource.currentTimeMillis()
			End Sub

			Public Overridable Sub logSplitEnd()
				Dim now As Long = timeSource.currentTimeMillis()
				splitTimes.Add(New BaseEventStats(lastSplitStartTime, now - lastSplitStartTime))
			End Sub

			Public Overridable Sub logMapPartitionsStart()
				lastMapPartitionsStartTime = timeSource.currentTimeMillis()
			End Sub

			Public Overridable Sub logMapPartitionsEnd(ByVal nPartitions As Integer)
				Dim now As Long = timeSource.currentTimeMillis()
				mapPartitions.Add(New PartitionCountEventStats(lastMapPartitionsStartTime, (now - lastMapPartitionsStartTime), nPartitions))
			End Sub

			Public Overridable Sub logAggregateStartTime()
				lastAggregateStartTime = timeSource.currentTimeMillis()
			End Sub

			Public Overridable Sub logAggregationEndTime()
				Dim now As Long = timeSource.currentTimeMillis()
				aggregateTimes.Add(New BaseEventStats(lastAggregateStartTime, now - lastAggregateStartTime))
			End Sub

			Public Overridable Sub logProcessParamsUpdaterStart()
				lastProcessParamsUpdaterStartTime = timeSource.currentTimeMillis()
			End Sub

			Public Overridable Sub logProcessParamsUpdaterEnd()
				Dim now As Long = timeSource.currentTimeMillis()
				processParamsUpdaterTimes.Add(New BaseEventStats(lastProcessParamsUpdaterStartTime, now - lastProcessParamsUpdaterStartTime))
			End Sub

			Public Overridable Sub addWorkerStats(ByVal workerStats As SparkTrainingStats)
				If Me.workerStats Is Nothing Then
					Me.workerStats = workerStats
				ElseIf workerStats IsNot Nothing Then
					Me.workerStats.addOtherTrainingStats(workerStats)
				End If
			End Sub

			Public Overridable Function build() As ParameterAveragingTrainingMasterStats
				Return New ParameterAveragingTrainingMasterStats(workerStats, exportTimes, countTimes, broadcastTimes, fitTimes, splitTimes, mapPartitions, aggregateTimes, processParamsUpdaterTimes, repartitionTimes)
			End Function

		End Class

	End Class

End Namespace