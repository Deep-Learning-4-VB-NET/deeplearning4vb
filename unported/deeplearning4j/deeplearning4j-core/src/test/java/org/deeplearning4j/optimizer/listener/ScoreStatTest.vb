Imports System
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports CollectScoresIterationListener = org.deeplearning4j.optimize.listeners.CollectScoresIterationListener
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports org.junit.jupiter.api.Assertions
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames

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
'ORIGINAL LINE: @DisplayName("Score Stat Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class ScoreStatTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class ScoreStatTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Score Stat Small") void testScoreStatSmall()
		Friend Overridable Sub testScoreStatSmall()
			Dim statTest As New CollectScoresIterationListener.ScoreStat()
			For i As Integer = 0 To CollectScoresIterationListener.ScoreStat.BUCKET_LENGTH - 1
				Dim score As Double = CDbl(i)
				statTest.addScore(i, score)
			Next i
			Dim indexes As IList(Of Long()) = statTest.getIndexes()
			Dim scores As IList(Of Double()) = statTest.getScores()
			assertTrue(indexes.Count = 1)
			assertTrue(scores.Count = 1)
			assertTrue(indexes(0).Length = CollectScoresIterationListener.ScoreStat.BUCKET_LENGTH)
			assertTrue(scores(0).Length = CollectScoresIterationListener.ScoreStat.BUCKET_LENGTH)
			assertEquals(indexes(0)(indexes(0).Length - 1), CollectScoresIterationListener.ScoreStat.BUCKET_LENGTH - 1)
			assertEquals(scores(0)(scores(0).Length - 1), CollectScoresIterationListener.ScoreStat.BUCKET_LENGTH - 1, 1e-4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Score Stat Average") void testScoreStatAverage()
		Friend Overridable Sub testScoreStatAverage()
			Dim dataSize As Integer = 1000000
			Dim indexes(dataSize - 1) As Long
			Dim scores(dataSize - 1) As Double
			For i As Integer = 0 To dataSize - 1
				indexes(i) = i
				scores(i) = i
			Next i
			Dim statTest As New CollectScoresIterationListener.ScoreStat()
			For i As Integer = 0 To dataSize - 1
				statTest.addScore(indexes(i), scores(i))
			Next i
			Dim indexesStored() As Long = statTest.getIndexes()(0)
			Dim scoresStored() As Double = statTest.getScores()(0)
			assertArrayEquals(indexes, indexesStored)
			assertArrayEquals(scores, scoresStored, 1e-4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Scores Clean") void testScoresClean()
		Friend Overridable Sub testScoresClean()
			' expected to be placed in 2 buckets of 10k elements size
			Dim dataSize As Integer = 10256
			Dim indexes(dataSize - 1) As Long
			Dim scores(dataSize - 1) As Double
			For i As Integer = 0 To dataSize - 1
				indexes(i) = i
				scores(i) = i
			Next i
			Dim statTest As New CollectScoresIterationListener.ScoreStat()
			For i As Integer = 0 To dataSize - 1
				statTest.addScore(indexes(i), scores(i))
			Next i
			Dim indexesEffective() As Long = statTest.EffectiveIndexes
			Dim scoresEffective() As Double = statTest.EffectiveScores
			assertArrayEquals(indexes, indexesEffective)
			assertArrayEquals(scores, scoresEffective, 1e-4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Test @DisplayName("Test Score Stat Big") void testScoreStatBig()
		Friend Overridable Sub testScoreStatBig()
			Dim statTest As New CollectScoresIterationListener.ScoreStat()
			Dim bigLength As Long = CLng(Math.Truncate(Integer.MaxValue)) + 5
			For i As Long = 0 To bigLength - 1
				Dim score As Double = CDbl(i)
				statTest.addScore(i, score)
			Next i
			Dim indexes As IList(Of Long()) = statTest.getIndexes()
			Dim scores As IList(Of Double()) = statTest.getScores()
			assertTrue(indexes.Count = 2)
			assertTrue(scores.Count = 2)
			For i As Integer = 0 To 4
				assertTrue(indexes(1)(i) = Integer.MaxValue + i)
				assertTrue(scores(1)(i) = Integer.MaxValue + i)
			Next i
		End Sub
	End Class

End Namespace