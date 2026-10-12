Imports System
Imports System.Collections.Generic
Imports System.IO
Imports NotImplementedException = org.apache.commons.lang3.NotImplementedException
Imports NeuralNetwork = org.deeplearning4j.nn.api.NeuralNetwork
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
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

Namespace org.deeplearning4j.rl4j.network.dqn


	<Obsolete>
	Public Class DQN
		Implements IDQN(Of DQN)

		Protected Friend ReadOnly mln As MultiLayerNetwork

		Friend i As Integer = 0

		Public Sub New(ByVal mln As MultiLayerNetwork)
			Me.mln = mln
		End Sub

		Public Overridable ReadOnly Property NeuralNetworks As NeuralNetwork()
			Get
				Return New NeuralNetwork() { mln }
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static DQN load(String path) throws java.io.IOException
		Public Shared Function load(ByVal path As String) As DQN
			Return New DQN(ModelSerializer.restoreMultiLayerNetwork(path))
		End Function

		Public Overridable ReadOnly Property Recurrent As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overridable Sub reset()
			' no recurrent layer
		End Sub

		Public Overridable Sub fit(ByVal input As INDArray, ByVal labels As INDArray) Implements IDQN(Of DQN).fit
			mln.fit(input, labels)
		End Sub

		Public Overridable Sub fit(ByVal input As INDArray, ByVal labels() As INDArray)
			fit(input, labels(0))
		End Sub

		Public Overridable Function output(ByVal batch As INDArray) As NeuralNetOutput
			Dim result As New NeuralNetOutput()
			result.put(CommonOutputNames.QValues, mln.output(batch))

			Return result
		End Function

		Public Overrides Function output(ByVal features As Features) As NeuralNetOutput
			Dim result As New NeuralNetOutput()
			result.put(CommonOutputNames.QValues, mln.output(features.get(0)))

			Return result
		End Function

		Public Overridable Function output(ByVal observation As Observation) As NeuralNetOutput
			Return output(observation.getChannelData(0))
		End Function

		<Obsolete>
		Public Overridable Function outputAll(ByVal batch As INDArray) As INDArray()
			Return New INDArray() {output(batch).get(CommonOutputNames.QValues)}
		End Function

		Public Overrides Sub fit(ByVal featuresLabels As FeaturesLabels)
			fit(featuresLabels.getFeatures().get(0), featuresLabels.getLabels(CommonLabelNames.QValues))
		End Sub

		Public Overrides Sub copyFrom(ByVal from As DQN)
			mln.Params = from.mln.params()
		End Sub

		Public Overrides Function clone() As DQN
			Dim nn As New DQN(mln.clone())
			nn.mln.setListeners(mln.getListeners())
			Return nn
		End Function

		Public Overridable Function gradient(ByVal input As INDArray, ByVal labels As INDArray) As Gradient() Implements IDQN(Of DQN).gradient
			mln.Input = input
			mln.Labels = labels
			mln.computeGradientAndScore()
			Dim iterationListeners As ICollection(Of TrainingListener) = mln.getListeners()
			If iterationListeners IsNot Nothing AndAlso iterationListeners.Count > 0 Then
				For Each l As TrainingListener In iterationListeners
					l.onGradientCalculation(mln)
				Next l
			End If
			'System.out.println("SCORE: " + mln.score());
			Return New Gradient() {mln.gradient()}
		End Function

		Public Overridable Function gradient(ByVal input As INDArray, ByVal labels() As INDArray) As Gradient() Implements IDQN(Of DQN).gradient
			Return gradient(input, labels(0))
		End Function


		Public Overrides Function computeGradients(ByVal featuresLabels As FeaturesLabels) As Gradients
			mln.Input = featuresLabels.getFeatures().get(0)
			mln.Labels = featuresLabels.getLabels(CommonLabelNames.QValues)
			mln.computeGradientAndScore()
			Dim iterationListeners As ICollection(Of TrainingListener) = mln.getListeners()
			If iterationListeners IsNot Nothing AndAlso iterationListeners.Count > 0 Then
				For Each l As TrainingListener In iterationListeners
					l.onGradientCalculation(mln)
				Next l
			End If
			Dim result As New Gradients(featuresLabels.BatchSize)
			result.putGradient(CommonGradientNames.QValues, mln.gradient())
			Return result
		End Function

		Public Overrides Sub applyGradients(ByVal gradients As Gradients)
			Dim qValues As Gradient = gradients.getGradient(CommonGradientNames.QValues)

			Dim mlnConf As MultiLayerConfiguration = mln.LayerWiseConfigurations
			Dim iterationCount As Integer = mlnConf.getIterationCount()
			Dim epochCount As Integer = mlnConf.EpochCount
			mln.Updater.update(mln, qValues, iterationCount, epochCount, CInt(Math.Truncate(gradients.getBatchSize())), LayerWorkspaceMgr.noWorkspaces())
			mln.params().subi(qValues.gradient())
			Dim iterationListeners As ICollection(Of TrainingListener) = mln.getListeners()
			If iterationListeners IsNot Nothing AndAlso iterationListeners.Count > 0 Then
				For Each listener As TrainingListener In iterationListeners
					listener.iterationDone(mln, iterationCount, epochCount)
				Next listener
			End If
			mlnConf.setIterationCount(iterationCount + 1)
		End Sub

		Public Overridable Sub applyGradient(ByVal gradient() As Gradient, ByVal batchSize As Integer)
			Dim mlnConf As MultiLayerConfiguration = mln.LayerWiseConfigurations
			Dim iterationCount As Integer = mlnConf.getIterationCount()
			Dim epochCount As Integer = mlnConf.EpochCount
			mln.Updater.update(mln, gradient(0), iterationCount, epochCount, batchSize, LayerWorkspaceMgr.noWorkspaces())
			mln.params().subi(gradient(0).gradient())
			Dim iterationListeners As ICollection(Of TrainingListener) = mln.getListeners()
			If iterationListeners IsNot Nothing AndAlso iterationListeners.Count > 0 Then
				For Each listener As TrainingListener In iterationListeners
					listener.iterationDone(mln, iterationCount, epochCount)
				Next listener
			End If
			mlnConf.setIterationCount(iterationCount + 1)
		End Sub

		Public Overridable ReadOnly Property LatestScore As Double
			Get
				Return mln.score()
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void save(java.io.OutputStream stream) throws java.io.IOException
		Public Overridable Sub save(ByVal stream As Stream)
			ModelSerializer.writeModel(mln, stream, True)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void save(String path) throws java.io.IOException
		Public Overridable Sub save(ByVal path As String)
			ModelSerializer.writeModel(mln, path, True)
		End Sub
	End Class

End Namespace