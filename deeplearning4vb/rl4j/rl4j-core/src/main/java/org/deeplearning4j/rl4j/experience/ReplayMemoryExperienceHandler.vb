Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports SuperBuilder = lombok.experimental.SuperBuilder
Imports org.deeplearning4j.rl4j.learning.sync
Imports org.deeplearning4j.rl4j.learning.sync
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
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
Namespace org.deeplearning4j.rl4j.experience


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode public class ReplayMemoryExperienceHandler<A> implements ExperienceHandler<A, StateActionRewardState<A>>
	Public Class ReplayMemoryExperienceHandler(Of A)
		Implements ExperienceHandler(Of A, StateActionRewardState(Of A))

		Private Const DEFAULT_MAX_REPLAY_MEMORY_SIZE As Integer = 150000
		Private Const DEFAULT_BATCH_SIZE As Integer = 32
		Private ReadOnly batchSize As Integer

		Private expReplay As IExpReplay(Of A)

		Private pendingStateActionRewardState As StateActionRewardState(Of A)

		Public Sub New(ByVal expReplay As IExpReplay(Of A))
			Me.expReplay = expReplay
			Me.batchSize = expReplay.DesignatedBatchSize
		End Sub

		Public Sub New(ByVal configuration As Configuration, ByVal random As Random)
			Me.New(New ExpReplay(Of A)(configuration.maxReplayMemorySize, configuration.batchSize, random))
		End Sub

		Public Overridable Sub addExperience(ByVal observation As Observation, ByVal action As A, ByVal reward As Double, ByVal isTerminal As Boolean) Implements ExperienceHandler(Of A, StateActionRewardState(Of A)).addExperience
			NextObservationOnPending = observation
			pendingStateActionRewardState = New StateActionRewardState(Of A)(observation, action, reward, isTerminal)
		End Sub

		Public Overridable WriteOnly Property FinalObservation(ByVal observation As Observation) Implements ExperienceHandler.setFinalObservation As Observation
			Set(ByVal observation As Observation)
				NextObservationOnPending = observation
				pendingStateActionRewardState = Nothing
			End Set
		End Property

		Public Overridable ReadOnly Property TrainingBatchSize As Integer Implements ExperienceHandler(Of A, StateActionRewardState(Of A)).getTrainingBatchSize
			Get
				Return expReplay.BatchSize
			End Get
		End Property

		Public Overridable ReadOnly Property TrainingBatchReady As Boolean Implements ExperienceHandler(Of A, StateActionRewardState(Of A)).isTrainingBatchReady
			Get
				Return expReplay.BatchSize >= batchSize
			End Get
		End Property

		''' <returns> A batch of experience selected from the replay memory. The replay memory is unchanged after the call. </returns>
		Public Overridable Function generateTrainingBatch() As IList(Of StateActionRewardState(Of A)) Implements ExperienceHandler(Of A, StateActionRewardState(Of A)).generateTrainingBatch
			Return expReplay.getBatch()
		End Function

		Public Overridable Sub reset() Implements ExperienceHandler(Of A, StateActionRewardState(Of A)).reset
			pendingStateActionRewardState = Nothing
		End Sub

		Private WriteOnly Property NextObservationOnPending As Observation
			Set(ByVal observation As Observation)
				If pendingStateActionRewardState IsNot Nothing Then
					pendingStateActionRewardState.setNextObservation(observation)
					expReplay.store(pendingStateActionRewardState)
				End If
			End Set
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuperBuilder @Data public static class Configuration
		Public Class Configuration
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int maxReplayMemorySize = DEFAULT_MAX_REPLAY_MEMORY_SIZE;
			Friend maxReplayMemorySize As Integer = DEFAULT_MAX_REPLAY_MEMORY_SIZE

			''' <summary>
			''' The size of training batches. Default is 32.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int batchSize = DEFAULT_BATCH_SIZE;
			Friend batchSize As Integer = DEFAULT_BATCH_SIZE
		End Class
	End Class

End Namespace