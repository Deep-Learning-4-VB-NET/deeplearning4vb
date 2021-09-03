Imports System.Collections.Generic
Imports System.Threading
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Model = org.deeplearning4j.nn.api.Model
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports WorkspaceMode = org.deeplearning4j.nn.conf.WorkspaceMode
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports SubsamplingLayer = org.deeplearning4j.nn.conf.layers.SubsamplingLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Execution = org.junit.jupiter.api.parallel.Execution
Imports ExecutionMode = org.junit.jupiter.api.parallel.ExecutionMode
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.deeplearning4j.parallelism


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.RNG) public class RandomTests extends org.deeplearning4j.BaseDL4JTest
	Public Class RandomTests
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) @Execution(org.junit.jupiter.api.parallel.ExecutionMode.SAME_THREAD) @Test public void testModelInitialParamsEquality1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testModelInitialParamsEquality1()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<org.deeplearning4j.nn.api.Model> models = new java.util.concurrent.CopyOnWriteArrayList<>();
			Dim models As IList(Of Model) = New CopyOnWriteArrayList(Of Model)()

			For i As Integer = 0 To 3
				Dim thread As New Thread(Sub()
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(119).l2(0.0005).weightInit(WeightInit.XAVIER).updater(New Nesterovs(0.01, 0.9)).trainingWorkspaceMode(WorkspaceMode.ENABLED).list().layer(0, (New ConvolutionLayer.Builder(5, 5)).nIn(1).stride(1, 1).nOut(20).activation(Activation.IDENTITY).build()).layer(1, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build()).layer(2, (New ConvolutionLayer.Builder(5, 5)).stride(1, 1).nOut(50).activation(Activation.IDENTITY).build()).layer(3, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build()).layer(4, (New DenseLayer.Builder()).activation(Activation.RELU).nOut(500).build()).layer(5, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(10).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()
				Dim network As New MultiLayerNetwork(conf)
				network.init()
				models.Add(network)
				End Sub)

				thread.Start()
				thread.Join()
			Next i


			' at the end of day, model params has to
			For i As Integer = 0 To models.Count - 1
				assertEquals(models(0).params(), models(i).params())
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRngInitMLN()
		Public Overridable Sub testRngInitMLN()
			Nd4j.Random.setSeed(12345)

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).activation(Activation.TANH).weightInit(WeightInit.XAVIER).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer(1, (New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build()).build()

			Dim json As String = conf.toJson()

			Dim net1 As New MultiLayerNetwork(conf)
			net1.init()

			Dim net2 As New MultiLayerNetwork(conf)
			net2.init()

			assertEquals(net1.params(), net2.params())

			Dim fromJson As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json)

			Nd4j.Random.setSeed(987654321)
			Dim net3 As New MultiLayerNetwork(fromJson)
			net3.init()

			assertEquals(net1.params(), net3.params())
		End Sub
	End Class

End Namespace