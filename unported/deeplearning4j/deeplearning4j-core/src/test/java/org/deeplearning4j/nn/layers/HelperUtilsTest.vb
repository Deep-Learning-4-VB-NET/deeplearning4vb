Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DropoutHelper = org.deeplearning4j.nn.conf.dropout.DropoutHelper
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ActivationLayer = org.deeplearning4j.nn.conf.layers.ActivationLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports org.deeplearning4j.nn.conf.layers
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports ConvolutionHelper = org.deeplearning4j.nn.layers.convolution.ConvolutionHelper
Imports SubsamplingHelper = org.deeplearning4j.nn.layers.convolution.subsampling.SubsamplingHelper
Imports org.deeplearning4j.nn.layers.mkldnn
Imports BatchNormalizationHelper = org.deeplearning4j.nn.layers.normalization.BatchNormalizationHelper
Imports LocalResponseNormalizationHelper = org.deeplearning4j.nn.layers.normalization.LocalResponseNormalizationHelper
Imports LSTMHelper = org.deeplearning4j.nn.layers.recurrent.LSTMHelper
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports ActivationELU = org.nd4j.linalg.activations.impl.ActivationELU
Imports ActivationRationalTanh = org.nd4j.linalg.activations.impl.ActivationRationalTanh
Imports ActivationSoftmax = org.nd4j.linalg.activations.impl.ActivationSoftmax
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.deeplearning4j.common.config.DL4JSystemProperties.DISABLE_HELPER_PROPERTY
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
Namespace org.deeplearning4j.nn.layers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Activation Layer Test") @NativeTag @Tag(TagNames.CUSTOM_FUNCTIONALITY) @Tag(TagNames.DL4J_OLD_API) public class HelperUtilsTest extends org.deeplearning4j.BaseDL4JTest
	Public Class HelperUtilsTest
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test instance creation of various helpers") public void testOneDnnHelperCreation()
		Public Overridable Sub testOneDnnHelperCreation()
			System.setProperty(DISABLE_HELPER_PROPERTY,"false")
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			assertNotNull(HelperUtils.createHelper("", GetType(MKLDNNBatchNormHelper).FullName, GetType(BatchNormalizationHelper),"layername",DataType))
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			assertNotNull(HelperUtils.createHelper("", GetType(MKLDNNLocalResponseNormalizationHelper).FullName, GetType(LocalResponseNormalizationHelper),"layername",DataType))
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			assertNotNull(HelperUtils.createHelper("", GetType(MKLDNNSubsamplingHelper).FullName, GetType(SubsamplingHelper),"layername",DataType))
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			assertNotNull(HelperUtils.createHelper("", GetType(MKLDNNConvHelper).FullName, GetType(ConvolutionHelper),"layername",DataType))


		End Sub


	End Class

End Namespace