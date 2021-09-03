Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data public class ReplaceEmptyStringTransform extends BaseStringTransform
	<Serializable>
	Public Class ReplaceEmptyStringTransform
		Inherits BaseStringTransform

		Private value As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ReplaceEmptyStringTransform(@JsonProperty("columnName") String columnName, @JsonProperty("value") String value)
		Public Sub New(ByVal columnName As String, ByVal value As String)
			MyBase.New(columnName)
			Me.value = value
		End Sub

		Public Overrides Function map(ByVal writable As Writable) As Text
			Dim s As String = writable.ToString()
			If s Is Nothing OrElse s.Length = 0 Then
				Return New Text(value)
			ElseIf TypeOf writable Is Text Then
				Return DirectCast(writable, Text)
			Else
				Return New Text(writable.ToString())
			End If
		End Function

		''' <summary>
		''' Transform an object
		''' in to another object
		''' </summary>
		''' <param name="input"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Public Overrides Function map(ByVal input As Object) As Object
			Dim s As String = input.ToString()
			If s Is Nothing OrElse s.Length = 0 Then
				Return value
			ElseIf TypeOf s Is String Then
				Return s
			Else
				Return s
			End If
		End Function
	End Class

End Namespace