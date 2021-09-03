Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports TimeMetaData = org.datavec.api.transform.metadata.TimeMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSchema = org.datavec.api.transform.schema.SequenceSchema
Imports LongWritable = org.datavec.api.writable.LongWritable
Imports Writable = org.datavec.api.writable.Writable
Imports DateTimeZone = org.joda.time.DateTimeZone

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

Namespace org.datavec.api.transform.sequence.window


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class TimeWindowFunction implements WindowFunction
	<Serializable>
	Public Class TimeWindowFunction
		Implements WindowFunction

		Private ReadOnly timeColumn As String
		Private ReadOnly windowSize As Long
		Private ReadOnly windowSizeUnit As TimeUnit
		Private ReadOnly offsetAmount As Long
		Private ReadOnly offsetUnit As TimeUnit
		Private ReadOnly addWindowStartTimeColumn As Boolean
		Private ReadOnly addWindowEndTimeColumn As Boolean
		Private ReadOnly excludeEmptyWindows As Boolean
'JAVA TO VB CONVERTER NOTE: The field inputSchema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputSchema_Conflict As Schema

		Private ReadOnly offsetAmountMilliseconds As Long
		Private ReadOnly windowSizeMilliseconds As Long

		Private timeZone As DateTimeZone

		''' <summary>
		''' Constructor with zero offset
		''' </summary>
		''' <param name="timeColumn">     Name of the column that contains the time values (must be a time column) </param>
		''' <param name="windowSize">     Numerical quantity for the size of the time window (used in conjunction with windowSizeUnit) </param>
		''' <param name="windowSizeUnit"> Unit of the time window </param>
		Public Sub New(ByVal timeColumn As String, ByVal windowSize As Long, ByVal windowSizeUnit As TimeUnit)
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
		''' <param name="addWindowStartTimeColumn"> If true: add a time column (name: "windowEndTime") that contains the end time
		'''                                 of the window </param>
		Public Sub New(ByVal timeColumn As String, ByVal windowSize As Long, ByVal windowSizeUnit As TimeUnit, ByVal addWindowStartTimeColumn As Boolean, ByVal addWindowEndTimeColumn As Boolean)
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
		Public Sub New(ByVal timeColumn As String, ByVal windowSize As Long, ByVal windowSizeUnit As TimeUnit, ByVal offset As Long, ByVal offsetUnit As TimeUnit)
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
		Public Sub New(ByVal timeColumn As String, ByVal windowSize As Long, ByVal windowSizeUnit As TimeUnit, ByVal offset As Long, ByVal offsetUnit As TimeUnit, ByVal addWindowStartTimeColumn As Boolean, ByVal addWindowEndTimeColumn As Boolean, ByVal excludeEmptyWindows As Boolean)
			Me.timeColumn = timeColumn
			Me.windowSize = windowSize
			Me.windowSizeUnit = windowSizeUnit
			Me.offsetAmount = offset
			Me.offsetUnit = offsetUnit
			Me.addWindowStartTimeColumn = addWindowStartTimeColumn
			Me.addWindowEndTimeColumn = addWindowEndTimeColumn
			Me.excludeEmptyWindows = excludeEmptyWindows

			If offsetAmount = 0 OrElse offsetUnit Is Nothing Then
				Me.offsetAmountMilliseconds = 0
			Else
				Me.offsetAmountMilliseconds = TimeUnit.MILLISECONDS.convert(offset, offsetUnit)
			End If

			Me.windowSizeMilliseconds = TimeUnit.MILLISECONDS.convert(windowSize, windowSizeUnit)
		End Sub

		Private Sub New(ByVal builder As Builder)
			Me.New(builder.timeColumn_Conflict, builder.windowSize_Conflict, builder.windowSizeUnit, builder.offsetAmount, builder.offsetUnit, builder.addWindowStartTimeColumn_Conflict, builder.addWindowEndTimeColumn_Conflict, builder.excludeEmptyWindows_Conflict)
		End Sub

		Public Overridable Property InputSchema Implements WindowFunction.setInputSchema As Schema
			Set(ByVal schema As Schema)
				If Not (TypeOf schema Is SequenceSchema) Then
					Throw New System.ArgumentException("Invalid schema: TimeWindowFunction can " & "only operate on SequenceSchema")
				End If
				If Not schema.hasColumn(timeColumn) Then
					Throw New System.InvalidOperationException("Input schema does not have a column with name """ & timeColumn & """")
				End If
    
				If schema.getMetaData(timeColumn).ColumnType <> ColumnType.Time Then
					Throw New System.InvalidOperationException("Invalid column: column """ & timeColumn & """ is not of type " & ColumnType.Time & "; is " & schema.getMetaData(timeColumn).ColumnType)
				End If
    
				Me.inputSchema_Conflict = schema
    
				timeZone = DirectCast(schema.getMetaData(timeColumn), TimeMetaData).getTimeZone()
			End Set
			Get
				Return inputSchema_Conflict
			End Get
		End Property


		Public Overridable Function transform(ByVal inputSchema As Schema) As Schema Implements WindowFunction.transform
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
			Return "TimeWindowFunction(column=""" & timeColumn & """,windowSize=" & windowSize + windowSizeUnit & ",offset=" & offsetAmount + (If(offsetAmount <> 0 AndAlso offsetUnit IsNot Nothing, offsetUnit, "")) + (If(addWindowStartTimeColumn, ",addWindowStartTimeColumn=true", "")) + (If(addWindowEndTimeColumn, ",addWindowEndTimeColumn=true", "")) + (If(excludeEmptyWindows, ",excludeEmptyWindows=true", "")) & ")"
		End Function


		Public Overridable Function applyToSequence(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of IList(Of Writable))) Implements WindowFunction.applyToSequence

			Dim timeColumnIdx As Integer = inputSchema_Conflict.getIndexOfColumn(Me.timeColumn)

			Dim [out] As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()

			'We are assuming here that the sequence is already ordered (as is usually the case)
			Dim currentWindowStartTime As Long = Long.MinValue
			Dim currentWindow As IList(Of IList(Of Writable)) = Nothing
			For Each timeStep As IList(Of Writable) In sequence
				Dim currentTime As Long = timeStep(timeColumnIdx).toLong()
				Dim windowStartTimeOfThisTimeStep As Long = getWindowStartTimeForTime(currentTime)

				'First time step...
				If currentWindowStartTime = Long.MinValue Then
					currentWindowStartTime = windowStartTimeOfThisTimeStep
					currentWindow = New List(Of IList(Of Writable))()
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
						currentWindow = New List(Of IList(Of Writable))()
						currentWindowStartTime += windowSizeMilliseconds
					Loop
				End If
				If addWindowStartTimeColumn OrElse addWindowEndTimeColumn Then
					Dim timeStep2 As IList(Of Writable) = New List(Of Writable)(timeStep)
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
			If Not (excludeEmptyWindows AndAlso currentWindow.Count = 0) AndAlso currentWindow IsNot Nothing Then
				[out].Add(currentWindow)
			End If

			Return [out]
		End Function


		''' <summary>
		''' Calculates the start time of the window for which the specified time belongs, in unix epoch (millisecond) format<br>
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
		''' interval: i.e., is equivalent to <seealso cref="getWindowStartTimeForTime(Long)"/> + interval (in milliseconds).<br>
		''' To get the last <i>inclusive</i> time for the interval, subtract 1L (1 millisecond) from the value returned by
		''' this method.<br>
		''' For example, if the window size is 1 hour with offset 0, then a time 10:17 would return 11:00, as the 1 hour window
		''' is for 10:00:00.000 (inclusive) to 11:00:00.000 (exclusive)
		''' </summary>
		''' <param name="time"> Time at which to determine the window start time
		''' @return </param>
		Public Overridable Function getWindowEndTimeForTime(ByVal time As Long) As Long
			Return getWindowStartTimeForTime(time) + Me.windowSizeMilliseconds
		End Function

		Public Class Builder
'JAVA TO VB CONVERTER NOTE: The field timeColumn was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend timeColumn_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field windowSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend windowSize_Conflict As Long = -1
			Friend windowSizeUnit As TimeUnit
			Friend offsetAmount As Long
			Friend offsetUnit As TimeUnit
'JAVA TO VB CONVERTER NOTE: The field addWindowStartTimeColumn was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend addWindowStartTimeColumn_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field addWindowEndTimeColumn was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend addWindowEndTimeColumn_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field excludeEmptyWindows was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend excludeEmptyWindows_Conflict As Boolean = False

'JAVA TO VB CONVERTER NOTE: The parameter timeColumn was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function timeColumn(ByVal timeColumn_Conflict As String) As Builder
				Me.timeColumn_Conflict = timeColumn_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter windowSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function windowSize(ByVal windowSize_Conflict As Long, ByVal windowSizeUnit As TimeUnit) As Builder
				Me.windowSize_Conflict = windowSize_Conflict
				Me.windowSizeUnit = windowSizeUnit
				Return Me
			End Function

			Public Overridable Function offset(ByVal offsetAmount As Long, ByVal offsetUnit As TimeUnit) As Builder
				Me.offsetAmount = offsetAmount
				Me.offsetUnit = offsetUnit
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter addWindowStartTimeColumn was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function addWindowStartTimeColumn(ByVal addWindowStartTimeColumn_Conflict As Boolean) As Builder
				Me.addWindowStartTimeColumn_Conflict = addWindowStartTimeColumn_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter addWindowEndTimeColumn was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function addWindowEndTimeColumn(ByVal addWindowEndTimeColumn_Conflict As Boolean) As Builder
				Me.addWindowEndTimeColumn_Conflict = addWindowEndTimeColumn_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter excludeEmptyWindows was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function excludeEmptyWindows(ByVal excludeEmptyWindows_Conflict As Boolean) As Builder
				Me.excludeEmptyWindows_Conflict = excludeEmptyWindows_Conflict
				Return Me
			End Function

			Public Overridable Function build() As TimeWindowFunction
				If timeColumn_Conflict Is Nothing Then
					Throw New System.InvalidOperationException("Time column is null (not specified)")
				End If
				If windowSize_Conflict = -1 OrElse windowSizeUnit Is Nothing Then
					Throw New System.InvalidOperationException("Window size/unit not set")
				End If
				Return New TimeWindowFunction(Me)
			End Function
		End Class
	End Class

End Namespace