Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ExistingDataSetIterator = org.deeplearning4j.datasets.iterator.ExistingDataSetIterator
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports WorkspaceMode = org.deeplearning4j.nn.conf.WorkspaceMode
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports ConvolutionLayer = org.deeplearning4j.nn.layers.convolution.ConvolutionLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports CuDNNValidationUtil = org.deeplearning4j.cuda.util.CuDNNValidationUtil
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports ActivationELU = org.nd4j.linalg.activations.impl.ActivationELU
Imports ActivationIdentity = org.nd4j.linalg.activations.impl.ActivationIdentity
Imports ActivationSoftmax = org.nd4j.linalg.activations.impl.ActivationSoftmax
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports IsMax = org.nd4j.linalg.api.ops.impl.transforms.any.IsMax
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports LossNegativeLogLikelihood = org.nd4j.linalg.lossfunctions.impl.LossNegativeLogLikelihood
Imports ScheduleType = org.nd4j.linalg.schedule.ScheduleType
Imports StepSchedule = org.nd4j.linalg.schedule.StepSchedule

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
'ORIGINAL LINE: @Slf4j @NativeTag public class ValidateCuDNN extends org.deeplearning4j.BaseDL4JTest
	Public Class ValidateCuDNN
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 360000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void validateConvLayers()
		Public Overridable Sub validateConvLayers()
			Nd4j.Random.setSeed(12345)

			Dim numClasses As Integer = 10
			'imageHeight,imageWidth,channels
			Dim imageHeight As Integer = 64
			Dim imageWidth As Integer = 64
			Dim channels As Integer = 3
			Dim activation As IActivation = New ActivationIdentity()
'JAVA TO VB CONVERTER NOTE: The variable multiLayerConfiguration was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim multiLayerConfiguration_Conflict As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).weightInit(WeightInit.XAVIER).seed(42).activation(New ActivationELU()).updater(New Nesterovs(1e-3, 0.9)).list((New Convolution2D.Builder()).nOut(16).kernelSize(4, 4).biasInit(0.0).stride(2, 2).build(), (New ActivationLayer.Builder()).activation(activation).build(), (New Pooling2D.Builder()).poolingType(SubsamplingLayer.PoolingType.MAX).kernelSize(3, 3).stride(2, 2).build(), (New Convolution2D.Builder()).nOut(256).kernelSize(5, 5).padding(2, 2).biasInit(0.0).stride(1, 1).build(), (New ActivationLayer.Builder()).activation(activation).build(), (New Pooling2D.Builder()).poolingType(SubsamplingLayer.PoolingType.MAX).kernelSize(3, 3).stride(2, 2).build(), (New Convolution2D.Builder()).nOut(16).kernelSize(3, 3).padding(1, 1).biasInit(0.0).stride(1, 1).build(), (New ActivationLayer.Builder()).activation(activation).build(), (New Convolution2D.Builder()).nOut(16).kernelSize(3, 3).padding(1, 1).stride(1, 1).build(), (New ActivationLayer.Builder()).activation(activation).build(), (New Pooling2D.Builder()).poolingType(SubsamplingLayer.PoolingType.MAX).kernelSize(3, 3).stride(2, 2).build(), (New DenseLayer.Builder()).nOut(64).biasInit(0.0).build(), (New ActivationLayer.Builder()).activation(activation).build(), (New OutputLayer.Builder()).activation(New ActivationSoftmax()).lossFunction(New LossNegativeLogLikelihood()).nOut(numClasses).biasInit(0.0).build()).setInputType(InputType.convolutionalFlat(imageHeight, imageWidth, channels)).build()

			Dim net As New MultiLayerNetwork(multiLayerConfiguration_Conflict)
			net.init()

			Dim fShape() As Integer = {8, channels, imageHeight, imageWidth}
			Dim lShape() As Integer = {8, numClasses}

			Dim classesToTest As IList(Of Type) = New List(Of Type)()
			classesToTest.Add(GetType(ConvolutionLayer))
			classesToTest.Add(GetType(org.deeplearning4j.nn.layers.convolution.subsampling.SubsamplingLayer))

			validateLayers(net, classesToTest, True, fShape, lShape, CuDNNValidationUtil.MAX_REL_ERROR, CuDNNValidationUtil.MIN_ABS_ERROR)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void validateConvLayersSimpleBN()
		Public Overridable Sub validateConvLayersSimpleBN()
			'Test ONLY BN - no other CuDNN functionality (i.e., DL4J impls for everything else)
			Nd4j.Random.setSeed(12345)

			Dim minibatch As Integer = 8
			Dim numClasses As Integer = 10
			'imageHeight,imageWidth,channels
			Dim imageHeight As Integer = 48
			Dim imageWidth As Integer = 48
			Dim channels As Integer = 3
			Dim activation As IActivation = New ActivationIdentity()
'JAVA TO VB CONVERTER NOTE: The variable multiLayerConfiguration was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim multiLayerConfiguration_Conflict As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).weightInit(WeightInit.XAVIER).seed(42).activation(New ActivationELU()).updater(Nesterovs.builder().momentum(0.9).learningRateSchedule(New StepSchedule(ScheduleType.EPOCH, 1e-2, 0.1, 20)).build()).list((New Convolution2D.Builder()).nOut(96).kernelSize(11, 11).biasInit(0.0).stride(4, 4).build(), (New ActivationLayer.Builder()).activation(activation).build(), (New BatchNormalization.Builder()).build(), (New Pooling2D.Builder()).poolingType(SubsamplingLayer.PoolingType.MAX).kernelSize(3, 3).stride(2, 2).build(), (New DenseLayer.Builder()).nOut(128).biasInit(0.0).build(), (New ActivationLayer.Builder()).activation(activation).build(), (New OutputLayer.Builder()).activation(New ActivationSoftmax()).lossFunction(New LossNegativeLogLikelihood()).nOut(numClasses).biasInit(0.0).build()).setInputType(InputType.convolutionalFlat(imageHeight, imageWidth, channels)).build()

			Dim net As New MultiLayerNetwork(multiLayerConfiguration_Conflict)
			net.init()

			Dim fShape() As Integer = {minibatch, channels, imageHeight, imageWidth}
			Dim lShape() As Integer = {minibatch, numClasses}

			Dim classesToTest As IList(Of Type) = New List(Of Type)()
			classesToTest.Add(GetType(org.deeplearning4j.nn.layers.normalization.BatchNormalization))

			validateLayers(net, classesToTest, False, fShape, lShape, CuDNNValidationUtil.MAX_REL_ERROR, CuDNNValidationUtil.MIN_ABS_ERROR)
		End Sub

		Public Overridable Sub validateConvLayersLRN()
			'Test ONLY LRN - no other CuDNN functionality (i.e., DL4J impls for everything else)
			Nd4j.Random.setSeed(12345)

			Dim minibatch As Integer = 8
			Dim numClasses As Integer = 10
			'imageHeight,imageWidth,channels
			Dim imageHeight As Integer = 48
			Dim imageWidth As Integer = 48
			Dim channels As Integer = 3
			Dim activation As IActivation = New ActivationIdentity()
'JAVA TO VB CONVERTER NOTE: The variable multiLayerConfiguration was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim multiLayerConfiguration_Conflict As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).weightInit(WeightInit.XAVIER).seed(42).activation(New ActivationELU()).updater(Nesterovs.builder().momentum(0.9).learningRateSchedule(New StepSchedule(ScheduleType.EPOCH, 1e-2, 0.1, 20)).build()).list((New Convolution2D.Builder()).nOut(96).kernelSize(11, 11).biasInit(0.0).stride(4, 4).build(), (New ActivationLayer.Builder()).activation(activation).build(), (New LocalResponseNormalization.Builder()).alpha(1e-3).beta(0.75).k(2).n(5).build(), (New Pooling2D.Builder()).poolingType(SubsamplingLayer.PoolingType.MAX).kernelSize(3, 3).stride(2, 2).build(), (New Convolution2D.Builder()).nOut(256).kernelSize(5, 5).padding(2, 2).biasInit(0.0).stride(1, 1).build(), (New ActivationLayer.Builder()).activation(activation).build(), (New OutputLayer.Builder()).activation(New ActivationSoftmax()).lossFunction(New LossNegativeLogLikelihood()).nOut(numClasses).biasInit(0.0).build()).setInputType(InputType.convolutionalFlat(imageHeight, imageWidth, channels)).build()

			Dim net As New MultiLayerNetwork(multiLayerConfiguration_Conflict)
			net.init()

			Dim fShape() As Integer = {minibatch, channels, imageHeight, imageWidth}
			Dim lShape() As Integer = {minibatch, numClasses}

			Dim classesToTest As IList(Of Type) = New List(Of Type)()
			classesToTest.Add(GetType(org.deeplearning4j.nn.layers.normalization.LocalResponseNormalization))

			validateLayers(net, classesToTest, False, fShape, lShape, 1e-2, 1e-2)
		End Sub

		Public Shared Sub validateLayers(ByVal net As MultiLayerNetwork, ByVal classesToTest As IList(Of [Class]), ByVal testAllCudnnPresent As Boolean, ByVal fShape() As Integer, ByVal lShape() As Integer, ByVal maxRE As Double, ByVal minAbsErr As Double)

			For Each wsm As WorkspaceMode In New WorkspaceMode(){WorkspaceMode.NONE, WorkspaceMode.ENABLED}

				net.LayerWiseConfigurations.setTrainingWorkspaceMode(wsm)
				net.LayerWiseConfigurations.setInferenceWorkspaceMode(wsm)

				Nd4j.Random.setSeed(12345)
				Dim features As INDArray = Nd4j.rand(fShape)
				Dim labels As INDArray = Nd4j.rand(lShape)
				labels = Nd4j.exec(New IsMax(labels, 1))(0).castTo(features.dataType())

				Dim testCaseList As IList(Of CuDNNValidationUtil.TestCase) = New List(Of CuDNNValidationUtil.TestCase)()

				Dim dataSets As IList(Of DataSet) = New List(Of DataSet)()
				For i As Integer = 0 To 5
					Dim f As INDArray = Nd4j.rand(fShape)
					Dim l As INDArray = Nd4j.rand(lShape)
					l = Nd4j.exec(New IsMax(l, 1))(0).castTo(features.dataType())
					dataSets.Add(New DataSet(f, l))
				Next i
				Dim iter As DataSetIterator = New ExistingDataSetIterator(dataSets)

				For Each c As Type In classesToTest
					Dim name As String = "WS=" & wsm & ", testCudnnFor=" & c.Name
					testCaseList.Add(CuDNNValidationUtil.TestCase.builder().testName(name).allowCudnnHelpersForClasses(Collections.singletonList(Of Type)(c)).testForward(True).testScore(True).testBackward(True).testTraining(True).trainFirst(False).features(features).labels(labels).data(iter).maxRE(maxRE).minAbsErr(minAbsErr).build())
				Next c

				If testAllCudnnPresent Then
					testCaseList.Add(CuDNNValidationUtil.TestCase.builder().testName("WS=" & wsm & ", ALL CLASSES").allowCudnnHelpersForClasses(classesToTest).testForward(True).testScore(True).testBackward(True).trainFirst(False).features(features).labels(labels).data(iter).maxRE(maxRE).minAbsErr(minAbsErr).build())
				End If

				For Each tc As CuDNNValidationUtil.TestCase In testCaseList
					log.info("Running test: " & tc.getTestName())
					CuDNNValidationUtil.validateMLN(net, tc)
				Next tc
			Next wsm
		End Sub

	End Class

End Namespace