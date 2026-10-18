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
Imports DeepLearning4VB.org.datavec.api.transform.analysis.quality
Imports AnalysisAddFunctionT = DeepLearning4VB.org.datavec.local.transforms.analysis.aggregate.AnalysisAddFunction
Imports AnalysisCounterT = DeepLearning4VB.org.datavec.api.transform.analysis.AnalysisCounter
Imports ColumnAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.ColumnAnalysis
Imports ColumnQualityT = DeepLearning4VB.org.datavec.api.transform.quality.columns.ColumnQuality
Imports ColumnTypeT = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports DataAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.DataAnalysis
Imports DataQualityAnalysisT = DeepLearning4VB.org.datavec.api.transform.quality.DataQualityAnalysis
Imports DataVecAnalysisUtilsT = DeepLearning4VB.org.datavec.api.transform.analysis.DataVecAnalysisUtils
Imports HistogramAddFunctionT = DeepLearning4VB.org.datavec.local.transforms.analysis.histogram.HistogramAddFunction
Imports HistogramCounterT = DeepLearning4VB.org.datavec.api.transform.analysis.histogram.HistogramCounter
Imports RecordReaderT = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceRecordReaderT = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms

    ''' <summary>
    ''' Analysis of the data of a record reader on the local machine: per-column summary statistics and histograms
    ''' (<see cref="analyze"/>), data quality reports (<see cref="analyzeQuality"/>) and the unique values of columns
    ''' (<see cref="getUnique"/>).
    ''' <para>
    ''' As in Java, <see cref="analyze"/> supports String, Integer, Long, Double, Categorical, Time and Bytes columns
    ''' (Float and Boolean columns throw an <see cref="ArgumentException"/>, NDArray columns a
    ''' <see cref="NotSupportedException"/>), and the quality analysis the column types supported by
    ''' <c>QualityAnalysisAddFunction</c>.
    ''' </para>
    ''' </summary>
    Public Class AnalyzeLocal

        Private Const DEFAULT_MAX_HISTOGRAM_BUCKETS As Integer = 30

        ''' <summary>
        ''' Analyse the specified data - returns a DataAnalysis object with summary information about each column
        ''' </summary>
        ''' <param name="schema"> Schema for data </param>
        ''' <param name="rr">     Data to analyze </param>
        ''' <returns> DataAnalysis for data </returns>
        Public Shared Function analyze(ByVal schema As SchemaT, ByVal rr As RecordReaderT) As DataAnalysisT
            Return analyze(schema, rr, DEFAULT_MAX_HISTOGRAM_BUCKETS)
        End Function

        ''' <summary>
        ''' Analyse the specified data - returns a DataAnalysis object with summary information about each column.
        ''' The histograms are computed in a second pass over the data, so they are only included when the reader can be
        ''' reset (<c>resetSupported()</c>).
        ''' </summary>
        ''' <param name="schema"> Schema for data </param>
        ''' <param name="rr">     Data to analyze </param>
        ''' <param name="maxHistogramBuckets"> Number of buckets of the histograms of the numerical and string columns </param>
        ''' <returns> DataAnalysis for data </returns>
        ''' <exception cref="InvalidOperationException">if the reader has no data (Java threw a NullPointerException)</exception>
        Public Shared Function analyze(ByVal schema As SchemaT, ByVal rr As RecordReaderT, ByVal maxHistogramBuckets As Integer) As DataAnalysisT
            Dim addFn As New AnalysisAddFunctionT(schema)
            Dim counters As IList(Of AnalysisCounterT) = Nothing
            Do While rr.hasNext()
                counters = addFn.apply(counters, rr.next())
            Loop
            If counters Is Nothing Then
                Throw New InvalidOperationException("Cannot analyze the data: the record reader has no records")
            End If

            Dim minsMaxes(counters.Count - 1)() As Double
            For i As Integer = 0 To minsMaxes.Length - 1
                minsMaxes(i) = New Double(1) {}
            Next i

            Dim columnTypes As IList(Of ColumnTypeT) = schema.getColumnTypes()
            Dim list As IList(Of ColumnAnalysisT) = DataVecAnalysisUtilsT.convertCounters(counters, minsMaxes, columnTypes)

            'Do another pass collecting histogram values:
            Dim histogramCounters As IList(Of HistogramCounterT) = Nothing
            Dim add As New HistogramAddFunctionT(maxHistogramBuckets, schema, minsMaxes)
            If rr.resetSupported() Then
                rr.reset()
                Do While rr.hasNext()
                    histogramCounters = add.apply(histogramCounters, rr.next())
                Loop

                DataVecAnalysisUtilsT.mergeCounters(list, histogramCounters)
            End If

            Return New DataAnalysisT(schema, list)
        End Function

        ''' <summary>
        ''' Analyze the data quality of sequence data - provides a report on missing values, values that don't comply with
        ''' schema, etc. Every time step of every sequence is checked against the schema.
        ''' (The Java version started from an empty list of states instead of none, so it failed on the first time step.)
        ''' </summary>
        ''' <param name="schema"> Schema for data </param>
        ''' <param name="data">   Data to analyze </param>
        ''' <returns> DataQualityAnalysis object (with no columns if there is no data) </returns>
        Public Shared Function analyzeQualitySequence(ByVal schema As SchemaT, ByVal data As SequenceRecordReaderT) As DataQualityAnalysisT
            Dim nColumns As Integer = schema.numColumns()
            Dim states As IList(Of QualityAnalysisState) = Nothing
            Dim addFn As New QualityAnalysisAddFunction(schema)
            Do While data.hasNext()
                Dim seq As IList(Of IList(Of WritableT)) = data.sequenceRecord()
                For Each [step] As IList(Of WritableT) In seq
                    states = addFn.apply(states, [step])
                Next [step]
            Loop

            Return toQualityAnalysis(schema, nColumns, states)
        End Function

        ''' <summary>
        ''' Analyze the data quality of data - provides a report on missing values, values that don't comply with schema, etc
        ''' </summary>
        ''' <param name="schema"> Schema for data </param>
        ''' <param name="data">   Data to analyze </param>
        ''' <returns> DataQualityAnalysis object (with no columns if there is no data; Java threw a NullPointerException) </returns>
        Public Shared Function analyzeQuality(ByVal schema As SchemaT, ByVal data As RecordReaderT) As DataQualityAnalysisT
            Dim nColumns As Integer = schema.numColumns()
            Dim states As IList(Of QualityAnalysisState) = Nothing
            Dim addFn As New QualityAnalysisAddFunction(schema)
            Do While data.hasNext()
                states = addFn.apply(states, data.next())
            Loop

            Return toQualityAnalysis(schema, nColumns, states)
        End Function

        Private Shared Function toQualityAnalysis(ByVal schema As SchemaT, ByVal nColumns As Integer, ByVal states As IList(Of QualityAnalysisState)) As DataQualityAnalysisT
            Dim list As IList(Of ColumnQualityT) = New List(Of ColumnQualityT)(nColumns)
            If states IsNot Nothing Then
                For Each qualityState As QualityAnalysisState In states
                    list.Add(qualityState.ColumnQuality)
                Next qualityState
            End If
            Return New DataQualityAnalysisT(schema, list)
        End Function

        ''' <summary>
        ''' Get a list of unique values from the specified columns.
        ''' For sequence data, use <see cref="getUniqueSequence(String, SchemaT, SequenceRecordReaderT)"/>
        ''' </summary>
        ''' <param name="columnName">    Name of the column to get unique values from </param>
        ''' <param name="schema">        Data schema </param>
        ''' <param name="data">          Data to get unique values from </param>
        ''' <returns>              List of unique values </returns>
        Public Shared Function getUnique(ByVal columnName As String, ByVal schema As SchemaT, ByVal data As RecordReaderT) As ISet(Of WritableT)
            Dim colIdx As Integer = schema.getIndexOfColumn(columnName)
            Dim unique As ISet(Of WritableT) = New HashSet(Of WritableT)()
            Do While data.hasNext()
                Dim [next] As IList(Of WritableT) = data.next()
                unique.Add([next](colIdx))
            Loop
            Return unique
        End Function

        ''' <summary>
        ''' Get a list of unique values from the specified columns.
        ''' For sequence data, use <see cref="getUniqueSequence(IList(Of String), SchemaT, SequenceRecordReaderT)"/>
        ''' </summary>
        ''' <param name="columnNames">   Names of the column to get unique values from </param>
        ''' <param name="schema">        Data schema </param>
        ''' <param name="data">          Data to get unique values from </param>
        ''' <returns>              List of unique values, for each of the specified columns </returns>
        Public Shared Function getUnique(ByVal columnNames As IList(Of String), ByVal schema As SchemaT, ByVal data As RecordReaderT) As IDictionary(Of String, ISet(Of WritableT))
            Dim m As IDictionary(Of String, ISet(Of WritableT)) = New Dictionary(Of String, ISet(Of WritableT))()
            For Each s As String In columnNames
                m(s) = New HashSet(Of WritableT)()
            Next s
            Do While data.hasNext()
                Dim [next] As IList(Of WritableT) = data.next()
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
        ''' <param name="sequenceData">    Sequence data to get unique values from </param>
        ''' <returns> the unique values </returns>
        Public Shared Function getUniqueSequence(ByVal columnName As String, ByVal schema As SchemaT, ByVal sequenceData As SequenceRecordReaderT) As ISet(Of WritableT)
            Dim colIdx As Integer = schema.getIndexOfColumn(columnName)
            Dim unique As ISet(Of WritableT) = New HashSet(Of WritableT)()
            Do While sequenceData.hasNext()
                Dim [next] As IList(Of IList(Of WritableT)) = sequenceData.sequenceRecord()
                For Each [step] As IList(Of WritableT) In [next]
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
        ''' <param name="sequenceData">    Sequence data to get unique values from </param>
        ''' <returns> the unique values, for each of the specified columns </returns>
        Public Shared Function getUniqueSequence(ByVal columnNames As IList(Of String), ByVal schema As SchemaT, ByVal sequenceData As SequenceRecordReaderT) As IDictionary(Of String, ISet(Of WritableT))
            Dim m As IDictionary(Of String, ISet(Of WritableT)) = New Dictionary(Of String, ISet(Of WritableT))()
            For Each s As String In columnNames
                m(s) = New HashSet(Of WritableT)()
            Next s
            Do While sequenceData.hasNext()
                Dim [next] As IList(Of IList(Of WritableT)) = sequenceData.sequenceRecord()
                For Each [step] As IList(Of WritableT) In [next]
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
