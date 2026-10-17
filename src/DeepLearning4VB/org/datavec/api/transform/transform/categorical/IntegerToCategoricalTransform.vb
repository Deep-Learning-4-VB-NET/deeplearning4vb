Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.JavaCompat
Imports CategoricalMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.CategoricalMetaData
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.categorical

    ''' <summary>
    ''' Converts an integer column to a categorical column, using a map from integer values to category names.
    ''' Equality compares the class, the column name and the map.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema", "columnNumber"}) @Data
    <Serializable>
    Public Class IntegerToCategoricalTransform
        Inherits BaseColumnTransform

        Private ReadOnly mapField As IDictionary(Of Integer, String)

        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal map As IDictionary(Of Integer, String))
            MyBase.New(columnName)
            Me.mapField = map
        End Sub

        ''' <param name="columnName"> Name of the column </param>
        ''' <param name="list">       Category names: value i is mapped to list(i) </param>
        Public Sub New(ByVal columnName As String, ByVal list As IList(Of String))
            MyBase.New(columnName)
            Me.mapField = New LinkedHashMap(Of Integer, String)()
            Dim i As Integer = 0
            For Each s As String In list
                mapField(i) = s
                i += 1
            Next s
        End Sub

        ''' <summary>Java <c>getMap()</c> (Lombok).</summary>
        Public Function getMap() As IDictionary(Of Integer, String)
            Return mapField
        End Function

        Public Overrides Function getNewColumnMetaData(ByVal newColumnName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
            Return New CategoricalMetaData(newColumnName, New List(Of String)(mapField.Values))
        End Function

        Public Overrides Function map(ByVal columnWritable As WritableT) As WritableT
            Return New Text(mapField.GetValueOrNull(columnWritable.toInt()))
        End Function

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.Append("IntegerToCategoricalTransform(map=[")
            Dim list As New List(Of Integer)(mapField.Keys)
            list.Sort()
            Dim first As Boolean = True
            For Each i As Integer In list
                If Not first Then
                    sb.Append(",")
                End If
                sb.Append(i).Append("=""").Append(mapField(i)).Append("""")
                first = False
            Next i
            sb.Append("])")
            Return sb.ToString()
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then
                Return True
            End If
            If o Is Nothing OrElse Me.GetType() IsNot o.GetType() Then
                Return False
            End If
            If Not MyBase.Equals(o) Then
                Return False
            End If

            Dim o2 As IntegerToCategoricalTransform = DirectCast(o, IntegerToCategoricalTransform)
            Return javaEquals(mapField, o2.mapField)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = MyBase.GetHashCode()
            result = 31 * result + javaHashCode(mapField)
            Return result
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object: the integer value (a number, or its string form) becomes the category as a
        ''' <c>Text</c>. (Java looked the value's string up in the integer-keyed map, which always failed.)
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Dim key As Integer = If(JavaNumbers.isNumber(input), JavaNumbers.intValue(input), JavaNumbers.parseInt(input.ToString()))
            Return New Text(mapField.GetValueOrNull(key))
        End Function

        ''' <summary>
        ''' Transform a sequence: each value is mapped with <c>map(Object)</c>. (Java cast the results to integer
        ''' lists, which always failed.)
        ''' </summary>
        ''' <param name="sequence"> </param>
        Public Overrides Function mapSequence(ByVal sequence As Object) As Object
            Return MyBase.mapSequence(sequence)
        End Function

    End Class

End Namespace
