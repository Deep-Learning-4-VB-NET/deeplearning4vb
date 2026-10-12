Imports System
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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

Namespace org.nd4j.linalg.api.iter


	<Serializable>
	Public Class LinearIndexLookup
		Private ordering As Char
		Private indexes()() As Long
		Private shape() As Long
		Private exists() As Boolean
		Private numIndexes As Long

		''' 
		''' <param name="shape"> the shape of the linear index </param>
		''' <param name="ordering"> the ordering of the linear index </param>
		Public Sub New(ByVal shape() As Integer, ByVal ordering As Char)
			Me.New(ArrayUtil.toLongArray(shape), ordering)
		End Sub


		Public Sub New(ByVal shape() As Long, ByVal ordering As Char)
			Me.shape = shape
			Me.ordering = ordering
			numIndexes = ArrayUtil.prodLong(shape)

			' FIMXE: long!
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: indexes = new Long[CInt(numIndexes)][shape.Length]
			indexes = RectangularArrays.RectangularLongArray(CInt(numIndexes), shape.Length)
			exists = New Boolean(CInt(numIndexes) - 1){}
		End Sub

		''' <summary>
		''' Give back a sub
		''' wrt the given linear index </summary>
		''' <param name="index"> the index </param>
		''' <returns> the sub for the given index </returns>
		Public Overridable Function lookup(ByVal index As Integer) As Long()
			If exists(index) Then
				Return indexes(index)
			Else
				exists(index) = True
				indexes(index) = If(ordering = "c"c, Shape.ind2subC(shape, index, numIndexes), Shape.ind2sub(shape, index, numIndexes))
				Return indexes(index)
			End If
		End Function


	End Class

End Namespace