Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports TrainingConfig = org.deeplearning4j.nn.api.TrainingConfig
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports LayerVertex = org.deeplearning4j.nn.graph.vertex.impl.LayerVertex
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public abstract class BaseGraphVertex implements GraphVertex
	<Serializable>
	Public MustInherit Class BaseGraphVertex
		Implements GraphVertex

		Public MustOverride Function feedForwardMaskArrays(ByVal maskArrays() As INDArray, ByVal currentMaskState As org.deeplearning4j.nn.api.MaskState, ByVal minibatchSize As Integer) As org.nd4j.common.primitives.Pair(Of INDArray, org.deeplearning4j.nn.api.MaskState)
		Public MustOverride WriteOnly Property BackpropGradientsViewArray Implements GraphVertex.setBackpropGradientsViewArray As INDArray
		Public MustOverride Property Inputs Implements GraphVertex.setInputs As INDArray()
		Public MustOverride Function doBackward(ByVal tbptt As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As org.nd4j.common.primitives.Pair(Of org.deeplearning4j.nn.gradient.Gradient, INDArray())
		Public MustOverride Function doForward(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
		Public MustOverride ReadOnly Property Layer As org.deeplearning4j.nn.api.Layer Implements GraphVertex.getLayer
		Public MustOverride WriteOnly Property OutputVertex As Boolean
		Public MustOverride ReadOnly Property OutputVertex As Boolean Implements GraphVertex.isOutputVertex
		Public MustOverride Function hasLayer() As Boolean Implements GraphVertex.hasLayer

		Protected Friend graph As ComputationGraph

'JAVA TO VB CONVERTER NOTE: The field vertexName was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend vertexName_Conflict As String

		''' <summary>
		''' The index of this vertex </summary>
'JAVA TO VB CONVERTER NOTE: The field vertexIndex was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend vertexIndex_Conflict As Integer

		''' <summary>
		'''A representation of the vertices that are inputs to this vertex (inputs during forward pass)
		''' Specifically, if inputVertices[X].getVertexIndex() = Y, and inputVertices[X].getVertexEdgeNumber() = Z
		''' then the Zth output of vertex Y is the Xth input to this vertex
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field inputVertices was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend inputVertices_Conflict() As VertexIndices

		''' <summary>
		'''A representation of the vertices that this vertex is connected to (outputs duing forward pass)
		''' Specifically, if outputVertices[X].getVertexIndex() = Y, and outputVertices[X].getVertexEdgeNumber() = Z
		''' then the output of this vertex (there is only one output) is connected to the Zth input of vertex Y
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field outputVertices was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend outputVertices_Conflict() As VertexIndices

		Protected Friend inputs() As INDArray
'JAVA TO VB CONVERTER NOTE: The field epsilon was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend epsilon_Conflict As INDArray

		'Set outputVertex to true when Layer is an OutputLayer, OR For use in specialized situations like reinforcement learning
		' For RL situations, this Layer insn't an OutputLayer, but is the last layer in a graph, that gets its error/epsilon
		' passed in externally
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter @Getter protected boolean outputVertex;
		Protected Friend outputVertex As Boolean

		Protected Friend dataType As DataType

		Protected Friend Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal inputVertices() As VertexIndices, ByVal outputVertices() As VertexIndices, ByVal dataType As DataType)
			Me.graph = graph
			Me.vertexName_Conflict = name
			Me.vertexIndex_Conflict = vertexIndex
			Me.inputVertices_Conflict = inputVertices
			Me.outputVertices_Conflict = outputVertices
			Me.dataType = dataType

			Me.inputs = New INDArray((If(inputVertices IsNot Nothing, inputVertices.Length, 0)) - 1){}
		End Sub

		Public Overridable ReadOnly Property VertexName As String Implements GraphVertex.getVertexName
			Get
				Return vertexName_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property VertexIndex As Integer Implements GraphVertex.getVertexIndex
			Get
				Return vertexIndex_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property NumInputArrays As Integer Implements GraphVertex.getNumInputArrays
			Get
				Return (If(inputVertices_Conflict Is Nothing, 0, inputVertices_Conflict.Length))
			End Get
		End Property

		Public Overridable ReadOnly Property NumOutputConnections As Integer Implements GraphVertex.getNumOutputConnections
			Get
				Return (If(outputVertices_Conflict Is Nothing, 0, outputVertices_Conflict.Length))
			End Get
		End Property

		''' <summary>
		'''A representation of the vertices that are inputs to this vertex (inputs duing forward pass)<br>
		''' Specifically, if inputVertices[X].getVertexIndex() = Y, and inputVertices[X].getVertexEdgeNumber() = Z
		''' then the Zth output of vertex Y is the Xth input to this vertex
		''' </summary>
		Public Overridable Property InputVertices As VertexIndices() Implements GraphVertex.getInputVertices
			Get
				Return inputVertices_Conflict
			End Get
			Set(ByVal inputVertices() As VertexIndices)
				Me.inputVertices_Conflict = inputVertices
				Me.inputs = New INDArray((If(inputVertices IsNot Nothing, inputVertices.Length, 0)) - 1){}
			End Set
		End Property


		''' <summary>
		'''A representation of the vertices that this vertex is connected to (outputs duing forward pass)
		''' Specifically, if outputVertices[X].getVertexIndex() = Y, and outputVertices[X].getVertexEdgeNumber() = Z
		''' then the Xth output of this vertex is connected to the Zth input of vertex Y
		''' </summary>
		Public Overridable Property OutputVertices As VertexIndices() Implements GraphVertex.getOutputVertices
			Get
				Return outputVertices_Conflict
			End Get
			Set(ByVal outputVertices() As VertexIndices)
				Me.outputVertices_Conflict = outputVertices
			End Set
		End Property


		Public Overridable ReadOnly Property InputVertex As Boolean Implements GraphVertex.isInputVertex
			Get
				Return False
			End Get
		End Property

		Public Overridable Sub setInput(ByVal inputNumber As Integer, ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) Implements GraphVertex.setInput
			If inputNumber >= NumInputArrays Then
				Throw New System.ArgumentException("Invalid input number")
			End If
			inputs(inputNumber) = input
		End Sub

		Public Overridable Property Epsilon Implements GraphVertex.setEpsilon As INDArray
			Set(ByVal epsilon As INDArray)
				Me.epsilon_Conflict = epsilon
			End Set
			Get
				Return epsilon_Conflict
			End Get
		End Property

		Public Overridable Sub clear() Implements GraphVertex.clear
			For i As Integer = 0 To inputs.Length - 1
				inputs(i) = Nothing
			Next i
			epsilon_Conflict = Nothing
			If Layer IsNot Nothing Then
				Layer.clear()
			End If
		End Sub

		Public Overridable Function canDoForward() As Boolean Implements GraphVertex.canDoForward
			For Each input As INDArray In inputs
				If input Is Nothing Then
					Return False
				End If
			Next input
			Return True
		End Function

		Public Overridable Function canDoBackward() As Boolean Implements GraphVertex.canDoBackward
			For Each input As INDArray In inputs
				If input Is Nothing Then
					Return False
				End If
			Next input
			Return epsilon_Conflict IsNot Nothing
		End Function


		Public MustOverride Overrides Function ToString() As String

		Public Overridable Sub setLayerAsFrozen() Implements GraphVertex.setLayerAsFrozen
			If Not (TypeOf Me Is LayerVertex) Then
				Throw New System.ArgumentException("Cannot set non layer vertices as frozen")
			End If
		End Sub

		Public Overridable Sub clearVertex() Implements GraphVertex.clearVertex
			clear()
		End Sub

		Public Overridable Function paramTable(ByVal backpropOnly As Boolean) As IDictionary(Of String, INDArray) Implements GraphVertex.paramTable
			Return Collections.emptyMap()
		End Function

		Public Overridable Function numParams() As Long
			Return If(params() Is Nothing, 0, params().length())
		End Function

		Public Overridable ReadOnly Property Config As TrainingConfig
			Get
				Return Nothing
			End Get
		End Property

		Public Overridable Function params() As INDArray
			Return Nothing
		End Function

		Public Overridable ReadOnly Property GradientsViewArray As INDArray
			Get
				Return Nothing
			End Get
		End Property

		Public Overridable Function updaterDivideByMinibatch(ByVal paramName As String) As Boolean
			If hasLayer() Then
				Return Layer.updaterDivideByMinibatch(paramName)
			End If
			Return True
		End Function
	End Class

End Namespace