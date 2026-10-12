Imports System
Imports IOUtils = org.apache.commons.io.IOUtils
Imports Loader = org.bytedeco.javacpp.Loader

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


Namespace org.nd4j.python4j


	Public Class PythonProcess
		Private Shared pythonExecutable As String = Loader.load(GetType(org.bytedeco.cpython.python))
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static String runAndReturn(String... arguments)throws IOException, InterruptedException
		Public Shared Function runAndReturn(ParamArray ByVal arguments() As String) As String
			Dim allArgs(arguments.Length) As String
			For i As Integer = 0 To arguments.Length - 1
				allArgs(i + 1) = arguments(i)
			Next i
			allArgs(0) = pythonExecutable
			Dim pb As New ProcessBuilder(allArgs)
			Dim process As Process = pb.start()
			Dim [out] As String = IOUtils.toString(process.getInputStream(), StandardCharsets.UTF_8)
			process.waitFor()
			Return [out]

		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void run(String... arguments)throws IOException, InterruptedException
		Public Shared Sub run(ParamArray ByVal arguments() As String)
			Dim allArgs(arguments.Length) As String
			For i As Integer = 0 To arguments.Length - 1
				allArgs(i + 1) = arguments(i)
			Next i
			allArgs(0) = pythonExecutable
			Dim pb As New ProcessBuilder(allArgs)
			pb.inheritIO().start().waitFor()
		End Sub
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void pipInstall(String packageName) throws PythonException
		Public Shared Sub pipInstall(ByVal packageName As String)
			Try
				run("-m", "pip", "install", packageName)
			Catch e As Exception
				Throw New PythonException("Error installing package " & packageName, e)
			End Try

		End Sub

		Public Shared Sub pipInstall(ByVal packageName As String, ByVal version As String)
			pipInstall(packageName & "==" & version)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void pipUninstall(String packageName) throws PythonException
		Public Shared Sub pipUninstall(ByVal packageName As String)
			Try
				run("-m", "pip", "uninstall", packageName)
			Catch e As Exception
				Throw New PythonException("Error uninstalling package " & packageName, e)
			End Try

		End Sub
		Public Shared Sub pipInstallFromGit(ByVal gitRepoUrl As String)
			If Not gitRepoUrl.Contains("://") Then
				gitRepoUrl = "git://" & gitRepoUrl
			End If
			Try
				run("-m", "pip", "install", "git+", gitRepoUrl)
			Catch e As Exception
				Throw New PythonException("Error installing package from " & gitRepoUrl, e)
			End Try

		End Sub

		Public Shared Function getPackageVersion(ByVal packageName As String) As String
			Dim [out] As String
			Try
				[out] = runAndReturn("-m", "pip", "show", packageName)
			Catch e As Exception
				Throw New PythonException("Error finding version for package " & packageName, e)
			End Try

			If Not [out].Contains("Version: ") Then
				Throw New PythonException("Can't find package " & packageName)
			End If
			Dim pkgVersion As String = [out].Split("Version: ", True)(1).Split(Environment.NewLine)(0)
			Return pkgVersion
		End Function

		Public Shared Function isPackageInstalled(ByVal packageName As String) As Boolean
			Try
				Dim [out] As String = runAndReturn("-m", "pip", "show", packageName)
				Return [out].Length > 0
			Catch e As Exception
				Throw New PythonException("Error checking if package is installed: " & packageName, e)
			End Try

		End Function

		Public Shared Sub pipInstallFromRequirementsTxt(ByVal path As String)
			Try
				run("-m", "pip", "install","-r", path)
			Catch e As Exception
				Throw New PythonException("Error installing packages from " & path, e)
			End Try
		End Sub

		Public Shared Sub pipInstallFromSetupScript(ByVal path As String, ByVal inplace As Boolean)

			Try
				run(path,If(inplace, "develop", "install"))
			Catch e As Exception
				Throw New PythonException("Error installing package from " & path, e)
			End Try

		End Sub

	End Class
End Namespace