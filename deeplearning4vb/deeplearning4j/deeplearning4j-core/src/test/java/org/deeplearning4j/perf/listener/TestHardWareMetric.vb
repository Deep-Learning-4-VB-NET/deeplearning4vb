Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports HardwareMetric = org.deeplearning4j.core.listener.HardwareMetric
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports SystemInfo = oshi.json.SystemInfo
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

Namespace org.deeplearning4j.perf.listener

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled("AB 2019/05/24 - Failing on CI - ""Could not initialize class oshi.jna.platform.linux.Libc"" - Issue #7657") @NativeTag @Tag(TagNames.JACKSON_SERDE) public class TestHardWareMetric extends org.deeplearning4j.BaseDL4JTest
	Public Class TestHardWareMetric
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testHardwareMetric()
		Public Overridable Sub testHardwareMetric()
			Dim hardwareMetric As HardwareMetric = HardwareMetric.fromSystem(New SystemInfo())
			assertNotNull(hardwareMetric)
			Console.WriteLine(hardwareMetric)

			Dim yaml As String = hardwareMetric.toYaml()
			Dim fromYaml As HardwareMetric = HardwareMetric.fromYaml(yaml)
			assertEquals(hardwareMetric, fromYaml)
		End Sub

	End Class

End Namespace