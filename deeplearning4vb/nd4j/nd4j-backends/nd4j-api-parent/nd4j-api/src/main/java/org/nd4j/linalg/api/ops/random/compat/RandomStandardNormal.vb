Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
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

Namespace org.nd4j.linalg.api.ops.random.compat


	Public Class RandomStandardNormal
		Inherits DynamicCustomOp

		Public Sub New()
			' values are just hardcoded for this op
			addTArgument(0.0, 1.0)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable)
			MyBase.New(Nothing, sameDiff, args)

			' values are just hardcoded for this op
			addTArgument(0.0, 1.0)
		End Sub

		Public Sub New(ByVal shape As INDArray)
			MyBase.New(Nothing, New INDArray(){shape},New INDArray(){})

			' values are just hardcoded for this op
			addTArgument(0.0, 1.0)
		End Sub

		Public Sub New(ByVal shape As INDArray, ByVal output As INDArray)
			MyBase.New(Nothing, New INDArray(){shape},New INDArray(){output})

			' values are just hardcoded for this op
			addTArgument(0.0, 1.0)
		End Sub

		Public Sub New(ByVal shape() As Long)
			Me.New(Nd4j.create(ArrayUtil.toDouble(shape)), Nd4j.create(shape))
		End Sub

		Public Overrides Function opName() As String
			Return "random_normal"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "RandomStandardNormal"
		End Function

		Public Overrides ReadOnly Property ExtraArgs As Object()
			Get
				' FIXME: why the hell we need this?
				Return New Object() {
					New Double?(0.0),
					New Double?(1.0)
				}
			End Get
		End Property

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got %s", Me.GetType(), inputDataTypes)
			'Input data type specifies the shape; output data type should be any float
			'TODO MAKE CONFIGUREABLE - https://github.com/eclipse/deeplearning4j/issues/6854
			Return Collections.singletonList(DataType.FLOAT)
		End Function
	End Class

End Namespace