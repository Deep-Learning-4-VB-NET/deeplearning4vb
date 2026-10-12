Imports System
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports EvaluationTools = org.deeplearning4j.core.evaluation.EvaluationTools
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports NormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.NormalizerStandardize
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions

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

Namespace org.deeplearning4j.eval

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.EVAL_METRICS) @Tag(TagNames.JACKSON_SERDE) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) public class EvaluationToolsTests extends org.deeplearning4j.BaseDL4JTest
	Public Class EvaluationToolsTests
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRocHtml()
		Public Overridable Sub testRocHtml()

			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(4).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder()).nIn(4).nOut(2).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim ns As New NormalizerStandardize()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			ns.fit(ds)
			ns.transform(ds)

			Dim newLabels As INDArray = Nd4j.create(150, 2)
			newLabels.getColumn(0).assign(ds.Labels.getColumn(0))
			newLabels.getColumn(0).addi(ds.Labels.getColumn(1))
			newLabels.getColumn(1).assign(ds.Labels.getColumn(2))
			ds.Labels = newLabels

			For i As Integer = 0 To 29
				net.fit(ds)
			Next i

			For Each numSteps As Integer In New Integer() {20, 0}
				Dim roc As New ROC(numSteps)
				iter.reset()

				Dim f As INDArray = ds.Features
				Dim l As INDArray = ds.Labels
				Dim [out] As INDArray = net.output(f)
				roc.eval(l, [out])


				Dim str As String = EvaluationTools.rocChartToHtml(roc)
				'            System.out.println(str);
			Next numSteps
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRocMultiToHtml() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRocMultiToHtml()
			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(4).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim ns As New NormalizerStandardize()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			ns.fit(ds)
			ns.transform(ds)

			For i As Integer = 0 To 29
				net.fit(ds)
			Next i

			For Each numSteps As Integer In New Integer() {20, 0}
				Dim roc As New ROCMultiClass(numSteps)
				iter.reset()

				Dim f As INDArray = ds.Features
				Dim l As INDArray = ds.Labels
				Dim [out] As INDArray = net.output(f)
				roc.eval(l, [out])


				Dim str As String = EvaluationTools.rocChartToHtml(roc, New List(Of String) From {"setosa", "versicolor", "virginica"})
	'            System.out.println(str);
			Next numSteps
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEvaluationCalibrationToHtml() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testEvaluationCalibrationToHtml()
			Dim minibatch As Integer = 1000
			Dim nClasses As Integer = 3

			Dim arr As INDArray = Nd4j.rand(minibatch, nClasses)
			arr.diviColumnVector(arr.sum(1))
			Dim labels As INDArray = Nd4j.zeros(minibatch, nClasses)
			Dim r As New Random(12345)
			For i As Integer = 0 To minibatch - 1
				labels.putScalar(i, r.Next(nClasses), 1.0)
			Next i

			Dim numBins As Integer = 10
			Dim ec As New EvaluationCalibration(numBins, numBins)
			ec.eval(labels, arr)

			Dim str As String = EvaluationTools.evaluationCalibrationToHtml(ec)
			'        System.out.println(str);
		End Sub

	End Class

End Namespace