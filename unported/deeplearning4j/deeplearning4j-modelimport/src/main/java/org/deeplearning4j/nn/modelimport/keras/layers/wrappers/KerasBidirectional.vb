Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InputTypeUtil = org.deeplearning4j.nn.conf.layers.InputTypeUtil
Imports LSTM = org.deeplearning4j.nn.conf.layers.LSTM
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports Bidirectional = org.deeplearning4j.nn.conf.layers.recurrent.Bidirectional
Imports LastTimeStep = org.deeplearning4j.nn.conf.layers.recurrent.LastTimeStep
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasLSTM = org.deeplearning4j.nn.modelimport.keras.layers.recurrent.KerasLSTM
Imports KerasSimpleRnn = org.deeplearning4j.nn.modelimport.keras.layers.recurrent.KerasSimpleRnn
Imports KerasLayerUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.nn.modelimport.keras.layers.wrappers


	Public Class KerasBidirectional
		Inherits KerasLayer

		Private kerasRnnlayer As KerasLayer

		''' <summary>
		''' Pass-through constructor from KerasLayer
		''' </summary>
		''' <param name="kerasVersion"> major keras version </param>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasBidirectional(System.Nullable<Integer> kerasVersion) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal kerasVersion As Integer?)
			MyBase.New(kerasVersion)
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasBidirectional(java.util.Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object))
			Me.New(layerConfig, True, Enumerable.Empty(Of String, KerasLayer)())
		End Sub


		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasBidirectional(java.util.Map<String, Object> layerConfig, java.util.Map<String, ? extends org.deeplearning4j.nn.modelimport.keras.KerasLayer> previousLayers) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal previousLayers As IDictionary(Of String, KerasLayer))
			Me.New(layerConfig, True, previousLayers)
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig">           dictionary containing Keras layer configuration </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training-related configuration options </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasBidirectional(java.util.Map<String, Object> layerConfig, boolean enforceTrainingConfig, java.util.Map<String, ? extends org.deeplearning4j.nn.modelimport.keras.KerasLayer> previousLayers) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean, ByVal previousLayers As IDictionary(Of String, KerasLayer))
			MyBase.New(layerConfig, enforceTrainingConfig)

			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If Not innerConfig.ContainsKey("merge_mode") Then
				Throw New InvalidKerasConfigurationException("Field 'merge_mode' not found in configuration of " & "Bidirectional layer.")
			End If
			If Not innerConfig.ContainsKey("layer") Then
				Throw New InvalidKerasConfigurationException("Field 'layer' not found in configuration of" & "Bidirectional layer, i.e. no layer to be wrapped found.")
			End If
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Map<String, Object> innerRnnConfig = (java.util.Map<String, Object>) innerConfig.get("layer");
			Dim innerRnnConfig As IDictionary(Of String, Object) = DirectCast(innerConfig("layer"), IDictionary(Of String, Object))
			If Not innerRnnConfig.ContainsKey("class_name") Then
				Throw New InvalidKerasConfigurationException("No 'class_name' specified within Bidirectional layer" & "configuration.")
			End If

			Dim mode As Bidirectional.Mode
			Dim mergeModeString As String = DirectCast(innerConfig("merge_mode"), String)
			Select Case mergeModeString
				Case "sum"
					mode = Bidirectional.Mode.ADD
				Case "concat"
					mode = Bidirectional.Mode.CONCAT
				Case "mul"
					mode = Bidirectional.Mode.MUL
				Case "ave"
					mode = Bidirectional.Mode.AVERAGE
				Case Else
					' Note that this is only for "None" mode, which we currently can't do.
					Throw New UnsupportedKerasConfigurationException("Merge mode " & mergeModeString & " not supported.")
			End Select

			innerRnnConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasMajorVersion

			Dim rnnClass As String = DirectCast(innerRnnConfig("class_name"), String)
			Select Case rnnClass
				Case "LSTM"
					kerasRnnlayer = New KerasLSTM(innerRnnConfig, enforceTrainingConfig, previousLayers)
					Try
						Dim rnnLayer As LSTM = DirectCast((DirectCast(kerasRnnlayer, KerasLSTM)).LSTMLayer, LSTM)
						layer_Conflict = New Bidirectional(mode, rnnLayer)
						layer_Conflict.setLayerName(layerName_Conflict)
					Catch e As Exception
						Dim rnnLayer As LastTimeStep = DirectCast((DirectCast(kerasRnnlayer, KerasLSTM)).LSTMLayer, LastTimeStep)
						Me.layer_Conflict = New Bidirectional(mode, rnnLayer)
						layer_Conflict.setLayerName(layerName_Conflict)
					End Try
				Case "SimpleRNN"
					kerasRnnlayer = New KerasSimpleRnn(innerRnnConfig, enforceTrainingConfig, previousLayers)
					Dim rnnLayer As Layer = DirectCast(kerasRnnlayer, KerasSimpleRnn).SimpleRnnLayer
					Me.layer_Conflict = New Bidirectional(mode, rnnLayer)
					layer_Conflict.setLayerName(layerName_Conflict)
				Case Else
					Throw New UnsupportedKerasConfigurationException("Currently only two types of recurrent Keras layers are" & "supported, 'LSTM' and 'SimpleRNN'. You tried to load a layer of class:" & rnnClass)
			End Select

		End Sub

		''' <summary>
		''' Return the underlying recurrent layer of this bidirectional layer
		''' </summary>
		''' <returns> Layer, recurrent layer </returns>
		Public Overridable ReadOnly Property UnderlyingRecurrentLayer As Layer
			Get
				Return kerasRnnlayer.Layer
			End Get
		End Property

		''' <summary>
		''' Get DL4J Bidirectional layer.
		''' </summary>
		''' <returns> Bidirectional Layer </returns>
		Public Overridable ReadOnly Property BidirectionalLayer As Bidirectional
			Get
				Return DirectCast(Me.layer_Conflict, Bidirectional)
			End Get
		End Property

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
				Throw New InvalidKerasConfigurationException("Keras Bidirectional layer accepts only one input (received " & inputType.Length & ")")
			End If
			Dim preProcessor As InputPreProcessor = getInputPreprocessor(inputType)
			If preProcessor IsNot Nothing Then
				Return Me.BidirectionalLayer.getOutputType(-1, preProcessor.getOutputType(inputType(0)))
			Else
				Return Me.BidirectionalLayer.getOutputType(-1, inputType(0))
			End If
		End Function

		''' <summary>
		''' Returns number of trainable parameters in layer.
		''' </summary>
		''' <returns> number of trainable parameters </returns>
		Public Overrides ReadOnly Property NumParams As Integer
			Get
				Return 2 * kerasRnnlayer.NumParams
			End Get
		End Property

		''' <summary>
		''' Gets appropriate DL4J InputPreProcessor for given InputTypes.
		''' </summary>
		''' <param name="inputType"> Array of InputTypes </param>
		''' <returns> DL4J InputPreProcessor </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras configuration exception </exception>
		''' <seealso cref= org.deeplearning4j.nn.conf.InputPreProcessor </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.InputPreProcessor getInputPreprocessor(org.deeplearning4j.nn.conf.inputs.InputType... inputType) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides Function getInputPreprocessor(ParamArray ByVal inputType() As InputType) As InputPreProcessor
			If inputType.Length > 1 Then
				Throw New InvalidKerasConfigurationException("Keras Bidirectional layer accepts only one input (received " & inputType.Length & ")")
			End If
			Return InputTypeUtil.getPreprocessorForInputTypeRnnLayers(inputType(0), DirectCast(layer_Conflict, Bidirectional).RNNDataFormat, layerName_Conflict)
		End Function

		''' <summary>
		''' Set weights for Bidirectional layer.
		''' </summary>
		''' <param name="weights"> Map of weights </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void setWeights(java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> weights) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides WriteOnly Property Weights As IDictionary(Of String, INDArray)
			Set(ByVal weights As IDictionary(Of String, INDArray))
    
				Dim forwardWeights As IDictionary(Of String, INDArray) = getUnderlyingWeights(DirectCast(Me.layer_Conflict, Bidirectional).getFwd(), weights, "forward")
				Dim backwardWeights As IDictionary(Of String, INDArray) = getUnderlyingWeights(DirectCast(Me.layer_Conflict, Bidirectional).getBwd(), weights, "backward")
    
				Me.weights_Conflict = New Dictionary(Of String, INDArray)()
    
				For Each key As String In forwardWeights.Keys
					Me.weights_Conflict("f" & key) = forwardWeights(key)
				Next key
				For Each key As String In backwardWeights.Keys
					Me.weights_Conflict("b" & key) = backwardWeights(key)
				Next key
			End Set
		End Property


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> getUnderlyingWeights(org.deeplearning4j.nn.conf.layers.Layer l, java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> weights, String direction) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Private Function getUnderlyingWeights(ByVal l As Layer, ByVal weights As IDictionary(Of String, INDArray), ByVal direction As String) As IDictionary(Of String, INDArray)
			Dim keras1SubstringLength As Integer
			If TypeOf kerasRnnlayer Is KerasLSTM Then
				keras1SubstringLength = 3
			ElseIf TypeOf kerasRnnlayer Is KerasSimpleRnn Then
				keras1SubstringLength = 1
			Else
				Throw New InvalidKerasConfigurationException("Unsupported layer type " & kerasRnnlayer.ClassName)
			End If

			Dim newWeights As System.Collections.IDictionary = New Dictionary(Of String, INDArray)()
			For Each key As String In weights.Keys
				If key.Contains(direction) Then
					Dim newKey As String
					If kerasMajorVersion = 2 Then
						Dim subKeys() As String = key.Split("_", True)
						If key.Contains("recurrent") Then
							newKey = subKeys(subKeys.Length - 2) & "_" & subKeys(subKeys.Length - 1)
						Else
							newKey = subKeys(subKeys.Length - 1)
						End If
					Else
						newKey = key.Substring(key.Length - keras1SubstringLength)
					End If
					newWeights(newKey) = weights(key)
				End If
			Next key
			If newWeights.Count > 0 Then
				weights = newWeights
			End If

			Dim layerBefore As Layer = kerasRnnlayer.Layer
			kerasRnnlayer.setLayer(l)
			kerasRnnlayer.Weights = weights
			Dim ret As IDictionary(Of String, INDArray) = kerasRnnlayer.getWeights()
			kerasRnnlayer.setLayer(layerBefore)
			Return ret
		End Function

	End Class

End Namespace