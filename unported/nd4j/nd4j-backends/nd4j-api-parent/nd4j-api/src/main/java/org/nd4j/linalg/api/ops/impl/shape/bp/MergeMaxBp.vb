Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
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
Namespace org.nd4j.linalg.api.ops.impl.shape.bp



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class MergeMaxBp extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class MergeMaxBp
		Inherits DynamicCustomOp

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MergeMaxBp(org.nd4j.autodiff.samediff.SameDiff sameDiff, @NonNull SDVariable[] inputs, @NonNull SDVariable gradO)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputs() As SDVariable, ByVal gradO As SDVariable)
			MyBase.New("mergemax_bp", sameDiff, ArrayUtils.add(inputs, gradO))
		End Sub

		Public Overrides Function opName() As String
			Return "mergemax_bp"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim list As IList(Of DataType) = New List(Of DataType)()
			Dim i As Integer=0
			Do While i< args().Length-1
				list.Add(inputDataTypes(0))
				i += 1
			Loop
			Return list

		End Function

		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				Return args().Length-1
			End Get
		End Property
	End Class

End Namespace