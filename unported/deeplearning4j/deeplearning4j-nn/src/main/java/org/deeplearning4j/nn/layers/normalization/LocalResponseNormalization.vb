Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports HelperUtils = org.deeplearning4j.nn.layers.HelperUtils
Imports LayerHelper = org.deeplearning4j.nn.layers.LayerHelper
Imports MKLDNNLocalResponseNormalizationHelper = org.deeplearning4j.nn.layers.mkldnn.MKLDNNLocalResponseNormalizationHelper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MulOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.MulOp
Imports ND4JOpProfilerException = org.nd4j.linalg.exception.ND4JOpProfilerException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports org.nd4j.common.primitives
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

Namespace org.deeplearning4j.nn.layers.normalization

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class LocalResponseNormalization extends org.deeplearning4j.nn.layers.AbstractLayer<org.deeplearning4j.nn.conf.layers.LocalResponseNormalization>
	<Serializable>
	Public Class LocalResponseNormalization
		Inherits AbstractLayer(Of org.deeplearning4j.nn.conf.layers.LocalResponseNormalization)

'JAVA TO VB CONVERTER NOTE: The field helper was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend helper_Conflict As LocalResponseNormalizationHelper = Nothing
		Protected Friend helperCountFail As Integer = 0
		Public Const LOCAL_RESPONSE_NORM_CUDNN_HELPER_CLASS_NAME As String = "org.deeplearning4j.cuda.normalization.CudnnLocalResponseNormalizationHelper"
		Public Overrides Function clone() As Layer
			Return New LocalResponseNormalization(conf_Conflict.clone(), dataType)
		End Function

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
			initializeHelper()
		End Sub

		Friend Overridable Sub initializeHelper()
			Dim backend As String = Nd4j.Executioner.EnvironmentInformation.getProperty("backend")
			If "CUDA".Equals(backend, StringComparison.OrdinalIgnoreCase) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				helper_Conflict = HelperUtils.createHelper(LOCAL_RESPONSE_NORM_CUDNN_HELPER_CLASS_NAME, GetType(MKLDNNLocalResponseNormalizationHelper).FullName, GetType(LocalResponseNormalizationHelper), layerConf().LayerName, dataType)
			End If
			'2019-03-09 AB - MKL-DNN helper disabled: https://github.com/eclipse/deeplearning4j/issues/7272
	'        else if("CPU".equalsIgnoreCase(backend)){
	'            helper = new MKLDNNLocalResponseNormalizationHelper();
	'            log.debug("Created MKLDNNLocalResponseNormalizationHelper");
	'        }
			If helper_Conflict IsNot Nothing AndAlso Not helper_Conflict.checkSupported(layerConf().getK(), layerConf().getN(), layerConf().getAlpha(), layerConf().getBeta()) Then
				log.debug("Removed helper {} as not supported (k={}, n={}, alpha={}, beta={})", helper_Conflict.GetType(), layerConf().getK(), layerConf().getN(), layerConf().getAlpha(), layerConf().getBeta())
				helper_Conflict = Nothing
			End If
		End Sub

		Public Overrides Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double
			Return 0
		End Function

		Public Overrides Function type() As Type
			Return Type.NORMALIZATION
		End Function

		Public Overrides Sub fit(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)

			Dim k As Double = layerConf().getK()
			Dim n As Double = layerConf().getN()
			Dim alpha As Double = layerConf().getAlpha()
			Dim beta As Double = layerConf().getBeta()
			Dim halfN As Integer = CInt(Math.Truncate(n)) \ 2

			If helper_Conflict IsNot Nothing AndAlso (helperCountFail = 0 OrElse Not layerConf().isCudnnAllowFallback()) Then
				Dim ret As Pair(Of Gradient, INDArray) = Nothing
				Try
					ret = helper_Conflict.backpropGradient(input_Conflict, epsilon, k, n, alpha, beta, workspaceMgr)
				Catch e As ND4JOpProfilerException
					Throw e 'NaN panic etc for debugging
				Catch t As Exception
					If t.getMessage() IsNot Nothing AndAlso t.getMessage().contains("Failed to allocate") Then
						'This is a memory exception - don't fallback to built-in implementation
						Throw t
					End If
					If layerConf().isCudnnAllowFallback() Then
						helperCountFail += 1
						log.warn("CuDNN LocalResponseNormalization backprop execution failed - falling back on built-in implementation",t)
					Else
						Throw New Exception("Error during LocalResponseNormalization CuDNN helper backprop - isCudnnAllowFallback() is set to false", t)
					End If
				End Try
				If ret IsNot Nothing Then
					Return ret
				End If
			End If

			Dim nchw As Boolean = layerConf().getDataFormat() = CNN2DFormat.NCHW
			Dim chDim As Integer = If(nchw, 1, 3)
			Dim hDim As Integer = If(nchw, 2, 1)
			Dim wDim As Integer = If(nchw, 3, 2)

			Dim triple As Triple(Of INDArray, INDArray, INDArray) = activateHelper(True, workspaceMgr, True)
			Dim activations As INDArray = triple.getFirst()
			Dim unitScale As INDArray = triple.getSecond()
			Dim scale As INDArray = triple.getThird()

			Dim channel As val = input_Conflict.size(chDim)
			Dim tmp, addVal As INDArray
			Dim retGradient As Gradient = New DefaultGradient()
			Dim reverse As INDArray = activations.mul(epsilon)
			Dim sumPart As INDArray = reverse.dup()

			' sumPart = sum(a^j_{x,y} * gb^j_{x,y})
			Dim i As Integer = 1
			Do While i < halfN + 1
				If nchw Then
					tmp = sumPart.get(NDArrayIndex.all(), interval(i, channel), NDArrayIndex.all(), NDArrayIndex.all())
					addVal = reverse.get(NDArrayIndex.all(), interval(0, channel - i), NDArrayIndex.all(), NDArrayIndex.all())
					sumPart.put(New INDArrayIndex(){NDArrayIndex.all(), interval(i, channel), NDArrayIndex.all(), NDArrayIndex.all()}, tmp.addi(addVal))

					tmp = sumPart.get(NDArrayIndex.all(), interval(0, channel - i), NDArrayIndex.all(), NDArrayIndex.all())
					addVal = reverse.get(NDArrayIndex.all(), interval(i, channel), NDArrayIndex.all(), NDArrayIndex.all())
					sumPart.put(New INDArrayIndex(){NDArrayIndex.all(), interval(0, channel - i), NDArrayIndex.all(), NDArrayIndex.all()}, tmp.addi(addVal))
				Else
					tmp = sumPart.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), interval(i, channel))
					addVal = reverse.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), interval(0, channel - i))
					sumPart.put(New INDArrayIndex(){NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), interval(i, channel)}, tmp.addi(addVal))

					tmp = sumPart.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), interval(0, channel - i))
					addVal = reverse.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), interval(i, channel))
					sumPart.put(New INDArrayIndex(){NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), interval(0, channel - i)}, tmp.addi(addVal))
				End If
				i += 1
			Loop

			' gx = gy * unitScale**-beta - 2 * alpha * beta * sumPart/unitScale * a^i_{x,y}    - rearranged for more in-place ops
			Dim nextEpsilon As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, epsilon.dataType(), epsilon.shape(), epsilon.ordering())
			Nd4j.Executioner.exec(New MulOp(epsilon, scale, nextEpsilon))
			nextEpsilon.subi(sumPart.muli(input_Conflict).divi(unitScale).muli(2 * alpha * beta))
			Return New Pair(Of Gradient, INDArray)(retGradient, nextEpsilon)
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Return activateHelper(training, workspaceMgr, False).getFirst()
		End Function

		Private Function activateHelper(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal forBackprop As Boolean) As Triple(Of INDArray, INDArray, INDArray)
			assertInputSet(False)
			Dim k As Double = layerConf().getK()
			Dim n As Double = layerConf().getN()
			Dim alpha As Double = layerConf().getAlpha()
			Dim beta As Double = layerConf().getBeta()
			Dim halfN As Integer = CInt(Math.Truncate(n)) \ 2

			If helper_Conflict IsNot Nothing AndAlso (helperCountFail = 0 OrElse Not layerConf().isCudnnAllowFallback()) Then
				Dim activations As INDArray = Nothing
				Try
					activations = helper_Conflict.activate(input_Conflict, training, k, n, alpha, beta, workspaceMgr)
				Catch e As ND4JOpProfilerException
					Throw e 'NaN panic etc for debugging
				Catch t As Exception
					If t.getMessage() IsNot Nothing AndAlso t.getMessage().contains("Failed to allocate") Then
						'This is a memory exception - don't fallback to built-in implementation
						Throw t
					End If

					If layerConf().isCudnnAllowFallback() Then
						helperCountFail += 1
						log.warn("CuDNN LocalResponseNormalization backprop execution failed - falling back on built-in implementation",t)
					Else
						Throw New Exception("Error during LocalRsponseNormalization CuDNN helper backprop - isCudnnAllowFallback() is set to false", t)
					End If
				End Try
				If activations IsNot Nothing Then
					Return New Triple(Of INDArray, INDArray, INDArray)(activations, Nothing, Nothing)
				End If
			End If

			Dim nchw As Boolean = layerConf().getDataFormat() = CNN2DFormat.NCHW
			Dim chDim As Integer = If(nchw, 1, 3)

			Dim channel As val = input_Conflict.size(chDim)
			Dim tmp, addVal As INDArray
			' x^2 = (a^j_{x,y})^2
			Dim activitySqr As INDArray = input_Conflict.mul(input_Conflict)
			Dim sumPart As INDArray = activitySqr.dup()

			'sum_{j=max(0, i - n/2)}^{max(N-1, i + n/2)} (a^j_{x,y})^2 )
			Dim i As Integer = 1
			Do While i < halfN + 1

				If nchw Then
					tmp = sumPart.get(NDArrayIndex.all(), interval(i, channel), NDArrayIndex.all(), NDArrayIndex.all())
					addVal = activitySqr.get(NDArrayIndex.all(), interval(0, channel - i), NDArrayIndex.all(), NDArrayIndex.all())
					sumPart.put(New INDArrayIndex(){NDArrayIndex.all(), interval(i, channel), NDArrayIndex.all(), NDArrayIndex.all()}, tmp.addi(addVal))

					tmp = sumPart.get(NDArrayIndex.all(), interval(0, channel - i), NDArrayIndex.all(), NDArrayIndex.all())
					addVal = activitySqr.get(NDArrayIndex.all(), interval(i, channel), NDArrayIndex.all(), NDArrayIndex.all())
					sumPart.put(New INDArrayIndex(){NDArrayIndex.all(), interval(0, channel - i), NDArrayIndex.all(), NDArrayIndex.all()}, tmp.addi(addVal))
				Else
					tmp = sumPart.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), interval(i, channel))
					addVal = activitySqr.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), interval(0, channel - i))
					sumPart.put(New INDArrayIndex(){NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), interval(i, channel)}, tmp.addi(addVal))

					tmp = sumPart.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), interval(0, channel - i))
					addVal = activitySqr.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), interval(i, channel))
					sumPart.put(New INDArrayIndex(){NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), interval(0, channel - i)}, tmp.addi(addVal))
				End If
				i += 1
			Loop

			Dim unitScale As INDArray = Nothing
			Dim scale As INDArray = Nothing
			Dim activations As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, input_Conflict.dataType(), input_Conflict.shape(), input_Conflict.ordering())
			If forBackprop Then
				' unitScale = (k + alpha * sum_{j=max(0, i - n/2)}^{max(N-1, i + n/2)} (a^j_{x,y})^2 )
				unitScale = sumPart.mul(alpha).addi(k)
				' y = x * unitScale**-beta
				scale = Transforms.pow(unitScale, -beta, True)
				Nd4j.Executioner.exec(New MulOp(input_Conflict, scale, activations))
			Else
				' unitScale = (k + alpha * sum_{j=max(0, i - n/2)}^{max(N-1, i + n/2)} (a^j_{x,y})^2 )
				sumPart.muli(alpha, activations).addi(k)
				Transforms.pow(activations, -beta, False)
				activations.muli(input_Conflict)
			End If
			If forBackprop Then
				Return New Triple(Of INDArray, INDArray, INDArray)(activations, unitScale, scale)
			Else
				Return New Triple(Of INDArray, INDArray, INDArray)(activations, Nothing, Nothing)
			End If
		End Function

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Sub clearNoiseWeightParams()
			'No op
		End Sub

		Public Overrides ReadOnly Property Helper As LayerHelper
			Get
				Return helper_Conflict
			End Get
		End Property

		Public Overrides Function params() As INDArray
			Return Nothing
		End Function

		Public Overrides Function getParam(ByVal param As String) As INDArray
			Return params()
		End Function

		Public Overrides WriteOnly Property Params As INDArray
			Set(ByVal params As INDArray)
    
			End Set
		End Property
	End Class

End Namespace