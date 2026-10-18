Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
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

Namespace org.nd4vb.linalg.api.ops.impl.shape

    ''' <summary>The custom op <c>eye</c> (Java <c>org.nd4j.linalg.api.ops.impl.shape.Eye</c>).</summary>
    Public Class Eye
        Inherits DynamicCustomOp

        Public Const DEFAULT_DTYPE As DataType = DataType.FLOAT

        Private numRows As Integer
        Private numCols As Integer
        Private batchDimension() As Integer = {}
        Private dataTypeField As DataType? = DEFAULT_DTYPE

        Public Sub New()
        End Sub

        Public Sub New(ByVal rows As INDArray)
            Me.New(rows.getInt(0))
            Preconditions.checkArgument(rows.Scalar, "Rows INDArray must be a scalar")
        End Sub

        Public Sub New(ByVal rows As INDArray, ByVal columns As INDArray)
            Me.New(rows.getInt(0), columns.getInt(0))
            Preconditions.checkArgument(rows.Scalar, "Rows INDArray must be a scalar")
            Preconditions.checkArgument(columns.Scalar, "Columns INDArray must be a scalar")
        End Sub

        Public Sub New(ByVal rows As Integer)
            Me.numRows = rows
            Me.numCols = rows
            addArgs()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal numRows As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable() {numRows}, False)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal numRows As SDVariable, ByVal numCols As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable() {numRows, numCols}, False)
		End Sub
		Public Sub New(ByVal sameDiff As SameDiff, ByVal numRows As SDVariable, ByVal numCols As SDVariable, ByVal batch_shape As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable() {numRows, numCols, batch_shape}, False)
		End Sub
		Public Sub New(ByVal sameDiff As SameDiff, ByVal numRows As Integer)
			Me.New(sameDiff, numRows, numRows)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal numRows As Integer, ByVal numCols As Integer)
			Me.New(sameDiff, numRows, numCols, DEFAULT_DTYPE)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal numRows As Integer, ByVal numCols As Integer, ByVal dataType As DataType)
			Me.New(sameDiff, numRows, numCols, dataType, Nothing)
		End Sub
#End If

        Public Sub New(ByVal numRows As Integer, ByVal numCols As Integer, ByVal dataType As DataType, ByVal batchDimension() As Integer)
            Me.numRows = numRows
            Me.numCols = numCols
            Me.batchDimension = batchDimension
            Me.dataTypeField = dataType
            addArgs()
        End Sub

        Public Sub New(ByVal numRows As Integer, ByVal numCols As Integer)
            Me.New(numRows, numCols, DEFAULT_DTYPE)
        End Sub

        Public Sub New(ByVal numRows As Integer, ByVal numCols As Integer, ByVal dataType As DataType)
            Me.New(numRows, numCols, dataType, Nothing)
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal numRows As Integer, ByVal numCols As Integer, ByVal dataType As DataType, ByVal batchDimension() As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable() {}, False)
			Me.numRows = numRows
			Me.numCols = numCols
			Me.batchDimension = batchDimension
			Me.dataType = dataType
			addArgs()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal numRows As SDVariable, ByVal numCols As SDVariable, ByVal dataType As DataType, ByVal batchDimension() As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable() {numRows, numCols}, False)
			Me.batchDimension = batchDimension
			Me.dataType = dataType
			addArgs()
		End Sub
#End If

        Protected Friend Overridable Sub addArgs()
            iArguments.Clear()
            tArguments.Clear()

            addIArgument(numRows)
            addIArgument(numCols)
            If batchDimension IsNot Nothing Then
                For Each [dim] As Integer In batchDimension
                    addIArgument([dim])
                Next [dim]
            End If

            addTArgument(CDbl(dataTypeField.Value.toInt()))
        End Sub

        Public Overrides Function opName() As String
            Return "eye"
        End Function

        Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
            Dim l As IList(Of LongShapeDescriptor) = MyBase.calculateOutputShape()
            If dataTypeField.HasValue AndAlso l IsNot Nothing AndAlso l.Count > 0 Then
                l(0) = l(0).asDataType(dataTypeField)
            End If
            Return l
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal outGrad As IList(Of SDVariable)) As IList(Of SDVariable)
			If arg() IsNot Nothing Then
				Return Collections.singletonList(sameDiff.onesLike(arg()))
			Else
				Return Collections.emptyList()
			End If
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Return New List(Of DataType) From {dataTypeField.GetValueOrDefault(DEFAULT_DTYPE)}
        End Function

    End Class

End Namespace
