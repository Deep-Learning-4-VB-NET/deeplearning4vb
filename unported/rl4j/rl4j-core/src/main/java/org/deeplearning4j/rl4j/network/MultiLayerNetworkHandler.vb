Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports RnnOutputLayer = org.deeplearning4j.nn.layers.recurrent.RnnOutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
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

	''' <summary>
	''' A <seealso cref="INetworkHandler"/> implementation to be used with <seealso cref="MultiLayerNetwork MultiLayerNetworks"/>
	''' </summary>
	Public Class MultiLayerNetworkHandler
		Implements INetworkHandler

		Private ReadOnly model As MultiLayerNetwork

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final boolean recurrent;
		Private ReadOnly recurrent As Boolean
		Private ReadOnly configuration As MultiLayerConfiguration
		Private ReadOnly labelName As String
		Private ReadOnly gradientName As String
		Private ReadOnly inputFeatureIdx As Integer

		''' 
		''' <param name="model"> The <seealso cref="MultiLayerNetwork"/> to use internally </param>
		''' <param name="labelName"> The name of the label (in <seealso cref="FeaturesLabels"/>) to use as the network's input. </param>
		''' <param name="gradientName"> The name of the gradient (in <seealso cref="Gradients"/>) to use as the network's output. </param>
		''' <param name="inputFeatureIdx"> The channel index to use as the input of the model </param>
		Public Sub New(ByVal model As MultiLayerNetwork, ByVal labelName As String, ByVal gradientName As String, ByVal inputFeatureIdx As Integer)
			Me.model = model
			recurrent = TypeOf model.OutputLayer Is RnnOutputLayer
			configuration = model.LayerWiseConfigurations
			Me.labelName = labelName
			Me.gradientName = gradientName
			Me.inputFeatureIdx = inputFeatureIdx
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
			Dim features As INDArray = featuresLabels.getFeatures().get(inputFeatureIdx)
			Dim labels As INDArray = featuresLabels.getLabels(labelName)
			model.fit(features, labels)
		End Sub

		Public Overridable Sub performGradientsComputation(ByVal featuresLabels As FeaturesLabels) Implements INetworkHandler.performGradientsComputation
			model.Input = featuresLabels.getFeatures().get(inputFeatureIdx)
			model.Labels = featuresLabels.getLabels(labelName)
			model.computeGradientAndScore()
		End Sub

		Private ReadOnly Property ModelCounters As BaseNetwork.ModelCounters
			Get
				Return New BaseNetwork.ModelCounters(configuration.getIterationCount(), configuration.EpochCount)
			End Get
		End Property

		Public Overridable Sub applyGradient(ByVal gradients As Gradients, ByVal batchSize As Long) Implements INetworkHandler.applyGradient
			Dim modelCounters As BaseNetwork.ModelCounters = Me.ModelCounters
			Dim iterationCount As Integer = modelCounters.getIterationCount()
			Dim gradient As Gradient = gradients.getGradient(gradientName)
			model.Updater.update(model, gradient, iterationCount, modelCounters.getEpochCount(), CInt(batchSize), LayerWorkspaceMgr.noWorkspaces())
			model.params().subi(gradient.gradient())
			configuration.setIterationCount(iterationCount + 1)
		End Sub

		Public Overridable Function recurrentStepOutput(ByVal observation As Observation) As INDArray() Implements INetworkHandler.recurrentStepOutput
			Return New INDArray() { model.rnnTimeStep(observation.getChannelData(inputFeatureIdx)) }
		End Function

		Public Overridable Function batchOutput(ByVal features As Features) As INDArray() Implements INetworkHandler.batchOutput
			Return New INDArray() { model.output(features.get(inputFeatureIdx)) }
		End Function

		Public Overridable Function stepOutput(ByVal observation As Observation) As INDArray() Implements INetworkHandler.stepOutput
			Return New INDArray() { model.output(observation.getChannelData(inputFeatureIdx)) }
		End Function

		Public Overridable Sub resetState() Implements INetworkHandler.resetState
			model.rnnClearPreviousState()
		End Sub

		Public Overridable Function clone() As INetworkHandler Implements INetworkHandler.clone
			Return New MultiLayerNetworkHandler(model.clone(), labelName, gradientName, inputFeatureIdx)
		End Function

		Public Overridable Sub copyFrom(ByVal from As INetworkHandler) Implements INetworkHandler.copyFrom
			model.Params = DirectCast(from, MultiLayerNetworkHandler).model.params()
		End Sub

		Public Overridable Sub fillGradientsResponse(ByVal gradients As Gradients) Implements INetworkHandler.fillGradientsResponse
			gradients.putGradient(gradientName, model.gradient())
		End Sub

	End Class

End Namespace