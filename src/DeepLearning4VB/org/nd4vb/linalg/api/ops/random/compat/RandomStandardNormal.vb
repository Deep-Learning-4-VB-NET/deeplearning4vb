Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
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

Namespace org.nd4vb.linalg.api.ops.random.compat

    ''' <summary>The custom op <c>random_normal</c> (Java <c>org.nd4j.linalg.api.ops.random.compat.RandomStandardNormal</c>).</summary>
    Public Class RandomStandardNormal
        Inherits DynamicCustomOp

        Public Sub New()
            ' values are just hardcoded for this op
            addTArgument(0.0, 1.0)
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable)
			MyBase.New(Nothing, sameDiff, args)

			' values are just hardcoded for this op
			addTArgument(0.0, 1.0)
		End Sub
#End If

        Public Sub New(ByVal shape As INDArray)
            MyBase.New(Nothing, New INDArray(){shape},New INDArray(){})

            ' values are just hardcoded for this op
            addTArgument(0.0, 1.0)
        End Sub

        Public Sub New(ByVal shape As INDArray, ByVal output As INDArray)
            MyBase.New(Nothing, New INDArray(){shape},New INDArray(){output})

            ' values are just hardcoded for this op
            addTArgument(0.0, 1.0)
        End Sub

        Public Sub New(ByVal shape() As Long)
            Me.New(Nd4vbT.create(ArrayUtil.toDouble(shape)), Nd4vbT.create(shape))
        End Sub

        Public Overrides Function opName() As String
            Return "random_normal"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "RandomStandardNormal"
        End Function

        Public Overrides Function getExtraArgs() As Object()
            ' FIXME: why the hell we need this?
            Return New Object() {
                New Double?(0.0),
                New Double?(1.0)
            }
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got %s", Me.GetType(), inputDataTypes)
            'Input data type specifies the shape; output data type should be any float
            'TODO MAKE CONFIGUREABLE - https://github.com/eclipse/deeplearning4j/issues/6854
            Return New List(Of DataType) From {DataType.FLOAT}
        End Function

    End Class

End Namespace
