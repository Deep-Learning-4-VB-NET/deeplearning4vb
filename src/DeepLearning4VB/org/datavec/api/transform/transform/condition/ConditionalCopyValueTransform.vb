Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports ConditionT = DeepLearning4VB.org.datavec.api.transform.condition.Condition
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.condition

    ''' <summary>
    ''' Replace the value in a specified column with a new value taken from another column, if a condition is satisfied/true.<br/>
    ''' Note that the condition can be any generic condition, including on other column(s), different to the column
    ''' that will be modified.
    ''' <para>Equality (Lombok, excluding the column indexes) compares the class, the two columns and the condition.</para>
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"columnToReplaceIdx", "sourceColumnIdx"})
    '       @EqualsAndHashCode(exclude = {"columnToReplaceIdx", "sourceColumnIdx"}) @Data
    <JsonIgnoreProperties("columnToReplaceIdx", "sourceColumnIdx")>
    <Serializable>
    Public Class ConditionalCopyValueTransform
        Implements TransformT

        Private ReadOnly columnToReplace As String
        Private ReadOnly sourceColumn As String
        Private ReadOnly condition As ConditionT
        Private columnToReplaceIdx As Integer = -1
        Private sourceColumnIdx As Integer = -1

        ''' <param name="columnToReplace"> Name of the column in which to replace the old value </param>
        ''' <param name="sourceColumn">    Name of the column to get the new value from </param>
        ''' <param name="condition">       Condition </param>
        <JsonConstructor>
        Public Sub New(ByVal columnToReplace As String, ByVal sourceColumn As String, ByVal condition As ConditionT)
            Me.columnToReplace = columnToReplace
            Me.sourceColumn = sourceColumn
            Me.condition = condition
        End Sub

        ''' <summary>Java <c>getColumnToReplace()</c> (Lombok).</summary>
        Public Function getColumnToReplace() As String
            Return columnToReplace
        End Function

        ''' <summary>Java <c>getSourceColumn()</c> (Lombok).</summary>
        Public Function getSourceColumn() As String
            Return sourceColumn
        End Function

        ''' <summary>Java <c>getCondition()</c> (Lombok).</summary>
        Public Function getCondition() As ConditionT
            Return condition
        End Function

        ''' <summary>Java <c>getColumnToReplaceIdx()</c> (Lombok).</summary>
        Public Function getColumnToReplaceIdx() As Integer
            Return columnToReplaceIdx
        End Function

        ''' <summary>Java <c>getSourceColumnIdx()</c> (Lombok).</summary>
        Public Function getSourceColumnIdx() As Integer
            Return sourceColumnIdx
        End Function

        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            'Conditional copy should not change any of the metadata, under normal usage
            Return inputSchema
        End Function

        ''' <summary>The input schema is held by the condition.</summary>
        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Set(ByVal value As SchemaT)
                If Not value.hasColumn(columnToReplace) Then
                    Throw New InvalidOperationException("Column """ & columnToReplace & """ not found in input schema")
                End If
                If Not value.hasColumn(sourceColumn) Then
                    Throw New InvalidOperationException("Column """ & sourceColumn & """ not found in input schema")
                End If
                columnToReplaceIdx = value.getIndexOfColumn(columnToReplace)
                sourceColumnIdx = value.getIndexOfColumn(sourceColumn)
                condition.InputSchema = value
            End Set
            Get
                Return condition.InputSchema
            End Get
        End Property

        Public Overridable Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT) Implements TransformT.map
            If condition.condition(writables) Then
                'Condition holds -> set new value
                Dim newList As IList(Of WritableT) = New List(Of WritableT)(writables)
                newList(columnToReplaceIdx) = writables(sourceColumnIdx)
                Return newList
            Else
                'Condition does not hold -> no change
                Return writables
            End If
        End Function

        Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of WritableT)) Implements TransformT.mapSequence
            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            For Each [step] As IList(Of WritableT) In sequence
                [out].Add(map([step]))
            Next [step]
            Return [out]
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object (as in Java: not supported, returns <c>Nothing</c>)
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overridable Function map(ByVal input As Object) As Object Implements TransformT.map
            Return Nothing
        End Function

        ''' <summary>
        ''' Transform a sequence
        ''' </summary>
        ''' <param name="sequence"> </param>
        Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements TransformT.mapSequence
            Dim seq As System.Collections.IList = DirectCast(sequence, System.Collections.IList)
            Dim ret As IList(Of Object) = New List(Of Object)()
            For Each [step] As Object In seq
                ret.Add(map([step]))
            Next [step]
            Return ret
        End Function

        Public Overrides Function ToString() As String
            Return "ConditionalCopyValueTransform(replaceColumn=""" & columnToReplace & """,sourceColumn=" & sourceColumn & ",condition=" & javaToString(condition) & ")"
        End Function

        ''' <summary>
        ''' The output column name
        ''' after the operation has been applied
        ''' </summary>
        ''' <returns> the output column name </returns>
        Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
            Return columnToReplace
        End Function

        ''' <summary>
        ''' The output column names
        ''' This will often be the same as the input
        ''' </summary>
        ''' <returns> the output column names </returns>
        Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
            Return New String() {columnToReplace}
        End Function

        ''' <summary>
        ''' Returns column names
        ''' this op is meant to run on
        ''' </summary>
        Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
            Return New String() {columnName()}
        End Function

        ''' <summary>
        ''' Returns a singular column name
        ''' this op is meant to run on
        ''' </summary>
        Public Overridable Function columnName() As String Implements ColumnOp.columnName
            Return sourceColumn
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim o As ConditionalCopyValueTransform = DirectCast(obj, ConditionalCopyValueTransform)
            Return String.Equals(columnToReplace, o.columnToReplace) AndAlso String.Equals(sourceColumn, o.sourceColumn) AndAlso Objects.equals(condition, o.condition)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(columnToReplace, sourceColumn, condition)
        End Function

    End Class

End Namespace
