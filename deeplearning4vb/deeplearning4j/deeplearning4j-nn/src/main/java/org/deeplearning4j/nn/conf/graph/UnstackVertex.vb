Imports System
Imports Getter = lombok.Getter
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
'ORIGINAL LINE: @Getter public class UnstackVertex extends GraphVertex
	<Serializable>
	Public Class UnstackVertex
		Inherits GraphVertex

		Protected Friend from As Integer
		Protected Friend stackSize As Integer

		''' <param name="from"> The first column index of the stacked inputs. </param>
		''' <param name="stackSize"> The total number of stacked inputs. An interval is automatically
		'''                  calculated according to the size of the first dimension. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public UnstackVertex(@JsonProperty("from") int from, @JsonProperty("stackSize") int stackSize)
		Public Sub New(ByVal from As Integer, ByVal stackSize As Integer)
			Me.from = from
			Me.stackSize = stackSize
		End Sub

		Public Overrides Function instantiate(ByVal graph As ComputationGraph, ByVal name As String, ByVal idx As Integer, ByVal paramsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDatatype As DataType) As org.deeplearning4j.nn.graph.vertex.GraphVertex
			Return New org.deeplearning4j.nn.graph.vertex.impl.UnstackVertex(graph, name, idx, Nothing, Nothing, from, stackSize, networkDatatype)
		End Function

		Public Overrides Function clone() As UnstackVertex
			Return New UnstackVertex(from, stackSize)
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Not (TypeOf o Is UnstackVertex) Then
				Return False
			End If
			Return DirectCast(o, UnstackVertex).from = from AndAlso DirectCast(o, UnstackVertex).stackSize = stackSize
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

		Public Overrides Function GetHashCode() As Integer
			Return 433682566
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(int layerIndex, org.deeplearning4j.nn.conf.inputs.InputType... vertexInputs) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ParamArray ByVal vertexInputs() As InputType) As InputType
			If vertexInputs.Length = 1 Then
				Return vertexInputs(0)
			End If
			Dim first As InputType = vertexInputs(0)
			If first.getType() = InputType.Type.CNNFlat Then
				'TODO
				'Merging flattened CNN format data could be messy?
				Throw New InvalidInputTypeException("Invalid input: UnstackVertex cannot currently merge CNN data in flattened format. Got: " & vertexInputs)
			ElseIf first.getType() <> InputType.Type.CNN Then
				'FF or RNN data inputs
				Dim size As Integer = 0
				Dim type As InputType.Type = Nothing
				For i As Integer = 0 To vertexInputs.Length - 1
					If vertexInputs(i).getType() <> first.getType() Then
						Throw New InvalidInputTypeException("Invalid input: UnstackVertex cannot merge activations of different types:" & " first type = " & first.getType() & ", input type " & (i + 1) & " = " & vertexInputs(i).getType())
					End If

					Dim thisSize As Long
					Select Case vertexInputs(i).getType()
						Case FF
							thisSize = DirectCast(vertexInputs(i), InputType.InputTypeFeedForward).getSize()
							type = InputType.Type.FF
						Case RNN
							thisSize = DirectCast(vertexInputs(i), InputType.InputTypeRecurrent).getSize()
							type = InputType.Type.RNN
						Case Else
							Throw New System.InvalidOperationException("Unknown input type: " & vertexInputs(i)) 'Should never happen
					End Select
					If thisSize <= 0 Then 'Size is not defined
						size = -1
					Else
						size += thisSize
					End If
				Next i

				If size > 0 Then
					'Size is specified
					If type = InputType.Type.FF Then
						Return InputType.feedForward(size)
					Else
						Return InputType.recurrent(size)
					End If
				Else
					'size is unknown
					If type = InputType.Type.FF Then
						Return InputType.feedForward(-1)
					Else
						Return InputType.recurrent(-1)
					End If
				End If
			Else
				'CNN inputs... also check that the channels, width and heights match:
				Dim firstConv As InputType.InputTypeConvolutional = DirectCast(first, InputType.InputTypeConvolutional)

				Dim fd As val = firstConv.getChannels()
				Dim fw As val = firstConv.getWidth()
				Dim fh As val = firstConv.getHeight()

				Dim depthSum As Long = fd

				For i As Integer = 1 To vertexInputs.Length - 1
					If vertexInputs(i).getType() <> InputType.Type.CNN Then
						Throw New InvalidInputTypeException("Invalid input: UnstackVertex cannot process activations of different types:" & " first type = " & InputType.Type.CNN & ", input type " & (i + 1) & " = " & vertexInputs(i).getType())
					End If

					Dim otherConv As InputType.InputTypeConvolutional = DirectCast(vertexInputs(i), InputType.InputTypeConvolutional)

					Dim od As val = otherConv.getChannels()
					Dim ow As val = otherConv.getWidth()
					Dim oh As val = otherConv.getHeight()

					If fw <> ow OrElse fh <> oh Then
						Throw New InvalidInputTypeException("Invalid input: UnstackVertex cannot merge CNN activations of different width/heights:" & "first [channels,width,height] = [" & fd & "," & fw & "," & fh & "], input " & i & " = [" & od & "," & ow & "," & oh & "]")
					End If

					depthSum += od
				Next i

				Return InputType.convolutional(fh, fw, depthSum)
			End If
		End Function

		Public Overrides Function getMemoryReport(ParamArray ByVal inputTypes() As InputType) As MemoryReport
			'Get op with dup - accounted for in activations size (no working memory)
			'Do one dup on the forward pass (output activations). Accounted for in output activations.
			Dim outputType As InputType = getOutputType(-1, inputTypes)
			Return (New LayerMemoryReport.Builder(Nothing, GetType(UnstackVertex), inputTypes(0), outputType)).standardMemory(0, 0).workingMemory(0, 0, 0, 0).cacheMemory(0, 0).build()
		End Function
	End Class

End Namespace