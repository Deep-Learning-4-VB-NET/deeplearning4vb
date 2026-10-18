Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
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

Namespace org.nd4vb.linalg.api.ops.random.custom

    ''' <summary>The custom op <c>random_shuffle</c> (Java <c>org.nd4j.linalg.api.ops.random.custom.RandomShuffle</c>).</summary>
    Public Class RandomShuffle
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Public Sub New(ByVal value As INDArray, ParamArray ByVal seeds() As Integer)
            addInputArgument(value)
            addIArgument(seeds)
        End Sub

        Public Sub New(ByVal value As INDArray)
            Me.New(value, 0, 0)
        End Sub

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public RandomShuffle(@NonNull SameDiff sameDiff, @NonNull SDVariable value, int...seeds)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal value As SDVariable, ParamArray ByVal seeds() As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){value})
			addIArgument(seeds)
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "random_shuffle"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "RandomShuffle"
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got %s", Me.GetType(), inputDataTypes)
            Return New List(Of DataType) From {inputDataTypes(0)}
        End Function

    End Class

End Namespace
