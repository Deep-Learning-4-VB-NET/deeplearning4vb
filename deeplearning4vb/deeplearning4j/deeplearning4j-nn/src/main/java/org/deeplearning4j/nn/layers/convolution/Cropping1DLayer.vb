Imports System
Imports val = lombok.val
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Cropping1D = org.deeplearning4j.nn.conf.layers.convolutional.Cropping1D
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
import static org.nd4j.linalg.indexing.NDArrayIndex.all
import static org.nd4j.linalg.indexing.NDArrayIndex.interval

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

Namespace org.deeplearning4j.nn.layers.convolution

	<Serializable>
	Public Class Cropping1DLayer
		Inherits AbstractLayer(Of Cropping1D)

		Private cropping() As Integer '[padTop, padBottom]

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
			Me.cropping = CType(conf.getLayer(), Cropping1D).getCropping()
		End Sub

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Sub clearNoiseWeightParams()
			'No op
		End Sub

		Public Overrides Function type() As Type
			Return Type.CONVOLUTIONAL
		End Function

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			Dim inShape As val = input_Conflict.shape()
			Dim epsNext As INDArray = workspaceMgr.create(ArrayType.ACTIVATION_GRAD, dataType, inShape, "c"c)
			Dim epsNextSubset As INDArray = epsNext.get(all(), all(), interval(cropping(0), epsNext.size(2)-cropping(1)))
			epsNextSubset.assign(epsilon)
			Return New Pair(Of Gradient, INDArray)(DirectCast(New DefaultGradient(), Gradient), epsNext)
		End Function


		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
			Return inputSubset(input_Conflict, ArrayType.ACTIVATIONS, workspaceMgr)
		End Function

		Public Overrides Function clone() As Layer
			Return New Cropping2DLayer(conf_Conflict.clone(), dataType)
		End Function

		Public Overrides Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double
			Return 0.0
		End Function

		Private Function inputSubset(ByVal from As INDArray, ByVal arrayType As ArrayType, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(arrayType)
				If from.dataType() = dataType Then
					Return from.get(all(), all(), interval(cropping(0), from.size(2)-cropping(1))).dup(from.ordering())
				Else
					Return from.get(all(), all(), interval(cropping(0), from.size(2)-cropping(1))).castTo(dataType)
				End If
			End Using
		End Function
	End Class

End Namespace