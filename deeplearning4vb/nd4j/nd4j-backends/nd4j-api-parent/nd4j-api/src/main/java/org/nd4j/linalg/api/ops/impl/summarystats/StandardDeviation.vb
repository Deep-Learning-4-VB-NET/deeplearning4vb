Imports System.Collections.Generic
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports StandardDeviationBp = org.nd4j.linalg.api.ops.impl.reduce.bp.StandardDeviationBp
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException

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


	Public Class StandardDeviation
		Inherits Variance

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal biasCorrected As Boolean, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, biasCorrected, keepDims, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal biasCorrected As Boolean, ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer)
			MyBase.New(x, biasCorrected, dimension)
			Me.keepDims_Conflict = keepDims
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal biasCorrected As Boolean, ParamArray ByVal dimension() As Integer)
			MyBase.New(x, biasCorrected, dimension)
		End Sub


		Public Sub New()
		End Sub

		Public Sub New(ByVal biasCorrected As Boolean)
			MyBase.New(biasCorrected)
		End Sub

		Public Sub New(ByVal x As INDArray, ParamArray ByVal dimension() As Integer)
			MyBase.New(x, dimension)
		End Sub

		Public Sub New(ByVal x As INDArray)
			MyBase.New(x)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal biasCorrected As Boolean, ParamArray ByVal dimension() As Integer)
			MyBase.New(x, z, biasCorrected, dimension)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal newFormat As Boolean, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			MyBase.New(x, z, newFormat, keepDims, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean, ByVal mean As Double)
			MyBase.New(sameDiff, i_v, dimensions, keepDims, mean)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean, ByVal mean As Double)
			MyBase.New(sameDiff, i_v, i_v2, dimensions, keepDims, mean)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal mean As Double)
			MyBase.New(sameDiff, i_v, mean)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer, ByVal mean As Double)
			MyBase.New(sameDiff, i_v, dimensions, mean)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal mean As Double)
			MyBase.New(sameDiff, i_v, i_v2, dimensions, mean)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean, ByVal mean As Double)
			MyBase.New(sameDiff, i_v, keepDims, mean)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean, ByVal mean As Double)
			MyBase.New(sameDiff, i_v, dimensions, keepDims, mean)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal mean As Double)
			MyBase.New(sameDiff, i_v, i_v2, mean)
		End Sub

		Public Sub New(ByVal mean As Double)
			MyBase.New(mean)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer, ByVal mean As Double)
			MyBase.New(x, y, z, keepDims, dimensions, mean)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal mean As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, mean, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal keepDims As Boolean, ByVal mean As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, keepDims, mean, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal mean As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, mean, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal mean As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, mean, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal mean As Double)
			MyBase.New(sameDiff, mean)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable, ByVal mean As Double)
			MyBase.New(sameDiff, i_v, i_v2, dimensions, mean)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(sameDiff, i_v, dimensions, keepDims, mean, bias)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(sameDiff, i_v, i_v2, dimensions, keepDims, mean, bias)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(sameDiff, i_v, mean, bias)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(sameDiff, i_v, dimensions, mean, bias)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(sameDiff, i_v, i_v2, dimensions, mean, bias)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(sameDiff, i_v, keepDims, mean, bias)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(sameDiff, i_v, dimensions, keepDims, mean, bias)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(sameDiff, i_v, i_v2, mean, bias)
		End Sub

		Public Sub New(ByVal mean As Double, ByVal bias As Double)
			MyBase.New(mean, bias)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(x, y, z, keepDims, dimensions, mean, bias)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal mean As Double, ByVal bias As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, mean, bias, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal keepDims As Boolean, ByVal mean As Double, ByVal bias As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, keepDims, mean, bias, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal mean As Double, ByVal bias As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, mean, bias, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal mean As Double, ByVal bias As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, mean, bias, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(sameDiff, mean, bias)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable, ByVal mean As Double, ByVal bias As Double)
			MyBase.New(sameDiff, i_v, i_v2, dimensions, mean, bias)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(sameDiff, i_v, dimensions, keepDims, mean, bias, biasCorrected)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(sameDiff, i_v, i_v2, dimensions, keepDims, mean, bias, biasCorrected)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(sameDiff, i_v, mean, bias, biasCorrected)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(sameDiff, i_v, dimensions, mean, bias, biasCorrected)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(sameDiff, i_v, i_v2, dimensions, mean, bias, biasCorrected)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(sameDiff, i_v, keepDims, mean, bias, biasCorrected)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(sameDiff, i_v, dimensions, keepDims, mean, bias, biasCorrected)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(sameDiff, i_v, i_v2, mean, bias, biasCorrected)
		End Sub

		Public Sub New(ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(mean, bias, biasCorrected)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(x, y, z, keepDims, dimensions, mean, bias, biasCorrected)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, mean, bias, biasCorrected, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal keepDims As Boolean, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, keepDims, mean, bias, biasCorrected, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, mean, bias, biasCorrected, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, mean, bias, biasCorrected, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(sameDiff, mean, bias, biasCorrected)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable, ByVal mean As Double, ByVal bias As Double, ByVal biasCorrected As Boolean)
			MyBase.New(sameDiff, i_v, i_v2, dimensions, mean, bias, biasCorrected)
		End Sub

		Public Overrides Function opNum() As Integer
			Return 1
		End Function

		Public Overrides Function opName() As String
			Return "std"
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function

		Public Overrides ReadOnly Property OpType As Type
			Get
				Return Type.SUMMARYSTATS
			End Get
		End Property

		Public Overrides Function opType() As Type
			Return Type.SUMMARYSTATS
		End Function


		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			'Here: calculating dL/dIn given dL/dOut (i.e., i_v1) and input/output
			'If out = stdev(in) then:
			'dL/dIn = dL/dOut * dOut/dIn
			'dOut/dIn_i = (in_i-mean)/(stdev * (n-1))
			Return (New StandardDeviationBp(sameDiff, arg(), grad(0), biasCorrected_Conflict, keepDims_Conflict, dimensions)).outputs()
		End Function

		Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
			If args().Length < 1 Then
				Throw New ND4JIllegalStateException("Unable to compute input shape. No arguments found.")
			End If

			Dim argShape() As Long = arg().Shape
			If argShape Is Nothing AndAlso x() Is Nothing Then
				Return Collections.emptyList()
			End If
			Dim inputShape() As Long = (If(argShape Is Nothing OrElse Shape.isPlaceholderShape(argShape), x().shape(), argShape))

			Dim ret As val = New List(Of LongShapeDescriptor)(1)
			Dim reducedShape As val = Shape.getReducedShape(inputShape,dimensions, KeepDims)
			ret.add(LongShapeDescriptor.fromShape(reducedShape, resultType()))
			Return ret
		End Function
	End Class

End Namespace