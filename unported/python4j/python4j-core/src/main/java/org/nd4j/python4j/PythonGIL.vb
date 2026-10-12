Imports System
Imports System.Threading
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports PyThreadState = org.bytedeco.cpython.PyThreadState
Imports org.bytedeco.cpython.global.python

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

Namespace org.nd4j.python4j




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class PythonGIL implements AutoCloseable
	Public Class PythonGIL
		Implements AutoCloseable

		Private Shared ReadOnly acquired As New AtomicBoolean()
		Private acquiredByMe As Boolean = False
		Private Shared defaultThreadId As Long = -1
		Private gilState As Integer
		Private Shared mainThreadState As PyThreadState
		Private Shared mainThreadId As Long = -1
		Shared Sub New()
			Dim tempVar As New PythonExecutioner()
		End Sub

		''' <summary>
		''' Set the main thread state
		''' based on the current thread calling this method.
		''' This method should not be called by the user.
		''' It is already invoked automatically in <seealso cref="PythonExecutioner"/>
		''' </summary>
		Public Shared Sub setMainThreadState()
			SyncLock GetType(PythonGIL)
				If mainThreadId < 0 AndAlso mainThreadState IsNot Nothing Then
					mainThreadState = PyThreadState_Get()
					mainThreadId = Thread.CurrentThread.getId()
				End If
        
			End SyncLock
		End Sub

		''' <summary>
		''' Asserts that the lock has been acquired.
		''' </summary>
		Public Shared Sub assertThreadSafe()
			If acquired.get() Then
				Return
			End If
			If defaultThreadId = -1 Then
				defaultThreadId = Thread.CurrentThread.getId()
			ElseIf defaultThreadId <> Thread.CurrentThread.getId() Then
				Throw New Exception("Attempt to use Python4j from multiple threads without " & "acquiring GIL. Enclose your code in a try(PythonGIL gil = PythonGIL.lock()){...}" & " block to ensure that GIL is acquired in multi-threaded environments.")
			End If

			If Not acquired.get() Then
				Throw New System.InvalidOperationException("Execution happening outside of GIL. Please use PythonExecutioner within a GIL block by wrapping it in a call via: try(PythonGIL gil = PythonGIL.lock()) { .. }")
			End If
		End Sub


		Private Sub New()
			Do While acquired.get()
				Try
					log.debug("Blocking for GIL on thread " & Thread.CurrentThread.getId())
					Thread.Sleep(100)
				Catch e As Exception
					Throw New Exception(e)
				End Try

			Loop

			log.debug("Acquiring GIL on " & Thread.CurrentThread.getId())
			acquired.set(True)
			acquiredByMe = True
			acquire()

		End Sub

		Public Overrides Sub close()
			SyncLock Me
				If acquiredByMe Then
					release()
					log.info("Releasing GIL on thread " & Thread.CurrentThread.getId())
					acquired.set(False)
					acquiredByMe = False
				Else
					log.info("Attempted to release GIL without having acquired GIL on thread " & Thread.CurrentThread.getId())
				End If
        
			End SyncLock
		End Sub


		''' <summary>
		''' Lock the GIL for running python scripts.
		''' This method should be used to create a new
		''' <seealso cref="PythonGIL"/> object in the form of:
		''' try(PythonGIL gil = PythonGIL.lock()) {
		'''     //your python code here
		''' } </summary>
		''' <returns> the gil for this instance </returns>
		Public Shared Function lock() As PythonGIL
			SyncLock GetType(PythonGIL)
				Return New PythonGIL()
			End SyncLock
		End Function

		Private Sub acquire()
			SyncLock Me
				If Thread.CurrentThread.getId() <> mainThreadId Then
					log.info("Pre Gil State ensure for thread " & Thread.CurrentThread.getId())
					gilState = PyGILState_Ensure()
					log.info("Thread " & Thread.CurrentThread.getId() & " acquired GIL")
				Else
					'See: https://github.com/pytorch/pytorch/issues/47776#issuecomment-726455206
					'From this thread: // PyEval_RestoreThread() should not be called if runtime is finalizing
					' See https://docs.python.org/3/c-api/init.html#c.PyEval_RestoreThread
        
					If _Py_IsFinalizing() <> 1 AndAlso PythonConstants.releaseGilAutomatically() Then
						PyEval_RestoreThread(mainThreadState)
					End If
				End If
			End SyncLock
		End Sub

		Private Sub release() ' do not synchronize!
			If Thread.CurrentThread.getId() <> mainThreadId Then
				log.debug("Pre gil state release for thread " & Thread.CurrentThread.getId())
				If PythonConstants.releaseGilAutomatically() Then
					PyGILState_Release(gilState)
				End If
			Else
				'See: https://github.com/pytorch/pytorch/issues/47776#issuecomment-726455206
				'From this thread: // PyEval_RestoreThread() should not be called if runtime is finalizing
				' See https://docs.python.org/3/c-api/init.html#c.PyEval_RestoreThread

				If _Py_IsFinalizing() <> 1 AndAlso PythonConstants.releaseGilAutomatically() Then
					PyEval_RestoreThread(mainThreadState)
				End If
			End If
		End Sub

		''' <summary>
		''' Returns true if the GIL is currently in use.
		''' This is typically true when <seealso cref="lock()"/>
		''' @return
		''' </summary>
		Public Shared Function locked() As Boolean
			Return acquired.get()
		End Function
	End Class
End Namespace