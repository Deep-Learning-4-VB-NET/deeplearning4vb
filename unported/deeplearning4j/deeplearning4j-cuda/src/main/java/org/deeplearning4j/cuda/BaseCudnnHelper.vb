Imports System
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.bytedeco.javacpp
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.bytedeco.cuda.cudnn
Imports org.bytedeco.cuda.global.cudart
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

Namespace org.deeplearning4j.cuda

	''' <summary>
	''' Functionality shared by all cuDNN-based helpers.
	''' 
	''' @author saudet
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseCudnnHelper
	Public MustInherit Class BaseCudnnHelper

		Protected Friend Shared Sub checkCuda(ByVal [error] As Integer)
			If [error] <> cudaSuccess Then
				Throw New Exception("CUDA error = " & [error] & ": " & cudaGetErrorString([error]).getString())
			End If
		End Sub

		Protected Friend Shared Sub checkCudnn(ByVal status As Integer)
			If status <> CUDNN_STATUS_SUCCESS Then
				Throw New Exception("cuDNN status = " & status & ": " & cudnnGetErrorString(status).getString())
			End If
		End Sub

		Protected Friend Class CudnnContext
			Inherits cudnnContext

			Protected Friend Class Deallocator
				Inherits CudnnContext
				Implements Pointer.Deallocator

				Friend Sub New(ByVal c As CudnnContext)
					MyBase.New(c)
				End Sub

				Public Overrides Sub deallocate()
					destroyHandles()
				End Sub
			End Class

			Public Sub New()
				' insure that cuDNN initializes on the same device as ND4J for this thread
				Nd4j.create(1)
				AtomicAllocator.Instance
				' This needs to be called in subclasses:
				' createHandles();
				' deallocator(new Deallocator(this));
			End Sub

			Public Sub New(ByVal c As CudnnContext)
				MyBase.New(c)
			End Sub

			Protected Friend Overridable Sub createHandles()
				checkCudnn(cudnnCreate(Me))
			End Sub

			Protected Friend Overridable Sub destroyHandles()
				checkCudnn(cudnnDestroy(Me))
			End Sub
		End Class

		Protected Friend Class DataCache
			Inherits Pointer

			Friend Class Deallocator
				Inherits DataCache
				Implements Pointer.Deallocator

				Friend Sub New(ByVal c As DataCache)
					MyBase.New(c)
				End Sub

				Public Overrides Sub deallocate()
					checkCuda(cudaFree(Me))
					setNull()
				End Sub
			End Class

			Friend Class HostDeallocator
				Inherits DataCache
				Implements Pointer.Deallocator

				Friend Sub New(ByVal c As DataCache)
					MyBase.New(c)
				End Sub

				Public Overrides Sub deallocate()
					checkCuda(cudaFreeHost(Me))
					setNull()
				End Sub
			End Class

			Public Sub New()
			End Sub

			Public Sub New(ByVal size As Long)
				position = 0
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: limit = capacity = size;
				capacity = size
					limit = capacity
				Dim [error] As Integer = cudaMalloc(Me, size)
				If [error] <> cudaSuccess Then
					log.warn("Cannot allocate " & size & " bytes of device memory (CUDA error = " & [error] & "), proceeding with host memory")
					checkCuda(cudaMallocHost(Me, size))
					deallocator(New HostDeallocator(Me))
				Else
					deallocator(New Deallocator(Me))
				End If
			End Sub

			Public Sub New(ByVal c As DataCache)
				MyBase.New(c)
			End Sub
		End Class

		Protected Friend Class TensorArray
			Inherits PointerPointer(Of cudnnTensorStruct)

			Friend Class Deallocator
				Inherits TensorArray
				Implements Pointer.Deallocator

				Friend owner As Pointer

				Friend Sub New(ByVal a As TensorArray, ByVal owner As Pointer)
					Me.address = a.address
					Me.capacity = a.capacity
					Me.owner = owner
				End Sub

				Public Overrides Sub deallocate()
					Dim i As Integer = 0
					Do While Not isNull() AndAlso i < capacity
						Dim t As cudnnTensorStruct = Me.get(GetType(cudnnTensorStruct), i)
						checkCudnn(cudnnDestroyTensorDescriptor(t))
						i += 1
					Loop
					If owner IsNot Nothing Then
						owner.deallocate()
						owner = Nothing
					End If
					setNull()
				End Sub
			End Class

			Public Sub New()
			End Sub

			Public Sub New(ByVal size As Long)
				Dim p As New PointerPointer(size)
				p.deallocate(False)
				Me.address = p.address()
				Me.limit = p.limit()
				Me.capacity = p.capacity()

				Dim t As New cudnnTensorStruct()
				For i As Integer = 0 To capacity - 1
					checkCudnn(cudnnCreateTensorDescriptor(t))
					Me.put(i, t)
				Next i
				deallocator(New Deallocator(Me, p))
			End Sub

			Public Sub New(ByVal a As TensorArray)
				MyBase.New(a)
			End Sub
		End Class

		Protected Friend ReadOnly nd4jDataType As DataType
		Protected Friend ReadOnly dataType As Integer
		Protected Friend ReadOnly dataTypeSize As Integer
		' both CUDNN_DATA_HALF and CUDNN_DATA_FLOAT need a float value for alpha and beta
		Protected Friend ReadOnly alpha As Pointer
		Protected Friend ReadOnly beta As Pointer
		Protected Friend sizeInBytes As New SizeTPointer(1)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BaseCudnnHelper(@NonNull DataType dataType)
		Public Sub New(ByVal dataType As DataType)
			Me.nd4jDataType = dataType
			Me.dataType = If(dataType = DataType.DOUBLE, CUDNN_DATA_DOUBLE, If(dataType = DataType.FLOAT, CUDNN_DATA_FLOAT, CUDNN_DATA_HALF))
			Me.dataTypeSize = If(dataType = DataType.DOUBLE, 8, If(dataType = DataType.FLOAT, 4, 2))
			' both CUDNN_DATA_HALF and CUDNN_DATA_FLOAT need a float value for alpha and beta
			Me.alpha = If(Me.dataType = CUDNN_DATA_DOUBLE, New DoublePointer(1.0), New FloatPointer(1.0f))
			Me.beta = If(Me.dataType = CUDNN_DATA_DOUBLE, New DoublePointer(0.0), New FloatPointer(0.0f))
		End Sub

		Public Shared Function toCudnnDataType(ByVal type As DataType) As Integer
			Select Case type.innerEnumValue
				Case DataType.InnerEnum.DOUBLE
					Return CUDNN_DATA_DOUBLE
				Case DataType.InnerEnum.FLOAT
					Return CUDNN_DATA_FLOAT
				Case DataType.InnerEnum.INT
					Return CUDNN_DATA_INT32
				Case DataType.InnerEnum.HALF
					Return CUDNN_DATA_HALF
				Case Else
					Throw New Exception("Cannot convert type: " & type)
			End Select
		End Function

		Public Overridable Function checkSupported() As Boolean
			' add general checks here, if any
			Return True
		End Function


		''' <summary>
		''' From CuDNN documentation -
		''' "Tensors are restricted to having at least 4 dimensions... When working with lower dimensional data, it is
		''' recommended that the user create a 4Dtensor, and set the size along unused dimensions to 1."
		''' 
		''' This method implements that - basically appends 1s to the end (shape or stride) to make it length 4,
		''' or leaves it unmodified if the length is already 4 or more.
		''' This method can be used for both shape and strides
		''' </summary>
		''' <param name="shapeOrStrides">
		''' @return </param>
		Protected Friend Shared Function adaptForTensorDescr(ByVal shapeOrStrides() As Integer) As Integer()
			If shapeOrStrides.Length >= 4 Then
				Return shapeOrStrides
			End If
			Dim [out](3) As Integer
			Dim i As Integer=0
			Do While i<shapeOrStrides.Length
				[out](i) = shapeOrStrides(i)
				i += 1
			Loop
			Do While i<4
				[out](i) = 1
				i += 1
			Loop
			Return [out]
		End Function
	End Class

End Namespace