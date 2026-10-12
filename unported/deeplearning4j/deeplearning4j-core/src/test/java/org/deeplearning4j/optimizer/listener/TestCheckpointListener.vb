Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Checkpoint = org.deeplearning4j.optimize.listeners.Checkpoint
Imports CheckpointListener = org.deeplearning4j.optimize.listeners.CheckpointListener
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.nd4j.common.primitives
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

Namespace org.deeplearning4j.optimizer.listener


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestCheckpointListener extends org.deeplearning4j.BaseDL4JTest
	Public Class TestCheckpointListener
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000L
			End Get
		End Property


		Private Shared ReadOnly Property NetAndData As Pair(Of MultiLayerNetwork, DataSetIterator)
			Get
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
    
				Dim net As New MultiLayerNetwork(conf)
				net.init()
    
				Dim iter As DataSetIterator = New IrisDataSetIterator(25,50)
    
				Return New Pair(Of MultiLayerNetwork, DataSetIterator)(net, iter)
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCheckpointListenerEvery2Epochs(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCheckpointListenerEvery2Epochs(ByVal tempDir As Path)
			Dim f As File = tempDir.toFile()
			Dim p As Pair(Of MultiLayerNetwork, DataSetIterator) = getNetAndData()
			Dim net As MultiLayerNetwork = p.First
			Dim iter As DataSetIterator = p.Second


			Dim l As CheckpointListener = (New CheckpointListener.Builder(f)).keepAll().saveEveryNEpochs(2).build()
			net.setListeners(l)

			For i As Integer = 0 To 9
				net.fit(iter)

				If i > 0 AndAlso i Mod 2 = 0 Then
					assertEquals(1 + i\2, f.list().length)
				End If
			Next i

			'Expect models saved at end of epochs: 1, 3, 5, 7, 9... (i.e., after 2, 4, 6 etc epochs)
			Dim files() As File = f.listFiles()
			Dim count As Integer = 0
			For Each f2 As File In files
				If Not f2.getPath().EndsWith(".zip") Then
					Continue For
				End If

				Dim prefixLength As Integer = "checkpoint_".Length
				Dim num As Integer = Integer.Parse(f2.getName().Substring(prefixLength, 1))

				Dim n As MultiLayerNetwork = ModelSerializer.restoreMultiLayerNetwork(f2, True)
				Dim expEpoch As Integer = 2 * (num + 1) - 1 'Saved at the end of the previous epoch
				Dim expIter As Integer = (expEpoch+1) * 2 '+1 due to epochs being zero indexed

				assertEquals(expEpoch, n.EpochCount)
				assertEquals(expIter, n.IterationCount)
				count += 1
			Next f2

			assertEquals(5, count)
			assertEquals(5, l.availableCheckpoints().Count)

			Dim listStatic As IList(Of Checkpoint) = CheckpointListener.availableCheckpoints(f)
			assertEquals(5, listStatic.Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCheckpointListenerEvery5Iter(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCheckpointListenerEvery5Iter(ByVal tempDir As Path)
			Dim f As File = tempDir.toFile()
			Dim p As Pair(Of MultiLayerNetwork, DataSetIterator) = getNetAndData()
			Dim net As MultiLayerNetwork = p.First
			Dim iter As DataSetIterator = p.Second


			Dim l As CheckpointListener = (New CheckpointListener.Builder(f)).keepLast(3).saveEveryNIterations(5).build()
			net.setListeners(l)

			For i As Integer = 0 To 19 '40 iterations total
				net.fit(iter)
			Next i

			'Expect models saved at iterations: 5, 10, 15, 20, 25, 30, 35  (training does 0 to 39 here)
			'But: keep only 25, 30, 35
			Dim files() As File = f.listFiles()
			Dim count As Integer = 0
			Dim ns As ISet(Of Integer) = New HashSet(Of Integer)()
			For Each f2 As File In files
				If Not f2.getPath().EndsWith(".zip") Then
					Continue For
				End If
				count += 1
				Dim prefixLength As Integer = "checkpoint_".Length
				Dim num As Integer = Integer.Parse(f2.getName().Substring(prefixLength, 1))

				Dim n As MultiLayerNetwork = ModelSerializer.restoreMultiLayerNetwork(f2, True)
				Dim expIter As Integer = 5 * (num+1)
				assertEquals(expIter, n.IterationCount)

				ns.Add(n.IterationCount)
				count += 1
			Next f2

			assertEquals(3, ns.Count,ns.ToString())
			assertTrue(ns.Contains(25))
			assertTrue(ns.Contains(30))
			assertTrue(ns.Contains(35))

			assertEquals(3, l.availableCheckpoints().Count)

			Dim listStatic As IList(Of Checkpoint) = CheckpointListener.availableCheckpoints(f)
			assertEquals(3, listStatic.Count)

			Dim netStatic As MultiLayerNetwork = CheckpointListener.loadCheckpointMLN(f, 6)
			assertEquals(35, netStatic.IterationCount)

			Dim netStatic2 As MultiLayerNetwork = CheckpointListener.loadLastCheckpointMLN(f)
			assertEquals(35, netStatic2.IterationCount)
			assertEquals(netStatic.params(), netStatic2.params())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCheckpointListenerEveryTimeUnit(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCheckpointListenerEveryTimeUnit(ByVal tempDir As Path)
			Dim f As File = tempDir.toFile()
			Dim p As Pair(Of MultiLayerNetwork, DataSetIterator) = getNetAndData()
			Dim net As MultiLayerNetwork = p.First
			Dim iter As DataSetIterator = p.Second


			Dim l As CheckpointListener = (New CheckpointListener.Builder(f)).keepLast(3).saveEvery(4900, TimeUnit.MILLISECONDS).build()
			net.setListeners(l)

			For i As Integer = 0 To 2 '10 iterations total
				net.fit(iter)
				Thread.Sleep(5000)
			Next i

			'Expect models saved at iterations: 2, 4, 6, 8 (iterations 0 and 1 shoud happen before first 3 seconds is up)
			'But: keep only 5, 7, 9
			Dim files() As File = f.listFiles()
			Dim ns As ISet(Of Integer) = New HashSet(Of Integer)()
			For Each f2 As File In files
				If Not f2.getPath().EndsWith(".zip") Then
					Continue For
				End If

				Dim prefixLength As Integer = "checkpoint_".Length
				Dim num As Integer = Integer.Parse(f2.getName().Substring(prefixLength, 1))

				Dim n As MultiLayerNetwork = ModelSerializer.restoreMultiLayerNetwork(f2, True)
				Dim expIter As Integer = 2 * (num + 1)
				assertEquals(expIter, n.IterationCount)

				ns.Add(n.IterationCount)
			Next f2

			assertEquals(2, l.availableCheckpoints().Count)
			assertEquals(2, ns.Count,ns.ToString())
			Console.WriteLine(ns)
			assertTrue(ns.ContainsAll(Arrays.asList(2,4)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCheckpointListenerKeepLast3AndEvery3(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCheckpointListenerKeepLast3AndEvery3(ByVal tempDir As Path)
			Dim f As File = tempDir.toFile()
			Dim p As Pair(Of MultiLayerNetwork, DataSetIterator) = getNetAndData()
			Dim net As MultiLayerNetwork = p.First
			Dim iter As DataSetIterator = p.Second


			Dim l As CheckpointListener = (New CheckpointListener.Builder(f)).keepLastAndEvery(3, 3).saveEveryNEpochs(2).build()
			net.setListeners(l)

			For i As Integer = 0 To 19 '40 iterations total
				net.fit(iter)
			Next i

			'Expect models saved at end of epochs: 1, 3, 5, 7, 9, 11, 13, 15, 17, 19
			'But: keep only 5, 11, 15, 17, 19
			Dim files() As File = f.listFiles()
			Dim count As Integer = 0
			Dim ns As ISet(Of Integer) = New HashSet(Of Integer)()
			For Each f2 As File In files
				If Not f2.getPath().EndsWith(".zip") Then
					Continue For
				End If
				count += 1
				Dim prefixLength As Integer = "checkpoint_".Length
				Dim [end] As Integer = f2.getName().LastIndexOf("_")
				Dim num As Integer = Integer.Parse(f2.getName().Substring(prefixLength, [end] - prefixLength))

				Dim n As MultiLayerNetwork = ModelSerializer.restoreMultiLayerNetwork(f2, True)
				Dim expEpoch As Integer = 2 * (num+1) - 1
				assertEquals(expEpoch, n.EpochCount)

				ns.Add(n.EpochCount)
				count += 1
			Next f2

			assertEquals(5, ns.Count,ns.ToString())
			assertTrue(ns.ContainsAll(Arrays.asList(5, 11, 15, 17, 19)),ns.ToString())

			assertEquals(5, l.availableCheckpoints().Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDeleteExisting(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDeleteExisting(ByVal tempDir As Path)
			Dim f As File = tempDir.toFile()
			Dim p As Pair(Of MultiLayerNetwork, DataSetIterator) = getNetAndData()
			Dim net As MultiLayerNetwork = p.First
			Dim iter As DataSetIterator = p.Second


			Dim l As CheckpointListener = (New CheckpointListener.Builder(f)).keepAll().saveEveryNEpochs(1).build()
			net.setListeners(l)

			For i As Integer = 0 To 2
				net.fit(iter)
			Next i

			'Now, create new listener:
			Try
				l = (New CheckpointListener.Builder(f)).keepAll().saveEveryNEpochs(1).build()
				fail("Expected exception")
			Catch e As System.InvalidOperationException
				assertTrue(e.Message.contains("Use deleteExisting(true)"))
			End Try

			l = (New CheckpointListener.Builder(f)).keepAll().saveEveryNEpochs(1).deleteExisting(True).build()
			net.setListeners(l)

			net.fit(iter)

			Dim fList() As File = f.listFiles() 'checkpoint meta file + 1 checkpoint
			assertNotNull(fList)
			assertEquals(2, fList.Length)
		End Sub
	End Class

End Namespace