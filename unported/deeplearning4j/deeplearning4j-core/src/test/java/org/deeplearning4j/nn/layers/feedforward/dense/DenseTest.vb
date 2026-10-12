Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports Evaluation = org.deeplearning4j.eval.Evaluation
Imports Layer = org.deeplearning4j.nn.api.Layer
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
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.deeplearning4j.nn.layers.feedforward.dense

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Dense Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class DenseTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class DenseTest
		Inherits BaseDL4JTest

		Private numSamples As Integer = 150

		Private batchSize As Integer = 150

		Private iter As DataSetIterator = New IrisDataSetIterator(batchSize, numSamples)

		Private data As DataSet

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Dense Bias Init") void testDenseBiasInit()
		Friend Overridable Sub testDenseBiasInit()
			Dim build As DenseLayer = (New DenseLayer.Builder()).nIn(1).nOut(3).biasInit(1).build()
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer(build).build()
			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim layer As Layer = conf.getLayer().instantiate(conf, Nothing, 0, params, True, Nd4j.defaultFloatingPointType())
			assertEquals(1, layer.getParam("b").size(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test MLP Multi Layer Pretrain") void testMLPMultiLayerPretrain()
		Friend Overridable Sub testMLPMultiLayerPretrain()
			' Note CNN does not do pretrain
			Dim model As MultiLayerNetwork = getDenseMLNConfig(False, True)
			model.fit(iter)
			Dim model2 As MultiLayerNetwork = getDenseMLNConfig(False, True)
			model2.fit(iter)
			iter.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim test As DataSet = iter.next()
			assertEquals(model.params(), model2.params())
			Dim eval As New Evaluation()
			Dim output As INDArray = model.output(test.Features)
			eval.eval(test.Labels, output)
			Dim f1Score As Double = eval.f1()
			Dim eval2 As New Evaluation()
			Dim output2 As INDArray = model2.output(test.Features)
			eval2.eval(test.Labels, output2)
			Dim f1Score2 As Double = eval2.f1()
			assertEquals(f1Score, f1Score2, 1e-4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test MLP Multi Layer Backprop") void testMLPMultiLayerBackprop()
		Friend Overridable Sub testMLPMultiLayerBackprop()
			Dim model As MultiLayerNetwork = getDenseMLNConfig(True, False)
			model.fit(iter)
			Dim model2 As MultiLayerNetwork = getDenseMLNConfig(True, False)
			model2.fit(iter)
			iter.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim test As DataSet = iter.next()
			assertEquals(model.params(), model2.params())
			Dim eval As New Evaluation()
			Dim output As INDArray = model.output(test.Features)
			eval.eval(test.Labels, output)
			Dim f1Score As Double = eval.f1()
			Dim eval2 As New Evaluation()
			Dim output2 As INDArray = model2.output(test.Features)
			eval2.eval(test.Labels, output2)
			Dim f1Score2 As Double = eval2.f1()
			assertEquals(f1Score, f1Score2, 1e-4)
		End Sub

		' ////////////////////////////////////////////////////////////////////////////////
		Private Shared Function getDenseMLNConfig(ByVal backprop As Boolean, ByVal pretrain As Boolean) As MultiLayerNetwork
			Dim numInputs As Integer = 4
			Dim outputNum As Integer = 3
			Dim seed As Long = 6
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(seed).updater(New Sgd(1e-3)).l1(0.3).l2(1e-3).list().layer(0, (New DenseLayer.Builder()).nIn(numInputs).nOut(3).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build()).layer(1, (New DenseLayer.Builder()).nIn(3).nOut(2).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).nIn(2).nOut(outputNum).activation(Activation.SOFTMAX).build()).build()
			Dim model As New MultiLayerNetwork(conf)
			model.init()
			Return model
		End Function
	End Class

End Namespace