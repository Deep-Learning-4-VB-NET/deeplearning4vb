Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports SequenceRecord = org.datavec.api.records.SequenceRecord
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataComposable = org.datavec.api.records.metadata.RecordMetaDataComposable
Imports RecordMetaDataComposableMap = org.datavec.api.records.metadata.RecordMetaDataComposableMap
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports ZeroLengthSequenceException = org.deeplearning4j.datasets.datavec.exception.ZeroLengthSequenceException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex

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


	<Serializable>
	Public Class SequenceRecordReaderDataSetIterator
		Implements DataSetIterator

		''' <summary>
		'''Alignment mode for dealing with input/labels of differing lengths (for example, one-to-many and many-to-one type situations).
		''' For example, might have 10 time steps total but only one label at end for sequence classification.<br>
		''' Currently supported modes:<br>
		''' <b>EQUAL_LENGTH</b>: Default. Assume that label and input time series are of equal length, and all examples are of
		''' the same length<br>
		''' <b>ALIGN_START</b>: Align the label/input time series at the first time step, and zero pad either the labels or
		''' the input at the end<br>
		''' <b>ALIGN_END</b>: Align the label/input at the last time step, zero padding either the input or the labels as required<br>
		''' 
		''' Note 1: When the time series for each example are of different lengths, the shorter time series will be padded to
		''' the length of the longest time series.<br>
		''' Note 2: When ALIGN_START or ALIGN_END are used, the DataSet masking functionality is used. Thus, the returned DataSets
		''' will have the input and mask arrays set. These mask arrays identify whether an input/label is actually present,
		''' or whether the value is merely masked.<br>
		''' </summary>
		Public Enum AlignmentMode
			EQUAL_LENGTH
			ALIGN_START
			ALIGN_END
		End Enum

		Private Const READER_KEY As String = "reader"
		Private Const READER_KEY_LABEL As String = "reader_labels"

		Private recordReader As SequenceRecordReader
		Private labelsReader As SequenceRecordReader
		Private miniBatchSize As Integer = 10
		Private ReadOnly regression As Boolean
		Private labelIndex As Integer = -1
		Private ReadOnly numPossibleLabels As Integer
		Private cursor As Integer = 0
'JAVA TO VB CONVERTER NOTE: The field inputColumns was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputColumns_Conflict As Integer = -1
'JAVA TO VB CONVERTER NOTE: The field totalOutcomes was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private totalOutcomes_Conflict As Integer = -1
		Private useStored As Boolean = False
		Private stored As DataSet = Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.dataset.api.DataSetPreProcessor preProcessor;
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As DataSetPreProcessor
		Private alignmentMode As AlignmentMode

		Private ReadOnly singleSequenceReaderMode As Boolean

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private boolean collectMetaData = false;
		Private collectMetaData As Boolean = False

		Private underlying As RecordReaderMultiDataSetIterator
		Private underlyingIsDisjoint As Boolean

		''' <summary>
		''' Constructor where features and labels come from different RecordReaders (for example, different files),
		''' and labels are for classification.
		''' </summary>
		''' <param name="featuresReader">       SequenceRecordReader for the features </param>
		''' <param name="labels">               Labels: assume single value per time step, where values are integers in the range 0 to numPossibleLables-1 </param>
		''' <param name="miniBatchSize">        Minibatch size for each call of next() </param>
		''' <param name="numPossibleLabels">    Number of classes for the labels </param>
		Public Sub New(ByVal featuresReader As SequenceRecordReader, ByVal labels As SequenceRecordReader, ByVal miniBatchSize As Integer, ByVal numPossibleLabels As Integer)
			Me.New(featuresReader, labels, miniBatchSize, numPossibleLabels, False)
		End Sub

		''' <summary>
		''' Constructor where features and labels come from different RecordReaders (for example, different files)
		''' </summary>
		Public Sub New(ByVal featuresReader As SequenceRecordReader, ByVal labels As SequenceRecordReader, ByVal miniBatchSize As Integer, ByVal numPossibleLabels As Integer, ByVal regression As Boolean)
			Me.New(featuresReader, labels, miniBatchSize, numPossibleLabels, regression, AlignmentMode.EQUAL_LENGTH)
		End Sub

		''' <summary>
		''' Constructor where features and labels come from different RecordReaders (for example, different files)
		''' </summary>
		Public Sub New(ByVal featuresReader As SequenceRecordReader, ByVal labels As SequenceRecordReader, ByVal miniBatchSize As Integer, ByVal numPossibleLabels As Integer, ByVal regression As Boolean, ByVal alignmentMode As AlignmentMode)
			Me.recordReader = featuresReader
			Me.labelsReader = labels
			Me.miniBatchSize = miniBatchSize
			Me.numPossibleLabels = numPossibleLabels
			Me.regression = regression
			Me.alignmentMode = alignmentMode
			Me.singleSequenceReaderMode = False
		End Sub

		''' <summary>
		''' Constructor where features and labels come from the SAME RecordReader (i.e., target/label is a column in the
		''' same data as the features). Defaults to regression = false - i.e., for classification </summary>
		''' <param name="reader"> SequenceRecordReader with data </param>
		''' <param name="miniBatchSize"> size of each minibatch </param>
		''' <param name="numPossibleLabels"> number of labels/classes for classification </param>
		''' <param name="labelIndex"> index in input of the label index. If in regression mode and numPossibleLabels > 1, labelIndex denotes the
		'''                   first index for labels. Everything before that index will be treated as input(s) and
		'''                   everything from that index (inclusive) to the end will be treated as output(s) </param>
		Public Sub New(ByVal reader As SequenceRecordReader, ByVal miniBatchSize As Integer, ByVal numPossibleLabels As Integer, ByVal labelIndex As Integer)
			Me.New(reader, miniBatchSize, numPossibleLabels, labelIndex, False)
		End Sub

		''' <summary>
		''' Constructor where features and labels come from the SAME RecordReader (i.e., target/label is a column in the
		''' same data as the features) </summary>
		''' <param name="reader"> SequenceRecordReader with data </param>
		''' <param name="miniBatchSize"> size of each minibatch </param>
		''' <param name="numPossibleLabels"> number of labels/classes for classification </param>
		''' <param name="labelIndex"> index in input of the label index. If in regression mode and numPossibleLabels > 1, labelIndex denotes the
		'''                   first index for labels. Everything before that index will be treated as input(s) and
		'''                   everything from that index (inclusive) to the end will be treated as output(s) </param>
		''' <param name="regression"> Whether output is for regression or classification </param>
		Public Sub New(ByVal reader As SequenceRecordReader, ByVal miniBatchSize As Integer, ByVal numPossibleLabels As Integer, ByVal labelIndex As Integer, ByVal regression As Boolean)
			Me.recordReader = reader
			Me.labelsReader = Nothing
			Me.miniBatchSize = miniBatchSize
			Me.regression = regression
			Me.labelIndex = labelIndex
			Me.numPossibleLabels = numPossibleLabels
			Me.singleSequenceReaderMode = True
		End Sub

		Private Sub initializeUnderlyingFromReader()
			initializeUnderlying(recordReader.nextSequence())
			underlying.reset()
		End Sub

		Private Sub initializeUnderlying(ByVal nextF As SequenceRecord)
			If nextF.getSequenceRecord().Count = 0 Then
				Throw New ZeroLengthSequenceException()
			End If
			Dim totalSizeF As Integer = nextF.getSequenceRecord()(0).Count

			'allow people to specify label index as -1 and infer the last possible label
			If singleSequenceReaderMode AndAlso numPossibleLabels >= 1 AndAlso labelIndex < 0 Then
				labelIndex = totalSizeF - 1
			ElseIf Not singleSequenceReaderMode AndAlso numPossibleLabels >= 1 AndAlso labelIndex < 0 Then
				labelIndex = 0
			End If

			recordReader.reset()

			'Add readers
			Dim builder As New RecordReaderMultiDataSetIterator.Builder(miniBatchSize)
			builder.addSequenceReader(READER_KEY, recordReader)
			If labelsReader IsNot Nothing Then
				builder.addSequenceReader(READER_KEY_LABEL, labelsReader)
			End If


			'Add outputs
			If singleSequenceReaderMode Then

				If labelIndex < 0 AndAlso numPossibleLabels < 0 Then
					'No labels - all values -> features array
					builder.addInput(READER_KEY)
				ElseIf labelIndex = 0 OrElse labelIndex = totalSizeF - 1 Then 'Features: subset of columns
					'Labels are first or last -> one input in underlying
					Dim inputFrom As Integer
					Dim inputTo As Integer
					If labelIndex < 0 Then
						'No label
						inputFrom = 0
						inputTo = totalSizeF - 1
					ElseIf labelIndex = 0 Then
						inputFrom = 1
						inputTo = totalSizeF - 1
					Else
						inputFrom = 0
						inputTo = labelIndex - 1
					End If

					builder.addInput(READER_KEY, inputFrom, inputTo)

					underlyingIsDisjoint = False
				ElseIf regression AndAlso numPossibleLabels > 1 Then
					'Multiple inputs and multiple outputs
					Dim inputFrom As Integer = 0
					Dim inputTo As Integer = labelIndex - 1
					Dim outputFrom As Integer = labelIndex
					Dim outputTo As Integer = totalSizeF - 1

					builder.addInput(READER_KEY, inputFrom, inputTo)
					builder.addOutput(READER_KEY, outputFrom, outputTo)

					underlyingIsDisjoint = False
				Else
					'Multiple inputs (disjoint features case)
					Dim firstFrom As Integer = 0
					Dim firstTo As Integer = labelIndex - 1
					Dim secondFrom As Integer = labelIndex + 1
					Dim secondTo As Integer = totalSizeF - 1

					builder.addInput(READER_KEY, firstFrom, firstTo)
					builder.addInput(READER_KEY, secondFrom, secondTo)

					underlyingIsDisjoint = True
				End If

				If Not (labelIndex < 0 AndAlso numPossibleLabels < 0) Then
					If regression AndAlso numPossibleLabels <= 1 Then
						'Multiple output regression already handled
						builder.addOutput(READER_KEY, labelIndex, labelIndex)
					ElseIf Not regression Then
						builder.addOutputOneHot(READER_KEY, labelIndex, numPossibleLabels)
					End If
				End If
			Else

				'Features: entire reader
				builder.addInput(READER_KEY)
				underlyingIsDisjoint = False

				If regression Then
					builder.addOutput(READER_KEY_LABEL)
				Else
					builder.addOutputOneHot(READER_KEY_LABEL, 0, numPossibleLabels)
				End If
			End If

			If alignmentMode <> Nothing Then
				Select Case alignmentMode
					Case org.deeplearning4j.datasets.datavec.SequenceRecordReaderDataSetIterator.AlignmentMode.EQUAL_LENGTH
						builder.sequenceAlignmentMode(RecordReaderMultiDataSetIterator.AlignmentMode.EQUAL_LENGTH)
					Case org.deeplearning4j.datasets.datavec.SequenceRecordReaderDataSetIterator.AlignmentMode.ALIGN_START
						builder.sequenceAlignmentMode(RecordReaderMultiDataSetIterator.AlignmentMode.ALIGN_START)
					Case org.deeplearning4j.datasets.datavec.SequenceRecordReaderDataSetIterator.AlignmentMode.ALIGN_END
						builder.sequenceAlignmentMode(RecordReaderMultiDataSetIterator.AlignmentMode.ALIGN_END)
				End Select
			End If

			underlying = builder.build()

			If collectMetaData Then
				underlying.setCollectMetaData(True)
			End If
		End Sub

		Private Function mdsToDataSet(ByVal mds As MultiDataSet) As DataSet
			Dim f As INDArray
			Dim fm As INDArray
			If underlyingIsDisjoint Then
				'Rare case: 2 input arrays -> concat
				Dim f1 As INDArray = RecordReaderDataSetIterator.getOrNull(mds.Features, 0)
				Dim f2 As INDArray = RecordReaderDataSetIterator.getOrNull(mds.Features, 1)
				fm = RecordReaderDataSetIterator.getOrNull(mds.FeaturesMaskArrays, 0) 'Per-example masking only on the input -> same for both

				'Can assume 3d features here
				f = Nd4j.createUninitialized(New Long() {f1.size(0), f1.size(1) + f2.size(1), f1.size(2)})
				f.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.interval(0, f1.size(1)), NDArrayIndex.all()}, f1)
				f.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.interval(f1.size(1), f1.size(1) + f2.size(1)), NDArrayIndex.all()}, f2)
			Else
				'Standard case
				f = RecordReaderDataSetIterator.getOrNull(mds.Features, 0)
				fm = RecordReaderDataSetIterator.getOrNull(mds.FeaturesMaskArrays, 0)
			End If

			Dim l As INDArray = RecordReaderDataSetIterator.getOrNull(mds.Labels, 0)
			Dim lm As INDArray = RecordReaderDataSetIterator.getOrNull(mds.LabelsMaskArrays, 0)

			Dim ds As New DataSet(f, l, fm, lm)

			If collectMetaData Then
				Dim temp As IList(Of Serializable) = mds.getExampleMetaData()
				Dim temp2 As IList(Of Serializable) = New List(Of Serializable)(temp.Count)
				For Each s As Serializable In temp
					Dim m As RecordMetaDataComposableMap = CType(s, RecordMetaDataComposableMap)
					If singleSequenceReaderMode Then
						temp2.Add(m.getMeta().get(READER_KEY))
					Else
						Dim c As New RecordMetaDataComposable(m.getMeta().get(READER_KEY), m.getMeta().get(READER_KEY_LABEL))
						temp2.Add(c)
					End If
				Next s
				ds.ExampleMetaData = temp2
			End If

			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess(ds)
			End If

			Return ds
		End Function

		Public Overrides Function hasNext() As Boolean
			If underlying Is Nothing Then
				initializeUnderlyingFromReader()
			End If
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return underlying.hasNext()
		End Function

		Public Overrides Function [next]() As DataSet
			Return [next](miniBatchSize)
		End Function


		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			If useStored Then
				useStored = False
				Dim temp As DataSet = stored
				stored = Nothing
				If preProcessor_Conflict IsNot Nothing Then
					preProcessor_Conflict.preProcess(temp)
				End If
				Return temp
			End If
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not hasNext() Then
				Throw New NoSuchElementException()
			End If

			If underlying Is Nothing Then
				initializeUnderlyingFromReader()
			End If

			Dim mds As MultiDataSet = underlying.next(num)
			Dim ds As DataSet = mdsToDataSet(mds)

			If totalOutcomes_Conflict = -1 Then
				inputColumns_Conflict = CInt(ds.Features.size(1))
				totalOutcomes_Conflict = If(ds.Labels Is Nothing, -1, CInt(ds.Labels.size(1)))
			End If

			Return ds
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			If inputColumns_Conflict <> -1 Then
				Return inputColumns_Conflict
			End If
			preLoad()
			Return inputColumns_Conflict
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			If totalOutcomes_Conflict <> -1 Then
				Return totalOutcomes_Conflict
			End If
			preLoad()
			Return totalOutcomes_Conflict
		End Function

		Private Sub preLoad()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			stored = [next]()
			useStored = True

			inputColumns_Conflict = CInt(stored.Features.size(1))
			totalOutcomes_Conflict = CInt(stored.Labels.size(1))
		End Sub

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			If underlying IsNot Nothing Then
				underlying.reset()
			End If

			cursor = 0
			stored = Nothing
			useStored = False
		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return miniBatchSize
		End Function

		Public Overridable WriteOnly Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
		End Property

		Public Overridable ReadOnly Property Labels As IList(Of String)
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides Sub remove()
			Throw New System.NotSupportedException("Remove not supported for this iterator")
		End Sub

		''' <summary>
		''' Load a single sequence example to a DataSet, using the provided RecordMetaData.
		''' Note that it is more efficient to load multiple instances at once, using <seealso cref="loadFromMetaData(List)"/>
		''' </summary>
		''' <param name="recordMetaData"> RecordMetaData to load from. Should have been produced by the given record reader </param>
		''' <returns> DataSet with the specified example </returns>
		''' <exception cref="IOException"> If an error occurs during loading of the data </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.dataset.DataSet loadFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws java.io.IOException
		Public Overridable Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As DataSet
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
'ORIGINAL LINE: public org.nd4j.linalg.dataset.DataSet loadFromMetaData(List<org.datavec.api.records.metadata.RecordMetaData> list) throws java.io.IOException
		Public Overridable Function loadFromMetaData(ByVal list As IList(Of RecordMetaData)) As DataSet
			If underlying Is Nothing Then
				Dim r As SequenceRecord = recordReader.loadSequenceFromMetaData(list(0))
				initializeUnderlying(r)
			End If

			'Two cases: single vs. multiple reader...
			Dim l As IList(Of RecordMetaData) = New List(Of RecordMetaData)(list.Count)
			If singleSequenceReaderMode Then
				For Each m As RecordMetaData In list
					l.Add(New RecordMetaDataComposableMap(Collections.singletonMap(READER_KEY, m)))
				Next m
			Else
				For Each m As RecordMetaData In list
					Dim rmdc As RecordMetaDataComposable = DirectCast(m, RecordMetaDataComposable)
					Dim map As IDictionary(Of String, RecordMetaData) = New Dictionary(Of String, RecordMetaData)(2)
					map(READER_KEY) = rmdc.getMeta()(0)
					map(READER_KEY_LABEL) = rmdc.getMeta()(1)
					l.Add(New RecordMetaDataComposableMap(map))
				Next m
			End If

			Return mdsToDataSet(underlying.loadFromMetaData(l))
		End Function
	End Class

End Namespace