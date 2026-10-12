Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports Transform = org.datavec.api.transform.Transform
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
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

Namespace org.datavec.api.transform.transform.sequence


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"inputSchema", "columnsToOffsetSet"}) @JsonInclude(JsonInclude.Include.NON_NULL) @Data @EqualsAndHashCode(exclude = {"columnsToOffsetSet", "inputSchema"}) public class SequenceOffsetTransform implements org.datavec.api.transform.Transform
	<Serializable>
	Public Class SequenceOffsetTransform
		Implements Transform

		Public Enum OperationType
			InPlace
			NewColumn
		End Enum

		Public Enum EdgeHandling
			TrimSequence
			SpecifiedValue
		End Enum

		Private columnsToOffset As IList(Of String)
		Private offsetAmount As Integer
		Private operationType As OperationType
		Private edgeHandling As EdgeHandling
		Private edgeCaseValue As Writable

		Private columnsToOffsetSet As ISet(Of String)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.datavec.api.transform.schema.Schema inputSchema;
'JAVA TO VB CONVERTER NOTE: The field inputSchema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputSchema_Conflict As Schema

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SequenceOffsetTransform(@JsonProperty("columnsToOffset") List<String> columnsToOffset, @JsonProperty("offsetAmount") int offsetAmount, @JsonProperty("operationType") OperationType operationType, @JsonProperty("edgeHandling") EdgeHandling edgeHandling, @JsonProperty("edgeCaseValue") org.datavec.api.writable.Writable edgeCaseValue)
		Public Sub New(ByVal columnsToOffset As IList(Of String), ByVal offsetAmount As Integer, ByVal operationType As OperationType, ByVal edgeHandling As EdgeHandling, ByVal edgeCaseValue As Writable)
			If edgeCaseValue IsNot Nothing AndAlso edgeHandling <> EdgeHandling.SpecifiedValue Then
				Throw New System.NotSupportedException("edgeCaseValue was non-null, but EdgeHandling was not set to SpecifiedValue. " & "edgeCaseValue can only be used with SpecifiedValue mode")
			End If

			Me.columnsToOffset = columnsToOffset
			Me.offsetAmount = offsetAmount
			Me.operationType = operationType
			Me.edgeHandling = edgeHandling
			Me.edgeCaseValue = edgeCaseValue

			Me.columnsToOffsetSet = New HashSet(Of String)(columnsToOffset)
		End Sub

		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			For Each s As String In columnsToOffset
				If Not inputSchema.hasColumn(s) Then
					Throw New System.InvalidOperationException("Column """ & s & """ is not found in input schema")
				End If
			Next s

			Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)()
			For Each m As ColumnMetaData In inputSchema.getColumnMetaData()
				If columnsToOffsetSet.Contains(m.Name) Then
					If operationType = OperationType.InPlace Then
						'Only change is to the name
						Dim mNew As ColumnMetaData = m.clone()
						mNew.Name = getNewColumnName(m)
					Else
						'Original is unmodified, new column is added
						newMeta.Add(m)
						Dim mNew As ColumnMetaData = m.clone()
						mNew.Name = getNewColumnName(m)
						newMeta.Add(mNew)
					End If
				Else
					'No change to this column
					newMeta.Add(m)
				End If
			Next m

			Return inputSchema.newSchema(newMeta)
		End Function

		Private Function getNewColumnName(ByVal m As ColumnMetaData) As String
			Return "sequenceOffset(" & offsetAmount & "," & m.Name & ")"
		End Function

		Public Overridable WriteOnly Property InputSchema As Schema
			Set(ByVal inputSchema As Schema)
				Me.inputSchema_Conflict = inputSchema
			End Set
		End Property

		Public Overridable Function outputColumnName() As String
			Return outputColumnNames()(0)
		End Function

		Public Overridable Function outputColumnNames() As String()
			Return CType(inputSchema_Conflict.getColumnNames(), List(Of String)).ToArray()
		End Function

		Public Overridable Function columnNames() As String()
			Return outputColumnNames()
		End Function

		Public Overridable Function columnName() As String
			Return outputColumnName()
		End Function

		Public Overridable Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable)
			Throw New System.NotSupportedException("SequenceOffsetTransform cannot be applied to non-sequence data")
		End Function

		Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable))
			'Edge case
			If offsetAmount >= sequence.Count AndAlso edgeHandling = EdgeHandling.TrimSequence Then
				'No output
				Return java.util.Collections.emptyList()
			End If

			Dim colNames As IList(Of String) = inputSchema_Conflict.getColumnNames()
			Dim nIn As Integer = inputSchema_Conflict.numColumns()
			Dim nOut As Integer = nIn + (If(operationType = OperationType.InPlace, 0, columnsToOffset.Count))

			'Depending on settings, the original sequence might be smaller than the input
			Dim firstOutputStepInclusive As Integer
			Dim lastOutputStepInclusive As Integer
			If edgeHandling = EdgeHandling.TrimSequence Then
				If offsetAmount >= 0 Then
					'Values in the specified columns are shifted later -> trim the start of the sequence
					firstOutputStepInclusive = offsetAmount
					lastOutputStepInclusive = sequence.Count - 1
				Else
					'Values in the specified columns are shifted earlier -> trim the end of the sequence
					firstOutputStepInclusive = 0
					lastOutputStepInclusive = sequence.Count - 1 + offsetAmount
				End If
			Else
				'Specified value -> same output size
				firstOutputStepInclusive = 0
				lastOutputStepInclusive = sequence.Count - 1
			End If

			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			For [step] As Integer = firstOutputStepInclusive To lastOutputStepInclusive
				Dim thisStepIn As IList(Of Writable) = sequence([step]) 'Input for the *non-shifted* values
				Dim thisStepOut As IList(Of Writable) = New List(Of Writable)(nOut)



				For j As Integer = 0 To nIn - 1
					If columnsToOffsetSet.Contains(colNames(j)) Then

						If edgeHandling = EdgeHandling.SpecifiedValue AndAlso [step] - offsetAmount < 0 OrElse [step] - offsetAmount >= sequence.Count Then
							If operationType = OperationType.NewColumn Then
								'Keep the original value
								thisStepOut.Add(thisStepIn(j))
							End If
							thisStepOut.Add(edgeCaseValue)
						Else
							'Trim case, or specified but within range
							Dim shifted As Writable = sequence([step] - offsetAmount)(j)
							If operationType = OperationType.InPlace Then
								'Shift by the specified amount and output
								thisStepOut.Add(shifted)
							Else
								'Add the old value and the new (offset) value
								thisStepOut.Add(thisStepIn(j))
								thisStepOut.Add(shifted)
							End If
						End If
					Else
						'Value is unmodified in this column
						thisStepOut.Add(thisStepIn(j))
					End If
				Next j

				[out].Add(thisStepOut)
			Next [step]

			Return [out]
		End Function

		Public Overridable Function map(ByVal input As Object) As Object Implements Transform.map
			Throw New System.NotSupportedException("SequenceOffsetTransform cannot be applied to non-sequence data")
		End Function

		Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements Transform.mapSequence
			Throw New System.NotSupportedException("Not yet implemented/supported")
		End Function
	End Class

End Namespace