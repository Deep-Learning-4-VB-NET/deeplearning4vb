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

Namespace org.nd4vb.linalg.api.ops.impl.controlflow

    ''' 
    Public Class Where
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable)
			MyBase.New(Nothing, sameDiff, args)
		End Sub
#End If

        Public Sub New(ByVal opName As String, ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal tArguments As IList(Of Double), ByVal iArguments As IList(Of Integer))
            MyBase.New(opName, inputs, outputs, tArguments, iArguments)
        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray)
            MyBase.New(Nothing, inputs, outputs)
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(Nothing, sameDiff, args, inPlace)
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "Where"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "Where"
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal inputTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputTypes IsNot Nothing AndAlso (inputTypes.Count = 1 OrElse inputTypes.Count = 3), "Expected 1 or 3 input types, got %s for op %s",inputTypes, Me.GetType())
            If inputTypes.Count = 3 Then
                Preconditions.checkState(inputTypes(1) = inputTypes(2), "X and Y input must be same type, got inputs %s for op %s", inputTypes, Me.GetType())
                'Output type same as x/y types
                Return New List(Of DataType) From {inputTypes(1)}
            Else
                'Coordinates of true elements
                'TODO allow this to be configured
                Return New List(Of DataType) From {DataType.LONG}
            End If
        End Function

    End Class

End Namespace
