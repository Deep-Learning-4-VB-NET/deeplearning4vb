Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Broadcast = org.nd4j.linalg.factory.Broadcast
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions

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

Namespace org.deeplearning4j.nn.conf.constraint


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) public class MinMaxNormConstraint extends BaseConstraint
	<Serializable>
	Public Class MinMaxNormConstraint
		Inherits BaseConstraint

		Public Const DEFAULT_RATE As Double = 1.0

		Private min As Double
		Private max As Double
		Private rate As Double

		Private Sub New()
			'No arg for json ser/de
		End Sub

		''' <summary>
		''' Apply to weights but not biases by default
		''' </summary>
		''' <param name="max">            Maximum L2 value </param>
		''' <param name="min">            Minimum L2 value </param>
		''' <param name="dimensions">     Dimensions to apply to. For DenseLayer, OutputLayer, RnnOutputLayer, LSTM, etc: this should
		'''                       be dimension 1. For CNNs, this should be dimensions [1,2,3] corresponding to last 3 of
		'''                       parameters which have order [depthOut, depthIn, kH, kW] </param>
		Public Sub New(ByVal min As Double, ByVal max As Double, ParamArray ByVal dimensions() As Integer)
			Me.New(min, max, DEFAULT_RATE, Nothing, dimensions)
		End Sub

		''' <summary>
		''' Apply to weights but not biases by default
		''' </summary>
		''' <param name="max">            Maximum L2 value </param>
		''' <param name="min">            Minimum L2 value </param>
		''' <param name="rate">           Constraint rate </param>
		''' <param name="dimensions">     Dimensions to apply to. For DenseLayer, OutputLayer, RnnOutputLayer, LSTM, etc: this should
		'''                       be dimension 1. For CNNs, this should be dimensions [1,2,3] corresponding to last 3 of
		'''                       parameters which have order [depthOut, depthIn, kH, kW] </param>
		Public Sub New(ByVal min As Double, ByVal max As Double, ByVal rate As Double, ParamArray ByVal dimensions() As Integer)
			Me.New(min, max, rate, Enumerable.Empty(Of String)(), dimensions)
		End Sub

		''' 
		''' <param name="max">            Maximum L2 value </param>
		''' <param name="min">            Minimum L2 value </param>
		''' <param name="rate">           Constraint rate </param>
		''' <param name="paramNames">     Which parameter names to apply constraint to </param>
		''' <param name="dimensions">     Dimensions to apply to. For DenseLayer, OutputLayer, RnnOutputLayer, LSTM, etc: this should
		'''                       be dimension 1. For CNNs, this should be dimensions [1,2,3] corresponding to last 3 of
		'''                       parameters which have order [depthOut, depthIn, kH, kW] </param>
		Public Sub New(ByVal min As Double, ByVal max As Double, ByVal rate As Double, ByVal paramNames As ISet(Of String), ParamArray ByVal dimensions() As Integer)
			MyBase.New(paramNames, dimensions)
			If rate <= 0 OrElse rate > 1.0 Then
				Throw New System.InvalidOperationException("Invalid rate: must be in interval (0,1]: got " & rate)
			End If
			Me.min = min
			Me.max = max
			Me.rate = rate
		End Sub

		Public Overrides Sub apply(ByVal param As INDArray)
			Dim norm As INDArray = param.norm2(dimensions)
			Dim clipped As INDArray = norm.unsafeDuplication()
			Dim op As CustomOp = DynamicCustomOp.builder("clipbyvalue").addInputs(clipped).callInplace(True).addFloatingPointArguments(min, max).build()
			Nd4j.Executioner.exec(op)

			norm.addi(epsilon)
			clipped.divi(norm)

			If rate <> 1.0 Then
				clipped.muli(rate).addi(norm.muli(1.0-rate))
			End If

			Broadcast.mul(param, clipped, param, getBroadcastDims(dimensions, param.rank()))
		End Sub

		Public Overrides Function clone() As MinMaxNormConstraint
			Return New MinMaxNormConstraint(min, max, rate, params, dimensions)
		End Function
	End Class

End Namespace