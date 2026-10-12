Imports System
Imports System.Collections.Generic
Imports lombok
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports EmptyParamInitializer = org.deeplearning4j.nn.params.EmptyParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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

Namespace org.deeplearning4j.nn.conf.layers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class ActivationLayer extends NoParamLayer
	<Serializable>
	Public Class ActivationLayer
		Inherits NoParamLayer

		Protected Friend activationFn As IActivation

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.activationFn = builder.activationFn
			initializeConstraints(builder)
		End Sub

		''' <param name="activation"> Activation function for the layer </param>
		Public Sub New(ByVal activation As Activation)
			Me.New((New Builder()).activation(activation))
		End Sub

		''' <param name="activationFn"> Activation function for the layer </param>
		Public Sub New(ByVal activationFn As IActivation)
			Me.New((New Builder()).activation(activationFn))
		End Sub

		Public Overrides Function clone() As ActivationLayer
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim clone_Conflict As ActivationLayer = CType(MyBase.clone(), ActivationLayer)
			Return clone_Conflict
		End Function

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			Dim ret As New org.deeplearning4j.nn.layers.ActivationLayer(conf, networkDataType)
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
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input type: null for layer name """ & getLayerName() & """")
			End If
			Return inputType
		End Function

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			'No input preprocessor required for any input
			Return Nothing
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim actElementsPerEx As val = inputType.arrayElementsPerExample()

			Return (New LayerMemoryReport.Builder(layerName, GetType(ActivationLayer), inputType, inputType)).standardMemory(0, 0).workingMemory(0, 0, 0, actElementsPerEx).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			'No op
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @NoArgsConstructor @Getter @Setter public static class Builder extends org.deeplearning4j.nn.conf.layers.Layer.Builder<Builder>
		Public Class Builder
			Inherits org.deeplearning4j.nn.conf.layers.Layer.Builder(Of Builder)

			''' <summary>
			''' Activation function for the layer
			''' </summary>
			Friend activationFn As IActivation = Nothing

			''' <summary>
			''' Layer activation function. Typical values include:<br> "relu" (rectified linear), "tanh", "sigmoid",
			''' "softmax", "hardtanh", "leakyrelu", "maxout", "softsign", "softplus"
			''' </summary>
			''' @deprecated Use <seealso cref="activation(Activation)"/> or <seealso cref="@activation(IActivation)"/> 
			<Obsolete("Use <seealso cref=""activation(Activation)""/> or <seealso cref=""@activation(IActivation)""/>")>
			Public Overridable Function activation(ByVal activationFunction As String) As Builder
				Return activation(Activation.fromString(activationFunction))
			End Function

			''' <param name="activationFunction"> Activation function for the layer </param>
			Public Overridable Function activation(ByVal activationFunction As IActivation) As Builder
				Me.setActivationFn(activationFunction)
				Return Me
			End Function

			''' <param name="activation"> Activation function for the layer </param>
'JAVA TO VB CONVERTER NOTE: The parameter activation was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function activation(ByVal activation_Conflict As Activation) As Builder
				Return activation(activation_Conflict.getActivationFunction())
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public ActivationLayer build()
			Public Overrides Function build() As ActivationLayer
				Return New ActivationLayer(Me)
			End Function
		End Class
	End Class

End Namespace