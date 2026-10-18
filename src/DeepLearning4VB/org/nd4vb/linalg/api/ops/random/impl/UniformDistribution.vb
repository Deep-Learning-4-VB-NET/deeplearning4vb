Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports OpContext = DeepLearning4VB.org.nd4vb.linalg.api.ops.OpContext
Imports BaseRandomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.random.BaseRandomOp
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
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

Namespace org.nd4vb.linalg.api.ops.random.impl

    ''' <summary>The legacy random op <c>distribution_uniform</c> (Java <c>org.nd4j.linalg.api.ops.random.impl.UniformDistribution</c>).</summary>
    Public Class UniformDistribution
        Inherits BaseRandomOp

        Private from As Double
        Private [to] As Double

        Public Sub New()
            MyBase.New()
        End Sub

#If SAMEDIFF Then
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
#End If

        Public Sub New(ByVal min As Double, ByVal max As Double, ByVal datatype As DataType, ParamArray ByVal shape() As Long)
            Me.New(Nd4vbT.createUninitialized(datatype, shape), min, max)
            Me.shape = shape
        End Sub

        ''' <summary>
        ''' This op fills Z with random values within from...to boundaries </summary>
        ''' <param name="z"> </param>
        ''' <param name="from"> </param>
        ''' <param name="to"> </param>
        Public Sub New(ByVal z As INDArray, ByVal from As Double, ByVal [to] As Double)
            MyBase.New(Nothing, Nothing, z)
            Me.from = from
            Me.to = [to]
            Me.extraArgsField = New Object() {Me.from, Me.to}
            Me.shape = z.shape()
        End Sub

        ''' <summary>
        ''' This op fills Z with random values within 0...1 </summary>
        ''' <param name="z"> </param>
        Public Sub New(ByVal z As INDArray)
            Me.New(z, 0.0, 1.0)
        End Sub

        ''' <summary>
        ''' This op fills Z with random values within 0...to </summary>
        ''' <param name="z"> </param>
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

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.emptyList()
		End Function
#End If

        Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
            Return calculateOutputShape()
        End Function

        Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
            Dim longShapeDescriptor As LongShapeDescriptor = LongShapeDescriptor.fromShape(shape, dataTypeField)
            Return New List(Of LongShapeDescriptor) From {longShapeDescriptor}
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes Is Nothing OrElse inputDataTypes.Count = 0, "Expected no input datatypes (no args) for %s, got %s", Me.GetType(), inputDataTypes)
            'Input data type specifies the shape; output data type should be any float
            'TODO MAKE CONFIGUREABLE - https://github.com/eclipse/deeplearning4j/issues/6854
            Return New List(Of DataType) From {dataTypeField}
        End Function

    End Class

End Namespace
