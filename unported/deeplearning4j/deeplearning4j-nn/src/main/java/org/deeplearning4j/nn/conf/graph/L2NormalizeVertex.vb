Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports val = lombok.val
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InvalidInputTypeException = org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
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
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = false) public class L2NormalizeVertex extends GraphVertex
	<Serializable>
	Public Class L2NormalizeVertex
		Inherits GraphVertex

		Public Const DEFAULT_EPS As Double = 1e-8

		Protected Friend dimension() As Integer
		Protected Friend eps As Double

		Public Sub New()
			Me.New(Nothing, DEFAULT_EPS)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public L2NormalizeVertex(@JsonProperty("dimension") int[] dimension, @JsonProperty("eps") double eps)
		Public Sub New(ByVal dimension() As Integer, ByVal eps As Double)
			Me.dimension = dimension
			Me.eps = eps
		End Sub



		Public Overrides Function clone() As L2NormalizeVertex
			Return New L2NormalizeVertex(dimension, eps)
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

			Return New org.deeplearning4j.nn.graph.vertex.impl.L2NormalizeVertex(graph, name, idx, dimension, eps, networkDatatype)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(int layerIndex, org.deeplearning4j.nn.conf.inputs.InputType... vertexInputs) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ParamArray ByVal vertexInputs() As InputType) As InputType
			If vertexInputs.Length = 1 Then
				Return vertexInputs(0)
			End If
			Dim first As InputType = vertexInputs(0)

			Return first 'Same output shape/size as
		End Function

		Public Overrides Function getMemoryReport(ParamArray ByVal inputTypes() As InputType) As MemoryReport
			Dim outputType As InputType = getOutputType(-1, inputTypes)
			'norm2 value (inference working mem): 1 per example during forward pass

			'Training working mem: 2 per example + 2x input size + 1 per example (in addition to epsilons)
			Dim trainModePerEx As val = 3 + 2 * inputTypes(0).arrayElementsPerExample()

			Return (New LayerMemoryReport.Builder(Nothing, GetType(L2NormalizeVertex), inputTypes(0), outputType)).standardMemory(0, 0).workingMemory(0, 1, 0, trainModePerEx).cacheMemory(0, 0).build()
		End Function
	End Class

End Namespace