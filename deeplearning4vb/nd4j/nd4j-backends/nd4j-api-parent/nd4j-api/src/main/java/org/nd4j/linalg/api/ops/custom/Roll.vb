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
Namespace org.nd4j.linalg.api.ops.custom


	Public Class Roll
		Inherits DynamicCustomOp

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Roll(@NonNull INDArray input, @NonNull INDArray shifts, @NonNull INDArray axes)
		Public Sub New(ByVal input As INDArray, ByVal shifts As INDArray, ByVal axes As INDArray)
			Preconditions.checkArgument(axes.rank() = shifts.rank(), "Roll: shifts and axes should be the same rank")
			Preconditions.checkArgument(axes.length() = shifts.length(), "Roll: shifts and axes should be the same length")
			addInputArgument(input, shifts, axes)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Roll(@NonNull INDArray input, int shift)
		Public Sub New(ByVal input As INDArray, ByVal shift As Integer)
			addInputArgument(input)
			addIArgument(shift)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Roll(@NonNull SameDiff sameDiff, @NonNull SDVariable input, @NonNull SDVariable shift)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal shift As SDVariable)
			MyBase.New("", sameDiff, New SDVariable(){input, shift})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Roll(@NonNull SameDiff sameDiff, @NonNull SDVariable input, @NonNull SDVariable shift, @NonNull SDVariable axes)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal shift As SDVariable, ByVal axes As SDVariable)
			MyBase.New("", sameDiff, New SDVariable(){input, shift, axes})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Roll(@NonNull SameDiff sameDiff, @NonNull SDVariable input, int shift)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal shift As Integer)
			MyBase.New("", sameDiff, New SDVariable(){input})
			addIArgument(shift)
		End Sub

		Public Overrides Function opName() As String
			Return "roll"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Roll"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace