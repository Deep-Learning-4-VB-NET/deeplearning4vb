Imports System
Imports Deallocator = org.nd4j.linalg.api.memory.Deallocator
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
Imports OpaqueContext = org.nd4j.nativeblas.OpaqueContext

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

Namespace org.nd4j.linalg.cpu.nativecpu.ops

	Public Class CpuOpContextDeallocator
		Implements Deallocator

		<NonSerialized>
		Private ReadOnly context As OpaqueContext

		Public Sub New(ByVal ctx As CpuOpContext)
			context = CType(ctx.contextPointer(), OpaqueContext)
		End Sub

		Public Overridable Sub deallocate() Implements Deallocator.deallocate
			NativeOpsHolder.Instance.getDeviceNativeOps().deleteGraphContext(context)
		End Sub
	End Class

End Namespace