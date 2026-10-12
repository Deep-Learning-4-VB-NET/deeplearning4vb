Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports FileUtils = org.apache.commons.io.FileUtils
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
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


	Public Class ClassPathResource
		Inherits AbstractFileResolvingResource

'JAVA TO VB CONVERTER NOTE: The field path was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly path_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field classLoader was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private classLoader_Conflict As ClassLoader
		Private clazz As Type

		Public Sub New(ByVal path As String)
			Me.New(path, DirectCast(Nothing, ClassLoader))
		End Sub

		Public Sub New(ByVal path As String, ByVal classLoader As ClassLoader)
			Assert.notNull(path, "Path must not be null")
			Dim pathToUse As String = StringUtils.cleanPath(path)
			If pathToUse.StartsWith("/", StringComparison.Ordinal) Then
				pathToUse = pathToUse.Substring(1)
			End If

			Me.path_Conflict = pathToUse
			Me.classLoader_Conflict = If(classLoader IsNot Nothing, classLoader, ND4JClassLoading.Nd4jClassloader)
		End Sub

		Public Sub New(ByVal path As String, ByVal clazz As Type)
			Assert.notNull(path, "Path must not be null")
			Me.path_Conflict = StringUtils.cleanPath(path)
			Me.clazz = clazz
		End Sub

		Protected Friend Sub New(ByVal path As String, ByVal classLoader As ClassLoader, ByVal clazz As Type)
			Me.path_Conflict = StringUtils.cleanPath(path)
			Me.classLoader_Conflict = classLoader
			Me.clazz = clazz
		End Sub

		Public ReadOnly Property Path As String
			Get
				Return Me.path_Conflict
			End Get
		End Property

		Public ReadOnly Property ClassLoader As ClassLoader
			Get
				Return If(Me.classLoader_Conflict IsNot Nothing, Me.classLoader_Conflict, Me.clazz.getClassLoader())
			End Get
		End Property

		''' <summary>
		''' Get the File.
		''' If the file cannot be accessed directly (for example, it is in a JAR file), we will attempt to extract it from
		''' the JAR and copy it to the temporary directory, using <seealso cref="getTempFileFromArchive()"/>
		''' </summary>
		''' <returns> The File, or a temporary copy if it can not be accessed directly </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public File getFile() throws IOException
		Public Overrides ReadOnly Property File As File
			Get
				Try
					Return MyBase.File
				Catch e As FileNotFoundException
					'java.io.FileNotFoundException: class path resource [iris.txt] cannot be resolved to absolute file path because
					' it does not reside in the file system: jar:file:/.../dl4j-test-resources-0.9.2-SNAPSHOT.jar!/iris.txt
					Return TempFileFromArchive
				End Try
			End Get
		End Property


		''' <summary>
		''' Get a temp file from the classpath.<br>
		''' This is for resources where a file is needed and the classpath resource is in a jar file. The file is copied
		''' to the default temporary directory, using <seealso cref="Files.createTempFile(String, String, FileAttribute[])"/>.
		''' Consequently, the extracted file will have a different filename to the extracted one.
		''' </summary>
		''' <returns> the temp file </returns>
		''' <exception cref="IOException"> If an error occurs when files are being copied </exception>
		''' <seealso cref= #getTempFileFromArchive(File) </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public File getTempFileFromArchive() throws IOException
		Public Overridable ReadOnly Property TempFileFromArchive As File
			Get
				Return getTempFileFromArchive(Nothing)
			End Get
		End Property

		''' <summary>
		''' Get a temp file from the classpath, and (optionally) place it in the specified directory<br>
		''' Note that:<br>
		''' - If the directory is not specified, the file is copied to the default temporary directory, using
		''' <seealso cref="Files.createTempFile(String, String, FileAttribute[])"/>. Consequently, the extracted file will have a
		''' different filename to the extracted one.<br>
		''' - If the directory *is* specified, the file is copied directly - and the original filename is maintained
		''' </summary>
		''' <param name="rootDirectory"> May be null. If non-null, copy to the specified directory </param>
		''' <returns> the temp file </returns>
		''' <exception cref="IOException"> If an error occurs when files are being copied </exception>
		''' <seealso cref= #getTempFileFromArchive(File) </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public File getTempFileFromArchive(File rootDirectory) throws IOException
		Public Overridable Function getTempFileFromArchive(ByVal rootDirectory As File) As File
			Dim [is] As Stream = Stream
			Dim tmpFile As File
			If rootDirectory IsNot Nothing Then
				'Maintain original file names, as it's going in a directory...
				tmpFile = New File(rootDirectory, FilenameUtils.getName(path_Conflict))
			Else
				tmpFile = Files.createTempFile(FilenameUtils.getName(path_Conflict), "tmp").toFile()
			End If

			tmpFile.deleteOnExit()

			Dim bos As New BufferedOutputStream(New FileStream(tmpFile, FileMode.Create, FileAccess.Write))

			IOUtils.copy([is], bos)
			bos.flush()
			bos.close()
			Return tmpFile
		End Function

		''' <summary>
		''' Extract the directory recursively to the specified location. Current ClassPathResource must point to
		''' a directory.<br>
		''' For example, if classpathresource points to "some/dir/", then the contents - not including the parent directory "dir" -
		''' will be extracted or copied to the specified destination.<br> </summary>
		''' <param name="destination"> Destination directory. Must exist </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void copyDirectory(File destination) throws IOException
		Public Overridable Sub copyDirectory(ByVal destination As File)
			Preconditions.checkState(destination.exists() AndAlso destination.isDirectory(), "Destination directory must exist and be a directory: %s", destination)


			Dim url As URL = Me.Url

			If isJarURL(url) Then
	'            
	'                This is actually request for file, that's packed into jar. Probably the current one, but that doesn't matters.
	'             
				Dim stream As Stream = Nothing
				Dim zipFile As ZipFile = Nothing
				Try
					Dim getStreamFromZip As GetStreamFromZip = (New GetStreamFromZip(Me, url, path_Conflict)).invoke()
					Dim entry As ZipEntry = getStreamFromZip.Entry
					stream = getStreamFromZip.Stream
					zipFile = getStreamFromZip.ZipFile

					Preconditions.checkState(entry.isDirectory(), "Source must be a directory: %s", entry.getName())

					Dim pathNoSlash As String = Me.path_Conflict
					If pathNoSlash.EndsWith("/", StringComparison.Ordinal) OrElse pathNoSlash.EndsWith("\", StringComparison.Ordinal) Then
						pathNoSlash = pathNoSlash.Substring(0, pathNoSlash.Length-1)
					End If

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.Iterator<? extends java.util.zip.ZipEntry> entries = zipFile.entries();
					Dim entries As IEnumerator(Of ZipEntry) = zipFile.entries()
					Do While entries.MoveNext()
						Dim e As ZipEntry = entries.Current
						Dim name As String = e.getName()
						If name.StartsWith(pathNoSlash, StringComparison.Ordinal) AndAlso name.Length > pathNoSlash.Length AndAlso (name.Chars(pathNoSlash.Length) = "/"c OrElse name.Chars(pathNoSlash.Length) = "\"c) Then 'second condition: to avoid "/dir/a/" and "/dir/abc/" both matching startsWith

							Dim relativePath As String = name.Substring(Me.path_Conflict.Length)

							Dim extractTo As New File(destination, relativePath)
							If e.isDirectory() Then
								extractTo.mkdirs()
							Else
								Using bos As New BufferedOutputStream(New FileStream(extractTo, FileMode.Create, FileAccess.Write))
									Dim [is] As Stream = getInputStream(name, clazz, classLoader_Conflict)
									IOUtils.copy([is], bos)
								End Using
							End If
						End If
					Loop

					stream.Close()
					zipFile.close()
				Catch e As Exception
					Throw New Exception(e)
				Finally
					If stream IsNot Nothing Then
						IOUtils.closeQuietly(stream)
					End If
					If zipFile IsNot Nothing Then
						IOUtils.closeQuietly(zipFile)
					End If
				End Try

			Else
				Dim source As File
				Try
					source = New File(url.toURI())
				Catch e As URISyntaxException
					Throw New IOException("Error converting URL to a URI - path may be invalid? Path=" & url)
				End Try
				Preconditions.checkState(source.isDirectory(), "Source must be a directory: %s", source)
				Preconditions.checkState(destination.exists() AndAlso destination.isDirectory(), "Destination must be a directory and must exist: %s", destination)
				FileUtils.copyDirectory(source, destination)
			End If
		End Sub

		Public Overrides Function exists() As Boolean
			Dim url As URL
			If Me.clazz IsNot Nothing Then
				url = Me.clazz.getResource(Me.path_Conflict)
			Else
				url = Me.classLoader_Conflict.getResource(Me.path_Conflict)
			End If

			Return url IsNot Nothing
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public InputStream getInputStream() throws IOException
		Public Overridable ReadOnly Property InputStream As Stream
			Get
				Return getInputStream(path_Conflict, clazz, classLoader_Conflict)
			End Get
		End Property


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static InputStream getInputStream(String path, @Class clazz, ClassLoader classLoader) throws IOException
		Private Shared Function getInputStream(ByVal path As String, ByVal clazz As Type, ByVal classLoader As ClassLoader) As Stream
			Dim [is] As Stream
			If clazz IsNot Nothing Then
				[is] = clazz.getResourceAsStream(path)
			Else
				[is] = classLoader.getResourceAsStream(path)
			End If

			If [is] Is Nothing Then
				Throw New FileNotFoundException(path & " cannot be opened because it does not exist")
			Else
				If TypeOf [is] Is BufferedInputStream Then
					Return [is]
				End If
				Return New BufferedInputStream([is])
			End If
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public java.net.URL getURL() throws IOException
		Public Overrides ReadOnly Property URL As URL
			Get
				Dim url As URL
				If Me.clazz IsNot Nothing Then
					url = Me.clazz.getResource(Me.path_Conflict)
				Else
					url = Me.classLoader_Conflict.getResource(Me.path_Conflict)
				End If
    
				If url Is Nothing Then
					Throw New FileNotFoundException(Me.Description & " cannot be resolved to URL because it does not exist")
				Else
					Return url
				End If
			End Get
		End Property

		Public Overrides Function createRelative(ByVal relativePath As String) As Resource
			Dim pathToUse As String = StringUtils.applyRelativePath(Me.path_Conflict, relativePath)
			Return New ClassPathResource(pathToUse, Me.classLoader_Conflict, Me.clazz)
		End Function

		Public Overrides ReadOnly Property Filename As String
			Get
				Return StringUtils.getFilename(Me.path_Conflict)
			End Get
		End Property

		Public Overridable ReadOnly Property Description As String
			Get
				Dim builder As New StringBuilder("class path resource [")
				Dim pathToUse As String = Me.path_Conflict
				If Me.clazz IsNot Nothing AndAlso Not pathToUse.StartsWith("/", StringComparison.Ordinal) Then
					builder.Append(ResourceUtils.classPackageAsResourcePath(Me.clazz))
					builder.Append("/"c)
				End If
    
				If pathToUse.StartsWith("/", StringComparison.Ordinal) Then
					pathToUse = pathToUse.Substring(1)
				End If
    
				builder.Append(pathToUse)
				builder.Append("]"c)
				Return builder.ToString()
			End Get
		End Property

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Me Then
				Return True
			ElseIf Not (TypeOf obj Is ClassPathResource) Then
				Return False
			Else
				Dim otherRes As ClassPathResource = DirectCast(obj, ClassPathResource)
				Return Me.path_Conflict.Equals(otherRes.path_Conflict) AndAlso ObjectUtils.nullSafeEquals(Me.classLoader_Conflict, otherRes.classLoader_Conflict) AndAlso ObjectUtils.nullSafeEquals(Me.clazz, otherRes.clazz)
			End If
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Me.path_Conflict.GetHashCode()
		End Function

		''' <summary>
		'''  Returns URL of the requested resource
		''' </summary>
		''' <returns> URL of the resource, if it's available in current Jar </returns>
		Private ReadOnly Property Url As URL
			Get
				Dim loader As ClassLoader = Nothing
				Try
					loader = ND4JClassLoading.Nd4jClassloader
				Catch e As Exception
					' do nothing
				End Try
    
				If loader Is Nothing Then
					loader = GetType(ClassPathResource).getClassLoader()
				End If
    
				Dim url As URL = loader.getResource(Me.path_Conflict)
				If url Is Nothing Then
					' try to check for mis-used starting slash
					' TODO: see TODO below
					If Me.path_Conflict.StartsWith("/", StringComparison.Ordinal) Then
						url = loader.getResource(Me.path_Conflict.replaceFirst("[\\/]", ""))
						If url IsNot Nothing Then
							Return url
						End If
					Else
						' try to add slash, to make clear it's not an issue
						' TODO: change this mechanic to actual path purifier
						url = loader.getResource("/" & Me.path_Conflict)
						If url IsNot Nothing Then
							Return url
						End If
					End If
					Throw New System.InvalidOperationException("Resource '" & Me.path_Conflict & "' cannot be found.")
				End If
				Return url
			End Get
		End Property

		''' <summary>
		''' Checks, if proposed URL is packed into archive.
		''' </summary>
		''' <param name="url"> URL to be checked </param>
		''' <returns> True, if URL is archive entry, False otherwise </returns>
		Private Shared Function isJarURL(ByVal url As URL) As Boolean
			Dim protocol As String = url.getProtocol()
			Return "jar".Equals(protocol) OrElse "zip".Equals(protocol) OrElse "wsjar".Equals(protocol) OrElse "code-source".Equals(protocol) AndAlso url.getPath().contains("!/")
		End Function

		Private Class GetStreamFromZip
			Private ReadOnly outerInstance As ClassPathResource

'JAVA TO VB CONVERTER NOTE: The field url was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend url_Conflict As URL
'JAVA TO VB CONVERTER NOTE: The field zipFile was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend zipFile_Conflict As ZipFile
'JAVA TO VB CONVERTER NOTE: The field entry was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend entry_Conflict As ZipEntry
'JAVA TO VB CONVERTER NOTE: The field stream was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend stream_Conflict As Stream
			Friend resourceName As String

			Public Sub New(ByVal outerInstance As ClassPathResource, ByVal url As URL, ByVal resourceName As String)
				Me.outerInstance = outerInstance
				Me.url_Conflict = url
				Me.resourceName = resourceName
			End Sub

			Public Overridable ReadOnly Property Url As URL
				Get
					Return url_Conflict
				End Get
			End Property

			Public Overridable ReadOnly Property ZipFile As ZipFile
				Get
					Return zipFile_Conflict
				End Get
			End Property

			Public Overridable ReadOnly Property Entry As ZipEntry
				Get
					Return entry_Conflict
				End Get
			End Property

			Public Overridable ReadOnly Property Stream As Stream
				Get
					Return stream_Conflict
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public GetStreamFromZip invoke() throws IOException
			Public Overridable Function invoke() As GetStreamFromZip
				url_Conflict = outerInstance.extractActualUrl(url_Conflict)

				zipFile_Conflict = New ZipFile(url_Conflict.getFile())
				entry_Conflict = zipFile_Conflict.getEntry(Me.resourceName)
				If entry_Conflict Is Nothing Then
					If Me.resourceName.StartsWith("/", StringComparison.Ordinal) Then
						entry_Conflict = zipFile_Conflict.getEntry(Me.resourceName.replaceFirst("/", ""))
						If entry_Conflict Is Nothing Then
							Throw New FileNotFoundException("Resource " & Me.resourceName & " not found")
						End If
					Else
						Throw New FileNotFoundException("Resource " & Me.resourceName & " not found")
					End If
				End If

				stream_Conflict = zipFile_Conflict.getInputStream(entry_Conflict)
				Return Me
			End Function
		End Class

		''' <summary>
		''' Extracts parent Jar URL from original ClassPath entry URL.
		''' </summary>
		''' <param name="jarUrl"> Original URL of the resource </param>
		''' <returns> URL of the Jar file, containing requested resource </returns>
		''' <exception cref="MalformedURLException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private java.net.URL extractActualUrl(java.net.URL jarUrl) throws java.net.MalformedURLException
		Private Function extractActualUrl(ByVal jarUrl As URL) As URL
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


	End Class

End Namespace