Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports At = org.nd4j.autodiff.listeners.At
Imports Listener = org.nd4j.autodiff.listeners.Listener
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports VariableType = org.nd4j.autodiff.samediff.VariableType
Imports ArrayCacheMemoryMgr = org.nd4j.autodiff.samediff.internal.memory.ArrayCacheMemoryMgr
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.linalg.api.ops
Imports DefaultOpExecutioner = org.nd4j.linalg.api.ops.executioner.DefaultOpExecutioner
Imports org.nd4j.linalg.api.ops.impl.controlflow.compat
Imports ExternalErrorsFunction = org.nd4j.linalg.api.ops.impl.layers.ExternalErrorsFunction
Imports Concat = org.nd4j.linalg.api.ops.impl.shape.Concat
Imports Stack = org.nd4j.linalg.api.ops.impl.shape.Stack
Imports org.nd4j.linalg.api.ops.impl.shape.tensorops
Imports Assert = org.nd4j.linalg.api.ops.impl.transforms.Assert
Imports GradientBackwardsMarker = org.nd4j.linalg.api.ops.impl.transforms.gradient.GradientBackwardsMarker
Imports Identity = org.nd4j.linalg.api.ops.impl.transforms.same.Identity
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports org.nd4j.common.primitives
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
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
'ORIGINAL LINE: @Slf4j public class InferenceSession extends AbstractSession<org.nd4j.linalg.api.ndarray.INDArray, org.nd4j.common.primitives.Pair<SameDiffOp,OpContext>>
	Public Class InferenceSession
		Inherits AbstractSession(Of INDArray, Pair(Of SameDiffOp, OpContext))

		Private Shared ReadOnly SCOPE_PANIC_MSG As String = "If required, arrays in workspaces can be detached using INDArray.detach() before being passed to the SameDiff instance." & vbLf & "Alternatively, arrays defined in a workspace must be replaced after the workspace has been closed."

		Protected Friend Const KERAS_TRAIN_TEST As String = "keras_learning_phase"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private SessionMemMgr mmgr;
		Private mmgr As SessionMemMgr 'Used for allocating and deallocating memory
		''' <summary>
		''' Array use tracker: What needs to happen before the array can be closed/released?
		''' As the name suggests, the INDArrays are tracked using qbject identity, not equality
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private IdentityDependencyTracker<org.nd4j.linalg.api.ndarray.INDArray, Dep> arrayUseTracker = new IdentityDependencyTracker<>();
		Private arrayUseTracker As New IdentityDependencyTracker(Of INDArray, Dep)()


		Private opContexts As IDictionary(Of String, OpContext) = New Dictionary(Of String, OpContext)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public InferenceSession(@NonNull SameDiff sameDiff)
		Public Sub New(ByVal sameDiff As SameDiff)
			MyBase.New(sameDiff)
			mmgr = New ArrayCacheMemoryMgr()
		End Sub

		Protected Friend Overrides Function preprocessPlaceholders(ByVal placeholders As IDictionary(Of String, INDArray), ByVal at As At) As IDictionary(Of String, INDArray)
			arrayUseTracker.clear()

			'We'll also use this method as a "pre execution" hook-in, to mark variables as something we should never deallocate
			'This occurs by never marking these "ConstantDep" and "VariableDep" instances as satisfied, so there's always
			' an unsatisfied dependency for them in the array use tracker
			'TODO we shouldn't be clearing this on every single iteration, in 99.5% of cases variables will be same as last iteration...
			For Each v As SDVariable In sameDiff.variables()
				If v.getVariableType() = VariableType.CONSTANT Then
					arrayUseTracker.addDependency(v.Arr, New ConstantDep(v.name()))
				ElseIf v.getVariableType() = VariableType.VARIABLE Then
					arrayUseTracker.addDependency(v.Arr, New VariableDep(v.name()))
				End If
			Next v

			'Workaround for some TF/Keras based models that require explicit train/test as a placeholder
			Dim kerasWorkaround As Boolean = False
			Dim phs As IList(Of String) = sameDiff.inputs()
			If phs IsNot Nothing AndAlso phs.Count > 0 Then
				For Each s As String In phs
					If s.EndsWith(KERAS_TRAIN_TEST, StringComparison.Ordinal) AndAlso Not placeholders.ContainsKey(s) Then
						' The behaviour of some Keras layers (like GRU) differs depending on whether the model is training.
						' We provide this value directly, unless the user has provided this manually
						Dim scalar As INDArray = mmgr.allocate(False, DataType.BOOL).assign(at.operation().isTrainingPhase())
						placeholders = New Dictionary(Of String, INDArray)(placeholders) 'Array might be singleton, or otherwise unmodifiable
						placeholders(s) = scalar
						kerasWorkaround = True
					End If
				Next s
			End If


			If placeholders Is Nothing OrElse placeholders.Count = 0 Then
				Return placeholders
			End If

			'Handle casting of the input array automatically.
			'The idea here is to avoid unexpected errors if the user (for example) tries to perform inference with a double
			' array for a float placeholder
			'TODO eventually we might have ops that support multiple input types, and hence won't need this casting
			Dim [out] As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			For Each e As KeyValuePair(Of String, INDArray) In placeholders.SetOfKeyValuePairs()
				Preconditions.checkState(sameDiff.hasVariable(e.Key), "Invalid placeholder passed for execution: " & "No variable/placeholder with name %s exists", e.Key)
				Dim arr As INDArray = e.Value
				'First: check workspaces
				If arr.Attached Then
					Dim ws As MemoryWorkspace = If(arr.data() Is Nothing, Nothing, arr.data().ParentWorkspace)
					If ws IsNot Nothing AndAlso ws.WorkspaceType <> MemoryWorkspace.Type.CIRCULAR Then
						If Not ws.ScopeActive Then
							Throw New ND4JIllegalStateException("Placeholder """ & e.Key & """ array uses leaked workspace pointer from workspace [" & ws.Id & "]: Workspace the array was defined in is no longer open." & vbLf & "All open workspaces: " & DefaultOpExecutioner.allOpenWorkspaces() & vbLf & SCOPE_PANIC_MSG)
						End If

						If ws.GenerationId <> arr.data().GenerationId Then
							Throw New ND4JIllegalStateException("Placeholder """ & e.Key & """ array uses outdated workspace pointer from workspace [" & ws.Id & "]: Workspace array was defined in has been closed and reopened at least once since array creation. Array WS iteration: " & arr.data().GenerationId & ". Workspace current iteration: " & ws.GenerationId & vbLf & "All open workspaces: " & DefaultOpExecutioner.allOpenWorkspaces() & vbLf & SCOPE_PANIC_MSG)
						End If
					End If
				End If


				'Second: cast the input to the required type
				'TODO For the casting case, we SHOULD actually deallocate this when we're done with it, which is usually sooner than "exec done"
				Dim dt As DataType = sameDiff.getVariable(e.Key).dataType()
				If kerasWorkaround AndAlso e.Key.EndsWith(KERAS_TRAIN_TEST) Then
					arrayUseTracker.addDependency(arr, New ExecDoneDep())
				ElseIf arr.dataType() = dt Then
					'Mark as a placeholder array in the array use tracker, so we never deallocate this array...
					arrayUseTracker.addDependency(e.Value, New PlaceholderDep(e.Key))
				Else
					Dim cast As INDArray = mmgr.allocate(False, dt, arr.shape())
					cast.assign(arr)
					arr = cast
					'This array CAN be deallocated once consumed, because of the cast
					'TODO we can likely close this sooner
					arrayUseTracker.addDependency(arr, New ExecDoneDep())
				End If
				[out](e.Key) = arr
			Next e

			Return [out]
		End Function

		Protected Friend Overrides Function postProcessOutput(ByVal output As IDictionary(Of String, INDArray)) As IDictionary(Of String, INDArray)

			'For any queued (not yet processed) ops - mark them as satisfied, so we can deallocate any arrays
			' that are waiting on them
			If dt.hasNewAllSatisfied() Then
				Dim execSteps As IList(Of ExecStep) = dt.getNewAllSatisfiedList()
				For Each es As ExecStep In execSteps
					If es.getType() = ExecType.OP Then
						Dim od As New OpDep(es.getName(), es.getFrameIter().getFrame(), es.getFrameIter().getIteration(), es.getFrameIter().getParentFrame())
						arrayUseTracker.markSatisfied(od, True)
					End If
				Next es
			End If

			'Also mark "end of execution" for array dependency tracker. Mainly used for TensorArray arrays at present.
			'TODO Optimize for reduced memory for some TensorArray operations - i.e., close/deallocate earlier
			arrayUseTracker.markSatisfied(New ExecDoneDep(), True)
			If arrayUseTracker.hasNewAllSatisfied() Then
				Dim l As IList(Of INDArray) = arrayUseTracker.getNewAllSatisfiedList()
				For Each arr As INDArray In l
					mmgr.release(arr)
				Next arr
			End If

			Return output
		End Function

		Public Overrides Function getOutputs(ByVal opPair As Pair(Of SameDiffOp, OpContext), ByVal outputFrameIter As FrameIter, ByVal opInputs As ISet(Of VarId), ByVal allIterInputs As ISet(Of VarId), ByVal constAndPhInputs As ISet(Of String), ByVal listeners As IList(Of Listener), ByVal at As At, ByVal batch As MultiDataSet, ByVal allReqVariables As ISet(Of String)) As INDArray()
			Dim op As SameDiffOp = opPair.First
			at.setFrameIter(outputFrameIter)
			If listeners IsNot Nothing AndAlso listeners.Count > 0 Then
				Dim sdOp As SameDiffOp = sameDiff.getOps().get(op.Op.getOwnName())
				For Each l As Listener In listeners
					If l.isActive(at.operation()) Then
						l.preOpExecution(sameDiff, at, sdOp, opPair.Second)
					End If
				Next l
			End If

			Dim [out]() As INDArray = doExec(op.Op, opPair.Right, outputFrameIter, opInputs, allIterInputs, constAndPhInputs)

			If log.isTraceEnabled() Then
				Dim sb As New StringBuilder()
				sb.Append(op.Name).Append(" - ").Append(outputFrameIter).Append(" outputs: ")
				Dim opOutNames As IList(Of String) = op.getOutputsOfOp()
				For i As Integer = 0 To [out].Length - 1
					If i > 0 Then
						sb.Append(", ")
					End If
					sb.Append("(").Append(i).Append(" - ").Append(opOutNames(i)).Append(" = ").Append(If([out](i) Is Nothing, Nothing, [out](i).Id)).Append(")")
				Next i
				log.trace(sb.ToString())
			End If

			'Call listeners, before we (maybe) deallocate input arrays
			If listeners IsNot Nothing AndAlso listeners.Count > 0 Then
				Dim namedOuts As IDictionary(Of String, INDArray) = Nothing

				For Each l As Listener In listeners
					If l.isActive(at.operation()) Then
						'Lazily create map, only if required
						If namedOuts Is Nothing Then
							Dim namedOutsBuilder As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()

							For i As Integer = 0 To [out].Length - 1
								namedOutsBuilder(op.outputsOfOp_Conflict(i)) = [out](i)
							Next i
							namedOuts = Collections.unmodifiableMap(namedOutsBuilder)
						End If


						l.opExecution(sameDiff, at, batch, op, opPair.Second, [out])

						For Each varName As String In namedOuts.Keys
							l.activationAvailable(sameDiff, at, batch, op, varName, namedOuts(varName))
						Next varName
					End If
				Next l
			End If
			op.Op.clearArrays()
			If opPair.Second IsNot Nothing Then
				opPair.Second.purge()
			End If


			'Record array uses for memory management/deallocation
			Dim o As SameDiffOp = sameDiff.getOps().get(op.Name)
			Dim outVarNames As IList(Of String) = o.getOutputsOfOp()
			For i As Integer = 0 To [out].Length - 1
				If [out](i) Is Nothing AndAlso TypeOf o.Op Is Switch Then
					Continue For 'Switch case: we only ever get one of 2 outputs, other is null (branch not executed)
				End If

				Dim name As String = outVarNames(i)
				Dim v As Variable = sameDiff.getVariables().get(name)
				Dim inputsForOps As IList(Of String) = v.getInputsForOp()
				If inputsForOps IsNot Nothing Then
					For Each opName As String In inputsForOps
						'Only add dependencies if we actually need the op this feeds into, otherwise the dependency
						' will will never be marked as satisfied
						If Not subgraphOps.Contains(opName) Then
							Continue For
						End If

						Dim forOp As SameDiffOp = sameDiff.getOps().get(opName)

						'TODO do switch or merge need special handling also?
						If TypeOf forOp.Op Is Enter Then
							Dim e As Enter = DirectCast(forOp.Op, Enter)
							If e.isConstant() Then
	'                        
	'                        Contant enter case: Need to keep this array around for the entire duration of the frame, including
	'                        any nested frames, and all iterations.
	'                        Unfortunately, we don't know exactly when we're done with a frame for good
	'                        This isn't a great solution, but other possibilities (frame close, trying to detect all exit ops,
	'                        detecting return to parent frame, etc all fail in certain circumstances, such as due to control dependencies
	'                        on variables).
	'                         
								Dim d As Dep = New ExecDoneDep()
								arrayUseTracker.addDependency([out](i), d)
							Else
								Dim d As Dep = New OpDep(opName, e.FrameName, 0, outputFrameIter)
								arrayUseTracker.addDependency([out](i), d) 'Op defined by "d" needs to be executed before specified array can be closed
							End If
						ElseIf TypeOf forOp.Op Is NextIteration Then
							'The array is needed by the NEXT iteration op, not the current one
							Dim d As Dep = New OpDep(opName, outputFrameIter.getFrame(), outputFrameIter.getIteration() + 1, outputFrameIter.getParentFrame())
							arrayUseTracker.addDependency([out](i), d)
						ElseIf TypeOf forOp.Op Is [Exit] Then
							'The array is needed at the EXIT frame (i.e., parent frame), not the inner/just executed one
							Dim fi As FrameIter = outputFrameIter.getParentFrame()
							Dim d As Dep = New OpDep(opName, fi.getFrame(), fi.getIteration(), fi.getParentFrame())
							arrayUseTracker.addDependency([out](i), d) 'Op defined by "d" needs to be executed before specified array can be closed
						Else
							'All other ops...
							Dim d As Dep = New OpDep(opName, outputFrameIter.getFrame(), outputFrameIter.getIteration(), outputFrameIter.getParentFrame())
							arrayUseTracker.addDependency([out](i), d) 'Op defined by "d" needs to be executed before specified array can be closed
						End If
					Next opName
				End If

				If OUTER_FRAME.Equals(outputFrameIter.getFrame()) AndAlso allReqVariables.Contains(name) Then
					'This variable is an output, record that in the array use tracker, so we don't deallocate it
					arrayUseTracker.addDependency([out](i), New ReqOutputDep(name))
				ElseIf (inputsForOps Is Nothing OrElse inputsForOps.Count = 0) AndAlso Not arrayUseTracker.hasDependency([out](i)) Then
					'This particular array is not actually needed anywhere, so we can deallocate in immediately
					'Possibly only a control dependency, or only one of the outputs of a multi-output op is used
					If log.isTraceEnabled() Then
						log.trace("Found array id {} (output of {}) not required anywhere, deallocating", [out](i).Id, o.Name)
					End If
					mmgr.release([out](i))
				End If
			Next i

			'Mark current op dependency as satisfied...
			Dim d As Dep = New OpDep(op.Name, outputFrameIter.getFrame(), outputFrameIter.getIteration(), outputFrameIter.getParentFrame())
			arrayUseTracker.markSatisfied(d, True)


			'Close any no longer required arrays
			If arrayUseTracker.hasNewAllSatisfied() Then
				Dim canClose As IList(Of INDArray) = arrayUseTracker.getNewAllSatisfiedList()
				For Each arr As INDArray In canClose
					If log.isTraceEnabled() Then
						log.trace("Closing array... id={}, {}", arr.Id, arr.shapeInfoToString())
					End If
					mmgr.release(arr)
				Next arr
			End If

			Return [out]
		End Function

		Public Overridable Function doExec(ByVal op As DifferentialFunction, ByVal opContext As OpContext, ByVal outputFrameIter As FrameIter, ByVal opInputs As ISet(Of VarId), ByVal allIterInputs As ISet(Of VarId), ByVal constAndPhInputs As ISet(Of String)) As INDArray()

			Dim totalInputs As Integer = (If(opInputs Is Nothing, 0, opInputs.Count)) + (If(constAndPhInputs Is Nothing, 0, constAndPhInputs.Count)) + (If(allIterInputs Is Nothing, 0, allIterInputs.Count))

			Dim constPhInput As Boolean = (opInputs Is Nothing OrElse opInputs.Count = 0) AndAlso (allIterInputs Is Nothing OrElse allIterInputs.Count = 0)

			If TypeOf op Is Identity Then
				Dim i As Identity = DirectCast(op, Identity)
				Dim argNames() As String = i.argNames()
				Preconditions.checkState(argNames.Length = 1, "Expected only 1 arg name in identity op, got %s", DirectCast(argNames, Object))
				Dim vid As VarId = outputFrameIter.toVarId(argNames(0))

				Dim orig As INDArray = nodeOutputs(vid)
				Return New INDArray(){orig}
			ElseIf TypeOf op Is Switch Then
				Dim s As Switch = DirectCast(op, Switch)
				Dim argNames() As String = s.argNames() 'Order: input, boolean array
				Dim vidPredicate As VarId = outputFrameIter.toVarId(argNames(1))
				Dim predicate As INDArray = Me.nodeOutputs(vidPredicate)
				If predicate Is Nothing AndAlso constAndPhInputs.Count > 0 AndAlso constAndPhInputs.Contains(argNames(1)) Then
					'Constant predicate...
					predicate = Me.nodeOutputs(New VarId(argNames(1), OUTER_FRAME, 0, Nothing))
				End If
				Preconditions.checkNotNull(predicate, "Error during graph execution: Predicate array was null. VarId=%s", vidPredicate)
				Preconditions.checkState(predicate.Scalar AndAlso predicate.dataType() = DataType.BOOL, "Expected boolean predicate: got %ndSInfo", predicate)
				Dim vid As VarId = outputFrameIter.toVarId(argNames(0))
				If predicate.getDouble(0) = 0.0 Then
					Return New INDArray(){Me.nodeOutputs(vid), Nothing}
				Else
					Return New INDArray(){Nothing, Me.nodeOutputs(vid)}
				End If
			ElseIf TypeOf op Is Enter Then
				'Enter op: forwards input to specified execution frame
				Dim e As Enter = DirectCast(op, Enter)
				Dim input() As String = e.argNames()
				Preconditions.checkState(input.Length = 1, "Expected only 1 arg name for enter op: got %s", DirectCast(input, Object))
				Preconditions.checkState(totalInputs = 1, "Expected exactly 1 op input for Enter op ""%s"", got %s+%s", e.getOwnName(), opInputs, constAndPhInputs)

				Dim inputVarId As VarId
				If constPhInput Then
					'Constant or placeholder
					inputVarId = New VarId(constAndPhInputs.GetEnumerator().next(), OUTER_FRAME, 0, Nothing)
				ElseIf allIterInputs IsNot Nothing AndAlso allIterInputs.Count > 0 Then
					inputVarId = allIterInputs.GetEnumerator().next()
				Else
					inputVarId = opInputs.GetEnumerator().next()
				End If
				Dim enterInput As INDArray = Me.nodeOutputs(inputVarId)

				Preconditions.checkNotNull(enterInput, "Could not get enter op ""%s"" input: output variable %s - %s", e.getOwnName(), e.outputVariablesNames(), outputFrameIter)
				Return New INDArray(){enterInput}
			ElseIf TypeOf op Is [Exit] Then
				'Exit node forwards input to parent frame

				Dim inputVarId As VarId
				If constPhInput Then
					'Constant or placeholder
					inputVarId = New VarId(constAndPhInputs.GetEnumerator().next(), OUTER_FRAME, 0, Nothing)
				ElseIf allIterInputs IsNot Nothing AndAlso allIterInputs.Count > 0 Then
					inputVarId = allIterInputs.GetEnumerator().next()
				Else
					inputVarId = opInputs.GetEnumerator().next()
				End If
				Dim exitInput As INDArray = Me.nodeOutputs(inputVarId)
				Return New INDArray(){exitInput}
			ElseIf TypeOf op Is NextIteration Then
				'NextIteration op: forwards its single input to the output of the current frame, but increments the iteration number
				Preconditions.checkState(totalInputs = 1, "Expected exactly 1 op input for NextIteration: got %s+%s", opInputs, constAndPhInputs)
				Dim [in] As VarId = (If(allIterInputs IsNot Nothing AndAlso allIterInputs.Count > 0, allIterInputs.GetEnumerator().next(), opInputs.GetEnumerator().next()))
				Preconditions.checkState(outputFrameIter.getFrame().Equals([in].getFrame()), "Expected same frame for NextIteration input vs. output:" & " got input %s, output %s", [in], outputFrameIter)
				Preconditions.checkState(outputFrameIter.getIteration() = [in].getIteration() + 1, "Expected output iteration for NextIteration output to" & " be 1 larger than the input iteration. Input: %s, output %s", [in], outputFrameIter)

				Dim inArr As INDArray = Me.nodeOutputs([in])
				If inArr Is Nothing Then
					Preconditions.throwStateEx("Could not find array for NextIteration operation %s with output %s (frame=%s, iteration=%s)", op.getOwnName(), sameDiff.getOps().get(op.getOwnName()).getOutputsOfOp().get(0), outputFrameIter.getFrame(), outputFrameIter.getIteration())
				End If
				Return New INDArray(){inArr}
			ElseIf TypeOf op Is Merge Then
				'Merge available for forward pass when any of its inputs are available. When multiple are available, behaviour
				' is undefined
				Dim m As Merge = DirectCast(op, Merge)
				Dim [in]() As String = sameDiff.getInputsForOp(op)
				For Each s As String In [in]
					Dim vid As VarId = outputFrameIter.toVarId(s)
					If nodeOutputs.ContainsKey(vid) Then
						log.trace("Returning input ""{}"" for merge node ""{}""", m.getOwnName(), s)
						Dim arr As INDArray = nodeOutputs(vid)
						Preconditions.checkState(arr IsNot Nothing, "Could not find output array for %s", vid)
						Return New INDArray(){arr}
					End If
				Next s
				Throw New System.InvalidOperationException("Merge node " & m.getOwnName() & " has no available inputs (all inputs: " & java.util.Arrays.toString([in]) & ") - should not be executed at this point")
			ElseIf TypeOf op Is LoopCond Then
				'LoopCond just forwards scalar boolean to output
				Dim lc As LoopCond = DirectCast(op, LoopCond)
				Dim argNames() As String = lc.argNames()
				Preconditions.checkState(argNames.Length = 1, "Expected only 1 arg name in LoopCond op, got %s", DirectCast(argNames, Object))
				Dim vid As VarId = outputFrameIter.toVarId(argNames(0))
				Dim arr As INDArray = nodeOutputs(vid)
				Preconditions.checkNotNull(arr, "Input to LoopCond op must not be null")
				Preconditions.checkState(arr.Scalar AndAlso arr.dataType() = DataType.BOOL, "LoopCond input must be a scalar boolean, got %ndShape")
				Return New INDArray(){arr}
			ElseIf TypeOf op Is BaseTensorOp Then
				'TensorOps - special cases...
				Return getOutputsHelperTensorArrayOps(op, outputFrameIter, opInputs, allIterInputs)
			ElseIf TypeOf op Is GradientBackwardsMarker Then
				Dim [out] As INDArray = mmgr.allocate(False, DataType.FLOAT).assign(1.0f)
				Return New INDArray(){[out]}
			ElseIf TypeOf op Is ExternalErrorsFunction Then
				Dim fn As ExternalErrorsFunction = DirectCast(op, ExternalErrorsFunction)
				Dim n As String = fn.GradPlaceholderName
				Dim arr As INDArray = nodeOutputs(New VarId(n, OUTER_FRAME, 0, Nothing))
				Preconditions.checkState(arr IsNot Nothing, "Could not find external errors placeholder array: %s", arr)
				Dim [out] As INDArray = mmgr.allocate(False, arr.dataType(), arr.shape())
				[out].assign(arr)
				Return New INDArray(){[out]}
			ElseIf TypeOf op Is Assert Then
				Dim a As Assert = DirectCast(op, Assert)
				Dim condition As Boolean = opContext.getInputArray(0).getDouble(0) <> 0.0
				If Not condition Then
					'Assertion failed
					Dim s As String = "Assertion failed for operation """ & op.getOwnName() & """ during execution"
					If a.numInputArguments() >= 3 Then
						Dim msg As INDArray = opContext.getInputArray(2)
						If msg IsNot Nothing AndAlso msg.dataType() = DataType.UTF8 Then
							s &= ": " & msg.getString(0)
						End If
					End If
					If a.numInputArguments() >= 5 Then
						Dim arr As INDArray = opContext.getInputArray(4)
						s &= vbLf & arr
					End If
					Throw New System.InvalidOperationException(s)
				End If
				Return CType(opContext.getOutputArrays(), List(Of INDArray)).ToArray()
			ElseIf TypeOf op Is CustomOp Then
				Dim c As CustomOp = DirectCast(op, CustomOp)
				Nd4j.exec(c, opContext)
				Return CType(opContext.getOutputArrays(), List(Of INDArray)).ToArray()
			ElseIf TypeOf op Is Op Then
				Dim o As Op = DirectCast(op, Op)
				Nd4j.exec(o, opContext)
				Return New INDArray(){opContext.getOutputArray(0)}
			Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New System.NotSupportedException("Execution not yet implemented for: " & op.GetType().FullName)
			End If
		End Function

		''' <summary>
		''' Forward pass for TensorArray ops
		''' </summary>
		Public Overridable Function getOutputsHelperTensorArrayOps(ByVal op As DifferentialFunction, ByVal outputFrameIter As FrameIter, ByVal opInputs As ISet(Of VarId), ByVal allIterInputs As ISet(Of VarId)) As INDArray()
	'        
	'        TODO: TensorArray memory management note: For now, we'll close any INDArrays stored in the TensorArray at the end of
	'        graph execution. This uses more memory than necessary for an earlier close strategy, but simplifies memory management.
	'        This should be revisited and optimized later
	'         

			If TypeOf op Is TensorArray Then
				'Create a TensorArray
				Dim vid As VarId = outputFrameIter.toVarId(op.outputVariable().name())
				Preconditions.checkState(Not tensorArrays.ContainsKey(vid), "TensorArray already exists for %s when executing TensorArrayV3", vid)
				tensorArrays(vid) = New List(Of INDArray)()

				' Note that TensorArray has 2 outputs - a 'dummy' SDVariable that represents it, and a second output (return a scalar 0.0)
				Dim dummy As INDArray = mmgr.allocate(False, DataType.BOOL).assign(True)
				Dim scalar As INDArray = mmgr.allocate(False, DataType.FLOAT).assign(0.0)
				Return New INDArray(){dummy, scalar}
			ElseIf TypeOf op Is TensorArrayRead Then
				'Do lookup and return
				'Input 0 is the TensorArray (or dummy variable that represents it). Sometimes (for import) this can be like (TensorArray -> Enter -> TensorArrayRead)
				'Input 1 is the index
				Dim idxSDV As SDVariable = op.arg(1)
				Dim idxArr As INDArray = getArray(idxSDV, opInputs, allIterInputs)
				Preconditions.checkState(idxArr.Scalar, "TensorArrayRead input argument 1 should be scalar - has shape %ndShape", idxArr)
				Dim i As Integer = idxArr.getInt(0)

				Dim inTensorArray As SDVariable = op.arg(0) 'Dummy variable representing the tensor array

				'Work out the frame/iteration:
				Dim v As VarId = (If(opInputs Is Nothing, Nothing, lookup(inTensorArray.name(), opInputs, False)))
				If v Is Nothing AndAlso allIterInputs IsNot Nothing Then
					v = lookup(inTensorArray.name(), allIterInputs, False)
				End If

				Preconditions.checkState(v IsNot Nothing, "Could not find input %s", inTensorArray.name())

				Do While TypeOf sameDiff.getVariableOutputOp(inTensorArray.name()) Is Enter
					'Handle the Enter case: this is like TensorArray -> Enter -> TensorArrayRead
					'TODO also TensorArrayWrite, scatter, etc??
					inTensorArray = sameDiff.getVariableOutputOp(inTensorArray.name()).arg()
					v = v.getParentFrame().toVarId(inTensorArray.name())
				Loop

				Dim list As IList(Of INDArray) = getTensorArrays().get(v)
				Preconditions.checkState(list IsNot Nothing, "Could not find TensorList for %s", v)
				Preconditions.checkState(list.Count > i, "Cannot get index %s from TensorList of size %s (array not present?) - VarId=%s", i, list.Count, v)

				Dim [out] As INDArray = list(i)
				Return New INDArray(){[out]}
			ElseIf TypeOf op Is TensorArrayWrite Then
				'TensorArrayWrite - also has a scalar 0.0 that it returns...
				Dim inTensorArray As SDVariable = op.arg(0) 'Dummy variable representing the tensor array
				'Work out the varid (frame/iteration) of the tensor array:
				Dim tArr As VarId = (If(opInputs Is Nothing, Nothing, lookup(inTensorArray.name(), opInputs, False)))
				If tArr Is Nothing AndAlso allIterInputs IsNot Nothing Then
					tArr = lookup(inTensorArray.name(), allIterInputs, False)
				End If

				Preconditions.checkState(tArr IsNot Nothing, "Could not find input %s", inTensorArray.name())

				Do While TypeOf sameDiff.getVariableOutputOp(inTensorArray.name()) Is Enter
					'Handle the Enter case: this is like TensorArray -> Enter -> TensorArrayWrite
					'TODO also TensorArrayScatter, etc??
					inTensorArray = sameDiff.getVariableOutputOp(inTensorArray.name()).arg()
					tArr = tArr.getParentFrame().toVarId(inTensorArray.name())
				Loop

				'Input 0 is the TensorArray (or dummy variable that represents it) - but sometimes Enter, in TensorArray -> Enter -> TensorARrayRead
				'Input 1 is the index
				'Input 2 is the value to write

				Dim idxName As String = op.arg(1).name()
				Dim idxSDV As SDVariable = sameDiff.getVariable(idxName)
				Dim idxArr As INDArray = getArray(idxSDV, opInputs, allIterInputs)
				Preconditions.checkState(idxArr.Scalar, "Index variable ID for TensorArrayWrite should be a scalar, got %ndShape", idxArr)
				Dim idx As Integer = idxArr.getInt(0)

				Dim inName As String = op.arg(2).name()
				Dim inSDV As SDVariable = sameDiff.getVariable(inName)
				Dim arr As INDArray = getArray(inSDV, opInputs, allIterInputs)
				Preconditions.checkState(arr IsNot Nothing, "Could not find array for %s", inName)

				Preconditions.checkState(tensorArrays.ContainsKey(tArr), "Tensor array does not exist for %s", tArr)
				'TODO is this always safe to insert by index for all execution orders?
				Dim l As IList(Of INDArray) = tensorArrays(tArr) '.set(idx, arr);
				Do While l.Count <= idx
					'Can't use set(int, E) if index >= size
					l.Add(Nothing)
				Loop
				l(idx) = arr

				'Add a dependency
				Dim d As Dep = New ExecDoneDep()
				arrayUseTracker.addDependency(arr, d)

				'Return dummy array
				Dim scalar As INDArray = mmgr.allocate(False, DataType.FLOAT).assign(0.0)
				Return New INDArray(){scalar}
			ElseIf TypeOf op Is TensorArraySize Then
				'Index 0 is the TensorArray (or dummy variable that represents it)
				Dim inTensorArray As SDVariable = op.arg(0) 'Dummy variable representing the tensor array
				'Work out the varid (frame/iteration) of the tensor array:
				Dim tArr As VarId = (If(opInputs Is Nothing, Nothing, lookup(inTensorArray.name(), opInputs, False)))
				If tArr Is Nothing AndAlso allIterInputs IsNot Nothing Then
					tArr = lookup(inTensorArray.name(), allIterInputs, False)
				End If
				Dim l As IList(Of INDArray) = tensorArrays(tArr)
				Preconditions.checkState(l IsNot Nothing, "Could not find TensorArray: %s", tArr)

				Dim scalar As INDArray = mmgr.allocate(False, DataType.INT).assign(l.Count)
				Return New INDArray(){scalar}
			ElseIf TypeOf op Is TensorArrayConcat Then
				Dim inTensorArray As SDVariable = op.arg(0) 'Dummy variable representing the tensor array
				Dim tArr As VarId = (If(opInputs Is Nothing, Nothing, lookup(inTensorArray.name(), opInputs, False)))
				If tArr Is Nothing AndAlso allIterInputs IsNot Nothing Then
					tArr = lookup(inTensorArray.name(), allIterInputs, False)
				End If
				Dim l As IList(Of INDArray) = tensorArrays(tArr)

				Dim c As New Concat(0, CType(l, List(Of INDArray)).ToArray())
				Dim shape As IList(Of LongShapeDescriptor) = c.calculateOutputShape()
				Dim [out] As INDArray = mmgr.allocate(False, shape(0))
				c.setOutputArgument(0, [out])
				Nd4j.exec(c)
				Return New INDArray(){[out]}
			ElseIf TypeOf op Is TensorArrayGather Then
				'Input 0: the TensorArray
				'Input 1: the indices (1d integer vector)

				Dim inTensorArray As SDVariable = op.arg(0) 'Dummy variable representing the tensor array
				Dim tArr As VarId = (If(opInputs Is Nothing, Nothing, lookup(inTensorArray.name(), opInputs, False)))
				If tArr Is Nothing AndAlso allIterInputs IsNot Nothing Then
					tArr = lookup(inTensorArray.name(), allIterInputs, False)
				End If
				Dim l As IList(Of INDArray) = tensorArrays(tArr)
				Preconditions.checkState(l IsNot Nothing, "Could not find TensorArray: %s", tArr)

				Dim indicesName As String = op.arg(1).name()
				Dim indicesSDV As SDVariable = sameDiff.getVariable(indicesName)
				Dim idxArr As INDArray = getArray(indicesSDV, opInputs, allIterInputs)
				Preconditions.checkState(idxArr.Vector, "Indices variable for TensorArrayGather should be a vector, got %ndShape for %s", idxArr, indicesName)
				Preconditions.checkState(idxArr.dataType().isIntType(), "Indices variable for TensorArrayGather should be an integer type, got %s for array %s", idxArr.dataType(), indicesName)

				Dim idxArrInt() As Integer = idxArr.toIntVector()

				'Edge case: -1 means "all"
				Dim newList As IList(Of INDArray) = New List(Of INDArray)()
				If idxArrInt.Length = 1 AndAlso idxArrInt(0) = -1 Then
					CType(newList, List(Of INDArray)).AddRange(l)
				Else
					For Each id As Integer In idxArrInt
						Preconditions.checkState(id >= 0, "Index for TensorArrayGather must be >= 0, got %s", id)
						newList.Add(l(id))
					Next id
				End If

				Dim s As New Stack(CType(newList, List(Of INDArray)).ToArray(), Nothing, 0)
				Dim shape As IList(Of LongShapeDescriptor) = s.calculateOutputShape()
				Dim [out] As INDArray = mmgr.allocate(False, shape(0))
				s.setOutputArgument(0, [out])
				Nd4j.exec(s)
				Return New INDArray(){[out]}
			ElseIf TypeOf op Is TensorArrayScatter Then
				'Scatter values from a rank (N+1)d tensor into specific indices of the TensorArray
				'Input 0: the TensorArray
				'Input 1: the indices (1d integer vector)
				'Input 2: The values to scatter

				Dim inTensorArray As SDVariable = op.arg(0) 'Dummy variable representing the tensor array
				Dim ta As TensorArray = DirectCast(sameDiff.getVariableOutputOp(inTensorArray.name()), TensorArray)
				Dim tArr As VarId = (If(opInputs Is Nothing, Nothing, lookup(inTensorArray.name(), opInputs, False)))
				If tArr Is Nothing AndAlso allIterInputs IsNot Nothing Then
					tArr = lookup(inTensorArray.name(), allIterInputs, False)
				End If

				Dim l As IList(Of INDArray) = tensorArrays(tArr)
				Preconditions.checkState(l IsNot Nothing, "Could not find TensorArray: %s", tArr)

				Dim indicesName As String = op.arg(1).name()
				Dim indicesSDV As SDVariable = sameDiff.getVariable(indicesName)
				Dim idxArr As INDArray = getArray(indicesSDV, opInputs, allIterInputs)
				Preconditions.checkState(idxArr.Vector, "Indices variable for TensorArrayScatter should be a vector, got %ndShape for %s", idxArr, indicesName)
				Preconditions.checkState(idxArr.dataType().isIntType(), "Indices variable for TensorArrayScatter should be an integer type, got %s for array %s", idxArr.dataType(), indicesName)
				Dim idxs() As Integer = idxArr.toIntVector()

				Dim valuesName As String = op.arg(2).name()
				Dim valuesSDV As SDVariable = sameDiff.getVariable(valuesName)
				Dim valuesArr As INDArray = getArray(valuesSDV, opInputs, allIterInputs)

				Do While l.Count <= idxs.Length 'Can't use set(int, E) if index >= size
					l.Add(Nothing)
				Loop

				'Edge case: idxs being [-1] means "all sub arrays" (i.e., "unstack" case)
				If idxs.Length = 1 AndAlso idxs(0) = -1 Then
					idxs = ArrayUtil.range(0, CInt(valuesArr.size(0)))
				End If

				Dim idx() As INDArrayIndex = ArrayUtil.nTimes(valuesArr.rank(), NDArrayIndex.all(), GetType(INDArrayIndex))
				For i As Integer = 0 To idxs.Length - 1
					idx(0) = NDArrayIndex.point(i)
					Dim get As INDArray = mmgr.dup(valuesArr.get(idx))
					Dim outIdx As Integer = idxs(i)
					If valuesArr.rank() = 1 AndAlso get.rank() > 0 Then
						get = get.reshape()
					End If

					'reflect the expanded storage
					If outIdx >= l.Count Then
						Do While l.Count <= outIdx
							l.Add(Nothing)
						Loop
					End If

					l(outIdx) = get

					'Add dependency for values array until end of execution
					arrayUseTracker.addDependency(get, New ExecDoneDep())
				Next i

				'Return dummy array
				Dim scalar As INDArray = mmgr.allocate(False, DataType.FLOAT).assign(0.0)
				Return New INDArray(){scalar}
			ElseIf TypeOf op Is TensorArraySplit Then
				'Split values from a rank (N+1)d tensor into sequential indices of the TensorArray
				'For example, orig=[8,2] sizearray with split (4,4) means TensorArray[0] = orig[0:4,:] and TensorArray[1] = orig[4:8,:]
				'Input 0: the TensorArray
				'Input 1: The values to split
				'Input 2: the size of each split (1d integer vector)

				Dim inTensorArray As SDVariable = op.arg(0) 'Dummy variable representing the tensor array
				Dim tArr As VarId = (If(opInputs Is Nothing, Nothing, lookup(inTensorArray.name(), opInputs, False)))
				If tArr Is Nothing AndAlso allIterInputs IsNot Nothing Then
					tArr = lookup(inTensorArray.name(), allIterInputs, False)
				End If
				Dim l As IList(Of INDArray) = tensorArrays(tArr)
				Preconditions.checkState(l IsNot Nothing, "Could not find TensorArray: %s", tArr)

				Dim splitName As String = op.arg(1).name()
				Dim splitArr As INDArray = getArray(sameDiff.getVariable(splitName), opInputs, allIterInputs)


				Dim sizeName As String = op.arg(2).name()
				Dim sizeSDV As SDVariable = sameDiff.getVariable(sizeName)
				Dim sizeArr As INDArray = getArray(sizeSDV, opInputs, allIterInputs)
				Preconditions.checkState(sizeArr.Vector, "Indices variable for TensorArraySplit should be a vector, got %ndShape for %s", sizeArr, sizeName)
				Preconditions.checkState(sizeArr.dataType().isIntType(), "Indices variable for TensorArraySplit should be an integer type, got %s for array %s", sizeArr.dataType(), sizeName)
				Dim sizes() As Integer = sizeArr.toIntVector()

				Do While l.Count <= sizes.Length 'Can't use set(int, E) if index >= size
					l.Add(Nothing)
				Loop

				Dim idx() As INDArrayIndex = ArrayUtil.nTimes(splitArr.rank(), NDArrayIndex.all(), GetType(INDArrayIndex))
				Dim soFar As Integer = 0
				For i As Integer = 0 To sizes.Length - 1
					idx(0) = NDArrayIndex.interval(soFar, soFar + sizes(i))
					Dim [sub] As INDArray = mmgr.dup(splitArr.get(idx))
					l(i) = [sub]
					soFar += sizes(i)

					'Add dependency for values array until end of execution
					arrayUseTracker.addDependency([sub], New ExecDoneDep())
				Next i

				'Return dummy array
				Dim scalar As INDArray = mmgr.allocate(False, DataType.FLOAT).assign(0.0)
				Return New INDArray(){scalar}
			Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New System.InvalidOperationException("Execution support not yet implemented for: " & op.GetType().FullName)
			End If
		End Function


		Public Overrides Function getConstantOrVariable(ByVal variableName As String) As INDArray
			Dim v As SDVariable = sameDiff.getVariable(variableName)
			Preconditions.checkState(sameDiff.getVariable(variableName).Constant OrElse v.getVariableType() = VariableType.VARIABLE, "Variable %s is not a constant", variableName)
			Return sameDiff.getArrForVarName(variableName)
		End Function

		Public Overrides Function getAndParameterizeOp(ByVal opName As String, ByVal frameIter As FrameIter, ByVal opInputs As ISet(Of VarId), ByVal allIterInputs As ISet(Of VarId), ByVal constAndPhInputs As ISet(Of String), ByVal placeholderValues As IDictionary(Of String, INDArray), ByVal allReqVariables As ISet(Of String)) As Pair(Of SameDiffOp, OpContext)
			Dim sdo As SameDiffOp = sameDiff.getOps().get(opName)
			Dim df As DifferentialFunction = sdo.Op

			'TODO Switch to OpContext - and make sure executing like that is thread safe (i.e., array fields in ops are not used etc)

			Preconditions.checkNotNull(df, "No differential function found with name ""%s""", opName)

			If TypeOf df Is LoopCond OrElse TypeOf df Is Enter OrElse TypeOf df Is [Exit] OrElse TypeOf df Is NextIteration OrElse TypeOf df Is Merge OrElse TypeOf df Is Switch OrElse TypeOf df Is BaseTensorOp Then
				'Control dependencies and tensor ops (like TensorArray, TensorArrayRead etc) don't need inputs set, execution is a special case
				Return New Pair(Of SameDiffOp, OpContext)(sdo, Nothing)
			End If

			'Infer the args based on the inputs (variable + frame + iteration)
			Dim argNames() As String = df.argNames()
			Dim numArgs As Integer = (If(argNames Is Nothing, 0, argNames.Length))
			Dim numNonConstIns As Integer = (If(opInputs Is Nothing, 0, opInputs.Count))
			Dim numNonConstInsAllIters As Integer = (If(allIterInputs Is Nothing, 0, allIterInputs.Count))
			Dim numConstPhIns As Integer = (If(constAndPhInputs Is Nothing, 0, constAndPhInputs.Count))

			If numArgs <> (numNonConstIns + numConstPhIns + numNonConstInsAllIters) Then
				If numArgs > 1 Then
					'Might be due to repeated inputs
					Dim uniqueArgNames As ISet(Of String) = New HashSet(Of String)()
					Collections.addAll(uniqueArgNames, argNames)
	'                Preconditions.checkState(uniqueArgNames.size() == (numNonConstIns + numConstPhIns + numNonConstInsAllIters),
	'                        "Different number of arg names as op inputs for op %s (%s): arg names %s vs. op inputs %s+%s", df.getClass().getSimpleName(),
	'                        opName, uniqueArgNames, opInputs, constAndPhInputs);
				Else
					Preconditions.checkState(numArgs = (numNonConstIns + numConstPhIns), "Different number of arg names as op inputs for op %s (%s): arg names %s vs. op inputs %s+%s", df.GetType().Name, opName, argNames, opInputs, constAndPhInputs)
				End If
			End If

			Dim args() As INDArray = Nothing
			If argNames IsNot Nothing AndAlso argNames.Length > 0 Then
				args = New INDArray(argNames.Length - 1){}
				Dim i As Integer = 0
				For Each s As String In argNames
					Dim v As SDVariable = sameDiff.getVariable(s)
					If v.Constant Then
						args(i) = v.Arr
					ElseIf v.getVariableType() = VariableType.VARIABLE Then
						args(i) = v.Arr
					ElseIf v.PlaceHolder Then
						Preconditions.checkState(placeholderValues IsNot Nothing AndAlso placeholderValues.ContainsKey(s), "No array was provided for required placeholder variable ""%s""", s)
						args(i) = placeholderValues(s)
					Else
						Dim vid As VarId = lookup(s, opInputs, allIterInputs, True)
						args(i) = nodeOutputs(vid)
					End If
					Preconditions.checkNotNull(args(i), "Could not parameterize op %s: array %s (variable %s) is null", opName, i, v.name())
					i += 1
				Next s
			End If

			'Set the op inputs and output arguments
			'Note that when we are in a loop (and non-first iteration), we want to allocate new arrays even if shapes are
			' ok: this is because we need the values in past iterations for backprop (potentially)
			'TODO let's find a way to use in-place modification for loops where possible to reduce memory requirements
			Dim isLoop As Boolean = Not frameIter.getFrame().Equals(OUTER_FRAME) AndAlso frameIter.getIteration() > 0

			Dim oc As OpContext = opContexts(opName)
			If oc Is Nothing Then
				oc = Nd4j.Executioner.buildContext()
				opContexts(opName) = oc
			End If

			If TypeOf df Is CustomOp Then
				Dim customOp As DynamicCustomOp = DirectCast(df, DynamicCustomOp)
				If args IsNot Nothing Then
					oc.setInputArrays(args)
				End If

				If TypeOf df Is Identity Then
					'We don't need to allocate an output array for Identity, we pass through the input array without copying
					Return New Pair(Of SameDiffOp, OpContext)(sdo, oc)
				End If

				If customOp.numIArguments() > 0 Then
					oc.IArguments = customOp.iArgs()
				End If
				If customOp.numDArguments() > 0 Then
					oc.DArguments = customOp.dArgs()
				End If
				If customOp.numTArguments() > 0 Then
					oc.TArguments = customOp.tArgs()
				End If
				If customOp.numBArguments() > 0 Then
					oc.BArguments = customOp.bArgs()
				End If


				Dim outShape As IList(Of LongShapeDescriptor) = customOp.calculateOutputShape(oc)
				Preconditions.checkState(outShape IsNot Nothing AndAlso outShape.Count > 0, "Failed to calculate output shapes for op %s (%s) - no shapes were returned by calculateOutputShape()", customOp.opName(), customOp.getOwnName())
				Dim outNames() As String = df.outputVariablesNames()
				Preconditions.checkState(outNames.Length = outShape.Count, "Error in operation shape calculation for op ""%s"": Got %s op output shapes for an operation" & " with %s outputs (number of shapes and outputs must be equal)", df.opName(), outShape.Count, outNames.Length)
				For i As Integer = 0 To outShape.Count - 1
					Dim reqShape As LongShapeDescriptor = outShape(i)

					'Issue: many ops have multiple valid output datatypes, and output shape calc can't at present know which: https://github.com/eclipse/deeplearning4j/issues/6872
					'As a workaround, we'll use the output variable datatype instead.
					Dim dt As DataType = sameDiff.getVariable(outNames(i)).dataType()
					Dim currDT As DataType = reqShape.dataType()
					If dt <> currDT Then
						reqShape = reqShape.asDataType(dt)
					End If

					'Always allocate new output array, rely on memory manager for efficient memory management and array reuse etc
					Dim isOutput As Boolean = allReqVariables.Contains(outNames(i))
					Dim [out] As INDArray = mmgr.allocate(isOutput, reqShape)
					If reqShape.Empty AndAlso Not [out].Empty Then
						Throw New System.InvalidOperationException("Output shape was empty, but created array was not.")
					End If
					oc.setOutputArray(i, [out])
				Next i

			ElseIf TypeOf df Is Op Then
				Dim op As Op = DirectCast(df, Op)

				Dim axisArg As Boolean = False
				Dim emptyReduce As Boolean = False
				If TypeOf op Is ReduceOp AndAlso DirectCast(op, ReduceOp).OpType <> Op.Type.REDUCE3 AndAlso df.argNames().Length = 2 Then
					'2nd input should be treated as integer axis arg...
					Dim axisArgVar As SDVariable = df.arg(1)
					Preconditions.checkState(axisArgVar.dataType().isIntType(), "Legacy op %s input 1 (axis) was expected to be an integer type, is %s", df.GetType(), axisArgVar.dataType())

					Dim arr As INDArray = getArray(axisArgVar, opInputs, allIterInputs)
					Preconditions.checkState(arr IsNot Nothing, "Could not get axis argument for op %s: %s", df.getOwnName(), df.GetType())
					If Not arr.Empty Then
						Dim axis() As Integer = arr.toIntVector()
						Dim rank As Integer = args(0).rank()
						axis = Shape.normalizeAxis(rank, axis)
						df.setDimensions(axis)
						DirectCast(op, BaseReduceOp).setEmptyReduce(False)
					Else
						df.setDimensions(Nothing)
						emptyReduce = True
						'Note: edge case: [x,y].sum(empty) = [x,y] for TF import compatibility.
						'Note also that empty is not the same as int[0] as in INDArray.sum(new int[0])
						DirectCast(op, BaseReduceOp).setEmptyReduce(True)
					End If
					axisArg = True
				ElseIf TypeOf op Is ScalarOp AndAlso df.argNames().Length = 2 Then
					'Scalar ops: 2nd input should be treated as scalar...
					Dim scalarVar As SDVariable = df.arg(1)
					Dim scalar As INDArray = getArray(scalarVar, opInputs, allIterInputs)
					Preconditions.checkState(scalar IsNot Nothing, "Could not get scalar argument for op %s: %s", df.getOwnName(), df.GetType())
					Preconditions.checkState(scalar.Scalar, "Scalar argument for op %s (%s) is not a scalar: has shape %ndShape", df.getOwnName(), df.GetType(), scalar)
					DirectCast(op, ScalarOp).setScalar(scalar)
				End If

				If args IsNot Nothing AndAlso args.Length > 0 Then
					oc.setInputArray(0, args(0))
					If args.Length = 2 AndAlso Not axisArg Then
						oc.setInputArray(1, args(1))
					End If
				End If


				'Check output shape; allocate a new Z if required
				'For example, if minibatch size has changed since last op execution
				Dim isOutput As Boolean = allReqVariables.Contains(DirectCast(op, BaseOp).outputVariablesNames()(0))
				If emptyReduce Then
					'Always allocate new output array, rely on memory manager for efficient memory management and array reuse etc
					Dim z As INDArray = mmgr.allocate(False, oc.getInputArray(0).dataType(), oc.getInputArray(0).shape())
					oc.setOutputArray(0, z)
				Else
					Dim outputShape As IList(Of LongShapeDescriptor) = DirectCast(op, BaseOp).calculateOutputShape(oc)
					Preconditions.checkState(outputShape IsNot Nothing AndAlso outputShape.Count = 1, "Could not calculate output shape for op: %s", op.GetType())
					Dim lsd As LongShapeDescriptor = outputShape(0)
					Dim z As INDArray = mmgr.allocate(isOutput, lsd)
					oc.setOutputArray(0, z)
				End If
			End If

			Return New Pair(Of SameDiffOp, OpContext)(sdo, oc)
		End Function


		Protected Friend Overridable Function getArray(ByVal sdv As SDVariable, ByVal opInputs As ICollection(Of VarId), ByVal allIterInputs As ICollection(Of VarId)) As INDArray
			Dim n As String = sdv.name()
			If sdv.getVariableType() = VariableType.CONSTANT OrElse sdv.getVariableType() = VariableType.VARIABLE Then
				Return getConstantOrVariable(n)
			Else
				Dim inVarId As VarId = lookup(n, opInputs, allIterInputs, False)
				Preconditions.checkState(inVarId IsNot Nothing, "Could not find array for variable %s", sdv.name())
				Return nodeOutputs(inVarId)
			End If
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public abstract static class Dep
		Public MustInherit Class Dep
			Protected Friend frame As String
			Protected Friend parentFrame As FrameIter
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data @EqualsAndHashCode(callSuper = true) public static class OpDep extends Dep
		Public Class OpDep
			Inherits Dep

			Protected Friend opName As String
			Protected Friend iter As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected OpDep(@NonNull String opName, @NonNull String frame, int iter, FrameIter parentFrame)
			Protected Friend Sub New(ByVal opName As String, ByVal frame As String, ByVal iter As Integer, ByVal parentFrame As FrameIter)
				Me.opName = opName
				Me.frame = frame
				Me.iter = iter
				Me.parentFrame = parentFrame
			End Sub

			Public Overrides Function ToString() As String
				Return "OpDep(" & opName & ",frame=" & frame & ",iter=" & iter + (If(parentFrame Is Nothing, "", ",parent=" & parentFrame)) & ")"
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) @AllArgsConstructor protected static class PlaceholderDep extends Dep
		Protected Friend Class PlaceholderDep
			Inherits Dep

			Protected Friend phName As String
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) @AllArgsConstructor protected static class VariableDep extends Dep
		Protected Friend Class VariableDep
			Inherits Dep

			Protected Friend varName As String
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) @AllArgsConstructor protected static class ConstantDep extends Dep
		Protected Friend Class ConstantDep
			Inherits Dep

			Protected Friend constName As String
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) @AllArgsConstructor protected static class ReqOutputDep extends Dep
		Protected Friend Class ReqOutputDep
			Inherits Dep

			Protected Friend outputName As String
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) @NoArgsConstructor protected static class ExecDoneDep extends Dep
		Protected Friend Class ExecDoneDep
			Inherits Dep

		End Class
	End Class

End Namespace