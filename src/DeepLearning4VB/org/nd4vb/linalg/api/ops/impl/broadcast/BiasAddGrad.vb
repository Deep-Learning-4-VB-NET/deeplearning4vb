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

Namespace org.nd4vb.linalg.api.ops.impl.broadcast

    ''' <summary>The custom op <c>biasadd_bp</c> (Java <c>org.nd4j.linalg.api.ops.impl.broadcast.BiasAddGrad</c>).</summary>
    Public Class BiasAddGrad
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Protected Friend nchw As Boolean = True

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal bias As SDVariable, ByVal gradient As SDVariable, ByVal nchw As Boolean)
			MyBase.New(Nothing, sameDiff, New SDVariable(){input, bias, gradient})
			Me.nchw = nchw
			addBArgument(nchw)
		End Sub
#End If

        Public Sub New(ByVal input As INDArray, ByVal bias As INDArray, ByVal gradient As INDArray, ByVal output As INDArray)
            MyBase.New(New INDArray(){input, bias, gradient}, wrapOrNull(output))
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal bias As INDArray, ByVal gradient As INDArray, ByVal nchw As Boolean)
            addInputArgument(input, bias, gradient)
            Me.nchw = nchw
            addBArgument(nchw)
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal bias As INDArray, ByVal gradient As INDArray)
            Me.New(input, bias, gradient, False)
        End Sub

        Public Overrides Function opNum() As Integer
            Return 0
        End Function

        Public Overrides Function opName() As String
            Return "biasadd_bp"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Differentiation not supported for op " & Me.GetType().Name)
		End Function
#End If

        Public Overrides Function onnxName() As String
            Return "BiasAddGrad"
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 3, "Expected 3 input data types for %s, got %s", Me.GetType(), inputDataTypes)
            Return New List(Of DataType) From {inputDataTypes(0), inputDataTypes(1)}
        End Function

    End Class

End Namespace
