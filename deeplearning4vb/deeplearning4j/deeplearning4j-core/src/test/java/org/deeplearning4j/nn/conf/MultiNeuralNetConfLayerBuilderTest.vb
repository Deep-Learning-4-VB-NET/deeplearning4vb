Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports PoolingType = org.deeplearning4j.nn.conf.layers.SubsamplingLayer.PoolingType
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Convolution = org.nd4j.linalg.convolution.Convolution
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction
import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.assertFalse
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
Namespace org.deeplearning4j.nn.conf

	''' <summary>
	''' @author Jeffrey Tang.
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Multi Neural Net Conf Layer Builder Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class MultiNeuralNetConfLayerBuilderTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class MultiNeuralNetConfLayerBuilderTest
		Inherits BaseDL4JTest

		Friend numIn As Integer = 10

		Friend numOut As Integer = 5

		Friend drop As Double = 0.3

		Friend act As Activation = Activation.SOFTMAX

		Friend poolType As PoolingType = PoolingType.MAX

		Friend filterSize() As Integer = { 2, 2 }

		Friend filterDepth As Integer = 6

		Friend stride() As Integer = { 2, 2 }

		Friend k As Integer = 1

		Friend convType As Convolution.Type = Convolution.Type.FULL

		Friend loss As LossFunctions.LossFunction = LossFunctions.LossFunction.MCXENT

		Friend weight As WeightInit = WeightInit.XAVIER

		Friend corrupt As Double = 0.4

		Friend sparsity As Double = 0.3

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Neural Net Config API") void testNeuralNetConfigAPI()
		Friend Overridable Sub testNeuralNetConfigAPI()
			Dim newLoss As LossFunctions.LossFunction = LossFunctions.LossFunction.SQUARED_LOSS
			Dim newNumIn As Integer = numIn + 1
			Dim newNumOut As Integer = numOut + 1
			Dim newWeight As WeightInit = WeightInit.UNIFORM
			Dim newDrop As Double = 0.5
			Dim newFS() As Integer = { 3, 3 }
			Dim newFD As Integer = 7
			Dim newStride() As Integer = { 3, 3 }
			Dim newConvType As Convolution.Type = Convolution.Type.SAME
			Dim newPoolType As PoolingType = PoolingType.AVG
			Dim newCorrupt As Double = 0.5
			Dim newSparsity As Double = 0.5
			Dim multiConf1 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New DenseLayer.Builder()).nIn(newNumIn).nOut(newNumOut).activation(act).build()).layer(1, (New DenseLayer.Builder()).nIn(newNumIn + 1).nOut(newNumOut + 1).activation(act).build()).build()
			Dim firstLayer As NeuralNetConfiguration = multiConf1.getConf(0)
			Dim secondLayer As NeuralNetConfiguration = multiConf1.getConf(1)
			assertFalse(firstLayer.Equals(secondLayer))
		End Sub
	End Class

End Namespace