Imports System.Collections.Generic
Imports Data = lombok.Data
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports AdaDelta = org.nd4j.linalg.learning.config.AdaDelta

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
'ORIGINAL LINE: @Data public class AdaDeltaUpdater implements GradientUpdater<org.nd4j.linalg.learning.config.AdaDelta>
	Public Class AdaDeltaUpdater
		Implements GradientUpdater(Of AdaDelta)

		Public Const MSG_STATE As String = "msg"
		Public Const MSDX_STATE As String = "msdx"

		Private ReadOnly config As AdaDelta

		Private msg As INDArray 'E[g^2]_t by arxiv paper, algorithm 1
		Private msdx As INDArray 'E[delta x^2]_t by arxiv paper, algorithm 1



		Public Sub New(ByVal config As AdaDelta)
			Me.config = config
		End Sub

		Public Overridable Sub setState(ByVal stateMap As IDictionary(Of String, INDArray), ByVal initialize As Boolean) Implements GradientUpdater(Of AdaDelta).setState
			If Not stateMap.ContainsKey(MSG_STATE) OrElse Not stateMap.ContainsKey(MSDX_STATE) OrElse stateMap.Count <> 2 Then
				Throw New System.InvalidOperationException("State map should contain only keys [" & MSG_STATE & "," & MSDX_STATE & "] but has keys " & stateMap.Keys)
			End If
			Me.msg = stateMap(MSG_STATE)
			Me.msdx = stateMap(MSDX_STATE)
		End Sub

		Public Overridable Function getState() As IDictionary(Of String, INDArray) Implements GradientUpdater(Of AdaDelta).getState
			Dim r As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			r(MSG_STATE) = msg
			r(MSDX_STATE) = msdx
			Return r
		End Function

		Public Overridable Sub setStateViewArray(ByVal viewArray As INDArray, ByVal gradientShape() As Long, ByVal gradientOrder As Char, ByVal initialize As Boolean) Implements GradientUpdater(Of AdaDelta).setStateViewArray
			If Not viewArray.RowVector Then
				Throw New System.ArgumentException("Invalid input: expect row vector input")
			End If
			If initialize Then
				viewArray.assign(0)
			End If
			Dim length As Long = viewArray.length()
			Me.msg = viewArray.get(NDArrayIndex.point(0), NDArrayIndex.interval(0, length \ 2))
			Me.msdx = viewArray.get(NDArrayIndex.point(0), NDArrayIndex.interval(length \ 2, length))

			'Reshape to match the expected shape of the input gradient arrays
			Me.msg = Shape.newShapeNoCopy(Me.msg, gradientShape, gradientOrder = "f"c)
			Me.msdx = Shape.newShapeNoCopy(Me.msdx, gradientShape, gradientOrder = "f"c)
			If msg Is Nothing OrElse msdx Is Nothing Then
				Throw New System.InvalidOperationException("Could not correctly reshape gradient view arrays")
			End If
		End Sub

		''' <summary>
		''' Get the updated gradient for the given gradient
		''' and also update the state of ada delta.
		''' </summary>
		''' <param name="gradient">  the gradient to get the
		'''                  updated gradient for </param>
		''' <param name="iteration"> </param>
		''' <returns> the update gradient </returns>
		Public Overridable Sub applyUpdater(ByVal gradient As INDArray, ByVal iteration As Integer, ByVal epoch As Integer) Implements GradientUpdater(Of AdaDelta).applyUpdater
			If msg Is Nothing OrElse msdx Is Nothing Then
				Throw New System.InvalidOperationException("Updater has not been initialized with view state")
			End If

			Dim rho As Double = config.getRho()
			Dim epsilon As Double = config.getEpsilon()

			'Line 4 of Algorithm 1: https://arxiv.org/pdf/1212.5701v1.pdf
			'E[g^2]_t = rho * E[g^2]_{t-1} + (1-rho)*g^2_t
			'Calculate update:
			'dX = - g * RMS[delta x]_{t-1} / RMS[g]_t
			'Note: negative is applied in the DL4J step function: params -= update rather than params += update
			'Accumulate gradients: E[delta x^2]_t = rho * E[delta x^2]_{t-1} + (1-rho)* (delta x_t)^2

			Nd4j.exec(New org.nd4j.linalg.api.ops.impl.updaters.AdaDeltaUpdater(gradient, msg, msdx, rho, epsilon))
		End Sub
	End Class

End Namespace