Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Data = lombok.Data

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

Namespace org.datavec.api.records.metadata


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class RecordMetaDataComposableMap implements RecordMetaData
	<Serializable>
	Public Class RecordMetaDataComposableMap
		Implements RecordMetaData

'JAVA TO VB CONVERTER NOTE: The field readerClass was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private readerClass_Conflict As Type
		Private meta As IDictionary(Of String, RecordMetaData)

		Public Sub New(ByVal recordMetaDatas As IDictionary(Of String, RecordMetaData))
			Me.New(Nothing, recordMetaDatas)
		End Sub

		Public Sub New(ByVal readerClass As Type, ByVal recordMetaDatas As IDictionary(Of String, RecordMetaData))
			Me.readerClass_Conflict = readerClass
			Me.meta = recordMetaDatas
		End Sub

		Public Overridable ReadOnly Property Location As String Implements RecordMetaData.getLocation
			Get
				Dim sb As New StringBuilder()
				sb.Append("locations(")
				Dim first As Boolean = True
				For Each rmd As KeyValuePair(Of String, RecordMetaData) In meta.SetOfKeyValuePairs()
					If Not first Then
						sb.Append(",")
					End If
					sb.Append(rmd.Key).Append("=")
					sb.Append(rmd.Value.getLocation())
					first = False
				Next rmd
				sb.Append(")")
				Return sb.ToString()
			End Get
		End Property

		Public Overridable ReadOnly Property URI As URI Implements RecordMetaData.getURI
			Get
				Dim first As String = meta.Keys.GetEnumerator().next()
				Return meta(first).getURI()
			End Get
		End Property

		Public Overridable ReadOnly Property ReaderClass As Type Implements RecordMetaData.getReaderClass
			Get
				Return readerClass_Conflict
			End Get
		End Property
	End Class

End Namespace