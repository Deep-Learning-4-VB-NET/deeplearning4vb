Imports System
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports Updater = org.deeplearning4j.nn.api.Updater
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
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

Namespace org.deeplearning4j.nn.updater

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestGradientNormalization extends org.deeplearning4j.BaseDL4JTest
	Public Class TestGradientNormalization
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRenormalizatonPerLayer()
		Public Overridable Sub testRenormalizatonPerLayer()
			Nd4j.Random.setSeed(12345)

			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New DenseLayer.Builder()).nIn(10).nOut(20).updater(New NoOp()).gradientNormalization(GradientNormalization.RenormalizeL2PerLayer).build()).build()

			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim layer As Layer = conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType())
			Dim gradArray As INDArray = Nd4j.rand(1, 220).muli(10).subi(5)
			layer.BackpropGradientsViewArray = gradArray
			Dim weightGrad As INDArray = Shape.newShapeNoCopy(gradArray.get(NDArrayIndex.point(0), NDArrayIndex.interval(0, 200)), New Integer() {10, 20}, True)
			Dim biasGrad As INDArray = gradArray.get(NDArrayIndex.point(0), NDArrayIndex.interval(200, 220))
			Dim weightGradCopy As INDArray = weightGrad.dup()
			Dim biasGradCopy As INDArray = biasGrad.dup()
			Dim gradient As Gradient = New DefaultGradient(gradArray)
			gradient.setGradientFor(DefaultParamInitializer.WEIGHT_KEY, weightGrad)
			gradient.setGradientFor(DefaultParamInitializer.BIAS_KEY, biasGrad)

			Dim updater As Updater = UpdaterCreator.getUpdater(layer)
			updater.update(layer, gradient, 0, 0, 1, LayerWorkspaceMgr.noWorkspaces())

			assertNotEquals(weightGradCopy, weightGrad)
			assertNotEquals(biasGradCopy, biasGrad)

			Dim sumSquaresWeight As Double = weightGradCopy.mul(weightGradCopy).sumNumber().doubleValue()
			Dim sumSquaresBias As Double = biasGradCopy.mul(biasGradCopy).sumNumber().doubleValue()
			Dim sumSquares As Double = sumSquaresWeight + sumSquaresBias
			Dim l2Layer As Double = Math.Sqrt(sumSquares)

			Dim normWeightsExpected As INDArray = weightGradCopy.div(l2Layer)
			Dim normBiasExpected As INDArray = biasGradCopy.div(l2Layer)

			Dim l2Weight As Double = gradient.getGradientFor(DefaultParamInitializer.WEIGHT_KEY).norm2Number().doubleValue()
			Dim l2Bias As Double = gradient.getGradientFor(DefaultParamInitializer.BIAS_KEY).norm2Number().doubleValue()
			assertTrue(Not Double.IsNaN(l2Weight) AndAlso l2Weight > 0.0)
			assertTrue(Not Double.IsNaN(l2Bias) AndAlso l2Bias > 0.0)
			assertEquals(normWeightsExpected, gradient.getGradientFor(DefaultParamInitializer.WEIGHT_KEY))
			assertEquals(normBiasExpected, gradient.getGradientFor(DefaultParamInitializer.BIAS_KEY))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRenormalizationPerParamType()
		Public Overridable Sub testRenormalizationPerParamType()
			Nd4j.Random.setSeed(12345)

			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New DenseLayer.Builder()).nIn(10).nOut(20).updater(New NoOp()).gradientNormalization(GradientNormalization.RenormalizeL2PerParamType).build()).build()

			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim layer As Layer = conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType())
			layer.BackpropGradientsViewArray = Nd4j.create(params.shape())
			Dim updater As Updater = UpdaterCreator.getUpdater(layer)
			Dim weightGrad As INDArray = Nd4j.rand(10, 20)
			Dim biasGrad As INDArray = Nd4j.rand(1, 20)
			Dim weightGradCopy As INDArray = weightGrad.dup()
			Dim biasGradCopy As INDArray = biasGrad.dup()
			Dim gradient As Gradient = New DefaultGradient()
			gradient.setGradientFor(DefaultParamInitializer.WEIGHT_KEY, weightGrad)
			gradient.setGradientFor(DefaultParamInitializer.BIAS_KEY, biasGrad)

			updater.update(layer, gradient, 0, 0, 1, LayerWorkspaceMgr.noWorkspaces())

			Dim normWeightsExpected As INDArray = weightGradCopy.div(weightGradCopy.norm2Number())
			Dim normBiasExpected As INDArray = biasGradCopy.div(biasGradCopy.norm2Number())

			assertEquals(normWeightsExpected, gradient.getGradientFor(DefaultParamInitializer.WEIGHT_KEY))
			assertEquals(normBiasExpected, gradient.getGradientFor(DefaultParamInitializer.BIAS_KEY))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAbsValueClippingPerElement()
		Public Overridable Sub testAbsValueClippingPerElement()
			Nd4j.Random.setSeed(12345)
			Dim threshold As Double = 3

			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New DenseLayer.Builder()).nIn(10).nOut(20).updater(New NoOp()).gradientNormalization(GradientNormalization.ClipElementWiseAbsoluteValue).gradientNormalizationThreshold(threshold).build()).build()

			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim layer As Layer = conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType())
			Dim gradArray As INDArray = Nd4j.rand(1, 220).muli(10).subi(5)
			layer.BackpropGradientsViewArray = gradArray
			Dim weightGrad As INDArray = Shape.newShapeNoCopy(gradArray.get(NDArrayIndex.point(0), NDArrayIndex.interval(0, 200)), New Integer() {10, 20}, True)
			Dim biasGrad As INDArray = gradArray.get(NDArrayIndex.point(0), NDArrayIndex.interval(200, 220))
			Dim weightGradCopy As INDArray = weightGrad.dup()
			Dim biasGradCopy As INDArray = biasGrad.dup()
			Dim gradient As Gradient = New DefaultGradient(gradArray)
			gradient.setGradientFor(DefaultParamInitializer.WEIGHT_KEY, weightGrad)
			gradient.setGradientFor(DefaultParamInitializer.BIAS_KEY, biasGrad)

			Dim updater As Updater = UpdaterCreator.getUpdater(layer)
			updater.update(layer, gradient, 0, 0, 1, LayerWorkspaceMgr.noWorkspaces())

			assertNotEquals(weightGradCopy, weightGrad)
			assertNotEquals(biasGradCopy, biasGrad)

			Dim expectedWeightGrad As INDArray = weightGradCopy.dup()
			Dim i As Integer = 0
			Do While i < expectedWeightGrad.length()
				Dim d As Double = expectedWeightGrad.getDouble(i)
				If d > threshold Then
					expectedWeightGrad.putScalar(i, threshold)
				ElseIf d < -threshold Then
					expectedWeightGrad.putScalar(i, -threshold)
				End If
				i += 1
			Loop
			Dim expectedBiasGrad As INDArray = biasGradCopy.dup()
			i = 0
			Do While i < expectedBiasGrad.length()
				Dim d As Double = expectedBiasGrad.getDouble(i)
				If d > threshold Then
					expectedBiasGrad.putScalar(i, threshold)
				ElseIf d < -threshold Then
					expectedBiasGrad.putScalar(i, -threshold)
				End If
				i += 1
			Loop

			assertEquals(expectedWeightGrad, gradient.getGradientFor(DefaultParamInitializer.WEIGHT_KEY))
			assertEquals(expectedBiasGrad, gradient.getGradientFor(DefaultParamInitializer.BIAS_KEY))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testL2ClippingPerLayer()
		Public Overridable Sub testL2ClippingPerLayer()
			Nd4j.Random.setSeed(12345)
			Dim threshold As Double = 3

			For t As Integer = 0 To 1
				't=0: small -> no clipping
				't=1: large -> clipping

				Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New DenseLayer.Builder()).nIn(10).nOut(20).updater(New NoOp()).gradientNormalization(GradientNormalization.ClipL2PerLayer).gradientNormalizationThreshold(threshold).build()).build()

				Dim numParams As val = conf.getLayer().initializer().numParams(conf)
				Dim params As INDArray = Nd4j.create(1, numParams)
				Dim layer As Layer = conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType())
				Dim gradArray As INDArray = Nd4j.rand(1, 220).muli(If(t = 0, 0.05, 10)).subi(If(t = 0, 0, 5))
				layer.BackpropGradientsViewArray = gradArray
				Dim weightGrad As INDArray = Shape.newShapeNoCopy(gradArray.get(NDArrayIndex.point(0), NDArrayIndex.interval(0, 200)), New Integer() {10, 20}, True)
				Dim biasGrad As INDArray = gradArray.get(NDArrayIndex.point(0), NDArrayIndex.interval(200, 220))
				Dim weightGradCopy As INDArray = weightGrad.dup()
				Dim biasGradCopy As INDArray = biasGrad.dup()
				Dim gradient As Gradient = New DefaultGradient(gradArray)
				gradient.setGradientFor(DefaultParamInitializer.WEIGHT_KEY, weightGrad)
				gradient.setGradientFor(DefaultParamInitializer.BIAS_KEY, biasGrad)

				Dim layerGradL2 As Double = gradient.gradient().norm2Number().doubleValue()
				If t = 0 Then
					assertTrue(layerGradL2 < threshold)
				Else
					assertTrue(layerGradL2 > threshold)
				End If

				Dim updater As Updater = UpdaterCreator.getUpdater(layer)
				updater.update(layer, gradient, 0, 0, 1, LayerWorkspaceMgr.noWorkspaces())

				If t = 0 Then
					'norm2 < threshold -> no change
					assertEquals(weightGradCopy, weightGrad)
					assertEquals(biasGradCopy, biasGrad)
					Continue For
				Else
					'norm2 > threshold -> rescale
					assertNotEquals(weightGradCopy, weightGrad)
					assertNotEquals(biasGradCopy, biasGrad)
				End If

				'for above threshold only...
				Dim scalingFactor As Double = threshold / layerGradL2
				Dim expectedWeightGrad As INDArray = weightGradCopy.mul(scalingFactor)
				Dim expectedBiasGrad As INDArray = biasGradCopy.mul(scalingFactor)
				assertEquals(expectedWeightGrad, gradient.getGradientFor(DefaultParamInitializer.WEIGHT_KEY))
				assertEquals(expectedBiasGrad, gradient.getGradientFor(DefaultParamInitializer.BIAS_KEY))
			Next t
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testL2ClippingPerParamType()
		Public Overridable Sub testL2ClippingPerParamType()
			Nd4j.Random.setSeed(12345)
			Dim threshold As Double = 3

			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer((New DenseLayer.Builder()).nIn(10).nOut(20).updater(New NoOp()).gradientNormalization(GradientNormalization.ClipL2PerParamType).gradientNormalizationThreshold(threshold).build()).build()

			Dim numParams As val = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim layer As Layer = conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType())
			layer.BackpropGradientsViewArray = Nd4j.create(params.shape())
			Dim updater As Updater = UpdaterCreator.getUpdater(layer)
			Dim weightGrad As INDArray = Nd4j.rand(10, 20).muli(0.05)
			Dim biasGrad As INDArray = Nd4j.rand(1, 20).muli(10)
			Dim weightGradCopy As INDArray = weightGrad.dup()
			Dim biasGradCopy As INDArray = biasGrad.dup()
			Dim gradient As Gradient = New DefaultGradient()
			gradient.setGradientFor(DefaultParamInitializer.WEIGHT_KEY, weightGrad)
			gradient.setGradientFor(DefaultParamInitializer.BIAS_KEY, biasGrad)

			Dim weightL2 As Double = weightGrad.norm2Number().doubleValue()
			Dim biasL2 As Double = biasGrad.norm2Number().doubleValue()
			assertTrue(weightL2 < threshold)
			assertTrue(biasL2 > threshold)

			updater.update(layer, gradient, 0, 0, 1, LayerWorkspaceMgr.noWorkspaces())

			assertEquals(weightGradCopy, weightGrad) 'weight norm2 < threshold -> no change
			assertNotEquals(biasGradCopy, biasGrad) 'bias norm2 > threshold -> rescale


			Dim biasScalingFactor As Double = threshold / biasL2
			Dim expectedBiasGrad As INDArray = biasGradCopy.mul(biasScalingFactor)
			assertEquals(expectedBiasGrad, gradient.getGradientFor(DefaultParamInitializer.BIAS_KEY))
		End Sub
	End Class

End Namespace