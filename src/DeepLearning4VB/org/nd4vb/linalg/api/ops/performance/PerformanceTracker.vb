Imports System
Imports System.Collections.Generic
Imports MemcpyDirection = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemcpyDirection
Imports OpExecutioner = DeepLearning4VB.org.nd4vb.linalg.api.ops.executioner.OpExecutioner
Imports AveragingTransactionsHolder = DeepLearning4VB.org.nd4vb.linalg.api.ops.performance.primitives.AveragingTransactionsHolder
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

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

Namespace org.nd4vb.linalg.api.ops.performance

    ''' <summary>
    ''' This class provides routines for performance tracking and holder for corresponding results: the memory
    ''' bandwidth per device and copy direction, recorded while the executioner's profiling mode is
    ''' <c>BANDWIDTH</c>. The singleton creates one holder per device of <c>Nd4vb.AffinityManager</c>.
    ''' </summary>
    Public Class PerformanceTracker

        Private Shared ReadOnly singleton As New PerformanceTracker()

        Private bandwidth As IDictionary(Of Integer, AveragingTransactionsHolder) = New Dictionary(Of Integer, AveragingTransactionsHolder)()
        Private operations As IDictionary(Of Integer, AveragingTransactionsHolder) = New Dictionary(Of Integer, AveragingTransactionsHolder)()

        Private Sub New()
            ' we put in initial holders, one per device
            Dim nd As Integer = Nd4vbT.AffinityManager.NumberOfDevices
            For e As Integer = 0 To nd - 1
                bandwidth(e) = New AveragingTransactionsHolder()
                operations(e) = New AveragingTransactionsHolder()
            Next e
        End Sub

        ''' <summary>The singleton (Java <c>getInstance()</c>).</summary>
        Public Shared ReadOnly Property Instance As PerformanceTracker
            Get
                Return singleton
            End Get
        End Property

        ''' <summary>
        ''' This method stores bandwidth used for given transaction.
        '''
        ''' PLEASE NOTE: Bandwidth is stored in per millisecond value.
        ''' </summary>
        ''' <param name="deviceId"> device used for this transaction </param>
        ''' <param name="timeSpentNanos"> time spent on this transaction in nanoseconds </param>
        ''' <param name="numberOfBytes"> number of bytes </param>
        Public Overridable Function addMemoryTransaction(ByVal deviceId As Integer, ByVal timeSpentNanos As Long, ByVal numberOfBytes As Long) As Long
            ' default is H2H transaction
            Return addMemoryTransaction(deviceId, timeSpentNanos, numberOfBytes, MemcpyDirection.HOST_TO_HOST)
        End Function

        ''' <summary>
        ''' This method stores bandwidth used for given transaction.
        '''
        ''' PLEASE NOTE: Bandwidth is stored in per millisecond value.
        ''' </summary>
        ''' <param name="deviceId"> device used for this transaction </param>
        ''' <param name="timeSpentNanos"> time spent on this transaction in nanoseconds </param>
        ''' <param name="numberOfBytes"> number of bytes </param>
        ''' <param name="direction"> direction for the given memory transaction </param>
        Public Overridable Function addMemoryTransaction(ByVal deviceId As Integer, ByVal timeSpentNanos As Long, ByVal numberOfBytes As Long, ByVal direction As MemcpyDirection) As Long
            ' we calculate bytes per microsecond now
            Dim bw As Long = CLng(Math.Truncate(numberOfBytes / (timeSpentNanos / CDbl(1000.0))))

            ' we skip too small values
            If bw > 0 Then
                bandwidth(deviceId).addValue(direction, bw)
            End If

            Return bw
        End Function

        Public Overridable Sub clear()
            For Each k As Integer In bandwidth.Keys
                bandwidth(k).clear()
            Next k
        End Sub

        Public Overridable Function helperStartTransaction() As Long
            If Nd4vbT.Executioner.getProfilingMode() = OpExecutioner.ProfilingMode.BANDWIDTH Then
                Return JavaSystem.nanoTime()
            Else
                Return 0L
            End If
        End Function

        Public Overridable Sub helperRegisterTransaction(ByVal deviceId As Integer, ByVal timeSpentNanos As Long, ByVal numberOfBytes As Long, ByVal direction As MemcpyDirection)
            ' only do something if profiling is enabled
            If Nd4vbT.Executioner.getProfilingMode() = OpExecutioner.ProfilingMode.BANDWIDTH Then
                addMemoryTransaction(deviceId, JavaSystem.nanoTime() - timeSpentNanos, numberOfBytes, direction)
            End If
        End Sub

        ''' <summary>The average bandwidth per device and copy direction (Java <c>getCurrentBandwidth()</c>).</summary>
        Public Overridable ReadOnly Property CurrentBandwidth As IDictionary(Of Integer, IDictionary(Of MemcpyDirection, Long))
            Get
                Dim result As New Dictionary(Of Integer, IDictionary(Of MemcpyDirection, Long))()
                For Each d As Integer In bandwidth.Keys
                    Dim perDirection As New Dictionary(Of MemcpyDirection, Long)()
                    ' get average for each MemcpyDirection and store it
                    For Each m As MemcpyDirection In [Enum].GetValues(GetType(MemcpyDirection))
                        perDirection(m) = bandwidth(d).getAverageValue(m).Value
                    Next m
                    result(d) = perDirection
                Next d

                Return result
            End Get
        End Property

    End Class

End Namespace
