Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.column

    ''' <summary>
    ''' Add a new column, where the values in that column for all records are identical (according to the specified value).
    ''' <para>Equality (Lombok <c>@Data</c>) compares the class, the new column's name, type and value, and the input schema.</para>
    ''' </summary>
    ' Java: @Data
    <Serializable>
    Public Class AddConstantColumnTransform
        Implements TransformT

        Private ReadOnly newColumnName As String
        Private ReadOnly newColumnType As ColumnType
        Private ReadOnly fixedValue As WritableT

        Private inputSchemaField As SchemaT

        <JsonConstructor>
        Public Sub New(ByVal newColumnName As String, ByVal newColumnType As ColumnType, ByVal fixedValue As WritableT)
            Me.newColumnName = newColumnName
            Me.newColumnType = newColumnType
            Me.fixedValue = fixedValue
        End Sub

        ''' <summary>Java <c>getNewColumnName()</c> (Lombok).</summary>
        Public Function getNewColumnName() As String
            Return newColumnName
        End Function

        ''' <summary>Java <c>getNewColumnType()</c> (Lombok).</summary>
        Public Function getNewColumnType() As ColumnType
            Return newColumnType
        End Function

        ''' <summary>Java <c>getFixedValue()</c> (Lombok).</summary>
        Public Function getFixedValue() As WritableT
            Return fixedValue
        End Function

        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            Dim outMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(inputSchema.getColumnMetaData())

            Dim newColMeta As ColumnMetaData = newColumnType.newColumnMetaData(newColumnName)
            outMeta.Add(newColMeta)
            Return inputSchema.newSchema(outMeta)
        End Function

        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Set(ByVal value As SchemaT)
                Me.inputSchemaField = value
            End Set
            Get
                Return inputSchemaField
            End Get
        End Property

        Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
            Return newColumnName
        End Function

        Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
            Return New String() {outputColumnName()}
        End Function

        Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
            Return New String() {}
        End Function

        Public Overridable Function columnName() As String Implements ColumnOp.columnName
            Return newColumnName
        End Function

        Public Overridable Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT) Implements TransformT.map
            Dim [out] As New List(Of WritableT)(writables.Count + 1)
            [out].AddRange(writables)
            [out].Add(fixedValue)
            Return [out]
        End Function

        Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of WritableT)) Implements TransformT.mapSequence
            Dim outSeq As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))(sequence.Count)
            For Each l As IList(Of WritableT) In sequence
                outSeq.Add(map(l))
            Next l
            Return outSeq
        End Function

        Public Overridable Function map(ByVal input As Object) As Object Implements TransformT.map
            Throw New NotSupportedException()
        End Function

        Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements TransformT.mapSequence
            Throw New NotSupportedException()
        End Function

        ''' <summary>Lombok <c>toString()</c>.</summary>
        Public Overrides Function ToString() As String
            Return "AddConstantColumnTransform(newColumnName=" & newColumnName & ", newColumnType=" & newColumnType.ToString() & ", fixedValue=" & javaToString(fixedValue) & ", inputSchema=" & javaToString(inputSchemaField) & ")"
        End Function

        ''' <summary>Lombok <c>@Data</c> equality (all fields, including the input schema).</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim o As AddConstantColumnTransform = DirectCast(obj, AddConstantColumnTransform)
            Return String.Equals(newColumnName, o.newColumnName) AndAlso newColumnType = o.newColumnType AndAlso
                Objects.equals(fixedValue, o.fixedValue) AndAlso Objects.equals(inputSchemaField, o.inputSchemaField)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(newColumnName, newColumnType, fixedValue, inputSchemaField)
        End Function

    End Class

End Namespace
