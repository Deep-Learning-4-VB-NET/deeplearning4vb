Imports System
Imports NoArgsConstructor = lombok.NoArgsConstructor

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


	''' 
	''' @param <T> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class Atomic<T extends java.io.Serializable> implements java.io.Serializable
	<Serializable>
	Public Class Atomic(Of T As java.io.Serializable)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: private volatile T value;
		Private value As T
		<NonSerialized>
		Private lock As New ReentrantReadWriteLock()

		Public Sub New(ByVal initialValue As T)
			Me.value = initialValue
		End Sub

		''' <summary>
		''' This method assigns new value </summary>
		''' <param name="value"> </param>
		Public Overridable Sub set(ByVal value As T)
			Try
				lock.writeLock().lock()

				Me.value = value
			Finally
				lock.writeLock().unlock()
			End Try
		End Sub

		''' <summary>
		''' This method returns current value
		''' @return
		''' </summary>
		Public Overridable Function get() As T
			Try
				lock.readLock().lock()

				Return Me.value
			Finally
				lock.readLock().unlock()
			End Try
		End Function



		''' <summary>
		''' This method implements compare-and-swap
		''' </summary>
		''' <param name="expected"> </param>
		''' <param name="newValue"> </param>
		''' <returns> true if value was swapped, false otherwise </returns>
		Public Overridable Function cas(ByVal expected As T, ByVal newValue As T) As Boolean
			Try
				lock.writeLock().lock()

				If Objects.equals(value, expected) Then
					Me.value = newValue
					Return True
				Else
					Return False
				End If
			Finally
				lock.writeLock().unlock()
			End Try
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws IOException, ClassNotFoundException
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
			[in].defaultReadObject()
			lock = New ReentrantReadWriteLock()
		End Sub

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: Atomic<?> atomic = (Atomic<?>) o;
			Dim atomic As Atomic(Of Object) = DirectCast(o, Atomic(Of Object))
			Try
				Me.lock.readLock().lock()
				atomic.lock.readLock().lock()

				Return Objects.equals(Me.value, atomic.value)
			Catch e As Exception
				Throw New Exception(e)
			Finally
				atomic.lock.readLock().unlock()
				Me.lock.readLock().unlock()
			End Try
		End Function

		Public Overrides Function GetHashCode() As Integer
			Try
				Me.lock.readLock().lock()

				Return Objects.hash(value)
			Finally
				Me.lock.readLock().unlock()
			End Try
		End Function
	End Class

End Namespace