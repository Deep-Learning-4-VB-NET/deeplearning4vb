Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports SuperBuilder = lombok.experimental.SuperBuilder
Imports Observation = org.deeplearning4j.rl4j.observation.Observation

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


	Public Class StateActionExperienceHandler(Of A)
		Implements ExperienceHandler(Of A, StateActionReward(Of A))

		Private Const DEFAULT_BATCH_SIZE As Integer = 8

		Private ReadOnly batchSize As Integer

		Private isFinalObservationSet As Boolean

		Public Sub New(ByVal configuration As Configuration)
			Me.batchSize = configuration.getBatchSize()
		End Sub

		Private stateActionRewards As IList(Of StateActionReward(Of A)) = New List(Of StateActionReward(Of A))()

		Public Overridable WriteOnly Property FinalObservation(ByVal observation As Observation) Implements ExperienceHandler.setFinalObservation As Observation
			Set(ByVal observation As Observation)
				isFinalObservationSet = True
			End Set
		End Property

		Public Overridable Sub addExperience(ByVal observation As Observation, ByVal action As A, ByVal reward As Double, ByVal isTerminal As Boolean) Implements ExperienceHandler(Of A, StateActionReward(Of A)).addExperience
			stateActionRewards.Add(New StateActionReward(Of A)(observation, action, reward, isTerminal))
		End Sub

		Public Overridable ReadOnly Property TrainingBatchSize As Integer Implements ExperienceHandler(Of A, StateActionReward(Of A)).getTrainingBatchSize
			Get
				Return stateActionRewards.Count
			End Get
		End Property

		Public Overridable ReadOnly Property TrainingBatchReady As Boolean Implements ExperienceHandler(Of A, StateActionReward(Of A)).isTrainingBatchReady
			Get
				Return stateActionRewards.Count >= batchSize OrElse (isFinalObservationSet AndAlso stateActionRewards.Count > 0)
			End Get
		End Property

		''' <summary>
		''' The elements are returned in the historical order (i.e. in the order they happened)
		''' Note: the experience store is cleared after calling this method.
		''' </summary>
		''' <returns> The list of experience elements </returns>
		Public Overridable Function generateTrainingBatch() As IList(Of StateActionReward(Of A)) Implements ExperienceHandler(Of A, StateActionReward(Of A)).generateTrainingBatch
			Dim result As IList(Of StateActionReward(Of A)) = stateActionRewards
			stateActionRewards = New List(Of StateActionReward(Of A))()

			Return result
		End Function

		Public Overridable Sub reset() Implements ExperienceHandler(Of A, StateActionReward(Of A)).reset
			stateActionRewards = New List(Of StateActionReward(Of A))()
			isFinalObservationSet = False
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuperBuilder @Data public static class Configuration
		Public Class Configuration
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int batchSize = DEFAULT_BATCH_SIZE;
			Friend batchSize As Integer = DEFAULT_BATCH_SIZE
		End Class
	End Class

End Namespace