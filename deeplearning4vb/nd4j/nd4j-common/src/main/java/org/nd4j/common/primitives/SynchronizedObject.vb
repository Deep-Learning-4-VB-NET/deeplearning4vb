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

Namespace org.nd4j.common.primitives


	<Serializable>
	Public Class SynchronizedObject(Of T)
		Protected Friend value As T
		<NonSerialized>
		Protected Friend lock As ReentrantReadWriteLock

		Public Sub New()
			lock = New ReentrantReadWriteLock()
		End Sub

		Public Sub New(ByVal value As T)
			Me.New()

			Me.set(value)
		End Sub

		''' <summary>
		''' This method returns stored value via read lock
		''' @return
		''' </summary>
		Public Function get() As T
			Try
				lock.readLock().lock()

				Return value
			Finally
				lock.readLock().unlock()
			End Try
		End Function

		''' <summary>
		''' This method updates stored value via write lock </summary>
		''' <param name="value"> </param>
		Public Sub set(ByVal value As T)
			Try
				lock.writeLock().lock()

				Me.value = value
			Finally
				lock.writeLock().unlock()
			End Try
		End Sub
	End Class

End Namespace