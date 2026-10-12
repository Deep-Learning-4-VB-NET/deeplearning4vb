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


	Public Class InFileAndMemoryDataSetCache
		Implements DataSetCache

		Private fileCache As InFileDataSetCache
		Private memoryCache As InMemoryDataSetCache

		Public Sub New(ByVal cacheDirectory As File)
			Me.fileCache = New InFileDataSetCache(cacheDirectory)
			Me.memoryCache = New InMemoryDataSetCache()
		End Sub

		Public Sub New(ByVal cacheDirectory As Path)
			Me.New(cacheDirectory.toFile())
		End Sub

		Public Sub New(ByVal cacheDirectory As String)
			Me.New(New File(cacheDirectory))
		End Sub

		Public Overridable Function isComplete(ByVal [namespace] As String) As Boolean Implements DataSetCache.isComplete
			Return fileCache.isComplete([namespace]) OrElse memoryCache.isComplete([namespace])
		End Function

		Public Overridable Sub setComplete(ByVal [namespace] As String, ByVal value As Boolean) Implements DataSetCache.setComplete
			fileCache.setComplete([namespace], value)
			memoryCache.setComplete([namespace], value)
		End Sub

		Public Overridable Function get(ByVal key As String) As DataSet Implements DataSetCache.get
			Dim dataSet As DataSet = Nothing

			If memoryCache.contains(key) Then
				dataSet = memoryCache.get(key)
				If Not fileCache.contains(key) Then
					fileCache.put(key, dataSet)
				End If
			ElseIf fileCache.contains(key) Then
				dataSet = fileCache.get(key)
				If dataSet IsNot Nothing AndAlso Not memoryCache.contains(key) Then
					memoryCache.put(key, dataSet)
				End If
			End If

			Return dataSet
		End Function

		Public Overridable Sub put(ByVal key As String, ByVal dataSet As DataSet) Implements DataSetCache.put
			fileCache.put(key, dataSet)
			memoryCache.put(key, dataSet)
		End Sub

		Public Overridable Function contains(ByVal key As String) As Boolean Implements DataSetCache.contains
			Return memoryCache.contains(key) OrElse fileCache.contains(key)
		End Function
	End Class

End Namespace