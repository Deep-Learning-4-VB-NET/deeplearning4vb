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
Imports DeepLearning4VB.Tests.org.datavec.api.transform.serde.testClasses
Imports JsonMappersT = DeepLearning4VB.org.datavec.api.transform.serde.JsonMappers
Imports MathOpT = DeepLearning4VB.org.datavec.api.transform.MathOp
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess

Namespace org.datavec.api.transform.serde

    ''' <summary>
    ''' Port of the DataVec <c>TestCustomTransformJsonYaml</c> JUnit class: user-defined transforms, filters and
    ''' conditions in a transform process survive a JSON round trip. YAML is not supported, so only the JSON half is
    ''' ported. Implementations in user assemblies are found without registration; a registered Java class name is
    ''' written instead of the .NET name and read back from Java JSON.
    ''' </summary>
    <TestClass>
    Public Class TestCustomTransformJsonYaml

        Private Shared Function buildProcess() As TransformProcessT
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnInteger("firstCol").addColumnDouble("secondCol").build()

            Return (New TransformProcessT.Builder(schema)).integerMathOp("firstCol", MathOpT.Add, 1).transform(New CustomTransform("secondCol", 3.14159)).doubleMathOp("secondCol", MathOpT.Multiply, 2.0).filter(New CustomFilter(123)).filter(New CustomCondition("someArg")).build()
        End Function

        <TestMethod>
        Public Sub testCustomTransform()
            Dim tp As TransformProcessT = buildProcess()

            Dim asJson As String = tp.toJson()
            Dim fromJson As TransformProcessT = TransformProcessT.fromJson(asJson)

            assertEquals(tp, fromJson)
            assertEquals(3.14159, DirectCast(fromJson.getActionList()(1).getTransform(), CustomTransform).getSomeArg(), 0.0)
            assertEquals(1, fromJson.getActionList()(1).getTransform().InputSchema.getIndexOfColumn("secondCol"))
        End Sub

        <TestMethod>
        Public Sub testCustomTransformWithJavaClassNames()
            JsonMappersT.registerSubtype(GetType(CustomTransform), "org.datavec.api.transform.serde.testClasses.CustomTransform")
            JsonMappersT.registerSubtype(GetType(CustomFilter), "org.datavec.api.transform.serde.testClasses.CustomFilter")
            JsonMappersT.registerSubtype(GetType(CustomCondition), "org.datavec.api.transform.serde.testClasses.CustomCondition")
            assertEquals("org.datavec.api.transform.serde.testClasses.CustomTransform", JsonMappersT.getJavaClassName(GetType(CustomTransform)))
            assertEquals(GetType(CustomFilter), JsonMappersT.getClassForJavaClassName("org.datavec.api.transform.serde.testClasses.CustomFilter"))

            Dim tp As TransformProcessT = buildProcess()
            Dim asJson As String = tp.toJson()
            assertTrue(asJson.Contains("""@class"" : ""org.datavec.api.transform.serde.testClasses.CustomTransform"""), asJson)
            assertTrue(asJson.Contains("""someFilterArg"" : 123"), asJson)
            assertEquals(tp, TransformProcessT.fromJson(asJson))

            ' The same process as Java DataVec writes it (custom classes in the Java test package)
            Dim javaJson As String = "{" & vbLf &
                "  ""actionList"" : [ {" & vbLf &
                "    ""transform"" : {" & vbLf &
                "      ""@class"" : ""org.datavec.api.transform.transform.integer.IntegerMathOpTransform""," & vbLf &
                "      ""columnName"" : ""firstCol""," & vbLf &
                "      ""mathOp"" : ""Add""," & vbLf &
                "      ""scalar"" : 1" & vbLf &
                "    }" & vbLf &
                "  }, {" & vbLf &
                "    ""transform"" : {" & vbLf &
                "      ""@class"" : ""org.datavec.api.transform.serde.testClasses.CustomTransform""," & vbLf &
                "      ""columnName"" : ""secondCol""," & vbLf &
                "      ""someArg"" : 3.14159" & vbLf &
                "    }" & vbLf &
                "  }, {" & vbLf &
                "    ""transform"" : {" & vbLf &
                "      ""@class"" : ""org.datavec.api.transform.transform.doubletransform.DoubleMathOpTransform""," & vbLf &
                "      ""columnName"" : ""secondCol""," & vbLf &
                "      ""mathOp"" : ""Multiply""," & vbLf &
                "      ""scalar"" : 2.0" & vbLf &
                "    }" & vbLf &
                "  }, {" & vbLf &
                "    ""filter"" : {" & vbLf &
                "      ""@class"" : ""org.datavec.api.transform.serde.testClasses.CustomFilter""," & vbLf &
                "      ""someFilterArg"" : 123" & vbLf &
                "    }" & vbLf &
                "  }, {" & vbLf &
                "    ""filter"" : {" & vbLf &
                "      ""@class"" : ""org.datavec.api.transform.filter.ConditionFilter""," & vbLf &
                "      ""condition"" : {" & vbLf &
                "        ""@class"" : ""org.datavec.api.transform.serde.testClasses.CustomCondition""," & vbLf &
                "        ""someArg"" : ""someArg""" & vbLf &
                "      }" & vbLf &
                "    }" & vbLf &
                "  } ]," & vbLf &
                "  ""initialSchema"" : {" & vbLf &
                "    ""@class"" : ""org.datavec.api.transform.schema.Schema""," & vbLf &
                "    ""columns"" : [ {" & vbLf &
                "      ""@class"" : ""org.datavec.api.transform.metadata.IntegerMetaData""," & vbLf &
                "      ""name"" : ""firstCol""" & vbLf &
                "    }, {" & vbLf &
                "      ""@class"" : ""org.datavec.api.transform.metadata.DoubleMetaData""," & vbLf &
                "      ""allowInfinite"" : false," & vbLf &
                "      ""allowNaN"" : false," & vbLf &
                "      ""name"" : ""secondCol""" & vbLf &
                "    } ]" & vbLf &
                "  }" & vbLf &
                "}"
            assertEquals(tp, TransformProcessT.fromJson(javaJson))
        End Sub

    End Class

End Namespace
