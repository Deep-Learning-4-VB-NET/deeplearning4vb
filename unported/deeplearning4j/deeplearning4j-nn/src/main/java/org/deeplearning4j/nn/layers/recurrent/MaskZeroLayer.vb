Imports System
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports BaseWrapperLayer = org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
Imports NonNull = lombok.NonNull
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
import static org.deeplearning4j.nn.conf.RNNFormat.NWC

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
	Public Class MaskZeroLayer
		Inherits BaseWrapperLayer

		Private Const serialVersionUID As Long = -7369482676002469854L
		Private maskingValue As Double

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MaskZeroLayer(@NonNull Layer underlying, double maskingValue)
		Public Sub New(ByVal underlying As Layer, ByVal maskingValue As Double)
			MyBase.New(underlying)
			Me.maskingValue = maskingValue
		End Sub


		Public Overrides Function type() As Type
			Return Type.RECURRENT
		End Function

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			Return underlying.backpropGradient(epsilon, workspaceMgr)
		End Function


		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim input As INDArray = Me.input()
			MaskFromInput = input
			Return underlying.activate(training, workspaceMgr)
		End Function

		Public Overrides Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			MaskFromInput = input
			Return underlying.activate(input, training, workspaceMgr)
		End Function

		Private WriteOnly Property MaskFromInput As INDArray
			Set(ByVal input As INDArray)
				If input.rank() <> 3 Then
					Throw New System.ArgumentException("Expected input of shape [batch_size, timestep_input_size, timestep], " & "got shape " & Arrays.toString(input.shape()) & " instead")
				End If
				If (TypeOf underlying Is BaseRecurrentLayer AndAlso DirectCast(underlying, BaseRecurrentLayer).getDataFormat() = NWC) Then
					input = input.permute(0, 2, 1)
				End If
				Dim mask As INDArray = input.eq(maskingValue).castTo(input.dataType()).sum(1).neq(input.shape()(1)).castTo(input.dataType())
				underlying.MaskArray = mask.detach()
			End Set
		End Property

		Public Overrides Function numParams() As Long
			Return underlying.numParams()
		End Function

		Public Overrides Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			underlying.feedForwardMaskArray(maskArray, currentMaskState, minibatchSize)

			'Input: 2d mask array, for masking a time series. After extracting out the last time step,
			' we no longer need the mask array
			Return New Pair(Of INDArray, MaskState)(Nothing, currentMaskState)
		End Function


	End Class

End Namespace