Imports System
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports org.nd4j.common.primitives

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
	Public Class CustomLayerImpl
		Inherits BaseLayer(Of CustomLayer) 'Generic parameter here: the configuration class type

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub


		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
	'        
	'        The activate method is used for doing forward pass. Note that it relies on the pre-output method;
	'        essentially we are just applying the activation function (or, functions in this example).
	'        In this particular (contrived) example, we have TWO activation functions - one for the first half of the outputs
	'        and another for the second half.
	'         

			Dim output As INDArray = preOutput(training, workspaceMgr)
			Dim columns As Integer = output.columns()

			Dim firstHalf As INDArray = output.get(NDArrayIndex.all(), NDArrayIndex.interval(0, columns \ 2))
			Dim secondHalf As INDArray = output.get(NDArrayIndex.all(), NDArrayIndex.interval(columns \ 2, columns))

			Dim activation1 As IActivation = layerConf().getActivationFn()
			Dim activation2 As IActivation = CType(conf.getLayer(), CustomLayer).SecondActivationFunction

			'IActivation function instances modify the activation functions in-place
			activation1.getActivation(firstHalf, training)
			activation2.getActivation(secondHalf, training)

			Return output
		End Function

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property


		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
	'        
	'        The baockprop gradient method here is very similar to the BaseLayer backprop gradient implementation
	'        The only major difference is the two activation functions we have added in this example.
	'
	'        Note that epsilon is dL/da - i.e., the derivative of the loss function with respect to the activations.
	'        It has the exact same shape as the activation arrays (i.e., the output of preOut and activate methods)
	'        This is NOT the 'delta' commonly used in the neural network literature; the delta is obtained from the
	'        epsilon ("epsilon" is dl4j's notation) by doing an element-wise product with the activation function derivative.
	'
	'        Note the following:
	'        1. Is it very important that you use the gradientViews arrays for the results.
	'           Note the gradientViews.get(...) and the in-place operations here.
	'           This is because DL4J uses a single large array for the gradients for efficiency. Subsets of this array (views)
	'           are distributed to each of the layers for efficient backprop and memory management.
	'        2. The method returns two things, as a Pair:
	'           (a) a Gradient object (essentially a Map<String,INDArray> of the gradients for each parameter (again, these
	'               are views of the full network gradient array)
	'           (b) an INDArray. This INDArray is the 'epsilon' to pass to the layer below. i.e., it is the gradient with
	'               respect to the input to this layer
	'
	'        

			Dim activationDerivative As INDArray = preOutput(True, workspaceMgr)
			Dim columns As Integer = activationDerivative.columns()

			Dim firstHalf As INDArray = activationDerivative.get(NDArrayIndex.all(), NDArrayIndex.interval(0, columns \ 2))
			Dim secondHalf As INDArray = activationDerivative.get(NDArrayIndex.all(), NDArrayIndex.interval(columns \ 2, columns))

			Dim epsilonFirstHalf As INDArray = epsilon.get(NDArrayIndex.all(), NDArrayIndex.interval(0, columns \ 2))
			Dim epsilonSecondHalf As INDArray = epsilon.get(NDArrayIndex.all(), NDArrayIndex.interval(columns \ 2, columns))

			Dim activation1 As IActivation = layerConf().getActivationFn()
			Dim activation2 As IActivation = CType(conf.getLayer(), CustomLayer).SecondActivationFunction

			'IActivation backprop method modifies the 'firstHalf' and 'secondHalf' arrays in-place, to contain dL/dz
			activation1.backprop(firstHalf, epsilonFirstHalf)
			activation2.backprop(secondHalf, epsilonSecondHalf)

			'The remaining code for this method: just copy & pasted from BaseLayer.backpropGradient
	'        INDArray delta = epsilon.muli(activationDerivative);
			If maskArray IsNot Nothing Then
				activationDerivative.muliColumnVector(maskArray)
			End If

			Dim ret As Gradient = New DefaultGradient()

			Dim weightGrad As INDArray = gradientViews(DefaultParamInitializer.WEIGHT_KEY) 'f order
			Nd4j.gemm(input, activationDerivative, weightGrad, True, False, 1.0, 0.0)
			Dim biasGrad As INDArray = gradientViews(DefaultParamInitializer.BIAS_KEY)
			biasGrad.assign(activationDerivative.sum(0)) 'TODO: do this without the assign

			ret.gradientForVariable()(DefaultParamInitializer.WEIGHT_KEY) = weightGrad
			ret.gradientForVariable()(DefaultParamInitializer.BIAS_KEY) = biasGrad

			Dim epsilonNext As INDArray = params(DefaultParamInitializer.WEIGHT_KEY).mmul(activationDerivative.transpose()).transpose()

			Return New Pair(Of Gradient, INDArray)(ret, epsilonNext)
		End Function

	End Class

End Namespace