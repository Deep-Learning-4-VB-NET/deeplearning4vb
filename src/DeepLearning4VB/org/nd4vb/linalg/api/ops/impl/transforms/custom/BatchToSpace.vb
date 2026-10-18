Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.custom

    ''' <summary>The custom op <c>batch_to_space</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.BatchToSpace</c>).</summary>
    Public Class BatchToSpace
        Inherits DynamicCustomOp

        Private blocks() As Integer
        Private crops()() As Integer

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal blocks() As Integer, ByVal croppingTop() As Integer, ParamArray ByVal croppingBottom() As Integer)
			Me.New(sameDiff, x, blocks, New Integer()(){croppingTop, croppingBottom}, False)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal blocks() As Integer, ByVal crops()() As Integer, ByVal inPlace As Boolean)
			Me.New(sameDiff, New SDVariable(){x}, blocks, crops, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal blocks() As Integer, ByVal crops()() As Integer, ByVal inPlace As Boolean)
			MyBase.New(Nothing, sameDiff, New SDVariable(){args(0), sameDiff.constant(Nd4j.createFromArray(crops))}, inPlace)

			Me.blocks = blocks
			Me.crops = crops

			For Each b As val In blocks
				addIArgument(b)
			Next b
		End Sub
#End If

        Public Sub New(ByVal x As INDArray, ByVal blocks() As Integer, ByVal croppingTop() As Integer, ParamArray ByVal croppingBottom() As Integer)
            addInputArgument(x)
            Dim crops()() As Integer = {croppingTop, croppingBottom}
            Me.blocks = blocks
            Me.crops = crops

            For Each b In blocks
                addIArgument(b)
            Next b
        End Sub

        Public Overrides Function opName() As String
            Return "batch_to_space"
        End Function

        Public Overrides Function onnxName() As String
            Return "batch_to_space"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "BatchToSpace"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			' Inverse of batch to space is space to batch with same blocks and padding as crops
			Dim gradient As SDVariable = sameDiff.setupFunction(i_v(0))
			Return New List(Of SDVariable) From {sameDiff.cnn().spaceToBatch(gradient, blocks, crops(0), crops(1))}
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
