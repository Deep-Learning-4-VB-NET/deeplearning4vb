Imports System
Imports System.Collections.Generic
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports TrainingConfig = org.deeplearning4j.nn.api.TrainingConfig
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
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

Namespace org.deeplearning4j.nn.graph.vertex


	<Serializable>
	Public MustInherit Class BaseWrapperVertex
		Implements GraphVertex

		Protected Friend underlying As GraphVertex

		Protected Friend Sub New(ByVal underlying As GraphVertex)
			Me.underlying = underlying
		End Sub

		Public Overridable ReadOnly Property VertexName As String Implements GraphVertex.getVertexName
			Get
				Return underlying.VertexName
			End Get
		End Property

		Public Overridable ReadOnly Property VertexIndex As Integer Implements GraphVertex.getVertexIndex
			Get
				Return underlying.VertexIndex
			End Get
		End Property

		Public Overridable ReadOnly Property NumInputArrays As Integer Implements GraphVertex.getNumInputArrays
			Get
				Return underlying.NumInputArrays
			End Get
		End Property

		Public Overridable ReadOnly Property NumOutputConnections As Integer Implements GraphVertex.getNumOutputConnections
			Get
				Return underlying.NumOutputConnections
			End Get
		End Property

		Public Overridable Property InputVertices As VertexIndices() Implements GraphVertex.getInputVertices
			Get
				Return underlying.InputVertices
			End Get
			Set(ByVal inputVertices() As VertexIndices)
				underlying.InputVertices = inputVertices
			End Set
		End Property


		Public Overridable Property OutputVertices As VertexIndices() Implements GraphVertex.getOutputVertices
			Get
				Return underlying.OutputVertices
			End Get
			Set(ByVal outputVertices() As VertexIndices)
				underlying.OutputVertices = outputVertices
			End Set
		End Property


		Public Overridable Function hasLayer() As Boolean Implements GraphVertex.hasLayer
			Return underlying.hasLayer()
		End Function

		Public Overridable ReadOnly Property InputVertex As Boolean Implements GraphVertex.isInputVertex
			Get
				Return underlying.InputVertex
			End Get
		End Property

		Public Overridable Property OutputVertex As Boolean Implements GraphVertex.isOutputVertex
			Get
				Return underlying.OutputVertex
			End Get
			Set(ByVal outputVertex As Boolean)
				underlying.OutputVertex = outputVertex
			End Set
		End Property


		Public Overridable ReadOnly Property Layer As Layer Implements GraphVertex.getLayer
			Get
				Return underlying.Layer
			End Get
		End Property

		Public Overridable Sub setInput(ByVal inputNumber As Integer, ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) Implements GraphVertex.setInput
			underlying.setInput(inputNumber, input, workspaceMgr)
		End Sub

		Public Overridable Property Epsilon Implements GraphVertex.setEpsilon As INDArray
			Set(ByVal epsilon As INDArray)
				underlying.Epsilon = epsilon
			End Set
			Get
				Return underlying.Epsilon
			End Get
		End Property

		Public Overridable Sub clear() Implements GraphVertex.clear
			underlying.clear()
		End Sub

		Public Overridable Function canDoForward() As Boolean Implements GraphVertex.canDoForward
			Return underlying.canDoForward()
		End Function

		Public Overridable Function canDoBackward() As Boolean Implements GraphVertex.canDoBackward
			Return underlying.canDoBackward()
		End Function

		Public Overridable Function doForward(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements GraphVertex.doForward
			Return underlying.doForward(training, workspaceMgr)
		End Function

		Public Overridable Function doBackward(ByVal tbptt As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray()) Implements GraphVertex.doBackward
			Return underlying.doBackward(tbptt, workspaceMgr)
		End Function

		Public Overridable Property Inputs As INDArray() Implements GraphVertex.getInputs
			Get
				Return underlying.Inputs
			End Get
			Set(ByVal inputs() As INDArray)
				underlying.Inputs = inputs
			End Set
		End Property



		Public Overridable ReadOnly Property GradientsViewArray As INDArray
			Get
				Return underlying.GradientsViewArray
			End Get
		End Property

		Public Overridable WriteOnly Property BackpropGradientsViewArray Implements GraphVertex.setBackpropGradientsViewArray As INDArray
			Set(ByVal backpropGradientsViewArray As INDArray)
				underlying.BackpropGradientsViewArray = backpropGradientsViewArray
			End Set
		End Property

		Public Overridable Function feedForwardMaskArrays(ByVal maskArrays() As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState) Implements GraphVertex.feedForwardMaskArrays
			Return underlying.feedForwardMaskArrays(maskArrays, currentMaskState, minibatchSize)
		End Function

		Public Overridable Sub setLayerAsFrozen() Implements GraphVertex.setLayerAsFrozen
			underlying.setLayerAsFrozen()
		End Sub

		Public Overridable Sub clearVertex() Implements GraphVertex.clearVertex
			underlying.clearVertex()
		End Sub

		Public Overridable Function paramTable(ByVal backpropOnly As Boolean) As IDictionary(Of String, INDArray) Implements GraphVertex.paramTable
			Return underlying.paramTable(backpropOnly)
		End Function

		Public Overridable ReadOnly Property Config As TrainingConfig
			Get
				Return underlying.Config
			End Get
		End Property

		Public Overridable Function params() As INDArray
			Return underlying.params()
		End Function

		Public Overridable Function numParams() As Long
			Return underlying.numParams()
		End Function


		Public Overridable Function updaterDivideByMinibatch(ByVal paramName As String) As Boolean
			Return underlying.updaterDivideByMinibatch(paramName)
		End Function
	End Class

End Namespace