Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports LossLayer = org.deeplearning4j.nn.conf.layers.LossLayer
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports org.nd4j.linalg.lossfunctions.impl
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
Namespace org.deeplearning4j.nn.conf

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Json Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class JsonTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class JsonTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Json Loss Functions") void testJsonLossFunctions()
		Friend Overridable Sub testJsonLossFunctions()
			Dim lossFunctions() As ILossFunction = {
				New LossBinaryXENT(),
				New LossBinaryXENT(),
				New LossCosineProximity(),
				New LossHinge(),
				New LossKLD(),
				New LossKLD(),
				New LossL1(),
				New LossL1(),
				New LossL2(),
				New LossL2(),
				New LossMAE(),
				New LossMAE(),
				New LossMAPE(),
				New LossMAPE(),
				New LossMCXENT(),
				New LossMSE(),
				New LossMSE(),
				New LossMSLE(),
				New LossMSLE(),
				New LossNegativeLogLikelihood(),
				New LossNegativeLogLikelihood(),
				New LossPoisson(),
				New LossSquaredHinge(),
				New LossFMeasure(),
				New LossFMeasure(2.0)
			}
			Dim outputActivationFn() As Activation = { Activation.SIGMOID, Activation.SIGMOID, Activation.TANH, Activation.TANH, Activation.SIGMOID, Activation.SOFTMAX, Activation.TANH, Activation.SOFTMAX, Activation.TANH, Activation.SOFTMAX, Activation.IDENTITY, Activation.SOFTMAX, Activation.IDENTITY, Activation.SOFTMAX, Activation.SOFTMAX, Activation.IDENTITY, Activation.SOFTMAX, Activation.SIGMOID, Activation.SOFTMAX, Activation.SIGMOID, Activation.SOFTMAX, Activation.SIGMOID, Activation.TANH, Activation.SIGMOID, Activation.SOFTMAX }
			Dim nOut() As Integer = { 1, 3, 5, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 2 }
			For i As Integer = 0 To lossFunctions.Length - 1
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(Updater.ADAM).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(nOut(i)).activation(Activation.TANH).build()).layer(1, (New LossLayer.Builder()).lossFunction(lossFunctions(i)).activation(outputActivationFn(i)).build()).validateOutputLayerConfig(False).build()
				Dim json As String = conf.toJson()
				Dim yaml As String = conf.toYaml()
				Dim fromJson As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json)
				Dim fromYaml As MultiLayerConfiguration = MultiLayerConfiguration.fromYaml(yaml)
				assertEquals(conf, fromJson)
				assertEquals(conf, fromYaml)
			Next i
		End Sub
	End Class

End Namespace