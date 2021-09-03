Imports System.Collections.Generic
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports AmsGradUpdater = org.nd4j.linalg.api.ops.impl.updaters.AmsGradUpdater
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports AMSGrad = org.nd4j.linalg.learning.config.AMSGrad

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
'ORIGINAL LINE: @Data public class AMSGradUpdater implements GradientUpdater<org.nd4j.linalg.learning.config.AMSGrad>
	Public Class AMSGradUpdater
		Implements GradientUpdater(Of AMSGrad)

		Public Const M_STATE As String = "M"
		Public Const V_STATE As String = "V"
		Public Const V_HAT_STATE As String = "V_HAT"

		Private config As AMSGrad
		Private m, v, vHat As INDArray ' moving avg, sqrd gradients, max

		Private gradientReshapeOrder As Char

		Public Sub New(ByVal config As AMSGrad)
			Me.config = config
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void setState(@NonNull Map<String, org.nd4j.linalg.api.ndarray.INDArray> stateMap, boolean initialize)
		Public Overridable Sub setState(ByVal stateMap As IDictionary(Of String, INDArray), ByVal initialize As Boolean)
			If Not stateMap.containsKey(M_STATE) OrElse Not stateMap.containsKey(V_STATE) OrElse Not stateMap.containsKey(V_HAT_STATE) OrElse stateMap.size() <> 3 Then
				Throw New System.InvalidOperationException("State map should contain only keys [" & M_STATE & "," & V_STATE & "," & V_HAT_STATE & "] but has keys " & stateMap.keySet())
			End If
			Me.m = stateMap.get(M_STATE)
			Me.v = stateMap.get(V_STATE)
			Me.vHat = stateMap.get(V_HAT_STATE)
		End Sub

		Public Overridable Function getState() As IDictionary(Of String, INDArray) Implements GradientUpdater(Of AMSGrad).getState
			Dim r As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			r(M_STATE) = m
			r(V_STATE) = v
			r(V_HAT_STATE) = vHat
			Return r
		End Function

		Public Overridable Sub setStateViewArray(ByVal viewArray As INDArray, ByVal gradientShape() As Long, ByVal gradientOrder As Char, ByVal initialize As Boolean) Implements GradientUpdater(Of AMSGrad).setStateViewArray
			If Not viewArray.RowVector Then
				Throw New System.ArgumentException("Invalid input: expect row vector input")
			End If
			If initialize Then
				viewArray.assign(0)
			End If
			Dim n As val = viewArray.length() \ 3
			Me.m = viewArray.get(NDArrayIndex.point(0), NDArrayIndex.interval(0, n))
			Me.v = viewArray.get(NDArrayIndex.point(0), NDArrayIndex.interval(n, 2*n))
			Me.vHat = viewArray.get(NDArrayIndex.point(0), NDArrayIndex.interval(2*n, 3*n))

			'Reshape to match the expected shape of the input gradient arrays
			Me.m = Shape.newShapeNoCopy(Me.m, gradientShape, gradientOrder = "f"c)
			Me.v = Shape.newShapeNoCopy(Me.v, gradientShape, gradientOrder = "f"c)
			Me.vHat = Shape.newShapeNoCopy(Me.vHat, gradientShape, gradientOrder = "f"c)
			If m Is Nothing OrElse v Is Nothing OrElse vHat Is Nothing Then
				Throw New System.InvalidOperationException("Could not correctly reshape gradient view arrays")
			End If

			Me.gradientReshapeOrder = gradientOrder
		End Sub

		Public Overridable Sub applyUpdater(ByVal gradient As INDArray, ByVal iteration As Integer, ByVal epoch As Integer) Implements GradientUpdater(Of AMSGrad).applyUpdater
			If m Is Nothing OrElse v Is Nothing OrElse vHat Is Nothing Then
				Throw New System.InvalidOperationException("Updater has not been initialized with view state")
			End If

			Dim beta1 As Double = config.getBeta1()
			Dim beta2 As Double = config.getBeta2()
			Dim learningRate As Double = config.getLearningRate(iteration, epoch)
			Dim epsilon As Double = config.getEpsilon()

			'm_t = b_1 * m_{t-1} + (1-b_1) * g_t       eq 1 pg 3
			'v_t = b_2 * v_{t-1} + (1-b_2) * (g_t)^2   eq 1 pg 3
			'vHat_t = max(vHat_{t-1}, v_t)
			'gradient array contains: sqrt(vHat) + eps
			'gradient = alphat * m_t / (sqrt(vHat) + eps)

			Nd4j.exec(New AmsGradUpdater(gradient, v, m, vHat, learningRate, beta1, beta2, epsilon, iteration))
		End Sub
	End Class

End Namespace