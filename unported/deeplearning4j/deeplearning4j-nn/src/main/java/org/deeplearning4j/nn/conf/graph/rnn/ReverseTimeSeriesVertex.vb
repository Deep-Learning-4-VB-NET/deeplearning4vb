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
'ORIGINAL LINE: @Data public class ReverseTimeSeriesVertex extends org.deeplearning4j.nn.conf.graph.GraphVertex
	<Serializable>
	Public Class ReverseTimeSeriesVertex
		Inherits GraphVertex

		Private ReadOnly maskArrayInputName As String

		''' <summary>
		''' Creates a new ReverseTimeSeriesVertex that doesn't pay attention to masks
		''' </summary>
		Public Sub New()
			Me.New(Nothing)
		End Sub

		''' <summary>
		''' Creates a new ReverseTimeSeriesVertex that uses the mask array of a given input </summary>
		''' <param name="maskArrayInputName"> The name of the input that holds the mask. </param>
		Public Sub New(ByVal maskArrayInputName As String)
			Me.maskArrayInputName = maskArrayInputName
		End Sub

		Public Overrides Function clone() As ReverseTimeSeriesVertex
			Return New ReverseTimeSeriesVertex(maskArrayInputName)
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Not (TypeOf o Is ReverseTimeSeriesVertex) Then
				Return False
			End If
			Dim rsgv As ReverseTimeSeriesVertex = DirectCast(o, ReverseTimeSeriesVertex)
			If maskArrayInputName Is Nothing AndAlso rsgv.maskArrayInputName IsNot Nothing OrElse maskArrayInputName IsNot Nothing AndAlso rsgv.maskArrayInputName Is Nothing Then
				Return False
			End If
			Return maskArrayInputName Is Nothing OrElse maskArrayInputName.Equals(rsgv.maskArrayInputName)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return If(maskArrayInputName IsNot Nothing, maskArrayInputName.GetHashCode(), 0)
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

		Public Overrides Function instantiate(ByVal graph As ComputationGraph, ByVal name As String, ByVal idx As Integer, ByVal paramsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDatatype As DataType) As org.deeplearning4j.nn.graph.vertex.impl.rnn.ReverseTimeSeriesVertex
			Return New org.deeplearning4j.nn.graph.vertex.impl.rnn.ReverseTimeSeriesVertex(graph, name, idx, maskArrayInputName, networkDatatype)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(int layerIndex, org.deeplearning4j.nn.conf.inputs.InputType... vertexInputs) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ParamArray ByVal vertexInputs() As InputType) As InputType
			If vertexInputs.Length <> 1 Then
				Throw New InvalidInputTypeException("Invalid input type: cannot revert more than 1 input")
			End If
			If vertexInputs(0).getType() <> InputType.Type.RNN Then
				Throw New InvalidInputTypeException("Invalid input type: cannot revert non RNN input (got: " & vertexInputs(0) & ")")
			End If

			Return vertexInputs(0)
		End Function

		Public Overrides Function getMemoryReport(ParamArray ByVal inputTypes() As InputType) As MemoryReport
			'No additional working memory (beyond activations/epsilons)
			Return (New LayerMemoryReport.Builder(Nothing, Me.GetType(), inputTypes(0), getOutputType(-1, inputTypes))).standardMemory(0, 0).workingMemory(0, 0, 0, 0).cacheMemory(0, 0).build()
		End Function

		Public Overrides Function ToString() As String
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String paramStr = (maskArrayInputName == null) ? "" : "inputName=" + maskArrayInputName;
			Dim paramStr As String = If(maskArrayInputName Is Nothing, "", "inputName=" & maskArrayInputName)
			Return "ReverseTimeSeriesVertex(" & paramStr & ")"
		End Function
	End Class

End Namespace