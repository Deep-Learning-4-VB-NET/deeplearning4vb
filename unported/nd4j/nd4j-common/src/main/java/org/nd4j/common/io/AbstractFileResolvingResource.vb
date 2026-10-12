Imports System
Imports System.IO

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



	Public MustInherit Class AbstractFileResolvingResource
		Inherits AbstractResource

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.io.File getFile() throws java.io.IOException
		Public Overrides ReadOnly Property File As File
			Get
				Dim url As URL = Me.URL
				Return If(url.getProtocol().StartsWith("vfs"), AbstractFileResolvingResource.VfsResourceDelegate.getResource(url).File, ResourceUtils.getFile(url, Me.getDescription()))
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override protected java.io.File getFileForLastModifiedCheck() throws java.io.IOException
		Protected Friend Overrides ReadOnly Property FileForLastModifiedCheck As File
			Get
				Dim url As URL = Me.URL
				If ResourceUtils.isJarURL(url) Then
					Dim actualUrl As URL = ResourceUtils.extractJarFileURL(url)
					Return If(actualUrl.getProtocol().StartsWith("vfs"), AbstractFileResolvingResource.VfsResourceDelegate.getResource(actualUrl).File, ResourceUtils.getFile(actualUrl, "Jar URL"))
				Else
					Return Me.File
				End If
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected java.io.File getFile(java.net.URI uri) throws java.io.IOException
		Protected Friend Overridable Overloads Function getFile(ByVal uri As URI) As File
			Return If(uri.getScheme().StartsWith("vfs"), AbstractFileResolvingResource.VfsResourceDelegate.getResource(uri).File, ResourceUtils.getFile(uri, Me.getDescription()))
		End Function

		Public Overrides Function exists() As Boolean
			Try
				Dim ex As URL = Me.URL
				If ResourceUtils.isFileURL(ex) Then
					Return Me.File.exists()
				Else
					Dim con As URLConnection = ex.openConnection()
					ResourceUtils.useCachesIfNecessary(con)
					Dim httpCon As HttpURLConnection = If(TypeOf con Is HttpURLConnection, CType(con, HttpURLConnection), Nothing)
					If httpCon IsNot Nothing Then
						httpCon.setRequestMethod("HEAD")
						Dim [is] As Integer = httpCon.getResponseCode()
						If [is] = 200 Then
							Return True
						End If

						If [is] = 404 Then
							Return False
						End If
					End If

					If con.getContentLength() >= 0 Then
						Return True
					ElseIf httpCon IsNot Nothing Then
						httpCon.disconnect()
						Return False
					Else
						Dim is1 As Stream = Me.getInputStream()
						is1.Close()
						Return True
					End If
				End If
			Catch var5 As IOException
				Return False
			End Try
		End Function

		Public Overrides ReadOnly Property Readable As Boolean
			Get
				Try
					Dim ex As URL = Me.URL
					If Not ResourceUtils.isFileURL(ex) Then
						Return True
					Else
						Dim file As File = Me.File
						Return file.canRead() AndAlso Not file.isDirectory()
					End If
				Catch var3 As IOException
					Return False
				End Try
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public long contentLength() throws java.io.IOException
		Public Overrides Function contentLength() As Long
			Dim url As URL = Me.URL
			If ResourceUtils.isFileURL(url) Then
				Return Me.File.length()
			Else
				Dim con As URLConnection = url.openConnection()
				ResourceUtils.useCachesIfNecessary(con)
				If TypeOf con Is HttpURLConnection Then
					CType(con, HttpURLConnection).setRequestMethod("HEAD")
				End If

				Return CLng(Math.Truncate(con.getContentLength()))
			End If
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public long lastModified() throws java.io.IOException
		Public Overrides Function lastModified() As Long
			Dim url As URL = Me.URL
			If Not ResourceUtils.isFileURL(url) AndAlso Not ResourceUtils.isJarURL(url) Then
				Dim con As URLConnection = url.openConnection()
				ResourceUtils.useCachesIfNecessary(con)
				If TypeOf con Is HttpURLConnection Then
					CType(con, HttpURLConnection).setRequestMethod("HEAD")
				End If

				Return con.getLastModified()
			Else
				Return MyBase.lastModified()
			End If
		End Function

		Private Class VfsResourceDelegate
			Friend Sub New()
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static Resource getResource(java.net.URL url) throws java.io.IOException
			Public Shared Function getResource(ByVal url As URL) As Resource
				Return New VfsResource(VfsUtils.getRoot(url))
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static Resource getResource(java.net.URI uri) throws java.io.IOException
			Public Shared Function getResource(ByVal uri As URI) As Resource
				Return New VfsResource(VfsUtils.getRoot(uri))
			End Function
		End Class
	End Class

End Namespace