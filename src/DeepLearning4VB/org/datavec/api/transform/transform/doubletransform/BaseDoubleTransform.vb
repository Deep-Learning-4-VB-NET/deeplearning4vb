Imports System
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports DoubleMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.DoubleMetaData

Namespace org.datavec.api.transform.transforms.doubletransform

    ''' <summary>
    ''' Abstract double transformation (single column): the new column keeps the old double metadata (renamed), or
    ''' gets unrestricted double metadata. Equality (Lombok <c>callSuper = true</c>, no own fields) is the base class's.
    ''' </summary>
    ' Java: @EqualsAndHashCode(callSuper = true) @Data @NoArgsConstructor
    <Serializable>
    Public MustInherit Class BaseDoubleTransform
        Inherits BaseColumnTransform

        ''' <summary>Lombok <c>@NoArgsConstructor</c> (for JSON deserialization).</summary>
        Protected Sub New()
        End Sub

        Public Sub New(ByVal column As String)
            MyBase.New(column)
        End Sub

        Public Overrides Function getNewColumnMetaData(ByVal newColumnName As String, ByVal oldColumnMeta As ColumnMetaData) As ColumnMetaData
            If TypeOf oldColumnMeta Is DoubleMetaData Then
                Dim meta As ColumnMetaData = oldColumnMeta.clone()
                meta.Name = newColumnName
                Return meta
            Else
                Return New DoubleMetaData(newColumnName)
            End If
        End Function

    End Class

End Namespace
