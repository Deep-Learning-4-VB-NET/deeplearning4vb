Imports System.Threading
Imports NonNull = lombok.NonNull
Imports Builder = org.apache.commons.lang3.builder.Builder
Imports org.deeplearning4j.rl4j.agent
Imports Preconditions = org.nd4j.common.base.Preconditions
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
Namespace org.deeplearning4j.rl4j.trainer


	' TODO: Add listeners & events

	Public Class AsyncTrainer(Of ACTION)
		Implements ITrainer

		Private ReadOnly agentLearnerBuilder As Builder(Of IAgentLearner(Of ACTION))
		Private ReadOnly stoppingCondition As System.Predicate(Of AsyncTrainer(Of ACTION))

		Private ReadOnly numThreads As Integer
'JAVA TO VB CONVERTER NOTE: The field episodeCount was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly episodeCount_Conflict As New AtomicInteger()
'JAVA TO VB CONVERTER NOTE: The field stepCount was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly stepCount_Conflict As New AtomicInteger()

		Private shouldStop As Boolean = False

		''' <summary>
		''' Build a AsyncTrainer that will train until a stopping condition is met. </summary>
		''' <param name="agentLearnerBuilder"> the builder that will be used to create the agent-learner instances.
		'''                            Can be a descendant of <seealso cref="org.deeplearning4j.rl4j.builder.BaseAgentLearnerBuilder BaseAgentLearnerBuilder"/>
		'''                            for common scenario, or any class or lambda that implements <tt>Builder&lt;IAgentLearner&lt;ACTION&gt;&gt;</tt> if any specific
		'''                            need is not met by BaseAgentLearnerBuilder. </param>
		''' <param name="stoppingCondition"> the training will stop when this condition evaluates to true </param>
		''' <param name="numThreads"> the number of threads to run in parallel </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @lombok.Builder public AsyncTrainer(@NonNull Builder<org.deeplearning4j.rl4j.agent.IAgentLearner<ACTION>> agentLearnerBuilder, @NonNull Predicate<AsyncTrainer<ACTION>> stoppingCondition, int numThreads)
		Public Sub New(ByVal agentLearnerBuilder As Builder(Of IAgentLearner(Of ACTION)), ByVal stoppingCondition As Predicate(Of AsyncTrainer(Of ACTION)), ByVal numThreads As Integer)
			Preconditions.checkArgument(numThreads > 0, "numThreads must be greater than 0, got: ", numThreads)

			Me.agentLearnerBuilder = agentLearnerBuilder
			Me.stoppingCondition = stoppingCondition
			Me.numThreads = numThreads
		End Sub

		Public Overridable Sub train() Implements ITrainer.train
			reset()
			Dim threads(numThreads - 1) As Thread

			For i As Integer = 0 To numThreads - 1
				Dim thread As New AgentLearnerThread(Me, agentLearnerBuilder.build(), i)
				threads(i) = thread
				thread.Start()
			Next i

			' Wait for all threads to finish
			For Each thread As Thread In threads
				Try
					thread.Join()
				Catch e As InterruptedException
					' Ignore
				End Try
			Next thread
		End Sub

		Private Sub reset()
			episodeCount_Conflict.set(0)
			stepCount_Conflict.set(0)
			shouldStop = False
		End Sub

		Public Overridable ReadOnly Property EpisodeCount As Integer Implements ITrainer.getEpisodeCount
			Get
				Return episodeCount_Conflict.get()
			End Get
		End Property

		Public Overridable ReadOnly Property StepCount As Integer Implements ITrainer.getStepCount
			Get
				Return stepCount_Conflict.get()
			End Get
		End Property

		Private Sub onEpisodeEnded(ByVal numStepsInEpisode As Integer)
			episodeCount_Conflict.incrementAndGet()
			stepCount_Conflict.addAndGet(numStepsInEpisode)
			If stoppingCondition.test(Me) Then
				shouldStop = True
			End If
		End Sub

		Private Class AgentLearnerThread
			Inherits Thread

			Private ReadOnly outerInstance As AsyncTrainer(Of ACTION)

			Friend ReadOnly agentLearner As IAgentLearner(Of ACTION)
			Friend ReadOnly deviceNum As Integer

			Public Sub New(ByVal outerInstance As AsyncTrainer(Of ACTION), ByVal agentLearner As IAgentLearner(Of ACTION), ByVal deviceNum As Integer)
				Me.outerInstance = outerInstance
				Me.agentLearner = agentLearner
				Me.deviceNum = deviceNum
			End Sub

			Public Overrides Sub run()
				Nd4j.AffinityManager.unsafeSetDevice(deviceNum)
				Do While Not outerInstance.shouldStop
					agentLearner.run()
					outerInstance.onEpisodeEnded(agentLearner.EpisodeStepCount)
				Loop
			End Sub

		End Class
	End Class
End Namespace