Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
Imports LinAlgExceptions = DeepLearning4VB.org.nd4vb.linalg.util.LinAlgExceptions
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports SameDiffUtils = DeepLearning4VB.org.nd4vb.autodiff.util.SameDiffUtils
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

Namespace org.nd4vb.linalg.api.ops

    ''' <summary>
    ''' The base class of the transform ops: z = f(x), or z = f(x, y) when the op is given a y array (the executioner
    ''' then runs it as a pairwise op). <see cref="getOpType"/> tells the transform kind (same, float, strict, bool or
    ''' any) and <see cref="resultType()"/> the data type of z.
    ''' </summary>
    Public MustInherit Class BaseTransformOp
        Inherits BaseOp
        Implements TransformOp

        Public MustOverride Function validateDataTypes(ByVal opContext As OpContext, ByVal experimentalMode As Boolean) As Boolean Implements TransformOp.validateDataTypes

        ''' <summary>The transform kind of the op (Java <c>getOpType()</c>).</summary>
        Public MustOverride Overloads Function getOpType() As Op.Type Implements TransformOp.getOpType

        Public MustOverride Function resultType(ByVal opContext As OpContext) As DataType Implements TransformOp.resultType

        Public MustOverride Function resultType() As DataType Implements TransformOp.resultType

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable)
			Me.New(sameDiff,i_v1,i_v2,False)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff,inPlace,New Object() {i_v2})
			If i_v1 IsNot Nothing AndAlso i_v2 IsNot Nothing Then
				SameDiffUtils.validateDifferentialFunctionSameDiff(sameDiff, i_v1, Me)
				SameDiffUtils.validateDifferentialFunctionSameDiff(sameDiff, i_v2, Me)
				Me.sameDiff = sameDiff
				Me.inPlace = inPlace
				Me.xVertexId = i_v1.name()
				Me.yVertexId = i_v2.name()
				sameDiff.addArgsFor(New SDVariable(){i_v1, i_v2},Me)
			Else
				Throw New System.ArgumentException("Input not null variables.")
			End If
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff)
			Me.sameDiff = sameDiff
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable, ByVal extraArgs() As Object)
			MyBase.New(sameDiff,extraArgs)
			If i_v1 IsNot Nothing AndAlso i_v2 IsNot Nothing Then
				SameDiffUtils.validateDifferentialFunctionSameDiff(sameDiff, i_v1, Me)
				SameDiffUtils.validateDifferentialFunctionSameDiff(sameDiff, i_v2, Me)
				Me.sameDiff = sameDiff
				Me.xVertexId = i_v1.name()
				Me.yVertexId = i_v2.name()
				sameDiff.addArgsFor(New SDVariable(){i_v1, i_v2},Me)
			Else
				Throw New System.ArgumentException("Input not null variables.")
			End If
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal inPlace As Boolean)
			Me.New(sameDiff,i_v,i_v.Shape,inPlace,Nothing)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal shape() As Long, ByVal inPlace As Boolean, ByVal extraArgs() As Object)
			MyBase.New(sameDiff,inPlace,extraArgs)
			If i_v IsNot Nothing Then
				SameDiffUtils.validateDifferentialFunctionSameDiff(sameDiff, i_v, Me)
				Me.xVertexId = i_v.name()
				sameDiff.addArgsFor(New SDVariable(){i_v},Me)
			Else
				Throw New System.ArgumentException("Input must not null variable.")
			End If
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal extraArgs() As Object)
			Me.New(sameDiff,i_v,i_v.Shape,False,extraArgs)
		End Sub
#End If

        ''' <summary>
        ''' Specify an alternative output array
        ''' </summary>
        ''' <param name="x"> the input </param>
        ''' <param name="z"> the output </param>
        Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
            MyBase.New(x, z)
            LinAlgExceptions.assertSameShape(x, z)
        End Sub

        Public Sub New()
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
            MyBase.New(x, y, z)
            If y IsNot Nothing Then
                LinAlgExceptions.assertSameLength(x, y, z)
            ElseIf z IsNot Nothing Then
                LinAlgExceptions.assertSameLength(x, z)
            End If
        End Sub

        ''' <summary>
        ''' An op for one ndarray
        ''' </summary>
        ''' <param name="x"> the ndarray </param>
        Public Sub New(ByVal x As INDArray)
            MyBase.New(x)
        End Sub

        Public MustOverride Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)

        Public Overrides Function z() As INDArray
            Return zField
        End Function

    End Class

End Namespace
