Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.gradient

    ''' <summary>The custom op <c>tanh_bp</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.gradient.TanhDerivative</c>).</summary>
    Public Class TanhDerivative
        Inherits DynamicCustomOp

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable)
			MyBase.New(sameDiff, New SDVariable(){i_v1, i_v2})
		End Sub
#End If

        ''' 
        ''' <param name="x"> Input </param>
        ''' <param name="y"> Gradient at output (dL/dOut) </param>
        ''' <param name="z"> Output array, gradient at input (dL/dIn - to be calculated) </param>
        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
            MyBase.New(Nothing, New INDArray(){x, y}, New INDArray(){z})
        End Sub

        Public Sub New()
        End Sub

        ''' <param name="x"> Input </param>
        ''' <param name="y"> Gradient at output (dL/dOut) </param>
        Public Sub New(ByVal x As INDArray, ByVal y As INDArray)
            Me.New(x, y, Nothing)
        End Sub

        Public Overrides Function opNum() As Integer
            Return 0
        End Function

        ''' <summary>
        ''' The opName of this operation
        ''' </summary>
        ''' <returns> the opName of this operation </returns>
        Public Overrides Function opName() As String
            Return "tanh_bp"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim ret As SDVariable = sameDiff.math_Conflict.div(sameDiff.onesLike(outputVariables()(0)), sameDiff.math_Conflict.pow(sameDiff.math_Conflict.cosh(arg()), 2))
			Return Collections.singletonList(ret)
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Return New List(Of DataType) From {inputDataTypes(0)}
        End Function

    End Class

End Namespace
