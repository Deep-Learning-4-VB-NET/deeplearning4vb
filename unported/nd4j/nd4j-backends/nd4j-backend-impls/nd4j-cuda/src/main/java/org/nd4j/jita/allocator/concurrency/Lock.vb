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
	''' Collection of multilevel locks for JITA
	''' 
	''' @author raver119@gmail.com
	''' </summary>
	Public Interface Lock

		''' <summary>
		''' This method notifies locker, that specific object was added to tracking list </summary>
		''' <param name="object"> </param>
		Sub attachObject(ByVal [object] As Object)

		''' <summary>
		''' This method notifies locker that specific object was removed from tracking list </summary>
		''' <param name="object"> </param>
		Sub detachObject(ByVal [object] As Object)

		'/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		'//  Global-level locks

		''' <summary>
		''' This method acquires global-level read lock
		''' </summary>
		Sub globalReadLock()

		''' <summary>
		''' This method releases global-level read lock
		''' </summary>
		Sub globalReadUnlock()

		''' <summary>
		''' This method acquires global-level write lock
		''' </summary>
		Sub globalWriteLock()

		''' <summary>
		''' This method releases global-level write lock
		''' </summary>
		Sub globalWriteUnlock()

		'/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		'//  Object-level locks

		''' <summary>
		''' This method acquires object-level read lock, and global-level read lock </summary>
		''' <param name="object"> </param>
		Sub objectReadLock(ByVal [object] As Object)

		''' <summary>
		''' This method releases object-level read lock, and global-level read lock </summary>
		''' <param name="object"> </param>
		Sub objectReadUnlock(ByVal [object] As Object)

		''' <summary>
		''' This method acquires object-level write lock, and global-level read lock </summary>
		''' <param name="object"> </param>
		Sub objectWriteLock(ByVal [object] As Object)

		''' <summary>
		''' This method releases object-level read lock, and global-level read lock </summary>
		''' <param name="object"> </param>
		Sub objectWriteUnlock(ByVal [object] As Object)

		'/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		'//  Shape-level locks

		''' <summary>
		''' This method acquires shape-level read lock, and read locks for object and global </summary>
		''' <param name="object"> </param>
		''' <param name="shape"> </param>
		Sub shapeReadLock(ByVal [object] As Object, ByVal shape As AllocationShape)

		''' <summary>
		''' This method releases shape-level read lock, and read locks for object and global </summary>
		''' <param name="object"> </param>
		''' <param name="shape"> </param>
		Sub shapeReadUnlock(ByVal [object] As Object, ByVal shape As AllocationShape)

		''' <summary>
		''' This method acquires shape-level write lock, and read locks for object and global </summary>
		''' <param name="object"> </param>
		''' <param name="shape"> </param>
		Sub shapeWriteLock(ByVal [object] As Object, ByVal shape As AllocationShape)

		''' <summary>
		''' This method releases shape-level write lock, and read locks for object and global </summary>
		''' <param name="object"> </param>
		''' <param name="shape"> </param>
		Sub shapeWriteUnlock(ByVal [object] As Object, ByVal shape As AllocationShape)

		''' <summary>
		''' This methods acquires read-lock on externals, and read-lock on global
		''' </summary>
		Sub externalsReadLock()

		''' <summary>
		''' This methods releases read-lock on externals, and read-lock on global
		''' </summary>
		Sub externalsReadUnlock()

		''' <summary>
		''' This methods acquires write-lock on externals, and read-lock on global
		''' </summary>
		Sub externalsWriteLock()

		''' <summary>
		''' This methods releases write-lock on externals, and read-lock on global
		''' </summary>
		Sub externalsWriteUnlock()
	End Interface

End Namespace