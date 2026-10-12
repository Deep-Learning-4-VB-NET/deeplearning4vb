Imports System
Imports lombok
Imports LongPointer = org.bytedeco.javacpp.LongPointer
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports PointerPointer = org.bytedeco.javacpp.PointerPointer
Imports GarbageResourceReference = org.nd4j.jita.allocator.garbage.GarbageResourceReference
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports CudaPointer = org.nd4j.jita.allocator.pointers.CudaPointer
Imports cublasHandle_t = org.nd4j.jita.allocator.pointers.cuda.cublasHandle_t
Imports cudaStream_t = org.nd4j.jita.allocator.pointers.cuda.cudaStream_t
Imports cusolverDnHandle_t = org.nd4j.jita.allocator.pointers.cuda.cusolverDnHandle_t
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports CublasPointer = org.nd4j.linalg.jcublas.CublasPointer
Imports NativeOps = org.nd4j.nativeblas.NativeOps
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder

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

Namespace org.nd4j.linalg.jcublas.context


	''' <summary>
	''' A higher level class for handling
	''' the different primitives around the cuda apis
	''' This being:
	''' streams (both old and new) as well as
	''' the cublas handles.
	''' 
	''' 
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @AllArgsConstructor @NoArgsConstructor @Builder public class CudaContext
	Public Class CudaContext

		' execution stream
		Private oldStream As cudaStream_t

		' memcpy stream
		Private specialStream As cudaStream_t

		' exactly what it says
'JAVA TO VB CONVERTER NOTE: The field cublasHandle was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private cublasHandle_Conflict As cublasHandle_t
'JAVA TO VB CONVERTER NOTE: The field solverHandle was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private solverHandle_Conflict As cusolverDnHandle_t

		' temporary buffers, exactly 1 per thread
		Private bufferReduction As Pointer
		Private bufferAllocation As Pointer
		Private bufferScalar As Pointer

		' legacy. to be removed.
		Private bufferSpecial As Pointer

		Private deviceId As Integer = -1

		<NonSerialized>
		Private Shared ReadOnly nativeOps As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()

		Public Overrides Function ToString() As String
			Return "CudaContext{" & "bufferReduction=" & bufferReduction & ", bufferScalar=" & bufferScalar & ", deviceId=" & deviceId & "}"c
		End Function

		''' <summary>
		''' Synchronizes
		''' on the old stream
		''' </summary>
		Public Overridable Sub syncOldStream()
			If nativeOps.streamSynchronize(oldStream) = 0 Then
				Throw New ND4JIllegalStateException("CUDA stream synchronization failed")
			End If
		End Sub

		Public Overridable Sub syncSpecialStream()
			If nativeOps.streamSynchronize(specialStream) = 0 Then
				Throw New ND4JIllegalStateException("CUDA special stream synchronization failed")
			End If
		End Sub

		Public Overridable ReadOnly Property CublasStream As Pointer
			Get
				' FIXME: can we cache this please
				Dim lptr As val = New PointerPointer(Me.getOldStream())
				Return lptr.get(0)
			End Get
		End Property

		Public Overridable ReadOnly Property CublasHandle As cublasHandle_t
			Get
				' FIXME: can we cache this please
				Dim lptr As val = New PointerPointer(cublasHandle_Conflict)
				Return New cublasHandle_t(lptr.get(0))
			End Get
		End Property

		Public Overridable ReadOnly Property SolverHandle As cusolverDnHandle_t
			Get
				' FIXME: can we cache this please
				Dim lptr As val = New PointerPointer(solverHandle_Conflict)
				Return New cusolverDnHandle_t(lptr.get(0))
			End Get
		End Property
	End Class

End Namespace