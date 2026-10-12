Imports System
Imports com.google.flatbuffers

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

Namespace org.nd4j.graph


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unused") public final class UIHistogram extends Table
	Public NotInheritable Class UIHistogram
		Inherits Table

	  Public Shared Function getRootAsUIHistogram(ByVal _bb As ByteBuffer) As UIHistogram
		  Return getRootAsUIHistogram(_bb, New UIHistogram())
	  End Function
	  Public Shared Function getRootAsUIHistogram(ByVal _bb As ByteBuffer, ByVal obj As UIHistogram) As UIHistogram
		  _bb.order(ByteOrder.LITTLE_ENDIAN)
		  Return (obj.__assign(_bb.getInt(_bb.position()) + _bb.position(), _bb))
	  End Function
	  Public Sub __init(ByVal _i As Integer, ByVal _bb As ByteBuffer)
		  bb_pos = _i
		  bb = _bb
	  End Sub
	  Public Function __assign(ByVal _i As Integer, ByVal _bb As ByteBuffer) As UIHistogram
		  __init(_i, _bb)
		  Return Me
	  End Function

	  Public Function type() As SByte
		  Dim o As Integer = __offset(4)
		  Return If(o <> 0, bb.get(o + bb_pos), 0)
	  End Function
	  Public Function numbins() As Long
		  Dim o As Integer = __offset(6)
		  Return If(o <> 0, CLng(Math.Truncate(bb.getInt(o + bb_pos))) And &HFFFFFFFFL, 0L)
	  End Function
	  Public Function binranges() As FlatArray
		  Return binranges(New FlatArray())
	  End Function
	  Public Function binranges(ByVal obj As FlatArray) As FlatArray
		  Dim o As Integer = __offset(8)
		  Return If(o <> 0, obj.__assign(__indirect(o + bb_pos), bb), Nothing)
	  End Function
	  Public Function y() As FlatArray
		  Return y(New FlatArray())
	  End Function
	  Public Function y(ByVal obj As FlatArray) As FlatArray
		  Dim o As Integer = __offset(10)
		  Return If(o <> 0, obj.__assign(__indirect(o + bb_pos), bb), Nothing)
	  End Function
	  Public Function binlabels(ByVal j As Integer) As String
		  Dim o As Integer = __offset(12)
		  Return If(o <> 0, __string(__vector(o) + j * 4), Nothing)
	  End Function
	  Public Function binlabelsLength() As Integer
		  Dim o As Integer = __offset(12)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function

	  Public Shared Function createUIHistogram(ByVal builder As FlatBufferBuilder, ByVal type As SByte, ByVal numbins As Long, ByVal binrangesOffset As Integer, ByVal yOffset As Integer, ByVal binlabelsOffset As Integer) As Integer
		builder.startObject(5)
		UIHistogram.addBinlabels(builder, binlabelsOffset)
		UIHistogram.addY(builder, yOffset)
		UIHistogram.addBinranges(builder, binrangesOffset)
		UIHistogram.addNumbins(builder, numbins)
		UIHistogram.addType(builder, type)
		Return UIHistogram.endUIHistogram(builder)
	  End Function

	  Public Shared Sub startUIHistogram(ByVal builder As FlatBufferBuilder)
		  builder.startObject(5)
	  End Sub
	  Public Shared Sub addType(ByVal builder As FlatBufferBuilder, ByVal type As SByte)
		  builder.addByte(0, type, 0)
	  End Sub
	  Public Shared Sub addNumbins(ByVal builder As FlatBufferBuilder, ByVal numbins As Long)
		  builder.addInt(1, CInt(numbins), CInt(0L))
	  End Sub
	  Public Shared Sub addBinranges(ByVal builder As FlatBufferBuilder, ByVal binrangesOffset As Integer)
		  builder.addOffset(2, binrangesOffset, 0)
	  End Sub
	  Public Shared Sub addY(ByVal builder As FlatBufferBuilder, ByVal yOffset As Integer)
		  builder.addOffset(3, yOffset, 0)
	  End Sub
	  Public Shared Sub addBinlabels(ByVal builder As FlatBufferBuilder, ByVal binlabelsOffset As Integer)
		  builder.addOffset(4, binlabelsOffset, 0)
	  End Sub
	  Public Shared Function createBinlabelsVector(ByVal builder As FlatBufferBuilder, ByVal data() As Integer) As Integer
		  builder.startVector(4, data.Length, 4)
		  For i As Integer = data.Length - 1 To 0 Step -1
			  builder.addOffset(data(i))
		  Next i
		  Return builder.endVector()
	  End Function
	  Public Shared Sub startBinlabelsVector(ByVal builder As FlatBufferBuilder, ByVal numElems As Integer)
		  builder.startVector(4, numElems, 4)
	  End Sub
	  Public Shared Function endUIHistogram(ByVal builder As FlatBufferBuilder) As Integer
		Dim o As Integer = builder.endObject()
		Return o
	  End Function
	End Class


End Namespace