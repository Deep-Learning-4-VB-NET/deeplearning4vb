Imports System
Imports System.Diagnostics
Imports System.Linq
Imports System.Reflection
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

Namespace org.nd4vb.linalg.profiler.data.primitives

    ''' <summary>
    ''' A call stack with the profiler's own frames cut off, outermost frame first (Java <c>StackDescriptor</c>).
    ''' Java's <c>StackTraceElement[]</c> is an array of <see cref="StackFrame"/>; an element's name is
    ''' <c>declaringType.method:line</c> (the line is 0 without debugging symbols), and two descriptors are equal when
    ''' their element names are equal (Java compared the elements, which compare class, method, file and line).
    ''' </summary>
    Public Class StackDescriptor

        Protected Friend stackTrace() As StackFrame
        Private ReadOnly names() As String

        Public Sub New(ByVal stack() As StackFrame)
            If stack Is Nothing Then Throw New ArgumentNullException(NameOf(stack))
            ' we cut off X first elements from stack, because they belong to profiler
            ' basically, we just want to make sure, no profiler-related code is mentioned in stack trace
            Dim start As Integer = 0
            Do While start < stack.Length
                If classNameOf(stack(start)).Contains("DefaultOpExecutioner") Then
                    Exit Do
                End If
                start += 1
            Loop

            ' in tests it's quite possible to have no DefaultOpExecutioner calls being used
            If start = stack.Length Then
                start = 0
                ' Java read stack[start + 1] without a bounds check
                Do While start + 1 < stack.Length
                    If Not classNameOf(stack(start + 1)).Contains("OpProfiler") AndAlso Not classNameOf(stack(start + 1)).Contains("StackAggregator") Then
                        Exit Do
                    End If
                    start += 1
                Loop
            Else
                Do While start < stack.Length
                    If Not classNameOf(stack(start)).Contains("DefaultOpExecutioner") Then
                        Exit Do
                    End If
                    start += 1
                Loop
            End If

            Do While start < stack.Length
                If Not classNameOf(stack(start)).Contains("OpProfiler") Then
                    Exit Do
                End If
                start += 1
            Loop

            Dim cut(stack.Length - start - 1) As StackFrame
            Array.Copy(stack, start, cut, 0, cut.Length)
            Array.Reverse(cut)
            Me.stackTrace = cut
            Me.names = cut.Select(Function(f) elementNameOf(f)).ToArray()
        End Sub

        ''' <summary>The frames, outermost first (Java <c>getStackTrace()</c>).</summary>
        Public Overridable Function getStackTrace() As StackFrame()
            Return stackTrace
        End Function

        Private Shared Function classNameOf(ByVal frame As StackFrame) As String
            Dim method As MethodBase = If(frame Is Nothing, Nothing, frame.GetMethod())
            If method Is Nothing OrElse method.DeclaringType Is Nothing Then
                Return ""
            End If
            Return If(method.DeclaringType.FullName, method.DeclaringType.Name)
        End Function

        Private Shared Function elementNameOf(ByVal frame As StackFrame) As String
            Dim method As MethodBase = If(frame Is Nothing, Nothing, frame.GetMethod())
            Dim methodName As String = If(method Is Nothing, "<unknown>", method.Name)
            Dim line As Integer = If(frame Is Nothing, 0, frame.GetFileLineNumber())
            Return classNameOf(frame) & "." & methodName & ":" & line.ToString(Globalization.CultureInfo.InvariantCulture)
        End Function

        Public Overridable ReadOnly Property EntryName As String
            Get
                Return getElementName(0)
            End Get
        End Property

        Public Overridable Function getElementName(ByVal idx As Integer) As String
            Return names(idx)
        End Function

        Public Overridable Function size() As Integer
            Return names.Length
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then
                Return True
            End If
            Dim that As StackDescriptor = TryCast(o, StackDescriptor)
            If that Is Nothing OrElse CObj(that).GetType() IsNot CObj(Me).GetType() Then
                Return False
            End If

            Return names.SequenceEqual(that.names)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            For Each n As String In names
                h = 31 * h + n.GetHashCode()
            Next n
            Return h
        End Function

        Public Overrides Function ToString() As String
            Dim builder As New StringBuilder()
            builder.Append("Stack trace: " & vbLf)

            For i As Integer = 0 To size() - 1
                builder.Append("         ").Append(i).Append(": ").Append(getElementName(i)).Append(vbLf)
            Next i

            Return builder.ToString()
        End Function

    End Class

End Namespace
