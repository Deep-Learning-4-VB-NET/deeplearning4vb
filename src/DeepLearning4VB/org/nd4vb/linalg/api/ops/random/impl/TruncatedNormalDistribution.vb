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

    ''' <summary>The legacy random op <c>distribution_truncated</c> (Java <c>org.nd4j.linalg.api.ops.random.impl.TruncatedNormalDistribution</c>).</summary>
    Public Class TruncatedNormalDistribution
        Inherits BaseRandomOp

        Private mean As Double
        Private stddev As Double

        Public Sub New()
            MyBase.New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sd As SameDiff, ByVal mean As Double, ByVal stddev As Double, ByVal shape() As Long)
			MyBase.New(sd, shape)
			Me.mean = mean
			Me.stddev = stddev
			Me.extraArgs = New Object() {Me.mean, Me.stddev}
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal mean As Double, ByVal stddev As Double, ByVal dataType As DataType, ByVal shape() As Long)
			MyBase.New(sd, shape)
			Me.mean = mean
			Me.stddev = stddev
			Me.extraArgs = New Object() {Me.mean, Me.stddev}
		End Sub
#End If

        Public Sub New(ByVal mean As Double, ByVal stddev As Double, ByVal datatype As DataType, ParamArray ByVal shape() As Long)
            Me.New(Nd4vbT.createUninitialized(datatype, shape), mean, stddev)
        End Sub

        ''' <summary>
        ''' This op fills Z with random values within stddev..mean..stddev boundaries </summary>
        ''' <param name="z"> </param>
        ''' <param name="mean"> </param>
        ''' <param name="stddev"> </param>
        Public Sub New(ByVal z As INDArray, ByVal mean As Double, ByVal stddev As Double)
            MyBase.New(z,z,z)
            Me.mean = mean
            Me.stddev = stddev
            Me.extraArgsField = New Object() {Me.mean, Me.stddev}
        End Sub

        Public Sub New(ByVal z As INDArray, ByVal means As INDArray, ByVal stddev As Double)
            MyBase.New(z, means, z)
            If z.length() <> means.length() Then
                Throw New System.InvalidOperationException("Result length should be equal to provided Means length")
            End If

            If means.elementWiseStride() < 1 Then
                Throw New System.InvalidOperationException("Means array can't have negative EWS")
            End If

            Me.mean = 0.0
            Me.stddev = stddev
            Me.extraArgsField = New Object() {Me.mean, Me.stddev}
        End Sub

        ''' <summary>
        ''' This op fills Z with random values within -1.0..0..1.0 </summary>
        ''' <param name="z"> </param>
        Public Sub New(ByVal z As INDArray)
            Me.New(z, 0.0, 1.0)
        End Sub

        ''' <summary>
        ''' This op fills Z with random values within stddev..0..stddev </summary>
        ''' <param name="z"> </param>
        Public Sub New(ByVal z As INDArray, ByVal stddev As Double)
            Me.New(z, 0.0, stddev)
        End Sub

        Public Overrides Function opNum() As Integer
            Return 11
        End Function

        Public Overrides Function opName() As String
            Return "distribution_truncated"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
        End Function

        Public Overrides Sub setZ(ByVal z As INDArray)
            'We want all 3 args set to z for this op
            Me.xField = z
            Me.yField = z
            Me.zField = z
        End Sub

        Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
            Return calculateOutputShape()
        End Function

        Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
            Dim longShapeDescriptor As LongShapeDescriptor = LongShapeDescriptor.fromShape(shape, dataTypeField)
            Return New List(Of LongShapeDescriptor) From {longShapeDescriptor}
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.emptyList()
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes Is Nothing OrElse inputDataTypes.Count = 0, "Expected no input datatypes (no args) for %s, got %s", Me.GetType(), inputDataTypes)
            'Input data type specifies the shape; output data type should be any float
            'TODO MAKE CONFIGUREABLE - https://github.com/eclipse/deeplearning4j/issues/6854
            Return New List(Of DataType) From {DataType.DOUBLE}
        End Function

        Public Overrides ReadOnly Property TripleArgRngOp As Boolean
            Get
                Return True
            End Get
        End Property

    End Class

End Namespace
