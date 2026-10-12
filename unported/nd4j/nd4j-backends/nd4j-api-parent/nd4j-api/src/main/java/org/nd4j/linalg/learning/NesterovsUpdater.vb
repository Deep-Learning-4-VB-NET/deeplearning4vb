Imports System.Collections.Generic
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs

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
'ORIGINAL LINE: @Data public class NesterovsUpdater implements GradientUpdater<org.nd4j.linalg.learning.config.Nesterovs>
	Public Class NesterovsUpdater
		Implements GradientUpdater(Of Nesterovs)

		Public Const V_STATE As String = "V"

		Private ReadOnly config As Nesterovs

		Private v As INDArray
		Private gradientReshapeOrder As Char

		Public Sub New(ByVal config As Nesterovs)
			Me.config = config
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void setState(@NonNull Map<String, org.nd4j.linalg.api.ndarray.INDArray> stateMap, boolean initialize)
		Public Overridable Sub setState(ByVal stateMap As IDictionary(Of String, INDArray), ByVal initialize As Boolean)
			If Not stateMap.containsKey(V_STATE) OrElse stateMap.size() <> 1 Then
				Throw New System.InvalidOperationException("State map should contain only key [" & V_STATE & "] but has keys " & stateMap.keySet())
			End If
			Me.v = stateMap.get(V_STATE)
		End Sub

		Public Overridable Function getState() As IDictionary(Of String, INDArray) Implements GradientUpdater(Of Nesterovs).getState
			Return Collections.singletonMap(V_STATE, Me.v)
		End Function

		Public Overridable Sub setStateViewArray(ByVal viewArray As INDArray, ByVal gradientShape() As Long, ByVal gradientOrder As Char, ByVal initialize As Boolean) Implements GradientUpdater(Of Nesterovs).setStateViewArray
			If Not viewArray.RowVectorOrScalar Then
				Throw New System.ArgumentException("Invalid input: expect row vector input")
			End If
			If initialize Then
				viewArray.assign(0)
			End If

			Me.v = viewArray

			'Reshape to match the expected shape of the input gradient arrays
			Me.v = Shape.newShapeNoCopy(Me.v, gradientShape, gradientOrder = "f"c)
			If v Is Nothing Then
				Throw New System.InvalidOperationException("Could not correctly reshape gradient view array")
			End If
			Me.gradientReshapeOrder = gradientOrder
		End Sub

		''' <summary>
		''' Get the nesterov update
		''' </summary>
		''' <param name="gradient">  the gradient to get the update for </param>
		''' <param name="iteration">
		''' @return </param>
		Public Overridable Sub applyUpdater(ByVal gradient As INDArray, ByVal iteration As Integer, ByVal epoch As Integer) Implements GradientUpdater(Of Nesterovs).applyUpdater
			If v Is Nothing Then
				Throw New System.InvalidOperationException("Updater has not been initialized with view state")
			End If

			Dim momentum As Double = config.currentMomentum(iteration, epoch)
			Dim learningRate As Double = config.getLearningRate(iteration, epoch)

			'reference https://cs231n.github.io/neural-networks-3/#sgd 2nd equation
			'DL4J default is negative step function thus we flipped the signs:
			' x += mu * v_prev + (-1 - mu) * v
			'i.e., we do params -= updatedGradient, not params += updatedGradient
			'v = mu * v - lr * gradient

			Nd4j.exec(New org.nd4j.linalg.api.ops.impl.updaters.NesterovsUpdater(gradient, v, learningRate, momentum))
		End Sub
	End Class

End Namespace