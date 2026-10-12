Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports EmbeddingLayer = org.deeplearning4j.nn.conf.layers.EmbeddingLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertArrayEquals
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
Namespace org.deeplearning4j.nn.misc

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @DisplayName("Large Net Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.FILE_IO) @Tag(TagNames.WORKSPACES) class LargeNetTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class LargeNetTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Test @DisplayName("Test Large Multi Layer Network") void testLargeMultiLayerNetwork()
		Friend Overridable Sub testLargeMultiLayerNetwork()
			Nd4j.DataType = DataType.FLOAT
			' More than 2.1 billion parameters
			' 10M classes plus 300 vector size -> 3 billion elements
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New EmbeddingLayer.Builder()).nIn(10_000_000).nOut(300).build()).layer((New OutputLayer.Builder()).nIn(300).nOut(10).activation(Activation.SOFTMAX).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim params As INDArray = net.params()
			Dim paramsLength As Long = params.length()
			Dim expParamsLength As Long = 10_000_000L * 300 + 300 * 10 + 10
			assertEquals(expParamsLength, paramsLength)
			Dim expW() As Long = { 10_000_000, 300 }
			assertArrayEquals(expW, net.getParam("0_W").shape())
			Dim expW1() As Long = { 300, 10 }
			assertArrayEquals(expW1, net.getParam("1_W").shape())
			Dim expB1() As Long = { 1, 10 }
			assertArrayEquals(expB1, net.getParam("1_b").shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Test @DisplayName("Test Large Comp Graph") void testLargeCompGraph()
		Friend Overridable Sub testLargeCompGraph()
			Nd4j.DataType = DataType.FLOAT
			' More than 2.1 billion parameters
			' 10M classes plus 300 vector size -> 3 billion elements
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("0", (New EmbeddingLayer.Builder()).nIn(10_000_000).nOut(300).build(), "in").layer("1", (New OutputLayer.Builder()).nIn(300).nOut(10).activation(Activation.SOFTMAX).build(), "0").setOutputs("1").build()
			Dim net As New ComputationGraph(conf)
			net.init()
			Dim params As INDArray = net.params()
			Dim paramsLength As Long = params.length()
			Dim expParamsLength As Long = 10_000_000L * 300 + 300 * 10 + 10
			assertEquals(expParamsLength, paramsLength)
			Dim expW() As Long = { 10_000_000, 300 }
			assertArrayEquals(expW, net.getParam("0_W").shape())
			Dim expW1() As Long = { 300, 10 }
			assertArrayEquals(expW1, net.getParam("1_W").shape())
			Dim expB1() As Long = { 1, 10 }
			assertArrayEquals(expB1, net.getParam("1_b").shape())
		End Sub
	End Class

End Namespace