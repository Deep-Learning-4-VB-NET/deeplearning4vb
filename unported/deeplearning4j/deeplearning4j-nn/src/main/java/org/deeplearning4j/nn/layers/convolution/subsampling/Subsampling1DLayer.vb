Imports System
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Broadcast = org.nd4j.linalg.factory.Broadcast
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr

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

Namespace org.deeplearning4j.nn.layers.convolution.subsampling


	<Serializable>
	Public Class Subsampling1DLayer
		Inherits SubsamplingLayer

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			If epsilon.rank() <> 3 Then
				Throw New DL4JInvalidInputException("Got rank " & epsilon.rank() & " array as epsilon for Subsampling1DLayer backprop with shape " & Arrays.toString(epsilon.shape()) & ". Expected rank 3 array with shape [minibatchSize, features, length]. " & layerId())
			End If
			If maskArray IsNot Nothing Then
				Dim maskOut As INDArray = feedForwardMaskArray(maskArray, MaskState.Active, CInt(epsilon.size(0))).First
				Preconditions.checkState(epsilon.size(0) = maskOut.size(0) AndAlso epsilon.size(2) = maskOut.size(1), "Activation gradients dimensions (0,2) and mask dimensions (0,1) don't match: Activation gradients %s, Mask %s", epsilon.shape(), maskOut.shape())
				Broadcast.mul(epsilon, maskOut, epsilon, 0, 2)
			End If

			' add singleton fourth dimension to input and next layer's epsilon
			Dim origInput As INDArray = input
			input = input.castTo(dataType).reshape(input.size(0), input.size(1), input.size(2), 1)
			epsilon = epsilon.reshape(ChrW(epsilon.size(0)), epsilon.size(1), epsilon.size(2), 1)

			' call 2D SubsamplingLayer's backpropGradient method
			Dim gradientEpsNext As Pair(Of Gradient, INDArray) = MyBase.backpropGradient(epsilon, workspaceMgr)
			Dim epsNext As INDArray = gradientEpsNext.Second

			' remove singleton fourth dimension from input and current epsilon
			input = origInput
			epsNext = epsNext.reshape(ChrW(epsNext.size(0)), epsNext.size(1), epsNext.size(2))

			Return New Pair(Of Gradient, INDArray)(gradientEpsNext.First, epsNext)
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			If input.rank() <> 3 Then
				Throw New DL4JInvalidInputException("Got rank " & input.rank() & " array as input to Subsampling1DLayer with shape " & Arrays.toString(input.shape()) & ". Expected rank 3 array with shape [minibatchSize, features, length]. " & layerId())
			End If

			' add singleton fourth dimension to input
			Dim origInput As INDArray = input
			input = input.castTo(dataType).reshape(input.size(0), input.size(1), input.size(2), 1)

			' call 2D SubsamplingLayer's activate method
			Dim acts As INDArray = MyBase.activate(training, workspaceMgr)

			' remove singleton fourth dimension from input and output activations
			input = origInput
			acts = acts.reshape(ChrW(acts.size(0)), acts.size(1), acts.size(2))

			If maskArray IsNot Nothing Then
				Dim maskOut As INDArray = feedForwardMaskArray(maskArray, MaskState.Active, CInt(acts.size(0))).First
				Preconditions.checkState(acts.size(0) = maskOut.size(0) AndAlso acts.size(2) = maskOut.size(1), "Activations dimensions (0,2) and mask dimensions (0,1) don't match: Activations %s, Mask %s", acts.shape(), maskOut.shape())
				Broadcast.mul(acts, maskOut, acts, 0, 2)
			End If

			Return acts
		End Function

		Public Overrides Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			Dim reduced As INDArray = ConvolutionUtils.cnn1dMaskReduction(maskArray, layerConf().getKernelSize()(0), layerConf().getStride()(0), layerConf().getPadding()(0), layerConf().getDilation()(0), layerConf().getConvolutionMode())
			Return New Pair(Of INDArray, MaskState)(reduced, currentMaskState)
		End Function
	End Class

End Namespace