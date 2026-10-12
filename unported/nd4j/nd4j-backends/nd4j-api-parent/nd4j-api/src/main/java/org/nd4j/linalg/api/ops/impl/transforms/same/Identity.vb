Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseDynamicTransformOp = org.nd4j.linalg.api.ops.impl.transforms.BaseDynamicTransformOp

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.same


	Public Class Identity
		Inherits BaseDynamicTransformOp

		Public Sub New(ByVal sd As SameDiff, ByVal input As SDVariable)
			MyBase.New(sd, New SDVariable(){input}, False)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
			MyBase.New(New INDArray(){x}, New INDArray(){z})
		End Sub

		Public Sub New(ByVal x As INDArray)
			MyBase.New(New INDArray(){x}, Nothing)
		End Sub

		Public Sub New()
		End Sub

		Public Overrides Function opName() As String
			Return "identity"
		End Function

		Public Overrides Function onnxName() As String
			Return "Constant"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Identity"
		End Function

		Public Overrides Function tensorflowNames() As String()
			Return New String(){"Identity", "DeepCopy", "CopyHost"}
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			'Eventually we'll optimize this out
			Return Collections.singletonList(sameDiff.identity(i_v(0)))
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got input %s", Me.GetType(), dataTypes)
			If dArguments.Count > 0 Then
				Return New List(Of DataType) From {dArguments(0)}
			End If
			Return dataTypes
		End Function

	End Class

End Namespace