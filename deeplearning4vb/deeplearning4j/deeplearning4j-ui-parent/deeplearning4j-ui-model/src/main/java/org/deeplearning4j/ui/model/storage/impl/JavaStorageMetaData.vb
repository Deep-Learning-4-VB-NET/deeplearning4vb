Imports System
Imports System.IO
Imports System.Reflection
Imports Data = lombok.Data
Imports IOUtils = org.apache.commons.compress.utils.IOUtils
Imports StorageMetaData = org.deeplearning4j.core.storage.StorageMetaData
Imports SbeUtil = org.deeplearning4j.ui.model.stats.impl.SbeUtil

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

Namespace org.deeplearning4j.ui.model.storage.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class JavaStorageMetaData implements org.deeplearning4j.core.storage.StorageMetaData
	<Serializable>
	Public Class JavaStorageMetaData
		Implements StorageMetaData

		Private timeStamp As Long
		Private sessionID As String
		Private typeID As String
		Private workerID As String
		Private initTypeClass As String
		Private updateTypeClass As String
		'Store serialized; saves class exceptions if we don't have the right class, and don't care about deserializing
		' on this machine, right now
		Private extraMeta() As SByte

		Public Sub New()
			'No arg constructor for serialization/deserialization
		End Sub

		Public Sub New(ByVal timeStamp As Long, ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String, ByVal initType As Type, ByVal updateType As Type)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Me.New(timeStamp, sessionID, typeID, workerID, (If(initType IsNot Nothing, initType.FullName, Nothing)), (If(updateType IsNot Nothing, updateType.FullName, Nothing)))
		End Sub

		Public Sub New(ByVal timeStamp As Long, ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String, ByVal initTypeClass As String, ByVal updateTypeClass As String)
			Me.New(timeStamp, sessionID, typeID, workerID, initTypeClass, updateTypeClass, Nothing)
		End Sub

		Public Sub New(ByVal timeStamp As Long, ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String, ByVal initTypeClass As String, ByVal updateTypeClass As String, ByVal extraMetaData As Serializable)
			Me.timeStamp = timeStamp
			Me.sessionID = sessionID
			Me.typeID = typeID
			Me.workerID = workerID
			Me.initTypeClass = initTypeClass
			Me.updateTypeClass = updateTypeClass
			Me.extraMeta = (If(extraMetaData Is Nothing, Nothing, SbeUtil.toBytesSerializable(extraMetaData)))
		End Sub

		Public Overridable Function encodingLengthBytes() As Integer
			'TODO - presumably a more efficient way to do this
			Dim encoded() As SByte = encode()
			Return encoded.Length
		End Function

		Public Overridable Function encode() As SByte()
			Dim baos As New MemoryStream()
			Try
					Using oos As New ObjectOutputStream(baos)
					oos.writeObject(Me)
					End Using
			Catch e As IOException
				Throw New Exception(e) 'Should never happen
			End Try
			Return baos.toByteArray()
		End Function

		Public Overridable Sub encode(ByVal buffer As ByteBuffer)
			buffer.put(encode())
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void encode(OutputStream outputStream) throws IOException
		Public Overridable Sub encode(ByVal outputStream As Stream)
			Using oos As New ObjectOutputStream(outputStream)
				oos.writeObject(Me)
			End Using
		End Sub

		Public Overridable Sub decode(ByVal decode() As SByte)
			Dim r As JavaStorageMetaData
			Try
					Using ois As New ObjectInputStream(New MemoryStream(decode))
					r = CType(ois.readObject(), JavaStorageMetaData)
					End Using
			Catch e As Exception When TypeOf e Is IOException OrElse TypeOf e Is ClassNotFoundException
				Throw New Exception(e) 'Should never happen
			End Try

			Dim fields() As System.Reflection.FieldInfo = GetType(JavaStorageMetaData).GetFields(BindingFlags.DeclaredOnly Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.Instance)
			For Each f As System.Reflection.FieldInfo In fields
				f.setAccessible(True)
				Try
					f.set(Me, f.get(r))
				Catch e As IllegalAccessException
					Throw New Exception(e) 'Should never happen
				End Try
			Next f
		End Sub

		Public Overridable Sub decode(ByVal buffer As ByteBuffer)
			Dim bytes(buffer.remaining() - 1) As SByte
			buffer.get(bytes)
			decode(bytes)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void decode(InputStream inputStream) throws IOException
		Public Overridable Sub decode(ByVal inputStream As Stream)
			decode(IOUtils.toByteArray(inputStream))
		End Sub

		Public Overridable ReadOnly Property ExtraMetaData As Serializable
			Get
				Return Nothing
			End Get
		End Property
	End Class

End Namespace