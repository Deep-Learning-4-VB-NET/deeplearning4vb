Imports System
Imports System.Collections.Generic
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports WorkspaceMode = org.deeplearning4j.nn.conf.WorkspaceMode
Imports UniformDistribution = org.deeplearning4j.nn.conf.distribution.UniformDistribution
Imports AlphaDropout = org.deeplearning4j.nn.conf.dropout.AlphaDropout
Imports GaussianDropout = org.deeplearning4j.nn.conf.dropout.GaussianDropout
Imports GaussianNoise = org.deeplearning4j.nn.conf.dropout.GaussianNoise
Imports SpatialDropout = org.deeplearning4j.nn.conf.dropout.SpatialDropout
Imports AttentionVertex = org.deeplearning4j.nn.conf.graph.AttentionVertex
Imports ElementWiseVertex = org.deeplearning4j.nn.conf.graph.ElementWiseVertex
Imports FrozenVertex = org.deeplearning4j.nn.conf.graph.FrozenVertex
Imports GraphVertex = org.deeplearning4j.nn.conf.graph.GraphVertex
Imports L2NormalizeVertex = org.deeplearning4j.nn.conf.graph.L2NormalizeVertex
Imports L2Vertex = org.deeplearning4j.nn.conf.graph.L2Vertex
Imports LayerVertex = org.deeplearning4j.nn.conf.graph.LayerVertex
Imports MergeVertex = org.deeplearning4j.nn.conf.graph.MergeVertex
Imports PoolHelperVertex = org.deeplearning4j.nn.conf.graph.PoolHelperVertex
Imports PreprocessorVertex = org.deeplearning4j.nn.conf.graph.PreprocessorVertex
Imports ReshapeVertex = org.deeplearning4j.nn.conf.graph.ReshapeVertex
Imports ScaleVertex = org.deeplearning4j.nn.conf.graph.ScaleVertex
Imports ShiftVertex = org.deeplearning4j.nn.conf.graph.ShiftVertex
Imports StackVertex = org.deeplearning4j.nn.conf.graph.StackVertex
Imports SubsetVertex = org.deeplearning4j.nn.conf.graph.SubsetVertex
Imports UnstackVertex = org.deeplearning4j.nn.conf.graph.UnstackVertex
Imports DuplicateToTimeSeriesVertex = org.deeplearning4j.nn.conf.graph.rnn.DuplicateToTimeSeriesVertex
Imports LastTimeStepVertex = org.deeplearning4j.nn.conf.graph.rnn.LastTimeStepVertex
Imports ReverseTimeSeriesVertex = org.deeplearning4j.nn.conf.graph.rnn.ReverseTimeSeriesVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ActivationLayer = org.deeplearning4j.nn.conf.layers.ActivationLayer
Imports AutoEncoder = org.deeplearning4j.nn.conf.layers.AutoEncoder
Imports BatchNormalization = org.deeplearning4j.nn.conf.layers.BatchNormalization
Imports CapsuleLayer = org.deeplearning4j.nn.conf.layers.CapsuleLayer
Imports CapsuleStrengthLayer = org.deeplearning4j.nn.conf.layers.CapsuleStrengthLayer
Imports CenterLossOutputLayer = org.deeplearning4j.nn.conf.layers.CenterLossOutputLayer
Imports Cnn3DLossLayer = org.deeplearning4j.nn.conf.layers.Cnn3DLossLayer
Imports CnnLossLayer = org.deeplearning4j.nn.conf.layers.CnnLossLayer
Imports Convolution1D = org.deeplearning4j.nn.conf.layers.Convolution1D
Imports Convolution2D = org.deeplearning4j.nn.conf.layers.Convolution2D
Imports Convolution3D = org.deeplearning4j.nn.conf.layers.Convolution3D
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports Deconvolution2D = org.deeplearning4j.nn.conf.layers.Deconvolution2D
Imports Deconvolution3D = org.deeplearning4j.nn.conf.layers.Deconvolution3D
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports DepthwiseConvolution2D = org.deeplearning4j.nn.conf.layers.DepthwiseConvolution2D
Imports DropoutLayer = org.deeplearning4j.nn.conf.layers.DropoutLayer
Imports EmbeddingLayer = org.deeplearning4j.nn.conf.layers.EmbeddingLayer
Imports EmbeddingSequenceLayer = org.deeplearning4j.nn.conf.layers.EmbeddingSequenceLayer
Imports GlobalPoolingLayer = org.deeplearning4j.nn.conf.layers.GlobalPoolingLayer
Imports GravesBidirectionalLSTM = org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM
Imports GravesLSTM = org.deeplearning4j.nn.conf.layers.GravesLSTM
Imports LSTM = org.deeplearning4j.nn.conf.layers.LSTM
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports LearnedSelfAttentionLayer = org.deeplearning4j.nn.conf.layers.LearnedSelfAttentionLayer
Imports LocalResponseNormalization = org.deeplearning4j.nn.conf.layers.LocalResponseNormalization
Imports LocallyConnected1D = org.deeplearning4j.nn.conf.layers.LocallyConnected1D
Imports LocallyConnected2D = org.deeplearning4j.nn.conf.layers.LocallyConnected2D
Imports LossLayer = org.deeplearning4j.nn.conf.layers.LossLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports PReLULayer = org.deeplearning4j.nn.conf.layers.PReLULayer
Imports Pooling1D = org.deeplearning4j.nn.conf.layers.Pooling1D
Imports Pooling2D = org.deeplearning4j.nn.conf.layers.Pooling2D
Imports PoolingType = org.deeplearning4j.nn.conf.layers.PoolingType
Imports PrimaryCapsules = org.deeplearning4j.nn.conf.layers.PrimaryCapsules
Imports RecurrentAttentionLayer = org.deeplearning4j.nn.conf.layers.RecurrentAttentionLayer
Imports RnnLossLayer = org.deeplearning4j.nn.conf.layers.RnnLossLayer
Imports RnnOutputLayer = org.deeplearning4j.nn.conf.layers.RnnOutputLayer
Imports SelfAttentionLayer = org.deeplearning4j.nn.conf.layers.SelfAttentionLayer
Imports SeparableConvolution2D = org.deeplearning4j.nn.conf.layers.SeparableConvolution2D
Imports SpaceToBatchLayer = org.deeplearning4j.nn.conf.layers.SpaceToBatchLayer
Imports SpaceToDepthLayer = org.deeplearning4j.nn.conf.layers.SpaceToDepthLayer
Imports Subsampling1DLayer = org.deeplearning4j.nn.conf.layers.Subsampling1DLayer
Imports Subsampling3DLayer = org.deeplearning4j.nn.conf.layers.Subsampling3DLayer
Imports SubsamplingLayer = org.deeplearning4j.nn.conf.layers.SubsamplingLayer
Imports Upsampling1D = org.deeplearning4j.nn.conf.layers.Upsampling1D
Imports Upsampling2D = org.deeplearning4j.nn.conf.layers.Upsampling2D
Imports Upsampling3D = org.deeplearning4j.nn.conf.layers.Upsampling3D
Imports ZeroPadding1DLayer = org.deeplearning4j.nn.conf.layers.ZeroPadding1DLayer
Imports ZeroPadding3DLayer = org.deeplearning4j.nn.conf.layers.ZeroPadding3DLayer
Imports ZeroPaddingLayer = org.deeplearning4j.nn.conf.layers.ZeroPaddingLayer
Imports Cropping1D = org.deeplearning4j.nn.conf.layers.convolutional.Cropping1D
Imports Cropping2D = org.deeplearning4j.nn.conf.layers.convolutional.Cropping2D
Imports Cropping3D = org.deeplearning4j.nn.conf.layers.convolutional.Cropping3D
Imports ElementWiseMultiplicationLayer = org.deeplearning4j.nn.conf.layers.misc.ElementWiseMultiplicationLayer
Imports FrozenLayer = org.deeplearning4j.nn.conf.layers.misc.FrozenLayer
Imports FrozenLayerWithBackprop = org.deeplearning4j.nn.conf.layers.misc.FrozenLayerWithBackprop
Imports RepeatVector = org.deeplearning4j.nn.conf.layers.misc.RepeatVector
Imports Yolo2OutputLayer = org.deeplearning4j.nn.conf.layers.objdetect.Yolo2OutputLayer
Imports Bidirectional = org.deeplearning4j.nn.conf.layers.recurrent.Bidirectional
Imports LastTimeStep = org.deeplearning4j.nn.conf.layers.recurrent.LastTimeStep
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
Imports TimeDistributed = org.deeplearning4j.nn.conf.layers.recurrent.TimeDistributed
Imports MaskLayer = org.deeplearning4j.nn.conf.layers.util.MaskLayer
Imports MaskZeroLayer = org.deeplearning4j.nn.conf.layers.util.MaskZeroLayer
Imports VariationalAutoencoder = org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
Imports BaseWrapperLayer = org.deeplearning4j.nn.conf.layers.wrapper.BaseWrapperLayer
Imports OCNNOutputLayer = org.deeplearning4j.nn.conf.ocnn.OCNNOutputLayer
Imports CnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor
Imports CnnToRnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToRnnPreProcessor
Imports ComposableInputPreProcessor = org.deeplearning4j.nn.conf.preprocessor.ComposableInputPreProcessor
Imports FeedForwardToCnn3DPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToCnn3DPreProcessor
Imports RnnToCnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.RnnToCnnPreProcessor
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports IdentityLayer = org.deeplearning4j.nn.layers.util.IdentityLayer
Imports TFOpLayer = org.deeplearning4j.nn.modelimport.keras.layers.TFOpLayer
Imports KerasFlattenRnnPreprocessor = org.deeplearning4j.nn.modelimport.keras.preprocessors.KerasFlattenRnnPreprocessor
Imports PermutePreprocessor = org.deeplearning4j.nn.modelimport.keras.preprocessors.PermutePreprocessor
Imports ReshapePreprocessor = org.deeplearning4j.nn.modelimport.keras.preprocessors.ReshapePreprocessor
Imports TensorFlowCnnToFeedForwardPreProcessor = org.deeplearning4j.nn.modelimport.keras.preprocessors.TensorFlowCnnToFeedForwardPreProcessor
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports WeightInitDistribution = org.deeplearning4j.nn.weights.WeightInitDistribution
Imports AfterClass = org.junit.AfterClass
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports ActivationSoftmax = org.nd4j.linalg.activations.impl.ActivationSoftmax
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports LossNegativeLogLikelihood = org.nd4j.linalg.lossfunctions.impl.LossNegativeLogLikelihood
Imports ProfilerConfig = org.nd4j.linalg.profiler.ProfilerConfig
Imports ImmutableSet = org.nd4j.shade.guava.collect.ImmutableSet
Imports ClassPath = org.nd4j.shade.guava.reflect.ClassPath

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

Namespace org.deeplearning4j.nn.dtypes


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Disabled @NativeTag @Tag(TagNames.DL4J_OLD_API) public class DTypeTests extends org.deeplearning4j.BaseDL4JTest
	Public Class DTypeTests
		Inherits BaseDL4JTest

		Protected Friend Shared seenLayers As ISet(Of Type) = New HashSet(Of Type)()
		Protected Friend Shared seenPreprocs As ISet(Of Type) = New HashSet(Of Type)()
		Protected Friend Shared seenVertices As ISet(Of Type) = New HashSet(Of Type)()

		Protected Friend Shared ignoreClasses As ISet(Of Type) = New HashSet(Of Type)(Arrays.asList(Of Type)(GetType(Pooling2D), GetType(Convolution2D), GetType(Pooling1D), GetType(Convolution1D), GetType(TensorFlowCnnToFeedForwardPreProcessor)))

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 9999999L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterClass public static void after()
		Public Shared Sub after()
			Dim info As ImmutableSet(Of ClassPath.ClassInfo)
			Try
				'Dependency note: this ClassPath class was added in Guava 14
				info = ClassPath.from(GetType(DTypeTests).getClassLoader()).getTopLevelClassesRecursive("org.deeplearning4j")
			Catch e As IOException
				'Should never happen
				Throw New Exception(e)
			End Try

			Dim layerClasses As ISet(Of Type) = New HashSet(Of Type)()
			Dim preprocClasses As ISet(Of Type) = New HashSet(Of Type)()
			Dim vertexClasses As ISet(Of Type) = New HashSet(Of Type)()
			For Each ci As ClassPath.ClassInfo In info
				Dim clazz As Type = DL4JClassLoading.loadClassByName(ci.getName())

				If Modifier.isAbstract(clazz.getModifiers()) OrElse clazz.IsInterface OrElse GetType(TFOpLayer) = clazz Then
					' Skip TFOpLayer here - dtype depends on imported model dtype
					Continue For
				End If

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				If clazz.FullName.ToLower().Contains("custom") OrElse clazz.FullName.Contains("samediff.testlayers") OrElse clazz.FullName.ToLower().Contains("test") OrElse ignoreClasses.Contains(clazz) Then
					Continue For
				End If

				If clazz.IsAssignableFrom(GetType(Layer)) Then
					layerClasses.Add(clazz)
				ElseIf clazz.IsAssignableFrom(GetType(InputPreProcessor)) Then
					preprocClasses.Add(clazz)
				ElseIf clazz.IsAssignableFrom(GetType(GraphVertex)) Then
					vertexClasses.Add(clazz)
				End If
			Next ci

			Dim fail As Boolean = False
			If seenLayers.Count < layerClasses.Count Then
				For Each c As Type In layerClasses
					If Not seenLayers.Contains(c) AndAlso Not ignoreClasses.Contains(c) Then
						log.warn("Layer class not tested for global vs. network datatypes: {}", c)
						fail = True
					End If
				Next c
			End If
			If seenPreprocs.Count < preprocClasses.Count Then
				For Each c As Type In preprocClasses
					If Not seenPreprocs.Contains(c) AndAlso Not ignoreClasses.Contains(c) Then
						log.warn("Preprocessor class not tested for global vs. network datatypes: {}", c)
						fail = True
					End If
				Next c
			End If
			If seenVertices.Count < vertexClasses.Count Then
				For Each c As Type In vertexClasses
					If Not seenVertices.Contains(c) AndAlso Not ignoreClasses.Contains(c) Then
						log.warn("GraphVertex class not tested for global vs. network datatypes: {}", c)
						fail = True
					End If
				Next c
			End If

	'        if (fail) {
	'            fail("Tested " + seenLayers.size() + " of " + layerClasses.size() + " layers, " + seenPreprocs.size() + " of " + preprocClasses.size() +
	'                    " preprocessors, " + seenVertices.size() + " of " + vertexClasses.size() + " vertices");
	'        }
		End Sub

		Public Shared Sub logUsedClasses(ByVal net As MultiLayerNetwork)
			Dim conf As MultiLayerConfiguration = net.LayerWiseConfigurations
			For Each nnc As NeuralNetConfiguration In conf.getConfs()
				Dim l As Layer = nnc.getLayer()
				seenLayers.Add(l.GetType())
				If TypeOf l Is BaseWrapperLayer Then
					Dim bwl As BaseWrapperLayer = DirectCast(l, BaseWrapperLayer)
					seenLayers.Add(bwl.getUnderlying().GetType())
				ElseIf TypeOf l Is Bidirectional Then
					seenLayers.Add(DirectCast(l, Bidirectional).getFwd().GetType())
				End If
			Next nnc

			Dim preprocs As IDictionary(Of Integer, InputPreProcessor) = conf.getInputPreProcessors()
			If preprocs IsNot Nothing Then
				For Each ipp As InputPreProcessor In preprocs.Values
					seenPreprocs.Add(ipp.GetType())
				Next ipp
			End If
		End Sub

		Public Shared Sub logUsedClasses(ByVal net As ComputationGraph)
			Dim conf As ComputationGraphConfiguration = net.Configuration
			For Each gv As GraphVertex In conf.getVertices().values()
				seenVertices.Add(gv.GetType())
				If TypeOf gv Is LayerVertex Then
					seenLayers.Add(DirectCast(gv, LayerVertex).getLayerConf().getLayer().GetType())
					Dim ipp As InputPreProcessor = DirectCast(gv, LayerVertex).PreProcessor
					If ipp IsNot Nothing Then
						seenPreprocs.Add(ipp.GetType())
					End If
				ElseIf TypeOf gv Is PreprocessorVertex Then
					seenPreprocs.Add(DirectCast(gv, PreprocessorVertex).getPreProcessor().GetType())
				End If
			Next gv

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMultiLayerNetworkTypeConversion()
		Public Overridable Sub testMultiLayerNetworkTypeConversion()

			For Each dt As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				Nd4j.setDefaultDataTypes(dt, dt)

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).weightInit(WeightInit.XAVIER).updater(New Adam(0.01)).dataType(DataType.DOUBLE).list().layer((New DenseLayer.Builder()).activation(Activation.TANH).nIn(10).nOut(10).build()).layer((New DenseLayer.Builder()).activation(Activation.TANH).nIn(10).nOut(10).build()).layer((New OutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim inD As INDArray = Nd4j.rand(DataType.DOUBLE, 1, 10)
				Dim lD As INDArray = Nd4j.create(DataType.DOUBLE, 1, 10)
				net.fit(inD, lD)

				Dim outDouble As INDArray = net.output(inD)
				net.Input = inD
				net.Labels = lD
				net.computeGradientAndScore()
				Dim scoreDouble As Double = net.score()
				Dim grads As INDArray = net.getFlattenedGradients()
				Dim u As INDArray = net.Updater.StateViewArray
				assertEquals(DataType.DOUBLE, net.params().dataType())
				assertEquals(DataType.DOUBLE, grads.dataType())
				assertEquals(DataType.DOUBLE, u.dataType())


				Dim netFloat As MultiLayerNetwork = net.convertDataType(DataType.FLOAT)
				netFloat.initGradientsView()
				assertEquals(DataType.FLOAT, netFloat.params().dataType())
				assertEquals(DataType.FLOAT, netFloat.getFlattenedGradients().dataType())
				assertEquals(DataType.FLOAT, netFloat.getUpdater(True).StateViewArray.dataType())
				Dim inF As INDArray = inD.castTo(DataType.FLOAT)
				Dim lF As INDArray = lD.castTo(DataType.FLOAT)
				Dim outFloat As INDArray = netFloat.output(inF)
				netFloat.Input = inF
				netFloat.Labels = lF
				netFloat.computeGradientAndScore()
				Dim scoreFloat As Double = netFloat.score()
				Dim gradsFloat As INDArray = netFloat.getFlattenedGradients()
				Dim uFloat As INDArray = netFloat.Updater.StateViewArray

				assertEquals(scoreDouble, scoreFloat, 1e-6)
				assertEquals(outDouble.castTo(DataType.FLOAT), outFloat)
				assertEquals(grads.castTo(DataType.FLOAT), gradsFloat)
				Dim uCast As INDArray = u.castTo(DataType.FLOAT)
				assertTrue(uCast.equalsWithEps(uFloat, 1e-4))

				Dim netFP16 As MultiLayerNetwork = net.convertDataType(DataType.HALF)
				netFP16.initGradientsView()
				assertEquals(DataType.HALF, netFP16.params().dataType())
				assertEquals(DataType.HALF, netFP16.getFlattenedGradients().dataType())
				assertEquals(DataType.HALF, netFP16.getUpdater(True).StateViewArray.dataType())

				Dim inH As INDArray = inD.castTo(DataType.HALF)
				Dim lH As INDArray = lD.castTo(DataType.HALF)
				Dim outHalf As INDArray = netFP16.output(inH)
				netFP16.Input = inH
				netFP16.Labels = lH
				netFP16.computeGradientAndScore()
				Dim scoreHalf As Double = netFP16.score()
				Dim gradsHalf As INDArray = netFP16.getFlattenedGradients()
				Dim uHalf As INDArray = netFP16.Updater.StateViewArray

				assertEquals(scoreDouble, scoreHalf, 1e-4)
				Dim outHalfEq As Boolean = outDouble.castTo(DataType.HALF).equalsWithEps(outHalf, 1e-3)
				assertTrue(outHalfEq)
				Dim gradsHalfEq As Boolean = grads.castTo(DataType.HALF).equalsWithEps(gradsHalf, 1e-3)
				assertTrue(gradsHalfEq)
				Dim uHalfCast As INDArray = u.castTo(DataType.HALF)
				assertTrue(uHalfCast.equalsWithEps(uHalf, 1e-4))
			Next dt
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testComputationGraphTypeConversion()
		Public Overridable Sub testComputationGraphTypeConversion()

			For Each dt As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				Nd4j.setDefaultDataTypes(dt, dt)

				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).weightInit(WeightInit.XAVIER).updater(New Adam(0.01)).dataType(DataType.DOUBLE).graphBuilder().addInputs("in").layer("l0", (New DenseLayer.Builder()).activation(Activation.TANH).nIn(10).nOut(10).build(), "in").layer("l1", (New DenseLayer.Builder()).activation(Activation.TANH).nIn(10).nOut(10).build(), "l0").layer("out", (New OutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "l1").setOutputs("out").build()

				Dim net As New ComputationGraph(conf)
				net.init()

				Dim inD As INDArray = Nd4j.rand(DataType.DOUBLE, 1, 10)
				Dim lD As INDArray = Nd4j.create(DataType.DOUBLE, 1, 10)
				net.fit(New DataSet(inD, lD))

				Dim outDouble As INDArray = net.outputSingle(inD)
				net.setInput(0, inD)
				net.Labels = lD
				net.computeGradientAndScore()
				Dim scoreDouble As Double = net.score()
				Dim grads As INDArray = net.getFlattenedGradients()
				Dim u As INDArray = net.Updater.StateViewArray
				assertEquals(DataType.DOUBLE, net.params().dataType())
				assertEquals(DataType.DOUBLE, grads.dataType())
				assertEquals(DataType.DOUBLE, u.dataType())


				Dim netFloat As ComputationGraph = net.convertDataType(DataType.FLOAT)
				netFloat.initGradientsView()
				assertEquals(DataType.FLOAT, netFloat.params().dataType())
				assertEquals(DataType.FLOAT, netFloat.getFlattenedGradients().dataType())
				assertEquals(DataType.FLOAT, netFloat.getUpdater(True).StateViewArray.dataType())
				Dim inF As INDArray = inD.castTo(DataType.FLOAT)
				Dim lF As INDArray = lD.castTo(DataType.FLOAT)
				Dim outFloat As INDArray = netFloat.outputSingle(inF)
				netFloat.setInput(0, inF)
				netFloat.Labels = lF
				netFloat.computeGradientAndScore()
				Dim scoreFloat As Double = netFloat.score()
				Dim gradsFloat As INDArray = netFloat.getFlattenedGradients()
				Dim uFloat As INDArray = netFloat.Updater.StateViewArray

				assertEquals(scoreDouble, scoreFloat, 1e-6)
				assertEquals(outDouble.castTo(DataType.FLOAT), outFloat)
				assertEquals(grads.castTo(DataType.FLOAT), gradsFloat)
				Dim uCast As INDArray = u.castTo(DataType.FLOAT)
				assertTrue(uCast.equalsWithEps(uFloat, 1e-4))

				Dim netFP16 As ComputationGraph = net.convertDataType(DataType.HALF)
				netFP16.initGradientsView()
				assertEquals(DataType.HALF, netFP16.params().dataType())
				assertEquals(DataType.HALF, netFP16.getFlattenedGradients().dataType())
				assertEquals(DataType.HALF, netFP16.getUpdater(True).StateViewArray.dataType())

				Dim inH As INDArray = inD.castTo(DataType.HALF)
				Dim lH As INDArray = lD.castTo(DataType.HALF)
				Dim outHalf As INDArray = netFP16.outputSingle(inH)
				netFP16.setInput(0, inH)
				netFP16.Labels = lH
				netFP16.computeGradientAndScore()
				Dim scoreHalf As Double = netFP16.score()
				Dim gradsHalf As INDArray = netFP16.getFlattenedGradients()
				Dim uHalf As INDArray = netFP16.Updater.StateViewArray

				assertEquals(scoreDouble, scoreHalf, 1e-4)
				Dim outHalfEq As Boolean = outDouble.castTo(DataType.HALF).equalsWithEps(outHalf, 1e-3)
				assertTrue(outHalfEq)
				Dim gradsHalfEq As Boolean = grads.castTo(DataType.HALF).equalsWithEps(gradsHalf, 1e-3)
				assertTrue(gradsHalfEq)
				Dim uHalfCast As INDArray = u.castTo(DataType.HALF)
				assertTrue(uHalfCast.equalsWithEps(uHalf, 1e-4))
			Next dt
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDtypesModelVsGlobalDtypeCnn()
		Public Overridable Sub testDtypesModelVsGlobalDtypeCnn()
			For Each globalDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				Nd4j.setDefaultDataTypes(globalDtype, globalDtype)
				For Each networkDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
					For outputLayer As Integer = 0 To 4
						assertEquals(globalDtype, Nd4j.dataType())
						assertEquals(globalDtype, Nd4j.defaultFloatingPointType())

						Dim msg As String = "Global dtype: " & globalDtype & ", network dtype: " & networkDtype & ", outputLayer=" & outputLayer

						Dim ol As Layer
						Dim secondLast As Layer
						Select Case outputLayer
							Case 0
								ol = (New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()
								secondLast = New GlobalPoolingLayer(PoolingType.MAX)
							Case 1
								ol = (New LossLayer.Builder()).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()
								secondLast = New FrozenLayerWithBackprop((New DenseLayer.Builder()).nOut(10).activation(Activation.SIGMOID).build())
							Case 2
								ol = (New CenterLossOutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()
								secondLast = (New VariationalAutoencoder.Builder()).encoderLayerSizes(10).decoderLayerSizes(10).nOut(10).activation(Activation.SIGMOID).build()
							Case 3
								ol = (New CnnLossLayer.Builder()).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()
								secondLast = (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).nOut(3).activation(Activation.TANH).build()
							Case 4
								ol = (New Yolo2OutputLayer.Builder()).boundingBoxPriors(Nd4j.create(New Double()(){
									New Double() {1.0, 1.0},
									New Double() {2.0, 2.0}
								}).castTo(networkDtype)).build()
								secondLast = (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).nOut(14).activation(Activation.TANH).build()
							Case Else
								Throw New Exception()
						End Select


						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(networkDtype).convolutionMode(ConvolutionMode.Same).updater(New Adam(1e-2)).list().layer((New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).nOut(3).activation(Activation.TANH).build()).layer(New LocalResponseNormalization()).layer(New DropoutLayer(0.5)).layer(New DropoutLayer(New AlphaDropout(0.5))).layer(New DropoutLayer(New GaussianDropout(0.5))).layer(New DropoutLayer(New GaussianNoise(0.1))).layer(New DropoutLayer(New SpatialDropout(0.5))).layer((New SubsamplingLayer.Builder()).poolingType(SubsamplingLayer.PoolingType.AVG).kernelSize(3, 3).stride(2, 2).build()).layer((New Pooling2D.Builder()).poolingType(SubsamplingLayer.PoolingType.AVG).kernelSize(2, 2).stride(1, 1).build()).layer((New Deconvolution2D.Builder()).kernelSize(2, 2).stride(2, 2).nOut(3).activation(Activation.TANH).build()).layer(New ZeroPaddingLayer(1, 1)).layer(New Cropping2D(1, 1)).layer(New IdentityLayer()).layer((New Upsampling2D.Builder()).size(2).build()).layer((New SubsamplingLayer.Builder()).kernelSize(2, 2).stride(2, 2).build()).layer((New DepthwiseConvolution2D.Builder()).nOut(3).activation(Activation.RELU).build()).layer((New SeparableConvolution2D.Builder()).nOut(3).activation(Activation.HARDTANH).build()).layer(New MaskLayer()).layer((New BatchNormalization.Builder()).build()).layer(New ActivationLayer(Activation.LEAKYRELU)).layer(secondLast).layer(ol).setInputType(InputType.convolutionalFlat(8, 8, 1)).build()

						Dim net As New MultiLayerNetwork(conf)
						net.init()

						net.initGradientsView()
						assertEquals(networkDtype, net.params().dataType(), msg)
						assertEquals(networkDtype, net.getFlattenedGradients().dataType(), msg)
						assertEquals(networkDtype, net.getUpdater(True).StateViewArray.dataType(), msg)

						Dim [in] As INDArray = Nd4j.rand(networkDtype, 2, 8 * 8)
						Dim label As INDArray
						If outputLayer < 3 Then
							label = TestUtils.randomOneHot(2, 10).castTo(networkDtype)
						ElseIf outputLayer = 3 Then
							'CNN loss
							label = Nd4j.rand(networkDtype, 2, 3, 8, 8)
						ElseIf outputLayer = 4 Then
							'YOLO
							label = Nd4j.ones(networkDtype, 2, 6, 8, 8)
						Else
							Throw New System.InvalidOperationException()
						End If

						Dim [out] As INDArray = net.output([in])
						assertEquals(networkDtype, [out].dataType(), msg)
						Dim ff As IList(Of INDArray) = net.feedForward([in])
						For i As Integer = 0 To ff.Count - 1
							Dim s As String = msg & " - layer " & (i - 1) & " - " & (If(i = 0, "input", net.getLayer(i - 1).conf().getLayer().GetType().Name))
							assertEquals(networkDtype, ff(i).dataType(), s)
						Next i

						net.Input = [in]
						net.Labels = label
						net.computeGradientAndScore()

						net.fit(New DataSet([in], label))

						logUsedClasses(net)

						'Now, test mismatched dtypes for input/labels:
						For Each inputLabelDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
							log.info(msg & " - input/label type: " & inputLabelDtype)
							Dim in2 As INDArray = [in].castTo(inputLabelDtype)
							Dim label2 As INDArray = label.castTo(inputLabelDtype)
							net.output(in2)
							net.Input = in2
							net.Labels = label2
							net.computeGradientAndScore()

							net.fit(New DataSet(in2, label2))
						Next inputLabelDtype
					Next outputLayer
				Next networkDtype
			Next globalDtype
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDtypesModelVsGlobalDtypeCnn3d()
		Public Overridable Sub testDtypesModelVsGlobalDtypeCnn3d()
			For Each globalDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				Nd4j.setDefaultDataTypes(globalDtype, globalDtype)
				For Each networkDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
					For outputLayer As Integer = 0 To 2
						assertEquals(globalDtype, Nd4j.dataType())
						assertEquals(globalDtype, Nd4j.defaultFloatingPointType())

						Dim msg As String = "Global dtype: " & globalDtype & ", network dtype: " & networkDtype & ", outputLayer=" & outputLayer
						log.info(msg)

						Dim ol As Layer
						Dim secondLast As Layer
						Select Case outputLayer
							Case 0
								ol = (New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()
								secondLast = New GlobalPoolingLayer(PoolingType.AVG)
							Case 1
								ol = (New Cnn3DLossLayer.Builder(Convolution3D.DataFormat.NCDHW)).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()
								secondLast = (New Convolution3D.Builder()).nOut(3).activation(Activation.ELU).build()
							Case 2
								ol = (New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()
								secondLast = (New Convolution3D.Builder()).nOut(3).activation(Activation.ELU).build()
							Case Else
								Throw New Exception()
						End Select


						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(networkDtype).convolutionMode(ConvolutionMode.Same).updater(New Nesterovs(1e-2, 0.9)).list().layer((New Convolution3D.Builder()).kernelSize(2, 2, 2).stride(1, 1, 1).nOut(3).activation(Activation.TANH).build()).layer((New Convolution3D.Builder()).kernelSize(2, 2, 2).stride(1, 1, 1).nOut(3).activation(Activation.TANH).build()).layer((New Subsampling3DLayer.Builder()).poolingType(PoolingType.AVG).kernelSize(2, 2, 2).stride(2, 2, 2).build()).layer((New Deconvolution3D.Builder()).kernelSize(2,2,2).stride(1,1,1).nIn(3).nOut(3).activation(Activation.TANH).build()).layer((New Cropping3D.Builder(1, 1, 1, 1, 1, 1)).build()).layer((New ZeroPadding3DLayer.Builder(1, 1, 1, 1, 1, 1)).build()).layer(New ActivationLayer(Activation.LEAKYRELU)).layer((New Upsampling3D.Builder()).size(2).build()).layer(secondLast).layer(ol).setInputType(InputType.convolutional3D(Convolution3D.DataFormat.NCDHW, 8, 8, 8, 1)).build()

						Dim net As New MultiLayerNetwork(conf)
						net.init()

						net.initGradientsView()
						assertEquals(networkDtype, net.params().dataType(), msg)
						assertEquals(networkDtype, net.getFlattenedGradients().dataType(), msg)
						assertEquals(networkDtype, net.getUpdater(True).StateViewArray.dataType(), msg)

						Dim [in] As INDArray = Nd4j.rand(networkDtype, 2, 1, 8, 8, 8)
						Dim label As INDArray
						If outputLayer = 0 Then
							label = TestUtils.randomOneHot(2, 10).castTo(networkDtype)
						ElseIf outputLayer = 1 Then
							'CNN3D loss
							label = Nd4j.rand(networkDtype, 2, 3, 8, 8, 8)
						ElseIf outputLayer = 2 Then
							label = TestUtils.randomOneHot(2, 10).castTo(networkDtype)
						Else
							Throw New Exception()
						End If

						Dim [out] As INDArray = net.output([in])
						assertEquals(networkDtype, [out].dataType(), msg)
						Dim ff As IList(Of INDArray) = net.feedForward([in])
						For i As Integer = 0 To ff.Count - 1
							Dim s As String = msg & " - layer " & (i - 1) & " - " & (If(i = 0, "input", net.getLayer(i - 1).conf().getLayer().GetType().Name))
							assertEquals(networkDtype, ff(i).dataType(), s)
						Next i

						net.Input = [in]
						net.Labels = label
						net.computeGradientAndScore()

						net.fit(New DataSet([in], label))

						logUsedClasses(net)

						'Now, test mismatched dtypes for input/labels:
						For Each inputLabelDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
							Dim in2 As INDArray = [in].castTo(inputLabelDtype)
							Dim label2 As INDArray = label.castTo(inputLabelDtype)
							net.output(in2)
							net.Input = in2
							net.Labels = label2
							net.computeGradientAndScore()

							net.fit(New DataSet(in2, label2))
						Next inputLabelDtype
					Next outputLayer
				Next networkDtype
			Next globalDtype
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testDtypesModelVsGlobalDtypeCnn1d()
		Public Overridable Sub testDtypesModelVsGlobalDtypeCnn1d()
			'Nd4jCpu.Environment.getInstance().setUseMKLDNN(false);
			Nd4j.Environment.Debug = True
			Nd4j.Executioner.enableVerboseMode(True)
			Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().checkForNAN(True).checkWorkspaces(True).checkForINF(True).build()
			For Each globalDtype As DataType In New DataType(){DataType.DOUBLE}
				Nd4j.setDefaultDataTypes(globalDtype, globalDtype)
				For Each networkDtype As DataType In New DataType(){DataType.DOUBLE}
					For outputLayer As Integer = 0 To 2
						assertEquals(globalDtype, Nd4j.dataType())
						assertEquals(globalDtype, Nd4j.defaultFloatingPointType())

						Dim msg As String = "Global dtype: " & globalDtype & ", network dtype: " & networkDtype & ", outputLayer=" & outputLayer & " at index " & outputLayer

						Dim ol As Layer
						Dim secondLast As Layer
						Select Case outputLayer
							Case 0
								ol = (New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()
								secondLast = New GlobalPoolingLayer(PoolingType.MAX)
							Case 1
								ol = (New RnnOutputLayer.Builder()).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).nOut(5).build()
								secondLast = (New Convolution1D.Builder()).kernelSize(2).nOut(5).build()
							Case 2
								ol = (New RnnLossLayer.Builder()).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()
								secondLast = (New Convolution1D.Builder()).kernelSize(2).nOut(5).build()
							Case Else
								Throw New Exception()
						End Select


						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).trainingWorkspaceMode(WorkspaceMode.NONE).inferenceWorkspaceMode(WorkspaceMode.NONE).dataType(networkDtype).convolutionMode(ConvolutionMode.Same).updater(New Adam(1e-2)).list().layer((New Convolution1D.Builder()).kernelSize(2).stride(1).nOut(3).activation(Activation.TANH).build()).layer((New Subsampling1DLayer.Builder()).poolingType(PoolingType.MAX).kernelSize(5).stride(1).build()).layer((New Cropping1D.Builder(1)).build()).layer(New ZeroPadding1DLayer(1)).layer((New Upsampling1D.Builder(2)).build()).layer(secondLast).layer(ol).setInputType(InputType.recurrent(5, 10,RNNFormat.NCW)).build()

						Dim net As New MultiLayerNetwork(conf)
						net.init()

						net.initGradientsView()
						assertEquals(networkDtype, net.params().dataType(), msg)
						assertEquals(networkDtype, net.getFlattenedGradients().dataType(), msg)
						assertEquals(networkDtype, net.getUpdater(True).StateViewArray.dataType(), msg)

						Dim [in] As INDArray = Nd4j.rand(networkDtype, 2, 5, 10)
						Dim label As INDArray
						If outputLayer = 0 Then
							'OutputLayer
							label = TestUtils.randomOneHot(2, 10).castTo(networkDtype)
						Else
							'RnnOutputLayer, RnnLossLayer
							label = Nd4j.rand(networkDtype, 2, 5, 20) 'Longer sequence due to upsampling
						End If

						Dim [out] As INDArray = net.output([in])
						assertEquals(networkDtype, [out].dataType(), msg)
						Dim ff As IList(Of INDArray) = net.feedForward([in])
						For i As Integer = 0 To ff.Count - 1
							Dim s As String = msg & " - layer " & (i - 1) & " - " & (If(i = 0, "input", net.getLayer(i - 1).conf().getLayer().GetType().Name))
							assertEquals(networkDtype, ff(i).dataType(), s)
						Next i

						net.Input = [in]
						net.Labels = label
						net.computeGradientAndScore()

						'net.fit(new DataSet(in, label));

						logUsedClasses(net)

						'Now, test mismatched dtypes for input/labels:
						For Each inputLabelDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT}
							Console.WriteLine(msg & " - " & inputLabelDtype)
							Dim in2 As INDArray = [in].castTo(inputLabelDtype)
							Dim label2 As INDArray = label.castTo(inputLabelDtype)
							net.output(in2)
							net.Input = in2
							net.Labels = label2
							net.computeGradientAndScore()

							'net.fit(new DataSet(in2, label2));
						Next inputLabelDtype
					Next outputLayer
				Next networkDtype
			Next globalDtype
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDtypesModelVsGlobalDtypeMisc()
		Public Overridable Sub testDtypesModelVsGlobalDtypeMisc()
			For Each globalDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				Nd4j.setDefaultDataTypes(globalDtype, globalDtype)
				For Each networkDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
					assertEquals(globalDtype, Nd4j.dataType())
					assertEquals(globalDtype, Nd4j.defaultFloatingPointType())

					Dim msg As String = "Global dtype: " & globalDtype & ", network dtype: " & networkDtype


					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(networkDtype).convolutionMode(ConvolutionMode.Same).updater(New Adam(1e-2)).list().layer((New SpaceToBatchLayer.Builder()).blocks(1, 1).build()).layer((New SpaceToDepthLayer.Builder()).blocks(2).build()).layer((New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).setInputType(InputType.convolutional(28, 28, 5)).build()

					Dim net As New MultiLayerNetwork(conf)
					net.init()

					net.initGradientsView()
					assertEquals(networkDtype, net.params().dataType(), msg)
					assertEquals(networkDtype, net.getFlattenedGradients().dataType(), msg)
					assertEquals(networkDtype, net.getUpdater(True).StateViewArray.dataType(), msg)

					Dim [in] As INDArray = Nd4j.rand(networkDtype, 2, 5, 28, 28)
					Dim label As INDArray = TestUtils.randomOneHot(2, 10).castTo(networkDtype)

					Dim [out] As INDArray = net.output([in])
					assertEquals(networkDtype, [out].dataType(), msg)
					Dim ff As IList(Of INDArray) = net.feedForward([in])
					For i As Integer = 0 To ff.Count - 1
						Dim s As String = msg & " - layer " & (i - 1) & " - " & (If(i = 0, "input", net.getLayer(i - 1).conf().getLayer().GetType().Name))
						assertEquals(networkDtype, ff(i).dataType(), s)
					Next i

					net.Input = [in]
					net.Labels = label
					net.computeGradientAndScore()

					net.fit(New DataSet([in], label))

					logUsedClasses(net)

					'Now, test mismatched dtypes for input/labels:
					For Each inputLabelDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
						Dim in2 As INDArray = [in].castTo(inputLabelDtype)
						Dim label2 As INDArray = label.castTo(inputLabelDtype)
						net.output(in2)
						net.Input = in2
						net.Labels = label2
						net.computeGradientAndScore()

						net.fit(New DataSet(in2, label2))
					Next inputLabelDtype
				Next networkDtype
			Next globalDtype
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDtypesModelVsGlobalDtypeRnn()
		Public Overridable Sub testDtypesModelVsGlobalDtypeRnn()
			For Each globalDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				Nd4j.setDefaultDataTypes(globalDtype, globalDtype)
				For Each networkDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
					For outputLayer As Integer = 0 To 2
						assertEquals(globalDtype, Nd4j.dataType())
						assertEquals(globalDtype, Nd4j.defaultFloatingPointType())

						Dim msg As String = "Global dtype: " & globalDtype & ", network dtype: " & networkDtype & ", outputLayer=" & outputLayer

						Dim ol As Layer
						Dim secondLast As Layer
						Select Case outputLayer
							Case 0
								ol = (New RnnOutputLayer.Builder()).nOut(5).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()
								secondLast = (New SimpleRnn.Builder()).nOut(5).activation(Activation.TANH).build()
							Case 1
								ol = (New RnnLossLayer.Builder()).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()
								secondLast = (New SimpleRnn.Builder()).nOut(5).activation(Activation.TANH).build()
							Case 2
								ol = (New OutputLayer.Builder()).nOut(5).build()
								secondLast = New LastTimeStep((New SimpleRnn.Builder()).nOut(5).activation(Activation.TANH).build())
							Case Else
								Throw New Exception()
						End Select

						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(networkDtype).convolutionMode(ConvolutionMode.Same).updater(New Adam(1e-2)).list().layer((New LSTM.Builder()).nIn(5).nOut(5).activation(Activation.TANH).build()).layer((New GravesLSTM.Builder()).nIn(5).nOut(5).activation(Activation.TANH).build()).layer((New DenseLayer.Builder()).nOut(5).build()).layer((New GravesBidirectionalLSTM.Builder()).nIn(5).nOut(5).activation(Activation.TANH).build()).layer(New Bidirectional((New LSTM.Builder()).nIn(5).nOut(5).activation(Activation.TANH).build())).layer(New TimeDistributed((New DenseLayer.Builder()).nIn(10).nOut(5).activation(Activation.TANH).build())).layer((New SimpleRnn.Builder()).nIn(5).nOut(5).build()).layer((New MaskZeroLayer.Builder()).underlying((New SimpleRnn.Builder()).nIn(5).nOut(5).build()).maskValue(0.0).build()).layer(secondLast).layer(ol).build()

						Dim net As New MultiLayerNetwork(conf)
						net.init()

						net.initGradientsView()
						assertEquals(networkDtype, net.params().dataType(), msg)
						assertEquals(networkDtype, net.getFlattenedGradients().dataType(), msg)
						assertEquals(networkDtype, net.getUpdater(True).StateViewArray.dataType(), msg)

						Dim [in] As INDArray = Nd4j.rand(networkDtype, 2, 5, 2)
						Dim label As INDArray
						If outputLayer = 2 Then
							label = TestUtils.randomOneHot(2, 5).castTo(networkDtype)
						Else
							label = TestUtils.randomOneHotTimeSeries(2, 5, 2).castTo(networkDtype)
						End If


						Dim [out] As INDArray = net.output([in])
						assertEquals(networkDtype, [out].dataType(), msg)
						Dim ff As IList(Of INDArray) = net.feedForward([in])
						For i As Integer = 0 To ff.Count - 1
							assertEquals(networkDtype, ff(i).dataType(), msg)
						Next i

						net.Input = [in]
						net.Labels = label
						net.computeGradientAndScore()

						net.fit(New DataSet([in], label, Nd4j.ones(networkDtype, 2, 2),If(outputLayer = 2, Nothing, Nd4j.ones(networkDtype, 2, 2))))

						logUsedClasses(net)

						'Now, test mismatched dtypes for input/labels:
						For Each inputLabelDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
							Dim in2 As INDArray = [in].castTo(inputLabelDtype)
							Dim label2 As INDArray = label.castTo(inputLabelDtype)
							net.output(in2)
							net.Input = in2
							net.Labels = label2
							net.computeGradientAndScore()

							net.fit(New DataSet(in2, label2))
						Next inputLabelDtype
					Next outputLayer
				Next networkDtype
			Next globalDtype
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCapsNetDtypes()
		Public Overridable Sub testCapsNetDtypes()
			For Each globalDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				Nd4j.setDefaultDataTypes(globalDtype, globalDtype)
				For Each networkDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
					assertEquals(globalDtype, Nd4j.dataType())
					assertEquals(globalDtype, Nd4j.defaultFloatingPointType())

					Dim msg As String = "Global dtype: " & globalDtype & ", network dtype: " & networkDtype

					Dim primaryCapsDim As Integer = 2
					Dim primarpCapsChannel As Integer = 8
					Dim capsule As Integer = 5
					Dim minibatchSize As Integer = 8
					Dim routing As Integer = 1
					Dim capsuleDim As Integer = 4
					Dim height As Integer = 6
					Dim width As Integer = 6
					Dim inputDepth As Integer = 4

					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(networkDtype).seed(123).updater(New NoOp()).weightInit(New WeightInitDistribution(New UniformDistribution(-6, 6))).list().layer((New PrimaryCapsules.Builder(primaryCapsDim, primarpCapsChannel)).kernelSize(3, 3).stride(2, 2).build()).layer((New CapsuleLayer.Builder(capsule, capsuleDim, routing)).build()).layer((New CapsuleStrengthLayer.Builder()).build()).layer((New ActivationLayer.Builder(New ActivationSoftmax())).build()).layer((New LossLayer.Builder(New LossNegativeLogLikelihood())).build()).setInputType(InputType.convolutional(height, width, inputDepth)).build()

					Dim net As New MultiLayerNetwork(conf)
					net.init()

					Dim [in] As INDArray = Nd4j.rand(networkDtype, minibatchSize, inputDepth * height * width).mul(10).reshape(-1, inputDepth, height, width)
					Dim label As INDArray = Nd4j.zeros(networkDtype, minibatchSize, capsule)
					For i As Integer = 0 To minibatchSize - 1
						label.putScalar(New Integer(){i, i Mod capsule}, 1.0)
					Next i

					Dim [out] As INDArray = net.output([in])
					assertEquals(networkDtype, [out].dataType(), msg)
					Dim ff As IList(Of INDArray) = net.feedForward([in])
					For i As Integer = 0 To ff.Count - 1
						Dim s As String = msg & " - layer " & (i - 1) & " - " & (If(i = 0, "input", net.getLayer(i - 1).conf().getLayer().GetType().Name))
						assertEquals(networkDtype, ff(i).dataType(), s)
					Next i

					net.Input = [in]
					net.Labels = label
					net.computeGradientAndScore()

					net.fit(New DataSet([in], label))

					logUsedClasses(net)

					'Now, test mismatched dtypes for input/labels:
					For Each inputLabelDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
						Dim in2 As INDArray = [in].castTo(inputLabelDtype)
						Dim label2 As INDArray = label.castTo(inputLabelDtype)
						net.output(in2)
						net.Input = in2
						net.Labels = label2
						net.computeGradientAndScore()

						net.fit(New DataSet(in2, label2))
					Next inputLabelDtype
				Next networkDtype
			Next globalDtype
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEmbeddingDtypes()
		Public Overridable Sub testEmbeddingDtypes()
			For Each globalDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				Nd4j.setDefaultDataTypes(globalDtype, globalDtype)
				For Each networkDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
					For Each frozen As Boolean In New Boolean(){False, True}
						For test As Integer = 0 To 2
							assertEquals(globalDtype, Nd4j.dataType())
							assertEquals(globalDtype, Nd4j.defaultFloatingPointType())

							Dim msg As String = "Global dtype: " & globalDtype & ", network dtype: " & networkDtype & ", test=" & test

							Dim conf As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).dataType(networkDtype).seed(123).updater(New NoOp()).weightInit(New WeightInitDistribution(New UniformDistribution(-6, 6))).graphBuilder().addInputs("in").setOutputs("out")

							Dim input As INDArray
							If test = 0 Then
								If frozen Then
									conf.layer("0", New FrozenLayer((New EmbeddingLayer.Builder()).nIn(5).nOut(5).build()), "in")
								Else
									conf.layer("0", (New EmbeddingLayer.Builder()).nIn(5).nOut(5).build(), "in")
								End If

								input = Nd4j.zeros(networkDtype, 10, 1).muli(5).castTo(DataType.INT)
								conf.InputTypes = InputType.feedForward(1)
							ElseIf test = 1 Then
								If frozen Then
									conf.layer("0", New FrozenLayer((New EmbeddingSequenceLayer.Builder()).nIn(5).nOut(5).build()), "in")
								Else
									conf.layer("0", (New EmbeddingSequenceLayer.Builder()).nIn(5).nOut(5).build(), "in")
								End If
								conf.layer("gp", (New GlobalPoolingLayer.Builder(PoolingType.PNORM)).pnorm(2).poolingDimensions(2).build(), "0")
								input = Nd4j.zeros(networkDtype, 10, 1, 5).muli(5).castTo(DataType.INT)
								conf.InputTypes = InputType.recurrent(1)
							Else
								conf.layer("0", (New RepeatVector.Builder()).repetitionFactor(5).nOut(5).build(), "in")
								conf.layer("gp", (New GlobalPoolingLayer.Builder(PoolingType.SUM)).build(), "0")
								input = Nd4j.zeros(networkDtype, 10, 5)
								conf.InputTypes = InputType.feedForward(5)
							End If

							conf.appendLayer("el", (New ElementWiseMultiplicationLayer.Builder()).nOut(5).build()).appendLayer("ae", (New AutoEncoder.Builder()).nOut(5).build()).appendLayer("prelu", (New PReLULayer.Builder()).nOut(5).inputShape(5).build()).appendLayer("out", (New OutputLayer.Builder()).nOut(10).build())

							Dim net As New ComputationGraph(conf.build())
							net.init()

							Dim label As INDArray = Nd4j.zeros(networkDtype, 10, 10)

							Dim [out] As INDArray = net.outputSingle(input)
							assertEquals(networkDtype, [out].dataType(), msg)
							Dim ff As IDictionary(Of String, INDArray) = net.feedForward(input, False)
							For Each e As KeyValuePair(Of String, INDArray) In ff.SetOfKeyValuePairs()
								If e.Key.Equals("in") Then
									Continue For
								End If
								Dim s As String = msg & " - layer: " & e.Key
								assertEquals(networkDtype, e.Value.dataType(), s)
							Next e

							net.setInput(0, input)
							net.Labels = label
							net.computeGradientAndScore()

							net.fit(New DataSet(input, label))

							logUsedClasses(net)

							'Now, test mismatched dtypes for input/labels:
							For Each inputLabelDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
								Dim in2 As INDArray = input.castTo(inputLabelDtype)
								Dim label2 As INDArray = label.castTo(inputLabelDtype)
								net.output(in2)
								net.setInput(0, in2)
								net.Labels = label2
								net.computeGradientAndScore()

								net.fit(New DataSet(in2, label2))
							Next inputLabelDtype
						Next test
					Next frozen
				Next networkDtype
			Next globalDtype
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVertexDtypes()
		Public Overridable Sub testVertexDtypes()
			For Each globalDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				Nd4j.setDefaultDataTypes(globalDtype, globalDtype)
				For Each networkDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
					assertEquals(globalDtype, Nd4j.dataType())
					assertEquals(globalDtype, Nd4j.defaultFloatingPointType())

					Dim [in]() As INDArray = Nothing
					For test As Integer = 0 To 7
						Dim msg As String = "Global dtype: " & globalDtype & ", network dtype: " & networkDtype & ", test=" & test

						Dim b As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).dataType(networkDtype).seed(123).updater(New NoOp()).weightInit(WeightInit.XAVIER).convolutionMode(ConvolutionMode.Same).graphBuilder()

						Select Case test
							Case 0
								b.addInputs("in").addLayer("l", (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).nOut(1).build(), "in").addVertex("preproc", New PreprocessorVertex(New CnnToRnnPreProcessor(28, 28, 1)), "l").addLayer("out", (New OutputLayer.Builder()).nOut(10).build(), "preproc").setInputTypes(InputType.convolutional(28, 28, 1)).setOutputs("out")
								[in] = New INDArray(){Nd4j.rand(networkDtype, 2, 1, 28, 28)}
							Case 1
								b.addInputs("in").addLayer("l", (New DenseLayer.Builder()).nOut(16).build(), "in").addVertex("preproc", New PreprocessorVertex(New FeedForwardToCnn3DPreProcessor(2, 2, 2, 2, True)), "l").addVertex("preproc2", New PreprocessorVertex(New PermutePreprocessor(0, 2, 3, 4, 1)), "preproc").addVertex("preproc3", New PreprocessorVertex(New ReshapePreprocessor(New Long(){2, 2, 2, 2}, New Long(){16}, False)), "preproc2").addLayer("out", (New OutputLayer.Builder()).nIn(16).nOut(10).build(), "preproc3").setInputTypes(InputType.feedForward(5)).setOutputs("out")
								[in] = New INDArray(){Nd4j.rand(networkDtype, 2, 5)}
							Case 2
								b.addInputs("in").addLayer("1", (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).nOut(1).build(), "in").addVertex("1a", New PoolHelperVertex(), "1").addVertex("2", New ShiftVertex(1), "1a").addVertex("3", New ScaleVertex(2), "2").addVertex("4", New ReshapeVertex(2, -1), "3").addVertex("5", New SubsetVertex(0, 99), "4").addVertex("6", New L2NormalizeVertex(), "5").addLayer("out", (New OCNNOutputLayer.Builder()).hiddenLayerSize(10).nIn(100).build(), "6").setInputTypes(InputType.convolutional(28, 28, 1)).setOutputs("out")
								[in] = New INDArray(){Nd4j.rand(networkDtype, 2, 1, 28, 28)}
							Case 3
								b.addInputs("in1", "in2", "in3").addVertex("1", New ElementWiseVertex(ElementWiseVertex.Op.Add), "in1", "in2").addVertex("2a", New UnstackVertex(0, 2), "1").addVertex("2b", New UnstackVertex(1, 2), "1").addVertex("3", New StackVertex(), "2a", "2b").addVertex("4", New DuplicateToTimeSeriesVertex("in3"), "3").addVertex("5", New ReverseTimeSeriesVertex(), "4").addLayer("6", New GlobalPoolingLayer(PoolingType.AVG), "5").addVertex("7", New LastTimeStepVertex("in3"), "in3").addVertex("8", New MergeVertex(), "6", "7").addVertex("9", New PreprocessorVertex(New ComposableInputPreProcessor()), "8").addLayer("out", (New OutputLayer.Builder()).nOut(10).build(), "9").setInputTypes(InputType.feedForward(8), InputType.feedForward(8), InputType.recurrent(8)).setOutputs("out")
								[in] = New INDArray(){Nd4j.rand(networkDtype, 2, 8), Nd4j.rand(networkDtype, 2, 8), Nd4j.rand(networkDtype, 2, 8, 5)}
							Case 4
								b.addInputs("in1", "in2").addLayer("1", (New LSTM.Builder()).nOut(8).build(), "in1").addVertex("preproc1", New PreprocessorVertex(New RnnToCnnPreProcessor(2, 2, 2)), "1").addVertex("preproc2", New PreprocessorVertex(New CnnToRnnPreProcessor(2, 2, 2)), "preproc1").addLayer("pool", New GlobalPoolingLayer(), "preproc2").addLayer("pool2", New GlobalPoolingLayer(), "in2").addLayer("out", (New OutputLayer.Builder()).nOut(10).build(), "pool", "pool2").setInputTypes(InputType.recurrent(8), InputType.convolutional(28, 28, 1)).setOutputs("out")
								[in] = New INDArray(){Nd4j.rand(networkDtype, 2, 8, 5), Nd4j.rand(networkDtype, 2, 1, 28, 28)}
							Case 5
								b.addInputs("in1", "in2").addVertex("fv", New FrozenVertex(New ScaleVertex(2.0)), "in1").addLayer("1", (New DenseLayer.Builder()).nOut(5).build(), "fv").addLayer("2", (New DenseLayer.Builder()).nOut(5).build(), "in2").addVertex("v", New L2Vertex(), "1", "2").addLayer("out", (New OutputLayer.Builder()).nOut(10).build(), "v").setInputTypes(InputType.feedForward(5), InputType.feedForward(5)).setOutputs("out")
								[in] = New INDArray(){Nd4j.rand(networkDtype, 2, 5), Nd4j.rand(networkDtype, 2, 5)}
							Case 6
								b.addInputs("in").addLayer("1", (New LSTM.Builder()).nOut(5).build(), "in").addVertex("2", New PreprocessorVertex(New KerasFlattenRnnPreprocessor(5, 4)), "1").addLayer("out", (New OutputLayer.Builder()).nOut(10).build(), "2").setOutputs("out").setInputTypes(InputType.recurrent(5, 4))
								[in] = New INDArray(){Nd4j.rand(networkDtype, 2, 5, 4)}
							Case 7
								b.addInputs("in").addLayer("1", (New ConvolutionLayer.Builder()).kernelSize(2, 2).nOut(5).convolutionMode(ConvolutionMode.Same).build(), "in").addVertex("2", New PreprocessorVertex(New CnnToFeedForwardPreProcessor(28, 28, 5)), "1").addLayer("out", (New OutputLayer.Builder()).nOut(10).build(), "2").setOutputs("out").setInputTypes(InputType.convolutional(28, 28, 1))
								[in] = New INDArray(){Nd4j.rand(networkDtype, 2, 1, 28, 28)}
						End Select

						Dim net As New ComputationGraph(b.build())
						net.init()

						Dim label As INDArray = TestUtils.randomOneHot(2, 10).castTo(networkDtype)

						Dim [out] As INDArray = net.outputSingle([in])
						assertEquals(networkDtype, [out].dataType(), msg)
						Dim ff As IDictionary(Of String, INDArray) = net.feedForward([in], False)
						For Each e As KeyValuePair(Of String, INDArray) In ff.SetOfKeyValuePairs()
							If e.Key.Equals("in") Then
								Continue For
							End If
							Dim s As String = msg & " - layer: " & e.Key
							assertEquals(networkDtype, e.Value.dataType(), s)
						Next e

						net.Inputs = [in]
						net.Labels = label
						net.computeGradientAndScore()

						net.fit(New MultiDataSet([in], New INDArray(){label}))

						logUsedClasses(net)

						'Now, test mismatched dtypes for input/labels:
						For Each inputLabelDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
							Dim in2([in].Length - 1) As INDArray
							For i As Integer = 0 To [in].Length - 1
								in2(i) = [in](i).castTo(inputLabelDtype)
							Next i
							Dim label2 As INDArray = label.castTo(inputLabelDtype)
							net.output(in2)
							net.Inputs = in2
							net.Labels = label2
							net.computeGradientAndScore()

							net.fit(New MultiDataSet(in2, New INDArray(){label2}))
						Next inputLabelDtype
					Next test
				Next networkDtype
			Next globalDtype
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLocallyConnected()
		Public Overridable Sub testLocallyConnected()
			For Each globalDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				Nd4j.setDefaultDataTypes(globalDtype, globalDtype)
				For Each networkDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
					assertEquals(globalDtype, Nd4j.dataType())
					assertEquals(globalDtype, Nd4j.defaultFloatingPointType())

					Dim [in]() As INDArray = Nothing
					For test As Integer = 0 To 1
						Dim msg As String = "Global dtype: " & globalDtype & ", network dtype: " & networkDtype & ", test=" & test

						Dim b As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).dataType(networkDtype).seed(123).updater(New NoOp()).weightInit(WeightInit.XAVIER).convolutionMode(ConvolutionMode.Same).graphBuilder()

						Dim label As INDArray
						Select Case test
							Case 0
								b.addInputs("in").addLayer("1", (New LSTM.Builder()).nOut(5).build(), "in").addLayer("2", (New LocallyConnected1D.Builder()).kernelSize(2).nOut(4).build(), "1").addLayer("out", (New RnnOutputLayer.Builder()).nOut(10).build(), "2").setOutputs("out").setInputTypes(InputType.recurrent(5, 2))
								[in] = New INDArray(){Nd4j.rand(networkDtype, 2, 5, 2)}
								label = TestUtils.randomOneHotTimeSeries(2, 10, 2)
							Case 1
								b.addInputs("in").addLayer("1", (New ConvolutionLayer.Builder()).kernelSize(2, 2).nOut(5).convolutionMode(ConvolutionMode.Same).build(), "in").addLayer("2", (New LocallyConnected2D.Builder()).kernelSize(2, 2).nOut(5).build(), "1").addLayer("out", (New OutputLayer.Builder()).nOut(10).build(), "2").setOutputs("out").setInputTypes(InputType.convolutional(8, 8, 1))
								[in] = New INDArray(){Nd4j.rand(networkDtype, 2, 1, 8, 8)}
								label = TestUtils.randomOneHot(2, 10).castTo(networkDtype)
							Case Else
								Throw New Exception()
						End Select

						Dim net As New ComputationGraph(b.build())
						net.init()

						Dim [out] As INDArray = net.outputSingle([in])
						assertEquals(networkDtype, [out].dataType(), msg)
						Dim ff As IDictionary(Of String, INDArray) = net.feedForward([in], False)
						For Each e As KeyValuePair(Of String, INDArray) In ff.SetOfKeyValuePairs()
							If e.Key.Equals("in") Then
								Continue For
							End If
							Dim s As String = msg & " - layer: " & e.Key
							assertEquals(networkDtype, e.Value.dataType(), s)
						Next e

						net.Inputs = [in]
						net.Labels = label
						net.computeGradientAndScore()

						net.fit(New MultiDataSet([in], New INDArray(){label}))

						logUsedClasses(net)

						'Now, test mismatched dtypes for input/labels:
						For Each inputLabelDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
							Dim in2([in].Length - 1) As INDArray
							For i As Integer = 0 To [in].Length - 1
								in2(i) = [in](i).castTo(inputLabelDtype)
							Next i
							Dim label2 As INDArray = label.castTo(inputLabelDtype)
							net.output(in2)
							net.Inputs = in2
							net.Labels = label2
							net.computeGradientAndScore()

							net.fit(New MultiDataSet(in2, New INDArray(){label2}))
						Next inputLabelDtype
					Next test
				Next networkDtype
			Next globalDtype
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAttentionDTypes()
		Public Overridable Sub testAttentionDTypes()
			For Each globalDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				Nd4j.setDefaultDataTypes(globalDtype, globalDtype)
				For Each networkDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
					assertEquals(globalDtype, Nd4j.dataType())
					assertEquals(globalDtype, Nd4j.defaultFloatingPointType())

					Dim msg As String = "Global dtype: " & globalDtype & ", network dtype: " & networkDtype

					Dim mb As Integer = 3
					Dim nIn As Integer = 3
					Dim nOut As Integer = 5
					Dim tsLength As Integer = 4
					Dim layerSize As Integer = 8
					Dim numQueries As Integer = 6

					Dim [in] As INDArray = Nd4j.rand(networkDtype, New Long(){mb, nIn, tsLength})
					Dim labels As INDArray = TestUtils.randomOneHot(mb, nOut)

					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(networkDtype).activation(Activation.TANH).updater(New NoOp()).weightInit(WeightInit.XAVIER).list().layer((New LSTM.Builder()).nOut(layerSize).build()).layer((New SelfAttentionLayer.Builder()).nOut(8).nHeads(2).projectInput(True).build()).layer((New LearnedSelfAttentionLayer.Builder()).nOut(8).nHeads(2).nQueries(numQueries).projectInput(True).build()).layer((New RecurrentAttentionLayer.Builder()).nIn(layerSize).nOut(layerSize).nHeads(1).projectInput(False).hasBias(False).build()).layer((New GlobalPoolingLayer.Builder()).poolingType(PoolingType.MAX).build()).layer((New OutputLayer.Builder()).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).setInputType(InputType.recurrent(nIn)).build()

					Dim net As New MultiLayerNetwork(conf)
					net.init()

					Dim [out] As INDArray = net.output([in])
					assertEquals(networkDtype, [out].dataType(), msg)
					Dim ff As IList(Of INDArray) = net.feedForward([in])
					For i As Integer = 0 To ff.Count - 1
						Dim s As String = msg & " - layer " & (i - 1) & " - " & (If(i = 0, "input", net.getLayer(i - 1).conf().getLayer().GetType().Name))
						assertEquals(networkDtype, ff(i).dataType(), s)
					Next i

					net.Input = [in]
					net.Labels = labels
					net.computeGradientAndScore()

					net.fit(New DataSet([in], labels))

					logUsedClasses(net)

					'Now, test mismatched dtypes for input/labels:
					For Each inputLabelDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
						Dim in2 As INDArray = [in].castTo(inputLabelDtype)
						Dim label2 As INDArray = labels.castTo(inputLabelDtype)
						net.output(in2)
						net.Input = in2
						net.Labels = label2
						net.computeGradientAndScore()

						net.fit(New DataSet(in2, label2))
					Next inputLabelDtype
				Next networkDtype
			Next globalDtype
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAttentionDTypes2()
		Public Overridable Sub testAttentionDTypes2()
			Dim nIn As Integer = 3
			Dim nOut As Integer = 5
			Dim tsLength As Integer = 4
			Dim layerSize As Integer = 8
			Dim mb As Integer = 3

			For Each globalDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				Nd4j.setDefaultDataTypes(globalDtype, globalDtype)
				For Each networkDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}

					assertEquals(globalDtype, Nd4j.dataType())
					assertEquals(globalDtype, Nd4j.defaultFloatingPointType())

					Dim msg As String = "Global dtype: " & globalDtype & ", network dtype: " & networkDtype

					Dim [in] As INDArray = Nd4j.rand(networkDtype, New Long(){mb, nIn, tsLength})
					Dim labels As INDArray = TestUtils.randomOneHot(mb, nOut).castTo(networkDtype)
					Dim maskType As String = "inputMask"

					Dim inMask As INDArray = Nd4j.ones(networkDtype, mb, tsLength)
					For i As Integer = 0 To mb - 1
						Dim firstMaskedStep As Integer = tsLength - 1 - i
						If firstMaskedStep = 0 Then
							firstMaskedStep = tsLength
						End If
						For j As Integer = firstMaskedStep To tsLength - 1
							inMask.putScalar(i, j, 0.0)
						Next j
					Next i

					Dim name As String = "testAttentionVertex() - mb=" & mb & ", tsLength = " & tsLength & ", maskType=" & maskType
					Console.WriteLine("Starting test: " & name)


					Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).dataType(networkDtype).activation(Activation.TANH).updater(New NoOp()).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("input").addLayer("lstmKeys", (New LSTM.Builder()).nOut(layerSize).build(), "input").addLayer("lstmQueries", (New LSTM.Builder()).nOut(layerSize).build(), "input").addLayer("lstmValues", (New LSTM.Builder()).nOut(layerSize).build(), "input").addVertex("attention", (New AttentionVertex.Builder()).nOut(8).nHeads(2).projectInput(True).nInQueries(layerSize).nInKeys(layerSize).nInValues(layerSize).build(), "lstmQueries", "lstmKeys", "lstmValues").addLayer("pooling", (New GlobalPoolingLayer.Builder()).poolingType(PoolingType.MAX).build(), "attention").addLayer("output", (New OutputLayer.Builder()).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "pooling").setOutputs("output").setInputTypes(InputType.recurrent(nIn)).build()
					Dim net As New ComputationGraph(conf)
					net.init()

					Dim [out] As INDArray = net.outputSingle([in])
					assertEquals(networkDtype, [out].dataType(), msg)
					Dim ff As IDictionary(Of String, INDArray) = net.feedForward([in], False)
					For Each e As KeyValuePair(Of String, INDArray) In ff.SetOfKeyValuePairs()
						Dim s As String = msg & " - layer " & e.Key
						assertEquals(networkDtype, e.Value.dataType(), s)
					Next e

					net.setInput(0, [in])
					net.Labels = labels
					net.computeGradientAndScore()

					net.fit(New DataSet([in], labels))

					logUsedClasses(net)

					'Now, test mismatched dtypes for input/labels:
					For Each inputLabelDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
						Dim in2 As INDArray = [in].castTo(inputLabelDtype)
						Dim label2 As INDArray = labels.castTo(inputLabelDtype)
						net.output(in2)
						net.setInput(0, in2)
						net.Labels = label2
						net.computeGradientAndScore()

						net.fit(New DataSet(in2, label2))
					Next inputLabelDtype
				Next networkDtype
			Next globalDtype
		End Sub
	End Class

End Namespace