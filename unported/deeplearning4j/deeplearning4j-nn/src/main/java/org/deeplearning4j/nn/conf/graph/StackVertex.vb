Imports System
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InvalidInputTypeException = org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports Preconditions = org.nd4j.common.base.Preconditions
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
	Public Class StackVertex
		Inherits GraphVertex

		Public Sub New()
		End Sub

		Public Overrides Function clone() As StackVertex
			Return New StackVertex()
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			Return TypeOf o Is StackVertex
		End Function

		Public Overrides Function numParams(ByVal backprop As Boolean) As Long
			Return 0
		End Function

		Public Overrides Function minVertexInputs() As Integer
			Return 1
		End Function

		Public Overrides Function maxVertexInputs() As Integer
			Return Integer.MaxValue
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return 433682566
		End Function

		Public Overrides Function instantiate(ByVal graph As ComputationGraph, ByVal name As String, ByVal idx As Integer, ByVal paramsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDatatype As DataType) As org.deeplearning4j.nn.graph.vertex.GraphVertex
			Return New org.deeplearning4j.nn.graph.vertex.impl.StackVertex(graph, name, idx, networkDatatype)
		End Function

		Public Overrides Function ToString() As String
			Return "StackVertex()"
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(int layerIndex, org.deeplearning4j.nn.conf.inputs.InputType... vertexInputs) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ParamArray ByVal vertexInputs() As InputType) As InputType
			If vertexInputs.Length = 1 Then
				Return vertexInputs(0)
			End If
			Dim first As InputType = vertexInputs(0)

			'Check that types are all the same...
			For i As Integer = 1 To vertexInputs.Length - 1
				Preconditions.checkState(vertexInputs(i).getType() = first.getType(), "Different input types found:" & " input types must be the same. First type: %s, type %s: %s", first, i, vertexInputs(i))

				'Check that types are equal:
				Preconditions.checkState(first.Equals(vertexInputs(i)), "Input types must be equal: %s and %s", first, vertexInputs(i))
			Next i

			'Stacking on dimension 0 -> same output type as input type
			Return first
		End Function

		Public Overrides Function getMemoryReport(ParamArray ByVal inputTypes() As InputType) As MemoryReport
			'No working memory, just output activations
			Dim outputType As InputType = getOutputType(-1, inputTypes)

			Return (New LayerMemoryReport.Builder(Nothing, GetType(StackVertex), inputTypes(0), outputType)).standardMemory(0, 0).workingMemory(0, 0, 0, 0).cacheMemory(0, 0).build()
		End Function
	End Class

End Namespace