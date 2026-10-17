Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.column

    ''' <summary>
    ''' Transform that removes all columns except for those that are explicitly specified as ones to keep
    ''' To specify only the columns to remove, use <see cref="RemoveColumnsTransform"/>.
    ''' Equality compares the class and the columns to keep.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema", "columnsToKeepIdx", "indicesToKeep"}) @Data
    <Serializable>
    Public Class RemoveAllColumnsExceptForTransform
        Inherits BaseTransform

        Private columnsToKeepIdx() As Integer
        Private columnsToKeep() As String
        Private indicesToKeep As ISet(Of Integer)

        <JsonConstructor>
        Public Sub New(ByVal ParamArray columnsToKeep() As String)
            Me.columnsToKeep = columnsToKeep
        End Sub

        ''' <summary>Java <c>getColumnsToKeepIdx()</c> (Lombok).</summary>
        Public Function getColumnsToKeepIdx() As Integer()
            Return columnsToKeepIdx
        End Function

        ''' <summary>Java <c>getColumnsToKeep()</c> (Lombok).</summary>
        Public Function getColumnsToKeep() As String()
            Return columnsToKeep
        End Function

        ''' <summary>Java <c>getIndicesToKeep()</c> (Lombok).</summary>
        Public Function getIndicesToKeep() As ISet(Of Integer)
            Return indicesToKeep
        End Function

        <JsonIgnore>
        Public Overrides Property InputSchema As SchemaT
            Get
                Return inputSchemaField
            End Get
            Set(ByVal value As SchemaT)
                MyBase.InputSchema = value

                indicesToKeep = New HashSet(Of Integer)()

                Dim i As Integer = 0
                columnsToKeepIdx = New Integer(columnsToKeep.Length - 1) {}
                For Each s As String In columnsToKeep
                    Dim idx As Integer = value.getIndexOfColumn(s)
                    If idx < 0 Then
                        Throw New Exception("Column """ & s & """ not found")
                    End If
                    columnsToKeepIdx(i) = idx
                    i += 1
                    indicesToKeep.Add(idx)
                Next s
            End Set
        End Property

        Public Overrides Function transform(ByVal schema As SchemaT) As SchemaT
            Dim origNames As IList(Of String) = schema.getColumnNames()
            Dim origMeta As IList(Of ColumnMetaData) = schema.getColumnMetaData()

            Dim keepSet As ISet(Of String) = New HashSet(Of String)(columnsToKeep)

            Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(columnsToKeep.Length)

            For i As Integer = 0 To origNames.Count - 1
                If keepSet.Contains(origNames(i)) Then
                    newMeta.Add(origMeta(i))
                End If
            Next i

            Return schema.newSchema(newMeta)
        End Function

        Public Overrides Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT)
            If writables.Count <> inputSchemaField.numColumns() Then
                Throw New InvalidOperationException(badRecordLengthMessage(writables.Count, inputSchemaField.numColumns(), Me))
            End If

            Dim outList As IList(Of WritableT) = New List(Of WritableT)(columnsToKeep.Length)

            Dim i As Integer = 0
            For Each w As WritableT In writables
                Dim keep As Boolean = indicesToKeep.Contains(i)
                i += 1
                If Not keep Then
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
            Return "RemoveAllColumnsExceptForTransform(" & Arrays.toString(columnsToKeep) & ")"
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then
                Return True
            End If
            If o Is Nothing OrElse Me.GetType() IsNot o.GetType() Then
                Return False
            End If

            Dim o2 As RemoveAllColumnsExceptForTransform = DirectCast(o, RemoveAllColumnsExceptForTransform)
            Return javaEquals(columnsToKeep, o2.columnsToKeep)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Arrays.hashCode(columnsToKeep)
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
            Return columnsToKeep
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
