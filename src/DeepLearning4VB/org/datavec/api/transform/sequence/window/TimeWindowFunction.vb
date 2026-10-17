'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.JavaCompat
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports TimeMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.TimeMetaData
Imports TimeUnitT = DeepLearning4VB.JavaCompat.TimeUnit
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.sequence.window

    ''' <summary>
    ''' A time window function splits a sequence into non-overlapping windows based on a time column: the window of a
    ''' time step starts at its time rounded down to a multiple of the window size, after shifting by the time zone
    ''' offset (of the time column's time zone) and the optional offset. Empty windows between two non-empty windows
    ''' are included unless <c>excludeEmptyWindows</c> is set. Optionally the window start and/or end times are added
    ''' as time columns. The sequence must already be sorted by time. Units are <see cref="TimeUnit"/>s; an offset unit
    ''' of Nothing means no offset. Equality (Lombok <c>@Data</c>) compares all fields, including the input schema and
    ''' the time zone.
    ''' </summary>
    <Serializable>
    Public Class TimeWindowFunction
        Implements WindowFunction

        Private ReadOnly timeColumn As String
        Private ReadOnly windowSize As Long
        Private ReadOnly windowSizeUnit As TimeUnitT
        Private ReadOnly offsetAmount As Long
        Private ReadOnly offsetUnit As TimeUnitT?
        Private ReadOnly addWindowStartTimeColumn As Boolean
        Private ReadOnly addWindowEndTimeColumn As Boolean
        Private ReadOnly excludeEmptyWindows As Boolean
        Private inputSchemaField As SchemaT

        Private ReadOnly offsetAmountMilliseconds As Long
        Private ReadOnly windowSizeMilliseconds As Long

        Private timeZone As DateTimeZone

        ''' <summary>
        ''' Constructor with zero offset
        ''' </summary>
        ''' <param name="timeColumn">     Name of the column that contains the time values (must be a time column) </param>
        ''' <param name="windowSize">     Numerical quantity for the size of the time window (used in conjunction with windowSizeUnit) </param>
        ''' <param name="windowSizeUnit"> Unit of the time window </param>
        Public Sub New(ByVal timeColumn As String, ByVal windowSize As Long, ByVal windowSizeUnit As TimeUnitT)
            Me.New(timeColumn, windowSize, windowSizeUnit, 0, Nothing)
        End Sub

        ''' <summary>
        ''' Constructor with zero offset, and supports adding columns containing the start and/or end times of the window
        ''' </summary>
        ''' <param name="timeColumn">               Name of the column that contains the time values (must be a time column) </param>
        ''' <param name="windowSize">               Numerical quantity for the size of the time window (used in conjunction with windowSizeUnit) </param>
        ''' <param name="windowSizeUnit">           Unit of the time window </param>
        ''' <param name="addWindowStartTimeColumn"> If true: add a time column (name: "windowStartTime") that contains the start time
        '''                                 of the window </param>
        ''' <param name="addWindowEndTimeColumn">   If true: add a time column (name: "windowEndTime") that contains the end time
        '''                                 of the window </param>
        Public Sub New(ByVal timeColumn As String, ByVal windowSize As Long, ByVal windowSizeUnit As TimeUnitT, ByVal addWindowStartTimeColumn As Boolean, ByVal addWindowEndTimeColumn As Boolean)
            Me.New(timeColumn, windowSize, windowSizeUnit, 0, Nothing, addWindowStartTimeColumn, addWindowEndTimeColumn, False)
        End Sub

        ''' <summary>
        ''' Constructor with optional offset
        ''' </summary>
        ''' <param name="timeColumn">     Name of the column that contains the time values (must be a time column) </param>
        ''' <param name="windowSize">     Numerical quantity for the size of the time window (used in conjunction with windowSizeUnit) </param>
        ''' <param name="windowSizeUnit"> Unit of the time window </param>
        ''' <param name="offset">         Optional offset amount, to shift start/end of the time window forward or back </param>
        ''' <param name="offsetUnit">     Optional offset unit for the offset amount. </param>
        Public Sub New(ByVal timeColumn As String, ByVal windowSize As Long, ByVal windowSizeUnit As TimeUnitT, ByVal offset As Long, ByVal offsetUnit As TimeUnitT?)
            Me.New(timeColumn, windowSize, windowSizeUnit, offset, offsetUnit, False, False, False)
        End Sub

        ''' <summary>
        ''' Constructor with optional offset
        ''' </summary>
        ''' <param name="timeColumn">               Name of the column that contains the time values (must be a time column) </param>
        ''' <param name="windowSize">               Numerical quantity for the size of the time window (used in conjunction with windowSizeUnit) </param>
        ''' <param name="windowSizeUnit">           Unit of the time window </param>
        ''' <param name="offset">                   Optional offset amount, to shift start/end of the time window forward or back </param>
        ''' <param name="offsetUnit">               Optional offset unit for the offset amount. </param>
        ''' <param name="addWindowStartTimeColumn"> If true: add a column (at the end) with the window start time </param>
        ''' <param name="addWindowEndTimeColumn">   If true: add a column (at the end) with the window end time </param>
        ''' <param name="excludeEmptyWindows">      If true: exclude any windows that don't have any values in them </param>
        Public Sub New(ByVal timeColumn As String, ByVal windowSize As Long, ByVal windowSizeUnit As TimeUnitT, ByVal offset As Long, ByVal offsetUnit As TimeUnitT?, ByVal addWindowStartTimeColumn As Boolean, ByVal addWindowEndTimeColumn As Boolean, ByVal excludeEmptyWindows As Boolean)
            Me.timeColumn = timeColumn
            Me.windowSize = windowSize
            Me.windowSizeUnit = windowSizeUnit
            Me.offsetAmount = offset
            Me.offsetUnit = offsetUnit
            Me.addWindowStartTimeColumn = addWindowStartTimeColumn
            Me.addWindowEndTimeColumn = addWindowEndTimeColumn
            Me.excludeEmptyWindows = excludeEmptyWindows

            If offsetAmount = 0 OrElse Not offsetUnit.HasValue Then
                Me.offsetAmountMilliseconds = 0
            Else
                Me.offsetAmountMilliseconds = TimeUnitT.MILLISECONDS.convert(offset, offsetUnit.Value)
            End If

            Me.windowSizeMilliseconds = TimeUnitT.MILLISECONDS.convert(windowSize, windowSizeUnit)
        End Sub

        Private Sub New(ByVal builder As Builder)
            Me.New(builder.timeColumnField, builder.windowSizeField, builder.windowSizeUnit.Value, builder.offsetAmount, builder.offsetUnit, builder.addWindowStartTimeColumnField, builder.addWindowEndTimeColumnField, builder.excludeEmptyWindowsField)
        End Sub

        ''' <summary>
        ''' The input schema: must be a sequence schema with a time column of the configured name. Setting it also
        ''' takes the time zone of that column.
        ''' </summary>
        ''' <exception cref="ArgumentException">if the schema is not a sequence schema</exception>
        ''' <exception cref="InvalidOperationException">if the column is missing or not a time column</exception>
        Public Overridable Property InputSchema As SchemaT Implements WindowFunction.InputSchema
            Get
                Return inputSchemaField
            End Get
            Set(ByVal value As SchemaT)
                If Not (TypeOf value Is SequenceSchemaT) Then
                    Throw New ArgumentException("Invalid schema: TimeWindowFunction can " & "only operate on SequenceSchema")
                End If
                If Not value.hasColumn(timeColumn) Then
                    Throw New InvalidOperationException("Input schema does not have a column with name """ & timeColumn & """")
                End If

                If value.getMetaData(timeColumn).ColumnType <> ColumnType.Time Then
                    Throw New InvalidOperationException("Invalid column: column """ & timeColumn & """ is not of type " & ColumnType.Time.ToString() & "; is " & value.getMetaData(timeColumn).ColumnType.ToString())
                End If

                Me.inputSchemaField = value

                timeZone = DirectCast(value.getMetaData(timeColumn), TimeMetaData).getTimeZone()
            End Set
        End Property

        ''' <summary>Java <c>getTimeColumn()</c> (Lombok).</summary>
        Public Function getTimeColumn() As String
            Return timeColumn
        End Function

        ''' <summary>Java <c>getWindowSize()</c> (Lombok).</summary>
        Public Function getWindowSize() As Long
            Return windowSize
        End Function

        ''' <summary>Java <c>getWindowSizeUnit()</c> (Lombok).</summary>
        Public Function getWindowSizeUnit() As TimeUnitT
            Return windowSizeUnit
        End Function

        ''' <summary>Java <c>getOffsetAmount()</c> (Lombok).</summary>
        Public Function getOffsetAmount() As Long
            Return offsetAmount
        End Function

        ''' <summary>Java <c>getOffsetUnit()</c> (Lombok); Nothing when there is no offset unit.</summary>
        Public Function getOffsetUnit() As TimeUnitT?
            Return offsetUnit
        End Function

        ''' <summary>Java <c>isAddWindowStartTimeColumn()</c> (Lombok).</summary>
        Public Function isAddWindowStartTimeColumn() As Boolean
            Return addWindowStartTimeColumn
        End Function

        ''' <summary>Java <c>isAddWindowEndTimeColumn()</c> (Lombok).</summary>
        Public Function isAddWindowEndTimeColumn() As Boolean
            Return addWindowEndTimeColumn
        End Function

        ''' <summary>Java <c>isExcludeEmptyWindows()</c> (Lombok).</summary>
        Public Function isExcludeEmptyWindows() As Boolean
            Return excludeEmptyWindows
        End Function

        ''' <summary>Java <c>getOffsetAmountMilliseconds()</c> (Lombok).</summary>
        Public Function getOffsetAmountMilliseconds() As Long
            Return offsetAmountMilliseconds
        End Function

        ''' <summary>Java <c>getWindowSizeMilliseconds()</c> (Lombok).</summary>
        Public Function getWindowSizeMilliseconds() As Long
            Return windowSizeMilliseconds
        End Function

        ''' <summary>Java <c>getTimeZone()</c> (Lombok): the time zone of the time column, known once the schema is set.</summary>
        Public Function getTimeZone() As DateTimeZone
            Return timeZone
        End Function

        ''' <summary>Java <c>setTimeZone()</c> (Lombok).</summary>
        Public Sub setTimeZone(ByVal timeZone As DateTimeZone)
            Me.timeZone = timeZone
        End Sub

        ''' <summary>The input schema plus the optional "windowStartTime"/"windowEndTime" time columns.</summary>
        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements WindowFunction.transform
            If Not addWindowStartTimeColumn AndAlso Not addWindowEndTimeColumn Then
                Return inputSchema
            End If

            Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(inputSchema.getColumnMetaData())

            If addWindowStartTimeColumn Then
                newMeta.Add(New TimeMetaData("windowStartTime"))
            End If

            If addWindowEndTimeColumn Then
                newMeta.Add(New TimeMetaData("windowEndTime"))
            End If

            Return inputSchema.newSchema(newMeta)
        End Function

        Public Overrides Function ToString() As String
            Return "TimeWindowFunction(column=""" & timeColumn & """,windowSize=" & windowSize & windowSizeUnit.ToString() & ",offset=" & offsetAmount &
                (If(offsetAmount <> 0 AndAlso offsetUnit.HasValue, offsetUnit.Value.ToString(), "")) &
                (If(addWindowStartTimeColumn, ",addWindowStartTimeColumn=true", "")) &
                (If(addWindowEndTimeColumn, ",addWindowEndTimeColumn=true", "")) &
                (If(excludeEmptyWindows, ",excludeEmptyWindows=true", "")) & ")"
        End Function

        ''' <summary>Splits the (time-sorted) sequence into consecutive time windows.</summary>
        Public Overridable Function applyToSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of IList(Of WritableT))) Implements WindowFunction.applyToSequence

            Dim timeColumnIdx As Integer = inputSchemaField.getIndexOfColumn(Me.timeColumn)

            Dim [out] As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()

            'We are assuming here that the sequence is already ordered (as is usually the case)
            Dim currentWindowStartTime As Long = Long.MinValue
            Dim currentWindow As IList(Of IList(Of WritableT)) = Nothing
            For Each timeStep As IList(Of WritableT) In sequence
                Dim currentTime As Long = timeStep(timeColumnIdx).toLong()
                Dim windowStartTimeOfThisTimeStep As Long = getWindowStartTimeForTime(currentTime)

                'First time step...
                If currentWindowStartTime = Long.MinValue Then
                    currentWindowStartTime = windowStartTimeOfThisTimeStep
                    currentWindow = New List(Of IList(Of WritableT))()
                End If

                'Two possibilities here: (a) we add it to the last time step, or (b) we need to make a new window...
                If currentWindowStartTime < windowStartTimeOfThisTimeStep Then
                    'New window. But: complication. We might have a bunch of empty windows...
                    Do While currentWindowStartTime < windowStartTimeOfThisTimeStep
                        If currentWindow IsNot Nothing Then
                            If Not (excludeEmptyWindows AndAlso currentWindow.Count = 0) Then
                                [out].Add(currentWindow)
                            End If
                        End If
                        currentWindow = New List(Of IList(Of WritableT))()
                        currentWindowStartTime += windowSizeMilliseconds
                    Loop
                End If
                If addWindowStartTimeColumn OrElse addWindowEndTimeColumn Then
                    Dim timeStep2 As IList(Of WritableT) = New List(Of WritableT)(timeStep)
                    If addWindowStartTimeColumn Then
                        timeStep2.Add(New LongWritable(currentWindowStartTime))
                    End If
                    If addWindowEndTimeColumn Then
                        timeStep2.Add(New LongWritable(currentWindowStartTime + windowSizeMilliseconds))
                    End If
                    currentWindow.Add(timeStep2)
                Else
                    currentWindow.Add(timeStep)
                End If
            Next timeStep

            'Add the final window to the output data...
            ' (null check first: Java evaluated currentWindow.size() before it, which failed for an empty sequence)
            If currentWindow IsNot Nothing AndAlso Not (excludeEmptyWindows AndAlso currentWindow.Count = 0) Then
                [out].Add(currentWindow)
            End If

            Return [out]
        End Function

        ''' <summary>
        ''' Calculates the start time of the window for which the specified time belongs, in unix epoch (millisecond) format<br/>
        ''' For example, if the window size is 1 hour with offset 0, then a time 10:17 would return 10:00, as the 1 hour window
        ''' is for 10:00:00.000 to 10:59:59.999 inclusive, or 10:00:00.000 (inclusive) to 11:00:00.000 (exclusive)
        ''' </summary>
        ''' <param name="time"> Time at which to determine the window start time (milliseconds epoch format) </param>
        Public Overridable Function getWindowStartTimeForTime(ByVal time As Long) As Long

            'Calculate aggregate offset: aggregate offset is due to both timezone and manual offset
            Dim aggregateOffset As Long = (timeZone.getOffset(time) + Me.offsetAmountMilliseconds) Mod Me.windowSizeMilliseconds

            Return (time + aggregateOffset) - (time + aggregateOffset) Mod Me.windowSizeMilliseconds
        End Function

        ''' <summary>
        ''' Calculates the end time of the window for which the specified time belongs, in unix epoch (millisecond) format.
        ''' <b>Note</b>: this value is not included in the interval. Put another way, it is the start time of the <i>next</i>
        ''' interval: i.e., is equivalent to <seealso cref="getWindowStartTimeForTime(Long)"/> + interval (in milliseconds).<br/>
        ''' To get the last <i>inclusive</i> time for the interval, subtract 1L (1 millisecond) from the value returned by
        ''' this method.<br/>
        ''' For example, if the window size is 1 hour with offset 0, then a time 10:17 would return 11:00, as the 1 hour window
        ''' is for 10:00:00.000 (inclusive) to 11:00:00.000 (exclusive)
        ''' </summary>
        ''' <param name="time"> Time at which to determine the window start time </param>
        Public Overridable Function getWindowEndTimeForTime(ByVal time As Long) As Long
            Return getWindowStartTimeForTime(time) + Me.windowSizeMilliseconds
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim other As TimeWindowFunction = DirectCast(obj, TimeWindowFunction)
            Return String.Equals(timeColumn, other.timeColumn) AndAlso windowSize = other.windowSize AndAlso windowSizeUnit = other.windowSizeUnit AndAlso
                offsetAmount = other.offsetAmount AndAlso Nullable.Equals(offsetUnit, other.offsetUnit) AndAlso
                addWindowStartTimeColumn = other.addWindowStartTimeColumn AndAlso addWindowEndTimeColumn = other.addWindowEndTimeColumn AndAlso
                excludeEmptyWindows = other.excludeEmptyWindows AndAlso Object.Equals(inputSchemaField, other.inputSchemaField) AndAlso
                offsetAmountMilliseconds = other.offsetAmountMilliseconds AndAlso windowSizeMilliseconds = other.windowSizeMilliseconds AndAlso
                Object.Equals(timeZone, other.timeZone)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(timeColumn, windowSize, windowSizeUnit, offsetAmount, offsetUnit, addWindowStartTimeColumn, addWindowEndTimeColumn,
                                excludeEmptyWindows, inputSchemaField, offsetAmountMilliseconds, windowSizeMilliseconds, timeZone)
        End Function

        ''' <summary>Fluent builder for a <see cref="TimeWindowFunction"/>.</summary>
        Public Class Builder
            Friend timeColumnField As String
            Friend windowSizeField As Long = -1
            Friend windowSizeUnit As TimeUnitT?
            Friend offsetAmount As Long
            Friend offsetUnit As TimeUnitT?
            Friend addWindowStartTimeColumnField As Boolean = False
            Friend addWindowEndTimeColumnField As Boolean = False
            Friend excludeEmptyWindowsField As Boolean = False

            ''' <summary>Name of the time column.</summary>
            Public Overridable Function timeColumn(ByVal name As String) As Builder
                Me.timeColumnField = name
                Return Me
            End Function

            ''' <summary>Size and unit of the window.</summary>
            Public Overridable Function windowSize(ByVal size As Long, ByVal windowSizeUnit As TimeUnitT) As Builder
                Me.windowSizeField = size
                Me.windowSizeUnit = windowSizeUnit
                Return Me
            End Function

            ''' <summary>Optional offset of the windows.</summary>
            Public Overridable Function offset(ByVal offsetAmount As Long, ByVal offsetUnit As TimeUnitT) As Builder
                Me.offsetAmount = offsetAmount
                Me.offsetUnit = offsetUnit
                Return Me
            End Function

            ''' <summary>Whether to add a "windowStartTime" column.</summary>
            Public Overridable Function addWindowStartTimeColumn(ByVal add As Boolean) As Builder
                Me.addWindowStartTimeColumnField = add
                Return Me
            End Function

            ''' <summary>Whether to add a "windowEndTime" column.</summary>
            Public Overridable Function addWindowEndTimeColumn(ByVal add As Boolean) As Builder
                Me.addWindowEndTimeColumnField = add
                Return Me
            End Function

            ''' <summary>Whether to exclude windows without any time step.</summary>
            Public Overridable Function excludeEmptyWindows(ByVal exclude As Boolean) As Builder
                Me.excludeEmptyWindowsField = exclude
                Return Me
            End Function

            ''' <exception cref="InvalidOperationException">if the time column or the window size/unit is not set</exception>
            Public Overridable Function build() As TimeWindowFunction
                If timeColumnField Is Nothing Then
                    Throw New InvalidOperationException("Time column is null (not specified)")
                End If
                If windowSizeField = -1 OrElse Not windowSizeUnit.HasValue Then
                    Throw New InvalidOperationException("Window size/unit not set")
                End If
                Return New TimeWindowFunction(Me)
            End Function
        End Class

    End Class

End Namespace
