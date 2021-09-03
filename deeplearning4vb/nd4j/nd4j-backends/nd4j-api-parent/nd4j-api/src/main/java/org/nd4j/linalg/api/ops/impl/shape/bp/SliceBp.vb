Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException

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
'ORIGINAL LINE: @Slf4j public class SliceBp extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class SliceBp
		Inherits DynamicCustomOp

		Private begin() As Integer
		Private size() As Integer

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SliceBp(org.nd4j.autodiff.samediff.SameDiff sameDiff, @NonNull SDVariable input, @NonNull SDVariable gradient, @NonNull int[] begin, @NonNull int[] size)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal gradient As SDVariable, ByVal begin() As Integer, ByVal size() As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){input, gradient})
			Me.begin = begin
			Me.size = size
			addIArgument(begin)
			addIArgument(size)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SliceBp(org.nd4j.autodiff.samediff.SameDiff sameDiff, @NonNull SDVariable input, @NonNull SDVariable gradient, @NonNull SDVariable begin, @NonNull SDVariable size)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal gradient As SDVariable, ByVal begin As SDVariable, ByVal size As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable(){input, begin, size, gradient})
		End Sub


		Public Overrides Function opName() As String
			Return "slice_bp"
		End Function


		Public Overrides Sub assertValidForExecution()
			If numInputArguments() <> 2 AndAlso numInputArguments() <> 4 Then
				Throw New ND4JIllegalStateException("Num input arguments must be 2 or 4.")
			End If
		End Sub

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Differentiation not supported for backprop op: " & Me.GetType().Name)
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes.Count = 2 OrElse dataTypes.Count = 4, "Expected list with exactly 2 or 4 datatypes for %s, got %s", Me.GetType(), dataTypes)
			'Output type is same as (original) input type
	'        if(args().length == 1){
				'Static begin/size
			Dim args() As SDVariable = Me.args()
				Return Collections.singletonList(arg().dataType())
	'        } else {
	'            //Dynamic begin/size
	'            return Arrays.asList(arg(0).dataType(), arg(1).dataType(), arg(2).dataType());
	'        }
		End Function
	End Class

End Namespace