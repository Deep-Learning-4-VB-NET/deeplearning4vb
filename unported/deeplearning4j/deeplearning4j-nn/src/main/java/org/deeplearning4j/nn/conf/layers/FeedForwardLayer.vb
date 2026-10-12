Imports System
Imports lombok
Imports DataFormat = org.deeplearning4j.nn.conf.DataFormat
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports Cnn3DToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.Cnn3DToFeedForwardPreProcessor
Imports CnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor
Imports RnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.RnnToFeedForwardPreProcessor
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer

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

Namespace org.deeplearning4j.nn.conf.layers

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public abstract class FeedForwardLayer extends BaseLayer
	<Serializable>
	Public MustInherit Class FeedForwardLayer
		Inherits BaseLayer

		Protected Friend nIn As Long
		Protected Friend nOut As Long
		Protected Friend timeDistributedFormat As DataFormat

		Public Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.nIn = builder.nIn
			Me.nOut = builder.nOut
		End Sub


		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing OrElse (inputType.getType() <> InputType.Type.FF AndAlso inputType.getType() <> InputType.Type.CNNFlat) Then
				Throw New System.InvalidOperationException("Invalid input type (layer index = " & layerIndex & ", layer name=""" & getLayerName() & """): expected FeedForward input type. Got: " & inputType)
			End If

			Return InputType.feedForward(nOut, timeDistributedFormat)
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If inputType Is Nothing OrElse (inputType.getType() <> InputType.Type.FF AndAlso inputType.getType() <> InputType.Type.CNNFlat AndAlso inputType.getType() <> InputType.Type.RNN) Then
				Throw New System.InvalidOperationException("Invalid input type (layer name=""" & getLayerName() & """): expected FeedForward input type. Got: " & inputType)
			End If

			If nIn <= 0 OrElse override Then
				If inputType.getType() = InputType.Type.FF Then
					Dim f As InputType.InputTypeFeedForward = DirectCast(inputType, InputType.InputTypeFeedForward)
					Me.nIn = f.getSize()
				ElseIf inputType.getType() = InputType.Type.RNN Then
					Dim recurrent As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
					'default value when initializing input type recurrent
					If recurrent.getTimeSeriesLength() < 0 Then
						Me.nIn = recurrent.getSize()
					Else
						Me.nIn = recurrent.getSize()

					End If
				Else
					Dim f As InputType.InputTypeConvolutionalFlat = DirectCast(inputType, InputType.InputTypeConvolutionalFlat)
					Me.nIn = f.FlattenedSize
				End If
			End If

			If TypeOf inputType Is InputType.InputTypeFeedForward Then
				Dim f As InputType.InputTypeFeedForward = DirectCast(inputType, InputType.InputTypeFeedForward)
				Me.timeDistributedFormat = f.getTimeDistributedFormat()
			End If
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input for layer (layer name = """ & getLayerName() & """): input type is null")
			End If

			Select Case inputType.getType()
				Case FF, CNNFlat
					'FF -> FF and CNN (flattened format) -> FF: no preprocessor necessary
					Return Nothing
				Case RNN
					'RNN -> FF
					Return New RnnToFeedForwardPreProcessor(DirectCast(inputType, InputType.InputTypeRecurrent).getFormat())
				Case CNN
					'CNN -> FF
					Dim c As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
					Return New CnnToFeedForwardPreProcessor(c.getHeight(), c.getWidth(), c.getChannels(), c.getFormat())
				Case CNN3D
					'CNN3D -> FF
					Dim c3d As InputType.InputTypeConvolutional3D = DirectCast(inputType, InputType.InputTypeConvolutional3D)
					Return New Cnn3DToFeedForwardPreProcessor(c3d.getDepth(), c3d.getHeight(), c3d.getWidth(), c3d.getChannels(), c3d.getDataFormat() = Convolution3D.DataFormat.NCDHW)
				Case Else
					Throw New Exception("Unknown input type: " & inputType)
			End Select
		End Function

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Return False 'No pretrain params in standard FF layers
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public abstract static class Builder<T extends Builder<T>> extends BaseLayer.Builder<T>
		Public MustInherit Class Builder(Of T As Builder(Of T))
			Inherits BaseLayer.Builder(Of T)

			''' <summary>
			''' Number of inputs for the layer (usually the size of the last layer). <br> Note that for Convolutional layers,
			''' this is the input channels, otherwise is the previous layer size.
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field nIn was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend nIn_Conflict As Long = 0

			''' <summary>
			''' Number of inputs for the layer (usually the size of the last layer). <br> Note that for Convolutional layers,
			''' this is the input channels, otherwise is the previous layer size.
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field nOut was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend nOut_Conflict As Long = 0

			''' <summary>
			''' Number of inputs for the layer (usually the size of the last layer). <br> Note that for Convolutional layers,
			''' this is the input channels, otherwise is the previous layer size.
			''' </summary>
			''' <param name="nIn"> Number of inputs for the layer </param>
'JAVA TO VB CONVERTER NOTE: The parameter nIn was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nIn(ByVal nIn_Conflict As Integer) As T
				Me.setNIn(nIn_Conflict)
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Number of inputs for the layer (usually the size of the last layer). <br> Note that for Convolutional layers,
			''' this is the input channels, otherwise is the previous layer size.
			''' </summary>
			''' <param name="nIn"> Number of inputs for the layer </param>
'JAVA TO VB CONVERTER NOTE: The parameter nIn was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nIn(ByVal nIn_Conflict As Long) As T
				Me.setNIn(nIn_Conflict)
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Number of outputs - used to set the layer size (number of units/nodes for the current layer). Note that this
			''' is equivalent to <seealso cref="units(Integer)"/>
			''' </summary>
			''' <param name="nOut"> Number of outputs / layer size </param>
'JAVA TO VB CONVERTER NOTE: The parameter nOut was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nOut(ByVal nOut_Conflict As Integer) As T
				Me.setNOut(nOut_Conflict)
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Number of outputs - used to set the layer size (number of units/nodes for the current layer). Note that this
			''' is equivalent to <seealso cref="units(Integer)"/>
			''' </summary>
			''' <param name="nOut"> Number of outputs / layer size </param>
'JAVA TO VB CONVERTER NOTE: The parameter nOut was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function nOut(ByVal nOut_Conflict As Long) As T
				Me.setNOut(CInt(nOut_Conflict))
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Set the number of units / layer size for this layer.<br> This is equivalent to <seealso cref="nOut(Integer)"/>
			''' </summary>
			''' <param name="units"> Size of the layer (number of units) / nOut </param>
			''' <seealso cref= #nOut(int) </seealso>
'JAVA TO VB CONVERTER NOTE: The parameter units was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function units(ByVal units_Conflict As Integer) As T
				Return nOut(units_Conflict)
			End Function
		End Class
	End Class

End Namespace