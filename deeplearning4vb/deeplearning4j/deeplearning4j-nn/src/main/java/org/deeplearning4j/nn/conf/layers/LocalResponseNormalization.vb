Imports System
Imports System.Collections.Generic
Imports lombok
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports EmptyParamInitializer = org.deeplearning4j.nn.params.EmptyParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization

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

Namespace org.deeplearning4j.nn.conf.layers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class LocalResponseNormalization extends Layer
	<Serializable>
	Public Class LocalResponseNormalization
		Inherits Layer

		' Defaults as per http://www.cs.toronto.edu/~fritz/absps/imagenet.pdf
		'Set defaults here as well as in builder, in case users use no-arg constructor instead of builder
		Protected Friend n As Double = 5 ' # adjacent kernal maps
		Protected Friend k As Double = 2 ' constant (e.g. scale)
		Protected Friend beta As Double = 0.75 ' decay rate
		Protected Friend alpha As Double = 1e-4 ' decay rate
		Protected Friend cudnnAllowFallback As Boolean = True
		Protected Friend dataFormat As CNN2DFormat = CNN2DFormat.NCHW

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.k = builder.k_Conflict
			Me.n = builder.n_Conflict
			Me.alpha = builder.alpha_Conflict
			Me.beta = builder.beta_Conflict
			Me.cudnnAllowFallback = builder.cudnnAllowFallback_Conflict
			Me.dataFormat = builder.dataFormat_Conflict
		End Sub

		Public Overrides Function clone() As LocalResponseNormalization
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim clone_Conflict As LocalResponseNormalization = CType(MyBase.clone(), LocalResponseNormalization)
			Return clone_Conflict
		End Function

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim ret As New org.deeplearning4j.nn.layers.normalization.LocalResponseNormalization(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return EmptyParamInitializer.Instance
		End Function


		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN Then
				Throw New System.InvalidOperationException("Invalid input type for LRN layer (layer index = " & layerIndex & ", layer name = """ & LayerName & """): Expected input of type CNN, got " & inputType)
			End If
			Return inputType
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			Preconditions.checkState(inputType.getType() = InputType.Type.CNN, "Only CNN input types can be used with LocalResponseNormalisation, got %s", inputType)
			Me.dataFormat = DirectCast(inputType, InputType.InputTypeConvolutional).getFormat()
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input type for LRN layer (layer name = """ & LayerName & """): null")
			End If

			Return InputTypeUtil.getPreProcessorForInputTypeCnnLayers(inputType, LayerName)
		End Function

		Public Overrides Function getRegularizationByParam(ByVal paramName As String) As IList(Of Regularization)
			Return Nothing
		End Function

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Return False 'No params in LRN
		End Function

		Public Overrides ReadOnly Property GradientNormalization As GradientNormalization
			Get
				Return GradientNormalization.None
			End Get
		End Property

		Public Overrides ReadOnly Property GradientNormalizationThreshold As Double
			Get
				Return 0
			End Get
		End Property

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim actElementsPerEx As val = inputType.arrayElementsPerExample()

			'Forward pass: 3x input size as working memory, in addition to output activations
			'Backward pass: 2x input size as working memory, in addition to epsilons

			Return (New LayerMemoryReport.Builder(layerName, GetType(DenseLayer), inputType, inputType)).standardMemory(0, 0).workingMemory(0, 2 * actElementsPerEx, 0, 3 * actElementsPerEx).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Getter @Setter public static class Builder extends Layer.Builder<Builder>
		Public Class Builder
			Inherits Layer.Builder(Of Builder)

			' defaults based on AlexNet model

			''' <summary>
			''' LRN scaling constant k. Default: 2
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field k was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend k_Conflict As Double = 2

			''' <summary>
			''' Number of adjacent kernel maps to use when doing LRN. default: 5
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field n was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend n_Conflict As Double = 5

			''' <summary>
			''' LRN scaling constant alpha. Default: 1e-4
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field alpha was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend alpha_Conflict As Double = 1e-4

			''' <summary>
			''' Scaling constant beta. Default: 0.75
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field beta was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend beta_Conflict As Double = 0.75

			''' <summary>
			''' When using CuDNN and an error is encountered, should fallback to the non-CuDNN implementatation be allowed?
			''' If set to false, an exception in CuDNN will be propagated back to the user. If false, the built-in
			''' (non-CuDNN) implementation for BatchNormalization will be used
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field cudnnAllowFallback was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend cudnnAllowFallback_Conflict As Boolean = True

'JAVA TO VB CONVERTER NOTE: The field dataFormat was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend dataFormat_Conflict As CNN2DFormat = CNN2DFormat.NCHW

			Public Sub New(ByVal k As Double, ByVal n As Double, ByVal alpha As Double, ByVal beta As Double)
				Me.New(k, n, alpha, beta, True, CNN2DFormat.NCHW)
			End Sub

			Public Sub New(ByVal k As Double, ByVal alpha As Double, ByVal beta As Double)
				Me.setK(k)
				Me.setAlpha(alpha)
				Me.setBeta(beta)
			End Sub

			Public Sub New()
			End Sub

			''' <summary>
			''' LRN scaling constant k. Default: 2
			''' </summary>
			''' <param name="k"> Scaling constant </param>
'JAVA TO VB CONVERTER NOTE: The parameter k was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function k(ByVal k_Conflict As Double) As Builder
				Me.setK(k_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Number of adjacent kernel maps to use when doing LRN. default: 5
			''' </summary>
			''' <param name="n"> Number of adjacent kernel maps </param>
'JAVA TO VB CONVERTER NOTE: The parameter n was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function n(ByVal n_Conflict As Double) As Builder
				Me.setN(n_Conflict)
				Return Me
			End Function

			''' <summary>
			''' LRN scaling constant alpha. Default: 1e-4
			''' </summary>
			''' <param name="alpha"> Scaling constant </param>
'JAVA TO VB CONVERTER NOTE: The parameter alpha was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function alpha(ByVal alpha_Conflict As Double) As Builder
				Me.setAlpha(alpha_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Scaling constant beta. Default: 0.75
			''' </summary>
			''' <param name="beta"> Scaling constant </param>
'JAVA TO VB CONVERTER NOTE: The parameter beta was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function beta(ByVal beta_Conflict As Double) As Builder
				Me.setBeta(beta_Conflict)
				Return Me
			End Function

			''' <summary>
			''' When using CuDNN and an error is encountered, should fallback to the non-CuDNN implementatation be allowed?
			''' If set to false, an exception in CuDNN will be propagated back to the user. If true, the built-in
			''' (non-CuDNN) implementation for BatchNormalization will be used
			''' </summary>
			''' @deprecated Use <seealso cref="helperAllowFallback(Boolean)"/>
			''' 
			''' <param name="allowFallback"> Whether fallback to non-CuDNN implementation should be used </param>
			<Obsolete("Use <seealso cref=""helperAllowFallback(Boolean)""/>")>
			Public Overridable Function cudnnAllowFallback(ByVal allowFallback As Boolean) As Builder
				Me.setCudnnAllowFallback(allowFallback)
				Return Me
			End Function

			''' <summary>
			''' When using CuDNN or MKLDNN and an error is encountered, should fallback to the non-helper implementation be allowed?
			''' If set to false, an exception in the helper will be propagated back to the user. If true, the built-in
			''' (non-MKL/CuDNN) implementation for LocalResponseNormalizationLayer will be used
			''' </summary>
			''' <param name="allowFallback"> Whether fallback to non-CuDNN implementation should be used </param>
			Public Overridable Function helperAllowFallback(ByVal allowFallback As Boolean) As Builder
				Me.cudnnAllowFallback_Conflict = allowFallback
				Return Me
			End Function

			''' <summary>
			''' Set the data format for the CNN activations - NCHW (channels first) or NHWC (channels last).
			''' See <seealso cref="CNN2DFormat"/> for more details.<br>
			''' Default: NCHW </summary>
			''' <param name="format"> Format for activations (in and out) </param>
'JAVA TO VB CONVERTER NOTE: The parameter dataFormat was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dataFormat(ByVal dataFormat_Conflict As CNN2DFormat) As Builder
				Me.dataFormat_Conflict = dataFormat_Conflict
				Return Me
			End Function

			Public Overrides Function build() As LocalResponseNormalization
				Return New LocalResponseNormalization(Me)
			End Function
		End Class

	End Class

End Namespace