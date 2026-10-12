Imports System.Collections.Generic
Imports System.Linq
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports org.deeplearning4j.datasets.iterator.impl
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Disabled = org.junit.jupiter.api.Disabled
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

Namespace org.deeplearning4j.nn.graph



	'@Disabled
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestCompGraphCNN extends org.deeplearning4j.BaseDL4JTest
	Public Class TestCompGraphCNN
		Inherits BaseDL4JTest

		Protected Friend conf As ComputationGraphConfiguration
		Protected Friend graph As ComputationGraph
		Protected Friend dataSetIterator As DataSetIterator
'JAVA TO VB CONVERTER NOTE: The field ds was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend ds_Conflict As DataSet

		Protected Friend Shared ReadOnly Property MultiInputGraphConfig As ComputationGraphConfiguration
			Get
				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).graphBuilder().addInputs("input").setInputTypes(InputType.convolutional(32, 32, 3)).addLayer("cnn1", (New ConvolutionLayer.Builder(4, 4)).stride(2, 2).nIn(3).nOut(3).build(), "input").addLayer("cnn2", (New ConvolutionLayer.Builder(4, 4)).stride(2, 2).nIn(3).nOut(3).build(), "input").addLayer("max1", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).stride(1, 1).kernelSize(2, 2).build(), "cnn1", "cnn2").addLayer("dnn1", (New DenseLayer.Builder()).nOut(7).build(), "max1").addLayer("output", (New OutputLayer.Builder()).nIn(7).nOut(10).activation(Activation.SOFTMAX).build(), "dnn1").setOutputs("output").build()
    
				Return conf
			End Get
		End Property

		Protected Friend Shared ReadOnly Property DS As DataSetIterator
			Get
    
				Dim list As IList(Of DataSet) = New List(Of DataSet)(5)
				For i As Integer = 0 To 4
					Dim f As INDArray = Nd4j.create(1, 32 * 32 * 3)
					Dim l As INDArray = Nd4j.create(1, 10)
					l.putScalar(i, 1.0)
					list.Add(New DataSet(f, l))
				Next i
				Return New ListDataSetIterator(list, 5)
			End Get
		End Property

		Protected Friend Shared ReadOnly Property NumParams As Integer
			Get
				Return 2 * (3 * 1 * 4 * 4 * 3 + 3) + (7 * 14 * 14 * 6 + 7) + (7 * 10 + 10)
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach @Disabled public void beforeDo()
		Public Overridable Sub beforeDo()
			conf = MultiInputGraphConfig
			graph = New ComputationGraph(conf)
			graph.init()

			dataSetIterator = DS
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			ds_Conflict = dataSetIterator.next()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConfigBasic()
		Public Overridable Sub testConfigBasic()
			'Check the order. there are 2 possible valid orders here
			Dim order() As Integer = graph.topologicalSortOrder()
			Dim expOrder1() As Integer = {0, 1, 2, 4, 3, 5, 6} 'First of 2 possible valid orders
			Dim expOrder2() As Integer = {0, 2, 1, 4, 3, 5, 6} 'Second of 2 possible valid orders
			Dim orderOK As Boolean = expOrder1.SequenceEqual(order) OrElse expOrder2.SequenceEqual(order)
			assertTrue(orderOK)

			Dim params As INDArray = graph.params()
			assertNotNull(params)

			' confirm param shape is what is expected
			Dim nParams As Integer = NumParams
			assertEquals(nParams, params.length())

			Dim arr As INDArray = Nd4j.linspace(0, nParams, nParams, DataType.FLOAT).reshape(ChrW(1), nParams)
			assertEquals(nParams, arr.length())

			' params are set
			graph.Params = arr
			params = graph.params()
			assertEquals(arr, params)

			'Number of inputs and outputs:
			assertEquals(1, graph.NumInputArrays)
			assertEquals(1, graph.NumOutputArrays)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testCNNComputationGraphKernelTooLarge()
		Public Overridable Sub testCNNComputationGraphKernelTooLarge()
		   assertThrows(GetType(DL4JInvalidConfigException),Sub()
		   Dim imageWidth As Integer = 23
		   Dim imageHeight As Integer = 19
		   Dim nChannels As Integer = 1
		   Dim classes As Integer = 2
		   Dim numSamples As Integer = 200
		   Dim kernelHeight As Integer = 3
		   Dim kernelWidth As Integer = imageWidth
		   Dim trainInput As DataSet
		   Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(123).graphBuilder().addInputs("input").setInputTypes(InputType.convolutional(nChannels, imageWidth, imageHeight)).addLayer("conv1", (New ConvolutionLayer.Builder()).kernelSize(kernelHeight, kernelWidth).stride(1, 1).dataFormat(CNN2DFormat.NCHW).nIn(nChannels).nOut(2).weightInit(WeightInit.XAVIER).activation(Activation.RELU).build(), "input").addLayer("pool1", (New SubsamplingLayer.Builder()).dataFormat(CNN2DFormat.NCHW).poolingType(SubsamplingLayer.PoolingType.MAX).kernelSize(imageHeight - kernelHeight + 1, 1).stride(1, 1).build(), "conv1").addLayer("output", (New OutputLayer.Builder()).nOut(classes).activation(Activation.SOFTMAX).build(), "pool1").setOutputs("output").build()
		   Dim model As New ComputationGraph(conf)
		   model.init()
		   Dim emptyFeatures As INDArray = Nd4j.zeros(numSamples, imageWidth * imageHeight * nChannels)
		   Dim emptyLables As INDArray = Nd4j.zeros(numSamples, classes)
		   trainInput = New DataSet(emptyFeatures, emptyLables)
		   model.fit(trainInput)
		   End Sub)

		End Sub
	End Class

End Namespace