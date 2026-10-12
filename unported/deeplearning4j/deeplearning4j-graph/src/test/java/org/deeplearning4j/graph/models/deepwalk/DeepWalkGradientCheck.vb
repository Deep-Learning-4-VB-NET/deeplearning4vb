Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports GraphLoader = org.deeplearning4j.graph.data.GraphLoader
Imports org.deeplearning4j.graph.graph
Imports org.deeplearning4j.graph.iterator
Imports org.deeplearning4j.graph.iterator
Imports InMemoryGraphLookupTable = org.deeplearning4j.graph.models.embeddings.InMemoryGraphLookupTable
Imports org.junit.jupiter.api
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports org.junit.jupiter.api.Assertions

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

Namespace org.deeplearning4j.graph.models.deepwalk


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled("Permissions issues on CI") @NativeTag @Tag(TagNames.FILE_IO) public class DeepWalkGradientCheck extends org.deeplearning4j.BaseDL4JTest
	Public Class DeepWalkGradientCheck
		Inherits BaseDL4JTest

		Public Const epsilon As Double = 1e-8
		Public Const MAX_REL_ERROR As Double = 1e-3
		Public Const MIN_ABS_ERROR As Double = 1e-5

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before()
		Public Overridable Sub before()
			Nd4j.DataType = DataType.DOUBLE
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(10000) public void checkGradients() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub checkGradients()

			Dim cpr As New ClassPathResource("deeplearning4j-graph/testgraph_7vertices.txt")

			Dim graph As Graph(Of String, String) = GraphLoader.loadUndirectedGraphEdgeListFile(cpr.TempFileFromArchive.getAbsolutePath(), 7, ",")

			Dim vectorSize As Integer = 5
			Dim windowSize As Integer = 2

			Nd4j.Random.setSeed(12345)
			Dim deepWalk As DeepWalk(Of String, String) = (New DeepWalk.Builder(Of String, String)()).learningRate(0.01).vectorSize(vectorSize).windowSize(windowSize).build()
			deepWalk.initialize(graph)

			For i As Integer = 0 To 6
				Dim vector As INDArray = deepWalk.getVertexVector(i)
				assertArrayEquals(New Long() {vectorSize}, vector.shape())
	'            System.out.println(Arrays.toString(vector.dup().data().asFloat()));
			Next i

			Dim iter As GraphWalkIterator(Of String) = New RandomWalkIterator(Of String)(graph, 8)

			deepWalk.fit(iter)

			'Now, to check gradients:
			Dim table As InMemoryGraphLookupTable = DirectCast(deepWalk.lookupTable(), InMemoryGraphLookupTable)
			Dim tree As GraphHuffman = DirectCast(table.Tree, GraphHuffman)

			'For each pair of input/output vertices: check gradients
			For i As Integer = 0 To 6 'in

				'First: check probabilities p(out|in)
				Dim probs(6) As Double
				Dim sumProb As Double = 0.0
				For j As Integer = 0 To 6
					probs(j) = table.calculateProb(i, j)
					assertTrue(probs(j) >= 0.0 AndAlso probs(j) <= 1.0)
					sumProb += probs(j)
				Next j
				assertTrue(Math.Abs(sumProb - 1.0) < 1e-5, "Output probabilities do not sum to 1.0")

				For j As Integer = 0 To 6 'out
					'p(j|i)

					Dim pathInnerNodes() As Integer = tree.getPathInnerNodes(j)

					'Calculate gradients:
					Dim vecAndGrads()() As INDArray = table.vectorsAndGradients(i, j)
					assertEquals(2, vecAndGrads.Length)
					assertEquals(pathInnerNodes.Length + 1, vecAndGrads(0).Length)
					assertEquals(pathInnerNodes.Length + 1, vecAndGrads(1).Length)

					'Calculate gradients:
					'Two types of gradients to test:
					'(a) gradient of loss fn. wrt inner node vector representation
					'(b) gradient of loss fn. wrt vector for input word


					Dim vertexVector As INDArray = table.getVector(i)

					'Check gradients for inner nodes:
					For p As Integer = 0 To pathInnerNodes.Length - 1
						Dim innerNodeIdx As Integer = pathInnerNodes(p)
						Dim innerNodeVector As INDArray = table.getInnerNodeVector(innerNodeIdx)

						Dim innerNodeGrad As INDArray = vecAndGrads(1)(p + 1)

						Dim v As Integer = 0
						Do While v < innerNodeVector.length()
							Dim backpropGradient As Double = innerNodeGrad.getDouble(v)

							Dim origParamValue As Double = innerNodeVector.getDouble(v)
							innerNodeVector.putScalar(v, origParamValue + epsilon)
							Dim scorePlus As Double = table.calculateScore(i, j)
							innerNodeVector.putScalar(v, origParamValue - epsilon)
							Dim scoreMinus As Double = table.calculateScore(i, j)
							innerNodeVector.putScalar(v, origParamValue) 'reset param so it doesn't affect later calcs


							Dim numericalGradient As Double = (scorePlus - scoreMinus) / (2 * epsilon)

							Dim relError As Double
							Dim absErr As Double
							If backpropGradient = 0.0 AndAlso numericalGradient = 0.0 Then
								relError = 0.0
								absErr = 0.0
							Else
								absErr = Math.Abs(backpropGradient - numericalGradient)
								relError = absErr / (Math.Abs(backpropGradient) + Math.Abs(numericalGradient))
							End If

							Dim msg As String = "innerNode grad: i=" & i & ", j=" & j & ", p=" & p & ", v=" & v & " - relError: " & relError & ", scorePlus=" & scorePlus & ", scoreMinus=" & scoreMinus & ", numGrad=" & numericalGradient & ", backpropGrad = " & backpropGradient

							If relError > MAX_REL_ERROR AndAlso absErr > MIN_ABS_ERROR Then
								fail(msg)
							End If
	'                        else
	'                            System.out.println(msg);
							v += 1
						Loop
					Next p

					'Check gradients for input word vector:
					Dim vectorGrad As INDArray = vecAndGrads(1)(0)
					assertArrayEquals(vectorGrad.shape(), vertexVector.shape())
					For v As Integer = 0 To vectorGrad.length() - 1

						Dim backpropGradient As Double = vectorGrad.getDouble(v)

						Dim origParamValue As Double = vertexVector.getDouble(v)
						vertexVector.putScalar(v, origParamValue + epsilon)
						Dim scorePlus As Double = table.calculateScore(i, j)
						vertexVector.putScalar(v, origParamValue - epsilon)
						Dim scoreMinus As Double = table.calculateScore(i, j)
						vertexVector.putScalar(v, origParamValue)

						Dim numericalGradient As Double = (scorePlus - scoreMinus) / (2 * epsilon)

						Dim relError As Double
						Dim absErr As Double
						If backpropGradient = 0.0 AndAlso numericalGradient = 0.0 Then
							relError = 0.0
							absErr = 0.0
						Else
							absErr = Math.Abs(backpropGradient - numericalGradient)
							relError = absErr / (Math.Abs(backpropGradient) + Math.Abs(numericalGradient))
						End If

						Dim msg As String = "vector grad: i=" & i & ", j=" & j & ", v=" & v & " - relError: " & relError & ", scorePlus=" & scorePlus & ", scoreMinus=" & scoreMinus & ", numGrad=" & numericalGradient & ", backpropGrad = " & backpropGradient

						If relError > MAX_REL_ERROR AndAlso absErr > MIN_ABS_ERROR Then
							fail(msg)
						End If
	'                    else
	'                        System.out.println(msg);
					Next v
	'                System.out.println();
				Next j

			Next i

		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(60000) public void checkGradients2() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub checkGradients2()

			Dim minAbsError As Double = 1e-5

			Dim cpr As New ClassPathResource("deeplearning4j-graph/graph13.txt")

			Dim nVertices As Integer = 13
			Dim graph As Graph(Of String, String) = GraphLoader.loadUndirectedGraphEdgeListFile(cpr.TempFileFromArchive.getAbsolutePath(), 13, ",")

			Dim vectorSize As Integer = 10
			Dim windowSize As Integer = 3

			Nd4j.Random.setSeed(12345)
			Dim deepWalk As DeepWalk(Of String, String) = (New DeepWalk.Builder(Of String, String)()).learningRate(0.01).vectorSize(vectorSize).windowSize(windowSize).learningRate(0.01).build()
			deepWalk.initialize(graph)

			For i As Integer = 0 To nVertices - 1
				Dim vector As INDArray = deepWalk.getVertexVector(i)
				assertArrayEquals(New Long() {vectorSize}, vector.shape())
	'            System.out.println(Arrays.toString(vector.dup().data().asFloat()));
			Next i

			Dim iter As GraphWalkIterator(Of String) = New RandomWalkIterator(Of String)(graph, 10)

			deepWalk.fit(iter)

			'Now, to check gradients:
			Dim table As InMemoryGraphLookupTable = DirectCast(deepWalk.lookupTable(), InMemoryGraphLookupTable)
			Dim tree As GraphHuffman = DirectCast(table.Tree, GraphHuffman)

			'For each pair of input/output vertices: check gradients
			For i As Integer = 0 To nVertices - 1 'in

				'First: check probabilities p(out|in)
				Dim probs(nVertices - 1) As Double
				Dim sumProb As Double = 0.0
				For j As Integer = 0 To nVertices - 1
					probs(j) = table.calculateProb(i, j)
					assertTrue(probs(j) >= 0.0 AndAlso probs(j) <= 1.0)
					sumProb += probs(j)
				Next j
				assertTrue(Math.Abs(sumProb - 1.0) < 1e-5, "Output probabilities do not sum to 1.0 (i=" & i & "), sum=" & sumProb)

				For j As Integer = 0 To nVertices - 1 'out
					'p(j|i)

					Dim pathInnerNodes() As Integer = tree.getPathInnerNodes(j)

					'Calculate gradients:
					Dim vecAndGrads()() As INDArray = table.vectorsAndGradients(i, j)
					assertEquals(2, vecAndGrads.Length)
					assertEquals(pathInnerNodes.Length + 1, vecAndGrads(0).Length)
					assertEquals(pathInnerNodes.Length + 1, vecAndGrads(1).Length)

					'Calculate gradients:
					'Two types of gradients to test:
					'(a) gradient of loss fn. wrt inner node vector representation
					'(b) gradient of loss fn. wrt vector for input word


					Dim vertexVector As INDArray = table.getVector(i)

					'Check gradients for inner nodes:
					For p As Integer = 0 To pathInnerNodes.Length - 1
						Dim innerNodeIdx As Integer = pathInnerNodes(p)
						Dim innerNodeVector As INDArray = table.getInnerNodeVector(innerNodeIdx)

						Dim innerNodeGrad As INDArray = vecAndGrads(1)(p + 1)

						Dim v As Integer = 0
						Do While v < innerNodeVector.length()
							Dim backpropGradient As Double = innerNodeGrad.getDouble(v)

							Dim origParamValue As Double = innerNodeVector.getDouble(v)
							innerNodeVector.putScalar(v, origParamValue + epsilon)
							Dim scorePlus As Double = table.calculateScore(i, j)
							innerNodeVector.putScalar(v, origParamValue - epsilon)
							Dim scoreMinus As Double = table.calculateScore(i, j)
							innerNodeVector.putScalar(v, origParamValue) 'reset param so it doesn't affect later calcs


							Dim numericalGradient As Double = (scorePlus - scoreMinus) / (2 * epsilon)

							Dim relError As Double
							If backpropGradient = 0.0 AndAlso numericalGradient = 0.0 Then
								relError = 0.0
							Else
								relError = Math.Abs(backpropGradient - numericalGradient) / (Math.Abs(backpropGradient) + Math.Abs(numericalGradient))
							End If
							Dim absErr As Double = Math.Abs(backpropGradient - numericalGradient)

							Dim msg As String = "innerNode grad: i=" & i & ", j=" & j & ", p=" & p & ", v=" & v & " - relError: " & relError & ", scorePlus=" & scorePlus & ", scoreMinus=" & scoreMinus & ", numGrad=" & numericalGradient & ", backpropGrad = " & backpropGradient

							If relError > MAX_REL_ERROR AndAlso absErr > minAbsError Then
								fail(msg)
							End If
	'                        else
	'                            System.out.println(msg);
							v += 1
						Loop
					Next p

					'Check gradients for input word vector:
					Dim vectorGrad As INDArray = vecAndGrads(1)(0)
					assertArrayEquals(vectorGrad.shape(), vertexVector.shape())
					For v As Integer = 0 To vectorGrad.length() - 1

						Dim backpropGradient As Double = vectorGrad.getDouble(v)

						Dim origParamValue As Double = vertexVector.getDouble(v)
						vertexVector.putScalar(v, origParamValue + epsilon)
						Dim scorePlus As Double = table.calculateScore(i, j)
						vertexVector.putScalar(v, origParamValue - epsilon)
						Dim scoreMinus As Double = table.calculateScore(i, j)
						vertexVector.putScalar(v, origParamValue)

						Dim numericalGradient As Double = (scorePlus - scoreMinus) / (2 * epsilon)

						Dim relError As Double
						Dim absErr As Double
						If backpropGradient = 0.0 AndAlso numericalGradient = 0.0 Then
							relError = 0.0
							absErr = 0.0
						Else
							relError = Math.Abs(backpropGradient - numericalGradient) / (Math.Abs(backpropGradient) + Math.Abs(numericalGradient))
							absErr = Math.Abs(backpropGradient - numericalGradient)
						End If

						Dim msg As String = "vector grad: i=" & i & ", j=" & j & ", v=" & v & " - relError: " & relError & ", scorePlus=" & scorePlus & ", scoreMinus=" & scoreMinus & ", numGrad=" & numericalGradient & ", backpropGrad = " & backpropGradient

						If relError > MAX_REL_ERROR AndAlso absErr > MIN_ABS_ERROR Then
							fail(msg)
						End If
	'                    else
	'                        System.out.println(msg);
					Next v
	'                System.out.println();
				Next j

			Next i

		End Sub

		Private Shared Function getBit(ByVal [in] As Long, ByVal bitNum As Integer) As Boolean
			Dim mask As Long = 1L << bitNum
			Return ([in] And mask) <> 0L
		End Function
	End Class

End Namespace