Imports System.Collections.Generic
Imports AccessLevel = lombok.AccessLevel
Imports Getter = lombok.Getter
Imports Value = lombok.Value
Imports NotImplementedException = org.apache.commons.lang3.NotImplementedException
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


	Public MustInherit Class BaseNetwork(Of NET_TYPE As BaseNetwork)
		Implements ITrainableNeuralNet(Of NET_TYPE)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(lombok.AccessLevel.@PROTECTED) private final INetworkHandler networkHandler;
		Private ReadOnly networkHandler As INetworkHandler

		Private ReadOnly neuralNetOutputCache As IDictionary(Of Observation, NeuralNetOutput) = New Dictionary(Of Observation, NeuralNetOutput)()

		Protected Friend Sub New(ByVal networkHandler As INetworkHandler)
			Me.networkHandler = networkHandler
		End Sub

		''' <returns> True if the network is recurrent. </returns>
		Public Overridable ReadOnly Property Recurrent As Boolean
			Get
				Return networkHandler.Recurrent
			End Get
		End Property

		''' <summary>
		''' Fit the network using the featuresLabels </summary>
		''' <param name="featuresLabels"> The feature-labels </param>
		Public Overridable Sub fit(ByVal featuresLabels As FeaturesLabels) Implements ITrainableNeuralNet(Of NET_TYPE).fit
			invalidateCache()
			networkHandler.performFit(featuresLabels)
		End Sub

		''' <summary>
		''' Compute the gradients from the featuresLabels </summary>
		''' <param name="featuresLabels"> The feature-labels </param>
		''' <returns> A <seealso cref="Gradients"/> instance </returns>
		Public Overridable Function computeGradients(ByVal featuresLabels As FeaturesLabels) As Gradients Implements ITrainableNeuralNet(Of NET_TYPE).computeGradients
			networkHandler.performGradientsComputation(featuresLabels)
			networkHandler.notifyGradientCalculation()
			Dim results As New Gradients(featuresLabels.BatchSize)
			networkHandler.fillGradientsResponse(results)

			Return results
		End Function

		''' <summary>
		''' Applies the <seealso cref="Gradients"/> </summary>
		''' <param name="gradients"> the gradients to be applied </param>
		Public Overridable Sub applyGradients(ByVal gradients As Gradients) Implements ITrainableNeuralNet(Of NET_TYPE).applyGradients
			invalidateCache()
			networkHandler.applyGradient(gradients, gradients.getBatchSize())
			networkHandler.notifyIterationDone()
		End Sub

		''' <summary>
		''' Computes the output from an observation or get the previously computed one if found in the cache. </summary>
		''' <param name="observation"> An <seealso cref="Observation"/> </param>
		''' <returns> a <seealso cref="NeuralNetOutput"/> instance </returns>
		Public Overrides Function output(ByVal observation As Observation) As NeuralNetOutput
			Dim result As NeuralNetOutput = neuralNetOutputCache(observation)
			If result Is Nothing Then
				If Recurrent Then
					result = packageResult(networkHandler.recurrentStepOutput(observation))
				Else
					result = packageResult(networkHandler.stepOutput(observation))
				End If

				neuralNetOutputCache(observation) = result
			End If

			Return result
		End Function

		Protected Friend MustOverride Function packageResult(ByVal output() As INDArray) As NeuralNetOutput

		''' <summary>
		''' Compute the output for a batch.
		''' Note: The current state is ignored if used with a recurrent network </summary>
		''' <param name="batch"> </param>
		''' <returns> a <seealso cref="NeuralNetOutput"/> instance </returns>
		Public Overridable Function output(ByVal batch As INDArray) As NeuralNetOutput
			' TODO: Remove when legacy code is gone
			Throw New NotImplementedException("output(INDArray): should use output(Observation) or output(Features)")
		End Function

		''' <summary>
		''' Compute the output for a batch.
		''' Note: The current state is ignored if used with a recurrent network </summary>
		''' <param name="features"> </param>
		''' <returns> a <seealso cref="NeuralNetOutput"/> instance </returns>
		Public Overridable Function output(ByVal features As Features) As NeuralNetOutput
			Return packageResult(networkHandler.batchOutput(features))
		End Function


		''' <summary>
		''' Resets the cache and the state of the network
		''' </summary>
		Public Overrides Sub reset()
			invalidateCache()
			If Recurrent Then
				networkHandler.resetState()
			End If
		End Sub

		Protected Friend Overridable Sub invalidateCache()
			neuralNetOutputCache.Clear()
		End Sub

		''' <summary>
		''' Copy the network parameters from the argument to the current network and clear the cache </summary>
		''' <param name="from"> The network that will be the source of the copy. </param>
		Public Overridable Sub copyFrom(ByVal from As BaseNetwork)
			reset()
			networkHandler.copyFrom(from.networkHandler)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Value protected static class ModelCounters
		Protected Friend Class ModelCounters
			Friend iterationCount As Integer
			Friend epochCount As Integer
		End Class

		Public MustOverride Function clone() As NET_TYPE Implements ITrainableNeuralNet(Of NET_TYPE).clone
	End Class

End Namespace