Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.column

    ''' <summary>
    ''' Remove the specified columns from the data.
    ''' To specify only the columns to keep, use <see cref="RemoveAllColumnsExceptForTransform"/>.
    ''' Equality compares the class and the columns to remove.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema", "columnsToRemoveIdx", "indicesToRemove"}) @Data
    <JsonIgnoreProperties("inputSchema", "columnsToRemoveIdx", "indicesToRemove")>
    <Serializable>
    Public Class RemoveColumnsTransform
        Inherits BaseTransform

        Private columnsToRemoveIdx() As Integer
        Private columnsToRemove() As String
        Private indicesToRemove As ISet(Of Integer)
        Private leftOverColumns() As String

        <JsonConstructor>
        Public Sub New(ByVal ParamArray columnsToRemove() As String)
            Me.columnsToRemove = columnsToRemove
        End Sub

        ''' <summary>Java <c>getColumnsToRemoveIdx()</c> (Lombok).</summary>
        Public Function getColumnsToRemoveIdx() As Integer()
            Return columnsToRemoveIdx
        End Function

        ''' <summary>Java <c>getColumnsToRemove()</c> (Lombok).</summary>
        Public Function getColumnsToRemove() As String()
            Return columnsToRemove
        End Function

        ''' <summary>Java <c>getIndicesToRemove()</c> (Lombok).</summary>
        Public Function getIndicesToRemove() As ISet(Of Integer)
            Return indicesToRemove
        End Function

        ''' <summary>Java <c>getLeftOverColumns()</c> (Lombok).</summary>
        Public Function getLeftOverColumns() As String()
            Return leftOverColumns
        End Function

        <JsonIgnore>
        Public Overrides Property InputSchema As SchemaT
            Get
                Return inputSchemaField
            End Get
            Set(ByVal value As SchemaT)
                MyBase.InputSchema = value

                'Validate that all 'columns to be removed exist
                For Each s As String In columnsToRemove
                    If Not inputSchemaField.hasColumn(s) Then
                        Throw New InvalidOperationException("Cannot remove column """ & s & """: column does not exist. All " & "columns for input schema: " & javaToString(inputSchemaField.getColumnNames()))
                    End If
                Next s

                leftOverColumns = New String((value.numColumns() - columnsToRemove.Length) - 1) {}

                indicesToRemove = New HashSet(Of Integer)()

                Dim i As Integer = 0
                columnsToRemoveIdx = New Integer(columnsToRemove.Length - 1) {}
                For Each s As String In columnsToRemove
                    Dim idx As Integer = value.getIndexOfColumn(s)
                    If idx < 0 Then
                        Throw New Exception("Column """ & s & """ not found")
                    End If
                    columnsToRemoveIdx(i) = idx
                    i += 1
                    indicesToRemove.Add(idx)
                Next s

                Dim leftOverColumnsIdx As Integer = 0
                Dim columnTest As IList(Of String) = Arrays.asList(columnsToRemove)
                Dim origColumnNames As IList(Of String) = value.getColumnNames()
                For remove As Integer = 0 To value.numColumns() - 1
                    If Not columnTest.Contains(origColumnNames(remove)) Then
                        leftOverColumns(leftOverColumnsIdx) = origColumnNames(remove)
                        leftOverColumnsIdx += 1
                    End If
                Next remove
            End Set
        End Property

        Public Overrides Function transform(ByVal schema As SchemaT) As SchemaT
            Dim nToRemove As Integer = columnsToRemove.Length
            Dim newNumColumns As Integer = schema.numColumns() - nToRemove
            If newNumColumns <= 0 Then
                Throw New InvalidOperationException("Number of columns after executing operation is " & newNumColumns & " (is <= 0). " & "origColumns = " & javaToString(schema.getColumnNames()) & ", toRemove = " & Arrays.toString(columnsToRemove))
            End If

            Dim origNames As IList(Of String) = schema.getColumnNames()
            Dim origMeta As IList(Of ColumnMetaData) = schema.getColumnMetaData()

            Dim [set] As ISet(Of String) = New HashSet(Of String)(columnsToRemove)

            Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(newNumColumns)

            For i As Integer = 0 To origNames.Count - 1
                If Not [set].Contains(origNames(i)) Then
                    newMeta.Add(origMeta(i))
                End If
            Next i

            Return schema.newSchema(newMeta)
        End Function

        Public Overrides Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT)
            If writables.Count <> inputSchemaField.numColumns() Then
                Dim list As New List(Of String)()
                For Each w As WritableT In writables
                    list.Add(w.ToString())
                Next w

                Dim recordString As String = String.Join(",", list)
                Throw New InvalidOperationException("Cannot execute transform: input writables list length (" & writables.Count & ") does not " & "match expected number of elements (schema: " & inputSchemaField.numColumns() & "). Transform = " & Me.ToString() & " and record " & recordString)
            End If

            Dim outList As IList(Of WritableT) = New List(Of WritableT)(writables.Count - columnsToRemove.Length)

            Dim i As Integer = 0
            For Each w As WritableT In writables
                Dim removeIt As Boolean = indicesToRemove.Contains(i)
                i += 1
                If removeIt Then
                    Continue For
                End If
                outList.Add(w)
            Next w
            Return outList
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

        Public Overrides Function ToString() As String
            Return "RemoveColumnsTransform(" & Arrays.toString(columnsToRemove) & ")"
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then
                Return True
            End If
            If o Is Nothing OrElse Me.GetType() IsNot o.GetType() Then
                Return False
            End If

            Dim o2 As RemoveColumnsTransform = DirectCast(o, RemoveColumnsTransform)
            Return javaEquals(columnsToRemove, o2.columnsToRemove)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Arrays.hashCode(columnsToRemove)
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
            Return leftOverColumns
        End Function

        ''' <summary>
        ''' Returns column names
        ''' this op is meant to run on
        ''' </summary>
        Public Overrides Function columnNames() As String()
            Return New List(Of String)(inputSchemaField.getColumnNames()).ToArray()
        End Function

        ''' <summary>
        ''' Returns a singular column name
        ''' this op is meant to run on
        ''' </summary>
        Public Overrides Function columnName() As String
            Return columnNames()(0)
        End Function

    End Class

End Namespace
