Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeAll = org.junit.jupiter.api.BeforeAll
Imports Disabled = org.junit.jupiter.api.Disabled
Imports org.nd4j.autodiff.opvalidation
Imports OpValidation = org.nd4j.autodiff.validation.OpValidation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assumptions.assumeFalse

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

Namespace org.nd4j

	'import org.nd4j.imports.tfgraphs.TFGraphTestAllSameDiff;


	'@Suite.SuiteClasses({
	'        //Note: these will be run as part of the suite only, and will NOT be run again separately
	'        LayerOpValidation.class,
	'        LossOpValidation.class,
	'        MiscOpValidation.class,
	'        RandomOpValidation.class,
	'        ReductionBpOpValidation.class,
	'        ReductionOpValidation.class,
	'        ShapeOpValidation.class,
	'        TransformOpValidation.class,
	'
	'        //TF import tests
	'        //TFGraphTestAllSameDiff.class
	'        //TFGraphTestAllLibnd4j.class
	'})
	'IMPORTANT: This ignore is added to avoid maven surefire running both the suite AND the individual tests in "mvn test"
	' With it ignored here, the individual tests will run outside (i.e., separately/independently) of the suite in both "mvn test" and IntelliJ
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled public class OpValidationSuite
	Public Class OpValidationSuite

	'    
	'    Change this variable from false to true to ignore any tests that call OpValidationSuite.ignoreFailing()
	'
	'    The idea: failing SameDiff tests are disabled by default, but can be re-enabled.
	'    This is so we can prevent regressions on already passing tests
	'     
		Public Const IGNORE_FAILING As Boolean = True

		''' <summary>
		''' NOTE: Do not change this.
		''' If all tests won't run,
		''' it's likely because of a mis specified test name.
		''' Keep this trigger as is for ignoring tests.
		''' </summary>
		Public Shared Sub ignoreFailing()
			'If IGNORE_FAILING
			assumeFalse(IGNORE_FAILING)
		End Sub


		Private Shared initialType As DataType

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll public static void beforeClass()
		Public Shared Sub beforeClass()
			Nd4j.create(1)
			initialType = Nd4j.dataType()
			Nd4j.DataType = DataType.DOUBLE
			Nd4j.Random.setSeed(123)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public static void afterClass()
		Public Shared Sub afterClass()
			Nd4j.DataType = initialType

			' Log coverage information
			OpValidation.logCoverageInformation(True, True, True, True, True)
		End Sub






	End Class

End Namespace