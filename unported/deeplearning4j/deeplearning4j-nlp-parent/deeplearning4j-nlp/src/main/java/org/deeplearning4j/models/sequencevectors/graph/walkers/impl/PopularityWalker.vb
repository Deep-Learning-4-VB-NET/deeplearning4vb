Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports MathArrays = org.apache.commons.math3.util.MathArrays
Imports NoEdgeHandling = org.deeplearning4j.models.sequencevectors.graph.enums.NoEdgeHandling
Imports PopularityMode = org.deeplearning4j.models.sequencevectors.graph.enums.PopularityMode
Imports SpreadSpectrum = org.deeplearning4j.models.sequencevectors.graph.enums.SpreadSpectrum
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


	Public Class PopularityWalker(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Inherits RandomWalker(Of T)
		Implements GraphWalker(Of T)

		Protected Friend popularityMode As PopularityMode = PopularityMode.MAXIMUM
		Protected Friend spread As Integer = 10
		Protected Friend spectrum As SpreadSpectrum

		Private Shared ReadOnly logger As Logger = LoggerFactory.getLogger(GetType(PopularityWalker))

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


		Protected Friend Class NodeComparator
			Implements IComparer(Of Node(Of T))

			Private ReadOnly outerInstance As PopularityWalker(Of T)

			Public Sub New(ByVal outerInstance As PopularityWalker(Of T))
				Me.outerInstance = outerInstance
			End Sub


			Public Overridable Function Compare(ByVal o1 As Node(Of T), ByVal o2 As Node(Of T)) As Integer Implements IComparer(Of Node(Of T)).Compare
				Return Integer.compare(o2.weight, o1.weight)
			End Function
		End Class

		''' <summary>
		''' This method returns next walk sequence from this graph
		''' 
		''' @return
		''' </summary>
		Public Overrides Function [next]() As Sequence(Of T) Implements GraphWalker(Of T).next
			Dim sequence As New Sequence(Of T)()
			Dim visitedHops(walkLength - 1) As Integer
			Arrays.Fill(visitedHops, -1)

			Dim startPosition As Integer = position.getAndIncrement()
			Dim lastId As Integer = -1
			Dim startPoint As Integer = order(startPosition)
			startPosition = startPoint
			For i As Integer = 0 To walkLength - 1

				Dim vertex As Vertex(Of T) = sourceGraph.getVertex(startPosition)

				Dim currentPosition As Integer = startPosition

				sequence.addElement(vertex.getValue())
				visitedHops(i) = vertex.vertexID()
				Dim cSpread As Integer = 0

				If alpha > 0 AndAlso lastId <> startPoint AndAlso lastId <> -1 AndAlso alpha > rng.NextDouble() Then
					startPosition = startPoint
					Continue For
				End If

				Select Case walkDirection
					Case WalkDirection.RANDOM, FORWARD_ONLY, FORWARD_UNIQUE, FORWARD_PREFERRED

						' ArrayUtils.removeElements(sourceGraph.getConnectedVertexIndices(order[currentPosition]), visitedHops);
						Dim connections() As Integer = ArrayUtils.removeElements(sourceGraph.getConnectedVertexIndices(vertex.vertexID()), visitedHops)

						' we get  popularity of each node connected to the current node.
						Dim queue As New PriorityQueue(Of Node(Of T))(Math.Max(10, connections.Length), New NodeComparator(Me))

						Dim start As Integer = 0
						Dim [stop] As Integer = 0
						Dim cnt As Integer = 0
						If connections.Length > 0 Then


							For Each connected As Integer In connections
								Dim tNode As New Node(Of T)(connected, sourceGraph.getConnectedVertices(connected).Count)
								queue.add(tNode)
							Next connected


							cSpread = If(spread > connections.Length, connections.Length, spread)

							Select Case popularityMode
								Case PopularityMode.MAXIMUM
									start = 0
									[stop] = start + cSpread - 1
								Case PopularityMode.MINIMUM
									start = connections.Length - cSpread
									[stop] = connections.Length - 1
								Case PopularityMode.AVERAGE
									Dim mid As Integer = connections.Length \ 2
									start = mid - (cSpread \ 2)
									[stop] = mid + (cSpread \ 2)
							End Select

							' logger.info("Spread: ["+ cSpread+ "], Connections: ["+ connections.length+"], Start: ["+start+"], Stop: ["+stop+"]");
							cnt = 0
							'logger.info("Queue: " + queue);
							'logger.info("Queue size: " + queue.size());

							Dim list As IList(Of Node(Of T)) = New List(Of Node(Of T))()
							Dim weights(cSpread - 1) As Double

							Dim fcnt As Integer = 0
							Do While Not queue.isEmpty()
								Dim node As Node(Of T) = queue.poll()
								If cnt >= start AndAlso cnt <= [stop] Then
									list.Add(node)
									weights(fcnt) = node.getWeight()
									fcnt += 1
								End If
								connections(cnt) = node.getVertexId()

								cnt += 1
							Loop


							Dim con As Integer = -1

							Select Case spectrum
								Case SpreadSpectrum.PLAIN
									con = RandomUtils.nextInt(start, [stop] + 1)

									'    logger.info("Picked selection: " + con);

									Dim nV As Vertex(Of T) = sourceGraph.getVertex(connections(con))
									startPosition = nV.vertexID()
									lastId = vertex.vertexID()
								Case SpreadSpectrum.PROPORTIONAL
									Dim norm() As Double = MathArrays.normalizeArray(weights, 1)
									Dim prob As Double = rng.NextDouble()
									Dim floor As Double = 0.0
									For b As Integer = 0 To weights.Length - 1
										If prob >= floor AndAlso prob < floor + norm(b) Then
											startPosition = list(b).getVertexId()
											lastId = startPosition
											Exit For
										Else
											floor += norm(b)
										End If
									Next b
							End Select

						Else
							Select Case noEdgeHandling
								Case NoEdgeHandling.EXCEPTION_ON_DISCONNECTED
									Throw New NoEdgesException("No more edges at vertex [" & currentPosition & "]")
								Case NoEdgeHandling.CUTOFF_ON_DISCONNECTED
									i += walkLength
								Case NoEdgeHandling.SELF_LOOP_ON_DISCONNECTED
									startPosition = currentPosition
								Case NoEdgeHandling.RESTART_ON_DISCONNECTED
									startPosition = startPoint
								Case Else
									Throw New System.NotSupportedException("Unsupported noEdgeHandling: [" & noEdgeHandling & "]")
							End Select
						End If
					Case Else
						Throw New System.NotSupportedException("Unknown WalkDirection: [" & walkDirection & "]")
				End Select

			Next i

			Return sequence
		End Function

		Public Overrides Sub reset(ByVal shuffle As Boolean) Implements GraphWalker(Of T).reset
			MyBase.reset(shuffle)
		End Sub

		Public Class Builder(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
			Inherits RandomWalker.Builder(Of T)

'JAVA TO VB CONVERTER NOTE: The field popularityMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend popularityMode_Conflict As PopularityMode = PopularityMode.MAXIMUM
			Protected Friend spread As Integer = 10
			Protected Friend spectrum As SpreadSpectrum = SpreadSpectrum.PLAIN

'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
'ORIGINAL LINE: public Builder(org.deeplearning4j.models.sequencevectors.graph.primitives.IGraph<T, ?> sourceGraph)
			Public Sub New(ByVal sourceGraph As IGraph(Of T1))
				MyBase.New(sourceGraph)
			End Sub


			''' <summary>
			''' This method defines which nodes should be taken in account when choosing next hope: maximum popularity, lowest popularity, or average popularity.
			''' Default value: MAXIMUM
			''' </summary>
			''' <param name="popularityMode">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> setPopularityMode(@NonNull PopularityMode popularityMode)
			Public Overridable Function setPopularityMode(ByVal popularityMode As PopularityMode) As Builder(Of T)
				Me.popularityMode_Conflict = popularityMode
				Return Me
			End Function

			''' <summary>
			''' This method defines, how much nodes should take place in next hop selection. Something like top-N nodes, or bottom-N nodes.
			''' Default value: 10
			''' </summary>
			''' <param name="topN">
			''' @return </param>
			Public Overridable Function setPopularitySpread(ByVal topN As Integer) As Builder(Of T)
				Me.spread = topN
				Return Me
			End Function

			''' <summary>
			''' This method allows you to define, if nodes within popularity spread should have equal chances to be picked for next hop, or they should have chances proportional to their popularity.
			''' 
			''' Default value: PLAIN
			''' </summary>
			''' <param name="spectrum">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> setSpreadSpectrum(@NonNull SpreadSpectrum spectrum)
			Public Overridable Function setSpreadSpectrum(ByVal spectrum As SpreadSpectrum) As Builder(Of T)
				Me.spectrum = spectrum
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
			''' This method specifies output sequence (walk) length
			''' </summary>
			''' <param name="walkLength">
			''' @return </param>
			Public Overrides Function setWalkLength(ByVal walkLength As Integer) As Builder(Of T)
				MyBase.WalkLength = walkLength
				Return Me
			End Function

			''' <summary>
			''' This method defines a chance for walk restart
			''' Good value would be somewhere between 0.03-0.07
			''' </summary>
			''' <param name="alpha">
			''' @return </param>
			Public Overrides Function setRestartProbability(ByVal alpha As Double) As Builder(Of T)
				MyBase.RestartProbability = alpha
				Return Me
			End Function

			''' <summary>
			''' This method builds PopularityWalker object with previously specified params
			''' 
			''' @return
			''' </summary>
			Public Overrides Function build() As PopularityWalker(Of T)
				Dim walker As New PopularityWalker(Of T)()
				walker.noEdgeHandling = Me.noEdgeHandling_Conflict
				walker.sourceGraph = Me.sourceGraph
				walker.walkLength = Me.walkLength_Conflict
				walker.seed = Me.seed_Conflict
				walker.walkDirection = Me.walkDirection_Conflict
				walker.alpha = Me.alpha
				walker.popularityMode = Me.popularityMode_Conflict
				walker.spread = Me.spread
				walker.spectrum = Me.spectrum

				walker.order = New Integer(sourceGraph.numVertices() - 1){}
				For i As Integer = 0 To walker.order.Length - 1
					walker.order(i) = i
				Next i

				If Me.seed_Conflict <> 0 Then
					walker.rng = New Random(CInt(Me.seed_Conflict))
				End If

				Return walker
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data private static class Node<T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> implements Comparable<Node<T>>
		Private Class Node(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
			Implements IComparable(Of Node(Of T))

			Friend vertexId As Integer
			Friend weight As Integer = 0

			Public Overridable Function CompareTo(ByVal o As Node(Of T)) As Integer Implements IComparable(Of Node(Of T)).CompareTo
				Return Integer.compare(Me.weight, o.weight)
			End Function
		End Class
	End Class

End Namespace