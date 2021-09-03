Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Op = org.nd4j.linalg.api.ops.Op
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef

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

Namespace org.nd4j.linalg.api.ops.impl.shape.tensorops


	Public Class TensorArray
		Inherits BaseTensorOp

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.nd4j.linalg.api.buffer.DataType tensorArrayDataType;
		Protected Friend tensorArrayDataType As DataType
		Public Overrides Function tensorflowName() As String
			Return "TensorArrayV3"
		End Function

		Public Sub New(ByVal name As String, ByVal sameDiff As SameDiff, ByVal dataType As DataType)
			MyBase.New(name, sameDiff, New SDVariable(){})
			Me.tensorArrayDataType = dataType
		End Sub
		Public Sub New(ByVal sameDiff As SameDiff, ByVal dataType As DataType)
			MyBase.New(sameDiff, New SDVariable(){})
			Me.tensorArrayDataType = dataType
		End Sub

		Public Sub New(ByVal ta As TensorArray)
			MyBase.New(ta.sameDiff, New SDVariable(){})
			Me.tensorArrayDataType = ta.tensorArrayDataType
		End Sub
		Public Sub New(ByVal ta As TensorArray, ByVal inputs() As SDVariable)
			MyBase.New(ta.sameDiff, inputs)
			Me.tensorArrayDataType = ta.tensorArrayDataType
		End Sub

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Dim idd As val = nodeDef.getInput(nodeDef.getInputCount() - 1)
			Dim iddNode As NodeDef = Nothing
			Dim i As Integer = 0
			Do While i < graph.getNodeCount()
				If graph.getNode(i).getName().Equals(idd) Then
					iddNode = graph.getNode(i)
				End If
				i += 1
			Loop

			Dim arr As val = TFGraphMapper.getNDArrayFromTensor(iddNode)

			If arr IsNot Nothing Then
				Dim idx As Integer = arr.getInt(0)
				addIArgument(idx)
			End If

			Me.tensorArrayDataType = TFGraphMapper.convertType(attributesForNode("dtype").getType())
		End Sub


		Public Sub New()
			Me.New(DataType.FLOAT)
		End Sub

		Public Sub New(ByVal dataType As DataType)
			Me.tensorArrayDataType = dataType
		End Sub

		Public Overrides Function ToString() As String
			Return opName()
		End Function

		Public Overrides Function opName() As String
			Return "tensorarrayv3"
		End Function


		Public Overrides Function opType() As Op.Type
			Return Op.Type.CUSTOM
		End Function


		Private ReadOnly Property Var As SDVariable
			Get
				Return outputVariable()
			End Get
		End Property

		Public Overrides ReadOnly Property SameDiff As SameDiff
			Get
				Dim sd As val = Me.sameDiff
				If sd.getChild() IsNot Nothing Then
					Return sd.getChild()
				End If
				Return sd
			End Get
		End Property

		Private Function intToVar(ParamArray ByVal index() As Integer) As SDVariable
			Return Me.sameDiff.constant(Nd4j.createFromArray(index))
		End Function


		'----------- read ops-----------------\\
		Public Overridable Function read(ByVal index As Integer) As SDVariable
			Return (New TensorArrayRead(SameDiff, New SDVariable(){Var, intToVar(index)})).outputVariable()
		End Function
		Public Overridable Function read(ByVal index As SDVariable) As SDVariable
			Return (New TensorArrayRead(SameDiff, New SDVariable(){Var, index})).outputVariable()
		End Function
		Public Overridable Function gather(ByVal flow As SDVariable, ParamArray ByVal indices() As Integer) As SDVariable
			Return (New TensorArrayGather(SameDiff, New SDVariable(){Var, sameDiff.constant(Nd4j.createFromArray(indices)), flow})).outputVariable()
		End Function
		Public Overridable Function gather(ByVal flow As SDVariable, ByVal indices As SDVariable) As SDVariable
			Return (New TensorArrayGather(SameDiff, New SDVariable(){Var, indices, flow})).outputVariable()
		End Function
		Public Overridable Function stack(ByVal flow As SDVariable) As SDVariable
			Return (New TensorArrayGather(SameDiff, New SDVariable(){Var, intToVar(-1), flow})).outputVariable()
		End Function

		Public Overridable Function concat(ByVal flow As SDVariable) As SDVariable
			Return (New TensorArrayConcat(SameDiff, New SDVariable(){Var})).outputVariable()
		End Function

		'----------- write ops-----------------\\
		Public Overridable Function write(ByVal flow As SDVariable, ByVal index As Integer, ByVal value As SDVariable) As SDVariable
			Return write(flow, intToVar(index), value)
		End Function

		Public Overridable Function write(ByVal flow As SDVariable, ByVal index As SDVariable, ByVal value As SDVariable) As SDVariable
			Return (New TensorArrayWrite(SameDiff, New SDVariable(){Var, index, value, flow})).outputVariable()
		End Function

		Public Overridable Function scatter(ByVal flow As SDVariable, ByVal value As SDVariable, ParamArray ByVal indices() As Integer) As SDVariable
			Return (New TensorArrayScatter(SameDiff, New SDVariable(){Var, intToVar(indices), value, flow})).outputVariable()
		End Function

		Public Overridable Function scatter(ByVal flow As SDVariable, ByVal value As SDVariable, ByVal indices As SDVariable) As SDVariable
			Return (New TensorArrayScatter(SameDiff, New SDVariable(){Var, indices, value, flow})).outputVariable()
		End Function

		Public Overridable Function unstack(ByVal flow As SDVariable, ByVal value As SDVariable) As SDVariable
			Return (New TensorArrayScatter(SameDiff, New SDVariable(){Var, intToVar(-1), value, flow})).outputVariable()
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataType As IList(Of DataType)) As IList(Of DataType)
			'The SDVariable that is the output of this "function" is just a dummy variable anyway...
			'Usually 2 outputs... seems like first one is dummy, second one is a float??
			'TODO work out exactly what this second output is for (it's used in TensorArrayWrite for example...
			Return New List(Of DataType) From {DataType.BOOL, DataType.FLOAT}
		End Function

		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				Return 2
			End Get
		End Property
	End Class

End Namespace