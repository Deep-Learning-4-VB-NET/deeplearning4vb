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


	Public Class AdjustContrast
		Inherits DynamicCustomOp

		Public Sub New()
			MyBase.New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AdjustContrast(@NonNull INDArray in, double factor, org.nd4j.linalg.api.ndarray.INDArray out)
		Public Sub New(ByVal [in] As INDArray, ByVal factor As Double, ByVal [out] As INDArray)
			Preconditions.checkArgument([in].rank() >= 3, "AdjustContrast: op expects rank of input array to be >= 3, but got %s instead", [in].rank())
			inputArguments_Conflict.Add([in])
			outputArguments_Conflict.Add([out])

			addTArgument(factor)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AdjustContrast(@NonNull INDArray in, double factor)
		Public Sub New(ByVal [in] As INDArray, ByVal factor As Double)
			Me.New([in], factor, Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AdjustContrast(@NonNull SameDiff sameDiff, @NonNull SDVariable in, @NonNull SDVariable factor)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal factor As SDVariable)
			MyBase.New(sameDiff, New SDVariable(){[in], factor})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AdjustContrast(@NonNull SameDiff sameDiff, @NonNull SDVariable in, double factor)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal factor As Double)
			MyBase.New(sameDiff, New SDVariable(){[in]})
			addTArgument(factor)
		End Sub

		Public Overrides Function opName() As String
			Return "adjust_contrast_v2"
		End Function

		Public Overrides Function tensorflowNames() As String()
			Return New String(){"AdjustContrast", "AdjustContrastv2"}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class
End Namespace