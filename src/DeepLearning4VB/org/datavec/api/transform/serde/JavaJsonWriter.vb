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

Imports System
Imports System.Globalization
Imports System.Text
Imports System.Text.Json
Imports System.Text.Json.Nodes

Namespace org.datavec.api.transform.serde

    ''' <summary>
    ''' Writes a JSON tree the way Jackson's <c>DefaultPrettyPrinter</c> lays it out (the Java DataVec
    ''' <c>INDENT_OUTPUT</c> setting): <c>"name" : value</c>, two spaces per object level, arrays on one line as
    ''' <c>[ a, b ]</c>, empty containers as <c>{ }</c> and <c>[ ]</c>, the platform line separator. Floating point fields
    ''' are written with Java's <c>Double.toString</c>/<c>Float.toString</c> format (<c>1.0</c>, <c>0.001</c>,
    ''' <c>1.0E7</c>, <c>1.5E-4</c>); the tree is walked together with the declared types to know which numbers are
    ''' floating point. Strings are escaped as Jackson escapes them.
    ''' </summary>
    Friend NotInheritable Class JavaJsonWriter

        Private ReadOnly sb As New StringBuilder()
        Private ReadOnly options As JsonSerializerOptions
        Private Shared ReadOnly nl As String = Environment.NewLine

        Private Sub New(ByVal options As JsonSerializerOptions)
            Me.options = options
        End Sub

        Friend Shared Function write(ByVal node As JsonNode, ByVal declaredType As Type, ByVal options As JsonSerializerOptions) As String
            Dim w As New JavaJsonWriter(options)
            w.writeValue(node, declaredType, 0)
            Return w.sb.ToString()
        End Function

        Private Sub writeValue(ByVal node As JsonNode, ByVal declared As Type, ByVal depth As Integer)
            If node Is Nothing Then
                sb.Append("null")
                Return
            End If
            Select Case node.GetValueKind()
                Case JsonValueKind.Object
                    writeObject(DirectCast(node, JsonObject), declared, depth)
                Case JsonValueKind.Array
                    writeArray(DirectCast(node, JsonArray), declared, depth)
                Case JsonValueKind.String
                    writeString(node.GetValue(Of String)())
                Case JsonValueKind.Number
                    writeNumber(node, declared)
                Case JsonValueKind.True
                    sb.Append("true")
                Case JsonValueKind.False
                    sb.Append("false")
                Case Else
                    sb.Append("null")
            End Select
        End Sub

        Private Sub writeObject(ByVal obj As JsonObject, ByVal declared As Type, ByVal depth As Integer)
            If obj.Count = 0 Then
                sb.Append("{ }")
                Return
            End If
            Dim actual As Type = JsonMappers.runtimeTypeOf(obj, If(declared, GetType(Object)), options)
            Dim catalog As TypeCatalog = JsonMappers.catalogOf(options)
            Dim wrapped As Boolean = JsonMappers.isWrapperRoot(actual) AndAlso obj.Count = 1
            sb.Append("{"c)
            Dim first As Boolean = True
            For Each kv In obj
                If Not first Then sb.Append(","c)
                first = False
                sb.Append(nl).Append(" "c, 2 * (depth + 1))
                writeString(kv.Key)
                sb.Append(" : ")
                Dim childType As Type
                If wrapped Then
                    childType = If(catalog.wrapperType(actual, kv.Key), GetType(Object))
                Else
                    childType = JsonMappers.propertyTypeOf(actual, kv.Key, options)
                End If
                writeValue(kv.Value, childType, depth + 1)
            Next
            sb.Append(nl).Append(" "c, 2 * depth).Append("}"c)
        End Sub

        Private Sub writeArray(ByVal arr As JsonArray, ByVal declared As Type, ByVal depth As Integer)
            If arr.Count = 0 Then
                sb.Append("[ ]")
                Return
            End If
            Dim elementType As Type = JsonMappers.elementTypeOf(declared)
            sb.Append("[ ")
            For i As Integer = 0 To arr.Count - 1
                If i > 0 Then sb.Append(", ")
                writeValue(arr(i), elementType, depth)
            Next
            sb.Append(" ]")
        End Sub

        Private Sub writeNumber(ByVal node As JsonNode, ByVal declared As Type)
            Dim t As Type = If(declared Is Nothing, Nothing, If(Nullable.GetUnderlyingType(declared), declared))
            If t Is GetType(Double) Then
                sb.Append(javaDoubleToString(node.GetValue(Of Double)()))
            ElseIf t Is GetType(Single) Then
                sb.Append(javaFloatToString(node.GetValue(Of Single)()))
            Else
                sb.Append(node.ToJsonString())
            End If
        End Sub

        Private Sub writeString(ByVal s As String)
            sb.Append(""""c)
            For Each c As Char In s
                Select Case c
                    Case """"c
                        sb.Append("\""")
                    Case "\"c
                        sb.Append("\\")
                    Case ChrW(8)
                        sb.Append("\b")
                    Case ChrW(9)
                        sb.Append("\t")
                    Case ChrW(10)
                        sb.Append("\n")
                    Case ChrW(12)
                        sb.Append("\f")
                    Case ChrW(13)
                        sb.Append("\r")
                    Case Else
                        If AscW(c) < 32 Then
                            sb.Append("\u").Append(AscW(c).ToString("X4", CultureInfo.InvariantCulture))
                        Else
                            sb.Append(c)
                        End If
                End Select
            Next
            sb.Append(""""c)
        End Sub

        ''' <summary>Java <c>Double.toString</c> (shortest round-trip digits).</summary>
        Friend Shared Function javaDoubleToString(ByVal d As Double) As String
            If Double.IsNaN(d) Then Return "NaN"
            If Double.IsPositiveInfinity(d) Then Return "Infinity"
            If Double.IsNegativeInfinity(d) Then Return "-Infinity"
            If d = 0 Then Return If(BitConverter.DoubleToInt64Bits(d) < 0, "-0.0", "0.0")
            Return javaFormat(d.ToString("R", CultureInfo.InvariantCulture))
        End Function

        ''' <summary>Java <c>Float.toString</c> (shortest round-trip digits).</summary>
        Friend Shared Function javaFloatToString(ByVal f As Single) As String
            If Single.IsNaN(f) Then Return "NaN"
            If Single.IsPositiveInfinity(f) Then Return "Infinity"
            If Single.IsNegativeInfinity(f) Then Return "-Infinity"
            If f = 0 Then Return If(BitConverter.DoubleToInt64Bits(CDbl(f)) < 0, "-0.0", "0.0")
            Return javaFormat(f.ToString("R", CultureInfo.InvariantCulture))
        End Function

        ''' <summary>Re-formats a .NET round-trip number string with Java's rules: plain notation with at least one
        ''' fraction digit for 10^-3 &lt;= |x| &lt; 10^7, else <c>d.dddE[-]n</c>.</summary>
        Private Shared Function javaFormat(ByVal r As String) As String
            Dim negative As Boolean = r.StartsWith("-", StringComparison.Ordinal)
            If negative Then r = r.Substring(1)
            Dim exponent As Integer = 0
            Dim ePos As Integer = r.IndexOfAny(New Char() {"E"c, "e"c})
            Dim mantissa As String = r
            If ePos >= 0 Then
                exponent = Integer.Parse(r.Substring(ePos + 1), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture)
                mantissa = r.Substring(0, ePos)
            End If
            Dim dot As Integer = mantissa.IndexOf("."c)
            Dim intPart As String = If(dot >= 0, mantissa.Substring(0, dot), mantissa)
            Dim fracPart As String = If(dot >= 0, mantissa.Substring(dot + 1), "")
            Dim all As String = intPart & fracPart
            Dim trimmed As String = all.TrimStart("0"c)
            Dim leadingZeros As Integer = all.Length - trimmed.Length
            Dim digits As String = trimmed.TrimEnd("0"c)
            If digits.Length = 0 Then Return If(negative, "-0.0", "0.0")
            ' value = 0.<digits> x 10^pointPos
            Dim pointPos As Integer = intPart.Length + exponent - leadingZeros
            Dim sciExp As Integer = pointPos - 1
            Dim result As String
            If sciExp >= -3 AndAlso sciExp < 7 Then
                If pointPos <= 0 Then
                    result = "0." & New String("0"c, -pointPos) & digits
                ElseIf pointPos >= digits.Length Then
                    result = digits & New String("0"c, pointPos - digits.Length) & ".0"
                Else
                    result = digits.Substring(0, pointPos) & "." & digits.Substring(pointPos)
                End If
            Else
                result = digits.Substring(0, 1) & "." & If(digits.Length > 1, digits.Substring(1), "0") & "E" & sciExp.ToString(CultureInfo.InvariantCulture)
            End If
            Return If(negative, "-" & result, result)
        End Function

    End Class

End Namespace
