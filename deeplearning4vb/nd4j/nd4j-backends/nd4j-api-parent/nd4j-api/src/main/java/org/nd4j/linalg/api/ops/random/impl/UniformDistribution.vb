Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports BaseRandomOp = org.nd4j.linalg.api.ops.random.BaseRandomOp
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports ArrayOptionsHelper = org.nd4j.linalg.api.shape.options.ArrayOptionsHelper
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

Namespace org.nd4j.linalg.api.ops.random.impl


	Public Class UniformDistribution
		Inherits BaseRandomOp

		Private from As Double
		Private [to] As Double

		Public Sub New()
			MyBase.New()
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal from As Double, ByVal [to] As Double, ByVal shape() As Long)
			MyBase.New(sd, shape)
			Me.from = from
			Me.to = [to]
			Me.extraArgs = New Object() {Me.from, Me.to}
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal from As Double, ByVal [to] As Double, ByVal dataType As DataType, ByVal shape() As Long)
			Me.New(sd, from, [to], shape)
			Me.dataType = dataType
		End Sub

		Public Sub New(ByVal min As Double, ByVal max As Double, ByVal datatype As DataType, ParamArray ByVal shape() As Long)
			Me.New(Nd4j.createUninitialized(datatype, shape), min, max)
			Me.shape = shape
		End Sub

		''' <summary>
		''' This op fills Z with random values within from...to boundaries </summary>
		''' <param name="z"> </param>
		''' <param name="from"> </param>
		''' <param name="to"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public UniformDistribution(@NonNull INDArray z, double from, double to)
		Public Sub New(ByVal z As INDArray, ByVal from As Double, ByVal [to] As Double)
			MyBase.New(Nothing, Nothing, z)
			Me.from = from
			Me.to = [to]
			Me.extraArgs = New Object() {Me.from, Me.to}
			Me.shape = z.shape()
		End Sub

		''' <summary>
		''' This op fills Z with random values within 0...1 </summary>
		''' <param name="z"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public UniformDistribution(@NonNull INDArray z)
		Public Sub New(ByVal z As INDArray)
			Me.New(z, 0.0, 1.0)
		End Sub

		''' <summary>
		''' This op fills Z with random values within 0...to </summary>
		''' <param name="z"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public UniformDistribution(@NonNull INDArray z, double to)
		Public Sub New(ByVal z As INDArray, ByVal [to] As Double)
			Me.New(z, 0.0, [to])
		End Sub

		Public Overrides Function opNum() As Integer
			Return 0
		End Function

		Public Overrides Function opName() As String
			Return "distribution_uniform"
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.emptyList()
		End Function

		Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
			Return calculateOutputShape()
		End Function

		Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
			Dim longShapeDescriptor As LongShapeDescriptor = LongShapeDescriptor.fromShape(shape,dataType)
			Return New List(Of LongShapeDescriptor) From {longShapeDescriptor}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes Is Nothing OrElse inputDataTypes.Count = 0, "Expected no input datatypes (no args) for %s, got %s", Me.GetType(), inputDataTypes)
			'Input data type specifies the shape; output data type should be any float
			'TODO MAKE CONFIGUREABLE - https://github.com/eclipse/deeplearning4j/issues/6854
			Return Collections.singletonList(dataType)
		End Function
	End Class

End Namespace