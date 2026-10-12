Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports System.Linq
Imports org.deeplearning4j.graph.api
Imports org.deeplearning4j.graph.api
Imports org.deeplearning4j.graph.api
Imports org.deeplearning4j.graph.api
Imports NoEdgesException = org.deeplearning4j.graph.exception.NoEdgesException
Imports org.deeplearning4j.graph.vertexfactory

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

Namespace org.deeplearning4j.graph.graph


	Public Class Graph(Of V, E)
		Inherits BaseGraph(Of V, E)

		Private allowMultipleEdges As Boolean
		Private edges() As IList(Of Edge(Of E)) 'edge[i].get(j).to = k, then edge from i -> k
		Private vertices As IList(Of Vertex(Of V))

		Public Sub New(ByVal numVertices As Integer, ByVal vertexFactory As VertexFactory(Of V))
			Me.New(numVertices, False, vertexFactory)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public Graph(int numVertices, boolean allowMultipleEdges, org.deeplearning4j.graph.vertexfactory.VertexFactory<V> vertexFactory)
		Public Sub New(ByVal numVertices As Integer, ByVal allowMultipleEdges As Boolean, ByVal vertexFactory As VertexFactory(Of V))
			If numVertices <= 0 Then
				Throw New System.ArgumentException()
			End If
			Me.allowMultipleEdges = allowMultipleEdges

			vertices = New List(Of Vertex(Of V))(numVertices)
			For i As Integer = 0 To numVertices - 1
				vertices.Add(vertexFactory.create(i))
			Next i

			edges = CType(Array.CreateInstance(GetType(System.Collections.IList), numVertices), IList(Of Edge(Of E))())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public Graph(List<org.deeplearning4j.graph.api.Vertex<V>> vertices, boolean allowMultipleEdges)
		Public Sub New(ByVal vertices As IList(Of Vertex(Of V)), ByVal allowMultipleEdges As Boolean)
			Me.vertices = New List(Of Vertex(Of V))(vertices)
			Me.allowMultipleEdges = allowMultipleEdges
			edges = CType(Array.CreateInstance(GetType(System.Collections.IList), vertices.Count), IList(Of Edge(Of E))())
		End Sub

		Public Sub New(ByVal vertices As IList(Of Vertex(Of V)))
			Me.New(vertices, False)
		End Sub

		Public Overrides Function numVertices() As Integer
			Return vertices.Count
		End Function

		Public Overrides Function getVertex(ByVal idx As Integer) As Vertex(Of V)
			If idx < 0 OrElse idx >= vertices.Count Then
				Throw New System.ArgumentException("Invalid index: " & idx)
			End If
			Return vertices(idx)
		End Function

		Public Overrides Function getVertices(ByVal indexes() As Integer) As IList(Of Vertex(Of V))
			Dim [out] As IList(Of Vertex(Of V)) = New List(Of Vertex(Of V))(indexes.Length)
			For Each i As Integer In indexes
				[out].Add(getVertex(i))
			Next i
			Return [out]
		End Function

		Public Overrides Function getVertices(ByVal from As Integer, ByVal [to] As Integer) As IList(Of Vertex(Of V))
			If [to] < from OrElse from < 0 OrElse [to] >= vertices.Count Then
				Throw New System.ArgumentException("Invalid range: from=" & from & ", to=" & [to])
			End If
			Dim [out] As IList(Of Vertex(Of V)) = New List(Of Vertex(Of V))([to] - from + 1)
			For i As Integer = from To [to]
				[out].Add(getVertex(i))
			Next i
			Return [out]
		End Function

		Public Overrides Sub addEdge(ByVal edge As Edge(Of E))
			If edge.getFrom() < 0 OrElse edge.getTo() >= vertices.Count Then
				Throw New System.ArgumentException("Invalid edge: " & edge & ", from/to indexes out of range")
			End If

			Dim fromList As IList(Of Edge(Of E)) = edges(edge.getFrom())
			If fromList Is Nothing Then
				fromList = New List(Of Edge(Of E))()
				edges(edge.getFrom()) = fromList
			End If
			addEdgeHelper(edge, fromList)

			If edge.isDirected() Then
				Return
			End If

			'Add other way too (to allow easy lookup for undirected edges)
			Dim toList As IList(Of Edge(Of E)) = edges(edge.getTo())
			If toList Is Nothing Then
				toList = New List(Of Edge(Of E))()
				edges(edge.getTo()) = toList
			End If
			addEdgeHelper(edge, toList)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public List<org.deeplearning4j.graph.api.Edge<E>> getEdgesOut(int vertex)
		Public Overrides Function getEdgesOut(ByVal vertex As Integer) As IList(Of Edge(Of E))
			If edges(vertex) Is Nothing Then
				Return java.util.Collections.emptyList()
			End If
			Return New List(Of Edge(Of E))(edges(vertex))
		End Function

		Public Overrides Function getVertexDegree(ByVal vertex As Integer) As Integer
			If edges(vertex) Is Nothing Then
				Return 0
			End If
			Return edges(vertex).Count
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.graph.api.Vertex<V> getRandomConnectedVertex(int vertex, Random rng) throws org.deeplearning4j.graph.exception.NoEdgesException
		Public Overrides Function getRandomConnectedVertex(ByVal vertex As Integer, ByVal rng As Random) As Vertex(Of V)
			If vertex < 0 OrElse vertex >= vertices.Count Then
				Throw New System.ArgumentException("Invalid vertex index: " & vertex)
			End If
			If edges(vertex) Is Nothing OrElse edges(vertex).Count = 0 Then
				Throw New NoEdgesException("Cannot generate random connected vertex: vertex " & vertex & " has no outgoing/undirected edges")
			End If
			Dim connectedVertexNum As Integer = rng.Next(edges(vertex).Count)
			Dim edge As Edge(Of E) = edges(vertex)(connectedVertexNum)
			If edge.getFrom() = vertex Then
				Return vertices(edge.getTo()) 'directed or undirected, vertex -> x
			Else
				Return vertices(edge.getFrom()) 'Undirected edge, x -> vertex
			End If
		End Function

		Public Overrides Function getConnectedVertices(ByVal vertex As Integer) As IList(Of Vertex(Of V))
			If vertex < 0 OrElse vertex >= vertices.Count Then
				Throw New System.ArgumentException("Invalid vertex index: " & vertex)
			End If

			If edges(vertex) Is Nothing Then
				Return java.util.Collections.emptyList()
			End If
			Dim list As IList(Of Vertex(Of V)) = New List(Of Vertex(Of V))(edges(vertex).Count)
			For Each edge As Edge(Of E) In edges(vertex)
				list.Add(vertices(edge.getTo()))
			Next edge
			Return list
		End Function

		Public Overrides Function getConnectedVertexIndices(ByVal vertex As Integer) As Integer()
			Dim [out]((If(edges(vertex) Is Nothing, 0, edges(vertex).Count)) - 1) As Integer
			If [out].Length = 0 Then
				Return [out]
			End If
			For i As Integer = 0 To [out].Length - 1
				Dim e As Edge(Of E) = edges(vertex)(i)
				[out](i) = (If(e.getFrom() = vertex, e.getTo(), e.getFrom()))
			Next i
			Return [out]
		End Function

		Private Sub addEdgeHelper(ByVal edge As Edge(Of E), ByVal list As IList(Of Edge(Of E)))
			If Not allowMultipleEdges Then
				'Check to avoid multiple edges
				Dim duplicate As Boolean = False

				If edge.isDirected() Then
					For Each e As Edge(Of E) In list
						If e.getTo() = edge.getTo() Then
							duplicate = True
							Exit For
						End If
					Next e
				Else
					For Each e As Edge(Of E) In list
						If (e.getFrom() = edge.getFrom() AndAlso e.getTo() = edge.getTo()) OrElse (e.getTo() = edge.getFrom() AndAlso e.getFrom() = edge.getTo()) Then
							duplicate = True
							Exit For
						End If
					Next e
				End If

				If Not duplicate Then
					list.Add(edge)
				End If
			Else
				'allow multiple/duplicate edges
				list.Add(edge)
			End If
		End Sub


		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			sb.Append("Graph {")
			sb.Append(vbLf & "Vertices {")
			For Each v As Vertex(Of V) In vertices
				sb.Append(vbLf & vbTab).Append(v)
			Next v
			sb.Append(vbLf & "}")
			sb.Append(vbLf & "Edges {")
			For i As Integer = 0 To edges.Length - 1
				sb.Append(vbLf & vbTab)
				If edges(i) Is Nothing Then
					Continue For
				End If
				sb.Append(i).Append(":")
				For Each e As Edge(Of E) In edges(i)
					sb.Append(" ").Append(e)
				Next e
			Next i
			sb.Append(vbLf & "}")
			sb.Append(vbLf & "}")
			Return sb.ToString()
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Not (TypeOf o Is Graph) Then
				Return False
			End If
			Dim g As Graph = DirectCast(o, Graph)
			If allowMultipleEdges <> g.allowMultipleEdges Then
				Return False
			End If
			If edges.Length <> g.edges.Length Then
				Return False
			End If
			If vertices.Count <> g.vertices.Count Then
				Return False
			End If
			For i As Integer = 0 To edges.Length - 1
'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: if (!edges[i].equals(g.edges[i]))
				If Not edges(i).SequenceEqual(g.edges(i)) Then
					Return False
				End If
			Next i
'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: return vertices.equals(g.vertices);
			Return vertices.SequenceEqual(g.vertices)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = 23
			result = 31 * result + (If(allowMultipleEdges, 1, 0))
			result = 31 * result + java.util.Arrays.hashCode(edges)
			result = 31 * result + vertices.GetHashCode()
			Return result
		End Function
	End Class

End Namespace