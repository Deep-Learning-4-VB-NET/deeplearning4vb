Imports System
Imports System.Collections.Generic
Imports Longs = org.nd4j.shade.guava.primitives.Longs
Imports Data = lombok.Data
Imports Generator = net.ericaro.neoitertools.Generator
Imports Itertools = net.ericaro.neoitertools.Itertools
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LongUtils = org.nd4j.linalg.util.LongUtils

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

Namespace org.nd4j.linalg.indexing


	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class SpecifiedIndex implements INDArrayIndex
	Public Class SpecifiedIndex
		Implements INDArrayIndex

		Private indexes() As Long

		Public Sub New(ParamArray ByVal indexes() As Integer)
			Me.indexes = LongUtils.toLongs(indexes)
		End Sub

		Public Sub New(ParamArray ByVal indexes() As Long)
			Me.indexes = indexes
		End Sub

		Public Overridable Function [end]() As Long Implements INDArrayIndex.end
			Return indexes(indexes.Length - 1)
		End Function

		Public Overridable Function offset() As Long Implements INDArrayIndex.offset
			Return indexes(0)
		End Function

		Public Overridable Function length() As Long Implements INDArrayIndex.length
			Return indexes.Length
		End Function

		Public Overridable Function stride() As Long Implements INDArrayIndex.stride
			Return 1
		End Function

		Public Overridable Sub reverse() Implements INDArrayIndex.reverse

		End Sub

		Public Overridable ReadOnly Property Interval As Boolean Implements INDArrayIndex.isInterval
			Get
				Return False
			End Get
		End Property

		Public Overridable Sub init(ByVal arr As INDArray, ByVal begin As Long, ByVal dimension As Integer) Implements INDArrayIndex.init

		End Sub

		Public Overridable Sub init(ByVal arr As INDArray, ByVal dimension As Integer) Implements INDArrayIndex.init

		End Sub

		Public Overridable Sub init(ByVal begin As Long, ByVal [end] As Long, ByVal max As Long) Implements INDArrayIndex.init

		End Sub

		Public Overridable Sub init(ByVal begin As Long, ByVal [end] As Long) Implements INDArrayIndex.init

		End Sub


		''' <summary>
		''' Iterate over a cross product of the
		''' coordinates </summary>
		''' <param name="indexes"> the coordinates to iterate over.
		'''                Each element of the array should be of opType <seealso cref="SpecifiedIndex"/>
		'''                otherwise it will end up throwing an exception </param>
		''' <returns> the generator for iterating over all the combinations of the specified indexes. </returns>
		Public Shared Function iterate(ParamArray ByVal indexes() As INDArrayIndex) As Generator(Of IList(Of IList(Of Long)))
			Dim gen As Generator(Of IList(Of IList(Of Long))) = Itertools.product(New SpecifiedIndexesGenerator(indexes))
			Return gen
		End Function

		''' <summary>
		''' Iterate over a cross product of the
		''' coordinates </summary>
		''' <param name="indexes"> the coordinates to iterate over.
		'''                Each element of the array should be of opType <seealso cref="SpecifiedIndex"/>
		'''                otherwise it will end up throwing an exception </param>
		''' <returns> the generator for iterating over all the combinations of the specified indexes. </returns>
		Public Shared Function iterateOverSparse(ParamArray ByVal indexes() As INDArrayIndex) As Generator(Of IList(Of IList(Of Long)))
			Dim gen As Generator(Of IList(Of IList(Of Long))) = Itertools.product(New SparseSpecifiedIndexesGenerator(indexes))
			Return gen
		End Function


		''' <summary>
		''' A generator for <seealso cref="SpecifiedIndex"/> for
		''' <seealso cref="Itertools.product(Generator)"/>
		'''    to iterate
		''' over an array given a set of  iterators
		''' </summary>
		Public Class SpecifiedIndexesGenerator
			Implements Generator(Of Generator(Of IList(Of Long)))

			Friend index As Integer = 0
			Friend indexes() As INDArrayIndex

			''' <summary>
			''' The indexes to generate from </summary>
			''' <param name="indexes"> the indexes to generate from </param>
			Public Sub New(ByVal indexes() As INDArrayIndex)
				Me.indexes = indexes
				For i As Integer = 0 To indexes.Length - 1
					'Replace point indices with specified indices
					If TypeOf indexes(i) Is PointIndex Then
						indexes(i) = New SpecifiedIndex(indexes(i).offset())
					End If
				Next i
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public net.ericaro.neoitertools.Generator<java.util.List<Long>> next() throws java.util.NoSuchElementException
			Public Overrides Function [next]() As Generator(Of IList(Of Long))
				If index >= indexes.Length Then
					Throw New NoSuchElementException("Done")
				End If

'JAVA TO VB CONVERTER NOTE: The variable specifiedIndex was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: SpecifiedIndex specifiedIndex = (SpecifiedIndex) indexes[index++];
				Dim specifiedIndex_Conflict As SpecifiedIndex = DirectCast(indexes(index), SpecifiedIndex)
					index += 1
				Dim ret As Generator(Of IList(Of Long)) = specifiedIndex_Conflict.generator()
				Return ret
			End Function
		End Class

		''' <summary>
		''' A generator for <seealso cref="SpecifiedIndex"/> for
		''' <seealso cref="Itertools.product(Generator)"/>
		'''    to iterate
		''' over an array given a set of  iterators
		''' </summary>
		Public Class SparseSpecifiedIndexesGenerator
			Implements Generator(Of Generator(Of IList(Of Long)))

			Friend index As Integer = 0
			Friend indexes() As INDArrayIndex

			''' <summary>
			''' The indexes to generate from </summary>
			''' <param name="indexes"> the indexes to generate from </param>
			Public Sub New(ByVal indexes() As INDArrayIndex)
				Me.indexes = indexes
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public net.ericaro.neoitertools.Generator<java.util.List<Long>> next() throws java.util.NoSuchElementException
			Public Overrides Function [next]() As Generator(Of IList(Of Long))
				If index >= indexes.Length Then
					Throw New NoSuchElementException("Done")
				End If

'JAVA TO VB CONVERTER NOTE: The variable specifiedIndex was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: SpecifiedIndex specifiedIndex = (SpecifiedIndex) indexes[index++];
				Dim specifiedIndex_Conflict As SpecifiedIndex = DirectCast(indexes(index), SpecifiedIndex)
					index += 1
				Dim ret As Generator(Of IList(Of Long)) = specifiedIndex_Conflict.sparseGenerator()
				Return ret
			End Function
		End Class


		Public Class SingleGenerator
			Implements Generator(Of IList(Of Long))

			Private ReadOnly outerInstance As SpecifiedIndex

			Public Sub New(ByVal outerInstance As SpecifiedIndex)
				Me.outerInstance = outerInstance
			End Sub

			''' <returns> the next item in the sequence. </returns>
			''' <exception cref="NoSuchElementException"> when sequence is exhausted. </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<Long> next() throws java.util.NoSuchElementException
			Public Overrides Function [next]() As IList(Of Long)
	'            if (!SpecifiedIndex.this.hasNext())
	'                throw new NoSuchElementException();
	'
	'            return Longs.asList(SpecifiedIndex.this.next());
				Throw New Exception()
			End Function
		End Class
		Public Class SparseSingleGenerator
			Implements Generator(Of IList(Of Long))

			Private ReadOnly outerInstance As SpecifiedIndex

			Public Sub New(ByVal outerInstance As SpecifiedIndex)
				Me.outerInstance = outerInstance
			End Sub

			''' <returns> the next item in the sequence. </returns>
			''' <exception cref="NoSuchElementException"> when sequence is exhausted. </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<Long> next() throws java.util.NoSuchElementException
			Public Overrides Function [next]() As IList(Of Long)
	'            if (!SpecifiedIndex.this.hasNext())
	'                throw new NoSuchElementException();
	'            long[] pair = SpecifiedIndex.this.nextSparse();
	'            return Arrays.asList(pair[0], pair[1]);
				Throw New Exception()
			End Function
		End Class

		Public Overridable Function generator() As Generator(Of IList(Of Long))
			Return New SingleGenerator(Me)
		End Function

		Public Overridable Function sparseGenerator() As Generator(Of IList(Of Long))
			Return New SparseSingleGenerator(Me)
		End Function

	End Class

End Namespace