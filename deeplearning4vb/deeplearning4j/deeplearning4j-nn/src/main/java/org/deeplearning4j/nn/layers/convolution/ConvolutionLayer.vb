Imports System
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports CacheMode = org.deeplearning4j.nn.conf.CacheMode
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports HelperUtils = org.deeplearning4j.nn.layers.HelperUtils
Imports LayerHelper = org.deeplearning4j.nn.layers.LayerHelper
Imports MKLDNNConvHelper = org.deeplearning4j.nn.layers.mkldnn.MKLDNNConvHelper
Imports ConvolutionParamInitializer = org.deeplearning4j.nn.params.ConvolutionParamInitializer
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Convolution = org.nd4j.linalg.convolution.Convolution
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports ND4JOpProfilerException = org.nd4j.linalg.exception.ND4JOpProfilerException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
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

Namespace org.deeplearning4j.nn.layers.convolution




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ConvolutionLayer extends org.deeplearning4j.nn.layers.BaseLayer<org.deeplearning4j.nn.conf.layers.ConvolutionLayer>
	<Serializable>
	Public Class ConvolutionLayer
		Inherits BaseLayer(Of org.deeplearning4j.nn.conf.layers.ConvolutionLayer)

		Protected Friend i2d As INDArray
'JAVA TO VB CONVERTER NOTE: The field helper was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend helper_Conflict As ConvolutionHelper = Nothing
		Protected Friend helperCountFail As Integer = 0
		Protected Friend convolutionMode As ConvolutionMode
		<NonSerialized>
		Protected Friend dummyBias As INDArray 'Used only when: hasBias == false AND helpers are used
		<NonSerialized>
		Protected Friend dummyBiasGrad As INDArray 'As above
		Public Const CUDA_CNN_HELPER_CLASS_NAME As String = "org.deeplearning4j.cuda.convolution.CudnnConvolutionHelper"
		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
			initializeHelper()
			convolutionMode = CType(Me.conf().getLayer(), org.deeplearning4j.nn.conf.layers.ConvolutionLayer).getConvolutionMode()
		End Sub

		Friend Overridable Sub initializeHelper()
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			helper_Conflict = HelperUtils.createHelper(CUDA_CNN_HELPER_CLASS_NAME, GetType(MKLDNNConvHelper).FullName, GetType(ConvolutionHelper), layerConf().LayerName, dataType)
		End Sub

		Public Overrides Function type() As Type
			Return Type.CONVOLUTIONAL
		End Function

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			Dim weights As INDArray = getParamWithNoise(ConvolutionParamInitializer.WEIGHT_KEY, True, workspaceMgr)
			Dim bias As INDArray = getParamWithNoise(ConvolutionParamInitializer.BIAS_KEY, True, workspaceMgr)

			Dim input As INDArray = Me.input_Conflict.castTo(dataType) 'No op if correct type
			If epsilon.dataType() <> dataType Then
				epsilon = epsilon.castTo(dataType)
			End If

			Dim origInput As INDArray = input
			Dim origEps As INDArray = epsilon
			If layerConf().getCnn2dDataFormat() <> CNN2DFormat.NCHW Then
				input = input.permute(0,3,1,2) 'NHWC to NCHW
				epsilon = epsilon.permute(0,3,1,2) 'NHWC to NCHW
			End If


			Dim miniBatch As Long = input.size(0)
			Dim inH As Integer = CInt(input.size(2))
			Dim inW As Integer = CInt(input.size(3))

			Dim outDepth As Long = weights.size(0)
			Dim inDepth As Long = weights.size(1)
			Dim kH As Integer = CInt(weights.size(2))
			Dim kW As Integer = CInt(weights.size(3))

			Dim dilation() As Integer = layerConf().getDilation()
			Dim kernel() As Integer = layerConf().getKernelSize()
			Dim strides() As Integer = layerConf().getStride()
			Dim pad() As Integer
			Dim outSize() As Integer
			If convolutionMode = ConvolutionMode.Same Then
				outSize = ConvolutionUtils.getOutputSize(input, kernel, strides, Nothing, convolutionMode, dilation, CNN2DFormat.NCHW) 'Also performs validation
				pad = ConvolutionUtils.getSameModeTopLeftPadding(outSize, New Integer() {inH, inW}, kernel, strides, dilation)
			Else
				pad = layerConf().getPadding()
				outSize = ConvolutionUtils.getOutputSize(input, kernel, strides, pad, convolutionMode, dilation, CNN2DFormat.NCHW) 'Also performs validation
			End If

			Dim outH As Integer = outSize(0)
			Dim outW As Integer = outSize(1)


			Dim biasGradView As INDArray = gradientViews(ConvolutionParamInitializer.BIAS_KEY)
			Dim weightGradView As INDArray = gradientViews(ConvolutionParamInitializer.WEIGHT_KEY) '4d, c order. Shape: [outDepth,inDepth,kH,kW]
			Dim weightGradView2df As INDArray = Shape.newShapeNoCopy(weightGradView, New Long(){outDepth, inDepth * kH * kW}, False).transpose()



			Dim delta As INDArray
			Dim afn As IActivation = layerConf().getActivationFn()

			Dim p As Pair(Of INDArray, INDArray) = preOutput4d(True, True, workspaceMgr)
			Dim z As INDArray = p.First
			Dim f As CNN2DFormat = layerConf().getCnn2dDataFormat()
			If f <> CNN2DFormat.NCHW Then
				z = z.permute(0,3,1,2) 'NHWC to NCHW
			End If
			delta = afn.backprop(z, epsilon).First 'TODO handle activation function params

			If helper_Conflict IsNot Nothing AndAlso (helperCountFail = 0 OrElse Not layerConf().isCudnnAllowFallback()) Then
				Dim helperDelta As INDArray = delta
				If layerConf().getCnn2dDataFormat() = CNN2DFormat.NHWC Then
					helperDelta = delta.permute(0,2,3,1) 'NCHW to NHWC
				End If

				If Not hasBias() AndAlso Not (TypeOf helper_Conflict Is MKLDNNConvHelper) Then
					'MKL-DNN supports no bias, CuDNN doesn't
					If dummyBiasGrad Is Nothing Then
						Using wsO As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
							dummyBiasGrad = Nd4j.create(1, layerConf().getNOut())
						End Using
					End If
					biasGradView = dummyBiasGrad
				End If

				Dim ret As Pair(Of Gradient, INDArray) = Nothing
				Try
					ret = helper_Conflict.backpropGradient(origInput, weights, bias, helperDelta, kernel, strides, pad, biasGradView, weightGradView, afn, layerConf().getCudnnAlgoMode(), layerConf().getCudnnBwdFilterAlgo(), layerConf().getCudnnBwdDataAlgo(), convolutionMode, dilation, layerConf().getCnn2dDataFormat(), workspaceMgr)
				Catch e As ND4JOpProfilerException
					Throw e 'NaN panic etc for debugging
				Catch e As Exception
					If e.Message.contains("Failed to allocate") Then
						'This is a memory exception - don't fallback to built-in implementation
						Throw e
					End If

					If layerConf().isCudnnAllowFallback() Then
						helperCountFail += 1
						If TypeOf helper_Conflict Is MKLDNNConvHelper Then
							log.warn("MKL-DNN execution failed - falling back on built-in implementation",e)
						Else
							log.warn("CuDNN execution failed - falling back on built-in implementation",e)
						End If
					Else
						Throw New Exception("Error during ConvolutionLayer MKL/CuDNN helper backprop - isCudnnAllowFallback() is set to false", e)
					End If
				End Try

				If ret IsNot Nothing Then
					'Backprop dropout, if present
					Dim gradPostDropout As INDArray = ret.Right
					gradPostDropout = backpropDropOutIfPresent(gradPostDropout)
					ret.Second = gradPostDropout
					Return ret
				End If
			End If

			delta = delta.permute(1, 0, 2, 3) 'To shape: [outDepth,miniBatch,outH,outW]

			'Note: due to the permute in preOut, and the fact that we essentially do a preOut.muli(epsilon), this reshape
			' should be zero-copy; only possible exception being sometimes with the "identity" activation case
			Dim delta2d As INDArray = delta.reshape("c"c, New Long() {outDepth, miniBatch * outH * outW}) 'Shape.newShapeNoCopy(delta,new int[]{outDepth,miniBatch*outH*outW},false);

			'Do im2col, but with order [miniB,outH,outW,depthIn,kH,kW]; but need to input [miniBatch,channels,kH,kW,outH,outW] given the current im2col implementation
			'To get this: create an array of the order we want, permute it to the order required by im2col implementation, and then do im2col on that
			'to get old order from required order: permute(0,3,4,5,1,2)
			Dim im2col2d As INDArray = p.Second 'Re-use im2col2d array from forward pass if available; recalculate if not
			If im2col2d Is Nothing Then
				Dim col As INDArray = Nd4j.createUninitialized(dataType, New Long() {miniBatch, outH, outW, inDepth, kH, kW}, "c"c)
				Dim col2 As INDArray = col.permute(0, 3, 4, 5, 1, 2)
				Convolution.im2col(input, kH, kW, strides(0), strides(1), pad(0), pad(1), dilation(0), dilation(1), convolutionMode = ConvolutionMode.Same, col2)
				'Shape im2col to 2d. Due to the permuting above, this should be a zero-copy reshape
				im2col2d = col.reshape("c"c, miniBatch * outH * outW, inDepth * kH * kW)
			End If

			'Calculate weight gradients, using cc->c mmul.
			'weightGradView2df is f order, but this is because it's transposed from c order
			'Here, we are using the fact that AB = (B^T A^T)^T; output here (post transpose) is in c order, not usual f order
			Nd4j.gemm(im2col2d, delta2d, weightGradView2df, True, True, 1.0, 0.0)

			'Flatten 4d weights to 2d... this again is a zero-copy op (unless weights are not originally in c order for some reason)
			Dim wPermuted As INDArray = weights.permute(3, 2, 1, 0) 'Start with c order weights, switch order to f order
			Dim w2d As INDArray = wPermuted.reshape("f"c, inDepth * kH * kW, outDepth)

			'Calculate epsilons for layer below, in 2d format (note: this is in 'image patch' format before col2im reduction)
			'Note: cc -> f mmul here, then reshape to 6d in f order
			Dim epsNext2d As INDArray = w2d.mmul(delta2d) 'TODO can we reuse im2col array instead of allocating new result array?
			Dim eps6d As INDArray = Shape.newShapeNoCopy(epsNext2d, New Long() {kW, kH, inDepth, outW, outH, miniBatch}, True)

			'Calculate epsilonNext by doing im2col reduction.
			'Current col2im implementation expects input with order: [miniBatch,channels,kH,kW,outH,outW]
			'currently have [kH,kW,inDepth,outW,outH,miniBatch] -> permute first
			eps6d = eps6d.permute(5, 2, 1, 0, 4, 3)
			Dim epsNextOrig As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, eps6d.dataType(), New Long() {inDepth, miniBatch, inH, inW}, "c"c)

			'Note: we are execute col2im in a way that the output array should be used in a stride 1 muli in the layer below... (same strides as zs/activations)
			Dim epsNext As INDArray = epsNextOrig.permute(1, 0, 2, 3)
			Convolution.col2im(eps6d, epsNext, strides(0), strides(1), pad(0), pad(1), inH, inW, dilation(0), dilation(1))

			Dim retGradient As Gradient = New DefaultGradient()
			If layerConf().hasBias() Then
				delta2d.sum(biasGradView, 1) 'biasGradView is initialized/zeroed first in sum op
				retGradient.setGradientFor(ConvolutionParamInitializer.BIAS_KEY, biasGradView)
			End If
			retGradient.setGradientFor(ConvolutionParamInitializer.WEIGHT_KEY, weightGradView, "c"c)

			weightNoiseParams.Clear()

			epsNext = backpropDropOutIfPresent(epsNext)

			If layerConf().getCnn2dDataFormat() <> CNN2DFormat.NCHW Then
				epsNext = epsNext.permute(0,2,3,1) 'NCHW to NHWC
			End If

			Return New Pair(Of Gradient, INDArray)(retGradient, epsNext)
		End Function

		''' <summary>
		''' preOutput4d: Used so that ConvolutionLayer subclasses (such as Convolution1DLayer) can maintain their standard
		''' non-4d preOutput method, while overriding this to return 4d activations (for use in backprop) without modifying
		''' the public API
		''' </summary>
		Protected Friend Overridable Function preOutput4d(ByVal training As Boolean, ByVal forBackprop As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of INDArray, INDArray)
			Return preOutput(training, forBackprop, workspaceMgr)
		End Function

		Protected Friend Overridable Sub validateInputRank()
			'Input validation: expect rank 4 matrix
			If input_Conflict.rank() <> 4 Then
				Dim layerName As String = conf_Conflict.getLayer().getLayerName()
				If layerName Is Nothing Then
					layerName = "(not named)"
				End If
				Throw New DL4JInvalidInputException("Got rank " & input_Conflict.rank() & " array as input to ConvolutionLayer (layer name = " & layerName & ", layer index = " & index_Conflict & ") with shape " & Arrays.toString(input_Conflict.shape()) & ". " & "Expected rank 4 array with shape [minibatchSize, layerInputDepth, inputHeight, inputWidth]." & (If(input_Conflict.rank() = 2, " (Wrong input type (see InputType.convolutionalFlat()) or wrong data type?)", "")) & " " & layerId())
			End If
		End Sub

		Protected Friend Overridable Sub validateInputDepth(ByVal inDepth As Long)
			Dim format As CNN2DFormat = layerConf().getCnn2dDataFormat()
			Dim [dim] As Integer = If(format = CNN2DFormat.NHWC, 3, 1)
			If input_Conflict.size([dim]) <> inDepth Then
				Dim layerName As String = conf_Conflict.getLayer().getLayerName()
				If layerName Is Nothing Then
					layerName = "(not named)"
				End If

				Dim s As String = "Cannot do forward pass in Convolution layer (layer name = " & layerName & ", layer index = " & index_Conflict & "): input array channels does not match CNN layer configuration" & " (data format = " & format & ", data input channels = " & input_Conflict.size([dim]) & ", " & layerConf().getCnn2dDataFormat().dimensionNames() & "=" & Arrays.toString(input_Conflict.shape()) & "; expected" & " input channels = " & inDepth & ") " & layerId()

				Dim dimIfWrongFormat As Integer = If(format = CNN2DFormat.NHWC, 1, 3)
				If input_Conflict.size(dimIfWrongFormat) = inDepth Then
					'User might have passed NCHW data to a NHWC net, or vice versa?
					s &= vbLf & ConvolutionUtils.NCHW_NHWC_ERROR_MSG
				End If


				Throw New DL4JInvalidInputException(s)
			End If
		End Sub

		''' <summary>
		''' PreOutput method that also returns the im2col2d array (if being called for backprop), as this can be re-used
		''' instead of being calculated again.
		''' </summary>
		''' <param name="training">    Train or test time (impacts dropout) </param>
		''' <param name="forBackprop"> If true: return the im2col2d array for re-use during backprop. False: return null for second
		'''                    pair entry. Note that it may still be null in the case of CuDNN and the like. </param>
		''' <returns>            Pair of arrays: preOutput (activations) and optionally the im2col2d array </returns>
		Protected Friend Overridable Overloads Function preOutput(ByVal training As Boolean, ByVal forBackprop As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of INDArray, INDArray)
			assertInputSet(False)
			Dim bias As INDArray = getParamWithNoise(ConvolutionParamInitializer.BIAS_KEY, training, workspaceMgr)
			Dim weights As INDArray = getParamWithNoise(ConvolutionParamInitializer.WEIGHT_KEY, training, workspaceMgr)

			validateInputRank()

			Dim input As INDArray = Me.input_Conflict.castTo(dataType)
			Dim inputOrig As INDArray = input
			If layerConf().getCnn2dDataFormat() = CNN2DFormat.NHWC Then
				input = input.permute(0,3,1,2).dup() 'NHWC to NCHW
			End If

			Dim miniBatch As Long = input.size(0)
			Dim outDepth As Long = weights.size(0)
			Dim inDepth As Long = weights.size(1)
			validateInputDepth(inDepth)

			Dim kH As Long = weights.size(2)
			Dim kW As Long = weights.size(3)


			Dim dilation() As Integer = layerConf().getDilation()
			Dim kernel() As Integer = layerConf().getKernelSize()
			Dim strides() As Integer = layerConf().getStride()



			Dim pad() As Integer
			Dim outSize() As Integer
			If convolutionMode = ConvolutionMode.Same Then
				outSize = ConvolutionUtils.getOutputSize(input, kernel, strides, Nothing, convolutionMode, dilation, CNN2DFormat.NCHW) 'Note: hardcoded to NCHW due to permute earlier in this method

				If input.size(2) > Integer.MaxValue OrElse input.size(3) > Integer.MaxValue Then
					Throw New ND4JArraySizeException()
				End If
				Dim inWidthHeight() As Integer
				'  if(layerConf().getCnn2dDataFormat() == CNN2DFormat.NCHW)
				'TODO: Switch hardcoded state later. For now, convolution is implemented as
				'switch to NCHW then permute back for NWHC
				inWidthHeight = New Integer() {CInt(input.size(2)), CInt(input.size(3))}

	'            else if(layerConf().getCnn2dDataFormat() == CNN2DFormat.NHWC) {
	'                inWidthHeight =  new int[] {(int) input.size(1), (int) input.size(2)};
	'            }
	'            else
	'                 throw new IllegalStateException("No data format configured!");
				pad = ConvolutionUtils.getSameModeTopLeftPadding(outSize, inWidthHeight, kernel, strides, dilation)
			Else
				pad = layerConf().getPadding()
				outSize = ConvolutionUtils.getOutputSize(input, kernel, strides, pad, convolutionMode, dilation, CNN2DFormat.NCHW) 'Note: hardcoded to NCHW due to permute earlier in this method
			End If

			Dim outH As Integer = outSize(0)
			Dim outW As Integer = outSize(1)


			If helper_Conflict IsNot Nothing AndAlso (helperCountFail = 0 OrElse Not layerConf().isCudnnAllowFallback()) Then
				If preOutput IsNot Nothing AndAlso forBackprop Then
					Return New Pair(Of INDArray, INDArray)(preOutput, Nothing)
				End If

				'For no-bias convolutional layers: use an empty (all 0s) value for biases
				If Not hasBias() Then
					If dummyBias Is Nothing Then
						Using wsO As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
							dummyBias = Nd4j.create(1, layerConf().getNOut())
						End Using
					End If
					bias = dummyBias
				End If

				Dim ret As INDArray = Nothing
				Try
					ret = helper_Conflict.preOutput(inputOrig, weights, bias, kernel, strides, pad, layerConf().getCudnnAlgoMode(), layerConf().getCudnnFwdAlgo(), convolutionMode, dilation, layerConf().getCnn2dDataFormat(), workspaceMgr)
				Catch e As ND4JOpProfilerException
					Throw e 'NaN panic etc for debugging
				Catch e As Exception
					If e.Message IsNot Nothing AndAlso e.Message.contains("Failed to allocate") Then
						'This is a memory exception - don't fallback to built-in implementation
						Throw e
					End If

					If layerConf().isCudnnAllowFallback() Then
						helperCountFail += 1
						If TypeOf helper_Conflict Is MKLDNNConvHelper Then
							log.warn("MKL-DNN execution failed - falling back on built-in implementation",e)
						Else
							log.warn("CuDNN execution failed - falling back on built-in implementation",e)
						End If
					Else
						Throw New Exception("Error during ConvolutionLayer MKL/CuDNN helper forward pass - isCudnnAllowFallback() is set to false", e)
					End If
				End Try
				If ret IsNot Nothing Then
					Return New Pair(Of INDArray, INDArray)(ret, Nothing)
				End If
			End If

			If preOutput IsNot Nothing AndAlso i2d IsNot Nothing AndAlso forBackprop Then
				Return New Pair(Of INDArray, INDArray)(preOutput, i2d)
			End If

			'im2col in the required order: want [outW,outH,miniBatch,depthIn,kH,kW], but need to input [miniBatch,channels,kH,kW,outH,outW] given the current im2col implementation
			'To get this: create an array of the order we want, permute it to the order required by im2col implementation, and then do im2col on that
			'to get old order from required order: permute(0,3,4,5,1,2)
			'Post reshaping: rows are such that minibatch varies slowest, outW fastest as we step through the rows post-reshape
			Dim col As INDArray = Nd4j.createUninitialized(weights.dataType(), New Long() {miniBatch, outH, outW, inDepth, kH, kW}, "c"c)
			Dim permute() As Integer = {0, 3, 4, 5, 1, 2}
			Dim col2 As INDArray = col.permute(permute)
			Dim im2ColIn As INDArray = input.castTo(col2.dataType()) 'No op if already (for example) float
			If kH > Integer.MaxValue OrElse kW > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Convolution.im2col(im2ColIn, CInt(kH), CInt(kW), strides(0), strides(1), pad(0), pad(1), dilation(0), dilation(1), convolutionMode = ConvolutionMode.Same, col2)


			Dim im2col2d As INDArray = Shape.newShapeNoCopy(col, New Long() {miniBatch * outH * outW, inDepth * kH * kW}, False)

			'Current order of weights: [depthOut,depthIn,kH,kW], c order
			'Permute to give [kW,kH,depthIn,depthOut], f order
			'Reshape to give [kW*kH*depthIn, depthOut]. This should always be zero-copy reshape, unless weights aren't in c order for some reason
			Dim permutedW As INDArray = weights.permute(3, 2, 1, 0)
			Dim reshapedW As INDArray = permutedW.reshape("f"c, kW * kH * inDepth, outDepth)

			'Do the MMUL; c and f orders in, f order out. output shape: [miniBatch*outH*outW,depthOut]
			Dim z As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, weights.dataType(), New Long(){im2col2d.size(0), reshapedW.size(1)}, "f"c)
			im2col2d.mmuli(reshapedW, z)

			'Add biases, before reshaping. Note that biases are [1,depthOut] and currently z is [miniBatch*outH*outW,depthOut] -> addiRowVector
			If layerConf().hasBias() Then
				z.addiRowVector(bias)
			End If

			'Now, reshape to [outW,outH,miniBatch,outDepth], and permute to have correct output order: [miniBatch,outDepth,outH,outW];
			z = Shape.newShapeNoCopy(z, New Long() {outW, outH, miniBatch, outDepth}, True)
			z = z.permute(2, 3, 1, 0)

			If training AndAlso cacheMode_Conflict <> CacheMode.NONE AndAlso workspaceMgr.hasConfiguration(ArrayType.FF_CACHE) AndAlso workspaceMgr.isWorkspaceOpen(ArrayType.FF_CACHE) Then
				Using wsB As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.FF_CACHE)
					i2d = im2col2d.unsafeDuplication()
				End Using
			End If

			If layerConf().getCnn2dDataFormat() = CNN2DFormat.NHWC Then
				z = z.permute(0,2,3,1) 'NCHW to NHWC
				z = workspaceMgr.dup(ArrayType.ACTIVATIONS, z)
			End If

			Return New Pair(Of INDArray, INDArray)(z,If(forBackprop, im2col2d, Nothing))
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			If input_Conflict Is Nothing Then
				Throw New System.ArgumentException("Cannot perform forward pass with null input " & layerId())
			End If

			If cacheMode_Conflict = Nothing Then
				cacheMode_Conflict = CacheMode.NONE
			End If

			applyDropOutIfNecessary(training, workspaceMgr)

			Dim z As INDArray = preOutput(training, False, workspaceMgr).First

			' we do cache only if cache workspace exists. Skip otherwise
			If training AndAlso cacheMode_Conflict <> CacheMode.NONE AndAlso workspaceMgr.hasConfiguration(ArrayType.FF_CACHE) AndAlso workspaceMgr.isWorkspaceOpen(ArrayType.FF_CACHE) Then
				Using wsB As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.FF_CACHE)
					preOutput = z.unsafeDuplication()
				End Using
			End If

			'String afn = conf.getLayer().getActivationFunction();
			Dim afn As IActivation = layerConf().getActivationFn()

			If helper_Conflict IsNot Nothing AndAlso Shape.strideDescendingCAscendingF(z) AndAlso (helperCountFail = 0 OrElse Not layerConf().isCudnnAllowFallback()) Then
				Dim ret As INDArray = Nothing
				Try
					ret = helper_Conflict.activate(z, layerConf().getActivationFn(), training)
				Catch e As ND4JOpProfilerException
					Throw e 'NaN panic etc for debugging
				Catch e As Exception
					If e.Message IsNot Nothing AndAlso e.Message.contains("Failed to allocate") Then
						'This is a memory exception - don't fallback to built-in implementation
						Throw e
					End If

					If layerConf().isCudnnAllowFallback() Then
						helperCountFail += 1
						If TypeOf helper_Conflict Is MKLDNNConvHelper Then
							log.warn("MKL-DNN execution failed - falling back on built-in implementation", e)
						Else
							log.warn("CuDNN execution failed - falling back on built-in implementation", e)
						End If
					Else
						Throw New Exception("Error during ConvolutionLayer MKL/CuDNN helper forward pass - isCudnnAllowFallback() is set to false", e)
					End If
				End Try

				If ret IsNot Nothing Then
					Return ret
				End If
			End If

			Dim activation As INDArray = afn.getActivation(z, training)
			Return activation
		End Function

		Public Overrides Function hasBias() As Boolean
			Return layerConf().hasBias()
		End Function

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Helper As LayerHelper
			Get
				Return helper_Conflict
			End Get
		End Property

		Public Overrides Sub fit(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overrides WriteOnly Property Params As INDArray
			Set(ByVal params As INDArray)
				'Override, as base layer does f order parameter flattening by default
				setParams(params, "c"c)
			End Set
		End Property

		Public Overrides Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			If maskArray Is Nothing Then
				'For same mode (with stride 1): output activations size is always same size as input activations size -> mask array is same size
				Return New Pair(Of INDArray, MaskState)(maskArray, currentMaskState)
			End If

			Dim outMask As INDArray = ConvolutionUtils.cnn2dMaskReduction(maskArray, layerConf().getKernelSize(), layerConf().getStride(), layerConf().getPadding(), layerConf().getDilation(), layerConf().getConvolutionMode())
			Return New Pair(Of INDArray, MaskState)(outMask, currentMaskState)
		End Function

	End Class

End Namespace