Imports System
Imports NonNull = lombok.NonNull
Imports org.datavec.api.io
Imports DataInputWrapperStream = org.datavec.api.util.ndarray.DataInputWrapperStream
Imports DataOutputWrapperStream = org.datavec.api.util.ndarray.DataOutputWrapperStream
Imports NdIndexIterator = org.nd4j.linalg.api.iter.NdIndexIterator
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports MathUtils = org.nd4j.common.util.MathUtils

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


	<Serializable>
	Public Class NDArrayWritable
		Inherits ArrayWritable
		Implements WritableComparable

		Public Const NDARRAY_SER_VERSION_HEADER_NULL As SByte = 0
		Public Const NDARRAY_SER_VERSION_HEADER As SByte = 1

		Private array As INDArray = Nothing
		Private hash As Integer? = Nothing

		Public Sub New()
		End Sub

		Public Sub New(ByVal array As INDArray)
			set(array)
		End Sub

		''' <summary>
		''' Deserialize into a row vector of default type.
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void readFields(DataInput in) throws IOException
		Public Overrides Sub readFields(ByVal [in] As DataInput)
			Dim dis As New DataInputStream(New DataInputWrapperStream([in]))
			Dim header As SByte = dis.readByte()
			If header <> NDARRAY_SER_VERSION_HEADER AndAlso header <> NDARRAY_SER_VERSION_HEADER_NULL Then
				Throw New System.InvalidOperationException("Unexpected NDArrayWritable version header - stream corrupt?")
			End If

			If header = NDARRAY_SER_VERSION_HEADER_NULL Then
				array = Nothing
				Return
			End If

			array = Nd4j.read(dis)
			hash = Nothing
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void writeType(DataOutput out) throws IOException
		Public Overrides Sub writeType(ByVal [out] As DataOutput)
			[out].writeShort(WritableType.NDArray.typeIdx())
		End Sub

		Public Overrides Function [getType]() As WritableType
			Return WritableType.NDArray
		End Function

		''' <summary>
		''' Serialize array data linearly.
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void write(DataOutput out) throws IOException
		Public Overrides Sub write(ByVal [out] As DataOutput)
			If array Is Nothing Then
				[out].write(NDARRAY_SER_VERSION_HEADER_NULL)
				Return
			End If

			Dim toWrite As INDArray
			If array.View Then
				toWrite = array.dup()
			Else
				toWrite = array
			End If

			'Write version header: this allows us to maintain backward compatibility in the future,
			' with features such as compression, sparse arrays or changes on the DataVec side
			[out].write(NDARRAY_SER_VERSION_HEADER)
			Nd4j.write(toWrite, New DataOutputStream(New DataOutputWrapperStream([out])))
		End Sub

		Public Overridable Sub set(ByVal array As INDArray)
			Me.array = array
			Me.hash = Nothing
		End Sub

		Public Overridable Function get() As INDArray
			Return array
		End Function

		''' <summary>
		''' Returns true iff <code>o</code> is a ArrayWritable with the same value.
		''' </summary>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Not (TypeOf o Is NDArrayWritable) Then
				Return False
			End If
			Dim io As INDArray = DirectCast(o, NDArrayWritable).get()

			If Me.array Is Nothing AndAlso io IsNot Nothing OrElse Me.array IsNot Nothing AndAlso io Is Nothing Then
				Return False
			End If

			If Me.array Is Nothing Then
				'Both are null
				Return True
			End If

			'For NDArrayWritable: we use strict equality. Otherwise, we can have a.equals(b) but a.hashCode() != b.hashCode()
			Return Me.array.equalsWithEps(io, 0.0)
		End Function

		Public Overrides Function GetHashCode() As Integer
			If hash IsNot Nothing Then
				Return hash
			End If

			'Hashcode needs to be invariant to array order - otherwise, equal arrays can have different hash codes
			' for example, C vs. F order arrays with otherwise identical contents

			If array Is Nothing Then
				hash = 0
				Return hash
			End If

			Dim hash As Integer = Arrays.hashCode(array.shape())
			Dim length As Long = array.length()
			Dim iter As New NdIndexIterator("c"c, array.shape())
			For i As Integer = 0 To length - 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				hash = hash Xor MathUtils.hashCode(array.getDouble(iter.next()))
			Next i

			Me.hash = hash
			Return hash
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public int compareTo(@NonNull Object o)
		Public Overrides Function compareTo(ByVal o As Object) As Integer
			Dim other As NDArrayWritable = DirectCast(o, NDArrayWritable)

			'Conventions used here for ordering NDArrays: x.compareTo(y): -ve if x < y, 0 if x == y, +ve if x > y
			'Null first
			'Then smallest rank first
			'Then smallest length first
			'Then sort by shape
			'Then sort by contents
			'The idea: avoid comparing contents for as long as possible

			If Me.array Is Nothing Then
				If other.array Is Nothing Then
					Return 0
				End If
				Return -1
			End If
			If other.array Is Nothing Then
				Return 1
			End If

			If Me.array.rank() <> other.array.rank() Then
				Return Integer.compare(array.rank(), other.array.rank())
			End If

			If array.length() <> other.array.length() Then
				Return Long.compare(array.length(), other.array.length())
			End If

			Dim i As Integer = 0
			Do While i < array.rank()
				If Long.compare(array.size(i), other.array.size(i)) <> 0 Then
					Return Long.compare(array.size(i), other.array.size(i))
				End If
				i += 1
			Loop

			'At this point: same rank, length, shape
			Dim iter As New NdIndexIterator("c"c, array.shape())
			Do While iter.MoveNext()
				Dim nextPos() As Long = iter.Current
				Dim d1 As Double = array.getDouble(nextPos)
				Dim d2 As Double = other.array.getDouble(nextPos)

				If d1.CompareTo(d2) <> 0 Then
					Return d1.CompareTo(d2)
				End If
			Loop

			'Same rank, length, shape and contents: must be equal
			Return 0
		End Function

		Public Overrides Function ToString() As String
			Return array.ToString()
		End Function

		Public Overrides Function length() As Long
			Return array.data().length()
		End Function

		Public Overrides Function getDouble(ByVal i As Long) As Double
			Return array.data().getDouble(i)
		End Function

		Public Overrides Function getFloat(ByVal i As Long) As Single
			Return array.data().getFloat(i)
		End Function

		Public Overrides Function getInt(ByVal i As Long) As Integer
			Return array.data().getInt(i)
		End Function

		Public Overrides Function getLong(ByVal i As Long) As Long
			Return CLng(Math.Truncate(array.data().getDouble(i)))
		End Function
	End Class

End Namespace