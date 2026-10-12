Imports System
Imports System.Collections.Generic
Imports System.IO
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports IOFileFilter = org.apache.commons.io.filefilter.IOFileFilter
Imports RegexFileFilter = org.apache.commons.io.filefilter.RegexFileFilter
Imports SuffixFileFilter = org.apache.commons.io.filefilter.SuffixFileFilter
Imports URIUtil = org.datavec.api.util.files.URIUtil
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports CompactHeapStringList = org.nd4j.common.collection.CompactHeapStringList
Imports MathUtils = org.nd4j.common.util.MathUtils

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

Namespace org.datavec.api.split


	Public Class FileSplit
		Inherits BaseInputSplit

'JAVA TO VB CONVERTER NOTE: The field rootDir was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend rootDir_Conflict As File
		' Use for Collections, pass in list of file type strings
		Protected Friend allowFormat() As String = Nothing
		Protected Friend recursive As Boolean = True
		Protected Friend random As Random
		Protected Friend randomize As Boolean = False


		Protected Friend Sub New(ByVal rootDir As File, ByVal allowFormat() As String, ByVal recursive As Boolean, ByVal random As Random, ByVal runMain As Boolean)
			Me.allowFormat = allowFormat
			Me.recursive = recursive
			Me.rootDir_Conflict = rootDir
			If random IsNot Nothing Then
				Me.random = random
				Me.randomize = True
			End If
			If runMain Then
				Me.initialize()
			End If
		End Sub

		Public Sub New(ByVal rootDir As File)
			Me.New(rootDir, Nothing, True, Nothing, True)
		End Sub

		Public Sub New(ByVal rootDir As File, ByVal rng As Random)
			Me.New(rootDir, Nothing, True, rng, True)
		End Sub

		Public Sub New(ByVal rootDir As File, ByVal allowFormat() As String)
			Me.New(rootDir, allowFormat, True, Nothing, True)
		End Sub

		Public Sub New(ByVal rootDir As File, ByVal allowFormat() As String, ByVal rng As Random)
			Me.New(rootDir, allowFormat, True, rng, True)
		End Sub

		Public Sub New(ByVal rootDir As File, ByVal allowFormat() As String, ByVal recursive As Boolean)
			Me.New(rootDir, allowFormat, recursive, Nothing, True)
		End Sub


		Protected Friend Overridable Sub initialize()
	'        Collection<File> subFiles;

			If rootDir_Conflict Is Nothing Then
				Throw New System.ArgumentException("File path must not be null")
			ElseIf rootDir_Conflict.isAbsolute() AndAlso Not rootDir_Conflict.exists() Then
				Try
					If Not rootDir_Conflict.createNewFile() Then
						Throw New System.ArgumentException("Unable to create file " & rootDir_Conflict.getAbsolutePath())
					'ensure uri strings has the root file if it's not a directory
					Else
						uriStrings = New List(Of String)()
						uriStrings.Add(rootDir_Conflict.toURI().ToString())
					End If
				Catch e As IOException
					Throw New System.InvalidOperationException(e)
				End Try
			ElseIf Not rootDir_Conflict.getAbsoluteFile().exists() Then
				' When implementing wild card characters in the rootDir, remove this if exists,
				' verify expanded paths exist and check for the edge case when expansion cannot be
				' translated to existed locations
				Throw New System.ArgumentException("No such file or directory: " & rootDir_Conflict.getAbsolutePath())
			ElseIf rootDir_Conflict.isDirectory() Then
				Dim list As IList(Of File) = listFiles(rootDir_Conflict, allowFormat, recursive)

				uriStrings = New CompactHeapStringList()

				If randomize Then
					iterationOrder = New Integer(list.Count - 1){}
					For i As Integer = 0 To iterationOrder.Length - 1
						iterationOrder(i) = i
					Next i

					MathUtils.shuffleArray(iterationOrder, random)
				End If
				For Each f As File In list
					uriStrings.Add(URIUtil.fileToURI(f).ToString())
					length_Conflict += 1
				Next f
			Else
				' Lists one file
				Dim toString As String = URIUtil.fileToURI(rootDir_Conflict).ToString() 'URI.getPath(), getRawPath() etc don't have file:/ prefix necessary for conversion back to URI
				uriStrings = New List(Of String)(1)
				uriStrings.Add(toString)
				length_Conflict += rootDir_Conflict.length()
			End If
		End Sub

		Public Overrides Function addNewLocation() As String
			If rootDir_Conflict.isDirectory() Then
				Return addNewLocation((New File(rootDir_Conflict, System.Guid.randomUUID().ToString())).toURI().ToString())
			Else
				'add a file in the same directory as the file with the same extension as the original file
				Return addNewLocation((New File(rootDir_Conflict.getParent(), System.Guid.randomUUID().ToString() & "." & FilenameUtils.getExtension(rootDir_Conflict.getAbsolutePath()))).toURI().ToString())

			End If
		End Function

		Public Overrides Function addNewLocation(ByVal location As String) As String
			Dim f As New File(URI.create(location))
			Try
				f.createNewFile()
			Catch e As IOException
				Throw New System.InvalidOperationException(e)
			End Try

			uriStrings.Add(location)
			length_Conflict += 1
			Return location
		End Function

		Public Overrides Sub updateSplitLocations(ByVal reset As Boolean)
			If reset Then
				initialize()
			End If
		End Sub

		Public Overrides Function needsBootstrapForWrite() As Boolean
			Return locations() Is Nothing OrElse locations().Length < 1 OrElse locations().Length = 1 AndAlso Not locations()(0).isAbsolute()
		End Function

		Public Overrides Sub bootStrapForWrite()
			If locations().Length = 1 AndAlso Not locations()(0).isAbsolute() Then
				Dim parentDir As New File(locations()(0))
				Dim writeFile As New File(parentDir,"write-file")
				Try
					writeFile.createNewFile()
					'since locations are dynamically generated, allow
					uriStrings.Add(writeFile.toURI().ToString())
				Catch e As IOException
					Throw New System.InvalidOperationException(e)
				End Try


			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public OutputStream openOutputStreamFor(String location) throws Exception
		Public Overrides Function openOutputStreamFor(ByVal location As String) As Stream
			Dim ret As FileStream = If(location.StartsWith("file:", StringComparison.Ordinal), New FileStream(URI.create(location), FileMode.Create, FileAccess.Write), New FileStream(location, FileMode.Create, FileAccess.Write))
			Return ret
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public InputStream openInputStreamFor(String location) throws Exception
		Public Overrides Function openInputStreamFor(ByVal location As String) As Stream
			Dim ret As FileStream = If(location.StartsWith("file:", StringComparison.Ordinal), New FileStream(URI.create(location), FileMode.Open, FileAccess.Read), New FileStream(location, FileMode.Open, FileAccess.Read))
			Return ret
		End Function

		Public Overrides Function length() As Long
			Return length_Conflict
		End Function

		Public Overrides Sub reset()
			If randomize Then
				'Shuffle the iteration order
				MathUtils.shuffleArray(iterationOrder, random)
			End If
		End Sub

		Public Overrides Function resetSupported() As Boolean
			Return True
		End Function


		Public Overridable ReadOnly Property RootDir As File
			Get
				Return rootDir_Conflict
			End Get
		End Property

		Private Function listFiles(ByVal dir As File, ByVal allowedFormats() As String, ByVal recursive As Boolean) As IList(Of File)
			Preconditions.checkState(dir.isDirectory(), "Argument is not a directory: %s", dir)
			Dim filter As IOFileFilter
			If allowedFormats Is Nothing Then
				filter = New RegexFileFilter(".*")
			Else
				filter = New SuffixFileFilter(allowedFormats)
			End If

			Dim queue As New LinkedList(Of File)()
			queue.AddLast(dir)

			Dim [out] As IList(Of File) = New List(Of File)()
			Do While queue.Count > 0
'JAVA TO VB CONVERTER NOTE: The local variable listFiles was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
				Dim listFiles_Conflict() As File = queue.RemoveFirst().listFiles()
				If listFiles_Conflict IsNot Nothing Then
					For Each f As File In listFiles_Conflict
						Dim isDir As Boolean = f.isDirectory()
						If isDir AndAlso recursive Then
							queue.AddLast(f)
						ElseIf Not isDir AndAlso filter.accept(f) Then
							[out].Add(f)
						End If
					Next f
				End If
			Loop
			Return [out]
		End Function
	End Class



End Namespace