Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports SameDiffCNNCases = org.deeplearning4j.integration.testcases.samediff.SameDiffCNNCases
Imports SameDiffMLPTestCases = org.deeplearning4j.integration.testcases.samediff.SameDiffMLPTestCases
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames

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
Namespace org.deeplearning4j.integration


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SAMEDIFF) @NativeTag public class IntegrationTestsSameDiff extends org.deeplearning4j.BaseDL4JTest
	Public Class IntegrationTestsSameDiff
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir static java.nio.file.Path testDir;
		Friend Shared testDir As Path

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 300_000L
			End Get
		End Property




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMLPMnist() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMLPMnist()
			IntegrationTestRunner.runTest(SameDiffMLPTestCases.MLPMnist, testDir)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMLPMoon() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMLPMoon()
			IntegrationTestRunner.runTest(SameDiffMLPTestCases.MLPMoon, testDir)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLenetMnist() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLenetMnist()
			IntegrationTestRunner.runTest(SameDiffCNNCases.LenetMnist, testDir)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnn3dSynthetic() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCnn3dSynthetic()
			IntegrationTestRunner.runTest(SameDiffCNNCases.Cnn3dSynthetic, testDir)
		End Sub


	End Class

End Namespace