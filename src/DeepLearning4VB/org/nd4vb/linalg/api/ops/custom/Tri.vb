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

Namespace org.nd4vb.linalg.api.ops.custom

    ''' <summary>The custom op <c>tri</c> (Java <c>org.nd4j.linalg.api.ops.custom.Tri</c>).</summary>
    Public Class Tri
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Private dataTypeField As DataType = DataType.FLOAT

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal row As Integer, ByVal column As Integer, ByVal diag As Integer)
			MyBase.New(sameDiff, New SDVariable(){})
			addIArgument(row,column,diag)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal dataType As DataType, ByVal row As Integer, ByVal column As Integer, ByVal diag As Integer)
			MyBase.New(sameDiff, New SDVariable(){})
			addIArgument(row,column,diag)
			addDArgument(dataType)
			Me.dataType = dataType

		End Sub
#End If

        Public Sub New(ByVal row As Integer, ByVal column As Integer, ByVal diag As Integer)
            MyBase.New(New INDArray(){}, Nothing)
            addIArgument(row,column,diag)

        End Sub

        Public Sub New(ByVal dataType As DataType, ByVal row As Integer, ByVal column As Integer, ByVal diag As Integer)
            MyBase.New(New INDArray(){}, Nothing)
            addIArgument(row,column,diag)
            addDArgument(dataType)
            Me.dataTypeField = dataType

        End Sub

        Public Overrides Function opName() As String
            Return "tri"
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)

            Return New List(Of DataType) From {Me.dataTypeField}

        End Function

    End Class

End Namespace
