Imports System.Collections.Generic
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseDynamicTransformOp = org.nd4j.linalg.api.ops.impl.transforms.BaseDynamicTransformOp
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.custom

	Public Class ATan2
		Inherits BaseDynamicTransformOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal y As SDVariable, ByVal x As SDVariable)
			MyBase.New(sameDiff, New SDVariable() {y, x},False)
		End Sub

		''' <summary>
		''' Note that the order of x and y match <seealso cref="Math.atan2(Double, Double)"/>,
		''' and are reversed when compared to OldATan2.
		''' See <seealso cref="Transforms.atan2(INDArray, INDArray)"/>
		''' </summary>
		Public Sub New(ByVal x As INDArray, ByVal y As INDArray)
			Me.New(x,y,Nothing)
		End Sub

		''' <summary>
		''' Note that the order of x and y match <seealso cref="Math.atan2(Double, Double)"/>,
		''' and are reversed when compared to OldATan2.
		''' See <seealso cref="Transforms.atan2(INDArray, INDArray)"/>
		''' </summary>
		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
			MyBase.New(New INDArray(){x, y}, wrapOrNull(z))
		End Sub

		Public Sub New()
		End Sub

		Public Overrides Function opName() As String
			Return "tf_atan2"
		End Function


		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Atan2"
		End Function


		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			'Let z=atan2(r), with r=y/x
			'dz/dr = 1/(r^2+1), dr/dy = 1/x, dr/dx = -y/x^2
			Dim y As SDVariable = larg()
			Dim x As SDVariable = rarg()

			Dim xGrad As val = sameDiff.math_Conflict.neg(y.div(x.pow(2).add(y.pow(2)))).mul(i_v(0))
			Dim yGrad As val = x.div(x.pow(2).add(y.pow(2))).mul(i_v(0))

			Return New List(Of SDVariable) From {yGrad, xGrad}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes for %s, got %s", Me.GetType(), dataTypes)
			Preconditions.checkState(dataTypes(0) = dataTypes(1), "Input datatypes must be same type: got %s", dataTypes)
			Return Collections.singletonList(dataTypes(0))
		End Function
	End Class

End Namespace