Imports System.Linq
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Convolution3D = org.deeplearning4j.nn.conf.layers.Convolution3D
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

	''' <summary>
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Convolution 3 D Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class Convolution3DTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class Convolution3DTest
		Inherits BaseDL4JTest

		Private nExamples As Integer = 1

		Private nChannelsOut As Integer = 1

		Private nChannelsIn As Integer = 1

		Private inputDepth As Integer = 2 * 2

		Private inputWidth As Integer = 28 \ 2

		Private inputHeight As Integer = 28 \ 2

		Private kernelSize() As Integer = { 2, 2, 2 }

		Private outputDepth As Integer = inputDepth - kernelSize(0) + 1

		Private outputHeight As Integer = inputHeight - kernelSize(1) + 1

		Private outputWidth As Integer = inputWidth - kernelSize(2) + 1

		Private epsilon As INDArray = Nd4j.ones(nExamples, nChannelsOut, outputDepth, outputHeight, outputWidth)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Convolution 3 d Forward Same Mode") void testConvolution3dForwardSameMode()
		Friend Overridable Sub testConvolution3dForwardSameMode()
			Dim containedInput As INDArray = ContainedData
			Dim layer As Convolution3DLayer = DirectCast(getConvolution3DLayer(ConvolutionMode.Same), Convolution3DLayer)
			assertTrue(layer.convolutionMode = ConvolutionMode.Same)
			Dim containedOutput As INDArray = layer.activate(containedInput, False, LayerWorkspaceMgr.noWorkspaces())
			assertTrue(containedInput.shape().SequenceEqual(containedOutput.shape()))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Convolution 3 d Forward Valid Mode") void testConvolution3dForwardValidMode() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testConvolution3dForwardValidMode()
			Dim layer As Convolution3DLayer = DirectCast(getConvolution3DLayer(ConvolutionMode.Strict), Convolution3DLayer)
			assertTrue(layer.convolutionMode = ConvolutionMode.Strict)
			Dim input As INDArray = Data
			Dim output As INDArray = layer.activate(input, False, LayerWorkspaceMgr.noWorkspaces())
			assertTrue(New Long() { nExamples, nChannelsOut, outputDepth, outputWidth, outputHeight }.SequenceEqual(output.shape()))
		End Sub

		Private Function getConvolution3DLayer(ByVal mode As ConvolutionMode) As Layer
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).gradientNormalization(GradientNormalization.RenormalizeL2PerLayer).seed(123).layer((New Convolution3D.Builder()).kernelSize(kernelSize).nIn(nChannelsIn).nOut(nChannelsOut).dataFormat(Convolution3D.DataFormat.NCDHW).convolutionMode(mode).hasBias(False).build()).build()
			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.ones(1, numParams)
			Return conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType())
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray getData() throws Exception
		Public Overridable ReadOnly Property Data As INDArray
			Get
				Dim data As DataSetIterator = New MnistDataSetIterator(5, 5)
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim mnist As DataSet = data.next()
				nExamples = mnist.numExamples()
				Return mnist.Features.reshape(ChrW(nExamples), nChannelsIn, inputDepth, inputHeight, inputWidth)
			End Get
		End Property

		Private ReadOnly Property ContainedData As INDArray
			Get
				Return Nd4j.create(New Double() { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8 }, New Integer() { 1, 1, 2, 2, 2 })
			End Get
		End Property
	End Class

End Namespace