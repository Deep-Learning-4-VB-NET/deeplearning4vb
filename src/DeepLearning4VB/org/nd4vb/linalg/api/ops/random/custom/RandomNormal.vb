Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
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

Namespace org.nd4vb.linalg.api.ops.random.custom

    ''' <summary>The custom op <c>randomnormal</c> (Java <c>org.nd4j.linalg.api.ops.random.custom.RandomNormal</c>).</summary>
    Public Class RandomNormal
        Inherits DynamicCustomOp

        Private mean As Double
        Private stdev As Double

        Public Sub New()

        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal shape As SDVariable, ByVal mean As Double, ByVal stdev As Double)
			MyBase.New(Nothing, sameDiff, New SDVariable(){shape})
			Me.mean = mean
			Me.stdev = stdev

			addTArgument(mean, stdev)
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "randomnormal"
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("Not TF op name set for " & Me.GetType().Name)
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
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
