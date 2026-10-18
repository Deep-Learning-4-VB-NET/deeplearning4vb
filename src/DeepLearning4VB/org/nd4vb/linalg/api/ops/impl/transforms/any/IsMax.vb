Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.any

    ''' <summary>
    ''' [1, 2, 3, 1] -> [0, 0, 1, 0]
    ''' @author Adam Gibson
    ''' </summary>
    Public Class IsMax
        Inherits DynamicCustomOp

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable)
			MyBase.New(sameDiff, i_v)
		End Sub
#End If

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
            MyBase.New(New INDArray(){x}, New INDArray(){z})
        End Sub

        Public Sub New()
        End Sub

        Public Sub New(ByVal x As INDArray)
            Me.New(x, Nd4vbT.createUninitialized(DataType.BOOL, x.shape(), x.ordering()))
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer)
            Me.New(x, z)
            Me.addIArgument(dimensions)
        End Sub

        Public Sub New(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer)
            Me.New(x, Nd4vbT.createUninitialized(DataType.BOOL, x.shape(), x.ordering()), dimensions)
        End Sub

        Public Overrides Function opName() As String
            Return "ismax"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            'Also supports other types if say float array is provided as output array
            Return New List(Of DataType) From {DataType.BOOL}
        End Function

    End Class

End Namespace
