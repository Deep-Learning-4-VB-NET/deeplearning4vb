Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.column

    ''' <summary>
    ''' Rename one or more columns. Only the schema changes; the values are not touched.
    ''' Equality compares the class and the old and new names.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema"}) @Data
    <Serializable>
    Public Class RenameColumnsTransform
        Implements TransformT

        Private ReadOnly oldNames As IList(Of String)
        Private ReadOnly newNames As IList(Of String)
        Private inputSchemaField As SchemaT

        Public Sub New(ByVal oldName As String, ByVal newName As String)
            Me.New(JavaCollections.singletonList(oldName), JavaCollections.singletonList(newName))
        End Sub

        <JsonConstructor>
        Public Sub New(ByVal oldNames As IList(Of String), ByVal newNames As IList(Of String))
            If oldNames.Count <> newNames.Count Then
                Throw New ArgumentException("Invalid input: old/new names lists differ in length")
            End If
            Me.oldNames = oldNames
            Me.newNames = newNames
        End Sub

        ''' <summary>Java <c>getOldNames()</c> (Lombok).</summary>
        Public Function getOldNames() As IList(Of String)
            Return oldNames
        End Function

        ''' <summary>Java <c>getNewNames()</c> (Lombok).</summary>
        Public Function getNewNames() As IList(Of String)
            Return newNames
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then
                Return True
            End If
            If o Is Nothing OrElse Me.GetType() IsNot o.GetType() Then
                Return False
            End If

            Dim o2 As RenameColumnsTransform = DirectCast(o, RenameColumnsTransform)

            If Not javaEquals(oldNames, o2.oldNames) Then
                Return False
            End If
            Return javaEquals(newNames, o2.newNames)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = javaHashCode(oldNames)
            result = 31 * result + javaHashCode(newNames)
            Return result
        End Function

        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            'Validate that all 'original' names exist:
            For i As Integer = 0 To oldNames.Count - 1
                Dim s As String = oldNames(i)
                If Not inputSchema.hasColumn(s) Then
                    Throw New InvalidOperationException("Cannot rename from """ & s & """ to """ & newNames(i) & """: original column name """ & s & """ does not exist. All columns for input schema: " & javaToString(inputSchema.getColumnNames()))
                End If
            Next i

            Dim inputNames As IList(Of String) = inputSchema.getColumnNames()

            Dim outputMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)()
            For Each s As String In inputNames
                Dim idx As Integer = oldNames.IndexOf(s)
                If idx >= 0 Then
                    'Switch the old and new names
                    Dim meta As ColumnMetaData = inputSchema.getMetaData(s).clone()
                    meta.Name = newNames(idx)
                    outputMeta.Add(meta)
                Else
                    outputMeta.Add(inputSchema.getMetaData(s))
                End If
            Next s

            Return inputSchema.newSchema(outputMeta)
        End Function

        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Set(ByVal value As SchemaT)
                Me.inputSchemaField = value
            End Set
            Get
                Return inputSchemaField
            End Get
        End Property

        Public Overridable Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT) Implements TransformT.map
            'No op
            Return writables
        End Function

        Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of WritableT)) Implements TransformT.mapSequence
            'No op
            Return sequence
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overridable Function map(ByVal input As Object) As Object Implements TransformT.map
            Throw New NotSupportedException("Unable to map. Please treat this as a special operation. This should be handled by your implementation.")
        End Function

        ''' <summary>
        ''' Transform a sequence
        ''' </summary>
        ''' <param name="sequence"> </param>
        Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements TransformT.mapSequence
            Throw New NotSupportedException("Unable to map. Please treat this as a special operation. This should be handled by your implementation.")
        End Function

        Public Overrides Function ToString() As String
            Return "RenameColumnsTransform(oldNames=" & javaToString(oldNames) & ",newNames=" & javaToString(newNames) & ")"
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
            Return New List(Of String)(newNames).ToArray()
        End Function

        ''' <summary>
        ''' Returns column names
        ''' this op is meant to run on
        ''' </summary>
        Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
            Return New List(Of String)(oldNames).ToArray()
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
