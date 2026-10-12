Imports System.Collections.Generic
Imports org.nd4j.common.primitives
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


	Public Class NdIndexIterator
		Implements IEnumerator(Of Long())

		Private length As Integer = -1
		Private i As Integer = 0
		Private shape() As Long
		Private order As Char = "c"c
		Private cache As Boolean = False
		Private Shared lookupMap As IDictionary(Of Pair(Of Long(), Char), LinearIndexLookup) = New Dictionary(Of Pair(Of Long(), Char), LinearIndexLookup)()
		Private lookup As LinearIndexLookup


		''' <summary>
		'''  Pass in the shape to iterate over.
		'''  Defaults to c ordering </summary>
		''' <param name="shape"> the shape to iterate over </param>
		Public Sub New(ParamArray ByVal shape() As Integer)
			Me.New("c"c, shape)
			Me.cache = False
		End Sub

		Public Sub New(ParamArray ByVal shape() As Long)
			Me.New("c"c, False, shape)
			Me.cache = False
		End Sub

		''' <summary>
		'''  Pass in the shape to iterate over.
		'''  Defaults to c ordering </summary>
		''' <param name="shape"> the shape to iterate over </param>
		Public Sub New(ByVal order As Char, ByVal cache As Boolean, ParamArray ByVal shape() As Long)
			Me.shape = ArrayUtil.copy(shape)
			Me.length = ArrayUtil.prod(shape)
			Me.order = order
			Me.cache = cache
			If Me.cache Then
				Dim lookup As LinearIndexLookup = lookupMap(New Pair(Of )(shape, AscW(order)))
				If lookup Is Nothing Then
					lookup = New LinearIndexLookup(shape, order)
					'warm up the cache
					For i As Integer = 0 To length - 1
						lookup.lookup(i)
					Next i
					lookupMap(New Pair(Of )(shape, AscW(order))) = lookup
					Me.lookup = lookup
				Else
					Me.lookup = lookupMap(New Pair(Of )(shape, AscW(order)))
				End If

			End If
		End Sub

		''' <summary>
		'''  Pass in the shape to iterate over </summary>
		''' <param name="shape"> the shape to iterate over </param>
		Public Sub New(ByVal order As Char, ParamArray ByVal shape() As Integer)
			Me.New(order, False, ArrayUtil.toLongArray(shape))
		End Sub

		Public Sub New(ByVal order As Char, ParamArray ByVal shape() As Long)
			Me.New(order, False, shape)
		End Sub

		Public Overrides Function hasNext() As Boolean
			Return i < length
		End Function



		Public Overrides Function [next]() As Long()
			If lookup IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: return lookup.lookup(i++);
				Dim tempVar = lookup.lookup(i)
					i += 1
					Return tempVar
			End If
			Select Case order
				Case "c"c
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: return org.nd4j.linalg.api.shape.Shape.ind2subC(shape, i++);
					Dim tempVar2 = Shape.ind2subC(shape, i)
						i += 1
						Return tempVar2
				Case "f"c
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: return org.nd4j.linalg.api.shape.Shape.ind2sub(shape, i++);
					Dim tempVar3 = Shape.ind2sub(shape, i)
						i += 1
						Return tempVar3
				Case Else
					Throw New System.ArgumentException("Illegal ordering " & order)
			End Select

		End Function



		Public Overrides Sub remove()

		End Sub

	End Class

End Namespace