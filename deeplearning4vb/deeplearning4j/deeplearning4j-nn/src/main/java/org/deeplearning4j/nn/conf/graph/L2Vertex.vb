Imports System
Imports val = lombok.val
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InvalidInputTypeException = org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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

Namespace org.deeplearning4j.nn.conf.graph


	<Serializable>
	Public Class L2Vertex
		Inherits GraphVertex

		Protected Friend eps As Double

		''' <summary>
		''' Constructor with default epsilon value of 1e-8
		''' </summary>
		Public Sub New()
			Me.eps = 1e-8
		End Sub

		''' <param name="eps"> Epsilon value to add to inputs (to avoid all zeros input and hence undefined gradients) </param>
		Public Sub New(ByVal eps As Double)
			Me.eps = eps
		End Sub

		Public Overrides Function clone() As L2Vertex
			Return New L2Vertex()
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			Return TypeOf o Is L2Vertex
		End Function

		Public Overrides Function numParams(ByVal backprop As Boolean) As Long
			Return 0
		End Function

		Public Overrides Function minVertexInputs() As Integer
			Return 2
		End Function

		Public Overrides Function maxVertexInputs() As Integer
			Return 2
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return 433682566
		End Function

		Public Overrides Function instantiate(ByVal graph As ComputationGraph, ByVal name As String, ByVal idx As Integer, ByVal paramsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDatatype As DataType) As org.deeplearning4j.nn.graph.vertex.GraphVertex
			Return New org.deeplearning4j.nn.graph.vertex.impl.L2Vertex(graph, name, idx, Nothing, Nothing, eps, networkDatatype)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(int layerIndex, org.deeplearning4j.nn.conf.inputs.InputType... vertexInputs) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ParamArray ByVal vertexInputs() As InputType) As InputType
			Return InputType.feedForward(1)
		End Function

		Public Overrides Function getMemoryReport(ParamArray ByVal inputTypes() As InputType) As MemoryReport
			Dim outputType As InputType = getOutputType(-1, inputTypes)

			'Inference: only calculation is for output activations; no working memory
			'Working memory for training:
			'1 for each example (fwd pass) + output size (1 per ex) + input size + output size... in addition to the returned eps arrays
			'output size == input size here
			Dim trainWorkingSizePerEx As val = 3 + 2 * inputTypes(0).arrayElementsPerExample()

			Return (New LayerMemoryReport.Builder(Nothing, GetType(L2Vertex), inputTypes(0), outputType)).standardMemory(0, 0).workingMemory(0, 0, 0, trainWorkingSizePerEx).cacheMemory(0, 0).build()
		End Function
	End Class

End Namespace