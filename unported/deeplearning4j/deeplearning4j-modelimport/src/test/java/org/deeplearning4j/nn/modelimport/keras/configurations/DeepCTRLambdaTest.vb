Imports System
Imports System.IO
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports SameDiffLambdaLayer = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLambdaLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports KerasModelImport = org.deeplearning4j.nn.modelimport.keras.KerasModelImport
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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
Namespace org.deeplearning4j.nn.modelimport.keras.configurations

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Deep CTR Lambda Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag public class DeepCTRLambdaTest
	Public Class DeepCTRLambdaTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Tensors Sum") class TensorsSum extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffLambdaLayer
		<Serializable>
		Friend Class TensorsSum
			Inherits SameDiffLambdaLayer

			Private ReadOnly outerInstance As DeepCTRLambdaTest

			Public Sub New(ByVal outerInstance As DeepCTRLambdaTest)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Function defineLayer(ByVal sameDiff As SameDiff, ByVal layerInput As SDVariable) As SDVariable
				Return layerInput.sum("tensors_sum-" & System.Guid.randomUUID().ToString(), False, 1)
			End Function

			Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
				Return inputType
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Tensors Square") class TensorsSquare extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffLambdaLayer
		<Serializable>
		Friend Class TensorsSquare
			Inherits SameDiffLambdaLayer

			Private ReadOnly outerInstance As DeepCTRLambdaTest

			Public Sub New(ByVal outerInstance As DeepCTRLambdaTest)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Function defineLayer(ByVal sameDiff As SameDiff, ByVal layerInput As SDVariable) As SDVariable
				Return layerInput.mul("tensor_square-" & System.Guid.randomUUID().ToString(), layerInput)
			End Function

			Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
				Return inputType
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Lambda 1") class Lambda1 extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffLambdaLayer
		<Serializable>
		Friend Class Lambda1
			Inherits SameDiffLambdaLayer

			Private ReadOnly outerInstance As DeepCTRLambdaTest

			Public Sub New(ByVal outerInstance As DeepCTRLambdaTest)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Function defineLayer(ByVal sameDiff As SameDiff, ByVal layerInput As SDVariable) As SDVariable
				Return layerInput.mul("lambda1-" & System.Guid.randomUUID().ToString(), 0.5)
			End Function

			Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
				Return inputType
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Tensor Mean") class TensorMean extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffLambdaLayer
		<Serializable>
		Friend Class TensorMean
			Inherits SameDiffLambdaLayer

			Private ReadOnly outerInstance As DeepCTRLambdaTest

			Public Sub New(ByVal outerInstance As DeepCTRLambdaTest)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Function defineLayer(ByVal sameDiff As SameDiff, ByVal layerInput As SDVariable) As SDVariable
				If Me.layerName.Equals("concat_embed_2d") OrElse Me.layerName.Equals("cat_embed_2d_genure_mean") Then
					Return layerInput.mean("mean_pooling-" & System.Guid.randomUUID().ToString(), True, 1)
				Else
					Return layerInput.mean("mean_pooling-" & System.Guid.randomUUID().ToString(), False, 1)
				End If
			End Function

			Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
				Return inputType
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Deep Ctr") void testDeepCtr() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testDeepCtr()
			KerasLayer.registerLambdaLayer("sum_of_tensors", New TensorsSum(Me))
			KerasLayer.registerLambdaLayer("square_of_tensors", New TensorsSquare(Me))
			KerasLayer.registerLambdaLayer("lambda_1", New Lambda1(Me))
			KerasLayer.registerLambdaLayer("cat_embed_2d_genure_mean", New TensorMean(Me))
			KerasLayer.registerLambdaLayer("embed_1d_mean", New TensorMean(Me))
			Dim classPathResource As New ClassPathResource("modelimport/keras/examples/deepfm/deepfm.h5")
			Using inputStream As Stream = classPathResource.InputStream, input0 As org.nd4j.linalg.api.ndarray.INDArray = org.nd4j.linalg.factory.Nd4j.createNpyFromInputStream((New org.nd4j.common.io.ClassPathResource("modelimport/keras/examples/deepfm/deepfm_x_0.npy")).InputStream), input1 As org.nd4j.linalg.api.ndarray.INDArray = org.nd4j.linalg.factory.Nd4j.createNpyFromInputStream((New org.nd4j.common.io.ClassPathResource("modelimport/keras/examples/deepfm/deepfm_x_1.npy")).InputStream), input2 As org.nd4j.linalg.api.ndarray.INDArray = org.nd4j.linalg.factory.Nd4j.createNpyFromInputStream((New org.nd4j.common.io.ClassPathResource("modelimport/keras/examples/deepfm/deepfm_x_2.npy")).InputStream), input3 As org.nd4j.linalg.api.ndarray.INDArray = org.nd4j.linalg.factory.Nd4j.createNpyFromInputStream((New org.nd4j.common.io.ClassPathResource("modelimport/keras/examples/deepfm/deepfm_x_3.npy")).InputStream)
				Dim input0Reshaped As INDArray = input0.reshape(ChrW(input0.length()), 1)
				Dim computationGraph As ComputationGraph = KerasModelImport.importKerasModelAndWeights(inputStream)
				computationGraph.output(input0Reshaped, input1, input2, input3)
			End Using
		End Sub
	End Class

End Namespace