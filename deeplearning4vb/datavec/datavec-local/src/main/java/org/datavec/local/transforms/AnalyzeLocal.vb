Imports System.Collections.Generic
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports org.datavec.api.transform.analysis
Imports DataAnalysis = org.datavec.api.transform.analysis.DataAnalysis
Imports DataVecAnalysisUtils = org.datavec.api.transform.analysis.DataVecAnalysisUtils
Imports ColumnAnalysis = org.datavec.api.transform.analysis.columns.ColumnAnalysis
Imports HistogramCounter = org.datavec.api.transform.analysis.histogram.HistogramCounter
Imports QualityAnalysisAddFunction = org.datavec.api.transform.analysis.quality.QualityAnalysisAddFunction
Imports org.datavec.api.transform.analysis.quality
Imports DataQualityAnalysis = org.datavec.api.transform.quality.DataQualityAnalysis
Imports ColumnQuality = org.datavec.api.transform.quality.columns.ColumnQuality
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
Imports AnalysisAddFunction = org.datavec.local.transforms.analysis.aggregate.AnalysisAddFunction
Imports HistogramAddFunction = org.datavec.local.transforms.analysis.histogram.HistogramAddFunction

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

Namespace org.datavec.local.transforms


	Public Class AnalyzeLocal
		Private Const DEFAULT_MAX_HISTOGRAM_BUCKETS As Integer = 30

		''' <summary>
		''' Analyse the specified data - returns a DataAnalysis object with summary information about each column
		''' </summary>
		''' <param name="schema"> Schema for data </param>
		''' <param name="rr">     Data to analyze </param>
		''' <returns> DataAnalysis for data </returns>
		Public Shared Function analyze(ByVal schema As Schema, ByVal rr As RecordReader) As DataAnalysis
			Return analyze(schema, rr, DEFAULT_MAX_HISTOGRAM_BUCKETS)
		End Function

		''' <summary>
		''' Analyse the specified data - returns a DataAnalysis object with summary information about each column
		''' </summary>
		''' <param name="schema"> Schema for data </param>
		''' <param name="rr">     Data to analyze </param>
		''' <returns> DataAnalysis for data </returns>
		Public Shared Function analyze(ByVal schema As Schema, ByVal rr As RecordReader, ByVal maxHistogramBuckets As Integer) As DataAnalysis
			Dim addFn As New AnalysisAddFunction(schema)
			Dim counters As IList(Of AnalysisCounter) = Nothing
			Do While rr.hasNext()
				counters = addFn.apply(counters, rr.next())
			Loop

'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim minsMaxes[][] As Double = new Double[counters.Count][2]
			Dim minsMaxes()() As Double = RectangularArrays.RectangularDoubleArray(counters.Count, 2)

			Dim columnTypes As IList(Of ColumnType) = schema.getColumnTypes()
			Dim list As IList(Of ColumnAnalysis) = DataVecAnalysisUtils.convertCounters(counters, minsMaxes, columnTypes)


			'Do another pass collecting histogram values:
			Dim histogramCounters As IList(Of HistogramCounter) = Nothing
			Dim add As New HistogramAddFunction(maxHistogramBuckets, schema, minsMaxes)
			If rr.resetSupported() Then
				rr.reset()
				Do While rr.hasNext()
					histogramCounters = add.apply(histogramCounters, rr.next())
				Loop

				DataVecAnalysisUtils.mergeCounters(list, histogramCounters)
			End If

			Return New DataAnalysis(schema, list)
		End Function


		''' <summary>
		''' Analyze the data quality of sequence data - provides a report on missing values, values that don't comply with schema, etc </summary>
		''' <param name="schema"> Schema for data </param>
		''' <param name="data">   Data to analyze </param>
		''' <returns> DataQualityAnalysis object </returns>
		Public Shared Function analyzeQualitySequence(ByVal schema As Schema, ByVal data As SequenceRecordReader) As DataQualityAnalysis
			Dim nColumns As Integer = schema.numColumns()
			Dim states As IList(Of QualityAnalysisState) = New List(Of QualityAnalysisState)()
			Dim addFn As New QualityAnalysisAddFunction(schema)
			Do While data.hasNext()
				Dim seq As IList(Of IList(Of Writable)) = data.sequenceRecord()
				For Each [step] As IList(Of Writable) In seq
					states = addFn.apply(states, [step])
				Next [step]
			Loop

			Dim list As IList(Of ColumnQuality) = New List(Of ColumnQuality)(nColumns)

			For Each qualityState As QualityAnalysisState In states
				list.Add(qualityState.getColumnQuality())
			Next qualityState
			Return New DataQualityAnalysis(schema, list)
		End Function

		''' <summary>
		''' Analyze the data quality of data - provides a report on missing values, values that don't comply with schema, etc </summary>
		''' <param name="schema"> Schema for data </param>
		''' <param name="data">   Data to analyze </param>
		''' <returns> DataQualityAnalysis object </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static org.datavec.api.transform.quality.DataQualityAnalysis analyzeQuality(final org.datavec.api.transform.schema.Schema schema, final org.datavec.api.records.reader.RecordReader data)
		Public Shared Function analyzeQuality(ByVal schema As Schema, ByVal data As RecordReader) As DataQualityAnalysis
			Dim nColumns As Integer = schema.numColumns()
			Dim states As IList(Of QualityAnalysisState) = Nothing
			Dim addFn As New QualityAnalysisAddFunction(schema)
			Do While data.hasNext()
				states = addFn.apply(states, data.next())
			Loop

			Dim list As IList(Of ColumnQuality) = New List(Of ColumnQuality)(nColumns)

			For Each qualityState As QualityAnalysisState In states
				list.Add(qualityState.getColumnQuality())
			Next qualityState
			Return New DataQualityAnalysis(schema, list)
		End Function

		''' <summary>
		''' Get a list of unique values from the specified columns.
		''' For sequence data, use <seealso cref="getUniqueSequence(List, Schema, SequenceRecordReader)"/>
		''' </summary>
		''' <param name="columnName">    Name of the column to get unique values from </param>
		''' <param name="schema">        Data schema </param>
		''' <param name="data">          Data to get unique values from </param>
		''' <returns>              List of unique values </returns>
		Public Shared Function getUnique(ByVal columnName As String, ByVal schema As Schema, ByVal data As RecordReader) As ISet(Of Writable)
			Dim colIdx As Integer = schema.getIndexOfColumn(columnName)
			Dim unique As ISet(Of Writable) = New HashSet(Of Writable)()
			Do While data.hasNext()
				Dim [next] As IList(Of Writable) = data.next()
				unique.Add([next](colIdx))
			Loop
			Return unique
		End Function

		''' <summary>
		''' Get a list of unique values from the specified columns.
		''' For sequence data, use <seealso cref="getUniqueSequence(String, Schema, SequenceRecordReader)"/>
		''' </summary>
		''' <param name="columnNames">   Names of the column to get unique values from </param>
		''' <param name="schema">        Data schema </param>
		''' <param name="data">          Data to get unique values from </param>
		''' <returns>              List of unique values, for each of the specified columns </returns>
		Public Shared Function getUnique(ByVal columnNames As IList(Of String), ByVal schema As Schema, ByVal data As RecordReader) As IDictionary(Of String, ISet(Of Writable))
			Dim m As IDictionary(Of String, ISet(Of Writable)) = New Dictionary(Of String, ISet(Of Writable))()
			For Each s As String In columnNames
				m(s) = New HashSet(Of Writable)()
			Next s
			Do While data.hasNext()
				Dim [next] As IList(Of Writable) = data.next()
				For Each s As String In columnNames
					Dim idx As Integer = schema.getIndexOfColumn(s)
					m(s).Add([next](idx))
				Next s
			Loop
			Return m
		End Function

		''' <summary>
		''' Get a list of unique values from the specified column of a sequence
		''' </summary>
		''' <param name="columnName">      Name of the column to get unique values from </param>
		''' <param name="schema">          Data schema </param>
		''' <param name="sequenceData">    Sequence data to get unique values from
		''' @return </param>
		Public Shared Function getUniqueSequence(ByVal columnName As String, ByVal schema As Schema, ByVal sequenceData As SequenceRecordReader) As ISet(Of Writable)
			Dim colIdx As Integer = schema.getIndexOfColumn(columnName)
			Dim unique As ISet(Of Writable) = New HashSet(Of Writable)()
			Do While sequenceData.hasNext()
				Dim [next] As IList(Of IList(Of Writable)) = sequenceData.sequenceRecord()
				For Each [step] As IList(Of Writable) In [next]
					unique.Add([step](colIdx))
				Next [step]
			Loop
			Return unique
		End Function

		''' <summary>
		''' Get a list of unique values from the specified columns of a sequence
		''' </summary>
		''' <param name="columnNames">     Name of the columns to get unique values from </param>
		''' <param name="schema">          Data schema </param>
		''' <param name="sequenceData">    Sequence data to get unique values from
		''' @return </param>
		Public Shared Function getUniqueSequence(ByVal columnNames As IList(Of String), ByVal schema As Schema, ByVal sequenceData As SequenceRecordReader) As IDictionary(Of String, ISet(Of Writable))
			Dim m As IDictionary(Of String, ISet(Of Writable)) = New Dictionary(Of String, ISet(Of Writable))()
			For Each s As String In columnNames
				m(s) = New HashSet(Of Writable)()
			Next s
			Do While sequenceData.hasNext()
				Dim [next] As IList(Of IList(Of Writable)) = sequenceData.sequenceRecord()
				For Each [step] As IList(Of Writable) In [next]
					For Each s As String In columnNames
						Dim idx As Integer = schema.getIndexOfColumn(s)
						m(s).Add([step](idx))
					Next s
				Next [step]
			Loop
			Return m
		End Function
	End Class

End Namespace