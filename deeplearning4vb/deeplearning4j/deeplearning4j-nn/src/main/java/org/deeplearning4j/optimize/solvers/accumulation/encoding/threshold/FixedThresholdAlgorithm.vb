Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports ThresholdAlgorithm = org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithm
Imports ThresholdAlgorithmReducer = org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithmReducer
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.optimize.solvers.accumulation.encoding.threshold

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public class FixedThresholdAlgorithm implements org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithm
	<Serializable>
	Public Class FixedThresholdAlgorithm
		Implements ThresholdAlgorithm

		Private ReadOnly threshold As Double

		Public Overridable Function calculateThreshold(ByVal iteration As Integer, ByVal epoch As Integer, ByVal lastThreshold As Double?, ByVal lastWasDense As Boolean?, ByVal lastSparsityRatio As Double?, ByVal updatesPlusResidual As INDArray) As Double Implements ThresholdAlgorithm.calculateThreshold
			Return threshold
		End Function

		Public Overridable Function newReducer() As ThresholdAlgorithmReducer
			Return New FixedAlgorithmThresholdReducer()
		End Function

		Public Overridable Function clone() As FixedThresholdAlgorithm
			Return New FixedThresholdAlgorithm(threshold)
		End Function


		<Serializable>
		Public Class FixedAlgorithmThresholdReducer
			Implements ThresholdAlgorithmReducer

			Friend instance As FixedThresholdAlgorithm

			Public Overridable Sub add(ByVal instance As ThresholdAlgorithm)
				Preconditions.checkState(TypeOf instance Is FixedThresholdAlgorithm, "Invalid threshold: cannot be reduced using this class, %s", instance.GetType().Name)
				Me.instance = DirectCast(instance, FixedThresholdAlgorithm)
			End Sub

			Public Overridable Function merge(ByVal other As ThresholdAlgorithmReducer) As ThresholdAlgorithmReducer
				If Me.instance IsNot Nothing OrElse other Is Nothing Then
					Return Me
				End If
				Me.instance = DirectCast(other, FixedAlgorithmThresholdReducer).instance
				Return Me
			End Function

			Public Overridable ReadOnly Property FinalResult As ThresholdAlgorithm
				Get
					Return instance
				End Get
			End Property
		End Class
	End Class

End Namespace