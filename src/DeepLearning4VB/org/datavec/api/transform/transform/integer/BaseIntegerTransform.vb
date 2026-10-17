Imports System
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData

Namespace org.datavec.api.transform.transforms.integer

    ''' <summary>
    ''' Abstract integer transformation (single column): the new column keeps the old metadata (renamed).
    ''' Equality (Lombok <c>callSuper = true</c>, no own fields) is the base class's.
    ''' </summary>
    ' Java: @EqualsAndHashCode(callSuper = true) @Data @NoArgsConstructor
    <Serializable>
    Public MustInherit Class BaseIntegerTransform
        Inherits BaseColumnTransform

        ''' <summary>Lombok <c>@NoArgsConstructor</c> (for JSON deserialization).</summary>
        Protected Sub New()
        End Sub

        Public Sub New(ByVal column As String)
            MyBase.New(column)
        End Sub

        Public Overrides Function getNewColumnMetaData(ByVal newName As String, ByVal oldColumnMeta As ColumnMetaData) As ColumnMetaData
            Dim meta As ColumnMetaData = oldColumnMeta.clone()
            meta.Name = newName
            Return meta
        End Function

    End Class

End Namespace
