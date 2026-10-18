Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports BaseOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.BaseOp
Imports RandomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.RandomOp
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports Shape = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape
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

Namespace org.nd4vb.linalg.api.ops.random

    ''' <summary>
    ''' The base class of the legacy random ops (<c>RANDOM</c>): <see cref="shape"/> and <see cref="dataTypeField"/> (Java field <c>dataType</c>) describe
    ''' the output when there is no z array. Java's <c>isInPlace()</c> compared native buffer addresses; here it
    ''' compares the underlying storage and offset (<see cref="isInPlace"/>).
    ''' </summary>
    Public MustInherit Class BaseRandomOp
        Inherits BaseOp
        Implements RandomOp

        Public Sub New()
        End Sub

        Protected Friend shape() As Long
        ''' <summary>The output data type (Java field <c>dataType</c>).</summary>
        Protected Friend dataTypeField As DataType = Nd4vbT.defaultFloatingPointType()

#If SAMEDIFF Then
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
#End If

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
            MyBase.New(x,y,z)
        End Sub

        Public Overrides Function opType() As Op.Type
            Return Op.Type.RANDOM
        End Function

        Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
            Return calculateOutputShape(Nothing)
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Return New List(Of DataType) From {DataType.FLOAT}
        End Function

        ''' <summary>
        ''' True when the op writes into x: x is not set, z is x, or z and x share the same storage at the same offset
        ''' (Java compared the native buffer addresses).
        ''' </summary>
        Public Overrides Function isInPlace() As Boolean
            Return xField Is Nothing OrElse xField Is zField OrElse (xField.data().sameUnderlyingData(zField.data()) AndAlso xField.data().offset() = zField.data().offset())
        End Function

        Public Overridable ReadOnly Property TripleArgRngOp As Boolean
            Get
                Return False
            End Get
        End Property

    End Class

End Namespace
