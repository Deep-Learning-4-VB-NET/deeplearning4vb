Imports System
Imports System.Collections.Generic
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports UniformDistribution = org.deeplearning4j.nn.conf.distribution.UniformDistribution
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports GravesLSTMParamInitializer = org.deeplearning4j.nn.params.GravesLSTMParamInitializer
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.nd4j.common.primitives
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
Namespace org.deeplearning4j.nn.layers.recurrent

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Graves LSTM Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class GravesLSTMTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class GravesLSTMTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test LSTM Graves Forward Basic") void testLSTMGravesForwardBasic()
		Friend Overridable Sub testLSTMGravesForwardBasic()
			' Very basic test of forward prop. of LSTM layer with a time series.
			' Essentially make sure it doesn't throw any exceptions, and provides output in the correct shape.
			Dim nIn As Integer = 13
			Dim nHiddenUnits As Integer = 17
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(nIn).nOut(nHiddenUnits).activation(Activation.TANH).build()).build()
			Dim numParams As val = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim layer As GravesLSTM = CType(conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType()), GravesLSTM)
			' Data: has shape [miniBatchSize,nIn,timeSeriesLength];
			' Output/activations has shape [miniBatchsize,nHiddenUnits,timeSeriesLength];
			Dim dataSingleExampleTimeLength1 As INDArray = Nd4j.ones(1, nIn, 1)
			Dim activations1 As INDArray = layer.activate(dataSingleExampleTimeLength1, False, LayerWorkspaceMgr.noWorkspaces())
			assertArrayEquals(activations1.shape(), New Long() { 1, nHiddenUnits, 1 })
			Dim dataMultiExampleLength1 As INDArray = Nd4j.ones(10, nIn, 1)
			Dim activations2 As INDArray = layer.activate(dataMultiExampleLength1, False, LayerWorkspaceMgr.noWorkspaces())
			assertArrayEquals(activations2.shape(), New Long() { 10, nHiddenUnits, 1 })
			Dim dataSingleExampleLength12 As INDArray = Nd4j.ones(1, nIn, 12)
			Dim activations3 As INDArray = layer.activate(dataSingleExampleLength12, False, LayerWorkspaceMgr.noWorkspaces())
			assertArrayEquals(activations3.shape(), New Long() { 1, nHiddenUnits, 12 })
			Dim dataMultiExampleLength15 As INDArray = Nd4j.ones(10, nIn, 15)
			Dim activations4 As INDArray = layer.activate(dataMultiExampleLength15, False, LayerWorkspaceMgr.noWorkspaces())
			assertArrayEquals(activations4.shape(), New Long() { 10, nHiddenUnits, 15 })
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test LSTM Graves Backward Basic") void testLSTMGravesBackwardBasic()
		Friend Overridable Sub testLSTMGravesBackwardBasic()
			' Very basic test of backprop for mini-batch + time series
			' Essentially make sure it doesn't throw any exceptions, and provides output in the correct shape.
			testGravesBackwardBasicHelper(13, 3, 17, 10, 7)
			' Edge case: miniBatchSize = 1
			testGravesBackwardBasicHelper(13, 3, 17, 1, 7)
			' Edge case: timeSeriesLength = 1
			testGravesBackwardBasicHelper(13, 3, 17, 10, 1)
			' Edge case: both miniBatchSize = 1 and timeSeriesLength = 1
			testGravesBackwardBasicHelper(13, 3, 17, 1, 1)
		End Sub

		Private Shared Sub testGravesBackwardBasicHelper(ByVal nIn As Integer, ByVal nOut As Integer, ByVal lstmNHiddenUnits As Integer, ByVal miniBatchSize As Integer, ByVal timeSeriesLength As Integer)
			Dim inputData As INDArray = Nd4j.ones(miniBatchSize, nIn, timeSeriesLength)
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(nIn).nOut(lstmNHiddenUnits).dist(New UniformDistribution(0, 1)).activation(Activation.TANH).build()).build()
			Dim numParams As val = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim lstm As GravesLSTM = CType(conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType()), GravesLSTM)
			lstm.BackpropGradientsViewArray = Nd4j.create(1, conf.getLayer().initializer().numParams(conf))
			' Set input, do a forward pass:
			lstm.activate(inputData, False, LayerWorkspaceMgr.noWorkspaces())
			assertNotNull(lstm.input())
			Dim epsilon As INDArray = Nd4j.ones(miniBatchSize, lstmNHiddenUnits, timeSeriesLength)
			Dim [out] As Pair(Of Gradient, INDArray) = lstm.backpropGradient(epsilon, LayerWorkspaceMgr.noWorkspaces())
			Dim outGradient As Gradient = [out].First
			Dim nextEpsilon As INDArray = [out].Second
			Dim biasGradient As INDArray = outGradient.getGradientFor(GravesLSTMParamInitializer.BIAS_KEY)
			Dim inWeightGradient As INDArray = outGradient.getGradientFor(GravesLSTMParamInitializer.INPUT_WEIGHT_KEY)
			Dim recurrentWeightGradient As INDArray = outGradient.getGradientFor(GravesLSTMParamInitializer.RECURRENT_WEIGHT_KEY)
			assertNotNull(biasGradient)
			assertNotNull(inWeightGradient)
			assertNotNull(recurrentWeightGradient)
			assertArrayEquals(biasGradient.shape(), New Long() { 1, 4 * lstmNHiddenUnits })
			assertArrayEquals(inWeightGradient.shape(), New Long() { nIn, 4 * lstmNHiddenUnits })
			assertArrayEquals(recurrentWeightGradient.shape(), New Long() { lstmNHiddenUnits, 4 * lstmNHiddenUnits + 3 })
			assertNotNull(nextEpsilon)
			assertArrayEquals(nextEpsilon.shape(), New Long() { miniBatchSize, nIn, timeSeriesLength })
			' Check update:
			For Each s As String In outGradient.gradientForVariable().Keys
				lstm.update(outGradient.getGradientFor(s), s)
			Next s
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Graves LSTM Forward Pass Helper") void testGravesLSTMForwardPassHelper() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testGravesLSTMForwardPassHelper()
			' GravesLSTM.activateHelper() has different behaviour (due to optimizations) when forBackprop==true vs false
			' But should otherwise provide identical activations
			Nd4j.Random.setSeed(12345)
			Dim nIn As Integer = 10
			Dim layerSize As Integer = 15
			Dim miniBatchSize As Integer = 4
			Dim timeSeriesLength As Integer = 7
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(nIn).nOut(layerSize).dist(New UniformDistribution(0, 1)).activation(Activation.TANH).build()).build()
			Dim numParams As val = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim lstm As GravesLSTM = CType(conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType()), GravesLSTM)
			Dim input As INDArray = Nd4j.rand(New Integer() { miniBatchSize, nIn, timeSeriesLength })
			lstm.setInput(input, LayerWorkspaceMgr.noWorkspaces())
			Dim actHelper As System.Reflection.MethodInfo = GetType(GravesLSTM).getDeclaredMethod("activateHelper", GetType(Boolean), GetType(INDArray), GetType(INDArray), GetType(Boolean), GetType(LayerWorkspaceMgr))
			actHelper.setAccessible(True)
			' Call activateHelper with both forBackprop == true, and forBackprop == false and compare
			Dim innerClass As Type = DL4JClassLoading.loadClassByName("org.deeplearning4j.nn.layers.recurrent.FwdPassReturn")
			' GravesLSTM.FwdPassReturn object; want fwdPassOutput INDArray
			Dim oFalse As Object = actHelper.invoke(lstm, False, Nothing, Nothing, False, LayerWorkspaceMgr.noWorkspacesImmutable())
			' want fwdPassOutputAsArrays object
			Dim oTrue As Object = actHelper.invoke(lstm, False, Nothing, Nothing, True, LayerWorkspaceMgr.noWorkspacesImmutable())
			Dim fwdPassOutput As System.Reflection.FieldInfo = innerClass.getDeclaredField("fwdPassOutput")
			fwdPassOutput.setAccessible(True)
			Dim fwdPassOutputAsArrays As System.Reflection.FieldInfo = innerClass.getDeclaredField("fwdPassOutputAsArrays")
			fwdPassOutputAsArrays.setAccessible(True)
			Dim fwdPassFalse As INDArray = DirectCast(fwdPassOutput.get(oFalse), INDArray)
			Dim fwdPassTrue() As INDArray = CType(fwdPassOutputAsArrays.get(oTrue), INDArray())
			For i As Integer = 0 To timeSeriesLength - 1
				Dim sliceFalse As INDArray = fwdPassFalse.tensorAlongDimension(i, 1, 0)
				Dim sliceTrue As INDArray = fwdPassTrue(i)
				assertTrue(sliceFalse.Equals(sliceTrue))
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Single Example") void testSingleExample()
		Friend Overridable Sub testSingleExample()
			Nd4j.Random.setSeed(12345)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.1)).seed(12345).list().layer(0, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).activation(Activation.TANH).nIn(2).nOut(2).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(2).nOut(1).activation(Activation.TANH).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim in1 As INDArray = Nd4j.rand(New Integer() { 1, 2, 4 })
			Dim in2 As INDArray = Nd4j.rand(New Integer() { 1, 2, 5 })
			in2.put(New INDArrayIndex() { NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 4) }, in1)
			assertEquals(in1, in2.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 4)))
			Dim labels1 As INDArray = Nd4j.rand(New Integer() { 1, 1, 4 })
			Dim labels2 As INDArray = Nd4j.create(1, 1, 5)
			labels2.put(New INDArrayIndex() { NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 4) }, labels1)
			assertEquals(labels1, labels2.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 4)))
			Dim out1 As INDArray = net.output(in1)
			Dim out2 As INDArray = net.output(in2)
			' System.out.println(Arrays.toString(net.output(in1).data().asFloat()));
			' System.out.println(Arrays.toString(net.output(in2).data().asFloat()));
			Dim activations1 As IList(Of INDArray) = net.feedForward(in1)
			Dim activations2 As IList(Of INDArray) = net.feedForward(in2)
			' for (int i = 0; i < 3; i++) {
			' System.out.println("-----\n" + i);
			' System.out.println(Arrays.toString(activations1.get(i).dup().data().asDouble()));
			' System.out.println(Arrays.toString(activations2.get(i).dup().data().asDouble()));
			' 
			' System.out.println(activations1.get(i));
			' System.out.println(activations2.get(i));
			' }
			' Expect first 4 time steps to be indentical...
			For i As Integer = 0 To 3
				Dim d1 As Double = out1.getDouble(i)
				Dim d2 As Double = out2.getDouble(i)
				assertEquals(d1, d2, 0.0)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Gate Activation Fns Sanity Check") void testGateActivationFnsSanityCheck()
		Friend Overridable Sub testGateActivationFnsSanityCheck()
			For Each gateAfn As String In New String() { "sigmoid", "hardsigmoid" }
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(12345).list().layer(0, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).gateActivationFunction(gateAfn).activation(Activation.TANH).nIn(2).nOut(2).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(2).nOut(2).activation(Activation.TANH).build()).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				assertEquals(gateAfn, CType(net.getLayer(0).conf().getLayer(), org.deeplearning4j.nn.conf.layers.GravesLSTM).getGateActivationFn().ToString())
				Dim [in] As INDArray = Nd4j.rand(New Integer() { 3, 2, 5 })
				Dim labels As INDArray = Nd4j.rand(New Integer() { 3, 2, 5 })
				net.fit([in], labels)
			Next gateAfn
		End Sub
	End Class

End Namespace