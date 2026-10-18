Imports System
Imports System.Collections.Generic
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports OpContext = DeepLearning4VB.org.nd4vb.linalg.api.ops.OpContext
Imports BaseRandomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.random.BaseRandomOp
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
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

    ''' <summary>The legacy random op <c>dropout</c> (Java <c>org.nd4j.linalg.api.ops.random.impl.DropOut</c>).</summary>
    Public Class DropOut
        Inherits BaseRandomOp

        Public Sub New()
        End Sub

        Private p As Double

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal p As Double)
			MyBase.New(sameDiff, input)
			Me.p = p
			Me.extraArgs = New Object() {p}

		End Sub
#End If

        Public Sub New(ByVal x As INDArray, ByVal p As Double)
            Me.New(x, x, p)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal p As Double)
            MyBase.New(x,Nothing,z)
            Me.p = p
            Me.extraArgsField = New Object() {p}
        End Sub

        Public Overrides Function opNum() As Integer
            Return 1
        End Function

        Public Overrides Function opName() As String
            Return "dropout"
        End Function

        Public Overrides Function opType() As Op.Type
            Return Op.Type.RANDOM
        End Function

        Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
            Dim input As INDArray = oc.getInputArray(0)
            Return New List(Of LongShapeDescriptor) From {input.shapeDescriptor()}
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not supported") 'We should only use *inverted* dropout with samediff
		End Function
#End If

    End Class

End Namespace
