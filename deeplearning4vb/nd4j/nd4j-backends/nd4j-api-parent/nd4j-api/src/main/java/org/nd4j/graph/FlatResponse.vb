﻿Imports com.google.flatbuffers

' automatically generated by the FlatBuffers compiler, do not modify
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
'ORIGINAL LINE: @SuppressWarnings("unused") public final class FlatResponse extends Table
	Public NotInheritable Class FlatResponse
		Inherits Table

	  Public Shared Function getRootAsFlatResponse(ByVal _bb As ByteBuffer) As FlatResponse
		  Return getRootAsFlatResponse(_bb, New FlatResponse())
	  End Function
	  Public Shared Function getRootAsFlatResponse(ByVal _bb As ByteBuffer, ByVal obj As FlatResponse) As FlatResponse
		  _bb.order(ByteOrder.LITTLE_ENDIAN)
		  Return (obj.__assign(_bb.getInt(_bb.position()) + _bb.position(), _bb))
	  End Function
	  Public Sub __init(ByVal _i As Integer, ByVal _bb As ByteBuffer)
		  bb_pos = _i
		  bb = _bb
		  vtable_start = bb_pos - bb.getInt(bb_pos)
		  vtable_size = bb.getShort(vtable_start)
	  End Sub
	  Public Function __assign(ByVal _i As Integer, ByVal _bb As ByteBuffer) As FlatResponse
		  __init(_i, _bb)
		  Return Me
	  End Function

	  Public Function status() As Integer
		  Dim o As Integer = __offset(4)
		  Return If(o <> 0, bb.getInt(o + bb_pos), 0)
	  End Function

	  Public Shared Function createFlatResponse(ByVal builder As FlatBufferBuilder, ByVal status As Integer) As Integer
		builder.startObject(1)
		FlatResponse.addStatus(builder, status)
		Return FlatResponse.endFlatResponse(builder)
	  End Function

	  Public Shared Sub startFlatResponse(ByVal builder As FlatBufferBuilder)
		  builder.startObject(1)
	  End Sub
	  Public Shared Sub addStatus(ByVal builder As FlatBufferBuilder, ByVal status As Integer)
		  builder.addInt(0, status, 0)
	  End Sub
	  Public Shared Function endFlatResponse(ByVal builder As FlatBufferBuilder) As Integer
		Dim o As Integer = builder.endObject()
		Return o
	  End Function
	End Class


End Namespace