Imports System
Imports System.Threading
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports DataType = org.nd4j.linalg.api.buffer.DataType

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

Namespace org.nd4j.nativeblas

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class OpaqueDataBuffer extends org.bytedeco.javacpp.Pointer
	Public Class OpaqueDataBuffer
		Inherits Pointer

		' TODO: make this configurable
		Private Const MAX_TRIES As Integer = 5

		Public Sub New(ByVal p As Pointer)
			MyBase.New(p)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static OpaqueDataBuffer externalizedDataBuffer(long numElements, @NonNull DataType dataType, org.bytedeco.javacpp.Pointer primary, org.bytedeco.javacpp.Pointer special)
		Public Shared Function externalizedDataBuffer(ByVal numElements As Long, ByVal dataType As DataType, ByVal primary As Pointer, ByVal special As Pointer) As OpaqueDataBuffer
			Return NativeOpsHolder.Instance.getDeviceNativeOps().dbCreateExternalDataBuffer(numElements, dataType.toInt(), primary, special)
		End Function

		''' <summary>
		''' This method allocates new InteropDataBuffer and returns pointer to it </summary>
		''' <param name="numElements"> </param>
		''' <param name="dataType"> </param>
		''' <param name="allocateBoth">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static OpaqueDataBuffer allocateDataBuffer(long numElements, @NonNull DataType dataType, boolean allocateBoth)
		Public Shared Function allocateDataBuffer(ByVal numElements As Long, ByVal dataType As DataType, ByVal allocateBoth As Boolean) As OpaqueDataBuffer
			Dim buffer As OpaqueDataBuffer = Nothing
			Dim ec As Integer = 0
			Dim em As String = Nothing

			For t As Integer = 0 To MAX_TRIES - 1
				Try
					' try to allocate data buffer
					buffer = NativeOpsHolder.Instance.getDeviceNativeOps().allocateDataBuffer(numElements, dataType.toInt(), allocateBoth)
					' check error code
					ec = NativeOpsHolder.Instance.getDeviceNativeOps().lastErrorCode()
					If ec <> 0 Then
						em = NativeOpsHolder.Instance.getDeviceNativeOps().lastErrorMessage()

						' if allocation failed it might be caused by casual OOM, so we'll try GC
						System.GC.Collect()

						' sleeping for 50ms
						Thread.Sleep(50)
					Else
						' just return the buffer
						Return buffer
					End If
				Catch e As Exception
					Throw New Exception(e)
				End Try
			Next t

			' if MAX_TRIES is over, we'll just throw an exception
			Throw New Exception("Allocation failed: [" & em & "]")
		End Function

		''' <summary>
		''' This method expands buffer, and copies content to the new buffer
		''' 
		''' PLEASE NOTE: if InteropDataBuffer doesn't own actual buffers - original pointers won't be released </summary>
		''' <param name="numElements"> </param>
		Public Overridable Sub expand(ByVal numElements As Long)
			Dim ec As Integer = 0
			Dim em As String = Nothing

			For t As Integer = 0 To MAX_TRIES - 1
				Try
					' try to expand the buffer
					NativeOpsHolder.Instance.getDeviceNativeOps().dbExpand(Me, numElements)

					' check error code
					ec = NativeOpsHolder.Instance.getDeviceNativeOps().lastErrorCode()
					If ec <> 0 Then
						em = NativeOpsHolder.Instance.getDeviceNativeOps().lastErrorMessage()

						' if expansion failed it might be caused by casual OOM, so we'll try GC
						System.GC.Collect()

						Thread.Sleep(50)
					Else
						' just return
						Return
					End If
				Catch e As Exception
					Throw New Exception(e)
				End Try
			Next t

			' if MAX_TRIES is over, we'll just throw an exception
			Throw New Exception("DataBuffer expansion failed: [" & em & "]")
		End Sub

		''' <summary>
		''' This method creates a view out of this InteropDataBuffer
		''' </summary>
		''' <param name="bytesLength"> </param>
		''' <param name="bytesOffset">
		''' @return </param>
		Public Overridable Function createView(ByVal bytesLength As Long, ByVal bytesOffset As Long) As OpaqueDataBuffer
			Dim buffer As OpaqueDataBuffer = Nothing
			Dim ec As Integer = 0
			Dim em As String = Nothing

			For t As Integer = 0 To MAX_TRIES - 1
				Try
					buffer = NativeOpsHolder.Instance.getDeviceNativeOps().dbCreateView(Me, bytesLength, bytesOffset)

					' check error code
					ec = NativeOpsHolder.Instance.getDeviceNativeOps().lastErrorCode()
					If ec <> 0 Then
						em = NativeOpsHolder.Instance.getDeviceNativeOps().lastErrorMessage()

						' if view creation failed it might be caused by casual OOM, so we'll try GC
						System.GC.Collect()

						' sleeping to let gc kick in
						Thread.Sleep(50)
					Else
						' just return
						Return buffer
					End If
				Catch e As Exception
					Throw New Exception(e)
				End Try
			Next t

			' if MAX_TRIES is over, we'll just throw an exception
			Throw New Exception("DataBuffer expansion failed: [" & em & "]")
		End Function

		''' <summary>
		''' This method returns pointer to linear buffer, primary one.
		''' @return
		''' </summary>
		Public Overridable Function primaryBuffer() As Pointer
			Return NativeOpsHolder.Instance.getDeviceNativeOps().dbPrimaryBuffer(Me)
		End Function

		''' <summary>
		''' This method returns pointer to special buffer, device one, if any.
		''' @return
		''' </summary>
		Public Overridable Function specialBuffer() As Pointer
			Return NativeOpsHolder.Instance.getDeviceNativeOps().dbSpecialBuffer(Me)
		End Function

		''' <summary>
		''' This method returns deviceId of this DataBuffer
		''' @return
		''' </summary>
		Public Overridable Function deviceId() As Integer
			Return NativeOpsHolder.Instance.getDeviceNativeOps().dbDeviceId(Me)
		End Function

		''' <summary>
		''' This method allows to set external pointer as primary buffer.
		''' 
		''' PLEASE NOTE: if InteropDataBuffer owns current memory buffer, it will be released </summary>
		''' <param name="ptr"> </param>
		''' <param name="numElements"> </param>
		Public Overridable Sub setPrimaryBuffer(ByVal ptr As Pointer, ByVal numElements As Long)
			NativeOpsHolder.Instance.getDeviceNativeOps().dbSetPrimaryBuffer(Me, ptr, numElements)
		End Sub

		''' <summary>
		''' This method allows to set external pointer as primary buffer.
		''' 
		''' PLEASE NOTE: if InteropDataBuffer owns current memory buffer, it will be released </summary>
		''' <param name="ptr"> </param>
		''' <param name="numElements"> </param>
		Public Overridable Sub setSpecialBuffer(ByVal ptr As Pointer, ByVal numElements As Long)
			NativeOpsHolder.Instance.getDeviceNativeOps().dbSetSpecialBuffer(Me, ptr, numElements)
		End Sub

		''' <summary>
		''' This method synchronizes device memory
		''' </summary>
		Public Overridable Sub syncToSpecial()
			NativeOpsHolder.Instance.getDeviceNativeOps().dbSyncToSpecial(Me)
		End Sub

		''' <summary>
		''' This method synchronizes host memory
		''' </summary>
		Public Overridable Sub syncToPrimary()
			NativeOpsHolder.Instance.getDeviceNativeOps().dbSyncToPrimary(Me)
		End Sub

		''' <summary>
		''' This method releases underlying buffer
		''' </summary>
		Public Overridable Sub closeBuffer()
			NativeOpsHolder.Instance.getDeviceNativeOps().dbClose(Me)
		End Sub
	End Class

End Namespace