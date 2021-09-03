Imports System
Imports NonNull = lombok.NonNull
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports BaseWrapperLayer = org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer
Imports TimeSeriesUtils = org.deeplearning4j.util.TimeSeriesUtils
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports org.nd4j.common.primitives
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
import static org.nd4j.linalg.indexing.NDArrayIndex.all
import static org.nd4j.linalg.indexing.NDArrayIndex.point

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
	Public Class LastTimeStepLayer
		Inherits BaseWrapperLayer

		Private lastTimeStepIdxs() As Integer
		Private origOutputShape() As Long

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public LastTimeStepLayer(@NonNull Layer underlying)
		Public Sub New(ByVal underlying As Layer)
			MyBase.New(underlying)
		End Sub

		Public Overrides Function type() As Type
			Return Type.FEED_FORWARD
		End Function

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			Dim newEpsShape() As Long = origOutputShape

			Dim nwc As Boolean = TimeSeriesUtils.getFormatFromRnnLayer(underlying.conf().getLayer()) = RNNFormat.NWC
			Dim newEps As INDArray = Nd4j.create(epsilon.dataType(), newEpsShape, "f"c)
			If lastTimeStepIdxs Is Nothing Then
				'no mask case
				If nwc Then
					newEps.put(New INDArrayIndex(){all(), point(origOutputShape(1)-1), all()}, epsilon)
				Else
					newEps.put(New INDArrayIndex(){all(), all(), point(origOutputShape(2)-1)}, epsilon)
				End If
			Else
				If nwc Then
					Dim arr() As INDArrayIndex = {Nothing, Nothing, all()}
					'TODO probably possible to optimize this with reshape + scatter ops...
					For i As Integer = 0 To lastTimeStepIdxs.Length - 1
						arr(0) = point(i)
						arr(1) = point(lastTimeStepIdxs(i))
						newEps.put(arr, epsilon.getRow(i))
					Next i
				Else
					Dim arr() As INDArrayIndex = {Nothing, all(), Nothing}
					'TODO probably possible to optimize this with reshape + scatter ops...
					For i As Integer = 0 To lastTimeStepIdxs.Length - 1
						arr(0) = point(i)
						arr(2) = point(lastTimeStepIdxs(i))
						newEps.put(arr, epsilon.getRow(i))
					Next i
				End If

			End If
			Return underlying.backpropGradient(newEps, workspaceMgr)
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Return getLastStep(underlying.activate(training, workspaceMgr), workspaceMgr, ArrayType.ACTIVATIONS)
		End Function

		Public Overrides Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim a As INDArray = underlying.activate(input, training, workspaceMgr)
			Return getLastStep(a, workspaceMgr, ArrayType.ACTIVATIONS)
		End Function


		Public Overrides Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			underlying.feedForwardMaskArray(maskArray, currentMaskState, minibatchSize)
			Me.MaskArray = maskArray

			'Input: 2d mask array, for masking a time series. After extracting out the last time step, we no longer need the mask array
			Return New Pair(Of INDArray, MaskState)(Nothing, currentMaskState)
		End Function


		Private Function getLastStep(ByVal [in] As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal arrayType As ArrayType) As INDArray
			If [in].rank() <> 3 Then
				Throw New System.ArgumentException("Expected rank 3 input with shape [minibatch, layerSize, tsLength]. Got " & "rank " & [in].rank() & " with shape " & Arrays.toString([in].shape()))
			End If
			origOutputShape = [in].shape()
			Dim nwc As Boolean = TimeSeriesUtils.getFormatFromRnnLayer(underlying.conf().getLayer()) = RNNFormat.NWC
	'        underlying instanceof  BaseRecurrentLayer && ((BaseRecurrentLayer)underlying).getDataFormat() == RNNFormat.NWC)||
	'                underlying instanceof MaskZeroLayer && ((MaskZeroLayer)underlying).getUnderlying() instanceof BaseRecurrentLayer &&
	'                        ((BaseRecurrentLayer)((MaskZeroLayer)underlying).getUnderlying()).getDataFormat() == RNNFormat.NWC;
			If nwc Then
				[in] = [in].permute(0, 2, 1)
			End If

			Dim mask As INDArray = underlying.MaskArray
			Dim p As Pair(Of INDArray, Integer()) = TimeSeriesUtils.pullLastTimeSteps([in], mask, workspaceMgr, arrayType)
			lastTimeStepIdxs = p.Second

			Return p.First
		End Function
	End Class

End Namespace