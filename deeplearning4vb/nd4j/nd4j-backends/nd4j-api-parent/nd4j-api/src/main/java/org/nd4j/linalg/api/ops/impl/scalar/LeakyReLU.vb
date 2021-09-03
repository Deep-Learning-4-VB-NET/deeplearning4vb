Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseScalarOp = org.nd4j.linalg.api.ops.BaseScalarOp
Imports LeakyReLUBp = org.nd4j.linalg.api.ops.impl.transforms.gradient.LeakyReLUBp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef

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

Namespace org.nd4j.linalg.api.ops.impl.scalar

	Public Class LeakyReLU
		Inherits BaseScalarOp

		Public Const DEFAULT_ALPHA As Double = 0.01
		Private alpha As Double = DEFAULT_ALPHA

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal inPlace As Boolean, ByVal alpha As Double)
			MyBase.New(sameDiff, i_v, alpha, inPlace)
			Me.alpha = alpha
			Me.extraArgs = New Object(){alpha}

		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal alpha As Double)
			Me.New(sameDiff, i_v, False, alpha)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal extraArgs() As Object, ByVal alpha As Double)
			MyBase.New(sameDiff, i_v, alpha, extraArgs)
			Me.alpha = alpha
			Me.extraArgs = New Object(){alpha}
		End Sub

		Public Sub New()
			MyBase.New()
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal alpha As Double)
			MyBase.New(x, alpha)
			Me.alpha = alpha
			Me.extraArgs = New Object(){alpha}
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal alpha As Double)
			MyBase.New(x, Nothing, z, alpha)
			Me.alpha = alpha
			Me.extraArgs = New Object(){alpha}
		End Sub


		Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
			Me.New(x, z, 0.01)
		End Sub

		Public Sub New(ByVal x As INDArray)
			MyBase.New(x, 0.01)
		End Sub

		Public Overrides Function opNum() As Integer
			Return 35
		End Function

		Public Overrides Function opName() As String
			Return "leakyrelu"
		End Function

		Public Overrides Function onnxName() As String
			Return "LeakyRelu"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "LeakyRelu"
		End Function


		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New LeakyReLUBp(sameDiff, arg(), i_v(0), alpha)).outputs()
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			alpha = attributesForNode("alpha").getF()
			extraArgs = New Object(){alpha}
			Me.setScalar(Nd4j.scalar(org.nd4j.linalg.api.buffer.DataType.FLOAT, alpha))
		End Sub
	End Class

End Namespace