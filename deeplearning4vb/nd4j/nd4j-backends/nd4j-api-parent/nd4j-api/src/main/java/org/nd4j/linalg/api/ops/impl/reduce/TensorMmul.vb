Imports System.Collections.Generic
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Ints = org.nd4j.shade.guava.primitives.Ints
Imports Longs = org.nd4j.shade.guava.primitives.Longs
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports val = lombok.val
Imports Onnx = onnx.Onnx
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports MMulTranspose = org.nd4j.linalg.api.blas.params.MMulTranspose
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Op = org.nd4j.linalg.api.ops.Op
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
Imports org.nd4j.common.util.ArrayUtil

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

Namespace org.nd4j.linalg.api.ops.impl.reduce


	''' <summary>
	''' TensorMmul
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class TensorMmul extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class TensorMmul
		Inherits DynamicCustomOp

		Private axes()() As Integer
		Protected Friend addedEdges As Boolean
		Protected Friend mMulTranspose As MMulTranspose


		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal axes()() As Integer)
			Me.New(x,y,axes(0), axes(1), False, False, False)
		End Sub

		''' <summary>
		''' Initialize with the given
		''' input, pairwise transform, result, and number
		''' of elements
		''' </summary>
		''' <param name="x"> the input </param>
		''' <param name="y"> the pairwise transform </param>
		''' <param name="z"> the result </param>
		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal axes()() As Integer)
			Me.New(x, y, axes(0), axes(1), False, False, False)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal dimensionsX() As Integer, ByVal dimensionsY() As Integer, ByVal transposeX As Boolean, ByVal transposeY As Boolean, ByVal transposeZ As Boolean)
			MyBase.New(Nothing,New INDArray(){x, y},Nothing)
			Me.axes = New Integer()(){dimensionsX, dimensionsY}
			addIArgument(dimensionsX.Length)
			addIArgument(dimensionsX)
			addIArgument(dimensionsY.Length)
			addIArgument(dimensionsY)
			addBArgument(transposeX, transposeY, transposeZ)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions()() As Integer)
			Me.New(sameDiff,i_v1,i_v2,dimensions,MMulTranspose.allFalse())
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions()() As Integer, ByVal mMulTranspose As MMulTranspose)
			MyBase.New(Nothing, sameDiff, New SDVariable(){i_v1, i_v2})
			Me.sameDiff = sameDiff
			Me.mMulTranspose = mMulTranspose
			Me.axes = dimensions
			If Not addedEdges AndAlso sameDiff.getOutputsForOp(Me) Is Nothing Then
				addedEdges = True
			End If

			addIArgument(dimensions(0).Length)
			addIArgument(dimensions(0))
			addIArgument(dimensions(1).Length)
			addIArgument(dimensions(1))
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable, ByVal dimensionsX() As Integer, ByVal dimensionsY() As Integer, ByVal transposeX As Boolean, ByVal transposeY As Boolean, ByVal transposeZ As Boolean)
			MyBase.New(Nothing, sameDiff, New SDVariable(){x, y})
			Me.sameDiff = sameDiff
			Me.axes = New Integer()(){dimensionsX, dimensionsY}
			addIArgument(dimensionsX.Length)
			addIArgument(dimensionsX(0))
			addIArgument(dimensionsY.Length)
			addIArgument(dimensionsY(0))
			addBArgument(transposeX, transposeY, transposeZ)
		End Sub

		Public Overrides Function doDiff(ByVal gradients As IList(Of SDVariable)) As IList(Of SDVariable)
			Return New List(Of SDVariable) From {(New TensorMmulBp(sameDiff, larg(), rarg(), gradients(0), axes)).outputVariables()}
		End Function

		Public Overrides Function opName() As String
			Return "tensordot"
		End Function


		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			MyBase.initFromTensorFlow(nodeDef, initWith, attributesForNode, graph)
			''' <summary>
			''' name: "MatMul"
			''' op: "MatMul"
			''' input: "input"
			''' input: "Variable/read"
			''' attr {
			''' key: "transpose_b"
			''' value {
			''' b: false
			''' }
			''' }
			''' attr {
			''' key: "transpose_a"
			''' value {
			''' b: false
			''' }
			''' }
			''' attr {
			''' key: "T"
			''' value {
			''' type: DT_FLOAT
			''' }
			''' }
			''' 
			''' </summary>

			Dim isTransposeA As val = attributesForNode("transpose_a").getB()
			Dim isTransposeB As val = attributesForNode("transpose_b").getB()
			Dim mMulTranspose As MMulTranspose = MMulTranspose.builder().transposeA(isTransposeA).transposeB(isTransposeB).build()
			Me.mMulTranspose = mMulTranspose
			Dim args As val = Me.args()
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			Dim isTransposeA As val = If(Not attributesForNode.ContainsKey("transA"), False, attributesForNode("transA").getI() > 0)
			Dim isTransposeB As val = If(Not attributesForNode.ContainsKey("transB"), False, attributesForNode("transB").getI() > 0)
			Dim mMulTranspose As MMulTranspose = MMulTranspose.builder().transposeA(isTransposeA).transposeB(isTransposeB).build()
			Me.mMulTranspose = mMulTranspose
		End Sub

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

			Dim that As TensorMmul = DirectCast(o, TensorMmul)

			If addedEdges <> that.addedEdges Then
				Return False
			End If
			If Not java.util.Arrays.deepEquals(axes, that.axes) Then
				Return False
			End If
			Return If(mMulTranspose IsNot Nothing, mMulTranspose.Equals(that.mMulTranspose), that.mMulTranspose Is Nothing)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = MyBase.GetHashCode()
			result = 31 * result + java.util.Arrays.deepHashCode(axes)
			result = 31 * result + (If(addedEdges, 1, 0))
			result = 31 * result + (If(mMulTranspose IsNot Nothing, mMulTranspose.GetHashCode(), 0))
			Return result
		End Function


		Public Overrides Function opType() As Op.Type
			Return Op.Type.CUSTOM
		End Function

		Public Overrides Function onnxName() As String
			Return "Gemm"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 2, "Expected exactly 2 input data types for %s, got %s", Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace