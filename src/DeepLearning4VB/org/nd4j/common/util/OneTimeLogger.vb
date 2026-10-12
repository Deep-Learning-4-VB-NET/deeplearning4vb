Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Logger = DeepLearning4VB.org.slf4j.Logger

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

Namespace org.nd4j.common.util

    ''' <summary>
    ''' Logs each distinct message format only once (the last 100 distinct messages are remembered).
    ''' </summary>
    Public Class OneTimeLogger
        Private Const BUFFER_SIZE As Integer = 100

        Protected Friend Shared ReadOnly hashSet As New HashSet(Of String)()
        Protected Friend Shared ReadOnly buffer As New Queue(Of String)()

        Private Shared ReadOnly lock As New ReaderWriterLockSlim()

        ''' <summary>True the first time a message is seen, false while it is still remembered.</summary>
        Protected Friend Shared Function isEligible(ByVal message As String) As Boolean
            lock.EnterReadLock()
            Try
                If hashSet.Contains(message) Then
                    Return False
                End If
            Finally
                lock.ExitReadLock()
            End Try

            lock.EnterWriteLock()
            Try
                If hashSet.Contains(message) Then
                    Return False
                End If
                If buffer.Count >= BUFFER_SIZE Then
                    Dim removed As String = buffer.Dequeue()
                    hashSet.Remove(removed)
                End If
                buffer.Enqueue(message)
                hashSet.Add(message)
                Return True
            Finally
                lock.ExitWriteLock()
            End Try
        End Function

        Public Shared Sub info(ByVal logger As Logger, ByVal format As String, ByVal ParamArray arguments() As Object)
            If Not isEligible(format) Then
                Return
            End If
            logger.info(format, arguments)
        End Sub

        Public Shared Sub warn(ByVal logger As Logger, ByVal format As String, ByVal ParamArray arguments() As Object)
            If Not isEligible(format) Then
                Return
            End If
            logger.warn(format, arguments)
        End Sub

        Public Shared Sub [error](ByVal logger As Logger, ByVal format As String, ByVal ParamArray arguments() As Object)
            If Not isEligible(format) Then
                Return
            End If
            logger.error(format, arguments)
        End Sub

        ''' <summary>Forgets all remembered messages so they can be logged again.</summary>
        Public Shared Sub reset()
            lock.EnterWriteLock()
            Try
                buffer.Clear()
                hashSet.Clear()
            Finally
                lock.ExitWriteLock()
            End Try
        End Sub
    End Class

End Namespace
