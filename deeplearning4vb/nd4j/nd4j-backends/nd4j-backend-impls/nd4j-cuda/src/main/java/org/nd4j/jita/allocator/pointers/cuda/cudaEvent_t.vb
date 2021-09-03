Imports System
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
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
	Public Class cudaEvent_t
		Inherits CudaPointer

'JAVA TO VB CONVERTER NOTE: The field destroyed was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private destroyed_Conflict As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private long clock;
		Private clock As Long

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private int laneId;
		Private laneId As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private int deviceId;
		Private deviceId As Integer

		Public Sub New(ByVal pointer As Pointer)
			MyBase.New(pointer)
		End Sub

		Public Overridable ReadOnly Property Destroyed As Boolean
			Get
				SyncLock Me
					Return destroyed_Conflict
				End SyncLock
			End Get
		End Property

		Public Overridable Sub markDestroyed()
			SyncLock Me
				destroyed_Conflict = True
			End SyncLock
		End Sub

		Public Overridable Sub destroy()
			If Not Destroyed Then
				NativeOpsHolder.Instance.getDeviceNativeOps().destroyEvent(Me)
				markDestroyed()
			End If
		End Sub

		Public Overridable Sub synchronize()
			If Not Destroyed Then
				Dim res As Integer = NativeOpsHolder.Instance.getDeviceNativeOps().eventSynchronize(Me)
				If res = 0 Then
					Throw New ND4JException("CUDA exception happened. Terminating. Last op: [" & Nd4j.Executioner.LastOp &"]")
				End If

				Dim code As val = NativeOpsHolder.Instance.getDeviceNativeOps().lastErrorCode()
				If code <> 0 Then
					Throw New Exception(NativeOpsHolder.Instance.getDeviceNativeOps().lastErrorMessage() & "; Error code: " & code)
				End If
			End If
		End Sub

		Public Overridable Sub register(ByVal stream As cudaStream_t)
			If Not Destroyed Then
				Dim res As Integer = NativeOpsHolder.Instance.getDeviceNativeOps().registerEvent(Me, stream)

				Dim code As val = NativeOpsHolder.Instance.getDeviceNativeOps().lastErrorCode()
				If code <> 0 Then
					Throw New Exception(NativeOpsHolder.Instance.getDeviceNativeOps().lastErrorMessage() & "; Error code: " & code)
				End If
			End If
		End Sub
	End Class

End Namespace