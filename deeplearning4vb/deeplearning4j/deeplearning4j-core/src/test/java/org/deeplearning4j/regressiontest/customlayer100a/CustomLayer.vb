Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports val = lombok.val
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports FeedForwardLayer = org.deeplearning4j.nn.conf.layers.FeedForwardLayer
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
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

Namespace org.deeplearning4j.regressiontest.customlayer100a


	<Serializable>
	Public Class CustomLayer
		Inherits FeedForwardLayer

'JAVA TO VB CONVERTER NOTE: The field secondActivationFunction was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private secondActivationFunction_Conflict As IActivation

		Public Sub New()
			'We need a no-arg constructor so we can deserialize the configuration from JSON or YAML format
			' Without this, you will likely get an exception like the following:
			'com.fasterxml.jackson.databind.JsonMappingException: No suitable constructor found for type [simple type, class org.deeplearning4j.examples.misc.customlayers.layer.CustomLayer]: can not instantiate from JSON object (missing default constructor or creator, or perhaps need to add/enable type information?)
		End Sub

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.secondActivationFunction_Conflict = builder.secondActivationFunction_Conflict
		End Sub

		Public Overridable Property SecondActivationFunction As IActivation
			Get
				'We also need setter/getter methods for our layer configuration fields (if any) for JSON serialization
				Return secondActivationFunction_Conflict
			End Get
			Set(ByVal secondActivationFunction As IActivation)
				'We also need setter/getter methods for our layer configuration fields (if any) for JSON serialization
				Me.secondActivationFunction_Conflict = secondActivationFunction
			End Set
		End Property


		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal iterationListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			'The instantiate method is how we go from the configuration class (i.e., this class) to the implementation class
			' (i.e., a CustomLayerImpl instance)
			'For the most part, it's the same for each type of layer

			Dim myCustomLayer As New CustomLayerImpl(conf, networkDataType)
			myCustomLayer.setListeners(iterationListeners) 'Set the iteration listeners, if any
			myCustomLayer.Index = layerIndex 'Integer index of the layer

			'Parameter view array: In Deeplearning4j, the network parameters for the entire network (all layers) are
			' allocated in one big array. The relevant section of this parameter vector is extracted out for each layer,
			' (i.e., it's a "view" array in that it's a subset of a larger array)
			' This is a row vector, with length equal to the number of parameters in the layer
			myCustomLayer.ParamsViewArray = layerParamsView

			'Initialize the layer parameters. For example,
			' Note that the entries in paramTable (2 entries here: a weight array of shape [nIn,nOut] and biases of shape [1,nOut]
			' are in turn a view of the 'layerParamsView' array.
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			myCustomLayer.ParamTable = paramTable
			myCustomLayer.Conf = conf
			Return myCustomLayer
		End Function

		Public Overrides Function initializer() As ParamInitializer
			'This method returns the parameter initializer for this type of layer
			'In this case, we can use the DefaultParamInitializer, which is the same one used for DenseLayer
			'For more complex layers, you may need to implement a custom parameter initializer
			'See the various parameter initializers here:
			'https://github.com/eclipse/deeplearning4j/tree/master/deeplearning4j-core/src/main/java/org/deeplearning4j/nn/params

			Return DefaultParamInitializer.Instance
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			'Memory report is used to estimate how much memory is required for the layer, for different configurations
			'If you don't need this functionality for your custom layer, you can return a LayerMemoryReport
			' with all 0s, or

			'This implementation: based on DenseLayer implementation
			Dim outputType As InputType = getOutputType(-1, inputType)

			Dim numParams As val = initializer().numParams(Me)
			Dim updaterStateSize As Integer = CInt(Math.Truncate(getIUpdater().stateSize(numParams)))

			Dim trainSizeFixed As Integer = 0
			Dim trainSizeVariable As Integer = 0
			If getIDropout() IsNot Nothing Then
				'Assume we dup the input for dropout
				trainSizeVariable += inputType.arrayElementsPerExample()
			End If

			'Also, during backprop: we do a preOut call -> gives us activations size equal to the output size
			' which is modified in-place by activation function backprop
			' then we have 'epsilonNext' which is equivalent to input size
			trainSizeVariable += outputType.arrayElementsPerExample()

			Return (New LayerMemoryReport.Builder(layerName, GetType(CustomLayer), inputType, outputType)).standardMemory(numParams, updaterStateSize).workingMemory(0, 0, trainSizeFixed, trainSizeVariable).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function


		'Here's an implementation of a builder pattern, to allow us to easily configure the layer
		'Note that we are inheriting all of the FeedForwardLayer.Builder options: things like n
		Public Class Builder
			Inherits FeedForwardLayer.Builder(Of Builder)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private org.nd4j.linalg.activations.IActivation secondActivationFunction;
'JAVA TO VB CONVERTER NOTE: The field secondActivationFunction was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend secondActivationFunction_Conflict As IActivation

			'This is an example of a custom property in the configuration

			''' <summary>
			''' A custom property used in this custom layer example. See the CustomLayerExampleReadme.md for details
			''' </summary>
			''' <param name="secondActivationFunction"> Second activation function for the layer </param>
'JAVA TO VB CONVERTER NOTE: The parameter secondActivationFunction was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function secondActivationFunction(ByVal secondActivationFunction_Conflict As String) As Builder
				Return secondActivationFunction(Activation.fromString(secondActivationFunction_Conflict))
			End Function

			''' <summary>
			''' A custom property used in this custom layer example. See the CustomLayerExampleReadme.md for details
			''' </summary>
			''' <param name="secondActivationFunction"> Second activation function for the layer </param>
'JAVA TO VB CONVERTER NOTE: The parameter secondActivationFunction was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function secondActivationFunction(ByVal secondActivationFunction_Conflict As Activation) As Builder
				Me.secondActivationFunction_Conflict = secondActivationFunction_Conflict.getActivationFunction()
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public CustomLayer build()
			Public Overrides Function build() As CustomLayer
				Return New CustomLayer(Me)
			End Function
		End Class

	End Class

End Namespace