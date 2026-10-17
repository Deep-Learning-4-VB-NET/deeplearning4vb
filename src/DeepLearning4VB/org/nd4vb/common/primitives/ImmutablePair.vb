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

    ''' <summary>An immutable pair of values.</summary>
    <Serializable>
    Public Class ImmutablePair(Of K, V)
        Private Const serialVersionUID As Long = 119L

        Protected Friend ReadOnly keyValue As K
        Protected Friend ReadOnly valueValue As V

        Protected Friend Sub New()
        End Sub

        Public Sub New(ByVal key As K, ByVal value As V)
            keyValue = key
            valueValue = value
        End Sub

        Public ReadOnly Property Key As K
            Get
                Return keyValue
            End Get
        End Property

        Public ReadOnly Property Value As V
            Get
                Return valueValue
            End Get
        End Property

        Public Overridable ReadOnly Property Left As K
            Get
                Return keyValue
            End Get
        End Property

        Public Overridable ReadOnly Property Right As V
            Get
                Return valueValue
            End Get
        End Property

        Public Overridable ReadOnly Property First As K
            Get
                Return keyValue
            End Get
        End Property

        Public Overridable ReadOnly Property Second As V
            Get
                Return valueValue
            End Get
        End Property

        Public Function getKey() As K
            Return keyValue
        End Function

        Public Function getValue() As V
            Return valueValue
        End Function

        Public Function getFirst() As K
            Return keyValue
        End Function

        Public Function getSecond() As V
            Return valueValue
        End Function

        Public Function getLeft() As K
            Return keyValue
        End Function

        Public Function getRight() As V
            Return valueValue
        End Function

        Public Shared Function [of](ByVal key As K, ByVal value As V) As ImmutablePair(Of K, V)
            Return New ImmutablePair(Of K, V)(key, value)
        End Function

        Public Shared Function makePair(ByVal key As K, ByVal value As V) As ImmutablePair(Of K, V)
            Return New ImmutablePair(Of K, V)(key, value)
        End Function

        Public Shared Function create(ByVal key As K, ByVal value As V) As ImmutablePair(Of K, V)
            Return New ImmutablePair(Of K, V)(key, value)
        End Function

        Public Shared Function pairOf(ByVal key As K, ByVal value As V) As ImmutablePair(Of K, V)
            Return New ImmutablePair(Of K, V)(key, value)
        End Function

        Public Overrides Function ToString() As String
            Return "ImmutablePair(key=" & Arrays.FormatElement(keyValue) & ", value=" & Arrays.FormatElement(valueValue) & ")"
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then Return True
            Dim other = TryCast(o, ImmutablePair(Of K, V))
            If other Is Nothing Then Return False
            Return Object.Equals(keyValue, other.keyValue) AndAlso Object.Equals(valueValue, other.valueValue)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(keyValue, valueValue)
        End Function
    End Class

    ''' <summary>Factory methods for <see cref="ImmutablePair(Of K, V)"/> with type inference (Java's static methods).</summary>
    Public NotInheritable Class ImmutablePair
        Private Sub New()
        End Sub

        Public Shared Function [of](Of T, E)(ByVal key As T, ByVal value As E) As ImmutablePair(Of T, E)
            Return New ImmutablePair(Of T, E)(key, value)
        End Function

        Public Shared Function makePair(Of T, E)(ByVal key As T, ByVal value As E) As ImmutablePair(Of T, E)
            Return New ImmutablePair(Of T, E)(key, value)
        End Function

        Public Shared Function create(Of T, E)(ByVal key As T, ByVal value As E) As ImmutablePair(Of T, E)
            Return New ImmutablePair(Of T, E)(key, value)
        End Function

        Public Shared Function pairOf(Of T, E)(ByVal key As T, ByVal value As E) As ImmutablePair(Of T, E)
            Return New ImmutablePair(Of T, E)(key, value)
        End Function
    End Class

End Namespace
