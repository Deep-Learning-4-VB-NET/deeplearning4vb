Imports System

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

Namespace org.datavec.api.io

	Public MustInherit Class BinaryComparable
		Implements IComparable(Of BinaryComparable)

		''' <summary>
		''' Return n st bytes 0..n-1 from {#getBytes()} are valid.
		''' </summary>
		Public MustOverride ReadOnly Property Length As Integer

		''' <summary>
		''' Return representative byte array for this instance.
		''' </summary>
		Public MustOverride ReadOnly Property Bytes As SByte()

		''' <summary>
		''' Compare bytes from {#getBytes()}. </summary>
		''' <seealso cref= org.apache.hadoop.io.WritableComparator#compareBytes(byte[],int,int,byte[],int,int) </seealso>
		Public Overridable Function CompareTo(ByVal other As BinaryComparable) As Integer Implements IComparable(Of BinaryComparable).CompareTo
			If Me Is other Then
				Return 0
			End If
			Return WritableComparator.compareBytes(Bytes, 0, Length, other.Bytes, 0, other.Length)
		End Function

		''' <summary>
		''' Compare bytes from {#getBytes()} to those provided.
		''' </summary>
		Public Overridable Function CompareTo(ByVal other() As SByte, ByVal off As Integer, ByVal len As Integer) As Integer Implements IComparable(Of BinaryComparable).CompareTo
			Return WritableComparator.compareBytes(Bytes, 0, Length, other, off, len)
		End Function

		''' <summary>
		''' Return true if bytes from {#getBytes()} match.
		''' </summary>
		Public Overrides Function Equals(ByVal other As Object) As Boolean
			If Not (TypeOf other Is BinaryComparable) Then
				Return False
			End If
			Dim that As BinaryComparable = DirectCast(other, BinaryComparable)
			If Me.Length <> that.Length Then
				Return False
			End If
			Return Me.CompareTo(that) = 0
		End Function

		''' <summary>
		''' Return a hash of the bytes returned from {#getBytes()}. </summary>
		''' <seealso cref= org.apache.hadoop.io.WritableComparator#hashBytes(byte[],int) </seealso>
		Public Overrides Function GetHashCode() As Integer
			Return WritableComparator.hashBytes(Bytes, Length)
		End Function

	End Class

End Namespace