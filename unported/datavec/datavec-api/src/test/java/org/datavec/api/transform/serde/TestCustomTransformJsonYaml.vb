Imports MathOp = org.datavec.api.transform.MathOp
Imports TransformProcess = org.datavec.api.transform.TransformProcess
Imports Schema = org.datavec.api.transform.schema.Schema
Imports CustomCondition = org.datavec.api.transform.serde.testClasses.CustomCondition
Imports CustomFilter = org.datavec.api.transform.serde.testClasses.CustomFilter
Imports CustomTransform = org.datavec.api.transform.serde.testClasses.CustomTransform
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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

Namespace org.datavec.api.transform.serde

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) @Tag(TagNames.JACKSON_SERDE) @Tag(TagNames.CUSTOM_FUNCTIONALITY) public class TestCustomTransformJsonYaml extends org.nd4j.common.tests.BaseND4JTest
	Public Class TestCustomTransformJsonYaml
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCustomTransform()
		Public Overridable Sub testCustomTransform()

			Dim schema As Schema = (New Schema.Builder()).addColumnInteger("firstCol").addColumnDouble("secondCol").build()

			Dim tp As TransformProcess = (New TransformProcess.Builder(schema)).integerMathOp("firstCol", MathOp.Add, 1).transform(New CustomTransform("secondCol", 3.14159)).doubleMathOp("secondCol", MathOp.Multiply, 2.0).filter(New CustomFilter(123)).filter(New CustomCondition("someArg")).build()

			Dim asJson As String = tp.toJson()
			Dim asYaml As String = tp.toYaml()

			Dim fromJson As TransformProcess = TransformProcess.fromJson(asJson)
			Dim fromYaml As TransformProcess = TransformProcess.fromYaml(asYaml)

			assertEquals(tp, fromJson)
			assertEquals(tp, fromYaml)
		End Sub

	End Class

End Namespace