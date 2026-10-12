Imports System
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

Namespace org.nd4j.common.validation

    ''' <summary>The outcome of validating a file or model: whether it is valid, and if not, why.</summary>
    <Serializable>
    Public Class ValidationResult

        ''' <summary>Human readable format/model type.</summary>
        Public Property FormatType As String
        ''' <summary>Actual class the format/model is (or should be).</summary>
        Public Property FormatClass As Type
        ''' <summary>Path of the file (if applicable).</summary>
        Public Property Path As String
        ''' <summary>Whether the file/model is valid.</summary>
        Public Property Valid As Boolean
        ''' <summary>List of issues (generally only present if not valid).</summary>
        Public Property Issues As IList(Of String)
        ''' <summary>Exception, if applicable.</summary>
        Public Property Exception As Exception

        Public Sub New()
        End Sub

        Public Sub New(ByVal formatType As String, ByVal formatClass As Type, ByVal path As String, ByVal valid As Boolean, ByVal issues As IList(Of String), ByVal exception As Exception)
            Me.FormatType = formatType
            Me.FormatClass = formatClass
            Me.Path = path
            Me.Valid = valid
            Me.Issues = issues
            Me.Exception = exception
        End Sub

        Public Function getFormatType() As String
            Return FormatType
        End Function

        Public Function getFormatClass() As Type
            Return FormatClass
        End Function

        Public Function getPath() As String
            Return Path
        End Function

        Public Function isValid() As Boolean
            Return Valid
        End Function

        Public Function getIssues() As IList(Of String)
            Return Issues
        End Function

        Public Function getException() As Exception
            Return Exception
        End Function

        Public Shared Function builder() As ValidationResultBuilder
            Return New ValidationResultBuilder()
        End Function

        ''' <summary>Fluent builder (Lombok <c>@Builder</c> in the Java original).</summary>
        Public Class ValidationResultBuilder
            Private ReadOnly result As New ValidationResult()

            Public Function formatType(ByVal value As String) As ValidationResultBuilder
                result.FormatType = value
                Return Me
            End Function

            Public Function formatClass(ByVal value As Type) As ValidationResultBuilder
                result.FormatClass = value
                Return Me
            End Function

            Public Function path(ByVal value As String) As ValidationResultBuilder
                result.Path = value
                Return Me
            End Function

            Public Function valid(ByVal value As Boolean) As ValidationResultBuilder
                result.Valid = value
                Return Me
            End Function

            Public Function issues(ByVal value As IList(Of String)) As ValidationResultBuilder
                result.Issues = value
                Return Me
            End Function

            Public Function issues(ByVal ParamArray value() As String) As ValidationResultBuilder
                result.Issues = New List(Of String)(value)
                Return Me
            End Function

            Public Function exception(ByVal value As Exception) As ValidationResultBuilder
                result.Exception = value
                Return Me
            End Function

            Public Function build() As ValidationResult
                Return New ValidationResult(result.FormatType, result.FormatClass, result.Path, result.Valid, result.Issues, result.Exception)
            End Function
        End Class

        Public Overrides Function ToString() As String
            Dim lines As IList(Of String) = New List(Of String)()
            If FormatType IsNot Nothing Then
                lines.Add("Format type: " & FormatType)
            End If
            If FormatClass IsNot Nothing Then
                lines.Add("Format class: " & FormatClass.FullName)
            End If
            If Path IsNot Nothing Then
                lines.Add("Path: " & Path)
            End If
            lines.Add("Format valid: " & If(Valid, "true", "false"))
            If Issues IsNot Nothing AndAlso Issues.Count > 0 Then
                If Issues.Count = 1 Then
                    addWithIndent(Issues(0), lines, "Issue: ", "       ")
                Else
                    lines.Add("Issues:")
                    For Each s As String In Issues
                        addWithIndent(s, lines, "- ", "  ")
                    Next s
                End If
            End If
            If Exception IsNot Nothing Then
                lines.Add("Stack Trace:")
                addWithIndent(Exception.ToString(), lines, "  ", "  ")
            End If
            Return String.Join(ControlChars.Lf, lines)
        End Function

        Protected Friend Shared Sub addWithIndent(ByVal toAdd As String, ByVal list As IList(Of String), ByVal firstLineIndent As String, ByVal laterLineIndent As String)
            Dim split() As String = toAdd.Replace(ControlChars.CrLf, ControlChars.Lf).Split(ControlChars.Lf)
            Dim first As Boolean = True
            For Each issueLine As String In split
                list.Add(If(first, firstLineIndent, laterLineIndent) & issueLine)
                first = False
            Next issueLine
        End Sub

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            Dim other = TryCast(o, ValidationResult)
            If other Is Nothing Then Return False
            Return String.Equals(FormatType, other.FormatType) AndAlso FormatClass Is other.FormatClass AndAlso String.Equals(Path, other.Path) AndAlso Valid = other.Valid AndAlso Arrays.equals(If(Issues Is Nothing, Nothing, New List(Of String)(Issues).ToArray()), If(other.Issues Is Nothing, Nothing, New List(Of String)(other.Issues).ToArray())) AndAlso Object.Equals(Exception, other.Exception)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(FormatType, Path, Valid)
        End Function

    End Class

End Namespace
