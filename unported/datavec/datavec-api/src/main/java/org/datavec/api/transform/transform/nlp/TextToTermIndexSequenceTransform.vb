Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports IntegerMetaData = org.datavec.api.transform.metadata.IntegerMetaData
Imports BaseSequenceExpansionTransform = org.datavec.api.transform.sequence.expansion.BaseSequenceExpansionTransform
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports Writable = org.datavec.api.writable.Writable
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
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

Namespace org.datavec.api.transform.transform.nlp


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true, exclude = {"writableMap"}) @JsonIgnoreProperties({"writableMap"}) @JsonInclude(JsonInclude.Include.NON_NULL) public class TextToTermIndexSequenceTransform extends org.datavec.api.transform.sequence.expansion.BaseSequenceExpansionTransform
	<Serializable>
	Public Class TextToTermIndexSequenceTransform
		Inherits BaseSequenceExpansionTransform

		Private wordIndexMap As IDictionary(Of String, Integer)
		Private delimiter As String
		Private exceptionOnUnknown As Boolean
		<NonSerialized>
		Private writableMap As IDictionary(Of String, IList(Of Writable))

		''' 
		''' <param name="columnName">         Name of the text column </param>
		''' <param name="newColumnName">      Name of the column after expansion </param>
		''' <param name="vocabulary">         Vocabulary </param>
		''' <param name="delimiter">          Delimiter </param>
		''' <param name="exceptionOnUnknown"> If true: throw an exception on unknown characters. False: skip unknown characters. </param>
		Public Sub New(ByVal columnName As String, ByVal newColumnName As String, ByVal vocabulary As IList(Of String), ByVal delimiter As String, ByVal exceptionOnUnknown As Boolean)
			MyBase.New(Collections.singletonList(columnName), Collections.singletonList(newColumnName))
			Me.wordIndexMap = New Dictionary(Of String, Integer)()
			For i As Integer = 0 To vocabulary.Count - 1
				Me.wordIndexMap(vocabulary(i)) = i
			Next i
			Me.delimiter = delimiter
			Me.exceptionOnUnknown = exceptionOnUnknown
		End Sub

		''' 
		''' <param name="columnName">         Name of the text column </param>
		''' <param name="newColumnName">      Name of the column after expansion </param>
		''' <param name="wordIndexMap">       Map from terms in vocabulary to indeces </param>
		''' <param name="delimiter">          Delimiter </param>
		''' <param name="exceptionOnUnknown"> If true: throw an exception on unknown characters. False: skip unknown characters. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public TextToTermIndexSequenceTransform(@JsonProperty("columnName") String columnName, @JsonProperty("newColumnName") String newColumnName, @JsonProperty("wordIndexMap") Map<String,Integer> wordIndexMap, @JsonProperty("delimiter") String delimiter, @JsonProperty("exceptionOnUnknown") boolean exceptionOnUnknown)
		Public Sub New(ByVal columnName As String, ByVal newColumnName As String, ByVal wordIndexMap As IDictionary(Of String, Integer), ByVal delimiter As String, ByVal exceptionOnUnknown As Boolean)
			MyBase.New(Collections.singletonList(columnName), Collections.singletonList(newColumnName))
			Me.wordIndexMap = wordIndexMap
			Me.delimiter = delimiter
			Me.exceptionOnUnknown = exceptionOnUnknown
		End Sub

		Protected Friend Overrides Function expandedColumnMetaDatas(ByVal origColumnMeta As IList(Of ColumnMetaData), ByVal expandedColumnNames As IList(Of String)) As IList(Of ColumnMetaData)
			Return Collections.singletonList(Of ColumnMetaData)(New IntegerMetaData(expandedColumnNames(0), 0, wordIndexMap.Count - 1))
		End Function

		Protected Friend Overrides Function expandTimeStep(ByVal currentStepValues As IList(Of Writable)) As IList(Of IList(Of Writable))
			If writableMap Is Nothing Then
				Dim m As IDictionary(Of String, IList(Of Writable)) = New Dictionary(Of String, IList(Of Writable))()
				For Each entry As KeyValuePair(Of String, Integer) In wordIndexMap.SetOfKeyValuePairs()
					m(entry.Key) = Collections.singletonList(Of Writable)(New IntWritable(entry.Value))
				Next entry
				writableMap = m
			End If
			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim text As String = currentStepValues(0).ToString()
			Dim tokens() As String = text.Split(Me.delimiter, True)
			For Each token As String In tokens
				Dim w As IList(Of Writable) = writableMap(token)
				If w Is Nothing Then
					If exceptionOnUnknown Then
						Throw New System.InvalidOperationException("Unknown token found in text: """ & token & """")
					End If
					Continue For
				End If
				[out].Add(w)
			Next token

			Return [out]
		End Function
	End Class
End Namespace