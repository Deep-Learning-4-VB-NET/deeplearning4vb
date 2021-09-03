Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports org.deeplearning4j.integration.testcases.dl4j
Imports AfterEach = org.junit.jupiter.api.AfterEach
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



	'@Disabled("AB - 2019/05/27 - Integration tests need to be updated")
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.DL4J_OLD_API) @NativeTag public class IntegrationTestsDL4J extends org.deeplearning4j.BaseDL4JTest
	Public Class IntegrationTestsDL4J
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
'ORIGINAL LINE: @AfterEach public static void afterClass()
		Public Shared Sub afterClass()
			IntegrationTestRunner.printCoverageInformation()
		End Sub

		' ***** MLPTestCases *****
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMLPMnist() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMLPMnist()
			IntegrationTestRunner.runTest(MLPTestCases.MLPMnist, testDir)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMlpMoon() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMlpMoon()
			IntegrationTestRunner.runTest(MLPTestCases.MLPMoon, testDir)
		End Sub

		' ***** RNNTestCases *****
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRnnSeqClassification1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRnnSeqClassification1()
			IntegrationTestRunner.runTest(RNNTestCases.RnnCsvSequenceClassificationTestCase1, testDir)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRnnSeqClassification2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRnnSeqClassification2()
			IntegrationTestRunner.runTest(RNNTestCases.RnnCsvSequenceClassificationTestCase2, testDir)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRnnCharacter() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRnnCharacter()
			IntegrationTestRunner.runTest(RNNTestCases.RnnCharacterTestCase, testDir)
		End Sub


		' ***** CNN1DTestCases *****
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnn1dCharacter() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCnn1dCharacter()
			IntegrationTestRunner.runTest(CNN1DTestCases.Cnn1dTestCaseCharRNN, testDir)
		End Sub


		' ***** CNN2DTestCases *****
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLenetMnist() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLenetMnist()
			IntegrationTestRunner.runTest(CNN2DTestCases.LenetMnist, testDir)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testYoloHouseNumbers() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testYoloHouseNumbers()
			IntegrationTestRunner.runTest(CNN2DTestCases.YoloHouseNumbers, testDir)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnn2DLenetTransferDropoutRepeatability() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCnn2DLenetTransferDropoutRepeatability()
			IntegrationTestRunner.runTest(CNN2DTestCases.testLenetTransferDropoutRepeatability(), testDir)
		End Sub


		' ***** CNN3DTestCases *****
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnn3dSynthetic() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCnn3dSynthetic()
			IntegrationTestRunner.runTest(CNN3DTestCases.Cnn3dTestCaseSynthetic, testDir)
		End Sub

		' ***** UnsupervisedTestCases *****
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVAEMnistAnomaly() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testVAEMnistAnomaly()
			IntegrationTestRunner.runTest(UnsupervisedTestCases.VAEMnistAnomaly, testDir)
		End Sub

		' ***** TransferLearningTestCases *****
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVgg16Transfer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testVgg16Transfer()
			IntegrationTestRunner.runTest(CNN2DTestCases.VGG16TransferTinyImagenet, testDir)
		End Sub
	End Class

End Namespace