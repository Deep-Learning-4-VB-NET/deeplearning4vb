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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.gradient

    ''' <summary>The custom op <c>dynamic_partition_bp</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.gradient.DynamicPartitionBp</c>).</summary>
    Public Class DynamicPartitionBp
        Inherits DynamicCustomOp

        Private numPartitions As Integer

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal partitions As SDVariable, ByVal gradsAtOutput() As SDVariable, ByVal numPartitions As Integer)
			MyBase.New(Nothing, sameDiff, argsArray(input, partitions, gradsAtOutput), False)
			Me.numPartitions = numPartitions
			addArgs()
		End Sub

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Backprop not supported")
		End Function
#End If

        Protected Friend Overridable Sub addArgs()
            addIArgument(numPartitions)
        End Sub

        Public Overrides Function opName() As String
            Return "dynamic_partition_bp"
        End Function

        Public Overrides ReadOnly Property NumOutputs As Integer
            Get
                Return 2 'input and partitions
            End Get
        End Property

#If SAMEDIFF Then
		Private Shared Function argsArray(ByVal input As SDVariable, ByVal partitions As SDVariable, ByVal grads() As SDVariable) As SDVariable()
			Dim [out](grads.Length + 1) As SDVariable
			[out](0) = input
			[out](1) = partitions
			Array.Copy(grads, 0, [out], 2, grads.Length)
			Return [out]
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            'Input gradients and partition 'gradients' - same type as inputs
            Return New List(Of DataType) From {dataTypes(0), dataTypes(1)}
        End Function

    End Class

End Namespace
