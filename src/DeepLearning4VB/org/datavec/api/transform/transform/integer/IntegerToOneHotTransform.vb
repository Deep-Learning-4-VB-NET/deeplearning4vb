Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports IntegerMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.IntegerMetaData
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.integer

    ''' <summary>
    ''' Convert an integer column to a set of 1 hot columns, based on the value in integer column.
    ''' Columns are named <c>column[min]</c> to <c>column[max]</c>.
    ''' <para>Equality (Lombok, <c>callSuper = false</c>, excluding columnIdx) compares the class, the column name and
    ''' the min and max values.</para>
    ''' </summary>
    ' Java: @Data @EqualsAndHashCode(exclude = {"columnIdx"}, callSuper = false)
    '       @JsonIgnoreProperties({"inputSchema", "columnIdx", "stateNames", "statesMap"})
    <Serializable>
    Public Class IntegerToOneHotTransform
        Inherits BaseTransform

        Private columnNameField As String
        Private minValue As Integer
        Private maxValue As Integer
        Private columnIdx As Integer = -1

        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal minValue As Integer, ByVal maxValue As Integer)
            Me.columnNameField = columnName
            Me.minValue = minValue
            Me.maxValue = maxValue
        End Sub

        ''' <summary>Java <c>getColumnName()</c> (Lombok).</summary>
        Public Function getColumnName() As String
            Return columnNameField
        End Function

        ''' <summary>Java <c>setColumnName()</c> (Lombok).</summary>
        Public Sub setColumnName(ByVal columnName As String)
            Me.columnNameField = columnName
        End Sub

        ''' <summary>Java <c>getMinValue()</c> (Lombok).</summary>
        Public Function getMinValue() As Integer
            Return minValue
        End Function

        ''' <summary>Java <c>setMinValue()</c> (Lombok).</summary>
        Public Sub setMinValue(ByVal minValue As Integer)
            Me.minValue = minValue
        End Sub

        ''' <summary>Java <c>getMaxValue()</c> (Lombok).</summary>
        Public Function getMaxValue() As Integer
            Return maxValue
        End Function

        ''' <summary>Java <c>setMaxValue()</c> (Lombok).</summary>
        Public Sub setMaxValue(ByVal maxValue As Integer)
            Me.maxValue = maxValue
        End Sub

        ''' <summary>Java <c>getColumnIdx()</c> (Lombok).</summary>
        Public Function getColumnIdx() As Integer
            Return columnIdx
        End Function

        <JsonIgnore>
        Public Overrides Property InputSchema As SchemaT
            Get
                Return inputSchemaField
            End Get
            Set(ByVal value As SchemaT)
                MyBase.InputSchema = value

                columnIdx = value.getIndexOfColumn(columnNameField)
                Dim meta As ColumnMetaData = value.getMetaData(columnNameField)
                If Not (TypeOf meta Is IntegerMetaData) Then
                    Throw New InvalidOperationException("Cannot convert column """ & columnNameField & """ from integer to one-hot: column is not integer (is: " & meta.ColumnType.ToString() & ")")
                End If
            End Set
        End Property

        ''' <summary>As in Java (which reused the CategoricalToOneHotTransform text).</summary>
        Public Overrides Function ToString() As String
            Return "CategoricalToOneHotTransform(columnName=""" & columnNameField & """)"
        End Function

        Public Overrides Function transform(ByVal schema As SchemaT) As SchemaT
            Dim origNames As IList(Of String) = schema.getColumnNames()
            Dim origMeta As IList(Of ColumnMetaData) = schema.getColumnMetaData()

            Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(schema.numColumns())

            For i As Integer = 0 To origNames.Count - 1
                Dim s As String = origNames(i)
                Dim t As ColumnMetaData = origMeta(i)
                If i = columnIdx Then
                    'Convert this to one-hot:
                    For x As Integer = minValue To maxValue
                        Dim newName As String = s & "[" & x & "]"
                        newMeta.Add(New IntegerMetaData(newName, 0, 1))
                    Next x
                Else
                    newMeta.Add(t)
                End If
            Next i

            Return schema.newSchema(newMeta)
        End Function

        Public Overrides Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT)
            If writables.Count <> inputSchemaField.numColumns() Then
                Throw New InvalidOperationException(badRecordLengthMessage(writables.Count, inputSchemaField.numColumns(), Me))
            End If
            Dim idx As Integer = getColumnIdx()

            Dim n As Integer = maxValue - minValue + 1
            Dim [out] As IList(Of WritableT) = New List(Of WritableT)(writables.Count + n)

            Dim i As Integer = 0
            For Each w As WritableT In writables
                If i = idx Then
                    Dim currValue As Integer = w.toInt()
                    checkRange(currValue)
                    For j As Integer = minValue To maxValue
                        If j = currValue Then
                            [out].Add(New IntWritable(1))
                        Else
                            [out].Add(New IntWritable(0))
                        End If
                    Next j
                Else
                    'No change to this column
                    [out].Add(w)
                End If
                i += 1
            Next w
            Return [out]
        End Function

        Private Sub checkRange(ByVal currValue As Integer)
            If currValue < minValue OrElse currValue > maxValue Then
                Throw New InvalidOperationException("Invalid value: integer value (" & currValue & ") is outside of " & "valid range: must be between " & minValue & " and " & maxValue & " inclusive")
            End If
        End Sub

        ''' <summary>
        ''' Transform an object
        ''' in to another object: a number becomes its one-hot values (a list of integers)
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Dim currValue As Integer = JavaNumbers.intValue(input)
            checkRange(currValue)
            Dim oneHot As IList(Of Integer) = New List(Of Integer)()
            For j As Integer = minValue To maxValue
                If j = currValue Then
                    oneHot.Add(1)
                Else
                    oneHot.Add(0)
                End If
            Next j
            Return oneHot
        End Function

        ''' <summary>
        ''' Transform a sequence
        ''' </summary>
        ''' <param name="sequence"> </param>
        Public Overrides Function mapSequence(ByVal sequence As Object) As Object
            Dim values As System.Collections.IList = DirectCast(sequence, System.Collections.IList)
            Dim ret As IList(Of IList(Of Integer)) = New List(Of IList(Of Integer))()
            For Each obj As Object In values
                ret.Add(DirectCast(map(obj), IList(Of Integer)))
            Next obj
            Return ret
        End Function

        ''' <summary>
        ''' The output column name
        ''' after the operation has been applied
        ''' </summary>
        ''' <returns> the output column name </returns>
        Public Overrides Function outputColumnName() As String
            Throw New NotSupportedException("Output column name will be more than 1")
        End Function

        ''' <summary>
        ''' The output column names
        ''' This will often be the same as the input
        ''' </summary>
        ''' <returns> the output column names </returns>
        Public Overrides Function outputColumnNames() As String()
            Return New List(Of String)(transform(inputSchemaField).getColumnNames()).ToArray()
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
            Return columnNameField
        End Function

        ''' <summary>Equal when the class, the column name and the min and max values are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim o As IntegerToOneHotTransform = DirectCast(obj, IntegerToOneHotTransform)
            Return String.Equals(columnNameField, o.columnNameField) AndAlso minValue = o.minValue AndAlso maxValue = o.maxValue
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(columnNameField, minValue, maxValue)
        End Function

    End Class

End Namespace
