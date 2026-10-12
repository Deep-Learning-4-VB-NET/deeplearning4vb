Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.linalg.api.blas
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Range = org.nd4j.linalg.api.ops.random.impl.Range
Imports Distribution = org.nd4j.linalg.api.rng.distribution.Distribution
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports AtomicDouble = org.nd4j.common.primitives.AtomicDouble
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

Namespace org.nd4j.linalg.factory



	Public MustInherit Class BaseNDArrayFactory
		Implements NDArrayFactory

		Public MustOverride Function create(ByVal strings As ICollection(Of String), ByVal shape() As Long, ByVal order As Char) As INDArray
		Public MustOverride Function create(ByVal data() As Double, ByVal shape() As Long, ByVal ordering As Char) As INDArray
		Public MustOverride Function create(ByVal data() As Single, ByVal shape() As Long, ByVal ordering As Char) As INDArray
		Public MustOverride Function create(ByVal data() As Double, ByVal shape() As Long, ByVal offset As Long, ByVal order As Char?) As INDArray
		Public MustOverride Function create(ByVal data() As Single, ByVal shape() As Long, ByVal offset As Long, ByVal order As Char?) As INDArray
		Public MustOverride Function sortCooIndices(ByVal x As INDArray) As INDArray Implements NDArrayFactory.sortCooIndices
		Public MustOverride Function sort(ByVal x As INDArray, ByVal descending As Boolean, ByVal dimensions() As Integer) As INDArray
		Public MustOverride Function sort(ByVal x As INDArray, ByVal descending As Boolean) As INDArray
		Public MustOverride Function tear(ByVal tensor As INDArray, ByVal dimensions() As Integer) As INDArray()
		Public MustOverride Function create(ByVal data() As Double, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char) As INDArray
		Public MustOverride Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char) As INDArray
		Public MustOverride Function convertToNumpy(ByVal array As INDArray) As org.bytedeco.javacpp.Pointer Implements NDArrayFactory.convertToNumpy
		Public MustOverride Function createFromNpzFile(ByVal file As java.io.File) As IDictionary(Of String, INDArray) Implements NDArrayFactory.createFromNpzFile
		Public MustOverride Function createFromNpyFile(ByVal file As java.io.File) As INDArray Implements NDArrayFactory.createFromNpyFile
		Public MustOverride Function createFromNpyHeaderPointer(ByVal pointer As org.bytedeco.javacpp.Pointer) As INDArray Implements NDArrayFactory.createFromNpyHeaderPointer
		Public MustOverride Function createFromNpyPointer(ByVal pointer As org.bytedeco.javacpp.Pointer) As INDArray Implements NDArrayFactory.createFromNpyPointer
		Public MustOverride Sub convertDataEx(ByVal typeSrc As org.nd4j.linalg.api.buffer.DataTypeEx, ByVal source As org.bytedeco.javacpp.Pointer, ByVal typeDst As org.nd4j.linalg.api.buffer.DataTypeEx, ByVal buffer As DataBuffer) Implements NDArrayFactory.convertDataEx
		Public MustOverride Sub convertDataEx(ByVal typeSrc As org.nd4j.linalg.api.buffer.DataTypeEx, ByVal source As org.bytedeco.javacpp.Pointer, ByVal typeDst As org.nd4j.linalg.api.buffer.DataTypeEx, ByVal target As org.bytedeco.javacpp.Pointer, ByVal length As Long)
		Public MustOverride Sub convertDataEx(ByVal typeSrc As org.nd4j.linalg.api.buffer.DataTypeEx, ByVal source As DataBuffer, ByVal typeDst As org.nd4j.linalg.api.buffer.DataTypeEx, ByVal target As DataBuffer) Implements NDArrayFactory.convertDataEx
		Public MustOverride Function convertDataEx(ByVal typeSrc As org.nd4j.linalg.api.buffer.DataTypeEx, ByVal source As DataBuffer, ByVal typeDst As org.nd4j.linalg.api.buffer.DataTypeEx) As DataBuffer Implements NDArrayFactory.convertDataEx
		Public MustOverride Function convertDataEx(ByVal typeSrc As org.nd4j.linalg.api.buffer.DataTypeEx, ByVal source As INDArray, ByVal typeDst As org.nd4j.linalg.api.buffer.DataTypeEx) As INDArray Implements NDArrayFactory.convertDataEx
		Public MustOverride Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal offset As Long) As INDArray
		Public MustOverride Function create(ByVal data() As Double, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char) As INDArray
		Public MustOverride Function create(ByVal data() As Double, ByVal shape() As Integer, ByVal offset As Long) As INDArray
		Public MustOverride Function create(ByVal list As IList(Of INDArray), ByVal shape() As Long, ByVal ordering As Char) As INDArray
		Public MustOverride Function create(ByVal list As IList(Of INDArray), ByVal shape() As Integer, ByVal ordering As Char) As INDArray
		Public MustOverride Function create(ByVal data() As Double, ByVal shape() As Integer, ByVal ordering As Char) As INDArray
		Public MustOverride Function create(ByVal data() As Single, ByVal rows As Long, ByVal columns As Long, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char) As INDArray
		Public MustOverride Function create(ByVal data() As Single, ByVal shape() As Integer, ByVal offset As Long, ByVal order As Char?) As INDArray
		Public MustOverride Function create(ByVal data As DataBuffer, ByVal newShape() As Long, ByVal newStride() As Long, ByVal offset As Long, ByVal ordering As Char, ByVal dataType As DataType) As INDArray
		Public MustOverride Function create(ByVal data As DataBuffer, ByVal newShape() As Long, ByVal newStride() As Long, ByVal offset As Long, ByVal ews As Long, ByVal ordering As Char) As INDArray
		Public MustOverride Function create(ByVal data As DataBuffer, ByVal newShape() As Long, ByVal newStride() As Long, ByVal offset As Long, ByVal ordering As Char) As INDArray
		Public MustOverride Function create(ByVal data As DataBuffer, ByVal newShape() As Integer, ByVal newStride() As Integer, ByVal offset As Long, ByVal ordering As Char) As INDArray
		Public MustOverride Function createUninitializedDetached(ByVal dataType As DataType, ByVal ordering As Char, ByVal shape() As Long) As INDArray
		Public MustOverride Function createUninitialized(ByVal dataType As DataType, ByVal shape() As Long, ByVal ordering As Char, ByVal workspace As MemoryWorkspace) As INDArray
		Public MustOverride Function createUninitialized(ByVal shape() As Long, ByVal ordering As Char) As INDArray
		Public MustOverride Function createUninitialized(ByVal shape() As Integer, ByVal ordering As Char) As INDArray
		Public MustOverride Function create(ByVal dataType As DataType, ByVal shape() As Long, ByVal paddings() As Long, ByVal paddingOffsets() As Long, ByVal ordering As Char, ByVal workspace As MemoryWorkspace) As INDArray
		Public MustOverride Function create(ByVal dataType As DataType, ByVal shape() As Long, ByVal strides() As Long, ByVal ordering As Char, ByVal workspace As MemoryWorkspace) As INDArray
		Public MustOverride Function create(ByVal dataType As DataType, ByVal shape() As Long, ByVal ordering As Char, ByVal workspace As MemoryWorkspace) As INDArray
		Public MustOverride Function create(ByVal shape() As Long, ByVal ordering As Char) As INDArray
		Public MustOverride Function create(ByVal shape() As Integer, ByVal ordering As Char) As INDArray
		Public MustOverride Function create(ByVal buffer As DataBuffer, ByVal shape() As Integer, ByVal offset As Long) As INDArray
		Public MustOverride Function create(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char) As INDArray
		Public MustOverride Function create(ByVal data()() As Single, ByVal ordering As Char) As INDArray
		Public MustOverride Function create(ByVal floats()() As Single) As INDArray
		Public MustOverride Function empty(ByVal type As DataType) As INDArray Implements NDArrayFactory.empty
		Public MustOverride Function create(ByVal rows As Long, ByVal columns As Long, ByVal stride() As Long, ByVal offset As Long) As INDArray
		Public MustOverride Function create(ByVal list As IList(Of INDArray), ByVal shape() As Long) As INDArray
		Public MustOverride Function create(ByVal data As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long) As INDArray
		Public MustOverride Function create(ByVal data As DataBuffer, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long) As INDArray
		Public MustOverride Function create(ByVal data As DataBuffer, ByVal shape() As Long) As INDArray
		Public MustOverride Function create(ByVal data As DataBuffer, ByVal shape() As Integer) As INDArray
		Public MustOverride Function create(ByVal data() As Boolean, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Public MustOverride Function create(ByVal data() As SByte, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Public MustOverride Function create(ByVal data() As Short, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Public MustOverride Function create(ByVal data() As Integer, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Public MustOverride Function create(ByVal data() As Long, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Public MustOverride Function create(ByVal data() As Double, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Public MustOverride Function create(ByVal data() As Boolean, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Public MustOverride Function create(ByVal data() As SByte, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Public MustOverride Function create(ByVal data() As Short, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Public MustOverride Function create(ByVal data() As Integer, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Public MustOverride Function create(ByVal data() As Long, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Public MustOverride Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Public MustOverride Function create(ByVal data() As Double, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
		Public MustOverride Function create(ByVal data() As Double, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long) As INDArray
		Public MustOverride Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType) As INDArray
		Public MustOverride Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long) As INDArray
		Public MustOverride Function create(ByVal data As DataBuffer, ByVal rows As Long, ByVal columns As Long, ByVal stride() As Integer, ByVal offset As Long) As INDArray
		Public MustOverride Function hstack(ByVal arrs() As INDArray) As INDArray Implements NDArrayFactory.hstack
		Public MustOverride Function create(ByVal data As DataBuffer) As INDArray Implements NDArrayFactory.create
		Public MustOverride Function average(ByVal target As INDArray, ByVal arrays As ICollection(Of INDArray)) As INDArray
		Public MustOverride Function accumulate(ByVal target As INDArray, ByVal arrays() As INDArray) As INDArray Implements NDArrayFactory.accumulate
		Public MustOverride Function average(ByVal arrays As ICollection(Of INDArray)) As INDArray
		Public MustOverride Function average(ByVal arrays() As INDArray) As INDArray Implements NDArrayFactory.average
		Public MustOverride Function average(ByVal target As INDArray, ByVal arrays() As INDArray) As INDArray Implements NDArrayFactory.average
		Public MustOverride Sub shuffle(ByVal array As IList(Of INDArray), ByVal rnd As Random, ByVal dimensions As IList(Of Integer()))
		Public MustOverride Sub shuffle(ByVal array As ICollection(Of INDArray), ByVal rnd As Random, ByVal dimension() As Integer)
		Public MustOverride Sub shuffle(ByVal array As INDArray, ByVal rnd As Random, ByVal dimension() As Integer)
		Public MustOverride Function pullRows(ByVal source As INDArray, ByVal destination As INDArray, ByVal sourceDimension As Integer, ByVal indexes() As Integer) As INDArray
		Public MustOverride Function pullRows(ByVal source As INDArray, ByVal sourceDimension As Integer, ByVal indexes() As Long) As INDArray
		Public MustOverride Function specialConcat(ByVal dimension As Integer, ByVal toConcat() As INDArray) As INDArray
		Public MustOverride Function create(ByVal data()() As Double, ByVal ordering As Char) As INDArray
		Public MustOverride Function create(ByVal data()() As Double) As INDArray
		Public MustOverride Function toFlattened(ByVal order As Char, ByVal matrices As ICollection(Of INDArray)) As INDArray
		Public MustOverride Function create(ByVal shape() As Integer, ByVal buffer As DataBuffer) As INDArray
		Public MustOverride Sub createLapack() Implements NDArrayFactory.createLapack
		Public MustOverride Sub createLevel3() Implements NDArrayFactory.createLevel3
		Public MustOverride Sub createLevel2() Implements NDArrayFactory.createLevel2
		Public MustOverride Sub createLevel1() Implements NDArrayFactory.createLevel1
		Public MustOverride Sub createBlas() Implements NDArrayFactory.createBlas

		' We don't really care about dtype field we'll use context instead
		' protected DataType dtype;
'JAVA TO VB CONVERTER NOTE: The field order was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend order_Conflict As Char
'JAVA TO VB CONVERTER NOTE: The field blas was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend blas_Conflict As Blas
'JAVA TO VB CONVERTER NOTE: The field level1 was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend level1_Conflict As Level1
'JAVA TO VB CONVERTER NOTE: The field level2 was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend level2_Conflict As Level2
'JAVA TO VB CONVERTER NOTE: The field level3 was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend level3_Conflict As Level3
'JAVA TO VB CONVERTER NOTE: The field lapack was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend lapack_Conflict As Lapack

		Public Sub New()
		End Sub

		Public Overridable Function lapack() As Lapack Implements NDArrayFactory.lapack
			If lapack_Conflict Is Nothing Then
				createLapack()
			End If
			Return lapack_Conflict
		End Function

		Public Overridable Function blas() As Blas Implements NDArrayFactory.blas
			If blas_Conflict Is Nothing Then
				createBlas()
			End If
			Return blas_Conflict
		End Function

		Public Overridable Function level1() As Level1 Implements NDArrayFactory.level1
			If level1_Conflict Is Nothing Then
				createLevel1()
			End If
			Return level1_Conflict
		End Function

		Public Overridable Function level2() As Level2 Implements NDArrayFactory.level2
			If level2_Conflict Is Nothing Then
				createLevel2()
			End If
			Return level2_Conflict
		End Function

		Public Overridable Function level3() As Level3 Implements NDArrayFactory.level3
			If level3_Conflict Is Nothing Then
				createLevel3()
			End If
			Return level3_Conflict
		End Function

		''' 
		''' <summary>
		''' Initialize with the given data opType and ordering
		''' The ndarray factory will use this for </summary>
		''' <param name="dtype"> the data opType </param>
		''' <param name="order"> the ordering in mem </param>
		Protected Friend Sub New(ByVal dtype As DataType, ByVal order As Char?)
			' this.dtype = dtype;
			If Not String.ReferenceEquals(Char.ToLower(order), "c"c) AndAlso Not String.ReferenceEquals(Char.ToLower(order), "f"c) Then
				Throw New System.ArgumentException("Order must either be c or f")
			End If

			Me.order_Conflict = Char.ToLower(order)
		End Sub

		''' <param name="dtype"> the data opType </param>
		''' <param name="order"> the ordering </param>
		Protected Friend Sub New(ByVal dtype As DataType, ByVal order As Char)
			' this.dtype = dtype;
			If Not String.ReferenceEquals(Char.ToLower(order), "c"c) AndAlso Not String.ReferenceEquals(Char.ToLower(order), "f"c) Then
				Throw New System.ArgumentException("Order must either be c or f")
			End If

			Me.order_Conflict = Char.ToLower(order)
		End Sub

		''' <summary>
		''' Sets the order. Primarily for testing purposes
		''' </summary>
		''' <param name="order"> </param>
		Public Overridable WriteOnly Property Order Implements NDArrayFactory.setOrder As Char
			Set(ByVal order As Char)
				Preconditions.checkArgument(order = "c"c OrElse order = "f"c, "Order specified must be either c or f: got %s", order.ToString())
				Me.order_Conflict = order
			End Set
		End Property

		Public Overridable Function rand(ByVal shape() As Long, ByVal min As Double, ByVal max As Double, ByVal rng As org.nd4j.linalg.api.rng.Random) As INDArray Implements NDArrayFactory.rand
			Nd4j.Random.Seed = rng.Seed
			Return Nd4j.Distributions.createUniform(min, max).sample(shape)
		End Function

		Public Overridable Function rand(ByVal shape() As Integer, ByVal min As Double, ByVal max As Double, ByVal rng As org.nd4j.linalg.api.rng.Random) As INDArray Implements NDArrayFactory.rand
			Nd4j.Random.Seed = rng.Seed
			Return Nd4j.Distributions.createUniform(min, max).sample(shape)
		End Function

		Public Overridable Function rand(ByVal rows As Long, ByVal columns As Long, ByVal min As Double, ByVal max As Double, ByVal rng As org.nd4j.linalg.api.rng.Random) As INDArray Implements NDArrayFactory.rand
			Nd4j.Random.Seed = rng.Seed
			Return rand(New Long() {rows, columns}, min, max, rng)
		End Function

		''' <summary>
		''' Sets the data opType
		''' </summary>
		''' <param name="dtype"> </param>
		Public Overridable WriteOnly Property DType Implements NDArrayFactory.setDType As DataType
			Set(ByVal dtype As DataType)
				Debug.Assert(dtype = DataType.DOUBLE OrElse dtype = DataType.FLOAT OrElse dtype = DataType.INT, "Invalid opType passed, must be float or double")
				' this.dtype = dtype;
			End Set
		End Property

		Public Overridable Function create(ByVal shape() As Integer, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray Implements NDArrayFactory.create
			Return create(shape, Nd4j.createBuffer(shape, dataType))
		End Function

		''' <summary>
		''' Returns the order for this ndarray for internal data storage
		''' </summary>
		''' <returns> the order (c or f) </returns>
		Public Overridable Function order() As Char Implements NDArrayFactory.order
			Return order_Conflict
		End Function

		''' <summary>
		''' Returns the data opType for this ndarray
		''' </summary>
		''' <returns> the data opType for this ndarray </returns>
		Public Overridable Function dtype() As DataType Implements NDArrayFactory.dtype
			Return Nd4j.dataType()
		End Function

		Public Overridable Function create(ByVal ints() As Integer, ByVal ints1() As Integer, ByVal stride() As Integer, ByVal offset As Long) As INDArray Implements NDArrayFactory.create
			Return create(Nd4j.createBuffer(ints), ints1, stride, offset)
		End Function

		Public Overridable Function create(ByVal rows As Long, ByVal columns As Long, ByVal ordering As Char) As INDArray Implements NDArrayFactory.create
			Return create(New Long() {rows, columns}, ordering)
		End Function


		''' <summary>
		''' Returns a vector with all of the elements in every nd array
		''' equal to the sum of the lengths of the ndarrays
		''' </summary>
		''' <param name="matrices"> the ndarrays to getFloat a flattened representation of </param>
		''' <returns> the flattened ndarray </returns>
		Public Overridable Function toFlattened(ByVal matrices As ICollection(Of INDArray)) As INDArray Implements NDArrayFactory.toFlattened
			Return toFlattened(AscW("c"c), matrices.ToArray())
		End Function

		Public Overridable Function toFlattened(Of T1 As INDArray)(ByVal length As Integer, ParamArray ByVal matrices() As IEnumerator(Of T1)) As INDArray Implements NDArrayFactory.toFlattened
			Dim arr As IList(Of INDArray) = New List(Of INDArray)()
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: for (Iterator<? extends org.nd4j.linalg.api.ndarray.INDArray> arrs : matrices)
			For Each arrs As IEnumerator(Of INDArray) In matrices
				Do While arrs.MoveNext()
					arr.Add(arrs.Current)
				Loop
			Next arrs
			Return toFlattened(arr)
		End Function

		''' <summary>
		''' Returns a column vector where each entry is the nth bilinear
		''' product of the nth slices of the two tensors.
		''' </summary>
		Public Overridable Function bilinearProducts(ByVal curr As INDArray, ByVal [in] As INDArray) As INDArray Implements NDArrayFactory.bilinearProducts
			Preconditions.checkArgument(curr.rank() = 3, "Argument 'curr' must be rank 3. Got input with rank: %s", curr.rank())
			If [in].columns() <> 1 Then
				Throw New AssertionError("Expected a column vector")
			End If
			If [in].rows() <> curr.size(curr.shape().Length - 1) Then
				Throw New AssertionError("Number of rows in the input does not match number of columns in tensor")
			End If
			If curr.size(curr.shape().Length - 2) <> curr.size(curr.shape().Length - 1) Then
				Throw New AssertionError("Can only perform this operation on a SimpleTensor with square slices")
			End If

			Dim ret As INDArray = Nd4j.create(curr.slices(), 1)
			Dim inT As INDArray = [in].transpose()
			Dim i As Integer = 0
			Do While i < curr.slices()
				Dim slice As INDArray = curr.slice(i)
				Dim inTTimesSlice As INDArray = inT.mmul(slice)
				ret.putScalar(i, Nd4j.BlasWrapper.dot(inTTimesSlice, [in]))
				i += 1
			Loop
			Return ret
		End Function

		Public Overridable Function toFlattened(ParamArray ByVal matrices() As INDArray) As INDArray Implements NDArrayFactory.toFlattened
			Return toFlattened(Nd4j.order(), java.util.Arrays.asList(matrices))
		End Function


		Public Overridable Function toFlattened(ByVal order As Char, ParamArray ByVal matrices() As INDArray) As INDArray Implements NDArrayFactory.toFlattened
			Return toFlattened(AscW(order), java.util.Arrays.asList(matrices))
		End Function

		''' <summary>
		''' Create the identity ndarray
		''' </summary>
		''' <param name="n"> the number for the identity
		''' @return </param>
		Public Overridable Function eye(ByVal n As Long) As INDArray Implements NDArrayFactory.eye
			Dim ret As INDArray = Nd4j.create(n, n)
			For i As Integer = 0 To n - 1
				ret.put(i, i, 1.0)
			Next i

			Return ret.reshape(ChrW(n), n)

		End Function

		''' <summary>
		''' Rotate a matrix 90 degrees
		''' </summary>
		''' <param name="toRotate"> the matrix to rotate </param>
		''' <returns> the rotated matrix </returns>
		Public Overridable Sub rot90(ByVal toRotate As INDArray) Implements NDArrayFactory.rot90
			If Not toRotate.Matrix Then
				Throw New System.ArgumentException("Only rotating matrices")
			End If

			Dim start As INDArray = toRotate.transpose()
			Dim i As Integer = 0
			Do While i < start.rows()
				start.putRow(i, reverse(start.getRow(i)))
				i += 1
			Loop

		End Sub

		''' <summary>
		''' Reverses the passed in matrix such that m[0] becomes m[m.length - 1] etc
		''' </summary>
		''' <param name="reverse"> the matrix to reverse </param>
		''' <returns> the reversed matrix </returns>
		Public Overridable Function rot(ByVal reverse As INDArray) As INDArray Implements NDArrayFactory.rot
			Dim ret As INDArray = Nd4j.create(reverse.shape())
			If reverse.Vector Then
				Return Me.reverse(reverse)
			Else
				Dim i As Integer = 0
				Do While i < reverse.slices()
					ret.putSlice(i, Me.reverse(reverse.slice(i)))
					i += 1
				Loop
			End If
			Return ret.reshape(reverse.shape())
		End Function

		''' <summary>
		''' Reverses the passed in matrix such that m[0] becomes m[m.length - 1] etc
		''' </summary>
		''' <param name="reverse"> the matrix to reverse </param>
		''' <returns> the reversed matrix </returns>

'JAVA TO VB CONVERTER NOTE: The parameter reverse was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function reverse(ByVal reverse_Conflict As INDArray) As INDArray Implements NDArrayFactory.reverse
			' FIXME: native method should be used instead
			Dim rev As INDArray = reverse_Conflict.reshape(ChrW(-1))
			Dim ret As INDArray = Nd4j.create(rev.shape())
			Dim count As Integer = 0
			For i As Long = rev.length() - 1 To 0 Step -1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret.putScalar(count++, rev.getFloat(i));
				ret.putScalar(count, rev.getFloat(i))
					count += 1

			Next i

			Return ret.reshape(reverse_Conflict.shape())
		End Function

		''' <summary>
		''' Array of evenly spaced values.
		''' </summary>
		''' <param name="begin"> the begin of the range </param>
		''' <param name="end">   the end of the range </param>
		''' <returns> the range vector </returns>
		Public Overridable Function arange(ByVal begin As Double, ByVal [end] As Double, ByVal [step] As Double) As INDArray Implements NDArrayFactory.arange
			Dim length As Long = CLng(Math.Truncate(Math.Floor(([end]-begin)/[step])))
			Dim op As DynamicCustomOp = New Range(begin, [end], [step], DataType.FLOAT)
			Dim [out] As INDArray = Nd4j.create(op.calculateOutputShape()(0))
			op.setOutputArgument(0, [out])
			Nd4j.exec(op)
			Return [out]
		End Function

		''' <summary>
		''' Copy a to b
		''' </summary>
		''' <param name="a"> the origin matrix </param>
		''' <param name="b"> the destination matrix </param>
		Public Overridable Sub copy(ByVal a As INDArray, ByVal b As INDArray) Implements NDArrayFactory.copy
			b.assign(a)
		End Sub

		''' <summary>
		''' Generates a random matrix between min and max
		''' </summary>
		''' <param name="shape"> the number of rows of the matrix </param>
		''' <param name="min">   the minimum number </param>
		''' <param name="max">   the maximum number </param>
		''' <param name="rng">   the rng to use </param>
		''' <returns> a random matrix of the specified shape and range </returns>
		Public Overridable Function rand(ByVal shape() As Integer, ByVal min As Single, ByVal max As Single, ByVal rng As org.nd4j.linalg.api.rng.Random) As INDArray Implements NDArrayFactory.rand
			'ensure shapes that wind up being scalar end up with the write shape
			If shape.Length = 1 AndAlso shape(0) = 0 Then
				shape = New Integer() {1, 1}
			End If
			Return Nd4j.Distributions.createUniform(min, max).sample(shape)
		End Function

		Public Overridable Function rand(ByVal shape() As Long, ByVal min As Single, ByVal max As Single, ByVal rng As org.nd4j.linalg.api.rng.Random) As INDArray Implements NDArrayFactory.rand
			'ensure shapes that wind up being scalar end up with the write shape
			If shape.Length = 1 AndAlso shape(0) = 0 Then
				shape = New Long() {1, 1}
			End If
			Return Nd4j.Distributions.createUniform(min, max).sample(shape)
		End Function

		''' <summary>
		''' Generates a random matrix between min and max
		''' </summary>
		''' <param name="rows">    the number of rows of the matrix </param>
		''' <param name="columns"> the number of columns in the matrix </param>
		''' <param name="min">     the minimum number </param>
		''' <param name="max">     the maximum number </param>
		''' <param name="rng">     the rng to use </param>
		''' <returns> a random matrix of the specified shape and range </returns>
		Public Overridable Function rand(ByVal rows As Long, ByVal columns As Long, ByVal min As Single, ByVal max As Single, ByVal rng As org.nd4j.linalg.api.rng.Random) As INDArray Implements NDArrayFactory.rand
			Return rand(New Long() {rows, columns}, min, max, rng)
		End Function

		''' <summary>
		''' Merge the vectors and append a bias.
		''' Each vector must be either row or column vectors.
		''' An exception is thrown for inconsistency (mixed row and column vectors)
		''' </summary>
		''' <param name="vectors"> the vectors to merge </param>
		''' <returns> the merged ndarray appended with the bias </returns>
		Public Overridable Function appendBias(ParamArray ByVal vectors() As INDArray) As INDArray Implements NDArrayFactory.appendBias
			Preconditions.checkArgument(vectors IsNot Nothing AndAlso vectors.Length > 0, "vectros must be not null and have at least one element")
			Dim size As Integer = 0
			For Each vector As INDArray In vectors
				size += vector.rows()
				Preconditions.checkArgument(vectors(0).dataType() = vector.dataType(), "appendBias: all arrays must have same type")
			Next vector


			Dim result As INDArray = Nd4j.create(vectors(0).dataType(), size + 1, vectors(0).columns())
			Dim index As Integer = 0
			For Each vector As INDArray In vectors
				Dim put As INDArray = toFlattened(vector, Nd4j.ones(vector.dataType(), 1))
				result.put(New INDArrayIndex() {NDArrayIndex.interval(index, index + vector.rows() + 1), NDArrayIndex.interval(0, vectors(0).columns())}, put)
				index += vector.rows()
			Next vector

			Return result
		End Function

		''' <summary>
		''' Create a random ndarray with the given shape using the given rng
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the number of columns in the matrix </param>
		''' <param name="r">       the random generator to use </param>
		''' <returns> the random ndarray with the specified shape </returns>
		Public Overridable Function rand(ByVal rows As Long, ByVal columns As Long, ByVal r As org.nd4j.linalg.api.rng.Random) As INDArray Implements NDArrayFactory.rand
			Return rand(New Long() {rows, columns}, r)
		End Function

		''' <summary>
		''' Create a random ndarray with the given shape using the given rng
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <param name="seed">    the  seed to use </param>
		''' <returns> the random ndarray with the specified shape </returns>
		Public Overridable Function rand(ByVal rows As Long, ByVal columns As Long, ByVal seed As Long) As INDArray Implements NDArrayFactory.rand
			Nd4j.Random.Seed = seed
			Return rand(New Long() {rows, columns}, Nd4j.Random)
		End Function

		''' <summary>
		''' Create a random ndarray with the given shape using
		''' the current time as the seed
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the number of columns in the matrix </param>
		''' <returns> the random ndarray with the specified shape </returns>
		Public Overridable Function rand(ByVal rows As Long, ByVal columns As Long) As INDArray Implements NDArrayFactory.rand
			Return rand(New Long() {rows, columns}, DateTimeHelper.CurrentUnixTimeMillis())
		End Function

		''' <summary>
		''' Create a random (uniform 0-1) NDArray with the specified shape and order </summary>
		''' <param name="order">      Order ('c' or 'f') of the output array </param>
		''' <param name="rows">       Number of rows of the output array </param>
		''' <param name="columns">    Number of columns of the output array </param>
		Public Overridable Function rand(ByVal order As Char, ByVal rows As Long, ByVal columns As Long) As INDArray Implements NDArrayFactory.rand
			Shape.assertValidOrder(order)
			Return Nd4j.Random.nextDouble(order, New Long() {rows, columns})
		End Function

		''' <summary>
		''' Random normal using the given rng
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the number of columns in the matrix </param>
		''' <param name="r">       the random generator to use
		''' @return </param>
		Public Overridable Function randn(ByVal rows As Long, ByVal columns As Long, ByVal r As org.nd4j.linalg.api.rng.Random) As INDArray Implements NDArrayFactory.randn
			Return randn(New Long() {rows, columns}, r)
		End Function

		''' <summary>
		''' Random normal using the current time stamp
		''' as the seed
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the number of columns in the matrix
		''' @return </param>
		Public Overridable Function randn(ByVal rows As Long, ByVal columns As Long) As INDArray Implements NDArrayFactory.randn
			Return randn(New Long() {rows, columns}, DateTimeHelper.CurrentUnixTimeMillis())
		End Function

		''' <summary>
		''' Generate a random normal N(0,1) with the specified order and shape </summary>
		''' <param name="order">   Order of the output array </param>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the number of columns in the matrix
		''' @return </param>
		Public Overridable Function randn(ByVal order As Char, ByVal rows As Long, ByVal columns As Long) As INDArray Implements NDArrayFactory.randn
			Shape.assertValidOrder(order)
			Return Nd4j.Random.nextGaussian(order, New Long() {rows, columns})
		End Function

		''' <summary>
		''' Random normal using the specified seed
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the number of columns in the matrix
		''' @return </param>
		Public Overridable Function randn(ByVal rows As Long, ByVal columns As Long, ByVal seed As Long) As INDArray Implements NDArrayFactory.randn
			Nd4j.Random.Seed = seed
			Return randn(New Long() {rows, columns}, Nd4j.Random)
		End Function

		''' <summary>
		''' Create a random ndarray with the given shape using the given rng
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <param name="r">     the random generator to use </param>
		''' <returns> the random ndarray with the specified shape </returns>
		Public Overridable Function rand(ByVal shape() As Integer, ByVal r As Distribution) As INDArray Implements NDArrayFactory.rand
			Dim ret As INDArray = r.sample(shape)
			Return ret
		End Function

		''' <summary>
		''' Create a random ndarray with the given shape using the given rng
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <param name="r">     the random generator to use </param>
		''' <returns> the random ndarray with the specified shape </returns>
		Public Overridable Function rand(ByVal shape() As Integer, ByVal r As org.nd4j.linalg.api.rng.Random) As INDArray Implements NDArrayFactory.rand
			Dim ret As INDArray = r.nextDouble(shape)
			Return ret
		End Function

		Public Overridable Function rand(ByVal shape() As Long, ByVal r As org.nd4j.linalg.api.rng.Random) As INDArray Implements NDArrayFactory.rand
			Dim ret As INDArray = r.nextDouble(shape)
			Return ret
		End Function

		''' <summary>
		''' Create a random ndarray with the given shape using the given rng
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <param name="seed">  the  seed to use </param>
		''' <returns> the random ndarray with the specified shape </returns>
		Public Overridable Function rand(ByVal shape() As Integer, ByVal seed As Long) As INDArray Implements NDArrayFactory.rand
			Nd4j.Random.Seed = seed
			Return rand(shape, Nd4j.Random)
		End Function

		Public Overridable Function rand(ByVal shape() As Long, ByVal seed As Long) As INDArray Implements NDArrayFactory.rand
			Nd4j.Random.Seed = seed
			Return rand(shape, Nd4j.Random)
		End Function

		''' <summary>
		''' Create a random ndarray with the given shape using
		''' the current time as the seed
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> the random ndarray with the specified shape </returns>
		Public Overridable Function rand(ByVal shape() As Integer) As INDArray Implements NDArrayFactory.rand
			Return rand(shape, DateTimeHelper.CurrentUnixTimeMillis())
		End Function

		Public Overridable Function rand(ByVal shape() As Long) As INDArray Implements NDArrayFactory.rand
			Return rand(shape, DateTimeHelper.CurrentUnixTimeMillis())
		End Function

		''' <summary>
		''' Create a random ndarray with the given shape and order
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> the random ndarray with the specified shape </returns>
		Public Overridable Function rand(ByVal order As Char, ByVal shape() As Integer) As INDArray Implements NDArrayFactory.rand
			Shape.assertValidOrder(order)
			Return Nd4j.Random.nextDouble(order, shape)
		End Function

		Public Overridable Function rand(ByVal order As Char, ByVal shape() As Long) As INDArray Implements NDArrayFactory.rand
			Shape.assertValidOrder(order)
			Return Nd4j.Random.nextDouble(order, shape)
		End Function

		''' <summary>
		''' Random normal using the given rng
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <param name="r">     the random generator to use
		''' @return </param>
		Public Overridable Function randn(ByVal shape() As Integer, ByVal r As org.nd4j.linalg.api.rng.Random) As INDArray Implements NDArrayFactory.randn
			Return r.nextGaussian(shape)
		End Function

		Public Overridable Function randn(ByVal shape() As Long, ByVal r As org.nd4j.linalg.api.rng.Random) As INDArray Implements NDArrayFactory.randn
			Return r.nextGaussian(shape)
		End Function

		''' <summary>
		''' Random normal using the current time stamp
		''' as the seed
		''' </summary>
		''' <param name="shape"> the shape of the ndarray
		''' @return </param>
		Public Overridable Function randn(ByVal order As Char, ByVal shape() As Integer) As INDArray Implements NDArrayFactory.randn
			Shape.assertValidOrder(order)
			Return Nd4j.Random.nextGaussian(order, shape)
		End Function

		Public Overridable Function randn(ByVal order As Char, ByVal shape() As Long) As INDArray Implements NDArrayFactory.randn
			Shape.assertValidOrder(order)
			Return Nd4j.Random.nextGaussian(order, shape)
		End Function

		''' <summary>
		''' Random normal N(0,1) with the specified shape and
		''' </summary>
		''' <param name="shape"> the shape of the ndarray
		''' @return </param>
		Public Overridable Function randn(ByVal shape() As Integer) As INDArray Implements NDArrayFactory.randn
			Return randn(shape, DateTimeHelper.CurrentUnixTimeMillis())
		End Function

		Public Overridable Function randn(ByVal shape() As Long) As INDArray Implements NDArrayFactory.randn
			Return randn(shape, DateTimeHelper.CurrentUnixTimeMillis())
		End Function

		''' <summary>
		''' Random normal using the specified seed
		''' </summary>
		''' <param name="shape"> the shape of the ndarray
		''' @return </param>
		Public Overridable Function randn(ByVal shape() As Integer, ByVal seed As Long) As INDArray Implements NDArrayFactory.randn
			Nd4j.Random.Seed = seed
			Return randn(shape, Nd4j.Random)
		End Function

		Public Overridable Function randn(ByVal shape() As Long, ByVal seed As Long) As INDArray Implements NDArrayFactory.randn
			Nd4j.Random.Seed = seed
			Return randn(shape, Nd4j.Random)
		End Function

		''' <summary>
		''' Creates a row vector with the data
		''' </summary>
		''' <param name="data"> the columns of the ndarray </param>
		''' <returns> the created ndarray </returns>
		Public Overridable Function create(ByVal data() As Double) As INDArray Implements NDArrayFactory.create
			Return create(data, New Integer() {1, data.Length})
		End Function

		''' <summary>
		''' Creates a row vector with the data
		''' </summary>
		''' <param name="data"> the columns of the ndarray </param>
		''' <returns> the created ndarray </returns>
		Public Overridable Function create(ByVal data() As Single) As INDArray Implements NDArrayFactory.create
			Return create(data, New Long() {data.Length})
		End Function

		''' <summary>
		''' Creates a row vector with the specified number of columns
		''' </summary>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <returns> the created ndarray </returns>
		Public Overridable Function create(ByVal columns As Long) As INDArray Implements NDArrayFactory.create
			Return create(New Long() {columns})
		End Function

		''' <summary>
		''' Creates a row vector with the specified number of columns
		''' </summary>
		''' <param name="rows">    the rows of the ndarray </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <returns> the created ndarray </returns>
		Public Overridable Function zeros(ByVal rows As Long, ByVal columns As Long) As INDArray Implements NDArrayFactory.zeros
			Return zeros(New Long() {rows, columns})
		End Function

		''' <summary>
		''' This method produces concatenated array, that consist from tensors, fetched from source array, against some dimension and specified indexes
		''' </summary>
		''' <param name="source"> source tensor </param>
		''' <param name="sourceDimension"> dimension of source tensor </param>
		''' <param name="indexes"> indexes from source array
		''' @return </param>
		Public Overridable Function pullRows(ByVal source As INDArray, ByVal sourceDimension As Integer, ByVal indexes() As Integer, ByVal order As Char) As INDArray Implements NDArrayFactory.pullRows
			Shape.assertValidOrder(order)
			Dim vectorLength As Long = source.shape()(sourceDimension)
			Dim ret As INDArray = Nd4j.createUninitialized(New Long() {indexes.Length, vectorLength}, order)

			For cnt As Integer = 0 To indexes.Length - 1
				ret.putRow(cnt, source.tensorAlongDimension(CInt(indexes(cnt)), sourceDimension))
			Next cnt

			Return ret
		End Function

		''' <summary>
		''' This method produces concatenated array, that consist from tensors, fetched from source array, against some dimension and specified indexes
		''' </summary>
		''' <param name="source">          source tensor </param>
		''' <param name="sourceDimension"> dimension of source tensor </param>
		''' <param name="indexes">         indexes from source array
		''' @return </param>
		Public Overridable Function pullRows(ByVal source As INDArray, ByVal sourceDimension As Integer, ByVal indexes() As Integer) As INDArray Implements NDArrayFactory.pullRows
			Return pullRows(source, sourceDimension, indexes, Nd4j.order())
		End Function

		''' <summary>
		''' Creates a row vector with the specified number of columns
		''' </summary>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <returns> the created ndarray </returns>
		Public Overridable Function zeros(ByVal columns As Long) As INDArray Implements NDArrayFactory.zeros
			Return zeros(New Long() {columns})
		End Function

		''' <summary>
		''' Creates an ndarray with the specified value
		''' as the  only value in the ndarray
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <param name="value"> the value to assign </param>
		''' <returns> the created ndarray </returns>
		Public Overridable Function valueArrayOf(ByVal shape() As Integer, ByVal value As Double) As INDArray Implements NDArrayFactory.valueArrayOf
			Dim ret As INDArray = Nd4j.createUninitialized(shape, Nd4j.order())
			ret.assign(value)
			Return ret
		End Function

		Public Overridable Function valueArrayOf(ByVal shape() As Long, ByVal value As Double) As INDArray Implements NDArrayFactory.valueArrayOf
			Dim ret As INDArray = Nd4j.createUninitialized(shape, Nd4j.order())
			ret.assign(value)
			Return ret
		End Function

		Public Overridable Function create(ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char) As INDArray Implements NDArrayFactory.create
			Shape.assertValidOrder(ordering)
			'ensure shapes that wind up being scalar end up with the write shape
			Dim length As Long = ArrayUtil.prodLong(shape)
			If length = 0 Then
				Return scalar(0.0)
			End If
			Return create(Nd4j.createBuffer(length), shape, stride, offset, ordering)
		End Function

		''' <summary>
		''' Creates a row vector with the specified number of columns
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <param name="value">   the value to assign </param>
		''' <returns> the created ndarray </returns>
		Public Overridable Function valueArrayOf(ByVal rows As Long, ByVal columns As Long, ByVal value As Double) As INDArray Implements NDArrayFactory.valueArrayOf
			Dim create As INDArray = createUninitialized(New Long() {rows, columns}, Nd4j.order())
			create.assign(value)
			Return create
		End Function

		''' <summary>
		''' Creates a row vector with the specified number of columns
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <returns> the created ndarray </returns>
		Public Overridable Function ones(ByVal rows As Long, ByVal columns As Long) As INDArray Implements NDArrayFactory.ones
			Return ones(New Long() {rows, columns})
		End Function

		''' <summary>
		''' Creates a row vector with the specified number of columns
		''' </summary>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <returns> the created ndarray </returns>
		Public Overridable Function ones(ByVal columns As Long) As INDArray Implements NDArrayFactory.ones
			Return ones(New Long() {columns})
		End Function

		Public Overridable Function create(ByVal data() As Single, ByVal shape() As Integer, ByVal ordering As Char) As INDArray Implements NDArrayFactory.create
			Shape.assertValidOrder(ordering)
			Dim length As Long = ArrayUtil.prodLong(shape)
			If length = 0 Then
				Return scalar(0.0)
			End If
			Return create(Nd4j.createBuffer(data), shape, Nd4j.getStrides(shape, ordering), 0, ordering)
		End Function

		''' <summary>
		''' concatenate ndarrays along a dimension
		''' </summary>
		''' <param name="dimension"> the dimension to concatenate along </param>
		''' <param name="toConcat">  the ndarrays to concatenate </param>
		''' <returns> the concatenate ndarrays </returns>
		Public Overridable Function concat(ByVal dimension As Integer, ParamArray ByVal toConcat() As INDArray) As INDArray Implements NDArrayFactory.concat
			If toConcat.Length = 1 Then
				Return toConcat(0)
			End If
			Dim sumAlongDim As Integer = 0
			Dim allC As Boolean = toConcat(0).ordering() = "c"c

			Dim outputShape() As Long = ArrayUtil.copy(toConcat(0).shape())
			outputShape(dimension) = sumAlongDim

			For i As Integer = 0 To toConcat.Length - 1
				sumAlongDim += toConcat(i).size(dimension)
				allC = allC AndAlso toConcat(i).ordering() = "c"c
				Dim j As Integer = 0
				Do While j < toConcat(i).rank()
					If j <> dimension AndAlso toConcat(i).size(j) <> outputShape(j) AndAlso Not toConcat(i).Vector Then
						Throw New System.ArgumentException("Illegal concatenation at array " & i & " and shape element " & j)
					End If
					j += 1
				Loop
			Next i



			Dim sortedStrides() As Long = Nd4j.getStrides(outputShape)

			Dim ret As INDArray = Nd4j.create(outputShape, sortedStrides)
			allC = allC And (ret.ordering() = "c"c)

			If toConcat(0).Scalar Then
				Dim retLinear As INDArray = ret.reshape(ChrW(-1))
				Dim i As Integer = 0
				Do While i < retLinear.length()
					retLinear.putScalar(i, toConcat(i).getDouble(0))
					i += 1
				Loop
				Return ret
			End If



			If dimension = 0 AndAlso allC Then
				Dim currBuffer As Integer = 0
				Dim currBufferOffset As Integer = 0
				Dim i As Integer = 0
				Do While i < ret.length()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret.data().put(i, toConcat[currBuffer].data().getDouble(toConcat[currBuffer].offset() + currBufferOffset++));
					ret.data().put(i, toConcat(currBuffer).data().getDouble(toConcat(currBuffer).offset() + currBufferOffset))
						currBufferOffset += 1
					If currBufferOffset >= toConcat(currBuffer).length() Then
						currBuffer += 1
						currBufferOffset = 0
					End If
					i += 1
				Loop

				Return ret
			End If

			Dim arrOffset As Integer = 0

			If ret.tensorsAlongDimension(dimension) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Dim retAlongDimensionArrays(CInt(ret.tensorsAlongDimension(dimension)) - 1) As INDArray
			For i As Integer = 0 To retAlongDimensionArrays.Length - 1
				retAlongDimensionArrays(i) = ret.tensorAlongDimension(i, dimension)
			Next i

			For Each arr As INDArray In toConcat
				Dim arrTensorLength As Long = -1

				If arr.tensorsAlongDimension(dimension) <> ret.tensorsAlongDimension(dimension) Then
					Throw New System.InvalidOperationException("Illegal concatenate. Tensors along dimension must be same length.")
				End If


				Dim i As Integer = 0
				Do While i < arr.tensorsAlongDimension(dimension)
					Dim retLinear As INDArray = retAlongDimensionArrays(i)
					Dim arrTensor As INDArray = arr.tensorAlongDimension(i, dimension)

					arrTensorLength = arrTensor.length()
					For j As Integer = 0 To arrTensor.length() - 1
						Dim idx As Integer = j + arrOffset
						retLinear.putScalar(idx, arrTensor.getDouble(j))
					Next j
					i += 1
				Loop

				'bump the sliding window
				arrOffset += arrTensorLength

			Next arr

			Return ret
		End Function

		''' <summary>
		''' Concatenates two matrices horizontally.
		''' Matrices must have identical
		''' numbers of rows.
		''' </summary>
		''' <param name="arrs"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray hstack(@NonNull INDArray... arrs)
		Public Overridable Function hstack(ParamArray ByVal arrs() As INDArray) As INDArray
			Dim firstRank As Integer = arrs(0).rank()
			Preconditions.checkState(firstRank > 0 AndAlso firstRank <= 2, "Only rank 1 and 2 arrays may be horizontally stacked; first input has rank %ndRank shape %nhShape", arrs(0), arrs(0))
			For i As Integer = 1 To arrs.Length - 1
				Preconditions.checkState(firstRank = arrs(i).rank(), "Array ranks must be equal for horizontal stacking, arrs[0].rank=%s, arrs[%s].rank=%s", arrs(0).rank(), i, arrs(i).rank())
			Next i
			If firstRank = 1 Then
				Return Nd4j.concat(0, arrs)
			Else
				Return Nd4j.concat(1, arrs)
			End If
		End Function

		''' <summary>
		''' Concatenates two matrices vertically. Matrices must have identical
		''' numbers of columns.
		''' </summary>
		''' <param name="arrs"> </param>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray vstack(final org.nd4j.linalg.api.ndarray.INDArray... arrs)
		Public Overridable Function vstack(ParamArray ByVal arrs() As INDArray) As INDArray Implements NDArrayFactory.vstack
			Return Nd4j.concat(0, arrs)
		End Function


		''' <summary>
		''' Create an ndarray of zeros
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> an ndarray with ones filled in </returns>
		Public Overridable Function zeros(ByVal shape() As Integer) As INDArray Implements NDArrayFactory.zeros
			Dim ret As INDArray = create(shape)
			Return ret
		End Function

		Public Overridable Function zeros(ByVal shape() As Long) As INDArray Implements NDArrayFactory.zeros
			Dim ret As INDArray = create(shape)
			Return ret
		End Function


		''' <summary>
		''' Create an ndarray of ones
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> an ndarray with ones filled in </returns>
		Public Overridable Function ones(ByVal shape() As Integer) As INDArray Implements NDArrayFactory.ones
			Dim ret As INDArray = createUninitialized(shape, Nd4j.order())
			ret.assign(1)
			Return ret
		End Function

		Public Overridable Function ones(ByVal shape() As Long) As INDArray Implements NDArrayFactory.ones
			'ensure shapes that wind up being scalar end up with the write shape
			Dim ret As INDArray = createUninitialized(shape, Nd4j.order())
			ret.assign(1)
			Return ret
		End Function


		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="data">    the data to use with the ndarray </param>
		''' <param name="rows">    the rows of the ndarray </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <param name="stride">  the stride for the ndarray </param>
		''' <param name="offset">  the offset of the ndarray </param>
		''' <returns> the instance </returns>
		Public Overridable Function create(ByVal data() As Single, ByVal rows As Long, ByVal columns As Long, ByVal stride() As Integer, ByVal offset As Long) As INDArray Implements NDArrayFactory.create
			Return create(data, New Long() {rows, columns}, ArrayUtil.toLongArray(stride), offset)
		End Function


		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape">  the shape of the ndarray </param>
		''' <param name="stride"> the stride for the ndarray </param>
		''' <param name="offset"> the offset of the ndarray </param>
		''' <returns> the instance </returns>
		Public MustOverride Function create(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long) As INDArray Implements NDArrayFactory.create


		''' <summary>
		''' Create an ndrray with the specified shape
		''' </summary>
		''' <param name="data">  the data to use with tne ndarray </param>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> the created ndarray </returns>
		Public Overridable Function create(ByVal data() As Double, ByVal shape() As Integer) As INDArray Implements NDArrayFactory.create
			Return create(data, shape, Nd4j.getStrides(shape), 0)
		End Function


		''' <summary>
		''' Create an ndrray with the specified shape
		''' </summary>
		''' <param name="data">  the data to use with tne ndarray </param>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> the created ndarray </returns>
		Public Overridable Function create(ByVal data() As Single, ByVal shape() As Integer) As INDArray Implements NDArrayFactory.create
			Return create(data, shape, Nd4j.getStrides(shape), 0)
		End Function

		Public Overridable Function create(ByVal data() As Single, ByVal shape() As Long) As INDArray Implements NDArrayFactory.create
			Return create(data, shape, Nd4j.getStrides(shape), 0)
		End Function

		Public Overridable Function create(ByVal data() As Double, ByVal shape() As Long) As INDArray Implements NDArrayFactory.create
			Return create(data, shape, Nd4j.getStrides(shape), 0)
		End Function


		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="data">    the data to use with tne ndarray </param>
		''' <param name="rows">    the rows of the ndarray </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <param name="stride">  the stride for the ndarray </param>
		''' <param name="offset">  the offset of the ndarray </param>
		''' <returns> the instance </returns>
		Public Overridable Function create(ByVal data() As Double, ByVal rows As Long, ByVal columns As Long, ByVal stride() As Integer, ByVal offset As Long) As INDArray Implements NDArrayFactory.create
			Return create(data, New Long() {rows, columns}, ArrayUtil.toLongArray(stride), offset)
		End Function


		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape">  the shape of the ndarray </param>
		''' <param name="stride"> the stride for the ndarray </param>
		''' <param name="offset"> the offset of the ndarray </param>
		''' <returns> the instance </returns>
		Public MustOverride Function create(ByVal data() As Double, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long) As INDArray Implements NDArrayFactory.create

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> the instance </returns>
		Public MustOverride Function create(ByVal list As IList(Of INDArray), ByVal shape() As Integer) As INDArray Implements NDArrayFactory.create


		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="rows">    the rows of the ndarray </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <param name="stride">  the stride for the ndarray </param>
		''' <param name="offset">  the offset of the ndarray </param>
		''' <returns> the instance </returns>
		Public Overridable Function create(ByVal rows As Long, ByVal columns As Long, ByVal stride() As Integer, ByVal offset As Long) As INDArray Implements NDArrayFactory.create
	'        
	'        if (Nd4j.dataType() == DataType.DOUBLE)
	'            return create(new double[rows * columns], new int[] {rows, columns}, stride, offset);
	'        if (Nd4j.dataType() == DataType.FLOAT || Nd4j.dataType() == DataType.HALF)
	'            return create(new float[rows * columns], new int[] {rows, columns}, stride, offset);
	'        if (Nd4j.dataType() == DataType.INT)
	'            return create(new int[rows * columns], new int[] {rows, columns}, stride, offset);
	'        throw new IllegalStateException("Illegal data opType " + Nd4j.dataType());
	'        

			Throw New System.NotSupportedException()
		End Function


		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape">  the shape of the ndarray </param>
		''' <param name="stride"> the stride for the ndarray </param>
		''' <param name="offset"> the offset of the ndarray </param>
		''' <returns> the instance </returns>
		Public Overridable Function create(ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long) As INDArray Implements NDArrayFactory.create
			Dim buffer As DataBuffer = Nd4j.createBuffer(ArrayUtil.prodLong(shape))
			Return create(buffer, shape, stride, offset)
		End Function

		Public Overridable Function create(ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long) As INDArray Implements NDArrayFactory.create
			Dim buffer As DataBuffer = Nd4j.createBuffer(ArrayUtil.prodLong(shape))
			Return create(buffer, shape, stride, offset)
		End Function


		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="rows">    the rows of the ndarray </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <param name="stride">  the stride for the ndarray </param>
		''' <returns> the instance </returns>
		Public Overridable Function create(ByVal rows As Long, ByVal columns As Long, ByVal stride() As Integer) As INDArray Implements NDArrayFactory.create
			Return create(New Long() {rows, columns}, ArrayUtil.toLongArray(stride))
		End Function

		Public Overridable Function create(ByVal shape() As Long, ByVal stride() As Long) As INDArray Implements NDArrayFactory.create
			Return create(shape, stride, 0, Nd4j.order())
		End Function

		Public Overridable Function create(ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char) As INDArray Implements NDArrayFactory.create
			Shape.assertValidOrder(ordering)
			Return create(Nd4j.createBuffer(ArrayUtil.prodLong(shape)), shape, stride, offset, ordering)
		End Function


		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape">  the shape of the ndarray </param>
		''' <param name="stride"> the stride for the ndarray </param>
		''' <returns> the instance </returns>
		Public Overridable Function create(ByVal shape() As Integer, ByVal stride() As Integer) As INDArray Implements NDArrayFactory.create
			Return create(shape, stride, 0)
		End Function


		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="rows">    the rows of the ndarray </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <returns> the instance </returns>
		Public Overridable Function create(ByVal rows As Long, ByVal columns As Long) As INDArray Implements NDArrayFactory.create
			Return create(New Long() {rows, columns})
		End Function

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> the instance </returns>
		Public Overridable Function create(ByVal shape() As Long) As INDArray Implements NDArrayFactory.create
			'ensure shapes that wind up being scalar end up with the write shape

			Return create(shape, Nd4j.getStrides(shape), 0L)
		End Function

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> the instance </returns>
		Public Overridable Function create(ByVal shape() As Integer) As INDArray Implements NDArrayFactory.create
			Return create(shape, Nd4j.getStrides(shape), 0)
		End Function

		''' <summary>
		''' Create a scalar nd array with the specified value and offset
		''' </summary>
		''' <param name="value">  the value of the scalar </param>
		''' <param name="offset"> the offset of the ndarray </param>
		''' <returns> the scalar nd array </returns>
		Public Overridable Function scalar(ByVal value As Single, ByVal offset As Long) As INDArray Implements NDArrayFactory.scalar
			Return create(New Single() {value}, New Integer(){}, New Integer(){}, offset)
		End Function

		''' <summary>
		''' Create a scalar nd array with the specified value and offset
		''' </summary>
		''' <param name="value">  the value of the scalar </param>
		''' <param name="offset"> the offset of the ndarray </param>
		''' <returns> the scalar nd array </returns>
		Public Overridable Function scalar(ByVal value As Double, ByVal offset As Long) As INDArray Implements NDArrayFactory.scalar
			Return create(New Double() {value}, New Integer(){}, New Integer(){}, offset)
		End Function

		''' <summary>
		''' Create a scalar nd array with the specified value and offset
		''' </summary>
		''' <param name="value">  the value of the scalar </param>
		''' <param name="offset"> the offset of the ndarray </param>
		''' <returns> the scalar nd array </returns>
		Public Overridable Function scalar(ByVal value As Integer, ByVal offset As Long) As INDArray Implements NDArrayFactory.scalar
			Return create(New Integer() {value}, New Long(){}, New Long(){}, DataType.INT, Nd4j.MemoryManager.CurrentWorkspace)
		End Function

		''' <summary>
		''' Create a scalar ndarray with the specified offset
		''' </summary>
		''' <param name="value"> the value to initialize the scalar with </param>
		''' <returns> the created ndarray </returns>
		Public Overridable Function scalar(ByVal value As Number) As INDArray Implements NDArrayFactory.scalar
			Dim ws As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace

			If TypeOf value Is Double? OrElse TypeOf value Is AtomicDouble Then ' note that org.nd4j.linalg.primitives.AtomicDouble extends org.nd4j.shade.guava.util.concurrent.AtomicDouble
				Return scalar(value.doubleValue())
			ElseIf TypeOf value Is Single? Then
				Return scalar(value.floatValue())
			ElseIf TypeOf value Is Long? OrElse TypeOf value Is AtomicLong Then
				Return create(New Long() {value.longValue()}, New Long() {}, New Long() {}, DataType.LONG, ws)
			ElseIf TypeOf value Is Integer? OrElse TypeOf value Is AtomicInteger Then
				Return create(New Integer() {value.intValue()}, New Long() {}, New Long() {}, DataType.INT, ws)
			ElseIf TypeOf value Is Short? Then
				Return create(New Short() {value.shortValue()}, New Long() {}, New Long() {}, DataType.SHORT, ws)
			ElseIf TypeOf value Is SByte? Then
				Return create(New SByte() {value}, New Long() {}, New Long() {}, DataType.BYTE, ws)
			Else
				Throw New System.NotSupportedException("Unsupported data type: [" & value.GetType().Name & "]")
			End If
		End Function

		''' <summary>
		''' Create a scalar nd array with the specified value and offset
		''' </summary>
		''' <param name="value"> the value of the scalar </param>
		''' <returns> the scalar nd array </returns>
		Public Overridable Function scalar(ByVal value As Double) As INDArray Implements NDArrayFactory.scalar
			Return create(New Double() {value}, New Long(){}, New Long(){}, DataType.DOUBLE, Nd4j.MemoryManager.CurrentWorkspace)
		End Function

		Public Overridable Function scalar(ByVal value As Single) As INDArray Implements NDArrayFactory.scalar
			Return create(New Single() {value}, New Long(){}, New Long(){}, DataType.FLOAT, Nd4j.MemoryManager.CurrentWorkspace)
		End Function

		Public Overridable Function create(ByVal data() As Single, ByVal shape() As Integer, ByVal offset As Long) As INDArray Implements NDArrayFactory.create
			Return create(Nd4j.createBuffer(data), shape, offset)
		End Function

		Public MustOverride Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray Implements NDArrayFactory.create

		Public Overridable Function create(ByVal data() As Single, ByVal order As Char) As INDArray Implements NDArrayFactory.create
			Dim shape As val = New Long() {data.Length}
			Dim stride As val = Nd4j.getStrides(shape, order)
			Return create(data, shape, stride, order, DataType.FLOAT)
		End Function

		Public Overridable Function create(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer, ByVal order As Char, ByVal offset As Long) As INDArray Implements NDArrayFactory.create
			Return create(Nd4j.createBuffer(data), shape, stride, order, offset)
		End Function


		Public Overridable Function create(ByVal data() As Double, ByVal order As Char) As INDArray Implements NDArrayFactory.create
			Shape.assertValidOrder(order)
			Return create(data, New Long() {data.Length}, New Long(){1}, DataType.DOUBLE, Nd4j.MemoryManager.CurrentWorkspace)
		End Function

		Public Overridable Function create(ByVal data() As Double, ByVal shape() As Integer, ByVal stride() As Integer, ByVal order As Char, ByVal offset As Long) As INDArray Implements NDArrayFactory.create
			Return create(Nd4j.createBuffer(data), shape, stride, order, offset)

		End Function

		Public Overridable Function create(ByVal buffer As DataBuffer, ByVal shape() As Integer, ByVal stride() As Integer, ByVal order As Char, ByVal offset As Long) As INDArray Implements NDArrayFactory.create
			Shape.assertValidOrder(order)
			Return create(buffer, shape, stride, offset, order)
		End Function

		Public Overridable Function create(ByVal data() As Integer, ByVal shape() As Integer, ByVal stride() As Integer, ByVal order As Char, ByVal offset As Long) As INDArray Implements NDArrayFactory.create
			Shape.assertValidOrder(order)
			Return create(Nd4j.createBuffer(data), shape, stride, order, offset)
		End Function
	End Class

End Namespace