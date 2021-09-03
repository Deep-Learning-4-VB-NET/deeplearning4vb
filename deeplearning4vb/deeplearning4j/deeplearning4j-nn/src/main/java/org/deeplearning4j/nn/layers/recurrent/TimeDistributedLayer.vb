Imports System
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports BaseWrapperLayer = org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports TimeSeriesUtils = org.deeplearning4j.util.TimeSeriesUtils
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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

Namespace org.deeplearning4j.nn.layers.recurrent

	<Serializable>
	Public Class TimeDistributedLayer
		Inherits BaseWrapperLayer

		Private rnnDataFormat As RNNFormat

		Public Sub New(ByVal underlying As Layer, ByVal rnnDataFormat As RNNFormat)
			MyBase.New(underlying)
			Me.rnnDataFormat = rnnDataFormat
		End Sub


		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			Dim reshapedEps As INDArray = reshape(epsilon)
			Dim p As Pair(Of Gradient, INDArray) = underlying.backpropGradient(reshapedEps, workspaceMgr)
			Dim reverted As INDArray = revertReshape(p.Second, epsilon.size(0))
			reverted = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, reverted)
			p.Second = reverted
			Return p
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Return activate(input(), training, workspaceMgr)
		End Function

		Public Overrides Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim reshaped As INDArray = reshape(input)
			Dim [out] As INDArray = underlying.activate(reshaped, training, workspaceMgr)
			Dim ret As INDArray = revertReshape([out], input.size(0))
			Return workspaceMgr.dup(ArrayType.ACTIVATIONS, ret)
		End Function

		Protected Friend Overridable Function reshape(ByVal array As INDArray) As INDArray
			'Reshape the time axis to the minibatch axis
			'For example, for RNN -> FF (dense time distributed): [mb, size, seqLen] -> [mb x seqLen, size]
			Dim axis As Integer = If(rnnDataFormat = RNNFormat.NCW, 2, 1)
			If axis < 0 Then
				axis += array.rank()
			End If

			Dim permuteAxis() As Integer = permuteAxes(array.rank(), axis)
			Dim permute As INDArray = array.permute(permuteAxis)

			Dim newShape(array.rank() - 2) As Long
			newShape(0) = array.size(0) * array.size(axis)
			Dim j As Integer=1
			Dim i As Integer=1
			Do While i<array.rank()
				If axis = i Then
					i += 1
					Continue Do
				End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: newShape[j++] = array.size(i);
				newShape(j) = array.size(i)
					j += 1
				i += 1
			Loop

'JAVA TO VB CONVERTER NOTE: The local variable reshape was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim reshape_Conflict As INDArray = permute.dup().reshape("c"c, newShape)
			Return reshape_Conflict
		End Function

		Protected Friend Overridable Function permuteAxes(ByVal rank As Integer, ByVal timeAxis As Integer) As Integer()
			Dim permuteAxis(rank - 1) As Integer
			permuteAxis(0) = 0
			permuteAxis(1) = timeAxis
			Dim j As Integer=2
			For i As Integer = 1 To rank - 1
				If timeAxis = i Then
					Continue For
				End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: permuteAxis[j++] = i;
				permuteAxis(j) = i
					j += 1
			Next i
			Return permuteAxis
		End Function

		Protected Friend Overridable Function revertReshape(ByVal toRevert As INDArray, ByVal minibatch As Long) As INDArray

			Dim axis As Integer = If(rnnDataFormat = RNNFormat.NCW, 2, 1)
			If axis < 0 Then
				axis += (toRevert.rank()+1)
			End If

			Dim newShape(toRevert.rank()) As Long
			newShape(0) = minibatch
			newShape(1) = toRevert.size(0)\minibatch
			Dim i As Integer=1
			Do While i<toRevert.rank()
				newShape(i+1) = toRevert.size(i)
				i += 1
			Loop

			Dim reshaped As INDArray = toRevert.reshape("c"c, newShape)

			Dim permute() As Integer = ArrayUtil.invertPermutation(permuteAxes(toRevert.rank() + 1, axis))

			Dim permuted As INDArray = reshaped.permute(permute)
			Return permuted
		End Function

		Public Overrides WriteOnly Property MaskArray As INDArray
			Set(ByVal maskArray As INDArray)
				If maskArray Is Nothing Then
					underlying.MaskArray = Nothing
				Else
					Dim reshaped As INDArray = TimeSeriesUtils.reshapeTimeSeriesMaskToVector(maskArray, LayerWorkspaceMgr.noWorkspaces(), ArrayType.ACTIVATIONS)
					underlying.MaskArray = reshaped
				End If
			End Set
		End Property

		Public Overrides Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			If maskArray Is Nothing Then
				Return underlying.feedForwardMaskArray(Nothing, currentMaskState, minibatchSize)
			Else
				Dim reshaped As INDArray = TimeSeriesUtils.reshapeTimeSeriesMaskToVector(maskArray, LayerWorkspaceMgr.noWorkspaces(), ArrayType.ACTIVATIONS)
				Dim p As Pair(Of INDArray, MaskState) = underlying.feedForwardMaskArray(reshaped, currentMaskState, minibatchSize)
				If p Is Nothing OrElse p.First Is Nothing Then
					Return p
				End If
				Dim reshaped2 As INDArray = TimeSeriesUtils.reshapeVectorToTimeSeriesMask(p.First, CInt(maskArray.size(0)))
				p.First = reshaped2
				Return p
			End If
		End Function
	End Class

End Namespace