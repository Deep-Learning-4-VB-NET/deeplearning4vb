Imports System
Imports System.Linq
Imports Data = lombok.Data
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InvalidInputTypeException = org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.deeplearning4j.nn.conf.graph


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class ReshapeVertex extends GraphVertex
	<Serializable>
	Public Class ReshapeVertex
		Inherits GraphVertex

		Public Const DEFAULT_RESHAPE_ORDER As Char = "c"c

		Protected Friend reshapeOrder As Char = "c"c
		Protected Friend newShape() As Integer
		Protected Friend maskShape() As Integer

		''' <summary>
		''' Reshape with the default reshape order of 'c' </summary>
		''' <param name="newShape"> New shape for activations </param>
		Public Sub New(ParamArray ByVal newShape() As Integer)
			Me.New(DEFAULT_RESHAPE_ORDER, newShape, Nothing)
		End Sub

		''' <param name="reshapeOrder"> Order (must be 'c' or 'f') for the activations </param>
		''' <param name="newShape">     New shape </param>
		''' <param name="maskShape">    Mask shape </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ReshapeVertex(@JsonProperty("reshapeOrder") char reshapeOrder, @JsonProperty("newShape") int[] newShape, @JsonProperty("maskShape") int[] maskShape)
		Public Sub New(ByVal reshapeOrder As Char, ByVal newShape() As Integer, ByVal maskShape() As Integer)
			Preconditions.checkState(reshapeOrder = "c"c OrElse reshapeOrder = "f"c, "Reshape order must be 'c' or 'f'. Got: '%s'", reshapeOrder.ToString())
			Me.reshapeOrder = reshapeOrder
			Me.newShape = newShape
			Me.maskShape = maskShape
		End Sub

		Public Overrides Function clone() As ReshapeVertex
			Return New ReshapeVertex(newShape)
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Not (TypeOf o Is ReshapeVertex) Then
				Return False
			End If
			Return DirectCast(o, ReshapeVertex).newShape.SequenceEqual(newShape)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return AscW(reshapeOrder) Xor Arrays.hashCode(newShape)
		End Function

		Public Overrides Function numParams(ByVal backprop As Boolean) As Long
			Return 0
		End Function

		Public Overrides Function minVertexInputs() As Integer
			Return 1
		End Function

		Public Overrides Function maxVertexInputs() As Integer
			Return 1
		End Function

		Public Overrides Function instantiate(ByVal graph As ComputationGraph, ByVal name As String, ByVal idx As Integer, ByVal paramsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDatatype As DataType) As org.deeplearning4j.nn.graph.vertex.GraphVertex
			Return New org.deeplearning4j.nn.graph.vertex.impl.ReshapeVertex(graph, name, idx, reshapeOrder, newShape, maskShape, networkDatatype)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(int layerIndex, org.deeplearning4j.nn.conf.inputs.InputType... vertexInputs) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ParamArray ByVal vertexInputs() As InputType) As InputType
			'Infer output shape from specified shape:
			Select Case newShape.Length
				Case 2
					Return InputType.feedForward(newShape(1))
				Case 3
					Return InputType.recurrent(newShape(1))
				Case 4
					Return InputType.convolutional(newShape(2), newShape(3), newShape(1)) '[mb,d,h,w] for activations
				Case Else
					Throw New System.NotSupportedException("Cannot infer input type for reshape array " & Arrays.toString(newShape))
			End Select
		End Function

		Public Overrides Function getMemoryReport(ParamArray ByVal inputTypes() As InputType) As MemoryReport
			'Assume it's a reshape-with-copy op. In this case: memory use is accounted for in activations
			Dim outputType As InputType = getOutputType(-1, inputTypes)
			Return (New LayerMemoryReport.Builder(Nothing, GetType(ReshapeVertex), inputTypes(0), outputType)).standardMemory(0, 0).workingMemory(0, 0, 0, 0).cacheMemory(0, 0).build()
		End Function
	End Class

End Namespace