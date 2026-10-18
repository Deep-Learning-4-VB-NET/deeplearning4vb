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
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Text.Json
Imports System.Text.Json.Nodes
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.writable
Imports CategoricalAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.CategoricalAnalysis
Imports CategoricalToIntegerTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.categorical.CategoricalToIntegerTransform
Imports ColumnAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.ColumnAnalysis
Imports ColumnMetaDataT = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports ColumnTypeT = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports ConditionOpT = DeepLearning4VB.org.datavec.api.transform.condition.ConditionOp
Imports ConvertFromSequenceT = DeepLearning4VB.org.datavec.api.transform.sequence.ConvertFromSequence
Imports ConvertToSequenceT = DeepLearning4VB.org.datavec.api.transform.sequence.ConvertToSequence
Imports DataActionT = DeepLearning4VB.org.datavec.api.transform.DataAction
Imports DataAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.DataAnalysis
Imports DateTimeFieldType = DeepLearning4VB.org.joda.time.DateTimeFieldType
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports DeriveColumnsFromTimeTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.time.DeriveColumnsFromTimeTransform
Imports DoubleAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.DoubleAnalysis
Imports DoubleColumnConditionT = DeepLearning4VB.org.datavec.api.transform.condition.column.DoubleColumnCondition
Imports DoubleMathOpTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.doubletransform.DoubleMathOpTransform
Imports ReduceOpT = DeepLearning4VB.org.datavec.api.transform.ReduceOp
Imports ReducerT = DeepLearning4VB.org.datavec.api.transform.reduce.Reducer
Imports JavaJsonWriterT = DeepLearning4VB.org.datavec.api.transform.serde.JavaJsonWriter
Imports JsonMappersT = DeepLearning4VB.org.datavec.api.transform.serde.JsonMappers
Imports JsonSerializerT = DeepLearning4VB.org.datavec.api.transform.serde.JsonSerializer
Imports LegacyJsonFormatT = DeepLearning4VB.org.datavec.api.transform.serde.legacy.LegacyJsonFormat
Imports MathOpT = DeepLearning4VB.org.datavec.api.transform.MathOp
Imports MinMaxNormalizerT = DeepLearning4VB.org.datavec.api.transform.transforms.doubletransform.MinMaxNormalizer
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports StringMapTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.string.StringMapTransform
Imports StringToTimeTransformT = DeepLearning4VB.org.datavec.api.transform.transforms.time.StringToTimeTransform
Imports TDigest = DeepLearning4VB.com.tdunning.math.stats.TDigest
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.serde

    ''' <summary>
    ''' Tests of the JSON mapping of the DataVec object model (<see cref="JsonMappersT"/>): the layout and number format
    ''' of Java's Jackson output, Java class names, JSON variants written by Java (property order, unknown properties,
    ''' named floating point values, the 1.0.0-alpha formats), Joda/locale/t-digest values and the use of the options
    ''' with System.Text.Json directly.
    ''' </summary>
    <TestClass>
    Public Class JsonMappersTest

        Private Shared ReadOnly nl As String = Environment.NewLine

        <TestMethod>
        Public Sub testJacksonLayout()
            Dim json As String = JsonMappersT.writeValueAsString(New DoubleMathOpTransformT("x", MathOpT.Add, 0.00001))
            assertEquals("{" & nl &
                         "  ""@class"" : ""org.datavec.api.transform.transform.doubletransform.DoubleMathOpTransform""," & nl &
                         "  ""columnName"" : ""x""," & nl &
                         "  ""mathOp"" : ""Add""," & nl &
                         "  ""scalar"" : 1.0E-5" & nl &
                         "}", json)

            ' empty objects and arrays, nested objects in arrays
            assertEquals("{" & nl & "  ""convertFromSequence"" : { }" & nl & "}", JsonMappersT.writeValueAsString(New DataActionT(New ConvertFromSequenceT())))
            Dim reducerJson As String = JsonMappersT.writeValueAsString((New ReducerT.Builder(ReduceOpT.TakeFirst)).build())
            assertTrue(reducerJson.StartsWith("{" & nl & "  ""Reducer"" : {" & nl & "    ""conditionalReductions"" : { },"), reducerJson)
            assertTrue(reducerJson.Contains("""ignoreInvalidInColumns"" : [ ],"), reducerJson)
            Dim schemaJson As String = (New SchemaT.Builder()).addColumnCategorical("c", "a", "b").addColumnInteger("i").build().toJson()
            assertEquals("{" & nl &
                         "  ""@class"" : ""org.datavec.api.transform.schema.Schema""," & nl &
                         "  ""columns"" : [ {" & nl &
                         "    ""@class"" : ""org.datavec.api.transform.metadata.CategoricalMetaData""," & nl &
                         "    ""name"" : ""c""," & nl &
                         "    ""stateNames"" : [ ""a"", ""b"" ]" & nl &
                         "  }, {" & nl &
                         "    ""@class"" : ""org.datavec.api.transform.metadata.IntegerMetaData""," & nl &
                         "    ""name"" : ""i""" & nl &
                         "  } ]" & nl &
                         "}", schemaJson)
        End Sub

        <TestMethod>
        Public Sub testNullsAndCreatorPropertyOrder()
            ' ConvertToSequence is not @JsonInclude(NON_NULL): nulls are written; creator properties come first
            Dim json As String = JsonMappersT.writeValueAsString(New DataActionT(New ConvertToSequenceT(True, Nothing, Nothing)))
            assertEquals("{" & nl &
                         "  ""convertToSequence"" : {" & nl &
                         "    ""comparator"" : null," & nl &
                         "    ""singleStepSequencesMode"" : true," & nl &
                         "    ""keyColumns"" : null" & nl &
                         "  }" & nl &
                         "}", json)
            Dim back As DataActionT = JsonMappersT.readValue(Of DataActionT)(json)
            assertTrue(back.getConvertToSequence().isSingleStepSequencesMode())
            Assert.IsNull(back.getConvertToSequence().getKeyColumns())
        End Sub

        <TestMethod>
        Public Sub testJavaNumberFormat()
            assertEquals("1.0", JavaJsonWriterT.javaDoubleToString(1.0))
            assertEquals("100.0", JavaJsonWriterT.javaDoubleToString(100.0))
            assertEquals("-2.5", JavaJsonWriterT.javaDoubleToString(-2.5))
            assertEquals("0.0", JavaJsonWriterT.javaDoubleToString(0.0))
            assertEquals("-0.0", JavaJsonWriterT.javaDoubleToString(BitConverter.Int64BitsToDouble(&H8000000000000000L)))
            assertEquals("0.001", JavaJsonWriterT.javaDoubleToString(0.001))
            assertEquals("1.0E-4", JavaJsonWriterT.javaDoubleToString(0.0001))
            assertEquals("1.5E-4", JavaJsonWriterT.javaDoubleToString(0.00015))
            assertEquals("1234567.0", JavaJsonWriterT.javaDoubleToString(1234567.0))
            assertEquals("1.0E7", JavaJsonWriterT.javaDoubleToString(10000000.0))
            assertEquals("1.23456789123E8", JavaJsonWriterT.javaDoubleToString(123456789.123))
            assertEquals("3.14159", JavaJsonWriterT.javaDoubleToString(3.14159))
            assertEquals("0.1", JavaJsonWriterT.javaDoubleToString(0.1))
            assertEquals("1.0E-300", JavaJsonWriterT.javaDoubleToString(1.0E-300))
            assertEquals("1.7976931348623157E308", JavaJsonWriterT.javaDoubleToString(Double.MaxValue))
            assertEquals("NaN", JavaJsonWriterT.javaDoubleToString(Double.NaN))
            assertEquals("3.14159", JavaJsonWriterT.javaFloatToString(3.14159F))
            assertEquals("1.5E10", JavaJsonWriterT.javaFloatToString(1.5E10F))
            assertEquals("100.0", JavaJsonWriterT.javaFloatToString(100.0F))

            ' integers stay integers, floating point fields get Java's format
            Dim json As String = JsonMappersT.writeValueAsString(New MinMaxNormalizerT("c", 0, 10, -1, 1))
            assertTrue(json.Contains("""max"" : 10.0,"), json)
            assertTrue(json.Contains("""newMin"" : -1.0"), json)
        End Sub

        <TestMethod>
        Public Sub testNamedFloatingPointValuesAndEscaping()
            Dim c As New DoubleColumnConditionT("d", ConditionOpT.Equal, Double.PositiveInfinity)
            Dim json As String = JsonMappersT.writeValueAsString(c)
            assertTrue(json.Contains("""value"" : ""Infinity"""), json)
            assertEquals(c, JsonMappersT.readValue(json, GetType(DoubleColumnConditionT)))

            Dim map As New Dictionary(Of String, String) From {{"a""b", "tab" & vbTab & "here"}, {"back\slash", "é" & ChrW(1) & "/"}}
            Dim t As New StringMapTransformT("col", map)
            json = JsonMappersT.writeValueAsString(t)
            assertTrue(json.Contains("""a\""b"" : ""tab\there"""), json)
            assertTrue(json.Contains("""back\\slash"" : ""é\u0001/"""), json)
            assertEquals(t, JsonMappersT.readValue(json, GetType(TransformT)))
        End Sub

        <TestMethod>
        Public Sub testJavaClassNames()
            assertEquals("org.datavec.api.transform.transform.categorical.CategoricalToIntegerTransform", JsonMappersT.getJavaClassName(GetType(CategoricalToIntegerTransformT)))
            assertEquals(GetType(CategoricalToIntegerTransformT), JsonMappersT.getClassForJavaClassName("org.datavec.api.transform.transform.categorical.CategoricalToIntegerTransform"))
            assertEquals("org.datavec.api.transform.schema.Schema", JsonMappersT.getJavaClassName(GetType(SchemaT)))
            assertEquals("org.datavec.api.writable.Text", JsonMappersT.getJavaClassName(GetType(Text)))
            ' nested classes use '$', as Class.getName() in Java
            assertEquals("org.datavec.api.transform.transform.time.DeriveColumnsFromTimeTransform$DerivedColumn", JsonMappersT.getJavaClassName(GetType(DeriveColumnsFromTimeTransformT.DerivedColumn)))
            Assert.IsNull(JsonMappersT.getClassForJavaClassName("com.example.DoesNotExist"))
        End Sub

        <TestMethod>
        Public Sub testJavaJsonVariants()
            ' "@class" not first, a property this version does not know, an omitted (null) field, extra whitespace
            Dim json As String = "{ ""columnName"":""c"", ""unknownField"": {""a"": [1, 2]}, ""@class"": ""org.datavec.api.transform.transform.doubletransform.DoubleMathOpTransform"", ""scalar"": 2, ""mathOp"": ""Multiply"" }"
            Dim t As TransformT = (New JsonSerializerT()).deserializeTransform(json)
            assertEquals(New DoubleMathOpTransformT("c", MathOpT.Multiply, 2.0), t)

            ' unknown type ids fail, as in Java
            assertThrows(Of JsonException)(Sub() Call (New JsonSerializerT()).deserializeTransform("{""@class"" : ""com.example.MissingTransform""}"))
            ' malformed JSON
            assertThrows(Of JsonException)(Sub() Call TransformProcessT.fromJson("{ ""actionList"" : [ "))
        End Sub

        <TestMethod>
        Public Sub testJodaAndLocaleValues()
            Dim t As New StringToTimeTransformT("col", "dd/MM/YYYY", DateTimeZone.forID("America/New_York"), CultureInfo.GetCultureInfo("fr-FR"))
            Dim json As String = JsonMappersT.writeValueAsString(t)
            assertTrue(json.Contains("""timeZone"" : ""America/New_York"""), json)
            assertTrue(json.Contains("""locale"" : ""fr_FR"""), json)
            Dim back As StringToTimeTransformT = DirectCast(JsonMappersT.readValue(json, GetType(TransformT)), StringToTimeTransformT)
            assertEquals(t, back)
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("col").build()
            back.InputSchema = schema
            t.InputSchema = schema
            assertEquals(t.map(New List(Of WritableT) From {New Text("25/12/2016")}), back.map(New List(Of WritableT) From {New Text("25/12/2016")}))

            ' DateTimeFieldType: {"fieldType" : "..."} as Java writes it; a bare name is accepted too
            Dim derived As DeriveColumnsFromTimeTransformT = (New DeriveColumnsFromTimeTransformT.Builder("time")).addIntegerDerivedColumn("dow", DateTimeFieldType.dayOfWeek()).build()
            json = JsonMappersT.writeValueAsString(derived)
            assertTrue(json.Contains("""fieldType"" : {" & nl & "      ""fieldType"" : ""dayOfWeek""" & nl & "    }"), json)
            assertEquals(derived, JsonMappersT.readValue(json, GetType(TransformT)))
            Dim bare As String = json.Replace("""fieldType"" : {" & nl & "      ""fieldType"" : ""dayOfWeek""" & nl & "    }", """fieldType"" : ""dayOfWeek""")
            assertEquals(derived, JsonMappersT.readValue(bare, GetType(TransformT)))
        End Sub

        <TestMethod>
        Public Sub testTDigestValues()
            Dim digest As TDigest = TDigest.createDigest(100)
            For i As Integer = 1 To 100
                digest.add(i)
            Next
            Dim a As DoubleAnalysisT = (New DoubleAnalysisT.Builder()).min(1).max(100).mean(50.5).digest(digest).build()
            Dim json As String = JsonMappersT.writeValueAsString(a, GetType(ColumnAnalysisT))
            Dim back As DoubleAnalysisT = DirectCast(JsonMappersT.readValue(json, GetType(ColumnAnalysisT)), DoubleAnalysisT)
            assertEquals(a, back)
            assertEquals(digest.quantile(0.5), back.getDigest().quantile(0.5), 1.0E-9)

            ' Java wrote a Java serialization of its MergingDigest: it cannot be read and is dropped
            Dim javaJson As String = "{""@class"":""org.datavec.api.transform.analysis.columns.DoubleAnalysis"",""countTotal"":3,""digest"":{""digest"":""rO0ABXNyAC1jb20udGR1bm5pbmcubWF0aC5zdGF0cy5NZXJnaW5nRGlnZXN0""},""max"":3.0,""mean"":2.0,""min"":1.0}"
            Dim fromJava As DoubleAnalysisT = DirectCast(JsonMappersT.readValue(javaJson, GetType(ColumnAnalysisT)), DoubleAnalysisT)
            Assert.IsNull(fromJava.getDigest())
            assertEquals(3L, fromJava.CountTotal)
            assertEquals(3.0, fromJava.MaxDouble, 0.0)
        End Sub

        <TestMethod>
        Public Sub testLegacyDataAnalysisFormat()
            Dim legacy As String = "{" & vbLf &
                "  ""DataAnalysis"" : [ {" & vbLf &
                "    ""columnName"" : ""d""," & vbLf &
                "    ""columnIndex"" : 0," & vbLf &
                "    ""columnType"" : ""Double""," & vbLf &
                "    ""analysis"" : {" & vbLf &
                "      ""DoubleAnalysis"" : {" & vbLf &
                "        ""min"" : -1.0," & vbLf &
                "        ""max"" : 1.0," & vbLf &
                "        ""mean"" : 0.5," & vbLf &
                "        ""countTotal"" : 4" & vbLf &
                "      }" & vbLf &
                "    }" & vbLf &
                "  }, {" & vbLf &
                "    ""columnName"" : ""c""," & vbLf &
                "    ""columnIndex"" : 1," & vbLf &
                "    ""columnType"" : ""Categorical""," & vbLf &
                "    ""stateNames"" : [ ""x"", ""y"" ]," & vbLf &
                "    ""analysis"" : {" & vbLf &
                "      ""CategoricalAnalysis"" : {" & vbLf &
                "        ""mapOfCounts"" : {" & vbLf &
                "          ""x"" : 1," & vbLf &
                "          ""y"" : 3" & vbLf &
                "        }" & vbLf &
                "      }" & vbLf &
                "    }" & vbLf &
                "  } ]" & vbLf &
                "}"
            Dim da As DataAnalysisT = DataAnalysisT.fromJson(legacy)
            assertEquals((New SchemaT.Builder()).addColumnDouble("d").addColumnCategorical("c", "x", "y").build(), da.getSchema())
            Dim d As DoubleAnalysisT = DirectCast(da.getColumnAnalysis("d"), DoubleAnalysisT)
            assertEquals(1.0, d.MaxDouble, 0.0)
            assertEquals(4L, d.CountTotal)
            Dim c As CategoricalAnalysisT = DirectCast(da.getColumnAnalysis("c"), CategoricalAnalysisT)
            assertEquals(4L, c.CountTotal)
        End Sub

        <TestMethod>
        Public Sub testLegacyConversionLeavesCurrentFormatUnchanged()
            Dim tp As TransformProcessT = (New TransformProcessT.Builder((New SchemaT.Builder()).addColumnDouble("d").build())).doubleMathOp("d", MathOpT.Add, 1).build()
            Dim node As JsonNode = JsonNode.Parse(tp.toJson())
            Dim before As String = node.ToJsonString()
            Dim after As JsonNode = LegacyJsonFormatT.convertLegacyJson(node, GetType(TransformProcessT), JsonMappersT.Mapper)
            assertEquals(before, after.ToJsonString())

            ' a legacy wrapper is rewritten into the "@class" form
            Dim legacy As JsonNode = JsonNode.Parse("{""Integer"" : {""name"" : ""i"", ""maxAllowedValue"" : 5}}")
            Dim converted As JsonNode = LegacyJsonFormatT.convertLegacyJson(legacy, GetType(ColumnMetaDataT), JsonMappersT.Mapper)
            assertEquals("{""@class"":""org.datavec.api.transform.metadata.IntegerMetaData"",""name"":""i"",""maxAllowedValue"":5}", converted.ToJsonString())
        End Sub

        <TestMethod>
        Public Sub testWritablesInJavaFormat()
            ' Java writes the whole backing array of a Text and its valid length
            assertEquals(New Text("abc"), JsonMappersT.readValue(Of WritableT)("{""@class"":""org.datavec.api.writable.Text"",""bytes"":""YWJjZGVm"",""length"":3}"))
            Dim writables As New List(Of WritableT) From {New IntWritable(1), New LongWritable(-2), New DoubleWritable(2.5), New FloatWritable(0.5F), New BooleanWritable(True), New ByteWritable(-3), New Text("tëxt"), NullWritable.INSTANCE, New BytesWritable(New Byte() {1, 2, 255})}
            For Each w As WritableT In writables
                Dim json As String = JsonMappersT.writeValueAsString(w, GetType(WritableT))
                assertTrue(json.Contains("""@class"" : ""org.datavec.api.writable."), json)
                assertEquals(w, JsonMappersT.readValue(Of WritableT)(json))
            Next
            assertTrue(JsonMappersT.writeValueAsString(New DoubleWritable(2.0)).Contains("""value"" : 2.0"))
            assertTrue(JsonMappersT.writeValueAsString(New BytesWritable(New Byte() {1, 2, 255})).Contains("""content"" : ""AQL/"""))
        End Sub

        <TestMethod>
        Public Sub testMapperWithSystemTextJson()
            ' the options can be used with System.Text.Json directly (compact output, same structure)
            Dim t As TransformT = New CategoricalToIntegerTransformT("cat")
            Dim json As String = System.Text.Json.JsonSerializer.Serialize(t, GetType(TransformT), JsonMappersT.Mapper)
            assertEquals("{""@class"":""org.datavec.api.transform.transform.categorical.CategoricalToIntegerTransform"",""columnName"":""cat""}", json)
            assertEquals(t, System.Text.Json.JsonSerializer.Deserialize(json, GetType(TransformT), JsonMappersT.Mapper))
            assertTrue(JsonMappersT.getMapper() Is JsonMappersT.Mapper)
            assertTrue((New JsonSerializerT()).getObjectMapper() Is JsonMappersT.Mapper)
        End Sub

    End Class

End Namespace
