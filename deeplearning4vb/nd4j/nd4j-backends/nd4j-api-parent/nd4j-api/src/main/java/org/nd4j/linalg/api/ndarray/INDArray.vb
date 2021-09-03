Imports System
Imports System.Collections.Generic
Imports FlatBufferBuilder = com.google.flatbuffers.FlatBufferBuilder
Imports NonNull = lombok.NonNull
Imports MMulTranspose = org.nd4j.linalg.api.blas.params.MMulTranspose
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Nd4jNoSuchWorkspaceException = org.nd4j.linalg.exception.Nd4jNoSuchWorkspaceException
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports Condition = org.nd4j.linalg.indexing.conditions.Condition
Imports NDArrayStrings = org.nd4j.linalg.string.NDArrayStrings

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

Namespace org.nd4j.linalg.api.ndarray

	Public Interface INDArray
		Inherits AutoCloseable

		''' <summary>
		''' Returns the shape information debugging information </summary>
		''' <returns> the shape information. </returns>
		Function shapeInfoToString() As String

		''' <summary>
		''' Shape info </summary>
		''' <returns> Shape info </returns>
		Function shapeInfoDataBuffer() As DataBuffer

		''' <summary>
		''' Shape info </summary>
		''' <returns> Shape info </returns>
		Function shapeInfo() As LongBuffer

		''' <summary>
		''' Check if this array is a view or not. </summary>
		''' <returns> true if array is a view. </returns>
		ReadOnly Property View As Boolean

		''' <summary>
		''' Check if this array is sparse </summary>
		''' <returns> true if this array is sparse. </returns>
		ReadOnly Property Sparse As Boolean

		''' <summary>
		''' Check if this array is compressed. </summary>
		''' <returns> true if this array is compressed. </returns>
		ReadOnly Property Compressed As Boolean

		''' <summary>
		''' This method marks INDArray instance as compressed
		''' PLEASE NOTE: Do not use this method unless you 100% have to
		''' </summary>
		''' <param name="reallyCompressed"> new value for compressed. </param>
		Sub markAsCompressed(ByVal reallyCompressed As Boolean)

		''' <summary>
		''' Returns the rank of the ndarray (the number of dimensions).
		''' </summary>
		''' <returns> the rank for the ndarray. </returns>
		Function rank() As Integer

		''' <summary>
		''' Calculate the stride along a particular dimension </summary>
		''' <param name="dimension"> the dimension to get the stride for </param>
		''' <returns> the stride for a particular dimension </returns>
		Function stride(ByVal dimension As Integer) As Integer

		''' <summary>
		''' Element wise stride </summary>
		''' <returns> the element wise stride </returns>
		Function elementWiseStride() As Integer

		' TODO: Unused untested method.
		''' <summary>
		''' Get a double at the given linear offset unsafe, without checks. </summary>
		''' <param name="offset"> the offset to get at </param>
		''' <returns> double value at offset </returns>
		Function getDoubleUnsafe(ByVal offset As Long) As Double

		''' <summary>
		''' Get string value at given index. </summary>
		''' <param name="index"> index to retreive </param>
		''' <returns> string value at index. </returns>
		Function getString(ByVal index As Long) As String

		' TODO: Unused untested method.
		''' <summary>
		''' Insert a scalar at the given linear offset </summary>
		''' <param name="offset"> the offset to insert at </param>
		''' <param name="value"> the value to insert </param>
		''' <returns> this </returns>
		Function putScalarUnsafe(ByVal offset As Long, ByVal value As Double) As INDArray

		''' <summary>
		''' Returns the number of possible vectors for a given dimension
		''' </summary>
		''' <param name="dimension"> the dimension to calculate the number of vectors for </param>
		''' <returns> the number of possible vectors along a dimension </returns>
		Function vectorsAlongDimension(ByVal dimension As Integer) As Long

		''' <summary>
		''' Get the vector along a particular dimension
		''' </summary>
		''' <param name="index">     the index of the vector to getScalar </param>
		''' <param name="dimension"> the dimension to getScalar the vector from </param>
		''' <returns> the vector along a particular dimension </returns>
		Function vectorAlongDimension(ByVal index As Integer, ByVal dimension As Integer) As INDArray

		''' <summary>
		''' Returns the number of possible vectors for a given dimension
		''' </summary>
		''' <param name="dimension"> the dimension to calculate the number of vectors for </param>
		''' <returns> the number of possible vectors along a dimension </returns>
		Function tensorsAlongDimension(ParamArray ByVal dimension() As Integer) As Long

		''' <summary>
		''' Get the vector along a particular dimension
		''' </summary>
		''' <param name="index">     the index of the vector to getScalar </param>
		''' <param name="dimension"> the dimension to getScalar the vector from </param>
		''' <returns> the vector along a particular dimension </returns>
		Function tensorAlongDimension(ByVal index As Long, ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns the cumulative sum along a dimension. In-place method.
		''' </summary>
		''' <param name="dimension"> the dimension to perform cumulative sum along. </param>
		''' <returns> this object. </returns>
		Function cumsumi(ByVal dimension As Integer) As INDArray

		''' <summary>
		''' Returns the cumulative sum along a dimension.
		''' </summary>
		''' <param name="dimension"> the dimension to perform cumulative sum along. </param>
		''' <returns> the cumulative sum along the specified dimension </returns>
		Function cumsum(ByVal dimension As Integer) As INDArray

		''' <summary>
		''' Assign all of the elements in the given ndarray to this ndarray
		''' </summary>
		''' <param name="arr"> the elements to assign </param>
		''' <returns> this </returns>
		Function assign(ByVal arr As INDArray) As INDArray

		' TODO: Unused untested method.
		''' <summary>
		''' Assign all elements from given ndarray that are matching given condition,
		''' ndarray to this ndarray
		''' </summary>
		''' <param name="arr"> the elements to assign </param>
		''' <returns> this </returns>
		Function assignIf(ByVal arr As INDArray, ByVal condition As Condition) As INDArray


		''' <summary>
		''' Replaces all elements in this ndarray that are matching give condition, with corresponding elements from given array
		''' </summary>
		''' <param name="arr">       Source array </param>
		''' <param name="condition"> Condition to apply </param>
		''' <returns> New array with values conditionally replaced </returns>
		Function replaceWhere(ByVal arr As INDArray, ByVal condition As Condition) As INDArray


		''' <summary>
		''' Insert the number linearly in to the ndarray
		''' </summary>
		''' <param name="i">     the index to insert into </param>
		''' <param name="value"> the value to insert </param>
		''' <returns> this </returns>
		Function putScalar(ByVal i As Long, ByVal value As Double) As INDArray

		''' <summary>
		''' Insert a scalar float at the specified index
		''' </summary>
		''' <param name="i">     The index to insert into </param>
		''' <param name="value"> Value to insert </param>
		''' <returns> This array </returns>
		Function putScalar(ByVal i As Long, ByVal value As Single) As INDArray

		''' <summary>
		''' Insert a scalar int at the specified index
		''' </summary>
		''' <param name="i">     The index to insert into </param>
		''' <param name="value"> Value to insert </param>
		''' <returns> This array </returns>
		Function putScalar(ByVal i As Long, ByVal value As Integer) As INDArray

		''' <summary>
		''' Insert the item at the specified indices
		''' </summary>
		''' <param name="i">     the indices to insert at </param>
		''' <param name="value"> the number to insert </param>
		''' <returns> this </returns>
		Function putScalar(ByVal i() As Integer, ByVal value As Double) As INDArray

		''' <summary>
		''' See <seealso cref="putScalar(Integer[], Double)"/>
		''' </summary>
		Function putScalar(ByVal i() As Long, ByVal value As Double) As INDArray

		''' <summary>
		''' See <seealso cref="putScalar(Integer[], Double)"/>
		''' </summary>
		Function putScalar(ByVal i() As Long, ByVal value As Single) As INDArray

		''' <summary>
		''' See <seealso cref="putScalar(Integer[], Double)"/>
		''' </summary>
		Function putScalar(ByVal i() As Long, ByVal value As Integer) As INDArray

		''' <summary>
		''' Insert the value at the specified indices, in a 2d (rank 2) NDArray<br>
		''' Equivalent to <seealso cref="putScalar(Integer[], Double)"/> but avoids int[] creation </summary>
		''' <param name="row">      Row (dimension 0) index </param>
		''' <param name="col">      Column (dimension 1) index </param>
		''' <param name="value">    Value to put </param>
		''' <returns>         This INDArray </returns>
		Function putScalar(ByVal row As Long, ByVal col As Long, ByVal value As Double) As INDArray

		''' <summary>
		''' Insert the value at the specified indices, in a 3d (rank 3) NDArray<br>
		''' Equivalent to <seealso cref="putScalar(Integer[], Double)"/> but avoids int[] creation </summary>
		''' <param name="dim0">     Dimension 0 index </param>
		''' <param name="dim1">     Dimension 1 index </param>
		''' <param name="dim2">     Dimension 2 index </param>
		''' <param name="value">    Value to put </param>
		''' <returns>         This INDArray </returns>
		Function putScalar(ByVal dim0 As Long, ByVal dim1 As Long, ByVal dim2 As Long, ByVal value As Double) As INDArray

		''' <summary>
		''' Insert the value at the specified indices, in a 4d (rank 4) NDArray<br>
		''' Equivalent to <seealso cref="putScalar(Integer[], Double)"/> but avoids int[] creation </summary>
		''' <param name="dim0">     Dimension 0 index </param>
		''' <param name="dim1">     Dimension 1 index </param>
		''' <param name="dim2">     Dimension 2 index </param>
		''' <param name="dim3">     Dimension 3 index </param>
		''' <param name="value">    Value to put </param>
		''' <returns>         This INDArray </returns>
		Function putScalar(ByVal dim0 As Long, ByVal dim1 As Long, ByVal dim2 As Long, ByVal dim3 As Long, ByVal value As Double) As INDArray

		''' <summary>
		''' Returns the binary ndarray for "Less" comparison.
		''' </summary>
		''' <param name="other"> the number to compare. </param>
		''' <returns> the binary ndarray for "Less" comparison. </returns>
		Function lt(ByVal other As Number) As INDArray

		''' <summary>
		''' Put the specified float value at the specified indices in this array
		''' </summary>
		''' <param name="indexes"> Indices to place the value </param>
		''' <param name="value">   Value to insert </param>
		''' <returns> This array </returns>
		Function putScalar(ByVal indexes() As Integer, ByVal value As Single) As INDArray

		''' <summary>
		''' Put the specified integer value at the specified indices in this array
		''' </summary>
		''' <param name="indexes"> Indices to place the value </param>
		''' <param name="value">   Value to insert </param>
		''' <returns> This array </returns>
		Function putScalar(ByVal indexes() As Integer, ByVal value As Integer) As INDArray

		''' <summary>
		''' Returns the binary ndarray for "Epsilon equals" comparison.
		''' </summary>
		''' <param name="other"> the number to compare. </param>
		''' <returns> the binary ndarray for "Epsilon equals" comparison. </returns>
		Function eps(ByVal other As Number) As INDArray

		''' <summary>
		''' Returns the binary ndarray for "Equals" comparison.
		''' </summary>
		''' <param name="other"> the number to compare. </param>
		''' <returns> the binary ndarray for "Equals" comparison. </returns>
		Function eq(ByVal other As Number) As INDArray

		''' <summary>
		''' Returns the binary ndarray for "Greater" comparison.
		''' </summary>
		''' <param name="other"> the number to compare. </param>
		''' <returns> the binary ndarray for "Greater" comparison. </returns>
		Function gt(ByVal other As Number) As INDArray

		''' <summary>
		''' Returns binary ndarray for "Greter or equals" comparison.
		''' </summary>
		''' <param name="other"> the number to compare. </param>
		''' <returns> binary ndarray for "Greter or equals" comparison. </returns>
		Function gte(ByVal other As Number) As INDArray

		''' <summary>
		''' Returns the binary ndarray for "Less or equals" comparison.
		''' </summary>
		''' <param name="other"> the number to compare. </param>
		''' <returns> the binary ndarray for "Less or equals" comparison. </returns>
		Function lte(ByVal other As Number) As INDArray

		''' <summary>
		''' Returns the binary ndarray for "Less" comparison.
		''' </summary>
		''' <param name="other"> the ndarray to compare. </param>
		''' <returns> the binary ndarray for "Less" comparison. </returns>
		Function lt(ByVal other As INDArray) As INDArray

		''' <summary>
		''' Returns the binary ndarray for "Epsilon equals" comparison.
		''' </summary>
		''' <param name="other"> the ndarray to compare. </param>
		''' <returns> the binary ndarray for "Epsilon equals" comparison. </returns>
		Function eps(ByVal other As INDArray) As INDArray

		''' <summary>
		''' Returns the binary ndarray for "Not equals" comparison.
		''' </summary>
		''' <param name="other"> the number to compare. </param>
		''' <returns> the binary ndarray for "Not equals" comparison. </returns>
		Function neq(ByVal other As Number) As INDArray

		''' <summary>
		''' Returns the binary ndarray for "Not equals" comparison.
		''' </summary>
		''' <param name="other"> the ndarray to compare. </param>
		''' <returns> the binary ndarray for "Not equals" comparison. </returns>
		Function neq(ByVal other As INDArray) As INDArray

		''' <summary>
		''' Returns the binary ndarray for "Equals" comparison.
		''' </summary>
		''' <param name="other"> the ndarray to compare. </param>
		''' <returns> the binary ndarray for "Equals" comparison. </returns>
		Function eq(ByVal other As INDArray) As INDArray

		''' <summary>
		''' Returns the binary ndarray for "Greater Than" comparison.
		''' </summary>
		''' <param name="other"> the ndarray to compare. </param>
		''' <returns> the binary ndarray for "Greater Than" comparison. </returns>
		Function gt(ByVal other As INDArray) As INDArray

		''' <summary>
		''' Returns the binary NDArray with value true where this array's entries are infinite, or false where they
		''' are not infinite
		''' </summary>
		ReadOnly Property Infinite As INDArray

		''' <summary>
		''' Returns the binary NDArray with value true where this array's entries are NaN, or false where they
		''' are not infinite
		''' </summary>
		ReadOnly Property NaN As INDArray

		''' <summary>
		''' Returns the ndarray negative (cloned)
		''' </summary>
		''' <returns> Array copy with all values negated </returns>
		Function neg() As INDArray

		''' <summary>
		''' In place setting of the negative version of this ndarray
		''' </summary>
		''' <returns> This array with all values negated </returns>
		Function negi() As INDArray

		''' <summary>
		''' Reverse division with a scalar - i.e., (n / thisArrayValues)
		''' </summary>
		''' <param name="n"> Value to use for reverse division </param>
		''' <returns>  Copy of array after applying reverse division </returns>
		Function rdiv(ByVal n As Number) As INDArray

		''' <summary>
		''' In place reverse division - i.e., (n / thisArrayValues)
		''' </summary>
		''' <param name="n"> Value to use for reverse division </param>
		''' <returns> This array after applying reverse division </returns>
		Function rdivi(ByVal n As Number) As INDArray

		''' <summary>
		''' Reverse subtraction with duplicates - i.e., (n - thisArrayValues)
		''' </summary>
		''' <param name="n"> Value to use for reverse subtraction </param>
		''' <returns> Copy of array after reverse subtraction </returns>
		Function rsub(ByVal n As Number) As INDArray

		''' <summary>
		''' Reverse subtraction in place - i.e., (n - thisArrayValues)
		''' </summary>
		''' <param name="n"> Value to use for reverse subtraction </param>
		''' <returns> This array after reverse subtraction </returns>
		Function rsubi(ByVal n As Number) As INDArray

		''' <summary>
		''' Division by a number
		''' </summary>
		''' <param name="n"> Number to divide values by </param>
		''' <returns> Copy of array after division </returns>
		Function div(ByVal n As Number) As INDArray

		''' <summary>
		''' In place scalar division
		''' </summary>
		''' <param name="n"> Number to divide values by </param>
		''' <returns> This array, after applying division operation </returns>
		Function divi(ByVal n As Number) As INDArray

		''' <summary>
		''' Scalar multiplication (copy)
		''' </summary>
		''' <param name="n"> the number to multiply by </param>
		''' <returns> a copy of this ndarray multiplied by the given number </returns>
		Function mul(ByVal n As Number) As INDArray

		''' <summary>
		''' In place scalar multiplication
		''' </summary>
		''' <param name="n"> The number to multiply by </param>
		''' <returns> This array, after applying scaler multiplication </returns>
		Function muli(ByVal n As Number) As INDArray

		''' <summary>
		''' Scalar subtraction (copied)
		''' </summary>
		''' <param name="n"> the number to subtract by </param>
		''' <returns> Copy of this array after applying subtraction operation </returns>
		Function [sub](ByVal n As Number) As INDArray

		''' <summary>
		''' In place scalar subtraction
		''' </summary>
		''' <param name="n"> Number to subtract </param>
		''' <returns> This array, after applying subtraction operation </returns>
		Function subi(ByVal n As Number) As INDArray

		''' <summary>
		''' Scalar addition (cloning)
		''' </summary>
		''' <param name="n"> the number to add </param>
		''' <returns> a clone with this matrix + the given number </returns>
		Function add(ByVal n As Number) As INDArray

		''' <summary>
		''' In place scalar addition
		''' </summary>
		''' <param name="n"> Number to add </param>
		''' <returns> This array, after adding value </returns>
		Function addi(ByVal n As Number) As INDArray

		''' <summary>
		''' Reverse division (number / ndarray)
		''' </summary>
		''' <param name="n">      the number to divide by </param>
		''' <param name="result"> Array to place the result in. Must match shape of this array </param>
		''' <returns> Result array </returns>
		Function rdiv(ByVal n As Number, ByVal result As INDArray) As INDArray

		''' <summary>
		''' Reverse in place division
		''' </summary>
		''' <param name="n">      the number to divide by </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result ndarray </returns>
		Function rdivi(ByVal n As Number, ByVal result As INDArray) As INDArray

		''' <summary>
		''' Reverse subtraction
		''' </summary>
		''' <param name="n">      the number to subtract by </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result ndarray </returns>
		Function rsub(ByVal n As Number, ByVal result As INDArray) As INDArray

		''' <summary>
		''' Reverse in place subtraction
		''' </summary>
		''' <param name="n">      the number to subtract by </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result ndarray </returns>
		Function rsubi(ByVal n As Number, ByVal result As INDArray) As INDArray

		''' <summary>
		''' Division if ndarray by number
		''' </summary>
		''' <param name="n">      the number to divide by </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result ndarray </returns>
		Function div(ByVal n As Number, ByVal result As INDArray) As INDArray

		''' <summary>
		''' In place division of this ndarray
		''' </summary>
		''' <param name="n">      the number to divide by </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result ndarray </returns>
		Function divi(ByVal n As Number, ByVal result As INDArray) As INDArray

		''' <summary>
		''' Multiplication of ndarray.
		''' </summary>
		''' <param name="n"> the number to multiply by </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result ndarray </returns>
		Function mul(ByVal n As Number, ByVal result As INDArray) As INDArray

		''' <summary>
		''' In place multiplication of this ndarray
		''' </summary>
		''' <param name="n">      the number to divide by </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result ndarray </returns>
		Function muli(ByVal n As Number, ByVal result As INDArray) As INDArray

		''' <summary>
		''' Subtraction of this ndarray
		''' </summary>
		''' <param name="n">      the number to subtract by </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result ndarray </returns>
		Function [sub](ByVal n As Number, ByVal result As INDArray) As INDArray

		''' <summary>
		''' In place subtraction of this ndarray
		''' </summary>
		''' <param name="n">      the number to subtract by </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result ndarray </returns>
		Function subi(ByVal n As Number, ByVal result As INDArray) As INDArray

		''' <summary>
		''' Addition of this ndarray. </summary>
		''' <param name="n">      the number to add </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result ndarray </returns>
		Function add(ByVal n As Number, ByVal result As INDArray) As INDArray

		''' <summary>
		''' In place addition
		''' </summary>
		''' <param name="n">      the number to add </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result ndarray </returns>
		Function addi(ByVal n As Number, ByVal result As INDArray) As INDArray

		''' <summary>
		''' Returns a subset of this array based on the specified indexes
		''' </summary>
		''' <param name="indexes"> the indexes in to the array </param>
		''' <returns> a view of the array with the specified indices </returns>
		Function get(ParamArray ByVal indexes() As INDArrayIndex) As INDArray

		'TODO: revisit after #8166 is resolved.
		''' <summary>
		''' Return a mask on whether each element matches the given condition </summary>
		''' <param name="comp"> </param>
		''' <param name="condition">
		''' @return </param>
		Function match(ByVal comp As INDArray, ByVal condition As Condition) As INDArray

		'TODO: revisit after #8166 is resolved.
		''' <summary>
		''' Returns a mask </summary>
		''' <param name="comp"> </param>
		''' <param name="condition">
		''' @return </param>
		Function match(ByVal comp As Number, ByVal condition As Condition) As INDArray

		''' <summary>
		''' Boolean indexing:
		''' Return the element if it fulfills the condition in
		''' result array </summary>
		''' <param name="comp"> the comparison array </param>
		''' <param name="condition"> the condition to apply </param>
		''' <returns> the array fulfilling the criteria </returns>
		Function getWhere(ByVal comp As INDArray, ByVal condition As Condition) As INDArray

		''' <summary>
		''' Boolean indexing:
		''' Return the element if it fulfills the condition in
		''' result array </summary>
		''' <param name="comp"> the comparison array </param>
		''' <param name="condition"> the condition to apply </param>
		''' <returns> the array fulfilling the criteria </returns>
		Function getWhere(ByVal comp As Number, ByVal condition As Condition) As INDArray

		'TODO: unused / untested method. (only used to forward calls from putWhere(Number,INDArray ,Condition).
		''' <summary>
		''' Assign the element according to the comparison array </summary>
		''' <param name="comp"> the comparison array </param>
		''' <param name="put"> the elements to put </param>
		''' <param name="condition"> the condition for masking on </param>
		''' <returns> a copy of this array with the conditional assignments. </returns>
		Function putWhere(ByVal comp As INDArray, ByVal put As INDArray, ByVal condition As Condition) As INDArray

		'TODO: unused / untested method.
		''' <summary>
		''' Assign the element according to the comparison array </summary>
		''' <param name="comp"> the comparison array </param>
		''' <param name="put"> the elements to put </param>
		''' <param name="condition"> the condition for masking on </param>
		''' <returns> a copy of this array with the conditional assignments. </returns>
		Function putWhere(ByVal comp As Number, ByVal put As INDArray, ByVal condition As Condition) As INDArray

		'TODO: unused / untested method. (only used to forward calls from  other putWhereWithMask implementations.
		''' <summary>
		''' Use a pre computed mask for assigning arrays </summary>
		''' <param name="mask"> the mask to use </param>
		''' <param name="put"> the array to put </param>
		''' <returns> a copy of this array with the conditional assignments. </returns>
		Function putWhereWithMask(ByVal mask As INDArray, ByVal put As INDArray) As INDArray

		'TODO: unused / untested method.
		''' <summary>
		''' Use a pre computed mask for assigning arrays </summary>
		''' <param name="mask"> the mask to use </param>
		''' <param name="put"> the array to put </param>
		''' <returns> a copy of this array with the conditional assignments. </returns>
		Function putWhereWithMask(ByVal mask As INDArray, ByVal put As Number) As INDArray

		'TODO: unused / untested method.
		''' <summary>
		''' Assign the element according to the comparison array </summary>
		''' <param name="comp"> the comparison array </param>
		''' <param name="put"> the elements to put </param>
		''' <param name="condition"> the condition for masking on </param>
		''' <returns> a copy of this array with the conditional assignments. </returns>
		Function putWhere(ByVal comp As Number, ByVal put As Number, ByVal condition As Condition) As INDArray

		''' <summary>
		''' Get the elements from this ndarray based on the specified indices </summary>
		''' <param name="indices"> an ndaray of the indices to get the elements for </param>
		''' <returns> the elements to get the array for </returns>
		Function get(ByVal indices As INDArray) As INDArray

		''' <summary>
		''' Get an INDArray comprised of the specified columns only. Copy operation.
		''' </summary>
		''' <param name="columns"> Columns to extract out of the current array </param>
		''' <returns> Array with only the specified columns </returns>
		Function getColumns(ParamArray ByVal columns() As Integer) As INDArray

		''' <summary>
		''' Get an INDArray comprised of the specified rows only. Copy operation
		''' </summary>
		''' <param name="rows"> Rose to extract from this array </param>
		''' <returns> Array with only the specified rows </returns>
		Function getRows(ParamArray ByVal rows() As Integer) As INDArray

		''' <summary>
		''' Reverse division, elements wise. i.e., other / this
		''' </summary>
		''' <param name="other"> the matrix to divide from </param>
		''' <returns> Copy of this array after performing element wise reverse division </returns>
		Function rdiv(ByVal other As INDArray) As INDArray

		''' <summary>
		''' Reverse divsion (in place). i.e., other / this
		''' </summary>
		''' <param name="other"> The matrix to divide from </param>
		''' <returns> This array after performing element wise reverse division </returns>
		Function rdivi(ByVal other As INDArray) As INDArray

		'TODO: unused / untested method.
		''' <summary>
		''' Reverse division
		''' </summary>
		''' <param name="other">  the matrix to divide from </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result ndarray </returns>
		Function rdiv(ByVal other As INDArray, ByVal result As INDArray) As INDArray

		''' <summary>
		''' Reverse division (in-place)
		''' </summary>
		''' <param name="other">  the matrix to divide from </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the ndarray with the operation applied </returns>
		Function rdivi(ByVal other As INDArray, ByVal result As INDArray) As INDArray

		''' <summary>
		''' Reverse subtraction
		''' </summary>
		''' <param name="other">  the matrix to subtract from </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result ndarray </returns>
		Function rsub(ByVal other As INDArray, ByVal result As INDArray) As INDArray

		''' <summary>
		''' Element-wise reverse subtraction (copy op). i.e., other - this
		''' </summary>
		''' <param name="other"> Other array to use in reverse subtraction </param>
		''' <returns> Copy of this array, after applying reverse subtraction </returns>
		Function rsub(ByVal other As INDArray) As INDArray

		''' <summary>
		''' Element-wise reverse subtraction (in the place op) - i.e., other - this
		''' </summary>
		''' <param name="other"> Other way to use in reverse subtraction operation </param>
		''' <returns> This array, after applying reverse subtraction </returns>
		Function rsubi(ByVal other As INDArray) As INDArray

		''' <summary>
		''' Reverse subtraction (in-place)
		''' </summary>
		''' <param name="other">  the other ndarray to subtract </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the ndarray with the operation applied </returns>
		Function rsubi(ByVal other As INDArray, ByVal result As INDArray) As INDArray

		''' <summary>
		''' Set all entries of the ndarray to the specified value
		''' </summary>
		''' <param name="value"> the value to assign </param>
		''' <returns> the ndarray with the values </returns>
		Function assign(ByVal value As Number) As INDArray

		''' <summary>
		''' Set all entries of the ndarray to the specified value
		''' </summary>
		''' <param name="value"> the value to assign </param>
		''' <returns> the ndarray with the values </returns>
		Function assign(ByVal value As Boolean) As INDArray

		''' <summary>
		''' Get the linear index of the data in to the array
		''' </summary>
		''' <param name="i"> the index to getScalar </param>
		''' <returns> the linear index in to the data </returns>
		Function linearIndex(ByVal i As Long) As Long

		'TODO: unused / untested method. only used recursively.
		''' <summary>
		''' Flattens the array for linear indexing in list.
		''' </summary>
		Sub sliceVectors(ByVal list As IList(Of INDArray))

		''' <summary>
		''' Assigns the given matrix (put) to the specified slice
		''' </summary>
		''' <param name="slice"> the slice to assign </param>
		''' <param name="put">   the slice to applyTransformToDestination </param>
		''' <returns> this for chainability </returns>
		Function putSlice(ByVal slice As Integer, ByVal put As INDArray) As INDArray

		''' <summary>
		''' Returns a binary INDArray with value 'true' if the element matches the specified condition and 'false' otherwise
		''' </summary>
		''' <param name="condition"> Condition to apply </param>
		''' <returns> Copy of this array with values 0 (condition does not apply), or one (condition applies) </returns>
		Function cond(ByVal condition As Condition) As INDArray

		''' <summary>
		''' Replicate and tile array to fill out to the given shape
		''' See:
		''' https://github.com/numpy/numpy/blob/master/numpy/matlib.py#L310-L358 </summary>
		''' <param name="shape"> the new shape of this ndarray </param>
		''' <returns> the shape to fill out to </returns>
		Function repmat(ParamArray ByVal shape() As Long) As INDArray

		<Obsolete>
		Function repmat(ParamArray ByVal shape() As Integer) As INDArray

		''' <summary>
		''' Repeat elements along a specified dimension.
		''' </summary>
		''' <param name="dimension"> the dimension to repeat </param>
		''' <param name="repeats"> the number of elements to repeat on each element </param>
		''' <returns> Repeated array </returns>
		Function repeat(ByVal dimension As Integer, ParamArray ByVal repeats() As Long) As INDArray

		''' <summary>
		''' Insert a row in to this array
		''' Will throw an exception if this ndarray is not a matrix
		''' </summary>
		''' <param name="row">   the row insert into </param>
		''' <param name="toPut"> the row to insert </param>
		''' <returns> this </returns>
		Function putRow(ByVal row As Long, ByVal toPut As INDArray) As INDArray

		''' <summary>
		''' Insert a column in to this array
		''' Will throw an exception if this ndarray is not a matrix
		''' </summary>
		''' <param name="column"> the column to insert </param>
		''' <param name="toPut">  the array to put </param>
		''' <returns> this </returns>
		Function putColumn(ByVal column As Integer, ByVal toPut As INDArray) As INDArray

		''' <summary>
		''' Returns the element at the specified row/column
		''' </summary>
		''' <param name="row">    the row of the element to return </param>
		''' <param name="column"> the row of the element to return </param>
		''' <returns> a scalar indarray of the element at this index </returns>
		Function getScalar(ByVal row As Long, ByVal column As Long) As INDArray

		''' <summary>
		''' Returns the element at the specified index
		''' </summary>
		''' <param name="i"> the index of the element to return </param>
		''' <returns> a scalar ndarray of the element at this index </returns>
		Function getScalar(ByVal i As Long) As INDArray

		''' <summary>
		''' Returns the square of the Euclidean distance.
		''' </summary>
		Function squaredDistance(ByVal other As INDArray) As Double

		''' <summary>
		''' Returns the (euclidean) distance.
		''' </summary>
		Function distance2(ByVal other As INDArray) As Double

		''' <summary>
		''' Returns the (1-norm) distance.
		''' </summary>
		Function distance1(ByVal other As INDArray) As Double

		''' <summary>
		''' Put element in to the indices denoted by
		''' the indices ndarray.
		''' In numpy this is equivalent to:
		''' a[indices] = element
		''' </summary>
		''' <param name="indices"> the indices to put </param>
		''' <param name="element"> the element array to put </param>
		''' <returns> this array </returns>
		Function put(ByVal indices As INDArray, ByVal element As INDArray) As INDArray

		''' <summary>
		''' Put the elements of the ndarray in to the specified indices
		''' </summary>
		''' <param name="indices"> the indices to put the ndarray in to </param>
		''' <param name="element"> the ndarray to put </param>
		''' <returns> this ndarray </returns>
		Function put(ByVal indices() As INDArrayIndex, ByVal element As INDArray) As INDArray

		''' <summary>
		''' Put the elements of the ndarray in to the specified indices
		''' </summary>
		''' <param name="indices"> the indices to put the ndarray in to </param>
		''' <param name="element"> the ndarray to put </param>
		''' <returns> this ndarray </returns>
		Function put(ByVal indices() As INDArrayIndex, ByVal element As Number) As INDArray

		''' <summary>
		''' Inserts the element at the specified index
		''' </summary>
		''' <param name="indices"> the indices to insert into </param>
		''' <param name="element"> a scalar ndarray </param>
		''' <returns> a scalar ndarray of the element at this index </returns>
		Function put(ByVal indices() As Integer, ByVal element As INDArray) As INDArray


		''' <summary>
		''' Inserts the element at the specified index
		''' </summary>
		''' <param name="i">       the row insert into </param>
		''' <param name="j">       the column to insert into </param>
		''' <param name="element"> a scalar ndarray </param>
		''' <returns> a scalar ndarray of the element at this index </returns>
		Function put(ByVal i As Integer, ByVal j As Integer, ByVal element As INDArray) As INDArray

		''' <summary>
		''' Inserts the element at the specified index
		''' </summary>
		''' <param name="i">       the row insert into </param>
		''' <param name="j">       the column to insert into </param>
		''' <param name="element"> a scalar ndarray </param>
		''' <returns> a scalar ndarray of the element at this index </returns>
		Function put(ByVal i As Integer, ByVal j As Integer, ByVal element As Number) As INDArray

		''' <summary>
		''' Inserts the element at the specified index
		''' </summary>
		''' <param name="i">       the index insert into </param>
		''' <param name="element"> a scalar ndarray </param>
		''' <returns> a scalar ndarray of the element at this index </returns>
		Function put(ByVal i As Integer, ByVal element As INDArray) As INDArray

		''' <summary>
		''' In place division of a column vector
		''' </summary>
		''' <param name="columnVector"> the column vector used for division </param>
		''' <returns> the result of the division  </returns>
		Function diviColumnVector(ByVal columnVector As INDArray) As INDArray

		''' <summary>
		''' Division of a column vector (copy)
		''' </summary>
		''' <param name="columnVector"> the column vector used for division </param>
		''' <returns> the result of the division  </returns>
		Function divColumnVector(ByVal columnVector As INDArray) As INDArray

		''' <summary>
		''' In place division of a row vector
		''' </summary>
		''' <param name="rowVector"> the row vector used for division </param>
		''' <returns> the result of the division  </returns>
		Function diviRowVector(ByVal rowVector As INDArray) As INDArray

		''' <summary>
		''' Division of a row vector (copy)
		''' </summary>
		''' <param name="rowVector"> the row vector used for division </param>
		''' <returns> the result of the division  </returns>
		Function divRowVector(ByVal rowVector As INDArray) As INDArray

		''' <summary>
		''' In place reverse divison of a column vector
		''' </summary>
		''' <param name="columnVector"> the column vector used for division </param>
		''' <returns> the result of the division  </returns>
		Function rdiviColumnVector(ByVal columnVector As INDArray) As INDArray

		''' <summary>
		''' Reverse division of a column vector (copy)
		''' </summary>
		''' <param name="columnVector"> the column vector used for division </param>
		''' <returns> the result of the division  </returns>
		Function rdivColumnVector(ByVal columnVector As INDArray) As INDArray

		''' <summary>
		''' In place reverse division of a column vector
		''' </summary>
		''' <param name="rowVector"> the row vector used for division </param>
		''' <returns> the result of the division  </returns>
		Function rdiviRowVector(ByVal rowVector As INDArray) As INDArray

		'TODO: unused / untested method.
		''' <summary>
		''' Reverse division of a column vector (copy)
		''' </summary>
		''' <param name="rowVector"> the row vector used for division </param>
		''' <returns> the result of the division  </returns>
		Function rdivRowVector(ByVal rowVector As INDArray) As INDArray

		''' <summary>
		''' In place multiplication of a column vector
		''' </summary>
		''' <param name="columnVector"> the column vector used for multiplication </param>
		''' <returns> the result of the multiplication </returns>
		Function muliColumnVector(ByVal columnVector As INDArray) As INDArray

		''' <summary>
		''' Multiplication of a column vector (copy)
		''' </summary>
		''' <param name="columnVector"> the column vector used for multiplication </param>
		''' <returns> the result of the multiplication </returns>
		Function mulColumnVector(ByVal columnVector As INDArray) As INDArray

		''' <summary>
		''' In place multiplication of a row vector
		''' </summary>
		''' <param name="rowVector"> the row vector used for multiplication </param>
		''' <returns> the result of the multiplication </returns>
		Function muliRowVector(ByVal rowVector As INDArray) As INDArray

		''' <summary>
		''' Multiplication of a row vector (copy)
		''' </summary>
		''' <param name="rowVector"> the row vector used for multiplication </param>
		''' <returns> the result of the multiplication </returns>
		Function mulRowVector(ByVal rowVector As INDArray) As INDArray

		''' <summary>
		''' In place reverse subtraction of a column vector
		''' </summary>
		''' <param name="columnVector"> the column vector to subtract </param>
		''' <returns> the result of the subtraction </returns>
		Function rsubiColumnVector(ByVal columnVector As INDArray) As INDArray

		''' <summary>
		''' Reverse subtraction of a column vector (copy)
		''' </summary>
		''' <param name="columnVector"> the column vector to subtract </param>
		''' <returns> the result of the subtraction </returns>
		Function rsubColumnVector(ByVal columnVector As INDArray) As INDArray

		''' <summary>
		''' In place reverse subtraction of a row vector
		''' </summary>
		''' <param name="rowVector"> the row vector to subtract </param>
		''' <returns> the result of the subtraction </returns>
		Function rsubiRowVector(ByVal rowVector As INDArray) As INDArray

		'TODO: unused / untested method.
		''' <summary>
		''' Reverse subtraction of a row vector (copy)
		''' </summary>
		''' <param name="rowVector"> the row vector to subtract </param>
		''' <returns> the result of the subtraction </returns>
		Function rsubRowVector(ByVal rowVector As INDArray) As INDArray

		''' <summary>
		''' In place subtraction of a column vector
		''' </summary>
		''' <param name="columnVector"> the column vector to subtract </param>
		''' <returns> the result of the subtraction </returns>
		Function subiColumnVector(ByVal columnVector As INDArray) As INDArray

		''' <summary>
		''' Subtraction of a column vector (copy)
		''' </summary>
		''' <param name="columnVector"> the column vector to subtract </param>
		''' <returns> the result of the subtraction </returns>
		Function subColumnVector(ByVal columnVector As INDArray) As INDArray

		''' <summary>
		''' In place subtraction of a row vector
		''' </summary>
		''' <param name="rowVector"> the row vector to subtract </param>
		''' <returns> the result of the subtraction </returns>
		Function subiRowVector(ByVal rowVector As INDArray) As INDArray

		''' <summary>
		''' Subtraction of a row vector (copy)
		''' </summary>
		''' <param name="rowVector"> the row vector to subtract </param>
		''' <returns> the result of the subtraction </returns>
		Function subRowVector(ByVal rowVector As INDArray) As INDArray

		''' <summary>
		''' In place addition of a column vector
		''' </summary>
		''' <param name="columnVector"> the column vector to add </param>
		''' <returns> the result of the addition </returns>
		Function addiColumnVector(ByVal columnVector As INDArray) As INDArray

		''' <summary>
		''' In place assignment of a column vector
		''' </summary>
		''' <param name="columnVector"> the column vector to add </param>
		''' <returns> the result of the addition </returns>
		Function putiColumnVector(ByVal columnVector As INDArray) As INDArray

		''' <summary>
		''' Addition of a column vector (copy)
		''' </summary>
		''' <param name="columnVector"> the column vector to add </param>
		''' <returns> the result of the addition </returns>
		Function addColumnVector(ByVal columnVector As INDArray) As INDArray

		''' <summary>
		''' In place addition of a row vector
		''' </summary>
		''' <param name="rowVector"> the row vector to add </param>
		''' <returns> the result of the addition </returns>
		Function addiRowVector(ByVal rowVector As INDArray) As INDArray

		''' <summary>
		''' in place assignment of row vector, to each row of this array
		''' </summary>
		''' <param name="rowVector"> Row vector to put </param>
		''' <returns> This array, after assigning every road to the specified value </returns>
		Function putiRowVector(ByVal rowVector As INDArray) As INDArray

		''' <summary>
		''' Addition of a row vector (copy)
		''' </summary>
		''' <param name="rowVector"> the row vector to add </param>
		''' <returns> the result of the addition </returns>
		Function addRowVector(ByVal rowVector As INDArray) As INDArray

		''' <summary>
		''' Perform a copy matrix multiplication
		''' </summary>
		''' <param name="other"> the other matrix to perform matrix multiply with </param>
		''' <returns> the result of the matrix multiplication </returns>
		Function mmul(ByVal other As INDArray, ByVal mMulTranspose As MMulTranspose) As INDArray

		''' <summary>
		''' Perform a copy matrix multiplication
		''' </summary>
		''' <param name="other"> the other matrix to perform matrix multiply with </param>
		''' <returns> the result of the matrix multiplication </returns>
		Function mmul(ByVal other As INDArray) As INDArray

		''' <summary>
		''' Perform a copy matrix multiplication </summary>
		''' <param name="other"> other the other matrix to perform matrix multiply with </param>
		''' <param name="resultOrder"> either C or F order for result array </param>
		''' <returns> the result of the matrix multiplication </returns>
		Function mmul(ByVal other As INDArray, ByVal resultOrder As Char) As INDArray

		''' <summary>
		''' Convert this ndarray to a 2d double matrix.
		''' Note that THIS SHOULD NOT BE USED FOR SPEED.
		''' This is mainly used for integrations with other libraries.
		''' Due to nd4j's off  heap nature, moving data on heap is very expensive
		''' and should not be used if possible. </summary>
		''' <returns> a copy of this array as a 2d double array </returns>
		Function toDoubleMatrix() As Double()()

		''' <summary>
		''' Convert this ndarray to a 1d double matrix.
		''' Note that THIS SHOULD NOT BE USED FOR SPEED.
		''' This is mainly used for integrations with other libraries.
		''' Due to nd4j's off  heap nature, moving data on heap is very expensive
		''' and should not be used if possible. </summary>
		''' <returns> a copy of this array as a 1d double array </returns>
		Function toDoubleVector() As Double()

		''' <summary>
		''' Convert this ndarray to a 1d float vector.
		''' Note that THIS SHOULD NOT BE USED FOR SPEED.
		''' This is mainly used for integrations with other libraries.
		''' Due to nd4j's off  heap nature, moving data on heap is very expensive
		''' and should not be used if possible. </summary>
		''' <returns> a copy of this array as a 1d float array </returns>
		Function toFloatVector() As Single()

		''' <summary>
		''' Convert this ndarray to a 2d float matrix.
		''' Note that THIS SHOULD NOT BE USED FOR SPEED.
		''' This is mainly used for integrations with other libraries.
		''' Due to nd4j's off  heap nature, moving data on heap is very expensive
		''' and should not be used if possible. </summary>
		''' <returns> a copy of this array as a 2d float array </returns>
		Function toFloatMatrix() As Single()()

		''' <summary>
		''' Convert this ndarray to a 1d int matrix.
		''' Note that THIS SHOULD NOT BE USED FOR SPEED.
		''' This is mainly used for integrations with other libraries.
		''' Due to nd4j's off  heap nature, moving data on heap is very expensive
		''' and should not be used if possible. </summary>
		''' <returns> a copy of this array as a 1d int array </returns>
		Function toIntVector() As Integer()

		''' <summary>
		''' Convert this ndarray to a 1d long matrix.
		''' Note that THIS SHOULD NOT BE USED FOR SPEED.
		''' This is mainly used for integrations with other libraries.
		''' Due to nd4j's off  heap nature, moving data on heap is very expensive
		''' and should not be used if possible. </summary>
		''' <returns> a copy of this array as a 1d long array </returns>
		Function toLongVector() As Long()

		''' <summary>
		''' Convert this ndarray to a 2d int matrix.
		''' Note that THIS SHOULD NOT BE USED FOR SPEED.
		''' This is mainly used for integrations with other libraries.
		''' Due to nd4j's off  heap nature, moving data on heap is very expensive
		''' and should not be used if possible. </summary>
		''' <returns> a copy of this array as a 2d int array </returns>
		Function toLongMatrix() As Long()()

		''' <summary>
		''' Convert this ndarray to a 2d int matrix.
		''' Note that THIS SHOULD NOT BE USED FOR SPEED.
		''' This is mainly used for integrations with other libraries.
		''' Due to nd4j's off  heap nature, moving data on heap is very expensive
		''' and should not be used if possible. </summary>
		''' <returns> a copy of this array as a 2d int array </returns>
		Function toIntMatrix() As Integer()()

		''' <summary>
		''' Perform an copy matrix multiplication
		''' </summary>
		''' <param name="other">  the other matrix to perform matrix multiply with </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result of the matrix multiplication </returns>
		Function mmul(ByVal other As INDArray, ByVal result As INDArray) As INDArray

		''' <summary>
		''' Perform an copy matrix multiplication
		''' </summary>
		''' <param name="other">  the other matrix to perform matrix multiply with </param>
		''' <param name="result"> the result ndarray </param>
		''' <param name="mMulTranspose"> the transpose status of each array </param>
		''' <returns> the result of the matrix multiplication </returns>
		Function mmul(ByVal other As INDArray, ByVal result As INDArray, ByVal mMulTranspose As MMulTranspose) As INDArray

		''' <summary>
		''' Copy (element wise) division of two NDArrays
		''' </summary>
		''' <param name="other"> the second ndarray to divide </param>
		''' <returns> the result of the divide </returns>
		Function div(ByVal other As INDArray) As INDArray

		''' <summary>
		''' copy (element wise) division of two NDArrays
		''' </summary>
		''' <param name="other">  the second ndarray to divide </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result of the divide </returns>
		Function div(ByVal other As INDArray, ByVal result As INDArray) As INDArray


		''' <summary>
		''' copy (element wise) multiplication of two NDArrays
		''' </summary>
		''' <param name="other"> the second ndarray to multiply </param>
		''' <returns> the result of the addition </returns>
		Function mul(ByVal other As INDArray) As INDArray

		''' <summary>
		''' copy (element wise) multiplication of two NDArrays
		''' </summary>
		''' <param name="other">  the second ndarray to multiply </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result of the multiplication </returns>
		Function mul(ByVal other As INDArray, ByVal result As INDArray) As INDArray

		''' <summary>
		''' copy subtraction of two NDArrays
		''' </summary>
		''' <param name="other"> the second ndarray to subtract </param>
		''' <returns> the result of the addition </returns>
		Function [sub](ByVal other As INDArray) As INDArray

		''' <summary>
		''' copy subtraction of two NDArrays
		''' </summary>
		''' <param name="other">  the second ndarray to subtract </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result of the subtraction </returns>
		Function [sub](ByVal other As INDArray, ByVal result As INDArray) As INDArray

		''' <summary>
		''' Element-wise copy addition of two NDArrays
		''' </summary>
		''' <param name="other"> the second ndarray to add </param>
		''' <returns> the result of the addition </returns>
		Function add(ByVal other As INDArray) As INDArray

		''' <summary>
		''' Element-wise copy addition of two NDArrays
		''' </summary>
		''' <param name="other">  the second ndarray to add </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result of the addition </returns>
		Function add(ByVal other As INDArray, ByVal result As INDArray) As INDArray

		''' <summary>
		''' Perform an copy matrix multiplication
		''' </summary>
		''' <param name="other"> the other matrix to perform matrix multiply with </param>
		''' <param name="transpose"> the transpose status of each ndarray </param>
		''' <returns> the result of the matrix multiplication </returns>
		Function mmuli(ByVal other As INDArray, ByVal transpose As MMulTranspose) As INDArray

		''' <summary>
		''' Perform an inplace matrix multiplication
		''' </summary>
		''' <param name="other"> the other matrix to perform matrix multiply with </param>
		''' <returns> the result of the matrix multiplication </returns>
		Function mmuli(ByVal other As INDArray) As INDArray

		''' <summary>
		''' Perform an in place matrix multiplication
		''' </summary>
		''' <param name="other">  the other matrix to perform matrix multiply with </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result of the matrix multiplication </returns>
		Function mmuli(ByVal other As INDArray, ByVal result As INDArray, ByVal transpose As MMulTranspose) As INDArray

		''' <summary>
		''' Perform an inplace matrix multiplication
		''' </summary>
		''' <param name="other">  the other matrix to perform matrix multiply with </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result of the matrix multiplication </returns>
		Function mmuli(ByVal other As INDArray, ByVal result As INDArray) As INDArray


		''' <summary>
		''' in place (element wise) division of two NDArrays
		''' </summary>
		''' <param name="other"> the second ndarray to divide </param>
		''' <returns> the result of the divide </returns>
		Function divi(ByVal other As INDArray) As INDArray

		''' <summary>
		''' in place (element wise) division of two NDArrays
		''' </summary>
		''' <param name="other">  the second ndarray to divide </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result of the divide </returns>
		Function divi(ByVal other As INDArray, ByVal result As INDArray) As INDArray


		''' <summary>
		''' in place (element wise) multiplication of two NDArrays
		''' </summary>
		''' <param name="other"> the second ndarray to multiply </param>
		''' <returns> the result of the multiplication </returns>
		Function muli(ByVal other As INDArray) As INDArray

		''' <summary>
		''' in place (element wise) multiplication of two NDArrays
		''' </summary>
		''' <param name="other">  the second ndarray to multiply </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result of the multiplication </returns>
		Function muli(ByVal other As INDArray, ByVal result As INDArray) As INDArray

		''' <summary>
		''' in place (element wise) subtraction of two NDArrays
		''' </summary>
		''' <param name="other"> the second ndarray to subtract </param>
		''' <returns> the result of the subtraction </returns>
		Function subi(ByVal other As INDArray) As INDArray

		''' <summary>
		''' in place (element wise) subtraction of two NDArrays
		''' </summary>
		''' <param name="other">  the second ndarray to subtract </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result of the subtraction </returns>
		Function subi(ByVal other As INDArray, ByVal result As INDArray) As INDArray

		''' <summary>
		''' in place (element wise) addition of two NDArrays
		''' </summary>
		''' <param name="other"> the second ndarray to add </param>
		''' <returns> the result of the addition </returns>
		Function addi(ByVal other As INDArray) As INDArray

		''' <summary>
		''' in place (element wise) addition of two NDArrays
		''' </summary>
		''' <param name="other">  the second ndarray to add </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result of the addition </returns>
		Function addi(ByVal other As INDArray, ByVal result As INDArray) As INDArray

		''' <summary>
		''' Returns the max norm (aka infinity norm, equal to the maximum absolute value) along the specified dimension(s)
		''' </summary>
		''' <param name="dimension"> the dimension to the max norm along </param>
		''' <returns> Max norm along the specified dimension </returns>
		Function normmax(ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns the max norm (aka infinity norm, equal to the maximum absolute value) along the specified dimension(s)
		''' </summary>
		''' <param name="dimension"> the dimension to the max norm along </param>
		''' <param name="keepDims"> whether to keep reduced dimensions as dimensions of size 1 </param>
		''' <returns> Max norm along the specified dimension </returns>
		Function normmax(ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Return the max norm (aka infinity norm, equal to the maximum absolute value) for the entire array
		''' </summary>
		''' <returns> Max norm for the entire array </returns>
		Function normmaxNumber() As Number

		''' <summary>
		''' Returns the norm2 (L2 norm, sqrt(sum(x_i^2), also known as Euclidean norm) along the specified dimension(s)
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the norm2 along </param>
		''' <returns> the norm2 along the specified dimension </returns>
		Function norm2(ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns the norm2 (L2 norm, sqrt(sum(x_i^2), also known as Euclidean norm) along the specified dimension(s)
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the norm2 along </param>
		''' <param name="keepDims"> whether to keep reduced dimensions as dimensions of size 1 </param>
		''' <returns> the norm2 along the specified dimension </returns>
		Function norm2(ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Return the norm2 (L2 norm, sqrt(sum(x_i^2), also known as Euclidean norm) for the entire array
		''' </summary>
		''' <returns> L2 norm for the array </returns>
		Function norm2Number() As Number

		''' <summary>
		''' Returns the norm1 (L1 norm, i.e., sum of absolute values; also known as Taxicab or Manhattan norm) along the
		''' specified dimension
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the norm1 along </param>
		''' <returns> the norm1 along the specified dimension </returns>
		Function norm1(ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns the norm1 (L1 norm, i.e., sum of absolute values; also known as Taxicab or Manhattan norm) along the
		''' specified dimension
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the norm1 along </param>
		''' <param name="keepDims"> whether to keep reduced dimensions as dimensions of size 1 </param>
		''' <returns> the norm1 along the specified dimension </returns>
		Function norm1(ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Calculate and return norm1 (L1 norm, i.e., sum of absolute values; also known as Taxicab or Manhattan norm) for
		''' the entire array
		''' </summary>
		''' <returns> Norm 1 for the array </returns>
		Function norm1Number() As Number

		''' <summary>
		''' Standard deviation of an INDArray along one or more dimensions
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the std along </param>
		''' <returns> the standard deviation along a particular dimension </returns>
		Function std(ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Calculate the standard deviation for the entire array
		''' </summary>
		''' <returns> standard deviation </returns>
		Function stdNumber() As Number

		''' <summary>
		''' Standard deviation of an ndarray along a dimension
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the std along </param>
		''' <returns> the standard deviation along a particular dimension </returns>
		Function std(ByVal biasCorrected As Boolean, ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Standard deviation of an ndarray along a dimension
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the std along </param>
		''' <param name="keepDims"> whether to keep reduced dimensions as dimensions of size 1 </param>
		''' <returns> the standard deviation along a particular dimension </returns>
		Function std(ByVal biasCorrected As Boolean, ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Calculate the standard deviation for the entire array, specifying whether it is bias corrected or not
		''' </summary>
		''' <param name="biasCorrected"> If true: bias corrected standard deviation. False: not bias corrected </param>
		''' <returns> Standard dev </returns>
		Function stdNumber(ByVal biasCorrected As Boolean) As Number

		''' <summary>
		''' Returns the product along a given dimension
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the product along </param>
		''' <returns> the product along the specified dimension </returns>
		Function prod(ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns the product along a given dimension
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the product along </param>
		''' <param name="keepDims"> whether to keep reduced dimensions as dimensions of size 1 </param>
		''' <returns> the product along the specified dimension </returns>
		Function prod(ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Calculate the product of all values in the array
		''' </summary>
		''' <returns> Product of all values in the array </returns>
		Function prodNumber() As Number

		''' <summary>
		''' Returns the overall mean of this ndarray
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the mean along </param>
		''' <returns> the mean along the specified dimension of this ndarray </returns>
		Function mean(ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns the overall mean of this ndarray
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the mean along </param>
		''' <returns> the mean along the specified dimension of this ndarray </returns>
		Function mean(ByVal result As INDArray, ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns the overall mean of this ndarray
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the mean along </param>
		''' <param name="keepDims"> whether to keep reduced dimensions as dimensions of size 1 </param>
		''' <returns> the mean along the specified dimension of this ndarray </returns>
		Function mean(ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns the overall mean of this ndarray
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the mean along </param>
		''' <param name="keepDims"> whether to keep reduced dimensions as dimensions of size 1 </param>
		''' <returns> the mean along the specified dimension of this ndarray </returns>
		Function mean(ByVal result As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns the absolute overall mean of this ndarray
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the mean along </param>
		''' <returns> the absolute mean along the specified dimension of this ndarray </returns>
		Function amean(ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns the overall mean of this ndarray
		''' </summary>
		''' <returns> the mean along the specified dimension of this ndarray </returns>
		Function meanNumber() As Number

		''' <summary>
		''' Returns the absolute overall mean of this ndarray
		''' </summary>
		''' <returns> the mean along the specified dimension of this ndarray </returns>
		Function ameanNumber() As Number

		''' <summary>
		''' Returns the overall variance of this ndarray
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the variance along </param>
		''' <returns> the variance along the specified dimension of this ndarray </returns>
		Function var(ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns the overall variance of this ndarray
		''' </summary>
		''' <param name="biasCorrected"> boolean on whether to apply corrected bias </param>
		''' <param name="dimension"> the dimension to getScalar the variance along </param>
		''' <returns> the variance along the specified dimension of this ndarray </returns>
		Function var(ByVal biasCorrected As Boolean, ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns the overall variance of all values in this INDArray
		''' </summary>
		''' <returns> variance </returns>
		Function varNumber() As Number

		''' <summary>
		''' Returns the overall max of this ndarray along given dimensions
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the max along </param>
		''' <returns> the max along the specified dimension of this ndarray </returns>
		Function max(ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns the overall max of this ndarray along given dimensions
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the max along </param>
		''' <param name="keepDims"> whether to keep reduced dimensions as dimensions of size 1 </param>
		''' <returns> the max along the specified dimension of this ndarray </returns>
		Function max(ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns the absolute overall max of this ndarray along given dimensions
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the amax along </param>
		''' <returns> the amax along the specified dimension of this ndarray </returns>
		Function amax(ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns maximum value in this INDArray </summary>
		''' <returns> maximum value </returns>
		Function maxNumber() As Number

		''' <summary>
		''' Returns maximum (absolute) value in this INDArray </summary>
		''' <returns> Max absolute value </returns>
		Function amaxNumber() As Number

		''' <summary>
		''' Returns the overall min of this ndarray
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the min along </param>
		''' <returns> the min along the specified dimension of this ndarray </returns>
		Function min(ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns the overall min of this ndarray
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the min along </param>
		''' <param name="keepDims"> whether to keep reduced dimensions as dimensions of size 1 </param>
		''' <returns> the min along the specified dimension of this ndarray </returns>
		Function min(ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns minimum (absolute) value in this INDArray, along the specified dimensions
		''' </summary>
		''' <returns> Minimum absolute value </returns>
		Function amin(ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns min value in this INDArray </summary>
		''' <returns> Minimum value in the array </returns>
		Function minNumber() As Number

		''' <summary>
		''' Returns absolute min value in this INDArray
		''' </summary>
		''' <returns> Absolute min value </returns>
		Function aminNumber() As Number

		''' <summary>
		''' Returns the sum along the last dimension of this ndarray
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the sum along </param>
		''' <returns> the sum along the specified dimension of this ndarray </returns>
		Function sum(ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns the sum along the last dimension of this ndarray
		''' </summary>
		''' <param name="dimension"> the dimension to getScalar the sum along </param>
		''' <param name="keepDims"> whether to keep reduced dimensions as dimensions of size 1 </param>
		''' <returns> the sum along the specified dimension of this ndarray </returns>
		Function sum(ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' This method takes boolean condition, and returns number of elements matching this condition
		''' </summary>
		''' <param name="condition"> Condition to calculate matches for </param>
		''' <returns> Number of elements matching condition </returns>
		Function scan(ByVal condition As Condition) As Number

		''' <summary>
		''' Returns the sum along the last dimension of this ndarray
		''' </summary>
		''' <param name="result"> result of this operation will be stored here </param>
		''' <param name="dimension"> the dimension to getScalar the sum along </param>
		''' <returns> the sum along the specified dimension of this ndarray </returns>
		Function sum(ByVal result As INDArray, ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns the sum along the last dimension of this ndarray
		''' </summary>
		''' <param name="result"> result of this operation will be stored here </param>
		''' <param name="keepDims"> whether to keep reduced dimensions as dimensions of size 1 </param>
		''' <param name="dimension"> the dimension to getScalar the sum along </param>
		''' <returns> the sum along the specified dimension of this ndarray </returns>
		Function sum(ByVal result As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Sum the entire array </summary>
		''' <returns> Sum of array </returns>
		Function sumNumber() As Number

		''' <summary>
		''' Returns entropy value for this INDArray </summary>
		''' <returns> entropy value </returns>
		Function entropyNumber() As Number

		''' <summary>
		''' Returns non-normalized Shannon entropy value for this INDArray </summary>
		''' <returns> non-normalized Shannon entropy </returns>
		Function shannonEntropyNumber() As Number

		''' <summary>
		''' Returns log entropy value for this INDArray </summary>
		''' <returns> log entropy value </returns>
		Function logEntropyNumber() As Number

		''' <summary>
		''' Returns entropy value for this INDArray along specified dimension(s) </summary>
		''' <param name="dimension"> specified dimension(s) </param>
		''' <returns> entropy value </returns>
		Function entropy(ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns Shannon entropy value for this INDArray along specified dimension(s) </summary>
		''' <param name="dimension"> specified dimension(s) </param>
		''' <returns> Shannon entropy </returns>
		Function shannonEntropy(ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Returns log entropy value for this INDArray along specified dimension(s) </summary>
		''' <param name="dimension"> specified dimension(s) </param>
		''' <returns> log entropy value </returns>
		Function logEntropy(ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Shape and stride setter </summary>
		''' <param name="shape"> new value for shape </param>
		''' <param name="stride"> new value for stride </param>
		Sub setShapeAndStride(ByVal shape() As Integer, ByVal stride() As Integer)

		''' <summary>
		''' Set the ordering </summary>
		''' <param name="order"> the ordering to set </param>
		WriteOnly Property Order As Char

		''' <summary>
		''' Returns the elements at the specified indices
		''' </summary>
		''' <param name="indices"> the indices to getScalar </param>
		''' <returns> the array with the specified elements </returns>
		Function getScalar(ParamArray ByVal indices() As Integer) As INDArray

		''' <summary>
		''' See <seealso cref="getScalar(Integer[])"/>
		''' </summary>
		Function getScalar(ParamArray ByVal indices() As Long) As INDArray

		''' <summary>
		''' Get an integer value at the specified indices. Result will be cast to an integer, precision loss is possible. </summary>
		''' <param name="indices"> Indices to get the integer at. Number of indices must match the array rank. </param>
		''' <returns> Integer value at the specified index </returns>
		Function getInt(ParamArray ByVal indices() As Integer) As Integer

		''' <summary>
		''' Get a long value at the specified index. </summary>
		''' <param name="index"> Index to get the integer at. </param>
		''' <returns> long value at the specified index </returns>
		Function getLong(ByVal index As Long) As Long

		''' <summary>
		''' Get a long value at the specified indices. </summary>
		''' <param name="indices"> Indices to get the double at. Number of indices must match the array rank. </param>
		''' <returns> long value at the specified index </returns>
		Function getLong(ParamArray ByVal indices() As Long) As Long

		''' <summary>
		''' Get the numeric value at the specified index. </summary>
		''' <param name="index"> index to retreive. </param>
		''' <returns> numeric value at the specified index. </returns>
		Function getNumber(ByVal index As Long) As Number

		''' <summary>
		''' Get a numeric value at the specified indices. </summary>
		''' <param name="indices"> Indices to get the value from. Number of indices must match the array rank. </param>
		''' <returns> Numeric value at the specified index </returns>
		Function getNumber(ParamArray ByVal indices() As Long) As Number

		''' <summary>
		''' Get a double value at the specified indices. </summary>
		''' <param name="indices"> Indices to get the double at. Number of indices must match the array rank. </param>
		''' <returns> Double value at the specified index </returns>
		Function getDouble(ParamArray ByVal indices() As Integer) As Double

		''' <summary>
		''' See <seealso cref="getDouble(Integer[])"/>
		''' </summary>
		Function getDouble(ParamArray ByVal indices() As Long) As Double

		''' <summary>
		''' Returns the elements at the specified indices
		''' </summary>
		''' <param name="indices"> the indices to getScalar </param>
		''' <returns> the array with the specified elements </returns>
		Function getFloat(ParamArray ByVal indices() As Integer) As Single

		''' <summary>
		''' See <seealso cref="getFloat(Integer...)"/>
		''' </summary>
		Function getFloat(ParamArray ByVal indices() As Long) As Single

		''' <summary>
		''' Get the double value at the specified linear index in the array
		''' </summary>
		''' <param name="i"> Index </param>
		''' <returns> Double value at the specified index </returns>
		Function getDouble(ByVal i As Long) As Double

		''' <summary>
		''' Get the double value at the specified indices. Can only be used for 2D (rank 2) arrays.
		''' </summary>
		''' <param name="i"> Dimension 0 (row) index </param>
		''' <param name="j"> Dimension 1 (column) index </param>
		''' <returns> double value at the specified indices </returns>
		Function getDouble(ByVal i As Long, ByVal j As Long) As Double

		''' <summary>
		''' Return the item at the linear index i
		''' </summary>
		''' <param name="i"> the index of the item to getScalar </param>
		''' <returns> the item at index j </returns>
		Function getFloat(ByVal i As Long) As Single

		''' <summary>
		''' Return the item at row i column j
		''' Note that this is the same as calling getScalar(new int[]{i,j}
		''' </summary>
		''' <param name="i"> the row to getScalar </param>
		''' <param name="j"> the column to getScalar </param>
		''' <returns> the item at row i column j </returns>
		Function getFloat(ByVal i As Long, ByVal j As Long) As Single

		''' <summary>
		''' Returns a copy of this ndarray
		''' </summary>
		''' <returns> a copy of this ndarray </returns>
		Function dup() As INDArray

		''' <summary>
		''' Returns a copy of this ndarray, where the returned ndarray has the specified order
		''' </summary>
		''' <param name="order"> order of the NDArray. 'f' or 'c' </param>
		''' <returns> copy of ndarray with specified order </returns>
		Function dup(ByVal order As Char) As INDArray

		''' <summary>
		''' Returns a flattened version (row vector) of this ndarray
		''' </summary>
		''' <returns> a flattened version (row vector) of this ndarray </returns>
		Function ravel() As INDArray

		''' <summary>
		''' Returns a flattened version (row vector) of this ndarray
		''' </summary>
		''' <returns> a flattened version (row vector) of this ndarray </returns>
		Function ravel(ByVal order As Char) As INDArray

		''' <summary>
		''' Set the data for this ndarray. </summary>
		''' <param name="data"> new value for the ndarray data. </param>
		WriteOnly Property Data As DataBuffer

		''' <summary>
		''' Returns the number of slices in this ndarray
		''' </summary>
		''' <returns> the number of slices in this ndarray </returns>
		Function slices() As Long

		''' <summary>
		''' Get the number of trailing ones in the array shape. For example, a rank 3 array with shape [10, 1, 1] would
		''' return 2 for this method
		''' </summary>
		''' <returns> Number of trailing ones in shape </returns>
		ReadOnly Property TrailingOnes As Integer

		''' <summary>
		''' Get the number of leading ones in the array shape. For example, a rank 3 array with shape [1, 10, 1] would
		''' return value 1 for this method
		''' </summary>
		''' <returns> Number of leading ones in shape </returns>
		ReadOnly Property LeadingOnes As Integer

		''' <summary>
		''' Returns the slice of this from the specified dimension
		''' </summary>
		''' <param name="i"> the index of the slice to return </param>
		''' <param name="dimension"> the dimension of the slice to return </param>
		''' <returns> the slice of this matrix from the specified dimension
		''' and dimension </returns>
		Function slice(ByVal i As Long, ByVal dimension As Integer) As INDArray

		''' <summary>
		''' Returns the specified slice of this ndarray
		''' </summary>
		''' <param name="i"> the index of the slice to return </param>
		''' <returns> the specified slice of this ndarray </returns>
		Function slice(ByVal i As Long) As INDArray

		''' <summary>
		''' Returns the start of where the ndarray is for the underlying data
		''' </summary>
		''' <returns> the starting offset </returns>
		Function offset() As Long

		' TODO: Unused untested method.
		''' <summary>
		''' Returns the start of where the ndarray is for the original data buffer
		''' </summary>
		''' <returns> original offset. </returns>
		Function originalOffset() As Long

		''' <summary>
		''' Reshapes the ndarray (can't change the length of the ndarray). Typically this will be a view, unless reshaping
		''' without copying is impossible.
		''' </summary>
		''' <param name="newShape"> the new shape of the ndarray </param>
		''' <returns> the reshaped ndarray </returns>
		Function reshape(ByVal order As Char, ParamArray ByVal newShape() As Long) As INDArray

		''' <summary>
		''' Reshapes the ndarray (can't change the length of the ndarray). Typically this will be a view, unless reshaping
		''' without copying is impossible.
		''' </summary>
		''' <param name="newShape"> the new shape of the ndarray </param>
		''' <returns> the reshaped ndarray </returns>
		Function reshape(ByVal order As Char, ParamArray ByVal newShape() As Integer) As INDArray

		''' <summary>
		''' Reshapes the ndarray (note: it's not possible to change the length of the ndarray).
		''' Typically this will be a view, unless reshaping without copying (i.e., returning a view) is impossible.<br>
		''' In that case, the behaviour will depend on the enforceView argument:
		''' enforceView == true: throw an exception<br>
		''' enforceView == false: return a copy<br>
		''' </summary>
		''' <param name="newShape"> the new shape of the ndarray </param>
		''' <returns> the reshaped ndarray </returns>
		Function reshape(ByVal order As Char, ByVal enforceView As Boolean, ParamArray ByVal newShape() As Long) As INDArray

		''' <summary>
		''' Reshapes the ndarray (can't change the length of the ndarray). Typically this will be a view, unless reshaping
		''' without copying is impossible.
		''' </summary>
		''' <param name="rows">    the rows of the matrix </param>
		''' <param name="columns"> the columns of the matrix </param>
		''' <returns> the reshaped ndarray </returns>
		Function reshape(ByVal order As Char, ByVal rows As Integer, ByVal columns As Integer) As INDArray

		''' <summary>
		''' Reshapes the ndarray (can't change the length of the ndarray). Typically this will be a view, unless reshaping
		''' without copying is impossible.
		''' </summary>
		''' <param name="newShape"> the new shape of the ndarray </param>
		''' <returns> the reshaped ndarray </returns>
		Function reshape(ParamArray ByVal newShape() As Long) As INDArray

		''' <summary>
		''' See <seealso cref="reshape(Long[])"/>
		''' </summary>
		Function reshape(ByVal shape() As Integer) As INDArray

		''' <summary>
		''' Reshapes the ndarray (can't change the length of the ndarray). Typically this will be a view, unless reshaping
		''' without copying is impossible.
		''' </summary>
		''' <param name="rows">    the rows of the matrix </param>
		''' <param name="columns"> the columns of the matrix </param>
		''' <returns> the reshaped ndarray </returns>
		Function reshape(ByVal rows As Long, ByVal columns As Long) As INDArray

		''' <summary>
		''' Flip the rows and columns of a matrix
		''' </summary>
		''' <returns> the flipped rows and columns of a matrix </returns>
		Function transpose() As INDArray

		''' <summary>
		''' Flip the rows and columns of a matrix, in-place
		''' </summary>
		''' <returns> the flipped rows and columns of a matrix </returns>
		Function transposei() As INDArray

		''' <summary>
		''' Mainly here for people coming from numpy.
		''' This is equivalent to a call to permute
		''' </summary>
		''' <param name="dimension"> the dimension to swap </param>
		''' <param name="with">      the one to swap it with </param>
		''' <returns> the swapped axes view </returns>
		Function swapAxes(ByVal dimension As Integer, ByVal [with] As Integer) As INDArray

		''' <summary>
		''' See: http://www.mathworks.com/help/matlab/ref/permute.html
		''' </summary>
		''' <param name="rearrange"> the dimensions to swap to </param>
		''' <returns> the newly permuted array </returns>
		Function permute(ParamArray ByVal rearrange() As Integer) As INDArray

		''' <summary>
		''' An <b>in-place</b> version of permute. The array  shape information (shape, strides)
		''' is modified by this operation (but not the data itself)
		''' See: http://www.mathworks.com/help/matlab/ref/permute.html
		''' </summary>
		''' <param name="rearrange"> the dimensions to swap to </param>
		''' <returns> the current array </returns>
		Function permutei(ParamArray ByVal rearrange() As Integer) As INDArray

		''' <summary>
		''' Dimshuffle: an extension of permute that adds the ability
		''' to broadcast various dimensions.
		''' This will only accept integers and xs.
		''' <p/>
		''' An x indicates a dimension should be broadcasted rather than permuted.
		''' 
		''' Examples originally from the theano docs:
		''' http://deeplearning.net/software/theano/library/tensor/basic.html
		''' 
		'''  Returns a view of this tensor with permuted dimensions. Typically the pattern will include the integers 0, 1, ... ndim-1, and any number of 'x' characters in dimensions where this tensor should be broadcasted.
		''' 
		''' A few examples of patterns and their effect:
		''' 
		''' ('x') -> make a 0d (scalar) into a 1d vector
		''' (0, 1) -> identity for 2d vectors
		''' (1, 0) -> inverts the first and second dimensions
		''' ('x', 0) -> make a row out of a 1d vector (N to 1xN)
		''' (0, 'x') -> make a column out of a 1d vector (N to Nx1)
		''' (2, 0, 1) -> AxBxC to CxAxB
		''' (0, 'x', 1) -> AxB to Ax1xB
		''' (1, 'x', 0) -> AxB to Bx1xA
		''' (1,) -> This remove dimensions 0. It must be a broadcastable dimension (1xA to A)
		''' </summary>
		''' <param name="rearrange">     the dimensions to swap to </param>
		''' <param name="newOrder">      the new order (think permute) </param>
		''' <param name="broadCastable"> (whether the dimension is broadcastable) (must be same length as new order) </param>
		''' <returns> the newly permuted array </returns>
		Function dimShuffle(ByVal rearrange() As Object, ByVal newOrder() As Integer, ByVal broadCastable() As Boolean) As INDArray

		''' <summary>
		''' See {@link #dimShuffle(Object[], int[], boolean[])
		''' </summary>
		Function dimShuffle(ByVal rearrange() As Object, ByVal newOrder() As Long, ByVal broadCastable() As Boolean) As INDArray

		''' <summary>
		''' Returns the specified column.
		''' Throws an exception if its not a matrix
		''' </summary>
		''' <param name="i"> the column to getScalar </param>
		''' <returns> the specified column </returns>
		Function getColumn(ByVal i As Long) As INDArray

		''' <summary>
		''' Returns the specified column. Throws an exception if its not a matrix (rank 2).
		''' Returned array will either be 1D (keepDim = false) or 2D (keepDim = true) with shape [length, 1]
		''' </summary>
		''' <param name="i"> the row to get </param>
		''' <param name="keepDim"> If true: return [length, 1] array. Otherwise: return [length] array </param>
		''' <returns> the specified row </returns>
		Function getColumn(ByVal i As Long, ByVal keepDim As Boolean) As INDArray

		''' <summary>
		''' Returns the specified row as a 1D vector.
		''' Throws an exception if its not a matrix
		''' </summary>
		''' <param name="i"> the row to getScalar </param>
		''' <returns> the specified row </returns>
		Function getRow(ByVal i As Long) As INDArray

		''' <summary>
		''' Returns the specified row. Throws an exception if its not a matrix.
		''' Returned array will either be 1D (keepDim = false) or 2D (keepDim = true) with shape [1, length]
		''' </summary>
		''' <param name="i"> the row to get </param>
		''' <param name="keepDim"> If true: return [1,length] array. Otherwise: return [length] array </param>
		''' <returns> the specified row </returns>
		Function getRow(ByVal i As Long, ByVal keepDim As Boolean) As INDArray

		''' <summary>
		''' Returns the number of columns in this matrix (throws exception if not 2d)
		''' </summary>
		''' <returns> the number of columns in this matrix </returns>
		Function columns() As Integer

		''' <summary>
		''' Returns the number of rows in this matrix (throws exception if not 2d)
		''' </summary>
		''' <returns> the number of rows in this matrix </returns>
		Function rows() As Integer

		''' <summary>
		''' Returns true if the number of columns is 1
		''' </summary>
		''' <returns> true if the number of columns is 1 </returns>
		ReadOnly Property ColumnVector As Boolean

		''' <summary>
		''' Returns true if the number of rows is 1
		''' </summary>
		''' <returns> true if the number of rows is 1 </returns>
		ReadOnly Property RowVector As Boolean

		''' <summary>
		''' Returns true if the number of columns is 1
		''' </summary>
		''' <returns> true if the number of columns is 1 </returns>
		ReadOnly Property ColumnVectorOrScalar As Boolean

		''' <summary>
		''' Returns true if the number of rows is 1
		''' </summary>
		''' <returns> true if the number of rows is 1 </returns>
		ReadOnly Property RowVectorOrScalar As Boolean

		''' <summary>
		''' Returns true if this ndarray is a vector
		''' </summary>
		''' <returns> whether this ndarray is a vector </returns>
		ReadOnly Property Vector As Boolean

		''' <summary>
		''' Returns true if this ndarray is a vector or scalar
		''' </summary>
		''' <returns> whether this ndarray is a vector or scalar </returns>
		ReadOnly Property VectorOrScalar As Boolean

		''' <summary>
		''' Returns whether the matrix
		''' has the same rows and columns
		''' </summary>
		''' <returns> true if the matrix has the same rows and columns
		''' false otherwise </returns>
		ReadOnly Property Square As Boolean

		''' <summary>
		''' Returns true if this ndarray is a matrix
		''' </summary>
		''' <returns> whether this ndarray is a matrix </returns>
		ReadOnly Property Matrix As Boolean

		''' <summary>
		''' Returns true if this ndarray is a scalar
		''' </summary>
		''' <returns> whether this ndarray is a scalar </returns>
		ReadOnly Property Scalar As Boolean

		''' <summary>
		''' Returns the shape of this ndarray
		''' </summary>
		''' <returns> the shape of this ndarray </returns>
		Function shape() As Long()

		''' <summary>
		''' Returns shape descriptor of this ndarray </summary>
		''' <returns> shape descriptor </returns>
		Function shapeDescriptor() As LongShapeDescriptor

		''' <summary>
		''' Returns the stride of this ndarray
		''' </summary>
		''' <returns> the stride of this ndarray </returns>
		Function stride() As Long()

		''' <summary>
		''' Return the ordering (fortran or c  'f' and 'c' respectively) of this ndarray </summary>
		''' <returns> the ordering of this ndarray </returns>
		Function ordering() As Char

		''' <summary>
		''' Returns the size along a specified dimension
		''' </summary>
		''' <param name="dimension"> the dimension to return the size for </param>
		''' <returns> the size of the array along the specified dimension </returns>
		Function size(ByVal dimension As Integer) As Long

		''' <summary>
		''' Returns the total number of elements in the ndarray
		''' </summary>
		''' <returns> the number of elements in the ndarray </returns>
		Function length() As Long

		''' <summary>
		''' Broadcasts this ndarray to be the specified shape
		''' </summary>
		''' <param name="shape"> the new shape of this ndarray </param>
		''' <returns> the broadcasted ndarray </returns>
		Function broadcast(ParamArray ByVal shape() As Long) As INDArray

		''' <summary>
		''' Broadcasts this ndarray to be the specified shape
		''' </summary>
		''' <returns> the broadcasted ndarray </returns>
		Function broadcast(ByVal result As INDArray) As INDArray

		''' <summary>
		''' Returns a scalar (individual element)
		''' of a scalar ndarray
		''' </summary>
		''' <returns> the individual item in this ndarray </returns>
		Function element() As Object

		''' <summary>
		''' Returns a linear double array representation of this ndarray
		''' </summary>
		''' <returns> the linear double array representation of this ndarray </returns>
		Function data() As DataBuffer

		''' <summary>
		''' This method checks 2 INDArrays equality with given eps
		''' </summary>
		''' <param name="o">   INDArray to compare against. </param>
		''' <param name="eps"> Epsilon value to use for the quality operation </param>
		''' <returns> True if ndarrays are equal within eps. </returns>
		Function equalsWithEps(ByVal o As Object, ByVal eps As Double) As Boolean

		''' <summary>
		''' This method checks 2 INDArrays for equal shapes.<br>
		''' Shapes are considered equal if:<br>
		''' (a) Both arrays have equal rank, and<br>
		''' (b) size(0)...size(rank()-1) are equal for both arrays </summary>
		''' <param name="other"> Other </param>
		''' <returns> True if shap </returns>
		Function equalShapes(ByVal other As INDArray) As Boolean

		''' <summary>
		''' Perform efficient (but unsafe) duplication. Don't use this method unless you know exactly what you are doing.
		''' Instead, use <seealso cref="dup()"/>
		''' </summary>
		''' <returns> Unsafe duplicate of array </returns>
		Function unsafeDuplication() As INDArray

		''' <summary>
		''' Perform efficient (but unsafe) duplication. Don't use this method unless you know exactly what you are doing.
		''' Instead, use <seealso cref="dup()"/>
		''' </summary>
		''' <returns> Unsafe duplicate of array </returns>
		Function unsafeDuplication(ByVal blocking As Boolean) As INDArray

		''' <summary>
		''' Remainder operator </summary>
		''' <param name="denominator"> the denominator </param>
		''' <returns> remainder </returns>
		Function remainder(ByVal denominator As INDArray) As INDArray

		''' <summary>
		''' Remainder operator </summary>
		''' <param name="denominator"> the denominator </param>
		''' <param name="result"> the result array to put this in </param>
		''' <returns> Remainder </returns>
		Function remainder(ByVal denominator As INDArray, ByVal result As INDArray) As INDArray

		''' <summary>
		''' The scalar remainder </summary>
		''' <param name="denominator"> the denominator as a scalar </param>
		''' <returns> Remainder </returns>
		Function remainder(ByVal denominator As Number) As INDArray

		''' <summary>
		''' The scalar remainder </summary>
		''' <param name="denominator"> the denominator as a scalar </param>
		''' <param name="result"> the result array to put this in </param>
		''' <returns> Remainder </returns>
		Function remainder(ByVal denominator As Number, ByVal result As INDArray) As INDArray

		' TODO: Unused untested method.
		''' <summary>
		''' In place remainder </summary>
		''' <param name="denominator"> the denominator </param>
		''' <returns> Remainder </returns>
		Function remainderi(ByVal denominator As INDArray) As INDArray

		' TODO: Unused untested method.
		''' <summary>
		''' In place remainder </summary>
		''' <param name="denominator"> the denominator </param>
		''' <returns> Remainder </returns>
		Function remainderi(ByVal denominator As Number) As INDArray

		''' <summary>
		''' remainder of division </summary>
		''' <param name="denominator"> the array of denominators for each element in this array </param>
		''' <returns> array of remainders </returns>
		Function fmod(ByVal denominator As INDArray) As INDArray

		''' <summary>
		'''  remainder of division </summary>
		''' <param name="denominator"> the array of denominators for each element in this array </param>
		''' <param name="result"> the result array </param>
		''' <returns> array of remainders </returns>
		Function fmod(ByVal denominator As INDArray, ByVal result As INDArray) As INDArray

		''' <summary>
		''' remainder of division by scalar.
		''' </summary>
		''' <param name="denominator"> the denominator </param>
		''' <returns> array of remainders </returns>
		Function fmod(ByVal denominator As Number) As INDArray

		''' <summary>
		''' remainder of division by scalar.
		''' </summary>
		''' <param name="denominator"> the denominator </param>
		''' <param name="result"> the result array </param>
		''' <returns> array of remainders </returns>
		Function fmod(ByVal denominator As Number, ByVal result As INDArray) As INDArray

		' TODO: Unused untested method.
		''' <summary>
		''' In place fmod </summary>
		''' <param name="denominator"> the array of denominators for each element in this array </param>
		''' <returns> array of remainders </returns>
		Function fmodi(ByVal denominator As INDArray) As INDArray

		' TODO: Unused untested method.
		''' <summary>
		''' In place fmod </summary>
		''' <param name="denominator"> the denominator as a scalar </param>
		''' <returns> array of remainders </returns>
		Function fmodi(ByVal denominator As Number) As INDArray

		''' <summary>
		''' This method returns index of highest value along specified dimension(s)
		''' </summary>
		''' <param name="dimension"> Dimension along which to perform the argMax operation </param>
		''' <returns> Array containing indices </returns>
		Function argMax(ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' This method returns True, if this INDArray instance is attached to some Workspace. False otherwise. </summary>
		''' <returns> True if attached to workspace, false otherwise </returns>
		ReadOnly Property Attached As Boolean

		''' <summary>
		''' This method checks, if given attached INDArray is still in scope of its parent Workspace
		''' 
		''' PLEASE NOTE: if this INDArray isn't attached to any Workspace, this method will return true </summary>
		''' <returns> true if attached to workspace. </returns>
		ReadOnly Property InScope As Boolean

		''' <summary>
		''' This method detaches INDArray from Workspace, returning copy.
		''' Basically it's dup() into new memory chunk.
		''' 
		''' PLEASE NOTE: If this INDArray instance is NOT attached - it will be returned unmodified.
		''' </summary>
		''' <returns> The attached copy of array, or original if not in workspace </returns>
		Function detach() As INDArray

		''' <summary>
		''' This method detaches INDArray from current Workspace, and attaches it to Workspace above, if any.
		''' 
		''' PLEASE NOTE: If this INDArray instance is NOT attached - it will be returned unmodified.
		''' PLEASE NOTE: If current Workspace is the top-tier one,
		''' effect will be equal to detach() call - detached copy will be returned
		''' </summary>
		''' <returns> this ndarray or a detached copy. </returns>
		Function leverage() As INDArray

		''' <summary>
		''' This method detaches INDArray from current Workspace, and attaches it to Workspace with a given Id - if a workspace
		''' with that ID exists. If no workspace with the specified ID exists, the current INDArray is returned unmodified.
		''' </summary>
		''' <seealso cref= #leverageTo(String, boolean) </seealso>
		Function leverageTo(ByVal id As String) As INDArray

		''' <summary>
		''' This method detaches INDArray from current Workspace, and attaches it to Workspace with a given Id.
		''' If enforceExistence == true, and no workspace with the specified ID exists, then an <seealso cref="Nd4jNoSuchWorkspaceException"/>
		''' is thrown. Otherwise, if enforceExistance == false and no workspace with the specified ID exists, then the current
		''' INDArray is returned unmodified (same as <seealso cref="leverage()"/>
		''' </summary>
		''' <param name="id"> ID of the workspace to leverage to </param>
		''' <param name="enforceExistence"> If true, and the specified workspace does not exist: an <seealso cref="Nd4jNoSuchWorkspaceException"/>
		'''                         will be thrown. </param>
		''' <returns> The INDArray, leveraged to the specified workspace </returns>
		''' <seealso cref= #leverageTo(String) </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: INDArray leverageTo(String id, boolean enforceExistence) throws org.nd4j.linalg.exception.Nd4jNoSuchWorkspaceException;
		Function leverageTo(ByVal id As String, ByVal enforceExistence As Boolean) As INDArray

		''' <summary>
		''' This method detaches INDArray from current Workspace, and attaches it to Workspace with a given Id, if a workspace
		''' with the given ID is open and active.
		''' 
		''' If the workspace does not exist, or is not active, the array is detached from any workspaces.
		''' </summary>
		''' <param name="id"> ID of the workspace to leverage to </param>
		''' <returns> The INDArray, leveraged to the specified workspace (if it exists and is active) otherwise the detached array </returns>
		''' <seealso cref= #leverageTo(String) </seealso>
		Function leverageOrDetach(ByVal id As String) As INDArray

		''' <summary>
		''' This method pulls this INDArray into current Workspace.
		''' 
		''' PLEASE NOTE: If there's no current Workspace - INDArray returned as is
		''' </summary>
		''' <returns> Migrated INDArray or <i>this</i> if no current workspace </returns>
		''' <seealso cref= #migrate(boolean) </seealso>
		Function migrate() As INDArray

		''' <summary>
		''' This method pulls this INDArray into current Workspace, or optionally detaches if no workspace is present.<br>
		''' That is:<br>
		''' If current workspace is present/active, INDArray is migrated to it.<br>
		''' If no current workspace is present/active, one of two things occur:
		''' 1. If detachOnNoWs arg is true: if there is no current workspace, INDArray is detached
		''' 2. If detachOnNoWs arg is false: this INDArray is returned as-is (no-op) - equivalent to <seealso cref="migrate()"/>
		''' </summary>
		''' <param name="detachOnNoWs"> If true: detach on no WS. If false and no workspace: return this. </param>
		''' <returns> Migrated INDArray </returns>
		Function migrate(ByVal detachOnNoWs As Boolean) As INDArray

		''' <summary>
		''' This method returns percentile value for this INDArray
		''' </summary>
		''' <param name="percentile"> target percentile in range of 0..100 </param>
		''' <returns> percentile value </returns>
		Function percentileNumber(ByVal percentile As Number) As Number

		''' <summary>
		''' This method returns median value for this INDArray
		''' </summary>
		''' <returns> Median value for array </returns>
		Function medianNumber() As Number

		''' <summary>
		''' This method returns median along given dimension(s) </summary>
		''' <param name="dimension"> Dimension to calculate median </param>
		''' <returns> Median along specified dimensions </returns>
		Function median(ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' This method returns percentile along given dimension(s) </summary>
		''' <param name="percentile"> target percentile in range of 0..100 </param>
		''' <param name="dimension">  Dimension to calculate percentile for </param>
		''' <returns> array with percentiles </returns>
		Function percentile(ByVal percentile As Number, ParamArray ByVal dimension() As Integer) As INDArray

		''' <summary>
		''' Add an <seealso cref="INDArray"/>
		''' to flatbuffers builder </summary>
		''' <param name="builder"> the builder to use </param>
		''' <returns> the offset to add </returns>
		Function toFlatArray(ByVal builder As FlatBufferBuilder) As Integer

		''' <summary>
		''' This method returns true if this INDArray is special case: no-value INDArray </summary>
		''' <returns> True if empty. </returns>
		ReadOnly Property Empty As Boolean

		''' <summary>
		''' This method returns shapeInformation as jvm long array </summary>
		''' <returns> shapeInformation </returns>
		Function shapeInfoJava() As Long()

		''' <summary>
		''' This method returns dtype for this INDArray </summary>
		''' <returns> Datattype </returns>
		Function dataType() As DataType

		''' <summary>
		''' This method checks if this INDArray instance is one of Real types </summary>
		''' <returns> true if data type is floating point, false otherwise </returns>
		ReadOnly Property R As Boolean

		''' <summary>
		''' This method checks if this INDArray instance is one of integer types </summary>
		''' <returns> true if integer type </returns>
		ReadOnly Property Z As Boolean

		''' <summary>
		''' This method checks if this INDArray instance has boolean type </summary>
		''' <returns> true if boolean type. </returns>
		ReadOnly Property B As Boolean

		''' <summary>
		''' This method checks if this INDArray instance has String type </summary>
		''' <returns> true if string type. </returns>
		ReadOnly Property S As Boolean

		''' <summary>
		''' This method cast elements of this INDArray to new data type
		''' </summary>
		''' <param name="dataType"> new datatype. </param>
		''' <returns> this if datatype matches, otherwise a new array of specified datatype. </returns>
		Function castTo(ByVal dataType As DataType) As INDArray

		''' <summary>
		''' This method checks if all elements within this array are non-zero (or true, in case of boolean) </summary>
		''' <returns> true if all non-zero. </returns>
		Function all() As Boolean

		''' <summary>
		''' This method checks if any of the elements within this array are non-zero (or true, in case of boolean) </summary>
		''' <returns> true if any non-zero. </returns>
		Function any() As Boolean

		''' <summary>
		''' This method checks if any of the elements within this array are non-zero (or true, in case of boolean) </summary>
		''' <returns> true if any non-zero </returns>
		Function none() As Boolean

		''' <summary>
		''' This method checks, if this INDArray instalce can use close() method </summary>
		''' <returns> true if array can be released, false otherwise </returns>
		Function closeable() As Boolean

		''' <summary>
		''' This method releases exclusive off-heap resources uses by this INDArray instance.
		''' If INDArray relies on shared resources, exception will be thrown instead
		''' 
		''' PLEASE NOTE: This method is NOT safe by any means
		''' </summary>
		Sub close()

		''' <summary>
		''' This method checks if array or its buffer was closed before </summary>
		''' <returns> true if was closed, false otherwise </returns>
		Function wasClosed() As Boolean

		''' <summary>
		''' This method returns empty array with the same dtype/order/shape as this one </summary>
		''' <returns> empty array with the same dtype/order/shape </returns>
		Function [like]() As INDArray

		''' <summary>
		''' This method returns uninitialized array with the same dtype/order/shape as this one </summary>
		''' <returns> uninitialized array with the same dtype/order/shape </returns>
		Function ulike() As INDArray

		''' <summary>
		''' Get a string representation of the array with configurable formatting </summary>
		''' <param name="options"> format options </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: String toString(@NonNull NDArrayStrings options);
		Function toString(ByVal options As NDArrayStrings) As String

		''' <summary>
		''' Get a string representation of the array
		''' </summary>
		''' <param name="maxElements"> Summarize if more than maxElements in the array </param>
		''' <param name="forceSummarize"> Force a summary instead of a full print </param>
		''' <param name="precision"> The number of decimals to print.  Doesn't print trailing 0s if negative </param>
		''' <returns> string representation of the array </returns>
		Function toString(ByVal maxElements As Long, ByVal forceSummarize As Boolean, ByVal precision As Integer) As String

		''' <summary>
		''' ToString with unlimited elements and precision </summary>
		''' <seealso cref= org.nd4j.linalg.api.ndarray.BaseNDArray#toString(long, boolean, int) </seealso>
		Function toStringFull() As String

		''' <summary>
		''' A unique ID for the INDArray object instance. Does not account for views. </summary>
		''' <returns> INDArray unique ID </returns>
		ReadOnly Property Id As Long
	End Interface

End Namespace