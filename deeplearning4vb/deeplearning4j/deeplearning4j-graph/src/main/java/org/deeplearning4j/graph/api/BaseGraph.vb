Imports System
Imports System.Collections.Generic

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

Namespace org.deeplearning4j.graph.api

	Public MustInherit Class BaseGraph(Of V, E)
		Implements IGraph(Of V, E)

		Public MustOverride Function getConnectedVertexIndices(ByVal vertex As Integer) As Integer()
		Public MustOverride Function getConnectedVertices(ByVal vertex As Integer) As IList(Of Vertex(Of V))
		Public MustOverride Function getRandomConnectedVertex(ByVal vertex As Integer, ByVal rng As Random) As Vertex(Of V)
		Public MustOverride Function getVertexDegree(ByVal vertex As Integer) As Integer
		Public MustOverride Function getEdgesOut(ByVal vertex As Integer) As IList(Of Edge(Of E))
		Public MustOverride Sub addEdge(ByVal edge As Edge(Of E)) Implements IGraph(Of V, E).addEdge
		Public MustOverride Function getVertices(ByVal from As Integer, ByVal [to] As Integer) As IList(Of Vertex(Of V))
		Public MustOverride Function getVertices(ByVal indexes() As Integer) As IList(Of Vertex(Of V))
		Public MustOverride Function getVertex(ByVal idx As Integer) As Vertex(Of V)
		Public MustOverride Function numVertices() As Integer Implements IGraph(Of V, E).numVertices


		Public Overridable Sub addEdge(ByVal from As Integer, ByVal [to] As Integer, ByVal value As E, ByVal directed As Boolean) Implements IGraph(Of V, E).addEdge
			addEdge(New Edge(Of )(from, [to], value, directed))
		End Sub

	End Class

End Namespace