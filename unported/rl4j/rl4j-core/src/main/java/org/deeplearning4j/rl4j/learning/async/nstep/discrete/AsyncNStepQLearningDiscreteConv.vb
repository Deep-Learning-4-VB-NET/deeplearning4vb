Imports System
Imports HistoryProcessor = org.deeplearning4j.rl4j.learning.HistoryProcessor
Imports org.deeplearning4j.rl4j.learning.async
Imports AsyncQLearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.AsyncQLearningConfiguration
Imports org.deeplearning4j.rl4j.mdp
Imports NetworkConfiguration = org.deeplearning4j.rl4j.network.configuration.NetworkConfiguration
Imports DQNFactory = org.deeplearning4j.rl4j.network.dqn.DQNFactory
Imports DQNFactoryStdConv = org.deeplearning4j.rl4j.network.dqn.DQNFactoryStdConv
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

	Public Class AsyncNStepQLearningDiscreteConv(Of OBSERVATION As org.deeplearning4j.rl4j.space.Encodable)
		Inherits AsyncNStepQLearningDiscrete(Of OBSERVATION)

		Private ReadOnly hpconf As HistoryProcessor.Configuration

		<Obsolete>
		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal dqn As IDQN, ByVal hpconf As HistoryProcessor.Configuration, ByVal conf As AsyncQLearningConfiguration, ByVal dataManager As IDataManager)
			Me.New(mdp, dqn, hpconf, conf)
			addListener(New DataManagerTrainingListener(dataManager))
		End Sub
		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal dqn As IDQN, ByVal hpconf As HistoryProcessor.Configuration, ByVal conf As AsyncQLearningConfiguration)
			MyBase.New(mdp, dqn, conf)
			Me.hpconf = hpconf
			setHistoryProcessor(hpconf)
		End Sub

		<Obsolete>
		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal factory As DQNFactory, ByVal hpconf As HistoryProcessor.Configuration, ByVal conf As AsyncQLearningConfiguration, ByVal dataManager As IDataManager)
			Me.New(mdp, factory.buildDQN(hpconf.getShape(), mdp.ActionSpace.getSize()), hpconf, conf, dataManager)
		End Sub
		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal factory As DQNFactory, ByVal hpconf As HistoryProcessor.Configuration, ByVal conf As AsyncQLearningConfiguration)
			Me.New(mdp, factory.buildDQN(hpconf.getShape(), mdp.ActionSpace.getSize()), hpconf, conf)
		End Sub

		<Obsolete>
		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal netConf As NetworkConfiguration, ByVal hpconf As HistoryProcessor.Configuration, ByVal conf As AsyncQLearningConfiguration, ByVal dataManager As IDataManager)
			Me.New(mdp, New DQNFactoryStdConv(netConf), hpconf, conf, dataManager)
		End Sub
		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal netConf As NetworkConfiguration, ByVal hpconf As HistoryProcessor.Configuration, ByVal conf As AsyncQLearningConfiguration)
			Me.New(mdp, New DQNFactoryStdConv(netConf), hpconf, conf)
		End Sub

		Public Overrides Function newThread(ByVal i As Integer, ByVal deviceNum As Integer) As AsyncThread
			Dim at As AsyncThread = MyBase.newThread(i, deviceNum)
			at.setHistoryProcessor(hpconf)
			Return at
		End Function
	End Class

End Namespace