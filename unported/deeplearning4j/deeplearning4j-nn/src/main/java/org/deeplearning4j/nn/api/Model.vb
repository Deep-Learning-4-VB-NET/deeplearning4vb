Imports System
Imports System.Collections.Generic
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ConvexOptimizer = org.deeplearning4j.optimize.api.ConvexOptimizer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.deeplearning4j.nn.api


	Public Interface Model

		''' <summary>
		''' Init the model
		''' </summary>
		Sub init()


		''' <summary>
		''' Set the trainingListeners for the ComputationGraph (and all layers in the network)
		''' </summary>
		WriteOnly Property Listeners As ICollection(Of TrainingListener)


		''' <summary>
		''' Set the trainingListeners for the ComputationGraph (and all layers in the network)
		''' </summary>
		WriteOnly Property Listeners As TrainingListener()

		''' <summary>
		''' This method ADDS additional TrainingListener to existing listeners
		''' </summary>
		''' <param name="listener"> </param>
		Sub addListeners(ParamArray ByVal listener() As TrainingListener)


		''' <summary>
		''' All models have a fit method
		''' </summary>
		<Obsolete>
		Sub fit()

		''' <summary>
		''' Update layer weights and biases with gradient change
		''' </summary>
		Sub update(ByVal gradient As Gradient)

		''' <summary>
		''' Perform one update  applying the gradient </summary>
		''' <param name="gradient"> the gradient to apply </param>
		Sub update(ByVal gradient As INDArray, ByVal paramType As String)


		''' <summary>
		''' The score for the model </summary>
		''' <returns> the score for the model </returns>
		Function score() As Double


		''' <summary>
		''' Update the score
		''' </summary>
		Sub computeGradientAndScore(ByVal workspaceMgr As LayerWorkspaceMgr)

		''' <summary>
		''' Parameters of the model (if any) </summary>
		''' <returns> the parameters of the model </returns>
		Function params() As INDArray

		''' <summary>
		''' the number of parameters for the model </summary>
		''' <returns> the number of parameters for the model
		'''  </returns>
		Function numParams() As Long


		''' <summary>
		''' the number of parameters for the model </summary>
		''' <returns> the number of parameters for the model
		'''  </returns>
		Function numParams(ByVal backwards As Boolean) As Long

		''' <summary>
		''' Set the parameters for this model.
		''' This expects a linear ndarray which then be unpacked internally
		''' relative to the expected ordering of the model </summary>
		''' <param name="params"> the parameters for the model </param>
		WriteOnly Property Params As INDArray

		''' <summary>
		''' Set the initial parameters array as a view of the full (backprop) network parameters
		''' NOTE: this is intended to be used internally in MultiLayerNetwork and ComputationGraph, not by users. </summary>
		''' <param name="params"> a 1 x nParams row vector that is a view of the larger (MLN/CG) parameters array </param>
		WriteOnly Property ParamsViewArray As INDArray


		ReadOnly Property GradientsViewArray As INDArray

		''' <summary>
		''' Set the gradients array as a view of the full (backprop) network parameters
		''' NOTE: this is intended to be used internally in MultiLayerNetwork and ComputationGraph, not by users. </summary>
		''' <param name="gradients"> a 1 x nParams row vector that is a view of the larger (MLN/CG) gradients array </param>
		WriteOnly Property BackpropGradientsViewArray As INDArray

		''' <summary>
		''' Fit the model to the given data </summary>
		''' <param name="data"> the data to fit the model to </param>
		Sub fit(ByVal data As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)


		''' <summary>
		''' Get the gradient. Note that this method will not calculate the gradient, it will rather return the gradient
		''' that has been computed before.
		''' For calculating the gradient, see <seealso cref="Model.computeGradientAndScore(LayerWorkspaceMgr)"/> } . </summary>
		''' <returns> the gradient for this model, as calculated before </returns>
		Function gradient() As Gradient

		''' <summary>
		''' Get the gradient and score </summary>
		''' <returns> the gradient and score </returns>
		Function gradientAndScore() As Pair(Of Gradient, Double)

		''' <summary>
		''' The current inputs batch size </summary>
		''' <returns> the current inputs batch size </returns>
		Function batchSize() As Integer


		''' <summary>
		''' The configuration for the neural network </summary>
		''' <returns> the configuration for the neural network </returns>
		Function conf() As NeuralNetConfiguration

		''' <summary>
		''' Setter for the configuration </summary>
		''' <param name="conf"> </param>
		WriteOnly Property Conf As NeuralNetConfiguration

		''' <summary>
		''' The input/feature matrix for the model </summary>
		''' <returns> the input/feature matrix for the model </returns>
		Function input() As INDArray

		''' <summary>
		''' Returns this models optimizer </summary>
		''' <returns> this models optimizer </returns>
		ReadOnly Property Optimizer As ConvexOptimizer

		''' <summary>
		''' Get the parameter </summary>
		''' <param name="param"> the key of the parameter </param>
		''' <returns> the parameter vector/matrix with that particular key </returns>
		Function getParam(ByVal param As String) As INDArray

		''' <summary>
		''' The param table
		''' @return
		''' </summary>
		Function paramTable() As IDictionary(Of String, INDArray)

		''' <summary>
		''' Table of parameters by key, for backprop
		''' For many models (dense layers, etc) - all parameters are backprop parameters </summary>
		''' <param name="backpropParamsOnly"> If true, return backprop params only. If false: return all params (equivalent to
		'''                           paramsTable()) </param>
		Function paramTable(ByVal backpropParamsOnly As Boolean) As IDictionary(Of String, INDArray)

		''' <summary>
		''' Setter for the param table </summary>
		''' <param name="paramTable"> </param>
		WriteOnly Property ParamTable As IDictionary(Of String, INDArray)


		''' <summary>
		''' Set the parameter with a new ndarray </summary>
		''' <param name="key"> the key to se t </param>
		''' <param name="val"> the new ndarray </param>
		Sub setParam(ByVal key As String, ByVal val As INDArray)

		''' <summary>
		''' Clear input
		''' </summary>
		Sub clear()


		''' <summary>
		''' Apply any constraints to the model
		''' </summary>
		Sub applyConstraints(ByVal iteration As Integer, ByVal epoch As Integer)


		Sub close()
	End Interface

End Namespace