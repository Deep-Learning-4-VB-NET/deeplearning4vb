Imports System
Imports System.Collections.Generic
Imports Table = com.google.flatbuffers.Table
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports UIListener = org.nd4j.autodiff.listeners.impl.UIListener
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports TrainingConfig = org.nd4j.autodiff.samediff.TrainingConfig
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports UIEvent = org.nd4j.graph.UIEvent
Imports UIGraphStructure = org.nd4j.graph.UIGraphStructure
Imports UIStaticInfoRecord = org.nd4j.graph.UIStaticInfoRecord
Imports LogFileWriter = org.nd4j.graph.ui.LogFileWriter
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports IrisDataSetIterator = org.nd4j.linalg.dataset.IrisDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Adam = org.nd4j.linalg.learning.config.Adam
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

Namespace org.nd4j.autodiff.ui


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.SAMEDIFF) @Tag(TagNames.UI) @Tag(TagNames.FILE_IO) public class UIListenerTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class UIListenerTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir Path testDir;
		Friend testDir As Path

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUIListenerBasic(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUIListenerBasic(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim iter As New IrisDataSetIterator(150, 150)

			Dim sd As SameDiff = SimpleNet

			Dim dir As File = testDir.toFile()
			Dim f As New File(dir, "logFile.bin")
			Dim l As UIListener = UIListener.builder(f).plotLosses(1).trainEvaluationMetrics("softmax", 0, Evaluation.Metric.ACCURACY, Evaluation.Metric.F1).updateRatios(1).build()

			sd.setListeners(l)

			sd.TrainingConfig = TrainingConfig.builder().dataSetFeatureMapping("in").dataSetLabelMapping("label").updater(New Adam(1e-1)).weightDecay(1e-3, True).build()

			sd.fit(iter, 20)

			'Test inference after training with UI Listener still around
			Dim m As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			iter.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			m("in") = iter.next().Features
			Dim [out] As INDArray = sd.outputSingle(m, "softmax")
			assertNotNull([out])
			assertArrayEquals(New Long(){150, 3}, [out].shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUIListenerContinue(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUIListenerContinue(ByVal backend As Nd4jBackend)
			Dim iter As New IrisDataSetIterator(150, 150)

			Dim sd1 As SameDiff = SimpleNet
			Dim sd2 As SameDiff = SimpleNet

			Dim dir As File = testDir.resolve("new-dir-1").toFile()
			dir.mkdirs()
			Dim f As New File(dir, "logFileNoContinue.bin")
			f.delete()
			Dim l1 As UIListener = UIListener.builder(f).plotLosses(1).trainEvaluationMetrics("softmax", 0, Evaluation.Metric.ACCURACY, Evaluation.Metric.F1).updateRatios(1).build()

			sd1.setListeners(l1)

			sd1.fit(iter, 2)


			'Do some thing with 2nd net, in 2 sets
			Dim f2 As New File(dir, "logFileContinue.bin")
			Dim l2 As UIListener = UIListener.builder(f2).plotLosses(1).trainEvaluationMetrics("softmax", 0, Evaluation.Metric.ACCURACY, Evaluation.Metric.F1).updateRatios(1).build()

			sd2.setListeners(l2)
			sd2.fit(iter, 1)

			l2 = UIListener.builder(f2).plotLosses(1).trainEvaluationMetrics("softmax", 0, Evaluation.Metric.ACCURACY, Evaluation.Metric.F1).updateRatios(1).build()
			sd2.setListeners(l2)
			sd2.setListeners(l2)
			sd2.fit(iter, 1)

			assertEquals(f.length(), f2.length())

			Dim lfw1 As New LogFileWriter(f)
			Dim lfw2 As New LogFileWriter(f2)


			'Check static info are equal:
			Dim si1 As LogFileWriter.StaticInfo = lfw1.readStatic()
			Dim si2 As LogFileWriter.StaticInfo = lfw2.readStatic()

			Dim ls1 As IList(Of Pair(Of UIStaticInfoRecord, Table)) = si1.getData()
			Dim ls2 As IList(Of Pair(Of UIStaticInfoRecord, Table)) = si2.getData()

			assertEquals(ls1.Count, ls2.Count)
			For i As Integer = 0 To ls1.Count - 1
				Dim p1 As Pair(Of UIStaticInfoRecord, Table) = ls1(i)
				Dim p2 As Pair(Of UIStaticInfoRecord, Table) = ls2(i)
				assertEquals(p1.First.infoType(), p2.First.infoType())
				If p1.Second Is Nothing Then
					assertNull(p2.Second)
				Else
					assertEquals(p1.Second.GetType(), p2.Second.GetType())
					If TypeOf p1.Second Is UIGraphStructure Then
						Dim g1 As UIGraphStructure = CType(p1.Second, UIGraphStructure)
						Dim g2 As UIGraphStructure = CType(p2.Second, UIGraphStructure)

						assertEquals(g1.inputsLength(), g2.inputsLength())
						assertEquals(g1.outputsLength(), g2.outputsLength())
						assertEquals(g1.opsLength(), g2.opsLength())
					End If
				End If
			Next i

			'Check events:
			Dim e1 As IList(Of Pair(Of UIEvent, Table)) = lfw1.readEvents()
			Dim e2 As IList(Of Pair(Of UIEvent, Table)) = lfw2.readEvents()
			assertEquals(e1.Count, e2.Count)

			For i As Integer = 0 To e1.Count - 1
				Dim p1 As Pair(Of UIEvent, Table) = e1(i)
				Dim p2 As Pair(Of UIEvent, Table) = e2(i)
				Dim ev1 As UIEvent = p1.First
				Dim ev2 As UIEvent = p2.First

				assertEquals(ev1.eventType(), ev2.eventType())
				assertEquals(ev1.epoch(), ev2.epoch())
				assertEquals(ev1.iteration(), ev2.iteration())
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUIListenerBadContinue(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUIListenerBadContinue(ByVal backend As Nd4jBackend)
			Dim iter As New IrisDataSetIterator(150, 150)
			Dim sd1 As SameDiff = SimpleNet

			Dim dir As File = testDir.resolve("new-dir-2").toFile()
			dir.mkdirs()
			Dim f As New File(dir, "logFile.bin")
			f.delete()
			Dim l1 As UIListener = UIListener.builder(f).plotLosses(1).trainEvaluationMetrics("softmax", 0, Evaluation.Metric.ACCURACY, Evaluation.Metric.F1).updateRatios(1).build()

			sd1.setListeners(l1)

			sd1.fit(iter, 2)

			'Now, fit with different net - more placeholders
			Dim sd2 As SameDiff = SameDiff.create()
			Dim in1 As SDVariable = sd2.placeHolder("in1", DataType.FLOAT, -1, 4)
			Dim in2 As SDVariable = sd2.placeHolder("in2", DataType.FLOAT, -1, 4)
			Dim w As SDVariable = sd2.var("w", DataType.FLOAT, 1, 4)
			Dim mul As SDVariable = in1.mul(in2).mul(w)
			Dim loss As SDVariable = mul.std(True)
			sd2.TrainingConfig = TrainingConfig.builder().dataSetFeatureMapping("in").dataSetLabelMapping("label").updater(New Adam(1e-1)).build()

			Dim l2 As UIListener = UIListener.builder(f).plotLosses(1).trainEvaluationMetrics("softmax", 0, Evaluation.Metric.ACCURACY, Evaluation.Metric.F1).updateRatios(1).build()

			sd2.setListeners(l2)
			Try
				sd2.fit(iter, 2)
				fail("Expected exception")
			Catch t As Exception
				Dim m As String = t.getMessage()
				assertTrue(m.Contains("placeholder"),m)
				assertTrue(m.Contains("FileMode.CREATE_APPEND_NOCHECK"),m)
			End Try


			'fit with different net - more variables
			Dim sd3 As SameDiff = SimpleNet
			sd3.var("SomeNewVar", DataType.FLOAT, 3,4)
			Dim l3 As UIListener = UIListener.builder(f).plotLosses(1).trainEvaluationMetrics("softmax", 0, Evaluation.Metric.ACCURACY, Evaluation.Metric.F1).updateRatios(1).build()

			sd3.setListeners(l3)

			Try
				sd3.fit(iter, 2)
				fail("Expected exception")
			Catch t As Exception
				Dim m As String = t.getMessage()
				assertTrue(m.Contains("variable"),m)
				assertTrue(m.Contains("FileMode.CREATE_APPEND_NOCHECK"),m)
			End Try


			'Fit with proper net:
			Dim sd4 As SameDiff = SimpleNet
			Dim l4 As UIListener = UIListener.builder(f).plotLosses(1).trainEvaluationMetrics("softmax", 0, Evaluation.Metric.ACCURACY, Evaluation.Metric.F1).updateRatios(1).build()

			sd4.setListeners(l4)
			sd4.fit(iter, 2)
		End Sub


		Private Shared ReadOnly Property SimpleNet As SameDiff
			Get
				Nd4j.Random.setSeed(12345)
				Dim sd As SameDiff = SameDiff.create()
				Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, -1, 4)
				Dim label As SDVariable = sd.placeHolder("label", DataType.FLOAT, -1, 3)
				Dim w As SDVariable = sd.var("W", Nd4j.rand(DataType.FLOAT, 4, 3))
				Dim b As SDVariable = sd.var("b", DataType.FLOAT, 1, 3)
				Dim mmul As SDVariable = [in].mmul(w).add(b)
				Dim softmax As SDVariable = sd.nn_Conflict.softmax("softmax", mmul)
				Dim loss As SDVariable = sd.loss().logLoss("loss", label, softmax)
    
				sd.TrainingConfig = TrainingConfig.builder().dataSetFeatureMapping("in").dataSetLabelMapping("label").updater(New Adam(1e-1)).weightDecay(1e-3, True).build()
				Return sd
			End Get
		End Property

	End Class

End Namespace