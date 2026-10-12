Imports System.Text
import static org.junit.jupiter.api.Assertions.assertEquals
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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

Namespace org.nd4j.linalg


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.NDARRAY_SERDE) @NativeTag public class ToStringTest extends BaseNd4jTestWithBackends
	Public Class ToStringTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToString(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testToString(ByVal backend As Nd4jBackend)
			assertEquals("[         1,         2,         3]", Nd4j.createFromArray(1, 2, 3).ToString())

			assertEquals("[       1,       2,       3,       4,       5,       6,       7,       8]", Nd4j.createFromArray(1, 2, 3, 4, 5, 6, 7, 8).toString(1000, False, 2))

			assertEquals("[    1.132,    2.644,    3.234]", Nd4j.createFromArray(1.132414, 2.64356456, 3.234234).toString(1000, False, 3))

			assertEquals("[               1.132414,             2.64356456,             3.25345234]", Nd4j.createFromArray(1.132414, 2.64356456, 3.25345234).toStringFull())

			assertEquals("[      1,      2,      3,  ...      6,      7,      8]", Nd4j.createFromArray(1, 2, 3, 4, 5, 6, 7, 8).toString(6, True, 1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled public void testToStringScalars(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testToStringScalars(ByVal backend As Nd4jBackend)
			Dim dataTypes() As DataType = {DataType.FLOAT, DataType.DOUBLE, DataType.BOOL, DataType.INT, DataType.UINT32}
			Dim strs() As String = {"1.0000", "1.0000", "true", "1", "1"}

			For dt As Integer = 0 To 4
				For i As Integer = 0 To 4
					Dim shape() As Long = ArrayUtil.nTimes(i, 1L)
					Dim scalar As INDArray = Nd4j.scalar(1.0f).castTo(dataTypes(dt)).reshape(shape)
					Dim str As String = scalar.ToString()
					Dim sb As New StringBuilder()
					For j As Integer = 0 To i - 1
						sb.Append("[")
					Next j
					sb.Append(strs(dt))
					For j As Integer = 0 To i - 1
						sb.Append("]")
					Next j
					Dim exp As String = sb.ToString()
					assertEquals("Rank: " & i & ", DT: " & dataTypes(dt), exp, str)
				Next i
			Next dt
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace