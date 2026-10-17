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

    ''' <summary>A simple mutable quadruple of values.</summary>
    <Serializable>
    Public Class Quad(Of F, S, T, O)
        Private Const serialVersionUID As Long = 119L

        Protected Friend firstValue As F
        Protected Friend secondValue As S
        Protected Friend thirdValue As T
        Protected Friend fourthValue As O

        Public Sub New()
        End Sub

        Public Sub New(ByVal first As F, ByVal second As S, ByVal third As T, ByVal fourth As O)
            firstValue = first
            secondValue = second
            thirdValue = third
            fourthValue = fourth
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

        Public Property Fourth As O
            Get
                Return fourthValue
            End Get
            Set(ByVal value As O)
                fourthValue = value
            End Set
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

        Public Function getFourth() As O
            Return fourthValue
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

        Public Sub setFourth(ByVal fourth As O)
            fourthValue = fourth
        End Sub

        Public Shared Function quadOf(ByVal first As F, ByVal second As S, ByVal third As T, ByVal fourth As O) As Quad(Of F, S, T, O)
            Return New Quad(Of F, S, T, O)(first, second, third, fourth)
        End Function

        Public Shared Function [of](ByVal first As F, ByVal second As S, ByVal third As T, ByVal fourth As O) As Quad(Of F, S, T, O)
            Return New Quad(Of F, S, T, O)(first, second, third, fourth)
        End Function

        Public Overrides Function ToString() As String
            Return "Quad(first=" & Arrays.FormatElement(firstValue) & ", second=" & Arrays.FormatElement(secondValue) & ", third=" & Arrays.FormatElement(thirdValue) & ", fourth=" & Arrays.FormatElement(fourthValue) & ")"
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then Return True
            Dim other = TryCast(o, Quad(Of F, S, T, O))
            If other Is Nothing Then Return False
            Return Object.Equals(firstValue, other.firstValue) AndAlso Object.Equals(secondValue, other.secondValue) AndAlso Object.Equals(thirdValue, other.thirdValue) AndAlso Object.Equals(fourthValue, other.fourthValue)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(firstValue, secondValue, thirdValue, fourthValue)
        End Function
    End Class

    ''' <summary>Factory methods for <see cref="Quad(Of F, S, T, O)"/> with type inference (Java's static methods).</summary>
    Public NotInheritable Class Quad
        Private Sub New()
        End Sub

        Public Shared Function quadOf(Of F, S, T, O)(ByVal first As F, ByVal second As S, ByVal third As T, ByVal fourth As O) As Quad(Of F, S, T, O)
            Return New Quad(Of F, S, T, O)(first, second, third, fourth)
        End Function

        Public Shared Function [of](Of F, S, T, O)(ByVal first As F, ByVal second As S, ByVal third As T, ByVal fourth As O) As Quad(Of F, S, T, O)
            Return New Quad(Of F, S, T, O)(first, second, third, fourth)
        End Function
    End Class

End Namespace
