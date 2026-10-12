Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ElementWiseVertex = org.deeplearning4j.nn.conf.graph.ElementWiseVertex
Imports MergeVertex = org.deeplearning4j.nn.conf.graph.MergeVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasLayerUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils

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
'ORIGINAL LINE: @Slf4j @Data public class KerasMerge extends org.deeplearning4j.nn.modelimport.keras.KerasLayer
	Public Class KerasMerge
		Inherits KerasLayer

		Private ReadOnly LAYER_FIELD_MODE As String = "mode"
		Private ReadOnly LAYER_MERGE_MODE_SUM As String = "sum"
		Private ReadOnly LAYER_MERGE_MODE_MUL As String = "mul"
		Private ReadOnly LAYER_MERGE_MODE_CONCAT As String = "concat"
		Private ReadOnly LAYER_MERGE_MODE_AVE As String = "ave"
		Private ReadOnly LAYER_MERGE_MODE_COS As String = "cos"
		Private ReadOnly LAYER_MERGE_MODE_DOT As String = "dot"
		Private ReadOnly LAYER_MERGE_MODE_MAX As String = "max"

		Private mergeMode As ElementWiseVertex.Op = Nothing

		''' <summary>
		''' Pass-through constructor from KerasLayer
		''' </summary>
		''' <param name="kerasVersion"> major keras version </param>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasMerge(System.Nullable<Integer> kerasVersion) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal kerasVersion As Integer?)
			MyBase.New(kerasVersion)
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration. </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasMerge(java.util.Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object))
			Me.New(layerConfig, True)
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary and merge mode passed in.
		''' </summary>
		''' <param name="layerConfig">           dictionary containing Keras layer configuration </param>
		''' <param name="mergeMode">             ElementWiseVertex merge mode </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training-related configuration options </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasMerge(java.util.Map<String, Object> layerConfig, org.deeplearning4j.nn.conf.graph.ElementWiseVertex.Op mergeMode, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal mergeMode As ElementWiseVertex.Op, ByVal enforceTrainingConfig As Boolean)
			MyBase.New(layerConfig, enforceTrainingConfig)
			Me.mergeMode = mergeMode

			If Me.mergeMode = Nothing Then
				Me.vertex_Conflict = New MergeVertex()
				Dim mergeVertex As MergeVertex = DirectCast(Me.vertex_Conflict, MergeVertex)
				If hasMergeAxis(layerConfig) Then
					mergeVertex.MergeAxis = getMergeAxisFromConfig(layerConfig)
				End If
			Else
				Me.vertex_Conflict = New ElementWiseVertex(mergeMode)
			End If
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig">           dictionary containing Keras layer configuration </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training-related configuration options </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasMerge(java.util.Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean)
			MyBase.New(layerConfig, enforceTrainingConfig)
			Me.mergeMode = getMergeMode(layerConfig)

			If Me.mergeMode = Nothing Then
				Me.vertex_Conflict = New MergeVertex()
				Dim mergeVertex As MergeVertex = DirectCast(Me.vertex_Conflict, MergeVertex)
				If hasMergeAxis(layerConfig) Then
					mergeVertex.MergeAxis = getMergeAxisFromConfig(layerConfig)
				End If
			Else
				Me.vertex_Conflict = New ElementWiseVertex(mergeMode)
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.deeplearning4j.nn.conf.graph.ElementWiseVertex.Op getMergeMode(java.util.Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Private Function getMergeMode(ByVal layerConfig As IDictionary(Of String, Object)) As ElementWiseVertex.Op
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If Not innerConfig.ContainsKey(LAYER_FIELD_MODE) Then
				Throw New InvalidKerasConfigurationException("Keras Merge layer config missing " & LAYER_FIELD_MODE & " field")
			End If
			Dim op As ElementWiseVertex.Op = Nothing
			Dim mergeMode As String = DirectCast(innerConfig(LAYER_FIELD_MODE), String)
			Select Case mergeMode
				Case LAYER_MERGE_MODE_SUM
					op = ElementWiseVertex.Op.Add
				Case LAYER_MERGE_MODE_MUL
					op = ElementWiseVertex.Op.Product
				Case LAYER_MERGE_MODE_CONCAT
					' leave null
				Case LAYER_MERGE_MODE_AVE
					op = ElementWiseVertex.Op.Average
				Case LAYER_MERGE_MODE_MAX
					op = ElementWiseVertex.Op.Max
				Case Else
					Throw New UnsupportedKerasConfigurationException("Keras Merge layer mode " & mergeMode & " not supported")
			End Select
			Return op
		End Function

		''' <summary>
		''' Get layer output type.
		''' </summary>
		''' <param name="inputType"> Array of InputTypes </param>
		''' <returns> output type as InputType </returns>
		Public Overrides Function getOutputType(ParamArray ByVal inputType() As InputType) As InputType
			Return Me.vertex_Conflict.getOutputType(-1, inputType)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private boolean hasMergeAxis(java.util.Map<String,Object> config) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Private Function hasMergeAxis(ByVal config As IDictionary(Of String, Object)) As Boolean
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(config, conf)
			Return innerConfig.ContainsKey(conf.getLAYER_FIELD_CONSTRAINT_DIM())
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private System.Nullable<Integer> getMergeAxisFromConfig(java.util.Map<String,Object> config) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Private Function getMergeAxisFromConfig(ByVal config As IDictionary(Of String, Object)) As Integer?
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(config, conf)
			If innerConfig.ContainsKey(conf.getLAYER_FIELD_CONSTRAINT_DIM()) Then
				Dim [dim] As Integer? = DirectCast(innerConfig(conf.getLAYER_FIELD_CONSTRAINT_DIM()), Integer?)
				Return [dim]
			End If

			Return Nothing
		End Function

	End Class

End Namespace