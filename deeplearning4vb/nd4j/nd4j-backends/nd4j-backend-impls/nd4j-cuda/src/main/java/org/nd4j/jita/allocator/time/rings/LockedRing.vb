Imports Ring = org.nd4j.jita.allocator.time.Ring

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

Namespace org.nd4j.jita.allocator.time.rings


	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
	Public Class LockedRing
		Implements Ring

		Private ReadOnly ring() As Single
		Private ReadOnly position As New AtomicInteger(0)

		Private lock As New ReentrantReadWriteLock()

		''' <summary>
		''' Builds new BasicRing with specified number of elements stored
		''' </summary>
		''' <param name="ringLength"> </param>
		Public Sub New(ByVal ringLength As Integer)
			Me.ring = New Single(ringLength - 1){}
		End Sub

		Public Overridable ReadOnly Property Average As Single Implements Ring.getAverage
			Get
				Try
					lock.readLock().lock()
    
					Dim rates As Single = 0.0f
					Dim x As Integer = 0
					Dim existing As Integer = 0
					For x = 0 To ring.Length - 1
						rates += ring(x)
						If ring(x) > 0 Then
							existing += 1
						End If
					Next x
					If existing > 0 Then
						Return rates / existing
					Else
						Return 0.0f
					End If
				Finally
					lock.readLock().unlock()
				End Try
			End Get
		End Property

		Public Overridable Sub store(ByVal rate As Double) Implements Ring.store
			store(CSng(rate))
		End Sub

		Public Overridable Sub store(ByVal rate As Single) Implements Ring.store
			Try
				lock.writeLock().lock()

				Dim pos As Integer = position.getAndIncrement()
				If pos >= ring.Length Then
					pos = 0
					position.set(0)
				End If
				ring(pos) = rate
			Finally
				lock.writeLock().unlock()
			End Try
		End Sub
	End Class

End Namespace