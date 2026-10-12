Imports System
Imports System.Linq
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports SubsamplingLayer = org.deeplearning4j.nn.conf.layers.SubsamplingLayer
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports org.junit.jupiter.api.Assertions
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
import static org.junit.jupiter.api.Assertions.assertThrows

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
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Subsampling Layer Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class SubsamplingLayerTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class SubsamplingLayerTest
		Inherits BaseDL4JTest

		Private nExamples As Integer = 1

		' channels & nOut
		Private depth As Integer = 20

		Private nChannelsIn As Integer = 1

		Private inputWidth As Integer = 28

		Private inputHeight As Integer = 28

		Private kernelSize() As Integer = { 2, 2 }

		Private stride() As Integer = { 2, 2 }

		Friend featureMapWidth As Integer = (inputWidth - kernelSize(0)) \ stride(0) + 1

		Friend featureMapHeight As Integer = (inputHeight - kernelSize(1)) \ stride(0) + 1

		Private epsilon As INDArray = Nd4j.ones(nExamples, depth, featureMapHeight, featureMapWidth)

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Sub Sample Max Activate") void testSubSampleMaxActivate() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSubSampleMaxActivate()
			Dim containedExpectedOut As INDArray = Nd4j.create(New Double() { 5.0, 7.0, 6.0, 8.0, 4.0, 7.0, 5.0, 9.0 }, New Long() { 1, 2, 2, 2 }).castTo(Nd4j.defaultFloatingPointType())
			Dim containedInput As INDArray = ContainedData
			Dim input As INDArray = Data
			Dim layer As Layer = getSubsamplingLayer(SubsamplingLayer.PoolingType.MAX)
			Dim containedOutput As INDArray = layer.activate(containedInput, False, LayerWorkspaceMgr.noWorkspaces())
			assertTrue(containedExpectedOut.shape().SequenceEqual(containedOutput.shape()))
			assertEquals(containedExpectedOut, containedOutput)
			Dim output As INDArray = layer.activate(input, False, LayerWorkspaceMgr.noWorkspaces())
			assertTrue(New Long() { nExamples, nChannelsIn, featureMapWidth, featureMapHeight }.SequenceEqual(output.shape()))
			' channels retained
			assertEquals(nChannelsIn, output.size(1), 1e-4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Sub Sample Mean Activate") void testSubSampleMeanActivate() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSubSampleMeanActivate()
			Dim containedExpectedOut As INDArray = Nd4j.create(New Double() { 2.0, 4.0, 3.0, 5.0, 3.5, 6.5, 4.5, 8.5 }, New Integer() { 1, 2, 2, 2 }).castTo(Nd4j.defaultFloatingPointType())
			Dim containedInput As INDArray = ContainedData
			Dim input As INDArray = Data
			Dim layer As Layer = getSubsamplingLayer(SubsamplingLayer.PoolingType.AVG)
			Dim containedOutput As INDArray = layer.activate(containedInput, False, LayerWorkspaceMgr.noWorkspaces())
			assertTrue(containedExpectedOut.shape().SequenceEqual(containedOutput.shape()))
			assertEquals(containedExpectedOut, containedOutput)
			Dim output As INDArray = layer.activate(input, False, LayerWorkspaceMgr.noWorkspaces())
			assertTrue(New Long() { nExamples, nChannelsIn, featureMapWidth, featureMapHeight }.SequenceEqual(output.shape()))
			' channels retained
			assertEquals(nChannelsIn, output.size(1), 1e-4)
		End Sub

		' ////////////////////////////////////////////////////////////////////////////////
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Sub Sample Layer Max Backprop") void testSubSampleLayerMaxBackprop() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSubSampleLayerMaxBackprop()
			Dim expectedContainedEpsilonInput As INDArray = Nd4j.create(New Double() { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 }, New Integer() { 1, 2, 2, 2 }).castTo(Nd4j.defaultFloatingPointType())
			Dim expectedContainedEpsilonResult As INDArray = Nd4j.create(New Double() { 0.0, 0.0, 0.0, 1.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0 }, New Integer() { 1, 2, 4, 4 }).castTo(Nd4j.defaultFloatingPointType())
			Dim input As INDArray = ContainedData
			Dim layer As Layer = getSubsamplingLayer(SubsamplingLayer.PoolingType.MAX)
			layer.activate(input, False, LayerWorkspaceMgr.noWorkspaces())
			Dim containedOutput As Pair(Of Gradient, INDArray) = layer.backpropGradient(expectedContainedEpsilonInput, LayerWorkspaceMgr.noWorkspaces())
			assertEquals(expectedContainedEpsilonResult, containedOutput.Second)
			assertEquals(Nothing, containedOutput.First.getGradientFor("W"))
			assertEquals(expectedContainedEpsilonResult.shape().Length, containedOutput.Second.shape().Length)
			Dim input2 As INDArray = Data
			layer.activate(input2, False, LayerWorkspaceMgr.noWorkspaces())
			Dim depth As Long = input2.size(1)
			epsilon = Nd4j.ones(5, depth, featureMapHeight, featureMapWidth)
			Dim [out] As Pair(Of Gradient, INDArray) = layer.backpropGradient(epsilon, LayerWorkspaceMgr.noWorkspaces())
			assertEquals(input.shape().Length, [out].Second.shape().Length)
			' channels retained
			assertEquals(depth, [out].Second.size(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Sub Sample Layer Avg Backprop") void testSubSampleLayerAvgBackprop() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSubSampleLayerAvgBackprop()
			Dim expectedContainedEpsilonInput As INDArray = Nd4j.create(New Double() { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0 }, New Integer() { 1, 2, 2, 2 }).castTo(Nd4j.defaultFloatingPointType())
			Dim expectedContainedEpsilonResult As INDArray = Nd4j.create(New Double() { 0.25, 0.25, 0.5, 0.5, 0.25, 0.25, 0.5, 0.5, 0.75, 0.75, 1.0, 1.0, 0.75, 0.75, 1.0, 1.0, 1.25, 1.25, 1.5, 1.5, 1.25, 1.25, 1.5, 1.5, 1.75, 1.75, 2.0, 2.0, 1.75, 1.75, 2.0, 2.0 }, New Integer() { 1, 2, 4, 4 }).castTo(Nd4j.defaultFloatingPointType())
			Dim input As INDArray = ContainedData
			Dim layer As Layer = getSubsamplingLayer(SubsamplingLayer.PoolingType.AVG)
			layer.activate(input, False, LayerWorkspaceMgr.noWorkspaces())
			Dim containedOutput As Pair(Of Gradient, INDArray) = layer.backpropGradient(expectedContainedEpsilonInput, LayerWorkspaceMgr.noWorkspaces())
			assertEquals(expectedContainedEpsilonResult, containedOutput.Second)
			assertEquals(Nothing, containedOutput.First.getGradientFor("W"))
			assertArrayEquals(expectedContainedEpsilonResult.shape(), containedOutput.Second.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Sub Sample Layer Sum Backprop") void testSubSampleLayerSumBackprop()
		Friend Overridable Sub testSubSampleLayerSumBackprop()
			assertThrows(GetType(System.NotSupportedException), Sub()
			Dim layer As Layer = getSubsamplingLayer(SubsamplingLayer.PoolingType.SUM)
			Dim input As INDArray = Data
			layer.setInput(input, LayerWorkspaceMgr.noWorkspaces())
			layer.backpropGradient(epsilon, LayerWorkspaceMgr.noWorkspaces())
			End Sub)
		End Sub

		' ////////////////////////////////////////////////////////////////////////////////
		Private Function getSubsamplingLayer(ByVal pooling As SubsamplingLayer.PoolingType) As Layer
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).gradientNormalization(GradientNormalization.RenormalizeL2PerLayer).seed(123).layer((New SubsamplingLayer.Builder(pooling, New Integer() { 2, 2 })).build()).build()
			Return conf.getLayer().instantiate(conf, Nothing, 0, Nothing, True, Nd4j.defaultFloatingPointType())
		End Function

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

		Public Overridable ReadOnly Property ContainedData As INDArray
			Get
				Dim ret As INDArray = Nd4j.create(New Double() { 1.0, 1.0, 3.0, 7.0, 5.0, 1.0, 3.0, 3.0, 2.0, 2.0, 8.0, 4.0, 2.0, 6.0, 4.0, 4.0, 3.0, 3.0, 6.0, 7.0, 4.0, 4.0, 6.0, 7.0, 5.0, 5.0, 9.0, 8.0, 4.0, 4.0, 9.0, 8.0 }, New Integer() { 1, 2, 4, 4 }).castTo(Nd4j.defaultFloatingPointType())
				Return ret
			End Get
		End Property

		Private Function createPrevGradient() As Gradient
			Dim gradient As Gradient = New DefaultGradient()
			Dim pseudoGradients As INDArray = Nd4j.ones(nExamples, nChannelsIn, inputHeight, inputWidth)
			gradient.gradientForVariable()(DefaultParamInitializer.BIAS_KEY) = pseudoGradients
			gradient.gradientForVariable()(DefaultParamInitializer.WEIGHT_KEY) = pseudoGradients
			Return gradient
		End Function

		' ////////////////////////////////////////////////////////////////////////////////
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Sub Too Large Kernel") void testSubTooLargeKernel()
		Friend Overridable Sub testSubTooLargeKernel()
			assertThrows(GetType(Exception), Sub()
			Dim imageHeight As Integer = 20
			Dim imageWidth As Integer = 23
			Dim nChannels As Integer = 1
			Dim classes As Integer = 2
			Dim numSamples As Integer = 200
			Dim kernelHeight As Integer = 3
			Dim kernelWidth As Integer = 3
			Dim trainInput As DataSet
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(123).list().layer(0, (New org.deeplearning4j.nn.conf.layers.ConvolutionLayer.Builder(kernelHeight, kernelWidth)).stride(1, 1).nOut(2).activation(Activation.RELU).weightInit(WeightInit.XAVIER).build()).layer(1, (New SubsamplingLayer.Builder()).poolingType(SubsamplingLayer.PoolingType.MAX).kernelSize(imageHeight - kernelHeight + 2, 1).stride(1, 1).build()).layer(2, (New OutputLayer.Builder()).nOut(classes).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(imageHeight, imageWidth, nChannels))
			Dim conf As MultiLayerConfiguration = builder.build()
			Dim model As New MultiLayerNetwork(conf)
			model.init()
			Dim emptyFeatures As INDArray = Nd4j.zeros(numSamples, imageWidth * imageHeight * nChannels)
			Dim emptyLables As INDArray = Nd4j.zeros(numSamples, classes)
			trainInput = New DataSet(emptyFeatures, emptyLables)
			model.fit(trainInput)
			End Sub)
		End Sub
	End Class

End Namespace