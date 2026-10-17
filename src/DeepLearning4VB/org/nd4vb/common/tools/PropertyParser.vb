Imports System
Imports System.Globalization
Imports DeepLearning4VB.JavaCompat

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

Namespace org.nd4vb.common.tools

    ''' <summary>
    ''' Typed accessors over a <see cref="Properties"/> set. The <c>parseX</c> methods throw when a
    ''' property is missing or malformed; the <c>toX</c> methods fall back to a default value.
    ''' Parsing is culture-invariant, as in Java.
    ''' </summary>
    Public Class PropertyParser

        Private propertiesValue As Properties

        Public Sub New(ByVal properties As Properties)
            propertiesValue = properties
        End Sub

        Public Overridable Property Properties As Properties
            Get
                Return propertiesValue
            End Get
            Set(ByVal value As Properties)
                propertiesValue = value
            End Set
        End Property

        Public Function getProperties() As Properties
            Return propertiesValue
        End Function

        Public Sub setProperties(ByVal properties As Properties)
            propertiesValue = properties
        End Sub

        Private Function Raw(ByVal name As String) As String
            Return propertiesValue.getProperty(name)
        End Function

        Public Overridable Function parseString(ByVal name As String) As String
            Dim [property] As String = Raw(name)
            If [property] Is Nothing Then
                Throw New NullReferenceException("Property '" & name & "' is not set")
            End If
            Return [property]
        End Function

        ''' <summary>Java <c>Integer.parseInt</c> semantics: a missing or malformed value throws <see cref="NumberFormatException"/>.</summary>
        Public Overridable Function parseInt(ByVal name As String) As Integer
            Dim raw As String = Me.Raw(name)
            Try
                Return Integer.Parse(raw, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture)
            Catch e As Exception When TypeOf e Is FormatException OrElse TypeOf e Is OverflowException OrElse TypeOf e Is ArgumentNullException
                Throw NumberFormatException.ForInputString(raw, e)
            End Try
        End Function

        ''' <summary>Java <c>Boolean.parseBoolean</c> semantics: true only for "true" (ignoring case).</summary>
        Public Overridable Function parseBoolean(ByVal name As String) As Boolean
            Dim [property] As String = Raw(name)
            If [property] Is Nothing Then
                Throw New ArgumentException("Property '" & name & "' is not set")
            End If
            Return String.Equals([property].Trim(), "true", StringComparison.OrdinalIgnoreCase)
        End Function

        ''' <summary>Java <c>Float.parseFloat</c> semantics: a missing value throws <see cref="NullReferenceException"/>, a malformed one <see cref="NumberFormatException"/>.</summary>
        Public Overridable Function parseFloat(ByVal name As String) As Single
            Dim raw As String = parseString(name)
            Try
                Return Single.Parse(raw, NumberStyles.Float, CultureInfo.InvariantCulture)
            Catch e As Exception When TypeOf e Is FormatException OrElse TypeOf e Is OverflowException
                Throw NumberFormatException.ForInputString(raw, e)
            End Try
        End Function

        ''' <summary>Java <c>Double.parseDouble</c> semantics: a missing value throws <see cref="NullReferenceException"/>, a malformed one <see cref="NumberFormatException"/>.</summary>
        Public Overridable Function parseDouble(ByVal name As String) As Double
            Dim raw As String = parseString(name)
            Try
                Return Double.Parse(raw, NumberStyles.Float, CultureInfo.InvariantCulture)
            Catch e As Exception When TypeOf e Is FormatException OrElse TypeOf e Is OverflowException
                Throw NumberFormatException.ForInputString(raw, e)
            End Try
        End Function

        ''' <summary>Java <c>Long.parseLong</c> semantics: a missing or malformed value throws <see cref="NumberFormatException"/>.</summary>
        Public Overridable Function parseLong(ByVal name As String) As Long
            Dim raw As String = Me.Raw(name)
            Try
                Return Long.Parse(raw, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture)
            Catch e As Exception When TypeOf e Is FormatException OrElse TypeOf e Is OverflowException OrElse TypeOf e Is ArgumentNullException
                Throw NumberFormatException.ForInputString(raw, e)
            End Try
        End Function

        Public Overridable Function parseChar(ByVal name As String) As Char
            Dim [property] As String = parseString(name)
            If [property].Length <> 1 Then
                Throw New ArgumentException(name & " property isn't a single character")
            End If
            Return [property](0)
        End Function

        Public Overloads Overridable Function toString(ByVal name As String) As String
            Return toString(name, "")
        End Function

        Public Overridable Function toInt(ByVal name As String) As Integer
            Return toInt(name, 0)
        End Function

        Public Overridable Function toBoolean(ByVal name As String) As Boolean
            Return toBoolean(name, False)
        End Function

        Public Overridable Function toFloat(ByVal name As String) As Single
            Return toFloat(name, 0.0F)
        End Function

        Public Overridable Function toDouble(ByVal name As String) As Double
            Return toDouble(name, 0.0)
        End Function

        Public Overridable Function toLong(ByVal name As String) As Long
            Return toLong(name, 0)
        End Function

        Public Overridable Function toChar(ByVal name As String) As Char
            Return toChar(name, ChrW(0))
        End Function

        Public Overloads Overridable Function toString(ByVal name As String, ByVal defaultValue As String) As String
            Dim [property] As String = Raw(name)
            Return If([property] IsNot Nothing, [property], defaultValue)
        End Function

        Public Overridable Function toInt(ByVal name As String, ByVal defaultValue As Integer) As Integer
            Try
                Return parseInt(name)
            Catch e As Exception
                Return defaultValue
            End Try
        End Function

        Public Overridable Function toBoolean(ByVal name As String, ByVal defaultValue As Boolean) As Boolean
            Dim [property] As String = Raw(name)
            Return If([property] IsNot Nothing, String.Equals([property].Trim(), "true", StringComparison.OrdinalIgnoreCase), defaultValue)
        End Function

        Public Overridable Function toFloat(ByVal name As String, ByVal defaultValue As Single) As Single
            Try
                Return parseFloat(name)
            Catch e As Exception
                Return defaultValue
            End Try
        End Function

        Public Overridable Function toDouble(ByVal name As String, ByVal defaultValue As Double) As Double
            Try
                Return parseDouble(name)
            Catch e As Exception
                Return defaultValue
            End Try
        End Function

        Public Overridable Function toLong(ByVal name As String, ByVal defaultValue As Long) As Long
            Try
                Return parseLong(name)
            Catch e As Exception
                Return defaultValue
            End Try
        End Function

        Public Overridable Function toChar(ByVal name As String, ByVal defaultValue As Char) As Char
            Try
                Return parseChar(name)
            Catch e As Exception
                Return defaultValue
            End Try
        End Function
    End Class

End Namespace
