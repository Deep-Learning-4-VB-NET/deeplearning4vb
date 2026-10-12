Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Linq
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BinaryByteUnit = com.jakewharton.byteunits.BinaryByteUnit
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports AlgoMode = org.deeplearning4j.nn.conf.layers.ConvolutionLayer.AlgoMode
Imports BwdDataAlgo = org.deeplearning4j.nn.conf.layers.ConvolutionLayer.BwdDataAlgo
Imports BwdFilterAlgo = org.deeplearning4j.nn.conf.layers.ConvolutionLayer.BwdFilterAlgo
Imports FwdAlgo = org.deeplearning4j.nn.conf.layers.ConvolutionLayer.FwdAlgo
Imports PoolingType = org.deeplearning4j.nn.conf.layers.PoolingType
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports BaseCudnnHelper = org.deeplearning4j.cuda.BaseCudnnHelper
Imports ConvolutionHelper = org.deeplearning4j.nn.layers.convolution.ConvolutionHelper
Imports ConvolutionParamInitializer = org.deeplearning4j.nn.params.ConvolutionParamInitializer
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
Imports Allocator = org.nd4j.jita.allocator.Allocator
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports CudaEnvironment = org.nd4j.jita.conf.CudaEnvironment
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports GridExecutioner = org.nd4j.linalg.api.ops.executioner.GridExecutioner
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports OneTimeLogger = org.nd4j.common.util.OneTimeLogger
Imports org.bytedeco.cuda.cudart
Imports org.bytedeco.cuda.cudnn
Imports org.bytedeco.cuda.global.cudnn
import static org.nd4j.linalg.indexing.NDArrayIndex.all
import static org.nd4j.linalg.indexing.NDArrayIndex.interval

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.deeplearning4j.cuda.convolution


	''' <summary>
	''' cuDNN-based helper for the convolution layer.
	''' 
	''' @author saudet
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CudnnConvolutionHelper extends org.deeplearning4j.cuda.BaseCudnnHelper implements org.deeplearning4j.nn.layers.convolution.ConvolutionHelper
	Public Class CudnnConvolutionHelper
		Inherits BaseCudnnHelper
		Implements ConvolutionHelper

		Public Sub New(ByVal dataType As DataType)
			MyBase.New(dataType)
		End Sub

		Private Class CudnnConvolutionContext
			Inherits CudnnContext

			Private Class Deallocator
				Inherits CudnnConvolutionContext
				Implements Pointer.Deallocator

				Friend Sub New(ByVal c As CudnnConvolutionContext)
					MyBase.New(c)
				End Sub

				Public Overrides Sub deallocate()
					destroyHandles()
				End Sub
			End Class

			Friend srcTensorDesc As New cudnnTensorStruct(), dstTensorDesc As New cudnnTensorStruct(), biasTensorDesc As New cudnnTensorStruct(), deltaTensorDesc As New cudnnTensorStruct()
			Friend filterDesc As New cudnnFilterStruct()
			Friend convDesc As New cudnnConvolutionStruct()
			Friend activationDesc As New cudnnActivationStruct()

			Public Sub New()
				createHandles()
				deallocator(New Deallocator(Me))
			End Sub

			Public Sub New(ByVal c As CudnnConvolutionContext)
				MyBase.New(c)
				srcTensorDesc = New cudnnTensorStruct(c.srcTensorDesc)
				dstTensorDesc = New cudnnTensorStruct(c.dstTensorDesc)
				biasTensorDesc = New cudnnTensorStruct(c.biasTensorDesc)
				deltaTensorDesc = New cudnnTensorStruct(c.deltaTensorDesc)
				filterDesc = New cudnnFilterStruct(c.filterDesc)
				convDesc = New cudnnConvolutionStruct(c.convDesc)
				activationDesc = New cudnnActivationStruct(c.activationDesc)
			End Sub

			Protected Friend Overrides Sub createHandles()
				MyBase.createHandles()
				checkCudnn(cudnnCreateTensorDescriptor(srcTensorDesc))
				checkCudnn(cudnnCreateTensorDescriptor(dstTensorDesc))
				checkCudnn(cudnnCreateTensorDescriptor(biasTensorDesc))
				checkCudnn(cudnnCreateTensorDescriptor(deltaTensorDesc))
				checkCudnn(cudnnCreateFilterDescriptor(filterDesc))
				checkCudnn(cudnnCreateConvolutionDescriptor(convDesc))
				checkCudnn(cudnnCreateActivationDescriptor(activationDesc))
			End Sub

			Protected Friend Overrides Sub destroyHandles()
				checkCudnn(cudnnDestroyActivationDescriptor(activationDesc))
				checkCudnn(cudnnDestroyConvolutionDescriptor(convDesc))
				checkCudnn(cudnnDestroyFilterDescriptor(filterDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(srcTensorDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(dstTensorDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(biasTensorDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(deltaTensorDesc))
				MyBase.destroyHandles()
			End Sub
		End Class

		Private cudnnContext As New CudnnConvolutionContext()

		Public Overridable Function backpropGradient(ByVal input As INDArray, ByVal weights As INDArray, ByVal bias As INDArray, ByVal delta As INDArray, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal pad() As Integer, ByVal biasGradView As INDArray, ByVal weightGradView As INDArray, ByVal afn As IActivation, ByVal mode As AlgoMode, ByVal bwdFilterAlgo As BwdFilterAlgo, ByVal bwdDataAlgo As BwdDataAlgo, ByVal convolutionMode As ConvolutionMode, ByVal dilation() As Integer, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray) Implements ConvolutionHelper.backpropGradient

			'AB 2020/04/21 - cuDNN does have NHWC support (with limitations) however I have been unable to get it working
			' correctly on NHWC data, even after updating all descriptors, tensor format, etc.
			'Therefore: all computation here is done in NCHW format only
			'As of a future (next?) release we'll likely switch to C++ for cuDNN support
			Dim origNHWC As Boolean = False
			If format = CNN2DFormat.NHWC Then
				input = input.permute(0,3,1,2) 'NHWC to NCHW
				delta = delta.permute(0,3,1,2)
				origNHWC = True
			End If

			Dim TENSOR_FORMAT As Integer = CUDNN_TENSOR_NCHW

			Dim code As Integer

			Dim miniBatch As val = input.size(0)
			Dim outDepth As val = weights.size(0)
			Dim inDepth As val = weights.size(1)
			Dim kH As val = weights.size(2)
			Dim kW As val = weights.size(3)

			Dim args As CudnnForwardArgs = getCudnnForwardArgs(input, kernel, strides, pad, dilation, convolutionMode, Nothing, CNN2DFormat.NCHW) 'Note hardcoded NCHW due to above
			input = args.getInput()
			Dim inH As val = input.size(2)
			Dim inW As val = input.size(3)
			Dim srcStride As val = input.stride()
			Dim outSize As val = args.getOutSize()
			Dim outH As val = outSize(0)
			Dim outW As val = outSize(1)

			If Not Shape.strideDescendingCAscendingF(delta) Then
				' apparently not supported by cuDNN
				delta = delta.dup()
			End If

			Dim deltaStride As val = delta.stride()
			Dim algo1(0) As Integer
			Dim algo2(0) As Integer


			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			code = cudnnSetTensor4dDescriptorEx(cudnnContext.srcTensorDesc, dataType, CInt(miniBatch), CInt(inDepth),CInt(inH), CInt(inW), CInt(Math.Truncate(srcStride(0))), CInt(Math.Truncate(srcStride(1))), CInt(Math.Truncate(srcStride(2))), CInt(Math.Truncate(srcStride(3))))
			checkCudnn(False, "cudnnSetTensor4dDescriptorEx", code, input, weights, Nothing, delta, kernel, strides, pad, mode, Nothing, bwdFilterAlgo, bwdDataAlgo, convolutionMode, dilation)
			code = cudnnSetTensor4dDescriptorEx(cudnnContext.deltaTensorDesc, dataType, CInt(miniBatch), CInt(outDepth), CInt(outH), CInt(outW), CInt(Math.Truncate(deltaStride(0))), CInt(Math.Truncate(deltaStride(1))), CInt(Math.Truncate(deltaStride(2))), CInt(Math.Truncate(deltaStride(3))))
			checkCudnn(False, "cudnnSetTensor4dDescriptorEx", code, input, weights, Nothing, delta, kernel, strides, pad, mode, Nothing, bwdFilterAlgo, bwdDataAlgo, convolutionMode, dilation)
			code = cudnnSetConvolution2dDescriptor(cudnnContext.convDesc, pad(0), pad(1), strides(0), strides(1), dilation(0), dilation(1), CUDNN_CROSS_CORRELATION, dataType)
			checkCudnn(False, "cudnnSetConvolution2dDescriptor", code, input, weights, Nothing, delta, kernel, strides, pad, mode, Nothing, bwdFilterAlgo, bwdDataAlgo, convolutionMode, dilation)
			code = cudnnSetFilter4dDescriptor(cudnnContext.filterDesc, dataType, TENSOR_FORMAT, CInt(outDepth), CInt(inDepth), CInt(kH), CInt(kW))
			checkCudnn(False, "cudnnSetFilter4dDescriptor", code, input, weights, Nothing, delta, kernel, strides, pad, mode, Nothing, bwdFilterAlgo, bwdDataAlgo, convolutionMode, dilation)

			If mode = AlgoMode.USER_SPECIFIED AndAlso bwdFilterAlgo <> Nothing AndAlso bwdDataAlgo <> Nothing Then
				Select Case bwdFilterAlgo
					Case BwdFilterAlgo.ALGO_0
						algo1(0) = CUDNN_CONVOLUTION_BWD_FILTER_ALGO_0
					Case BwdFilterAlgo.ALGO_1
						algo1(0) = CUDNN_CONVOLUTION_BWD_FILTER_ALGO_1
					Case BwdFilterAlgo.FFT
						algo1(0) = CUDNN_CONVOLUTION_BWD_FILTER_ALGO_FFT
					Case BwdFilterAlgo.ALGO_3
						algo1(0) = CUDNN_CONVOLUTION_BWD_FILTER_ALGO_3
					Case BwdFilterAlgo.WINOGRAD
						algo1(0) = CUDNN_CONVOLUTION_BWD_FILTER_ALGO_WINOGRAD
					Case BwdFilterAlgo.WINOGRAD_NONFUSED
						algo1(0) = CUDNN_CONVOLUTION_BWD_FILTER_ALGO_WINOGRAD_NONFUSED
					Case BwdFilterAlgo.FFT_TILING
						algo1(0) = CUDNN_CONVOLUTION_BWD_FILTER_ALGO_FFT_TILING
					Case BwdFilterAlgo.COUNT
						algo1(0) = CUDNN_CONVOLUTION_BWD_FILTER_ALGO_COUNT
					Case Else
						Throw New System.ArgumentException("Unknown BwdFilterAlgo: " & bwdFilterAlgo)
				End Select

				Select Case bwdDataAlgo
					Case BwdDataAlgo.ALGO_0
						algo2(0) = CUDNN_CONVOLUTION_BWD_DATA_ALGO_0
					Case BwdDataAlgo.ALGO_1
						algo2(0) = CUDNN_CONVOLUTION_BWD_DATA_ALGO_1
					Case BwdDataAlgo.FFT
						algo2(0) = CUDNN_CONVOLUTION_BWD_DATA_ALGO_FFT
					Case BwdDataAlgo.FFT_TILING
						algo2(0) = CUDNN_CONVOLUTION_BWD_DATA_ALGO_FFT_TILING
					Case BwdDataAlgo.WINOGRAD
						algo2(0) = CUDNN_CONVOLUTION_BWD_DATA_ALGO_WINOGRAD
					Case BwdDataAlgo.WINOGRAD_NONFUSED
						algo2(0) = CUDNN_CONVOLUTION_BWD_DATA_ALGO_WINOGRAD_NONFUSED
					Case BwdDataAlgo.COUNT
						algo2(0) = CUDNN_CONVOLUTION_BWD_DATA_ALGO_COUNT
					Case Else
						Throw New System.ArgumentException("Unknown BwdDataAlgo: " & bwdDataAlgo)
				End Select
			Else
	'            
	'            code = cudnnGetConvolutionBackwardFilterAlgorithm(cudnnContext, cudnnContext.srcTensorDesc,
	'                    cudnnContext.deltaTensorDesc, cudnnContext.convDesc, cudnnContext.filterDesc,
	'                    mode == AlgoMode.NO_WORKSPACE ? CUDNN_CONVOLUTION_BWD_FILTER_NO_WORKSPACE
	'                            : CUDNN_CONVOLUTION_BWD_FILTER_PREFER_FASTEST,
	'                    0, algo1);
	'            
				Dim fa As val = New cudnnConvolutionBwdFilterAlgoPerf_t()
				Dim counts As val = New Integer(0){}
				code = cudnnFindConvolutionBackwardFilterAlgorithm(cudnnContext, cudnnContext.srcTensorDesc, cudnnContext.deltaTensorDesc, cudnnContext.convDesc, cudnnContext.filterDesc, 1, counts, fa)
				algo1(0) = fa.algo()

				checkCudnn(False, "cudnnGetConvolutionBackwardFilterAlgorithm", code, input, weights, Nothing, delta, kernel, strides, pad, mode, Nothing, bwdFilterAlgo, bwdDataAlgo, convolutionMode, dilation)

	'            
	'            code = cudnnGetConvolutionBackwardDataAlgorithm(cudnnContext, cudnnContext.filterDesc,
	'                    cudnnContext.deltaTensorDesc, cudnnContext.convDesc, cudnnContext.srcTensorDesc,
	'                    mode == AlgoMode.NO_WORKSPACE ? CUDNN_CONVOLUTION_BWD_DATA_NO_WORKSPACE
	'                            : CUDNN_CONVOLUTION_BWD_DATA_PREFER_FASTEST,
	'                    0, algo2);
	'             

				Dim da As val = New cudnnConvolutionBwdDataAlgoPerf_t()
				code = cudnnFindConvolutionBackwardDataAlgorithm(cudnnContext, cudnnContext.filterDesc, cudnnContext.deltaTensorDesc, cudnnContext.convDesc, cudnnContext.srcTensorDesc, 1, counts, da)

				algo2(0) = da.algo()
				checkCudnn(False, "cudnnGetConvolutionBackwardDataAlgorithm", code, input, weights, Nothing, delta, kernel, strides, pad, mode, Nothing, bwdFilterAlgo, bwdDataAlgo, convolutionMode, dilation)
			End If

			If log.isTraceEnabled() Then
				Dim fa As BwdFilterAlgo = System.Enum.GetValues(GetType(BwdFilterAlgo))(algo1(0))
				Dim da As BwdDataAlgo = System.Enum.GetValues(GetType(BwdDataAlgo))(algo2(0))
				log.trace("CudnnConvolutionHelper backward algorithm selection: mode {}, filter algorithm {}, data algorithm {}", mode, fa, da)
			End If

			Dim epsNext As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, weights.dataType(), New Long() {CInt(miniBatch), CInt(inDepth), CInt(inH), CInt(inW)}, "c"c)

			Dim dstStride As val = epsNext.stride()

			Dim allocator As Allocator = AtomicAllocator.Instance
			Dim context As CudaContext = allocator.FlowController.prepareActionAllWrite(input, weights, weightGradView, biasGradView, delta, epsNext)
			Dim srcData As Pointer = allocator.getPointer(input, context)
			Dim filterData As Pointer = allocator.getPointer(weights, context)
			Dim filterGradData As Pointer = allocator.getPointer(weightGradView, context)
			Dim biasGradData As Pointer = allocator.getPointer(biasGradView, context)
			Dim deltaData As Pointer = allocator.getPointer(delta, context)
			Dim dstData As Pointer = allocator.getPointer(epsNext, context)

			code = cudnnSetStream(cudnnContext, New CUstream_st(context.CublasStream))
			checkCudnn(False, "cudnnSetStream", code, input, weights, Nothing, delta, kernel, strides, pad, mode, Nothing, bwdFilterAlgo, bwdDataAlgo, convolutionMode, dilation)

			code = cudnnSetTensor4dDescriptorEx(cudnnContext.dstTensorDesc, dataType, CInt(miniBatch), CInt(inDepth), CInt(inH), CInt(inW), CInt(Math.Truncate(dstStride(0))), CInt(Math.Truncate(dstStride(1))), CInt(Math.Truncate(dstStride(2))), CInt(Math.Truncate(dstStride(3))))
			checkCudnn(False, "cudnnSetTensor4dDescriptorEx", code, input, weights, Nothing, delta, kernel, strides, pad, mode, Nothing, bwdFilterAlgo, bwdDataAlgo, convolutionMode, dilation)

			code = cudnnGetConvolutionBackwardFilterWorkspaceSize(cudnnContext, cudnnContext.srcTensorDesc, cudnnContext.deltaTensorDesc, cudnnContext.convDesc, cudnnContext.filterDesc, algo1(0), sizeInBytes)
			checkCudnn(False, "cudnnGetConvolutionBackwardFilterWorkspaceSize", code, input, weights, Nothing, delta, kernel, strides, pad, mode, Nothing, bwdFilterAlgo, bwdDataAlgo, convolutionMode, dilation)

			Dim sizeInBytes1 As Long = sizeInBytes.get(0)
			code = cudnnGetConvolutionBackwardDataWorkspaceSize(cudnnContext, cudnnContext.filterDesc, cudnnContext.deltaTensorDesc, cudnnContext.convDesc, cudnnContext.dstTensorDesc, algo2(0), sizeInBytes)
			checkCudnn(False, "cudnnGetConvolutionBackwardDataWorkspaceSize", code, input, weights, Nothing, delta, kernel, strides, pad, mode, Nothing, bwdFilterAlgo, bwdDataAlgo, convolutionMode, dilation)

			Dim workSpace As DataCache = workspaceMgr.getHelperWorkspace(LayerWorkspaceMgr.CUDNN_WORKSPACE_KEY)
			Dim sizeInBytes2 As Long = sizeInBytes.get(0)
			If workSpace Is Nothing OrElse sizeInBytes1 > workSpace.capacity() OrElse sizeInBytes2 > workSpace.capacity() Then
				Dim newSize As Long = Math.Max(sizeInBytes1, sizeInBytes2)
				If log.isTraceEnabled() Then
					If workSpace Is Nothing Then
						log.trace("CudnnConvolutionHelper backpropGradient: Allocating initial workspace of size {} ({})", newSize, BinaryByteUnit.format(newSize, "#.00"))
					Else
						log.trace("CudnnConvolutionHelper backpropGradient: Deallocating workspace of size {} ({}), allocating new workspace of size {} ({})", workSpace.capacity(), BinaryByteUnit.format(workSpace.capacity(), "#.00"), newSize, BinaryByteUnit.format(newSize, "#.00"))
					End If
				End If
				If workSpace IsNot Nothing Then
					workSpace.deallocate()
				End If
				workSpace = New DataCache(newSize)
				workspaceMgr.setHelperWorkspace(LayerWorkspaceMgr.CUDNN_WORKSPACE_KEY, workSpace)
			End If

			code = cudnnSetTensor4dDescriptor(cudnnContext.biasTensorDesc, TENSOR_FORMAT, dataType, 1, CInt(outDepth), 1, 1)
			checkCudnn(False, "cudnnSetTensor4dDescriptor", code, input, weights, Nothing, delta, kernel, strides, pad, mode, Nothing, bwdFilterAlgo, bwdDataAlgo, convolutionMode, dilation)

			code = cudnnConvolutionBackwardBias(cudnnContext, alpha, cudnnContext.deltaTensorDesc, deltaData, beta, cudnnContext.biasTensorDesc, biasGradData)
			checkCudnn(False, "cudnnConvolutionBackwardBias", code, input, weights, Nothing, delta, kernel, strides, pad, mode, Nothing, bwdFilterAlgo, bwdDataAlgo, convolutionMode, dilation)

			code = cudnnConvolutionBackwardFilter(cudnnContext, alpha, cudnnContext.srcTensorDesc, srcData, cudnnContext.deltaTensorDesc, deltaData, cudnnContext.convDesc, algo1(0), workSpace, workSpace.capacity(), beta, cudnnContext.filterDesc, filterGradData)
			checkCudnn(False, "cudnnConvolutionBackwardFilter", code, input, weights, Nothing, delta, kernel, strides, pad, mode, Nothing, bwdFilterAlgo, bwdDataAlgo, convolutionMode, dilation)

			code = cudnnConvolutionBackwardData(cudnnContext, alpha, cudnnContext.filterDesc, filterData, cudnnContext.deltaTensorDesc, deltaData, cudnnContext.convDesc, algo2(0), workSpace, workSpace.capacity(), beta, cudnnContext.dstTensorDesc, dstData)
			checkCudnn(False, "cudnnConvolutionBackwardData", code, input, weights, Nothing, delta, kernel, strides, pad, mode, Nothing, bwdFilterAlgo, bwdDataAlgo, convolutionMode, dilation)

			allocator.FlowController.registerActionAllWrite(context, input, weights, weightGradView, biasGradView, delta, epsNext)

			Dim retGradient As Gradient = New DefaultGradient()
			retGradient.setGradientFor(ConvolutionParamInitializer.BIAS_KEY, biasGradView)
			retGradient.setGradientFor(ConvolutionParamInitializer.WEIGHT_KEY, weightGradView, "c"c)

			If CudaEnvironment.Instance.Configuration.isDebug() Then
				context.syncOldStream()
			End If

			'Note that: if we had to manually pad for SAME mode, we have to 'undo' this manual padding for the epsilon
			' we return. The returned epsilon (i.e., dL/dIn array) has to be the same shape as the *original* input.
			If args.isManualPadBottom() OrElse args.isManualPadRight() Then
				epsNext = epsNext.get(all(), all(), interval(0, epsNext.size(2) - (If(args.isManualPadBottom(), 1, 0))), interval(0, epsNext.size(3) - (If(args.isManualPadRight(), 1, 0))))
			End If

			If origNHWC Then
				epsNext = epsNext.permute(0,2,3,1) 'NCHW to NHWC
			End If

			Return New Pair(Of Gradient, INDArray)(retGradient, epsNext)
		End Function

		Public Overridable Function preOutput(ByVal input As INDArray, ByVal weights As INDArray, ByVal bias As INDArray, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal pad() As Integer, ByVal mode As AlgoMode, ByVal fwdAlgo As FwdAlgo, ByVal convolutionMode As ConvolutionMode, ByVal dilation() As Integer, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements ConvolutionHelper.preOutput

			'AB 2020/04/21 - cuDNN does have NHWC support (with limitations) however I have been unable to get it working
			' correctly on NHWC data, even after updating all descriptors, tensor format, etc.
			'Therefore: all computation here is done in NCHW format only
			'As of a future (next?) release we'll likely switch to C++ for cuDNN support
			Dim origNHWC As Boolean = False
			If format = CNN2DFormat.NHWC Then
				input = input.permute(0,3,1,2) 'NHWC to NCHW
				origNHWC = True
			End If

			Dim TENSOR_FORMAT As Integer = CUDNN_TENSOR_NCHW

			Dim code As Integer

			Dim miniBatch As val = input.size(0)
			Dim outDepth As val = weights.size(0)
			Dim inDepth As val = weights.size(1)
			Dim kH As val = weights.size(2)
			Dim kW As val = weights.size(3)

			Dim args As CudnnForwardArgs = getCudnnForwardArgs(input, kernel, strides, pad, dilation, convolutionMode, Nothing, CNN2DFormat.NCHW) 'Note hardcoded NCHW due to above
			input = args.getInput()
			Dim inH As val = input.size(2)
			Dim inW As val = input.size(3)
			Dim srcStride As val = input.stride()
			Dim outSize As val = args.getOutSize()

			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			Dim z As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, weights.dataType(), New Long() {CInt(miniBatch), CInt(outDepth), outSize(0), outSize(1)})

			code = cudnnSetTensor4dDescriptorEx(cudnnContext.srcTensorDesc, dataType, CInt(miniBatch), CInt(inDepth), CInt(inH), CInt(inW), CInt(Math.Truncate(srcStride(0))), CInt(Math.Truncate(srcStride(1))), CInt(Math.Truncate(srcStride(2))), CInt(Math.Truncate(srcStride(3))))
			checkCudnn(True, "cudnnSetTensor4dDescriptorEx", code, input, weights, bias, Nothing, kernel, strides, pad, mode, fwdAlgo, Nothing, Nothing, convolutionMode, dilation)

			code = cudnnSetFilter4dDescriptor(cudnnContext.filterDesc, dataType, TENSOR_FORMAT, CInt(outDepth), CInt(inDepth), CInt(kH), CInt(kW))
			checkCudnn(True, "cudnnSetFilter4dDescriptor", code, input, weights, bias, Nothing, kernel, strides, pad, mode, fwdAlgo, Nothing, Nothing, convolutionMode, dilation)

			code = cudnnSetConvolution2dDescriptor(cudnnContext.convDesc, pad(0), pad(1), strides(0), strides(1), dilation(0), dilation(1), CUDNN_CROSS_CORRELATION, dataType)
			checkCudnn(True, "cudnnSetConvolution2dDescriptor", code, input, weights, bias, Nothing, kernel, strides, pad, mode, fwdAlgo, Nothing, Nothing, convolutionMode, dilation)


			' find dimension of convolution output
			'        checkCudnn(cudnnGetConvolution2dForwardOutputDim(cudnnContext.convDesc, cudnnContext.srcTensorDesc, cudnnContext.filterDesc, n, c, h, w));
			'        INDArray z = Nd4j.createUninitialized(new int[]{n[0],c[0],h[0],w[0]},'c');


			Dim algo(0) As Integer
			Dim dstStride As val = z.stride()
			code = cudnnSetTensor4dDescriptorEx(cudnnContext.dstTensorDesc, dataType, CInt(miniBatch), CInt(outDepth), CInt(Math.Truncate(outSize(0))), CInt(Math.Truncate(outSize(1))), CInt(Math.Truncate(dstStride(0))), CInt(Math.Truncate(dstStride(1))), CInt(Math.Truncate(dstStride(2))), CInt(Math.Truncate(dstStride(3))))
			checkCudnn(True, "cudnnSetTensor4dDescriptorEx", code, input, weights, bias, Nothing, kernel, strides, pad, mode, fwdAlgo, Nothing, Nothing, convolutionMode, dilation)

			If mode = AlgoMode.USER_SPECIFIED AndAlso fwdAlgo <> Nothing Then
				Select Case fwdAlgo
					Case FwdAlgo.IMPLICIT_GEMM
						algo(0) = CUDNN_CONVOLUTION_FWD_ALGO_IMPLICIT_GEMM
					Case FwdAlgo.IMPLICIT_PRECOMP_GEMM
						algo(0) = CUDNN_CONVOLUTION_FWD_ALGO_IMPLICIT_PRECOMP_GEMM
					Case FwdAlgo.GEMM
						algo(0) = CUDNN_CONVOLUTION_FWD_ALGO_GEMM
					Case FwdAlgo.DIRECT
						algo(0) = CUDNN_CONVOLUTION_FWD_ALGO_DIRECT
					Case FwdAlgo.FFT
						algo(0) = CUDNN_CONVOLUTION_FWD_ALGO_FFT
					Case FwdAlgo.FFT_TILING
						algo(0) = CUDNN_CONVOLUTION_FWD_ALGO_FFT_TILING
					Case FwdAlgo.WINOGRAD
						algo(0) = CUDNN_CONVOLUTION_FWD_ALGO_WINOGRAD
					Case FwdAlgo.WINOGRAD_NONFUSED
						algo(0) = CUDNN_CONVOLUTION_FWD_ALGO_WINOGRAD_NONFUSED
					Case FwdAlgo.COUNT
						algo(0) = CUDNN_CONVOLUTION_FWD_ALGO_COUNT
					Case Else
						Throw New System.ArgumentException("Unknown FwdAlgo: " & fwdAlgo)
				End Select
			Else
	'            
	'            code = cudnnGetConvolutionForwardAlgorithm_v7(cudnnContext, cudnnContext.srcTensorDesc,
	'                    cudnnContext.filterDesc, cudnnContext.convDesc,
	'                    cudnnContext.dstTensorDesc, mode == AlgoMode.NO_WORKSPACE
	'                            ? CUDNN_CONVOLUTION_FWD_ : CUDNN_CONVOLUTION_FWD_PREFER_FASTEST,
	'                    0, algo);
	'            

				Dim cdf As val = New cudnnConvolutionFwdAlgoPerf_t()
				Dim count As val = New Integer(0){}
				code = cudnnFindConvolutionForwardAlgorithm(cudnnContext, cudnnContext.srcTensorDesc, cudnnContext.filterDesc, cudnnContext.convDesc, cudnnContext.dstTensorDesc, 1, count, cdf)

				If code <> CUDNN_STATUS_SUCCESS Then
					'If CuDNN can't infer algorithm - try IMPLICIT_GEMM
					'Why this specifically? According to the docs, it seems to have the least number of restrictions
					' to things like dilation

					OneTimeLogger.warn(log, "Error getting CuDNN forward algorithm - falling back on IMPLICIT_GEMM")
					mode = AlgoMode.USER_SPECIFIED
					fwdAlgo = FwdAlgo.IMPLICIT_GEMM
					algo(0) = CUDNN_CONVOLUTION_FWD_ALGO_IMPLICIT_GEMM
				End If

				algo(0) = cdf.algo()
			End If

			If log.isTraceEnabled() Then
				Dim a As FwdAlgo = System.Enum.GetValues(GetType(FwdAlgo))(algo(0))
				log.trace("CudnnConvolutionHelper forward algorithm selection: mode {}, algorithm {}", mode, a)
			End If

			Dim allocator As Allocator = AtomicAllocator.Instance
			Dim context As CudaContext = allocator.FlowController.prepareAction(z, input, weights, bias)
			Dim srcData As Pointer = allocator.getPointer(input, context)
			Dim filterData As Pointer = allocator.getPointer(weights, context)
			Dim biasData As Pointer = allocator.getPointer(bias, context)
			Dim dstData As Pointer = allocator.getPointer(z, context)

			code = cudnnSetStream(cudnnContext, New CUstream_st(context.CublasStream))
			checkCudnn(True, "cudnnSetStream", code, input, weights, bias, Nothing, kernel, strides, pad, mode, fwdAlgo, Nothing, Nothing, convolutionMode, dilation)

			code = cudnnGetConvolutionForwardWorkspaceSize(cudnnContext, cudnnContext.srcTensorDesc, cudnnContext.filterDesc, cudnnContext.convDesc, cudnnContext.dstTensorDesc, algo(0), sizeInBytes)
			checkCudnn(True, "cudnnGetConvolutionForwardWorkspaceSize", code, input, weights, bias, Nothing, kernel, strides, pad, mode, fwdAlgo, Nothing, Nothing, convolutionMode, dilation)

			Dim workSpace As DataCache = workspaceMgr.getHelperWorkspace(LayerWorkspaceMgr.CUDNN_WORKSPACE_KEY)
			If workSpace Is Nothing OrElse sizeInBytes.get(0) > workSpace.capacity() Then
				If log.isTraceEnabled() Then
					If workSpace Is Nothing Then
						log.trace("CudnnConvolutionHelper preOutput: allocating initial workspace of size {} ({})", sizeInBytes.get(), BinaryByteUnit.format(sizeInBytes.get(), "#.00"))
					Else
						log.trace("CudnnConvolutionHelper preOutput: Deallocating workspace of size {} ({}), allocating new workspace of size {} ({})", workSpace.capacity(), BinaryByteUnit.format(workSpace.capacity(), "#.00"), sizeInBytes.get(), BinaryByteUnit.format(sizeInBytes.get(), "#.00"))
					End If
				End If
				If workSpace IsNot Nothing Then
					workSpace.deallocate()
				End If
				workSpace = New DataCache(sizeInBytes.get(0))
				workspaceMgr.setHelperWorkspace(LayerWorkspaceMgr.CUDNN_WORKSPACE_KEY, workSpace)
			End If
			code = cudnnConvolutionForward(cudnnContext, alpha, cudnnContext.srcTensorDesc, srcData, cudnnContext.filterDesc, filterData, cudnnContext.convDesc, algo(0), workSpace, workSpace.capacity(), beta, cudnnContext.dstTensorDesc, dstData)
			checkCudnn(True, "cudnnConvolutionForward", code, input, weights, bias, Nothing, kernel, strides, pad, mode, fwdAlgo, Nothing, Nothing, convolutionMode, dilation)


			code = cudnnSetTensor4dDescriptor(cudnnContext.biasTensorDesc, TENSOR_FORMAT, dataType, 1, CInt(outDepth), 1, 1)
			checkCudnn(True, "cudnnSetTensor4dDescriptor", code, input, weights, bias, Nothing, kernel, strides, pad, mode, fwdAlgo, Nothing, Nothing, convolutionMode, dilation)

			code = cudnnAddTensor(cudnnContext, alpha, cudnnContext.biasTensorDesc, biasData, alpha, cudnnContext.dstTensorDesc, dstData)
			checkCudnn(True, "cudnnAddTensor", code, input, weights, bias, Nothing, kernel, strides, pad, mode, fwdAlgo, Nothing, Nothing, convolutionMode, dilation)

			allocator.registerAction(context, z, input, weights, bias)

			If CudaEnvironment.Instance.Configuration.isDebug() Then
				context.syncOldStream()
			End If

			If origNHWC Then
				z = z.permute(0,2,3,1) 'NCHW to NHWC
			End If

			Return z
		End Function

		Private Sub checkCudnn(ByVal forward As Boolean, ByVal [step] As String, ByVal code As Integer, ByVal input As INDArray, ByVal weights As INDArray, ByVal bias As INDArray, ByVal delta As INDArray, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal pad() As Integer, ByVal mode As AlgoMode, ByVal fwdAlgo As FwdAlgo, ByVal bwdFilterAlgo As BwdFilterAlgo, ByVal bwdDataAlgo As BwdDataAlgo, ByVal convolutionMode As ConvolutionMode, ByVal dilation() As Integer)

			If code <> CUDNN_STATUS_SUCCESS Then
				Dim sb As New StringBuilder()
				sb.Append("CuDNN error = ").Append(code).Append(": ").Append(cudnnGetErrorString(code).getString()).Append(" during ").Append(If(forward, "forward pass", "backward pass")).Append(" - step ").Append([step]).Append(": inputShape=").Append(Arrays.toString(input.shape())).Append(", weightsShape=").Append(Arrays.toString(weights.shape())).Append(", biasShape=").Append(If(bias Is Nothing, Nothing, Arrays.toString(bias.shape())))
				If Not forward Then
					sb.Append(", gradientShape=").Append(Arrays.toString(delta.shape()))
				End If
				sb.Append(", kernel=").Append(Arrays.toString(kernel)).Append(", stride=").Append(Arrays.toString(strides)).Append(", padding=").Append(Arrays.toString(pad)).Append(", dilation=").Append(Arrays.toString(dilation)).Append(", AlgoMode=").Append(mode)
				If forward Then
					sb.Append(", fwdAlgo=").Append(fwdAlgo)
				Else
					sb.Append(", bwdFilterAlgo=").Append(bwdFilterAlgo).Append(", bwdDataAlgo=").Append(bwdDataAlgo)
				End If
				sb.Append(", convolutionMode=").Append(convolutionMode)

				Throw New Exception(sb.ToString())
			End If
		End Sub

		Public Overridable Function activate(ByVal z As INDArray, ByVal afn As IActivation, ByVal training As Boolean) As INDArray Implements ConvolutionHelper.activate
			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			Dim activation As INDArray = z

			Dim allocator As Allocator = AtomicAllocator.Instance
			Dim context As CudaContext = allocator.FlowController.prepareAction(z)
			Dim dstData As Pointer = allocator.getPointer(z, context)

			checkCudnn(cudnnSetStream(cudnnContext, New CUstream_st(context.CublasStream)))
			Select Case afn.ToString()
				Case "identity"
				Case "sigmoid"
					checkCudnn(cudnnSetActivationDescriptor(cudnnContext.activationDesc, CUDNN_ACTIVATION_SIGMOID, CUDNN_PROPAGATE_NAN, 0))
					checkCudnn(cudnnActivationForward(cudnnContext, cudnnContext.activationDesc, alpha, cudnnContext.dstTensorDesc, dstData, beta, cudnnContext.dstTensorDesc, dstData))
				Case "relu"
					checkCudnn(cudnnSetActivationDescriptor(cudnnContext.activationDesc, CUDNN_ACTIVATION_RELU, CUDNN_PROPAGATE_NAN, 0))
					checkCudnn(cudnnActivationForward(cudnnContext, cudnnContext.activationDesc, alpha, cudnnContext.dstTensorDesc, dstData, beta, cudnnContext.dstTensorDesc, dstData))
				Case "tanh"
					checkCudnn(cudnnSetActivationDescriptor(cudnnContext.activationDesc, CUDNN_ACTIVATION_TANH, CUDNN_PROPAGATE_NAN, 0))
					checkCudnn(cudnnActivationForward(cudnnContext, cudnnContext.activationDesc, alpha, cudnnContext.dstTensorDesc, dstData, beta, cudnnContext.dstTensorDesc, dstData))
				Case "softmax"
					checkCudnn(cudnnSoftmaxForward(cudnnContext, CUDNN_SOFTMAX_ACCURATE, CUDNN_SOFTMAX_MODE_CHANNEL, alpha, cudnnContext.dstTensorDesc, dstData, beta, cudnnContext.dstTensorDesc, dstData))
				Case "logsoftmax"
					checkCudnn(cudnnSoftmaxForward(cudnnContext, CUDNN_SOFTMAX_LOG, CUDNN_SOFTMAX_MODE_CHANNEL, alpha, cudnnContext.dstTensorDesc, dstData, beta, cudnnContext.dstTensorDesc, dstData))
				Case Else
					activation = Nothing
			End Select

			allocator.registerAction(context, activation)

			If CudaEnvironment.Instance.Configuration.isDebug() Then
				context.syncOldStream()
			End If

			Return activation
		End Function

		''' <param name="poolingType">     Used when preparing data for subsampling layers ONLY. Null for convolution layers
		''' @return </param>
		Public Shared Function getCudnnForwardArgs(ByVal input As INDArray, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal padding() As Integer, ByVal dilation() As Integer, ByVal convolutionMode As ConvolutionMode, ByVal poolingType As PoolingType, ByVal format As CNN2DFormat) As CudnnForwardArgs
			Dim origInput As INDArray = input

			'Check if we need to dup the input: views, non-contiguous, etc. CuDNN also seems to have has issues if strides
			' are non-default for C order - even if they *should* be OK otherwise
			If input.View OrElse Not Shape.hasDefaultStridesForShape(input) Then
				input = input.dup("c"c)
			End If

			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			Dim hIdx As Integer = If(nchw, 2, 1)
			Dim wIdx As Integer = If(nchw, 3, 2)

			Dim inH As val = input.size(hIdx)
			Dim inW As val = input.size(wIdx)

			Dim manualPadBottom As Boolean = False
			Dim manualPadRight As Boolean = False

			Dim outSize() As Integer
			If convolutionMode = ConvolutionMode.Same Then
				outSize = ConvolutionUtils.getOutputSize(input, kernel, strides, Nothing, convolutionMode, dilation, format) 'Also performs validation
				padding = ConvolutionUtils.getSameModeTopLeftPadding(outSize, New Integer() {CInt(inH), CInt(inW)}, kernel, strides, dilation)
				Dim padBottomRight() As Integer = ConvolutionUtils.getSameModeBottomRightPadding(outSize, New Integer() {CInt(inH), CInt(inW)}, kernel, strides, dilation)
				If Not padding.SequenceEqual(padBottomRight) Then
	'                
	'                CuDNN - even as of 7.1 (CUDA 9.1) still doesn't have support for proper SAME mode padding (i.e., asymmetric
	'                padding) - padding can *only* be specified as the same amount for both the top/bottom, and for left/right.
	'                In SAME mode padding, sometimes these are the same - but often they are not.
	'                Note that when they differ, the bottom or right padding will be exactly 1 more than the top or left padding.
	'                As per TF, we'll manually pad here: https://github.com/tensorflow/tensorflow/blob/master/tensorflow/core/kernels/conv_ops.cc#L571-L607
	'                 
					manualPadBottom = (padding(0) <> padBottomRight(0))
					manualPadRight = (padding(1) <> padBottomRight(1))

					'NCHW format
					Dim newShape() As Long
					If nchw Then
						newShape = New Long(){input.size(0), input.size(1), input.size(2) + (If(manualPadBottom, 1, 0)), input.size(3) + (If(manualPadRight, 1, 0))}
					Else
						newShape = New Long(){input.size(0), input.size(1) + (If(manualPadBottom, 1, 0)), input.size(2) + (If(manualPadRight, 1, 0)), input.size(3)}
					End If
					Dim newInput As INDArray
					If poolingType = Nothing OrElse poolingType <> PoolingType.MAX Then
						newInput = Nd4j.create(input.dataType(), newShape)
					Else
						'For max pooling, we don't want to include the padding in the maximum values. But, CuDNN doesn't knowm
						' that these values are padding and hence should be excluded. Instead: We'll use -infinity so that,
						' if the 'real' (non-padding) values are all < 0, we take the real value, not the padding value
						newInput = Nd4j.valueArrayOf(newShape, Double.NegativeInfinity, input.dataType())
					End If

					If nchw Then
						newInput.put(New INDArrayIndex(){all(), all(), interval(0,input.size(2)), interval(0, input.size(3))}, input)
					Else
						newInput.put(New INDArrayIndex(){all(), interval(0,input.size(1)), interval(0, input.size(2)), all()}, input)
					End If

					input = newInput
					'Now: we've manually applied the "extra" bottom/right padding only - if required. Consequently, we
					' now have the same amount of padding required for top/bottom, and left/right - which we'll let
					' CuDNN handle
				End If
			Else
				outSize = ConvolutionUtils.getOutputSize(input, kernel, strides, padding, convolutionMode, dilation, format) 'Also performs validation
			End If

			Return New CudnnForwardArgs(manualPadBottom, manualPadRight, input, origInput, padding, outSize)
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public static class CudnnForwardArgs
		Public Class CudnnForwardArgs
			Friend manualPadBottom As Boolean
			Friend manualPadRight As Boolean
			Friend input As INDArray
			Friend origInput As INDArray
			Friend padding() As Integer
			Friend outSize() As Integer
		End Class

		Public Overridable Function helperMemoryUse() As IDictionary(Of String, Long)
			'No memory use other than shared, and the structs (which are small)
			Return Collections.emptyMap()
		End Function

	End Class
End Namespace