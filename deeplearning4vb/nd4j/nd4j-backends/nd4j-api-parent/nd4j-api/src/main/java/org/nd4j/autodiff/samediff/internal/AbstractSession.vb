Imports System
Imports System.Collections.Generic
Imports System.Text
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports At = org.nd4j.autodiff.listeners.At
Imports Listener = org.nd4j.autodiff.listeners.Listener
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports VariableType = org.nd4j.autodiff.samediff.VariableType
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.linalg.api.ops.impl.controlflow.compat
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports org.nd4j.common.function
import static org.nd4j.imports.VariableUtils.stripVarSuffix

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

Namespace org.nd4j.autodiff.samediff.internal


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class AbstractSession<T, O>
	Public MustInherit Class AbstractSession(Of T, O)

		''' <summary>
		''' All execution in Samediff happens in a frame... this is the name of the main/outer frame - i.e., the "default" frame
		''' Other frames (such as for loops) may be nested within this frame
		''' </summary>
		Public Const OUTER_FRAME As String = "main"

		Protected Friend ReadOnly sameDiff As SameDiff
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final Map<VarId, T> nodeOutputs = new HashMap<>();
		Protected Friend ReadOnly nodeOutputs As IDictionary(Of VarId, T) = New Dictionary(Of VarId, T)() 'Key: variable (at a given frame + iteration). Value: the calculated output for that variable
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final Map<VarId, List<T>> tensorArrays = new HashMap<>();
		Protected Friend ReadOnly tensorArrays As IDictionary(Of VarId, IList(Of T)) = New Dictionary(Of VarId, IList(Of T))() 'Stores the underlying arrays for TensorArray ops
	'    
	'    The dependency tracker is responsible for determining what ops (at what frame/iteration) can be executed next, given
	'    what has been executed so far.
	'    For static graphs, such as abstraction would not be necessary; for dynamic graphs (i.e., nested loops, of arbitary
	'    number of iterations and depth - and also switch ops which can cause whole subgraphs to not be executed) this is necessary
	'    Note: the ExecStep represents one step for execution - some steps are as simple as "execute an op (at the given frame/iter)"
	'    It works by adding dependencies (X -> Y - such as "op Y depends on the output of op X") and then marking them as
	'    satisfied ("op X has been calculated"). Once all dependencies for an execution step have been satisfied, the execution step
	'    is added to a queue - outputs of which can be accessed with dt.getNewAllSatisfied() and dt.getNewAllSatisfiedList(),
	'    at which point it is removed from the dependency tracker
	'     
		Protected Friend ReadOnly dt As New DependencyTracker(Of ExecStep, ExecStep)()

		''' <summary>
		''' Contains variables we *might* need to execute in process of getting outputs we want.
		''' Variables not in this set are definitely not needed to get the requested output variables, but variables that are
		''' in this set may not be executed depending on the graph structure - i.e., switch ops, etc
		''' </summary>
		Protected Friend ReadOnly subgraph As ISet(Of String) = New HashSet(Of String)()
		''' <summary>
		''' As per subgraph set, but for ops instead
		''' </summary>
		Protected Friend ReadOnly subgraphOps As ISet(Of String) = New HashSet(Of String)()

		''' <summary>
		''' Constains the names of ops that don't have any inputs. Kept because normally ops are triggered for execution when
		''' their all their inputs have been calculated; we'll trigger that step manually during execution initialization
		''' </summary>
		Protected Friend ReadOnly zeroInputOpsInSubgraph As ISet(Of String) = New HashSet(Of String)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AbstractSession(@NonNull SameDiff sameDiff)
		Public Sub New(ByVal sameDiff As SameDiff)
			Me.sameDiff = sameDiff
		End Sub

		Public Overridable Function contains(ByVal variable As String, ByVal frame As String, ByVal iteration As Integer, ByVal parentFrameIter As FrameIter) As Boolean
			Dim varId As New VarId(variable, frame, iteration, parentFrameIter)
			Return nodeOutputs.ContainsKey(varId)
		End Function

		''' <summary>
		''' Get a previously calculated output; throws an exception if the output does not exist
		''' </summary>
		Public Overridable Function get(ByVal variable As String, ByVal frame As String, ByVal iteration As Integer, ByVal parentFrameIter As FrameIter) As T
			Return get(variable, frame, iteration, parentFrameIter, True)
		End Function

		''' <summary>
		''' Get a previously calculated output
		''' </summary>
		''' <param name="enforceExistence"> If true: throw an exception if the array does not exist </param>
		Public Overridable Function get(ByVal variable As String, ByVal frame As String, ByVal iteration As Integer, ByVal parentFrameIter As FrameIter, ByVal enforceExistence As Boolean) As T
			'TODO eventually we'll cache and reuse VarId objects here to avoid garbage generation on lookup etc
			Dim varId As New VarId(variable, frame, iteration, parentFrameIter)
			Dim [out] As T = nodeOutputs(varId)
			If enforceExistence Then
				Preconditions.checkNotNull([out], "No output found for variable %s (frame %s, iteration %s)", variable, frame, iteration)
			End If
			Return [out]
		End Function

		''' <summary>
		''' Get the output of the session - i.e., perform inference/forward pass and return the outputs for the specified variables
		''' </summary>
		''' <param name="variables">           Name of the variables we want the arrays/activations for </param>
		''' <param name="placeholderValues">   The placeholder values (if any). May be null. </param>
		''' <param name="batch">               The batch data, used to call Listener.opExecution </param>
		''' <param name="requiredActivations"> Additional activations that are required.  Won't be outputed, but opExecution will be called.  May be null. </param>
		''' <returns> The specified variable values, optionally in the specified workspace </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Map<String, T> output(@NonNull List<String> variables, Map<String, T> placeholderValues, org.nd4j.linalg.dataset.api.MultiDataSet batch, Collection<String> requiredActivations, List<org.nd4j.autodiff.listeners.Listener> listeners, org.nd4j.autodiff.listeners.At at)
		Public Overridable Function output(ByVal variables As IList(Of String), ByVal placeholderValues As IDictionary(Of String, T), ByVal batch As MultiDataSet, ByVal requiredActivations As ICollection(Of String), ByVal listeners As IList(Of Listener), ByVal at As At) As IDictionary(Of String, T)
			Preconditions.checkState(variables.Count > 0 OrElse requiredActivations.Count > 0, "Variables to perform forward pass for must not be empty")

			If requiredActivations Is Nothing Then
				requiredActivations = java.util.Collections.emptySet()
			End If

			If at Is Nothing Then
				at = At.defaultAt()
			End If

			'Step 0: validation - that variables exist, placeholders have arrays, etc
			For Each s As String In variables
				Preconditions.checkState(sameDiff.variableMap().ContainsKey(s), "Requested output variable %s does not exist in SameDiff instance", s)
			Next s

			Dim reqOutputVariablesSet As ISet(Of String) = New HashSet(Of String)(variables)

			placeholderValues = preprocessPlaceholders(placeholderValues, at)

			'Clear state from past iterations, if any
			dt.clear()
			subgraph.Clear()
			subgraphOps.Clear()
			nodeOutputs.Clear() 'TODO eventually we'll have (optional) cache here for later execs... main challenge is detecting in-place array modifications and invalidating old results. And overall memory use...
			tensorArrays.Clear()

			'Step 1: determine subgraph structure we actually need to execute
			'Basic plan: work backwards from the variables we want, based on the graph structure, to work out what
			' we actually need to execute
			'TODO we'll optimize this and cache the results, only recalculating if the graph structure changes
			Dim userRequestedUnique As ISet(Of String) = New HashSet(Of String)(variables)
			Dim allRequired As ISet(Of String) = New HashSet(Of String)(requiredActivations)
			allRequired.addAll(variables)
			initSubgraph(allRequired)

			'Step 2: Check that we have required placeholders
			Dim phNames As IList(Of String) = sameDiff.inputs()
			If placeholderValues Is Nothing OrElse Not placeholderValues.Keys.ContainsAll(phNames) Then
	'             We only have a subset of all placeholders
	'            Validate that we have all *required* placeholder values. Some might not be needed to calculate the requested outputs
	'            A placeholder is required if:
	'            (a) It's one of the requested outputs
	'            (b) It's required to calculate any of the ops in the subgraph
	'            For example, we might have a label placeholder, and we're doing inference not training
	'             
				For Each s As String In phNames
					Dim required As Boolean = False
					If variables.Contains(s) Then
						required = True
					End If
					If Not required Then
						Dim v As Variable = sameDiff.getVariables().get(s)
						If v.getInputsForOp() IsNot Nothing Then
							For Each s2 As String In v.getInputsForOp()
								If subgraph.Contains(s2) Then
									'Placeholder is required
									required = True
									Exit For
								End If
							Next s2
						End If
					End If

					If required AndAlso (placeholderValues Is Nothing OrElse Not placeholderValues.ContainsKey(s)) Then
						Throw New System.InvalidOperationException("An input placeholder """ & s & """ is required to calculate the requested outputs," & " but a placeholder value was not provided")
					End If
				Next s
			End If

			'Step 3: Mark the (required) variables, constants and placeholders as available via dependency tracker
			'And also any "zero dependency" ops - i.e., those without any inputs
			Dim start As New ExecStep(ExecType.EXEC_START, "", Nothing) 'Dummy dependency to trigger the variables and constants
			For Each v As SDVariable In sameDiff.variables()
				Dim vt As VariableType = v.getVariableType()
				If vt = VariableType.VARIABLE OrElse vt = VariableType.CONSTANT Then
					Dim et As ExecType = If(vt = VariableType.VARIABLE, ExecType.VARIABLE, ExecType.CONSTANT)
					Dim es As New ExecStep(et, v.name(), New FrameIter(OUTER_FRAME, 0, Nothing))
					dt.addDependency(es, start)

					Dim var As Variable = sameDiff.getVariables().get(v.name())
					If var.getControlDeps() IsNot Nothing Then
						addVarControlDeps(es, var) 'Before this variable can be considered available for use, we need specified op to be executed
					End If
				End If
			Next v
			For Each s As String In phNames
				Dim es As New ExecStep(ExecType.PLACEHOLDER, s, New FrameIter(OUTER_FRAME, 0, Nothing))
				dt.addDependency(es, start)

				Dim var As Variable = sameDiff.getVariables().get(s)
				If var.getControlDeps() IsNot Nothing Then
					addVarControlDeps(es, var) 'Before this variable can be considered available for use, we need specified op to be executed
				End If
			Next s
			For Each s As String In zeroInputOpsInSubgraph
				Dim es As New ExecStep(ExecType.OP, s, New FrameIter(OUTER_FRAME, 0, Nothing))
				dt.addDependency(es, start)
			Next s
			dt.markSatisfied(start, True)


			'Step 4: execute in any order, but not switching to new frame/iteration until all from current frame/iter ops
			' are done - until we have all required nodeOutputs
	'        
	'        The idea is simple: we start off with a set of "available to execute" variables - just the placeholders,
	'        constants and variables (assuming no control dependencies) at the start of execution.
	'
	'        Then, we remove an "available to execute" node and execute it. Execution may be:
	'        (a) For constants, variable type SDVariables, and placeholders: just look up the value
	'        (b) For variables as outputs of ops: actually execute the op
	'
	'        After execution, we look at the graph structure and determine what that now executed/calculated variable is
	'        an input to. If all inputs are available for the op, we mark all output variables of that op as available for execution.
	'        Both parts of this (tracking dependencies, and also what's now available to execute) are handled in the dependency tracker
	'
	'        We stop computation once all the required outputs are available. At this point, subgraph may NOT be empty - for example,
	'        switch ops may cause entire branches of the graph to be skipped.
	'         

			Dim [out] As IDictionary(Of String, T) = New Dictionary(Of String, T)() 'Outputs, returned to the user
			Dim allExecuted As ISet(Of String) = New HashSet(Of String)()
			Dim [step] As Integer = 0 'Number of execution steps
			'Next 3: current execution frame
			Dim currentFrame As String = OUTER_FRAME
			Dim currentFrameIter As Integer = 0
			Dim currParentFrame As FrameIter = Nothing
			Dim predicate As New ExecStepPredicate(Me)
			Do While allExecuted.Count < allRequired.Count
				If Not dt.hasNewAllSatisfied() Then
					'Haven't got all of the outputs the user requested, but there's nothing left that we can execute. Should not happen.
					execFailed(userRequestedUnique, [out], allRequired, allExecuted, [step])
				End If

				'Get variable in the current frame/iteration and execute it's corresponding op
				'If no more ops exist for the current frame/iter, we'll switch to the next frame/iter
				'The idea is to not mix the order of execution of ops in different frames/iters - i.e., finish the current
				' frame/iter before starting the next one
				predicate.setCurrentFrame(currentFrame)
				predicate.setCurrentFrameIter(currentFrameIter)
				predicate.setCurrParentFrame(currParentFrame)

				Dim es As ExecStep = dt.getFirstNewAllSatisfiedMatching(predicate)
				If es Is Nothing Then
					'We must have finished the current frame/iter, and are switching to the next one
					es = dt.NewAllSatisfied
				End If

				currentFrame = es.getFrameIter().getFrame()
				currentFrameIter = es.getFrameIter().getIteration()
				currParentFrame = es.getFrameIter().getParentFrame()

				log.trace("Beginning execution step {}: {}", [step], es)

				Dim outFrameIter As FrameIter
				Dim skipDepUpdate As Boolean = False 'Only used for Switch ops, which have slighly different handling...
				Dim skipMarkSatisfied As Boolean = False 'Only for enter ops, because of different frame/iter
				If es.getType() = ExecType.CONSTANT OrElse es.getType() = ExecType.VARIABLE Then
					Dim vid As New VarId(es.getName(), OUTER_FRAME, 0, Nothing)
					Dim arr As T = getConstantOrVariable(es.getName())
					Preconditions.checkNotNull(arr, "Encountered null placeholder array for constant: %s", vid)
					nodeOutputs(vid) = arr
					outFrameIter = New FrameIter(OUTER_FRAME, 0, Nothing)
					If userRequestedUnique.Contains(es.getName()) Then
						'User requested const/variable as one of the outputs
						[out](es.getName()) = arr
					End If
					If allRequired.Contains(es.getName()) Then
						allExecuted.Add(es.getName())
					End If
				ElseIf es.getType() = ExecType.PLACEHOLDER Then
					Dim vid As New VarId(es.getName(), OUTER_FRAME, 0, Nothing)
					Dim phVal As T = If(placeholderValues Is Nothing, Nothing, placeholderValues(es.getName()))

					nodeOutputs(vid) = phVal
					outFrameIter = New FrameIter(OUTER_FRAME, 0, Nothing)
					If allRequired.Contains(es.getName()) Then
						Preconditions.checkState(placeholderValues IsNot Nothing AndAlso placeholderValues.ContainsKey(es.getName()), "No array was provided for the placeholder variable ""%s"" that is required for execution", es.getName())
						'User requested placeholder value as one of the outputs
						[out](es.getName()) = placeholderValues(es.getName())
					End If
					If allRequired.Contains(es.getName()) Then
						allExecuted.Add(es.getName())
					End If
				ElseIf es.getType() = ExecType.OP Then
					Dim opName As String = es.getName()
					Dim op As SameDiffOp = sameDiff.getOps().get(opName)
					Dim o As DifferentialFunction = op.Op

					If TypeOf o Is Enter Then
						'Enter op: output is variable in a new (specified) frame, iteration 0.
						'Parent is current (input) frame
						Dim outFrame As String = DirectCast(o, Enter).FrameName
						outFrameIter = New FrameIter(outFrame, 0, es.getFrameIter())

					ElseIf TypeOf o Is [Exit] Then
						'Exit node forwards input to parent frame
						Dim outFrame As String = es.getFrameIter().getParentFrame().getFrame()
						Dim outIter As Integer = es.getFrameIter().getParentFrame().getIteration()
						Dim outParentFrame As FrameIter = es.getFrameIter().getParentFrame().getParentFrame()
						outFrameIter = New FrameIter(outFrame, outIter, outParentFrame)
					ElseIf TypeOf o Is NextIteration Then
						'NextIteration op: forwards its single input to its output varible in the current frame, but increments the iteration number
						outFrameIter = es.getFrameIter().clone()
						outFrameIter.setIteration(outFrameIter.getIteration())
					Else
						'Standard ops - output variable has same frame and iteration number as the input(s)
						'Also loopCond, merge, while, etc
						outFrameIter = es.getFrameIter()
					End If


					'Resolve the inputs to this execution step (op) to actual arrays
					Dim inputs As ISet(Of VarId) = Nothing
					Dim allIterInputs As ISet(Of VarId) = Nothing
					Dim constAndPhInputs As ISet(Of String) = Nothing
					Dim dl As DependencyList(Of ExecStep, ExecStep) = dt.getDependencies(es)

					Dim inputNames As IList(Of String) = op.getInputsToOp()
					If inputNames IsNot Nothing AndAlso inputNames.Count > 0 Then
						inputs = New HashSet(Of VarId)()
						allIterInputs = New HashSet(Of VarId)()
						constAndPhInputs = New HashSet(Of String)()
						Dim deps As IList(Of ExecStep) = dl.getDependencies()
						If deps IsNot Nothing AndAlso deps.Count > 0 Then
							For Each dep As ExecStep In deps
								Select Case dep.getType()
									Case OP, SWITCH_L, SWITCH_R
										'The current execution step depends on one output of the op "dep"
										Dim toExecOp As SameDiffOp = sameDiff.getOps().get(es.getName())
										Dim inputsToExecOp As IList(Of String) = toExecOp.getInputsToOp()
										Dim inputOp As SameDiffOp = sameDiff.getOps().get(dep.getName())
										Dim inputOpOutNames As IList(Of String) = inputOp.getOutputsOfOp()
										For Each s As String In inputsToExecOp
											If inputOpOutNames.Contains(s) Then
												Dim vid As New VarId(s, dep.getFrameIter().getFrame(), dep.getFrameIter().getIteration(), dep.getFrameIter().getParentFrame())
												inputs.Add(vid)
											End If
										Next s
									Case VARIABLE
										inputs.Add(New VarId(dep.getName(), OUTER_FRAME, 0, Nothing))
									Case CONSTANT, PLACEHOLDER
										constAndPhInputs.Add(dep.getName())
									Case Else
										Throw New System.NotSupportedException("Not yet implemented: " & dep.getType())
								End Select
							Next dep
						End If
					End If


					' Do execution of the op, in 2 steps
					' (a) "Parameterize" the op - i.e., find and set the arrays on the op, allocate outputs, etc ready for execution
					' (b) actually execute the operation
					Dim parameterizedOp As O = getAndParameterizeOp(opName, outFrameIter, inputs, allIterInputs, constAndPhInputs, placeholderValues, reqOutputVariablesSet)
					Dim opOutputValues() As T = getOutputs(parameterizedOp, outFrameIter, inputs, allIterInputs, constAndPhInputs, listeners, at, batch, reqOutputVariablesSet)
					Dim opOutVarNames As IList(Of String) = op.getOutputsOfOp()

					Preconditions.checkState(opOutputValues.Length = opOutVarNames.Count, "Unexpected number of outputs from executed op %s:" & " got %s outputs when %s outputs were expected (%s)", parameterizedOp.GetType().Name, opOutputValues.Length, opOutVarNames.Count, opOutVarNames)

					'Store the op outputs
					For i As Integer = 0 To opOutputValues.Length - 1
						If opOutputValues(i) Is Nothing AndAlso TypeOf op.Op Is Switch Then
							'Switch op only forwards the input to one of the outputs
							Continue For
						End If

						Dim n As String = opOutVarNames(i)
						Dim vid As New VarId(n, outFrameIter.getFrame(), outFrameIter.getIteration(), outFrameIter.getParentFrame())
						nodeOutputs(vid) = opOutputValues(i)

						If userRequestedUnique.Contains(n) Then
							[out](n) = opOutputValues(i)
						End If
						If allRequired.Contains(n) Then
							allExecuted.Add(n)
						End If
					Next i

					'Post execution: update dependency tracker so we know what is available to execute next, given we now
					' have these new values
					If TypeOf o Is Switch Then
	'                    
	'                    Switch is a special case: only one output/branch is considered to exist post execution.
	'                    Unlike every other type of op, only 1 of 2 output arrays is actually executed.
	'                    For dependency tracking purposes, this is why we have SWITCH_L and _R execution types.
	'                    If we just depended on the op, the dependency tracker would incorrectly conclude that ops relying on
	'                    both branches (i.e., including the unavailable one) can now be executed
	'                     
						skipDepUpdate = True
						skipMarkSatisfied = True
						Dim nullCount As Integer = (If(opOutputValues(0) Is Nothing, 1, 0)) + (If(opOutputValues(1) Is Nothing, 1, 0))
						Preconditions.checkState(nullCount = 1, "Expected exactly one output to be present for switch ops, got %s", nullCount)
						Dim left As Boolean = opOutputValues(0) IsNot Nothing
						Dim branch As ExecStep
						If left Then
							branch = New ExecStep(ExecType.SWITCH_L, es.getName(), es.getFrameIter())
						Else
							branch = New ExecStep(ExecType.SWITCH_R, es.getName(), es.getFrameIter())
						End If
						updateDescendantDeps(branch, outFrameIter)
						dt.markSatisfied(branch, True)
					ElseIf TypeOf o Is Enter Then
						'Enter op: we want to say that the inner frame is executed...
						skipDepUpdate = True
						skipMarkSatisfied = True
						Dim e As Enter = DirectCast(o, Enter)
						Dim fi As New FrameIter(e.FrameName, 0, es.getFrameIter())
						Dim exec As New ExecStep(ExecType.OP, es.getName(), fi)
						updateDescendantDeps(exec, fi)
						dt.markSatisfied(exec, True)
					ElseIf TypeOf o Is [Exit] Then
						'Exit op: we want to say that the parent frame is executed...
						skipDepUpdate = True
						skipMarkSatisfied = True
						Dim fi As FrameIter = es.getFrameIter().getParentFrame()
						Dim exec As New ExecStep(ExecType.OP, es.getName(), fi)
						updateDescendantDeps(exec, fi)
						dt.markSatisfied(exec, True)
					End If

	'                
	'                Edge case for TensorFlow import control dependencies: for some reason, TF allows op control dependencies
	'                like /while/x -> SomeConstant - i.e., a constant depending on something inside a scope.
	'                This should be handled with an enter op, but TF doesn't always use this :/
	'                Note that this is equivalent to marking the control dependency as satisfied on the first iteration
	'                TODO double check that this is exactly the same behaviour as TF - otherwise this approach might fail in
	'                     some rare cases that rely on the constant/variable not being available
	'                 
					Dim cdFor As IList(Of String) = op.getControlDepFor()
					If cdFor IsNot Nothing Then
						Dim cdEs As New ExecStep(ExecType.CONTROL_DEP, opName, Nothing)
						If Not dt.isSatisfied(cdEs) Then
							dt.markSatisfied(cdEs, True)
						End If
					End If

				Else
					'Should never happen
					Throw New Exception("Unknown ExecStep: " & es)
				End If

				'Standard ops
				If Not skipDepUpdate Then
					updateDescendantDeps(es, outFrameIter)
				End If
				If Not skipMarkSatisfied Then
					dt.markSatisfied(es, True)
				End If

				[step] += 1
			Loop

			'TODO we should clear the node outputs map to get rid of the invalid (closed, out of workspace, etc) arrays

			[out] = postProcessOutput([out]) 'Hook-in for subclass sessions, if needed
			Return [out]
		End Function

		''' <summary>
		''' Add the control dependency from Op -> variable
		''' </summary>
		''' <param name="es"> Execution step for the variable </param>
		''' <param name="v">  Variable </param>
		Protected Friend Overridable Sub addVarControlDeps(ByVal es As ExecStep, ByVal v As Variable)
			Dim cds As IList(Of String) = v.getControlDeps()
			If cds IsNot Nothing Then
				For Each s As String In cds
					Dim controlES As New ExecStep(ExecType.CONTROL_DEP, s, Nothing)
					dt.addDependency(es, controlES) 'Before this variable can be considered available for use, we need specified op to be executed
				Next s
			End If
		End Sub

		''' <summary>
		''' Execution failed - can't calculate all requested outputs, and there's nothing left to calculate.
		''' Throws an exception with a useful message
		''' </summary>
		''' <param name="userRequestedUnique"> All outputs that the user requested </param>
		''' <param name="out">                 Current outputs </param>
		''' <param name="step">                Execution step </param>
		Protected Friend Overridable Sub execFailed(ByVal userRequestedUnique As ISet(Of String), ByVal [out] As IDictionary(Of String, T), ByVal allRequired As ISet(Of String), ByVal allExecuted As ISet(Of String), ByVal [step] As Integer)
			Dim missingCount As Integer = userRequestedUnique.Count - [out].Count
			Dim sb As New StringBuilder()
			sb.Append("No variable are available for execution at step ").Append([step]).Append(": ").Append(missingCount).Append(" requested output values remaining, ").Append(allExecuted.Count - allRequired.Count).Append(" variables required to be executed remaining")
			Dim missing As ISet(Of String) = New HashSet(Of String)()
			For Each s As String In userRequestedUnique
				If Not [out].ContainsKey(s) Then
					missing.Add(s)
				End If
			Next s

			If missingCount <= 10 Then
				sb.Append(". Missing variables: ")
				sb.Append(missing)
			Else
				sb.Append(". First 10 missing variables: ")
				Dim iter As IEnumerator(Of String) = missing.GetEnumerator()
				Dim i As Integer = 0
				Do While i < 10 AndAlso iter.MoveNext()
					If i > 0 Then
						sb.Append(",")
					End If
					sb.Append(iter.Current)
					i += 1
				Loop
			End If
			Dim s As String = sb.ToString()
			Console.WriteLine(sameDiff.summary())
			Throw New System.InvalidOperationException(s)
		End Sub

		''' <summary>
		''' Update the descendant dependencies
		''' So if the graph structure is X -> A, then add all (X,Y,Z,...) -> A to the dependency tracker
		''' This is for a specific frame and iteration, for both sides of the dependency (in and out)
		''' </summary>
		''' <param name="justExecuted"> The execution step that has just completed </param>
		''' <param name="outFrameIter"> The frame/iteration of the output </param>
		Protected Friend Overridable Sub updateDescendantDeps(ByVal justExecuted As ExecStep, ByVal outFrameIter As FrameIter)
			Dim t As ExecType = justExecuted.getType()
			Dim n As String = justExecuted.getName()
			If justExecuted.getType() = ExecType.OP Then
				Dim op As SameDiffOp = sameDiff.getOps().get(n)
				Dim outNames As IList(Of String) = op.getOutputsOfOp()
				For Each s As String In outNames
					Dim v As Variable = sameDiff.getVariables().get(s)
					If v IsNot Nothing Then
						Dim inputsToOps As IList(Of String) = v.getInputsForOp()
						If inputsToOps IsNot Nothing Then
							For Each opName As String In inputsToOps
								If subgraphOps.Contains(opName) Then
									'We've just executed X, and there's dependency X -> Y
									'But, there also might be a Z -> Y that we should mark as needed for Y
									addDependenciesForOp(opName, outFrameIter)
								End If
							Next opName
						End If


						'Also add control dependencies (variable)
						Dim cdForOps As IList(Of String) = v.getControlDepsForOp()
						If cdForOps IsNot Nothing Then
							For Each opName As String In cdForOps
								If subgraphOps.Contains(opName) Then
									'We've just executed X, and there's dependency X -> Y
									'But, there also might be a Z -> Y that we should mark as needed for Y
									addDependenciesForOp(opName, outFrameIter)
								End If
							Next opName
						End If
					End If

				Next s
			ElseIf t = ExecType.VARIABLE OrElse t = ExecType.CONSTANT OrElse t = ExecType.PLACEHOLDER Then
				Dim v As Variable = sameDiff.getVariables().get(n)
				If v IsNot Nothing Then
					Dim inputsToOps As IList(Of String) = v.getInputsForOp()
					If inputsToOps IsNot Nothing Then
						For Each opName As String In inputsToOps
							If subgraphOps.Contains(opName) Then
								addDependenciesForOp(opName, outFrameIter)
							End If
						Next opName
					End If
				End If

			ElseIf justExecuted.getType() = ExecType.SWITCH_L OrElse justExecuted.getType() = ExecType.SWITCH_R Then
				Dim op As SameDiffOp = sameDiff.getOps().get(n)
				Dim outNames As IList(Of String) = op.getOutputsOfOp()
				Dim branchVarName As String = (If(justExecuted.getType() = ExecType.SWITCH_L, outNames(0), outNames(1)))
				Dim v As Variable = sameDiff.getVariables().get(branchVarName)
				If v IsNot Nothing Then
					Dim inputsToOps As IList(Of String) = v.getInputsForOp()
					If inputsToOps IsNot Nothing Then
						For Each opName As String In inputsToOps
							If subgraphOps.Contains(opName) Then
								'We've just executed X, and there's dependency X -> Y
								'But, there also might be a Z -> Y that we should mark as needed for Y
								addDependenciesForOp(opName, outFrameIter)
							End If
						Next opName
					End If
				End If

			Else
				Throw New System.NotSupportedException("Unknown or not yet implemented exec type: " & justExecuted)
			End If
		End Sub

		''' <summary>
		''' Suppose operation X has just been executed.
		''' For X -> someOp, add all dependencies for someOp, i.e., all Z -> someOp
		''' (which includes X, but may not only be X)
		''' </summary>
		''' <param name="opName">       Name of the op </param>
		''' <param name="depFrameIter"> Frame/iteration of the op instance to be executed </param>
		Protected Friend Overridable Sub addDependenciesForOp(ByVal opName As String, ByVal depFrameIter As FrameIter)
			Dim op As SameDiffOp = sameDiff.getOps().get(opName)
			Dim inputs As IList(Of String) = op.getInputsToOp()
			Dim cdOps As IList(Of String) = op.getControlDeps()
			Dim cdVars As IList(Of String) = op.getVarControlDeps()

			Dim es As New ExecStep(ExecType.OP, opName, depFrameIter)
			If Not (TypeOf op.Op Is NextIteration) AndAlso dt.hasDependency(es) Then
				'Already processed this once. We only add dependencies once per op (for a given frame/iteration)
				Return
			End If

			If TypeOf op.Op Is Merge Then
				'Merge ops are a special case: they can be executed with EITHER ONE of the inputs available - unlike every
				' other op, we don't need all inputs, just one, before it can be executed
				Dim v0 As Variable = sameDiff.getVariables().get(inputs(0))
				Dim v1 As Variable = sameDiff.getVariables().get(inputs(1))

				Dim or0 As ExecStep = getExecStepForVar(v0.getName(), depFrameIter)
				Dim or1 As ExecStep = getExecStepForVar(v1.getName(), depFrameIter)
				dt.addOrDependency(es, or0, or1)
			ElseIf TypeOf op.Op Is NextIteration Then
				'For NextIteration, dependencies should be of the form X(iter) -> NextIter(iter+1)
				Dim fi As FrameIter = depFrameIter.clone()
				fi.setIteration(fi.getIteration() + 1)
				es = New ExecStep(ExecType.OP, opName, fi)
				For Each s As String In inputs
					Dim req As ExecStep = getExecStepForVar(s, depFrameIter)
					dt.addDependency(es, req)
				Next s
			Else
				For Each s As String In inputs
					Dim req As ExecStep = getExecStepForVar(s, depFrameIter)
					dt.addDependency(es, req)
				Next s
			End If

			If cdOps IsNot Nothing Then
				For Each s As String In cdOps
					Dim req As ExecStep = getExecStepForVar(s, depFrameIter)
					dt.addDependency(es, req)
				Next s
			End If

			If cdVars IsNot Nothing Then
				For Each s As String In cdVars

				Next s
			End If
		End Sub

		''' <summary>
		''' Get the ExecStep for the given variable, given execution is happening at the specified frame/iteration
		''' </summary>
		Protected Friend Overridable Function getExecStepForVar(ByVal varName As String, ByVal frameIter As FrameIter) As ExecStep
			Dim v As Variable = sameDiff.getVariables().get(varName)
			Dim vt As VariableType = v.getVariable().getVariableType()
			If vt = VariableType.VARIABLE Then
				Return New ExecStep(ExecType.VARIABLE, v.getVariable().name(), New FrameIter(OUTER_FRAME, 0, Nothing))
			ElseIf vt = VariableType.PLACEHOLDER Then
				Return New ExecStep(ExecType.PLACEHOLDER, v.getVariable().name(), New FrameIter(OUTER_FRAME, 0, Nothing))
			ElseIf vt = VariableType.CONSTANT Then
				Return New ExecStep(ExecType.CONSTANT, v.getVariable().name(), New FrameIter(OUTER_FRAME, 0, Nothing))
			Else
				'Array type. Must be output of an op
				If v.getOutputOfOp() Is Nothing Then
					v = sameDiff.getVariables().get(stripVarSuffix(v.getName()))
				End If

				Dim outOfOp As String = v.getOutputOfOp()
				Dim sdo As SameDiffOp = sameDiff.getOps().get(outOfOp)

				If sdo Is Nothing Then
					Throw New System.InvalidOperationException("Samediff output op named " & v.getName() & " did not have any ops associated with it.")
				End If

				If TypeOf sdo.Op Is Switch Then
					'For dependency tracking purposes, we track left and right output branches of switch op separately
					'Otherwise, ops depending both branches will be marked as available if we just rely on "op has been executed"
					Dim opOutputs As IList(Of String) = sdo.getOutputsOfOp()
					Dim idx As Integer = opOutputs.IndexOf(v.getName())
					If idx = 0 Then
						'Left branch
						Return New ExecStep(ExecType.SWITCH_L, outOfOp, frameIter)
					ElseIf idx = 1 Then
						'Right branch
						Return New ExecStep(ExecType.SWITCH_R, outOfOp, frameIter)
					Else
						'Should never happen
						Throw New System.InvalidOperationException("Expected variable """ & v.getName() & """ to be an output of operation """ & outOfOp & """, but op output variables are: " & opOutputs)
					End If
				ElseIf TypeOf sdo.Op Is Enter Then
					Dim e As Enter = DirectCast(sdo.Op, Enter)

					'For enter ops, "constant=true" enter ops are available for ALL iterations, hence use iter=0
					'For constant=false, these are only available at iteration 0 - so use *current* iteration, same as all other ops
					' (which is this case, won't be triggered on iter > 0 - as desired/expected)
					If e.isConstant() Then
						Dim fi As FrameIter = frameIter.clone()
						fi.setIteration(0)

						'Nested constant enter case: Iteration 0 all the way down...
						Dim inVarName As String = sdo.getInputsToOp()(0)
						Dim parentFrame As FrameIter = fi.getParentFrame()
						Do While parentFrame IsNot Nothing
							Dim var As Variable = sameDiff.getVariables().get(inVarName)
							If var.getOutputOfOp() IsNot Nothing Then
								Dim opName As String = var.getOutputOfOp()
								Dim sdo2 As SameDiffOp = sameDiff.getOps().get(opName)
								If TypeOf sdo2.Op Is Enter Then
									Dim e2 As Enter = DirectCast(sdo.Op, Enter)
									If e2.isConstant() Then
										parentFrame.setIteration(0)
										parentFrame = parentFrame.getParentFrame()
										inVarName = sdo2.getInputsToOp()(0)
									Else
										Exit Do
									End If
								Else
									Exit Do
								End If
							Else
								Exit Do
							End If
						Loop

						Return New ExecStep(ExecType.OP, outOfOp, fi)
					End If

					'Intentional fall-through to default case
				End If
				Return New ExecStep(ExecType.OP, outOfOp, frameIter)
			End If
		End Function

		''' <summary>
		''' Initialize the subgraph - the subgraph and subgraphOps sets
		''' This works our what ops and variables we might need to execute to get the requested outputs.
		''' In general, this is a subset of the graph.
		''' </summary>
		''' <param name="variables"> Set of output variables we need </param>
		Protected Friend Overridable Sub initSubgraph(ByVal variables As ISet(Of String))
			'Step 1: determine subgraph structure we actually need to execute
			Dim processingQueue As New LinkedList(Of String)(variables)

			'Note subgraph initially should include placeholders and constants
			Do While processingQueue.Count > 0
				Dim varName As String = processingQueue.RemoveFirst()
				Dim opName As String = (If(sameDiff.getVariableOutputOp(varName) Is Nothing, Nothing, sameDiff.getVariableOutputOp(varName).getOwnName()))

				If Not subgraph.Contains(varName) Then
					Dim opInputs() As String = If(opName Is Nothing, Nothing, sameDiff.getInputsForOp(sameDiff.getOpById(opName)))
					Dim currVar As Variable = sameDiff.getVariables().get(varName)
					log.trace("Adding " & varName & " to subgraph for output.")
					Dim opInputsFor As IList(Of String) = currVar.getInputsForOp()
					Dim controlDeps As IList(Of String) = currVar.getControlDeps()
					Dim output As String = currVar.getOutputOfOp()
					Dim numInputs As Integer = (If(opInputs Is Nothing, 0, opInputs.Length))
					If controlDeps IsNot Nothing Then
						'Also count variable control dependencies as inputs - even a constant may not be available for use
						' until after execution of some other ops (for example, in conditional operations)
						numInputs += controlDeps.Count
					End If
					If numInputs = 0 AndAlso opName IsNot Nothing Then
						zeroInputOpsInSubgraph.Add(opName)
					End If


					subgraph.Add(varName)

					If opName IsNot Nothing Then
						subgraphOps.Add(opName)
					End If

					If controlDeps IsNot Nothing Then
						'If variable has control dependencies, it's not available right away... to make it available,
						' we need the "inputs" to be available first. This is mainly used for TF import.
						For Each s As String In controlDeps
							If Not subgraph.Contains(s) Then
								processingQueue.AddLast(s)
							End If
						Next s
					End If


				End If

				If opName IsNot Nothing Then
					'To execute op - and hence get this variable: need inputs to that op
					Dim opById As DifferentialFunction = sameDiff.getOpById(opName)
					Dim inputs() As String = sameDiff.getInputsForOp(opById)
					For Each s2 As String In inputs
						If Not subgraph.Contains(s2) Then
							processingQueue.AddLast(s2)
						End If
					Next s2

					'To execute op - and hence get this variable - we also need control deps
					Dim opControlDeps As IList(Of String) = sameDiff.getOps().get(opName).getControlDeps()
					If opControlDeps IsNot Nothing Then
						For Each s2 As String In opControlDeps
							If Not subgraph.Contains(s2) Then
								processingQueue.AddLast(s2)
							End If
						Next s2
					End If
				End If
			Loop
		End Sub

		''' <summary>
		''' Preprocess the placeholder values, if required.
		''' Mainly reserved for casting in the case of InferenceSession
		''' </summary>
		''' <param name="placeholders"> Placeholders to preprocess. </param>
		''' <returns> Preprocessed placeholders </returns>
		Protected Friend Overridable Function preprocessPlaceholders(ByVal placeholders As IDictionary(Of String, T), ByVal at As At) As IDictionary(Of String, T)
			Return placeholders
		End Function

		''' <summary>
		''' Post process the session output values, if required.
		''' Override if required in session subclasses
		''' </summary>
		''' <param name="output"> Output to be returned to the user </param>
		''' <returns> Post processed output </returns>
		Protected Friend Overridable Function postProcessOutput(ByVal output As IDictionary(Of String, T)) As IDictionary(Of String, T)
			Return output
		End Function

		''' <summary>
		''' Get the constant or variable output - for example, constant array or constant shape.
		''' Note that both constants and variables (i.e., VariableType.CONSTANT and VariableType.VARIABLE) are the same
		''' for all frames and iterations.
		''' </summary>
		''' <param name="variableName"> The name of the variable to get the constant for </param>
		''' <returns> The constant </returns>
		Public MustOverride Function getConstantOrVariable(ByVal variableName As String) As T

		''' <summary>
		''' Get the parameterized op to execute - for example, the op/DifferentialFunction with all inputs set
		''' </summary>
		''' <param name="opName">           Name of the op </param>
		''' <param name="frameIter">        The frame and iteration of the op outputs </param>
		''' <param name="inputs">           The inputs to the op (excluding constants/placeholders) - for the specific frame + iteration </param>
		''' <param name="allIterInputs">    The inputs - those that are not iteration-specific (mainly Enter op vars, which might be used in all iterations but are only executed once on iter 0) </param>
		''' <param name="constAndPhInputs"> The constant and placeholder inputs - used for all frames/iterations </param>
		''' <param name="allReqVariables">  All required variables requested for the current session execution (not just the current op outputs) </param>
		''' <returns> The parameterized op </returns>
		Public MustOverride Function getAndParameterizeOp(ByVal opName As String, ByVal frameIter As FrameIter, ByVal inputs As ISet(Of VarId), ByVal allIterInputs As ISet(Of VarId), ByVal constAndPhInputs As ISet(Of String), ByVal placeholderValues As IDictionary(Of String, T), ByVal allReqVariables As ISet(Of String)) As O

		''' <summary>
		''' Execute the op - calculate INDArrays, or shape info, etc
		''' </summary>
		''' <param name="op">              Operation to exit. This should be parameterized (i.e., all inputs set) </param>
		''' <param name="outputFrameIter"> The frame and iteration of the outputs </param>
		''' <param name="inputs">          The specific input arrays for the op </param>
		''' <param name="allReqVariables"> All required variables requested for the current session execution (not just the current op outputs) </param>
		''' <returns> The outputs of the op </returns>
		Public MustOverride Function getOutputs(ByVal op As O, ByVal outputFrameIter As FrameIter, ByVal inputs As ISet(Of VarId), ByVal allIterInputs As ISet(Of VarId), ByVal constAndPhInputs As ISet(Of String), ByVal listeners As IList(Of Listener), ByVal at As At, ByVal batch As MultiDataSet, ByVal allReqVariables As ISet(Of String)) As T()

		''' <summary>
		''' Get the VarId from the specified name. The VarId should be in one or the other of the collections,
		''' and only one VarId with that name should exist
		''' </summary>
		Protected Friend Shared Function lookup(ByVal name As String, ByVal varIds As ICollection(Of VarId), ByVal varIds2 As ICollection(Of VarId), ByVal exceptionOnNotFound As Boolean) As VarId
			Dim vid As VarId = If(varIds Is Nothing, Nothing, lookup(name, varIds, False))
			If vid Is Nothing AndAlso varIds2 IsNot Nothing Then
				vid = lookup(name, varIds2, False)
			End If

			If vid Is Nothing AndAlso exceptionOnNotFound Then
				Throw New Exception("Could not find VarId for input """ & name & """")
			End If
			Return vid
		End Function

		''' <summary>
		''' Get the VarId from the specified name. The VarId should be in the collection,
		''' and only one VarId with that name should exist
		''' </summary>
		Protected Friend Shared Function lookup(ByVal name As String, ByVal varIds As ICollection(Of VarId), ByVal exceptionOnNotFound As Boolean) As VarId
			For Each vid As VarId In varIds
				If vid.getVariable().Equals(name) Then
					Return vid
				End If
			Next vid
			If exceptionOnNotFound Then
				Throw New Exception("Could not find VarId to input " & name)
			End If
			Return Nothing
		End Function

		''' <summary>
		''' VarId: identifies the value of a variable in a specific frame and frame iteration<br>
		''' Note that frames can be nested - which generally represents nested loop situations.<br>
		''' Used for 2 places:<br>
		''' (a) to identify variables that are available for execution<br>
		''' (b) to store results<br>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @AllArgsConstructor public static class VarId
		Public Class VarId
			Friend variable As String
			Friend frame As String
			Friend iteration As Integer
			Friend parentFrame As FrameIter

			Public Overrides Function ToString() As String
				Return "VarId(""" & variable & """,""" & frame & """," & iteration & ",parent=" & parentFrame & ")"
			End Function

			''' <returns> FrameIter corresponding to the VarId </returns>
			Public Overridable Function toFrameIter() As FrameIter
				Return New FrameIter(frame, iteration, parentFrame)
			End Function
		End Class

		''' <summary>
		''' ExecType: Execution type, as used in ExecStep<br>
		''' OP: Operation execution<br>
		''' VARIABLE: Variable "execution", mainly used to trigger ops that depend on the variable<br>
		''' CONSTANT: As per variable<br>
		''' PLACEHOLDER: As per variable<br>
		''' SWITCH_L and SWITCH_R: This is a bit of a hack to account for the fact that only one of
		''' the switch branches (left or right) will ever be available; without this, once the switch op is executed, we'll
		''' (incorrectly) conclude that *both* branches can be executed<br>
		''' EXEC_START: Start of execution<br>
		''' CONTROL_DEP: Control dependency for op. Used for TF import, due to its odd "constant depends on op in a frame" behaviour
		''' </summary>
		Protected Friend Enum ExecType
			OP
			VARIABLE
			CONSTANT
			PLACEHOLDER
			SWITCH_L
			SWITCH_R
			EXEC_START
			CONTROL_DEP

		End Enum

		''' <summary>
		''' ExecStep represents a single execution step, for a single op (or variable/constant etc) at a specific frame/iteration
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @EqualsAndHashCode protected static class ExecStep
		Protected Friend Class ExecStep
			Protected Friend ReadOnly type As ExecType
			Protected Friend ReadOnly name As String
			Protected Friend ReadOnly frameIter As FrameIter

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected ExecStep(@NonNull ExecType execType, @NonNull String name, FrameIter frameIter)
			Protected Friend Sub New(ByVal execType As ExecType, ByVal name As String, ByVal frameIter As FrameIter)
				Me.type = execType
				Me.name = name
				Me.frameIter = frameIter
			End Sub

			Protected Friend Overridable Function toVarId() As VarId
				Return New VarId(name, frameIter.getFrame(), frameIter.getIteration(), frameIter.getParentFrame())
			End Function

			Public Overrides Function ToString() As String
				Return "ExecStep(" & type & ",name=""" & name & """," & frameIter & ")"
			End Function
		End Class

		''' <summary>
		''' Used in getting the next ExecStep that matches the specified (current) frame/iteration
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @AllArgsConstructor @NoArgsConstructor protected class ExecStepPredicate implements org.nd4j.common.function.Predicate<ExecStep>
		Protected Friend Class ExecStepPredicate
			Implements Predicate(Of ExecStep)

			Private ReadOnly outerInstance As AbstractSession(Of T, O)

			Public Sub New(ByVal outerInstance As AbstractSession(Of T, O))
				Me.outerInstance = outerInstance
			End Sub


			Protected Friend currentFrame As String
			Protected Friend currentFrameIter As Integer
			Protected Friend currParentFrame As FrameIter

			Public Overridable Function test(ByVal execStep As ExecStep) As Boolean
				Return currentFrame.Equals(execStep.getFrameIter().getFrame()) AndAlso currentFrameIter = execStep.getFrameIter().getIteration() AndAlso (currParentFrame Is Nothing AndAlso execStep.getFrameIter().getParentFrame() Is Nothing OrElse currParentFrame.Equals(execStep.getFrameIter().getParentFrame()))
			End Function
		End Class


	End Class

End Namespace