Imports System.Collections.Generic
Imports System.Linq
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports LastTimeStep = org.deeplearning4j.nn.conf.layers.recurrent.LastTimeStep
Imports MaskZeroLayer = org.deeplearning4j.nn.conf.layers.util.MaskZeroLayer
Imports BaseWrapperLayer = org.deeplearning4j.nn.conf.layers.wrapper.BaseWrapperLayer
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasConstraintUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasConstraintUtils
Imports KerasLayerUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils
Imports LSTMParamInitializer = org.deeplearning4j.nn.params.LSTMParamInitializer
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports TimeSeriesUtils = org.deeplearning4j.util.TimeSeriesUtils
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports org.nd4j.common.primitives
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasActivationUtils.getIActivationFromConfig
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasActivationUtils.mapToIActivation
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasInitilizationUtils.getWeightInitFromConfig
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils.getHasBiasFromConfig
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
	''' Imports a Keras LSTM layer as a DL4J LSTM layer.
	''' 
	''' @author dave@skymind.io, Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Data @EqualsAndHashCode(callSuper = false) public class KerasLSTM extends org.deeplearning4j.nn.modelimport.keras.KerasLayer
	Public Class KerasLSTM
		Inherits KerasLayer

		Private ReadOnly LSTM_FORGET_BIAS_INIT_ZERO As String = "zero"
		Private ReadOnly LSTM_FORGET_BIAS_INIT_ONE As String = "one"

		Private ReadOnly NUM_TRAINABLE_PARAMS_KERAS_2 As Integer = 3
		Private ReadOnly NUM_TRAINABLE_PARAMS As Integer = 12

		Private ReadOnly KERAS_PARAM_NAME_W_C As String = "W_c"
		Private ReadOnly KERAS_PARAM_NAME_W_F As String = "W_f"
		Private ReadOnly KERAS_PARAM_NAME_W_I As String = "W_i"
		Private ReadOnly KERAS_PARAM_NAME_W_O As String = "W_o"
		Private ReadOnly KERAS_PARAM_NAME_U_C As String = "U_c"
		Private ReadOnly KERAS_PARAM_NAME_U_F As String = "U_f"
		Private ReadOnly KERAS_PARAM_NAME_U_I As String = "U_i"
		Private ReadOnly KERAS_PARAM_NAME_U_O As String = "U_o"
		Private ReadOnly KERAS_PARAM_NAME_B_C As String = "b_c"
		Private ReadOnly KERAS_PARAM_NAME_B_F As String = "b_f"
		Private ReadOnly KERAS_PARAM_NAME_B_I As String = "b_i"
		Private ReadOnly KERAS_PARAM_NAME_B_O As String = "b_o"
		Private ReadOnly NUM_WEIGHTS_IN_KERAS_LSTM As Integer = 12

'JAVA TO VB CONVERTER NOTE: The field unroll was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend unroll_Conflict As Boolean = False
		Protected Friend returnSequences As Boolean

		''' <summary>
		''' Pass-through constructor from KerasLayer
		''' </summary>
		''' <param name="kerasVersion"> major keras version </param>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasLSTM(System.Nullable<Integer> kerasVersion) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
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
'ORIGINAL LINE: public KerasLSTM(java.util.Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object))
			Me.New(layerConfig, True)
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig">           dictionary containing Keras layer configuration. </param>
		''' <param name="enforceTrainingConfig"> whether to load Keras training configuration </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasLSTM(java.util.Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean)
			Me.New(layerConfig, enforceTrainingConfig, Enumerable.Empty(Of String, KerasLayer)())
		End Sub


		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig">    dictionary containing Keras layer configuration. </param>
		''' <param name="previousLayers"> dictionary containing the previous layers in the topology </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasLSTM(java.util.Map<String, Object> layerConfig, java.util.Map<String, ? extends org.deeplearning4j.nn.modelimport.keras.KerasLayer> previousLayers) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal previousLayers As IDictionary(Of String, KerasLayer))
			Me.New(layerConfig, True, previousLayers)
		End Sub


		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig">           dictionary containing Keras layer configuration </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training-related configuration options </param>
		''' <param name="previousLayers">        - dictionary containing the previous layers in the topology </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasLSTM(java.util.Map<String, Object> layerConfig, boolean enforceTrainingConfig, java.util.Map<String, ? extends org.deeplearning4j.nn.modelimport.keras.KerasLayer> previousLayers) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean, ByVal previousLayers As IDictionary(Of String, KerasLayer))
			MyBase.New(layerConfig, enforceTrainingConfig)

			Dim init As IWeightInit = getWeightInitFromConfig(layerConfig, conf.getLAYER_FIELD_INIT(), enforceTrainingConfig, conf, kerasMajorVersion)

			Dim recurrentInit As IWeightInit = getWeightInitFromConfig(layerConfig, conf.getLAYER_FIELD_INNER_INIT(), enforceTrainingConfig, conf, kerasMajorVersion)

			Dim hasBias As Boolean = getHasBiasFromConfig(layerConfig, conf)

			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			Me.returnSequences = DirectCast(innerConfig(conf.getLAYER_FIELD_RETURN_SEQUENCES()), Boolean?)

			' TODO: support recurrent dropout
			' double recurrentDropout = KerasRnnUtils.getRecurrentDropout(conf, layerConfig);
			Me.unroll_Conflict = KerasRnnUtils.getUnrollRecurrentLayer(conf, layerConfig)

			Dim biasConstraint As LayerConstraint = KerasConstraintUtils.getConstraintsFromConfig(layerConfig, conf.getLAYER_FIELD_B_CONSTRAINT(), conf, kerasMajorVersion)
			Dim weightConstraint As LayerConstraint = KerasConstraintUtils.getConstraintsFromConfig(layerConfig, conf.getLAYER_FIELD_W_CONSTRAINT(), conf, kerasMajorVersion)
			Dim recurrentConstraint As LayerConstraint = KerasConstraintUtils.getConstraintsFromConfig(layerConfig, conf.getLAYER_FIELD_RECURRENT_CONSTRAINT(), conf, kerasMajorVersion)

			Dim maskingConfig As Pair(Of Boolean, Double) = KerasLayerUtils.getMaskingConfiguration(inboundLayerNames_Conflict, previousLayers)

			Dim builder As LSTM.Builder = (New LSTM.Builder()).gateActivationFunction(getGateActivationFromConfig(layerConfig)).forgetGateBiasInit(getForgetBiasInitFromConfig(layerConfig, enforceTrainingConfig)).name(Me.layerName_Conflict).nOut(getNOutFromConfig(layerConfig, conf)).dropOut(Me.dropout).activation(getIActivationFromConfig(layerConfig, conf)).weightInit(init).weightInitRecurrent(recurrentInit).biasInit(0.0).l1(Me.weightL1Regularization).l2(Me.weightL2Regularization).dataFormat(RNNFormat.NWC)
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
		''' Get DL4J Layer. If returnSequences is true, this can be casted to an "LSTM" layer, otherwise it can be casted
		''' to a "LastTimeStep" layer.
		''' </summary>
		''' <returns> LSTM Layer </returns>
		Public Overridable ReadOnly Property LSTMLayer As Layer
			Get
				Return layer_Conflict
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
			If inputType.Length > 1 AndAlso inputType.Length <> 3 Then
				Throw New InvalidKerasConfigurationException("Keras LSTM layer accepts only one single input" & "or three (input to LSTM and two states tensors, but " & "received " & inputType.Length & ".")
			End If
			Dim preProcessor As InputPreProcessor = getInputPreprocessor(inputType)
			If preProcessor IsNot Nothing Then
				If returnSequences Then
					Return preProcessor.getOutputType(inputType(0))
				Else
					Return Me.LSTMLayer.getOutputType(-1, preProcessor.getOutputType(inputType(0)))
				End If
			Else
				Return Me.LSTMLayer.getOutputType(-1, inputType(0))
			End If

		End Function

		''' <summary>
		''' Returns number of trainable parameters in layer.
		''' </summary>
		''' <returns> number of trainable parameters (12) </returns>
		Public Overrides ReadOnly Property NumParams As Integer
			Get
				Return If(kerasMajorVersion = 2, NUM_TRAINABLE_PARAMS_KERAS_2, NUM_TRAINABLE_PARAMS)
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
			If inputType.Length > 1 AndAlso inputType.Length <> 3 Then
				Throw New InvalidKerasConfigurationException("Keras LSTM layer accepts only one single input" & "or three (input to LSTM and two states tensors, but " & "received " & inputType.Length & ".")
			End If
			Dim f As RNNFormat = TimeSeriesUtils.getFormatFromRnnLayer(layer_Conflict)
			Return InputTypeUtil.getPreprocessorForInputTypeRnnLayers(inputType(0), f,layerName_Conflict)
		End Function

		''' <summary>
		''' Set weights for layer.
		''' </summary>
		''' <param name="weights"> LSTM layer weights </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void setWeights(java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> weights) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides WriteOnly Property Weights As IDictionary(Of String, INDArray)
			Set(ByVal weights As IDictionary(Of String, INDArray))
				Me.weights_Conflict = New Dictionary(Of String, INDArray)()
		'         Keras stores LSTM parameters in distinct arrays (e.g., the recurrent weights
		'         * are stored in four matrices: U_c, U_f, U_i, U_o) while DL4J stores them
		'         * concatenated into one matrix (e.g., U = [ U_c U_f U_o U_i ]). Thus we have
		'         * to map the Keras weight matrix to its corresponding DL4J weight submatrix.
		'         
				Dim W_i As INDArray
				Dim W_f As INDArray
				Dim W_c As INDArray
				Dim W_o As INDArray
				Dim U_i As INDArray
				Dim U_f As INDArray
				Dim U_c As INDArray
				Dim U_o As INDArray
				Dim b_i As INDArray
				Dim b_f As INDArray
				Dim b_c As INDArray
				Dim b_o As INDArray
    
    
				If kerasMajorVersion = 2 Then
					Dim W As INDArray
					If weights.ContainsKey(conf.getKERAS_PARAM_NAME_W()) Then
						W = weights(conf.getKERAS_PARAM_NAME_W())
					Else
						Throw New InvalidKerasConfigurationException("Keras LSTM layer does not contain parameter " & conf.getKERAS_PARAM_NAME_W())
					End If
					Dim U As INDArray
					If weights.ContainsKey(conf.getKERAS_PARAM_NAME_RW()) Then
						U = weights(conf.getKERAS_PARAM_NAME_RW())
					Else
						Throw New InvalidKerasConfigurationException("Keras LSTM layer does not contain parameter " & conf.getKERAS_PARAM_NAME_RW())
					End If
					Dim b As INDArray
					If weights.ContainsKey(conf.getKERAS_PARAM_NAME_B()) Then
						b = weights(conf.getKERAS_PARAM_NAME_B())
					Else
						Throw New InvalidKerasConfigurationException("Keras LSTM layer does not contain parameter " & conf.getKERAS_PARAM_NAME_B())
					End If
    
					Dim sliceInterval As val = b.length() \ 4
					W_i = W.get(NDArrayIndex.all(), NDArrayIndex.interval(0, sliceInterval))
					W_f = W.get(NDArrayIndex.all(), NDArrayIndex.interval(sliceInterval, 2 * sliceInterval))
					W_c = W.get(NDArrayIndex.all(), NDArrayIndex.interval(2 * sliceInterval, 3 * sliceInterval))
					W_o = W.get(NDArrayIndex.all(), NDArrayIndex.interval(3 * sliceInterval, 4 * sliceInterval))
					U_i = U.get(NDArrayIndex.all(), NDArrayIndex.interval(0, sliceInterval))
					U_f = U.get(NDArrayIndex.all(), NDArrayIndex.interval(sliceInterval, 2 * sliceInterval))
					U_c = U.get(NDArrayIndex.all(), NDArrayIndex.interval(2 * sliceInterval, 3 * sliceInterval))
					U_o = U.get(NDArrayIndex.all(), NDArrayIndex.interval(3 * sliceInterval, 4 * sliceInterval))
					b_i = b.get(NDArrayIndex.interval(0, sliceInterval))
					b_f = b.get(NDArrayIndex.interval(sliceInterval, 2 * sliceInterval))
					b_c = b.get(NDArrayIndex.interval(2 * sliceInterval, 3 * sliceInterval))
					b_o = b.get(NDArrayIndex.interval(3 * sliceInterval, 4 * sliceInterval))
				Else
					If weights.ContainsKey(KERAS_PARAM_NAME_W_C) Then
						W_c = weights(KERAS_PARAM_NAME_W_C)
					Else
						Throw New InvalidKerasConfigurationException("Keras LSTM layer does not contain parameter " & KERAS_PARAM_NAME_W_C)
					End If
					If weights.ContainsKey(KERAS_PARAM_NAME_W_F) Then
						W_f = weights(KERAS_PARAM_NAME_W_F)
					Else
						Throw New InvalidKerasConfigurationException("Keras LSTM layer does not contain parameter " & KERAS_PARAM_NAME_W_F)
					End If
					If weights.ContainsKey(KERAS_PARAM_NAME_W_O) Then
						W_o = weights(KERAS_PARAM_NAME_W_O)
					Else
						Throw New InvalidKerasConfigurationException("Keras LSTM layer does not contain parameter " & KERAS_PARAM_NAME_W_O)
					End If
					If weights.ContainsKey(KERAS_PARAM_NAME_W_I) Then
						W_i = weights(KERAS_PARAM_NAME_W_I)
					Else
						Throw New InvalidKerasConfigurationException("Keras LSTM layer does not contain parameter " & KERAS_PARAM_NAME_W_I)
					End If
					If weights.ContainsKey(KERAS_PARAM_NAME_U_C) Then
						U_c = weights(KERAS_PARAM_NAME_U_C)
					Else
						Throw New InvalidKerasConfigurationException("Keras LSTM layer does not contain parameter " & KERAS_PARAM_NAME_U_C)
					End If
					If weights.ContainsKey(KERAS_PARAM_NAME_U_F) Then
						U_f = weights(KERAS_PARAM_NAME_U_F)
					Else
						Throw New InvalidKerasConfigurationException("Keras LSTM layer does not contain parameter " & KERAS_PARAM_NAME_U_F)
					End If
					If weights.ContainsKey(KERAS_PARAM_NAME_U_O) Then
						U_o = weights(KERAS_PARAM_NAME_U_O)
					Else
						Throw New InvalidKerasConfigurationException("Keras LSTM layer does not contain parameter " & KERAS_PARAM_NAME_U_O)
					End If
					If weights.ContainsKey(KERAS_PARAM_NAME_U_I) Then
						U_i = weights(KERAS_PARAM_NAME_U_I)
					Else
						Throw New InvalidKerasConfigurationException("Keras LSTM layer does not contain parameter " & KERAS_PARAM_NAME_U_I)
					End If
					If weights.ContainsKey(KERAS_PARAM_NAME_B_C) Then
						b_c = weights(KERAS_PARAM_NAME_B_C)
					Else
						Throw New InvalidKerasConfigurationException("Keras LSTM layer does not contain parameter " & KERAS_PARAM_NAME_B_C)
					End If
					If weights.ContainsKey(KERAS_PARAM_NAME_B_F) Then
						b_f = weights(KERAS_PARAM_NAME_B_F)
					Else
						Throw New InvalidKerasConfigurationException("Keras LSTM layer does not contain parameter " & KERAS_PARAM_NAME_B_F)
					End If
					If weights.ContainsKey(KERAS_PARAM_NAME_B_O) Then
						b_o = weights(KERAS_PARAM_NAME_B_O)
					Else
						Throw New InvalidKerasConfigurationException("Keras LSTM layer does not contain parameter " & KERAS_PARAM_NAME_B_O)
					End If
					If weights.ContainsKey(KERAS_PARAM_NAME_B_I) Then
						b_i = weights(KERAS_PARAM_NAME_B_I)
					Else
						Throw New InvalidKerasConfigurationException("Keras LSTM layer does not contain parameter " & KERAS_PARAM_NAME_B_I)
					End If
    
				End If
    
				' Need to convert from IFCO to CFOI order
				Dim wCols As Integer = W_c.columns()
				Dim wRows As Integer = W_c.rows()
    
				Dim W As INDArray = Nd4j.zeros(wRows, 4 * wCols)
				W.put(New INDArrayIndex(){NDArrayIndex.interval(0, wRows), NDArrayIndex.interval(0, wCols)}, W_c)
				W.put(New INDArrayIndex(){NDArrayIndex.interval(0, wRows), NDArrayIndex.interval(wCols, 2 * wCols)}, W_f)
				W.put(New INDArrayIndex(){NDArrayIndex.interval(0, wRows), NDArrayIndex.interval(2 * wCols, 3 * wCols)}, W_o)
				W.put(New INDArrayIndex(){NDArrayIndex.interval(0, wRows), NDArrayIndex.interval(3 * wCols, 4 * wCols)}, W_i)
				Me.weights_Conflict(LSTMParamInitializer.INPUT_WEIGHT_KEY) = W
    
				Dim uCols As Integer = U_c.columns()
				Dim uRows As Integer = U_c.rows()
				Dim U As INDArray = Nd4j.zeros(uRows, 4 * uCols)
				U.put(New INDArrayIndex(){NDArrayIndex.interval(0, U.rows()), NDArrayIndex.interval(0, uCols)}, U_c)
				U.put(New INDArrayIndex(){NDArrayIndex.interval(0, U.rows()), NDArrayIndex.interval(uCols, 2 * uCols)}, U_f)
				U.put(New INDArrayIndex(){NDArrayIndex.interval(0, U.rows()), NDArrayIndex.interval(2 * uCols, 3 * uCols)}, U_o)
				U.put(New INDArrayIndex(){NDArrayIndex.interval(0, U.rows()), NDArrayIndex.interval(3 * uCols, 4 * uCols)}, U_i)
				Me.weights_Conflict(LSTMParamInitializer.RECURRENT_WEIGHT_KEY) = U
    
    
				Dim bCols As Integer = b_c.columns()
				Dim bRows As Integer = b_c.rows()
				Dim b As INDArray = Nd4j.zeros(bRows, 4 * bCols)
				b.put(New INDArrayIndex(){NDArrayIndex.interval(0, b.rows()), NDArrayIndex.interval(0, bCols)}, b_c)
				b.put(New INDArrayIndex(){NDArrayIndex.interval(0, b.rows()), NDArrayIndex.interval(bCols, 2 * bCols)}, b_f)
				b.put(New INDArrayIndex(){NDArrayIndex.interval(0, b.rows()), NDArrayIndex.interval(2 * bCols, 3 * bCols)}, b_o)
				b.put(New INDArrayIndex(){NDArrayIndex.interval(0, b.rows()), NDArrayIndex.interval(3 * bCols, 4 * bCols)}, b_i)
				Me.weights_Conflict(LSTMParamInitializer.BIAS_KEY) = b
    
				If weights.Count > NUM_WEIGHTS_IN_KERAS_LSTM Then
					Dim paramNames As ISet(Of String) = weights.Keys
					paramNames.remove(KERAS_PARAM_NAME_W_C)
					paramNames.remove(KERAS_PARAM_NAME_W_F)
					paramNames.remove(KERAS_PARAM_NAME_W_I)
					paramNames.remove(KERAS_PARAM_NAME_W_O)
					paramNames.remove(KERAS_PARAM_NAME_U_C)
					paramNames.remove(KERAS_PARAM_NAME_U_F)
					paramNames.remove(KERAS_PARAM_NAME_U_I)
					paramNames.remove(KERAS_PARAM_NAME_U_O)
					paramNames.remove(KERAS_PARAM_NAME_B_C)
					paramNames.remove(KERAS_PARAM_NAME_B_F)
					paramNames.remove(KERAS_PARAM_NAME_B_I)
					paramNames.remove(KERAS_PARAM_NAME_B_O)
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
				If ffl.getNIn() <> wRows Then
					'Workaround/hack for ambiguous input shapes (nIn inference) for some RNN models (using NCW format but not recorded in config)
					'We can reliably infer nIn from the shape of the weights array however
					ffl.setNIn(wRows)
				End If
			End Set
		End Property

		''' <summary>
		''' Get whether LSTM layer should be unrolled (for truncated BPTT).
		''' </summary>
		''' <returns> whether to unroll the LSTM </returns>
		Public Overridable ReadOnly Property Unroll As Boolean
			Get
				Return Me.unroll_Conflict
			End Get
		End Property


		''' <summary>
		''' Get LSTM gate activation function from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> LSTM inner activation function </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.activations.IActivation getGateActivationFromConfig(java.util.Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Overridable Function getGateActivationFromConfig(ByVal layerConfig As IDictionary(Of String, Object)) As IActivation
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If Not innerConfig.ContainsKey(conf.getLAYER_FIELD_INNER_ACTIVATION()) Then
				Throw New InvalidKerasConfigurationException("Keras LSTM layer config missing " & conf.getLAYER_FIELD_INNER_ACTIVATION() & " field")
			End If
			Return mapToIActivation(DirectCast(innerConfig(conf.getLAYER_FIELD_INNER_ACTIVATION()), String), conf)
		End Function

		''' <summary>
		''' Get LSTM forget gate bias initialization from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> LSTM forget gate bias init </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public double getForgetBiasInitFromConfig(java.util.Map<String, Object> layerConfig, boolean train) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Overridable Function getForgetBiasInitFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal train As Boolean) As Double
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			Dim kerasForgetBiasInit As String
			If innerConfig.ContainsKey(conf.getLAYER_FIELD_UNIT_FORGET_BIAS()) Then
				kerasForgetBiasInit = LSTM_FORGET_BIAS_INIT_ONE
			ElseIf Not innerConfig.ContainsKey(conf.getLAYER_FIELD_FORGET_BIAS_INIT()) Then
				Throw New InvalidKerasConfigurationException("Keras LSTM layer config missing " & conf.getLAYER_FIELD_FORGET_BIAS_INIT() & " field")
			Else
				kerasForgetBiasInit = DirectCast(innerConfig(conf.getLAYER_FIELD_FORGET_BIAS_INIT()), String)
			End If
			Dim init As Double
			Select Case kerasForgetBiasInit
				Case LSTM_FORGET_BIAS_INIT_ZERO
					init = 0.0
				Case LSTM_FORGET_BIAS_INIT_ONE
					init = 1.0
				Case Else
					If train Then
						Throw New UnsupportedKerasConfigurationException("Unsupported LSTM forget gate bias initialization: " & kerasForgetBiasInit)
					Else
						init = 1.0
						log.warn("Unsupported LSTM forget gate bias initialization: " & kerasForgetBiasInit & " (using 1 instead)")
					End If
			End Select
			Return init
		End Function
	End Class

End Namespace