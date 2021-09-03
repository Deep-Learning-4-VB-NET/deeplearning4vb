Imports System
Imports System.Threading
Imports Microsoft.VisualBasic
Imports EarlyTerminationDataSetIterator = org.deeplearning4j.datasets.iterator.EarlyTerminationDataSetIterator
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Resources = org.nd4j.common.resources.Resources
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports RmsProp = org.nd4j.linalg.learning.config.RmsProp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions

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

Namespace org.deeplearning4j


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.RNG) public class RandomTests extends BaseDL4JTest
	Public Class RandomTests
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) public void testReproduce() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testReproduce()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.conf.MultiLayerConfiguration conf = new org.deeplearning4j.nn.conf.NeuralNetConfiguration.Builder().updater(new org.nd4j.linalg.learning.config.RmsProp()).optimizationAlgo(org.deeplearning4j.nn.api.OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, new org.deeplearning4j.nn.conf.layers.DenseLayer.Builder().nIn(28 * 28).nOut(10).activation(org.nd4j.linalg.activations.Activation.TANH).build()).layer(1, new org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction.MCXENT).nIn(10).nOut(10).activation(org.nd4j.linalg.activations.Activation.SOFTMAX).build()).build();
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New RmsProp()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New org.deeplearning4j.nn.conf.layers.DenseLayer.Builder()).nIn(28 * 28).nOut(10).activation(Activation.TANH).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(10).nOut(10).activation(Activation.SOFTMAX).build()).build()

			For e As Integer = 0 To 2

				Dim nThreads As Integer = 10
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.CountDownLatch l = new java.util.concurrent.CountDownLatch(nThreads);
				Dim l As New System.Threading.CountdownEvent(nThreads)
				For i As Integer = 0 To nThreads - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int j = i;
					Dim j As Integer = i
					Dim t As New Thread(Sub()
					Try
						Dim net As New MultiLayerNetwork(conf.clone())
						net.init()
						Dim iter As DataSetIterator = New EarlyTerminationDataSetIterator(New MnistDataSetIterator(10, False, 12345), 100)
						net.fit(iter)
					Catch t As Exception
						Console.WriteLine("Thread failed: " & j)
						t.printStackTrace()
					Finally
						l.Signal()
					End Try
					End Sub)
					t.Start()
				Next i

				l.await()
				Console.WriteLine("DONE " & e & vbLf)
			Next e
		End Sub
	End Class

End Namespace