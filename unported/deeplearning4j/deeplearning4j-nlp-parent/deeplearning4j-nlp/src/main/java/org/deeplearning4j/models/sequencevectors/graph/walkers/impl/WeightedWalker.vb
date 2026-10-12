Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports NoEdgeHandling = org.deeplearning4j.models.sequencevectors.graph.enums.NoEdgeHandling
Imports WalkDirection = org.deeplearning4j.models.sequencevectors.graph.enums.WalkDirection
Imports NoEdgesException = org.deeplearning4j.models.sequencevectors.graph.exception.NoEdgesException
Imports org.deeplearning4j.models.sequencevectors.graph.primitives
Imports org.deeplearning4j.models.sequencevectors.graph.primitives
Imports org.deeplearning4j.models.sequencevectors.graph.primitives
Imports org.deeplearning4j.models.sequencevectors.graph.walkers
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement

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

Namespace org.deeplearning4j.models.sequencevectors.graph.walkers.impl


	Public Class WeightedWalker(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Inherits RandomWalker(Of T)
		Implements GraphWalker(Of T)

		Protected Friend Sub New()

		End Sub

		''' <summary>
		''' This method checks, if walker has any more sequences left in queue
		''' 
		''' @return
		''' </summary>
		Public Overrides Function hasNext() As Boolean Implements GraphWalker(Of T).hasNext
			Return MyBase.hasNext()
		End Function

		Public Overrides ReadOnly Property LabelEnabled As Boolean Implements GraphWalker(Of T).isLabelEnabled
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' This method returns next walk sequence from this graph
		''' 
		''' @return
		''' </summary>
		Public Overrides Function [next]() As Sequence(Of T) Implements GraphWalker(Of T).next
			Dim sequence As New Sequence(Of T)()

			Dim startPosition As Integer = position.getAndIncrement()
			Dim lastId As Integer = -1
			Dim currentPoint As Integer = order(startPosition)
			Dim startPoint As Integer = currentPoint
			For i As Integer = 0 To walkLength - 1

				If alpha > 0 AndAlso lastId <> startPoint AndAlso lastId <> -1 AndAlso alpha > rng.NextDouble() Then
					startPosition = startPoint
					Continue For
				End If


				Dim vertex As Vertex(Of T) = sourceGraph.getVertex(currentPoint)
				sequence.addElement(vertex.getValue())

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.List<? extends org.deeplearning4j.models.sequencevectors.graph.primitives.Edge<? extends Number>> edges = sourceGraph.getEdgesOut(currentPoint);
				Dim edges As IList(Of Edge(Of Number)) = sourceGraph.getEdgesOut(currentPoint)

				If edges Is Nothing OrElse edges.Count = 0 Then
					Select Case noEdgeHandling
						Case NoEdgeHandling.CUTOFF_ON_DISCONNECTED
							' we just break this sequence
							i = walkLength
						Case NoEdgeHandling.EXCEPTION_ON_DISCONNECTED
							Throw New NoEdgesException("No available edges left")
						Case NoEdgeHandling.PADDING_ON_DISCONNECTED
							' TODO: implement padding
							Throw New System.NotSupportedException("Padding isn't implemented yet")
						Case NoEdgeHandling.RESTART_ON_DISCONNECTED
							currentPoint = order(startPosition)
						Case NoEdgeHandling.SELF_LOOP_ON_DISCONNECTED
							' we pad walk with this vertex, to do that - we just don't do anything, and currentPoint will be the same till the end of walk
					End Select
				Else
					Dim totalWeight As Double = 0.0
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: for (org.deeplearning4j.models.sequencevectors.graph.primitives.Edge<? extends Number> edge : edges)
					For Each edge As Edge(Of Number) In edges
						totalWeight += edge.getValue().doubleValue()
					Next edge

					Dim d As Double = rng.NextDouble()
					Dim threshold As Double = d * totalWeight
					Dim sumWeight As Double = 0.0
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: for (org.deeplearning4j.models.sequencevectors.graph.primitives.Edge<? extends Number> edge : edges)
					For Each edge As Edge(Of Number) In edges
						sumWeight += edge.getValue().doubleValue()
						If sumWeight >= threshold Then
							If edge.isDirected() Then
								currentPoint = edge.getTo()
							Else
								If edge.getFrom() = currentPoint Then
									currentPoint = edge.getTo()
								Else
									currentPoint = edge.getFrom() 'Undirected edge: might be next--currVertexIdx instead of currVertexIdx--next
								End If
							End If
							lastId = currentPoint
							Exit For
						End If
					Next edge
				End If
			Next i

			Return sequence
		End Function

		''' <summary>
		''' This method resets walker
		''' </summary>
		''' <param name="shuffle"> if TRUE, order of walks will be shuffled </param>
		Public Overrides Sub reset(ByVal shuffle As Boolean) Implements GraphWalker(Of T).reset
			MyBase.reset(shuffle)
		End Sub

		Public Class Builder(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
			Inherits RandomWalker.Builder(Of T)

'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
'ORIGINAL LINE: public Builder(org.deeplearning4j.models.sequencevectors.graph.primitives.IGraph<T, ? extends Number> sourceGraph)
			Public Sub New(ByVal sourceGraph As IGraph(Of T, Number))
				MyBase.New(sourceGraph)
			End Sub

			''' <summary>
			''' This method specifies output sequence (walk) length
			''' </summary>
			''' <param name="walkLength">
			''' @return </param>
			Public Overrides Function setWalkLength(ByVal walkLength As Integer) As Builder(Of T)
				MyBase.WalkLength = walkLength
				Return Me
			End Function

			''' <summary>
			''' This method defines walker behavior when it gets to node which has no next nodes available
			''' Default value: RESTART_ON_DISCONNECTED
			''' </summary>
			''' <param name="handling">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder<T> setNoEdgeHandling(@NonNull NoEdgeHandling handling)
			Public Overrides Function setNoEdgeHandling(ByVal handling As NoEdgeHandling) As Builder(Of T)
				MyBase.NoEdgeHandling = handling
				Return Me
			End Function

			''' <summary>
			''' This method specifies random seed.
			''' </summary>
			''' <param name="seed">
			''' @return </param>
			Public Overrides Function setSeed(ByVal seed As Long) As Builder(Of T)
				MyBase.Seed = seed
				Return Me
			End Function

			''' <summary>
			''' This method defines next hop selection within walk
			''' </summary>
			''' <param name="direction">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder<T> setWalkDirection(@NonNull WalkDirection direction)
			Public Overrides Function setWalkDirection(ByVal direction As WalkDirection) As Builder(Of T)
				MyBase.WalkDirection = direction
				Return Me
			End Function

			''' <summary>
			''' This method defines a chance for walk restart
			''' Good value would be somewhere between 0.03-0.07
			''' </summary>
			''' <param name="alpha">
			''' @return </param>
			Public Overrides Function setRestartProbability(ByVal alpha As Double) As RandomWalker.Builder(Of T)
				Return MyBase.setRestartProbability(alpha)
			End Function

			Public Overrides Function build() As WeightedWalker(Of T)
				Dim walker As New WeightedWalker(Of T)()
				walker.noEdgeHandling = Me.noEdgeHandling_Conflict
				walker.sourceGraph = Me.sourceGraph
				walker.walkLength = Me.walkLength_Conflict
				walker.seed = Me.seed_Conflict
				walker.walkDirection = Me.walkDirection_Conflict
				walker.alpha = Me.alpha

				walker.order = New Integer(sourceGraph.numVertices() - 1){}
				For i As Integer = 0 To walker.order.Length - 1
					walker.order(i) = i
				Next i

				If Me.seed_Conflict <> 0 Then
					walker.rng = New Random(Me.seed_Conflict)
				End If

				Return walker
			End Function
		End Class
	End Class

End Namespace