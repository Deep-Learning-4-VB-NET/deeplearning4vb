Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports org.deeplearning4j.nn.conf
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Test = org.junit.jupiter.api.Test
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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
Namespace org.deeplearning4j.nn.layers

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Cache Mode Test") class CacheModeTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class CacheModeTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Conv Cache Mode Simple") void testConvCacheModeSimple()
		Friend Overridable Sub testConvCacheModeSimple()
			Dim conf1 As MultiLayerConfiguration = getConf(CacheMode.NONE)
			Dim conf2 As MultiLayerConfiguration = getConf(CacheMode.DEVICE)
			Dim net1 As New MultiLayerNetwork(conf1)
			net1.init()
			Dim net2 As New MultiLayerNetwork(conf2)
			net2.init()
			Dim [in] As INDArray = Nd4j.rand(3, 28 * 28)
			Dim labels As INDArray = TestUtils.randomOneHot(3, 10)
			Dim out1 As INDArray = net1.output([in])
			Dim out2 As INDArray = net2.output([in])
			assertEquals(out1, out2)
			assertEquals(net1.params(), net2.params())
			net1.fit([in], labels)
			net2.fit([in], labels)
			assertEquals(net1.params(), net2.params())
		End Sub

		Private Shared Function getConf(ByVal cacheMode As CacheMode) As MultiLayerConfiguration
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).inferenceWorkspaceMode(WorkspaceMode.ENABLED).trainingWorkspaceMode(WorkspaceMode.ENABLED).seed(12345).cacheMode(cacheMode).list().layer((New ConvolutionLayer.Builder()).nOut(3).build()).layer((New ConvolutionLayer.Builder()).nOut(3).build()).layer((New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()
			Return conf
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test LSTM Cache Mode Simple") void testLSTMCacheModeSimple()
		Friend Overridable Sub testLSTMCacheModeSimple()
			For Each graves As Boolean In New Boolean() { True, False }
				Dim conf1 As MultiLayerConfiguration = getConfLSTM(CacheMode.NONE, graves)
				Dim conf2 As MultiLayerConfiguration = getConfLSTM(CacheMode.DEVICE, graves)
				Dim net1 As New MultiLayerNetwork(conf1)
				net1.init()
				Dim net2 As New MultiLayerNetwork(conf2)
				net2.init()
				Dim [in] As INDArray = Nd4j.rand(New Integer() { 3, 3, 10 })
				Dim labels As INDArray = TestUtils.randomOneHotTimeSeries(3, 10, 10)
				Dim out1 As INDArray = net1.output([in])
				Dim out2 As INDArray = net2.output([in])
				assertEquals(out1, out2)
				assertEquals(net1.params(), net2.params())
				net1.fit([in], labels)
				net2.fit([in], labels)
				assertEquals(net1.params(), net2.params())
			Next graves
		End Sub

		Private Shared Function getConfLSTM(ByVal cacheMode As CacheMode, ByVal graves As Boolean) As MultiLayerConfiguration
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).inferenceWorkspaceMode(WorkspaceMode.ENABLED).trainingWorkspaceMode(WorkspaceMode.ENABLED).seed(12345).cacheMode(cacheMode).list().layer(If(graves, (New GravesLSTM.Builder()).nIn(3).nOut(3).build(), (New LSTM.Builder()).nIn(3).nOut(3).build())).layer(If(graves, (New GravesLSTM.Builder()).nIn(3).nOut(3).build(), (New LSTM.Builder()).nIn(3).nOut(3).build())).layer((New RnnOutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).build()).build()
			Return conf
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Conv Cache Mode Simple CG") void testConvCacheModeSimpleCG()
		Friend Overridable Sub testConvCacheModeSimpleCG()
			Dim conf1 As ComputationGraphConfiguration = getConfCG(CacheMode.NONE)
			Dim conf2 As ComputationGraphConfiguration = getConfCG(CacheMode.DEVICE)
			Dim net1 As New ComputationGraph(conf1)
			net1.init()
			Dim net2 As New ComputationGraph(conf2)
			net2.init()
			Dim [in] As INDArray = Nd4j.rand(3, 28 * 28)
			Dim labels As INDArray = TestUtils.randomOneHot(3, 10)
			Dim out1 As INDArray = net1.outputSingle([in])
			Dim out2 As INDArray = net2.outputSingle([in])
			assertEquals(out1, out2)
			assertEquals(net1.params(), net2.params())
			net1.fit(New DataSet([in], labels))
			net2.fit(New DataSet([in], labels))
			assertEquals(net1.params(), net2.params())
		End Sub

		Private Shared Function getConfCG(ByVal cacheMode As CacheMode) As ComputationGraphConfiguration
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).inferenceWorkspaceMode(WorkspaceMode.ENABLED).trainingWorkspaceMode(WorkspaceMode.ENABLED).seed(12345).cacheMode(cacheMode).graphBuilder().addInputs("in").layer("0", (New ConvolutionLayer.Builder()).nOut(3).build(), "in").layer("1", (New ConvolutionLayer.Builder()).nOut(3).build(), "0").layer("2", (New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).build(), "1").setOutputs("2").setInputTypes(InputType.convolutionalFlat(28, 28, 1)).build()
			Return conf
		End Function
	End Class

End Namespace