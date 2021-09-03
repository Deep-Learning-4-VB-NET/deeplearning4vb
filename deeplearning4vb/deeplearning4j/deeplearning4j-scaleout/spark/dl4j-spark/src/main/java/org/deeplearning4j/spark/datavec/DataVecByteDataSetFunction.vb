Imports System.Collections.Generic
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BytesWritable = org.apache.hadoop.io.BytesWritable
Imports Text = org.apache.hadoop.io.Text
Imports PairFunction = org.apache.spark.api.java.function.PairFunction
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DataVecByteDataSetFunction implements org.apache.spark.api.java.function.PairFunction<scala.Tuple2<org.apache.hadoop.io.Text, org.apache.hadoop.io.BytesWritable>, Double, org.nd4j.linalg.dataset.DataSet>
	Public Class DataVecByteDataSetFunction
		Implements PairFunction(Of Tuple2(Of Text, BytesWritable), Double, DataSet)

		Private labelIndex As Integer = 0
		Private numPossibleLabels As Integer
		Private byteFileLen As Integer
		Private batchSize As Integer
		Private numExamples As Integer
		Private regression As Boolean = False
		Private preProcessor As DataSetPreProcessor

		Public Sub New(ByVal labelIndex As Integer, ByVal numPossibleLabels As Integer, ByVal batchSize As Integer, ByVal byteFileLen As Integer)
			Me.New(labelIndex, numPossibleLabels, batchSize, byteFileLen, False, Nothing)
		End Sub

		Public Sub New(ByVal labelIndex As Integer, ByVal numPossibleLabels As Integer, ByVal batchSize As Integer, ByVal byteFileLen As Integer, ByVal regression As Boolean)
			Me.New(labelIndex, numPossibleLabels, batchSize, byteFileLen, regression, Nothing)
		End Sub

		''' <param name="labelIndex"> Index of the label column </param>
		''' <param name="numPossibleLabels"> Number of classes for classification  (not used if regression = true) </param>
		''' <param name="batchSize"> size of examples in DataSet. Pass in total examples if including all </param>
		''' <param name="byteFileLen"> number of bytes per individual file </param>
		''' <param name="regression"> False for classification, true for regression </param>
		''' <param name="preProcessor"> DataSetPreprocessor (may be null) </param>
		Public Sub New(ByVal labelIndex As Integer, ByVal numPossibleLabels As Integer, ByVal batchSize As Integer, ByVal byteFileLen As Integer, ByVal regression As Boolean, ByVal preProcessor As DataSetPreProcessor)
			Me.labelIndex = labelIndex
			Me.numPossibleLabels = numPossibleLabels
			Me.batchSize = batchSize
			Me.byteFileLen = byteFileLen
			Me.regression = regression
			Me.preProcessor = preProcessor

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public scala.Tuple2<Double, org.nd4j.linalg.dataset.DataSet> call(scala.Tuple2<org.apache.hadoop.io.Text, org.apache.hadoop.io.BytesWritable> inputTuple) throws Exception
		Public Overrides Function [call](ByVal inputTuple As Tuple2(Of Text, BytesWritable)) As Tuple2(Of Double, DataSet)
			Dim lenFeatureVector As Integer = 0

			If numPossibleLabels >= 1 Then
				lenFeatureVector = byteFileLen - 1
				If labelIndex < 0 Then
					labelIndex = byteFileLen - 1
				End If
			End If

			Dim inputStream As Stream = New DataInputStream(New MemoryStream(inputTuple._2().getBytes()))

			Dim batchNumCount As Integer = 0
			Dim byteFeature(byteFileLen - 1) As SByte
			Dim dataSets As IList(Of DataSet) = New List(Of DataSet)()
			Dim label As INDArray
			Dim featureCount As Integer

			Try
				Dim featureVector As INDArray = Nd4j.create(lenFeatureVector)
				Do While (inputStream.Read(byteFeature, 0, byteFeature.Length)) <> -1 AndAlso batchNumCount <> batchSize
					featureCount = 0
					label = FeatureUtil.toOutcomeVector(byteFeature(labelIndex), numPossibleLabels)
					Dim j As Integer = 1
					Do While j <= featureVector.length()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: featureVector.putScalar(featureCount++, byteFeature[j]);
						featureVector.putScalar(featureCount, byteFeature(j))
							featureCount += 1
						j += 1
					Loop
					dataSets.Add(New DataSet(featureVector, label))
					batchNumCount += 1
					byteFeature = New SByte(byteFileLen - 1){}
					featureVector = Nd4j.create(lenFeatureVector)
				Loop
			Catch e As IOException
				log.error("",e)
			End Try

			Dim inputs As IList(Of INDArray) = New List(Of INDArray)()
			Dim labels As IList(Of INDArray) = New List(Of INDArray)()

			For Each data As DataSet In dataSets
				inputs.Add(data.Features)
				labels.Add(data.Labels)
			Next data

			Dim ds As New DataSet(Nd4j.vstack(CType(inputs, List(Of INDArray)).ToArray()), Nd4j.vstack(CType(labels, List(Of INDArray)).ToArray()))
			If preProcessor IsNot Nothing Then
				preProcessor.preProcess(ds)
			End If
			Return New Tuple2(Of Double, DataSet)(CDbl(batchNumCount), ds)

		End Function

	End Class

End Namespace