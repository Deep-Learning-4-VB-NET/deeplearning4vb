Imports System
Imports System.Collections.Generic
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports Trainable = org.deeplearning4j.nn.api.Trainable
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports GraphVertex = org.deeplearning4j.nn.graph.vertex.GraphVertex
Imports org.deeplearning4j.nn.updater
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

Namespace org.deeplearning4j.nn.updater.graph


	<Serializable>
	Public Class ComputationGraphUpdater
		Inherits BaseMultiLayerUpdater(Of ComputationGraph)

'JAVA TO VB CONVERTER NOTE: The field orderedLayers was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend orderedLayers_Conflict() As Trainable

		Public Sub New(ByVal graph As ComputationGraph)
			Me.New(graph, Nothing)
		End Sub

		Public Sub New(ByVal graph As ComputationGraph, ByVal updaterState As INDArray)
			MyBase.New(graph, updaterState)

			layersByName = New Dictionary(Of String, Trainable)()
			Dim layers() As Trainable = OrderedLayers
			For Each l As Trainable In layers
				layersByName(l.Config.LayerName) = l
			Next l
		End Sub

		Protected Friend Overrides ReadOnly Property OrderedLayers As Trainable()
			Get
				If orderedLayers_Conflict IsNot Nothing Then
					Return orderedLayers_Conflict
				End If
				Dim vertices() As GraphVertex = network.getVertices()
    
				'In CompGraph: we need to know topological ordering, so we know how parameters are laid out in the 1d view arrays
				Dim topologicalOrdering() As Integer = network.topologicalSortOrder()
    
				Dim [out]((network.getVertices().length) - 1) As Trainable
    
				Dim j As Integer = 0
				For i As Integer = 0 To topologicalOrdering.Length - 1
					Dim currentVertex As GraphVertex = vertices(topologicalOrdering(i))
					If currentVertex.numParams() = 0 Then
						Continue For
					End If
    
	'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
	'ORIGINAL LINE: out[j++] = currentVertex;
					[out](j) = currentVertex
						j += 1
				Next i
				If j <> [out].Length Then
					[out] = Arrays.CopyOfRange([out], 0, j)
				End If
    
				orderedLayers_Conflict = [out]
				Return orderedLayers_Conflict
			End Get
		End Property

		Protected Friend Overrides ReadOnly Property FlattenedGradientsView As INDArray
			Get
				If network.getFlattenedGradients() Is Nothing Then
					network.initGradientsView()
				End If
				Return network.getFlattenedGradients()
			End Get
		End Property

		Protected Friend Overrides ReadOnly Property Params As INDArray
			Get
				Return network.params()
			End Get
		End Property

		Protected Friend Overrides ReadOnly Property MiniBatch As Boolean
			Get
				Return network.conf().isMiniBatch()
			End Get
		End Property
	End Class

End Namespace