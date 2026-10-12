Imports System.Collections.Generic
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
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

Namespace org.deeplearning4j.nn.api


	''' <summary>
	''' Param initializer for a layer
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Interface ParamInitializer

		Function numParams(ByVal conf As NeuralNetConfiguration) As Long

		Function numParams(ByVal layer As Layer) As Long

		''' <summary>
		''' Get a list of all parameter keys given the layer configuration
		''' </summary>
		''' <param name="layer"> Layer </param>
		''' <returns> All parameter keys </returns>
		Function paramKeys(ByVal layer As Layer) As IList(Of String)

		''' <summary>
		''' Weight parameter keys given the layer configuration
		''' </summary>
		''' <param name="layer"> Layer </param>
		''' <returns> Weight parameter keys </returns>
		Function weightKeys(ByVal layer As Layer) As IList(Of String)

		''' <summary>
		''' Bias parameter keys given the layer configuration
		''' </summary>
		''' <param name="layer"> Layer </param>
		''' <returns> Bias parameter keys </returns>
		Function biasKeys(ByVal layer As Layer) As IList(Of String)

		''' <summary>
		''' Is the specified parameter a weight?
		''' </summary>
		''' <param name="layer"> Layer </param>
		''' <param name="key"> Key to check </param>
		''' <returns> True if parameter is a weight </returns>
		Function isWeightParam(ByVal layer As Layer, ByVal key As String) As Boolean

		''' <summary>
		''' Is the specified parameter a bias?
		''' </summary>
		''' <param name="layer"> Layer </param>
		''' <param name="key"> Key to check </param>
		''' <returns> True if parameter is a bias </returns>
		Function isBiasParam(ByVal layer As Layer, ByVal key As String) As Boolean

		''' <summary>
		''' Initialize the parameters
		''' </summary>
		''' <param name="conf">             the configuration </param>
		''' <param name="paramsView">       a view of the full network (backprop) parameters </param>
		''' <param name="initializeParams"> if true: initialize the parameters according to the configuration. If false: don't modify the
		'''                         values in the paramsView array (but do select out the appropriate subset, reshape etc as required) </param>
		''' <returns> Map of parameters keyed by type (view of the 'paramsView' array) </returns>
		Function init(ByVal conf As NeuralNetConfiguration, ByVal paramsView As INDArray, ByVal initializeParams As Boolean) As IDictionary(Of String, INDArray)

		''' <summary>
		''' Return a map of gradients (in their standard non-flattened representation), taken from the flattened (row vector) gradientView array.
		''' The idea is that operates in exactly the same way as the paramsView does in <seealso cref="init(Map, NeuralNetConfiguration, INDArray)"/>;
		''' thus the position in the view (and, the array orders) must match those of the parameters
		''' </summary>
		''' <param name="conf">         Configuration </param>
		''' <param name="gradientView"> The flattened gradients array, as a view of the larger array </param>
		''' <returns> A map containing an array by parameter type, that is a view of the full network gradients array </returns>
		Function getGradientsFromFlattened(ByVal conf As NeuralNetConfiguration, ByVal gradientView As INDArray) As IDictionary(Of String, INDArray)

	End Interface

End Namespace