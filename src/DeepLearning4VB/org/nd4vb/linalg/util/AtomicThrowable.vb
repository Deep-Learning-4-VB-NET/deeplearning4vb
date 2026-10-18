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

Namespace org.nd4vb.linalg.util

    ''' <summary>
    ''' A thread-safe holder for an exception, used to pass the first failure of a worker thread to the caller.
    ''' <para>
    ''' Java's <c>Throwable</c> is <see cref="System.Exception"/>, the fair <c>ReentrantReadWriteLock</c> is a
    ''' <see cref="ReaderWriterLockSlim"/> (which has no fairness option), Java's <c>get()</c>/<c>set()</c> are
    ''' <see cref="[get]"/>/<see cref="[set]"/>, the setter <c>setIfFirst(Throwable)</c> is the method
    ''' <see cref="setIfFirst"/> and <c>isTriggered()</c> is the property <see cref="Triggered"/>.
    ''' </para>
    ''' </summary>
    ''' @author raver119@gmail.com
    Public Class AtomicThrowable

        ''' <summary>The stored exception (Java <c>volatile Throwable t</c>).</summary>
        Protected Friend t As System.Exception = Nothing
        ''' <summary>Guards <see cref="t"/>.</summary>
        Protected Friend lock As ReaderWriterLockSlim

        ''' <summary>
        ''' Creates an empty holder.
        ''' </summary>
        Public Sub New()
            ' unfortunately we want fair queue here, to avoid situations when we return null after exception
            lock = New ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion)
        End Sub

        ''' <summary>
        ''' Creates a holder that already contains <paramref name="e"/>.
        ''' </summary>
        Public Sub New(ByVal e As System.Exception)
            Me.New()
            t = e
        End Sub

        ''' <summary>
        ''' The stored exception, or <c>Nothing</c>.
        ''' </summary>
        Public Overridable Function [get]() As System.Exception
            lock.EnterReadLock()
            Try
                Return t
            Finally
                lock.ExitReadLock()
            End Try
        End Function

        ''' <summary>
        ''' Stores an exception, replacing any stored one.
        ''' </summary>
        Public Overridable Sub [set](ByVal t As System.Exception)
            lock.EnterWriteLock()
            Try
                Me.t = t
            Finally
                lock.ExitWriteLock()
            End Try
        End Sub

        ''' <summary>
        ''' Stores an exception only if none is stored yet.
        ''' </summary>
        Public Overridable Sub setIfFirst(ByVal t As System.Exception)
            lock.EnterWriteLock()
            Try
                If Me.t Is Nothing Then
                    Me.t = t
                End If
            Finally
                lock.ExitWriteLock()
            End Try
        End Sub

        ''' <summary>
        ''' True when an exception is stored.
        ''' </summary>
        Public Overridable ReadOnly Property Triggered As Boolean
            Get
                lock.EnterReadLock()
                Try
                    Return t IsNot Nothing
                Finally
                    lock.ExitReadLock()
                End Try
            End Get
        End Property

    End Class

End Namespace
