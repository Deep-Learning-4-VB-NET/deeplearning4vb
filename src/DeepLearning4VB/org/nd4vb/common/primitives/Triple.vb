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

Namespace org.nd4vb.common.primitives

    ''' <summary>A simple mutable triple of values.</summary>
    <Serializable>
    Public Class Triple(Of F, S, T)
        Private Const serialVersionUID As Long = 119L

        Protected Friend firstValue As F
        Protected Friend secondValue As S
        Protected Friend thirdValue As T

        Public Sub New()
        End Sub

        Public Sub New(ByVal first As F, ByVal second As S, ByVal third As T)
            firstValue = first
            secondValue = second
            thirdValue = third
        End Sub

        Public Property First As F
            Get
                Return firstValue
            End Get
            Set(ByVal value As F)
                firstValue = value
            End Set
        End Property

        Public Property Second As S
            Get
                Return secondValue
            End Get
            Set(ByVal value As S)
                secondValue = value
            End Set
        End Property

        Public Property Third As T
            Get
                Return thirdValue
            End Get
            Set(ByVal value As T)
                thirdValue = value
            End Set
        End Property

        Public Overridable ReadOnly Property Left As F
            Get
                Return firstValue
            End Get
        End Property

        Public Overridable ReadOnly Property Middle As S
            Get
                Return secondValue
            End Get
        End Property

        Public Overridable ReadOnly Property Right As T
            Get
                Return thirdValue
            End Get
        End Property

        Public Function getFirst() As F
            Return firstValue
        End Function

        Public Function getSecond() As S
            Return secondValue
        End Function

        Public Function getThird() As T
            Return thirdValue
        End Function

        Public Function getLeft() As F
            Return firstValue
        End Function

        Public Function getMiddle() As S
            Return secondValue
        End Function

        Public Function getRight() As T
            Return thirdValue
        End Function

        Public Sub setFirst(ByVal first As F)
            firstValue = first
        End Sub

        Public Sub setSecond(ByVal second As S)
            secondValue = second
        End Sub

        Public Sub setThird(ByVal third As T)
            thirdValue = third
        End Sub

        Public Shared Function tripleOf(ByVal first As F, ByVal second As S, ByVal third As T) As Triple(Of F, S, T)
            Return New Triple(Of F, S, T)(first, second, third)
        End Function

        Public Shared Function [of](ByVal first As F, ByVal second As S, ByVal third As T) As Triple(Of F, S, T)
            Return New Triple(Of F, S, T)(first, second, third)
        End Function

        Public Overrides Function ToString() As String
            Return "Triple(first=" & Arrays.FormatElement(firstValue) & ", second=" & Arrays.FormatElement(secondValue) & ", third=" & Arrays.FormatElement(thirdValue) & ")"
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then Return True
            Dim other = TryCast(o, Triple(Of F, S, T))
            If other Is Nothing Then Return False
            Return Object.Equals(firstValue, other.firstValue) AndAlso Object.Equals(secondValue, other.secondValue) AndAlso Object.Equals(thirdValue, other.thirdValue)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(firstValue, secondValue, thirdValue)
        End Function
    End Class

    ''' <summary>Factory methods for <see cref="Triple(Of F, S, T)"/> with type inference (Java's static methods).</summary>
    Public NotInheritable Class Triple
        Private Sub New()
        End Sub

        Public Shared Function tripleOf(Of F, S, T)(ByVal first As F, ByVal second As S, ByVal third As T) As Triple(Of F, S, T)
            Return New Triple(Of F, S, T)(first, second, third)
        End Function

        Public Shared Function [of](Of F, S, T)(ByVal first As F, ByVal second As S, ByVal third As T) As Triple(Of F, S, T)
            Return New Triple(Of F, S, T)(first, second, third)
        End Function
    End Class

End Namespace
