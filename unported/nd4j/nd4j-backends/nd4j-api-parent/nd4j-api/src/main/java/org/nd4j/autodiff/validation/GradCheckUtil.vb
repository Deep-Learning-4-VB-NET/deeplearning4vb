Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports Listener = org.nd4j.autodiff.listeners.Listener
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports VariableType = org.nd4j.autodiff.samediff.VariableType
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Variable = org.nd4j.autodiff.samediff.internal.Variable
Imports NonInplaceValidationListener = org.nd4j.autodiff.validation.listeners.NonInplaceValidationListener
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports NdIndexIterator = org.nd4j.linalg.api.iter.NdIndexIterator
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.autodiff.validation


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class GradCheckUtil
	Public Class GradCheckUtil

		Public Enum Subset
			EVERY_N
			RANDOM

		End Enum
		Public Const DEFAULT_PRINT As Boolean = False
		Public Const DEFAULT_EXIT_FIRST_FAILURE As Boolean = False
		Public Const DEFAULT_DEBUG_MODE As Boolean = False
		Public Const DEFAULT_EPS As Double = 1e-5
		Public Const DEFAULT_MAX_REL_ERROR As Double = 1e-5
		Public Const DEFAULT_MIN_ABS_ERROR As Double = 1e-6

		Public Shared Function checkGradients(ByVal t As TestCase) As Boolean
			Return checkGradients(t.sameDiff(), t.placeholderValues(), t.gradCheckEpsilon(), t.gradCheckMaxRelativeError(), t.gradCheckMinAbsError(), t.gradCheckPrint(), t.gradCheckDefaultExitFirstFailure(), False, t.gradCheckDebugMode(), t.gradCheckSkipVariables(), t.gradCheckMask())
		End Function

		Public Shared Function checkGradients(ByVal sd As SameDiff, ByVal placeholderValues As IDictionary(Of String, INDArray), ParamArray ByVal skipVariables() As String) As Boolean
			Dim skip As ISet(Of String) = Nothing
			If skipVariables IsNot Nothing Then
				skip = New HashSet(Of String)()
				Collections.addAll(skip, skipVariables)
			End If
			Return checkGradients(sd, placeholderValues, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, DEFAULT_PRINT, DEFAULT_EXIT_FIRST_FAILURE, False, DEFAULT_DEBUG_MODE, skip, Nothing)
		End Function

		Public Shared Function checkGradients(ByVal sd As SameDiff, ByVal placeholderValues As IDictionary(Of String, INDArray), ByVal print As Boolean, ByVal exitOnFirstFailure As Boolean) As Boolean
			Return checkGradients(sd, placeholderValues, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, print, exitOnFirstFailure)
		End Function


		Public Shared Function checkGradients(ByVal sd As SameDiff, ByVal placeholderValues As IDictionary(Of String, INDArray), ByVal eps As Double, ByVal maxRelError As Double, ByVal minAbsError As Double, ByVal print As Boolean, ByVal exitOnFirstFailure As Boolean) As Boolean
			Return checkGradients(sd, placeholderValues, eps, maxRelError, minAbsError, print, exitOnFirstFailure, False, DEFAULT_DEBUG_MODE, Nothing, Nothing)
		End Function

		Public Shared Function checkGradients(ByVal sd As SameDiff, ByVal placeholderValues As IDictionary(Of String, INDArray), ByVal eps As Double, ByVal maxRelError As Double, ByVal minAbsError As Double, ByVal print As Boolean, ByVal exitOnFirstFailure As Boolean, ByVal skipValidation As Boolean, ByVal debugMode As Boolean, ByVal skipVariables As ISet(Of String), ByVal gradCheckMask As IDictionary(Of String, INDArray)) As Boolean
			Return checkGradients(sd, placeholderValues, eps, maxRelError, minAbsError, print, exitOnFirstFailure, skipValidation, debugMode, skipVariables, gradCheckMask, -1, Nothing)
		End Function

		Public Shared Function checkGradients(ByVal sd As SameDiff, ByVal placeholderValues As IDictionary(Of String, INDArray), ByVal eps As Double, ByVal maxRelError As Double, ByVal minAbsError As Double, ByVal print As Boolean, ByVal exitOnFirstFailure As Boolean, ByVal skipValidation As Boolean, ByVal debugMode As Boolean, ByVal skipVariables As ISet(Of String), ByVal gradCheckMask As IDictionary(Of String, INDArray), ByVal maxPerParam As Integer, ByVal subset As Subset) As Boolean

			Dim debugBefore As Boolean = sd.isDebugMode()
			If debugMode Then
				sd.enableDebugMode()
			End If

			'Validation sanity checks:
			If Not skipValidation Then
				validateInternalState(sd, True)
			End If

			'Check data type:
			If Nd4j.dataType() <> DataType.DOUBLE Then
				Throw New System.InvalidOperationException("Data type must be set to double")
			End If

			Dim fnOutputs As ISet(Of String) = New HashSet(Of String)()
			For Each f As DifferentialFunction In sd.ops()
				For Each s As SDVariable In f.outputVariables()
					fnOutputs.Add(s.name())
				Next s
			Next f

			'Check that all non-Array type SDVariables have arrays associated with them
			For Each v As Variable In sd.getVariables().values()
				If v.getVariable().getVariableType() = VariableType.ARRAY Then
					'OK if variable is not available for this, it'll be created during forward pass
					Continue For
				End If

				If v.getVariable().getArr(True) Is Nothing Then
					Throw New System.InvalidOperationException("Variable """ & v.getName() & """ does not have array associated with it")
				End If
			Next v

			'Do forward pass, check that output is a scalar:
			Dim lossFnVariables As IList(Of String) = sd.getLossVariables()
			Preconditions.checkState(lossFnVariables IsNot Nothing AndAlso lossFnVariables.Count > 0, "Expected 1 or more loss function variables for gradient check, got %s", lossFnVariables)

			'TODO also check that all inputs are non-zero (otherwise: consider out = sum(x * y) with all x and y being 0
			' in this case, gradients of x and y are all 0 too

			'Collect variables to get gradients for - we want placeholders AND variables
			Dim varsNeedingGrads As ISet(Of String) = New HashSet(Of String)()
			For Each v As Variable In sd.getVariables().values()
				If v.getVariable().dataType().isFPType() AndAlso (v.getVariable().getVariableType() = VariableType.VARIABLE OrElse v.getVariable().getVariableType() = VariableType.PLACEHOLDER) Then
					Dim g As SDVariable = v.getVariable().getGradient()
					Preconditions.checkNotNull(g, "No gradient variable found for variable %s", v.getVariable())
					varsNeedingGrads.Add(v.getName())
				End If
			Next v

			'Add non-inplace validation listener, to check that non-inplace ops don't modify their inputs
			Dim listenersBefore As IList(Of Listener) = New List(Of Listener)(sd.getListeners())
			Dim listenerIdx As Integer = -1
			If listenersBefore.Count = 0 Then
				sd.addListeners(New NonInplaceValidationListener())
				listenerIdx = 0
			Else
				Dim found As Boolean = False
				Dim i As Integer = 0
				For Each l As Listener In listenersBefore
					If TypeOf l Is NonInplaceValidationListener Then
						found = True
						listenerIdx = i
						Exit For
					End If
					i += 1
				Next l
				If Not found Then
					sd.addListeners(New NonInplaceValidationListener())
					listenerIdx = i
				End If
			End If


			Dim gm As IDictionary(Of String, INDArray) = sd.calculateGradients(placeholderValues, varsNeedingGrads)

			'Remove listener, to reduce overhead
			sd.getListeners().RemoveAt(listenerIdx)

			Dim grad As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			For Each v As SDVariable In sd.variables()
				If fnOutputs.Contains(v.name()) Then
					'This is not an input to the graph
					Continue For
				End If
				If Not v.hasGradient() Then
					'Skip non-fp variables, or variables that don't impact loss function value
					Continue For
				End If
				Dim g As SDVariable = sd.grad(v.name())
				If g Is Nothing Then
					Throw New System.InvalidOperationException("Null gradient variable for """ & v.name() & """")
				End If
				Dim ga As INDArray = gm(v.name())
				If ga Is Nothing Then
					Throw New System.InvalidOperationException("Null gradient array encountered for variable: " & v.name())
				End If
				If Not v.Arr.shape().SequenceEqual(ga.shape()) Then
					Throw New System.InvalidOperationException("Gradient shape does not match variable shape for variable """ & v.name() & """: shape " & java.util.Arrays.toString(v.Arr.shape()) & " vs. gradient shape " & java.util.Arrays.toString(ga.shape()))
				End If
				grad(v.name()) = ga.dup()
			Next v

			'Validate gradients for each variable:
			Dim totalNFailures As Integer = 0
			Dim totalCount As Integer = 0
			Dim maxError As Double = 0.0
			Dim r As New Random(12345)
			For Each s As SDVariable In sd.variables()
				If fnOutputs.Contains(s.name()) OrElse Not s.dataType().isFPType() Then
					'This is not an input to the graph, or is not a floating point input (so can't be gradient checked)
					Continue For
				End If

				If skipVariables IsNot Nothing AndAlso skipVariables.Contains(s.name()) Then
					log.info("Grad check: skipping variable ""{}""", s.name())
					Continue For
				End If

				If s.dataType() <> DataType.DOUBLE Then
					log.warn("DataType for variable {} is not double (is: {}) may cause precision issues in gradient checks", s.name(), s.dataType())
				End If

				Dim name As String = s.name()
				Dim a As INDArray = s.Arr
				Dim n As Long = a.length()
				If print Then
					log.info("Starting test for variable ""{}"" with {} values", s.name(), n)
				End If

				Dim iter As IEnumerator(Of Long())
				If maxPerParam > 0 AndAlso subset <> Nothing AndAlso maxPerParam < a.length() Then
					'Subset case
					Dim shape() As Long = a.shape()
					Dim l As IList(Of Long()) = New List(Of Long())()
					If subset = Subset.RANDOM Then
						Dim set As ISet(Of Integer) = New HashSet(Of Integer)()
						Do While set.Count < maxPerParam
							Dim [next] As Integer = r.Next(CInt(a.length()))
							set.Add([next])
						Loop
						Dim sorted As IList(Of Integer) = New List(Of Integer)(set)
						sorted.Sort()

						For Each i As Integer? In sorted
							Dim pos() As Long = Shape.ind2subC(shape, i)
							l.Add(pos)
						Next i
					Else
						'Every N
						Dim everyN As Long = n \ maxPerParam
						Dim curr As Long = 0
						Do While curr < n
							Dim pos() As Long = Shape.ind2subC(shape, curr)
							l.Add(pos)
							curr += everyN
						Loop
					End If
					iter = l.GetEnumerator()
				Else
					'Standard case: do all parameters
					iter = New NdIndexIterator("c"c,a.shape())
				End If

				Dim varMask As INDArray = (If(gradCheckMask Is Nothing, Nothing, gradCheckMask(s.name())))

				If varMask IsNot Nothing Then
					Preconditions.checkState(a.equalShapes(varMask), "Variable ""%s"": Gradient check mask and array shapes must be equal: got %s vs. mask shape %s", s.name(), a.shape(), varMask.shape())
					Preconditions.checkState(varMask.dataType() = DataType.BOOL, "Variable ""%s"": Gradient check mask must be BOOLEAN datatype, got %s", s.name(), varMask.dataType())
				End If

				Dim i As Integer = 0
				Do While iter.MoveNext()
					Dim idx() As Long = iter.Current
					Dim strIdx As String = Nothing
					If print Then
						strIdx = java.util.Arrays.toString(idx).replaceAll(" ","")
					End If

					Dim maskValue As Boolean = (varMask Is Nothing OrElse (varMask.getDouble(idx) <> 0))
					If Not maskValue Then
						'Skip this specific entry (masked out)
						Continue Do
					End If

					totalCount += 1
					Dim orig As Double = a.getDouble(idx)
					a.putScalar(idx, orig+eps)
					Dim scorePlus As Double = 0.0
					Dim m As IDictionary(Of String, INDArray) = sd.output(placeholderValues, lossFnVariables) '.get(outName).sumNumber().doubleValue();
					For Each arr As INDArray In m.Values
						scorePlus += arr.sumNumber().doubleValue()
					Next arr
					a.putScalar(idx, orig-eps)
					m = sd.output(placeholderValues, lossFnVariables)
					Dim scoreMinus As Double = 0.0
					For Each arr As INDArray In m.Values
						scoreMinus += arr.sumNumber().doubleValue()
					Next arr
					a.putScalar(idx, orig)

					Dim numericalGrad As Double = (scorePlus - scoreMinus) / (2 * eps)
					Dim aGrad As INDArray = grad(s.name())
					If aGrad Is Nothing Then
						log.warn("No gradient array for variable ""{}"" was found, skipping variable...", s.name())
						Continue Do
					End If
					Dim analyticGrad As Double = aGrad.getDouble(idx)

					If Double.IsInfinity(numericalGrad) OrElse Double.IsNaN(numericalGrad) Then
						Throw New System.InvalidOperationException("Numerical gradient was " & numericalGrad & " for variable """ & name & """, parameter " & i & " of " & n & " (position: " & strIdx & ")")
					End If
					If Double.IsInfinity(analyticGrad) OrElse Double.IsNaN(analyticGrad) Then
						Throw New System.InvalidOperationException("Analytic (SameDiff) gradient was " & analyticGrad & " for variable """ & name & """, parameter " & i & " of " & n & " (position: " & strIdx & ")")
					End If


					Dim relError As Double
					If numericalGrad = 0.0 AndAlso analyticGrad = 0.0 Then
						relError = 0.0
					Else
						relError = Math.Abs(analyticGrad - numericalGrad) / (Math.Abs(Math.Abs(analyticGrad) + Math.Abs(numericalGrad)))
					End If

					If relError > maxError Then
						maxError = relError
					End If

					If relError > maxRelError OrElse Double.IsNaN(relError) Then
						Dim absError As Double = Math.Abs(analyticGrad - numericalGrad)
						If absError < minAbsError Then
							If print Then
								log.info("Param " & i & " (" & name & strIdx & ") passed: grad= " & analyticGrad & ", numericalGrad= " & numericalGrad & ", relError= " & relError & "; absolute error = " & absError & " < minAbsoluteError = " & minAbsError)
							End If
						Else
							log.info("Param " & i & " (" & name & strIdx & ") FAILED: grad= " & analyticGrad & ", numericalGrad= " & numericalGrad & ", relError= " & relError & ", absError=" & absError & ", scorePlus=" & scorePlus & ", scoreMinus= " & scoreMinus)
							If exitOnFirstFailure Then
								Return False
							End If
							totalNFailures += 1
						End If
					ElseIf print Then
						log.info("Param " & i & " (" & name & strIdx & ") passed: grad= " & analyticGrad & ", numericalGrad= " & numericalGrad & ", relError= " & relError)
					End If
					i += 1
				Loop
			Next s

			Dim nPass As Integer = totalCount - totalNFailures
			log.info("GradCheckUtil.checkGradients(): " & totalCount & " params checked, " & nPass & " passed, " & totalNFailures & " failed. Largest relative error = " & maxError)

			If debugMode AndAlso Not debugBefore Then
				sd.disableDebugging()
			End If

			Return totalNFailures = 0
		End Function


		''' <summary>
		''' Gradient check the ACTIVATIONS (i.e., ARRAY type SDVariables) as opposed to the parameters of a network (as
		''' are tested in <seealso cref="checkGradients(SameDiff, Map, Double, Double, Double, Boolean, Boolean, Boolean, Boolean, Set, Map, Integer, Subset)"/> </summary>
		''' <param name="config"> Configuration for gradient check </param>
		''' <returns> True if gradient checks pass </returns>
		Public Shared Function checkActivationGradients(ByVal config As ActGradConfig) As Boolean
			Dim sd As SameDiff = config.getSd()
			Dim actGrads As IList(Of String) = config.getActivationGradsToCheck()
			Dim maxRelError As Double = config.getMaxRelError()
			Dim minAbsError As Double = config.getMinAbsError()

			Preconditions.checkState(sd IsNot Nothing, "SameDiff instance was not set in configuration")
			Preconditions.checkState(actGrads IsNot Nothing AndAlso actGrads.Count > 0, "No activation gradients were specified to gradient check")
			Preconditions.checkState(config.getEps() > 0.0, "Epsilon has not been set")
			Preconditions.checkState(maxRelError > 0.0, "Max relative error must be set (is 0.0)")

			For Each s As String In actGrads
				Dim v As SDVariable = sd.getVariables().get(s).getVariable()
				Preconditions.checkState(v IsNot Nothing, "No variable with name ""%s"" was found", s)
				Preconditions.checkState(v.getVariableType() = VariableType.ARRAY, "Only variables with type ARRAY may be " & "gradient checked using this method. Variable ""%s"" has type %s", s, v.getVariableType())
				Preconditions.checkState(v.dataType().isFPType(), "Cannot gradient check activation variable ""%s"": must be floating point type. Is type: %s", s, v.dataType())
				If v.dataType() <> DataType.DOUBLE Then
					log.warn("Floating point variable {} is not double precision - this may result in spurious failures due to limited precision. Variable is type: {}", s, v.dataType())
				End If
			Next s

			Dim debugBefore As Boolean = sd.isDebugMode()
			If config.isDebugMode() Then
				sd.enableDebugMode()
			End If

			'Validation sanity checks:
			If Not config.isSkipValidation() Then
				validateInternalState(sd, True)
			End If

			'Loss function variables
			Dim lossFnVariables As IList(Of String) = sd.getLossVariables()
			Preconditions.checkState(lossFnVariables IsNot Nothing AndAlso lossFnVariables.Count > 0, "Expected 1 or more loss function variables for gradient check, got %s", lossFnVariables)

			'TODO also check that all inputs are non-zero (otherwise: consider out = sum(x * y) with all x and y being 0
			' in this case, gradients of x and y are all 0 too

			'Collect names of variables to get gradients for - i.e., the names of the GRADIENT variables for the specified activations
			sd.createGradFunction()
			Dim varsRequiringGrads As ISet(Of String) = New HashSet(Of String)()
			For Each s As String In actGrads
				Dim grad As SDVariable = sd.getVariable(s).gradient()
				Preconditions.checkState(grad IsNot Nothing,"Could not get gradient for activation ""%s"": gradient variable is null", s)
				varsRequiringGrads.Add(s)
			Next s

			'Calculate analytical gradients
			Dim grads As IDictionary(Of String, INDArray) = sd.calculateGradients(config.getPlaceholderValues(), New List(Of String, INDArray)(varsRequiringGrads))
			Dim gradientsForAct As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			For Each s As String In actGrads
				Dim arr As INDArray = grads(s)
				Preconditions.checkState(arr IsNot Nothing, "No activation gradient array for variable ""%s""", s)
				gradientsForAct(s) = arr.dup()
			Next s


			'Now, check gradients
			Dim totalNFailures As Integer = 0
			Dim totalCount As Integer = 0
			Dim maxError As Double = 0.0
			Dim listener As New ActivationGradientCheckListener()
			sd.setListeners(listener)
			Dim r As New Random(12345)
			Dim maxPerParam As Integer = config.getMaxPerParam()
			For Each s As String In actGrads

				Dim n As Long = gradientsForAct(s).length()
				If config.isPrint() Then
					log.info("Starting test for variable ""{}"" with {} values", s, n)
				End If

				Dim iter As IEnumerator(Of Long())
				If maxPerParam > 0 AndAlso config.getSubset() IsNot Nothing AndAlso maxPerParam < n Then
					'Subset case
					Dim shape() As Long = gradientsForAct(s).shape()
					Dim l As IList(Of Long()) = New List(Of Long())()
					If config.getSubset() = Subset.RANDOM Then
						Dim set As ISet(Of Integer) = New HashSet(Of Integer)()
						Do While set.Count < maxPerParam
							Dim [next] As Integer = r.Next(CInt(n))
							set.Add([next])
						Loop
						Dim sorted As IList(Of Integer) = New List(Of Integer)(set)
						sorted.Sort()

						For Each i As Integer? In sorted
							Dim pos() As Long = Shape.ind2subC(shape, i)
							l.Add(pos)
						Next i
					Else
						'Every N
						Dim everyN As Long = n \ maxPerParam
						Dim curr As Long = 0
						Do While curr < n
							Dim pos() As Long = Shape.ind2subC(shape, curr)
							l.Add(pos)
							curr += everyN
						Loop
					End If
					iter = l.GetEnumerator()
				Else
					'Standard case: do all parameters
					iter = New NdIndexIterator("c"c,gradientsForAct(s).shape())
				End If

				Dim varMask As INDArray = (If(config.getGradCheckMask() Is Nothing, Nothing, config.getGradCheckMask().get(s)))

				listener.setVariableName(s)

				Dim i As Integer=0
				Do While iter.MoveNext()
					Dim idx() As Long = iter.Current

					Dim strIdx As String = Nothing
					If config.isPrint() Then
						strIdx = java.util.Arrays.toString(idx).replaceAll(" ","")
					End If

					Dim maskValue As Boolean = (varMask Is Nothing OrElse (varMask.getDouble(idx) <> 0))
					If Not maskValue Then
						'Skip this specific entry (masked out)
						Continue Do
					End If

					'Set listener to apply eps, then do forward pass:
					listener.setIdx(idx)
					listener.setEps(config.getEps())
					Dim scorePlus As Double = 0.0
					Dim m As IDictionary(Of String, INDArray) = sd.output(config.getPlaceholderValues(), lossFnVariables)
					For Each arr As INDArray In m.Values
						scorePlus += arr.sumNumber().doubleValue()
					Next arr
					listener.setEps(-config.getEps())
					m = sd.output(config.getPlaceholderValues(), lossFnVariables)
					Dim scoreMinus As Double = 0.0
					For Each arr As INDArray In m.Values
						scoreMinus += arr.sumNumber().doubleValue()
					Next arr

					Dim numericalGrad As Double = (scorePlus - scoreMinus) / (2 * config.getEps())
					Dim analyticGrad As Double = gradientsForAct(s).getDouble(idx)

					If Double.IsInfinity(numericalGrad) OrElse Double.IsNaN(numericalGrad) Then
						Throw New System.InvalidOperationException("Numerical gradient was " & numericalGrad & " for variable """ & s & """, parameter " & i & " of " & n & " (position: " & strIdx & ")")
					End If
					If Double.IsInfinity(analyticGrad) OrElse Double.IsNaN(analyticGrad) Then
						Throw New System.InvalidOperationException("Analytic (SameDiff) gradient was " & analyticGrad & " for variable """ & s & """, parameter " & i & " of " & n & " (position: " & strIdx & ")")
					End If

					Dim relError As Double
					If numericalGrad = 0.0 AndAlso analyticGrad = 0.0 Then
						relError = 0.0
					Else
						relError = Math.Abs(analyticGrad - numericalGrad) / (Math.Abs(Math.Abs(analyticGrad) + Math.Abs(numericalGrad)))
					End If

					If relError > maxError Then
						maxError = relError
					End If

					If relError > maxRelError OrElse Double.IsNaN(relError) Then
						Dim absError As Double = Math.Abs(analyticGrad - numericalGrad)
						If absError < minAbsError Then
							If config.isPrint() Then
								log.info("Param " & i & " (" & s & strIdx & ") passed: grad= " & analyticGrad & ", numericalGrad= " & numericalGrad & ", relError= " & relError & "; absolute error = " & absError & " < minAbsoluteError = " & minAbsError)
							End If
						Else
							If config.isPrint() Then
								log.info("Param " & i & " (" & s & strIdx & ") FAILED: grad= " & analyticGrad & ", numericalGrad= " & numericalGrad & ", relError= " & relError & ", absError=" & absError & ", scorePlus=" & scorePlus & ", scoreMinus= " & scoreMinus)
							End If
							If config.isExitOnFirstFailure() Then
								Return False
							End If
							totalNFailures += 1
						End If
					ElseIf config.isPrint() Then
						log.info("Param " & i & " (" & s & strIdx & ") passed: grad= " & analyticGrad & ", numericalGrad= " & numericalGrad & ", relError= " & relError)
					End If
					i += 1

				Loop
			Next s

			Return totalNFailures = 0
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder @Data public static class ActGradConfig
		Public Class ActGradConfig
			Friend sd As SameDiff
			Friend placeholderValues As IDictionary(Of String, INDArray)
			Friend activationGradsToCheck As IList(Of String)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private double eps = DEFAULT_EPS;
			Friend eps As Double = DEFAULT_EPS
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private double maxRelError = DEFAULT_MAX_REL_ERROR;
			Friend maxRelError As Double = DEFAULT_MAX_REL_ERROR
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private double minAbsError = DEFAULT_MIN_ABS_ERROR;
			Friend minAbsError As Double = DEFAULT_MIN_ABS_ERROR
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private boolean print = DEFAULT_PRINT;
			Friend print As Boolean = DEFAULT_PRINT
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default boolean exitOnFirstFailure = DEFAULT_EXIT_FIRST_FAILURE;
			Friend exitOnFirstFailure As Boolean = DEFAULT_EXIT_FIRST_FAILURE
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private boolean skipValidation = false;
			Friend skipValidation As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private boolean debugMode = DEFAULT_DEBUG_MODE;
			Friend debugMode As Boolean = DEFAULT_DEBUG_MODE
			Friend skipVariables As ISet(Of String)
			Friend gradCheckMask As IDictionary(Of String, INDArray)
			Friend maxPerParam As Integer
			Friend subset As Subset
		End Class


		Public Shared Sub validateInternalState(ByVal sd As SameDiff, ByVal generateAndCheckGradFn As Boolean)

	'        
	'        Some conditions that should always hold:
	'        1. incomingArgsReverse and outgoingArgsReverse:
	'            (a) all differential functions should be present here exactly once
	'            (b) The values should be valid variable names
	'        2. variableMap: should contain all variables, and only all variables
	'        3. functionArgsFor should contain all variables, all functions... same for functionOutputsFor
	'        4. Gradient function: should contain all of the existing functions, and more
	'         

			Dim dfs() As DifferentialFunction = sd.ops()
			Dim vars As IList(Of SDVariable) = sd.variables()

			Dim varSetStr As ISet(Of String) = New HashSet(Of String)()
			For Each v As SDVariable In vars
				If varSetStr.Contains(v.name()) Then
					Throw New System.InvalidOperationException("Variable with name " & v.name() & " already encountered")
				End If
				varSetStr.Add(v.name())
			Next v
			Preconditions.checkState(vars.Count = varSetStr.Count, "Duplicate variables in variables() list")

			'1. Check incomingArgsReverse and outgoingArgsReverse
			Dim ops As IDictionary(Of String, SameDiffOp) = sd.getOps()
			Preconditions.checkState(dfs.Length = ops.Count, "All functions not present in incomingArgsReverse")
			For Each df As DifferentialFunction In dfs
				Preconditions.checkState(ops.ContainsKey(df.getOwnName()), df.getOwnName() & " not present in ops map")
				Dim sameDiffOp As SameDiffOp = ops(df.getOwnName())
				Dim str As IList(Of String) = sameDiffOp.getInputsToOp()
				If str IsNot Nothing Then
					For Each s As String In str
						Preconditions.checkState(varSetStr.Contains(s), "Variable " & s & " in op inputs not a known variable name")
					Next s
				End If

				str = sameDiffOp.getOutputsOfOp()
				If str IsNot Nothing Then
					For Each s As String In str
						Preconditions.checkState(varSetStr.Contains(s), "Variable " & s & " in op outputs not a known variable name")
					Next s
				End If
			Next df

			'Also check that outgoingArgsReverse values are unique: i.e., shouldn't have the same op appearing multiple times
			Dim seen As IDictionary(Of String, String) = New Dictionary(Of String, String)()
			For Each e As KeyValuePair(Of String, SameDiffOp) In ops.SetOfKeyValuePairs()
				Dim varNames As IList(Of String) = e.Value.getOutputsOfOp()
				If varNames IsNot Nothing Then
					For Each s As String In varNames
						If seen.ContainsKey(s) Then
							Throw New System.InvalidOperationException("Already saw variable """ & s & """ as output for op """ & seen(s) & """: expected variables to be present as an output only once; also seen as output for op """ & e.Key & """")
						End If
						seen(s) = e.Key
					Next s
				End If
			Next e

			'2. Check variableMap
			Dim variableMap As IDictionary(Of String, Variable) = sd.getVariables()
			Preconditions.checkState(vars.Count = variableMap.Count, "Variable map size check failed")
			For Each e As KeyValuePair(Of String, Variable) In variableMap.SetOfKeyValuePairs()
				Preconditions.checkState(e.Key.Equals(e.Value.getVariable().name()), "Name not equal")
			Next e

			If generateAndCheckGradFn Then
				'3. Check gradient function
				If sd.getFunction("grad") Is Nothing Then
					sd.createGradFunction()
				End If

				Dim gradFn As SameDiff = sd.getFunction("grad")
				'Run same validation for gradient fn...
				validateInternalState(gradFn, False)

				'Check that all original functions are present in the gradient function
				For Each dfOrig As DifferentialFunction In dfs
					Preconditions.checkNotNull(gradFn.getOpById(dfOrig.getOwnName()), "DifferentialFunction " & dfOrig.getOwnName() & " from original SameDiff instance not present in grad fn")
				Next dfOrig
			End If
		End Sub

		Private Shared Function getObject(Of T)(ByVal fieldName As String, ByVal from As Object, ByVal fromClass As Type) As T
			Try
				Dim f As System.Reflection.FieldInfo = fromClass.getDeclaredField(fieldName)
				f.setAccessible(True)
				Return CType(f.get(from), T)
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function
	End Class

End Namespace