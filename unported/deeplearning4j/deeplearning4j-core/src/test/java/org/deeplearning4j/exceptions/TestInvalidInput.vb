Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports DL4JException = org.deeplearning4j.exception.DL4JException
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.exceptions


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.EVAL_METRICS) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) public class TestInvalidInput extends org.deeplearning4j.BaseDL4JTest
	Public Class TestInvalidInput
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInputNinMismatchDense()
		Public Overridable Sub testInputNinMismatchDense()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer(1, (New OutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Try
				net.feedForward(Nd4j.create(1, 20))
				fail("Expected DL4JException")
			Catch e As DL4JException
				Console.WriteLine("testInputNinMismatchDense(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail("Expected DL4JException")
			End Try
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLabelsNOutMismatchOutputLayer()
		Public Overridable Sub testLabelsNOutMismatchOutputLayer()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer(1, (New OutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Try
				net.fit(Nd4j.create(1, 10), Nd4j.create(1, 20))
				fail("Expected IllegalArgumentException")
			Catch e As System.ArgumentException
				'From loss function
				Console.WriteLine("testLabelsNOutMismatchOutputLayer(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail("Expected DL4JException")
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLabelsNOutMismatchRnnOutputLayer()
		Public Overridable Sub testLabelsNOutMismatchRnnOutputLayer()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New LSTM.Builder()).nIn(5).nOut(5).build()).layer(1, (New RnnOutputLayer.Builder()).nIn(5).nOut(5).activation(Activation.SOFTMAX).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Try
				net.fit(Nd4j.create(1, 5, 8), Nd4j.create(1, 10, 8))
				fail("Expected IllegalArgumentException")
			Catch e As System.ArgumentException
				'From loss function
				Console.WriteLine("testLabelsNOutMismatchRnnOutputLayer(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail("Expected DL4JException")
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInputNinMismatchConvolutional()
		Public Overridable Sub testInputNinMismatchConvolutional()
			'Rank 4 input, but input channels does not match nIn channels

			Dim h As Integer = 16
			Dim w As Integer = 16
			Dim d As Integer = 3

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New ConvolutionLayer.Builder()).nIn(d).nOut(5).build()).layer(1, (New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(h, w, d)).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Try
				net.feedForward(Nd4j.create(1, 5, h, w))
				fail("Expected DL4JException")
			Catch e As DL4JException
				Console.WriteLine("testInputNinMismatchConvolutional(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail("Expected DL4JException")
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInputNinRank2Convolutional()
		Public Overridable Sub testInputNinRank2Convolutional()
			'Rank 2 input, instead of rank 4 input. For example, forgetting the

			Dim h As Integer = 16
			Dim w As Integer = 16
			Dim d As Integer = 3

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New ConvolutionLayer.Builder()).nIn(d).nOut(5).build()).layer(1, (New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(h, w, d)).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Try
				net.feedForward(Nd4j.create(1, 5 * h * w))
				fail("Expected DL4JException")
			Catch e As DL4JException
				Console.WriteLine("testInputNinRank2Convolutional(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail("Expected DL4JException")
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInputNinRank2Subsampling()
		Public Overridable Sub testInputNinRank2Subsampling()
			'Rank 2 input, instead of rank 4 input. For example, using the wrong input type
			Dim h As Integer = 16
			Dim w As Integer = 16
			Dim d As Integer = 3

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New SubsamplingLayer.Builder()).kernelSize(2, 2).build()).layer(1, (New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(h, w, d)).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Try
				net.feedForward(Nd4j.create(1, 5 * h * w))
				fail("Expected DL4JException")
			Catch e As DL4JException
				Console.WriteLine("testInputNinRank2Subsampling(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail("Expected DL4JException")
			End Try
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInputNinMismatchLSTM()
		Public Overridable Sub testInputNinMismatchLSTM()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New GravesLSTM.Builder()).nIn(5).nOut(5).build()).layer(1, (New RnnOutputLayer.Builder()).nIn(5).nOut(5).activation(Activation.SOFTMAX).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Try
				net.fit(Nd4j.create(1, 10, 5), Nd4j.create(1, 5, 5))
				fail("Expected DL4JException")
			Catch e As DL4JException
				Console.WriteLine("testInputNinMismatchLSTM(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail("Expected DL4JException")
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInputNinMismatchBidirectionalLSTM()
		Public Overridable Sub testInputNinMismatchBidirectionalLSTM()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New GravesBidirectionalLSTM.Builder()).nIn(5).nOut(5).build()).layer(1, (New RnnOutputLayer.Builder()).nIn(5).nOut(5).activation(Activation.SOFTMAX).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Try
				net.fit(Nd4j.create(1, 10, 5), Nd4j.create(1, 5, 5))
				fail("Expected DL4JException")
			Catch e As DL4JException
				Console.WriteLine("testInputNinMismatchBidirectionalLSTM(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail("Expected DL4JException")
			End Try

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInputNinMismatchEmbeddingLayer()
		Public Overridable Sub testInputNinMismatchEmbeddingLayer()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New EmbeddingLayer.Builder()).nIn(10).nOut(10).build()).layer(1, (New OutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Try
				net.feedForward(Nd4j.create(10, 5))
				fail("Expected DL4JException")
			Catch e As DL4JException
				Console.WriteLine("testInputNinMismatchEmbeddingLayer(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail("Expected DL4JException")
			End Try
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInvalidRnnTimeStep()
		Public Overridable Sub testInvalidRnnTimeStep()
			'Idea: Using rnnTimeStep with a different number of examples between calls
			'(i.e., not calling reset between time steps)

			For Each layerType As String In New String(){"simple", "lstm", "graves"}

				Dim l As Layer
				Select Case layerType
					Case "simple"
						l = (New SimpleRnn.Builder()).nIn(5).nOut(5).build()
					Case "lstm"
						l = (New LSTM.Builder()).nIn(5).nOut(5).build()
					Case "graves"
						l = (New GravesLSTM.Builder()).nIn(5).nOut(5).build()
					Case Else
						Throw New Exception()
				End Select

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(l).layer((New RnnOutputLayer.Builder()).nIn(5).nOut(5).activation(Activation.SOFTMAX).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				net.rnnTimeStep(Nd4j.create(3, 5, 10))

				Dim m As IDictionary(Of String, INDArray) = net.rnnGetPreviousState(0)
				assertNotNull(m)
				assertFalse(m.Count = 0)

				Try
					net.rnnTimeStep(Nd4j.create(5, 5, 10))
					fail("Expected Exception - " & layerType)
				Catch e As Exception
					log.error("",e)
					Dim msg As String = e.Message
					assertTrue(msg IsNot Nothing AndAlso msg.Contains("rnn") AndAlso msg.Contains("batch"), msg)
				End Try
			Next layerType
		End Sub
	End Class

End Namespace