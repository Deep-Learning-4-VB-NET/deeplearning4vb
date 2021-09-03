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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data public class ReplaceStringTransform extends BaseStringTransform
	<Serializable>
	Public Class ReplaceStringTransform
		Inherits BaseStringTransform

'JAVA TO VB CONVERTER NOTE: The field map was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly map_Conflict As IDictionary(Of String, String)

		''' <summary>
		''' Constructs a new ReplaceStringTransform using the specified </summary>
		''' <param name="columnName"> Name of the column </param>
		''' <param name="map"> Key: regular expression; Value: replacement value </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ReplaceStringTransform(@JsonProperty("columnName") String columnName, @JsonProperty("map") java.util.Map<String, String> map)
		Public Sub New(ByVal columnName As String, ByVal map As IDictionary(Of String, String))
			MyBase.New(columnName)
			Me.map_Conflict = map
		End Sub

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.writable.Text map(final org.datavec.api.writable.Writable writable)
		Public Overrides Function map(ByVal writable As Writable) As Text
			Dim value As String = writable.ToString()
			value = replaceAll(value)
			Return New Text(value)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: @Override public Object map(final Object o)
		Public Overrides Function map(ByVal o As Object) As Object
			Dim value As String = o.ToString()
			value = replaceAll(value)
			Return value
		End Function

		Private Function replaceAll(ByVal value As String) As String
			If map_Conflict IsNot Nothing AndAlso map_Conflict.Count > 0 Then
				For Each entry As KeyValuePair(Of String, String) In map_Conflict.SetOfKeyValuePairs()
					value = value.replaceAll(entry.Key, entry.Value)
				Next entry
			End If
			Return value
		End Function

	End Class

End Namespace