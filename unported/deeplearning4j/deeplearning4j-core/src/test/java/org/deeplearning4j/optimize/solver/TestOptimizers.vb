Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports org.deeplearning4j.nn.api
Imports CacheMode = org.deeplearning4j.nn.conf.CacheMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports LayerHelper = org.deeplearning4j.nn.layers.LayerHelper
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ConvexOptimizer = org.deeplearning4j.optimize.api.ConvexOptimizer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ConjugateGradient = org.deeplearning4j.optimize.solvers.ConjugateGradient
Imports LBFGS = org.deeplearning4j.optimize.solvers.LBFGS
Imports LineGradientDescent = org.deeplearning4j.optimize.solvers.LineGradientDescent
Imports StochasticGradientDescent = org.deeplearning4j.optimize.solvers.StochasticGradientDescent
Imports NegativeDefaultStepFunction = org.deeplearning4j.optimize.stepfunctions.NegativeDefaultStepFunction
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Cos = org.nd4j.linalg.api.ops.impl.transforms.strict.Cos
Imports Sin = org.nd4j.linalg.api.ops.impl.transforms.strict.Sin
Imports DefaultRandom = org.nd4j.linalg.api.rng.DefaultRandom
Imports Random = org.nd4j.linalg.api.rng.Random
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Condition = org.nd4j.linalg.indexing.conditions.Condition
Imports AdaGrad = org.nd4j.linalg.learning.config.AdaGrad
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction
Imports org.nd4j.common.primitives
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.deeplearning4j.optimize.solver


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.TRAINING) public class TestOptimizers extends org.deeplearning4j.BaseDL4JTest
	Public Class TestOptimizers
		Inherits BaseDL4JTest

		'For debugging.
		Private Const PRINT_OPT_RESULTS As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testOptimizersBasicMLPBackprop()
		Public Overridable Sub testOptimizersBasicMLPBackprop()
			'Basic tests of the 'does it throw an exception' variety.

			Dim iter As DataSetIterator = New IrisDataSetIterator(5, 50)

			Dim toTest() As OptimizationAlgorithm = {OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT, OptimizationAlgorithm.LINE_GRADIENT_DESCENT, OptimizationAlgorithm.CONJUGATE_GRADIENT, OptimizationAlgorithm.LBFGS}

			For Each oa As OptimizationAlgorithm In toTest
				Dim network As New MultiLayerNetwork(getMLPConfigIris(oa))
				network.init()

				iter.reset()
				network.fit(iter)
			Next oa
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testOptimizersMLP()
		Public Overridable Sub testOptimizersMLP()
			'Check that the score actually decreases over time

			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)

			Dim toTest() As OptimizationAlgorithm = {OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT, OptimizationAlgorithm.LINE_GRADIENT_DESCENT, OptimizationAlgorithm.CONJUGATE_GRADIENT, OptimizationAlgorithm.LBFGS}

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			ds.normalizeZeroMeanZeroUnitVariance()

			For Each oa As OptimizationAlgorithm In toTest
				Dim nIter As Integer = 5
				Dim network As New MultiLayerNetwork(getMLPConfigIris(oa))
				network.init()
				Dim score As Double = network.score(ds)
				assertTrue(score <> 0.0 AndAlso Not Double.IsNaN(score))

				If PRINT_OPT_RESULTS Then
					Console.WriteLine("testOptimizersMLP() - " & oa)
				End If

				Dim nCallsToOptimizer As Integer = 10
				Dim scores(nCallsToOptimizer) As Double
				scores(0) = score
				For i As Integer = 0 To nCallsToOptimizer - 1
					For j As Integer = 0 To nIter - 1
						network.fit(ds)
					Next j
					Dim scoreAfter As Double = network.score(ds)
					scores(i + 1) = scoreAfter
					assertTrue(Not Double.IsNaN(scoreAfter),"Score is NaN after optimization")
					assertTrue(scoreAfter <= score,"OA= " & oa & ", before= " & score & ", after= " & scoreAfter)
					score = scoreAfter
				Next i

				If PRINT_OPT_RESULTS Then
					Console.WriteLine(oa & " - " & Arrays.toString(scores))
				End If
			Next oa
		End Sub

		Private Shared Function getMLPConfigIris(ByVal oa As OptimizationAlgorithm) As MultiLayerConfiguration
			Dim c As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(oa).updater(New AdaGrad(1e-1)).seed(12345L).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).weightInit(WeightInit.XAVIER).activation(Activation.RELU).build()).layer(1, (New OutputLayer.Builder(LossFunction.MCXENT)).nIn(3).nOut(3).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build()).build()

			Return c
		End Function

		'==================================================
		' Sphere Function Optimizer Tests

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSphereFnOptStochGradDescent()
		Public Overridable Sub testSphereFnOptStochGradDescent()
			testSphereFnOptHelper(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT, 5, 2)
			testSphereFnOptHelper(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT, 5, 10)
			testSphereFnOptHelper(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT, 5, 100)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSphereFnOptLineGradDescent()
		Public Overridable Sub testSphereFnOptLineGradDescent()
			'Test a single line search with calculated search direction (with multiple line search iterations)
			Dim numLineSearchIter() As Integer = {5, 10}
			For Each n As Integer In numLineSearchIter
				testSphereFnOptHelper(OptimizationAlgorithm.LINE_GRADIENT_DESCENT, n, 2)
			Next n

			For Each n As Integer In numLineSearchIter
				testSphereFnOptHelper(OptimizationAlgorithm.LINE_GRADIENT_DESCENT, n, 10)
			Next n

			For Each n As Integer In numLineSearchIter
				testSphereFnOptHelper(OptimizationAlgorithm.LINE_GRADIENT_DESCENT, n, 100)
			Next n
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSphereFnOptCG()
		Public Overridable Sub testSphereFnOptCG()
			'Test a single line search with calculated search direction (with multiple line search iterations)
			Dim numLineSearchIter() As Integer = {5, 10}
			For Each n As Integer In numLineSearchIter
				testSphereFnOptHelper(OptimizationAlgorithm.CONJUGATE_GRADIENT, n, 2)
			Next n

			For Each n As Integer In numLineSearchIter
				testSphereFnOptHelper(OptimizationAlgorithm.CONJUGATE_GRADIENT, n, 10)
			Next n

			For Each n As Integer In numLineSearchIter
				testSphereFnOptHelper(OptimizationAlgorithm.CONJUGATE_GRADIENT, n, 100)
			Next n
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSphereFnOptLBFGS()
		Public Overridable Sub testSphereFnOptLBFGS()
			'Test a single line search with calculated search direction (with multiple line search iterations)
			Dim numLineSearchIter() As Integer = {5, 10}
			For Each n As Integer In numLineSearchIter
				testSphereFnOptHelper(OptimizationAlgorithm.LBFGS, n, 2)
			Next n

			For Each n As Integer In numLineSearchIter
				testSphereFnOptHelper(OptimizationAlgorithm.LBFGS, n, 10)
			Next n

			For Each n As Integer In numLineSearchIter
				testSphereFnOptHelper(OptimizationAlgorithm.LBFGS, n, 100)
			Next n
		End Sub

		Public Overridable Sub testSphereFnOptHelper(ByVal oa As OptimizationAlgorithm, ByVal numLineSearchIter As Integer, ByVal nDimensions As Integer)

			If PRINT_OPT_RESULTS Then
				Console.WriteLine("---------" & vbLf & " Alg= " & oa & ", nIter= " & numLineSearchIter & ", nDimensions= " & nDimensions)
			End If

			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).maxNumLineSearchIterations(numLineSearchIter).updater(New Sgd(1e-2)).layer((New DenseLayer.Builder()).nIn(1).nOut(1).build()).build()
			conf.addVariable("W") 'Normally done by ParamInitializers, but obviously that isn't done here

			Dim rng As Random = New DefaultRandom(12345L)
			Dim dist As org.nd4j.linalg.api.rng.distribution.Distribution = New org.nd4j.linalg.api.rng.distribution.impl.UniformDistribution(rng, -10, 10)
			Dim m As Model = New SphereFunctionModel(nDimensions, dist, conf)
			m.computeGradientAndScore(LayerWorkspaceMgr.noWorkspaces())
			Dim scoreBefore As Double = m.score()
			assertTrue(Not Double.IsNaN(scoreBefore) AndAlso Not Double.IsInfinity(scoreBefore))
			If PRINT_OPT_RESULTS Then
				Console.WriteLine("Before:")
				Console.WriteLine(scoreBefore)
				Console.WriteLine(m.params())
			End If

			Dim opt As ConvexOptimizer = getOptimizer(oa, conf, m)

			opt.setupSearchState(m.gradientAndScore())
			For i As Integer = 0 To 99
				opt.optimize(LayerWorkspaceMgr.noWorkspaces())
			Next i
			m.computeGradientAndScore(LayerWorkspaceMgr.noWorkspaces())
			Dim scoreAfter As Double = m.score()

			assertTrue(Not Double.IsNaN(scoreAfter) AndAlso Not Double.IsInfinity(scoreAfter))
			If PRINT_OPT_RESULTS Then
				Console.WriteLine("After:")
				Console.WriteLine(scoreAfter)
				Console.WriteLine(m.params())
			End If

			'Expected behaviour after optimization:
			'(a) score is better (lower) after optimization.
			'(b) Parameters are closer to minimum after optimization (TODO)
			assertTrue(scoreAfter < scoreBefore, "Score did not improve after optimization (b= " & scoreBefore & " ,a= " & scoreAfter & ")")
		End Sub

		Private Shared Function getOptimizer(ByVal oa As OptimizationAlgorithm, ByVal conf As NeuralNetConfiguration, ByVal m As Model) As ConvexOptimizer
			Select Case oa
				Case org.deeplearning4j.nn.api.OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT
					Return New StochasticGradientDescent(conf, New NegativeDefaultStepFunction(), Nothing, m)
				Case org.deeplearning4j.nn.api.OptimizationAlgorithm.LINE_GRADIENT_DESCENT
					Return New LineGradientDescent(conf, New NegativeDefaultStepFunction(), Nothing, m)
				Case org.deeplearning4j.nn.api.OptimizationAlgorithm.CONJUGATE_GRADIENT
					Return New ConjugateGradient(conf, New NegativeDefaultStepFunction(), Nothing, m)
				Case LBFGS
					Return New LBFGS(conf, New NegativeDefaultStepFunction(), Nothing, m)
				Case Else
					Throw New System.NotSupportedException()
			End Select
		End Function

		Private Shared Sub testSphereFnMultipleStepsHelper(ByVal oa As OptimizationAlgorithm, ByVal nOptIter As Integer, ByVal maxNumLineSearchIter As Integer)
			Dim scores(nOptIter) As Double

			For i As Integer = 0 To nOptIter
				Dim rng As Random = New DefaultRandom(12345L)
				Dim dist As org.nd4j.linalg.api.rng.distribution.Distribution = New org.nd4j.linalg.api.rng.distribution.impl.UniformDistribution(rng, -10, 10)
				Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).maxNumLineSearchIterations(maxNumLineSearchIter).updater(New Sgd(0.1)).layer((New DenseLayer.Builder()).nIn(1).nOut(1).build()).build()
				conf.addVariable("W") 'Normally done by ParamInitializers, but obviously that isn't done here

				Dim m As Model = New SphereFunctionModel(100, dist, conf)
				If i = 0 Then
					m.computeGradientAndScore(LayerWorkspaceMgr.noWorkspaces())
					scores(0) = m.score() 'Before optimization
				Else
					Dim opt As ConvexOptimizer = getOptimizer(oa, conf, m)
					For j As Integer = 0 To 99
						opt.optimize(LayerWorkspaceMgr.noWorkspaces())
					Next j
					m.computeGradientAndScore(LayerWorkspaceMgr.noWorkspaces())
					scores(i) = m.score()
					assertTrue(Not Double.IsNaN(scores(i)) AndAlso Not Double.IsInfinity(scores(i)))
				End If
			Next i

			If PRINT_OPT_RESULTS Then
				Console.WriteLine("Multiple optimization iterations (" & nOptIter & " opt. iter.) score vs iteration, maxNumLineSearchIter=" & maxNumLineSearchIter & ": " & oa)
				Console.WriteLine(Arrays.toString(scores))
			End If

			For i As Integer = 1 To scores.Length - 1
				assertTrue(scores(i) <= scores(i - 1))
			Next i
			assertTrue(scores(scores.Length - 1) < 1.0) 'Very easy function, expect score ~= 0 with any reasonable number of steps/numLineSearchIter
		End Sub


		''' <summary>
		''' A non-NN optimization problem. Optimization function (cost function) is
		''' \sum_i x_i^2. Has minimum of 0.0 at x_i=0 for all x_i
		''' See: https://en.wikipedia.org/wiki/Test_functions_for_optimization
		''' </summary>
		<Serializable>
		Private Class SphereFunctionModel
			Inherits SimpleOptimizableModel

			Friend Const serialVersionUID As Long = -6963606137417355405L

			Friend Sub New(ByVal nParams As Integer, ByVal distribution As org.nd4j.linalg.api.rng.distribution.Distribution, ByVal conf As NeuralNetConfiguration)
				MyBase.New(distribution.sample(New Integer() {1, nParams}), conf)
			End Sub


			Public Overrides Sub computeGradientAndScore(ByVal workspaceMgr As LayerWorkspaceMgr)
				' Gradients: d(x^2)/dx = 2x
				Dim gradient As INDArray = parameters.mul(2)
				Dim g As Gradient = New DefaultGradient()
				g.gradientForVariable()("W") = Me.gradientView
				Me.gradient_Conflict = g
				Me.score_Conflict = Nd4j.BlasWrapper.dot(parameters, parameters) 'sum_i x_i^2
				Me.gradientView.assign(gradient)
			End Sub

			Public Overrides Function numParams(ByVal backwards As Boolean) As Long
				Return 0
			End Function

			Public Overrides WriteOnly Property ParamsViewArray As INDArray
				Set(ByVal params As INDArray)
					Throw New System.NotSupportedException("Not supported")
				End Set
			End Property

			Public Overrides WriteOnly Property BackpropGradientsViewArray As INDArray
				Set(ByVal gradients As INDArray)
					Throw New System.NotSupportedException()
				End Set
			End Property

			Public Overrides WriteOnly Property CacheMode As CacheMode
				Set(ByVal mode As CacheMode)
					Throw New System.NotSupportedException()
				End Set
			End Property

			Public Overrides WriteOnly Property Listeners As TrainingListener()
				Set(ByVal listeners() As TrainingListener)
    
				End Set
			End Property

			Public Overrides ReadOnly Property Index As Integer
				Get
					Return 0
				End Get
			End Property

			Public Overrides Sub setInput(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)

			End Sub

			Public Overrides ReadOnly Property PretrainLayer As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides Sub clearNoiseWeightParams()

			End Sub
		End Class


		'==================================================
		' Rastrigin Function Optimizer Tests


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRastriginFnOptStochGradDescentMultipleSteps()
		Public Overridable Sub testRastriginFnOptStochGradDescentMultipleSteps()
			testRastriginFnMultipleStepsHelper(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT, 5, 20)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRastriginFnOptLineGradDescentMultipleSteps()
		Public Overridable Sub testRastriginFnOptLineGradDescentMultipleSteps()
			testRastriginFnMultipleStepsHelper(OptimizationAlgorithm.LINE_GRADIENT_DESCENT, 10, 20)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRastriginFnOptCGMultipleSteps()
		Public Overridable Sub testRastriginFnOptCGMultipleSteps()
			testRastriginFnMultipleStepsHelper(OptimizationAlgorithm.CONJUGATE_GRADIENT, 10, 20)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRastriginFnOptLBFGSMultipleSteps()
		Public Overridable Sub testRastriginFnOptLBFGSMultipleSteps()
			testRastriginFnMultipleStepsHelper(OptimizationAlgorithm.LBFGS, 10, 20)
		End Sub


		Private Shared Sub testRastriginFnMultipleStepsHelper(ByVal oa As OptimizationAlgorithm, ByVal nOptIter As Integer, ByVal maxNumLineSearchIter As Integer)
			Dim scores(nOptIter) As Double

			For i As Integer = 0 To nOptIter
				Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).maxNumLineSearchIterations(maxNumLineSearchIter).miniBatch(False).updater(New AdaGrad(1e-2)).layer((New DenseLayer.Builder()).nIn(1).nOut(1).build()).build()
				conf.addVariable("W") 'Normally done by ParamInitializers, but obviously that isn't done here

				Dim m As Model = New RastriginFunctionModel(10, conf)
				Dim nParams As Integer = CInt(m.numParams())
				If i = 0 Then
					m.computeGradientAndScore(LayerWorkspaceMgr.noWorkspaces())
					scores(0) = m.score() 'Before optimization
				Else
					Dim opt As ConvexOptimizer = getOptimizer(oa, conf, m)
					opt.Updater.setStateViewArray(DirectCast(m, Layer), Nd4j.create(New Integer() {1, nParams}, "c"c), True)
					opt.optimize(LayerWorkspaceMgr.noWorkspaces())
					m.computeGradientAndScore(LayerWorkspaceMgr.noWorkspaces())
					scores(i) = m.score()
					assertTrue(Not Double.IsNaN(scores(i)) AndAlso Not Double.IsInfinity(scores(i)))
				End If
			Next i

			If PRINT_OPT_RESULTS Then
				Console.WriteLine("Rastrigin: Multiple optimization iterations (" & nOptIter & " opt. iter.) score vs iteration, maxNumLineSearchIter=" & maxNumLineSearchIter & ": " & oa)
				Console.WriteLine(Arrays.toString(scores))
			End If
			For i As Integer = 1 To scores.Length - 1
				If i = 1 Then
					assertTrue(scores(i) <= scores(i - 1)) 'Require at least one step of improvement
				Else
					assertTrue(scores(i) <= scores(i - 1))
				End If
			Next i
		End Sub

		''' <summary>
		''' Rastrigin function: A much more complex non-NN multi-dimensional optimization problem.
		''' Global minimum of 0 at x_i = 0 for all x_i.
		''' Very large number of local minima. Can't expect to achieve global minimum with gradient-based (line search)
		''' optimizers, but can expect significant improvement in score/cost relative to initial parameters.
		''' This implementation has cost function = infinity if any parameters x_i are
		''' outside of range [-5.12,5.12]
		''' https://en.wikipedia.org/wiki/Rastrigin_function
		''' </summary>
		<Serializable>
		Private Class RastriginFunctionModel
			Inherits SimpleOptimizableModel

			Friend Const serialVersionUID As Long = -1772954508787487941L

			Friend Sub New(ByVal nDimensions As Integer, ByVal conf As NeuralNetConfiguration)
				MyBase.New(initParams(nDimensions), conf)
			End Sub

			Friend Shared Function initParams(ByVal nDimensions As Integer) As INDArray
				Dim rng As Random = New DefaultRandom(12345L)
				Dim dist As org.nd4j.linalg.api.rng.distribution.Distribution = New org.nd4j.linalg.api.rng.distribution.impl.UniformDistribution(rng, -5.12, 5.12)
				Return dist.sample(New Integer() {1, nDimensions})
			End Function


			Public Overrides Sub computeGradientAndScore(ByVal workspaceMgr As LayerWorkspaceMgr)
				'Gradient decomposes due to sum, so:
				'd(x^2 - 10*cos(2*Pi*x))/dx
				' = 2x + 20*pi*sin(2*Pi*x)
				Dim gradient As INDArray = parameters.mul(2 * Math.PI)
				Nd4j.Executioner.exec(New Sin(gradient))
				gradient.muli(20 * Math.PI)
				gradient.addi(parameters.mul(2))

				Dim g As Gradient = New DefaultGradient(Me.gradientView)
				g.gradientForVariable()("W") = Me.gradientView
				Me.gradient_Conflict = g
				'If any parameters are outside range [-5.12,5.12]: score = infinity
				Dim paramExceeds512 As INDArray = parameters.cond(New ConditionAnonymousInnerClass(Me))

				Dim nExceeds512 As Integer = paramExceeds512.castTo(DataType.DOUBLE).sum(Integer.MaxValue).getInt(0)
				If nExceeds512 > 0 Then
					Me.score_Conflict = Double.PositiveInfinity
				End If

				'Otherwise:
				Dim costFn As Double = 10 * parameters.length()
				costFn += Nd4j.BlasWrapper.dot(parameters, parameters) 'xi*xi
				Dim temp As INDArray = parameters.mul(2.0 * Math.PI)
				Nd4j.Executioner.exec(New Cos(temp))
				temp.muli(-10.0) 'After this: each element is -10*cos(2*Pi*xi)
				costFn += temp.sum(Integer.MaxValue).getDouble(0)

				Me.score_Conflict = costFn
				Me.gradientView.assign(gradient)
			End Sub

			Private Class ConditionAnonymousInnerClass
				Implements Condition

				Private ReadOnly outerInstance As RastriginFunctionModel

				Public Sub New(ByVal outerInstance As RastriginFunctionModel)
					Me.outerInstance = outerInstance
				End Sub

				Public Function condtionNum() As Integer Implements Condition.condtionNum
					Return 0
				End Function

				Public ReadOnly Property Value As Double Implements Condition.getValue
					Get
						Return 0
					End Get
				End Property

				Public Function epsThreshold() As Double Implements Condition.epsThreshold
					Return 0
				End Function

				Public Function apply(ByVal input As Number) As Boolean?
					Return Math.Abs(input.doubleValue()) > 5.12
				End Function
			End Class

			Public Overrides Function numParams(ByVal backwards As Boolean) As Long
				Return 0
			End Function

			Public Overrides WriteOnly Property ParamsViewArray As INDArray
				Set(ByVal params As INDArray)
					Throw New System.NotSupportedException("Not supported")
				End Set
			End Property

			Public Overrides WriteOnly Property BackpropGradientsViewArray As INDArray
				Set(ByVal gradients As INDArray)
					Throw New System.NotSupportedException()
				End Set
			End Property


			Public Overrides WriteOnly Property CacheMode As CacheMode
				Set(ByVal mode As CacheMode)
					Throw New System.NotSupportedException()
				End Set
			End Property

			Public Overrides WriteOnly Property Listeners As TrainingListener()
				Set(ByVal listeners() As TrainingListener)
    
				End Set
			End Property

			Public Overrides ReadOnly Property Index As Integer
				Get
					Return 0
				End Get
			End Property

			Public Overrides Sub setInput(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)

			End Sub

			Public Overrides ReadOnly Property PretrainLayer As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides Sub clearNoiseWeightParams()

			End Sub
		End Class


		'==================================================
		' Rosenbrock Function Optimizer Tests

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRosenbrockFnOptLineGradDescentMultipleSteps()
		Public Overridable Sub testRosenbrockFnOptLineGradDescentMultipleSteps()
			testRosenbrockFnMultipleStepsHelper(OptimizationAlgorithm.LINE_GRADIENT_DESCENT, 20, 20)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRosenbrockFnOptCGMultipleSteps()
		Public Overridable Sub testRosenbrockFnOptCGMultipleSteps()
			testRosenbrockFnMultipleStepsHelper(OptimizationAlgorithm.CONJUGATE_GRADIENT, 20, 20)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRosenbrockFnOptLBFGSMultipleSteps()
		Public Overridable Sub testRosenbrockFnOptLBFGSMultipleSteps()
			testRosenbrockFnMultipleStepsHelper(OptimizationAlgorithm.LBFGS, 20, 20)
		End Sub


		Private Shared Sub testRosenbrockFnMultipleStepsHelper(ByVal oa As OptimizationAlgorithm, ByVal nOptIter As Integer, ByVal maxNumLineSearchIter As Integer)
			Dim scores(nOptIter) As Double

			For i As Integer = 0 To nOptIter
				Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).maxNumLineSearchIterations(maxNumLineSearchIter).updater(New Sgd(1e-1)).stepFunction(New org.deeplearning4j.nn.conf.stepfunctions.NegativeDefaultStepFunction()).layer((New DenseLayer.Builder()).nIn(1).nOut(1).build()).build()
				conf.addVariable("W") 'Normally done by ParamInitializers, but obviously that isn't done here

				Dim m As Model = New RosenbrockFunctionModel(100, conf)
				If i = 0 Then
					m.computeGradientAndScore(LayerWorkspaceMgr.noWorkspaces())
					scores(0) = m.score() 'Before optimization
				Else
					Dim opt As ConvexOptimizer = getOptimizer(oa, conf, m)
					opt.optimize(LayerWorkspaceMgr.noWorkspaces())
					m.computeGradientAndScore(LayerWorkspaceMgr.noWorkspaces())
					scores(i) = m.score()
					assertTrue(Not Double.IsNaN(scores(i)) AndAlso Not Double.IsInfinity(scores(i)),"NaN or infinite score: " & scores(i))
				End If
			Next i

			If PRINT_OPT_RESULTS Then
				Console.WriteLine("Rosenbrock: Multiple optimization iterations ( " & nOptIter & " opt. iter.) score vs iteration, maxNumLineSearchIter= " & maxNumLineSearchIter & ": " & oa)
				Console.WriteLine(Arrays.toString(scores))
			End If
			For i As Integer = 1 To scores.Length - 1
				If i = 1 Then
					assertTrue(scores(i) < scores(i - 1)) 'Require at least one step of improvement
				Else
					assertTrue(scores(i) <= scores(i - 1))
				End If
			Next i
		End Sub



		''' <summary>
		'''Rosenbrock function: a multi-dimensional 'valley' type function.
		''' Has a single local/global minimum of f(x)=0 at x_i=1 for all x_i.
		''' Expect gradient-based optimization functions to find global minimum eventually,
		''' but optimization may be slow due to nearly flat gradient along valley.
		''' Restricted here to the range [-5,5]. This implementation gives infinite cost/score
		''' if any parameter is outside of this range.
		''' Parameters initialized in range [-4,4]
		''' See: http://www.sfu.ca/~ssurjano/rosen.html
		''' </summary>
		<Serializable>
		Private Class RosenbrockFunctionModel
			Inherits SimpleOptimizableModel

			Friend Const serialVersionUID As Long = -5129494342531033706L

			Friend Sub New(ByVal nDimensions As Integer, ByVal conf As NeuralNetConfiguration)
				MyBase.New(initParams(nDimensions), conf)
			End Sub

			Friend Shared Function initParams(ByVal nDimensions As Integer) As INDArray
				Dim rng As Random = New DefaultRandom(12345L)
				Dim dist As org.nd4j.linalg.api.rng.distribution.Distribution = New org.nd4j.linalg.api.rng.distribution.impl.UniformDistribution(rng, -4.0, 4.0)
				Return dist.sample(New Integer() {1, nDimensions})
			End Function

			Public Overrides Sub computeGradientAndScore(ByVal workspaceMgr As LayerWorkspaceMgr)
				Dim nDims As val = parameters.length()
				Dim gradient As INDArray = Nd4j.zeros(nDims)
				Dim x0 As Double = parameters.getDouble(0)
				Dim x1 As Double = parameters.getDouble(1)
				Dim g0 As Double = -400 * x0 * (x1 - x0 * x0) + 2 * (x0 - 1)
				gradient.put(0, 0, g0)
				Dim i As Integer = 1
				Do While i < nDims - 1
					Dim xim1 As Double = parameters.getDouble(i - 1)
					Dim xi As Double = parameters.getDouble(i)
					Dim xip1 As Double = parameters.getDouble(i + 1)
					Dim g As Double = 200 * (xi - xim1 * xim1) - 400 * xi * (xip1 - xi * xi) + 2 * (xi - 1)
					gradient.put(0, i, g)
					i += 1
				Loop

				Dim xl As Double = parameters.getDouble(nDims - 1)
				Dim xlm1 As Double = parameters.getDouble(nDims - 2)
				Dim gl As Double = 200 * (xl - xlm1 * xlm1)

				If nDims - 1 > Integer.MaxValue Then
					Throw New ND4JArraySizeException()
				End If
				gradient.put(0, CInt(nDims) - 1, gl)
				Dim g As Gradient = New DefaultGradient()
				g.gradientForVariable()("W") = gradient
				Me.gradient_Conflict = g

				Dim paramExceeds5 As INDArray = parameters.cond(New ConditionAnonymousInnerClass(Me))

				Dim nExceeds5 As Integer = paramExceeds5.castTo(DataType.DOUBLE).sum(Integer.MaxValue).getInt(0)
				If nExceeds5 > 0 Then
					Me.score_Conflict = Double.PositiveInfinity
				Else
					Dim score As Double = 0.0
					i = 0
					Do While i < nDims - 1
						Dim xi As Double = parameters.getDouble(i)
						Dim xi1 As Double = parameters.getDouble(i + 1)
						score += 100.0 * Math.Pow((xi1 - xi * xi), 2.0) + (xi - 1) * (xi - 1)
						i += 1
					Loop


					Me.score_Conflict = score
				End If


			End Sub

			Private Class ConditionAnonymousInnerClass
				Implements Condition

				Private ReadOnly outerInstance As RosenbrockFunctionModel

				Public Sub New(ByVal outerInstance As RosenbrockFunctionModel)
					Me.outerInstance = outerInstance
				End Sub

				Public Function condtionNum() As Integer Implements Condition.condtionNum
					Return 0
				End Function

				Public ReadOnly Property Value As Double Implements Condition.getValue
					Get
						Return 0
					End Get
				End Property

				Public Function epsThreshold() As Double Implements Condition.epsThreshold
					Return 0
				End Function

				Public Function apply(ByVal input As Number) As Boolean?
					Return Math.Abs(input.doubleValue()) > 5.0
				End Function
			End Class

			Public Overrides Function numParams(ByVal backwards As Boolean) As Long
				Return 0
			End Function

			Public Overrides WriteOnly Property ParamsViewArray As INDArray
				Set(ByVal params As INDArray)
					Throw New System.NotSupportedException("Not supported")
				End Set
			End Property

			Public Overrides WriteOnly Property BackpropGradientsViewArray As INDArray
				Set(ByVal gradients As INDArray)
					Throw New System.NotSupportedException()
				End Set
			End Property


			Public Overrides WriteOnly Property CacheMode As CacheMode
				Set(ByVal mode As CacheMode)
					Throw New System.NotSupportedException()
				End Set
			End Property

			Public Overrides WriteOnly Property Listeners As TrainingListener()
				Set(ByVal listeners() As TrainingListener)
    
				End Set
			End Property

			Public Overrides ReadOnly Property Index As Integer
				Get
					Return 0
				End Get
			End Property

			Public Overrides Sub setInput(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)

			End Sub

			Public Overrides ReadOnly Property PretrainLayer As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides Sub clearNoiseWeightParams()

			End Sub
		End Class


		''' <summary>
		''' Simple abstract class to deal with the fact that we don't care about the majority of the Model/Layer
		''' methods here. Classes extending this model for optimizer tests need only implement the score() and
		''' gradient() methods.
		''' </summary>
		<Serializable>
		Private MustInherit Class SimpleOptimizableModel
			Implements Model, Layer

			Public MustOverride Sub clearNoiseWeightParams() Implements Layer.clearNoiseWeightParams
			Public MustOverride ReadOnly Property PretrainLayer As Boolean Implements Layer.isPretrainLayer
			Public MustOverride WriteOnly Property CacheMode Implements Layer.setCacheMode As CacheMode
			Public MustOverride WriteOnly Property BackpropGradientsViewArray Implements Model.setBackpropGradientsViewArray As INDArray
			Public MustOverride WriteOnly Property ParamsViewArray Implements Model.setParamsViewArray As INDArray
			Public MustOverride Function numParams(ByVal backwards As Boolean) As Long
			Friend Const serialVersionUID As Long = 4409380971404019303L
			Protected Friend parameters As INDArray
			Protected Friend gradientView As INDArray
'JAVA TO VB CONVERTER NOTE: The field conf was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend ReadOnly conf_Conflict As NeuralNetConfiguration
'JAVA TO VB CONVERTER NOTE: The field gradient was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend gradient_Conflict As Gradient
'JAVA TO VB CONVERTER NOTE: The field score was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend score_Conflict As Double

			'''<param name="parameterInit"> Initial parameters. Also determines dimensionality of problem. Should be row vector. </param>
			Friend Sub New(ByVal parameterInit As INDArray, ByVal conf As NeuralNetConfiguration)
				Me.parameters = parameterInit.dup()
				Me.gradientView = Nd4j.create(parameterInit.shape())
				Me.conf_Conflict = conf
			End Sub

			Public Overridable Sub addListeners(ParamArray ByVal listener() As TrainingListener) Implements Model.addListeners
				' no-op
			End Sub

			Public Overridable ReadOnly Property Config As TrainingConfig Implements org.deeplearning4j.nn.api.Trainable.getConfig
				Get
					Return conf_Conflict.getLayer()
				End Get
			End Property

			''' <summary>
			''' Init the model
			''' </summary>
			Public Overridable Sub init() Implements Model.init

			End Sub

			Public Overridable Property Index As Integer Implements Layer.getIndex
				Get
					Return 0
				End Get
				Set(ByVal index As Integer)
					Throw New System.NotSupportedException()
				End Set
			End Property

			Public Overridable Sub setInput(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) Implements Layer.setInput

			End Sub

			Public Overridable Sub fit() Implements Model.fit
				Throw New System.NotSupportedException()
			End Sub

			Public Overridable Sub update(ByVal gradient As INDArray, ByVal paramType As String) Implements Model.update
				If Not "W".Equals(paramType) Then
					Throw New System.NotSupportedException()
				End If
				parameters.subi(gradient)
			End Sub

			Public Overridable Property Listeners Implements Model.setListeners, Layer.setListeners As TrainingListener()
				Set(ByVal listeners() As TrainingListener)
    
				End Set
				Get
					Return Nothing
				End Get
			End Property

			Public Overridable Sub update(ByVal gradient As Gradient) Implements Model.update

			End Sub

			Public Overridable Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements Layer.activate
				Return Nothing
			End Function

			Public Overridable Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements Layer.activate
				Return Nothing
			End Function

			Public Overridable Function score() As Double Implements Model.score
				Return score_Conflict
			End Function

			Public Overridable Function gradient() As Gradient Implements Model.gradient
				Return gradient_Conflict
			End Function

			Public Overridable Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double Implements Layer.calcRegularizationScore
				Return 0
			End Function

			Public Overridable Sub computeGradientAndScore(ByVal workspaceMgr As LayerWorkspaceMgr) Implements Model.computeGradientAndScore
				Throw New System.NotSupportedException("Ensure you implement this function.")
			End Sub

			Public Overridable Function params() As INDArray Implements Model.params
				Return parameters
			End Function

			Public Overridable Function numParams() As Long Implements Model.numParams
				Return parameters.length()
			End Function

			Public Overridable WriteOnly Property Params Implements Model.setParams As INDArray
				Set(ByVal params As INDArray)
					Me.parameters = params
				End Set
			End Property

			Public Overridable Sub fit(ByVal data As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) Implements Model.fit
				Throw New System.NotSupportedException()
			End Sub

			Public Overridable Function gradientAndScore() As Pair(Of Gradient, Double) Implements Model.gradientAndScore
				computeGradientAndScore(LayerWorkspaceMgr.noWorkspaces())
				Return New Pair(Of Gradient, Double)(gradient(), score())
			End Function

			Public Overridable Function batchSize() As Integer Implements Model.batchSize
				Return 1
			End Function

			Public Overridable Function conf() As NeuralNetConfiguration Implements Model.conf
				Return conf_Conflict
			End Function

			Public Overridable WriteOnly Property Conf Implements Model.setConf As NeuralNetConfiguration
				Set(ByVal conf As NeuralNetConfiguration)
					Throw New System.NotSupportedException()
				End Set
			End Property

			Public Overridable Function input() As INDArray Implements Model.input
				'Work-around for BaseUpdater.postApply(): Uses Layer.input().size(0)
				'in order to get mini-batch size. i.e., divide by 1 here.
				Return Nd4j.zeros(1)
			End Function

			Public Overridable ReadOnly Property Optimizer As ConvexOptimizer Implements Model.getOptimizer
				Get
					Throw New System.NotSupportedException()
				End Get
			End Property

			Public Overridable Function getParam(ByVal param As String) As INDArray Implements Model.getParam
				Return parameters
			End Function

			Public Overridable Function paramTable() As IDictionary(Of String, INDArray) Implements Model.paramTable
				Return Collections.singletonMap("W", getParam("W"))
			End Function

			Public Overridable Function paramTable(ByVal backpropParamsOnly As Boolean) As IDictionary(Of String, INDArray) Implements Model.paramTable
				Return paramTable()
			End Function

			Public Overridable WriteOnly Property ParamTable Implements Model.setParamTable As IDictionary(Of String, INDArray)
				Set(ByVal paramTable As IDictionary(Of String, INDArray))
					Throw New System.NotSupportedException()
				End Set
			End Property

			Public Overridable Sub setParam(ByVal key As String, ByVal val As INDArray) Implements Model.setParam
				Throw New System.NotSupportedException()
			End Sub

			Public Overridable Sub clear() Implements Model.clear
				Throw New System.NotSupportedException()
			End Sub

			Public Overridable Function type() As Type Implements Layer.type
				Throw New System.NotSupportedException()
			End Function

			Public Overridable Function backpropGradient(ByVal epsilon As INDArray, ByVal mgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray) Implements Layer.backpropGradient
				Throw New System.NotSupportedException()
			End Function


			Public Overridable WriteOnly Property Listeners Implements Model.setListeners, Layer.setListeners As ICollection(Of TrainingListener)
				Set(ByVal listeners As ICollection(Of TrainingListener))
					Throw New System.NotSupportedException()
				End Set
			End Property


			Public Overridable Property InputMiniBatchSize Implements Layer.setInputMiniBatchSize As Integer
				Set(ByVal size As Integer)
				End Set
				Get
					Return 1
				End Get
			End Property

			Public Overridable Property MaskArray Implements Layer.setMaskArray As INDArray
				Set(ByVal maskArray As INDArray)
				End Set
				Get
					Return Nothing
				End Get
			End Property

			Public Overridable Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState) Implements Layer.feedForwardMaskArray
				Throw New System.NotSupportedException()
			End Function

			Public Overridable ReadOnly Property GradientsViewArray As INDArray Implements Model.getGradientsViewArray
				Get
					Return gradientView
				End Get
			End Property

			Public Overridable Sub applyConstraints(ByVal iteration As Integer, ByVal epoch As Integer) Implements Model.applyConstraints

			End Sub

			Public Overridable Property IterationCount As Integer Implements Layer.getIterationCount
				Get
					Return 0
				End Get
				Set(ByVal iterationCount As Integer)
    
				End Set
			End Property

			Public Overridable Property EpochCount As Integer Implements Layer.getEpochCount
				Get
					Return 0
				End Get
				Set(ByVal epochCount As Integer)
    
				End Set
			End Property



			Public Overridable Sub allowInputModification(ByVal allow As Boolean) Implements Layer.allowInputModification

			End Sub

			Public Overridable ReadOnly Property Helper As LayerHelper Implements Layer.getHelper
				Get
					Return Nothing
				End Get
			End Property

			Public Overridable Function updaterDivideByMinibatch(ByVal paramName As String) As Boolean Implements org.deeplearning4j.nn.api.Trainable.updaterDivideByMinibatch
				Return True
			End Function

			Public Overridable Sub close() Implements Model.close
			End Sub
		End Class
	End Class

End Namespace