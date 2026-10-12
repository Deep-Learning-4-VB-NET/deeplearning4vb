Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports Data = lombok.Data
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports SparkContext = org.apache.spark.SparkContext
Imports EventStats = org.deeplearning4j.spark.stats.EventStats
Imports StatsUtils = org.deeplearning4j.spark.stats.StatsUtils

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class CommonSparkTrainingStats implements SparkTrainingStats
	<Serializable>
	Public Class CommonSparkTrainingStats
		Implements SparkTrainingStats

		Public Const DEFAULT_DELIMITER As String = ","
		Public Const FILENAME_TOTAL_TIME_STATS As String = "workerFlatMapTotalTimeMs.txt"
		Public Const FILENAME_GET_INITIAL_MODEL_STATS As String = "workerFlatMapGetInitialModelTimeMs.txt"
		Public Const FILENAME_DATASET_GET_TIME_STATS As String = "workerFlatMapDataSetGetTimesMs.txt"
		Public Const FILENAME_PROCESS_MINIBATCH_TIME_STATS As String = "workerFlatMapProcessMiniBatchTimesMs.txt"

		Public Const WORKER_FLAT_MAP_TOTAL_TIME_MS As String = "WorkerFlatMapTotalTimeMs"
		Public Const WORKER_FLAT_MAP_GET_INITIAL_MODEL_TIME_MS As String = "WorkerFlatMapGetInitialModelTimeMs"
		Public Const WORKER_FLAT_MAP_DATA_SET_GET_TIMES_MS As String = "WorkerFlatMapDataSetGetTimesMs"
		Public Const WORKER_FLAT_MAP_PROCESS_MINI_BATCH_TIMES_MS As String = "WorkerFlatMapProcessMiniBatchTimesMs"
		Private Shared columnNames As ISet(Of String) = Collections.unmodifiableSet(New LinkedHashSet(Of String)(java.util.Arrays.asList(WORKER_FLAT_MAP_TOTAL_TIME_MS, WORKER_FLAT_MAP_GET_INITIAL_MODEL_TIME_MS, WORKER_FLAT_MAP_DATA_SET_GET_TIMES_MS, WORKER_FLAT_MAP_PROCESS_MINI_BATCH_TIMES_MS)))

		Private trainingWorkerSpecificStats As SparkTrainingStats
		Private workerFlatMapTotalTimeMs As IList(Of EventStats)
		Private workerFlatMapGetInitialModelTimeMs As IList(Of EventStats)
		Private workerFlatMapDataSetGetTimesMs As IList(Of EventStats)
		Private workerFlatMapProcessMiniBatchTimesMs As IList(Of EventStats)



		Public Sub New()

		End Sub

		Private Sub New(ByVal builder As Builder)
			Me.trainingWorkerSpecificStats = builder.trainingMasterSpecificStats_Conflict
			Me.workerFlatMapTotalTimeMs = builder.workerFlatMapTotalTimeMs_Conflict
			Me.workerFlatMapGetInitialModelTimeMs = builder.workerFlatMapGetInitialModelTimeMs_Conflict
			Me.workerFlatMapDataSetGetTimesMs = builder.workerFlatMapDataSetGetTimesMs_Conflict
			Me.workerFlatMapProcessMiniBatchTimesMs = builder.workerFlatMapProcessMiniBatchTimesMs_Conflict
		End Sub


		Public Overridable ReadOnly Property KeySet As ISet(Of String)
			Get
				Dim set As ISet(Of String) = New LinkedHashSet(Of String)(columnNames)
				If trainingWorkerSpecificStats IsNot Nothing Then
					set.addAll(trainingWorkerSpecificStats.getKeySet())
				End If
    
				Return set
			End Get
		End Property

		Public Overridable Function getValue(ByVal key As String) As IList(Of EventStats)
			Select Case key
				Case WORKER_FLAT_MAP_TOTAL_TIME_MS
					Return workerFlatMapTotalTimeMs
				Case WORKER_FLAT_MAP_GET_INITIAL_MODEL_TIME_MS
					Return workerFlatMapGetInitialModelTimeMs
				Case WORKER_FLAT_MAP_DATA_SET_GET_TIMES_MS
					Return workerFlatMapDataSetGetTimesMs
				Case WORKER_FLAT_MAP_PROCESS_MINI_BATCH_TIMES_MS
					Return workerFlatMapProcessMiniBatchTimesMs
				Case Else
					If trainingWorkerSpecificStats IsNot Nothing Then
						Return trainingWorkerSpecificStats.getValue(key)
					End If
					Throw New System.ArgumentException("Unknown key: """ & key & """")
			End Select
		End Function

		Public Overridable Function getShortNameForKey(ByVal key As String) As String Implements SparkTrainingStats.getShortNameForKey
			Select Case key
				Case WORKER_FLAT_MAP_TOTAL_TIME_MS
					Return "Total"
				Case WORKER_FLAT_MAP_GET_INITIAL_MODEL_TIME_MS
					Return "GetInitModel"
				Case WORKER_FLAT_MAP_DATA_SET_GET_TIMES_MS
					Return "GetDataSet"
				Case WORKER_FLAT_MAP_PROCESS_MINI_BATCH_TIMES_MS
					Return "ProcessBatch"
				Case Else
					If trainingWorkerSpecificStats IsNot Nothing Then
						Return trainingWorkerSpecificStats.getShortNameForKey(key)
					End If
					Throw New System.ArgumentException("Unknown key: """ & key & """")
			End Select
		End Function

		Public Overridable Function defaultIncludeInPlots(ByVal key As String) As Boolean Implements SparkTrainingStats.defaultIncludeInPlots
			Select Case key
				Case WORKER_FLAT_MAP_TOTAL_TIME_MS, WORKER_FLAT_MAP_GET_INITIAL_MODEL_TIME_MS, WORKER_FLAT_MAP_PROCESS_MINI_BATCH_TIMES_MS
					Return False 'Covered by worker stats generally
				Case WORKER_FLAT_MAP_DATA_SET_GET_TIMES_MS
					Return True
				Case Else
					If trainingWorkerSpecificStats IsNot Nothing Then
						Return trainingWorkerSpecificStats.defaultIncludeInPlots(key)
					End If
					Return False
			End Select
		End Function

		Public Overridable Sub addOtherTrainingStats(ByVal other As SparkTrainingStats) Implements SparkTrainingStats.addOtherTrainingStats
			If Not (TypeOf other Is CommonSparkTrainingStats) Then
				Throw New System.ArgumentException("Cannot add other training stats: not an instance of CommonSparkTrainingStats")
			End If

			Dim o As CommonSparkTrainingStats = DirectCast(other, CommonSparkTrainingStats)

			CType(workerFlatMapTotalTimeMs, List(Of EventStats)).AddRange(o.workerFlatMapTotalTimeMs)
			CType(workerFlatMapGetInitialModelTimeMs, List(Of EventStats)).AddRange(o.workerFlatMapGetInitialModelTimeMs)
			CType(workerFlatMapDataSetGetTimesMs, List(Of EventStats)).AddRange(o.workerFlatMapDataSetGetTimesMs)
			CType(workerFlatMapProcessMiniBatchTimesMs, List(Of EventStats)).AddRange(o.workerFlatMapProcessMiniBatchTimesMs)

			If trainingWorkerSpecificStats IsNot Nothing Then
				trainingWorkerSpecificStats.addOtherTrainingStats(o.trainingWorkerSpecificStats)
			ElseIf o.trainingWorkerSpecificStats IsNot Nothing Then
				Throw New System.InvalidOperationException("Cannot merge: training master specific stats is null in one, but not the other")
			End If
		End Sub

		Public Overridable ReadOnly Property NestedTrainingStats As SparkTrainingStats Implements SparkTrainingStats.getNestedTrainingStats
			Get
				Return trainingWorkerSpecificStats
			End Get
		End Property

		Public Overridable Function statsAsString() As String Implements SparkTrainingStats.statsAsString
			Dim sb As New StringBuilder()
			Dim f As String = SparkTrainingStats.DEFAULT_PRINT_FORMAT

			sb.Append(String.format(f, WORKER_FLAT_MAP_TOTAL_TIME_MS))
			If workerFlatMapTotalTimeMs Is Nothing Then
				sb.Append("-" & vbLf)
			Else
				sb.Append(StatsUtils.getDurationAsString(workerFlatMapTotalTimeMs, ",")).Append(vbLf)
			End If

			sb.Append(String.format(f, WORKER_FLAT_MAP_GET_INITIAL_MODEL_TIME_MS))
			If workerFlatMapGetInitialModelTimeMs Is Nothing Then
				sb.Append("-" & vbLf)
			Else
				sb.Append(StatsUtils.getDurationAsString(workerFlatMapGetInitialModelTimeMs, ",")).Append(vbLf)
			End If

			sb.Append(String.format(f, WORKER_FLAT_MAP_DATA_SET_GET_TIMES_MS))
			If workerFlatMapDataSetGetTimesMs Is Nothing Then
				sb.Append("-" & vbLf)
			Else
				sb.Append(StatsUtils.getDurationAsString(workerFlatMapDataSetGetTimesMs, ",")).Append(vbLf)
			End If

			sb.Append(String.format(f, WORKER_FLAT_MAP_PROCESS_MINI_BATCH_TIMES_MS))
			If workerFlatMapProcessMiniBatchTimesMs Is Nothing Then
				sb.Append("-" & vbLf)
			Else
				sb.Append(StatsUtils.getDurationAsString(workerFlatMapProcessMiniBatchTimesMs, ",")).Append(vbLf)
			End If

			If trainingWorkerSpecificStats IsNot Nothing Then
				sb.Append(trainingWorkerSpecificStats.statsAsString()).Append(vbLf)
			End If

			Return sb.ToString()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void exportStatFiles(String outputPath, org.apache.spark.SparkContext sc) throws java.io.IOException
		Public Overridable Sub exportStatFiles(ByVal outputPath As String, ByVal sc As SparkContext) Implements SparkTrainingStats.exportStatFiles
			Dim d As String = DEFAULT_DELIMITER


			'Total time stats (includes total example counts)
			Dim totalTimeStatsPath As String = FilenameUtils.concat(outputPath, FILENAME_TOTAL_TIME_STATS)
			StatsUtils.exportStats(workerFlatMapTotalTimeMs, totalTimeStatsPath, d, sc)

			'"Get initial model" stats:
			Dim getInitialModelStatsPath As String = FilenameUtils.concat(outputPath, FILENAME_GET_INITIAL_MODEL_STATS)
			StatsUtils.exportStats(workerFlatMapGetInitialModelTimeMs, getInitialModelStatsPath, d, sc)

			'"DataSet get time" stats:
			Dim getDataSetStatsPath As String = FilenameUtils.concat(outputPath, FILENAME_DATASET_GET_TIME_STATS)
			StatsUtils.exportStats(workerFlatMapDataSetGetTimesMs, getDataSetStatsPath, d, sc)

			'Process minibatch time stats:
			Dim processMiniBatchStatsPath As String = FilenameUtils.concat(outputPath, FILENAME_PROCESS_MINIBATCH_TIME_STATS)
			StatsUtils.exportStats(workerFlatMapProcessMiniBatchTimesMs, processMiniBatchStatsPath, d, sc)

			If trainingWorkerSpecificStats IsNot Nothing Then
				trainingWorkerSpecificStats.exportStatFiles(outputPath, sc)
			End If
		End Sub

		Public Class Builder
'JAVA TO VB CONVERTER NOTE: The field trainingMasterSpecificStats was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend trainingMasterSpecificStats_Conflict As SparkTrainingStats
'JAVA TO VB CONVERTER NOTE: The field workerFlatMapTotalTimeMs was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend workerFlatMapTotalTimeMs_Conflict As IList(Of EventStats)
'JAVA TO VB CONVERTER NOTE: The field workerFlatMapGetInitialModelTimeMs was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend workerFlatMapGetInitialModelTimeMs_Conflict As IList(Of EventStats)
'JAVA TO VB CONVERTER NOTE: The field workerFlatMapDataSetGetTimesMs was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend workerFlatMapDataSetGetTimesMs_Conflict As IList(Of EventStats)
'JAVA TO VB CONVERTER NOTE: The field workerFlatMapProcessMiniBatchTimesMs was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend workerFlatMapProcessMiniBatchTimesMs_Conflict As IList(Of EventStats)

'JAVA TO VB CONVERTER NOTE: The parameter trainingMasterSpecificStats was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function trainingMasterSpecificStats(ByVal trainingMasterSpecificStats_Conflict As SparkTrainingStats) As Builder
				Me.trainingMasterSpecificStats_Conflict = trainingMasterSpecificStats_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter workerFlatMapTotalTimeMs was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function workerFlatMapTotalTimeMs(ByVal workerFlatMapTotalTimeMs_Conflict As IList(Of EventStats)) As Builder
				Me.workerFlatMapTotalTimeMs_Conflict = workerFlatMapTotalTimeMs_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter workerFlatMapGetInitialModelTimeMs was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function workerFlatMapGetInitialModelTimeMs(ByVal workerFlatMapGetInitialModelTimeMs_Conflict As IList(Of EventStats)) As Builder
				Me.workerFlatMapGetInitialModelTimeMs_Conflict = workerFlatMapGetInitialModelTimeMs_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter workerFlatMapDataSetGetTimesMs was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function workerFlatMapDataSetGetTimesMs(ByVal workerFlatMapDataSetGetTimesMs_Conflict As IList(Of EventStats)) As Builder
				Me.workerFlatMapDataSetGetTimesMs_Conflict = workerFlatMapDataSetGetTimesMs_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter workerFlatMapProcessMiniBatchTimesMs was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function workerFlatMapProcessMiniBatchTimesMs(ByVal workerFlatMapProcessMiniBatchTimesMs_Conflict As IList(Of EventStats)) As Builder
				Me.workerFlatMapProcessMiniBatchTimesMs_Conflict = workerFlatMapProcessMiniBatchTimesMs_Conflict
				Return Me
			End Function

			Public Overridable Function build() As CommonSparkTrainingStats
				Return New CommonSparkTrainingStats(Me)
			End Function
		End Class
	End Class

End Namespace