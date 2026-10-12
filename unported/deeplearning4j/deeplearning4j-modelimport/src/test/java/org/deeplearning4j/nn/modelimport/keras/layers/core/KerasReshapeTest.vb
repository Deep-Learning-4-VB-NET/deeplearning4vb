Imports System.Collections.Generic
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Keras1LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras1LayerConfiguration
Imports Keras2LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras2LayerConfiguration
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports ReshapePreprocessor = org.deeplearning4j.nn.modelimport.keras.preprocessors.ReshapePreprocessor
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Assertions = org.junit.jupiter.api.Assertions
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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
Namespace org.deeplearning4j.nn.modelimport.keras.layers.core

	''' <summary>
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Keras Reshape Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasReshapeTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasReshapeTest
		Inherits BaseDL4JTest

		Private keras1 As Integer? = 1

		Private keras2 As Integer? = 2

		Private conf1 As New Keras1LayerConfiguration()

		Private conf2 As New Keras2LayerConfiguration()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Reshape Layer") void testReshapeLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testReshapeLayer()
			buildReshapeLayer(conf1, keras1)
			buildReshapeLayer(conf2, keras2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Reshape Dynamic Minibatch") void testReshapeDynamicMinibatch() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testReshapeDynamicMinibatch()
			testDynamicMinibatches(conf1, keras1)
			testDynamicMinibatches(conf2, keras2)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void buildReshapeLayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion) throws Exception
		Private Sub buildReshapeLayer(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?)
			Dim targetShape() As Integer = { 10, 5 }
			Dim targetShapeList As IList(Of Integer) = New List(Of Integer)()
			targetShapeList.Add(targetShape(0))
			targetShapeList.Add(targetShape(1))
			Dim preProcessor As ReshapePreprocessor = getReshapePreProcessor(conf, kerasVersion, targetShapeList)
			assertEquals(preProcessor.getTargetShape()(0), targetShape(0))
			assertEquals(preProcessor.getTargetShape()(1), targetShape(1))
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.deeplearning4j.nn.modelimport.keras.preprocessors.ReshapePreprocessor getReshapePreProcessor(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion, List<Integer> targetShapeList) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Private Function getReshapePreProcessor(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?, ByVal targetShapeList As IList(Of Integer)) As ReshapePreprocessor
			Dim layerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = conf.getLAYER_CLASS_NAME_RESHAPE()
			Dim config As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			Dim LAYER_FIELD_TARGET_SHAPE As String = "target_shape"
			config(LAYER_FIELD_TARGET_SHAPE) = targetShapeList
			Dim layerName As String = "reshape"
			config(conf.getLAYER_FIELD_NAME()) = layerName
			layerConfig(conf.getLAYER_FIELD_CONFIG()) = config
			layerConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasVersion
			Dim inputType As InputType = InputType.InputTypeFeedForward.feedForward(20)
			Return DirectCast((New KerasReshape(layerConfig)).getInputPreprocessor(inputType), ReshapePreprocessor)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void testDynamicMinibatches(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Private Sub testDynamicMinibatches(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?)
			Dim targetShape As IList(Of Integer) = Collections.singletonList(20)
			Dim preprocessor As ReshapePreprocessor = getReshapePreProcessor(conf, kerasVersion, targetShape)
			Dim r1 As INDArray = preprocessor.preProcess(Nd4j.zeros(10, 20), 10, LayerWorkspaceMgr.noWorkspaces())
			Dim r2 As INDArray = preprocessor.preProcess(Nd4j.zeros(5, 20), 5, LayerWorkspaceMgr.noWorkspaces())
			Assertions.assertArrayEquals(r2.shape(), New Long() { 5, 20 })
			Assertions.assertArrayEquals(r1.shape(), New Long() { 10, 20 })
		End Sub
	End Class

End Namespace