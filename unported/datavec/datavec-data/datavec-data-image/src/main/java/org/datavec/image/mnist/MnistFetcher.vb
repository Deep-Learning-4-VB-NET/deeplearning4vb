Imports System.IO
Imports Microsoft.VisualBasic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports ArchiveUtils = org.nd4j.common.util.ArchiveUtils
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.datavec.image.mnist


	Public Class MnistFetcher

		Private fileDir As File
		Private Shared log As Logger = LoggerFactory.getLogger(GetType(MnistFetcher))
		Private Const trainingFilesURL As String = "http://yann.lecun.com/exdb/mnist/train-images-idx3-ubyte.gz"

		Private Const trainingFilesFilename As String = "images-idx1-ubyte.gz"
		Public Const trainingFilesFilename_unzipped As String = "images-idx1-ubyte"

		Private Const trainingFileLabelsURL As String = "http://yann.lecun.com/exdb/mnist/train-labels-idx1-ubyte.gz"
		Private Const trainingFileLabelsFilename As String = "labels-idx1-ubyte.gz"
		Public Const trainingFileLabelsFilename_unzipped As String = "labels-idx1-ubyte"
		Private Const LOCAL_DIR_NAME As String = "MNIST"



'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public java.io.File downloadAndUntar() throws java.io.IOException
		Public Overridable Function downloadAndUntar() As File
			If fileDir IsNot Nothing Then
				Return fileDir
			End If
			' mac gives unique tmp each run and we want to store this persist
			' this data across restarts
			Dim tmpDir As New File(System.getProperty("user.home"))

			Dim baseDir As New File(tmpDir, LOCAL_DIR_NAME)
			If Not (baseDir.isDirectory() OrElse baseDir.mkdir()) Then
				Throw New IOException("Could not mkdir " & baseDir)
			End If


			log.info("Downloading mnist...")
			' getFromOrigin training records
			Dim tarFile As New File(baseDir, trainingFilesFilename)

			If Not tarFile.isFile() Then
				FileUtils.copyURLToFile(New URL(trainingFilesURL), tarFile)
			End If

			ArchiveUtils.unzipFileTo(tarFile.getAbsolutePath(), baseDir.getAbsolutePath())



			' getFromOrigin training records
			Dim labels As New File(baseDir, trainingFileLabelsFilename)

			If Not labels.isFile() Then
				FileUtils.copyURLToFile(New URL(trainingFileLabelsURL), labels)
			End If

			ArchiveUtils.unzipFileTo(labels.getAbsolutePath(), baseDir.getAbsolutePath())



			fileDir = baseDir
			Return fileDir
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void untarFile(java.io.File baseDir, java.io.File tarFile) throws java.io.IOException
		Public Overridable Sub untarFile(ByVal baseDir As File, ByVal tarFile As File)

			log.info("Untaring File: " & tarFile.ToString())

			Dim p As Process = Runtime.getRuntime().exec(String.Format("tar -C {0} -xvf {1}", baseDir.getAbsolutePath(), tarFile.getAbsolutePath()))
			Dim stdError As New StreamReader(p.getErrorStream())
			log.info("Here is the standard error of the command (if any):" & vbLf)
			Dim s As String
			s = stdError.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((s = stdError.readLine()) != null)
			Do While s IsNot Nothing
				log.info(s)
					s = stdError.ReadLine()
			Loop
			stdError.Close()


		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void gunzipFile(java.io.File baseDir, java.io.File gzFile) throws java.io.IOException
		Public Shared Sub gunzipFile(ByVal baseDir As File, ByVal gzFile As File)

			log.info("gunzip'ing File: " & gzFile.ToString())

			Dim p As Process = Runtime.getRuntime().exec(String.Format("gunzip {0}", gzFile.getAbsolutePath()))
			Dim stdError As New StreamReader(p.getErrorStream())
			log.info("Here is the standard error of the command (if any):" & vbLf)
			Dim s As String
			s = stdError.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((s = stdError.readLine()) != null)
			Do While s IsNot Nothing
				log.info(s)
					s = stdError.ReadLine()
			Loop
			stdError.Close()


		End Sub


	End Class

End Namespace