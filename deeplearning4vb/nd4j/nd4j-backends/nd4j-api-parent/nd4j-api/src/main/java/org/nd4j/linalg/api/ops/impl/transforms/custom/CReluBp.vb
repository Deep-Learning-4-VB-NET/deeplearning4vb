Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NonNull = lombok.NonNull

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
'ORIGINAL LINE: @NoArgsConstructor public class CReluBp extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class CReluBp
		Inherits DynamicCustomOp

		Public Sub New(ByVal sd As SameDiff, ByVal input As SDVariable, ByVal epsilonNext As SDVariable)
			MyBase.New(sd, New SDVariable(){input, epsilonNext})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CReluBp(@NonNull INDArray input, @NonNull INDArray epsilonNext, org.nd4j.linalg.api.ndarray.INDArray output)
		Public Sub New(ByVal input As INDArray, ByVal epsilonNext As INDArray, ByVal output As INDArray)
			MyBase.New(New INDArray(){input, epsilonNext}, wrapOrNull(output))
		End Sub


		Public Overrides Function opName() As String
			Return "crelu_bp"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkArgument(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes, got %s", dataTypes)
			Preconditions.checkArgument(dataTypes(0).isFPType(), "Input datatype must be floating point, got %s", dataTypes)

			Return Collections.singletonList(dataTypes(0))
		End Function


	End Class

End Namespace