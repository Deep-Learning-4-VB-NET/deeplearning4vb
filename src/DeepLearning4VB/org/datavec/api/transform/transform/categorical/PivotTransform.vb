Imports System
Imports System.Collections.Generic
Imports CategoricalMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.CategoricalMetaData
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports FloatWritable = DeepLearning4VB.org.datavec.api.writable.FloatWritable
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports NullWritable = DeepLearning4VB.org.datavec.api.writable.NullWritable
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.categorical

    ''' <summary>
    ''' Pivot transform operates on two columns:
    ''' - a categorical column that operates as a key, and
    ''' - Another column that contains a value
    ''' Essentially, Pivot transform takes keyvalue pairs and breaks them out into separate columns.
    '''
    ''' For example, with schema [col0, key, value, col3]
    ''' and values with key in {a,b,c}
    ''' Output schema is [col0, key[a], key[b], key[c], col3]
    ''' and input (col0Val, b, x, col3Val) gets mapped to (col0Val, 0, x, 0, col3Val).
    '''
    ''' When expanding columns, a default value is used - for example 0 for numerical columns.
    ''' <para>Equality (Lombok <c>@Data</c>) compares the class, the key and value columns and the default value.</para>
    ''' </summary>
    ' Java: @Data
    <Serializable>
    Public Class PivotTransform
        Inherits BaseTransform

        Private ReadOnly keyColumn As String
        Private ReadOnly valueColumn As String
        Private defaultValue As WritableT

        ''' <param name="keyColumnName">   Key column to expand </param>
        ''' <param name="valueColumnName"> Name of the column that contains the value </param>
        Public Sub New(ByVal keyColumnName As String, ByVal valueColumnName As String)
            Me.New(keyColumnName, valueColumnName, Nothing)
        End Sub

        ''' <param name="keyColumnName">   Key column to expand </param>
        ''' <param name="valueColumnName"> Name of the column that contains the value </param>
        ''' <param name="defaultValue">    The default value to use in expanded columns. </param>
        Public Sub New(ByVal keyColumnName As String, ByVal valueColumnName As String, ByVal defaultValue As WritableT)
            Me.keyColumn = keyColumnName
            Me.valueColumn = valueColumnName
            Me.defaultValue = defaultValue
        End Sub

        ''' <summary>Java <c>getKeyColumn()</c> (Lombok).</summary>
        Public Function getKeyColumn() As String
            Return keyColumn
        End Function

        ''' <summary>Java <c>getValueColumn()</c> (Lombok).</summary>
        Public Function getValueColumn() As String
            Return valueColumn
        End Function

        ''' <summary>Java <c>getDefaultValue()</c> (Lombok).</summary>
        Public Function getDefaultValue() As WritableT
            Return defaultValue
        End Function

        ''' <summary>Java <c>setDefaultValue()</c> (Lombok).</summary>
        Public Sub setDefaultValue(ByVal defaultValue As WritableT)
            Me.defaultValue = defaultValue
        End Sub

        Public Overrides Function transform(ByVal inputSchema As SchemaT) As SchemaT
            If Not inputSchema.hasColumn(keyColumn) OrElse Not inputSchema.hasColumn(valueColumn) Then
                Throw New NotSupportedException("Key or value column not found: " & keyColumn & ", " & valueColumn & " in " & javaToString(inputSchema.getColumnNames()))
            End If

            Dim origNames As IList(Of String) = inputSchema.getColumnNames()
            Dim origMeta As IList(Of ColumnMetaData) = inputSchema.getColumnMetaData()

            Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(inputSchema.numColumns())

            Dim idxKey As Integer = inputSchema.getIndexOfColumn(keyColumn)
            Dim idxValue As Integer = inputSchema.getIndexOfColumn(valueColumn)

            Dim valueMeta As ColumnMetaData = inputSchema.getMetaData(idxValue)

            For i As Integer = 0 To origNames.Count - 1
                Dim s As String = origNames(i)
                Dim t As ColumnMetaData = origMeta(i)

                If i = idxKey Then
                    'Convert this to a set of separate columns
                    Dim stateNames As IList(Of String) = DirectCast(inputSchema.getMetaData(idxKey), CategoricalMetaData).getStateNames()
                    For Each stateName As String In stateNames
                        Dim newName As String = s & "[" & stateName & "]"

                        Dim newValueMeta As ColumnMetaData = valueMeta.clone()
                        newValueMeta.Name = newName
                        newMeta.Add(newValueMeta)
                    Next stateName
                ElseIf i = idxValue Then
                    Continue For 'Skip column
                Else
                    newMeta.Add(t)
                End If
            Next i

            'Infer the default value if necessary
            If defaultValue Is Nothing Then
                Select Case valueMeta.ColumnType
                    Case ColumnType.String
                        defaultValue = New Text("")
                    Case ColumnType.Integer
                        defaultValue = New IntWritable(0)
                    Case ColumnType.Long
                        defaultValue = New LongWritable(0)
                    Case ColumnType.Double
                        defaultValue = New DoubleWritable(0.0)
                    Case ColumnType.Float
                        defaultValue = New FloatWritable(0.0F)
                    Case ColumnType.Categorical
                        defaultValue = New NullWritable()
                    Case ColumnType.Time
                        defaultValue = New LongWritable(0)
                    Case ColumnType.Bytes
                        Throw New NotSupportedException("Cannot infer default value for bytes")
                    Case ColumnType.Boolean
                        defaultValue = New Text("false")
                    Case Else
                        Throw New NotSupportedException("Cannot infer default value for " & valueMeta.ColumnType.ToString())
                End Select
            End If

            Return inputSchema.newSchema(newMeta)
        End Function

        Public Overrides Function outputColumnName() As String
            Throw New NotSupportedException("Output column name will be more than 1")
        End Function

        Public Overrides Function outputColumnNames() As String()
            Dim l As IList(Of String) = DirectCast(inputSchemaField.getMetaData(keyColumn), CategoricalMetaData).getStateNames()
            Return New List(Of String)(l).ToArray()
        End Function

        Public Overrides Function columnNames() As String()
            Return New String() {keyColumn, valueColumn}
        End Function

        Public Overrides Function columnName() As String
            Throw New NotSupportedException("Multiple input columns")
        End Function

        Public Overrides Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT)
            If writables.Count <> inputSchemaField.numColumns() Then
                Throw New InvalidOperationException(badRecordLengthMessage(writables.Count, inputSchemaField.numColumns(), Me))
            End If

            Dim idxKey As Integer = inputSchemaField.getIndexOfColumn(keyColumn)
            Dim idxValue As Integer = inputSchemaField.getIndexOfColumn(valueColumn)
            Dim stateNames As IList(Of String) = DirectCast(inputSchemaField.getMetaData(idxKey), CategoricalMetaData).getStateNames()

            Dim i As Integer = 0
            Dim [out] As IList(Of WritableT) = New List(Of WritableT)()
            For Each w As WritableT In writables
                If i = idxKey Then
                    'Do conversion
                    Dim str As String = w.ToString()
                    Dim stateIdx As Integer = stateNames.IndexOf(str)

                    If stateIdx < 0 Then
                        Throw New Exception("Unknown state (index not found): " & str)
                    End If
                    For j As Integer = 0 To stateNames.Count - 1
                        If j = stateIdx Then
                            [out].Add(writables(idxValue))
                        Else
                            [out].Add(defaultValue)
                        End If
                    Next j
                ElseIf i = idxValue Then
                    i += 1
                    Continue For
                Else
                    'No change to this column
                    [out].Add(w)
                End If
                i += 1
            Next w
            Return [out]
        End Function

        ''' <summary>Maps a (key, value) pair of writables to the expanded values.</summary>
        Public Overrides Function map(ByVal input As Object) As Object
            Dim l As System.Collections.IList = DirectCast(input, System.Collections.IList)
            Dim k As WritableT = DirectCast(l(0), WritableT)
            Dim v As WritableT = DirectCast(l(1), WritableT)

            Dim idxKey As Integer = inputSchemaField.getIndexOfColumn(keyColumn)
            Dim stateNames As IList(Of String) = DirectCast(inputSchemaField.getMetaData(idxKey), CategoricalMetaData).getStateNames()
            Dim n As Integer = stateNames.Count
            Dim position As Integer = stateNames.IndexOf(k.ToString())

            Dim [out] As IList(Of WritableT) = New List(Of WritableT)()
            For j As Integer = 0 To n - 1
                If j = position Then
                    [out].Add(v)
                Else
                    [out].Add(defaultValue)
                End If
            Next j
            Return [out]
        End Function

        ''' <summary>Maps each (key, value) pair of the sequence with <c>map(Object)</c>.</summary>
        Public Overrides Function mapSequence(ByVal sequence As Object) As Object
            Dim values As System.Collections.IList = DirectCast(sequence, System.Collections.IList)
            Dim ret As IList(Of Object) = New List(Of Object)()
            For Each obj As Object In values
                ret.Add(map(obj))
            Next obj
            Return ret
        End Function

        ''' <summary>Lombok <c>toString()</c>.</summary>
        Public Overrides Function ToString() As String
            Return "PivotTransform(keyColumn=" & keyColumn & ", valueColumn=" & valueColumn & ", defaultValue=" & javaToString(defaultValue) & ")"
        End Function

        ''' <summary>Lombok <c>@Data</c> equality: the key and value columns and the default value.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim o As PivotTransform = DirectCast(obj, PivotTransform)
            Return String.Equals(keyColumn, o.keyColumn) AndAlso String.Equals(valueColumn, o.valueColumn) AndAlso Objects.equals(defaultValue, o.defaultValue)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(keyColumn, valueColumn, defaultValue)
        End Function

    End Class

End Namespace
