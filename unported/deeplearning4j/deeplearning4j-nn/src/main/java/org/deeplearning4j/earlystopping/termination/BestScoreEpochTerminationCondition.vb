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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class BestScoreEpochTerminationCondition implements EpochTerminationCondition
	<Serializable>
	Public Class BestScoreEpochTerminationCondition
		Implements EpochTerminationCondition

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonProperty private final double bestExpectedScore;
		Private ReadOnly bestExpectedScore As Double

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BestScoreEpochTerminationCondition(@JsonProperty("bestExpectedScore") double bestExpectedScore)
		Public Sub New(ByVal bestExpectedScore As Double)
			Me.bestExpectedScore = bestExpectedScore
		End Sub

		''' @deprecated "lessBetter" argument no longer used 
		<Obsolete("""lessBetter"" argument no longer used")>
		Public Sub New(ByVal bestExpectedScore As Double, ByVal lesserBetter As Boolean)
			Me.New(bestExpectedScore)
		End Sub

		Public Overridable Sub initialize() Implements EpochTerminationCondition.initialize
			' No OP 
		End Sub

		Public Overridable Function terminate(ByVal epochNum As Integer, ByVal score As Double, ByVal minimize As Boolean) As Boolean Implements EpochTerminationCondition.terminate
			If minimize Then
				Return score < bestExpectedScore
			Else
				Return bestExpectedScore < score
			End If
		End Function

		Public Overrides Function ToString() As String
			Return "BestScoreEpochTerminationCondition(" & bestExpectedScore & ")"
		End Function
	End Class

End Namespace