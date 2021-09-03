Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class ReverseBp extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class ReverseBp
		Inherits DynamicCustomOp

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ReverseBp(@NonNull SameDiff sameDiff, @NonNull SDVariable i_v, @NonNull SDVariable grad, @NonNull int... dimensions)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal grad As SDVariable, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, New SDVariable(){i_v, grad})
			addIArgument(dimensions)
		End Sub


		Public Overrides Function opName() As String
			Return "reverse_bp"
		End Function


		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace