Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports WritableConverter = org.datavec.api.io.WritableConverter
Imports SelfWritableConverter = org.datavec.api.io.converters.SelfWritableConverter
Imports Record = org.datavec.api.records.Record
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataComposableMap = org.datavec.api.records.metadata.RecordMetaDataComposableMap
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports ConcatenatingRecordReader = org.datavec.api.records.reader.impl.ConcatenatingRecordReader
Imports CollectionRecordReader = org.datavec.api.records.reader.impl.collection.CollectionRecordReader
Imports Writable = org.datavec.api.writable.Writable
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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
'ORIGINAL LINE: @Slf4j public class RecordReaderDataSetIterator implements org.nd4j.linalg.dataset.api.iterator.DataSetIterator
	<Serializable>
	Public Class RecordReaderDataSetIterator
		Implements DataSetIterator

		Private Const READER_KEY As String = "reader"
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.datavec.api.records.reader.RecordReader recordReader;
		Protected Friend recordReader As RecordReader
		Protected Friend converter As WritableConverter
		Protected Friend batchSize As Integer = 10
		Protected Friend maxNumBatches As Integer = -1
		Protected Friend batchNum As Integer = 0
		Protected Friend labelIndex As Integer = -1
		Protected Friend labelIndexTo As Integer = -1
		Protected Friend numPossibleLabels As Integer = -1
		Protected Friend sequenceIter As IEnumerator(Of IList(Of Writable))
		Protected Friend last As DataSet
		Protected Friend useCurrent As Boolean = False
		Protected Friend regression As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.nd4j.linalg.dataset.api.DataSetPreProcessor preProcessor;
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend preProcessor_Conflict As DataSetPreProcessor

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean collectMetaData = false;
'JAVA TO VB CONVERTER NOTE: The field collectMetaData was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private collectMetaData_Conflict As Boolean = False

		Private underlying As RecordReaderMultiDataSetIterator
		Private underlyingIsDisjoint As Boolean

		''' <summary>
		''' Constructor for classification, where:<br>
		''' (a) the label index is assumed to be the very last Writable/column, and<br>
		''' (b) the number of classes is inferred from RecordReader.getLabels()<br>
		''' Note that if RecordReader.getLabels() returns null, no output labels will be produced
		''' </summary>
		''' <param name="recordReader"> Record reader to use as the source of data </param>
		''' <param name="batchSize">    Minibatch size, for each call of .next() </param>
		Public Sub New(ByVal recordReader As RecordReader, ByVal batchSize As Integer)
			Me.New(recordReader, New SelfWritableConverter(), batchSize, -1, -1,If(recordReader.getLabels() Is Nothing, -1, recordReader.getLabels().Count), -1, False)
		End Sub

		''' <summary>
		''' Main constructor for classification. This will convert the input class index (at position labelIndex, with integer
		''' values 0 to numPossibleLabels-1 inclusive) to the appropriate one-hot output/labels representation.
		''' </summary>
		''' <param name="recordReader">         RecordReader: provides the source of the data </param>
		''' <param name="batchSize">            Batch size (number of examples) for the output DataSet objects </param>
		''' <param name="labelIndex">           Index of the label Writable (usually an IntWritable), as obtained by recordReader.next() </param>
		''' <param name="numPossibleLabels">    Number of classes (possible labels) for classification </param>
		Public Sub New(ByVal recordReader As RecordReader, ByVal batchSize As Integer, ByVal labelIndex As Integer, ByVal numPossibleLabels As Integer)
			Me.New(recordReader, New SelfWritableConverter(), batchSize, labelIndex, labelIndex, numPossibleLabels, -1, False)
		End Sub

		''' <summary>
		''' Constructor for classification, where the maximum number of returned batches is limited to the specified value
		''' </summary>
		''' <param name="recordReader">      the recordreader to use </param>
		''' <param name="labelIndex">        the index/column of the label (for classification) </param>
		''' <param name="numPossibleLabels"> the number of possible labels for classification. Not used if regression == true </param>
		''' <param name="maxNumBatches">     The maximum number of batches to return between resets. Set to -1 to return all available data </param>
		Public Sub New(ByVal recordReader As RecordReader, ByVal batchSize As Integer, ByVal labelIndex As Integer, ByVal numPossibleLabels As Integer, ByVal maxNumBatches As Integer)
			Me.New(recordReader, New SelfWritableConverter(), batchSize, labelIndex, labelIndex, numPossibleLabels, maxNumBatches, False)
		End Sub

		''' <summary>
		''' Main constructor for multi-label regression (i.e., regression with multiple outputs). Can also be used for single
		''' output regression with labelIndexFrom == labelIndexTo
		''' </summary>
		''' <param name="recordReader">      RecordReader to get data from </param>
		''' <param name="labelIndexFrom">    Index of the first regression target </param>
		''' <param name="labelIndexTo">      Index of the last regression target, inclusive </param>
		''' <param name="batchSize">         Minibatch size </param>
		''' <param name="regression">        Require regression = true. Mainly included to avoid clashing with other constructors previously defined :/ </param>
		Public Sub New(ByVal recordReader As RecordReader, ByVal batchSize As Integer, ByVal labelIndexFrom As Integer, ByVal labelIndexTo As Integer, ByVal regression As Boolean)
			Me.New(recordReader, New SelfWritableConverter(), batchSize, labelIndexFrom, labelIndexTo, -1, -1, regression)
			If Not regression Then
				Throw New System.ArgumentException("This constructor is only for creating regression iterators. " & "If you're doing classification you need to use another constructor that " & "(implicitly) specifies numPossibleLabels")
			End If
		End Sub


		''' <summary>
		''' Main constructor
		''' </summary>
		''' <param name="recordReader">      the recordreader to use </param>
		''' <param name="converter">         Converter. May be null. </param>
		''' <param name="batchSize">         Minibatch size - number of examples returned for each call of .next() </param>
		''' <param name="labelIndexFrom">    the index of the label (for classification), or the first index of the labels for multi-output regression </param>
		''' <param name="labelIndexTo">      only used if regression == true. The last index <i>inclusive</i> of the multi-output regression </param>
		''' <param name="numPossibleLabels"> the number of possible labels for classification. Not used if regression == true </param>
		''' <param name="maxNumBatches">     Maximum number of batches to return </param>
		''' <param name="regression">        if true: regression. If false: classification (assume labelIndexFrom is the class it belongs to) </param>
		Public Sub New(ByVal recordReader As RecordReader, ByVal converter As WritableConverter, ByVal batchSize As Integer, ByVal labelIndexFrom As Integer, ByVal labelIndexTo As Integer, ByVal numPossibleLabels As Integer, ByVal maxNumBatches As Integer, ByVal regression As Boolean)
			Me.recordReader = recordReader
			Me.converter = converter
			Me.batchSize = batchSize
			Me.maxNumBatches = maxNumBatches
			Me.labelIndex = labelIndexFrom
			Me.labelIndexTo = labelIndexTo
			Me.numPossibleLabels = numPossibleLabels
			Me.regression = regression
		End Sub


		Protected Friend Sub New(ByVal b As Builder)
			Me.recordReader = b.recordReader
			Me.converter = b.converter
			Me.batchSize = b.batchSize
			Me.maxNumBatches = b.maxNumBatches_Conflict
			Me.labelIndex = b.labelIndex
			Me.labelIndexTo = b.labelIndexTo
			Me.numPossibleLabels = b.numPossibleLabels
			Me.regression = b.regression_Conflict
			Me.preProcessor_Conflict = b.preProcessor_Conflict
			Me.collectMetaData_Conflict = b.collectMetaData_Conflict
		End Sub

		''' <summary>
		''' When set to true: metadata for  the current examples will be present in the returned DataSet.
		''' Disabled by default.
		''' </summary>
		''' <param name="collectMetaData"> Whether to collect metadata or  not </param>
		Public Overridable WriteOnly Property CollectMetaData As Boolean
			Set(ByVal collectMetaData As Boolean)
				If underlying IsNot Nothing Then
					underlying.setCollectMetaData(collectMetaData)
				End If
				Me.collectMetaData_Conflict = collectMetaData
			End Set
		End Property

		Private Sub initializeUnderlying()
			If underlying Is Nothing Then
				Dim [next] As Record = recordReader.nextRecord()
				initializeUnderlying([next])
			End If
		End Sub

		Private Sub initializeUnderlying(ByVal [next] As Record)
			Dim totalSize As Integer = [next].getRecord().Count

			'allow people to specify label index as -1 and infer the last possible label
			If numPossibleLabels >= 1 AndAlso labelIndex < 0 Then
				labelIndex = totalSize - 1
				labelIndexTo = labelIndex
			End If

			If recordReader.resetSupported() Then
				recordReader.reset()
			Else
				'Hack around the fact that we need the first record to initialize the underlying RRMDSI, but can't reset
				' the original reader
				recordReader = New ConcatenatingRecordReader(New CollectionRecordReader(Collections.singletonList([next].getRecord())), recordReader)
			End If

			Dim builder As New RecordReaderMultiDataSetIterator.Builder(batchSize)
			If TypeOf recordReader Is SequenceRecordReader Then
				builder.addSequenceReader(READER_KEY, DirectCast(recordReader, SequenceRecordReader))
			Else
				builder.addReader(READER_KEY, recordReader)
			End If

			If regression Then
				builder.addOutput(READER_KEY, labelIndex, labelIndexTo)
			ElseIf numPossibleLabels >= 1 Then
				builder.addOutputOneHot(READER_KEY, labelIndex, numPossibleLabels)
			End If

			'Inputs: assume to be all of the other writables
			'In general: can't assume label indices are all at the start or end (event though 99% of the time they are)
			'If they are: easy. If not: use 2 inputs in the underlying as a workaround, and concat them

			If labelIndex >= 0 AndAlso (labelIndex = 0 OrElse labelIndexTo = totalSize - 1) Then
				'Labels are first or last -> one input in underlying
				Dim inputFrom As Integer
				Dim inputTo As Integer
				If labelIndex < 0 Then
					'No label
					inputFrom = 0
					inputTo = totalSize - 1
				ElseIf labelIndex = 0 Then
					inputFrom = labelIndexTo + 1
					inputTo = totalSize - 1
				Else
					inputFrom = 0
					inputTo = labelIndex - 1
				End If

				builder.addInput(READER_KEY, inputFrom, inputTo)

				underlyingIsDisjoint = False
			ElseIf labelIndex >= 0 Then
				Preconditions.checkState(labelIndex < [next].getRecord().Count, "Invalid label (from) index: index must be in range 0 to first record size of (0 to %s inclusive), got %s", [next].getRecord().Count - 1, labelIndex)
				Preconditions.checkState(labelIndexTo < [next].getRecord().Count, "Invalid label (to) index: index must be in range 0 to first record size of (0 to %s inclusive), got %s", [next].getRecord().Count - 1, labelIndexTo)


				'Multiple inputs
				Dim firstFrom As Integer = 0
				Dim firstTo As Integer = labelIndex - 1
				Dim secondFrom As Integer = labelIndexTo + 1
				Dim secondTo As Integer = totalSize - 1

				builder.addInput(READER_KEY, firstFrom, firstTo)
				builder.addInput(READER_KEY, secondFrom, secondTo)

				underlyingIsDisjoint = True
			Else
				'No labels - only features
				builder.addInput(READER_KEY)
				underlyingIsDisjoint = False
			End If


			underlying = builder.build()

			If collectMetaData_Conflict Then
				underlying.setCollectMetaData(True)
			End If
		End Sub

		Private Function mdsToDataSet(ByVal mds As MultiDataSet) As DataSet
			Dim f As INDArray
			Dim fm As INDArray
			If underlyingIsDisjoint Then
				'Rare case: 2 input arrays -> concat
				Dim f1 As INDArray = getOrNull(mds.Features, 0)
				Dim f2 As INDArray = getOrNull(mds.Features, 1)
				fm = getOrNull(mds.FeaturesMaskArrays, 0) 'Per-example masking only on the input -> same for both

				'Can assume 2d features here
				f = Nd4j.hstack(f1, f2)
			Else
				'Standard case
				f = getOrNull(mds.Features, 0)
				fm = getOrNull(mds.FeaturesMaskArrays, 0)
			End If

			Dim l As INDArray = getOrNull(mds.Labels, 0)
			Dim lm As INDArray = getOrNull(mds.LabelsMaskArrays, 0)

			Dim ds As New DataSet(f, l, fm, lm)

			If collectMetaData_Conflict Then
				Dim temp As IList(Of Serializable) = mds.getExampleMetaData()
				Dim temp2 As IList(Of Serializable) = New List(Of Serializable)(temp.Count)
				For Each s As Serializable In temp
					Dim m As RecordMetaDataComposableMap = CType(s, RecordMetaDataComposableMap)
					temp2.Add(m.getMeta().get(READER_KEY))
				Next s
				ds.ExampleMetaData = temp2
			End If

			'Edge case, for backward compatibility:
			'If labelIdx == -1 && numPossibleLabels == -1 -> no labels -> set labels array to features array
			If labelIndex = -1 AndAlso numPossibleLabels = -1 AndAlso ds.Labels Is Nothing Then
				ds.Labels = ds.Features
			End If

			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess(ds)
			End If

			Return ds
		End Function


		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			If useCurrent Then
				useCurrent = False
				If preProcessor_Conflict IsNot Nothing Then
					preProcessor_Conflict.preProcess(last)
				End If
				Return last
			End If

			If underlying Is Nothing Then
				initializeUnderlying()
			End If


			batchNum += 1
			Return mdsToDataSet(underlying.next(num))
		End Function

		'Package private
		Friend Shared Function getOrNull(ByVal arr() As INDArray, ByVal idx As Integer) As INDArray
			If arr Is Nothing OrElse arr.Length = 0 Then
				Return Nothing
			End If
			Return arr(idx)
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			If last Is Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim [next] As DataSet = Me.next()
				last = [next]
				useCurrent = True
				Return [next].numInputs()
			Else
				Return last.numInputs()
			End If
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			If last Is Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim [next] As DataSet = Me.next()
				last = [next]
				useCurrent = True
				Return [next].numOutcomes()
			Else
				Return last.numOutcomes()
			End If
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			If underlying Is Nothing Then
				initializeUnderlying()
			End If
			Return underlying.resetSupported()
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			batchNum = 0
			If underlying IsNot Nothing Then
				underlying.reset()
			End If

			last = Nothing
			useCurrent = False
		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return batchSize
		End Function

		Public Overridable WriteOnly Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
		End Property

		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return (((sequenceIter IsNot Nothing AndAlso sequenceIter.hasNext()) OrElse recordReader.hasNext()) AndAlso (maxNumBatches < 0 OrElse batchNum < maxNumBatches))
		End Function

		Public Overrides Function [next]() As DataSet
			Return [next](batchSize)
		End Function

		Public Overrides Sub remove()
			Throw New System.NotSupportedException()
		End Sub

		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Return recordReader.getLabels()
			End Get
		End Property

		''' <summary>
		''' Load a single example to a DataSet, using the provided RecordMetaData.
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
		''' Load a multiple examples to a DataSet, using the provided RecordMetaData instances.
		''' </summary>
		''' <param name="list"> List of RecordMetaData instances to load from. Should have been produced by the record reader provided
		'''             to the RecordReaderDataSetIterator constructor </param>
		''' <returns> DataSet with the specified examples </returns>
		''' <exception cref="IOException"> If an error occurs during loading of the data </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.dataset.DataSet loadFromMetaData(java.util.List<org.datavec.api.records.metadata.RecordMetaData> list) throws java.io.IOException
		Public Overridable Function loadFromMetaData(ByVal list As IList(Of RecordMetaData)) As DataSet
			If underlying Is Nothing Then
				Dim r As Record = recordReader.loadFromMetaData(list(0))
				initializeUnderlying(r)
			End If

			'Convert back to composable:
			Dim l As IList(Of RecordMetaData) = New List(Of RecordMetaData)(list.Count)
			For Each m As RecordMetaData In list
				l.Add(New RecordMetaDataComposableMap(Collections.singletonMap(READER_KEY, m)))
			Next m
			Dim m As MultiDataSet = underlying.loadFromMetaData(l)

			Return mdsToDataSet(m)
		End Function

		''' <summary>
		''' Builder class for RecordReaderDataSetIterator
		''' </summary>
		Public Class Builder

			Protected Friend recordReader As RecordReader
			Protected Friend converter As WritableConverter
			Protected Friend batchSize As Integer
'JAVA TO VB CONVERTER NOTE: The field maxNumBatches was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend maxNumBatches_Conflict As Integer = -1
			Protected Friend labelIndex As Integer = -1
			Protected Friend labelIndexTo As Integer = -1
			Protected Friend numPossibleLabels As Integer = -1
'JAVA TO VB CONVERTER NOTE: The field regression was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend regression_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend preProcessor_Conflict As DataSetPreProcessor
'JAVA TO VB CONVERTER NOTE: The field collectMetaData was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectMetaData_Conflict As Boolean = False

			Friend clOrRegCalled As Boolean = False

			''' 
			''' <param name="rr">        Underlying record reader to source data from </param>
			''' <param name="batchSize"> Batch size to use </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull RecordReader rr, int batchSize)
			Public Sub New(ByVal rr As RecordReader, ByVal batchSize As Integer)
				Me.recordReader = rr
				Me.batchSize = batchSize
			End Sub

			Public Overridable Function writableConverter(ByVal converter As WritableConverter) As Builder
				Me.converter = converter
				Return Me
			End Function

			''' <summary>
			''' Optional argument, usually not used. If set, can be used to limit the maximum number of minibatches that
			''' will be returned (between resets). If not set, will always return as many minibatches as there is data
			''' available.
			''' </summary>
			''' <param name="maxNumBatches"> Maximum number of minibatches per epoch / reset </param>
'JAVA TO VB CONVERTER NOTE: The parameter maxNumBatches was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function maxNumBatches(ByVal maxNumBatches_Conflict As Integer) As Builder
				Me.maxNumBatches_Conflict = maxNumBatches_Conflict
				Return Me
			End Function

			''' <summary>
			''' Use this for single output regression (i.e., 1 output/regression target)
			''' </summary>
			''' <param name="labelIndex"> Column index that contains the regression target (indexes start at 0) </param>
			Public Overridable Function regression(ByVal labelIndex As Integer) As Builder
				Return regression(labelIndex, labelIndex)
			End Function

			''' <summary>
			''' Use this for multiple output regression (1 or more output/regression targets). Note that all regression
			''' targets must be contiguous (i.e., positions x to y, without gaps)
			''' </summary>
			''' <param name="labelIndexFrom"> Column index of the first regression target (indexes start at 0) </param>
			''' <param name="labelIndexTo">   Column index of the last regression target (inclusive) </param>
			Public Overridable Function regression(ByVal labelIndexFrom As Integer, ByVal labelIndexTo As Integer) As Builder
				Me.labelIndex = labelIndexFrom
				Me.labelIndexTo = labelIndexTo
				Me.regression_Conflict = True
				clOrRegCalled = True
				Return Me
			End Function

			''' <summary>
			''' Use this for classification
			''' </summary>
			''' <param name="labelIndex"> Index that contains the label index. Column (indexes start from 0) be an integer value,
			'''                   and contain values 0 to numClasses-1 </param>
			''' <param name="numClasses"> Number of label classes (i.e., number of categories/classes in the dataset) </param>
			Public Overridable Function classification(ByVal labelIndex As Integer, ByVal numClasses As Integer) As Builder
				Me.labelIndex = labelIndex
				Me.labelIndexTo = labelIndex
				Me.numPossibleLabels = numClasses
				Me.regression_Conflict = False
				clOrRegCalled = True
				Return Me
			End Function

			''' <summary>
			''' Optional arg. Allows the preprocessor to be set </summary>
			''' <param name="preProcessor"> Preprocessor to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter preProcessor was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function preProcessor(ByVal preProcessor_Conflict As DataSetPreProcessor) As Builder
				Me.preProcessor_Conflict = preProcessor_Conflict
				Return Me
			End Function

			''' <summary>
			''' When set to true: metadata for  the current examples will be present in the returned DataSet.
			''' Disabled by default.
			''' </summary>
			''' <param name="collectMetaData"> Whether metadata should be collected or not </param>
'JAVA TO VB CONVERTER NOTE: The parameter collectMetaData was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectMetaData(ByVal collectMetaData_Conflict As Boolean) As Builder
				Me.collectMetaData_Conflict = collectMetaData_Conflict
				Return Me
			End Function

			Public Overridable Function build() As RecordReaderDataSetIterator
				Return New RecordReaderDataSetIterator(Me)
			End Function

		End Class
	End Class

End Namespace