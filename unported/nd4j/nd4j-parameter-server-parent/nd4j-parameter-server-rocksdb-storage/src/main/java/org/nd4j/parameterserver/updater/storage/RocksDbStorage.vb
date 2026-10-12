Imports System
Imports UnsafeBuffer = org.agrona.concurrent.UnsafeBuffer
Imports NDArrayMessage = org.nd4j.aeron.ipc.NDArrayMessage
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Options = org.rocksdb.Options
Imports RocksDB = org.rocksdb.RocksDB
Imports RocksDBException = org.rocksdb.RocksDBException
Imports RocksIterator = org.rocksdb.RocksIterator

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

Namespace org.nd4j.parameterserver.updater.storage


	Public Class RocksDbStorage
		Inherits BaseUpdateStorage
		Implements AutoCloseable

		Shared Sub New()
			' a static method that loads the RocksDB C++ library.
			RocksDB.loadLibrary()
		End Sub


		Private db As RocksDB
		Private size As Integer = 0

		Public Sub New(ByVal dbPath As String)
			' that determines the behavior of a database.
			Dim options As Options = (New Options()).setCreateIfMissing(True)
			Try
				' a factory method that returns a RocksDB instance
				db = RocksDB.open(options, dbPath)
				' do something
			Catch e As RocksDBException
				' do some error handling

			End Try
		End Sub

		''' <summary>
		''' Add an ndarray to the storage
		''' </summary>
		''' <param name="array"> the array to add </param>
		Public Overrides Sub addUpdate(ByVal array As NDArrayMessage)
			Dim directBuffer As UnsafeBuffer = CType(NDArrayMessage.toBuffer(array), UnsafeBuffer)
			Dim data() As SByte = directBuffer.byteArray()
			If data Is Nothing Then
				data = New SByte(directBuffer.capacity() - 1){}
				directBuffer.getBytes(0, data, 0, data.Length)
			End If
			Dim key() As SByte = ByteBuffer.allocate(4).putInt(size).array()
			Try
				db.put(key, data)
			Catch e As RocksDBException
				Throw New Exception(e)
			End Try

			size += 1

		End Sub

		''' <summary>
		''' The number of updates added
		''' to the update storage
		''' 
		''' @return
		''' </summary>
		Public Overrides Function numUpdates() As Integer
			Return size
		End Function

		''' <summary>
		''' Clear the array storage
		''' </summary>
		Public Overrides Sub clear()
			Dim iterator As RocksIterator = db.newIterator()
			Do While iterator.isValid()
				Try
					db.remove(iterator.key())
				Catch e As RocksDBException
					Throw New Exception(e)
				End Try
			Loop
			iterator.close()
			size = 0
		End Sub

		''' <summary>
		''' A method for actually performing the implementation
		''' of retrieving the ndarray
		''' </summary>
		''' <param name="index"> the index of the <seealso cref="INDArray"/> to get </param>
		''' <returns> the ndarray at the specified index </returns>
		Public Overrides Function doGetUpdate(ByVal index As Integer) As NDArrayMessage
			Dim key() As SByte = ByteBuffer.allocate(4).putInt(index).array()
			Try
				Dim unsafeBuffer As New UnsafeBuffer(db.get(key))
				Return NDArrayMessage.fromBuffer(unsafeBuffer, 0)
			Catch e As RocksDBException
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Close the database
		''' </summary>
		Public Overrides Sub close()
			db.close()
		End Sub
	End Class

End Namespace