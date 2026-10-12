Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports PowBp = org.nd4j.linalg.api.ops.impl.reduce.bp.PowBp

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


	Public Class Pow
		Inherits DynamicCustomOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable)
			MyBase.New(sameDiff, New SDVariable(){x, y})
		End Sub

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Pow(@NonNull INDArray x, @NonNull INDArray y)
		Public Sub New(ByVal x As INDArray, ByVal y As INDArray)
			MyBase.New(New INDArray(){x, y}, Nothing)
		End Sub

		Public Overrides Function opName() As String
			Return "Pow"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Pow"
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			'TODO: replace this with discrete op once available: https://github.com/eclipse/deeplearning4j/issues/7461
			'If y=a^b, then:
			'dL/da = b*a^(b-1) * dL/dy
			'dL/db = a^b * log(a) * dL/dy

	'        SDVariable a = arg(0);
	'        SDVariable b = arg(1);
	'        SDVariable dlda = b.mul(sameDiff.math().pow(a,b.sub(1))).mul(f1.get(0));
	'        SDVariable dldb = outputVariable().mul(sameDiff.math().log(a)).mul(f1.get(0));
	'        return Arrays.asList(dlda, dldb);

			Return (New PowBp(sameDiff, arg(0), arg(1), f1(0))).outputs()
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes for %s, got %s", Me.GetType(), dataTypes)
			Return Collections.singletonList(dataTypes(0))
		End Function
	End Class

End Namespace