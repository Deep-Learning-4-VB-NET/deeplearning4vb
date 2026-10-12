Imports JSONObject = org.json.JSONObject
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports TimestampedStringVector = com.microsoft.msr.malmo.TimestampedStringVector
Imports WorldState = com.microsoft.msr.malmo.WorldState

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

Namespace org.deeplearning4j.malmo

	Public Class MalmoObservationSpacePosition
		Inherits MalmoObservationSpace

		Public Overrides ReadOnly Property Name As String
			Get
				Return "Box(5,)"
			End Get
		End Property

		Public Overrides ReadOnly Property Shape As Integer()
			Get
				Return New Integer() {5}
			End Get
		End Property

		Public Overrides ReadOnly Property Low As INDArray
			Get
				Dim low As INDArray = Nd4j.create(New Single() {Integer.MinValue, Integer.MinValue, Integer.MinValue})
				Return low
			End Get
		End Property

		Public Overrides ReadOnly Property High As INDArray
			Get
				Dim high As INDArray = Nd4j.create(New Single() {Integer.MaxValue, Integer.MaxValue, Integer.MaxValue})
				Return high
			End Get
		End Property

		Public Overrides Function getObservation(ByVal world_state As WorldState) As MalmoBox
			Dim observations As TimestampedStringVector = world_state.getObservations()

			If observations.isEmpty() Then
				Return Nothing
			End If

			Dim obs_text As String = observations.get(CInt(observations.size() - 1)).getText()

			Dim observation As New JSONObject(obs_text)

			Dim xpos As Double = observation.getDouble("XPos")
			Dim ypos As Double = observation.getDouble("YPos")
			Dim zpos As Double = observation.getDouble("ZPos")
			Dim yaw As Double = observation.getDouble("Yaw")
			Dim pitch As Double = observation.getDouble("Pitch")

			Return New MalmoBox(xpos, ypos, zpos, yaw, pitch)
		End Function
	End Class

End Namespace