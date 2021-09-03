Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports TrainingConfig = org.nd4j.autodiff.samediff.TrainingConfig
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports RegressionEvaluation = org.nd4j.evaluation.regression.RegressionEvaluation
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports NormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.NormalizerStandardize
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.linalg.learning.config
Imports L1Regularization = org.nd4j.linalg.learning.regularization.L1Regularization
Imports L2Regularization = org.nd4j.linalg.learning.regularization.L2Regularization
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
Imports WeightDecay = org.nd4j.linalg.learning.regularization.WeightDecay
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports XavierInitScheme = org.nd4j.weightinit.impl.XavierInitScheme
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.fail

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

Namespace org.deeplearning4j.samediff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.SAMEDIFF) public class CompareTrainingImplementations extends org.deeplearning4j.BaseDL4JTest
	Public Class CompareTrainingImplementations
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCompareMlpTrainingIris()
		Public Overridable Sub testCompareMlpTrainingIris()
			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
			Dim std As New NormalizerStandardize()
			std.fit(iter)
			iter.PreProcessor = std

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			Dim f As INDArray = ds.Features
			Dim l As INDArray = ds.Labels

			Dim l1() As Double = {0.0, 0.0, 0.01, 0.01, 0.0}
			Dim l2() As Double = {0.0, 0.02, 0.00, 0.02, 0.0}
			Dim wd() As Double = {0.0, 0.0, 0.0, 0.0, 0.03}

			For Each u As String In New String(){"sgd", "adam", "nesterov", "adamax", "amsgrad"}
				For i As Integer = 0 To l1.Length - 1
					Nd4j.Random.setSeed(12345)
					Dim l1Val As Double = l1(i)
					Dim l2Val As Double = l2(i)
					Dim wdVal As Double = wd(i)

					Dim testName As String = u & ", l1=" & l1Val & ", l2=" & l2Val & ", wd=" & wdVal

					log.info("Starting: {}", testName)
					Dim sd As SameDiff = SameDiff.create()

					Dim [in] As SDVariable = sd.placeHolder("input", DataType.DOUBLE, -1, 4)
					Dim label As SDVariable = sd.placeHolder("label", DataType.DOUBLE, -1, 3)

					Dim w0 As SDVariable = sd.var("w0", New XavierInitScheme("c"c, 4, 10), DataType.DOUBLE, 4, 10)
					Dim b0 As SDVariable = sd.var("b0", Nd4j.create(DataType.DOUBLE, 1, 10))

					Dim w1 As SDVariable = sd.var("w1", New XavierInitScheme("c"c, 10, 3), DataType.DOUBLE, 10, 3)
					Dim b1 As SDVariable = sd.var("b1", Nd4j.create(DataType.DOUBLE, 1, 3))

					Dim z0 As SDVariable = [in].mmul(w0).add(b0)
					Dim a0 As SDVariable = sd.nn().tanh(z0)
					Dim z1 As SDVariable = a0.mmul(w1).add("prediction", b1)
					Dim a1 As SDVariable = sd.nn().softmax("softmax", z1)

					Dim diff As SDVariable = sd.math().squaredDifference(a1, label)
					Dim lossMse As SDVariable = diff.mean()
					lossMse.markAsLoss()

					Dim updater As IUpdater
					Dim lr As Double
					Select Case u
						Case "sgd"
							lr = 3e-1
							updater = New Sgd(lr)
						Case "adam"
							lr = 1e-2
							updater = New Adam(lr)
						Case "nesterov"
							lr = 1e-1
							updater = New Nesterovs(lr)
						Case "adamax"
							lr = 1e-2
							updater = New AdaMax(lr)
						Case "amsgrad"
							lr = 1e-2
							updater = New AMSGrad(lr)
						Case Else
							Throw New Exception()
					End Select

					Dim r As IList(Of Regularization) = New List(Of Regularization)()
					If l2Val > 0 Then
						r.Add(New L2Regularization(l2Val))
					End If
					If l1Val > 0 Then
						r.Add(New L1Regularization(l1Val))
					End If
					If wdVal > 0 Then
						r.Add(New WeightDecay(wdVal, True))
					End If
					Dim conf As TrainingConfig = (New TrainingConfig.Builder()).updater(updater).regularization(r).dataSetFeatureMapping("input").dataSetLabelMapping("label").build()
					sd.TrainingConfig = conf


					'Create equivalent DL4J net
					Dim mlc As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).weightInit(WeightInit.XAVIER).seed(12345).l1(l1Val).l2(l2Val).l1Bias(l1Val).l2Bias(l2Val).weightDecay(wdVal, True).weightDecayBias(wdVal, True).updater(New Sgd(1.0)).list().layer((New DenseLayer.Builder()).nIn(4).nOut(10).activation(Activation.TANH).build()).layer((New OutputLayer.Builder()).nIn(10).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MSE).build()).build()

					Dim net As New MultiLayerNetwork(mlc)
					net.init()
					Dim oldParams As IDictionary(Of String, INDArray) = net.paramTable()

					'Assign parameters so we have identical models at the start:
					w0.Arr.assign(net.getParam("0_W"))
					b0.Arr.assign(net.getParam("0_b"))
					w1.Arr.assign(net.getParam("1_W"))
					b1.Arr.assign(net.getParam("1_b"))

					'Check output (forward pass)
					Dim placeholders As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
					placeholders("input") = f
					placeholders("label") = l
					Dim map As IDictionary(Of String, INDArray) = sd.output(placeholders, lossMse.name(), a1.name())
					Dim outSd As INDArray = map(a1.name())
					Dim outDl4j As INDArray = net.output(f)

					assertEquals(outDl4j, outSd, testName)

					net.Input = f
					net.Labels = l
					net.computeGradientAndScore()
					net.Updater.update(net, net.gradient(), 0, 0, 150, LayerWorkspaceMgr.noWorkspacesImmutable()) 'Division by minibatch, apply L1/L2



					'Check score
					Dim scoreDl4j As Double = net.score()
					Dim scoreSd As Double = map(lossMse.name()).getDouble(0) + sd.calcRegularizationScore()
					assertEquals(scoreDl4j, scoreSd, 1e-6,testName)

					Dim lossRegScoreSD As Double = sd.calcRegularizationScore()
					Dim lossRegScoreDL4J As Double = net.calcRegularizationScore(True)

					assertEquals(lossRegScoreDL4J, lossRegScoreSD, 1e-6)

					'Check gradients (before updater applied)
					Dim grads As IDictionary(Of String, INDArray) = net.gradient().gradientForVariable()
					Dim gm As IDictionary(Of String, INDArray) = sd.calculateGradients(placeholders, b1.name(), w1.name(), b0.name(), w0.name())

					'Note that the SameDiff gradients don't include the L1/L2 terms at present just from execBackwards()... these are added in fitting only
					'We can check correctness though with training param checks later
					If l1Val = 0 AndAlso l2Val = 0 AndAlso wdVal = 0 Then
						assertEquals(grads("1_b"), gm(b1.name()), testName)
						assertEquals(grads("1_W"), gm(w1.name()), testName)
						assertEquals(grads("0_b"), gm(b0.name()), testName)
						assertEquals(grads("0_W"), gm(w0.name()), testName)
					End If


					'Check training with updater
					mlc = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).weightInit(WeightInit.XAVIER).seed(12345).l1(l1Val).l2(l2Val).l1Bias(l1Val).l2Bias(l2Val).weightDecay(wdVal, True).weightDecayBias(wdVal, True).updater(updater.clone()).list().layer((New DenseLayer.Builder()).nIn(4).nOut(10).activation(Activation.TANH).build()).layer((New OutputLayer.Builder()).nIn(10).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MSE).build()).build()
					net = New MultiLayerNetwork(mlc)
					net.init()
					net.ParamTable = oldParams

					For j As Integer = 0 To 2
						net.fit(ds)
						sd.fit(ds)

						Dim s As String = testName & " - " & j
						Dim dl4j_0W As INDArray = net.getParam("0_W")
						Dim sd_0W As INDArray = w0.Arr
						assertEquals(dl4j_0W, sd_0W, s)
						assertEquals(net.getParam("0_b"), b0.Arr, s)
						assertEquals(net.getParam("1_W"), w1.Arr, s)
						assertEquals(net.getParam("1_b"), b1.Arr, s)
					Next j

					'Compare evaluations
					Dim evalDl4j As Evaluation = net.doEvaluation(iter, New Evaluation())(0)
					Dim evalSd As New Evaluation()
					sd.evaluate(iter, "softmax", evalSd)
					assertEquals(evalDl4j, evalSd)

					Dim rEvalDl4j As RegressionEvaluation = net.doEvaluation(iter, New RegressionEvaluation())(0)
					Dim rEvalSd As New RegressionEvaluation()
					sd.evaluate(iter, "softmax", rEvalSd)
					assertEquals(rEvalDl4j, rEvalSd)

	'                System.out.println("---------------------------------");
				Next i
			Next u
		End Sub
	End Class

End Namespace