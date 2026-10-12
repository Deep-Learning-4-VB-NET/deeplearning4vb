Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports org.deeplearning4j.graph.api
Imports org.deeplearning4j.graph.api
Imports NoEdgeHandling = org.deeplearning4j.graph.api.NoEdgeHandling
Imports org.deeplearning4j.graph.iterator
Imports org.deeplearning4j.graph.iterator.parallel
Imports org.deeplearning4j.graph.iterator.parallel
Imports GraphVectorLookupTable = org.deeplearning4j.graph.models.embeddings.GraphVectorLookupTable
Imports org.deeplearning4j.graph.models.embeddings
Imports InMemoryGraphLookupTable = org.deeplearning4j.graph.models.embeddings.InMemoryGraphLookupTable
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
Imports PriorityScheduler = org.threadly.concurrent.PriorityScheduler
Imports FutureUtils = org.threadly.concurrent.future.FutureUtils

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


	<Serializable>
	Public Class DeepWalk(Of V, E)
		Inherits GraphVectorsImpl(Of V, E)

		Public Const STATUS_UPDATE_FREQUENCY As Integer = 1000
		Private log As Logger = LoggerFactory.getLogger(GetType(DeepWalk))

'JAVA TO VB CONVERTER NOTE: The field vectorSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private vectorSize_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field windowSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private windowSize_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field learningRate was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private learningRate_Conflict As Double
		Private initCalled As Boolean = False
		Private seed As Long
		Private nThreads As Integer = Runtime.getRuntime().availableProcessors()
		<NonSerialized>
		Private walkCounter As New AtomicLong(0)

		Public Sub New()

		End Sub

		Public Overrides ReadOnly Property VectorSize As Integer
			Get
				Return vectorSize_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property WindowSize As Integer
			Get
				Return windowSize_Conflict
			End Get
		End Property

		Public Overridable Property LearningRate As Double
			Get
				Return learningRate_Conflict
			End Get
			Set(ByVal learningRate As Double)
				Me.learningRate_Conflict = learningRate
				If lookupTable IsNot Nothing Then
					lookupTable.LearningRate = learningRate
				End If
			End Set
		End Property


		''' <summary>
		''' Initialize the DeepWalk model with a given graph. </summary>
		Public Overridable Sub initialize(ByVal graph As IGraph(Of V, E))
			Dim nVertices As Integer = graph.numVertices()
			Dim degrees(nVertices - 1) As Integer
			For i As Integer = 0 To nVertices - 1
				degrees(i) = graph.getVertexDegree(i)
			Next i
			initialize(degrees)
		End Sub

		''' <summary>
		''' Initialize the DeepWalk model with a list of vertex degrees for a graph.<br>
		''' Specifically, graphVertexDegrees[i] represents the vertex degree of the ith vertex<br>
		''' vertex degrees are used to construct a binary (Huffman) tree, which is in turn used in
		''' the hierarchical softmax implementation </summary>
		''' <param name="graphVertexDegrees"> degrees of each vertex </param>
		Public Overridable Sub initialize(ByVal graphVertexDegrees() As Integer)
			log.info("Initializing: Creating Huffman tree and lookup table...")
			Dim gh As New GraphHuffman(graphVertexDegrees.Length)
			gh.buildTree(graphVertexDegrees)
			lookupTable = New InMemoryGraphLookupTable(graphVertexDegrees.Length, vectorSize_Conflict, gh, learningRate_Conflict)
			initCalled = True
			log.info("Initialization complete")
		End Sub

		''' <summary>
		''' Fit the model, in parallel.
		''' This creates a set of GraphWalkIterators, which are then distributed one to each thread </summary>
		''' <param name="graph"> Graph to fit </param>
		''' <param name="walkLength"> Length of rangom walks to generate </param>
		Public Overridable Sub fit(ByVal graph As IGraph(Of V, E), ByVal walkLength As Integer)
			If Not initCalled Then
				initialize(graph)
			End If
			'First: create iterators, one for each thread

			Dim iteratorProvider As GraphWalkIteratorProvider(Of V) = New RandomWalkGraphIteratorProvider(Of V)(graph, walkLength, seed, NoEdgeHandling.SELF_LOOP_ON_DISCONNECTED)

			fit(iteratorProvider)
		End Sub

		''' <summary>
		''' Fit the model, in parallel, using a given GraphWalkIteratorProvider.<br>
		''' This object is used to generate multiple GraphWalkIterators, which can then be distributed to each thread
		''' to do in parallel<br>
		''' Note that <seealso cref="fit(IGraph, Integer)"/> will be more convenient in many cases<br>
		''' Note that <seealso cref="initialize(IGraph)"/> or <seealso cref="initialize(Integer[])"/> <em>must</em> be called first. </summary>
		''' <param name="iteratorProvider"> GraphWalkIteratorProvider </param>
		''' <seealso cref= #fit(IGraph, int) </seealso>
		Public Overridable Sub fit(ByVal iteratorProvider As GraphWalkIteratorProvider(Of V))
			If Not initCalled Then
				Throw New System.NotSupportedException("DeepWalk not initialized (call initialize before fit)")
			End If
			Dim iteratorList As IList(Of GraphWalkIterator(Of V)) = iteratorProvider.getGraphWalkIterators(nThreads)

			Dim scheduler As New PriorityScheduler(nThreads)

			Dim list As IList(Of Future(Of Void)) = New List(Of Future(Of Void))(iteratorList.Count)
			'log.info("Fitting Graph with {} threads", Math.max(nThreads,iteratorList.size()));
			For Each iter As GraphWalkIterator(Of V) In iteratorList
				Dim c As New LearningCallable(Me, iter)
				list.Add(scheduler.submit(c))
			Next iter

			scheduler.shutdown() ' wont shutdown till complete

			Try
				FutureUtils.blockTillAllCompleteOrFirstError(list)
			Catch e As InterruptedException
				' should not be possible with blocking till scheduler terminates
				Thread.CurrentThread.Interrupt()
				Throw New Exception(e)
			Catch e As ExecutionException
				Throw New Exception(e)
			End Try
		End Sub

		''' <summary>
		'''Fit the DeepWalk model <b>using a single thread</b> using a given GraphWalkIterator. If parallel fitting is required,
		''' <seealso cref="fit(IGraph, Integer)"/> or <seealso cref="fit(GraphWalkIteratorProvider)"/> should be used.<br>
		''' Note that <seealso cref="initialize(IGraph)"/> or <seealso cref="initialize(Integer[])"/> <em>must</em> be called first.
		''' </summary>
		''' <param name="iterator"> iterator for graph walks </param>
		Public Overridable Sub fit(ByVal iterator As GraphWalkIterator(Of V))
			If Not initCalled Then
				Throw New System.NotSupportedException("DeepWalk not initialized (call initialize before fit)")
			End If
			Dim walkLength As Integer = iterator.walkLength()

			Do While iterator.hasNext()
				Dim sequence As IVertexSequence(Of V) = iterator.next()

				'Skipgram model:
				Dim walk(walkLength) As Integer
				Dim i As Integer = 0
				Do While sequence.MoveNext()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: walk[i++] = sequence.Current.vertexID();
					walk(i) = sequence.Current.vertexID()
						i += 1
				Loop

				skipGram(walk)

				Dim iter As Long = walkCounter.incrementAndGet()
				If iter Mod STATUS_UPDATE_FREQUENCY = 0 Then
					log.info("Processed {} random walks on graph", iter)
				End If
			Loop
		End Sub

		Private Sub skipGram(ByVal walk() As Integer)
			For mid As Integer = windowSize_Conflict To (walk.Length - windowSize_Conflict) - 1
				Dim pos As Integer = mid - windowSize_Conflict
				Do While pos <= mid + windowSize_Conflict
					If pos = mid Then
						pos += 1
						Continue Do
					End If

					'pair of vertices: walk[mid] -> walk[pos]
					lookupTable.iterate(walk(mid), walk(pos))
					pos += 1
				Loop
			Next mid
		End Sub

		Public Overridable Function lookupTable() As GraphVectorLookupTable
			Return lookupTable
		End Function


		Public Class Builder(Of V, E)
'JAVA TO VB CONVERTER NOTE: The field vectorSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend vectorSize_Conflict As Integer = 100
'JAVA TO VB CONVERTER NOTE: The field seed was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend seed_Conflict As Long = DateTimeHelper.CurrentUnixTimeMillis()
'JAVA TO VB CONVERTER NOTE: The field learningRate was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend learningRate_Conflict As Double = 0.01
'JAVA TO VB CONVERTER NOTE: The field windowSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend windowSize_Conflict As Integer = 2

			''' <summary>
			''' Sets the size of the vectors to be learned for each vertex in the graph </summary>
'JAVA TO VB CONVERTER NOTE: The parameter vectorSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function vectorSize(ByVal vectorSize_Conflict As Integer) As Builder(Of V, E)
				Me.vectorSize_Conflict = vectorSize_Conflict
				Return Me
			End Function

			''' <summary>
			''' Set the learning rate </summary>
'JAVA TO VB CONVERTER NOTE: The parameter learningRate was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function learningRate(ByVal learningRate_Conflict As Double) As Builder(Of V, E)
				Me.learningRate_Conflict = learningRate_Conflict
				Return Me
			End Function

			''' <summary>
			''' Sets the window size used in skipgram model </summary>
'JAVA TO VB CONVERTER NOTE: The parameter windowSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function windowSize(ByVal windowSize_Conflict As Integer) As Builder(Of V, E)
				Me.windowSize_Conflict = windowSize_Conflict
				Return Me
			End Function

			''' <summary>
			''' Seed for random number generation (used for repeatability).
			''' Note however that parallel/async gradient descent might result in behaviour that
			''' is not repeatable, in spite of setting seed
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter seed was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function seed(ByVal seed_Conflict As Long) As Builder(Of V, E)
				Me.seed_Conflict = seed_Conflict
				Return Me
			End Function

			Public Overridable Function build() As DeepWalk(Of V, E)
				Dim dw As New DeepWalk(Of V, E)()
				dw.vectorSize = vectorSize_Conflict
				dw.windowSize = windowSize_Conflict
				dw.learningRate = learningRate_Conflict
				dw.seed = seed_Conflict

				Return dw
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor private class LearningCallable implements Callable<Void>
		Private Class LearningCallable
			Implements Callable(Of Void)

			Private ReadOnly outerInstance As DeepWalk(Of V, E)

			Public Sub New(ByVal outerInstance As DeepWalk(Of V, E))
				Me.outerInstance = outerInstance
			End Sub


			Friend ReadOnly iterator As GraphWalkIterator(Of V)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Void call() throws Exception
			Public Overrides Function [call]() As Void
				outerInstance.fit(iterator)

				Return Nothing
			End Function
		End Class
	End Class

End Namespace