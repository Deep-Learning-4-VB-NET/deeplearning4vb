Imports System.Collections.Generic
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports Trainable = org.deeplearning4j.nn.api.Trainable
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr

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

Namespace org.deeplearning4j.nn.graph.vertex


	Public Interface GraphVertex
		Inherits Trainable

		''' <summary>
		''' Get the name/label of the GraphVertex
		''' </summary>
		ReadOnly Property VertexName As String

		''' <summary>
		''' Get the index of the GraphVertex </summary>
		ReadOnly Property VertexIndex As Integer

		''' <summary>
		''' Get the number of input arrays. For example, a Layer may have only one input array, but in general a GraphVertex
		''' may have an arbtrary (>=1) number of input arrays (for example, from multiple other layers)
		''' </summary>
		ReadOnly Property NumInputArrays As Integer

		''' <summary>
		''' Get the number of outgoing connections from this GraphVertex. A GraphVertex may only have a single output (for
		''' example, the activations out of a layer), but this output may be used as the input to an arbitrary number of other
		''' GraphVertex instances. This method returns the number of GraphVertex instances the output of this GraphVertex is input for.
		''' </summary>
		ReadOnly Property NumOutputConnections As Integer

		''' <summary>
		''' A representation of the vertices that are inputs to this vertex (inputs duing forward pass)<br>
		''' Specifically, if inputVertices[X].getVertexIndex() = Y, and inputVertices[X].getVertexEdgeNumber() = Z
		''' then the Zth output connection (see <seealso cref="getNumOutputConnections()"/> of vertex Y is the Xth input to this vertex
		''' </summary>
		Property InputVertices As VertexIndices()


		''' <summary>
		''' A representation of the vertices that this vertex is connected to (outputs duing forward pass)
		''' Specifically, if outputVertices[X].getVertexIndex() = Y, and outputVertices[X].getVertexEdgeNumber() = Z
		''' then the Xth output of this vertex is connected to the Zth input of vertex Y
		''' </summary>
		Property OutputVertices As VertexIndices()


		''' <summary>
		''' Whether the GraphVertex contains a <seealso cref="Layer"/> object or not </summary>
		Function hasLayer() As Boolean

		''' <summary>
		''' Whether the GraphVertex is an input vertex </summary>
		ReadOnly Property InputVertex As Boolean

		''' <summary>
		''' Whether the GraphVertex is an output vertex </summary>
		Property OutputVertex As Boolean


		''' <summary>
		''' Get the Layer (if any). Returns null if <seealso cref="hasLayer()"/> == false </summary>
		ReadOnly Property Layer As Layer

		''' <summary>
		''' Set the input activations. </summary>
		'''  <param name="inputNumber"> Must be in range 0 to <seealso cref="getNumInputArrays()"/>-1 </param>
		''' <param name="input"> The input array </param>
		''' <param name="workspaceMgr"> </param>
		Sub setInput(ByVal inputNumber As Integer, ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)

		''' <summary>
		''' Set the errors (epsilon - aka dL/dActivation) for this GraphVertex </summary>
		Property Epsilon As INDArray

		''' <summary>
		''' Clear the internal state (if any) of the GraphVertex. For example, any stored inputs/errors </summary>
		Sub clear()

		''' <summary>
		''' Whether the GraphVertex can do forward pass. Typically, this is just whether all inputs are set. </summary>
		Function canDoForward() As Boolean

		''' <summary>
		''' Whether the GraphVertex can do backward pass. Typically, this is just whether all errors/epsilons are set </summary>
		Function canDoBackward() As Boolean

		''' <summary>
		''' Do forward pass using the stored inputs </summary>
		''' <param name="training"> if true: forward pass at training time. If false: forward pass at test time </param>
		''' <returns> The output (for example, activations) of the GraphVertex </returns>
		Function doForward(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray

		''' <summary>
		''' Do backward pass </summary>
		''' <param name="tbptt"> If true: do backprop using truncated BPTT </param>
		''' <returns> The gradients (may be null), and the errors/epsilons for all inputs to this GraphVertex </returns>
		Function doBackward(ByVal tbptt As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray())

		''' <summary>
		''' Get the array of inputs previously set for this GraphVertex </summary>
		Property Inputs As INDArray()



		''' <summary>
		''' See <seealso cref="Layer.setBackpropGradientsViewArray(INDArray)"/> </summary>
		''' <param name="backpropGradientsViewArray"> </param>
		WriteOnly Property BackpropGradientsViewArray As INDArray

		Function feedForwardMaskArrays(ByVal maskArrays() As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)

		''' <summary>
		''' Only applies to layer vertices. Will throw exceptions on others.
		''' If applied to a layer vertex it will treat the parameters of the layer within it as constant.
		''' Activations through these will be calculated as they would as test time regardless of training mode
		''' </summary>
		Sub setLayerAsFrozen()

		''' <summary>
		''' This method clears inpjut for this vertex
		''' </summary>
		Sub clearVertex()

		''' <summary>
		''' Get the parameter table for the vertex </summary>
		''' <param name="backpropOnly"> If true: exclude unsupervised training parameters </param>
		''' <returns> Parameter table </returns>
		Function paramTable(ByVal backpropOnly As Boolean) As IDictionary(Of String, INDArray)
	End Interface

End Namespace