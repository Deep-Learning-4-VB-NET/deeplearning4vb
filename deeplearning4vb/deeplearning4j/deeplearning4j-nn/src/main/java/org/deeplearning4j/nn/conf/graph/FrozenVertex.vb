Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InvalidInputTypeException = org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
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
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = false) public class FrozenVertex extends GraphVertex
	<Serializable>
	Public Class FrozenVertex
		Inherits GraphVertex

		Private underlying As GraphVertex

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FrozenVertex(@JsonProperty("underlying") GraphVertex underlying)
		Public Sub New(ByVal underlying As GraphVertex)
			Me.underlying = underlying
		End Sub

		Public Overrides Function clone() As GraphVertex
			Return New FrozenVertex(underlying.clone())
		End Function

		Public Overrides Function numParams(ByVal backprop As Boolean) As Long
			Return underlying.numParams(backprop)
		End Function

		Public Overrides Function minVertexInputs() As Integer
			Return underlying.minVertexInputs()
		End Function

		Public Overrides Function maxVertexInputs() As Integer
			Return underlying.maxVertexInputs()
		End Function

		Public Overrides Function instantiate(ByVal graph As ComputationGraph, ByVal name As String, ByVal idx As Integer, ByVal paramsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDatatype As DataType) As org.deeplearning4j.nn.graph.vertex.GraphVertex
			Dim u As org.deeplearning4j.nn.graph.vertex.GraphVertex = underlying.instantiate(graph, name, idx, paramsView, initializeParams, networkDatatype)
			Return New org.deeplearning4j.nn.graph.vertex.impl.FrozenVertex(u)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(int layerIndex, org.deeplearning4j.nn.conf.inputs.InputType... vertexInputs) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ParamArray ByVal vertexInputs() As InputType) As InputType
			Return underlying.getOutputType(layerIndex, vertexInputs)
		End Function

		Public Overrides Function getMemoryReport(ParamArray ByVal inputTypes() As InputType) As MemoryReport
			Return underlying.getMemoryReport(inputTypes)
		End Function
	End Class

End Namespace