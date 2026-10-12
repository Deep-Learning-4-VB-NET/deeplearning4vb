Imports System
Imports val = lombok.val
Imports BaseDataBuffer = org.nd4j.linalg.api.buffer.BaseDataBuffer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
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

Namespace org.nd4j.linalg.api.ndarray



	''' <summary>
	''' @author Susan Eraly
	''' </summary>
	<Serializable>
	Public Class BaseNDArrayProxy

		''' <summary>
		''' This is a proxy class so that ndarrays can be serialized and deserialized independent of the backend
		''' Be it cpu or gpu
		''' </summary>

		Protected Friend arrayShape() As Long
		Protected Friend length As Long
		Protected Friend arrayOrdering As Char
		<NonSerialized>
		Protected Friend data As DataBuffer

		Public Sub New(ByVal anInstance As INDArray)
			If anInstance.View Then
				anInstance = anInstance.dup(anInstance.ordering())
			End If
			Me.arrayShape = anInstance.shape()
			Me.length = anInstance.length()
			Me.arrayOrdering = anInstance.ordering()
			Me.data = anInstance.data()
		End Sub

		' READ DONE HERE - return an NDArray using the available backend
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private Object readResolve() throws java.io.ObjectStreamException
		Private Function readResolve() As Object
			Return Nd4j.create(data, arrayShape, Nd4j.getStrides(arrayShape, arrayOrdering), 0, arrayOrdering)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws IOException, ClassNotFoundException
		Private Sub readObject(ByVal s As ObjectInputStream)
			Try
				'Should have array shape and ordering here
				s.defaultReadObject()
				'Need to call deser explicitly on data buffer
				read(s)
			Catch e As Exception
				Throw New Exception(e)
			End Try

		End Sub

		'Custom deserialization for Java serialization
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected void read(java.io.ObjectInputStream s) throws IOException, ClassNotFoundException
		Protected Friend Overridable Sub read(ByVal s As ObjectInputStream)
			Dim header As val = BaseDataBuffer.readHeader(s)
			data = Nd4j.createBuffer(header.getRight(), length, False)

			data.read(s, header.getLeft(), header.getMiddle(), header.getRight())
		End Sub

		' WRITE DONE HERE
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream out) throws java.io.IOException
		Private Sub writeObject(ByVal [out] As ObjectOutputStream)
			'takes care of everything but data buffer
			[out].defaultWriteObject()
			write([out])
		End Sub

		'Custom serialization for Java serialization
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected void write(java.io.ObjectOutputStream out) throws java.io.IOException
		Protected Friend Overridable Sub write(ByVal [out] As ObjectOutputStream)
			data.write([out])
		End Sub

	End Class

End Namespace