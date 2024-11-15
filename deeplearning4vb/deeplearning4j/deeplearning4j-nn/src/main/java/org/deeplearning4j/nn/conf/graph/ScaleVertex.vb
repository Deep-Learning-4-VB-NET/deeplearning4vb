﻿Imports System
Imports Data = lombok.Data
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
'ORIGINAL LINE: @Data public class ScaleVertex extends GraphVertex
	<Serializable>
	Public Class ScaleVertex
		Inherits GraphVertex

		''' <param name="scaleFactor"> The scaling factor to multiply input activations by </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ScaleVertex(@JsonProperty("scaleFactor") double scaleFactor)
		Public Sub New(ByVal scaleFactor As Double)
			Me.scaleFactor = scaleFactor
		End Sub

		Protected Friend scaleFactor As Double

		Public Overrides Function clone() As ScaleVertex
			Return New ScaleVertex(scaleFactor)
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Not (TypeOf o Is ScaleVertex) Then
				Return False
			End If
			Return DirectCast(o, ScaleVertex).scaleFactor = scaleFactor
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return 123073088
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

			Return New org.deeplearning4j.nn.graph.vertex.impl.ScaleVertex(graph, name, idx, scaleFactor, networkDatatype)
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
			'Do one dup on the forward pass (output activations). Accounted for in output activations.
			Dim outputType As InputType = getOutputType(-1, inputTypes)
			Return (New LayerMemoryReport.Builder(Nothing, GetType(ScaleVertex), inputTypes(0), outputType)).standardMemory(0, 0).workingMemory(0, 0, 0, 0).cacheMemory(0, 0).build()
		End Function
	End Class

End Namespace