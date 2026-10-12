Imports System
Imports System.Collections.Generic
Imports Table = com.google.flatbuffers.Table
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports VariableType = org.nd4j.autodiff.samediff.VariableType
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Variable = org.nd4j.autodiff.samediff.internal.Variable
Imports FlatBuffersMapper = org.nd4j.autodiff.samediff.serde.FlatBuffersMapper
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports FlatArray = org.nd4j.graph.FlatArray
Imports UIAddName = org.nd4j.graph.UIAddName
Imports UIEvent = org.nd4j.graph.UIEvent
Imports UIGraphStructure = org.nd4j.graph.UIGraphStructure
Imports UIInfoType = org.nd4j.graph.UIInfoType
Imports UIOp = org.nd4j.graph.UIOp
Imports UIVariable = org.nd4j.graph.UIVariable
Imports LogFileWriter = org.nd4j.graph.ui.LogFileWriter
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports org.nd4j.common.primitives
import static org.junit.jupiter.api.Assertions.assertEquals
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

Namespace org.nd4j.autodiff.ui


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @NativeTag public class FileReadWriteTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class FileReadWriteTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir Path testDir;
		Friend testDir As Path

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before()
		Public Overridable Sub before()
			Nd4j.create(1)
			Nd4j.setDefaultDataTypes(DataType.DOUBLE, DataType.DOUBLE)
			Nd4j.Random.setSeed(123)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSimple(org.nd4j.linalg.factory.Nd4jBackend backend) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSimple(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim v As SDVariable = sd.var("variable", DataType.DOUBLE, 3, 4)
			Dim sum As SDVariable = v.sum()

			Dim f As File = testDir.resolve("new-dir-1").toFile()
			f.mkdirs()
			If f.exists() Then
				f.delete()
			End If
			Console.WriteLine(f.getAbsolutePath())


			Dim w As New LogFileWriter(f)
			Dim bytesWritten As Long = w.writeGraphStructure(sd)
			Dim bytesWritten2 As Long = w.writeFinishStaticMarker()

			assertTrue(bytesWritten > 0)
			assertTrue(bytesWritten2 > 0)

			Dim read As LogFileWriter.StaticInfo = w.readStatic()
			assertEquals(2, read.getData().size())

			Dim fileLength As Long = f.length()
			assertEquals(fileLength, read.getFileOffset())

			'Check graph structure:
				'Inputs
			Dim s As UIGraphStructure = CType(read.getData().get(0).getSecond(), UIGraphStructure)
			Dim l As IList(Of String) = New List(Of String)(s.inputsLength())
			Dim i As Integer = 0
			Do While i < s.inputsLength()
				l.Add(s.inputs(i))
				i += 1
			Loop
			assertEquals(sd.inputs(), l)

				'Outputs
			Dim outputs As IList(Of String) = New List(Of String)(s.outputsLength())
			i = 0
			Do While i < s.outputsLength()
				outputs.Add(s.outputs(i))
				i += 1
			Loop
			If outputs.Count = 0 Then
				outputs = Nothing
			End If
			assertEquals(sd.outputs(), outputs)

				'Check variables
			Dim numVars As Integer = s.variablesLength()
			Dim varsList As IList(Of UIVariable) = New List(Of UIVariable)(numVars)
			Dim varsMap As IDictionary(Of String, UIVariable) = New Dictionary(Of String, UIVariable)()
			For i As Integer = 0 To numVars - 1
				Dim uivar As UIVariable = s.variables(i)
				varsList.Add(uivar)
				Dim name As String = uivar.name()
				varsMap(name) = uivar
			Next i

			Dim sdVarsMap As IDictionary(Of String, Variable) = sd.getVariables()
			assertEquals(sdVarsMap.Keys, varsMap.Keys)
			For Each vName As String In sdVarsMap.Keys
				Dim vt As VariableType = sdVarsMap(vName).getVariable().getVariableType()
				Dim vt2 As VariableType = FlatBuffersMapper.fromVarType(varsMap(vName).type())
				assertEquals(vt, vt2)

				'TODO check inputs to, output of, etc
			Next vName

			'Check ops
			Dim numOps As Integer = s.opsLength()
			Dim opsList As IList(Of UIOp) = New List(Of UIOp)(numVars)
			Dim opMap As IDictionary(Of String, UIOp) = New Dictionary(Of String, UIOp)()
			For i As Integer = 0 To numOps - 1
				Dim uiop As UIOp = s.ops(i)
				opsList.Add(uiop)
				Dim name As String = uiop.name()
				opMap(name) = uiop
			Next i

			Dim sdOpsMap As IDictionary(Of String, SameDiffOp) = sd.getOps()
			assertEquals(sdOpsMap.Keys, opMap.Keys)
			'TODO check inputs, outputs etc

			assertEquals(UIInfoType.START_EVENTS, read.getData().get(1).getFirst().infoType())

			'Append a number of events
			w.registerEventName("accuracy")
			For iter As Integer = 0 To 2
				Dim t As Long = DateTimeHelper.CurrentUnixTimeMillis()
				w.writeScalarEvent("accuracy", LogFileWriter.EventSubtype.EVALUATION, t, iter, 0, 0.5 + 0.1 * iter)
			Next iter

			'Read events back in...
			Dim events As IList(Of Pair(Of UIEvent, Table)) = w.readEvents()
			assertEquals(4, events.Count) 'add name + 3 scalars

			Dim addName As UIAddName = CType(events(0).getRight(), UIAddName)
			assertEquals("accuracy", addName.name())

			For i As Integer = 1 To 3
				Dim fa As FlatArray = CType(events(i).getRight(), FlatArray)
				Dim arr As INDArray = Nd4j.createFromFlatArray(fa)

				Dim exp As INDArray = Nd4j.scalar(0.5 + (i - 1) * 0.1)
				assertEquals(exp, arr)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNullBinLabels(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNullBinLabels(ByVal backend As Nd4jBackend)
			Dim dir As File = testDir.resolve("new-dir").toFile()
			dir.mkdirs()
			Dim f As New File(dir, "temp.bin")
			Dim w As New LogFileWriter(f)

			Dim sd As SameDiff = SameDiff.create()
			Dim v As SDVariable = sd.var("variable", DataType.DOUBLE, 3, 4)
			Dim sum As SDVariable = v.sum()

			w.writeGraphStructure(sd)
			w.writeFinishStaticMarker()

			w.registerEventName("name")
			Dim arr As INDArray = Nd4j.create(1)
			w.writeHistogramEventDiscrete("name", LogFileWriter.EventSubtype.TUNING_METRIC, DateTimeHelper.CurrentUnixTimeMillis(), 0, 0, Nothing, arr)
		End Sub
	End Class

End Namespace