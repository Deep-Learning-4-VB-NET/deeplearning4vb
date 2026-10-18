Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports OpContext = DeepLearning4VB.org.nd4vb.linalg.api.ops.OpContext
Imports BaseRandomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.random.BaseRandomOp
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
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

    ''' <summary>The legacy random op <c>alpha_dropout</c> (Java <c>org.nd4j.linalg.api.ops.random.impl.AlphaDropOut</c>).</summary>
    Public Class AlphaDropOut
        Inherits BaseRandomOp

        Private p As Double
        Private a As Double
        Private alphaPrime As Double
        Private b As Double

        Public Sub New()

        End Sub

        Public Sub New(ByVal x As INDArray, ByVal p As Double, ByVal alpha As Double, ByVal alphaPrime As Double, ByVal beta As Double)
            Me.New(x, x, p, alpha, alphaPrime, beta)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal p As Double, ByVal alpha As Double, ByVal alphaPrime As Double, ByVal beta As Double)
            MyBase.New(x,Nothing,z)
            Me.p = p
            Me.a = alpha
            Me.b = beta
            Me.alphaPrime = alphaPrime
            Me.extraArgsField = New Object() {p, a, b, alphaPrime}
        End Sub

        Public Overrides Function opNum() As Integer
            Return 12
        End Function

        Public Overrides Function opName() As String
            Return "alpha_dropout"
        End Function

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
			Return Nothing
		End Function
#End If

    End Class

End Namespace
