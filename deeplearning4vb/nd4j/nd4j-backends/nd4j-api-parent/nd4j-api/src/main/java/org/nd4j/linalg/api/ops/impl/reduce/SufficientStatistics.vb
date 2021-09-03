Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
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

Namespace org.nd4j.linalg.api.ops.impl.reduce


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class SufficientStatistics extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class SufficientStatistics
		Inherits DynamicCustomOp

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SufficientStatistics(org.nd4j.autodiff.samediff.SameDiff sameDiff, @NonNull SDVariable x, @NonNull SDVariable axis, org.nd4j.autodiff.samediff.SDVariable shift)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal axis As SDVariable, ByVal shift As SDVariable)
			MyBase.New(Nothing, sameDiff, argsNoNull(x, axis, shift), False)
		End Sub

		Private Shared Function argsNoNull(ByVal x As SDVariable, ByVal axis As SDVariable, ByVal shift As SDVariable) As SDVariable()
			If shift Is Nothing Then
				Return New SDVariable(){x, axis}
			Else
				Return New SDVariable(){x, axis, shift}
			End If
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SufficientStatistics(@NonNull INDArray x, @NonNull INDArray axes, org.nd4j.linalg.api.ndarray.INDArray shift)
		Public Sub New(ByVal x As INDArray, ByVal axes As INDArray, ByVal shift As INDArray)
			If shift IsNot Nothing Then
				addInputArgument(x, axes, shift)
			Else
				addInputArgument(x, axes)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SufficientStatistics(@NonNull INDArray x, @NonNull INDArray axes)
		Public Sub New(ByVal x As INDArray, ByVal axes As INDArray)
			Me.New(x,axes,Nothing)
		End Sub

		Public Overrides Function opName() As String
			Return "sufficient_statistics"
		End Function

		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Backprop not yet implemented for op: " & Me.GetType().Name)
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			' FIXME
			Return New List(Of DataType) From {inputDataTypes(0), inputDataTypes(0),inputDataTypes(0)}
		End Function
	End Class

End Namespace