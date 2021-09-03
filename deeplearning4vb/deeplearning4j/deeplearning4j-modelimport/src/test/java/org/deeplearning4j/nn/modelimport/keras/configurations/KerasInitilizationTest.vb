Imports System.Collections.Generic
Imports org.deeplearning4j.nn.conf.distribution
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Keras1LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras1LayerConfiguration
Imports Keras2LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras2LayerConfiguration
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports KerasDense = org.deeplearning4j.nn.modelimport.keras.layers.core.KerasDense
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports WeightInitIdentity = org.deeplearning4j.nn.weights.WeightInitIdentity
Imports WeightInitVarScalingNormalFanIn = org.deeplearning4j.nn.weights.WeightInitVarScalingNormalFanIn
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
Namespace org.deeplearning4j.nn.modelimport.keras.configurations

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Keras Initilization Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasInitilizationTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasInitilizationTest
		Inherits BaseDL4JTest

		Private minValue As Double = -0.2

		Private maxValue As Double = 0.2

		Private mean As Double = 0.0

		Private stdDev As Double = 0.2

		Private value As Double = 42.0

		Private gain As Double = 0.2

		Private conf1 As New Keras1LayerConfiguration()

		Private conf2 As New Keras2LayerConfiguration()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Initializers") void testInitializers() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testInitializers()
			Dim keras1 As Integer? = 1
			Dim keras2 As Integer? = 2
			Dim keras1Inits() As String = initializers(conf1)
			Dim keras2Inits() As String = initializers(conf2)
			Dim dl4jInits() As IWeightInit = dl4jInitializers()
			For i As Integer = 0 To dl4jInits.Length - 2
				initilizationDenseLayer(conf1, keras1, keras1Inits(i), dl4jInits(i))
				initilizationDenseLayer(conf2, keras2, keras2Inits(i), dl4jInits(i))
				initilizationDenseLayer(conf2, keras2, keras2Inits(dl4jInits.Length - 1), dl4jInits(dl4jInits.Length - 1))
			Next i
		End Sub

		Private Function initializers(ByVal conf As KerasLayerConfiguration) As String()
			Return New String() { conf.getINIT_GLOROT_NORMAL(), conf.getINIT_GLOROT_UNIFORM_ALIAS(), conf.getINIT_LECUN_NORMAL(), conf.getINIT_LECUN_UNIFORM(), conf.getINIT_RANDOM_UNIFORM(), conf.getINIT_HE_NORMAL(), conf.getINIT_HE_UNIFORM(), conf.getINIT_ONES(), conf.getINIT_ZERO(), conf.getINIT_IDENTITY(), conf.getINIT_NORMAL(), conf.getINIT_ORTHOGONAL(), conf.getINIT_CONSTANT(), conf.getINIT_VARIANCE_SCALING() }
		End Function

		Private Function dl4jInitializers() As IWeightInit()
			Return New IWeightInit() { WeightInit.XAVIER.getWeightInitFunction(), WeightInit.XAVIER_UNIFORM.getWeightInitFunction(), WeightInit.LECUN_NORMAL.getWeightInitFunction(), WeightInit.LECUN_UNIFORM.getWeightInitFunction(), WeightInit.DISTRIBUTION.getWeightInitFunction(New UniformDistribution(minValue, maxValue)), WeightInit.RELU.getWeightInitFunction(), WeightInit.RELU_UNIFORM.getWeightInitFunction(), WeightInit.ONES.getWeightInitFunction(), WeightInit.ZERO.getWeightInitFunction(), New WeightInitIdentity(0.2), WeightInit.DISTRIBUTION.getWeightInitFunction(New NormalDistribution(mean, stdDev)), WeightInit.DISTRIBUTION.getWeightInitFunction(New OrthogonalDistribution(gain)), WeightInit.DISTRIBUTION.getWeightInitFunction(New ConstantDistribution(value)), New WeightInitVarScalingNormalFanIn(0.2) }
		End Function

		Private Function dl4jDistributions() As Distribution()
			Return New Distribution() { Nothing, Nothing, Nothing, Nothing, New UniformDistribution(minValue, maxValue), Nothing, Nothing, Nothing, Nothing, Nothing, New NormalDistribution(mean, stdDev), New OrthogonalDistribution(gain), New ConstantDistribution(value), Nothing }
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void initilizationDenseLayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion, String initializer, org.deeplearning4j.nn.weights.IWeightInit dl4jInitializer) throws Exception
		Private Sub initilizationDenseLayer(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?, ByVal initializer As String, ByVal dl4jInitializer As IWeightInit)
			Dim layerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = conf.getLAYER_CLASS_NAME_DENSE()
			Dim config As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			config(conf.getLAYER_FIELD_ACTIVATION()) = "linear"
			config(conf.getLAYER_FIELD_NAME()) = "init_test"
			Dim scale As Double = 0.2
			If kerasVersion = 1 Then
				config(conf.getLAYER_FIELD_INIT()) = initializer
				config(conf.getLAYER_FIELD_INIT_MEAN()) = mean
				config(conf.getLAYER_FIELD_INIT_STDDEV()) = stdDev
				config(conf.getLAYER_FIELD_INIT_SCALE()) = scale
				config(conf.getLAYER_FIELD_INIT_MINVAL()) = minValue
				config(conf.getLAYER_FIELD_INIT_MAXVAL()) = maxValue
				config(conf.getLAYER_FIELD_INIT_VALUE()) = value
				config(conf.getLAYER_FIELD_INIT_GAIN()) = gain
			Else
				Dim init As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
				init("class_name") = initializer
				Dim innerInit As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
				innerInit(conf.getLAYER_FIELD_INIT_MEAN()) = mean
				innerInit(conf.getLAYER_FIELD_INIT_STDDEV()) = stdDev
				innerInit(conf.getLAYER_FIELD_INIT_SCALE()) = scale
				innerInit(conf.getLAYER_FIELD_INIT_MINVAL()) = minValue
				innerInit(conf.getLAYER_FIELD_INIT_MAXVAL()) = maxValue
				innerInit(conf.getLAYER_FIELD_INIT_VALUE()) = value
				innerInit(conf.getLAYER_FIELD_INIT_GAIN()) = gain
				Dim mode As String = "fan_in"
				innerInit(conf.getLAYER_FIELD_INIT_MODE()) = mode
				Dim distribution As String = "normal"
				innerInit(conf.getLAYER_FIELD_INIT_DISTRIBUTION()) = distribution
				init(conf.getLAYER_FIELD_CONFIG()) = innerInit
				config(conf.getLAYER_FIELD_INIT()) = init
			End If
			config(conf.getLAYER_FIELD_OUTPUT_DIM()) = 1337
			layerConfig(conf.getLAYER_FIELD_CONFIG()) = config
			layerConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasVersion
			Dim layer As DenseLayer = (New KerasDense(layerConfig, False)).DenseLayer
			assertEquals(dl4jInitializer, layer.getWeightInitFn())
		End Sub
	End Class

End Namespace