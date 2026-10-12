Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DataFormat = org.deeplearning4j.nn.conf.DataFormat
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports Convolution3D = org.deeplearning4j.nn.conf.layers.Convolution3D
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports OneTimeLogger = org.nd4j.common.util.OneTimeLogger
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports JsonIgnore = org.nd4j.shade.jackson.annotation.JsonIgnore
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo

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

Namespace org.deeplearning4j.nn.conf.inputs


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") @Slf4j public abstract class InputType implements java.io.Serializable
	<Serializable>
	Public MustInherit Class InputType

		''' <summary>
		''' The type of activations in/out of a given GraphVertex<br>
		''' FF: Standard feed-foward (2d minibatch, 1d per example) data<br>
		''' RNN: Recurrent neural network (3d minibatch) time series data<br>
		''' CNN: 2D Convolutional neural network (4d minibatch, [miniBatchSize, channels, height, width])
		''' CNNFlat: Flattened 2D conv net data (2d minibatch, [miniBatchSize, height * width * channels])
		''' CNN3D: 3D convolutional neural network (5d minibatch, [miniBatchSize, channels, height, width, channels])
		''' </summary>
		Public Enum Type
			FF
			RNN
			CNN
			CNNFlat
			CNN3D
		End Enum

		Public Shared Property DefaultCNN2DFormat As CNN2DFormat
			Get
				Return defaultCNN2DFormat
			End Get
			Set(ByVal defaultCNN2DFormat As CNN2DFormat)
				InputType.defaultCNN2DFormat = defaultCNN2DFormat
			End Set
		End Property


		Private Shared defaultCNN2DFormat As CNN2DFormat = CNN2DFormat.NCHW

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnore public abstract Type getType();
		Public MustOverride Function [getType]() As Type

		Public MustOverride Overrides Function ToString() As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnore public abstract long arrayElementsPerExample();
		Public MustOverride Function arrayElementsPerExample() As Long

		''' <summary>
		''' Returns the shape of this InputType
		''' </summary>
		''' <param name="includeBatchDim"> Whether to include minibatch in the return shape array </param>
		''' <returns> int[] </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnore public abstract long[] getShape(boolean includeBatchDim);
		Public MustOverride Function getShape(ByVal includeBatchDim As Boolean) As Long()

		''' <summary>
		''' Returns the shape of this InputType without minibatch dimension in the returned array
		''' </summary>
		''' <returns> int[] </returns>
		Public Overridable ReadOnly Property Shape As Long()
			Get
				Return getShape(False)
			End Get
		End Property

		''' <summary>
		''' InputType for feed forward network data
		''' </summary>
		''' <param name="size"> The size of the activations </param>
		''' <returns> InputTypeFeedForward </returns>
		Public Shared Function feedForward(ByVal size As Long) As InputType
			Return New InputTypeFeedForward(size, Nothing)
		End Function

		Public Shared Function feedForward(ByVal size As Long, ByVal timeDistributedFormat As DataFormat) As InputType
			Return New InputTypeFeedForward(size,timeDistributedFormat)
		End Function

		''' <summary>
		''' InputType for recurrent neural network (time series) data
		''' </summary>
		''' <param name="size"> The size of the activations </param>
		''' <returns> InputTypeRecurrent </returns>
		Public Shared Function recurrent(ByVal size As Long) As InputType
			Return New InputTypeRecurrent(size)
		End Function

		''' <summary>
		''' InputType for recurrent neural network (time series) data
		''' </summary>
		''' <param name="size">             The size of the activations </param>
		''' <param name="timeSeriesLength"> Length of the input time series </param>
		''' <returns> InputTypeRecurrent </returns>
		Public Shared Function recurrent(ByVal size As Long, ByVal timeSeriesLength As Long) As InputType
			Return New InputTypeRecurrent(size, timeSeriesLength, RNNFormat.NCW)
		End Function

		Public Shared Function recurrent(ByVal size As Long, ByVal format As RNNFormat) As InputType
			Return New InputTypeRecurrent(size, format)
		End Function

		Public Shared Function recurrent(ByVal size As Long, ByVal timeSeriesLength As Long, ByVal format As RNNFormat) As InputType
			Return New InputTypeRecurrent(size, timeSeriesLength, format)
		End Function
		''' <summary>
		''' Input type for convolutional (CNN) data, that is 4d with shape [miniBatchSize, channels, height, width].
		''' For CNN data that has been flattened, use <seealso cref="convolutionalFlat(Long, Long, Long)"/>
		''' </summary>
		''' <param name="height"> height of the input </param>
		''' <param name="width">  Width of the input </param>
		''' <param name="depth">  Depth, or number of channels </param>
		''' <returns> InputTypeConvolutional </returns>
		Public Shared Function convolutional(ByVal height As Long, ByVal width As Long, ByVal depth As Long) As InputType
			Return convolutional(height, width, depth, DefaultCNN2DFormat)
		End Function

		Public Shared Function convolutional(ByVal height As Long, ByVal width As Long, ByVal depth As Long, ByVal format As CNN2DFormat) As InputType
			Return New InputTypeConvolutional(height, width, depth, format)
		End Function

		''' <summary>
		''' Input type for 3D convolutional (CNN3D) data in NDHWC format, that is 5d with shape
		''' [miniBatchSize, depth, height, width, channels].
		''' </summary>
		''' <param name="height">   height of the input </param>
		''' <param name="width">    Width of the input </param>
		''' <param name="depth">    Depth of the input </param>
		''' <param name="channels"> Number of channels of the input </param>
		''' <returns> InputTypeConvolutional3D </returns>
		''' @deprecated Use <seealso cref="convolutional3D(Convolution3D.DataFormat, Long, Long, Long, Long)"/> 
		<Obsolete("Use <seealso cref=""convolutional3D(Convolution3D.DataFormat, Long, Long, Long, Long)""/>")>
		Public Shared Function convolutional3D(ByVal depth As Long, ByVal height As Long, ByVal width As Long, ByVal channels As Long) As InputType
			Return convolutional3D(Convolution3D.DataFormat.NDHWC, depth, height, width, channels)
		End Function

		''' <summary>
		''' Input type for 3D convolutional (CNN3D) 5d data:<br>
		''' If NDHWC format [miniBatchSize, depth, height, width, channels]<br>
		''' If NDCWH
		''' </summary>
		''' <param name="height">   height of the input </param>
		''' <param name="width">    Width of the input </param>
		''' <param name="depth">    Depth of the input </param>
		''' <param name="channels"> Number of channels of the input </param>
		''' <returns> InputTypeConvolutional3D </returns>
		Public Shared Function convolutional3D(ByVal dataFormat As Convolution3D.DataFormat, ByVal depth As Long, ByVal height As Long, ByVal width As Long, ByVal channels As Long) As InputType
			Return New InputTypeConvolutional3D(dataFormat, depth, height, width, channels)
		End Function

		''' <summary>
		''' Input type for convolutional (CNN) data, where the data is in flattened (row vector) format.
		''' Expect data with shape [miniBatchSize, height * width * channels]. For CNN data in 4d format,
		''' use <seealso cref="convolutional(Long, Long, Long)"/>
		''' </summary>
		''' <param name="height"> Height of the (unflattened) data represented by this input type </param>
		''' <param name="width">  Width of the (unflattened) data represented by this input type </param>
		''' <param name="depth">  Depth of the (unflattened) data represented by this input type </param>
		''' <returns> InputTypeConvolutionalFlat </returns>
		Public Shared Function convolutionalFlat(ByVal height As Long, ByVal width As Long, ByVal depth As Long) As InputType
			Return New InputTypeConvolutionalFlat(height, width, depth)
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Getter @EqualsAndHashCode(callSuper = false) public static class InputTypeFeedForward extends InputType
		<Serializable>
		Public Class InputTypeFeedForward
			Inherits InputType

			Friend size As Long
			Friend timeDistributedFormat As DataFormat

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public InputTypeFeedForward(@JsonProperty("size") long size, @JsonProperty("timeDistributedFormat") org.deeplearning4j.nn.conf.DataFormat timeDistributedFormat)
			Public Sub New(ByVal size As Long, ByVal timeDistributedFormat As DataFormat)
				If size <= 0 Then
					OneTimeLogger.warn(log,"Assigning a size of zero. This is normally only valid in model import cases with unknown dimensions.")
				End If
				Me.size = size
				Me.timeDistributedFormat = timeDistributedFormat
			End Sub

			Public Overrides Function [getType]() As Type
				Return Type.FF
			End Function

			Public Overrides Function ToString() As String
				Return "InputTypeFeedForward(" & size + (If(timeDistributedFormat IsNot Nothing, "," & timeDistributedFormat, "")) & ")"
			End Function

			Public Overrides Function arrayElementsPerExample() As Long
				Return size
			End Function

			Public Overrides Function getShape(ByVal includeBatchDim As Boolean) As Long()
				If includeBatchDim Then
					Return New Long(){-1, size}
				Else
					Return New Long(){size}
				End If
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Getter @EqualsAndHashCode(callSuper = false) public static class InputTypeRecurrent extends InputType
		<Serializable>
		Public Class InputTypeRecurrent
			Inherits InputType

			Friend size As Long
			Friend timeSeriesLength As Long
			Friend format As RNNFormat = RNNFormat.NCW
			Public Sub New(ByVal size As Long)
				Me.New(size, -1)
			End Sub
			Public Sub New(ByVal size As Long, ByVal timeSeriesLength As Long)
				Me.New(size, timeSeriesLength, RNNFormat.NCW)
			End Sub

			Public Sub New(ByVal size As Long, ByVal format As RNNFormat)
				Me.New(size, -1, format)
			End Sub
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public InputTypeRecurrent(@JsonProperty("size") long size, @JsonProperty("timeSeriesLength") long timeSeriesLength, @JsonProperty("format") org.deeplearning4j.nn.conf.RNNFormat format)
			Public Sub New(ByVal size As Long, ByVal timeSeriesLength As Long, ByVal format As RNNFormat)
				Me.size = size
				Me.timeSeriesLength = timeSeriesLength
				Me.format = format
			End Sub

			Public Overrides Function [getType]() As Type
				Return Type.RNN
			End Function

			Public Overrides Function ToString() As String
				If timeSeriesLength > 0 Then
					Return "InputTypeRecurrent(" & size & ",timeSeriesLength=" & timeSeriesLength & ",format=" & format & ")"
				Else
					Return "InputTypeRecurrent(" & size & ",format=" & format & ")"
				End If
			End Function

			Public Overrides Function arrayElementsPerExample() As Long
				If timeSeriesLength <= 0 Then
					Throw New System.InvalidOperationException("Cannot calculate number of array elements per example: " & "time series length is not set. Use InputType.recurrent(int size, int timeSeriesLength) instead?")
				End If
				Return timeSeriesLength * size
			End Function

			Public Overrides Function getShape(ByVal includeBatchDim As Boolean) As Long()
				If includeBatchDim Then
					If format = RNNFormat.NCW Then
						Return New Long(){-1, size, timeSeriesLength}
					Else
						Return New Long(){-1, timeSeriesLength, size}
					End If

				Else
					If format = RNNFormat.NCW Then
						Return New Long(){size, timeSeriesLength}
					Else
						Return New Long(){timeSeriesLength, size}
					End If
				End If
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Data @EqualsAndHashCode(callSuper = false) public static class InputTypeConvolutional extends InputType
		<Serializable>
		Public Class InputTypeConvolutional
			Inherits InputType

			Friend height As Long
			Friend width As Long
			Friend channels As Long
			Friend format As CNN2DFormat = CNN2DFormat.NCHW 'Default for JSON deserialization of older configurations

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public InputTypeConvolutional(@JsonProperty("height") long height, @JsonProperty("width") long width, @JsonProperty("channels") long channels, @JsonProperty("format") org.deeplearning4j.nn.conf.CNN2DFormat format)
			Public Sub New(ByVal height As Long, ByVal width As Long, ByVal channels As Long, ByVal format As CNN2DFormat)
				If height <= 0 Then
					OneTimeLogger.warn(log,"Assigning height of 0. Normally this is not valid. Exceptions for this are generally related" & "to model import and unknown dimensions")
				End If

				If width <= 0 Then
					OneTimeLogger.warn(log,"Assigning height of 0. Normally this is not valid. Exceptions for this are generally related" & "to model import and unknown dimensions")
				End If

				If width <= 0 Then
					OneTimeLogger.warn(log,"Assigning width of 0. Normally this is not valid. Exceptions for this are generally related" & "to model import and unknown dimensions")
				End If

				If channels <= 0 Then
					OneTimeLogger.warn(log,"Assigning width of 0. Normally this is not valid. Exceptions for this are generally related" & "to model import and unknown dimensions")
				End If


				Me.height = height
				Me.width = width
				Me.channels = channels
				If format <> Nothing Then
					Me.format = format
				End If
			End Sub

			Public Sub New(ByVal height As Long, ByVal width As Long, ByVal channels As Long)
				Me.New(height, width, channels, CNN2DFormat.NCHW)
			End Sub

			''' <summary>
			''' Return the number of channels / depth for this 2D convolution. This method has been deprecated,
			''' for consistency purposes, use getChannels() instead.
			''' </summary>
			''' <returns> number of channels, i.e. depth for 2D convolutions </returns>
			<Obsolete>
			Public Overridable Property Depth As Long
				Get
					Return channels
				End Get
				Set(ByVal depth As Long)
					Me.channels = depth
				End Set
			End Property


			Public Overrides Function [getType]() As Type
				Return Type.CNN
			End Function

			Public Overrides Function ToString() As String
				Return "InputTypeConvolutional(h=" & height & ",w=" & width & ",c=" & channels & "," & format & ")"
			End Function

			Public Overrides Function arrayElementsPerExample() As Long
				Return height * width * channels
			End Function

			Public Overrides Function getShape(ByVal includeBatchDim As Boolean) As Long()
				If format = CNN2DFormat.NCHW Then
					If includeBatchDim Then
						Return New Long(){-1, channels, height, width}
					Else
						Return New Long(){channels, height, width}
					End If
				Else
					If includeBatchDim Then
						Return New Long(){-1, height, width, channels}
					Else
						Return New Long(){height, width, channels}
					End If
				End If
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Data @EqualsAndHashCode(callSuper = false) public static class InputTypeConvolutional3D extends InputType
		<Serializable>
		Public Class InputTypeConvolutional3D
			Inherits InputType

			Friend dataFormat As Convolution3D.DataFormat
			Friend depth As Long
			Friend height As Long
			Friend width As Long
			Friend channels As Long

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public InputTypeConvolutional3D(@JsonProperty("dataFormat") org.deeplearning4j.nn.conf.layers.Convolution3D.DataFormat dataFormat, @JsonProperty("depth") long depth, @JsonProperty("height") long height, @JsonProperty("width") long width, @JsonProperty("channels") long channels)
			Public Sub New(ByVal dataFormat As Convolution3D.DataFormat, ByVal depth As Long, ByVal height As Long, ByVal width As Long, ByVal channels As Long)
				Me.dataFormat = dataFormat
				Me.depth = depth
				Me.height = height
				Me.width = width
				Me.channels = channels
			End Sub

			Public Overrides Function [getType]() As Type
				Return Type.CNN3D
			End Function

			Public Overrides Function ToString() As String
				Return "InputTypeConvolutional3D(format=" & dataFormat & ",d=" & depth & ",h=" & height & ",w=" & width & ",c=" & channels & ")"
			End Function

			Public Overrides Function arrayElementsPerExample() As Long
				Return height * width * depth * channels
			End Function

			Public Overrides Function getShape(ByVal includeBatchDim As Boolean) As Long()
				If dataFormat = Convolution3D.DataFormat.NDHWC Then
					If includeBatchDim Then
						Return New Long(){-1, depth, height, width, channels}
					Else
						Return New Long(){depth, height, width, channels}
					End If
				Else
					If includeBatchDim Then
						Return New Long(){-1, channels, depth, height, width}
					Else
						Return New Long(){channels, depth, height, width}
					End If
				End If
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Data @EqualsAndHashCode(callSuper = false) public static class InputTypeConvolutionalFlat extends InputType
		<Serializable>
		Public Class InputTypeConvolutionalFlat
			Inherits InputType

			Friend height As Long
			Friend width As Long
			Friend depth As Long

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public InputTypeConvolutionalFlat(@JsonProperty("height") long height, @JsonProperty("width") long width, @JsonProperty("depth") long depth)
			Public Sub New(ByVal height As Long, ByVal width As Long, ByVal depth As Long)
				Me.height = height
				Me.width = width
				Me.depth = depth
			End Sub

			Public Overrides Function [getType]() As Type
				Return Type.CNNFlat
			End Function

			Public Overridable ReadOnly Property FlattenedSize As Long
				Get
					Return height * width * depth
				End Get
			End Property

			Public Overridable ReadOnly Property UnflattenedType As InputType
				Get
					Return InputType.convolutional(height, width, depth)
				End Get
			End Property

			Public Overrides Function ToString() As String
				Return "InputTypeConvolutionalFlat(h=" & height & ",w=" & width & ",d=" & depth & ")"
			End Function

			Public Overrides Function arrayElementsPerExample() As Long
				Return height * width * depth
			End Function

			Public Overrides Function getShape(ByVal includeBatchDim As Boolean) As Long()
				If includeBatchDim Then
					Return New Long(){-1, depth, height, width}
				Else
					Return New Long(){depth, height, width}
				End If
			End Function
		End Class


		Public Shared Function inferInputType(ByVal inputArray As INDArray) As InputType
			'Note: ConvolutionalFlat and FeedForward look identical... but either should work OK if using something
			' like FeedForwardToCnnPreProcessor

			Select Case inputArray.rank()
				Case 2
					Return InputType.feedForward(inputArray.size(1))
				Case 3
					Return InputType.recurrent(inputArray.size(1), CInt(inputArray.size(2)))
				Case 4
					'Order: [minibatch, channels, height, width] -> [h, w, c]
					Return InputType.convolutional(inputArray.size(2), CInt(inputArray.size(3)), CInt(inputArray.size(1)))
				Case 5
					'Order: [minibatch, channels, depth, height, width] -> [d, h, w, c]
					Return InputType.convolutional3D(inputArray.size(2), CInt(inputArray.size(3)), CInt(inputArray.size(4)), CInt(inputArray.size(1)))
				Case Else
					Throw New System.ArgumentException("Cannot infer input type for array with shape: " & Arrays.toString(inputArray.shape()))
			End Select
		End Function

		Public Shared Function inferInputTypes(ParamArray ByVal inputArrays() As INDArray) As InputType()
			Dim [out](inputArrays.Length - 1) As InputType
			For i As Integer = 0 To inputArrays.Length - 1
				[out](i) = inferInputType(inputArrays(i))
			Next i

			Return [out]
		End Function

	End Class

End Namespace