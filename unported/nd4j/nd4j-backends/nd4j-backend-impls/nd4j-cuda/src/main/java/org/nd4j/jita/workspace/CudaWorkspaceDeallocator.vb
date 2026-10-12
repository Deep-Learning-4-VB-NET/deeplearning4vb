Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Deallocator = org.nd4j.linalg.api.memory.Deallocator
Imports MemoryKind = org.nd4j.linalg.api.memory.enums.MemoryKind
Imports PointersPair = org.nd4j.linalg.api.memory.pointers.PointersPair
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.jita.workspace


	''' <summary>
	''' Deallocator implementation for CpuWorkspace
	''' @author raver119@gmail.com
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CudaWorkspaceDeallocator implements org.nd4j.linalg.api.memory.Deallocator
	Public Class CudaWorkspaceDeallocator
		Implements Deallocator

		Private pointersPair As PointersPair
		Private pinnedPointers As LinkedList(Of PointersPair)
		Private externalPointers As IList(Of PointersPair)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CudaWorkspaceDeallocator(@NonNull CudaWorkspace workspace)
		Public Sub New(ByVal workspace As CudaWorkspace)
			Me.pointersPair = workspace.workspace()
			Me.pinnedPointers = workspace.pinnedPointers()
			Me.externalPointers = workspace.externalPointers()
		End Sub

		Public Overridable Sub deallocate() Implements Deallocator.deallocate
			log.trace("Deallocating CUDA workspace")

			' purging workspace planes
			If pointersPair IsNot Nothing Then
				If pointersPair.getDevicePointer() IsNot Nothing Then
					'log.info("Deallocating device...");
					Nd4j.MemoryManager.release(pointersPair.getDevicePointer(), MemoryKind.DEVICE)
				End If

				If pointersPair.getHostPointer() IsNot Nothing Then
					'                                log.info("Deallocating host...");
					Nd4j.MemoryManager.release(pointersPair.getHostPointer(), MemoryKind.HOST)
				End If
			End If

			' purging all spilled pointers
			For Each pair2 As PointersPair In externalPointers
				If pair2 IsNot Nothing Then
					If pair2.getHostPointer() IsNot Nothing Then
						Nd4j.MemoryManager.release(pair2.getHostPointer(), MemoryKind.HOST)
					End If

					If pair2.getDevicePointer() IsNot Nothing Then
						Nd4j.MemoryManager.release(pair2.getDevicePointer(), MemoryKind.DEVICE)
					End If
				End If
			Next pair2

			' purging all pinned pointers
			' purging all spilled pointers
			For Each pair2 As PointersPair In externalPointers
				If pair2 IsNot Nothing Then
					If pair2.getHostPointer() IsNot Nothing Then
						Nd4j.MemoryManager.release(pair2.getHostPointer(), MemoryKind.HOST)
					End If

					If pair2.getDevicePointer() IsNot Nothing Then
						Nd4j.MemoryManager.release(pair2.getDevicePointer(), MemoryKind.DEVICE)
					End If
				End If
			Next pair2

			' purging all pinned pointers
			Dim pair As PointersPair = Nothing
			pair = pinnedPointers.RemoveFirst()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((pair = pinnedPointers.poll()) != null)
			Do While pair IsNot Nothing
				If pair.getHostPointer() IsNot Nothing Then
					Nd4j.MemoryManager.release(pair.getHostPointer(), MemoryKind.HOST)
				End If

				If pair.getDevicePointer() IsNot Nothing Then
					Nd4j.MemoryManager.release(pair.getDevicePointer(), MemoryKind.DEVICE)
				End If
					pair = pinnedPointers.RemoveFirst()
			Loop

		End Sub
	End Class

End Namespace