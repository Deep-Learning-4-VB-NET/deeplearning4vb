Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Record = org.datavec.api.records.Record
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataIndex = org.datavec.api.records.metadata.RecordMetaDataIndex
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.arrow.recordreader


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class ArrowRecord implements org.datavec.api.records.Record
	<Serializable>
	Public Class ArrowRecord
		Implements Record

		Private arrowWritableRecordBatch As ArrowWritableRecordBatch
		Private index As Integer
		Private recordUri As URI

		Public Overridable Property Record As IList(Of Writable) Implements Record.getRecord
			Get
				Return arrowWritableRecordBatch(index)
			End Get
			Set(ByVal record As IList(Of Writable))
				arrowWritableRecordBatch(index) = record
			End Set
		End Property


		Public Overridable Property MetaData As RecordMetaData Implements Record.getMetaData
			Get
				Dim ret As RecordMetaData = New RecordMetaDataIndex(index,recordUri,GetType(ArrowRecordReader))
				Return ret
			End Get
			Set(ByVal recordMetaData As RecordMetaData)
    
			End Set
		End Property

	End Class

End Namespace