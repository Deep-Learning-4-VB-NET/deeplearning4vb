Imports System
Imports System.Linq
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports DataFormat = org.deeplearning4j.nn.conf.DataFormat
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InvalidInputTypeException = org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
Imports Convolution3D = org.deeplearning4j.nn.conf.layers.Convolution3D
Imports BaseInputPreProcessor = org.deeplearning4j.nn.conf.preprocessor.BaseInputPreProcessor
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty
import static org.nd4j.common.util.ArrayUtil.prodLong

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

Namespace org.deeplearning4j.nn.modelimport.keras.preprocessors


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Slf4j @EqualsAndHashCode(callSuper = false) @JsonIgnoreProperties({"miniBatchSize", "staticTargetShape"}) public class ReshapePreprocessor extends org.deeplearning4j.nn.conf.preprocessor.BaseInputPreProcessor
	<Serializable>
	Public Class ReshapePreprocessor
		Inherits BaseInputPreProcessor

		Private ReadOnly inputShape() As Long
		Private ReadOnly targetShape() As Long
		Private hasMiniBatchDimension As Boolean
		Private format As DataFormat

		''' <param name="inputShape">            Input shape, with or without leading minibatch dimension, depending on value of hasMiniBatchDimension </param>
		''' <param name="targetShape">           Target shape, with or without leading minibatch dimension, depending on value of hasMiniBatchDimension </param>
		''' <param name="hasMiniBatchDimension"> If true: shapes should be of the form [minibatch, x, y, ...]; if false: shapes should be of form [x, y, ...] </param>
		Public Sub New(ByVal inputShape() As Long, ByVal targetShape() As Long, ByVal hasMiniBatchDimension As Boolean)
			Me.New(inputShape, targetShape, hasMiniBatchDimension, Nothing)
		End Sub

		''' <param name="inputShape">            Input shape, with or without leading minibatch dimension, depending on value of hasMiniBatchDimension </param>
		''' <param name="targetShape">           Target shape, with or without leading minibatch dimension, depending on value of hasMiniBatchDimension </param>
		''' <param name="hasMiniBatchDimension"> If true: shapes should be of the form [minibatch, x, y, ...]; if false: shapes should be of form [x, y, ...] </param>
		''' <param name="dataFormat">            May be null. If non-null: </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ReshapePreprocessor(@JsonProperty("inputShape") long[] inputShape, @JsonProperty("targetShape") long[] targetShape, @JsonProperty("hasMiniBatchDimension") boolean hasMiniBatchDimension, @JsonProperty("dataFormat") org.deeplearning4j.nn.conf.DataFormat dataFormat)
		Public Sub New(ByVal inputShape() As Long, ByVal targetShape() As Long, ByVal hasMiniBatchDimension As Boolean, ByVal dataFormat As DataFormat)
			Me.inputShape = inputShape
			Me.targetShape = targetShape
			Me.hasMiniBatchDimension = hasMiniBatchDimension
			Me.format = dataFormat
		End Sub

		Private Function getShape(ByVal originalShape() As Long, ByVal minibatch As Long) As Long()
			Dim newShape() As Long = (If(hasMiniBatchDimension, originalShape, prependMiniBatchSize(originalShape, minibatch)))
			If newShape(0) <> minibatch Then
				newShape = CType(newShape.Clone(), Long())
				newShape(0) = minibatch
			End If
			Return newShape
		End Function

		Private Shared Function prependMiniBatchSize(ByVal shape() As Long, ByVal miniBatchSize As Long) As Long()
			Dim shapeLength As Integer = shape.Length
			Dim miniBatchShape As val = New Long(shapeLength){}
			miniBatchShape(0) = miniBatchSize
			For i As Integer = 1 To miniBatchShape.length - 1
				miniBatchShape(i) = shape(i - 1)
			Next i
			Return miniBatchShape
		End Function

		Public Overrides Function preProcess(ByVal input As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			' the target shape read from a keras config does not have mini-batch size included. We prepend it here dynamically.
			Dim targetShape() As Long = getShape(Me.targetShape, miniBatchSize)

			If prodLong(input.shape()) = prodLong((targetShape)) Then
				If input.ordering() <> "c"c OrElse Not Shape.hasDefaultStridesForShape(input) Then
					input = workspaceMgr.dup(ArrayType.ACTIVATIONS, input, "c"c)
				End If
				Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, input.reshape(targetShape))
			Else
				Throw New System.InvalidOperationException("Input shape " & Arrays.toString(input.shape()) & " and target shape" & Arrays.toString(targetShape) & " do not match")
			End If
		End Function

		Public Overrides Function backprop(ByVal output As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim targetShape() As Long = getShape(Me.targetShape, miniBatchSize)
			Dim inputShape() As Long = getShape(Me.inputShape, miniBatchSize)

			If Not targetShape.SequenceEqual(output.shape()) Then
				Throw New System.InvalidOperationException("Unexpected output shape" & Arrays.toString(output.shape()) & " (expected to be " & Arrays.toString(targetShape) & ")")
			End If
			If prodLong(output.shape()) = prodLong((targetShape)) Then
				If output.ordering() <> "c"c OrElse Not Shape.hasDefaultStridesForShape(output) Then
					output = workspaceMgr.dup(ArrayType.ACTIVATIONS, output, "c"c)
				End If
				Return workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, output.reshape(inputShape))
			Else
				Throw New System.InvalidOperationException("Output shape" & Arrays.toString(output.shape()) & " and input shape" & Arrays.toString(targetShape) & " do not match")
			End If
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(org.deeplearning4j.nn.conf.inputs.InputType inputType) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
		Public Overrides Function getOutputType(ByVal inputType As InputType) As InputType
			Dim shape() As Long = getShape(Me.targetShape, 0)
			Dim ret As InputType
			Select Case shape.Length
				Case 2
					ret = InputType.feedForward(shape(1))
				Case 3
					Dim format As RNNFormat = RNNFormat.NWC
					If Me.format IsNot Nothing AndAlso TypeOf Me.format Is RNNFormat Then
						format = DirectCast(Me.format, RNNFormat)
					End If

					ret = InputType.recurrent(shape(2), shape(1), format)
				Case 4
					If inputShape.Length = 1 OrElse inputType.getType() = InputType.Type.RNN Then
						'note here the default is tensorflow initialization for keras.
						'being channels first has side effects when working with other models
						ret = InputType.convolutional(shape(1), shape(2), shape(3),CNN2DFormat.NHWC)
					Else

						Dim cnnFormat As CNN2DFormat = CNN2DFormat.NCHW
						If Me.format IsNot Nothing AndAlso TypeOf Me.format Is CNN2DFormat Then
							cnnFormat = DirectCast(Me.format, CNN2DFormat)
						End If

						If cnnFormat = CNN2DFormat.NCHW Then
							ret = InputType.convolutional(shape(2), shape(3), shape(1), cnnFormat)
						Else
							ret = InputType.convolutional(shape(1), shape(2), shape(3), cnnFormat)
						End If
					End If
				Case 5
					If inputShape.Length = 1 OrElse inputType.getType() = InputType.Type.RNN Then
						'note here the default is tensorflow initialization for keras.
						'being channels first has side effects when working with other models
						Dim dataFormat As Convolution3D.DataFormat = DirectCast(Me.format, Convolution3D.DataFormat)
						If dataFormat = Convolution3D.DataFormat.NCDHW Then
							ret = InputType.convolutional3D(dataFormat,shape(2),shape(3),shape(4),shape(1))
							'default value
						ElseIf dataFormat = Convolution3D.DataFormat.NDHWC OrElse dataFormat = Nothing Then
							ret = InputType.convolutional3D(dataFormat,shape(1),shape(2),shape(3),shape(4))
						Else
							Throw New System.ArgumentException("Illegal format found " & dataFormat)
						End If
					Else

						Dim cnnFormat As CNN2DFormat = CNN2DFormat.NCHW
						If Me.format IsNot Nothing AndAlso TypeOf Me.format Is CNN2DFormat Then
							cnnFormat = DirectCast(Me.format, CNN2DFormat)
						End If

						If cnnFormat = CNN2DFormat.NCHW Then
							ret = InputType.convolutional(shape(2), shape(3), shape(1), cnnFormat)
						Else
							ret = InputType.convolutional(shape(1), shape(2), shape(3), cnnFormat)
						End If
					End If
				Case Else
					Throw New System.NotSupportedException("Cannot infer input type for reshape array " & Arrays.toString(shape))
			End Select
			Return ret
		End Function
	End Class
End Namespace