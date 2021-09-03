Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports EarlyTerminationDataSetIterator = org.deeplearning4j.datasets.iterator.EarlyTerminationDataSetIterator
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports WorkspaceMode = org.deeplearning4j.nn.conf.WorkspaceMode
Imports BernoulliReconstructionDistribution = org.deeplearning4j.nn.conf.layers.variational.BernoulliReconstructionDistribution
Imports GaussianReconstructionDistribution = org.deeplearning4j.nn.conf.layers.variational.GaussianReconstructionDistribution
Imports VariationalAutoencoder = org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports Adam = org.nd4j.linalg.learning.config.Adam
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotEquals

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

Namespace org.deeplearning4j.nn.graph


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class TestCompGraphUnsupervised extends org.deeplearning4j.BaseDL4JTest
	Public Class TestCompGraphUnsupervised
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVAE() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testVAE()

			For Each wsm As WorkspaceMode In New WorkspaceMode(){WorkspaceMode.NONE, WorkspaceMode.ENABLED}

				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New Adam(1e-3)).weightInit(WeightInit.XAVIER).inferenceWorkspaceMode(wsm).trainingWorkspaceMode(wsm).graphBuilder().addInputs("in").addLayer("vae1", (New VariationalAutoencoder.Builder()).nIn(784).nOut(32).encoderLayerSizes(16).decoderLayerSizes(16).activation(Activation.TANH).pzxActivationFunction(Activation.SIGMOID).reconstructionDistribution(New BernoulliReconstructionDistribution(Activation.SIGMOID)).build(), "in").addLayer("vae2", (New VariationalAutoencoder.Builder()).nIn(32).nOut(8).encoderLayerSizes(16).decoderLayerSizes(16).activation(Activation.TANH).pzxActivationFunction(Activation.SIGMOID).reconstructionDistribution(New GaussianReconstructionDistribution(Activation.TANH)).build(), "vae1").setOutputs("vae2").build()

				Dim cg As New ComputationGraph(conf)
				cg.init()

				Dim ds As DataSetIterator = New EarlyTerminationDataSetIterator(New MnistDataSetIterator(8, True, 12345), 3)

				Dim paramsBefore As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()

				'Pretrain first layer
				For Each e As KeyValuePair(Of String, INDArray) In cg.paramTable().SetOfKeyValuePairs()
					paramsBefore(e.Key) = e.Value.dup()
				Next e
				cg.pretrainLayer("vae1", ds)
				For Each e As KeyValuePair(Of String, INDArray) In cg.paramTable().SetOfKeyValuePairs()
					If e.Key.StartsWith("vae1") Then
						assertNotEquals(paramsBefore(e.Key), e.Value)
					Else
						assertEquals(paramsBefore(e.Key), e.Value)
					End If
				Next e

				Dim count As Integer = Nd4j.Executioner.exec(New MatchCondition(cg.params(), Conditions.Nan)).getInt(0)
				assertEquals(0, count)


				'Pretrain second layer
				For Each e As KeyValuePair(Of String, INDArray) In cg.paramTable().SetOfKeyValuePairs()
					paramsBefore(e.Key) = e.Value.dup()
				Next e
				cg.pretrainLayer("vae2", ds)
				For Each e As KeyValuePair(Of String, INDArray) In cg.paramTable().SetOfKeyValuePairs()
					If e.Key.StartsWith("vae2") Then
						assertNotEquals(paramsBefore(e.Key), e.Value)
					Else
						assertEquals(paramsBefore(e.Key), e.Value)
					End If
				Next e

				count = Nd4j.Executioner.exec(New MatchCondition(cg.params(), Conditions.Nan)).getInt(0)
				assertEquals(0, count)
			Next wsm
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void compareImplementations() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub compareImplementations()

			For Each wsm As WorkspaceMode In New WorkspaceMode(){WorkspaceMode.NONE, WorkspaceMode.ENABLED}

				Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New Adam(1e-3)).weightInit(WeightInit.XAVIER).inferenceWorkspaceMode(wsm).trainingWorkspaceMode(wsm).list().layer((New VariationalAutoencoder.Builder()).nIn(784).nOut(32).encoderLayerSizes(16).decoderLayerSizes(16).activation(Activation.TANH).pzxActivationFunction(Activation.SIGMOID).reconstructionDistribution(New BernoulliReconstructionDistribution(Activation.SIGMOID)).build()).layer((New VariationalAutoencoder.Builder()).nIn(32).nOut(8).encoderLayerSizes(16).decoderLayerSizes(16).activation(Activation.TANH).pzxActivationFunction(Activation.SIGMOID).reconstructionDistribution(New GaussianReconstructionDistribution(Activation.TANH)).build()).build()

				Dim net As New MultiLayerNetwork(conf2)
				net.init()

				Dim cg As ComputationGraph = net.toComputationGraph()
				cg.Configuration.setInferenceWorkspaceMode(wsm)
				cg.Configuration.setTrainingWorkspaceMode(wsm)
				Dim ds As DataSetIterator = New EarlyTerminationDataSetIterator(New MnistDataSetIterator(1, True, 12345), 1)
				Nd4j.Random.setSeed(12345)
				net.pretrainLayer(0, ds)

				ds = New EarlyTerminationDataSetIterator(New MnistDataSetIterator(1, True, 12345), 1)
				Nd4j.Random.setSeed(12345)
				cg.pretrainLayer("0", ds)

				assertEquals(net.params(), cg.params())
			Next wsm
		End Sub

	End Class

End Namespace