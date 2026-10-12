Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.deeplearning4j.nn.conf.distribution
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports org.deeplearning4j.nn.weights

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

Namespace org.deeplearning4j.nn.modelimport.keras.utils


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class KerasInitilizationUtils
	Public Class KerasInitilizationUtils

		''' <summary>
		''' Map Keras to DL4J weight initialization functions.
		''' </summary>
		''' <param name="kerasInit"> String containing Keras initialization function name </param>
		''' <returns> DL4J weight initialization enum </returns>
		''' <seealso cref= WeightInit </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static IWeightInit mapWeightInitialization(String kerasInit, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, java.util.Map<String, Object> initConfig, int kerasMajorVersion) throws UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function mapWeightInitialization(ByVal kerasInit As String, ByVal conf As KerasLayerConfiguration, ByVal initConfig As IDictionary(Of String, Object), ByVal kerasMajorVersion As Integer) As IWeightInit


			' TODO: Identity and VarianceScaling need "scale" factor
			If kerasInit IsNot Nothing Then
				If kerasInit.Equals(conf.getINIT_GLOROT_NORMAL()) OrElse kerasInit.Equals(conf.getINIT_GLOROT_NORMAL_ALIAS()) Then
					Return WeightInit.XAVIER.getWeightInitFunction()
				ElseIf kerasInit.Equals(conf.getINIT_GLOROT_UNIFORM()) OrElse kerasInit.Equals(conf.getINIT_GLOROT_UNIFORM_ALIAS()) Then
					Return WeightInit.XAVIER_UNIFORM.getWeightInitFunction()
				ElseIf kerasInit.Equals(conf.getINIT_LECUN_NORMAL()) OrElse kerasInit.Equals(conf.getINIT_LECUN_NORMAL_ALIAS()) Then
					Return WeightInit.LECUN_NORMAL.getWeightInitFunction()
				ElseIf kerasInit.Equals(conf.getINIT_LECUN_UNIFORM()) OrElse kerasInit.Equals(conf.getINIT_LECUN_UNIFORM_ALIAS()) Then
					Return WeightInit.LECUN_UNIFORM.getWeightInitFunction()
				ElseIf kerasInit.Equals(conf.getINIT_HE_NORMAL()) OrElse kerasInit.Equals(conf.getINIT_HE_NORMAL_ALIAS()) Then
					Return WeightInit.RELU.getWeightInitFunction()
				ElseIf kerasInit.Equals(conf.getINIT_HE_UNIFORM()) OrElse kerasInit.Equals(conf.getINIT_HE_UNIFORM_ALIAS()) Then
					Return WeightInit.RELU_UNIFORM.getWeightInitFunction()
				ElseIf kerasInit.Equals(conf.getINIT_ONE()) OrElse kerasInit.Equals(conf.getINIT_ONES()) OrElse kerasInit.Equals(conf.getINIT_ONES_ALIAS()) Then
					Return WeightInit.ONES.getWeightInitFunction()
				ElseIf kerasInit.Equals(conf.getINIT_ZERO()) OrElse kerasInit.Equals(conf.getINIT_ZEROS()) OrElse kerasInit.Equals(conf.getINIT_ZEROS_ALIAS()) Then
					Return WeightInit.ZERO.getWeightInitFunction()
				ElseIf kerasInit.Equals(conf.getINIT_UNIFORM()) OrElse kerasInit.Equals(conf.getINIT_RANDOM_UNIFORM()) OrElse kerasInit.Equals(conf.getINIT_RANDOM_UNIFORM_ALIAS()) Then
					If kerasMajorVersion = 2 Then
						Dim minVal As Double = DirectCast(initConfig(conf.getLAYER_FIELD_INIT_MINVAL()), Double)
						Dim maxVal As Double = DirectCast(initConfig(conf.getLAYER_FIELD_INIT_MAXVAL()), Double)
						Return New WeightInitDistribution(New UniformDistribution(minVal, maxVal))
					Else
						Dim scale As Double = 0.05
						If initConfig.ContainsKey(conf.getLAYER_FIELD_INIT_SCALE()) Then
							scale = DirectCast(initConfig(conf.getLAYER_FIELD_INIT_SCALE()), Double)
						End If
						Return New WeightInitDistribution(New UniformDistribution(-scale, scale))
					End If
				ElseIf kerasInit.Equals(conf.getINIT_NORMAL()) OrElse kerasInit.Equals(conf.getINIT_RANDOM_NORMAL()) OrElse kerasInit.Equals(conf.getINIT_RANDOM_NORMAL_ALIAS()) Then
					If kerasMajorVersion = 2 Then
						Dim mean As Double = DirectCast(initConfig(conf.getLAYER_FIELD_INIT_MEAN()), Double)
						Dim stdDev As Double = DirectCast(initConfig(conf.getLAYER_FIELD_INIT_STDDEV()), Double)
						Return New WeightInitDistribution(New NormalDistribution(mean, stdDev))
					Else
						Dim scale As Double = 0.05
						If initConfig.ContainsKey(conf.getLAYER_FIELD_INIT_SCALE()) Then
							scale = DirectCast(initConfig(conf.getLAYER_FIELD_INIT_SCALE()), Double)
						End If
						Return New WeightInitDistribution(New NormalDistribution(0, scale))
					End If
				ElseIf kerasInit.Equals(conf.getINIT_CONSTANT()) OrElse kerasInit.Equals(conf.getINIT_CONSTANT_ALIAS()) Then
					Dim value As Double = DirectCast(initConfig(conf.getLAYER_FIELD_INIT_VALUE()), Double)
					Return New WeightInitDistribution(New ConstantDistribution(value))
				ElseIf kerasInit.Equals(conf.getINIT_ORTHOGONAL()) OrElse kerasInit.Equals(conf.getINIT_ORTHOGONAL_ALIAS()) Then
					If kerasMajorVersion = 2 Then
						Dim gain As Double
						Try
							gain = DirectCast(initConfig(conf.getLAYER_FIELD_INIT_GAIN()), Double)
						Catch e As Exception
							gain = DirectCast(initConfig(conf.getLAYER_FIELD_INIT_GAIN()), Integer)
						End Try
						Return New WeightInitDistribution(New OrthogonalDistribution(gain))
					Else
						Dim scale As Double = 1.1
						If initConfig.ContainsKey(conf.getLAYER_FIELD_INIT_SCALE()) Then
							scale = DirectCast(initConfig(conf.getLAYER_FIELD_INIT_SCALE()), Double)
						End If
						Return New WeightInitDistribution(New OrthogonalDistribution(scale))
					End If
				ElseIf kerasInit.Equals(conf.getINIT_TRUNCATED_NORMAL()) OrElse kerasInit.Equals(conf.getINIT_TRUNCATED_NORMAL_ALIAS()) Then
					Dim mean As Double = DirectCast(initConfig(conf.getLAYER_FIELD_INIT_MEAN()), Double)
					Dim stdDev As Double = DirectCast(initConfig(conf.getLAYER_FIELD_INIT_STDDEV()), Double)
					Return New WeightInitDistribution(New TruncatedNormalDistribution(mean, stdDev))
				ElseIf kerasInit.Equals(conf.getINIT_IDENTITY()) OrElse kerasInit.Equals(conf.getINIT_IDENTITY_ALIAS()) Then
					If kerasMajorVersion = 2 Then
						Dim gain As Double = DirectCast(initConfig(conf.getLAYER_FIELD_INIT_GAIN()), Double)
						If gain <> 1.0 Then
						If gain <> 1.0 Then
							Return New WeightInitIdentity(gain)
						Else
							Return New WeightInitIdentity()
						End If
						End If
					Else
						Dim scale As Double = 1.0
						If initConfig.ContainsKey(conf.getLAYER_FIELD_INIT_SCALE()) Then
							scale = DirectCast(initConfig(conf.getLAYER_FIELD_INIT_SCALE()), Double)
						End If
						If scale <> 1.0 Then
							Return New WeightInitIdentity(scale)
						Else
							Return New WeightInitIdentity()
						End If
					End If
				ElseIf kerasInit.Equals(conf.getINIT_VARIANCE_SCALING()) Then
					Dim scale As Double
					Try
						scale = DirectCast(initConfig(conf.getLAYER_FIELD_INIT_SCALE()), Double)
					Catch e As Exception
						scale = DirectCast(initConfig(conf.getLAYER_FIELD_INIT_SCALE()), Integer)
					End Try
					Dim mode As String = DirectCast(initConfig(conf.getLAYER_FIELD_INIT_MODE()), String)
					Dim distribution As String = DirectCast(initConfig(conf.getLAYER_FIELD_INIT_DISTRIBUTION()), String)
					Select Case mode
						Case "fan_in"
							If distribution.Equals("normal") Then
								Return New WeightInitVarScalingNormalFanIn(scale)
							Else
								Return New WeightInitVarScalingUniformFanIn(scale)
							End If
						Case "fan_out"
							If distribution.Equals("normal") Then
								Return New WeightInitVarScalingNormalFanOut(scale)
							Else
								Return New WeightInitVarScalingUniformFanOut(scale)
							End If
						Case "fan_avg"
							If distribution.Equals("normal") Then
								Return New WeightInitVarScalingNormalFanAvg(scale)
							Else
								Return New WeightInitVarScalingUniformFanAvg(scale)
							End If
						Case Else
							Throw New InvalidKerasConfigurationException("Initialization argument 'mode' has to be either " & "fan_in, fan_out or fan_avg")
					End Select
				Else
					Throw New UnsupportedKerasConfigurationException("Unknown keras weight initializer " & kerasInit)
				End If
			End If
			Throw New System.InvalidOperationException("Error getting Keras weight initialization")
		End Function

		''' <summary>
		''' Get weight initialization from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig">           dictionary containing Keras layer configuration </param>
		''' <param name="enforceTrainingConfig"> whether to enforce loading configuration for further training </param>
		''' <returns> Pair of DL4J weight initialization and distribution </returns>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static IWeightInit getWeightInitFromConfig(java.util.Map<String, Object> layerConfig, String initField, boolean enforceTrainingConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, int kerasMajorVersion) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function getWeightInitFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal initField As String, ByVal enforceTrainingConfig As Boolean, ByVal conf As KerasLayerConfiguration, ByVal kerasMajorVersion As Integer) As IWeightInit
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If Not innerConfig.ContainsKey(initField) Then
				Throw New InvalidKerasConfigurationException("Keras layer is missing " & initField & " field")
			End If
			Dim kerasInit As String
			Dim initMap As IDictionary(Of String, Object)
			If kerasMajorVersion <> 2 Then
				kerasInit = DirectCast(innerConfig(initField), String)
				initMap = innerConfig
			Else
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Map<String, Object> fullInitMap = (java.util.HashMap) innerConfig.get(initField);
				Dim fullInitMap As IDictionary(Of String, Object) = DirectCast(innerConfig(initField), Hashtable)
				initMap = DirectCast(fullInitMap("config"), Hashtable)
				If fullInitMap.ContainsKey("class_name") Then
					kerasInit = DirectCast(fullInitMap("class_name"), String)
				Else
					Throw New UnsupportedKerasConfigurationException("Incomplete initialization class")
				End If
			End If
			Dim init As IWeightInit
			Try
				init = mapWeightInitialization(kerasInit, conf, initMap, kerasMajorVersion)
			Catch e As UnsupportedKerasConfigurationException
				If enforceTrainingConfig Then
					Throw e
				Else
					init = New WeightInitXavier()
					log.warn("Unknown weight initializer " & kerasInit & " (Using XAVIER instead).")
				End If
			End Try
			Return init
		End Function

	End Class

End Namespace