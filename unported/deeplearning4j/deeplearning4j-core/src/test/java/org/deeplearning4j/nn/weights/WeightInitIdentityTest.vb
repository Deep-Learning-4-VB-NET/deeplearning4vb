Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports ActivationIdentity = org.nd4j.linalg.activations.impl.ActivationIdentity
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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
Namespace org.deeplearning4j.nn.weights

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Weight Init Identity Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class WeightInitIdentityTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class WeightInitIdentityTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("Ignore for now. Underlying logic changed. Gradient checker passes so implementatin is valid.") @DisplayName("Test Id Conv 1 D") void testIdConv1D()
		Friend Overridable Sub testIdConv1D()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray input = org.nd4j.linalg.factory.Nd4j.randn(org.nd4j.linalg.api.buffer.DataType.FLOAT, 1, 5, 7);
			Dim input As INDArray = Nd4j.randn(DataType.FLOAT, 1, 5, 7)
			Const inputName As String = "input"
			Const conv As String = "conv"
			Const output As String = "output"
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.graph.ComputationGraph graph = new org.deeplearning4j.nn.graph.ComputationGraph(new org.deeplearning4j.nn.conf.NeuralNetConfiguration.Builder().graphBuilder().addInputs(inputName).setOutputs(output).layer(conv, new Convolution1DLayer.Builder(7).convolutionMode(org.deeplearning4j.nn.conf.ConvolutionMode.Same).nOut(input.size(1)).weightInit(new WeightInitIdentity()).activation(new org.nd4j.linalg.activations.impl.ActivationIdentity()).build(), inputName).layer(output, new RnnLossLayer.Builder().activation(new org.nd4j.linalg.activations.impl.ActivationIdentity()).build(), conv).setInputTypes(org.deeplearning4j.nn.conf.inputs.InputType.recurrent(5, 7, org.deeplearning4j.nn.conf.RNNFormat.NCW)).build());
			Dim graph As New ComputationGraph((New NeuralNetConfiguration.Builder()).graphBuilder().addInputs(inputName).setOutputs(output).layer(conv, (New Convolution1DLayer.Builder(7)).convolutionMode(ConvolutionMode.Same).nOut(input.size(1)).weightInit(New WeightInitIdentity()).activation(New ActivationIdentity()).build(), inputName).layer(output, (New RnnLossLayer.Builder()).activation(New ActivationIdentity()).build(), conv).setInputTypes(InputType.recurrent(5, 7, RNNFormat.NCW)).build())
			graph.init()
			Dim reshape As INDArray = graph.outputSingle(input).reshape(input.shape())
			assertEquals(input, reshape, "Mapping was not identity!")
		End Sub

		''' <summary>
		''' Test identity mapping for 2d convolution
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Id Conv 2 D") void testIdConv2D()
		Friend Overridable Sub testIdConv2D()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray input = org.nd4j.linalg.factory.Nd4j.randn(org.nd4j.linalg.api.buffer.DataType.FLOAT, 1, 5, 7, 11);
			Dim input As INDArray = Nd4j.randn(DataType.FLOAT, 1, 5, 7, 11)
			Const inputName As String = "input"
			Const conv As String = "conv"
			Const output As String = "output"
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.graph.ComputationGraph graph = new org.deeplearning4j.nn.graph.ComputationGraph(new org.deeplearning4j.nn.conf.NeuralNetConfiguration.Builder().graphBuilder().setInputTypes(org.deeplearning4j.nn.conf.inputs.InputType.inferInputType(input)).addInputs(inputName).setOutputs(output).layer(conv, new ConvolutionLayer.Builder(3, 5).convolutionMode(org.deeplearning4j.nn.conf.ConvolutionMode.Same).nOut(input.size(1)).weightInit(new WeightInitIdentity()).activation(new org.nd4j.linalg.activations.impl.ActivationIdentity()).build(), inputName).layer(output, new CnnLossLayer.Builder().activation(new org.nd4j.linalg.activations.impl.ActivationIdentity()).build(), conv).build());
			Dim graph As New ComputationGraph((New NeuralNetConfiguration.Builder()).graphBuilder().setInputTypes(InputType.inferInputType(input)).addInputs(inputName).setOutputs(output).layer(conv, (New ConvolutionLayer.Builder(3, 5)).convolutionMode(ConvolutionMode.Same).nOut(input.size(1)).weightInit(New WeightInitIdentity()).activation(New ActivationIdentity()).build(), inputName).layer(output, (New CnnLossLayer.Builder()).activation(New ActivationIdentity()).build(), conv).build())
			graph.init()
			assertEquals(input, graph.outputSingle(input), "Mapping was not identity!")
		End Sub

		''' <summary>
		''' Test identity mapping for 3d convolution
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Id Conv 3 D") void testIdConv3D()
		Friend Overridable Sub testIdConv3D()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray input = org.nd4j.linalg.factory.Nd4j.randn(org.nd4j.linalg.api.buffer.DataType.FLOAT, 1, 5, 7, 11, 13);
			Dim input As INDArray = Nd4j.randn(DataType.FLOAT, 1, 5, 7, 11, 13)
			Const inputName As String = "input"
			Const conv As String = "conv"
			Const output As String = "output"
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.graph.ComputationGraph graph = new org.deeplearning4j.nn.graph.ComputationGraph(new org.deeplearning4j.nn.conf.NeuralNetConfiguration.Builder().graphBuilder().setInputTypes(org.deeplearning4j.nn.conf.inputs.InputType.inferInputType(input)).addInputs(inputName).setOutputs(output).layer(conv, new Convolution3D.Builder(3, 7, 5).convolutionMode(org.deeplearning4j.nn.conf.ConvolutionMode.Same).dataFormat(Convolution3D.DataFormat.NCDHW).nOut(input.size(1)).weightInit(new WeightInitIdentity()).activation(new org.nd4j.linalg.activations.impl.ActivationIdentity()).build(), inputName).layer(output, new Cnn3DLossLayer.Builder(Convolution3D.DataFormat.NCDHW).activation(new org.nd4j.linalg.activations.impl.ActivationIdentity()).build(), conv).build());
			Dim graph As New ComputationGraph((New NeuralNetConfiguration.Builder()).graphBuilder().setInputTypes(InputType.inferInputType(input)).addInputs(inputName).setOutputs(output).layer(conv, (New Convolution3D.Builder(3, 7, 5)).convolutionMode(ConvolutionMode.Same).dataFormat(Convolution3D.DataFormat.NCDHW).nOut(input.size(1)).weightInit(New WeightInitIdentity()).activation(New ActivationIdentity()).build(), inputName).layer(output, (New Cnn3DLossLayer.Builder(Convolution3D.DataFormat.NCDHW)).activation(New ActivationIdentity()).build(), conv).build())
			graph.init()
			assertEquals(input, graph.outputSingle(input), "Mapping was not identity!")
		End Sub
	End Class

End Namespace