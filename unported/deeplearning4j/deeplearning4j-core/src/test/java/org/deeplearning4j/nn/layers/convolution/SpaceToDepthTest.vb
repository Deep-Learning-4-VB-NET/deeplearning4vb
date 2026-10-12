Imports System.Linq
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports SpaceToDepthLayer = org.deeplearning4j.nn.conf.layers.SpaceToDepthLayer
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue
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
Namespace org.deeplearning4j.nn.layers.convolution

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Space To Depth Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class SpaceToDepthTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class SpaceToDepthTest
		Inherits BaseDL4JTest

		Private mb As Integer = 1

		Private inDepth As Integer = 2

		Private inputWidth As Integer = 2

		Private inputHeight As Integer = 2

		Private blockSize As Integer = 2

		Private dataFormat As SpaceToDepthLayer.DataFormat = SpaceToDepthLayer.DataFormat.NCHW

		Private outDepth As Integer = inDepth * blockSize * blockSize

		Private outputHeight As Integer = inputHeight \ blockSize

		Private outputWidth As Integer = inputWidth \ blockSize

		Private ReadOnly Property ContainedData As INDArray
			Get
				Return Nd4j.create(New Double() { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0 }, New Integer() { mb, inDepth, inputHeight, inputWidth }, "c"c)
			End Get
		End Property

		Private ReadOnly Property ContainedOutput As INDArray
			Get
				Return Nd4j.create(New Double() { 1.0, 5.0, 2.0, 6.0, 3.0, 7.0, 4.0, 8.0 }, New Integer() { mb, outDepth, outputHeight, outputWidth }, "c"c)
			End Get
		End Property

		Private ReadOnly Property SpaceToDepthLayer As Layer
			Get
				Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).gradientNormalization(GradientNormalization.RenormalizeL2PerLayer).seed(123).layer((New SpaceToDepthLayer.Builder(blockSize, dataFormat)).build()).build()
				Return conf.getLayer().instantiate(conf, Nothing, 0, Nothing, True, Nd4j.defaultFloatingPointType())
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Space To Depth Forward") void testSpaceToDepthForward() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSpaceToDepthForward()
			Dim containedInput As INDArray = ContainedData
			Dim containedExpectedOut As INDArray = ContainedOutput
			Dim std As Layer = SpaceToDepthLayer
			Dim containedOutput As INDArray = std.activate(containedInput, False, LayerWorkspaceMgr.noWorkspaces())
			assertTrue(containedExpectedOut.shape().SequenceEqual(containedOutput.shape()))
			assertEquals(containedExpectedOut, containedOutput)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Space To Depth Backward") void testSpaceToDepthBackward() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSpaceToDepthBackward()
			Dim containedInputEpsilon As INDArray = ContainedOutput
			Dim containedExpectedOut As INDArray = ContainedData
			Dim std As Layer = SpaceToDepthLayer
			std.setInput(ContainedData, LayerWorkspaceMgr.noWorkspaces())
			Dim containedOutput As INDArray = std.backpropGradient(containedInputEpsilon, LayerWorkspaceMgr.noWorkspaces()).Right
			assertTrue(containedExpectedOut.shape().SequenceEqual(containedOutput.shape()))
			assertEquals(containedExpectedOut, containedOutput)
		End Sub
	End Class

End Namespace