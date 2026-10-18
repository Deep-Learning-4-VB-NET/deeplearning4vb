Imports System
Imports System.Collections.Generic
Imports System.Runtime.Serialization
Imports System.Text
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports DateTimeFieldType = DeepLearning4VB.org.joda.time.DateTimeFieldType
Imports DateTimeFormat = DeepLearning4VB.org.joda.time.format.DateTimeFormat
Imports DateTimeFormatter = DeepLearning4VB.org.joda.time.format.DateTimeFormatter
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports IntegerMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.IntegerMetaData
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports JodaDateTime = DeepLearning4VB.org.joda.time.DateTime
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports StringMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.StringMetaData
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports TimeMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.TimeMetaData
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.time

    ''' <summary>
    ''' Create a number of new columns by deriving their values from a Time column.
    ''' Can be used for example to create new columns with the year, month, day, hour, minute, second etc.
    ''' <para>Equality (Lombok, excluding inputSchema and the column indexes) compares the class, the time column, the
    ''' insert-after column, the input time zone and the derived columns.</para>
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema", "insertAfterIdx", "deriveFromIdx"})
    '       @EqualsAndHashCode(exclude = {"inputSchema", "insertAfterIdx", "deriveFromIdx"}) @Data
    <JsonIgnoreProperties("inputSchema", "insertAfterIdx", "deriveFromIdx")>
    <Serializable>
    Public Class DeriveColumnsFromTimeTransform
        Implements TransformT

        Private ReadOnly columnNameField As String
        Private ReadOnly insertAfter As String
        Private inputTimeZone As DateTimeZone
        Private ReadOnly derivedColumns As IList(Of DerivedColumn)
        Private inputSchemaField As SchemaT
        Private insertAfterIdx As Integer = -1
        Private deriveFromIdx As Integer = -1

        Private Sub New(ByVal builder As Builder)
            Me.derivedColumns = builder.derivedColumns
            Me.columnNameField = builder.columnName
            Me.insertAfter = builder.insertAfterField
        End Sub

        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal insertAfter As String, ByVal inputTimeZone As DateTimeZone, ByVal derivedColumns As IList(Of DerivedColumn))
            Me.columnNameField = columnName
            Me.insertAfter = insertAfter
            Me.inputTimeZone = inputTimeZone
            Me.derivedColumns = derivedColumns
        End Sub

        ''' <summary>Java <c>getColumnName()</c> (Lombok).</summary>
        Public Function getColumnName() As String
            Return columnNameField
        End Function

        ''' <summary>Java <c>getInsertAfter()</c> (Lombok).</summary>
        Public Function getInsertAfter() As String
            Return insertAfter
        End Function

        ''' <summary>Java <c>getInputTimeZone()</c> (Lombok): set from the time column's metadata with the input schema.</summary>
        Public Function getInputTimeZone() As DateTimeZone
            Return inputTimeZone
        End Function

        ''' <summary>Java <c>setInputTimeZone()</c> (Lombok).</summary>
        Public Sub setInputTimeZone(ByVal inputTimeZone As DateTimeZone)
            Me.inputTimeZone = inputTimeZone
        End Sub

        ''' <summary>Java <c>getDerivedColumns()</c> (Lombok).</summary>
        Public Function getDerivedColumns() As IList(Of DerivedColumn)
            Return derivedColumns
        End Function

        ''' <summary>Java <c>getInsertAfterIdx()</c> (Lombok).</summary>
        Public Function getInsertAfterIdx() As Integer
            Return insertAfterIdx
        End Function

        ''' <summary>Java <c>getDeriveFromIdx()</c> (Lombok).</summary>
        Public Function getDeriveFromIdx() As Integer
            Return deriveFromIdx
        End Function

        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            Dim oldMeta As IList(Of ColumnMetaData) = inputSchema.getColumnMetaData()
            Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(oldMeta.Count + derivedColumns.Count)

            Dim oldNames As IList(Of String) = inputSchema.getColumnNames()

            For i As Integer = 0 To oldMeta.Count - 1
                Dim current As String = oldNames(i)
                newMeta.Add(oldMeta(i))

                If insertAfter.Equals(current) Then
                    'Insert the derived columns here
                    For Each d As DerivedColumn In derivedColumns
                        Select Case d.getColumnType()
                            Case ColumnType.String
                                newMeta.Add(New StringMetaData(d.getColumnName()))
                            Case ColumnType.Integer
                                newMeta.Add(New IntegerMetaData(d.getColumnName())) 'TODO: ranges... if it's a day, we know it must be 1 to 31, etc...
                            Case Else
                                Throw New InvalidOperationException("Unexpected column type: " & d.getColumnType().ToString())
                        End Select
                    Next d
                End If
            Next i

            Return inputSchema.newSchema(newMeta)
        End Function

        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Set(ByVal value As SchemaT)
                insertAfterIdx = value.getColumnNames().IndexOf(insertAfter)
                If insertAfterIdx = -1 Then
                    Throw New InvalidOperationException("Invalid schema/insert after column: input schema does not contain column """ & insertAfter & """")
                End If

                deriveFromIdx = value.getColumnNames().IndexOf(columnNameField)
                If deriveFromIdx = -1 Then
                    Throw New InvalidOperationException("Invalid source column: input schema does not contain column """ & columnNameField & """")
                End If

                Me.inputSchemaField = value

                If Not (TypeOf value.getMetaData(columnNameField) Is TimeMetaData) Then
                    Throw New InvalidOperationException("Invalid state: input column """ & columnNameField & """ is not a time column. Is: " & javaToString(value.getMetaData(columnNameField)))
                End If
                Dim meta As TimeMetaData = DirectCast(value.getMetaData(columnNameField), TimeMetaData)
                inputTimeZone = meta.getTimeZone()
            End Set
            Get
                Return inputSchemaField
            End Get
        End Property

        Public Overridable Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT) Implements TransformT.map
            If writables.Count <> inputSchemaField.numColumns() Then
                Throw New InvalidOperationException(badRecordLengthMessage(writables.Count, inputSchemaField.numColumns(), Me))
            End If

            Dim i As Integer = 0
            Dim source As WritableT = writables(deriveFromIdx)
            Dim list As IList(Of WritableT) = New List(Of WritableT)(writables.Count + derivedColumns.Count)
            For Each w As WritableT In writables
                list.Add(w)
                If i = insertAfterIdx Then
                    For Each d As DerivedColumn In derivedColumns
                        list.Add(derive(d, source.toLong()))
                    Next d
                End If
                i += 1
            Next w
            Return list
        End Function

        Private Function derive(ByVal d As DerivedColumn, ByVal instant As Long) As WritableT
            Select Case d.getColumnType()
                Case ColumnType.String
                    Return New Text(d.getDateTimeFormatter().print(instant))
                Case ColumnType.Integer
                    Dim dt As New JodaDateTime(instant, inputTimeZone)
                    Return New IntWritable(dt.get(d.getFieldType()))
                Case Else
                    Throw New InvalidOperationException("Unexpected column type: " & d.getColumnType().ToString())
            End Select
        End Function

        Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of WritableT)) Implements TransformT.mapSequence
            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))(sequence.Count)
            For Each [step] As IList(Of WritableT) In sequence
                [out].Add(map([step]))
            Next [step]
            Return [out]
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object: a time (a boxed Long of epoch milliseconds) becomes the list of derived values
        ''' (Strings and Integers)
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overridable Function map(ByVal input As Object) As Object Implements TransformT.map
            Dim ret As IList(Of Object) = New List(Of Object)()
            Dim l As Long = DirectCast(input, Long)
            For Each d As DerivedColumn In derivedColumns
                Select Case d.getColumnType()
                    Case ColumnType.String
                        ret.Add(d.getDateTimeFormatter().print(l))
                    Case ColumnType.Integer
                        Dim dt As New JodaDateTime(l, inputTimeZone)
                        ret.Add(dt.get(d.getFieldType()))
                    Case Else
                        Throw New InvalidOperationException("Unexpected column type: " & d.getColumnType().ToString())
                End Select
            Next d
            Return ret
        End Function

        ''' <summary>
        ''' Transform a sequence: each time (a boxed Long) becomes its list of derived values
        ''' </summary>
        ''' <param name="sequence"> </param>
        Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements TransformT.mapSequence
            Dim longs As System.Collections.IList = DirectCast(sequence, System.Collections.IList)
            Dim ret As IList(Of IList(Of Object)) = New List(Of IList(Of Object))()
            For Each l As Object In longs
                ret.Add(DirectCast(map(l), IList(Of Object)))
            Next l
            Return ret
        End Function

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.Append("DeriveColumnsFromTimeTransform(timeColumn=""").Append(columnNameField).Append(""",insertAfter=""").Append(insertAfter).Append(""",derivedColumns=(")

            Dim first As Boolean = True
            For Each d As DerivedColumn In derivedColumns
                If Not first Then
                    sb.Append(",")
                End If
                sb.Append(d.ToString())
                first = False
            Next d

            sb.Append("))")

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' The output column name
        ''' after the operation has been applied
        ''' </summary>
        ''' <returns> the output column name </returns>
        Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
            Return outputColumnNames()(0)
        End Function

        ''' <summary>
        ''' The output column names
        ''' This will often be the same as the input
        ''' </summary>
        ''' <returns> the output column names </returns>
        Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
            Dim ret(derivedColumns.Count - 1) As String
            For i As Integer = 0 To ret.Length - 1
                ret(i) = derivedColumns(i).getColumnName()
            Next i
            Return ret
        End Function

        ''' <summary>
        ''' Returns column names
        ''' this op is meant to run on
        ''' </summary>
        Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
            Return New String() {columnName()}
        End Function

        ''' <summary>
        ''' Returns a singular column name
        ''' this op is meant to run on
        ''' </summary>
        Public Overridable Function columnName() As String Implements ColumnOp.columnName
            Return columnNameField
        End Function

        ''' <summary>Lombok equality (excluding inputSchema and the column indexes).</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim o As DeriveColumnsFromTimeTransform = DirectCast(obj, DeriveColumnsFromTimeTransform)
            Return String.Equals(columnNameField, o.columnNameField) AndAlso String.Equals(insertAfter, o.insertAfter) AndAlso
                Objects.equals(inputTimeZone, o.inputTimeZone) AndAlso javaEquals(derivedColumns, o.derivedColumns)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(columnNameField, insertAfter, inputTimeZone, derivedColumns)
        End Function

        ''' <summary>Builder for a <see cref="DeriveColumnsFromTimeTransform"/>.</summary>
        Public Class Builder

            Friend ReadOnly columnName As String
            Friend insertAfterField As String
            Friend ReadOnly derivedColumns As IList(Of DerivedColumn) = New List(Of DerivedColumn)()

            ''' <param name="timeColumnName"> The name of the time column from which to derive the new values </param>
            Public Sub New(ByVal timeColumnName As String)
                Me.columnName = timeColumnName
                Me.insertAfterField = timeColumnName
            End Sub

            ''' <summary>
            ''' Where should the new columns be inserted?
            ''' By default, they will be inserted after the source column
            ''' </summary>
            ''' <param name="columnName"> Name of the column to insert the derived columns after </param>
            Public Overridable Function insertAfter(ByVal columnName As String) As Builder
                Me.insertAfterField = columnName
                Return Me
            End Function

            ''' <summary>
            ''' Add a String column (for example, human readable format), derived from the time
            ''' </summary>
            ''' <param name="columnName"> Name of the new/derived column </param>
            ''' <param name="format">     Joda time format, as per <a href="http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html">http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html</a> </param>
            ''' <param name="timeZone">   Timezone to use for formatting </param>
            Public Overridable Function addStringDerivedColumn(ByVal columnName As String, ByVal format As String, ByVal timeZone As DateTimeZone) As Builder
                derivedColumns.Add(New DerivedColumn(columnName, ColumnType.String, format, timeZone, Nothing))
                Return Me
            End Function

            ''' <summary>
            ''' Add an integer derived column - for example, the hour of day, etc. Uses timezone from the time column metadata
            ''' </summary>
            ''' <param name="columnName"> Name of the column </param>
            ''' <param name="type">       Type of field (for example, DateTimeFieldType.hourOfDay() etc) </param>
            Public Overridable Function addIntegerDerivedColumn(ByVal columnName As String, ByVal type As DateTimeFieldType) As Builder
                derivedColumns.Add(New DerivedColumn(columnName, ColumnType.Integer, Nothing, Nothing, type))
                Return Me
            End Function

            ''' <summary>
            ''' Create the transform instance
            ''' </summary>
            Public Overridable Function build() As DeriveColumnsFromTimeTransform
                Return New DeriveColumnsFromTimeTransform(Me)
            End Function

        End Class

        ''' <summary>
        ''' A column derived from the time: either a String (the time printed with a Joda format in a zone) or an
        ''' Integer (a field of the time, such as the hour of day). The formatter is not serialized; it is rebuilt after
        ''' deserialization (Java <c>readObject</c>). Equality (Lombok, excluding the formatter) compares all other fields.
        ''' </summary>
        ' Java: @JsonInclude(NON_NULL) @EqualsAndHashCode(exclude = "dateTimeFormatter") @Data @JsonIgnoreProperties({"dateTimeFormatter"})
        <JsonIgnoreProperties("dateTimeFormatter")>
        <JsonIncludeNonNull>
        <Serializable>
        Public Class DerivedColumn

            Private ReadOnly columnNameField As String
            Private ReadOnly columnTypeField As ColumnType
            Private ReadOnly format As String
            Private ReadOnly dateTimeZone As DateTimeZone
            ' Java: @JsonSerialize/@JsonDeserialize with DateTimeFieldTypeSerializer/Deserializer
            Private ReadOnly fieldType As DateTimeFieldType
            <NonSerialized>
            Private dateTimeFormatter As DateTimeFormatter

            <JsonConstructor>
            Public Sub New(ByVal columnName As String, ByVal columnType As ColumnType, ByVal format As String, ByVal dateTimeZone As DateTimeZone, ByVal fieldType As DateTimeFieldType)
                Me.columnNameField = columnName
                Me.columnTypeField = columnType
                Me.format = format
                Me.dateTimeZone = dateTimeZone
                Me.fieldType = fieldType
                initFormatter()
            End Sub

            Private Sub initFormatter()
                If format IsNot Nothing Then
                    dateTimeFormatter = DateTimeFormat.forPattern(Me.format).withZone(dateTimeZone)
                End If
            End Sub

            ''' <summary>Java <c>readObject</c>: the formatter is not serialized and is rebuilt here.</summary>
            <OnDeserialized>
            Private Sub onDeserialized(ByVal context As StreamingContext)
                initFormatter()
            End Sub

            ''' <summary>Java <c>getColumnName()</c> (Lombok).</summary>
            Public Function getColumnName() As String
                Return columnNameField
            End Function

            ''' <summary>Java <c>getColumnType()</c> (Lombok).</summary>
            Public Function getColumnType() As ColumnType
                Return columnTypeField
            End Function

            ''' <summary>Java <c>getFormat()</c> (Lombok).</summary>
            Public Function getFormat() As String
                Return format
            End Function

            ''' <summary>Java <c>getDateTimeZone()</c> (Lombok).</summary>
            Public Function getDateTimeZone() As DateTimeZone
                Return dateTimeZone
            End Function

            ''' <summary>Java <c>getFieldType()</c> (Lombok).</summary>
            Public Function getFieldType() As DateTimeFieldType
                Return fieldType
            End Function

            ''' <summary>Java <c>getDateTimeFormatter()</c> (Lombok).</summary>
            Public Function getDateTimeFormatter() As DateTimeFormatter
                Return dateTimeFormatter
            End Function

            Public Overrides Function ToString() As String
                Return "(name=" & columnNameField & ",type=" & columnTypeField.ToString() & ",derived=" & (If(format IsNot Nothing, format, javaToString(fieldType))) & ")"
            End Function

            Public Overrides Function Equals(ByVal obj As Object) As Boolean
                If obj Is Me Then Return True
                If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
                Dim o As DerivedColumn = DirectCast(obj, DerivedColumn)
                Return String.Equals(columnNameField, o.columnNameField) AndAlso columnTypeField = o.columnTypeField AndAlso
                    String.Equals(format, o.format) AndAlso Objects.equals(dateTimeZone, o.dateTimeZone) AndAlso Objects.equals(fieldType, o.fieldType)
            End Function

            Public Overrides Function GetHashCode() As Integer
                Return lombokHash(columnNameField, columnTypeField, format, dateTimeZone, fieldType)
            End Function

        End Class

    End Class

End Namespace
