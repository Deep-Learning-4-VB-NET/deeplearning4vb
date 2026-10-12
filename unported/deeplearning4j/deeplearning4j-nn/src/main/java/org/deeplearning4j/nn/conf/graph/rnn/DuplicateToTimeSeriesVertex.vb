Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports GraphVertex = org.deeplearning4j.nn.conf.graph.GraphVertex
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

Namespace org.deeplearning4j.nn.conf.graph.rnn

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = false) public class DuplicateToTimeSeriesVertex extends org.deeplearning4j.nn.conf.graph.GraphVertex
	<Serializable>
	Public Class DuplicateToTimeSeriesVertex
		Inherits GraphVertex

		Private inputName As String

		''' <param name="inputName"> Name of the input in the ComputationGraph network to use, to determine how long the output time
		'''                  series should be. This input should (a) exist, and (b) be a time series input </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DuplicateToTimeSeriesVertex(@JsonProperty("inputName") String inputName)
		Public Sub New(ByVal inputName As String)
			Me.inputName = inputName
		End Sub

		Public Overrides Function clone() As GraphVertex
			Return New DuplicateToTimeSeriesVertex(inputName)
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Not (TypeOf o Is DuplicateToTimeSeriesVertex) Then
				Return False
			End If
			Dim d As DuplicateToTimeSeriesVertex = DirectCast(o, DuplicateToTimeSeriesVertex)
			If inputName Is Nothing AndAlso d.inputName IsNot Nothing OrElse inputName IsNot Nothing AndAlso d.inputName Is Nothing Then
				Return False
			End If
			Return inputName Is Nothing OrElse inputName.Equals(d.inputName)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return 534806565 Xor (If(inputName IsNot Nothing, inputName.GetHashCode(), 0))
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
			Return New org.deeplearning4j.nn.graph.vertex.impl.rnn.DuplicateToTimeSeriesVertex(graph, name, idx, inputName, networkDatatype)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(int layerIndex, org.deeplearning4j.nn.conf.inputs.InputType... vertexInputs) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ParamArray ByVal vertexInputs() As InputType) As InputType
			If vertexInputs.Length <> 1 Then
				Throw New InvalidInputTypeException("Invalid input type: cannot duplicate more than 1 input")
			End If

			Dim tsLength As Integer = 1 'TODO work this out properly

			If vertexInputs(0).getType() = InputType.Type.FF Then
				Return InputType.recurrent(DirectCast(vertexInputs(0), InputType.InputTypeFeedForward).getSize(), tsLength)
			ElseIf vertexInputs(0).getType() = InputType.Type.CNNFlat Then
				Return InputType.recurrent(DirectCast(vertexInputs(0), InputType.InputTypeConvolutionalFlat).FlattenedSize, tsLength)
			Else
				Throw New InvalidInputTypeException("Invalid input type: cannot duplicate to time series non feed forward (or CNN flat) input (got: " & vertexInputs(0) & ")")
			End If


		End Function

		Public Overrides Function getMemoryReport(ParamArray ByVal inputTypes() As InputType) As MemoryReport
			'No working memory in addition to output activations
			Return (New LayerMemoryReport.Builder(Nothing, GetType(DuplicateToTimeSeriesVertex), inputTypes(0), getOutputType(-1, inputTypes))).standardMemory(0, 0).workingMemory(0, 0, 0, 0).cacheMemory(0, 0).build()
		End Function
	End Class

End Namespace