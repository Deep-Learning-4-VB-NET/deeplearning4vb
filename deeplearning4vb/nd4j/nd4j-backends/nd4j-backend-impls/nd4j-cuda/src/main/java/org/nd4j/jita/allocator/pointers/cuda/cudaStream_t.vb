Imports System
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports CudaPointer = org.nd4j.jita.allocator.pointers.CudaPointer
Imports ND4JException = org.nd4j.linalg.exception.ND4JException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.nd4j.jita.allocator.pointers.cuda

	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
	Public Class cudaStream_t
		Inherits CudaPointer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public cudaStream_t(@NonNull Pointer pointer)
		Public Sub New(ByVal pointer As Pointer)
			MyBase.New(pointer)
		End Sub

		Public Overridable Function synchronize() As Integer
			Dim nativeOps As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()
			Dim res As Integer = nativeOps.streamSynchronize(Me)

			Dim ec As val = nativeOps.lastErrorCode()
			If ec <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage() & "; Error code: " & ec)
			End If

			Return res
		End Function
	End Class

End Namespace