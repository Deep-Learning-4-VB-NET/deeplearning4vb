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

Namespace org.nd4j.common.io



	Public MustInherit Class Assert
		Public Sub New()
		End Sub

		Public Shared Sub isTrue(ByVal expression As Boolean, ByVal message As String)
			If Not expression Then
				Throw New System.ArgumentException(message)
			End If
		End Sub

		Public Shared Sub isTrue(ByVal expression As Boolean)
			isTrue(expression, "[Assertion failed] - this expression must be true")
		End Sub

		Public Shared Sub isNull(ByVal [object] As Object, ByVal message As String)
			If [object] IsNot Nothing Then
				Throw New System.ArgumentException(message)
			End If
		End Sub

		Public Shared Sub isNull(ByVal [object] As Object)
			isNull([object], "[Assertion failed] - the object argument must be null")
		End Sub

		Public Shared Sub notNull(ByVal [object] As Object, ByVal message As String)
			If [object] Is Nothing Then
				Throw New System.ArgumentException(message)
			End If
		End Sub

		Public Shared Sub notNull(ByVal [object] As Object)
			notNull([object], "[Assertion failed] - this argument is required; it must not be null")
		End Sub

		Public Shared Sub hasLength(ByVal text As String, ByVal message As String)
			If Not StringUtils.hasLength(text) Then
				Throw New System.ArgumentException(message)
			End If
		End Sub

		Public Shared Sub hasLength(ByVal text As String)
			hasLength(text, "[Assertion failed] - this String argument must have length; it must not be null or empty")
		End Sub

		Public Shared Sub hasText(ByVal text As String, ByVal message As String)
			If Not StringUtils.hasText(text) Then
				Throw New System.ArgumentException(message)
			End If
		End Sub

		Public Shared Sub hasText(ByVal text As String)
			hasText(text, "[Assertion failed] - this String argument must have text; it must not be null, empty, or blank")
		End Sub

		Public Shared Sub doesNotContain(ByVal textToSearch As String, ByVal substring As String, ByVal message As String)
			If StringUtils.hasLength(textToSearch) AndAlso StringUtils.hasLength(substring) AndAlso textToSearch.Contains(substring) Then
				Throw New System.ArgumentException(message)
			End If
		End Sub

		Public Shared Sub doesNotContain(ByVal textToSearch As String, ByVal substring As String)
			doesNotContain(textToSearch, substring, "[Assertion failed] - this String argument must not contain the substring [" & substring & "]")
		End Sub

		Public Shared Sub notEmpty(ByVal array() As Object, ByVal message As String)
			If ObjectUtils.isEmpty(array) Then
				Throw New System.ArgumentException(message)
			End If
		End Sub

		Public Shared Sub notEmpty(ByVal array() As Object)
			notEmpty(array, "[Assertion failed] - this array must not be empty: it must contain at least 1 element")
		End Sub

		Public Shared Sub noNullElements(ByVal array() As Object, ByVal message As String)
			If array IsNot Nothing Then
				Dim arr$() As Object = array
				Dim len$ As Integer = array.Length

				For i$ As Integer = 0 To len$ - 1
					Dim element As Object = arr$(i$)
					If element Is Nothing Then
						Throw New System.ArgumentException(message)
					End If
				Next i$
			End If

		End Sub

		Public Shared Sub noNullElements(ByVal array() As Object)
			noNullElements(array, "[Assertion failed] - this array must not contain any null elements")
		End Sub

		Public Shared Sub notEmpty(ByVal collection As System.Collections.ICollection, ByVal message As String)
			If CollectionUtils.isEmpty(collection) Then
				Throw New System.ArgumentException(message)
			End If
		End Sub

		Public Shared Sub notEmpty(ByVal collection As System.Collections.ICollection)
			notEmpty(collection, "[Assertion failed] - this collection must not be empty: it must contain at least 1 element")
		End Sub

		Public Shared Sub notEmpty(ByVal map As System.Collections.IDictionary, ByVal message As String)
			If CollectionUtils.isEmpty(map) Then
				Throw New System.ArgumentException(message)
			End If
		End Sub

		Public Shared Sub notEmpty(ByVal map As System.Collections.IDictionary)
			notEmpty(map, "[Assertion failed] - this map must not be empty; it must contain at least one entry")
		End Sub

		Public Shared Sub isInstanceOf(ByVal clazz As Type, ByVal obj As Object)
			isInstanceOf(clazz, obj, "")
		End Sub

		Public Shared Sub isInstanceOf(ByVal type As Type, ByVal obj As Object, ByVal message As String)
			notNull(type, "Type to check against must not be null")
			If Not type.IsInstanceOfType(obj) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New System.ArgumentException((If(StringUtils.hasLength(message), message & " ", "")) & "Object of class [" & (If(obj IsNot Nothing, obj.GetType().FullName, "null")) & "] must be an instance of " & type)
			End If
		End Sub

		Public Shared Sub isAssignable(ByVal superType As Type, ByVal subType As Type)
			isAssignable(superType, subType, "")
		End Sub

		Public Shared Sub isAssignable(ByVal superType As Type, ByVal subType As Type, ByVal message As String)
			notNull(superType, "Type to check against must not be null")
			If subType Is Nothing OrElse Not superType.IsAssignableFrom(subType) Then
				Throw New System.ArgumentException(message & subType & " is not assignable to " & superType)
			End If
		End Sub

		Public Shared Sub state(ByVal expression As Boolean, ByVal message As String)
			If Not expression Then
				Throw New System.InvalidOperationException(message)
			End If
		End Sub

		Public Shared Sub state(ByVal expression As Boolean)
			state(expression, "[Assertion failed] - this state invariant must be true")
		End Sub
	End Class

End Namespace