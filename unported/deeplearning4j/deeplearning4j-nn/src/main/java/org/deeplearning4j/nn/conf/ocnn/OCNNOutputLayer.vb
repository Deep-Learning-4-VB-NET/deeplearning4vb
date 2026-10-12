Imports System
Imports System.Collections.Generic
Imports lombok
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports BaseOutputLayer = org.deeplearning4j.nn.conf.layers.BaseOutputLayer
Imports LayerValidation = org.deeplearning4j.nn.conf.layers.LayerValidation
Imports OCNNParamInitializer = org.deeplearning4j.nn.layers.ocnn.OCNNParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports ActivationIdentity = org.nd4j.linalg.activations.impl.ActivationIdentity
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports JsonCreator = org.nd4j.shade.jackson.annotation.JsonCreator
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.deeplearning4j.nn.conf.ocnn


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) @JsonIgnoreProperties("lossFn") public class OCNNOutputLayer extends org.deeplearning4j.nn.conf.layers.BaseOutputLayer
	<Serializable>
	Public Class OCNNOutputLayer
		Inherits BaseOutputLayer

		'embedded hidden layer size
		'aka "K"
		Private hiddenSize As Integer

		Private nu As Double = 0.04

		Private windowSize As Integer = 10000

		Private initialRValue As Double = 0.1

		Private configureR As Boolean = True

		''' <summary>
		''' Psuedo code from keras: start_time = time.time() for epoch in range(100): # Train with each example
		''' sess.run(updates, feed_dict={X: train_X,r:rvalue}) rvalue = nnScore(train_X, w_1, w_2, g) with sess.as_default():
		''' rvalue = rvalue.eval() rvalue = np.percentile(rvalue,q=100*nu) print("Epoch = %d, r = %f" % (epoch + 1,rvalue))
		''' </summary>
		Private lastEpochSinceRUpdated As Integer = 0

		Public Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.hiddenSize = builder.hiddenLayerSize_Conflict
			Me.nu = builder.nu_Conflict
			Me.activationFn = builder.activation_Conflict
			Me.windowSize = builder.windowSize_Conflict
			Me.initialRValue = builder.initialRValue_Conflict
			Me.configureR = builder.configureR_Conflict

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonCreator @SuppressWarnings("unused") public OCNNOutputLayer(@JsonProperty("hiddenSize") int hiddenSize, @JsonProperty("nu") double nu, @JsonProperty("activation") org.nd4j.linalg.activations.IActivation activation, @JsonProperty("windowSize") int windowSize, @JsonProperty("initialRValue") double initialRValue, @JsonProperty("configureR") boolean configureR)
		Public Sub New(ByVal hiddenSize As Integer, ByVal nu As Double, ByVal activation As IActivation, ByVal windowSize As Integer, ByVal initialRValue As Double, ByVal configureR As Boolean)
			Me.hiddenSize = hiddenSize
			Me.nu = nu
			Me.activationFn = activation
			Me.windowSize = windowSize
			Me.initialRValue = initialRValue
			Me.configureR = configureR
		End Sub

		Public Overrides ReadOnly Property LossFn As ILossFunction
			Get
				Return lossFn
			End Get
		End Property

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			LayerValidation.assertNInNOutSet("OCNNOutputLayer", LayerName, layerIndex, getNIn(), NOut)

			Dim ret As New org.deeplearning4j.nn.layers.ocnn.OCNNOutputLayer(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			ret.setActivation(activationFn)
			If lastEpochSinceRUpdated = 0 AndAlso configureR Then
				paramTable(OCNNParamInitializer.R_KEY).putScalar(0, initialRValue)
			End If
			Return ret
		End Function

		Public Overrides Property NOut As Long
			Get
				'we don't change number of outputs here
				Return 1L
			End Get
			Set(ByVal nOut As Long)
					Throw New System.NotSupportedException("Unable to specify number of outputs with ocnn. Outputs are fixed to 1.")
				End Set
		End Property

		Public Overrides Function initializer() As ParamInitializer
			Return OCNNParamInitializer.Instance
		End Function


		Public Overrides Function getRegularizationByParam(ByVal paramName As String) As IList(Of Regularization)
			'Not applicable
			Return Nothing
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter @NoArgsConstructor public static class Builder extends org.deeplearning4j.nn.conf.layers.BaseOutputLayer.Builder<Builder>
		Public Class Builder
			Inherits BaseOutputLayer.Builder(Of Builder)

			''' <summary>
			''' The hidden layer size for the one class neural network. Note this would be nOut on a dense layer. NOut in
			''' this neural net is always set to 1 though.
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field hiddenLayerSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend hiddenLayerSize_Conflict As Integer

			''' <summary>
			''' For nu definition see the paper
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field nu was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend nu_Conflict As Double = 0.04

			''' <summary>
			''' The number of examples to use for computing the quantile for the r value update. This value should generally
			''' be the same as the number of examples in the dataset
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field windowSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend windowSize_Conflict As Integer = 10000

			''' <summary>
			''' The activation function to use with ocnn
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field activation was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend activation_Conflict As IActivation = New ActivationIdentity()

			''' <summary>
			''' The initial r value to use for ocnn for definition, see the paper, note this is only active when {@link
			''' #configureR} is specified as true
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field initialRValue was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend initialRValue_Conflict As Double = 0.1

			''' <summary>
			''' Whether to use the specified <seealso cref="initialRValue"/> or use the weight initialization with the neural network
			''' for the r value
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field configureR was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend configureR_Conflict As Boolean = True

			''' <summary>
			''' Whether to use the specified <seealso cref="initialRValue"/> or use the weight initialization with the neural network
			''' for the r value
			''' </summary>
			''' <param name="configureR"> true if we should use the initial <seealso cref="initialRValue"/> </param>
'JAVA TO VB CONVERTER NOTE: The parameter configureR was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function configureR(ByVal configureR_Conflict As Boolean) As Builder
				Me.setConfigureR(configureR_Conflict)
				Return Me
			End Function


			''' <summary>
			''' The initial r value to use for ocnn for definition, see the paper, note this is only active when {@link
			''' #configureR} is specified as true
			''' </summary>
			''' <param name="initialRValue"> the int </param>
'JAVA TO VB CONVERTER NOTE: The parameter initialRValue was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function initialRValue(ByVal initialRValue_Conflict As Double) As Builder
				Me.setInitialRValue(initialRValue_Conflict)
				Return Me
			End Function

			''' <summary>
			''' The number of examples to use for computing the quantile for the r value update. This value should generally
			''' be the same as the number of examples in the dataset
			''' </summary>
			''' <param name="windowSize"> the number of examples to use for computing the quantile of the dataset for the r value
			''' update </param>
'JAVA TO VB CONVERTER NOTE: The parameter windowSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function windowSize(ByVal windowSize_Conflict As Integer) As Builder
				Me.setWindowSize(windowSize_Conflict)
				Return Me
			End Function


			''' <summary>
			''' For nu definition see the paper
			''' </summary>
			''' <param name="nu"> the nu for ocnn </param>
'JAVA TO VB CONVERTER NOTE: The parameter nu was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nu(ByVal nu_Conflict As Double) As Builder
				Me.setNu(nu_Conflict)
				Return Me
			End Function

			''' <summary>
			''' The activation function to use with ocnn
			''' </summary>
			''' <param name="activation"> the activation function to sue </param>
'JAVA TO VB CONVERTER NOTE: The parameter activation was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function activation(ByVal activation_Conflict As IActivation) As Builder
				Me.setActivation(activation_Conflict)
				Return Me
			End Function

			''' <summary>
			''' The hidden layer size for the one class neural network. Note this would be nOut on a dense layer. NOut in
			''' this neural net is always set to 1 though.
			''' </summary>
			''' <param name="hiddenLayerSize"> the hidden layer size to use with ocnn </param>
'JAVA TO VB CONVERTER NOTE: The parameter hiddenLayerSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function hiddenLayerSize(ByVal hiddenLayerSize_Conflict As Integer) As Builder
				Me.setHiddenLayerSize(hiddenLayerSize_Conflict)
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter nOut was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function nOut(ByVal nOut_Conflict As Integer) As Builder
				Throw New System.NotSupportedException("Unable to specify number of outputs with ocnn. Outputs are fixed to 1.")
			End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public OCNNOutputLayer build()
			Public Overrides Function build() As OCNNOutputLayer
				Return New OCNNOutputLayer(Me)
			End Function
		End Class
	End Class

End Namespace