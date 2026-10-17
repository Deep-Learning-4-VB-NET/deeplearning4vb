Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports CategoricalMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.CategoricalMetaData
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports IntegerMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.IntegerMetaData
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.categorical

    ''' <summary>
    ''' Converts a categorical column to an integer column holding the index of the category in the column's state
    ''' names (0 to numStates - 1). Equality compares the column name.
    ''' </summary>
    ' Java: @Data @JsonIgnoreProperties({"inputSchema", "columnIdx", "stateNames", "statesMap"})
    <Serializable>
    Public Class CategoricalToIntegerTransform
        Inherits BaseTransform

        Private columnNameField As String
        Private columnIdx As Integer = -1
        Private stateNames As IList(Of String)
        Private statesMap As IDictionary(Of String, Integer)

        <JsonConstructor>
        Public Sub New(ByVal columnName As String)
            Me.columnNameField = columnName
        End Sub

        <JsonIgnore>
        Public Overrides Property InputSchema As SchemaT
            Get
                Return inputSchemaField
            End Get
            Set(ByVal value As SchemaT)
                MyBase.InputSchema = value

                columnIdx = value.getIndexOfColumn(columnNameField)
                Dim meta As ColumnMetaData = value.getMetaData(columnNameField)
                If Not (TypeOf meta Is CategoricalMetaData) Then
                    Throw New InvalidOperationException("Cannot convert column """ & columnNameField & """ from categorical to one-hot: column is not categorical (is: " & meta.ColumnType.ToString() & ")")
                End If
                Me.stateNames = DirectCast(meta, CategoricalMetaData).getStateNames()

                Me.statesMap = New Dictionary(Of String, Integer)(stateNames.Count)
                For i As Integer = 0 To stateNames.Count - 1
                    Me.statesMap(stateNames(i)) = i
                Next i
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

        ''' <summary>Java <c>getColumnIdx()</c> (Lombok).</summary>
        Public Function getColumnIdx() As Integer
            Return columnIdx
        End Function

        ''' <summary>Java <c>getStateNames()</c> (Lombok).</summary>
        Public Function getStateNames() As IList(Of String)
            Return stateNames
        End Function

        ''' <summary>Java <c>getStatesMap()</c> (Lombok).</summary>
        Public Function getStatesMap() As IDictionary(Of String, Integer)
            Return statesMap
        End Function

        Public Overrides Function transform(ByVal schema As SchemaT) As SchemaT
            Dim origMeta As IList(Of ColumnMetaData) = schema.getColumnMetaData()
            Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(schema.numColumns())

            Dim i As Integer = 0
            For Each t As ColumnMetaData In origMeta
                If i = columnIdx Then
                    'Convert this to integer
                    Dim nClasses As Integer = stateNames.Count
                    newMeta.Add(New IntegerMetaData(t.Name, 0, nClasses - 1))
                Else
                    newMeta.Add(t)
                End If
                i += 1
            Next t

            Return schema.newSchema(newMeta)
        End Function

        Public Overrides Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT)
            If writables.Count <> inputSchemaField.numColumns() Then
                Throw New InvalidOperationException(badRecordLengthMessage(writables.Count, inputSchemaField.numColumns(), Me))
            End If
            Dim idx As Integer = getColumnIdx()

            Dim n As Integer = stateNames.Count
            Dim [out] As IList(Of WritableT) = New List(Of WritableT)(writables.Count + n)

            Dim i As Integer = 0
            For Each w As WritableT In writables
                If i = idx Then
                    'Do conversion
                    [out].Add(New IntWritable(classIndex(w.ToString())))
                Else
                    'No change to this column
                    [out].Add(w)
                End If
                i += 1
            Next w
            Return [out]
        End Function

        Private Function classIndex(ByVal str As String) As Integer
            Dim classIdx As Integer
            If str Is Nothing OrElse Not statesMap.TryGetValue(str, classIdx) Then
                Throw New InvalidOperationException("Cannot convert categorical value to integer value: input value (""" & str & """) is not in the list of known categories (state names/categories: " & javaToString(stateNames) & ")")
            End If
            Return classIdx
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Return classIndex(input.ToString())
        End Function

        ''' <summary>
        ''' Transform a sequence (as in Java: not supported, returns <c>Nothing</c>)
        ''' </summary>
        ''' <param name="sequence"> </param>
        Public Overrides Function mapSequence(ByVal sequence As Object) As Object
            Return Nothing
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            If Not (TypeOf o Is CategoricalToIntegerTransform) Then
                Return False
            End If

            Dim o2 As CategoricalToIntegerTransform = DirectCast(o, CategoricalToIntegerTransform)
            Return String.Equals(columnNameField, o2.columnNameField)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return If(columnNameField Is Nothing, 0, columnNameField.GetHashCode())
        End Function

        ''' <summary>Lombok <c>toString()</c>.</summary>
        Public Overrides Function ToString() As String
            Return "CategoricalToIntegerTransform(columnName=" & columnNameField & ", columnIdx=" & columnIdx & ", stateNames=" & javaToString(stateNames) & ", statesMap=" & javaToString(statesMap) & ")"
        End Function

        ''' <summary>
        ''' The output column name
        ''' after the operation has been applied
        ''' </summary>
        ''' <returns> the output column name </returns>
        Public Overrides Function outputColumnName() As String
            Return columnNameField
        End Function

        ''' <summary>
        ''' The output column names
        ''' This will often be the same as the input
        ''' </summary>
        ''' <returns> the output column names </returns>
        Public Overrides Function outputColumnNames() As String()
            Return New String() {columnName()}
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

    End Class

End Namespace
