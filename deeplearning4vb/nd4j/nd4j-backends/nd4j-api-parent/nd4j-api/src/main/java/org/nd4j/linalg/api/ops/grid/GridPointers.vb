Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseOp = org.nd4j.linalg.api.ops.BaseOp
Imports Op = org.nd4j.linalg.api.ops.Op

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

Namespace org.nd4j.linalg.api.ops.grid

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @AllArgsConstructor public class GridPointers
	Public Class GridPointers
		Private type As Op.Type
		Private opNum As Integer
		Private dtype As DataType

		' indarrays
		Private opX As INDArray
		Private opY As INDArray
		Private opZ As INDArray

		' data buffers
		Private x As Pointer
		Private y As Pointer
		Private z As Pointer


		' strides
		Private xStride As Long = -1
		Private yStride As Long = -1
		Private zStride As Long = -1

		Private xLength As Long = 0
		Private yLength As Long = 0
		Private zLength As Long = 0

		Private xOrder As Char
		Private yOrder As Char
		Private zOrder As Char

		' shapeInfo pointers
		Private xShapeInfo As Pointer
		Private yShapeInfo As Pointer
		Private zShapeInfo As Pointer

		' dimension-related data
		Private dimensions As Pointer
		Private dimensionsLength As Integer = 0

		' TAD shapes
		Private tadShape As Pointer
		Private tadOffsets As Pointer

		' Op extraArgs
		Private extraArgs As Pointer

		Public Sub New(ByVal op As Op, ParamArray ByVal dimensions() As Integer)
			Me.type = BaseOp.getOpType(op)
			Me.dtype = op.x().data().dataType()
			Me.opNum = op.opNum()

			Me.opX = op.x()
			Me.opZ = op.z()

			Me.xLength = op.x().length()
			Me.zLength = op.z().length()

			Me.xOrder = op.x().ordering()
			Me.zOrder = op.z().ordering()

			Me.xStride = op.x().elementWiseStride()
			Me.zStride = op.z().elementWiseStride()
			If op.y() IsNot Nothing Then
				Me.yStride = op.y().elementWiseStride()
				Me.yLength = op.y().length()
				Me.yOrder = op.y().ordering()
				Me.opY = op.y()
			End If

			If dimensions IsNot Nothing Then
				Me.dimensionsLength = dimensions.Length
			End If
		End Sub
	End Class

End Namespace