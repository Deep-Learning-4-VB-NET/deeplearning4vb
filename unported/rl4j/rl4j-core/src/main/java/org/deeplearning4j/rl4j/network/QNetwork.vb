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

	''' <summary>
	''' A QNetwork implementation.<br/>
	''' Label names: "Q"<br/>
	''' Gradient names: "Q"<br/>
	''' </summary>
	Public Class QNetwork
		Inherits BaseNetwork(Of QNetwork)

		Private Shared ReadOnly LABEL_NAMES() As String = { CommonLabelNames.QValues }

		Private Sub New(ByVal handler As INetworkHandler)
			MyBase.New(handler)
		End Sub

		Protected Friend Overrides Function packageResult(ByVal output() As INDArray) As NeuralNetOutput
			Dim result As New NeuralNetOutput()
			result.put(CommonOutputNames.QValues, output(0))

			Return result
		End Function

		Public Overrides Function clone() As QNetwork
			Return New QNetwork(getNetworkHandler().clone())
		End Function

		Public Shared Function builder() As Builder
			Return New Builder()
		End Function

		Public Class Builder
			Friend ReadOnly networkHelper As New NetworkHelper()

			Friend networkInputsToFeatureBindings() As ChannelToNetworkInputMapper.NetworkInputToChannelBinding
'JAVA TO VB CONVERTER NOTE: The field channelNames was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend channelNames_Conflict() As String
			Friend inputChannelName As String

			Friend cgNetwork As ComputationGraph
			Friend mlnNetwork As MultiLayerNetwork

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder withNetwork(@NonNull ComputationGraph network)
			Public Overridable Function withNetwork(ByVal network As ComputationGraph) As Builder
				Me.cgNetwork = network
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder withNetwork(@NonNull MultiLayerNetwork network)
			Public Overridable Function withNetwork(ByVal network As MultiLayerNetwork) As Builder
				Me.mlnNetwork = network
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

			Public Overridable Function build() As QNetwork
				Dim networkHandler As INetworkHandler

				Preconditions.checkState(cgNetwork IsNot Nothing OrElse mlnNetwork IsNot Nothing, "A network must be set.")

				If cgNetwork IsNot Nothing Then
					networkHandler = If(networkInputsToFeatureBindings Is Nothing, networkHelper.buildHandler(cgNetwork, inputChannelName, channelNames_Conflict, LABEL_NAMES, CommonGradientNames.QValues), networkHelper.buildHandler(cgNetwork, networkInputsToFeatureBindings, channelNames_Conflict, LABEL_NAMES, CommonGradientNames.QValues))
				Else
					networkHandler = networkHelper.buildHandler(mlnNetwork, inputChannelName, channelNames_Conflict, CommonLabelNames.QValues, CommonGradientNames.QValues)
				End If

				Return New QNetwork(networkHandler)
			End Function
		End Class

	End Class
End Namespace