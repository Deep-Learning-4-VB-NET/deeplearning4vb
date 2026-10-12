Imports System
Imports System.Linq
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Setter = lombok.Setter
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.datavec.api.writable


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class BytesWritable extends ArrayWritable
	<Serializable>
	Public Class BytesWritable
		Inherits ArrayWritable

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private byte[] content;
		Private content() As SByte

		<NonSerialized>
		Private cached As ByteBuffer

		''' <summary>
		''' Pass in the content for this writable </summary>
		''' <param name="content"> the content for this writable </param>
		Public Sub New(ByVal content() As SByte)
			Me.content = content
		End Sub

		''' <summary>
		''' Convert the underlying contents of this <seealso cref="Writable"/>
		''' to an nd4j <seealso cref="DataBuffer"/>. Note that this is a *copy*
		''' of the underlying buffer.
		''' Also note that <seealso cref="java.nio.ByteBuffer.allocateDirect(Integer)"/>
		''' is used for allocation.
		''' This should be considered an expensive operation.
		''' 
		''' This buffer should be cached when used. Once used, this can be
		''' used in standard Nd4j operations.
		''' 
		''' Beyond that, the reason we have to use allocateDirect
		''' is due to nd4j data buffers being stored off heap (whether on cpu or gpu) </summary>
		''' <param name="type"> the type of the data buffer </param>
		''' <param name="elementSize"> the size of each element in the buffer </param>
		''' <returns> the equivalent nd4j data buffer </returns>
		Public Overridable Function asNd4jBuffer(ByVal type As DataType, ByVal elementSize As Integer) As DataBuffer
			Dim length As Integer = content.Length \ elementSize
			Dim ret As DataBuffer = Nd4j.createBuffer(ByteBuffer.allocateDirect(content.Length),type,length,0)
			For i As Integer = 0 To length - 1
				Select Case type.innerEnumValue
					Case DataType.InnerEnum.DOUBLE
						ret.put(i,getDouble(i))
					Case DataType.InnerEnum.INT
						ret.put(i,getInt(i))
					Case DataType.InnerEnum.FLOAT
						ret.put(i,getFloat(i))
					Case DataType.InnerEnum.LONG
						ret.put(i,getLong(i))
				End Select
			Next i
			Return ret
		End Function

		Public Overrides Function length() As Long
			Return content.Length
		End Function

		Public Overrides Function getDouble(ByVal i As Long) As Double
			Return cachedByteByteBuffer().getDouble(CInt(i) * 8)
		End Function

		Public Overrides Function getFloat(ByVal i As Long) As Single
			Return cachedByteByteBuffer().getFloat(CInt(i) * 4)
		End Function

		Public Overrides Function getInt(ByVal i As Long) As Integer
			Return cachedByteByteBuffer().getInt(CInt(i) * 4)
		End Function

		Public Overrides Function getLong(ByVal i As Long) As Long
			Return cachedByteByteBuffer().getLong(CInt(i) * 8)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void write(java.io.DataOutput out) throws java.io.IOException
		Public Overrides Sub write(ByVal [out] As DataOutput)
			[out].write(content)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void readFields(java.io.DataInput in) throws java.io.IOException
		Public Overrides Sub readFields(ByVal [in] As DataInput)
			[in].readFully(content)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void writeType(java.io.DataOutput out) throws java.io.IOException
		Public Overrides Sub writeType(ByVal [out] As DataOutput)
			[out].writeShort([getType]().typeIdx())
		End Sub

		Public Overrides Function [getType]() As WritableType
			Return WritableType.Bytes
		End Function

		Private Function cachedByteByteBuffer() As ByteBuffer
			If cached Is Nothing Then
				cached = ByteBuffer.wrap(content)
			End If
			Return cached
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If
			Dim that As BytesWritable = DirectCast(o, BytesWritable)
			Return content.SequenceEqual(that.content)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Arrays.hashCode(content)
		End Function
	End Class

End Namespace