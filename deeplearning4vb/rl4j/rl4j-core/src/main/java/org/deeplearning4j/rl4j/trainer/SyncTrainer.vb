Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Builder = org.apache.commons.lang3.builder.Builder
Imports org.deeplearning4j.rl4j.agent

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

	Public Class SyncTrainer(Of ACTION)
		Implements ITrainer

		Private ReadOnly stoppingCondition As System.Predicate(Of SyncTrainer(Of ACTION))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int episodeCount;
		Private episodeCount As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int stepCount;
		Private stepCount As Integer


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter final org.deeplearning4j.rl4j.agent.IAgentLearner<ACTION> agentLearner;
		Friend ReadOnly agentLearner As IAgentLearner(Of ACTION)

		''' <summary>
		''' Build a SyncTrainer that will train until a stopping condition is met. </summary>
		''' <param name="agentLearnerBuilder"> the builder that will be used to create the agent-learner instance.
		'''                            Can be a descendant of <seealso cref="org.deeplearning4j.rl4j.builder.BaseAgentLearnerBuilder BaseAgentLearnerBuilder"/>
		'''                            for common scenario, or any class or lambda that implements <tt>Builder&lt;IAgentLearner&lt;ACTION&gt;&gt;</tt> if any specific
		'''                            need is not met by BaseAgentLearnerBuilder. </param>
		''' <param name="stoppingCondition"> the training will stop when this condition evaluates to true </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @lombok.Builder public SyncTrainer(@NonNull Builder<org.deeplearning4j.rl4j.agent.IAgentLearner<ACTION>> agentLearnerBuilder, @NonNull Predicate<SyncTrainer<ACTION>> stoppingCondition)
		Public Sub New(ByVal agentLearnerBuilder As Builder(Of IAgentLearner(Of ACTION)), ByVal stoppingCondition As Predicate(Of SyncTrainer(Of ACTION)))
			Me.stoppingCondition = stoppingCondition
			agentLearner = agentLearnerBuilder.build()
		End Sub

		Public Overridable Sub train() Implements ITrainer.train
			episodeCount = 0
			stepCount = 0

			Do While Not stoppingCondition.test(Me)
				agentLearner.run()
				episodeCount += 1
				stepCount += agentLearner.EpisodeStepCount
			Loop
		End Sub
	End Class
End Namespace