Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports CategoricalMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.CategoricalMetaData
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.categorical

    ''' <summary>
    ''' Converts a string column to a categorical column with the given state names; the values are unchanged.
    ''' Equality compares the class, the column name and the state names.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema", "columnNumber"}) @Data
    <JsonIgnoreProperties("inputSchema", "columnNumber")>
    <Serializable>
    Public Class StringToCategoricalTransform
        Inherits BaseColumnTransform

        Private ReadOnly stateNames As IList(Of String)

        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal stateNames As IList(Of String))
            MyBase.New(columnName)
            If stateNames Is Nothing OrElse stateNames.Count = 0 Then
                Throw New ArgumentException("State names must not be null or empty")
            End If
            Me.stateNames = stateNames
        End Sub

        Public Sub New(ByVal columnName As String, ByVal ParamArray stateNames() As String)
            Me.New(columnName, Arrays.asList(stateNames))
        End Sub

        ''' <summary>Java <c>getStateNames()</c> (Lombok).</summary>
        Public Function getStateNames() As IList(Of String)
            Return stateNames
        End Function

        Public Overrides Function getNewColumnMetaData(ByVal newColumnName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
            Return New CategoricalMetaData(newColumnName, stateNames)
        End Function

        Public Overrides Function map(ByVal columnWritable As WritableT) As WritableT
            Return columnWritable
        End Function

        Public Overrides Function ToString() As String
            Return "StringToCategoricalTransform(stateNames=" & javaToString(stateNames) & ")"
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Return input
        End Function

        ''' <summary>
        ''' Transform a sequence
        ''' </summary>
        ''' <param name="sequence"> </param>
        Public Overrides Function mapSequence(ByVal sequence As Object) As Object
            Return sequence
        End Function

        ''' <summary>Equal when the class, the column name and the state names are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return MyBase.Equals(obj) AndAlso javaEquals(stateNames, DirectCast(obj, StringToCategoricalTransform).stateNames)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), stateNames)
        End Function

    End Class

End Namespace
