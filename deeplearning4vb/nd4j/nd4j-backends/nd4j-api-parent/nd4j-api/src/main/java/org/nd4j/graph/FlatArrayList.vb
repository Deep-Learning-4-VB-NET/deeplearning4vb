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
'ORIGINAL LINE: @SuppressWarnings("unused") public final class FlatArrayList extends Table
	Public NotInheritable Class FlatArrayList
		Inherits Table

	  Public Shared Function getRootAsFlatArrayList(ByVal _bb As ByteBuffer) As FlatArrayList
		  Return getRootAsFlatArrayList(_bb, New FlatArrayList())
	  End Function
	  Public Shared Function getRootAsFlatArrayList(ByVal _bb As ByteBuffer, ByVal obj As FlatArrayList) As FlatArrayList
		  _bb.order(ByteOrder.LITTLE_ENDIAN)
		  Return (obj.__assign(_bb.getInt(_bb.position()) + _bb.position(), _bb))
	  End Function
	  Public Sub __init(ByVal _i As Integer, ByVal _bb As ByteBuffer)
		  bb_pos = _i
		  bb = _bb
	  End Sub
	  Public Function __assign(ByVal _i As Integer, ByVal _bb As ByteBuffer) As FlatArrayList
		  __init(_i, _bb)
		  Return Me
	  End Function

	  Public Function list(ByVal j As Integer) As FlatArray
		  Return list(New FlatArray(), j)
	  End Function
	  Public Function list(ByVal obj As FlatArray, ByVal j As Integer) As FlatArray
		  Dim o As Integer = __offset(4)
		  Return If(o <> 0, obj.__assign(__indirect(__vector(o) + j * 4), bb), Nothing)
	  End Function
	  Public Function listLength() As Integer
		  Dim o As Integer = __offset(4)
		  Return If(o <> 0, __vector_len(o), 0)
	  End Function

	  Public Shared Function createFlatArrayList(ByVal builder As FlatBufferBuilder, ByVal listOffset As Integer) As Integer
		builder.startObject(1)
		FlatArrayList.addList(builder, listOffset)
		Return FlatArrayList.endFlatArrayList(builder)
	  End Function

	  Public Shared Sub startFlatArrayList(ByVal builder As FlatBufferBuilder)
		  builder.startObject(1)
	  End Sub
	  Public Shared Sub addList(ByVal builder As FlatBufferBuilder, ByVal listOffset As Integer)
		  builder.addOffset(0, listOffset, 0)
	  End Sub
	  Public Shared Function createListVector(ByVal builder As FlatBufferBuilder, ByVal data() As Integer) As Integer
		  builder.startVector(4, data.Length, 4)
		  For i As Integer = data.Length - 1 To 0 Step -1
			  builder.addOffset(data(i))
		  Next i
		  Return builder.endVector()
	  End Function
	  Public Shared Sub startListVector(ByVal builder As FlatBufferBuilder, ByVal numElems As Integer)
		  builder.startVector(4, numElems, 4)
	  End Sub
	  Public Shared Function endFlatArrayList(ByVal builder As FlatBufferBuilder) As Integer
		Dim o As Integer = builder.endObject()
		Return o
	  End Function
	End Class


End Namespace