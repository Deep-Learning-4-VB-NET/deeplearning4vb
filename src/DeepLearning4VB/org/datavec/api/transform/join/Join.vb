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
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports NullWritable = DeepLearning4VB.org.datavec.api.writable.NullWritable
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.join

    ''' <summary>
    ''' Join class: used to specify a join (like an SQL join) of two data sets on one or more key columns.
    ''' This class describes the join (output schema) and joins single pairs of examples
    ''' (<see cref="joinExamples"/>); grouping the examples by key is done by the executor.
    ''' Built with <c>New Join.Builder(Join.JoinType.Inner).setJoinColumns("key").setSchemas(left, right).build()</c>.
    ''' Equality (Lombok <c>@Data</c>) compares the join type, both schemas and the join columns.
    ''' </summary>
    <Serializable>
    Public Class Join

        ''' <summary>
        ''' Type of join<br/>
        ''' Inner: Return examples where the join column values occur in both
        ''' LeftOuter: Return all examples from left data, whether there is a matching right value or not.
        ''' (If not: right values will have NullWritable instead)<br/>
        ''' RightOuter: Return all examples from the right data, whether there is a matching left value or not.
        ''' (If not: left values will have NullWritable instead)<br/>
        ''' FullOuter: return all examples from both left/right, whether there is a matching value from the other side or not.
        ''' (If not: other values will have NullWritable instead)
        ''' Serializes by name; <see cref="JoinTypeExtensions.JoinTypeValueOf"/> is Java's <c>JoinType.valueOf</c>.
        ''' </summary>
        <JsonConverter(GetType(JsonStringEnumConverter))>
        Public Enum JoinType
            Inner
            LeftOuter
            RightOuter
            FullOuter
        End Enum

        Private joinTypeField As JoinType
        Private leftSchema As SchemaT
        Private rightSchema As SchemaT
        Private joinColumnsLeft() As String
        Private joinColumnsRight() As String

        Private Sub New(ByVal builder As Builder)
            Me.joinTypeField = builder.joinTypeField
            Me.leftSchema = builder.leftSchema
            Me.rightSchema = builder.rightSchema
            Me.joinColumnsLeft = builder.joinColumnsLeftField
            Me.joinColumnsRight = builder.joinColumnsRightField

            'Perform validation: ensure columns are correct, etc
            '(The Java null check of the join type is not needed: a VB enum cannot be null.)
            If leftSchema Is Nothing Then
                Throw New ArgumentException("Left schema cannot be null")
            End If
            If rightSchema Is Nothing Then
                Throw New ArgumentException("Right schema cannot be null")
            End If
            If joinColumnsLeft Is Nothing OrElse joinColumnsLeft.Length = 0 Then
                Throw New ArgumentException("Invalid left join columns: " & If(joinColumnsLeft Is Nothing, "null", Arrays.toString(joinColumnsLeft)))
            End If
            If joinColumnsRight Is Nothing OrElse joinColumnsRight.Length = 0 Then
                Throw New ArgumentException("Invalid right join columns: " & If(joinColumnsRight Is Nothing, "null", Arrays.toString(joinColumnsRight)))
            End If

            'Check that the join columns actually appear in the schemas:
            For Each leftCol As String In joinColumnsLeft
                If Not leftSchema.hasColumn(leftCol) Then
                    Throw New ArgumentException("Cannot perform join: left join column """ & leftCol & """ does not exist in left schema. All columns in left schema: " & namesToString(leftSchema.getColumnNames()))
                End If
            Next leftCol

            For Each rightCol As String In joinColumnsRight
                If Not rightSchema.hasColumn(rightCol) Then
                    Throw New ArgumentException("Cannot perform join: right join column """ & rightCol & """ does not exist in right schema. All columns in right schema: " & namesToString(rightSchema.getColumnNames()))
                End If
            Next rightCol
        End Sub

        ''' <summary>Java <c>getJoinType()</c> (Lombok).</summary>
        Public Function getJoinType() As JoinType
            Return joinTypeField
        End Function

        ''' <summary>Java <c>setJoinType()</c> (Lombok).</summary>
        Public Sub setJoinType(ByVal joinType As JoinType)
            Me.joinTypeField = joinType
        End Sub

        ''' <summary>Java <c>getLeftSchema()</c> (Lombok).</summary>
        Public Function getLeftSchema() As SchemaT
            Return leftSchema
        End Function

        ''' <summary>Java <c>setLeftSchema()</c> (Lombok).</summary>
        Public Sub setLeftSchema(ByVal leftSchema As SchemaT)
            Me.leftSchema = leftSchema
        End Sub

        ''' <summary>Java <c>getRightSchema()</c> (Lombok).</summary>
        Public Function getRightSchema() As SchemaT
            Return rightSchema
        End Function

        ''' <summary>Java <c>setRightSchema()</c> (Lombok).</summary>
        Public Sub setRightSchema(ByVal rightSchema As SchemaT)
            Me.rightSchema = rightSchema
        End Sub

        ''' <summary>Java <c>getJoinColumnsLeft()</c> (Lombok).</summary>
        Public Function getJoinColumnsLeft() As String()
            Return joinColumnsLeft
        End Function

        ''' <summary>Java <c>setJoinColumnsLeft()</c> (Lombok).</summary>
        Public Sub setJoinColumnsLeft(ByVal joinColumnsLeft() As String)
            Me.joinColumnsLeft = joinColumnsLeft
        End Sub

        ''' <summary>Java <c>getJoinColumnsRight()</c> (Lombok).</summary>
        Public Function getJoinColumnsRight() As String()
            Return joinColumnsRight
        End Function

        ''' <summary>Java <c>setJoinColumnsRight()</c> (Lombok).</summary>
        Public Sub setJoinColumnsRight(ByVal joinColumnsRight() As String)
            Me.joinColumnsRight = joinColumnsRight
        End Sub

        ''' <summary>Fluent builder for a <see cref="Join"/>.</summary>
        Public Class Builder

            Friend joinTypeField As JoinType
            Friend leftSchema As SchemaT
            Friend rightSchema As SchemaT
            Friend joinColumnsLeftField() As String
            Friend joinColumnsRightField() As String

            Public Sub New(ByVal type As JoinType)
                Me.joinTypeField = type
            End Sub

            Public Overridable Function setSchemas(ByVal left As SchemaT, ByVal right As SchemaT) As Builder
                Me.leftSchema = left
                Me.rightSchema = right
                Return Me
            End Function

            ''' @deprecated Use <seealso cref="setJoinColumns(String())"/>
            <Obsolete("Use setJoinColumns(String...)")>
            Public Overridable Function setKeyColumns(ByVal ParamArray keyColumnNames() As String) As Builder
                Return setJoinColumns(keyColumnNames)
            End Function

            ''' @deprecated Use <seealso cref="setJoinColumnsLeft(String())"/>
            <Obsolete("Use setJoinColumnsLeft(String...)")>
            Public Overridable Function setKeyColumnsLeft(ByVal ParamArray keyColumnNames() As String) As Builder
                Return setJoinColumnsLeft(keyColumnNames)
            End Function

            ''' @deprecated Use <seealso cref="setJoinColumnsRight(String())"/>
            <Obsolete("Use setJoinColumnsRight(String...)")>
            Public Overridable Function setKeyColumnsRight(ByVal ParamArray keyColumnNames() As String) As Builder
                Return setJoinColumnsRight(keyColumnNames)
            End Function

            ''' <summary>
            ''' Specify the column(s) to join on.
            ''' Here, we are assuming that both data sources have the same column names. If this is not the case,
            ''' use <seealso cref="setJoinColumnsLeft(String())"/> and <seealso cref="setJoinColumnsRight(String())"/>.
            ''' The idea: join examples where firstDataValues(joinColumNames[i]) == secondDataValues(joinColumnNames[i]) for all i </summary>
            ''' <param name="joinColumnNames">    Name of the columns to use as the key to join on </param>
            Public Overridable Function setJoinColumns(ByVal ParamArray joinColumnNames() As String) As Builder
                setJoinColumnsLeft(joinColumnNames)
                Return setJoinColumnsRight(joinColumnNames)
            End Function

            ''' <summary>
            ''' Specify the names of the columns to join on, for the left data)
            ''' The idea: join examples where firstDataValues(joinColumNamesLeft[i]) == secondDataValues(joinColumNamesRight[i]) for all i </summary>
            ''' <param name="joinColumnNames"> Names of the columns to join on (for left data) </param>
            Public Overridable Function setJoinColumnsLeft(ByVal ParamArray joinColumnNames() As String) As Builder
                Me.joinColumnsLeftField = joinColumnNames
                Return Me
            End Function

            ''' <summary>
            ''' Specify the names of the columns to join on, for the right data)
            ''' The idea: join examples where firstDataValues(joinColumNamesLeft[i]) == secondDataValues(joinColumNamesRight[i]) for all i </summary>
            ''' <param name="joinColumnNames"> Names of the columns to join on (for right data) </param>
            Public Overridable Function setJoinColumnsRight(ByVal ParamArray joinColumnNames() As String) As Builder
                Me.joinColumnsRightField = joinColumnNames
                Return Me
            End Function

            ''' <summary>Builds and validates the join.</summary>
            ''' <exception cref="InvalidOperationException">if a schema is missing</exception>
            ''' <exception cref="ArgumentException">if the join columns are missing or not in the schemas</exception>
            Public Overridable Function build() As Join
                If leftSchema Is Nothing OrElse rightSchema Is Nothing Then
                    Throw New InvalidOperationException("Cannot build Join: left and/or right schemas are null")
                End If
                Return New Join(Me)
            End Function
        End Class

        ''' <summary>
        ''' The output schema: the left schema plus the columns of the right schema that are not join columns.
        ''' </summary>
        Public Overridable ReadOnly Property OutputSchema As SchemaT
            Get
                If leftSchema Is Nothing Then
                    Throw New InvalidOperationException("Left schema is not set (null)")
                End If
                If rightSchema Is Nothing Then
                    Throw New InvalidOperationException("Right schema is not set (null)")
                End If
                If joinColumnsLeft Is Nothing Then
                    Throw New InvalidOperationException("Left key columns are not set (null)")
                End If
                If joinColumnsRight Is Nothing Then
                    Throw New ArgumentException("Right key columns are not set (null")
                End If

                'Approach here: take the left schema, plus the right schema (excluding the key columns from the right schema)
                Dim metaDataOut As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(leftSchema.getColumnMetaData())

                Dim keySetRight As ISet(Of String) = New HashSet(Of String)(joinColumnsRight)

                For Each rightMeta As ColumnMetaData In rightSchema.getColumnMetaData()
                    If keySetRight.Contains(rightMeta.Name) Then
                        Continue For
                    End If
                    metaDataOut.Add(rightMeta)
                Next rightMeta

                Return leftSchema.newSchema(metaDataOut)
            End Get
        End Property

        ''' <summary>Java <c>getOutputSchema()</c>.</summary>
        Public Function getOutputSchema() As SchemaT
            Return OutputSchema
        End Function

        ''' <summary>
        ''' Join the examples.
        ''' Note: left or right examples may be null; if they are null, the appropriate NullWritables are inserted.
        ''' When the left example is null, the values of its key columns are taken from the matching key columns of the
        ''' right example (the i-th left join column from the i-th right join column; the Java code always used the
        ''' first right join column).
        ''' </summary>
        ''' <param name="leftExample">  the left example (may be Nothing) </param>
        ''' <param name="rightExample"> the right example (may be Nothing) </param>
        ''' <exception cref="ArgumentException">if both examples are Nothing</exception>
        Public Overridable Function joinExamples(ByVal leftExample As IList(Of WritableT), ByVal rightExample As IList(Of WritableT)) As IList(Of WritableT)

            Dim [out] As New List(Of WritableT)()
            If leftExample Is Nothing Then
                If rightExample Is Nothing Then
                    Throw New ArgumentException("Cannot join examples: Both examples are null (max 1 allowed to be null)")
                End If

                'Insert a set of null writables...
                'Complication here: the **key values** should still exist (we have to extract them from second value)
                Dim nLeft As Integer = leftSchema.numColumns()
                Dim leftNames As IList(Of String) = leftSchema.getColumnNames()
                For i As Integer = 0 To nLeft - 1
                    Dim name As String = leftNames(i)
                    Dim keyIdx As Integer = Array.IndexOf(joinColumnsLeft, name)
                    If keyIdx >= 0 Then
                        'This would normally be where the left key came from...
                        'So let's get the key value from the *right* example
                        Dim rightKeyName As String = joinColumnsRight(keyIdx)
                        Dim idxOfRightKey As Integer = rightSchema.getIndexOfColumn(rightKeyName)
                        [out].Add(rightExample(idxOfRightKey))
                    Else
                        'Not a key column, so just add a NullWritable
                        [out].Add(NullWritable.INSTANCE)
                    End If
                Next i
            Else
                [out].AddRange(leftExample)
            End If

            Dim rightNames As IList(Of String) = rightSchema.getColumnNames()
            If rightExample Is Nothing Then
                'Insert a set of null writables...
                Dim nRight As Integer = rightSchema.numColumns()
                For i As Integer = 0 To nRight - 1
                    Dim name As String = rightNames(i)
                    If Array.IndexOf(joinColumnsRight, name) >= 0 Then
                        Continue For 'Skip the key column value
                    End If
                    [out].Add(NullWritable.INSTANCE)
                Next i
            Else
                'Add all values from right, except for key columns...
                For i As Integer = 0 To rightExample.Count - 1
                    Dim name As String = rightNames(i)
                    If Array.IndexOf(joinColumnsRight, name) >= 0 Then
                        Continue For 'Skip the key column value
                    End If
                    [out].Add(rightExample(i))
                Next i
            End If

            Return [out]
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim other As Join = DirectCast(obj, Join)
            Return joinTypeField = other.joinTypeField AndAlso Object.Equals(leftSchema, other.leftSchema) AndAlso Object.Equals(rightSchema, other.rightSchema) AndAlso
                Arrays.equals(joinColumnsLeft, other.joinColumnsLeft) AndAlso Arrays.equals(joinColumnsRight, other.joinColumnsRight)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(joinTypeField, leftSchema, rightSchema, Arrays.hashCode(joinColumnsLeft), Arrays.hashCode(joinColumnsRight))
        End Function

        Public Overrides Function ToString() As String
            Return "Join(joinType=" & joinTypeField.ToString() & ", leftSchema=" & Objects.toString(leftSchema) & ", rightSchema=" & Objects.toString(rightSchema) &
                ", joinColumnsLeft=" & Arrays.toString(joinColumnsLeft) & ", joinColumnsRight=" & Arrays.toString(joinColumnsRight) & ")"
        End Function

        ''' <summary>Java <c>List.toString</c> of column names: <c>[a, b]</c>.</summary>
        Private Shared Function namesToString(ByVal names As IList(Of String)) As String
            Return "[" & String.Join(", ", names) & "]"
        End Function

    End Class

    ''' <summary>The Java static methods of the <see cref="Join.JoinType"/> enum.</summary>
    Public Module JoinTypeExtensions

        ''' <summary>Java <c>JoinType.values()</c>.</summary>
        Public Function JoinTypeValues() As Join.JoinType()
            Return DirectCast([Enum].GetValues(GetType(Join.JoinType)), Join.JoinType())
        End Function

        ''' <summary>Parses the enum constant name (Java <c>JoinType.valueOf</c>).</summary>
        ''' <exception cref="ArgumentException">for an unknown name (including numeric strings)</exception>
        Public Function JoinTypeValueOf(ByVal name As String) As Join.JoinType
            Dim result As Join.JoinType
            ' Enum.TryParse also accepts numeric strings ("1"); Java valueOf only accepts the constant names
            If name IsNot Nothing AndAlso name.Length > 0 AndAlso Char.IsLetter(name(0)) AndAlso [Enum].TryParse(name, False, result) AndAlso [Enum].IsDefined(GetType(Join.JoinType), result) Then
                Return result
            End If
            Throw New ArgumentException("No enum constant org.datavec.api.transform.join.Join.JoinType." & name)
        End Function

    End Module

End Namespace
