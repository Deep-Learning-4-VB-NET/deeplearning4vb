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
	Public Class DataVecSequenceDataSetFunction
		Implements [Function](Of IList(Of IList(Of Writable)), DataSet)

		Private ReadOnly regression As Boolean
		Private ReadOnly labelIndex As Integer
		Private ReadOnly numPossibleLabels As Integer
		Private ReadOnly preProcessor As DataSetPreProcessor
		Private ReadOnly converter As WritableConverter

		''' <param name="labelIndex"> Index of the label column </param>
		''' <param name="numPossibleLabels"> Number of classes for classification  (not used if regression = true) </param>
		''' <param name="regression"> False for classification, true for regression </param>
		Public Sub New(ByVal labelIndex As Integer, ByVal numPossibleLabels As Integer, ByVal regression As Boolean)
			Me.New(labelIndex, numPossibleLabels, regression, Nothing, Nothing)
		End Sub

		''' <param name="labelIndex"> Index of the label column </param>
		''' <param name="numPossibleLabels"> Number of classes for classification  (not used if regression = true) </param>
		''' <param name="regression"> False for classification, true for regression </param>
		''' <param name="preProcessor"> DataSetPreprocessor (may be null) </param>
		''' <param name="converter"> WritableConverter (may be null) </param>
		Public Sub New(ByVal labelIndex As Integer, ByVal numPossibleLabels As Integer, ByVal regression As Boolean, ByVal preProcessor As DataSetPreProcessor, ByVal converter As WritableConverter)
			Me.labelIndex = labelIndex
			Me.numPossibleLabels = numPossibleLabels
			Me.regression = regression
			Me.preProcessor = preProcessor
			Me.converter = converter
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.DataSet call(java.util.List<java.util.List<org.datavec.api.writable.Writable>> input) throws Exception
		Public Overrides Function [call](ByVal input As IList(Of IList(Of Writable))) As DataSet
			Dim iter As IEnumerator(Of IList(Of Writable)) = input.GetEnumerator()

			Dim features As INDArray = Nothing
			Dim labels As INDArray = Nd4j.zeros(1, (If(regression, 1, numPossibleLabels)), input.Count)

			Dim fIdx(2) As Integer
			Dim lIdx(2) As Integer

			Dim i As Integer = 0
			Do While iter.MoveNext()
				Dim [step] As IList(Of Writable) = iter.Current
				If i = 0 Then
					features = Nd4j.zeros(1, [step].Count - 1, input.Count)
				End If

				Dim timeStepIter As IEnumerator(Of Writable) = [step].GetEnumerator()
				Dim countIn As Integer = 0
				Dim countFeatures As Integer = 0
				Do While timeStepIter.MoveNext()
					Dim current As Writable = timeStepIter.Current
					If converter IsNot Nothing Then
						current = converter.convert(current)
					End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (countIn++ == labelIndex)
					If countIn = labelIndex Then
							countIn += 1
						'label
						If regression Then
							lIdx(2) = i
							labels.putScalar(lIdx, current.toDouble())
						Else
							Dim line As INDArray = FeatureUtil.toOutcomeVector(current.toInt(), numPossibleLabels)
							labels.tensorAlongDimension(i, 1).assign(line) '1d from [1,nOut,timeSeriesLength] -> tensor i along dimension 1 is at time i
						End If
					Else
							countIn += 1
						'feature
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: fIdx[1] = countFeatures++;
						fIdx(1) = countFeatures
							countFeatures += 1
						fIdx(2) = i
						Try
							features.putScalar(fIdx, current.toDouble())
						Catch e As System.NotSupportedException
							' This isn't a scalar, so check if we got an array already
							If TypeOf current Is NDArrayWritable Then
								features.get(NDArrayIndex.point(fIdx(0)), NDArrayIndex.all(), NDArrayIndex.point(fIdx(2))).putRow(0, DirectCast(current, NDArrayWritable).get())
							Else
								Throw e
							End If
						End Try
					End If
				Loop
				i += 1
			Loop

			Dim ds As New DataSet(features, labels)
			If preProcessor IsNot Nothing Then
				preProcessor.preProcess(ds)
			End If
			Return ds
		End Function
	End Class

End Namespace