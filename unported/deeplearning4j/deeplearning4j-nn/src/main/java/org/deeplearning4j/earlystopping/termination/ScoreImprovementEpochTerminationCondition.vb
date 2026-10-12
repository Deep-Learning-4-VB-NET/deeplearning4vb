Imports System
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
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
'ORIGINAL LINE: @Slf4j @Data public class ScoreImprovementEpochTerminationCondition implements EpochTerminationCondition
	<Serializable>
	Public Class ScoreImprovementEpochTerminationCondition
		Implements EpochTerminationCondition

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonProperty private int maxEpochsWithNoImprovement;
		Private maxEpochsWithNoImprovement As Integer
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonProperty private int bestEpoch = -1;
		Private bestEpoch As Integer = -1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonProperty private double bestScore;
		Private bestScore As Double
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonProperty private double minImprovement = 0.0;
		Private minImprovement As Double = 0.0

		Public Sub New(ByVal maxEpochsWithNoImprovement As Integer)
			Me.maxEpochsWithNoImprovement = maxEpochsWithNoImprovement
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ScoreImprovementEpochTerminationCondition(@JsonProperty("maxEpochsWithNoImprovement") int maxEpochsWithNoImprovement, @JsonProperty("minImprovement") double minImprovement)
		Public Sub New(ByVal maxEpochsWithNoImprovement As Integer, ByVal minImprovement As Double)
			Me.maxEpochsWithNoImprovement = maxEpochsWithNoImprovement
			Me.minImprovement = minImprovement
		End Sub

		Public Overridable Sub initialize() Implements EpochTerminationCondition.initialize
			bestEpoch = -1
			bestScore = Double.NaN
		End Sub

		Public Overridable Function terminate(ByVal epochNum As Integer, ByVal score As Double, ByVal minimize As Boolean) As Boolean Implements EpochTerminationCondition.terminate
			If bestEpoch = -1 Then
				bestEpoch = epochNum
				bestScore = score
				Return False
			Else
				Dim improvement As Double = (If(minimize, bestScore - score, score - bestScore))
				If improvement > minImprovement Then
					If minImprovement > 0 Then
						log.info("Epoch with score greater than threshold * * *")
					End If
					bestScore = score
					bestEpoch = epochNum
					Return False
				End If

				Return epochNum >= bestEpoch + maxEpochsWithNoImprovement
			End If
		End Function

		Public Overrides Function ToString() As String
			Return "ScoreImprovementEpochTerminationCondition(maxEpochsWithNoImprovement=" & maxEpochsWithNoImprovement & ", minImprovement=" & minImprovement & ")"
		End Function
	End Class

End Namespace