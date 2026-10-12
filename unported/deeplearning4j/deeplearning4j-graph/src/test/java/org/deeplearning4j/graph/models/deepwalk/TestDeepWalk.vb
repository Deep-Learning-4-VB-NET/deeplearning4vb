Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports org.deeplearning4j.graph.api
Imports org.deeplearning4j.graph.api
Imports GraphLoader = org.deeplearning4j.graph.data.GraphLoader
Imports org.deeplearning4j.graph.graph
Imports org.deeplearning4j.graph.iterator
Imports org.deeplearning4j.graph.iterator
Imports org.deeplearning4j.graph.iterator.parallel
Imports org.deeplearning4j.graph.iterator.parallel
Imports org.deeplearning4j.graph.models
Imports GraphVectorSerializer = org.deeplearning4j.graph.models.loader.GraphVectorSerializer
Imports StringVertexFactory = org.deeplearning4j.graph.vertexfactory.StringVertexFactory
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Timeout = org.junit.jupiter.api.Timeout
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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
'ORIGINAL LINE: @Disabled("Permissions issues on CI") @NativeTag @Tag(TagNames.FILE_IO) public class TestDeepWalk extends org.deeplearning4j.BaseDL4JTest
	Public Class TestDeepWalk
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 120_000L 'Increase timeout due to intermittently slow CI machines
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(60000) public void testBasic() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBasic()
			'Very basic test. Load graph, build tree, call fit, make sure it doesn't throw any exceptions

			Dim cpr As New ClassPathResource("deeplearning4j-graph/testgraph_7vertices.txt")

			Dim graph As Graph(Of String, String) = GraphLoader.loadUndirectedGraphEdgeListFile(cpr.TempFileFromArchive.getAbsolutePath(), 7, ",")

			Dim vectorSize As Integer = 5
			Dim windowSize As Integer = 2

			Dim deepWalk As DeepWalk(Of String, String) = (New DeepWalk.Builder(Of String, String)()).learningRate(0.01).vectorSize(vectorSize).windowSize(windowSize).learningRate(0.01).build()
			deepWalk.initialize(graph)

			For i As Integer = 0 To 6
				Dim vector As INDArray = deepWalk.getVertexVector(i)
				assertArrayEquals(New Long() {vectorSize}, vector.shape())
	'            System.out.println(Arrays.toString(vector.dup().data().asFloat()));
			Next i

			Dim iter As GraphWalkIterator(Of String) = New RandomWalkIterator(Of String)(graph, 8)

			deepWalk.fit(iter)

			For t As Integer = 0 To 4
				iter.reset()
				deepWalk.fit(iter)
	'            System.out.println("--------------------");
				For i As Integer = 0 To 6
					Dim vector As INDArray = deepWalk.getVertexVector(i)
					assertArrayEquals(New Long() {vectorSize}, vector.shape())
	'                System.out.println(Arrays.toString(vector.dup().data().asFloat()));
				Next i
			Next t
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(180000) public void testParallel()
		Public Overridable Sub testParallel()

			Dim graph As IGraph(Of String, String) = generateRandomGraph(30, 4)

			Dim vectorSize As Integer = 20
			Dim windowSize As Integer = 2

			Dim deepWalk As DeepWalk(Of String, String) = (New DeepWalk.Builder(Of String, String)()).learningRate(0.01).vectorSize(vectorSize).windowSize(windowSize).learningRate(0.01).build()
			deepWalk.initialize(graph)



			deepWalk.fit(graph, 6)
		End Sub


		Private Shared Function generateRandomGraph(ByVal nVertices As Integer, ByVal nEdgesPerVertex As Integer) As Graph(Of String, String)

			Dim r As New Random(12345)

			Dim graph As New Graph(Of String, String)(nVertices, New StringVertexFactory())
			Dim i As Integer = 0
			Do While i < nVertices
				For j As Integer = 0 To nEdgesPerVertex - 1
					Dim [to] As Integer = r.Next(nVertices)
					Dim edge As New Edge(Of String)(i, [to], i & "--" & [to], False)
					graph.addEdge(edge)
				Next j
				i += 1
			Loop
			Return graph
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(60000) public void testVerticesNearest()
		Public Overridable Sub testVerticesNearest()

			Dim nVertices As Integer = 20
			Dim graph As Graph(Of String, String) = generateRandomGraph(nVertices, 8)

			Dim vectorSize As Integer = 5
			Dim windowSize As Integer = 2
			Dim deepWalk As DeepWalk(Of String, String) = (New DeepWalk.Builder(Of String, String)()).learningRate(0.01).vectorSize(vectorSize).windowSize(windowSize).learningRate(0.01).build()
			deepWalk.initialize(graph)

			deepWalk.fit(graph, 10)

			Dim topN As Integer = 5
			Dim nearestTo As Integer = 4
			Dim nearest() As Integer = deepWalk.verticesNearest(nearestTo, topN)
			Dim cosSim(topN - 1) As Double
			Dim minSimNearest As Double = 1
			For i As Integer = 0 To topN - 1
				cosSim(i) = deepWalk.similarity(nearest(i), nearestTo)
				minSimNearest = Math.Min(minSimNearest, cosSim(i))
				If i > 0 Then
					assertTrue(cosSim(i) <= cosSim(i - 1))
				End If
			Next i

			For i As Integer = 0 To nVertices - 1
				If i = nearestTo Then
					Continue For
				End If
				Dim skip As Boolean = False
				For j As Integer = 0 To nearest.Length - 1
					If i = nearest(j) Then
						skip = True
						Continue For
					End If
				Next j
				If skip Then
					Continue For
				End If

				Dim sim As Double = deepWalk.similarity(i, nearestTo)
	'            System.out.println(i + "\t" + nearestTo + "\t" + sim);
				assertTrue(sim <= minSimNearest)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(60000) public void testLoadingSaving(@TempDir Path testDir) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLoadingSaving(ByVal testDir As Path)
			Dim [out] As String = "dl4jdwtestout.txt"

			Dim nVertices As Integer = 20
			Dim graph As Graph(Of String, String) = generateRandomGraph(nVertices, 8)

			Dim vectorSize As Integer = 5
			Dim windowSize As Integer = 2
			Dim deepWalk As DeepWalk(Of String, String) = (New DeepWalk.Builder(Of String, String)()).learningRate(0.01).vectorSize(vectorSize).windowSize(windowSize).learningRate(0.01).build()
			deepWalk.initialize(graph)

			deepWalk.fit(graph, 10)

			Dim f As New File(testDir.toFile(),[out])
			GraphVectorSerializer.writeGraphVectors(deepWalk, f.getAbsolutePath())

			Dim vectors As GraphVectors(Of String, String) = CType(GraphVectorSerializer.loadTxtVectors(f), GraphVectors(Of String, String))

			assertEquals(deepWalk.numVertices(), vectors.numVertices())
			assertEquals(deepWalk.VectorSize, vectors.VectorSize)

			For i As Integer = 0 To nVertices - 1
				Dim vecDW As INDArray = deepWalk.getVertexVector(i)
				Dim vecLoaded As INDArray = vectors.getVertexVector(i)

				For j As Integer = 0 To vectorSize - 1
					Dim d1 As Double = vecDW.getDouble(j)
					Dim d2 As Double = vecLoaded.getDouble(j)
					Dim relError As Double = Math.Abs(d1 - d2) / (Math.Abs(d1) + Math.Abs(d2))
					assertTrue(relError < 1e-6)
				Next j
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(180000) public void testDeepWalk13Vertices() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDeepWalk13Vertices()

			Dim nVertices As Integer = 13

			Dim cpr As New ClassPathResource("deeplearning4j-graph/graph13.txt")
			Dim graph As Graph(Of String, String) = GraphLoader.loadUndirectedGraphEdgeListFile(cpr.TempFileFromArchive.getAbsolutePath(), 13, ",")

	'        System.out.println(graph);

			Nd4j.Random.setSeed(12345)

			Dim nEpochs As Integer = 5

			'Set up network
			Dim deepWalk As DeepWalk(Of String, String) = (New DeepWalk.Builder(Of String, String)()).vectorSize(50).windowSize(4).seed(12345).build()

			'Run learning
			For i As Integer = 0 To nEpochs - 1
				deepWalk.LearningRate = 0.03 / nEpochs * (nEpochs - i)
				deepWalk.fit(graph, 10)
			Next i

			'Calculate similarity(0,i)
			For i As Integer = 0 To nVertices - 1
	'            System.out.println(deepWalk.similarity(0, i));
				deepWalk.similarity(0, i)
			Next i

			For i As Integer = 0 To nVertices - 1
	'            System.out.println(deepWalk.getVertexVector(i));
				deepWalk.getVertexVector(i)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(60000) public void testDeepWalkWeightedParallel() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDeepWalkWeightedParallel()

			'Load graph
			Dim path As String = (New ClassPathResource("deeplearning4j-graph/WeightedGraph.txt")).TempFileFromArchive.getAbsolutePath()
			Dim numVertices As Integer = 9
			Dim delim As String = ","
			Dim ignoreLinesStartingWith() As String = {"//"} 'Comment lines start with "//"
			Dim graph As IGraph(Of String, Double) = GraphLoader.loadWeightedEdgeListFile(path, numVertices, delim, True, ignoreLinesStartingWith)

			'Set up DeepWalk
			Dim vectorSize As Integer = 5
			Dim windowSize As Integer = 2
			Dim deepWalk As DeepWalk(Of String, Double) = (New DeepWalk.Builder(Of String, Double)()).learningRate(0.01).vectorSize(vectorSize).windowSize(windowSize).learningRate(0.01).build()
			deepWalk.initialize(graph)

			'Can't use the following method here: defaults to unweighted random walk
			'deepWalk.fit(graph, 10);  //Unweighted random walk

			'Create GraphWalkIteratorProvider. The GraphWalkIteratorProvider is used to create multiple GraphWalkIterator objects.
			'Here, it is used to create a GraphWalkIterator, one for each thread
			Dim walkLength As Integer = 5
			Dim iteratorProvider As GraphWalkIteratorProvider(Of String) = New WeightedRandomWalkGraphIteratorProvider(Of String)(graph, walkLength)

			'Fit in parallel
			deepWalk.fit(iteratorProvider)

		End Sub
	End Class

End Namespace