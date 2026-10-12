Imports System
Imports System.Collections.Generic
Imports val = lombok.val
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports org.nd4j.common.primitives
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

Namespace org.nd4j.linalg.checkutil


	Public Class NDArrayCreationUtil
		Private Sub New()
		End Sub

		''' <summary>
		''' Get an array of INDArrays (2d) all with the specified shape. Pair<INDArray,String> returned to aid
		''' debugging: String contains information on how to reproduce the matrix (i.e., which function, and arguments)
		''' Each NDArray in the returned array has been obtained by applying an operation such as transpose, tensorAlongDimension,
		''' etc to an original array.
		''' </summary>
		Public Shared Function getAllTestMatricesWithShape(ByVal ordering As Char, ByVal rows As Integer, ByVal cols As Integer, ByVal seed As Integer, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Dim all As IList(Of Pair(Of INDArray, String)) = New List(Of Pair(Of INDArray, String))()
			Nd4j.Random.setSeed(seed)
			all.Add(New Pair(Of INDArray, String)(Nd4j.linspace(1, rows * cols, rows * cols, dataType).reshape(ordering, rows, cols), "Nd4j..linspace(1,rows * cols,rows * cols).reshape(rows,cols)"))

			all.Add(getTransposedMatrixWithShape(ordering, rows, cols, seed, dataType))

			CType(all, List(Of Pair(Of INDArray, String))).AddRange(getSubMatricesWithShape(ordering, rows, cols, seed, dataType))

			CType(all, List(Of Pair(Of INDArray, String))).AddRange(getTensorAlongDimensionMatricesWithShape(ordering, rows, cols, seed, dataType))

			all.Add(getPermutedWithShape(ordering, rows, cols, seed, dataType))
			all.Add(getReshapedWithShape(ordering, rows, cols, seed, dataType))

			Return all
		End Function


		''' <summary>
		''' Get an array of INDArrays (2d) all with the specified shape. Pair<INDArray,String> returned to aid
		''' debugging: String contains information on how to reproduce the matrix (i.e., which function, and arguments)
		''' Each NDArray in the returned array has been obtained by applying an operation such as transpose, tensorAlongDimension,
		''' etc to an original array.
		''' </summary>
		Public Shared Function getAllTestMatricesWithShape(ByVal rows As Long, ByVal cols As Long, ByVal seed As Long, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Dim all As IList(Of Pair(Of INDArray, String)) = New List(Of Pair(Of INDArray, String))()
			Nd4j.Random.Seed = seed
			all.Add(New Pair(Of INDArray, String)(Nd4j.linspace(1L, rows * cols, rows * cols, dataType).reshape(ChrW(rows), cols), "Nd4j..linspace(1,rows * cols,rows * cols).reshape(rows,cols)"))

			all.Add(getTransposedMatrixWithShape(rows, cols, seed, dataType))

			CType(all, List(Of Pair(Of INDArray, String))).AddRange(getSubMatricesWithShape(rows, cols, seed, dataType))

			CType(all, List(Of Pair(Of INDArray, String))).AddRange(getTensorAlongDimensionMatricesWithShape(rows, cols, seed, dataType))

			all.Add(getPermutedWithShape(rows, cols, seed, dataType))
			all.Add(getReshapedWithShape(rows, cols, seed, dataType))

			Return all
		End Function

		''' <summary>
		''' Test utility to sweep shapes given a rank
		''' Given a rank will generate random test matrices that will cover all cases of a shape with a '1' anywhere in the shape
		''' as well a shape with random ints that are not 0 or 1
		''' eg. rank 2: 1,1; 1,2; 2,1; 2,2; 3,4
		''' Motivated by TADs that often hit bugs when a "1" occurs as the size of a dimension
		''' </summary>
		''' <param name="rank"> any rank including true scalars i.e rank >= 0 </param>
		''' <param name="order"> what order array to return i.e 'c' or 'f' order arrays </param>
		''' <returns> List of arrays and the shapes as strings </returns>
		Public Shared Function getTestMatricesWithVaryingShapes(ByVal rank As Integer, ByVal order As Char, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Dim all As IList(Of Pair(Of INDArray, String)) = New List(Of Pair(Of INDArray, String))()
			If rank = 0 Then
				'scalar
				all.Add(New Pair(Of INDArray, String)(Nd4j.scalar(dataType, Nd4j.rand(dataType, New Integer(){1, 1}).getDouble(0)), "{}"))
				Return all
			End If
			'generate all possible combinations with a 1 and a 2
			Dim maxCount As Integer = CInt(Math.Truncate(Math.Pow(2.0, rank)))
			Dim defaultOnes(rank - 1) As Integer
			Arrays.Fill(defaultOnes, 1)
			'use binary and just add 1
			For i As Integer = 0 To maxCount - 1
				Dim num As Integer = i
				Dim iShape() As Integer = ArrayUtils.clone(defaultOnes)
				Dim b As Integer = 0
				Do While num > 0
					iShape(b) = (num Mod 2) + 1
					b += 1
					num \= 2
				Loop
				all.Add(New Pair(Of INDArray, String)(Nd4j.rand(dataType, order, iShape), ArrayUtils.toString(iShape)))
			Next i
			' add a random shape of correct rank with elements > 2 that is not too big
			Dim aRandomShape(rank - 1) As Integer
			Dim ran As New Random()
			For i As Integer = 0 To rank - 1
				aRandomShape(i) = 2 + ran.Next(6)
			Next i
			all.Add(New Pair(Of INDArray, String)(Nd4j.rand(dataType, order, aRandomShape), ArrayUtils.toString(aRandomShape)))
			Return all
		End Function


		Public Shared Function getTransposedMatrixWithShape(ByVal ordering As Char, ByVal rows As Integer, ByVal cols As Integer, ByVal seed As Integer, ByVal dataType As DataType) As Pair(Of INDArray, String)
			Nd4j.Random.setSeed(seed)
			Dim [out] As INDArray = Nd4j.linspace(1, rows * cols, rows * cols, dataType).reshape(ordering, cols, rows)
			Return New Pair(Of INDArray, String)([out].transpose(), "getTransposedMatrixWithShape(" & rows & "," & cols & "," & seed & ")")
		End Function

		Public Shared Function getTransposedMatrixWithShape(ByVal rows As Long, ByVal cols As Long, ByVal seed As Long, ByVal dataType As DataType) As Pair(Of INDArray, String)
			Nd4j.Random.Seed = seed
			Dim [out] As INDArray = Nd4j.linspace(1, rows * cols, rows * cols, dataType).reshape(ChrW(cols), rows)
			Return New Pair(Of INDArray, String)([out].transpose(), "getTransposedMatrixWithShape(" & rows & "," & cols & "," & seed & ")")
		End Function

		Public Shared Function getSubMatricesWithShape(ByVal rows As Long, ByVal cols As Long, ByVal seed As Long, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Return getSubMatricesWithShape(Nd4j.order(), rows, cols, seed, dataType)
		End Function

		Public Shared Function getSubMatricesWithShape(ByVal ordering As Char, ByVal rows As Long, ByVal cols As Long, ByVal seed As Long, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			'Create 3 identical matrices. Could do get() on single original array, but in-place modifications on one
			'might mess up tests for another
			Nd4j.Random.Seed = seed
			Dim shape() As Long = {2 * rows + 4, 2 * cols + 4}
			Dim len As Integer = ArrayUtil.prod(shape)
			Dim orig As INDArray = Nd4j.linspace(1, len, len, dataType).reshape(ordering, shape)
			Dim first As INDArray = orig.get(NDArrayIndex.interval(0, rows), NDArrayIndex.interval(0, cols))
			Nd4j.Random.Seed = seed
			orig = Nd4j.linspace(1, len, len, dataType).reshape(shape)
			Dim second As INDArray = orig.get(NDArrayIndex.interval(3, rows + 3), NDArrayIndex.interval(3, cols + 3))
			Nd4j.Random.Seed = seed
			orig = Nd4j.linspace(1, len, len, dataType).reshape(ordering, shape)
			Dim third As INDArray = orig.get(NDArrayIndex.interval(rows, 2 * rows), NDArrayIndex.interval(cols, 2 * cols))

			Dim baseMsg As String = "getSubMatricesWithShape(" & rows & "," & cols & "," & seed & ")"
			Dim list As IList(Of Pair(Of INDArray, String)) = New List(Of Pair(Of INDArray, String))(3)
			list.Add(New Pair(Of INDArray, String)(first, baseMsg & ".get(0)"))
			list.Add(New Pair(Of INDArray, String)(second, baseMsg & ".get(1)"))
			list.Add(New Pair(Of INDArray, String)(third, baseMsg & ".get(2)"))
			Return list
		End Function



		Public Shared Function getTensorAlongDimensionMatricesWithShape(ByVal ordering As Char, ByVal rows As Long, ByVal cols As Long, ByVal seed As Long, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Nd4j.Random.Seed = seed
			'From 3d NDArray: do various tensors. One offset 0, one offset > 0
			'[0,1], [0,2], [1,0], [1,2], [2,0], [2,1]
			Dim [out](11) As INDArray

			Dim temp01 As INDArray = Nd4j.linspace(1, cols * rows * 4, cols * rows * 4, dataType).reshape(ChrW(cols), rows, 4)
			[out](0) = temp01.tensorAlongDimension(0, 0, 1).reshape(rows, cols)
			Dim temp01Shape() As Long = {cols, rows, 4}
			Dim len As Integer = ArrayUtil.prod(temp01Shape)
			temp01 = Nd4j.linspace(1, len, len, dataType).reshape(temp01Shape)
			[out](1) = temp01.tensorAlongDimension(2, 0, 1).reshape(rows, cols)

			Nd4j.Random.Seed = seed
			Dim temp02 As INDArray = Nd4j.linspace(1, len, len, dataType).reshape(New Long() {cols, 4, rows})
			[out](2) = temp02.tensorAlongDimension(0, 0, 2).reshape(rows, cols)
			temp02 = Nd4j.linspace(1, len, len, dataType).reshape(ChrW(cols), 4, rows)
			[out](3) = temp02.tensorAlongDimension(2, 0, 2).reshape(rows, cols)

			Dim temp10 As INDArray = Nd4j.linspace(1, len, len, dataType).reshape(ChrW(rows), cols, 4)
			[out](4) = temp10.tensorAlongDimension(0, 1, 0).reshape(rows, cols)
			temp10 = Nd4j.linspace(1, len, len, dataType).reshape(ChrW(rows), cols, 4)
			[out](5) = temp10.tensorAlongDimension(2, 1, 0).reshape(rows, cols)

			Dim temp12 As INDArray = Nd4j.linspace(1, len, len, dataType).reshape(ChrW(4), cols, rows)
			[out](6) = temp12.tensorAlongDimension(0, 1, 2).reshape(rows, cols)
			temp12 = Nd4j.linspace(1, len, len, dataType).reshape(ChrW(4), cols, rows)
			[out](7) = temp12.tensorAlongDimension(2, 1, 2).reshape(rows, cols)

			Dim temp20 As INDArray = Nd4j.linspace(1, len, len, dataType).reshape(ChrW(rows), 4, cols)
			[out](8) = temp20.tensorAlongDimension(0, 2, 0).reshape(rows, cols)
			temp20 = Nd4j.linspace(1, len, len, dataType).reshape(ChrW(rows), 4, cols)
			[out](9) = temp20.tensorAlongDimension(2, 2, 0).reshape(rows, cols)

			Dim temp21 As INDArray = Nd4j.linspace(1, len, len, dataType).reshape(ChrW(4), rows, cols)
			[out](10) = temp21.tensorAlongDimension(0, 2, 1).reshape(rows, cols)
			temp21 = Nd4j.linspace(1, len, len, dataType).reshape(ChrW(4), rows, cols)
			[out](11) = temp21.tensorAlongDimension(2, 2, 1).reshape(rows, cols)

			Dim baseMsg As String = "getTensorAlongDimensionMatricesWithShape(" & rows & "," & cols & "," & seed & ")"
			Dim list As IList(Of Pair(Of INDArray, String)) = New List(Of Pair(Of INDArray, String))(12)

			For i As Integer = 0 To [out].Length - 1
				list.Add(New Pair(Of INDArray, String)([out](i), baseMsg & ".get(" & i & ")"))
			Next i

			Return list
		End Function

		Public Shared Function getTensorAlongDimensionMatricesWithShape(ByVal rows As Long, ByVal cols As Long, ByVal seed As Long, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Return getTensorAlongDimensionMatricesWithShape(Nd4j.order(), rows, cols, seed, dataType)
		End Function


		Public Shared Function getPermutedWithShape(ByVal ordering As Char, ByVal rows As Long, ByVal cols As Long, ByVal seed As Long, ByVal dataType As DataType) As Pair(Of INDArray, String)
			Nd4j.Random.Seed = seed
			Dim len As Long = rows * cols
			Dim arr As INDArray = Nd4j.linspace(1, len, len, dataType).reshape(ChrW(cols), rows)
			Return New Pair(Of INDArray, String)(arr.permute(1, 0), "getPermutedWithShape(" & rows & "," & cols & "," & seed & ")")
		End Function

		Public Shared Function getPermutedWithShape(ByVal rows As Long, ByVal cols As Long, ByVal seed As Long, ByVal dataType As DataType) As Pair(Of INDArray, String)
			Return getPermutedWithShape(Nd4j.order(), rows, cols, seed, dataType)
		End Function


		Public Shared Function getReshapedWithShape(ByVal ordering As Char, ByVal rows As Long, ByVal cols As Long, ByVal seed As Long, ByVal dataType As DataType) As Pair(Of INDArray, String)
			Nd4j.Random.Seed = seed
			Dim origShape(2) As Long
			If rows Mod 2 = 0 Then
				origShape(0) = rows \ 2
				origShape(1) = cols
				origShape(2) = 2
			ElseIf cols Mod 2 = 0 Then
				origShape(0) = rows
				origShape(1) = cols \ 2
				origShape(2) = 2
			Else
				origShape(0) = 1
				origShape(1) = rows
				origShape(2) = cols
			End If

			Dim len As Integer = ArrayUtil.prod(origShape)
			Dim orig As INDArray = Nd4j.linspace(1, len, len, dataType).reshape(ordering, origShape)
			Return New Pair(Of INDArray, String)(orig.reshape(ordering, rows, cols), "getReshapedWithShape(" & rows & "," & cols & "," & seed & ")")
		End Function

		Public Shared Function getReshapedWithShape(ByVal rows As Long, ByVal cols As Long, ByVal seed As Long, ByVal dataType As DataType) As Pair(Of INDArray, String)
			Return getReshapedWithShape(Nd4j.order(), rows, cols, seed, dataType)
		End Function

		Public Shared Function getAll3dTestArraysWithShape(ByVal seed As Integer, ByVal shape() As Integer, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Return getAll3dTestArraysWithShape(seed, ArrayUtil.toLongArray(shape), dataType)
		End Function

		Public Shared Function getAll3dTestArraysWithShape(ByVal seed As Long, ByVal shape() As Long, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			If shape.Length <> 3 Then
				Throw New System.ArgumentException("Shape is not length 3")
			End If

			Dim list As IList(Of Pair(Of INDArray, String)) = New List(Of Pair(Of INDArray, String))()

			Dim baseMsg As String = "getAll3dTestArraysWithShape(" & seed & "," & java.util.Arrays.toString(shape) & ").get("


			Dim len As val = ArrayUtil.prodLong(shape)
			'Basic 3d in C and F orders:
			Nd4j.Random.Seed = seed
			Dim stdC As INDArray = Nd4j.linspace(1, len, len, dataType).reshape("c"c, shape)
			Dim stdF As INDArray = Nd4j.linspace(1, len, len, dataType).reshape("f"c, shape)
			list.Add(New Pair(Of INDArray, String)(stdC, baseMsg & "0)/Nd4j.linspace(1,len,len)(" & java.util.Arrays.toString(shape) & ",'c')"))
			list.Add(New Pair(Of INDArray, String)(stdF, baseMsg & "1)/Nd4j.linspace(1,len,len(" & java.util.Arrays.toString(shape) & ",'f')"))

			'Various sub arrays:
			CType(list, List(Of Pair(Of INDArray, String))).AddRange(get3dSubArraysWithShape(seed, shape, dataType))

			'TAD
			CType(list, List(Of Pair(Of INDArray, String))).AddRange(get3dTensorAlongDimensionWithShape(seed, shape, dataType))

			'Permuted
			CType(list, List(Of Pair(Of INDArray, String))).AddRange(get3dPermutedWithShape(seed, shape, dataType))

			'Reshaped
			CType(list, List(Of Pair(Of INDArray, String))).AddRange(get3dReshapedWithShape(seed, shape, dataType))

			Return list
		End Function

		Public Shared Function get3dSubArraysWithShape(ByVal seed As Integer, ByVal shape() As Integer, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Return get3dSubArraysWithShape(seed, ArrayUtil.toLongArray(shape), dataType)
		End Function

		Public Shared Function get3dSubArraysWithShape(ByVal seed As Long, ByVal shape() As Long, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Dim list As IList(Of Pair(Of INDArray, String)) = New List(Of Pair(Of INDArray, String))()
			Dim baseMsg As String = "get3dSubArraysWithShape(" & seed & "," & java.util.Arrays.toString(shape) & ")"
			'Create and return various sub arrays:
			Nd4j.Random.Seed = seed
			Dim newShape1 As val = Arrays.CopyOf(shape, shape.Length)
			newShape1(0) += 5
			Dim len As Integer = ArrayUtil.prod(newShape1)
			Dim temp1 As INDArray = Nd4j.linspace(1, len, len, dataType).reshape(newShape1)
			Dim subset1 As INDArray = temp1.get(NDArrayIndex.interval(2, shape(0) + 2), NDArrayIndex.all(), NDArrayIndex.all())
			list.Add(New Pair(Of INDArray, String)(subset1, baseMsg & ".get(0)"))

			Dim newShape2 As val = Arrays.CopyOf(shape, shape.Length)
			newShape2(1) += 5
			Dim len2 As Integer = ArrayUtil.prod(newShape2)
			Dim temp2 As INDArray = Nd4j.linspace(1, len2, len2, dataType).reshape(newShape2)
			Dim subset2 As INDArray = temp2.get(NDArrayIndex.all(), NDArrayIndex.interval(3, shape(1) + 3), NDArrayIndex.all())
			list.Add(New Pair(Of INDArray, String)(subset2, baseMsg & ".get(1)"))

			Dim newShape3 As val = Arrays.CopyOf(shape, shape.Length)
			newShape3(2) += 5
			Dim len3 As Integer = ArrayUtil.prod(newShape3)
			Dim temp3 As INDArray = Nd4j.linspace(1, len3, len3, dataType).reshape(newShape3)
			Dim subset3 As INDArray = temp3.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(4, shape(2) + 4))
			list.Add(New Pair(Of INDArray, String)(subset3, baseMsg & ".get(2)"))

			Dim newShape4 As val = Arrays.CopyOf(shape, shape.Length)
			newShape4(0) += 5
			newShape4(1) += 5
			newShape4(2) += 5
			Dim len4 As Integer = ArrayUtil.prod(newShape4)
			Dim temp4 As INDArray = Nd4j.linspace(1, len4, len4, dataType).reshape(newShape4)
			Dim subset4 As INDArray = temp4.get(NDArrayIndex.interval(4, shape(0) + 4), NDArrayIndex.interval(3, shape(1) + 3), NDArrayIndex.interval(2, shape(2) + 2))
			list.Add(New Pair(Of INDArray, String)(subset4, baseMsg & ".get(3)"))

			Return list
		End Function

		Public Shared Function get3dTensorAlongDimensionWithShape(ByVal seed As Integer, ByVal shape() As Integer, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Return get3dTensorAlongDimensionWithShape(seed, ArrayUtil.toLongArray(shape), dataType)
		End Function

		Public Shared Function get3dTensorAlongDimensionWithShape(ByVal seed As Long, ByVal shape() As Long, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Dim list As IList(Of Pair(Of INDArray, String)) = New List(Of Pair(Of INDArray, String))()
			Dim baseMsg As String = "get3dTensorAlongDimensionWithShape(" & seed & "," & java.util.Arrays.toString(shape) & ")"

			'Create some 4d arrays and get subsets using 3d TAD on them
			'This is not an exhaustive list of possible 3d arrays from 4d via TAD

			Nd4j.Random.Seed = seed
			'            int[] shape4d1 = {shape[2],shape[1],shape[0],3};
			Dim shape4d1 As val = New Long(){shape(0), shape(1), shape(2), 3}
			Dim lenshape4d1 As Integer = ArrayUtil.prod(shape4d1)
			Dim orig1a As INDArray = Nd4j.linspace(1, lenshape4d1, lenshape4d1, dataType).reshape(shape4d1)
			Dim tad1a As INDArray = orig1a.tensorAlongDimension(0, 0, 1, 2)
			Dim orig1b As INDArray = Nd4j.linspace(1, lenshape4d1, lenshape4d1, dataType).reshape(shape4d1)
			Dim tad1b As INDArray = orig1b.tensorAlongDimension(1, 0, 1, 2)

			list.Add(New Pair(Of INDArray, String)(tad1a, baseMsg & ".get(0)"))
			list.Add(New Pair(Of INDArray, String)(tad1b, baseMsg & ".get(1)"))

			Dim shape4d2() As Long = {3, shape(0), shape(1), shape(2)}
			Dim lenshape4d2 As Integer = ArrayUtil.prod(shape4d2)
			Dim orig2 As INDArray = Nd4j.linspace(1, lenshape4d2, lenshape4d2, dataType).reshape(shape4d2)
			Dim tad2 As INDArray = orig2.tensorAlongDimension(1, 1, 2, 3)
			list.Add(New Pair(Of INDArray, String)(tad2, baseMsg & ".get(2)"))

			Dim shape4d3() As Long = {shape(0), shape(1), 3, shape(2)}
			Dim lenshape4d3 As Integer = ArrayUtil.prod(shape4d3)
			Dim orig3 As INDArray = Nd4j.linspace(1, lenshape4d3, lenshape4d3, dataType).reshape(shape4d3)
			Dim tad3 As INDArray = orig3.tensorAlongDimension(1, 1, 3, 0)
			list.Add(New Pair(Of INDArray, String)(tad3, baseMsg & ".get(3)"))

			Dim shape4d4() As Long = {shape(0), 3, shape(1), shape(2)}
			Dim lenshape4d4 As Integer = ArrayUtil.prod(shape4d4)
			Dim orig4 As INDArray = Nd4j.linspace(1, lenshape4d4, lenshape4d4, dataType).reshape(shape4d4)
			Dim tad4 As INDArray = orig4.tensorAlongDimension(1, 2, 0, 3)
			list.Add(New Pair(Of INDArray, String)(tad4, baseMsg & ".get(4)"))

			Return list
		End Function

		Public Shared Function get3dPermutedWithShape(ByVal seed As Integer, ByVal shape() As Integer, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Return get3dPermutedWithShape(seed, ArrayUtil.toLongArray(shape), dataType)
		End Function

		Public Shared Function get3dPermutedWithShape(ByVal seed As Long, ByVal shape() As Long, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Nd4j.Random.Seed = seed
			Dim createdShape() As Long = {shape(1), shape(2), shape(0)}
			Dim lencreatedShape As Integer = ArrayUtil.prod(createdShape)
			Dim arr As INDArray = Nd4j.linspace(1, lencreatedShape, lencreatedShape, dataType).reshape(createdShape)
			Dim permuted As INDArray = arr.permute(2, 0, 1)
			Return Collections.singletonList(New Pair(Of )(permuted, "get3dPermutedWithShape(" & seed & "," & java.util.Arrays.toString(shape) & ").get(0)"))
		End Function

		Public Shared Function get3dReshapedWithShape(ByVal seed As Integer, ByVal shape() As Integer, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Return get3dReshapedWithShape(seed, ArrayUtil.toLongArray(shape), dataType)
		End Function

		Public Shared Function get3dReshapedWithShape(ByVal seed As Long, ByVal shape() As Long, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Nd4j.Random.Seed = seed
			Dim shape2d() As Long = {shape(0) * shape(2), shape(1)}
			Dim lenshape2d As Integer = ArrayUtil.prod(shape2d)
			Dim array2d As INDArray = Nd4j.linspace(1, lenshape2d, lenshape2d, dataType).reshape(shape2d)
			Dim array3d As INDArray = array2d.reshape(shape)
			Return Collections.singletonList(New Pair(Of )(array3d, "get3dReshapedWithShape(" & seed & "," & java.util.Arrays.toString(shape) & ").get(0)"))
		End Function

		Public Shared Function getAll4dTestArraysWithShape(ByVal seed As Integer, ByVal shape() As Long, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Return getAll4dTestArraysWithShape(seed, ArrayUtil.toInts(shape), dataType)
		End Function

		Public Shared Function getAll4dTestArraysWithShape(ByVal seed As Integer, ByVal shape() As Integer, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			If shape.Length <> 4 Then
				Throw New System.ArgumentException("Shape is not length 4")
			End If

			Dim list As IList(Of Pair(Of INDArray, String)) = New List(Of Pair(Of INDArray, String))()

			Dim baseMsg As String = "getAll4dTestArraysWithShape(" & seed & "," & java.util.Arrays.toString(shape) & ").get("

			'Basic 4d in C and F orders:
			Nd4j.Random.setSeed(seed)
			Dim len As Integer = ArrayUtil.prod(shape)
			Dim stdC As INDArray = Nd4j.linspace(1, len, len, dataType).reshape("c"c, ArrayUtil.toLongArray(shape))
			Dim stdF As INDArray = Nd4j.linspace(1, len, len, dataType).reshape("f"c, ArrayUtil.toLongArray(shape))
			list.Add(New Pair(Of INDArray, String)(stdC, baseMsg & "0)/Nd4j.rand(" & java.util.Arrays.toString(shape) & ",'c')"))
			list.Add(New Pair(Of INDArray, String)(stdF, baseMsg & "1)/Nd4j.rand(" & java.util.Arrays.toString(shape) & ",'f')"))

			'Various sub arrays:
			CType(list, List(Of Pair(Of INDArray, String))).AddRange(get4dSubArraysWithShape(seed, shape, dataType))

			'TAD
			CType(list, List(Of Pair(Of INDArray, String))).AddRange(get4dTensorAlongDimensionWithShape(seed, shape, dataType))

			'Permuted
			CType(list, List(Of Pair(Of INDArray, String))).AddRange(get4dPermutedWithShape(seed, shape, dataType))

			'Reshaped
			CType(list, List(Of Pair(Of INDArray, String))).AddRange(get4dReshapedWithShape(seed, shape, dataType))

			Return list
		End Function

		Public Shared Function get4dSubArraysWithShape(ByVal seed As Integer, ByVal shape() As Integer, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Dim list As IList(Of Pair(Of INDArray, String)) = New List(Of Pair(Of INDArray, String))()
			Dim baseMsg As String = "get4dSubArraysWithShape(" & seed & "," & java.util.Arrays.toString(shape) & ")"
			'Create and return various sub arrays:
			Nd4j.Random.setSeed(seed)
			Dim newShape1() As Integer = Arrays.CopyOf(shape, shape.Length)
			newShape1(0) += 5
			Dim len As Integer = ArrayUtil.prod(newShape1)
			Dim temp1 As INDArray = Nd4j.linspace(1, len, len, dataType).reshape(ArrayUtil.toLongArray(newShape1))
			Dim subset1 As INDArray = temp1.get(NDArrayIndex.interval(2, shape(0) + 2), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all())
			list.Add(New Pair(Of INDArray, String)(subset1, baseMsg & ".get(0)"))

			Dim newShape2() As Integer = Arrays.CopyOf(shape, shape.Length)
			newShape2(1) += 5
			Dim len2 As Integer = ArrayUtil.prod(newShape2)
			Dim temp2 As INDArray = Nd4j.linspace(1, len2, len2, dataType).reshape(ArrayUtil.toLongArray(newShape2))
			Dim subset2 As INDArray = temp2.get(NDArrayIndex.all(), NDArrayIndex.interval(3, shape(1) + 3), NDArrayIndex.all(), NDArrayIndex.all())
			list.Add(New Pair(Of INDArray, String)(subset2, baseMsg & ".get(1)"))

			Dim newShape3() As Integer = Arrays.CopyOf(shape, shape.Length)
			newShape3(2) += 5
			Dim len3 As Integer = ArrayUtil.prod(newShape3)
			Dim temp3 As INDArray = Nd4j.linspace(1, len3, len3, dataType).reshape(ArrayUtil.toLongArray(newShape3))
			Dim subset3 As INDArray = temp3.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(4, shape(2) + 4), NDArrayIndex.all())
			list.Add(New Pair(Of INDArray, String)(subset3, baseMsg & ".get(2)"))

			Dim newShape4() As Integer = Arrays.CopyOf(shape, shape.Length)
			newShape4(3) += 5
			Dim len4 As Integer = ArrayUtil.prod(newShape4)
			Dim temp4 As INDArray = Nd4j.linspace(1, len4, len4, dataType).reshape(ArrayUtil.toLongArray(newShape4))
			Dim subset4 As INDArray = temp4.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(3, shape(3) + 3))
			list.Add(New Pair(Of INDArray, String)(subset4, baseMsg & ".get(3)"))

			Dim newShape5() As Integer = Arrays.CopyOf(shape, shape.Length)
			newShape5(0) += 5
			newShape5(1) += 5
			newShape5(2) += 5
			newShape5(3) += 5
			Dim len5 As Integer = ArrayUtil.prod(newShape5)
			Dim temp5 As INDArray = Nd4j.linspace(1, len5, len5, dataType).reshape(ArrayUtil.toLongArray(newShape5))
			Dim subset5 As INDArray = temp5.get(NDArrayIndex.interval(4, shape(0) + 4), NDArrayIndex.interval(3, shape(1) + 3), NDArrayIndex.interval(2, shape(2) + 2), NDArrayIndex.interval(1, shape(3) + 1))
			list.Add(New Pair(Of INDArray, String)(subset5, baseMsg & ".get(4)"))

			Return list
		End Function

		Public Shared Function get4dTensorAlongDimensionWithShape(ByVal seed As Integer, ByVal shape() As Integer, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Dim list As IList(Of Pair(Of INDArray, String)) = New List(Of Pair(Of INDArray, String))()
			Dim baseMsg As String = "get4dTensorAlongDimensionWithShape(" & seed & "," & java.util.Arrays.toString(shape) & ")"

			'Create some 5d arrays and get subsets using 4d TAD on them
			'This is not an exhausive list of possible 4d arrays from 5d via TAD
			Nd4j.Random.setSeed(seed)
			Dim shape4d1() As Integer = {3, shape(0), shape(1), shape(2), shape(3)}
			Dim len As Integer = ArrayUtil.prod(shape4d1)
			Dim orig1a As INDArray = Nd4j.linspace(1, len, len, dataType).reshape(ArrayUtil.toLongArray(shape4d1))
			Dim tad1a As INDArray = orig1a.tensorAlongDimension(0, 1, 2, 3, 4)
			Dim orig1b As INDArray = Nd4j.linspace(1, len, len, dataType).reshape(ArrayUtil.toLongArray(shape4d1))
			Dim tad1b As INDArray = orig1b.tensorAlongDimension(2, 1, 2, 3, 4)

			list.Add(New Pair(Of INDArray, String)(tad1a, baseMsg & ".get(0)"))
			list.Add(New Pair(Of INDArray, String)(tad1b, baseMsg & ".get(1)"))

			Dim shape4d2() As Integer = {3, shape(0), shape(1), shape(2), shape(3)}
			Dim len2 As Integer = ArrayUtil.prod(shape4d2)
			Dim orig2 As INDArray = Nd4j.linspace(1, len2, len2, dataType).reshape(ArrayUtil.toLongArray(shape4d2))
			Dim tad2 As INDArray = orig2.tensorAlongDimension(1, 3, 4, 2, 1)
			list.Add(New Pair(Of INDArray, String)(tad2, baseMsg & ".get(2)"))

			Dim shape4d3() As Integer = {shape(0), shape(1), 3, shape(2), shape(3)}
			Dim len3 As Integer = ArrayUtil.prod(shape4d3)
			Dim orig3 As INDArray = Nd4j.linspace(1, len3, len3, dataType).reshape(ArrayUtil.toLongArray(shape4d3))
			Dim tad3 As INDArray = orig3.tensorAlongDimension(1, 4, 1, 3, 0)
			list.Add(New Pair(Of INDArray, String)(tad3, baseMsg & ".get(3)"))

			Dim shape4d4() As Integer = {shape(0), shape(1), shape(2), shape(3), 3}
			Dim len4 As Integer = ArrayUtil.prod(shape4d4)
			Dim orig4 As INDArray = Nd4j.linspace(1, len4, len4, dataType).reshape(ArrayUtil.toLongArray(shape4d4))
			Dim tad4 As INDArray = orig4.tensorAlongDimension(1, 2, 0, 3, 1)
			list.Add(New Pair(Of INDArray, String)(tad4, baseMsg & ".get(4)"))

			Return list
		End Function

		Public Shared Function get4dPermutedWithShape(ByVal seed As Integer, ByVal shape() As Integer, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Nd4j.Random.setSeed(seed)
			Dim createdShape() As Integer = {shape(1), shape(3), shape(2), shape(0)}
			Dim arr As INDArray = Nd4j.rand(dataType, createdShape)
			Dim permuted As INDArray = arr.permute(3, 0, 2, 1)
			Return Collections.singletonList(New Pair(Of )(permuted, "get4dPermutedWithShape(" & seed & "," & java.util.Arrays.toString(shape) & ").get(0)"))
		End Function

		Public Shared Function get4dReshapedWithShape(ByVal seed As Integer, ByVal shape() As Integer, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Nd4j.Random.setSeed(seed)
			Dim shape2d() As Integer = {shape(0) * shape(2), shape(1) * shape(3)}
			Dim array2d As INDArray = Nd4j.rand(dataType, shape2d)
			Dim array3d As INDArray = array2d.reshape(ArrayUtil.toLongArray(shape))
			Return Collections.singletonList(New Pair(Of )(array3d, "get4dReshapedWithShape(" & seed & "," & java.util.Arrays.toString(shape) & ").get(0)"))
		End Function



		Public Shared Function getAll5dTestArraysWithShape(ByVal seed As Integer, ByVal shape() As Integer, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			If shape.Length <> 5 Then
				Throw New System.ArgumentException("Shape is not length 5")
			End If

			Dim list As IList(Of Pair(Of INDArray, String)) = New List(Of Pair(Of INDArray, String))()

			Dim baseMsg As String = "getAll5dTestArraysWithShape(" & seed & "," & java.util.Arrays.toString(shape) & ").get("

			'Basic 5d in C and F orders:
			Nd4j.Random.setSeed(seed)
			Dim stdC As INDArray = Nd4j.rand(dataType, shape, "c"c)
			Dim stdF As INDArray = Nd4j.rand(dataType, shape, "f"c)
			list.Add(New Pair(Of INDArray, String)(stdC, baseMsg & "0)/Nd4j.rand(" & java.util.Arrays.toString(shape) & ",'c')"))
			list.Add(New Pair(Of INDArray, String)(stdF, baseMsg & "1)/Nd4j.rand(" & java.util.Arrays.toString(shape) & ",'f')"))

			'Various sub arrays:
			CType(list, List(Of Pair(Of INDArray, String))).AddRange(get5dSubArraysWithShape(seed, shape, dataType))

			'TAD
			CType(list, List(Of Pair(Of INDArray, String))).AddRange(get5dTensorAlongDimensionWithShape(seed, shape, dataType))

			'Permuted
			CType(list, List(Of Pair(Of INDArray, String))).AddRange(get5dPermutedWithShape(seed, shape, dataType))

			'Reshaped
			CType(list, List(Of Pair(Of INDArray, String))).AddRange(get5dReshapedWithShape(seed, shape, dataType))

			Return list
		End Function

		Public Shared Function get5dSubArraysWithShape(ByVal seed As Integer, ByVal shape() As Integer, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Dim list As IList(Of Pair(Of INDArray, String)) = New List(Of Pair(Of INDArray, String))()
			Dim baseMsg As String = "get5dSubArraysWithShape(" & seed & "," & java.util.Arrays.toString(shape) & ")"
			'Create and return various sub arrays:
			Nd4j.Random.setSeed(seed)
			Dim newShape1() As Integer = Arrays.CopyOf(shape, shape.Length)
			newShape1(0) += 5
			Dim temp1 As INDArray = Nd4j.rand(dataType, newShape1)
			Dim subset1 As INDArray = temp1.get(NDArrayIndex.interval(2, shape(0) + 2), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all())
			list.Add(New Pair(Of INDArray, String)(subset1, baseMsg & ".get(0)"))

			Dim newShape2() As Integer = Arrays.CopyOf(shape, shape.Length)
			newShape2(1) += 5
			Dim temp2 As INDArray = Nd4j.rand(dataType, newShape2)
			Dim subset2 As INDArray = temp2.get(NDArrayIndex.all(), NDArrayIndex.interval(3, shape(1) + 3), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all())
			list.Add(New Pair(Of INDArray, String)(subset2, baseMsg & ".get(1)"))

			Dim newShape3() As Integer = Arrays.CopyOf(shape, shape.Length)
			newShape3(2) += 5
			Dim temp3 As INDArray = Nd4j.rand(dataType, newShape3)
			Dim subset3 As INDArray = temp3.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(4, shape(2) + 4), NDArrayIndex.all(), NDArrayIndex.all())
			list.Add(New Pair(Of INDArray, String)(subset3, baseMsg & ".get(2)"))

			Dim newShape4() As Integer = Arrays.CopyOf(shape, shape.Length)
			newShape4(3) += 5
			Dim temp4 As INDArray = Nd4j.rand(dataType, newShape4)
			Dim subset4 As INDArray = temp4.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(3, shape(3) + 3), NDArrayIndex.all())
			list.Add(New Pair(Of INDArray, String)(subset4, baseMsg & ".get(3)"))

			Dim newShape5() As Integer = Arrays.CopyOf(shape, shape.Length)
			newShape5(4) += 5
			Dim temp5 As INDArray = Nd4j.rand(dataType, newShape5)
			Dim subset5 As INDArray = temp5.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(3, shape(4) + 3))
			list.Add(New Pair(Of INDArray, String)(subset5, baseMsg & ".get(4)"))

			Dim newShape6() As Integer = Arrays.CopyOf(shape, shape.Length)
			newShape6(0) += 5
			newShape6(1) += 5
			newShape6(2) += 5
			newShape6(3) += 5
			newShape6(4) += 5
			Dim temp6 As INDArray = Nd4j.rand(dataType, newShape6)
			Dim subset6 As INDArray = temp6.get(NDArrayIndex.interval(4, shape(0) + 4), NDArrayIndex.interval(3, shape(1) + 3), NDArrayIndex.interval(2, shape(2) + 2), NDArrayIndex.interval(1, shape(3) + 1), NDArrayIndex.interval(2, shape(4) + 2))
			list.Add(New Pair(Of INDArray, String)(subset6, baseMsg & ".get(5)"))

			Return list
		End Function

		Public Shared Function get5dTensorAlongDimensionWithShape(ByVal seed As Integer, ByVal shape() As Integer, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Dim list As IList(Of Pair(Of INDArray, String)) = New List(Of Pair(Of INDArray, String))()
			Dim baseMsg As String = "get5dTensorAlongDimensionWithShape(" & seed & "," & java.util.Arrays.toString(shape) & ")"

			'Create some 6d arrays and get subsets using 5d TAD on them
			'This is not an exhausive list of possible 5d arrays from 6d via TAD
			Nd4j.Random.setSeed(seed)
			Dim shape4d1() As Integer = {3, shape(0), shape(1), shape(2), shape(3), shape(4)}
			Dim orig1a As INDArray = Nd4j.rand(dataType, shape4d1)
			Dim tad1a As INDArray = orig1a.tensorAlongDimension(0, 1, 2, 3, 4, 5)
			Dim orig1b As INDArray = Nd4j.rand(dataType, shape4d1)
			Dim tad1b As INDArray = orig1b.tensorAlongDimension(2, 1, 2, 3, 4, 5)

			list.Add(New Pair(Of INDArray, String)(tad1a, baseMsg & ".get(0)"))
			list.Add(New Pair(Of INDArray, String)(tad1b, baseMsg & ".get(1)"))

			Dim shape4d2() As Integer = {3, shape(0), shape(1), shape(2), shape(3), shape(4)}
			Dim orig2 As INDArray = Nd4j.rand(dataType, shape4d2)
			Dim tad2 As INDArray = orig2.tensorAlongDimension(1, 3, 5, 4, 2, 1)
			list.Add(New Pair(Of INDArray, String)(tad2, baseMsg & ".get(2)"))

			Dim shape4d3() As Integer = {shape(0), shape(1), shape(2), shape(3), shape(4), 2}
			Dim orig3 As INDArray = Nd4j.rand(dataType, shape4d3)
			Dim tad3 As INDArray = orig3.tensorAlongDimension(1, 4, 1, 3, 2, 0)
			list.Add(New Pair(Of INDArray, String)(tad3, baseMsg & ".get(3)"))

			Dim shape4d4() As Integer = {shape(0), shape(1), shape(2), shape(3), 3, shape(4)}
			Dim orig4 As INDArray = Nd4j.rand(dataType, shape4d4)
			Dim tad4 As INDArray = orig4.tensorAlongDimension(1, 5, 2, 0, 3, 1)
			list.Add(New Pair(Of INDArray, String)(tad4, baseMsg & ".get(4)"))

			Return list
		End Function

		Public Shared Function get5dPermutedWithShape(ByVal seed As Integer, ByVal shape() As Integer, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Nd4j.Random.setSeed(seed)
			Dim createdShape() As Integer = {shape(1), shape(4), shape(3), shape(2), shape(0)}
			Dim arr As INDArray = Nd4j.rand(dataType, createdShape)
			Dim permuted As INDArray = arr.permute(4, 0, 3, 2, 1)
			Return Collections.singletonList(New Pair(Of )(permuted, "get5dPermutedWithShape(" & seed & "," & java.util.Arrays.toString(shape) & ").get(0)"))
		End Function

		Public Shared Function get5dReshapedWithShape(ByVal seed As Integer, ByVal shape() As Integer, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Nd4j.Random.setSeed(seed)
			Dim shape2d() As Integer = {shape(0) * shape(2), shape(4), shape(1) * shape(3)}
			Dim array3d As INDArray = Nd4j.rand(dataType, shape2d)
			Dim array5d As INDArray = array3d.reshape(ArrayUtil.toLongArray(shape))
			Return Collections.singletonList(New Pair(Of )(array5d, "get5dReshapedWithShape(" & seed & "," & java.util.Arrays.toString(shape) & ").get(0)"))
		End Function



		Public Shared Function getAll6dTestArraysWithShape(ByVal seed As Integer, ByVal shape() As Integer, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			If shape.Length <> 6 Then
				Throw New System.ArgumentException("Shape is not length 6")
			End If

			Dim list As IList(Of Pair(Of INDArray, String)) = New List(Of Pair(Of INDArray, String))()

			Dim baseMsg As String = "getAll6dTestArraysWithShape(" & seed & "," & java.util.Arrays.toString(shape) & ").get("

			'Basic 5d in C and F orders:
			Nd4j.Random.setSeed(seed)
			Dim stdC As INDArray = Nd4j.rand(dataType, shape, "c"c)
			Dim stdF As INDArray = Nd4j.rand(dataType, shape, "f"c)
			list.Add(New Pair(Of INDArray, String)(stdC, baseMsg & "0)/Nd4j.rand(" & java.util.Arrays.toString(shape) & ",'c')"))
			list.Add(New Pair(Of INDArray, String)(stdF, baseMsg & "1)/Nd4j.rand(" & java.util.Arrays.toString(shape) & ",'f')"))

			'Various sub arrays:
			CType(list, List(Of Pair(Of INDArray, String))).AddRange(get6dSubArraysWithShape(seed, shape, dataType))

			'Permuted
			CType(list, List(Of Pair(Of INDArray, String))).AddRange(get6dPermutedWithShape(seed, shape, dataType))

			'Reshaped
			CType(list, List(Of Pair(Of INDArray, String))).AddRange(get6dReshapedWithShape(seed, shape, dataType))

			Return list
		End Function

		Public Shared Function get6dSubArraysWithShape(ByVal seed As Integer, ByVal shape() As Integer, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Dim list As IList(Of Pair(Of INDArray, String)) = New List(Of Pair(Of INDArray, String))()
			Dim baseMsg As String = "get6dSubArraysWithShape(" & seed & "," & java.util.Arrays.toString(shape) & ")"
			'Create and return various sub arrays:
			Nd4j.Random.setSeed(seed)
			Dim newShape1() As Integer = Arrays.CopyOf(shape, shape.Length)
			newShape1(0) += 5
			Dim temp1 As INDArray = Nd4j.rand(dataType, newShape1)
			Dim subset1 As INDArray = temp1.get(NDArrayIndex.interval(2, shape(0) + 2), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all())
			list.Add(New Pair(Of INDArray, String)(subset1, baseMsg & ".get(0)"))

			Dim newShape2() As Integer = Arrays.CopyOf(shape, shape.Length)
			newShape2(1) += 5
			Dim temp2 As INDArray = Nd4j.rand(dataType, newShape2)
			Dim subset2 As INDArray = temp2.get(NDArrayIndex.all(), NDArrayIndex.interval(3, shape(1) + 3), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all())
			list.Add(New Pair(Of INDArray, String)(subset2, baseMsg & ".get(1)"))

			Dim newShape3() As Integer = Arrays.CopyOf(shape, shape.Length)
			newShape3(2) += 5
			Dim temp3 As INDArray = Nd4j.rand(dataType, newShape3)
			Dim subset3 As INDArray = temp3.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(4, shape(2) + 4), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all())
			list.Add(New Pair(Of INDArray, String)(subset3, baseMsg & ".get(2)"))

			Dim newShape4() As Integer = Arrays.CopyOf(shape, shape.Length)
			newShape4(3) += 5
			Dim temp4 As INDArray = Nd4j.rand(dataType, newShape4)
			Dim subset4 As INDArray = temp4.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(3, shape(3) + 3), NDArrayIndex.all(), NDArrayIndex.all())
			list.Add(New Pair(Of INDArray, String)(subset4, baseMsg & ".get(3)"))

			Dim newShape5() As Integer = Arrays.CopyOf(shape, shape.Length)
			newShape5(4) += 5
			Dim temp5 As INDArray = Nd4j.rand(dataType, newShape5)
			Dim subset5 As INDArray = temp5.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(3, shape(4) + 3), NDArrayIndex.all())
			list.Add(New Pair(Of INDArray, String)(subset5, baseMsg & ".get(4)"))

			Dim newShape6() As Integer = Arrays.CopyOf(shape, shape.Length)
			newShape6(5) += 5
			Dim temp6 As INDArray = Nd4j.rand(dataType, newShape6)
			Dim subset6 As INDArray = temp6.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(1, shape(5) + 1))
			list.Add(New Pair(Of INDArray, String)(subset6, baseMsg & ".get(5)"))

			Dim newShape7() As Integer = Arrays.CopyOf(shape, shape.Length)
			newShape7(0) += 5
			newShape7(1) += 5
			newShape7(2) += 5
			newShape7(3) += 5
			newShape7(4) += 5
			newShape7(5) += 5
			Dim temp7 As INDArray = Nd4j.rand(dataType, newShape7)
			Dim subset7 As INDArray = temp7.get(NDArrayIndex.interval(4, shape(0) + 4), NDArrayIndex.interval(3, shape(1) + 3), NDArrayIndex.interval(2, shape(2) + 2), NDArrayIndex.interval(1, shape(3) + 1), NDArrayIndex.interval(2, shape(4) + 2), NDArrayIndex.interval(3, shape(5) + 3))
			list.Add(New Pair(Of INDArray, String)(subset7, baseMsg & ".get(6)"))

			Return list
		End Function

		Public Shared Function get6dPermutedWithShape(ByVal seed As Integer, ByVal shape() As Integer, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Nd4j.Random.setSeed(seed)
			Dim createdShape() As Integer = {shape(1), shape(4), shape(5), shape(3), shape(2), shape(0)}
			Dim arr As INDArray = Nd4j.rand(dataType, createdShape)
			Dim permuted As INDArray = arr.permute(5, 0, 4, 3, 1, 2)
			Return Collections.singletonList(New Pair(Of )(permuted, "get6dPermutedWithShape(" & seed & "," & java.util.Arrays.toString(shape) & ").get(0)"))
		End Function

		Public Shared Function get6dReshapedWithShape(ByVal seed As Integer, ByVal shape() As Integer, ByVal dataType As DataType) As IList(Of Pair(Of INDArray, String))
			Nd4j.Random.setSeed(seed)
			Dim shape3d() As Integer = {shape(0) * shape(2), shape(4) * shape(5), shape(1) * shape(3)}
			Dim array3d As INDArray = Nd4j.rand(dataType, shape3d)
			Dim array6d As INDArray = array3d.reshape(ArrayUtil.toLongArray(shape))
			Return Collections.singletonList(New Pair(Of )(array6d, "get6dReshapedWithShape(" & seed & "," & java.util.Arrays.toString(shape) & ").get(0)"))
		End Function


		''' <summary>
		''' Create an ndarray
		''' of </summary>
		''' <param name="seed"> </param>
		''' <param name="rank"> </param>
		''' <param name="numShapes">
		''' @return </param>
		Public Shared Function getRandomBroadCastShape(ByVal seed As Long, ByVal rank As Integer, ByVal numShapes As Integer) As Integer()()
			Nd4j.Random.Seed = seed
			Dim coinFlip As INDArray = Nd4j.Distributions.createBinomial(1, 0.5).sample(New Integer() {numShapes, rank})
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret[][] As Integer = new Integer[CInt(coinFlip.rows())][CInt(coinFlip.columns())]
			Dim ret()() As Integer = RectangularArrays.RectangularIntegerArray(CInt(coinFlip.rows()), CInt(coinFlip.columns()))
			Dim i As Integer = 0
			Do While i < coinFlip.rows()
				Dim j As Integer = 0
				Do While j < coinFlip.columns()
					Dim set As Integer = coinFlip.getInt(i, j)
					If set > 0 Then
						ret(i)(j) = set
					Else
						'anything from 0 to 9
						ret(i)(j) = Nd4j.Random.nextInt(9) + 1
					End If
					j += 1
				Loop
				i += 1
			Loop

			Return ret
		End Function


		''' <summary>
		''' Generate a random shape to
		''' broadcast to
		''' given a randomly generated
		''' shape with 1s in it as inputs </summary>
		''' <param name="inputShapeWithOnes"> </param>
		''' <param name="seed">
		''' @return </param>
		Public Shared Function broadcastToShape(ByVal inputShapeWithOnes() As Integer, ByVal seed As Long) As Integer()
			Nd4j.Random.Seed = seed
			Dim shape(inputShapeWithOnes.Length - 1) As Integer
			For i As Integer = 0 To shape.Length - 1
				If inputShapeWithOnes(i) = 1 Then
					shape(i) = Nd4j.Random.nextInt(9) + 1
				Else
					shape(i) = inputShapeWithOnes(i)
				End If
			Next i

			Return shape
		End Function

		Public Shared Function broadcastToShape(ByVal inputShapeWithOnes() As Long, ByVal seed As Long) As Long()
			Nd4j.Random.Seed = seed
			Dim shape As val = New Long(inputShapeWithOnes.Length - 1){}
			For i As Integer = 0 To shape.length - 1
				If inputShapeWithOnes(i) = 1 Then
					shape(i) = Nd4j.Random.nextInt(9) + 1
				Else
					shape(i) = inputShapeWithOnes(i)
				End If
			Next i

			Return shape
		End Function

	End Class

End Namespace