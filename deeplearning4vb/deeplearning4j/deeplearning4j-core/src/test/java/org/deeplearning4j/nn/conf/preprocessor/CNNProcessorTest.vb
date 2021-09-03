Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports org.deeplearning4j.nn.conf
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports org.junit.jupiter.api.Assertions
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
Namespace org.deeplearning4j.nn.conf.preprocessor

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Cnn Processor Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class CNNProcessorTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class CNNProcessorTest
		Inherits BaseDL4JTest

		Private Shared rows As Integer = 28

		Private Shared cols As Integer = 28

		Private Shared in2D As INDArray = Nd4j.create(DataType.FLOAT, 1, 784)

		Private Shared in3D As INDArray = Nd4j.create(DataType.FLOAT, 20, 784, 7)

		Private Shared in4D As INDArray = Nd4j.create(DataType.FLOAT, 20, 1, 28, 28)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Feed Forward To Cnn Pre Processor") void testFeedForwardToCnnPreProcessor()
		Friend Overridable Sub testFeedForwardToCnnPreProcessor()
			Dim convProcessor As New FeedForwardToCnnPreProcessor(rows, cols, 1)
			Dim check2to4 As INDArray = convProcessor.preProcess(in2D, -1, LayerWorkspaceMgr.noWorkspaces())
			Dim val2to4 As Integer = check2to4.shape().Length
			assertTrue(val2to4 = 4)
			assertEquals(Nd4j.create(DataType.FLOAT, 1, 1, 28, 28), check2to4)
			Dim check4to4 As INDArray = convProcessor.preProcess(in4D, -1, LayerWorkspaceMgr.noWorkspaces())
			Dim val4to4 As Integer = check4to4.shape().Length
			assertTrue(val4to4 = 4)
			assertEquals(Nd4j.create(DataType.FLOAT, 20, 1, 28, 28), check4to4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Feed Forward To Cnn Pre Processor 2") void testFeedForwardToCnnPreProcessor2()
		Friend Overridable Sub testFeedForwardToCnnPreProcessor2()
			Dim nRows() As Integer = { 1, 5, 20 }
			Dim nCols() As Integer = { 1, 5, 20 }
			Dim nDepth() As Integer = { 1, 3 }
			Dim nMiniBatchSize() As Integer = { 1, 5 }
			For Each rows As Integer In nRows
				For Each cols As Integer In nCols
					For Each d As Integer In nDepth
						Dim convProcessor As New FeedForwardToCnnPreProcessor(rows, cols, d)
						For Each miniBatch As Integer In nMiniBatchSize
							Dim ffShape() As Long = { miniBatch, rows * cols * d }
							Dim rand As INDArray = Nd4j.rand(ffShape)
							Dim ffInput_c As INDArray = Nd4j.create(DataType.FLOAT, ffShape, "c"c)
							Dim ffInput_f As INDArray = Nd4j.create(DataType.FLOAT, ffShape, "f"c)
							ffInput_c.assign(rand)
							ffInput_f.assign(rand)
							assertEquals(ffInput_c, ffInput_f)
							' Test forward pass:
							Dim convAct_c As INDArray = convProcessor.preProcess(ffInput_c, -1, LayerWorkspaceMgr.noWorkspaces())
							Dim convAct_f As INDArray = convProcessor.preProcess(ffInput_f, -1, LayerWorkspaceMgr.noWorkspaces())
							Dim convShape() As Long = { miniBatch, d, rows, cols }
							assertArrayEquals(convShape, convAct_c.shape())
							assertArrayEquals(convShape, convAct_f.shape())
							assertEquals(convAct_c, convAct_f)
							' Check values:
							' CNN reshaping (for each example) takes a 1d vector and converts it to 3d
							' (4d total, for minibatch data)
							' 1d vector is assumed to be rows from channels 0 concatenated, followed by channels 1, etc
							For ex As Integer = 0 To miniBatch - 1
								For r As Integer = 0 To rows - 1
									For c As Integer = 0 To cols - 1
										For depth As Integer = 0 To d - 1
											' pos in vector
											Dim origPosition As Integer = depth * (rows * cols) + r * cols + c
											Dim vecValue As Double = ffInput_c.getDouble(ex, origPosition)
											Dim convValue As Double = convAct_c.getDouble(ex, depth, r, c)
											assertEquals(vecValue, convValue, 0.0)
										Next depth
									Next c
								Next r
							Next ex
							' Test backward pass:
							' Idea is that backward pass should do opposite to forward pass
							Dim epsilon4_c As INDArray = Nd4j.create(DataType.FLOAT, convShape, "c"c)
							Dim epsilon4_f As INDArray = Nd4j.create(DataType.FLOAT, convShape, "f"c)
							epsilon4_c.assign(convAct_c)
							epsilon4_f.assign(convAct_f)
							Dim epsilon2_c As INDArray = convProcessor.backprop(epsilon4_c, -1, LayerWorkspaceMgr.noWorkspaces())
							Dim epsilon2_f As INDArray = convProcessor.backprop(epsilon4_f, -1, LayerWorkspaceMgr.noWorkspaces())
							assertEquals(ffInput_c, epsilon2_c)
							assertEquals(ffInput_c, epsilon2_f)
						Next miniBatch
					Next d
				Next cols
			Next rows
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Feed Forward To Cnn Pre Processor Backprop") void testFeedForwardToCnnPreProcessorBackprop()
		Friend Overridable Sub testFeedForwardToCnnPreProcessorBackprop()
			Dim convProcessor As New FeedForwardToCnnPreProcessor(rows, cols, 1)
			convProcessor.preProcess(in2D, -1, LayerWorkspaceMgr.noWorkspaces())
			Dim check2to2 As INDArray = convProcessor.backprop(in2D, -1, LayerWorkspaceMgr.noWorkspaces())
			Dim val2to2 As Integer = check2to2.shape().Length
			assertTrue(val2to2 = 2)
			assertEquals(Nd4j.create(DataType.FLOAT, 1, 784), check2to2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn To Feed Forward Processor") void testCnnToFeedForwardProcessor()
		Friend Overridable Sub testCnnToFeedForwardProcessor()
			Dim convProcessor As New CnnToFeedForwardPreProcessor(rows, cols, 1)
			Dim check2to4 As INDArray = convProcessor.backprop(in2D, -1, LayerWorkspaceMgr.noWorkspaces())
			Dim val2to4 As Integer = check2to4.shape().Length
			assertTrue(val2to4 = 4)
			assertEquals(Nd4j.create(DataType.FLOAT, 1, 1, 28, 28), check2to4)
			Dim check4to4 As INDArray = convProcessor.backprop(in4D, -1, LayerWorkspaceMgr.noWorkspaces())
			Dim val4to4 As Integer = check4to4.shape().Length
			assertTrue(val4to4 = 4)
			assertEquals(Nd4j.create(DataType.FLOAT, 20, 1, 28, 28), check4to4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn To Feed Forward Pre Processor Backprop") void testCnnToFeedForwardPreProcessorBackprop()
		Friend Overridable Sub testCnnToFeedForwardPreProcessorBackprop()
			Dim convProcessor As New CnnToFeedForwardPreProcessor(rows, cols, 1)
			convProcessor.preProcess(in4D, -1, LayerWorkspaceMgr.noWorkspaces())
			Dim check2to2 As INDArray = convProcessor.preProcess(in2D, -1, LayerWorkspaceMgr.noWorkspaces())
			Dim val2to2 As Integer = check2to2.shape().Length
			assertTrue(val2to2 = 2)
			assertEquals(Nd4j.create(DataType.FLOAT, 1, 784), check2to2)
			Dim check4to2 As INDArray = convProcessor.preProcess(in4D, -1, LayerWorkspaceMgr.noWorkspaces())
			Dim val4to2 As Integer = check4to2.shape().Length
			assertTrue(val4to2 = 2)
			assertEquals(Nd4j.create(DataType.FLOAT, 20, 784), check4to2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn To Feed Forward Pre Processor 2") void testCnnToFeedForwardPreProcessor2()
		Friend Overridable Sub testCnnToFeedForwardPreProcessor2()
			Dim nRows() As Integer = { 1, 5, 20 }
			Dim nCols() As Integer = { 1, 5, 20 }
			Dim nDepth() As Integer = { 1, 3 }
			Dim nMiniBatchSize() As Integer = { 1, 5 }
			For Each rows As Integer In nRows
				For Each cols As Integer In nCols
					For Each d As Integer In nDepth
						Dim convProcessor As New CnnToFeedForwardPreProcessor(rows, cols, d)
						For Each miniBatch As Integer In nMiniBatchSize
							Dim convActShape() As Long = { miniBatch, d, rows, cols }
							Dim rand As INDArray = Nd4j.rand(convActShape)
							Dim convInput_c As INDArray = Nd4j.create(DataType.FLOAT, convActShape, "c"c)
							Dim convInput_f As INDArray = Nd4j.create(DataType.FLOAT, convActShape, "f"c)
							convInput_c.assign(rand)
							convInput_f.assign(rand)
							assertEquals(convInput_c, convInput_f)
							' Test forward pass:
							Dim ffAct_c As INDArray = convProcessor.preProcess(convInput_c, -1, LayerWorkspaceMgr.noWorkspaces())
							Dim ffAct_f As INDArray = convProcessor.preProcess(convInput_f, -1, LayerWorkspaceMgr.noWorkspaces())
							Dim ffActShape() As Long = { miniBatch, d * rows * cols }
							assertArrayEquals(ffActShape, ffAct_c.shape())
							assertArrayEquals(ffActShape, ffAct_f.shape())
							assertEquals(ffAct_c, ffAct_f)
							' Check values:
							' CNN reshaping (for each example) takes a 1d vector and converts it to 3d
							' (4d total, for minibatch data)
							' 1d vector is assumed to be rows from channels 0 concatenated, followed by channels 1, etc
							For ex As Integer = 0 To miniBatch - 1
								For r As Integer = 0 To rows - 1
									For c As Integer = 0 To cols - 1
										For depth As Integer = 0 To d - 1
											' pos in vector after reshape
											Dim vectorPosition As Integer = depth * (rows * cols) + r * cols + c
											Dim vecValue As Double = ffAct_c.getDouble(ex, vectorPosition)
											Dim convValue As Double = convInput_c.getDouble(ex, depth, r, c)
											assertEquals(convValue, vecValue, 0.0)
										Next depth
									Next c
								Next r
							Next ex
							' Test backward pass:
							' Idea is that backward pass should do opposite to forward pass
							Dim epsilon2_c As INDArray = Nd4j.create(DataType.FLOAT, ffActShape, "c"c)
							Dim epsilon2_f As INDArray = Nd4j.create(DataType.FLOAT, ffActShape, "f"c)
							epsilon2_c.assign(ffAct_c)
							epsilon2_f.assign(ffAct_c)
							Dim epsilon4_c As INDArray = convProcessor.backprop(epsilon2_c, -1, LayerWorkspaceMgr.noWorkspaces())
							Dim epsilon4_f As INDArray = convProcessor.backprop(epsilon2_f, -1, LayerWorkspaceMgr.noWorkspaces())
							assertEquals(convInput_c, epsilon4_c)
							assertEquals(convInput_c, epsilon4_f)
						Next miniBatch
					Next d
				Next cols
			Next rows
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Invalid Input Shape") void testInvalidInputShape()
		Friend Overridable Sub testInvalidInputShape()
			Dim builder As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(123).miniBatch(True).cacheMode(CacheMode.DEVICE).updater(New Nesterovs(0.9)).gradientNormalization(GradientNormalization.RenormalizeL2PerLayer).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT)
			Dim kernelArray() As Integer = { 3, 3 }
			Dim strideArray() As Integer = { 1, 1 }
			Dim zeroPaddingArray() As Integer = { 0, 0 }
			Dim processWidth As Integer = 4
			' Building the DL4J network
			Dim listBuilder As NeuralNetConfiguration.ListBuilder = builder.list()
			listBuilder = listBuilder.layer(0, (New ConvolutionLayer.Builder(kernelArray, strideArray, zeroPaddingArray)).name("cnn1").convolutionMode(ConvolutionMode.Strict).nIn(2).nOut(processWidth).weightInit(WeightInit.XAVIER_UNIFORM).activation(Activation.RELU).biasInit(1e-2).build())
			listBuilder = listBuilder.layer(1, (New ConvolutionLayer.Builder(kernelArray, strideArray, zeroPaddingArray)).name("cnn2").convolutionMode(ConvolutionMode.Strict).nOut(processWidth).weightInit(WeightInit.XAVIER_UNIFORM).activation(Activation.RELU).biasInit(1e-2).build())
			listBuilder = listBuilder.layer(2, (New ConvolutionLayer.Builder(kernelArray, strideArray, zeroPaddingArray)).name("cnn3").convolutionMode(ConvolutionMode.Strict).nOut(processWidth).weightInit(WeightInit.XAVIER_UNIFORM).activation(Activation.RELU).build())
			listBuilder = listBuilder.layer(3, (New ConvolutionLayer.Builder(kernelArray, strideArray, zeroPaddingArray)).name("cnn4").convolutionMode(ConvolutionMode.Strict).nOut(processWidth).weightInit(WeightInit.XAVIER_UNIFORM).activation(Activation.RELU).build())
			listBuilder = listBuilder.layer(4, (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).name("output").nOut(1).activation(Activation.TANH).build())
			Dim conf As MultiLayerConfiguration = listBuilder.setInputType(InputType.convolutional(20, 10, 2)).build()
			' For some reason, this model works
			Dim niceModel As New MultiLayerNetwork(conf)
			niceModel.init()
			' Valid
			niceModel.output(Nd4j.create(DataType.FLOAT, 1, 2, 20, 10))
			Try
				niceModel.output(Nd4j.create(DataType.FLOAT, 1, 2, 10, 20))
				fail("Expected exception")
			Catch e As System.InvalidOperationException
				' OK
			End Try
		End Sub
	End Class

End Namespace