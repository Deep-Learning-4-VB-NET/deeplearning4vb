Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Getter = lombok.Getter
Imports NotImplementedException = org.apache.commons.lang3.NotImplementedException
Imports NeuralNetwork = org.deeplearning4j.nn.api.NeuralNetwork
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports RnnOutputLayer = org.deeplearning4j.nn.layers.recurrent.RnnOutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports Features = org.deeplearning4j.rl4j.agent.learning.update.Features
Imports FeaturesLabels = org.deeplearning4j.rl4j.agent.learning.update.FeaturesLabels
Imports Gradients = org.deeplearning4j.rl4j.agent.learning.update.Gradients
Imports CommonGradientNames = org.deeplearning4j.rl4j.network.CommonGradientNames
Imports CommonLabelNames = org.deeplearning4j.rl4j.network.CommonLabelNames
Imports CommonOutputNames = org.deeplearning4j.rl4j.network.CommonOutputNames
Imports NeuralNetOutput = org.deeplearning4j.rl4j.network.NeuralNetOutput
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.rl4j.network.ac


	<Obsolete>
	Public Class ActorCriticSeparate(Of NN As ActorCriticSeparate)
		Implements IActorCritic(Of NN)

		Protected Friend ReadOnly valueNet As MultiLayerNetwork
		Protected Friend ReadOnly policyNet As MultiLayerNetwork
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final boolean recurrent;
		Protected Friend ReadOnly recurrent As Boolean

		Public Sub New(ByVal valueNet As MultiLayerNetwork, ByVal policyNet As MultiLayerNetwork)
			Me.valueNet = valueNet
			Me.policyNet = policyNet
			Me.recurrent = TypeOf valueNet.OutputLayer Is RnnOutputLayer
		End Sub

		Public Overridable ReadOnly Property NeuralNetworks As NeuralNetwork()
			Get
				Return New NeuralNetwork() { valueNet, policyNet }
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static ActorCriticSeparate load(String pathValue, String pathPolicy) throws java.io.IOException
		Public Shared Function load(ByVal pathValue As String, ByVal pathPolicy As String) As ActorCriticSeparate
			Return New ActorCriticSeparate(ModelSerializer.restoreMultiLayerNetwork(pathValue), ModelSerializer.restoreMultiLayerNetwork(pathPolicy))
		End Function

		Public Overridable Sub reset()
			If recurrent Then
				valueNet.rnnClearPreviousState()
				policyNet.rnnClearPreviousState()
			End If
		End Sub

		Public Overridable Sub fit(ByVal input As INDArray, ByVal labels() As INDArray)
			valueNet.fit(input, labels(0))
			policyNet.fit(input, labels(1))
		End Sub

		Public Overridable Function outputAll(ByVal batch As INDArray) As INDArray() Implements IActorCritic(Of NN).outputAll
			If recurrent Then
				Return New INDArray() {valueNet.rnnTimeStep(batch), policyNet.rnnTimeStep(batch)}
			Else
				Return New INDArray() {valueNet.output(batch), policyNet.output(batch)}
			End If
		End Function

		Public Overridable Function clone() As NN
			Dim nn As NN = CType(New ActorCriticSeparate(valueNet.clone(), policyNet.clone()), NN)
			nn.valueNet.setListeners(valueNet.getListeners())
			nn.policyNet.setListeners(policyNet.getListeners())
			Return nn
		End Function

		Public Overrides Sub fit(ByVal featuresLabels As FeaturesLabels)
			valueNet.fit(featuresLabels.getFeatures().get(0), featuresLabels.getLabels(CommonLabelNames.ActorCritic.Value))
			policyNet.fit(featuresLabels.getFeatures().get(0), featuresLabels.getLabels(CommonLabelNames.ActorCritic.Policy))
		End Sub

		Public Overrides Function computeGradients(ByVal featuresLabels As FeaturesLabels) As Gradients
			valueNet.Input = featuresLabels.getFeatures().get(0)
			valueNet.Labels = featuresLabels.getLabels(CommonLabelNames.ActorCritic.Value)
			valueNet.computeGradientAndScore()
			Dim valueIterationListeners As ICollection(Of TrainingListener) = valueNet.getListeners()
			If valueIterationListeners IsNot Nothing AndAlso valueIterationListeners.Count > 0 Then
				For Each l As TrainingListener In valueIterationListeners
					l.onGradientCalculation(valueNet)
				Next l
			End If

			policyNet.Input = featuresLabels.getFeatures().get(0)
			policyNet.Labels = featuresLabels.getLabels(CommonLabelNames.ActorCritic.Policy)
			policyNet.computeGradientAndScore()
			Dim policyIterationListeners As ICollection(Of TrainingListener) = policyNet.getListeners()
			If policyIterationListeners IsNot Nothing AndAlso policyIterationListeners.Count > 0 Then
				For Each l As TrainingListener In policyIterationListeners
					l.onGradientCalculation(policyNet)
				Next l
			End If

			Dim result As New Gradients(featuresLabels.BatchSize)
			result.putGradient(CommonGradientNames.ActorCritic.Value, valueNet.gradient())
			result.putGradient(CommonGradientNames.ActorCritic.Policy, policyNet.gradient())
			Return result
		End Function

		Public Overrides Sub applyGradients(ByVal gradients As Gradients)
			Dim batchSize As Integer = CInt(Math.Truncate(gradients.getBatchSize()))
			Dim valueConf As MultiLayerConfiguration = valueNet.LayerWiseConfigurations
			Dim valueIterationCount As Integer = valueConf.getIterationCount()
			Dim valueEpochCount As Integer = valueConf.EpochCount
			Dim valueGradient As Gradient = gradients.getGradient(CommonGradientNames.ActorCritic.Value)
			valueNet.Updater.update(valueNet, valueGradient, valueIterationCount, valueEpochCount, batchSize, LayerWorkspaceMgr.noWorkspaces())
			valueNet.params().subi(valueGradient.gradient())
			Dim valueIterationListeners As ICollection(Of TrainingListener) = valueNet.getListeners()
			If valueIterationListeners IsNot Nothing AndAlso valueIterationListeners.Count > 0 Then
				For Each listener As TrainingListener In valueIterationListeners
					listener.iterationDone(valueNet, valueIterationCount, valueEpochCount)
				Next listener
			End If
			valueConf.setIterationCount(valueIterationCount + 1)

			Dim policyConf As MultiLayerConfiguration = policyNet.LayerWiseConfigurations
			Dim policyIterationCount As Integer = policyConf.getIterationCount()
			Dim policyEpochCount As Integer = policyConf.EpochCount
			Dim policyGradient As Gradient = gradients.getGradient(CommonGradientNames.ActorCritic.Policy)
			policyNet.Updater.update(policyNet, policyGradient, policyIterationCount, policyEpochCount, batchSize, LayerWorkspaceMgr.noWorkspaces())
			policyNet.params().subi(policyGradient.gradient())
			Dim policyIterationListeners As ICollection(Of TrainingListener) = policyNet.getListeners()
			If policyIterationListeners IsNot Nothing AndAlso policyIterationListeners.Count > 0 Then
				For Each listener As TrainingListener In policyIterationListeners
					listener.iterationDone(policyNet, policyIterationCount, policyEpochCount)
				Next listener
			End If
			policyConf.setIterationCount(policyIterationCount + 1)
		End Sub

		Public Overridable Sub copyFrom(ByVal from As NN)
			valueNet.Params = from.valueNet.params()
			policyNet.Params = from.policyNet.params()
		End Sub

		<Obsolete>
		Public Overridable Function gradient(ByVal input As INDArray, ByVal labels() As INDArray) As Gradient()
			valueNet.Input = input
			valueNet.Labels = labels(0)
			valueNet.computeGradientAndScore()
			Dim valueIterationListeners As ICollection(Of TrainingListener) = valueNet.getListeners()
			If valueIterationListeners IsNot Nothing AndAlso valueIterationListeners.Count > 0 Then
				For Each l As TrainingListener In valueIterationListeners
						l.onGradientCalculation(valueNet)
				Next l
			End If

			policyNet.Input = input
			policyNet.Labels = labels(1)
			policyNet.computeGradientAndScore()
			Dim policyIterationListeners As ICollection(Of TrainingListener) = policyNet.getListeners()
			If policyIterationListeners IsNot Nothing AndAlso policyIterationListeners.Count > 0 Then
				For Each l As TrainingListener In policyIterationListeners
					l.onGradientCalculation(policyNet)
				Next l
			End If
			Return New Gradient() {valueNet.gradient(), policyNet.gradient()}
		End Function

		<Obsolete>
		Public Overridable Sub applyGradient(ByVal gradient() As Gradient, ByVal batchSize As Integer)
			Dim valueConf As MultiLayerConfiguration = valueNet.LayerWiseConfigurations
			Dim valueIterationCount As Integer = valueConf.getIterationCount()
			Dim valueEpochCount As Integer = valueConf.EpochCount
			valueNet.Updater.update(valueNet, gradient(0), valueIterationCount, valueEpochCount, batchSize, LayerWorkspaceMgr.noWorkspaces())
			valueNet.params().subi(gradient(0).gradient())
			Dim valueIterationListeners As ICollection(Of TrainingListener) = valueNet.getListeners()
			If valueIterationListeners IsNot Nothing AndAlso valueIterationListeners.Count > 0 Then
				For Each listener As TrainingListener In valueIterationListeners
					listener.iterationDone(valueNet, valueIterationCount, valueEpochCount)
				Next listener
			End If
			valueConf.setIterationCount(valueIterationCount + 1)

			Dim policyConf As MultiLayerConfiguration = policyNet.LayerWiseConfigurations
			Dim policyIterationCount As Integer = policyConf.getIterationCount()
			Dim policyEpochCount As Integer = policyConf.EpochCount
			policyNet.Updater.update(policyNet, gradient(1), policyIterationCount, policyEpochCount, batchSize, LayerWorkspaceMgr.noWorkspaces())
			policyNet.params().subi(gradient(1).gradient())
			Dim policyIterationListeners As ICollection(Of TrainingListener) = policyNet.getListeners()
			If policyIterationListeners IsNot Nothing AndAlso policyIterationListeners.Count > 0 Then
				For Each listener As TrainingListener In policyIterationListeners
					listener.iterationDone(policyNet, policyIterationCount, policyEpochCount)
				Next listener
			End If
			policyConf.setIterationCount(policyIterationCount + 1)
		End Sub

		Public Overridable ReadOnly Property LatestScore As Double
			Get
				Return valueNet.score()
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void save(java.io.OutputStream stream) throws java.io.IOException
		Public Overridable Sub save(ByVal stream As Stream)
			Throw New System.NotSupportedException("Call save(streamValue, streamPolicy)")
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void save(String path) throws java.io.IOException
		Public Overridable Sub save(ByVal path As String)
			Throw New System.NotSupportedException("Call save(pathValue, pathPolicy)")
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void save(java.io.OutputStream streamValue, java.io.OutputStream streamPolicy) throws java.io.IOException
		Public Overridable Sub save(ByVal streamValue As Stream, ByVal streamPolicy As Stream) Implements IActorCritic(Of NN).save
			ModelSerializer.writeModel(valueNet, streamValue, True)
			ModelSerializer.writeModel(policyNet, streamPolicy, True)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void save(String pathValue, String pathPolicy) throws java.io.IOException
		Public Overridable Sub save(ByVal pathValue As String, ByVal pathPolicy As String) Implements IActorCritic(Of NN).save
			ModelSerializer.writeModel(valueNet, pathValue, True)
			ModelSerializer.writeModel(policyNet, pathPolicy, True)
		End Sub

		Public Overrides Function output(ByVal observation As Observation) As NeuralNetOutput
			If Not isRecurrent() Then
				Return output(observation.getChannelData(0))
			End If

			Dim observationData As INDArray = observation.getChannelData(0)
			Return packageResult(valueNet.rnnTimeStep(observationData), policyNet.rnnTimeStep(observationData))
		End Function

		Public Overrides Function output(ByVal batch As INDArray) As NeuralNetOutput
			Return packageResult(valueNet.output(batch), policyNet.output(batch))
		End Function

		Public Overrides Function output(ByVal features As Features) As NeuralNetOutput
			Throw New NotImplementedException("Not implemented in legacy classes")
		End Function

		Private Function packageResult(ByVal value As INDArray, ByVal policy As INDArray) As NeuralNetOutput
			Dim result As New NeuralNetOutput()
			result.put(CommonOutputNames.ActorCritic.Value, value)
			result.put(CommonOutputNames.ActorCritic.Policy, policy)

			Return result
		End Function
	End Class



End Namespace