Imports org.datavec.api.transform
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
Namespace org.deeplearning4j.rl4j.observation.transform.operation

	Public Class SimpleNormalizationTransform
		Implements Operation(Of INDArray, INDArray)

		Private ReadOnly offset As Double
		Private ReadOnly divisor As Double

		Public Sub New(ByVal min As Double, ByVal max As Double)
			Preconditions.checkArgument(min < max, "Min must be smaller than max.")

			Me.offset = min
			Me.divisor = (max - min)
		End Sub

		Public Overridable Function transform(ByVal input As INDArray) As INDArray
			If offset <> 0.0 Then
				input.subi(offset)
			End If

			input.divi(divisor)

			Return input
		End Function
	End Class

End Namespace