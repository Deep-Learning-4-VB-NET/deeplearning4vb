Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports EluBp = org.nd4j.linalg.api.ops.impl.transforms.gradient.EluBp

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.strict


	Public Class ELU
		Inherits DynamicCustomOp

		Public Const DEFAULT_ALPHA As Double = 1.0

		Protected Friend alpha As Double

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable)
			MyBase.New(sameDiff, New SDVariable(){i_v})
			Me.alpha = DEFAULT_ALPHA
			addTArgument(alpha)
		End Sub

		Public Sub New()
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
			Me.New(x, z, DEFAULT_ALPHA)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal alpha As Double)
			MyBase.New(Nothing, wrapOrNull(x), wrapOrNull(z))
			Me.alpha = alpha
			addTArgument(alpha)
		End Sub

		Public Sub New(ByVal x As INDArray)
			Me.New(x, Nothing, DEFAULT_ALPHA)
		End Sub

		Public Overrides Function opName() As String
			Return "elu"
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Elu"
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			'ELU: e^x-1 if x<0, x otherwise
			'dL/dIn = dL/Out * dOut/dIn
			Return (New EluBp(sameDiff, arg(), i_v(0), alpha)).outputs()
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 1, "Expected exactly 1 datatype for ELU, got %s", dataTypes)
			Preconditions.checkState(dataTypes(0).isFPType(), "Expected floating point input type for ELU, got %s", dataTypes)

			Return dataTypes
		End Function
	End Class

End Namespace