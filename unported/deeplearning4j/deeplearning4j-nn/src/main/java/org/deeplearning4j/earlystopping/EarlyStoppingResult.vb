Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Model = org.deeplearning4j.nn.api.Model

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

Namespace org.deeplearning4j.earlystopping


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class EarlyStoppingResult<T extends org.deeplearning4j.nn.api.Model> implements java.io.Serializable
	<Serializable>
	Public Class EarlyStoppingResult(Of T As org.deeplearning4j.nn.api.Model)
		Public Enum TerminationReason
			[Error]
			IterationTerminationCondition
			EpochTerminationCondition
		End Enum

		Private terminationReason As TerminationReason
		Private terminationDetails As String
		Private scoreVsEpoch As IDictionary(Of Integer, Double)
		Private bestModelEpoch As Integer
		Private bestModelScore As Double
		Private totalEpochs As Integer
'JAVA TO VB CONVERTER NOTE: The field bestModel was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private bestModel_Conflict As T

		Public Sub New(ByVal terminationReason As TerminationReason, ByVal terminationDetails As String, ByVal scoreVsEpoch As IDictionary(Of Integer, Double), ByVal bestModelEpoch As Integer, ByVal bestModelScore As Double, ByVal totalEpochs As Integer, ByVal bestModel As T)
			Me.terminationReason = terminationReason
			Me.terminationDetails = terminationDetails
			Me.scoreVsEpoch = scoreVsEpoch
			Me.bestModelEpoch = bestModelEpoch
			Me.bestModelScore = bestModelScore
			Me.totalEpochs = totalEpochs
			Me.bestModel_Conflict = bestModel
		End Sub

		Public Overrides Function ToString() As String
			Return "EarlyStoppingResult(terminationReason=" & terminationReason & ",details=" & terminationDetails & ",bestModelEpoch=" & bestModelEpoch & ",bestModelScore=" & bestModelScore & ",totalEpochs=" & totalEpochs & ")"

		End Function

		Public Overridable ReadOnly Property BestModel As T
			Get
				Return bestModel_Conflict
			End Get
		End Property

	End Class

End Namespace