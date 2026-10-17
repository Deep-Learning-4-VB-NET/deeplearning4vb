Imports System
Imports System.Threading

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

    ''' <summary>
    ''' A lock-free boolean with atomic get/set/compare-and-set, equivalent to
    ''' <c>java.util.concurrent.atomic.AtomicBoolean</c> (which the Java class extended).
    ''' </summary>
    <Serializable>
    Public Class AtomicBoolean
        Private state As Integer

        Public Sub New(ByVal initialValue As Boolean)
            state = If(initialValue, 1, 0)
        End Sub

        Public Sub New()
            Me.New(False)
        End Sub

        Public Function [get]() As Boolean
            Return Volatile.Read(state) <> 0
        End Function

        Public Sub [set](ByVal newValue As Boolean)
            Volatile.Write(state, If(newValue, 1, 0))
        End Sub

        Public Sub lazySet(ByVal newValue As Boolean)
            [set](newValue)
        End Sub

        Public Property Value As Boolean
            Get
                Return [get]()
            End Get
            Set(ByVal newValue As Boolean)
                [set](newValue)
            End Set
        End Property

        ''' <summary>Atomically sets the value and returns the previous value.</summary>
        Public Function getAndSet(ByVal newValue As Boolean) As Boolean
            Return Interlocked.Exchange(state, If(newValue, 1, 0)) <> 0
        End Function

        ''' <summary>Atomically sets the value to <paramref name="update"/> if the current value equals <paramref name="expect"/>.</summary>
        Public Function compareAndSet(ByVal expect As Boolean, ByVal update As Boolean) As Boolean
            Dim expected As Integer = If(expect, 1, 0)
            Return Interlocked.CompareExchange(state, If(update, 1, 0), expected) = expected
        End Function

        Public Function weakCompareAndSet(ByVal expect As Boolean, ByVal update As Boolean) As Boolean
            Return compareAndSet(expect, update)
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If TypeOf o Is AtomicBoolean Then
                Return [get]() = DirectCast(o, AtomicBoolean).[get]()
            ElseIf TypeOf o Is Boolean Then
                Return [get]() = CBool(o)
            End If
            Return False
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return If([get](), 1, 0)
        End Function

        Public Overrides Function ToString() As String
            Return If([get](), "true", "false")
        End Function

    End Class

End Namespace
