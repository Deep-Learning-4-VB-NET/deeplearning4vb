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

Namespace org.nd4j.common.primitives

    ''' <summary>
    ''' A container that may or may not hold a non-null value (a minimal <c>java.util.Optional</c>).
    ''' </summary>
    Public Class [Optional](Of T)
        Private Shared ReadOnly EMPTY_INSTANCE As New [Optional](Of T)()

        Private ReadOnly value As T

        Private Sub New()
        End Sub

        Private Sub New(ByVal value As T)
            Me.value = value
        End Sub

        ''' <summary>Returns an empty Optional instance. No value is present for this Optional.</summary>
        Public Shared Function empty() As [Optional](Of T)
            Return EMPTY_INSTANCE
        End Function

        ''' <summary>Returns an Optional with the specified present non-null value.</summary>
        ''' <param name="value">The value to be present, which must be non-null.</param>
        Public Shared Function [of](ByVal value As T) As [Optional](Of T)
            If value Is Nothing Then
                Throw New NullReferenceException("value is marked non-null but is null")
            End If
            Return New [Optional](Of T)(value)
        End Function

        ''' <summary>Returns an Optional describing the specified value if non-null, otherwise an empty Optional.</summary>
        Public Shared Function ofNullable(ByVal value As T) As [Optional](Of T)
            If value Is Nothing Then
                Return empty()
            End If
            Return New [Optional](Of T)(value)
        End Function

        ''' <summary>If a value is present, returns the value, otherwise throws <see cref="InvalidOperationException"/>.</summary>
        Public Overridable Function [get]() As T
            If Not Present Then
                Throw New InvalidOperationException("Optional is empty")
            End If
            Return value
        End Function

        ''' <summary>True if there is a value present, otherwise false.</summary>
        Public Overridable ReadOnly Property Present As Boolean
            Get
                Return value IsNot Nothing
            End Get
        End Property

        Public Function isPresent() As Boolean
            Return Present
        End Function

        ''' <summary>Returns the value if present, otherwise <paramref name="other"/>.</summary>
        Public Overridable Function [orElse](ByVal other As T) As T
            If Present Then
                Return [get]()
            End If
            Return other
        End Function

        Public Overrides Function ToString() As String
            If Present Then
                Return "Optional(" & value.ToString() & ")"
            End If
            Return "Optional()"
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then
                Return True
            End If
            Dim other = TryCast(o, [Optional](Of T))
            If other Is Nothing Then
                Return False
            End If
            Return Object.Equals(value, other.value)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hashCode(value)
        End Function
    End Class

End Namespace
