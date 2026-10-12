Imports System
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports NoEdgeHandling = org.deeplearning4j.models.sequencevectors.graph.enums.NoEdgeHandling
Imports WalkDirection = org.deeplearning4j.models.sequencevectors.graph.enums.WalkDirection
Imports NoEdgesException = org.deeplearning4j.models.sequencevectors.graph.exception.NoEdgesException
Imports org.deeplearning4j.models.sequencevectors.graph.primitives
Imports org.deeplearning4j.models.sequencevectors.graph.primitives
Imports org.deeplearning4j.models.sequencevectors.graph.walkers
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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


	Public Class RandomWalker(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements GraphWalker(Of T)

		Protected Friend walkLength As Integer = 5
		Protected Friend noEdgeHandling As NoEdgeHandling = NoEdgeHandling.EXCEPTION_ON_DISCONNECTED
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.deeplearning4j.models.sequencevectors.graph.primitives.IGraph<T, ?> sourceGraph;
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
		Protected Friend sourceGraph As IGraph(Of T, Object)
		Protected Friend position As New AtomicInteger(0)
		Protected Friend rng As New Random(DateTimeHelper.CurrentUnixTimeMillis())
		Protected Friend seed As Long
		Protected Friend order() As Integer
		Protected Friend walkDirection As WalkDirection
		Protected Friend alpha As Double

		Private Shared ReadOnly logger As Logger = LoggerFactory.getLogger(GetType(RandomWalker))

		Protected Friend Sub New()

		End Sub


		''' <summary>
		''' This method checks, if walker has any more sequences left in queue
		''' 
		''' @return
		''' </summary>
		Public Overridable Function hasNext() As Boolean Implements GraphWalker(Of T).hasNext
			Return position.get() < sourceGraph.numVertices()
		End Function

		Public Overridable ReadOnly Property LabelEnabled As Boolean Implements GraphWalker(Of T).isLabelEnabled
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' This method returns next walk sequence from this graph
		''' 
		''' @return
		''' </summary>
		Public Overridable Function [next]() As Sequence(Of T) Implements GraphWalker(Of T).next
			Dim visitedHops(walkLength - 1) As Integer
			Arrays.Fill(visitedHops, -1)

			Dim sequence As New Sequence(Of T)()

			Dim startPosition As Integer = position.getAndIncrement()
			Dim lastId As Integer = -1
			Dim startPoint As Integer = order(startPosition)
			'System.out.println("");


			startPosition = startPoint

			'if (startPosition == 0 || startPoint % 1000 == 0)
			'   System.out.println("ATZ Walk: ");

			For i As Integer = 0 To walkLength - 1
				Dim vertex As Vertex(Of T) = sourceGraph.getVertex(startPosition)

				Dim currentPosition As Integer = startPosition

				sequence.addElement(vertex.getValue())
				visitedHops(i) = vertex.vertexID()
				'if (startPoint == 0 || startPoint % 1000 == 0)
				' System.out.print("" + vertex.vertexID() + " -> ");


				If alpha > 0 AndAlso lastId <> startPoint AndAlso lastId <> -1 AndAlso alpha > rng.NextDouble() Then
					startPosition = startPoint
					Continue For
				End If


				' get next vertex
				Select Case walkDirection
					Case WalkDirection.RANDOM
						Dim nextHops() As Integer = sourceGraph.getConnectedVertexIndices(currentPosition)
						startPosition = nextHops(rng.Next(nextHops.Length))
					Case WalkDirection.FORWARD_ONLY
						' here we remove only last hop
						Dim nextHops() As Integer = ArrayUtils.removeElements(sourceGraph.getConnectedVertexIndices(currentPosition), lastId)
						If nextHops.Length > 0 Then
							startPosition = nextHops(rng.Next(nextHops.Length))
						Else
							Select Case noEdgeHandling
								Case NoEdgeHandling.CUTOFF_ON_DISCONNECTED
									i += walkLength
								Case NoEdgeHandling.EXCEPTION_ON_DISCONNECTED
									Throw New NoEdgesException("No more edges at vertex [" & currentPosition & "]")
								Case NoEdgeHandling.SELF_LOOP_ON_DISCONNECTED
									startPosition = currentPosition
								Case NoEdgeHandling.PADDING_ON_DISCONNECTED
									Throw New System.NotSupportedException("PADDING not implemented yet")
								Case NoEdgeHandling.RESTART_ON_DISCONNECTED
									startPosition = startPoint
								Case Else
									Throw New System.NotSupportedException("NoEdgeHandling mode [" & noEdgeHandling & "] not implemented yet.")
							End Select
						End If
					Case WalkDirection.FORWARD_UNIQUE
						' here we remove all previously visited hops, and we don't get  back to them ever
						Dim nextHops() As Integer = ArrayUtils.removeElements(sourceGraph.getConnectedVertexIndices(currentPosition), visitedHops)
						If nextHops.Length > 0 Then
							startPosition = nextHops(rng.Next(nextHops.Length))
						Else
							' if we don't have any more unique hops within this path - break out.
							Select Case noEdgeHandling
								Case NoEdgeHandling.CUTOFF_ON_DISCONNECTED
									i += walkLength
								Case NoEdgeHandling.EXCEPTION_ON_DISCONNECTED
									Throw New NoEdgesException("No more edges at vertex [" & currentPosition & "]")
								Case NoEdgeHandling.SELF_LOOP_ON_DISCONNECTED
									startPosition = currentPosition
								Case NoEdgeHandling.PADDING_ON_DISCONNECTED
									Throw New System.NotSupportedException("PADDING not implemented yet")
								Case NoEdgeHandling.RESTART_ON_DISCONNECTED
									startPosition = startPoint
								Case Else
									Throw New System.NotSupportedException("NoEdgeHandling mode [" & noEdgeHandling & "] not implemented yet.")
							End Select
						End If
					Case WalkDirection.FORWARD_PREFERRED
						' here we remove all previously visited hops, and if there's no next unique hop available - we fallback to anything, but the last one
						Dim nextHops() As Integer = ArrayUtils.removeElements(sourceGraph.getConnectedVertexIndices(currentPosition), visitedHops)
						If nextHops.Length = 0 Then
							nextHops = ArrayUtils.removeElements(sourceGraph.getConnectedVertexIndices(currentPosition), lastId)
							If nextHops.Length = 0 Then
								Select Case noEdgeHandling
									Case NoEdgeHandling.CUTOFF_ON_DISCONNECTED
										i += walkLength
									Case NoEdgeHandling.EXCEPTION_ON_DISCONNECTED
										Throw New NoEdgesException("No more edges at vertex [" & currentPosition & "]")
									Case NoEdgeHandling.SELF_LOOP_ON_DISCONNECTED
										startPosition = currentPosition
									Case NoEdgeHandling.PADDING_ON_DISCONNECTED
										Throw New System.NotSupportedException("PADDING not implemented yet")
									Case NoEdgeHandling.RESTART_ON_DISCONNECTED
										startPosition = startPoint
									Case Else
										Throw New System.NotSupportedException("NoEdgeHandling mode [" & noEdgeHandling & "] not implemented yet.")
								End Select
							Else
								startPosition = nextHops(rng.Next(nextHops.Length))
							End If
						End If
					Case Else
						Throw New System.NotSupportedException("Unknown WalkDirection [" & walkDirection & "]")
				End Select

				lastId = vertex.vertexID()
			Next i

			'if (startPoint == 0 || startPoint % 1000 == 0)
			'System.out.println("");
			Return sequence
		End Function

		''' <summary>
		''' This method resets walker
		''' </summary>
		''' <param name="shuffle"> if TRUE, order of walks will be shuffled </param>
		Public Overridable Sub reset(ByVal shuffle As Boolean) Implements GraphWalker(Of T).reset
			Me.position.set(0)
			If shuffle Then
				logger.trace("Calling shuffle() on entries...")
				' https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#The_modern_algorithm
				For i As Integer = order.Length - 1 To 1 Step -1
					Dim j As Integer = rng.Next(i + 1)
					Dim temp As Integer = order(j)
					order(j) = order(i)
					order(i) = temp
				Next i
			End If
		End Sub

		Public Class Builder(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
'JAVA TO VB CONVERTER NOTE: The field walkLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend walkLength_Conflict As Integer = 5
'JAVA TO VB CONVERTER NOTE: The field noEdgeHandling was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend noEdgeHandling_Conflict As NoEdgeHandling = NoEdgeHandling.RESTART_ON_DISCONNECTED
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: protected org.deeplearning4j.models.sequencevectors.graph.primitives.IGraph<T, ?> sourceGraph;
			Protected Friend sourceGraph As IGraph(Of T, Object)
'JAVA TO VB CONVERTER NOTE: The field seed was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend seed_Conflict As Long = 0
'JAVA TO VB CONVERTER NOTE: The field walkDirection was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend walkDirection_Conflict As WalkDirection = WalkDirection.FORWARD_ONLY
			Protected Friend alpha As Double

			''' <summary>
			''' Builder constructor for RandomWalker
			''' </summary>
			''' <param name="graph"> source graph to be used for this walker </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull IGraph<T, ?> graph)
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
			Public Sub New(ByVal graph As IGraph(Of T1))
				Me.sourceGraph = graph
			End Sub

			''' <summary>
			''' This method specifies output sequence (walk) length
			''' </summary>
			''' <param name="walkLength">
			''' @return </param>
			Public Overridable Function setWalkLength(ByVal walkLength As Integer) As Builder(Of T)
				Me.walkLength_Conflict = walkLength
				Return Me
			End Function

			''' <summary>
			''' This method defines walker behavior when it gets to node which has no next nodes available
			''' Default value: RESTART_ON_DISCONNECTED
			''' </summary>
			''' <param name="handling">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> setNoEdgeHandling(@NonNull NoEdgeHandling handling)
			Public Overridable Function setNoEdgeHandling(ByVal handling As NoEdgeHandling) As Builder(Of T)
				Me.noEdgeHandling_Conflict = handling
				Return Me
			End Function

			''' <summary>
			''' This method specifies random seed.
			''' </summary>
			''' <param name="seed">
			''' @return </param>
			Public Overridable Function setSeed(ByVal seed As Long) As Builder(Of T)
				Me.seed_Conflict = seed
				Return Me
			End Function

			''' <summary>
			''' This method defines next hop selection within walk
			''' </summary>
			''' <param name="direction">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> setWalkDirection(@NonNull WalkDirection direction)
			Public Overridable Function setWalkDirection(ByVal direction As WalkDirection) As Builder(Of T)
				Me.walkDirection_Conflict = direction
				Return Me
			End Function

			''' <summary>
			''' This method defines a chance for walk restart
			''' Good value would be somewhere between 0.03-0.07
			''' </summary>
			''' <param name="alpha">
			''' @return </param>
			Public Overridable Function setRestartProbability(ByVal alpha As Double) As Builder(Of T)
				Me.alpha = alpha
				Return Me
			End Function

			''' <summary>
			''' This method builds RandomWalker instance
			''' @return
			''' </summary>
			Public Overridable Function build() As RandomWalker(Of T)
				Dim walker As New RandomWalker(Of T)()
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