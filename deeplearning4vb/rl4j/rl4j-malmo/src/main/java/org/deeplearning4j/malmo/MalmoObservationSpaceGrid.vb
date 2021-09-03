Imports System.Collections.Generic
Imports TimestampedStringVector = com.microsoft.msr.malmo.TimestampedStringVector
Imports WorldState = com.microsoft.msr.malmo.WorldState
Imports JSONArray = deeplearning4vb.org.json.JSONArray
Imports JSONObject = deeplearning4vb.org.json.JSONObject
Imports INDArray = deeplearning4vb.org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = deeplearning4vb.org.nd4j.linalg.factory.Nd4j

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


	Public Class MalmoObservationSpaceGrid
		Inherits MalmoObservationSpace

		Friend Const MAX_BLOCK As Integer = 4095

		Friend size As Integer

		Friend name_Conflict As String

		Friend totalSize As Integer

		Friend blockMap As New Dictionary(Of String, Integer)()

		''' <summary>
		''' Construct observation space from a array of blocks policy should distinguish between.
		''' </summary>
		''' <param name="name">   Name given to Grid element in mission specification </param>
		''' <param name="xSize">  total x size of grid </param>
		''' <param name="ySize">  total y size of grid </param>
		''' <param name="zSize">  total z size of grid </param>
		''' <param name="blocks"> Array of block names to distinguish between. Supports combination of individual strings and/or arrays of strings to map multiple block types to a single observation value. If not specified, it will dynamically map block names to integers - however, because these will be mapped as they are seen, different missions may have different mappings! </param>
		Public Sub New(ByVal name As String, ByVal xSize As Integer, ByVal ySize As Integer, ByVal zSize As Integer, ParamArray ByVal blocks() As Object)
			Me.name_Conflict = name

			Me.totalSize = xSize * ySize * zSize

			If blocks.Length = 0 Then
				Me.size = MAX_BLOCK
			Else
				Me.size = blocks.Length

				' Mapping is 1-based;  0 == all other types
				For i As Integer = 0 To blocks.Length - 1
					If TypeOf blocks(i) Is String Then
						blockMap(DirectCast(blocks(i), String)) = i + 1
					Else
						For Each block As String In DirectCast(blocks(i), String())
							blockMap(block) = i + 1
						Next block
					End If
				Next i
			End If
		End Sub

		Public Overrides ReadOnly Property Name As String
			Get
				Return "Box(" & totalSize & ")"
			End Get
		End Property

		Public Overrides ReadOnly Property Shape As Integer()
			Get
				Return New Integer(){totalSize}
			End Get
		End Property

		Public Overrides ReadOnly Property Low As INDArray
			Get
				Dim low As INDArray = nd4j.create(Shape)
				Return low
			End Get
		End Property

		Public Overrides ReadOnly Property High As INDArray
			Get
				Dim high As INDArray = Nd4j.linspace(255, 255, totalSize).reshape(Shape)
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
			Dim blocks As JSONArray = observation.getJSONArray(name_Conflict)

			Dim blockTypes(totalSize - 1) As Double

			For i As Integer = 0 To totalSize - 1
				Dim block As String = blocks.getString(i)
				Dim mapped As Integer? = blockMap(block)

				If size = MAX_BLOCK AndAlso mapped Is Nothing Then
					mapped = blockMap.Count
					blockMap(block) = mapped
				End If

				blockTypes(i) = If(mapped Is Nothing, 0, mapped)
			Next i

			Return New MalmoBox(blockTypes)
		End Function
	End Class

End Namespace