Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor

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
Namespace org.nd4j.linalg.api.ops.impl.reduce.custom


	Public MustInherit Class BaseDynamicCustomBoolReduction
		Inherits BaseDynamicCustomReduction

		Public Sub New()
			MyBase.New()
		End Sub

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

		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray)
			MyBase.New(inputs, outputs)
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal keepDims As Boolean)
			MyBase.New(inputs, outputs, keepDims)
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			MyBase.New(inputs, outputs, keepDims, dimensions)
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			MyBase.New(inputs, keepDims, dimensions)
		End Sub

		Public Sub New(ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
			MyBase.New(keepDims, isComplex, isEmptyReduce, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal arg As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, arg, keepDims, isComplex, isEmptyReduce, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, args, keepDims, isComplex, isEmptyReduce, dimensions)
		End Sub

		Public Sub New(ByVal opName As String, ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
			MyBase.New(opName, sameDiff, args, keepDims, isComplex, isEmptyReduce, dimensions)
		End Sub

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

		Public Sub New(ByVal opName As String, ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
			MyBase.New(opName, sameDiff, args, inPlace, keepDims, isComplex, isEmptyReduce, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, args, inPlace, keepDims, isComplex, isEmptyReduce, dimensions)
		End Sub

		Public Sub New(ByVal opName As String, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
			MyBase.New(opName, keepDims, isComplex, isEmptyReduce, dimensions)
		End Sub

		Public Sub New(ByVal input() As INDArray, ByVal output() As INDArray, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal dimensions() As Integer)
			MyBase.New(input, output, keepDims, isComplex, dimensions)
		End Sub

		Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
			Return calculateOutputShape(Nothing)
		End Function



		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			'All reduce bool: always bool output type. 2nd input is axis arg
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso (dataTypes.Count = 1 OrElse dataTypes.Count = 2), "Expected 1 or input datatype for %s, got input %s", Me.GetType(), dataTypes)
			Preconditions.checkState(dataTypes.Count = 1 OrElse dataTypes(1).isIntType(), "When executing reductions" & "with 2 inputs, second input (axis) must be an integer datatype for %s, got %s", Me.GetType(), dataTypes)
			Return Collections.singletonList(DataType.BOOL)
		End Function

	End Class

End Namespace