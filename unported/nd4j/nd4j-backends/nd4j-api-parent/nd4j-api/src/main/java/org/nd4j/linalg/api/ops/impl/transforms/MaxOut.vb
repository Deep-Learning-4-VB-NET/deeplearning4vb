Imports System.Collections.Generic
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseTransformOp = org.nd4j.linalg.api.ops.BaseTransformOp
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.linalg.api.ops.impl.transforms


	Public Class MaxOut
		Inherits BaseTransformOp

		Private max As Number = Double.NaN

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal inPlace As Boolean, ByVal max As Number)
			MyBase.New(sameDiff, i_v, inPlace)
			Me.max = max
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal extraArgs() As Object, ByVal max As Number)
			MyBase.New(sameDiff, i_v, extraArgs)
			Me.max = max
		End Sub

		Public Sub New()
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
			MyBase.New(x, z)
		End Sub

		Public Sub New(ByVal x As INDArray)
			MyBase.New(x)
		End Sub

		Public Overrides Function opNum() As Integer
			Throw New System.NotSupportedException()
		End Function


		Public Overrides Function opName() As String
			Return "maxout"
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function


		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("Tensorflow name not found for " & opName())
			'return "Maxout";
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Nothing
		End Function

		Public Overrides Function resultType() As DataType
			Return Nd4j.defaultFloatingPointType()
		End Function

		Public Overrides Function resultType(ByVal oc As OpContext) As DataType
			Return Nd4j.defaultFloatingPointType()
		End Function

		Public Overrides ReadOnly Property OpType As Type
			Get
				Return Type.TRANSFORM_STRICT
			End Get
		End Property

		Public Overrides Function validateDataTypes(ByVal oc As OpContext, ByVal experimentalMode As Boolean) As Boolean
			Dim x As INDArray = If(oc IsNot Nothing, oc.getInputArray(0), Me.x())
			Dim y As INDArray = If(oc IsNot Nothing, oc.getInputArray(1), Me.y())
			Dim z As INDArray = If(oc IsNot Nothing, oc.getOutputArray(0), Me.z())

			If Not x.R Then
				Return False
			End If

			If y IsNot Nothing AndAlso Not Me.y().R Then
				Return False
			End If

			If z IsNot Nothing AndAlso Me.z().dataType() <> Me.x().dataType() Then
				Return False
			End If

			Return True
		End Function

		Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
			Dim ret As val = New List(Of LongShapeDescriptor)(1)
			If arg() Is Nothing Then
				Throw New ND4JIllegalStateException("No arg found for op!")
			End If

			Dim arr As val = sameDiff.getArrForVarName(arg().name())
			If arr Is Nothing Then
				Return Collections.emptyList()
			End If

			ret.add(LongShapeDescriptor.fromShape(arr.shape(), Nd4j.defaultFloatingPointType()))
			Return ret
		End Function
	End Class

End Namespace