Imports System
Imports ND4JClassLoading = org.nd4j.common.config.ND4JClassLoading

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

Namespace org.nd4j.common.io



	Public MustInherit Class ResourceUtils
		Public Const CLASSPATH_URL_PREFIX As String = "classpath:"
		Public Const FILE_URL_PREFIX As String = "file:"
		Public Const URL_PROTOCOL_FILE As String = "file"
		Public Const URL_PROTOCOL_JAR As String = "jar"
		Public Const URL_PROTOCOL_ZIP As String = "zip"
		Public Const URL_PROTOCOL_VFSZIP As String = "vfszip"
		Public Const URL_PROTOCOL_VFS As String = "vfs"
		Public Const URL_PROTOCOL_WSJAR As String = "wsjar"
		Public Const URL_PROTOCOL_CODE_SOURCE As String = "code-source"
		Public Const JAR_URL_SEPARATOR As String = "!/"

		Public Sub New()
		End Sub

		Public Shared Function isUrl(ByVal resourceLocation As String) As Boolean
			If resourceLocation Is Nothing Then
				Return False
			ElseIf resourceLocation.StartsWith("classpath:", StringComparison.Ordinal) Then
				Return True
			Else
				Try
					Dim tempVar As New URL(resourceLocation)
					Return True
				Catch var2 As MalformedURLException
					Return False
				End Try
			End If
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static URL getURL(String resourceLocation) throws java.io.FileNotFoundException
		Public Shared Function getURL(ByVal resourceLocation As String) As URL
			Assert.notNull(resourceLocation, "Resource location must not be null")
			If resourceLocation.StartsWith("classpath:", StringComparison.Ordinal) Then
				Dim ex As String = resourceLocation.Substring("classpath:".Length)
				Dim ex2 As URL = ND4JClassLoading.Nd4jClassloader.getResource(ex)
				If ex2 Is Nothing Then
					Dim description As String = "class path resource [" & ex & "]"
					Throw New FileNotFoundException(description & " cannot be resolved to URL because it does not exist")
				Else
					Return ex2
				End If
			Else
				Try
					Return New URL(resourceLocation)
				Catch var5 As MalformedURLException
					Try
						Return (New File(resourceLocation)).toURI().toURL()
					Catch var4 As MalformedURLException
						Throw New FileNotFoundException("Resource location [" & resourceLocation & "] is neither a URL not a well-formed file path")
					End Try
				End Try
			End If
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static java.io.File getFile(String resourceLocation) throws java.io.FileNotFoundException
		Public Shared Function getFile(ByVal resourceLocation As String) As File
			Assert.notNull(resourceLocation, "Resource location must not be null")
			If resourceLocation.StartsWith("classpath:", StringComparison.Ordinal) Then
				Dim ex As String = resourceLocation.Substring("classpath:".Length)
				Dim description As String = "class path resource [" & ex & "]"
				Dim url As URL = ND4JClassLoading.Nd4jClassloader.getResource(ex)
				If url Is Nothing Then
					Throw New FileNotFoundException(description & " cannot be resolved to absolute file path " & "because it does not reside in the file system")
				Else
					Return getFile(url, description)
				End If
			Else
				Try
					Return getFile(New URL(resourceLocation))
				Catch var4 As MalformedURLException
					Return New File(resourceLocation)
				End Try
			End If
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static java.io.File getFile(URL resourceUrl) throws java.io.FileNotFoundException
		Public Shared Function getFile(ByVal resourceUrl As URL) As File
			Return getFile(resourceUrl, "URL")
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static java.io.File getFile(URL resourceUrl, String description) throws java.io.FileNotFoundException
		Public Shared Function getFile(ByVal resourceUrl As URL, ByVal description As String) As File
			Assert.notNull(resourceUrl, "Resource URL must not be null")
			If Not "file".Equals(resourceUrl.getProtocol()) Then
				Throw New FileNotFoundException(description & " cannot be resolved to absolute file path " & "because it does not reside in the file system: " & resourceUrl)
			Else
				Try
					Return New File(toURI(resourceUrl).getSchemeSpecificPart())
				Catch var3 As URISyntaxException
					Return New File(resourceUrl.getFile())
				End Try
			End If
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static java.io.File getFile(URI resourceUri) throws java.io.FileNotFoundException
		Public Shared Function getFile(ByVal resourceUri As URI) As File
			Return getFile(resourceUri, "URI")
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static java.io.File getFile(URI resourceUri, String description) throws java.io.FileNotFoundException
		Public Shared Function getFile(ByVal resourceUri As URI, ByVal description As String) As File
			Assert.notNull(resourceUri, "Resource URI must not be null")
			If Not "file".Equals(resourceUri.getScheme()) Then
				Throw New FileNotFoundException(description & " cannot be resolved to absolute file path " & "because it does not reside in the file system: " & resourceUri)
			Else
				Return New File(resourceUri.getSchemeSpecificPart())
			End If
		End Function

		Public Shared Function isFileURL(ByVal url As URL) As Boolean
			Dim protocol As String = url.getProtocol()
			Return "file".Equals(protocol) OrElse protocol.StartsWith("vfs", StringComparison.Ordinal)
		End Function

		Public Shared Function isJarURL(ByVal url As URL) As Boolean
			Dim protocol As String = url.getProtocol()
			Return "jar".Equals(protocol) OrElse "zip".Equals(protocol) OrElse "wsjar".Equals(protocol) OrElse "code-source".Equals(protocol) AndAlso url.getPath().contains("!/")
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static URL extractJarFileURL(URL jarUrl) throws MalformedURLException
		Public Shared Function extractJarFileURL(ByVal jarUrl As URL) As URL
			Dim urlFile As String = jarUrl.getFile()
			Dim separatorIndex As Integer = urlFile.IndexOf("!/", StringComparison.Ordinal)
			If separatorIndex <> -1 Then
				Dim jarFile As String = urlFile.Substring(0, separatorIndex)

				Try
					Return New URL(jarFile)
				Catch var5 As MalformedURLException
					If Not jarFile.StartsWith("/", StringComparison.Ordinal) Then
						jarFile = "/" & jarFile
					End If

					Return New URL("file:" & jarFile)
				End Try
			Else
				Return jarUrl
			End If
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static URI toURI(URL url) throws URISyntaxException
		Public Shared Function toURI(ByVal url As URL) As URI
			Return toURI(url.ToString())
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static URI toURI(String location) throws URISyntaxException
		Public Shared Function toURI(ByVal location As String) As URI
			Return New URI(StringUtils.replace(location, " ", "%20"))
		End Function

		Public Shared Sub useCachesIfNecessary(ByVal con As URLConnection)
			con.setUseCaches(con.GetType().Name.StartsWith("JNLP", StringComparison.Ordinal))
		End Sub

		Public Shared Function classPackageAsResourcePath(ByVal clazz As Type) As String
			Objects.requireNonNull(clazz)

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Dim className As String = clazz.FullName
			Dim packageEndIndex As Integer = className.LastIndexOf(46)
			If packageEndIndex = -1 Then
				Return ""
			Else
				Dim packageName As String = className.Substring(0, packageEndIndex)
				Return packageName.Replace("."c, "/"c)
			End If
		End Function
	End Class

End Namespace