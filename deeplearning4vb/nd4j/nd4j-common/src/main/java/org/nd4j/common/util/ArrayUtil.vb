Imports System
Imports System.Collections.Generic
Imports Ints = org.nd4j.shade.guava.primitives.Ints
Imports Longs = org.nd4j.shade.guava.primitives.Longs
Imports val = lombok.val
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports Preconditions = org.nd4j.common.base.Preconditions

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

Namespace org.nd4j.common.util


	''' <summary>
	''' @author Adam Gibson
	''' </summary>
	Public Class ArrayUtil


		Private Sub New()
		End Sub


		''' <summary>
		''' Returns true if any array elements are negative.
		''' If the array is null, it returns false </summary>
		''' <param name="arr"> the array to test
		''' @return </param>
		Public Shared Function containsAnyNegative(ByVal arr() As Integer) As Boolean
			If arr Is Nothing Then
				Return False
			End If

			For i As Integer = 0 To arr.Length - 1
				If arr(i) < 0 Then
					Return True
				End If
			Next i
			Return False
		End Function

		Public Shared Function containsAnyNegative(ByVal arr() As Long) As Boolean
			If arr Is Nothing Then
				Return False
			End If

			For i As Integer = 0 To arr.Length - 1
				If arr(i) < 0 Then
					Return True
				End If
			Next i
			Return False
		End Function

		Public Shared Function contains(ByVal arr() As Integer, ByVal value As Integer) As Boolean
			If arr Is Nothing Then
				Return False
			End If
			For Each i As Integer In arr
				If i = value Then
					Return True
				End If
			Next i
			Return False
		End Function

		Public Shared Function contains(ByVal arr() As Long, ByVal value As Integer) As Boolean
			If arr Is Nothing Then
				Return False
			End If
			For Each i As Long In arr
				If i = value Then
					Return True
				End If
			Next i
			Return False
		End Function

		''' 
		''' <param name="arrs"> </param>
		''' <param name="check">
		''' @return </param>
		Public Shared Function anyLargerThan(ByVal arrs() As Integer, ByVal check As Integer) As Boolean
			For i As Integer = 0 To arrs.Length - 1
				If arrs(i) > check Then
					Return True
				End If
			Next i

			Return False
		End Function


		''' 
		''' <param name="arrs"> </param>
		''' <param name="check">
		''' @return </param>
		Public Shared Function anyLessThan(ByVal arrs() As Integer, ByVal check As Integer) As Boolean
			For i As Integer = 0 To arrs.Length - 1
				If arrs(i) < check Then
					Return True
				End If
			Next i

			Return False
		End Function


		''' <summary>
		''' Convert a int array to a string array </summary>
		''' <param name="arr"> the array to convert </param>
		''' <returns> the equivalent string array </returns>
		Public Shared Function convertToString(ByVal arr() As Integer) As String()
			Preconditions.checkNotNull(arr)
			Dim ret(arr.Length - 1) As String
			For i As Integer = 0 To arr.Length - 1
				ret(i) = arr(i).ToString()
			Next i

			Return ret
		End Function


		''' <summary>
		''' Proper comparison contains for list of int
		''' arrays </summary>
		''' <param name="list"> the to search </param>
		''' <param name="target"> the target int array </param>
		''' <returns> whether the given target
		''' array is contained in the list </returns>
		Public Shared Function listOfIntsContains(ByVal list As IList(Of Integer()), ByVal target() As Integer) As Boolean
			For Each arr As Integer() In list
				If target.SequenceEqual(arr) Then
					Return True
				End If
			Next arr
			Return False
		End Function

		''' <summary>
		''' Repeat a value n times </summary>
		''' <param name="n"> the number of times to repeat </param>
		''' <param name="toReplicate"> the value to repeat </param>
		''' <returns> an array of length n filled with the
		''' given value </returns>
		Public Shared Function nTimes(ByVal n As Integer, ByVal toReplicate As Integer) As Integer()
			Dim ret(n - 1) As Integer
			Arrays.Fill(ret, toReplicate)
			Return ret
		End Function

		Public Shared Function nTimes(ByVal n As Long, ByVal toReplicate As Long) As Long()
			If n > Integer.MaxValue Then
				Throw New Exception("Index overflow in nTimes")
			End If
			Dim ret As val = New Long(CInt(n) - 1){}
			Arrays.Fill(ret, toReplicate)
			Return ret
		End Function

		Public Shared Function nTimes(Of T)(ByVal n As Integer, ByVal toReplicate As T, ByVal tClass As Type(Of T)) As T()
			Preconditions.checkState(n>=0, "Invalid number of times to replicate: must be >= 0, got %s", n)
			Dim [out]() As T = CType(Array.CreateInstance(tClass, n), T())
			For i As Integer = 0 To n - 1
				[out](i) = toReplicate
			Next i
			Return [out]
		End Function

		''' <summary>
		''' Returns true if all of the elements in the
		''' given int array are unique </summary>
		''' <param name="toTest"> the array to test </param>
		''' <returns> true if all o fthe items
		''' are unique false otherwise </returns>
		Public Shared Function allUnique(ByVal toTest() As Integer) As Boolean
			Dim set As ISet(Of Integer) = New HashSet(Of Integer)()
			For Each i As Integer In toTest
				If Not set.Contains(i) Then
					set.Add(i)
				Else
					Return False
				End If
			Next i

			Return True
		End Function

		''' <summary>
		''' Credit to mikio braun from jblas
		''' <p/>
		''' Create a random permutation of the numbers 0, ..., size - 1.
		''' <p/>
		''' see Algorithm P, D.E. Knuth: The Art of Computer Programming, Vol. 2, p. 145
		''' </summary>
		Public Shared Function randomPermutation(ByVal size As Integer) As Integer()
			Dim r As New Random()
			Dim result(size - 1) As Integer

			For j As Integer = 0 To size - 1
				result(j) = j + 1
			Next j

			For j As Integer = size - 1 To 1 Step -1
				Dim k As Integer = r.Next(j)
				Dim temp As Integer = result(j)
				result(j) = result(k)
				result(k) = temp
			Next j

			Return result
		End Function


		Public Shared Function toBFloat16(ByVal data As Single) As Short
			Return CShort(Float.floatToIntBits(data) << 16)
		End Function

		Public Shared Function toBFloat16(ByVal data As Double) As Short
			Return toBFloat16(CSng(data))
		End Function

		Public Shared Function toHalf(ByVal data As Single) As Short
			Return fromFloat(data)
		End Function

		Public Shared Function toHalf(ByVal data As Double) As Short
			Return fromFloat(CSng(data))
		End Function

		Public Shared Function toHalfs(ByVal data() As Single) As Short()
			Dim ret(data.Length - 1) As Short
			For i As Integer = 0 To ret.Length - 1
				ret(i) = fromFloat(data(i))
			Next i
			Return ret
		End Function

		Public Shared Function toHalfs(ByVal data() As Integer) As Short()
			Dim ret(data.Length - 1) As Short
			For i As Integer = 0 To ret.Length - 1
				ret(i) = fromFloat(CSng(data(i)))
			Next i
			Return ret
		End Function

		Public Shared Function toHalfs(ByVal data() As Long) As Short()
			Dim ret(data.Length - 1) As Short
			For i As Integer = 0 To ret.Length - 1
				ret(i) = fromFloat(CSng(data(i)))
			Next i
			Return ret
		End Function

		Public Shared Function toBfloats(ByVal data() As Single) As Short()
			Dim ret(data.Length - 1) As Short
			For i As Integer = 0 To ret.Length - 1
				ret(i) = toBFloat16(data(i))
			Next i
			Return ret
		End Function

		Public Shared Function toBfloats(ByVal data() As Integer) As Short()
			Dim ret(data.Length - 1) As Short
			For i As Integer = 0 To ret.Length - 1
				ret(i) = toBFloat16(CSng(data(i)))
			Next i
			Return ret
		End Function

		Public Shared Function toBfloats(ByVal data() As Long) As Short()
			Dim ret(data.Length - 1) As Short
			For i As Integer = 0 To ret.Length - 1
				ret(i) = toBFloat16(CSng(data(i)))
			Next i
			Return ret
		End Function

		Public Shared Function toLongs(ByVal data() As SByte) As Long()
			Dim ret As val = New Long(data.Length - 1){}
			For i As Integer = 0 To ret.length - 1
				ret(i) = CLng(data(i))
			Next i
			Return ret
		End Function

		Public Shared Function toLongs(ByVal data() As Boolean) As Long()
			Dim ret As val = New Long(data.Length - 1){}
			For i As Integer = 0 To ret.length - 1
				ret(i) = If(data(i), 1, 0)
			Next i
			Return ret
		End Function

		Public Shared Function toLongs(ByVal data() As Short) As Long()
			Dim ret As val = New Long(data.Length - 1){}
			For i As Integer = 0 To ret.length - 1
				ret(i) = CLng(data(i))
			Next i
			Return ret
		End Function

		Public Shared Function toLongs(ByVal data() As Integer) As Long()
			Dim ret As val = New Long(data.Length - 1){}
			For i As Integer = 0 To ret.length - 1
				ret(i) = CLng(data(i))
			Next i
			Return ret
		End Function

		Public Shared Function toLongs(ByVal data() As Single) As Long()
			Dim ret As val = New Long(data.Length - 1){}
			For i As Integer = 0 To ret.length - 1
				ret(i) = CLng(Math.Truncate(data(i)))
			Next i
			Return ret
		End Function

		Public Shared Function toLongs(ByVal data() As Double) As Long()
			Dim ret As val = New Long(data.Length - 1){}
			For i As Integer = 0 To ret.length - 1
				ret(i) = CLng(Math.Truncate(data(i)))
			Next i
			Return ret
		End Function

		Public Shared Function toHalfs(ByVal data() As Double) As Short()
			Dim ret(data.Length - 1) As Short
			For i As Integer = 0 To ret.Length - 1
				ret(i) = fromFloat(CSng(data(i)))
			Next i
			Return ret
		End Function

		Public Shared Function fromFloat(ByVal v As Single) As Short
			If Single.IsNaN(v) Then
				Return CShort(&H7fff)
			End If
			If v = Single.PositiveInfinity Then
				Return CShort(&H7c00)
			End If
			If v = Single.NegativeInfinity Then
				Return CShort(&Hfc00)
			End If
			If v = 0.0f Then
				Return CShort(&H0)
			End If
			If v = -0.0f Then
				Return CShort(&H8000)
			End If
			If v > 65504.0f Then
				Return &H7bff ' max value supported by half float
			End If
			If v < -65504.0f Then
				Return CShort(&H7bff Or &H8000)
			End If
			If v > 0.0f AndAlso v < 5.96046E-8f Then
				Return &H1
			End If
			If v < 0.0f AndAlso v > -5.96046E-8f Then
				Return CShort(&H8001)
			End If

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int f = Float.floatToIntBits(v);
			Dim f As Integer = Float.floatToIntBits(v)

			Return CShort(((f >> 16) And &H8000) Or ((((f And &H7f800000) - &H38000000) >> 13) And &H7c00) Or ((f >> 13) And &H3ff))
		End Function

		Public Shared Function toInts(ByVal data() As Single) As Integer()
			Dim ret(data.Length - 1) As Integer
			For i As Integer = 0 To ret.Length - 1
				ret(i) = CInt(Math.Truncate(data(i)))
			Next i
			Return ret
		End Function

		Public Shared Function toInts(ByVal data() As Double) As Integer()
			Dim ret(data.Length - 1) As Integer
			For i As Integer = 0 To ret.Length - 1
				ret(i) = CInt(Math.Truncate(data(i)))
			Next i
			Return ret
		End Function

		Public Shared Function toBytes(ByVal array() As Integer) As SByte()
			Dim retVal As val = New SByte(array.Length - 1){}
			For i As Integer = 0 To array.Length - 1
				retVal(i) = CSByte(array(i))
			Next i
			Return retVal
		End Function

		Public Shared Function toBytes(ByVal array() As Single) As SByte()
			Dim retVal As val = New SByte(array.Length - 1){}
			For i As Integer = 0 To array.Length - 1
				retVal(i) = CSByte(Math.Truncate(array(i)))
			Next i
			Return retVal
		End Function

		Public Shared Function toBytes(ByVal array() As Double) As SByte()
			Dim retVal As val = New SByte(array.Length - 1){}
			For i As Integer = 0 To array.Length - 1
				retVal(i) = CSByte(Math.Truncate(array(i)))
			Next i
			Return retVal
		End Function

		Public Shared Function toBytes(ByVal array() As Long) As SByte()
			Dim retVal As val = New SByte(array.Length - 1){}
			For i As Integer = 0 To array.Length - 1
				retVal(i) = CSByte(array(i))
			Next i
			Return retVal
		End Function

		Public Shared Function toInts(ByVal array() As Long) As Integer()
			Dim retVal(array.Length - 1) As Integer

			For i As Integer = 0 To array.Length - 1
				retVal(i) = CInt(array(i))
			Next i

			Return retVal
		End Function


'JAVA TO VB CONVERTER NOTE: The parameter mod was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Shared Function [mod](ByVal input() As Integer, ByVal mod_Conflict As Integer) As Integer()
			Dim ret(input.Length - 1) As Integer
			For i As Integer = 0 To ret.Length - 1
				ret(i) = input(i) Mod mod_Conflict
			Next i

			Return ret
		End Function


		''' <summary>
		''' Calculate the offset for a given stride array </summary>
		''' <param name="stride"> the stride to use </param>
		''' <param name="i"> the offset to calculate for </param>
		''' <returns> the offset for the given
		''' stride </returns>
		Public Shared Function offsetFor(ByVal stride() As Integer, ByVal i As Integer) As Integer
			Dim ret As Integer = 0
			For j As Integer = 0 To stride.Length - 1
				ret += (i * stride(j))
			Next j
			Return ret

		End Function

		''' <summary>
		''' Sum of an int array </summary>
		''' <param name="add"> the elements
		'''            to calculate the sum for </param>
		''' <returns> the sum of this array </returns>
		Public Shared Function sum(ByVal add As IList(Of Integer)) As Integer
			If add.Count = 0 Then
				Return 0
			End If
			Dim ret As Integer = 0
			For i As Integer = 0 To add.Count - 1
				ret += add(i)
			Next i
			Return ret
		End Function

		''' <summary>
		''' Sum of an int array </summary>
		''' <param name="add"> the elements
		'''            to calculate the sum for </param>
		''' <returns> the sum of this array </returns>
		Public Shared Function sum(ByVal add() As Integer) As Integer
			If add.Length < 1 Then
				Return 0
			End If
			Dim ret As Integer = 0
			For i As Integer = 0 To add.Length - 1
				ret += add(i)
			Next i
			Return ret
		End Function

		Public Shared Function sumLong(ParamArray ByVal add() As Long) As Long
			If add.Length < 1 Then
				Return 0
			End If
			Dim ret As Integer = 0
			For i As Integer = 0 To add.Length - 1
				ret += add(i)
			Next i
			Return ret
		End Function

		''' <summary>
		''' Product of an int array </summary>
		''' <param name="mult"> the elements
		'''            to calculate the sum for </param>
		''' <returns> the product of this array </returns>
		Public Shared Function prod(ByVal mult As IList(Of Integer)) As Integer
			If mult.Count = 0 Then
				Return 0
			End If
			Dim ret As Integer = 1
			For i As Integer = 0 To mult.Count - 1
				ret *= mult(i)
			Next i
			Return ret
		End Function



		''' <summary>
		''' Product of an int array </summary>
		''' <param name="mult"> the elements
		'''            to calculate the sum for </param>
		''' <returns> the product of this array </returns>
		Public Shared Function prod(ParamArray ByVal mult() As Long) As Integer
			If mult.Length < 1 Then
				Return 0
			End If
			Dim ret As Integer = 1
			For i As Integer = 0 To mult.Length - 1
				ret *= mult(i)
			Next i
			Return ret
		End Function


		''' <summary>
		''' Product of an int array </summary>
		''' <param name="mult"> the elements
		'''            to calculate the sum for </param>
		''' <returns> the product of this array </returns>
		Public Shared Function prod(ParamArray ByVal mult() As Integer) As Integer
			If mult.Length < 1 Then
				Return 0
			End If
			Dim ret As Integer = 1
			For i As Integer = 0 To mult.Length - 1
				ret *= mult(i)
			Next i
			Return ret
		End Function

		''' <summary>
		''' Product of an int array </summary>
		''' <param name="mult"> the elements
		'''            to calculate the sum for </param>
		''' <returns> the product of this array </returns>
		Public Shared Function prodLong(Of T1 As Number)(ByVal mult As IList(Of T1)) As Long
			If mult.Count = 0 Then
				Return 0
			End If
			Dim ret As Long = 1
			For i As Integer = 0 To mult.Count - 1
				ret *= mult(i).longValue()
			Next i
			Return ret
		End Function


		''' <summary>
		''' Product of an int array </summary>
		''' <param name="mult"> the elements
		'''            to calculate the sum for </param>
		''' <returns> the product of this array </returns>
		Public Shared Function prodLong(ParamArray ByVal mult() As Integer) As Long
			If mult.Length < 1 Then
				Return 0
			End If
			Dim ret As Long = 1
			For i As Integer = 0 To mult.Length - 1
				ret *= mult(i)
			Next i
			Return ret
		End Function

		Public Shared Function prodLong(ParamArray ByVal mult() As Long) As Long
			If mult.Length < 1 Then
				Return 0
			End If
			Dim ret As Long = 1
			For i As Integer = 0 To mult.Length - 1
				ret *= mult(i)
			Next i
			Return ret
		End Function

		Public Shared Function equals(ByVal data() As Single, ByVal data2() As Double) As Boolean
			If data.Length <> data2.Length Then
				Return False
			End If
			For i As Integer = 0 To data.Length - 1
				Dim equals As Double = Math.Abs(data2(i) - data(i))
				If equals > 1e-6 Then
					Return False
				End If
			Next i
			Return True
		End Function


		Public Shared Function consArray(ByVal a As Integer, ByVal [as]() As Integer) As Integer()
			Dim len As Integer = [as].Length
			Dim nas(len) As Integer
			nas(0) = a
			Array.Copy([as], 0, nas, 1, len)
			Return nas
		End Function


		''' <summary>
		''' Returns true if any of the elements are zero </summary>
		''' <param name="as">
		''' @return </param>
		Public Shared Function isZero(ByVal [as]() As Integer) As Boolean
			For i As Integer = 0 To [as].Length - 1
				If [as](i) = 0 Then
					Return True
				End If
			Next i
			Return False
		End Function

		Public Shared Function isZero(ByVal [as]() As Long) As Boolean
			For i As Integer = 0 To [as].Length - 1
				If [as](i) = 0L Then
					Return True
				End If
			Next i
			Return False
		End Function

		Public Shared Function anyMore(ByVal target() As Integer, ByVal test() As Integer) As Boolean
			Preconditions.checkArgument(target.Length = test.Length, "Unable to compare: different sizes: length %s vs. %s", target.Length, test.Length)
			For i As Integer = 0 To target.Length - 1
				If target(i) > test(i) Then
					Return True
				End If
			Next i
			Return False
		End Function


		Public Shared Function anyLess(ByVal target() As Integer, ByVal test() As Integer) As Boolean
			Preconditions.checkArgument(target.Length = test.Length, "Unable to compare: different sizes: length %s vs. %s", target.Length, test.Length)
			For i As Integer = 0 To target.Length - 1
				If target(i) < test(i) Then
					Return True
				End If
			Next i
			Return False
		End Function

		Public Shared Function lessThan(ByVal target() As Integer, ByVal test() As Integer) As Boolean
			Preconditions.checkArgument(target.Length = test.Length, "Unable to compare: different sizes: length %s vs. %s", target.Length, test.Length)
			For i As Integer = 0 To target.Length - 1
				If target(i) < test(i) Then
					Return True
				End If
				If target(i) > test(i) Then
					Return False
				End If
			Next i
			Return False
		End Function

		Public Shared Function greaterThan(ByVal target() As Integer, ByVal test() As Integer) As Boolean
			Preconditions.checkArgument(target.Length = test.Length, "Unable to compare: different sizes: length %s vs. %s", target.Length, test.Length)
			For i As Integer = 0 To target.Length - 1
				If target(i) > test(i) Then
					Return True
				End If
				If target(i) < test(i) Then
					Return False
				End If
			Next i
			Return False
		End Function


		''' <summary>
		''' Compute the offset
		''' based on teh shape strides and offsets </summary>
		''' <param name="shape"> the shape to compute </param>
		''' <param name="offsets"> the offsets to compute </param>
		''' <param name="strides"> the strides to compute </param>
		''' <returns> the offset for the given shape,offset,and strides </returns>
		Public Shared Function calcOffset(ByVal shape As IList(Of Integer), ByVal offsets As IList(Of Integer), ByVal strides As IList(Of Integer)) As Integer
			If shape.Count <> offsets.Count OrElse shape.Count <> strides.Count Then
				Throw New System.ArgumentException("Shapes,strides, and offsets must be the same size")
			End If
			Dim ret As Integer = 0
			For i As Integer = 0 To offsets.Count - 1
				'we should only do this in the general case, not on vectors
				'the reason for this is we force everything including scalars
				'to be 2d
				If shape(i) = 1 AndAlso offsets.Count > 2 AndAlso i > 0 Then
					Continue For
				End If
				ret += offsets(i) * strides(i)
			Next i

			Return ret
		End Function


		''' <summary>
		''' Compute the offset
		''' based on teh shape strides and offsets </summary>
		''' <param name="shape"> the shape to compute </param>
		''' <param name="offsets"> the offsets to compute </param>
		''' <param name="strides"> the strides to compute </param>
		''' <returns> the offset for the given shape,offset,and strides </returns>
		Public Shared Function calcOffset(ByVal shape() As Integer, ByVal offsets() As Integer, ByVal strides() As Integer) As Integer
			If shape.Length <> offsets.Length OrElse shape.Length <> strides.Length Then
				Throw New System.ArgumentException("Shapes,strides, and offsets must be the same size")
			End If

			Dim ret As Integer = 0
			For i As Integer = 0 To offsets.Length - 1
				If shape(i) = 1 Then
					Continue For
				End If
				ret += offsets(i) * strides(i)
			Next i

			Return ret
		End Function

		''' <summary>
		''' Compute the offset
		''' based on teh shape strides and offsets </summary>
		''' <param name="shape"> the shape to compute </param>
		''' <param name="offsets"> the offsets to compute </param>
		''' <param name="strides"> the strides to compute </param>
		''' <returns> the offset for the given shape,offset,and strides </returns>
		Public Shared Function calcOffset(ByVal shape() As Long, ByVal offsets() As Long, ByVal strides() As Long) As Long
			If shape.Length <> offsets.Length OrElse shape.Length <> strides.Length Then
				Throw New System.ArgumentException("Shapes,strides, and offsets must be the same size")
			End If

			Dim ret As Long = 0
			For i As Integer = 0 To offsets.Length - 1
				If shape(i) = 1 Then
					Continue For
				End If
				ret += offsets(i) * strides(i)
			Next i

			Return ret
		End Function

		''' <summary>
		''' Compute the offset
		''' based on teh shape strides and offsets </summary>
		''' <param name="shape"> the shape to compute </param>
		''' <param name="offsets"> the offsets to compute </param>
		''' <param name="strides"> the strides to compute </param>
		''' <returns> the offset for the given shape,offset,and strides </returns>
		Public Shared Function calcOffsetLong(ByVal shape As IList(Of Integer), ByVal offsets As IList(Of Integer), ByVal strides As IList(Of Integer)) As Long
			If shape.Count <> offsets.Count OrElse shape.Count <> strides.Count Then
				Throw New System.ArgumentException("Shapes,strides, and offsets must be the same size")
			End If
			Dim ret As Long = 0
			For i As Integer = 0 To offsets.Count - 1
				'we should only do this in the general case, not on vectors
				'the reason for this is we force everything including scalars
				'to be 2d
				If shape(i) = 1 AndAlso offsets.Count > 2 AndAlso i > 0 Then
					Continue For
				End If
				ret += CLng(offsets(i)) * strides(i)
			Next i

			Return ret
		End Function


		Public Shared Function calcOffsetLong2(ByVal shape As IList(Of Long), ByVal offsets As IList(Of Long), ByVal strides As IList(Of Long)) As Long
			If shape.Count <> offsets.Count OrElse shape.Count <> strides.Count Then
				Throw New System.ArgumentException("Shapes,strides, and offsets must be the same size")
			End If
			Dim ret As Long = 0
			For i As Integer = 0 To offsets.Count - 1
				'we should only do this in the general case, not on vectors
				'the reason for this is we force everything including scalars
				'to be 2d
				If shape(i) = 1 AndAlso offsets.Count > 2 AndAlso i > 0 Then
					Continue For
				End If
				ret += CLng(offsets(i)) * strides(i)
			Next i

			Return ret
		End Function


		''' <summary>
		''' Compute the offset
		''' based on teh shape strides and offsets </summary>
		''' <param name="shape"> the shape to compute </param>
		''' <param name="offsets"> the offsets to compute </param>
		''' <param name="strides"> the strides to compute </param>
		''' <returns> the offset for the given shape,offset,and strides </returns>
		Public Shared Function calcOffsetLong(ByVal shape() As Integer, ByVal offsets() As Integer, ByVal strides() As Integer) As Long
			If shape.Length <> offsets.Length OrElse shape.Length <> strides.Length Then
				Throw New System.ArgumentException("Shapes,strides, and offsets must be the same size")
			End If

			Dim ret As Long = 0
			For i As Integer = 0 To offsets.Length - 1
				If shape(i) = 1 Then
					Continue For
				End If
				ret += CLng(offsets(i)) * strides(i)
			Next i

			Return ret
		End Function

		''' 
		''' <param name="xs"> </param>
		''' <param name="ys">
		''' @return </param>
		Public Shared Function dotProduct(ByVal xs As IList(Of Integer), ByVal ys As IList(Of Integer)) As Integer
			Dim result As Integer = 0
			Dim n As Integer = xs.Count

			If ys.Count <> n Then
				Throw New System.ArgumentException("Different array sizes")
			End If

			For i As Integer = 0 To n - 1
				result += xs(i) * ys(i)
			Next i
			Return result
		End Function

		''' 
		''' <param name="xs"> </param>
		''' <param name="ys">
		''' @return </param>
		Public Shared Function dotProduct(ByVal xs() As Integer, ByVal ys() As Integer) As Integer
			Dim result As Integer = 0
			Dim n As Integer = xs.Length

			If ys.Length <> n Then
				Throw New System.ArgumentException("Different array sizes")
			End If

			For i As Integer = 0 To n - 1
				result += xs(i) * ys(i)
			Next i
			Return result
		End Function

		''' 
		''' <param name="xs"> </param>
		''' <param name="ys">
		''' @return </param>
		Public Shared Function dotProductLong(ByVal xs As IList(Of Integer), ByVal ys As IList(Of Integer)) As Long
			Dim result As Long = 0
			Dim n As Integer = xs.Count

			If ys.Count <> n Then
				Throw New System.ArgumentException("Different array sizes")
			End If

			For i As Integer = 0 To n - 1
				result += CLng(xs(i)) * ys(i)
			Next i
			Return result
		End Function

		''' 
		''' <param name="xs"> </param>
		''' <param name="ys">
		''' @return </param>
		Public Shared Function dotProductLong2(ByVal xs As IList(Of Long), ByVal ys As IList(Of Long)) As Long
			Dim result As Long = 0
			Dim n As Integer = xs.Count

			If ys.Count <> n Then
				Throw New System.ArgumentException("Different array sizes")
			End If

			For i As Integer = 0 To n - 1
				result += CLng(xs(i)) * ys(i)
			Next i
			Return result
		End Function

		''' 
		''' <param name="xs"> </param>
		''' <param name="ys">
		''' @return </param>
		Public Shared Function dotProductLong(ByVal xs() As Integer, ByVal ys() As Integer) As Long
			Dim result As Long = 0
			Dim n As Integer = xs.Length

			If ys.Length <> n Then
				Throw New System.ArgumentException("Different array sizes")
			End If

			For i As Integer = 0 To n - 1
				result += CLng(xs(i)) * ys(i)
			Next i
			Return result
		End Function


		Public Shared Function empty() As Integer()
			Return New Integer(){}
		End Function


		Public Shared Function [of](ParamArray ByVal arr() As Integer) As Integer()
			Return arr
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter copy was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Shared Function copy(ByVal copy_Conflict() As Integer) As Integer()
			Dim ret(copy_Conflict.Length - 1) As Integer
			Array.Copy(copy_Conflict, 0, ret, 0, ret.Length)
			Return ret
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter copy was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Shared Function copy(ByVal copy_Conflict() As Long) As Long()
			Dim ret(copy_Conflict.Length - 1) As Long
			Array.Copy(copy_Conflict, 0, ret, 0, ret.Length)
			Return ret
		End Function


		Public Shared Function doubleCopyOf(ByVal data() As Single) As Double()
			Dim ret(data.Length - 1) As Double
			For i As Integer = 0 To ret.Length - 1
				ret(i) = data(i)
			Next i
			Return ret
		End Function

		Public Shared Function floatCopyOf(ByVal data() As Double) As Single()
			If data.Length = 0 Then
				Return New Single(0){}
			End If
			Dim ret(data.Length - 1) As Single
			For i As Integer = 0 To ret.Length - 1
				ret(i) = CSng(data(i))
			Next i
			Return ret
		End Function


		''' <summary>
		''' Returns a subset of an array from 0 to "to" (exclusive)
		''' </summary>
		''' <param name="data"> the data to getFromOrigin a subset of </param>
		''' <param name="to">   the end point of the data </param>
		''' <returns> the subset of the data specified </returns>
		Public Shared Function range(ByVal data() As Double, ByVal [to] As Integer) As Double()
			Return range(data, [to], 1)
		End Function


		''' <summary>
		''' Returns a subset of an array from 0 to "to" (exclusive) using the specified stride
		''' </summary>
		''' <param name="data">   the data to getFromOrigin a subset of </param>
		''' <param name="to">     the end point of the data </param>
		''' <param name="stride"> the stride to go through the array </param>
		''' <returns> the subset of the data specified </returns>
		Public Shared Function range(ByVal data() As Double, ByVal [to] As Integer, ByVal stride As Integer) As Double()
			Return range(data, [to], stride, 1)
		End Function


		''' <summary>
		''' Returns a subset of an array from 0 to "to"
		''' using the specified stride
		''' </summary>
		''' <param name="data">                  the data to getFromOrigin a subset of </param>
		''' <param name="to">                    the end point of the data </param>
		''' <param name="stride">                the stride to go through the array </param>
		''' <param name="numElementsEachStride"> the number of elements to collect at each stride </param>
		''' <returns> the subset of the data specified </returns>
		Public Shared Function range(ByVal data() As Double, ByVal [to] As Integer, ByVal stride As Integer, ByVal numElementsEachStride As Integer) As Double()
			Dim ret(([to] \ stride) - 1) As Double
			If ret.Length < 1 Then
				ret = New Double(0){}
			End If
			Dim count As Integer = 0
			For i As Integer = 0 To data.Length - 1 Step stride
				For j As Integer = 0 To numElementsEachStride - 1
					If i + j >= data.Length OrElse count >= ret.Length Then
						Exit For
					End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = data[i + j];
					ret(count) = data(i + j)
						count += 1
				Next j
			Next i
			Return ret
		End Function

		Public Shared Function toList(ParamArray ByVal ints() As Integer) As IList(Of Integer)
			If ints Is Nothing Then
				Return Nothing
			End If
			Dim ret As IList(Of Integer) = New List(Of Integer)()
			For Each anInt As Integer In ints
				ret.Add(anInt)
			Next anInt
			Return ret
		End Function

		Public Shared Function toArray(ByVal list As IList(Of Integer)) As Integer()
			Dim ret(list.Count - 1) As Integer
			For i As Integer = 0 To list.Count - 1
				ret(i) = list(i)
			Next i
			Return ret
		End Function

		Public Shared Function toArrayLong(ByVal list As IList(Of Long)) As Long()
			Dim ret(list.Count - 1) As Long
			For i As Integer = 0 To list.Count - 1
				ret(i) = list(i)
			Next i
			Return ret
		End Function


		Public Shared Function toArrayDouble(ByVal list As IList(Of Double)) As Double()
			Dim ret(list.Count - 1) As Double
			For i As Integer = 0 To list.Count - 1
				ret(i) = list(i)
			Next i
			Return ret

		End Function


		''' <summary>
		''' Generate an int array ranging from "from" to "to".
		''' The total number of elements is (from-to)/increment - i.e., range(0,2,1) returns [0,1]
		''' If from is > to this method will count backwards
		''' </summary>
		''' <param name="from">      the from </param>
		''' <param name="to">        the end point of the data </param>
		''' <param name="increment"> the amount to increment by </param>
		''' <returns> the int array with a length equal to absoluteValue(from - to) </returns>
		Public Shared Function range(ByVal from As Integer, ByVal [to] As Integer, ByVal increment As Integer) As Integer()
			Dim diff As Integer = Math.Abs(from - [to])
			Dim ret((diff \ increment) - 1) As Integer
			If ret.Length < 1 Then
				ret = New Integer(0){}
			End If

			If from < [to] Then
				Dim count As Integer = 0
				For i As Integer = from To [to] - 1 Step increment
					If count >= ret.Length Then
						Exit For
					End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = i;
					ret(count) = i
						count += 1
				Next i
			ElseIf from > [to] Then
				Dim count As Integer = 0
				For i As Integer = from - 1 To [to] Step -increment
					If count >= ret.Length Then
						Exit For
					End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = i;
					ret(count) = i
						count += 1
				Next i
			End If

			Return ret
		End Function


		Public Shared Function range(ByVal from As Long, ByVal [to] As Long, ByVal increment As Long) As Long()
			Dim diff As Long = Math.Abs(from - [to])
			Dim ret(CInt(diff \ increment) - 1) As Long
			If ret.Length < 1 Then
				ret = New Long(0){}
			End If

			If from < [to] Then
				Dim count As Integer = 0
				For i As Long = from To [to] - 1 Step increment
					If count >= ret.Length Then
						Exit For
					End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = i;
					ret(count) = i
						count += 1
				Next i
			ElseIf from > [to] Then
				Dim count As Integer = 0
				For i As Integer = CInt(from) - 1 To [to] Step -increment
					If count >= ret.Length Then
						Exit For
					End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = i;
					ret(count) = i
						count += 1
				Next i
			End If

			Return ret
		End Function

		''' <summary>
		''' Generate an int array ranging from "from" to "to".
		''' The total number of elements is (from-to) - i.e., range(0,2) returns [0,1]
		''' If from is > to this method will count backwards
		''' </summary>
		''' <param name="from"> the from </param>
		''' <param name="to">   the end point of the data </param>
		''' <returns> the int array with a length equal to absoluteValue(from - to) </returns>
		Public Shared Function range(ByVal from As Integer, ByVal [to] As Integer) As Integer()
			If from = [to] Then
				Return New Integer(){}
			End If
			Return range(from, [to], 1)
		End Function

		Public Shared Function range(ByVal from As Long, ByVal [to] As Long) As Long()
			If from = [to] Then
				Return New Long(){}
			End If
			Return range(from, [to], 1)
		End Function

		Public Shared Function toDoubles(ByVal ints() As Integer) As Double()
			Dim ret(ints.Length - 1) As Double
			For i As Integer = 0 To ints.Length - 1
				ret(i) = CDbl(ints(i))
			Next i
			Return ret
		End Function

		Public Shared Function toDoubles(ByVal ints() As Long) As Double()
			Dim ret(ints.Length - 1) As Double
			For i As Integer = 0 To ints.Length - 1
				ret(i) = CDbl(ints(i))
			Next i
			Return ret
		End Function

		Public Shared Function toDoubles(ByVal ints() As Single) As Double()
			Dim ret(ints.Length - 1) As Double
			For i As Integer = 0 To ints.Length - 1
				ret(i) = CDbl(ints(i))
			Next i
			Return ret
		End Function

		Public Shared Function toFloats(ByVal ints()() As Integer) As Single()
			Return toFloats(Ints.concat(ints))
		End Function

		Public Shared Function toDoubles(ByVal ints()() As Integer) As Double()
			Return toDoubles(Ints.concat(ints))
		End Function

		Public Shared Function toShorts(ByVal ints() As Long) As Short()
			Dim ret As val = New Short(ints.Length - 1){}
			For i As Integer = 0 To ints.Length - 1
				ret(i) = CShort(ints(i))
			Next i
			Return ret
		End Function

		Public Shared Function toShorts(ByVal ints() As Integer) As Short()
			Dim ret As val = New Short(ints.Length - 1){}
			For i As Integer = 0 To ints.Length - 1
				ret(i) = CShort(ints(i))
			Next i
			Return ret
		End Function

		Public Shared Function toShorts(ByVal ints() As Single) As Short()
			Dim ret As val = New Short(ints.Length - 1){}
			For i As Integer = 0 To ints.Length - 1
				ret(i) = CShort(Math.Truncate(ints(i)))
			Next i
			Return ret
		End Function

		Public Shared Function toShorts(ByVal ints() As Double) As Short()
			Dim ret As val = New Short(ints.Length - 1){}
			For i As Integer = 0 To ints.Length - 1
				ret(i) = CShort(Math.Truncate(ints(i)))
			Next i
			Return ret
		End Function

		Public Shared Function toFloats(ByVal ints() As Integer) As Single()
			Dim ret(ints.Length - 1) As Single
			For i As Integer = 0 To ints.Length - 1
				ret(i) = CSng(ints(i))
			Next i
			Return ret
		End Function

		Public Shared Function toFloats(ByVal ints() As Long) As Single()
			Dim ret(ints.Length - 1) As Single
			For i As Integer = 0 To ints.Length - 1
				ret(i) = CSng(ints(i))
			Next i
			Return ret
		End Function

		Public Shared Function toFloats(ByVal ints() As Double) As Single()
			Dim ret(ints.Length - 1) As Single
			For i As Integer = 0 To ints.Length - 1
				ret(i) = CSng(ints(i))
			Next i
			Return ret
		End Function

		Public Shared Function cutBelowZero(ByVal data() As Integer) As Integer()
			Dim ret As val = New Integer(data.Length - 1){}
			For i As Integer = 0 To data.Length - 1
				ret(i) = If(data(i) < 0, 0, data(i))
			Next i
			Return ret
		End Function

		Public Shared Function cutBelowZero(ByVal data() As Long) As Long()
			Dim ret As val = New Long(data.Length - 1){}
			For i As Integer = 0 To data.Length - 1
				ret(i) = If(data(i) < 0, 0, data(i))
			Next i
			Return ret
		End Function

		Public Shared Function cutBelowZero(ByVal data() As Short) As Short()
			Dim ret As val = New Short(data.Length - 1){}
			For i As Integer = 0 To data.Length - 1
				ret(i) = If(data(i) < 0, 0, data(i))
			Next i
			Return ret
		End Function

		Public Shared Function cutBelowZero(ByVal data() As SByte) As SByte()
			Dim ret As val = New SByte(data.Length - 1){}
			For i As Integer = 0 To data.Length - 1
				ret(i) = If(data(i) < 0, 0, data(i))
			Next i
			Return ret
		End Function

		''' <summary>
		''' Return a copy of this array with the
		''' given index omitted
		''' </summary>
		''' <param name="data">     the data to copy </param>
		''' <param name="index">    the index of the item to remove </param>
		''' <param name="newValue"> the newValue to replace </param>
		''' <returns> the new array with the omitted
		''' item </returns>
		Public Shared Function replace(ByVal data() As Integer, ByVal index As Integer, ByVal newValue As Integer) As Integer()
			Dim copy() As Integer = ArrayUtil.copy(data)
			copy(index) = newValue
			Return copy
		End Function

		''' <summary>
		''' Return a copy of this array with only the
		''' given index(es) remaining
		''' </summary>
		''' <param name="data">  the data to copy </param>
		''' <param name="index"> the index of the item to remove </param>
		''' <returns> the new array with the omitted
		''' item </returns>
		Public Shared Function keep(ByVal data() As Integer, ParamArray ByVal index() As Integer) As Integer()
			If index.Length = data.Length Then
				Return data
			End If

			Dim ret(index.Length - 1) As Integer
			Dim count As Integer = 0
			For i As Integer = 0 To data.Length - 1
				If Ints.contains(index, i) Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = data[i];
					ret(count) = data(i)
						count += 1
				End If
			Next i

			Return ret
		End Function

		''' <summary>
		''' Return a copy of this array with only the
		''' given index(es) remaining
		''' </summary>
		''' <param name="data">  the data to copy </param>
		''' <param name="index"> the index of the item to remove </param>
		''' <returns> the new array with the omitted
		''' item </returns>
		Public Shared Function keep(ByVal data() As Long, ParamArray ByVal index() As Integer) As Long()
			If index.Length = data.Length Then
				Return data
			End If

			Dim ret(index.Length - 1) As Long
			Dim count As Integer = 0
			For i As Integer = 0 To data.Length - 1
				If Ints.contains(index, i) Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = data[i];
					ret(count) = data(i)
						count += 1
				End If
			Next i

			Return ret
		End Function


		''' <summary>
		''' Return a copy of this array with the
		''' given index omitted
		''' 
		''' PLEASE NOTE: index to be omitted must exist in source array.
		''' </summary>
		''' <param name="data">  the data to copy </param>
		''' <param name="index"> the index of the item to remove </param>
		''' <returns> the new array with the omitted
		''' item </returns>
		Public Shared Function removeIndex(ByVal data() As Integer, ParamArray ByVal index() As Integer) As Integer()
			If index.Length >= data.Length Then
				Throw New System.InvalidOperationException("Illegal remove: indexes.length > data.length (index.length=" & index.Length & ", data.length=" & data.Length & ")")
			End If
			Dim offset As Integer = 0
	'        
	'            workaround for non-existent indexes (such as Integer.MAX_VALUE)
	'        
	'        
	'        for (int i = 0; i < index.length; i ++) {
	'            if (index[i] >= data.length || index[i] < 0) offset++;
	'        }
	'        

			Dim ret((data.Length - index.Length + offset) - 1) As Integer
			Dim count As Integer = 0
			For i As Integer = 0 To data.Length - 1
				If Not Ints.contains(index, i) Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = data[i];
					ret(count) = data(i)
						count += 1
				End If
			Next i

			Return ret
		End Function

		Public Shared Function removeIndex(ByVal data() As Long, ParamArray ByVal index() As Integer) As Long()
			If index.Length >= data.Length Then
				Throw New System.InvalidOperationException("Illegal remove: indexes.length >= data.length (index.length=" & index.Length & ", data.length=" & data.Length & ")")
			End If
			Dim offset As Integer = 0
	'        
	'            workaround for non-existent indexes (such as Integer.MAX_VALUE)
	'
	'
	'        for (int i = 0; i < index.length; i ++) {
	'            if (index[i] >= data.length || index[i] < 0) offset++;
	'        }
	'        

			Dim ret((data.Length - index.Length + offset) - 1) As Long
			Dim count As Integer = 0
			For i As Integer = 0 To data.Length - 1
				If Not Ints.contains(index, i) Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = data[i];
					ret(count) = data(i)
						count += 1
				End If
			Next i

			Return ret
		End Function



		''' <summary>
		''' Zip 2 arrays in to:
		''' </summary>
		''' <param name="as"> </param>
		''' <param name="bs">
		''' @return </param>
		Public Shared Function zip(ByVal [as]() As Integer, ByVal bs() As Integer) As Integer()()
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim result[][] As Integer = new Integer[as.Length][2]
			Dim result()() As Integer = RectangularArrays.RectangularIntegerArray([as].Length, 2)
			For i As Integer = 0 To result.Length - 1
				result(i) = New Integer() {[as](i), bs(i)}
			Next i

			Return result
		End Function

		''' <summary>
		''' Get the tensor matrix multiply shape </summary>
		''' <param name="aShape"> the shape of the first array </param>
		''' <param name="bShape"> the shape of the second array </param>
		''' <param name="axes"> the axes to do the multiply </param>
		''' <returns> the shape for tensor matrix multiply </returns>
		Public Shared Function getTensorMmulShape(ByVal aShape() As Long, ByVal bShape() As Long, ByVal axes()() As Integer) As Long()

			Dim validationLength As Integer = Math.Min(axes(0).Length, axes(1).Length)
			For i As Integer = 0 To validationLength - 1
				If aShape(axes(0)(i)) <> bShape(axes(1)(i)) Then
					Throw New System.ArgumentException("Size of the given axes a" & " t each dimension must be the same size.")
				End If
				If axes(0)(i) < 0 Then
					axes(0)(i) += aShape.Length
				End If
				If axes(1)(i) < 0 Then
					axes(1)(i) += bShape.Length
				End If

			Next i

			Dim listA As IList(Of Integer) = New List(Of Integer)()
			For i As Integer = 0 To aShape.Length - 1
				If Not Ints.contains(axes(0), i) Then
					listA.Add(i)
				End If
			Next i



			Dim listB As IList(Of Integer) = New List(Of Integer)()
			For i As Integer = 0 To bShape.Length - 1
				If Not Ints.contains(axes(1), i) Then
					listB.Add(i)
				End If
			Next i


			Dim n2 As Integer = 1
			Dim aLength As Integer = Math.Min(aShape.Length, axes(0).Length)
			For i As Integer = 0 To aLength - 1
				n2 *= aShape(axes(0)(i))
			Next i

			'if listA and listB are empty these donot initialize.
			'so initializing with {1} which will then get overriden if not empty
			Dim oldShapeA() As Long
			If listA.Count = 0 Then
				oldShapeA = New Long() {1}
			Else
				oldShapeA = Longs.toArray(listA)
				For i As Integer = 0 To oldShapeA.Length - 1
					oldShapeA(i) = aShape(CInt(oldShapeA(i)))
				Next i
			End If

			Dim n3 As Integer = 1
			Dim bNax As Integer = Math.Min(bShape.Length, axes(1).Length)
			For i As Integer = 0 To bNax - 1
				n3 *= bShape(axes(1)(i))
			Next i


			Dim oldShapeB() As Long
			If listB.Count = 0 Then
				oldShapeB = New Long() {1}
			Else
				oldShapeB = Longs.toArray(listB)
				For i As Integer = 0 To oldShapeB.Length - 1
					oldShapeB(i) = bShape(CInt(oldShapeB(i)))
				Next i
			End If


			Dim aPlusB() As Long = Longs.concat(oldShapeA, oldShapeB)
			Return aPlusB
		End Function

		''' <summary>
		''' Permute the given input
		''' switching the dimensions of the input shape
		''' array with in the order of the specified
		''' dimensions </summary>
		''' <param name="shape"> the shape to permute </param>
		''' <param name="dimensions"> the dimensions
		''' @return </param>
		Public Shared Function permute(ByVal shape() As Integer, ByVal dimensions() As Integer) As Integer()
			Dim ret(shape.Length - 1) As Integer
			For i As Integer = 0 To shape.Length - 1
				ret(i) = shape(dimensions(i))
			Next i

			Return ret
		End Function


		Public Shared Function permute(ByVal shape() As Long, ByVal dimensions() As Integer) As Long()
			Dim ret As val = New Long(shape.Length - 1){}
			For i As Integer = 0 To shape.Length - 1
				ret(i) = shape(dimensions(i))
			Next i

			Return ret
		End Function


		''' <summary>
		''' Original credit: https://github.com/alberts/array4j/blob/master/src/main/java/net/lunglet/util/ArrayUtils.java </summary>
		''' <param name="a">
		''' @return </param>
		Public Shared Function argsort(ByVal a() As Integer) As Integer()
			Return argsort(a, True)
		End Function


		''' 
		''' <param name="a"> </param>
		''' <param name="ascending">
		''' @return </param>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static int[] argsort(final int[] a, final boolean ascending)
		Public Shared Function argsort(ByVal a() As Integer, ByVal ascending As Boolean) As Integer()
			Dim indexes(a.Length - 1) As Integer?
			For i As Integer = 0 To indexes.Length - 1
				indexes(i) = i
			Next i
			Array.Sort(indexes, New ComparatorAnonymousInnerClass(a, ascending))

			Dim ret(indexes.Length - 1) As Integer
			For i As Integer = 0 To ret.Length - 1
				ret(i) = indexes(i)
			Next i

			Return ret
		End Function

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of Integer)

			private Integer() a
			private Boolean ascending

			public ComparatorAnonymousInnerClass(Integer() a, Boolean ascending)
			If True Then
				Me.a = a
				Me.ascending = ascending
			End If

			public Integer compare(final Integer? i1, final Integer? i2)
			If True Then
				Return (If(ascending, 1, -1)) * Ints.compare(a(i1), a(i2))
			End If
		End Class



		''' <summary>
		''' Convert all dimensions in the specified
		''' axes array to be positive
		''' based on the specified range of values </summary>
		''' <param name="range"> </param>
		''' <param name="axes">
		''' @return </param>
		public static Integer() convertNegativeIndices(Integer range, Integer() axes)
		If True Then
			Dim axesRet() As Integer = ArrayUtil.range(0, range)
			Dim newAxes() As Integer = ArrayUtil.copy(axes)
			For i As Integer = 0 To axes.length - 1
				newAxes(i) = axes(axesRet(i))
			Next i

			Return newAxes
		End If



		''' <summary>
		''' Generate an array from 0 to length
		''' and generate take a subset </summary>
		''' <param name="length"> the length to generate to </param>
		''' <param name="from"> the begin of the interval to take </param>
		''' <param name="to"> the end of the interval to take </param>
		''' <returns> the generated array </returns>
		public static Integer() copyOfRangeFrom(Integer length, Integer from, Integer [to])
		If True Then
			Return Arrays.CopyOfRange(ArrayUtil.range(0, length), from, [to])

		End If

		'Credit: https://stackoverflow.com/questions/15533854/converting-byte-array-to-double-array

		''' 
		''' <param name="doubleArray">
		''' @return </param>
		public static SByte() toByteArray(Double() doubleArray)
		If True Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim times As Integer = (Len(New Double()) * 8) / (Len(New SByte()) * 8)
			Dim bytes((doubleArray.length * times) - 1) As SByte
			For i As Integer = 0 To doubleArray.length - 1
				ByteBuffer.wrap(bytes, i * times, times).putDouble(doubleArray(i))
			Next i
			Return bytes
		End If

		''' 
		''' <param name="byteArray">
		''' @return </param>
		public static Double() toDoubleArray(SByte() byteArray)
		If True Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim times As Integer = (Len(New Double()) * 8) / (Len(New SByte()) * 8)
			Dim doubles((byteArray.length \ times) - 1) As Double
			For i As Integer = 0 To doubles.Length - 1
				doubles(i) = ByteBuffer.wrap(byteArray, i * times, times).getDouble()
			Next i
			Return doubles
		End If


		''' 
		''' <param name="doubleArray">
		''' @return </param>
		public static SByte() toByteArray(Single() doubleArray)
		If True Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim times As Integer = (Len(New Single()) * 8) / (Len(New SByte()) * 8)
			Dim bytes((doubleArray.length * times) - 1) As SByte
			For i As Integer = 0 To doubleArray.length - 1
				ByteBuffer.wrap(bytes, i * times, times).putFloat(doubleArray(i))
			Next i
			Return bytes
		End If

		public static Long() toLongArray(Integer() intArray)
		If True Then
			Dim ret(intArray.length - 1) As Long
			For i As Integer = 0 To intArray.length - 1
				ret(i) = intArray(i)
			Next i
			Return ret
		End If

		public static Long() toLongArray(Single() array)
		If True Then
			Dim ret As val = New Long(array.length - 1){}
			For i As Integer = 0 To array.length - 1
				ret(i) = CLng(Math.Truncate(array(i)))
			Next i
			Return ret
		End If

		''' 
		''' <param name="byteArray">
		''' @return </param>
		public static Single() toFloatArray(SByte() byteArray)
		If True Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim times As Integer = (Len(New Single()) * 8) / (Len(New SByte()) * 8)
			Dim doubles((byteArray.length \ times) - 1) As Single
			For i As Integer = 0 To doubles.Length - 1
				doubles(i) = ByteBuffer.wrap(byteArray, i * times, times).getFloat()
			Next i
			Return doubles
		End If

		''' 
		''' <param name="intArray">
		''' @return </param>
		public static SByte() toByteArray(Integer() intArray)
		If True Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim times As Integer = (Len(New Integer()) * 8) / (Len(New SByte()) * 8)
			Dim bytes((intArray.length * times) - 1) As SByte
			For i As Integer = 0 To intArray.length - 1
				ByteBuffer.wrap(bytes, i * times, times).putInt(intArray(i))
			Next i
			Return bytes
		End If

		''' 
		''' <param name="byteArray">
		''' @return </param>
		public static Integer() toIntArray(SByte() byteArray)
		If True Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim times As Integer = (Len(New Integer()) * 8) / (Len(New SByte()) * 8)
			Dim ints((byteArray.length \ times) - 1) As Integer
			For i As Integer = 0 To ints.Length - 1
				ints(i) = ByteBuffer.wrap(byteArray, i * times, times).getInt()
			Next i
			Return ints
		End If


		''' <summary>
		''' Return a copy of this array with the
		''' given index omitted
		''' </summary>
		''' <param name="data">  the data to copy </param>
		''' <param name="index"> the index of the item to remove </param>
		''' <returns> the new array with the omitted
		''' item </returns>
		public static Integer() removeIndex(Integer() data, Integer index)
		If True Then
			If data Is Nothing Then
				Return Nothing
			End If

			If index >= data.length Then
				Throw New System.ArgumentException("Unable to remove index " & index & " was >= data.length")
			End If
			If data.length < 1 Then
				Return data
			End If
			If index < 0 Then
				Return data
			End If

			Dim len As Integer = data.length
			Dim result(len - 2) As Integer
			Array.Copy(data, 0, result, 0, index)
			Array.Copy(data, index + 1, result, index, len - index - 1)
			Return result
		End If

		public static Long() removeIndex(Long() data, Integer index)
		If True Then
			If data Is Nothing Then
				Return Nothing
			End If

			If index >= data.length Then
				Throw New System.ArgumentException("Unable to remove index " & index & " was >= data.length")
			End If
			If data.length < 1 Then
				Return data
			End If
			If index < 0 Then
				Return data
			End If

			Dim len As Integer = data.length
			Dim result(len - 2) As Long
			Array.Copy(data, 0, result, 0, index)
			Array.Copy(data, index + 1, result, index, len - index - 1)
			Return result
		End If


		''' <summary>
		''' Create a copy of the given array
		''' starting at the given index with the given length.
		''' 
		''' The intent here is for striding.
		''' 
		''' For example in slicing, you want the major stride to be first.
		''' You achieve this by taking the last index
		''' of the matrix's stride and putting
		''' this as the first stride of the new ndarray
		''' for slicing.
		''' 
		''' All of the elements except the copied elements are
		''' initialized as the given value </summary>
		''' <param name="valueStarting">  the starting value </param>
		''' <param name="copy"> the array to copy </param>
		''' <param name="idxFrom"> the index to start at in the from array </param>
		''' <param name="idxAt"> the index to start at in the return array </param>
		''' <param name="length"> the length of the array to create </param>
		''' <returns> the given array </returns>
		public static Integer() valueStartingAt(Integer valueStarting, Integer() copy, Integer idxFrom, Integer idxAt, Integer length)
		If True Then
			Dim ret(length - 1) As Integer
			Arrays.Fill(ret, valueStarting)
			For i As Integer = 0 To length - 1
				If i + idxFrom >= copy.length OrElse i + idxAt >= ret.Length Then
					Exit For
				End If
				ret(i + idxAt) = copy(i + idxFrom)
			Next i

			Return ret
		End If



		''' <summary>
		''' Returns the array with the item in index
		''' removed, if the array is empty it will return the array itself
		''' </summary>
		''' <param name="data">  the data to remove data from </param>
		''' <param name="index"> the index of the item to remove </param>
		''' <returns> a copy of the array with the removed item,
		''' or the array itself if empty </returns>
		public static Integer?() removeIndex(Integer?() data, Integer index)
		If True Then
			If data Is Nothing Then
				Return Nothing
			End If
			If data.length < 1 Then
				Return data
			End If
			Dim len As Integer = data.length
			Dim result(len - 2) As Integer?
			Array.Copy(data, 0, result, 0, index)
			Array.Copy(data, index + 1, result, index, len - index - 1)
			Return result
		End If


		''' <summary>
		''' Computes the standard packed array strides for a given shape.
		''' </summary>
		''' <param name="shape">    the shape of a matrix: </param>
		''' <param name="startNum"> the start number for the strides </param>
		''' <returns> the strides for a matrix of n dimensions </returns>
		public static Integer() calcStridesFortran(Integer() shape, Integer startNum)
		If True Then
			If shape.length = 2 AndAlso (shape(0) = 1 OrElse shape(1) = 1) Then
				Dim ret(1) As Integer
				Arrays.Fill(ret, startNum)
				Return ret
			End If

			Dim dimensions As Integer = shape.length
			Dim stride(dimensions - 1) As Integer
			Dim st As Integer = startNum
			For j As Integer = 0 To stride.Length - 1
				stride(j) = st
				st *= shape(j)
			Next j

			Return stride
		End If

		''' <summary>
		''' Computes the standard packed array strides for a given shape.
		''' </summary>
		''' <param name="shape">    the shape of a matrix: </param>
		''' <param name="startNum"> the start number for the strides </param>
		''' <returns> the strides for a matrix of n dimensions </returns>
		public static Long() calcStridesFortran(Long() shape, Integer startNum)
		If True Then
			If shape.length = 2 AndAlso (shape(0) = 1 OrElse shape(1) = 1) Then
				Dim ret(1) As Long
				Arrays.Fill(ret, startNum)
				Return ret
			End If

			Dim dimensions As Integer = shape.length
			Dim stride(dimensions - 1) As Long
			Dim st As Integer = startNum
			For j As Integer = 0 To stride.Length - 1
				stride(j) = st
				st *= shape(j)
			Next j

			Return stride
		End If

		''' <summary>
		''' Computes the standard packed array strides for a given shape.
		''' </summary>
		''' <param name="shape"> the shape of a matrix: </param>
		''' <returns> the strides for a matrix of n dimensions </returns>
		public static Integer() calcStridesFortran(Integer() shape)
		If True Then
			Return calcStridesFortran(shape, 1)
		End If

		public static Long() calcStridesFortran(Long() shape)
		If True Then
			Return calcStridesFortran(shape, 1)
		End If


		''' <summary>
		''' Computes the standard packed array strides for a given shape.
		''' </summary>
		''' <param name="shape">      the shape of a matrix: </param>
		''' <param name="startValue"> the startValue for the strides </param>
		''' <returns> the strides for a matrix of n dimensions </returns>
		public static Integer() calcStrides(Integer() shape, Integer startValue)
		If True Then
			If shape.length = 2 AndAlso (shape(0) = 1 OrElse shape(1) = 1) Then
				Dim ret(1) As Integer
				Arrays.Fill(ret, startValue)
				Return ret
			End If


			Dim dimensions As Integer = shape.length
			Dim stride(dimensions - 1) As Integer

			Dim st As Integer = startValue
			For j As Integer = dimensions - 1 To 0 Step -1
				stride(j) = st
				st *= shape(j)
			Next j

			Return stride
		End If

		''' <summary>
		''' Computes the standard packed array strides for a given shape.
		''' </summary>
		''' <param name="shape">      the shape of a matrix: </param>
		''' <param name="startValue"> the startValue for the strides </param>
		''' <returns> the strides for a matrix of n dimensions </returns>
		public static Long() calcStrides(Long() shape, Integer startValue)
		If True Then
			If shape.length = 2 AndAlso (shape(0) = 1 OrElse shape(1) = 1) Then
				Dim ret(1) As Long
				Arrays.Fill(ret, startValue)
				Return ret
			End If


			Dim dimensions As Integer = shape.length
			Dim stride(dimensions - 1) As Long

			Dim st As Integer = startValue
			For j As Integer = dimensions - 1 To 0 Step -1
				stride(j) = st
				st *= shape(j)
			Next j

			Return stride
		End If


		''' <summary>
		''' Returns true if the given
		''' two arrays are reverse copies of each other </summary>
		''' <param name="first"> </param>
		''' <param name="second">
		''' @return </param>
		public static Boolean isInverse(Integer() first, Integer() second)
		If True Then
			Dim backWardCount As Integer = second.length - 1
			For i As Integer = 0 To first.length - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (first[i] != second[backWardCount--])
				If first(i) <> second(backWardCount) Then
						backWardCount -= 1
					Return False
					Else
						backWardCount -= 1
					End If
			Next i
			Return True
		End If

		public static Integer() plus(Integer() ints, Integer mult)
		If True Then
			Dim ret(ints.length - 1) As Integer
			For i As Integer = 0 To ints.length - 1
				ret(i) = ints(i) + mult
			Next i
			Return ret
		End If


		public static Integer() plus(Integer() ints, Integer() mult)
		If True Then
			If ints.length <> mult.length Then
				Throw New System.ArgumentException("Both arrays must have the same length")
			End If
			Dim ret(ints.length - 1) As Integer
			For i As Integer = 0 To ints.length - 1
				ret(i) = ints(i) + mult(i)
			Next i
			Return ret
		End If

		public static Integer() times(Integer() ints, Integer mult)
		If True Then
			Dim ret(ints.length - 1) As Integer
			For i As Integer = 0 To ints.length - 1
				ret(i) = ints(i) * mult
			Next i
			Return ret
		End If

		public static Integer() times(Integer() ints, Integer() mult)
		If True Then
			Preconditions.checkArgument(ints.length = mult.length, "Ints and mult must be the same length")
			Dim ret(ints.length - 1) As Integer
			For i As Integer = 0 To ints.length - 1
				ret(i) = ints(i) * mult(i)
			Next i
			Return ret
		End If



		''' <summary>
		''' For use with row vectors to ensure consistent strides
		''' with varying offsets
		''' </summary>
		''' <param name="arr"> the array to get the stride for </param>
		''' <returns> the stride </returns>
		public static Integer nonOneStride(Integer() arr)
		If True Then
			For i As Integer = 0 To arr.length - 1
				If arr(i) <> 1 Then
					Return arr(i)
				End If
			Next i
			Return 1
		End If


		''' <summary>
		''' Computes the standard packed array strides for a given shape.
		''' </summary>
		''' <param name="shape"> the shape of a matrix: </param>
		''' <returns> the strides for a matrix of n dimensions </returns>
		public static Integer() calcStrides(Integer() shape)
		If True Then
			Return calcStrides(shape, 1)
		End If

		public static Long() calcStrides(Long() shape)
		If True Then
			Return calcStrides(shape, 1)
		End If


		''' <summary>
		''' Create a backwards copy of the given array
		''' </summary>
		''' <param name="e"> the array to createComplex a reverse clone of </param>
		''' <returns> the reversed copy </returns>
		public static Integer() reverseCopy(Integer() e)
		If True Then
			If e.length < 1 Then
				Return e
			End If

			Dim copy(e.length - 1) As Integer
			For i As Integer = 0 To e.length \ 2
				Dim temp As Integer = e(i)
				copy(i) = e(e.length - i - 1)
				copy(e.length - i - 1) = temp
			Next i
			Return copy
		End If

		public static Long() reverseCopy(Long() e)
		If True Then
			If e.length < 1 Then
				Return e
			End If

			Dim copy(e.length - 1) As Long
			For i As Integer = 0 To e.length \ 2
				Dim temp As Long = e(i)
				copy(i) = e(e.length - i - 1)
				copy(e.length - i - 1) = temp
			Next i
			Return copy
		End If


		public static Double() read(Integer length, DataInputStream dis) throws IOException
		If True Then
			Dim ret(length - 1) As Double
			For i As Integer = 0 To length - 1
				ret(i) = dis.readDouble()
			Next i
			Return ret
		End If


		public static void write(Double() data, DataOutputStream dos) throws IOException
		If True Then
			For i As Integer = 0 To data.length - 1
				dos.writeDouble(data(i))
			Next i
		End If

		public static Double() readDouble(Integer length, DataInputStream dis) throws IOException
		If True Then
			Dim ret(length - 1) As Double
			For i As Integer = 0 To length - 1
				ret(i) = dis.readDouble()
			Next i
			Return ret
		End If


		public static Single() readFloat(Integer length, DataInputStream dis) throws IOException
		If True Then
			Dim ret(length - 1) As Single
			For i As Integer = 0 To length - 1
				ret(i) = dis.readFloat()
			Next i
			Return ret
		End If


		public static void write(Single() data, DataOutputStream dos) throws IOException
		If True Then
			For i As Integer = 0 To data.length - 1
				dos.writeFloat(data(i))
			Next i
		End If


		public static void assertSquare(Double()... d)
		If True Then
			If d.length > 2 Then
				For i As Integer = 0 To d.length - 1
					assertSquare(d(i))
				Next i
			Else
				Dim firstLength As Integer = d(0).length
				For i As Integer = 1 To d.length - 1
					Preconditions.checkState(d(i).length = firstLength)
				Next i
			End If
		End If


		''' <summary>
		''' Multiply the given array
		''' by the given scalar </summary>
		''' <param name="arr"> the array to multily </param>
		''' <param name="mult"> the scalar to multiply by </param>
		public static void multiplyBy(Integer() arr, Integer mult)
		If True Then
			For i As Integer = 0 To arr.length - 1
				arr(i) *= mult
			Next i

		End If

		''' <summary>
		''' Reverse the passed in array in place
		''' </summary>
		''' <param name="e"> the array to reverse </param>
		public static void reverse(Integer() e)
		If True Then
			For i As Integer = 0 To e.length \ 2
				Dim temp As Integer = e(i)
				e(i) = e(e.length - i - 1)
				e(e.length - i - 1) = temp
			Next i
		End If

		public static void reverse(Long() e)
		If True Then
			For i As Integer = 0 To e.length \ 2
				Dim temp As Long = e(i)
				e(i) = e(e.length - i - 1)
				e(e.length - i - 1) = temp
			Next i
		End If


		public static IList(Of Double()) zerosMatrix(Long... dimensions)
		If True Then
			Dim ret As IList(Of Double()) = New List(Of Double())()
			For i As Integer = 0 To dimensions.length - 1
				ret.Add(New Double(CInt(Math.Truncate(dimensions(i))) - 1){})
			Next i
			Return ret
		End If

		public static IList(Of Double()) zerosMatrix(Integer... dimensions)
		If True Then
			Dim ret As IList(Of Double()) = New List(Of Double())()
			For i As Integer = 0 To dimensions.length - 1
				ret.Add(New Double(dimensions(i) - 1){})
			Next i
			Return ret
		End If


		public static Single() reverseCopy(Single() e)
		If True Then
			Dim copy(e.length - 1) As Single
			For i As Integer = 0 To e.length \ 2
				Dim temp As Single = e(i)
				copy(i) = e(e.length - i - 1)
				copy(e.length - i - 1) = temp
			Next i
			Return copy

		End If


		public static (Of E) E() reverseCopy(E() e)
		If True Then
			Dim copy() As E = DirectCast(New Object(e.length - 1){}, E())
			For i As Integer = 0 To e.length \ 2
				Dim temp As E = e(i)
				copy(i) = e(e.length - i - 1)
				copy(e.length - i - 1) = temp
			Next i
			Return copy

		End If

		public static (Of E) void reverse(E() e)
		If True Then
			For i As Integer = 0 To e.length \ 2
				Dim temp As E = e(i)
				e(i) = e(e.length - i - 1)
				e(e.length - i - 1) = temp
			Next i
		End If

		public static Boolean() flatten(Boolean()() arr)
		If True Then
			If arr.length = 0 OrElse arr(0).length = 0 Then
				Return New Boolean(){}
			End If
			Dim ret((arr.length * arr(0).length) - 1) As Boolean
			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Array.Copy(arr(i), 0, ret, count, arr(i).length)
				count += arr(i).length
			Next i
			Return ret
		End If

		public static Boolean() flatten(Boolean()()() arr)
		If True Then
			If arr.length = 0 OrElse arr(0).length = 0 OrElse arr(0)(0).length = 0 Then
				Return New Boolean(){}
			End If
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret[] As Boolean = new Boolean[arr.length * arr[0].length * arr[0][0].length]
			Dim ret() As Boolean = RectangularArrays.RectangularBooleanArray(arr.length * arr(0).length * arr(0)(0).length)

			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Dim j As Integer = 0
				Do While j < arr(0).length
					Array.Copy(arr(i)(j), 0, ret, count, arr(0)(0).length)
					count += arr(0)(0).length
					j += 1
				Loop
			Next i
			Return ret
		End If

		public static Single() flatten(Single()() arr)
		If True Then
			If arr.length = 0 OrElse arr(0).length = 0 Then
				Return New Single(){}
			End If
			Dim ret((arr.length * arr(0).length) - 1) As Single
			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Array.Copy(arr(i), 0, ret, count, arr(i).length)
				count += arr(i).length
			Next i
			Return ret
		End If


		public static Single() flatten(Single()()() arr)
		If True Then
			If arr.length = 0 OrElse arr(0).length = 0 OrElse arr(0)(0).length = 0 Then
				Return New Single(){}
			End If
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret[] As Single = new Single[arr.length * arr[0].length * arr[0][0].length]
			Dim ret() As Single = RectangularArrays.RectangularSingleArray(arr.length * arr(0).length * arr(0)(0).length)

			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Dim j As Integer = 0
				Do While j < arr(0).length
					Array.Copy(arr(i)(j), 0, ret, count, arr(0)(0).length)
					count += arr(0)(0).length
					j += 1
				Loop
			Next i

			Return ret
		End If

		public static Double() flatten(Double()()() arr)
		If True Then
			If arr.length = 0 OrElse arr(0).length = 0 OrElse arr(0)(0).length = 0 Then
				Return New Double(){}
			End If
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret[] As Double = new Double[arr.length * arr[0].length * arr[0][0].length]
			Dim ret() As Double = RectangularArrays.RectangularDoubleArray(arr.length * arr(0).length * arr(0)(0).length)

			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Dim j As Integer = 0
				Do While j < arr(0).length
					Array.Copy(arr(i)(j), 0, ret, count, arr(0)(0).length)
					count += arr(0)(0).length
					j += 1
				Loop
			Next i
			Return ret
		End If

		public static Integer() flatten(Integer()()() arr)
		If True Then
			If arr.length = 0 OrElse arr(0).length = 0 OrElse arr(0)(0).length = 0 Then
				Return New Integer(){}
			End If
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret[] As Integer = new Integer[arr.length * arr[0].length * arr[0][0].length]
			Dim ret() As Integer = RectangularArrays.RectangularIntegerArray(arr.length * arr(0).length * arr(0)(0).length)

			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Dim j As Integer = 0
				Do While j < arr(0).length
					Array.Copy(arr(i)(j), 0, ret, count, arr(0)(0).length)
					count += arr(0)(0).length
					j += 1
				Loop
			Next i
			Return ret
		End If

		public static Short() flatten(Short()()() arr)
		If True Then
			If arr.length = 0 OrElse arr(0).length = 0 OrElse arr(0)(0).length = 0 Then
				Return New Short(){}
			End If
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret As val = new Short[arr.length * arr[0].length * arr[0][0].length]
			Dim ret As val = RectangularArrays.RectangularShortArray(arr.length * arr(0).length * arr(0)(0).length)

			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Dim j As Integer = 0
				Do While j < arr(0).length
					Array.Copy(arr(i)(j), 0, ret, count, arr(0)(0).length)
					count += arr(0)(0).length
					j += 1
				Loop
			Next i
			Return ret
		End If

		public static SByte() flatten(SByte()()() arr)
		If True Then
			If arr.length = 0 OrElse arr(0).length = 0 OrElse arr(0)(0).length = 0 Then
				Return New SByte(){}
			End If
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret As val = new SByte[arr.length * arr[0].length * arr[0][0].length]
			Dim ret As val = RectangularArrays.RectangularSByteArray(arr.length * arr(0).length * arr(0)(0).length)

			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Dim j As Integer = 0
				Do While j < arr(0).length
					Array.Copy(arr(i)(j), 0, ret, count, arr(0)(0).length)
					count += arr(0)(0).length
					j += 1
				Loop
			Next i
			Return ret
		End If

		public static Long() flatten(Long()()()() arr)
		If True Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret As val = new Long[arr.length * arr[0].length * arr[0][0].length * arr[0][0][0].length]
			Dim ret As val = RectangularArrays.RectangularLongArray(arr.length * arr(0).length * arr(0)(0).length * arr(0)(0)(0).length)

			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Dim j As Integer = 0
				Do While j < arr(0).length
					Dim k As Integer = 0
					Do While k < arr(0)(0).length
						Array.Copy(arr(i)(j)(k), 0, ret, count, arr(0)(0)(0).length)
						count += arr(0)(0)(0).length
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return ret
		End If

		public static Short() flatten(Short()()()() arr)
		If True Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret As val = new Short[arr.length * arr[0].length * arr[0][0].length * arr[0][0][0].length]
			Dim ret As val = RectangularArrays.RectangularShortArray(arr.length * arr(0).length * arr(0)(0).length * arr(0)(0)(0).length)

			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Dim j As Integer = 0
				Do While j < arr(0).length
					Dim k As Integer = 0
					Do While k < arr(0)(0).length
						Array.Copy(arr(i)(j)(k), 0, ret, count, arr(0)(0)(0).length)
						count += arr(0)(0)(0).length
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return ret
		End If

		public static SByte() flatten(SByte()()()() arr)
		If True Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret As val = new SByte[arr.length * arr[0].length * arr[0][0].length * arr[0][0][0].length]
			Dim ret As val = RectangularArrays.RectangularSByteArray(arr.length * arr(0).length * arr(0)(0).length * arr(0)(0)(0).length)

			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Dim j As Integer = 0
				Do While j < arr(0).length
					Dim k As Integer = 0
					Do While k < arr(0)(0).length
						Array.Copy(arr(i)(j)(k), 0, ret, count, arr(0)(0)(0).length)
						count += arr(0)(0)(0).length
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return ret
		End If

		public static Boolean() flatten(Boolean()()()() arr)
		If True Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret As val = new Boolean[arr.length * arr[0].length * arr[0][0].length * arr[0][0][0].length]
			Dim ret As val = RectangularArrays.RectangularBooleanArray(arr.length * arr(0).length * arr(0)(0).length * arr(0)(0)(0).length)

			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Dim j As Integer = 0
				Do While j < arr(0).length
					Dim k As Integer = 0
					Do While k < arr(0)(0).length
						Array.Copy(arr(i)(j)(k), 0, ret, count, arr(0)(0)(0).length)
						count += arr(0)(0)(0).length
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return ret
		End If

		public static Single() flatten(Single()()()() arr)
		If True Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret[] As Single = new Single[arr.length * arr[0].length * arr[0][0].length * arr[0][0][0].length]
			Dim ret() As Single = RectangularArrays.RectangularSingleArray(arr.length * arr(0).length * arr(0)(0).length * arr(0)(0)(0).length)

			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Dim j As Integer = 0
				Do While j < arr(0).length
					Dim k As Integer = 0
					Do While k < arr(0)(0).length
						Array.Copy(arr(i)(j)(k), 0, ret, count, arr(0)(0)(0).length)
						count += arr(0)(0)(0).length
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return ret
		End If

		public static Double() flatten(Double()()()() arr)
		If True Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret[] As Double = new Double[arr.length * arr[0].length * arr[0][0].length * arr[0][0][0].length]
			Dim ret() As Double = RectangularArrays.RectangularDoubleArray(arr.length * arr(0).length * arr(0)(0).length * arr(0)(0)(0).length)

			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Dim j As Integer = 0
				Do While j < arr(0).length
					Dim k As Integer = 0
					Do While k < arr(0)(0).length
						Array.Copy(arr(i)(j)(k), 0, ret, count, arr(0)(0)(0).length)
						count += arr(0)(0)(0).length
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return ret
		End If

		public static Integer() flatten(Integer()()()() arr)
		If True Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret[] As Integer = new Integer[arr.length * arr[0].length * arr[0][0].length * arr[0][0][0].length]
			Dim ret() As Integer = RectangularArrays.RectangularIntegerArray(arr.length * arr(0).length * arr(0)(0).length * arr(0)(0)(0).length)

			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Dim j As Integer = 0
				Do While j < arr(0).length
					Dim k As Integer = 0
					Do While k < arr(0)(0).length
						Array.Copy(arr(i)(j)(k), 0, ret, count, arr(0)(0)(0).length)
						count += arr(0)(0)(0).length
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return ret
		End If


		public static Integer() flatten(Integer()() arr)
		If True Then
			If arr.length = 0 OrElse arr(0).length = 0 Then
				Return New Integer(){}
			End If
			Dim ret((arr.length * arr(0).length) - 1) As Integer
			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Array.Copy(arr(i), 0, ret, count, arr(i).length)
				count += arr(i).length
			Next i
			Return ret
		End If

		public static Short() flatten(Short()() arr)
		If True Then
			If arr.length = 0 OrElse arr(0).length = 0 Then
				Return New Short(){}
			End If
			Dim ret As val = New Short((arr.length * arr(0).length) - 1){}
			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Array.Copy(arr(i), 0, ret, count, arr(i).length)
				count += arr(i).length
			Next i
			Return ret
		End If

		public static SByte() flatten(SByte()() arr)
		If True Then
			If arr.length = 0 OrElse arr(0).length = 0 Then
				Return New SByte(){}
			End If
			Dim ret As val = New SByte((arr.length * arr(0).length) - 1){}
			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Array.Copy(arr(i), 0, ret, count, arr(i).length)
				count += arr(i).length
			Next i
			Return ret
		End If

		public static Long() flatten(Long()() arr)
		If True Then
			If arr.length = 0 OrElse arr(0).length = 0 Then
				Return New Long(){}
			End If
			Dim ret((arr.length * arr(0).length) - 1) As Long
			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Array.Copy(arr(i), 0, ret, count, arr(i).length)
				count += arr(i).length
			Next i
			Return ret
		End If

		public static Long() flatten(Long()()() arr)
		If True Then
			If arr.length = 0 OrElse arr(0).length = 0 OrElse arr(0)(0).length = 0 Then
				Return New Long(){}
			End If
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret[] As Long = new Long[arr.length * arr[0].length * arr[0][0].length]
			Dim ret() As Long = RectangularArrays.RectangularLongArray(arr.length * arr(0).length * arr(0)(0).length)

			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Dim j As Integer = 0
				Do While j < arr(0).length
					Array.Copy(arr(i)(j), 0, ret, count, arr(0)(0).length)
					count += arr(0)(0).length
					j += 1
				Loop
			Next i
			Return ret
		End If


		''' <summary>
		''' Convert a 2darray in to a flat
		''' array (row wise) </summary>
		''' <param name="arr"> the array to flatten </param>
		''' <returns> a flattened representation of the array </returns>
		public static Double() flatten(Double()() arr)
		If True Then
			If arr.length = 0 OrElse arr(0).length = 0 Then
				Return New Double(){}
			End If
			Dim ret((arr.length * arr(0).length) - 1) As Double
			Dim count As Integer = 0
			For i As Integer = 0 To arr.length - 1
				Array.Copy(arr(i), 0, ret, count, arr(i).length)
				count += arr(i).length
			Next i
			Return ret
		End If

		''' <summary>
		''' Convert a 2darray in to a flat
		''' array (row wise) </summary>
		''' <param name="arr"> the array to flatten </param>
		''' <returns> a flattened representation of the array </returns>
		public static Double() flattenF(Double()() arr)
		If True Then
			Dim ret((arr.length * arr(0).length) - 1) As Double
			Dim count As Integer = 0
			Dim j As Integer = 0
			Do While j < arr(0).length
				For i As Integer = 0 To arr.length - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = arr[i][j];
					ret(count) = arr(i)(j)
						count += 1
				Next i
				j += 1
			Loop
			Return ret
		End If

		public static Single() flattenF(Single()() arr)
		If True Then
			Dim ret((arr.length * arr(0).length) - 1) As Single
			Dim count As Integer = 0
			Dim j As Integer = 0
			Do While j < arr(0).length
				For i As Integer = 0 To arr.length - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = arr[i][j];
					ret(count) = arr(i)(j)
						count += 1
				Next i
				j += 1
			Loop
			Return ret
		End If

		public static Integer() flattenF(Integer()() arr)
		If True Then
			Dim ret((arr.length * arr(0).length) - 1) As Integer
			Dim count As Integer = 0
			Dim j As Integer = 0
			Do While j < arr(0).length
				For i As Integer = 0 To arr.length - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = arr[i][j];
					ret(count) = arr(i)(j)
						count += 1
				Next i
				j += 1
			Loop
			Return ret
		End If


		public static Long() flattenF(Long()() arr)
		If True Then
			Dim ret((arr.length * arr(0).length) - 1) As Long
			Dim count As Integer = 0
			Dim j As Integer = 0
			Do While j < arr(0).length
				For i As Integer = 0 To arr.length - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = arr[i][j];
					ret(count) = arr(i)(j)
						count += 1
				Next i
				j += 1
			Loop
			Return ret
		End If

		public static Integer()() reshapeInt(Integer() [in], Integer rows, Integer cols)
		If True Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim out[][] As Integer = new Integer[rows][cols]
			Dim [out]()() As Integer = RectangularArrays.RectangularIntegerArray(rows, cols)
			Dim x As Integer = 0
			For i As Integer = 0 To rows - 1
				For j As Integer = 0 To cols - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out[i][j] = in[x++];
					[out](i)(j) = [in](x)
						x += 1
				Next j
			Next i
			Return [out]
		End If

		public static Integer()()() reshapeInt(Integer() [in], Integer d0, Integer d1, Integer d2)
		If True Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim out[][][] As Integer = new Integer[d0][d1][d2]
			Dim [out]()()() As Integer = RectangularArrays.RectangularIntegerArray(d0, d1, d2)
			Dim x As Integer = 0
			For i As Integer = 0 To d0 - 1
				For j As Integer = 0 To d1 - 1
					For k As Integer = 0 To d2 - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out[i][j][k] = in[x++];
						[out](i)(j)(k) = [in](x)
							x += 1
					Next k
				Next j
			Next i
			Return [out]
		End If

		public static Double()() reshapeDouble(Double() [in], Integer rows, Integer cols)
		If True Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim out[][] As Double = new Double[rows][cols]
			Dim [out]()() As Double = RectangularArrays.RectangularDoubleArray(rows, cols)
			Dim x As Integer = 0
			For i As Integer = 0 To rows - 1
				For j As Integer = 0 To cols - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out[i][j] = in[x++];
					[out](i)(j) = [in](x)
						x += 1
				Next j
			Next i
			Return [out]
		End If

		public static Double()()() reshapeDouble(Double() [in], Integer d0, Integer d1, Integer d2)
		If True Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim out[][][] As Double = new Double[d0][d1][d2]
			Dim [out]()()() As Double = RectangularArrays.RectangularDoubleArray(d0, d1, d2)
			Dim x As Integer = 0
			For i As Integer = 0 To d0 - 1
				For j As Integer = 0 To d1 - 1
					For k As Integer = 0 To d2 - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out[i][j][k] = in[x++];
						[out](i)(j)(k) = [in](x)
							x += 1
					Next k
				Next j
			Next i
			Return [out]
		End If

		public static Long()() reshapeLong(Long() [in], Integer rows, Integer cols)
		If True Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim out[][] As Long = new Long[rows][cols]
			Dim [out]()() As Long = RectangularArrays.RectangularLongArray(rows, cols)
			Dim x As Integer = 0
			For i As Integer = 0 To rows - 1
				For j As Integer = 0 To cols - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out[i][j] = in[x++];
					[out](i)(j) = [in](x)
						x += 1
				Next j
			Next i
			Return [out]
		End If

		public static Long()()() reshapeLong(Long() [in], Integer d0, Integer d1, Integer d2)
		If True Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim out[][][] As Long = new Long[d0][d1][d2]
			Dim [out]()()() As Long = RectangularArrays.RectangularLongArray(d0, d1, d2)
			Dim x As Integer = 0
			For i As Integer = 0 To d0 - 1
				For j As Integer = 0 To d1 - 1
					For k As Integer = 0 To d2 - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out[i][j][k] = in[x++];
						[out](i)(j)(k) = [in](x)
							x += 1
					Next k
				Next j
			Next i
			Return [out]
		End If

		public static Boolean()() reshapeBoolean(Boolean() [in], Integer rows, Integer cols)
		If True Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim out[][] As Boolean = new Boolean[rows][cols]
			Dim [out]()() As Boolean = RectangularArrays.RectangularBooleanArray(rows, cols)
			Dim x As Integer = 0
			For i As Integer = 0 To rows - 1
				For j As Integer = 0 To cols - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out[i][j] = in[x++];
					[out](i)(j) = [in](x)
						x += 1
				Next j
			Next i
			Return [out]
		End If

		public static Boolean()()() reshapeBoolean(Boolean() [in], Integer d0, Integer d1, Integer d2)
		If True Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim out[][][] As Boolean = new Boolean[d0][d1][d2]
			Dim [out]()()() As Boolean = RectangularArrays.RectangularBooleanArray(d0, d1, d2)
			Dim x As Integer = 0
			For i As Integer = 0 To d0 - 1
				For j As Integer = 0 To d1 - 1
					For k As Integer = 0 To d2 - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out[i][j][k] = in[x++];
						[out](i)(j)(k) = [in](x)
							x += 1
					Next k
				Next j
			Next i
			Return [out]
		End If

		public static (Of T) T()() reshapeObject(T() [in], Integer rows, Integer cols)
		If True Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim out[][] As Object = new Object[rows][cols]
			Dim [out]()() As Object = RectangularArrays.RectangularObjectArray(rows, cols)
			Dim x As Integer = 0
			For i As Integer = 0 To rows - 1
				For j As Integer = 0 To cols - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out[i][j] = in[x++];
					[out](i)(j) = [in](x)
						x += 1
				Next j
			Next i
			Return DirectCast([out], T()())
		End If

		public static (Of T) T()()() reshapeObject(T() [in], Integer d0, Integer d1, Integer d2)
		If True Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim out[][][] As Object = new Object[d0][d1][d2]
			Dim [out]()()() As Object = RectangularArrays.RectangularObjectArray(d0, d1, d2)
			Dim x As Integer = 0
			For i As Integer = 0 To d0 - 1
				For j As Integer = 0 To d1 - 1
					For k As Integer = 0 To d2 - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out[i][j][k] = in[x++];
						[out](i)(j)(k) = [in](x)
							x += 1
					Next k
				Next j
			Next i
			Return DirectCast([out], T()()())
		End If

		''' <summary>
		''' Cast an int array to a double array </summary>
		''' <param name="arr"> the array to cast </param>
		''' <returns> the elements of this
		''' array cast as an int </returns>
		public static Double()() toDouble(Integer()() arr)
		If True Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret[][] As Double = new Double[arr.length][arr[0].length]
			Dim ret()() As Double = RectangularArrays.RectangularDoubleArray(arr.length, arr(0).length)
			For i As Integer = 0 To arr.length - 1
				Dim j As Integer = 0
				Do While j < arr(i).length
					ret(i)(j) = arr(i)(j)
					j += 1
				Loop
			Next i
			Return ret
		End If



		''' <summary>
		''' Combines a applyTransformToDestination of int arrays in to one flat int array
		''' </summary>
		''' <param name="nums"> the int arrays to combineDouble </param>
		''' <returns> one combined int array </returns>
		public static Single() combineFloat(IList(Of Single()) nums)
		If True Then
			Dim length As Integer = 0
			For i As Integer = 0 To nums.size() - 1
				length += nums.get(i).length
			Next i
			Dim ret(length - 1) As Single
			Dim count As Integer = 0
			For Each i As Single() In nums
				For j As Integer = 0 To i.Length - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = i[j];
					ret(count) = i(j)
						count += 1
				Next j
			Next i

			Return ret
		End If


		''' <summary>
		''' Combines a apply of int arrays in to one flat int array
		''' </summary>
		''' <param name="nums"> the int arrays to combineDouble </param>
		''' <returns> one combined int array </returns>
		public static Single() combine(IList(Of Single()) nums)
		If True Then
			Dim length As Integer = 0
			For i As Integer = 0 To nums.size() - 1
				length += nums.get(i).length
			Next i
			Dim ret(length - 1) As Single
			Dim count As Integer = 0
			For Each i As Single() In nums
				For j As Integer = 0 To i.Length - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = i[j];
					ret(count) = i(j)
						count += 1
				Next j
			Next i

			Return ret
		End If

		''' <summary>
		''' Combines a apply of int arrays in to one flat int array
		''' </summary>
		''' <param name="nums"> the int arrays to combineDouble </param>
		''' <returns> one combined int array </returns>
		public static Double() combineDouble(IList(Of Double()) nums)
		If True Then
			Dim length As Integer = 0
			For i As Integer = 0 To nums.size() - 1
				length += nums.get(i).length
			Next i
			Dim ret(length - 1) As Double
			Dim count As Integer = 0
			For Each i As Double() In nums
				For j As Integer = 0 To i.Length - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = i[j];
					ret(count) = i(j)
						count += 1
				Next j
			Next i

			Return ret
		End If

		''' <summary>
		''' Combines a apply of int arrays in to one flat int array
		''' </summary>
		''' <param name="ints"> the int arrays to combineDouble </param>
		''' <returns> one combined int array </returns>
		public static Double() combine(Single()... ints)
		If True Then
			Dim length As Integer = 0
			For i As Integer = 0 To ints.length - 1
				length += ints(i).length
			Next i
			Dim ret(length - 1) As Double
			Dim count As Integer = 0
			For Each i As Single() In ints
				For j As Integer = 0 To i.Length - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = i[j];
					ret(count) = i(j)
						count += 1
				Next j
			Next i

			Return ret
		End If

		''' <summary>
		''' Combines a apply of int arrays in to one flat int array
		''' </summary>
		''' <param name="ints"> the int arrays to combineDouble </param>
		''' <returns> one combined int array </returns>
		public static Integer() combine(Integer()... ints)
		If True Then
			Dim length As Integer = 0
			For i As Integer = 0 To ints.length - 1
				length += ints(i).length
			Next i
			Dim ret(length - 1) As Integer
			Dim count As Integer = 0
			For Each i As Integer() In ints
				For j As Integer = 0 To i.Length - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = i[j];
					ret(count) = i(j)
						count += 1
				Next j
			Next i

			Return ret
		End If

		''' <summary>
		''' Combines a apply of long arrays in to one flat long array
		''' </summary>
		''' <param name="ints"> the int arrays to combineDouble </param>
		''' <returns> one combined int array </returns>
		public static Long() combine(Long()... ints)
		If True Then
			Dim length As Integer = 0
			For i As Integer = 0 To ints.length - 1
				length += ints(i).length
			Next i
			Dim ret(length - 1) As Long
			Dim count As Integer = 0
			For Each i As Long() In ints
				For j As Integer = 0 To i.Length - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = i[j];
					ret(count) = i(j)
						count += 1
				Next j
			Next i

			Return ret
		End If


		public static (Of E) E() combine(E()... arrs)
		If True Then
			Dim length As Integer = 0
			For i As Integer = 0 To arrs.length - 1
				length += arrs(i).length
			Next i

			Dim ret() As E = CType(Array.CreateInstance(arrs(0)(0).GetType(), length), E())
			Dim count As Integer = 0
			For Each i As E() In arrs
				For j As Integer = 0 To i.Length - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = i[j];
					ret(count) = i(j)
						count += 1
				Next j
			Next i

			Return ret
		End If


		public static Integer() toOutcomeArray(Integer outcome, Integer numOutcomes)
		If True Then
			Dim nums(numOutcomes - 1) As Integer
			nums(outcome) = 1
			Return nums
		End If

		public static Double() toDouble(Integer() data)
		If True Then
			Dim ret(data.length - 1) As Double
			For i As Integer = 0 To ret.Length - 1
				ret(i) = data(i)
			Next i
			Return ret
		End If

		public static Double() toDouble(Long() data)
		If True Then
			Dim ret(data.length - 1) As Double
			For i As Integer = 0 To ret.Length - 1
				ret(i) = data(i)
			Next i
			Return ret
		End If

		public static Single() copy(Single() data)
		If True Then
			Dim result(data.length - 1) As Single
			Array.Copy(data, 0, result, 0, data.length)
			Return result
		End If

		public static Double() copy(Double() data)
		If True Then
			Dim result(data.length - 1) As Double
			Array.Copy(data, 0, result, 0, data.length)
			Return result
		End If


		''' <summary>
		''' Convert an arbitrary-dimensional rectangular double array to flat vector.<br>
		''' Can pass double[], double[][], double[][][], etc.
		''' </summary>
		public static Double() flattenDoubleArray(Object doubleArray)
		If True Then
			If TypeOf doubleArray Is Double() Then
				Return CType(doubleArray, Double())
			End If

			Dim stack As New LinkedList(Of Object)()
			stack.AddFirst(doubleArray)

			Dim shape() As Integer = arrayShape(doubleArray)
			Dim length As Integer = ArrayUtil.prod(shape)
			Dim flat(length - 1) As Double
			Dim count As Integer = 0

			Do While stack.Count > 0
				Dim current As Object = stack.RemoveFirst()
				If TypeOf current Is Double() Then
					Dim arr() As Double = DirectCast(current, Double())
					For i As Integer = 0 To arr.Length - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: flat[count++] = arr[i];
						flat(count) = arr(i)
							count += 1
					Next i
				ElseIf TypeOf current Is Object() Then
					Dim o() As Object = DirectCast(current, Object())
					For i As Integer = o.Length - 1 To 0 Step -1
						stack.AddFirst(o(i))
					Next i
				Else
					Throw New System.ArgumentException("Base array is not double[]")
				End If
			Loop

			If count <> flat.Length Then
				Throw New System.ArgumentException("Fewer elements than expected. Array is ragged?")
			End If
			Return flat
		End If

		''' <summary>
		''' Convert an arbitrary-dimensional rectangular float array to flat vector.<br>
		''' Can pass float[], float[][], float[][][], etc.
		''' </summary>
		public static Single() flattenFloatArray(Object floatArray)
		If True Then
			If TypeOf floatArray Is Single() Then
				Return CType(floatArray, Single())
			End If

			Dim stack As New LinkedList(Of Object)()
			stack.AddFirst(floatArray)

			Dim shape() As Integer = arrayShape(floatArray)
			Dim length As Integer = ArrayUtil.prod(shape)
			Dim flat(length - 1) As Single
			Dim count As Integer = 0

			Do While stack.Count > 0
				Dim current As Object = stack.RemoveFirst()
				If TypeOf current Is Single() Then
					Dim arr() As Single = DirectCast(current, Single())
					For i As Integer = 0 To arr.Length - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: flat[count++] = arr[i];
						flat(count) = arr(i)
							count += 1
					Next i
				ElseIf TypeOf current Is Object() Then
					Dim o() As Object = DirectCast(current, Object())
					For i As Integer = o.Length - 1 To 0 Step -1
						stack.AddFirst(o(i))
					Next i
				Else
					Throw New System.ArgumentException("Base array is not float[]")
				End If
			Loop

			If count <> flat.Length Then
				Throw New System.ArgumentException("Fewer elements than expected. Array is ragged?")
			End If
			Return flat
		End If

		''' <summary>
		''' Calculate the shape of an arbitrary multi-dimensional array. Assumes:<br>
		''' (a) array is rectangular (not ragged) and first elements (i.e., array[0][0][0]...) are non-null <br>
		''' (b) First elements have > 0 length. So array[0].length > 0, array[0][0].length > 0, etc.<br>
		''' Can pass any Java array opType: double[], Object[][][], float[][], etc.<br>
		''' Length of returned array is number of dimensions; returned[i] is size of ith dimension.
		''' </summary>
		public static Integer() arrayShape(Object array)
		If True Then
			Return arrayShape(array, False)
		End If

		''' <summary>
		''' Calculate the shape of an arbitrary multi-dimensional array.<br>
		''' Note that the method assumes the array is rectangular (not ragged) and first elements (i.e., array[0][0][0]...) are non-null <br>
		''' Note also that if allowSize0Dims is true, any elements are length 0, all subsequent dimensions will be reported as 0.
		''' i.e., a double[3][0][2] would be reported as shape [3,0,0]. If allowSize0Dims is false, an exception will be thrown for this case instead.
		''' Can pass any Java array opType: double[], Object[][][], float[][], etc.<br>
		''' Length of returned array is number of dimensions; returned[i] is size of ith dimension.
		''' </summary>
		public static Integer() arrayShape(Object array, Boolean allowSize0Dims)
		If True Then
			Dim nDimensions As Integer = 0
			Dim c As Type = array.GetType().GetElementType()
			Do While c IsNot Nothing
				nDimensions += 1
				c = c.GetElementType()
			Loop

			Dim shape(nDimensions - 1) As Integer
			Dim current As Object = array
			For i As Integer = 0 To shape.Length - 2
				shape(i) = DirectCast(current, Object()).Length
				If shape(i) = 0 Then
					If allowSize0Dims Then
						Return shape
					End If
					Throw New System.InvalidOperationException("Cannot calculate array shape: Array has size 0 for dimension " & i)
				End If
				current = DirectCast(current, Object())(0)
			Next i

			If TypeOf current Is Object() Then
				shape(shape.Length - 1) = DirectCast(current, Object()).Length
			ElseIf TypeOf current Is Double() Then
				shape(shape.Length - 1) = DirectCast(current, Double()).Length
			ElseIf TypeOf current Is Single() Then
				shape(shape.Length - 1) = DirectCast(current, Single()).Length
			ElseIf TypeOf current Is Long() Then
				shape(shape.Length - 1) = DirectCast(current, Long()).Length
			ElseIf TypeOf current Is Integer() Then
				shape(shape.Length - 1) = DirectCast(current, Integer()).Length
			ElseIf TypeOf current Is SByte() Then
				shape(shape.Length - 1) = DirectCast(current, SByte()).Length
			ElseIf TypeOf current Is Char() Then
				shape(shape.Length - 1) = DirectCast(current, Char()).Length
			ElseIf TypeOf current Is Boolean() Then
				shape(shape.Length - 1) = DirectCast(current, Boolean()).Length
			ElseIf TypeOf current Is Short() Then
				shape(shape.Length - 1) = DirectCast(current, Short()).Length
			Else
				Throw New System.InvalidOperationException("Unknown array type") 'Should never happen
			End If
			Return shape
		End If


		''' <summary>
		''' Returns the maximum value in the array </summary>
		public static Integer max(Integer() [in])
		If True Then
			Dim max As Integer = Integer.MinValue
			For i As Integer = 0 To [in].length - 1
				If [in](i) > max Then
					max = [in](i)
				End If
			Next i
			Return max
		End If

		''' <summary>
		''' Returns the minimum value in the array </summary>
		public static Integer min(Integer() [in])
		If True Then
			Dim min As Integer = Integer.MaxValue
			For i As Integer = 0 To [in].length - 1
				If [in](i) < min Then
					min = [in](i)
				End If
			Next i
			Return min
		End If

		''' <summary>
		''' Returns the index of the maximum value in the array.
		''' If two entries have same maximum value, index of the first one is returned. 
		''' </summary>
		public static Integer argMax(Integer() [in])
		If True Then
			Dim maxIdx As Integer = 0
			For i As Integer = 1 To [in].length - 1
				If [in](i) > [in](maxIdx) Then
					maxIdx = i
				End If
			Next i
			Return maxIdx
		End If

		''' <summary>
		''' Returns the index of the minimum value in the array.
		''' If two entries have same minimum value, index of the first one is returned. 
		''' </summary>
		public static Integer argMin(Integer() [in])
		If True Then
			Dim minIdx As Integer = 0
			For i As Integer = 1 To [in].length - 1
				If [in](i) < [in](minIdx) Then
					minIdx = i
				End If
			Next i
			Return minIdx
		End If

		''' <summary>
		''' Returns the index of the maximum value in the array.
		''' If two entries have same maximum value, index of the first one is returned. 
		''' </summary>
		public static Integer argMax(Long() [in])
		If True Then
			Dim maxIdx As Integer = 0
			For i As Integer = 1 To [in].length - 1
				If [in](i) > [in](maxIdx) Then
					maxIdx = i
				End If
			Next i
			Return maxIdx
		End If

		''' <summary>
		''' Returns the index of the minimum value in the array.
		''' If two entries have same minimum value, index of the first one is returned. 
		''' </summary>
		public static Integer argMin(Long() [in])
		If True Then
			Dim minIdx As Integer = 0
			For i As Integer = 1 To [in].length - 1
				If [in](i) < [in](minIdx) Then
					minIdx = i
				End If
			Next i
			Return minIdx
		End If

		''' 
		''' <summary>
		''' @return
		''' </summary>
		public static Integer() buildHalfVector(Random rng, Integer length)
		If True Then
			Dim result(length - 1) As Integer
			Dim indexes As IList(Of Integer) = New List(Of Integer)()

			' we add indexes from second half only
			For i As Integer = result.Length - 1 To result.Length \ 2 Step -1
				indexes.Add(i)
			Next i

			Collections.shuffle(indexes, rng)

			For i As Integer = 0 To result.Length - 1
				If i < result.Length \ 2 Then
					result(i) = indexes(0)
					indexes.RemoveAt(0)
				Else
					result(i) = -1
				End If
			Next i

			Return result
		End If

		public static Integer() buildInterleavedVector(Random rng, Integer length)
		If True Then
			Dim result(length - 1) As Integer

			Dim indexes As IList(Of Integer) = New List(Of Integer)()
			Dim odds As IList(Of Integer) = New List(Of Integer)()

			' we add odd indexes only to list
			For i As Integer = 1 To result.Length - 1 Step 2
				indexes.Add(i)
				odds.Add(i - 1)
			Next i

			Collections.shuffle(indexes, rng)

			' now all even elements will be interleaved with odd elements
			For i As Integer = 0 To result.Length - 1
				If i Mod 2 = 0 AndAlso indexes.Count > 0 Then
					Dim idx As Integer = indexes(0)
					indexes.RemoveAt(0)
					result(i) = idx
				Else
					result(i) = -1
				End If
			Next i

			' for odd tad numbers, we add special random clause for last element
			If length Mod 2 <> 0 Then
				Dim rndClause As Integer = odds(rng.nextInt(odds.Count))
				Dim tmp As Integer = result(rndClause)
				result(rndClause) = result(result.Length - 1)
				result(result.Length - 1) = tmp
			End If


			Return result
		End If

		public static Long() buildInterleavedVector(Random rng, Long length)
		If True Then
			If length > Integer.MaxValue Then
				Throw New Exception("Integer overflow")
			End If
			Dim result As val = New Long(CInt(Math.Truncate(length)) - 1){}

			Dim indexes As IList(Of Integer) = New List(Of Integer)()
			Dim odds As IList(Of Integer) = New List(Of Integer)()

			' we add odd indexes only to list
			For i As Integer = 1 To result.length - 1 Step 2
				indexes.Add(i)
				odds.Add(i - 1)
			Next i

			Collections.shuffle(indexes, rng)

			' now all even elements will be interleaved with odd elements
			For i As Integer = 0 To result.length - 1
				If i Mod 2 = 0 AndAlso indexes.Count > 0 Then
					Dim idx As Integer = indexes(0)
					indexes.RemoveAt(0)
					result(i) = idx
				Else
					result(i) = -1
				End If
			Next i

			' for odd tad numbers, we add special random clause for last element
			If length Mod 2 <> 0 Then
				Dim rndClause As Integer = odds(rng.nextInt(odds.Count))
				Dim tmp As Long = result(rndClause)
				result(rndClause) = result(result.length - 1)
				result(result.length - 1) = tmp
			End If


			Return result
		End If

		protected static (Of T As Object) void swap(IList(Of T) objects, Integer idxA, Integer idxB)
		If True Then
			Dim tmpA As T = objects.get(idxA)
			Dim tmpB As T = objects.get(idxB)
			objects.set(idxA, tmpB)
			objects.set(idxB, tmpA)
		End If

		public static (Of T As Object) void shuffleWithMap(IList(Of T) objects, Integer() map)
		If True Then
			For i As Integer = 0 To map.length - 1
				If map(i) >= 0 Then
					swap(objects, i, map(i))
				End If
			Next i
		End If

		public static Integer argMinOfMax(Integer() first, Integer() second)
		If True Then
			Dim minIdx As Integer = 0
			Dim maxAtMinIdx As Integer = Math.Max(first(0), second(0))
			For i As Integer = 1 To first.length - 1
				Dim maxAtIndex As Integer = Math.Max(first(i), second(i))
				If maxAtMinIdx > maxAtIndex Then
					maxAtMinIdx = maxAtIndex
					minIdx = i
				End If
			Next i
			Return minIdx
		End If

		public static Long argMinOfMax(Long() first, Long() second)
		If True Then
			Dim minIdx As Long = 0
			Dim maxAtMinIdx As Long = Math.Max(first(0), second(0))
			For i As Integer = 1 To first.length - 1
				Dim maxAtIndex As Long = Math.Max(first(i), second(i))
				If maxAtMinIdx > maxAtIndex Then
					maxAtMinIdx = maxAtIndex
					minIdx = i
				End If
			Next i
			Return minIdx
		End If

		public static Integer argMinOfMax(Integer()... arrays)
		If True Then
			Dim minIdx As Integer = 0
			Dim maxAtMinIdx As Integer = Integer.MaxValue

			Dim i As Integer = 0
			Do While i < arrays(0).length
				Dim maxAtIndex As Integer = Integer.MinValue
				For j As Integer = 0 To arrays.length - 1
					maxAtIndex = Math.Max(maxAtIndex, arrays(j)(i))
				Next j

				If maxAtMinIdx > maxAtIndex Then
					maxAtMinIdx = maxAtIndex
					minIdx = i
				End If
				i += 1
			Loop
			Return minIdx
		End If

		public static Long argMinOfMax(Long()... arrays)
		If True Then
			Dim minIdx As Integer = 0
			Dim maxAtMinIdx As Long = Long.MaxValue

			Dim i As Integer = 0
			Do While i < arrays(0).length
				Dim maxAtIndex As Long = Long.MinValue
				For j As Integer = 0 To arrays.length - 1
					maxAtIndex = Math.Max(maxAtIndex, arrays(j)(i))
				Next j

				If maxAtMinIdx > maxAtIndex Then
					maxAtMinIdx = maxAtIndex
					minIdx = i
				End If
				i += 1
			Loop
			Return minIdx
		End If

		public static Integer argMinOfSum(Integer() first, Integer() second)
		If True Then
			Dim minIdx As Integer = 0
			Dim sumAtMinIdx As Integer = first(0) + second(0)
			For i As Integer = 1 To first.length - 1
				Dim sumAtIndex As Integer = first(i) + second(i)
				If sumAtMinIdx > sumAtIndex Then
					sumAtMinIdx = sumAtIndex
					minIdx = i
				End If
			Next i
			Return minIdx
		End If

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to the Java 'super' constraint:
'ORIGINAL LINE: public static <K, V extends Comparable<? super V>> Map<K, V> sortMapByValue(Map<K, V> map)
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
		public static (Of K, V As IComparable(Of Object)) IDictionary(Of K, V) sortMapByValue(IDictionary(Of K, V) map)
		If True Then
			Dim list As IList(Of KeyValuePair(Of K, V)) = New LinkedList(Of KeyValuePair(Of K, V))(map.entrySet())
			list.Sort(New ComparatorAnonymousInnerClass2(Me))

			Dim result As IDictionary(Of K, V) = New LinkedHashMap(Of K, V)()
			For Each entry As KeyValuePair(Of K, V) In list
				result(entry.Key) = entry.Value
			Next entry
			Return result
		End If


		public static (Of T) T getRandomElement(IList(Of T) list)
		If True Then
			If list.isEmpty() Then
				Return Nothing
			End If

			Return list.get(RandomUtils.nextInt(0, list.size()))
		End If

		''' <summary>
		''' Convert an int </summary>
		''' <param name="bool">
		''' @return </param>
		public static Integer fromBoolean(Boolean bool)
		If True Then
			Return If(bool, 1, 0)
		End If

		public static Long() toPrimitives(Long?() array)
		If True Then
			Dim res As val = New Long(array.length - 1){}
			For e As Integer = 0 To array.length - 1
				res(e) = array(e)
			Next e

			Return res
		End If

		public static Integer() toPrimitives(Integer?() array)
		If True Then
			Dim res As val = New Integer(array.length - 1){}
			For e As Integer = 0 To array.length - 1
				res(e) = array(e)
			Next e

			Return res
		End If

		public static Short() toPrimitives(Short?() array)
		If True Then
			Dim res As val = New Short(array.length - 1){}
			For e As Integer = 0 To array.length - 1
				res(e) = array(e)
			Next e

			Return res
		End If

		public static SByte() toPrimitives(SByte?() array)
		If True Then
			Dim res As val = New SByte(array.length - 1){}
			For e As Integer = 0 To array.length - 1
				res(e) = array(e)
			Next e

			Return res
		End If

		public static Single() toPrimitives(Single?() array)
		If True Then
			Dim res As val = New Single(array.length - 1){}
			For e As Integer = 0 To array.length - 1
				res(e) = array(e)
			Next e

			Return res
		End If

		public static Double() toPrimitives(Double?() array)
		If True Then
			Dim res As val = New Double(array.length - 1){}
			For e As Integer = 0 To array.length - 1
				res(e) = array(e)
			Next e

			Return res
		End If

		public static Boolean() toPrimitives(Boolean?() array)
		If True Then
			Dim res As val = New Boolean(array.length - 1){}
			For e As Integer = 0 To array.length - 1
				res(e) = array(e)
			Next e

			Return res
		End If

		public static Long()() toPrimitives(Long?()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new Long[array.length][array[0].length]
			Dim res As val = RectangularArrays.RectangularLongArray(array.length, array(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					res(i)(j) = array(i)(j)
					j += 1
				Loop
			Next i

			Return res
		End If

		public static Integer()() toPrimitives(Integer?()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new Integer[array.length][array[0].length]
			Dim res As val = RectangularArrays.RectangularIntegerArray(array.length, array(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					res(i)(j) = array(i)(j)
					j += 1
				Loop
			Next i

			Return res
		End If

		public static Short()() toPrimitives(Short?()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new Short[array.length][array[0].length]
			Dim res As val = RectangularArrays.RectangularShortArray(array.length, array(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					res(i)(j) = array(i)(j)
					j += 1
				Loop
			Next i

			Return res
		End If

		public static SByte()() toPrimitives(SByte?()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new SByte[array.length][array[0].length]
			Dim res As val = RectangularArrays.RectangularSByteArray(array.length, array(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					res(i)(j) = array(i)(j)
					j += 1
				Loop
			Next i

			Return res
		End If

		public static Double()() toPrimitives(Double?()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new Double[array.length][array[0].length]
			Dim res As val = RectangularArrays.RectangularDoubleArray(array.length, array(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					res(i)(j) = array(i)(j)
					j += 1
				Loop
			Next i

			Return res
		End If

		public static Single()() toPrimitives(Single?()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new Single[array.length][array[0].length]
			Dim res As val = RectangularArrays.RectangularSingleArray(array.length, array(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					res(i)(j) = array(i)(j)
					j += 1
				Loop
			Next i

			Return res
		End If

		public static Boolean ()() toPrimitives(Boolean?()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new Boolean[array.length][array[0].length]
			Dim res As val = RectangularArrays.RectangularBooleanArray(array.length, array(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					res(i)(j) = array(i)(j)
					j += 1
				Loop
			Next i

			Return res
		End If

		public static Long()()() toPrimitives(Long?()()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new Long[array.length][array[0].length][array[0][0].length]
			Dim res As val = RectangularArrays.RectangularLongArray(array.length, array(0).length, array(0)(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					Dim k As Integer = 0
					Do While j < array(0)(0).length
						res(i)(j)(k) = array(i)(j)(k)
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return res
		End If

		public static Integer()()() toPrimitives(Integer?()()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new Integer[array.length][array[0].length][array[0][0].length]
			Dim res As val = RectangularArrays.RectangularIntegerArray(array.length, array(0).length, array(0)(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					Dim k As Integer = 0
					Do While j < array(0)(0).length
						res(i)(j)(k) = array(i)(j)(k)
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return res
		End If

		public static Short()()() toPrimitives(Short?()()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new Short[array.length][array[0].length][array[0][0].length]
			Dim res As val = RectangularArrays.RectangularShortArray(array.length, array(0).length, array(0)(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					Dim k As Integer = 0
					Do While j < array(0)(0).length
						res(i)(j)(k) = array(i)(j)(k)
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return res
		End If

		public static SByte()()() toPrimitives(SByte?()()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new SByte[array.length][array[0].length][array[0][0].length]
			Dim res As val = RectangularArrays.RectangularSByteArray(array.length, array(0).length, array(0)(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					Dim k As Integer = 0
					Do While j < array(0)(0).length
						res(i)(j)(k) = array(i)(j)(k)
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return res
		End If

		public static Double()()() toPrimitives(Double?()()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new Double[array.length][array[0].length][array[0][0].length]
			Dim res As val = RectangularArrays.RectangularDoubleArray(array.length, array(0).length, array(0)(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					Dim k As Integer = 0
					Do While j < array(0)(0).length
						res(i)(j)(k) = array(i)(j)(k)
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return res
		End If

		public static Single()()() toPrimitives(Single?()()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new Single[array.length][array[0].length][array[0][0].length]
			Dim res As val = RectangularArrays.RectangularSingleArray(array.length, array(0).length, array(0)(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					Dim k As Integer = 0
					Do While j < array(0)(0).length
						res(i)(j)(k) = array(i)(j)(k)
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return res
		End If

		public static Boolean()()() toPrimitives(Boolean?()()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new Boolean[array.length][array[0].length][array[0][0].length]
			Dim res As val = RectangularArrays.RectangularBooleanArray(array.length, array(0).length, array(0)(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					Dim k As Integer = 0
					Do While j < array(0)(0).length
						res(i)(j)(k) = array(i)(j)(k)
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return res
		End If

		public static Long()()()() toPrimitives(Long?()()()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new Long[array.length][array[0].length][array[0][0].length][array[0][0][0].length]
			Dim res As val = RectangularArrays.RectangularLongArray(array.length, array(0).length, array(0)(0).length, array(0)(0)(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					Dim k As Integer = 0
					Do While j < array(0)(0).length
						Dim l As Integer = 0
						Do While l < array(0)(0)(0).length
							res(i)(j)(k)(l) = array(i)(j)(k)(l)
							l += 1
						Loop
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return res
		End If

		public static Integer()()()() toPrimitives(Integer?()()()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new Integer[array.length][array[0].length][array[0][0].length][array[0][0][0].length]
			Dim res As val = RectangularArrays.RectangularIntegerArray(array.length, array(0).length, array(0)(0).length, array(0)(0)(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					Dim k As Integer = 0
					Do While j < array(0)(0).length
						Dim l As Integer = 0
						Do While l < array(0)(0)(0).length
							res(i)(j)(k)(l) = array(i)(j)(k)(l)
							l += 1
						Loop
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return res
		End If

		public static Short()()()() toPrimitives(Short?()()()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new Short[array.length][array[0].length][array[0][0].length][array[0][0][0].length]
			Dim res As val = RectangularArrays.RectangularShortArray(array.length, array(0).length, array(0)(0).length, array(0)(0)(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					Dim k As Integer = 0
					Do While j < array(0)(0).length
						Dim l As Integer = 0
						Do While l < array(0)(0)(0).length
							res(i)(j)(k)(l) = array(i)(j)(k)(l)
							l += 1
						Loop
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return res
		End If

		public static SByte()()()() toPrimitives(SByte?()()()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new SByte[array.length][array[0].length][array[0][0].length][array[0][0][0].length]
			Dim res As val = RectangularArrays.RectangularSByteArray(array.length, array(0).length, array(0)(0).length, array(0)(0)(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					Dim k As Integer = 0
					Do While j < array(0)(0).length
						Dim l As Integer = 0
						Do While l < array(0)(0)(0).length
							res(i)(j)(k)(l) = array(i)(j)(k)(l)
							l += 1
						Loop
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return res
		End If

		public static Double()()()() toPrimitives(Double?()()()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new Double[array.length][array[0].length][array[0][0].length][array[0][0][0].length]
			Dim res As val = RectangularArrays.RectangularDoubleArray(array.length, array(0).length, array(0)(0).length, array(0)(0)(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					Dim k As Integer = 0
					Do While j < array(0)(0).length
						Dim l As Integer = 0
						Do While l < array(0)(0)(0).length
							res(i)(j)(k)(l) = array(i)(j)(k)(l)
							l += 1
						Loop
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return res
		End If

		public static Single()()()() toPrimitives(Single?()()()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new Single[array.length][array[0].length][array[0][0].length][array[0][0][0].length]
			Dim res As val = RectangularArrays.RectangularSingleArray(array.length, array(0).length, array(0)(0).length, array(0)(0)(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					Dim k As Integer = 0
					Do While j < array(0)(0).length
						Dim l As Integer = 0
						Do While l < array(0)(0)(0).length
							res(i)(j)(k)(l) = array(i)(j)(k)(l)
							l += 1
						Loop
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return res
		End If

		public static Boolean()()()() toPrimitives(Boolean?()()()() array)
		If True Then
			ArrayUtil.assertNotRagged(array)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim res As val = new Boolean[array.length][array[0].length][array[0][0].length][array[0][0][0].length]
			Dim res As val = RectangularArrays.RectangularBooleanArray(array.length, array(0).length, array(0)(0).length, array(0)(0)(0).length)
			For i As Integer = 0 To array.length - 1
				Dim j As Integer = 0
				Do While j < array(0).length
					Dim k As Integer = 0
					Do While j < array(0)(0).length
						Dim l As Integer = 0
						Do While l < array(0)(0)(0).length
							res(i)(j)(k)(l) = array(i)(j)(k)(l)
							l += 1
						Loop
						k += 1
					Loop
					j += 1
				Loop
			Next i

			Return res
		End If


		''' <summary>
		''' Assert that the specified array is not ragged (i.e., is rectangular).<br>
		''' Can be used to check Object arrays with any number of dimensions (up to rank 4), or primitive arrays with rank 2 or higher<br>
		''' An IllegalStateException is thrown if the array is ragged
		''' </summary>
		''' <param name="array"> Array to check </param>
		public static (Of T) void assertNotRagged(T() array)
		If True Then
			Dim c As Type = array.GetType().GetElementType()
			Dim arrayShape() As Integer = ArrayUtil.arrayShape(array, True)
			Dim rank As Integer = arrayShape.Length

			If rank = 1 Then
				'Rank 1 cannot be ragged
				Return
			End If

			If rank >= 2 Then
				Dim i As Integer=1
				Do While i<arrayShape(0)
					Dim subArray As Object = array(i)
					Dim len As Integer = arrayLength(subArray)
					Preconditions.checkState(arrayShape(1) = len, "Ragged array detected: array[0].length=%s does not match array[%s].length=%s", arrayShape(1), i, len)
					i += 1
				Loop
				If rank = 2 Then
					Return
				End If
			End If
			If rank >= 3 Then

				Dim i As Integer=0
				Do While i<arrayShape(0)
					Dim j As Integer=0
					Do While j<arrayShape(1)
						Dim subArray As Object = CType(array, Object()())(i)(j)
						Dim len As Integer = arrayLength(subArray)
						Preconditions.checkState(arrayShape(2) = len, "Ragged array detected: array[0][0].length=%s does not match array[%s][%s].length=%s", arrayShape(2), i, j, len)
						j += 1
					Loop
					i += 1
				Loop

				If rank = 3 Then
					Return
				End If
			End If
			If rank >= 4 Then
				Dim i As Integer=0
				Do While i<arrayShape(0)
					Dim j As Integer=0
					Do While j<arrayShape(1)
						Dim k As Integer=0
						Do While k<arrayShape(2)
							Dim subArray As Object = CType(array, Object()()())(i)(j)(k)
							Dim len As Integer = arrayLength(subArray)
							Preconditions.checkState(arrayShape(3) = len, "Ragged array detected: array[0][0][0].length=%s does not match array[%s][%s][%s].length=%s", arrayShape(3), i, j, k, len)
							k += 1
						Loop
						j += 1
					Loop
					i += 1
				Loop
			End If
		End If

		''' <summary>
		''' Calculate the length of the object or primitive array. If </summary>
		''' <param name="current">
		''' @return </param>
		public static Integer arrayLength(Object current)
		If True Then
			If TypeOf current Is Object() Then
				Return CType(current, Object()).Length
			ElseIf TypeOf current Is Double() Then
				Return CType(current, Double()).Length
			ElseIf TypeOf current Is Single() Then
				Return CType(current, Single()).Length
			ElseIf TypeOf current Is Long() Then
				Return CType(current, Long()).Length
			ElseIf TypeOf current Is Integer() Then
				Return CType(current, Integer()).Length
			ElseIf TypeOf current Is SByte() Then
				Return CType(current, SByte()).Length
			ElseIf TypeOf current Is Char() Then
				Return CType(current, Char()).Length
			ElseIf TypeOf current Is Boolean() Then
				Return CType(current, Boolean()).Length
			ElseIf TypeOf current Is Short() Then
				Return CType(current, Short()).Length
			Else
				Throw New System.InvalidOperationException("Unknown array type (or not an array): " & current.GetType()) 'Should never happen
			End If
		End If

		''' <summary>
		''' Compute the inverse permutation indices for a permutation operation<br>
		''' Example: if input is [2, 0, 1] then output is [1, 2, 0]<br>
		''' The idea is that x.permute(input).permute(invertPermutation(input)) == x
		''' </summary>
		''' <param name="input"> 1D indices for permutation </param>
		''' <returns> 1D inverted permutation </returns>
		public static Integer() invertPermutation(Integer... input)
		If True Then
			Dim target(input.length - 1) As Integer

			For i As Integer = 0 To input.length - 1
				target(input(i)) = i
			Next i

			Return target
		End If

		''' <seealso cref= #invertPermutation(int...)
		''' </seealso>
		''' <param name="input"> 1D indices for permutation </param>
		''' <returns> 1D inverted permutation </returns>
		public static Long() invertPermutation(Long... input)
		If True Then
			Dim target(input.length - 1) As Long

			For i As Integer = 0 To input.length - 1
				target(CInt(Math.Truncate(input(i)))) = i
			Next i

			Return target
		End If

		''' <summary>
		''' Is this shape an empty shape?
		''' Shape is considered to be an empty shape if it contains any zeros.
		''' Note: a length 0 shape is NOT considered empty (it's rank 0 scalar) </summary>
		''' <param name="shape"> Shape to check </param>
		''' <returns> True if shape contains zeros </returns>
		public static Boolean isEmptyShape(Long() shape)
		If True Then
			For Each l As Long In shape
				If l = 0 Then
					Return True
				End If
			Next l
			Return False
		End If

		''' <summary>
		''' Is this shape an empty shape?
		''' Shape is considered to be an empty shape if it contains any zeros.
		''' Note: a length 0 shape is NOT considered empty (it's rank 0 scalar) </summary>
		''' <param name="shape"> Shape to check </param>
		''' <returns> True if shape contains zeros </returns>
		public static Boolean isEmptyShape(Integer() shape)
		If True Then
			For Each i As Integer In shape
				If i = 0 Then
					Return True
				End If
			Next i
			Return False
		End If

		public static (Of T) T() filterNull(T... [in])
		If True Then
			Dim count As Integer = 0
			For i As Integer = 0 To [in].length - 1
				If [in](i) IsNot Nothing Then
					count += 1
				End If
			Next i
			Dim [out]() As T = CType(Array.CreateInstance([in].GetType().GetElementType(), count), T())
			Dim j As Integer=0
			For i As Integer = 0 To [in].length - 1
				If [in](i) IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out[j++] = in[i];
					[out](j) = [in](i)
						j += 1
				End If
			Next i
			Return [out]
		End If
	End Class

End Namespace