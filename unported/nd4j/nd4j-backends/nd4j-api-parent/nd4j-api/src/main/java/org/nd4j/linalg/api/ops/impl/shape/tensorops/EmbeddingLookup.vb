Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports PartitionMode = org.nd4j.enums.PartitionMode
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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
Namespace org.nd4j.linalg.api.ops.impl.shape.tensorops


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class EmbeddingLookup extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class EmbeddingLookup
		Inherits DynamicCustomOp

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EmbeddingLookup(@NonNull SameDiff sameDiff, @NonNull SDVariable in, @NonNull SDVariable indices, org.nd4j.enums.PartitionMode partitionMode)
		 Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal indices As SDVariable, ByVal partitionMode As PartitionMode)
			MyBase.New("embedding_lookup", sameDiff, New SDVariable(){[in], indices})
			addIArgument(partitionMode.ordinal())
		 End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EmbeddingLookup(@NonNull INDArray in, @NonNull INDArray indices, org.nd4j.enums.PartitionMode partitionMode, org.nd4j.linalg.api.ndarray.INDArray output)
		Public Sub New(ByVal [in] As INDArray, ByVal indices As INDArray, ByVal partitionMode As PartitionMode, ByVal output As INDArray)
			MyBase.New("embedding_lookup", New INDArray(){[in], indices}, wrapOrNull(output))
			addIArgument(partitionMode.ordinal())

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EmbeddingLookup(@NonNull INDArray in, org.nd4j.linalg.api.ndarray.INDArray output, org.nd4j.enums.PartitionMode partitionMode, @NonNull int... indices)
		Public Sub New(ByVal [in] As INDArray, ByVal output As INDArray, ByVal partitionMode As PartitionMode, ParamArray ByVal indices() As Integer)
			MyBase.New("embedding_lookup", New INDArray(){[in], Nd4j.createFromArray(indices)}, wrapOrNull(output))
			addIArgument(partitionMode.ordinal())


		End Sub

		Public Overrides Function opName() As String
			Return "embedding_lookup"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkArgument(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes, got %s", dataTypes)
			Preconditions.checkArgument(dataTypes(0).isFPType(), "Input datatype must be floating point, got %s", dataTypes)
			Preconditions.checkArgument(dataTypes(1).isIntType(), "Input datatype must be integer point, got %s", dataTypes)

			Return Collections.singletonList(dataTypes(0))
		End Function


	End Class

End Namespace