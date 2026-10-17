Imports System
Imports System.Runtime.CompilerServices
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

Namespace org.nd4vb.common.io

    ''' <summary>
    ''' Miscellaneous object utilities, a copy of Spring's <c>org.springframework.util.ObjectUtils</c>.
    ''' <para>Differences from the Java original: .NET has no checked exceptions, so
    ''' <see cref="isCheckedException"/> is always False and <see cref="isCompatibleWithThrowsClause"/>
    ''' always True; the Java <c>byte[]</c> overloads take <c>Byte()</c> (unsigned, as everywhere in this
    ''' port); the identity hash is <see cref="RuntimeHelpers.GetHashCode"/>; scalars and array elements
    ''' are rendered Java-style (<c>true</c>, <c>1.0</c>, <c>null</c>); the hash codes use the Java
    ''' algorithms (<c>Boolean.hashCode</c>, <c>Double.hashCode</c>, ...) with wrap-around arithmetic.</para>
    ''' </summary>
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

        ''' <summary>
        ''' Java distinguishes checked exceptions (neither <c>RuntimeException</c> nor <c>Error</c>)
        ''' from unchecked ones. .NET has no checked exceptions, so this always returns False.
        ''' </summary>
        Public Shared Function isCheckedException(ByVal ex As Exception) As Boolean
            Return False
        End Function

        ''' <summary>
        ''' Always True: the Java code returned true for every unchecked exception before looking at
        ''' the declared exception types, and in .NET every exception is unchecked.
        ''' </summary>
        Public Shared Function isCompatibleWithThrowsClause(ByVal ex As Exception, ByVal ParamArray declaredExceptions() As Type) As Boolean
            Return True
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
            End If
            For Each arrayEle As Object In array
                If nullSafeEquals(arrayEle, element) Then
                    Return True
                End If
            Next arrayEle
            Return False
        End Function

        ''' <summary>
        ''' Java: <c>containsConstant(Enum&lt;?&gt;[], String)</c>. Works on any array whose elements
        ''' name themselves through <c>ToString</c>, e.g. <c>[Enum].GetValues(GetType(Color))</c>.
        ''' </summary>
        Public Shared Function containsConstant(Of E)(ByVal enumValues() As E, ByVal constant As String) As Boolean
            Return containsConstant(enumValues, constant, False)
        End Function

        Public Shared Function containsConstant(Of E)(ByVal enumValues() As E, ByVal constant As String, ByVal caseSensitive As Boolean) As Boolean
            For Each candidate As E In enumValues
                Dim name As String = candidate.ToString()
                If caseSensitive Then
                    If name.Equals(constant) Then
                        Return True
                    End If
                ElseIf name.Equals(constant, StringComparison.OrdinalIgnoreCase) Then
                    Return True
                End If
            Next candidate
            Return False
        End Function

        Public Shared Function caseInsensitiveValueOf(Of E)(ByVal enumValues() As E, ByVal constant As String) As E
            For Each candidate As E In enumValues
                If candidate.ToString().Equals(constant, StringComparison.OrdinalIgnoreCase) Then
                    Return candidate
                End If
            Next candidate
            Throw New ArgumentException(String.Format("constant [{0}] does not exist in enum opType {1}", constant, enumValues.GetType().GetElementType().FullName))
        End Function

        ''' <summary>
        ''' Appends <paramref name="obj"/> to a copy of <paramref name="array"/>. The element type of the
        ''' result is the array's, or the object's runtime type when the array is Nothing.
        ''' </summary>
        Public Shared Function addObjectToArray(Of A, O As A)(ByVal array() As A, ByVal obj As O) As A()
            Dim compType As Type = GetType(Object)
            If array IsNot Nothing Then
                compType = array.GetType().GetElementType()
            ElseIf obj IsNot Nothing Then
                compType = obj.GetType()
            End If
            Dim newArrLength As Integer = If(array IsNot Nothing, array.Length + 1, 1)
            Dim newArr As System.Array = System.Array.CreateInstance(compType, newArrLength)
            If array IsNot Nothing Then
                System.Array.Copy(array, 0, newArr, 0, array.Length)
            End If
            newArr.SetValue(obj, newArr.Length - 1)
            Return DirectCast(DirectCast(newArr, Object), A())
        End Function

        ''' <summary>
        ''' Returns the array as <c>Object()</c>: reference-type arrays are returned as they are, a
        ''' value-type array is boxed element by element (Java converted it to the wrapper type),
        ''' Nothing gives an empty array and anything else throws <see cref="ArgumentException"/>.
        ''' </summary>
        Public Shared Function toObjectArray(ByVal source As Object) As Object()
            If TypeOf source Is Object() Then
                Return DirectCast(source, Object())
            End If
            If source Is Nothing Then
                Return New Object() {}
            End If
            Dim arr As System.Array = TryCast(source, System.Array)
            If arr Is Nothing Then
                Throw New ArgumentException("Source is not an array: " & source.ToString())
            End If
            Dim length As Integer = arr.Length
            If length = 0 Then
                Return New Object() {}
            End If
            Dim newArray(length - 1) As Object
            For i As Integer = 0 To length - 1
                newArray(i) = arr.GetValue(i)
            Next i
            Return newArray
        End Function

        ''' <summary>Equality that also compares arrays element by element (not deep), like Java's <c>Arrays.equals</c>.</summary>
        Public Shared Function nullSafeEquals(ByVal o1 As Object, ByVal o2 As Object) As Boolean
            If o1 Is o2 Then
                Return True
            End If
            If o1 Is Nothing OrElse o2 Is Nothing Then
                Return False
            End If
            If o1.Equals(o2) Then
                Return True
            End If
            If o1.GetType().IsArray AndAlso o2.GetType().IsArray Then
                If TypeOf o1 Is Object() AndAlso TypeOf o2 Is Object() Then
                    Return Arrays.equals(DirectCast(o1, Object()), DirectCast(o2, Object()))
                End If
                ' value-type arrays: Java compares them only when both have the same component type
                If o1.GetType() Is o2.GetType() Then
                    Dim a1 As System.Array = DirectCast(o1, System.Array)
                    Dim a2 As System.Array = DirectCast(o2, System.Array)
                    If a1.Rank <> 1 OrElse a2.Rank <> 1 OrElse a1.Length <> a2.Length Then
                        Return False
                    End If
                    For i As Integer = 0 To a1.Length - 1
                        If Not Object.Equals(a1.GetValue(i), a2.GetValue(i)) Then
                            Return False
                        End If
                    Next i
                    Return True
                End If
            End If
            Return False
        End Function

        ''' <summary>Hash code of an object or array (Spring's seed-7 / multiplier-31 algorithm for arrays); 0 for Nothing.</summary>
        Public Shared Function nullSafeHashCode(ByVal obj As Object) As Integer
            If obj Is Nothing Then
                Return 0
            End If
            If obj.GetType().IsArray Then
                If TypeOf obj Is Object() Then
                    Return nullSafeHashCode(DirectCast(obj, Object()))
                End If
                If TypeOf obj Is Boolean() Then
                    Return nullSafeHashCode(DirectCast(obj, Boolean()))
                End If
                If TypeOf obj Is Byte() Then
                    Return nullSafeHashCode(DirectCast(obj, Byte()))
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
            Return Objects.hashCode(obj)
        End Function

        ''' <summary><c>MULTIPLIER * hash + elementHash</c> with Java's wrap-around int arithmetic.</summary>
        Private Shared Function combine(ByVal hash As Integer, ByVal elementHash As Integer) As Integer
            Return Arrays.WrapToInt32(MULTIPLIER * CLng(hash) + elementHash)
        End Function

        Public Shared Function nullSafeHashCode(ByVal array() As Object) As Integer
            If array Is Nothing Then
                Return 0
            End If
            Dim hash As Integer = INITIAL_HASH
            For Each element As Object In array
                hash = combine(hash, nullSafeHashCode(element))
            Next element
            Return hash
        End Function

        Public Shared Function nullSafeHashCode(ByVal array() As Boolean) As Integer
            If array Is Nothing Then
                Return 0
            End If
            Dim hash As Integer = INITIAL_HASH
            For Each element As Boolean In array
                hash = combine(hash, hashCode(element))
            Next element
            Return hash
        End Function

        ''' <summary>Java: <c>nullSafeHashCode(byte[])</c>; note that Java bytes are signed and <c>Byte</c> is not.</summary>
        Public Shared Function nullSafeHashCode(ByVal array() As Byte) As Integer
            If array Is Nothing Then
                Return 0
            End If
            Dim hash As Integer = INITIAL_HASH
            For Each element As Byte In array
                hash = combine(hash, CInt(element))
            Next element
            Return hash
        End Function

        Public Shared Function nullSafeHashCode(ByVal array() As Char) As Integer
            If array Is Nothing Then
                Return 0
            End If
            Dim hash As Integer = INITIAL_HASH
            For Each element As Char In array
                hash = combine(hash, AscW(element))
            Next element
            Return hash
        End Function

        Public Shared Function nullSafeHashCode(ByVal array() As Double) As Integer
            If array Is Nothing Then
                Return 0
            End If
            Dim hash As Integer = INITIAL_HASH
            For Each element As Double In array
                hash = combine(hash, hashCode(element))
            Next element
            Return hash
        End Function

        Public Shared Function nullSafeHashCode(ByVal array() As Single) As Integer
            If array Is Nothing Then
                Return 0
            End If
            Dim hash As Integer = INITIAL_HASH
            For Each element As Single In array
                hash = combine(hash, hashCode(element))
            Next element
            Return hash
        End Function

        Public Shared Function nullSafeHashCode(ByVal array() As Integer) As Integer
            If array Is Nothing Then
                Return 0
            End If
            Dim hash As Integer = INITIAL_HASH
            For Each element As Integer In array
                hash = combine(hash, element)
            Next element
            Return hash
        End Function

        Public Shared Function nullSafeHashCode(ByVal array() As Long) As Integer
            If array Is Nothing Then
                Return 0
            End If
            Dim hash As Integer = INITIAL_HASH
            For Each element As Long In array
                hash = combine(hash, hashCode(element))
            Next element
            Return hash
        End Function

        Public Shared Function nullSafeHashCode(ByVal array() As Short) As Integer
            If array Is Nothing Then
                Return 0
            End If
            Dim hash As Integer = INITIAL_HASH
            For Each element As Short In array
                hash = combine(hash, CInt(element))
            Next element
            Return hash
        End Function

        ''' <summary>Java <c>Boolean.hashCode</c>: 1231 for true, 1237 for false.</summary>
        Public Shared Function hashCode(ByVal bool As Boolean) As Integer
            Return If(bool, 1231, 1237)
        End Function

        ''' <summary>Java <c>Double.hashCode</c> (NaN hashes as the canonical NaN bit pattern).</summary>
        Public Shared Function hashCode(ByVal dbl As Double) As Integer
            Dim bits As Long = If(Double.IsNaN(dbl), &H7FF8000000000000L, BitConverter.DoubleToInt64Bits(dbl))
            Return hashCode(bits)
        End Function

        ''' <summary>Java <c>Float.floatToIntBits</c> (NaN hashes as the canonical NaN bit pattern).</summary>
        Public Shared Function hashCode(ByVal flt As Single) As Integer
            If Single.IsNaN(flt) Then
                Return &H7FC00000
            End If
            Return BitConverter.ToInt32(BitConverter.GetBytes(flt), 0)
        End Function

        ''' <summary>Java <c>Long.hashCode</c>: <c>(int)(value ^ (value &gt;&gt;&gt; 32))</c>.</summary>
        Public Shared Function hashCode(ByVal lng As Long) As Integer
            Return Arrays.WrapToInt32(lng Xor (lng >> 32))
        End Function

        ''' <summary>
        ''' Java: <c>getClass().getName() + "@" + Integer.toHexString(System.identityHashCode(obj))</c>,
        ''' using the type's full name and <see cref="RuntimeHelpers.GetHashCode"/> as the identity hash.
        ''' </summary>
        Public Shared Function identityToString(ByVal obj As Object) As String
            If obj Is Nothing Then
                Return EMPTY_STRING
            End If
            Return obj.GetType().FullName & "@" & getIdentityHexString(obj)
        End Function

        Public Shared Function getIdentityHexString(ByVal obj As Object) As String
            Return (CLng(RuntimeHelpers.GetHashCode(obj)) And &HFFFFFFFFL).ToString("x")
        End Function

        Public Shared Function getDisplayString(ByVal obj As Object) As String
            Return If(obj Is Nothing, EMPTY_STRING, nullSafeToString(obj))
        End Function

        ''' <summary>The full type name of the object, or <c>"null"</c>.</summary>
        Public Shared Function nullSafeClassName(ByVal obj As Object) As String
            Return If(obj IsNot Nothing, obj.GetType().FullName, NULL_STRING)
        End Function

        ''' <summary>
        ''' Renders the object: <c>"null"</c> for Nothing, strings as they are, arrays as
        ''' <c>{a, b}</c> (characters quoted) and everything else like Java's <c>String.valueOf</c>.
        ''' </summary>
        Public Shared Function nullSafeToString(ByVal obj As Object) As String
            If obj Is Nothing Then
                Return NULL_STRING
            End If
            If TypeOf obj Is String Then
                Return DirectCast(obj, String)
            End If
            If TypeOf obj Is Object() Then
                Return nullSafeToString(DirectCast(obj, Object()))
            End If
            If TypeOf obj Is Boolean() Then
                Return nullSafeToString(DirectCast(obj, Boolean()))
            End If
            If TypeOf obj Is Byte() Then
                Return nullSafeToString(DirectCast(obj, Byte()))
            End If
            If TypeOf obj Is Char() Then
                Return nullSafeToString(DirectCast(obj, Char()))
            End If
            If TypeOf obj Is Double() Then
                Return nullSafeToString(DirectCast(obj, Double()))
            End If
            If TypeOf obj Is Single() Then
                Return nullSafeToString(DirectCast(obj, Single()))
            End If
            If TypeOf obj Is Integer() Then
                Return nullSafeToString(DirectCast(obj, Integer()))
            End If
            If TypeOf obj Is Long() Then
                Return nullSafeToString(DirectCast(obj, Long()))
            End If
            If TypeOf obj Is Short() Then
                Return nullSafeToString(DirectCast(obj, Short()))
            End If
            If TypeOf obj Is System.Array Then
                ' arrays without a Java counterpart (UInteger(), Decimal(), ...) are rendered the same way
                Return arrayToString(DirectCast(obj, System.Array), False)
            End If
            Dim str As String = Arrays.FormatElement(obj)
            Return If(str IsNot Nothing, str, EMPTY_STRING)
        End Function

        Private Shared Function arrayToString(ByVal array As System.Array, ByVal quoteChars As Boolean) As String
            If array Is Nothing Then
                Return NULL_STRING
            End If
            If array.Length = 0 Then
                Return EMPTY_ARRAY
            End If
            Dim sb As New StringBuilder()
            Dim first As Boolean = True
            For Each element As Object In array
                sb.Append(If(first, ARRAY_START, ARRAY_ELEMENT_SEPARATOR))
                first = False
                If quoteChars Then
                    sb.Append("'"c).Append(Arrays.FormatElement(element)).Append("'"c)
                Else
                    sb.Append(Arrays.FormatElement(element))
                End If
            Next element
            sb.Append(ARRAY_END)
            Return sb.ToString()
        End Function

        Public Shared Function nullSafeToString(ByVal array() As Object) As String
            Return arrayToString(array, False)
        End Function

        Public Shared Function nullSafeToString(ByVal array() As Boolean) As String
            Return arrayToString(array, False)
        End Function

        ''' <summary>Java: <c>nullSafeToString(byte[])</c>; <c>Byte</c> values print unsigned.</summary>
        Public Shared Function nullSafeToString(ByVal array() As Byte) As String
            Return arrayToString(array, False)
        End Function

        Public Shared Function nullSafeToString(ByVal array() As Char) As String
            Return arrayToString(array, True)
        End Function

        Public Shared Function nullSafeToString(ByVal array() As Double) As String
            Return arrayToString(array, False)
        End Function

        Public Shared Function nullSafeToString(ByVal array() As Single) As String
            Return arrayToString(array, False)
        End Function

        Public Shared Function nullSafeToString(ByVal array() As Integer) As String
            Return arrayToString(array, False)
        End Function

        Public Shared Function nullSafeToString(ByVal array() As Long) As String
            Return arrayToString(array, False)
        End Function

        Public Shared Function nullSafeToString(ByVal array() As Short) As String
            Return arrayToString(array, False)
        End Function
    End Class

End Namespace
