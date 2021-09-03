Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Text

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

Namespace org.nd4j.common.util



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings({"rawtypes", "unchecked"}) public class Index implements java.io.Serializable
	<Serializable>
	Public Class Index

		Private Const serialVersionUID As Long = 1160629777026141078L
		Private objects As IDictionary(Of Integer, Object) = New ConcurrentDictionary(Of Integer, Object)()
		Private indexes As IDictionary(Of Object, Integer) = New ConcurrentDictionary(Of Object, Integer)()

		Public Overridable Function add(ByVal o As Object, ByVal idx As Integer) As Boolean
			SyncLock Me
				If TypeOf o Is String AndAlso o.ToString().Length = 0 Then
					Throw New System.ArgumentException("Unable to add the empty string")
				End If
        
				Dim index As Integer? = indexes(o)
				If index Is Nothing Then
					index = idx
					objects(idx) = o
					indexes(o) = index
					Return True
				End If
				Return False
			End SyncLock
		End Function

		Public Overridable Function add(ByVal o As Object) As Boolean
			SyncLock Me
				If TypeOf o Is String AndAlso o.ToString().Length = 0 Then
					Throw New System.ArgumentException("Unable to add the empty string")
				End If
				Dim index As Integer? = indexes(o)
				If index Is Nothing Then
					index = objects.Count
					objects(index) = o
					indexes(o) = index
					Return True
				End If
				Return False
			End SyncLock
		End Function

		Public Overridable Function indexOf(ByVal o As Object) As Integer
			SyncLock Me
				Dim index As Integer? = indexes(o)
				If index Is Nothing Then
					Return -1
				Else
					Return index
				End If
			End SyncLock
		End Function

		Public Overridable Function get(ByVal i As Integer) As Object
			SyncLock Me
				Return objects(i)
			End SyncLock
		End Function

		Public Overridable Function size() As Integer
			Return objects.Count
		End Function

		Public Overrides Function ToString() As String
			Dim buff As New StringBuilder("[")
			Dim sz As Integer = objects.Count
			Dim i As Integer
			For i = 0 To sz - 1
				Dim e As Object = objects(i)
				buff.Append(e)
				If i < (sz - 1) Then
					buff.Append(" , ")
				End If
			Next i
			buff.Append("]")
			Return buff.ToString()

		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

			Dim index As Index = DirectCast(o, Index)

			If If(objects IsNot Nothing, Not objects.Equals(index.objects), index.objects IsNot Nothing) Then
				Return False
			End If
			Return Not (If(indexes IsNot Nothing, Not indexes.Equals(index.indexes), index.indexes IsNot Nothing))

		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = If(objects IsNot Nothing, objects.GetHashCode(), 0)
			result = 31 * result + (If(indexes IsNot Nothing, indexes.GetHashCode(), 0))
			Return result
		End Function
	End Class

End Namespace