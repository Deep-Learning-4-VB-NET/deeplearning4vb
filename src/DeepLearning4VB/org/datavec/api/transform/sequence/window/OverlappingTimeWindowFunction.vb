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
Imports DeepLearning4VB.org.datavec.api.transform.serde
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
    ''' A windowing function based on time, with potentially overlapping windows. Time for each entry in the sequence is
    ''' provided by a Time column. The window size determines how long each window is; the window separation how far
    ''' apart the starts of two consecutive windows are (separation &lt; size: overlapping windows). Windows are
    ''' aligned to multiples of the separation (plus the optional offset; the time zone is not used). Optionally the
    ''' window start and/or end times are added as time columns, and empty windows can be excluded. The sequence must
    ''' already be sorted by time. Equality (Lombok <c>@EqualsAndHashCode(exclude = {"inputSchema",
    ''' "offsetAmountMilliseconds", "windowSizeMilliseconds", "windowSeparationMilliseconds", "timeZone"})</c>)
    ''' compares the configuration.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema", "offsetAmountMilliseconds", "windowSizeMilliseconds",
    '  "windowSeparationMilliseconds", "timeZone"})
    <JsonIgnoreProperties("inputSchema", "offsetAmountMilliseconds", "windowSizeMilliseconds", "windowSeparationMilliseconds", "timeZone")>
    <Serializable>
    Public Class OverlappingTimeWindowFunction
        Implements WindowFunction

        Private ReadOnly timeColumn As String
        Private ReadOnly windowSize As Long
        Private ReadOnly windowSizeUnit As TimeUnitT
        Private ReadOnly windowSeparation As Long
        Private ReadOnly windowSeparationUnit As TimeUnitT
        Private ReadOnly offsetAmount As Long
        Private ReadOnly offsetUnit As TimeUnitT?
        Private ReadOnly addWindowStartTimeColumn As Boolean
        Private ReadOnly addWindowEndTimeColumn As Boolean
        Private ReadOnly excludeEmptyWindows As Boolean
        Private inputSchemaField As SchemaT

        Private ReadOnly offsetAmountMilliseconds As Long
        Private ReadOnly windowSizeMilliseconds As Long
        Private ReadOnly windowSeparationMilliseconds As Long

        Private timeZone As DateTimeZone

        ''' <summary>
        ''' Constructor with zero offset
        ''' </summary>
        ''' <param name="timeColumn">           Name of the column that contains the time values (must be a time column) </param>
        ''' <param name="windowSize">           Numerical quantity for the size of the time window (used in conjunction with windowSizeUnit) </param>
        ''' <param name="windowSizeUnit">       Unit of the time window </param>
        ''' <param name="windowSeparation">     The separation between consecutive window start times (used in conjunction with WindowSeparationUnit) </param>
        ''' <param name="windowSeparationUnit"> Unit for the separation between windows </param>
        Public Sub New(ByVal timeColumn As String, ByVal windowSize As Long, ByVal windowSizeUnit As TimeUnitT, ByVal windowSeparation As Long, ByVal windowSeparationUnit As TimeUnitT)
            Me.New(timeColumn, windowSize, windowSizeUnit, windowSeparation, windowSeparationUnit, 0, Nothing)
        End Sub

        ''' <summary>
        ''' Constructor with zero offset, ability to add window start/end time columns
        ''' </summary>
        ''' <param name="timeColumn">           Name of the column that contains the time values (must be a time column) </param>
        ''' <param name="windowSize">           Numerical quantity for the size of the time window (used in conjunction with windowSizeUnit) </param>
        ''' <param name="windowSizeUnit">       Unit of the time window </param>
        ''' <param name="windowSeparation">     The separation between consecutive window start times (used in conjunction with WindowSeparationUnit) </param>
        ''' <param name="windowSeparationUnit"> Unit for the separation between windows </param>
        ''' <param name="addWindowStartTimeColumn"> If true: add a column (at the end) with the window start time </param>
        ''' <param name="addWindowEndTimeColumn">   If true: add a column (at the end) with the window end time </param>
        Public Sub New(ByVal timeColumn As String, ByVal windowSize As Long, ByVal windowSizeUnit As TimeUnitT, ByVal windowSeparation As Long, ByVal windowSeparationUnit As TimeUnitT, ByVal addWindowStartTimeColumn As Boolean, ByVal addWindowEndTimeColumn As Boolean)
            Me.New(timeColumn, windowSize, windowSizeUnit, windowSeparation, windowSeparationUnit, 0, Nothing, addWindowStartTimeColumn, addWindowEndTimeColumn, False)
        End Sub

        ''' <summary>
        ''' Constructor with optional offset
        ''' </summary>
        ''' <param name="timeColumn">           Name of the column that contains the time values (must be a time column) </param>
        ''' <param name="windowSize">           Numerical quantity for the size of the time window (used in conjunction with windowSizeUnit) </param>
        ''' <param name="windowSizeUnit">       Unit of the time window </param>
        ''' <param name="windowSeparation">     The separation between consecutive window start times (used in conjunction with WindowSeparationUnit) </param>
        ''' <param name="windowSeparationUnit"> Unit for the separation between windows </param>
        ''' <param name="offset">               Optional offset amount, to shift start/end of the time window forward or back </param>
        ''' <param name="offsetUnit">           Optional offset unit for the offset amount. </param>
        Public Sub New(ByVal timeColumn As String, ByVal windowSize As Long, ByVal windowSizeUnit As TimeUnitT, ByVal windowSeparation As Long, ByVal windowSeparationUnit As TimeUnitT, ByVal offset As Long, ByVal offsetUnit As TimeUnitT?)
            Me.New(timeColumn, windowSize, windowSizeUnit, windowSeparation, windowSeparationUnit, offset, offsetUnit, False, False, False)
        End Sub

        ''' <summary>
        ''' Constructor with optional offset, ability to add window start/end time columns
        ''' </summary>
        ''' <param name="timeColumn">               Name of the column that contains the time values (must be a time column) </param>
        ''' <param name="windowSize">               Numerical quantity for the size of the time window (used in conjunction with windowSizeUnit) </param>
        ''' <param name="windowSizeUnit">           Unit of the time window </param>
        ''' <param name="windowSeparation">         The separation between consecutive window start times (used in conjunction with WindowSeparationUnit) </param>
        ''' <param name="windowSeparationUnit">     Unit for the separation between windows </param>
        ''' <param name="offset">                   Optional offset amount, to shift start/end of the time window forward or back </param>
        ''' <param name="offsetUnit">               Optional offset unit for the offset amount. </param>
        ''' <param name="addWindowStartTimeColumn"> If true: add a column (at the end) with the window start time </param>
        ''' <param name="addWindowEndTimeColumn">   If true: add a column (at the end) with the window end time </param>
        ''' <param name="excludeEmptyWindows">      If true: exclude any windows that don't have any values in them </param>
        <JsonConstructor>
        Public Sub New(ByVal timeColumn As String, ByVal windowSize As Long, ByVal windowSizeUnit As TimeUnitT, ByVal windowSeparation As Long, ByVal windowSeparationUnit As TimeUnitT, <JsonProperty("offsetAmount")> ByVal offset As Long, ByVal offsetUnit As TimeUnitT?, ByVal addWindowStartTimeColumn As Boolean, ByVal addWindowEndTimeColumn As Boolean, ByVal excludeEmptyWindows As Boolean)
            Me.timeColumn = timeColumn
            Me.windowSize = windowSize
            Me.windowSizeUnit = windowSizeUnit
            Me.windowSeparation = windowSeparation
            Me.windowSeparationUnit = windowSeparationUnit
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
            Me.windowSeparationMilliseconds = TimeUnitT.MILLISECONDS.convert(windowSeparation, windowSeparationUnit)
        End Sub

        Private Sub New(ByVal builder As Builder)
            Me.New(builder.timeColumnField, builder.windowSizeField, builder.windowSizeUnit.Value, builder.windowSeparationField, builder.windowSeparationUnit.Value, builder.offsetAmount, builder.offsetUnit, builder.addWindowStartTimeColumnField, builder.addWindowEndTimeColumnField, builder.excludeEmptyWindowsField)
        End Sub

        ''' <summary>
        ''' The input schema: must be a sequence schema with a time column of the configured name.
        ''' </summary>
        ''' <exception cref="ArgumentException">if the schema is not a sequence schema</exception>
        ''' <exception cref="InvalidOperationException">if the column is missing or not a time column</exception>
        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements WindowFunction.InputSchema
            Get
                Return inputSchemaField
            End Get
            Set(ByVal value As SchemaT)
                If Not (TypeOf value Is SequenceSchemaT) Then
                    Throw New ArgumentException("Invalid schema: OverlappingTimeWindowFunction can only operate on SequenceSchema")
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

        ''' <summary>Java <c>getWindowSeparation()</c> (Lombok).</summary>
        Public Function getWindowSeparation() As Long
            Return windowSeparation
        End Function

        ''' <summary>Java <c>getWindowSeparationUnit()</c> (Lombok).</summary>
        Public Function getWindowSeparationUnit() As TimeUnitT
            Return windowSeparationUnit
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

        ''' <summary>Java <c>getWindowSeparationMilliseconds()</c> (Lombok).</summary>
        Public Function getWindowSeparationMilliseconds() As Long
            Return windowSeparationMilliseconds
        End Function

        ''' <summary>Java <c>getTimeZone()</c> (Lombok): the time zone of the time column, known once the schema is set.</summary>
        Public Function getTimeZone() As DateTimeZone
            Return timeZone
        End Function

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
            Return "OverlappingTimeWindowFunction(columnName=""" & timeColumn & """,windowSize=" & windowSize & windowSizeUnit.ToString() &
                ",windowSeparation=" & windowSeparation & windowSeparationUnit.ToString() & ",offset=" & offsetAmount &
                (If(offsetAmount <> 0 AndAlso offsetUnit.HasValue, offsetUnit.Value.ToString(), "")) &
                (If(addWindowStartTimeColumn, ",addWindowStartTimeColumn=true", "")) &
                (If(addWindowEndTimeColumn, ",addWindowEndTimeColumn=true", "")) &
                (If(excludeEmptyWindows, ",excludeEmptyWindows=true", "")) & ")"
        End Function

        ''' <summary>Splits the (time-sorted, non-empty) sequence into the (possibly overlapping) time windows.</summary>
        Public Overridable Function applyToSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of IList(Of WritableT))) Implements WindowFunction.applyToSequence

            Dim timeColumnIdx As Integer = inputSchemaField.getIndexOfColumn(Me.timeColumn)

            Dim [out] As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()

            'We are assuming here that the sequence is already ordered (as is usually the case)

            'First: work out the window to start on. The window to start on is the first window that includes the first time step values
            Dim firstTimeStepTimePlusOffset As Long = sequence(0)(timeColumnIdx).toLong() + offsetAmountMilliseconds
            Dim windowBorder As Long = firstTimeStepTimePlusOffset - (firstTimeStepTimePlusOffset Mod windowSeparationMilliseconds) 'Round down to time where a window starts/ends
            'At this windowBorder time: the window that _ends_ at windowBorder does NOT include the first time step
            ' Therefore the window that ends at windowBorder+1*windowSeparation is first window that includes the first data point

            'Second: work out the window to end on. The window to end on is the last window that includes the last time step values
            Dim lastTimeStepTimePlusOffset As Long = sequence(sequence.Count - 1)(timeColumnIdx).toLong() + offsetAmountMilliseconds
            Dim windowBorderLastTimeStep As Long = lastTimeStepTimePlusOffset - (lastTimeStepTimePlusOffset Mod windowSeparationMilliseconds)
            'At this windowBorderLastTimeStep time: the window that _starts_ this time is the last window to include the last time step
            Dim lastWindowStartTime As Long = windowBorderLastTimeStep

            Dim currentWindowStartTime As Long = windowBorder + windowSeparationMilliseconds - windowSizeMilliseconds
            Dim nextWindowStartTime As Long = currentWindowStartTime + windowSeparationMilliseconds
            Dim currentWindowEndTime As Long = currentWindowStartTime + windowSizeMilliseconds

            Dim currentWindow As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            Dim currentWindowStartIdx As Integer = 0
            Dim sequenceLength As Integer = sequence.Count
            Dim foundIndexForNextWindowStart As Boolean = False
            Do While currentWindowStartTime <= lastWindowStartTime
                For i As Integer = currentWindowStartIdx To sequenceLength - 1
                    Dim timeStep As IList(Of WritableT) = sequence(i)
                    Dim currentTime As Long = timeStep(timeColumnIdx).toLong()

                    'As we go through: let's keep track of the index of the first element in the next window
                    If Not foundIndexForNextWindowStart AndAlso currentTime >= nextWindowStartTime Then
                        foundIndexForNextWindowStart = True
                        currentWindowStartIdx = i
                    End If
                    Dim nextWindow As Boolean = False
                    If currentTime < currentWindowEndTime Then
                        'This time step is included in the current window
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
                    Else
                        'This time step is NOT included in the current window -> done with the current window -> start the next window
                        nextWindow = True
                    End If

                    'Once we reach the end of the input sequence: we might have added it to the current time step, but still
                    ' need to create the next window
                    If i = sequenceLength - 1 Then
                        nextWindow = True
                    End If

                    If nextWindow Then
                        If Not (excludeEmptyWindows AndAlso currentWindow.Count = 0) Then
                            [out].Add(currentWindow)
                        End If
                        currentWindow = New List(Of IList(Of WritableT))()
                        currentWindowStartTime = currentWindowStartTime + windowSeparationMilliseconds
                        currentWindowEndTime = currentWindowStartTime + windowSizeMilliseconds
                        foundIndexForNextWindowStart = False
                        nextWindowStartTime = currentWindowStartTime + windowSeparationMilliseconds
                        Exit For
                    End If
                Next i
            Loop

            Return [out]
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim other As OverlappingTimeWindowFunction = DirectCast(obj, OverlappingTimeWindowFunction)
            Return String.Equals(timeColumn, other.timeColumn) AndAlso windowSize = other.windowSize AndAlso windowSizeUnit = other.windowSizeUnit AndAlso
                windowSeparation = other.windowSeparation AndAlso windowSeparationUnit = other.windowSeparationUnit AndAlso
                offsetAmount = other.offsetAmount AndAlso Nullable.Equals(offsetUnit, other.offsetUnit) AndAlso
                addWindowStartTimeColumn = other.addWindowStartTimeColumn AndAlso addWindowEndTimeColumn = other.addWindowEndTimeColumn AndAlso
                excludeEmptyWindows = other.excludeEmptyWindows
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(timeColumn, windowSize, windowSizeUnit, windowSeparation, windowSeparationUnit, offsetAmount, offsetUnit,
                                addWindowStartTimeColumn, addWindowEndTimeColumn, excludeEmptyWindows)
        End Function

        ''' <summary>Fluent builder for an <see cref="OverlappingTimeWindowFunction"/>.</summary>
        Public Class Builder
            Friend timeColumnField As String
            Friend windowSizeField As Long = -1
            Friend windowSizeUnit As TimeUnitT?
            Friend windowSeparationField As Long = -1
            Friend windowSeparationUnit As TimeUnitT?
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

            ''' <summary>Size and unit of each window.</summary>
            Public Overridable Function windowSize(ByVal size As Long, ByVal windowSizeUnit As TimeUnitT) As Builder
                Me.windowSizeField = size
                Me.windowSizeUnit = windowSizeUnit
                Return Me
            End Function

            ''' <summary>Separation (and its unit) between the starts of consecutive windows.</summary>
            Public Overridable Function windowSeparation(ByVal separation As Long, ByVal windowSeparationUnit As TimeUnitT) As Builder
                Me.windowSeparationField = separation
                Me.windowSeparationUnit = windowSeparationUnit
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

            ''' <exception cref="InvalidOperationException">if the time column, the window size/unit or the separation/unit is not set</exception>
            Public Overridable Function build() As OverlappingTimeWindowFunction
                If timeColumnField Is Nothing Then
                    Throw New InvalidOperationException("Time column is null (not specified)")
                End If
                If windowSizeField = -1 OrElse Not windowSizeUnit.HasValue Then
                    Throw New InvalidOperationException("Window size/unit not set")
                End If
                If windowSeparationField = -1 OrElse Not windowSeparationUnit.HasValue Then
                    Throw New InvalidOperationException("Window separation and/or unit not set")
                End If
                Return New OverlappingTimeWindowFunction(Me)
            End Function
        End Class

    End Class

End Namespace
