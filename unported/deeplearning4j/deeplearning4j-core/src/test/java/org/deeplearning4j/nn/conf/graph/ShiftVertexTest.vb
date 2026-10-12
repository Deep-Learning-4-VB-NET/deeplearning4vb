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
Imports BaseActivationFunction = org.nd4j.linalg.activations.BaseActivationFunction
Imports ActivationSigmoid = org.nd4j.linalg.activations.impl.ActivationSigmoid
Imports ActivationTanH = org.nd4j.linalg.activations.impl.ActivationTanH
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction
Imports org.nd4j.common.primitives
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
'ORIGINAL LINE: @DisplayName("Shift Vertex Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class ShiftVertexTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class ShiftVertexTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Shift Vertex Num Params True") void testShiftVertexNumParamsTrue()
		Friend Overridable Sub testShiftVertexNumParamsTrue()
	'        
	'         * https://github.com/eclipse/deeplearning4j/pull/3514#issuecomment-307754386
	'         * from @agibsonccc: check for the basics: like 0 numParams
	'         
			' The 0.7 doesn't really matter.
			Dim sv As New ShiftVertex(0.7)
			Assertions.assertEquals(0, sv.numParams(True))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Shift Vertex Num Params False") void testShiftVertexNumParamsFalse()
		Friend Overridable Sub testShiftVertexNumParamsFalse()
	'        
	'         * https://github.com/eclipse/deeplearning4j/pull/3514#issuecomment-307754386
	'         * from @agibsonccc: check for the basics: like 0 numParams
	'         
			' The 0.7 doesn't really matter.
			Dim sv As New ShiftVertex(0.7)
			Assertions.assertEquals(0, sv.numParams(False))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Get") void testGet()
		Friend Overridable Sub testGet()
			Dim sv As New ShiftVertex(0.7)
			Assertions.assertEquals(0.7, sv.getShiftFactor(), Me.epsilon)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Simple") void testSimple()
		Friend Overridable Sub testSimple()
	'        
	'         * This function _simply_ tests whether ShiftVertex is _in fact_ adding the shift value to it's inputs.
	'         
			' Just first n primes / 10.
			Dim input As INDArray = Nd4j.create(New Double()() {
				New Double() { 0.2, 0.3, 0.5 },
				New Double() { 0.7, 1.1, 1.3 },
				New Double() { 1.7, 1.9, 2.3 },
				New Double() { 2.9, 3.1, 3.7 }
			})
			Dim sf As Double = 4.1
			Dim cgc As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("input").addLayer("denselayer", (New DenseLayer.Builder()).nIn(input.columns()).nOut(1).activation(Activation.IDENTITY).build(), "input").addLayer("identityinputactivation", (New ActivationLayer.Builder()).activation(Activation.IDENTITY).build(), "input").addVertex("shiftvertex", New ShiftVertex(sf), "identityinputactivation").addLayer("identityshiftvertex", (New ActivationLayer.Builder()).activation(Activation.IDENTITY).build(), "shiftvertex").setOutputs("identityshiftvertex", "denselayer").build()
			Dim cg As New ComputationGraph(cgc)
			cg.init()
			' We can call outputSingle, because we only have a single output layer. It has nothing to do with minibatches.
			Dim output As INDArray = cg.output(True, input)(0)
			Dim target As INDArray = Nd4j.zeros(input.shape())
			target.addi(input)
			target.addi(sf)
			Dim squared As INDArray = output.sub(target)
			Dim rms As Double = squared.mul(squared).sumNumber().doubleValue()
			Assertions.assertEquals(0.0, rms, Me.epsilon)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Comprehensive") void testComprehensive()
		Friend Overridable Sub testComprehensive()
	'        
	'         * This function tests ShiftVertex more comprehensively. Specifically, it verifies that the lossfunction works as
	'         * expected on a ComputationGraph _with_ a ShiftVertex and it verifies that the derivatives produced by 
	'         * back propagating work as expected.
	'         
			Dim a1 As BaseActivationFunction = New ActivationTanH()
			Dim a2 As BaseActivationFunction = New ActivationSigmoid()
			' Just first n primes / 10.
			Dim input As INDArray = Nd4j.create(New Double()() {
				New Double() { 0.2, 0.3, 0.5 },
				New Double() { 0.7, 1.1, 1.3 },
				New Double() { 1.7, 1.9, 2.3 },
				New Double() { 2.9, 3.1, 3.7 }
			})
			Dim sf As Double = 4.1
			' Actually, given that I'm using a sigmoid on the output,
			' these should really be between 0 and 1
			Dim target As INDArray = Nd4j.create(New Double()() {
				New Double() { 0.05, 0.10, 0.15, 0.20, 0.25 },
				New Double() { 0.30, 0.35, 0.40, 0.45, 0.50 },
				New Double() { 0.55, 0.60, 0.65, 0.70, 0.75 },
				New Double() { 0.80, 0.85, 0.90, 0.95, 0.99 }
			})
			Dim cgc As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).dataType(DataType.DOUBLE).updater(New Sgd(0.01)).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).graphBuilder().addInputs("input").addLayer("denselayer", (New DenseLayer.Builder()).nIn(input.columns()).nOut(input.columns()).activation(a1).build(), "input").addVertex("shiftvertex", New ShiftVertex(sf), "denselayer").addLayer("output", (New OutputLayer.Builder()).nIn(input.columns()).nOut(target.columns()).activation(a2).lossFunction(LossFunction.MSE).build(), "shiftvertex").setOutputs("output").build()
			Dim cg As New ComputationGraph(cgc)
			cg.init()
			cg.setInput(0, input)
			cg.setLabel(0, target)
			cg.computeGradientAndScore()
			Dim score_dl4j As Double = cg.score()
			Dim weights As IDictionary(Of String, INDArray) = cg.paramTable()
			Dim g As Gradient = cg.gradient()
			Dim gradients As IDictionary(Of String, INDArray) = g.gradientForVariable()
			Dim manual_gradients As IDictionary(Of String, INDArray) = New SortedDictionary(Of String, INDArray)()
			Dim W As INDArray = nullsafe(weights("denselayer_W"))
			Dim b As INDArray = nullsafe(weights("denselayer_b"))
			Dim V As INDArray = nullsafe(weights("output_W"))
			Dim c As INDArray = nullsafe(weights("output_b"))
			Dim manual_weights As IDictionary(Of String, INDArray) = New SortedDictionary(Of String, INDArray)()
			manual_weights("denselayer_W") = W
			manual_weights("denselayer_b") = b
			manual_weights("output_W") = V
			manual_weights("output_b") = c
			' First things first, let's calculate the score.
			Dim batchsz As Long = input.shape()(0)
			Dim z As INDArray = input.castTo(W.dataType()).mmul(W).add(b.repmat(batchsz, 1))
			' activation modifies it's input!!
			Dim a As INDArray = a1.getActivation(z.dup(), True).add(sf)
			Dim q As INDArray = a.mmul(V).add(c.repmat(batchsz, 1))
			Dim o As INDArray = nullsafe(a2.getActivation(q.dup(), True))
			Dim score_manual As Double = sum_errors(o, target) / (o.columns() * o.rows())
	'        
	'         * So. We have
	'         * z5 = input1 * W15 + input2 * W25 + input3 * W35 + b5
	'         * a5 = activation(z5) + sr
	'         * q9 = a1 * V19 + a2 * V29 + a3 * V39 + c9
	'         * o9 = activation(q9)
	'         *  
	'         * dE/do = 2(o-t)
	'         * doj/dqj = activation'(qj)
	'         * dqj/dVij = ai dqj/dai = Vij dqj/dbj = 1
	'         * 
	'         * dq1/dv11 = a1 dq2/dV12 = a1 dq3/dV13 = a1 ...
	'         * dq1/dv21 = a2 dq2...
	'         
			' Nd4j.zeros(target.shape());
			Dim dEdo As INDArray = target.like()
			' This should be of size batchsz x outputsz
			dEdo.addi(o.castTo(dEdo.dataType())).subi(target).muli(2)
			' Why? Because the LossFunction divides by the _element size_ of the output.
			dEdo.divi(target.shape()(1))
			Dim derivs2 As Pair(Of INDArray, INDArray) = a2.backprop(q, dEdo)
			' This should be of size batchsz x outputsz (dE/do * do/dq) this _should_ be o * (1-o) * dE/do for Sigmoid.
			Dim dEdq As INDArray = derivs2.First
			' Should be o = q^3 do/dq = 3 q^2 for Cube.
	'        
	'        INDArray dodq = q.mul(q).mul(3);
	'        INDArray tbv = dodq.mul(dEdo);
	'        System.err.println("----");
	'        System.err.println(q);
	'        System.err.println(o);
	'        System.err.println(tbv);
	'        System.err.println(dEdq);
	'        
			Dim dqdc As INDArray = Nd4j.ones(1, batchsz)
			' This should be of size 1 x outputsz
			Dim dEdc As INDArray = dqdc.mmul(dEdq)
			Dim dEdV As INDArray = a.transpose().mmul(dEdq)
			' This should be dEdo * dodq * dqda
			Dim dEda As INDArray = dEdq.mmul(V.transpose())
			Dim derivs1 As Pair(Of INDArray, INDArray) = a1.backprop(z, dEda)
			Dim dEdz As INDArray = derivs1.First
			Dim dzdb As INDArray = Nd4j.ones(1, batchsz)
			Dim dEdb As INDArray = dzdb.mmul(dEdz)
			Dim dEdW As INDArray = input.transpose().mmul(dEdz)
			manual_gradients("output_b") = dEdc
			manual_gradients("output_W") = dEdV
			manual_gradients("denselayer_b") = dEdb
			manual_gradients("denselayer_W") = dEdW
			Dim summse As Double = Math.Pow((score_manual - score_dl4j), 2)
			Dim denominator As Integer = 1
			For Each mesi As KeyValuePair(Of String, INDArray) In gradients.SetOfKeyValuePairs()
				Dim name As String = mesi.Key
				Dim dl4j_gradient As INDArray = nullsafe(mesi.Value)
				Dim manual_gradient As INDArray = nullsafe(manual_gradients(name))
				Dim se As Double = sum_errors(dl4j_gradient, manual_gradient)
				summse += se
				denominator += dl4j_gradient.columns() * dl4j_gradient.rows()
			Next mesi
			Assertions.assertEquals(0.0, summse / denominator, Me.epsilon)
		End Sub

		Private Shared Function sum_errors(ByVal a As INDArray, ByVal b As INDArray) As Double
			Dim o As INDArray = a.sub(b.castTo(a.dataType()))
			Return o.mul(o).sumNumber().doubleValue()
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