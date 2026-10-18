Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports BaseTransformSameOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.BaseTransformSameOp
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.pairwise

    ''' <summary>The legacy transform same op <c>BinaryMinimalRelativeError</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.pairwise.BinaryMinimalRelativeError</c>).</summary>
    Public Class BinaryMinimalRelativeError
        Inherits BaseTransformSameOp

        Private thresholdRelative As Double = 0.0
        Private thresholdAbsolute As Double = 0.0

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable)
			MyBase.New(sameDiff, i_v1, i_v2)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, i_v1, i_v2, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, i_v, inPlace)
		End Sub
#End If

        Public Sub New()
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal thresholdRelative As Double, ByVal thresholdAbsolute As Double)
            MyBase.New(x, y, z)
            Me.thresholdRelative = thresholdRelative
            Me.thresholdAbsolute = thresholdAbsolute
            Me.extraArgsField = New Object() {thresholdRelative, thresholdAbsolute}
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal thresholdRelative As Double, ByVal thresholdAbsolute As Double)
            MyBase.New(x, y, x)
            Me.thresholdRelative = thresholdRelative
            Me.thresholdAbsolute = thresholdAbsolute
            Me.extraArgsField = New Object() {thresholdRelative, thresholdAbsolute}
        End Sub

        Public Overrides Function opNum() As Integer
            Return 28
        End Function

        Public Overrides Function opName() As String
            Return "BinaryMinimalRelativeError"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No  onnx opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No  onnx opName found for " & opName())
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException()
		End Function
#End If

    End Class

End Namespace
