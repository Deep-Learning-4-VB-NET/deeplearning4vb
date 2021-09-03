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


	Public MustInherit Class AbstractResource
		Implements Resource

		Public MustOverride ReadOnly Property InputStream As Stream Implements org.nd4j.common.io.InputStreamSource.getInputStream
		Public MustOverride ReadOnly Property Description As String Implements Resource.getDescription
		Public Sub New()
		End Sub

		Public Overridable Function exists() As Boolean Implements Resource.exists
			Try
				Return Me.File.exists()
			Catch var4 As IOException
				Try
					Dim isEx As Stream = Me.InputStream
					isEx.Close()
					Return True
				Catch var3 As Exception
					Return False
				End Try
			End Try
		End Function

		Public Overridable ReadOnly Property Readable As Boolean Implements Resource.isReadable
			Get
				Return True
			End Get
		End Property

		Public Overridable ReadOnly Property Open As Boolean Implements Resource.isOpen
			Get
				Return False
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public java.net.URL getURL() throws java.io.IOException
		Public Overridable ReadOnly Property URL As URL Implements Resource.getURL
			Get
				Throw New FileNotFoundException(Me.Description & " cannot be resolved to URL")
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public java.net.URI getURI() throws java.io.IOException
		Public Overridable ReadOnly Property URI As URI Implements Resource.getURI
			Get
				Dim url As URL = Me.URL
    
				Try
					Return ResourceUtils.toURI(url)
				Catch var3 As URISyntaxException
					Throw New IOException("Invalid URI [" & url & "]", var3)
				End Try
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public java.io.File getFile() throws java.io.IOException
		Public Overridable ReadOnly Property File As File Implements Resource.getFile
			Get
				Throw New FileNotFoundException(Me.Description & " cannot be resolved to absolute file path")
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public long contentLength() throws java.io.IOException
		Public Overridable Function contentLength() As Long Implements Resource.contentLength
			Dim [is] As Stream = Me.InputStream
			Assert.state([is] IsNot Nothing, "resource input stream must not be null")

			Try
				Dim size As Long = 0L

				Dim read As Integer
				Dim buf(254) As SByte
				read = [is].Read(buf, 0, buf.Length)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: for (byte[] buf = new byte[255]; (read = is.read(buf)) != -1; size += (long) read)
				Do While read <> -1

						read = [is].Read(buf, 0, buf.Length)
					size += CLng(read)
				Loop

				Dim var6 As Long = size
				Return var6
			Finally
				Try
					[is].Close()
				Catch var14 As IOException

				End Try

			End Try
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public long lastModified() throws java.io.IOException
		Public Overridable Function lastModified() As Long Implements Resource.lastModified
'JAVA TO VB CONVERTER NOTE: The local variable lastModified was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim lastModified_Conflict As Long = Me.FileForLastModifiedCheck.lastModified()
			If lastModified_Conflict = 0L Then
				Throw New FileNotFoundException(Me.Description & " cannot be resolved in the file system for resolving its last-modified timestamp")
			Else
				Return lastModified_Conflict
			End If
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected java.io.File getFileForLastModifiedCheck() throws java.io.IOException
		Protected Friend Overridable ReadOnly Property FileForLastModifiedCheck As File
			Get
				Return Me.File
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public Resource createRelative(String relativePath) throws java.io.IOException
		Public Overridable Function createRelative(ByVal relativePath As String) As Resource Implements Resource.createRelative
			Throw New FileNotFoundException("Cannot create a relative resource for " & Me.Description)
		End Function

		Public Overridable ReadOnly Property Filename As String Implements Resource.getFilename
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides Function ToString() As String
			Return Me.Description
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return obj Is Me OrElse TypeOf obj Is Resource AndAlso DirectCast(obj, Resource).Description.Equals(Me.Description)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Me.Description.GetHashCode()
		End Function
	End Class

End Namespace