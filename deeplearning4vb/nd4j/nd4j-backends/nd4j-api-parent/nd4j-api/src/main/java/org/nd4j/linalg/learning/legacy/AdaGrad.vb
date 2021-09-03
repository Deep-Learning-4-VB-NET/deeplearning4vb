Imports System
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
import static org.nd4j.linalg.ops.transforms.Transforms.sqrt

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

Namespace org.nd4j.linalg.learning.legacy



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor public class AdaGrad implements java.io.Serializable
	<Serializable>
	Public Class AdaGrad
		Public Const DEFAULT_ADAGRAD_EPSILON As Double = 1e-6

		Public historicalGradient As INDArray
		Public shape() As Long
		Protected Friend learningRate As Double = 1e-1 ' learning rate
		Protected Friend numIterations As Integer = 0
		Private epsilon As Double = DEFAULT_ADAGRAD_EPSILON

		Private gradientReshapeOrder As Char


		Public Overridable Function stateSizeForInputSize(ByVal inputSize As Integer) As Integer
			Return inputSize
		End Function

		Public Overridable Sub setStateViewArray(ByVal viewArray As INDArray, ByVal gradientShape() As Integer, ByVal gradientOrder As Char, ByVal initialize As Boolean)
			setStateViewArray(viewArray, ArrayUtil.toLongArray(gradientShape), gradientOrder, initialize)
		End Sub

		Public Overridable Sub setStateViewArray(ByVal viewArray As INDArray, ByVal gradientShape() As Long, ByVal gradientOrder As Char, ByVal initialize As Boolean)
			If Not viewArray.RowVector AndAlso Not (viewArray.rank() = 2 AndAlso viewArray.columns() = 1 AndAlso viewArray.rows() = 1) Then
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

		''' <param name="rows"> </param>
		''' <param name="cols"> </param>
		''' <param name="learningRate"> </param>
		Public Sub New(ByVal rows As Integer, ByVal cols As Integer, ByVal learningRate As Double)
			Me.shape = New Long() {rows, cols}
			Me.learningRate = learningRate
		End Sub

		Public Sub New(ByVal rows As Integer, ByVal cols As Integer)
			Me.New(rows, cols, 0.1)
		End Sub

		Public Sub New(ByVal shape() As Long, ByVal learningRate As Double)
			Me.shape = shape
			Me.learningRate = learningRate
		End Sub

		Public Sub New(ByVal learningRate As Double)
			Me.learningRate = learningRate
		End Sub

		Public Sub New(ByVal learningRate As Double, ByVal epsilon As Double)
			Me.learningRate = learningRate
			Me.epsilon = epsilon
		End Sub

		Public Overridable Sub update(ParamArray ByVal args() As Object)
			If args.Length > 0 Then
				learningRate = DirectCast(args(0), Double?)
			End If
		End Sub

		''' <summary>
		''' Gets feature specific learning rates
		''' Adagrad keeps a history of gradients being passed in.
		''' Note that each gradient passed in becomes adapted over time, hence
		''' the opName adagrad
		''' </summary>
		''' <param name="gradient">  the gradient to get learning rates for </param>
		''' <param name="iteration"> </param>
		''' <returns> the feature specific learning rates </returns>
		Public Overridable Function getGradient(ByVal gradient As INDArray, ByVal iteration As Integer) As INDArray
			If historicalGradient Is Nothing Then
				Throw New System.InvalidOperationException("Updater has not been initialized with view state")
			End If

			historicalGradient.addi(gradient.mul(gradient))

			Dim sqrtHistory As INDArray = sqrt(historicalGradient.dup(gradientReshapeOrder), False).addi(epsilon)
			' lr * gradient / (sqrt(sumSquaredGradients) + epsilon)
			Dim ret As INDArray = gradient.muli(sqrtHistory.rdivi(learningRate))
			numIterations += 1
			Return ret
		End Function

		Public Overridable Function getGradient(ByVal gradient As Double, ByVal column As Integer, ByVal shape() As Long) As Double
			Dim historicalInitialized As Boolean = False
			If Me.historicalGradient Is Nothing Then
				Me.historicalGradient = Nd4j.ones(shape)
				historicalInitialized = True
			End If

			Dim sqrtHistory As Double = If(Not historicalInitialized, Math.Sqrt(historicalGradient.getDouble(column)), historicalGradient.getDouble(column))
			Dim learningRates As Double = learningRate / (sqrtHistory + epsilon)
			Dim adjustedGradient As Double = gradient * (learningRates)

			historicalGradient.putScalar(column, historicalGradient.getDouble(column) + gradient * gradient)
			numIterations += 1

			'ensure no zeros
			Return adjustedGradient
		End Function

		Public Overridable Function getGradient(ByVal gradient As INDArray, ByVal slice As Integer, ByVal shape() As Long) As INDArray
			Dim historicalInitialized As Boolean = False
			Dim sqrtHistory As INDArray

			If Me.historicalGradient Is Nothing Then
				Me.historicalGradient = Nd4j.zeros(shape).add(epsilon)
				historicalInitialized = True
			ElseIf Not Me.historicalGradient.Vector AndAlso Me.historicalGradient.slice(slice).length() <> gradient.length() Then
				Throw New System.ArgumentException("Illegal gradient")
			End If

			If historicalGradient.Vector Then
				sqrtHistory = sqrt(historicalGradient)
			Else
				sqrtHistory = If(Not historicalInitialized, sqrt(historicalGradient.slice(slice)), historicalGradient)
			End If
			Dim learningRates As INDArray
			Try
				learningRates = sqrtHistory.rdivi(learningRate)
			Catch ae As ArithmeticException
				learningRates = sqrtHistory.rdivi(learningRate + epsilon)
			End Try
			If gradient.length() <> learningRates.length() Then
				gradient.muli(learningRates.slice(slice))
			Else
				gradient.muli(learningRates)
			End If

			Me.historicalGradient.slice(slice).addi(gradient.mul(gradient))
			numIterations += 1

			'ensure no zeros
			Return gradient
		End Function

		Public Overridable Function createSubset(ByVal index As Integer) As AdaGrad
			If historicalGradient Is Nothing Then
				Me.historicalGradient = Nd4j.ones(shape)
			End If

			If Shape.isMatrix(shape) Then
				Dim a As New AdaGrad(1, historicalGradient.columns())
				'grab only the needed elements
				Dim slice As INDArray = historicalGradient.slice(index).dup()
				a.historicalGradient = slice
				a.setLearningRate(learningRate)
				Return a
			Else
				Dim a As New AdaGrad(1, 1)
				'grab only the needed elements
				Dim slice As INDArray = Nd4j.scalar(historicalGradient.getDouble(index))
				a.historicalGradient = slice
				a.setLearningRate(learningRate)
				Return a
			End If
		End Function
	End Class

End Namespace