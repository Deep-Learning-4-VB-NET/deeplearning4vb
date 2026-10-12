Imports System
Imports FastMath = org.apache.commons.math3.util.FastMath
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports TruncatedNormalDistribution = org.nd4j.linalg.api.ops.random.impl.TruncatedNormalDistribution
Imports Distribution = org.nd4j.linalg.api.rng.distribution.Distribution
Imports OrthogonalDistribution = org.nd4j.linalg.api.rng.distribution.impl.OrthogonalDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.nn.weights




	''' <summary>
	''' Weight initialization utility
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class WeightInitUtil

		''' <summary>
		''' Default order for the arrays created by WeightInitUtil.
		''' </summary>
		Public Const DEFAULT_WEIGHT_INIT_ORDER As Char = "f"c

		Private Sub New()
		End Sub


		''' <summary>
		''' Initializes a matrix with the given weight initialization scheme.
		''' Note: Defaults to fortran ('f') order arrays for the weights. Use <seealso cref="initWeights(Integer[], WeightInit, Distribution, Char, INDArray)"/>
		''' to control this
		''' </summary>
		''' <param name="shape">      the shape of the matrix </param>
		''' <param name="initScheme"> the scheme to use </param>
		''' <returns> a matrix of the specified dimensions with the specified
		''' distribution based on the initialization scheme </returns>
		<Obsolete>
		Public Shared Function initWeights(ByVal fanIn As Double, ByVal fanOut As Double, ByVal shape() As Integer, ByVal initScheme As WeightInit, ByVal dist As Distribution, ByVal paramView As INDArray) As INDArray
			Return initWeights(fanIn, fanOut, ArrayUtil.toLongArray(shape), initScheme, dist, DEFAULT_WEIGHT_INIT_ORDER, paramView)
		End Function

		''' <summary>
		''' Initializes a matrix with the given weight initialization scheme.
		''' Note: Defaults to fortran ('f') order arrays for the weights. Use <seealso cref="initWeights(Long[], WeightInit, Distribution, Char, INDArray)"/>
		''' to control this
		''' </summary>
		''' <param name="shape">      the shape of the matrix </param>
		''' <param name="initScheme"> the scheme to use </param>
		''' <returns> a matrix of the specified dimensions with the specified
		''' distribution based on the initialization scheme </returns>
		Public Shared Function initWeights(ByVal fanIn As Double, ByVal fanOut As Double, ByVal shape() As Long, ByVal initScheme As WeightInit, ByVal dist As Distribution, ByVal paramView As INDArray) As INDArray
			Return initWeights(fanIn, fanOut, shape, initScheme, dist, DEFAULT_WEIGHT_INIT_ORDER, paramView)
		End Function

		<Obsolete>
		Public Shared Function initWeights(ByVal fanIn As Double, ByVal fanOut As Double, ByVal shape() As Integer, ByVal initScheme As WeightInit, ByVal dist As Distribution, ByVal order As Char, ByVal paramView As INDArray) As INDArray
			Return initWeights(fanIn, fanOut, ArrayUtil.toLongArray(shape), initScheme, dist, order, paramView)
		End Function

		Public Shared Function initWeights(ByVal fanIn As Double, ByVal fanOut As Double, ByVal shape() As Long, ByVal initScheme As WeightInit, ByVal dist As Distribution, ByVal order As Char, ByVal paramView As INDArray) As INDArray
			Select Case initScheme.innerEnumValue
				Case org.deeplearning4j.nn.weights.WeightInit.InnerEnum.DISTRIBUTION
					If TypeOf dist Is OrthogonalDistribution Then
						dist.sample(paramView.reshape(order, shape))
					Else
						dist.sample(paramView)
					End If
				Case org.deeplearning4j.nn.weights.WeightInit.InnerEnum.RELU
					Nd4j.randn(paramView).muli(FastMath.sqrt(2.0 / fanIn)) 'N(0, 2/nIn)
				Case org.deeplearning4j.nn.weights.WeightInit.InnerEnum.RELU_UNIFORM
					Dim u As Double = Math.Sqrt(6.0 / fanIn)
					Nd4j.rand(paramView, Nd4j.Distributions.createUniform(-u, u)) 'U(-sqrt(6/fanIn), sqrt(6/fanIn)
				Case org.deeplearning4j.nn.weights.WeightInit.InnerEnum.SIGMOID_UNIFORM
					Dim r As Double = 4.0 * Math.Sqrt(6.0 / (fanIn + fanOut))
					Nd4j.rand(paramView, Nd4j.Distributions.createUniform(-r, r))
				Case org.deeplearning4j.nn.weights.WeightInit.InnerEnum.UNIFORM
					Dim a As Double = 1.0 / Math.Sqrt(fanIn)
					Nd4j.rand(paramView, Nd4j.Distributions.createUniform(-a, a))
				Case org.deeplearning4j.nn.weights.WeightInit.InnerEnum.LECUN_UNIFORM
					Dim b As Double = 3.0 / Math.Sqrt(fanIn)
					Nd4j.rand(paramView, Nd4j.Distributions.createUniform(-b, b))
				Case org.deeplearning4j.nn.weights.WeightInit.InnerEnum.XAVIER
					Nd4j.randn(paramView).muli(FastMath.sqrt(2.0 / (fanIn + fanOut)))
				Case org.deeplearning4j.nn.weights.WeightInit.InnerEnum.XAVIER_UNIFORM
					'As per Glorot and Bengio 2010: Uniform distribution U(-s,s) with s = sqrt(6/(fanIn + fanOut))
					'Eq 16: http://jmlr.org/proceedings/papers/v9/glorot10a/glorot10a.pdf
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim s As Double = Math.Sqrt(6.0) / Math.Sqrt(fanIn + fanOut)
					Nd4j.rand(paramView, Nd4j.Distributions.createUniform(-s, s))
				Case org.deeplearning4j.nn.weights.WeightInit.InnerEnum.LECUN_NORMAL, NORMAL, XAVIER_FAN_IN 'Fall through: these 3 are equivalent
					Nd4j.randn(paramView).divi(FastMath.sqrt(fanIn))
				Case org.deeplearning4j.nn.weights.WeightInit.InnerEnum.XAVIER_LEGACY
					Nd4j.randn(paramView).divi(FastMath.sqrt(shape(0) + shape(1)))
				Case org.deeplearning4j.nn.weights.WeightInit.InnerEnum.ZERO
					paramView.assign(0.0)
				Case org.deeplearning4j.nn.weights.WeightInit.InnerEnum.ONES
					paramView.assign(1.0)
				Case org.deeplearning4j.nn.weights.WeightInit.InnerEnum.IDENTITY
					If shape.Length <> 2 OrElse shape(0) <> shape(1) Then
						Throw New System.InvalidOperationException("Cannot use IDENTITY init with parameters of shape " & Arrays.toString(shape) & ": weights must be a square matrix for identity")
					End If
					Dim ret As INDArray
					If order = Nd4j.order() Then
						ret = Nd4j.eye(shape(0))
					Else
						ret = Nd4j.createUninitialized(shape, order).assign(Nd4j.eye(shape(0)))
					End If
					Dim flat As INDArray = Nd4j.toFlattened(order, ret)
					paramView.assign(flat)
				Case org.deeplearning4j.nn.weights.WeightInit.InnerEnum.VAR_SCALING_NORMAL_FAN_IN
					Nd4j.exec(New TruncatedNormalDistribution(paramView, 0.0, Math.Sqrt(1.0 / fanIn)))
				Case org.deeplearning4j.nn.weights.WeightInit.InnerEnum.VAR_SCALING_NORMAL_FAN_OUT
					Nd4j.exec(New TruncatedNormalDistribution(paramView, 0.0, Math.Sqrt(1.0 / fanOut)))
				Case org.deeplearning4j.nn.weights.WeightInit.InnerEnum.VAR_SCALING_NORMAL_FAN_AVG
					Nd4j.exec(New TruncatedNormalDistribution(paramView, 0.0, Math.Sqrt(2.0 / (fanIn + fanOut))))
				Case org.deeplearning4j.nn.weights.WeightInit.InnerEnum.VAR_SCALING_UNIFORM_FAN_IN
					Dim scalingFanIn As Double = 3.0 / Math.Sqrt(fanIn)
					Nd4j.rand(paramView, Nd4j.Distributions.createUniform(-scalingFanIn, scalingFanIn))
				Case org.deeplearning4j.nn.weights.WeightInit.InnerEnum.VAR_SCALING_UNIFORM_FAN_OUT
					Dim scalingFanOut As Double = 3.0 / Math.Sqrt(fanOut)
					Nd4j.rand(paramView, Nd4j.Distributions.createUniform(-scalingFanOut, scalingFanOut))
				Case org.deeplearning4j.nn.weights.WeightInit.InnerEnum.VAR_SCALING_UNIFORM_FAN_AVG
					Dim scalingFanAvg As Double = 3.0 / Math.Sqrt((fanIn + fanOut) / 2)
					Nd4j.rand(paramView, Nd4j.Distributions.createUniform(-scalingFanAvg, scalingFanAvg))
				Case Else
					Throw New System.InvalidOperationException("Illegal weight init value: " & initScheme)
			End Select

			Return paramView.reshape(order, shape)
		End Function


		''' <summary>
		''' Reshape the parameters view, without modifying the paramsView array values.
		''' </summary>
		''' <param name="shape">      Shape to reshape </param>
		''' <param name="paramsView"> Parameters array view </param>
		Public Shared Function reshapeWeights(ByVal shape() As Integer, ByVal paramsView As INDArray) As INDArray
			Return reshapeWeights(shape, paramsView, DEFAULT_WEIGHT_INIT_ORDER)
		End Function

		''' <summary>
		''' Reshape the parameters view, without modifying the paramsView array values.
		''' </summary>
		''' <param name="shape">      Shape to reshape </param>
		''' <param name="paramsView"> Parameters array view </param>
		Public Shared Function reshapeWeights(ByVal shape() As Long, ByVal paramsView As INDArray) As INDArray
			Return reshapeWeights(shape, paramsView, DEFAULT_WEIGHT_INIT_ORDER)
		End Function

		''' <summary>
		''' Reshape the parameters view, without modifying the paramsView array values.
		''' </summary>
		''' <param name="shape">           Shape to reshape </param>
		''' <param name="paramsView">      Parameters array view </param>
		''' <param name="flatteningOrder"> Order in which parameters are flattened/reshaped </param>
		Public Shared Function reshapeWeights(ByVal shape() As Integer, ByVal paramsView As INDArray, ByVal flatteningOrder As Char) As INDArray
			Return paramsView.reshape(flatteningOrder, shape)
		End Function

		''' <summary>
		''' Reshape the parameters view, without modifying the paramsView array values.
		''' </summary>
		''' <param name="shape">           Shape to reshape </param>
		''' <param name="paramsView">      Parameters array view </param>
		''' <param name="flatteningOrder"> Order in which parameters are flattened/reshaped </param>
		Public Shared Function reshapeWeights(ByVal shape() As Long, ByVal paramsView As INDArray, ByVal flatteningOrder As Char) As INDArray
			Return paramsView.reshape(flatteningOrder, shape)
		End Function


	End Class

End Namespace