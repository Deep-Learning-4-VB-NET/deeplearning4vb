Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports BatchNormalization = org.deeplearning4j.nn.conf.layers.BatchNormalization
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports SubsamplingLayer = org.deeplearning4j.nn.conf.layers.SubsamplingLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.junit.jupiter.api.Assertions

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.deeplearning4j.cuda


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class TestDataTypes extends org.deeplearning4j.BaseDL4JTest
	Public Class TestDataTypes
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 180000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDataTypesSimple() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDataTypesSimple()

			Dim outMapTrain As IDictionary(Of DataType, INDArray) = New Dictionary(Of DataType, INDArray)()
			Dim outMapTest As IDictionary(Of DataType, INDArray) = New Dictionary(Of DataType, INDArray)()
			For Each globalDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				Nd4j.setDefaultDataTypes(globalDtype, globalDtype)
				For Each netDType As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
					log.info("Starting test: global dtype = {}, net dtype = {}", globalDtype, netDType)
					assertEquals(globalDtype, Nd4j.dataType())
					assertEquals(globalDtype, Nd4j.defaultFloatingPointType())

					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(1e-2)).dataType(netDType).convolutionMode(ConvolutionMode.Same).activation(Activation.TANH).seed(12345).weightInit(WeightInit.XAVIER).list().layer((New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).padding(0, 0).nOut(3).build()).layer((New SubsamplingLayer.Builder()).kernelSize(2, 2).stride(1, 1).padding(0, 0).build()).layer((New BatchNormalization.Builder()).eps(1e-3).build()).layer((New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).padding(0, 0).nOut(3).build()).layer((New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()

					Dim net As New MultiLayerNetwork(conf)
					net.init()


					Dim f1 As System.Reflection.FieldInfo = GetType(org.deeplearning4j.nn.layers.convolution.ConvolutionLayer).getDeclaredField("helper")
					f1.setAccessible(True)

					Dim f2 As System.Reflection.FieldInfo = GetType(org.deeplearning4j.nn.layers.convolution.subsampling.SubsamplingLayer).getDeclaredField("helper")
					f2.setAccessible(True)

					Dim f3 As System.Reflection.FieldInfo = GetType(org.deeplearning4j.nn.layers.normalization.BatchNormalization).getDeclaredField("helper")
					f3.setAccessible(True)

					assertNotNull(f1.get(net.getLayer(0)))
					assertNotNull(f2.get(net.getLayer(1)))
					assertNotNull(f3.get(net.getLayer(2)))
					assertNotNull(f1.get(net.getLayer(3)))

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = (New MnistDataSetIterator(32, True, 12345)).next()

					'Simple sanity checks:
					'System.out.println("STARTING FIT");
					net.fit(ds)
					net.fit(ds)

					'System.out.println("STARTING OUTPUT");
					Dim outTrain As INDArray = net.output(ds.Features, False)
					Dim outTest As INDArray = net.output(ds.Features, True)

					outMapTrain(netDType) = outTrain.castTo(DataType.DOUBLE)
					outMapTest(netDType) = outTest.castTo(DataType.DOUBLE)
				Next netDType
			Next globalDtype

			Nd4j.DataType = DataType.DOUBLE
			Dim fp64Train As INDArray = outMapTrain(DataType.DOUBLE)
			Dim fp32Train As INDArray = outMapTrain(DataType.FLOAT).castTo(DataType.DOUBLE)
			Dim fp16Train As INDArray = outMapTrain(DataType.HALF).castTo(DataType.DOUBLE)

			Dim eq64_32 As Boolean = fp64Train.equalsWithEps(fp32Train, 1e-3)
			Dim eq64_16 As Boolean = fp64Train.equalsWithEps(fp16Train, 1e-2)

			If Not eq64_32 Then
				Console.WriteLine("FP64/32")
				Console.WriteLine("fp64Train:" & vbLf & fp64Train)
				Console.WriteLine("fp32Train:" & vbLf & fp32Train)
			End If

			If Not eq64_16 Then
				Console.WriteLine("FP64/16")
				Console.WriteLine("fp64Train:" & vbLf & fp64Train)
				Console.WriteLine("fp16Train:" & vbLf & fp16Train)
			End If

			assertTrue(eq64_32)
			assertTrue(eq64_16)
		End Sub
	End Class

End Namespace