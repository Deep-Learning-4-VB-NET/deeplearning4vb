Imports System
Imports System.Collections.Generic
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports LocalResponseNormalizationHelper = org.deeplearning4j.nn.layers.normalization.LocalResponseNormalizationHelper
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports LocalResponseNormalization = org.nd4j.linalg.api.ops.impl.layers.convolution.LocalResponseNormalization
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.nn.layers.mkldnn


	Public Class MKLDNNLocalResponseNormalizationHelper
		Inherits BaseMKLDNNHelper
		Implements LocalResponseNormalizationHelper

		Protected Friend context As OpContext

		Public Sub New(ByVal dataType As DataType)

		End Sub

		Public Overridable Function checkSupported(ByVal k As Double, ByVal n As Double, ByVal alpha As Double, ByVal beta As Double) As Boolean Implements LocalResponseNormalizationHelper.checkSupported
			Return BaseMKLDNNHelper.mklDnnEnabled()
		End Function

		Public Overridable Function backpropGradient(ByVal input As INDArray, ByVal epsilon As INDArray, ByVal k As Double, ByVal n As Double, ByVal alpha As Double, ByVal beta As Double, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray) Implements LocalResponseNormalizationHelper.backpropGradient
			Dim gradAtInput As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, input.dataType(), input.shape())

			If context Is Nothing Then
				context = Nd4j.Executioner.buildContext()
				context.setTArguments(k, alpha, beta)
				context.IArguments = CInt(Math.Truncate(n))
			Else
				context.purge()
			End If

			Dim op As New LocalResponseNormalization()

			context.setInputArray(0, input)
			context.setInputArray(0, epsilon)
			context.setOutputArray(0, gradAtInput)

			Nd4j.exec(op, context)
			Dim g As Gradient = New DefaultGradient()
			Return New Pair(Of Gradient, INDArray)(g, gradAtInput)
		End Function

		Public Overridable Function activate(ByVal x As INDArray, ByVal training As Boolean, ByVal k As Double, ByVal n As Double, ByVal alpha As Double, ByVal beta As Double, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements LocalResponseNormalizationHelper.activate
			Dim [out] As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, x.dataType(), x.shape())

			If context Is Nothing Then
				context = Nd4j.Executioner.buildContext()
				context.setTArguments(k, alpha, beta)
				context.IArguments = CInt(Math.Truncate(n))
			Else
				context.purge()
			End If

			context.setInputArray(0, x)
			context.setOutputArray(0, [out])

			Dim op As New LocalResponseNormalization()

			Nd4j.exec(op, context)
			Return [out]
		End Function

		Public Overridable Function helperMemoryUse() As IDictionary(Of String, Long)
			Return Collections.emptyMap()
		End Function

		Public Overridable Function checkSupported() As Boolean
			Return BaseMKLDNNHelper.mklDnnEnabled()
		End Function
	End Class

End Namespace