Imports System.Collections
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports MaxNormConstraint = org.deeplearning4j.nn.conf.constraint.MaxNormConstraint
Imports MinMaxNormConstraint = org.deeplearning4j.nn.conf.constraint.MinMaxNormConstraint
Imports NonNegativeConstraint = org.deeplearning4j.nn.conf.constraint.NonNegativeConstraint
Imports UnitNormConstraint = org.deeplearning4j.nn.conf.constraint.UnitNormConstraint
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException

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
'ORIGINAL LINE: @Slf4j public class KerasConstraintUtils
	Public Class KerasConstraintUtils

		''' <summary>
		''' Map Keras to DL4J constraint.
		''' </summary>
		''' <param name="kerasConstraint"> String containing Keras constraint name </param>
		''' <param name="conf">            Keras layer configuration </param>
		''' <returns> DL4J LayerConstraint </returns>
		''' <seealso cref= LayerConstraint </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.api.layers.LayerConstraint mapConstraint(String kerasConstraint, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, java.util.Map<String, Object> constraintConfig) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function mapConstraint(ByVal kerasConstraint As String, ByVal conf As KerasLayerConfiguration, ByVal constraintConfig As IDictionary(Of String, Object)) As LayerConstraint
			Dim constraint As LayerConstraint
			If kerasConstraint.Equals(conf.getLAYER_FIELD_MINMAX_NORM_CONSTRAINT()) OrElse kerasConstraint.Equals(conf.getLAYER_FIELD_MINMAX_NORM_CONSTRAINT_ALIAS()) Then
				Dim min As Double = DirectCast(constraintConfig(conf.getLAYER_FIELD_MINMAX_MIN_CONSTRAINT()), Double)
				Dim max As Double = DirectCast(constraintConfig(conf.getLAYER_FIELD_MINMAX_MAX_CONSTRAINT()), Double)
				Dim rate As Double = DirectCast(constraintConfig(conf.getLAYER_FIELD_CONSTRAINT_RATE()), Double)
				Dim [dim] As Integer = DirectCast(constraintConfig(conf.getLAYER_FIELD_CONSTRAINT_DIM()), Integer)
				constraint = New MinMaxNormConstraint(min, max, rate, [dim] + 1)
			ElseIf kerasConstraint.Equals(conf.getLAYER_FIELD_MAX_NORM_CONSTRAINT()) OrElse kerasConstraint.Equals(conf.getLAYER_FIELD_MAX_NORM_CONSTRAINT_ALIAS()) OrElse kerasConstraint.Equals(conf.getLAYER_FIELD_MAX_NORM_CONSTRAINT_ALIAS_2()) Then
				Dim max As Double = DirectCast(constraintConfig(conf.getLAYER_FIELD_MAX_CONSTRAINT()), Double)
				Dim [dim] As Integer = DirectCast(constraintConfig(conf.getLAYER_FIELD_CONSTRAINT_DIM()), Integer)
				constraint = New MaxNormConstraint(max, [dim] + 1)
			ElseIf kerasConstraint.Equals(conf.getLAYER_FIELD_UNIT_NORM_CONSTRAINT()) OrElse kerasConstraint.Equals(conf.getLAYER_FIELD_UNIT_NORM_CONSTRAINT_ALIAS()) OrElse kerasConstraint.Equals(conf.getLAYER_FIELD_UNIT_NORM_CONSTRAINT_ALIAS_2()) Then
				Dim [dim] As Integer = DirectCast(constraintConfig(conf.getLAYER_FIELD_CONSTRAINT_DIM()), Integer)
				constraint = New UnitNormConstraint([dim] + 1)
			ElseIf kerasConstraint.Equals(conf.getLAYER_FIELD_NON_NEG_CONSTRAINT()) OrElse kerasConstraint.Equals(conf.getLAYER_FIELD_NON_NEG_CONSTRAINT_ALIAS()) OrElse kerasConstraint.Equals(conf.getLAYER_FIELD_NON_NEG_CONSTRAINT_ALIAS_2()) Then
				constraint = New NonNegativeConstraint()
			Else
				Throw New UnsupportedKerasConfigurationException("Unknown keras constraint " & kerasConstraint)
			End If

			Return constraint
		End Function

		''' <summary>
		''' Get constraint initialization from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig">       dictionary containing Keras layer configuration </param>
		''' <param name="constraintField">   string in configuration representing parameter to constrain </param>
		''' <param name="conf">              Keras layer configuration </param>
		''' <param name="kerasMajorVersion"> Major keras version as integer (1 or 2) </param>
		''' <returns> a valid LayerConstraint </returns>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid configuration </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.api.layers.LayerConstraint getConstraintsFromConfig(java.util.Map<String, Object> layerConfig, String constraintField, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, int kerasMajorVersion) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function getConstraintsFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal constraintField As String, ByVal conf As KerasLayerConfiguration, ByVal kerasMajorVersion As Integer) As LayerConstraint
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If Not innerConfig.ContainsKey(constraintField) Then
				' log.warn("Keras layer is missing " + constraintField + " field");
				Return Nothing
			End If
			Dim constraintMap As Hashtable = DirectCast(innerConfig(constraintField), Hashtable)
			If constraintMap Is Nothing Then
				Return Nothing
			End If

			Dim kerasConstraint As String
			If constraintMap.ContainsKey(conf.getLAYER_FIELD_CONSTRAINT_NAME()) Then
				kerasConstraint = CStr(constraintMap(conf.getLAYER_FIELD_CONSTRAINT_NAME()))
			Else
				Throw New InvalidKerasConfigurationException("Keras layer is missing " & conf.getLAYER_FIELD_CONSTRAINT_NAME() & " field")
			End If

			Dim constraintConfig As IDictionary(Of String, Object)
			If kerasMajorVersion = 2 Then
				constraintConfig = KerasLayerUtils.getInnerLayerConfigFromConfig(constraintMap, conf)
			Else
				constraintConfig = constraintMap
			End If
			Dim layerConstraint As LayerConstraint = mapConstraint(kerasConstraint, conf, constraintConfig)

			Return layerConstraint
		End Function
	End Class

End Namespace