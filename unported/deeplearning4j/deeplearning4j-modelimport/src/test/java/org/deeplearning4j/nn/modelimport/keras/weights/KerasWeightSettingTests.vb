Imports System
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports KerasModel = org.deeplearning4j.nn.modelimport.keras.KerasModel
Imports KerasSpaceToDepth = org.deeplearning4j.nn.modelimport.keras.layers.convolutional.KerasSpaceToDepth
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Resources = org.nd4j.common.resources.Resources
import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.deeplearning4j.nn.modelimport.keras.weights


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag public class KerasWeightSettingTests extends org.deeplearning4j.BaseDL4JTest
	Public Class KerasWeightSettingTests
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 9999999L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSimpleLayersWithWeights(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSimpleLayersWithWeights(ByVal tempDir As Path)
			Dim kerasVersions() As Integer = {1, 2}
			Dim backends() As String = {"tensorflow", "theano"}

			For Each version As Integer In kerasVersions
				For Each backend As String In backends
					Dim densePath As String = "modelimport/keras/weights/dense_" & backend & "_" & version & ".h5"
					importDense(tempDir,densePath)

					Dim conv2dPath As String = "modelimport/keras/weights/conv2d_" & backend & "_" & version & ".h5"
					importConv2D(tempDir,conv2dPath)

					If version = 2 AndAlso backend.Equals("tensorflow") Then ' TODO should work for theano
						Dim conv2dReshapePath As String = "modelimport/keras/weights/conv2d_reshape_" & backend & "_" & version & ".h5"
						Console.WriteLine(backend & "_" & version)
						importConv2DReshape(tempDir,conv2dReshapePath)
					End If

					If version = 2 Then
						Dim conv1dFlattenPath As String = "modelimport/keras/weights/embedding_conv1d_flatten_" & backend & "_" & version & ".h5"
						importConv1DFlatten(tempDir,conv1dFlattenPath)
					End If

					Dim lstmPath As String = "modelimport/keras/weights/lstm_" & backend & "_" & version & ".h5"
					importLstm(tempDir,lstmPath)

					Dim embeddingLstmPath As String = "modelimport/keras/weights/embedding_lstm_" & backend & "_" & version & ".h5"
					importEmbeddingLstm(tempDir,embeddingLstmPath)


					If version = 2 Then
						Dim embeddingConv1dExtendedPath As String = "modelimport/keras/weights/embedding_conv1d_extended_" & backend & "_" & version & ".h5"
						importEmbeddingConv1DExtended(tempDir,embeddingConv1dExtendedPath)
					End If

					If version = 2 Then
						Dim embeddingConv1dPath As String = "modelimport/keras/weights/embedding_conv1d_" & backend & "_" & version & ".h5"
						importEmbeddingConv1D(tempDir,embeddingConv1dPath)
					End If

					Dim simpleRnnPath As String = "modelimport/keras/weights/simple_rnn_" & backend & "_" & version & ".h5"
					importSimpleRnn(tempDir,simpleRnnPath)

					Dim bidirectionalLstmPath As String = "modelimport/keras/weights/bidirectional_lstm_" & backend & "_" & version & ".h5"
					importBidirectionalLstm(tempDir,bidirectionalLstmPath)

					Dim bidirectionalLstmNoSequencesPath As String = "modelimport/keras/weights/bidirectional_lstm_no_return_sequences_" & backend & "_" & version & ".h5"
					importBidirectionalLstm(tempDir,bidirectionalLstmNoSequencesPath)

					If version = 2 AndAlso backend.Equals("tensorflow") Then
						Dim batchToConv2dPath As String = "modelimport/keras/weights/batch_to_conv2d_" & backend & "_" & version & ".h5"
						importBatchNormToConv2D(tempDir,batchToConv2dPath)
					End If

					If backend.Equals("tensorflow") AndAlso version = 2 Then ' TODO should work for theano
						Dim simpleSpaceToBatchPath As String = "modelimport/keras/weights/space_to_depth_simple_" & backend & "_" & version & ".h5"
						importSimpleSpaceToDepth(tempDir,simpleSpaceToBatchPath)
					End If

					If backend.Equals("tensorflow") AndAlso version = 2 Then
						Dim graphSpaceToBatchPath As String = "modelimport/keras/weights/space_to_depth_graph_" & backend & "_" & version & ".h5"
						importGraphSpaceToDepth(tempDir,graphSpaceToBatchPath)
					End If

					If backend.Equals("tensorflow") AndAlso version = 2 Then
						Dim sepConvPath As String = "modelimport/keras/weights/sepconv2d_" & backend & "_" & version & ".h5"
						importSepConv2D(tempDir,sepConvPath)
					End If
				Next backend
			Next version
		End Sub

		Private Sub logSuccess(ByVal modelPath As String)
			log.info("***** Successfully imported " & modelPath)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void importDense(java.nio.file.Path tempDir,String modelPath) throws Exception
		Private Sub importDense(ByVal tempDir As Path, ByVal modelPath As String)
			Dim model As MultiLayerNetwork = loadMultiLayerNetwork(tempDir,modelPath, True)

			Dim weights As INDArray = model.getLayer(0).getParam("W")
			Dim weightShape As val = weights.shape()
			assertEquals(4, weightShape(0))
			assertEquals(6, weightShape(1))

			Dim bias As INDArray = model.getLayer(0).getParam("b")
			assertEquals(6, bias.length())
			logSuccess(modelPath)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void importSepConv2D(java.nio.file.Path tempDir,String modelPath) throws Exception
		Private Sub importSepConv2D(ByVal tempDir As Path, ByVal modelPath As String)
			Dim model As MultiLayerNetwork = loadMultiLayerNetwork(tempDir,modelPath, False)

			Dim depthWeights As INDArray = model.getLayer(0).getParam("W")
			Dim depthWeightShape As val = depthWeights.shape()

			Dim depthMult As Long = 2
			Dim kernel As Long = 3
			Dim nIn As Long = 5
			Dim nOut As Long = 6

			assertEquals(depthMult, depthWeightShape(0))
			assertEquals(nIn, depthWeightShape(1))
			assertEquals(kernel, depthWeightShape(2))
			assertEquals(kernel, depthWeightShape(3))

			Dim weights As INDArray = model.getLayer(0).getParam("pW")
			Dim weightShape As val = weights.shape()


			assertEquals(nOut, weightShape(0))
			assertEquals(nIn * depthMult, weightShape(1))
			assertEquals(1, weightShape(2))
			assertEquals(1, weightShape(3))

			Dim bias As INDArray = model.getLayer(0).getParam("b")
			assertEquals(6, bias.length())

			Dim input As INDArray = Nd4j.ones(1, 3, 4, 5) 'NHWC
			Dim output As INDArray = model.output(input)

			assertArrayEquals(New Long() {1, 1, 2, 6}, output.shape()) 'NHWC

			logSuccess(modelPath)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void importConv2D(java.nio.file.Path tempDir,String modelPath) throws Exception
		Private Sub importConv2D(ByVal tempDir As Path, ByVal modelPath As String)
			Dim model As MultiLayerNetwork = loadMultiLayerNetwork(tempDir,modelPath, False)

			Dim weights As INDArray = model.getLayer(0).getParam("W")
			Dim weightShape As val = weights.shape()
			assertEquals(6, weightShape(0))
			assertEquals(5, weightShape(1))
			assertEquals(3, weightShape(2))
			assertEquals(3, weightShape(3))

			Dim bias As INDArray = model.getLayer(0).getParam("b")
			assertEquals(6,bias.length())
			logSuccess(modelPath)
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void importConv2DReshape(java.nio.file.Path tempDir,String modelPath) throws Exception
		Private Sub importConv2DReshape(ByVal tempDir As Path, ByVal modelPath As String)
			Dim model As MultiLayerNetwork = loadMultiLayerNetwork(tempDir,modelPath, False)


			Dim nOut As Integer = 12
			Dim mb As Integer = 10

			Dim inShape() As Integer = {5, 5, 5}
			Dim input As INDArray = Nd4j.zeros(mb, inShape(0), inShape(1), inShape(2))
			Dim output As INDArray = model.output(input)
			assertArrayEquals(New Long(){mb, nOut}, output.shape())
			logSuccess(modelPath)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void importConv1DFlatten(java.nio.file.Path tempDir,String modelPath) throws Exception
		Private Sub importConv1DFlatten(ByVal tempDir As Path, ByVal modelPath As String)
			Dim model As MultiLayerNetwork = loadMultiLayerNetwork(tempDir,modelPath, False)

			Dim nOut As Integer = 6
			Dim inputLength As Integer = 10
			Dim mb As Integer = 42
			Dim kernel As Integer = 3

			Dim input As INDArray = Nd4j.zeros(mb, inputLength)
			Dim output As INDArray = model.output(input)
			If modelPath.Contains("tensorflow") Then
				assertArrayEquals(New Long(){mb, inputLength - kernel + 1, nOut}, output.shape()) 'NWC
			ElseIf modelPath.Contains("theano") Then
				assertArrayEquals(New Long(){mb, nOut, inputLength - kernel + 1}, output.shape()) 'NCW

			End If
			logSuccess(modelPath)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void importBatchNormToConv2D(java.nio.file.Path tempDir,String modelPath) throws Exception
		Private Sub importBatchNormToConv2D(ByVal tempDir As Path, ByVal modelPath As String)
			Dim model As MultiLayerNetwork = loadMultiLayerNetwork(tempDir,modelPath, False)
			model.summary()
			logSuccess(modelPath)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void importSimpleSpaceToDepth(java.nio.file.Path tempDir,String modelPath) throws Exception
		Private Sub importSimpleSpaceToDepth(ByVal tempDir As Path, ByVal modelPath As String)
			KerasLayer.registerCustomLayer("Lambda", GetType(KerasSpaceToDepth))
			Dim model As MultiLayerNetwork = loadMultiLayerNetwork(tempDir,modelPath, False)

			Dim input As INDArray = Nd4j.zeros(10, 6, 6, 4)
			Dim output As INDArray = model.output(input)
			assertArrayEquals(New Long(){10, 3, 3, 16}, output.shape())
			logSuccess(modelPath)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void importGraphSpaceToDepth(java.nio.file.Path tempDir,String modelPath) throws Exception
		Private Sub importGraphSpaceToDepth(ByVal tempDir As Path, ByVal modelPath As String)
			KerasLayer.registerCustomLayer("Lambda", GetType(KerasSpaceToDepth))
			Dim model As ComputationGraph = loadComputationalGraph(tempDir,modelPath, False)

	'        INDArray input[] = new INDArray[]{Nd4j.zeros(10, 4, 6, 6), Nd4j.zeros(10, 16, 3, 3)};
			Dim input() As INDArray = {Nd4j.zeros(10, 6, 6, 4), Nd4j.zeros(10, 3, 3, 16)}
			Dim output() As INDArray = model.output(input)
			log.info(Arrays.toString(output(0).shape()))
			assertArrayEquals(New Long(){10, 3, 3, 32}, output(0).shape())
			logSuccess(modelPath)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void importLstm(java.nio.file.Path tempDir,String modelPath) throws Exception
		Private Sub importLstm(ByVal tempDir As Path, ByVal modelPath As String)
			Dim model As MultiLayerNetwork = loadMultiLayerNetwork(tempDir,modelPath, False)
			model.summary()
			' TODO: check weights
			logSuccess(modelPath)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void importEmbeddingLstm(java.nio.file.Path tempDir,String modelPath) throws Exception
		Private Sub importEmbeddingLstm(ByVal tempDir As Path, ByVal modelPath As String)
			Dim model As MultiLayerNetwork = loadMultiLayerNetwork(tempDir,modelPath, False)

			Dim nIn As Integer = 4
			Dim nOut As Integer = 6
			Dim outputDim As Integer = 5
			Dim inputLength As Integer = 10
			Dim mb As Integer = 42

			Dim embeddingWeight As INDArray = model.getLayer(0).getParam("W")
			Dim embeddingWeightShape As val = embeddingWeight.shape()
			assertEquals(nIn, embeddingWeightShape(0))
			assertEquals(outputDim, embeddingWeightShape(1))

			Dim inEmbedding As INDArray = Nd4j.zeros(mb, inputLength)
			Dim output As INDArray = model.output(inEmbedding)
			assertArrayEquals(New Long(){mb, inputLength, nOut}, output.shape()) 'NWC format
			logSuccess(modelPath)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void importEmbeddingConv1DExtended(java.nio.file.Path tempDir,String modelPath) throws Exception
		Private Sub importEmbeddingConv1DExtended(ByVal tempDir As Path, ByVal modelPath As String)
			Dim model As MultiLayerNetwork = loadMultiLayerNetwork(tempDir,modelPath, False)
			logSuccess(modelPath)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void importEmbeddingConv1D(java.nio.file.Path tempDir,String modelPath) throws Exception
		Private Sub importEmbeddingConv1D(ByVal tempDir As Path, ByVal modelPath As String)
			Dim model As MultiLayerNetwork = loadMultiLayerNetwork(tempDir,modelPath, False)

			Dim nIn As Integer = 4
			Dim nOut As Integer = 6
			Dim outputDim As Integer = 5
			Dim inputLength As Integer = 10
			Dim kernel As Integer = 3
			Dim mb As Integer = 42

			Dim embeddingWeight As INDArray = model.getLayer(0).getParam("W")
			Dim embeddingWeightShape As val = embeddingWeight.shape()
			assertEquals(nIn, embeddingWeightShape(0))
			assertEquals(outputDim, embeddingWeightShape(1))

			Dim inEmbedding As INDArray = Nd4j.zeros(mb, inputLength)
			Dim output As INDArray = model.output(inEmbedding)
			If modelPath.Contains("tensorflow") Then
				assertArrayEquals(New Long(){mb, inputLength - kernel + 1, nOut}, output.shape()) 'NWC
			ElseIf modelPath.Contains("theano") Then
				assertArrayEquals(New Long(){mb, nOut, inputLength - kernel + 1}, output.shape()) 'NCC
			End If

			logSuccess(modelPath)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void importSimpleRnn(java.nio.file.Path tempDir,String modelPath) throws Exception
		Private Sub importSimpleRnn(ByVal tempDir As Path, ByVal modelPath As String)
			Dim model As MultiLayerNetwork = loadMultiLayerNetwork(tempDir,modelPath, False)
			model.summary()
			logSuccess(modelPath)
			' TODO: check weights
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void importBidirectionalLstm(java.nio.file.Path tempDir,String modelPath) throws Exception
		Private Sub importBidirectionalLstm(ByVal tempDir As Path, ByVal modelPath As String)
			Dim model As MultiLayerNetwork = loadMultiLayerNetwork(tempDir,modelPath, False)
			model.summary()
			logSuccess(modelPath)
			' TODO: check weights
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.deeplearning4j.nn.multilayer.MultiLayerNetwork loadMultiLayerNetwork(java.nio.file.Path tempDir, String modelPath, boolean training) throws Exception
		Private Function loadMultiLayerNetwork(ByVal tempDir As Path, ByVal modelPath As String, ByVal training As Boolean) As MultiLayerNetwork
			Dim modelFile As File = createTempFile(tempDir,"temp", ".h5")
			Using [is] As Stream = org.nd4j.common.resources.Resources.asStream(modelPath)
				Files.copy([is], modelFile.toPath(), StandardCopyOption.REPLACE_EXISTING)
				Return (New KerasModel()).modelBuilder().modelHdf5Filename(modelFile.getAbsolutePath()).enforceTrainingConfig(training).buildSequential().MultiLayerNetwork
			End Using
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.deeplearning4j.nn.graph.ComputationGraph loadComputationalGraph(java.nio.file.Path tempDir,String modelPath, boolean training) throws Exception
		Private Function loadComputationalGraph(ByVal tempDir As Path, ByVal modelPath As String, ByVal training As Boolean) As ComputationGraph
			Dim modelFile As File = createTempFile(tempDir,"temp", ".h5")
			Using [is] As Stream = org.nd4j.common.resources.Resources.asStream(modelPath)
				Files.copy([is], modelFile.toPath(), StandardCopyOption.REPLACE_EXISTING)
				Return (New KerasModel()).modelBuilder().modelHdf5Filename(modelFile.getAbsolutePath()).enforceTrainingConfig(training).buildModel().ComputationGraph
			End Using
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private java.io.File createTempFile(java.nio.file.Path tempDir,String prefix, String suffix) throws java.io.IOException
		Private Function createTempFile(ByVal tempDir As Path, ByVal prefix As String, ByVal suffix As String) As File
'JAVA TO VB CONVERTER NOTE: The local variable createTempFile was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim createTempFile_Conflict As File = Files.createTempFile(tempDir,prefix & "-" & System.nanoTime(),suffix).toFile()
			Return createTempFile_Conflict
		End Function

	End Class

End Namespace