Imports System
Imports System.Collections.Generic
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports org.nd4j.linalg.api.blas
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeEx = org.nd4j.linalg.api.buffer.DataTypeEx
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Distribution = org.nd4j.linalg.api.rng.distribution.Distribution

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

Namespace org.nd4j.linalg.factory



	Public Interface NDArrayFactory


'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in VB:
'		char FORTRAN = "f"c;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in VB:
'		char C = "c"c;

		''' <summary>
		''' Return extra blas operations
		''' @return
		''' </summary>
		Function blas() As Blas

		Function lapack() As Lapack

		''' <summary>
		''' Return the level 1 blas operations
		''' @return
		''' </summary>
		Function level1() As Level1

		''' <summary>
		''' Return the level 2 blas operations
		''' @return
		''' </summary>
		Function level2() As Level2

		''' <summary>
		''' Return the level 3 blas operations
		''' @return
		''' </summary>
		Function level3() As Level3

		''' <summary>
		''' Create blas
		''' </summary>
		Sub createBlas()

		''' <summary>
		''' Create level 1
		''' </summary>
		Sub createLevel1()

		''' <summary>
		''' Create level 2
		''' </summary>
		Sub createLevel2()

		''' <summary>
		''' Create level 3
		''' </summary>
		Sub createLevel3()

		''' <summary>
		''' Create lapack
		''' </summary>
		Sub createLapack()

		''' <summary>
		''' Sets the order. Primarily for testing purposes
		''' </summary>
		''' <param name="order"> </param>
		WriteOnly Property Order As Char

		''' <summary>
		''' Sets the data opType
		''' </summary>
		''' <param name="dtype"> </param>
		WriteOnly Property DType As DataType

		''' <summary>
		''' Create an ndarray with the given shape
		''' and data </summary>
		''' <param name="shape"> the shape to use </param>
		''' <param name="buffer"> the buffer to use </param>
		''' <returns> the ndarray </returns>
		Function create(ByVal shape() As Integer, ByVal buffer As DataBuffer) As INDArray

		''' <summary>
		''' Returns the order for this ndarray for internal data storage
		''' </summary>
		''' <returns> the order (c or f) </returns>
		Function order() As Char

		''' <summary>
		''' Returns the data opType for this ndarray
		''' </summary>
		''' <returns> the data opType for this ndarray </returns>
		Function dtype() As DataType

		''' <summary>
		''' /**
		''' Returns a flattened ndarray with all of the elements in each ndarray
		''' regardless of dimension
		''' </summary>
		''' <param name="matrices"> the ndarrays to use </param>
		''' <returns> a flattened ndarray of the elements in the order of titerating over the ndarray and the linear view of
		''' each </returns>
		Function toFlattened(ByVal matrices As ICollection(Of INDArray)) As INDArray


		''' <summary>
		''' Returns a flattened ndarray with all of the elements in each ndarray
		''' regardless of dimension
		''' </summary>
		''' <param name="matrices"> the ndarrays to use </param>
		''' <returns> a flattened ndarray of the elements in the order of titerating over the ndarray and the linear view of
		''' each </returns>
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: org.nd4j.linalg.api.ndarray.INDArray toFlattened(int length, Iterator<? extends org.nd4j.linalg.api.ndarray.INDArray>... matrices);
		Function toFlattened(Of T1 As INDArray)(ByVal length As Integer, ParamArray ByVal matrices() As IEnumerator(Of T1)) As INDArray

		''' <summary>
		''' Returns a flattened ndarray with all elements in each ndarray
		''' regardless of dimension.
		''' Order is specified to ensure flattening order is consistent across </summary>
		''' <param name="matrices"> the ndarrays to flatten </param>
		''' <param name="order"> the order in which the ndarray values should be flattened
		''' @return </param>
		Function toFlattened(ByVal order As Char, ByVal matrices As ICollection(Of INDArray)) As INDArray

		''' <summary>
		''' Returns a column vector where each entry is the nth bilinear
		''' product of the nth slices of the two tensors.
		''' </summary>
		Function bilinearProducts(ByVal curr As INDArray, ByVal [in] As INDArray) As INDArray

		''' <summary>
		''' Flatten all of the ndarrays in to one long vector
		''' </summary>
		''' <param name="matrices"> the matrices to flatten </param>
		''' <returns> the flattened vector </returns>
		Function toFlattened(ParamArray ByVal matrices() As INDArray) As INDArray

		''' <summary>
		''' Flatten all of the ndarrays in to one long vector
		''' </summary>
		''' <param name="matrices"> the matrices to flatten </param>
		''' <returns> the flattened vector </returns>
		Function toFlattened(ByVal order As Char, ParamArray ByVal matrices() As INDArray) As INDArray

		''' <summary>
		''' Create the identity ndarray
		''' </summary>
		''' <param name="n"> the number for the identity
		''' @return </param>
		Function eye(ByVal n As Long) As INDArray

		''' <summary>
		''' Rotate a matrix 90 degrees
		''' </summary>
		''' <param name="toRotate"> the matrix to rotate </param>
		''' <returns> the rotated matrix </returns>
		Sub rot90(ByVal toRotate As INDArray)

		''' <summary>
		''' Reverses the passed in matrix such that m[0] becomes m[m.length - 1] etc
		''' </summary>
		''' <param name="reverse"> the matrix to reverse </param>
		''' <returns> the reversed matrix </returns>
		Function rot(ByVal reverse As INDArray) As INDArray

		''' <summary>
		''' Reverses the passed in matrix such that m[0] becomes m[m.length - 1] etc
		''' </summary>
		''' <param name="reverse"> the matrix to reverse </param>
		''' <returns> the reversed matrix </returns>
		Function reverse(ByVal reverse As INDArray) As INDArray


		''' <summary>
		''' Array of evenly spaced values.
		''' Returns a row vector
		''' </summary>
		''' <param name="begin"> the begin of the range </param>
		''' <param name="end">   the end of the range </param>
		''' <returns> the range vector </returns>
		Function arange(ByVal begin As Double, ByVal [end] As Double, ByVal [step] As Double) As INDArray


		''' <summary>
		''' Copy a to b
		''' </summary>
		''' <param name="a"> the origin matrix </param>
		''' <param name="b"> the destination matrix </param>
		Sub copy(ByVal a As INDArray, ByVal b As INDArray)

		''' <summary>
		''' Generates a random matrix between min and max
		''' </summary>
		''' <param name="shape"> the number of rows of the matrix </param>
		''' <param name="min">   the minimum number </param>
		''' <param name="max">   the maximum number </param>
		''' <param name="rng">   the rng to use </param>
		''' <returns> a drandom matrix of the specified shape and range </returns>
		Function rand(ByVal shape() As Integer, ByVal min As Single, ByVal max As Single, ByVal rng As org.nd4j.linalg.api.rng.Random) As INDArray

		Function rand(ByVal shape() As Long, ByVal min As Single, ByVal max As Single, ByVal rng As org.nd4j.linalg.api.rng.Random) As INDArray

		''' <summary>
		''' Generates a random matrix between min and max
		''' </summary>
		''' <param name="rows">    the number of rows of the matrix </param>
		''' <param name="columns"> the number of columns in the matrix </param>
		''' <param name="min">     the minimum number </param>
		''' <param name="max">     the maximum number </param>
		''' <param name="rng">     the rng to use </param>
		''' <returns> a drandom matrix of the specified shape and range </returns>
		Function rand(ByVal rows As Long, ByVal columns As Long, ByVal min As Single, ByVal max As Single, ByVal rng As org.nd4j.linalg.api.rng.Random) As INDArray

		Function appendBias(ParamArray ByVal vectors() As INDArray) As INDArray

		''' <summary>
		''' Create an ndarray with the given data layout
		''' </summary>
		''' <param name="data"> the data to create the ndarray with </param>
		''' <returns> the ndarray with the given data layout </returns>
		Function create(ByVal data()() As Double) As INDArray

		''' <summary>
		''' Create a matrix from the given
		''' data and ordering </summary>
		''' <param name="data"> </param>
		''' <param name="ordering">
		''' @return </param>
		Function create(ByVal data()() As Double, ByVal ordering As Char) As INDArray


		''' <summary>
		''' Concatneate ndarrays along a dimension
		''' </summary>
		''' <param name="dimension"> the dimension to concatneate along </param>
		''' <param name="toConcat">  the ndarrays to concateneate </param>
		''' <returns> the concatneated ndarrays </returns>
		Function concat(ByVal dimension As Integer, ParamArray ByVal toConcat() As INDArray) As INDArray

		''' <summary>
		''' Concatenate ndarrays along a dimension
		''' 
		''' PLEASE NOTE: This method is special for GPU backend, it works on HOST side only.
		''' </summary>
		''' <param name="dimension"> the dimension to concatneate along </param>
		''' <param name="toConcat">  the ndarrays to concateneate </param>
		''' <returns> the concatneated ndarrays </returns>
		Function specialConcat(ByVal dimension As Integer, ParamArray ByVal toConcat() As INDArray) As INDArray

		''' <summary>
		''' This method produces concatenated array, that consist from tensors, fetched from source array, against some dimension and specified indexes
		''' </summary>
		''' <param name="source"> source tensor </param>
		''' <param name="sourceDimension"> dimension of source tensor </param>
		''' <param name="indexes"> indexes from source array
		''' @return </param>
		Function pullRows(ByVal source As INDArray, ByVal sourceDimension As Integer, ByVal indexes() As Integer) As INDArray
		Function pullRows(ByVal source As INDArray, ByVal sourceDimension As Integer, ByVal indexes() As Long) As INDArray

		''' <summary>
		''' This method produces concatenated array, that consist from tensors,
		''' fetched from source array, against some dimension and specified indexes
		''' </summary>
		''' <param name="source"> source tensor </param>
		''' <param name="sourceDimension"> dimension of source tensor </param>
		''' <param name="indexes"> indexes from source array </param>
		''' <param name="order"> order for result array
		''' @return </param>
		Function pullRows(ByVal source As INDArray, ByVal sourceDimension As Integer, ByVal indexes() As Integer, ByVal order As Char) As INDArray


		''' <summary>
		''' * This method produces concatenated array, that consist from tensors,
		''' fetched from source array, against some dimension and specified indexes
		''' in to the destination array </summary>
		''' <param name="source"> </param>
		''' <param name="destination"> </param>
		''' <param name="sourceDimension"> </param>
		''' <param name="indexes">
		''' @return </param>
		Function pullRows(ByVal source As INDArray, ByVal destination As INDArray, ByVal sourceDimension As Integer, ByVal indexes() As Integer) As INDArray

		''' <summary>
		''' In place shuffle of an ndarray
		''' along a specified set of dimensions
		''' </summary>
		''' <param name="array"> the ndarray to shuffle </param>
		''' <param name="dimension"> the dimension to do the shuffle
		''' @return </param>
		Sub shuffle(ByVal array As INDArray, ByVal rnd As Random, ParamArray ByVal dimension() As Integer)

		''' <summary>
		''' Symmetric in place shuffle of an ndarray
		''' along a specified set of dimensions. All arrays should have equal shapes.
		''' </summary>
		''' <param name="array"> the ndarray to shuffle </param>
		''' <param name="dimension"> the dimension to do the shuffle
		''' @return </param>
		Sub shuffle(ByVal array As ICollection(Of INDArray), ByVal rnd As Random, ParamArray ByVal dimension() As Integer)

		''' <summary>
		''' Symmetric in place shuffle of an ndarray
		''' along a specified set of dimensions. Each array in list should have it's own dimension at the same index of dimensions array
		''' </summary>
		''' <param name="array"> the ndarray to shuffle </param>
		''' <param name="dimensions"> the dimensions to do the shuffle
		''' @return </param>
		Sub shuffle(ByVal array As IList(Of INDArray), ByVal rnd As Random, ByVal dimensions As IList(Of Integer()))

		''' <summary>
		''' This method averages input arrays, and returns averaged array
		''' </summary>
		''' <param name="arrays">
		''' @return </param>
		Function average(ByVal target As INDArray, ByVal arrays() As INDArray) As INDArray

		''' <summary>
		''' This method averages input arrays, and returns averaged array
		''' </summary>
		''' <param name="arrays">
		''' @return </param>
		Function average(ByVal arrays() As INDArray) As INDArray

		''' <summary>
		''' This method averages input arrays, and returns averaged array
		''' </summary>
		''' <param name="arrays">
		''' @return </param>
		Function average(ByVal arrays As ICollection(Of INDArray)) As INDArray


		''' <summary>
		''' This method sums given arrays to target
		''' </summary>
		''' <param name="target"> </param>
		''' <param name="arrays">
		''' @return </param>
		Function accumulate(ByVal target As INDArray, ParamArray ByVal arrays() As INDArray) As INDArray


		''' <summary>
		''' This method averages input arrays, and returns averaged array
		''' </summary>
		''' <param name="arrays">
		''' @return </param>
		Function average(ByVal target As INDArray, ByVal arrays As ICollection(Of INDArray)) As INDArray


		''' <summary>
		''' Create a random ndarray with the given shape using the given rng
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the number of columns in the matrix </param>
		''' <param name="r">       the random generator to use </param>
		''' <returns> the random ndarray with the specified shape </returns>
		Function rand(ByVal rows As Long, ByVal columns As Long, ByVal r As org.nd4j.linalg.api.rng.Random) As INDArray

		''' <summary>
		''' Create a random ndarray with the given shape using the given rng
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <param name="seed">    the  seed to use </param>
		''' <returns> the random ndarray with the specified shape </returns>
		Function rand(ByVal rows As Long, ByVal columns As Long, ByVal seed As Long) As INDArray

		''' <summary>
		''' Create a random ndarray with the given shape using
		''' the current time as the seed
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the number of columns in the matrix </param>
		''' <returns> the random ndarray with the specified shape </returns>
		Function rand(ByVal rows As Long, ByVal columns As Long) As INDArray

		''' <summary>
		''' Create a random (uniform 0-1) NDArray with the specified shape and order </summary>
		''' <param name="order">      Order ('c' or 'f') of the output array </param>
		''' <param name="rows">       Number of rows of the output array </param>
		''' <param name="columns">    Number of columns of the output array </param>
		Function rand(ByVal order As Char, ByVal rows As Long, ByVal columns As Long) As INDArray

		''' <summary>
		''' Random normal using the given rng
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the number of columns in the matrix </param>
		''' <param name="r">       the random generator to use
		''' @return </param>
		Function randn(ByVal rows As Long, ByVal columns As Long, ByVal r As org.nd4j.linalg.api.rng.Random) As INDArray

		''' <summary>
		''' Random normal (N(0,1)) using the current time stamp
		''' as the seed
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the number of columns in the matrix
		''' @return </param>
		Function randn(ByVal rows As Long, ByVal columns As Long) As INDArray

		''' <summary>
		''' Random normal N(0,1), with specified output order
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the number of columns in the matrix </param>
		Function randn(ByVal order As Char, ByVal rows As Long, ByVal columns As Long) As INDArray

		''' <summary>
		''' Random normal using the specified seed
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the number of columns in the matrix
		''' @return </param>
		Function randn(ByVal rows As Long, ByVal columns As Long, ByVal seed As Long) As INDArray


		''' <summary>
		''' Create a random ndarray with the given shape using the given rng
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <param name="r">     the random generator to use </param>
		''' <returns> the random ndarray with the specified shape </returns>
		Function rand(ByVal shape() As Integer, ByVal r As Distribution) As INDArray

		''' <summary>
		''' Create a random ndarray with the given shape using the given rng
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <param name="r">     the random generator to use </param>
		''' <returns> the random ndarray with the specified shape </returns>
		Function rand(ByVal shape() As Integer, ByVal r As org.nd4j.linalg.api.rng.Random) As INDArray

		Function rand(ByVal shape() As Long, ByVal r As org.nd4j.linalg.api.rng.Random) As INDArray

		''' <summary>
		''' Create a random ndarray with the given shape using the given rng
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <param name="seed">  the  seed to use </param>
		''' <returns> the random ndarray with the specified shape </returns>
		Function rand(ByVal shape() As Integer, ByVal seed As Long) As INDArray

		Function rand(ByVal shape() As Long, ByVal seed As Long) As INDArray

		''' <summary>
		''' Create a random ndarray with the given shape using
		''' the current time as the seed
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> the random ndarray with the specified shape </returns>
		Function rand(ByVal shape() As Integer) As INDArray


		''' <summary>
		''' Create a random ndarray with the given shape using
		''' the current time as the seed
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> the random ndarray with the specified shape </returns>
		Function rand(ByVal shape() As Long) As INDArray

		''' <summary>
		''' Create a random ndarray with the given shape, and specified output order
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> the random ndarray with the specified shape </returns>
		Function rand(ByVal order As Char, ByVal shape() As Integer) As INDArray

		''' <summary>
		''' Create a random ndarray with the given shape
		''' and specified output order </summary>
		''' <param name="order"> the order of the array </param>
		''' <param name="shape"> the shape of the array </param>
		''' <returns> the created ndarray </returns>
		Function rand(ByVal order As Char, ByVal shape() As Long) As INDArray

		''' <summary>
		''' Random normal using the given rng
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <param name="r">     the random generator to use </param>
		Function randn(ByVal shape() As Integer, ByVal r As org.nd4j.linalg.api.rng.Random) As INDArray

		Function randn(ByVal shape() As Long, ByVal r As org.nd4j.linalg.api.rng.Random) As INDArray

		''' <summary>
		''' Random normal N(0,1) using the current time stamp
		''' as the seed
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		Function randn(ByVal shape() As Integer) As INDArray

		''' <summary>
		''' Random normal N(0,1) using the current time stamp
		''' as the seed
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		Function randn(ByVal shape() As Long) As INDArray

		''' <summary>
		''' Random normal N(0,1) with the specified shape and order
		''' </summary>
		''' <param name="order"> the order ('c' or 'f') of the output array </param>
		''' <param name="shape"> the shape of the ndarray </param>
		Function randn(ByVal order As Char, ByVal shape() As Integer) As INDArray

		''' <summary>
		''' Random normal N(0,1) with the specified shape and order
		''' </summary>
		''' <param name="order"> the order ('c' or 'f') of the output array </param>
		''' <param name="shape"> the shape of the ndarray </param>
		Function randn(ByVal order As Char, ByVal shape() As Long) As INDArray

		''' <summary>
		''' Random normal using the specified seed
		''' </summary>
		''' <param name="shape"> the shape of the ndarray
		''' @return </param>
		Function randn(ByVal shape() As Integer, ByVal seed As Long) As INDArray


		''' <summary>
		''' Random normal using the specified seed
		''' </summary>
		''' <param name="shape"> the shape of the ndarray
		''' @return </param>
		Function randn(ByVal shape() As Long, ByVal seed As Long) As INDArray


		''' <summary>
		''' Creates a row vector with the data
		''' </summary>
		''' <param name="data"> the columns of the ndarray </param>
		''' <returns> the created ndarray </returns>
		Function create(ByVal data() As Double) As INDArray

		''' <summary>
		''' Creates a row vector with the data
		''' </summary>
		''' <param name="data"> the columns of the ndarray </param>
		''' <returns> the created ndarray </returns>
		Function create(ByVal data() As Single) As INDArray


		''' <summary>
		''' Creates a row vector with the data
		''' </summary>
		''' <param name="data"> the columns of the ndarray </param>
		''' <returns> the created ndarray </returns>
		Function create(ByVal data As DataBuffer) As INDArray


		''' <summary>
		''' Creates a row vector with the specified number of columns
		''' </summary>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <returns> the created ndarray </returns>
		Function create(ByVal columns As Long) As INDArray

		''' <summary>
		''' Creates a row vector with the specified number of columns
		''' </summary>
		''' <param name="rows">    the rows of the ndarray </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <returns> the created ndarray </returns>
		Function zeros(ByVal rows As Long, ByVal columns As Long) As INDArray


		''' <summary>
		''' Creates a row vector with the specified number of columns
		''' </summary>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <returns> the created ndarray </returns>
		Function zeros(ByVal columns As Long) As INDArray


		''' <summary>
		''' Creates an ndarray with the specified value
		''' as the  only value in the ndarray
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <param name="value"> the value to assign </param>
		''' <returns> the created ndarray </returns>
		Function valueArrayOf(ByVal shape() As Integer, ByVal value As Double) As INDArray

		Function valueArrayOf(ByVal shape() As Long, ByVal value As Double) As INDArray


		''' <summary>
		''' Creates a row vector with the specified number of columns
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <param name="value">   the value to assign </param>
		''' <returns> the created ndarray </returns>
		Function valueArrayOf(ByVal rows As Long, ByVal columns As Long, ByVal value As Double) As INDArray


		''' <summary>
		''' Creates a row vector with the specified number of columns
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <returns> the created ndarray </returns>
		Function ones(ByVal rows As Long, ByVal columns As Long) As INDArray

		''' <summary>
		''' Creates a row vector with the specified number of columns
		''' </summary>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <returns> the created ndarray </returns>
		Function ones(ByVal columns As Long) As INDArray


		''' <summary>
		''' Concatenates two matrices horizontally. Matrices must have identical
		''' numbers of rows.
		''' </summary>
		''' <param name="arrs"> </param>
		Function hstack(ParamArray ByVal arrs() As INDArray) As INDArray

		''' <summary>
		''' Concatenates two matrices vertically. Matrices must have identical
		''' numbers of columns.
		''' </summary>
		''' <param name="arrs"> </param>
		Function vstack(ParamArray ByVal arrs() As INDArray) As INDArray


		''' <summary>
		''' Create an ndarray of zeros
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> an ndarray with ones filled in </returns>
		Function zeros(ByVal shape() As Integer) As INDArray

		Function zeros(ByVal shape() As Long) As INDArray

		''' <summary>
		''' Create an ndarray of ones
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> an ndarray with ones filled in </returns>
		Function ones(ByVal shape() As Integer) As INDArray

		Function ones(ByVal shape() As Long) As INDArray

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="data">    the data to use with the ndarray </param>
		''' <param name="rows">    the rows of the ndarray </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <param name="stride">  the stride for the ndarray </param>
		''' <param name="offset">  the offset of the ndarray </param>
		''' <returns> the instance </returns>
		Function create(ByVal data As DataBuffer, ByVal rows As Long, ByVal columns As Long, ByVal stride() As Integer, ByVal offset As Long) As INDArray


		''' <param name="data"> </param>
		''' <param name="rows"> </param>
		''' <param name="columns"> </param>
		''' <param name="stride"> </param>
		''' <param name="offset">
		''' @return </param>
		Function create(ByVal data() As Single, ByVal rows As Long, ByVal columns As Long, ByVal stride() As Integer, ByVal offset As Long) As INDArray


		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape">  the shape of the ndarray </param>
		''' <param name="stride"> the stride for the ndarray </param>
		''' <param name="offset"> the offset of the ndarray </param>
		''' <returns> the instance </returns>
		Function create(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long) As INDArray

		Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long) As INDArray

		Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType) As INDArray

		''' <summary>
		''' Create an ndrray with the specified shape
		''' </summary>
		''' <param name="data">  the data to use with tne ndarray </param>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> the created ndarray </returns>
		Function create(ByVal data() As Double, ByVal shape() As Integer) As INDArray

		Function create(ByVal data() As Double, ByVal shape() As Long) As INDArray
		Function create(ByVal data() As Single, ByVal shape() As Long) As INDArray

		''' <summary>
		''' Create an ndrray with the specified shape
		''' </summary>
		''' <param name="data">  the data to use with tne ndarray </param>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> the created ndarray </returns>
		Function create(ByVal data() As Single, ByVal shape() As Integer) As INDArray

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="data">    the data to use with tne ndarray </param>
		''' <param name="rows">    the rows of the ndarray </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <param name="stride">  the stride for the ndarray </param>
		''' <param name="offset">  the offset of the ndarray </param>
		''' <returns> the instance </returns>
		Function create(ByVal data() As Double, ByVal rows As Long, ByVal columns As Long, ByVal stride() As Integer, ByVal offset As Long) As INDArray


		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape">  the shape of the ndarray </param>
		''' <param name="stride"> the stride for the ndarray </param>
		''' <param name="offset"> the offset of the ndarray </param>
		''' <returns> the instance </returns>
		Function create(ByVal data() As Double, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long) As INDArray

		Function create(ByVal data() As Double, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long) As INDArray

		Function create(ByVal data() As Double, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Function create(ByVal data() As Long, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Function create(ByVal data() As Integer, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Function create(ByVal data() As Short, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Function create(ByVal data() As SByte, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Function create(ByVal data() As Boolean, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray


		Function create(ByVal data() As Double, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Function create(ByVal data() As Long, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Function create(ByVal data() As Integer, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Function create(ByVal data() As Short, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Function create(ByVal data() As SByte, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Function create(ByVal data() As Boolean, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray


		''' <summary>
		''' Create an ndrray with the specified shape
		''' </summary>
		''' <param name="data">  the data to use with tne ndarray </param>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> the created ndarray </returns>
		Function create(ByVal data As DataBuffer, ByVal shape() As Integer) As INDArray


		Function create(ByVal data As DataBuffer, ByVal shape() As Long) As INDArray


		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape">  the shape of the ndarray </param>
		''' <param name="stride"> the stride for the ndarray </param>
		''' <param name="offset"> the offset of the ndarray </param>
		''' <returns> the instance </returns>
		Function create(ByVal data As DataBuffer, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long) As INDArray


		Function create(ByVal data As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long) As INDArray


		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> the instance </returns>
		Function create(ByVal list As IList(Of INDArray), ByVal shape() As Integer) As INDArray

		Function create(ByVal list As IList(Of INDArray), ByVal shape() As Long) As INDArray

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="rows">    the rows of the ndarray </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <param name="stride">  the stride for the ndarray </param>
		''' <param name="offset">  the offset of the ndarray </param>
		''' <returns> the instance </returns>
		Function create(ByVal rows As Long, ByVal columns As Long, ByVal stride() As Integer, ByVal offset As Long) As INDArray

		Function create(ByVal rows As Long, ByVal columns As Long, ByVal stride() As Long, ByVal offset As Long) As INDArray

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape">  the shape of the ndarray </param>
		''' <param name="stride"> the stride for the ndarray </param>
		''' <param name="offset"> the offset of the ndarray </param>
		''' <returns> the instance </returns>
		Function create(ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long) As INDArray


		Function create(ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long) As INDArray

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="rows">    the rows of the ndarray </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <param name="stride">  the stride for the ndarray </param>
		''' <returns> the instance </returns>
		Function create(ByVal rows As Long, ByVal columns As Long, ByVal stride() As Integer) As INDArray

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape">  the shape of the ndarray </param>
		''' <param name="stride"> the stride for the ndarray </param>
		''' <returns> the instance </returns>
		Function create(ByVal shape() As Integer, ByVal stride() As Integer) As INDArray

		Function create(ByVal shape() As Long, ByVal stride() As Long) As INDArray

		Function create(ByVal shape() As Long) As INDArray

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="rows">    the rows of the ndarray </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <returns> the instance </returns>
		Function create(ByVal rows As Long, ByVal columns As Long) As INDArray

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> the instance </returns>
		Function create(ByVal shape() As Integer) As INDArray

		''' <summary>
		''' Create a scalar nd array with the specified value and offset
		''' </summary>
		''' <param name="value">  the value of the scalar </param>
		''' <param name="offset"> the offset of the ndarray </param>
		''' <returns> the scalar nd array </returns>
		Function scalar(ByVal value As Single, ByVal offset As Long) As INDArray

		''' <summary>
		''' Create a scalar nd array with the specified value and offset
		''' </summary>
		''' <param name="value">  the value of the scalar </param>
		''' <param name="offset"> the offset of the ndarray </param>
		''' <returns> the scalar nd array </returns>
		Function scalar(ByVal value As Double, ByVal offset As Long) As INDArray


		Function scalar(ByVal value As Integer, ByVal offset As Long) As INDArray

		''' <summary>
		''' Create a scalar ndarray with the specified offset
		''' </summary>
		''' <param name="value"> the value to initialize the scalar with </param>
		''' <returns> the created ndarray </returns>
		Function scalar(ByVal value As Number) As INDArray

		Function empty(ByVal type As DataType) As INDArray

		''' <summary>
		''' Create a scalar nd array with the specified value and offset
		''' </summary>
		''' <param name="value"> the value of the scalar </param>
		'''              =     * <returns> the scalar nd array </returns>
		Function scalar(ByVal value As Single) As INDArray

		''' <summary>
		''' Create a scalar nd array with the specified value and offset
		''' </summary>
		''' <param name="value"> the value of the scalar </param>
		'''              =     * <returns> the scalar nd array </returns>
		Function scalar(ByVal value As Double) As INDArray

		''' 
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		''' <param name="offset">
		''' @return </param>
		Function create(ByVal data() As Single, ByVal shape() As Integer, ByVal offset As Long) As INDArray

		''' 
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		''' <param name="ordering">
		''' @return </param>
		Function create(ByVal data() As Single, ByVal shape() As Integer, ByVal ordering As Char) As INDArray

		''' 
		''' <param name="floats">
		''' @return </param>
		Function create(ByVal floats()() As Single) As INDArray

		Function create(ByVal data()() As Single, ByVal ordering As Char) As INDArray

		''' 
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		''' <param name="stride"> </param>
		''' <param name="offset"> </param>
		''' <param name="ordering">
		''' @return </param>
		Function create(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char) As INDArray

		''' 
		''' <param name="buffer"> </param>
		''' <param name="shape"> </param>
		''' <param name="offset">
		''' @return </param>
		Function create(ByVal buffer As DataBuffer, ByVal shape() As Integer, ByVal offset As Long) As INDArray

		''' 
		''' <param name="shape"> </param>
		''' <param name="ordering">
		''' @return </param>
		Function create(ByVal shape() As Integer, ByVal ordering As Char) As INDArray


		Function create(ByVal shape() As Long, ByVal ordering As Char) As INDArray

		Function create(ByVal dataType As DataType, ByVal shape() As Long, ByVal ordering As Char, ByVal workspace As MemoryWorkspace) As INDArray

		Function create(ByVal dataType As DataType, ByVal shape() As Long, ByVal strides() As Long, ByVal ordering As Char, ByVal workspace As MemoryWorkspace) As INDArray

	   ''' <summary>
	   ''' Create an ndArray with padded Buffer </summary>
	   ''' <param name="dataType"> </param>
	   ''' <param name="shape"> </param>
	   ''' <param name="paddings"> </param>
	   ''' <param name="paddingOffsets"> </param>
	   ''' <param name="ordering"> Fortran 'f' or C/C++ 'c' ordering. </param>
	   ''' <param name="workspace">
	   ''' @return </param>
		Function create(ByVal dataType As DataType, ByVal shape() As Long, ByVal paddings() As Long, ByVal paddingOffsets() As Long, ByVal ordering As Char, ByVal workspace As MemoryWorkspace) As INDArray

		Function createUninitialized(ByVal shape() As Integer, ByVal ordering As Char) As INDArray

		Function createUninitialized(ByVal shape() As Long, ByVal ordering As Char) As INDArray

		Function createUninitialized(ByVal dataType As DataType, ByVal shape() As Long, ByVal ordering As Char, ByVal workspace As MemoryWorkspace) As INDArray

		''' <summary>
		''' Create an uninitialized ndArray. Detached from workspace. </summary>
		''' <param name="dataType"> data type. Exceptions will be thrown for UTF8, COMPRESSED and UNKNOWN data types. </param>
		''' <param name="ordering">  Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <param name="shape"> the shape of the array. </param>
		''' <returns> the created detached array. </returns>
		Function createUninitializedDetached(ByVal dataType As DataType, ByVal ordering As Char, ParamArray ByVal shape() As Long) As INDArray

		''' 
		''' <param name="data"> </param>
		''' <param name="newShape"> </param>
		''' <param name="newStride"> </param>
		''' <param name="offset"> </param>
		''' <param name="ordering">
		''' @return </param>
		Function create(ByVal data As DataBuffer, ByVal newShape() As Integer, ByVal newStride() As Integer, ByVal offset As Long, ByVal ordering As Char) As INDArray

		Function create(ByVal data As DataBuffer, ByVal newShape() As Long, ByVal newStride() As Long, ByVal offset As Long, ByVal ordering As Char) As INDArray

		Function create(ByVal data As DataBuffer, ByVal newShape() As Long, ByVal newStride() As Long, ByVal offset As Long, ByVal ews As Long, ByVal ordering As Char) As INDArray

		Function create(ByVal data As DataBuffer, ByVal newShape() As Long, ByVal newStride() As Long, ByVal offset As Long, ByVal ordering As Char, ByVal dataType As DataType) As INDArray

		''' 
		''' <param name="rows"> </param>
		''' <param name="columns"> </param>
		''' <param name="min"> </param>
		''' <param name="max"> </param>
		''' <param name="rng">
		''' @return </param>
		Function rand(ByVal rows As Long, ByVal columns As Long, ByVal min As Double, ByVal max As Double, ByVal rng As org.nd4j.linalg.api.rng.Random) As INDArray

		''' 
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		''' <param name="offset"> </param>
		''' <param name="order">
		''' @return </param>
		Function create(ByVal data() As Single, ByVal shape() As Integer, ByVal offset As Long, ByVal order As Char?) As INDArray

		''' 
		''' <param name="data"> </param>
		''' <param name="rows"> </param>
		''' <param name="columns"> </param>
		''' <param name="stride"> </param>
		''' <param name="offset"> </param>
		''' <param name="ordering">
		''' @return </param>
		Function create(ByVal data() As Single, ByVal rows As Long, ByVal columns As Long, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char) As INDArray

		''' 
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		''' <param name="ordering">
		''' @return </param>
		Function create(ByVal data() As Double, ByVal shape() As Integer, ByVal ordering As Char) As INDArray

		''' 
		''' <param name="list"> </param>
		''' <param name="shape"> </param>
		''' <param name="ordering">
		''' @return </param>
		Function create(ByVal list As IList(Of INDArray), ByVal shape() As Integer, ByVal ordering As Char) As INDArray

		Function create(ByVal list As IList(Of INDArray), ByVal shape() As Long, ByVal ordering As Char) As INDArray

		''' 
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		''' <param name="offset">
		''' @return </param>
		Function create(ByVal data() As Double, ByVal shape() As Integer, ByVal offset As Long) As INDArray

		''' 
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		''' <param name="stride"> </param>
		''' <param name="offset"> </param>
		''' <param name="ordering">
		''' @return </param>
		Function create(ByVal data() As Double, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char) As INDArray

		''' 
		''' <param name="shape"> </param>
		''' <param name="min"> </param>
		''' <param name="max"> </param>
		''' <param name="rng">
		''' @return </param>
		Function rand(ByVal shape() As Integer, ByVal min As Double, ByVal max As Double, ByVal rng As org.nd4j.linalg.api.rng.Random) As INDArray

		Function rand(ByVal shape() As Long, ByVal min As Double, ByVal max As Double, ByVal rng As org.nd4j.linalg.api.rng.Random) As INDArray

		''' 
		''' <param name="ints"> </param>
		''' <param name="ints1"> </param>
		''' <param name="stride"> </param>
		''' <param name="offset">
		''' @return </param>
		Function create(ByVal ints() As Integer, ByVal ints1() As Integer, ByVal stride() As Integer, ByVal offset As Long) As INDArray

		''' 
		''' <param name="shape"> </param>
		''' <param name="ints1"> </param>
		''' <param name="stride"> </param>
		''' <param name="order"> </param>
		''' <param name="offset">
		''' @return </param>
		Function create(ByVal shape() As Integer, ByVal ints1() As Integer, ByVal stride() As Integer, ByVal order As Char, ByVal offset As Long) As INDArray

		''' 
		''' <param name="rows"> </param>
		''' <param name="columns"> </param>
		''' <param name="ordering">
		''' @return </param>
		Function create(ByVal rows As Long, ByVal columns As Long, ByVal ordering As Char) As INDArray

		''' 
		''' <param name="shape"> </param>
		''' <param name="dataType">
		''' @return </param>
		Function create(ByVal shape() As Integer, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray

		''' 
		''' <param name="data"> </param>
		''' <param name="order">
		''' @return </param>
		Function create(ByVal data() As Single, ByVal order As Char) As INDArray

		''' 
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		''' <param name="stride"> </param>
		''' <param name="order"> </param>
		''' <param name="offset">
		''' @return </param>
		Function create(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer, ByVal order As Char, ByVal offset As Long) As INDArray

		Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal offset As Long) As INDArray

		''' 
		''' <param name="buffer"> </param>
		''' <param name="shape"> </param>
		''' <param name="stride"> </param>
		''' <param name="order"> </param>
		''' <param name="offset">
		''' @return </param>
		Function create(ByVal buffer As DataBuffer, ByVal shape() As Integer, ByVal stride() As Integer, ByVal order As Char, ByVal offset As Long) As INDArray

		''' 
		''' <param name="data"> </param>
		''' <param name="order">
		''' @return </param>
		Function create(ByVal data() As Double, ByVal order As Char) As INDArray

		''' 
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		''' <param name="stride"> </param>
		''' <param name="order"> </param>
		''' <param name="offset">
		''' @return </param>
		Function create(ByVal data() As Double, ByVal shape() As Integer, ByVal stride() As Integer, ByVal order As Char, ByVal offset As Long) As INDArray

		''' 
		''' <param name="shape"> </param>
		''' <param name="stride"> </param>
		''' <param name="offset"> </param>
		''' <param name="ordering">
		''' @return </param>
		Function create(ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char) As INDArray

		Function create(ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char) As INDArray


		'    DataBuffer restoreFromHalfs(DataBuffer buffer);


		'    DataBuffer convertToHalfs(DataBuffer buffer);

		''' 
		''' <param name="typeSrc"> </param>
		''' <param name="source"> </param>
		''' <param name="typeDst">
		''' @return </param>

		Function convertDataEx(ByVal typeSrc As DataTypeEx, ByVal source As INDArray, ByVal typeDst As DataTypeEx) As INDArray

		''' 
		''' <param name="typeSrc"> </param>
		''' <param name="source"> </param>
		''' <param name="typeDst">
		''' @return </param>
		Function convertDataEx(ByVal typeSrc As DataTypeEx, ByVal source As DataBuffer, ByVal typeDst As DataTypeEx) As DataBuffer

		''' 
		''' <param name="typeSrc"> </param>
		''' <param name="source"> </param>
		''' <param name="typeDst"> </param>
		''' <param name="target"> </param>
		Sub convertDataEx(ByVal typeSrc As DataTypeEx, ByVal source As DataBuffer, ByVal typeDst As DataTypeEx, ByVal target As DataBuffer)

		''' 
		''' <param name="typeSrc"> </param>
		''' <param name="source"> </param>
		''' <param name="typeDst"> </param>
		''' <param name="target"> </param>
		''' <param name="length"> </param>
		Sub convertDataEx(ByVal typeSrc As DataTypeEx, ByVal source As Pointer, ByVal typeDst As DataTypeEx, ByVal target As Pointer, ByVal length As Long)

		''' 
		''' <param name="typeSrc"> </param>
		''' <param name="source"> </param>
		''' <param name="typeDst"> </param>
		''' <param name="buffer"> </param>
		Sub convertDataEx(ByVal typeSrc As DataTypeEx, ByVal source As Pointer, ByVal typeDst As DataTypeEx, ByVal buffer As DataBuffer)

		''' <summary>
		''' Create from an in memory numpy pointer </summary>
		''' <param name="pointer"> the pointer to the
		'''                numpy array </param>
		''' <returns> an ndarray created from the in memory
		''' numpy pointer </returns>
		Function createFromNpyPointer(ByVal pointer As Pointer) As INDArray


		''' <summary>
		''' Create from an in memory numpy header.
		''' Use this when not interopping
		''' directly from python.
		''' </summary>
		''' <param name="pointer"> the pointer to the
		'''                numpy header </param>
		''' <returns> an ndarray created from the in memory
		''' numpy pointer </returns>
		Function createFromNpyHeaderPointer(ByVal pointer As Pointer) As INDArray

		''' <summary>
		''' Create from a given numpy file. </summary>
		''' <param name="file"> the file to create the ndarray from </param>
		''' <returns> the created ndarray </returns>
		Function createFromNpyFile(ByVal file As File) As INDArray

		''' <summary>
		''' Create a Map<String, INDArray> from given npz file. </summary>
		''' <param name="file"> the file to create the map from </param>
		''' <returns> Map<String, INDArray> </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public Map<String, org.nd4j.linalg.api.ndarray.INDArray> createFromNpzFile(java.io.File file) throws Exception;
		Function createFromNpzFile(ByVal file As File) As IDictionary(Of String, INDArray)

		''' <summary>
		''' Convert an <seealso cref="INDArray"/>
		''' to a numpy array.
		''' This will usually be used
		''' for writing out to an external source.
		''' Note that this will create a zero copy reference
		''' to this ndarray's underlying data.
		''' 
		''' </summary>
		''' <param name="array"> the array to convert
		''' @returnthe created pointer representing
		''' a pointer to a numpy header </param>
		Function convertToNumpy(ByVal array As INDArray) As Pointer

		Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char) As INDArray

		Function create(ByVal data() As Double, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char) As INDArray


		''' 
		''' <param name="tensor"> </param>
		''' <param name="dimensions">
		''' @return </param>
		Function tear(ByVal tensor As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray()

		''' 
		''' <param name="x"> </param>
		''' <param name="descending">
		''' @return </param>
		Function sort(ByVal x As INDArray, ByVal descending As Boolean) As INDArray

		Function sort(ByVal x As INDArray, ByVal descending As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray

		Function sortCooIndices(ByVal x As INDArray) As INDArray

		Function create(ByVal data() As Single, ByVal shape() As Long, ByVal offset As Long, ByVal order As Char?) As INDArray
		Function create(ByVal data() As Double, ByVal shape() As Long, ByVal offset As Long, ByVal order As Char?) As INDArray
		Function create(ByVal data() As Single, ByVal shape() As Long, ByVal ordering As Char) As INDArray
		Function create(ByVal data() As Double, ByVal shape() As Long, ByVal ordering As Char) As INDArray

		' =========== String methods ============

		Function create(ByVal strings As ICollection(Of String), ByVal shape() As Long, ByVal order As Char) As INDArray
	End Interface

End Namespace