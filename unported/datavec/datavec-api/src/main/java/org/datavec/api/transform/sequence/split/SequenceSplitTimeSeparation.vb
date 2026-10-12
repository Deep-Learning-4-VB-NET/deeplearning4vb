Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSplit = org.datavec.api.transform.sequence.SequenceSplit
Imports Writable = org.datavec.api.writable.Writable
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

Namespace org.datavec.api.transform.sequence.split


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"separationMilliseconds", "timeColumnIdx", "schema"}) @EqualsAndHashCode(exclude = {"separationMilliseconds", "timeColumnIdx", "schema"}) @Data public class SequenceSplitTimeSeparation implements org.datavec.api.transform.sequence.SequenceSplit
	<Serializable>
	Public Class SequenceSplitTimeSeparation
		Implements SequenceSplit

		Private ReadOnly timeColumn As String
		Private ReadOnly timeQuantity As Long
		Private ReadOnly timeUnit As TimeUnit
		Private ReadOnly separationMilliseconds As Long
		Private timeColumnIdx As Integer = -1
		Private schema As Schema

		''' <param name="timeColumn">      Time column to consider when splitting </param>
		''' <param name="timeQuantity">    Value/amount (of the specified TimeUnit) </param>
		''' <param name="timeUnit">        The unit of time </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SequenceSplitTimeSeparation(@JsonProperty("timeColumn") String timeColumn, @JsonProperty("timeQuantity") long timeQuantity, @JsonProperty("timeUnit") java.util.concurrent.TimeUnit timeUnit)
		Public Sub New(ByVal timeColumn As String, ByVal timeQuantity As Long, ByVal timeUnit As TimeUnit)
			Me.timeColumn = timeColumn
			Me.timeQuantity = timeQuantity
			Me.timeUnit = timeUnit

			Me.separationMilliseconds = TimeUnit.MILLISECONDS.convert(timeQuantity, timeUnit)
		End Sub

		Public Overridable Function split(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of IList(Of Writable))) Implements SequenceSplit.split

			Dim [out] As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()

			Dim lastTimeStepTime As Long = Long.MinValue
			Dim currentSplit As IList(Of IList(Of Writable)) = Nothing

			For Each timeStep As IList(Of Writable) In sequence
				Dim currStepTime As Long = timeStep(timeColumnIdx).toLong()
				If lastTimeStepTime = Long.MinValue OrElse (currStepTime - lastTimeStepTime) > separationMilliseconds Then
					'New split
					If currentSplit IsNot Nothing Then
						[out].Add(currentSplit)
					End If
					currentSplit = New List(Of IList(Of Writable))()
				End If
				currentSplit.Add(timeStep)
				lastTimeStepTime = currStepTime
			Next timeStep

			'Add the final split to the output...
			[out].Add(currentSplit)

			Return [out]
		End Function

		Public Overridable Property InputSchema Implements SequenceSplit.setInputSchema As Schema
			Set(ByVal inputSchema As Schema)
				If Not inputSchema.hasColumn(timeColumn) Then
					Throw New System.InvalidOperationException("Invalid state: schema does not have column " & "with name """ & timeColumn & """")
				End If
				If inputSchema.getMetaData(timeColumn).ColumnType <> ColumnType.Time Then
					Throw New System.InvalidOperationException("Invalid input schema: schema column """ & timeColumn & """ is not a time column." & " (Is type: " & inputSchema.getMetaData(timeColumn).ColumnType & ")")
				End If
    
				Me.timeColumnIdx = inputSchema.getIndexOfColumn(timeColumn)
				Me.schema = inputSchema
			End Set
			Get
				Return schema
			End Get
		End Property


		Public Overrides Function ToString() As String
			Return "SequenceSplitTimeSeparation(timeColumn=""" & timeColumn & """,timeQuantity=" & timeQuantity & ",timeUnit=" & timeUnit & ")"
		End Function
	End Class

End Namespace