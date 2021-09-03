Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DotBp = org.nd4j.linalg.api.ops.impl.reduce.bp.DotBp

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

Namespace org.nd4j.linalg.api.ops.impl.reduce3


	Public Class Dot
		Inherits BaseReduce3Op

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, i_v, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub

		Public Sub New()
		End Sub

		''' <summary>
		''' Full array dot product reduction, optionally along specified dimensions.<br>
		''' See <a href="https://en.wikipedia.org/wiki/Dot_product">wikipedia</a> for details.
		''' </summary>
		''' <param name="x">          input variable. </param>
		''' <param name="y">          input variable. </param>
		''' <param name="z">          (optional) place holder for the result. Must have the expected shape. </param>
		''' <param name="dimensions"> (optional) Dimensions to reduce over. If dimensions are not specified, full array reduction is performed. </param>
		''' <seealso cref= org.nd4j.linalg.ops.transforms.Transforms#dot Transforms.dot(...) for a wrapper around the common use case of 2 INDArrays. </seealso>
		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer)
			Me.New(x, y, z, True, False, dimensions)
		End Sub


		''' <seealso cref= #Dot(INDArray x, INDArray y, INDArray z, int...) </seealso>
		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ParamArray ByVal dimensions() As Integer)
			Me.New(x, y, Nothing, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal allDistances As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, allDistances, dimensions)
		End Sub

		''' <seealso cref= #Dot(INDArray x, INDArray y, INDArray z, int...) </seealso>
		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
			Me.New(x, y, z, Nothing)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, keepDims, dimensions)
		End Sub

		''' <seealso cref= #Dot(INDArray x, INDArray y, INDArray z, int...) </seealso>
		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal newFormat As Boolean, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, keepDims, False, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, dimensions)
		End Sub

		Public Overrides Function opNum() As Integer
			Return 3
		End Function

		Public Overrides Function opName() As String
			Return "dot"
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			'TODO KEEP DIMS
			Return (New DotBp(sameDiff, arg(0), arg(1), f1(0), False, dimensions)).outputs()
		End Function
	End Class

End Namespace