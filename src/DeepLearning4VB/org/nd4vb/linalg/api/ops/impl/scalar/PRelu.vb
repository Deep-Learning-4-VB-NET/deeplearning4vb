Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports PReluBp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms.gradient.PReluBp
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

Namespace org.nd4vb.linalg.api.ops.impl.scalar

    ''' <summary>
    ''' Parameterized ReLU op
    ''' </summary>
    Public Class PRelu
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Protected Friend sharedAxes() As Integer

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public PRelu(@NonNull SameDiff sameDiff, @NonNull SDVariable x, @NonNull SDVariable alpha, @NonNull int... sharedAxes)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal alpha As SDVariable, ParamArray ByVal sharedAxes() As Integer)
			MyBase.New(sameDiff, New SDVariable(){x, alpha})
			Me.sharedAxes = sharedAxes
			addIArgument(sharedAxes)
		End Sub
#End If

        Public Sub New(ByVal x As INDArray, ByVal alpha As INDArray, ParamArray ByVal sharedAxes() As Integer)
            Me.New(x, Nothing, alpha, sharedAxes)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal alpha As INDArray, ParamArray ByVal sharedAxes() As Integer)
            MyBase.New(New INDArray(){x, alpha}, New INDArray(){z})
            Me.sharedAxes = sharedAxes
            addIArgument(sharedAxes)
        End Sub

        Public Overrides Function opName() As String
            Return "prelu"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkArgument(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes, got %s", dataTypes)
            Preconditions.checkArgument(dataTypes(0).isFPType() AndAlso dataTypes(1).isFPType(), "Input datatypes must be floating point, got %s", dataTypes)

            Return New List(Of DataType) From {dataTypes(0)}
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New PReluBp(sameDiff, arg(0), arg(1), i_v(0), sharedAxes)).outputs()
		End Function
#End If

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>sharedAxes</c>.</summary>
        Public Overridable Function getSharedAxes() As Integer()
            Return sharedAxes
        End Function

    End Class

End Namespace
