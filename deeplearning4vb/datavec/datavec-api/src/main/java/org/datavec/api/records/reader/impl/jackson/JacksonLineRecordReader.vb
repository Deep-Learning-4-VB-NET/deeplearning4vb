Imports System
Imports System.Collections.Generic
Imports LineRecordReader = org.datavec.api.records.reader.impl.LineRecordReader
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper

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

Namespace org.datavec.api.records.reader.impl.jackson


	<Serializable>
	Public Class JacksonLineRecordReader
		Inherits LineRecordReader

		Private selection As FieldSelection
		Private mapper As ObjectMapper

		Public Sub New(ByVal selection As FieldSelection, ByVal mapper As ObjectMapper)
			Me.selection = selection
			Me.mapper = mapper
		End Sub

		Public Overrides Function [next]() As IList(Of Writable)
			Dim t As Text = CType(MyBase.next().GetEnumerator().next(), Text)
			Dim val As String = t.ToString()
			Return parseLine(val)
		End Function

		Protected Friend Overridable Function parseLine(ByVal line As String) As IList(Of Writable)
			Return JacksonReaderUtils.parseRecord(line, selection, mapper)
		End Function

		Public Overrides ReadOnly Property Labels As IList(Of String)
			Get
				Throw New System.NotSupportedException()
			End Get
		End Property
	End Class

End Namespace