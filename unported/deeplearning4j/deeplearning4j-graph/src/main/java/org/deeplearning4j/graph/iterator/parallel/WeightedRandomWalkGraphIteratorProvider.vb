Imports System
Imports System.Collections.Generic
Imports org.deeplearning4j.graph.api
Imports NoEdgeHandling = org.deeplearning4j.graph.api.NoEdgeHandling
Imports org.deeplearning4j.graph.iterator
Imports org.deeplearning4j.graph.iterator

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

Namespace org.deeplearning4j.graph.iterator.parallel


	Public Class WeightedRandomWalkGraphIteratorProvider(Of V)
		Implements GraphWalkIteratorProvider(Of V)

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: private org.deeplearning4j.graph.api.IGraph<V, ? extends Number> graph;
		Private graph As IGraph(Of V, Number)
		Private walkLength As Integer
		Private rng As Random
		Private mode As NoEdgeHandling

'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
'ORIGINAL LINE: public WeightedRandomWalkGraphIteratorProvider(org.deeplearning4j.graph.api.IGraph<V, ? extends Number> graph, int walkLength)
		Public Sub New(ByVal graph As IGraph(Of V, Number), ByVal walkLength As Integer)
			Me.New(graph, walkLength, DateTimeHelper.CurrentUnixTimeMillis(), NoEdgeHandling.EXCEPTION_ON_DISCONNECTED)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
'ORIGINAL LINE: public WeightedRandomWalkGraphIteratorProvider(org.deeplearning4j.graph.api.IGraph<V, ? extends Number> graph, int walkLength, long seed, org.deeplearning4j.graph.api.NoEdgeHandling mode)
		Public Sub New(ByVal graph As IGraph(Of V, Number), ByVal walkLength As Integer, ByVal seed As Long, ByVal mode As NoEdgeHandling)
			Me.graph = graph
			Me.walkLength = walkLength
			Me.rng = New Random(seed)
			Me.mode = mode
		End Sub


		Public Overridable Function getGraphWalkIterators(ByVal numIterators As Integer) As IList(Of GraphWalkIterator(Of V)) Implements GraphWalkIteratorProvider(Of V).getGraphWalkIterators
			Dim nVertices As Integer = graph.numVertices()
			If numIterators > nVertices Then
				numIterators = nVertices
			End If

			Dim verticesPerIter As Integer = nVertices \ numIterators

			Dim list As IList(Of GraphWalkIterator(Of V)) = New List(Of GraphWalkIterator(Of V))(numIterators)
			Dim last As Integer = 0
			For i As Integer = 0 To numIterators - 1
				Dim from As Integer = last
				Dim [to] As Integer = Math.Min(nVertices, from + verticesPerIter)
				If i = numIterators - 1 Then
					[to] = nVertices
				End If

				Dim iter As GraphWalkIterator(Of V) = New WeightedRandomWalkIterator(Of V)(graph, walkLength, rng.nextLong(), mode, from, [to])
				list.Add(iter)
				last = [to]
			Next i

			Return list
		End Function
	End Class

End Namespace