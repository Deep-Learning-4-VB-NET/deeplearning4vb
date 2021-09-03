﻿Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports Timeout = org.junit.jupiter.api.Timeout
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NDArrayMessage = org.nd4j.aeron.ipc.NDArrayMessage
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.nd4j.parameterserver.updater.storage


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled public class UpdaterStorageTests extends org.nd4j.common.tests.BaseND4JTest
	Public Class UpdaterStorageTests
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(30000L) public void testInMemory()
		Public Overridable Sub testInMemory()
			Dim updateStorage As UpdateStorage = New RocksDbStorage("/tmp/rocksdb")
			Dim message As NDArrayMessage = NDArrayMessage.wholeArrayUpdate(Nd4j.scalar(1.0))
			updateStorage.addUpdate(message)
			assertEquals(1, updateStorage.numUpdates())
			assertEquals(message, updateStorage.getUpdate(0))
			updateStorage.clear()
			assertEquals(0, updateStorage.numUpdates())
			updateStorage.close()
		End Sub
	End Class

End Namespace