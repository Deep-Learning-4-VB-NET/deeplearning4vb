Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
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

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.ATan2</c>.</summary>
    Public Class ATan2
        Inherits BaseDynamicTransformOp

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal y As SDVariable, ByVal x As SDVariable)
			MyBase.New(sameDiff, New SDVariable() {y, x},False)
		End Sub
#End If

        ''' <summary>
        ''' Note that the order of x and y match <seealso cref="Math.atan2(Double, Double)"/>,
        ''' and are reversed when compared to OldATan2.
        ''' See <seealso cref="TransformsT.atan2(INDArray, INDArray)"/>
        ''' </summary>
        Public Sub New(ByVal x As INDArray, ByVal y As INDArray)
            Me.New(x,y,Nothing)
        End Sub

        ''' <summary>
        ''' Note that the order of x and y match <seealso cref="Math.atan2(Double, Double)"/>,
        ''' and are reversed when compared to OldATan2.
        ''' See <seealso cref="TransformsT.atan2(INDArray, INDArray)"/>
        ''' </summary>
        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
            MyBase.New(New INDArray(){x, y}, wrapOrNull(z))
        End Sub

        Public Sub New()
        End Sub

        Public Overrides Function opName() As String
            Return "tf_atan2"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Return "Atan2"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			'Let z=atan2(r), with r=y/x
			'dz/dr = 1/(r^2+1), dr/dy = 1/x, dr/dx = -y/x^2
			Dim y As SDVariable = larg()
			Dim x As SDVariable = rarg()

			Dim xGrad As val = sameDiff.math_Conflict.neg(y.div(x.pow(2).add(y.pow(2)))).mul(i_v(0))
			Dim yGrad As val = x.div(x.pow(2).add(y.pow(2))).mul(i_v(0))

			Return New List(Of SDVariable) From {yGrad, xGrad}
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes for %s, got %s", Me.GetType(), dataTypes)
            Preconditions.checkState(dataTypes(0) = dataTypes(1), "Input datatypes must be same type: got %s", dataTypes)
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
