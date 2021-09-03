Imports org.datavec.api.transform
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

	Public Class ArrayToINDArrayTransform
		Implements Operation(Of Double(), INDArray)

		Private ReadOnly shape() As Long

		''' <param name="shape"> Reshapes the INDArrays with this shape </param>
		Public Sub New(ParamArray ByVal shape() As Long)
			Me.shape = shape
		End Sub

		''' <summary>
		''' Will construct 1-D INDArrays
		''' </summary>
		Public Sub New()
			Me.shape = Nothing
		End Sub

		Public Overridable Function transform(ByVal data() As Double) As INDArray
			Dim result As INDArray = Nd4j.create(data)
			If shape IsNot Nothing Then
				result = result.reshape(shape)
			End If
			Return result
		End Function
	End Class

End Namespace