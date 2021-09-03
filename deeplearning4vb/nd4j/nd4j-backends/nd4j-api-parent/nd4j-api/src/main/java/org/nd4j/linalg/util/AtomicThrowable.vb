Imports System

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

Namespace org.nd4j.linalg.util


	Public Class AtomicThrowable
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: protected volatile Throwable t = null;
		Protected Friend t As Exception = Nothing
		Protected Friend lock As ReentrantReadWriteLock

		''' <summary>
		''' This method creates new instance
		''' </summary>
		Public Sub New()
			' unfortunately we want fair queue here, to avoid situations when we return null after exception
			lock = New ReentrantReadWriteLock(True)
		End Sub

		''' <summary>
		''' This method creates new instance with given initial state </summary>
		''' <param name="e"> </param>
		Public Sub New(ByVal e As Exception)
			Me.New()
			t = e
		End Sub

		''' <summary>
		''' This method returns current state
		''' @return
		''' </summary>
		Public Overridable Function get() As Exception
			Try
				lock.readLock().lock()

				Return t
			Finally
				lock.readLock().unlock()
			End Try
		End Function

		''' <summary>
		''' This method updates state with given Throwable </summary>
		''' <param name="t"> </param>
		Public Overridable Sub set(ByVal t As Exception)
			Try
				lock.writeLock().lock()

				Me.t = t
			Finally
				lock.writeLock().unlock()
			End Try
		End Sub

		''' <summary>
		''' This method updates state only if it wasn't set before
		''' </summary>
		''' <param name="t"> </param>
		Public Overridable WriteOnly Property IfFirst As Exception
			Set(ByVal t As Exception)
				Try
					lock.writeLock().lock()
    
					If Me.t Is Nothing Then
						Me.t = t
					End If
				Finally
					lock.writeLock().unlock()
				End Try
			End Set
		End Property

		''' <summary>
		''' This method returns TRUE if internal state holds error, FALSE otherwise
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Triggered As Boolean
			Get
				Try
					lock.readLock().lock()
    
					Return t IsNot Nothing
				Finally
					lock.readLock().unlock()
				End Try
			End Get
		End Property
	End Class

End Namespace