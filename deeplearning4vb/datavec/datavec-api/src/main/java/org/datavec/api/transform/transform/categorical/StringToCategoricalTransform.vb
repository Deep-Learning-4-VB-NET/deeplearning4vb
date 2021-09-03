Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports CategoricalMetaData = org.datavec.api.transform.metadata.CategoricalMetaData
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports BaseColumnTransform = org.datavec.api.transform.transform.BaseColumnTransform
Imports Writable = org.datavec.api.writable.Writable
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
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

Namespace org.datavec.api.transform.transform.categorical


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"inputSchema", "columnNumber"}) @Data public class StringToCategoricalTransform extends org.datavec.api.transform.transform.BaseColumnTransform
	<Serializable>
	Public Class StringToCategoricalTransform
		Inherits BaseColumnTransform

		Private ReadOnly stateNames As IList(Of String)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public StringToCategoricalTransform(@JsonProperty("columnName") String columnName, @JsonProperty("stateNames") java.util.List<String> stateNames)
		Public Sub New(ByVal columnName As String, ByVal stateNames As IList(Of String))
			MyBase.New(columnName)
			If stateNames Is Nothing OrElse stateNames.Count = 0 Then
				Throw New System.ArgumentException("State names must not be null or empty")
			End If

			Me.stateNames = stateNames
		End Sub

		Public Sub New(ByVal columnName As String, ParamArray ByVal stateNames() As String)
			Me.New(columnName, Arrays.asList(stateNames))
		End Sub

		Public Overrides Function getNewColumnMetaData(ByVal newColumnName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
			Return New CategoricalMetaData(newColumnName, stateNames)
		End Function

		Public Overrides Function map(ByVal columnWritable As Writable) As Writable
			Return columnWritable
		End Function

		Public Overrides Function ToString() As String
			Return "StringToCategoricalTransform(stateNames=" & stateNames & ")"
		End Function

		''' <summary>
		''' Transform an object
		''' in to another object
		''' </summary>
		''' <param name="input"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Public Overridable Overloads Function map(ByVal input As Object) As Object
			Return input
		End Function

		''' <summary>
		''' Transform a sequence
		''' </summary>
		''' <param name="sequence"> </param>
		Public Overrides Function mapSequence(ByVal sequence As Object) As Object
			Return sequence
		End Function
	End Class

End Namespace