Imports System
Imports Data = lombok.Data
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.deeplearning4j.earlystopping.termination


	''' <summary>
	'''Terminate training based on max time.
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class MaxTimeIterationTerminationCondition implements IterationTerminationCondition
	<Serializable>
	Public Class MaxTimeIterationTerminationCondition
		Implements IterationTerminationCondition

		Private maxTimeAmount As Long
		Private maxTimeUnit As TimeUnit
		Private initializationTime As Long
		Private endTime As Long

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MaxTimeIterationTerminationCondition(@JsonProperty("maxTimeAmount") long maxTimeAmount, @JsonProperty("maxTimeUnit") java.util.concurrent.TimeUnit maxTimeUnit)
		Public Sub New(ByVal maxTimeAmount As Long, ByVal maxTimeUnit As TimeUnit)
			If maxTimeAmount <= 0 OrElse maxTimeUnit Is Nothing Then
				Throw New System.ArgumentException("Invalid maximum training time: " & "amount = " & maxTimeAmount & " unit = " & maxTimeUnit)
			End If
			Me.maxTimeAmount = maxTimeAmount
			Me.maxTimeUnit = maxTimeUnit
		End Sub

		Public Overridable Sub initialize() Implements IterationTerminationCondition.initialize
			initializationTime = DateTimeHelper.CurrentUnixTimeMillis()
			endTime = initializationTime + maxTimeUnit.toMillis(maxTimeAmount)
		End Sub

		Public Overridable Function terminate(ByVal lastMiniBatchScore As Double) As Boolean Implements IterationTerminationCondition.terminate
			Return DateTimeHelper.CurrentUnixTimeMillis() >= endTime
		End Function

		Public Overrides Function ToString() As String
			Return "MaxTimeIterationTerminationCondition(" & maxTimeAmount & ",unit=" & maxTimeUnit & ")"
		End Function
	End Class

End Namespace