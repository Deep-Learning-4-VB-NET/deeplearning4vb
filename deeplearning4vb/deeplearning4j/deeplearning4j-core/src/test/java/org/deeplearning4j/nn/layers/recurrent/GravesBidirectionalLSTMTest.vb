Imports System.Collections.Generic
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports CacheMode = org.deeplearning4j.nn.conf.CacheMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports UniformDistribution = org.deeplearning4j.nn.conf.distribution.UniformDistribution
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports GravesBidirectionalLSTMParamInitializer = org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer
Imports GravesLSTMParamInitializer = org.deeplearning4j.nn.params.GravesLSTMParamInitializer
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports Arguments = org.junit.jupiter.params.provider.Arguments
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports org.nd4j.common.primitives
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Activation = org.nd4j.linalg.activations.Activation
Imports ActivationSigmoid = org.nd4j.linalg.activations.impl.ActivationSigmoid
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports AdaGrad = org.nd4j.linalg.learning.config.AdaGrad
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
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
Namespace org.deeplearning4j.nn.layers.recurrent


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Graves Bidirectional LSTM Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class GravesBidirectionalLSTMTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class GravesBidirectionalLSTMTest
		Inherits BaseDL4JTest

		Private score As Double = 0.0



		Public Shared Function params() As Stream(Of Arguments)
			Dim args As IList(Of Arguments) = New List(Of Arguments)()
			For Each nd4jBackend As Nd4jBackend In BaseNd4jTestWithBackends.BACKENDS
				For Each rnnFormat As RNNFormat In System.Enum.GetValues(GetType(RNNFormat))
					args.Add(Arguments.of(rnnFormat,nd4jBackend))
				Next rnnFormat
			Next nd4jBackend
			Return args.stream()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Bidirectional LSTM Graves Forward Basic") @MethodSource("org.deeplearning4j.nn.layers.recurrent.GravesBidirectionalLSTMTest#params") @ParameterizedTest void testBidirectionalLSTMGravesForwardBasic(org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testBidirectionalLSTMGravesForwardBasic(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)
			' Very basic test of forward prop. of LSTM layer with a time series.
			' Essentially make sure it doesn't throw any exceptions, and provides output in the correct shape.
			Dim nIn As Integer = 13
			Dim nHiddenUnits As Integer = 17
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.conf.NeuralNetConfiguration conf = new org.deeplearning4j.nn.conf.NeuralNetConfiguration.Builder().layer(new org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM.Builder().nIn(nIn).nOut(nHiddenUnits).dataFormat(rnnDataFormat).activation(org.nd4j.linalg.activations.Activation.TANH).build()).build();
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM.Builder()).nIn(nIn).nOut(nHiddenUnits).dataFormat(rnnDataFormat).activation(Activation.TANH).build()).build()
			Dim numParams As val = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final GravesBidirectionalLSTM layer = (GravesBidirectionalLSTM) conf.getLayer().instantiate(conf, null, 0, params, true, params.dataType());
			Dim layer As GravesBidirectionalLSTM = CType(conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType()), GravesBidirectionalLSTM)
			' Data: has shape [miniBatchSize,nIn,timeSeriesLength];
			' Output/activations has shape [miniBatchsize,nHiddenUnits,timeSeriesLength];
			If rnnDataFormat = RNNFormat.NCW Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray dataSingleExampleTimeLength1 = org.nd4j.linalg.factory.Nd4j.ones(1, nIn, 1);
				Dim dataSingleExampleTimeLength1 As INDArray = Nd4j.ones(1, nIn, 1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray activations1 = layer.activate(dataSingleExampleTimeLength1, false, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr.noWorkspaces());
				Dim activations1 As INDArray = layer.activate(dataSingleExampleTimeLength1, False, LayerWorkspaceMgr.noWorkspaces())
				assertArrayEquals(activations1.shape(), New Long() { 1, nHiddenUnits, 1 })
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray dataMultiExampleLength1 = org.nd4j.linalg.factory.Nd4j.ones(10, nIn, 1);
				Dim dataMultiExampleLength1 As INDArray = Nd4j.ones(10, nIn, 1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray activations2 = layer.activate(dataMultiExampleLength1, false, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr.noWorkspaces());
				Dim activations2 As INDArray = layer.activate(dataMultiExampleLength1, False, LayerWorkspaceMgr.noWorkspaces())
				assertArrayEquals(activations2.shape(), New Long() { 10, nHiddenUnits, 1 })
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray dataSingleExampleLength12 = org.nd4j.linalg.factory.Nd4j.ones(1, nIn, 12);
				Dim dataSingleExampleLength12 As INDArray = Nd4j.ones(1, nIn, 12)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray activations3 = layer.activate(dataSingleExampleLength12, false, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr.noWorkspaces());
				Dim activations3 As INDArray = layer.activate(dataSingleExampleLength12, False, LayerWorkspaceMgr.noWorkspaces())
				assertArrayEquals(activations3.shape(), New Long() { 1, nHiddenUnits, 12 })
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray dataMultiExampleLength15 = org.nd4j.linalg.factory.Nd4j.ones(10, nIn, 15);
				Dim dataMultiExampleLength15 As INDArray = Nd4j.ones(10, nIn, 15)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray activations4 = layer.activate(dataMultiExampleLength15, false, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr.noWorkspaces());
				Dim activations4 As INDArray = layer.activate(dataMultiExampleLength15, False, LayerWorkspaceMgr.noWorkspaces())
				assertArrayEquals(activations4.shape(), New Long() { 10, nHiddenUnits, 15 })
			Else
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray dataSingleExampleTimeLength1 = org.nd4j.linalg.factory.Nd4j.ones(1, 1, nIn);
				Dim dataSingleExampleTimeLength1 As INDArray = Nd4j.ones(1, 1, nIn)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray activations1 = layer.activate(dataSingleExampleTimeLength1, false, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr.noWorkspaces());
				Dim activations1 As INDArray = layer.activate(dataSingleExampleTimeLength1, False, LayerWorkspaceMgr.noWorkspaces())
				assertArrayEquals(activations1.shape(), New Long() { 1, 1, nHiddenUnits })
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray dataMultiExampleLength1 = org.nd4j.linalg.factory.Nd4j.ones(10, 1, nIn);
				Dim dataMultiExampleLength1 As INDArray = Nd4j.ones(10, 1, nIn)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray activations2 = layer.activate(dataMultiExampleLength1, false, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr.noWorkspaces());
				Dim activations2 As INDArray = layer.activate(dataMultiExampleLength1, False, LayerWorkspaceMgr.noWorkspaces())
				assertArrayEquals(activations2.shape(), New Long() { 10, 1, nHiddenUnits })
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray dataSingleExampleLength12 = org.nd4j.linalg.factory.Nd4j.ones(1, 12, nIn);
				Dim dataSingleExampleLength12 As INDArray = Nd4j.ones(1, 12, nIn)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray activations3 = layer.activate(dataSingleExampleLength12, false, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr.noWorkspaces());
				Dim activations3 As INDArray = layer.activate(dataSingleExampleLength12, False, LayerWorkspaceMgr.noWorkspaces())
				assertArrayEquals(activations3.shape(), New Long() { 1, 12, nHiddenUnits })
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray dataMultiExampleLength15 = org.nd4j.linalg.factory.Nd4j.ones(10, 15, nIn);
				Dim dataMultiExampleLength15 As INDArray = Nd4j.ones(10, 15, nIn)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray activations4 = layer.activate(dataMultiExampleLength15, false, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr.noWorkspaces());
				Dim activations4 As INDArray = layer.activate(dataMultiExampleLength15, False, LayerWorkspaceMgr.noWorkspaces())
				assertArrayEquals(activations4.shape(), New Long() { 10, 15, nHiddenUnits })
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Bidirectional LSTM Graves Backward Basic") @MethodSource("org.deeplearning4j.nn.layers.recurrent.GravesBidirectionalLSTMTest#params") @ParameterizedTest void testBidirectionalLSTMGravesBackwardBasic(org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testBidirectionalLSTMGravesBackwardBasic(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)
			' Very basic test of backprop for mini-batch + time series
			' Essentially make sure it doesn't throw any exceptions, and provides output in the correct shape.
			testGravesBackwardBasicHelper(rnnDataFormat,13, 3, 17, 10, 7)
			' Edge case: miniBatchSize = 1
			testGravesBackwardBasicHelper(rnnDataFormat,13, 3, 17, 1, 7)
			' Edge case: timeSeriesLength = 1
			testGravesBackwardBasicHelper(rnnDataFormat,13, 3, 17, 10, 1)
			' Edge case: both miniBatchSize = 1 and timeSeriesLength = 1
			testGravesBackwardBasicHelper(rnnDataFormat,13, 3, 17, 1, 1)
		End Sub

		Private Sub testGravesBackwardBasicHelper(ByVal rnnDataFormat As RNNFormat, ByVal nIn As Integer, ByVal nOut As Integer, ByVal lstmNHiddenUnits As Integer, ByVal miniBatchSize As Integer, ByVal timeSeriesLength As Integer)
			Dim inputData As INDArray = If(rnnDataFormat = RNNFormat.NCW, Nd4j.ones(miniBatchSize, nIn, timeSeriesLength), Nd4j.ones(miniBatchSize, timeSeriesLength, nIn))
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM.Builder()).nIn(nIn).nOut(lstmNHiddenUnits).dataFormat(rnnDataFormat).dist(New UniformDistribution(0, 1)).activation(Activation.TANH).build()).build()
			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim lstm As GravesBidirectionalLSTM = CType(conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType()), GravesBidirectionalLSTM)
			lstm.BackpropGradientsViewArray = Nd4j.create(1, conf.getLayer().initializer().numParams(conf))
			' Set input, do a forward pass:
			lstm.activate(inputData, False, LayerWorkspaceMgr.noWorkspaces())
			assertNotNull(lstm.input())
			Dim epsilon As INDArray = If(rnnDataFormat = RNNFormat.NCW, Nd4j.ones(miniBatchSize, lstmNHiddenUnits, timeSeriesLength), Nd4j.ones(miniBatchSize, timeSeriesLength, lstmNHiddenUnits))
			Dim [out] As Pair(Of Gradient, INDArray) = lstm.backpropGradient(epsilon, LayerWorkspaceMgr.noWorkspaces())
			Dim outGradient As Gradient = [out].First
			Dim nextEpsilon As INDArray = [out].Second
			Dim biasGradientF As INDArray = outGradient.getGradientFor(GravesBidirectionalLSTMParamInitializer.BIAS_KEY_FORWARDS)
			Dim inWeightGradientF As INDArray = outGradient.getGradientFor(GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS)
			Dim recurrentWeightGradientF As INDArray = outGradient.getGradientFor(GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_FORWARDS)
			assertNotNull(biasGradientF)
			assertNotNull(inWeightGradientF)
			assertNotNull(recurrentWeightGradientF)
			Dim biasGradientB As INDArray = outGradient.getGradientFor(GravesBidirectionalLSTMParamInitializer.BIAS_KEY_BACKWARDS)
			Dim inWeightGradientB As INDArray = outGradient.getGradientFor(GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_BACKWARDS)
			Dim recurrentWeightGradientB As INDArray = outGradient.getGradientFor(GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_BACKWARDS)
			assertNotNull(biasGradientB)
			assertNotNull(inWeightGradientB)
			assertNotNull(recurrentWeightGradientB)
			assertArrayEquals(biasGradientF.shape(), New Long() { 1, 4 * lstmNHiddenUnits })
			assertArrayEquals(inWeightGradientF.shape(), New Long() { nIn, 4 * lstmNHiddenUnits })
			assertArrayEquals(recurrentWeightGradientF.shape(), New Long() { lstmNHiddenUnits, 4 * lstmNHiddenUnits + 3 })
			assertArrayEquals(biasGradientB.shape(), New Long() { 1, 4 * lstmNHiddenUnits })
			assertArrayEquals(inWeightGradientB.shape(), New Long() { nIn, 4 * lstmNHiddenUnits })
			assertArrayEquals(recurrentWeightGradientB.shape(), New Long() { lstmNHiddenUnits, 4 * lstmNHiddenUnits + 3 })
			assertNotNull(nextEpsilon)
			If rnnDataFormat = RNNFormat.NCW Then
				assertArrayEquals(nextEpsilon.shape(), New Long() { miniBatchSize, nIn, timeSeriesLength })
			Else
				assertArrayEquals(nextEpsilon.shape(), New Long() { miniBatchSize, timeSeriesLength, nIn })
			End If
			' Check update:
			For Each s As String In outGradient.gradientForVariable().Keys
				lstm.update(outGradient.getGradientFor(s), s)
			Next s
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Graves Bidirectional LSTM Forward Pass Helper") @ParameterizedTest @MethodSource("org.deeplearning4j.nn.layers.recurrent.GravesBidirectionalLSTMTest#params") void testGravesBidirectionalLSTMForwardPassHelper(org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testGravesBidirectionalLSTMForwardPassHelper(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)
			' GravesBidirectionalLSTM.activateHelper() has different behaviour (due to optimizations) when forBackprop==true vs false
			' But should otherwise provide identical activations
			Nd4j.Random.setSeed(12345)
			Const nIn As Integer = 10
			Const layerSize As Integer = 15
			Const miniBatchSize As Integer = 4
			Const timeSeriesLength As Integer = 7
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.conf.NeuralNetConfiguration conf = new org.deeplearning4j.nn.conf.NeuralNetConfiguration.Builder().layer(new org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM.Builder().nIn(nIn).nOut(layerSize).dist(new org.deeplearning4j.nn.conf.distribution.UniformDistribution(0, 1)).activation(org.nd4j.linalg.activations.Activation.TANH).build()).build();
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM.Builder()).nIn(nIn).nOut(layerSize).dist(New UniformDistribution(0, 1)).activation(Activation.TANH).build()).build()
			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final GravesBidirectionalLSTM lstm = (GravesBidirectionalLSTM) conf.getLayer().instantiate(conf, null, 0, params, true, params.dataType());
			Dim lstm As GravesBidirectionalLSTM = CType(conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType()), GravesBidirectionalLSTM)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray input = org.nd4j.linalg.factory.Nd4j.rand(new int[] { miniBatchSize, nIn, timeSeriesLength });
			Dim input As INDArray = Nd4j.rand(New Integer() { miniBatchSize, nIn, timeSeriesLength })
			lstm.setInput(input, LayerWorkspaceMgr.noWorkspaces())
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray fwdPassFalse = LSTMHelpers.activateHelper(lstm, lstm.conf(), new org.nd4j.linalg.activations.impl.ActivationSigmoid(), lstm.input(), lstm.getParam(org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_FORWARDS), lstm.getParam(org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS), lstm.getParam(org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.BIAS_KEY_FORWARDS), false, null, null, false, true, org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS, null, true, null, org.deeplearning4j.nn.conf.CacheMode.NONE, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr.noWorkspaces(), true).fwdPassOutput;
			Dim fwdPassFalse As INDArray = LSTMHelpers.activateHelper(lstm, lstm.conf(), New ActivationSigmoid(), lstm.input(), lstm.getParam(GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_FORWARDS), lstm.getParam(GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS), lstm.getParam(GravesBidirectionalLSTMParamInitializer.BIAS_KEY_FORWARDS), False, Nothing, Nothing, False, True, GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS, Nothing, True, Nothing, CacheMode.NONE, LayerWorkspaceMgr.noWorkspaces(), True).fwdPassOutput
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray[] fwdPassTrue = LSTMHelpers.activateHelper(lstm, lstm.conf(), new org.nd4j.linalg.activations.impl.ActivationSigmoid(), lstm.input(), lstm.getParam(org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_FORWARDS), lstm.getParam(org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS), lstm.getParam(org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.BIAS_KEY_FORWARDS), false, null, null, true, true, org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS, null, true, null, org.deeplearning4j.nn.conf.CacheMode.NONE, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr.noWorkspaces(), true).fwdPassOutputAsArrays;
			Dim fwdPassTrue() As INDArray = LSTMHelpers.activateHelper(lstm, lstm.conf(), New ActivationSigmoid(), lstm.input(), lstm.getParam(GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_FORWARDS), lstm.getParam(GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS), lstm.getParam(GravesBidirectionalLSTMParamInitializer.BIAS_KEY_FORWARDS), False, Nothing, Nothing, True, True, GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS, Nothing, True, Nothing, CacheMode.NONE, LayerWorkspaceMgr.noWorkspaces(), True).fwdPassOutputAsArrays
			' I have no idea what the heck this does --Ben
			For i As Integer = 0 To timeSeriesLength - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray sliceFalse = fwdPassFalse.tensorAlongDimension(i, 1, 0);
				Dim sliceFalse As INDArray = fwdPassFalse.tensorAlongDimension(i, 1, 0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray sliceTrue = fwdPassTrue[i];
				Dim sliceTrue As INDArray = fwdPassTrue(i)
				assertTrue(sliceFalse.Equals(sliceTrue))
			Next i
		End Sub

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: private static void reverseColumnsInPlace(final org.nd4j.linalg.api.ndarray.INDArray x)
		Private Shared Sub reverseColumnsInPlace(ByVal x As INDArray)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long N = x.size(1);
			Dim N As Long = x.size(1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray x2 = x.dup();
			Dim x2 As INDArray = x.dup()
			For t As Integer = 0 To N - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long b = N - t - 1;
				Dim b As Long = N - t - 1
				' clone?
				x.putColumn(t, x2.getColumn(b))
			Next t
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Get Set Params") @MethodSource("org.deeplearning4j.nn.layers.recurrent.GravesBidirectionalLSTMTest#params") @ParameterizedTest void testGetSetParmas(org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testGetSetParmas(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)
			Const nIn As Integer = 2
			Const layerSize As Integer = 3
			Const miniBatchSize As Integer = 2
			Const timeSeriesLength As Integer = 10
			Nd4j.Random.setSeed(12345)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.conf.NeuralNetConfiguration confBidirectional = new org.deeplearning4j.nn.conf.NeuralNetConfiguration.Builder().layer(new org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM.Builder().nIn(nIn).nOut(layerSize).dataFormat(rnnDataFormat).dist(new org.deeplearning4j.nn.conf.distribution.UniformDistribution(-0.1, 0.1)).activation(org.nd4j.linalg.activations.Activation.TANH).build()).build();
			Dim confBidirectional As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM.Builder()).nIn(nIn).nOut(layerSize).dataFormat(rnnDataFormat).dist(New UniformDistribution(-0.1, 0.1)).activation(Activation.TANH).build()).build()
			Dim numParams As Long = confBidirectional.getLayer().initializer().numParams(confBidirectional)
			Dim params As INDArray = Nd4j.create(1, numParams)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final GravesBidirectionalLSTM bidirectionalLSTM = (GravesBidirectionalLSTM) confBidirectional.getLayer().instantiate(confBidirectional, null, 0, params, true, params.dataType());
			Dim bidirectionalLSTM As GravesBidirectionalLSTM = CType(confBidirectional.getLayer().instantiate(confBidirectional, Nothing, 0, params, True, params.dataType()), GravesBidirectionalLSTM)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray sig = (rnnDataFormat == org.deeplearning4j.nn.conf.RNNFormat.NCW) ? org.nd4j.linalg.factory.Nd4j.rand(new int[] { miniBatchSize, nIn, timeSeriesLength }) : org.nd4j.linalg.factory.Nd4j.rand(new int[] { miniBatchSize, timeSeriesLength, nIn });
			Dim sig As INDArray = If(rnnDataFormat = RNNFormat.NCW, Nd4j.rand(New Integer() { miniBatchSize, nIn, timeSeriesLength }), Nd4j.rand(New Integer() { miniBatchSize, timeSeriesLength, nIn }))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray act1 = bidirectionalLSTM.activate(sig, false, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr.noWorkspaces());
			Dim act1 As INDArray = bidirectionalLSTM.activate(sig, False, LayerWorkspaceMgr.noWorkspaces())
			params = bidirectionalLSTM.params()
			bidirectionalLSTM.Params = params
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray act2 = bidirectionalLSTM.activate(sig, false, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr.noWorkspaces());
			Dim act2 As INDArray = bidirectionalLSTM.activate(sig, False, LayerWorkspaceMgr.noWorkspaces())
			assertArrayEquals(act2.data().asDouble(), act1.data().asDouble(), 1e-8)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Simple Forwards And Backwards Activation") @MethodSource("org.deeplearning4j.nn.layers.recurrent.GravesBidirectionalLSTMTest#params") @ParameterizedTest void testSimpleForwardsAndBackwardsActivation(org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testSimpleForwardsAndBackwardsActivation(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)
			Const nIn As Integer = 2
			Const layerSize As Integer = 3
			Const miniBatchSize As Integer = 1
			Const timeSeriesLength As Integer = 5
			Nd4j.Random.setSeed(12345)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.conf.NeuralNetConfiguration confBidirectional = new org.deeplearning4j.nn.conf.NeuralNetConfiguration.Builder().layer(new org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM.Builder().nIn(nIn).nOut(layerSize).dataFormat(rnnDataFormat).dist(new org.deeplearning4j.nn.conf.distribution.UniformDistribution(-0.1, 0.1)).activation(org.nd4j.linalg.activations.Activation.TANH).updater(new org.nd4j.linalg.learning.config.NoOp()).build()).build();
			Dim confBidirectional As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM.Builder()).nIn(nIn).nOut(layerSize).dataFormat(rnnDataFormat).dist(New UniformDistribution(-0.1, 0.1)).activation(Activation.TANH).updater(New NoOp()).build()).build()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.conf.NeuralNetConfiguration confForwards = new org.deeplearning4j.nn.conf.NeuralNetConfiguration.Builder().layer(new org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder().nIn(nIn).nOut(layerSize).dataFormat(rnnDataFormat).weightInit(org.deeplearning4j.nn.weights.WeightInit.ZERO).activation(org.nd4j.linalg.activations.Activation.TANH).build()).build();
			Dim confForwards As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(nIn).nOut(layerSize).dataFormat(rnnDataFormat).weightInit(WeightInit.ZERO).activation(Activation.TANH).build()).build()
			Dim numParams As Long = confForwards.getLayer().initializer().numParams(confForwards)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim numParamsBD As Long = confBidirectional.getLayer().initializer().numParams(confBidirectional)
			Dim paramsBD As INDArray = Nd4j.create(1, numParamsBD)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final GravesBidirectionalLSTM bidirectionalLSTM = (GravesBidirectionalLSTM) confBidirectional.getLayer().instantiate(confBidirectional, null, 0, paramsBD, true, params.dataType());
			Dim bidirectionalLSTM As GravesBidirectionalLSTM = CType(confBidirectional.getLayer().instantiate(confBidirectional, Nothing, 0, paramsBD, True, params.dataType()), GravesBidirectionalLSTM)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final GravesLSTM forwardsLSTM = (GravesLSTM) confForwards.getLayer().instantiate(confForwards, null, 0, params, true, params.dataType());
			Dim forwardsLSTM As GravesLSTM = CType(confForwards.getLayer().instantiate(confForwards, Nothing, 0, params, True, params.dataType()), GravesLSTM)
			bidirectionalLSTM.BackpropGradientsViewArray = Nd4j.create(1, confBidirectional.getLayer().initializer().numParams(confBidirectional))
			forwardsLSTM.BackpropGradientsViewArray = Nd4j.create(1, confForwards.getLayer().initializer().numParams(confForwards))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray sig = (rnnDataFormat == org.deeplearning4j.nn.conf.RNNFormat.NCW) ? org.nd4j.linalg.factory.Nd4j.rand(new int[] { miniBatchSize, nIn, timeSeriesLength }) : org.nd4j.linalg.factory.Nd4j.rand(new int[] { miniBatchSize, timeSeriesLength, nIn });
			Dim sig As INDArray = If(rnnDataFormat = RNNFormat.NCW, Nd4j.rand(New Integer() { miniBatchSize, nIn, timeSeriesLength }), Nd4j.rand(New Integer() { miniBatchSize, timeSeriesLength, nIn }))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray sigb = sig.dup();
			Dim sigb As INDArray = sig.dup()
			If rnnDataFormat = RNNFormat.NCW Then
				reverseColumnsInPlace(sigb.slice(0))
			Else
				reverseColumnsInPlace(sigb.slice(0).permute(1, 0))
			End If
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray recurrentWeightsF = bidirectionalLSTM.getParam(org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_FORWARDS);
			Dim recurrentWeightsF As INDArray = bidirectionalLSTM.getParam(GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_FORWARDS)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray inputWeightsF = bidirectionalLSTM.getParam(org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS);
			Dim inputWeightsF As INDArray = bidirectionalLSTM.getParam(GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray biasWeightsF = bidirectionalLSTM.getParam(org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.BIAS_KEY_FORWARDS);
			Dim biasWeightsF As INDArray = bidirectionalLSTM.getParam(GravesBidirectionalLSTMParamInitializer.BIAS_KEY_FORWARDS)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray recurrentWeightsF2 = forwardsLSTM.getParam(org.deeplearning4j.nn.params.GravesLSTMParamInitializer.RECURRENT_WEIGHT_KEY);
			Dim recurrentWeightsF2 As INDArray = forwardsLSTM.getParam(GravesLSTMParamInitializer.RECURRENT_WEIGHT_KEY)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray inputWeightsF2 = forwardsLSTM.getParam(org.deeplearning4j.nn.params.GravesLSTMParamInitializer.INPUT_WEIGHT_KEY);
			Dim inputWeightsF2 As INDArray = forwardsLSTM.getParam(GravesLSTMParamInitializer.INPUT_WEIGHT_KEY)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray biasWeightsF2 = forwardsLSTM.getParam(org.deeplearning4j.nn.params.GravesLSTMParamInitializer.BIAS_KEY);
			Dim biasWeightsF2 As INDArray = forwardsLSTM.getParam(GravesLSTMParamInitializer.BIAS_KEY)
			' assert that the forwards part of the bidirectional layer is equal to that of the regular LSTM
			assertArrayEquals(recurrentWeightsF2.shape(), recurrentWeightsF.shape())
			assertArrayEquals(inputWeightsF2.shape(), inputWeightsF.shape())
			assertArrayEquals(biasWeightsF2.shape(), biasWeightsF.shape())
			forwardsLSTM.setParam(GravesLSTMParamInitializer.RECURRENT_WEIGHT_KEY, recurrentWeightsF)
			forwardsLSTM.setParam(GravesLSTMParamInitializer.INPUT_WEIGHT_KEY, inputWeightsF)
			forwardsLSTM.setParam(GravesLSTMParamInitializer.BIAS_KEY, biasWeightsF)
			' copy forwards weights to make the forwards activations do the same thing
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray recurrentWeightsB = bidirectionalLSTM.getParam(org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_BACKWARDS);
			Dim recurrentWeightsB As INDArray = bidirectionalLSTM.getParam(GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_BACKWARDS)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray inputWeightsB = bidirectionalLSTM.getParam(org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_BACKWARDS);
			Dim inputWeightsB As INDArray = bidirectionalLSTM.getParam(GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_BACKWARDS)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray biasWeightsB = bidirectionalLSTM.getParam(org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.BIAS_KEY_BACKWARDS);
			Dim biasWeightsB As INDArray = bidirectionalLSTM.getParam(GravesBidirectionalLSTMParamInitializer.BIAS_KEY_BACKWARDS)
			' assert that the forwards and backwards are the same shapes
			assertArrayEquals(recurrentWeightsF.shape(), recurrentWeightsB.shape())
			assertArrayEquals(inputWeightsF.shape(), inputWeightsB.shape())
			assertArrayEquals(biasWeightsF.shape(), biasWeightsB.shape())
			' zero out backwards layer
			bidirectionalLSTM.setParam(GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_BACKWARDS, Nd4j.zeros(recurrentWeightsB.shape()))
			bidirectionalLSTM.setParam(GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_BACKWARDS, Nd4j.zeros(inputWeightsB.shape()))
			bidirectionalLSTM.setParam(GravesBidirectionalLSTMParamInitializer.BIAS_KEY_BACKWARDS, Nd4j.zeros(biasWeightsB.shape()))
			forwardsLSTM.setInput(sig, LayerWorkspaceMgr.noWorkspaces())
			' compare activations
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray activation1 = forwardsLSTM.activate(sig, false, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr.noWorkspaces()).slice(0);
			Dim activation1 As INDArray = forwardsLSTM.activate(sig, False, LayerWorkspaceMgr.noWorkspaces()).slice(0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray activation2 = bidirectionalLSTM.activate(sig, false, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr.noWorkspaces()).slice(0);
			Dim activation2 As INDArray = bidirectionalLSTM.activate(sig, False, LayerWorkspaceMgr.noWorkspaces()).slice(0)
			assertArrayEquals(activation1.data().asFloat(), activation2.data().asFloat(), 1e-5f)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray randSig = (rnnDataFormat == org.deeplearning4j.nn.conf.RNNFormat.NCW) ? org.nd4j.linalg.factory.Nd4j.rand(new int[] { 1, layerSize, timeSeriesLength }) : org.nd4j.linalg.factory.Nd4j.rand(new int[] { 1, timeSeriesLength, layerSize });
			Dim randSig As INDArray = If(rnnDataFormat = RNNFormat.NCW, Nd4j.rand(New Integer() { 1, layerSize, timeSeriesLength }), Nd4j.rand(New Integer() { 1, timeSeriesLength, layerSize }))
			Dim randSigBackwards As INDArray = randSig.dup()
			If rnnDataFormat = RNNFormat.NCW Then
				reverseColumnsInPlace(randSigBackwards.slice(0))
			Else
				reverseColumnsInPlace(randSigBackwards.slice(0).permute(1, 0))
			End If
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.common.primitives.Pair<org.deeplearning4j.nn.gradient.Gradient, org.nd4j.linalg.api.ndarray.INDArray> backprop1 = forwardsLSTM.backpropGradient(randSig, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr.noWorkspaces());
			Dim backprop1 As Pair(Of Gradient, INDArray) = forwardsLSTM.backpropGradient(randSig, LayerWorkspaceMgr.noWorkspaces())
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.common.primitives.Pair<org.deeplearning4j.nn.gradient.Gradient, org.nd4j.linalg.api.ndarray.INDArray> backprop2 = bidirectionalLSTM.backpropGradient(randSig, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr.noWorkspaces());
			Dim backprop2 As Pair(Of Gradient, INDArray) = bidirectionalLSTM.backpropGradient(randSig, LayerWorkspaceMgr.noWorkspaces())
			' compare gradients
			assertArrayEquals(backprop1.First.getGradientFor(GravesLSTMParamInitializer.RECURRENT_WEIGHT_KEY).dup().data().asFloat(), backprop2.First.getGradientFor(GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_FORWARDS).dup().data().asFloat(), 1e-5f)
			assertArrayEquals(backprop1.First.getGradientFor(GravesLSTMParamInitializer.INPUT_WEIGHT_KEY).dup().data().asFloat(), backprop2.First.getGradientFor(GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS).dup().data().asFloat(), 1e-5f)
			assertArrayEquals(backprop1.First.getGradientFor(GravesLSTMParamInitializer.BIAS_KEY).dup().data().asFloat(), backprop2.First.getGradientFor(GravesBidirectionalLSTMParamInitializer.BIAS_KEY_FORWARDS).dup().data().asFloat(), 1e-5f)
			' copy forwards to backwards
			bidirectionalLSTM.setParam(GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_BACKWARDS, bidirectionalLSTM.getParam(GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_FORWARDS))
			bidirectionalLSTM.setParam(GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_BACKWARDS, bidirectionalLSTM.getParam(GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS))
			bidirectionalLSTM.setParam(GravesBidirectionalLSTMParamInitializer.BIAS_KEY_BACKWARDS, bidirectionalLSTM.getParam(GravesBidirectionalLSTMParamInitializer.BIAS_KEY_FORWARDS))
			' zero out forwards layer
			bidirectionalLSTM.setParam(GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_FORWARDS, Nd4j.zeros(recurrentWeightsB.shape()))
			bidirectionalLSTM.setParam(GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS, Nd4j.zeros(inputWeightsB.shape()))
			bidirectionalLSTM.setParam(GravesBidirectionalLSTMParamInitializer.BIAS_KEY_FORWARDS, Nd4j.zeros(biasWeightsB.shape()))
			' run on reversed signal
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray activation3 = bidirectionalLSTM.activate(sigb, false, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr.noWorkspaces()).slice(0);
			Dim activation3 As INDArray = bidirectionalLSTM.activate(sigb, False, LayerWorkspaceMgr.noWorkspaces()).slice(0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray activation3Reverse = activation3.dup();
			Dim activation3Reverse As INDArray = activation3.dup()
			If rnnDataFormat = RNNFormat.NCW Then
				reverseColumnsInPlace(activation3Reverse)
			Else
				reverseColumnsInPlace(activation3Reverse.permute(1, 0))
			End If
			assertArrayEquals(activation3Reverse.shape(), activation1.shape())
			assertEquals(activation3Reverse, activation1)
			' test backprop now
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray refBackGradientReccurrent = backprop1.getFirst().getGradientFor(org.deeplearning4j.nn.params.GravesLSTMParamInitializer.RECURRENT_WEIGHT_KEY);
			Dim refBackGradientReccurrent As INDArray = backprop1.First.getGradientFor(GravesLSTMParamInitializer.RECURRENT_WEIGHT_KEY)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray refBackGradientInput = backprop1.getFirst().getGradientFor(org.deeplearning4j.nn.params.GravesLSTMParamInitializer.INPUT_WEIGHT_KEY);
			Dim refBackGradientInput As INDArray = backprop1.First.getGradientFor(GravesLSTMParamInitializer.INPUT_WEIGHT_KEY)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray refBackGradientBias = backprop1.getFirst().getGradientFor(org.deeplearning4j.nn.params.GravesLSTMParamInitializer.BIAS_KEY);
			Dim refBackGradientBias As INDArray = backprop1.First.getGradientFor(GravesLSTMParamInitializer.BIAS_KEY)
			' reverse weights only with backwards signal should yield same result as forwards weights with forwards signal
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.common.primitives.Pair<org.deeplearning4j.nn.gradient.Gradient, org.nd4j.linalg.api.ndarray.INDArray> backprop3 = bidirectionalLSTM.backpropGradient(randSigBackwards, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr.noWorkspaces());
			Dim backprop3 As Pair(Of Gradient, INDArray) = bidirectionalLSTM.backpropGradient(randSigBackwards, LayerWorkspaceMgr.noWorkspaces())
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray backGradientRecurrent = backprop3.getFirst().getGradientFor(org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_BACKWARDS);
			Dim backGradientRecurrent As INDArray = backprop3.First.getGradientFor(GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_BACKWARDS)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray backGradientInput = backprop3.getFirst().getGradientFor(org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_BACKWARDS);
			Dim backGradientInput As INDArray = backprop3.First.getGradientFor(GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_BACKWARDS)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray backGradientBias = backprop3.getFirst().getGradientFor(org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.BIAS_KEY_BACKWARDS);
			Dim backGradientBias As INDArray = backprop3.First.getGradientFor(GravesBidirectionalLSTMParamInitializer.BIAS_KEY_BACKWARDS)
			assertArrayEquals(refBackGradientBias.dup().data().asDouble(), backGradientBias.dup().data().asDouble(), 1e-6)
			assertArrayEquals(refBackGradientInput.dup().data().asDouble(), backGradientInput.dup().data().asDouble(), 1e-6)
			assertArrayEquals(refBackGradientReccurrent.dup().data().asDouble(), backGradientRecurrent.dup().data().asDouble(), 1e-6)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray refEpsilon = backprop1.getSecond().dup();
			Dim refEpsilon As INDArray = backprop1.Second.dup()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray backEpsilon = backprop3.getSecond().dup();
			Dim backEpsilon As INDArray = backprop3.Second.dup()
			If rnnDataFormat = RNNFormat.NCW Then
				reverseColumnsInPlace(refEpsilon.slice(0))
			Else
				reverseColumnsInPlace(refEpsilon.slice(0).permute(1, 0))
			End If
			assertArrayEquals(backEpsilon.dup().data().asDouble(), refEpsilon.dup().data().asDouble(), 1e-6)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.deeplearning4j.nn.layers.recurrent.GravesBidirectionalLSTMTest#params") @DisplayName("Test Serialization") @ParameterizedTest void testSerialization(org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testSerialization(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.conf.MultiLayerConfiguration conf1 = new org.deeplearning4j.nn.conf.NeuralNetConfiguration.Builder().optimizationAlgo(org.deeplearning4j.nn.api.OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(new org.nd4j.linalg.learning.config.AdaGrad(0.1)).l2(0.001).seed(12345).list().layer(0, new org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM.Builder().activation(org.nd4j.linalg.activations.Activation.TANH).nIn(2).nOut(2).dist(new org.deeplearning4j.nn.conf.distribution.UniformDistribution(-0.05, 0.05)).build()).layer(1, new org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM.Builder().activation(org.nd4j.linalg.activations.Activation.TANH).nIn(2).nOut(2).dist(new org.deeplearning4j.nn.conf.distribution.UniformDistribution(-0.05, 0.05)).build()).layer(2, new org.deeplearning4j.nn.conf.layers.RnnOutputLayer.Builder().activation(org.nd4j.linalg.activations.Activation.SOFTMAX).lossFunction(org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction.MCXENT).nIn(2).nOut(2).build()).build();
			Dim conf1 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New AdaGrad(0.1)).l2(0.001).seed(12345).list().layer(0, (New org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM.Builder()).activation(Activation.TANH).nIn(2).nOut(2).dist(New UniformDistribution(-0.05, 0.05)).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM.Builder()).activation(Activation.TANH).nIn(2).nOut(2).dist(New UniformDistribution(-0.05, 0.05)).build()).layer(2, (New org.deeplearning4j.nn.conf.layers.RnnOutputLayer.Builder()).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).nIn(2).nOut(2).build()).build()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String json1 = conf1.toJson();
			Dim json1 As String = conf1.toJson()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.conf.MultiLayerConfiguration conf2 = org.deeplearning4j.nn.conf.MultiLayerConfiguration.fromJson(json1);
			Dim conf2 As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String json2 = conf1.toJson();
			Dim json2 As String = conf1.toJson()
			assertEquals(json1, json2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Gate Activation Fns Sanity Check") @MethodSource("org.deeplearning4j.nn.layers.recurrent.GravesBidirectionalLSTMTest#params") @ParameterizedTest void testGateActivationFnsSanityCheck(org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testGateActivationFnsSanityCheck(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)
			For Each gateAfn As String In New String() { "sigmoid", "hardsigmoid" }
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(12345).list().layer(0, (New org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM.Builder()).gateActivationFunction(gateAfn).activation(Activation.TANH).nIn(2).nOut(2).dataFormat(rnnDataFormat).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(2).nOut(2).dataFormat(rnnDataFormat).activation(Activation.TANH).build()).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				assertEquals(gateAfn, CType(net.getLayer(0).conf().getLayer(), org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM).getGateActivationFn().ToString())
				Dim [in] As INDArray = Nd4j.rand(New Integer() { 3, 2, 5 })
				Dim labels As INDArray = Nd4j.rand(New Integer() { 3, 2, 5 })
				If rnnDataFormat = RNNFormat.NWC Then
					[in] = [in].permute(0, 2, 1)
					labels = labels.permute(0, 2, 1)
				End If
				net.fit([in], labels)
			Next gateAfn
		End Sub
	End Class

End Namespace