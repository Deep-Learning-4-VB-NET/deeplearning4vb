Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Reflection
Imports Data = lombok.Data
Imports IOUtils = org.apache.commons.compress.utils.IOUtils
Imports StatsInitializationReport = org.deeplearning4j.ui.model.stats.api.StatsInitializationReport

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

Namespace org.deeplearning4j.ui.model.stats.impl.java


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class JavaStatsInitializationReport implements org.deeplearning4j.ui.model.stats.api.StatsInitializationReport
	<Serializable>
	Public Class JavaStatsInitializationReport
		Implements StatsInitializationReport

		Private sessionID As String
		Private typeID As String
		Private workerID As String
		Private timeStamp As Long

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
			Me.sessionID = sessionID
			Me.typeID = typeID
			Me.workerID = workerID
			Me.timeStamp = timeStamp
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
			Dim r As JavaStatsInitializationReport
			Try
					Using ois As New ObjectInputStream(New MemoryStream(decode))
					r = CType(ois.readObject(), JavaStatsInitializationReport)
					End Using
			Catch e As Exception When TypeOf e Is IOException OrElse TypeOf e Is ClassNotFoundException
				Throw New Exception(e) 'Should never happen
			End Try

			Dim fields() As System.Reflection.FieldInfo = GetType(JavaStatsInitializationReport).GetFields(BindingFlags.DeclaredOnly Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.Instance)
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
	End Class

End Namespace