Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports IntegerMetaData = org.datavec.api.transform.metadata.IntegerMetaData
Imports BaseSequenceExpansionTransform = org.datavec.api.transform.sequence.expansion.BaseSequenceExpansionTransform
Imports IntWritable = org.datavec.api.writable.IntWritable
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

Namespace org.datavec.api.transform.transform.nlp


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true, exclude = {""}) public class TextToCharacterIndexTransform extends org.datavec.api.transform.sequence.expansion.BaseSequenceExpansionTransform
	<Serializable>
	Public Class TextToCharacterIndexTransform
		Inherits BaseSequenceExpansionTransform

		Private characterIndexMap As IDictionary(Of Char, Integer)
		Private exceptionOnUnknown As Boolean
		<NonSerialized>
		Private writableMap As IDictionary(Of Char, IList(Of Writable))

		''' 
		''' <param name="columnName">         Name of the text column </param>
		''' <param name="newColumnName">      Name of the column after expansion </param>
		''' <param name="characterIndexMap">  Character to integer index map </param>
		''' <param name="exceptionOnUnknown"> If true: throw an exception on unknown characters. False: skip unknown characters. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public TextToCharacterIndexTransform(@JsonProperty("columnName") String columnName, @JsonProperty("newColumnName") String newColumnName, @JsonProperty("characterIndexMap") Map<Char,Integer> characterIndexMap, @JsonProperty("exceptionOnUnknown") boolean exceptionOnUnknown)
		Public Sub New(ByVal columnName As String, ByVal newColumnName As String, ByVal characterIndexMap As IDictionary(Of Char, Integer), ByVal exceptionOnUnknown As Boolean)
			MyBase.New(Collections.singletonList(columnName), Collections.singletonList(newColumnName))
			Me.characterIndexMap = characterIndexMap
			Me.exceptionOnUnknown = exceptionOnUnknown
		End Sub

		Protected Friend Overrides Function expandedColumnMetaDatas(ByVal origColumnMeta As IList(Of ColumnMetaData), ByVal expandedColumnNames As IList(Of String)) As IList(Of ColumnMetaData)
			Return Collections.singletonList(Of ColumnMetaData)(New IntegerMetaData(expandedColumnNames(0), 0, characterIndexMap.Count - 1))
		End Function

		Protected Friend Overrides Function expandTimeStep(ByVal currentStepValues As IList(Of Writable)) As IList(Of IList(Of Writable))
			If writableMap Is Nothing Then
				Dim m As IDictionary(Of Char, IList(Of Writable)) = New Dictionary(Of Char, IList(Of Writable))()
				For Each entry As KeyValuePair(Of Char, Integer) In characterIndexMap.SetOfKeyValuePairs()
					m(entry.Key) = Collections.singletonList(Of Writable)(New IntWritable(entry.Value))
				Next entry
				writableMap = m
			End If
			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim cArr() As Char = currentStepValues(0).ToString().ToCharArray()
			For Each c As Char In cArr
				Dim w As IList(Of Writable) = writableMap(c)
				If w Is Nothing Then
					If exceptionOnUnknown Then
						Throw New System.InvalidOperationException("Unknown character found in text: """ & AscW(c) & """")
					End If
					Continue For
				End If

				[out].Add(w)
			Next c

			Return [out]
		End Function
	End Class

End Namespace