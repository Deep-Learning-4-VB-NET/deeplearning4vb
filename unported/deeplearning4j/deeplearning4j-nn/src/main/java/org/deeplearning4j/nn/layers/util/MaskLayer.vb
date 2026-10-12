Imports System
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Broadcast = org.nd4j.linalg.factory.Broadcast
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType

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

Namespace org.deeplearning4j.nn.layers.util


	<Serializable>
	Public Class MaskLayer
		Inherits AbstractLayer(Of org.deeplearning4j.nn.conf.layers.util.MaskLayer)

		Private emptyGradient As Gradient = New DefaultGradient()

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Public Overrides Function clone() As Layer
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Sub clearNoiseWeightParams()
			'No op
		End Sub

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			Return New Pair(Of Gradient, INDArray)(emptyGradient, applyMask(epsilon, maskArray_Conflict, workspaceMgr, ArrayType.ACTIVATION_GRAD))
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Return applyMask(input_Conflict, maskArray_Conflict, workspaceMgr, ArrayType.ACTIVATIONS)
		End Function

		Private Shared Overloads Function applyMask(ByVal input As INDArray, ByVal maskArray As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal type As ArrayType) As INDArray
			If maskArray Is Nothing Then
				Return workspaceMgr.leverageTo(type, input)
			End If
			Select Case input.rank()
				Case 2
					If Not maskArray.ColumnVectorOrScalar OrElse maskArray.size(0) <> input.size(0) Then
						Throw New System.InvalidOperationException("Expected column vector for mask with 2d input, with same size(0)" & " as input. Got mask with shape: " & Arrays.toString(maskArray.shape()) & ", input shape = " & Arrays.toString(input.shape()))
					End If
					Return workspaceMgr.leverageTo(type, input.mulColumnVector(maskArray))
				Case 3
					'Time series input, shape [Minibatch, size, tsLength], Expect rank 2 mask
					If maskArray.rank() <> 2 OrElse input.size(0) <> maskArray.size(0) OrElse input.size(2) <> maskArray.size(1) Then
						Throw New System.InvalidOperationException("With 3d (time series) input with shape [minibatch, size, sequenceLength]=" & Arrays.toString(input.shape()) & ", expected 2d mask array with shape [minibatch, sequenceLength]." & " Got mask with shape: " & Arrays.toString(maskArray.shape()))
					End If
					Dim fwd As INDArray = workspaceMgr.createUninitialized(type, input.dataType(), input.shape(), "f"c)
					Broadcast.mul(input, maskArray, fwd, 0, 2)
					Return fwd
				Case 4
					'CNN input. Expect column vector to be shape [mb,1,h,1], [mb,1,1,w], or [mb,1,h,w]
					Dim dimensions(3) As Integer
					Dim count As Integer = 0
					For i As Integer = 0 To 3
						If input.size(i) = maskArray.size(i) Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: dimensions[count++] = i;
							dimensions(count) = i
								count += 1
						End If
					Next i
					If count < 4 Then
						dimensions = Arrays.CopyOfRange(dimensions, 0, count)
					End If

					Dim fwd2 As INDArray = workspaceMgr.createUninitialized(type, input.dataType(), input.shape(), "c"c)
					Broadcast.mul(input, maskArray, fwd2, dimensions)
					Return fwd2
				Case Else
					Throw New Exception("Expected rank 2 to 4 input. Got rank " & input.rank() & " with shape " & Arrays.toString(input.shape()))
			End Select
		End Function

	End Class

End Namespace