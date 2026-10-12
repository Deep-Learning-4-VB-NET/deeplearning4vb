Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports org.deeplearning4j.nn.conf.layers
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports org.junit.jupiter.api
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports Evaluation = org.deeplearning4j.eval.Evaluation
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports InferenceMode = org.deeplearning4j.parallelism.inference.InferenceMode
Imports InferenceObservable = org.deeplearning4j.parallelism.inference.InferenceObservable
Imports BasicInferenceObserver = org.deeplearning4j.parallelism.inference.observers.BasicInferenceObserver
Imports BatchedInferenceObservable = org.deeplearning4j.parallelism.inference.observers.BatchedInferenceObservable
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives
Imports Resources = org.nd4j.common.resources.Resources
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

Namespace org.deeplearning4j.parallelism


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class ParallelInferenceTest extends org.deeplearning4j.BaseDL4JTest
	Public Class ParallelInferenceTest
		Inherits BaseDL4JTest

		Private Shared model As MultiLayerNetwork
		Private Shared iterator As DataSetIterator



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()
			If model Is Nothing Then
				Dim file As File = Resources.asFile("models/LenetMnistMLN.zip")
				model = ModelSerializer.restoreMultiLayerNetwork(file, True)

				iterator = New MnistDataSetIterator(1, False, 12345)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void tearDown() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub tearDown()
			iterator.reset()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(30000) public void testInferenceSequential1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInferenceSequential1()

			Dim count0 As Long = 0
			Dim count1 As Long = 0

			'We can't guarantee that on any particular run each thread will get data - it might randomly be assigned to
			' only one. Consequently: we'll run the test multiple times and ensure that in at least *some* of the test
			' runs both workers get some data.
			Dim i As Integer = 0
			Do While i < 20 AndAlso (count0 = 0 OrElse count1 = 0)
				iterator = New MnistDataSetIterator(1, False, 12345)

				Dim inf As ParallelInference = (New ParallelInference.Builder(model)).inferenceMode(InferenceMode.SEQUENTIAL).workers(2).build()

				Try

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					log.info("Features shape: {}", java.util.Arrays.toString(iterator.next().getFeatures().shapeInfoDataBuffer().asInt()))

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim array1 As INDArray = inf.output(iterator.next().getFeatures())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim array2 As INDArray = inf.output(iterator.next().getFeatures())

					assertFalse(array1.Attached)
					assertFalse(array2.Attached)

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim array3 As INDArray = inf.output(iterator.next().getFeatures())
					assertFalse(array3.Attached)

					iterator.reset()

					evalClassifcationSingleThread(inf, iterator)

					count0 = inf.getWorkerCounter(0)
					count1 = inf.getWorkerCounter(1)
	'            System.out.println("Counts: " + count0 + ", " + count1);
				Finally
					inf.shutdown()
				End Try
				i += 1
			Loop
			' both workers threads should have non-zero
			assertTrue(count0 > 0L)
			assertTrue(count1 > 0L)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(30000) public void testInferenceSequential2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInferenceSequential2()

			Dim count0 As Long = 0
			Dim count1 As Long = 0

			'We can't guarantee that on any particular run each thread will get data - it might randomly be assigned to
			' only one. Consequently: we'll run the test multiple times and ensure that in at least *some* of the test
			' runs both workers get some data.
			Dim i As Integer = 0
			Do While i < 20 AndAlso (count0 = 0 OrElse count1 = 0)
				iterator = New MnistDataSetIterator(1, False, 12345)
				Dim inf As ParallelInference = (New ParallelInference.Builder(model)).inferenceMode(InferenceMode.SEQUENTIAL).workers(2).build()

				Try

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					log.info("Features shape: {}", java.util.Arrays.toString(iterator.next().getFeatures().shapeInfoDataBuffer().asInt()))

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim array1 As INDArray = inf.output(iterator.next().getFeatures())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim array2 As INDArray = inf.output(iterator.next().getFeatures())

					assertFalse(array1.Attached)
					assertFalse(array2.Attached)

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim array3 As INDArray = inf.output(iterator.next().getFeatures())
					assertFalse(array3.Attached)

					iterator.reset()

					evalClassifcationMultipleThreads(inf, iterator, 10)

					' both workers threads should have non-zero
					count0 = inf.getWorkerCounter(0)
					count1 = inf.getWorkerCounter(1)
	'            System.out.println("Counts: " + count0 + ", " + count1);
				Finally
					inf.shutdown()
				End Try
				i += 1
			Loop
			assertTrue(count0 > 0L)
			assertTrue(count1 > 0L)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(30000) public void testInferenceBatched1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInferenceBatched1()
			Dim count0 As Long = 0
			Dim count1 As Long = 0

			'We can't guarantee that on any particular run each thread will get data - it might randomly be assigned to
			' only one. Consequently: we'll run the test multiple times and ensure that in at least *some* of the test
			' runs both workers get some data.
			Dim i As Integer=0
			Do While i<20 AndAlso (count0 = 0 OrElse count1 = 0)
				Dim inf As ParallelInference = (New ParallelInference.Builder(model)).inferenceMode(InferenceMode.BATCHED).batchLimit(8).workers(2).build()
				Try

					iterator = New MnistDataSetIterator(1, False, 12345)


'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					log.info("Features shape: {}", java.util.Arrays.toString(iterator.next().getFeatures().shapeInfoDataBuffer().asInt()))

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim array1 As INDArray = inf.output(iterator.next().getFeatures())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim array2 As INDArray = inf.output(iterator.next().getFeatures())

					assertFalse(array1.Attached)
					assertFalse(array2.Attached)

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim array3 As INDArray = inf.output(iterator.next().getFeatures())
					assertFalse(array3.Attached)

					iterator.reset()

					evalClassifcationMultipleThreads(inf, iterator, 10)

					' both workers threads should have non-zero
					count0 = inf.getWorkerCounter(0)
					count1 = inf.getWorkerCounter(1)
	'            System.out.println("Counts: " + count0 + ", " + count1);
				Finally
					inf.shutdown()
				End Try
				i += 1
			Loop
			assertTrue(count0 > 0L)
			assertTrue(count1 > 0L)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testProvider1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testProvider1()
			Dim queue As New LinkedBlockingQueue()
			Dim observer As New BasicInferenceObserver()

			Dim provider As New ParallelInference.ObservablesProvider(10000000L, 100, queue)

			Dim observable1 As InferenceObservable = provider.setInput(observer, Nd4j.create(1, 100))
			Dim observable2 As InferenceObservable = provider.setInput(observer, Nd4j.create(1, 100))

			assertNotEquals(Nothing, observable1)

			assertTrue(observable1 Is observable2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testProvider2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testProvider2()
			Dim queue As New LinkedBlockingQueue()
			Dim observer As New BasicInferenceObserver()
			Dim provider As New ParallelInference.ObservablesProvider(10000000L, 100, queue)

			Dim observable1 As InferenceObservable = provider.setInput(observer, Nd4j.create(1,100).assign(1.0))
			Dim observable2 As InferenceObservable = provider.setInput(observer, Nd4j.create(1,100).assign(2.0))

			assertNotEquals(Nothing, observable1)

			assertTrue(observable1 Is observable2)

			Dim l As IList(Of Pair(Of INDArray(), INDArray())) = observable1.getInputBatches()
			assertEquals(1, l.Count)
			Dim input() As INDArray = l(0).getFirst()
			assertNull(l(0).getSecond())

			assertEquals(1, input.Length)
			assertArrayEquals(New Long() {2, 100}, input(0).shape())
			assertEquals(1.0f, input(0).tensorAlongDimension(0, 1).meanNumber().floatValue(), 0.001)
			assertEquals(2.0f, input(0).tensorAlongDimension(1, 1).meanNumber().floatValue(), 0.001)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testProvider3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testProvider3()
			Dim queue As New LinkedBlockingQueue()
			Dim observer As New BasicInferenceObserver()
			Dim provider As New ParallelInference.ObservablesProvider(10000000L, 2, queue)

			Dim observable1 As InferenceObservable = provider.setInput(observer, Nd4j.create(1,100).assign(1.0))
			Dim observable2 As InferenceObservable = provider.setInput(observer, Nd4j.create(1,100).assign(2.0))

			Dim observable3 As InferenceObservable = provider.setInput(observer, Nd4j.create(1,100).assign(3.0))


			assertNotEquals(Nothing, observable1)
			assertNotEquals(Nothing, observable3)

			assertTrue(observable1 Is observable2)
			assertTrue(observable1 IsNot observable3)

			Dim l As IList(Of Pair(Of INDArray(), INDArray())) = observable1.getInputBatches()
			assertEquals(1, l.Count)
			Dim input() As INDArray = l(0).getFirst()
			assertNull(l(0).getSecond())

			assertEquals(1.0f, input(0).tensorAlongDimension(0, 1).meanNumber().floatValue(), 0.001)
			assertEquals(2.0f, input(0).tensorAlongDimension(1, 1).meanNumber().floatValue(), 0.001)


			l = observable3.getInputBatches()
			assertEquals(1, l.Count)
			input = l(0).getFirst()
			assertNull(l(0).getSecond())
			assertEquals(3.0f, input(0).tensorAlongDimension(0, 1).meanNumber().floatValue(), 0.001)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testProvider4() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testProvider4()
			Dim queue As New LinkedBlockingQueue()
			Dim observer As New BasicInferenceObserver()
			Dim provider As New ParallelInference.ObservablesProvider(10000000L, 4, queue)

			Dim observable1 As BatchedInferenceObservable = CType(provider.setInput(observer, Nd4j.create(1,100).assign(1.0)), BatchedInferenceObservable)
			Dim observable2 As BatchedInferenceObservable = CType(provider.setInput(observer, Nd4j.create(1,100).assign(2.0)), BatchedInferenceObservable)
			Dim observable3 As BatchedInferenceObservable = CType(provider.setInput(observer, Nd4j.create(1,100).assign(3.0)), BatchedInferenceObservable)

			Dim bigOutput As INDArray = Nd4j.create(3, 10)
			Dim i As Integer = 0
			Do While i < bigOutput.rows()
				bigOutput.getRow(i).assign(CSng(i))
				i += 1
			Loop


			Dim f As System.Reflection.FieldInfo = GetType(BatchedInferenceObservable).getDeclaredField("outputBatchInputArrays")
			f.setAccessible(True)
			Dim l As IList(Of Integer()) = New List(Of Integer())()
			l.Add(New Integer(){0, 2})
			f.set(observable3, l)

			f = GetType(BatchedInferenceObservable).getDeclaredField("inputs")
			f.setAccessible(True)
			f.set(observable3, java.util.Arrays.asList(New INDArray(){bigOutput.getRow(0, True)}, New INDArray(){bigOutput.getRow(1, True)}, New INDArray(){bigOutput.getRow(2, True)}))


			observable3.OutputBatches = Collections.singletonList(New INDArray(){bigOutput})
			Dim [out] As INDArray = Nothing

			observable3.Position = 0
			[out] = observable3.Output(0)
			assertArrayEquals(New Long() {1, 10}, [out].shape())
			assertEquals(0.0f, [out].meanNumber().floatValue(), 0.01f)

			observable3.Position = 1
			[out] = observable3.Output(0)
			assertArrayEquals(New Long() {1, 10}, [out].shape())
			assertEquals(1.0f, [out].meanNumber().floatValue(), 0.01f)

			observable3.Position = 2
			[out] = observable3.Output(0)
			assertArrayEquals(New Long() {1, 10}, [out].shape())
			assertEquals(2.0f, [out].meanNumber().floatValue(), 0.01f)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected void evalClassifcationSingleThread(@NonNull ParallelInference inf, @NonNull DataSetIterator iterator)
		Protected Friend Overridable Sub evalClassifcationSingleThread(ByVal inf As ParallelInference, ByVal iterator As DataSetIterator)
			Dim ds As DataSet = iterator.next()
			log.info("NumColumns: {}", ds.Labels.columns())
			iterator.reset()
			Dim eval As New Evaluation(ds.Labels.columns())
			Dim count As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: while (iterator.hasNext() && (count++ < 100))
			Do While iterator.hasNext() AndAlso (count++ < 100)
				ds = iterator.next()
				Dim output As INDArray = inf.output(ds.Features)
				eval.eval(ds.Labels, output)
			Loop
			log.info(eval.stats())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected void evalClassifcationMultipleThreads(@NonNull ParallelInference inf, @NonNull DataSetIterator iterator, int numThreads) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Protected Friend Overridable Sub evalClassifcationMultipleThreads(ByVal inf As ParallelInference, ByVal iterator As DataSetIterator, ByVal numThreads As Integer)
			Dim ds As DataSet = iterator.next()
			log.info("NumColumns: {}", ds.Labels.columns())
			iterator.reset()
			Dim eval As New Evaluation(ds.Labels.columns())
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Queue<org.nd4j.linalg.dataset.DataSet> dataSets = new java.util.concurrent.LinkedBlockingQueue<>();
			Dim dataSets As LinkedList(Of DataSet) = New LinkedBlockingQueue(Of DataSet)()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Queue<org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray, org.nd4j.linalg.api.ndarray.INDArray>> outputs = new java.util.concurrent.LinkedBlockingQueue<>();
			Dim outputs As LinkedList(Of Pair(Of INDArray, INDArray)) = New LinkedBlockingQueue(Of Pair(Of INDArray, INDArray))()
			Dim cnt As Integer = 0
			' first of all we'll build datasets
			Do While iterator.hasNext() AndAlso cnt < 256
				ds = iterator.next()
				dataSets.AddLast(ds)
				cnt += 1
			Loop

			' now we'll build outputs in parallel
			Dim threads(numThreads - 1) As Thread
			For i As Integer = 0 To numThreads - 1
				threads(i) = New Thread(Sub()
				Dim ds As DataSet
				ds = dataSets.RemoveFirst()
				Do While ds IsNot Nothing
					Dim output As INDArray = inf.output(ds)
					outputs.AddLast(Pair.makePair(ds.Labels, output))
					ds = dataSets.RemoveFirst()
				Loop
				End Sub)
			Next i

			For i As Integer = 0 To numThreads - 1
				threads(i).Start()
			Next i

			For i As Integer = 0 To numThreads - 1
				threads(i).Join()
			Next i

			' and now we'll evaluate in single thread once again
			Dim output As Pair(Of INDArray, INDArray)
			output = outputs.RemoveFirst()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((output = outputs.poll()) != null)
			Do While output IsNot Nothing
				eval.eval(output.First, output.Second)
					output = outputs.RemoveFirst()
			Loop
			log.info(eval.stats())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(120000) public void testParallelInferenceVariableLengthTS() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testParallelInferenceVariableLengthTS()
			Nd4j.Random.setSeed(12345)

			Dim nIn As Integer = 10
			Dim tsLengths() As Integer = {3, 5, 7, 10, 50, 100}

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).seed(12345).list().layer((New LSTM.Builder()).nIn(nIn).nOut(5).build()).layer((New RnnOutputLayer.Builder()).nIn(5).nOut(5).activation(Activation.SOFTMAX).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			For Each m As InferenceMode In System.Enum.GetValues(GetType(InferenceMode))
				For Each w As Integer In New Integer(){1, 2}

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ParallelInference inf = new ParallelInference.Builder(net).inferenceMode(m).batchLimit(20).queueLimit(64).workers(w).build();
					Dim inf As ParallelInference = (New ParallelInference.Builder(net)).inferenceMode(m).batchLimit(20).queueLimit(64).workers(w).build()
					Try

						Dim arrs As IList(Of INDArray) = New List(Of INDArray)()
						Dim exp As IList(Of INDArray) = New List(Of INDArray)()
						For Each l As Integer In tsLengths
							Dim [in] As INDArray = Nd4j.rand(New Integer(){1, nIn, l})
							arrs.Add([in])
							Dim [out] As INDArray = net.output([in])
							exp.Add([out])
						Next l

						testParallelInference(inf, arrs, exp)
					Finally
						inf.shutdown()
					End Try
				Next w
			Next m
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(120000) public void testParallelInferenceVariableLengthTS2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testParallelInferenceVariableLengthTS2()
			Nd4j.Random.setSeed(12345)

			Dim nIn As Integer = 10

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).seed(12345).list().layer((New LSTM.Builder()).nIn(nIn).nOut(5).build()).layer((New RnnOutputLayer.Builder()).nIn(5).nOut(5).activation(Activation.SOFTMAX).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim defaultSize() As Integer = {1, 10, 5}

			For Each m As InferenceMode In System.Enum.GetValues(GetType(InferenceMode))
				For Each w As Integer In New Integer(){2, 3}

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ParallelInference inf = new ParallelInference.Builder(net).inferenceMode(m).batchLimit(20).queueLimit(64).workers(w).build();
					Dim inf As ParallelInference = (New ParallelInference.Builder(net)).inferenceMode(m).batchLimit(20).queueLimit(64).workers(w).build()
					Try

						Dim arrs As IList(Of INDArray) = New List(Of INDArray)()
						Dim exp As IList(Of INDArray) = New List(Of INDArray)()

						Dim r As New Random()
						Dim runs As Integer = If(IntegrationTests, 500, 30)
						For i As Integer = 0 To runs - 1
							Dim shape() As Integer = defaultSize
							If r.NextDouble() < 0.4 Then
								shape = New Integer(){r.Next(5) + 1, 10, r.Next(10) + 1}
							End If

							Dim [in] As INDArray = Nd4j.rand(shape)
							arrs.Add([in])
							Dim [out] As INDArray = net.output([in])
							exp.Add([out])
						Next i
						testParallelInference(inf, arrs, exp)
					Finally
						inf.shutdown()
					End Try
				Next w
			Next m
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(30000) public void testParallelInferenceVariableSizeCNN() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testParallelInferenceVariableSizeCNN()
			'Variable size input for CNN model - for example, YOLO models
			'In these cases, we can't batch and have to execute the different size inputs separately

			Nd4j.Random.setSeed(12345)

			Dim nIn As Integer = 3
			Dim shapes()() As Integer = {
				New Integer() {1, nIn, 10, 10},
				New Integer() {1, nIn, 10, 15},
				New Integer() {1, nIn, 20, 15},
				New Integer() {1, nIn, 20, 20},
				New Integer() {1, nIn, 30, 30},
				New Integer() {1, nIn, 40, 40},
				New Integer() {1, nIn, 40, 45}
			}

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).seed(12345).list().layer((New ConvolutionLayer.Builder()).nIn(nIn).nOut(5).build()).layer((New CnnLossLayer.Builder()).activation(Activation.SOFTMAX).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			For Each m As InferenceMode In System.Enum.GetValues(GetType(InferenceMode))
				For Each w As Integer In New Integer(){1, 2}

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ParallelInference inf = new ParallelInference.Builder(net).inferenceMode(m).batchLimit(20).queueLimit(64).workers(w).build();
					Dim inf As ParallelInference = (New ParallelInference.Builder(net)).inferenceMode(m).batchLimit(20).queueLimit(64).workers(w).build()

					Dim arrs As IList(Of INDArray) = New List(Of INDArray)()
					Dim exp As IList(Of INDArray) = New List(Of INDArray)()
					For Each shape As Integer() In shapes
						Dim [in] As INDArray = Nd4j.rand(shape)
						arrs.Add([in])
						Dim [out] As INDArray = net.output([in])
						exp.Add([out])
					Next shape

					testParallelInference(inf, arrs, exp)

					inf.shutdown()
				Next w
			Next m
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(30000) public void testParallelInferenceVariableSizeCNN2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testParallelInferenceVariableSizeCNN2()
			'Variable size input for CNN model - for example, YOLO models
			'In these cases, we can't batch and have to execute the different size inputs separately

			Nd4j.Random.setSeed(12345)

			Dim nIn As Integer = 3
			Dim defaultShape() As Integer = {1, nIn, 16, 16}

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).seed(12345).convolutionMode(ConvolutionMode.Same).list().layer((New ConvolutionLayer.Builder()).nIn(nIn).nOut(5).build()).layer((New CnnLossLayer.Builder()).activation(Activation.SOFTMAX).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			For Each m As InferenceMode In System.Enum.GetValues(GetType(InferenceMode))
				For Each w As Integer In New Integer(){1, 2}

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ParallelInference inf = new ParallelInference.Builder(net).inferenceMode(m).batchLimit(20).queueLimit(64).workers(w).build();
					Dim inf As ParallelInference = (New ParallelInference.Builder(net)).inferenceMode(m).batchLimit(20).queueLimit(64).workers(w).build()

					Dim arrs As IList(Of INDArray) = New List(Of INDArray)()
					Dim exp As IList(Of INDArray) = New List(Of INDArray)()
					Dim r As New Random()
					Dim runs As Integer = If(IntegrationTests, 500, 20)
					For i As Integer = 0 To runs - 1
						Dim shape() As Integer = defaultShape
						If r.NextDouble() < 0.4 Then
							shape = New Integer(){r.Next(5)+1, nIn, 10, r.Next(10)+1}
						End If

						Dim [in] As INDArray = Nd4j.rand(shape)
						arrs.Add([in])
						Dim [out] As INDArray = net.output([in])
						exp.Add([out])
					Next i
					testParallelInference(inf, arrs, exp)

					inf.shutdown()
				Next w
			Next m
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(20000) public void testParallelInferenceErrorPropagation()
		Public Overridable Sub testParallelInferenceErrorPropagation()

			Dim nIn As Integer = 10
			Dim wrongNIn As Integer = 5

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).seed(12345).list().layer((New DenseLayer.Builder()).nIn(nIn).nOut(5).build()).layer((New OutputLayer.Builder()).nIn(5).nOut(5).activation(Activation.SOFTMAX).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim inOk As INDArray = Nd4j.ones(1, nIn)
			Dim inWrong As INDArray = Nd4j.ones(1, wrongNIn)

			Dim expOk As INDArray = net.output(inOk)

			For Each m As InferenceMode In System.Enum.GetValues(GetType(InferenceMode))
				For Each w As Integer In New Integer(){1, 2}
					log.info("Starting: m={}, w={}", m, w)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ParallelInference inf = new ParallelInference.Builder(net).inferenceMode(m).batchLimit(20).queueLimit(64).workers(w).build();
					Dim inf As ParallelInference = (New ParallelInference.Builder(net)).inferenceMode(m).batchLimit(20).queueLimit(64).workers(w).build()

					Dim actOk As INDArray = inf.output(inOk)
					assertEquals(expOk, actOk)

					Try
						inf.output(inWrong)
						fail("Expected exception")
					Catch e As DL4JInvalidInputException
						'OK
						Console.WriteLine("Expected exception: " & e.Message)
					Catch e As Exception
						log.error("",e)
						fail("Expected other exception type")
					End Try

					actOk = inf.output(inOk)
					assertEquals(expOk, actOk)

					inf.shutdown()
				Next w
			Next m
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInputMaskingCyclic() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInputMaskingCyclic()
			For e As Integer = 0 To 2
				testInputMasking()
				log.info("Iteration: {} finished", e)
				System.GC.Collect()
			Next e
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void testInputMasking() throws Exception
		Private Sub testInputMasking()
			Nd4j.Random.setSeed(12345)

			Dim nIn As Integer = 10
			Dim tsLength As Integer = 16

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).seed(12345).list().layer((New LSTM.Builder()).nIn(nIn).nOut(5).build()).layer(New GlobalPoolingLayer(PoolingType.AVG)).layer((New OutputLayer.Builder()).nIn(5).nOut(5).activation(Activation.SOFTMAX).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

	'        InferenceMode[] inferenceModes = new InferenceMode[]{InferenceMode.SEQUENTIAL, InferenceMode.BATCHED, InferenceMode.INPLACE, InferenceMode.SEQUENTIAL};
	'        int[] workers = new int[]{2, 2, 2, 1};
	'        boolean[] randomTS = new boolean[]{true, false, true, false};

			Dim r As New Random()
			For Each m As InferenceMode In System.Enum.GetValues(GetType(InferenceMode))
				log.info("Testing inference mode: [{}]", m)
				For Each w As Integer In New Integer(){1, 2}
					For Each randomTSLength As Boolean In New Boolean(){False, True}
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ParallelInference inf = new ParallelInference.Builder(net).inferenceMode(m).batchLimit(5).queueLimit(64).workers(w).build();
						Dim inf As ParallelInference = (New ParallelInference.Builder(net)).inferenceMode(m).batchLimit(5).queueLimit(64).workers(w).build()

						Dim [in] As IList(Of INDArray) = New List(Of INDArray)()
						Dim inMasks As IList(Of INDArray) = New List(Of INDArray)()
						Dim exp As IList(Of INDArray) = New List(Of INDArray)()
						Dim nRuns As Integer = If(IntegrationTests, 100, 10)
						For i As Integer = 0 To nRuns - 1
							Dim currTSLength As Integer = (If(randomTSLength, 1 + r.Next(tsLength), tsLength))
							Dim currNumEx As Integer = 1 + r.Next(3)
							Dim inArr As INDArray = Nd4j.rand(New Integer(){currNumEx, nIn, currTSLength})
							[in].Add(inArr)

							Dim inMask As INDArray = Nothing
							If r.NextDouble() < 0.5 Then
								inMask = Nd4j.ones(currNumEx, currTSLength)
								For mb As Integer = 0 To currNumEx - 1
									If currTSLength > 1 Then
										Dim firstMaskedStep As Integer = 1 + r.Next(currTSLength)
										For j As Integer = firstMaskedStep To currTSLength - 1
											inMask.putScalar(mb, j, 0.0)
										Next j
									End If
								Next mb
							End If
							inMasks.Add(inMask)

							Dim [out] As INDArray = net.output(inArr, False, inMask, Nothing)
							exp.Add([out])
						Next i

						testParallelInference(inf, [in], inMasks, exp)

						inf.shutdown()
					Next randomTSLength
				Next w
			Next m
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(20000) public void testModelUpdate_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testModelUpdate_1()
			Dim nIn As Integer = 5

			Dim conf As val = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("out0", (New OutputLayer.Builder()).nIn(nIn).nOut(4).activation(Activation.SOFTMAX).build(), "in").layer("out1", (New OutputLayer.Builder()).nIn(nIn).nOut(6).activation(Activation.SOFTMAX).build(), "in").setOutputs("out0", "out1").build()

			Dim net As New ComputationGraph(conf)
			net.init()

			Dim inf As val = (New ParallelInference.Builder(net)).inferenceMode(InferenceMode.SEQUENTIAL).batchLimit(5).queueLimit(64).workers(4).build()

			' imitating use of the original model
			For e As Integer = 0 To 9
				Dim output As val = inf.output(New INDArray(){Nd4j.createUninitialized(1, 5)})
				assertNotNull(output)
				assertNotEquals(0, output.length)
			Next e

			Dim modelsBefore() As Model = inf.getCurrentModelsFromWorkers()
			assertEquals(4, modelsBefore.Length)

			Dim passed As Boolean = False
			Dim cnt0 As Integer = 0
			For Each m As Model In modelsBefore
				' model can be null for some of the workers yet, due to race condition
				If m IsNot Nothing Then
					Thread.Sleep(500)
					assertEquals(net.params(), m.params(), "Failed at model [" & cnt0 & "]")
					passed = True
				End If
				cnt0 += 1
			Next m
			assertTrue(passed)


			Dim conf2 As val = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("out0", (New OutputLayer.Builder()).nIn(nIn).nOut(4).build(), "in").layer("out1", (New OutputLayer.Builder()).nIn(nIn).nOut(6).build(), "in").layer("out2", (New OutputLayer.Builder()).nIn(nIn).nOut(8).build(), "in").setOutputs("out0", "out1", "out2").build()

			Dim net2 As val = New ComputationGraph(conf2)
			net2.init()

			inf.updateModel(net2)

			Dim modelsAfter As val = inf.getCurrentModelsFromWorkers()
			assertEquals(4, modelsAfter.length)

			cnt0 = 0
			For Each m As val In modelsAfter
				assertNotNull(m,"Failed at model [" & cnt0 & "]")
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: assertEquals(net2.params(), m.params(), "Failed at model [" + cnt0++ + "]");
				assertEquals(net2.params(), m.params(), "Failed at model [" & cnt0 & "]")
					cnt0 += 1
			Next m

			inf.shutdown()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(120000) public void testMultiOutputNet() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultiOutputNet()

			Dim nIn As Integer = 5

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("out0", (New OutputLayer.Builder()).nIn(nIn).nOut(4).activation(Activation.SOFTMAX).build(), "in").layer("out1", (New OutputLayer.Builder()).nIn(nIn).nOut(6).activation(Activation.SOFTMAX).build(), "in").setOutputs("out0", "out1").build()

			Dim net As New ComputationGraph(conf)
			net.init()

			Dim r As New Random()
			For Each m As InferenceMode In System.Enum.GetValues(GetType(InferenceMode))
				For Each w As Integer In New Integer(){1, 2}

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ParallelInference inf = new ParallelInference.Builder(net).inferenceMode(m).batchLimit(5).queueLimit(64).workers(w).build();
					Dim inf As ParallelInference = (New ParallelInference.Builder(net)).inferenceMode(m).batchLimit(5).queueLimit(64).workers(w).build()

					Dim [in] As IList(Of INDArray()) = New List(Of INDArray())()
					Dim exp As IList(Of INDArray()) = New List(Of INDArray())()
					Dim runs As Integer = If(IntegrationTests, 100, 20)
					For i As Integer = 0 To 99
						Dim currNumEx As Integer = 1 + r.Next(3)
						Dim inArr As INDArray = Nd4j.rand(New Integer(){currNumEx, nIn})
						[in].Add(New INDArray(){inArr})

						Dim [out]() As INDArray = net.output(inArr)
						exp.Add([out])
					Next i

					testParallelInferenceMulti(inf, [in], Nothing, exp)
					inf.shutdown()
				Next w
			Next m

		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void testParallelInference(ParallelInference inf, List<org.nd4j.linalg.api.ndarray.INDArray> in, List<org.nd4j.linalg.api.ndarray.INDArray> exp) throws Exception
		Private Shared Sub testParallelInference(ByVal inf As ParallelInference, ByVal [in] As IList(Of INDArray), ByVal exp As IList(Of INDArray))
			testParallelInference(inf, [in], Nothing, exp)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void testParallelInference(ParallelInference inf, List<org.nd4j.linalg.api.ndarray.INDArray> in, List<org.nd4j.linalg.api.ndarray.INDArray> inMasks, List<org.nd4j.linalg.api.ndarray.INDArray> exp) throws Exception
		Private Shared Sub testParallelInference(ByVal inf As ParallelInference, ByVal [in] As IList(Of INDArray), ByVal inMasks As IList(Of INDArray), ByVal exp As IList(Of INDArray))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray[] act = new org.nd4j.linalg.api.ndarray.INDArray[in.size()];
			Dim act([in].Count - 1) As INDArray
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger counter = new java.util.concurrent.atomic.AtomicInteger(0);
			Dim counter As New AtomicInteger(0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger failedCount = new java.util.concurrent.atomic.AtomicInteger(0);
			Dim failedCount As New AtomicInteger(0)

			Dim q As LinkedList(Of Triple(Of INDArray, INDArray, Integer)) = New ConcurrentLinkedQueue(Of Triple(Of INDArray, INDArray, Integer))()
			For i As Integer = 0 To [in].Count - 1
				Dim f As INDArray = [in](i)
				Dim m As INDArray = (If(inMasks Is Nothing, Nothing, inMasks(i)))
	'            INDArray e = exp.get(i);
				q.AddLast(New Triple(Of )(f,m,i))
			Next i

			Dim nThreads As Integer = 8
			Dim threads As val = New List(Of Thread)(nThreads)

			For i As Integer = 0 To nThreads - 1
				Dim t As val = New Thread(Sub()
				Do While q.Count > 0
					Try
						Dim t As Triple(Of INDArray, INDArray, Integer) = q.RemoveFirst()
						If t Is Nothing Then 'May be null if other thread gets last element between isEmpty and poll calls
							Continue Do
						End If
						counter.incrementAndGet()
						Dim idx As Integer = t.getRight()
						act(idx) = inf.output(t.getFirst(), t.getSecond())
					Catch e As Exception
						log.error("",e)
						failedCount.incrementAndGet()
					End Try
				Loop
				End Sub)

				t.start()

				threads.add(t)
			Next i

			' wait for ALL started threads
			For Each t As val In threads
				If failedCount.get() > 0 Then
					Throw New Exception("One of threads failed!")
				End If
				t.join()
			Next t

			assertEquals(0, failedCount.get())
			assertEquals([in].Count, counter.get())
			For i As Integer = 0 To [in].Count - 1
				Dim e As INDArray = exp(i)
				Dim a As INDArray = act(i)

	'            float[] fe = e.dup().data().asFloat();
	'            float[] fa = a.dup().data().asFloat();
	'            System.out.println(Arrays.toString(fe));
	'            System.out.println(Arrays.toString(fa));
	'            assertArrayEquals(fe, fa, 1e-8f);
	'            System.out.println(Arrays.toString(e.shape()) + " vs " + Arrays.toString(a.shape()));
	'            assertArrayEquals(e.shape(), a.shape());

				assertEquals(e, a, "Failed at iteration [" & i & "]")
			Next i
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void testParallelInferenceMulti(ParallelInference inf, List<org.nd4j.linalg.api.ndarray.INDArray[]> in, List<org.nd4j.linalg.api.ndarray.INDArray[]> inMasks, List<org.nd4j.linalg.api.ndarray.INDArray[]> exp) throws Exception
		Private Shared Sub testParallelInferenceMulti(ByVal inf As ParallelInference, ByVal [in] As IList(Of INDArray()), ByVal inMasks As IList(Of INDArray()), ByVal exp As IList(Of INDArray()))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray[][] act = new org.nd4j.linalg.api.ndarray.INDArray[in.size()][0];
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim act[][] As INDArray = new INDArray[in.Count][0]
			Dim act()() As INDArray = RectangularArrays.RectangularINDArrayArray([in].Count, 0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger counter = new java.util.concurrent.atomic.AtomicInteger(0);
			Dim counter As New AtomicInteger(0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger failedCount = new java.util.concurrent.atomic.AtomicInteger(0);
			Dim failedCount As New AtomicInteger(0)

			For i As Integer = 0 To [in].Count - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int j=i;
				Dim j As Integer=i
				Call (New Thread(Sub()
				Try
					Dim inMask() As INDArray = (If(inMasks Is Nothing, Nothing, inMasks(j)))
					act(j) = inf.output([in](j), inMask)
					counter.incrementAndGet()
				Catch e As Exception
					log.error("",e)
					failedCount.incrementAndGet()
				End Try
				End Sub)).Start()
			Next i

			Dim start As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim current As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Do While current < start + 20000 AndAlso failedCount.get() = 0 AndAlso counter.get() < [in].Count
				Thread.Sleep(1000L)
			Loop

			assertEquals(0, failedCount.get())
			assertEquals([in].Count, counter.get())
			For i As Integer = 0 To [in].Count - 1
				Dim e() As INDArray = exp(i)
				Dim a() As INDArray = act(i)

				assertArrayEquals(e, a)
			Next i
		End Sub
	End Class

End Namespace