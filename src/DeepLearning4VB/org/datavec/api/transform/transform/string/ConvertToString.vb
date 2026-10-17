Imports System
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.string

    ''' <summary>
    ''' Convert any value to a string.
    ''' Equality compares the class and the column name.
    ''' </summary>
    ' Java: @NoArgsConstructor @Data
    <Serializable>
    Public Class ConvertToString
        Inherits BaseStringTransform

        ''' <summary>Lombok <c>@NoArgsConstructor</c>.</summary>
        Public Sub New()
        End Sub

        Public Sub New(ByVal column As String)
            MyBase.New(column)
        End Sub

        ''' <summary>
        ''' Transform the writable in to a
        ''' string
        ''' </summary>
        ''' <param name="writable"> the writable to transform </param>
        ''' <returns> the string form of this writable </returns>
        Public Overrides Function map(ByVal writable As WritableT) As WritableT
            Return New Text(writable.ToString())
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Return input.ToString()
        End Function

        ''' <summary>Lombok <c>toString()</c>.</summary>
        Public Overrides Function ToString() As String
            Return "ConvertToString()"
        End Function

    End Class

End Namespace
