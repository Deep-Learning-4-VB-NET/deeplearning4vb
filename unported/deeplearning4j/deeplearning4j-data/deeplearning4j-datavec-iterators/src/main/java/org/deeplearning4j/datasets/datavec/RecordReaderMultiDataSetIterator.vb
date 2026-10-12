Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports val = lombok.val
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports Record = org.datavec.api.records.Record
Imports SequenceRecord = org.datavec.api.records.SequenceRecord
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataComposableMap = org.datavec.api.records.metadata.RecordMetaDataComposableMap
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports RecordConverter = org.datavec.api.util.ndarray.RecordConverter
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports NDArrayRecordBatch = org.datavec.api.writable.batch.NDArrayRecordBatch
Imports ZeroLengthSequenceException = org.deeplearning4j.datasets.datavec.exception.ZeroLengthSequenceException
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.datasets.datavec


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter public class RecordReaderMultiDataSetIterator implements org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator, java.io.Serializable
	<Serializable>
	Public Class RecordReaderMultiDataSetIterator
		Implements MultiDataSetIterator

		''' <summary>
		''' When dealing with time series data of different lengths, how should we align the input/labels time series?
		''' For equal length: use EQUAL_LENGTH
		''' For sequence classification: use ALIGN_END
		''' </summary>
		Public Enum AlignmentMode
			EQUAL_LENGTH
			ALIGN_START
			ALIGN_END
		End Enum

		Private batchSize As Integer
		Private alignmentMode As AlignmentMode
		Private recordReaders As IDictionary(Of String, RecordReader) = New Dictionary(Of String, RecordReader)()
		Private sequenceRecordReaders As IDictionary(Of String, SequenceRecordReader) = New Dictionary(Of String, SequenceRecordReader)()

		Private inputs As IList(Of SubsetDetails) = New List(Of SubsetDetails)()
		Private outputs As IList(Of SubsetDetails) = New List(Of SubsetDetails)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private boolean collectMetaData = false;
		Private collectMetaData As Boolean = False

		Private timeSeriesRandomOffset As Boolean = False
		Private timeSeriesRandomOffsetRng As Random

'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As MultiDataSetPreProcessor

'JAVA TO VB CONVERTER NOTE: The field resetSupported was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private resetSupported_Conflict As Boolean = True

		Private Sub New(ByVal builder As Builder)
			Me.batchSize = builder.batchSize
			Me.alignmentMode = builder.alignmentMode
			Me.recordReaders = builder.recordReaders
			Me.sequenceRecordReaders = builder.sequenceRecordReaders
			CType(Me.inputs, List(Of SubsetDetails)).AddRange(builder.inputs)
			CType(Me.outputs, List(Of SubsetDetails)).AddRange(builder.outputs)
			Me.timeSeriesRandomOffset = builder.timeSeriesRandomOffset_Conflict
			If Me.timeSeriesRandomOffset Then
				timeSeriesRandomOffsetRng = New Random(CInt(builder.timeSeriesRandomOffsetSeed))
			End If


			If recordReaders IsNot Nothing Then
				For Each rr As RecordReader In recordReaders.Values
					resetSupported_Conflict = resetSupported_Conflict And rr.resetSupported()
				Next rr
			End If
			If sequenceRecordReaders IsNot Nothing Then
				For Each srr As SequenceRecordReader In sequenceRecordReaders.Values
					resetSupported_Conflict = resetSupported_Conflict And srr.resetSupported()
				Next srr
			End If
		End Sub

		Public Overrides Function [next]() As MultiDataSet
			Return [next](batchSize)
		End Function

		Public Overrides Sub remove()
			Throw New System.NotSupportedException("Remove not supported")
		End Sub

		Public Overridable Function [next](ByVal num As Integer) As MultiDataSet Implements MultiDataSetIterator.next
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not hasNext() Then
				Throw New NoSuchElementException("No next elements")
			End If

			'First: load the next values from the RR / SeqRRs
			Dim nextRRVals As IDictionary(Of String, IList(Of IList(Of Writable))) = New Dictionary(Of String, IList(Of IList(Of Writable)))()
			Dim nextRRValsBatched As IDictionary(Of String, IList(Of INDArray)) = Nothing
			Dim nextSeqRRVals As IDictionary(Of String, IList(Of IList(Of IList(Of Writable)))) = New Dictionary(Of String, IList(Of IList(Of IList(Of Writable))))()
			Dim nextMetas As IList(Of RecordMetaDataComposableMap) = (If(collectMetaData, New List(Of RecordMetaDataComposableMap)(), Nothing))


			For Each entry As KeyValuePair(Of String, RecordReader) In recordReaders.SetOfKeyValuePairs()
				Dim rr As RecordReader = entry.Value
				If Not collectMetaData AndAlso rr.batchesSupported() Then
					'Batch case, for efficiency: ImageRecordReader etc
					Dim batchWritables As IList(Of IList(Of Writable)) = rr.next(num)

					Dim batch As IList(Of INDArray)
					If TypeOf batchWritables Is NDArrayRecordBatch Then
						'ImageRecordReader etc case
						batch = CType(batchWritables, NDArrayRecordBatch).getArrays()
					Else
						batchWritables = filterRequiredColumns(entry.Key, batchWritables)
						batch = New List(Of INDArray)()
						Dim temp As IList(Of Writable) = New List(Of Writable)()
						Dim sz As Integer = batchWritables(0).Count
						For i As Integer = 0 To sz - 1
							temp.Clear()
							For j As Integer = 0 To batchWritables.Count - 1
								temp.Add(batchWritables(j)(i))
							Next j
							batch.Add(RecordConverter.toMinibatchArray(temp))
						Next i
					End If

					If nextRRValsBatched Is Nothing Then
						nextRRValsBatched = New Dictionary(Of String, IList(Of INDArray))()
					End If
					nextRRValsBatched(entry.Key) = batch
				Else
					'Standard case
					Dim writables As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(Math.Min(num, 100000)) 'Min op: in case user puts batch size >> amount of data
					Dim i As Integer = 0
					Do While i < num AndAlso rr.hasNext()
						Dim record As IList(Of Writable)
						If collectMetaData Then
							Dim r As Record = rr.nextRecord()
							record = r.getRecord()
							If nextMetas.Count <= i Then
								nextMetas.Add(New RecordMetaDataComposableMap(New Dictionary(Of String, RecordMetaData)()))
							End If
							Dim map As RecordMetaDataComposableMap = nextMetas(i)
							map.getMeta().put(entry.Key, r.MetaData)
						Else
							record = rr.next()
						End If
						writables.Add(record)
						i += 1
					Loop

					nextRRVals(entry.Key) = writables
				End If
			Next entry

			For Each entry As KeyValuePair(Of String, SequenceRecordReader) In sequenceRecordReaders.SetOfKeyValuePairs()
				Dim rr As SequenceRecordReader = entry.Value
				Dim writables As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))(num)
				Dim i As Integer = 0
				Do While i < num AndAlso rr.hasNext()
					Dim sequence As IList(Of IList(Of Writable))
					If collectMetaData Then
						Dim r As SequenceRecord = rr.nextSequence()
						sequence = r.getSequenceRecord()
						If nextMetas.Count <= i Then
							nextMetas.Add(New RecordMetaDataComposableMap(New Dictionary(Of String, RecordMetaData)()))
						End If
						Dim map As RecordMetaDataComposableMap = nextMetas(i)
						map.getMeta().put(entry.Key, r.MetaData)
					Else
						sequence = rr.sequenceRecord()
					End If
					writables.Add(sequence)
					i += 1
				Loop

				nextSeqRRVals(entry.Key) = writables
			Next entry

			Return nextMultiDataSet(nextRRVals, nextRRValsBatched, nextSeqRRVals, nextMetas)
		End Function

		'Filter out the required columns before conversion. This is to avoid trying to convert String etc columns
		Private Function filterRequiredColumns(ByVal readerName As String, ByVal list As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable))

			'Options: (a) entire reader
			'(b) one or more subsets

			Dim entireReader As Boolean = False
			Dim subsetList As IList(Of SubsetDetails) = Nothing
			Dim max As Integer = -1
			Dim min As Integer = Integer.MaxValue
			For Each sdList As IList(Of SubsetDetails) In java.util.Arrays.asList(inputs, outputs)
				For Each sd As SubsetDetails In sdList
					If readerName.Equals(sd.readerName) Then
						If sd.entireReader Then
							entireReader = True
							Exit For
						Else
							If subsetList Is Nothing Then
								subsetList = New List(Of SubsetDetails)()
							End If
							subsetList.Add(sd)
							max = Math.Max(max, sd.subsetEndInclusive)
							min = Math.Min(min, sd.subsetStart)
						End If
					End If
				Next sd
			Next sdList

			If entireReader Then
				'No filtering required
				Return list
			ElseIf subsetList Is Nothing Then
				Throw New System.InvalidOperationException("Found no usages of reader: " & readerName)
			Else
				'we need some - but not all - columns
				Dim req(max) As Boolean
				For Each sd As SubsetDetails In subsetList
					For i As Integer = sd.subsetStart To sd.subsetEndInclusive
						req(i) = True
					Next i
				Next sd

				Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
				Dim zero As New IntWritable(0)
				For Each l As IList(Of Writable) In list
					Dim lNew As IList(Of Writable) = New List(Of Writable)(l.Count)
					For i As Integer = 0 To l.Count - 1
						If i >= req.Length OrElse Not req(i) Then
							lNew.Add(zero)
						Else
							lNew.Add(l(i))
						End If
					Next i
					[out].Add(lNew)
				Next l
				Return [out]
			End If
		End Function

		Public Overridable Function nextMultiDataSet(ByVal nextRRVals As IDictionary(Of String, IList(Of IList(Of Writable))), ByVal nextRRValsBatched As IDictionary(Of String, IList(Of INDArray)), ByVal nextSeqRRVals As IDictionary(Of String, IList(Of IList(Of IList(Of Writable)))), ByVal nextMetas As IList(Of RecordMetaDataComposableMap)) As MultiDataSet
			Dim minExamples As Integer = Integer.MaxValue
			For Each exampleData As IList(Of IList(Of Writable)) In nextRRVals.Values
				minExamples = Math.Min(minExamples, exampleData.Count)
			Next exampleData
			If nextRRValsBatched IsNot Nothing Then
				For Each exampleData As IList(Of INDArray) In nextRRValsBatched.Values
					'Assume all NDArrayWritables here
					For Each w As INDArray In exampleData
						Dim n As val = w.size(0)

						If Math.Min(minExamples, n) < Integer.MaxValue Then
							minExamples = CInt(Math.Min(minExamples, n))
						End If
					Next w
				Next exampleData
			End If
			For Each exampleData As IList(Of IList(Of IList(Of Writable))) In nextSeqRRVals.Values
				minExamples = Math.Min(minExamples, exampleData.Count)
			Next exampleData


			If minExamples = Integer.MaxValue Then
				Throw New Exception("Error occurred during data set generation: no readers?") 'Should never happen
			End If

			'In order to align data at the end (for each example individually), we need to know the length of the
			' longest time series for each example
			Dim longestSequence() As Integer = Nothing
			If timeSeriesRandomOffset OrElse alignmentMode = AlignmentMode.ALIGN_END Then
				longestSequence = New Integer(minExamples - 1){}
				For Each entry As KeyValuePair(Of String, IList(Of IList(Of IList(Of Writable)))) In nextSeqRRVals.SetOfKeyValuePairs()
					Dim list As IList(Of IList(Of IList(Of Writable))) = entry.Value
					Dim i As Integer = 0
					Do While i < list.Count AndAlso i < minExamples
						longestSequence(i) = Math.Max(longestSequence(i), list(i).Count)
						i += 1
					Loop
				Next entry
			End If

			'Second: create the input/feature arrays
			'To do this, we need to know longest time series length, so we can do padding
			Dim longestTS As Integer = -1
			If alignmentMode <> AlignmentMode.EQUAL_LENGTH Then
				For Each entry As KeyValuePair(Of String, IList(Of IList(Of IList(Of Writable)))) In nextSeqRRVals.SetOfKeyValuePairs()
					Dim list As IList(Of IList(Of IList(Of Writable))) = entry.Value
					For Each c As IList(Of IList(Of Writable)) In list
						longestTS = Math.Max(longestTS, c.Count)
					Next c
				Next entry
			End If
			Dim rngSeed As Long = (If(timeSeriesRandomOffset, timeSeriesRandomOffsetRng.nextLong(), -1))
			Dim features As Pair(Of INDArray(), INDArray()) = convertFeaturesOrLabels(New INDArray(inputs.Count - 1){}, New INDArray(inputs.Count - 1){}, inputs, minExamples, nextRRVals, nextRRValsBatched, nextSeqRRVals, longestTS, longestSequence, rngSeed)


			'Third: create the outputs/labels
			Dim labels As Pair(Of INDArray(), INDArray()) = convertFeaturesOrLabels(New INDArray(outputs.Count - 1){}, New INDArray(outputs.Count - 1){}, outputs, minExamples, nextRRVals, nextRRValsBatched, nextSeqRRVals, longestTS, longestSequence, rngSeed)



			Dim mds As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet(features.First, labels.First, features.Second, labels.Second)
			If collectMetaData Then
				mds.ExampleMetaData = nextMetas
			End If
			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess(mds)
			End If
			Return mds
		End Function

		Private Function convertFeaturesOrLabels(ByVal featuresOrLabels() As INDArray, ByVal masks() As INDArray, ByVal subsetDetails As IList(Of SubsetDetails), ByVal minExamples As Integer, ByVal nextRRVals As IDictionary(Of String, IList(Of IList(Of Writable))), ByVal nextRRValsBatched As IDictionary(Of String, IList(Of INDArray)), ByVal nextSeqRRVals As IDictionary(Of String, IList(Of IList(Of IList(Of Writable)))), ByVal longestTS As Integer, ByVal longestSequence() As Integer, ByVal rngSeed As Long) As Pair(Of INDArray(), INDArray())
			Dim hasMasks As Boolean = False
			Dim i As Integer = 0

			For Each d As SubsetDetails In subsetDetails
				If nextRRValsBatched IsNot Nothing AndAlso nextRRValsBatched.ContainsKey(d.readerName) Then
					'Standard reader, but batch ops
					featuresOrLabels(i) = convertWritablesBatched(nextRRValsBatched(d.readerName), d)
				ElseIf nextRRVals.ContainsKey(d.readerName) Then
					'Standard reader
					Dim list As IList(Of IList(Of Writable)) = nextRRVals(d.readerName)
					featuresOrLabels(i) = convertWritables(list, minExamples, d)
				Else
					'Sequence reader
					Dim list As IList(Of IList(Of IList(Of Writable))) = nextSeqRRVals(d.readerName)
					Dim p As Pair(Of INDArray, INDArray) = convertWritablesSequence(list, minExamples, longestTS, d, longestSequence, rngSeed)
					featuresOrLabels(i) = p.First
					masks(i) = p.Second
					If masks(i) IsNot Nothing Then
						hasMasks = True
					End If
				End If
				i += 1
			Next d

			Return New Pair(Of INDArray(), INDArray())(featuresOrLabels,If(hasMasks, masks, Nothing))
		End Function

		Private Function convertWritablesBatched(ByVal list As IList(Of INDArray), ByVal details As SubsetDetails) As INDArray
			Dim arr As INDArray
			If details.entireReader Then
				If list.Count = 1 Then
					arr = list(0)
				Else
					'Need to concat column vectors
					Dim asArray() As INDArray = CType(list, List(Of INDArray)).ToArray()
					arr = Nd4j.concat(1, asArray)
				End If
			ElseIf details.subsetStart = details.subsetEndInclusive OrElse details.oneHot Then
				arr = list(details.subsetStart)
			Else
				'Concat along dimension 1
				Dim count As Integer = details.subsetEndInclusive - details.subsetStart + 1
				Dim temp(count - 1) As INDArray
				Dim x As Integer = 0
				For i As Integer = details.subsetStart To details.subsetEndInclusive
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: temp[x++] = list.get(i);
					temp(x) = list(i)
						x += 1
				Next i
				arr = Nd4j.concat(1, temp)
			End If

			If Not details.oneHot OrElse arr.size(1) = details.oneHotNumClasses Then
				'Not one-hot: no conversion required
				'Also, ImageRecordReader already does the one-hot conversion internally
				Return arr
			End If

			'Do one-hot conversion
			If arr.size(1) <> 1 Then
				Throw New System.NotSupportedException("Cannot do conversion to one hot using batched reader: " & details.oneHotNumClasses & " output classes, but array.size(1) is " & arr.size(1) & " (must be equal to 1 or numClasses = " & details.oneHotNumClasses & ")")
			End If

			Dim n As val = arr.size(0)
			Dim [out] As INDArray = Nd4j.create(n, details.oneHotNumClasses)
			For i As Integer = 0 To n - 1
				Dim v As Integer = arr.getInt(i, 0)
				[out].putScalar(i, v, 1.0)
			Next i

			Return [out]
		End Function

		Private Function countLength(ByVal list As IList(Of Writable)) As Integer
			Return countLength(list, 0, list.Count - 1)
		End Function

		Private Function countLength(ByVal list As IList(Of Writable), ByVal from As Integer, ByVal [to] As Integer) As Integer
			Dim length As Integer = 0
			For i As Integer = from To [to]
				Dim w As Writable = list(i)
				If TypeOf w Is NDArrayWritable Then
					Dim a As INDArray = DirectCast(w, NDArrayWritable).get()
					If Not a.RowVectorOrScalar Then
						Throw New System.NotSupportedException("Multiple writables present but NDArrayWritable is " & "not a row vector. Can only concat row vectors with other writables. Shape: " & java.util.Arrays.toString(a.shape()))
					End If
					length += a.length()
				Else
					'Assume all others are single value
					length += 1
				End If
			Next i

			Return length
		End Function

		Private Function convertWritables(ByVal list As IList(Of IList(Of Writable)), ByVal minValues As Integer, ByVal details As SubsetDetails) As INDArray
			Try
				Return convertWritablesHelper(list, minValues, details)
			Catch e As System.FormatException
				Throw New Exception("Error parsing data (writables) from record readers - value is non-numeric", e)
			Catch e As System.InvalidOperationException
				Throw e
			Catch t As Exception
				Throw New Exception("Error parsing data (writables) from record readers", t)
			End Try
		End Function

		Private Function convertWritablesHelper(ByVal list As IList(Of IList(Of Writable)), ByVal minValues As Integer, ByVal details As SubsetDetails) As INDArray
			Dim arr As INDArray
			If details.entireReader Then
				If list(0).Count = 1 AndAlso TypeOf list(0)(0) Is NDArrayWritable Then
					'Special case: single NDArrayWritable...
					Dim temp As INDArray = CType(list(0)(0), NDArrayWritable).get()
					Dim shape As val = ArrayUtils.clone(temp.shape())
					shape(0) = minValues
					arr = Nd4j.create(shape)
				Else
					arr = Nd4j.create(minValues, countLength(list(0)))
				End If
			ElseIf details.oneHot Then
				arr = Nd4j.zeros(minValues, details.oneHotNumClasses)
			Else
				If details.subsetStart = details.subsetEndInclusive AndAlso TypeOf list(0)(details.subsetStart) Is NDArrayWritable Then
					'Special case: single NDArrayWritable (example: ImageRecordReader)
					Dim temp As INDArray = CType(list(0)(details.subsetStart), NDArrayWritable).get()
					Dim shape As val = ArrayUtils.clone(temp.shape())
					shape(0) = minValues
					arr = Nd4j.create(shape)
				Else
					'Need to check for multiple NDArrayWritables, or mixed NDArrayWritable + DoubleWritable etc
					Dim length As Integer = countLength(list(0), details.subsetStart, details.subsetEndInclusive)
					arr = Nd4j.create(minValues, length)
				End If
			End If

			For i As Integer = 0 To minValues - 1
				Dim c As IList(Of Writable) = list(i)
				If details.entireReader Then
					'Convert entire reader contents, without modification
					Dim converted As INDArray = RecordConverter.toArray(Nd4j.defaultFloatingPointType(), c)
					putExample(arr, converted, i)
				ElseIf details.oneHot Then
					'Convert a single column to a one-hot representation
					Dim w As Writable = c(details.subsetStart)
					'Index of class
					Dim classIdx As Integer = w.toInt()
					If classIdx >= details.oneHotNumClasses Then
						Throw New System.InvalidOperationException("Cannot convert sequence writables to one-hot: class index " & classIdx & " >= numClass (" & details.oneHotNumClasses & "). (Note that classes are zero-" & "indexed, thus only values 0 to nClasses-1 are valid)")
					End If
					arr.putScalar(i, w.toInt(), 1.0)
				Else
					'Convert a subset of the columns

					'Special case: subsetStart == subsetEndInclusive && NDArrayWritable. Example: ImageRecordReader
					If details.subsetStart = details.subsetEndInclusive AndAlso (TypeOf c(details.subsetStart) Is NDArrayWritable) Then
						putExample(arr, DirectCast(c(details.subsetStart), NDArrayWritable).get(), i)
					Else

						Dim iter As IEnumerator(Of Writable) = c.GetEnumerator()
						For j As Integer = 0 To details.subsetStart - 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							iter.next()
						Next j
						Dim k As Integer = 0
						For j As Integer = details.subsetStart To details.subsetEndInclusive
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							Dim w As Writable = iter.next()

							If TypeOf w Is NDArrayWritable Then
								Dim toPut As INDArray = DirectCast(w, NDArrayWritable).get()
								arr.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.interval(k, k + toPut.length())}, toPut)
								k += toPut.length()
							Else
								arr.putScalar(i, k, w.toDouble())
								k += 1
							End If
						Next j
					End If
				End If
			Next i

			Return arr
		End Function

		Private Sub putExample(ByVal arr As INDArray, ByVal singleExample As INDArray, ByVal exampleIdx As Integer)
			Preconditions.checkState(singleExample.size(0) = 1 AndAlso singleExample.rank() = arr.rank(), "Cannot put array: array should have leading dimension of 1 " & "and equal rank to output array. Attempting to put array of shape %s into output array of shape %s", singleExample.shape(), arr.shape())

			Dim arrShape() As Long = arr.shape()
			Dim singleShape() As Long = singleExample.shape()
			Dim i As Integer=1
			Do While i<arr.rank()
				Preconditions.checkState(arrShape(i) = singleShape(i), "Single example array and output arrays differ at position %s:" & "single example shape %s, output array shape %s", i, singleShape, arrShape)
				i += 1
			Loop
			Select Case arr.rank()
				Case 2
					arr.put(New INDArrayIndex() {NDArrayIndex.point(exampleIdx), NDArrayIndex.all()}, singleExample)
				Case 3
					arr.put(New INDArrayIndex() {NDArrayIndex.point(exampleIdx), NDArrayIndex.all(), NDArrayIndex.all()}, singleExample)
				Case 4
					arr.put(New INDArrayIndex() {NDArrayIndex.point(exampleIdx), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()}, singleExample)
				Case 5
					arr.put(New INDArrayIndex() {NDArrayIndex.point(exampleIdx), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()}, singleExample)
				Case Else
					Throw New Exception("Unexpected array rank: " & arr.rank() & " with shape " & java.util.Arrays.toString(arr.shape()) & " input arrays should be rank 2 to 5 inclusive")
			End Select
		End Sub

		''' <summary>
		''' Convert the writables to a sequence (3d) data set, and also return the mask array (if necessary)
		''' </summary>
		Private Function convertWritablesSequence(ByVal list As IList(Of IList(Of IList(Of Writable))), ByVal minValues As Integer, ByVal maxTSLength As Integer, ByVal details As SubsetDetails, ByVal longestSequence() As Integer, ByVal rngSeed As Long) As Pair(Of INDArray, INDArray)
			If maxTSLength = -1 Then
				maxTSLength = list(0).Count
			End If
			Dim arr As INDArray

			If list(0).Count = 0 Then
				Throw New ZeroLengthSequenceException("Zero length sequence encountered")
			End If

			Dim firstStep As IList(Of Writable) = list(0)(0)

			Dim size As Integer = 0
			If details.entireReader Then
				'Need to account for NDArrayWritables etc in list:
				For Each w As Writable In firstStep
					If TypeOf w Is NDArrayWritable Then
						size += DirectCast(w, NDArrayWritable).get().size(1)
					Else
						size += 1
					End If
				Next w
			ElseIf details.oneHot Then
				size = details.oneHotNumClasses
			Else
				'Need to account for NDArrayWritables etc in list:
				For i As Integer = details.subsetStart To details.subsetEndInclusive
					Dim w As Writable = firstStep(i)
					If TypeOf w Is NDArrayWritable Then
						size += DirectCast(w, NDArrayWritable).get().size(1)
					Else
						size += 1
					End If
				Next i
			End If
			arr = Nd4j.create(New Integer() {minValues, size, maxTSLength}, "f"c)

			Dim needMaskArray As Boolean = False
			For Each c As IList(Of IList(Of Writable)) In list
				If c.Count < maxTSLength Then
					needMaskArray = True
				End If
			Next c

			If needMaskArray AndAlso alignmentMode = AlignmentMode.EQUAL_LENGTH Then
				Throw New System.NotSupportedException("Alignment mode is set to EQUAL_LENGTH but variable length data was " & "encountered. Use AlignmentMode.ALIGN_START or AlignmentMode.ALIGN_END with variable length data")
			End If

			Dim maskArray As INDArray
			If needMaskArray Then
				maskArray = Nd4j.ones(minValues, maxTSLength)
			Else
				maskArray = Nothing
			End If

			'Don't use the global RNG as we need repeatability for each subset (i.e., features and labels must be aligned)
			Dim rng As Random = Nothing
			If timeSeriesRandomOffset Then
				rng = New Random(CInt(rngSeed))
			End If

			For i As Integer = 0 To minValues - 1
				Dim sequence As IList(Of IList(Of Writable)) = list(i)

				'Offset for alignment:
				Dim startOffset As Integer
				If alignmentMode = AlignmentMode.ALIGN_START OrElse alignmentMode = AlignmentMode.EQUAL_LENGTH Then
					startOffset = 0
				Else
					'Align end
					'Only practical differences here are: (a) offset, and (b) masking
					startOffset = longestSequence(i) - sequence.Count
				End If

				If timeSeriesRandomOffset Then
					Dim maxPossible As Integer = maxTSLength - sequence.Count + 1
					startOffset = rng.Next(maxPossible)
				End If

				Dim t As Integer = 0
				Dim k As Integer
				For Each timeStep As IList(Of Writable) In sequence
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: k = startOffset + t++;
					k = startOffset + t
						t += 1

					If details.entireReader Then
						'Convert entire reader contents, without modification
						Dim iter As IEnumerator(Of Writable) = timeStep.GetEnumerator()
						Dim j As Integer = 0
						Do While iter.MoveNext()
							Dim w As Writable = iter.Current

							If TypeOf w Is NDArrayWritable Then
								Dim row As INDArray = DirectCast(w, NDArrayWritable).get()

								arr.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.interval(j, j + row.length()), NDArrayIndex.point(k)}, row)
								j += row.length()
							Else
								arr.putScalar(i, j, k, w.toDouble())
								j += 1
							End If
						Loop
					ElseIf details.oneHot Then
						'Convert a single column to a one-hot representation
						Dim w As Writable = Nothing
						If TypeOf timeStep Is System.Collections.IList Then
							w = timeStep(details.subsetStart)
						Else
							Dim iter As IEnumerator(Of Writable) = timeStep.GetEnumerator()
							For x As Integer = 0 To details.subsetStart
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
								w = iter.next()
							Next x
						End If
						Dim classIdx As Integer = w.toInt()
						If classIdx >= details.oneHotNumClasses Then
							Throw New System.InvalidOperationException("Cannot convert sequence writables to one-hot: class index " & classIdx & " >= numClass (" & details.oneHotNumClasses & "). (Note that classes are zero-" & "indexed, thus only values 0 to nClasses-1 are valid)")
						End If
						arr.putScalar(i, classIdx, k, 1.0)
					Else
						'Convert a subset of the columns...
						Dim l As Integer = 0
						For j As Integer = details.subsetStart To details.subsetEndInclusive
							Dim w As Writable = timeStep(j)

							If TypeOf w Is NDArrayWritable Then
								Dim row As INDArray = DirectCast(w, NDArrayWritable).get()
								arr.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.interval(l, l + row.length()), NDArrayIndex.point(k)}, row)

								l += row.length()
							Else
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: arr.putScalar(i, l++, k, w.toDouble());
								arr.putScalar(i, l, k, w.toDouble())
									l += 1
							End If
						Next j
					End If
				Next timeStep

				'For any remaining time steps: set mask array to 0 (just padding)
				If needMaskArray Then
					'Masking array entries at start (for align end)
					If timeSeriesRandomOffset OrElse alignmentMode = AlignmentMode.ALIGN_END Then
						For t2 As Integer = 0 To startOffset - 1
							maskArray.putScalar(i, t2, 0.0)
						Next t2
					End If

					'Masking array entries at end (for align start)
					Dim lastStep As Integer = startOffset + sequence.Count
					If timeSeriesRandomOffset OrElse alignmentMode = AlignmentMode.ALIGN_START OrElse lastStep < maxTSLength Then
						For t2 As Integer = lastStep To maxTSLength - 1
							maskArray.putScalar(i, t2, 0.0)
						Next t2
					End If
				End If
			Next i

			Return New Pair(Of INDArray, INDArray)(arr, maskArray)
		End Function

		Public Overridable Property PreProcessor Implements MultiDataSetIterator.setPreProcessor As MultiDataSetPreProcessor
			Set(ByVal preProcessor As MultiDataSetPreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
			Get
				Return preProcessor_Conflict
			End Get
		End Property


		Public Overridable Function resetSupported() As Boolean Implements MultiDataSetIterator.resetSupported
			Return resetSupported_Conflict
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements MultiDataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Sub reset() Implements MultiDataSetIterator.reset
			If Not resetSupported_Conflict Then
				Throw New System.InvalidOperationException("Cannot reset iterator - reset not supported (resetSupported() == false):" & " one or more underlying (sequence) record readers do not support resetting")
			End If

			For Each rr As RecordReader In recordReaders.Values
				rr.reset()
			Next rr
			For Each rr As SequenceRecordReader In sequenceRecordReaders.Values
				rr.reset()
			Next rr
		End Sub

		Public Overrides Function hasNext() As Boolean
			For Each rr As RecordReader In recordReaders.Values
				If Not rr.hasNext() Then
					Return False
				End If
			Next rr
			For Each rr As SequenceRecordReader In sequenceRecordReaders.Values
				If Not rr.hasNext() Then
					Return False
				End If
			Next rr
			Return True
		End Function


		Public Class Builder

			Friend batchSize As Integer
			Friend alignmentMode As AlignmentMode = AlignmentMode.ALIGN_START
			Friend recordReaders As IDictionary(Of String, RecordReader) = New Dictionary(Of String, RecordReader)()
			Friend sequenceRecordReaders As IDictionary(Of String, SequenceRecordReader) = New Dictionary(Of String, SequenceRecordReader)()

			Friend inputs As IList(Of SubsetDetails) = New List(Of SubsetDetails)()
			Friend outputs As IList(Of SubsetDetails) = New List(Of SubsetDetails)()

'JAVA TO VB CONVERTER NOTE: The field timeSeriesRandomOffset was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend timeSeriesRandomOffset_Conflict As Boolean = False
			Friend timeSeriesRandomOffsetSeed As Long = DateTimeHelper.CurrentUnixTimeMillis()

			''' <param name="batchSize"> The batch size for the RecordReaderMultiDataSetIterator </param>
			Public Sub New(ByVal batchSize As Integer)
				Me.batchSize = batchSize
			End Sub

			''' <summary>
			''' Add a RecordReader for use in .addInput(...) or .addOutput(...)
			''' </summary>
			''' <param name="readerName">   Name of the reader (for later reference) </param>
			''' <param name="recordReader"> RecordReader </param>
			Public Overridable Function addReader(ByVal readerName As String, ByVal recordReader As RecordReader) As Builder
				recordReaders(readerName) = recordReader
				Return Me
			End Function

			''' <summary>
			''' Add a SequenceRecordReader for use in .addInput(...) or .addOutput(...)
			''' </summary>
			''' <param name="seqReaderName">   Name of the sequence reader (for later reference) </param>
			''' <param name="seqRecordReader"> SequenceRecordReader </param>
			Public Overridable Function addSequenceReader(ByVal seqReaderName As String, ByVal seqRecordReader As SequenceRecordReader) As Builder
				sequenceRecordReaders(seqReaderName) = seqRecordReader
				Return Me
			End Function

			''' <summary>
			''' Set the sequence alignment mode for all sequences
			''' </summary>
			Public Overridable Function sequenceAlignmentMode(ByVal alignmentMode As AlignmentMode) As Builder
				Me.alignmentMode = alignmentMode
				Return Me
			End Function

			''' <summary>
			''' Set as an input, the entire contents (all columns) of the RecordReader or SequenceRecordReader
			''' </summary>
			Public Overridable Function addInput(ByVal readerName As String) As Builder
				inputs.Add(New SubsetDetails(readerName, True, False, -1, -1, -1))
				Return Me
			End Function

			''' <summary>
			''' Set as an input, a subset of the specified RecordReader or SequenceRecordReader
			''' </summary>
			''' <param name="readerName">  Name of the reader </param>
			''' <param name="columnFirst"> First column index, inclusive </param>
			''' <param name="columnLast">  Last column index, inclusive </param>
			Public Overridable Function addInput(ByVal readerName As String, ByVal columnFirst As Integer, ByVal columnLast As Integer) As Builder
				inputs.Add(New SubsetDetails(readerName, False, False, -1, columnFirst, columnLast))
				Return Me
			End Function

			''' <summary>
			''' Add as an input a single column from the specified RecordReader / SequenceRecordReader
			''' The assumption is that the specified column contains integer values in range 0..numClasses-1;
			''' this integer will be converted to a one-hot representation
			''' </summary>
			''' <param name="readerName"> Name of the RecordReader or SequenceRecordReader </param>
			''' <param name="column">     Column that contains the index </param>
			''' <param name="numClasses"> Total number of classes </param>
			Public Overridable Function addInputOneHot(ByVal readerName As String, ByVal column As Integer, ByVal numClasses As Integer) As Builder
				inputs.Add(New SubsetDetails(readerName, False, True, numClasses, column, column))
				Return Me
			End Function

			''' <summary>
			''' Set as an output, the entire contents (all columns) of the RecordReader or SequenceRecordReader
			''' </summary>
			Public Overridable Function addOutput(ByVal readerName As String) As Builder
				outputs.Add(New SubsetDetails(readerName, True, False, -1, -1, -1))
				Return Me
			End Function

			''' <summary>
			''' Add an output, with a subset of the columns from the named RecordReader or SequenceRecordReader
			''' </summary>
			''' <param name="readerName">  Name of the reader </param>
			''' <param name="columnFirst"> First column index </param>
			''' <param name="columnLast">  Last column index (inclusive) </param>
			Public Overridable Function addOutput(ByVal readerName As String, ByVal columnFirst As Integer, ByVal columnLast As Integer) As Builder
				outputs.Add(New SubsetDetails(readerName, False, False, -1, columnFirst, columnLast))
				Return Me
			End Function

			''' <summary>
			''' An an output, where the output is taken from a single column from the specified RecordReader / SequenceRecordReader
			''' The assumption is that the specified column contains integer values in range 0..numClasses-1;
			''' this integer will be converted to a one-hot representation (usually for classification)
			''' </summary>
			''' <param name="readerName"> Name of the RecordReader / SequenceRecordReader </param>
			''' <param name="column">     index of the column </param>
			''' <param name="numClasses"> Number of classes </param>
			Public Overridable Function addOutputOneHot(ByVal readerName As String, ByVal column As Integer, ByVal numClasses As Integer) As Builder
				outputs.Add(New SubsetDetails(readerName, False, True, numClasses, column, column))
				Return Me
			End Function

			''' <summary>
			''' For use with timeseries trained with tbptt
			''' In a given minbatch, shorter time series are padded and appropriately masked to be the same length as the longest time series.
			''' Cases with a skewed distrbution of lengths can result in the last few updates from the time series coming from mostly masked time steps.
			''' timeSeriesRandomOffset randomly offsettsthe time series + masking appropriately to address this </summary>
			''' <param name="timeSeriesRandomOffset">, "true" to randomly offset time series within a minibatch </param>
			''' <param name="rngSeed"> seed for reproducibility </param>
'JAVA TO VB CONVERTER NOTE: The parameter timeSeriesRandomOffset was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function timeSeriesRandomOffset(ByVal timeSeriesRandomOffset_Conflict As Boolean, ByVal rngSeed As Long) As Builder
				Me.timeSeriesRandomOffset_Conflict = timeSeriesRandomOffset_Conflict
				Me.timeSeriesRandomOffsetSeed = rngSeed
				Return Me
			End Function

			''' <summary>
			''' Create the RecordReaderMultiDataSetIterator
			''' </summary>
			Public Overridable Function build() As RecordReaderMultiDataSetIterator
				'Validate input:
				If recordReaders.Count = 0 AndAlso sequenceRecordReaders.Count = 0 Then
					Throw New System.InvalidOperationException("Cannot construct RecordReaderMultiDataSetIterator with no readers")
				End If

				If batchSize <= 0 Then
					Throw New System.InvalidOperationException("Cannot construct RecordReaderMultiDataSetIterator with batch size <= 0")
				End If

				If inputs.Count = 0 AndAlso outputs.Count = 0 Then
					Throw New System.InvalidOperationException("Cannot construct RecordReaderMultiDataSetIterator with no inputs/outputs")
				End If

				For Each ssd As SubsetDetails In inputs
					If Not recordReaders.ContainsKey(ssd.readerName) AndAlso Not sequenceRecordReaders.ContainsKey(ssd.readerName) Then
						Throw New System.InvalidOperationException("Invalid input name: """ & ssd.readerName & """ - no reader found with this name")
					End If
				Next ssd

				For Each ssd As SubsetDetails In outputs
					If Not recordReaders.ContainsKey(ssd.readerName) AndAlso Not sequenceRecordReaders.ContainsKey(ssd.readerName) Then
						Throw New System.InvalidOperationException("Invalid output name: """ & ssd.readerName & """ - no reader found with this name")
					End If
				Next ssd

				Return New RecordReaderMultiDataSetIterator(Me)
			End Function
		End Class

		''' <summary>
		''' Load a single example to a DataSet, using the provided RecordMetaData.
		''' Note that it is more efficient to load multiple instances at once, using <seealso cref="loadFromMetaData(List)"/>
		''' </summary>
		''' <param name="recordMetaData"> RecordMetaData to load from. Should have been produced by the given record reader </param>
		''' <returns> DataSet with the specified example </returns>
		''' <exception cref="IOException"> If an error occurs during loading of the data </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.dataset.api.MultiDataSet loadFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws java.io.IOException
		Public Overridable Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As MultiDataSet
			Return loadFromMetaData(Collections.singletonList(recordMetaData))
		End Function

		''' <summary>
		''' Load a multiple sequence examples to a DataSet, using the provided RecordMetaData instances.
		''' </summary>
		''' <param name="list"> List of RecordMetaData instances to load from. Should have been produced by the record reader provided
		'''             to the SequenceRecordReaderDataSetIterator constructor </param>
		''' <returns> DataSet with the specified examples </returns>
		''' <exception cref="IOException"> If an error occurs during loading of the data </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.dataset.api.MultiDataSet loadFromMetaData(List<org.datavec.api.records.metadata.RecordMetaData> list) throws java.io.IOException
		Public Overridable Function loadFromMetaData(ByVal list As IList(Of RecordMetaData)) As MultiDataSet
			'First: load the next values from the RR / SeqRRs
			Dim nextRRVals As IDictionary(Of String, IList(Of IList(Of Writable))) = New Dictionary(Of String, IList(Of IList(Of Writable)))()
			Dim nextSeqRRVals As IDictionary(Of String, IList(Of IList(Of IList(Of Writable)))) = New Dictionary(Of String, IList(Of IList(Of IList(Of Writable))))()
			Dim nextMetas As IList(Of RecordMetaDataComposableMap) = (If(collectMetaData, New List(Of RecordMetaDataComposableMap)(), Nothing))


			For Each entry As KeyValuePair(Of String, RecordReader) In recordReaders.SetOfKeyValuePairs()
				Dim rr As RecordReader = entry.Value

				Dim thisRRMeta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
				For Each m As RecordMetaData In list
					Dim m2 As RecordMetaDataComposableMap = DirectCast(m, RecordMetaDataComposableMap)
					thisRRMeta.Add(m2.getMeta().get(entry.Key))
				Next m

				Dim fromMeta As IList(Of Record) = rr.loadFromMetaData(thisRRMeta)
				Dim writables As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(list.Count)
				For Each r As Record In fromMeta
					writables.Add(r.getRecord())
				Next r

				nextRRVals(entry.Key) = writables
			Next entry

			For Each entry As KeyValuePair(Of String, SequenceRecordReader) In sequenceRecordReaders.SetOfKeyValuePairs()
				Dim rr As SequenceRecordReader = entry.Value

				Dim thisRRMeta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
				For Each m As RecordMetaData In list
					Dim m2 As RecordMetaDataComposableMap = DirectCast(m, RecordMetaDataComposableMap)
					thisRRMeta.Add(m2.getMeta().get(entry.Key))
				Next m

				Dim fromMeta As IList(Of SequenceRecord) = rr.loadSequenceFromMetaData(thisRRMeta)
				Dim writables As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))(list.Count)
				For Each r As SequenceRecord In fromMeta
					writables.Add(r.getSequenceRecord())
				Next r

				nextSeqRRVals(entry.Key) = writables
			Next entry

			Return nextMultiDataSet(nextRRVals, Nothing, nextSeqRRVals, nextMetas)

		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor private static class SubsetDetails implements java.io.Serializable
		<Serializable>
		Private Class SubsetDetails
			Friend ReadOnly readerName As String
			Friend ReadOnly entireReader As Boolean
			Friend ReadOnly oneHot As Boolean
			Friend ReadOnly oneHotNumClasses As Integer
			Friend ReadOnly subsetStart As Integer
			Friend ReadOnly subsetEndInclusive As Integer
		End Class
	End Class

End Namespace