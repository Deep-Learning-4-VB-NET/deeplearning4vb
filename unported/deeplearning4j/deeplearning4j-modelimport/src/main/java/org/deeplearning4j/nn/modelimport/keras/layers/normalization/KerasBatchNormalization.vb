Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports BatchNormalization = org.deeplearning4j.nn.conf.layers.BatchNormalization
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasConstraintUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasConstraintUtils
Imports KerasLayerUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils
Imports BatchNormalizationParamInitializer = org.deeplearning4j.nn.params.BatchNormalizationParamInitializer
Imports OneTimeLogger = org.nd4j.common.util.OneTimeLogger
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.nn.modelimport.keras.layers.normalization


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Data @EqualsAndHashCode(callSuper = false) public class KerasBatchNormalization extends org.deeplearning4j.nn.modelimport.keras.KerasLayer
	Public Class KerasBatchNormalization
		Inherits KerasLayer

		' Keras layer configuration fields. 
		Private ReadOnly LAYER_BATCHNORM_MODE_1 As Integer = 1
		Private ReadOnly LAYER_BATCHNORM_MODE_2 As Integer = 2
		Private ReadOnly LAYER_FIELD_GAMMA_REGULARIZER As String = "gamma_regularizer"
		Private ReadOnly LAYER_FIELD_BETA_REGULARIZER As String = "beta_regularizer"
		Private ReadOnly LAYER_FIELD_MODE As String = "mode"
		Private ReadOnly LAYER_FIELD_AXIS As String = "axis"
		Private ReadOnly LAYER_FIELD_MOMENTUM As String = "momentum"
		Private ReadOnly LAYER_FIELD_EPSILON As String = "epsilon"
		Private ReadOnly LAYER_FIELD_SCALE As String = "scale"
		Private ReadOnly LAYER_FIELD_CENTER As String = "center"


		' Keras layer parameter names. 
		Private ReadOnly NUM_TRAINABLE_PARAMS As Integer = 4
		Private ReadOnly PARAM_NAME_GAMMA As String = "gamma"
		Private ReadOnly PARAM_NAME_BETA As String = "beta"
		Private ReadOnly PARAM_NAME_RUNNING_MEAN As String = "running_mean"
		Private ReadOnly PARAM_NAME_RUNNING_STD As String = "running_std"


		Private scale As Boolean = True
		Private center As Boolean = True


		''' <summary>
		''' Pass-through constructor from KerasLayer
		''' </summary>
		''' <param name="kerasVersion"> major keras version </param>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasBatchNormalization(System.Nullable<Integer> kerasVersion) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
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
'ORIGINAL LINE: public KerasBatchNormalization(Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
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
'ORIGINAL LINE: public KerasBatchNormalization(Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean)
			Me.New(layerConfig,enforceTrainingConfig, java.util.Collections.emptyMap())
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig">           dictionary containing Keras layer configuration </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training-related configuration options </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasBatchNormalization(Map<String, Object> layerConfig, boolean enforceTrainingConfig,Map<String,? extends org.deeplearning4j.nn.modelimport.keras.KerasLayer> previousLayers) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean, ByVal previousLayers As IDictionary(Of String, KerasLayer))
			MyBase.New(layerConfig, enforceTrainingConfig)
			Dim config2 As Object = layerConfig("config")
			Dim config1 As IDictionary(Of String, Object) = DirectCast(config2, IDictionary(Of String, Object))
			'default ordering
			Dim inboundNodes As IList(Of Object) = DirectCast(layerConfig(conf.getLAYER_FIELD_INBOUND_NODES()), IList(Of Object))
			Dim cnn2DFormat As CNN2DFormat = CNN2DFormat.NCHW

			If inboundNodes.Count > 0 Then
				Dim list As IList(Of Object) = DirectCast(inboundNodes(0), IList(Of Object))
				Dim list1 As IList(Of Object) = DirectCast(list(0), IList(Of Object))
				Dim inputName As String = list1(0).ToString()
				Dim kerasLayer As KerasLayer = previousLayers(inputName)
				Dim dimOrderFromConfig As DimOrder = KerasLayerUtils.getDimOrderFromConfig(kerasLayer.getOriginalLayerConfig(), kerasLayer.getConf())
				If dimOrderFromConfig = DimOrder.NONE OrElse dimOrderFromConfig = DimOrder.TENSORFLOW Then
					cnn2DFormat = CNN2DFormat.NHWC
				End If

			End If

			Me.scale = getScaleParameter(layerConfig)
			Me.center = getCenterParameter(layerConfig)

			' TODO: these helper functions should return regularizers that we use in constructor
			getGammaRegularizerFromConfig(layerConfig, enforceTrainingConfig)
			getBetaRegularizerFromConfig(layerConfig, enforceTrainingConfig)
			Dim batchNormMode As Integer = getBatchNormMode(layerConfig, enforceTrainingConfig)
			If batchNormMode <> 0 Then
				Throw New UnsupportedKerasConfigurationException("Unsupported batch normalization mode " & batchNormMode & "Keras modes 1 and 2 have been removed from keras 2.x altogether." & "Try running with mode 0.")
			End If
			Dim batchNormAxis As Integer = getBatchNormAxis(layerConfig)
			If Not (batchNormAxis = 3 OrElse batchNormAxis = -1) Then
				OneTimeLogger.warn(log,"Warning: batch normalization axis " & batchNormAxis & vbLf & " DL4J currently picks batch norm dimensions for you, according to industry" & "standard conventions. If your results do not match, please file an issue.")
			End If

			Dim betaConstraint As LayerConstraint = KerasConstraintUtils.getConstraintsFromConfig(layerConfig, conf.getLAYER_FIELD_BATCHNORMALIZATION_BETA_CONSTRAINT(), conf, kerasMajorVersion)
			Dim gammaConstraint As LayerConstraint = KerasConstraintUtils.getConstraintsFromConfig(layerConfig, conf.getLAYER_FIELD_BATCHNORMALIZATION_GAMMA_CONSTRAINT(), conf, kerasMajorVersion)

			Dim builder As BatchNormalization.Builder = (New BatchNormalization.Builder()).name(Me.layerName_Conflict).dropOut(Me.dropout).minibatch(True).lockGammaBeta(False).useLogStd(False).decay(getMomentumFromConfig(layerConfig)).eps(getEpsFromConfig(layerConfig))
			If betaConstraint IsNot Nothing Then
				builder.constrainBeta(betaConstraint)
			End If
			If gammaConstraint IsNot Nothing Then
				builder.constrainGamma(gammaConstraint)
			End If
			builder.setCnn2DFormat(cnn2DFormat)
			Me.layer_Conflict = builder.build()
		End Sub

		''' <summary>
		''' Get DL4J BatchNormalizationLayer.
		''' </summary>
		''' <returns> BatchNormalizationLayer </returns>
		Public Overridable ReadOnly Property BatchNormalizationLayer As BatchNormalization
			Get
				Return DirectCast(Me.layer_Conflict, BatchNormalization)
			End Get
		End Property

		''' <summary>
		''' Get layer output type.
		''' </summary>
		''' <param name="inputType"> Array of InputTypes </param>
		''' <returns> output type as InputType </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(org.deeplearning4j.nn.conf.inputs.InputType... inputType) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides Function getOutputType(ParamArray ByVal inputType() As InputType) As InputType
			If inputType.Length > 1 Then
				Throw New InvalidKerasConfigurationException("Keras BatchNorm layer accepts only one input (received " & inputType.Length & ")")
			End If
			Return Me.BatchNormalizationLayer.getOutputType(-1, inputType(0))
		End Function

		''' <summary>
		''' Returns number of trainable parameters in layer.
		''' </summary>
		''' <returns> number of trainable parameters (4) </returns>
		Public Overrides ReadOnly Property NumParams As Integer
			Get
				Return NUM_TRAINABLE_PARAMS
			End Get
		End Property

		''' <summary>
		''' Set weights for layer.
		''' </summary>
		''' <param name="weights"> Map from parameter name to INDArray. </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void setWeights(Map<String, org.nd4j.linalg.api.ndarray.INDArray> weights) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides WriteOnly Property Weights As IDictionary(Of String, INDArray)
			Set(ByVal weights As IDictionary(Of String, INDArray))
				Me.weights_Conflict = New Dictionary(Of String, INDArray)()
				If center Then
					If weights.ContainsKey(PARAM_NAME_BETA) Then
						Me.weights_Conflict(BatchNormalizationParamInitializer.BETA) = weights(PARAM_NAME_BETA)
					Else
						Throw New InvalidKerasConfigurationException("Parameter " & PARAM_NAME_BETA & " does not exist in weights")
					End If
				Else
					Dim dummyBeta As INDArray = Nd4j.zerosLike(weights(PARAM_NAME_BETA))
					Me.weights_Conflict(BatchNormalizationParamInitializer.BETA) = dummyBeta
				End If
				If scale Then
					If weights.ContainsKey(PARAM_NAME_GAMMA) Then
						Me.weights_Conflict(BatchNormalizationParamInitializer.GAMMA) = weights(PARAM_NAME_GAMMA)
					Else
						Throw New InvalidKerasConfigurationException("Parameter " & PARAM_NAME_GAMMA & " does not exist in weights")
					End If
				Else
					Dim dummyGamma As INDArray = If(weights.ContainsKey(PARAM_NAME_GAMMA), Nd4j.onesLike(weights(PARAM_NAME_GAMMA)), Nd4j.onesLike(weights(PARAM_NAME_BETA)))
					Me.weights_Conflict(BatchNormalizationParamInitializer.GAMMA) = dummyGamma
				End If
				If weights.ContainsKey(conf.getLAYER_FIELD_BATCHNORMALIZATION_MOVING_MEAN()) Then
					Me.weights_Conflict(BatchNormalizationParamInitializer.GLOBAL_MEAN) = weights(conf.getLAYER_FIELD_BATCHNORMALIZATION_MOVING_MEAN())
				Else
					Throw New InvalidKerasConfigurationException("Parameter " & conf.getLAYER_FIELD_BATCHNORMALIZATION_MOVING_MEAN() & " does not exist in weights")
				End If
				If weights.ContainsKey(conf.getLAYER_FIELD_BATCHNORMALIZATION_MOVING_VARIANCE()) Then
					Me.weights_Conflict(BatchNormalizationParamInitializer.GLOBAL_VAR) = weights(conf.getLAYER_FIELD_BATCHNORMALIZATION_MOVING_VARIANCE())
				Else
					Throw New InvalidKerasConfigurationException("Parameter " & conf.getLAYER_FIELD_BATCHNORMALIZATION_MOVING_VARIANCE() & " does not exist in weights")
				End If
				If weights.Count > 4 Then
					Dim paramNames As ISet(Of String) = weights.Keys
					paramNames.remove(PARAM_NAME_BETA)
					paramNames.remove(PARAM_NAME_GAMMA)
					paramNames.remove(conf.getLAYER_FIELD_BATCHNORMALIZATION_MOVING_MEAN())
					paramNames.remove(conf.getLAYER_FIELD_BATCHNORMALIZATION_MOVING_VARIANCE())
					Dim unknownParamNames As String = paramNames.ToString()
					log.warn("Attempting to set weights for unknown parameters: " & unknownParamNames.Substring(1, (unknownParamNames.Length - 1) - 1))
				End If
			End Set
		End Property

		''' <summary>
		''' Get BatchNormalization epsilon parameter from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> epsilon </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private double getEpsFromConfig(Map<String, Object> layerConfig) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Private Function getEpsFromConfig(ByVal layerConfig As IDictionary(Of String, Object)) As Double
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If Not innerConfig.ContainsKey(LAYER_FIELD_EPSILON) Then
				Throw New InvalidKerasConfigurationException("Keras BatchNorm layer config missing " & LAYER_FIELD_EPSILON & " field")
			End If
			Return DirectCast(innerConfig(LAYER_FIELD_EPSILON), Double)
		End Function

		''' <summary>
		''' Get BatchNormalization momentum parameter from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> momentum </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private double getMomentumFromConfig(Map<String, Object> layerConfig) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Private Function getMomentumFromConfig(ByVal layerConfig As IDictionary(Of String, Object)) As Double
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If Not innerConfig.ContainsKey(LAYER_FIELD_MOMENTUM) Then
				Throw New InvalidKerasConfigurationException("Keras BatchNorm layer config missing " & LAYER_FIELD_MOMENTUM & " field")
			End If
			Return DirectCast(innerConfig(LAYER_FIELD_MOMENTUM), Double)
		End Function

		''' <summary>
		''' Get BatchNormalization gamma regularizer from Keras layer configuration. Currently unsupported.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> Batchnormalization gamma regularizer </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void getGammaRegularizerFromConfig(Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Private Sub getGammaRegularizerFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean)
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If innerConfig(LAYER_FIELD_GAMMA_REGULARIZER) IsNot Nothing Then
				If enforceTrainingConfig Then
					Throw New UnsupportedKerasConfigurationException("Regularization for BatchNormalization gamma parameter not supported")
				Else
					log.warn("Regularization for BatchNormalization gamma parameter not supported...ignoring.")
				End If
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private boolean getScaleParameter(Map<String, Object> layerConfig) throws UnsupportedOperationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Private Function getScaleParameter(ByVal layerConfig As IDictionary(Of String, Object)) As Boolean
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If innerConfig.ContainsKey(LAYER_FIELD_SCALE) Then
				Return DirectCast(innerConfig(LAYER_FIELD_SCALE), Boolean)
			Else
				Return True
			End If
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private boolean getCenterParameter(Map<String, Object> layerConfig) throws UnsupportedOperationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Private Function getCenterParameter(ByVal layerConfig As IDictionary(Of String, Object)) As Boolean
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If innerConfig.ContainsKey(LAYER_FIELD_CENTER) Then
				Return DirectCast(innerConfig(LAYER_FIELD_CENTER), Boolean)
			Else
				Return True
			End If
		End Function

		''' <summary>
		''' Get BatchNormalization beta regularizer from Keras layer configuration. Currently unsupported.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> Batchnormalization beta regularizer </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void getBetaRegularizerFromConfig(Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Private Sub getBetaRegularizerFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean)
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If innerConfig(LAYER_FIELD_BETA_REGULARIZER) IsNot Nothing Then
				If enforceTrainingConfig Then
					Throw New UnsupportedKerasConfigurationException("Regularization for BatchNormalization beta parameter not supported")
				Else
					log.warn("Regularization for BatchNormalization beta parameter not supported...ignoring.")
				End If
			End If
		End Sub

		''' <summary>
		''' Get BatchNormalization "mode" from Keras layer configuration. Most modes currently unsupported.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> batchnormalization mode </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private int getBatchNormMode(Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Private Function getBatchNormMode(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean) As Integer
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			Dim batchNormMode As Integer = 0
			If Me.kerasMajorVersion = 1 And Not innerConfig.ContainsKey(LAYER_FIELD_MODE) Then
				Throw New InvalidKerasConfigurationException("Keras BatchNorm layer config missing " & LAYER_FIELD_MODE & " field")
			End If
			If Me.kerasMajorVersion = 1 Then
				batchNormMode = DirectCast(innerConfig(LAYER_FIELD_MODE), Integer)
			End If
			Select Case batchNormMode
				Case LAYER_BATCHNORM_MODE_1
					Throw New UnsupportedKerasConfigurationException("Keras BatchNormalization mode " & LAYER_BATCHNORM_MODE_1 & " (sample-wise) not supported")
				Case LAYER_BATCHNORM_MODE_2
					Throw New UnsupportedKerasConfigurationException("Keras BatchNormalization (per-batch statistics during testing) " & LAYER_BATCHNORM_MODE_2 & " not supported")
			End Select
			Return batchNormMode
		End Function

		''' <summary>
		''' Get BatchNormalization axis from Keras layer configuration. Currently unused.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> batchnorm axis </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private int getBatchNormAxis(Map<String, Object> layerConfig) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Private Function getBatchNormAxis(ByVal layerConfig As IDictionary(Of String, Object)) As Integer
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			Dim batchNormAxis As Object = innerConfig(LAYER_FIELD_AXIS)
			If TypeOf batchNormAxis Is System.Collections.IList Then
				Return CType(DirectCast(batchNormAxis, System.Collections.IList)(0), Number).intValue()
			End If
			Return DirectCast(innerConfig(LAYER_FIELD_AXIS), Number).intValue()
		End Function
	End Class

End Namespace