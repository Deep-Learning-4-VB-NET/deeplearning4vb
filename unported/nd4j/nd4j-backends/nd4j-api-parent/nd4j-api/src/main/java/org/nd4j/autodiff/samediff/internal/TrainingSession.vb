Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports At = org.nd4j.autodiff.listeners.At
Imports Listener = org.nd4j.autodiff.listeners.Listener
Imports Loss = org.nd4j.autodiff.listeners.Loss
Imports Operation = org.nd4j.autodiff.listeners.Operation
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports TrainingConfig = org.nd4j.autodiff.samediff.TrainingConfig
Imports VariableType = org.nd4j.autodiff.samediff.VariableType
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports org.nd4j.linalg.learning
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
Imports AtomicDouble = org.nd4j.common.primitives.AtomicDouble
Imports org.nd4j.common.primitives

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
'ORIGINAL LINE: @Slf4j public class TrainingSession extends InferenceSession
	Public Class TrainingSession
		Inherits InferenceSession

		Protected Friend config As TrainingConfig
		Protected Friend gradVarToVarMap As IDictionary(Of String, String)
		Protected Friend updaters As IDictionary(Of String, GradientUpdater)
		Protected Friend lossVarsToLossIdx As IDictionary(Of String, Integer)
		Protected Friend currIterLoss() As Double
		Protected Friend currIterRegLoss As IDictionary(Of Type, AtomicDouble)
		Protected Friend listeners As IList(Of Listener)


		Public Sub New(ByVal sameDiff As SameDiff)
			MyBase.New(sameDiff)
		End Sub

		''' <summary>
		''' Perform one iteration of training - i.e., do forward and backward passes, and update the parameters
		''' </summary>
		''' <param name="config">        Training configuration </param>
		''' <param name="placeholders">  Current placeholders </param>
		''' <param name="paramsToTrain"> Set of parameters that will be trained </param>
		''' <param name="updaters">      Current updater state </param>
		''' <param name="batch">         Current data/batch (mainly for listeners, should have already been converted to placeholders map) </param>
		''' <param name="lossVariables"> Loss variables (names) </param>
		''' <param name="listeners">     Listeners (if any) </param>
		''' <param name="at">            Current epoch, iteration, etc </param>
		''' <returns> The Loss at the current iteration </returns>
		Public Overridable Function trainingIteration(ByVal config As TrainingConfig, ByVal placeholders As IDictionary(Of String, INDArray), ByVal paramsToTrain As ISet(Of String), ByVal updaters As IDictionary(Of String, GradientUpdater), ByVal batch As MultiDataSet, ByVal lossVariables As IList(Of String), ByVal listeners As IList(Of Listener), ByVal at As At) As Loss
			Me.config = config
			Me.updaters = updaters

			'Preprocess listeners, get the relevant ones
			If listeners Is Nothing Then
				Me.listeners = Nothing
			Else
				Dim filtered As IList(Of Listener) = New List(Of Listener)()
				For Each l As Listener In listeners
					If l.isActive(at.operation()) Then
						filtered.Add(l)
					End If
				Next l
				Me.listeners = If(filtered.Count = 0, Nothing, filtered)
			End If

			Dim requiredActivations As ISet(Of String) = New HashSet(Of String)()
			gradVarToVarMap = New Dictionary(Of String, String)() 'Key: gradient variable. Value: variable that the key is gradient for
			For Each s As String In paramsToTrain
				Preconditions.checkState(sameDiff.hasVariable(s), "SameDiff instance does not have a variable with name ""%s""", s)
				Dim v As SDVariable = sameDiff.getVariable(s)
				Preconditions.checkState(v.getVariableType() = VariableType.VARIABLE, "Can only train VARIABLE type variable - ""%s"" has type %s", s, v.getVariableType())
				Dim grad As SDVariable = sameDiff.getVariable(s).Gradient
				If grad Is Nothing Then
					'In some cases, a variable won't actually impact the loss value, and hence won't have a gradient associated with it
					'For example: floatVar -> cast to integer -> cast to float -> sum -> loss
					'In this case, the gradient of floatVar isn't defined (due to no floating point connection to the loss)
					Continue For
				End If

				requiredActivations.Add(grad.name())

				gradVarToVarMap(grad.name()) = s
			Next s

			'Also add evaluations - in case we want to evaluate something that isn't required to determine loss
			' (hence wouldn't normally be calculated)
			If config.getTrainEvaluations() IsNot Nothing Then
				requiredActivations.addAll(config.getTrainEvaluations().keySet())
			End If

			'Set up losses
			lossVarsToLossIdx = New LinkedHashMap(Of String, Integer)()
			Dim lossVars As IList(Of String)
			currIterLoss = New Double(lossVariables.Count - 1){}
			currIterRegLoss = New Dictionary(Of Type, AtomicDouble)()
			For i As Integer = 0 To lossVariables.Count - 1
				lossVarsToLossIdx(lossVariables(i)) = i
			Next i

			'Do training iteration
			Dim outputVars As IList(Of String) = New List(Of String)(gradVarToVarMap.Keys) 'TODO this should be empty, and grads calculated in requiredActivations
			Dim m As IDictionary(Of String, INDArray) = output(outputVars, placeholders, batch, requiredActivations, listeners, at)


			Dim finalLoss((currIterLoss.Length + currIterRegLoss.Count) - 1) As Double
			Array.Copy(currIterLoss, 0, finalLoss, 0, currIterLoss.Length)
			If currIterRegLoss.Count > 0 Then
				lossVars = New List(Of String)(lossVariables.Count + currIterRegLoss.Count)
				CType(lossVars, List(Of String)).AddRange(lossVariables)
				Dim s As Integer = currIterRegLoss.Count
				'Collect regularization losses
				For Each entry As KeyValuePair(Of Type, AtomicDouble) In currIterRegLoss.SetOfKeyValuePairs()
					lossVars.Add(entry.Key.getSimpleName())
					finalLoss(s) = entry.Value.get()
				Next entry
			Else
				lossVars = lossVariables
			End If

			Dim loss As New Loss(lossVars, finalLoss)
			If listeners IsNot Nothing Then
				For Each l As Listener In listeners
					If l.isActive(Operation.TRAINING) Then
						l.iterationDone(sameDiff, at, batch, loss)
					End If
				Next l
			End If

			Return loss
		End Function

		Public Overrides Function getOutputs(ByVal opPair As Pair(Of SameDiffOp, OpContext), ByVal outputFrameIter As FrameIter, ByVal opInputs As ISet(Of VarId), ByVal allIterInputs As ISet(Of VarId), ByVal constAndPhInputs As ISet(Of String), ByVal listeners As IList(Of Listener), ByVal at As At, ByVal batch As MultiDataSet, ByVal allReqVariables As ISet(Of String)) As INDArray()
			'Get outputs from InferenceSession
			Dim [out]() As INDArray = MyBase.getOutputs(opPair, outputFrameIter, opInputs, allIterInputs, constAndPhInputs, listeners, at, batch, allReqVariables)
			Dim op As SameDiffOp = opPair.First

			Dim outputs As IList(Of String) = op.getOutputsOfOp()
			Dim outIdx As Integer = 0
			For Each s As String In outputs
				'If this is a loss variable - record it
				If lossVarsToLossIdx.ContainsKey(s) Then
					Dim lossIdx As Integer = lossVarsToLossIdx(s)
					Dim arr As INDArray = [out](outIdx)
					Dim l As Double = If(arr.Scalar, arr.getDouble(0), arr.sumNumber().doubleValue())
					currIterLoss(lossIdx) += l
				End If

				'If this is a gradient variable - apply the updater and update the parameter array in-line
				If gradVarToVarMap.ContainsKey(s) Then
					Dim varName As String = gradVarToVarMap(s)
					'log.info("Calculated gradient for variable \"{}\": (grad var name: \"{}\")", varName, s);

					Dim gradVar As Variable = sameDiff.getVariables().get(s)
					If gradVar.getInputsForOp() IsNot Nothing AndAlso gradVar.getInputsForOp().isEmpty() Then
						'Should be rare, and we should handle this by tracking dependencies, and only update when safe
						' (i.e., dependency tracking)
						Throw New System.InvalidOperationException("Op depends on gradient variable: " & s & " for variable " & varName)
					End If

					Dim u As GradientUpdater = updaters(varName)
					Preconditions.checkState(u IsNot Nothing, "No updater found for variable ""%s""", varName)

					Dim var As Variable = sameDiff.getVariables().get(varName)
					Dim gradArr As INDArray = [out](outIdx)
					Dim paramArr As INDArray = var.getVariable().getArr()

					'Pre-updater regularization (L1, L2)
					Dim r As IList(Of Regularization) = config.getRegularization()
					If r IsNot Nothing AndAlso r.Count > 0 Then
						Dim lr As Double = If(config.getUpdater().hasLearningRate(), config.getUpdater().getLearningRate(at.iteration(), at.epoch()), 1.0)
						For Each reg As Regularization In r
							If reg.applyStep() = Regularization.ApplyStep.BEFORE_UPDATER Then
								If Me.listeners IsNot Nothing Then
									Dim score As Double = reg.score(paramArr, at.iteration(), at.epoch())
									If Not currIterRegLoss.ContainsKey(reg.GetType()) Then
										currIterRegLoss(reg.GetType()) = New AtomicDouble()
									End If
									currIterRegLoss(reg.GetType()).addAndGet(score)
								End If
								reg.apply(paramArr, gradArr, lr, at.iteration(), at.epoch())
							End If
						Next reg
					End If

					u.applyUpdater(gradArr, at.iteration(), at.epoch())

					'Post-apply regularization (weight decay)
					If r IsNot Nothing AndAlso r.Count > 0 Then
						Dim lr As Double = If(config.getUpdater().hasLearningRate(), config.getUpdater().getLearningRate(at.iteration(), at.epoch()), 1.0)
						For Each reg As Regularization In r
							If reg.applyStep() = Regularization.ApplyStep.POST_UPDATER Then
								If Me.listeners IsNot Nothing Then
									Dim score As Double = reg.score(paramArr, at.iteration(), at.epoch())
									If Not currIterRegLoss.ContainsKey(reg.GetType()) Then
										currIterRegLoss(reg.GetType()) = New AtomicDouble()
									End If
									currIterRegLoss(reg.GetType()).addAndGet(score)
								End If
								reg.apply(paramArr, gradArr, lr, at.iteration(), at.epoch())
							End If
						Next reg
					End If

					If listeners IsNot Nothing Then
						For Each l As Listener In listeners
							If l.isActive(at.operation()) Then
								l.preUpdate(sameDiff, at, var, gradArr)
							End If
						Next l
					End If

					'Update:
					If config.isMinimize() Then
						paramArr.subi(gradArr)
					Else
						paramArr.addi(gradArr)
					End If
					log.trace("Applied updater to gradient and updated variable: {}", varName)
				End If

				outIdx += 1
			Next s

			Return [out]
		End Function
	End Class

End Namespace