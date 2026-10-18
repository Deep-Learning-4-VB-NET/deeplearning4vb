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

Namespace org.nd4vb.linalg.api.ops.impl.reduce

    ''' <summary>The custom op <c>tensormmul_bp</c> (Java <c>org.nd4j.linalg.api.ops.impl.reduce.TensorMmulBp</c>).</summary>
    Public Class TensorMmulBp
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal samediff As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable, ByVal gradAtOutput As SDVariable, ByVal axes()() As Integer)
			Me.New(samediff, x, y, gradAtOutput, axes(0), axes(1))
		End Sub

		Public Sub New(ByVal samediff As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable, ByVal gradAtOutput As SDVariable, ByVal axesX() As Integer, ByVal axesY() As Integer)
			MyBase.New(Nothing, samediff, New SDVariable(){x, y, gradAtOutput})
			Dim axes()() As Integer = {axesX, axesY}
			addIArgument(axesX.Length)
			addIArgument(axesX)
			addIArgument(axesY.Length)
			addIArgument(axesY)
		End Sub
#End If

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal gradAtOutput As INDArray, ByVal axes()() As Integer)
            Me.New(x, y, gradAtOutput, axes(0), axes(1))
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal gradAtOutput As INDArray, ByVal axesX() As Integer, ByVal axesY() As Integer)
            MyBase.New(Nothing,New INDArray(){x, y, gradAtOutput},Nothing)
            Dim axes()() As Integer = {axesX, axesY}
            addIArgument(axesX.Length)
            addIArgument(axesX)
            addIArgument(axesY.Length)
            addIArgument(axesY)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal gradAtOutput As INDArray, ByVal dldx As INDArray, ByVal dldy As INDArray, ByVal axes()() As Integer)
            Me.New(x, y, gradAtOutput, dldx, dldy, axes(0), axes(1))
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal gradAtOutput As INDArray, ByVal dldx As INDArray, ByVal dldy As INDArray, ByVal axesX() As Integer, ByVal axesY() As Integer)
                MyBase.New(Nothing, New INDArray(){x, y, gradAtOutput}, New INDArray(){dldx, dldy})
                Dim axes()() As Integer = {axesX, axesY}
                addIArgument(axesX.Length)
                addIArgument(axesX)
                addIArgument(axesY.Length)
                addIArgument(axesY)
        End Sub

        Public Overrides Function opName() As String
            Return "tensormmul_bp"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v1 As IList(Of SDVariable)) As IList(Of SDVariable)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New System.NotSupportedException("Differentiation of " & Me.GetType().FullName & " not supported")
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 3, "Expected exactly 3 inputs to tensormmul_bp op, got %s", dataTypes)
            Preconditions.checkState(dataTypes(0).isFPType() AndAlso dataTypes(1).isFPType() AndAlso dataTypes(0).isFPType(), "Inputs to tensormmul_bp op must both be a floating" & "point type: got %s", dataTypes)
            Return dataTypes.ToList().GetRange(0, (2) - (0))
        End Function

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok equals over the fields.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As TensorMmulBp = TryCast(o, TensorMmulBp)
            If other Is Nothing Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>Lombok hashCode over the fields.</summary>
        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            Return result
        End Function

    End Class

End Namespace
