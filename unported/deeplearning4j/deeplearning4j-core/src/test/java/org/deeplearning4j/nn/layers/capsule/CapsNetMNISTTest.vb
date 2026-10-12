Imports System
import static org.junit.jupiter.api.Assertions.assertTrue
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ActivationLayer = org.deeplearning4j.nn.conf.layers.ActivationLayer
Imports CapsuleLayer = org.deeplearning4j.nn.conf.layers.CapsuleLayer
Imports CapsuleStrengthLayer = org.deeplearning4j.nn.conf.layers.CapsuleStrengthLayer
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports LossLayer = org.deeplearning4j.nn.conf.layers.LossLayer
Imports PrimaryCapsules = org.deeplearning4j.nn.conf.layers.PrimaryCapsules
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports ActivationSoftmax = org.nd4j.linalg.activations.impl.ActivationSoftmax
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports LossNegativeLogLikelihood = org.nd4j.linalg.lossfunctions.impl.LossNegativeLogLikelihood
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
'ORIGINAL LINE: @Disabled("AB - ignored due to excessive runtime. Keep for manual debugging when required") @DisplayName("Caps Net MNIST Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class CapsNetMNISTTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class CapsNetMNISTTest
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Caps Net On MNIST") void testCapsNetOnMNIST()
		Friend Overridable Sub testCapsNetOnMNIST()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(123).updater(New Adam()).list().layer((New ConvolutionLayer.Builder()).nOut(16).kernelSize(9, 9).stride(3, 3).build()).layer((New PrimaryCapsules.Builder(8, 8)).kernelSize(7, 7).stride(2, 2).build()).layer((New CapsuleLayer.Builder(10, 16, 3)).build()).layer((New CapsuleStrengthLayer.Builder()).build()).layer((New ActivationLayer.Builder(New ActivationSoftmax())).build()).layer((New LossLayer.Builder(New LossNegativeLogLikelihood())).build()).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()
			Dim model As New MultiLayerNetwork(conf)
			model.init()
			Dim rngSeed As Integer = 12345
			Try
				Dim mnistTrain As New MnistDataSetIterator(64, True, rngSeed)
				Dim mnistTest As New MnistDataSetIterator(64, False, rngSeed)
				For i As Integer = 0 To 1
					model.fit(mnistTrain)
				Next i
				Dim eval As Evaluation = model.evaluate(mnistTest)
				assertTrue(eval.accuracy() > 0.95, "Accuracy not over 95%")
				assertTrue(eval.precision() > 0.95, "Precision not over 95%")
				assertTrue(eval.recall() > 0.95, "Recall not over 95%")
				assertTrue(eval.f1() > 0.95, "F1-score not over 95%")
			Catch e As IOException
				Console.WriteLine("Could not load MNIST.")
			End Try
		End Sub
	End Class

End Namespace