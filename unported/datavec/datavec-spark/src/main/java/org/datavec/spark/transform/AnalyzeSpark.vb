Imports System.Collections.Generic
Imports JavaDoubleRDD = org.apache.spark.api.java.JavaDoubleRDD
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports org.datavec.api.transform.analysis
Imports DataAnalysis = org.datavec.api.transform.analysis.DataAnalysis
Imports DataVecAnalysisUtils = org.datavec.api.transform.analysis.DataVecAnalysisUtils
Imports SequenceDataAnalysis = org.datavec.api.transform.analysis.SequenceDataAnalysis
Imports org.datavec.api.transform.analysis.columns
Imports HistogramCounter = org.datavec.api.transform.analysis.histogram.HistogramCounter
Imports QualityAnalysisAddFunction = org.datavec.api.transform.analysis.quality.QualityAnalysisAddFunction
Imports QualityAnalysisCombineFunction = org.datavec.api.transform.analysis.quality.QualityAnalysisCombineFunction
Imports org.datavec.api.transform.analysis.quality
Imports SequenceLengthAnalysis = org.datavec.api.transform.analysis.sequence.SequenceLengthAnalysis
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports DataQualityAnalysis = org.datavec.api.transform.quality.DataQualityAnalysis
Imports ColumnQuality = org.datavec.api.transform.quality.columns.ColumnQuality
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
Imports Comparators = org.datavec.api.writable.comparator.Comparators
Imports SelectColumnFunction = org.datavec.spark.transform.analysis.SelectColumnFunction
Imports SequenceFlatMapFunction = org.datavec.spark.transform.analysis.SequenceFlatMapFunction
Imports SequenceLengthFunction = org.datavec.spark.transform.analysis.SequenceLengthFunction
Imports AnalysisAddFunction = org.datavec.spark.transform.analysis.aggregate.AnalysisAddFunction
Imports AnalysisCombineFunction = org.datavec.spark.transform.analysis.aggregate.AnalysisCombineFunction
Imports HistogramAddFunction = org.datavec.spark.transform.analysis.histogram.HistogramAddFunction
Imports HistogramCombineFunction = org.datavec.spark.transform.analysis.histogram.HistogramCombineFunction
Imports IntToDoubleFunction = org.datavec.spark.transform.analysis.seqlength.IntToDoubleFunction
Imports SequenceLengthAnalysisAddFunction = org.datavec.spark.transform.analysis.seqlength.SequenceLengthAnalysisAddFunction
Imports SequenceLengthAnalysisCounter = org.datavec.spark.transform.analysis.seqlength.SequenceLengthAnalysisCounter
Imports SequenceLengthAnalysisMergeFunction = org.datavec.spark.transform.analysis.seqlength.SequenceLengthAnalysisMergeFunction
Imports UniqueAddFunction = org.datavec.spark.transform.analysis.unique.UniqueAddFunction
Imports UniqueMergeFunction = org.datavec.spark.transform.analysis.unique.UniqueMergeFunction
Imports FilterWritablesBySchemaFunction = org.datavec.spark.transform.filter.FilterWritablesBySchemaFunction
Imports ColumnToKeyPairTransform = org.datavec.spark.transform.misc.ColumnToKeyPairTransform
Imports SumLongsFunction2 = org.datavec.spark.transform.misc.SumLongsFunction2
Imports org.datavec.spark.transform.misc.comparator
Imports org.datavec.spark.transform.utils.adapter
Imports Tuple2 = scala.Tuple2

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

Namespace org.datavec.spark.transform


	Public Class AnalyzeSpark

		Public Const DEFAULT_HISTOGRAM_BUCKETS As Integer = 30

		Public Shared Function analyzeSequence(ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of IList(Of Writable)))) As SequenceDataAnalysis
			Return analyzeSequence(schema, data, DEFAULT_HISTOGRAM_BUCKETS)
		End Function

		''' 
		''' <param name="schema"> </param>
		''' <param name="data"> </param>
		''' <param name="maxHistogramBuckets">
		''' @return </param>
		Public Shared Function analyzeSequence(ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of IList(Of Writable))), ByVal maxHistogramBuckets As Integer) As SequenceDataAnalysis
			data.cache()
			Dim fmSeq As JavaRDD(Of IList(Of Writable)) = data.flatMap(New SequenceFlatMapFunction())
			Dim da As DataAnalysis = analyze(schema, fmSeq)
			'Analyze the length of the sequences:
			Dim seqLengths As JavaRDD(Of Integer) = data.map(New SequenceLengthFunction())
			seqLengths.cache()
			Dim counter As New SequenceLengthAnalysisCounter()
			counter = seqLengths.aggregate(counter, New SequenceLengthAnalysisAddFunction(), New SequenceLengthAnalysisMergeFunction())

			Dim max As Integer = counter.getMaxLengthSeen()
			Dim min As Integer = counter.getMinLengthSeen()
			Dim nBuckets As Integer = counter.getMaxLengthSeen() - counter.getMinLengthSeen()

			Dim hist As Tuple2(Of Double(), Long())
			If max = min Then
				'Edge case that spark doesn't like
				hist = New Tuple2(Of Double(), Long())(New Double() {min}, New Long() {counter.getCountTotal()})
			ElseIf nBuckets < maxHistogramBuckets Then
				Dim drdd As JavaDoubleRDD = seqLengths.mapToDouble(New IntToDoubleFunction())
				hist = drdd.histogram(nBuckets)
			Else
				Dim drdd As JavaDoubleRDD = seqLengths.mapToDouble(New IntToDoubleFunction())
				hist = drdd.histogram(maxHistogramBuckets)
			End If
			seqLengths.unpersist()


			Dim lengthAnalysis As SequenceLengthAnalysis = SequenceLengthAnalysis.builder().totalNumSequences(counter.getCountTotal()).minSeqLength(counter.getMinLengthSeen()).maxSeqLength(counter.getMaxLengthSeen()).countZeroLength(counter.getCountZeroLength()).countOneLength(counter.getCountOneLength()).meanLength(counter.getMean()).histogramBuckets(hist._1()).histogramBucketCounts(hist._2()).build()

			Return New SequenceDataAnalysis(schema, da.getColumnAnalysis(), lengthAnalysis)
		End Function


		''' <summary>
		''' Analyse the specified data - returns a DataAnalysis object with summary information about each column
		''' </summary>
		''' <param name="schema"> Schema for data </param>
		''' <param name="data">   Data to analyze </param>
		''' <returns>       DataAnalysis for data </returns>
		Public Shared Function analyze(ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of Writable))) As DataAnalysis
			Return analyze(schema, data, DEFAULT_HISTOGRAM_BUCKETS)
		End Function

		Public Shared Function analyze(ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of Writable)), ByVal maxHistogramBuckets As Integer) As DataAnalysis
			data.cache()
	'        
	'         * TODO: Some care should be given to add histogramBuckets and histogramBucketCounts to this in the future
	'         

			Dim columnTypes As IList(Of ColumnType) = schema.getColumnTypes()
			Dim counters As IList(Of AnalysisCounter) = data.aggregate(Nothing, New AnalysisAddFunction(schema), New AnalysisCombineFunction())

'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim minsMaxes[][] As Double = new Double[counters.Count][2]
			Dim minsMaxes()() As Double = RectangularArrays.RectangularDoubleArray(counters.Count, 2)
			Dim list As IList(Of ColumnAnalysis) = DataVecAnalysisUtils.convertCounters(counters, minsMaxes, columnTypes)

			Dim histogramCounters As IList(Of HistogramCounter) = data.aggregate(Nothing, New HistogramAddFunction(maxHistogramBuckets, schema, minsMaxes), New HistogramCombineFunction())

			DataVecAnalysisUtils.mergeCounters(list, histogramCounters)
			Return New DataAnalysis(schema, list)
		End Function

		''' <summary>
		''' Randomly sample values from a single column
		''' </summary>
		''' <param name="count">         Number of values to sample </param>
		''' <param name="columnName">    Name of the column to sample from </param>
		''' <param name="schema">        Schema </param>
		''' <param name="data">          Data to sample from </param>
		''' <returns>              A list of random samples </returns>
		Public Shared Function sampleFromColumn(ByVal count As Integer, ByVal columnName As String, ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of Writable))) As IList(Of Writable)
			Dim colIdx As Integer = schema.getIndexOfColumn(columnName)
			Dim ithColumn As JavaRDD(Of Writable) = data.map(New SelectColumnFunction(colIdx))

			Return ithColumn.takeSample(False, count)
		End Function

		''' <summary>
		''' Randomly sample values from a single column, in all sequences.
		''' Values may be taken from any sequence (i.e., sequence order is not preserved)
		''' </summary>
		''' <param name="count">         Number of values to sample </param>
		''' <param name="columnName">    Name of the column to sample from </param>
		''' <param name="schema">        Schema </param>
		''' <param name="sequenceData">  Data to sample from </param>
		''' <returns>              A list of random samples </returns>
		Public Shared Function sampleFromColumnSequence(ByVal count As Integer, ByVal columnName As String, ByVal schema As Schema, ByVal sequenceData As JavaRDD(Of IList(Of IList(Of Writable)))) As IList(Of Writable)
			Dim flattenedSequence As JavaRDD(Of IList(Of Writable)) = sequenceData.flatMap(New SequenceFlatMapFunction())
			Return sampleFromColumn(count, columnName, schema, flattenedSequence)
		End Function

		''' <summary>
		''' Get a list of unique values from the specified columns.
		''' For sequence data, use <seealso cref="getUniqueSequence(List, Schema, JavaRDD)"/>
		''' </summary>
		''' <param name="columnName">    Name of the column to get unique values from </param>
		''' <param name="schema">        Data schema </param>
		''' <param name="data">          Data to get unique values from </param>
		''' <returns>              List of unique values </returns>
		Public Shared Function getUnique(ByVal columnName As String, ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of Writable))) As IList(Of Writable)
			Dim colIdx As Integer = schema.getIndexOfColumn(columnName)
			Dim ithColumn As JavaRDD(Of Writable) = data.map(New SelectColumnFunction(colIdx))
			Return ithColumn.distinct().collect()
		End Function

		''' <summary>
		''' Get a list of unique values from the specified columns.
		''' For sequence data, use <seealso cref="getUniqueSequence(String, Schema, JavaRDD)"/>
		''' </summary>
		''' <param name="columnNames">   Names of the column to get unique values from </param>
		''' <param name="schema">        Data schema </param>
		''' <param name="data">          Data to get unique values from </param>
		''' <returns>              List of unique values, for each of the specified columns </returns>
		Public Shared Function getUnique(ByVal columnNames As IList(Of String), ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of Writable))) As IDictionary(Of String, IList(Of Writable))
			Dim m As IDictionary(Of String, ISet(Of Writable)) = data.aggregate(Nothing, New UniqueAddFunction(columnNames, schema), New UniqueMergeFunction())
			Dim [out] As IDictionary(Of String, IList(Of Writable)) = New Dictionary(Of String, IList(Of Writable))()
			For Each s As String In m.Keys
				[out](s) = New List(Of Writable)(m(s))
			Next s
			Return [out]
		End Function

		''' <summary>
		''' Get a list of unique values from the specified column of a sequence
		''' </summary>
		''' <param name="columnName">      Name of the column to get unique values from </param>
		''' <param name="schema">          Data schema </param>
		''' <param name="sequenceData">    Sequence data to get unique values from
		''' @return </param>
		Public Shared Function getUniqueSequence(ByVal columnName As String, ByVal schema As Schema, ByVal sequenceData As JavaRDD(Of IList(Of IList(Of Writable)))) As IList(Of Writable)
			Dim flattenedSequence As JavaRDD(Of IList(Of Writable)) = sequenceData.flatMap(New SequenceFlatMapFunction())
			Return getUnique(columnName, schema, flattenedSequence)
		End Function

		''' <summary>
		''' Get a list of unique values from the specified columns of a sequence
		''' </summary>
		''' <param name="columnNames">     Name of the columns to get unique values from </param>
		''' <param name="schema">          Data schema </param>
		''' <param name="sequenceData">    Sequence data to get unique values from
		''' @return </param>
		Public Shared Function getUniqueSequence(ByVal columnNames As IList(Of String), ByVal schema As Schema, ByVal sequenceData As JavaRDD(Of IList(Of IList(Of Writable)))) As IDictionary(Of String, IList(Of Writable))
			Dim flattenedSequence As JavaRDD(Of IList(Of Writable)) = sequenceData.flatMap(New SequenceFlatMapFunction())
			Return getUnique(columnNames, schema, flattenedSequence)
		End Function

		''' <summary>
		''' Randomly sample a set of examples
		''' </summary>
		''' <param name="count">    Number of samples to generate </param>
		''' <param name="data">     Data to sample from </param>
		''' <returns>         Samples </returns>
		Public Shared Function sample(ByVal count As Integer, ByVal data As JavaRDD(Of IList(Of Writable))) As IList(Of IList(Of Writable))
			Return data.takeSample(False, count)
		End Function

		''' <summary>
		''' Randomly sample a number of sequences from the data </summary>
		''' <param name="count">    Number of sequences to sample </param>
		''' <param name="data">     Data to sample from </param>
		''' <returns>         Sequence samples </returns>
		Public Shared Function sampleSequence(ByVal count As Integer, ByVal data As JavaRDD(Of IList(Of IList(Of Writable)))) As IList(Of IList(Of IList(Of Writable)))
			Return data.takeSample(False, count)
		End Function


		''' <summary>
		''' Analyze the data quality of sequence data - provides a report on missing values, values that don't comply with schema, etc </summary>
		''' <param name="schema"> Schema for data </param>
		''' <param name="data">   Data to analyze </param>
		''' <returns> DataQualityAnalysis object </returns>
		Public Shared Function analyzeQualitySequence(ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of IList(Of Writable)))) As DataQualityAnalysis
			Dim fmSeq As JavaRDD(Of IList(Of Writable)) = data.flatMap(New SequenceFlatMapFunction())
			Return analyzeQuality(schema, fmSeq)
		End Function

		''' <summary>
		''' Analyze the data quality of data - provides a report on missing values, values that don't comply with schema, etc </summary>
		''' <param name="schema"> Schema for data </param>
		''' <param name="data">   Data to analyze </param>
		''' <returns> DataQualityAnalysis object </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static org.datavec.api.transform.quality.DataQualityAnalysis analyzeQuality(final org.datavec.api.transform.schema.Schema schema, final org.apache.spark.api.java.JavaRDD<List<org.datavec.api.writable.Writable>> data)
		Public Shared Function analyzeQuality(ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of Writable))) As DataQualityAnalysis
			Dim nColumns As Integer = schema.numColumns()
			Dim states As IList(Of QualityAnalysisState) = data.aggregate(Nothing, New BiFunctionAdapter(Of QualityAnalysisState)(New QualityAnalysisAddFunction(schema)), New BiFunctionAdapter(Of )(New QualityAnalysisCombineFunction()))

			Dim list As IList(Of ColumnQuality) = New List(Of ColumnQuality)(nColumns)

			For Each qualityState As QualityAnalysisState In states
				list.Add(qualityState.getColumnQuality())
			Next qualityState
			Return New DataQualityAnalysis(schema, list)
		End Function

		''' <summary>
		''' Randomly sample a set of invalid values from a specified column.
		''' Values are considered invalid according to the Schema / ColumnMetaData
		''' </summary>
		''' <param name="numToSample">    Maximum number of invalid values to sample </param>
		''' <param name="columnName">     Same of the column from which to sample invalid values </param>
		''' <param name="schema">         Data schema </param>
		''' <param name="data">           Data </param>
		''' <returns>               List of invalid examples </returns>
		Public Shared Function sampleInvalidFromColumn(ByVal numToSample As Integer, ByVal columnName As String, ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of Writable))) As IList(Of Writable)
			Return sampleInvalidFromColumn(numToSample, columnName, schema, data, False)
		End Function

		''' <summary>
		''' Randomly sample a set of invalid values from a specified column.
		''' Values are considered invalid according to the Schema / ColumnMetaData
		''' </summary>
		''' <param name="numToSample">    Maximum number of invalid values to sample </param>
		''' <param name="columnName">     Same of the column from which to sample invalid values </param>
		''' <param name="schema">         Data schema </param>
		''' <param name="data">           Data </param>
		''' <param name="ignoreMissing">  If true: ignore missing values (NullWritable or empty/null string) when sampling. If false: include missing values in sampling </param>
		''' <returns>               List of invalid examples </returns>
		Public Shared Function sampleInvalidFromColumn(ByVal numToSample As Integer, ByVal columnName As String, ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of Writable)), ByVal ignoreMissing As Boolean) As IList(Of Writable)
			'First: filter out all valid entries, to leave only invalid entries
			Dim colIdx As Integer = schema.getIndexOfColumn(columnName)
			Dim ithColumn As JavaRDD(Of Writable) = data.map(New SelectColumnFunction(colIdx))

			Dim meta As ColumnMetaData = schema.getMetaData(columnName)

			Dim invalid As JavaRDD(Of Writable) = ithColumn.filter(New FilterWritablesBySchemaFunction(meta, False, ignoreMissing))

			Return invalid.takeSample(False, numToSample)
		End Function

		''' <summary>
		''' Randomly sample a set of invalid values from a specified column, for a sequence data set.
		''' Values are considered invalid according to the Schema / ColumnMetaData
		''' </summary>
		''' <param name="numToSample">    Maximum number of invalid values to sample </param>
		''' <param name="columnName">     Same of the column from which to sample invalid values </param>
		''' <param name="schema">         Data schema </param>
		''' <param name="data">           Data </param>
		''' <returns>               List of invalid examples </returns>
		Public Shared Function sampleInvalidFromColumnSequence(ByVal numToSample As Integer, ByVal columnName As String, ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of IList(Of Writable)))) As IList(Of Writable)
			Dim flattened As JavaRDD(Of IList(Of Writable)) = data.flatMap(New SequenceFlatMapFunction())
			Return sampleInvalidFromColumn(numToSample, columnName, schema, flattened)
		End Function

		''' <summary>
		''' Sample the N most frequently occurring values in the specified column
		''' </summary>
		''' <param name="nMostFrequent">    Top N values to sample </param>
		''' <param name="columnName">       Name of the column to sample from </param>
		''' <param name="schema">           Schema of the data </param>
		''' <param name="data">             RDD containing the data </param>
		''' <returns>                 List of the most frequently occurring Writable objects in that column, along with their counts </returns>
		Public Shared Function sampleMostFrequentFromColumn(ByVal nMostFrequent As Integer, ByVal columnName As String, ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of Writable))) As IDictionary(Of Writable, Long)
			Dim columnIdx As Integer = schema.getIndexOfColumn(columnName)

			Dim keyedByWritable As JavaPairRDD(Of Writable, Long) = data.mapToPair(New ColumnToKeyPairTransform(columnIdx))
			Dim reducedByWritable As JavaPairRDD(Of Writable, Long) = keyedByWritable.reduceByKey(New SumLongsFunction2())

			Dim list As IList(Of Tuple2(Of Writable, Long)) = reducedByWritable.takeOrdered(nMostFrequent, New Tuple2Comparator(Of Writable)(False))

			Dim sorted As IList(Of Tuple2(Of Writable, Long)) = New List(Of Tuple2(Of Writable, Long))(list)
			sorted.Sort(New Tuple2Comparator(Of Writable)(False))

			Dim map As IDictionary(Of Writable, Long) = New LinkedHashMap(Of Writable, Long)()
			For Each t2 As Tuple2(Of Writable, Long) In sorted
				map(t2._1()) = t2._2()
			Next t2

			Return map
		End Function

		''' <summary>
		''' Get the minimum value for the specified column
		''' </summary>
		''' <param name="allData">    All data </param>
		''' <param name="columnName"> Name of the column to get the minimum value for </param>
		''' <param name="schema">     Schema of the data </param>
		''' <returns>           Minimum value for the column </returns>
		Public Shared Function min(ByVal allData As JavaRDD(Of IList(Of Writable)), ByVal columnName As String, ByVal schema As Schema) As Writable
			Dim columnIdx As Integer = schema.getIndexOfColumn(columnName)
			Dim col As JavaRDD(Of Writable) = allData.map(New SelectColumnFunction(columnIdx))
			Return col.min(Comparators.forType(schema.getType(columnName).getWritableType()))
		End Function

		''' <summary>
		''' Get the maximum value for the specified column
		''' </summary>
		''' <param name="allData">    All data </param>
		''' <param name="columnName"> Name of the column to get the minimum value for </param>
		''' <param name="schema">     Schema of the data </param>
		''' <returns>           Maximum value for the column </returns>
		Public Shared Function max(ByVal allData As JavaRDD(Of IList(Of Writable)), ByVal columnName As String, ByVal schema As Schema) As Writable
			Dim columnIdx As Integer = schema.getIndexOfColumn(columnName)
			Dim col As JavaRDD(Of Writable) = allData.map(New SelectColumnFunction(columnIdx))
			Return col.max(Comparators.forType(schema.getType(columnName).getWritableType()))
		End Function

	End Class

End Namespace