import static org.junit.jupiter.api.Assertions.assertArrayEquals
Imports Test = org.junit.jupiter.api.Test
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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

Namespace org.nd4j.common.util

	Public Class ArrayUtilTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInvertPermutationInt()
		Public Overridable Sub testInvertPermutationInt()
			assertArrayEquals(New Integer(){ 2, 4, 3, 0, 1 }, ArrayUtil.invertPermutation(3, 4, 0, 2, 1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInvertPermutationLong()
		Public Overridable Sub testInvertPermutationLong()
			assertArrayEquals(New Long(){ 2, 4, 3, 0, 1 }, ArrayUtil.invertPermutation(3L, 4L, 0L, 2L, 1L))
		End Sub

	End Class

End Namespace