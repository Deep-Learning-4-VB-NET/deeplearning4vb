Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports PReLULayer = org.deeplearning4j.nn.conf.layers.PReLULayer
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasConstraintUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasConstraintUtils
Imports KerasLayerUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils
Imports PReLUParamInitializer = org.deeplearning4j.nn.params.PReLUParamInitializer
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasInitilizationUtils.getWeightInitFromConfig

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

Namespace org.deeplearning4j.nn.modelimport.keras.layers.advanced.activations


	''' <summary>
	''' Imports PReLU layer from Keras
	''' 
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class KerasPReLU extends org.deeplearning4j.nn.modelimport.keras.KerasLayer
	Public Class KerasPReLU
		Inherits KerasLayer

		Private ReadOnly ALPHA As String = "alpha"
		Private ReadOnly ALPHA_INIT As String = "alpha_initializer"
		Private ReadOnly ALPHA_CONSTRAINT As String = "alpha_constraint"
		Private ReadOnly SHARED_AXES As String = "shared_axes"

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasPReLU(java.util.Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object))
			Me.New(layerConfig, True)
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig">           dictionary containing Keras layer configuration </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training-related configuration options </param>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasPReLU(java.util.Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean)
			MyBase.New(layerConfig, enforceTrainingConfig)

			Dim weightConstraint As LayerConstraint = KerasConstraintUtils.getConstraintsFromConfig(layerConfig, ALPHA_CONSTRAINT, conf, kerasMajorVersion)

			Dim init As IWeightInit = getWeightInitFromConfig(layerConfig, ALPHA_INIT, enforceTrainingConfig, conf, kerasMajorVersion)
			Dim axes() As Long = getSharedAxes(layerConfig)

			Dim builder As PReLULayer.Builder = (New PReLULayer.Builder()).sharedAxes(axes).weightInit(init).name(layerName_Conflict)
			If weightConstraint IsNot Nothing Then
				builder.constrainWeights(weightConstraint)
			End If
			Me.layer_Conflict = builder.build()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private long[] getSharedAxes(java.util.Map<String, Object> layerConfig) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Private Function getSharedAxes(ByVal layerConfig As IDictionary(Of String, Object)) As Long()
			Dim axes() As Long = Nothing
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			Try
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<Integer> axesList = (java.util.List<Integer>) innerConfig.get(SHARED_AXES);
				Dim axesList As IList(Of Integer) = DirectCast(innerConfig(SHARED_AXES), IList(Of Integer))
				Dim intAxes() As Integer = ArrayUtil.toArray(axesList)
				axes = New Long(intAxes.Length - 1){}
				For i As Integer = 0 To intAxes.Length - 1
					axes(i) = CLng(intAxes(i))
				Next i
			Catch e As Exception
				' no shared axes
			End Try
			Return axes
		End Function

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
				Throw New InvalidKerasConfigurationException("Keras PReLU layer accepts only one input (received " & inputType.Length & ")")
			End If
			Dim inType As InputType = inputType(0)

			' Dynamically infer input shape of PReLU layer from input type
			Dim shapedLayer As PReLULayer = DirectCast(Me.layer_Conflict, PReLULayer)
			shapedLayer.setInputShape(inType.Shape)
			Me.layer_Conflict = shapedLayer

			Return Me.PReLULayer.getOutputType(-1, inputType(0))
		End Function

		''' <summary>
		''' Get DL4J ActivationLayer.
		''' </summary>
		''' <returns> ActivationLayer </returns>
		Public Overridable ReadOnly Property PReLULayer As PReLULayer
			Get
				Return DirectCast(Me.layer_Conflict, PReLULayer)
			End Get
		End Property

		''' <summary>
		''' Set weights for layer.
		''' </summary>
		''' <param name="weights"> Dense layer weights </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void setWeights(java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> weights) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides WriteOnly Property Weights As IDictionary(Of String, INDArray)
			Set(ByVal weights As IDictionary(Of String, INDArray))
				Me.weights_Conflict = New Dictionary(Of String, INDArray)()
				If weights.ContainsKey(ALPHA) Then
					Me.weights_Conflict(PReLUParamInitializer.WEIGHT_KEY) = weights(ALPHA)
				Else
					Throw New InvalidKerasConfigurationException("Parameter " & ALPHA & " does not exist in weights")
				End If
				If weights.Count > 1 Then
					Dim paramNames As ISet(Of String) = weights.Keys
					paramNames.remove(ALPHA)
					Dim unknownParamNames As String = paramNames.ToString()
					log.warn("Attemping to set weights for unknown parameters: " & unknownParamNames.Substring(1, (unknownParamNames.Length - 1) - 1))
				End If
			End Set
		End Property

	End Class

End Namespace