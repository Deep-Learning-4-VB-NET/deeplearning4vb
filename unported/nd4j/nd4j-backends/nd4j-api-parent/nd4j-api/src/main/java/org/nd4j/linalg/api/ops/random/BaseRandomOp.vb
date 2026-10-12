Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseOp = org.nd4j.linalg.api.ops.BaseOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports RandomOp = org.nd4j.linalg.api.ops.RandomOp
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Shape = org.nd4j.linalg.api.shape.Shape
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

Namespace org.nd4j.linalg.api.ops.random


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public abstract class BaseRandomOp extends org.nd4j.linalg.api.ops.BaseOp implements org.nd4j.linalg.api.ops.RandomOp
	Public MustInherit Class BaseRandomOp
		Inherits BaseOp
		Implements RandomOp

		Public Overrides MustOverride WriteOnly Property ExtraArgs As Object()
		Protected Friend shape() As Long
		Protected Friend dataType As DataType = Nd4j.defaultFloatingPointType()

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable)
			Preconditions.checkNotNull(i_v, "Input variable can't be null with this constructor")
			Me.sameDiff = sameDiff
			Me.xVertexId = i_v.name()
			If i_v.Shape IsNot Nothing Then
				Me.shape = i_v.Shape
			ElseIf i_v.Arr.shape() IsNot Nothing Then
				Me.shape = i_v.Arr.shape()
			End If
			sameDiff.addArgsFor(New String(){xVertexId},Me)
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal shape() As Long)
			MyBase.New(sd, Nothing)
			Preconditions.checkArgument(shape IsNot Nothing AndAlso shape.Length > 0, "Shape must be non-null, length > 0. Got: %s", shape)
			Me.sameDiff = sd
			Me.shape = shape
			setInstanceId()
			sameDiff.addArgsFor(New String(){}, Me)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
			MyBase.New(x,y,z)
		End Sub

		Public Overrides Function opType() As Type
			Return Type.RANDOM
		End Function

		Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
			Return calculateOutputShape(Nothing)
		End Function



		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Return Collections.singletonList(DataType.FLOAT)
		End Function

		Public Overrides ReadOnly Property InPlace As Boolean
			Get
				Return x_Conflict Is Nothing OrElse x_Conflict Is z_Conflict OrElse x_Conflict.data().pointer().address() = z_Conflict.data().pointer().address()
			End Get
		End Property

		Public Overridable ReadOnly Property TripleArgRngOp As Boolean
			Get
				Return False
			End Get
		End Property
	End Class

End Namespace