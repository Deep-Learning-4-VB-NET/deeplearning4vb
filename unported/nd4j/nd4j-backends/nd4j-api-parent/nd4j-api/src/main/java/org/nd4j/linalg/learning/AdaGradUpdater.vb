Imports System.Collections.Generic
Imports Data = lombok.Data
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports AdaGrad = org.nd4j.linalg.learning.config.AdaGrad

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

Namespace org.nd4j.linalg.learning




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class AdaGradUpdater implements GradientUpdater<org.nd4j.linalg.learning.config.AdaGrad>
	Public Class AdaGradUpdater
		Implements GradientUpdater(Of AdaGrad)

		Public Const GRAD_STATE As String = "grad"
		Public historicalGradient As INDArray
		Public shape() As Integer
		Protected Friend learningRate As Double = 1e-1 ' learning rate
		Protected Friend numIterations As Integer = 0
		Private epsilon As Double = AdaGrad.DEFAULT_ADAGRAD_EPSILON

		Private gradientReshapeOrder As Char

		Private config As AdaGrad

		Public Sub New(ByVal config As AdaGrad)
			Me.config = config
		End Sub

		Public Overridable Sub setState(ByVal stateMap As IDictionary(Of String, INDArray), ByVal initialize As Boolean) Implements GradientUpdater(Of AdaGrad).setState
			If Not stateMap.ContainsKey(GRAD_STATE) OrElse stateMap.Count <> 1 Then
				Throw New System.InvalidOperationException("State map should contain only key [" & GRAD_STATE & "] but has keys " & stateMap.Keys)
			End If
			Me.historicalGradient = stateMap(GRAD_STATE)
		End Sub

		Public Overridable Function getState() As IDictionary(Of String, INDArray) Implements GradientUpdater(Of AdaGrad).getState
			Return Collections.singletonMap(GRAD_STATE, historicalGradient)
		End Function

		Public Overridable Sub setStateViewArray(ByVal viewArray As INDArray, ByVal gradientShape() As Long, ByVal gradientOrder As Char, ByVal initialize As Boolean) Implements GradientUpdater(Of AdaGrad).setStateViewArray
			If Not viewArray.RowVectorOrScalar Then
				Throw New System.ArgumentException("Invalid input: expect row vector input")
			End If
			If initialize Then
				viewArray.assign(epsilon)
			End If
			Me.historicalGradient = viewArray
			'Reshape to match the expected shape of the input gradient arrays
			Me.historicalGradient = Shape.newShapeNoCopy(Me.historicalGradient, gradientShape, gradientOrder = "f"c)
			If historicalGradient Is Nothing Then
				Throw New System.InvalidOperationException("Could not correctly reshape gradient view array")
			End If

			Me.gradientReshapeOrder = gradientOrder
		End Sub

		''' <summary>
		''' Gets feature specific learning rates
		''' Adagrad keeps a history of gradients being passed in.
		''' Note that each gradient passed in becomes adapted over time, hence the opName adagrad
		''' </summary>
		''' <param name="gradient">  the gradient to get learning rates for </param>
		''' <param name="iteration"> </param>
		Public Overridable Sub applyUpdater(ByVal gradient As INDArray, ByVal iteration As Integer, ByVal epoch As Integer) Implements GradientUpdater(Of AdaGrad).applyUpdater
			If historicalGradient Is Nothing Then
				Throw New System.InvalidOperationException("Updater has not been initialized with view state")
			End If

			Dim learningRate As Double = config.getLearningRate(iteration, epoch)
			Dim epsilon As Double = config.getEpsilon()

			Nd4j.exec(New org.nd4j.linalg.api.ops.impl.updaters.AdaGradUpdater(gradient, historicalGradient, learningRate, epsilon))
		End Sub
	End Class

End Namespace