Imports System
Imports System.Text.Json.Serialization
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.string

    ''' <summary>
    ''' Change case (to, e.g, all lower case) of String column. Case conversion is culture invariant (Java used the
    ''' default locale).
    ''' Equality compares the class, the column name and the case type.
    ''' </summary>
    ' Java: @Data
    <Serializable>
    Public Class ChangeCaseStringTransform
        Inherits BaseStringTransform

        <JsonConverter(GetType(JsonStringEnumConverter))>
        Public Enum CaseType
            LOWER
            UPPER
        End Enum

        Private ReadOnly caseTypeField As CaseType

        Public Sub New(ByVal column As String)
            MyBase.New(column)
            Me.caseTypeField = CaseType.LOWER ' default is all lower case
        End Sub

        <JsonConstructor>
        Public Sub New(ByVal column As String, ByVal caseType As CaseType)
            MyBase.New(column)
            Me.caseTypeField = caseType
        End Sub

        ''' <summary>Java <c>getCaseType()</c> (Lombok).</summary>
        Public Function getCaseType() As CaseType
            Return caseTypeField
        End Function

        Private Function mapHelper(ByVal input As String) As String
            Dim result As String
            Select Case caseTypeField
                Case CaseType.UPPER
                    result = input.ToUpperInvariant()
                Case Else
                    result = input.ToLowerInvariant()
            End Select
            Return result
        End Function

        Public Overrides Function map(ByVal writable As WritableT) As WritableT
            Return New Text(mapHelper(writable.ToString()))
        End Function

        Public Overrides Function map(ByVal input As Object) As Object
            Return mapHelper(input.ToString())
        End Function

        ''' <summary>Lombok <c>toString()</c>.</summary>
        Public Overrides Function ToString() As String
            Return "ChangeCaseStringTransform(caseType=" & caseTypeField.ToString() & ")"
        End Function

        ''' <summary>Equal when the class, the column name and the case type are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return MyBase.Equals(obj) AndAlso caseTypeField = DirectCast(obj, ChangeCaseStringTransform).caseTypeField
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), caseTypeField)
        End Function

    End Class

End Namespace
