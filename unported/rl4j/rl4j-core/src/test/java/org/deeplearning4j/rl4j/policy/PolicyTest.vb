Imports System
Imports System.IO
Imports NeuralNetwork = org.deeplearning4j.nn.api.NeuralNetwork
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Features = org.deeplearning4j.rl4j.agent.learning.update.Features
Imports FeaturesLabels = org.deeplearning4j.rl4j.agent.learning.update.FeaturesLabels
Imports Gradients = org.deeplearning4j.rl4j.agent.learning.update.Gradients
Imports IHistoryProcessor = org.deeplearning4j.rl4j.learning.IHistoryProcessor
Imports org.deeplearning4j.rl4j.learning
Imports QLearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.QLearningConfiguration
Imports org.deeplearning4j.rl4j.network
Imports NeuralNetOutput = org.deeplearning4j.rl4j.network.NeuralNetOutput
Imports org.deeplearning4j.rl4j.network.ac
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports org.deeplearning4j.rl4j.space
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports org.deeplearning4j.rl4j.support
Imports org.deeplearning4j.rl4j.util
Imports Test = org.junit.jupiter.api.Test
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.junit.jupiter.api.Assertions

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

Namespace org.deeplearning4j.rl4j.policy


	''' 
	''' <summary>
	''' @author saudet
	''' </summary>
	Public Class PolicyTest

		Public Class DummyAC(Of NN As DummyAC)
			Implements IActorCritic(Of NN)

'JAVA TO VB CONVERTER NOTE: The field nn was renamed since Visual Basic does not allow fields with the same name as class-level generic type parameters:
			Friend nn_Conflict As NeuralNetwork
			Friend Sub New(ByVal nn As NeuralNetwork)
				Me.nn_Conflict = nn
			End Sub

			Public Overrides ReadOnly Property NeuralNetworks As NeuralNetwork()
				Get
					Return New NeuralNetwork() { nn_Conflict }
				End Get
			End Property

			Public Overrides ReadOnly Property Recurrent As Boolean
				Get
					Throw New System.NotSupportedException()
				End Get
			End Property

			Public Overrides Sub reset()
				Throw New System.NotSupportedException()
			End Sub

			Public Overrides Sub fit(ByVal input As INDArray, ByVal labels() As INDArray)
				Throw New System.NotSupportedException()
			End Sub

			Public Overridable Function outputAll(ByVal batch As INDArray) As INDArray() Implements IActorCritic(Of NN).outputAll
				Return New INDArray() {batch, batch}
			End Function

			Public Overrides Function clone() As NN
				Throw New System.NotSupportedException()
			End Function

			Public Overrides Sub fit(ByVal featuresLabels As FeaturesLabels)
				Throw New System.NotSupportedException()
			End Sub

			Public Overrides Function computeGradients(ByVal featuresLabels As FeaturesLabels) As Gradients
				Throw New System.NotSupportedException()
			End Function

			Public Overrides Sub applyGradients(ByVal gradients As Gradients)
				Throw New System.NotSupportedException()
			End Sub

			Public Overrides Sub copyFrom(ByVal from As NN)
				Throw New System.NotSupportedException()
			End Sub

			Public Overrides Function gradient(ByVal input As INDArray, ByVal labels() As INDArray) As Gradient()
				Throw New System.NotSupportedException()
			End Function

			Public Overrides Sub applyGradient(ByVal gradient() As Gradient, ByVal batchSize As Integer)
				Throw New System.NotSupportedException()
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void save(java.io.OutputStream streamValue, java.io.OutputStream streamPolicy) throws java.io.IOException
			Public Overridable Sub save(ByVal streamValue As Stream, ByVal streamPolicy As Stream) Implements IActorCritic(Of NN).save
				Throw New System.NotSupportedException()
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void save(String pathValue, String pathPolicy) throws java.io.IOException
			Public Overridable Sub save(ByVal pathValue As String, ByVal pathPolicy As String) Implements IActorCritic(Of NN).save
				Throw New System.NotSupportedException()
			End Sub

			Public Overrides ReadOnly Property LatestScore As Double
				Get
					Throw New System.NotSupportedException()
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void save(java.io.OutputStream os) throws java.io.IOException
			Public Overrides Sub save(ByVal os As Stream)
				Throw New System.NotSupportedException()
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void save(String filename) throws java.io.IOException
			Public Overrides Sub save(ByVal filename As String)
				Throw New System.NotSupportedException()
			End Sub

			Public Overrides Function output(ByVal observation As Observation) As NeuralNetOutput
				Throw New System.NotSupportedException()
			End Function

			Public Overrides Function output(ByVal batch As INDArray) As NeuralNetOutput
				Throw New System.NotSupportedException()
			End Function

			Public Overrides Function output(ByVal features As Features) As NeuralNetOutput
				Throw New System.NotSupportedException()
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testACPolicy() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testACPolicy()
			Dim cg As New ComputationGraph((New NeuralNetConfiguration.Builder()).seed(444).graphBuilder().addInputs("input").addLayer("output", (New OutputLayer.Builder()).nOut(1).lossFunction(LossFunctions.LossFunction.XENT).activation(Activation.SIGMOID).build(), "input").setOutputs("output").build())
			Dim mln As New MultiLayerNetwork((New NeuralNetConfiguration.Builder()).seed(555).list().layer(0, (New OutputLayer.Builder()).nOut(1).lossFunction(LossFunctions.LossFunction.XENT).activation(Activation.SIGMOID).build()).build())

			Dim policy As New ACPolicy(New DummyAC(mln), True, Nd4j.Random)

			Dim input As INDArray = Nd4j.create(New Double() {1.0, 0.0}, New Long(){1, 2})
			For i As Integer = 0 To 99
				assertEquals(0, CInt(Math.Truncate(policy.nextAction(input))))
			Next i

			input = Nd4j.create(New Double() {0.0, 1.0}, New Long(){1, 2})
			For i As Integer = 0 To 99
				assertEquals(1, CInt(Math.Truncate(policy.nextAction(input))))
			Next i

			input = Nd4j.create(New Double() {0.1, 0.2, 0.3, 0.4}, New Long(){1, 4})
			Dim count(3) As Integer
			For i As Integer = 0 To 99
				count(policy.nextAction(input)) += 1
			Next i
	'        System.out.println(count[0] + " " + count[1] + " " + count[2] + " " + count[3]);
			assertTrue(count(0) < 20)
			assertTrue(count(1) < 30)
			assertTrue(count(2) < 40)
			assertTrue(count(3) < 50)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void refacPolicyPlay()
		Public Overridable Sub refacPolicyPlay()
			' Arrange
			Dim observationSpace As New MockObservationSpace()
			Dim dqn As New MockDQN()
			Dim random As New MockRandom(New Double() { 0.7309677600860596, 0.8314409852027893, 0.2405363917350769, 0.6063451766967773, 0.6374173760414124, 0.3090505599975586, 0.5504369735717773, 0.11700659990310669 }, New Integer() { 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4 })
			Dim mdp As New MockMDP(observationSpace, 30, random)

			Dim conf As QLearningConfiguration = QLearningConfiguration.builder().seed(0L).maxEpochStep(0).maxStep(0).expRepMaxSize(5).batchSize(1).targetDqnUpdateFreq(0).updateStart(0).rewardFactor(1.0).gamma(0).errorClamp(0).minEpsilon(0).epsilonNbStep(0).doubleDQN(True).build()

			Dim nnMock As New MockNeuralNet()
			Dim hpConf As New IHistoryProcessor.Configuration(5, 4, 4, 4, 4, 0, 0, 2)
			Dim sut As New MockRefacPolicy(nnMock, observationSpace.Shape, hpConf.getSkipFrame(), hpConf.getHistoryLength())
			Dim hp As New MockHistoryProcessor(hpConf)

			' Act
			Dim totalReward As Double = sut.play(mdp, hp)

			' Assert
			assertEquals(1, nnMock.resetCallCount)
			assertEquals(465.0, totalReward, 0.0001)

			' MDP
			assertEquals(1, mdp.resetCount)
			assertEquals(30, mdp.actions.Count)
			For i As Integer = 0 To mdp.actions.Count - 1
				assertEquals(0, CInt(mdp.actions(i)))
			Next i

			' DQN
			assertEquals(0, dqn.fitParams.Count)
			assertEquals(0, dqn.outputParams.Count)
		End Sub

		Public Class MockRefacPolicy
			Inherits Policy(Of Integer)

'JAVA TO VB CONVERTER NOTE: The field neuralNet was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend neuralNet_Conflict As NeuralNet
			Friend ReadOnly shape() As Integer
			Friend ReadOnly skipFrame As Integer
			Friend ReadOnly historyLength As Integer

			Public Sub New(ByVal neuralNet As NeuralNet, ByVal shape() As Integer, ByVal skipFrame As Integer, ByVal historyLength As Integer)
				Me.neuralNet_Conflict = neuralNet
				Me.shape = shape
				Me.skipFrame = skipFrame
				Me.historyLength = historyLength
			End Sub

			Public Overrides ReadOnly Property NeuralNet As NeuralNet
				Get
					Return neuralNet_Conflict
				End Get
			End Property

			Public Overrides Function nextAction(ByVal obs As Observation) As Integer?
				Return nextAction(obs.Data)
			End Function

			Public Overridable Overloads Function nextAction(ByVal input As INDArray) As Integer?
				Return CInt(Math.Truncate(input.getDouble(0)))
			End Function

			Protected Friend Overrides Function refacInitMdp(Of MockObservation As Encodable, [AS] As ActionSpace(Of Integer))(ByVal mdpWrapper As LegacyMDPWrapper(Of MockObservation, Integer, [AS]), ByVal hp As IHistoryProcessor) As Learning.InitMdp(Of Observation)
				mdpWrapper.setTransformProcess(MockMDP.buildTransformProcess(skipFrame, historyLength))
				Return MyBase.refacInitMdp(mdpWrapper, hp)
			End Function
		End Class
	End Class

End Namespace