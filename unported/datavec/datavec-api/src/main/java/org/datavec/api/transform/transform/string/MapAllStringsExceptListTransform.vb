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
'ORIGINAL LINE: @Data @EqualsAndHashCode public class MapAllStringsExceptListTransform extends BaseStringTransform
	<Serializable>
	Public Class MapAllStringsExceptListTransform
		Inherits BaseStringTransform

		Private ReadOnly exceptions As ISet(Of String)
		Private ReadOnly newValue As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MapAllStringsExceptListTransform(@JsonProperty("columnName") String columnName, @JsonProperty("newValue") String newValue, @JsonProperty("exceptions") java.util.List<String> exceptions)
		Public Sub New(ByVal columnName As String, ByVal newValue As String, ByVal exceptions As IList(Of String))
			MyBase.New(columnName)
			Me.newValue = newValue
			Me.exceptions = New HashSet(Of String)(exceptions)
		End Sub

		Public Overrides Function map(ByVal writable As Writable) As Text
			Dim str As String = writable.ToString()
			If exceptions.Contains(str) Then
				Return New Text(str)
			Else
				Return New Text(newValue)
			End If
		End Function

		''' <summary>
		''' Transform an object
		''' in to another object
		''' </summary>
		''' <param name="input"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Public Overrides Function map(ByVal input As Object) As Object
			Dim str As String = input.ToString()
			If exceptions.Contains(str) Then
				Return str
			Else
				Return newValue
			End If
		End Function
	End Class

End Namespace