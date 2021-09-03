Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports JointParallelDataSetIterator = org.deeplearning4j.datasets.iterator.parallel.JointParallelDataSetIterator
Imports SimpleVariableGenerator = org.deeplearning4j.datasets.iterator.tools.SimpleVariableGenerator
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports InequalityHandling = org.nd4j.linalg.dataset.api.iterator.enums.InequalityHandling
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotNull
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.deeplearning4j.datasets.iterator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Joint Parallel Data Set Iterator Test") @NativeTag @Tag(TagNames.FILE_IO) class JointParallelDataSetIteratorTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class JointParallelDataSetIteratorTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Joint Iterator 1") void testJointIterator1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testJointIterator1()
			Dim iteratorA As DataSetIterator = New SimpleVariableGenerator(119, 100, 32, 100, 10)
			Dim iteratorB As DataSetIterator = New SimpleVariableGenerator(119, 100, 32, 100, 10)
			Dim jpdsi As JointParallelDataSetIterator = (New JointParallelDataSetIterator.Builder(InequalityHandling.STOP_EVERYONE)).addSourceIterator(iteratorA).addSourceIterator(iteratorB).build()
			Dim cnt As Integer = 0
			Dim example As Integer = 0
			Do While jpdsi.MoveNext()
				Dim ds As DataSet = jpdsi.Current
				assertNotNull(ds,"Failed on iteration " & cnt)
				' ds.detach();
				' ds.migrate();
				assertEquals(CDbl(example), ds.Features.meanNumber().doubleValue(), 0.001,"Failed on iteration " & cnt)
				assertEquals(CDbl(example) + 0.5, ds.Labels.meanNumber().doubleValue(), 0.001,"Failed on iteration " & cnt)
				cnt += 1
				If cnt Mod 2 = 0 Then
					example += 1
				End If
			Loop
			assertEquals(100, example)
			assertEquals(200, cnt)
		End Sub

		''' <summary>
		''' This test checks for pass_null scenario, so in total we should have 300 real datasets + 100 nulls </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Joint Iterator 2") void testJointIterator2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testJointIterator2()
			Dim iteratorA As DataSetIterator = New SimpleVariableGenerator(119, 200, 32, 100, 10)
			Dim iteratorB As DataSetIterator = New SimpleVariableGenerator(119, 100, 32, 100, 10)
			Dim jpdsi As JointParallelDataSetIterator = (New JointParallelDataSetIterator.Builder(InequalityHandling.PASS_NULL)).addSourceIterator(iteratorA).addSourceIterator(iteratorB).build()
			Dim cnt As Integer = 0
			Dim example As Integer = 0
			Dim nulls As Integer = 0
			Do While jpdsi.MoveNext()
				Dim ds As DataSet = jpdsi.Current
				If cnt < 200 Then
					assertNotNull(ds,"Failed on iteration " & cnt)
				End If
				If ds Is Nothing Then
					nulls += 1
				End If
				If cnt Mod 2 = 2 Then
					assertEquals(CDbl(example), ds.Features.meanNumber().doubleValue(), 0.001,"Failed on iteration " & cnt)
					assertEquals(CDbl(example) + 0.5, ds.Labels.meanNumber().doubleValue(), 0.001,"Failed on iteration " & cnt)
				End If
				cnt += 1
				If cnt Mod 2 = 0 Then
					example += 1
				End If
			Loop
			assertEquals(100, nulls)
			assertEquals(200, example)
			assertEquals(400, cnt)
		End Sub

		''' <summary>
		''' Testing relocate
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Joint Iterator 3") void testJointIterator3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testJointIterator3()
			Dim iteratorA As DataSetIterator = New SimpleVariableGenerator(119, 200, 32, 100, 10)
			Dim iteratorB As DataSetIterator = New SimpleVariableGenerator(119, 100, 32, 100, 10)
			Dim jpdsi As JointParallelDataSetIterator = (New JointParallelDataSetIterator.Builder(InequalityHandling.RELOCATE)).addSourceIterator(iteratorA).addSourceIterator(iteratorB).build()
			Dim cnt As Integer = 0
			Dim example As Integer = 0
			Do While jpdsi.MoveNext()
				Dim ds As DataSet = jpdsi.Current
				assertNotNull(ds,"Failed on iteration " & cnt)
				assertEquals(CDbl(example), ds.Features.meanNumber().doubleValue(), 0.001,"Failed on iteration " & cnt)
				assertEquals(CDbl(example) + 0.5, ds.Labels.meanNumber().doubleValue(), 0.001,"Failed on iteration " & cnt)
				cnt += 1
				If cnt < 200 Then
					If cnt Mod 2 = 0 Then
						example += 1
					End If
				Else
					example += 1
				End If
			Loop
			assertEquals(300, cnt)
			assertEquals(200, example)
		End Sub

		''' <summary>
		''' Testing relocate
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Joint Iterator 4") void testJointIterator4() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testJointIterator4()
			Dim iteratorA As DataSetIterator = New SimpleVariableGenerator(119, 200, 32, 100, 10)
			Dim iteratorB As DataSetIterator = New SimpleVariableGenerator(119, 100, 32, 100, 10)
			Dim jpdsi As JointParallelDataSetIterator = (New JointParallelDataSetIterator.Builder(InequalityHandling.RESET)).addSourceIterator(iteratorA).addSourceIterator(iteratorB).build()
			Dim cnt As Integer = 0
			Dim cnt_sec As Integer = 0
			Dim example_sec As Integer = 0
			Dim example As Integer = 0
			Do While jpdsi.MoveNext()
				Dim ds As DataSet = jpdsi.Current
				assertNotNull(ds,"Failed on iteration " & cnt)
				If cnt Mod 2 = 0 Then
					assertEquals(CDbl(example), ds.Features.meanNumber().doubleValue(), 0.001,"Failed on iteration " & cnt)
					assertEquals(CDbl(example) + 0.5, ds.Labels.meanNumber().doubleValue(), 0.001,"Failed on iteration " & cnt)
				Else
					If cnt <= 200 Then
						assertEquals(CDbl(example), ds.Features.meanNumber().doubleValue(), 0.001,"Failed on iteration " & cnt)
						assertEquals(CDbl(example) + 0.5, ds.Labels.meanNumber().doubleValue(), 0.001,"Failed on iteration " & cnt)
					Else
						assertEquals(CDbl(example_sec), ds.Features.meanNumber().doubleValue(), 0.001,"Failed on iteration " & cnt & ", second iteration " & cnt_sec)
						assertEquals(CDbl(example_sec) + 0.5, ds.Labels.meanNumber().doubleValue(), 0.001,"Failed on iteration " & cnt & ", second iteration " & cnt_sec)
					End If
				End If
				cnt += 1
				If cnt Mod 2 = 0 Then
					example += 1
				End If
				If cnt > 201 AndAlso cnt Mod 2 = 1 Then
					cnt_sec += 1
					example_sec += 1
				End If
			Loop
			assertEquals(400, cnt)
			assertEquals(200, example)
		End Sub
	End Class

End Namespace