Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports RnnOutputLayer = org.deeplearning4j.nn.layers.recurrent.RnnOutputLayer
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports Features = org.deeplearning4j.rl4j.agent.learning.update.Features
Imports FeaturesLabels = org.deeplearning4j.rl4j.agent.learning.update.FeaturesLabels
Imports Gradients = org.deeplearning4j.rl4j.agent.learning.update.Gradients
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
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
Namespace org.deeplearning4j.rl4j.network

	Public Class ComputationGraphHandler
		Implements INetworkHandler

		Private ReadOnly model As ComputationGraph

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final boolean recurrent;
		Private ReadOnly recurrent As Boolean
		Private ReadOnly configuration As ComputationGraphConfiguration
		Private ReadOnly labelNames() As String
		Private ReadOnly gradientName As String
		Private ReadOnly inputFeatureIdx As Integer
		Private ReadOnly channelToNetworkInputMapper As ChannelToNetworkInputMapper

		''' <param name="model"> The <seealso cref="ComputationGraph"/> to use internally. </param>
		''' <param name="labelNames"> An array of the labels (in <seealso cref="FeaturesLabels"/>) to use as the network's input. </param>
		''' <param name="gradientName"> The name of the gradient (in <seealso cref="Gradients"/>) to use as the network's output. </param>
		''' <param name="channelToNetworkInputMapper"> a <seealso cref="ChannelToNetworkInputMapper"/> instance that map the network inputs
		'''                                    to the feature channels </param>
		Public Sub New(ByVal model As ComputationGraph, ByVal labelNames() As String, ByVal gradientName As String, ByVal channelToNetworkInputMapper As ChannelToNetworkInputMapper)
			Me.model = model
			recurrent = TypeOf model.getOutputLayer(0) Is RnnOutputLayer
			configuration = model.Configuration
			Me.labelNames = labelNames
			Me.gradientName = gradientName

			Me.inputFeatureIdx = 0
			Me.channelToNetworkInputMapper = channelToNetworkInputMapper
		End Sub

		''' <param name="model"> The <seealso cref="ComputationGraph"/> to use internally. </param>
		''' <param name="labelNames"> An array of the labels (in <seealso cref="FeaturesLabels"/>) to use as the network's input. </param>
		''' <param name="gradientName"> The name of the gradient (in <seealso cref="Gradients"/>) to use as the network's output. </param>
		''' <param name="inputFeatureIdx"> The channel index to use as the input of the model </param>
		Public Sub New(ByVal model As ComputationGraph, ByVal labelNames() As String, ByVal gradientName As String, ByVal inputFeatureIdx As Integer)
			Me.model = model
			recurrent = TypeOf model.getOutputLayer(0) Is RnnOutputLayer
			configuration = model.Configuration
			Me.labelNames = labelNames
			Me.gradientName = gradientName

			Me.inputFeatureIdx = inputFeatureIdx
			Me.channelToNetworkInputMapper = Nothing
		End Sub

		Public Overridable Sub notifyGradientCalculation() Implements INetworkHandler.notifyGradientCalculation
			Dim listeners As IEnumerable(Of TrainingListener) = model.getListeners()

			If listeners IsNot Nothing Then
				For Each l As TrainingListener In listeners
					l.onGradientCalculation(model)
				Next l
			End If
		End Sub

		Public Overridable Sub notifyIterationDone() Implements INetworkHandler.notifyIterationDone
			Dim modelCounters As BaseNetwork.ModelCounters = Me.ModelCounters
			Dim listeners As IEnumerable(Of TrainingListener) = model.getListeners()
			If listeners IsNot Nothing Then
				For Each l As TrainingListener In listeners
					l.iterationDone(model, modelCounters.getIterationCount(), modelCounters.getEpochCount())
				Next l
			End If
		End Sub

		Public Overridable Sub performFit(ByVal featuresLabels As FeaturesLabels) Implements INetworkHandler.performFit
			model.fit(buildInputs(featuresLabels.getFeatures()), buildLabels(featuresLabels))
		End Sub

		Public Overridable Sub performGradientsComputation(ByVal featuresLabels As FeaturesLabels) Implements INetworkHandler.performGradientsComputation
			model.Inputs = buildInputs(featuresLabels.getFeatures())
			model.Labels = buildLabels(featuresLabels)
			model.computeGradientAndScore()
		End Sub

		Public Overridable Sub fillGradientsResponse(ByVal gradients As Gradients) Implements INetworkHandler.fillGradientsResponse
			gradients.putGradient(gradientName, model.gradient())
		End Sub

		Private Function buildLabels(ByVal featuresLabels As FeaturesLabels) As INDArray()
			Dim numLabels As Integer = labelNames.Length
			Dim result(numLabels - 1) As INDArray
			For i As Integer = 0 To numLabels - 1
				result(i) = featuresLabels.getLabels(labelNames(i))
			Next i

			Return result
		End Function

		Private ReadOnly Property ModelCounters As BaseNetwork.ModelCounters
			Get
				Return New BaseNetwork.ModelCounters(configuration.getIterationCount(), configuration.getEpochCount())
			End Get
		End Property

		Public Overridable Sub applyGradient(ByVal gradients As Gradients, ByVal batchSize As Long) Implements INetworkHandler.applyGradient
			Dim modelCounters As BaseNetwork.ModelCounters = Me.ModelCounters
			Dim iterationCount As Integer = modelCounters.getIterationCount()
			Dim gradient As Gradient = gradients.getGradient(gradientName)
			model.Updater.update(gradient, iterationCount, modelCounters.getEpochCount(), CInt(batchSize), LayerWorkspaceMgr.noWorkspaces())
			model.params().subi(gradient.gradient())
			configuration.setIterationCount(iterationCount + 1)
		End Sub

		Public Overridable Function recurrentStepOutput(ByVal observation As Observation) As INDArray() Implements INetworkHandler.recurrentStepOutput
			Return model.rnnTimeStep(buildInputs(observation))
		End Function

		Public Overridable Function stepOutput(ByVal observation As Observation) As INDArray() Implements INetworkHandler.stepOutput
			Return model.output(buildInputs(observation))
		End Function

		Public Overridable Function batchOutput(ByVal features As Features) As INDArray() Implements INetworkHandler.batchOutput
			Return model.output(buildInputs(features))
		End Function

		Public Overridable Sub resetState() Implements INetworkHandler.resetState
			model.rnnClearPreviousState()
		End Sub

		Public Overridable Function clone() As INetworkHandler Implements INetworkHandler.clone
			If channelToNetworkInputMapper IsNot Nothing Then
				Return New ComputationGraphHandler(model.clone(), labelNames, gradientName, channelToNetworkInputMapper)
			End If
			Return New ComputationGraphHandler(model.clone(), labelNames, gradientName, inputFeatureIdx)
		End Function

		Public Overridable Sub copyFrom(ByVal from As INetworkHandler) Implements INetworkHandler.copyFrom
			model.Params = DirectCast(from, ComputationGraphHandler).model.params()
		End Sub


		Protected Friend Overridable Function buildInputs(ByVal observation As Observation) As INDArray()
			Return If(channelToNetworkInputMapper Is Nothing, New INDArray() { observation.getChannelData(inputFeatureIdx) }, channelToNetworkInputMapper.getNetworkInputs(observation))
		End Function

		Protected Friend Overridable Function buildInputs(ByVal features As Features) As INDArray()
			Return If(channelToNetworkInputMapper Is Nothing, New INDArray() { features.get(inputFeatureIdx) }, channelToNetworkInputMapper.getNetworkInputs(features))
		End Function

	End Class

End Namespace