Imports System
Imports System.Text
Imports System.Linq

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


	Public MustInherit Class ObjectUtils
		Private Const INITIAL_HASH As Integer = 7
		Private Const MULTIPLIER As Integer = 31
		Private Const EMPTY_STRING As String = ""
		Private Const NULL_STRING As String = "null"
		Private Const ARRAY_START As String = "{"
		Private Const ARRAY_END As String = "}"
		Private Const EMPTY_ARRAY As String = "{}"
		Private Const ARRAY_ELEMENT_SEPARATOR As String = ", "

		Public Sub New()
		End Sub

		Public Shared Function isCheckedException(ByVal ex As Exception) As Boolean
			Return Not (TypeOf ex Is Exception) AndAlso Not (TypeOf ex Is Exception)
		End Function

		Public Shared Function isCompatibleWithThrowsClause(ByVal ex As Exception, ByVal declaredExceptions() As Type) As Boolean
			If Not isCheckedException(ex) Then
				Return True
			Else
				If declaredExceptions IsNot Nothing Then
					For i As Integer = 0 To declaredExceptions.Length - 1
						If declaredExceptions(i).IsAssignableFrom(ex.GetType()) Then
							Return True
						End If
					Next i
				End If

				Return False
			End If
		End Function

		Public Shared Function isArray(ByVal obj As Object) As Boolean
			Return obj IsNot Nothing AndAlso obj.GetType().IsArray
		End Function

		Public Shared Function isEmpty(ByVal array() As Object) As Boolean
			Return array Is Nothing OrElse array.Length = 0
		End Function

		Public Shared Function containsElement(ByVal array() As Object, ByVal element As Object) As Boolean
			If array Is Nothing Then
				Return False
			Else
				Dim arr$() As Object = array
				Dim len$ As Integer = array.Length

				For i$ As Integer = 0 To len$ - 1
					Dim arrayEle As Object = arr$(i$)
					If nullSafeEquals(arrayEle, element) Then
						Return True
					End If
				Next i$

				Return False
			End If
		End Function

		Public Shared Function containsConstant(Of T1)(ByVal enumValues() As [Enum](Of T1), ByVal constant As String) As Boolean
			Return containsConstant(enumValues, constant, False)
		End Function

		Public Shared Function containsConstant(Of T1)(ByVal enumValues() As [Enum](Of T1), ByVal constant As String, ByVal caseSensitive As Boolean) As Boolean
			Dim arr$() As [Enum] = enumValues
			Dim len$ As Integer = enumValues.Length
			Dim i$ As Integer = 0

			Do
				If i$ >= len$ Then
					Return False
				End If

				Dim candidate As [Enum] = arr$(i$)
				If caseSensitive Then
					If candidate.ToString().Equals(constant) Then
						Exit Do
					End If
				ElseIf candidate.ToString().Equals(constant, StringComparison.OrdinalIgnoreCase) Then
					Exit Do
				End If

				i$ += 1
			Loop

			Return True
		End Function

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: public static <E extends @Enum<?>> E caseInsensitiveValueOf(E[] enumValues, String constant)
		Public Shared Function caseInsensitiveValueOf(Of E As [Enum](Of Object))(ByVal enumValues() As E, ByVal constant As String) As E
			Dim arr$() As [Enum] = enumValues
			Dim len$ As Integer = enumValues.Length

			For i$ As Integer = 0 To len$ - 1
				Dim candidate As [Enum] = arr$(i$)
				If candidate.ToString().Equals(constant, StringComparison.OrdinalIgnoreCase) Then
					Return CType(candidate, E)
				End If
			Next i$

			Throw New System.ArgumentException(String.Format("constant [{0}] does not exist in enum opType {1}", New Object() {constant, enumValues.GetType().GetElementType().getName()}))
		End Function

		Public Shared Function addObjectToArray(Of A, O As A)(ByVal array() As A, ByVal obj As O) As A()
			Dim compType As Type = GetType(Object)
			If array IsNot Nothing Then
				compType = array.GetType().GetElementType()
			ElseIf obj IsNot Nothing Then
				compType = obj.GetType()
			End If

			Dim newArrLength As Integer = If(array IsNot Nothing, array.Length + 1, 1)
			Dim newArr() As Object = CType(Array.CreateInstance(compType, newArrLength), Object())
			If array IsNot Nothing Then
				Array.Copy(array, 0, newArr, 0, array.Length)
			End If

			newArr(newArr.Length - 1) = obj
			Return DirectCast(newArr, A())
		End Function

		Public Shared Function toObjectArray(ByVal source As Object) As Object()
			If TypeOf source Is Object() Then
				Return DirectCast(source, Object())
			ElseIf source Is Nothing Then
				Return New Object(){}
			ElseIf Not source.GetType().IsArray Then
				Throw New System.ArgumentException("Source is not an array: " & source)
			Else
				Dim length As Integer = Array.getLength(source)
				If length = 0 Then
					Return New Object(){}
				Else
					Dim wrapperType As Type = Array.get(source, 0).GetType()
					Dim newArray() As Object = CType(Array.CreateInstance(wrapperType, length), Object())

					For i As Integer = 0 To length - 1
						newArray(i) = Array.get(source, i)
					Next i

					Return newArray
				End If
			End If
		End Function

		Public Shared Function nullSafeEquals(ByVal o1 As Object, ByVal o2 As Object) As Boolean
			If o1 Is o2 Then
				Return True
			ElseIf o1 IsNot Nothing AndAlso o2 IsNot Nothing Then
				If o1.Equals(o2) Then
					Return True
				Else
					If o1.GetType().IsArray AndAlso o2.GetType().IsArray Then
						If TypeOf o1 Is Object() AndAlso TypeOf o2 Is Object() Then
							Return CType(o1.SequenceEqual(DirectCast(o2, Object())), Object())
						End If

						If TypeOf o1 Is Boolean() AndAlso TypeOf o2 Is Boolean() Then
							Return CType(o1.SequenceEqual(DirectCast(o2, Boolean())), Boolean())
						End If

						If TypeOf o1 Is SByte() AndAlso TypeOf o2 Is SByte() Then
							Return CType(o1.SequenceEqual(DirectCast(o2, SByte())), SByte())
						End If

						If TypeOf o1 Is Char() AndAlso TypeOf o2 Is Char() Then
							Return CType(o1.SequenceEqual(DirectCast(o2, Char())), Char())
						End If

						If TypeOf o1 Is Double() AndAlso TypeOf o2 Is Double() Then
							Return CType(o1.SequenceEqual(DirectCast(o2, Double())), Double())
						End If

						If TypeOf o1 Is Single() AndAlso TypeOf o2 Is Single() Then
							Return CType(o1.SequenceEqual(DirectCast(o2, Single())), Single())
						End If

						If TypeOf o1 Is Integer() AndAlso TypeOf o2 Is Integer() Then
							Return CType(o1.SequenceEqual(DirectCast(o2, Integer())), Integer())
						End If

						If TypeOf o1 Is Long() AndAlso TypeOf o2 Is Long() Then
							Return CType(o1.SequenceEqual(DirectCast(o2, Long())), Long())
						End If

						If TypeOf o1 Is Short() AndAlso TypeOf o2 Is Short() Then
							Return CType(o1.SequenceEqual(DirectCast(o2, Short())), Short())
						End If
					End If

					Return False
				End If
			Else
				Return False
			End If
		End Function

		Public Shared Function nullSafeHashCode(ByVal obj As Object) As Integer
			If obj Is Nothing Then
				Return 0
			Else
				If obj.GetType().IsArray Then
					If TypeOf obj Is Object() Then
						Return nullSafeHashCode(DirectCast(obj, Object()))
					End If

					If TypeOf obj Is Boolean() Then
						Return nullSafeHashCode(DirectCast(obj, Boolean()))
					End If

					If TypeOf obj Is SByte() Then
						Return nullSafeHashCode(DirectCast(obj, SByte()))
					End If

					If TypeOf obj Is Char() Then
						Return nullSafeHashCode(DirectCast(obj, Char()))
					End If

					If TypeOf obj Is Double() Then
						Return nullSafeHashCode(DirectCast(obj, Double()))
					End If

					If TypeOf obj Is Single() Then
						Return nullSafeHashCode(DirectCast(obj, Single()))
					End If

					If TypeOf obj Is Integer() Then
						Return nullSafeHashCode(DirectCast(obj, Integer()))
					End If

					If TypeOf obj Is Long() Then
						Return nullSafeHashCode(DirectCast(obj, Long()))
					End If

					If TypeOf obj Is Short() Then
						Return nullSafeHashCode(DirectCast(obj, Short()))
					End If
				End If

				Return obj.GetHashCode()
			End If
		End Function

		Public Shared Function nullSafeHashCode(ByVal array() As Object) As Integer
			If array Is Nothing Then
				Return 0
			Else
				Dim hash As Integer = 7
				Dim arraySize As Integer = array.Length

				For i As Integer = 0 To arraySize - 1
					hash = 31 * hash + nullSafeHashCode(array(i))
				Next i

				Return hash
			End If
		End Function

		Public Shared Function nullSafeHashCode(ByVal array() As Boolean) As Integer
			If array Is Nothing Then
				Return 0
			Else
				Dim hash As Integer = 7
				Dim arraySize As Integer = array.Length

				For i As Integer = 0 To arraySize - 1
					hash = 31 * hash + hashCode(array(i))
				Next i

				Return hash
			End If
		End Function

		Public Shared Function nullSafeHashCode(ByVal array() As SByte) As Integer
			If array Is Nothing Then
				Return 0
			Else
				Dim hash As Integer = 7
				Dim arraySize As Integer = array.Length

				For i As Integer = 0 To arraySize - 1
					hash = 31 * hash + array(i)
				Next i

				Return hash
			End If
		End Function

		Public Shared Function nullSafeHashCode(ByVal array() As Char) As Integer
			If array Is Nothing Then
				Return 0
			Else
				Dim hash As Integer = 7
				Dim arraySize As Integer = array.Length

				For i As Integer = 0 To arraySize - 1
					hash = 31 * hash + AscW(array(i))
				Next i

				Return hash
			End If
		End Function

		Public Shared Function nullSafeHashCode(ByVal array() As Double) As Integer
			If array Is Nothing Then
				Return 0
			Else
				Dim hash As Integer = 7
				Dim arraySize As Integer = array.Length

				For i As Integer = 0 To arraySize - 1
					hash = 31 * hash + hashCode(array(i))
				Next i

				Return hash
			End If
		End Function

		Public Shared Function nullSafeHashCode(ByVal array() As Single) As Integer
			If array Is Nothing Then
				Return 0
			Else
				Dim hash As Integer = 7
				Dim arraySize As Integer = array.Length

				For i As Integer = 0 To arraySize - 1
					hash = 31 * hash + hashCode(array(i))
				Next i

				Return hash
			End If
		End Function

		Public Shared Function nullSafeHashCode(ByVal array() As Integer) As Integer
			If array Is Nothing Then
				Return 0
			Else
				Dim hash As Integer = 7
				Dim arraySize As Integer = array.Length

				For i As Integer = 0 To arraySize - 1
					hash = 31 * hash + array(i)
				Next i

				Return hash
			End If
		End Function

		Public Shared Function nullSafeHashCode(ByVal array() As Long) As Integer
			If array Is Nothing Then
				Return 0
			Else
				Dim hash As Integer = 7
				Dim arraySize As Integer = array.Length

				For i As Integer = 0 To arraySize - 1
					hash = 31 * hash + hashCode(array(i))
				Next i

				Return hash
			End If
		End Function

		Public Shared Function nullSafeHashCode(ByVal array() As Short) As Integer
			If array Is Nothing Then
				Return 0
			Else
				Dim hash As Integer = 7
				Dim arraySize As Integer = array.Length

				For i As Integer = 0 To arraySize - 1
					hash = 31 * hash + array(i)
				Next i

				Return hash
			End If
		End Function

		Public Shared Function hashCode(ByVal bool As Boolean) As Integer
			Return If(bool, 1231, 1237)
		End Function

		Public Shared Function hashCode(ByVal dbl As Double) As Integer
			Dim bits As Long = System.BitConverter.DoubleToInt64Bits(dbl)
			Return hashCode(bits)
		End Function

		Public Shared Function hashCode(ByVal flt As Single) As Integer
			Return Float.floatToIntBits(flt)
		End Function

		Public Shared Function hashCode(ByVal lng As Long) As Integer
			Return CInt(lng Xor CLng(CULng(lng) >> 32))
		End Function

		Public Shared Function identityToString(ByVal obj As Object) As String
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Return If(obj Is Nothing, "", obj.GetType().FullName & "@" & getIdentityHexString(obj))
		End Function

		Public Shared Function getIdentityHexString(ByVal obj As Object) As String
			Return System.identityHashCode(obj).ToString("x")
		End Function

		Public Shared Function getDisplayString(ByVal obj As Object) As String
			Return If(obj Is Nothing, "", nullSafeToString(obj))
		End Function

		Public Shared Function nullSafeClassName(ByVal obj As Object) As String
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Return If(obj IsNot Nothing, obj.GetType().FullName, "null")
		End Function

		Public Shared Function nullSafeToString(ByVal obj As Object) As String
			If obj Is Nothing Then
				Return "null"
			ElseIf TypeOf obj Is String Then
				Return DirectCast(obj, String)
			ElseIf TypeOf obj Is Object() Then
				Return nullSafeToString(DirectCast(obj, Object()))
			ElseIf TypeOf obj Is Boolean() Then
				Return nullSafeToString(DirectCast(obj, Boolean()))
			ElseIf TypeOf obj Is SByte() Then
				Return nullSafeToString(DirectCast(obj, SByte()))
			ElseIf TypeOf obj Is Char() Then
				Return nullSafeToString(DirectCast(obj, Char()))
			ElseIf TypeOf obj Is Double() Then
				Return nullSafeToString(DirectCast(obj, Double()))
			ElseIf TypeOf obj Is Single() Then
				Return nullSafeToString(DirectCast(obj, Single()))
			ElseIf TypeOf obj Is Integer() Then
				Return nullSafeToString(DirectCast(obj, Integer()))
			ElseIf TypeOf obj Is Long() Then
				Return nullSafeToString(DirectCast(obj, Long()))
			ElseIf TypeOf obj Is Short() Then
				Return nullSafeToString(DirectCast(obj, Short()))
			Else
				Dim str As String = obj.ToString()
				Return If(str IsNot Nothing, str, "")
			End If
		End Function

		Public Shared Function nullSafeToString(ByVal array() As Object) As String
			If array Is Nothing Then
				Return "null"
			Else
				Dim length As Integer = array.Length
				If length = 0 Then
					Return "{}"
				Else
					Dim sb As New StringBuilder()

					For i As Integer = 0 To length - 1
						If i = 0 Then
							sb.Append("{")
						Else
							sb.Append(", ")
						End If

						sb.Append(array(i).ToString())
					Next i

					sb.Append("}")
					Return sb.ToString()
				End If
			End If
		End Function

		Public Shared Function nullSafeToString(ByVal array() As Boolean) As String
			If array Is Nothing Then
				Return "null"
			Else
				Dim length As Integer = array.Length
				If length = 0 Then
					Return "{}"
				Else
					Dim sb As New StringBuilder()

					For i As Integer = 0 To length - 1
						If i = 0 Then
							sb.Append("{")
						Else
							sb.Append(", ")
						End If

						sb.Append(array(i))
					Next i

					sb.Append("}")
					Return sb.ToString()
				End If
			End If
		End Function

		Public Shared Function nullSafeToString(ByVal array() As SByte) As String
			If array Is Nothing Then
				Return "null"
			Else
				Dim length As Integer = array.Length
				If length = 0 Then
					Return "{}"
				Else
					Dim sb As New StringBuilder()

					For i As Integer = 0 To length - 1
						If i = 0 Then
							sb.Append("{")
						Else
							sb.Append(", ")
						End If

						sb.Append(array(i))
					Next i

					sb.Append("}")
					Return sb.ToString()
				End If
			End If
		End Function

		Public Shared Function nullSafeToString(ByVal array() As Char) As String
			If array Is Nothing Then
				Return "null"
			Else
				Dim length As Integer = array.Length
				If length = 0 Then
					Return "{}"
				Else
					Dim sb As New StringBuilder()

					For i As Integer = 0 To length - 1
						If i = 0 Then
							sb.Append("{")
						Else
							sb.Append(", ")
						End If

						sb.Append("'").Append(array(i)).Append("'")
					Next i

					sb.Append("}")
					Return sb.ToString()
				End If
			End If
		End Function

		Public Shared Function nullSafeToString(ByVal array() As Double) As String
			If array Is Nothing Then
				Return "null"
			Else
				Dim length As Integer = array.Length
				If length = 0 Then
					Return "{}"
				Else
					Dim sb As New StringBuilder()

					For i As Integer = 0 To length - 1
						If i = 0 Then
							sb.Append("{")
						Else
							sb.Append(", ")
						End If

						sb.Append(array(i))
					Next i

					sb.Append("}")
					Return sb.ToString()
				End If
			End If
		End Function

		Public Shared Function nullSafeToString(ByVal array() As Single) As String
			If array Is Nothing Then
				Return "null"
			Else
				Dim length As Integer = array.Length
				If length = 0 Then
					Return "{}"
				Else
					Dim sb As New StringBuilder()

					For i As Integer = 0 To length - 1
						If i = 0 Then
							sb.Append("{")
						Else
							sb.Append(", ")
						End If

						sb.Append(array(i))
					Next i

					sb.Append("}")
					Return sb.ToString()
				End If
			End If
		End Function

		Public Shared Function nullSafeToString(ByVal array() As Integer) As String
			If array Is Nothing Then
				Return "null"
			Else
				Dim length As Integer = array.Length
				If length = 0 Then
					Return "{}"
				Else
					Dim sb As New StringBuilder()

					For i As Integer = 0 To length - 1
						If i = 0 Then
							sb.Append("{")
						Else
							sb.Append(", ")
						End If

						sb.Append(array(i))
					Next i

					sb.Append("}")
					Return sb.ToString()
				End If
			End If
		End Function

		Public Shared Function nullSafeToString(ByVal array() As Long) As String
			If array Is Nothing Then
				Return "null"
			Else
				Dim length As Integer = array.Length
				If length = 0 Then
					Return "{}"
				Else
					Dim sb As New StringBuilder()

					For i As Integer = 0 To length - 1
						If i = 0 Then
							sb.Append("{")
						Else
							sb.Append(", ")
						End If

						sb.Append(array(i))
					Next i

					sb.Append("}")
					Return sb.ToString()
				End If
			End If
		End Function

		Public Shared Function nullSafeToString(ByVal array() As Short) As String
			If array Is Nothing Then
				Return "null"
			Else
				Dim length As Integer = array.Length
				If length = 0 Then
					Return "{}"
				Else
					Dim sb As New StringBuilder()

					For i As Integer = 0 To length - 1
						If i = 0 Then
							sb.Append("{")
						Else
							sb.Append(", ")
						End If

						sb.Append(array(i))
					Next i

					sb.Append("}")
					Return sb.ToString()
				End If
			End If
		End Function
	End Class

End Namespace