Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class MatrixBandPart extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class MatrixBandPart
		Inherits DynamicCustomOp

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MatrixBandPart(@NonNull INDArray input, int minLower, int maxUpper)
		Public Sub New(ByVal input As INDArray, ByVal minLower As Integer, ByVal maxUpper As Integer)
			Preconditions.checkArgument(input.rank() >= 2, "MatrixBandPart: Input rank should be 2 or higher")
			Dim N As Long = input.size(-2)
			Dim M As Long = input.size(-1)
			Preconditions.checkArgument(minLower > -N AndAlso minLower < N, "MatrixBandPart: lower diagonal count %s should be less than %s", minLower, N)
			Preconditions.checkArgument(maxUpper > -M AndAlso maxUpper < M, "MatrixBandPart: upper diagonal count %s should be less than %s.", maxUpper, M)
			addInputArgument(input)
			addIArgument(minLower, maxUpper)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MatrixBandPart(@NonNull SameDiff sameDiff, @NonNull SDVariable input, org.nd4j.autodiff.samediff.SDVariable minLower, org.nd4j.autodiff.samediff.SDVariable maxUpper)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal minLower As SDVariable, ByVal maxUpper As SDVariable)
			MyBase.New("", sameDiff, New SDVariable(){input, minLower, maxUpper})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MatrixBandPart(@NonNull SameDiff sameDiff, @NonNull SDVariable input, int minLower, int maxUpper)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal minLower As Integer, ByVal maxUpper As Integer)
			MyBase.New("", sameDiff, New SDVariable(){input})
			addIArgument(minLower, maxUpper)
		End Sub

		Public Overrides Function opName() As String
			Return "matrix_band_part"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "MatrixBandPart"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace