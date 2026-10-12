Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports org.nd4j.common.primitives
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType

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

Namespace org.deeplearning4j.nn.layers.feedforward.embedding

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class EmbeddingLayer extends org.deeplearning4j.nn.layers.BaseLayer<org.deeplearning4j.nn.conf.layers.EmbeddingLayer>
	<Serializable>
	Public Class EmbeddingLayer
		Inherits BaseLayer(Of org.deeplearning4j.nn.conf.layers.EmbeddingLayer)

		Private Shared ReadOnly DIM_1() As Integer = {1}

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			'If this layer is layer L, then epsilon is (w^(L+1)*(d^(L+1))^T) (or equivalent)
			Dim z As INDArray = preOutput(True, workspaceMgr)
			Dim delta As INDArray = layerConf().getActivationFn().backprop(z, epsilon).getFirst() 'TODO handle activation function params

			If maskArray_Conflict IsNot Nothing Then
				delta.muliColumnVector(maskArray_Conflict.castTo(dataType))
			End If

			Dim weightGradients As INDArray = gradientViews(DefaultParamInitializer.WEIGHT_KEY)
			weightGradients.assign(0)

			Dim indexes(CInt(input_Conflict.length()) - 1) As Long
			For i As Integer = 0 To indexes.Length - 1
				indexes(i) = input_Conflict.getInt(i, 0)
			Next i

			Dim indices As INDArray = Nd4j.createFromArray(indexes)
			Nd4j.scatterUpdate(org.nd4j.linalg.api.ops.impl.scatter.ScatterUpdate.UpdateOp.ADD, weightGradients, indices, delta, DIM_1)


			Dim ret As Gradient = New DefaultGradient()
			ret.gradientForVariable()(DefaultParamInitializer.WEIGHT_KEY) = weightGradients

			If hasBias() Then
				Dim biasGradientsView As INDArray = gradientViews(DefaultParamInitializer.BIAS_KEY)
				delta.sum(biasGradientsView, 0) 'biasGradientView is initialized/zeroed first in sum op
				ret.gradientForVariable()(DefaultParamInitializer.BIAS_KEY) = biasGradientsView
			End If

			Return New Pair(Of Gradient, INDArray)(ret, Nothing) 'Don't bother returning epsilons: no layer below this one...
		End Function

		Protected Friend Overrides Function preOutput(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
			If input_Conflict.columns() <> 1 Then
				If input_Conflict.RowVector Then
					input_Conflict = input_Conflict.reshape(ChrW(input_Conflict.length()), 1)
				Else
					'Assume shape is [numExamples,1], and each entry is an integer index
					Throw New DL4JInvalidInputException("Cannot do forward pass for embedding layer with input more than one column. " & "Expected input shape: [numExamples,1] with each entry being an integer index " & layerId())
				End If
			End If

			Dim nIn As val = layerConf().getNIn()

			If input_Conflict.length() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Dim indexes(CInt(input_Conflict.length()) - 1) As Integer
			For i As Integer = 0 To indexes.Length - 1
				indexes(i) = input_Conflict.getInt(i, 0)

				If indexes(i) < 0 OrElse indexes(i) >= nIn Then
					Throw New DL4JInvalidInputException("Invalid index for embedding layer: got index " & indexes(i) & " for entry " & i & " in minibatch; indexes must be between 0 and nIn-1 inclusive (0 to " & (nIn -1) & ")")
				End If
			Next i

			Dim weights As INDArray = getParam(DefaultParamInitializer.WEIGHT_KEY)
			Dim bias As INDArray = getParam(DefaultParamInitializer.BIAS_KEY)

			Dim destination As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, weights.dataType(), input_Conflict.size(0), weights.size(1))
			Dim rows As INDArray = Nd4j.pullRows(weights, destination, 1, indexes)
			If hasBias() Then
				rows.addiRowVector(bias)
			End If

			Return rows
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim rows As INDArray = preOutput(training, workspaceMgr)

			Dim ret As INDArray = layerConf().getActivationFn().getActivation(rows, training)
			If maskArray_Conflict IsNot Nothing Then
				ret.muliColumnVector(maskArray_Conflict.castTo(dataType))
			End If
			Return ret
		End Function

		Public Overrides Function hasBias() As Boolean
			Return layerConf().hasBias()
		End Function

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Protected Friend Overrides Sub applyDropOutIfNecessary(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr)
			Throw New System.NotSupportedException("Dropout not supported with EmbeddingLayer " & layerId())
		End Sub

	End Class

End Namespace