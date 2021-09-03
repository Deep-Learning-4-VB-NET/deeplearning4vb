Imports System
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports JsonCreator = org.nd4j.shade.jackson.annotation.JsonCreator
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
'ORIGINAL LINE: @NoArgsConstructor @Data public class MaxEpochsTerminationCondition implements EpochTerminationCondition
	<Serializable>
	Public Class MaxEpochsTerminationCondition
		Implements EpochTerminationCondition

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonProperty private int maxEpochs;
		Private maxEpochs As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonCreator public MaxEpochsTerminationCondition(int maxEpochs)
		Public Sub New(ByVal maxEpochs As Integer)
			If maxEpochs <= 0 Then
				Throw New System.ArgumentException("Max number of epochs must be >= 1")
			End If
			Me.maxEpochs = maxEpochs
		End Sub

		Public Overridable Sub initialize() Implements EpochTerminationCondition.initialize
			'No op
		End Sub

		Public Overridable Function terminate(ByVal epochNum As Integer, ByVal score As Double, ByVal minimize As Boolean) As Boolean Implements EpochTerminationCondition.terminate
			Return epochNum + 1 >= maxEpochs 'epochNum starts at 0
		End Function

		Public Overrides Function ToString() As String
			Return "MaxEpochsTerminationCondition(" & maxEpochs & ")"
		End Function
	End Class

End Namespace