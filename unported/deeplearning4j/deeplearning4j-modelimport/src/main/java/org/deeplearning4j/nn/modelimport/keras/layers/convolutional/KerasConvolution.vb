Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports ConvolutionParamInitializer = org.deeplearning4j.nn.params.ConvolutionParamInitializer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils.removeDefaultWeights

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

Namespace org.deeplearning4j.nn.modelimport.keras.layers.convolutional


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Data @EqualsAndHashCode(callSuper = false) public abstract class KerasConvolution extends org.deeplearning4j.nn.modelimport.keras.KerasLayer
	Public MustInherit Class KerasConvolution
		Inherits KerasLayer

		Protected Friend numTrainableParams As Integer
		Protected Friend hasBias As Boolean

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasConvolution() throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New()
		End Sub

		''' <summary>
		''' Pass-through constructor from KerasLayer
		''' </summary>
		''' <param name="kerasVersion"> major keras version </param>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasConvolution(System.Nullable<Integer> kerasVersion) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
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
'ORIGINAL LINE: public KerasConvolution(java.util.Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
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
'ORIGINAL LINE: public KerasConvolution(java.util.Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean)
			MyBase.New(layerConfig, enforceTrainingConfig)
		End Sub

		''' <summary>
		''' Returns number of trainable parameters in layer.
		''' </summary>
		''' <returns> number of trainable parameters (2) </returns>
		Public Overrides ReadOnly Property NumParams As Integer
			Get
				Return numTrainableParams
			End Get
		End Property

		''' <summary>
		''' Set weights for layer.
		''' </summary>
		''' <param name="weights"> Map from parameter name to INDArray. </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void setWeights(java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> weights) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides WriteOnly Property Weights As IDictionary(Of String, INDArray)
			Set(ByVal weights As IDictionary(Of String, INDArray))
				Me.weights_Conflict = New Dictionary(Of String, INDArray)()
				If weights.ContainsKey(conf.getKERAS_PARAM_NAME_W()) Then
					Dim kerasParamValue As INDArray = weights(conf.getKERAS_PARAM_NAME_W())
					Dim paramValue As INDArray = getConvParameterValues(kerasParamValue)
					Me.weights_Conflict(ConvolutionParamInitializer.WEIGHT_KEY) = paramValue
				Else
					Throw New InvalidKerasConfigurationException("Parameter " & conf.getKERAS_PARAM_NAME_W() & " does not exist in weights")
				End If
    
				If hasBias Then
					If weights.ContainsKey(conf.getKERAS_PARAM_NAME_B()) Then
						Me.weights_Conflict(ConvolutionParamInitializer.BIAS_KEY) = weights(conf.getKERAS_PARAM_NAME_B())
					Else
						Throw New InvalidKerasConfigurationException("Parameter " & conf.getKERAS_PARAM_NAME_B() & " does not exist in weights")
					End If
				End If
				removeDefaultWeights(weights, conf)
			End Set
		End Property

		''' <summary>
		''' Return processed parameter values obtained from Keras convolutional layers.
		''' </summary>
		''' <param name="kerasParamValue"> INDArray containing raw Keras weights to be processed </param>
		''' <returns> Processed weights, according to which backend was used. </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras configuration exception. </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray getConvParameterValues(org.nd4j.linalg.api.ndarray.INDArray kerasParamValue) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overridable Function getConvParameterValues(ByVal kerasParamValue As INDArray) As INDArray
			Dim paramValue As INDArray
			Select Case Me.DimOrder
				Case TENSORFLOW
					If kerasParamValue.rank() = 5 Then
						' CNN 3D case
						paramValue = kerasParamValue.permute(4, 3, 0, 1, 2)
					Else
						' TensorFlow convolutional weights: # rows, # cols, # inputs, # outputs 
						paramValue = kerasParamValue.permute(3, 2, 0, 1)
					End If
				Case THEANO
	'                 Theano convolutional weights match DL4J: # outputs, # inputs, # rows, # cols
	'                 * Theano's default behavior is to rotate filters by 180 degree before application.
	'                 
					paramValue = kerasParamValue.dup()
					Dim i As Integer = 0
					Do While i < paramValue.tensorsAlongDimension(2, 3)
						'dup required since we only want data from the view not the whole array
						Dim copyFilter As INDArray = paramValue.tensorAlongDimension(i, 2, 3).dup()
						Dim flattenedFilter() As Double = copyFilter.ravel().data().asDouble()
						ArrayUtils.reverse(flattenedFilter)
						Dim newFilter As INDArray = Nd4j.create(flattenedFilter, copyFilter.shape())
						'manipulating weights in place to save memory
						Dim inPlaceFilter As INDArray = paramValue.tensorAlongDimension(i, 2, 3)
						inPlaceFilter.muli(0).addi(newFilter.castTo(inPlaceFilter.dataType()))
						i += 1
					Loop
				Case Else
					Throw New InvalidKerasConfigurationException("Unknown keras backend " & Me.DimOrder)
			End Select
			Return paramValue
		End Function
	End Class

End Namespace