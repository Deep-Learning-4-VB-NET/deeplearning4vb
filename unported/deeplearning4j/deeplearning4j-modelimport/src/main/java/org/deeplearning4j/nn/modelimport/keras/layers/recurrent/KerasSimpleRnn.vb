Imports System.Collections.Generic
Imports System.Linq
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports FeedForwardLayer = org.deeplearning4j.nn.conf.layers.FeedForwardLayer
Imports InputTypeUtil = org.deeplearning4j.nn.conf.layers.InputTypeUtil
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports LastTimeStep = org.deeplearning4j.nn.conf.layers.recurrent.LastTimeStep
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
Imports MaskZeroLayer = org.deeplearning4j.nn.conf.layers.util.MaskZeroLayer
Imports BaseWrapperLayer = org.deeplearning4j.nn.conf.layers.wrapper.BaseWrapperLayer
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasConstraintUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasConstraintUtils
Imports KerasLayerUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils
Imports SimpleRnnParamInitializer = org.deeplearning4j.nn.params.SimpleRnnParamInitializer
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports TimeSeriesUtils = org.deeplearning4j.util.TimeSeriesUtils
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasActivationUtils.getIActivationFromConfig
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasInitilizationUtils.getWeightInitFromConfig
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils.getNOutFromConfig

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

Namespace org.deeplearning4j.nn.modelimport.keras.layers.recurrent


	''' <summary>
	''' Imports a Keras SimpleRNN layer as a DL4J SimpleRnn layer.
	''' 
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Data @EqualsAndHashCode(callSuper = false) public class KerasSimpleRnn extends org.deeplearning4j.nn.modelimport.keras.KerasLayer
	Public Class KerasSimpleRnn
		Inherits KerasLayer

		Private ReadOnly NUM_TRAINABLE_PARAMS As Integer = 3
'JAVA TO VB CONVERTER NOTE: The field unroll was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend unroll_Conflict As Boolean = False
		Protected Friend returnSequences As Boolean

		''' <summary>
		''' Pass-through constructor from KerasLayer
		''' </summary>
		''' <param name="kerasVersion"> major keras version </param>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasSimpleRnn(System.Nullable<Integer> kerasVersion) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
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
'ORIGINAL LINE: public KerasSimpleRnn(java.util.Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object))
			Me.New(layerConfig, True, Enumerable.Empty(Of String, KerasLayer)())
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration. </param>
		''' <param name="previousLayers"> dictionary containing the previous layers in the topology </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasSimpleRnn(java.util.Map<String, Object> layerConfig, java.util.Map<String, ? extends org.deeplearning4j.nn.modelimport.keras.KerasLayer> previousLayers) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal previousLayers As IDictionary(Of String, KerasLayer))
			Me.New(layerConfig, True, previousLayers)
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig">           dictionary containing Keras layer configuration. </param>
		''' <param name="enforceTrainingConfig"> whether to load Keras training configuration </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasSimpleRnn(java.util.Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean)
			Me.New(layerConfig, enforceTrainingConfig, Enumerable.Empty(Of String, KerasLayer)())
		End Sub


		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig">           dictionary containing Keras layer configuration </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training-related configuration options </param>
		''' <param name="previousLayers"> dictionary containing the previous layers in the topology </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasSimpleRnn(java.util.Map<String, Object> layerConfig, boolean enforceTrainingConfig, java.util.Map<String, ? extends org.deeplearning4j.nn.modelimport.keras.KerasLayer> previousLayers) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean, ByVal previousLayers As IDictionary(Of String, KerasLayer))
			MyBase.New(layerConfig, enforceTrainingConfig)

			Dim init As IWeightInit = getWeightInitFromConfig(layerConfig, conf.getLAYER_FIELD_INIT(), enforceTrainingConfig, conf, kerasMajorVersion)

			Dim recurrentInit As IWeightInit = getWeightInitFromConfig(layerConfig, conf.getLAYER_FIELD_INNER_INIT(), enforceTrainingConfig, conf, kerasMajorVersion)

			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			Me.returnSequences = DirectCast(innerConfig(conf.getLAYER_FIELD_RETURN_SEQUENCES()), Boolean?)

			KerasRnnUtils.getRecurrentDropout(conf, layerConfig)
			Me.unroll_Conflict = KerasRnnUtils.getUnrollRecurrentLayer(conf, layerConfig)

			Dim maskingConfig As Pair(Of Boolean, Double) = KerasLayerUtils.getMaskingConfiguration(inboundLayerNames_Conflict, previousLayers)

			Dim biasConstraint As LayerConstraint = KerasConstraintUtils.getConstraintsFromConfig(layerConfig, conf.getLAYER_FIELD_B_CONSTRAINT(), conf, kerasMajorVersion)
			Dim weightConstraint As LayerConstraint = KerasConstraintUtils.getConstraintsFromConfig(layerConfig, conf.getLAYER_FIELD_W_CONSTRAINT(), conf, kerasMajorVersion)
			Dim recurrentConstraint As LayerConstraint = KerasConstraintUtils.getConstraintsFromConfig(layerConfig, conf.getLAYER_FIELD_RECURRENT_CONSTRAINT(), conf, kerasMajorVersion)

			Dim builder As SimpleRnn.Builder = (New SimpleRnn.Builder()).name(Me.layerName_Conflict).nOut(getNOutFromConfig(layerConfig, conf)).dropOut(Me.dropout).activation(getIActivationFromConfig(layerConfig, conf)).weightInit(init).weightInitRecurrent(recurrentInit).biasInit(0.0).l1(Me.weightL1Regularization).l2(Me.weightL2Regularization).dataFormat(RNNFormat.NWC)
			Dim nIn As Integer? = KerasLayerUtils.getNInFromInputDim(layerConfig, conf)
			If nIn IsNot Nothing Then
				builder.setNIn(nIn)
			End If
			If biasConstraint IsNot Nothing Then
				builder.constrainBias(biasConstraint)
			End If
			If weightConstraint IsNot Nothing Then
				builder.constrainInputWeights(weightConstraint)
			End If
			If recurrentConstraint IsNot Nothing Then
				builder.constrainRecurrent(recurrentConstraint)
			End If

			Me.layer_Conflict = builder.build()
			If Not returnSequences Then
				Me.layer_Conflict = New LastTimeStep(Me.layer_Conflict)
			End If
			If maskingConfig.First Then
				Me.layer_Conflict = New MaskZeroLayer(Me.layer_Conflict, maskingConfig.Second)
			End If
		End Sub

		''' <summary>
		''' Get DL4J SimpleRnn layer.
		''' </summary>
		''' <returns> SimpleRnn Layer </returns>
		Public Overridable ReadOnly Property SimpleRnnLayer As Layer
			Get
				Return Me.layer_Conflict
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
				Throw New InvalidKerasConfigurationException("Keras SimpleRnn layer accepts only one input (received " & inputType.Length & ")")
			End If
			Dim preProcessor As InputPreProcessor = getInputPreprocessor(inputType)
			If preProcessor IsNot Nothing Then
				Return preProcessor.getOutputType(inputType(0))
			Else
				Return Me.SimpleRnnLayer.getOutputType(-1, inputType(0))
			End If
		End Function

		''' <summary>
		''' Returns number of trainable parameters in layer.
		''' </summary>
		''' <returns> number of trainable parameters (12) </returns>
		Public Overrides ReadOnly Property NumParams As Integer
			Get
				Return NUM_TRAINABLE_PARAMS
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
				Throw New InvalidKerasConfigurationException("Keras SimpleRnn layer accepts only one input (received " & inputType.Length & ")")
			End If

			Dim f As RNNFormat = TimeSeriesUtils.getFormatFromRnnLayer(layer_Conflict)
			Return InputTypeUtil.getPreprocessorForInputTypeRnnLayers(inputType(0), f, layerName_Conflict)
		End Function

		''' <summary>
		''' Get whether SimpleRnn layer should be unrolled (for truncated BPTT).
		''' </summary>
		''' <returns> whether RNN should be unrolled (boolean) </returns>
		Public Overridable ReadOnly Property Unroll As Boolean
			Get
				Return Me.unroll_Conflict
			End Get
		End Property


		''' <summary>
		''' Set weights for layer.
		''' </summary>
		''' <param name="weights"> Simple RNN weights </param>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras configuration exception </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void setWeights(java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> weights) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides WriteOnly Property Weights As IDictionary(Of String, INDArray)
			Set(ByVal weights As IDictionary(Of String, INDArray))
				Me.weights_Conflict = New Dictionary(Of String, INDArray)()
    
    
				Dim W As INDArray
				If weights.ContainsKey(conf.getKERAS_PARAM_NAME_W()) Then
					W = weights(conf.getKERAS_PARAM_NAME_W())
				Else
					Throw New InvalidKerasConfigurationException("Keras SimpleRNN layer does not contain parameter " & conf.getKERAS_PARAM_NAME_W())
				End If
				Me.weights_Conflict(SimpleRnnParamInitializer.WEIGHT_KEY) = W
    
    
				Dim RW As INDArray
				If weights.ContainsKey(conf.getKERAS_PARAM_NAME_RW()) Then
					RW = weights(conf.getKERAS_PARAM_NAME_RW())
				Else
					Throw New InvalidKerasConfigurationException("Keras SimpleRNN layer does not contain parameter " & conf.getKERAS_PARAM_NAME_RW())
				End If
				Me.weights_Conflict(SimpleRnnParamInitializer.RECURRENT_WEIGHT_KEY) = RW
    
    
				Dim b As INDArray
				If weights.ContainsKey(conf.getKERAS_PARAM_NAME_B()) Then
					b = weights(conf.getKERAS_PARAM_NAME_B())
				Else
					Throw New InvalidKerasConfigurationException("Keras SimpleRNN layer does not contain parameter " & conf.getKERAS_PARAM_NAME_B())
				End If
				Me.weights_Conflict(SimpleRnnParamInitializer.BIAS_KEY) = b
    
    
				If weights.Count > NUM_TRAINABLE_PARAMS Then
					Dim paramNames As ISet(Of String) = weights.Keys
					paramNames.remove(conf.getKERAS_PARAM_NAME_B())
					paramNames.remove(conf.getKERAS_PARAM_NAME_W())
					paramNames.remove(conf.getKERAS_PARAM_NAME_RW())
					Dim unknownParamNames As String = paramNames.ToString()
					log.warn("Attemping to set weights for unknown parameters: " & unknownParamNames.Substring(1, (unknownParamNames.Length - 1) - 1))
				End If
    
				Dim ffl As FeedForwardLayer
				If TypeOf Me.layer_Conflict Is BaseWrapperLayer Then
					Dim bwl As BaseWrapperLayer = DirectCast(Me.layer_Conflict, BaseWrapperLayer)
					ffl = CType(bwl.getUnderlying(), FeedForwardLayer)
				Else
					ffl = DirectCast(Me.layer_Conflict, FeedForwardLayer)
				End If
				If ffl.getNIn() <> W.rows() Then
					'Workaround/hack for ambiguous input shapes (nIn inference) for some RNN models (using NCW format but not recorded in config)
					'We can reliably infer nIn from the shape of the weights array however
					ffl.setNIn(W.rows())
				End If
			End Set
		End Property

	End Class

End Namespace