Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Timeout = org.junit.jupiter.api.Timeout
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NDArrayMessage = org.nd4j.aeron.ipc.NDArrayMessage
Imports InMemoryNDArrayHolder = org.nd4j.aeron.ndarrayholder.InMemoryNDArrayHolder
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NoUpdateStorage = org.nd4j.parameterserver.updater.storage.NoUpdateStorage
Imports org.junit.jupiter.api.Assertions
Imports org.junit.jupiter.api.Assumptions

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

Namespace org.nd4j.parameterserver.updater

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class ParameterServerUpdaterTests extends org.nd4j.common.tests.BaseND4JTest
	Public Class ParameterServerUpdaterTests
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(30000L) public void synchronousTest()
		Public Overridable Sub synchronousTest()
			Dim cores As Integer = Runtime.getRuntime().availableProcessors()
			Dim updater As ParameterServerUpdater = New SynchronousParameterUpdater(New NoUpdateStorage(), New InMemoryNDArrayHolder(Nd4j.zeros(2, 2)), cores)
			For i As Integer = 0 To cores - 1
				updater.update(NDArrayMessage.wholeArrayUpdate(Nd4j.ones(2, 2)))
			Next i

			assertTrue(updater.shouldReplicate())
			updater.reset()
			assertFalse(updater.shouldReplicate())
			assertNotNull(updater.toJson())


		End Sub

	End Class

End Namespace