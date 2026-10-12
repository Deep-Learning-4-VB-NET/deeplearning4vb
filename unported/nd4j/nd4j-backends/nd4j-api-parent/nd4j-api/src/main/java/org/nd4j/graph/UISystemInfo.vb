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
'ORIGINAL LINE: @SuppressWarnings("unused") public final class UISystemInfo extends Table
	Public NotInheritable Class UISystemInfo
		Inherits Table

	  Public Shared Function getRootAsUISystemInfo(ByVal _bb As ByteBuffer) As UISystemInfo
		  Return getRootAsUISystemInfo(_bb, New UISystemInfo())
	  End Function
	  Public Shared Function getRootAsUISystemInfo(ByVal _bb As ByteBuffer, ByVal obj As UISystemInfo) As UISystemInfo
		  _bb.order(ByteOrder.LITTLE_ENDIAN)
		  Return (obj.__assign(_bb.getInt(_bb.position()) + _bb.position(), _bb))
	  End Function
	  Public Sub __init(ByVal _i As Integer, ByVal _bb As ByteBuffer)
		  bb_pos = _i
		  bb = _bb
	  End Sub
	  Public Function __assign(ByVal _i As Integer, ByVal _bb As ByteBuffer) As UISystemInfo
		  __init(_i, _bb)
		  Return Me
	  End Function

	  Public Function physicalCores() As Integer
		  Dim o As Integer = __offset(4)
		  Return If(o <> 0, bb.getInt(o + bb_pos), 0)
	  End Function

	  Public Shared Function createUISystemInfo(ByVal builder As FlatBufferBuilder, ByVal physicalCores As Integer) As Integer
		builder.startObject(1)
		UISystemInfo.addPhysicalCores(builder, physicalCores)
		Return UISystemInfo.endUISystemInfo(builder)
	  End Function

	  Public Shared Sub startUISystemInfo(ByVal builder As FlatBufferBuilder)
		  builder.startObject(1)
	  End Sub
	  Public Shared Sub addPhysicalCores(ByVal builder As FlatBufferBuilder, ByVal physicalCores As Integer)
		  builder.addInt(0, physicalCores, 0)
	  End Sub
	  Public Shared Function endUISystemInfo(ByVal builder As FlatBufferBuilder) As Integer
		Dim o As Integer = builder.endObject()
		Return o
	  End Function
	End Class


End Namespace