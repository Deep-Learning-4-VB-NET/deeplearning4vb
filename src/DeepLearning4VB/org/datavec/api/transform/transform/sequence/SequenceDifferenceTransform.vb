Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.transform.metadata
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports FloatWritable = DeepLearning4VB.org.datavec.api.writable.FloatWritable
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchema = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.sequence

    ''' <summary>
    ''' SequenceDifferenceTransform: for an input sequence, calculate the difference on one column.<br/>
    ''' For each time t, calculate someColumn(t) - someColumn(t-s), where s >= 1 is the 'lookback' period.<br/>
    ''' <br/>
    ''' Note: at t=0 (i.e., the first step in a sequence; or more generally, for all times t &lt; s), there is no previous value
    ''' from which to calculate the difference. The <see cref="FirstStepMode"/> enumeration provides the following ways of handling
    ''' these time steps:<br/>
    ''' 1. Default: output = someColumn(t) - someColumn(max(t-s, 0))<br/>
    ''' 2. SpecifiedValue: output = someColumn(t) - someColumn(t-s) if t-s >= 0, or a custom Writable object (provided by the user)
    ''' otherwise.<br/>
    ''' <br/>
    ''' Note that this is an in-place operation: i.e., the values in each column are modified. If the original values are
    ''' to be retained in the data set, first make a copy (for example, using DuplicateColumnsTransform) and then apply the
    ''' SequenceDifferenceTransform operation on the copy.
    ''' <para>With <c>FirstStepMode.SpecifiedValue</c> the specified value is used for the first <c>lookback</c> time
    ''' steps, as documented; the Java code compared the column index instead of the time step with the lookback.</para>
    ''' <para>Equality (Lombok <c>@Data</c>) compares the class and all fields, including the input schema and the
    ''' column type.</para>
    ''' </summary>
    ' Java: @JsonInclude(NON_NULL) @JsonIgnoreProperties({"inputSchema", "columnType"}) @Data
    <Serializable>
    Public Class SequenceDifferenceTransform
        Implements TransformT

        <JsonConverter(GetType(JsonStringEnumConverter))>
        Public Enum FirstStepMode
            [Default]
            SpecifiedValue
        End Enum

        Private ReadOnly columnNameField As String
        Private ReadOnly newColumnName As String
        Private ReadOnly lookback As Integer
        Private ReadOnly firstStepModeField As FirstStepMode
        Private ReadOnly specifiedValueWritable As WritableT

        Private inputSchemaField As SchemaT
        Private columnTypeField As ColumnType

        ''' <summary>
        ''' Create a SequenceDifferenceTransform with default lookback of 1, and using FirstStepMode.Default.
        ''' Output column name is the same as the input column name.
        ''' </summary>
        ''' <param name="columnName"> Name of the column to perform the operation on. </param>
        Public Sub New(ByVal columnName As String)
            Me.New(columnName, columnName, 1, FirstStepMode.Default, Nothing)
        End Sub

        ''' <summary>
        ''' Create a SequenceDifferenceTransform with default lookback of 1, and using FirstStepMode.Default,
        ''' where the output column name is specified
        ''' </summary>
        ''' <param name="columnName">    Name of the column to perform the operation on. </param>
        ''' <param name="newColumnName"> New name for the column. May be same as the origina lcolumn name </param>
        ''' <param name="lookback">      Lookback period, in number of time steps. Must be > 0 </param>
        Public Sub New(ByVal columnName As String, ByVal newColumnName As String, ByVal lookback As Integer)
            Me.New(columnName, newColumnName, lookback, FirstStepMode.Default, Nothing)
        End Sub

        ''' <summary>
        ''' Create a SequenceDifferenceTransform with default lookback of 1, and using FirstStepMode.Default,
        ''' where the output column name is specified
        ''' </summary>
        ''' <param name="columnName">             Name of the column to perform the operation on. </param>
        ''' <param name="newColumnName">          New name for the column. May be same as the origina lcolumn name </param>
        ''' <param name="lookback">               Lookback period, in number of time steps. Must be > 0 </param>
        ''' <param name="firstStepMode">          see <seealso cref="FirstStepMode"/> </param>
        ''' <param name="specifiedValueWritable"> Must be null if using FirstStepMode.Default, or non-null if using FirstStepMode.SpecifiedValue </param>
        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal newColumnName As String, ByVal lookback As Integer, ByVal firstStepMode As FirstStepMode, ByVal specifiedValueWritable As WritableT)
            If firstStepMode <> FirstStepMode.SpecifiedValue AndAlso specifiedValueWritable IsNot Nothing Then
                Throw New ArgumentException("Specified value writable provided (" & specifiedValueWritable.ToString() & ") but " & "firstStepMode != FirstStepMode.SpecifiedValue")
            End If
            If firstStepMode = FirstStepMode.SpecifiedValue AndAlso specifiedValueWritable Is Nothing Then
                Throw New ArgumentException("Specified value writable is null but firstStepMode != FirstStepMode.SpecifiedValue")
            End If
            If lookback <= 0 Then
                Throw New ArgumentException("Lookback period must be > 0. Got: lookback period = " & lookback)
            End If
            Me.columnNameField = columnName
            Me.newColumnName = newColumnName
            Me.lookback = lookback
            Me.firstStepModeField = firstStepMode
            Me.specifiedValueWritable = specifiedValueWritable
        End Sub

        ''' <summary>Java <c>getColumnName()</c> (Lombok).</summary>
        Public Function getColumnName() As String
            Return columnNameField
        End Function

        ''' <summary>Java <c>getNewColumnName()</c> (Lombok).</summary>
        Public Function getNewColumnName() As String
            Return newColumnName
        End Function

        ''' <summary>Java <c>getLookback()</c> (Lombok).</summary>
        Public Function getLookback() As Integer
            Return lookback
        End Function

        ''' <summary>Java <c>getFirstStepMode()</c> (Lombok).</summary>
        Public Function getFirstStepMode() As FirstStepMode
            Return firstStepModeField
        End Function

        ''' <summary>Java <c>getSpecifiedValueWritable()</c> (Lombok).</summary>
        Public Function getSpecifiedValueWritable() As WritableT
            Return specifiedValueWritable
        End Function

        ''' <summary>Java <c>getColumnType()</c> (Lombok): the type of the column, known once the input schema is set.</summary>
        Public Function getColumnType() As ColumnType
            Return columnTypeField
        End Function

        ''' <summary>
        ''' The output column name
        ''' after the operation has been applied
        ''' </summary>
        ''' <returns> the output column name </returns>
        Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
            Return columnNameField
        End Function

        ''' <summary>
        ''' The output column names
        ''' This will often be the same as the input
        ''' </summary>
        ''' <returns> the output column names </returns>
        Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
            Return New String() {columnName()}
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

        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            If Not inputSchema.hasColumn(columnNameField) Then
                Throw New InvalidOperationException("Invalid input schema: does not have column with name """ & columnNameField & """" & vbLf & ". All schema names: " & javaToString(inputSchema.getColumnNames()))
            End If
            If Not (TypeOf inputSchema Is SequenceSchema) Then
                Throw New InvalidOperationException("Invalid input schema: expected a SequenceSchema, got " & CObj(inputSchema).GetType().ToString())
            End If

            Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(inputSchema.numColumns())
            For Each m As ColumnMetaData In inputSchema.getColumnMetaData()
                If columnNameField.Equals(m.Name) Then
                    Select Case m.ColumnType
                        Case ColumnType.Integer
                            newMeta.Add(New IntegerMetaData(newColumnName))
                        Case ColumnType.Long
                            newMeta.Add(New LongMetaData(newColumnName))
                        Case ColumnType.Double
                            newMeta.Add(New DoubleMetaData(newColumnName))
                        Case ColumnType.Float
                            newMeta.Add(New FloatMetaData(newColumnName))
                        Case ColumnType.Time
                            newMeta.Add(New LongMetaData(newColumnName)) 'not Time - time column isn't used for duration...
                        Case Else
                            Throw New InvalidOperationException("Cannot perform sequence difference on column of type " & m.ColumnType.ToString())
                    End Select
                Else
                    newMeta.Add(m)
                End If
            Next m

            Return inputSchema.newSchema(newMeta)
        End Function

        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Set(ByVal value As SchemaT)
                If Not value.hasColumn(columnNameField) Then
                    Throw New InvalidOperationException("Invalid input schema: does not have column with name """ & columnNameField & """" & vbLf & ". All schema names: " & javaToString(value.getColumnNames()))
                End If

                Me.columnTypeField = value.getMetaData(columnNameField).ColumnType
                Me.inputSchemaField = value
            End Set
            Get
                Return inputSchemaField
            End Get
        End Property

        Public Overridable Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT) Implements TransformT.map
            Throw New NotSupportedException("Only sequence operations are supported for SequenceDifferenceTransform." & " Attempting to apply SequenceDifferenceTransform on non-sequence data?")
        End Function

        Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of WritableT)) Implements TransformT.mapSequence
            Dim columnIdx As Integer = inputSchemaField.getIndexOfColumn(columnNameField)

            Dim numSteps As Integer = sequence.Count
            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            For i As Integer = 0 To numSteps - 1
                Dim timeStep As IList(Of WritableT) = sequence(i)
                Dim newTimeStep As IList(Of WritableT) = New List(Of WritableT)(timeStep.Count)
                For j As Integer = 0 To timeStep.Count - 1
                    If j = columnIdx Then
                        ' Java compared the column index j (not the time step i) with the lookback
                        If i < lookback AndAlso firstStepModeField = FirstStepMode.SpecifiedValue Then
                            newTimeStep.Add(specifiedValueWritable)
                        Else
                            Dim current As WritableT = timeStep(j)
                            Dim past As WritableT = sequence(Math.Max(0, i - lookback))(j)
                            Select Case columnTypeField
                                Case ColumnType.Integer
                                    newTimeStep.Add(New IntWritable(current.toInt() - past.toInt()))
                                Case ColumnType.Double
                                    newTimeStep.Add(New DoubleWritable(current.toDouble() - past.toDouble()))
                                Case ColumnType.Float
                                    newTimeStep.Add(New FloatWritable(current.toFloat() - past.toFloat()))
                                Case ColumnType.Long, ColumnType.Time
                                    newTimeStep.Add(New LongWritable(current.toLong() - past.toLong()))
                                Case Else
                                    Throw New InvalidOperationException("Cannot perform sequence difference on column of type " & columnTypeField.ToString())
                            End Select
                        End If
                    Else
                        newTimeStep.Add(timeStep(j))
                    End If
                Next j
                [out].Add(newTimeStep)
            Next i

            Return [out]
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overridable Function map(ByVal input As Object) As Object Implements TransformT.map
            Throw New NotSupportedException("Only sequence operations are supported for SequenceDifferenceTransform." & " Attempting to apply SequenceDifferenceTransform on non-sequence data?")
        End Function

        ''' <summary>
        ''' Transform a sequence
        ''' </summary>
        ''' <param name="sequence"> </param>
        Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements TransformT.mapSequence
            Throw New NotSupportedException("Only sequence operations are supported for SequenceDifferenceTransform." & " Attempting to apply SequenceDifferenceTransform on non-sequence data?")
        End Function

        ''' <summary>Lombok <c>toString()</c>.</summary>
        Public Overrides Function ToString() As String
            Return "SequenceDifferenceTransform(columnName=" & columnNameField & ", newColumnName=" & newColumnName & ", lookback=" & lookback &
                ", firstStepMode=" & firstStepModeField.ToString() & ", specifiedValueWritable=" & javaToString(specifiedValueWritable) &
                ", inputSchema=" & javaToString(inputSchemaField) & ", columnType=" & columnTypeField.ToString() & ")"
        End Function

        ''' <summary>Lombok <c>@Data</c> equality (all fields).</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim o As SequenceDifferenceTransform = DirectCast(obj, SequenceDifferenceTransform)
            Return String.Equals(columnNameField, o.columnNameField) AndAlso String.Equals(newColumnName, o.newColumnName) AndAlso
                lookback = o.lookback AndAlso firstStepModeField = o.firstStepModeField AndAlso
                Objects.equals(specifiedValueWritable, o.specifiedValueWritable) AndAlso
                Objects.equals(inputSchemaField, o.inputSchemaField) AndAlso columnTypeField = o.columnTypeField
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(columnNameField, newColumnName, lookback, firstStepModeField, specifiedValueWritable, inputSchemaField, columnTypeField)
        End Function

    End Class

End Namespace
