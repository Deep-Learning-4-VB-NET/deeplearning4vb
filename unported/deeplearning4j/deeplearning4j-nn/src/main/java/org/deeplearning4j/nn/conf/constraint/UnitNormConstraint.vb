Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Broadcast = org.nd4j.linalg.factory.Broadcast

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
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) public class UnitNormConstraint extends BaseConstraint
	<Serializable>
	Public Class UnitNormConstraint
		Inherits BaseConstraint

		Private Sub New()
			'No arg for json ser/de
		End Sub

		''' <summary>
		''' Apply to weights but not biases by default
		''' </summary>
		''' <param name="dimensions">     Dimensions to apply to. For DenseLayer, OutputLayer, RnnOutputLayer, LSTM, etc: this should
		'''                       be dimension 1. For CNNs, this should be dimensions [1,2,3] corresponding to last 3 of
		'''                       parameters which have order [depthOut, depthIn, kH, kW] </param>
		Public Sub New(ParamArray ByVal dimensions() As Integer)
			Me.New(Enumerable.Empty(Of String)(), dimensions)
		End Sub


		''' <param name="dimensions">     Dimensions to apply to. For DenseLayer, OutputLayer, RnnOutputLayer, LSTM, etc: this should
		'''                       be dimension 1. For CNNs, this should be dimensions [1,2,3] corresponding to last 3 of
		'''                       parameters which have order [depthOut, depthIn, kH, kW] </param>
		Public Sub New(ByVal paramNames As ISet(Of String), ParamArray ByVal dimensions() As Integer)
			MyBase.New(paramNames, dimensions)
		End Sub

		Public Overrides Sub apply(ByVal param As INDArray)
			Dim norm2 As INDArray = param.norm2(dimensions)
			Broadcast.div(param, norm2, param, getBroadcastDims(dimensions, param.rank()))
		End Sub

		Public Overrides Function clone() As UnitNormConstraint
			Return New UnitNormConstraint(params, dimensions)
		End Function
	End Class

End Namespace