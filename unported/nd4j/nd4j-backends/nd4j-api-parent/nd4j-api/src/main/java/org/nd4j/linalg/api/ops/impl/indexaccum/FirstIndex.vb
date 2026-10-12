Imports System.Collections.Generic
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseIndexAccumulation = org.nd4j.linalg.api.ops.BaseIndexAccumulation
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

Namespace org.nd4j.linalg.api.ops.impl.indexaccum


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor public class FirstIndex extends org.nd4j.linalg.api.ops.BaseIndexAccumulation
	Public Class FirstIndex
		Inherits BaseIndexAccumulation

		Protected Friend condition As Condition
		Protected Friend compare As Double
		Protected Friend eps As Double
		Protected Friend mode As Integer

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal condition As Condition, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, keepDims, dimensions)
			Me.condition = condition
			Me.compare = condition.getValue()
			Me.mode = condition.condtionNum()
			Me.eps = eps
			Me.extraArgs = New Object() {compare, eps, CDbl(mode)}
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean, ByVal condition As Condition, ParamArray ByVal dimensions() As Integer)
			Me.New(sameDiff, i_v, condition, keepDims, dimensions)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FirstIndex(org.nd4j.linalg.api.ndarray.INDArray x, @NonNull Condition condition, int... dimension)
		Public Sub New(ByVal x As INDArray, ByVal condition As Condition, ParamArray ByVal dimension() As Integer)
			Me.New(x, condition, False, dimension)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FirstIndex(org.nd4j.linalg.api.ndarray.INDArray x, boolean keepDims, @NonNull Condition condition, int... dimension)
		Public Sub New(ByVal x As INDArray, ByVal keepDims As Boolean, ByVal condition As Condition, ParamArray ByVal dimension() As Integer)
			Me.New(x,condition,keepDims,dimension)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FirstIndex(org.nd4j.linalg.api.ndarray.INDArray x, @NonNull Condition condition, boolean keepDims, int... dimension)
		Public Sub New(ByVal x As INDArray, ByVal condition As Condition, ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer)
			Me.New(x, condition, Nd4j.EPS_THRESHOLD, dimension)
			Me.keepDims = keepDims
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FirstIndex(org.nd4j.linalg.api.ndarray.INDArray x, @NonNull Condition condition, double eps, int... dimension)
		Public Sub New(ByVal x As INDArray, ByVal condition As Condition, ByVal eps As Double, ParamArray ByVal dimension() As Integer)
			MyBase.New(x, dimension)
			Me.condition = condition
			Me.compare = condition.getValue()
			Me.mode = condition.condtionNum()
			Me.eps = eps
			Me.extraArgs = New Object() {compare, eps, CDbl(mode)}
		End Sub

		Public Overrides Function opNum() As Integer
			Return 4
		End Function

		Public Overrides Function opName() As String
			Return "first_index"
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function

	End Class

End Namespace