import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.assertEquals
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports CapsuleStrengthLayer = org.deeplearning4j.nn.conf.layers.CapsuleStrengthLayer
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
'ORIGINAL LINE: @DisplayName("Capsule Strength Layer Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class CapsuleStrengthLayerTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class CapsuleStrengthLayerTest
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Output Type") void testOutputType()
		Friend Overridable Sub testOutputType()
			Dim layer As CapsuleStrengthLayer = (New CapsuleStrengthLayer.Builder()).build()
			Dim in1 As InputType = InputType.recurrent(5, 8)
			assertEquals(InputType.feedForward(5), layer.getOutputType(0, in1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Layer") void testLayer()
		Friend Overridable Sub testLayer()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(123).list().layer((New CapsuleStrengthLayer.Builder()).build()).setInputType(InputType.recurrent(5, 8)).build()
			Dim model As New MultiLayerNetwork(conf)
			model.init()
			Dim emptyFeatures As INDArray = Nd4j.zeros(64, 5, 10)
			Dim shape() As Long = model.output(emptyFeatures).shape()
			assertArrayEquals(New Long() { 64, 5 }, shape)
		End Sub
	End Class

End Namespace