Imports System.Collections.Generic
Imports System.Linq
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports PermutePreprocessor = org.deeplearning4j.nn.modelimport.keras.preprocessors.PermutePreprocessor
Imports KerasLayerUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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
	''' Imports Permute layer from Keras
	''' 
	''' @author Max Pumperla
	''' </summary>
	Public Class KerasPermute
		Inherits KerasLayer

		Private permutationIndices() As Integer


		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasPermute(java.util.Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object))
			Me.New(layerConfig, True)
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig">           dictionary containing Keras layer configuration </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training-related configuration options </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasPermute(java.util.Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean)
			MyBase.New(layerConfig, enforceTrainingConfig)
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			Dim permutationInfo As String = "dims"
			If innerConfig.ContainsKey(permutationInfo) Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<Integer> targetShapeList = (java.util.List<Integer>) innerConfig.get(permutationInfo);
				Dim targetShapeList As IList(Of Integer) = DirectCast(innerConfig(permutationInfo), IList(Of Integer))
				Me.permutationIndices = ArrayUtil.toArray(targetShapeList)
			End If

		End Sub

		''' <summary>
		''' KerasPermute is an InputPreProcessor
		''' </summary>
		Public Overrides ReadOnly Property InputPreProcessor As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Gets appropriate DL4J InputPreProcessor for given InputTypes.
		''' </summary>
		''' <param name="inputType"> Array of InputTypes </param>
		''' <returns> DL4J InputPreProcessor </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
		''' <seealso cref= InputPreProcessor </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.InputPreProcessor getInputPreprocessor(org.deeplearning4j.nn.conf.inputs.InputType... inputType) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides Function getInputPreprocessor(ParamArray ByVal inputType() As InputType) As InputPreProcessor
			If inputType.Length > 1 Then
				Throw New InvalidKerasConfigurationException("Keras Permute layer accepts only one input (received " & inputType.Length & ")")
			End If
			Dim preprocessor As InputPreProcessor = Nothing
			If TypeOf inputType(0) Is InputType.InputTypeConvolutional Then
				Select Case Me.DimOrder
					Case THEANO
						preprocessor = New PermutePreprocessor(permutationIndices)
					Case NONE, TENSORFLOW ' TF by default
						' account for channels last
						permutationIndices = New Integer() {permutationIndices(2), permutationIndices(0), permutationIndices(1)}
						preprocessor = New PermutePreprocessor(New Integer(){1, 3, 2})
				End Select
			ElseIf TypeOf inputType(0) Is InputType.InputTypeRecurrent Then
				If permutationIndices.SequenceEqual(New Integer() {2, 1}) Then
					preprocessor = New PermutePreprocessor(permutationIndices)
				Else
					Throw New InvalidKerasConfigurationException("For RNN type input data, permutation dims have to be" & "(2, 1) in Permute layer, got " & Arrays.toString(permutationIndices))
				End If
			ElseIf TypeOf inputType(0) Is InputType.InputTypeFeedForward Then
				preprocessor = Nothing
			Else
				Throw New InvalidKerasConfigurationException("Input type not supported: " & inputType(0))
			End If
			Return preprocessor
		End Function

		''' <summary>
		''' Get layer output type.
		''' </summary>
		''' <param name="inputType"> Array of InputTypes </param>
		''' <returns> output type as InputType </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(org.deeplearning4j.nn.conf.inputs.InputType... inputType) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides Function getOutputType(ParamArray ByVal inputType() As InputType) As InputType
			If inputType.Length > 1 Then
				Throw New InvalidKerasConfigurationException("Keras Permute layer accepts only one input (received " & inputType.Length & ")")
			End If
			Dim reshape As PermutePreprocessor = DirectCast(getInputPreprocessor(inputType), PermutePreprocessor)
			Return reshape.getOutputType(inputType(0))
		End Function
	End Class

End Namespace