Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports var = lombok.var
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports SimpleVariableGenerator = org.deeplearning4j.datasets.iterator.tools.SimpleVariableGenerator
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotNull
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

Namespace org.deeplearning4j.datasets.iterator


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.JAVA_ONLY) public class DummyBlockDataSetIteratorTests extends org.deeplearning4j.BaseDL4JTest
	Public Class DummyBlockDataSetIteratorTests
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBlock_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBlock_1()
			Dim simpleIterator As val = New SimpleVariableGenerator(123, 8, 3, 3, 3)

			Dim iterator As val = New DummyBlockDataSetIterator(simpleIterator)

			assertTrue(iterator.hasAnything())
			Dim list As val = New List(Of DataSet)(8)

			Dim datasets As var = iterator.next(3)
			assertNotNull(datasets)
			assertEquals(3, datasets.length)
			list.addAll(Arrays.asList(datasets))



			datasets = iterator.next(3)
			assertNotNull(datasets)
			assertEquals(3, datasets.length)
			list.addAll(Arrays.asList(datasets))

			datasets = iterator.next(3)
			assertNotNull(datasets)
			assertEquals(2, datasets.length)
			list.addAll(Arrays.asList(datasets))

			For e As Integer = 0 To list.size() - 1
				Dim dataset As val = list.get(e)

				assertEquals(e, CInt(Math.Truncate(dataset.getFeatures().getDouble(0))))
				assertEquals(e + 0.5, dataset.getLabels().getDouble(0), 1e-3)
			Next e
		End Sub
	End Class

End Namespace