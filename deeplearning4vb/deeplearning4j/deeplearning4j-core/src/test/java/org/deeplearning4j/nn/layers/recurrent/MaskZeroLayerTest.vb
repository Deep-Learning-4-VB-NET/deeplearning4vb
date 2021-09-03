Imports System.Collections.Generic
Imports System.Linq
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports LSTM = org.deeplearning4j.nn.conf.layers.LSTM
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports Arguments = org.junit.jupiter.params.provider.Arguments
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
import static org.junit.jupiter.api.Assertions.assertEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
'ORIGINAL LINE: @DisplayName("Mask Zero Layer Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class MaskZeroLayerTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class MaskZeroLayerTest
		Inherits BaseDL4JTest


		Public Shared Function params() As Stream(Of Arguments)
			Dim args As IList(Of Arguments) = New List(Of Arguments)()
			For Each nd4jBackend As Nd4jBackend In BaseNd4jTestWithBackends.BACKENDS
				For Each rnnFormat As RNNFormat In System.Enum.GetValues(GetType(RNNFormat))
					args.Add(Arguments.of(rnnFormat,nd4jBackend))
				Next rnnFormat
			Next nd4jBackend
			Return args.stream()
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Activate") @ParameterizedTest @MethodSource("org.deeplearning4j.nn.layers.recurrent.MaskZeroLayerTest#params") void activate(org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub activate(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)
			' GIVEN two examples where some of the timesteps are zero.
			Dim ex1 As INDArray = Nd4j.create(New Double()() {
				New Double() { 0, 3, 5 },
				New Double() { 0, 0, 2 }
			})
			Dim ex2 As INDArray = Nd4j.create(New Double()() {
				New Double() { 0, 0, 2 },
				New Double() { 0, 0, 2 }
			})
			' A LSTM which adds one for every non-zero timestep
			Dim underlying As LSTM = (New LSTM.Builder()).activation(Activation.IDENTITY).gateActivationFunction(Activation.IDENTITY).nIn(2).nOut(1).dataFormat(rnnDataFormat).build()
			Dim conf As New NeuralNetConfiguration()
			conf.setLayer(underlying)
			Dim params As INDArray = Nd4j.zeros(New Integer() { 1, 16 })
			' Set the biases to 1.
			For i As Integer = 12 To 15
				params.putScalar(i, 1.0)
			Next i
			Dim lstm As Layer = underlying.instantiate(conf, Enumerable.Empty(Of TrainingListener)(), 0, params, False, params.dataType())
			Dim maskingValue As Double = 0.0
			Dim l As New MaskZeroLayer(lstm, maskingValue)
			Dim input As INDArray = Nd4j.create(Arrays.asList(ex1, ex2), New Integer() { 2, 2, 3 })
			If rnnDataFormat = RNNFormat.NWC Then
				input = input.permute(0, 2, 1)
			End If
			' WHEN
			Dim [out] As INDArray = l.activate(input, True, LayerWorkspaceMgr.noWorkspaces())
			If rnnDataFormat = RNNFormat.NWC Then
				[out] = [out].permute(0, 2, 1)
			End If
			' THEN output should only be incremented for the non-zero timesteps
			Dim firstExampleOutput As INDArray = [out].get(NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.all())
			Dim secondExampleOutput As INDArray = [out].get(NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.all())
			assertEquals(0.0, firstExampleOutput.getDouble(0), 1e-6)
			assertEquals(1.0, firstExampleOutput.getDouble(1), 1e-6)
			assertEquals(2.0, firstExampleOutput.getDouble(2), 1e-6)
			assertEquals(0.0, secondExampleOutput.getDouble(0), 1e-6)
			assertEquals(0.0, secondExampleOutput.getDouble(1), 1e-6)
			assertEquals(1.0, secondExampleOutput.getDouble(2), 1e-6)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Serialization") @ParameterizedTest @MethodSource("org.deeplearning4j.nn.layers.recurrent.MaskZeroLayerTest#params") void testSerialization(org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testSerialization(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New org.deeplearning4j.nn.conf.layers.util.MaskZeroLayer.Builder()).setMaskValue(0.0).setUnderlying((New LSTM.Builder()).nIn(4).nOut(5).dataFormat(rnnDataFormat).build()).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			TestUtils.testModelSerialization(net)
		End Sub
	End Class

End Namespace