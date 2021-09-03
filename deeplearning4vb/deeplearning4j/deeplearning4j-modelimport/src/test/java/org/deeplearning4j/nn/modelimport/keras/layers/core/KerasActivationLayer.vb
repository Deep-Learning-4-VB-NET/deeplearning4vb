Imports System.Collections.Generic
Imports ActivationLayer = org.deeplearning4j.nn.conf.layers.ActivationLayer
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Keras1LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras1LayerConfiguration
Imports Keras2LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras2LayerConfiguration
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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

Namespace org.deeplearning4j.nn.modelimport.keras.layers.core


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag public class KerasActivationLayer extends org.deeplearning4j.BaseDL4JTest
	Public Class KerasActivationLayer
		Inherits BaseDL4JTest

		Private ReadOnly ACTIVATION_KERAS As String = "linear"
		Private ReadOnly ACTIVATION_DL4J As String = "identity"
		Private ReadOnly LAYER_NAME As String = "test_layer"

		Private keras1 As Integer? = 1
		Private keras2 As Integer? = 2
		Private conf1 As New Keras1LayerConfiguration()
		Private conf2 As New Keras2LayerConfiguration()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testActivationLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testActivationLayer()
			buildActivationLayer(conf1, keras1)
			buildActivationLayer(conf2, keras2)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void buildActivationLayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion) throws Exception
		Private Sub buildActivationLayer(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?)
			Dim layerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = conf.getLAYER_FIELD_ACTIVATION()
			Dim config As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			config(conf.getLAYER_FIELD_ACTIVATION()) = ACTIVATION_KERAS
			config(conf.getLAYER_FIELD_NAME()) = LAYER_NAME
			layerConfig(conf.getLAYER_FIELD_CONFIG()) = config
			layerConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasVersion

			Dim layer As ActivationLayer = (New KerasActivation(layerConfig)).ActivationLayer
			assertEquals(ACTIVATION_DL4J, layer.getActivationFn().ToString())
			assertEquals(LAYER_NAME, layer.LayerName)
		End Sub

	End Class

End Namespace