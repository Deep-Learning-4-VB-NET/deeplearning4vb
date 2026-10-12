Imports System.Collections.Generic
Imports org.deeplearning4j.gym
Imports org.deeplearning4j.rl4j.mdp
Imports EncodableToINDArrayTransform = org.deeplearning4j.rl4j.observation.transform.EncodableToINDArrayTransform
Imports TransformProcess = org.deeplearning4j.rl4j.observation.transform.TransformProcess
Imports UniformSkippingFilter = org.deeplearning4j.rl4j.observation.transform.filter.UniformSkippingFilter
Imports HistoryMergeTransform = org.deeplearning4j.rl4j.observation.transform.operation.HistoryMergeTransform
Imports SimpleNormalizationTransform = org.deeplearning4j.rl4j.observation.transform.operation.SimpleNormalizationTransform
Imports CircularFifoStore = org.deeplearning4j.rl4j.observation.transform.operation.historymerge.CircularFifoStore
Imports DiscreteSpace = org.deeplearning4j.rl4j.space.DiscreteSpace
Imports org.deeplearning4j.rl4j.space
Imports Random = org.nd4j.linalg.api.rng.Random

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

Namespace org.deeplearning4j.rl4j.support


	Public Class MockMDP
		Implements MDP(Of MockObservation, Integer, DiscreteSpace)

'JAVA TO VB CONVERTER NOTE: The field actionSpace was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly actionSpace_Conflict As DiscreteSpace
		Private ReadOnly stepsUntilDone As Integer
		Private currentObsValue As Integer = 0
'JAVA TO VB CONVERTER NOTE: The field observationSpace was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly observationSpace_Conflict As ObservationSpace

		Public ReadOnly actions As IList(Of Integer) = New List(Of Integer)()
'JAVA TO VB CONVERTER NOTE: The field step was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private step_Conflict As Integer = 0
		Public resetCount As Integer = 0

		Public Sub New(ByVal observationSpace As ObservationSpace, ByVal stepsUntilDone As Integer, ByVal actionSpace As DiscreteSpace)
			Me.stepsUntilDone = stepsUntilDone
			Me.actionSpace_Conflict = actionSpace
			Me.observationSpace_Conflict = observationSpace
		End Sub

		Public Sub New(ByVal observationSpace As ObservationSpace, ByVal stepsUntilDone As Integer, ByVal rnd As Random)
			Me.New(observationSpace, stepsUntilDone, New DiscreteSpace(5, rnd))
		End Sub

		Public Sub New(ByVal observationSpace As ObservationSpace)
			Me.New(observationSpace, Integer.MaxValue, New DiscreteSpace(5))
		End Sub

		Public Sub New(ByVal observationSpace As ObservationSpace, ByVal rnd As Random)
			Me.New(observationSpace, Integer.MaxValue, New DiscreteSpace(5, rnd))
		End Sub

		Public Overridable ReadOnly Property ObservationSpace As ObservationSpace
			Get
				Return observationSpace_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property ActionSpace As DiscreteSpace
			Get
				Return actionSpace_Conflict
			End Get
		End Property

		Public Overridable Function reset() As MockObservation
			resetCount += 1
			currentObsValue = 0
			step_Conflict = 0
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: return new MockObservation(currentObsValue++);
			Dim tempVar = New MockObservation(currentObsValue)
				currentObsValue += 1
				Return tempVar
		End Function

		Public Overridable Sub close() Implements MDP(Of MockObservation, Integer, DiscreteSpace).close

		End Sub

		Public Overridable Function [step](ByVal action As Integer?) As StepReply(Of MockObservation)
			actions.Add(action)
			step_Conflict += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: return new org.deeplearning4j.gym.StepReply<>(new MockObservation(currentObsValue), (double) currentObsValue++, isDone(), null);
			Dim tempVar = New StepReply(Of MockObservation)(New MockObservation(currentObsValue), CDbl(currentObsValue), Done, Nothing)
				currentObsValue += 1
				Return tempVar
		End Function

		Public Overridable ReadOnly Property Done As Boolean Implements MDP(Of MockObservation, Integer, DiscreteSpace).isDone
			Get
				Return step_Conflict >= stepsUntilDone
			End Get
		End Property

		Public Overridable Function newInstance() As MDP
			Return Nothing
		End Function

		Public Shared Function buildTransformProcess(ByVal skipFrame As Integer, ByVal historyLength As Integer) As TransformProcess
			Return TransformProcess.builder().filter(New UniformSkippingFilter(skipFrame)).transform("data", New EncodableToINDArrayTransform()).transform("data", New SimpleNormalizationTransform(0.0, 255.0)).transform("data", HistoryMergeTransform.builder().elementStore(New CircularFifoStore(historyLength)).build(4)).build("data")
		End Function

	End Class

End Namespace