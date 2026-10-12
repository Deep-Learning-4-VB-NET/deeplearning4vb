Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports ListOrderedMap = org.apache.commons.collections.map.ListOrderedMap
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports Column = org.apache.spark.sql.Column
Imports Dataset = org.apache.spark.sql.Dataset
Imports Row = org.apache.spark.sql.Row
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable

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



	Public Class Normalization


		''' <summary>
		''' Normalize by zero mean unit variance
		''' </summary>
		''' <param name="frame"> the data to normalize </param>
		''' <returns> a zero mean unit variance centered
		''' rdd </returns>
		Public Shared Function zeromeanUnitVariance(ByVal frame As Dataset(Of Row)) As Dataset(Of Row)
			Return zeromeanUnitVariance(frame, Enumerable.Empty(Of String)())
		End Function

		''' <summary>
		''' Normalize by zero mean unit variance
		''' </summary>
		''' <param name="schema"> the schema to use
		'''               to create the data frame </param>
		''' <param name="data">   the data to normalize </param>
		''' <returns> a zero mean unit variance centered
		''' rdd </returns>
		Public Shared Function zeromeanUnitVariance(ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of Writable))) As JavaRDD(Of IList(Of Writable))
			Return zeromeanUnitVariance(schema, data, Enumerable.Empty(Of String)())
		End Function

		''' <summary>
		''' Scale based on min,max
		''' </summary>
		''' <param name="dataFrame"> the dataframe to scale </param>
		''' <param name="min">       the minimum value </param>
		''' <param name="max">       the maximum value </param>
		''' <returns> the normalized dataframe per column </returns>
		Public Shared Function normalize(ByVal dataFrame As Dataset(Of Row), ByVal min As Double, ByVal max As Double) As Dataset(Of Row)
			Return normalize(dataFrame, min, max, Enumerable.Empty(Of String)())
		End Function

		''' <summary>
		''' Scale based on min,max
		''' </summary>
		''' <param name="schema"> the schema of the data to scale </param>
		''' <param name="data">   the data to sclae </param>
		''' <param name="min">    the minimum value </param>
		''' <param name="max">    the maximum value </param>
		''' <returns> the normalized ata </returns>
		Public Shared Function normalize(ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of Writable)), ByVal min As Double, ByVal max As Double) As JavaRDD(Of IList(Of Writable))
			Dim frame As Dataset(Of Row) = DataFrames.toDataFrame(schema, data)
			Return DataFrames.toRecords(normalize(frame, min, max, Enumerable.Empty(Of String)())).Second
		End Function


		''' <summary>
		''' Scale based on min,max
		''' </summary>
		''' <param name="dataFrame"> the dataframe to scale </param>
		''' <returns> the normalized dataframe per column </returns>
		Public Shared Function normalize(ByVal dataFrame As Dataset(Of Row)) As Dataset(Of Row)
			Return normalize(dataFrame, 0, 1, Enumerable.Empty(Of String)())
		End Function

		''' <summary>
		''' Scale all data  0 to 1
		''' </summary>
		''' <param name="schema"> the schema of the data to scale </param>
		''' <param name="data">   the data to scale </param>
		''' <returns> the normalized ata </returns>
		Public Shared Function normalize(ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of Writable))) As JavaRDD(Of IList(Of Writable))
			Return normalize(schema, data, 0, 1, Enumerable.Empty(Of String)())
		End Function


		''' <summary>
		''' Normalize by zero mean unit variance
		''' </summary>
		''' <param name="frame"> the data to normalize </param>
		''' <returns> a zero mean unit variance centered
		''' rdd </returns>
		Public Shared Function zeromeanUnitVariance(ByVal frame As Dataset(Of Row), ByVal skipColumns As IList(Of String)) As Dataset(Of Row)
			Dim columnsList As IList(Of String) = DataFrames.toList(frame.columns())
			columnsList.RemoveAll(skipColumns)
			Dim columnNames() As String = DataFrames.toArray(columnsList)
			'first row is std second row is mean, each column in a row is for a particular column
			Dim stdDevMean As IList(Of Row) = stdDevMeanColumns(frame, columnNames)
			For i As Integer = 0 To columnNames.Length - 1
				Dim columnName As String = columnNames(i)
				Dim std As Double = CType(stdDevMean(0).get(i), Number).doubleValue()
				Dim mean As Double = CType(stdDevMean(1).get(i), Number).doubleValue()
				If std = 0.0 Then
					std = 1 'All same value -> (x-x)/1 = 0
				End If

				frame = frame.withColumn(columnName, frame.col(columnName).minus(mean).divide(std))
			Next i



			Return frame
		End Function

		''' <summary>
		''' Normalize by zero mean unit variance
		''' </summary>
		''' <param name="schema"> the schema to use
		'''               to create the data frame </param>
		''' <param name="data">   the data to normalize </param>
		''' <returns> a zero mean unit variance centered
		''' rdd </returns>
		Public Shared Function zeromeanUnitVariance(ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of Writable)), ByVal skipColumns As IList(Of String)) As JavaRDD(Of IList(Of Writable))
			Dim frame As Dataset(Of Row) = DataFrames.toDataFrame(schema, data)
			Return DataFrames.toRecords(zeromeanUnitVariance(frame, skipColumns)).Second
		End Function

		''' <summary>
		''' Normalize the sequence by zero mean unit variance
		''' </summary>
		''' <param name="schema">   Schema of the data to normalize </param>
		''' <param name="sequence"> Sequence data </param>
		''' <returns> Normalized sequence </returns>
		Public Shared Function zeroMeanUnitVarianceSequence(ByVal schema As Schema, ByVal sequence As JavaRDD(Of IList(Of IList(Of Writable)))) As JavaRDD(Of IList(Of IList(Of Writable)))
			Return zeroMeanUnitVarianceSequence(schema, sequence, Nothing)
		End Function

		''' <summary>
		''' Normalize the sequence by zero mean unit variance
		''' </summary>
		''' <param name="schema">         Schema of the data to normalize </param>
		''' <param name="sequence">       Sequence data </param>
		''' <param name="excludeColumns"> List of  columns to exclude from the normalization </param>
		''' <returns> Normalized sequence </returns>
		Public Shared Function zeroMeanUnitVarianceSequence(ByVal schema As Schema, ByVal sequence As JavaRDD(Of IList(Of IList(Of Writable))), ByVal excludeColumns As IList(Of String)) As JavaRDD(Of IList(Of IList(Of Writable)))
			Dim frame As Dataset(Of Row) = DataFrames.toDataFrameSequence(schema, sequence)
			If excludeColumns Is Nothing Then
				excludeColumns = New List(Of String) From {DataFrames.SEQUENCE_UUID_COLUMN, DataFrames.SEQUENCE_INDEX_COLUMN}
			Else
				excludeColumns = New List(Of String)(excludeColumns)
				excludeColumns.Add(DataFrames.SEQUENCE_UUID_COLUMN)
				excludeColumns.Add(DataFrames.SEQUENCE_INDEX_COLUMN)
			End If
			frame = zeromeanUnitVariance(frame, excludeColumns)
			Return DataFrames.toRecordsSequence(frame).Second
		End Function

		''' <summary>
		''' Returns the min and max of the given columns </summary>
		''' <param name="data"> the data to get the max for </param>
		''' <param name="columns"> the columns to get the
		''' @return </param>
		Public Shared Function minMaxColumns(ByVal data As Dataset(Of Row), ByVal columns As IList(Of String)) As IList(Of Row)
			Dim arr(columns.Count - 1) As String
			For i As Integer = 0 To arr.Length - 1
				arr(i) = columns(i)
			Next i
			Return minMaxColumns(data, arr)
		End Function

		''' <summary>
		''' Returns the min and max of the given columns.
		''' The list returned is a list of size 2 where each row </summary>
		''' <param name="data"> the data to get the max for </param>
		''' <param name="columns"> the columns to get the
		''' @return </param>
		Public Shared Function minMaxColumns(ByVal data As Dataset(Of Row), ParamArray ByVal columns() As String) As IList(Of Row)
			Return aggregate(data, columns, New String() {"min", "max"})
		End Function


		''' <summary>
		''' Returns the standard deviation and mean of the given columns </summary>
		''' <param name="data"> the data to get the max for </param>
		''' <param name="columns"> the columns to get the
		''' @return </param>
		Public Shared Function stdDevMeanColumns(ByVal data As Dataset(Of Row), ByVal columns As IList(Of String)) As IList(Of Row)
			Dim arr(columns.Count - 1) As String
			For i As Integer = 0 To arr.Length - 1
				arr(i) = columns(i)
			Next i
			Return stdDevMeanColumns(data, arr)
		End Function

		''' <summary>
		''' Returns the standard deviation
		''' and mean of the given columns
		''' The list returned is a list of size 2 where each row
		''' represents the standard deviation of each column and the mean of each column </summary>
		''' <param name="data"> the data to get the standard deviation and mean for </param>
		''' <param name="columns"> the columns to get the
		''' @return </param>
		Public Shared Function stdDevMeanColumns(ByVal data As Dataset(Of Row), ParamArray ByVal columns() As String) As IList(Of Row)
			Return aggregate(data, columns, New String() {"stddev", "mean"})
		End Function

		''' <summary>
		''' Aggregate based on an arbitrary list
		''' of aggregation and grouping functions </summary>
		''' <param name="data"> the dataframe to aggregate </param>
		''' <param name="columns"> the columns to aggregate </param>
		''' <param name="functions"> the functions to use </param>
		''' <returns> the list of rows with the aggregated statistics.
		''' Each row will be a function with the desired columnar output
		''' in the order in which the columns were specified. </returns>
		Public Shared Function aggregate(ByVal data As Dataset(Of Row), ByVal columns() As String, ByVal functions() As String) As IList(Of Row)
			Dim rest(columns.Length - 2) As String
			Array.Copy(columns, 1, rest, 0, rest.Length)
			Dim rows As IList(Of Row) = New List(Of Row)()
			For Each op As String In functions
				Dim expressions As IDictionary(Of String, String) = New ListOrderedMap()
				For Each s As String In columns
					expressions(s) = op
				Next s

				'compute the aggregation based on the operation
				Dim aggregated As Dataset(Of Row) = data.agg(expressions)
				Dim columns2() As String = aggregated.columns()
				'strip out the op name and parentheses from the columns
				Dim opReplace As IDictionary(Of String, String) = New SortedDictionary(Of String, String)()
				For Each s As String In columns2
					If s.Contains("min(") OrElse s.Contains("max(") Then
						opReplace(s) = s.Replace(op, "").replaceAll("[()]", "")
					ElseIf s.Contains("avg") Then
						opReplace(s) = s.Replace("avg", "").replaceAll("[()]", "")
					Else
						opReplace(s) = s.Replace(op, "").replaceAll("[()]", "")
					End If
				Next s


				'get rid of the operation name in the column
				Dim rearranged As Dataset(Of Row) = Nothing
				For Each entries As KeyValuePair(Of String, String) In opReplace.SetOfKeyValuePairs()
					'first column
					If rearranged Is Nothing Then
						rearranged = aggregated.withColumnRenamed(entries.Key, entries.Value)
					'rearranged is just a copy of aggregated at this point
					Else
						rearranged = rearranged.withColumnRenamed(entries.Key, entries.Value)
					End If
				Next entries

				rearranged = rearranged.select(DataFrames.toColumns(columns))
				'op
				CType(rows, List(Of Row)).AddRange(rearranged.collectAsList())
			Next op


			Return rows
		End Function


		''' <summary>
		''' Scale based on min,max
		''' </summary>
		''' <param name="dataFrame"> the dataframe to scale </param>
		''' <param name="min">       the minimum value </param>
		''' <param name="max">       the maximum value </param>
		''' <returns> the normalized dataframe per column </returns>
		Public Shared Function normalize(ByVal dataFrame As Dataset(Of Row), ByVal min As Double, ByVal max As Double, ByVal skipColumns As IList(Of String)) As Dataset(Of Row)
			Dim columnsList As IList(Of String) = DataFrames.toList(dataFrame.columns())
			columnsList.RemoveAll(skipColumns)
			Dim columnNames() As String = DataFrames.toArray(columnsList)
			'first row is min second row is max, each column in a row is for a particular column
			Dim minMax As IList(Of Row) = minMaxColumns(dataFrame, columnNames)
			For i As Integer = 0 To columnNames.Length - 1
				Dim columnName As String = columnNames(i)
				Dim dMin As Double = CType(minMax(0).get(i), Number).doubleValue()
				Dim dMax As Double = CType(minMax(1).get(i), Number).doubleValue()
				Dim maxSubMin As Double = (dMax - dMin)
				If maxSubMin = 0 Then
					maxSubMin = 1
				End If

				Dim newCol As Column = dataFrame.col(columnName).minus(dMin).divide(maxSubMin).multiply(max - min).plus(min)
				dataFrame = dataFrame.withColumn(columnName, newCol)
			Next i


			Return dataFrame
		End Function

		''' <summary>
		''' Scale based on min,max
		''' </summary>
		''' <param name="schema"> the schema of the data to scale </param>
		''' <param name="data">   the data to scale </param>
		''' <param name="min">    the minimum value </param>
		''' <param name="max">    the maximum value </param>
		''' <returns> the normalized ata </returns>
		Public Shared Function normalize(ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of Writable)), ByVal min As Double, ByVal max As Double, ByVal skipColumns As IList(Of String)) As JavaRDD(Of IList(Of Writable))
			Dim frame As Dataset(Of Row) = DataFrames.toDataFrame(schema, data)
			Return DataFrames.toRecords(normalize(frame, min, max, skipColumns)).Second
		End Function

		''' 
		''' <param name="schema"> </param>
		''' <param name="data">
		''' @return </param>
		Public Shared Function normalizeSequence(ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of IList(Of Writable)))) As JavaRDD(Of IList(Of IList(Of Writable)))
			Return normalizeSequence(schema, data, 0, 1)
		End Function

		''' <summary>
		''' Normalize each column of a sequence, based on min/max
		''' </summary>
		''' <param name="schema"> Schema of the data </param>
		''' <param name="data">   Data to normalize </param>
		''' <param name="min">    New minimum value </param>
		''' <param name="max">    New maximum value </param>
		''' <returns> Normalized data </returns>
		Public Shared Function normalizeSequence(ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of IList(Of Writable))), ByVal min As Double, ByVal max As Double) As JavaRDD(Of IList(Of IList(Of Writable)))
			Return normalizeSequence(schema, data, min, max, Nothing)
		End Function

		''' <summary>
		''' Normalize each column of a sequence, based on min/max
		''' </summary>
		''' <param name="schema">         Schema of the data </param>
		''' <param name="data">           Data to normalize </param>
		''' <param name="min">            New minimum value </param>
		''' <param name="max">            New maximum value </param>
		''' <param name="excludeColumns"> List of columns to exclude </param>
		''' <returns> Normalized data </returns>
		Public Shared Function normalizeSequence(ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of IList(Of Writable))), ByVal min As Double, ByVal max As Double, ByVal excludeColumns As IList(Of String)) As JavaRDD(Of IList(Of IList(Of Writable)))
			If excludeColumns Is Nothing Then
				excludeColumns = New List(Of String) From {DataFrames.SEQUENCE_UUID_COLUMN, DataFrames.SEQUENCE_INDEX_COLUMN}
			Else
				excludeColumns = New List(Of String)(excludeColumns)
				excludeColumns.Add(DataFrames.SEQUENCE_UUID_COLUMN)
				excludeColumns.Add(DataFrames.SEQUENCE_INDEX_COLUMN)
			End If
			Dim frame As Dataset(Of Row) = DataFrames.toDataFrameSequence(schema, data)
			Return DataFrames.toRecordsSequence(normalize(frame, min, max, excludeColumns)).Second
		End Function


		''' <summary>
		''' Scale based on min,max
		''' </summary>
		''' <param name="dataFrame"> the dataframe to scale </param>
		''' <returns> the normalized dataframe per column </returns>
		Public Shared Function normalize(ByVal dataFrame As Dataset(Of Row), ByVal skipColumns As IList(Of String)) As Dataset(Of Row)
			Return normalize(dataFrame, 0, 1, skipColumns)
		End Function

		''' <summary>
		''' Scale all data  0 to 1
		''' </summary>
		''' <param name="schema"> the schema of the data to scale </param>
		''' <param name="data">   the data to scale </param>
		''' <returns> the normalized ata </returns>
		Public Shared Function normalize(ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of Writable)), ByVal skipColumns As IList(Of String)) As JavaRDD(Of IList(Of Writable))
			Return normalize(schema, data, 0, 1, skipColumns)
		End Function
	End Class

End Namespace