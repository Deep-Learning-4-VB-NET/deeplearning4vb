Imports System
Imports System.Collections.Generic
Imports System.IO

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

Namespace org.deeplearning4j.ui.model.stats.impl


	Public Class SbeUtil

		Public Shared ReadOnly UTF8 As Charset = Charset.forName("UTF-8")
		Public Shared ReadOnly EMPTY_BYTES(-1) As SByte 'Also equivalent to "".getBytes(UTF8);

		Private Sub New()
		End Sub

		Public Shared Function length(ByVal bytes() As SByte) As Integer
			If bytes Is Nothing Then
				Return 0
			End If
			Return bytes.Length
		End Function

		Public Shared Function length(ByVal bytes()() As SByte) As Integer
			If bytes Is Nothing Then
				Return 0
			End If
			Dim count As Integer = 0
			For i As Integer = 0 To bytes.Length - 1
				If bytes(i) <> Nothing Then
					count += bytes(i).Length
				End If
			Next i
			Return count
		End Function

		Public Shared Function length(ByVal bytes()()() As SByte) As Integer
			If bytes Is Nothing Then
				Return 0
			End If
			Dim count As Integer = 0
			For Each arr As SByte()() In bytes
				count += length(arr)
			Next arr
			Return count
		End Function

		Public Shared Function length(ByVal str As String) As Integer
			If str Is Nothing Then
				Return 0
			End If
			Return str.Length
		End Function

		Public Shared Function length(ByVal arr() As String) As Integer
			If arr Is Nothing OrElse arr.Length = 0 Then
				Return 0
			End If
			Dim sum As Integer = 0
			For Each s As String In arr
				sum += length(s)
			Next s
			Return sum
		End Function

		Public Shared Function toBytes(ByVal present As Boolean, ByVal str As String) As SByte()
			If Not present OrElse str Is Nothing Then
				Return EMPTY_BYTES
			End If
			Return str.GetBytes(UTF8)
		End Function

		Public Shared Function toBytes(ByVal present As Boolean, ByVal str() As String) As SByte()()
			If str Is Nothing Then
				Return Nothing
			End If
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim b[][] As SByte = new SByte[str.Length][0]
			Dim b()() As SByte = RectangularArrays.RectangularSByteArray(str.Length, 0)
			For i As Integer = 0 To str.Length - 1
				If str(i) Is Nothing Then
					Continue For
				End If
				b(i) = toBytes(present, str(i))
			Next i
			Return b
		End Function

		Public Shared Function toBytes(ByVal map As IDictionary(Of String, String)) As SByte()()()
			If map Is Nothing Then
				Return Nothing
			End If
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim b[][][] As SByte = new SByte[map.Count][2][0]
			Dim b()()() As SByte = RectangularArrays.RectangularSByteArray(map.Count, 2, 0)
			Dim i As Integer = 0
			For Each entry As KeyValuePair(Of String, String) In map.SetOfKeyValuePairs()
				b(i)(0) = toBytes(True, entry.Key)
				b(i)(1) = toBytes(True, entry.Value)
				i += 1
			Next entry
			Return b
		End Function

		Public Shared Function toBytesSerializable(ByVal serializable As Serializable) As SByte()
			If serializable Is Nothing Then
				Return EMPTY_BYTES
			End If
			Dim baos As New MemoryStream()
			Try
					Using oos As New ObjectOutputStream(baos)
					oos.writeObject(serializable)
					End Using
			Catch e As IOException
				Throw New Exception("Unexpected IOException during serialization", e)
			End Try
			Return baos.toByteArray()
		End Function

		Public Shared Function fromBytesSerializable(ByVal bytes() As SByte) As Serializable
			If bytes Is Nothing OrElse bytes.Length = 0 Then
				Return Nothing
			End If
			Dim bais As New MemoryStream(bytes)
			Try
					Using ois As New ObjectInputStream(bais)
					Return CType(ois.readObject(), Serializable)
					End Using
			Catch e As IOException
				Throw New Exception("Unexpected IOException during deserialization", e)
			Catch e As ClassNotFoundException
				Throw New Exception(e)
			End Try
		End Function
	End Class

End Namespace