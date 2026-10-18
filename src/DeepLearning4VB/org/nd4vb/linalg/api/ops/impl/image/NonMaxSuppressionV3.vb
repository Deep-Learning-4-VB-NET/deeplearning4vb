Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports Op = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op
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

Namespace org.nd4vb.linalg.api.ops.impl.image

    ''' <summary>The custom op <c>non_max_suppression_v3</c> (Java <c>org.nd4j.linalg.api.ops.impl.image.NonMaxSuppressionV3</c>).</summary>
    Public Class NonMaxSuppressionV3
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NonMaxSuppressionV3(org.nd4j.autodiff.samediff.SameDiff sameDiff, @NonNull SDVariable boxes, @NonNull SDVariable scores, @NonNull SDVariable maxOutSize, @NonNull SDVariable iouThreshold, @NonNull SDVariable scoreThreshold)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal boxes As SDVariable, ByVal scores As SDVariable, ByVal maxOutSize As SDVariable, ByVal iouThreshold As SDVariable, ByVal scoreThreshold As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable(){boxes, scores, maxOutSize, iouThreshold, scoreThreshold}, False)
		End Sub
#End If

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

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            'Always 1D integer tensor (indices)
            Return New List(Of DataType) From {DataType.INT}
        End Function

    End Class

End Namespace
