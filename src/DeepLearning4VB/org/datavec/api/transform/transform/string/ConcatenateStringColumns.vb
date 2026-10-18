Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.string

    ''' <summary>
    ''' Concatenate values of one or more String columns into a new String column. Retains the constituent String
    ''' columns so user must remove those manually, if desired.
    '''
    ''' TODO: use new String Reduce functionality in DataVec?
    ''' <para>Equality compares the class, the delimiter and the columns to concatenate.</para>
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema"}) @Data
    <JsonIgnoreProperties("inputSchema")>
    <Serializable>
    Public Class ConcatenateStringColumns
        Inherits BaseTransform

        Private ReadOnly newColumnName As String
        Private ReadOnly delimiter As String
        Private ReadOnly columnsToConcatenate As IList(Of String)

        ''' <param name="columnsToConcatenate"> A partial or complete order of the columns in the output </param>
        Public Sub New(ByVal newColumnName As String, ByVal delimiter As String, ByVal ParamArray columnsToConcatenate() As String)
            Me.New(newColumnName, delimiter, Arrays.asList(columnsToConcatenate))
        End Sub

        ''' <param name="columnsToConcatenate"> A partial or complete order of the columns in the output </param>
        <JsonConstructor>
        Public Sub New(ByVal newColumnName As String, ByVal delimiter As String, ByVal columnsToConcatenate As IList(Of String))
            Me.newColumnName = newColumnName
            Me.delimiter = delimiter
            Me.columnsToConcatenate = columnsToConcatenate
        End Sub

        ''' <summary>Java <c>getNewColumnName()</c> (Lombok).</summary>
        Public Function getNewColumnName() As String
            Return newColumnName
        End Function

        ''' <summary>Java <c>getDelimiter()</c> (Lombok).</summary>
        Public Function getDelimiter() As String
            Return delimiter
        End Function

        ''' <summary>Java <c>getColumnsToConcatenate()</c> (Lombok).</summary>
        Public Function getColumnsToConcatenate() As IList(Of String)
            Return columnsToConcatenate
        End Function

        Public Overrides Function transform(ByVal inputSchema As SchemaT) As SchemaT
            For Each s As String In columnsToConcatenate
                If Not inputSchema.hasColumn(s) Then
                    Throw New InvalidOperationException("Input schema does not contain column with name """ & s & """")
                End If
            Next s

            Dim outMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(inputSchema.getColumnMetaData())

            Dim newColMeta As ColumnMetaData = ColumnType.String.newColumnMetaData(newColumnName)
            outMeta.Add(newColMeta)
            Return inputSchema.newSchema(outMeta)
        End Function

        <JsonIgnore>
        Public Overrides Property InputSchema As SchemaT
            Set(ByVal value As SchemaT)
                For Each s As String In columnsToConcatenate
                    If Not value.hasColumn(s) Then
                        Throw New InvalidOperationException("Input schema does not contain column with name """ & s & """")
                    End If
                Next s
                Me.inputSchemaField = value
            End Set
            Get
                Return inputSchemaField
            End Get
        End Property

        Public Overrides Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT)
            Dim newColumnText As New StringBuilder()
            Dim [out] As IList(Of WritableT) = New List(Of WritableT)(writables)
            Dim i As Integer = 0
            For Each columnName As String In columnsToConcatenate
                If i > 0 Then
                    newColumnText.Append(delimiter)
                End If
                i += 1
                Dim columnIdx As Integer = inputSchemaField.getIndexOfColumn(columnName)
                newColumnText.Append(javaToString(writables(columnIdx)))
            Next columnName
            [out].Add(New Text(newColumnText.ToString()))
            Return [out]
        End Function

        Public Overrides Function mapSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of WritableT))
            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            For Each [step] As IList(Of WritableT) In sequence
                [out].Add(map([step]))
            Next [step]
            Return [out]
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Throw New NotSupportedException("Unable to map. Please treat this as a special operation. This should be handled by your implementation.")
        End Function

        ''' <summary>
        ''' Transform a sequence
        ''' </summary>
        ''' <param name="sequence"> </param>
        Public Overrides Function mapSequence(ByVal sequence As Object) As Object
            Throw New NotSupportedException("Unable to map. Please treat this as a special operation. This should be handled by your implementation.")
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then
                Return True
            End If
            If o Is Nothing OrElse Me.GetType() IsNot o.GetType() Then
                Return False
            End If

            Dim o2 As ConcatenateStringColumns = DirectCast(o, ConcatenateStringColumns)
            Return String.Equals(delimiter, o2.delimiter) AndAlso javaEquals(columnsToConcatenate, o2.columnsToConcatenate)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = If(delimiter Is Nothing, 0, delimiter.GetHashCode())
            result = 31 * result + javaHashCode(columnsToConcatenate)
            Return result
        End Function

        Public Overrides Function ToString() As String
            Return "ConcatenateStringColumns(delimiters=" & delimiter & " columnsToConcatenate=" & javaToString(columnsToConcatenate) & ")"
        End Function

        ''' <summary>
        ''' The output column name
        ''' after the operation has been applied
        ''' </summary>
        ''' <returns> the output column name </returns>
        Public Overrides Function outputColumnName() As String
            Return newColumnName
        End Function

        ''' <summary>
        ''' The output column names
        ''' This will often be the same as the input
        ''' </summary>
        ''' <returns> the output column names </returns>
        Public Overrides Function outputColumnNames() As String()
            Return New String() {newColumnName}
        End Function

        ''' <summary>
        ''' Returns column names
        ''' this op is meant to run on
        ''' </summary>
        Public Overrides Function columnNames() As String()
            Return New List(Of String)(columnsToConcatenate).ToArray()
        End Function

        ''' <summary>
        ''' Returns a singular column name
        ''' this op is meant to run on
        ''' </summary>
        Public Overrides Function columnName() As String
            Return columnsToConcatenate(0)
        End Function

    End Class

End Namespace
