Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
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

Namespace org.deeplearning4j.nn.graph


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestSetGetParameters extends org.deeplearning4j.BaseDL4JTest
	Public Class TestSetGetParameters
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInitWithParamsCG()
		Public Overridable Sub testInitWithParamsCG()

			Nd4j.Random.setSeed(12345)

			'Create configuration. Doesn't matter if this doesn't actually work for forward/backward pass here
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "in").addLayer("1", (New GravesLSTM.Builder()).nIn(10).nOut(10).build(), "in").addLayer("2", (New GravesBidirectionalLSTM.Builder()).nIn(10).nOut(10).build(), "in").addLayer("3", (New ConvolutionLayer.Builder()).nIn(10).nOut(10).kernelSize(2, 2).stride(2, 2).padding(2, 2).build(), "in").addLayer("4", (New OutputLayer.Builder(LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build(), "3").addLayer("5", (New OutputLayer.Builder(LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build(), "0").addLayer("6", (New RnnOutputLayer.Builder(LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build(), "1", "2").setOutputs("4", "5", "6").build()

			Dim net As New ComputationGraph(conf)
			net.init()
			Dim params As INDArray = net.params()


			Dim net2 As New ComputationGraph(conf)
			net2.init(params, True)

			Dim net3 As New ComputationGraph(conf)
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