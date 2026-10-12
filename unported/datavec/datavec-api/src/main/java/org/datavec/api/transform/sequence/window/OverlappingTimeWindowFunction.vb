Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports TimeMetaData = org.datavec.api.transform.metadata.TimeMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSchema = org.datavec.api.transform.schema.SequenceSchema
Imports LongWritable = org.datavec.api.writable.LongWritable
Imports Writable = org.datavec.api.writable.Writable
Imports DateTimeZone = org.joda.time.DateTimeZone
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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
'ORIGINAL LINE: @JsonIgnoreProperties({"inputSchema", "offsetAmountMilliseconds", "windowSizeMilliseconds", "windowSeparationMilliseconds", "timeZone"}) @EqualsAndHashCode(exclude = {"inputSchema", "offsetAmountMilliseconds", "windowSizeMilliseconds", "windowSeparationMilliseconds", "timeZone"}) @Data public class OverlappingTimeWindowFunction implements WindowFunction
	<Serializable>
	Public Class OverlappingTimeWindowFunction
		Implements WindowFunction

		Private ReadOnly timeColumn As String
		Private ReadOnly windowSize As Long
		Private ReadOnly windowSizeUnit As TimeUnit
		Private ReadOnly windowSeparation As Long
		Private ReadOnly windowSeparationUnit As TimeUnit
		Private ReadOnly offsetAmount As Long
		Private ReadOnly offsetUnit As TimeUnit
		Private ReadOnly addWindowStartTimeColumn As Boolean
		Private ReadOnly addWindowEndTimeColumn As Boolean
		Private ReadOnly excludeEmptyWindows As Boolean
'JAVA TO VB CONVERTER NOTE: The field inputSchema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputSchema_Conflict As Schema

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
		Public Sub New(ByVal timeColumn As String, ByVal windowSize As Long, ByVal windowSizeUnit As TimeUnit, ByVal windowSeparation As Long, ByVal windowSeparationUnit As TimeUnit)
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
		Public Sub New(ByVal timeColumn As String, ByVal windowSize As Long, ByVal windowSizeUnit As TimeUnit, ByVal windowSeparation As Long, ByVal windowSeparationUnit As TimeUnit, ByVal addWindowStartTimeColumn As Boolean, ByVal addWindowEndTimeColumn As Boolean)
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
		Public Sub New(ByVal timeColumn As String, ByVal windowSize As Long, ByVal windowSizeUnit As TimeUnit, ByVal windowSeparation As Long, ByVal windowSeparationUnit As TimeUnit, ByVal offset As Long, ByVal offsetUnit As TimeUnit)
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
		''' <param name="addWindowStartTimeColumn"> If true: add a time column (name: "windowStartTime") that contains the start time
		'''                                 of the window </param>
		''' <param name="addWindowEndTimeColumn">   If true: add a time column (name: "windowEndTime") that contains the end time
		'''                                 of the window </param>
		''' <param name="excludeEmptyWindows">      If true: exclude any windows that don't have any values in them </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public OverlappingTimeWindowFunction(@JsonProperty("timeColumn") String timeColumn, @JsonProperty("windowSize") long windowSize, @JsonProperty("windowSizeUnit") java.util.concurrent.TimeUnit windowSizeUnit, @JsonProperty("windowSeparation") long windowSeparation, @JsonProperty("windowSeparationUnit") java.util.concurrent.TimeUnit windowSeparationUnit, @JsonProperty("offset") long offset, @JsonProperty("offsetUnit") java.util.concurrent.TimeUnit offsetUnit, @JsonProperty("addWindowStartTimeColumn") boolean addWindowStartTimeColumn, @JsonProperty("addWindowEndTimeColumn") boolean addWindowEndTimeColumn, @JsonProperty("excludeEmptyWindows") boolean excludeEmptyWindows)
		Public Sub New(ByVal timeColumn As String, ByVal windowSize As Long, ByVal windowSizeUnit As TimeUnit, ByVal windowSeparation As Long, ByVal windowSeparationUnit As TimeUnit, ByVal offset As Long, ByVal offsetUnit As TimeUnit, ByVal addWindowStartTimeColumn As Boolean, ByVal addWindowEndTimeColumn As Boolean, ByVal excludeEmptyWindows As Boolean)
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

			If offsetAmount = 0 OrElse offsetUnit Is Nothing Then
				Me.offsetAmountMilliseconds = 0
			Else
				Me.offsetAmountMilliseconds = TimeUnit.MILLISECONDS.convert(offset, offsetUnit)
			End If

			Me.windowSizeMilliseconds = TimeUnit.MILLISECONDS.convert(windowSize, windowSizeUnit)
			Me.windowSeparationMilliseconds = TimeUnit.MILLISECONDS.convert(windowSeparation, windowSeparationUnit)
		End Sub

		Private Sub New(ByVal builder As Builder)
			Me.New(builder.timeColumn_Conflict, builder.windowSize_Conflict, builder.windowSizeUnit, builder.windowSeparation_Conflict, builder.windowSeparationUnit, builder.offsetAmount, builder.offsetUnit, builder.addWindowStartTimeColumn_Conflict, builder.addWindowEndTimeColumn_Conflict, builder.excludeEmptyWindows_Conflict)
		End Sub

		Public Overridable Property InputSchema Implements WindowFunction.setInputSchema As Schema
			Set(ByVal schema As Schema)
				If Not (TypeOf schema Is SequenceSchema) Then
					Throw New System.ArgumentException("Invalid schema: OverlappingTimeWindowFunction can only operate on SequenceSchema")
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
			Return "OverlappingTimeWindowFunction(columnName=""" & timeColumn & """,windowSize=" & windowSize + windowSizeUnit & ",windowSeparation=" & windowSeparation + windowSeparationUnit & ",offset=" & offsetAmount + (If(offsetAmount <> 0 AndAlso offsetUnit IsNot Nothing, offsetUnit, "")) + (If(addWindowStartTimeColumn, ",addWindowStartTimeColumn=true", "")) + (If(addWindowEndTimeColumn, ",addWindowEndTimeColumn=true", "")) + (If(excludeEmptyWindows, ",excludeEmptyWindows=true", "")) & ")"
		End Function


		Public Overridable Function applyToSequence(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of IList(Of Writable))) Implements WindowFunction.applyToSequence

			Dim timeColumnIdx As Integer = inputSchema_Conflict.getIndexOfColumn(Me.timeColumn)

			Dim [out] As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()

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
			Dim currentWindow As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()

			Dim currentWindowStartIdx As Integer = 0
			Dim sequenceLength As Integer = sequence.Count
			Dim foundIndexForNextWindowStart As Boolean = False
			Do While currentWindowStartTime <= lastWindowStartTime

				For i As Integer = currentWindowStartIdx To sequenceLength - 1
					Dim timeStep As IList(Of Writable) = sequence(i)
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
						currentWindow = New List(Of IList(Of Writable))()
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

		Public Class Builder
'JAVA TO VB CONVERTER NOTE: The field timeColumn was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend timeColumn_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field windowSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend windowSize_Conflict As Long = -1
			Friend windowSizeUnit As TimeUnit
'JAVA TO VB CONVERTER NOTE: The field windowSeparation was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend windowSeparation_Conflict As Long = -1
			Friend windowSeparationUnit As TimeUnit
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

'JAVA TO VB CONVERTER NOTE: The parameter windowSeparation was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function windowSeparation(ByVal windowSeparation_Conflict As Long, ByVal windowSeparationUnit As TimeUnit) As Builder
				Me.windowSeparation_Conflict = windowSeparation_Conflict
				Me.windowSeparationUnit = windowSeparationUnit
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

			Public Overridable Function build() As OverlappingTimeWindowFunction
				If timeColumn_Conflict Is Nothing Then
					Throw New System.InvalidOperationException("Time column is null (not specified)")
				End If
				If windowSize_Conflict = -1 OrElse windowSizeUnit Is Nothing Then
					Throw New System.InvalidOperationException("Window size/unit not set")
				End If
				If windowSeparation_Conflict = -1 OrElse windowSeparationUnit Is Nothing Then
					Throw New System.InvalidOperationException("Window separation and/or unit not set")
				End If
				Return New OverlappingTimeWindowFunction(Me)
			End Function
		End Class
	End Class

End Namespace