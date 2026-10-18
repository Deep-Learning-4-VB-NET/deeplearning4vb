Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
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

    ''' <summary>The custom op <c>flatten</c> (Java <c>org.nd4j.linalg.api.ops.custom.Flatten</c>).</summary>
    Public Class Flatten
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Private order As Long

        Public Sub New(ByVal order As Char, ParamArray ByVal inputs() As INDArray)
            Me.order = AscW(order)

            For Each [in] In inputs
                inputArgumentsField.Add([in])
            Next [in]

            iArguments.Add(Convert.ToInt64(CInt(Me.order)))
        End Sub

        Public Sub New(ByVal output As INDArray, ParamArray ByVal inputs() As INDArray)
            Me.New(output.ordering(), inputs)

            outputArgumentsField.Add(output)
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal order As Char, ParamArray ByVal inputs() As SDVariable)
			MyBase.New(sameDiff, inputs)
			Me.order = AscW(order)
			addIArgument(order)
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "flatten"
        End Function

#If SAMEDIFF Then
		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			Return New List(Of DataType) From {inputDataTypes(0)}
		End Function
#Else
        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Throw New NotSupportedException("calculateOutputDataTypes needs SameDiff (phase 2 milestone M5)")
        End Function
#End If

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>order</c>.</summary>
        Public Overridable Function getOrder() As Long
            Return order
        End Function

        ''' <summary>Lombok setter of <c>order</c>.</summary>
        Public Overridable Sub setOrder(ByVal value As Long)
            Me.order = value
        End Sub

        ''' <summary>Lombok equals over the fields.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As Flatten = TryCast(o, Flatten)
            If other Is Nothing Then
                Return False
            End If
            If Not Object.Equals(order, other.order) Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>Lombok hashCode over the fields.</summary>
        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + If(CObj(order) Is Nothing, 43, order.GetHashCode())
            Return result
        End Function

        ''' <summary>Lombok toString over the fields.</summary>
        Public Overrides Function ToString() As String
            Return "Flatten(order=" & Arrays.FormatElement(order) & ")"
        End Function

    End Class

End Namespace
