Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Getter = lombok.Getter
Imports NotImplementedException = org.apache.commons.lang3.NotImplementedException
Imports NeuralNetwork = org.deeplearning4j.nn.api.NeuralNetwork
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports RnnOutputLayer = org.deeplearning4j.nn.layers.recurrent.RnnOutputLayer
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
	Public Class ActorCriticCompGraph
		Implements IActorCritic(Of ActorCriticCompGraph)

		Protected Friend ReadOnly cg As ComputationGraph
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final boolean recurrent;
		Protected Friend ReadOnly recurrent As Boolean

		Public Sub New(ByVal cg As ComputationGraph)
			Me.cg = cg
			Me.recurrent = TypeOf cg.getOutputLayer(0) Is RnnOutputLayer
		End Sub

		Public Overridable ReadOnly Property NeuralNetworks As NeuralNetwork()
			Get
				Return New NeuralNetwork() { cg }
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static ActorCriticCompGraph load(String path) throws java.io.IOException
		Public Shared Function load(ByVal path As String) As ActorCriticCompGraph
			Return New ActorCriticCompGraph(ModelSerializer.restoreComputationGraph(path))
		End Function

		Public Overridable Sub fit(ByVal input As INDArray, ByVal labels() As INDArray)
			cg.fit(New INDArray() {input}, labels)
		End Sub

		Public Overridable Sub reset()
			If recurrent Then
				cg.rnnClearPreviousState()
			End If
		End Sub

		Public Overridable Function outputAll(ByVal batch As INDArray) As INDArray() Implements IActorCritic(Of ActorCriticCompGraph).outputAll
			If recurrent Then
				Return cg.rnnTimeStep(batch)
			Else
				Return cg.output(batch)
			End If
		End Function

		Public Overridable Function clone() As ActorCriticCompGraph
			Dim nn As New ActorCriticCompGraph(cg.clone())
			nn.cg.setListeners(cg.getListeners())
			Return nn
		End Function

		Public Overrides Sub fit(ByVal featuresLabels As FeaturesLabels)
			Dim features() As INDArray = { featuresLabels.getFeatures().get(0) }
			Dim labels() As INDArray = { featuresLabels.getLabels(CommonLabelNames.ActorCritic.Value), featuresLabels.getLabels(CommonLabelNames.ActorCritic.Policy) }
			cg.fit(features, labels)
		End Sub

		Public Overrides Function computeGradients(ByVal featuresLabels As FeaturesLabels) As Gradients
			cg.setInput(0, featuresLabels.getFeatures().get(0))
			cg.setLabels(featuresLabels.getLabels(CommonLabelNames.ActorCritic.Value), featuresLabels.getLabels(CommonLabelNames.ActorCritic.Policy))
			cg.computeGradientAndScore()
			Dim iterationListeners As ICollection(Of TrainingListener) = cg.getListeners()
			If iterationListeners IsNot Nothing AndAlso iterationListeners.Count > 0 Then
				For Each l As TrainingListener In iterationListeners
					l.onGradientCalculation(cg)
				Next l
			End If

			Dim result As New Gradients(featuresLabels.BatchSize)
			result.putGradient(CommonGradientNames.ActorCritic.Combined, cg.gradient())

			Return result
		End Function

		Public Overrides Sub applyGradients(ByVal gradients As Gradients)
			Dim cgConf As ComputationGraphConfiguration = cg.Configuration
			Dim iterationCount As Integer = cgConf.getIterationCount()
			Dim epochCount As Integer = cgConf.getEpochCount()

			Dim gradient As Gradient = gradients.getGradient(CommonGradientNames.ActorCritic.Combined)
			cg.Updater.update(gradient, iterationCount, epochCount, CInt(Math.Truncate(gradients.getBatchSize())), LayerWorkspaceMgr.noWorkspaces())
			cg.params().subi(gradient.gradient())
			Dim iterationListeners As ICollection(Of TrainingListener) = cg.getListeners()
			If iterationListeners IsNot Nothing AndAlso iterationListeners.Count > 0 Then
				For Each listener As TrainingListener In iterationListeners
					listener.iterationDone(cg, iterationCount, epochCount)
				Next listener
			End If
			cgConf.setIterationCount(iterationCount + 1)
		End Sub

		Public Overridable Sub copyFrom(ByVal from As ActorCriticCompGraph)
			cg.Params = from.cg.params()
		End Sub

		<Obsolete>
		Public Overridable Function gradient(ByVal input As INDArray, ByVal labels() As INDArray) As Gradient()
			cg.setInput(0, input)
			cg.Labels = labels
			cg.computeGradientAndScore()
			Dim iterationListeners As ICollection(Of TrainingListener) = cg.getListeners()
			If iterationListeners IsNot Nothing AndAlso iterationListeners.Count > 0 Then
				For Each l As TrainingListener In iterationListeners
					l.onGradientCalculation(cg)
				Next l
			End If
			Return New Gradient() {cg.gradient()}
		End Function


		Public Overridable Sub applyGradient(ByVal gradient() As Gradient, ByVal batchSize As Integer)
			If recurrent Then
				' assume batch sizes of 1 for recurrent networks,
				' since we are learning each episode as a time serie
				batchSize = 1
			End If
			Dim cgConf As ComputationGraphConfiguration = cg.Configuration
			Dim iterationCount As Integer = cgConf.getIterationCount()
			Dim epochCount As Integer = cgConf.getEpochCount()
			cg.Updater.update(gradient(0), iterationCount, epochCount, batchSize, LayerWorkspaceMgr.noWorkspaces())
			cg.params().subi(gradient(0).gradient())
			Dim iterationListeners As ICollection(Of TrainingListener) = cg.getListeners()
			If iterationListeners IsNot Nothing AndAlso iterationListeners.Count > 0 Then
				For Each listener As TrainingListener In iterationListeners
					listener.iterationDone(cg, iterationCount, epochCount)
				Next listener
			End If
			cgConf.setIterationCount(iterationCount + 1)
		End Sub

		Public Overridable ReadOnly Property LatestScore As Double
			Get
				Return cg.score()
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void save(java.io.OutputStream stream) throws java.io.IOException
		Public Overridable Sub save(ByVal stream As Stream)
			ModelSerializer.writeModel(cg, stream, True)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void save(String path) throws java.io.IOException
		Public Overridable Sub save(ByVal path As String)
			ModelSerializer.writeModel(cg, path, True)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void save(java.io.OutputStream streamValue, java.io.OutputStream streamPolicy) throws java.io.IOException
		Public Overridable Sub save(ByVal streamValue As Stream, ByVal streamPolicy As Stream) Implements IActorCritic(Of ActorCriticCompGraph).save
			Throw New System.NotSupportedException("Call save(stream)")
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void save(String pathValue, String pathPolicy) throws java.io.IOException
		Public Overridable Sub save(ByVal pathValue As String, ByVal pathPolicy As String) Implements IActorCritic(Of ActorCriticCompGraph).save
			Throw New System.NotSupportedException("Call save(path)")
		End Sub

		Public Overrides Function output(ByVal observation As Observation) As NeuralNetOutput
			If Not isRecurrent() Then
				Return output(observation.getChannelData(0))
			End If

			Dim cgOutput() As INDArray = cg.rnnTimeStep(observation.getChannelData(0))
			Return packageResult(cgOutput(0), cgOutput(1))
		End Function

		Public Overrides Function output(ByVal batch As INDArray) As NeuralNetOutput
			Dim cgOutput() As INDArray = cg.output(batch)
			Return packageResult(cgOutput(0), cgOutput(1))
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