Imports System
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports TimeMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.TimeMetaData
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.time

    ''' <summary>
    ''' Transform math op on a time column
    '''
    ''' Note: only the following MathOps are supported: Add, Subtract, ScalarMax, ScalarMin<br/>
    ''' For ScalarMax and ScalarMin, the TimeUnit must be milliseconds - i.e., value must be in epoch millisecond format
    ''' <para>Equality compares the class, the column name, the op, the time quantity and unit (and the derived number
    ''' of milliseconds, as Lombok does).</para>
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema", "columnNumber", "asMilliseconds"}) @Data
    <JsonIgnoreProperties("inputSchema", "columnNumber", "asMilliseconds")>
    <Serializable>
    Public Class TimeMathOpTransform
        Inherits BaseColumnTransform

        Private ReadOnly mathOpField As MathOp
        Private ReadOnly timeQuantity As Long
        Private ReadOnly timeUnitField As TimeUnit
        Private ReadOnly asMilliseconds As Long

        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal mathOp As MathOp, ByVal timeQuantity As Long, ByVal timeUnit As TimeUnit)
            MyBase.New(columnName)
            If mathOp <> MathOp.Add AndAlso mathOp <> MathOp.Subtract AndAlso mathOp <> MathOp.ScalarMin AndAlso mathOp <> MathOp.ScalarMax Then
                Throw New ArgumentException("Invalid MathOp: only Add/Subtract/ScalarMin/ScalarMax supported")
            End If
            If (mathOp = MathOp.ScalarMin OrElse mathOp = MathOp.ScalarMax) AndAlso timeUnit <> TimeUnit.MILLISECONDS Then
                Throw New ArgumentException("Only valid time unit for ScalarMin/Max is Milliseconds (i.e., timestamp format)")
            End If

            Me.mathOpField = mathOp
            Me.timeQuantity = timeQuantity
            Me.timeUnitField = timeUnit
            Me.asMilliseconds = TimeUnit.MILLISECONDS.convert(timeQuantity, timeUnit)
        End Sub

        ''' <summary>Java <c>getMathOp()</c> (Lombok).</summary>
        Public Function getMathOp() As MathOp
            Return mathOpField
        End Function

        ''' <summary>Java <c>getTimeQuantity()</c> (Lombok).</summary>
        Public Function getTimeQuantity() As Long
            Return timeQuantity
        End Function

        ''' <summary>Java <c>getTimeUnit()</c> (Lombok).</summary>
        Public Function getTimeUnit() As TimeUnit
            Return timeUnitField
        End Function

        ''' <summary>Java <c>getAsMilliseconds()</c> (Lombok).</summary>
        Public Function getAsMilliseconds() As Long
            Return asMilliseconds
        End Function

        Public Overrides Function getNewColumnMetaData(ByVal newName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
            If Not (TypeOf oldColumnType Is TimeMetaData) Then
                Throw New InvalidOperationException("Cannot execute TimeMathOpTransform on column with type " & javaToString(oldColumnType))
            End If
            Return New TimeMetaData(newName, DirectCast(oldColumnType, TimeMetaData).getTimeZone())
        End Function

        Public Overrides Function map(ByVal columnWritable As WritableT) As WritableT
            Dim currTime As Long = columnWritable.toLong()
            Select Case mathOpField
                Case MathOp.Add
                    Return New LongWritable(currTime + asMilliseconds)
                Case MathOp.Subtract
                    Return New LongWritable(currTime - asMilliseconds)
                Case MathOp.ScalarMax
                    Return New LongWritable(Math.Max(asMilliseconds, currTime))
                Case MathOp.ScalarMin
                    Return New LongWritable(Math.Min(asMilliseconds, currTime))
                Case Else
                    Throw New Exception("Invalid MathOp for TimeMathOpTransform: " & mathOpField.ToString())
            End Select
        End Function

        Public Overrides Function ToString() As String
            Return "TimeMathOpTransform(mathOp=" & mathOpField.ToString() & "," & timeQuantity & "-" & timeUnitField.ToString() & ")"
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object (as in Java: not supported, returns <c>Nothing</c>)
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Return Nothing
        End Function

        ''' <summary>Equal when the class, the column name, the op, the quantity and the unit are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim o As TimeMathOpTransform = DirectCast(obj, TimeMathOpTransform)
            Return mathOpField = o.mathOpField AndAlso timeQuantity = o.timeQuantity AndAlso timeUnitField = o.timeUnitField AndAlso asMilliseconds = o.asMilliseconds
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), mathOpField, timeQuantity, timeUnitField, asMilliseconds)
        End Function

    End Class

End Namespace
