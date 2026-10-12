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


	Public Class VfsResource
		Inherits AbstractResource

		Private ReadOnly resource As Object

		Public Sub New(ByVal resources As Object)
			Assert.notNull(resources, "VirtualFile must not be null")
			Me.resource = resources
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public java.io.InputStream getInputStream() throws java.io.IOException
		Public Overrides ReadOnly Property InputStream As Stream
			Get
				Return VfsUtils.getInputStream(Me.resource)
			End Get
		End Property

		Public Overrides Function exists() As Boolean
			Return VfsUtils.exists(Me.resource)
		End Function

		Public Overrides ReadOnly Property Readable As Boolean
			Get
				Return VfsUtils.isReadable(Me.resource)
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public java.net.URL getURL() throws java.io.IOException
		Public Overrides ReadOnly Property URL As URL
			Get
				Try
					Return VfsUtils.getURL(Me.resource)
				Catch var2 As Exception
					Throw New IOException("Failed to obtain URL for file " & Me.resource, var2)
				End Try
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public java.net.URI getURI() throws java.io.IOException
		Public Overrides ReadOnly Property URI As URI
			Get
				Try
					Return VfsUtils.getURI(Me.resource)
				Catch var2 As Exception
					Throw New IOException("Failed to obtain URI for " & Me.resource, var2)
				End Try
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public java.io.File getFile() throws java.io.IOException
		Public Overrides ReadOnly Property File As File
			Get
				Return VfsUtils.getFile(Me.resource)
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public long contentLength() throws java.io.IOException
		Public Overrides Function contentLength() As Long
			Return VfsUtils.getSize(Me.resource)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public long lastModified() throws java.io.IOException
		Public Overrides Function lastModified() As Long
			Return VfsUtils.getLastModified(Me.resource)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public Resource createRelative(String relativePath) throws java.io.IOException
		Public Overrides Function createRelative(ByVal relativePath As String) As Resource
			If Not relativePath.StartsWith(".", StringComparison.Ordinal) AndAlso relativePath.Contains("/") Then
				Try
					Return New VfsResource(VfsUtils.getChild(Me.resource, relativePath))
				Catch var3 As IOException

				End Try
			End If

			Return New VfsResource(VfsUtils.getRelative(New URL(Me.URL, relativePath)))
		End Function

		Public Overrides ReadOnly Property Filename As String
			Get
				Return VfsUtils.getName(Me.resource)
			End Get
		End Property

		Public Overrides ReadOnly Property Description As String
			Get
				Return Me.resource.ToString()
			End Get
		End Property

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return obj Is Me OrElse TypeOf obj Is VfsResource AndAlso Me.resource.Equals(DirectCast(obj, VfsResource).resource)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Me.resource.GetHashCode()
		End Function
	End Class

End Namespace