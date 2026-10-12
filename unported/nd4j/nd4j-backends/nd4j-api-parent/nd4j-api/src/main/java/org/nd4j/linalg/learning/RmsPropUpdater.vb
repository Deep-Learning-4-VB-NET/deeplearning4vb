Imports System.Collections.Generic
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports RmsProp = org.nd4j.linalg.learning.config.RmsProp

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
'ORIGINAL LINE: @Data public class RmsPropUpdater implements GradientUpdater<org.nd4j.linalg.learning.config.RmsProp>
	Public Class RmsPropUpdater
		Implements GradientUpdater(Of RmsProp)

		Public Const G_STATE As String = "G"

		Private ReadOnly config As RmsProp

		Private lastGradient As INDArray
		Private gradientReshapeOrder As Char

		Public Sub New(ByVal config As RmsProp)
			Me.config = config
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void setState(@NonNull Map<String, org.nd4j.linalg.api.ndarray.INDArray> stateMap, boolean initialize)
		Public Overridable Sub setState(ByVal stateMap As IDictionary(Of String, INDArray), ByVal initialize As Boolean)
			If Not stateMap.containsKey(G_STATE) OrElse stateMap.size() <> 1 Then
				Throw New System.InvalidOperationException("State map should contain only key [" & G_STATE & "] but has keys " & stateMap.keySet())
			End If
			Me.lastGradient = stateMap.get(G_STATE)
		End Sub

		Public Overridable Function getState() As IDictionary(Of String, INDArray) Implements GradientUpdater(Of RmsProp).getState
			Return Collections.singletonMap(G_STATE, Me.lastGradient)
		End Function

		Public Overridable Sub setStateViewArray(ByVal viewArray As INDArray, ByVal gradientShape() As Long, ByVal gradientOrder As Char, ByVal initialize As Boolean) Implements GradientUpdater(Of RmsProp).setStateViewArray
			If Not viewArray.RowVectorOrScalar Then
				Throw New System.ArgumentException("Invalid input: expect row vector input")
			End If
			If initialize Then
				viewArray.assign(config.getEpsilon())
			End If
			Me.lastGradient = viewArray

			'Reshape to match the expected shape of the input gradient arrays
			Me.lastGradient = Shape.newShapeNoCopy(Me.lastGradient, gradientShape, gradientOrder = "f"c)
			If lastGradient Is Nothing Then
				Throw New System.InvalidOperationException("Could not correctly reshape gradient view array")
			End If

			gradientReshapeOrder = gradientOrder
		End Sub

		Public Overridable Sub applyUpdater(ByVal gradient As INDArray, ByVal iteration As Integer, ByVal epoch As Integer) Implements GradientUpdater(Of RmsProp).applyUpdater
			If lastGradient Is Nothing Then
				Throw New System.InvalidOperationException("Updater has not been initialized with view state")
			End If

			Dim learningRate As Double = config.getLearningRate(iteration, epoch)
			Dim rmsDecay As Double = config.getRmsDecay()
			Dim epsilon As Double = config.getEpsilon()

			' lr * gradient / (sqrt(cache) + 1e-8)
			Nd4j.exec(New org.nd4j.linalg.api.ops.impl.updaters.RmsPropUpdater(gradient, lastGradient, learningRate, rmsDecay, epsilon))
		End Sub
	End Class

End Namespace