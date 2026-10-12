Imports System
Imports Data = lombok.Data
Imports val = lombok.val
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InvalidInputTypeException = org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
Imports InputTypeUtil = org.deeplearning4j.nn.conf.layers.InputTypeUtil
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
'ORIGINAL LINE: @Data public class ElementWiseVertex extends GraphVertex
	<Serializable>
	Public Class ElementWiseVertex
		Inherits GraphVertex

		''' <param name="op"> The operation to perform on the inputs </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ElementWiseVertex(@JsonProperty("op") Op op)
		Public Sub New(ByVal op As Op)
			Me.op = op
		End Sub

		Public Enum Op
			Add
			Subtract
			Product
			Average
			Max
		End Enum

		Protected Friend op As Op

		Public Overrides Function clone() As ElementWiseVertex
			Return New ElementWiseVertex(op)
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Not (TypeOf o Is ElementWiseVertex) Then
				Return False
			End If
			Return DirectCast(o, ElementWiseVertex).op = op
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return op.GetHashCode()
		End Function

		Public Overrides Function numParams(ByVal backprop As Boolean) As Long
			Return 0
		End Function

		Public Overrides Function minVertexInputs() As Integer
			Return 2
		End Function

		Public Overrides Function maxVertexInputs() As Integer
			Select Case op
				Case org.deeplearning4j.nn.conf.graph.ElementWiseVertex.Op.Add, Average, Product, Max
					'No upper bound
					Return Integer.MaxValue
				Case org.deeplearning4j.nn.conf.graph.ElementWiseVertex.Op.Subtract
					Return 2
				Case Else
					Throw New System.NotSupportedException("Unknown op: " & op)
			End Select
		End Function

		Public Overrides Function instantiate(ByVal graph As ComputationGraph, ByVal name As String, ByVal idx As Integer, ByVal paramsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDatatype As DataType) As org.deeplearning4j.nn.graph.vertex.GraphVertex
			Dim op As org.deeplearning4j.nn.graph.vertex.impl.ElementWiseVertex.Op
			Select Case Me.op
				Case org.deeplearning4j.nn.conf.graph.ElementWiseVertex.Op.Add
					op = org.deeplearning4j.nn.graph.vertex.impl.ElementWiseVertex.Op.Add
				Case org.deeplearning4j.nn.conf.graph.ElementWiseVertex.Op.Average
					op = org.deeplearning4j.nn.graph.vertex.impl.ElementWiseVertex.Op.Average
				Case org.deeplearning4j.nn.conf.graph.ElementWiseVertex.Op.Subtract
					op = org.deeplearning4j.nn.graph.vertex.impl.ElementWiseVertex.Op.Subtract
				Case org.deeplearning4j.nn.conf.graph.ElementWiseVertex.Op.Product
					op = org.deeplearning4j.nn.graph.vertex.impl.ElementWiseVertex.Op.Product
				Case org.deeplearning4j.nn.conf.graph.ElementWiseVertex.Op.Max
					op = org.deeplearning4j.nn.graph.vertex.impl.ElementWiseVertex.Op.Max
				Case Else
					Throw New Exception()
			End Select
			Return New org.deeplearning4j.nn.graph.vertex.impl.ElementWiseVertex(graph, name, idx, op, networkDatatype)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(int layerIndex, org.deeplearning4j.nn.conf.inputs.InputType... vertexInputs) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ParamArray ByVal vertexInputs() As InputType) As InputType
			If vertexInputs.Length = 1 Then
				Return vertexInputs(0)
			End If
			InputTypeUtil.convertMultipleTypes(vertexInputs)

			Dim first As InputType = vertexInputs(0)
			If first.getType() <> InputType.Type.CNN Then
				'FF, RNN or flat CNN data inputs
				For i As Integer = 1 To vertexInputs.Length - 1
					If vertexInputs(i).getType() <> first.getType() Then
						Throw New InvalidInputTypeException("Invalid input: ElementWise vertex cannot process activations of different types:" & " first type = " & first.getType() & ", input type " & (i + 1) & " = " & vertexInputs(i).getType())
					End If
				Next i
			Else
				'CNN inputs... also check that the channels, width and heights match:
				Dim firstConv As InputType.InputTypeConvolutional = DirectCast(first, InputType.InputTypeConvolutional)

				Dim fd As val = firstConv.getChannels()
				Dim fw As val = firstConv.getWidth()
				Dim fh As val = firstConv.getHeight()

				For i As Integer = 1 To vertexInputs.Length - 1
					If vertexInputs(i).getType() <> InputType.Type.CNN Then
						Throw New InvalidInputTypeException("Invalid input: ElementWise vertex cannot process activations of different types:" & " first type = " & InputType.Type.CNN & ", input type " & (i + 1) & " = " & vertexInputs(i).getType())
					End If

					Dim otherConv As InputType.InputTypeConvolutional = DirectCast(vertexInputs(i), InputType.InputTypeConvolutional)

					Dim od As val = otherConv.getChannels()
					Dim ow As val = otherConv.getWidth()
					Dim oh As val = otherConv.getHeight()

					If fd <> od OrElse fw <> ow OrElse fh <> oh Then
						Throw New InvalidInputTypeException("Invalid input: ElementWise vertex cannot process CNN activations of different sizes:" & "first [channels,width,height] = [" & fd & "," & fw & "," & fh & "], input " & i & " = [" & od & "," & ow & "," & oh & "]")
					End If
				Next i
			End If

			If vertexInputs.Length < 2 Then
				Return vertexInputs(0)
			End If

			If first.getType() = InputType.Type.FF Then
				'could be 1s and a higher value. broadcast to the higher value where possible
				Dim maxInputType As InputType.InputTypeFeedForward = Nothing
				For i As Integer = 0 To vertexInputs.Length - 1
					Dim feedForward As InputType.InputTypeFeedForward = DirectCast(vertexInputs(i), InputType.InputTypeFeedForward)
					If maxInputType Is Nothing Then
						maxInputType = feedForward
					Else
						If maxInputType.getSize() < feedForward.getSize() Then
							maxInputType = feedForward
						End If
					End If
				Next i

				Return maxInputType
			ElseIf first.getType() = InputType.Type.CNNFlat Then
				'could be 1s and a higher value. broadcast to the higher value where possible
				Dim maxInputType As InputType.InputTypeConvolutionalFlat = Nothing
				For i As Integer = 0 To vertexInputs.Length - 1
					Dim feedForward As InputType.InputTypeConvolutionalFlat = DirectCast(vertexInputs(i), InputType.InputTypeConvolutionalFlat)
					If maxInputType Is Nothing Then
						maxInputType = feedForward
					Else
						If maxInputType.FlattenedSize < feedForward.FlattenedSize Then
							maxInputType = feedForward
						End If
					End If
				Next i

				Return maxInputType
			ElseIf first.getType() = InputType.Type.RNN Then
				'could be 1s and a higher value. broadcast to the higher value where possible
				Dim maxInputType As InputType.InputTypeRecurrent = Nothing
				For i As Integer = 0 To vertexInputs.Length - 1
					Dim feedForward As InputType.InputTypeRecurrent = DirectCast(vertexInputs(i), InputType.InputTypeRecurrent)
					If maxInputType Is Nothing Then
						maxInputType = feedForward
					Else
						If maxInputType.getTimeSeriesLength() < feedForward.getTimeSeriesLength() Then
							maxInputType = feedForward
						End If
					End If
				Next i

				Return maxInputType
			End If


			Return first 'Same output shape/size as
		End Function

		Public Overrides Function getMemoryReport(ParamArray ByVal inputTypes() As InputType) As MemoryReport
			'No working memory in addition to output activations
			Return (New LayerMemoryReport.Builder(Nothing, GetType(ElementWiseVertex), inputTypes(0), inputTypes(0))).standardMemory(0, 0).workingMemory(0, 0, 0, 0).cacheMemory(0, 0).build()
		End Function
	End Class

End Namespace