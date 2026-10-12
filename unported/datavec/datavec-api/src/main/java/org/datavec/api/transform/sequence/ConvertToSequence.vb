Imports System.Collections.Generic
Imports System.Linq
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSchema = org.datavec.api.transform.schema.SequenceSchema
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

Namespace org.datavec.api.transform.sequence



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(exclude = {"inputSchema"}) @JsonIgnoreProperties({"inputSchema"}) public class ConvertToSequence
	Public Class ConvertToSequence

		Private singleStepSequencesMode As Boolean
		Private ReadOnly keyColumns() As String
		Private ReadOnly comparator As SequenceComparator 'For sorting values within collected (unsorted) sequence
'JAVA TO VB CONVERTER NOTE: The field inputSchema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputSchema_Conflict As Schema

		''' 
		''' <param name="keyColumn"> The value to use as the key for inferring which examples belong to what sequence </param>
		''' <param name="comparator"> The comparator to use when deciding the order of each possible time step in the sequence </param>
		Public Sub New(ByVal keyColumn As String, ByVal comparator As SequenceComparator)
			Me.New(False, New String(){keyColumn}, comparator)
		End Sub

		''' 
		''' <param name="keyColumns"> The value or values to use as the key (multiple values: compound key)  for inferring which
		'''                   examples belong to what sequence </param>
		''' <param name="comparator"> The comparator to use when deciding the order of each possible time step in the sequence </param>
		Public Sub New(ByVal keyColumns As ICollection(Of String), ByVal comparator As SequenceComparator)
			Me.New(False, keyColumns.ToArray(), comparator)
		End Sub

		''' 
		''' <param name="keyColumns"> The value or values to use as the key (multiple values: compound key)  for inferring which
		'''                   examples belong to what sequence </param>
		''' <param name="comparator"> The comparator to use when deciding the order of each possible time step in the sequence </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ConvertToSequence(@JsonProperty("singleStepSequencesMode") boolean singleStepSequencesMode, @JsonProperty("keyColumn") String[] keyColumns, @JsonProperty("comparator") SequenceComparator comparator)
		Public Sub New(ByVal singleStepSequencesMode As Boolean, ByVal keyColumns() As String, ByVal comparator As SequenceComparator)
			Me.singleStepSequencesMode = singleStepSequencesMode
			Me.keyColumns = keyColumns
			Me.comparator = comparator
		End Sub

		Public Overridable Function transform(ByVal schema As Schema) As SequenceSchema
			Return New SequenceSchema(schema.getColumnMetaData())
		End Function

		Public Overridable WriteOnly Property InputSchema As Schema
			Set(ByVal schema As Schema)
				Me.inputSchema_Conflict = schema
				If Not singleStepSequencesMode Then
					comparator.Schema = transform(schema)
				End If
			End Set
		End Property

		Public Overrides Function ToString() As String
			If singleStepSequencesMode Then
				Return "ConvertToSequence()"
			ElseIf keyColumns.Length = 1 Then
				Return "ConvertToSequence(keyColumn=""" & keyColumns(0) & """,comparator=" & comparator & ")"
			Else
				Return "ConvertToSequence(keyColumns=""" & Arrays.toString(keyColumns) & """,comparator=" & comparator & ")"
			End If
		End Function

	End Class

End Namespace