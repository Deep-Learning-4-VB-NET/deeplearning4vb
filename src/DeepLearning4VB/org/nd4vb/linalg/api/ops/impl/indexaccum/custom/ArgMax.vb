Imports System
Imports System.Collections.Generic
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports BaseDynamicCustomIndexReduction = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.reduce.custom.BaseDynamicCustomIndexReduction
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

Namespace org.nd4vb.linalg.api.ops.impl.indexaccum.custom

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMax</c>.</summary>
    Public Class ArgMax
        Inherits BaseDynamicCustomIndexReduction

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal keepDims As Boolean)
			MyBase.New(sameDiff, args, keepDims)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, args, keepDims, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean)
			MyBase.New(sameDiff, args, keepDims, isComplex)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, args, keepDims, isComplex, dimensions)
		End Sub
#End If

        Public Sub New(ByVal inputs() As INDArray)
            MyBase.New(inputs, Nothing)
        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray)
            MyBase.New(inputs, outputs)
        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal keepDims As Boolean)
            MyBase.New(inputs, outputs, keepDims)
        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
            MyBase.New(inputs, outputs, keepDims, dimensions)
        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
            MyBase.New(inputs, keepDims, dimensions)
        End Sub

        Public Sub New(ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
            MyBase.New(keepDims, isComplex, isEmptyReduce, dimensions)
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal arg As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, arg, keepDims, isComplex, isEmptyReduce, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, args, keepDims, isComplex, isEmptyReduce, dimensions)
		End Sub

		Public Sub New(ByVal opName As String, ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
			MyBase.New(opName, sameDiff, args, keepDims, isComplex, isEmptyReduce, dimensions)
		End Sub
#End If

        Public Sub New(ByVal opName As String, ByVal input As INDArray, ByVal output As INDArray, ByVal tArguments As IList(Of Double), ByVal iArguments() As Integer, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
            MyBase.New(opName, input, output, tArguments, iArguments, keepDims, isComplex, isEmptyReduce, dimensions)
        End Sub

        Public Sub New(ByVal opName As String, ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal tArguments As IList(Of Double), ByVal iArguments() As Integer, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
            MyBase.New(opName, inputs, outputs, tArguments, iArguments, keepDims, isComplex, isEmptyReduce, dimensions)
        End Sub

        Public Sub New(ByVal opName As String, ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal tArguments As IList(Of Double), ByVal iArguments As IList(Of Integer), ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
            MyBase.New(opName, inputs, outputs, tArguments, iArguments, keepDims, isComplex, isEmptyReduce, dimensions)
        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
            MyBase.New(inputs, outputs, keepDims, isComplex, isEmptyReduce, dimensions)
        End Sub

        Public Sub New(ByVal opName As String, ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
            MyBase.New(opName, inputs, outputs, keepDims, isComplex, isEmptyReduce, dimensions)
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal opName As String, ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
			MyBase.New(opName, sameDiff, args, inPlace, keepDims, isComplex, isEmptyReduce, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, args, inPlace, keepDims, isComplex, isEmptyReduce, dimensions)
		End Sub
#End If

        Public Sub New(ByVal opName As String, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
            MyBase.New(opName, keepDims, isComplex, isEmptyReduce, dimensions)
        End Sub

        Public Sub New(ByVal input() As INDArray, ByVal output() As INDArray, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal dimensions() As Integer)
            MyBase.New(input, output, keepDims, isComplex, dimensions)
        End Sub

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sd As SameDiff, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			Me.New(sd,New SDVariable(){[in]},keepDims,dimensions)
		End Sub
#End If

        Public Sub New(ByVal [in] As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
            Me.New(New INDArray(){[in]},keepDims,dimensions)
        End Sub

        Public Sub New(ByVal arr As INDArray)
            Me.New(New INDArray(){arr}, False, CType(Nothing, Integer()))
        End Sub

        Public Overrides Function opName() As String
            Return "argmax"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "ArgMax"
        End Function

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok equals over the fields.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As ArgMax = TryCast(o, ArgMax)
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

        ''' <summary>Lombok toString over the fields.</summary>
        Public Overrides Function ToString() As String
            Return "ArgMax()"
        End Function

    End Class

End Namespace
