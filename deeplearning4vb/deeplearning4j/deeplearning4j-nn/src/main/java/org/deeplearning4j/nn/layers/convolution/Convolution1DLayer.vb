Imports System
Imports System.Collections.Generic
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports Convolution1D = org.deeplearning4j.nn.conf.layers.Convolution1D
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ConvolutionParamInitializer = org.deeplearning4j.nn.params.ConvolutionParamInitializer
Imports Convolution1DUtils = org.deeplearning4j.util.Convolution1DUtils
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Conv1D = org.nd4j.linalg.api.ops.impl.layers.convolution.Conv1D
Imports Conv1DDerivative = org.nd4j.linalg.api.ops.impl.layers.convolution.Conv1DDerivative
Imports Conv1DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv1DConfig
Imports PaddingMode = org.nd4j.linalg.api.ops.impl.layers.convolution.config.PaddingMode
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Broadcast = org.nd4j.linalg.factory.Broadcast
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
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

Namespace org.deeplearning4j.nn.layers.convolution


	<Serializable>
	Public Class Convolution1DLayer
		Inherits ConvolutionLayer

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub


		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			If epsilon.rank() <> 3 Then
				Throw New DL4JInvalidInputException("Got rank " & epsilon.rank() & " array as epsilon for Convolution1DLayer backprop with shape " & Arrays.toString(epsilon.shape()) & ". Expected rank 3 array with shape [minibatchSize, features, length]. " & layerId())
			End If
			Dim fwd As Pair(Of INDArray, INDArray) = preOutput(False,True,workspaceMgr)
			Dim afn As IActivation = layerConf().getActivationFn()
			Dim delta As INDArray = afn.backprop(fwd.First, epsilon).First 'TODO handle activation function params

			Dim c As org.deeplearning4j.nn.conf.layers.Convolution1DLayer = layerConf()
			Dim conf As Conv1DConfig = Conv1DConfig.builder().k(c.getKernelSize()(0)).s(c.getStride()(0)).d(c.getDilation()(0)).p(c.getPadding()(0)).dataFormat(Conv1DConfig.NCW).paddingMode(ConvolutionUtils.paddingModeForConvolutionMode(convolutionMode)).build()

			Dim w As INDArray = Convolution1DUtils.reshapeWeightArrayOrGradientForFormat(getParam(ConvolutionParamInitializer.WEIGHT_KEY), RNNFormat.NCW)

			Dim inputArrs() As INDArray
			Dim outputArrs() As INDArray
			Dim wg As INDArray = Convolution1DUtils.reshapeWeightArrayOrGradientForFormat(gradientViews.get(ConvolutionParamInitializer.WEIGHT_KEY), RnnDataFormat)
			Dim epsOut As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, input.dataType(), input.shape())
			Dim input As INDArray = Me.input.castTo(dataType)
			If layerConf().getRnnDataFormat() = RNNFormat.NWC Then
				input = input.permute(0,2,1) 'NHWC to NCHW
			End If

			If layerConf().hasBias() Then
				Dim b As INDArray = getParam(ConvolutionParamInitializer.BIAS_KEY)
				b = b.reshape(ChrW(b.length()))
				inputArrs = New INDArray(){input, w, b, delta}
				Dim bg As INDArray = gradientViews.get(ConvolutionParamInitializer.BIAS_KEY)
				bg = bg.reshape(ChrW(bg.length()))
				outputArrs = New INDArray(){epsOut, wg, bg}
			Else
				inputArrs = New INDArray(){input, w, delta}
				outputArrs = New INDArray(){epsOut, wg}
			End If

			Dim op As New Conv1DDerivative(inputArrs, outputArrs, conf)
			Nd4j.exec(op)

			Dim retGradient As Gradient = New DefaultGradient()
			If layerConf().hasBias() Then
				retGradient.setGradientFor(ConvolutionParamInitializer.BIAS_KEY, gradientViews.get(ConvolutionParamInitializer.BIAS_KEY))
			End If
			retGradient.setGradientFor(ConvolutionParamInitializer.WEIGHT_KEY, gradientViews.get(ConvolutionParamInitializer.WEIGHT_KEY), "c"c)
			If RnnDataFormat = RNNFormat.NWC Then
				epsOut = epsOut.permute(0, 2, 1)
			End If
			Return New Pair(Of Gradient, INDArray)(retGradient, epsOut)
		End Function

		Protected Friend Overrides Function preOutput4d(ByVal training As Boolean, ByVal forBackprop As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of INDArray, INDArray)
			Dim preOutput As Pair(Of INDArray, INDArray) = MyBase.preOutput(True, forBackprop, workspaceMgr)
			Dim p3d As INDArray = preOutput.First
			Dim p As INDArray = preOutput.First.reshape(ChrW(p3d.size(0)), p3d.size(1), p3d.size(2), 1)
			preOutput.First = p
			Return preOutput
		End Function

		Protected Friend Overrides Function preOutput(ByVal training As Boolean, ByVal forBackprop As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of INDArray, INDArray)
			assertInputSet(False)

			Dim input As INDArray = Me.input.castTo(dataType)
			If layerConf().getRnnDataFormat() = RNNFormat.NWC Then
				If input.rank() = 3 Then
					input = input.permute(0,2,1) 'NHWC to NCHW
				ElseIf input.rank() = 4 Then
					input = input.permute(0,2,3,1) 'NHWC to NCHW

				End If
			End If

			Dim c As org.deeplearning4j.nn.conf.layers.Convolution1DLayer = layerConf()
			Dim conf As Conv1DConfig = Conv1DConfig.builder().k(c.getKernelSize()(0)).s(c.getStride()(0)).d(c.getDilation()(0)).p(c.getPadding()(0)).dataFormat(Conv1DConfig.NCW).paddingMode(ConvolutionUtils.paddingModeForConvolutionMode(convolutionMode)).build()


			Dim w As INDArray = Convolution1DUtils.reshapeWeightArrayOrGradientForFormat(getParam(ConvolutionParamInitializer.WEIGHT_KEY),RNNFormat.NCW)


			Dim inputs() As INDArray
			If layerConf().hasBias() Then
				Dim b As INDArray = getParam(ConvolutionParamInitializer.BIAS_KEY)
				b = b.reshape(ChrW(b.length()))
				inputs = New INDArray(){input, w, b}
			Else
				inputs = New INDArray(){input, w}
			End If

			Dim op As New Conv1D(inputs, Nothing, conf)
			Dim outShape As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			op.setOutputArgument(0, Nd4j.create(outShape(0), False))
			Nd4j.exec(op)
			Dim output As INDArray = op.getOutputArgument(0)

			If RnnDataFormat = RNNFormat.NWC Then
				output = output.permute(0,2,1)
			End If

			Return New Pair(Of INDArray, INDArray)(output, Nothing)
		End Function


		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim act4d As INDArray = MyBase.activate(training, workspaceMgr)
			Dim act3d As INDArray = If(act4d.rank() > 3, act4d.reshape(ChrW(act4d.size(0)), act4d.size(1), act4d.size(2)), act4d)

			If maskArray IsNot Nothing Then
				Dim maskOut As INDArray = feedForwardMaskArray(maskArray, MaskState.Active, CInt(act3d.size(0))).First
				Preconditions.checkState(act3d.size(0) = maskOut.size(0) AndAlso act3d.size(2) = maskOut.size(1), "Activations dimensions (0,2) and mask dimensions (0,1) don't match: Activations %s, Mask %s", act3d.shape(), maskOut.shape())
				Broadcast.mul(act3d, maskOut, act3d, 0, 2)
			End If

			Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, act3d) 'Should be zero copy most of the time
		End Function

		Public Overrides Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			Dim reduced As INDArray = ConvolutionUtils.cnn1dMaskReduction(maskArray, layerConf().getKernelSize()(0), layerConf().getStride()(0), layerConf().getPadding()(0), layerConf().getDilation()(0), layerConf().getConvolutionMode())
			Return New Pair(Of INDArray, MaskState)(reduced, currentMaskState)
		End Function

		Public Overrides Function layerConf() As org.deeplearning4j.nn.conf.layers.Convolution1DLayer
			Return CType(conf().getLayer(), org.deeplearning4j.nn.conf.layers.Convolution1DLayer)
		End Function

		Private ReadOnly Property RnnDataFormat As RNNFormat
			Get
				Return layerConf().getRnnDataFormat()
			End Get
		End Property
	End Class

End Namespace