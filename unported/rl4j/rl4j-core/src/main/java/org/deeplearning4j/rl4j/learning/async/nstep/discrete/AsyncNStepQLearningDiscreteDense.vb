Imports System
Imports AsyncQLearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.AsyncQLearningConfiguration
Imports org.deeplearning4j.rl4j.mdp
Imports DQNDenseNetworkConfiguration = org.deeplearning4j.rl4j.network.configuration.DQNDenseNetworkConfiguration
Imports DQNFactory = org.deeplearning4j.rl4j.network.dqn.DQNFactory
Imports DQNFactoryStdDense = org.deeplearning4j.rl4j.network.dqn.DQNFactoryStdDense
Imports org.deeplearning4j.rl4j.network.dqn
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports DiscreteSpace = org.deeplearning4j.rl4j.space.DiscreteSpace
Imports DataManagerTrainingListener = org.deeplearning4j.rl4j.util.DataManagerTrainingListener
Imports IDataManager = org.deeplearning4j.rl4j.util.IDataManager

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

Namespace org.deeplearning4j.rl4j.learning.async.nstep.discrete

	Public Class AsyncNStepQLearningDiscreteDense(Of OBSERVATION As org.deeplearning4j.rl4j.space.Encodable)
		Inherits AsyncNStepQLearningDiscrete(Of OBSERVATION)

		<Obsolete>
		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal dqn As IDQN, ByVal conf As AsyncNStepQLConfiguration, ByVal dataManager As IDataManager)
			MyBase.New(mdp, dqn, conf.toLearningConfiguration())
			addListener(New DataManagerTrainingListener(dataManager))
		End Sub

		<Obsolete>
		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal dqn As IDQN, ByVal conf As AsyncNStepQLConfiguration)
			MyBase.New(mdp, dqn, conf.toLearningConfiguration())
		End Sub

		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal dqn As IDQN, ByVal conf As AsyncQLearningConfiguration)
			MyBase.New(mdp, dqn, conf)
		End Sub

		<Obsolete>
		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal factory As DQNFactory, ByVal conf As AsyncNStepQLConfiguration, ByVal dataManager As IDataManager)
			Me.New(mdp, factory.buildDQN(mdp.getObservationSpace().Shape, mdp.ActionSpace.getSize()), conf, dataManager)
		End Sub

		<Obsolete>
		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal factory As DQNFactory, ByVal conf As AsyncNStepQLConfiguration)
			Me.New(mdp, factory.buildDQN(mdp.getObservationSpace().Shape, mdp.ActionSpace.getSize()), conf)
		End Sub

		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal factory As DQNFactory, ByVal conf As AsyncQLearningConfiguration)
			Me.New(mdp, factory.buildDQN(mdp.getObservationSpace().Shape, mdp.ActionSpace.getSize()), conf)
		End Sub

		<Obsolete>
		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal netConf As DQNFactoryStdDense.Configuration, ByVal conf As AsyncNStepQLConfiguration, ByVal dataManager As IDataManager)
			Me.New(mdp, New DQNFactoryStdDense(netConf.toNetworkConfiguration()), conf, dataManager)
		End Sub

		<Obsolete>
		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal netConf As DQNFactoryStdDense.Configuration, ByVal conf As AsyncNStepQLConfiguration)
			Me.New(mdp, New DQNFactoryStdDense(netConf.toNetworkConfiguration()), conf)
		End Sub

		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal netConf As DQNDenseNetworkConfiguration, ByVal conf As AsyncQLearningConfiguration)
			Me.New(mdp, New DQNFactoryStdDense(netConf), conf)
		End Sub


	End Class

End Namespace