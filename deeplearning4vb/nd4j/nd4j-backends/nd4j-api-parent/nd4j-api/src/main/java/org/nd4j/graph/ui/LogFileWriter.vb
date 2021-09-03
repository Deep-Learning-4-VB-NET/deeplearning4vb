Imports System
Imports System.Collections.Generic
Imports FlatBufferBuilder = com.google.flatbuffers.FlatBufferBuilder
Imports Table = com.google.flatbuffers.Table
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports VariableType = org.nd4j.autodiff.samediff.VariableType
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Variable = org.nd4j.autodiff.samediff.internal.Variable
Imports FlatBuffersMapper = org.nd4j.autodiff.samediff.serde.FlatBuffersMapper
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.graph
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseCompatOp = org.nd4j.linalg.api.ops.impl.controlflow.compat.BaseCompatOp
Imports Enter = org.nd4j.linalg.api.ops.impl.controlflow.compat.Enter
Imports [Exit] = org.nd4j.linalg.api.ops.impl.controlflow.compat.Exit
Imports NextIteration = org.nd4j.linalg.api.ops.impl.controlflow.compat.NextIteration
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

Namespace org.nd4j.graph.ui


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class LogFileWriter
	Public Class LogFileWriter
		Public NotInheritable Class EventSubtype
			Public Shared ReadOnly NONE As New EventSubtype("NONE", InnerEnum.NONE)
			Public Shared ReadOnly EVALUATION As New EventSubtype("EVALUATION", InnerEnum.EVALUATION)
			Public Shared ReadOnly LOSS As New EventSubtype("LOSS", InnerEnum.LOSS)
			Public Shared ReadOnly LEARNING_RATE As New EventSubtype("LEARNING_RATE", InnerEnum.LEARNING_RATE)
			Public Shared ReadOnly TUNING_METRIC As New EventSubtype("TUNING_METRIC", InnerEnum.TUNING_METRIC)
			Public Shared ReadOnly PERFORMANCE As New EventSubtype("PERFORMANCE", InnerEnum.PERFORMANCE)
			Public Shared ReadOnly PROFILING As New EventSubtype("PROFILING", InnerEnum.PROFILING)
			Public Shared ReadOnly FEATURE_LABEL As New EventSubtype("FEATURE_LABEL", InnerEnum.FEATURE_LABEL)
			Public Shared ReadOnly PREDICTION As New EventSubtype("PREDICTION", InnerEnum.PREDICTION)
			Public Shared ReadOnly USER_CUSTOM As New EventSubtype("USER_CUSTOM", InnerEnum.USER_CUSTOM)

			Private Shared ReadOnly valueList As New List(Of EventSubtype)()

			Shared Sub New()
				valueList.Add(NONE)
				valueList.Add(EVALUATION)
				valueList.Add(LOSS)
				valueList.Add(LEARNING_RATE)
				valueList.Add(TUNING_METRIC)
				valueList.Add(PERFORMANCE)
				valueList.Add(PROFILING)
				valueList.Add(FEATURE_LABEL)
				valueList.Add(PREDICTION)
				valueList.Add(USER_CUSTOM)
			End Sub

			Public Enum InnerEnum
				NONE
				EVALUATION
				LOSS
				LEARNING_RATE
				TUNING_METRIC
				PERFORMANCE
				PROFILING
				FEATURE_LABEL
				PREDICTION
				USER_CUSTOM
			End Enum

			Public ReadOnly innerEnumValue As InnerEnum
			Private ReadOnly nameValue As String
			Private ReadOnly ordinalValue As Integer
			Private Shared nextOrdinal As Integer = 0

			Private Sub New(ByVal name As String, ByVal thisInnerEnumValue As InnerEnum)
				nameValue = name
				ordinalValue = nextOrdinal
				nextOrdinal += 1
				innerEnumValue = thisInnerEnumValue
			End Sub

			Public Function asUIEventSubtype() As SByte
				Select Case Me
					Case NONE
						Return UIEventSubtype.NONE
					Case EVALUATION
						Return UIEventSubtype.EVALUATION
					Case LOSS
						Return UIEventSubtype.LOSS
					Case LEARNING_RATE
						Return UIEventSubtype.LEARNING_RATE
					Case TUNING_METRIC
						Return UIEventSubtype.TUNING_METRIC
					Case PERFORMANCE
						Return UIEventSubtype.PERFORMANCE
					Case PROFILING
						Return UIEventSubtype.PROFILING
					Case FEATURE_LABEL
						Return UIEventSubtype.FEATURE_LABEL
					Case PREDICTION
						Return UIEventSubtype.PREDICTION
					Case USER_CUSTOM
						Return UIEventSubtype.USER_CUSTOM
					Case Else
						Throw New Exception()
				End Select
			End Function

			Public Shared Function values() As EventSubtype()
				Return valueList.ToArray()
			End Function

			Public Function ordinal() As Integer
				Return ordinalValue
			End Function

			Public Overrides Function ToString() As String
				Return nameValue
			End Function

			Public Shared Operator =(ByVal one As EventSubtype, ByVal two As EventSubtype) As Boolean
				Return one.innerEnumValue = two.innerEnumValue
			End Operator

			Public Shared Operator <>(ByVal one As EventSubtype, ByVal two As EventSubtype) As Boolean
				Return one.innerEnumValue <> two.innerEnumValue
			End Operator

			Public Shared Function valueOf(ByVal name As String) As EventSubtype
				For Each enumInstance As EventSubtype In EventSubtype.valueList
					If enumInstance.nameValue = name Then
						Return enumInstance
					End If
				Next
				Throw New System.ArgumentException(name)
			End Function
		End Class

		Private ReadOnly file As File
		Private endStaticInfoOffset As Long = -1
		Private ReadOnly nameIndexCounter As New AtomicInteger(0)
		Private ReadOnly nameIndexMap As IDictionary(Of Integer, String) = New Dictionary(Of Integer, String)()
		Private ReadOnly indexNameMap As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public LogFileWriter(java.io.File file) throws java.io.IOException
		Public Sub New(ByVal file As File)
			Me.file = file
			If file.exists() Then
				'Restore state
				Dim si As StaticInfo = readStatic()
				Dim staticList As IList(Of Pair(Of UIStaticInfoRecord, Table)) = si.getData()
				Dim staticInfoOffset As Long = 0
				Dim seenEndStatic As Boolean = False
				For i As Integer = 0 To staticList.Count - 1
					Dim r As UIStaticInfoRecord = staticList(i).getFirst()
					If r.infoType() = UIInfoType.START_EVENTS Then
						seenEndStatic = True
					End If
					staticInfoOffset += r.getByteBuffer().capacity()
				Next i

	'            if(seenEndStatic)
	'                endStaticInfoOffset = staticInfoOffset;
				endStaticInfoOffset = si.getFileOffset()

				'Restore names:
				Dim events As IList(Of Pair(Of UIEvent, Table)) = readEvents()
				For Each p As Pair(Of UIEvent, Table) In events
					If p.First.eventType() = UIEventType.ADD_NAME Then
						nameIndexCounter.getAndIncrement()
						Dim name As UIAddName = CType(p.Second, UIAddName)
						nameIndexMap(name.nameIdx()) = name.name()
						indexNameMap(name.name()) = name.nameIdx()
					End If
				Next p
			End If
		End Sub

		''' <summary>
		''' Write the graph structure </summary>
		''' <param name="sd"> SameDiff instance to write the graph structure for </param>
		''' <returns> Number of bytes written </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public long writeGraphStructure(org.nd4j.autodiff.samediff.SameDiff sd) throws java.io.IOException
		Public Overridable Function writeGraphStructure(ByVal sd As SameDiff) As Long
			Preconditions.checkState(endStaticInfoOffset < 0, "Cannot write graph structure - already wrote end of static info marker")
			Dim h As Pair(Of Integer, FlatBufferBuilder) = encodeStaticHeader(UIInfoType.GRAPH_STRUCTURE)

			Dim fbb2 As New FlatBufferBuilder(0)
			Dim graphStructureOffset As Integer = encodeGraphStructure(fbb2, sd)
			fbb2.finish(graphStructureOffset)

			Return append(h.Second, fbb2)
		End Function


		''' <summary>
		''' Write marker for final static data
		''' @return </summary>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public long writeFinishStaticMarker() throws java.io.IOException
		Public Overridable Function writeFinishStaticMarker() As Long
			Preconditions.checkState(endStaticInfoOffset < 0, "Wrote final static already information already")
			Dim encoded As Pair(Of Integer, FlatBufferBuilder) = encodeStaticHeader(UIInfoType.START_EVENTS)
			Dim [out] As Long = append(encoded.Second, Nothing)
			endStaticInfoOffset = file.length()
			Return [out]
		End Function

		''' <summary>
		''' Read all static information at the start of the file
		''' 
		''' @return </summary>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public StaticInfo readStatic() throws java.io.IOException
		Public Overridable Function readStatic() As StaticInfo

			Dim [out] As IList(Of Pair(Of UIStaticInfoRecord, Table)) = New List(Of Pair(Of UIStaticInfoRecord, Table))()
			Dim allStaticRead As Boolean = False
			Using f As New java.io.RandomAccessFile(file, "r"), fc As java.nio.channels.FileChannel = f.getChannel()
				f.seek(0)
				Do While Not allStaticRead

					'read 2 header ints
					Dim lengthHeader As Integer = f.readInt()
					Dim lengthContent As Integer = f.readInt()

					'Read header
					Dim bb As ByteBuffer = ByteBuffer.allocate(lengthHeader)
					f.getChannel().read(bb)
					Dim buffer As Buffer = CType(bb, Buffer)
					buffer.flip() 'Flip for reading
					Dim r As UIStaticInfoRecord = UIStaticInfoRecord.getRootAsUIStaticInfoRecord(bb)

					'Read content
					bb = ByteBuffer.allocate(lengthContent)
					f.getChannel().read(bb)
					Dim buffer1 As Buffer = CType(bb, Buffer)
					buffer1.flip() 'Flip for reading

					Dim infoType As SByte = r.infoType()
					Dim t As Table
					Select Case infoType
						Case UIInfoType.GRAPH_STRUCTURE
							t = UIGraphStructure.getRootAsUIGraphStructure(bb)
						Case UIInfoType.SYTEM_INFO
							t = UISystemInfo.getRootAsUISystemInfo(bb)
						Case UIInfoType.START_EVENTS
							t = Nothing
						Case Else
							Throw New Exception("Unknown UI static info type: " & r.infoType())
					End Select

					'TODO do we need to close file here?

					[out].Add(New Pair(Of UIStaticInfoRecord, Table)(r, t))
					Dim pointer As Long = f.getFilePointer()
					Dim length As Long = f.length()
					If True Then
						log.trace("File pointer = {}, file length = {}", pointer, length)
						If infoType = UIInfoType.START_EVENTS OrElse pointer >= length Then
							allStaticRead = True
						End If
					End If
				Loop
				Dim s As New StaticInfo([out], f.getFilePointer())
				Return s
			End Using
		End Function

		''' <summary>
		''' Read all of the events.
		''' </summary>
		''' <returns> All of the UI events </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public List<org.nd4j.common.primitives.Pair<UIEvent, com.google.flatbuffers.Table>> readEvents() throws java.io.IOException
		Public Overridable Function readEvents() As IList(Of Pair(Of UIEvent, Table))
			'TODO eventually we'll support working out the offset for files that were not written in this session
			Preconditions.checkState(endStaticInfoOffset >= 0, "Cannot read events - have not written end of static info marker")
			Return readEvents(endStaticInfoOffset)
		End Function

		''' <summary>
		''' Read all of the events starting at a specific file offset
		''' </summary>
		''' <returns> All of the UI events </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public List<org.nd4j.common.primitives.Pair<UIEvent, com.google.flatbuffers.Table>> readEvents(long startOffset) throws java.io.IOException
		Public Overridable Function readEvents(ByVal startOffset As Long) As IList(Of Pair(Of UIEvent, Table))
			If endStaticInfoOffset >= file.length() Then
				Return java.util.Collections.emptyList()
			End If

			Dim [out] As IList(Of Pair(Of UIEvent, Table)) = New List(Of Pair(Of UIEvent, Table))()
			Using f As New java.io.RandomAccessFile(file, "r"), fc As java.nio.channels.FileChannel = f.getChannel()
				f.seek(startOffset)
				Do While f.getFilePointer() < f.length()
					'read 2 header ints
					Dim lengthHeader As Integer = f.readInt()
					Dim lengthContent As Integer = f.readInt()

					'Read header
					Dim bb As ByteBuffer = ByteBuffer.allocate(lengthHeader)
					f.getChannel().read(bb)
					Dim buffer2 As Buffer = CType(bb, Buffer)
					buffer2.flip() 'Flip for reading
					Dim e As UIEvent = UIEvent.getRootAsUIEvent(bb)

					'Read Content
					bb = ByteBuffer.allocate(lengthContent)
					f.getChannel().read(bb)
					Dim buffer3 As Buffer = CType(bb, Buffer)
					buffer3.flip() 'Flip for reading

					Dim infoType As SByte = e.eventType()
					Dim t As Table
					Select Case infoType
						Case UIEventType.ADD_NAME
							t = UIAddName.getRootAsUIAddName(bb)
						Case UIEventType.SCALAR, UIEventType.ARRAY
							t = FlatArray.getRootAsFlatArray(bb)

						'TODO
						Case Else
							Throw New Exception("Unknown or not yet implemented event type: " & e.eventType())
					End Select

					'TODO do we need to close file here?

					[out].Add(New Pair(Of UIEvent, Table)(e, t))
				Loop
				Return [out]
			End Using
		End Function

		Public Overridable Function registeredEventName(ByVal name As String) As Boolean
			Return indexNameMap.ContainsKey(name)
		End Function

		Public Overridable Function registerEventNameQuiet(ByVal name As String) As Long
			Preconditions.checkState(Not registeredEventName(name), "Event name ""%s"" has already been registered", name)
			Try
				Return registerEventName(name)
			Catch e As IOException
				Throw New Exception("Error writing to log file", e)
			End Try
		End Function

		''' <summary>
		''' Register the event name - "accuracy", "loss", etc for later use in recording events. </summary>
		''' <param name="name"> Name to register </param>
		''' <returns> Number of bytes written </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public long registerEventName(String name) throws java.io.IOException
		Public Overridable Function registerEventName(ByVal name As String) As Long
			Preconditions.checkState(endStaticInfoOffset >= 0, "Cannot write name - have not written end of static info marker")

			Dim fbb As New FlatBufferBuilder(0)
			Dim time As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim offset As Integer = UIEvent.createUIEvent(fbb, UIEventType.ADD_NAME, UIEventSubtype.NONE, -1, time, 0, 0, CShort(-1), 0, 0)
			fbb.finish(offset)

			Dim fbb2 As New FlatBufferBuilder(0)
			Dim idx As Integer = nameIndexCounter.getAndIncrement()
			nameIndexMap(idx) = name
			indexNameMap(name) = idx
			Dim strOffset As Integer = fbb2.createString(name)
			Dim offset2 As Integer = UIAddName.createUIAddName(fbb2, idx, strOffset)
			fbb2.finish(offset2)

			Dim l As Long = append(fbb, fbb2)
			Return l
		End Function

		''' <summary>
		''' Write a single scalar event to the file </summary>
		''' <param name="name">      Name of the event. Must be registered by <seealso cref="registerEventName(String)"/> first! </param>
		''' <param name="time">      Timestamp </param>
		''' <param name="iteration"> Iteration for the event </param>
		''' <param name="epoch">     Epoch for the event </param>
		''' <param name="scalar">    Scalar value to write </param>
		''' <returns>          Number of bytes written </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public long writeScalarEvent(String name, EventSubtype subtype, long time, int iteration, int epoch, Number scalar) throws java.io.IOException
		Public Overridable Function writeScalarEvent(ByVal name As String, ByVal subtype As EventSubtype, ByVal time As Long, ByVal iteration As Integer, ByVal epoch As Integer, ByVal scalar As Number) As Long
			'TODO add support for plugin, variable and frame/iter
			Preconditions.checkState(indexNameMap.ContainsKey(name), "Name ""%s"" not yet registered", name)
			Dim idx As Integer = indexNameMap(name)
			Dim fbb As New FlatBufferBuilder(0)
			Dim offset As Integer = UIEvent.createUIEvent(fbb, UIEventType.SCALAR, subtype.asUIEventSubtype(), idx, time, iteration, epoch, CShort(-1), 0, 0)
			fbb.finish(offset)

			Dim fbb2 As New FlatBufferBuilder(0)
			Dim offset2 As Integer = Nd4j.scalar(scalar).toFlatArray(fbb2)
			fbb2.finish(offset2)

			Return append(fbb, fbb2)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public long writeHistogramEventDiscrete(@NonNull String name, EventSubtype subtype, long time, int iteration, int epoch, List<String> binLabels, @NonNull INDArray y) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Function writeHistogramEventDiscrete(ByVal name As String, ByVal subtype As EventSubtype, ByVal time As Long, ByVal iteration As Integer, ByVal epoch As Integer, ByVal binLabels As IList(Of String), ByVal y As INDArray) As Long
			Preconditions.checkState(binLabels Is Nothing OrElse binLabels.Count = y.length(), "Number of bin labels (if present) must " & "be same as Y array length - got %s bins, array shape %ndShape", (If(binLabels Is Nothing, 0L, binLabels.Count)), y.length())
			Preconditions.checkState(y.rank() = 1, "Y array must be rank 1, got Y array with shape %ndShape", y)

			'TODO add support for plugin, variable and frame/iter
			Preconditions.checkState(indexNameMap.ContainsKey(name), "Name ""%s"" not yet registered", name)
			Dim idx As Integer = indexNameMap(name)

			Dim fbb As New FlatBufferBuilder(0)
			Dim offset As Integer = UIEvent.createUIEvent(fbb, UIEventType.HISTOGRAM, subtype.asUIEventSubtype(), idx, time, iteration, epoch, CShort(-1), 0, 0)
			fbb.finish(offset)

			Dim fbb2 As New FlatBufferBuilder(0)
			Dim yOffset As Integer = y.toFlatArray(fbb2)
			Dim binLabelsOffset As Integer = 0
			If binLabels IsNot Nothing Then
				Dim str(binLabels.Count - 1) As Integer
				For i As Integer = 0 To binLabels.Count - 1
					Dim s As String = binLabels(i)
					If s Is Nothing Then
						s = ""
					End If
					str(i) = fbb2.createString(s)
				Next i
				binLabelsOffset = UIHistogram.createBinlabelsVector(fbb2, str)
			End If
			Dim offset2 As Integer = UIHistogram.createUIHistogram(fbb2, UIHistogramType.DISCRETE, y.length(), 0, yOffset, binLabelsOffset)
			fbb2.finish(offset2)

			Return append(fbb, fbb2)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public long writeHistogramEventEqualSpacing(String name, EventSubtype subtype, long time, int iteration, int epoch, double min, double max, org.nd4j.linalg.api.ndarray.INDArray y) throws java.io.IOException
		Public Overridable Function writeHistogramEventEqualSpacing(ByVal name As String, ByVal subtype As EventSubtype, ByVal time As Long, ByVal iteration As Integer, ByVal epoch As Integer, ByVal min As Double, ByVal max As Double, ByVal y As INDArray) As Long
			Preconditions.checkState(y.rank() = 1, "Y array must be rank 1, got Y array with shape %ndShape", y)
			Preconditions.checkState(max > min, "Maximum histogram value must be greater than minimum - got max=%s, min=%s", max, min)

			'TODO add support for plugin, variable and frame/iter
			'TODO: Code duplication for histogram methods...
			Preconditions.checkState(indexNameMap.ContainsKey(name), "Name ""%s"" not yet registered", name)
			Dim idx As Integer = indexNameMap(name)

			Dim fbb As New FlatBufferBuilder(0)
			Dim offset As Integer = UIEvent.createUIEvent(fbb, UIEventType.HISTOGRAM, subtype.asUIEventSubtype(), idx, time, iteration, epoch, CShort(-1), 0, 0)
			fbb.finish(offset)

			Dim fbb2 As New FlatBufferBuilder(0)
			Dim yOffset As Integer = y.toFlatArray(fbb2)

			Dim binRangesArr As INDArray = Nd4j.createFromArray(min, max)
			Dim binRangesOffset As Integer = binRangesArr.toFlatArray(fbb2)

			Dim offset2 As Integer = UIHistogram.createUIHistogram(fbb2, UIHistogramType.EQUAL_SPACING, y.length(), binRangesOffset, yOffset, 0)
			fbb2.finish(offset2)

			Return append(fbb, fbb2)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public long writeHistogramEventCustomBins(String name, EventSubtype subtype, long time, int iteration, int epoch, org.nd4j.linalg.api.ndarray.INDArray bins, org.nd4j.linalg.api.ndarray.INDArray y) throws java.io.IOException
		Public Overridable Function writeHistogramEventCustomBins(ByVal name As String, ByVal subtype As EventSubtype, ByVal time As Long, ByVal iteration As Integer, ByVal epoch As Integer, ByVal bins As INDArray, ByVal y As INDArray) As Long
			Preconditions.checkState(y.rank() = 1, "Y array must be rank 1, got Y array with shape %ndShape", y)
			Preconditions.checkState(bins.rank() = 2, "Bins array must have shape [2,numBins], got bins array with shape %ndShape", bins)
			Preconditions.checkState(y.length() = bins.size(1), "Bins array must have shape [2,numBins], where numBins must match y.length()=%s, got bins array with shape %ndShape", y.length(), bins)

			'TODO add support for plugin, variable and frame/iter
			'TODO: Code duplication for histogram methods...
			Preconditions.checkState(indexNameMap.ContainsKey(name), "Name ""%s"" not yet registered", name)
			Dim idx As Integer = indexNameMap(name)

			Dim fbb As New FlatBufferBuilder(0)
			Dim offset As Integer = UIEvent.createUIEvent(fbb, UIEventType.HISTOGRAM, subtype.asUIEventSubtype(), idx, time, iteration, epoch, CShort(-1), 0, 0)
			fbb.finish(offset)

			Dim fbb2 As New FlatBufferBuilder(0)
			Dim yOffset As Integer = y.toFlatArray(fbb2)

			Dim binRangesOffset As Integer = bins.toFlatArray(fbb2)

			Dim offset2 As Integer = UIHistogram.createUIHistogram(fbb2, UIHistogramType.CUSTOM, y.length(), binRangesOffset, yOffset, 0)
			fbb2.finish(offset2)

			Return append(fbb, fbb2)
		End Function

		''' <summary>
		''' Encode the header as a UIStaticInfoRecord instance for the specific <seealso cref="UIEventType"/> </summary>
		''' <param name="type"> UIEventType </param>
		Protected Friend Overridable Function encodeStaticHeader(ByVal type As SByte) As Pair(Of Integer, FlatBufferBuilder)
			Dim fbb As New FlatBufferBuilder(12)

			Dim staticInfoOffset As Integer = UIStaticInfoRecord.createUIStaticInfoRecord(fbb, type)
			fbb.finish(staticInfoOffset)
			Dim lengthHeader As Integer = fbb.offset() 'MUST be called post finish to get real length
			Return New Pair(Of Integer, FlatBufferBuilder)(lengthHeader, fbb)
		End Function

		Protected Friend Overridable Function encodeGraphStructure(ByVal fbb As FlatBufferBuilder, ByVal sd As SameDiff) As Integer
			'Create inputs list:
			Dim inputs As IList(Of String) = sd.inputs()
			Dim inputListStrOffsets(inputs.Count - 1) As Integer
			For i As Integer = 0 To inputs.Count - 1
				inputListStrOffsets(i) = fbb.createString(inputs(i))
			Next i
			Dim inputsOffset As Integer = UIGraphStructure.createInputsVector(fbb, inputListStrOffsets)

			'Create inputs pair list:
			Dim inputPairOffset As Integer = -1

			'Create outputs list:
			Dim outputs As IList(Of String) = sd.outputs()
			Dim outputsOffset As Integer = 0
			If outputs IsNot Nothing AndAlso outputs.Count > 0 Then
				Dim outputListStrOffsets(outputs.Count - 1) As Integer
				For i As Integer = 0 To outputListStrOffsets.Length - 1
					outputListStrOffsets(i) = fbb.createString(outputs(i))
				Next i
				outputsOffset = UIGraphStructure.createInputsVector(fbb, outputListStrOffsets)
			End If


			'Create variables list
			Dim varMap As IDictionary(Of String, Variable) = sd.getVariables()
			Dim varListOffsets(varMap.Count - 1) As Integer
			Dim count As Integer = 0
			For Each e As KeyValuePair(Of String, Variable) In varMap.SetOfKeyValuePairs()
'JAVA TO VB CONVERTER NOTE: The variable intPair was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
				Dim intPair_Conflict As Integer = 0
				Dim name As Integer = fbb.createString(e.Key)

				Dim outputOfOp As String = e.Value.getOutputOfOp()
				Dim outputOfOpIdx As Integer = 0
				If outputOfOp IsNot Nothing Then
					outputOfOpIdx = fbb.createString(outputOfOp)
				End If

				Dim inputsForOps As IList(Of String) = e.Value.getInputsForOp()
				Dim inputsForOpIdx As Integer = 0
				If inputsForOps IsNot Nothing AndAlso inputsForOps.Count > 0 Then
					Dim idx() As Integer = encodeStrings(fbb, inputsForOps)
					inputsForOpIdx = UIVariable.createInputsForOpVector(fbb, idx)
				End If

				Dim controlDepsForOp As IList(Of String) = e.Value.getControlDepsForOp()
				Dim controlDepsForOpIdx As Integer = 0
				If controlDepsForOp IsNot Nothing AndAlso controlDepsForOp.Count > 0 Then
					Dim idx() As Integer = encodeStrings(fbb, controlDepsForOp)
					controlDepsForOpIdx = UIVariable.createInputsForOpVector(fbb, idx)
				End If

				Dim controlDepsForVar As IList(Of String) = e.Value.getControlDepsForVar()
				Dim controlDepsForVarIdx As Integer = 0
				If controlDepsForVar IsNot Nothing AndAlso controlDepsForVar.Count > 0 Then
					Dim idx() As Integer = encodeStrings(fbb, controlDepsForVar)
					controlDepsForVarIdx = UIVariable.createInputsForOpVector(fbb, idx)
				End If

				Dim dt As DataType = e.Value.getVariable().dataType()
				Dim dtVal As SByte = FlatBuffersMapper.getDataTypeAsByte(dt)

				Dim shape() As Long = e.Value.getVariable().getShape()
				Dim shapeOffset As Integer = 0
				If shape IsNot Nothing Then
					shapeOffset = UIVariable.createShapeVector(fbb, shape)
				End If

				Dim controlDepsIdx As Integer = 0
				If e.Value.getControlDeps() IsNot Nothing Then
					Dim cds As IList(Of String) = e.Value.getControlDeps()
					If cds.Count > 0 Then
						Dim cdIdxs(cds.Count - 1) As Integer
						For i As Integer = 0 To cdIdxs.Length - 1
							cdIdxs(i) = fbb.createString(cds(i))
						Next i
						controlDepsIdx = UIVariable.createControlDepsVector(fbb, cdIdxs)
					End If
				End If

				Dim uiExtraLabelOffset As Integer = 0 'String value - "extra" information to be shown in label. Currently unused
				Dim constantValueOffset As Integer = 0
				If e.Value.getVariable().getVariableType() = VariableType.CONSTANT Then
					Dim arr As INDArray = e.Value.getVariable().getArr()
					If arr IsNot Nothing AndAlso arr.length() < 1000 Then
						constantValueOffset = arr.toFlatArray(fbb)
					End If
				End If

				Dim uiVariableIdx As Integer = UIVariable.createUIVariable(fbb, intPair_Conflict, name, FlatBuffersMapper.toVarType(e.Value.getVariable().getVariableType()), dtVal, shapeOffset, controlDepsIdx, outputOfOpIdx, inputsForOpIdx, controlDepsForOpIdx, controlDepsForVarIdx, 0, uiExtraLabelOffset, constantValueOffset)

'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: varListOffsets[count++] = uiVariableIdx;
				varListOffsets(count) = uiVariableIdx
					count += 1
			Next e
			Dim outputsListOffset As Integer = UIGraphStructure.createVariablesVector(fbb, varListOffsets)


			'Create ops list
			Dim opMap As IDictionary(Of String, SameDiffOp) = sd.getOps()
			count = 0
			Dim opListOffsets(opMap.Count - 1) As Integer
			For Each e As KeyValuePair(Of String, SameDiffOp) In opMap.SetOfKeyValuePairs()

				Dim nameIdx As Integer = fbb.createString(e.Key)
				Dim opName As String = e.Value.getOp().opName()
				Dim opNameIdx As Integer = fbb.createString(opName)

				'Op input variables
				Dim inputsIdx As Integer = 0
				Dim opInputs As IList(Of String) = e.Value.getInputsToOp()
				If opInputs IsNot Nothing AndAlso opInputs.Count > 0 Then
					Dim idx() As Integer = encodeStrings(fbb, opInputs)
					inputsIdx = UIOp.createInputsVector(fbb, idx)
				End If

				'Op output variables
				Dim outputsIdx As Integer = 0
				Dim opOutputs As IList(Of String) = e.Value.getOutputsOfOp()
				If opOutputs IsNot Nothing AndAlso opOutputs.Count > 0 Then
					Dim idx() As Integer = encodeStrings(fbb, opOutputs)
					outputsIdx = UIOp.createOutputsVector(fbb, idx)
				End If

				Dim controlDepIdxs As Integer = 0
				Dim opCDeps As IList(Of String) = e.Value.getControlDeps()
				If opCDeps IsNot Nothing AndAlso opCDeps.Count > 0 Then
					Dim idx() As Integer = encodeStrings(fbb, opCDeps)
					controlDepIdxs = UIOp.createControlDepsVector(fbb, idx)
				End If

				Dim extraLabelOffset As Integer = 0
				Dim df As DifferentialFunction = e.Value.getOp()
				If TypeOf df Is Enter OrElse TypeOf df Is [Exit] OrElse TypeOf df Is NextIteration Then 'Enter, Exit, NextIteration
					Dim frame As String = DirectCast(df, BaseCompatOp).FrameName
					If frame IsNot Nothing Then
						Dim extra As String = "Frame: """ & frame & """"
						extraLabelOffset = fbb.createString(extra)
					End If
				End If

'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: opListOffsets[count++] = UIOp.createUIOp(fbb, nameIdx, opNameIdx, inputsIdx, outputsIdx, controlDepIdxs, extraLabelOffset);
				opListOffsets(count) = UIOp.createUIOp(fbb, nameIdx, opNameIdx, inputsIdx, outputsIdx, controlDepIdxs, extraLabelOffset)
					count += 1

			Next e
			Dim opsListOffset As Integer = UIGraphStructure.createOpsVector(fbb, opListOffsets)

			Return UIGraphStructure.createUIGraphStructure(fbb, inputsOffset, inputPairOffset, outputsOffset, outputsListOffset, opsListOffset)
		End Function

		Private Function encodeStrings(ByVal fbb As FlatBufferBuilder, ByVal list As IList(Of String)) As Integer()
			If list Is Nothing OrElse list.Count = 0 Then
				Return Nothing
			End If
			Dim idx(list.Count - 1) As Integer
			For i As Integer = 0 To idx.Length - 1
				idx(i) = fbb.createString(list(i))
			Next i
			Return idx
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private long append(com.google.flatbuffers.FlatBufferBuilder h, com.google.flatbuffers.FlatBufferBuilder c) throws java.io.IOException
		Private Function append(ByVal h As FlatBufferBuilder, ByVal c As FlatBufferBuilder) As Long
			Dim bb1 As ByteBuffer = h.dataBuffer()
			Dim bb2 As ByteBuffer = (If(c Is Nothing, Nothing, c.dataBuffer()))

			Using f As New java.io.RandomAccessFile(file, "rw"), fc As java.nio.channels.FileChannel = f.getChannel(), lock As java.nio.channels.FileLock = fc.lock()
				'TODO can we make this more efficient - use a single byte buffer?

				'Seek to end for append
				f.seek(f.length())
				Dim startPos As Long = f.getFilePointer()

				'Write header - length of SystemInfo header, length of content header
				Dim header As ByteBuffer = ByteBuffer.allocate(8) '8 bytes = 2x 4 byte integers
				Dim l1 As Integer = bb1.remaining()
				Dim l2 As Integer = If(bb2 Is Nothing, 0, bb2.remaining())
				header.putInt(l1)
				header.putInt(l2)
				Dim buffer As Buffer = CType(header, Buffer)
				buffer.flip()

				'System.out.println("Lengths - header, content: " + l1 + ", " + l2);

				Dim w1 As Integer = fc.write(header)
				Dim w2 As Integer = fc.write(bb1)
				Dim w3 As Integer = If(bb2 Is Nothing, 0, fc.write(bb2))
				Dim total As Long = w1 + w2 + w3
				'System.out.println("Wrote " + total + " bytes starting at position " + startPos);
				'System.out.println("Post writing file length: " + file.length());
				Return total
			End Using
		End Function



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public static class StaticInfo
		Public Class StaticInfo
			Friend ReadOnly data As IList(Of Pair(Of UIStaticInfoRecord, Table))
			Friend ReadOnly fileOffset As Long
		End Class
	End Class

End Namespace