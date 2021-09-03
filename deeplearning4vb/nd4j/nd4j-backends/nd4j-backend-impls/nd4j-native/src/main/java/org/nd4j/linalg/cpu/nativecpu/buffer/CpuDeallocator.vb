﻿Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Deallocator = org.nd4j.linalg.api.memory.Deallocator
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
Imports OpaqueDataBuffer = org.nd4j.nativeblas.OpaqueDataBuffer

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

Namespace org.nd4j.linalg.cpu.nativecpu.buffer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CpuDeallocator implements org.nd4j.linalg.api.memory.Deallocator
	Public Class CpuDeallocator
		Implements Deallocator

		<NonSerialized>
		Private ReadOnly opaqueDataBuffer As OpaqueDataBuffer

		Public Sub New(ByVal buffer As BaseCpuDataBuffer)
			opaqueDataBuffer = buffer.OpaqueDataBuffer
		End Sub

		Public Overridable Sub deallocate() Implements Deallocator.deallocate
			If opaqueDataBuffer Is Nothing Then
				Throw New Exception("opaqueDataBuffer is null")
			End If

			NativeOpsHolder.Instance.getDeviceNativeOps().deleteDataBuffer(opaqueDataBuffer)
		End Sub
	End Class

End Namespace