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

    ''' <summary>The custom op <c>space_to_batch_nd</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.SpaceToBatchND</c>).</summary>
    Public Class SpaceToBatchND
        Inherits DynamicCustomOp

        Protected Friend blocks() As Integer
        Protected Friend padding()() As Integer

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal blocks() As Integer, ByVal padding()() As Integer, ByVal inPlace As Boolean)
			MyBase.New(Nothing, sameDiff, args, inPlace)

			Me.blocks = blocks
			Me.padding = padding

			For Each b As val In blocks
				addIArgument(b)
			Next b

			For e As Integer = 0 To padding.Length - 1
				addIArgument(padding(e)(0), padding(e)(1))
			Next e
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "space_to_batch_nd"
        End Function

        Public Overrides Function onnxName() As String
            Return "space_to_batch_nd"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "SpaceToBatchND"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			' Inverse of space to batch is batch to space with same blocks and crops as padding
			Dim gradient As SDVariable = sameDiff.setupFunction(i_v(0))
			Return New List(Of SDVariable) From {sameDiff.cnn().batchToSpace(gradient, blocks, padding(0), padding(1))}
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
