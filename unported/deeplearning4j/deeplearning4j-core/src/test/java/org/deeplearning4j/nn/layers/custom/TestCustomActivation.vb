Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports CustomActivation = org.deeplearning4j.nn.layers.custom.testclasses.CustomActivation
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports AnnotatedClass = org.nd4j.shade.jackson.databind.introspect.AnnotatedClass
Imports NamedType = org.nd4j.shade.jackson.databind.jsontype.NamedType
import static org.junit.jupiter.api.Assertions.assertEquals
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

Namespace org.deeplearning4j.nn.layers.custom


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.CUSTOM_FUNCTIONALITY) public class TestCustomActivation extends org.deeplearning4j.BaseDL4JTest
	Public Class TestCustomActivation
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCustomActivationFn()
		Public Overridable Sub testCustomActivationFn()
			'Second: let's create a MultiLayerCofiguration with one, and check JSON and YAML config actually works...

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(10).activation(New CustomActivation()).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build()).build()

			Dim json As String = conf.toJson()
			Dim yaml As String = conf.toYaml()

	'        System.out.println(json);

			Dim confFromJson As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json)
			assertEquals(conf, confFromJson)

			Dim confFromYaml As MultiLayerConfiguration = MultiLayerConfiguration.fromYaml(yaml)
			assertEquals(conf, confFromYaml)

		End Sub

	End Class

End Namespace