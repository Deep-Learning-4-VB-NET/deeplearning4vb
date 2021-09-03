Imports System.Collections.Generic
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.custom



	Public Class BatchToSpaceND
		Inherits DynamicCustomOp

		Private blocks() As Integer
		Private crops()() As Integer

		Public Sub New()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal blocks() As Integer, ByVal crops()() As Integer, ByVal inPlace As Boolean)
			MyBase.New(Nothing, sameDiff, args, inPlace)

			Me.blocks = blocks
			Me.crops = crops

			For Each b As val In blocks
				addIArgument(b)
			Next b

			For e As Integer = 0 To crops.Length - 1
				addIArgument(crops(e)(0), crops(e)(1))
			Next e
		End Sub

		Public Overrides Function opName() As String
			Return "batch_to_space_nd"
		End Function

		Public Overrides Function onnxName() As String
			Return "batch_to_space_nd"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "BatchToSpaceND"
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			' Inverse of batch to space is space to batch with same blocks and padding as crops
			Dim gradient As SDVariable = sameDiff.setupFunction(i_v(0))
			Return New List(Of SDVariable) From {sameDiff.cnn().spaceToBatch(gradient, blocks, crops(0), crops(1))}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Return Collections.singletonList(dataTypes(0))
		End Function
	End Class

End Namespace