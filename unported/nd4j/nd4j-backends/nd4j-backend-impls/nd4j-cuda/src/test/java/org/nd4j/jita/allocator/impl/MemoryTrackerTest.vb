Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

' ******************************************************************************
' *
' *
' * This program and the accompanying materials are made available under the
' * terms of the Apache License, Version 2.0 which is available at
' * https://www.apache.org/licenses/LICENSE-2.0.
' *
' *  See the NOTICE file distributed with this work for additional
' *  information regarding copyright ownership.
' * Unless required by applicable law or agreed to in writing, software
' * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' * License for the specific language governing permissions and limitations
' * under the License.
' *
' * SPDX-License-Identifier: Apache-2.0
' *****************************************************************************

Namespace org.nd4j.jita.allocator.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.LARGE_RESOURCES) @Tag(TagNames.LONG_TEST) public class MemoryTrackerTest extends org.nd4j.common.tests.BaseND4JTest
	Public Class MemoryTrackerTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAllocatedDelta()
		Public Overridable Sub testAllocatedDelta()
			Dim precBefore As val = MemoryTracker.Instance.getPreciseFreeMemory(0)
			Dim approxBefore As val = MemoryTracker.Instance.getApproximateFreeMemory(0)
			Dim deltaBefore As val = precBefore - approxBefore

			For i As Integer = 0 To 99
				Dim buffer As val = Nd4j.createBuffer(DataType.FLOAT, 100000, False)
			Next i

			Dim precAfter As val = MemoryTracker.Instance.getPreciseFreeMemory(0)
			Dim approxAfter As val = MemoryTracker.Instance.getApproximateFreeMemory(0)
			Dim deltaAfter As val = precAfter - approxAfter

			log.info("Initial delta: {}; Allocation delta: {}", deltaBefore, deltaAfter)
			log.info("BEFORE: Precise: {}; Approx: {};", precBefore, approxBefore)
			log.info("AFTER: Precise: {}; Approx: {};", precAfter, approxAfter)
			log.info("Precise allocated: {}", precBefore - precAfter)
			log.info("Approx allocated: {}", MemoryTracker.Instance.getActiveMemory(0))
		End Sub
	End Class
End Namespace