Imports System.Collections.Generic
Imports System.IO
Imports DataSet = org.nd4j.linalg.dataset.DataSet
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

Namespace org.nd4j.linalg.dataset.api.iterator.cache


	Public Class InMemoryDataSetCache
		Implements DataSetCache

		Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(DataSetCache))

		Private cache As IDictionary(Of String, SByte()) = New Dictionary(Of String, SByte())()
		Private completeNamespaces As ISet(Of String) = New HashSet(Of String)()

		Public Overridable Function isComplete(ByVal [namespace] As String) As Boolean Implements DataSetCache.isComplete
			Return completeNamespaces.Contains([namespace])
		End Function

		Public Overridable Sub setComplete(ByVal [namespace] As String, ByVal value As Boolean) Implements DataSetCache.setComplete
			If value Then
				completeNamespaces.Add([namespace])
			Else
				completeNamespaces.remove([namespace])
			End If
		End Sub

		Public Overridable Function get(ByVal key As String) As DataSet Implements DataSetCache.get

			If Not cache.ContainsKey(key) Then
				Return Nothing
			End If

			Dim data() As SByte = cache(key)

			Dim [is] As New MemoryStream(data)

			Dim ds As New DataSet()

			ds.load([is])

			Return ds
		End Function

		Public Overridable Sub put(ByVal key As String, ByVal dataSet As DataSet) Implements DataSetCache.put
			If cache.ContainsKey(key) Then
				log.debug("evicting key %s from data set cache", key)
				cache.Remove(key)
			End If

			Dim os As New MemoryStream()

			dataSet.save(os)

			cache(key) = os.toByteArray()
		End Sub

		Public Overridable Function contains(ByVal key As String) As Boolean Implements DataSetCache.contains
			Return cache.ContainsKey(key)
		End Function
	End Class

End Namespace