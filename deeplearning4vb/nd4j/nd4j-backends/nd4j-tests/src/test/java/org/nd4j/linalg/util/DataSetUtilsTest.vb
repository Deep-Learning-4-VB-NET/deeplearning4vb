Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports SIS = org.nd4j.common.tools.SIS
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.nd4j.linalg.util


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.FILE_IO) public class DataSetUtilsTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class DataSetUtilsTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir Path tmpFld;
		Friend tmpFld As Path

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

		'

		'
		Private sis As SIS
		'
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAll(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAll(ByVal backend As Nd4jBackend)
			'
			sis = New SIS()
			'
			Dim mtLv As Integer = 0
			'
			sis.initValues(mtLv, "TEST", System.out, System.err, tmpFld.toAbsolutePath().ToString(), "Test", "ABC", True, True)
			'
			Dim in_INDA As INDArray = Nd4j.zeros(8, 8)
			Dim ot_INDA As INDArray = Nd4j.ones(8, 1)
			'
			ot_INDA.putScalar(7, 5)
			'
			Dim ds As New DataSet(in_INDA, ot_INDA)
			'
			Dim dl4jt As New DataSetUtils(sis, "TEST")
			'
			dl4jt.showDataSet(mtLv, "ds", ds, 2, 2, 20, 20)
			'
		'	assertEquals( 100, sis.getcharsCount() );
			'
			assertTrue(sis.getcharsCount() > 1190 AndAlso sis.getcharsCount() < 1210)
			'
			Dim spec_INDA As INDArray = Nd4j.zeros(8, 8)
			'
			dl4jt.showINDArray(mtLv, "spec_INDA", spec_INDA, 3, 20, 20)
			'
		'	assertEquals( 100, sis.getcharsCount() );
			'
			' this test might show different length on different systems due to various regional formatting options.
			assertTrue(sis.getcharsCount() > 2150 AndAlso sis.getcharsCount() < 2170)
			'
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after()
		Public Overridable Sub after()
			'
			Dim mtLv As Integer = 0
			If sis IsNot Nothing Then
				sis.onStop(mtLv)
			End If
			'
		'	tmpFld.delete();
			'
		End Sub



	End Class
End Namespace