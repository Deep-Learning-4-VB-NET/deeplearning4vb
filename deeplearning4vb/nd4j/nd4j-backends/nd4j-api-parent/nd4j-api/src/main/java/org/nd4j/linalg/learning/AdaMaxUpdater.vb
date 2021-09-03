﻿Imports System.Collections.Generic
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports AdaMax = org.nd4j.linalg.learning.config.AdaMax

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


	''' <summary>
	''' The AdaMax updater, a variant of Adam.
	''' https://arxiv.org/abs/1412.6980
	''' 
	''' @author Justin Long
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class AdaMaxUpdater implements GradientUpdater<org.nd4j.linalg.learning.config.AdaMax>
	Public Class AdaMaxUpdater
		Implements GradientUpdater(Of AdaMax)

		Public Const M_STATE As String = "M"
		Public Const U_STATE As String = "V"

		Private ReadOnly config As AdaMax

		Private m, u As INDArray ' moving avg & exponentially weighted infinity norm
		Private gradientReshapeOrder As Char

		Public Sub New(ByVal config As AdaMax)
			Me.config = config
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void setState(@NonNull Map<String, org.nd4j.linalg.api.ndarray.INDArray> stateMap, boolean initialize)
		Public Overridable Sub setState(ByVal stateMap As IDictionary(Of String, INDArray), ByVal initialize As Boolean)
			If Not stateMap.containsKey(M_STATE) OrElse Not stateMap.containsKey(U_STATE) OrElse stateMap.size() <> 2 Then
				Throw New System.InvalidOperationException("State map should contain only keys [" & M_STATE & "," & U_STATE & "] but has keys " & stateMap.keySet())
			End If
			Me.m = stateMap.get(M_STATE)
			Me.u = stateMap.get(U_STATE)
		End Sub

		Public Overridable Function getState() As IDictionary(Of String, INDArray) Implements GradientUpdater(Of AdaMax).getState
			Dim r As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			r(M_STATE) = m
			r(U_STATE) = u
			Return r
		End Function

		Public Overridable Sub setStateViewArray(ByVal viewArray As INDArray, ByVal gradientShape() As Long, ByVal gradientOrder As Char, ByVal initialize As Boolean) Implements GradientUpdater(Of AdaMax).setStateViewArray
			If Not viewArray.RowVector Then
				Throw New System.ArgumentException("Invalid input: expect row vector input")
			End If
			If initialize Then
				viewArray.assign(0)
			End If
			Dim length As Long = viewArray.length()
			Me.m = viewArray.get(NDArrayIndex.point(0), NDArrayIndex.interval(0, length \ 2))
			Me.u = viewArray.get(NDArrayIndex.point(0), NDArrayIndex.interval(length \ 2, length))

			'Reshape to match the expected shape of the input gradient arrays
			Me.m = Shape.newShapeNoCopy(Me.m, gradientShape, gradientOrder = "f"c)
			Me.u = Shape.newShapeNoCopy(Me.u, gradientShape, gradientOrder = "f"c)
			If m Is Nothing OrElse u Is Nothing Then
				Throw New System.InvalidOperationException("Could not correctly reshape gradient view arrays")
			End If

			Me.gradientReshapeOrder = gradientOrder
		End Sub

		''' <summary>
		''' Calculate the update based on the given gradient
		''' </summary>
		''' <param name="gradient">  the gradient to get the update for </param>
		''' <param name="iteration"> </param>
		''' <returns> the gradient </returns>
		Public Overridable Sub applyUpdater(ByVal gradient As INDArray, ByVal iteration As Integer, ByVal epoch As Integer) Implements GradientUpdater(Of AdaMax).applyUpdater
			If m Is Nothing OrElse u Is Nothing Then
				Throw New System.InvalidOperationException("Updater has not been initialized with view state")
			End If

			'm = B_1 * m + (1-B_1)*grad
			'u = max(B_2 * u, |grad|)

			Dim lr As Double = config.getLearningRate(iteration, epoch)
			Dim b1 As Double = config.getBeta1()
			Dim b2 As Double = config.getBeta2()
			Dim eps As Double = config.getEpsilon()

			Nd4j.exec(New org.nd4j.linalg.api.ops.impl.updaters.AdaMaxUpdater(gradient, u, m, lr, b1, b2, eps, iteration))
		End Sub
	End Class

End Namespace