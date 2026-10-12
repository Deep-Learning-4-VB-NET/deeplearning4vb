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
Namespace org.nd4j.linalg.api.ops.impl.shape



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class MergeMaxIndex extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class MergeMaxIndex
		Inherits DynamicCustomOp

		Private dataType As DataType = DataType.INT32

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MergeMaxIndex(@NonNull SameDiff sameDiff, @NonNull SDVariable... inputs)
		Public Sub New(ByVal sameDiff As SameDiff, ParamArray ByVal inputs() As SDVariable)
			MyBase.New("mergemaxindex", sameDiff, inputs)
			addIArgument(dataType.toInt())

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MergeMaxIndex(@NonNull INDArray... inputs)
		Public Sub New(ParamArray ByVal inputs() As INDArray)
			MyBase.New("mergemaxindex", inputs, Nothing)
			Preconditions.checkArgument(areEqualShapes(inputs), "All inputs have to be equal shapes")
			addIArgument(dataType.toInt())

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MergeMaxIndex(@NonNull SameDiff sd, @NonNull SDVariable[] x, @NonNull DataType dataType)
		Public Sub New(ByVal sd As SameDiff, ByVal x() As SDVariable, ByVal dataType As DataType)
			MyBase.New("mergemaxindex", sd, x)
			Me.dataType = dataType
			addIArgument(dataType.toInt())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MergeMaxIndex(@NonNull INDArray[] x, @NonNull DataType dataType)
		Public Sub New(ByVal x() As INDArray, ByVal dataType As DataType)
			MyBase.New(x, Nothing)
			Preconditions.checkArgument(areEqualShapes(x), "All inputs have to be equal shapes")
			Me.dataType = dataType
			addIArgument(dataType.toInt())

		End Sub


		Protected Friend Shared Function areEqualShapes(ParamArray ByVal inputs() As INDArray) As Boolean
			For Each input As INDArray In inputs
				If Not inputs(0).equalShapes(input) Then
					Return False
				End If
			Next input
			Return True
		End Function

		Public Overrides Function opName() As String
			Return "mergemaxindex"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Return Collections.singletonList(Me.dataType)
		End Function
	End Class
End Namespace