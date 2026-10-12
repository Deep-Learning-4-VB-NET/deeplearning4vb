Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp

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


	Public Class Trace
		Inherits DynamicCustomOp

		Public Sub New(ByVal sd As SameDiff, ByVal [in] As SDVariable)
			MyBase.New(Nothing, sd, New SDVariable(){[in]})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Trace(@NonNull INDArray in)
		Public Sub New(ByVal [in] As INDArray)
			MyBase.New(wrapOrNull([in]), Nothing)
		End Sub

		Public Sub New()
		End Sub

		Public Overrides Function opName() As String
			Return "trace"
		End Function

		Public Overrides Function doDiff(ByVal gradAtOutput As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim rows As SDVariable = sameDiff.reshape(sameDiff.sizeAt(arg(), -2), 1)
			Dim cols As SDVariable = sameDiff.reshape(sameDiff.sizeAt(arg(), -1), 1)
			Dim eye As SDVariable = sameDiff.math().eye(rows, cols)
			'Reshape gradient from [x,y,z] to [x,y,z,1,1]
			Dim reshapedGrad As SDVariable = sameDiff.expandDims(gradAtOutput(0), -1)
			reshapedGrad = sameDiff.expandDims(reshapedGrad, -1)
			Return Collections.singletonList(reshapedGrad.mul(eye))
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got %s", Me.GetType(), dataTypes)
			Return Collections.singletonList(dataTypes(0))
		End Function

	End Class

End Namespace