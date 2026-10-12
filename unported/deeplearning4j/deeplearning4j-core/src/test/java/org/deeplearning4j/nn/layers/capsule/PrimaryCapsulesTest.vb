import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertFalse
import static org.junit.jupiter.api.Assertions.assertTrue
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports PrimaryCapsules = org.deeplearning4j.nn.conf.layers.PrimaryCapsules
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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
Namespace org.deeplearning4j.nn.layers.capsule

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Primary Capsules Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class PrimaryCapsulesTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class PrimaryCapsulesTest
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Output Type") void testOutputType()
		Friend Overridable Sub testOutputType()
			Dim layer As PrimaryCapsules = (New PrimaryCapsules.Builder(8, 8)).kernelSize(7, 7).stride(2, 2).build()
			Dim in1 As InputType = InputType.convolutional(7, 7, 16)
			assertEquals(InputType.recurrent(8, 8), layer.getOutputType(0, in1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Input Type") void testInputType()
		Friend Overridable Sub testInputType()
			Dim layer As PrimaryCapsules = (New PrimaryCapsules.Builder(8, 8)).kernelSize(7, 7).stride(2, 2).build()
			Dim in1 As InputType = InputType.convolutional(7, 7, 16)
			layer.setNIn(in1, True)
			assertEquals(8, layer.getCapsules())
			assertEquals(8, layer.getCapsuleDimensions())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Config") void testConfig()
		Friend Overridable Sub testConfig()
			Dim layer1 As PrimaryCapsules = (New PrimaryCapsules.Builder(8, 10)).kernelSize(5, 5).stride(4, 4).useLeakyReLU(0.5).build()
			assertEquals(8, layer1.getCapsuleDimensions())
			assertEquals(10, layer1.getChannels())
			assertArrayEquals(New Integer() { 5, 5 }, layer1.getKernelSize())
			assertArrayEquals(New Integer() { 4, 4 }, layer1.getStride())
			assertArrayEquals(New Integer() { 0, 0 }, layer1.getPadding())
			assertArrayEquals(New Integer() { 1, 1 }, layer1.getDilation())
			assertTrue(layer1.isUseRelu())
			assertEquals(0.5, layer1.getLeak(), 0.001)
			Dim layer2 As PrimaryCapsules = (New PrimaryCapsules.Builder(8, 10)).kernelSize(5, 5).stride(4, 4).build()
			assertFalse(layer2.isUseRelu())
			Dim layer3 As PrimaryCapsules = (New PrimaryCapsules.Builder(8, 10)).kernelSize(5, 5).stride(4, 4).useReLU().build()
			assertTrue(layer3.isUseRelu())
			assertEquals(0, layer3.getLeak(), 0.001)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Layer") void testLayer()
		Friend Overridable Sub testLayer()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(123).list().layer((New PrimaryCapsules.Builder(8, 10)).kernelSize(5, 5).stride(4, 4).useLeakyReLU(0.5).build()).setInputType(InputType.convolutional(20, 20, 20)).build()
			Dim model As New MultiLayerNetwork(conf)
			model.init()
			Dim emptyFeatures As INDArray = Nd4j.zeros(64, 20, 20, 20)
			Dim shape() As Long = model.output(emptyFeatures).shape()
			assertArrayEquals(New Long() { 64, 160, 8 }, shape)
		End Sub
	End Class

End Namespace