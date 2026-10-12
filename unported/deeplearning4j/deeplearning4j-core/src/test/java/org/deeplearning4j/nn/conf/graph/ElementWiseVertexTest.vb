Imports System
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports ActivationLayer = org.deeplearning4j.nn.conf.layers.ActivationLayer
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Assertions = org.junit.jupiter.api.Assertions
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports ActivationSigmoid = org.nd4j.linalg.activations.impl.ActivationSigmoid
Imports ActivationTanH = org.nd4j.linalg.activations.impl.ActivationTanH
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports UniformDistribution = org.nd4j.linalg.api.rng.distribution.impl.UniformDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
import static org.junit.jupiter.api.Assertions.assertArrayEquals
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
Namespace org.deeplearning4j.nn.conf.graph

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Element Wise Vertex Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class ElementWiseVertexTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class ElementWiseVertexTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Element Wise Vertex Num Params") void testElementWiseVertexNumParams()
		Friend Overridable Sub testElementWiseVertexNumParams()
	'        
	'         * https://github.com/eclipse/deeplearning4j/pull/3514#issuecomment-307754386
	'         * from @agibsonccc: check for the basics: like 0 numParams
	'         
			Dim ops() As ElementWiseVertex.Op = { ElementWiseVertex.Op.Add, ElementWiseVertex.Op.Subtract, ElementWiseVertex.Op.Product }
			For Each op As ElementWiseVertex.Op In ops
				Dim ewv As New ElementWiseVertex(op)
				Assertions.assertEquals(0, ewv.numParams(True))
				Assertions.assertEquals(0, ewv.numParams(False))
			Next op
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Element Wise Vertex Forward Add") void testElementWiseVertexForwardAdd()
		Friend Overridable Sub testElementWiseVertexForwardAdd()
			Dim batchsz As Integer = 24
			Dim featuresz As Integer = 17
			Dim cgc As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("input1", "input2", "input3").addLayer("denselayer", (New DenseLayer.Builder()).nIn(featuresz).nOut(1).activation(Activation.IDENTITY).build(), "input1").addVertex("elementwiseAdd", New ElementWiseVertex(ElementWiseVertex.Op.Add), "input1", "input2", "input3").addLayer("Add", (New ActivationLayer.Builder()).activation(Activation.IDENTITY).build(), "elementwiseAdd").setOutputs("Add", "denselayer").build()
			Dim cg As New ComputationGraph(cgc)
			cg.init()
			Dim input1 As INDArray = Nd4j.rand(batchsz, featuresz)
			Dim input2 As INDArray = Nd4j.rand(batchsz, featuresz)
			Dim input3 As INDArray = Nd4j.rand(batchsz, featuresz)
			Dim target As INDArray = input1.dup().addi(input2).addi(input3)
			Dim output As INDArray = cg.output(input1, input2, input3)(0)
			Dim squared As INDArray = output.sub(target.castTo(output.dataType()))
			Dim rms As Double = squared.mul(squared).sumNumber().doubleValue()
			Assertions.assertEquals(0.0, rms, Me.epsilon)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Element Wise Vertex Forward Product") void testElementWiseVertexForwardProduct()
		Friend Overridable Sub testElementWiseVertexForwardProduct()
			Dim batchsz As Integer = 24
			Dim featuresz As Integer = 17
			Dim cgc As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("input1", "input2", "input3").addLayer("denselayer", (New DenseLayer.Builder()).nIn(featuresz).nOut(1).activation(Activation.IDENTITY).build(), "input1").addVertex("elementwiseProduct", New ElementWiseVertex(ElementWiseVertex.Op.Product), "input1", "input2", "input3").addLayer("Product", (New ActivationLayer.Builder()).activation(Activation.IDENTITY).build(), "elementwiseProduct").setOutputs("Product", "denselayer").build()
			Dim cg As New ComputationGraph(cgc)
			cg.init()
			Dim input1 As INDArray = Nd4j.rand(batchsz, featuresz)
			Dim input2 As INDArray = Nd4j.rand(batchsz, featuresz)
			Dim input3 As INDArray = Nd4j.rand(batchsz, featuresz)
			Dim target As INDArray = input1.dup().muli(input2).muli(input3)
			Dim output As INDArray = cg.output(input1, input2, input3)(0)
			Dim squared As INDArray = output.sub(target.castTo(output.dataType()))
			Dim rms As Double = squared.mul(squared).sumNumber().doubleValue()
			Assertions.assertEquals(0.0, rms, Me.epsilon)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Element Wise Vertex Forward Subtract") void testElementWiseVertexForwardSubtract()
		Friend Overridable Sub testElementWiseVertexForwardSubtract()
			Dim batchsz As Integer = 24
			Dim featuresz As Integer = 17
			Dim cgc As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("input1", "input2").addLayer("denselayer", (New DenseLayer.Builder()).nIn(featuresz).nOut(1).activation(Activation.IDENTITY).build(), "input1").addVertex("elementwiseSubtract", New ElementWiseVertex(ElementWiseVertex.Op.Subtract), "input1", "input2").addLayer("Subtract", (New ActivationLayer.Builder()).activation(Activation.IDENTITY).build(), "elementwiseSubtract").setOutputs("Subtract", "denselayer").build()
			Dim cg As New ComputationGraph(cgc)
			cg.init()
			Dim input1 As INDArray = Nd4j.rand(batchsz, featuresz)
			Dim input2 As INDArray = Nd4j.rand(batchsz, featuresz)
			Dim target As INDArray = input1.dup().subi(input2)
			Dim output As INDArray = cg.output(input1, input2)(0)
			Dim squared As INDArray = output.sub(target)
			Dim rms As Double = Math.Sqrt(squared.mul(squared).sumNumber().doubleValue())
			Assertions.assertEquals(0.0, rms, Me.epsilon)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Element Wise Vertex Full Add") void testElementWiseVertexFullAdd()
		Friend Overridable Sub testElementWiseVertexFullAdd()
			Dim batchsz As Integer = 24
			Dim featuresz As Integer = 17
			Dim midsz As Integer = 13
			Dim outputsz As Integer = 11
			Dim cgc As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).dataType(DataType.DOUBLE).biasInit(0.0).updater(New Sgd()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).graphBuilder().addInputs("input1", "input2", "input3").addLayer("dense1", (New DenseLayer.Builder()).nIn(featuresz).nOut(midsz).activation(New ActivationTanH()).build(), "input1").addLayer("dense2", (New DenseLayer.Builder()).nIn(featuresz).nOut(midsz).activation(New ActivationTanH()).build(), "input2").addLayer("dense3", (New DenseLayer.Builder()).nIn(featuresz).nOut(midsz).activation(New ActivationTanH()).build(), "input3").addVertex("elementwiseAdd", New ElementWiseVertex(ElementWiseVertex.Op.Add), "dense1", "dense2", "dense3").addLayer("output", (New OutputLayer.Builder()).nIn(midsz).nOut(outputsz).activation(New ActivationSigmoid()).lossFunction(LossFunction.MSE).build(), "elementwiseAdd").setOutputs("output").build()
			Dim cg As New ComputationGraph(cgc)
			cg.init()
			Dim input1 As INDArray = Nd4j.rand(New Integer() { batchsz, featuresz }, New UniformDistribution(-1, 1))
			Dim input2 As INDArray = Nd4j.rand(New Integer() { batchsz, featuresz }, New UniformDistribution(-1, 1))
			Dim input3 As INDArray = Nd4j.rand(New Integer() { batchsz, featuresz }, New UniformDistribution(-1, 1))
			Dim target As INDArray = nullsafe(Nd4j.rand(New Integer() { batchsz, outputsz }, New UniformDistribution(0, 1)))
			cg.setInputs(input1, input2, input3)
			cg.Labels = target
			cg.computeGradientAndScore()
			' Let's figure out what our params are now.
			Dim params As IDictionary(Of String, INDArray) = cg.paramTable()
			Dim dense1_W As INDArray = nullsafe(params("dense1_W"))
			Dim dense1_b As INDArray = nullsafe(params("dense1_b"))
			Dim dense2_W As INDArray = nullsafe(params("dense2_W"))
			Dim dense2_b As INDArray = nullsafe(params("dense2_b"))
			Dim dense3_W As INDArray = nullsafe(params("dense3_W"))
			Dim dense3_b As INDArray = nullsafe(params("dense3_b"))
			Dim output_W As INDArray = nullsafe(params("output_W"))
			Dim output_b As INDArray = nullsafe(params("output_b"))
			' Now, let's calculate what we expect the output to be.
			Dim mh As INDArray = input1.mmul(dense1_W).addi(dense1_b.repmat(batchsz, 1))
			Dim m As INDArray = (Transforms.tanh(mh))
			Dim nh As INDArray = input2.mmul(dense2_W).addi(dense2_b.repmat(batchsz, 1))
			Dim n As INDArray = (Transforms.tanh(nh))
			Dim oh As INDArray = input3.mmul(dense3_W).addi(dense3_b.repmat(batchsz, 1))
			Dim o As INDArray = (Transforms.tanh(oh))
			Dim middle As INDArray = Nd4j.zeros(batchsz, midsz)
			middle.addi(m).addi(n).addi(o)
			Dim expect As INDArray = Nd4j.zeros(batchsz, outputsz)
			expect.addi(Transforms.sigmoid(middle.mmul(output_W).addi(output_b.repmat(batchsz, 1))))
			Dim output As INDArray = nullsafe(cg.output(input1, input2, input3)(0))
			Assertions.assertEquals(0.0, mse(output, expect), Me.epsilon)
			Dim pgd As Pair(Of Gradient, Double) = cg.gradientAndScore()
			Dim score As Double = pgd.Second
			Assertions.assertEquals(score, mse(output, target), Me.epsilon)
			Dim gradients As IDictionary(Of String, INDArray) = pgd.First.gradientForVariable()
	'        
	'         * So. Let's say we have inputs a, b, c
	'         * mh = a W1 + b1
	'         * m = tanh(mh)
	'         *
	'         * nh = b W2 + b2
	'         * n = tanh(nh)
	'         *
	'         * oh = c W3 + b3
	'         * o = tanh(oh)
	'         *
	'         * s = m+n+o
	'         *
	'         * yh = s W4 + b4
	'         * y = sigmoid(yh)
	'         *
	'         * E = (y-t)^2
	'         * dE/dy = 2 (y-t)
	'         *
	'         * dy/dyh = y * (1-y)
	'         * dE/dyh = 2 * y * (1-y) * (y-t)
	'         *
	'         * dyh/dW4 = s.transpose()
	'         * dyh/db4 = Nd4j.ones(1, batchsz)
	'         * dyh/ds = W4.tranpose()
	'         *
	'         * ds/dm = Nd4j.ones(1, midsz)
	'         *
	'         * dm/dmh = 1-(m^2)
	'         *
	'         * dmh/dW1 = a.transpose()
	'         * dmh/db1 = Nd4j.ones(1, batchsz)
	'         *
	'         
			Dim y As INDArray = output
			Dim s As INDArray = middle
			Dim W4 As INDArray = output_W
			Dim dEdy As INDArray = Nd4j.zeros(target.shape())
			' This should be of size batchsz x outputsz
			dEdy.addi(y).subi(target).muli(2)
			' Why? Because the LossFunction divides by the _element size_ of the output.
			dEdy.divi(target.shape()(1))
			' This is of size batchsz x outputsz
			Dim dydyh As INDArray = y.mul(y.mul(-1).add(1))
			Dim dEdyh As INDArray = dydyh.mul(dEdy)
			Dim dyhdW4 As INDArray = s.transpose()
			Dim dEdW4 As INDArray = nullsafe(dyhdW4.mmul(dEdyh))
			Dim dyhdb4 As INDArray = Nd4j.ones(1, batchsz)
			Dim dEdb4 As INDArray = nullsafe(dyhdb4.mmul(dEdyh))
			Dim dyhds As INDArray = W4.transpose()
			Dim dEds As INDArray = dEdyh.mmul(dyhds)
			Dim dsdm As INDArray = Nd4j.ones(batchsz, midsz)
			Dim dEdm As INDArray = dsdm.mul(dEds)
			Dim dmdmh As INDArray = (m.mul(m)).mul(-1).add(1)
			Dim dEdmh As INDArray = dmdmh.mul(dEdm)
			Dim dmhdW1 As INDArray = input1.transpose()
			Dim dEdW1 As INDArray = nullsafe(dmhdW1.mmul(dEdmh))
			Dim dmhdb1 As INDArray = Nd4j.ones(1, batchsz)
			Dim dEdb1 As INDArray = nullsafe(dmhdb1.mmul(dEdmh))
			Dim dsdn As INDArray = Nd4j.ones(batchsz, midsz)
			Dim dEdn As INDArray = dsdn.mul(dEds)
			Dim dndnh As INDArray = (n.mul(n)).mul(-1).add(1)
			Dim dEdnh As INDArray = dndnh.mul(dEdn)
			Dim dnhdW2 As INDArray = input2.transpose()
			Dim dEdW2 As INDArray = nullsafe(dnhdW2.mmul(dEdnh))
			Dim dnhdb2 As INDArray = Nd4j.ones(1, batchsz)
			Dim dEdb2 As INDArray = nullsafe(dnhdb2.mmul(dEdnh))
			Dim dsdo As INDArray = Nd4j.ones(batchsz, midsz)
			Dim dEdo As INDArray = dsdo.mul(dEds)
			Dim dodoh As INDArray = (o.mul(o)).mul(-1).add(1)
			Dim dEdoh As INDArray = dodoh.mul(dEdo)
			Dim dohdW3 As INDArray = input3.transpose()
			Dim dEdW3 As INDArray = nullsafe(dohdW3.mmul(dEdoh))
			Dim dohdb3 As INDArray = Nd4j.ones(1, batchsz)
			Dim dEdb3 As INDArray = nullsafe(dohdb3.mmul(dEdoh))
			Assertions.assertEquals(0, mse(nullsafe(gradients("output_W")), dEdW4), Me.epsilon)
			Assertions.assertEquals(0, mse(nullsafe(gradients("output_b")), dEdb4), Me.epsilon)
			Assertions.assertEquals(0, mse(nullsafe(gradients("dense1_W")), dEdW1), Me.epsilon)
			Assertions.assertEquals(0, mse(nullsafe(gradients("dense1_b")), dEdb1), Me.epsilon)
			Assertions.assertEquals(0, mse(nullsafe(gradients("dense2_W")), dEdW2), Me.epsilon)
			Assertions.assertEquals(0, mse(nullsafe(gradients("dense2_b")), dEdb2), Me.epsilon)
			Assertions.assertEquals(0, mse(nullsafe(gradients("dense3_W")), dEdW3), Me.epsilon)
			Assertions.assertEquals(0, mse(nullsafe(gradients("dense3_b")), dEdb3), Me.epsilon)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Element Wise Vertex Full Product") void testElementWiseVertexFullProduct()
		Friend Overridable Sub testElementWiseVertexFullProduct()
			Dim batchsz As Integer = 24
			Dim featuresz As Integer = 17
			Dim midsz As Integer = 13
			Dim outputsz As Integer = 11
			Dim cgc As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).dataType(DataType.DOUBLE).biasInit(0.0).updater(New Sgd()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).graphBuilder().addInputs("input1", "input2", "input3").addLayer("dense1", (New DenseLayer.Builder()).nIn(featuresz).nOut(midsz).activation(New ActivationTanH()).build(), "input1").addLayer("dense2", (New DenseLayer.Builder()).nIn(featuresz).nOut(midsz).activation(New ActivationTanH()).build(), "input2").addLayer("dense3", (New DenseLayer.Builder()).nIn(featuresz).nOut(midsz).activation(New ActivationTanH()).build(), "input3").addVertex("elementwiseProduct", New ElementWiseVertex(ElementWiseVertex.Op.Product), "dense1", "dense2", "dense3").addLayer("output", (New OutputLayer.Builder()).nIn(midsz).nOut(outputsz).activation(New ActivationSigmoid()).lossFunction(LossFunction.MSE).build(), "elementwiseProduct").setOutputs("output").build()
			Dim cg As New ComputationGraph(cgc)
			cg.init()
			Dim input1 As INDArray = Nd4j.rand(New Integer() { batchsz, featuresz }, New UniformDistribution(-1, 1))
			Dim input2 As INDArray = Nd4j.rand(New Integer() { batchsz, featuresz }, New UniformDistribution(-1, 1))
			Dim input3 As INDArray = Nd4j.rand(New Integer() { batchsz, featuresz }, New UniformDistribution(-1, 1))
			Dim target As INDArray = nullsafe(Nd4j.rand(New Integer() { batchsz, outputsz }, New UniformDistribution(0, 1)))
			cg.setInputs(input1, input2, input3)
			cg.Labels = target
			cg.computeGradientAndScore()
			' Let's figure out what our params are now.
			Dim params As IDictionary(Of String, INDArray) = cg.paramTable()
			Dim dense1_W As INDArray = nullsafe(params("dense1_W"))
			Dim dense1_b As INDArray = nullsafe(params("dense1_b"))
			Dim dense2_W As INDArray = nullsafe(params("dense2_W"))
			Dim dense2_b As INDArray = nullsafe(params("dense2_b"))
			Dim dense3_W As INDArray = nullsafe(params("dense3_W"))
			Dim dense3_b As INDArray = nullsafe(params("dense3_b"))
			Dim output_W As INDArray = nullsafe(params("output_W"))
			Dim output_b As INDArray = nullsafe(params("output_b"))
			' Now, let's calculate what we expect the output to be.
			Dim mh As INDArray = input1.mmul(dense1_W).addi(dense1_b.repmat(batchsz, 1))
			Dim m As INDArray = (Transforms.tanh(mh))
			Dim nh As INDArray = input2.mmul(dense2_W).addi(dense2_b.repmat(batchsz, 1))
			Dim n As INDArray = (Transforms.tanh(nh))
			Dim oh As INDArray = input3.mmul(dense3_W).addi(dense3_b.repmat(batchsz, 1))
			Dim o As INDArray = (Transforms.tanh(oh))
			Dim middle As INDArray = Nd4j.ones(batchsz, midsz)
			middle.muli(m).muli(n).muli(o)
			Dim expect As INDArray = Nd4j.zeros(batchsz, outputsz)
			expect.addi(Transforms.sigmoid(middle.mmul(output_W).addi(output_b.repmat(batchsz, 1))))
			Dim output As INDArray = nullsafe(cg.output(input1, input2, input3)(0))
			Assertions.assertEquals(0.0, mse(output, expect), Me.epsilon)
			Dim pgd As Pair(Of Gradient, Double) = cg.gradientAndScore()
			Dim score As Double = pgd.Second
			Assertions.assertEquals(score, mse(output, target), Me.epsilon)
			Dim gradients As IDictionary(Of String, INDArray) = pgd.First.gradientForVariable()
	'        
	'         * So. Let's say we have inputs a, b, c
	'         * mh = a W1 + b1
	'         * m = tanh(mh)
	'         *
	'         * nh = b W2 + b2
	'         * n = tanh(nh)
	'         *
	'         * oh = c W3 + b3
	'         * o = tanh(oh)
	'         *
	'         * s = m*n*o
	'         *
	'         * yh = s W4 + b4
	'         * y = sigmoid(yh)
	'         *
	'         * E = (y-t)^2
	'         * dE/dy = 2 (y-t)
	'         *
	'         * dy/dyh = y * (1-y)
	'         * dE/dyh = 2 * y * (1-y) * (y-t)
	'         *
	'         * dyh/dW4 = s.transpose()
	'         * dyh/db4 = Nd4j.ones(1, batchsz)
	'         * dyh/ds = W4.tranpose()
	'         *
	'         * ds/dm = Nd4j.ones(1, midsz).mul(o).mul(n) // Basically the _rest_ of the middle layers
	'         *
	'         * dm/dmh = 1-(m^2)
	'         *
	'         * dmh/dW1 = a.transpose()
	'         * dmh/db1 = Nd4j.ones(1, batchsz)
	'         *
	'         
			Dim y As INDArray = output
			Dim s As INDArray = middle
			Dim W4 As INDArray = output_W
			Dim dEdy As INDArray = Nd4j.zeros(target.shape())
			' This should be of size batchsz x outputsz
			dEdy.addi(y).subi(target).muli(2)
			' Why? Because the LossFunction divides by the _element size_ of the output.
			dEdy.divi(target.shape()(1))
			' This is of size batchsz x outputsz
			Dim dydyh As INDArray = y.mul(y.mul(-1).add(1))
			Dim dEdyh As INDArray = dydyh.mul(dEdy)
			Dim dyhdW4 As INDArray = s.transpose()
			Dim dEdW4 As INDArray = nullsafe(dyhdW4.mmul(dEdyh))
			Dim dyhdb4 As INDArray = Nd4j.ones(1, batchsz)
			Dim dEdb4 As INDArray = nullsafe(dyhdb4.mmul(dEdyh))
			Dim dyhds As INDArray = W4.transpose()
			Dim dEds As INDArray = dEdyh.mmul(dyhds)
			Dim dsdm As INDArray = Nd4j.ones(batchsz, midsz).muli(n).muli(o)
			Dim dEdm As INDArray = dsdm.mul(dEds)
			Dim dmdmh As INDArray = (m.mul(m)).mul(-1).add(1)
			Dim dEdmh As INDArray = dmdmh.mul(dEdm)
			Dim dmhdW1 As INDArray = input1.transpose()
			Dim dEdW1 As INDArray = nullsafe(dmhdW1.mmul(dEdmh))
			Dim dmhdb1 As INDArray = Nd4j.ones(1, batchsz)
			Dim dEdb1 As INDArray = nullsafe(dmhdb1.mmul(dEdmh))
			Dim dsdn As INDArray = Nd4j.ones(batchsz, midsz).muli(m).muli(o)
			Dim dEdn As INDArray = dsdn.mul(dEds)
			Dim dndnh As INDArray = (n.mul(n)).mul(-1).add(1)
			Dim dEdnh As INDArray = dndnh.mul(dEdn)
			Dim dnhdW2 As INDArray = input2.transpose()
			Dim dEdW2 As INDArray = nullsafe(dnhdW2.mmul(dEdnh))
			Dim dnhdb2 As INDArray = Nd4j.ones(1, batchsz)
			Dim dEdb2 As INDArray = nullsafe(dnhdb2.mmul(dEdnh))
			Dim dsdo As INDArray = Nd4j.ones(batchsz, midsz).muli(m).muli(n)
			Dim dEdo As INDArray = dsdo.mul(dEds)
			Dim dodoh As INDArray = (o.mul(o)).mul(-1).add(1)
			Dim dEdoh As INDArray = dodoh.mul(dEdo)
			Dim dohdW3 As INDArray = input3.transpose()
			Dim dEdW3 As INDArray = nullsafe(dohdW3.mmul(dEdoh))
			Dim dohdb3 As INDArray = Nd4j.ones(1, batchsz)
			Dim dEdb3 As INDArray = nullsafe(dohdb3.mmul(dEdoh))
			Assertions.assertEquals(0, mse(nullsafe(gradients("output_W")), dEdW4), Me.epsilon)
			Assertions.assertEquals(0, mse(nullsafe(gradients("output_b")), dEdb4), Me.epsilon)
			Assertions.assertEquals(0, mse(nullsafe(gradients("dense1_W")), dEdW1), Me.epsilon)
			Assertions.assertEquals(0, mse(nullsafe(gradients("dense1_b")), dEdb1), Me.epsilon)
			Assertions.assertEquals(0, mse(nullsafe(gradients("dense2_W")), dEdW2), Me.epsilon)
			Assertions.assertEquals(0, mse(nullsafe(gradients("dense2_b")), dEdb2), Me.epsilon)
			Assertions.assertEquals(0, mse(nullsafe(gradients("dense3_W")), dEdW3), Me.epsilon)
			Assertions.assertEquals(0, mse(nullsafe(gradients("dense3_b")), dEdb3), Me.epsilon)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Element Wise Vertex Full Subtract") void testElementWiseVertexFullSubtract()
		Friend Overridable Sub testElementWiseVertexFullSubtract()
			Dim batchsz As Integer = 24
			Dim featuresz As Integer = 17
			Dim midsz As Integer = 13
			Dim outputsz As Integer = 11
			Dim cgc As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).dataType(DataType.DOUBLE).biasInit(0.0).updater(New Sgd()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).graphBuilder().addInputs("input1", "input2").addLayer("dense1", (New DenseLayer.Builder()).nIn(featuresz).nOut(midsz).activation(New ActivationTanH()).build(), "input1").addLayer("dense2", (New DenseLayer.Builder()).nIn(featuresz).nOut(midsz).activation(New ActivationTanH()).build(), "input2").addVertex("elementwiseSubtract", New ElementWiseVertex(ElementWiseVertex.Op.Subtract), "dense1", "dense2").addLayer("output", (New OutputLayer.Builder()).nIn(midsz).nOut(outputsz).activation(New ActivationSigmoid()).lossFunction(LossFunction.MSE).build(), "elementwiseSubtract").setOutputs("output").build()
			Dim cg As New ComputationGraph(cgc)
			cg.init()
			Dim input1 As INDArray = Nd4j.rand(New Integer() { batchsz, featuresz }, New UniformDistribution(-1, 1))
			Dim input2 As INDArray = Nd4j.rand(New Integer() { batchsz, featuresz }, New UniformDistribution(-1, 1))
			Dim target As INDArray = nullsafe(Nd4j.rand(New Integer() { batchsz, outputsz }, New UniformDistribution(0, 1)))
			cg.setInputs(input1, input2)
			cg.Labels = target
			cg.computeGradientAndScore()
			' Let's figure out what our params are now.
			Dim params As IDictionary(Of String, INDArray) = cg.paramTable()
			Dim dense1_W As INDArray = nullsafe(params("dense1_W"))
			Dim dense1_b As INDArray = nullsafe(params("dense1_b"))
			Dim dense2_W As INDArray = nullsafe(params("dense2_W"))
			Dim dense2_b As INDArray = nullsafe(params("dense2_b"))
			Dim output_W As INDArray = nullsafe(params("output_W"))
			Dim output_b As INDArray = nullsafe(params("output_b"))
			' Now, let's calculate what we expect the output to be.
			Dim mh As INDArray = input1.mmul(dense1_W).addi(dense1_b.repmat(batchsz, 1))
			Dim m As INDArray = (Transforms.tanh(mh))
			Dim nh As INDArray = input2.mmul(dense2_W).addi(dense2_b.repmat(batchsz, 1))
			Dim n As INDArray = (Transforms.tanh(nh))
			Dim middle As INDArray = Nd4j.zeros(batchsz, midsz)
			middle.addi(m).subi(n)
			Dim expect As INDArray = Nd4j.zeros(batchsz, outputsz)
			expect.addi(Transforms.sigmoid(middle.mmul(output_W).addi(output_b.repmat(batchsz, 1))))
			Dim output As INDArray = nullsafe(cg.output(input1, input2)(0))
			Assertions.assertEquals(0.0, mse(output, expect), Me.epsilon)
			Dim pgd As Pair(Of Gradient, Double) = cg.gradientAndScore()
			Dim score As Double = pgd.Second
			Assertions.assertEquals(score, mse(output, target), Me.epsilon)
			Dim gradients As IDictionary(Of String, INDArray) = pgd.First.gradientForVariable()
	'        
	'         * So. Let's say we have inputs a, b, c
	'         * mh = a W1 + b1
	'         * m = tanh(mh)
	'         *
	'         * nh = b W2 + b2
	'         * n = tanh(nh)
	'         *
	'         * s = m-n
	'         *
	'         * yh = s W4 + b4
	'         * y = sigmoid(yh)
	'         *
	'         * E = (y-t)^2
	'         * dE/dy = 2 (y-t)
	'         *
	'         * dy/dyh = y * (1-y)
	'         * dE/dyh = 2 * y * (1-y) * (y-t)
	'         *
	'         * dyh/dW4 = s.transpose()
	'         * dyh/db4 = Nd4j.ones(1, batchsz)
	'         * dyh/ds = W4.tranpose()
	'         *
	'         * ds/dm = Nd4j.ones(1, midsz)
	'         * ds/dn = Nd4j.ones(1, midsz).muli(-1)
	'         *
	'         * dm/dmh = 1-(m^2)
	'         *
	'         * dmh/dW1 = a.transpose()
	'         * dmh/db1 = Nd4j.ones(1, batchsz)
	'         *
	'         
			Dim y As INDArray = output
			Dim s As INDArray = middle
			Dim W4 As INDArray = output_W
			Dim dEdy As INDArray = Nd4j.zeros(target.shape())
			' This should be of size batchsz x outputsz
			dEdy.addi(y).subi(target).muli(2)
			' Why? Because the LossFunction divides by the _element size_ of the output.
			dEdy.divi(target.shape()(1))
			' This is of size batchsz x outputsz
			Dim dydyh As INDArray = y.mul(y.mul(-1).add(1))
			Dim dEdyh As INDArray = dydyh.mul(dEdy)
			Dim dyhdW4 As INDArray = s.transpose()
			Dim dEdW4 As INDArray = nullsafe(dyhdW4.mmul(dEdyh))
			Dim dyhdb4 As INDArray = Nd4j.ones(1, batchsz)
			Dim dEdb4 As INDArray = nullsafe(dyhdb4.mmul(dEdyh))
			Dim dyhds As INDArray = W4.transpose()
			Dim dEds As INDArray = dEdyh.mmul(dyhds)
			Dim dsdm As INDArray = Nd4j.ones(batchsz, midsz)
			Dim dEdm As INDArray = dsdm.mul(dEds)
			Dim dmdmh As INDArray = (m.mul(m)).mul(-1).add(1)
			Dim dEdmh As INDArray = dmdmh.mul(dEdm)
			Dim dmhdW1 As INDArray = input1.transpose()
			Dim dEdW1 As INDArray = nullsafe(dmhdW1.mmul(dEdmh))
			Dim dmhdb1 As INDArray = Nd4j.ones(1, batchsz)
			Dim dEdb1 As INDArray = nullsafe(dmhdb1.mmul(dEdmh))
			Dim dsdn As INDArray = Nd4j.ones(batchsz, midsz).muli(-1)
			Dim dEdn As INDArray = dsdn.mul(dEds)
			Dim dndnh As INDArray = (n.mul(n)).mul(-1).add(1)
			Dim dEdnh As INDArray = dndnh.mul(dEdn)
			Dim dnhdW2 As INDArray = input2.transpose()
			Dim dEdW2 As INDArray = nullsafe(dnhdW2.mmul(dEdnh))
			Dim dnhdb2 As INDArray = Nd4j.ones(1, batchsz)
			Dim dEdb2 As INDArray = nullsafe(dnhdb2.mmul(dEdnh))
			Assertions.assertEquals(0, mse(nullsafe(gradients("output_W")), dEdW4), Me.epsilon)
			Assertions.assertEquals(0, mse(nullsafe(gradients("output_b")), dEdb4), Me.epsilon)
			Assertions.assertEquals(0, mse(nullsafe(gradients("dense1_W")), dEdW1), Me.epsilon)
			Assertions.assertEquals(0, mse(nullsafe(gradients("dense1_b")), dEdb1), Me.epsilon)
			Assertions.assertEquals(0, mse(nullsafe(gradients("dense2_W")), dEdW2), Me.epsilon)
			Assertions.assertEquals(0, mse(nullsafe(gradients("dense2_b")), dEdb2), Me.epsilon)
		End Sub

		Private Shared Function mse(ByVal output As INDArray, ByVal target As INDArray) As Double
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim mse_expect As Double = Transforms.pow(output.sub(target), 2.0).sumNumber().doubleValue() / (output.columns() * output.rows())
			Return mse_expect
		End Function

		Private Shared Function nullsafe(Of T)(ByVal obj As T) As T
			If obj Is Nothing Then
				Throw New System.NullReferenceException()
			End If
			Dim clean As T = obj
			Return clean
		End Function

		Private epsilon As Double = 1e-10
	End Class

End Namespace