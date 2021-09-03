﻿Imports System.Collections.Generic
Imports ActivationLayer = org.deeplearning4j.nn.conf.layers.ActivationLayer
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Keras1LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras1LayerConfiguration
Imports Keras2LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras2LayerConfiguration
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports KerasThresholdedReLU = org.deeplearning4j.nn.modelimport.keras.layers.advanced.activations.KerasThresholdedReLU
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
import static org.junit.jupiter.api.Assertions.assertEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames

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
Namespace org.deeplearning4j.nn.modelimport.keras.layers.advanced.activation

	''' <summary>
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Keras Thresholded Re LU Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasThresholdedReLUTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasThresholdedReLUTest
		Inherits BaseDL4JTest

		Private conf1 As New Keras1LayerConfiguration()

		Private conf2 As New Keras2LayerConfiguration()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Thresholded Re LU Layer") void testThresholdedReLULayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testThresholdedReLULayer()
			Dim keras1 As Integer? = 1
			buildThresholdedReLULayer(conf1, keras1)
			Dim keras2 As Integer? = 2
			buildThresholdedReLULayer(conf2, keras2)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void buildThresholdedReLULayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion) throws Exception
		Private Sub buildThresholdedReLULayer(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?)
			Dim theta As Double = 0.5
			Dim layerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = conf.getLAYER_CLASS_NAME_THRESHOLDED_RELU()
			Dim config As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			Dim LAYER_FIELD_THRESHOLD As String = "theta"
			config(LAYER_FIELD_THRESHOLD) = theta
			Dim layerName As String = "thresholded_relu"
			config(conf.getLAYER_FIELD_NAME()) = layerName
			layerConfig(conf.getLAYER_FIELD_CONFIG()) = config
			layerConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasVersion
			Dim layer As ActivationLayer = (New KerasThresholdedReLU(layerConfig)).ActivationLayer
			assertEquals(layer.getActivationFn().ToString(), "thresholdedrelu(theta=0.5)")
			assertEquals(layerName, layer.LayerName)
		End Sub
	End Class

End Namespace