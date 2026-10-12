Imports Nd4jBlas = org.nd4j.nativeblas.Nd4jBlas
Imports org.bytedeco.cuda.cublas
Imports org.bytedeco.cuda.global.cublas

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

Namespace org.nd4j.linalg.jcublas.blas

	''' <summary>
	''' Implementation of Nd4jBlas for cuBLAS
	''' 
	''' @author saudet
	''' </summary>
	Public Class CudaBlas
		Inherits Nd4jBlas

		Friend Shared Function convertStatus(ByVal status As Integer) As Integer
			Select Case status
				Case 0
					Return CUBLAS_STATUS_SUCCESS
				Case 1
					Return CUBLAS_STATUS_NOT_INITIALIZED
				Case 3
					Return CUBLAS_STATUS_ALLOC_FAILED
				Case 7
					Return CUBLAS_STATUS_INVALID_VALUE
				Case 8
					Return CUBLAS_STATUS_ARCH_MISMATCH
				Case 11
					Return CUBLAS_STATUS_MAPPING_ERROR
				Case 13
					Return CUBLAS_STATUS_EXECUTION_FAILED
				Case 14
					Return CUBLAS_STATUS_INTERNAL_ERROR
				Case 15
					Return CUBLAS_STATUS_NOT_SUPPORTED
				Case 16
					Return CUBLAS_STATUS_LICENSE_ERROR
				Case Else
					Return CUBLAS_STATUS_SUCCESS
			End Select
		End Function

		Friend Shared Function convertUplo(ByVal fillMode As Integer) As Integer
			Select Case fillMode
				Case 0
					Return CUBLAS_FILL_MODE_LOWER
				Case 1
					Return CUBLAS_FILL_MODE_UPPER
				Case Else
					Return CUBLAS_FILL_MODE_LOWER
			End Select
		End Function

		Friend Shared Function convertDiag(ByVal diag As Integer) As Integer
			Select Case diag
				Case 0
					Return CUBLAS_DIAG_NON_UNIT
				Case 1
					Return CUBLAS_DIAG_UNIT
				Case Else
					Return CUBLAS_DIAG_NON_UNIT
			End Select
		End Function

		Friend Shared Function convertTranspose(ByVal op As Integer) As Integer
			Select Case op
				Case 78
					Return CUBLAS_OP_N
				Case 84
					Return CUBLAS_OP_T
				Case 67
					Return CUBLAS_OP_C
				Case Else
					Return CUBLAS_OP_N
			End Select
		End Function

		Friend Shared Function convertPointerMode(ByVal pointerMode As Integer) As Integer
			Select Case pointerMode
				Case 0
					Return CUBLAS_POINTER_MODE_HOST
				Case 1
					Return CUBLAS_POINTER_MODE_DEVICE
				Case Else
					Return CUBLAS_POINTER_MODE_HOST
			End Select
		End Function

		Friend Shared Function convertSideMode(ByVal sideMode As Integer) As Integer
			Select Case sideMode
				Case 0
					Return CUBLAS_SIDE_LEFT
				Case 1
					Return CUBLAS_SIDE_RIGHT
				Case Else
					Return CUBLAS_SIDE_LEFT
			End Select
		End Function

		Public Overrides Property MaxThreads As Integer
			Set(ByVal num As Integer)
				' no-op
			End Set
			Get
				' 0 - cuBLAS
				Return 0
			End Get
		End Property


		''' <summary>
		''' Returns the BLAS library vendor id
		''' 
		''' 1 - CUBLAS
		''' </summary>
		''' <returns> the BLAS library vendor id </returns>
		Public Overrides ReadOnly Property BlasVendorId As Integer
			Get
				Return 1
			End Get
		End Property

		Public Overrides Function logOpenMPBlasThreads() As Boolean
			Return False
		End Function
	End Class

End Namespace