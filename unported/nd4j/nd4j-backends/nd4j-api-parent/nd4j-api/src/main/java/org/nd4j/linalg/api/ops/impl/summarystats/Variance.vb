Imports System.Collections.Generic
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseReduceOp = org.nd4j.linalg.api.ops.BaseReduceOp
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports VarianceBp = org.nd4j.linalg.api.ops.impl.reduce.bp.VarianceBp
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Shape = org.nd4j.linalg.api.shape.Shape
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

Namespace org.nd4j.linalg.api.ops.impl.summarystats


	Public Class Variance
		Inherits BaseReduceOp

		Protected Friend mean, bias As Double
'JAVA TO VB CONVERTER NOTE: The field biasCorrected was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend biasCorrected_Conflict As Boolean = True

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean, ByVal mean As Double)
			MyBase.New(sameDiff, i_v, dimensions, keepDims)
			Me.mean = mean
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean, ByVal mean As Double)
			MyBase.New(sameDiff, i_v, i_v2, dimensions, keepDims)
			Me.mean = mean
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal mean As Double)
			MyBase.New(sameDiff, i_v)
			Me.mean = mean
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer, ByVal mean As Double)
			MyBase.New(sameDiff, i_v, dimensions)
			Me.mean = mean
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal mean As Double)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
			Me.mean = mean
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean, ByVal mean As Double)
			MyBase.New(sameDiff, i_v, keepDims)
			Me.mean = mean
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean, ByVal mean As Double)
			MyBase.New(sameDiff, i_v, dimensions, keepDims)
			Me.mean = mean
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal mean As Double)
			MyBase.New(sameDiff, i_v, i_v2)
			Me.mean = mean
		End Sub

		Public Sub New(ByVal mean As Double)
			Me.mean = mean
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer, ByVal mean As Double)
			MyBase.New(x, y, z, keepDims, dimensions)
			Me.mean = mean
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal mean As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, dimensions)
			Me.mean = mean
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal keepDims As Boolean, ByVal mean As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, keepDims, dimensions)
			Me.mean = mean
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal mean As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, dimensions)
			Me.mean = mean
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal mean As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, dimensions)
			Me.mean = mean
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal mean As Double)
			MyBase.New(sameDiff)
			Me.mean = mean
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable, ByVal mean As Double)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
			Me.mean = mean
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(sameDiff, i_v, dimensions, keepDims)
			Me.mean = mean
			Me.bias = bias
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(sameDiff, i_v, i_v2, dimensions, keepDims)
			Me.mean = mean
			Me.bias = bias
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(sameDiff, i_v)
			Me.mean = mean
			Me.bias = bias
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(sameDiff, i_v, dimensions)
			Me.mean = mean
			Me.bias = bias
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
			Me.mean = mean
			Me.bias = bias
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(sameDiff, i_v, keepDims)
			Me.mean = mean
			Me.bias = bias
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(sameDiff, i_v, dimensions, keepDims)
			Me.mean = mean
			Me.bias = bias
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(sameDiff, i_v, i_v2)
			Me.mean = mean
			Me.bias = bias
		End Sub

		Public Sub New(ByVal mean As Double, ByVal bias As Double)
			Me.mean = mean
			Me.bias = bias
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(x, y, z, keepDims, dimensions)
			Me.mean = mean
			Me.bias = bias
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal mean As Double, ByVal bias As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, dimensions)
			Me.mean = mean
			Me.bias = bias
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal keepDims As Boolean, ByVal mean As Double, ByVal bias As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, keepDims, dimensions)
			Me.mean = mean
			Me.bias = bias
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal mean As Double, ByVal bias As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, dimensions)
			Me.mean = mean
			Me.bias = bias
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal mean As Double, ByVal bias As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, dimensions)
			Me.mean = mean
			Me.bias = bias
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(sameDiff)
			Me.mean = mean
			Me.bias = bias
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
			Me.mean = mean
			Me.bias = bias
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(sameDiff, i_v, dimensions, keepDims)
			Me.mean = mean
			Me.bias = bias
			Me.biasCorrected_Conflict = biasCorrected
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(sameDiff, i_v, i_v2, dimensions, keepDims)
			Me.mean = mean
			Me.bias = bias
			Me.biasCorrected_Conflict = biasCorrected
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(sameDiff, i_v)
			Me.mean = mean
			Me.bias = bias
			Me.biasCorrected_Conflict = biasCorrected
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(sameDiff, i_v, dimensions)
			Me.mean = mean
			Me.bias = bias
			Me.biasCorrected_Conflict = biasCorrected
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
			Me.mean = mean
			Me.bias = bias
			Me.biasCorrected_Conflict = biasCorrected
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(sameDiff, i_v, keepDims)
			Me.mean = mean
			Me.bias = bias
			Me.biasCorrected_Conflict = biasCorrected
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(sameDiff, i_v, dimensions, keepDims)
			Me.mean = mean
			Me.bias = bias
			Me.biasCorrected_Conflict = biasCorrected
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(sameDiff, i_v, i_v2)
			Me.mean = mean
			Me.bias = bias
			Me.biasCorrected_Conflict = biasCorrected
		End Sub

		Public Sub New(ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			Me.mean = mean
			Me.bias = bias
			Me.biasCorrected_Conflict = biasCorrected
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(x, y, z, keepDims, dimensions)
			Me.mean = mean
			Me.bias = bias
			Me.biasCorrected_Conflict = biasCorrected
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, dimensions)
			Me.mean = mean
			Me.bias = bias
			Me.biasCorrected_Conflict = biasCorrected
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal keepDims As Boolean, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, keepDims, dimensions)
			Me.mean = mean
			Me.bias = bias
			Me.biasCorrected_Conflict = biasCorrected
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, dimensions)
			Me.mean = mean
			Me.bias = bias
			Me.biasCorrected_Conflict = biasCorrected
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, dimensions)
			Me.mean = mean
			Me.bias = bias
			Me.biasCorrected_Conflict = biasCorrected
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(sameDiff)
			Me.mean = mean
			Me.bias = bias
			Me.biasCorrected_Conflict = biasCorrected
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
			Me.mean = mean
			Me.bias = bias
			Me.biasCorrected_Conflict = biasCorrected
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal biasCorrected As Boolean, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, dimensions, keepDims)
			Me.biasCorrected_Conflict = biasCorrected
			defineDimensions(dimensions)
		End Sub

		Public Sub New()
		End Sub

		Public Sub New(ByVal biasCorrected As Boolean)
			Me.biasCorrected_Conflict = biasCorrected
		End Sub

		Public Sub New(ByVal x As INDArray, ParamArray ByVal dimension() As Integer)
			Me.New(x, True, dimension)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal biasCorrected As Boolean, ParamArray ByVal dimensions() As Integer)
			Me.New(x, z, True, False, dimensions)
			Me.biasCorrected_Conflict = biasCorrected
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal biasCorrected As Boolean, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			Me.New(x, Nothing, biasCorrected, keepDims, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal biasCorrected As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x)
			Me.biasCorrected_Conflict = biasCorrected
			defineDimensions(dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal biasCorrected As Boolean, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, Nothing, z, keepDims, dimensions)
			Me.biasCorrected_Conflict = biasCorrected
			defineDimensions(dimensions)
		End Sub

		Public Overrides Function noOp() As INDArray
			Return Nd4j.zerosLike(x())
		End Function

		Public Overrides Function opNum() As Integer
			Return 0
		End Function



		Public Overrides Function opName() As String
			Return "var"
		End Function


		Public Overridable Property BiasCorrected As Boolean
			Get
				Return biasCorrected_Conflict
			End Get
			Set(ByVal biasCorrected As Boolean)
				Me.biasCorrected_Conflict = biasCorrected
			End Set
		End Property



		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			'If out = var(in) then:
			'dL/dIn = dL/dOut * dOut/dIn
			' with dOut/dIn = (in-mean) * 2/(n-1)
			Return (New VarianceBp(sameDiff, arg(), grad(0), biasCorrected_Conflict, keepDims_Conflict, dimensions)).outputs()
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function


		Public Overrides ReadOnly Property OpType As Type
			Get
				Return Type.VARIANCE
			End Get
		End Property

		Public Overrides Function resultType() As DataType
			Return resultType(Nothing)
		End Function

		Public Overrides Function resultType(ByVal oc As OpContext) As DataType
			Dim x As INDArray = If(oc IsNot Nothing, oc.getInputArray(0), Me.x())
			If x IsNot Nothing AndAlso x.R Then
				Return x.dataType()
			End If

			If Me.arg() IsNot Nothing Then
				Return Me.arg().dataType()
			End If

			Return Nd4j.defaultFloatingPointType()
		End Function

		Public Overrides Function validateDataTypes(ByVal oc As OpContext) As Boolean
			Dim x As INDArray = If(oc IsNot Nothing, oc.getInputArray(0), Me.x())
			If x IsNot Nothing AndAlso Not x.R Then
				Return False
			End If

			Dim y As INDArray = If(oc IsNot Nothing, oc.getInputArray(1), Me.y())
			If y IsNot Nothing AndAlso Not y.R Then
				Return False
			End If

			Dim z As INDArray = If(oc IsNot Nothing, oc.getOutputArray(0), Me.z())
			Return Not (z IsNot Nothing AndAlso Not z.R)
		End Function

		Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
			Return calculateOutputShape(Nothing)
		End Function

		Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
			Dim x As INDArray = If(oc IsNot Nothing, oc.getInputArray(0), Me.x())

			If oc Is Nothing AndAlso args().Length < 1 Then
				Throw New ND4JIllegalStateException("Unable to compute input shape. No arguments found.")
			End If

			Dim argShape() As Long = arg().Shape
			If argShape Is Nothing AndAlso x Is Nothing Then
				Return Collections.emptyList()
			End If
			Dim inputShape() As Long = (If(argShape Is Nothing OrElse Shape.isPlaceholderShape(argShape), x.shape(), argShape))

			Dim ret As val = New List(Of LongShapeDescriptor)(1)
			Dim reducedShape As val = Shape.getReducedShape(inputShape,dimensions, KeepDims)
			ret.add(LongShapeDescriptor.fromShape(reducedShape, resultType()))
			Return ret
		End Function

		Public Overrides Function opType() As Type
			Return Type.VARIANCE
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got input %s", Me.GetType(), dataTypes)
			'Variance and stdev reduction: Always FP out, but if FP in is float/double/half then it's float/double/half out
			'If not FP in, then return default FP type out
			If dataTypes(0).isFPType() Then
				Return dataTypes
			End If
			Return Collections.singletonList(Nd4j.defaultFloatingPointType())
		End Function
	End Class

End Namespace