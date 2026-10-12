Imports System
Imports Microsoft.VisualBasic
Imports FastMath = org.apache.commons.math3.util.FastMath
Imports InvalidStepException = org.deeplearning4j.exception.InvalidStepException
Imports Model = org.deeplearning4j.nn.api.Model
Imports NegativeGradientStepFunction = org.deeplearning4j.nn.conf.stepfunctions.NegativeGradientStepFunction
Imports ConvexOptimizer = org.deeplearning4j.optimize.api.ConvexOptimizer
Imports LineOptimizer = org.deeplearning4j.optimize.api.LineOptimizer
Imports StepFunction = org.deeplearning4j.optimize.api.StepFunction
Imports NegativeDefaultStepFunction = org.deeplearning4j.optimize.stepfunctions.NegativeDefaultStepFunction
Imports Level1 = org.nd4j.linalg.api.blas.Level1
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ScalarSetValue = org.nd4j.linalg.api.ops.impl.scalar.comparison.ScalarSetValue
Imports Eps = org.nd4j.linalg.api.ops.impl.transforms.comparison.Eps
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
import static org.nd4j.linalg.ops.transforms.Transforms.abs

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

Namespace org.deeplearning4j.optimize.solvers


	<Serializable>
	Public Class BackTrackLineSearch
		Implements LineOptimizer

		Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(BackTrackLineSearch))
		Private layer As Model
		Private stepFunction As StepFunction
		Private optimizer As ConvexOptimizer
'JAVA TO VB CONVERTER NOTE: The field maxIterations was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private maxIterations_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field stepMax was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Friend stepMax_Conflict As Double = 100
		Private minObjectiveFunction As Boolean = True

		' termination conditions: either
		'   a) abs(delta x/x) < REL_TOLX for all coordinates
		'   b) abs(delta x) < ABS_TOLX for all coordinates
		'   c) sufficient function increase (uses ALF)
'JAVA TO VB CONVERTER NOTE: The field relTolx was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private relTolx_Conflict As Double = 1e-7f
'JAVA TO VB CONVERTER NOTE: The field absTolx was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private absTolx_Conflict As Double = 1e-4f ' tolerance on absolute value difference
		Protected Friend ReadOnly ALF As Double = 1e-4f

		''' <param name="layer"> </param>
		''' <param name="stepFunction"> </param>
		''' <param name="optimizer"> </param>
		Public Sub New(ByVal layer As Model, ByVal stepFunction As StepFunction, ByVal optimizer As ConvexOptimizer)
			Me.layer = layer
			Me.stepFunction = stepFunction
			Me.optimizer = optimizer
			Me.maxIterations_Conflict = layer.conf().getMaxNumLineSearchIterations()
		End Sub

		''' <param name="optimizable"> </param>
		''' <param name="optimizer"> </param>
		Public Sub New(ByVal optimizable As Model, ByVal optimizer As ConvexOptimizer)
			Me.New(optimizable, New NegativeDefaultStepFunction(), optimizer)
		End Sub


		Public Overridable Property StepMax As Double
			Set(ByVal stepMax As Double)
				Me.stepMax_Conflict = stepMax
			End Set
			Get
				Return stepMax_Conflict
			End Get
		End Property



		''' <summary>
		''' Sets the tolerance of relative diff in function value.
		''' Line search converges if abs(delta x / x) < tolx
		''' for all coordinates.
		''' </summary>
		Public Overridable WriteOnly Property RelTolx As Double
			Set(ByVal tolx As Double)
				relTolx_Conflict = tolx
			End Set
		End Property

		''' <summary>
		''' Sets the tolerance of absolute diff in function value.
		''' Line search converges if abs(delta x) < tolx
		''' for all coordinates.
		''' </summary>
		Public Overridable WriteOnly Property AbsTolx As Double
			Set(ByVal tolx As Double)
				absTolx_Conflict = tolx
			End Set
		End Property

		Public Overridable Property MaxIterations As Integer
			Get
				Return maxIterations_Conflict
			End Get
			Set(ByVal maxIterations As Integer)
				Me.maxIterations_Conflict = maxIterations
			End Set
		End Property


		Public Overridable Function setScoreFor(ByVal parameters As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Double
			layer.Params = parameters
			layer.computeGradientAndScore(workspaceMgr)
			Return layer.score()
		End Function

		' returns fraction of step size if found a good step
		' returns 0.0 if could not step in direction
		' step == alam and score == f in book

		''' <param name="parameters">      the parameters to optimize </param>
		''' <param name="gradients">       the line/rate of change </param>
		''' <param name="searchDirection"> the point for the line search to go in </param>
		''' <returns> the next step size </returns>
		''' <exception cref="InvalidStepException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public double optimize(org.nd4j.linalg.api.ndarray.INDArray parameters, org.nd4j.linalg.api.ndarray.INDArray gradients, org.nd4j.linalg.api.ndarray.INDArray searchDirection, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr workspaceMgr) throws org.deeplearning4j.exception.InvalidStepException
		Public Overridable Function optimize(ByVal parameters As INDArray, ByVal gradients As INDArray, ByVal searchDirection As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Double Implements LineOptimizer.optimize
			Dim test, stepMin, [step], step2, oldStep, tmpStep As Double
			Dim rhs1, rhs2, a, b, disc, score, scoreAtStart, score2 As Double
			minObjectiveFunction = (TypeOf stepFunction Is NegativeDefaultStepFunction OrElse TypeOf stepFunction Is NegativeGradientStepFunction)

			Dim l1Blas As Level1 = Nd4j.BlasWrapper.level1()

			Dim sum As Double = l1Blas.nrm2(searchDirection)
			Dim slope As Double = -1f * Nd4j.BlasWrapper.dot(searchDirection, gradients)

			log.debug("slope = {}", slope)

			Dim maxOldParams As INDArray = abs(parameters)
			Nd4j.Executioner.exec(New ScalarSetValue(maxOldParams, 1))
			Dim testMatrix As INDArray = abs(gradients).divi(maxOldParams)
			test = testMatrix.max(Integer.MaxValue).getDouble(0)

			[step] = 1.0 ' initially, step = 1.0, i.e. take full Newton step
			stepMin = relTolx_Conflict / test ' relative convergence tolerance
			oldStep = 0.0
			step2 = 0.0

'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: score = score2 = scoreAtStart = layer.score();
			scoreAtStart = layer.score()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: score = score2 = scoreAtStart
				score2 = scoreAtStart
					score = score2
			Dim bestScore As Double = score
			Dim bestStepSize As Double = 1.0

			If log.isTraceEnabled() Then
				Dim norm1 As Double = l1Blas.asum(searchDirection)
				Dim infNormIdx As Integer = l1Blas.iamax(searchDirection)
				Dim infNorm As Double = FastMath.max(Single.NegativeInfinity, searchDirection.getDouble(infNormIdx))
				log.trace("ENTERING BACKTRACK" & vbLf)
				log.trace("Entering BackTrackLineSearch, value = " & scoreAtStart & "," & vbLf & "direction.oneNorm:" & norm1 & "  direction.infNorm:" & infNorm)
			End If
			If sum > stepMax_Conflict Then
				log.warn("Attempted step too big. scaling: sum= {}, stepMax= {}", sum, stepMax_Conflict)
				searchDirection.muli(stepMax_Conflict / sum)
			End If

			'        if (slope >= 0.0) {
			'            throw new InvalidStepException("Slope " + slope + " is >= 0.0. Expect slope < 0.0 when minimizing objective function");
			'        }

			' find maximum lambda
			' converge when (delta x) / x < REL_TOLX for all coordinates.
			' the largest step size that triggers this threshold is precomputed and saved in stepMin
			' look for step size in direction given by "line"
			Dim candidateParameters As INDArray = Nothing
			For iteration As Integer = 0 To maxIterations_Conflict - 1
				If log.isTraceEnabled() Then
					log.trace("BackTrack loop iteration {} : step={}, oldStep={}", iteration, [step], oldStep)
					log.trace("before step, x.1norm: {} " & vbLf & "step: {} " & vbLf & "oldStep: {}", parameters.norm1(Integer.MaxValue), [step], oldStep)
				End If

				If [step] = oldStep Then
					Throw New System.ArgumentException("Current step == oldStep")
				End If

				' step
				candidateParameters = parameters.dup("f"c)
				stepFunction.step(candidateParameters, searchDirection, [step])
				oldStep = [step]

				If log.isTraceEnabled() Then
					Dim norm1 As Double = l1Blas.asum(candidateParameters)
					log.trace("after step, x.1norm: " & norm1)
				End If

				' check for convergence on delta x
				If ([step] < stepMin) OrElse Nd4j.Executioner.exec(New Eps(parameters, candidateParameters,Nd4j.createUninitialized(DataType.BOOL, candidateParameters.shape(), candidateParameters.ordering()))).castTo(DataType.FLOAT).sumNumber().longValue() = candidateParameters.length() Then
					score = setScoreFor(parameters, workspaceMgr)
					log.debug("EXITING BACKTRACK: Jump too small (stepMin = {}). Exiting and using original params. Score = {}", stepMin, score)
					Return 0.0
				End If

				score = setScoreFor(candidateParameters, workspaceMgr)
				log.debug("Model score after step = {}", score)

				'Score best step size for use if we terminate on maxIterations
				If (minObjectiveFunction AndAlso score < bestScore) OrElse (Not minObjectiveFunction AndAlso score > bestScore) Then
					bestScore = score
					bestStepSize = [step]
				End If

				'Sufficient decrease in cost/loss function (Wolfe condition / Armijo condition)
				If minObjectiveFunction AndAlso score <= scoreAtStart + ALF * [step] * slope Then
					log.debug("Sufficient decrease (Wolfe cond.), exiting backtrack on iter {}: score={}, scoreAtStart={}", iteration, score, scoreAtStart)
					If score > scoreAtStart Then
						Throw New System.InvalidOperationException("Function did not decrease: score = " & score & " > " & scoreAtStart & " = oldScore")
					End If
					Return [step]
				End If

				'Sufficient increase in cost/loss function (Wolfe condition / Armijo condition)
				If Not minObjectiveFunction AndAlso score >= scoreAtStart + ALF * [step] * slope Then
					log.debug("Sufficient increase (Wolfe cond.), exiting backtrack on iter {}: score={}, bestScore={}", iteration, score, scoreAtStart)
					If score < scoreAtStart Then
						Throw New System.InvalidOperationException("Function did not increase: score = " & score & " < " & scoreAtStart & " = scoreAtStart")
					End If
					Return [step]

				' if value is infinite, i.e. we've jumped to unstable territory, then scale down jump
				ElseIf Double.IsInfinity(score) OrElse Double.IsInfinity(score2) OrElse Double.IsNaN(score) OrElse Double.IsNaN(score2) Then
					log.warn("Value is infinite after jump. oldStep={}. score={}, score2={}. Scaling back step size...", oldStep, score, score2)
					tmpStep = .2 * [step]
					If [step] < stepMin Then 'convergence on delta x
						score = setScoreFor(parameters, workspaceMgr)
						log.warn("EXITING BACKTRACK: Jump too small (step={} < stepMin={}). Exiting and using previous parameters. Value={}", [step], stepMin, score)
						Return 0.0
					End If

				' backtrack

				ElseIf minObjectiveFunction Then
					If [step] = 1.0 Then ' first time through
						tmpStep = -slope / (2.0 * (score - scoreAtStart - slope))
					Else
						rhs1 = score - scoreAtStart - [step] * slope
						rhs2 = score2 - scoreAtStart - step2 * slope
						If [step] = step2 Then
							Throw New System.InvalidOperationException("FAILURE: dividing by step-step2 which equals 0. step=" & [step])
						End If
						Dim stepSquared As Double = [step] * [step]
						Dim step2Squared As Double = step2 * step2
						a = (rhs1 / stepSquared - rhs2 / step2Squared) / ([step] - step2)
						b = (-step2 * rhs1 / stepSquared + [step] * rhs2 / step2Squared) / ([step] - step2)
						If a = 0.0 Then
							tmpStep = -slope / (2.0 * b)
						Else
							disc = b * b - 3.0 * a * slope
							If disc < 0.0 Then
								tmpStep = 0.5 * [step]
							ElseIf b <= 0.0 Then
								tmpStep = (-b + FastMath.sqrt(disc)) / (3.0 * a)
							Else
								tmpStep = -slope / (b + FastMath.sqrt(disc))
							End If
						End If
						If tmpStep > 0.5 * [step] Then
							tmpStep = 0.5 * [step] ' lambda <= 0.5 lambda_1
						End If
					End If
				Else
					If [step] = 1.0 Then ' first time through
						tmpStep = -slope / (2.0 * (scoreAtStart - score - slope))
					Else
						rhs1 = scoreAtStart - score - [step] * slope
						rhs2 = scoreAtStart - score2 - step2 * slope
						If [step] = step2 Then
							Throw New System.InvalidOperationException("FAILURE: dividing by step-step2 which equals 0. step=" & [step])
						End If
						Dim stepSquared As Double = [step] * [step]
						Dim step2Squared As Double = step2 * step2
						a = (rhs1 / stepSquared - rhs2 / step2Squared) / ([step] - step2)
						b = (-step2 * rhs1 / stepSquared + [step] * rhs2 / step2Squared) / ([step] - step2)
						If a = 0.0 Then
							tmpStep = -slope / (2.0 * b)
						Else
							disc = b * b - 3.0 * a * slope
							If disc < 0.0 Then
								tmpStep = 0.5 * [step]
							ElseIf b <= 0.0 Then
								tmpStep = (-b + FastMath.sqrt(disc)) / (3.0 * a)
							Else
								tmpStep = -slope / (b + FastMath.sqrt(disc))
							End If
						End If
						If tmpStep > 0.5 * [step] Then
							tmpStep = 0.5 * [step] ' lambda <= 0.5 lambda_1
						End If
					End If

				End If

				step2 = [step]
				score2 = score
				log.debug("tmpStep: {}", tmpStep)
				[step] = Math.Max(tmpStep, .1f * [step]) ' lambda >= .1*Lambda_1
			Next iteration


			If minObjectiveFunction AndAlso bestScore < scoreAtStart Then
				'Return best step size
				log.debug("Exited line search after maxIterations termination condition; bestStepSize={}, bestScore={}, scoreAtStart={}", bestStepSize, bestScore, scoreAtStart)
				Return bestStepSize
			ElseIf Not minObjectiveFunction AndAlso bestScore > scoreAtStart Then
				'Return best step size
				log.debug("Exited line search after maxIterations termination condition; bestStepSize={}, bestScore={}, scoreAtStart={}", bestStepSize, bestScore, scoreAtStart)
				Return bestStepSize
			Else
				log.debug("Exited line search after maxIterations termination condition; score did not improve (bestScore={}, scoreAtStart={}). Resetting parameters", bestScore, scoreAtStart)
				setScoreFor(parameters, workspaceMgr)
				Return 0.0
			End If
		End Function



	End Class


End Namespace