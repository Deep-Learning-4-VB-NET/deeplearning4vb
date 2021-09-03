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


	Public Class AdjustHue
		Inherits DynamicCustomOp

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AdjustHue(@NonNull INDArray in, double delta, org.nd4j.linalg.api.ndarray.INDArray out)
		Public Sub New(ByVal [in] As INDArray, ByVal delta As Double, ByVal [out] As INDArray)
			Me.New([in], delta)
			If [out] IsNot Nothing Then
				outputArguments_Conflict.Add([out])
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AdjustHue(@NonNull INDArray in, double delta)
		Public Sub New(ByVal [in] As INDArray, ByVal delta As Double)
			Preconditions.checkArgument([in].rank() >= 3, "AdjustSaturation: op expects rank of input array to be >= 3, but got %s instead", [in].rank())
			Preconditions.checkArgument(-1.0 <= delta AndAlso delta <= 1.0, "AdjustHue: parameter delta must be within [-1, 1] interval," & " but got %s instead", delta)
			inputArguments_Conflict.Add([in])

			addTArgument(delta)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AdjustHue(@NonNull SameDiff sameDiff, @NonNull SDVariable in, @NonNull SDVariable factor)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal factor As SDVariable)
			MyBase.New(sameDiff,New SDVariable(){[in], factor})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AdjustHue(@NonNull SameDiff sameDiff, @NonNull SDVariable in, double factor)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal factor As Double)
			MyBase.New(sameDiff,New SDVariable(){[in]})
			addTArgument(factor)
		End Sub

		Public Overrides Function opName() As String
			Return "adjust_hue"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "AdjustHue"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace