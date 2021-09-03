Imports System
Imports System.IO
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports Cnn3DToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.Cnn3DToFeedForwardPreProcessor
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports GraphVertex = org.deeplearning4j.nn.graph.vertex.GraphVertex
Imports PreprocessorVertex = org.deeplearning4j.nn.graph.vertex.impl.PreprocessorVertex
Imports KerasModelImport = org.deeplearning4j.nn.modelimport.keras.KerasModelImport
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports org.junit.jupiter.api.Assertions
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames

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
Namespace org.deeplearning4j.nn.modelimport.keras.layers.flatten

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Keras Flatten 3 d Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasFlatten3dTest
	Friend Class KerasFlatten3dTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Flatten 3 d") void testFlatten3d() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testFlatten3d()
			Dim classPathResource As New ClassPathResource("modelimport/keras/weights/flatten_3d.hdf5")
			Using inputStream As Stream = classPathResource.InputStream
				Dim computationGraph As ComputationGraph = KerasModelImport.importKerasModelAndWeights(inputStream)
				assertNotNull(computationGraph)
				assertEquals(3, computationGraph.Vertices.Length)
				Dim vertices() As GraphVertex = computationGraph.Vertices
				assertTrue(TypeOf vertices(1) Is PreprocessorVertex)
				Dim preprocessorVertex As PreprocessorVertex = DirectCast(vertices(1), PreprocessorVertex)
				Dim preProcessor As InputPreProcessor = preprocessorVertex.getPreProcessor()
				assertTrue(TypeOf preProcessor Is Cnn3DToFeedForwardPreProcessor)
				Dim cnn3DToFeedForwardPreProcessor As Cnn3DToFeedForwardPreProcessor = DirectCast(preProcessor, Cnn3DToFeedForwardPreProcessor)
				assertTrue(cnn3DToFeedForwardPreProcessor.isNCDHW())
				assertEquals(10, cnn3DToFeedForwardPreProcessor.getInputDepth())
				assertEquals(10, cnn3DToFeedForwardPreProcessor.getInputHeight())
				assertEquals(1, cnn3DToFeedForwardPreProcessor.getNumChannels())
				assertEquals(10, cnn3DToFeedForwardPreProcessor.getInputWidth())
				Console.WriteLine(cnn3DToFeedForwardPreProcessor)
			End Using
		End Sub
	End Class

End Namespace