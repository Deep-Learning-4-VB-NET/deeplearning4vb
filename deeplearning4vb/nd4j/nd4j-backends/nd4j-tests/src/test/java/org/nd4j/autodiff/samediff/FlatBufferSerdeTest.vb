Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports IOUtils = org.apache.commons.io.IOUtils
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports FlatConfiguration = org.nd4j.graph.FlatConfiguration
Imports FlatGraph = org.nd4j.graph.FlatGraph
Imports FlatNode = org.nd4j.graph.FlatNode
Imports FlatVariable = org.nd4j.graph.FlatVariable
Imports IntPair = org.nd4j.graph.IntPair
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Pooling3DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Pooling3DConfig
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports org.nd4j.linalg.learning
Imports AMSGrad = org.nd4j.linalg.learning.config.AMSGrad
Imports AdaDelta = org.nd4j.linalg.learning.config.AdaDelta
Imports AdaGrad = org.nd4j.linalg.learning.config.AdaGrad
Imports AdaMax = org.nd4j.linalg.learning.config.AdaMax
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports Nadam = org.nd4j.linalg.learning.config.Nadam
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports RmsProp = org.nd4j.linalg.learning.config.RmsProp
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports L1Regularization = org.nd4j.linalg.learning.regularization.L1Regularization
Imports L2Regularization = org.nd4j.linalg.learning.regularization.L2Regularization
Imports WeightDecay = org.nd4j.linalg.learning.regularization.WeightDecay
Imports org.junit.jupiter.api.Assertions

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

Namespace org.nd4j.autodiff.samediff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @Tag(TagNames.SAMEDIFF) @NativeTag public class FlatBufferSerdeTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class FlatBufferSerdeTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir Path testDir;
		Friend testDir As Path


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasic(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBasic(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim arr As INDArray = Nd4j.linspace(1,12,12).reshape(ChrW(3), 4)
			Dim [in] As SDVariable = sd.placeHolder("in", arr.dataType(), arr.shape())
			Dim tanh As SDVariable = sd.math().tanh([in])
			tanh.markAsLoss()

			Dim bb As ByteBuffer = sd.asFlatBuffers(True)

			Dim f As File = Files.createTempFile(testDir,"some-file","bin").toFile()
			f.delete()

			Using fc As java.nio.channels.FileChannel = (New FileStream(f, False)).getChannel()
				fc.write(bb)
			End Using

			Dim bytes() As SByte
			Using [is] As Stream = New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
				bytes = IOUtils.toByteArray([is])
			End Using
			Dim bbIn As ByteBuffer = ByteBuffer.wrap(bytes)

			Dim fg As FlatGraph = FlatGraph.getRootAsFlatGraph(bbIn)

			Dim numNodes As Integer = fg.nodesLength()
			Dim numVars As Integer = fg.variablesLength()
			Dim nodes As IList(Of FlatNode) = New List(Of FlatNode)(numNodes)
			For i As Integer = 0 To numNodes - 1
				nodes.Add(fg.nodes(i))
			Next i
			Dim vars As IList(Of FlatVariable) = New List(Of FlatVariable)(numVars)
			For i As Integer = 0 To numVars - 1
				vars.Add(fg.variables(i))
			Next i

			Dim conf As FlatConfiguration = fg.configuration()

			Dim numOutputs As Integer = fg.outputsLength()
			Dim outputs As IList(Of IntPair) = New List(Of IntPair)(numOutputs)
			For i As Integer = 0 To numOutputs - 1
				outputs.Add(fg.outputs(i))
			Next i

			assertEquals(2, numVars)
			assertEquals(1, numNodes)

			'Check placeholders:
			assertEquals(1, fg.placeholdersLength())
			assertEquals("in", fg.placeholders(0))

			'Check loss variables:
			assertEquals(sd.getLossVariables().Count, fg.lossVariablesLength())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSimple(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSimple(ByVal backend As Nd4jBackend)
			For i As Integer = 0 To 9
				For Each execFirst As Boolean In New Boolean(){False, True}
					log.info("Starting test: i={}, execFirst={}", i, execFirst)
					Dim sd As SameDiff = SameDiff.create()
					Dim arr As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)
					Dim [in] As SDVariable = sd.placeHolder("in", arr.dataType(), arr.shape())
					Dim x As SDVariable
					Select Case i
						Case 0
							'Custom op
							x = sd.cumsum("out", [in], False, False, 1)
						Case 1
							'Transform
							x = sd.math().tanh([in])
						Case 2, 3
							'Reduction
							x = sd.mean("x", [in], i = 2, 1)
						Case 4
							'Transform
							x = sd.math().square([in])
						Case 5, 6
							'Index reduction
							x = sd.argmax("x", [in], i = 5, 1)
						Case 7
							'Scalar:
							x = [in].add(10)
						Case 8
							'Reduce 3:
							Dim y As SDVariable = sd.var("in2", Nd4j.linspace(1,12,12).muli(0.1).addi(0.5).reshape(3,4))
							x = sd.math().cosineSimilarity([in], y)
						Case 9
							'Reduce 3 (along dim)
							Dim z As SDVariable = sd.var("in2", Nd4j.linspace(1,12,12).muli(0.1).addi(0.5).reshape(3,4))
							x = sd.math().cosineSimilarity([in], z, 1)
						Case Else
							Throw New Exception()
					End Select
					If x.dataType().isFPType() Then
						'Can't mark argmax as loss, because it's not FP
						x.markAsLoss()
					End If

					If execFirst Then
						sd.output(Collections.singletonMap("in", arr), Collections.singletonList(x.name()))
					End If

					Dim f As File = Files.createTempFile(testDir,"some-file","fb").toFile()
					f.delete()
					sd.asFlatFile(f)

					Dim restored As SameDiff = SameDiff.fromFlatFile(f)

					Dim varsOrig As IList(Of SDVariable) = sd.variables()
					Dim varsRestored As IList(Of SDVariable) = restored.variables()
					assertEquals(varsOrig.Count, varsRestored.Count)
					For j As Integer = 0 To varsOrig.Count - 1
						assertEquals(varsOrig(j).name(), varsRestored(j).name())
					Next j

					Dim fOrig() As DifferentialFunction = sd.ops()
					Dim fRestored() As DifferentialFunction = restored.ops()
					assertEquals(fOrig.Length, fRestored.Length)

					Dim j As Integer = 0
					Do While j < sd.ops().Length
						assertEquals(fOrig(j).GetType(), fRestored(j).GetType())
						j += 1
					Loop

					assertEquals(sd.getLossVariables(), restored.getLossVariables())


					Dim m As IDictionary(Of String, INDArray) = sd.output(Collections.singletonMap("in", arr), Collections.singletonList(x.name()))
					Dim outOrig As INDArray = m(x.name())
					Dim m2 As IDictionary(Of String, INDArray) = restored.output(Collections.singletonMap("in", arr), Collections.singletonList(x.name()))
					Dim outRestored As INDArray = m2(x.name())

					assertEquals(outOrig, outRestored,i.ToString())


					'Check placeholders
					Dim vBefore As IDictionary(Of String, SDVariable) = sd.variableMap()
					Dim vAfter As IDictionary(Of String, SDVariable) = restored.variableMap()
					assertEquals(vBefore.Keys, vAfter.Keys)
					For Each s As String In vBefore.Keys
						assertEquals(vBefore(s).isPlaceHolder(), vAfter(s).isPlaceHolder(),s)
						assertEquals(vBefore(s).isConstant(), vAfter(s).isConstant(),s)
					Next s


					'Check save methods
					For Each withUpdaterState As Boolean In New Boolean(){False, True}

						Dim f2 As File = Files.createTempFile(testDir,"some-file-2","fb").toFile()
						sd.save(f2, withUpdaterState)
						Dim r2 As SameDiff = SameDiff.load(f2, withUpdaterState)
						assertEquals(varsOrig.Count, r2.variables().Count)
						assertEquals(fOrig.Length, r2.ops().Length)
						assertEquals(sd.getLossVariables(), r2.getLossVariables())

						'Save via stream:
						Dim f3 As File = Files.createTempFile(testDir,"some-file-3","fb").toFile()
						Using os As Stream = New BufferedOutputStream(New FileStream(f3, FileMode.Create, FileAccess.Write))
							sd.save(os, withUpdaterState)
						End Using

						'Load via stream:
						Using [is] As Stream = New BufferedInputStream(New FileStream(f3, FileMode.Open, FileAccess.Read))
							Dim r3 As SameDiff = SameDiff.load([is], withUpdaterState)
							assertEquals(varsOrig.Count, r3.variables().Count)
							assertEquals(fOrig.Length, r3.ops().Length)
							assertEquals(sd.getLossVariables(), r3.getLossVariables())
						End Using
					Next withUpdaterState
				Next execFirst
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) public void testTrainingSerde(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTrainingSerde(ByVal backend As Nd4jBackend)

			'Ensure 2 things:
			'1. Training config is serialized/deserialized correctly
			'2. Updater state

			For Each u As IUpdater In New IUpdater(){
				New AdaDelta(),
				New AdaGrad(2e-3),
				New Adam(2e-3),
				New AdaMax(2e-3),
				New AMSGrad(2e-3),
				New Nadam(2e-3),
				New Nesterovs(2e-3),
				New NoOp(),
				New RmsProp(2e-3),
				New Sgd(2e-3)
			}

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				log.info("Testing: {}", u.GetType().FullName)

				Dim sd As SameDiff = SameDiff.create()
				Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, -1, 4)
				Dim label As SDVariable = sd.placeHolder("label", DataType.FLOAT, -1, 3)
				Dim w As SDVariable = sd.var("w", Nd4j.rand(DataType.FLOAT, 4, 3))
				Dim b As SDVariable = sd.var("b", Nd4j.rand(DataType.FLOAT, 1, 3))

				Dim mmul As SDVariable = [in].mmul(w).add(b)
				Dim softmax As SDVariable = sd.nn().softmax(mmul, 0)
				'SDVariable loss = sd.loss().logLoss("loss", label, softmax);

				sd.TrainingConfig = TrainingConfig.builder().updater(u).regularization(New L1Regularization(1e-2), New L2Regularization(1e-2), New WeightDecay(1e-2, True)).dataSetFeatureMapping("in").dataSetLabelMapping("label").build()

				Dim inArr As INDArray = Nd4j.rand(DataType.FLOAT, 3, 4)
				Dim labelArr As INDArray = Nd4j.rand(DataType.FLOAT, 3, 3)

				Dim ds As New DataSet(inArr, labelArr)

				For i As Integer = 0 To 9
					sd.fit(ds)
				Next i


				Dim dir As File = testDir.toFile()
				Dim f As New File(dir, "samediff.bin")
				sd.asFlatFile(f)

				Dim sd2 As SameDiff = SameDiff.fromFlatFile(f)
				assertNotNull(sd2.getTrainingConfig())
				assertNotNull(sd2.getUpdaterMap())
				assertTrue(sd2.isInitializedTraining())

				assertEquals(sd.getTrainingConfig(), sd2.getTrainingConfig())
				assertEquals(sd.getTrainingConfig().toJson(), sd2.getTrainingConfig().toJson())
				Dim m1 As IDictionary(Of String, GradientUpdater) = sd.getUpdaterMap()
				Dim m2 As IDictionary(Of String, GradientUpdater) = sd2.getUpdaterMap()
				assertEquals(m1.Keys, m2.Keys)
				For Each s As String In m1.Keys
					Dim g1 As GradientUpdater = m1(s)
					Dim g2 As GradientUpdater = m2(s)
					assertEquals(g1.getState(), g2.getState())
					assertEquals(g1.getConfig(), g2.getConfig())
				Next s


				'Check training post deserialization
				For i As Integer = 0 To 2
					sd.fit(ds)
					sd2.fit(ds)
				Next i

				For Each v As SDVariable In sd.variables()
					If v.PlaceHolder OrElse v.getVariableType() = VariableType.ARRAY Then
						Continue For
					End If

					Dim v2 As SDVariable = sd2.getVariable(v.name())

					Dim a1 As INDArray = v.Arr
					Dim a2 As INDArray = v2.Arr

					assertEquals(a1, a2)
				Next v
			Next u
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void pooling3DSerialization(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub pooling3DSerialization(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim x As SDVariable = sd.placeHolder("x", DataType.FLOAT, 1, 28, 28)
			Dim o As SDVariable = sd.cnn_Conflict.maxPooling3d("pool", x, Pooling3DConfig.builder().build())

			Dim bbSerialized As ByteBuffer = sd.asFlatBuffers(True)

			Dim deserialized As SameDiff
			Try
				deserialized = SameDiff.fromFlatBuffers(bbSerialized)
			Catch e As IOException
				Throw New Exception("IOException deserializing from FlatBuffers", e)
			End Try
			assertEquals(sd.getVariableOutputOp("pool").GetType(), deserialized.getVariableOutputOp("pool").GetType())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void pooling3DSerialization2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub pooling3DSerialization2(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim x As SDVariable = sd.placeHolder("x", DataType.FLOAT, 1, 28, 28)
			Dim o As SDVariable = sd.cnn_Conflict.avgPooling3d("pool", x, Pooling3DConfig.builder().build())

			Dim bbSerialized As ByteBuffer = sd.asFlatBuffers(True)

			Dim deserialized As SameDiff
			Try
				deserialized = SameDiff.fromFlatBuffers(bbSerialized)
			Catch e As IOException
				Throw New Exception("IOException deserializing from FlatBuffers", e)
			End Try
			assertEquals(sd.getVariableOutputOp("pool").GetType(), deserialized.getVariableOutputOp("pool").GetType())
		End Sub
	End Class

End Namespace