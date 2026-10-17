Imports System
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports StringMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.StringMetaData

Namespace org.datavec.api.transform.transforms.string

    ''' <summary>
    ''' Abstract String column transform: the output column is a string column, and <c>map(Writable)</c> returns a
    ''' <c>Text</c> (Java declared the covariant return type <c>Text</c>; VB overrides keep <c>Writable</c>).
    ''' Equality (Lombok <c>callSuper = true</c>, no own fields) is the base class's.
    ''' </summary>
    ' Java: @EqualsAndHashCode(callSuper = true) @Data @NoArgsConstructor
    <Serializable>
    Public MustInherit Class BaseStringTransform
        Inherits BaseColumnTransform

        ''' <summary>Lombok <c>@NoArgsConstructor</c> (for JSON deserialization).</summary>
        Protected Sub New()
        End Sub

        Public Sub New(ByVal column As String)
            MyBase.New(column)
        End Sub

        Public Overrides Function getNewColumnMetaData(ByVal newName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
            Return New StringMetaData(newName)
        End Function

    End Class

End Namespace
