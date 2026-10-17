Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.string

    ''' <summary>
    ''' A simple String -> String map function.
    '''
    ''' Keys in the map are the original values; the Values in the map are their replacements.
    ''' If a String appears in the data but does not appear in the provided map (as a key), that String values will
    ''' not be modified.
    ''' <para>Equality (Lombok <c>callSuper = true</c>) compares the class, the column name and the map.</para>
    ''' </summary>
    ' Java: @EqualsAndHashCode(callSuper = true) @Data
    <Serializable>
    Public Class StringMapTransform
        Inherits BaseStringTransform

        Private ReadOnly mapField As IDictionary(Of String, String)

        ''' <param name="columnName"> Name of the column </param>
        ''' <param name="map"> Key: From. Value: To </param>
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
            Dim orig As String = writable.ToString()
            If orig IsNot Nothing AndAlso mapField.ContainsKey(orig) Then
                Return New Text(mapField(orig))
            End If

            If TypeOf writable Is Text Then
                Return DirectCast(writable, Text)
            Else
                Return New Text(writable.ToString())
            End If
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Dim orig As String = input.ToString()
            If orig IsNot Nothing AndAlso mapField.ContainsKey(orig) Then
                Return mapField(orig)
            End If

            If TypeOf input Is String Then
                Return input
            Else
                Return orig
            End If
        End Function

        ''' <summary>Lombok <c>toString()</c>.</summary>
        Public Overrides Function ToString() As String
            Return "StringMapTransform(map=" & javaToString(mapField) & ")"
        End Function

        ''' <summary>Equal when the class, the column name and the map are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return MyBase.Equals(obj) AndAlso javaEquals(mapField, DirectCast(obj, StringMapTransform).mapField)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), mapField)
        End Function

    End Class

End Namespace
