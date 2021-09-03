Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BinaryByteUnit = com.jakewharton.byteunits.BinaryByteUnit
Imports org.bytedeco.javacpp
Imports DropoutHelper = org.deeplearning4j.nn.conf.dropout.DropoutHelper
Imports BaseCudnnHelper = org.deeplearning4j.cuda.BaseCudnnHelper
Imports Allocator = org.nd4j.jita.allocator.Allocator
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports CudaEnvironment = org.nd4j.jita.conf.CudaEnvironment
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext
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

Namespace org.deeplearning4j.cuda.dropout


	''' <summary>
	''' CuDNN dropout helper
	''' 
	''' Note that for repeatability between calls (for example, for gradient checks), we need to do two things:
	''' (a) set the ND4J RNG seed
	''' (b) clear the rngStates field
	''' 
	''' @author Alex Black
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Slf4j public class CudnnDropoutHelper extends org.deeplearning4j.cuda.BaseCudnnHelper implements org.deeplearning4j.nn.conf.dropout.DropoutHelper
	Public Class CudnnDropoutHelper
		Inherits BaseCudnnHelper
		Implements DropoutHelper

		Private Class CudnnDropoutContext
			Inherits CudnnContext

			Private Class Deallocator
				Inherits CudnnDropoutContext
				Implements Pointer.Deallocator

				Friend Sub New(ByVal c As CudnnDropoutContext)
					MyBase.New(c)
				End Sub

				Public Overrides Sub deallocate()
					destroyHandles()
				End Sub
			End Class

			Friend xTensorDesc As New cudnnTensorStruct() 'Input
			Friend dxTensorDesc As New cudnnTensorStruct() 'Grad at input
			Friend yTensorDesc As New cudnnTensorStruct() 'Output
			Friend dyTensorDesc As New cudnnTensorStruct() 'Grad at output
			Friend dropoutDesc As New cudnnDropoutStruct()

			Public Sub New()
				createHandles()
				deallocator(New Deallocator(Me))
			End Sub

			Public Sub New(ByVal c As CudnnDropoutContext)
				MyBase.New(c)
				xTensorDesc = New cudnnTensorStruct(c.xTensorDesc)
				dxTensorDesc = New cudnnTensorStruct(c.dxTensorDesc)
				yTensorDesc = New cudnnTensorStruct(c.yTensorDesc)
				dyTensorDesc = New cudnnTensorStruct(c.dyTensorDesc)
				dropoutDesc = New cudnnDropoutStruct(c.dropoutDesc)
			End Sub

			Protected Friend Overrides Sub createHandles()
				MyBase.createHandles()
				checkCudnn(cudnnCreateTensorDescriptor(xTensorDesc))
				checkCudnn(cudnnCreateTensorDescriptor(dxTensorDesc))
				checkCudnn(cudnnCreateTensorDescriptor(yTensorDesc))
				checkCudnn(cudnnCreateTensorDescriptor(dyTensorDesc))
				checkCudnn(cudnnCreateDropoutDescriptor(dropoutDesc))
			End Sub

			Protected Friend Overrides Sub destroyHandles()
				checkCudnn(cudnnDestroyTensorDescriptor(xTensorDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(dxTensorDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(yTensorDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(dyTensorDesc))
				checkCudnn(cudnnDestroyDropoutDescriptor(dropoutDesc))
				MyBase.destroyHandles()
			End Sub
		End Class

		Private cudnnContext As New CudnnDropoutContext()
		Private initializedDescriptor As Boolean = False
		Private rngStates As DataCache '"Pointer to user-allocated GPU memory that will hold random number generator states."
		Private mask As DataCache 'Mask: persistence between forward and backward
		Private stateSizeBytesPtr As SizeTPointer
		Private reserveSizeBytesPtr As SizeTPointer
		Private lastInitializedP As Single

		Public Sub New(ByVal dataType As DataType)
			MyBase.New(dataType)
		End Sub

		Public Overridable Function helperMemoryUse() As IDictionary(Of String, Long)
			Return Collections.emptyMap()
		End Function

		Public Overrides Function checkSupported() As Boolean Implements DropoutHelper.checkSupported
			Return True
		End Function

		Public Overridable Sub applyDropout(ByVal input As INDArray, ByVal resultArray As INDArray, ByVal dropoutInputRetainProb As Double) Implements DropoutHelper.applyDropout
			Dim p As Single = CSng(1.0 - dropoutInputRetainProb) 'CuDNN uses p = probability of setting to 0. We use p = probability of retaining

			'TODO int cast
			Dim inShape() As Integer = adaptForTensorDescr(ArrayUtil.toInts(input.shape()))
			Dim inStride() As Integer = adaptForTensorDescr(ArrayUtil.toInts(input.stride()))
			checkCudnn(cudnnSetTensorNdDescriptor(cudnnContext.xTensorDesc, dataType, inShape.Length, inShape, inStride))

			Dim outShape() As Integer = adaptForTensorDescr(ArrayUtil.toInts(resultArray.shape()))
			Dim outStride() As Integer = adaptForTensorDescr(ArrayUtil.toInts(resultArray.stride()))
			checkCudnn(cudnnSetTensorNdDescriptor(cudnnContext.yTensorDesc, dataType, outShape.Length, outShape, outStride))


			If stateSizeBytesPtr Is Nothing Then
				stateSizeBytesPtr = New SizeTPointer(1)
				reserveSizeBytesPtr = New SizeTPointer(1)
			End If
			checkCudnn(cudnnDropoutGetStatesSize(cudnnContext, stateSizeBytesPtr))
			Dim rngStateSizeBytes As Long = stateSizeBytesPtr.get()
			checkCudnn(cudnnDropoutGetReserveSpaceSize(cudnnContext.xTensorDesc, reserveSizeBytesPtr))
			Dim maskReserveSizeBytes As Long = reserveSizeBytesPtr.get()

			If rngStates Is Nothing OrElse rngStates.capacity() < rngStateSizeBytes Then
				If log.isTraceEnabled() Then
					If rngStates Is Nothing Then
						log.trace("CudnnDropoutHelper: Allocating intial RNG states workspace of size {} ({})", rngStateSizeBytes, BinaryByteUnit.format(rngStateSizeBytes, "#.00"))
					Else
						log.trace("CudnnDropoutHelper: Deallocating RNG states of size {} ({}), allocating new workspace of size {} ({})", rngStates.capacity(), BinaryByteUnit.format(rngStates.capacity(), "#.00"), rngStateSizeBytes, BinaryByteUnit.format(rngStateSizeBytes, "#.00"))
					End If
				End If

				If rngStates IsNot Nothing Then
					rngStates.deallocate()
				End If
				'states = "Pointer to user-allocated GPU memory that will hold random number generator states."
				rngStates = New DataCache(rngStateSizeBytes)
				initializedDescriptor = False
			End If
			If mask Is Nothing OrElse mask.capacity() < maskReserveSizeBytes Then
				If log.isTraceEnabled() Then
					If mask Is Nothing Then
						log.trace("CudnnDropoutHelper: Allocating intial mask array of size {} ({})", maskReserveSizeBytes, BinaryByteUnit.format(maskReserveSizeBytes, "#.00"))
					Else
						log.trace("CudnnDropoutHelper: Deallocating mask array of size {} ({}), allocating new mask array of size {} ({})", mask.capacity(), BinaryByteUnit.format(mask.capacity(), "#.00"), maskReserveSizeBytes, BinaryByteUnit.format(maskReserveSizeBytes, "#.00"))
					End If
				End If

				If mask IsNot Nothing Then
					mask.deallocate()
				End If
				'mask = "Pointer to user-allocated GPU memory used by this function. It is expected
				'that contents of reserveSpace doe not change between cudnnDropoutForward and
				'cudnnDropoutBackward calls."
				mask = New DataCache(maskReserveSizeBytes)
			End If

			'Dropout descriptor: (re)initialize if required
			If Not initializedDescriptor OrElse p <> lastInitializedP Then
				If log.isTraceEnabled() Then
					log.trace("CudnnDropoutHelper: (re)initializing dropout descriptor")
				End If
				'NOTE: cudnnSetDropoutDescriptor has some internal computation/initialization, and hence is expensive to
				' call - so we want to call this as infrequently as possible, and cache the result
				Dim seed As Long = Nd4j.Random.nextLong()
				lastInitializedP = p
				checkCudnn(cudnnSetDropoutDescriptor(cudnnContext.dropoutDesc, cudnnContext, p, rngStates, rngStates.capacity(), seed))
				initializedDescriptor = True
			End If

			Dim allocator As Allocator = AtomicAllocator.Instance
			Dim context As CudaContext = allocator.FlowController.prepareAction(input, resultArray)
			Dim xPtr As Pointer = allocator.getPointer(input, context)
			Dim yPtr As Pointer = allocator.getPointer(resultArray, context)

			checkCudnn(cudnnSetStream(cudnnContext, New CUstream_st(context.CublasStream)))
			checkCudnn(cudnnDropoutForward(cudnnContext, cudnnContext.dropoutDesc, cudnnContext.xTensorDesc, xPtr, cudnnContext.yTensorDesc, yPtr, mask, mask.capacity()))

			allocator.registerAction(context, input, resultArray)
			If CudaEnvironment.Instance.Configuration.isDebug() Then
				context.syncOldStream()
			End If
		End Sub

		Public Overridable Sub backprop(ByVal gradAtOutput As INDArray, ByVal gradAtInput As INDArray) Implements DropoutHelper.backprop
			Dim gradAtOutShape() As Integer = adaptForTensorDescr(ArrayUtil.toInts(gradAtOutput.shape()))
			Dim gradAtOutStride() As Integer = adaptForTensorDescr(ArrayUtil.toInts(gradAtOutput.stride()))
			checkCudnn(cudnnSetTensorNdDescriptor(cudnnContext.dyTensorDesc, dataType, gradAtOutShape.Length, gradAtOutShape, gradAtOutStride))

			Dim gradAtInShape() As Integer = adaptForTensorDescr(ArrayUtil.toInts(gradAtInput.shape()))
			Dim gradAtInStride() As Integer = adaptForTensorDescr(ArrayUtil.toInts(gradAtInput.stride()))
			checkCudnn(cudnnSetTensorNdDescriptor(cudnnContext.dxTensorDesc, dataType, gradAtInShape.Length, gradAtInShape, gradAtInStride))

			Dim allocator As Allocator = AtomicAllocator.Instance
			Dim context As CudaContext = allocator.FlowController.prepareAction(gradAtOutput, gradAtInput)
			Dim dyPtr As Pointer = allocator.getPointer(gradAtOutput, context)
			Dim dxPtr As Pointer = allocator.getPointer(gradAtInput, context)

			checkCudnn(cudnnDropoutBackward(cudnnContext, cudnnContext.dropoutDesc, cudnnContext.dyTensorDesc, dyPtr, cudnnContext.dxTensorDesc, dxPtr, mask, mask.capacity()))

			allocator.registerAction(context, gradAtOutput, gradAtInput)
			If CudaEnvironment.Instance.Configuration.isDebug() Then
				context.syncOldStream()
			End If
		End Sub
	End Class

End Namespace