Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.nd4j.linalg.api.ops.impl.reduce.bp



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class DotBp extends BaseReductionBp
	Public Class DotBp
		Inherits BaseReductionBp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal origInput1 As SDVariable, ByVal origInput2 As SDVariable, ByVal gradAtOutput As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, origInput1, origInput2, gradAtOutput, keepDims, dimensions)
		End Sub

		Public Sub New(ByVal origInput1 As INDArray, ByVal origInput2 As INDArray, ByVal gradAtOutput As INDArray, ByVal output As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(origInput1, origInput2, gradAtOutput, output, keepDims, dimensions)
		End Sub

		Public Sub New(ByVal origInput1 As INDArray, ByVal origInput2 As INDArray, ByVal gradAtOutput As INDArray, ByVal outputX As INDArray, ByVal outputY As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(origInput1, origInput2, gradAtOutput, outputX, outputY, keepDims, dimensions)
		End Sub

		Public Overrides Function opName() As String
			Return "reduce_dot_bp"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 3, "Expected exactly 3 input datatype for %s, got input %s", Me.GetType(), dataTypes)
			Preconditions.checkState(dataTypes(0).isFPType(), "First input must be a floating point type, got %s", dataTypes(0))
			Preconditions.checkState(dataTypes(1).isFPType(), "Second input (gradient at reduction output) must be a floating point type, got %s", dataTypes(1))
			Preconditions.checkState(dataTypes(2).isFPType(), "Second input (gradient at reduction output) must be a floating point type, got %s", dataTypes(2))
			Return New List(Of DataType) From {dataTypes(0), dataTypes(0)}
		End Function
	End Class

End Namespace