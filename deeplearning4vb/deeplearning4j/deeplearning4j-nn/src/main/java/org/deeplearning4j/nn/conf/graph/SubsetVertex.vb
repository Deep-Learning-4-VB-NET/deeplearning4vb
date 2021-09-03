Imports System
Imports Data = lombok.Data
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
'ORIGINAL LINE: @Data public class SubsetVertex extends GraphVertex
	<Serializable>
	Public Class SubsetVertex
		Inherits GraphVertex

		Private from As Integer
		Private [to] As Integer

		''' <param name="from"> The first column index, inclusive </param>
		''' <param name="to">   The last column index, inclusive </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SubsetVertex(@JsonProperty("from") int from, @JsonProperty("to") int to)
		Public Sub New(ByVal from As Integer, ByVal [to] As Integer)
			Me.from = from
			Me.to = [to]
		End Sub

		Public Overrides Function clone() As SubsetVertex
			Return New SubsetVertex(from, [to])
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Not (TypeOf o Is SubsetVertex) Then
				Return False
			End If
			Dim s As SubsetVertex = DirectCast(o, SubsetVertex)
			Return s.from = from AndAlso s.to = [to]
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return (New Integer?(from)).GetHashCode() Xor (New Integer?([to])).GetHashCode()
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
			Return New org.deeplearning4j.nn.graph.vertex.impl.SubsetVertex(graph, name, idx, from, [to], networkDatatype)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(int layerIndex, org.deeplearning4j.nn.conf.inputs.InputType... vertexInputs) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ParamArray ByVal vertexInputs() As InputType) As InputType
			If vertexInputs.Length <> 1 Then
				Throw New InvalidInputTypeException("SubsetVertex expects single input type. Received: " & Arrays.toString(vertexInputs))
			End If

			Select Case vertexInputs(0).getType()
				Case FF
					Return InputType.feedForward([to] - from + 1)
				Case RNN
					Return InputType.recurrent([to] - from + 1)
				Case CNN
					Dim conv As InputType.InputTypeConvolutional = DirectCast(vertexInputs(0), InputType.InputTypeConvolutional)
					Dim depth As val = conv.getChannels()
					If [to] >= depth Then
						Throw New InvalidInputTypeException("Invalid range: Cannot select channels subset [" & from & "," & [to] & "] inclusive from CNN activations with " & " [channels,width,height] = [" & depth & "," & conv.getWidth() & "," & conv.getHeight() & "]")
					End If
					Return InputType.convolutional(conv.getHeight(), conv.getWidth(), from - [to] + 1)
				Case CNNFlat
					'TODO work out how to do this - could be difficult...
					Throw New System.NotSupportedException("Subsetting data in flattened convolutional format not yet supported")
				Case Else
					Throw New Exception("Unknown input type: " & vertexInputs(0))
			End Select
		End Function

		Public Overrides Function getMemoryReport(ParamArray ByVal inputTypes() As InputType) As MemoryReport
			'Get op without dup - no additional memory use
			Dim outputType As InputType = getOutputType(-1, inputTypes)
			Return (New LayerMemoryReport.Builder(Nothing, GetType(SubsetVertex), inputTypes(0), outputType)).standardMemory(0, 0).workingMemory(0, 0, 0, 0).cacheMemory(0, 0).build()
		End Function
	End Class

End Namespace