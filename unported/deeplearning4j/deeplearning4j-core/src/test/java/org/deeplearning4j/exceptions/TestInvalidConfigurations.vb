Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports DL4JException = org.deeplearning4j.exception.DL4JException
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports CnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertThrows
import static org.junit.jupiter.api.Assertions.fail

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
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.EVAL_METRICS) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) public class TestInvalidConfigurations extends org.deeplearning4j.BaseDL4JTest
	Public Class TestInvalidConfigurations
		Inherits BaseDL4JTest

		Public Shared Function getDensePlusOutput(ByVal nIn As Integer, ByVal nOut As Integer) As MultiLayerNetwork
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(10).build()).layer(1, (New OutputLayer.Builder()).nIn(10).nOut(nOut).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Return net
		End Function

		Public Shared Function getLSTMPlusRnnOutput(ByVal nIn As Integer, ByVal nOut As Integer) As MultiLayerNetwork
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New GravesLSTM.Builder()).nIn(nIn).nOut(10).build()).layer(1, (New RnnOutputLayer.Builder()).nIn(10).nOut(nOut).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Return net
		End Function

		Public Shared Function getCnnPlusOutputLayer(ByVal depthIn As Integer, ByVal inH As Integer, ByVal inW As Integer, ByVal nOut As Integer) As MultiLayerNetwork
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New ConvolutionLayer.Builder()).nIn(depthIn).nOut(5).build()).layer(1, (New OutputLayer.Builder()).nOut(nOut).build()).setInputType(InputType.convolutional(inH, inW, depthIn)).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Return net
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDenseNin0()
		Public Overridable Sub testDenseNin0()
			Try
				getDensePlusOutput(0, 10)
				fail("Expected exception")
			Catch e As DL4JException
				Console.WriteLine("testDenseNin0(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDenseNout0()
		Public Overridable Sub testDenseNout0()
			Try
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(0).build()).layer(1, (New OutputLayer.Builder()).nIn(10).nOut(10).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()
				fail("Expected exception")
			Catch e As DL4JException
				Console.WriteLine("testDenseNout0(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testOutputLayerNin0()
		Public Overridable Sub testOutputLayerNin0()
			Try
				getDensePlusOutput(10, 0)
				fail("Expected exception")
			Catch e As DL4JException
				Console.WriteLine("testOutputLayerNin0(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRnnOutputLayerNin0()
		Public Overridable Sub testRnnOutputLayerNin0()
			Try
				getLSTMPlusRnnOutput(10, 0)
				fail("Expected exception")
			Catch e As DL4JException
				Console.WriteLine("testRnnOutputLayerNin0(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLSTMNIn0()
		Public Overridable Sub testLSTMNIn0()
			Try
				getLSTMPlusRnnOutput(0, 10)
				fail("Expected exception")
			Catch e As DL4JException
				Console.WriteLine("testLSTMNIn0(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLSTMNOut0()
		Public Overridable Sub testLSTMNOut0()
			Try
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New GravesLSTM.Builder()).nIn(10).nOut(0).build()).layer(1, (New RnnOutputLayer.Builder()).nIn(10).nOut(10).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()
				fail("Expected exception")
			Catch e As DL4JException
				Console.WriteLine("testLSTMNOut0(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConvolutionalNin0()
		Public Overridable Sub testConvolutionalNin0()
			Try
				getCnnPlusOutputLayer(0, 10, 10, 10)
				fail("Expected exception")
			Catch e As DL4JException
				Console.WriteLine("testConvolutionalNin0(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConvolutionalNOut0()
		Public Overridable Sub testConvolutionalNOut0()
			Try
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New ConvolutionLayer.Builder()).nIn(5).nOut(0).build()).layer(1, (New OutputLayer.Builder()).nOut(10).build()).setInputType(InputType.convolutional(10, 10, 5)).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()
				fail("Expected exception")
			Catch e As DL4JException
				Console.WriteLine("testConvolutionalNOut0(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail()
			End Try
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnInvalidConfigPaddingStridesHeight()
		Public Overridable Sub testCnnInvalidConfigPaddingStridesHeight()
			'Idea: some combination of padding/strides are invalid.

			Dim depthIn As Integer = 3
			Dim hIn As Integer = 10
			Dim wIn As Integer = 10

			'Using kernel size of 3, stride of 2:
			'(10-3+2*0)/2+1 = 7/2 + 1

			Try
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).convolutionMode(ConvolutionMode.Strict).list().layer(0, (New ConvolutionLayer.Builder()).kernelSize(3, 2).stride(2, 2).padding(0, 0).nOut(5).build()).layer(1, (New OutputLayer.Builder()).nOut(10).build()).setInputType(InputType.convolutional(hIn, wIn, depthIn)).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()
				fail("Expected exception")
			Catch e As DL4JException
				Console.WriteLine("testCnnInvalidConfigPaddingStridesHeight(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnInvalidConfigOrInput_SmallerDataThanKernel()
		Public Overridable Sub testCnnInvalidConfigOrInput_SmallerDataThanKernel()
			'Idea: same as testCnnInvalidConfigPaddingStridesHeight() but network is fed incorrect sized data
			' or equivalently, network is set up without using InputType functionality (hence missing validation there)

			Dim depthIn As Integer = 3
			Dim hIn As Integer = 10
			Dim wIn As Integer = 10

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New ConvolutionLayer.Builder()).kernelSize(7, 7).stride(1, 1).padding(0, 0).nOut(5).build()).layer(1, (New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(hIn, wIn, depthIn)).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Try
				net.feedForward(Nd4j.create(3, depthIn, 5, 5))
				fail("Expected exception")
			Catch e As DL4JException
				Console.WriteLine("testCnnInvalidConfigOrInput_SmallerDataThanKernel(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnInvalidConfigOrInput_BadStrides()
		Public Overridable Sub testCnnInvalidConfigOrInput_BadStrides()
			'Idea: same as testCnnInvalidConfigPaddingStridesHeight() but network is fed incorrect sized data
			' or equivalently, network is set up without using InputType functionality (hence missing validation there)

			Dim depthIn As Integer = 3
			Dim hIn As Integer = 10
			Dim wIn As Integer = 10

			'Invalid: (10-3+0)/2+1 = 4.5

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).convolutionMode(ConvolutionMode.Strict).list().layer(0, (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(2, 2).padding(0, 0).nIn(depthIn).nOut(5).build()).layer(1, (New OutputLayer.Builder()).nIn(5 * 4 * 4).nOut(10).activation(Activation.SOFTMAX).build()).inputPreProcessor(1, New CnnToFeedForwardPreProcessor()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Try
				net.feedForward(Nd4j.create(3, depthIn, hIn, wIn))
				fail("Expected exception")
			Catch e As DL4JException
				Console.WriteLine("testCnnInvalidConfigOrInput_BadStrides(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail()
			End Try
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnInvalidConfigPaddingStridesWidth()
		Public Overridable Sub testCnnInvalidConfigPaddingStridesWidth()
			'Idea: some combination of padding/strides are invalid.
			Dim depthIn As Integer = 3
			Dim hIn As Integer = 10
			Dim wIn As Integer = 10

			'Using kernel size of 3, stride of 2:
			'(10-3+2*0)/2+1 = 7/2 + 1

			Try
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New ConvolutionLayer.Builder()).kernelSize(2, 3).stride(2, 2).padding(0, 0).nOut(5).build()).layer(1, (New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(hIn, wIn, depthIn)).build()
			Catch e As Exception
				fail("Did not expect exception with default (truncate)")
			End Try

			Try
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).convolutionMode(ConvolutionMode.Strict).list().layer(0, (New ConvolutionLayer.Builder()).kernelSize(2, 3).stride(2, 2).padding(0, 0).nOut(5).build()).layer(1, (New OutputLayer.Builder()).nOut(10).build()).setInputType(InputType.convolutional(hIn, wIn, depthIn)).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()
				fail("Expected exception")
			Catch e As DL4JException
				Console.WriteLine("testCnnInvalidConfigPaddingStridesWidth(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnInvalidConfigPaddingStridesWidthSubsampling()
		Public Overridable Sub testCnnInvalidConfigPaddingStridesWidthSubsampling()
			'Idea: some combination of padding/strides are invalid.
			Dim depthIn As Integer = 3
			Dim hIn As Integer = 10
			Dim wIn As Integer = 10

			'Using kernel size of 3, stride of 2:
			'(10-3+2*0)/2+1 = 7/2 + 1

			Try
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).convolutionMode(ConvolutionMode.Strict).list().layer(0, (New SubsamplingLayer.Builder()).kernelSize(2, 3).stride(2, 2).padding(0, 0).build()).layer(1, (New OutputLayer.Builder()).nOut(10).build()).setInputType(InputType.convolutional(hIn, wIn, depthIn)).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()
				fail("Expected exception")
			Catch e As DL4JException
				Console.WriteLine("testCnnInvalidConfigPaddingStridesWidthSubsampling(): " & e.Message)
			Catch e As Exception
				log.error("",e)
				fail()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testCnnInvalidKernel()
		Public Overridable Sub testCnnInvalidKernel()
			assertThrows(GetType(System.InvalidOperationException), Sub()
			Call (New ConvolutionLayer.Builder()).kernelSize(3, 0).build()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testCnnInvalidKernel2()
		Public Overridable Sub testCnnInvalidKernel2()
			assertThrows(GetType(System.ArgumentException), Sub()
			Call (New ConvolutionLayer.Builder()).kernelSize(2, 2, 2).build()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testCnnInvalidStride()
		Public Overridable Sub testCnnInvalidStride()
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Call (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(0, 1).build()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testCnnInvalidStride2()
		Public Overridable Sub testCnnInvalidStride2()
			assertThrows(GetType(System.ArgumentException),Sub()
			Call (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(1).build()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testCnnInvalidPadding()
		Public Overridable Sub testCnnInvalidPadding()
			assertThrows(GetType(System.ArgumentException),Sub()
			Call (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(1, 1).padding(-1, 0).build()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testCnnInvalidPadding2()
		Public Overridable Sub testCnnInvalidPadding2()
			assertThrows(GetType(System.ArgumentException),Sub()
			Call (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(1, 1).padding(0, 0, 0).build()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testSubsamplingInvalidKernel()
		Public Overridable Sub testSubsamplingInvalidKernel()
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Call (New SubsamplingLayer.Builder()).kernelSize(3, 0).build()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testSubsamplingInvalidKernel2()
		Public Overridable Sub testSubsamplingInvalidKernel2()
			assertThrows(GetType(System.ArgumentException),Sub()
			Call (New SubsamplingLayer.Builder()).kernelSize(2).build()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testSubsamplingInvalidStride()
		Public Overridable Sub testSubsamplingInvalidStride()
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Call (New SubsamplingLayer.Builder()).kernelSize(3, 3).stride(0, 1).build()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testSubsamplingInvalidStride2()
		Public Overridable Sub testSubsamplingInvalidStride2()
			assertThrows(GetType(Exception),Sub()
			Call (New SubsamplingLayer.Builder()).kernelSize(3, 3).stride(1, 1, 1).build()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testSubsamplingInvalidPadding()
		Public Overridable Sub testSubsamplingInvalidPadding()
			assertThrows(GetType(System.ArgumentException),Sub()
			Call (New SubsamplingLayer.Builder()).kernelSize(3, 3).stride(1, 1).padding(-1, 0).build()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testSubsamplingInvalidPadding2()
		Public Overridable Sub testSubsamplingInvalidPadding2()
			assertThrows(GetType(Exception),Sub()
			Call (New SubsamplingLayer.Builder()).kernelSize(3, 3).stride(1, 1).padding(0).build()
			End Sub)
		End Sub

	End Class

End Namespace