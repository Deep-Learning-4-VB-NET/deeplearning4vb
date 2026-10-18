Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports PartitionMode = DeepLearning4VB.org.nd4vb.enums.PartitionMode
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
#End If

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

Namespace org.nd4vb.linalg.api.ops.impl.shape.tensorops

    ''' <summary>The custom op <c>embedding_lookup</c> (Java <c>org.nd4j.linalg.api.ops.impl.shape.tensorops.EmbeddingLookup</c>).</summary>
    Public Class EmbeddingLookup
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EmbeddingLookup(@NonNull SameDiff sameDiff, @NonNull SDVariable in, @NonNull SDVariable indices, org.nd4j.enums.PartitionMode partitionMode)
		 Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal indices As SDVariable, ByVal partitionMode As PartitionMode)
			MyBase.New("embedding_lookup", sameDiff, New SDVariable(){[in], indices})
			addIArgument(partitionMode.ordinal())
		 End Sub
#End If

        Public Sub New(ByVal [in] As INDArray, ByVal indices As INDArray, ByVal partitionMode As PartitionMode, ByVal output As INDArray)
            MyBase.New("embedding_lookup", New INDArray(){[in], indices}, wrapOrNull(output))
            addIArgument(CInt(partitionMode))

        End Sub

        Public Sub New(ByVal [in] As INDArray, ByVal output As INDArray, ByVal partitionMode As PartitionMode, ParamArray ByVal indices() As Integer)
            MyBase.New("embedding_lookup", New INDArray(){[in], Nd4vbT.createFromArray(indices)}, wrapOrNull(output))
            addIArgument(CInt(partitionMode))

        End Sub

        Public Overrides Function opName() As String
            Return "embedding_lookup"
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkArgument(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes, got %s", dataTypes)
            Preconditions.checkArgument(dataTypes(0).isFPType(), "Input datatype must be floating point, got %s", dataTypes)
            Preconditions.checkArgument(dataTypes(1).isIntType(), "Input datatype must be integer point, got %s", dataTypes)

            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
