Imports System
Imports System.IO
Imports Data = lombok.Data
Imports DirectBuffer = org.agrona.DirectBuffer
Imports MutableDirectBuffer = org.agrona.MutableDirectBuffer
Imports UnsafeBuffer = org.agrona.concurrent.UnsafeBuffer
Imports IOUtils = org.apache.commons.io.IOUtils
Imports StorageMetaData = org.deeplearning4j.core.storage.StorageMetaData
Imports SbeUtil = org.deeplearning4j.ui.model.stats.impl.SbeUtil
Imports MessageHeaderDecoder = org.deeplearning4j.ui.model.stats.sbe.MessageHeaderDecoder
Imports MessageHeaderEncoder = org.deeplearning4j.ui.model.stats.sbe.MessageHeaderEncoder
Imports StorageMetaDataDecoder = org.deeplearning4j.ui.model.stats.sbe.StorageMetaDataDecoder
Imports StorageMetaDataEncoder = org.deeplearning4j.ui.model.stats.sbe.StorageMetaDataEncoder
Imports AgronaPersistable = org.deeplearning4j.ui.model.storage.AgronaPersistable

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
'ORIGINAL LINE: @Data public class SbeStorageMetaData implements org.deeplearning4j.core.storage.StorageMetaData, org.deeplearning4j.ui.model.storage.AgronaPersistable
	<Serializable>
	Public Class SbeStorageMetaData
		Implements StorageMetaData, AgronaPersistable


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

		Public Overridable ReadOnly Property ExtraMetaData As Serializable Implements StorageMetaData.getExtraMetaData
			Get
				Return SbeUtil.fromBytesSerializable(extraMeta)
			End Get
		End Property

		Public Overridable Function encodingLengthBytes() As Integer
			'TODO store byte[]s so we don't end up calculating again in encode
			'SBE buffer is composed of:
			'(a) Header: 8 bytes (4x uint16 = 8 bytes)
			'(b) timestamp: fixed length long value (8 bytes)
			'(b) 5 variable length fields. 4 bytes header (each) + content = 20 bytes + content
			'(c) Variable length byte[]. 4 bytes header + content

			Dim bufferSize As Integer = 8 + 8 + 20 + 4
			Dim bSessionID() As SByte = SbeUtil.toBytes(True, sessionID)
			Dim bTypeID() As SByte = SbeUtil.toBytes(True, typeID)
			Dim bWorkerID() As SByte = SbeUtil.toBytes(True, workerID)
			Dim bInitTypeClass() As SByte = SbeUtil.toBytes(True, initTypeClass)
			Dim bUpdateTypeClass() As SByte = SbeUtil.toBytes(True, updateTypeClass)
			Dim bExtraMetaData() As SByte = SbeUtil.toBytesSerializable(extraMeta)

			bufferSize += bSessionID.Length + bTypeID.Length + bWorkerID.Length + bInitTypeClass.Length + bUpdateTypeClass.Length + bExtraMetaData.Length

			Return bufferSize
		End Function

		Public Overridable Function encode() As SByte()
			Dim bytes(encodingLengthBytes() - 1) As SByte
			Dim buffer As MutableDirectBuffer = New UnsafeBuffer(bytes)
			encode(buffer)
			Return bytes
		End Function

		Public Overridable Sub encode(ByVal buffer As ByteBuffer)
			encode(New UnsafeBuffer(buffer))
		End Sub

		Public Overridable Sub encode(ByVal buffer As MutableDirectBuffer) Implements AgronaPersistable.encode
			Dim enc As New MessageHeaderEncoder()
			Dim smde As New StorageMetaDataEncoder()

			enc.wrap(buffer, 0).blockLength(smde.sbeBlockLength()).templateId(smde.sbeTemplateId()).schemaId(smde.sbeSchemaId()).version(smde.sbeSchemaVersion())

			Dim offset As Integer = enc.encodedLength() 'Expect 8 bytes

			Dim bSessionID() As SByte = SbeUtil.toBytes(True, sessionID)
			Dim bTypeID() As SByte = SbeUtil.toBytes(True, typeID)
			Dim bWorkerID() As SByte = SbeUtil.toBytes(True, workerID)
			Dim bInitTypeClass() As SByte = SbeUtil.toBytes(True, initTypeClass)
			Dim bUpdateTypeClass() As SByte = SbeUtil.toBytes(True, updateTypeClass)

			smde.wrap(buffer, offset).timeStamp(timeStamp)

			Dim ext As StorageMetaDataEncoder.ExtraMetaDataBytesEncoder = smde.extraMetaDataBytesCount(If(extraMeta Is Nothing, 0, extraMeta.Length))
			If extraMeta IsNot Nothing Then
				For Each b As SByte In extraMeta
					ext.next().bytes(b)
				Next b
			End If

			smde.putSessionID(bSessionID, 0, bSessionID.Length).putTypeID(bTypeID, 0, bTypeID.Length).putWorkerID(bWorkerID, 0, bWorkerID.Length).putInitTypeClass(bInitTypeClass, 0, bInitTypeClass.Length).putUpdateTypeClass(bUpdateTypeClass, 0, bUpdateTypeClass.Length)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void encode(java.io.OutputStream outputStream) throws java.io.IOException
		Public Overridable Sub encode(ByVal outputStream As Stream)
			'TODO there may be more efficient way of doing this
			outputStream.Write(encode(), 0, encode().Length)
		End Sub

		Public Overridable Sub decode(ByVal decode() As SByte)
			Dim buffer As MutableDirectBuffer = New UnsafeBuffer(decode)
			decode(buffer)
		End Sub

		Public Overridable Sub decode(ByVal buffer As ByteBuffer)
			decode(New UnsafeBuffer(buffer))
		End Sub

		Public Overridable Sub decode(ByVal buffer As DirectBuffer) Implements AgronaPersistable.decode

			Dim dec As New MessageHeaderDecoder()
			dec.wrap(buffer, 0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int blockLength = dec.blockLength();
			Dim blockLength As Integer = dec.blockLength()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int version = dec.version();
			Dim version As Integer = dec.version()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int headerLength = dec.encodedLength();
			Dim headerLength As Integer = dec.encodedLength()

			'TODO Validate header, version etc

			Dim smdd As New StorageMetaDataDecoder()
			smdd.wrap(buffer, headerLength, blockLength, version)
			timeStamp = smdd.timeStamp()

			Dim ext As StorageMetaDataDecoder.ExtraMetaDataBytesDecoder = smdd.extraMetaDataBytes()
			Dim length As Integer = ext.count()
			If length > 0 Then
				extraMeta = New SByte(length - 1){}
				Dim i As Integer = 0
				For Each d As StorageMetaDataDecoder.ExtraMetaDataBytesDecoder In ext
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: extraMeta[i++] = d.bytes();
					extraMeta(i) = d.bytes()
						i += 1
				Next d
			End If

			sessionID = smdd.sessionID()
			typeID = smdd.typeID()
			workerID = smdd.workerID()
			initTypeClass = smdd.initTypeClass()
			updateTypeClass = smdd.updateTypeClass()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void decode(java.io.InputStream inputStream) throws java.io.IOException
		Public Overridable Sub decode(ByVal inputStream As Stream)
			Dim bytes() As SByte = IOUtils.toByteArray(inputStream)
			decode(bytes)
		End Sub
	End Class

End Namespace