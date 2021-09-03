Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
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

Namespace org.nd4j.rng.deallocator


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class NativeRandomDeallocator
	Public Class NativeRandomDeallocator
'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New NativeRandomDeallocator()

		' we don't really need concurrency here, so 1 queue will be just fine
		Private ReadOnly queue As ReferenceQueue(Of NativePack)
		Private ReadOnly referenceMap As IDictionary(Of Long, GarbageStateReference)
		Private deallocatorThreads As IList(Of DeallocatorThread) = New List(Of DeallocatorThread)()

		Private Sub New()
			Me.queue = New ReferenceQueue(Of NativePack)()
			Me.referenceMap = New ConcurrentDictionary(Of Long, GarbageStateReference)()

			Dim thread As New DeallocatorThread(Me, 0, queue, referenceMap)
			thread.Start()

			deallocatorThreads.Add(thread)
		End Sub

		Public Shared ReadOnly Property Instance As NativeRandomDeallocator
			Get
				Return INSTANCE_Conflict
			End Get
		End Property


		''' <summary>
		''' This method is used internally from NativeRandom deallocators
		''' This method doesn't accept Random interface implementations intentionally.
		''' </summary>
		''' <param name="random"> </param>
		Public Overridable Sub trackStatePointer(ByVal random As NativePack)
			If random.getStatePointer() IsNot Nothing Then
				Dim reference As New GarbageStateReference(random, queue)
				referenceMap(random.getStatePointer().address()) = reference
			End If
		End Sub


		<Obsolete>
		Public Const DeallocatorThreadNamePrefix As String = "NativeRandomDeallocator thread "

		''' <summary>
		''' This class provides garbage collection for NativeRandom state memory. It's not too big amount of memory used, but we don't want any leaks.
		''' 
		''' </summary>
		Protected Friend Class DeallocatorThread
			Inherits Thread
			Implements ThreadStart

			Private ReadOnly outerInstance As NativeRandomDeallocator

			Friend ReadOnly queue As ReferenceQueue(Of NativePack)
			Friend ReadOnly referenceMap As IDictionary(Of Long, GarbageStateReference)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected DeallocatorThread(int threadId, @NonNull ReferenceQueue<NativePack> queue, java.util.Map<Long, GarbageStateReference> referenceMap)
			Protected Friend Sub New(ByVal outerInstance As NativeRandomDeallocator, ByVal threadId As Integer, ByVal queue As ReferenceQueue(Of NativePack), ByVal referenceMap As IDictionary(Of Long, GarbageStateReference))
				Me.outerInstance = outerInstance
				Me.queue = queue
				Me.referenceMap = referenceMap
				Me.setName(DeallocatorThreadNamePrefix & threadId)
				Me.setDaemon(True)
			End Sub

			Public Overrides Sub run()
				Do
					Try
						Dim reference As GarbageStateReference = CType(queue.remove(), GarbageStateReference)
						If reference IsNot Nothing Then
							If reference.getStatePointer() IsNot Nothing Then
								referenceMap.Remove(reference.getStatePointer().address())
								NativeOpsHolder.Instance.getDeviceNativeOps().destroyRandom(reference.getStatePointer())
							End If
						Else
							LockSupport.parkNanos(5000L)
						End If
					Catch e As InterruptedException
						' do nothing
					Catch e As Exception
						Throw New Exception(e)
					End Try
				Loop
			End Sub
		End Class
	End Class

End Namespace