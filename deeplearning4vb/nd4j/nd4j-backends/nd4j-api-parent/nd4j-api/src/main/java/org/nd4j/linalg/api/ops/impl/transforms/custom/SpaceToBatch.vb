Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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



	Public Class SpaceToBatch
		Inherits DynamicCustomOp

		Protected Friend blocks() As Integer
		Protected Friend padding()() As Integer

		Public Sub New()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal blocks() As Integer, ByVal paddingTop() As Integer, ParamArray ByVal paddingBottom() As Integer)
			Me.New(sameDiff, New SDVariable(){x}, blocks, New Integer()(){paddingBottom, paddingBottom}, False)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal blocks() As Integer, ByVal padding()() As Integer, ByVal inPlace As Boolean)
			MyBase.New(Nothing, sameDiff, New SDVariable(){args(0), sameDiff.constant(Nd4j.createFromArray(padding))}, inPlace)

			Me.blocks = blocks
			Me.padding = padding

			addIArgument(blocks(0))
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal blocks() As Integer, ByVal paddingTop() As Integer, ParamArray ByVal paddingBottom() As Integer)
			addInputArgument(x)
			Me.blocks = blocks
			Me.padding = padding

			addIArgument(blocks(0))
		End Sub

		Public Overrides Function opName() As String
			Return "space_to_batch"
		End Function

		Public Overrides Function onnxName() As String
			Return "space_to_batch"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "SpaceToBatch"
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			' Inverse of space to batch is batch to space with same blocks and crops as padding
			Dim gradient As SDVariable = sameDiff.setupFunction(i_v(0))
			Return New List(Of SDVariable) From {sameDiff.cnn().batchToSpace(gradient, blocks, padding(0), padding(1))}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Return Collections.singletonList(dataTypes(0))
		End Function

	End Class

End Namespace