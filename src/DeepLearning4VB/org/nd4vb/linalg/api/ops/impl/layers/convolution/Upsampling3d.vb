Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
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

Namespace org.nd4vb.linalg.api.ops.impl.layers.convolution

    ''' <summary>
    ''' Upsampling3d operation
    ''' </summary>
    Public Class Upsampling3d
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Protected Friend ncdhw As Boolean
        Protected Friend scaleH As Integer
        Protected Friend scaleW As Integer
        Protected Friend scaleD As Integer

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal ncdhw As Boolean, ByVal scaleD As Integer, ByVal scaleH As Integer, ByVal scaleW As Integer)
			MyBase.New("upsampling3d",sameDiff, New SDVariable(){input})
			Me.ncdhw = ncdhw

			Me.scaleD = scaleD
			Me.scaleH = scaleH
			Me.scaleW = scaleW

			addIArgument(scaleD)
			addIArgument(scaleH)
			addIArgument(scaleW)
			addIArgument(scaleD)
			addIArgument(If(ncdhw, 1, 0))
		End Sub
#End If

        Public Sub New(ByVal input As INDArray, ByVal ncdhw As Boolean, ByVal scaleH As Integer, ByVal scaleW As Integer, ByVal scaleD As Integer)
            MyBase.New(New INDArray(){input}, Nothing)
            Me.ncdhw = ncdhw

            Me.scaleD = scaleD
            Me.scaleH = scaleH
            Me.scaleW = scaleW

            addIArgument(scaleD)
            addIArgument(scaleH)
            addIArgument(scaleW)
            addIArgument(scaleD)
            addIArgument(If(ncdhw, 0, 1))
        End Sub

        Public Overrides Function opName() As String
            Return "upsampling3d"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return New List(Of SDVariable) From {(New Upsampling3dBp(sameDiff, arg(0), f1(0), Me.ncdhw)).outputVariables()}
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 1, "Expected 1 input data type for %s, got %s", Me.GetType(), inputDataTypes)
            Return New List(Of DataType) From {inputDataTypes(0)}
        End Function

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>ncdhw</c>.</summary>
        Public Overridable Function isNcdhw() As Boolean
            Return ncdhw
        End Function

        ''' <summary>Lombok getter of <c>scaleH</c>.</summary>
        Public Overridable Function getScaleH() As Integer
            Return scaleH
        End Function

        ''' <summary>Lombok getter of <c>scaleW</c>.</summary>
        Public Overridable Function getScaleW() As Integer
            Return scaleW
        End Function

        ''' <summary>Lombok getter of <c>scaleD</c>.</summary>
        Public Overridable Function getScaleD() As Integer
            Return scaleD
        End Function

    End Class

End Namespace
