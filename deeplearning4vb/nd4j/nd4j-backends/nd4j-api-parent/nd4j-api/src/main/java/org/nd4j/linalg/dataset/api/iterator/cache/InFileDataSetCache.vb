Imports System
Imports DataSet = org.nd4j.linalg.dataset.DataSet

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

Namespace org.nd4j.linalg.dataset.api.iterator.cache


	Public Class InFileDataSetCache
		Implements DataSetCache

		Private cacheDirectory As File

		Public Sub New(ByVal cacheDirectory As File)
			If cacheDirectory.exists() AndAlso Not cacheDirectory.isDirectory() Then
				Throw New System.ArgumentException("can't use path " & cacheDirectory & " as file cache directory " & "because it already exists, but is not a directory")
			End If
			Me.cacheDirectory = cacheDirectory
		End Sub

		Public Sub New(ByVal cacheDirectory As Path)
			Me.New(cacheDirectory.toFile())
		End Sub

		Public Sub New(ByVal cacheDirectory As String)
			Me.New(New File(cacheDirectory))
		End Sub

		Private Function resolveKey(ByVal key As String) As File
			Dim filename As String = key.replaceAll("[^a-zA-Z0-9.-]", "_")
			Return New File(cacheDirectory, filename)
		End Function

		Private Function namespaceFile(ByVal [namespace] As String) As File
			Dim filename As String = String.Format("{0}-complete.txt", [namespace])
			Return New File(cacheDirectory, filename)
		End Function

		Public Overridable Function isComplete(ByVal [namespace] As String) As Boolean Implements DataSetCache.isComplete
			Return namespaceFile([namespace]).exists()
		End Function

		Public Overridable Sub setComplete(ByVal [namespace] As String, ByVal value As Boolean) Implements DataSetCache.setComplete
			Dim file As File = namespaceFile([namespace])
			If value Then
				If Not file.exists() Then
					Dim parentFile As File = file.getParentFile()
					parentFile.mkdirs()
					Try
						file.createNewFile()
					Catch e As IOException
						Throw New Exception(e)
					End Try
				End If
			Else
				If file.exists() Then
					file.delete()
				End If
			End If
		End Sub

		Public Overridable Function get(ByVal key As String) As DataSet Implements DataSetCache.get
			Dim file As File = resolveKey(key)

			If Not file.exists() Then
				Return Nothing
			ElseIf Not file.isFile() Then
				Throw New System.InvalidOperationException("ERROR: cannot read DataSet: cache path " & file & " is not a file")
			Else
				Dim ds As New DataSet()
				ds.load(file)
				Return ds
			End If
		End Function

		Public Overridable Sub put(ByVal key As String, ByVal dataSet As DataSet) Implements DataSetCache.put
			Dim file As File = resolveKey(key)

			Dim parentDir As File = file.getParentFile()
			If Not parentDir.exists() Then
				If Not parentDir.mkdirs() Then
					Throw New System.InvalidOperationException("ERROR: cannot create parent directory: " & parentDir)
				End If
			End If

			If file.exists() Then
				file.delete()
			End If

			dataSet.save(file)
		End Sub

		Public Overridable Function contains(ByVal key As String) As Boolean Implements DataSetCache.contains
			Dim file As File = resolveKey(key)

			Dim exists As Boolean? = file.exists()
			If exists.Value AndAlso Not file.isFile() Then
				Throw New System.InvalidOperationException("ERROR: DataSet cache path " & file & " exists but is not a file")
			End If

			Return exists
		End Function
	End Class

End Namespace