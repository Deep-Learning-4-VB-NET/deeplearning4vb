Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports BaseIndexAccumulation = DeepLearning4VB.org.nd4vb.linalg.api.ops.BaseIndexAccumulation
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
Imports Condition = DeepLearning4VB.org.nd4vb.linalg.indexing.conditions.Condition
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

Namespace org.nd4vb.linalg.api.ops.impl.indexaccum

    ''' <summary>
    ''' The legacy index reduce op <c>first_index</c> (Java <c>org.nd4j.linalg.api.ops.impl.indexaccum.FirstIndex</c>): the index of the
    ''' first element that matches the condition, -1 when none does. extraArgs are
    ''' {compare, eps, mode} from the condition.
    ''' <para>
    ''' Lombok's <c>@Data</c> members are written out: the accessors of condition, compare, eps and mode, and
    ''' <see cref="Equals"/>, <see cref="GetHashCode"/> and <see cref="ToString"/> over these four fields only
    ''' (Lombok's default), which replace the base class versions.
    ''' </para>
    ''' </summary>
    Public Class FirstIndex
        Inherits BaseIndexAccumulation

        Public Sub New()
        End Sub

        Protected Friend condition As Condition
        Protected Friend compare As Double
        Protected Friend eps As Double
        Protected Friend mode As Integer

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal condition As Condition, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, keepDims, dimensions)
			Me.condition = condition
			Me.compare = condition.getValue()
			Me.mode = condition.condtionNum()
			Me.eps = eps
			Me.extraArgs = New Object() {compare, eps, CDbl(mode)}
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean, ByVal condition As Condition, ParamArray ByVal dimensions() As Integer)
			Me.New(sameDiff, i_v, condition, keepDims, dimensions)
		End Sub
#End If

        Public Sub New(ByVal x As INDArray, ByVal condition As Condition, ParamArray ByVal dimension() As Integer)
            Me.New(x, condition, False, dimension)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal keepDims As Boolean, ByVal condition As Condition, ParamArray ByVal dimension() As Integer)
            Me.New(x,condition,keepDims,dimension)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal condition As Condition, ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer)
            Me.New(x, condition, Nd4vbT.EPS_THRESHOLD, dimension)
            Me.keepDimsField = keepDims
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal condition As Condition, ByVal eps As Double, ParamArray ByVal dimension() As Integer)
            MyBase.New(x, dimension)
            Me.condition = condition
            Me.compare = condition.Value
            Me.mode = condition.condtionNum()
            Me.eps = eps
            Me.extraArgsField = New Object() {compare, eps, CDbl(mode)}
        End Sub

        ' ---------------------------------------------------------------- Lombok @Data

        Public Overridable Function getCondition() As Condition
            Return condition
        End Function

        Public Overridable Sub setCondition(ByVal condition As Condition)
            Me.condition = condition
        End Sub

        Public Overridable Function getCompare() As Double
            Return compare
        End Function

        Public Overridable Sub setCompare(ByVal compare As Double)
            Me.compare = compare
        End Sub

        Public Overridable Function getEps() As Double
            Return eps
        End Function

        Public Overridable Sub setEps(ByVal eps As Double)
            Me.eps = eps
        End Sub

        Public Overridable Function getMode() As Integer
            Return mode
        End Function

        Public Overridable Sub setMode(ByVal mode As Integer)
            Me.mode = mode
        End Sub

        ''' <summary>Lombok <c>@Data</c>: equal when the condition, compare, eps and mode are equal (the base state is not compared).</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As FirstIndex = TryCast(o, FirstIndex)
            If other Is Nothing OrElse Not other.canEqual(Me) Then
                Return False
            End If
            If Not Object.Equals(condition, other.condition) Then
                Return False
            End If
            Return compare.CompareTo(other.compare) = 0 AndAlso eps.CompareTo(other.eps) = 0 AndAlso mode = other.mode
        End Function

        Protected Overrides Function canEqual(ByVal other As Object) As Boolean
            Return TypeOf other Is FirstIndex
        End Function

        Public Overrides Function GetHashCode() As Integer
            Const PRIME As Integer = 59
            Dim result As Integer = 1
            result = result * PRIME + If(condition Is Nothing, 43, condition.GetHashCode())
            Dim compareBits As Long = BitConverter.DoubleToInt64Bits(compare)
            result = result * PRIME + Arrays.WrapToInt32(compareBits >> 32 Xor compareBits)
            Dim epsBits As Long = BitConverter.DoubleToInt64Bits(eps)
            result = result * PRIME + Arrays.WrapToInt32(epsBits >> 32 Xor epsBits)
            result = result * PRIME + mode
            Return result
        End Function

        Public Overrides Function ToString() As String
            Return "FirstIndex(condition=" & If(condition Is Nothing, "null", condition.ToString()) & ", compare=" & Arrays.JavaDoubleToString(compare) &
                ", eps=" & Arrays.JavaDoubleToString(eps) & ", mode=" & mode & ")"
        End Function

        Public Overrides Function opNum() As Integer
            Return 4
        End Function

        Public Overrides Function opName() As String
            Return "first_index"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function
#End If

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
        End Function

    End Class

End Namespace
