Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports OverlappingTimeWindowFunction = DeepLearning4VB.org.datavec.api.transform.sequence.window.OverlappingTimeWindowFunction
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports TimeUnit = DeepLearning4VB.JavaCompat.TimeUnit
Imports TimeWindowFunction = DeepLearning4VB.org.datavec.api.transform.sequence.window.TimeWindowFunction
Imports WindowFunction = DeepLearning4VB.org.datavec.api.transform.sequence.window.WindowFunction
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.sequence

    ''' <summary>
    ''' Port of the DataVec <c>TestWindowFunctions</c> JUnit class, plus tests of the window start/end columns, offsets,
    ''' builders and accessors of <see cref="TimeWindowFunction"/> and <see cref="OverlappingTimeWindowFunction"/>.
    ''' </summary>
    <TestClass>
    Public Class TestWindowFunctions

        Private Shared Function row(ByVal ParamArray values() As WritableT) As IList(Of WritableT)
            Return New List(Of WritableT)(values)
        End Function

        ''' <summary>The sequence used by the first three Java tests (1 second windows, the third one empty).</summary>
        Private Shared Function secondsSequence() As IList(Of IList(Of WritableT))
            Dim sequence As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            'First window:
            sequence.Add(row(New LongWritable(1451606400000L), New IntWritable(0)))
            sequence.Add(row(New LongWritable(1451606400000L + 100L), New IntWritable(1)))
            sequence.Add(row(New LongWritable(1451606400000L + 200L), New IntWritable(2)))
            'Second window:
            sequence.Add(row(New LongWritable(1451606400000L + 1000L), New IntWritable(3)))
            'Third window: empty
            'Fourth window:
            sequence.Add(row(New LongWritable(1451606400000L + 3000L), New IntWritable(4)))
            sequence.Add(row(New LongWritable(1451606400000L + 3100L), New IntWritable(5)))
            Return sequence
        End Function

        ''' <summary>The sequence used by the overlapping window Java tests.</summary>
        Private Shared Function overlappingSequence() As IList(Of IList(Of WritableT))
            Dim sequence As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            sequence.Add(row(New LongWritable(0), New IntWritable(0)))
            sequence.Add(row(New LongWritable(100), New IntWritable(1)))
            sequence.Add(row(New LongWritable(200), New IntWritable(2)))
            sequence.Add(row(New LongWritable(1000), New IntWritable(3)))
            sequence.Add(row(New LongWritable(1500), New IntWritable(4)))
            sequence.Add(row(New LongWritable(2000), New IntWritable(5)))
            sequence.Add(row(New LongWritable(5000), New IntWritable(7)))
            Return sequence
        End Function

        Private Shared Function timeSchema() As SchemaT
            Return (New SequenceSchemaT.Builder()).addColumnTime("timecolumn", DateTimeZone.UTC).addColumnInteger("intcolumn").build()
        End Function

        <TestMethod>
        Public Sub testTimeWindowFunction()

            'Time windowing: 1 second (1000 milliseconds) window
            Dim sequence As IList(Of IList(Of WritableT)) = secondsSequence()

            Dim schema As SchemaT = timeSchema()

            Dim wf As WindowFunction = New TimeWindowFunction("timecolumn", 1, TimeUnit.SECONDS)
            wf.InputSchema = schema

            Dim windows As IList(Of IList(Of IList(Of WritableT))) = wf.applyToSequence(sequence)

            assertEquals(4, windows.Count)
            assertEquals(3, windows(0).Count)
            assertEquals(1, windows(1).Count)
            assertEquals(0, windows(2).Count)
            assertEquals(2, windows(3).Count)

            Dim exp0 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            exp0.Add(sequence(0))
            exp0.Add(sequence(1))
            exp0.Add(sequence(2))
            assertEquals(exp0, windows(0))

            Dim exp1 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            exp1.Add(sequence(3))
            assertEquals(exp1, windows(1))

            Dim exp2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            assertEquals(exp2, windows(2))

            Dim exp3 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            exp3.Add(sequence(4))
            exp3.Add(sequence(5))
            assertEquals(exp3, windows(3))
        End Sub

        <TestMethod>
        Public Sub testTimeWindowFunctionExcludeEmpty()

            'Time windowing: 1 second (1000 milliseconds) window
            Dim sequence As IList(Of IList(Of WritableT)) = secondsSequence()

            Dim schema As SchemaT = timeSchema()

            Dim wf As WindowFunction = (New TimeWindowFunction.Builder()).timeColumn("timecolumn").windowSize(1, TimeUnit.SECONDS).excludeEmptyWindows(True).build()
            wf.InputSchema = schema

            Dim windows As IList(Of IList(Of IList(Of WritableT))) = wf.applyToSequence(sequence)

            assertEquals(3, windows.Count)
            assertEquals(3, windows(0).Count)
            assertEquals(1, windows(1).Count)
            assertEquals(2, windows(2).Count)

            Dim exp0 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            exp0.Add(sequence(0))
            exp0.Add(sequence(1))
            exp0.Add(sequence(2))
            assertEquals(exp0, windows(0))

            Dim exp1 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            exp1.Add(sequence(3))
            assertEquals(exp1, windows(1))

            Dim exp2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            exp2.Add(sequence(4))
            exp2.Add(sequence(5))
            assertEquals(exp2, windows(2))
        End Sub

        <TestMethod>
        Public Sub testOverlappingTimeWindowFunctionSimple()
            'Compare Overlapping and standard window functions where the window separation is equal to the window size
            ' In this case, we should get exactly the same results from both.
            'Time windowing: 1 second (1000 milliseconds) window
            Dim sequence As IList(Of IList(Of WritableT)) = secondsSequence()

            Dim schema As SchemaT = timeSchema()

            Dim wf As WindowFunction = New TimeWindowFunction("timecolumn", 1, TimeUnit.SECONDS)
            wf.InputSchema = schema

            Dim wf2 As WindowFunction = New OverlappingTimeWindowFunction("timecolumn", 1, TimeUnit.SECONDS, 1, TimeUnit.SECONDS)
            wf2.InputSchema = schema

            Dim windowsExp As IList(Of IList(Of IList(Of WritableT))) = wf.applyToSequence(sequence)
            Dim windowsAct As IList(Of IList(Of IList(Of WritableT))) = wf2.applyToSequence(sequence)

            Dim expSizes() As Integer = {3, 1, 0, 2}
            assertEquals(4, windowsExp.Count)
            assertEquals(4, windowsAct.Count)
            For i As Integer = 0 To 3
                assertEquals(expSizes(i), windowsExp(i).Count)
                assertEquals(expSizes(i), windowsAct(i).Count)

                assertEquals(windowsExp(i), windowsAct(i))
            Next i
        End Sub

        ''' <summary>The windows the Java tests expect for <see cref="overlappingSequence"/> (2 s windows every 1 s).</summary>
        Private Shared Function expectedOverlappingWindows(ByVal includeEmpty As Boolean) As IList(Of IList(Of IList(Of WritableT)))
            'First window: -1000 to 1000
            Dim exp0 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New LongWritable(0), New IntWritable(0)), row(New LongWritable(100), New IntWritable(1)), row(New LongWritable(200), New IntWritable(2))}
            'Second window: 0 to 2000
            Dim exp1 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New LongWritable(0), New IntWritable(0)), row(New LongWritable(100), New IntWritable(1)), row(New LongWritable(200), New IntWritable(2)),
                row(New LongWritable(1000), New IntWritable(3)), row(New LongWritable(1500), New IntWritable(4))}
            'Third window: 1000 to 3000
            Dim exp2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New LongWritable(1000), New IntWritable(3)), row(New LongWritable(1500), New IntWritable(4)), row(New LongWritable(2000), New IntWritable(5))}
            'Fourth window: 2000 to 4000
            Dim exp3 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(New LongWritable(2000), New IntWritable(5))}
            'Fifth window: 3000 to 5000
            Dim exp4 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            'Sixth window: 4000 to 6000
            Dim exp5 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(New LongWritable(5000), New IntWritable(7))}
            'Seventh window: 5000 to 7000
            Dim exp6 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(New LongWritable(5000), New IntWritable(7))}

            If includeEmpty Then
                Return New List(Of IList(Of IList(Of WritableT))) From {exp0, exp1, exp2, exp3, exp4, exp5, exp6}
            End If
            Return New List(Of IList(Of IList(Of WritableT))) From {exp0, exp1, exp2, exp3, exp5, exp6}
        End Function

        <TestMethod>
        Public Sub testOverlappingTimeWindowFunction()
            Dim sequence As IList(Of IList(Of WritableT)) = overlappingSequence()

            Dim schema As SchemaT = timeSchema()

            'Window size: 2 seconds; calculated every 1 second
            Dim wf2 As WindowFunction = New OverlappingTimeWindowFunction("timecolumn", 2, TimeUnit.SECONDS, 1, TimeUnit.SECONDS)
            wf2.InputSchema = schema

            Dim windowsAct As IList(Of IList(Of IList(Of WritableT))) = wf2.applyToSequence(sequence)

            Dim windowsExp As IList(Of IList(Of IList(Of WritableT))) = expectedOverlappingWindows(True)

            assertEquals(7, windowsAct.Count)
            For i As Integer = 0 To 6
                Dim exp As IList(Of IList(Of WritableT)) = windowsExp(i)
                Dim act As IList(Of IList(Of WritableT)) = windowsAct(i)

                assertEquals(exp, act)
            Next i
        End Sub

        <TestMethod>
        Public Sub testOverlappingTimeWindowFunctionExcludeEmpty()
            Dim sequence As IList(Of IList(Of WritableT)) = overlappingSequence()

            Dim schema As SchemaT = timeSchema()

            'Window size: 2 seconds; calculated every 1 second
            Dim wf2 As WindowFunction = (New OverlappingTimeWindowFunction.Builder()).timeColumn("timecolumn").windowSize(2, TimeUnit.SECONDS).windowSeparation(1, TimeUnit.SECONDS).excludeEmptyWindows(True).build()
            wf2.InputSchema = schema

            Dim windowsAct As IList(Of IList(Of IList(Of WritableT))) = wf2.applyToSequence(sequence)

            'Fifth window: 3000 to 5000 -> Empty: excluded
            Dim windowsExp As IList(Of IList(Of IList(Of WritableT))) = expectedOverlappingWindows(False)

            assertEquals(6, windowsAct.Count)
            For i As Integer = 0 To 5
                Dim exp As IList(Of IList(Of WritableT)) = windowsExp(i)
                Dim act As IList(Of IList(Of WritableT)) = windowsAct(i)

                assertEquals(exp, act)
            Next i
        End Sub

        ' ---- additional tests ----

        <TestMethod>
        Public Sub testTimeWindowFunctionStartEndColumns()
            Dim schema As SchemaT = timeSchema()
            Dim wf As New TimeWindowFunction("timecolumn", 1, TimeUnit.SECONDS, True, True)
            wf.InputSchema = schema

            Dim outSchema As SchemaT = wf.transform(schema)
            assertTrue(TypeOf outSchema Is SequenceSchemaT)
            assertEquals(New List(Of String) From {"timecolumn", "intcolumn", "windowStartTime", "windowEndTime"}, outSchema.getColumnNames())
            assertEquals(New List(Of ColumnType) From {ColumnType.Time, ColumnType.Integer, ColumnType.Time, ColumnType.Time}, outSchema.getColumnTypes())

            Dim windows As IList(Of IList(Of IList(Of WritableT))) = wf.applyToSequence(secondsSequence())
            assertEquals(4, windows.Count)
            Dim start As Long = 1451606400000L
            assertEquals(row(New LongWritable(start + 100L), New IntWritable(1), New LongWritable(start), New LongWritable(start + 1000L)), windows(0)(1))
            assertEquals(row(New LongWritable(start + 3100L), New IntWritable(5), New LongWritable(start + 3000L), New LongWritable(start + 4000L)), windows(3)(1))

            'Without the extra columns the schema is returned as it is
            Dim plain As New TimeWindowFunction("timecolumn", 1, TimeUnit.SECONDS)
            assertTrue(plain.transform(schema) Is schema)
        End Sub

        <TestMethod>
        Public Sub testTimeWindowFunctionOffsetAndTimeZone()
            Dim schema As SchemaT = timeSchema()
            'Hourly windows, shifted by 15 minutes
            Dim wf As New TimeWindowFunction("timecolumn", 1, TimeUnit.HOURS, 15, TimeUnit.MINUTES)
            wf.InputSchema = schema
            assertEquals(15L * 60000L, wf.getOffsetAmountMilliseconds())
            assertEquals(3600000L, wf.getWindowSizeMilliseconds())
            Dim t As Long = 1451606400000L + 10L * 60000L '00:10 UTC
            'As in Java the start time is (time + offset) rounded down: the windows are [23:45, 00:45) labelled 00:00,
            '[00:45, 01:45) labelled 01:00 and so on
            assertEquals(1451606400000L, wf.getWindowStartTimeForTime(t))
            assertEquals(1451606400000L, wf.getWindowStartTimeForTime(1451606400000L + 44L * 60000L))
            assertEquals(1451606400000L + 3600000L, wf.getWindowStartTimeForTime(1451606400000L + 45L * 60000L))
            assertEquals(wf.getWindowStartTimeForTime(t) + 3600000L, wf.getWindowEndTimeForTime(t))

            'Time zone offsets shift the windows too: UTC+01:00, hourly windows -> the same as UTC for whole hours
            Dim plusOne As SchemaT = (New SequenceSchemaT.Builder()).addColumnTime("timecolumn", DateTimeZone.forOffsetHours(1)).addColumnInteger("intcolumn").build()
            Dim hourly As New TimeWindowFunction("timecolumn", 1, TimeUnit.HOURS)
            hourly.InputSchema = plusOne
            assertEquals(DateTimeZone.forOffsetHours(1), hourly.getTimeZone())
            assertEquals(1451606400000L, hourly.getWindowStartTimeForTime(t))

            'Daily windows in UTC+01:00: (t + 1h) rounded down to whole days
            Dim daily As New TimeWindowFunction("timecolumn", 1, TimeUnit.DAYS)
            daily.InputSchema = plusOne
            Dim dayMs As Long = 86400000L
            assertEquals((t + 3600000L) - (t + 3600000L) Mod dayMs, daily.getWindowStartTimeForTime(t))
        End Sub

        <TestMethod>
        Public Sub testTimeWindowFunctionSchemaChecks()
            Dim wf As New TimeWindowFunction("timecolumn", 1, TimeUnit.SECONDS)
            'Not a sequence schema
            Dim nonSequence As SchemaT = (New SchemaT.Builder()).addColumnTime("timecolumn", DateTimeZone.UTC).build()
            assertThrows(Of ArgumentException)(Sub() wf.InputSchema = nonSequence)
            'No such column
            Dim noColumn As SchemaT = (New SequenceSchemaT.Builder()).addColumnTime("other", DateTimeZone.UTC).build()
            assertThrows(Of InvalidOperationException)(Sub() wf.InputSchema = noColumn)
            'Not a time column
            Dim notTime As SchemaT = (New SequenceSchemaT.Builder()).addColumnLong("timecolumn").build()
            assertThrows(Of InvalidOperationException)(Sub() wf.InputSchema = notTime)
            assertNull(wf.InputSchema)

            'An empty sequence gives no windows
            wf.InputSchema = timeSchema()
            assertEquals(0, wf.applyToSequence(New List(Of IList(Of WritableT))()).Count)
            Dim excluding As TimeWindowFunction = (New TimeWindowFunction.Builder()).timeColumn("timecolumn").windowSize(1, TimeUnit.SECONDS).excludeEmptyWindows(True).build()
            excluding.InputSchema = timeSchema()
            assertEquals(0, excluding.applyToSequence(New List(Of IList(Of WritableT))()).Count)
        End Sub

        <TestMethod>
        Public Sub testTimeWindowFunctionBuilderAndAccessors()
            assertThrows(Of InvalidOperationException)(Sub() Call (New TimeWindowFunction.Builder()).windowSize(1, TimeUnit.SECONDS).build())
            assertThrows(Of InvalidOperationException)(Sub() Call (New TimeWindowFunction.Builder()).timeColumn("t").build())

            Dim built As TimeWindowFunction = (New TimeWindowFunction.Builder()).timeColumn("t").windowSize(2, TimeUnit.MINUTES).offset(30, TimeUnit.SECONDS).addWindowStartTimeColumn(True).addWindowEndTimeColumn(False).build()
            Dim direct As New TimeWindowFunction("t", 2, TimeUnit.MINUTES, 30, TimeUnit.SECONDS, True, False, False)
            assertEquals(direct, built)
            assertEquals(direct.GetHashCode(), built.GetHashCode())
            assertEquals("t", built.getTimeColumn())
            assertEquals(2L, built.getWindowSize())
            assertEquals(TimeUnit.MINUTES, built.getWindowSizeUnit())
            assertEquals(30L, built.getOffsetAmount())
            assertEquals(CType(TimeUnit.SECONDS, TimeUnit?), built.getOffsetUnit())
            assertTrue(built.isAddWindowStartTimeColumn())
            assertFalse(built.isAddWindowEndTimeColumn())
            assertFalse(built.isExcludeEmptyWindows())
            assertEquals("TimeWindowFunction(column=""t"",windowSize=2MINUTES,offset=30SECONDS,addWindowStartTimeColumn=true)", built.ToString())
            assertEquals("TimeWindowFunction(column=""t"",windowSize=1SECONDS,offset=0)", (New TimeWindowFunction("t", 1, TimeUnit.SECONDS)).ToString())
            assertFalse(built.Equals(New TimeWindowFunction("t", 2, TimeUnit.MINUTES)))
            'An offset without a unit is no offset
            assertEquals(0L, (New TimeWindowFunction("t", 1, TimeUnit.SECONDS, 5, Nothing)).getOffsetAmountMilliseconds())
        End Sub

        <TestMethod>
        Public Sub testOverlappingTimeWindowFunctionStartEndColumnsAndOffset()
            Dim schema As SchemaT = timeSchema()
            Dim wf As New OverlappingTimeWindowFunction("timecolumn", 2, TimeUnit.SECONDS, 1, TimeUnit.SECONDS, True, True)
            wf.InputSchema = schema
            assertEquals(New List(Of String) From {"timecolumn", "intcolumn", "windowStartTime", "windowEndTime"}, wf.transform(schema).getColumnNames())

            Dim windows As IList(Of IList(Of IList(Of WritableT))) = wf.applyToSequence(overlappingSequence())
            assertEquals(7, windows.Count)
            'Second window: 0 to 2000
            assertEquals(row(New LongWritable(1500), New IntWritable(4), New LongWritable(0), New LongWritable(2000)), windows(1)(4))
            'First window: -1000 to 1000
            assertEquals(row(New LongWritable(0), New IntWritable(0), New LongWritable(-1000), New LongWritable(1000)), windows(0)(0))

            Dim shifted As New OverlappingTimeWindowFunction("timecolumn", 2, TimeUnit.SECONDS, 1, TimeUnit.SECONDS, 500, TimeUnit.MILLISECONDS)
            shifted.InputSchema = schema
            assertEquals(500L, shifted.getOffsetAmountMilliseconds())
            assertEquals(1000L, shifted.getWindowSeparationMilliseconds())
            assertEquals(2000L, shifted.getWindowSizeMilliseconds())
            assertTrue(shifted.applyToSequence(overlappingSequence()).Count > 0)
        End Sub

        <TestMethod>
        Public Sub testOverlappingTimeWindowFunctionBuilderAndChecks()
            assertThrows(Of InvalidOperationException)(Sub() Call (New OverlappingTimeWindowFunction.Builder()).windowSize(1, TimeUnit.SECONDS).windowSeparation(1, TimeUnit.SECONDS).build())
            assertThrows(Of InvalidOperationException)(Sub() Call (New OverlappingTimeWindowFunction.Builder()).timeColumn("t").windowSeparation(1, TimeUnit.SECONDS).build())
            assertThrows(Of InvalidOperationException)(Sub() Call (New OverlappingTimeWindowFunction.Builder()).timeColumn("t").windowSize(1, TimeUnit.SECONDS).build())

            Dim built As OverlappingTimeWindowFunction = (New OverlappingTimeWindowFunction.Builder()).timeColumn("t").windowSize(2, TimeUnit.SECONDS).windowSeparation(1, TimeUnit.SECONDS).offset(5, TimeUnit.MILLISECONDS).excludeEmptyWindows(True).build()
            Dim direct As New OverlappingTimeWindowFunction("t", 2, TimeUnit.SECONDS, 1, TimeUnit.SECONDS, 5, TimeUnit.MILLISECONDS, False, False, True)
            assertEquals(direct, built)
            assertEquals(direct.GetHashCode(), built.GetHashCode())
            assertEquals("OverlappingTimeWindowFunction(columnName=""t"",windowSize=2SECONDS,windowSeparation=1SECONDS,offset=5MILLISECONDS,excludeEmptyWindows=true)", built.ToString())
            assertEquals(1L, built.getWindowSeparation())
            assertEquals(TimeUnit.SECONDS, built.getWindowSeparationUnit())

            'Equality ignores the input schema (and the time zone taken from it)
            built.InputSchema = (New SequenceSchemaT.Builder()).addColumnTime("t", DateTimeZone.forOffsetHours(2)).build()
            assertEquals(DateTimeZone.forOffsetHours(2), built.getTimeZone())
            assertEquals(direct, built)

            Dim nonSequence As SchemaT = (New SchemaT.Builder()).addColumnTime("t", DateTimeZone.UTC).build()
            assertThrows(Of ArgumentException)(Sub() direct.InputSchema = nonSequence)
            Dim notTime As SchemaT = (New SequenceSchemaT.Builder()).addColumnInteger("t").build()
            assertThrows(Of InvalidOperationException)(Sub() direct.InputSchema = notTime)
            Dim noColumn As SchemaT = (New SequenceSchemaT.Builder()).addColumnInteger("x").build()
            assertThrows(Of InvalidOperationException)(Sub() direct.InputSchema = noColumn)
        End Sub

    End Class

End Namespace
