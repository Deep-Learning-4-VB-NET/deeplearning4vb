Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports Updater = org.deeplearning4j.nn.api.Updater
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
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
Namespace org.deeplearning4j.nn.misc

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.FILE_IO) @Tag(TagNames.WORKSPACES) public class CloseNetworkTests extends org.deeplearning4j.BaseDL4JTest
	Public Class CloseNetworkTests
		Inherits BaseDL4JTest

		Public Shared ReadOnly Property TestNet As MultiLayerNetwork
			Get
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Adam(1e-3)).list().layer((New ConvolutionLayer.Builder()).nOut(5).kernelSize(3, 3).activation(Activation.TANH).build()).layer((New BatchNormalization.Builder()).nOut(5).build()).layer((New SubsamplingLayer.Builder()).build()).layer((New DenseLayer.Builder()).nOut(10).activation(Activation.RELU).build()).layer((New OutputLayer.Builder()).nOut(10).build()).setInputType(InputType.convolutional(28, 28, 1)).build()
    
				Dim net As New MultiLayerNetwork(conf)
				net.init()
    
				Return net
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCloseMLN()
		Public Overridable Sub testCloseMLN()
			For Each train As Boolean In New Boolean(){False, True}
				For Each test As Boolean In New Boolean(){False, True}
					Dim net As MultiLayerNetwork = TestNet

					Dim f As INDArray = Nd4j.rand(DataType.FLOAT, 16, 1, 28, 28)
					Dim l As INDArray = TestUtils.randomOneHot(16, 10)

					If train Then
						For i As Integer = 0 To 2
							net.fit(f, l)
						Next i
					End If

					If test Then
						For i As Integer = 0 To 2
							net.output(f)
						Next i
					End If

					net.close()

					assertTrue(net.params().wasClosed())
					If train Then
						assertTrue(net.GradientsViewArray.wasClosed())
						Dim u As Updater = net.getUpdater(False)
						assertTrue(u.StateViewArray.wasClosed())
					End If

					'Make sure we don't get crashes etc when trying to use after closing
					Try
						net.output(f)
					Catch e As System.InvalidOperationException
						Dim msg As String = e.Message
						assertTrue(msg.Contains("released"),msg)
					End Try

					Try
						net.fit(f, l)
					Catch e As System.InvalidOperationException
						Dim msg As String = e.Message
						assertTrue(msg.Contains("released"),msg)
					End Try
				Next test
			Next train
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCloseCG()
		Public Overridable Sub testCloseCG()
			For Each train As Boolean In New Boolean(){False, True}
				For Each test As Boolean In New Boolean(){False, True}
					Dim net As ComputationGraph = TestNet.toComputationGraph()

					Dim f As INDArray = Nd4j.rand(DataType.FLOAT, 16, 1, 28, 28)
					Dim l As INDArray = TestUtils.randomOneHot(16, 10)

					If train Then
						For i As Integer = 0 To 2
							net.fit(New INDArray(){f}, New INDArray(){l})
						Next i
					End If

					If test Then
						For i As Integer = 0 To 2
							net.output(f)
						Next i
					End If

					net.close()

					assertTrue(net.params().wasClosed())
					If train Then
						assertTrue(net.GradientsViewArray.wasClosed())
						Dim u As Updater = net.getUpdater(False)
						assertTrue(u.StateViewArray.wasClosed())
					End If

					'Make sure we don't get crashes etc when trying to use after closing
					Try
						net.output(f)
					Catch e As System.InvalidOperationException
						Dim msg As String = e.Message
						assertTrue(msg.Contains("released"),msg)
					End Try

					Try
						net.fit(New INDArray(){f}, New INDArray(){l})
					Catch e As System.InvalidOperationException
						Dim msg As String = e.Message
						assertTrue(msg.Contains("released"),msg)
					End Try
				Next test
			Next train
		End Sub
	End Class

End Namespace