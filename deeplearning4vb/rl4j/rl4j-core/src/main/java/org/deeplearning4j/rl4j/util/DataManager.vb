Imports System
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic
Imports ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Getter = lombok.Getter
Imports Value = lombok.Value
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.deeplearning4j.rl4j.learning
Imports org.deeplearning4j.rl4j.learning
Imports ILearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.ILearningConfiguration
Imports org.deeplearning4j.rl4j.network.ac
Imports DQN = org.deeplearning4j.rl4j.network.dqn.DQN
Imports org.deeplearning4j.rl4j.network.dqn
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.rl4j.util


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DataManager implements IDataManager
	Public Class DataManager
		Implements IDataManager

		Private ReadOnly home As String = System.getProperty("user.home")
		Private ReadOnly mapper As New ObjectMapper()
		Private dataRoot As String = home & "/" & Constants.DATA_DIR
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean saveData;
		Private saveData As Boolean
		Private currentDir As String

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public DataManager() throws java.io.IOException
		Public Sub New()
			create(dataRoot, False)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public DataManager(boolean saveData) throws java.io.IOException
		Public Sub New(ByVal saveData As Boolean)
			create(dataRoot, saveData)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public DataManager(String dataRoot, boolean saveData) throws java.io.IOException
		Public Sub New(ByVal dataRoot As String, ByVal saveData As Boolean)
			create(dataRoot, saveData)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void writeEntry(java.io.InputStream inputStream, java.util.zip.ZipOutputStream zipStream) throws java.io.IOException
		Private Shared Sub writeEntry(ByVal inputStream As Stream, ByVal zipStream As ZipOutputStream)
			Dim bytes(1023) As SByte
			Dim bytesRead As Integer
			bytesRead = inputStream.Read(bytes, 0, bytes.Length)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((bytesRead = inputStream.read(bytes)) != -1)
			Do While bytesRead <> -1
				zipStream.write(bytes, 0, bytesRead)
					bytesRead = inputStream.Read(bytes, 0, bytes.Length)
			Loop
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void save(String path, org.deeplearning4j.rl4j.learning.ILearning learning) throws java.io.IOException
		Public Shared Sub save(ByVal path As String, ByVal learning As ILearning)
			Using os As New java.io.BufferedOutputStream(New FileStream(path, FileMode.Create, FileAccess.Write))
				save(os, learning)
			End Using
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void save(java.io.OutputStream os, org.deeplearning4j.rl4j.learning.ILearning learning) throws java.io.IOException
		Public Shared Sub save(ByVal os As Stream, ByVal learning As ILearning)

			Using zipfile As New java.util.zip.ZipOutputStream(os)

				Dim config As New ZipEntry("configuration.json")
				zipfile.putNextEntry(config)
				Dim json As String = (New ObjectMapper()).writeValueAsString(learning.getConfiguration())
				writeEntry(New MemoryStream(json.GetBytes()), zipfile)

				Try
					Dim dqn As New ZipEntry("dqn.bin")
					zipfile.putNextEntry(dqn)

					Dim bos As New MemoryStream()
					If TypeOf learning Is NeuralNetFetchable Then
						CType(learning, NeuralNetFetchable).getNeuralNet().save(bos)
					End If
					bos.Flush()
					bos.Close()

					Dim inputStream As Stream = New MemoryStream(bos.toByteArray())
					writeEntry(inputStream, zipfile)
				Catch e As System.NotSupportedException
					Dim bos As New MemoryStream()
					Dim bos2 As New MemoryStream()
					CType(CType(learning, NeuralNetFetchable).getNeuralNet(), IActorCritic).save(bos, bos2)

					bos.Flush()
					bos.Close()
					Dim inputStream As Stream = New MemoryStream(bos.toByteArray())
					Dim value As New ZipEntry("value.bin")
					zipfile.putNextEntry(value)
					writeEntry(inputStream, zipfile)

					bos2.Flush()
					bos2.Close()
					Dim inputStream2 As Stream = New MemoryStream(bos2.toByteArray())
					Dim policy As New ZipEntry("policy.bin")
					zipfile.putNextEntry(policy)
					writeEntry(inputStream2, zipfile)
				End Try

				If learning.getHistoryProcessor() IsNot Nothing Then
					Dim hpconf As New ZipEntry("hpconf.bin")
					zipfile.putNextEntry(hpconf)

					Dim bos2 As New MemoryStream()
					If TypeOf learning Is NeuralNetFetchable Then
						CType(learning, NeuralNetFetchable).getNeuralNet().save(bos2)
					End If
					bos2.Flush()
					bos2.Close()

					Dim inputStream2 As Stream = New MemoryStream(bos2.toByteArray())
					writeEntry(inputStream2, zipfile)
				End If


				zipfile.flush()
				zipfile.close()

			End Using
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <C> org.nd4j.common.primitives.Pair<org.deeplearning4j.rl4j.network.dqn.IDQN, C> load(java.io.File file, @Class<C> cClass) throws java.io.IOException
		Public Shared Function load(Of C)(ByVal file As File, ByVal cClass As Type(Of C)) As Pair(Of IDQN, C)
			log.info("Deserializing: " & file.getName())

			Dim conf As C = Nothing
			Dim dqn As IDQN = Nothing
			Using zipFile As New java.util.zip.ZipFile(file)
				Dim config As ZipEntry = zipFile.getEntry("configuration.json")
				Dim stream As Stream = zipFile.getInputStream(config)
				Dim reader As New StreamReader(stream)
				Dim line As String = ""
				Dim js As New StringBuilder()
				line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
				Do While line IsNot Nothing
					js.Append(line).Append(vbLf)
						line = reader.ReadLine()
				Loop
				Dim json As String = js.ToString()

				reader.Close()
				stream.Close()

				conf = (New ObjectMapper()).readValue(json, cClass)

				Dim dqnzip As ZipEntry = zipFile.getEntry("dqn.bin")
				Dim dqnstream As Stream = zipFile.getInputStream(dqnzip)
				Dim tmpFile As File = File.createTempFile("restore", "dqn")
				Files.copy(dqnstream, Paths.get(tmpFile.getAbsolutePath()), StandardCopyOption.REPLACE_EXISTING)
				dqn = New DQN(ModelSerializer.restoreMultiLayerNetwork(tmpFile))
				dqnstream.Close()
			End Using

			Return New Pair(Of IDQN, C)(dqn, conf)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <C> org.nd4j.common.primitives.Pair<org.deeplearning4j.rl4j.network.dqn.IDQN, C> load(String path, @Class<C> cClass) throws java.io.IOException
		Public Shared Function load(Of C)(ByVal path As String, ByVal cClass As Type(Of C)) As Pair(Of IDQN, C)
			Return load(New File(path), cClass)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <C> org.nd4j.common.primitives.Pair<org.deeplearning4j.rl4j.network.dqn.IDQN, C> load(java.io.InputStream is, @Class<C> cClass) throws java.io.IOException
		Public Shared Function load(Of C)(ByVal [is] As Stream, ByVal cClass As Type(Of C)) As Pair(Of IDQN, C)
			Dim tmpFile As File = File.createTempFile("restore", "learning")
			Files.copy([is], Paths.get(tmpFile.getAbsolutePath()), StandardCopyOption.REPLACE_EXISTING)
			Return load(tmpFile, cClass)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void create(String dataRoot, boolean saveData) throws java.io.IOException
		Private Sub create(ByVal dataRoot As String, ByVal saveData As Boolean)
			Me.saveData = saveData
			Me.dataRoot = dataRoot
			createSubdir()
		End Sub

		'FIXME race condition if you create them at the same time where checking if dir exists is not atomic with the creation
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public String createSubdir() throws java.io.IOException
		Public Overridable Function createSubdir() As String

			If Not saveData Then
				Return ""
			End If

			Dim dr As New File(dataRoot)
			dr.mkdirs()
			Dim rootChildren() As File = dr.listFiles()

			Dim i As Integer = 1
			Do While childrenExist(rootChildren, i & "")
				i += 1
			Loop

			Dim f As New File(dataRoot & "/" & i)
			f.mkdirs()

			currentDir = f.getAbsolutePath()
			log.info("Created training data directory: " & currentDir)

			Dim mov As New File(VideoDir)
			mov.mkdirs()

			Dim model As New File(ModelDir)
			model.mkdirs()

			Dim stat As New File(Me.Stat)
			Dim info As New File(Me.Info)
			stat.createNewFile()
			info.createNewFile()
			Return f.getAbsolutePath()
		End Function

		Public Overridable ReadOnly Property VideoDir As String Implements IDataManager.getVideoDir
			Get
				Return currentDir & "/" & Constants.VIDEO_DIR
			End Get
		End Property

		Public Overridable ReadOnly Property ModelDir As String
			Get
				Return currentDir & "/" & Constants.MODEL_DIR
			End Get
		End Property

		Public Overridable ReadOnly Property Info As String
			Get
				Return currentDir & "/" & Constants.INFO_FILENAME
			End Get
		End Property

		Public Overridable ReadOnly Property Stat As String
			Get
				Return currentDir & "/" & Constants.STATISTIC_FILENAME
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void appendStat(StatEntry statEntry) throws java.io.IOException
		Public Overridable Sub appendStat(ByVal statEntry As StatEntry) Implements IDataManager.appendStat

			If Not saveData Then
				Return
			End If

			Dim statPath As Path = Paths.get(Stat)
			Dim toAppend As String = toJson(statEntry)
			Files.write(statPath, toAppend.GetBytes(), StandardOpenOption.APPEND)

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private String toJson(Object object) throws java.io.IOException
		Private Function toJson(ByVal [object] As Object) As String
			Return mapper.writeValueAsString([object]) & vbLf
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void writeInfo(org.deeplearning4j.rl4j.learning.ILearning iLearning) throws java.io.IOException
		Public Overridable Sub writeInfo(ByVal iLearning As ILearning) Implements IDataManager.writeInfo

			If Not saveData Then
				Return
			End If

			Dim infoPath As Path = Paths.get(Info)

			Dim info As New Info(iLearning.GetType().Name, iLearning.getMdp().GetType().Name, iLearning.getConfiguration(), iLearning.getStepCount(), DateTimeHelper.CurrentUnixTimeMillis())
			Dim toWrite As String = toJson(info)

			Files.write(infoPath, toWrite.GetBytes(), StandardOpenOption.TRUNCATE_EXISTING)
		End Sub

		Private Function childrenExist(ByVal files() As File, ByVal children As String) As Boolean
			Dim exists As Boolean = False
			For i As Integer = 0 To files.Length - 1
				If files(i).getName().Equals(children) Then
					exists = True
					Exit For
				End If
			Next i
			Return exists
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void save(org.deeplearning4j.rl4j.learning.ILearning learning) throws java.io.IOException
		Public Overridable Sub save(ByVal learning As ILearning) Implements IDataManager.save

			If Not saveData Then
				Return
			End If

			save(ModelDir & "/" & learning.getStepCount() & ".training", learning)
			If TypeOf learning Is NeuralNetFetchable Then
				Try
					CType(learning, NeuralNetFetchable).getNeuralNet().save(ModelDir & "/" & learning.getStepCount() & ".model")
				Catch e As System.NotSupportedException
					Dim path As String = ModelDir & "/" & learning.getStepCount()
					CType(CType(learning, NeuralNetFetchable).getNeuralNet(), IActorCritic).save(path & "_value.model", path & "_policy.model")
				End Try
			End If

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Value @Builder public static class Info
		Public Class Info
			Friend trainingName As String
			Friend mdpName As String
			Friend conf As ILearningConfiguration
			Friend stepCounter As Integer
			Friend millisTime As Long
		End Class
	End Class

End Namespace