Imports System.Collections.Generic
Imports Getter = lombok.Getter
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


	Public Class CompoundNetworkHandler
		Implements INetworkHandler

		Private ReadOnly networkHandlers() As INetworkHandler
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean recurrent;
		Private recurrent As Boolean

		''' <param name="networkHandlers"> All networks to be used in this instance. </param>
		Public Sub New(ParamArray ByVal networkHandlers() As INetworkHandler)
			Me.networkHandlers = networkHandlers

			For Each handler As INetworkHandler In networkHandlers
				recurrent = recurrent Or handler.Recurrent
			Next handler
		End Sub

		Public Overridable Sub notifyGradientCalculation() Implements INetworkHandler.notifyGradientCalculation
			For Each handler As INetworkHandler In networkHandlers
				handler.notifyGradientCalculation()
			Next handler
		End Sub

		Public Overridable Sub notifyIterationDone() Implements INetworkHandler.notifyIterationDone
			For Each handler As INetworkHandler In networkHandlers
				handler.notifyIterationDone()
			Next handler
		End Sub

		Public Overridable Sub performFit(ByVal featuresLabels As FeaturesLabels) Implements INetworkHandler.performFit
			For Each handler As INetworkHandler In networkHandlers
				handler.performFit(featuresLabels)
			Next handler
		End Sub

		Public Overridable Sub performGradientsComputation(ByVal featuresLabels As FeaturesLabels) Implements INetworkHandler.performGradientsComputation
			For Each handler As INetworkHandler In networkHandlers
				handler.performGradientsComputation(featuresLabels)
			Next handler
		End Sub

		Public Overridable Sub fillGradientsResponse(ByVal gradients As Gradients) Implements INetworkHandler.fillGradientsResponse
			For Each handler As INetworkHandler In networkHandlers
				handler.fillGradientsResponse(gradients)
			Next handler
		End Sub

		Public Overridable Sub applyGradient(ByVal gradients As Gradients, ByVal batchSize As Long) Implements INetworkHandler.applyGradient
			For Each handler As INetworkHandler In networkHandlers
				handler.applyGradient(gradients, batchSize)
			Next handler
		End Sub

		Public Overridable Function recurrentStepOutput(ByVal observation As Observation) As INDArray() Implements INetworkHandler.recurrentStepOutput
			Dim outputs As IList(Of INDArray) = New List(Of INDArray)()
			For Each handler As INetworkHandler In networkHandlers
				Collections.addAll(outputs, handler.recurrentStepOutput(observation))
			Next handler

			Return CType(outputs, List(Of INDArray)).ToArray()
		End Function

		Public Overridable Function stepOutput(ByVal observation As Observation) As INDArray() Implements INetworkHandler.stepOutput
			Dim outputs As IList(Of INDArray) = New List(Of INDArray)()
			For Each handler As INetworkHandler In networkHandlers
				Collections.addAll(outputs, handler.stepOutput(observation))
			Next handler

			Return CType(outputs, List(Of INDArray)).ToArray()
		End Function

		Public Overridable Function batchOutput(ByVal features As Features) As INDArray() Implements INetworkHandler.batchOutput
			Dim outputs As IList(Of INDArray) = New List(Of INDArray)()
			For Each handler As INetworkHandler In networkHandlers
				Collections.addAll(outputs, handler.batchOutput(features))
			Next handler

			Return CType(outputs, List(Of INDArray)).ToArray()
		End Function

		Public Overridable Sub resetState() Implements INetworkHandler.resetState
			For Each handler As INetworkHandler In networkHandlers
				If handler.Recurrent Then
					handler.resetState()
				End If
			Next handler
		End Sub

		Public Overridable Function clone() As INetworkHandler Implements INetworkHandler.clone
			Dim clonedHandlers(networkHandlers.Length - 1) As INetworkHandler
			For i As Integer = 0 To networkHandlers.Length - 1
				clonedHandlers(i) = networkHandlers(i).clone()
			Next i

			Return New CompoundNetworkHandler(clonedHandlers)
		End Function

		Public Overridable Sub copyFrom(ByVal from As INetworkHandler) Implements INetworkHandler.copyFrom
			For i As Integer = 0 To networkHandlers.Length - 1
				networkHandlers(i).copyFrom(DirectCast(from, CompoundNetworkHandler).networkHandlers(i))
			Next i
		End Sub
	End Class

End Namespace