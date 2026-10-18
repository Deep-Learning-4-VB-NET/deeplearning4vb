Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports BaseDynamicTransformOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms.BaseDynamicTransformOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports MergeAddBp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms.pairwise.arithmetic.bp.MergeAddBp
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.pairwise.arithmetic

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.MergeAddOp</c>.</summary>
    Public Class MergeAddOp
        Inherits BaseDynamicTransformOp

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, args, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable)
			Me.New(sameDiff, args, False)
		End Sub
#End If

        Public Sub New(ParamArray ByVal inputs() As INDArray)
            Me.New(inputs, Nothing)
        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray)
            MyBase.New(inputs, outputs)
        End Sub

        Public Overrides Function opName() As String
            Return "mergeadd"
        End Function

        Public Overrides Function onnxName() As String
            Return "mergeadd"
        End Function

        Public Overrides Function tensorflowNames() As String()
            Return New String(){"add_n", "AccumulateNV2"}
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			 Return New List(Of SDVariable) From {(New MergeAddBp(sameDiff, args(), i_v(0))).outputVariables()}

		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Dim first As DataType = dataTypes(0)
            For i As Integer = 1 To dataTypes.Count - 1
                Preconditions.checkState(first = dataTypes(i), "Expected all input datatypes to be the same: first input is %s, input %s is %s", first, i, dataTypes(i))
            Next i
            Return New List(Of DataType) From {first}
        End Function

    End Class

End Namespace
