Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports CategoricalMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.CategoricalMetaData
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.string

    ''' <summary>
    ''' Convert a delimited String to a list of binary categorical columns.
    ''' Suppose the possible String values were {"a","b","c","d"} and the String column value to be converted contained
    ''' the String "a,c", then the 4 output columns would have values ["true","false","true","false"]
    ''' <para>Equality (Lombok, <c>callSuper = false</c>, excluding columnIdx) compares the class, the column name,
    ''' the new column names, the tokens, the delimiter and the token map.</para>
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema", "map", "columnIdx"})
    '       @EqualsAndHashCode(callSuper = false, exclude = {"columnIdx"}) @Data
    <JsonIgnoreProperties("inputSchema", "map", "columnIdx")>
    <Serializable>
    Public Class StringListToCategoricalSetTransform
        Inherits BaseTransform

        Private ReadOnly columnNameField As String
        Private ReadOnly newColumnNames As IList(Of String)
        Private ReadOnly categoryTokens As IList(Of String)
        Private ReadOnly delimiter As String

        Private ReadOnly mapField As IDictionary(Of String, Integer)

        Private columnIdx As Integer = -1

        ''' <param name="columnName">     The name of the column to convert </param>
        ''' <param name="newColumnNames"> The names of the new columns to create </param>
        ''' <param name="categoryTokens"> The possible tokens that may be present. Note this list must have the same length and order
        '''                       as the newColumnNames list </param>
        ''' <param name="delimiter">      The delimiter for the Strings to convert </param>
        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal newColumnNames As IList(Of String), ByVal categoryTokens As IList(Of String), ByVal delimiter As String)
            If newColumnNames.Count <> categoryTokens.Count Then
                Throw New ArgumentException("Names/tokens sizes cannot differ")
            End If
            Me.columnNameField = columnName
            Me.newColumnNames = newColumnNames
            Me.categoryTokens = categoryTokens
            Me.delimiter = delimiter

            mapField = New Dictionary(Of String, Integer)()
            For i As Integer = 0 To categoryTokens.Count - 1
                mapField(categoryTokens(i)) = i
            Next i
        End Sub

        ''' <summary>Java <c>getColumnName()</c> (Lombok).</summary>
        Public Function getColumnName() As String
            Return columnNameField
        End Function

        ''' <summary>Java <c>getNewColumnNames()</c> (Lombok).</summary>
        Public Function getNewColumnNames() As IList(Of String)
            Return newColumnNames
        End Function

        ''' <summary>Java <c>getCategoryTokens()</c> (Lombok).</summary>
        Public Function getCategoryTokens() As IList(Of String)
            Return categoryTokens
        End Function

        ''' <summary>Java <c>getDelimiter()</c> (Lombok).</summary>
        Public Function getDelimiter() As String
            Return delimiter
        End Function

        ''' <summary>Java <c>getMap()</c> (Lombok).</summary>
        Public Function getMap() As IDictionary(Of String, Integer)
            Return mapField
        End Function

        ''' <summary>Java <c>getColumnIdx()</c> (Lombok).</summary>
        Public Function getColumnIdx() As Integer
            Return columnIdx
        End Function

        Public Overrides Function transform(ByVal inputSchema As SchemaT) As SchemaT
            Dim colIdx As Integer = inputSchema.getIndexOfColumn(columnNameField)

            Dim oldMeta As IList(Of ColumnMetaData) = inputSchema.getColumnMetaData()
            Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(oldMeta.Count + newColumnNames.Count - 1)

            Dim i As Integer = 0
            For Each t As ColumnMetaData In oldMeta
                If i = colIdx Then
                    'Replace String column with a set of binary/categorical columns
                    If t.ColumnType <> ColumnType.String Then
                        Throw New InvalidOperationException("Cannot convert non-string type")
                    End If

                    For j As Integer = 0 To newColumnNames.Count - 1
                        Dim meta As ColumnMetaData = New CategoricalMetaData(newColumnNames(j), "true", "false")
                        newMeta.Add(meta)
                    Next j
                Else
                    newMeta.Add(t)
                End If
                i += 1
            Next t

            Return inputSchema.newSchema(newMeta)
        End Function

        <JsonIgnore>
        Public Overrides Property InputSchema As SchemaT
            Get
                Return inputSchemaField
            End Get
            Set(ByVal value As SchemaT)
                Me.inputSchemaField = value
                Me.columnIdx = value.getIndexOfColumn(columnNameField)
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return "StringListToCategoricalSetTransform(columnName=" & columnNameField & ",newColumnNames=" & javaToString(newColumnNames) & ",categoryTokens=" & javaToString(categoryTokens) & ",delimiter=""" & delimiter & """)"
        End Function

        Public Overrides Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT)
            If writables.Count <> inputSchemaField.numColumns() Then
                Throw New InvalidOperationException(badRecordLengthMessage(writables.Count, inputSchemaField.numColumns(), Me))
            End If
            Dim n As Integer = writables.Count
            Dim [out] As IList(Of WritableT) = New List(Of WritableT)(n)

            Dim i As Integer = 0
            For Each w As WritableT In writables
                If i = columnIdx Then
                    Dim str As String = w.ToString()
                    Dim present(categoryTokens.Count - 1) As Boolean
                    If str IsNot Nothing AndAlso str.Length > 0 Then
                        Dim split() As String = javaSplit(str, delimiter)
                        For Each s As String In split
                            Dim idx As Integer
                            If Not mapField.TryGetValue(s, idx) Then
                                Throw New InvalidOperationException("Encountered unknown String: """ & s & """")
                            End If
                            present(idx) = True
                        Next s
                    End If
                    For j As Integer = 0 To present.Length - 1
                        [out].Add(New Text(If(present(j), "true", "false")))
                    Next j
                Else
                    'No change to this column
                    [out].Add(w)
                End If
                i += 1
            Next w

            Return [out]
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object (as in Java: not supported, returns <c>Nothing</c>)
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Return Nothing
        End Function

        ''' <summary>
        ''' Transform a sequence (as in Java: not supported, returns <c>Nothing</c>)
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
            Throw New NotSupportedException("New column names is always more than 1 in length")
        End Function

        ''' <summary>
        ''' The output column names
        ''' This will often be the same as the input
        ''' </summary>
        ''' <returns> the output column names </returns>
        Public Overrides Function outputColumnNames() As String()
            Return New List(Of String)(newColumnNames).ToArray()
        End Function

        ''' <summary>
        ''' Returns column names
        ''' this op is meant to run on
        ''' </summary>
        Public Overrides Function columnNames() As String()
            Return New String() {columnName()}
        End Function

        ''' <summary>
        ''' Returns a singular column name
        ''' this op is meant to run on
        ''' </summary>
        Public Overrides Function columnName() As String
            Return columnNameField
        End Function

        ''' <summary>Lombok equality (excluding columnIdx).</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim o As StringListToCategoricalSetTransform = DirectCast(obj, StringListToCategoricalSetTransform)
            Return String.Equals(columnNameField, o.columnNameField) AndAlso javaEquals(newColumnNames, o.newColumnNames) AndAlso
                javaEquals(categoryTokens, o.categoryTokens) AndAlso String.Equals(delimiter, o.delimiter) AndAlso javaEquals(mapField, o.mapField)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(columnNameField, newColumnNames, categoryTokens, delimiter, mapField)
        End Function

    End Class

End Namespace
