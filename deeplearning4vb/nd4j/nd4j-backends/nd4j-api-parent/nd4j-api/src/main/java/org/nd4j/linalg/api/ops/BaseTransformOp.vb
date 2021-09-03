Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffUtils = org.nd4j.autodiff.util.SameDiffUtils
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports LinAlgExceptions = org.nd4j.linalg.util.LinAlgExceptions

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

Namespace org.nd4j.linalg.api.ops


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseTransformOp extends BaseOp implements TransformOp
	Public MustInherit Class BaseTransformOp
		Inherits BaseOp
		Implements TransformOp

		Public MustOverride Function validateDataTypes(ByVal opContext As OpContext, ByVal experimentalMode As Boolean) As Boolean
		Public MustOverride ReadOnly Property OpType As Type Implements TransformOp.getOpType
		Public MustOverride Function resultType(ByVal opContext As OpContext) As org.nd4j.linalg.api.buffer.DataType Implements TransformOp.resultType
		Public MustOverride Function resultType() As org.nd4j.linalg.api.buffer.DataType Implements TransformOp.resultType
		Public Overrides MustOverride WriteOnly Property ExtraArgs Implements org.nd4j.linalg.api.ops.Op.setExtraArgs As Object()


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

		Public Sub New(ByVal x As INDArray)
			MyBase.New(x)
		End Sub

		Public Overrides MustOverride Function calculateOutputShape() As IList(Of LongShapeDescriptor)


		Public Overrides Function z() As INDArray Implements org.nd4j.linalg.api.ops.Op.z
			Return z_Conflict
		End Function


	End Class

End Namespace