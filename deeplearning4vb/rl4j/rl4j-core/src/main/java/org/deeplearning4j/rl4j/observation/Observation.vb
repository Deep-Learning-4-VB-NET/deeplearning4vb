Imports Getter = lombok.Getter
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
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

Namespace org.deeplearning4j.rl4j.observation

	''' <summary>
	''' Represent an observation from the environment
	''' 
	''' @author Alexandre Boulanger
	''' </summary>
	Public Class Observation
		Implements Encodable

		''' <summary>
		''' A singleton representing a skipped observation
		''' </summary>
		Public Shared SkippedObservation As New Observation()

		''' <returns> A INDArray containing the data of the observation </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final org.nd4j.linalg.api.ndarray.INDArray[] channelsData;
		Private ReadOnly channelsData() As INDArray

		Public Overridable Function getChannelData(ByVal channelIdx As Integer) As INDArray
			Return channelsData(channelIdx)
		End Function

		' TODO: Remove once Encodable is removed
		Public Overridable Function toArray() As Double() Implements Encodable.toArray
			Return channelsData(0).data().asDouble()
		End Function

		Public Overridable ReadOnly Property Skipped As Boolean Implements Encodable.isSkipped
			Get
				Return channelsData Is Nothing
			End Get
		End Property

		Private Sub New()
			Me.channelsData = Nothing
		End Sub

		' TODO: Remove when legacy code is gone
		Public Sub New(ByVal data As INDArray)
			Me.channelsData = New INDArray() { data }
		End Sub

		Public Sub New(ByVal channelsData() As INDArray)
			Me.channelsData = channelsData
		End Sub

		' TODO: Remove when legacy code is gone
		Public Overridable ReadOnly Property Data As INDArray Implements Encodable.getData
			Get
				Return channelsData(0)
			End Get
		End Property

		Public Overridable Function numChannels() As Integer
			Return channelsData.Length
		End Function

		''' <summary>
		''' Creates a duplicate instance of the current observation
		''' @return
		''' </summary>
		Public Overridable Function dup() As Observation
			If channelsData Is Nothing Then
				Return SkippedObservation
			End If

			Dim duplicated(channelsData.Length - 1) As INDArray
			For i As Integer = 0 To channelsData.Length - 1
				duplicated(i) = channelsData(i).dup()
			Next i
			Return New Observation(duplicated)
		End Function
	End Class

End Namespace