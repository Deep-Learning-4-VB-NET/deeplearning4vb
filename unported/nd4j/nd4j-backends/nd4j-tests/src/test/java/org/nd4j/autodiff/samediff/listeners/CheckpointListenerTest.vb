Imports System.Collections.Generic
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports Execution = org.junit.jupiter.api.parallel.Execution
Imports ExecutionMode = org.junit.jupiter.api.parallel.ExecutionMode
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports CheckpointListener = org.nd4j.autodiff.listeners.checkpoint.CheckpointListener
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports TrainingConfig = org.nd4j.autodiff.samediff.TrainingConfig
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports IrisDataSetIterator = org.nd4j.linalg.dataset.IrisDataSetIterator
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Adam = org.nd4j.linalg.learning.config.Adam
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.nd4j.autodiff.samediff.listeners



	Public Class CheckpointListenerTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir Path testDir;
		Friend testDir As Path


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000L
			End Get
		End Property

		Public Shared ReadOnly Property Model As SameDiff
			Get
				Nd4j.Random.setSeed(12345)
				Dim sd As SameDiff = SameDiff.create()
				Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, -1, 4)
				Dim label As SDVariable = sd.placeHolder("label", DataType.FLOAT, -1, 3)
				Dim w As SDVariable = sd.var("W", Nd4j.rand(DataType.FLOAT, 4, 3))
				Dim b As SDVariable = sd.var("b", DataType.FLOAT, 3)
    
				Dim mmul As SDVariable = [in].mmul(w).add(b)
				Dim softmax As SDVariable = sd.nn().softmax(mmul)
				Dim loss As SDVariable = sd.loss().logLoss("loss", label, softmax)
    
				sd.TrainingConfig = TrainingConfig.builder().dataSetFeatureMapping("in").dataSetLabelMapping("label").updater(New Adam(1e-2)).weightDecay(1e-2, True).build()
    
				Return sd
			End Get
		End Property

		Public Shared ReadOnly Property Iter As DataSetIterator
			Get
				Return getIter(15, 150)
			End Get
		End Property

		Public Shared Function getIter(ByVal batch As Integer, ByVal totalExamples As Integer) As DataSetIterator
			Return New IrisDataSetIterator(batch, totalExamples)
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCheckpointEveryEpoch(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCheckpointEveryEpoch(ByVal backend As Nd4jBackend)
			Dim dir As File = testDir.toFile()

			Dim sd As SameDiff = Model
			Dim l As CheckpointListener = CheckpointListener.builder(dir).saveEveryNEpochs(1).build()

			sd.setListeners(l)

			Dim iter As DataSetIterator = CheckpointListenerTest.Iter
			sd.fit(iter, 3)

			Dim files() As File = dir.listFiles()
			Dim s1 As String = "checkpoint-0_epoch-0_iter-9" 'Note: epoch is 10 iterations, 0-9, 10-19, 20-29, etc
			Dim s2 As String = "checkpoint-1_epoch-1_iter-19"
			Dim s3 As String = "checkpoint-2_epoch-2_iter-29"
			Dim found1 As Boolean = False
			Dim found2 As Boolean = False
			Dim found3 As Boolean = False
			For Each f As File In files
				Dim s As String = f.getAbsolutePath()
				If s.Contains(s1) Then
					found1 = True
				End If
				If s.Contains(s2) Then
					found2 = True
				End If
				If s.Contains(s3) Then
					found3 = True
				End If
			Next f
			assertEquals(4, files.Length) '3 checkpoints and 1 text file (metadata)
			assertTrue(found1 AndAlso found2 AndAlso found3)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCheckpointEvery5Iter(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCheckpointEvery5Iter(ByVal backend As Nd4jBackend)
			Dim dir As File = testDir.toFile()

			Dim sd As SameDiff = Model
			Dim l As CheckpointListener = CheckpointListener.builder(dir).saveEveryNIterations(5).build()

			sd.setListeners(l)

			Dim iter As DataSetIterator = CheckpointListenerTest.Iter
			sd.fit(iter, 2) '2 epochs = 20 iter

			Dim files() As File = dir.listFiles()
			Dim names As IList(Of String) = New List(Of String) From {"checkpoint-0_epoch-0_iter-4", "checkpoint-1_epoch-0_iter-9", "checkpoint-2_epoch-1_iter-14", "checkpoint-3_epoch-1_iter-19"}
			Dim found(names.Count - 1) As Boolean
			For Each f As File In files
				Dim s As String = f.getAbsolutePath()
	'            System.out.println(s);
				For i As Integer = 0 To names.Count - 1
					If s.Contains(names(i)) Then
						found(i) = True
						Exit For
					End If
				Next i
			Next f
			assertEquals(5, files.Length) '4 checkpoints and 1 text file (metadata)

			For i As Integer = 0 To found.Length - 1
				assertTrue(found(i), names(i))
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Execution(org.junit.jupiter.api.parallel.ExecutionMode.SAME_THREAD) @Disabled("Inconsistent results on output") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void testCheckpointListenerEveryTimeUnit(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCheckpointListenerEveryTimeUnit(ByVal backend As Nd4jBackend)
			Dim dir As File = testDir.resolve("new-dir-" & System.Guid.randomUUID().ToString()).toFile()
			assertTrue(dir.mkdirs())
			Dim sd As SameDiff = Model

			Dim l As CheckpointListener = (New CheckpointListener.Builder(dir)).keepLast(2).saveEvery(4, TimeUnit.SECONDS).build()
			sd.setListeners(l)

			Dim iter As DataSetIterator = getIter(15, 150)

			For i As Integer = 0 To 4 '10 iterations total
				sd.fit(iter, 1)
			Next i

			'Expect models saved at iterations: 10, 20, 30, 40
			'But: keep only 30, 40
			Dim files() As File = dir.listFiles()

			assertEquals(3, files.Length) '2 files, 1 metadata file

			Dim names As IList(Of String) = New List(Of String) From {"checkpoint-2_epoch-3_iter-30", "checkpoint-3_epoch-4_iter-40"}
			Dim found(names.Count - 1) As Boolean
			For Each f As File In files
				Dim s As String = f.getAbsolutePath()
	'            System.out.println(s);
				For i As Integer = 0 To names.Count - 1
					If s.Contains(names(i)) Then
						found(i) = True
						Exit For
					End If
				Next i
			Next f

			For i As Integer = 0 To found.Length - 1
				assertTrue(found(i), names(i))
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCheckpointListenerKeepLast3AndEvery3(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCheckpointListenerKeepLast3AndEvery3(ByVal backend As Nd4jBackend)
			Dim dir As File = testDir.toFile()
			Dim sd As SameDiff = Model

			Dim l As CheckpointListener = (New CheckpointListener.Builder(dir)).keepLastAndEvery(3, 3).saveEveryNEpochs(2).fileNamePrefix("myFilePrefix").build()
			sd.setListeners(l)

			Dim iter As DataSetIterator = CheckpointListenerTest.Iter

			sd.fit(iter, 20)

			'Expect models saved at end of epochs: 1, 3, 5, 7, 9, 11, 13, 15, 17, 19
			'But: keep only 5, 11, 15, 17, 19
			Dim files() As File = dir.listFiles()
			Dim count As Integer = 0
			Dim cpNums As ISet(Of Integer) = New HashSet(Of Integer)()
			Dim epochNums As ISet(Of Integer) = New HashSet(Of Integer)()
			For Each f2 As File In files
				If Not f2.getPath().EndsWith(".bin") Then
					Continue For
				End If
				count += 1
				Dim idx As Integer = f2.getName().IndexOf("epoch-")
				Dim [end] As Integer = f2.getName().IndexOf("_", idx)
								Dim tempVar = idx & "epoch-".length()
				Dim num As Integer = Integer.Parse(f2.getName().Substring(tempVar, [end] - (tempVar)))
				epochNums.Add(num)

				Dim start As Integer = f2.getName().IndexOf("checkpoint-")
				[end] = f2.getName().IndexOf("_", start & "checkpoint-".Length)
								Dim tempVar2 = start & "checkpoint-".length()
				Dim epochNum As Integer = Integer.Parse(f2.getName().Substring(tempVar2, [end] - (tempVar2)))
				cpNums.Add(epochNum)
			Next f2

			assertEquals(5, cpNums.Count,cpNums.ToString())
			assertTrue(cpNums.ContainsAll(java.util.Arrays.asList(2, 5, 7, 8, 9)), cpNums.ToString())
			assertTrue(epochNums.ContainsAll(java.util.Arrays.asList(5, 11, 15, 17, 19)), epochNums.ToString())

			assertEquals(5, l.availableCheckpoints().Count)
		End Sub
	End Class

End Namespace