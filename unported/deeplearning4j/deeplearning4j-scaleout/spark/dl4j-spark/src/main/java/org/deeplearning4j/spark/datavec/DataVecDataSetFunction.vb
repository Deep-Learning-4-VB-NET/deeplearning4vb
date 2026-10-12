Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports [Function] = org.apache.spark.api.java.function.Function
Imports WritableConverter = org.datavec.api.io.WritableConverter
Imports WritableConverterException = org.datavec.api.io.converters.WritableConverterException
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports RecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderDataSetIterator
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DataVecDataSetFunction implements org.apache.spark.api.java.function.@Function<java.util.List<org.datavec.api.writable.Writable>, org.nd4j.linalg.dataset.DataSet>, java.io.Serializable
	<Serializable>
	Public Class DataVecDataSetFunction
		Implements [Function](Of IList(Of Writable), DataSet)

		Private ReadOnly labelIndex As Integer
		Private ReadOnly labelIndexTo As Integer
		Private ReadOnly numPossibleLabels As Integer
		Private ReadOnly regression As Boolean
		Private ReadOnly preProcessor As DataSetPreProcessor
		Private ReadOnly converter As WritableConverter
		Protected Friend batchSize As Integer = -1

		Public Sub New(ByVal labelIndex As Integer, ByVal numPossibleLabels As Integer, ByVal regression As Boolean)
			Me.New(labelIndex, numPossibleLabels, regression, Nothing, Nothing)
		End Sub

		''' <param name="labelIndex"> Index of the label column </param>
		''' <param name="numPossibleLabels"> Number of classes for classification  (not used if regression = true) </param>
		''' <param name="regression"> False for classification, true for regression </param>
		''' <param name="preProcessor"> DataSetPreprocessor (may be null) </param>
		''' <param name="converter"> WritableConverter (may be null) </param>
		Public Sub New(ByVal labelIndex As Integer, ByVal numPossibleLabels As Integer, ByVal regression As Boolean, ByVal preProcessor As DataSetPreProcessor, ByVal converter As WritableConverter)
			Me.New(labelIndex, labelIndex, numPossibleLabels, regression, preProcessor, converter)
		End Sub

		''' <summary>
		''' Main constructor, including for multi-label regression
		''' </summary>
		''' <param name="labelIndexFrom">    Index of the first target </param>
		''' <param name="labelIndexTo">      Index of the last target, inclusive (for classification or single-output regression: same as labelIndexFrom) </param>
		''' <param name="numPossibleLabels"> Unused for regression, or number of classes for classification </param>
		''' <param name="regression">        If true: regression. false: classification </param>
		Public Sub New(ByVal labelIndexFrom As Integer, ByVal labelIndexTo As Integer, ByVal numPossibleLabels As Integer, ByVal regression As Boolean, ByVal preProcessor As DataSetPreProcessor, ByVal converter As WritableConverter)
			Me.labelIndex = labelIndexFrom
			Me.labelIndexTo = labelIndexTo
			Me.numPossibleLabels = numPossibleLabels
			Me.regression = regression
			Me.preProcessor = preProcessor
			Me.converter = converter
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.DataSet call(java.util.List<org.datavec.api.writable.Writable> currList) throws Exception
		Public Overrides Function [call](ByVal currList As IList(Of Writable)) As DataSet

			'allow people to specify label index as -1 and infer the last possible label
			Dim labelIndex As Integer = Me.labelIndex
			If numPossibleLabels >= 1 AndAlso labelIndex < 0 Then
				labelIndex = currList.Count - 1
			End If

			Dim label As INDArray = Nothing
			Dim featureVector As INDArray = Nothing
			Dim featureCount As Integer = 0
			Dim labelCount As Integer = 0

			'no labels
			If currList.Count = 2 AndAlso TypeOf currList(1) Is NDArrayWritable AndAlso TypeOf currList(0) Is NDArrayWritable AndAlso currList(0) Is currList(1) Then
				Dim writable As NDArrayWritable = DirectCast(currList(0), NDArrayWritable)
				Dim ds As New DataSet(writable.get(), writable.get())
				If preProcessor IsNot Nothing Then
					preProcessor.preProcess(ds)
				End If
				Return ds
			End If
			If currList.Count = 2 AndAlso TypeOf currList(0) Is NDArrayWritable Then
				If Not regression Then
					label = FeatureUtil.toOutcomeVector(CInt(Math.Truncate(Double.Parse(currList(1).ToString()))), numPossibleLabels)
				Else
					label = Nd4j.scalar(Double.Parse(currList(1).ToString())).reshape(ChrW(1), 1)
				End If
				Dim ndArrayWritable As NDArrayWritable = DirectCast(currList(0), NDArrayWritable)
				featureVector = ndArrayWritable.get()
				Dim ds As New DataSet(featureVector, label)
				If preProcessor IsNot Nothing Then
					preProcessor.preProcess(ds)
				End If
				Return ds
			End If

			For j As Integer = 0 To currList.Count - 1
				Dim current As Writable = currList(j)
				'ndarray writable is an insane slow down here
				If Not (TypeOf current Is NDArrayWritable) AndAlso current.ToString().Length = 0 Then
					Continue For
				End If

				If labelIndex >= 0 AndAlso j >= labelIndex AndAlso j <= labelIndexTo Then
					'single label case (classification, single label regression etc)
					If converter IsNot Nothing Then
						Try
							current = converter.convert(current)
						Catch e As WritableConverterException

							log.error("",e)
						End Try
					End If
					If regression Then
						'single and multi-label regression
						If label Is Nothing Then
							label = Nd4j.zeros(1, labelIndexTo - labelIndex + 1)
						End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: label.putScalar(0, labelCount++, current.toDouble());
						label.putScalar(0, labelCount, current.toDouble())
							labelCount += 1
					Else
						If numPossibleLabels < 1 Then
							Throw New System.InvalidOperationException("Number of possible labels invalid, must be >= 1 for classification")
						End If
						Dim curr As Integer = current.toInt()
						If curr >= numPossibleLabels Then
							Throw New System.InvalidOperationException("Invalid index: got index " & curr & " but numPossibleLabels is " & numPossibleLabels & " (must be 0 <= idx < numPossibleLabels")
						End If
						label = FeatureUtil.toOutcomeVector(curr, numPossibleLabels)
					End If
				Else
					Try
						Dim value As Double = current.toDouble()
						If featureVector Is Nothing Then
							If regression AndAlso labelIndex >= 0 Then
								'Handle the possibly multi-label regression case here:
								Dim nLabels As Integer = labelIndexTo - labelIndex + 1
								featureVector = Nd4j.create(1, currList.Count - nLabels)
							Else
								'Classification case, and also no-labels case
								featureVector = Nd4j.create(1,If(labelIndex >= 0, currList.Count - 1, currList.Count))
							End If
						End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: featureVector.putScalar(featureCount++, value);
						featureVector.putScalar(featureCount, value)
							featureCount += 1
					Catch e As System.NotSupportedException
						' This isn't a scalar, so check if we got an array already
						If TypeOf current Is NDArrayWritable Then
							Preconditions.checkState(featureVector Is Nothing, "Already got an array")
							featureVector = DirectCast(current, NDArrayWritable).get()
						Else
							Throw e
						End If
					End Try
				End If
			Next j

			Dim ds As New DataSet(featureVector, (If(labelIndex >= 0, label, featureVector)))
			If preProcessor IsNot Nothing Then
				preProcessor.preProcess(ds)
			End If
			Return ds
		End Function
	End Class

End Namespace