Imports System
Imports System.Globalization
Imports System.Threading.Tasks

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

Namespace org.nd4vb.linalg.executors

    ''' <summary>
    ''' The shared thread pools of ND4VB (Java <c>ExecutorServiceProvider</c>), sized by the system properties
    ''' <see cref="EXEC_THREADS"/> (default: the processor count) and <see cref="ENABLED"/> (default true; false means one
    ''' thread), read once.
    ''' </summary>
    ''' <remarks>
    ''' .NET has no <c>ExecutorService</c> or <c>ForkJoinPool</c>: <see cref="ExecutorService"/> is a <see cref="TaskFactory"/>
    ''' whose scheduler runs at most that many tasks at once on the .NET thread pool (background threads, like Java's daemon
    ''' threads; <c>submit(task)</c> is <c>StartNew(action)</c> and <c>future.get(timeout)</c> is <c>task.Wait(timeout)</c>),
    ''' and <see cref="ForkJoinPool"/> is a <see cref="ParallelOptions"/> with that degree of parallelism and the same
    ''' scheduler, for <c>Parallel.For</c>/<c>Parallel.ForEach</c>.
    ''' </remarks>
    Public Class ExecutorServiceProvider

        ''' <summary>The system property with the number of threads.</summary>
        Public Const EXEC_THREADS As String = "org.nd4vb.parallel.threads"
        ''' <summary>The system property that enables parallel execution ("true" by default).</summary>
        Public Const ENABLED As String = "org.nd4vb.parallel.enabled"

        Private Shared ReadOnly nThreads As Integer = readThreads()
        Private Shared executorServiceField As TaskFactory
        Private Shared forkJoinPoolField As ParallelOptions
        Private Shared scheduler As TaskScheduler
        Private Shared ReadOnly gate As New Object()

        Private Sub New()
        End Sub

        Private Shared Function readThreads() As Integer
            Dim defaultThreads As Integer = System.Environment.ProcessorCount
            ' Java Boolean.parseBoolean: only "true" (any case) enables
            Dim enabled As Boolean = String.Equals(JavaSystem.getProperty(ENABLED, "true"), "true", StringComparison.OrdinalIgnoreCase)
            If Not enabled Then
                Return 1
            End If
            Dim value As String = JavaSystem.getProperty(EXEC_THREADS, defaultThreads.ToString(CultureInfo.InvariantCulture))
            Dim n As Integer
            If Not Integer.TryParse(value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, n) Then
                Throw New JavaCompat.NumberFormatException("For input string: """ & value & """")
            End If
            Return Math.Max(1, n)
        End Function

        ''' <summary>The number of threads of the pools.</summary>
        Public Shared ReadOnly Property NumberOfThreads As Integer
            Get
                Return nThreads
            End Get
        End Property

        Private Shared Function limitedScheduler() As TaskScheduler
            If scheduler Is Nothing Then
                scheduler = New ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, nThreads).ConcurrentScheduler
            End If
            Return scheduler
        End Function

        ''' <summary>A task factory that runs at most <see cref="NumberOfThreads"/> tasks at once (Java: a fixed thread pool).</summary>
        Public Shared ReadOnly Property ExecutorService As TaskFactory
            Get
                SyncLock gate
                    If executorServiceField Is Nothing Then
                        executorServiceField = New TaskFactory(limitedScheduler())
                    End If
                    Return executorServiceField
                End SyncLock
            End Get
        End Property

        ''' <summary>Options for <c>Parallel</c> loops with <see cref="NumberOfThreads"/> as the degree of parallelism (Java: a fork-join pool).</summary>
        Public Shared ReadOnly Property ForkJoinPool As ParallelOptions
            Get
                SyncLock gate
                    If forkJoinPoolField Is Nothing Then
                        forkJoinPoolField = New ParallelOptions() With {.MaxDegreeOfParallelism = nThreads, .TaskScheduler = limitedScheduler()}
                    End If
                    Return forkJoinPoolField
                End SyncLock
            End Get
        End Property

    End Class

End Namespace
