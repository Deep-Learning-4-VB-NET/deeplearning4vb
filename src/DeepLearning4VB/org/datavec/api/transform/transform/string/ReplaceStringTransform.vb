Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.string

    ''' <summary>
    ''' Replaces String values that match regular expressions (Java <c>String.replaceAll</c> semantics, applied for
    ''' each map entry in iteration order).
    ''' Equality (Lombok <c>callSuper = true</c>) compares the class, the column name and the map.
    ''' </summary>
    ' Java: @EqualsAndHashCode(callSuper = true) @Data
    <Serializable>
    Public Class ReplaceStringTransform
        Inherits BaseStringTransform

        Private ReadOnly mapField As IDictionary(Of String, String)

        ''' <summary>
        ''' Constructs a new ReplaceStringTransform using the specified
        ''' </summary>
        ''' <param name="columnName"> Name of the column </param>
        ''' <param name="map"> Key: regular expression; Value: replacement value </param>
        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal map As IDictionary(Of String, String))
            MyBase.New(columnName)
            Me.mapField = map
        End Sub

        ''' <summary>Java <c>getMap()</c> (Lombok).</summary>
        Public Function getMap() As IDictionary(Of String, String)
            Return mapField
        End Function

        Public Overrides Function map(ByVal writable As WritableT) As WritableT
            Dim value As String = writable.ToString()
            value = replaceAll(value)
            Return New Text(value)
        End Function

        Public Overrides Function map(ByVal o As Object) As Object
            Dim value As String = o.ToString()
            value = replaceAll(value)
            Return value
        End Function

        Private Function replaceAll(ByVal value As String) As String
            If mapField IsNot Nothing AndAlso mapField.Count > 0 Then
                For Each entry As KeyValuePair(Of String, String) In mapField
                    value = javaReplaceAll(value, entry.Key, entry.Value)
                Next entry
            End If
            Return value
        End Function

        ''' <summary>Lombok <c>toString()</c>.</summary>
        Public Overrides Function ToString() As String
            Return "ReplaceStringTransform(map=" & javaToString(mapField) & ")"
        End Function

        ''' <summary>Equal when the class, the column name and the map are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return MyBase.Equals(obj) AndAlso javaEquals(mapField, DirectCast(obj, ReplaceStringTransform).mapField)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), mapField)
        End Function

    End Class

End Namespace
