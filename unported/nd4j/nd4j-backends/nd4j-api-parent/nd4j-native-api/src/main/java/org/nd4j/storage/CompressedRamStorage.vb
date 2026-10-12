Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports NoOp = org.nd4j.compression.impl.NoOp
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.linalg.compression
Imports NDArrayCompressor = org.nd4j.linalg.compression.NDArrayCompressor

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

Namespace org.nd4j.storage


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CompressedRamStorage<T extends Object> implements org.nd4j.linalg.compression.AbstractStorage<T>
	Public Class CompressedRamStorage(Of T As Object)
		Implements AbstractStorage(Of T)

		Private compressor As NDArrayCompressor = New NoOp()
		Private compressedEntries As IDictionary(Of T, INDArray) = New ConcurrentDictionary(Of T, INDArray)()
		Private useInplaceCompression As Boolean = False
		Private lock As New ReentrantReadWriteLock()
		Private emulateIsAbsent As Boolean = False

		Private Sub New()
			'
		End Sub

		''' <summary>
		''' Store object into storage
		''' </summary>
		''' <param name="key"> </param>
		''' <param name="object"> </param>
		Public Overridable Sub store(ByVal key As T, ByVal [object] As INDArray) Implements AbstractStorage(Of T).store
			Dim toStore As INDArray
			If useInplaceCompression Then
				compressor.compressi([object])
				toStore = [object]
			Else
				toStore = compressor.compress([object])
			End If

			If emulateIsAbsent Then
				lock.writeLock().lock()
			End If

			compressedEntries(key) = toStore

			If emulateIsAbsent Then
				lock.writeLock().unlock()
			End If
		End Sub

		''' <summary>
		''' Store object into storage
		''' </summary>
		''' <param name="key"> </param>
		''' <param name="array"> </param>
		Public Overridable Sub store(ByVal key As T, ByVal array() As Single) Implements AbstractStorage(Of T).store
			Dim toStore As INDArray = compressor.compress(array)

			If emulateIsAbsent Then
				lock.writeLock().lock()
			End If

			compressedEntries(key) = toStore

			If emulateIsAbsent Then
				lock.writeLock().unlock()
			End If
		End Sub

		''' <summary>
		''' Store object into storage
		''' </summary>
		''' <param name="key"> </param>
		''' <param name="array"> </param>
		Public Overridable Sub store(ByVal key As T, ByVal array() As Double) Implements AbstractStorage(Of T).store
			Dim toStore As INDArray = compressor.compress(array)

			If emulateIsAbsent Then
				lock.writeLock().lock()
			End If

			compressedEntries(key) = toStore

			If emulateIsAbsent Then
				lock.writeLock().unlock()
			End If
		End Sub

		''' <summary>
		''' Store object into storage, if it doesn't exist
		''' </summary>
		''' <param name="key"> </param>
		''' <param name="object"> </param>
		''' <returns> Returns TRUE if store operation was applied, FALSE otherwise </returns>
		Public Overridable Function storeIfAbsent(ByVal key As T, ByVal [object] As INDArray) As Boolean Implements AbstractStorage(Of T).storeIfAbsent
			Try
				If emulateIsAbsent Then
					lock.writeLock().lock()
				End If

				If compressedEntries.ContainsKey(key) Then
					Return False
				Else
					store(key, [object])
					Return True
				End If
			Finally
				If emulateIsAbsent Then
					lock.writeLock().unlock()
				End If
			End Try
		End Function

		''' <summary>
		''' Get object from the storage, by key
		''' </summary>
		''' <param name="key"> </param>
		Public Overridable Function get(ByVal key As T) As INDArray Implements AbstractStorage(Of T).get
			Try
				If emulateIsAbsent Then
					lock.readLock().lock()
				End If

				If containsKey(key) Then
					Dim result As INDArray = compressedEntries(key)

					' TODO: we don't save decompressed entries here, but something like LRU might be good idea
					Return compressor.decompress(result)
				Else
					Return Nothing
				End If
			Finally
				If emulateIsAbsent Then
					lock.readLock().unlock()
				End If
			End Try
		End Function

		''' <summary>
		''' This method checks, if storage contains specified key
		''' </summary>
		''' <param name="key">
		''' @return </param>
		Public Overridable Function containsKey(ByVal key As T) As Boolean Implements AbstractStorage(Of T).containsKey
			Try
				If emulateIsAbsent Then
					lock.readLock().lock()
				End If

				Return compressedEntries.ContainsKey(key)
			Finally
				If emulateIsAbsent Then
					lock.readLock().unlock()
				End If
			End Try
		End Function

		''' <summary>
		''' This method purges everything from storage
		''' </summary>
		Public Overridable Sub clear() Implements AbstractStorage(Of T).clear
			If emulateIsAbsent Then
				lock.writeLock().lock()
			End If

			compressedEntries.Clear()

			If emulateIsAbsent Then
				lock.writeLock().unlock()
			End If
		End Sub

		''' <summary>
		''' This method removes value by specified key
		''' </summary>
		''' <param name="key"> </param>
		Public Overridable Sub drop(ByVal key As T) Implements AbstractStorage(Of T).drop
			If emulateIsAbsent Then
				lock.writeLock().lock()
			End If

			compressedEntries.Remove(key)

			If emulateIsAbsent Then
				lock.writeLock().unlock()
			End If
		End Sub

		''' <summary>
		''' This method returns number of entries available in storage
		''' </summary>
		Public Overridable Function size() As Long Implements AbstractStorage(Of T).size
			Try
				If emulateIsAbsent Then
					lock.readLock().lock()
				End If

				Return compressedEntries.Count
			Finally
				If emulateIsAbsent Then
					lock.readLock().unlock()
				End If
			End Try
		End Function

		Public Class Builder(Of T)
			' we use NoOp as default compressor
'JAVA TO VB CONVERTER NOTE: The field compressor was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend compressor_Conflict As NDArrayCompressor = New NoOp()
'JAVA TO VB CONVERTER NOTE: The field useInplaceCompression was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend useInplaceCompression_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field emulateIsAbsent was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend emulateIsAbsent_Conflict As Boolean = False

			Public Sub New()

			End Sub

			''' <summary>
			''' This method defines, which compression algorithm will be used during storage
			''' Default value: NoOp();
			''' </summary>
			''' <param name="compressor">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> setCompressor(@NonNull NDArrayCompressor compressor)
			Public Overridable Function setCompressor(ByVal compressor As NDArrayCompressor) As Builder(Of T)
				Me.compressor_Conflict = compressor
				Return Me
			End Function

			''' <summary>
			''' If set to TRUE, all store/update calls will use inplace compression.
			''' If set to FALSE, original array won't be modified, and copy will be used.
			''' 
			''' Default value: FALSE;
			''' </summary>
			''' <param name="reallyUse">
			''' @return </param>
			Public Overridable Function useInplaceCompression(ByVal reallyUse As Boolean) As Builder(Of T)
				Me.useInplaceCompression_Conflict = reallyUse
				Return Me
			End Function

			''' <summary>
			''' If set to TRUE, all Read/Write locks will be used to emulate storeIfAbsent behaviour
			''' If set to FALSE, concurrency will be provided by ConcurrentHashMap at Java7 level
			''' 
			''' Default value: FALSE;
			''' </summary>
			''' <param name="reallyEmulate">
			''' @return </param>
			Public Overridable Function emulateIsAbsent(ByVal reallyEmulate As Boolean) As Builder(Of T)
				Me.emulateIsAbsent_Conflict = reallyEmulate
				Return Me
			End Function


			Public Overridable Function build() As CompressedRamStorage(Of T)
				Dim storage As New CompressedRamStorage(Of T)()
				storage.compressor = Me.compressor_Conflict
				storage.useInplaceCompression = Me.useInplaceCompression_Conflict
				storage.emulateIsAbsent = Me.emulateIsAbsent_Conflict

				Return storage
			End Function
		End Class
	End Class

End Namespace