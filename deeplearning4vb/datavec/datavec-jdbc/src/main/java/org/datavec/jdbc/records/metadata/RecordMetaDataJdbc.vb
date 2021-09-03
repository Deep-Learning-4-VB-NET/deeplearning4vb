Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData

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

Namespace org.datavec.jdbc.records.metadata

	<Serializable>
	Public Class RecordMetaDataJdbc
		Implements RecordMetaData

'JAVA TO VB CONVERTER NOTE: The field uri was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly uri_Conflict As URI
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final String request;
		Private ReadOnly request As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final java.util.List<Object> params;
		Private ReadOnly params As IList(Of Object)
'JAVA TO VB CONVERTER NOTE: The field readerClass was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly readerClass_Conflict As Type

'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
'ORIGINAL LINE: public RecordMetaDataJdbc(java.net.URI uri, String request, java.util.List<? extends Object> params, @Class readerClass)
		Public Sub New(ByVal uri As URI, ByVal request As String, ByVal params As IList(Of Object), ByVal readerClass As Type)
			Me.uri_Conflict = uri
			Me.request = request
			Me.params = Collections.unmodifiableList(params)
			Me.readerClass_Conflict = readerClass
		End Sub

		Public Overridable ReadOnly Property Location As String Implements RecordMetaData.getLocation
			Get
				Return Me.ToString()
			End Get
		End Property

		Public Overridable ReadOnly Property URI As URI Implements RecordMetaData.getURI
			Get
				Return uri_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property ReaderClass As Type Implements RecordMetaData.getReaderClass
			Get
				Return readerClass_Conflict
			End Get
		End Property

		Public Overrides Function ToString() As String
			Return "jdbcRecord(uri=" & uri_Conflict & ", request='" & request & "'"c & ", parameters='" & params.ToString() & "'"c & ", readerClass=" & readerClass_Conflict & ")"c
		End Function
	End Class

End Namespace