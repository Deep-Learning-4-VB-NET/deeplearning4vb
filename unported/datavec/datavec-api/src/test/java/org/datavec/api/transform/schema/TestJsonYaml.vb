Imports Microsoft.VisualBasic
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports DateTimeZone = org.joda.time.DateTimeZone
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
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) @Tag(TagNames.JACKSON_SERDE) public class TestJsonYaml extends org.nd4j.common.tests.BaseND4JTest
	Public Class TestJsonYaml
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToFromJsonYaml()
		Public Overridable Sub testToFromJsonYaml()

'JAVA TO VB CONVERTER NOTE: The variable schema was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim schema_Conflict As Schema = (New Schema.Builder()).addColumnCategorical("Cat", "State1", "State2").addColumnDouble("Dbl").addColumnDouble("Dbl2", Nothing, 100.0, True, False).addColumnInteger("Int").addColumnInteger("Int2", 0, 10).addColumnLong("Long").addColumnLong("Long2", -100L, Nothing).addColumnString("Str").addColumnString("Str2", "someregexhere", 1, Nothing).addColumnTime("TimeCol", DateTimeZone.UTC).addColumnTime("TimeCol2", DateTimeZone.UTC, Nothing, 1000L).addColumnNDArray("ndarray", New Long(){1, 10}).addColumnBoolean("boolean").addColumnFloat("float").addColumnFloat("float2", -100f, 100f, True, False).build()

			Dim asJson As String = schema_Conflict.toJson()
			'        System.out.println(asJson);

			Dim schema2 As Schema = Schema.fromJson(asJson)

			Dim count As Integer = schema_Conflict.numColumns()
			For i As Integer = 0 To count - 1
				Dim c1 As ColumnMetaData = schema_Conflict.getMetaData(i)
				Dim c2 As ColumnMetaData = schema2.getMetaData(i)
				assertEquals(c1, c2)
			Next i
			assertEquals(schema_Conflict, schema2)


			Dim asYaml As String = schema_Conflict.toYaml()
			'        System.out.println(asYaml);

			Dim schema3 As Schema = Schema.fromYaml(asYaml)
			Dim i As Integer = 0
			Do While i < schema_Conflict.numColumns()
				Dim c1 As ColumnMetaData = schema_Conflict.getMetaData(i)
				Dim c3 As ColumnMetaData = schema3.getMetaData(i)
				assertEquals(c1, c3)
				i += 1
			Loop
			assertEquals(schema_Conflict, schema3)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMissingPrimitives()
		Public Overridable Sub testMissingPrimitives()

'JAVA TO VB CONVERTER NOTE: The variable schema was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim schema_Conflict As Schema = (New Schema.Builder()).addColumnDouble("Dbl2", Nothing, 100.0, False, False).build()
			'Legacy format JSON
			Dim strJson As String = "{" & vbLf & "  ""Schema"" : {" & vbLf & "    ""columns"" : [ {" & vbLf & "      ""Double"" : {" & vbLf & "        ""name"" : ""Dbl2""," & vbLf & "        ""maxAllowedValue"" : 100.0" & vbLf & "      }" & vbLf & "    } ]" & vbLf & "  }" & vbLf & "}"

			Dim schema2 As Schema = Schema.fromJson(strJson)
			assertEquals(schema_Conflict, schema2)



			Dim strYaml As String = "--- !<Schema>" & vbLf & "columns:" & vbLf & "- !<Double>" & vbLf & "  name: ""Dbl2""" & vbLf & "  maxAllowedValue: 100.0"
			'"  allowNaN: false\n" +                       //Normally included: but exclude here to test
			'"  allowInfinite: false";                     //Normally included: but exclude here to test

	'        Schema schema2a = Schema.fromYaml(strYaml);
	'        assertEquals(schema, schema2a);
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToFromJsonYamlSequence()
		Public Overridable Sub testToFromJsonYamlSequence()

'JAVA TO VB CONVERTER NOTE: The variable schema was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim schema_Conflict As Schema = (New SequenceSchema.Builder()).addColumnCategorical("Cat", "State1", "State2").addColumnDouble("Dbl").addColumnDouble("Dbl2", Nothing, 100.0, True, False).addColumnInteger("Int").addColumnInteger("Int2", 0, 10).addColumnLong("Long").addColumnLong("Long2", -100L, Nothing).addColumnString("Str").addColumnString("Str2", "someregexhere", 1, Nothing).addColumnTime("TimeCol", DateTimeZone.UTC).addColumnTime("TimeCol2", DateTimeZone.UTC, Nothing, 1000L).build()

			Dim asJson As String = schema_Conflict.toJson()
			'        System.out.println(asJson);

			Dim schema2 As Schema = Schema.fromJson(asJson)

			Dim count As Integer = schema_Conflict.numColumns()
			For i As Integer = 0 To count - 1
				Dim c1 As ColumnMetaData = schema_Conflict.getMetaData(i)
				Dim c2 As ColumnMetaData = schema2.getMetaData(i)
				assertEquals(c1, c2)
			Next i
			assertEquals(schema_Conflict, schema2)


			Dim asYaml As String = schema_Conflict.toYaml()
			'        System.out.println(asYaml);

			Dim schema3 As Schema = Schema.fromYaml(asYaml)
			Dim i As Integer = 0
			Do While i < schema_Conflict.numColumns()
				Dim c1 As ColumnMetaData = schema_Conflict.getMetaData(i)
				Dim c3 As ColumnMetaData = schema3.getMetaData(i)
				assertEquals(c1, c3)
				i += 1
			Loop
			assertEquals(schema_Conflict, schema3)

		End Sub

	End Class

End Namespace