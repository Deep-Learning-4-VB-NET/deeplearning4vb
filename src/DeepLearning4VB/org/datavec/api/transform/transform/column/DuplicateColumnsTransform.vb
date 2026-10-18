Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.column

    ''' <summary>
    ''' Duplicate one or more columns.
    ''' The duplicated columns are placed immediately after the original columns.
    ''' Equality compares the class, the columns to duplicate and the new names.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"columnsToDuplicateSet", "columnIndexesToDuplicateSet", "inputSchema"}) @Data
    <JsonIgnoreProperties("columnsToDuplicateSet", "columnIndexesToDuplicateSet", "inputSchema")>
    <Serializable>
    Public Class DuplicateColumnsTransform
        Implements TransformT

        Private ReadOnly columnsToDuplicate As IList(Of String)
        Private ReadOnly newColumnNames As IList(Of String)
        Private ReadOnly columnsToDuplicateSet As ISet(Of String)
        Private ReadOnly columnIndexesToDuplicateSet As ISet(Of Integer)
        Private inputSchemaField As SchemaT

        ''' <param name="columnsToDuplicate"> List of columns to duplicate </param>
        ''' <param name="newColumnNames">     List of names for the new (duplicate) columns </param>
        <JsonConstructor>
        Public Sub New(ByVal columnsToDuplicate As IList(Of String), ByVal newColumnNames As IList(Of String))
            If columnsToDuplicate Is Nothing OrElse newColumnNames Is Nothing Then
                Throw New ArgumentException("Columns/names cannot be null")
            End If
            If columnsToDuplicate.Count <> newColumnNames.Count Then
                Throw New ArgumentException("Invalid input: columns to duplicate and the new names must have equal lengths")
            End If
            Me.columnsToDuplicate = columnsToDuplicate
            Me.newColumnNames = newColumnNames
            Me.columnsToDuplicateSet = New HashSet(Of String)(columnsToDuplicate)
            Me.columnIndexesToDuplicateSet = New HashSet(Of Integer)()
        End Sub

        ''' <summary>Java <c>getColumnsToDuplicate()</c> (Lombok).</summary>
        Public Function getColumnsToDuplicate() As IList(Of String)
            Return columnsToDuplicate
        End Function

        ''' <summary>Java <c>getNewColumnNames()</c> (Lombok).</summary>
        Public Function getNewColumnNames() As IList(Of String)
            Return newColumnNames
        End Function

        ''' <summary>Java <c>getColumnsToDuplicateSet()</c> (Lombok).</summary>
        Public Function getColumnsToDuplicateSet() As ISet(Of String)
            Return columnsToDuplicateSet
        End Function

        ''' <summary>Java <c>getColumnIndexesToDuplicateSet()</c> (Lombok).</summary>
        Public Function getColumnIndexesToDuplicateSet() As ISet(Of Integer)
            Return columnIndexesToDuplicateSet
        End Function

        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            Dim oldMeta As IList(Of ColumnMetaData) = inputSchema.getColumnMetaData()
            Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(oldMeta.Count + newColumnNames.Count)

            Dim oldNames As IList(Of String) = inputSchema.getColumnNames()

            Dim dupCount As Integer = 0
            For i As Integer = 0 To oldMeta.Count - 1
                Dim current As String = oldNames(i)
                newMeta.Add(oldMeta(i))

                If columnsToDuplicateSet.Contains(current) Then
                    'Duplicate the current columnName, and place it after...
                    Dim dupName As String = newColumnNames(dupCount)
                    Dim m As ColumnMetaData = oldMeta(i).clone()
                    m.Name = dupName
                    newMeta.Add(m)
                    dupCount += 1
                End If
            Next i

            Return inputSchema.newSchema(newMeta)
        End Function

        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Set(ByVal value As SchemaT)
                columnIndexesToDuplicateSet.Clear()

                Dim schemaColumnNames As IList(Of String) = value.getColumnNames()
                For Each s As String In columnsToDuplicate
                    Dim idx As Integer = schemaColumnNames.IndexOf(s)
                    If idx = -1 Then
                        Throw New InvalidOperationException("Invalid state: column to duplicate """ & s & """ does not appear " & "in input schema")
                    End If
                    columnIndexesToDuplicateSet.Add(idx)
                Next s

                Me.inputSchemaField = value
            End Set
            Get
                Return inputSchemaField
            End Get
        End Property

        Public Overridable Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT) Implements TransformT.map
            If writables.Count <> inputSchemaField.numColumns() Then
                Throw New InvalidOperationException(badRecordLengthMessage(writables.Count, inputSchemaField.numColumns(), Me))
            End If
            Dim [out] As IList(Of WritableT) = New List(Of WritableT)(writables.Count + columnsToDuplicate.Count)
            Dim i As Integer = 0
            For Each w As WritableT In writables
                [out].Add(w)
                If columnIndexesToDuplicateSet.Contains(i) Then
                    [out].Add(w) 'TODO safter to copy here...
                End If
                i += 1
            Next w
            Return [out]
        End Function

        Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of WritableT)) Implements TransformT.mapSequence
            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))(sequence.Count)
            For Each l As IList(Of WritableT) In sequence
                [out].Add(map(l))
            Next l
            Return [out]
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overridable Function map(ByVal input As Object) As Object Implements TransformT.map
            Return input
        End Function

        ''' <summary>
        ''' Transform a sequence
        ''' </summary>
        ''' <param name="sequence"> </param>
        Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements TransformT.mapSequence
            Return sequence
        End Function

        Public Overrides Function ToString() As String
            Return "DuplicateColumnsTransform(toDuplicate=" & javaToString(columnsToDuplicate) & ",newNames=" & javaToString(newColumnNames) & ")"
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then
                Return True
            End If
            If o Is Nothing OrElse Me.GetType() IsNot o.GetType() Then
                Return False
            End If

            Dim o2 As DuplicateColumnsTransform = DirectCast(o, DuplicateColumnsTransform)

            If Not javaEquals(columnsToDuplicate, o2.columnsToDuplicate) Then
                Return False
            End If
            Return javaEquals(newColumnNames, o2.newColumnNames)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = javaHashCode(columnsToDuplicate)
            result = 31 * result + javaHashCode(newColumnNames)
            Return result
        End Function

        ''' <summary>
        ''' The output column name
        ''' after the operation has been applied
        ''' </summary>
        ''' <returns> the output column name </returns>
        Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
            Return outputColumnNames()(0)
        End Function

        ''' <summary>
        ''' The output column names
        ''' This will often be the same as the input
        ''' </summary>
        ''' <returns> the output column names </returns>
        Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
            Return New List(Of String)(newColumnNames).ToArray()
        End Function

        ''' <summary>
        ''' Returns column names
        ''' this op is meant to run on
        ''' </summary>
        Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
            Return New List(Of String)(columnsToDuplicate).ToArray()
        End Function

        ''' <summary>
        ''' Returns a singular column name
        ''' this op is meant to run on
        ''' </summary>
        Public Overridable Function columnName() As String Implements ColumnOp.columnName
            Return columnNames()(0)
        End Function

    End Class

End Namespace
