Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports org.junit.jupiter.api.Assertions
import static org.junit.jupiter.api.Assertions.assertArrayEquals
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
Namespace org.deeplearning4j.nn.adapters

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Regression 2 d Adapter Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class Regression2dAdapterTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class Regression2dAdapterTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Regression Adapter _ 2 D _ 1") void testRegressionAdapter_2D_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testRegressionAdapter_2D_1()
			Dim [in] As val = New Double()() {
				New Double() { 1, 2, 3 },
				New Double() { 4, 5, 6 }
			}
			Dim adapter As val = New Regression2dAdapter()
			Dim result As val = adapter.apply(Nd4j.create([in]))
			assertArrayEquals(ArrayUtil.flatten([in]), ArrayUtil.flatten(result), 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Regression Adapter _ 2 D _ 2") void testRegressionAdapter_2D_2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testRegressionAdapter_2D_2()
			Dim [in] As val = New Double() { 1, 2, 3 }
			Dim adapter As val = New Regression2dAdapter()
			Dim result As val = adapter.apply(Nd4j.create([in]))
			assertArrayEquals([in], ArrayUtil.flatten(result), 1e-5)
		End Sub
	End Class

End Namespace