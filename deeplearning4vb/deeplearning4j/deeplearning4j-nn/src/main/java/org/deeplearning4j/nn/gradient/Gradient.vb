Imports System.Collections.Generic
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

Namespace org.deeplearning4j.nn.gradient


	Public Interface Gradient

		''' <summary>
		''' Gradient look up table
		''' </summary>
		''' <returns> the gradient look up table </returns>
		Function gradientForVariable() As IDictionary(Of String, INDArray)

		''' <summary>
		''' The full gradient as one flat vector
		''' 
		''' @return
		''' </summary>
		Function gradient(ByVal order As IList(Of String)) As INDArray

		''' <summary>
		''' The full gradient as one flat vector
		''' 
		''' @return
		''' </summary>
		Function gradient() As INDArray

		''' <summary>
		''' Clear residual parameters (useful for returning a gradient and then clearing old objects)
		''' </summary>
		Sub clear()

		''' <summary>
		''' The gradient for the given variable
		''' </summary>
		''' <param name="variable"> the variable to get the gradient for </param>
		''' <returns> the gradient for the given variable or null </returns>
		Function getGradientFor(ByVal variable As String) As INDArray

		''' <summary>
		''' Update gradient for the given variable
		''' </summary>
		''' <param name="variable"> the variable to get the gradient for </param>
		''' <param name="gradient"> the gradient values </param>
		''' <returns> the gradient for the given variable or null </returns>
		Function setGradientFor(ByVal variable As String, ByVal gradient As INDArray) As INDArray

		''' <summary>
		''' Update gradient for the given variable; also (optionally) specify the order in which the array should be flattened
		''' to a row vector
		''' </summary>
		''' <param name="variable">        the variable to get the gradient for </param>
		''' <param name="gradient">        the gradient values </param>
		''' <param name="flatteningOrder"> the order in which gradients should be flattened (null ok - default) </param>
		''' <returns> the gradient for the given variable or null </returns>
		Function setGradientFor(ByVal variable As String, ByVal gradient As INDArray, ByVal flatteningOrder As Char?) As INDArray

		''' <summary>
		''' Return the gradient flattening order for the specified variable, or null if it is not explicitly set </summary>
		''' <param name="variable">    Variable to return the gradient flattening order for </param>
		''' <returns>            Order in which the specified variable's gradient should be flattened </returns>
		Function flatteningOrderForVariable(ByVal variable As String) As Char?

	End Interface

End Namespace