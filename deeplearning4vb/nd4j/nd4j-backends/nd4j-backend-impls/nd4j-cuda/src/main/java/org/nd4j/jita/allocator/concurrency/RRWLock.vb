Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports AllocationShape = org.nd4j.jita.allocator.impl.AllocationShape

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

Namespace org.nd4j.jita.allocator.concurrency


	''' <summary>
	''' Lock implementation based on ReentrantReadWriteLock
	''' 
	''' @author raver119@gmail.com
	''' </summary>
	Public Class RRWLock
		Implements Lock

		Private globalLock As New ReentrantReadWriteLock()
		Private externalsLock As New ReentrantReadWriteLock()

		Private objectLocks As IDictionary(Of Object, ReentrantReadWriteLock) = New ConcurrentDictionary(Of Object, ReentrantReadWriteLock)()


		''' <summary>
		''' This method notifies locker, that specific object was added to tracking list
		''' </summary>
		''' <param name="object"> </param>
		Public Overridable Sub attachObject(ByVal [object] As Object) Implements Lock.attachObject
			If Not objectLocks.ContainsKey([object]) Then
				objectLocks([object]) = New ReentrantReadWriteLock()
			End If
		End Sub

		''' <summary>
		''' This method notifies locker that specific object was removed from tracking list
		''' </summary>
		''' <param name="object"> </param>
		Public Overridable Sub detachObject(ByVal [object] As Object) Implements Lock.detachObject
			objectLocks.Remove([object])
		End Sub

		''' <summary>
		''' This method acquires global-level read lock
		''' </summary>
		Public Overridable Sub globalReadLock() Implements Lock.globalReadLock
			globalLock.readLock().lock()
		End Sub

		''' <summary>
		''' This method releases global-level read lock
		''' </summary>
		Public Overridable Sub globalReadUnlock() Implements Lock.globalReadUnlock
			globalLock.readLock().unlock()
		End Sub

		''' <summary>
		''' This method acquires global-level write lock
		''' </summary>
		Public Overridable Sub globalWriteLock() Implements Lock.globalWriteLock
			globalLock.writeLock().lock()
		End Sub

		''' <summary>
		''' This method releases global-level write lock
		''' </summary>
		Public Overridable Sub globalWriteUnlock() Implements Lock.globalWriteUnlock
			globalLock.writeLock().unlock()
		End Sub

		''' <summary>
		''' This method acquires object-level read lock, and global-level read lock
		''' </summary>
		''' <param name="object"> </param>
		Public Overridable Sub objectReadLock(ByVal [object] As Object) Implements Lock.objectReadLock
			'     globalReadLock();

			objectLocks([object]).readLock().lock()
		End Sub

		''' <summary>
		''' This method releases object-level read lock, and global-level read lock
		''' </summary>
		''' <param name="object"> </param>
		Public Overridable Sub objectReadUnlock(ByVal [object] As Object) Implements Lock.objectReadUnlock
			objectLocks([object]).readLock().unlock()

			'     globalReadUnlock();
		End Sub

		''' <summary>
		''' This method acquires object-level write lock, and global-level read lock
		''' </summary>
		''' <param name="object"> </param>
		Public Overridable Sub objectWriteLock(ByVal [object] As Object) Implements Lock.objectWriteLock
			'     globalReadLock();

			objectLocks([object]).writeLock().lock()
		End Sub

		''' <summary>
		''' This method releases object-level read lock, and global-level read lock
		''' </summary>
		''' <param name="object"> </param>
		Public Overridable Sub objectWriteUnlock(ByVal [object] As Object) Implements Lock.objectWriteUnlock
			objectLocks([object]).writeLock().unlock()

			'      globalReadUnlock();
		End Sub

		''' <summary>
		''' This method acquires shape-level read lock, and read locks for object and global
		''' </summary>
		''' <param name="object"> </param>
		''' <param name="shape"> </param>
		Public Overridable Sub shapeReadLock(ByVal [object] As Object, ByVal shape As AllocationShape) Implements Lock.shapeReadLock
			objectReadLock([object])
		End Sub

		''' <summary>
		''' This method releases shape-level read lock, and read locks for object and global
		''' </summary>
		''' <param name="object"> </param>
		''' <param name="shape"> </param>
		Public Overridable Sub shapeReadUnlock(ByVal [object] As Object, ByVal shape As AllocationShape) Implements Lock.shapeReadUnlock

			objectReadUnlock([object])
		End Sub

		''' <summary>
		''' This method acquires shape-level write lock, and read locks for object and global
		''' </summary>
		''' <param name="object"> </param>
		''' <param name="shape"> </param>
		Public Overridable Sub shapeWriteLock(ByVal [object] As Object, ByVal shape As AllocationShape) Implements Lock.shapeWriteLock
			objectReadLock([object])
		End Sub

		''' <summary>
		''' This method releases shape-level write lock, and read locks for object and global
		''' </summary>
		''' <param name="object"> </param>
		''' <param name="shape"> </param>
		Public Overridable Sub shapeWriteUnlock(ByVal [object] As Object, ByVal shape As AllocationShape) Implements Lock.shapeWriteUnlock
			objectReadUnlock([object])
		End Sub

		''' <summary>
		''' This methods acquires read-lock on externals, and read-lock on global
		''' </summary>
		Public Overridable Sub externalsReadLock() Implements Lock.externalsReadLock
			externalsLock.readLock().lock()
		End Sub

		''' <summary>
		''' This methods releases read-lock on externals, and read-lock on global
		''' </summary>
		Public Overridable Sub externalsReadUnlock() Implements Lock.externalsReadUnlock
			externalsLock.readLock().unlock()
		End Sub

		''' <summary>
		''' This methods acquires write-lock on externals, and read-lock on global
		''' </summary>
		Public Overridable Sub externalsWriteLock() Implements Lock.externalsWriteLock
			externalsLock.writeLock().lock()
		End Sub

		''' <summary>
		''' This methods releases write-lock on externals, and read-lock on global
		''' </summary>
		Public Overridable Sub externalsWriteUnlock() Implements Lock.externalsWriteUnlock
			externalsLock.writeLock().unlock()
		End Sub
	End Class

End Namespace