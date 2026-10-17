Imports System
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports FloatMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.FloatMetaData

Namespace org.datavec.api.transform.transforms.floattransform

    ''' <summary>
    ''' Abstract float transformation (single column): the new column keeps the old float metadata (renamed), or gets
    ''' unrestricted float metadata. Equality (Lombok <c>callSuper = true</c>, no own fields) is the base class's.
    ''' </summary>
    ' Java: @EqualsAndHashCode(callSuper = true) @Data @NoArgsConstructor
    <Serializable>
    Public MustInherit Class BaseFloatTransform
        Inherits BaseColumnTransform

        ''' <summary>Lombok <c>@NoArgsConstructor</c> (for JSON deserialization).</summary>
        Protected Sub New()
        End Sub

        Public Sub New(ByVal column As String)
            MyBase.New(column)
        End Sub

        Public Overrides Function getNewColumnMetaData(ByVal newColumnName As String, ByVal oldColumnMeta As ColumnMetaData) As ColumnMetaData
            If TypeOf oldColumnMeta Is FloatMetaData Then
                Dim meta As ColumnMetaData = oldColumnMeta.clone()
                meta.Name = newColumnName
                Return meta
            Else
                Return New FloatMetaData(newColumnName)
            End If
        End Function

    End Class

End Namespace
