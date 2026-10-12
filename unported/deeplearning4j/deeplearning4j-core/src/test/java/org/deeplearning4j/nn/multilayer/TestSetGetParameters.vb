Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports org.deeplearning4j.nn.conf.layers
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction
Imports org.junit.jupiter.api.Assertions

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

Namespace org.deeplearning4j.nn.multilayer


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestSetGetParameters extends org.deeplearning4j.BaseDL4JTest
	Public Class TestSetGetParameters
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSetParameters()
		Public Overridable Sub testSetParameters()
			'Set up a MLN, then do set(get) on parameters. Results should be identical compared to before doing this.
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New DenseLayer.Builder()).nIn(9).nOut(10).dist(New NormalDistribution(0, 1)).build()).layer(1, (New DenseLayer.Builder()).nIn(10).nOut(11).dist(New NormalDistribution(0, 1)).build()).layer(2, (New AutoEncoder.Builder()).corruptionLevel(0.5).nIn(11).nOut(12).dist(New NormalDistribution(0, 1)).build()).layer(3, (New OutputLayer.Builder(LossFunction.MSE)).nIn(12).nOut(12).dist(New NormalDistribution(0, 1)).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim initParams As INDArray = net.params().dup()
			Dim initParams2 As IDictionary(Of String, INDArray) = net.paramTable()

			net.Params = net.params()

			Dim initParamsAfter As INDArray = net.params()
			Dim initParams2After As IDictionary(Of String, INDArray) = net.paramTable()

			For Each s As String In initParams2.Keys
				assertTrue(initParams2(s).Equals(initParams2After(s)),"Params differ: " & s)
			Next s

			assertEquals(initParams, initParamsAfter)

			'Now, try the other way: get(set(random))
			Dim randomParams As INDArray = Nd4j.rand(initParams.dataType(), initParams.shape())
			net.Params = randomParams.dup()

			assertEquals(net.params(), randomParams)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSetParametersRNN()
		Public Overridable Sub testSetParametersRNN()
			'Set up a MLN, then do set(get) on parameters. Results should be identical compared to before doing this.

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New GravesLSTM.Builder()).nIn(9).nOut(10).dist(New NormalDistribution(0, 1)).build()).layer(1, (New GravesLSTM.Builder()).nIn(10).nOut(11).dist(New NormalDistribution(0, 1)).build()).layer(2, (New RnnOutputLayer.Builder(LossFunction.MSE)).dist(New NormalDistribution(0, 1)).nIn(11).nOut(12).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim initParams As INDArray = net.params().dup()
			Dim initParams2 As IDictionary(Of String, INDArray) = net.paramTable()

			net.Params = net.params()

			Dim initParamsAfter As INDArray = net.params()
			Dim initParams2After As IDictionary(Of String, INDArray) = net.paramTable()

			For Each s As String In initParams2.Keys
				assertTrue(initParams2(s).Equals(initParams2After(s)),"Params differ: " & s)
			Next s

			assertEquals(initParams, initParamsAfter)

			'Now, try the other way: get(set(random))
			Dim randomParams As INDArray = Nd4j.rand(initParams.dataType(), initParams.shape())
			net.Params = randomParams.dup()

			assertEquals(net.params(), randomParams)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInitWithParams()
		Public Overridable Sub testInitWithParams()

			Nd4j.Random.setSeed(12345)

			'Create configuration. Doesn't matter if this doesn't actually work for forward/backward pass here
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(0, (New ConvolutionLayer.Builder()).nIn(10).nOut(10).kernelSize(2, 2).stride(2, 2).padding(2, 2).build()).layer(1, (New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer(2, (New GravesLSTM.Builder()).nIn(10).nOut(10).build()).layer(3, (New GravesBidirectionalLSTM.Builder()).nIn(10).nOut(10).build()).layer(4, (New OutputLayer.Builder(LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim params As INDArray = net.params()


			Dim net2 As New MultiLayerNetwork(conf)
			net2.init(params, True)

			Dim net3 As New MultiLayerNetwork(conf)
			net3.init(params, False)

			assertEquals(params, net2.params())
			assertEquals(params, net3.params())

			assertFalse(params Is net2.params()) 'Different objects due to clone
			assertTrue(params Is net3.params()) 'Same object due to clone


			Dim paramsMap As IDictionary(Of String, INDArray) = net.paramTable()
			Dim paramsMap2 As IDictionary(Of String, INDArray) = net2.paramTable()
			Dim paramsMap3 As IDictionary(Of String, INDArray) = net3.paramTable()
			For Each s As String In paramsMap.Keys
				assertEquals(paramsMap(s), paramsMap2(s))
				assertEquals(paramsMap(s), paramsMap3(s))
			Next s
		End Sub
	End Class

End Namespace