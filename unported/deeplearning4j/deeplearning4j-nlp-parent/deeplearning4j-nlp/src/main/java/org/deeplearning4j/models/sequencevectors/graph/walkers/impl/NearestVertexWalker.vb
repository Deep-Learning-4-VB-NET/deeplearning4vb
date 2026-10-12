Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SamplingMode = org.deeplearning4j.models.sequencevectors.graph.enums.SamplingMode
Imports org.deeplearning4j.models.sequencevectors.graph.primitives
Imports org.deeplearning4j.models.sequencevectors.graph.primitives
Imports org.deeplearning4j.models.sequencevectors.graph.walkers
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class NearestVertexWalker<V extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> implements org.deeplearning4j.models.sequencevectors.graph.walkers.GraphWalker<V>
	Public Class NearestVertexWalker(Of V As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements GraphWalker(Of V)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.deeplearning4j.models.sequencevectors.graph.primitives.IGraph<V, ?> sourceGraph;
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
		Protected Friend sourceGraph As IGraph(Of V, Object)
		Protected Friend walkLength As Integer = 0
		Protected Friend seed As Long = 0
		Protected Friend samplingMode As SamplingMode = SamplingMode.RANDOM
		Protected Friend order() As Integer
		Protected Friend rng As Random
		Protected Friend depth As Integer

		Private position As New AtomicInteger(0)

		Protected Friend Sub New()

		End Sub

		Public Overridable Function hasNext() As Boolean Implements GraphWalker(Of V).hasNext
			Return position.get() < order.Length
		End Function

		Public Overridable Function [next]() As Sequence(Of V) Implements GraphWalker(Of V).next
			Return walk(sourceGraph.getVertex(order(position.getAndIncrement())), 1)
		End Function

		Public Overridable Sub reset(ByVal shuffle As Boolean) Implements GraphWalker(Of V).reset
			position.set(0)
			If shuffle Then
				log.trace("Calling shuffle() on entries...")
				' https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#The_modern_algorithm
				For i As Integer = order.Length - 1 To 1 Step -1
					Dim j As Integer = rng.Next(i + 1)
					Dim temp As Integer = order(j)
					order(j) = order(i)
					order(i) = temp
				Next i
			End If
		End Sub

		Protected Friend Overridable Function walk(ByVal node As Vertex(Of V), ByVal cDepth As Integer) As Sequence(Of V)
			Dim sequence As New Sequence(Of V)()

			Dim idx As Integer = node.vertexID()
			Dim vertices As IList(Of Vertex(Of V)) = sourceGraph.getConnectedVertices(idx)

			sequence.SequenceLabel = node.getValue()

			If walkLength = 0 Then
				' if walk is unlimited - we use all connected vertices as is
				For Each vertex As Vertex(Of V) In vertices
					sequence.addElement(vertex.getValue())
				Next vertex
			Else
				' if walks are limited, we care about sampling mode
				Select Case samplingMode
					Case SamplingMode.MAX_POPULARITY
						vertices.Sort(New VertexComparator(Me, Of )(sourceGraph))
						For i As Integer = 0 To walkLength - 1
							sequence.addElement(vertices(i).getValue())

							' going for one more depth level
							If depth > 1 AndAlso cDepth < depth Then
								cDepth += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: org.deeplearning4j.models.sequencevectors.sequence.Sequence<V> nextDepth = walk(vertices.get(i), ++cDepth);
								Dim nextDepth As Sequence(Of V) = walk(vertices(i), cDepth)
								For Each element As V In nextDepth.getElements()
									If sequence.getElementByLabel(element.getLabel()) Is Nothing Then
										sequence.addElement(element)
									End If
								Next element
							End If
						Next i

'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
					Case SamplingMode.MEDIAN_POPULARITY
						vertices.Sort(New VertexComparator(Me, Of )(sourceGraph))
						Dim i As Integer = (vertices.Count \ 2) - (walkLength \ 2)
						Dim e As Integer = 0
						Do While e < walkLength AndAlso i < vertices.Count
							sequence.addElement(vertices(i).getValue())

							' going for one more depth level
							If depth > 1 AndAlso cDepth < depth Then
								cDepth += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: org.deeplearning4j.models.sequencevectors.sequence.Sequence<V> nextDepth = walk(vertices.get(i), ++cDepth);
								Dim nextDepth As Sequence(Of V) = walk(vertices(i), cDepth)
								For Each element As V In nextDepth.getElements()
									If sequence.getElementByLabel(element.getLabel()) Is Nothing Then
										sequence.addElement(element)
									End If
								Next element
							End If
							i += 1
							e += 1
						Loop

'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
					Case SamplingMode.MIN_POPULARITY
						vertices.Sort(New VertexComparator(Me, Of )(sourceGraph))
						Dim i As Integer = vertices.Count
						Dim e As Integer = 0
						Do While e < walkLength AndAlso i >= 0
							sequence.addElement(vertices(i).getValue())

							' going for one more depth level
							If depth > 1 AndAlso cDepth < depth Then
								cDepth += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: org.deeplearning4j.models.sequencevectors.sequence.Sequence<V> nextDepth = walk(vertices.get(i), ++cDepth);
								Dim nextDepth As Sequence(Of V) = walk(vertices(i), cDepth)
								For Each element As V In nextDepth.getElements()
									If sequence.getElementByLabel(element.getLabel()) Is Nothing Then
										sequence.addElement(element)
									End If
								Next element
							End If
							i -= 1
							e += 1
						Loop
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
					Case SamplingMode.RANDOM
						' we randomly sample some number of connected vertices
						If vertices.Count <= walkLength Then
							For Each vertex As Vertex(Of V) In vertices
								sequence.addElement(vertex.getValue())
							Next vertex
						Else
							Dim elements As ISet(Of V) = New HashSet(Of V)()
							Do While elements.Count < walkLength
								Dim vertex As Vertex(Of V) = ArrayUtil.getRandomElement(vertices)
								elements.Add(vertex.getValue())

								' going for one more depth level
								If depth > 1 AndAlso cDepth < depth Then
									cDepth += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: org.deeplearning4j.models.sequencevectors.sequence.Sequence<V> nextDepth = walk(vertex, ++cDepth);
									Dim nextDepth As Sequence(Of V) = walk(vertex, cDepth)
									For Each element As V In nextDepth.getElements()
										If sequence.getElementByLabel(element.getLabel()) Is Nothing Then
											sequence.addElement(element)
										End If
									Next element
								End If
							Loop

							sequence.addElements(elements)
						End If
					Case Else
						Throw New ND4JIllegalStateException("Unknown sampling mode was passed in: [" & samplingMode & "]")
				End Select
			End If

			Return sequence
		End Function

		Public Overridable ReadOnly Property LabelEnabled As Boolean Implements GraphWalker(Of V).isLabelEnabled
			Get
				Return True
			End Get
		End Property

		Public Class Builder(Of V As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
'JAVA TO VB CONVERTER NOTE: The field walkLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend walkLength_Conflict As Integer = 0
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: protected org.deeplearning4j.models.sequencevectors.graph.primitives.IGraph<V, ?> sourceGraph;
			Protected Friend sourceGraph As IGraph(Of V, Object)
'JAVA TO VB CONVERTER NOTE: The field samplingMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend samplingMode_Conflict As SamplingMode = SamplingMode.RANDOM
'JAVA TO VB CONVERTER NOTE: The field seed was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend seed_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field depth was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend depth_Conflict As Integer = 1

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull IGraph<V, ?> graph)
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
			Public Sub New(ByVal graph As IGraph(Of T1))
				Me.sourceGraph = graph
			End Sub

			Public Overridable Function setSeed(ByVal seed As Long) As Builder
				Me.seed_Conflict = seed
				Return Me
			End Function

			''' <summary>
			''' This method defines maximal number of nodes to be visited during walk.
			''' 
			''' PLEASE NOTE: If set to 0 - no limits will be used.
			''' 
			''' Default value: 0 </summary>
			''' <param name="length">
			''' @return </param>
			Public Overridable Function setWalkLength(ByVal length As Integer) As Builder
				walkLength_Conflict = length
				Return Me
			End Function

			''' <summary>
			''' This method specifies, how deep walker goes from starting point
			''' 
			''' Default value: 1 </summary>
			''' <param name="depth">
			''' @return </param>
			Public Overridable Function setDepth(ByVal depth As Integer) As Builder
				Me.depth_Conflict = depth
				Return Me
			End Function

			''' <summary>
			''' This method defines sorting which will be used to generate walks.
			''' 
			''' PLEASE NOTE: This option has effect only if walkLength is limited (>0).
			''' </summary>
			''' <param name="mode">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder setSamplingMode(@NonNull SamplingMode mode)
			Public Overridable Function setSamplingMode(ByVal mode As SamplingMode) As Builder
				Me.samplingMode_Conflict = mode
				Return Me
			End Function

			''' <summary>
			''' This method returns you new GraphWalker instance
			''' 
			''' @return
			''' </summary>
			Public Overridable Function build() As NearestVertexWalker(Of V)
				Dim walker As New NearestVertexWalker(Of V)()
				walker.sourceGraph = Me.sourceGraph
				walker.walkLength = Me.walkLength_Conflict
				walker.samplingMode = Me.samplingMode_Conflict
				walker.depth = Me.depth_Conflict

				walker.order = New Integer(sourceGraph.numVertices() - 1){}
				For i As Integer = 0 To walker.order.Length - 1
					walker.order(i) = i
				Next i

				walker.rng = New Random(CInt(seed_Conflict))

				walker.reset(True)

				Return walker
			End Function
		End Class

		Protected Friend Class VertexComparator(Of V As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement, E As Number)
			Implements IComparer(Of Vertex(Of V))

			Private ReadOnly outerInstance As NearestVertexWalker(Of V)

			Friend graph As IGraph(Of V, E)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public VertexComparator(@NonNull IGraph<V, E> graph)
			Public Sub New(ByVal outerInstance As NearestVertexWalker(Of V), ByVal graph As IGraph(Of V, E))
				Me.outerInstance = outerInstance
				Me.graph = graph
			End Sub

			Public Overridable Function Compare(ByVal o1 As Vertex(Of V), ByVal o2 As Vertex(Of V)) As Integer Implements IComparer(Of Vertex(Of V)).Compare
				Return Integer.compare(graph.getConnectedVertices(o2.vertexID()).Count, graph.getConnectedVertices(o1.vertexID()).Count)
			End Function
		End Class
	End Class

End Namespace