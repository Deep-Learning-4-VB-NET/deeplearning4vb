Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.gradient

    ''' <summary>The custom op <c>thresholdedrelu_bp</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.gradient.ThresholdReluBp</c>).</summary>
    Public Class ThresholdReluBp
        Inherits DynamicCustomOp

        Private cutoff As Double = 0

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sd As SameDiff, ByVal input As SDVariable, ByVal gradient As SDVariable, ByVal cutoff As Double)
			MyBase.New(sd, New SDVariable(){input, gradient})
			Me.cutoff = cutoff
			addTArgument(cutoff)
		End Sub
#End If

        Public Sub New(ByVal input As INDArray, ByVal gradient As INDArray, ByVal output As INDArray, ByVal cutoff As Double)
            MyBase.New(New INDArray(){input, gradient}, wrapOrNull(output))
            Me.cutoff = cutoff
            addTArgument(cutoff)
        End Sub

        Public Overrides Function opName() As String
            Return "thresholdedrelu_bp"
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkArgument(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes, got %s", dataTypes)
            Preconditions.checkArgument(dataTypes(0).isFPType() AndAlso dataTypes(1).isFPType(), "Input datatypes must be floating point, got %s", dataTypes)

            Return New List(Of DataType) From {dataTypes(0)}
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not supported")
		End Function
#End If

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>cutoff</c>.</summary>
        Public Overridable Function getCutoff() As Double
            Return cutoff
        End Function

    End Class

End Namespace
