Imports System
Imports System.Runtime.Serialization
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

Namespace org.nd4j.common.primitives

    ''' <summary>
    ''' A thread-safe holder for a single value, guarded by a reader/writer lock.
    ''' The Java original used a <c>ReentrantReadWriteLock</c>; this port uses
    ''' <see cref="ReaderWriterLockSlim"/>, which is recreated after deserialization.
    ''' </summary>
    ''' <typeparam name="T">Type of the stored value.</typeparam>
    <Serializable>
    Public Class Atomic(Of T)
        Private currentValue As T
        <NonSerialized>
        Private lock As ReaderWriterLockSlim

        Public Sub New()
            lock = New ReaderWriterLockSlim()
        End Sub

        Public Sub New(ByVal initialValue As T)
            Me.New()
            currentValue = initialValue
        End Sub

        ''' <summary>Assigns a new value under the write lock.</summary>
        Public Overridable Sub [set](ByVal newValue As T)
            lock.EnterWriteLock()
            Try
                currentValue = newValue
            Finally
                lock.ExitWriteLock()
            End Try
        End Sub

        ''' <summary>Returns the current value under the read lock.</summary>
        Public Overridable Function [get]() As T
            lock.EnterReadLock()
            Try
                Return currentValue
            Finally
                lock.ExitReadLock()
            End Try
        End Function

        Public Property Value As T
            Get
                Return [get]()
            End Get
            Set(ByVal newValue As T)
                [set](newValue)
            End Set
        End Property

        ''' <summary>
        ''' Compare-and-swap: replaces the value with <paramref name="newValue"/> only if the
        ''' current value equals <paramref name="expected"/>.
        ''' </summary>
        ''' <returns>True if the value was swapped, false otherwise.</returns>
        Public Overridable Function cas(ByVal expected As T, ByVal newValue As T) As Boolean
            lock.EnterWriteLock()
            Try
                If Object.Equals(currentValue, expected) Then
                    currentValue = newValue
                    Return True
                End If
                Return False
            Finally
                lock.ExitWriteLock()
            End Try
        End Function

        <OnDeserialized>
        Private Sub OnDeserialized(ByVal context As StreamingContext)
            lock = New ReaderWriterLockSlim()
        End Sub

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then
                Return True
            End If
            If o Is Nothing OrElse Me.GetType() IsNot o.GetType() Then
                Return False
            End If
            Dim other = DirectCast(o, Atomic(Of T))
            Return Object.Equals(Me.[get](), other.[get]())
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash([get]())
        End Function

        Public Overrides Function ToString() As String
            Return "Atomic(" & Arrays.FormatElement([get]()) & ")"
        End Function
    End Class

End Namespace
