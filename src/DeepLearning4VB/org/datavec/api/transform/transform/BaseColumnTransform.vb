Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms

    ''' <summary>
    ''' Base class of the transforms that replace the values of a single column (keeping its position): subclasses
    ''' map one value with <c>map(Writable)</c> and describe the new column with <see cref="getNewColumnMetaData"/>.
    ''' Equality compares the class and the column name; subclasses add their own fields.
    ''' </summary>
    ' Java: @Data @JsonIgnoreProperties({"inputSchema", "columnNumber"}) @NoArgsConstructor
    <JsonIgnoreProperties("inputSchema", "columnNumber")>
    <Serializable>
    Public MustInherit Class BaseColumnTransform
        Inherits BaseTransform

        Protected Friend columnNameField As String
        Protected Friend columnNumber As Integer = -1

        ''' <summary>Lombok <c>@NoArgsConstructor</c> (for JSON deserialization).</summary>
        Protected Sub New()
        End Sub

        Public Sub New(ByVal columnName As String)
            Me.columnNameField = columnName
        End Sub

        <JsonIgnore>
        Public Overrides Property InputSchema As SchemaT
            Get
                Return inputSchemaField
            End Get
            Set(ByVal value As SchemaT)
                Me.inputSchemaField = value
                columnNumber = value.getIndexOfColumn(columnNameField)
            End Set
        End Property

        ''' <summary>Java <c>getColumnName()</c> (Lombok).</summary>
        Public Function getColumnName() As String
            Return columnNameField
        End Function

        ''' <summary>Java <c>setColumnName()</c> (Lombok).</summary>
        Public Sub setColumnName(ByVal columnName As String)
            Me.columnNameField = columnName
        End Sub

        ''' <summary>Java <c>getColumnNumber()</c> (Lombok).</summary>
        Public Function getColumnNumber() As Integer
            Return columnNumber
        End Function

        Public Overrides Function transform(ByVal schema As SchemaT) As SchemaT
            If columnNumber = -1 Then
                Throw New InvalidOperationException("columnNumber == -1 -> setInputSchema not called?")
            End If
            Dim oldMeta As IList(Of ColumnMetaData) = schema.getColumnMetaData()
            Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(oldMeta.Count)

            Dim i As Integer = 0
            For Each t As ColumnMetaData In oldMeta
                If i = columnNumber Then
                    newMeta.Add(getNewColumnMetaData(t.Name, t))
                Else
                    newMeta.Add(t)
                End If
                i += 1
            Next t

            Return schema.newSchema(newMeta)
        End Function

        Public MustOverride Function getNewColumnMetaData(ByVal newName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData

        Public Overrides Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT)
            If writables.Count <> inputSchemaField.numColumns() Then
                Throw New InvalidOperationException(badRecordLengthMessage(writables.Count, inputSchemaField.numColumns(), Me))
            End If
            Dim n As Integer = writables.Count
            Dim [out] As IList(Of WritableT) = New List(Of WritableT)(n)

            Dim i As Integer = 0
            For Each w As WritableT In writables
                If i = columnNumber Then
                    [out].Add(map(w))
                Else
                    [out].Add(w)
                End If
                i += 1
            Next w

            Return [out]
        End Function

        ''' <summary>
        ''' The output column name
        ''' after the operation has been applied
        ''' </summary>
        ''' <returns> the output column name </returns>
        Public Overrides Function outputColumnName() As String
            Return outputColumnNames()(0)
        End Function

        ''' <summary>
        ''' The output column names
        ''' This will often be the same as the input
        ''' </summary>
        ''' <returns> the output column names </returns>
        Public Overrides Function outputColumnNames() As String()
            Return New String() {columnNameField}
        End Function

        ''' <summary>
        ''' Returns column names
        ''' this op is meant to run on
        ''' </summary>
        Public Overrides Function columnNames() As String()
            Return New String() {columnNameField}
        End Function

        ''' <summary>
        ''' Returns a singular column name
        ''' this op is meant to run on
        ''' </summary>
        Public Overrides Function columnName() As String
            Return columnNames()(0)
        End Function

        ''' <summary>Maps the value of the column.</summary>
        Public MustOverride Overloads Function map(ByVal columnWritable As WritableT) As WritableT

        Public MustOverride Overrides Function ToString() As String

        ''' <summary>Equal when the other object is of the same class and has the same column name.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then
                Return True
            End If
            If o Is Nothing OrElse Me.GetType() IsNot o.GetType() Then
                Return False
            End If
            Dim o2 As BaseColumnTransform = DirectCast(o, BaseColumnTransform)
            Return String.Equals(columnNameField, o2.columnNameField)
        End Function

        ''' <summary>
        ''' Transform a sequence
        ''' </summary>
        ''' <param name="sequence"> </param>
        Public Overrides Function mapSequence(ByVal sequence As Object) As Object
            Dim list As System.Collections.IList = DirectCast(sequence, System.Collections.IList)
            Dim ret As IList(Of Object) = New List(Of Object)()
            For Each o As Object In list
                ret.Add(map(o))
            Next o
            Return ret
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return If(columnNameField Is Nothing, 0, columnNameField.GetHashCode())
        End Function

    End Class

End Namespace
