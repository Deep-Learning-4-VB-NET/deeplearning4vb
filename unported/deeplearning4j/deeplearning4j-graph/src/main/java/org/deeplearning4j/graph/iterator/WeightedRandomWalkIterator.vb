Imports System
Imports System.Collections.Generic
Imports org.deeplearning4j.graph.api
Imports org.deeplearning4j.graph.api
Imports org.deeplearning4j.graph.api
Imports NoEdgeHandling = org.deeplearning4j.graph.api.NoEdgeHandling
Imports NoEdgesException = org.deeplearning4j.graph.exception.NoEdgesException
Imports org.deeplearning4j.graph.graph

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

Namespace org.deeplearning4j.graph.iterator


	Public Class WeightedRandomWalkIterator(Of V)
		Implements GraphWalkIterator(Of V)

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: private final org.deeplearning4j.graph.api.IGraph<V, ? extends Number> graph;
		Private ReadOnly graph As IGraph(Of V, Number)
'JAVA TO VB CONVERTER NOTE: The field walkLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly walkLength_Conflict As Integer
		Private ReadOnly mode As NoEdgeHandling
		Private ReadOnly firstVertex As Integer
		Private ReadOnly lastVertex As Integer


		Private position As Integer
		Private rng As Random
		Private order() As Integer

'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
'ORIGINAL LINE: public WeightedRandomWalkIterator(org.deeplearning4j.graph.api.IGraph<V, ? extends Number> graph, int walkLength)
		Public Sub New(ByVal graph As IGraph(Of V, Number), ByVal walkLength As Integer)
			Me.New(graph, walkLength, DateTimeHelper.CurrentUnixTimeMillis(), NoEdgeHandling.EXCEPTION_ON_DISCONNECTED)
		End Sub

		''' <summary>
		'''Construct a RandomWalkIterator for a given graph, with a specified walk length and random number generator seed.<br>
		''' Uses {@code NoEdgeHandling.EXCEPTION_ON_DISCONNECTED} - hence exception will be thrown when generating random
		''' walks on graphs with vertices containing having no edges, or no outgoing edges (for directed graphs) </summary>
		''' <seealso cref= #WeightedRandomWalkIterator(IGraph, int, long, NoEdgeHandling) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
'ORIGINAL LINE: public WeightedRandomWalkIterator(org.deeplearning4j.graph.api.IGraph<V, ? extends Number> graph, int walkLength, long rngSeed)
		Public Sub New(ByVal graph As IGraph(Of V, Number), ByVal walkLength As Integer, ByVal rngSeed As Long)
			Me.New(graph, walkLength, rngSeed, NoEdgeHandling.EXCEPTION_ON_DISCONNECTED)
		End Sub

		''' <param name="graph"> IGraph to conduct walks on </param>
		''' <param name="walkLength"> length of each walk. Walk of length 0 includes 1 vertex, walk of 1 includes 2 vertices etc </param>
		''' <param name="rngSeed"> seed for randomization </param>
		''' <param name="mode"> mode for handling random walks from vertices with either no edges, or no outgoing edges (for directed graphs) </param>
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
'ORIGINAL LINE: public WeightedRandomWalkIterator(org.deeplearning4j.graph.api.IGraph<V, ? extends Number> graph, int walkLength, long rngSeed, org.deeplearning4j.graph.api.NoEdgeHandling mode)
		Public Sub New(ByVal graph As IGraph(Of V, Number), ByVal walkLength As Integer, ByVal rngSeed As Long, ByVal mode As NoEdgeHandling)
			Me.New(graph, walkLength, rngSeed, mode, 0, graph.numVertices())
		End Sub

		''' <summary>
		'''Constructor used to generate random walks starting at a subset of the vertices in the graph. Order of starting
		''' vertices is randomized within this subset </summary>
		''' <param name="graph"> IGraph to conduct walks on </param>
		''' <param name="walkLength"> length of each walk. Walk of length 0 includes 1 vertex, walk of 1 includes 2 vertices etc </param>
		''' <param name="rngSeed"> seed for randomization </param>
		''' <param name="mode"> mode for handling random walks from vertices with either no edges, or no outgoing edges (for directed graphs) </param>
		''' <param name="firstVertex"> first vertex index (inclusive) to start random walks from </param>
		''' <param name="lastVertex"> last vertex index (exclusive) to start random walks from </param>
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
'ORIGINAL LINE: public WeightedRandomWalkIterator(org.deeplearning4j.graph.api.IGraph<V, ? extends Number> graph, int walkLength, long rngSeed, org.deeplearning4j.graph.api.NoEdgeHandling mode, int firstVertex, int lastVertex)
		Public Sub New(ByVal graph As IGraph(Of V, Number), ByVal walkLength As Integer, ByVal rngSeed As Long, ByVal mode As NoEdgeHandling, ByVal firstVertex As Integer, ByVal lastVertex As Integer)
			Me.graph = graph
			Me.walkLength_Conflict = walkLength
			Me.rng = New Random(rngSeed)
			Me.mode = mode
			Me.firstVertex = firstVertex
			Me.lastVertex = lastVertex

			order = New Integer((lastVertex - firstVertex) - 1){}
			For i As Integer = 0 To order.Length - 1
				order(i) = firstVertex + i
			Next i
			reset()
		End Sub

		Public Overridable Function [next]() As IVertexSequence(Of V) Implements GraphWalkIterator(Of V).next
			If Not hasNext() Then
				Throw New NoSuchElementException()
			End If
			'Generate a weighted random walk starting at vertex order[current]
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: int currVertexIdx = order[position++];
			Dim currVertexIdx As Integer = order(position)
				position += 1
			Dim indices(walkLength_Conflict) As Integer
			indices(0) = currVertexIdx
			If walkLength_Conflict = 0 Then
				Return New VertexSequence(Of V)(graph, indices)
			End If

			For i As Integer = 1 To walkLength_Conflict
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.List<? extends org.deeplearning4j.graph.api.Edge<? extends Number>> edgeList = graph.getEdgesOut(currVertexIdx);
				Dim edgeList As IList(Of Edge(Of Number)) = graph.getEdgesOut(currVertexIdx)

				'First: check if there are any outgoing edges from this vertex. If not: handle the situation
				If edgeList Is Nothing OrElse edgeList.Count = 0 Then
					Select Case mode
						Case NoEdgeHandling.SELF_LOOP_ON_DISCONNECTED
							For j As Integer = i To walkLength_Conflict - 1
								indices(j) = currVertexIdx
							Next j
							Return New VertexSequence(Of V)(graph, indices)
						Case NoEdgeHandling.EXCEPTION_ON_DISCONNECTED
							Throw New NoEdgesException("Cannot conduct random walk: vertex " & currVertexIdx & " has no outgoing edges. " & " Set NoEdgeHandling mode to NoEdgeHandlingMode.SELF_LOOP_ON_DISCONNECTED to self loop instead of " & "throwing an exception in this situation.")
						Case Else
							Throw New Exception("Unknown/not implemented NoEdgeHandling mode: " & mode)
					End Select
				End If

				'To do a weighted random walk: we need to know total weight of all outgoing edges
				Dim totalWeight As Double = 0.0
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: for (org.deeplearning4j.graph.api.Edge<? extends Number> edge : edgeList)
				For Each edge As Edge(Of Number) In edgeList
					totalWeight += edge.getValue().doubleValue()
				Next edge

				Dim d As Double = rng.NextDouble()
				Dim threshold As Double = d * totalWeight
				Dim sumWeight As Double = 0.0
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: for (org.deeplearning4j.graph.api.Edge<? extends Number> edge : edgeList)
				For Each edge As Edge(Of Number) In edgeList
					sumWeight += edge.getValue().doubleValue()
					If sumWeight >= threshold Then
						If edge.isDirected() Then
							currVertexIdx = edge.getTo()
						Else
							If edge.getFrom() = currVertexIdx Then
								currVertexIdx = edge.getTo()
							Else
								currVertexIdx = edge.getFrom() 'Undirected edge: might be next--currVertexIdx instead of currVertexIdx--next
							End If
						End If
						indices(i) = currVertexIdx
						Exit For
					End If
				Next edge
			Next i
			Return New VertexSequence(Of V)(graph, indices)
		End Function

		Public Overridable Function hasNext() As Boolean Implements GraphWalkIterator(Of V).hasNext
			Return position < order.Length
		End Function

		Public Overridable Sub reset() Implements GraphWalkIterator(Of V).reset
			position = 0
			'https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#The_modern_algorithm
			For i As Integer = order.Length - 1 To 1 Step -1
				Dim j As Integer = rng.Next(i + 1)
				Dim temp As Integer = order(j)
				order(j) = order(i)
				order(i) = temp
			Next i
		End Sub

		Public Overridable Function walkLength() As Integer Implements GraphWalkIterator(Of V).walkLength
			Return walkLength_Conflict
		End Function
	End Class

End Namespace