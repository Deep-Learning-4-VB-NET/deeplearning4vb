Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

' The Java package is org.datavec.api.transform.transform. VB cannot declare a namespace "transform" next to the
' interface org.datavec.api.transform.Transform (names are case-insensitive: error BC30179), so the transform
' implementations live in org.datavec.api.transform.transforms.
Namespace org.datavec.api.transform.transforms

    ''' <summary>
    ''' Base class of most transforms: keeps the input schema and maps a sequence step by step with
    ''' <c>map(List)</c>. Equality (Lombok <c>@Data</c>) compares the class and the input
    ''' schema; subclasses define their own.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema"}) @Data
    <Serializable>
    Public MustInherit Class BaseTransform
        Implements TransformT

        Protected Friend inputSchemaField As SchemaT

        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Get
                Return inputSchemaField
            End Get
            Set(ByVal value As SchemaT)
                Me.inputSchemaField = value
            End Set
        End Property

        ''' <summary>
        ''' Get the output schema for this transformation, given an input schema
        ''' </summary>
        Public MustOverride Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform

        Public MustOverride Function columnName() As String Implements ColumnOp.columnName

        Public MustOverride Function columnNames() As String() Implements ColumnOp.columnNames

        Public MustOverride Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames

        Public MustOverride Function outputColumnName() As String Implements ColumnOp.outputColumnName

        Public MustOverride Function mapSequence(ByVal sequence As Object) As Object Implements TransformT.mapSequence

        Public MustOverride Function map(ByVal input As Object) As Object Implements TransformT.map

        Public MustOverride Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT) Implements TransformT.map

        Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of WritableT)) Implements TransformT.mapSequence
            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))(sequence.Count)
            For Each c As IList(Of WritableT) In sequence
                [out].Add(map(c))
            Next c
            Return [out]
        End Function

        Public MustOverride Overrides Function ToString() As String

        ''' <summary>Lombok <c>@Data</c> equality: same class and equal input schemas.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Return Objects.equals(inputSchemaField, DirectCast(obj, BaseTransform).inputSchemaField)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(inputSchemaField)
        End Function

    End Class

End Namespace
