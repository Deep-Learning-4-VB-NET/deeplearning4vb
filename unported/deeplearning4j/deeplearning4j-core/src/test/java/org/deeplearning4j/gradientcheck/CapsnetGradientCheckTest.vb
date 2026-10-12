Imports System
import static org.junit.jupiter.api.Assertions.assertTrue
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports UniformDistribution = org.deeplearning4j.nn.conf.distribution.UniformDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ActivationLayer = org.deeplearning4j.nn.conf.layers.ActivationLayer
Imports CapsuleLayer = org.deeplearning4j.nn.conf.layers.CapsuleLayer
Imports CapsuleStrengthLayer = org.deeplearning4j.nn.conf.layers.CapsuleStrengthLayer
Imports LossLayer = org.deeplearning4j.nn.conf.layers.LossLayer
Imports PrimaryCapsules = org.deeplearning4j.nn.conf.layers.PrimaryCapsules
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInitDistribution = org.deeplearning4j.nn.weights.WeightInitDistribution
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports ActivationSoftmax = org.nd4j.linalg.activations.impl.ActivationSoftmax
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
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
Namespace org.deeplearning4j.gradientcheck

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @DisplayName("Capsnet Gradient Check Test") @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) @NativeTag class CapsnetGradientCheckTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class CapsnetGradientCheckTest
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Caps Net") void testCapsNet()
		Friend Overridable Sub testCapsNet()
			Dim minibatchSizes() As Integer = { 8, 16 }
			Dim width As Integer = 6
			Dim height As Integer = 6
			Dim inputDepth As Integer = 4
			Dim primaryCapsDims() As Integer = { 2, 4 }
			Dim primaryCapsChannels() As Integer = { 8 }
			Dim capsules() As Integer = { 5 }
			Dim capsuleDims() As Integer = { 4, 8 }
			Dim routings() As Integer = { 1 }
			Nd4j.Random.setSeed(12345)
			For Each routing As Integer In routings
				For Each primaryCapsDim As Integer In primaryCapsDims
					For Each primarpCapsChannel As Integer In primaryCapsChannels
						For Each capsule As Integer In capsules
							For Each capsuleDim As Integer In capsuleDims
								For Each minibatchSize As Integer In minibatchSizes
									Dim input As INDArray = Nd4j.rand(minibatchSize, inputDepth * height * width).mul(10).reshape(-1, inputDepth, height, width)
									Dim labels As INDArray = Nd4j.zeros(minibatchSize, capsule)
									For i As Integer = 0 To minibatchSize - 1
										labels.putScalar(New Integer() { i, i Mod capsule }, 1.0)
									Next i
									Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).seed(123).updater(New NoOp()).weightInit(New WeightInitDistribution(New UniformDistribution(-6, 6))).list().layer((New PrimaryCapsules.Builder(primaryCapsDim, primarpCapsChannel)).kernelSize(3, 3).stride(2, 2).build()).layer((New CapsuleLayer.Builder(capsule, capsuleDim, routing)).build()).layer((New CapsuleStrengthLayer.Builder()).build()).layer((New ActivationLayer.Builder(New ActivationSoftmax())).build()).layer((New LossLayer.Builder(New LossNegativeLogLikelihood())).build()).setInputType(InputType.convolutional(height, width, inputDepth)).build()
									Dim net As New MultiLayerNetwork(conf)
									net.init()
									For i As Integer = 0 To 3
										Console.WriteLine("nParams, layer " & i & ": " & net.getLayer(i).numParams())
									Next i
									Dim msg As String = "minibatch=" & minibatchSize & ", PrimaryCaps: " & primarpCapsChannel & " channels, " & primaryCapsDim & " dimensions, Capsules: " & capsule & " capsules with " & capsuleDim & " dimensions and " & routing & " routings"
									Console.WriteLine(msg)
									Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input(input).labels(labels).subset(True).maxPerParam(100))
									assertTrue(gradOK,msg)
									TestUtils.testModelSerialization(net)
								Next minibatchSize
							Next capsuleDim
						Next capsule
					Next primarpCapsChannel
				Next primaryCapsDim
			Next routing
		End Sub
	End Class

End Namespace