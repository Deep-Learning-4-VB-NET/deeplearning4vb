Imports System
Imports System.Collections.Generic
Imports [Function] = org.apache.spark.api.java.function.Function
Imports WritableConverter = org.datavec.api.io.WritableConverter
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports SequenceRecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.SequenceRecordReaderDataSetIterator
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports FeatureUtil = org.nd4j.linalg.util.FeatureUtil
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

Namespace org.deeplearning4j.spark.datavec


	<Serializable>
	Public Class DataVecSequencePairDataSetFunction
		Implements [Function](Of Tuple2(Of IList(Of IList(Of Writable)), IList(Of IList(Of Writable))), DataSet)

		''' <summary>
		'''Alignment mode for dealing with input/labels of differing lengths (for example, one-to-many and many-to-one type situations).
		''' For example, might have 10 time steps total but only one label at end for sequence classification.<br>
		''' <b>EQUAL_LENGTH</b>: Default. Assume that label and input time series are of equal length<br>
		''' <b>ALIGN_START</b>: Align the label/input time series at the first time step, and zero pad either the labels or
		''' the input at the end (pad whichever is shorter)<br>
		''' <b>ALIGN_END</b>: Align the label/input at the last time step, zero padding either the input or the labels as required<br>
		''' </summary>
		Public Enum AlignmentMode
			EQUAL_LENGTH
			ALIGN_START
			ALIGN_END
		End Enum

		Private ReadOnly regression As Boolean
		Private ReadOnly numPossibleLabels As Integer
		Private ReadOnly alignmentMode As AlignmentMode
		Private ReadOnly preProcessor As DataSetPreProcessor
		Private ReadOnly converter As WritableConverter

		''' <summary>
		''' Constructor for equal length and no conversion of labels (i.e., regression or already in one-hot representation).
		''' No data set proprocessor or writable converter
		''' </summary>
		Public Sub New()
			Me.New(-1, True)
		End Sub

		''' <summary>
		'''Constructor for equal length, no data set preprocessor or writable converter </summary>
		''' <seealso cref= #DataVecSequencePairDataSetFunction(int, boolean, AlignmentMode, DataSetPreProcessor, WritableConverter) </seealso>
		Public Sub New(ByVal numPossibleLabels As Integer, ByVal regression As Boolean)
			Me.New(numPossibleLabels, regression, AlignmentMode.EQUAL_LENGTH)
		End Sub

		''' <summary>
		'''Constructor for data with a specified alignment mode, no data set preprocessor or writable converter </summary>
		''' <seealso cref= #DataVecSequencePairDataSetFunction(int, boolean, AlignmentMode, DataSetPreProcessor, WritableConverter) </seealso>
		Public Sub New(ByVal numPossibleLabels As Integer, ByVal regression As Boolean, ByVal alignmentMode As AlignmentMode)
			Me.New(numPossibleLabels, regression, alignmentMode, Nothing, Nothing)
		End Sub

		''' <param name="numPossibleLabels"> Number of classes for classification  (not used if regression = true) </param>
		''' <param name="regression"> False for classification, true for regression </param>
		''' <param name="alignmentMode"> Alignment mode for data. See <seealso cref="DataVecSequencePairDataSetFunction.AlignmentMode"/> </param>
		''' <param name="preProcessor"> DataSetPreprocessor (may be null) </param>
		''' <param name="converter"> WritableConverter (may be null) </param>
		Public Sub New(ByVal numPossibleLabels As Integer, ByVal regression As Boolean, ByVal alignmentMode As AlignmentMode, ByVal preProcessor As DataSetPreProcessor, ByVal converter As WritableConverter)
			Me.numPossibleLabels = numPossibleLabels
			Me.regression = regression
			Me.alignmentMode = alignmentMode
			Me.preProcessor = preProcessor
			Me.converter = converter
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.DataSet call(scala.Tuple2<java.util.List<java.util.List<org.datavec.api.writable.Writable>>, java.util.List<java.util.List<org.datavec.api.writable.Writable>>> input) throws Exception
		Public Overrides Function [call](ByVal input As Tuple2(Of IList(Of IList(Of Writable)), IList(Of IList(Of Writable)))) As DataSet
			Dim featuresSeq As IList(Of IList(Of Writable)) = input._1()
			Dim labelsSeq As IList(Of IList(Of Writable)) = input._2()

			Dim featuresLength As Integer = featuresSeq.Count
			Dim labelsLength As Integer = labelsSeq.Count


			Dim fIter As IEnumerator(Of IList(Of Writable)) = featuresSeq.GetEnumerator()
			Dim lIter As IEnumerator(Of IList(Of Writable)) = labelsSeq.GetEnumerator()

			Dim inputArr As INDArray = Nothing
			Dim outputArr As INDArray = Nothing

			Dim idx(2) As Integer
			Dim i As Integer = 0
			Do While fIter.MoveNext()
				Dim [step] As IList(Of Writable) = fIter.Current
				If i = 0 Then
					Dim inShape() As Integer = {1, [step].Count, featuresLength}
					inputArr = Nd4j.create(inShape)
				End If
				Dim timeStepIter As IEnumerator(Of Writable) = [step].GetEnumerator()
				Dim f As Integer = 0
				idx(1) = 0
				Do While timeStepIter.MoveNext()
					Dim current As Writable = timeStepIter.Current
					If converter IsNot Nothing Then
						current = converter.convert(current)
					End If
					Try
						inputArr.putScalar(idx, current.toDouble())
					Catch e As System.NotSupportedException
						' This isn't a scalar, so check if we got an array already
						If TypeOf current Is NDArrayWritable Then
							inputArr.get(NDArrayIndex.point(idx(0)), NDArrayIndex.all(), NDArrayIndex.point(idx(2))).putRow(0, DirectCast(current, NDArrayWritable).get())
						Else
							Throw e
						End If
					End Try
					f += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: idx[1] = ++f;
					idx(1) = f
				Loop
				i += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: idx[2] = ++i;
				idx(2) = i
			Loop

			idx = New Integer(2){}
			i = 0
			Do While lIter.MoveNext()
				Dim [step] As IList(Of Writable) = lIter.Current
				If i = 0 Then
					Dim outShape() As Integer = {1, (If(regression, [step].Count, numPossibleLabels)), labelsLength}
					outputArr = Nd4j.create(outShape)
				End If
				Dim timeStepIter As IEnumerator(Of Writable) = [step].GetEnumerator()
				Dim f As Integer = 0
				idx(1) = 0
				If regression Then
					'Load all values without modification
					Do While timeStepIter.MoveNext()
						Dim current As Writable = timeStepIter.Current
						If converter IsNot Nothing Then
							current = converter.convert(current)
						End If
						outputArr.putScalar(idx, current.toDouble())
						f += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: idx[1] = ++f;
						idx(1) = f
					Loop
				Else
					'Expect a single value (index) -> convert to one-hot vector
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim value As Writable = timeStepIter.next()
					Dim labelClassIdx As Integer = value.toInt()
					Dim line As INDArray = FeatureUtil.toOutcomeVector(labelClassIdx, numPossibleLabels)
					outputArr.tensorAlongDimension(i, 1).assign(line) '1d from [1,nOut,timeSeriesLength] -> tensor i along dimension 1 is at time i
				End If

				i += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: idx[2] = ++i;
				idx(2) = i
			Loop

			Dim ds As DataSet
			If alignmentMode = AlignmentMode.EQUAL_LENGTH OrElse featuresLength = labelsLength Then
				ds = New DataSet(inputArr, outputArr)
			ElseIf alignmentMode = AlignmentMode.ALIGN_END Then
				If featuresLength > labelsLength Then
					'Input longer, pad output
					Dim newOutput As INDArray = Nd4j.create(1, outputArr.size(1), featuresLength)
					newOutput.get(NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.interval(featuresLength - labelsLength, featuresLength)).assign(outputArr)
					'Need an output mask array, but not an input mask array
					Dim outputMask As INDArray = Nd4j.create(1, featuresLength)
					For j As Integer = featuresLength - labelsLength To featuresLength - 1
						outputMask.putScalar(j, 1.0)
					Next j
					ds = New DataSet(inputArr, newOutput, Nd4j.ones(outputMask.shape()), outputMask)
				Else
					'Output longer, pad input
					Dim newInput As INDArray = Nd4j.create(1, inputArr.size(1), labelsLength)
					newInput.get(NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.interval(labelsLength - featuresLength, labelsLength)).assign(inputArr)
					'Need an input mask array, but not an output mask array
					Dim inputMask As INDArray = Nd4j.create(1, labelsLength)
					For j As Integer = labelsLength - featuresLength To labelsLength - 1
						inputMask.putScalar(j, 1.0)
					Next j
					ds = New DataSet(newInput, outputArr, inputMask, Nd4j.ones(inputMask.shape()))
				End If
			ElseIf alignmentMode = AlignmentMode.ALIGN_START Then
				If featuresLength > labelsLength Then
					'Input longer, pad output
					Dim newOutput As INDArray = Nd4j.create(1, outputArr.size(1), featuresLength)
					newOutput.get(NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.interval(0, labelsLength)).assign(outputArr)
					'Need an output mask array, but not an input mask array
					Dim outputMask As INDArray = Nd4j.create(1, featuresLength)
					For j As Integer = 0 To labelsLength - 1
						outputMask.putScalar(j, 1.0)
					Next j
					ds = New DataSet(inputArr, newOutput, Nd4j.ones(outputMask.shape()), outputMask)
				Else
					'Output longer, pad input
					Dim newInput As INDArray = Nd4j.create(1, inputArr.size(1), labelsLength)
					newInput.get(NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.interval(0, featuresLength)).assign(inputArr)
					'Need an input mask array, but not an output mask array
					Dim inputMask As INDArray = Nd4j.create(1, labelsLength)
					For j As Integer = 0 To featuresLength - 1
						inputMask.putScalar(j, 1.0)
					Next j
					ds = New DataSet(newInput, outputArr, inputMask, Nd4j.ones(inputMask.shape()))
				End If
			Else
				Throw New System.NotSupportedException("Invalid alignment mode: " & alignmentMode)
			End If


			If preProcessor IsNot Nothing Then
				preProcessor.preProcess(ds)
			End If
			Return ds
		End Function
	End Class

End Namespace