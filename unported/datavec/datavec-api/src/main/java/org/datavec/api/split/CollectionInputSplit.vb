Imports System.Collections.Generic
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

Namespace org.datavec.api.split


	Public Class CollectionInputSplit
		Inherits BaseInputSplit

		Public Sub New(ByVal array() As URI)
			Me.New(Arrays.asList(array))
		End Sub

		Public Sub New(ByVal list As ICollection(Of URI))
			uriStrings = New List(Of String)(list.Count)
			For Each uri As URI In list
				uriStrings.Add(uri.ToString())
			Next uri
		End Sub

		Public Overrides Sub updateSplitLocations(ByVal reset As Boolean)

		End Sub

		Public Overrides Function needsBootstrapForWrite() As Boolean
			Return False
		End Function

		Public Overrides Sub bootStrapForWrite()

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public OutputStream openOutputStreamFor(String location) throws Exception
		Public Overrides Function openOutputStreamFor(ByVal location As String) As Stream
			Return Nothing
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public InputStream openInputStreamFor(String location) throws Exception
		Public Overrides Function openInputStreamFor(ByVal location As String) As Stream
			Return Nothing
		End Function

		Public Overrides Function length() As Long
			Return uriStrings.Count
		End Function

		Public Overrides Sub reset()
			'No op
		End Sub

		Public Overrides Function resetSupported() As Boolean
			Return True
		End Function

	End Class

End Namespace