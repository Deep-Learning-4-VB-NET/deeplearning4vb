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
'ORIGINAL LINE: @SuppressWarnings("unused") public final class FlatProperties extends Table
	Public NotInheritable Class FlatProperties
		Inherits Table

	  Public Shared Function getRootAsFlatProperties(ByVal _bb As ByteBuffer) As FlatProperties
		  Return getRootAsFlatProperties(_bb, New FlatProperties())
	  End Function
	  Public Shared Function getRootAsFlatProperties(ByVal _bb As ByteBuffer, ByVal obj As FlatProperties) As FlatProperties
		  _bb.order(ByteOrder.LITTLE_ENDIAN)
		  Return (obj.__assign(_bb.getInt(_bb.position()) + _bb.position(), _bb))
	  End Function
	  Public Sub __init(ByVal _i As Integer, ByVal _bb As ByteBuffer)
		  bb_pos = _i
		  bb = _bb
	  End Sub
	  Public Function __assign(ByVal _i As Integer, ByVal _bb As ByteBuffer) As FlatProperties
		  __init(_i, _bb)
		  Return Me
	  End Function

	  Public Function name() As String
		  Dim o As Integer = __offset(4)
		  Return If(o <> 0, __string(o + bb_pos), Nothing)
	  End Function
	  Public Function nameAsByteBuffer() As ByteBuffer
		  Return __vector_as_bytebuffer(4, 1)
	  End Function
	  Public Function nameInByteBuffer(ByVal _bb As ByteBuffer) As ByteBuffer
		  Return __vector_in_bytebuffer(_bb, 4, 1)
	  End Function
	  Public Function i(ByVal j As Integer) As Integer
		  Dim o As Integer = __offset(6)
		  Return If(o <> 0, bb.getInt(__vector(o) + j * 4), 0)
	  End Function
	  Public Function iLength() As Integer
		  Dim o As Integer = __offset(6)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function iAsByteBuffer() As ByteBuffer
		  Return __vector_as_bytebuffer(6, 4)
	  End Function
	  Public Function iInByteBuffer(ByVal _bb As ByteBuffer) As ByteBuffer
		  Return __vector_in_bytebuffer(_bb, 6, 4)
	  End Function
	  Public Function l(ByVal j As Integer) As Long
		  Dim o As Integer = __offset(8)
		  Return If(o <> 0, bb.getLong(__vector(o) + j * 8), 0)
	  End Function
	  Public Function lLength() As Integer
		  Dim o As Integer = __offset(8)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function lAsByteBuffer() As ByteBuffer
		  Return __vector_as_bytebuffer(8, 8)
	  End Function
	  Public Function lInByteBuffer(ByVal _bb As ByteBuffer) As ByteBuffer
		  Return __vector_in_bytebuffer(_bb, 8, 8)
	  End Function
	  Public Function d(ByVal j As Integer) As Double
		  Dim o As Integer = __offset(10)
		  Return If(o <> 0, bb.getDouble(__vector(o) + j * 8), 0)
	  End Function
	  Public Function dLength() As Integer
		  Dim o As Integer = __offset(10)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function dAsByteBuffer() As ByteBuffer
		  Return __vector_as_bytebuffer(10, 8)
	  End Function
	  Public Function dInByteBuffer(ByVal _bb As ByteBuffer) As ByteBuffer
		  Return __vector_in_bytebuffer(_bb, 10, 8)
	  End Function
	  Public Function a(ByVal j As Integer) As FlatArray
		  Return a(New FlatArray(), j)
	  End Function
	  Public Function a(ByVal obj As FlatArray, ByVal j As Integer) As FlatArray
		  Dim o As Integer = __offset(12)
		  Return If(o <> 0, obj.__assign(__indirect(__vector(o) + j * 4), bb), Nothing)
	  End Function
	  Public Function aLength() As Integer
		  Dim o As Integer = __offset(12)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function b(ByVal j As Integer) As Boolean
		  Dim o As Integer = __offset(14)
		  Return If(o <> 0, 0<>bb.get(__vector(o) + j * 1), False)
	  End Function
	  Public Function bLength() As Integer
		  Dim o As Integer = __offset(14)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function bAsByteBuffer() As ByteBuffer
		  Return __vector_as_bytebuffer(14, 1)
	  End Function
	  Public Function bInByteBuffer(ByVal _bb As ByteBuffer) As ByteBuffer
		  Return __vector_in_bytebuffer(_bb, 14, 1)
	  End Function
	  Public Function s(ByVal j As Integer) As String
		  Dim o As Integer = __offset(16)
		  Return If(o <> 0, __string(__vector(o) + j * 4), Nothing)
	  End Function
	  Public Function sLength() As Integer
		  Dim o As Integer = __offset(16)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function shape(ByVal j As Integer) As Integer
		  Dim o As Integer = __offset(18)
		  Return If(o <> 0, bb.getInt(__vector(o) + j * 4), 0)
	  End Function
	  Public Function shapeLength() As Integer
		  Dim o As Integer = __offset(18)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function
	  Public Function shapeAsByteBuffer() As ByteBuffer
		  Return __vector_as_bytebuffer(18, 4)
	  End Function
	  Public Function shapeInByteBuffer(ByVal _bb As ByteBuffer) As ByteBuffer
		  Return __vector_in_bytebuffer(_bb, 18, 4)
	  End Function

	  Public Shared Function createFlatProperties(ByVal builder As FlatBufferBuilder, ByVal nameOffset As Integer, ByVal iOffset As Integer, ByVal lOffset As Integer, ByVal dOffset As Integer, ByVal aOffset As Integer, ByVal bOffset As Integer, ByVal sOffset As Integer, ByVal shapeOffset As Integer) As Integer
		builder.startObject(8)
		FlatProperties.addShape(builder, shapeOffset)
		FlatProperties.addS(builder, sOffset)
		FlatProperties.addB(builder, bOffset)
		FlatProperties.addA(builder, aOffset)
		FlatProperties.addD(builder, dOffset)
		FlatProperties.addL(builder, lOffset)
		FlatProperties.addI(builder, iOffset)
		FlatProperties.addName(builder, nameOffset)
		Return FlatProperties.endFlatProperties(builder)
	  End Function

	  Public Shared Sub startFlatProperties(ByVal builder As FlatBufferBuilder)
		  builder.startObject(8)
	  End Sub
	  Public Shared Sub addName(ByVal builder As FlatBufferBuilder, ByVal nameOffset As Integer)
		  builder.addOffset(0, nameOffset, 0)
	  End Sub
	  Public Shared Sub addI(ByVal builder As FlatBufferBuilder, ByVal iOffset As Integer)
		  builder.addOffset(1, iOffset, 0)
	  End Sub
	  Public Shared Function createIVector(ByVal builder As FlatBufferBuilder, ByVal data() As Integer) As Integer
		  builder.startVector(4, data.Length, 4)
		  For i As Integer = data.Length - 1 To 0 Step -1
			  builder.addInt(data(i))
		  Next i
		  Return builder.endVector()
	  End Function
	  Public Shared Sub startIVector(ByVal builder As FlatBufferBuilder, ByVal numElems As Integer)
		  builder.startVector(4, numElems, 4)
	  End Sub
	  Public Shared Sub addL(ByVal builder As FlatBufferBuilder, ByVal lOffset As Integer)
		  builder.addOffset(2, lOffset, 0)
	  End Sub
	  Public Shared Function createLVector(ByVal builder As FlatBufferBuilder, ByVal data() As Long) As Integer
		  builder.startVector(8, data.Length, 8)
		  For i As Integer = data.Length - 1 To 0 Step -1
			  builder.addLong(data(i))
		  Next i
		  Return builder.endVector()
	  End Function
	  Public Shared Sub startLVector(ByVal builder As FlatBufferBuilder, ByVal numElems As Integer)
		  builder.startVector(8, numElems, 8)
	  End Sub
	  Public Shared Sub addD(ByVal builder As FlatBufferBuilder, ByVal dOffset As Integer)
		  builder.addOffset(3, dOffset, 0)
	  End Sub
	  Public Shared Function createDVector(ByVal builder As FlatBufferBuilder, ByVal data() As Double) As Integer
		  builder.startVector(8, data.Length, 8)
		  For i As Integer = data.Length - 1 To 0 Step -1
			  builder.addDouble(data(i))
		  Next i
		  Return builder.endVector()
	  End Function
	  Public Shared Sub startDVector(ByVal builder As FlatBufferBuilder, ByVal numElems As Integer)
		  builder.startVector(8, numElems, 8)
	  End Sub
	  Public Shared Sub addA(ByVal builder As FlatBufferBuilder, ByVal aOffset As Integer)
		  builder.addOffset(4, aOffset, 0)
	  End Sub
	  Public Shared Function createAVector(ByVal builder As FlatBufferBuilder, ByVal data() As Integer) As Integer
		  builder.startVector(4, data.Length, 4)
		  For i As Integer = data.Length - 1 To 0 Step -1
			  builder.addOffset(data(i))
		  Next i
		  Return builder.endVector()
	  End Function
	  Public Shared Sub startAVector(ByVal builder As FlatBufferBuilder, ByVal numElems As Integer)
		  builder.startVector(4, numElems, 4)
	  End Sub
	  Public Shared Sub addB(ByVal builder As FlatBufferBuilder, ByVal bOffset As Integer)
		  builder.addOffset(5, bOffset, 0)
	  End Sub
	  Public Shared Function createBVector(ByVal builder As FlatBufferBuilder, ByVal data() As Boolean) As Integer
		  builder.startVector(1, data.Length, 1)
		  For i As Integer = data.Length - 1 To 0 Step -1
			  builder.addBoolean(data(i))
		  Next i
		  Return builder.endVector()
	  End Function
	  Public Shared Sub startBVector(ByVal builder As FlatBufferBuilder, ByVal numElems As Integer)
		  builder.startVector(1, numElems, 1)
	  End Sub
	  Public Shared Sub addS(ByVal builder As FlatBufferBuilder, ByVal sOffset As Integer)
		  builder.addOffset(6, sOffset, 0)
	  End Sub
	  Public Shared Function createSVector(ByVal builder As FlatBufferBuilder, ByVal data() As Integer) As Integer
		  builder.startVector(4, data.Length, 4)
		  For i As Integer = data.Length - 1 To 0 Step -1
			  builder.addOffset(data(i))
		  Next i
		  Return builder.endVector()
	  End Function
	  Public Shared Sub startSVector(ByVal builder As FlatBufferBuilder, ByVal numElems As Integer)
		  builder.startVector(4, numElems, 4)
	  End Sub
	  Public Shared Sub addShape(ByVal builder As FlatBufferBuilder, ByVal shapeOffset As Integer)
		  builder.addOffset(7, shapeOffset, 0)
	  End Sub
	  Public Shared Function createShapeVector(ByVal builder As FlatBufferBuilder, ByVal data() As Integer) As Integer
		  builder.startVector(4, data.Length, 4)
		  For i As Integer = data.Length - 1 To 0 Step -1
			  builder.addInt(data(i))
		  Next i
		  Return builder.endVector()
	  End Function
	  Public Shared Sub startShapeVector(ByVal builder As FlatBufferBuilder, ByVal numElems As Integer)
		  builder.startVector(4, numElems, 4)
	  End Sub
	  Public Shared Function endFlatProperties(ByVal builder As FlatBufferBuilder) As Integer
		Dim o As Integer = builder.endObject()
		Return o
	  End Function
	  Public Shared Sub finishFlatPropertiesBuffer(ByVal builder As FlatBufferBuilder, ByVal offset As Integer)
		  builder.finish(offset)
	  End Sub
	  Public Shared Sub finishSizePrefixedFlatPropertiesBuffer(ByVal builder As FlatBufferBuilder, ByVal offset As Integer)
		  builder.finishSizePrefixed(offset)
	  End Sub
	End Class


End Namespace