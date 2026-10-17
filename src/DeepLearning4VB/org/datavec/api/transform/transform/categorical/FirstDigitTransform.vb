Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports CategoricalMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.CategoricalMetaData
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.categorical

    ''' <summary>
    ''' FirstDigitTransform converts a column to a categorical column, with values being the first digit of the number.<br/>
    ''' For example, "3.1415" becomes "3" and "2.0" becomes "2".<br/>
    ''' Negative numbers ignore the sign: "-7.123" becomes "7".<br/>
    ''' Note that two {@link Mode}s are supported, which determines how non-numerical entries should be handled:<br/>
    ''' EXCEPTION_ON_INVALID: output has 10 category values ("0", ..., "9"), and any non-numerical values result in an exception<br/>
    ''' INCLUDE_OTHER_CATEGORY: output has 11 category values ("0", ..., "9", "Other"), all non-numerical values are mapped to "Other"<br/>
    ''' <br/>
    ''' FirstDigitTransform is useful (combined with CategoricalToOneHotTransform and Reductions) to implement
    ''' <a href="https://en.wikipedia.org/wiki/Benford%27s_law">Benford's law</a>.
    ''' <para>Equality compares the class, the input and output columns and the mode (Java inherited the Lombok
    ''' equality of <c>BaseTransform</c>, which only compares the input schemas).</para>
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema", "columnIdx"})
    <Serializable>
    Public Class FirstDigitTransform
        Inherits BaseTransform

        Public Const OTHER_CATEGORY As String = "Other"

        ''' <summary>
        ''' Mode determines how non-numerical entries should be handled:<br/>
        ''' EXCEPTION_ON_INVALID: output has 10 category values ("0", ..., "9"), and any non-numerical values result in an exception<br/>
        ''' INCLUDE_OTHER_CATEGORY: output has 11 category values ("0", ..., "9", "Other"), all non-numerical values are mapped to "Other"<br/>
        ''' </summary>
        <JsonConverter(GetType(JsonStringEnumConverter))>
        Public Enum Mode
            EXCEPTION_ON_INVALID
            INCLUDE_OTHER_CATEGORY
        End Enum

        Protected Friend inputColumn As String
        Protected Friend outputColumn As String
        Protected Friend modeField As Mode
        Private columnIdx As Integer = -1

        ''' <param name="inputColumn">  Input column name </param>
        ''' <param name="outputColumn"> Output column name. If same as input, input column is replaced </param>
        ''' <param name="mode"> See <seealso cref="FirstDigitTransform.Mode"/> </param>
        <JsonConstructor>
        Public Sub New(ByVal inputColumn As String, ByVal outputColumn As String, ByVal mode As Mode)
            Me.inputColumn = inputColumn
            Me.outputColumn = outputColumn
            Me.modeField = mode
        End Sub

        Public Overrides Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT)
            Dim [out] As IList(Of WritableT) = New List(Of WritableT)()
            Dim i As Integer = 0
            Dim inplace As Boolean = inputColumn.Equals(outputColumn)
            For Each w As WritableT In writables
                If i = columnIdx Then
                    If Not inplace Then
                        [out].Add(w)
                    End If

                    Dim s As String = w.ToString()
                    If s.Length = 0 Then
                        If modeField = Mode.INCLUDE_OTHER_CATEGORY Then
                            [out].Add(New Text(OTHER_CATEGORY))
                        Else
                            Throw New InvalidOperationException("Encountered empty string in FirstDigitTransform that is set to Mode.EXCEPTION_ON_INVALID." & " Either data contains an invalid (non-numerical) entry, or set FirstDigitTransform to Mode.INCLUDE_OTHER_CATEGORY")
                        End If
                    Else
                        Dim first As Char = s.Chars(0)
                        If first = "-"c AndAlso s.Length > 1 Then
                            'Handle negatives
                            first = s.Chars(1)
                        End If
                        If first >= "0"c AndAlso first <= "9"c Then
                            [out].Add(New Text(first.ToString()))
                        Else
                            If modeField = Mode.INCLUDE_OTHER_CATEGORY Then
                                [out].Add(New Text(OTHER_CATEGORY))
                            Else
                                Dim s2 As String = s
                                If s.Length > 100 Then
                                    s2 = s2.Substring(0, 100) & "..."
                                End If
                                Throw New InvalidOperationException("Encountered string """ & s2 & """ with non-numerical first character in " & "FirstDigitTransform that is set to Mode.EXCEPTION_ON_INVALID." & " Either data contains an invalid (non-numerical) entry, or set FirstDigitTransform to Mode.INCLUDE_OTHER_CATEGORY")
                            End If
                        End If
                    End If
                Else
                    [out].Add(w)
                End If
                i += 1
            Next w
            Return [out]
        End Function

        Public Overrides Function map(ByVal input As Object) As Object
            Throw New NotSupportedException("Not supported")
        End Function

        Public Overrides Function mapSequence(ByVal sequence As Object) As Object
            Throw New NotSupportedException("Not supported")
        End Function

        Public Overrides Function ToString() As String
            Return "FirstDigitTransform(input=""" & inputColumn & """,output=""" & outputColumn & """,mode=" & modeField.ToString() & ")"
        End Function

        Public Overrides Function transform(ByVal inputSchema As SchemaT) As SchemaT
            Dim origNames As IList(Of String) = inputSchema.getColumnNames()
            Dim origMeta As IList(Of ColumnMetaData) = inputSchema.getColumnMetaData()

            Preconditions.checkState(origNames.Contains(inputColumn), "Input column with name ""%s"" not found in schema", CObj(inputColumn))
            Preconditions.checkState(inputColumn.Equals(outputColumn) OrElse Not origNames.Contains(outputColumn), "Output column with name ""%s"" already exists in schema (only allowable if input column == output column)", CObj(outputColumn))

            Dim outMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(origNames.Count + 1)
            For i As Integer = 0 To origNames.Count - 1
                Dim s As String = origNames(i)
                If s.Equals(inputColumn) Then
                    If Not outputColumn.Equals(inputColumn) Then
                        outMeta.Add(origMeta(i))
                    End If

                    Dim states As New List(Of String) From {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"}
                    If modeField = Mode.INCLUDE_OTHER_CATEGORY Then
                        states.Add(OTHER_CATEGORY)
                    End If
                    Dim cm As New CategoricalMetaData(outputColumn, states.AsReadOnly())
                    outMeta.Add(cm)
                Else
                    outMeta.Add(origMeta(i))
                End If
            Next i

            Return inputSchema.newSchema(outMeta)
        End Function

        Public Overrides Function outputColumnName() As String
            Return outputColumn
        End Function

        Public Overrides Function outputColumnNames() As String()
            Return New String() {outputColumn}
        End Function

        Public Overrides Function columnNames() As String()
            Return New String() {inputColumn}
        End Function

        Public Overrides Function columnName() As String
            Return inputColumn
        End Function

        <JsonIgnore>
        Public Overrides Property InputSchema As SchemaT
            Get
                Return inputSchemaField
            End Get
            Set(ByVal value As SchemaT)
                MyBase.InputSchema = value
                columnIdx = value.getIndexOfColumn(inputColumn)
                Preconditions.checkState(columnIdx >= 0, "Input column ""%s"" not found in schema", CObj(inputColumn))
            End Set
        End Property

        ''' <summary>Equal when the class, the input and output columns and the mode are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim o As FirstDigitTransform = DirectCast(obj, FirstDigitTransform)
            Return String.Equals(inputColumn, o.inputColumn) AndAlso String.Equals(outputColumn, o.outputColumn) AndAlso modeField = o.modeField
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(inputColumn, outputColumn, modeField)
        End Function

    End Class

End Namespace
