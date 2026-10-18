Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.custom

    ''' <summary>The custom op <c>reverse_bp</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.ReverseBp</c>).</summary>
    Public Class ReverseBp
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ReverseBp(@NonNull SameDiff sameDiff, @NonNull SDVariable i_v, @NonNull SDVariable grad, @NonNull int... dimensions)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal grad As SDVariable, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, New SDVariable(){i_v, grad})
			addIArgument(dimensions)
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "reverse_bp"
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Return New List(Of DataType) From {inputDataTypes(0)}
        End Function

    End Class

End Namespace
