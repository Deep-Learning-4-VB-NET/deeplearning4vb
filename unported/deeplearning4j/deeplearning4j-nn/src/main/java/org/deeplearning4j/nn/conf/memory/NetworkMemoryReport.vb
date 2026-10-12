Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports CacheMode = org.deeplearning4j.nn.conf.CacheMode
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.deeplearning4j.nn.conf.memory


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @EqualsAndHashCode(callSuper = true) public class NetworkMemoryReport extends MemoryReport
	Public Class NetworkMemoryReport
		Inherits MemoryReport

		Private Shared ReadOnly BYTES_FORMAT As New DecimalFormat("#,###")

		Private ReadOnly layerAndVertexReports As IDictionary(Of String, MemoryReport)
		Private ReadOnly modelClass As Type
		Private ReadOnly modelName As String
		Private ReadOnly networkInputTypes() As InputType

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NetworkMemoryReport(@NonNull @JsonProperty("layerAndVertexReports") java.util.Map<String, MemoryReport> layerAndVertexReports, @NonNull @JsonProperty("modelClass") @Class modelClass, @JsonProperty("modelName") String modelName, @NonNull @JsonProperty("networkInputTypes") org.deeplearning4j.nn.conf.inputs.InputType... networkInputTypes)
		Public Sub New(ByVal layerAndVertexReports As IDictionary(Of String, MemoryReport), ByVal modelClass As Type, ByVal modelName As String, ParamArray ByVal networkInputTypes() As InputType)
			Me.layerAndVertexReports = layerAndVertexReports
			Me.modelClass = modelClass
			Me.modelName = modelName
			Me.networkInputTypes = networkInputTypes
		End Sub


		Public Overrides ReadOnly Property ReportClass As Type
			Get
				Return modelClass
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return modelName
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public long getTotalMemoryBytes(int minibatchSize, @NonNull MemoryUseMode memoryUseMode, @NonNull CacheMode cacheMode, @NonNull DataType dataType)
		Public Overrides Function getTotalMemoryBytes(ByVal minibatchSize As Integer, ByVal memoryUseMode As MemoryUseMode, ByVal cacheMode As CacheMode, ByVal dataType As DataType) As Long

			'As per MemoryReport javadoc: we need
			' sum_layers (StdFixed + minibatch * StdVariable) + sum_layers (CacheFixed + minibatch * CacheVariable)
			' + max_layers ( WorkingMemoryFixed + minibatch * WorkingMemoryVariable)

			Dim totalBytes As Long = 0
			Dim maxWorking As Long = 0
			Dim maxWorkingFixed As Long = 0
			Dim maxWorkingVariable As Long = 0
			For Each lmr As MemoryReport In layerAndVertexReports.Values

				For Each mt As MemoryType In MemoryType.values()
					If mt = MemoryType.WORKING_MEMORY_FIXED OrElse mt = MemoryType.WORKING_MEMORY_VARIABLE Then
						Continue For
					End If
					totalBytes += lmr.getMemoryBytes(mt, minibatchSize, memoryUseMode, cacheMode, dataType)
				Next mt

				Dim workFixed As Long = lmr.getMemoryBytes(MemoryType.WORKING_MEMORY_FIXED, minibatchSize, memoryUseMode, cacheMode, dataType)
				Dim workVar As Long = lmr.getMemoryBytes(MemoryType.WORKING_MEMORY_VARIABLE, minibatchSize, memoryUseMode, cacheMode, dataType)
				Dim currWorking As Long = workFixed + workVar

				If currWorking > maxWorking Then
					maxWorking = currWorking
					maxWorkingFixed = workFixed
					maxWorkingVariable = workVar
				End If
			Next lmr

			Return totalBytes + maxWorkingFixed + maxWorkingVariable
		End Function

		Public Overrides Function getMemoryBytes(ByVal memoryType As MemoryType, ByVal minibatchSize As Integer, ByVal memoryUseMode As MemoryUseMode, ByVal cacheMode As CacheMode, ByVal dataType As DataType) As Long
			Dim totalBytes As Long = 0
			For Each lmr As MemoryReport In layerAndVertexReports.Values

				Dim bytes As Long = lmr.getMemoryBytes(memoryType, minibatchSize, memoryUseMode, cacheMode, dataType)

				If memoryType = MemoryType.WORKING_MEMORY_FIXED OrElse memoryType = MemoryType.WORKING_MEMORY_VARIABLE Then
					totalBytes = Math.Max(totalBytes, bytes)
				Else
					totalBytes += bytes
				End If
			Next lmr

			Return totalBytes
		End Function

		Public Overrides Function ToString() As String

			Dim fixedMemBytes As Long = getTotalMemoryBytes(0, MemoryUseMode.INFERENCE, CacheMode.NONE, DataType.FLOAT)
			Dim perEx As Long = getTotalMemoryBytes(1, MemoryUseMode.INFERENCE, CacheMode.NONE, DataType.FLOAT) - fixedMemBytes

			Dim fixedMemBytesTrain As Long = getTotalMemoryBytes(0, MemoryUseMode.TRAINING, CacheMode.NONE, DataType.FLOAT)
			Dim perExTrain As Long = getTotalMemoryBytes(1, MemoryUseMode.TRAINING, CacheMode.NONE, DataType.FLOAT) - fixedMemBytesTrain

			Dim layerCounts As IDictionary(Of Type, Integer) = New LinkedHashMap(Of Type, Integer)()
			For Each mr As MemoryReport In layerAndVertexReports.Values
				If layerCounts.ContainsKey(mr.ReportClass) Then
					layerCounts(mr.ReportClass) = layerCounts(mr.ReportClass) + 1
				Else
					layerCounts(mr.ReportClass) = 1
				End If
			Next mr

			Dim sbLayerCounts As New StringBuilder()
			For Each e As KeyValuePair(Of Type, Integer) In layerCounts.SetOfKeyValuePairs()
				sbLayerCounts.Append(e.Value).Append(" x ").Append(e.Key.getSimpleName()).Append(", ")
			Next e

			Dim sb As New StringBuilder()
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			sb.Append("----- Network Memory Report -----" & vbLf).Append("  Model Class:                        ").Append(modelClass.FullName).Append(vbLf).Append("  Model Name:                         ").Append(modelName).Append(vbLf).Append("  Network Input:                      ").Append(Arrays.toString(networkInputTypes)).Append(vbLf).Append("  # Layers:                           ").Append(layerAndVertexReports.Count).Append(vbLf).Append("  Layer Types:                        ").Append(sbLayerCounts).Append(vbLf)

			appendFixedPlusVariable(sb, "  Inference Memory (FP32)             ", fixedMemBytes, perEx)
			appendFixedPlusVariable(sb, "  Training Memory (FP32):             ", fixedMemBytesTrain, perExTrain)

			sb.Append("  Inference Memory Breakdown (FP32):" & vbLf)
			appendBreakDown(sb, MemoryUseMode.INFERENCE, CacheMode.NONE, DataType.FLOAT)

			sb.Append("  Training Memory Breakdown (CacheMode = ").Append(CacheMode.NONE).Append(", FP32):" & vbLf)
			appendBreakDown(sb, MemoryUseMode.TRAINING, CacheMode.NONE, DataType.FLOAT)


			Return sb.ToString()
		End Function

		Private Sub appendBreakDown(ByVal sb As StringBuilder, ByVal useMode As MemoryUseMode, ByVal cacheMode As CacheMode, ByVal dataType As DataType)
			For Each mt As MemoryType In MemoryType.values()
				If useMode = MemoryUseMode.INFERENCE AndAlso Not mt.isInference() Then
					Continue For
				End If

				Dim bytesFixed As Long = getMemoryBytes(mt, 0, useMode, cacheMode, dataType)
				Dim bytesPerEx As Long = getMemoryBytes(mt, 1, useMode, cacheMode, dataType) - bytesFixed

				If bytesFixed > 0 OrElse bytesPerEx > 0 Then
					Dim formatted As String = String.Format("  - {0,-34}", mt)
					appendFixedPlusVariable(sb, formatted, bytesFixed, bytesPerEx)
				End If
			Next mt
		End Sub

		Private Sub appendFixedPlusVariable(ByVal sb As StringBuilder, ByVal title As String, ByVal bytesFixed As Long, ByVal bytesPerEx As Long)
			sb.Append(title)
			If bytesFixed > 0 Then
				sb.Append(formatBytes(bytesFixed)).Append(" bytes")
			End If
			If bytesPerEx > 0 Then
				If bytesFixed > 0 Then
					sb.Append(" + ")
				End If
				sb.Append("nExamples * ").Append(formatBytes(bytesPerEx)).Append(" bytes")
			End If
			sb.Append(vbLf)
		End Sub

		Private Function formatBytes(ByVal bytes As Long) As String
			Return BYTES_FORMAT.format(bytes)
		End Function

	End Class

End Namespace