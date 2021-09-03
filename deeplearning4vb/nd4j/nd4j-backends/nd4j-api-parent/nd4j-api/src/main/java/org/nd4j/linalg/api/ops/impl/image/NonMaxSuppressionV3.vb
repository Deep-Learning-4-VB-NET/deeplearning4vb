Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Op = org.nd4j.linalg.api.ops.Op

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

Namespace org.nd4j.linalg.api.ops.impl.image


	Public Class NonMaxSuppressionV3
		Inherits DynamicCustomOp

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NonMaxSuppressionV3(org.nd4j.autodiff.samediff.SameDiff sameDiff, @NonNull SDVariable boxes, @NonNull SDVariable scores, @NonNull SDVariable maxOutSize, @NonNull SDVariable iouThreshold, @NonNull SDVariable scoreThreshold)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal boxes As SDVariable, ByVal scores As SDVariable, ByVal maxOutSize As SDVariable, ByVal iouThreshold As SDVariable, ByVal scoreThreshold As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable(){boxes, scores, maxOutSize, iouThreshold, scoreThreshold}, False)
		End Sub

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx name found for shape " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Return "NonMaxSuppressionV3"
		End Function

		Public Overrides Function tensorflowNames() As String()
			Return New String(){"NonMaxSuppressionV3", "NonMaxSuppressionV4", "NonMaxSuppressionV5"}
		End Function

		Public Overrides Function opName() As String
			Return "non_max_suppression_v3"
		End Function

		Public Overrides Function opType() As Op.Type
			Return Op.Type.CUSTOM
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			'Always 1D integer tensor (indices)
			Return Collections.singletonList(DataType.INT)
		End Function
	End Class

End Namespace