Imports System
Imports HistoryProcessor = org.deeplearning4j.rl4j.learning.HistoryProcessor
Imports org.deeplearning4j.rl4j.learning.async
Imports A3CLearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.A3CLearningConfiguration
Imports org.deeplearning4j.rl4j.mdp
Imports ActorCriticFactoryCompGraph = org.deeplearning4j.rl4j.network.ac.ActorCriticFactoryCompGraph
Imports ActorCriticFactoryCompGraphStdConv = org.deeplearning4j.rl4j.network.ac.ActorCriticFactoryCompGraphStdConv
Imports org.deeplearning4j.rl4j.network.ac
Imports ActorCriticNetworkConfiguration = org.deeplearning4j.rl4j.network.configuration.ActorCriticNetworkConfiguration
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

Namespace org.deeplearning4j.rl4j.learning.async.a3c.discrete

	Public Class A3CDiscreteConv(Of OBSERVATION As org.deeplearning4j.rl4j.space.Encodable)
		Inherits A3CDiscrete(Of OBSERVATION)

		Private ReadOnly hpconf As HistoryProcessor.Configuration

		<Obsolete>
		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal actorCritic As IActorCritic, ByVal hpconf As HistoryProcessor.Configuration, ByVal conf As A3CConfiguration, ByVal dataManager As IDataManager)
			Me.New(mdp, actorCritic, hpconf, conf)
			addListener(New DataManagerTrainingListener(dataManager))
		End Sub

		<Obsolete>
		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal IActorCritic As IActorCritic, ByVal hpconf As HistoryProcessor.Configuration, ByVal conf As A3CConfiguration)

			MyBase.New(mdp, IActorCritic, conf.toLearningConfiguration())
			Me.hpconf = hpconf
			setHistoryProcessor(hpconf)
		End Sub

		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal IActorCritic As IActorCritic, ByVal hpconf As HistoryProcessor.Configuration, ByVal conf As A3CLearningConfiguration)
			MyBase.New(mdp, IActorCritic, conf)
			Me.hpconf = hpconf
			setHistoryProcessor(hpconf)
		End Sub

		<Obsolete>
		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal factory As ActorCriticFactoryCompGraph, ByVal hpconf As HistoryProcessor.Configuration, ByVal conf As A3CConfiguration, ByVal dataManager As IDataManager)
			Me.New(mdp, factory.buildActorCritic(hpconf.getShape(), mdp.ActionSpace.getSize()), hpconf, conf, dataManager)
		End Sub

		<Obsolete>
		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal factory As ActorCriticFactoryCompGraph, ByVal hpconf As HistoryProcessor.Configuration, ByVal conf As A3CConfiguration)
			Me.New(mdp, factory.buildActorCritic(hpconf.getShape(), mdp.ActionSpace.getSize()), hpconf, conf)
		End Sub

		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal factory As ActorCriticFactoryCompGraph, ByVal hpconf As HistoryProcessor.Configuration, ByVal conf As A3CLearningConfiguration)
			Me.New(mdp, factory.buildActorCritic(hpconf.getShape(), mdp.ActionSpace.getSize()), hpconf, conf)
		End Sub

		<Obsolete>
		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal netConf As ActorCriticFactoryCompGraphStdConv.Configuration, ByVal hpconf As HistoryProcessor.Configuration, ByVal conf As A3CConfiguration, ByVal dataManager As IDataManager)
			Me.New(mdp, New ActorCriticFactoryCompGraphStdConv(netConf.toNetworkConfiguration()), hpconf, conf, dataManager)
		End Sub

		<Obsolete>
		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal netConf As ActorCriticFactoryCompGraphStdConv.Configuration, ByVal hpconf As HistoryProcessor.Configuration, ByVal conf As A3CConfiguration)
			Me.New(mdp, New ActorCriticFactoryCompGraphStdConv(netConf.toNetworkConfiguration()), hpconf, conf)
		End Sub

		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal netConf As ActorCriticNetworkConfiguration, ByVal hpconf As HistoryProcessor.Configuration, ByVal conf As A3CLearningConfiguration)
			Me.New(mdp, New ActorCriticFactoryCompGraphStdConv(netConf), hpconf, conf)
		End Sub

		Public Overrides Function newThread(ByVal i As Integer, ByVal deviceNum As Integer) As AsyncThread
			Dim at As AsyncThread = MyBase.newThread(i, deviceNum)
			at.setHistoryProcessor(hpconf)
			Return at
		End Function
	End Class

End Namespace