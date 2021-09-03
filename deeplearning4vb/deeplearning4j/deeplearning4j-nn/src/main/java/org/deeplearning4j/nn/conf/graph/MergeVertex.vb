Imports System
Imports Data = lombok.Data
Imports Setter = lombok.Setter
Imports val = lombok.val
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InvalidInputTypeException = org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
Imports Convolution3D = org.deeplearning4j.nn.conf.layers.Convolution3D
Imports InputTypeUtil = org.deeplearning4j.nn.conf.layers.InputTypeUtil
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class MergeVertex extends GraphVertex
	<Serializable>
	Public Class MergeVertex
		Inherits GraphVertex

'JAVA TO VB CONVERTER NOTE: The field mergeAxis was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend mergeAxis_Conflict As Integer = DEFAULT_MERGE_DIM 'default value for backward compatibility (deserialization of old version JSON) - NCHW and NCW format
		Protected Friend modified As Boolean = False

		Public Const DEFAULT_MERGE_DIM As Integer = 1

		Public Overrides Function clone() As MergeVertex
			Return New MergeVertex()
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			Return TypeOf o Is MergeVertex
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return 433682566
		End Function

		Public Overrides Function numParams(ByVal backprop As Boolean) As Long
			Return 0
		End Function

		Public Overrides Function minVertexInputs() As Integer
			Return 2
		End Function

		Public Overrides Function maxVertexInputs() As Integer
			Return Integer.MaxValue
		End Function

		Public Overrides Function ToString() As String
			Return "MergeVertex()"
		End Function

		Public Overrides Function instantiate(ByVal graph As ComputationGraph, ByVal name As String, ByVal idx As Integer, ByVal paramsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDatatype As DataType) As org.deeplearning4j.nn.graph.vertex.GraphVertex
			Return New org.deeplearning4j.nn.graph.vertex.impl.MergeVertex(graph, name, idx, networkDatatype, mergeAxis_Conflict)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(int layerIndex, org.deeplearning4j.nn.conf.inputs.InputType... vertexInputs) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ParamArray ByVal vertexInputs() As InputType) As InputType
			If vertexInputs.Length = 1 Then
				Return vertexInputs(0)
			End If

			InputTypeUtil.convertMultipleTypes(vertexInputs)


			Dim first As InputType = vertexInputs(0)
			If first.getType() = InputType.Type.CNNFlat Then
				'TODO
				'Merging flattened CNN format data could be messy?
				Throw New InvalidInputTypeException("Invalid input: MergeVertex cannot currently merge CNN data in flattened format. Got: " & vertexInputs)
			ElseIf first.getType() = InputType.Type.CNN3D Then
				' CNN3D inputs: check that the channels, width and height match:
				Dim firstConv As InputType.InputTypeConvolutional3D = DirectCast(first, InputType.InputTypeConvolutional3D)

				Dim fd As val = firstConv.getDepth()
				Dim fw As val = firstConv.getWidth()
				Dim fh As val = firstConv.getHeight()
				Dim fc As val = firstConv.getChannels()

				Dim depthSum As Long = fc
				Dim otherConv As InputType.InputTypeConvolutional3D = Nothing
				For i As Integer = 1 To vertexInputs.Length - 1
					If vertexInputs(i).getType() <> InputType.Type.CNN3D Then
						Throw New InvalidInputTypeException("Invalid input: MergeVertex cannot process activations of different types:" & " first type = " & InputType.Type.CNN3D & ", input type " & (i + 1) & " = " & vertexInputs(i).getType())
					End If

					otherConv = DirectCast(vertexInputs(i), InputType.InputTypeConvolutional3D)
					Dim od As val = otherConv.getDepth()
					Dim ow As val = otherConv.getWidth()
					Dim oh As val = otherConv.getHeight()
					Dim oc As val = otherConv.getChannels()

					If fd <> od OrElse fw <> ow OrElse fh <> oh Then
						Throw New InvalidInputTypeException("Invalid input: MergeVertex cannot merge CNN3D activations of different width/heights:" & "first [channels,width,height] = [" & fd & "," & fw & "," & fh & "], input " & i & " = [" & od & "," & ow & "," & oh & "]")
					End If

					depthSum += oc
				Next i

				Return InputType.convolutional3D(Convolution3D.DataFormat.NDHWC, fd, fh, fw, depthSum)
			ElseIf first.getType() <> InputType.Type.CNN Then
				'FF or RNN data inputs
				Dim size As Integer = 0
				Dim type As InputType.Type = Nothing
				Dim format As RNNFormat = Nothing
				Dim timeSeriesLength As Long = -1
				'scan for input type for recurrent
				For i As Integer = 0 To vertexInputs.Length - 1
					If vertexInputs(i).getType() = InputType.Type.RNN Then
						If format = Nothing Then
							Dim input As InputType.InputTypeRecurrent = DirectCast(vertexInputs(i), InputType.InputTypeRecurrent)
							format = input.getFormat()
							timeSeriesLength = DirectCast(vertexInputs(i), InputType.InputTypeRecurrent).getTimeSeriesLength()
						ElseIf format <> Nothing Then
							Dim input As InputType.InputTypeRecurrent = DirectCast(vertexInputs(i), InputType.InputTypeRecurrent)
							If input.getFormat() IsNot Nothing AndAlso format <> input.getFormat() Then
								Throw New System.ArgumentException("Unable to merge inputs with 2 different layouts of input type: " & input.getType() & " and type " & vertexInputs(i).getType())
							End If
						End If
					End If
				Next i

				For i As Integer = 0 To vertexInputs.Length - 1
					Dim thisSize As Long = 0
					Select Case vertexInputs(i).getType()
						Case FF
							'ignore feedforward, rnn trumps feedforward and can be merged
							If format <> Nothing Then
								thisSize = DirectCast(vertexInputs(i), InputType.InputTypeFeedForward).getSize()
								type = InputType.Type.FF
							'feedforward case
							Else
								thisSize = DirectCast(vertexInputs(i), InputType.InputTypeFeedForward).getSize()
								type = InputType.Type.FF
							End If
						Case RNN
							thisSize = DirectCast(vertexInputs(i), InputType.InputTypeRecurrent).getSize()
							'don't change dimension if it was already modified
							If Not modified Then
								Me.mergeAxis_Conflict = If(format = RNNFormat.NCW, 1, 2)
							End If
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
						Dim tsLength As val = DirectCast(vertexInputs(0), InputType.InputTypeRecurrent).getTimeSeriesLength()
						Return InputType.recurrent(size, tsLength, format)
					End If
				Else
					'size is unknown
					If type = InputType.Type.FF Then
						Return InputType.feedForward(-1)
					Else
						If first.getType() = InputType.Type.FF Then
							Dim inputTypeFeedForward As InputType.InputTypeFeedForward = DirectCast(first, InputType.InputTypeFeedForward)
							Return InputType.recurrent(inputTypeFeedForward.getSize(), timeSeriesLength, format)
						Else
							Return InputType.recurrent(-1, timeSeriesLength, format)
						End If
					End If
				End If

			Else
				'CNN inputs... also check that the channels, width and heights match:
				Dim firstConv As InputType.InputTypeConvolutional = DirectCast(first, InputType.InputTypeConvolutional)
				Dim format As CNN2DFormat = firstConv.getFormat()

				Dim fd As val = firstConv.getChannels()
				Dim fw As val = firstConv.getWidth()
				Dim fh As val = firstConv.getHeight()

				Dim depthSum As Long = fd

				For i As Integer = 1 To vertexInputs.Length - 1
					If vertexInputs(i).getType() <> InputType.Type.CNN Then
						Throw New InvalidInputTypeException("Invalid input: MergeVertex cannot process activations of different types:" & " first type = " & InputType.Type.CNN & ", input type " & (i + 1) & " = " & vertexInputs(i).getType())
					End If

					Dim otherConv As InputType.InputTypeConvolutional = DirectCast(vertexInputs(i), InputType.InputTypeConvolutional)

					Dim od As val = otherConv.getChannels()
					Dim ow As val = otherConv.getWidth()
					Dim oh As val = otherConv.getHeight()

					If fw <> ow OrElse fh <> oh Then
						Throw New InvalidInputTypeException("Invalid input: MergeVertex cannot merge CNN activations of different width/heights:" & "first [channels,width,height] = [" & fd & "," & fw & "," & fh & "], input " & i & " = [" & od & "," & ow & "," & oh & "]")
					End If

					depthSum += od
				Next i

				'don't change dimension if it was already modified
				If Me.mergeAxis_Conflict = DEFAULT_MERGE_DIM Then
					Me.mergeAxis_Conflict = If(format = CNN2DFormat.NCHW, 1, 3)
				End If
				Return InputType.convolutional(fh, fw, depthSum, format)
			End If
		End Function

		Public Overridable Property MergeAxis As Integer
			Get
				Return mergeAxis_Conflict
			End Get
			Set(ByVal mergeAxis As Integer)
				Me.mergeAxis_Conflict = mergeAxis
				modified = True
			End Set
		End Property


		Public Overrides Function getMemoryReport(ParamArray ByVal inputTypes() As InputType) As MemoryReport
			Dim outputType As InputType = getOutputType(-1, inputTypes)

			'TODO multiple input types
			Return (New LayerMemoryReport.Builder(Nothing, GetType(MergeVertex), inputTypes(0), outputType)).standardMemory(0, 0).workingMemory(0, 0, 0, 0).cacheMemory(0, 0).build()
		End Function
	End Class

End Namespace