Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports GRUWeights = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.recurrent.weights.GRUWeights
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

Namespace org.nd4vb.linalg.api.ops.impl.layers.recurrent

    ''' <summary>The custom op <c>gruCell</c> (Java <c>org.nd4j.linalg.api.ops.impl.layers.recurrent.GRUCell</c>).</summary>
    Public Class GRUCell
        Inherits DynamicCustomOp

        Private weights As GRUWeights

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal hLast As SDVariable, ByVal weights As GRUWeights)
			MyBase.New(Nothing, sameDiff, weights.argsWithInputs(x, hLast))
			Me.weights = weights
		End Sub
#End If

        Public Sub New(ByVal x As INDArray, ByVal hLast As INDArray, ByVal gruWeights As GRUWeights)
            MyBase.New(Nothing, Nothing, gruWeights.argsWithInputs(x, hLast))
            Me.weights = gruWeights
        End Sub

        Public Overrides Function opName() As String
            Return "gruCell"
        End Function

        Public Overrides Function onnxName() As String
            Return "GRU"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "GRUBlockCell"
        End Function

        Public Overrides Function onnxNames() As String()
            Return MyBase.onnxNames()
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 6, "Expected exactly 6 inputs to GRUCell, got %s", inputDataTypes)
            '4 outputs, all of same type as input
            Dim dt As DataType = inputDataTypes(0)
            Preconditions.checkState(dt.isFPType(), "Input type 0 must be a floating point type, got %s", dt)
            Return New List(Of DataType) From {dt, dt, dt, dt}
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grads As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not yet implemented")
		End Function
#End If

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>weights</c>.</summary>
        Public Overridable Function getWeights() As GRUWeights
            Return weights
        End Function

    End Class

End Namespace
