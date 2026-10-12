Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports HashedMap = org.apache.commons.collections4.map.HashedMap
Imports Features = org.deeplearning4j.rl4j.agent.learning.update.Features
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
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


	Public Class ChannelToNetworkInputMapper
		Private ReadOnly networkInputsToChannelNameMap() As IdxBinding
		Private ReadOnly inputCount As Integer

		''' <param name="networkInputsToChannelNameMap"> An array that describe how to map the network inputs with the channel names. </param>
		''' <param name="networkInputNames"> An ordered array of the network inputs. </param>
		''' <param name="channelNames"> An ordered array of the observation/features channel names </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ChannelToNetworkInputMapper(@NonNull NetworkInputToChannelBinding[] networkInputsToChannelNameMap, String[] networkInputNames, String[] channelNames)
		Public Sub New(ByVal networkInputsToChannelNameMap() As NetworkInputToChannelBinding, ByVal networkInputNames() As String, ByVal channelNames() As String)
			Preconditions.checkArgument(networkInputsToChannelNameMap.Length > 0, "networkInputsToChannelNameMap is empty.")
			Preconditions.checkArgument(networkInputNames.Length > 0, "networkInputNames is empty.")
			Preconditions.checkArgument(channelNames.Length > 0, "channelNames is empty.")

			' All network inputs must be mapped exactly once.
			For Each inputName As String In networkInputNames
				Dim numTimesMapped As Integer = 0
'JAVA TO VB CONVERTER NOTE: The variable networkInputToChannelBinding was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
				For Each networkInputToChannelBinding_Conflict As NetworkInputToChannelBinding In networkInputsToChannelNameMap
					numTimesMapped += If(String.ReferenceEquals(inputName, networkInputToChannelBinding_Conflict.networkInputName), 1, 0)
				Next networkInputToChannelBinding_Conflict

				If numTimesMapped <> 1 Then
					Throw New System.ArgumentException("All network inputs must be mapped exactly once. Input '" & inputName & "' is mapped " & numTimesMapped & " times.")
				End If
			Next inputName

			Dim networkNameToIdx As IDictionary(Of String, Integer) = New HashedMap(Of String, Integer)()
			For i As Integer = 0 To networkInputNames.Length - 1
				networkNameToIdx(networkInputNames(i)) = i
			Next i

			Dim channelNamesToIdx As IDictionary(Of String, Integer) = New HashedMap(Of String, Integer)()
			For i As Integer = 0 To channelNames.Length - 1
				channelNamesToIdx(channelNames(i)) = i
			Next i

			Me.networkInputsToChannelNameMap = New IdxBinding(networkInputNames.Length - 1){}
			For i As Integer = 0 To networkInputsToChannelNameMap.Length - 1
				Dim nameMap As NetworkInputToChannelBinding = networkInputsToChannelNameMap(i)

				Dim networkIdx As Integer? = networkNameToIdx(nameMap.networkInputName)
				If networkIdx Is Nothing Then
					Throw New System.ArgumentException("'" & nameMap.networkInputName & "' not found in networkInputNames")
				End If

				Dim channelIdx As Integer? = channelNamesToIdx(nameMap.channelName)
				If channelIdx Is Nothing Then
					Throw New System.ArgumentException("'" & nameMap.channelName & "' not found in channelNames")
				End If

				Me.networkInputsToChannelNameMap(i) = New IdxBinding(networkIdx, channelIdx)
			Next i

			inputCount = networkInputNames.Length
		End Sub

		Public Overridable Function getNetworkInputs(ByVal observation As Observation) As INDArray()
			Dim result(inputCount - 1) As INDArray
			For Each map As IdxBinding In networkInputsToChannelNameMap
				result(map.networkInputIdx) = observation.getChannelData(map.channelIdx)
			Next map

			Return result
		End Function

		Public Overridable Function getNetworkInputs(ByVal features As Features) As INDArray()
			Dim result(inputCount - 1) As INDArray
			For Each map As IdxBinding In networkInputsToChannelNameMap
				result(map.networkInputIdx) = features.get(map.channelIdx)
			Next map

			Return result
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public static class NetworkInputToChannelBinding
		Public Class NetworkInputToChannelBinding
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private String networkInputName;
			Friend networkInputName As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private String channelName;
			Friend channelName As String

			Public Shared Function map(ByVal networkInputName As String, ByVal channelName As String) As NetworkInputToChannelBinding
				Return New NetworkInputToChannelBinding(networkInputName, channelName)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor private static class IdxBinding
		Private Class IdxBinding
			Friend networkInputIdx As Integer
			Friend channelIdx As Integer
		End Class

	End Class

End Namespace