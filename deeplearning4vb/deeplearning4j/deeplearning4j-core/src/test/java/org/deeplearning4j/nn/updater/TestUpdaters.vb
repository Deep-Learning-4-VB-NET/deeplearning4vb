Imports System
Imports System.Collections.Generic
Imports FastMath = org.apache.commons.math3.util.FastMath
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports Updater = org.deeplearning4j.nn.api.Updater
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports org.deeplearning4j.nn.conf.layers
Imports VariationalAutoencoder = org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports org.deeplearning4j.nn.layers
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports PretrainParamInitializer = org.deeplearning4j.nn.params.PretrainParamInitializer
Imports ComputationGraphUpdater = org.deeplearning4j.nn.updater.graph.ComputationGraphUpdater
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.linalg.learning
Imports org.nd4j.linalg.learning.config
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports org.junit.jupiter.api.Assertions
import static org.nd4j.linalg.indexing.NDArrayIndex.interval
import static org.nd4j.linalg.indexing.NDArrayIndex.point

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

Namespace org.deeplearning4j.nn.updater



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestUpdaters extends org.deeplearning4j.BaseDL4JTest
	Public Class TestUpdaters
		Inherits BaseDL4JTest

		Protected Friend nIn As Integer = 3
		Protected Friend nOut As Integer = 2
		'    protected double epsilon = 1e-8;
		Protected Friend gradients As INDArray
		Protected Friend weightGradient As INDArray
		Protected Friend biasGradient As INDArray
		Protected Friend gradient As New DefaultGradient()
		Protected Friend val, gradExpected As INDArray
		Protected Friend key As String


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void beforeDo()
		Public Overridable Sub beforeDo()
			gradients = Nd4j.ones(1, nIn * nOut + nOut)
			weightGradient = gradients.get(point(0), interval(0, nIn * nOut))
			biasGradient = gradients.get(point(0), interval(nIn * nOut, nIn * nOut + nOut))
			gradient.setGradientFor(DefaultParamInitializer.WEIGHT_KEY, weightGradient)
			gradient.setGradientFor(DefaultParamInitializer.BIAS_KEY, biasGradient)
			gradient.setFlattenedGradient(gradients)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAdaDeltaUpdate()
		Public Overridable Sub testAdaDeltaUpdate()
			'Here: test updaters manually vs. using updater
			Dim dxSquared As INDArray
			Dim msg As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			Dim msdx As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()

			Dim rho As Double = 0.85

			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New DenseLayer.Builder()).nIn(nIn).nOut(nOut).updater(New AdaDelta(rho, Nd4j.EPS_THRESHOLD)).build()).build()

			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim layer As BaseLayer = CType(conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType()), BaseLayer)
			layer.setBackpropGradientsViewArray(gradients)
			Dim updater As Updater = UpdaterCreator.getUpdater(layer)
			Dim updaterStateSize As Integer = CInt(Math.Truncate(layer.layerConf().getIUpdater().stateSize(numParams)))
			Dim updaterState As INDArray = Nd4j.create(1, updaterStateSize)
			updater.setStateViewArray(layer, updaterState, True)

			Dim gradientCopyPreUpdate As Gradient = New DefaultGradient()
			Dim g As INDArray = gradients.dup()
			Dim wg As INDArray = g.get(point(0), interval(0, nIn * nOut))
			Dim bg As INDArray = g.get(point(0), interval(nIn * nOut, nIn * nOut + nOut))
			gradientCopyPreUpdate.setGradientFor(DefaultParamInitializer.WEIGHT_KEY, wg)
			gradientCopyPreUpdate.setGradientFor(DefaultParamInitializer.BIAS_KEY, bg)

			Dim count As Integer = 0
			For i As Integer = 0 To 1
				updater.update(layer, gradient, i, 0, 1, LayerWorkspaceMgr.noWorkspaces())

				' calculations for one iteration / update

				For Each entry As KeyValuePair(Of String, INDArray) In gradientCopyPreUpdate.gradientForVariable().SetOfKeyValuePairs()
					key = entry.Key
					val = entry.Value
					Dim msgTmp As INDArray = msg(key)
					Dim msdxTmp As INDArray = msdx(key)

					If msgTmp Is Nothing Then
						msgTmp = Nd4j.zeros(val.shape())
						msdxTmp = Nd4j.zeros(val.shape())
					End If

					msgTmp.muli(rho)
					msgTmp.addi(val.mul(val).muli(1 - rho))

					gradExpected = Transforms.sqrt(msdxTmp.add(Nd4j.EPS_THRESHOLD)).divi(Transforms.sqrt(msgTmp.add(Nd4j.EPS_THRESHOLD))).muli(val)
					gradientCopyPreUpdate.setGradientFor(key, gradExpected)

					assertEquals(gradExpected, gradient.getGradientFor(entry.Key))

					msdxTmp.muli(rho)
					dxSquared = gradExpected.mul(gradExpected)
					msdxTmp.addi(dxSquared.muli(1 - rho))

					msg(key) = msgTmp
					msdx(key) = msdxTmp
					count += 1
				Next entry
				assertEquals(rho, CType(layer.layerConf().getIUpdater(), AdaDelta).getRho(), 1e-4)
			Next i

			assertEquals(4, count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAdaGradUpdater()
		Public Overridable Sub testAdaGradUpdater()
			Dim lr As Double = 1e-2
			Dim epsilon As Double = AdaGrad.DEFAULT_ADAGRAD_EPSILON

			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).updater(New AdaGrad(lr)).layer((New DenseLayer.Builder()).nIn(nIn).nOut(nOut).build()).build()

			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim layer As BaseLayer = CType(conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType()), BaseLayer)
			layer.setBackpropGradientsViewArray(gradients)
			Dim updater As Updater = UpdaterCreator.getUpdater(layer)
			Dim updaterStateSize As Integer = CInt(Math.Truncate(layer.layerConf().getIUpdater().stateSize(numParams)))
			Dim updaterState As INDArray = Nd4j.create(1, updaterStateSize)
			updater.setStateViewArray(layer, updaterState, True)

			Dim gradientCopyPreUpdate As Gradient = New DefaultGradient()
			Dim g As INDArray = gradients.dup()
			Dim wg As INDArray = g.get(point(0), interval(0, nIn * nOut))
			Dim bg As INDArray = g.get(point(0), interval(nIn * nOut, nIn * nOut + nOut))
			gradientCopyPreUpdate.setGradientFor(DefaultParamInitializer.WEIGHT_KEY, wg)
			gradientCopyPreUpdate.setGradientFor(DefaultParamInitializer.BIAS_KEY, bg)

			updater.update(layer, gradient, -1, 0, 1, LayerWorkspaceMgr.noWorkspaces())

			Dim count As Integer = 0
			For Each entry As KeyValuePair(Of String, INDArray) In gradientCopyPreUpdate.gradientForVariable().SetOfKeyValuePairs()
				val = entry.Value
				gradExpected = Transforms.sqrt(val.mul(val).add(epsilon)).rdiv(lr).mul(val)
				assertEquals(gradExpected, gradient.getGradientFor(entry.Key))
				count += 1
			Next entry
			assertEquals(lr, CType(layer.layerConf().getIUpdater(), AdaGrad).getLearningRate(), 1e-4)
			assertEquals(2, count)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAdamUpdater()
		Public Overridable Sub testAdamUpdater()
			Dim m, v As INDArray
			Dim lr As Double = 0.01
			Dim iteration As Integer = 0
			Dim beta1 As Double = 0.8
			Dim beta2 As Double = 0.888
			Dim epsilon As Double = Adam.DEFAULT_ADAM_EPSILON


			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Adam(lr, beta1, beta2, Adam.DEFAULT_ADAM_EPSILON)).layer((New DenseLayer.Builder()).nIn(nIn).nOut(nOut).build()).build()

			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim layer As BaseLayer = CType(conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType()), BaseLayer)
			layer.setBackpropGradientsViewArray(gradients)
			Dim updater As Updater = UpdaterCreator.getUpdater(layer)
			Dim updaterStateSize As Integer = CInt(Math.Truncate(layer.layerConf().getIUpdater().stateSize(numParams)))
			Dim updaterState As INDArray = Nd4j.create(1, updaterStateSize)
			updater.setStateViewArray(layer, updaterState, True)

			updater.update(layer, gradient, iteration, 0, 1, LayerWorkspaceMgr.noWorkspaces())

			Dim beta1t As Double = FastMath.pow(beta1, iteration + 1)
			Dim beta2t As Double = FastMath.pow(beta2, iteration + 1)
			Dim alphat As Double = lr * FastMath.sqrt(1 - beta2t) / (1 - beta1t)
			If Double.IsNaN(alphat) OrElse alphat = 0.0 Then
				alphat = epsilon
			End If

			Dim gradientCopyPreUpdate As Gradient = New DefaultGradient()
			Dim g As INDArray = gradients.dup()
			Dim wg As INDArray = g.get(point(0), interval(0, nIn * nOut))
			Dim bg As INDArray = g.get(point(0), interval(nIn * nOut, nIn * nOut + nOut))
			gradientCopyPreUpdate.setGradientFor(DefaultParamInitializer.WEIGHT_KEY, wg)
			gradientCopyPreUpdate.setGradientFor(DefaultParamInitializer.BIAS_KEY, bg)

			Dim count As Integer = 0
			For Each entry As KeyValuePair(Of String, INDArray) In gradientCopyPreUpdate.gradientForVariable().SetOfKeyValuePairs()
				val = entry.Value
				m = Nd4j.zeros(val.shape())
				v = Nd4j.zeros(val.shape())

				m.muli(beta1).addi(val.mul(1.0 - beta1))
				v.muli(beta2).addi(val.mul(val).mul(1.0 - beta2))
				gradExpected = m.mul(alphat).divi(Transforms.sqrt(v).addi(epsilon))
				If Not gradExpected.Equals(gradient.getGradientFor(entry.Key)) Then
					Console.WriteLine(java.util.Arrays.toString(gradExpected.dup().data().asFloat()))
					Console.WriteLine(java.util.Arrays.toString(gradient.getGradientFor(entry.Key).dup().data().asFloat()))
				End If
				assertEquals(gradExpected, gradient.getGradientFor(entry.Key))
				count += 1
			Next entry

			assertEquals(beta1, CType(layer.layerConf().getIUpdater(), Adam).getBeta1(), 1e-4)
			assertEquals(beta2, CType(layer.layerConf().getIUpdater(), Adam).getBeta2(), 1e-4)
			assertEquals(2, count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNadamUpdater()
		Public Overridable Sub testNadamUpdater()
			Dim m, v As INDArray
			Dim lr As Double = 0.01
			Dim iteration As Integer = 0
			Dim beta1 As Double = 0.8
			Dim beta2 As Double = 0.888
			Dim epsilon As Double = Nadam.DEFAULT_NADAM_EPSILON

			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New DenseLayer.Builder()).nIn(nIn).nOut(nOut).updater(Nadam.builder().learningRate(lr).beta1(beta1).beta2(beta2).epsilon(epsilon).build()).build()).build()

			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim layer As BaseLayer = CType(conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType()), BaseLayer)
			layer.setBackpropGradientsViewArray(gradients)

			Dim updater As Updater = UpdaterCreator.getUpdater(layer)
			Dim updaterStateSize As Integer = CInt(Math.Truncate(layer.layerConf().getIUpdater().stateSize(numParams)))
			Dim updaterState As INDArray = Nd4j.create(1, updaterStateSize)
			updater.setStateViewArray(layer, updaterState, True)

	'        
	'        * Making update for layer
	'        * 
			updater.update(layer, gradient, iteration, 0,1, LayerWorkspaceMgr.noWorkspaces())

			Dim beta1t As Double = FastMath.pow(beta1, iteration + 1)

			Dim gradientCopyPreUpdate As Gradient = New DefaultGradient()
			Dim g As INDArray = gradients.dup()
			Dim wg As INDArray = g.get(point(0), interval(0, nIn * nOut))
			Dim bg As INDArray = g.get(point(0), interval(nIn * nOut, nIn * nOut + nOut))
			gradientCopyPreUpdate.setGradientFor(DefaultParamInitializer.WEIGHT_KEY, wg)
			gradientCopyPreUpdate.setGradientFor(DefaultParamInitializer.BIAS_KEY, bg)

			Dim count As Integer = 0
			For Each entry As KeyValuePair(Of String, INDArray) In gradientCopyPreUpdate.gradientForVariable().SetOfKeyValuePairs()
				val = entry.Value
				m = Nd4j.zeros(val.shape())
				v = Nd4j.zeros(val.shape())

				Dim oneMinusBeta1Grad As INDArray = val.mul(1.0 - beta1)
				m.muli(beta1).addi(oneMinusBeta1Grad)

				Dim oneMinusBeta2GradSquared As INDArray = val.mul(val).muli(1.0 - beta2)
				v.muli(beta2).addi(oneMinusBeta2GradSquared)

				Dim biasCorrectedEstimateOfMomentum As INDArray = m.mul(beta1).divi(1.0 - beta1t)
				Dim secondTerm As INDArray = oneMinusBeta1Grad.divi(1.0 - beta1t)

				Dim alphat As INDArray = biasCorrectedEstimateOfMomentum.add(secondTerm).muli(lr)

				Dim sqrtV As INDArray = Transforms.sqrt(v, False).addi(epsilon)

				gradExpected = val.assign(alphat).divi(sqrtV)
				If Not gradExpected.Equals(gradient.getGradientFor(entry.Key)) Then
					Console.WriteLine(java.util.Arrays.toString(gradExpected.dup().data().asFloat()))
					Console.WriteLine(java.util.Arrays.toString(gradient.getGradientFor(entry.Key).dup().data().asFloat()))
				End If
				assertEquals(gradExpected, gradient.getGradientFor(entry.Key))
				count += 1
			Next entry

			assertEquals(2, count,"Count should be equal to 2, one for weight gradient and one for bias gradient")

	'        
	'        * Check that we are not erroneously mutating moving avg gradient while calculating
	'        * `biasCorrectedEstimateOfMomentum = m * beta1 /(1.0 - beta1t);`
	'        * 
			Dim baseUpdater As BaseMultiLayerUpdater = DirectCast(updater, BaseMultiLayerUpdater)
			Dim ub As UpdaterBlock = CType(baseUpdater.getUpdaterBlocks().get(0), UpdaterBlock)
'JAVA TO VB CONVERTER NOTE: The variable nadamUpdater was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim nadamUpdater_Conflict As NadamUpdater = CType(ub.GradientUpdater, NadamUpdater)


			'Calculated for following setup: initialWeights are all equal to 1, beta1 = 0.8, beta2 = 0.888, learning rate = 0.01
			Dim calculatedByHandMScalar As Double = 0.2
			Dim expectedM() As Double = Nd4j.ones(1, numParams).mul(calculatedByHandMScalar).data().asDouble()

			Dim actualM() As Double = Arrays.CopyOfRange(nadamUpdater_Conflict.getM().data().asDouble(), 0, CInt(numParams))
			For i As Integer = 0 To actualM.Length - 1
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				actualM(i) = CLng(Math.Round(actualM(i) * 1e2, MidpointRounding.AwayFromZero)) / 1e2
			Next i

			assertEquals(expectedM.SequenceEqual(actualM), True, "Wrong weight gradient after first iteration's update")

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAdaMaxUpdater()
		Public Overridable Sub testAdaMaxUpdater()
			Dim m, v As INDArray
			Dim lr As Double = 0.01
			Dim iteration As Integer = 0
			Dim beta1 As Double = 0.8
			Dim beta2 As Double = 0.888
			Dim epsilon As Double = AdaMax.DEFAULT_ADAMAX_EPSILON

			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).updater(New AdaMax(lr, beta1, beta2, AdaMax.DEFAULT_ADAMAX_EPSILON)).layer((New DenseLayer.Builder()).nIn(nIn).nOut(nOut).build()).build()

			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim layer As BaseLayer = CType(conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType()), BaseLayer)
			layer.setBackpropGradientsViewArray(gradients)
			Dim updater As Updater = UpdaterCreator.getUpdater(layer)
			Dim updaterStateSize As Integer = CInt(Math.Truncate(layer.layerConf().getIUpdater().stateSize(numParams)))
			Dim updaterState As INDArray = Nd4j.create(1, updaterStateSize)
			updater.setStateViewArray(layer, updaterState, True)

			updater.update(layer, gradient, iteration, 0, 1, LayerWorkspaceMgr.noWorkspaces())

			Dim beta1t As Double = FastMath.pow(beta1, iteration + 1)
			Dim beta2t As Double = FastMath.pow(beta2, iteration + 1)
			Dim alphat As Double = lr * FastMath.sqrt(1 - beta2t) / (1 - beta1t)
			If Double.IsNaN(alphat) OrElse alphat = 0.0 Then
				alphat = epsilon
			End If

			Dim gradientCopyPreUpdate As Gradient = New DefaultGradient()
			Dim g As INDArray = gradients.dup()
			Dim wg As INDArray = g.get(point(0), interval(0, nIn * nOut))
			Dim bg As INDArray = g.get(point(0), interval(nIn * nOut, nIn * nOut + nOut))
			gradientCopyPreUpdate.setGradientFor(DefaultParamInitializer.WEIGHT_KEY, wg)
			gradientCopyPreUpdate.setGradientFor(DefaultParamInitializer.BIAS_KEY, bg)

			Dim count As Integer = 0
			For Each entry As KeyValuePair(Of String, INDArray) In gradientCopyPreUpdate.gradientForVariable().SetOfKeyValuePairs()
				val = entry.Value
				m = Nd4j.zeros(val.shape())
				v = Nd4j.zeros(val.shape())

				m.muli(beta1).addi(val.mul(1.0 - beta1))
				v.muli(beta2).addi(val.mul(val).mul(1.0 - beta2))
				gradExpected = m.mul(alphat).divi(Transforms.sqrt(v).addi(epsilon))
				If Not gradExpected.Equals(gradient.getGradientFor(entry.Key)) Then
					Console.WriteLine(java.util.Arrays.toString(gradExpected.dup().data().asFloat()))
					Console.WriteLine(java.util.Arrays.toString(gradient.getGradientFor(entry.Key).dup().data().asFloat()))
				End If
				assertEquals(gradExpected, gradient.getGradientFor(entry.Key))
				count += 1
			Next entry

			assertEquals(beta1, CType(layer.layerConf().getIUpdater(), AdaMax).getBeta1(), 1e-4)
			assertEquals(beta2, CType(layer.layerConf().getIUpdater(), AdaMax).getBeta2(), 1e-4)
			assertEquals(2, count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNestorovsUpdater()
		Public Overridable Sub testNestorovsUpdater()
			Dim lr As Double = 1e-2
			Dim mu As Double = 0.6

			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Nesterovs(lr, mu)).layer((New DenseLayer.Builder()).nIn(nIn).nOut(nOut).build()).build()

			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim layer As BaseLayer = CType(conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType()), BaseLayer)
			layer.setBackpropGradientsViewArray(gradients)
			Dim updater As Updater = UpdaterCreator.getUpdater(layer)
			Dim updaterStateSize As Integer = CInt(Math.Truncate(layer.layerConf().getIUpdater().stateSize(numParams)))
			Dim updaterState As INDArray = Nd4j.create(1, updaterStateSize)
			updater.setStateViewArray(layer, updaterState, True)

			Dim gradientCopyPreUpdate As Gradient = New DefaultGradient()
			Dim g As INDArray = gradients.dup()
			Dim wg As INDArray = g.get(point(0), interval(0, nIn * nOut))
			Dim bg As INDArray = g.get(point(0), interval(nIn * nOut, nIn * nOut + nOut))
			gradientCopyPreUpdate.setGradientFor(DefaultParamInitializer.WEIGHT_KEY, wg)
			gradientCopyPreUpdate.setGradientFor(DefaultParamInitializer.BIAS_KEY, bg)

			updater.update(layer, gradient, -1, 0, 1, LayerWorkspaceMgr.noWorkspaces())

			Dim count As Integer = 0
			For Each entry As KeyValuePair(Of String, INDArray) In gradientCopyPreUpdate.gradientForVariable().SetOfKeyValuePairs()
				Dim val As INDArray = entry.Value
				Dim v As INDArray = Nd4j.create(val.shape())
				Dim vPrev As INDArray = v.dup()
				v = v.mul(mu).subi(val.mul(lr))
				gradExpected = vPrev.muli(mu).addi(v.mul(-mu - 1))

				assertEquals(gradExpected, gradient.getGradientFor(entry.Key))
				count += 1
			Next entry

			assertEquals(mu, CType(layer.layerConf().getIUpdater(), Nesterovs).getMomentum(), 1e-4)
			assertEquals(2, count)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRMSPropUpdater()
		Public Overridable Sub testRMSPropUpdater()
			Dim lr As Double = 0.01
			Dim rmsDecay As Double = 0.25
			Dim lastG As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()


			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).updater(New RmsProp(lr,rmsDecay, RmsProp.DEFAULT_RMSPROP_EPSILON)).layer((New DenseLayer.Builder()).nIn(nIn).nOut(nOut).build()).build()

			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim layer As BaseLayer = CType(conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType()), BaseLayer)
			layer.setBackpropGradientsViewArray(gradients)
			Dim updater As Updater = UpdaterCreator.getUpdater(layer)
			Dim updaterStateSize As Integer = CInt(Math.Truncate(layer.layerConf().getIUpdater().stateSize(numParams)))
			Dim updaterState As INDArray = Nd4j.create(1, updaterStateSize)
			updater.setStateViewArray(layer, updaterState, True)


			Dim gradientCopyPreUpdate As Gradient = New DefaultGradient()
			Dim g As INDArray = gradients.dup()
			Dim wg As INDArray = g.get(point(0), interval(0, nIn * nOut))
			Dim bg As INDArray = g.get(point(0), interval(nIn * nOut, nIn * nOut + nOut))
			gradientCopyPreUpdate.setGradientFor(DefaultParamInitializer.WEIGHT_KEY, wg)
			gradientCopyPreUpdate.setGradientFor(DefaultParamInitializer.BIAS_KEY, bg)

			updater.update(layer, gradient, -1, 0, 1, LayerWorkspaceMgr.noWorkspaces())

			Dim epsilon As Double = 1e-8

			For Each entry As KeyValuePair(Of String, INDArray) In gradientCopyPreUpdate.gradientForVariable().SetOfKeyValuePairs()
				key = entry.Key
				val = entry.Value
				Dim lastGTmp As INDArray = lastG(key)

				If lastGTmp Is Nothing Then
					lastGTmp = Nd4j.zeros(val.shape())
				End If

				lastGTmp.muli(rmsDecay).addi(val.mul(val).muli(1 - rmsDecay))
				gradExpected = val.mul(lr).div(Transforms.sqrt(lastGTmp.add(epsilon)))

				assertEquals(gradExpected, gradient.getGradientFor(entry.Key))
				lastG(key) = lastGTmp
			Next entry
			assertEquals(rmsDecay, CType(layer.layerConf().getIUpdater(), RmsProp).getRmsDecay(), 1e-4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSGDUpdater()
		Public Overridable Sub testSGDUpdater()
			Dim lr As Double = 0.05

			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(lr)).layer((New DenseLayer.Builder()).nIn(nIn).nOut(nOut).build()).build()

			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim layer As BaseLayer = CType(conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType()), BaseLayer)
			layer.setBackpropGradientsViewArray(gradients)
			Dim updater As Updater = UpdaterCreator.getUpdater(layer)

			Dim gradientCopyPreUpdate As Gradient = New DefaultGradient()
			Dim g As INDArray = gradients.dup()
			Dim wg As INDArray = g.get(point(0), interval(0, nIn * nOut))
			Dim bg As INDArray = g.get(point(0), interval(nIn * nOut, nIn * nOut + nOut))
			gradientCopyPreUpdate.setGradientFor(DefaultParamInitializer.WEIGHT_KEY, wg)
			gradientCopyPreUpdate.setGradientFor(DefaultParamInitializer.BIAS_KEY, bg)

			updater.update(layer, gradient, -1, 0, 1, LayerWorkspaceMgr.noWorkspaces())

			For Each entry As KeyValuePair(Of String, INDArray) In gradientCopyPreUpdate.gradientForVariable().SetOfKeyValuePairs()
				val = entry.Value
				gradExpected = val.mul(lr)
				assertEquals(gradExpected, gradient.getGradientFor(entry.Key))
			Next entry
			assertEquals(lr, CType(layer.layerConf().getIUpdater(), Sgd).getLearningRate(), 1e-4)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNoOpUpdater()
		Public Overridable Sub testNoOpUpdater()
			Dim r As New Random(CInt(12345L))
			Dim lr As Double = 0.5

			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).layer((New DenseLayer.Builder()).nIn(nIn).nOut(nOut).build()).build()

			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim layer As Layer = conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType())
			layer.BackpropGradientsViewArray = gradients
			Dim updater As Updater = UpdaterCreator.getUpdater(layer)

			Dim i As Integer = 0
			Do While i < weightGradient.length()
				weightGradient.putScalar(i, r.NextDouble())
				i += 1
			Loop
			i = 0
			Do While i < biasGradient.length()
				biasGradient.putScalar(i, r.NextDouble())
				i += 1
			Loop

			Dim g As INDArray = gradients.dup()
			Dim wg As INDArray = g.get(point(0), interval(0, nIn * nOut))
			Dim bg As INDArray = g.get(point(0), interval(nIn * nOut, nIn * nOut + nOut))
			gradient.gradientForVariable()(DefaultParamInitializer.WEIGHT_KEY) = wg
			gradient.gradientForVariable()(DefaultParamInitializer.BIAS_KEY) = bg

			updater.update(layer, gradient, -1, 0, 1, LayerWorkspaceMgr.noWorkspaces())

			Dim weightGradActual As INDArray = gradient.getGradientFor(DefaultParamInitializer.WEIGHT_KEY)
			Dim biasGradActual As INDArray = gradient.getGradientFor(DefaultParamInitializer.BIAS_KEY)

			assertEquals(wg, weightGradActual)
			assertEquals(bg, biasGradActual)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMultiLayerUpdater() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultiLayerUpdater()
			Nd4j.Random.Seed = 12345L
			Dim lr As Double = 0.03

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(5).updater(New Sgd(lr)).build()).layer(1, (New DenseLayer.Builder()).nIn(5).nOut(6).updater(New NoOp()).build()).layer(2, (New DenseLayer.Builder()).nIn(6).nOut(7).updater(New AdaGrad(lr)).build()).layer(3, (New OutputLayer.Builder()).nIn(7).nOut(8).updater(New Nesterovs(0.6)).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.MSE).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()
			net.fit(Nd4j.create(1, 4), Nd4j.create(1, 8))

			Dim updater As Updater = net.Updater
			assertNotNull(updater)
			assertTrue(updater.GetType() = GetType(MultiLayerUpdater))

			Dim mlu As MultiLayerUpdater = DirectCast(updater, MultiLayerUpdater)

			Dim count As Integer = 0
			For Each u As UpdaterBlock In mlu.getUpdaterBlocks()
				Dim gu As GradientUpdater = u.GradientUpdater
				Select Case count
					Case 0
						assertTrue(TypeOf gu Is SgdUpdater)
					Case 1
						assertTrue(TypeOf gu Is NoOpUpdater)
					Case 2
						assertTrue(TypeOf gu Is AdaGradUpdater)
					Case 3
						assertTrue(TypeOf gu Is NesterovsUpdater)
					Case Else
						Throw New Exception()
				End Select
				count += 1
			Next u


			Dim uArr(3) As GradientUpdater
			uArr(0) = New SgdUpdater(New Sgd(lr))
			uArr(1) = New NoOpUpdater(New NoOp())
			uArr(2) = New AdaGradUpdater(New AdaGrad(lr, AdaGrad.DEFAULT_ADAGRAD_EPSILON))
			Dim updaterState As INDArray = Nd4j.create(1, 6 * 7 + 7, "f"c)
			uArr(2).setStateViewArray(updaterState, New Long() {1, 6 * 7 + 7}, "f"c, True)

			uArr(3) = New NesterovsUpdater(New Nesterovs(lr, 0.6))
			'        updaterStateSize = uArr[3].stateSizeForLayer(net.getLayer(3));
			updaterState = Nd4j.create(1, 7 * 8 + 8, "f"c)
			uArr(3).setStateViewArray(updaterState, New Long() {1, 7 * 8 + 8}, "f"c, True)

			Dim nIns() As Integer = {4, 5, 6, 7}
			Dim nOuts() As Integer = {5, 6, 7, 8}

			For i As Integer = 0 To 4
				Dim gradient As Gradient = New DefaultGradient()
				Dim expectedGradient As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()

				Dim j As Integer = 0
				Do While j < net.getnLayers()
					'Generate test gradient:
					Dim wGrad As INDArray = Nd4j.rand(DataType.FLOAT, nIns(j), nOuts(j))
					Dim bGrad As INDArray = Nd4j.rand(DataType.FLOAT, 1, nOuts(j))

					Dim wKey As String = j & "_" & DefaultParamInitializer.WEIGHT_KEY
					Dim bKey As String = j & "_" & DefaultParamInitializer.BIAS_KEY

					gradient.setGradientFor(wKey, wGrad)
					gradient.setGradientFor(bKey, bGrad)

					'Also put copy of gradient through separate layer updaters to compare
					Dim layerGradient As Gradient = New DefaultGradient()
					layerGradient.setGradientFor(DefaultParamInitializer.WEIGHT_KEY, wGrad.dup())
					layerGradient.setGradientFor(DefaultParamInitializer.BIAS_KEY, bGrad.dup())

	'                uArr[j].getConfig().applySchedules(0, net.getLayer(j).conf().getLearningRateByParam("W"));
					For Each s As String In layerGradient.gradientForVariable().Keys
						expectedGradient(j & "_" & s) = layerGradient.getGradientFor(s)
					Next s
					j += 1
				Loop

				updater.update(net, gradient, i, 0, 1, LayerWorkspaceMgr.noWorkspaces())
				assertEquals(gradient.gradientForVariable(), expectedGradient)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSetGetUpdater()
		Public Overridable Sub testSetGetUpdater()

			Nd4j.Random.Seed = 12345L
			Dim lr As Double = 0.03

			Dim nIn As Integer = 4
			Dim nOut As Integer = 8

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Nesterovs(lr,0.6)).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(5).updater(org.deeplearning4j.nn.conf.Updater.SGD).build()).layer(1, (New DenseLayer.Builder()).nIn(5).nOut(6).updater(New NoOp()).build()).layer(2, (New DenseLayer.Builder()).nIn(6).nOut(7).updater(org.deeplearning4j.nn.conf.Updater.ADAGRAD).build()).layer(3, (New OutputLayer.Builder()).nIn(7).nOut(nOut).activation(Activation.SOFTMAX).updater(org.deeplearning4j.nn.conf.Updater.NESTEROVS).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()
			net.fit(Nd4j.rand(5, nIn), Nd4j.rand(5, nOut)) 'Fit, to initialize optimizer/updater

			Dim updater As Updater = net.Updater
			assertTrue(TypeOf updater Is MultiLayerUpdater)

			Dim newUpdater As Updater = UpdaterCreator.getUpdater(net)
			net.Updater = newUpdater
			assertTrue(newUpdater Is net.Updater) 'Should be identical object
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSetGetUpdater2()
		Public Overridable Sub testSetGetUpdater2()
			'Same as above test, except that we are doing setUpdater on a new network
			Nd4j.Random.Seed = 12345L
			Dim lr As Double = 0.03
			Dim nIn As Integer = 4
			Dim nOut As Integer = 8

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Nesterovs(lr,0.6)).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(5).updater(org.deeplearning4j.nn.conf.Updater.SGD).build()).layer(1, (New DenseLayer.Builder()).nIn(5).nOut(6).updater(New NoOp()).build()).layer(2, (New DenseLayer.Builder()).nIn(6).nOut(7).updater(org.deeplearning4j.nn.conf.Updater.ADAGRAD).build()).layer(3, (New OutputLayer.Builder()).nIn(7).nOut(nOut).activation(Activation.SOFTMAX).updater(org.deeplearning4j.nn.conf.Updater.NESTEROVS).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim newUpdater As Updater = UpdaterCreator.getUpdater(net)
			net.Updater = newUpdater
			assertTrue(newUpdater Is net.Updater) 'Should be identical object
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPretrain()
		Public Overridable Sub testPretrain()

			gradients = Nd4j.ones(1, nIn * nOut + nOut + nIn)
			weightGradient = gradients.get(point(0), interval(0, nIn * nOut))
			biasGradient = gradients.get(point(0), interval(nIn * nOut, nIn * nOut + nOut))
			Dim vbiasGradient As INDArray = gradients.get(point(0), interval(nIn * nOut + nOut, nIn * nOut + nOut + nIn))
			gradient.setFlattenedGradient(gradients)


			'Test with pretrain = true
			Dim lr As Double = 0.05
			gradient.setGradientFor(DefaultParamInitializer.WEIGHT_KEY, weightGradient)
			gradient.setGradientFor(DefaultParamInitializer.BIAS_KEY, biasGradient)
			gradient.setGradientFor(PretrainParamInitializer.VISIBLE_BIAS_KEY, vbiasGradient)


			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(lr)).seed(42).layer((New AutoEncoder.Builder()).lossFunction(LossFunctions.LossFunction.COSINE_PROXIMITY).activation(Activation.IDENTITY).nIn(nIn).nOut(nOut).build()).build()
			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim layer As BaseLayer = CType(conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType()), BaseLayer)
			layer.setBackpropGradientsViewArray(gradients)
			Dim updater As Updater = UpdaterCreator.getUpdater(layer)

			Dim gradientCopyPreUpdate As New DefaultGradient()
			Dim g As INDArray = gradients.dup()
			Dim wg As INDArray = g.get(point(0), interval(0, nIn * nOut))
			Dim bg As INDArray = g.get(point(0), interval(nIn * nOut, nIn * nOut + nOut))
			Dim vbg As INDArray = g.get(point(0), interval(nIn * nOut + nOut, nIn * nOut + nOut + nIn))
			gradientCopyPreUpdate.setGradientFor(DefaultParamInitializer.WEIGHT_KEY, wg)
			gradientCopyPreUpdate.setGradientFor(DefaultParamInitializer.BIAS_KEY, bg)
			gradientCopyPreUpdate.setGradientFor(PretrainParamInitializer.VISIBLE_BIAS_KEY, vbg)

			updater.update(layer, gradient, -1, 0, 1, LayerWorkspaceMgr.noWorkspaces())

			For Each entry As KeyValuePair(Of String, INDArray) In gradientCopyPreUpdate.gradientForVariable().SetOfKeyValuePairs()
				val = entry.Value
				gradExpected = val.mul(lr)
				assertEquals(gradExpected, gradient.getGradientFor(entry.Key))
			Next entry
			assertEquals(lr, CType(layer.layerConf().getIUpdater(), Sgd).getLearningRate(), 1e-4)


			'Test with pretrain == false
			gradients = Nd4j.ones(1, nIn * nOut + nOut + nIn)
			weightGradient = gradients.get(point(0), interval(0, nIn * nOut))
			biasGradient = gradients.get(point(0), interval(nIn * nOut, nIn * nOut + nOut))
			vbiasGradient = gradients.get(point(0), interval(nIn * nOut + nOut, nIn * nOut + nOut + nIn))
			gradient.setGradientFor(DefaultParamInitializer.WEIGHT_KEY, weightGradient)
			gradient.setGradientFor(DefaultParamInitializer.BIAS_KEY, biasGradient)
			gradient.setGradientFor(PretrainParamInitializer.VISIBLE_BIAS_KEY, vbiasGradient)
			gradient.setFlattenedGradient(gradients)

			gradientCopyPreUpdate = New DefaultGradient()
			g = gradients.dup()
			wg = g.get(point(0), interval(0, nIn * nOut))
			bg = g.get(point(0), interval(nIn * nOut, nIn * nOut + nOut))
			vbg = g.get(point(0), interval(nIn * nOut + nOut, nIn * nOut + nOut + nIn))
			gradientCopyPreUpdate.setGradientFor(DefaultParamInitializer.WEIGHT_KEY, wg)
			gradientCopyPreUpdate.setGradientFor(DefaultParamInitializer.BIAS_KEY, bg)
			gradientCopyPreUpdate.setGradientFor(PretrainParamInitializer.VISIBLE_BIAS_KEY, vbg)
			gradientCopyPreUpdate.setFlattenedGradient(g)

			params = Nd4j.create(1, numParams)
			layer = CType(conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType()), BaseLayer)
			layer.setBackpropGradientsViewArray(gradients)
			updater = UpdaterCreator.getUpdater(layer)
			assertEquals(lr, CType(layer.layerConf().getIUpdater(), Sgd).getLearningRate(), 1e-4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUpdaterBlockMlnAndCG()
		Public Overridable Sub testUpdaterBlockMlnAndCG()
			For i As Integer = 0 To 1

				Dim blocks As IList(Of UpdaterBlock)
				If i = 0 Then
					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(10).name("l0").updater(New Adam(0.5)).build()).layer(1, (New DenseLayer.Builder()).nIn(10).nOut(10).name("l1").updater(New Adam(0.5)).biasUpdater(New Adam(0.25)).build()).layer(2, (New DenseLayer.Builder()).nIn(10).nOut(10).name("l2").updater(New AdaDelta()).build()).layer(3, (New DenseLayer.Builder()).nIn(10).nOut(10).name("l3").updater(New AdaGrad(0.5)).build()).layer(4, (New OutputLayer.Builder()).nIn(10).nOut(10).name("l4").activation(Activation.SOFTMAX).updater(New AdaMax(0.5)).build()).build()

					Dim net As New MultiLayerNetwork(conf)
					net.init()

					Dim u As MultiLayerUpdater = DirectCast(net.Updater, MultiLayerUpdater)
					blocks = u.getUpdaterBlocks()
				Else
					Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("l0", (New DenseLayer.Builder()).nIn(10).nOut(10).updater(New Adam(0.5)).build(), "in").addLayer("l1", (New DenseLayer.Builder()).nIn(10).nOut(10).updater(New Adam(0.5)).biasUpdater(New Adam(0.25)).build(), "l0").addLayer("l2", (New DenseLayer.Builder()).nIn(10).nOut(10).updater(New AdaDelta()).build(), "l1").addLayer("l3", (New DenseLayer.Builder()).nIn(10).nOut(10).updater(New AdaGrad(0.5)).build(), "l2").addLayer("l4", (New OutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).updater(New AdaMax(0.5)).build(), "l3").setOutputs("l4").build()

					Dim net As New ComputationGraph(conf)
					net.init()

					Dim u As ComputationGraphUpdater = net.Updater
					blocks = u.getUpdaterBlocks()
				End If


				'Expect 4 blocks: (layer0 W, layer0 B, layer 1 W], [layer 1 B], [layer 2 W, layer 2 B],
				' [layer 3 W, layer 3 B], [layer 4 W, layer 4 B]
				assertEquals(5, blocks.Count)


				'Check first updater block:
				Dim ub0 As UpdaterBlock = blocks(0)
				assertEquals(3, ub0.getLayersAndVariablesInBlock().size())
				assertEquals("l0", ub0.getLayersAndVariablesInBlock().get(0).getLayer().getConfig().getLayerName())
				assertEquals(DefaultParamInitializer.WEIGHT_KEY, ub0.getLayersAndVariablesInBlock().get(0).getParamName())
				assertEquals("l0", ub0.getLayersAndVariablesInBlock().get(1).getLayer().getConfig().getLayerName())
				assertEquals(DefaultParamInitializer.BIAS_KEY, ub0.getLayersAndVariablesInBlock().get(1).getParamName())
				assertEquals("l1", ub0.getLayersAndVariablesInBlock().get(2).getLayer().getConfig().getLayerName())
				assertEquals(DefaultParamInitializer.WEIGHT_KEY, ub0.getLayersAndVariablesInBlock().get(2).getParamName())

				Dim nParams0 As Integer = 10 * 10 + 10 + 10 * 10
				assertEquals(0, ub0.getParamOffsetStart())
				assertEquals(nParams0, ub0.getParamOffsetEnd())
				Dim nUpdaterVals0 As Integer = 2 * nParams0 '2x for Adam
				assertEquals(0, ub0.getUpdaterViewOffsetStart())
				assertEquals(nUpdaterVals0, ub0.getUpdaterViewOffsetEnd())

				'Check second updater block:
				Dim ub1 As UpdaterBlock = blocks(1)
				assertEquals(1, ub1.getLayersAndVariablesInBlock().size())
				assertEquals("l1", ub1.getLayersAndVariablesInBlock().get(0).getLayer().getConfig().getLayerName())
				assertEquals(DefaultParamInitializer.BIAS_KEY, ub1.getLayersAndVariablesInBlock().get(0).getParamName())

				Dim nParams1 As Integer = 10
				assertEquals(nParams0, ub1.getParamOffsetStart())
				assertEquals(nParams0 + nParams1, ub1.getParamOffsetEnd())
				Dim nUpdaterVals1 As Integer = 2 * nParams1 '2x for Adam
				assertEquals(nUpdaterVals0, ub1.getUpdaterViewOffsetStart())
				assertEquals(nUpdaterVals0 + nUpdaterVals1, ub1.getUpdaterViewOffsetEnd())

				'Check third updater block:
				Dim ub2 As UpdaterBlock = blocks(2)
				assertEquals(2, ub2.getLayersAndVariablesInBlock().size())
				assertEquals("l2", ub2.getLayersAndVariablesInBlock().get(0).getLayer().getConfig().getLayerName())
				assertEquals(DefaultParamInitializer.WEIGHT_KEY, ub2.getLayersAndVariablesInBlock().get(0).getParamName())
				assertEquals("l2", ub2.getLayersAndVariablesInBlock().get(1).getLayer().getConfig().getLayerName())
				assertEquals(DefaultParamInitializer.BIAS_KEY, ub2.getLayersAndVariablesInBlock().get(1).getParamName())

				Dim nParams2 As Integer = 10 * 10 + 10
				assertEquals(nParams0 + nParams1, ub2.getParamOffsetStart())
				assertEquals(nParams0 + nParams1 + nParams2, ub2.getParamOffsetEnd())
				Dim nUpdaterVals2 As Integer = 2 * nParams2 '2x for Adadelta
				assertEquals(nUpdaterVals0 + nUpdaterVals1, ub2.getUpdaterViewOffsetStart())
				assertEquals(nUpdaterVals0 + nUpdaterVals1 + nUpdaterVals2, ub2.getUpdaterViewOffsetEnd())

				'Check fourth updater block:
				Dim ub3 As UpdaterBlock = blocks(3)
				assertEquals(2, ub3.getLayersAndVariablesInBlock().size())
				assertEquals("l3", ub3.getLayersAndVariablesInBlock().get(0).getLayer().getConfig().getLayerName())
				assertEquals(DefaultParamInitializer.WEIGHT_KEY, ub3.getLayersAndVariablesInBlock().get(0).getParamName())
				assertEquals("l3", ub3.getLayersAndVariablesInBlock().get(1).getLayer().getConfig().getLayerName())
				assertEquals(DefaultParamInitializer.BIAS_KEY, ub3.getLayersAndVariablesInBlock().get(1).getParamName())

				Dim nParams3 As Integer = 10 * 10 + 10
				assertEquals(nParams0 + nParams1 + nParams2, ub3.getParamOffsetStart())
				assertEquals(nParams0 + nParams1 + nParams2 + nParams3, ub3.getParamOffsetEnd())
				Dim nUpdaterVals3 As Integer = nParams3 '1x for AdaGrad
				assertEquals(nUpdaterVals0 + nUpdaterVals1 + nUpdaterVals2, ub3.getUpdaterViewOffsetStart())
				assertEquals(nUpdaterVals0 + nUpdaterVals1 + nUpdaterVals2 + nUpdaterVals3, ub3.getUpdaterViewOffsetEnd())

				'Check fifth updater black
				Dim ub4 As UpdaterBlock = blocks(4)
				assertEquals(2, ub4.getLayersAndVariablesInBlock().size())
				assertEquals("l4", ub4.getLayersAndVariablesInBlock().get(0).getLayer().getConfig().getLayerName())
				assertEquals(DefaultParamInitializer.WEIGHT_KEY, ub4.getLayersAndVariablesInBlock().get(0).getParamName())
				assertEquals("l4", ub4.getLayersAndVariablesInBlock().get(1).getLayer().getConfig().getLayerName())
				assertEquals(DefaultParamInitializer.BIAS_KEY, ub4.getLayersAndVariablesInBlock().get(1).getParamName())

				Dim nParams4 As Integer = 10 * 10 + 10
				assertEquals(nParams0 + nParams1 + nParams2 + nParams3, ub4.getParamOffsetStart())
				assertEquals(nParams0 + nParams1 + nParams2 + nParams3 + nParams4, ub4.getParamOffsetEnd())
				Dim nUpdaterVals4 As Integer = 2 * nParams4 '2x for AdaGrad
				assertEquals(nUpdaterVals0 + nUpdaterVals1 + nUpdaterVals2 + nUpdaterVals3, ub4.getUpdaterViewOffsetStart())
				assertEquals(nUpdaterVals0 + nUpdaterVals1 + nUpdaterVals2 + nUpdaterVals3 + nUpdaterVals4, ub4.getUpdaterViewOffsetEnd())
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUpdaterBlockVae()
		Public Overridable Sub testUpdaterBlockVae()

			Dim blocks As IList(Of UpdaterBlock)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Adam(0.5)).list().layer(0, (New VariationalAutoencoder.Builder()).nIn(8).nOut(12).encoderLayerSizes(10, 11).decoderLayerSizes(13, 14).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim u As MultiLayerUpdater = DirectCast(net.Updater, MultiLayerUpdater)
			blocks = u.getUpdaterBlocks()


			'Expect 2 blocks: Standard, and pretrain-only params
			assertEquals(2, blocks.Count)


			'Check first updater block (all backprop-only params)
			Dim ub0 As UpdaterBlock = blocks(0)
			Dim expParams As IList(Of String) = New List(Of String) From {"e0W", "e0b", "e1W", "e1b", "pZXMeanW", "pZXMeanb"}
			Dim actParams As IList(Of String) = New List(Of String)()
			For Each vs As UpdaterBlock.ParamState In ub0.getLayersAndVariablesInBlock()
				actParams.Add(vs.getParamName())
			Next vs
			assertEquals(expParams, actParams)

			'Check second updater block
			Dim ub1 As UpdaterBlock = blocks(1)
			expParams = New List(Of String) From {"pZXLogStd2W", "pZXLogStd2b", "d0W", "d0b", "d1W", "d1b", "pXZW", "pXZb"}
			actParams = New List(Of String)()
			For Each vs As UpdaterBlock.ParamState In ub1.getLayersAndVariablesInBlock()
				actParams.Add(vs.getParamName())
			Next vs
			assertEquals(expParams, actParams)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDivisionByMinibatch1()
		Public Overridable Sub testDivisionByMinibatch1()
			'No batch norm - should be single INDArray equal to flattened gradient view

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer((New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer((New OutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			net.fit(Nd4j.create(1,10), Nd4j.create(1,10))

			Dim u As BaseMultiLayerUpdater = DirectCast(net.Updater, BaseMultiLayerUpdater)
			Dim l As IList(Of INDArray) = u.getGradientsForMinibatchDivision()
			assertNotNull(l)
			assertEquals(1, l.Count)

			Dim arr As INDArray = l(0)
			assertEquals(3 * (10 * 10 + 10), arr.length())
			assertEquals(net.getFlattenedGradients(), arr)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDivisionByMinibatch2()
		Public Overridable Sub testDivisionByMinibatch2()
			'With batch norm - should be multiple 'division by minibatch' array segments
			'i.e., exclude batch norm mean/variance

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New DenseLayer.Builder()).nIn(10).nOut(9).build()).layer((New BatchNormalization.Builder()).nOut(9).build()).layer((New DenseLayer.Builder()).nIn(9).nOut(8).build()).layer((New BatchNormalization.Builder()).nOut(8).build()).layer((New OutputLayer.Builder()).nIn(8).nOut(7).activation(Activation.SOFTMAX).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			net.fit(Nd4j.create(1,10), Nd4j.create(1,7))

			Dim u As BaseMultiLayerUpdater = DirectCast(net.Updater, BaseMultiLayerUpdater)
			Dim l As IList(Of INDArray) = u.getGradientsForMinibatchDivision()
			assertNotNull(l)
			assertEquals(3, l.Count) '3 segments

			'First subset: 0_W, 0_b, 1_gamma, 1_beta           Size    10x9 + 9 + 2x9
			'Then excluding 1_mean, 1_var
			'Second subset: 2_W, 2_b, 3_gamma, 3_beta          Size    9x8 + 8 + 2x8
			'Then excluding 3_mean, 3_var
			'Third subset: 4_W, 4_b                            Size    8x7 + 7

			assertEquals(10*9 + 9 + 2*9, l(0).length())
			assertEquals(9*8 + 8 + 2*8, l(1).length())
			assertEquals(8*7 + 7, l(2).length())

			Dim view As INDArray = DirectCast(net.Updater, BaseMultiLayerUpdater).getFlattenedGradientsView()
			view.assign(Nd4j.linspace(1, view.length(), view.length(), Nd4j.dataType()))

			Dim expView1 As INDArray = view.get(interval(0,0,True), interval(0, 10*9 + 9 + 2*9))
			assertEquals(expView1, l(0))

			Dim start2 As Long = (10*9 + 9 + 2*9) + 2*9
			Dim length2 As Long = 9*8 + 8 + 2*8
			Dim expView2 As INDArray = view.get(interval(0,0,True), interval(start2, start2 + length2))
			assertEquals(expView2, l(1))

			Dim start3 As Long = start2 + length2 + 2*8
			Dim length3 As Long = 8*7 + 7
			Dim expView3 As INDArray = view.get(interval(0,0,True), interval(start3, start3 + length3))
			assertEquals(expView3, l(2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDivisionByMinibatch3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDivisionByMinibatch3()
			'With batch norm - should be multiple 'division by minibatch' array segments
			'i.e., exclude batch norm mean/variance

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New BatchNormalization.Builder()).nOut(6).build()).layer((New ConvolutionLayer.Builder()).nIn(6).nOut(5).kernelSize(2,2).build()).layer((New BatchNormalization.Builder()).nOut(5).build()).layer((New ConvolutionLayer.Builder()).nIn(5).nOut(4).kernelSize(2,2).build()).layer((New BatchNormalization.Builder()).nOut(4).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()


			Dim u As BaseMultiLayerUpdater = DirectCast(net.Updater, BaseMultiLayerUpdater)

			Dim m As System.Reflection.MethodInfo = GetType(BaseMultiLayerUpdater).getDeclaredMethod("divideByMinibatch", GetType(Boolean), GetType(Gradient), GetType(Integer))
			m.setAccessible(True)
			m.invoke(u, False, Nothing, 32)

			Dim l As IList(Of INDArray) = u.getGradientsForMinibatchDivision()
			assertNotNull(l)
			assertEquals(3, l.Count) '3 segments

			'First subset: 0_gamma, 0_beta,                    2x6
			'Then excluding 0_mean, 0_var
			'Second subset: 1_b, 1_W, 2_gamma, 2_beta          (6x5x2x2) + 5 + 2x5
			'Then excluding 2_mean, 2_var
			'Third subset: 3_b, 3_W, 4_gamma, 4_beta           (5*4*2*2) + 4 + 2*4
			'Then excluding 4_mean, 4_beta

			assertEquals(2*6, l(0).length())
			assertEquals(6*5*2*2 + 5 + 2*5, l(1).length())
			assertEquals(5*4*2*2 + 4 + 2*4, l(2).length())

			Dim view As INDArray = DirectCast(net.Updater, BaseMultiLayerUpdater).getFlattenedGradientsView()
			view.assign(Nd4j.linspace(1, view.length(), view.length(), Nd4j.dataType()))

			Dim expView1 As INDArray = view.get(interval(0,0,True), interval(0, 2*6))
			assertEquals(expView1, l(0))

			Dim start2 As Long = 2*6 + 2*6
			Dim length2 As Long = 6*5*2*2 + 5 + 2*5
			Dim expView2 As INDArray = view.get(interval(0,0,True), interval(start2, start2 + length2))
			assertEquals(expView2, l(1))

			Dim start3 As Long = start2 + length2 + 2*5
			Dim length3 As Long = 5*4*2*2 + 4 + 2*4
			Dim expView3 As INDArray = view.get(interval(0,0,True), interval(start3, start3 + length3))
			assertEquals(expView3, l(2))
		End Sub
	End Class

End Namespace