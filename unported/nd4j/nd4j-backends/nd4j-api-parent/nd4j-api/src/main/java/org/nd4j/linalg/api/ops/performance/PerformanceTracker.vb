Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports AveragingTransactionsHolder = org.nd4j.linalg.api.ops.performance.primitives.AveragingTransactionsHolder
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports MemcpyDirection = org.nd4j.linalg.api.memory.MemcpyDirection

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

Namespace org.nd4j.linalg.api.ops.performance


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class PerformanceTracker
	Public Class PerformanceTracker
'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New PerformanceTracker()

		Private bandwidth As IDictionary(Of Integer, AveragingTransactionsHolder) = New Dictionary(Of Integer, AveragingTransactionsHolder)()
		Private operations As IDictionary(Of Integer, AveragingTransactionsHolder) = New Dictionary(Of Integer, AveragingTransactionsHolder)()

		Private Sub New()
			' we put in initial holders, one per device
			Dim nd As val = Nd4j.AffinityManager.NumberOfDevices
			For e As Integer = 0 To nd - 1
				bandwidth(e) = New AveragingTransactionsHolder()
				operations(e) = New AveragingTransactionsHolder()
			Next e
		End Sub

		Public Shared ReadOnly Property Instance As PerformanceTracker
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		''' <summary>
		''' This method stores bandwidth used for given transaction.
		''' 
		''' PLEASE NOTE: Bandwidth is stored in per millisecond value.
		''' </summary>
		''' <param name="deviceId"> device used for this transaction </param>
		''' <param name="timeSpent"> time spent on this transaction in nanoseconds </param>
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
		''' <param name="timeSpent"> time spent on this transaction in nanoseconds </param>
		''' <param name="numberOfBytes"> number of bytes </param>
		''' <param name="direction"> direction for the given memory transaction </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public long addMemoryTransaction(int deviceId, long timeSpentNanos, long numberOfBytes, @NonNull MemcpyDirection direction)
		Public Overridable Function addMemoryTransaction(ByVal deviceId As Integer, ByVal timeSpentNanos As Long, ByVal numberOfBytes As Long, ByVal direction As MemcpyDirection) As Long
			' we calculate bytes per microsecond now
			Dim bw As val = CLng(Math.Truncate(numberOfBytes / (timeSpentNanos / CDbl(1000.0))))

			' we skip too small values
			If bw > 0 Then
				bandwidth(deviceId).addValue(direction, bw)
			End If

			Return bw
		End Function

		Public Overridable Sub clear()
			For Each k As val In bandwidth.Keys
				bandwidth(k).clear()
			Next k
		End Sub


		Public Overridable Function helperStartTransaction() As Long
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.BANDWIDTH Then
				Return System.nanoTime()
			Else
				Return 0L
			End If
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void helperRegisterTransaction(int deviceId, long timeSpentNanos, long numberOfBytes, @NonNull MemcpyDirection direction)
		Public Overridable Sub helperRegisterTransaction(ByVal deviceId As Integer, ByVal timeSpentNanos As Long, ByVal numberOfBytes As Long, ByVal direction As MemcpyDirection)
			' only do something if profiling is enabled
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.BANDWIDTH Then
				addMemoryTransaction(deviceId, System.nanoTime() - timeSpentNanos, numberOfBytes, direction)
			End If
		End Sub

		Public Overridable ReadOnly Property CurrentBandwidth As IDictionary(Of Integer, IDictionary(Of MemcpyDirection, Long))
			Get
				Dim result As val = New Dictionary(Of Integer, IDictionary(Of MemcpyDirection, Long))()
				Dim keys As val = bandwidth.Keys
				For Each d As val In keys
    
					result.put(d, New Dictionary(Of MemcpyDirection, Long)())
    
					' get average for each MemcpyDirection and store it
					For Each m As val In System.Enum.GetValues(GetType(MemcpyDirection))
						result.get(d).put(m, bandwidth(d).getAverageValue(m))
					Next m
    
				Next d
    
				Return result
			End Get
		End Property
	End Class

End Namespace