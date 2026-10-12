Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
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
'ORIGINAL LINE: @AllArgsConstructor @Data public class RecordMetaDataURI implements RecordMetaData
	<Serializable>
	Public Class RecordMetaDataURI
		Implements RecordMetaData

'JAVA TO VB CONVERTER NOTE: The field uri was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly uri_Conflict As URI
'JAVA TO VB CONVERTER NOTE: The field readerClass was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly readerClass_Conflict As Type

		Public Sub New(ByVal uri As URI)
			Me.New(uri, Nothing)
		End Sub

		Public Overridable ReadOnly Property Location As String Implements RecordMetaData.getLocation
			Get
				Return uri_Conflict.ToString()
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
	End Class

End Namespace