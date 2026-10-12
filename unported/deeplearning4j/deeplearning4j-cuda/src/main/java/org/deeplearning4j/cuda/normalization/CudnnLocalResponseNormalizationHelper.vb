Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports BaseCudnnHelper = org.deeplearning4j.cuda.BaseCudnnHelper
Imports LocalResponseNormalizationHelper = org.deeplearning4j.nn.layers.normalization.LocalResponseNormalizationHelper
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
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports org.bytedeco.cuda.cudart
Imports org.bytedeco.cuda.cudnn
Imports org.bytedeco.cuda.global.cudnn

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

Namespace org.deeplearning4j.cuda.normalization


	''' <summary>
	''' cuDNN-based helper for the local response normalization layer.
	''' 
	''' @author saudet
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CudnnLocalResponseNormalizationHelper extends org.deeplearning4j.cuda.BaseCudnnHelper implements org.deeplearning4j.nn.layers.normalization.LocalResponseNormalizationHelper
	Public Class CudnnLocalResponseNormalizationHelper
		Inherits BaseCudnnHelper
		Implements LocalResponseNormalizationHelper

		Public Sub New(ByVal dataType As DataType)
			MyBase.New(dataType)
		End Sub

		Private Class CudnnLocalResponseNormalizationContext
			Inherits CudnnContext

			Private Class Deallocator
				Inherits CudnnLocalResponseNormalizationContext
				Implements Pointer.Deallocator

				Friend Sub New(ByVal c As CudnnLocalResponseNormalizationContext)
					MyBase.New(c)
				End Sub

				Public Overrides Sub deallocate()
					destroyHandles()
				End Sub
			End Class

			Friend srcTensorDesc As New cudnnTensorStruct(), dstTensorDesc As New cudnnTensorStruct(), deltaTensorDesc As New cudnnTensorStruct()
			Friend lrnDesc As New cudnnLRNStruct()

			Public Sub New()
				createHandles()
				deallocator(New Deallocator(Me))
			End Sub

			Public Sub New(ByVal c As CudnnLocalResponseNormalizationContext)
				MyBase.New(c)
				srcTensorDesc = New cudnnTensorStruct(c.srcTensorDesc)
				dstTensorDesc = New cudnnTensorStruct(c.dstTensorDesc)
				deltaTensorDesc = New cudnnTensorStruct(c.deltaTensorDesc)
				lrnDesc = New cudnnLRNStruct(c.lrnDesc)
			End Sub

			Protected Friend Overrides Sub createHandles()
				MyBase.createHandles()
				checkCudnn(cudnnCreateTensorDescriptor(srcTensorDesc))
				checkCudnn(cudnnCreateTensorDescriptor(dstTensorDesc))
				checkCudnn(cudnnCreateTensorDescriptor(deltaTensorDesc))
				checkCudnn(cudnnCreateLRNDescriptor(lrnDesc))
			End Sub

			Protected Friend Overrides Sub destroyHandles()
				checkCudnn(cudnnDestroyLRNDescriptor(lrnDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(srcTensorDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(dstTensorDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(deltaTensorDesc))
				MyBase.destroyHandles()
			End Sub
		End Class

		Private cudnnContext As New CudnnLocalResponseNormalizationContext()
		Private activations As INDArray = Nothing

		Public Overridable Overloads Function checkSupported(ByVal k As Double, ByVal n As Double, ByVal alpha As Double, ByVal beta As Double) As Boolean Implements LocalResponseNormalizationHelper.checkSupported
			Dim supported As Boolean = checkSupported()
			If n < CUDNN_LRN_MIN_N Then
				supported = False
				log.warn("Not supported: n < CUDNN_LRN_MIN_N (" & n & " < " & CUDNN_LRN_MIN_N & ")")
			End If
			If n > CUDNN_LRN_MAX_N Then
				supported = False
				log.warn("Not supported: n > CUDNN_LRN_MAX_N (" & n & " > " & CUDNN_LRN_MAX_N & ")")
			End If
			If k < CUDNN_LRN_MIN_K Then
				supported = False
				log.warn("Not supported: k < CUDNN_LRN_MIN_K (" & k & " < " & CUDNN_LRN_MIN_K & ")")
			End If
			If beta < CUDNN_LRN_MIN_BETA Then
				supported = False
				log.warn("Not supported: beta < CUDNN_LRN_MIN_BETA (" & beta & " < " & CUDNN_LRN_MIN_BETA & ")")
			End If
			Return supported
		End Function

		Public Overridable Function backpropGradient(ByVal input As INDArray, ByVal epsilon As INDArray, ByVal k As Double, ByVal n As Double, ByVal alpha As Double, ByVal beta As Double, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray) Implements LocalResponseNormalizationHelper.backpropGradient
			Dim miniBatch As val = CInt(input.size(0))
			Dim depth As val = CInt(input.size(1))
			Dim inH As val = CInt(input.size(2))
			Dim inW As val = CInt(input.size(3))

			Dim retGradient As Gradient = New DefaultGradient()

			If Not Shape.hasDefaultStridesForShape(epsilon) Then
				' apparently not supported by cuDNN
				epsilon = epsilon.dup("c"c)
			End If

			Dim srcStride As val = ArrayUtil.toInts(input.stride())
			Dim deltaStride As val = ArrayUtil.toInts(epsilon.stride())

			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			checkCudnn(cudnnSetTensor4dDescriptorEx(cudnnContext.srcTensorDesc, dataType, miniBatch, depth, inH, inW, srcStride(0), srcStride(1), srcStride(2), srcStride(3)))
			checkCudnn(cudnnSetTensor4dDescriptorEx(cudnnContext.deltaTensorDesc, dataType, miniBatch, depth, inH, inW, deltaStride(0), deltaStride(1), deltaStride(2), deltaStride(3)))
			checkCudnn(cudnnSetLRNDescriptor(cudnnContext.lrnDesc, CInt(Math.Truncate(n)), alpha, beta, k))

			Dim nextEpsilon As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, input.dataType(), New Long() {miniBatch, depth, inH, inW}, "c"c)

			Dim dstStride As val = ArrayUtil.toInts(nextEpsilon.stride())
			checkCudnn(cudnnSetTensor4dDescriptorEx(cudnnContext.dstTensorDesc, dataType, miniBatch, depth, inH, inW, dstStride(0), dstStride(1), dstStride(2), dstStride(3)))

			Dim allocator As Allocator = AtomicAllocator.Instance
			Dim context As CudaContext = allocator.FlowController.prepareActionAllWrite(input, epsilon, activations, nextEpsilon)
			Dim srcData As Pointer = allocator.getPointer(input, context)
			Dim epsData As Pointer = allocator.getPointer(epsilon, context)
			Dim zData As Pointer = allocator.getPointer(activations, context)
			Dim dstData As Pointer = allocator.getPointer(nextEpsilon, context)

			checkCudnn(cudnnSetStream(cudnnContext, New CUstream_st(context.CublasStream)))
			checkCudnn(cudnnLRNCrossChannelBackward(cudnnContext, cudnnContext.lrnDesc, CUDNN_LRN_CROSS_CHANNEL_DIM1, Me.alpha, cudnnContext.deltaTensorDesc, zData, cudnnContext.deltaTensorDesc, epsData, cudnnContext.srcTensorDesc, srcData, Me.beta, cudnnContext.dstTensorDesc, dstData))

			allocator.FlowController.registerActionAllWrite(context, input, epsilon, activations, nextEpsilon)

			If CudaEnvironment.Instance.Configuration.isDebug() Then
				context.syncOldStream()
			End If

			Return New Pair(Of Gradient, INDArray)(retGradient, nextEpsilon)
		End Function


		Public Overridable Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal k As Double, ByVal n As Double, ByVal alpha As Double, ByVal beta As Double, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements LocalResponseNormalizationHelper.activate
			Dim miniBatch As val = CInt(input.size(0))
			Dim inDepth As val = CInt(input.size(1))
			Dim inH As val = CInt(input.size(2))
			Dim inW As val = CInt(input.size(3))

			If Not Shape.hasDefaultStridesForShape(input) Then
				input = input.dup("c"c)
			End If

			Dim srcStride As val = ArrayUtil.toInts(input.stride())
			checkCudnn(cudnnSetTensor4dDescriptorEx(cudnnContext.srcTensorDesc, dataType, miniBatch, inDepth, inH, inW, srcStride(0), srcStride(1), srcStride(2), srcStride(3)))

			activations = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, input.dataType(), New Long() {miniBatch, inDepth, inH, inW}, "c"c)

			Dim dstStride As val = ArrayUtil.toInts(activations.stride())
			checkCudnn(cudnnSetTensor4dDescriptorEx(cudnnContext.dstTensorDesc, dataType, miniBatch, inDepth, inH, inW, dstStride(0), dstStride(1), dstStride(2), dstStride(3)))
			checkCudnn(cudnnSetLRNDescriptor(cudnnContext.lrnDesc, CInt(Math.Truncate(n)), alpha, beta, k))

			Dim allocator As Allocator = AtomicAllocator.Instance
			Dim context As CudaContext = allocator.FlowController.prepareActionAllWrite(input, activations)
			Dim srcData As Pointer = allocator.getPointer(input, context)
			Dim dstData As Pointer = allocator.getPointer(activations, context)

			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			checkCudnn(cudnnSetStream(cudnnContext, New CUstream_st(context.CublasStream)))
			checkCudnn(cudnnLRNCrossChannelForward(cudnnContext, cudnnContext.lrnDesc, CUDNN_LRN_CROSS_CHANNEL_DIM1, Me.alpha, cudnnContext.srcTensorDesc, srcData, Me.beta, cudnnContext.dstTensorDesc, dstData))

			allocator.FlowController.registerActionAllWrite(context, input, activations)

			If CudaEnvironment.Instance.Configuration.isDebug() Then
				context.syncOldStream()
			End If

			Return activations
		End Function

		Public Overridable Function helperMemoryUse() As IDictionary(Of String, Long)
			'No persistent memory use other than the structs (which are small)
			Return Collections.emptyMap()
		End Function
	End Class

End Namespace