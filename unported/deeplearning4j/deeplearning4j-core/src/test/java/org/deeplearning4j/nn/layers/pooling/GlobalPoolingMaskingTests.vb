Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports org.deeplearning4j.nn.conf.layers
Imports GlobalPoolingLayer = org.deeplearning4j.nn.conf.layers.GlobalPoolingLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BroadcastMulOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMulOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.assertEquals
Imports org.nd4j.linalg.indexing.NDArrayIndex

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

Namespace org.deeplearning4j.nn.layers.pooling


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class GlobalPoolingMaskingTests extends org.deeplearning4j.BaseDL4JTest
	Public Class GlobalPoolingMaskingTests
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMaskingRnn()
		Public Overridable Sub testMaskingRnn()


			Dim timeSeriesLength As Integer = 5
			Dim nIn As Integer = 5
			Dim layerSize As Integer = 4
			Dim nOut As Integer = 2
			Dim minibatchSizes() As Integer = {1, 3}

			For Each miniBatchSize As Integer In minibatchSizes

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dist(New NormalDistribution(0, 1.0)).seed(12345L).list().layer(0, (New GravesLSTM.Builder()).nIn(nIn).nOut(layerSize).activation(Activation.TANH).build()).layer(1, (New GlobalPoolingLayer.Builder()).poolingType(PoolingType.AVG).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(layerSize).nOut(nOut).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim r As New Random(12345L)
				Dim input As INDArray = Nd4j.rand(New Integer() {miniBatchSize, nIn, timeSeriesLength}).subi(0.5)

				Dim mask As INDArray
				If miniBatchSize = 1 Then
					mask = Nd4j.create(New Double() {1, 1, 1, 1, 0}).reshape(ChrW(1), 5)
				Else
					mask = Nd4j.create(New Double()() {
						New Double() {1, 1, 1, 1, 1},
						New Double() {1, 1, 1, 1, 0},
						New Double() {1, 1, 1, 0, 0}
					})
				End If

				Dim labels As INDArray = Nd4j.zeros(miniBatchSize, nOut)
				For i As Integer = 0 To miniBatchSize - 1
					Dim idx As Integer = r.Next(nOut)
					labels.putScalar(i, idx, 1.0)
				Next i

				net.setLayerMaskArrays(mask, Nothing)
				Dim outputMasked As INDArray = net.output(input)

				net.clearLayerMaskArrays()

				For i As Integer = 0 To miniBatchSize - 1
					Dim maskRow As INDArray = mask.getRow(i)
					Dim tsLength As Integer = maskRow.sumNumber().intValue()
					Dim inputSubset As INDArray = input.get(interval(i, i, True), all(), interval(0, tsLength))

					Dim outSubset As INDArray = net.output(inputSubset)
					Dim outputMaskedSubset As INDArray = outputMasked.getRow(i,True)

					assertEquals(outSubset, outputMaskedSubset)
				Next i
			Next miniBatchSize
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMaskingCnnDim3_SingleExample()
		Public Overridable Sub testMaskingCnnDim3_SingleExample()
			'Test masking, where mask is along dimension 3

			Dim minibatch As Integer = 1
			Dim depthIn As Integer = 2
			Dim depthOut As Integer = 2
			Dim nOut As Integer = 2
			Dim height As Integer = 3
			Dim width As Integer = 6

			Dim poolingTypes() As PoolingType = {PoolingType.SUM, PoolingType.AVG, PoolingType.MAX, PoolingType.PNORM}

			For Each pt As PoolingType In poolingTypes
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).convolutionMode(ConvolutionMode.Same).seed(12345L).list().layer(0, (New ConvolutionLayer.Builder()).nIn(depthIn).nOut(depthOut).kernelSize(height, 2).stride(height, 1).activation(Activation.TANH).build()).layer(1, (New GlobalPoolingLayer.Builder()).poolingType(pt).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(depthOut).nOut(nOut).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim inToBeMasked As INDArray = Nd4j.rand(New Integer() {minibatch, depthIn, height, width})

				'Shape for mask: [minibatch, 1, 1, width]
				Dim maskArray As INDArray = Nd4j.create(New Double() {1, 1, 1, 1, 1, 0}, New Integer(){1, 1, 1, width})

				'Multiply the input by the mask array, to ensure the 0s in the mask correspond to 0s in the input vector
				' as would be the case in practice...
				Nd4j.Executioner.exec(New BroadcastMulOp(inToBeMasked, maskArray, inToBeMasked, 0, 3))


				net.setLayerMaskArrays(maskArray, Nothing)

				Dim outMasked As INDArray = net.output(inToBeMasked)
				net.clearLayerMaskArrays()

				Dim numSteps As Integer = width - 1
				Dim subset As INDArray = inToBeMasked.get(interval(0, 0, True), all(), all(), interval(0, numSteps))
				assertArrayEquals(New Long() {1, depthIn, height, 5}, subset.shape())

				Dim outSubset As INDArray = net.output(subset)
				Dim outMaskedSubset As INDArray = outMasked.getRow(0)

				assertEquals(outSubset, outMaskedSubset)

				'Finally: check gradient calc for exceptions
				net.setLayerMaskArrays(maskArray, Nothing)
				net.Input = inToBeMasked
				Dim labels As INDArray = Nd4j.create(New Double() {0, 1}, New Long(){1, 2})
				net.Labels = labels

				net.computeGradientAndScore()
			Next pt
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMaskingCnnDim2_SingleExample()
		Public Overridable Sub testMaskingCnnDim2_SingleExample()
			'Test masking, where mask is along dimension 2

			Dim minibatch As Integer = 1
			Dim depthIn As Integer = 2
			Dim depthOut As Integer = 2
			Dim nOut As Integer = 2
			Dim height As Integer = 6
			Dim width As Integer = 3

			Dim poolingTypes() As PoolingType = {PoolingType.SUM, PoolingType.AVG, PoolingType.MAX, PoolingType.PNORM}

			For Each pt As PoolingType In poolingTypes
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).convolutionMode(ConvolutionMode.Same).seed(12345L).list().layer(0, (New ConvolutionLayer.Builder()).nIn(depthIn).nOut(depthOut).kernelSize(2, width).stride(1, width).activation(Activation.TANH).build()).layer(1, (New GlobalPoolingLayer.Builder()).poolingType(pt).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(depthOut).nOut(nOut).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim inToBeMasked As INDArray = Nd4j.rand(New Integer() {minibatch, depthIn, height, width})

				'Shape for mask: [minibatch, width]
				Dim maskArray As INDArray = Nd4j.create(New Double() {1, 1, 1, 1, 1, 0}, New Integer(){1, 1, height, 1})

				'Multiply the input by the mask array, to ensure the 0s in the mask correspond to 0s in the input vector
				' as would be the case in practice...
				Nd4j.Executioner.exec(New BroadcastMulOp(inToBeMasked, maskArray, inToBeMasked, 0, 2))


				net.setLayerMaskArrays(maskArray, Nothing)

				Dim outMasked As INDArray = net.output(inToBeMasked)
				net.clearLayerMaskArrays()

				Dim numSteps As Integer = height - 1
				Dim subset As INDArray = inToBeMasked.get(interval(0, 0, True), all(), interval(0, numSteps), all())
				assertArrayEquals(New Long() {1, depthIn, 5, width}, subset.shape())

				Dim outSubset As INDArray = net.output(subset)
				Dim outMaskedSubset As INDArray = outMasked.getRow(0)

				assertEquals(outSubset, outMaskedSubset)

				'Finally: check gradient calc for exceptions
				net.setLayerMaskArrays(maskArray, Nothing)
				net.Input = inToBeMasked
				Dim labels As INDArray = Nd4j.create(New Double() {0, 1}, New Long(){1, 2})
				net.Labels = labels

				net.computeGradientAndScore()
			Next pt
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMaskingCnnDim3()
		Public Overridable Sub testMaskingCnnDim3()
			'Test masking, where mask is along dimension 3

			Dim minibatch As Integer = 3
			Dim depthIn As Integer = 3
			Dim depthOut As Integer = 4
			Dim nOut As Integer = 5
			Dim height As Integer = 3
			Dim width As Integer = 6

			Dim poolingTypes() As PoolingType = {PoolingType.SUM, PoolingType.AVG, PoolingType.MAX, PoolingType.PNORM}

			For Each pt As PoolingType In poolingTypes
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).convolutionMode(ConvolutionMode.Same).seed(12345L).list().layer(0, (New ConvolutionLayer.Builder()).nIn(depthIn).nOut(depthOut).kernelSize(height, 2).stride(height, 1).activation(Activation.TANH).build()).layer(1, (New GlobalPoolingLayer.Builder()).poolingType(pt).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(depthOut).nOut(nOut).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim inToBeMasked As INDArray = Nd4j.rand(New Integer() {minibatch, depthIn, height, width})

				'Shape for mask: [minibatch, width]
				Dim maskArray As INDArray = Nd4j.create(New Double()() {
					New Double() {1, 1, 1, 1, 1, 1},
					New Double() {1, 1, 1, 1, 1, 0},
					New Double() {1, 1, 1, 1, 0, 0}
				}).reshape("c"c, minibatch, 1, 1, width)

				'Multiply the input by the mask array, to ensure the 0s in the mask correspond to 0s in the input vector
				' as would be the case in practice...
				Nd4j.Executioner.exec(New BroadcastMulOp(inToBeMasked, maskArray, inToBeMasked, 0, 3))


				net.setLayerMaskArrays(maskArray, Nothing)

				Dim outMasked As INDArray = net.output(inToBeMasked)
				net.clearLayerMaskArrays()

				For i As Integer = 0 To minibatch - 1
					Dim numSteps As Integer = width - i
					Dim subset As INDArray = inToBeMasked.get(interval(i, i, True), all(), all(), interval(0, numSteps))
					assertArrayEquals(New Long() {1, depthIn, height, width - i}, subset.shape())

					Dim outSubset As INDArray = net.output(subset)
					Dim outMaskedSubset As INDArray = outMasked.getRow(i, True)

					assertEquals(outSubset, outMaskedSubset, "minibatch: " & i)
				Next i
			Next pt
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMaskingCnnDim2()
		Public Overridable Sub testMaskingCnnDim2()
			'Test masking, where mask is along dimension 2

			Dim minibatch As Integer = 3
			Dim depthIn As Integer = 3
			Dim depthOut As Integer = 4
			Dim nOut As Integer = 5
			Dim height As Integer = 5
			Dim width As Integer = 4

			Dim poolingTypes() As PoolingType = {PoolingType.SUM, PoolingType.AVG, PoolingType.MAX, PoolingType.PNORM}

			For Each pt As PoolingType In poolingTypes
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).convolutionMode(ConvolutionMode.Same).seed(12345L).list().layer(0, (New ConvolutionLayer.Builder()).nIn(depthIn).nOut(depthOut).kernelSize(2, width).stride(1, width).activation(Activation.TANH).build()).layer(1, (New GlobalPoolingLayer.Builder()).poolingType(pt).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(depthOut).nOut(nOut).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim inToBeMasked As INDArray = Nd4j.rand(New Integer() {minibatch, depthIn, height, width})

				'Shape for mask: [minibatch, 1, height, 1] -> broadcast
				Dim maskArray As INDArray = Nd4j.create(New Double()() {
					New Double() {1, 1, 1, 1, 1},
					New Double() {1, 1, 1, 1, 0},
					New Double() {1, 1, 1, 0, 0}
				}).reshape("c"c, minibatch, 1, height, 1)

				'Multiply the input by the mask array, to ensure the 0s in the mask correspond to 0s in the input vector
				' as would be the case in practice...
				Nd4j.Executioner.exec(New BroadcastMulOp(inToBeMasked, maskArray, inToBeMasked, 0, 2))


				net.setLayerMaskArrays(maskArray, Nothing)

				Dim outMasked As INDArray = net.output(inToBeMasked)
				net.clearLayerMaskArrays()

				For i As Integer = 0 To minibatch - 1
					Dim numSteps As Integer = height - i
					Dim subset As INDArray = inToBeMasked.get(interval(i, i, True), all(), interval(0, numSteps), all())
					assertArrayEquals(New Long() {1, depthIn, height - i, width}, subset.shape())

					Dim outSubset As INDArray = net.output(subset)
					Dim outMaskedSubset As INDArray = outMasked.getRow(i, True)

					assertEquals(outSubset, outMaskedSubset, "minibatch: " & i)
				Next i
			Next pt
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMaskingCnnDim23()
		Public Overridable Sub testMaskingCnnDim23()
			'Test masking, where mask is along dimension 2 AND 3
			'For example, input images of 2 different sizes

			Dim minibatch As Integer = 2
			Dim depthIn As Integer = 2
			Dim depthOut As Integer = 4
			Dim nOut As Integer = 5
			Dim height As Integer = 5
			Dim width As Integer = 4

			Dim poolingTypes() As PoolingType = {PoolingType.SUM, PoolingType.AVG, PoolingType.MAX, PoolingType.PNORM}

			For Each pt As PoolingType In poolingTypes
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).convolutionMode(ConvolutionMode.Same).seed(12345L).list().layer(0, (New ConvolutionLayer.Builder()).nIn(depthIn).nOut(depthOut).kernelSize(2, 2).stride(1, 1).activation(Activation.TANH).build()).layer(1, (New GlobalPoolingLayer.Builder()).poolingType(pt).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(depthOut).nOut(nOut).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim inToBeMasked As INDArray = Nd4j.rand(New Integer() {minibatch, depthIn, height, width})

				'Second example in minibatch: size [3,2]
				inToBeMasked.get(point(1), all(), interval(3,height), all()).assign(0)
				inToBeMasked.get(point(1), all(), all(), interval(2,width)).assign(0)

				'Shape for mask: [minibatch, 1, height, 1] -> broadcast
				Dim maskArray As INDArray = Nd4j.create(minibatch, 1, height, width)
				maskArray.get(point(0), all(), all(), all()).assign(1)
				maskArray.get(point(1), all(), interval(0,3), interval(0,2)).assign(1)

				net.setLayerMaskArrays(maskArray, Nothing)

				Dim outMasked As INDArray = net.output(inToBeMasked)
				net.clearLayerMaskArrays()

				net.setLayerMaskArrays(maskArray, Nothing)

				For i As Integer = 0 To minibatch - 1
					Dim subset As INDArray
					If i = 0 Then
						subset = inToBeMasked.get(interval(i, i, True), all(), all(), all())
					Else
						subset = inToBeMasked.get(interval(i, i, True), all(), interval(0,3), interval(0,2))
					End If

					net.clear()
					net.clearLayerMaskArrays()
					Dim outSubset As INDArray = net.output(subset)
					Dim outMaskedSubset As INDArray = outMasked.getRow(i,True)

					assertEquals(outSubset, outMaskedSubset, "minibatch: " & i & ", " & pt)
				Next i
			Next pt
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMaskLayerDataTypes()
		Public Overridable Sub testMaskLayerDataTypes()

			For Each dt As DataType In New DataType(){DataType.FLOAT16, DataType.BFLOAT16, DataType.FLOAT, DataType.DOUBLE, DataType.INT8, DataType.INT16, DataType.INT32, DataType.INT64, DataType.UINT8, DataType.UINT16, DataType.UINT32, DataType.UINT64}
				Dim mask As INDArray = Nd4j.rand(DataType.FLOAT, 2, 10).addi(0.3).castTo(dt)

				For Each networkDtype As DataType In New DataType(){DataType.FLOAT16, DataType.BFLOAT16, DataType.FLOAT, DataType.DOUBLE}

					Dim [in] As INDArray = Nd4j.rand(networkDtype, 2, 5, 10)
					Dim label1 As INDArray = Nd4j.rand(networkDtype, 2, 5)
					Dim label2 As INDArray = Nd4j.rand(networkDtype, 2, 5, 10)

					For Each pt As PoolingType In System.Enum.GetValues(GetType(PoolingType))
						'System.out.println("Net: " + networkDtype + ", mask: " + dt + ", pt=" + pt);

						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(New GlobalPoolingLayer(pt)).layer((New OutputLayer.Builder()).nIn(5).nOut(5).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.MSE).build()).build()

						Dim net As New MultiLayerNetwork(conf)
						net.init()

						net.output([in], False, mask, Nothing)
						net.output([in], False, mask, Nothing)


						Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New RnnOutputLayer.Builder()).nIn(5).nOut(5).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.MSE).build()).build()

						Dim net2 As New MultiLayerNetwork(conf2)
						net2.init()

						net2.output([in], False, mask, mask)
						net2.output([in], False, mask, mask)

						net.fit([in], label1, mask, Nothing)
						net2.fit([in], label2, mask, mask)
					Next pt
				Next networkDtype
			Next dt
		End Sub
	End Class

End Namespace