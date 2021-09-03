Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports BaseCudnnHelper = org.deeplearning4j.cuda.BaseCudnnHelper
Imports BatchNormalizationHelper = org.deeplearning4j.nn.layers.normalization.BatchNormalizationHelper
Imports BatchNormalizationParamInitializer = org.deeplearning4j.nn.params.BatchNormalizationParamInitializer
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Allocator = org.nd4j.jita.allocator.Allocator
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports CudaEnvironment = org.nd4j.jita.conf.CudaEnvironment
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports GridExecutioner = org.nd4j.linalg.api.ops.executioner.GridExecutioner
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext
Imports org.nd4j.common.primitives
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
	''' cuDNN-based helper for the batch normalization layer.
	''' 
	''' @author saudet
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CudnnBatchNormalizationHelper extends org.deeplearning4j.cuda.BaseCudnnHelper implements org.deeplearning4j.nn.layers.normalization.BatchNormalizationHelper
	Public Class CudnnBatchNormalizationHelper
		Inherits BaseCudnnHelper
		Implements BatchNormalizationHelper

		Public Sub New(ByVal dataType As DataType)
			MyBase.New(dataType)
		End Sub

		Private Class CudnnBatchNormalizationContext
			Inherits CudnnContext

			Private Class Deallocator
				Inherits CudnnBatchNormalizationContext
				Implements Pointer.Deallocator

				Friend Sub New(ByVal c As CudnnBatchNormalizationContext)
					MyBase.New(c)
				End Sub

				Public Overrides Sub deallocate()
					destroyHandles()
				End Sub
			End Class

			Friend srcTensorDesc As New cudnnTensorStruct(), dstTensorDesc As New cudnnTensorStruct(), deltaTensorDesc As New cudnnTensorStruct(), gammaBetaTensorDesc As New cudnnTensorStruct()

			Public Sub New()
				createHandles()
				deallocator(New Deallocator(Me))
			End Sub

			Public Sub New(ByVal c As CudnnBatchNormalizationContext)
				MyBase.New(c)
				srcTensorDesc = New cudnnTensorStruct(c.srcTensorDesc)
				dstTensorDesc = New cudnnTensorStruct(c.dstTensorDesc)
				deltaTensorDesc = New cudnnTensorStruct(c.deltaTensorDesc)
				gammaBetaTensorDesc = New cudnnTensorStruct(c.gammaBetaTensorDesc)
			End Sub

			Protected Friend Overrides Sub createHandles()
				MyBase.createHandles()
				checkCudnn(cudnnCreateTensorDescriptor(srcTensorDesc))
				checkCudnn(cudnnCreateTensorDescriptor(dstTensorDesc))
				checkCudnn(cudnnCreateTensorDescriptor(deltaTensorDesc))
				checkCudnn(cudnnCreateTensorDescriptor(gammaBetaTensorDesc))
			End Sub

			Protected Friend Overrides Sub destroyHandles()
				checkCudnn(cudnnDestroyTensorDescriptor(srcTensorDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(dstTensorDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(deltaTensorDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(gammaBetaTensorDesc))
				MyBase.destroyHandles()
			End Sub
		End Class

		Protected Friend ReadOnly batchNormMode As Integer = CUDNN_BATCHNORM_SPATIAL ' would need to increase rank of gamma and beta for CUDNN_BATCHNORM_PER_ACTIVATION

		Private cudnnContext As New CudnnBatchNormalizationContext()
		Private meanCache As INDArray
		Private varCache As INDArray
		Private eps As Double

		Public Overridable Overloads Function checkSupported(ByVal eps As Double, ByVal isFixedGammaBeta As Boolean) As Boolean Implements BatchNormalizationHelper.checkSupported
			Dim supported As Boolean = checkSupported()
			If eps < CUDNN_BN_MIN_EPSILON Then
				supported = False
				log.warn("Not supported: eps < CUDNN_BN_MIN_EPSILON (" & eps & " < " & CUDNN_BN_MIN_EPSILON & ")")
			End If
			Return supported
		End Function

		Public Overridable Function backpropGradient(ByVal input As INDArray, ByVal epsilon As INDArray, ByVal shape() As Long, ByVal gamma As INDArray, ByVal beta As INDArray, ByVal dGammaView As INDArray, ByVal dBetaView As INDArray, ByVal eps As Double, ByVal format As CNN2DFormat, ByVal layerWorkspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray) Implements BatchNormalizationHelper.backpropGradient

			Dim nchw As Boolean = format = CNN2DFormat.NCHW

			Me.eps = eps

			Dim cudnnTensorFormat As Integer = If(nchw, CUDNN_TENSOR_NCHW, CUDNN_TENSOR_NHWC)
			Dim chIdx As Integer = If(nchw, 1, 3)
			Dim hIdx As Integer = If(nchw, 2, 1)
			Dim wIdx As Integer = If(nchw, 3, 2)

			Dim miniBatch As val = CInt(input.size(0))
			Dim depth As val = CInt(input.size(chIdx))
			Dim inH As val = CInt(input.size(hIdx))
			Dim inW As val = CInt(input.size(wIdx))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final boolean isHalf = (input.dataType() == org.nd4j.linalg.api.buffer.DataType.HALF);
			Dim isHalf As Boolean = (input.dataType() = DataType.HALF)
			Dim gammaOrig As INDArray = Nothing
			Dim dGammaViewOrig As INDArray = Nothing
			Dim dBetaViewOrig As INDArray = Nothing
			If isHalf Then 'Convert FP16 to FP32 if required (CuDNN BN doesn't support FP16 for these params, only for input/output)
				gammaOrig = gamma
				dGammaViewOrig = dGammaView
				dBetaViewOrig = dBetaView
	'            
	'            From CuDNN docs: bnScale, resultBnScaleDiff, resultBnBiasDiff, savedMean, savedInvVariance
	'            "Note: The data type of this tensor descriptor must be 'float' for FP16 and FP32 input tensors, and 'double'
	'            for FP64 input tensors."
	'            >> Last 2 are the meanCache and varCache; first 3 are below
	'             
				gamma = gamma.castTo(DataType.FLOAT)
				dGammaView = dGammaView.castTo(DataType.FLOAT)
				dBetaView = dBetaView.castTo(DataType.FLOAT)
			End If

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

			checkCudnn(cudnnSetTensor4dDescriptorEx(cudnnContext.srcTensorDesc, dataType, CInt(miniBatch), CInt(depth), CInt(inH), CInt(inW), CInt(Math.Truncate(srcStride(0))), CInt(Math.Truncate(srcStride(chIdx))), CInt(Math.Truncate(srcStride(hIdx))), CInt(Math.Truncate(srcStride(wIdx)))))
			checkCudnn(cudnnSetTensor4dDescriptorEx(cudnnContext.deltaTensorDesc, dataType, CInt(miniBatch), CInt(depth), CInt(inH), CInt(inW), CInt(Math.Truncate(deltaStride(0))), CInt(Math.Truncate(deltaStride(chIdx))), CInt(Math.Truncate(deltaStride(hIdx))), CInt(Math.Truncate(deltaStride(wIdx)))))

			Dim nextEpsShape() As Long = If(nchw, New Long() {miniBatch, depth, inH, inW}, New Long()){miniBatch, inH, inW, depth}
			Dim nextEpsilon As INDArray = layerWorkspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, input.dataType(), nextEpsShape, "c"c)
			Dim dstStride As val = ArrayUtil.toInts(nextEpsilon.stride())

			checkCudnn(cudnnSetTensor4dDescriptorEx(cudnnContext.dstTensorDesc, dataType, miniBatch, depth, inH, inW, dstStride(0), dstStride(chIdx), dstStride(hIdx), dstStride(wIdx)))
			checkCudnn(cudnnSetTensor4dDescriptor(cudnnContext.gammaBetaTensorDesc, cudnnTensorFormat, toCudnnDataType(gamma.data().dataType()), CInt(shape(0)), CInt(shape(1)),If(shape.Length > 2, CInt(shape(2)), 1),If(shape.Length > 3, CInt(shape(3)), 1)))

			Dim allocator As Allocator = AtomicAllocator.Instance
			Dim context As CudaContext = allocator.FlowController.prepareActionAllWrite(input, epsilon, nextEpsilon, gamma, dGammaView, dBetaView)
			Dim srcData As Pointer = allocator.getPointer(input, context)
			Dim epsData As Pointer = allocator.getPointer(epsilon, context)
			Dim dstData As Pointer = allocator.getPointer(nextEpsilon, context)
			Dim gammaData As Pointer = allocator.getPointer(gamma, context)
			Dim dGammaData As Pointer = allocator.getPointer(dGammaView, context)
			Dim dBetaData As Pointer = allocator.getPointer(dBetaView, context)
			Dim meanCacheData As Pointer = allocator.getPointer(meanCache, context)
			Dim varCacheData As Pointer = allocator.getPointer(varCache, context)

			checkCudnn(cudnnSetStream(cudnnContext, New CUstream_st(context.CublasStream)))
			checkCudnn(cudnnBatchNormalizationBackward(cudnnContext, batchNormMode, alpha, Me.beta, alpha, alpha, cudnnContext.srcTensorDesc, srcData, cudnnContext.deltaTensorDesc, epsData, cudnnContext.dstTensorDesc, dstData, cudnnContext.gammaBetaTensorDesc, gammaData, dGammaData, dBetaData, eps, meanCacheData, varCacheData))

			allocator.FlowController.registerActionAllWrite(context, input, epsilon, nextEpsilon, gamma, dGammaView, dBetaView)

			retGradient.setGradientFor(BatchNormalizationParamInitializer.GAMMA, dGammaView)
			retGradient.setGradientFor(BatchNormalizationParamInitializer.BETA, dBetaView)

			context.syncOldStream()

			'Convert back and assign, if required:
			If isHalf Then
				gammaOrig.assign(gamma.castTo(DataType.HALF))
				dGammaViewOrig.assign(dGammaView.castTo(DataType.HALF))
				dBetaViewOrig.assign(dBetaView.castTo(DataType.HALF))
			End If

			Return New Pair(Of Gradient, INDArray)(retGradient, nextEpsilon)
		End Function


		Public Overridable Function preOutput(ByVal x As INDArray, ByVal training As Boolean, ByVal shape() As Long, ByVal gamma As INDArray, ByVal beta As INDArray, ByVal mean As INDArray, ByVal var As INDArray, ByVal decay As Double, ByVal eps As Double, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements BatchNormalizationHelper.preOutput
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			Dim cudnnTensorFormat As Integer = If(nchw, CUDNN_TENSOR_NCHW, CUDNN_TENSOR_NHWC)
			Dim chIdx As Integer = If(nchw, 1, 3)
			Dim hIdx As Integer = If(nchw, 2, 1)
			Dim wIdx As Integer = If(nchw, 3, 2)

			Me.eps = eps
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final boolean isHalf = (x.dataType() == org.nd4j.linalg.api.buffer.DataType.FLOAT16);
			Dim isHalf As Boolean = (x.dataType() = DataType.FLOAT16)
			Dim origGamma As INDArray = gamma
			Dim origBeta As INDArray = beta
			Dim origMean As INDArray = mean
			Dim origVar As INDArray = var
			If isHalf Then
				gamma = gamma.castTo(DataType.FLOAT)
				beta = beta.castTo(DataType.FLOAT)
				mean = mean.castTo(DataType.FLOAT)
				var = var.castTo(DataType.FLOAT)
			End If

			'Notation difference between CuDNN and our implementation:
			'Us:       runningMean = (1-decay) * batchMean + decay * runningMean
			'CuDNN:    runningMean = decay * batchMean + (1-decay) * runningMean
			'i.e., "decay" has a different meaning...
			'Disable in-place updating of running mean/variance, so that all parameter changes are done via the update/gradient
			' vector. This is necessary for BatchNormalization to be safe to use in distributed gradient sharing settings
			decay = 0.0 'From cudnn docs: runningMean = newMean*factor + runningMean*(1-factor). -> 0 = "in-place modification of running mean disabled"

			Dim miniBatch As val = CInt(x.size(0))
			Dim inDepth As val = CInt(x.size(chIdx))
			Dim inH As val = CInt(x.size(hIdx))
			Dim inW As val = CInt(x.size(wIdx))

			Dim srcStride As val = ArrayUtil.toInts(x.stride())
			checkCudnn(cudnnSetTensor4dDescriptorEx(cudnnContext.srcTensorDesc, dataType, miniBatch, inDepth, inH, inW, srcStride(0), srcStride(chIdx), srcStride(hIdx), srcStride(wIdx)))

			Dim actShape() As Long = If(nchw, New Long() {miniBatch, inDepth, inH, inW}, New Long()){miniBatch, inH, inW, inDepth}
			Dim activations As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, x.dataType(), actShape, "c"c)

			Dim dstStride As val = ArrayUtil.toInts(activations.stride())
			checkCudnn(cudnnSetTensor4dDescriptorEx(cudnnContext.dstTensorDesc, dataType, miniBatch, inDepth, inH, inW, dstStride(0), dstStride(chIdx), dstStride(hIdx), dstStride(wIdx)))

			checkCudnn(cudnnSetTensor4dDescriptor(cudnnContext.gammaBetaTensorDesc, cudnnTensorFormat, toCudnnDataType(mean.data().dataType()), CInt(shape(0)), CInt(shape(1)),If(shape.Length > 2, CInt(shape(2)), 1),If(shape.Length > 3, CInt(shape(3)), 1)))

			Dim allocator As Allocator = AtomicAllocator.Instance
			Dim context As CudaContext = allocator.FlowController.prepareActionAllWrite(x, activations, gamma, beta, mean, var)
			Dim srcData As Pointer = allocator.getPointer(x, context)
			Dim dstData As Pointer = allocator.getPointer(activations, context)
			Dim gammaData As Pointer = allocator.getPointer(gamma, context)
			Dim betaData As Pointer = allocator.getPointer(beta, context)
			Dim meanData As Pointer = allocator.getPointer(mean, context)
			Dim varData As Pointer = allocator.getPointer(var, context)

			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			checkCudnn(cudnnSetStream(cudnnContext, New CUstream_st(context.CublasStream)))
			If training Then
				If meanCache Is Nothing OrElse meanCache.length() < mean.length() Then
					Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
						meanCache = Nd4j.createUninitialized(x.dataType(), mean.length())
					End Using
					If x.dataType() = DataType.HALF Then
						Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
							meanCache = meanCache.castTo(DataType.FLOAT)
						End Using
					End If
				End If
				If varCache Is Nothing OrElse varCache.length() < mean.length() Then
					Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
						varCache = Nd4j.createUninitialized(x.dataType(), mean.length())
					End Using
					If nd4jDataType = DataType.HALF Then
						Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
							varCache = varCache.castTo(DataType.FLOAT)
						End Using
					End If
				End If
				Dim meanCacheData As Pointer = allocator.getPointer(meanCache, context)
				Dim varCacheData As Pointer = allocator.getPointer(varCache, context)

				checkCudnn(cudnnBatchNormalizationForwardTraining(cudnnContext, batchNormMode, Me.alpha, Me.beta, cudnnContext.srcTensorDesc, srcData, cudnnContext.dstTensorDesc, dstData, cudnnContext.gammaBetaTensorDesc, gammaData, betaData, decay, meanData, varData, eps, meanCacheData, varCacheData))
			Else
				checkCudnn(cudnnBatchNormalizationForwardInference(cudnnContext, batchNormMode, Me.alpha, Me.beta, cudnnContext.srcTensorDesc, srcData, cudnnContext.dstTensorDesc, dstData, cudnnContext.gammaBetaTensorDesc, gammaData, betaData, meanData, varData, eps))
			End If

			allocator.FlowController.registerActionAllWrite(context, x, activations, gamma, beta, mean, var)

			If CudaEnvironment.Instance.Configuration.isDebug() Then
				context.syncOldStream()
			End If

			context.syncOldStream()
			If training Then
				AtomicAllocator.Instance.getAllocationPoint(meanCache).tickDeviceWrite()
				AtomicAllocator.Instance.getAllocationPoint(varCache).tickDeviceWrite()
			End If

			If training AndAlso isHalf Then
				'Update the running mean and variance arrays; also gamma/beta
				origMean.assign(mean.castTo(DataType.HALF))
				origVar.assign(var.castTo(DataType.HALF))
				origGamma.assign(gamma.castTo(DataType.HALF))
				origBeta.assign(beta.castTo(DataType.HALF))
			End If

			Return activations
		End Function

		Public Overridable Function getMeanCache(ByVal dataType As DataType) As INDArray Implements BatchNormalizationHelper.getMeanCache
			If dataType = DataType.HALF Then
				'Buffer is FP32
				Return meanCache.castTo(DataType.HALF)
			End If
			Return meanCache
		End Function

		Public Overridable Function getVarCache(ByVal dataType As DataType) As INDArray Implements BatchNormalizationHelper.getVarCache
			Dim ret As INDArray
			If dataType = DataType.HALF Then
				Dim vc As INDArray = varCache.castTo(DataType.HALF)
				ret = vc.mul(vc).rdivi(1.0).subi(eps)
			Else
				ret = varCache.mul(varCache).rdivi(1.0).subi(eps)
			End If
			If dataType = DataType.HALF Then
				'Buffer is FP32
				Return ret.castTo(DataType.HALF)
			End If
			Return ret
		End Function


		Public Overridable Function helperMemoryUse() As IDictionary(Of String, Long)
			Dim memUse As IDictionary(Of String, Long) = New Dictionary(Of String, Long)()
			memUse("meanCache") = If(meanCache Is Nothing, 0, meanCache.length() * meanCache.data().ElementSize)
			memUse("varCache") = If(varCache Is Nothing, 0, varCache.length() * varCache.data().ElementSize)
			Return memUse
		End Function
	End Class

End Namespace