Imports System
Imports System.Collections.Generic
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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data public class StringMapTransform extends BaseStringTransform
	<Serializable>
	Public Class StringMapTransform
		Inherits BaseStringTransform

'JAVA TO VB CONVERTER NOTE: The field map was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly map_Conflict As IDictionary(Of String, String)

		''' 
		''' <param name="columnName"> Name of the column </param>
		''' <param name="map"> Key: From. Value: To </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public StringMapTransform(@JsonProperty("columnName") String columnName, @JsonProperty("map") java.util.Map<String, String> map)
		Public Sub New(ByVal columnName As String, ByVal map As IDictionary(Of String, String))
			MyBase.New(columnName)
			Me.map_Conflict = map
		End Sub

		Public Overrides Function map(ByVal writable As Writable) As Text
			Dim orig As String = writable.ToString()
			If map_Conflict.ContainsKey(orig) Then
				Return New Text(map(orig))
			End If

			If TypeOf writable Is Text Then
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
			Dim orig As String = input.ToString()
			If map_Conflict.ContainsKey(orig) Then
				Return map(orig)
			End If

			If TypeOf input Is String Then
				Return input
			Else
				Return orig
			End If
		End Function
	End Class

End Namespace