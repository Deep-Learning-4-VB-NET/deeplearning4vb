Imports System
Imports System.Collections.Generic
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

Namespace org.nd4vb.linalg.api.ops.impl.shape

    ''' <summary>
    ''' GatherND op
    ''' </summary>
    Public Class GatherNd
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal indices As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable() {input, indices})
		End Sub
#End If

        Public Sub New(ByVal df As INDArray, ByVal indices As INDArray)
            MyBase.New(New INDArray(){df, indices}, Nothing)
        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray)
            MyBase.New(inputs, outputs)
        End Sub

        Public Overrides Function opName() As String
            Return "gather_nd"
        End Function

        Public Overrides Function onnxName() As String
            Return "GatherND"
        End Function

        Public Overrides Function tensorflowNames() As String()
            Return New String(){"GatherNd"}
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            'Output type is same as (first) input type
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
