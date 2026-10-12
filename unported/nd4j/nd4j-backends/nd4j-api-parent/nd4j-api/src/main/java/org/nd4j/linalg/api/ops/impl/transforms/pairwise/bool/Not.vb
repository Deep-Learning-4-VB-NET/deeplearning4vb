Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseTransformBoolOp = org.nd4j.linalg.api.ops.BaseTransformBoolOp

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool


	Public Class [Not]
		Inherits BaseTransformBoolOp

		Protected Friend comparable As Double = 0.0

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable)
			MyBase.New(sameDiff, i_v, False)
			Me.extraArgs = New Object() {Me.comparable}
		End Sub

		Public Sub New()
			Me.extraArgs = New Object() {Me.comparable}
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public @Not(@NonNull INDArray x, org.nd4j.linalg.api.ndarray.INDArray y, org.nd4j.linalg.api.ndarray.INDArray z, Number comparable)
		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal comparable As Number)
			MyBase.New(x, y, z)
			Me.comparable = comparable.doubleValue()
			Me.extraArgs = New Object() {Me.comparable}
		End Sub

		Public Overrides Function opNum() As Integer
			Return 10
		End Function

		Public Overrides Function opName() As String
			Return "not"
		End Function

		Public Overrides Function onnxName() As String
			Return "Not"
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function
	End Class

End Namespace