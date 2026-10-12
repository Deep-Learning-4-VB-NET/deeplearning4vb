Imports NonNull = lombok.NonNull
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Preconditions = org.nd4j.common.base.Preconditions
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

	Public Class ActorCriticNetwork
		Inherits BaseNetwork(Of ActorCriticNetwork)

		Private Shared ReadOnly LABEL_NAMES() As String = { CommonLabelNames.ActorCritic.Value, CommonLabelNames.ActorCritic.Policy }
		Private ReadOnly isCombined As Boolean

		Private Sub New(ByVal handler As INetworkHandler, ByVal isCombined As Boolean)
			MyBase.New(handler)
			Me.isCombined = isCombined
		End Sub

		Protected Friend Overrides Function packageResult(ByVal output() As INDArray) As NeuralNetOutput
			Dim result As New NeuralNetOutput()
			result.put(CommonOutputNames.ActorCritic.Value, output(0))
			result.put(CommonOutputNames.ActorCritic.Policy, output(1))

			Return result
		End Function

		Public Overrides Function clone() As ActorCriticNetwork
			Return New ActorCriticNetwork(getNetworkHandler().clone(), isCombined)
		End Function

		Public Shared Function builder() As Builder
			Return New Builder()
		End Function

		Public Class Builder
			Friend ReadOnly networkHelper As New NetworkHelper()

			Friend isCombined As Boolean
			Friend combinedNetwork As ComputationGraph

			Friend cgValueNetwork As ComputationGraph
			Friend mlnValueNetwork As MultiLayerNetwork

			Friend cgPolicyNetwork As ComputationGraph
			Friend mlnPolicyNetwork As MultiLayerNetwork
			Friend inputChannelName As String
'JAVA TO VB CONVERTER NOTE: The field channelNames was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend channelNames_Conflict() As String
			Friend networkInputsToFeatureBindings() As ChannelToNetworkInputMapper.NetworkInputToChannelBinding


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder withCombinedNetwork(@NonNull ComputationGraph combinedNetwork)
			Public Overridable Function withCombinedNetwork(ByVal combinedNetwork As ComputationGraph) As Builder
				isCombined = True
				Me.combinedNetwork = combinedNetwork

				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder withSeparateNetworks(@NonNull ComputationGraph valueNetwork, @NonNull ComputationGraph policyNetwork)
			Public Overridable Function withSeparateNetworks(ByVal valueNetwork As ComputationGraph, ByVal policyNetwork As ComputationGraph) As Builder
				Me.cgValueNetwork = valueNetwork
				Me.cgPolicyNetwork = policyNetwork
				isCombined = False
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder withSeparateNetworks(@NonNull MultiLayerNetwork valueNetwork, @NonNull ComputationGraph policyNetwork)
			Public Overridable Function withSeparateNetworks(ByVal valueNetwork As MultiLayerNetwork, ByVal policyNetwork As ComputationGraph) As Builder
				Me.mlnValueNetwork = valueNetwork
				Me.cgPolicyNetwork = policyNetwork
				isCombined = False

				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder withSeparateNetworks(@NonNull ComputationGraph valueNetwork, @NonNull MultiLayerNetwork policyNetwork)
			Public Overridable Function withSeparateNetworks(ByVal valueNetwork As ComputationGraph, ByVal policyNetwork As MultiLayerNetwork) As Builder
				Me.cgValueNetwork = valueNetwork
				Me.mlnPolicyNetwork = policyNetwork
				isCombined = False

				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder withSeparateNetworks(@NonNull MultiLayerNetwork valueNetwork, @NonNull MultiLayerNetwork policyNetwork)
			Public Overridable Function withSeparateNetworks(ByVal valueNetwork As MultiLayerNetwork, ByVal policyNetwork As MultiLayerNetwork) As Builder
				Me.mlnValueNetwork = valueNetwork
				Me.mlnPolicyNetwork = policyNetwork
				isCombined = False

				Return Me
			End Function

			Public Overridable Function inputBindings(ByVal networkInputsToFeatureBindings() As ChannelToNetworkInputMapper.NetworkInputToChannelBinding) As Builder
				Me.networkInputsToFeatureBindings = networkInputsToFeatureBindings
				Return Me
			End Function

			Public Overridable Function specificBinding(ByVal inputChannelName As String) As Builder
				Me.inputChannelName = inputChannelName
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter channelNames was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function channelNames(ByVal channelNames_Conflict() As String) As Builder
				Me.channelNames_Conflict = channelNames_Conflict
				Return Me
			End Function

			Public Overridable Function build() As ActorCriticNetwork
				Dim networkHandler As INetworkHandler

				Dim isValueNetworkSet As Boolean = Not (cgValueNetwork Is Nothing AndAlso mlnValueNetwork Is Nothing)
				Dim isPolicyNetworkSet As Boolean = Not (cgPolicyNetwork Is Nothing AndAlso mlnPolicyNetwork Is Nothing)
				Preconditions.checkState(combinedNetwork IsNot Nothing OrElse (isValueNetworkSet AndAlso isPolicyNetworkSet), "A network must be set.")

				If isCombined Then
					networkHandler = If(networkInputsToFeatureBindings Is Nothing, networkHelper.buildHandler(combinedNetwork, inputChannelName, channelNames_Conflict, LABEL_NAMES, CommonGradientNames.ActorCritic.Combined), networkHelper.buildHandler(combinedNetwork, networkInputsToFeatureBindings, channelNames_Conflict, LABEL_NAMES, CommonGradientNames.ActorCritic.Combined))
				Else
					Dim valueNetworkHandler As INetworkHandler
					If cgValueNetwork IsNot Nothing Then
						valueNetworkHandler = If(networkInputsToFeatureBindings Is Nothing, networkHelper.buildHandler(cgValueNetwork, inputChannelName, channelNames_Conflict, New String() { CommonLabelNames.ActorCritic.Value }, CommonGradientNames.ActorCritic.Value), networkHelper.buildHandler(cgValueNetwork, networkInputsToFeatureBindings, channelNames_Conflict, LABEL_NAMES, CommonGradientNames.ActorCritic.Value))
					Else
						valueNetworkHandler = networkHelper.buildHandler(mlnValueNetwork, inputChannelName, channelNames_Conflict, CommonLabelNames.ActorCritic.Value, CommonGradientNames.ActorCritic.Value)
					End If

					Dim policyNetworkHandler As INetworkHandler
					If cgPolicyNetwork IsNot Nothing Then
						policyNetworkHandler = If(networkInputsToFeatureBindings Is Nothing, networkHelper.buildHandler(cgPolicyNetwork, inputChannelName, channelNames_Conflict, New String() { CommonLabelNames.ActorCritic.Policy }, CommonGradientNames.ActorCritic.Policy), networkHelper.buildHandler(cgPolicyNetwork, networkInputsToFeatureBindings, channelNames_Conflict, LABEL_NAMES, CommonGradientNames.ActorCritic.Policy))
					Else
						policyNetworkHandler = networkHelper.buildHandler(mlnPolicyNetwork, inputChannelName, channelNames_Conflict, CommonLabelNames.ActorCritic.Policy, CommonGradientNames.ActorCritic.Policy)
					End If

					networkHandler = New CompoundNetworkHandler(valueNetworkHandler, policyNetworkHandler)
				End If

				Return New ActorCriticNetwork(networkHandler, isCombined)
			End Function

		End Class

	End Class

End Namespace