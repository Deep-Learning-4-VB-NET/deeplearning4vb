Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports CompatStringSplit = org.nd4j.linalg.api.ops.compat.CompatStringSplit
Imports PrintVariable = org.nd4j.linalg.api.ops.util.PrintVariable
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotNull

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

Namespace org.nd4j.linalg.custom

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class ExpandableOpsTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class ExpandableOpsTests
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCompatStringSplit_1(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCompatStringSplit_1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.create("first string", "second")
			Dim delimiter As val = Nd4j.create(" ")

			Dim exp0 As val = Nd4j.createFromArray(New Long() {0, 0, 0, 1, 1, 0})
			Dim exp1 As val = Nd4j.create("first", "string", "second")

			Dim results As val = Nd4j.exec(New CompatStringSplit(array, delimiter))
			assertNotNull(results)
			assertEquals(2, results.length)

			assertEquals(exp0, results(0))
			assertEquals(exp1, results(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub test(ByVal backend As Nd4jBackend)
			Dim arr As val = Nd4j.createFromArray(0, 1, 2, 3, 4, 5, 6, 7, 8).reshape(3, 3)
			Nd4j.exec(New PrintVariable(arr))

			Dim row As val = arr.getRow(1)
			Nd4j.exec(New PrintVariable(row))
		End Sub
	End Class

End Namespace