Imports System
Imports System.Collections.Generic
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

    ''' <summary>The legacy random op <c>linspace_random</c> (Java <c>org.nd4j.linalg.api.ops.random.impl.Linspace</c>).</summary>
    Public Class Linspace
        Inherits BaseRandomOp

        Private from As Double
        Private [to] As Double
        Private [step] As Double
        Private length As Long

        Public Sub New()
            ' no-op
        End Sub

        Public Sub New(ByVal from As Double, ByVal length As Long, ByVal [step] As Double, ByVal dataType As DataType)
            Me.New(Nd4vbT.createUninitialized(dataType, New Long() {length}, Nd4vbT.order()), from, from, [step])
        End Sub

        Public Sub New(ByVal from As Double, ByVal [to] As Double, ByVal length As Long, ByVal dataType As DataType)
            Me.New(Nd4vbT.createUninitialized(dataType, New Long() {length}, Nd4vbT.order()), from, [to])
        End Sub

        Public Sub New(ByVal z As INDArray, ByVal from As Double, ByVal [to] As Double)
            MyBase.New(Nothing, Nothing, z)
            Me.from = from
            Me.to = [to]
            Me.length = z.length()
            Dim [step] As Double = 0.0
            Me.extraArgsField = New Object() {from, [to], [step]}
        End Sub

        Public Sub New(ByVal z As INDArray, ByVal from As Double, ByVal [to] As Double, ByVal [step] As Double)
            MyBase.New(Nothing, Nothing, z)
            Me.from = from
            Me.to = [to]
            Me.length = z.length()
            Me.step = [step]
            Me.extraArgsField = New Object() {from, [to], [step]}
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sd As SameDiff, ByVal from As Double, ByVal [to] As Double, ByVal length As Long)
			MyBase.New(sd, New Long(){length})
			Me.sameDiff = sd
			Me.from = from
			Me.to = [to]
			Me.length = length
			Dim [step] As Double = 0.0 '(to - from) / (length - 1);
			Me.extraArgs = New Object() {from, [to], [step]}
		End Sub
#End If

        Public Overrides Function opNum() As Integer
            Return 4
        End Function

        Public Overrides Function opName() As String
            Return "linspace_random"
        End Function

        Public Overrides Function x() As INDArray
            'Workaround/hack for: https://github.com/eclipse/deeplearning4j/issues/6723
            'If x or y is present, can't execute this op properly (wrong signature is used)
            Return Nothing
        End Function

        Public Overrides Function y() As INDArray
            'Workaround/hack for: https://github.com/eclipse/deeplearning4j/issues/6723
            'If x or y is present, can't execute this op properly (wrong signature is used)
            Return Nothing
        End Function

        Public Overrides Sub setX(ByVal x As INDArray)
            'Workaround/hack for: https://github.com/eclipse/deeplearning4j/issues/6723
            'If x or y is present, can't execute this op properly (wrong signature is used)
            Me.xField = Nothing
        End Sub

        Public Overrides Sub setY(ByVal y As INDArray)
            'Workaround for: https://github.com/eclipse/deeplearning4j/issues/6723
            'If x or y is present, can't execute this op properly (wrong signature is used)
            Me.yField = Nothing
        End Sub

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
        End Function

        Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
            Return calculateOutputShape()
        End Function

        Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
            Dim longShapeDescriptor As LongShapeDescriptor = LongShapeDescriptor.fromShape(shape, dataTypeField)
            Return New List(Of LongShapeDescriptor) From {longShapeDescriptor}
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			'No inputs
			Return Collections.emptyList()
		End Function
#End If

    End Class

End Namespace
