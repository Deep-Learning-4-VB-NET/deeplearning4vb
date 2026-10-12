Imports System
Imports Data = lombok.Data
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.datavec.api.transform.transform.string

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class ChangeCaseStringTransform extends BaseStringTransform
	<Serializable>
	Public Class ChangeCaseStringTransform
		Inherits BaseStringTransform

		Public Enum CaseType
			LOWER
			UPPER
		End Enum

		Private ReadOnly caseType As CaseType

		Public Sub New(ByVal column As String)
			MyBase.New(column)
			Me.caseType = CaseType.LOWER ' default is all lower case
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ChangeCaseStringTransform(@JsonProperty("column") String column, @JsonProperty("caseType") CaseType caseType)
		Public Sub New(ByVal column As String, ByVal caseType As CaseType)
			MyBase.New(column)
			Me.caseType = caseType
		End Sub

		Private Function mapHelper(ByVal input As String) As String
			Dim result As String
			Select Case caseType
				Case org.datavec.api.transform.transform.string.ChangeCaseStringTransform.CaseType.UPPER
					result = input.ToUpper()
				Case Else
					result = input.ToLower()
			End Select
			Return result
		End Function

		Public Overrides Function map(ByVal writable As Writable) As Text
			Return New Text(mapHelper(writable.ToString()))
		End Function

		Public Overrides Function map(ByVal input As Object) As Object
			Return mapHelper(input.ToString())
		End Function
	End Class

End Namespace