﻿Imports System.Linq
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Upsampling2D = org.deeplearning4j.nn.conf.layers.Upsampling2D
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
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
Namespace org.deeplearning4j.nn.layers.convolution

	''' <summary>
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Upsampling 2 D Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class Upsampling2DTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class Upsampling2DTest
		Inherits BaseDL4JTest

		Private nExamples As Integer = 1

		Private depth As Integer = 20

		Private nChannelsIn As Integer = 1

		Private inputWidth As Integer = 28

		Private inputHeight As Integer = 28

		Private size As Integer = 2

		Private outputWidth As Integer = inputWidth * size

		Private outputHeight As Integer = inputHeight * size

		Private epsilon As INDArray = Nd4j.ones(nExamples, depth, outputHeight, outputWidth)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Upsampling") void testUpsampling() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testUpsampling()
			Dim outArray() As Double = { 1.0, 1.0, 2.0, 2.0, 1.0, 1.0, 2.0, 2.0, 3.0, 3.0, 4.0, 4.0, 3.0, 3.0, 4.0, 4.0 }
			Dim containedExpectedOut As INDArray = Nd4j.create(outArray, New Integer() { 1, 1, 4, 4 })
			Dim containedInput As INDArray = ContainedData
			Dim input As INDArray = Data
			Dim layer As Layer = UpsamplingLayer
			Dim containedOutput As INDArray = layer.activate(containedInput, False, LayerWorkspaceMgr.noWorkspaces())
			assertTrue(containedExpectedOut.shape().SequenceEqual(containedOutput.shape()))
			assertEquals(containedExpectedOut, containedOutput)
			Dim output As INDArray = layer.activate(input, False, LayerWorkspaceMgr.noWorkspaces())
			assertTrue(New Long() { nExamples, nChannelsIn, outputWidth, outputHeight }.SequenceEqual(output.shape()))
			assertEquals(nChannelsIn, output.size(1), 1e-4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Upsampling 2 D Backprop") void testUpsampling2DBackprop() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testUpsampling2DBackprop()
			Dim expectedContainedEpsilonInput As INDArray = Nd4j.create(New Double() { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 }, New Integer() { 1, 1, 4, 4 })
			Dim expectedContainedEpsilonResult As INDArray = Nd4j.create(New Double() { 4.0, 4.0, 4.0, 4.0 }, New Integer() { 1, 1, 2, 2 })
			Dim input As INDArray = ContainedData
			Dim layer As Layer = UpsamplingLayer
			layer.activate(input, False, LayerWorkspaceMgr.noWorkspaces())
			Dim containedOutput As Pair(Of Gradient, INDArray) = layer.backpropGradient(expectedContainedEpsilonInput, LayerWorkspaceMgr.noWorkspaces())
			assertEquals(expectedContainedEpsilonResult, containedOutput.Second)
			assertEquals(Nothing, containedOutput.First.getGradientFor("W"))
			assertEquals(expectedContainedEpsilonResult.shape().Length, containedOutput.Second.shape().Length)
			Dim input2 As INDArray = Data
			layer.activate(input2, False, LayerWorkspaceMgr.noWorkspaces())
			Dim depth As val = input2.size(1)
			epsilon = Nd4j.ones(5, depth, outputHeight, outputWidth)
			Dim [out] As Pair(Of Gradient, INDArray) = layer.backpropGradient(epsilon, LayerWorkspaceMgr.noWorkspaces())
			assertEquals(input.shape().Length, [out].Second.shape().Length)
			assertEquals(depth, [out].Second.size(1))
		End Sub

		Private ReadOnly Property UpsamplingLayer As Layer
			Get
				Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).gradientNormalization(GradientNormalization.RenormalizeL2PerLayer).seed(123).layer((New Upsampling2D.Builder(size)).build()).build()
				Return conf.getLayer().instantiate(conf, Nothing, 0, Nothing, True, Nd4j.defaultFloatingPointType())
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray getData() throws Exception
		Public Overridable ReadOnly Property Data As INDArray
			Get
				Dim data As DataSetIterator = New MnistDataSetIterator(5, 5)
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim mnist As DataSet = data.next()
				nExamples = mnist.numExamples()
				Return mnist.Features.reshape(ChrW(nExamples), nChannelsIn, inputWidth, inputHeight)
			End Get
		End Property

		Private ReadOnly Property ContainedData As INDArray
			Get
				Dim ret As INDArray = Nd4j.create(New Double() { 1.0, 2.0, 3.0, 4.0 }, New Integer() { 1, 1, 2, 2 })
				Return ret
			End Get
		End Property
	End Class

End Namespace