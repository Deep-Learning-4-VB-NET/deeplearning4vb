Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports LocalResponseNormalization = org.deeplearning4j.nn.conf.layers.LocalResponseNormalization
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.deeplearning4j.gradientcheck


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) @NativeTag public class LRNGradientCheckTests extends org.deeplearning4j.BaseDL4JTest
	Public Class LRNGradientCheckTests
		Inherits BaseDL4JTest

		Private Const PRINT_RESULTS As Boolean = True
		Private Const RETURN_ON_FIRST_FAILURE As Boolean = False
		Private Const DEFAULT_EPS As Double = 1e-5
		Private Const DEFAULT_MAX_REL_ERROR As Double = 1e-5
		Private Const DEFAULT_MIN_ABS_ERROR As Double = 1e-9

		Shared Sub New()
			Nd4j.DataType = DataType.DOUBLE
		End Sub

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000L
			End Get
		End Property


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGradientLRNSimple()
		Public Overridable Sub testGradientLRNSimple()
			Nd4j.Random.setSeed(12345)
			Dim minibatch As Integer = 10
			Dim depth As Integer = 6
			Dim hw As Integer = 5
			Dim nOut As Integer = 4
			Dim input As INDArray = Nd4j.rand(New Integer() {minibatch, depth, hw, hw})
			Dim labels As INDArray = Nd4j.zeros(minibatch, nOut)
			Dim r As New Random(12345)
			For i As Integer = 0 To minibatch - 1
				labels.putScalar(i, r.Next(nOut), 1.0)
			Next i

			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dataType(DataType.DOUBLE).seed(12345L).dist(New NormalDistribution(0, 2)).list().layer(0, (New ConvolutionLayer.Builder()).nOut(6).kernelSize(2, 2).stride(1, 1).activation(Activation.TANH).build()).layer(1, (New LocalResponseNormalization.Builder()).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(nOut).build()).setInputType(InputType.convolutional(hw, hw, depth))

			Dim mln As New MultiLayerNetwork(builder.build())
			mln.init()

	'        if (PRINT_RESULTS) {
	'            for (int j = 0; j < mln.getnLayers(); j++)
	'                System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
	'        }

			Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)

			assertTrue(gradOK)
			TestUtils.testModelSerialization(mln)
		End Sub

	End Class

End Namespace