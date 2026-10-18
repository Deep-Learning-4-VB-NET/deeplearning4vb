Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports BaseDynamicTransformOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms.BaseDynamicTransformOp
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.custom

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.Min</c>.</summary>
    Public Class Min
        Inherits BaseDynamicTransformOp

        Public Sub New()
        End Sub

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Min(org.nd4j.autodiff.samediff.SameDiff sameDiff, @NonNull SDVariable first, @NonNull SDVariable second)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal first As SDVariable, ByVal second As SDVariable)
			Me.New(sameDiff, New SDVariable(){first, second}, False)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, args, inPlace)
		End Sub
#End If

        Public Sub New(ByVal first As INDArray, ByVal second As INDArray, ByVal [out] As INDArray)
            MyBase.New(New INDArray(){first, second},If([out] Is Nothing, Nothing, New INDArray() {[out]}))
        End Sub

        Public Sub New(ByVal first As INDArray, ByVal second As INDArray)
            Me.New(first, second, Nothing)
        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray)
            MyBase.New(inputs, outputs)
        End Sub

        Public Overrides Function opName() As String
            Return "minimum"
        End Function

        Public Overrides Function onnxName() As String
           Return "Min"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "Minimum"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			'TODO Switch to minimum_bp op - https://github.com/eclipse/deeplearning4j/blob/master/libnd4j/include/ops/declarable/generic/broadcastable/minimum.cpp
			Dim min As SDVariable = outputVariables()(0)
			Dim eq1 As SDVariable = sameDiff.eq(larg(), min).castTo(arg(0).dataType())
			Dim eq2 As SDVariable = sameDiff.eq(rarg(), min).castTo(arg(1).dataType())

			Return New List(Of SDVariable) From {eq1.mul(f1(0)), eq2.mul(f1(0))}
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes for %s, got %s", Me.GetType(), dataTypes)
            Preconditions.checkState(dataTypes(0) = dataTypes(1), "Input datatypes must be the same, got %s", dataTypes)
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
