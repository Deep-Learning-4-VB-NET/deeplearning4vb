Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.deeplearning4j.spark.impl.common.repartition

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class BalancedPartitionerTest
	Public Class BalancedPartitionerTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void balancedPartitionerFirstElements()
		Public Overridable Sub balancedPartitionerFirstElements()
			Dim bp As New BalancedPartitioner(10, 10, 0)
			' the 10 first elements should go in the 1st partition
			For i As Integer = 0 To 9
				Dim p As Integer = bp.getPartition(i)
				assertEquals(0, p,"Found wrong partition output " & p & ", not 0")
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void balancedPartitionerFirstElementsWithRemainder()
		Public Overridable Sub balancedPartitionerFirstElementsWithRemainder()
			Dim bp As New BalancedPartitioner(10, 10, 1)
			' the 10 first elements should go in the 1st partition
			For i As Integer = 0 To 9
				Dim p As Integer = bp.getPartition(i)
				assertEquals(0, p,"Found wrong partition output " & p & ", not 0")
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void balancedPartitionerDoesBalance()
		Public Overridable Sub balancedPartitionerDoesBalance()
			Dim bp As New BalancedPartitioner(10, 10, 0)
			Dim countPerPartition(9) As Integer
			For i As Integer = 0 To (10 * 10) - 1
				Dim p As Integer = bp.getPartition(i)
				countPerPartition(p) += 1
			Next i
			For i As Integer = 0 To 9
				assertEquals(10, countPerPartition(i))
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void balancedPartitionerDoesBalanceWithRemainder()
		Public Overridable Sub balancedPartitionerDoesBalanceWithRemainder()
			Dim bp As New BalancedPartitioner(10, 10, 7)
			Dim countPerPartition(9) As Integer
			For i As Integer = 0 To 10 * 10 + 6
				Dim p As Integer = bp.getPartition(i)
				countPerPartition(p) += 1
			Next i
			For i As Integer = 0 To 9
				If i < 7 Then
					assertEquals(10 + 1, countPerPartition(i))
				Else
					assertEquals(10, countPerPartition(i))
				End If
			Next i
		End Sub

	End Class

End Namespace