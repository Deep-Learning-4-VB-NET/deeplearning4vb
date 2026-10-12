Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports SameDiffLayer = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer
Imports SDLayerParams = org.deeplearning4j.nn.conf.layers.samediff.SDLayerParams
Imports SameDiffLayerUtils = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayerUtils
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports WeightInitUtil = org.deeplearning4j.nn.weights.WeightInitUtil
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties

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

Namespace org.deeplearning4j.nn.layers.samediff.testlayers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true, exclude = {"paramShapes"}) @JsonIgnoreProperties("paramShapes") public class SameDiffDense extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer
	<Serializable>
	Public Class SameDiffDense
		Inherits SameDiffLayer

		Private Shared ReadOnly W_KEYS As IList(Of String) = Collections.singletonList(DefaultParamInitializer.WEIGHT_KEY)
		Private Shared ReadOnly B_KEYS As IList(Of String) = Collections.singletonList(DefaultParamInitializer.BIAS_KEY)
		Private Shared ReadOnly PARAM_KEYS As IList(Of String) = New List(Of String) From {DefaultParamInitializer.WEIGHT_KEY, DefaultParamInitializer.BIAS_KEY}

		Private paramShapes As IDictionary(Of String, Long())

		Private nIn As Long
		Private nOut As Long
		Private activation As Activation

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)

			nIn = builder.nIn_Conflict
			nOut = builder.nOut_Conflict
			activation = builder.activation_Conflict
		End Sub

		Private Sub New()
			'No op constructor for Jackson
		End Sub

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			Return Nothing
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If override Then
				Me.nIn = DirectCast(inputType, InputType.InputTypeFeedForward).getSize()
			End If
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Return Nothing
		End Function

		Public Overrides Sub defineParameters(ByVal params As SDLayerParams)
			params.clear()
			params.addWeightParam(DefaultParamInitializer.WEIGHT_KEY, New Long(){nIn, nOut})
			params.addBiasParam(DefaultParamInitializer.BIAS_KEY, New Long(){1, nOut})
		End Sub

		Public Overrides Sub initializeParameters(ByVal params As IDictionary(Of String, INDArray))
			For Each e As KeyValuePair(Of String, INDArray) In params.SetOfKeyValuePairs()
				If paramWeightInit IsNot Nothing AndAlso paramWeightInit.ContainsKey(e.Key) Then
					paramWeightInit(e.Key).init(nIn, nOut, e.Value.shape(), "c"c, e.Value)
				Else
					If DefaultParamInitializer.BIAS_KEY.Equals(e.Key) Then
						e.Value.assign(0.0)
					Else
						'Normally use 'c' order, but use 'f' for direct comparison to DL4J DenseLayer
						WeightInitUtil.initWeights(nIn, nOut, New Long(){nIn, nOut}, weightInit, Nothing, "f"c, e.Value)
					End If
				End If
			Next e
		End Sub

		Public Overrides Function defineLayer(ByVal sd As SameDiff, ByVal layerInput As SDVariable, ByVal paramTable As IDictionary(Of String, SDVariable), ByVal mask As SDVariable) As SDVariable
			Dim weights As SDVariable = paramTable(DefaultParamInitializer.WEIGHT_KEY)
			Dim bias As SDVariable = paramTable(DefaultParamInitializer.BIAS_KEY)

			Dim mmul As SDVariable = sd.mmul("mmul", layerInput, weights)
			Dim z As SDVariable = mmul.add("z", bias)
			Return activation.asSameDiff("out", sd, z)
		End Function

		Public Overrides Sub applyGlobalConfigToLayer(ByVal globalConfig As NeuralNetConfiguration.Builder)
			If activation = Nothing Then
				activation = SameDiffLayerUtils.fromIActivation(globalConfig.getActivationFn())
			End If
		End Sub

		Public Overrides Function paramReshapeOrder(ByVal param As String) As Char
			'To match DL4J for easy comparison
			Return "f"c
		End Function

		Public Class Builder
			Inherits SameDiffLayer.Builder(Of Builder)

'JAVA TO VB CONVERTER NOTE: The field nIn was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend nIn_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field nOut was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend nOut_Conflict As Integer

'JAVA TO VB CONVERTER NOTE: The field activation was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend activation_Conflict As Activation

'JAVA TO VB CONVERTER NOTE: The parameter nIn was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nIn(ByVal nIn_Conflict As Integer) As Builder
				Me.nIn_Conflict = nIn_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter nOut was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nOut(ByVal nOut_Conflict As Integer) As Builder
				Me.nOut_Conflict = nOut_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter activation was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function activation(ByVal activation_Conflict As Activation) As Builder
				Me.activation_Conflict = activation_Conflict
				Return Me
			End Function

			Public Overrides Function build() As SameDiffDense
				Return New SameDiffDense(Me)
			End Function
		End Class
	End Class

End Namespace