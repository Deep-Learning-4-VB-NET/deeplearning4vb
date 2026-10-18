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

Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ColumnMetaDataT = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema

Namespace org.datavec.api.transform.schema

    ''' <summary>
    ''' Port of the DataVec <c>schema.TestJsonYaml</c> JUnit class. YAML is not supported, so only the JSON halves are
    ''' ported; the NDArray column of testToFromJsonYaml is left out (NDArrayMetaData needs INDArray).
    ''' </summary>
    <TestClass>
    Public Class TestJsonYaml

        <TestMethod>
        Public Sub testToFromJson()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnCategorical("Cat", "State1", "State2").addColumnDouble("Dbl").addColumnDouble("Dbl2", Nothing, 100.0, True, False).addColumnInteger("Int").addColumnInteger("Int2", 0, 10).addColumnLong("Long").addColumnLong("Long2", -100L, Nothing).addColumnString("Str").addColumnString("Str2", "someregexhere", 1, Nothing).addColumnTime("TimeCol", DateTimeZone.UTC).addColumnTime("TimeCol2", DateTimeZone.UTC, Nothing, 1000L).addColumnBoolean("boolean").addColumnFloat("float").addColumnFloat("float2", -100.0F, 100.0F, True, False).build()

            Dim asJson As String = schema.toJson()

            Dim schema2 As SchemaT = SchemaT.fromJson(asJson)

            Dim count As Integer = schema.numColumns()
            For i As Integer = 0 To count - 1
                Dim c1 As ColumnMetaDataT = schema.getMetaData(i)
                Dim c2 As ColumnMetaDataT = schema2.getMetaData(i)
                assertEquals(c1, c2)
            Next i
            assertEquals(schema, schema2)
            assertEquals(schema.getColumnNames(), schema2.getColumnNames())
            assertEquals(13, schema2.getIndexOfColumn("float2"))
        End Sub

        <TestMethod>
        Public Sub testMissingPrimitives()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnDouble("Dbl2", Nothing, 100.0, False, False).build()
            'Legacy format JSON
            Dim strJson As String = "{" & vbLf & "  ""Schema"" : {" & vbLf & "    ""columns"" : [ {" & vbLf & "      ""Double"" : {" & vbLf & "        ""name"" : ""Dbl2""," & vbLf & "        ""maxAllowedValue"" : 100.0" & vbLf & "      }" & vbLf & "    } ]" & vbLf & "  }" & vbLf & "}"

            Dim schema2 As SchemaT = SchemaT.fromJson(strJson)
            assertEquals(schema, schema2)
        End Sub

        <TestMethod>
        Public Sub testToFromJsonSequence()
            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnCategorical("Cat", "State1", "State2").addColumnDouble("Dbl").addColumnDouble("Dbl2", Nothing, 100.0, True, False).addColumnInteger("Int").addColumnInteger("Int2", 0, 10).addColumnLong("Long").addColumnLong("Long2", -100L, Nothing).addColumnString("Str").addColumnString("Str2", "someregexhere", 1, Nothing).addColumnTime("TimeCol", DateTimeZone.UTC).addColumnTime("TimeCol2", DateTimeZone.UTC, Nothing, 1000L).build()

            Dim asJson As String = schema.toJson()

            Dim schema2 As SchemaT = SchemaT.fromJson(asJson)
            assertTrue(TypeOf schema2 Is SequenceSchemaT)

            Dim count As Integer = schema.numColumns()
            For i As Integer = 0 To count - 1
                Dim c1 As ColumnMetaDataT = schema.getMetaData(i)
                Dim c2 As ColumnMetaDataT = schema2.getMetaData(i)
                assertEquals(c1, c2)
            Next i
            assertEquals(schema, schema2)
        End Sub

        <TestMethod>
        Public Sub testSequenceSchemaLengthsAndJavaFormat()
            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).minSequenceLength(2).maxSequenceLength(5).addColumnInteger("x").build()
            Dim json As String = schema.toJson()
            Dim nl As String = Environment.NewLine
            assertEquals("{" & nl &
                         "  ""@class"" : ""org.datavec.api.transform.schema.SequenceSchema""," & nl &
                         "  ""columns"" : [ {" & nl &
                         "    ""@class"" : ""org.datavec.api.transform.metadata.IntegerMetaData""," & nl &
                         "    ""name"" : ""x""" & nl &
                         "  } ]," & nl &
                         "  ""maxSequenceLength"" : 5," & nl &
                         "  ""minSequenceLength"" : 2" & nl &
                         "}", json)
            Dim back As SequenceSchemaT = DirectCast(SchemaT.fromJson(json), SequenceSchemaT)
            assertEquals(schema, back)
            assertEquals(2, back.getMinSequenceLength())
            assertEquals(5, back.getMaxSequenceLength())
        End Sub

    End Class

End Namespace
