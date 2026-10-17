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

Namespace org.nd4vb.common.primitives

    ''' <summary>
    ''' A value holder whose reads and writes are guarded by a reader/writer lock.
    ''' </summary>
    <Serializable>
    Public Class SynchronizedObject(Of T)
        Protected Friend storedValue As T
        <NonSerialized>
        Protected Friend lock As ReaderWriterLockSlim

        Public Sub New()
            lock = New ReaderWriterLockSlim()
        End Sub

        Public Sub New(ByVal value As T)
            Me.New()
            Me.[set](value)
        End Sub

        ''' <summary>Returns the stored value under the read lock.</summary>
        Public Function [get]() As T
            lock.EnterReadLock()
            Try
                Return storedValue
            Finally
                lock.ExitReadLock()
            End Try
        End Function

        ''' <summary>Updates the stored value under the write lock.</summary>
        Public Sub [set](ByVal newValue As T)
            lock.EnterWriteLock()
            Try
                storedValue = newValue
            Finally
                lock.ExitWriteLock()
            End Try
        End Sub

        Public Property Value As T
            Get
                Return [get]()
            End Get
            Set(ByVal newValue As T)
                [set](newValue)
            End Set
        End Property

        <OnDeserialized>
        Private Sub OnDeserialized(ByVal context As StreamingContext)
            lock = New ReaderWriterLockSlim()
        End Sub
    End Class

End Namespace
