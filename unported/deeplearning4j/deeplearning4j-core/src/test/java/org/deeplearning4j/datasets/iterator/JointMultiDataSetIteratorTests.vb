Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports DataSetGenerator = org.deeplearning4j.datasets.iterator.tools.DataSetGenerator
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Timeout = org.junit.jupiter.api.Timeout
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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

Namespace org.deeplearning4j.datasets.iterator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.FILE_IO) public class JointMultiDataSetIteratorTests extends org.deeplearning4j.BaseDL4JTest
	Public Class JointMultiDataSetIteratorTests
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(20000L) public void testJMDSI_1()
		Public Overridable Sub testJMDSI_1()
			Dim iter0 As val = New DataSetGenerator(32, New Integer(){3, 3}, New Integer(){2, 2})
			Dim iter1 As val = New DataSetGenerator(32, New Integer(){3, 3, 3}, New Integer(){2, 2, 2})
			Dim iter2 As val = New DataSetGenerator(32, New Integer(){3, 3, 3, 3}, New Integer(){2, 2, 2, 2})

			Dim mdsi As val = New JointMultiDataSetIterator(iter0, iter1, iter2)

			Dim cnt As Integer = 0
			Do While mdsi.hasNext()
				Dim mds As val = mdsi.next()

				assertNotNull(mds)

				Dim f As val = mds.getFeatures()
				Dim l As val = mds.getLabels()

				Dim fm As val = mds.getFeaturesMaskArrays()
				Dim lm As val = mds.getLabelsMaskArrays()

				assertNotNull(f)
				assertNotNull(l)
				assertNull(fm)
				assertNull(lm)

				assertArrayEquals(New Long(){3, 3}, f(0).shape())
				assertArrayEquals(New Long(){3, 3, 3}, f(1).shape())
				assertArrayEquals(New Long(){3, 3, 3, 3}, f(2).shape())

				assertEquals(cnt, CInt(Math.Truncate(f(0).getDouble(0))))
				assertEquals(cnt, CInt(Math.Truncate(f(1).getDouble(0))))
				assertEquals(cnt, CInt(Math.Truncate(f(2).getDouble(0))))

				assertArrayEquals(New Long(){2, 2}, l(0).shape())
				assertArrayEquals(New Long(){2, 2, 2}, l(1).shape())
				assertArrayEquals(New Long(){2, 2, 2, 2}, l(2).shape())

				cnt += 1
			Loop

			assertEquals(32, cnt)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(20000L) public void testJMDSI_2()
		Public Overridable Sub testJMDSI_2()
			Dim iter0 As val = New DataSetGenerator(32, New Integer(){3, 3}, New Integer(){2, 2})
			Dim iter1 As val = New DataSetGenerator(32, New Integer(){3, 3, 3}, New Integer(){2, 2, 2})
			Dim iter2 As val = New DataSetGenerator(32, New Integer(){3, 3, 3, 3}, New Integer(){2, 2, 2, 2})

			Dim mdsi As val = New JointMultiDataSetIterator(1, iter0, iter1, iter2)

			Dim cnt As Integer = 0
			Do While mdsi.hasNext()
				Dim mds As val = mdsi.next()

				assertNotNull(mds)

				Dim f As val = mds.getFeatures()
				Dim l As val = mds.getLabels()

				Dim fm As val = mds.getFeaturesMaskArrays()
				Dim lm As val = mds.getLabelsMaskArrays()

				assertNotNull(f)
				assertNotNull(l)
				assertNull(fm)
				assertNull(lm)

				assertArrayEquals(New Long(){3, 3}, f(0).shape())
				assertArrayEquals(New Long(){3, 3, 3}, f(1).shape())
				assertArrayEquals(New Long(){3, 3, 3, 3}, f(2).shape())

				assertEquals(cnt, CInt(Math.Truncate(f(0).getDouble(0))))
				assertEquals(cnt, CInt(Math.Truncate(f(1).getDouble(0))))
				assertEquals(cnt, CInt(Math.Truncate(f(2).getDouble(0))))

				assertEquals(1, l.length)

				assertArrayEquals(New Long(){2, 2, 2}, l(0).shape())
				assertEquals(cnt, CInt(Math.Truncate(l(0).getDouble(0))))

				cnt += 1
			Loop

			assertEquals(32, cnt)
		End Sub
	End Class

End Namespace