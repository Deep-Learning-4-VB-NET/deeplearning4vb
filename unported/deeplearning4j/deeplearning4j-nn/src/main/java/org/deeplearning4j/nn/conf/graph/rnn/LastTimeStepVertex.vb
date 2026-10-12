Imports System
Imports Data = lombok.Data
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
'ORIGINAL LINE: @Data public class LastTimeStepVertex extends org.deeplearning4j.nn.conf.graph.GraphVertex
	<Serializable>
	Public Class LastTimeStepVertex
		Inherits GraphVertex

		Private maskArrayInputName As String

		''' 
		''' <param name="maskArrayInputName"> The name of the input to look at when determining the last time step. Specifically, the
		'''                           mask array of this time series input is used when determining which time step to extract
		'''                           and return. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public LastTimeStepVertex(@JsonProperty("maskArrayInputName") String maskArrayInputName)
		Public Sub New(ByVal maskArrayInputName As String)
			Me.maskArrayInputName = maskArrayInputName
		End Sub

		Public Overrides Function clone() As GraphVertex
			Return New LastTimeStepVertex(maskArrayInputName)
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Not (TypeOf o Is LastTimeStepVertex) Then
				Return False
			End If
			Dim ltsv As LastTimeStepVertex = DirectCast(o, LastTimeStepVertex)
			If maskArrayInputName Is Nothing AndAlso ltsv.maskArrayInputName IsNot Nothing OrElse maskArrayInputName IsNot Nothing AndAlso ltsv.maskArrayInputName Is Nothing Then
				Return False
			End If
			Return maskArrayInputName Is Nothing OrElse maskArrayInputName.Equals(ltsv.maskArrayInputName)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return (If(maskArrayInputName Is Nothing, 452766971, maskArrayInputName.GetHashCode()))
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

		Public Overrides Function instantiate(ByVal graph As ComputationGraph, ByVal name As String, ByVal idx As Integer, ByVal paramsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDatatype As DataType) As org.deeplearning4j.nn.graph.vertex.impl.rnn.LastTimeStepVertex
			Return New org.deeplearning4j.nn.graph.vertex.impl.rnn.LastTimeStepVertex(graph, name, idx, maskArrayInputName, networkDatatype)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(int layerIndex, org.deeplearning4j.nn.conf.inputs.InputType... vertexInputs) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ParamArray ByVal vertexInputs() As InputType) As InputType
			If vertexInputs.Length <> 1 Then
				Throw New InvalidInputTypeException("Invalid input type: cannot get last time step of more than 1 input")
			End If
			If vertexInputs(0).getType() <> InputType.Type.RNN Then
				Throw New InvalidInputTypeException("Invalid input type: cannot get subset of non RNN input (got: " & vertexInputs(0) & ")")
			End If

			Return InputType.feedForward(DirectCast(vertexInputs(0), InputType.InputTypeRecurrent).getSize())
		End Function

		Public Overrides Function getMemoryReport(ParamArray ByVal inputTypes() As InputType) As MemoryReport
			'No additional working memory (beyond activations/epsilons)
			Return (New LayerMemoryReport.Builder(Nothing, GetType(LastTimeStepVertex), inputTypes(0), getOutputType(-1, inputTypes))).standardMemory(0, 0).workingMemory(0, 0, 0, 0).cacheMemory(0, 0).build()
		End Function

		Public Overrides Function ToString() As String
			Return "LastTimeStepVertex(inputName=" & maskArrayInputName & ")"
		End Function
	End Class

End Namespace