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

Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.sequence.expansion

    ''' <summary>
    ''' A base class for sequence expansion operations.
    ''' The idea: for one or more columns, expand the values in each time step into multiple time steps.
    ''' The other columns are copied into each of the new time steps. For example, a text column may be expanded into
    ''' one time step per character. Subclasses implement <see cref="expandedColumnMetaDatas"/> and
    ''' <see cref="expandTimeStep"/>. Equality (Lombok <c>@EqualsAndHashCode(exclude = {"inputSchema"})</c>) compares
    ''' the class, the required columns and the expanded column names.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema"}) @JsonInclude(NON_NULL)
    <JsonIgnoreProperties("inputSchema")>
    <JsonIncludeNonNull>
    <Serializable>
    Public MustInherit Class BaseSequenceExpansionTransform
        Implements TransformT

        Protected Friend requiredColumns As IList(Of String)
        Protected Friend expandedColumnNames As IList(Of String)
        Protected Friend inputSchemaField As SchemaT

        ''' <param name="requiredColumns">      Input columns, to be expanded </param>
        ''' <param name="expandedColumnNames">  Names of the columns after expansion </param>
        ''' <exception cref="ArgumentNullException">if an argument is Nothing (Lombok <c>@NonNull</c>)</exception>
        ''' <exception cref="ArgumentException">if there are no required columns</exception>
        Protected Friend Sub New(ByVal requiredColumns As IList(Of String), ByVal expandedColumnNames As IList(Of String))
            If requiredColumns Is Nothing Then
                Throw New ArgumentNullException("requiredColumns", "requiredColumns is marked non-null but is null")
            End If
            If expandedColumnNames Is Nothing Then
                Throw New ArgumentNullException("expandedColumnNames", "expandedColumnNames is marked non-null but is null")
            End If
            If requiredColumns.Count = 0 Then
                Throw New ArgumentException("No columns have values to be expanded. Must have requiredColumns.size() > 0")
            End If
            Me.requiredColumns = requiredColumns
            Me.expandedColumnNames = expandedColumnNames
        End Sub

        ''' <summary>Java <c>getRequiredColumns()</c> (Lombok).</summary>
        Public Function getRequiredColumns() As IList(Of String)
            Return requiredColumns
        End Function

        ''' <summary>Java <c>setRequiredColumns()</c> (Lombok).</summary>
        Public Sub setRequiredColumns(ByVal requiredColumns As IList(Of String))
            Me.requiredColumns = requiredColumns
        End Sub

        ''' <summary>Java <c>getExpandedColumnNames()</c> (Lombok).</summary>
        Public Function getExpandedColumnNames() As IList(Of String)
            Return expandedColumnNames
        End Function

        ''' <summary>Java <c>setExpandedColumnNames()</c> (Lombok).</summary>
        Public Sub setExpandedColumnNames(ByVal expandedColumnNames As IList(Of String))
            Me.expandedColumnNames = expandedColumnNames
        End Sub

        ''' <summary>The input schema (Java <c>@Getter @Setter inputSchema</c>).</summary>
        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Get
                Return inputSchemaField
            End Get
            Set(ByVal value As SchemaT)
                inputSchemaField = value
            End Set
        End Property

        ''' <summary>The metadata of the expanded columns, given the metadata of the required (original) columns.</summary>
        Protected Friend MustOverride Function expandedColumnMetaDatas(ByVal origColumnMeta As IList(Of ColumnMetaData), ByVal expandedColumnNames As IList(Of String)) As IList(Of ColumnMetaData)

        ''' <summary>Expands the values of the required columns of one time step into the values of several time steps.</summary>
        Protected Friend MustOverride Function expandTimeStep(ByVal currentStepValues As IList(Of WritableT)) As IList(Of IList(Of WritableT))

        ''' <summary>
        ''' The input schema with each required column replaced by the matching expanded column (the i-th required
        ''' column by the i-th expanded column metadata).
        ''' </summary>
        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            'Same schema *except* for the expanded columns

            Dim meta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(inputSchema.numColumns())

            Dim oldMetaToExpand As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)()
            For Each s As String In requiredColumns
                oldMetaToExpand.Add(inputSchema.getMetaData(s))
            Next s
            Dim newMetaToExpand As IList(Of ColumnMetaData) = expandedColumnMetaDatas(oldMetaToExpand, expandedColumnNames)

            For Each m As ColumnMetaData In inputSchema.getColumnMetaData()

                Dim requiredIdx As Integer = requiredColumns.IndexOf(m.Name)
                If requiredIdx >= 0 Then
                    'Possibly changed column (expanded). The expanded metadata is in the order of the required columns
                    '(the Java code took it in schema order, which disagreed with mapSequence when the orders differ)
                    meta.Add(newMetaToExpand(requiredIdx))
                Else
                    'Unmodified column
                    meta.Add(m)
                End If
            Next m

            Return inputSchema.newSchema(meta)
        End Function

        Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
            Return expandedColumnNames(0)
        End Function

        Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
            Return New List(Of String)(expandedColumnNames).ToArray()
        End Function

        Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
            Return New List(Of String)(requiredColumns).ToArray()
        End Function

        Public Overridable Function columnName() As String Implements ColumnOp.columnName
            Return columnNames()(0)
        End Function

        Public Overridable Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT) Implements TransformT.map
            Throw New NotSupportedException("Cannot perform sequence expansion on non-sequence data")
        End Function

        ''' <summary>
        ''' Expands each time step of the sequence: the values of the required columns are replaced by the expanded
        ''' values (one new time step per expanded value set), the other values are copied.
        ''' </summary>
        Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of WritableT)) Implements TransformT.mapSequence

            Dim nCols As Integer = inputSchemaField.numColumns()
            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            Dim expandColumnIdxsMap As IDictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)() 'Map from "position in vector idx" to "required column idx"
            Dim expandColumnIdxs(requiredColumns.Count - 1) As Integer
            Dim count As Integer = 0
            For Each s As String In requiredColumns
                Dim idx As Integer = inputSchemaField.getIndexOfColumn(s)
                expandColumnIdxsMap(idx) = count
                expandColumnIdxs(count) = idx
                count += 1
            Next s

            Dim toExpand As IList(Of WritableT) = New List(Of WritableT)(requiredColumns.Count)
            For Each [step] As IList(Of WritableT) In sequence
                toExpand.Clear()
                For Each i As Integer In expandColumnIdxs
                    toExpand.Add([step](i))
                Next i

                Dim expanded As IList(Of IList(Of WritableT)) = expandTimeStep(toExpand)

                'Now: for each expanded step, copy the original values out
                Dim expansionSize As Integer = expanded.Count
                For i As Integer = 0 To expansionSize - 1
                    Dim newStep As IList(Of WritableT) = New List(Of WritableT)(nCols)
                    For j As Integer = 0 To nCols - 1
                        Dim expandIdx As Integer
                        If expandColumnIdxsMap.TryGetValue(j, expandIdx) Then
                            'This is one of the expanded columns
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

        Public Overridable Function map(ByVal input As Object) As Object Implements TransformT.map
            Throw New NotSupportedException("Not supported")
        End Function

        Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements TransformT.mapSequence
            Throw New NotSupportedException("Not yet implemented")
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim other As BaseSequenceExpansionTransform = DirectCast(obj, BaseSequenceExpansionTransform)
            Return listEquals(requiredColumns, other.requiredColumns) AndAlso listEquals(expandedColumnNames, other.expandedColumnNames)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return listHash(requiredColumns) * 59 + listHash(expandedColumnNames)
        End Function

        ''' <summary>Java <c>List.equals</c> for string lists (null-safe). Candidate for hoisting.</summary>
        Private Shared Function listEquals(ByVal a As IList(Of String), ByVal b As IList(Of String)) As Boolean
            If a Is Nothing OrElse b Is Nothing Then Return a Is Nothing AndAlso b Is Nothing
            If a.Count <> b.Count Then Return False
            For i As Integer = 0 To a.Count - 1
                If Not String.Equals(a(i), b(i)) Then Return False
            Next i
            Return True
        End Function

        ''' <summary>Java <c>List.hashCode</c> for string lists (0 for Nothing). Candidate for hoisting.</summary>
        Private Shared Function listHash(ByVal a As IList(Of String)) As Integer
            If a Is Nothing Then Return 0
            Dim h As Integer = 1
            For Each s As String In a
                h = 31 * h + Objects.hashCode(s)
            Next s
            Return h
        End Function

    End Class

End Namespace
