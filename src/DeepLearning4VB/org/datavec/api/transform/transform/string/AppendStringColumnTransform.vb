Imports System
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports StringMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.StringMetaData
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.string

    ''' <summary>
    ''' Append a String to the values in a single column.
    ''' Equality compares the class, the column name and the appended string.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema", "columnNumber"}) @Data
    <JsonIgnoreProperties("inputSchema", "columnNumber")>
    <Serializable>
    Public Class AppendStringColumnTransform
        Inherits BaseColumnTransform

        Private toAppend As String

        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal toAppend As String)
            MyBase.New(columnName)
            Me.toAppend = toAppend
        End Sub

        ''' <summary>Java <c>getToAppend()</c> (Lombok).</summary>
        Public Function getToAppend() As String
            Return toAppend
        End Function

        ''' <summary>Java <c>setToAppend()</c> (Lombok).</summary>
        Public Sub setToAppend(ByVal toAppend As String)
            Me.toAppend = toAppend
        End Sub

        Public Overrides Function getNewColumnMetaData(ByVal newName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
            Return New StringMetaData(newName) 'Output after transform: String (Text)
        End Function

        Public Overrides Function map(ByVal columnWritable As WritableT) As WritableT
            Return New Text(columnWritable.ToString() & toAppend)
        End Function

        Public Overrides Function ToString() As String
            Return "AppendStringColumnTransform(append=""" & toAppend & """)"
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Return input.ToString() & toAppend
        End Function

        ''' <summary>Equal when the class, the column name and the appended string are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return MyBase.Equals(obj) AndAlso String.Equals(toAppend, DirectCast(obj, AppendStringColumnTransform).toAppend)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), toAppend)
        End Function

    End Class

End Namespace
