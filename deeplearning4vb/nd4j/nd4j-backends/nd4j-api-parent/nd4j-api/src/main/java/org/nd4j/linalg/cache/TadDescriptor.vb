Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape

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

Namespace org.nd4j.linalg.cache

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Data public class TadDescriptor
	Public Class TadDescriptor
		Private dimensionLength As Integer
		Private dimension() As Integer
		Private shape() As Long

		''' <summary>
		''' Pass in an ndarray to get the databuffer
		''' and the appropriate dimensions </summary>
		''' <param name="array"> the array to pass in
		'''              to get the shape info from </param>
		''' <param name="dimension"> the dimensions for the TAD </param>
		Public Sub New(ByVal array As INDArray, ByVal dimension() As Integer)
			Me.dimensionLength = If(dimension Is Nothing, 0, dimension.Length)
			Me.dimension = dimension

			' TODO: change this to fill shapeInfo
			Me.shape = dataBufferToArray(array.shapeInfoDataBuffer())
		End Sub


		''' <summary>
		''' Obtain the values from the shape buffer
		''' for the array </summary>
		''' <param name="buffer"> the buffer to get the values from </param>
		''' <returns> the int array version of this data buffer </returns>
		Public Shared Function dataBufferToArray(ByVal buffer As DataBuffer) As Long()
			Dim rank As Integer = buffer.getInt(0)
			Dim ret As val = New Long(Shape.shapeInfoLength(rank) - 1){}
			ret(0) = rank
			Dim e As Integer = 1
			Do While e < Shape.shapeInfoLength(rank)
				ret(e) = buffer.getInt(e)
				e += 1
			Loop

			Return ret
		End Function

	End Class

End Namespace