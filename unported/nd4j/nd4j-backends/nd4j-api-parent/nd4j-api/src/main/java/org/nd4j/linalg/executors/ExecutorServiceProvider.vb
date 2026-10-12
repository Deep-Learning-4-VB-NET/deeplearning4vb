Imports System.Threading

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

Namespace org.nd4j.linalg.executors


	Public Class ExecutorServiceProvider

		Public Const EXEC_THREADS As String = "org.nd4j.parallel.threads"
		Public Const ENABLED As String = "org.nd4j.parallel.enabled"

		Private Shared ReadOnly nThreads As Integer
'JAVA TO VB CONVERTER NOTE: The field executorService was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared executorService_Conflict As ExecutorService
'JAVA TO VB CONVERTER NOTE: The field forkJoinPool was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared forkJoinPool_Conflict As ForkJoinPool

		Shared Sub New()
			Dim defaultThreads As Integer = Runtime.getRuntime().availableProcessors()
'JAVA TO VB CONVERTER NOTE: The variable enabled was renamed since Visual Basic does not handle local variables named the same as class members well:
			Dim enabled_Conflict As Boolean = Boolean.Parse(System.getProperty(ENABLED, "true"))
			If Not enabled_Conflict Then
				nThreads = 1
			Else
				nThreads = Integer.Parse(System.getProperty(EXEC_THREADS, defaultThreads.ToString()))
			End If
		End Sub

		Public Shared ReadOnly Property ExecutorService As ExecutorService
			Get
				SyncLock GetType(ExecutorServiceProvider)
					If executorService_Conflict IsNot Nothing Then
						Return executorService_Conflict
					End If
            
					executorService_Conflict = New ThreadPoolExecutor(nThreads, nThreads, 60L, TimeUnit.SECONDS, New LinkedTransferQueue(Of ThreadStart)(), New ThreadFactoryAnonymousInnerClass())
					Return executorService_Conflict
				End SyncLock
			End Get
		End Property

		Private Class ThreadFactoryAnonymousInnerClass
			Inherits ThreadFactory

			Public Overrides Function newThread(ByVal r As ThreadStart) As Thread
				Dim t As Thread = Executors.defaultThreadFactory().newThread(r)
				t.setDaemon(True)
				Return t
			End Function
		End Class

		Public Shared ReadOnly Property ForkJoinPool As ForkJoinPool
			Get
				SyncLock GetType(ExecutorServiceProvider)
					If forkJoinPool_Conflict IsNot Nothing Then
						Return forkJoinPool_Conflict
					End If
					forkJoinPool_Conflict = New ForkJoinPool(nThreads, ForkJoinPool.defaultForkJoinWorkerThreadFactory, Nothing, True)
					Return forkJoinPool_Conflict
				End SyncLock
			End Get
		End Property

	End Class

End Namespace