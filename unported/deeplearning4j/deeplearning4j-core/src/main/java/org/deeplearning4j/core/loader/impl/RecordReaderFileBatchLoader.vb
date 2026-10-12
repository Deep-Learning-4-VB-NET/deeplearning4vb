Imports System
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports FileBatchRecordReader = org.datavec.api.records.reader.impl.filebatch.FileBatchRecordReader
Imports DataSetLoader = org.deeplearning4j.core.loader.DataSetLoader
Imports RecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderDataSetIterator
Imports FileBatch = org.nd4j.common.loader.FileBatch
Imports Source = org.nd4j.common.loader.Source
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor

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

Namespace org.deeplearning4j.core.loader.impl


	<Serializable>
	Public Class RecordReaderFileBatchLoader
		Implements DataSetLoader

		Private ReadOnly recordReader As RecordReader
		Private ReadOnly batchSize As Integer
		Private ReadOnly labelIndexFrom As Integer
		Private ReadOnly labelIndexTo As Integer
		Private ReadOnly numPossibleLabels As Integer
		Private ReadOnly regression As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private org.nd4j.linalg.dataset.api.DataSetPreProcessor preProcessor;
		Private preProcessor As DataSetPreProcessor

		''' <summary>
		''' Main constructor for classification. This will convert the input class index (at position labelIndex, with integer
		''' values 0 to numPossibleLabels-1 inclusive) to the appropriate one-hot output/labels representation.
		''' </summary>
		''' <param name="recordReader"> RecordReader: provides the source of the data </param>
		''' <param name="batchSize">    Batch size (number of examples) for the output DataSet objects </param>
		''' <param name="labelIndex">   Index of the label Writable (usually an IntWritable), as obtained by recordReader.next() </param>
		''' <param name="numClasses">   Number of classes (possible labels) for classification </param>
		Public Sub New(ByVal recordReader As RecordReader, ByVal batchSize As Integer, ByVal labelIndex As Integer, ByVal numClasses As Integer)
			Me.New(recordReader, batchSize, labelIndex, labelIndex, numClasses, False, Nothing)
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
			Me.New(recordReader, batchSize, labelIndexFrom, labelIndexTo, -1, regression, Nothing)
		End Sub

		''' <summary>
		''' Main constructor
		''' </summary>
		''' <param name="recordReader">      the recordreader to use </param>
		''' <param name="batchSize">         Minibatch size - number of examples returned for each call of .next() </param>
		''' <param name="labelIndexFrom">    the index of the label (for classification), or the first index of the labels for multi-output regression </param>
		''' <param name="labelIndexTo">      only used if regression == true. The last index <i>inclusive</i> of the multi-output regression </param>
		''' <param name="numPossibleLabels"> the number of possible labels for classification. Not used if regression == true </param>
		''' <param name="regression">        if true: regression. If false: classification (assume labelIndexFrom is the class it belongs to) </param>
		''' <param name="preProcessor">      Optional DataSetPreProcessor. May be null. </param>
		Public Sub New(ByVal recordReader As RecordReader, ByVal batchSize As Integer, ByVal labelIndexFrom As Integer, ByVal labelIndexTo As Integer, ByVal numPossibleLabels As Integer, ByVal regression As Boolean, ByVal preProcessor As DataSetPreProcessor)
			Me.recordReader = recordReader
			Me.batchSize = batchSize
			Me.labelIndexFrom = labelIndexFrom
			Me.labelIndexTo = labelIndexTo
			Me.numPossibleLabels = numPossibleLabels
			Me.regression = regression
			Me.preProcessor = preProcessor
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.DataSet load(org.nd4j.common.loader.Source source) throws java.io.IOException
		Public Overridable Function load(ByVal source As Source) As DataSet
			Dim fb As FileBatch = FileBatch.readFromZip(source.InputStream)

			'Wrap file batch in RecordReader
			'Create RecordReaderDataSetIterator
			'Return dataset
			Dim rr As RecordReader = New FileBatchRecordReader(recordReader, fb)
			Dim iter As New RecordReaderDataSetIterator(rr, Nothing, batchSize, labelIndexFrom, labelIndexTo, numPossibleLabels, -1, regression)
			If preProcessor IsNot Nothing Then
				iter.PreProcessor = preProcessor
			End If
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			Return ds
		End Function
	End Class

End Namespace