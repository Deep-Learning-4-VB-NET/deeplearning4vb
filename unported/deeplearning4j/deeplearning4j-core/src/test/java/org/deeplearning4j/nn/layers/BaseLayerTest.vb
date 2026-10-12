Imports System.Collections.Generic
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotEquals
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
'ORIGINAL LINE: @DisplayName("Base Layer Test") @NativeTag @Tag(TagNames.CUSTOM_FUNCTIONALITY) @Tag(TagNames.DL4J_OLD_API) class BaseLayerTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class BaseLayerTest
		Inherits BaseDL4JTest

		Protected Friend weight As INDArray = Nd4j.create(New Double() { 0.10, -0.20, -0.15, 0.05 }, New Integer() { 2, 2 })

		Protected Friend bias As INDArray = Nd4j.create(New Double() { 0.5, 0.5 }, New Integer() { 1, 2 })

		Protected Friend paramTable As IDictionary(Of String, INDArray)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach void doBefore()
		Friend Overridable Sub doBefore()
			paramTable = New Dictionary(Of String, INDArray)()
			paramTable("W") = weight
			paramTable("b") = bias
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Set Existing Params Convolution Single Layer") void testSetExistingParamsConvolutionSingleLayer()
		Friend Overridable Sub testSetExistingParamsConvolutionSingleLayer()
			Dim layer As Layer = configureSingleLayer()
			assertNotEquals(paramTable, layer.paramTable())
			layer.ParamTable = paramTable
			assertEquals(paramTable, layer.paramTable())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Set Existing Params Dense Multi Layer") void testSetExistingParamsDenseMultiLayer()
		Friend Overridable Sub testSetExistingParamsDenseMultiLayer()
			Dim net As MultiLayerNetwork = configureMultiLayer()
			For Each layer As Layer In net.Layers
				assertNotEquals(paramTable, layer.paramTable())
				layer.ParamTable = paramTable
				assertEquals(paramTable, layer.paramTable())
			Next layer
		End Sub

		Public Overridable Function configureSingleLayer() As Layer
			Dim nIn As Integer = 2
			Dim nOut As Integer = 2
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New ConvolutionLayer.Builder()).nIn(nIn).nOut(nOut).build()).build()
			Dim numParams As val = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Return conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType())
		End Function

		Public Overridable Function configureMultiLayer() As MultiLayerNetwork
			Dim nIn As Integer = 2
			Dim nOut As Integer = 2
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(nOut).build()).layer(1, (New OutputLayer.Builder()).nIn(nIn).nOut(nOut).activation(Activation.SOFTMAX).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Return net
		End Function
	End Class

End Namespace