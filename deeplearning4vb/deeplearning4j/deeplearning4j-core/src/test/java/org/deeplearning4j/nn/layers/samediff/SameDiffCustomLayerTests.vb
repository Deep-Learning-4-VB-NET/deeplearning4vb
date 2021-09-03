Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports GraphVertex = org.deeplearning4j.nn.conf.graph.GraphVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InvalidInputTypeException = org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports SDLayerParams = org.deeplearning4j.nn.conf.layers.samediff.SDLayerParams
Imports SDVertexParams = org.deeplearning4j.nn.conf.layers.samediff.SDVertexParams
Imports SameDiffVertex = org.deeplearning4j.nn.conf.layers.samediff.SameDiffVertex
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
import static org.junit.jupiter.api.Assertions.assertThrows

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

Namespace org.deeplearning4j.nn.layers.samediff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.SAMEDIFF) @Tag(TagNames.CUSTOM_FUNCTIONALITY) public class SameDiffCustomLayerTests extends org.deeplearning4j.BaseDL4JTest
	Public Class SameDiffCustomLayerTests
		Inherits BaseDL4JTest

		Private initialType As DataType


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before()
		Public Overridable Sub before()
			Nd4j.create(1)
			initialType = Nd4j.dataType()

			Nd4j.DataType = DataType.DOUBLE
			Nd4j.Random.setSeed(123)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after()
		Public Overridable Sub after()
			Nd4j.DataType = initialType

			NativeOpsHolder.Instance.getDeviceNativeOps().enableDebugMode(False)
			NativeOpsHolder.Instance.getDeviceNativeOps().enableVerboseMode(False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInputValidationSameDiffLayer()
		Public Overridable Sub testInputValidationSameDiffLayer()
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim config As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(New ValidatingSameDiffLayer(Me)).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.SIGMOID).nOut(2).build()).setInputType(InputType.feedForward(2)).build()
			Dim net As New MultiLayerNetwork(config)
			net.init()
			Dim goodInput As INDArray = Nd4j.rand(1, 2)
			Dim badInput As INDArray = Nd4j.rand(2, 2)
			net.fit(goodInput, goodInput)
			net.fit(badInput, badInput)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInputValidationSameDiffVertex()
		Public Overridable Sub testInputValidationSameDiffVertex()
		   assertThrows(GetType(System.ArgumentException),Sub()
		   Dim config As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addVertex("a", New ValidatingSameDiffVertex(Me), "input").addLayer("output", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.SIGMOID).nOut(2).build(), "a").addInputs("input").setInputTypes(InputType.feedForward(2)).setOutputs("output").build()
		   Dim net As New ComputationGraph(config)
		   net.init()
		   Dim goodInput As INDArray = Nd4j.rand(1, 2)
		   Dim badInput As INDArray = Nd4j.rand(2, 2)
		   net.fit(New INDArray(){goodInput}, New INDArray(){goodInput})
		   net.fit(New INDArray(){badInput}, New INDArray(){badInput})
		   End Sub)

		End Sub

		<Serializable>
		Private Class ValidatingSameDiffLayer
			Inherits org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer

			Private ReadOnly outerInstance As SameDiffCustomLayerTests

			Public Sub New(ByVal outerInstance As SameDiffCustomLayerTests)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Sub validateInput(ByVal input As INDArray)
				Preconditions.checkArgument(input.size(0) < 2, "Expected Message")
			End Sub

			Public Overrides Function defineLayer(ByVal sameDiff As SameDiff, ByVal layerInput As SDVariable, ByVal paramTable As IDictionary(Of String, SDVariable), ByVal mask As SDVariable) As SDVariable
				Return layerInput
			End Function

			Public Overrides Sub defineParameters(ByVal params As SDLayerParams)
			End Sub
			Public Overrides Sub initializeParameters(ByVal params As IDictionary(Of String, INDArray))
			End Sub
			Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
				Return inputType
			End Function
		End Class

		<Serializable>
		Private Class ValidatingSameDiffVertex
			Inherits SameDiffVertex

			Private ReadOnly outerInstance As SameDiffCustomLayerTests

			Public Sub New(ByVal outerInstance As SameDiffCustomLayerTests)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Function clone() As GraphVertex
				Return New ValidatingSameDiffVertex(outerInstance)
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(int layerIndex, org.deeplearning4j.nn.conf.inputs.InputType... vertexInputs) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
			Public Overrides Function getOutputType(ByVal layerIndex As Integer, ParamArray ByVal vertexInputs() As InputType) As InputType
				Return vertexInputs(0)
			End Function

			Public Overrides Sub validateInput(ByVal input() As INDArray)
				Preconditions.checkArgument(input(0).size(0) < 2, "Expected Message")
			End Sub

			Public Overrides Function defineVertex(ByVal sameDiff As SameDiff, ByVal layerInput As IDictionary(Of String, SDVariable), ByVal paramTable As IDictionary(Of String, SDVariable), ByVal maskVars As IDictionary(Of String, SDVariable)) As SDVariable
				Return layerInput("input")
			End Function

			Public Overrides Sub defineParametersAndInputs(ByVal params As SDVertexParams)
				params.defineInputs("input")
			End Sub

			Public Overrides Sub initializeParameters(ByVal params As IDictionary(Of String, INDArray))
			End Sub
		End Class
	End Class

End Namespace