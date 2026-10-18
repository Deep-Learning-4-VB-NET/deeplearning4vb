Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
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

Namespace org.nd4vb.linalg.api.ops.impl.shape

    ''' <summary>The custom op <c>size_at</c> (Java <c>org.nd4j.linalg.api.ops.impl.shape.SizeAt</c>).</summary>
    Public Class SizeAt
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal dimension As Integer)
            Me.New(input, Nothing, dimension)
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal output As INDArray, ByVal dimension As Integer)
            MyBase.New(Nothing,input, output, New List(Of Double)(), New Integer(){dimension})
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal dimension As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable() {input}, False)

			iArguments.Add(Convert.ToInt64(dimension))
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "size_at"
        End Function

        Public Overrides Function opType() As Op.Type
            Return Op.Type.CUSTOM
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes.Count = 1, "Expected list with exactly 1 datatype for %s, got %s", Me.GetType(), dataTypes)
            'Output type is always long (i.e., size of array)
            Return New List(Of DataType) From {DataType.LONG}
        End Function

    End Class

End Namespace
