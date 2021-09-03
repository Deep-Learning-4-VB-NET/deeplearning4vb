Imports System.Collections.Generic
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ByteVector = com.microsoft.msr.malmo.ByteVector
Imports TimestampedVideoFrameVector = com.microsoft.msr.malmo.TimestampedVideoFrameVector
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


	Public Class MalmoObservationSpacePixels
		Inherits MalmoObservationSpace

		Friend xSize As Integer
		Friend ySize As Integer

		Friend blockMap As New Dictionary(Of String, Integer)()

		''' <summary>
		''' Construct observation space from a bitmap size. Assumes 3 color channels.
		''' </summary>
		''' <param name="xSize"> total x size of bitmap </param>
		''' <param name="ySize"> total y size of bitmap </param>
		Public Sub New(ByVal xSize As Integer, ByVal ySize As Integer)
			Me.xSize = xSize
			Me.ySize = ySize
		End Sub

		Public Overrides ReadOnly Property Name As String
			Get
				Return "Box(" & ySize & "," & xSize & ",3)"
			End Get
		End Property

		Public Overrides ReadOnly Property Shape As Integer()
			Get
				Return New Integer() {ySize, xSize, 3}
			End Get
		End Property

		Public Overrides ReadOnly Property Low As INDArray
			Get
				Dim low As INDArray = Nd4j.create(Shape)
				Return low
			End Get
		End Property

		Public Overrides ReadOnly Property High As INDArray
			Get
				Dim high As INDArray = Nd4j.linspace(255, 255, xSize * ySize * 3).reshape(Shape)
				Return high
			End Get
		End Property

		Public Overrides Function getObservation(ByVal world_state As WorldState) As MalmoBox
			Dim observations As TimestampedVideoFrameVector = world_state.getVideoFrames()

			Dim rawPixels((xSize * ySize * 3) - 1) As Double

			If Not observations.isEmpty() Then
				Dim pixels As ByteVector = observations.get(CInt(observations.size() - 1)).getPixels()

				Dim i As Integer = 0
				For x As Integer = 0 To xSize - 1
					For y As Integer = 0 To ySize - 1
						For c As Integer = 2 To 0 Step -1 ' BGR -> RGB
							rawPixels(i) = pixels.get(3 * x * ySize + y * 3 + c) / 255.0
							i += 1
						Next c
					Next y
				Next x
			End If

			Return New MalmoBox(rawPixels)
		End Function
	End Class

End Namespace