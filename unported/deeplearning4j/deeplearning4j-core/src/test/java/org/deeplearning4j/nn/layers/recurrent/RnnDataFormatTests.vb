Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports GravesBidirectionalLSTM = org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM
Imports GravesLSTM = org.deeplearning4j.nn.conf.layers.GravesLSTM
Imports LSTM = org.deeplearning4j.nn.conf.layers.LSTM
Imports RnnOutputLayer = org.deeplearning4j.nn.conf.layers.RnnOutputLayer
Imports LastTimeStep = org.deeplearning4j.nn.conf.layers.recurrent.LastTimeStep
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
Imports MaskZeroLayer = org.deeplearning4j.nn.conf.layers.util.MaskZeroLayer
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports Arguments = org.junit.jupiter.params.provider.Arguments
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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

Namespace org.deeplearning4j.nn.layers.recurrent


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class RnnDataFormatTests extends org.deeplearning4j.BaseDL4JTest
	Public Class RnnDataFormatTests
		Inherits BaseDL4JTest


		Public Shared Function params() As Stream(Of Arguments)
			Dim ret As IList(Of Object()) = New List(Of Object())()
			For Each helpers As Boolean In New Boolean(){True, False}
				For Each lastTimeStep As Boolean In New Boolean(){True, False}
					For Each maskZero As Boolean In New Boolean(){True, False}
						For Each backend As Nd4jBackend In BaseNd4jTestWithBackends.BACKENDS
							ret.Add(New Object(){helpers, lastTimeStep, maskZero, backend})
						Next backend
					Next maskZero
				Next lastTimeStep
			Next helpers
			Return ret.Select(AddressOf Arguments.of)
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.deeplearning4j.nn.layers.recurrent.RnnDataFormatTests#params") @ParameterizedTest public void testSimpleRnn(boolean helpers, boolean lastTimeStep, boolean maskZeros, org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSimpleRnn(ByVal helpers As Boolean, ByVal lastTimeStep As Boolean, ByVal maskZeros As Boolean, ByVal backend As Nd4jBackend)
			Try

				Nd4j.Random.setSeed(12345)
				Nd4j.Environment.allowHelpers(helpers)
				Dim msg As String = "Helpers: " & helpers & ", lastTimeStep: " & lastTimeStep & ", maskZeros: " & maskZeros
				Console.WriteLine(" --- " & msg & " ---")

				Dim inNCW As INDArray = Nd4j.rand(DataType.FLOAT, 2, 3, 12)

				Dim labelsNWC As INDArray = If(lastTimeStep, TestUtils.randomOneHot(2, 10), TestUtils.randomOneHot(2 * 12, 10).reshape(ChrW(2), 12, 10))

				Dim tc As TestCase = TestCase.builder().msg(msg).net1(getSimpleRnnNet(RNNFormat.NCW, True, lastTimeStep, maskZeros)).net2(getSimpleRnnNet(RNNFormat.NCW, False, lastTimeStep, maskZeros)).net3(getSimpleRnnNet(RNNFormat.NWC, True, lastTimeStep, maskZeros)).net4(getSimpleRnnNet(RNNFormat.NWC, False, lastTimeStep, maskZeros)).inNCW(inNCW).labelsNCW(If(lastTimeStep, labelsNWC, labelsNWC.permute(0, 2, 1))).labelsNWC(labelsNWC).testLayerIdx(1).build()

				TestCase.testHelper(tc)


			Finally
				Nd4j.Environment.allowHelpers(True)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.deeplearning4j.nn.layers.recurrent.RnnDataFormatTests#params") public void testLSTM(boolean helpers, boolean lastTimeStep, boolean maskZeros,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLSTM(ByVal helpers As Boolean, ByVal lastTimeStep As Boolean, ByVal maskZeros As Boolean, ByVal backend As Nd4jBackend)
			Try

				Nd4j.Random.setSeed(12345)
				Nd4j.Environment.allowHelpers(helpers)
				Dim msg As String = "Helpers: " & helpers & ", lastTimeStep: " & lastTimeStep & ", maskZeros: " & maskZeros
				Console.WriteLine(" --- " & msg & " ---")

				Dim inNCW As INDArray = Nd4j.rand(DataType.FLOAT, 2, 3, 12)

				Dim labelsNWC As INDArray = If(lastTimeStep, TestUtils.randomOneHot(2, 10), TestUtils.randomOneHot(2 * 12, 10).reshape(ChrW(2), 12, 10))

				Dim tc As TestCase = TestCase.builder().msg(msg).net1(getLstmNet(RNNFormat.NCW, True, lastTimeStep, maskZeros)).net2(getLstmNet(RNNFormat.NCW, False, lastTimeStep, maskZeros)).net3(getLstmNet(RNNFormat.NWC, True, lastTimeStep, maskZeros)).net4(getLstmNet(RNNFormat.NWC, False, lastTimeStep, maskZeros)).inNCW(inNCW).labelsNCW(If(lastTimeStep, labelsNWC, labelsNWC.permute(0, 2, 1))).labelsNWC(labelsNWC).testLayerIdx(1).build()

				TestCase.testHelper(tc)


			Finally
				Nd4j.Environment.allowHelpers(True)
			End Try
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.deeplearning4j.nn.layers.recurrent.RnnDataFormatTests#params") @ParameterizedTest @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) public void testGraveLSTM(boolean helpers, boolean lastTimeStep, boolean maskZeros,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGraveLSTM(ByVal helpers As Boolean, ByVal lastTimeStep As Boolean, ByVal maskZeros As Boolean, ByVal backend As Nd4jBackend)
			Try

				Nd4j.Random.setSeed(12345)
				Nd4j.Environment.allowHelpers(helpers)
				Dim msg As String = "Helpers: " & helpers & ", lastTimeStep: " & lastTimeStep & ", maskZeros: " & maskZeros
				Console.WriteLine(" --- " & msg & " ---")

				Dim inNCW As INDArray = Nd4j.rand(DataType.FLOAT, 2, 3, 12)

				Dim labelsNWC As INDArray = If(lastTimeStep, TestUtils.randomOneHot(2, 10), TestUtils.randomOneHot(2 * 12, 10).reshape(ChrW(2), 12, 10))

				Dim tc As TestCase = TestCase.builder().msg(msg).net1(getGravesLstmNet(RNNFormat.NCW, True, lastTimeStep, maskZeros)).net2(getGravesLstmNet(RNNFormat.NCW, False, lastTimeStep, maskZeros)).net3(getGravesLstmNet(RNNFormat.NWC, True, lastTimeStep, maskZeros)).net4(getGravesLstmNet(RNNFormat.NWC, False, lastTimeStep, maskZeros)).inNCW(inNCW).labelsNCW(If(lastTimeStep, labelsNWC, labelsNWC.permute(0, 2, 1))).labelsNWC(labelsNWC).testLayerIdx(1).build()

				TestCase.testHelper(tc)


			Finally
				Nd4j.Environment.allowHelpers(True)
			End Try
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.deeplearning4j.nn.layers.recurrent.RnnDataFormatTests#params") @ParameterizedTest public void testGraveBiLSTM(boolean helpers, boolean lastTimeStep, boolean maskZeros,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGraveBiLSTM(ByVal helpers As Boolean, ByVal lastTimeStep As Boolean, ByVal maskZeros As Boolean, ByVal backend As Nd4jBackend)
			Try

				Nd4j.Random.setSeed(12345)
				Nd4j.Environment.allowHelpers(helpers)
				Dim msg As String = "Helpers: " & helpers & ", lastTimeStep: " & lastTimeStep & ", maskZeros: " & maskZeros
				Console.WriteLine(" --- " & msg & " ---")

				Dim inNCW As INDArray = Nd4j.rand(DataType.FLOAT, 2, 3, 12)

				Dim labelsNWC As INDArray = If(lastTimeStep, TestUtils.randomOneHot(2, 10), TestUtils.randomOneHot(2 * 12, 10).reshape(ChrW(2), 12, 10))

				Dim tc As TestCase = TestCase.builder().msg(msg).net1(getGravesBidirectionalLstmNet(RNNFormat.NCW, True, lastTimeStep, maskZeros)).net2(getGravesBidirectionalLstmNet(RNNFormat.NCW, False, lastTimeStep, maskZeros)).net3(getGravesBidirectionalLstmNet(RNNFormat.NWC, True, lastTimeStep, maskZeros)).net4(getGravesBidirectionalLstmNet(RNNFormat.NWC, False, lastTimeStep, maskZeros)).inNCW(inNCW).labelsNCW(If(lastTimeStep, labelsNWC, labelsNWC.permute(0, 2, 1))).labelsNWC(labelsNWC).testLayerIdx(1).build()

				TestCase.testHelper(tc)


			Finally
				Nd4j.Environment.allowHelpers(True)
			End Try
		End Sub


		Private Function getGravesBidirectionalLstmNet(ByVal format As RNNFormat, ByVal setOnLayerAlso As Boolean, ByVal lastTimeStep As Boolean, ByVal maskZeros As Boolean) As MultiLayerNetwork
			If setOnLayerAlso Then
				Return getNetWithLayer((New GravesBidirectionalLSTM.Builder()).nOut(3).dataFormat(format).build(), format, lastTimeStep, maskZeros)
			Else
				Return getNetWithLayer((New GravesBidirectionalLSTM.Builder()).nOut(3).build(), format, lastTimeStep, maskZeros)
			End If
		End Function
		Private Function getGravesLstmNet(ByVal format As RNNFormat, ByVal setOnLayerAlso As Boolean, ByVal lastTimeStep As Boolean, ByVal maskZeros As Boolean) As MultiLayerNetwork
			If setOnLayerAlso Then
				Return getNetWithLayer((New GravesLSTM.Builder()).nOut(3).dataFormat(format).build(), format, lastTimeStep, maskZeros)
			Else
				Return getNetWithLayer((New GravesLSTM.Builder()).nOut(3).build(), format, lastTimeStep, maskZeros)
			End If
		End Function

		Private Function getLstmNet(ByVal format As RNNFormat, ByVal setOnLayerAlso As Boolean, ByVal lastTimeStep As Boolean, ByVal maskZeros As Boolean) As MultiLayerNetwork
			If setOnLayerAlso Then
				Return getNetWithLayer((New LSTM.Builder()).nOut(3).dataFormat(format).build(), format, lastTimeStep, maskZeros)
			Else
				Return getNetWithLayer((New LSTM.Builder()).nOut(3).build(), format, lastTimeStep, maskZeros)
			End If
		End Function

		Private Function getSimpleRnnNet(ByVal format As RNNFormat, ByVal setOnLayerAlso As Boolean, ByVal lastTimeStep As Boolean, ByVal maskZeros As Boolean) As MultiLayerNetwork
			If setOnLayerAlso Then
				Return getNetWithLayer((New SimpleRnn.Builder()).nOut(3).dataFormat(format).build(), format, lastTimeStep, maskZeros)
			Else
				Return getNetWithLayer((New SimpleRnn.Builder()).nOut(3).build(), format, lastTimeStep, maskZeros)
			End If
		End Function
		Private Function getNetWithLayer(ByVal layer As Layer, ByVal format As RNNFormat, ByVal lastTimeStep As Boolean, ByVal maskZeros As Boolean) As MultiLayerNetwork
			If maskZeros Then
				layer = (New MaskZeroLayer.Builder()).setMaskValue(0.0).setUnderlying(layer).build()
			End If
			If lastTimeStep Then
				layer = New LastTimeStep(layer)
			End If
			Dim builder As NeuralNetConfiguration.ListBuilder = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer((New LSTM.Builder()).nIn(3).activation(Activation.TANH).dataFormat(format).nOut(3).helperAllowFallback(False).build()).layer(layer).layer(If(lastTimeStep, (New OutputLayer.Builder()).activation(Activation.SOFTMAX).nOut(10).build(), (New RnnOutputLayer.Builder()).activation(Activation.SOFTMAX).nOut(10).dataFormat(format).build())).setInputType(InputType.recurrent(3, 12, format))

			Dim net As New MultiLayerNetwork(builder.build())
			net.init()
			Return net
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data @NoArgsConstructor @Builder private static class TestCase
		Private Class TestCase
			Friend msg As String
			Friend net1 As MultiLayerNetwork
			Friend net2 As MultiLayerNetwork
			Friend net3 As MultiLayerNetwork
			Friend net4 As MultiLayerNetwork
			Friend inNCW As INDArray
			Friend labelsNCW As INDArray
			Friend labelsNWC As INDArray
			Friend testLayerIdx As Integer
			Friend nwcOutput As Boolean

			Public Shared Sub testHelper(ByVal tc As TestCase)

				tc.net2.params().assign(tc.net1.params())
				tc.net3.params().assign(tc.net1.params())
				tc.net4.params().assign(tc.net1.params())

				Dim inNCW As INDArray = tc.inNCW
				Dim inNWC As INDArray = tc.inNCW.permute(0, 2, 1).dup()

				Dim l0_1 As INDArray = tc.net1.feedForward(inNCW)(tc.testLayerIdx + 1)
				Dim l0_2 As INDArray = tc.net2.feedForward(inNCW)(tc.testLayerIdx + 1)
				Dim l0_3 As INDArray = tc.net3.feedForward(inNWC)(tc.testLayerIdx + 1)
				Dim l0_4 As INDArray = tc.net4.feedForward(inNWC)(tc.testLayerIdx + 1)

				Dim rank3Out As Boolean = tc.labelsNCW.rank() = 3
				assertEquals(l0_1, l0_2, tc.msg)
				If rank3Out Then
					assertEquals(l0_1, l0_3.permute(0, 2, 1), tc.msg)
					assertEquals(l0_1, l0_4.permute(0, 2, 1), tc.msg)
				Else
					assertEquals(l0_1, l0_3, tc.msg)
					assertEquals(l0_1, l0_4, tc.msg)
				End If
				Dim out1 As INDArray = tc.net1.output(inNCW)
				Dim out2 As INDArray = tc.net2.output(inNCW)
				Dim out3 As INDArray = tc.net3.output(inNWC)
				Dim out4 As INDArray = tc.net4.output(inNWC)

				assertEquals(out1, out2, tc.msg)
				If rank3Out Then
					assertEquals(out1, out3.permute(0, 2, 1), tc.msg) 'NWC to NCW
					assertEquals(out1, out4.permute(0, 2, 1), tc.msg)
				Else
					assertEquals(out1, out3, tc.msg) 'NWC to NCW
					assertEquals(out1, out4, tc.msg)
				End If


				'Test backprop
				Dim p1 As Pair(Of Gradient, INDArray) = tc.net1.calculateGradients(inNCW, tc.labelsNCW, Nothing, Nothing)
				Dim p2 As Pair(Of Gradient, INDArray) = tc.net2.calculateGradients(inNCW, tc.labelsNCW, Nothing, Nothing)
				Dim p3 As Pair(Of Gradient, INDArray) = tc.net3.calculateGradients(inNWC, tc.labelsNWC, Nothing, Nothing)
				Dim p4 As Pair(Of Gradient, INDArray) = tc.net4.calculateGradients(inNWC, tc.labelsNWC, Nothing, Nothing)

				'Inpput gradients
				assertEquals(p1.Second, p2.Second, tc.msg)

				assertEquals(p1.Second, p3.Second.permute(0, 2, 1), tc.msg) 'Input gradients for NWC input are also in NWC format
				assertEquals(p1.Second, p4.Second.permute(0, 2, 1), tc.msg)


				Dim diff12 As IList(Of String) = differentGrads(p1.First, p2.First)
				Dim diff13 As IList(Of String) = differentGrads(p1.First, p3.First)
				Dim diff14 As IList(Of String) = differentGrads(p1.First, p4.First)
				assertEquals(0, diff12.Count,tc.msg & " " & diff12)
				assertEquals(0, diff13.Count,tc.msg & " " & diff13)
				assertEquals(0, diff14.Count,tc.msg & " " & diff14)

				assertEquals(p1.First.gradientForVariable(), p2.First.gradientForVariable(), tc.msg)
				assertEquals(p1.First.gradientForVariable(), p3.First.gradientForVariable(), tc.msg)
				assertEquals(p1.First.gradientForVariable(), p4.First.gradientForVariable(), tc.msg)

				tc.net1.fit(inNCW, tc.labelsNCW)
				tc.net2.fit(inNCW, tc.labelsNCW)
				tc.net3.fit(inNWC, tc.labelsNWC)
				tc.net4.fit(inNWC, tc.labelsNWC)

				assertEquals(tc.net1.params(), tc.net2.params(), tc.msg)
				assertEquals(tc.net1.params(), tc.net3.params(), tc.msg)
				assertEquals(tc.net1.params(), tc.net4.params(), tc.msg)

				'Test serialization
				Dim net1a As MultiLayerNetwork = TestUtils.testModelSerialization(tc.net1)
				Dim net2a As MultiLayerNetwork = TestUtils.testModelSerialization(tc.net2)
				Dim net3a As MultiLayerNetwork = TestUtils.testModelSerialization(tc.net3)
				Dim net4a As MultiLayerNetwork = TestUtils.testModelSerialization(tc.net4)

				out1 = tc.net1.output(inNCW)
				assertEquals(out1, net1a.output(inNCW), tc.msg)
				assertEquals(out1, net2a.output(inNCW), tc.msg)

				If rank3Out Then
					assertEquals(out1, net3a.output(inNWC).permute(0, 2, 1), tc.msg) 'NWC to NCW
					assertEquals(out1, net4a.output(inNWC).permute(0, 2, 1), tc.msg)
				Else
					assertEquals(out1, net3a.output(inNWC), tc.msg) 'NWC to NCW
					assertEquals(out1, net4a.output(inNWC), tc.msg)
				End If
			End Sub

		End Class
		Private Shared Function differentGrads(ByVal g1 As Gradient, ByVal g2 As Gradient) As IList(Of String)
			Dim differs As IList(Of String) = New List(Of String)()
			Dim m1 As IDictionary(Of String, INDArray) = g1.gradientForVariable()
			Dim m2 As IDictionary(Of String, INDArray) = g2.gradientForVariable()
			For Each s As String In m1.Keys
				Dim a1 As INDArray = m1(s)
				Dim a2 As INDArray = m2(s)
				If Not a1.Equals(a2) Then
					differs.Add(s)
				End If
			Next s
			Return differs
		End Function
	End Class

End Namespace