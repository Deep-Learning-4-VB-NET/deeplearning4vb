Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports FileUtils = org.apache.commons.io.FileUtils
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports NDArrayMetaData = org.datavec.api.transform.metadata.NDArrayMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports BaseTransform = org.datavec.api.transform.transform.BaseTransform
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.datavec.api.transform.transform.string


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"inputSchema", "map", "columnIdx"}) @EqualsAndHashCode(callSuper = false, exclude = {"columnIdx"}) @Data public class StringListToCountsNDArrayTransform extends org.datavec.api.transform.transform.BaseTransform
	<Serializable>
	Public Class StringListToCountsNDArrayTransform
		Inherits BaseTransform

'JAVA TO VB CONVERTER NOTE: The field columnName was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend ReadOnly columnName_Conflict As String
		Protected Friend ReadOnly newColumnName As String
		Protected Friend ReadOnly vocabulary As IList(Of String)
		Protected Friend ReadOnly delimiter As String
		Protected Friend ReadOnly binary As Boolean
		Protected Friend ReadOnly ignoreUnknown As Boolean

'JAVA TO VB CONVERTER NOTE: The field map was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend ReadOnly map_Conflict As IDictionary(Of String, Integer)

		Protected Friend columnIdx As Integer = -1

		''' <param name="columnName">     The name of the column to convert </param>
		''' <param name="vocabulary">     The possible tokens that may be present. </param>
		''' <param name="delimiter">      The delimiter for the Strings to convert </param>
		''' <param name="ignoreUnknown">  Whether to ignore unknown tokens </param>
		Public Sub New(ByVal columnName As String, ByVal vocabulary As IList(Of String), ByVal delimiter As String, ByVal binary As Boolean, ByVal ignoreUnknown As Boolean)
			Me.New(columnName, columnName & "[BOW]", vocabulary, delimiter, binary, ignoreUnknown)
		End Sub

		''' <param name="columnName">     The name of the column to convert </param>
		''' <param name="vocabulary">     The possible tokens that may be present. </param>
		''' <param name="delimiter">      The delimiter for the Strings to convert </param>
		''' <param name="ignoreUnknown">  Whether to ignore unknown tokens </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public StringListToCountsNDArrayTransform(@JsonProperty("columnName") String columnName, @JsonProperty("newColumnName") String newColumnName, @JsonProperty("vocabulary") List<String> vocabulary, @JsonProperty("delimiter") String delimiter, @JsonProperty("binary") boolean binary, @JsonProperty("ignoreUnknown") boolean ignoreUnknown)
		Public Sub New(ByVal columnName As String, ByVal newColumnName As String, ByVal vocabulary As IList(Of String), ByVal delimiter As String, ByVal binary As Boolean, ByVal ignoreUnknown As Boolean)
			Me.columnName_Conflict = columnName
			Me.newColumnName = newColumnName
			Me.vocabulary = vocabulary
			Me.delimiter = delimiter
			Me.binary = binary
			Me.ignoreUnknown = ignoreUnknown

			map_Conflict = New Dictionary(Of String, Integer)()
			For i As Integer = 0 To vocabulary.Count - 1
				map(vocabulary(i)) = i
			Next i
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static List<String> readVocabFromFile(String path) throws java.io.IOException
		Public Shared Function readVocabFromFile(ByVal path As String) As IList(Of String)
			Return FileUtils.readLines(New File(path), "utf-8")
		End Function

		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema

			Dim colIdx As Integer = inputSchema.getIndexOfColumn(columnName_Conflict)

			Dim oldMeta As IList(Of ColumnMetaData) = inputSchema.getColumnMetaData()
			Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)()
			Dim oldNames As IList(Of String) = inputSchema.getColumnNames()

			Dim typesIter As IEnumerator(Of ColumnMetaData) = oldMeta.GetEnumerator()
			Dim namesIter As IEnumerator(Of String) = oldNames.GetEnumerator()

			Dim i As Integer = 0
			Do While typesIter.MoveNext()
				Dim t As ColumnMetaData = typesIter.Current
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim name As String = namesIter.next()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (i++ == colIdx)
				If i = colIdx Then
						i += 1
					'Replace String column with a set of binary/integer columns
					If t.ColumnType <> ColumnType.String Then
						Throw New System.InvalidOperationException("Cannot convert non-string type")
					End If

					Dim meta As ColumnMetaData = New NDArrayMetaData(newColumnName, New Long() {vocabulary.Count})
					newMeta.Add(meta)
				Else
						i += 1
					newMeta.Add(t)
				End If
			Loop

			Return inputSchema.newSchema(newMeta)

		End Function

		Public Overrides WriteOnly Property InputSchema As Schema
			Set(ByVal inputSchema As Schema)
				Me.inputSchema_Conflict = inputSchema
				Me.columnIdx = inputSchema.getIndexOfColumn(columnName_Conflict)
			End Set
		End Property

		Public Overrides Function ToString() As String
			Return "StringListToCountsTransform(columnName=" & columnName_Conflict & ",vocabularySize=" & vocabulary.Count & ",delimiter=""" & delimiter & """)"
		End Function

		Protected Friend Overridable Function getIndices(ByVal text As String) As ICollection(Of Integer)
			Dim indices As ICollection(Of Integer)
			If binary Then
				indices = New HashSet(Of Integer)()
			Else
				indices = New List(Of Integer)()
			End If
			If text IsNot Nothing AndAlso text.Length > 0 Then
				Dim split() As String = text.Split(delimiter, True)
				For Each s As String In split
					Dim idx As Integer? = map(s)
					If idx Is Nothing AndAlso Not ignoreUnknown Then
						Throw New System.InvalidOperationException("Encountered unknown String: """ & s & """")
					ElseIf idx IsNot Nothing Then
						indices.Add(idx)
					End If
				Next s
			End If
			Return indices
		End Function

		Protected Friend Overridable Function makeBOWNDArray(ByVal indices As ICollection(Of Integer)) As INDArray
			Dim counts As INDArray = Nd4j.zeros(1, vocabulary.Count)
			For Each idx As Integer? In indices
				counts.putScalar(idx, counts.getDouble(idx) + 1)
			Next idx
			Nd4j.Executioner.commit()
			Return counts
		End Function

		Public Overridable Overloads Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable)
			If writables.Count <> inputSchema_Conflict.numColumns() Then
				Throw New System.InvalidOperationException("Cannot execute transform: input writables list length (" & writables.Count & ") does not " & "match expected number of elements (schema: " & inputSchema_Conflict.numColumns() & "). Transform = " & ToString())
			End If
			Dim n As Integer = writables.Count
			Dim [out] As IList(Of Writable) = New List(Of Writable)(n)

			Dim i As Integer = 0
			For Each w As Writable In writables
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (i++ == columnIdx)
				If i = columnIdx Then
						i += 1
					Dim text As String = w.ToString()
					Dim indices As ICollection(Of Integer) = getIndices(text)
					Dim counts As INDArray = makeBOWNDArray(indices)
					[out].Add(New NDArrayWritable(counts))
				Else
						i += 1
					'No change to this column
					[out].Add(w)
				End If
			Next w

			Return [out]
		End Function

		''' <summary>
		''' Transform an object
		''' in to another object
		''' </summary>
		''' <param name="input"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Public Overrides Function map(ByVal input As Object) As Object
			Return Nothing
		End Function

		''' <summary>
		''' Transform a sequence
		''' </summary>
		''' <param name="sequence"> </param>
		Public Overrides Function mapSequence(ByVal sequence As Object) As Object
			Return Nothing
		End Function

		''' <summary>
		''' The output column name
		''' after the operation has been applied
		''' </summary>
		''' <returns> the output column name </returns>
		Public Overrides Function outputColumnName() As String
			Throw New System.NotSupportedException("New column names is always more than 1 in length")
		End Function

		''' <summary>
		''' The output column names
		''' This will often be the same as the input
		''' </summary>
		''' <returns> the output column names </returns>
		Public Overrides Function outputColumnNames() As String()
			Return CType(vocabulary, List(Of String)).ToArray()
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overrides Function columnNames() As String()
			Return New String() {columnName()}
		End Function

		''' <summary>
		''' Returns a singular column name
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overrides Function columnName() As String
			Return columnName_Conflict
		End Function
	End Class

End Namespace