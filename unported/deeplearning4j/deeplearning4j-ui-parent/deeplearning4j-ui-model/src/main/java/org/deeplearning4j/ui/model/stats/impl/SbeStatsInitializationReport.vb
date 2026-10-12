Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Data = lombok.Data
Imports DirectBuffer = org.agrona.DirectBuffer
Imports MutableDirectBuffer = org.agrona.MutableDirectBuffer
Imports UnsafeBuffer = org.agrona.concurrent.UnsafeBuffer
Imports IOUtils = org.apache.commons.io.IOUtils
Imports StatsInitializationReport = org.deeplearning4j.ui.model.stats.api.StatsInitializationReport
Imports org.deeplearning4j.ui.model.stats.sbe
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

Namespace org.deeplearning4j.ui.model.stats.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class SbeStatsInitializationReport implements org.deeplearning4j.ui.model.stats.api.StatsInitializationReport, org.deeplearning4j.ui.model.storage.AgronaPersistable
	<Serializable>
	Public Class SbeStatsInitializationReport
		Implements StatsInitializationReport, AgronaPersistable

'JAVA TO VB CONVERTER NOTE: The field sessionID was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private sessionID_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field typeID was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private typeID_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field workerID was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private workerID_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field timeStamp was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private timeStamp_Conflict As Long

'JAVA TO VB CONVERTER NOTE: The field hasSoftwareInfo was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private hasSoftwareInfo_Conflict As Boolean
'JAVA TO VB CONVERTER NOTE: The field hasHardwareInfo was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private hasHardwareInfo_Conflict As Boolean
'JAVA TO VB CONVERTER NOTE: The field hasModelInfo was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private hasModelInfo_Conflict As Boolean

		Private swArch As String
		Private swOsName As String
		Private swJvmName As String
		Private swJvmVersion As String
		Private swJvmSpecVersion As String
		Private swNd4jBackendClass As String
		Private swNd4jDataTypeName As String
		Private swHostName As String
		Private swJvmUID As String
		Private swEnvironmentInfo As IDictionary(Of String, String)

		Private hwJvmAvailableProcessors As Integer
		Private hwNumDevices As Integer
		Private hwJvmMaxMemory As Long
		Private hwOffHeapMaxMemory As Long
		Private hwDeviceTotalMemory() As Long
		Private hwDeviceDescription() As String
		Private hwHardwareUID As String

		Private modelClassName As String
		Private modelConfigJson As String
		Private modelParamNames() As String
		Private modelNumLayers As Integer
		Private modelNumParams As Long

		Public Overridable Sub reportIDs(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String, ByVal timeStamp As Long) Implements StatsInitializationReport.reportIDs
			Me.sessionID_Conflict = sessionID
			Me.typeID_Conflict = typeID
			Me.workerID_Conflict = workerID
			Me.timeStamp_Conflict = timeStamp
		End Sub

		Public Overridable Sub reportSoftwareInfo(ByVal arch As String, ByVal osName As String, ByVal jvmName As String, ByVal jvmVersion As String, ByVal jvmSpecVersion As String, ByVal nd4jBackendClass As String, ByVal nd4jDataTypeName As String, ByVal hostname As String, ByVal jvmUid As String, ByVal swEnvironmentInfo As IDictionary(Of String, String)) Implements StatsInitializationReport.reportSoftwareInfo
			Me.swArch = arch
			Me.swOsName = osName
			Me.swJvmName = jvmName
			Me.swJvmVersion = jvmVersion
			Me.swJvmSpecVersion = jvmSpecVersion
			Me.swNd4jBackendClass = nd4jBackendClass
			Me.swNd4jDataTypeName = nd4jDataTypeName
			Me.swHostName = hostname
			Me.swJvmUID = jvmUid
			Me.swEnvironmentInfo = swEnvironmentInfo
			hasSoftwareInfo_Conflict = True
		End Sub

		Public Overridable Sub reportHardwareInfo(ByVal jvmAvailableProcessors As Integer, ByVal numDevices As Integer, ByVal jvmMaxMemory As Long, ByVal offHeapMaxMemory As Long, ByVal deviceTotalMemory() As Long, ByVal deviceDescription() As String, ByVal hardwareUID As String) Implements StatsInitializationReport.reportHardwareInfo
			Me.hwJvmAvailableProcessors = jvmAvailableProcessors
			Me.hwNumDevices = numDevices
			Me.hwJvmMaxMemory = jvmMaxMemory
			Me.hwOffHeapMaxMemory = offHeapMaxMemory
			Me.hwDeviceTotalMemory = deviceTotalMemory
			Me.hwDeviceDescription = deviceDescription
			Me.hwHardwareUID = hardwareUID
			hasHardwareInfo_Conflict = True
		End Sub

		Public Overridable Sub reportModelInfo(ByVal modelClassName As String, ByVal modelConfigJson As String, ByVal modelParamNames() As String, ByVal numLayers As Integer, ByVal numParams As Long) Implements StatsInitializationReport.reportModelInfo
			Me.modelClassName = modelClassName
			Me.modelConfigJson = modelConfigJson
			Me.modelParamNames = modelParamNames
			Me.modelNumLayers = numLayers
			Me.modelNumParams = numParams
			hasModelInfo_Conflict = True
		End Sub

		Public Overridable Function hasSoftwareInfo() As Boolean Implements StatsInitializationReport.hasSoftwareInfo
			Return hasSoftwareInfo_Conflict
		End Function

		Public Overridable Function hasHardwareInfo() As Boolean Implements StatsInitializationReport.hasHardwareInfo
			Return hasHardwareInfo_Conflict
		End Function

		Public Overridable Function hasModelInfo() As Boolean Implements StatsInitializationReport.hasModelInfo
			Return hasModelInfo_Conflict
		End Function



		Private Sub clearHwFields()
			hwDeviceTotalMemory = Nothing
			hwDeviceDescription = Nothing
			hwHardwareUID = Nothing
		End Sub

		Private Sub clearSwFields()
			swArch = Nothing
			swOsName = Nothing
			swJvmName = Nothing
			swJvmVersion = Nothing
			swJvmSpecVersion = Nothing
			swNd4jBackendClass = Nothing
			swNd4jDataTypeName = Nothing
			swHostName = Nothing
			swJvmUID = Nothing
		End Sub

		Private Sub clearModelFields()
			modelClassName = Nothing
			modelConfigJson = Nothing
			modelParamNames = Nothing
		End Sub

		Public Overridable ReadOnly Property SessionID As String
			Get
				Return sessionID_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property TypeID As String
			Get
				Return typeID_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property WorkerID As String
			Get
				Return workerID_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property TimeStamp As Long
			Get
				Return timeStamp_Conflict
			End Get
		End Property

		Public Overridable Function encodingLengthBytes() As Integer
			'TODO reuse the byte[]s here, to avoid converting them twice...

			'First: need to determine how large a buffer to use.
			'Buffer is composed of:
			'(a) Header: 8 bytes (4x uint16 = 8 bytes)
			'(b) Fixed length entries length (sie.BlockLength())
			'(c) Group 1: Hardware devices (GPUs) max memory: 4 bytes header + nEntries * 8 (int64) + nEntries * variable length Strings (header + content)  = 4 + 8*n + content
			'(d) Group 2: Software device info: 4 bytes header + 2x variable length Strings for each
			'(d) Group 3: Parameter names: 4 bytes header + nEntries * variable length strings (header + content) = 4 + content
			'(e) Variable length fields: 15 String length fields. Size: 4 bytes header, plus content. 60 bytes header
			'Fixed length + repeating groups + variable length...
			Dim sie As New StaticInfoEncoder()
			Dim bufferSize As Integer = 8 + sie.sbeBlockLength() + 4 + 4 + 60 'header + fixed values + group headers + variable length headers

			'For variable length field lengths: easist way is simply to convert to UTF-8
			'Of course, it is possible to calculate it first - but we might as well convert (1 pass), rather than count then convert (2 passes)
			Dim bSessionId() As SByte = SbeUtil.toBytes(True, sessionID_Conflict)
			Dim bTypeId() As SByte = SbeUtil.toBytes(True, typeID_Conflict)
			Dim bWorkerId() As SByte = SbeUtil.toBytes(True, workerID_Conflict)

			Dim bswArch() As SByte = SbeUtil.toBytes(hasSoftwareInfo_Conflict, swArch)
			Dim bswOsName() As SByte = SbeUtil.toBytes(hasSoftwareInfo_Conflict, swOsName)
			Dim bswJvmName() As SByte = SbeUtil.toBytes(hasSoftwareInfo_Conflict, swJvmName)
			Dim bswJvmVersion() As SByte = SbeUtil.toBytes(hasSoftwareInfo_Conflict, swJvmVersion)
			Dim bswJvmSpecVersion() As SByte = SbeUtil.toBytes(hasSoftwareInfo_Conflict, swJvmSpecVersion)
			Dim bswNd4jBackendClass() As SByte = SbeUtil.toBytes(hasSoftwareInfo_Conflict, swNd4jBackendClass)
			Dim bswNd4jDataTypeName() As SByte = SbeUtil.toBytes(hasSoftwareInfo_Conflict, swNd4jDataTypeName)
			Dim bswHostname() As SByte = SbeUtil.toBytes(hasSoftwareInfo_Conflict, swHostName)
			Dim bswJvmUID() As SByte = SbeUtil.toBytes(hasSoftwareInfo_Conflict, swJvmUID)
			Dim bHwHardwareUID() As SByte = SbeUtil.toBytes(hasHardwareInfo_Conflict, hwHardwareUID)
			Dim bmodelConfigClass() As SByte = SbeUtil.toBytes(hasModelInfo_Conflict, modelClassName)
			Dim bmodelConfigJson() As SByte = SbeUtil.toBytes(hasModelInfo_Conflict, modelConfigJson)

			Dim bhwDeviceDescription()() As SByte = SbeUtil.toBytes(hasHardwareInfo_Conflict, hwDeviceDescription)
			Dim bswEnvInfo()()() As SByte = SbeUtil.toBytes(swEnvironmentInfo)
			Dim bModelParamNames()() As SByte = SbeUtil.toBytes(hasModelInfo_Conflict, modelParamNames)



			bufferSize += bSessionId.Length + bTypeId.Length + bWorkerId.Length

			bufferSize += 4 'swEnvironmentInfo group header (always present)
			If hasSoftwareInfo_Conflict Then
				bufferSize += SbeUtil.length(bswArch)
				bufferSize += SbeUtil.length(bswOsName)
				bufferSize += SbeUtil.length(bswJvmName)
				bufferSize += SbeUtil.length(bswJvmVersion)
				bufferSize += SbeUtil.length(bswJvmSpecVersion)
				bufferSize += SbeUtil.length(bswNd4jBackendClass)
				bufferSize += SbeUtil.length(bswNd4jDataTypeName)
				bufferSize += SbeUtil.length(bswHostname)
				bufferSize += SbeUtil.length(bswJvmUID)
				'For each entry: 2 variable-length headers (2x4 bytes each) + content
				Dim envCount As Integer = (If(bswEnvInfo IsNot Nothing, bswEnvInfo.Length, 0))
				bufferSize += envCount * 8
				bufferSize += SbeUtil.length(bswEnvInfo)
			End If
			Dim nHWDeviceStats As Integer = hwNumDevices
			If Not hasHardwareInfo_Conflict Then
				nHWDeviceStats = 0
			End If
			If hasHardwareInfo_Conflict Then
				'Device info group:
				bufferSize += hwNumDevices * 8 'fixed content in group: int64 -> 8 bytes. Encode an entry, even if hwDeviceTotalMemory is null
				bufferSize += hwNumDevices * 4 'uint32: 4 bytes per entry for var length header...; as above
				bufferSize += SbeUtil.length(bhwDeviceDescription)
				bufferSize += SbeUtil.length(bHwHardwareUID)
			End If
			If hasModelInfo_Conflict Then
				bufferSize += SbeUtil.length(bmodelConfigClass)
				bufferSize += SbeUtil.length(bmodelConfigJson)
				bufferSize += SbeUtil.length(bModelParamNames)
				bufferSize += (If(bModelParamNames Is Nothing, 0, bModelParamNames.Length * 4)) 'uint32: 4 bytes per entry for var length header...
			End If

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
			Dim sie As New StaticInfoEncoder()

			Dim bSessionId() As SByte = SbeUtil.toBytes(True, sessionID_Conflict)
			Dim bTypeId() As SByte = SbeUtil.toBytes(True, typeID_Conflict)
			Dim bWorkerId() As SByte = SbeUtil.toBytes(True, workerID_Conflict)

			Dim bswArch() As SByte = SbeUtil.toBytes(hasSoftwareInfo_Conflict, swArch)
			Dim bswOsName() As SByte = SbeUtil.toBytes(hasSoftwareInfo_Conflict, swOsName)
			Dim bswJvmName() As SByte = SbeUtil.toBytes(hasSoftwareInfo_Conflict, swJvmName)
			Dim bswJvmVersion() As SByte = SbeUtil.toBytes(hasSoftwareInfo_Conflict, swJvmVersion)
			Dim bswJvmSpecVersion() As SByte = SbeUtil.toBytes(hasSoftwareInfo_Conflict, swJvmSpecVersion)
			Dim bswNd4jBackendClass() As SByte = SbeUtil.toBytes(hasSoftwareInfo_Conflict, swNd4jBackendClass)
			Dim bswNd4jDataTypeName() As SByte = SbeUtil.toBytes(hasSoftwareInfo_Conflict, swNd4jDataTypeName)
			Dim bswHostname() As SByte = SbeUtil.toBytes(hasSoftwareInfo_Conflict, swHostName)
			Dim bswJvmUID() As SByte = SbeUtil.toBytes(hasSoftwareInfo_Conflict, swJvmUID)
			Dim bHwHardwareUID() As SByte = SbeUtil.toBytes(hasHardwareInfo_Conflict, hwHardwareUID)
			Dim bmodelConfigClass() As SByte = SbeUtil.toBytes(hasModelInfo_Conflict, modelClassName)
			Dim bmodelConfigJson() As SByte = SbeUtil.toBytes(hasModelInfo_Conflict, modelConfigJson)

			Dim bhwDeviceDescription()() As SByte = SbeUtil.toBytes(hasHardwareInfo_Conflict, hwDeviceDescription)
			Dim bswEnvInfo()()() As SByte = SbeUtil.toBytes(swEnvironmentInfo)
			Dim bModelParamNames()() As SByte = SbeUtil.toBytes(hasModelInfo_Conflict, modelParamNames)

			enc.wrap(buffer, 0).blockLength(sie.sbeBlockLength()).templateId(sie.sbeTemplateId()).schemaId(sie.sbeSchemaId()).version(sie.sbeSchemaVersion())

			Dim offset As Integer = enc.encodedLength() 'Expect 8 bytes...

			'Fixed length fields: always encoded, whether present or not.
			sie.wrap(buffer, offset).time(timeStamp_Conflict).fieldsPresent().softwareInfo(hasSoftwareInfo_Conflict).hardwareInfo(hasHardwareInfo_Conflict).modelInfo(hasModelInfo_Conflict)
			sie.hwJvmProcessors(hwJvmAvailableProcessors).hwNumDevices(CShort(hwNumDevices)).hwJvmMaxMemory(hwJvmMaxMemory).hwOffheapMaxMemory(hwOffHeapMaxMemory).modelNumLayers(modelNumLayers).modelNumParams(modelNumParams)
			'Device info group...
			Dim hwdEnc As StaticInfoEncoder.HwDeviceInfoGroupEncoder = sie.hwDeviceInfoGroupCount(hwNumDevices)
			Dim nHWDeviceStats As Integer = (If(hasHardwareInfo_Conflict, hwNumDevices, 0))
			For i As Integer = 0 To nHWDeviceStats - 1
				Dim maxMem As Long = If(hwDeviceTotalMemory Is Nothing OrElse hwDeviceTotalMemory.Length <= i, 0, hwDeviceTotalMemory(i))
				Dim descr() As SByte = If(bhwDeviceDescription Is Nothing OrElse bhwDeviceDescription.Length <= i, SbeUtil.EMPTY_BYTES, bhwDeviceDescription(i))
				If descr Is Nothing Then
					descr = SbeUtil.EMPTY_BYTES
				End If
				hwdEnc.next().deviceMemoryMax(maxMem).putDeviceDescription(descr, 0, descr.Length)
			Next i

			'Environment info group
			Dim numEnvValues As Integer = (If(hasSoftwareInfo_Conflict AndAlso swEnvironmentInfo IsNot Nothing, swEnvironmentInfo.Count, 0))
			Dim swEnv As StaticInfoEncoder.SwEnvironmentInfoEncoder = sie.swEnvironmentInfoCount(numEnvValues)
			If numEnvValues > 0 Then
				Dim mapAsBytes()()() As SByte = SbeUtil.toBytes(swEnvironmentInfo)
				For Each entryBytes As SByte()() In mapAsBytes
					swEnv.next().putEnvKey(entryBytes(0), 0, entryBytes(0).Length).putEnvValue(entryBytes(1), 0, entryBytes(1).Length)
				Next entryBytes
			End If

			Dim nParamNames As Integer = If(modelParamNames Is Nothing, 0, modelParamNames.Length)
			Dim mpnEnc As StaticInfoEncoder.ModelParamNamesEncoder = sie.modelParamNamesCount(nParamNames)
			For i As Integer = 0 To nParamNames - 1
				mpnEnc.next().putModelParamNames(bModelParamNames(i), 0, bModelParamNames(i).Length)
			Next i

			'In the case of !hasSoftwareInfo: these will all be empty byte arrays... still need to encode them (for 0 length) however
			sie.putSessionID(bSessionId, 0, bSessionId.Length).putTypeID(bTypeId, 0, bTypeId.Length).putWorkerID(bWorkerId, 0, bWorkerId.Length).putSwArch(bswArch, 0, bswArch.Length).putSwOsName(bswOsName, 0, bswOsName.Length).putSwJvmName(bswJvmName, 0, bswJvmName.Length).putSwJvmVersion(bswJvmVersion, 0, bswJvmVersion.Length).putSwJvmSpecVersion(bswJvmSpecVersion, 0, bswJvmSpecVersion.Length).putSwNd4jBackendClass(bswNd4jBackendClass, 0, bswNd4jBackendClass.Length).putSwNd4jDataTypeName(bswNd4jDataTypeName, 0, bswNd4jDataTypeName.Length).putSwHostName(bswHostname, 0, bswHostname.Length).putSwJvmUID(bswJvmUID, 0, bswJvmUID.Length).putHwHardwareUID(bHwHardwareUID, 0, bHwHardwareUID.Length)
			'Similar: !hasModelInfo -> empty byte[]
			sie.putModelConfigClassName(bmodelConfigClass, 0, bmodelConfigClass.Length).putModelConfigJson(bmodelConfigJson, 0, bmodelConfigJson.Length)
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
			'TODO we could do this much more efficiently, with buffer re-use, etc.
			Dim dec As New MessageHeaderDecoder()
			Dim sid As New StaticInfoDecoder()
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
			'TODO: in general, we should check the header, version, schema etc. But we don't have any other versions yet.

			sid.wrap(buffer, headerLength, blockLength, version)
			timeStamp_Conflict = sid.time()
			Dim fields As InitFieldsPresentDecoder = sid.fieldsPresent()
			hasSoftwareInfo_Conflict = fields.softwareInfo()
			hasHardwareInfo_Conflict = fields.hardwareInfo()
			hasModelInfo_Conflict = fields.modelInfo()

			'These fields: always present, even if !hasHardwareInfo
			hwJvmAvailableProcessors = sid.hwJvmProcessors()
			hwNumDevices = sid.hwNumDevices()
			hwJvmMaxMemory = sid.hwJvmMaxMemory()
			hwOffHeapMaxMemory = sid.hwOffheapMaxMemory()
			modelNumLayers = sid.modelNumLayers()
			modelNumParams = sid.modelNumParams()

			'Hardware device info group
			Dim hwDeviceInfoGroupDecoder As StaticInfoDecoder.HwDeviceInfoGroupDecoder = sid.hwDeviceInfoGroup()
			Dim count As Integer = hwDeviceInfoGroupDecoder.count()
			If count > 0 Then
				hwDeviceTotalMemory = New Long(count - 1){}
				hwDeviceDescription = New String(count - 1){}
			End If
			Dim i As Integer = 0
			For Each hw As StaticInfoDecoder.HwDeviceInfoGroupDecoder In hwDeviceInfoGroupDecoder
				hwDeviceTotalMemory(i) = hw.deviceMemoryMax()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: hwDeviceDescription[i++] = hw.deviceDescription();
				hwDeviceDescription(i) = hw.deviceDescription()
					i += 1
			Next hw

			'Environment info group
			i = 0
			Dim swEnvDecoder As StaticInfoDecoder.SwEnvironmentInfoDecoder = sid.swEnvironmentInfo()
			If swEnvDecoder.count() > 0 Then
				swEnvironmentInfo = New Dictionary(Of String, String)()
			End If
			For Each env As StaticInfoDecoder.SwEnvironmentInfoDecoder In swEnvDecoder
				Dim key As String = env.envKey()
				Dim value As String = env.envValue()
				swEnvironmentInfo(key) = value
			Next env

			i = 0
			Dim mpdec As StaticInfoDecoder.ModelParamNamesDecoder = sid.modelParamNames()
			Dim mpnCount As Integer = mpdec.count()
			modelParamNames = New String(mpnCount - 1){}
			For Each mp As StaticInfoDecoder.ModelParamNamesDecoder In mpdec
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: modelParamNames[i++] = mp.modelParamNames();
				modelParamNames(i) = mp.modelParamNames()
					i += 1
			Next mp
			'Variable length data. Even if it is missing: still needs to be read, to advance buffer
			'Again, the exact order of these calls matters here
			sessionID_Conflict = sid.sessionID()
			typeID_Conflict = sid.typeID()
			workerID_Conflict = sid.workerID()
			swArch = sid.swArch()
			swOsName = sid.swOsName()
			swJvmName = sid.swJvmName()
			swJvmVersion = sid.swJvmVersion()
			swJvmSpecVersion = sid.swJvmSpecVersion()
			swNd4jBackendClass = sid.swNd4jBackendClass()
			swNd4jDataTypeName = sid.swNd4jDataTypeName()
			swHostName = sid.swHostName()
			swJvmUID = sid.swJvmUID()
			If Not hasSoftwareInfo_Conflict Then
				clearSwFields()
			End If
			hwHardwareUID = sid.hwHardwareUID()
			If Not hasHardwareInfo_Conflict Then
				clearHwFields()
			End If
			modelClassName = sid.modelConfigClassName()
			modelConfigJson = sid.modelConfigJson()
			If Not hasModelInfo_Conflict Then
				clearModelFields()
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void decode(java.io.InputStream inputStream) throws java.io.IOException
		Public Overridable Sub decode(ByVal inputStream As Stream)
			Dim bytes() As SByte = IOUtils.toByteArray(inputStream)
			decode(bytes)
		End Sub
	End Class

End Namespace