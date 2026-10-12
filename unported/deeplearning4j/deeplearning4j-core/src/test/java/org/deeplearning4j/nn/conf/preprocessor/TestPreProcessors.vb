Imports System
Imports Microsoft.VisualBasic
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports FeedForwardLayer = org.deeplearning4j.nn.conf.layers.FeedForwardLayer
Imports GravesLSTM = org.deeplearning4j.nn.conf.layers.GravesLSTM
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports RnnOutputLayer = org.deeplearning4j.nn.conf.layers.RnnOutputLayer
Imports ConvolutionLayer = org.deeplearning4j.nn.layers.convolution.ConvolutionLayer
Imports DenseLayer = org.deeplearning4j.nn.layers.feedforward.dense.DenseLayer
Imports ReshapePreprocessor = org.deeplearning4j.nn.modelimport.keras.preprocessors.ReshapePreprocessor
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
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

Namespace org.deeplearning4j.nn.conf.preprocessor


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestPreProcessors extends org.deeplearning4j.BaseDL4JTest
	Public Class TestPreProcessors
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRnnToFeedForwardPreProcessor()
		Public Overridable Sub testRnnToFeedForwardPreProcessor()
			Dim miniBatchSizes() As Integer = {5, 1, 5, 1}
			Dim timeSeriesLengths() As Integer = {9, 9, 1, 1}

			For x As Integer = 0 To miniBatchSizes.Length - 1
				Dim miniBatchSize As Integer = miniBatchSizes(x)
				Dim layerSize As Integer = 7
				Dim timeSeriesLength As Integer = timeSeriesLengths(x)

				Dim proc As New RnnToFeedForwardPreProcessor()
				Dim nnc As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New org.deeplearning4j.nn.conf.layers.DenseLayer.Builder()).nIn(layerSize).nOut(layerSize).build()).build()

				Dim numParams As Long = nnc.getLayer().initializer().numParams(nnc)
				Dim params As INDArray = Nd4j.create(1, numParams)
				Dim layer As DenseLayer = CType(nnc.getLayer().instantiate(nnc, Nothing, 0, params, True, params.dataType()), DenseLayer)
				layer.InputMiniBatchSize = miniBatchSize

				Dim activations3dc As INDArray = Nd4j.create(New Integer() {miniBatchSize, layerSize, timeSeriesLength}, "c"c).castTo(params.dataType())
				Dim activations3df As INDArray = Nd4j.create(New Integer() {miniBatchSize, layerSize, timeSeriesLength}, "f"c).castTo(params.dataType())
				For i As Integer = 0 To miniBatchSize - 1
					For j As Integer = 0 To layerSize - 1
						For k As Integer = 0 To timeSeriesLength - 1
							Dim value As Double = 100 * i + 10 * j + k 'value abc -> example=a, neuronNumber=b, time=c
							activations3dc.putScalar(New Integer() {i, j, k}, value)
							activations3df.putScalar(New Integer() {i, j, k}, value)
						Next k
					Next j
				Next i
				assertEquals(activations3dc, activations3df)


				Dim activations2dc As INDArray = proc.preProcess(activations3dc, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
				Dim activations2df As INDArray = proc.preProcess(activations3df, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
				assertArrayEquals(activations2dc.shape(), New Long() {miniBatchSize * timeSeriesLength, layerSize})
				assertArrayEquals(activations2df.shape(), New Long() {miniBatchSize * timeSeriesLength, layerSize})
				assertEquals(activations2dc, activations2df)

				'Expect each row in activations2d to have order:
				'(example=0,t=0), (example=0,t=1), (example=0,t=2), ..., (example=1,t=0), (example=1,t=1), ...
				Dim nRows As Integer = activations2dc.rows()
				For i As Integer = 0 To nRows - 1
					Dim rowc As INDArray = activations2dc.getRow(i, True)
					Dim rowf As INDArray = activations2df.getRow(i, True)
					assertArrayEquals(rowc.shape(), New Long() {1, layerSize})
					assertEquals(rowc, rowf)

					'c order reshaping
					'                int origExampleNum = i / timeSeriesLength;
					'                int time = i % timeSeriesLength;
					'f order reshaping
					Dim time As Integer = i \ miniBatchSize
					Dim origExampleNum As Integer = i Mod miniBatchSize
					Dim expectedRow As INDArray = activations3dc.tensorAlongDimension(time, 1, 0).getRow(origExampleNum, True)
					assertEquals(expectedRow, rowc)
					assertEquals(expectedRow, rowf)
				Next i

				'Given that epsilons and activations have same shape, we can do this (even though it's not the intended use)
				'Basically backprop should be exact opposite of preProcess
				Dim outc As INDArray = proc.backprop(activations2dc, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
				Dim outf As INDArray = proc.backprop(activations2df, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
				assertEquals(activations3dc, outc)
				assertEquals(activations3df, outf)

				'Also check case when epsilons are different orders:
				Dim eps2d_c As INDArray = Nd4j.create(activations2dc.shape(), "c"c)
				Dim eps2d_f As INDArray = Nd4j.create(activations2dc.shape(), "f"c)
				eps2d_c.assign(activations2dc)
				eps2d_f.assign(activations2df)
				Dim eps3d_c As INDArray = proc.backprop(eps2d_c, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
				Dim eps3d_f As INDArray = proc.backprop(eps2d_f, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
				assertEquals(activations3dc, eps3d_c)
				assertEquals(activations3df, eps3d_f)
			Next x
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFeedForwardToRnnPreProcessor()
		Public Overridable Sub testFeedForwardToRnnPreProcessor()
			Nd4j.Random.Seed = 12345L

			Dim miniBatchSizes() As Integer = {5, 1, 5, 1}
			Dim timeSeriesLengths() As Integer = {9, 9, 1, 1}

			For x As Integer = 0 To miniBatchSizes.Length - 1
				Dim miniBatchSize As Integer = miniBatchSizes(x)
				Dim layerSize As Integer = 7
				Dim timeSeriesLength As Integer = timeSeriesLengths(x)

				Dim msg As String = "minibatch=" & miniBatchSize

				Dim proc As New FeedForwardToRnnPreProcessor()

				Dim nnc As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New org.deeplearning4j.nn.conf.layers.DenseLayer.Builder()).nIn(layerSize).nOut(layerSize).build()).build()

				Dim numParams As val = nnc.getLayer().initializer().numParams(nnc)
				Dim params As INDArray = Nd4j.create(1, numParams)
				Dim layer As DenseLayer = CType(nnc.getLayer().instantiate(nnc, Nothing, 0, params, True, params.dataType()), DenseLayer)
				layer.InputMiniBatchSize = miniBatchSize

				Dim rand As INDArray = Nd4j.rand(miniBatchSize * timeSeriesLength, layerSize)
				Dim activations2dc As INDArray = Nd4j.create(New Integer() {miniBatchSize * timeSeriesLength, layerSize}, "c"c)
				Dim activations2df As INDArray = Nd4j.create(New Integer() {miniBatchSize * timeSeriesLength, layerSize}, "f"c)
				activations2dc.assign(rand)
				activations2df.assign(rand)
				assertEquals(activations2dc, activations2df)

				Dim activations3dc As INDArray = proc.preProcess(activations2dc, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
				Dim activations3df As INDArray = proc.preProcess(activations2df, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
				assertArrayEquals(New Long() {miniBatchSize, layerSize, timeSeriesLength}, activations3dc.shape())
				assertArrayEquals(New Long() {miniBatchSize, layerSize, timeSeriesLength}, activations3df.shape())
				assertEquals(activations3dc, activations3df)

				Dim nRows2D As Integer = miniBatchSize * timeSeriesLength
				For i As Integer = 0 To nRows2D - 1
					'c order reshaping:
					'                int time = i % timeSeriesLength;
					'                int example = i / timeSeriesLength;
					'f order reshaping
					Dim time As Integer = i \ miniBatchSize
					Dim example As Integer = i Mod miniBatchSize

					Dim row2d As INDArray = activations2dc.getRow(i, True)
					Dim row3dc As INDArray = activations3dc.tensorAlongDimension(time, 0, 1).getRow(example, True)
					Dim row3df As INDArray = activations3df.tensorAlongDimension(time, 0, 1).getRow(example, True)

					assertEquals(row2d, row3dc)
					assertEquals(row2d, row3df)
				Next i

				'Again epsilons and activations have same shape, we can do this (even though it's not the intended use)
				Dim epsilon2d1 As INDArray = proc.backprop(activations3dc, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
				Dim epsilon2d2 As INDArray = proc.backprop(activations3df, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
				assertEquals(activations2dc, epsilon2d1, msg)
				assertEquals(activations2dc, epsilon2d2, msg)

				'Also check backprop with 3d activations in f order vs. c order:
				Dim act3d_c As INDArray = Nd4j.create(activations3dc.shape(), "c"c)
				act3d_c.assign(activations3dc)
				Dim act3d_f As INDArray = Nd4j.create(activations3dc.shape(), "f"c)
				act3d_f.assign(activations3dc)

				assertEquals(activations2dc, proc.backprop(act3d_c, miniBatchSize, LayerWorkspaceMgr.noWorkspaces()), msg)
				assertEquals(activations2dc, proc.backprop(act3d_f, miniBatchSize, LayerWorkspaceMgr.noWorkspaces()), msg)
			Next x
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnToRnnPreProcessor()
		Public Overridable Sub testCnnToRnnPreProcessor()
			'Two ways to test this:
			' (a) check that doing preProcess + backprop on a given input gives same result
			' (b) compare to ComposableInputPreProcessor(CNNtoFF, FFtoRNN)

			Dim miniBatchSizes() As Integer = {5, 1}
			Dim timeSeriesLengths() As Integer = {9, 1}
			Dim inputHeights() As Integer = {10, 30}
			Dim inputWidths() As Integer = {10, 30}
			Dim numChannels() As Integer = {1, 3, 6}
			Dim cnnNChannelsIn As Integer = 3

			Nd4j.Random.setSeed(12345)

			For Each miniBatchSize As Integer In miniBatchSizes
				For Each timeSeriesLength As Integer In timeSeriesLengths
					For Each inputHeight As Integer In inputHeights
						For Each inputWidth As Integer In inputWidths
							For Each nChannels As Integer In numChannels

								Dim msg As String = "miniBatch=" & miniBatchSize & ", tsLength=" & timeSeriesLength & ", h=" & inputHeight & ", w=" & inputWidth & ", ch=" & nChannels

								Dim proc As InputPreProcessor = New CnnToRnnPreProcessor(inputHeight, inputWidth, nChannels)

								Dim nnc As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New org.deeplearning4j.nn.conf.layers.ConvolutionLayer.Builder(inputWidth, inputHeight)).nIn(cnnNChannelsIn).nOut(nChannels).build()).build()

								Dim numParams As val = nnc.getLayer().initializer().numParams(nnc)
								Dim params As INDArray = Nd4j.create(1, numParams)
								Dim layer As ConvolutionLayer = CType(nnc.getLayer().instantiate(nnc, Nothing, 0, params, True, params.dataType()), ConvolutionLayer)
								layer.InputMiniBatchSize = miniBatchSize

								Dim activationsCnn As INDArray = Nd4j.rand(New Integer() {miniBatchSize * timeSeriesLength, nChannels, inputHeight, inputWidth})

								'Check shape of outputs:
								Dim prod As val = nChannels * inputHeight * inputWidth
								Dim activationsRnn As INDArray = proc.preProcess(activationsCnn, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
								assertArrayEquals(New Long() {miniBatchSize, prod, timeSeriesLength}, activationsRnn.shape(),msg)

								'Check backward pass. Given that activations and epsilons have same shape, they should
								'be opposite operations - i.e., get the same thing back out
								Dim twiceProcessed As INDArray = proc.backprop(activationsRnn, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
								assertArrayEquals(activationsCnn.shape(), twiceProcessed.shape(),msg)
								assertEquals(activationsCnn, twiceProcessed, msg)

								'Second way to check: compare to ComposableInputPreProcessor(CNNtoFF, FFtoRNN)
								Dim compProc As InputPreProcessor = New ComposableInputPreProcessor(New CnnToFeedForwardPreProcessor(inputHeight, inputWidth, nChannels), New FeedForwardToRnnPreProcessor())

								Dim activationsRnnComp As INDArray = compProc.preProcess(activationsCnn, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
								assertEquals(activationsRnnComp, activationsRnn, msg)

								Dim epsilonsRnn As INDArray = Nd4j.rand(New Integer() {miniBatchSize, nChannels * inputHeight * inputWidth, timeSeriesLength})
								Dim epsilonsCnnComp As INDArray = compProc.backprop(epsilonsRnn, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
								Dim epsilonsCnn As INDArray = proc.backprop(epsilonsRnn, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
								If Not epsilonsCnn.Equals(epsilonsCnnComp) Then
									Console.WriteLine(miniBatchSize & vbTab & timeSeriesLength & vbTab & inputHeight & vbTab & inputWidth & vbTab & nChannels)
									Console.WriteLine("expected - epsilonsCnnComp")
									Console.WriteLine(Arrays.toString(epsilonsCnnComp.shape()))
									Console.WriteLine(epsilonsCnnComp)
									Console.WriteLine("actual - epsilonsCnn")
									Console.WriteLine(Arrays.toString(epsilonsCnn.shape()))
									Console.WriteLine(epsilonsCnn)
								End If
								assertEquals(epsilonsCnnComp, epsilonsCnn, msg)
							Next nChannels
						Next inputWidth
					Next inputHeight
				Next timeSeriesLength
			Next miniBatchSize
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRnnToCnnPreProcessor()
		Public Overridable Sub testRnnToCnnPreProcessor()
			'Two ways to test this:
			' (a) check that doing preProcess + backprop on a given input gives same result
			' (b) compare to ComposableInputPreProcessor(CNNtoFF, FFtoRNN)

			Dim miniBatchSizes() As Integer = {5, 1}
			Dim timeSeriesLengths() As Integer = {9, 1}
			Dim inputHeights() As Integer = {10, 30}
			Dim inputWidths() As Integer = {10, 30}
			Dim numChannels() As Integer = {1, 3, 6}
			Dim cnnNChannelsIn As Integer = 3

			Nd4j.Random.setSeed(12345)

			Console.WriteLine()
			For Each miniBatchSize As Integer In miniBatchSizes
				For Each timeSeriesLength As Integer In timeSeriesLengths
					For Each inputHeight As Integer In inputHeights
						For Each inputWidth As Integer In inputWidths
							For Each nChannels As Integer In numChannels
								Dim proc As InputPreProcessor = New RnnToCnnPreProcessor(inputHeight, inputWidth, nChannels)

								Dim nnc As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New org.deeplearning4j.nn.conf.layers.ConvolutionLayer.Builder(inputWidth, inputHeight)).nIn(cnnNChannelsIn).nOut(nChannels).build()).build()

								Dim numParams As val = nnc.getLayer().initializer().numParams(nnc)
								Dim params As INDArray = Nd4j.create(1, numParams)
								Dim layer As ConvolutionLayer = CType(nnc.getLayer().instantiate(nnc, Nothing, 0, params, True, params.dataType()), ConvolutionLayer)
								layer.InputMiniBatchSize = miniBatchSize

								Dim shape_rnn As val = New Long() {miniBatchSize, nChannels * inputHeight * inputWidth, timeSeriesLength}
								Dim rand As INDArray = Nd4j.rand(shape_rnn)
								Dim activationsRnn_c As INDArray = Nd4j.create(shape_rnn, "c"c)
								Dim activationsRnn_f As INDArray = Nd4j.create(shape_rnn, "f"c)
								activationsRnn_c.assign(rand)
								activationsRnn_f.assign(rand)
								assertEquals(activationsRnn_c, activationsRnn_f)

								'Check shape of outputs:
								Dim activationsCnn_c As INDArray = proc.preProcess(activationsRnn_c, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
								Dim activationsCnn_f As INDArray = proc.preProcess(activationsRnn_f, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
								Dim shape_cnn As val = New Long() {miniBatchSize * timeSeriesLength, nChannels, inputHeight, inputWidth}
								assertArrayEquals(shape_cnn, activationsCnn_c.shape())
								assertArrayEquals(shape_cnn, activationsCnn_f.shape())
								assertEquals(activationsCnn_c, activationsCnn_f)

								'Check backward pass. Given that activations and epsilons have same shape, they should
								'be opposite operations - i.e., get the same thing back out
								Dim twiceProcessed_c As INDArray = proc.backprop(activationsCnn_c, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
								Dim twiceProcessed_f As INDArray = proc.backprop(activationsCnn_c, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
								assertArrayEquals(shape_rnn, twiceProcessed_c.shape())
								assertArrayEquals(shape_rnn, twiceProcessed_f.shape())
								assertEquals(activationsRnn_c, twiceProcessed_c)
								assertEquals(activationsRnn_c, twiceProcessed_f)

								'Second way to check: compare to ComposableInputPreProcessor(RNNtoFF, FFtoCNN)
								Dim compProc As InputPreProcessor = New ComposableInputPreProcessor(New RnnToFeedForwardPreProcessor(), New FeedForwardToCnnPreProcessor(inputHeight, inputWidth, nChannels))

								Dim activationsCnnComp_c As INDArray = compProc.preProcess(activationsRnn_c, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
								Dim activationsCnnComp_f As INDArray = compProc.preProcess(activationsRnn_f, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
								assertEquals(activationsCnnComp_c, activationsCnn_c)
								assertEquals(activationsCnnComp_f, activationsCnn_f)

								Dim epsilonShape() As Integer = {miniBatchSize * timeSeriesLength, nChannels, inputHeight, inputWidth}
								rand = Nd4j.rand(epsilonShape)
								Dim epsilonsCnn_c As INDArray = Nd4j.create(epsilonShape, "c"c)
								Dim epsilonsCnn_f As INDArray = Nd4j.create(epsilonShape, "f"c)
								epsilonsCnn_c.assign(rand)
								epsilonsCnn_f.assign(rand)

								Dim epsilonsRnnComp_c As INDArray = compProc.backprop(epsilonsCnn_c, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
								Dim epsilonsRnnComp_f As INDArray = compProc.backprop(epsilonsCnn_f, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
								assertEquals(epsilonsRnnComp_c, epsilonsRnnComp_f)
								Dim epsilonsRnn_c As INDArray = proc.backprop(epsilonsCnn_c, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
								Dim epsilonsRnn_f As INDArray = proc.backprop(epsilonsCnn_f, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
								assertEquals(epsilonsRnn_c, epsilonsRnn_f)

								If Not epsilonsRnn_c.Equals(epsilonsRnnComp_c) Then
									Console.WriteLine(miniBatchSize & vbTab & timeSeriesLength & vbTab & inputHeight & vbTab & inputWidth & vbTab & nChannels)
									Console.WriteLine("expected - epsilonsRnnComp")
									Console.WriteLine(Arrays.toString(epsilonsRnnComp_c.shape()))
									Console.WriteLine(epsilonsRnnComp_c)
									Console.WriteLine("actual - epsilonsRnn")
									Console.WriteLine(Arrays.toString(epsilonsRnn_c.shape()))
									Console.WriteLine(epsilonsRnn_c)
								End If
								assertEquals(epsilonsRnnComp_c, epsilonsRnn_c)
								assertEquals(epsilonsRnnComp_c, epsilonsRnn_f)
							Next nChannels
						Next inputWidth
					Next inputHeight
				Next timeSeriesLength
			Next miniBatchSize
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAutoAdditionOfPreprocessors()
		Public Overridable Sub testAutoAdditionOfPreprocessors()
			'FF->RNN and RNN->FF
			Dim conf1 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New org.deeplearning4j.nn.conf.layers.DenseLayer.Builder()).nIn(5).nOut(6).build()).layer(1, (New GravesLSTM.Builder()).nIn(6).nOut(7).build()).layer(2, (New org.deeplearning4j.nn.conf.layers.DenseLayer.Builder()).nIn(7).nOut(8).build()).layer(3, (New RnnOutputLayer.Builder()).nIn(8).nOut(9).activation(Activation.SOFTMAX).build()).build()
			'Expect preprocessors: layer1: FF->RNN; 2: RNN->FF; 3: FF->RNN
			assertEquals(3, conf1.getInputPreProcessors().size())
			assertTrue(TypeOf conf1.getInputPreProcess(1) Is FeedForwardToRnnPreProcessor)
			assertTrue(TypeOf conf1.getInputPreProcess(2) Is RnnToFeedForwardPreProcessor)
			assertTrue(TypeOf conf1.getInputPreProcess(3) Is FeedForwardToRnnPreProcessor)


			'FF-> CNN, CNN-> FF, FF->RNN
			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New org.deeplearning4j.nn.conf.layers.ConvolutionLayer.Builder()).nOut(10).kernelSize(5, 5).stride(1, 1).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.DenseLayer.Builder()).nOut(6).build()).layer(2, (New RnnOutputLayer.Builder()).nIn(6).nOut(5).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()
			'Expect preprocessors: 0: FF->CNN; 1: CNN->FF; 2: FF->RNN
			assertEquals(3, conf2.getInputPreProcessors().size())
			assertTrue(TypeOf conf2.getInputPreProcess(0) Is FeedForwardToCnnPreProcessor)
			assertTrue(TypeOf conf2.getInputPreProcess(1) Is CnnToFeedForwardPreProcessor)
			assertTrue(TypeOf conf2.getInputPreProcess(2) Is FeedForwardToRnnPreProcessor)

			'CNN-> FF, FF->RNN - InputType.convolutional instead of convolutionalFlat
			Dim conf2a As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New org.deeplearning4j.nn.conf.layers.ConvolutionLayer.Builder()).nOut(10).kernelSize(5, 5).stride(1, 1).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.DenseLayer.Builder()).nOut(6).build()).layer(2, (New RnnOutputLayer.Builder()).nIn(6).nOut(5).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(28, 28, 1)).build()
			'Expect preprocessors: 1: CNN->FF; 2: FF->RNN
			assertEquals(2, conf2a.getInputPreProcessors().size())
			assertTrue(TypeOf conf2a.getInputPreProcess(1) Is CnnToFeedForwardPreProcessor)
			assertTrue(TypeOf conf2a.getInputPreProcess(2) Is FeedForwardToRnnPreProcessor)


			'FF->CNN and CNN->RNN:
			Dim conf3 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New org.deeplearning4j.nn.conf.layers.ConvolutionLayer.Builder()).nOut(10).kernelSize(5, 5).stride(1, 1).build()).layer(1, (New GravesLSTM.Builder()).nOut(6).build()).layer(2, (New RnnOutputLayer.Builder()).nIn(6).nOut(5).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()
			'Expect preprocessors: 0: FF->CNN, 1: CNN->RNN;
			assertEquals(2, conf3.getInputPreProcessors().size())
			assertTrue(TypeOf conf3.getInputPreProcess(0) Is FeedForwardToCnnPreProcessor)
			assertTrue(TypeOf conf3.getInputPreProcess(1) Is CnnToRnnPreProcessor)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnToDense()
		Public Overridable Sub testCnnToDense()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New org.deeplearning4j.nn.conf.layers.ConvolutionLayer.Builder(4, 4)).nIn(1).nOut(10).padding(2, 2).stride(2, 2).weightInit(WeightInit.RELU).activation(Activation.RELU).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.DenseLayer.Builder()).activation(Activation.RELU).nOut(200).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(200).nOut(5).weightInit(WeightInit.RELU).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()

			assertNotNull(conf.getInputPreProcess(0))
			assertNotNull(conf.getInputPreProcess(1))

			assertTrue(TypeOf conf.getInputPreProcess(0) Is FeedForwardToCnnPreProcessor)
			assertTrue(TypeOf conf.getInputPreProcess(1) Is CnnToFeedForwardPreProcessor)

			Dim ffcnn As FeedForwardToCnnPreProcessor = DirectCast(conf.getInputPreProcess(0), FeedForwardToCnnPreProcessor)
			Dim cnnff As CnnToFeedForwardPreProcessor = DirectCast(conf.getInputPreProcess(1), CnnToFeedForwardPreProcessor)

			assertEquals(28, ffcnn.getInputHeight())
			assertEquals(28, ffcnn.getInputWidth())
			assertEquals(1, ffcnn.getNumChannels())

			assertEquals(15, cnnff.getInputHeight())
			assertEquals(15, cnnff.getInputWidth())
			assertEquals(10, cnnff.getNumChannels())

			assertEquals(15 * 15 * 10, CType(conf.getConf(1).getLayer(), FeedForwardLayer).getNIn())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPreprocessorVertex()
		Public Overridable Sub testPreprocessorVertex()
			For Each withMinibatchDim As Boolean In New Boolean(){True, False}
				Dim inShape() As Long = If(withMinibatchDim, New Long(){-1, 32}, New Long()){32}
				Dim targetShape() As Long = If(withMinibatchDim, New Long(){-1, 2, 4, 4}, New Long()){2, 4, 4}

				For Each minibatch As Long In New Long(){1, 3}
					Dim inArrayShape() As Long = {minibatch, 32}
					Dim targetArrayShape() As Long = {minibatch, 2, 4, 4}
					Dim length As Long = minibatch * 32

					Dim [in] As INDArray = Nd4j.linspace(1, length, length).reshape("c"c, inArrayShape)

					Dim pp As New ReshapePreprocessor(inShape, targetShape, withMinibatchDim)

					For i As Integer = 0 To 2
						Dim [out] As INDArray = pp.preProcess([in], CInt(minibatch), LayerWorkspaceMgr.noWorkspaces())
						Dim expOut As INDArray = [in].reshape(targetArrayShape)
						assertEquals(expOut, [out])

						Dim backprop As INDArray = pp.backprop(expOut, CInt(minibatch), LayerWorkspaceMgr.noWorkspaces())
						assertEquals([in], backprop)
					Next i
				Next minibatch
			Next withMinibatchDim
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPreprocessorVertex3d()
		Public Overridable Sub testPreprocessorVertex3d()
			For Each withMinibatchDim As Boolean In New Boolean(){True, False}
				Dim inShape() As Long = If(withMinibatchDim, New Long(){-1, 64}, New Long()){64}
				Dim targetShape() As Long = If(withMinibatchDim, New Long(){-1, 2, 4, 4, 2}, New Long()){2, 4, 4,2}

				For Each minibatch As Long In New Long(){1, 3}
					Dim inArrayShape() As Long = {minibatch, 64}
					Dim targetArrayShape() As Long = {minibatch, 2, 4, 4, 2}
					Dim length As Long = minibatch * 64

					Dim [in] As INDArray = Nd4j.linspace(1, length, length).reshape("c"c, inArrayShape)

					Dim pp As New ReshapePreprocessor(inShape, targetShape, withMinibatchDim)

					For i As Integer = 0 To 2
						Dim [out] As INDArray = pp.preProcess([in], CInt(minibatch), LayerWorkspaceMgr.noWorkspaces())
						Dim expOut As INDArray = [in].reshape(targetArrayShape)
						assertEquals(expOut, [out])

						Dim backprop As INDArray = pp.backprop(expOut, CInt(minibatch), LayerWorkspaceMgr.noWorkspaces())
						assertEquals([in], backprop)
					Next i
				Next minibatch
			Next withMinibatchDim
		End Sub
	End Class

End Namespace