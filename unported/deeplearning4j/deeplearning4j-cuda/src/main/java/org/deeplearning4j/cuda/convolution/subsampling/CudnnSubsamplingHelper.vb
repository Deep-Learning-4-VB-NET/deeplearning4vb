Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports PoolingType = org.deeplearning4j.nn.conf.layers.PoolingType
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports BaseCudnnHelper = org.deeplearning4j.cuda.BaseCudnnHelper
Imports CudnnConvolutionHelper = org.deeplearning4j.cuda.convolution.CudnnConvolutionHelper
Imports SubsamplingHelper = org.deeplearning4j.nn.layers.convolution.subsampling.SubsamplingHelper
Imports Allocator = org.nd4j.jita.allocator.Allocator
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports CudaEnvironment = org.nd4j.jita.conf.CudaEnvironment
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports GridExecutioner = org.nd4j.linalg.api.ops.executioner.GridExecutioner
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports org.bytedeco.cuda.cudart
Imports org.bytedeco.cuda.cudnn
Imports org.bytedeco.cuda.global.cudnn
import static org.deeplearning4j.cuda.convolution.CudnnConvolutionHelper.getCudnnForwardArgs
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

Namespace org.deeplearning4j.cuda.convolution.subsampling


	''' <summary>
	''' cuDNN-based helper for the subsampling layer.
	''' 
	''' @author saudet
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CudnnSubsamplingHelper extends org.deeplearning4j.cuda.BaseCudnnHelper implements org.deeplearning4j.nn.layers.convolution.subsampling.SubsamplingHelper
	Public Class CudnnSubsamplingHelper
		Inherits BaseCudnnHelper
		Implements SubsamplingHelper

		Public Sub New(ByVal dataType As DataType)
			MyBase.New(dataType)
		End Sub

		Private Class CudnnSubsamplingContext
			Inherits CudnnContext

			Private Class Deallocator
				Inherits CudnnSubsamplingContext
				Implements Pointer.Deallocator

				Friend Sub New(ByVal c As CudnnSubsamplingContext)
					MyBase.New(c)
				End Sub

				Public Overrides Sub deallocate()
					destroyHandles()
				End Sub
			End Class

			Friend srcTensorDesc As New cudnnTensorStruct(), dstTensorDesc As New cudnnTensorStruct(), deltaTensorDesc As New cudnnTensorStruct()
			Friend poolingDesc As New cudnnPoolingStruct()

			Public Sub New()
				createHandles()
				deallocator(New Deallocator(Me))
			End Sub

			Public Sub New(ByVal c As CudnnSubsamplingContext)
				MyBase.New(c)
				srcTensorDesc = New cudnnTensorStruct(c.srcTensorDesc)
				dstTensorDesc = New cudnnTensorStruct(c.dstTensorDesc)
				deltaTensorDesc = New cudnnTensorStruct(c.deltaTensorDesc)
				poolingDesc = New cudnnPoolingStruct(c.poolingDesc)
			End Sub

			Protected Friend Overrides Sub createHandles()
				MyBase.createHandles()
				checkCudnn(cudnnCreateTensorDescriptor(srcTensorDesc))
				checkCudnn(cudnnCreateTensorDescriptor(dstTensorDesc))
				checkCudnn(cudnnCreateTensorDescriptor(deltaTensorDesc))
				checkCudnn(cudnnCreatePoolingDescriptor(poolingDesc))
			End Sub

			Protected Friend Overrides Sub destroyHandles()
				checkCudnn(cudnnDestroyPoolingDescriptor(poolingDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(srcTensorDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(dstTensorDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(deltaTensorDesc))
				MyBase.destroyHandles()
			End Sub
		End Class

		Private cudnnContext As New CudnnSubsamplingContext()

		Public Overridable Function backpropGradient(ByVal input As INDArray, ByVal epsilon As INDArray, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal pad() As Integer, ByVal poolingType As PoolingType, ByVal convolutionMode As ConvolutionMode, ByVal dilation() As Integer, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray) Implements SubsamplingHelper.backpropGradient
			If dilation(0) <> 1 OrElse dilation(1) <> 1 Then
				'CuDNN doesn't support dilated subsampling
				Return Nothing
			End If

			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			Dim chIdx As Integer = If(nchw, 1, 3)
			Dim hIdx As Integer = If(nchw, 2, 1)
			Dim wIdx As Integer = If(nchw, 3, 2)

			'We require the output as one of the arguments for backprop here
			'TODO we could add cache mode support here somehow...
			Dim reduced As INDArray = activate(input, True, kernel, strides, pad, poolingType, convolutionMode, dilation, format, workspaceMgr)

			Dim miniBatch As val = input.size(0)
			Dim depth As val = input.size(chIdx)

			Dim args As CudnnConvolutionHelper.CudnnForwardArgs = getCudnnForwardArgs(input, kernel, strides, pad, dilation, convolutionMode, poolingType, format)
			input = args.getInput()
			Dim inH As val = input.size(hIdx)
			Dim inW As val = input.size(wIdx)
			Dim srcStride As val = input.stride()
			Dim outSize() As Integer = args.getOutSize()
			Dim outH As Integer = outSize(0)
			Dim outW As Integer = outSize(1)

			'subsampling doesn't have weights and thus gradients are not calculated for this layer
			'only scale and reshape epsilon
			Dim retGradient As Gradient = New DefaultGradient()

			'Epsilons in shape: [miniBatch, channels, outH, outW]
			'Epsilons out shape: [miniBatch, channels, inH, inW]

			Dim poolingMode As Integer
			Select Case poolingType
				Case PoolingType.AVG
					poolingMode = CUDNN_POOLING_AVERAGE_COUNT_INCLUDE_PADDING
				Case PoolingType.MAX
					poolingMode = CUDNN_POOLING_MAX
				Case Else
					Return Nothing
			End Select

			If Not Shape.hasDefaultStridesForShape(epsilon) OrElse epsilon.View Then
				' apparently not supported by cuDNN
				epsilon = epsilon.dup("c"c)
			End If

			input = input.dup()

			Dim deltaStride As val = epsilon.stride()

			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			checkCudnn(cudnnSetTensor4dDescriptorEx(cudnnContext.srcTensorDesc, dataType, CInt(miniBatch), CInt(depth), CInt(inH), CInt(inW), CInt(Math.Truncate(srcStride(0))), CInt(Math.Truncate(srcStride(chIdx))), CInt(Math.Truncate(srcStride(hIdx))), CInt(Math.Truncate(srcStride(wIdx)))))
			checkCudnn(cudnnSetTensor4dDescriptorEx(cudnnContext.deltaTensorDesc, dataType, CInt(miniBatch), CInt(depth), CInt(outH), CInt(outW), CInt(Math.Truncate(deltaStride(0))), CInt(Math.Truncate(deltaStride(chIdx))), CInt(Math.Truncate(deltaStride(hIdx))), CInt(Math.Truncate(deltaStride(wIdx)))))
			checkCudnn(cudnnSetPooling2dDescriptor(cudnnContext.poolingDesc, poolingMode, CUDNN_PROPAGATE_NAN, kernel(0), kernel(1), pad(0), pad(1), strides(0), strides(1)))

			Dim outEpsShape() As Long = If(nchw, New Long() {miniBatch, depth, inH, inW}, New Long()){miniBatch, inH, inW, depth}
			Dim outEpsilon As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, input.dataType(), outEpsShape, "c"c)

			Dim dstStride As val = outEpsilon.stride()
			checkCudnn(cudnnSetTensor4dDescriptorEx(cudnnContext.dstTensorDesc, dataType, CInt(miniBatch), CInt(depth), CInt(inH), CInt(inW), CInt(Math.Truncate(dstStride(0))), CInt(Math.Truncate(dstStride(chIdx))), CInt(Math.Truncate(dstStride(hIdx))), CInt(Math.Truncate(dstStride(wIdx)))))

			Dim allocator As Allocator = AtomicAllocator.Instance
			Dim context As CudaContext = allocator.FlowController.prepareAction(input, epsilon, reduced, outEpsilon)
			Dim srcData As Pointer = allocator.getPointer(input, context)
			Dim epsData As Pointer = allocator.getPointer(epsilon, context)
			Dim zData As Pointer = allocator.getPointer(reduced, context)
			Dim dstData As Pointer = allocator.getPointer(outEpsilon, context)

			checkCudnn(cudnnSetStream(cudnnContext, New CUstream_st(context.CublasStream)))
			checkCudnn(cudnnPoolingBackward(cudnnContext, cudnnContext.poolingDesc, alpha, cudnnContext.deltaTensorDesc, zData, cudnnContext.deltaTensorDesc, epsData, cudnnContext.srcTensorDesc, srcData, beta, cudnnContext.dstTensorDesc, dstData))

			allocator.registerAction(context, outEpsilon, input, epsilon, reduced)

			If CudaEnvironment.Instance.Configuration.isDebug() Then
				context.syncOldStream()
			End If

			'Note that: if we had to manually pad for SAME mode, we have to 'undo' this manual padding for the epsilon
			' we return. The returned epsilon (i.e., dL/dIn array) has to be the same shape as the *original* input.
			If args.isManualPadBottom() OrElse args.isManualPadRight() Then
				If nchw Then
					outEpsilon = outEpsilon.get(all(), all(), interval(0, outEpsilon.size(2) - (If(args.isManualPadBottom(), 1, 0))), interval(0, outEpsilon.size(3) - (If(args.isManualPadRight(), 1, 0))))
				Else
					outEpsilon = outEpsilon.get(all(), interval(0, outEpsilon.size(1) - (If(args.isManualPadBottom(), 1, 0))), interval(0, outEpsilon.size(2) - (If(args.isManualPadRight(), 1, 0))), all())
				End If
			End If

			Return New Pair(Of Gradient, INDArray)(retGradient, outEpsilon)
		End Function


		Public Overridable Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal pad() As Integer, ByVal poolingType As PoolingType, ByVal convolutionMode As ConvolutionMode, ByVal dilation() As Integer, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements SubsamplingHelper.activate
			If dilation(0) <> 1 OrElse dilation(1) <> 1 Then
				'CuDNN doesn't support dilated subsampling
				Return Nothing
			End If

			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			Dim chIdx As Integer = If(nchw, 1, 3)
			Dim hIdx As Integer = If(nchw, 2, 1)
			Dim wIdx As Integer = If(nchw, 3, 2)

			Dim miniBatch As val = input.size(0)
			Dim inDepth As val = input.size(If(nchw, 1, 3))

			Dim args As CudnnConvolutionHelper.CudnnForwardArgs = getCudnnForwardArgs(input, kernel, strides, pad, dilation, convolutionMode, poolingType, format)
			input = args.getInput()
			Dim inH As val = input.size(If(nchw, 2, 1))
			Dim inW As val = input.size(If(nchw, 3, 2))
			Dim srcStride As val = input.stride()
			Dim outSize As val = args.getOutSize()
			Dim outH As Integer = outSize(0)
			Dim outW As Integer = outSize(1)


			Dim poolingMode As Integer
			Select Case poolingType
				Case PoolingType.AVG
					poolingMode = CUDNN_POOLING_AVERAGE_COUNT_INCLUDE_PADDING
				Case PoolingType.MAX
					poolingMode = CUDNN_POOLING_MAX
				Case Else
					Return Nothing
			End Select

			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			checkCudnn(cudnnSetPooling2dDescriptor(cudnnContext.poolingDesc, poolingMode, CUDNN_PROPAGATE_NAN, kernel(0), kernel(1), pad(0), pad(1), strides(0), strides(1)))
			checkCudnn(cudnnSetTensor4dDescriptorEx(cudnnContext.srcTensorDesc, dataType, CInt(miniBatch), CInt(inDepth), CInt(inH), CInt(inW), CInt(Math.Truncate(srcStride(0))), CInt(Math.Truncate(srcStride(chIdx))), CInt(Math.Truncate(srcStride(hIdx))), CInt(Math.Truncate(srcStride(wIdx)))))

			Dim outShape() As Long = If(nchw, New Long() {miniBatch, inDepth, outH, outW}, New Long()){miniBatch, outH, outW, inDepth}
			Dim reduced As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, input.dataType(), outShape, "c"c)

			Dim dstStride As val = reduced.stride()
			checkCudnn(cudnnSetTensor4dDescriptorEx(cudnnContext.dstTensorDesc, dataType, CInt(miniBatch), CInt(inDepth), CInt(outH), CInt(outW), CInt(Math.Truncate(dstStride(0))), CInt(Math.Truncate(dstStride(chIdx))), CInt(Math.Truncate(dstStride(hIdx))), CInt(Math.Truncate(dstStride(wIdx)))))

			Dim allocator As Allocator = AtomicAllocator.Instance
			Dim context As CudaContext = allocator.FlowController.prepareAction(input, reduced)
			Dim srcData As Pointer = allocator.getPointer(input, context)
			Dim dstData As Pointer = allocator.getPointer(reduced, context)

			checkCudnn(cudnnSetStream(cudnnContext, New CUstream_st(context.CublasStream)))
			checkCudnn(cudnnPoolingForward(cudnnContext, cudnnContext.poolingDesc, alpha, cudnnContext.srcTensorDesc, srcData, beta, cudnnContext.dstTensorDesc, dstData))

			allocator.registerAction(context, reduced, input)

			If CudaEnvironment.Instance.Configuration.isDebug() Then
				context.syncOldStream()
			End If

			Return reduced
		End Function

		Public Overridable Function helperMemoryUse() As IDictionary(Of String, Long)
			'No persistent memory use other than the structs (which are small)
			Return Collections.emptyMap()
		End Function

	End Class

End Namespace