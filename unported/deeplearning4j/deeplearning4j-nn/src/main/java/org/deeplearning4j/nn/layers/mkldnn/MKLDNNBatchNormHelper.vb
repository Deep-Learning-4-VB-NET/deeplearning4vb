Imports System.Collections.Generic
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports BatchNormalizationHelper = org.deeplearning4j.nn.layers.normalization.BatchNormalizationHelper
Imports BatchNormalizationParamInitializer = org.deeplearning4j.nn.params.BatchNormalizationParamInitializer
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports BatchNorm = org.nd4j.linalg.api.ops.impl.layers.convolution.BatchNorm
Imports Variance = org.nd4j.linalg.api.ops.impl.summarystats.Variance
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.nn.layers.mkldnn


	Public Class MKLDNNBatchNormHelper
		Implements BatchNormalizationHelper

		Private Shared ReadOnly RANK2_DIMS() As Integer = {0}
		Private Shared ReadOnly RANK4_DIMS_NCHW() As Integer = {0, 2, 3}
		Private Shared ReadOnly RANK4_DIMS_NHWC() As Integer = {0, 1, 2}

		Protected Friend context As OpContext
		Private meanCache As INDArray
		Private varCache As INDArray

		Public Sub New(ByVal dataType As DataType)

		End Sub

		Public Overridable Function checkSupported(ByVal eps As Double, ByVal fixedGammaBeta As Boolean) As Boolean Implements BatchNormalizationHelper.checkSupported
			Return Not fixedGammaBeta AndAlso BaseMKLDNNHelper.mklDnnEnabled()
		End Function

		Public Overridable Function backpropGradient(ByVal input As INDArray, ByVal epsilon As INDArray, ByVal shape() As Long, ByVal gamma As INDArray, ByVal beta As INDArray, ByVal dGammaView As INDArray, ByVal dBetaView As INDArray, ByVal eps As Double, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray) Implements BatchNormalizationHelper.backpropGradient

			'Workaround for: https://github.com/eclipse/deeplearning4j/issues/8860
			If Not Shape.hasDefaultStridesForShape(epsilon) Then
				epsilon = epsilon.dup("c"c)
			End If

			If input.dataType() <> DataType.FLOAT Then
				Return Nothing 'MKL-DNN only supports float
			End If

			Dim axis As Integer = If(input.rank() <> 4 OrElse format = CNN2DFormat.NCHW, 1, 3)

			Dim args As IList(Of INDArray) = New List(Of INDArray)()
			args.Add(input)
			args.Add(meanCache)
			args.Add(varCache)
			If gamma IsNot Nothing Then
				args.Add(gamma.reshape(ChrW(gamma.length())))
			End If
			If beta IsNot Nothing Then
				args.Add(beta.reshape(ChrW(beta.length())))
			End If
			args.Add(epsilon)


			Dim op As DynamicCustomOp = DynamicCustomOp.builder("batchnorm_bp").addInputs(CType(args, List(Of INDArray)).ToArray()).addIntegerArguments(If(gamma Is Nothing, 0, 1),If(beta Is Nothing, 0, 1), axis).addFloatingPointArguments(eps).build()

			Dim epsAtInput As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, input.dataType(), input.shape())
			Dim dLdm As INDArray = workspaceMgr.createUninitialized(ArrayType.BP_WORKING_MEM, meanCache.dataType(), meanCache.shape())
			Dim dLdv As INDArray = workspaceMgr.createUninitialized(ArrayType.BP_WORKING_MEM, meanCache.dataType(), meanCache.shape())

			op.setOutputArgument(0, epsAtInput)
			op.setOutputArgument(1, dLdm)
			op.setOutputArgument(2, dLdv)
			If dGammaView IsNot Nothing Then
				'Both are always null/not null simultaneously
				op.setOutputArgument(3, dGammaView.reshape(ChrW(dGammaView.length())))
				op.setOutputArgument(4, dBetaView.reshape(ChrW(dBetaView.length())))
			End If


			Nd4j.exec(op)

			Dim g As Gradient = New DefaultGradient()
			g.setGradientFor(BatchNormalizationParamInitializer.GAMMA, dGammaView)
			g.setGradientFor(BatchNormalizationParamInitializer.BETA, dBetaView)

			Return New Pair(Of Gradient, INDArray)(g, epsAtInput)
		End Function

		Public Overridable Function preOutput(ByVal x As INDArray, ByVal training As Boolean, ByVal shape() As Long, ByVal gamma As INDArray, ByVal beta As INDArray, ByVal mean As INDArray, ByVal var As INDArray, ByVal decay As Double, ByVal eps As Double, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements BatchNormalizationHelper.preOutput
			If x.dataType() <> DataType.FLOAT Then
				Return Nothing 'MKL-DNN only supports float
			End If

			Dim axis As Integer = If(x.rank() <> 4 OrElse format = CNN2DFormat.NCHW, 1, 3)

			If context Is Nothing Then
				context = Nd4j.Executioner.buildContext()
				context.setIArguments(ArrayUtil.fromBoolean(gamma IsNot Nothing), ArrayUtil.fromBoolean(beta IsNot Nothing), axis) 'Axis - 1 = NCHW, 3 = NHWC
				context.TArguments = eps
			End If

			'Mean and variance: args here are *global*. Depending on train/test mode we might need to use batch mean/var
			Dim m, v As INDArray
			If training Then
				If meanCache Is Nothing Then
					Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
						meanCache = Nd4j.createUninitialized(x.dataType(), x.size(axis))
						varCache = Nd4j.createUninitialized(x.dataType(), x.size(axis))
					End Using
				End If

				Dim dims() As Integer
				If x.rank() = 2 Then
					dims = RANK2_DIMS
				ElseIf format = CNN2DFormat.NCHW Then
					dims = RANK4_DIMS_NCHW
				Else
					dims = RANK4_DIMS_NHWC
				End If

				x.mean(meanCache, dims)
				Nd4j.exec(New Variance(x, varCache, False, dims))

				m = meanCache
				v = varCache
			Else
				m = mean.reshape(ChrW(mean.length()))
				v = var.reshape(ChrW(var.length()))
			End If

			'Note: batchnorm op expects rank 1 inputs for mean/var etc, not rank 2 shape [1,x]
			context.purge()
			context.setInputArray(0, x)
			context.setInputArray(1, m)
			context.setInputArray(2, v)
			If gamma IsNot Nothing AndAlso beta IsNot Nothing Then
				context.setInputArray(3,If(gamma.rank() = 2, gamma.reshape(ChrW(gamma.length())), gamma))
				context.setInputArray(4,If(beta.rank() = 2, beta.reshape(ChrW(beta.length())), beta))
			End If

			Dim [out] As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, x.dataType(), x.shape())
			context.setOutputArray(0, [out])

			Dim bn As New BatchNorm()
			Nd4j.exec(bn, context)
			Return [out]
		End Function

		Public Overridable Function getMeanCache(ByVal dataType As DataType) As INDArray Implements BatchNormalizationHelper.getMeanCache
			Return meanCache
		End Function

		Public Overridable Function getVarCache(ByVal dataType As DataType) As INDArray Implements BatchNormalizationHelper.getVarCache
			Return varCache
		End Function

		Public Overridable Function helperMemoryUse() As IDictionary(Of String, Long)
			Return Collections.emptyMap()
		End Function

		Public Overridable Function checkSupported() As Boolean
			Return BaseMKLDNNHelper.mklDnnEnabled()
		End Function
	End Class

End Namespace