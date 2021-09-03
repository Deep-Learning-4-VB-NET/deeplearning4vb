Imports ColumnType = org.datavec.api.transform.ColumnType
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

Namespace org.datavec.api.transform.schema

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class TestSchemaMethods extends org.nd4j.common.tests.BaseND4JTest
	Public Class TestSchemaMethods
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNumberedColumnAdding()
		Public Overridable Sub testNumberedColumnAdding()

'JAVA TO VB CONVERTER NOTE: The variable schema was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim schema_Conflict As Schema = (New Schema.Builder()).addColumnsDouble("doubleCol_%d", 0, 2).addColumnsLong("longCol_%d", 3, 5).addColumnsString("stringCol_%d", 6, 8).build()

			assertEquals(9, schema_Conflict.numColumns())

			For i As Integer = 0 To 8
				If i <= 2 Then
					assertEquals("doubleCol_" & i, schema_Conflict.getName(i))
					assertEquals(ColumnType.Double, schema_Conflict.getType(i))
				ElseIf i <= 5 Then
					assertEquals("longCol_" & i, schema_Conflict.getName(i))
					assertEquals(ColumnType.Long, schema_Conflict.getType(i))
				Else
					assertEquals("stringCol_" & i, schema_Conflict.getName(i))
					assertEquals(ColumnType.String, schema_Conflict.getType(i))
				End If
			Next i

		End Sub

	End Class

End Namespace