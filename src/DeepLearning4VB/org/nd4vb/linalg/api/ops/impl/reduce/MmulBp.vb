Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports MMulTranspose = DeepLearning4VB.org.nd4vb.linalg.api.blas.params.MMulTranspose
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
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

Namespace org.nd4vb.linalg.api.ops.impl.reduce

    ''' <summary>The custom op <c>matmul_bp</c> (Java <c>org.nd4j.linalg.api.ops.impl.reduce.MmulBp</c>).</summary>
    Public Class MmulBp
        Inherits DynamicCustomOp

        Protected Friend mt As MMulTranspose

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable, ByVal eps As SDVariable, ByVal mt As MMulTranspose)
			MyBase.New(Nothing,sameDiff,New SDVariable(){x, y, eps})
			Me.mt = mt
			addIArgument(ArrayUtil.fromBoolean(mt.isTransposeA()), ArrayUtil.fromBoolean(mt.isTransposeB()), ArrayUtil.fromBoolean(mt.isTransposeResult()))
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable, ByVal eps As SDVariable)
			Me.New(sameDiff,x,y, eps, MMulTranspose.allFalse())
		End Sub
#End If

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal eps As INDArray, ByVal dldx As INDArray, ByVal dldy As INDArray, ByVal mt As MMulTranspose)
            MyBase.New(Nothing, New INDArray(){x, y, eps}, New INDArray(){dldx, dldy})
            If mt IsNot Nothing Then
              Me.mt = mt
              addIArgument(ArrayUtil.fromBoolean(mt.isTransposeA()), ArrayUtil.fromBoolean(mt.isTransposeB()), ArrayUtil.fromBoolean(mt.isTransposeResult()))
            End If
        End Sub

        Public Sub New()
        End Sub

        Public Overrides Function opName() As String
            Return "matmul_bp"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v1 As IList(Of SDVariable)) As IList(Of SDVariable)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New System.NotSupportedException("Differentiation of " & Me.GetType().FullName & " not supported")
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 3, "Expected exactly 3 inputs to matmul_bp op, got %s", dataTypes)
            Preconditions.checkState(dataTypes(0).isFPType() AndAlso dataTypes(1).isFPType() AndAlso dataTypes(0).isFPType(), "Inputs to matmul_bp op must both be a floating" & "point type: got %s", dataTypes)
            Return dataTypes.ToList().GetRange(0, (2) - (0))
        End Function

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok equals over the fields.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As MmulBp = TryCast(o, MmulBp)
            If other Is Nothing Then
                Return False
            End If
            If Not Object.Equals(mt, other.mt) Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>Lombok hashCode over the fields.</summary>
        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + If(CObj(mt) Is Nothing, 43, mt.GetHashCode())
            Return result
        End Function

    End Class

End Namespace
