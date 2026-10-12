Imports System
Imports System.Collections.Generic
Imports lombok
Imports Transform = org.datavec.api.transform.Transform
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude

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

Namespace org.datavec.api.transform.sequence.expansion


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(exclude = {"inputSchema"}) @JsonIgnoreProperties({"inputSchema"}) @JsonInclude(JsonInclude.Include.NON_NULL) @Data public abstract class BaseSequenceExpansionTransform implements org.datavec.api.transform.Transform
	<Serializable>
	Public MustInherit Class BaseSequenceExpansionTransform
		Implements Transform

		Public MustOverride Property InputSchema As Schema

		Protected Friend requiredColumns As IList(Of String)
		Protected Friend expandedColumnNames As IList(Of String)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter @Getter protected org.datavec.api.transform.schema.Schema inputSchema;
		Protected Friend inputSchema As Schema

		''' <param name="requiredColumns">      Input columns, to be expanded </param>
		''' <param name="expandedColumnNames">  Names of the columns after expansion </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected BaseSequenceExpansionTransform(@NonNull List<String> requiredColumns, @NonNull List<String> expandedColumnNames)
		Protected Friend Sub New(ByVal requiredColumns As IList(Of String), ByVal expandedColumnNames As IList(Of String))
			If requiredColumns.Count = 0 Then
				Throw New System.ArgumentException("No columns have values to be expanded. Must have requiredColumns.size() > 0")
			End If
			Me.requiredColumns = requiredColumns
			Me.expandedColumnNames = expandedColumnNames
		End Sub

		Protected Friend MustOverride Function expandedColumnMetaDatas(ByVal origColumnMeta As IList(Of ColumnMetaData), ByVal expandedColumnNames As IList(Of String)) As IList(Of ColumnMetaData)

		Protected Friend MustOverride Function expandTimeStep(ByVal currentStepValues As IList(Of Writable)) As IList(Of IList(Of Writable))


		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			'Same schema *except* for the expanded columns

			Dim meta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(inputSchema.numColumns())

			Dim oldMetaToExpand As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)()
			For Each s As String In requiredColumns
				oldMetaToExpand.Add(inputSchema.getMetaData(s))
			Next s
			Dim newMetaToExpand As IList(Of ColumnMetaData) = expandedColumnMetaDatas(oldMetaToExpand, expandedColumnNames)

			Dim modColumnIdx As Integer = 0
			For Each m As ColumnMetaData In inputSchema.getColumnMetaData()

				If requiredColumns.Contains(m.Name) Then
					'Possibly changed column (expanded)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: meta.add(newMetaToExpand.get(modColumnIdx++));
					meta.Add(newMetaToExpand(modColumnIdx))
						modColumnIdx += 1
				Else
					'Unmodified column
					meta.Add(m)
				End If
			Next m

			Return inputSchema.newSchema(meta)
		End Function

		Public Overridable Function outputColumnName() As String
			Return expandedColumnNames(0)
		End Function

		Public Overridable Function outputColumnNames() As String()
			Return CType(expandedColumnNames, List(Of String)).ToArray()
		End Function

		Public Overridable Function columnNames() As String()
			Return CType(requiredColumns, List(Of String)).ToArray()
		End Function

		Public Overridable Function columnName() As String
			Return columnNames()(0)
		End Function

		Public Overridable Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable)
			Throw New System.NotSupportedException("Cannot perform sequence expansion on non-sequence data")
		End Function

		Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable))

			Dim nCols As Integer = inputSchema.numColumns()
			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()

			Dim expandColumnIdxsMap As IDictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)() 'Map from "position in vector idx" to "required column idx"
			Dim expandColumnIdxs(requiredColumns.Count - 1) As Integer
			Dim count As Integer=0
			For Each s As String In requiredColumns
				Dim idx As Integer = inputSchema.getIndexOfColumn(s)
				expandColumnIdxsMap(idx) = count
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: expandColumnIdxs[count++] = idx;
				expandColumnIdxs(count) = idx
					count += 1
			Next s

			Dim toExpand As IList(Of Writable) = New List(Of Writable)(requiredColumns.Count)
			For Each [step] As IList(Of Writable) In sequence
				toExpand.Clear()

				For Each i As Integer In expandColumnIdxs
					toExpand.Add([step](i))
				Next i

				Dim expanded As IList(Of IList(Of Writable)) = expandTimeStep(toExpand)

				'Now: for each expanded step, copy the original values out
				Dim expansionSize As Integer = expanded.Count
				For i As Integer = 0 To expansionSize - 1
					Dim newStep As IList(Of Writable) = New List(Of Writable)(nCols)
					For j As Integer = 0 To nCols - 1
						If expandColumnIdxsMap.ContainsKey(j) Then
							'This is one of the expanded columns
							Dim expandIdx As Integer = expandColumnIdxsMap(j)
							newStep.Add(expanded(i)(expandIdx))
						Else
							'Copy existing  value
							newStep.Add([step](j))
						End If
					Next j

					[out].Add(newStep)
				Next i
			Next [step]

			Return [out]
		End Function

		Public Overridable Function map(ByVal input As Object) As Object Implements Transform.map
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements Transform.mapSequence
			Throw New System.NotSupportedException("Not yet implemented")
		End Function
	End Class

End Namespace