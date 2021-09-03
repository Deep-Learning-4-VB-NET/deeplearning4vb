Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseTransformBoolOp = org.nd4j.linalg.api.ops.BaseTransformBoolOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Condition = org.nd4j.linalg.indexing.conditions.Condition

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.bool


	Public Class MatchConditionTransform
		Inherits BaseTransformBoolOp

		Private condition As Condition
		Private compare As Double
		Private eps As Double
		Private mode As Integer

		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal condition As Condition)
			MyBase.New(sameDiff, [in], False)
			Me.condition = condition
			Me.compare = condition.getValue()
			Me.mode = condition.condtionNum()
			Me.eps = Nd4j.EPS_THRESHOLD
			Me.extraArgs = New Object() {compare, eps, mode}
		End Sub

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MatchConditionTransform(@NonNull INDArray x, @NonNull INDArray y, @NonNull INDArray z, @NonNull Condition condition)
		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal condition As Condition)
			Me.New(x, z, Nd4j.EPS_THRESHOLD, condition)
			Me.y_Conflict = y
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MatchConditionTransform(@NonNull INDArray x, @NonNull INDArray z, @NonNull Condition condition)
		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal condition As Condition)
			Me.New(x, z, Nd4j.EPS_THRESHOLD, condition)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MatchConditionTransform(org.nd4j.linalg.api.ndarray.INDArray x, @NonNull Condition condition)
		Public Sub New(ByVal x As INDArray, ByVal condition As Condition)
			Me.New(x, Nothing, Nd4j.EPS_THRESHOLD, condition)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MatchConditionTransform(org.nd4j.linalg.api.ndarray.INDArray x, org.nd4j.linalg.api.ndarray.INDArray z, double eps, @NonNull Condition condition)
		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal eps As Double, ByVal condition As Condition)
			MyBase.New(x, Nothing, z)

			Me.compare = condition.getValue()
			Me.mode = condition.condtionNum()
			Me.eps = eps

			Me.extraArgs = New Object() {compare, eps, mode}
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MatchConditionTransform(org.nd4j.linalg.api.ndarray.INDArray x, double eps, @NonNull Condition condition)
		Public Sub New(ByVal x As INDArray, ByVal eps As Double, ByVal condition As Condition)
			Me.New(x, Nothing, eps, condition)
		End Sub

		Public Overrides Function opNum() As Integer
			Return 5
		End Function

		Public Overrides Function opName() As String
			Return "match_condition_transform"
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function



		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function
	End Class

End Namespace