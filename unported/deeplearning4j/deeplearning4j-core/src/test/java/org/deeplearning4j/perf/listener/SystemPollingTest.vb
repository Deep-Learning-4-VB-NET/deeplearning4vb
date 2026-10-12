Imports System
Imports System.Threading
Imports FileUtils = org.apache.commons.io.FileUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports HardwareMetric = org.deeplearning4j.core.listener.HardwareMetric
Imports SystemPolling = org.deeplearning4j.core.listener.SystemPolling
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
'ORIGINAL LINE: @Disabled("AB 2019/05/24 - Failing on CI - ""Could not initialize class oshi.jna.platform.linux.Libc"" - Issue #7657") @DisplayName("System Polling Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class SystemPollingTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class SystemPollingTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path tempDir;
		Public tempDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Polling") void testPolling() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testPolling()
			Nd4j.create(1)
			Dim tmpDir As File = tempDir.toFile()
			Dim systemPolling As SystemPolling = (New SystemPolling.Builder()).outputDirectory(tmpDir).pollEveryMillis(1000).build()
			systemPolling.run()
			Thread.Sleep(8000)
			systemPolling.stopPolling()
			Dim files() As File = tmpDir.listFiles()
			assertTrue(files IsNot Nothing AndAlso files.Length > 0)
			' System.out.println(Arrays.toString(files));
			Dim yaml As String = FileUtils.readFileToString(files(0))
			Dim fromYaml As HardwareMetric = HardwareMetric.fromYaml(yaml)
			Console.WriteLine(fromYaml)
		End Sub
	End Class

End Namespace