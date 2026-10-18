Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports Nd4j = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
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

    ''' <summary>The custom op <c>random_exponential</c> (Java <c>org.nd4j.linalg.api.ops.random.custom.RandomExponential</c>).</summary>
    Public Class RandomExponential
        Inherits DynamicCustomOp

        Private lambda As Double = 0.0
        Private dataTypeField As DataType = DataType.DOUBLE

        Public Sub New()
            '
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sd As SameDiff, ByVal shape As SDVariable, ByVal lambda As Double)
			MyBase.New(Nothing, sd, New SDVariable(){shape})
			Preconditions.checkState(lambda >= 0, "Lambda parameter must be > 0 - got %s", lambda)
			Me.lambda = lambda
			addTArgument(lambda)
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal lambda As Double, ByVal dataType As DataType, ParamArray ByVal shape() As Long)
			MyBase.New(Nothing, sd, New SDVariable(){sd.constant(Nd4j.createFromArray(shape))})
			Me.lambda = lambda
			addTArgument(lambda)
			Me.dataType = dataType
			addDArgument(dataType)
			addIArgument(shape)
		End Sub
#End If

        Public Sub New(ByVal lambda As Double, ByVal datatype As DataType, ParamArray ByVal shape() As Long)
            Me.New(Nd4vbT.createFromArray(shape), Nd4vbT.createUninitialized(datatype, shape), lambda)
        End Sub

        Public Sub New(ByVal shape As INDArray, ByVal [out] As INDArray, ByVal lambda As Double)
            MyBase.New(Nothing, New INDArray(){shape}, New INDArray(){[out]}, JavaCollections.singletonList(lambda), DirectCast(Nothing, IList(Of Integer)))
            Me.lambda = lambda
        End Sub

        Public Overrides Function opName() As String
            Return "random_exponential"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal gradients As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got %s", Me.GetType(), inputDataTypes)
            'Input data type specifies the shape; output data type should be any float
            'TODO MAKE CONFIGUREABLE - https://github.com/eclipse/deeplearning4j/issues/6854
            Return New List(Of DataType) From {DataType.FLOAT}
        End Function

    End Class

End Namespace
