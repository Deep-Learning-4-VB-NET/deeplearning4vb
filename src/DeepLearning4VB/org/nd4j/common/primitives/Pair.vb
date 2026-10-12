Imports System
Imports Preconditions = DeepLearning4VB.org.nd4j.common.base.Preconditions

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
    ''' A simple mutable pair of values. The Java original relied on Lombok to generate the
    ''' constructors, accessors, equality and hash code; those members are written out here.
    ''' </summary>
    <Serializable>
    Public Class Pair(Of K, V)
        Private Const serialVersionUID As Long = 119L

        Protected Friend keyField As K
        Protected Friend valueField As V

        Public Sub New()
        End Sub

        Public Sub New(ByVal key As K, ByVal value As V)
            keyField = key
            valueField = value
        End Sub

        Public Property Key As K
            Get
                Return keyField
            End Get
            Set(ByVal newKey As K)
                keyField = newKey
            End Set
        End Property

        Public Property Value As V
            Get
                Return valueField
            End Get
            Set(ByVal newValue As V)
                valueField = newValue
            End Set
        End Property

        Public Overridable ReadOnly Property Left As K
            Get
                Return keyField
            End Get
        End Property

        Public Overridable ReadOnly Property Right As V
            Get
                Return valueField
            End Get
        End Property

        Public Overridable Property First As K
            Get
                Return keyField
            End Get
            Set(ByVal first As K)
                keyField = first
            End Set
        End Property

        Public Overridable Property Second As V
            Get
                Return valueField
            End Get
            Set(ByVal second As V)
                valueField = second
            End Set
        End Property

        ' Java-style accessors, kept so code ported from the Java API keeps working.
        Public Function getKey() As K
            Return keyField
        End Function

        Public Function getValue() As V
            Return valueField
        End Function

        Public Function getFirst() As K
            Return keyField
        End Function

        Public Function getSecond() As V
            Return valueField
        End Function

        Public Function getLeft() As K
            Return keyField
        End Function

        Public Function getRight() As V
            Return valueField
        End Function

        Public Sub setKey(ByVal key As K)
            keyField = key
        End Sub

        Public Sub setValue(ByVal value As V)
            valueField = value
        End Sub

        Public Sub setFirst(ByVal first As K)
            keyField = first
        End Sub

        Public Sub setSecond(ByVal second As V)
            valueField = second
        End Sub

        Public Overrides Function ToString() As String
            Return "Pair{key=" & Arrays.FormatElement(keyField) & ", value=" & Arrays.FormatElement(valueField) & "}"
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then Return True
            Dim other = TryCast(o, Pair(Of K, V))
            If other Is Nothing Then Return False
            Return Object.Equals(keyField, other.keyField) AndAlso Object.Equals(valueField, other.valueField)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = Arrays.WrapToInt32(59L * result + If(keyField Is Nothing, 43, keyField.GetHashCode()))
            result = Arrays.WrapToInt32(59L * result + If(valueField Is Nothing, 43, valueField.GetHashCode()))
            Return result
        End Function

        Public Shared Function [of](Of T, E)(ByVal key As T, ByVal value As E) As Pair(Of T, E)
            Return New Pair(Of T, E)(key, value)
        End Function

        Public Shared Function makePair(Of T, E)(ByVal key As T, ByVal value As E) As Pair(Of T, E)
            Return New Pair(Of T, E)(key, value)
        End Function

        Public Shared Function create(Of T, E)(ByVal key As T, ByVal value As E) As Pair(Of T, E)
            Return New Pair(Of T, E)(key, value)
        End Function

        Public Shared Function pairOf(Of T, E)(ByVal key As T, ByVal value As E) As Pair(Of T, E)
            Return New Pair(Of T, E)(key, value)
        End Function

        Public Shared Function fromArray(Of T)(ByVal arr() As T) As Pair(Of T, T)
            Preconditions.checkArgument(arr.Length = 2, "Can only create a pair from an array with two values, got %s", arr.Length)
            Return New Pair(Of T, T)(arr(0), arr(1))
        End Function
    End Class

    ''' <summary>
    ''' Factory methods for <see cref="Pair(Of K, V)"/> with type inference. In Java these are static
    ''' methods of the generic class; VB needs a non-generic home for them so that
    ''' <c>Pair.of(key, value)</c> compiles without explicit type arguments.
    ''' </summary>
    Public NotInheritable Class Pair
        Private Sub New()
        End Sub

        Public Shared Function [of](Of T, E)(ByVal key As T, ByVal value As E) As Pair(Of T, E)
            Return New Pair(Of T, E)(key, value)
        End Function

        Public Shared Function makePair(Of T, E)(ByVal key As T, ByVal value As E) As Pair(Of T, E)
            Return New Pair(Of T, E)(key, value)
        End Function

        Public Shared Function create(Of T, E)(ByVal key As T, ByVal value As E) As Pair(Of T, E)
            Return New Pair(Of T, E)(key, value)
        End Function

        Public Shared Function pairOf(Of T, E)(ByVal key As T, ByVal value As E) As Pair(Of T, E)
            Return New Pair(Of T, E)(key, value)
        End Function

        Public Shared Function fromArray(Of T)(ByVal arr() As T) As Pair(Of T, T)
            Preconditions.checkArgument(arr.Length = 2, "Can only create a pair from an array with two values, got %s", arr.Length)
            Return New Pair(Of T, T)(arr(0), arr(1))
        End Function
    End Class

End Namespace
