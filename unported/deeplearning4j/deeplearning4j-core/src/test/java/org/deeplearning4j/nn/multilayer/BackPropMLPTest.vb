Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports NdIndexIterator = org.nd4j.linalg.api.iter.NdIndexIterator
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports SigmoidDerivative = org.nd4j.linalg.api.ops.impl.transforms.strict.SigmoidDerivative
Imports TanhDerivative = org.nd4j.linalg.api.ops.impl.transforms.strict.TanhDerivative
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.fail
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
Namespace org.deeplearning4j.nn.multilayer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Back Prop MLP Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class BackPropMLPTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class BackPropMLPTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test MLP Trivial") void testMLPTrivial()
		Friend Overridable Sub testMLPTrivial()
			' Simplest possible case: 1 hidden layer, 1 hidden neuron, batch size of 1.
			Dim network As New MultiLayerNetwork(getIrisMLPSimpleConfig(New Integer() { 1 }, Activation.SIGMOID))
			network.setListeners(New ScoreIterationListener(1))
			network.init()
			Dim iter As DataSetIterator = New IrisDataSetIterator(1, 10)
			Do While iter.MoveNext()
				network.fit(iter.Current)
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test MLP") void testMLP()
		Friend Overridable Sub testMLP()
			' Simple mini-batch test with multiple hidden layers
			Dim conf As MultiLayerConfiguration = getIrisMLPSimpleConfig(New Integer() { 5, 4, 3 }, Activation.SIGMOID)
			' System.out.println(conf);
			Dim network As New MultiLayerNetwork(conf)
			network.init()
			Dim iter As DataSetIterator = New IrisDataSetIterator(10, 100)
			Do While iter.MoveNext()
				network.fit(iter.Current)
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test MLP 2") void testMLP2()
		Friend Overridable Sub testMLP2()
			' Simple mini-batch test with multiple hidden layers
			Dim conf As MultiLayerConfiguration = getIrisMLPSimpleConfig(New Integer() { 5, 15, 3 }, Activation.TANH)
			' System.out.println(conf);
			Dim network As New MultiLayerNetwork(conf)
			network.init()
			Dim iter As DataSetIterator = New IrisDataSetIterator(12, 120)
			Do While iter.MoveNext()
				network.fit(iter.Current)
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Single Example Weight Updates") void testSingleExampleWeightUpdates()
		Friend Overridable Sub testSingleExampleWeightUpdates()
			' Simplest possible case: 1 hidden layer, 1 hidden neuron, batch size of 1.
			' Manually calculate weight updates (entirely outside of DL4J and ND4J)
			' and compare expected and actual weights after backprop
			Dim iris As DataSetIterator = New IrisDataSetIterator(1, 10)
			Dim network As New MultiLayerNetwork(getIrisMLPSimpleConfig(New Integer() { 1 }, Activation.SIGMOID))
			network.init()
			Dim layers() As Layer = network.Layers
			Const printCalculations As Boolean = False
			Do While iris.MoveNext()
				Dim data As DataSet = iris.Current
				Dim x As INDArray = data.Features
				Dim y As INDArray = data.Labels
				Dim xFloat() As Single = asFloat(x)
				Dim yFloat() As Single = asFloat(y)
				' Do forward pass:
				' Hidden layer
				Dim l1Weights As INDArray = layers(0).getParam(DefaultParamInitializer.WEIGHT_KEY).dup()
				' Output layer
				Dim l2Weights As INDArray = layers(1).getParam(DefaultParamInitializer.WEIGHT_KEY).dup()
				Dim l1Bias As INDArray = layers(0).getParam(DefaultParamInitializer.BIAS_KEY).dup()
				Dim l2Bias As INDArray = layers(1).getParam(DefaultParamInitializer.BIAS_KEY).dup()
				Dim l1WeightsFloat() As Single = asFloat(l1Weights)
				Dim l2WeightsFloat() As Single = asFloat(l2Weights)
				Dim l1BiasFloat As Single = l1Bias.getFloat(0)
				Dim l2BiasFloatArray() As Single = asFloat(l2Bias)
				' z=w*x+b
				Dim hiddenUnitPreSigmoid As Single = dotProduct(l1WeightsFloat, xFloat) + l1BiasFloat
				' a=sigma(z)
				Dim hiddenUnitPostSigmoid As Single = sigmoid(hiddenUnitPreSigmoid)
				Dim outputPreSoftmax(2) As Single
				' Normally a matrix multiplication here, but only one hidden unit in this trivial example
				For i As Integer = 0 To 2
					outputPreSoftmax(i) = hiddenUnitPostSigmoid * l2WeightsFloat(i) + l2BiasFloatArray(i)
				Next i
				Dim outputPostSoftmax() As Single = softmax(outputPreSoftmax)
				' Do backward pass:
				' out-labels
				Dim deltaOut() As Single = vectorDifference(outputPostSoftmax, yFloat)
				' deltaHidden = sigmaPrime(hiddenUnitZ) * sum_k (w_jk * \delta_k); here, only one j
				Dim deltaHidden As Single = 0.0f
				For i As Integer = 0 To 2
					deltaHidden += l2WeightsFloat(i) * deltaOut(i)
				Next i
				deltaHidden *= derivOfSigmoid(hiddenUnitPreSigmoid)
				' Calculate weight/bias updates:
				' dL/dW = delta * (activation of prev. layer)
				' dL/db = delta
				Dim dLdwOut(2) As Single
				For i As Integer = 0 To dLdwOut.Length - 1
					dLdwOut(i) = deltaOut(i) * hiddenUnitPostSigmoid
				Next i
				Dim dLdwHidden(3) As Single
				For i As Integer = 0 To dLdwHidden.Length - 1
					dLdwHidden(i) = deltaHidden * xFloat(i)
				Next i
				Dim dLdbOut() As Single = deltaOut
				Dim dLdbHidden As Single = deltaHidden
				If printCalculations Then
					Console.WriteLine("deltaOut = " & Arrays.toString(deltaOut))
					Console.WriteLine("deltaHidden = " & deltaHidden)
					Console.WriteLine("dLdwOut = " & Arrays.toString(dLdwOut))
					Console.WriteLine("dLdbOut = " & Arrays.toString(dLdbOut))
					Console.WriteLine("dLdwHidden = " & Arrays.toString(dLdwHidden))
					Console.WriteLine("dLdbHidden = " & dLdbHidden)
				End If
				' Calculate new parameters:
				' w_i = w_i - (learningRate)/(batchSize) * sum_j (dL_j/dw_i)
				' b_i = b_i - (learningRate)/(batchSize) * sum_j (dL_j/db_i)
				' Which for batch size of one (here) is simply:
				' w_i = w_i - learningRate * dL/dW
				' b_i = b_i - learningRate * dL/db
				Dim expectedL1WeightsAfter(3) As Single
				Dim expectedL2WeightsAfter(2) As Single
				Dim expectedL1BiasAfter As Single = l1BiasFloat - 0.1f * dLdbHidden
				Dim expectedL2BiasAfter(2) As Single
				For i As Integer = 0 To 3
					expectedL1WeightsAfter(i) = l1WeightsFloat(i) - 0.1f * dLdwHidden(i)
				Next i
				For i As Integer = 0 To 2
					expectedL2WeightsAfter(i) = l2WeightsFloat(i) - 0.1f * dLdwOut(i)
				Next i
				For i As Integer = 0 To 2
					expectedL2BiasAfter(i) = l2BiasFloatArray(i) - 0.1f * dLdbOut(i)
				Next i
				' Finally, do back-prop on network, and compare parameters vs. expected parameters
				network.fit(data)
	'              INDArray l1WeightsAfter = layers[0].getParam(DefaultParamInitializer.WEIGHT_KEY).dup();	//Hidden layer
	'            INDArray l2WeightsAfter = layers[1].getParam(DefaultParamInitializer.WEIGHT_KEY).dup();	//Output layer
	'            INDArray l1BiasAfter = layers[0].getParam(DefaultParamInitializer.BIAS_KEY).dup();
	'            INDArray l2BiasAfter = layers[1].getParam(DefaultParamInitializer.BIAS_KEY).dup();
	'            float[] l1WeightsFloatAfter = asFloat(l1WeightsAfter);
	'            float[] l2WeightsFloatAfter = asFloat(l2WeightsAfter);
	'            float l1BiasFloatAfter = l1BiasAfter.getFloat(0);
	'            float[] l2BiasFloatAfter = asFloat(l2BiasAfter);
	'            
	'            if( printCalculations) {
	'                System.out.println("Expected L1 weights = " + Arrays.toString(expectedL1WeightsAfter));
	'                System.out.println("Actual L1 weights = " + Arrays.toString(asFloat(l1WeightsAfter)));
	'                System.out.println("Expected L2 weights = " + Arrays.toString(expectedL2WeightsAfter));
	'                System.out.println("Actual L2 weights = " + Arrays.toString(asFloat(l2WeightsAfter)));
	'                System.out.println("Expected L1 bias = " + expectedL1BiasAfter);
	'                System.out.println("Actual L1 bias = " + Arrays.toString(asFloat(l1BiasAfter)));
	'                System.out.println("Expected L2 bias = " + Arrays.toString(expectedL2BiasAfter));
	'                System.out.println("Actual L2 bias = " + Arrays.toString(asFloat(l2BiasAfter)));
	'            }
	'            
	'            
	'            float eps = 1e-4f;
	'            assertArrayEquals(l1WeightsFloatAfter,expectedL1WeightsAfter,eps);
	'            assertArrayEquals(l2WeightsFloatAfter,expectedL2WeightsAfter,eps);
	'            assertEquals(l1BiasFloatAfter,expectedL1BiasAfter,eps);
	'            assertArrayEquals(l2BiasFloatAfter,expectedL2BiasAfter,eps);
	'            
				' System.out.println("\n\n--------------");
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test MLP Gradient Calculation") void testMLPGradientCalculation()
		Friend Overridable Sub testMLPGradientCalculation()
			testIrisMiniBatchGradients(1, New Integer() { 1 }, Activation.SIGMOID)
			testIrisMiniBatchGradients(1, New Integer() { 5 }, Activation.SIGMOID)
			testIrisMiniBatchGradients(12, New Integer() { 15, 25, 10 }, Activation.SIGMOID)
			testIrisMiniBatchGradients(50, New Integer() { 10, 50, 200, 50, 10 }, Activation.TANH)
			testIrisMiniBatchGradients(150, New Integer() { 30, 50, 20 }, Activation.TANH)
		End Sub

		Private Shared Sub testIrisMiniBatchGradients(ByVal miniBatchSize As Integer, ByVal hiddenLayerSizes() As Integer, ByVal activationFunction As Activation)
			Dim totalExamples As Integer = 10 * miniBatchSize
			If totalExamples > 150 Then
				totalExamples = miniBatchSize * (150 \ miniBatchSize)
			End If
			If miniBatchSize > 150 Then
				fail()
			End If
			Dim iris As DataSetIterator = New IrisDataSetIterator(miniBatchSize, totalExamples)
			Dim network As New MultiLayerNetwork(getIrisMLPSimpleConfig(hiddenLayerSizes, Activation.SIGMOID))
			network.init()
			Dim layers() As Layer = network.Layers
			Dim nLayers As Integer = layers.Length
			Do While iris.MoveNext()
				Dim data As DataSet = iris.Current
				Dim x As INDArray = data.Features
				Dim y As INDArray = data.Labels
				' Do forward pass:
				Dim layerWeights(nLayers - 1) As INDArray
				Dim layerBiases(nLayers - 1) As INDArray
				For i As Integer = 0 To nLayers - 1
					layerWeights(i) = layers(i).getParam(DefaultParamInitializer.WEIGHT_KEY).dup()
					layerBiases(i) = layers(i).getParam(DefaultParamInitializer.BIAS_KEY).dup()
				Next i
				Dim layerZs(nLayers - 1) As INDArray
				Dim layerActivations(nLayers - 1) As INDArray
				For i As Integer = 0 To nLayers - 1
					Dim layerInput As INDArray = (If(i = 0, x, layerActivations(i - 1)))
					layerZs(i) = layerInput.castTo(layerWeights(i).dataType()).mmul(layerWeights(i)).addiRowVector(layerBiases(i))
					layerActivations(i) = (If(i = nLayers - 1, doSoftmax(layerZs(i).dup()), doSigmoid(layerZs(i).dup())))
				Next i
				' Do backward pass:
				Dim deltas(nLayers - 1) As INDArray
				' Out - labels; shape=[miniBatchSize,nOut];
				deltas(nLayers - 1) = layerActivations(nLayers - 1).sub(y.castTo(layerActivations(nLayers - 1).dataType()))
				assertArrayEquals(deltas(nLayers - 1).shape(), New Long() { miniBatchSize, 3 })
				For i As Integer = nLayers - 2 To 0 Step -1
					Dim sigmaPrimeOfZ As INDArray
					sigmaPrimeOfZ = doSigmoidDerivative(layerZs(i))
					Dim epsilon As INDArray = layerWeights(i + 1).mmul(deltas(i + 1).transpose()).transpose()
					deltas(i) = epsilon.mul(sigmaPrimeOfZ)
					assertArrayEquals(deltas(i).shape(), New Long() { miniBatchSize, hiddenLayerSizes(i) })
				Next i
				Dim dLdw(nLayers - 1) As INDArray
				Dim dLdb(nLayers - 1) As INDArray
				For i As Integer = 0 To nLayers - 1
					Dim prevActivations As INDArray = (If(i = 0, x, layerActivations(i - 1)))
					' Raw gradients, so not yet divided by mini-batch size (division is done in BaseUpdater)
					' Shape: [nIn, nOut]
					dLdw(i) = deltas(i).transpose().castTo(prevActivations.dataType()).mmul(prevActivations).transpose()
					' Shape: [1,nOut]
					dLdb(i) = deltas(i).sum(True, 0)
					Dim nIn As Integer = (If(i = 0, 4, hiddenLayerSizes(i - 1)))
					Dim nOut As Integer = (If(i < nLayers - 1, hiddenLayerSizes(i), 3))
					assertArrayEquals(dLdw(i).shape(), New Long() { nIn, nOut })
					assertArrayEquals(dLdb(i).shape(), New Long() { 1, nOut })
				Next i
				' Calculate and get gradient, compare to expected
				network.Input = x
				network.Labels = y
				network.computeGradientAndScore()
				Dim gradient As Gradient = network.gradientAndScore().First
				Dim eps As Single = 1e-4f
				For i As Integer = 0 To hiddenLayerSizes.Length - 1
					Dim wKey As String = i & "_" & DefaultParamInitializer.WEIGHT_KEY
					Dim bKey As String = i & "_" & DefaultParamInitializer.BIAS_KEY
					Dim wGrad As INDArray = gradient.getGradientFor(wKey)
					Dim bGrad As INDArray = gradient.getGradientFor(bKey)
					Dim wGradf() As Single = asFloat(wGrad)
					Dim bGradf() As Single = asFloat(bGrad)
					Dim expWGradf() As Single = asFloat(dLdw(i))
					Dim expBGradf() As Single = asFloat(dLdb(i))
					assertArrayEquals(wGradf, expWGradf, eps)
					assertArrayEquals(bGradf, expBGradf, eps)
				Next i
			Loop
		End Sub

		''' <summary>
		''' Very simple back-prop config set up for Iris.
		''' Learning Rate = 0.1
		''' No regularization, no Adagrad, no momentum etc. One iteration.
		''' </summary>
		Private Shared Function getIrisMLPSimpleConfig(ByVal hiddenLayerSizes() As Integer, ByVal activationFunction As Activation) As MultiLayerConfiguration
			Dim lb As NeuralNetConfiguration.ListBuilder = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).seed(12345L).list()
			For i As Integer = 0 To hiddenLayerSizes.Length - 1
				Dim nIn As Integer = (If(i = 0, 4, hiddenLayerSizes(i - 1)))
				lb.layer(i, (New DenseLayer.Builder()).nIn(nIn).nOut(hiddenLayerSizes(i)).weightInit(WeightInit.XAVIER).activation(activationFunction).build())
			Next i
			lb.layer(hiddenLayerSizes.Length, (New OutputLayer.Builder(LossFunction.MCXENT)).nIn(hiddenLayerSizes(hiddenLayerSizes.Length - 1)).nOut(3).weightInit(WeightInit.XAVIER).activation(If(activationFunction.Equals(Activation.IDENTITY), Activation.IDENTITY, Activation.SOFTMAX)).build())
			Return lb.build()
		End Function

		Public Shared Function asFloat(ByVal arr As INDArray) As Single()
			Dim len As Long = arr.length()
			If len > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Dim f(CInt(len) - 1) As Single
			Dim iterator As New NdIndexIterator("c"c, arr.shape())
			For i As Integer = 0 To len - 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				f(i) = arr.getFloat(iterator.next())
			Next i
			Return f
		End Function

		Public Shared Function dotProduct(ByVal x() As Single, ByVal y() As Single) As Single
			Dim sum As Single = 0.0f
			For i As Integer = 0 To x.Length - 1
				sum += x(i) * y(i)
			Next i
			Return sum
		End Function

		Public Shared Function sigmoid(ByVal [in] As Single) As Single
			Return CSng(1.0 / (1.0 + Math.Exp(-[in])))
		End Function

		Public Shared Function sigmoid(ByVal [in]() As Single) As Single()
			Dim [out]([in].Length - 1) As Single
			For i As Integer = 0 To [in].Length - 1
				[out](i) = sigmoid([in](i))
			Next i
			Return [out]
		End Function

		Public Shared Function derivOfSigmoid(ByVal [in] As Single) As Single
			' float v = (float)( Math.exp(in) / Math.pow(1+Math.exp(in),2.0) );
			Dim v As Single = [in] * (1 - [in])
			Return v
		End Function

		Public Shared Function derivOfSigmoid(ByVal [in]() As Single) As Single()
			Dim [out]([in].Length - 1) As Single
			For i As Integer = 0 To [in].Length - 1
				[out](i) = derivOfSigmoid([in](i))
			Next i
			Return [out]
		End Function

		Public Shared Function softmax(ByVal [in]() As Single) As Single()
			Dim [out]([in].Length - 1) As Single
			Dim sumExp As Single = 0.0f
			For i As Integer = 0 To [in].Length - 1
				sumExp += Math.Exp([in](i))
			Next i
			For i As Integer = 0 To [in].Length - 1
				[out](i) = CSng(Math.Exp([in](i))) / sumExp
			Next i
			Return [out]
		End Function

		Public Shared Function vectorDifference(ByVal x() As Single, ByVal y() As Single) As Single()
			Dim [out](x.Length - 1) As Single
			For i As Integer = 0 To x.Length - 1
				[out](i) = x(i) - y(i)
			Next i
			Return [out]
		End Function

		Public Shared Function doSoftmax(ByVal input As INDArray) As INDArray
			Return Transforms.softmax(input, True)
		End Function

		Public Shared Function doSigmoid(ByVal input As INDArray) As INDArray
			Return Transforms.sigmoid(input, True)
		End Function

		Public Shared Function doSigmoidDerivative(ByVal input As INDArray) As INDArray
			Return Nd4j.Executioner.exec(New SigmoidDerivative(input.dup()))
		End Function
	End Class

End Namespace