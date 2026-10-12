Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualBasic
Imports Ints = org.nd4j.shade.guava.primitives.Ints
Imports Longs = org.nd4j.shade.guava.primitives.Longs
Imports FlatBufferBuilder = com.google.flatbuffers.FlatBufferBuilder
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports FastMath = org.apache.commons.math3.util.FastMath
Imports FlatBuffersMapper = org.nd4j.autodiff.samediff.serde.FlatBuffersMapper
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports ByteOrder = org.nd4j.graph.ByteOrder
Imports FlatArray = org.nd4j.graph.FlatArray
Imports BlasBufferUtil = org.nd4j.linalg.api.blas.BlasBufferUtil
Imports MMulTranspose = org.nd4j.linalg.api.blas.params.MMulTranspose
Imports org.nd4j.linalg.api.buffer
Imports FirstAxisIterator = org.nd4j.linalg.api.iter.FirstAxisIterator
Imports NdIndexIterator = org.nd4j.linalg.api.iter.NdIndexIterator
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports HashCode = org.nd4j.linalg.api.ops.impl.reduce.HashCode
Imports All = org.nd4j.linalg.api.ops.impl.reduce.bool.All
Imports Any = org.nd4j.linalg.api.ops.impl.reduce.bool.Any
Imports org.nd4j.linalg.api.ops.impl.reduce.floating
Imports org.nd4j.linalg.api.ops.impl.reduce.same
Imports EqualsWithEps = org.nd4j.linalg.api.ops.impl.reduce3.EqualsWithEps
Imports EuclideanDistance = org.nd4j.linalg.api.ops.impl.reduce3.EuclideanDistance
Imports ManhattanDistance = org.nd4j.linalg.api.ops.impl.reduce3.ManhattanDistance
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports org.nd4j.linalg.api.ops.impl.broadcast
Imports Where = org.nd4j.linalg.api.ops.impl.controlflow.Where
Imports org.nd4j.linalg.api.ops.impl.scalar
Imports org.nd4j.linalg.api.ops.impl.scalar.comparison
Imports Tile = org.nd4j.linalg.api.ops.impl.shape.Tile
Imports StandardDeviation = org.nd4j.linalg.api.ops.impl.summarystats.StandardDeviation
Imports Variance = org.nd4j.linalg.api.ops.impl.summarystats.Variance
Imports Assign = org.nd4j.linalg.api.ops.impl.transforms.custom.Assign
Imports MatchConditionTransform = org.nd4j.linalg.api.ops.impl.transforms.bool.MatchConditionTransform
Imports EqualTo = org.nd4j.linalg.api.ops.impl.transforms.custom.EqualTo
Imports GreaterThan = org.nd4j.linalg.api.ops.impl.transforms.custom.GreaterThan
Imports LessThan = org.nd4j.linalg.api.ops.impl.transforms.custom.LessThan
Imports NotEqualTo = org.nd4j.linalg.api.ops.impl.transforms.custom.NotEqualTo
Imports Negative = org.nd4j.linalg.api.ops.impl.transforms.same.Negative
Imports org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic
Imports org.nd4j.linalg.api.ops.impl.transforms.comparison
Imports PerformanceTracker = org.nd4j.linalg.api.ops.performance.PerformanceTracker
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ArrayOptionsHelper = org.nd4j.linalg.api.shape.options.ArrayOptionsHelper
Imports org.nd4j.linalg.exception
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.linalg.indexing
Imports Condition = org.nd4j.linalg.indexing.conditions.Condition
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports MemcpyDirection = org.nd4j.linalg.api.memory.MemcpyDirection
Imports org.nd4j.common.primitives
Imports NDArrayStrings = org.nd4j.linalg.string.NDArrayStrings
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports LinAlgExceptions = org.nd4j.linalg.util.LinAlgExceptions
Imports NDArrayMath = org.nd4j.linalg.util.NDArrayMath
Imports WorkspaceUtils = org.nd4j.linalg.workspace.WorkspaceUtils
Imports org.nd4j.linalg.factory.Nd4j

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




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseNDArray implements INDArray, Iterable
	<Serializable>
	Public MustInherit Class BaseNDArray
		Implements INDArray, System.Collections.IEnumerable

		Public MustOverride Function toString(ByVal options As NDArrayStrings) As String
		Public MustOverride Function unsafeDuplication(ByVal blocking As Boolean) As INDArray
		Public MustOverride Function unsafeDuplication() As INDArray Implements INDArray.unsafeDuplication
		Public MustOverride Function shapeDescriptor() As LongShapeDescriptor Implements INDArray.shapeDescriptor
		Public MustOverride Function getString(ByVal index As Long) As String

		Private Const serialVersionUID As Long = 3285982317165542614L

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: protected transient volatile DataBuffer shapeInformation;
'JAVA TO VB CONVERTER NOTE: The field shapeInformation was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Protected Friend shapeInformation_Conflict As DataBuffer
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: protected transient volatile DataBuffer data;
'JAVA TO VB CONVERTER NOTE: The field data was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Protected Friend data_Conflict As DataBuffer
		'protected transient DataBuffer shape;
		'protected transient DataBuffer stride;
'JAVA TO VB CONVERTER NOTE: The field compressed was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Protected Friend compressed_Conflict As Boolean = False

		<NonSerialized>
		Protected Friend released As Boolean = False

		' this field holds jvm copy of shapeInfo
		<NonSerialized>
		Protected Friend jvmShapeInfo As JvmShapeInfo


		Private Shared ReadOnly arrayCounter As New AtomicLong(0)
		<NonSerialized>
		Protected Friend ReadOnly arrayId As Long = arrayCounter.getAndIncrement()


		'Precalculate these arrays (like [3,2,1,0], [2,1,0], [1,0], [0] etc) for use in TAD, to avoid creating same int[]s over and over
		Private Shared ReadOnly tadFinalPermuteDimensions()() As Integer
		Shared Sub New()
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: tadFinalPermuteDimensions = new Integer[32][0]
			tadFinalPermuteDimensions = RectangularArrays.RectangularIntegerArray(32, 0)
			tadFinalPermuteDimensions(1) = New Integer() {1, 0} 'Edge case for 1d tensors: selectively apply to column vectors
			For i As Integer = 2 To 31
				tadFinalPermuteDimensions(i) = New Integer(i - 1){}
				Dim k As Integer = i - 1
				Dim j As Integer = 0
				Do While k >= 0
					tadFinalPermuteDimensions(i)(j) = k
					k -= 1
					j += 1
				Loop
			Next i
			Dim t As val =1
		End Sub

		Public Sub New()
		End Sub

		Public Overridable ReadOnly Property Compressed As Boolean Implements INDArray.isCompressed
			Get
				Return compressed_Conflict
			End Get
		End Property

		Public Overridable Sub markAsCompressed(ByVal reallyCompressed As Boolean) Implements INDArray.markAsCompressed
			Me.compressed_Conflict = reallyCompressed
		End Sub

		''' 
		''' <param name="buffer"> </param>
		Public Sub New(ByVal buffer As DataBuffer)
			Me.data_Conflict = buffer
			If buffer.length() >= Integer.MaxValue Then
				Throw New System.ArgumentException("Length of buffer can not be >= Integer.MAX_VALUE")
			End If
			Dim shape() As Long = {1, CInt(buffer.length())}
			Dim stride() As Long = getStrides(shape)
			ShapeInformation = Nd4j.ShapeInfoProvider.createShapeInformation(shape, stride, 1, order(), buffer.dataType(), False)
			init(shape, stride)
		End Sub

		''' 
		''' <param name="buffer"> </param>
		''' <param name="shape"> </param>
		''' <param name="stride"> </param>
		''' <param name="offset"> </param>
		''' <param name="ordering"> </param>
		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char)
			Shape.assertValidOrder(ordering)
			Me.data_Conflict = If(offset > 0, createBuffer(buffer, offset, Shape.lengthOfBuffer(shape, stride)), buffer)
			ShapeInformation = Nd4j.ShapeInfoProvider.createShapeInformation(ArrayUtil.toLongArray(shape), ArrayUtil.toLongArray(stride), Shape.elementWiseStride(shape, stride, ordering = "f"c), ordering, buffer.dataType(), False)
			init(shape, stride)
			' Shape.setElementWiseStride(this.shapeInfo(),Shape.elementWiseStride(shape, stride, ordering == 'f'));

		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char)
			Me.New(buffer, shape, stride, offset, Shape.elementWiseStride(shape, stride, ordering = "f"c), ordering)
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ews As Long, ByVal ordering As Char)
			Shape.assertValidOrder(ordering)
			Me.data_Conflict = If(offset > 0, createBuffer(buffer, offset, Shape.lengthOfBuffer(shape, stride)), buffer)
			ShapeInformation = Nd4j.ShapeInfoProvider.createShapeInformation(shape, stride, ews, ordering, buffer.dataType(), False)
			init(shape, stride)
			' Shape.setElementWiseStride(this.shapeInfo(),Shape.elementWiseStride(shape, stride, ordering == 'f'));
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char, ByVal dataType As DataType)
			Me.New(buffer, shape, stride, offset, Shape.elementWiseStride(shape, stride, ordering = "f"c), ordering, dataType)
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ews As Long, ByVal ordering As Char, ByVal dataType As DataType)
			Me.data_Conflict = If(offset > 0, createBuffer(buffer, offset, Shape.lengthOfBuffer(shape, stride)), buffer)
			ShapeInformation = Nd4j.ShapeInfoProvider.createShapeInformation(shape, stride, ews, ordering, dataType, False)
			init(shape, stride)
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal ordering As Char, ByVal type As DataType)
			Me.data_Conflict = buffer
			ShapeInformation = Nd4j.ShapeInfoProvider.createShapeInformation(shape, stride, Shape.elementWiseStride(shape, stride, ordering = "f"c), ordering, type, False)
			init(shape, stride)
			' Shape.setElementWiseStride(this.shapeInfo(),Shape.elementWiseStride(shape, stride, ordering == 'f'));
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal ordering As Char, ByVal type As DataType, ByVal workspace As MemoryWorkspace)
			Me.data_Conflict = buffer
			ShapeInformation = Nd4j.ShapeInfoProvider.createShapeInformation(shape, stride, Shape.elementWiseStride(shape, stride, ordering = "f"c), ordering, type, False)
			init(shape, stride)
			' Shape.setElementWiseStride(this.shapeInfo(),Shape.elementWiseStride(shape, stride, ordering == 'f'));
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal dataType As DataType, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char)
			Me.data_Conflict = If(offset > 0, createBuffer(buffer, offset, Shape.lengthOfBuffer(shape, stride)), buffer)
			ShapeInformation = Nd4j.ShapeInfoProvider.createShapeInformation(shape, stride, Shape.elementWiseStride(shape, stride, ordering = "f"c), ordering, dataType, False)
			init(shape, stride)
			' Shape.setElementWiseStride(this.shapeInfo(),Shape.elementWiseStride(shape, stride, ordering == 'f'));
		End Sub

		''' <summary>
		''' Initialize the ndarray as a matrix
		''' with the given data (indices preserved) </summary>
		''' <param name="data"> </param>
		Public Sub New(ByVal data()() As Double)
			Me.New(data, order())
		End Sub

		''' 
		''' <param name="data"> </param>
		''' <param name="ordering"> </param>
		Public Sub New(ByVal data()() As Double, ByVal ordering As Char)
			Me.New(internalCreateBuffer(If(ordering = "c"c, ArrayUtil.flatten(data), ArrayUtil.flattenF(data))), New Integer() {data.Length, data(0).Length}, getStrides(New Integer() {data.Length, data(0).Length}, ordering), 0, ordering)

			Dim c As Integer = columns()
			Dim r As Integer = 0
			Do While r < rows()
				Preconditions.checkState(data(r).Length = c, "data[%s].length=%s must be equal to number of columns %s", r, data(r).Length, c)
				r += 1
			Loop
		End Sub


		''' <summary>
		''' Create with the specified shape and buffer
		''' </summary>
		''' <param name="shape">  the shape </param>
		''' <param name="buffer"> the buffer </param>
		Public Sub New(ByVal shape() As Integer, ByVal buffer As DataBuffer)
			Me.data_Conflict = buffer
			init(shape, getStrides(shape))
		End Sub

		''' <summary>
		''' Create this ndarray with the given data and shape and 0 offset
		''' </summary>
		''' <param name="data">  the data to use </param>
		''' <param name="shape"> the shape of the ndarray </param>
		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal ordering As Char)
			Me.New(data, shape, 0, ordering)
		End Sub

		''' <param name="data">     the data to use </param>
		''' <param name="shape">    the shape of the ndarray </param>
		''' <param name="offset">   the desired offset </param>
		''' <param name="ordering"> the ordering of the ndarray </param>
		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal offset As Long, ByVal ordering As Char)
			Me.New(data, shape, getStrides(shape, ordering), offset)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal shape() As Long, ByVal offset As Long, ByVal ordering As Char)
			Me.New(data, shape, getStrides(shape, ordering), offset)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal shape() As Long, ByVal offset As Long, ByVal ordering As Char)
			Me.New(data, shape, getStrides(shape, ordering), offset)
		End Sub


		''' <summary>
		''' Construct an ndarray of the specified shape
		''' with an empty data array
		''' </summary>
		''' <param name="shape">    the shape of the ndarray </param>
		''' <param name="stride">   the stride of the ndarray </param>
		''' <param name="offset">   the desired offset </param>
		''' <param name="ordering"> the ordering of the ndarray </param>
		Public Sub New(ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char)
			Me.New(createBuffer(If(shape.Length = 0, 1, ArrayUtil.prodLong(shape))), shape, stride, offset, ordering)
		End Sub

		Public Sub New(ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char)
			Me.New(createBuffer(If(shape.Length = 0, 1, ArrayUtil.prodLong(shape))), shape, stride, offset, ordering)
		End Sub

		''' <summary>
		''' Construct an ndarray of the specified shape.
		''' </summary>
		''' <param name="shape">    the shape of the ndarray </param>
		''' <param name="stride">   the stride of the ndarray </param>
		''' <param name="offset">   the desired offset </param>
		''' <param name="ordering"> the ordering of the ndarray </param>
		''' <param name="initialize"> Whether to initialize the INDArray. If true: initialize. If false: don't. </param>
		Public Sub New(ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char, ByVal initialize As Boolean)
			Me.New(createBuffer(If(shape.Length = 0, 1, ArrayUtil.prodLong(shape)), initialize), shape, stride, offset, ordering)
		End Sub

		Public Sub New(ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char, ByVal initialize As Boolean)
			Me.New(createBuffer(If(shape.Length = 0, 1, ArrayUtil.prodLong(shape)), initialize), shape, stride, offset, ordering)
		End Sub

		Public Sub New(ByVal type As DataType, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char, ByVal initialize As Boolean)
			Me.New(createBuffer(type,If(shape.Length = 0, 1, ArrayUtil.prodLong(shape)), initialize), type, shape, stride, offset, ordering)
		End Sub

		Public Sub New(ByVal type As DataType, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace)
			Me.New(createBuffer(type,If(shape.Length = 0, 1, ArrayUtil.prodLong(shape)), initialize, workspace), type, shape, stride, offset, ordering)
		End Sub

		Public Sub New(ByVal type As DataType, ByVal shape() As Long, ByVal paddings() As Long, ByVal paddingOffsets() As Long, ByVal ordering As Char, ByVal workspace As MemoryWorkspace)

			'calculate strides with paddings
			Dim rank As Integer = shape.Length
			If paddings Is Nothing OrElse paddings.Length <> rank Then
				Throw New System.ArgumentException("The length of Padding should be equal to the length of Shape")
			End If
			Dim paddedShape(rank - 1) As Long
			Dim empty As Boolean = False
			Dim zeroOffset As Boolean = paddingOffsets Is Nothing OrElse paddingOffsets.Length = 0
			Dim paddingOffsetsInvalid As Boolean = paddingOffsets IsNot Nothing AndAlso paddingOffsets.Length <> rank
			Dim ews As Long = 1
			If Not paddingOffsetsInvalid Then
				For i As Integer = 0 To rank - 1
					paddedShape(i) = shape(i) + paddings(i)
					If paddings(i) <> 0 Then
						ews = 0
					End If
					If shape(i) = 0 Then
						empty = True
					End If
					If paddingOffsets(i)> paddings(i) Then
						paddingOffsetsInvalid = True
						Exit For
					End If
				Next i
			End If
			If Not zeroOffset AndAlso paddingOffsetsInvalid Then
				Throw New System.ArgumentException("If PaddingOffsets is not empty or zero length then its length should match the length of Paddings and also its elements should not be greater")
			End If

			Dim paddedStride() As Long = If(ordering = "c"c, ArrayUtil.calcStrides(paddedShape,1), ArrayUtil.calcStridesFortran(paddedShape,1))
			Dim paddedAllocSize As Long = If(ordering = "c"c, paddedShape(0) * paddedStride(0), paddedShape(rank-1) * paddedStride(rank-1))

			Dim offset As Long = If(empty OrElse ews = 1 OrElse zeroOffset, 0, ArrayUtil.calcOffset(paddedShape, paddingOffsets, paddedStride))
			Dim buffer As DataBuffer = createBuffer(type, paddedAllocSize, False, workspace)
			Me.data_Conflict = If(offset > 0, createBuffer(buffer, offset, paddedAllocSize - offset), buffer)
			Dim extras As Long = ArrayOptionsHelper.setOptionBit(0, type)
			If empty Then
				extras = ArrayOptionsHelper.setOptionBit(extras, ArrayOptionsHelper.ATYPE_EMPTY_BIT)
			ElseIf ews<>1 Then
				extras = ArrayOptionsHelper.setOptionBit(extras, ArrayOptionsHelper.HAS_PADDED_BUFFER)
			End If
			ShapeInformation = Nd4j.ShapeInfoProvider.createShapeInformation(shape, paddedStride, ews, ordering, extras)
		End Sub

		''' <summary>
		''' Create the ndarray with
		''' the specified shape and stride and an offset of 0
		''' </summary>
		''' <param name="shape">    the shape of the ndarray </param>
		''' <param name="stride">   the stride of the ndarray </param>
		''' <param name="ordering"> the ordering of the ndarray </param>
		Public Sub New(ByVal shape() As Integer, ByVal stride() As Integer, ByVal ordering As Char)
			Me.New(shape, stride, 0, ordering)
		End Sub


		''' 
		''' <param name="shape"> </param>
		''' <param name="offset"> </param>
		''' <param name="ordering"> </param>
		Public Sub New(ByVal shape() As Integer, ByVal offset As Long, ByVal ordering As Char)
			Me.New(shape, getStrides(shape, ordering), offset, ordering)
		End Sub

		Public Sub New(ByVal shape() As Long, ByVal offset As Long, ByVal ordering As Char)
			Me.New(shape, getStrides(shape, ordering), offset, ordering)
		End Sub


		''' <summary>
		''' Create an ndarray
		''' with the given shape </summary>
		''' <param name="shape"> </param>
		Public Sub New(ByVal shape() As Integer)
			Me.New(shape, 0, order())
		End Sub

		Public Sub New(ByVal shape() As Long)
			Me.New(shape, 0, order())
		End Sub


		''' <summary>
		''' Creates a new <i>n</i> times <i>m</i> <tt>DoubleMatrix</tt>.
		''' </summary>
		''' <param name="newRows">    the number of rows (<i>n</i>) of the new matrix. </param>
		''' <param name="newColumns"> the number of columns (<i>m</i>) of the new matrix. </param>
		Public Sub New(ByVal newRows As Integer, ByVal newColumns As Integer, ByVal ordering As Char)
			Shape.assertValidOrder(ordering)
			Me.data_Conflict = createBuffer(CLng(newRows) * newColumns)
			Dim shape As val = New Long() {newRows, newColumns}
			Dim stride As val = getStrides(shape, ordering)
			ShapeInformation = Nd4j.ShapeInfoProvider.createShapeInformation(shape, stride, Shape.elementWiseStride(shape, stride, ordering = "f"c), ordering, dataType(), False)
			init(shape, stride)
		End Sub

		Public Sub New(ByVal newRows As Long, ByVal newColumns As Long, ByVal ordering As Char)
			Shape.assertValidOrder(ordering)
			Me.data_Conflict = createBuffer(CLng(newRows) * newColumns)
			Dim shape() As Long = {newRows, newColumns}
			Dim stride() As Long = getStrides(shape, ordering)
			ShapeInformation = Nd4j.ShapeInfoProvider.createShapeInformation(shape, stride, Shape.elementWiseStride(shape, stride, ordering = "f"c), ordering, dataType(), False)
			init(shape, stride)
		End Sub


		''' <summary>
		''' Create an ndarray from the specified slices.
		''' This will go through and merge all of the
		''' data from each slice in to one ndarray
		''' which will then take the specified shape
		''' </summary>
		''' <param name="slices"> the slices to merge </param>
		''' <param name="shape">  the shape of the ndarray </param>
		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Integer, ByVal ordering As Char)
			Me.New(slices, shape, getStrides(shape, ordering), ordering)
		End Sub

		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Long, ByVal ordering As Char)
			Me.New(slices, shape, getStrides(shape, ordering), ordering)
		End Sub


		''' <summary>
		''' Create an ndarray from the specified slices.
		''' This will go through and merge all of the
		''' data from each slice in to one ndarray
		''' which will then take the specified shape
		''' </summary>
		''' <param name="slices"> the slices to merge </param>
		''' <param name="shape">  the shape of the ndarray </param>
		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Integer, ByVal stride() As Integer, ByVal ordering As Char)
			Shape.assertValidOrder(ordering)
			Dim ret As DataBuffer = If(slices(0).data().dataType() = (DataType.FLOAT), createBuffer(New Single(ArrayUtil.prod(shape) - 1){}), createBuffer(New Double(ArrayUtil.prod(shape) - 1){}))
			Me.data_Conflict = ret
			ShapeInformation = Nd4j.ShapeInfoProvider.createShapeInformation(ArrayUtil.toLongArray(shape), ArrayUtil.toLongArray(stride), Shape.elementWiseStride(shape, stride, ordering = "f"c), ordering, slices(0).dataType(), False)
			init(shape, stride)
			'    Shape.setElementWiseStride(this.shapeInfo(),Shape.elementWiseStride(shape, stride, ordering == 'f'));

			If slices(0).isScalar() Then
				Dim i As Integer = 0
				Do While i < length()
					putScalar(i, slices(i).getDouble(0))
					i += 1
				Loop
			Else
				Dim i As Integer = 0
				Do While i < Me.slices()
					putSlice(i, slices(i))
					i += 1
				Loop
			End If
		End Sub


		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Long, ByVal stride() As Long, ByVal ordering As Char)
			Dim ret As DataBuffer = createBuffer(slices(0).dataType(), Shape.lengthOf(shape), False) 'slices.get(0).data().dataType() == (DataType.FLOAT)
	'                ? Nd4j.createBuffer(new float[ArrayUtil.prod(shape)])
	'                : Nd4j.createBuffer(new double[ArrayUtil.prod(shape)]);
	'                
			Me.data_Conflict = ret
			ShapeInformation = Nd4j.ShapeInfoProvider.createShapeInformation(shape, stride, Shape.elementWiseStride(shape, stride, ordering = "f"c), ordering, slices(0).dataType(), False)
			init(shape, stride)
			'    Shape.setElementWiseStride(this.shapeInfo(),Shape.elementWiseStride(shape, stride, ordering == 'f'));

			If slices(0).isScalar() Then
				Dim i As Integer = 0
				Do While i < length()
					putScalar(i, slices(i).getDouble(0))
					i += 1
				Loop
			Else
				Dim i As Integer = 0
				Do While i < Me.slices()
					putSlice(i, slices(i))
					i += 1
				Loop
			End If
		End Sub

		''' 
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		''' <param name="stride"> </param>
		''' <param name="ordering"> </param>
		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer, ByVal ordering As Char)
			Me.New(data, shape, stride, 0, ordering)
		End Sub

		''' 
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		''' <param name="stride"> </param>
		''' <param name="offset"> </param>
		''' <param name="ordering"> </param>
		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char)
			Shape.assertValidOrder(ordering)
			ShapeInformation = Nd4j.ShapeInfoProvider.createShapeInformation(ArrayUtil.toLongArray(shape), ArrayUtil.toLongArray(stride), Shape.elementWiseStride(shape, stride, ordering = "f"c), ordering, DataType.FLOAT,If(data IsNot Nothing AndAlso data.Length > 0, False, True))
			If data IsNot Nothing AndAlso data.Length > 0 Then

				Dim perfD As val = PerformanceTracker.Instance.helperStartTransaction()

				Me.data_Conflict = internalCreateBuffer(data, offset)

				PerformanceTracker.Instance.helperRegisterTransaction(0, perfD, data.Length * sizeOfDataType(DataType.FLOAT), MemcpyDirection.HOST_TO_HOST)

				If offset >= data.Length Then
					Throw New System.ArgumentException("invalid offset: must be < data.length")
				End If
			End If

			init(shape, stride)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char)
			Shape.assertValidOrder(ordering)
			ShapeInformation = Nd4j.ShapeInfoProvider.createShapeInformation(shape, stride, Shape.elementWiseStride(shape, stride, ordering = "f"c), ordering, DataType.FLOAT,If(data IsNot Nothing AndAlso data.Length > 0, False, True))
			If data IsNot Nothing AndAlso data.Length > 0 Then
				Me.data_Conflict = createTypedBuffer(data, DataType.FLOAT)
				If offset >= data.Length Then
					Throw New System.ArgumentException("invalid offset: must be < data.length")
				End If
			End If

			init(shape, stride)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char)
			Shape.assertValidOrder(ordering)
			ShapeInformation = Nd4j.ShapeInfoProvider.createShapeInformation(shape, stride, Shape.elementWiseStride(shape, stride, ordering = "f"c), ordering, DataType.DOUBLE,If(data IsNot Nothing AndAlso data.Length > 0, False, True))
			If data IsNot Nothing AndAlso data.Length > 0 Then
				Me.data_Conflict = createBuffer(data, offset)
				If offset >= data.Length Then
					Throw New System.ArgumentException("invalid offset: must be < data.length")
				End If
			End If

			init(shape, stride)
		End Sub

		''' 
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		''' <param name="stride"> </param>
		''' <param name="offset"> </param>
		Public Sub New(ByVal data As DataBuffer, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long)
			Me.data_Conflict = createBuffer(data, offset, ArrayUtil.prodLong(shape))
			ShapeInformation = Nd4j.ShapeInfoProvider.createShapeInformation(ArrayUtil.toLongArray(shape), ArrayUtil.toLongArray(stride), Shape.elementWiseStride(shape, stride, order() = "f"c), order(), data.dataType(), False)
			init(shape, stride)
			'  Shape.setElementWiseStride(this.shapeInfo(),Shape.elementWiseStride(shape, stride, Nd4j.order() == 'f'));


		End Sub

		''' 
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		''' <param name="strides"> </param>
		Public Sub New(ByVal data() As Integer, ByVal shape() As Integer, ByVal strides() As Integer)
			Me.New(internalCreateBuffer(data), shape, strides)
		End Sub

		''' 
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		Public Sub New(ByVal data As DataBuffer, ByVal shape() As Integer)
			Me.New(data, shape, getStrides(shape, order()), 0, order())
		End Sub

		Public Sub New(ByVal data As DataBuffer, ByVal shape() As Long)
			Me.New(data, shape, getStrides(shape, order()), 0, order())
		End Sub


		''' 
		''' <param name="buffer"> </param>
		''' <param name="shape"> </param>
		''' <param name="offset"> </param>
		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Integer, ByVal offset As Long)
			Me.New(createBuffer(buffer, offset, ArrayUtil.prodLong(shape)), shape, getStrides(shape), offset, order())
		End Sub

		''' 
		''' <param name="buffer"> </param>
		''' <param name="shape"> </param>
		''' <param name="ordering"> </param>
		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Integer, ByVal ordering As Char)
			Me.New(buffer, shape, getStrides(shape, ordering), 0, ordering)
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Long, ByVal ordering As Char)
			Me.New(buffer, shape, getStrides(shape, ordering), 0, ordering)
		End Sub

		''' 
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		''' <param name="ordering"> </param>
		Public Sub New(ByVal data() As Double, ByVal shape() As Integer, ByVal ordering As Char)
			Me.New(createBuffer(data), shape, ordering)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal shape() As Long, ByVal ordering As Char)
			Me.New(createBuffer(data), shape, ordering)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal shape() As Long, ByVal ordering As Char)
			Me.New(createBuffer(data), shape, ordering)
		End Sub

		''' 
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		''' <param name="stride"> </param>
		''' <param name="offset"> </param>
		''' <param name="ordering"> </param>
		Public Sub New(ByVal data() As Double, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char)
			Me.New(internalCreateBuffer(data, offset), shape, stride, offset, ordering)
		End Sub

		''' 
		''' <param name="data"> </param>
		''' <param name="order"> </param>
		Public Sub New(ByVal data() As Single, ByVal order As Char)
			Me.New(internalCreateBuffer(data), order)
		End Sub

		Protected Friend Shared Function internalCreateBuffer(ByVal data() As Single) As DataBuffer
			Dim perfX As val = PerformanceTracker.Instance.helperStartTransaction()

			Dim buffer As val = createBuffer(data)
			PerformanceTracker.Instance.helperRegisterTransaction(0, perfX, data.Length * sizeOfDataType(buffer.dataType()), MemcpyDirection.HOST_TO_HOST)

			Return buffer
		End Function

		Protected Friend Shared Function internalCreateBuffer(ByVal data() As Double) As DataBuffer
			Dim perfX As val = PerformanceTracker.Instance.helperStartTransaction()

			Dim buffer As val = createBuffer(data)
			PerformanceTracker.Instance.helperRegisterTransaction(0, perfX, data.Length * sizeOfDataType(buffer.dataType()), MemcpyDirection.HOST_TO_HOST)

			Return buffer
		End Function

		Protected Friend Shared Function internalCreateBuffer(ByVal data() As Integer) As DataBuffer
			Dim perfX As val = PerformanceTracker.Instance.helperStartTransaction()

			Dim buffer As val = createBuffer(data)
			PerformanceTracker.Instance.helperRegisterTransaction(0, perfX, data.Length * sizeOfDataType(buffer.dataType()), MemcpyDirection.HOST_TO_HOST)

			Return buffer
		End Function

		Protected Friend Shared Function internalCreateBuffer(ByVal data() As Single, ByVal offset As Long) As DataBuffer
			Dim perfX As val = PerformanceTracker.Instance.helperStartTransaction()

			Dim buffer As val = createBuffer(data, offset)
			PerformanceTracker.Instance.helperRegisterTransaction(0, perfX, data.Length * sizeOfDataType(buffer.dataType()), MemcpyDirection.HOST_TO_HOST)

			Return buffer
		End Function

		Protected Friend Shared Function internalCreateBuffer(ByVal data() As Double, ByVal offset As Long) As DataBuffer
			Dim perfX As val = PerformanceTracker.Instance.helperStartTransaction()

			Dim buffer As val = createBuffer(data, offset)
			PerformanceTracker.Instance.helperRegisterTransaction(0, perfX, data.Length * sizeOfDataType(buffer.dataType()), MemcpyDirection.HOST_TO_HOST)

			Return buffer
		End Function

		''' 
		''' <param name="floatBuffer"> </param>
		''' <param name="order"> </param>
		Public Sub New(ByVal floatBuffer As DataBuffer, ByVal order As Char)
			Me.New(floatBuffer, New Integer() {CInt(floatBuffer.length())}, getStrides(New Integer() {CInt(floatBuffer.length())}, order), 0, order)
			Shape.assertValidOrder(order)
			If floatBuffer.length() >= Integer.MaxValue Then
				Throw New System.ArgumentException("Length of buffer can not be >= Integer.MAX_VALUE")
			End If
		End Sub

		''' 
		''' <param name="buffer"> </param>
		''' <param name="shape"> </param>
		''' <param name="strides"> </param>
		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Integer, ByVal strides() As Integer)
			Me.New(buffer, shape, strides, 0, order())
		End Sub


		''' <summary>
		''' Create this ndarray with the given data and shape and 0 offset
		''' </summary>
		''' <param name="data">  the data to use </param>
		''' <param name="shape"> the shape of the ndarray </param>
		Public Sub New(ByVal data() As Single, ByVal shape() As Integer)
			Me.New(data, shape, 0)
		End Sub


		''' 
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		''' <param name="offset"> </param>
		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal offset As Long)
			Me.New(data, shape, offset, order())

		End Sub

		''' <summary>
		''' Construct an ndarray of the specified shape
		''' with an empty data array
		''' </summary>
		''' <param name="shape">  the shape of the ndarray </param>
		''' <param name="stride"> the stride of the ndarray </param>
		''' <param name="offset"> the desired offset </param>
		Public Sub New(ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long)
			Me.New(New Single(ArrayUtil.prod(shape) - 1){}, shape, stride, offset, order())
		End Sub

		Public Sub New(ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long)
			Me.New(New Single(ArrayUtil.prod(shape) - 1){}, shape, stride, offset, order())
		End Sub

		''' <summary>
		''' Create the ndarray with
		''' the specified shape and stride and an offset of 0
		''' </summary>
		''' <param name="shape">  the shape of the ndarray </param>
		''' <param name="stride"> the stride of the ndarray </param>
		Public Sub New(ByVal shape() As Integer, ByVal stride() As Integer)
			Me.New(shape, stride, 0)
		End Sub

		''' 
		''' <param name="shape"> </param>
		''' <param name="offset"> </param>
		Public Sub New(ByVal shape() As Integer, ByVal offset As Long)
			Me.New(shape, getStrides(shape), offset)
		End Sub

		''' 
		''' <param name="shape"> </param>
		''' <param name="ordering"> </param>
		Public Sub New(ByVal shape() As Integer, ByVal ordering As Char)
			Me.New(shape, 0, ordering)
		End Sub


		''' <summary>
		''' Creates a new <i>n</i> times <i>m</i> <tt>DoubleMatrix</tt>.
		''' </summary>
		''' <param name="newRows">    the number of rows (<i>n</i>) of the new matrix. </param>
		''' <param name="newColumns"> the number of columns (<i>m</i>) of the new matrix. </param>
		Public Sub New(ByVal newRows As Integer, ByVal newColumns As Integer)
			Me.New(newRows, newColumns, order())
		End Sub

		Public Sub New(ByVal newRows As Long, ByVal newColumns As Long)
			Me.New(newRows, newColumns, order())
		End Sub


		''' <summary>
		''' Create an ndarray from the specified slices.
		''' This will go through and merge all of the
		''' data from each slice in to one ndarray
		''' which will then take the specified shape
		''' </summary>
		''' <param name="slices"> the slices to merge </param>
		''' <param name="shape">  the shape of the ndarray </param>
		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Integer)
			Me.New(slices, shape, order())
		End Sub

		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Long)
			Me.New(slices, shape, order())
		End Sub

		''' <summary>
		''' Create an ndarray from the specified slices.
		''' This will go through and merge all of the
		''' data from each slice in to one ndarray
		''' which will then take the specified shape
		''' </summary>
		''' <param name="slices"> the slices to merge </param>
		''' <param name="shape">  the shape of the ndarray </param>
		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Integer, ByVal stride() As Integer)
			Me.New(slices, shape, stride, order())
		End Sub

		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Long, ByVal stride() As Long)
			Me.New(slices, shape, stride, order())
		End Sub

		''' 
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		''' <param name="stride"> </param>
		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer)
			Me.New(data, shape, stride, order())
		End Sub


		''' 
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		''' <param name="stride"> </param>
		''' <param name="offset"> </param>
		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long)
			Me.New(data, shape, stride, offset, order())
		End Sub

		Public Sub New(ByVal data() As Double, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long)
			Me.New(data, shape, stride, offset, order())
		End Sub

		Public Sub New(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long)
			Me.New(data, shape, stride, offset, order())
		End Sub

		''' 
		''' <param name="data"> </param>
		Public Sub New(ByVal data() As Single)
			Me.New(createBuffer(data))
		End Sub


		''' <summary>
		''' Initialize the ndarray
		''' with the given data </summary>
		''' <param name="data"> </param>
		Public Sub New(ByVal data()() As Single)
			Me.New(data, order())
		End Sub

		''' 
		''' <param name="data"> </param>
		''' <param name="ordering"> </param>
		Public Sub New(ByVal data()() As Single, ByVal ordering As Char)
			Me.New(internalCreateBuffer(If(ordering = "c"c, ArrayUtil.flatten(data), ArrayUtil.flattenF(data))), New Integer() {data.Length, data(0).Length}, getStrides(New Integer() {data.Length, data(0).Length}, ordering), 0, ordering)

			Dim c As Integer = columns()
			Dim r As Integer = 0
			Do While r < rows()
				Preconditions.checkState(data(r).Length = c, "data[%s].length=%s must be equal to number of columns %s", r, data(r).Length, c)
				r += 1
			Loop
		End Sub



		''' <summary>
		''' Constructor for stride and offset
		''' </summary>
		''' <param name="buffer"> </param>
		''' <param name="shape"> </param>
		''' <param name="offset"> </param>
		''' <param name="ordering"> </param>
		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Integer, ByVal offset As Long, ByVal ordering As Char)
			Me.New(buffer, shape, getStrides(shape, ordering), offset, ordering)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long)
			Me.New(data, shape, stride, offset, order())
		End Sub


		''' <summary>
		''' Returns whether the ndarray is valid or not </summary>
		''' <returns> true if the ndarray is valid
		''' false otherwise </returns>
		<Obsolete>
		Public Overridable ReadOnly Property Valid As Boolean
			Get
				Try
					linearIndex(length() - 1)
				Catch e As Exception
					Return False
				End Try
				Return True
			End Get
		End Property

		Protected Friend Overridable Function create(ByVal data As DataBuffer, ByVal shape() As Integer, ByVal offset As Long) As INDArray
			Return create(data, shape, offset)
		End Function

		Public Overridable Function elementWiseStride() As Integer Implements INDArray.elementWiseStride
			Return Shape.elementWiseStride(shapeInfoDataBuffer())
		End Function

		Public Overridable Function tensorsAlongDimension(ParamArray ByVal dimension() As Integer) As Long Implements INDArray.tensorsAlongDimension
			If dimension Is Nothing OrElse dimension.Length = 0 Then
				Throw New System.ArgumentException("Invalid input: dimensions not specified (null or length 0)")
			End If
			If dimension.Length >= rank() OrElse dimension.Length = 1 AndAlso dimension(0) = Integer.MaxValue Then
				Return 1
			End If
			For i As Integer = 0 To dimension.Length - 1
				If dimension(i) < 0 Then
					dimension(i) += rank()
				End If
			Next i
			Dim tensorShape() As Long = ArrayUtil.keep(shape(), dimension)
			Dim len As Long = ArrayUtil.prodLong(tensorShape)
			If len = 0 Then
				Throw New System.InvalidOperationException("Illegal length found after removing index")
			End If
			Dim length As Long = Me.length()
			If length \ len >= Integer.MaxValue Then
				Throw New System.ArgumentException("Tensors along dimension can not be >= Integer.MAX_VALUE")
			End If
			Return length \ len
		End Function

		Public Overridable Function tensorAlongDimension(ByVal index As Long, ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.tensorAlongDimension
			If dimension Is Nothing OrElse dimension.Length = 0 Then
				Throw New System.ArgumentException("Invalid input: dimensions not specified (null or length 0)")
			End If

			Preconditions.checkArgument(Not Me.Empty, "tensorAlongDimension(...) can't be used on empty tensors")

			If dimension.Length >= rank() OrElse dimension.Length = 1 AndAlso dimension(0) = Integer.MaxValue Then
				Return Me
			End If
			For i As Integer = 0 To dimension.Length - 1
				If dimension(i) < 0 Then
					dimension(i) += rank()
				End If
			Next i

			'dedup
			If dimension.Length > 1 Then
				dimension = Ints.toArray(New List(Of )(New SortedSet(Of )(Ints.asList(dimension))))
			End If

			If dimension.Length > 1 Then
				Array.Sort(dimension)
			End If

			Dim tads As Long = tensorsAlongDimension(dimension)
			If index >= tads Then
				Throw New System.ArgumentException("Illegal index " & index & " out of tads " & tads)
			End If


			If dimension.Length = 1 Then
				If dimension(0) = 0 AndAlso ColumnVector Then
					Return Me.transpose()
				ElseIf dimension(0) = 1 AndAlso RowVector Then
					Return Me
				End If
			End If

			Dim tadInfo As Pair(Of DataBuffer, DataBuffer) = Nd4j.Executioner.TADManager.getTADOnlyShapeInfo(Me, dimension)
			Dim shapeInfo As DataBuffer = tadInfo.First
			Dim jShapeInfo As val = shapeInfo.asLong()
			Dim shape As val = Shape.shape(jShapeInfo)
			Dim stride As val = Shape.stride(jShapeInfo)
			Dim offset As Long = Me.offset() + tadInfo.Second.getLong(index)
			Dim ews As val = shapeInfo.getLong(jShapeInfo(0) * 2 + 2)
			Dim tadOrder As Char = ChrW(shapeInfo.getInt(jShapeInfo(0) * 2 + 3))
			Dim toTad As val = create(data(), shape, stride, offset, ews, tadOrder)
			Return toTad
		End Function

		Private WriteOnly Property ShapeInformation As Pair(Of DataBuffer, Long())
			Set(ByVal shapeInfo As Pair(Of DataBuffer, Long()))
				Me.shapeInformation_Conflict = shapeInfo.First
				Me.jvmShapeInfo = New JvmShapeInfo(shapeInfo.Second)
			End Set
		End Property


		Private Function doTad(ByVal index As Integer, ParamArray ByVal dimension() As Integer) As INDArray
			If dimension Is Nothing OrElse dimension.Length = 0 Then
				Throw New System.ArgumentException("Invalid input: dimensions not specified (null or length 0)")
			End If

			If dimension.Length >= rank() Then
				Return Me
			End If
			For i As Integer = 0 To dimension.Length - 1
				If dimension(i) < 0 Then
					dimension(i) += rank()
				End If
			Next i

			If dimension.Length > 1 Then
				Array.Sort(dimension)
			End If

			Dim tads As Long = tensorsAlongDimension(dimension)
			If index >= tads Then
				Throw New System.ArgumentException("Illegal index " & index & " out of tads " & tads)
			End If


			If dimension.Length = 1 Then
				If dimension(0) = 0 AndAlso ColumnVector Then
					Return Me.transpose()
				ElseIf dimension(0) = 1 AndAlso RowVector Then
					Return Me
				End If
			End If


			Dim tensorShape() As Long = ArrayUtil.keep(shape(), dimension)
			Dim reverseDimensions() As Integer = ArrayUtil.reverseCopy(dimension)
			Dim remove() As Integer = ArrayUtil.removeIndex(ArrayUtil.range(0, rank()), dimension)
			Dim newPermuteDims() As Integer = Ints.concat(remove, reverseDimensions)
			Dim finalPermuteDims() As Integer = tadFinalPermuteDimensions(dimension.Length)

			Dim permuted As INDArray = permute(newPermuteDims)
			Dim sliceIdx As Long = NDArrayMath.sliceOffsetForTensor(index, permuted, tensorShape)

			Dim ret2 As INDArray = permuted.slice(sliceIdx)
			If dimension.Length = tensorShape.Length AndAlso ArrayUtil.prodLong(tensorShape) = ret2.length() Then
				If dimension.Length = 1 AndAlso ret2.RowVector Then
					Return ret2
				End If
				If finalPermuteDims.Length <> ret2.rank() Then
					finalPermuteDims = New Integer(ret2.rank() - 1){}
					Dim count As Integer = 0
					For i As Integer = finalPermuteDims.Length - 1 To 0 Step -1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: finalPermuteDims[count++] = i;
						finalPermuteDims(count) = i
							count += 1
					Next i
				End If
				Return ret2.permutei(finalPermuteDims)
			End If


			Dim length As Integer = ArrayUtil.prod(tensorShape)
			Dim tensorLength As Integer = ArrayUtil.prod(tensorShape)
			Dim offset As Long = index * tensorLength \ NDArrayMath.lengthPerSlice(ret2)

			If sliceIdx = 0 AndAlso length = NDArrayMath.lengthPerSlice(ret2) Then
				If offset > Integer.MaxValue Then
					Throw New ND4JArraySizeException()
				End If
				ret2 = ret2.slice(CInt(offset))
				If dimension.Length = 1 AndAlso ret2.RowVectorOrScalar Then
					Return ret2
				End If
				Return ret2.permutei(finalPermuteDims)

			ElseIf length = NDArrayMath.lengthPerSlice(ret2) Then
				offset -= ret2.slices() * (offset \ ret2.slices())

				If offset > Integer.MaxValue Then
					Throw New ND4JArraySizeException()
				End If
				ret2 = ret2.slice(CInt(offset))
				If dimension.Length = 1 AndAlso ret2.RowVectorOrScalar Then
					Return ret2
				End If
				Return ret2.permutei(finalPermuteDims)
			End If

			Do While ret2.length() > length
				sliceIdx = NDArrayMath.sliceOffsetForTensor(index, ret2, tensorShape)
				sliceIdx -= ret2.slices() * (sliceIdx \ ret2.slices())
				ret2 = ret2.slice(sliceIdx)
			Loop

			If dimension.Length = 1 AndAlso ret2.RowVectorOrScalar Then
				Return ret2
			End If

			Return ret2.permutei(finalPermuteDims)
		End Function

		Public Overridable Function vectorsAlongDimension(ByVal dimension As Integer) As Long Implements INDArray.vectorsAlongDimension
			If dimension = 0 AndAlso ArrayList OrElse RowVectorOrScalar Then
				Return 1
			End If
			If size(dimension) = 1 AndAlso Not ArrayList Then
				Dim i As Integer = dimension
				Do While i < rank()
					If size(i) <> 1 Then
						Return vectorsAlongDimension(i)
					End If
					i += 1
				Loop

				Return length()

			ElseIf size(0) = 1 AndAlso Not VectorOrScalar Then
				Dim realDimension As Integer = rank() - LeadingOnes
				Dim length As Long = Me.length()
				If length \ size(realDimension) >= Integer.MaxValue Then
					Throw New System.ArgumentException("Vectors along dimension can not be >= Integer.MAX_VALUE")
				End If
				Return length \ size(realDimension)
			End If

			Dim length As Long = Me.length()

			If dimension >= jvmShapeInfo.rank Then
				If length \ size(jvmShapeInfo.rank - 1) >= Integer.MaxValue Then
					Throw New System.ArgumentException("Vectors along dimension can not be >= Integer.MAX_VALUE")
				End If
				Return CInt(length \ size(jvmShapeInfo.rank - 1))
			End If
			If length \ size(dimension) >= Integer.MaxValue Then
				Throw New System.ArgumentException("Vectors along dimension can not be >= Integer.MAX_VALUE")
			End If
			Return length \ size(dimension)
		End Function

		Public Overridable Function vectorAlongDimension(ByVal index As Integer, ByVal dimension As Integer) As INDArray Implements INDArray.vectorAlongDimension
			If dimension < 0 Then
				dimension = jvmShapeInfo.getRank() + dimension
			End If

			'return the whole thing
			If dimension = jvmShapeInfo.getRank() - 1 AndAlso size(dimension) = 1 AndAlso rank() > 2 OrElse rank() > 2 AndAlso dimension = 0 AndAlso size(dimension) = 1 Then
				Return Me
			End If

			Return tensorAlongDimension(index, dimension)
		End Function

		Public Overridable WriteOnly Property Order Implements INDArray.setOrder As Char
			Set(ByVal order As Char)
				ShapeInformation = Nd4j.ShapeInfoProvider.createShapeInformation(shape(), stride(), elementWiseStride(), order, Me.dataType(), Empty)
			End Set
		End Property

		Public Overridable Sub setShapeAndStride(ByVal shape() As Integer, ByVal stride() As Integer) Implements INDArray.setShapeAndStride
			ShapeInformation = Nd4j.ShapeInfoProvider.createShapeInformation(ArrayUtil.toLongArray(shape), ArrayUtil.toLongArray(stride), 0, ordering(), Me.dataType(), False)
		End Sub

		Public Overridable Function cumsumi(ByVal dimension As Integer) As INDArray Implements INDArray.cumsumi
			validateNumericalArray("cumsumi", True)

			If Scalar OrElse Empty Then
				Return Me
			End If

			If ArrayList Then
				Dim s As Double = 0.0
				Dim i As Integer = 0
				Do While i < length()
					s += getDouble(i)
					putScalar(i, s)
					i += 1
				Loop
			ElseIf dimension = Integer.MaxValue Then
				Dim flattened As INDArray = ravel()
				Dim prevVal As Double = flattened.getDouble(0)
				Dim i As Integer = 1
				Do While i < flattened.length()
					Dim d As Double = prevVal + flattened.getDouble(i)
					flattened.putScalar(i, d)
					prevVal = d
					i += 1
				Loop

				Return flattened
			Else
				Dim i As Integer = 0
				Do While i < vectorsAlongDimension(dimension)
					Dim vec As INDArray = vectorAlongDimension(i, dimension)
					vec.cumsumi(0)

					i += 1
				Loop
			End If

			Return Me
		End Function

		Public Overridable Function normmaxNumber() As Number Implements INDArray.normmaxNumber
			Return normmax(Integer.MaxValue).getDouble(0)
		End Function

		Public Overridable Function norm2Number() As Number Implements INDArray.norm2Number
			Return norm2(Integer.MaxValue).getDouble(0)
		End Function

		Public Overridable Function norm1Number() As Number Implements INDArray.norm1Number
			Return norm1(Integer.MaxValue).getDouble(0)
		End Function

		Public Overridable Function stdNumber() As Number Implements INDArray.stdNumber
			Return std(Integer.MaxValue).getDouble(0)
		End Function

		Public Overridable Function prodNumber() As Number Implements INDArray.prodNumber
			If Scalar Then
				Return getNumber(0)
			End If
			Return prod(Integer.MaxValue).getDouble(0)
		End Function

		Public Overridable Function meanNumber() As Number Implements INDArray.meanNumber
			validateNumericalArray("meanNumber", False)
			If Scalar Then
				Return getNumber(0)
			End If
			Return mean(Integer.MaxValue).getDouble(0)
		End Function

		Public Overridable Function ameanNumber() As Number Implements INDArray.ameanNumber
			Return amean(Integer.MaxValue).getDouble(0)
		End Function

		Public Overridable Function varNumber() As Number Implements INDArray.varNumber
			Return var(Integer.MaxValue).getDouble(0)
		End Function

		Public Overridable Function maxNumber() As Number Implements INDArray.maxNumber
			If Scalar Then
				Return getNumber(0)
			End If
			Return max(Integer.MaxValue).getDouble(0)
		End Function

		Public Overridable Function amaxNumber() As Number Implements INDArray.amaxNumber
			Return amax(Integer.MaxValue).getDouble(0)
		End Function

		Public Overridable Function minNumber() As Number Implements INDArray.minNumber
			If Scalar Then
				Return getNumber(0)
			End If
			Return min(Integer.MaxValue).getDouble(0)
		End Function

		Public Overridable Function aminNumber() As Number Implements INDArray.aminNumber
			Return amin(Integer.MaxValue).getDouble(0)
		End Function

		Public Overridable Function scan(ByVal condition As Condition) As Number Implements INDArray.scan
			Dim op As New MatchCondition(Me, condition)
			Return Nd4j.Executioner.exec(op).getDouble(0)
		End Function

		Public Overridable Function sumNumber() As Number Implements INDArray.sumNumber
			validateNumericalArray("sum", False)
			If Scalar Then
				Return getNumber(0)
			End If
			Dim scalar As val = sum(Integer.MaxValue)
			Nd4j.Executioner.commit()
			Return scalar.getDouble(0)
		End Function

		Public Overridable Function entropyNumber() As Number Implements INDArray.entropyNumber
			Return entropy(Integer.MaxValue).getDouble(0)
		End Function

		Public Overridable Function shannonEntropyNumber() As Number Implements INDArray.shannonEntropyNumber
			Return shannonEntropy(Integer.MaxValue).getDouble(0)
		End Function

		Public Overridable Function logEntropyNumber() As Number Implements INDArray.logEntropyNumber
			Return logEntropy(Integer.MaxValue).getDouble(0)
		End Function

		Public Overridable Function cumsum(ByVal dimension As Integer) As INDArray Implements INDArray.cumsum
			validateNumericalArray("cumsum", True)
			Return dup().cumsumi(dimension)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: @Override public INDArray assign(final INDArray arr)
		Public Overridable Function assign(ByVal arr As INDArray) As INDArray Implements INDArray.assign
			Preconditions.checkState((Me.Scalar AndAlso arr.Scalar) OrElse (Me.Vector AndAlso arr.Vector) OrElse Shape.shapeEqualWithSqueeze(Me.shape(), arr.shape()), "Cannot assign arrays: arrays must both be scalars, both vectors, or shapes must be equal other than size 1 dimensions. Attempting to do x.assign(y)" & " with x.shape=%ndShape and y.shape=%ndShape", Me, arr)

			Preconditions.checkArgument(Me.length() = arr.length(), "Length of both arrays must be equal")

			Nd4j.Executioner.exec(New org.nd4j.linalg.api.ops.impl.transforms.any.Assign(arr, Me))
			Return Me
		End Function

		Public Overridable Function putScalar(ByVal i As Long, ByVal value As Double) As INDArray Implements INDArray.putScalar
			Preconditions.checkArgument(dataType() <> DataType.BOOL OrElse value = 0.0 OrElse value = 1.0, "Cannot put value %s into boolean array" & " - only putScalar with values 0 or 1 is allowed on boolean arrays", value)
			If i < 0 Then
				i += rank()
			End If

			' TODO: i'm not sure that rank == 1 has fair shortcut here
			If Scalar Then
				autoProcessScalarCall()
				data_Conflict.put(i, value)
				Return Me
			ElseIf rank() = 1 Then
				data_Conflict.put(i * stride(0), value)
				Return Me
			End If

			' we cant raise rank here, if original rank is 1
			If RowVector AndAlso rank() = 2 Then
				Return putScalar(0, i, value)
			ElseIf ColumnVector AndAlso rank() = 2 Then
				Return putScalar(i, 0, value)
			End If
			Dim indexes() As Long = If(ordering() = "c"c, Shape.ind2subC(Me, i), Shape.ind2sub(Me, i))
			Return putScalar(indexes, value)
		End Function

		Public Overridable Function putScalar(ByVal i As Long, ByVal value As Single) As INDArray Implements INDArray.putScalar
			Return putScalar(i, CDbl(value))
		End Function

		Public Overridable Function putScalar(ByVal i As Long, ByVal value As Integer) As INDArray Implements INDArray.putScalar
			Return putScalar(i, CDbl(value))
		End Function

		Public Overridable Function putScalar(ByVal indexes() As Integer, ByVal value As Double) As INDArray Implements INDArray.putScalar
			Nd4j.Compressor.autoDecompress(Me)

			Preconditions.checkArgument(dataType() <> DataType.BOOL OrElse value = 0.0 OrElse value = 1.0, "Cannot put value %s into boolean array" & " - only putScalar with values 0 or 1 is allowed on boolean arrays", value)

			For i As Integer = 0 To indexes.Length - 1
				If indexes(i) < 0 Then
					indexes(i) += Me.size(i)
				End If
			Next i

			If indexes.Length = 1 Then
				Return putScalar(indexes(0), value)
			ElseIf indexes.Length = 2 Then
				Return putScalar(indexes(0), indexes(1), value)
			ElseIf indexes.Length = 3 Then
				Return putScalar(indexes(0), indexes(1), indexes(2), value)
			ElseIf indexes.Length = 4 Then
				Return putScalar(indexes(0), indexes(1), indexes(2), indexes(3), value)
			Else
				autoProcessScalarCall()
				Dim offset As Long = Shape.getOffset(jvmShapeInfo.javaShapeInformation, indexes)
				data_Conflict.put(offset, value)
			End If
			Return Me
		End Function

		Public Overridable Function putScalar(ByVal indexes() As Long, ByVal value As Double) As INDArray Implements INDArray.putScalar
			Nd4j.Compressor.autoDecompress(Me)

			Preconditions.checkArgument(dataType() <> DataType.BOOL OrElse value = 0.0 OrElse value = 1.0, "Cannot put value %s into boolean array" & " - only putScalar with values 0 or 1 is allowed on boolean arrays", value)


			For i As Integer = 0 To indexes.Length - 1
				If indexes(i) < 0 Then
					indexes(i) += size(i)
				End If
			Next i

			If indexes.Length = 1 Then
				Return putScalar(indexes(0), value)
			ElseIf indexes.Length = 2 Then
				Return putScalar(indexes(0), indexes(1), value)
			ElseIf indexes.Length = 3 Then
				Return putScalar(indexes(0), indexes(1), indexes(2), value)
			ElseIf indexes.Length = 4 Then
				Return putScalar(indexes(0), indexes(1), indexes(2), indexes(3), value)
			Else
				autoProcessScalarCall()
				Dim offset As Long = Shape.getOffset(jvmShapeInfo.javaShapeInformation, indexes)
				data_Conflict.put(offset, value)
			End If
			Return Me
		End Function

		Public Overridable Function putScalar(ByVal indexes() As Long, ByVal value As Single) As INDArray Implements INDArray.putScalar
			Return putScalar(indexes, CDbl(value))
		End Function

		Public Overridable Function putScalar(ByVal row As Long, ByVal col As Long, ByVal value As Double) As INDArray Implements INDArray.putScalar
			Nd4j.Compressor.autoDecompress(Me)
			autoProcessScalarCall()

			Preconditions.checkArgument(dataType() <> DataType.BOOL OrElse value = 0.0 OrElse value = 1.0, "Cannot put value %s into boolean array" & " - only putScalar with values 0 or 1 is allowed on boolean arrays", value)

			If rank() > 2 Then
				Throw New System.InvalidOperationException("Cannot use putScalar(int,int,double) on a rank " & rank() & " INDArray")
			End If
			Dim offset As Long = Shape.getOffsetUnsafe(jvmShapeInfo.javaShapeInformation, row, col)
			data_Conflict.put(offset, value)
			Return Me
		End Function

		Public Overridable Function putScalar(ByVal dim0 As Long, ByVal dim1 As Long, ByVal dim2 As Long, ByVal value As Double) As INDArray Implements INDArray.putScalar
			Nd4j.Compressor.autoDecompress(Me)
			autoProcessScalarCall()

			Preconditions.checkArgument(dataType() <> DataType.BOOL OrElse value = 0.0 OrElse value = 1.0, "Cannot put value %s into boolean array" & " - only putScalar with values 0 or 1 is allowed on boolean arrays", value)

			If rank() <> 3 Then
				Throw New System.InvalidOperationException("Cannot use putScalar(int,int,int,double) on a rank " & rank() & " INDArray")
			End If
			Dim offset As Long = 0 ' Shape.getOffsetUnsafe(javaShapeInformation, dim0, dim1, dim2);
			Dim size_0 As Long = jvmShapeInfo.javaShapeInformation(1)
			Dim size_1 As Long = jvmShapeInfo.javaShapeInformation(1 + 1)
			Dim size_2 As Long = jvmShapeInfo.javaShapeInformation(1 + 2)

			If size_0 <> 1 Then
				offset += dim0 * jvmShapeInfo.javaShapeInformation(1 + 0 + 3)
			End If
			If size_1 <> 1 Then
				offset += dim1 * jvmShapeInfo.javaShapeInformation(1 + 1 + 3)
			End If
			If size_2 <> 1 Then
				offset += dim2 * jvmShapeInfo.javaShapeInformation(1 + 2 + 3)
			End If

			data_Conflict.put(offset, value)
			Return Me
		End Function

		Public Overridable Function putScalar(ByVal dim0 As Long, ByVal dim1 As Long, ByVal dim2 As Long, ByVal dim3 As Long, ByVal value As Double) As INDArray Implements INDArray.putScalar
			Nd4j.Compressor.autoDecompress(Me)
			autoProcessScalarCall()
			Preconditions.checkArgument(dataType() <> DataType.BOOL OrElse value = 0.0 OrElse value = 1.0, "Cannot put value %s into boolean array" & " - only putScalar with values 0 or 1 is allowed on boolean arrays", value)

			If rank() <> 4 Then
				Throw New System.InvalidOperationException("Cannot use putScalar(int,int,int,int,double) on a rank " & rank() & " INDArray")
			End If
			Dim offset As Long = Shape.getOffsetUnsafe(jvmShapeInfo.javaShapeInformation, dim0, dim1, dim2, dim3)
			data_Conflict.put(offset, value)
			Return Me
		End Function

		Public Overridable Function putScalar(ByVal indexes() As Integer, ByVal value As Single) As INDArray Implements INDArray.putScalar
			Return putScalar(indexes, CDbl(value))
		End Function

		Public Overridable Function putScalar(ByVal indexes() As Integer, ByVal value As Integer) As INDArray Implements INDArray.putScalar
			Return putScalar(indexes, CDbl(value))
		End Function

		Public Overridable Function putScalar(ByVal indexes() As Long, ByVal value As Integer) As INDArray Implements INDArray.putScalar
			Return putScalar(indexes, CDbl(value))
		End Function

		Public Overridable Function eps(ByVal other As Number) As INDArray Implements INDArray.eps
			validateNumericalArray("eps", True)
			Return Nd4j.Executioner.exec(New ScalarEps(Me, createUninitialized(DataType.BOOL, Me.shape(), Me.ordering()), other))
		End Function

		Public Overridable Function eps(ByVal other As INDArray) As INDArray Implements INDArray.eps
			validateNumericalArray("eps", True)
			Return Nd4j.Executioner.exec(New Eps(Me, other, createUninitialized(DataType.BOOL, Me.shape(), Me.ordering())))
		End Function

		Public Overridable Function lt(ByVal other As Number) As INDArray Implements INDArray.lt
			validateNumericalArray("less than (lt)", False)
			Return Nd4j.Executioner.exec(New ScalarLessThan(Me, createUninitialized(DataType.BOOL, Me.shape(), Me.ordering()), other))
		End Function

		Public Overridable Function lte(ByVal other As Number) As INDArray Implements INDArray.lte
			validateNumericalArray("less than or equals (lte)", False)
			Return Nd4j.Executioner.exec(New ScalarLessThanOrEqual(Me, createUninitialized(DataType.BOOL, Me.shape(), Me.ordering()), other))
		End Function

		Public Overridable Function eq(ByVal other As Number) As INDArray Implements INDArray.eq
			Preconditions.checkArgument(dataType() <> DataType.BOOL OrElse other.doubleValue() = 0.0 OrElse other.doubleValue() = 1.0, "Scalar equality on boolean arrays can only be applied with values 0 or 1: got value %s",other)
			Return Nd4j.Executioner.exec(New ScalarEquals(Me, createUninitialized(DataType.BOOL, Me.shape(), Me.ordering()), other))
		End Function

		Public Overridable Function gt(ByVal other As Number) As INDArray Implements INDArray.gt
			validateNumericalArray("greater than (gt)", False)
			Return Nd4j.Executioner.exec(New ScalarGreaterThan(Me, createUninitialized(DataType.BOOL, Me.shape(), Me.ordering()), other))
		End Function

		Public Overridable Function gte(ByVal other As Number) As INDArray Implements INDArray.gte
			validateNumericalArray("greater than or equals (gte)", False)
			Return Nd4j.Executioner.exec(New ScalarGreaterThanOrEqual(Me, createUninitialized(DataType.BOOL, Me.shape(), Me.ordering()), other))
		End Function

		Public Overridable Function lt(ByVal other As INDArray) As INDArray Implements INDArray.lt
			validateNumericalArray("less than (lt)", False)
			If Shape.shapeEquals(Me.shape(), other.shape()) Then
				Return Nd4j.Executioner.exec(New LessThan(Me, other, createUninitialized(DataType.BOOL, Me.shape(), Me.ordering())))(0)
			ElseIf Shape.areShapesBroadcastable(Me.shape(), other.shape()) Then
				Return exec(New LessThan(New INDArray(){Me, other}, New INDArray(){createUninitialized(DataType.BOOL, Shape.broadcastOutputShape(Me.shape(), other.shape()))}))(0)
			Else
				Throw New System.ArgumentException("Shapes must be broadcastable")
			End If
		End Function

		Public Overridable Function neq(ByVal other As Number) As INDArray Implements INDArray.neq
			Preconditions.checkArgument(dataType() <> DataType.BOOL OrElse other.doubleValue() = 0.0 OrElse other.doubleValue() = 1.0, "Scalar non-equality on boolean arrays can only be applied with values 0 or 1: got value %s",other)
			Preconditions.checkState(Not Empty, "Cannot perform operation neq (not equal) on empty array")
			Return Nd4j.Executioner.exec(New ScalarNotEquals(Me, createUninitialized(DataType.BOOL, Me.shape(), Me.ordering()), other))
		End Function

		Public Overridable Function neq(ByVal other As INDArray) As INDArray Implements INDArray.neq
			Preconditions.checkState(Not Empty, "Cannot perform operation neq (not equal) on empty array")
			Return Nd4j.Executioner.exec(New NotEqualTo(Me, other, createUninitialized(DataType.BOOL, Me.shape(), Me.ordering())))(0)
		End Function

		Public Overridable Function eq(ByVal other As INDArray) As INDArray Implements INDArray.eq
			If Shape.shapeEquals(Me.shape(), other.shape()) Then
				Return Nd4j.Executioner.exec(New EqualTo(Me, other, createUninitialized(DataType.BOOL, Me.shape(), Me.ordering())))(0)
			ElseIf Shape.areShapesBroadcastable(Me.shape(), other.shape()) Then
				Return exec(New EqualTo(New INDArray(){Me, other}, New INDArray(){createUninitialized(DataType.BOOL, Shape.broadcastOutputShape(Me.shape(), other.shape()))}))(0)
			Else
				Throw New System.ArgumentException("Shapes must be broadcastable")
			End If
		End Function

		Public Overridable Function gt(ByVal other As INDArray) As INDArray Implements INDArray.gt
			validateNumericalArray("greater than (gt)", False)
			If Shape.shapeEquals(Me.shape(), other.shape()) Then
				Return Nd4j.Executioner.exec(New GreaterThan(Me, other, createUninitialized(DataType.BOOL, Me.shape(), Me.ordering())))(0)
			ElseIf Shape.areShapesBroadcastable(Me.shape(), other.shape()) Then
				Return exec(New GreaterThan(New INDArray(){Me, other}, New INDArray(){createUninitialized(DataType.BOOL, Shape.broadcastOutputShape(Me.shape(), other.shape()))}))(0)
			Else
				Throw New System.ArgumentException("Shapes must be broadcastable")
			End If
		End Function

		Public Overridable ReadOnly Property Infinite As INDArray Implements INDArray.isInfinite
			Get
				validateNumericalArray("isInfinite", True)
				If Empty Then
					Return empty(DataType.BOOL)
				End If
				Return Nd4j.Executioner.exec(New MatchConditionTransform(Me, createUninitialized(DataType.BOOL, Me.shape(), Me.ordering()), Conditions.Infinite))
			End Get
		End Property

		Public Overridable ReadOnly Property NaN As INDArray Implements INDArray.isNaN
			Get
				validateNumericalArray("isNaN", True)
				If Empty Then
					Return empty(DataType.BOOL)
				End If
				Return Nd4j.Executioner.exec(New MatchConditionTransform(Me, createUninitialized(DataType.BOOL, Me.shape(), Me.ordering()), Conditions.Nan))
			End Get
		End Property

		Public Overridable Function neg() As INDArray Implements INDArray.neg
			validateNumericalArray("negative (neg)", True)
			If Empty Then
				Return Me
			End If
			Return Nd4j.Executioner.exec(New Negative(Me, createUninitialized(Me.dataType(), Me.shape(), Me.ordering())))
		End Function

		Public Overridable Function negi() As INDArray Implements INDArray.negi
			validateNumericalArray("negative (negi)", True)
			If Empty Then
				Return Me
			End If
			Nd4j.Executioner.exec(New Negative(Me))
			Return Me
		End Function

		Public Overridable Function rdiv(ByVal n As Number, ByVal result As INDArray) As INDArray Implements INDArray.rdiv
			Return rdivi(n, result)
		End Function

		Public Overridable Function rdivi(ByVal n As Number, ByVal result As INDArray) As INDArray Implements INDArray.rdivi
			validateNumericalArray("rdivi", False)
			If Double.IsNaN(n.doubleValue()) Then
				n = EPS_THRESHOLD
			End If
			Nd4j.Executioner.exec(New ScalarReverseDivision(Me, Nothing, result, n))
			Return result
		End Function

		Public Overridable Function rsub(ByVal n As Number, ByVal result As INDArray) As INDArray Implements INDArray.rsub
			Return rsubi(n, result)
		End Function

		Public Overridable Function rsubi(ByVal n As Number, ByVal result As INDArray) As INDArray Implements INDArray.rsubi
			validateNumericalArray("rsubi", False)

			If Double.IsNaN(n.doubleValue()) Then
				n = EPS_THRESHOLD
			End If

			Nd4j.Executioner.exec(New ScalarReverseSubtraction(Me, result, n))
			Return result
		End Function

		Public Overridable Function div(ByVal n As Number, ByVal result As INDArray) As INDArray Implements INDArray.div
			Return divi(n, result)
		End Function

		Public Overridable Function divi(ByVal n As Number, ByVal result As INDArray) As INDArray Implements INDArray.divi
			validateNumericalArray("divi", False)

			If Double.IsNaN(n.doubleValue()) Then
				n = EPS_THRESHOLD
			End If
			Nd4j.Executioner.exec(New ScalarDivision(Me, Nothing, result, n))
			Return result
		End Function

		Public Overridable Function mul(ByVal n As Number, ByVal result As INDArray) As INDArray Implements INDArray.mul
			Return muli(n, result)
		End Function

		Public Overridable Function muli(ByVal n As Number, ByVal result As INDArray) As INDArray Implements INDArray.muli
			validateNumericalArray("muli", False)
			If Double.IsNaN(n.doubleValue()) Then
				n = EPS_THRESHOLD
			End If
			Nd4j.Executioner.exec(New ScalarMultiplication(Me, Nothing, result, n))
			Return result
		End Function

		Public Overridable Function [sub](ByVal n As Number, ByVal result As INDArray) As INDArray Implements INDArray.sub
			Return subi(n, result)
		End Function

		Public Overridable Function subi(ByVal n As Number, ByVal result As INDArray) As INDArray Implements INDArray.subi
			validateNumericalArray("subi", False)

			If Double.IsNaN(n.doubleValue()) Then
				n = EPS_THRESHOLD
			End If

			Nd4j.Executioner.exec(New ScalarSubtraction(Me, Nothing, result, n))
			Return result
		End Function

		Public Overridable Function add(ByVal n As Number, ByVal result As INDArray) As INDArray Implements INDArray.add
			Return addi(n, result)
		End Function

		Public Overridable Function addi(ByVal n As Number, ByVal result As INDArray) As INDArray Implements INDArray.addi
			validateNumericalArray("addi", False)
			If Double.IsNaN(n.doubleValue()) Then
				n = EPS_THRESHOLD
			End If

			Nd4j.Executioner.exec(New ScalarAdd(Me, Nothing, result, n))
			Return result
		End Function

		Public Overridable Function getScalar(ByVal row As Long, ByVal column As Long) As INDArray Implements INDArray.getScalar
			Return getScalar(New Long() {row, column})
		End Function

		Public Overridable Function dup() As INDArray Implements INDArray.dup
			Return dup(order())
		End Function

		Public Overridable Function dup(ByVal order As Char) As INDArray Implements INDArray.dup
			WorkspaceUtils.assertValidArray(Me, "Cannot duplicate INDArray")
			If Me.Compressed AndAlso Me.ordering() = order Then
				Dim ret As INDArray = createArrayFromShapeBuffer(data().dup(), Me.shapeInfoDataBuffer())
				ret.markAsCompressed(True)
				Return ret
			End If
			If Empty Then
				Return Me
			End If

			Nd4j.Compressor.autoDecompress(Me)

			' fixme: eventually it would be nice to have this in native code
			If S Then
				Dim list As val = New List(Of String)()
				For e As Integer = 0 To Me.length() - 1
					list.add(Me.getString(e))
				Next e

				Return create(list, Me.shape(), Me.ordering())
			End If

			Dim z As val = createUninitialized(Me.dataType(), Me.shape(), order)
			z.assign(Me)
			Return z
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function getInt(ParamArray ByVal indices_Conflict() As Integer) As Integer Implements INDArray.getInt
			Return CInt(Math.Truncate(getDouble(indices_Conflict)))
		End Function

		Public Overridable Function getLong(ByVal index As Long) As Long Implements INDArray.getLong
			Nd4j.Compressor.autoDecompress(Me)
			Preconditions.checkState(Not Empty, "Unable to get value from empty array")

			If index >= length() Then
				Throw New System.ArgumentException("Unable to get linear index " & index & ": values is greater than length (" & length() & ")")
			End If

			autoProcessScalarCall()

			If index = 0 Then
				Return data().getLong(index)
			End If

			Dim dimensions() As Long = If(ordering() = "c"c, Shape.ind2subC(Me, index), Shape.ind2sub(Me, index))
			Shape.assertShapeLessThan(dimensions, shape())
			Return getLong(dimensions)
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function getLong(ParamArray ByVal indices_Conflict() As Long) As Long Implements INDArray.getLong
			If Scalar Then
				Return data().getLong(0)
			End If
			Return Shape.getLong(Me, indices_Conflict)
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function getDouble(ParamArray ByVal indices_Conflict() As Integer) As Double Implements INDArray.getDouble
			autoProcessScalarCall()
			Nd4j.Compressor.autoDecompress(Me)
			Preconditions.checkState(Not Empty, "Unable to get value from empty array")

			For i As Integer = 0 To indices_Conflict.Length - 1
				If indices_Conflict(i) < 0 Then
					indices_Conflict(i) += rank()
				End If
			Next i
			If indices_Conflict.Length = 1 Then
				If rank() = 1 Then
					Return Shape.getDouble(Me, indices_Conflict(0))
				ElseIf RowVector Then
					Return Shape.getDouble(Me, 0, indices_Conflict(0))
				ElseIf ColumnVector Then
					Return Shape.getDouble(Me, indices_Conflict(0), 0)
				ElseIf (Scalar OrElse length() = 1) AndAlso indices_Conflict(0) = 0 Then
					Return data().getDouble(0)
				End If
			End If
			Return Shape.getDouble(Me, indices_Conflict)
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function getDouble(ParamArray ByVal indices_Conflict() As Long) As Double Implements INDArray.getDouble
			autoProcessScalarCall()
			Nd4j.Compressor.autoDecompress(Me)
			Preconditions.checkState(Not Empty, "Unable to get value from empty array")

			For i As Integer = 0 To indices_Conflict.Length - 1
				If indices_Conflict(i) < 0 Then
					indices_Conflict(i) += rank()
				End If
			Next i
			If indices_Conflict.Length = 1 Then
				If rank() = 1 Then
					Return Shape.getDouble(Me, indices_Conflict(0))
				ElseIf RowVector Then
					Return Shape.getDouble(Me, 0, indices_Conflict(0))
				ElseIf ColumnVector Then
					Return Shape.getDouble(Me, indices_Conflict(0), 0)
				ElseIf Scalar AndAlso indices_Conflict(0) = 0 Then
					Return data().getDouble(0)
				Else
					Throw New System.InvalidOperationException("Indexes length must be > 1 for non vectors and scalars")
				End If
			End If
			Return Shape.getDouble(Me, indices_Conflict)
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function getFloat(ParamArray ByVal indices_Conflict() As Integer) As Single Implements INDArray.getFloat
			Return CSng(getDouble(indices_Conflict))
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function getFloat(ParamArray ByVal indices_Conflict() As Long) As Single Implements INDArray.getFloat
			Return CSng(getDouble(indices_Conflict))
		End Function

		Public Overridable ReadOnly Property Scalar As Boolean Implements INDArray.isScalar
			Get
				If Empty Then
					Return False
				End If
    
				If jvmShapeInfo.rank = 0 Then
					Return True
				ElseIf jvmShapeInfo.rank > 2 Then
					Return False
				ElseIf jvmShapeInfo.rank = 1 Then
					Return shape()(0) = 1
				ElseIf jvmShapeInfo.rank = 2 Then
					Return shape()(0) = 1 AndAlso shape()(1) = 1 OrElse length() = 1
    
				Else
					Return False
				End If
    
			End Get
		End Property

'JAVA TO VB CONVERTER NOTE: The parameter indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function put(ByVal indices_Conflict() As Integer, ByVal element As INDArray) As INDArray Implements INDArray.put
			Nd4j.Compressor.autoDecompress(Me)
			If Not element.Scalar Then
				Throw New System.ArgumentException("Unable to insert anything but a scalar")
			End If
			If RowVector AndAlso indices_Conflict(0) = 0 AndAlso indices_Conflict.Length = 2 Then
				Dim ix As Integer = 0
				For i As Integer = 1 To indices_Conflict.Length - 1
					ix += indices_Conflict(i) * stride(i)
				Next i
				If ix >= data_Conflict.length() Then
					Throw New System.ArgumentException("Illegal indices " & java.util.Arrays.toString(indices_Conflict))
				End If
				data_Conflict.put(ix, element.getDouble(0))
			Else
				Dim ix As Integer = 0
				For i As Integer = 0 To indices_Conflict.Length - 1
					If size(i) <> 1 Then
						ix += indices_Conflict(i) * stride(i)
					End If
				Next i
				If ix >= data_Conflict.length() Then
					Throw New System.ArgumentException("Illegal indices " & java.util.Arrays.toString(indices_Conflict))
				End If
				data_Conflict.put(ix, element.getDouble(0))
			End If
			Return Me
		End Function

		Public Overridable Function match(ByVal comp As INDArray, ByVal condition As Condition) As INDArray Implements INDArray.match
			' TODO: obviously, we can make this broadcastable, eventually. But this will require new CustomOp based on MatchCondition
			Preconditions.checkArgument(Me.shape().SequenceEqual(comp.shape()), "Shapes must be equal")
			Preconditions.checkArgument(Me.dataType() = comp.dataType(), "Data types bmust be equal")
			Return Nd4j.Executioner.exec(New MatchConditionTransform(Me, comp, createUninitialized(DataType.BOOL, Me.shape()), condition))
		End Function

		Public Overridable Function match(ByVal comp As Number, ByVal condition As Condition) As INDArray Implements INDArray.match
			condition.setValue(comp)
			Return Nd4j.Executioner.exec(New MatchConditionTransform(Me, EPS_THRESHOLD, condition))
		End Function

		Public Overridable Function getWhere(ByVal comp As INDArray, ByVal condition As Condition) As INDArray Implements INDArray.getWhere
			Return BooleanIndexing.chooseFrom(New INDArray(){Me, comp},condition)
		End Function

		Public Overridable Function getWhere(ByVal comp As Number, ByVal condition As Condition) As INDArray Implements INDArray.getWhere
			Return BooleanIndexing.chooseFrom(New INDArray(){Me},java.util.Arrays.asList(comp.doubleValue()),Enumerable.Empty(Of Integer)(),condition)
		End Function

		Public Overridable Function putWhere(ByVal comp As INDArray, ByVal put As INDArray, ByVal condition As Condition) As INDArray Implements INDArray.putWhere
			Nd4j.Compressor.autoDecompress(Me)
			Dim matchCondition As New MatchConditionTransform(Me,comp,condition)
			Nd4j.Executioner.exec(matchCondition)
			Return putWhereWithMask(matchCondition.z(),put)
		End Function

		Public Overridable Function putWhere(ByVal comp As Number, ByVal put As INDArray, ByVal condition As Condition) As INDArray Implements INDArray.putWhere
			Return putWhere(scalar(comp),put,condition)
		End Function

		Public Overridable Function putWhere(ByVal comp As Number, ByVal put As Number, ByVal condition As Condition) As INDArray Implements INDArray.putWhere
			Return putWhere(scalar(comp),scalar(put),condition)
		End Function


		Public Overridable Function putWhereWithMask(ByVal mask As INDArray, ByVal put As INDArray) As INDArray Implements INDArray.putWhereWithMask
			Dim output As INDArray = dup()
			Nd4j.Executioner.execAndReturn(New Where(New INDArray(){mask, Me, put},New INDArray(){output}))
			Return output
		End Function

		Public Overridable Function putWhereWithMask(ByVal mask As INDArray, ByVal put As Number) As INDArray Implements INDArray.putWhereWithMask
			Return putWhereWithMask(mask,scalar(put))
		End Function

		Public Overridable Function put(ByVal i As Integer, ByVal j As Integer, ByVal element As INDArray) As INDArray Implements INDArray.put
			Return put(New Integer() {i, j}, element)
		End Function

		Public Overridable Function put(ByVal i As Integer, ByVal j As Integer, ByVal element As Number) As INDArray Implements INDArray.put
			Return putScalar(New Integer() {i, j}, element.doubleValue())
		End Function

		Public Overridable Function putSlice(ByVal slice As Integer, ByVal put As INDArray) As INDArray Implements INDArray.putSlice
			Nd4j.Compressor.autoDecompress(Me)


			If Scalar Then
				Preconditions.checkState(put.Scalar, "Invalid dimension. Can only insert a scalar in to another scalar")
				Me.put(0, put.getScalar(0))
				Return Me
			ElseIf ArrayList Then
				Preconditions.checkState(put.VectorOrScalar AndAlso put.length() = length(), "Invalid dimension on insertion. Can only insert scalars/vectors into other scalar/vectors")
				If put.Scalar Then
					putScalar(slice, put.getDouble(0))
				Else
					Dim i As Integer = 0
					Do While i < length()
						putScalar(i, put.getDouble(i))
						i += 1
					Loop
				End If
				Return Me
			End If

			assertSlice(put, slice)


			Dim view As INDArray = Me.slice(slice)

			If put.length() = 1 Then
				putScalar(slice, put.getDouble(0))
			Else
				If Not (view.Vector AndAlso put.Vector AndAlso view.length() = put.length()) AndAlso Not view.equalShapes(put) Then
					Throw New System.InvalidOperationException("Cannot put slice: array to be put (" & java.util.Arrays.toString(put.shape()) & ") and slice array (" & java.util.Arrays.toString(view.shape()) & ") have different shapes")
				End If
				view.assign(put)
			End If
			Return Me
		End Function

		Protected Friend Overridable Sub assertSlice(ByVal put As INDArray, ByVal slice As Long)
			Preconditions.checkArgument(slice < slices(), "Invalid slice specified: slice %s must be in range 0 (inclusive) to numSlices=%s (exclusive)", slice, slices())
			Dim sliceShape() As Long = put.shape()
			If Shape.isRowVectorShape(sliceShape) Then
				Return
			Else
				Dim requiredShape() As Long = ArrayUtil.removeIndex(shape(), 0)

				'no need to compare for scalar; primarily due to shapes either being [1] or length 0
				If put.Scalar Then
					Return
				End If

				If ArrayList AndAlso put.Vector AndAlso put.length() < length() Then
					Return
				End If
				'edge case for column vectors
				If Shape.isColumnVectorShape(sliceShape) Then
					Return
				End If
				If Not Shape.shapeEquals(sliceShape, requiredShape) AndAlso Not Shape.isRowVectorShape(requiredShape) AndAlso Not Shape.isRowVectorShape(sliceShape) Then
					Throw New System.InvalidOperationException(String.Format("Invalid shape size of {0} . Should have been {1} ", java.util.Arrays.toString(sliceShape), java.util.Arrays.toString(requiredShape)))
				End If
			End If
		End Sub

		Public Overridable ReadOnly Property Matrix As Boolean Implements INDArray.isMatrix
			Get
				Return rank() = 2
			End Get
		End Property

'JAVA TO VB CONVERTER NOTE: The parameter newShape was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Protected Friend Overridable Function newShape(ByVal newShape_Conflict() As Long, ByVal ordering As Char) As INDArray

			Return create(data(), newShape_Conflict, stride(), 0, ordering)
		End Function

		Protected Friend Overridable Function create(ByVal data As DataBuffer, ByVal newShape() As Integer, ByVal newStrides() As Integer, ByVal offset As Long, ByVal ordering As Char) As INDArray
			Return create(data, newShape, newStrides, offset, ordering)
		End Function

		Protected Friend Overridable Function create(ByVal data As DataBuffer, ByVal newShape() As Long, ByVal newStrides() As Long, ByVal offset As Long, ByVal ordering As Char) As INDArray
			Return create(data, newShape, newStrides, offset, ordering)
		End Function

		Protected Friend Overridable Function create(ByVal data As DataBuffer, ByVal newShape() As Integer, ByVal newStrides() As Integer, ByVal offset As Long) As INDArray
			Return create(data, newShape, newStrides, offset)
		End Function

		Protected Friend Overridable Function create(ByVal shape() As Integer) As INDArray
			Return create(shape, getStrides(shape, order()), 0)
		End Function

		Protected Friend Overridable Function create(ByVal shape() As Integer, ByVal strides() As Integer, ByVal offset As Long) As INDArray
			Return create(shape, strides, offset)
		End Function

		Protected Friend Overridable Function getStrides(ByVal shape() As Integer, ByVal ordering As Char) As Integer()
			Return getStrides(shape, ordering)
		End Function

		Public Overridable Function squaredDistance(ByVal other As INDArray) As Double Implements INDArray.squaredDistance
			validateNumericalArray("squaredDistance", False)
			Dim d2 As Double = distance2(other)
			Return d2 * d2
		End Function

		Public Overridable Function distance2(ByVal other As INDArray) As Double Implements INDArray.distance2
			validateNumericalArray("distance2", False)
			Nd4j.Compressor.autoDecompress(Me)
			Return Nd4j.Executioner.execAndReturn(New EuclideanDistance(Me, other)).getFinalResult().doubleValue()
		End Function

		Public Overridable Function distance1(ByVal other As INDArray) As Double Implements INDArray.distance1
			validateNumericalArray("distance1", False)
			Nd4j.Compressor.autoDecompress(Me)
			Return Nd4j.Executioner.execAndReturn(New ManhattanDistance(Me, other)).getFinalResult().doubleValue()
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function get(ByVal indices_Conflict As INDArray) As INDArray Implements INDArray.get
			If indices_Conflict.rank() > 2 Then
				Throw New ND4JIllegalArgumentException("Indices must be a vector or matrix.")
			End If

			If rank() = 1 Then
				Preconditions.checkArgument(indices_Conflict.rank() <= 1, "For 1D vector indices must be either scalar or vector as well")
				Dim ret As val = createUninitialized(Me.dataType(), indices_Conflict.length())
				For e As Integer = 0 To indices_Conflict.length() - 1
					Dim idx As val = indices_Conflict.getLong(e)
					Dim value As val = getDouble(idx)
					ret.putScalar(e, value)
				Next e

				Return ret
			ElseIf indices_Conflict.rows() = rank() Then
				Dim ret As INDArray = create(Me.dataType(), indices_Conflict.columns())

				Dim i As Integer = 0
				Do While i < indices_Conflict.columns()
'JAVA TO VB CONVERTER NOTE: The variable specifiedIndex was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
					Dim specifiedIndex_Conflict() As Integer = indices_Conflict.getColumn(i).dup().data().asInt()
					Dim v As val = getDouble(specifiedIndex_Conflict)
					ret.putScalar(i, v)
					i += 1
				Loop

				Return ret
			Else
				Dim arrList As IList(Of INDArray) = New List(Of INDArray)()

				If indices_Conflict.Matrix OrElse indices_Conflict.ColumnVector OrElse (indices_Conflict.Scalar AndAlso indices_Conflict.rank() = 2) Then ' we need this for compatibility with legacy code
					Dim i As Integer = 0
					Do While i < indices_Conflict.rows()
						If i = 0 Then
							Dim row As INDArray = indices_Conflict.getRow(i)
							For j As Integer = 0 To row.length() - 1
								arrList.Add(slice(row.getInt(j)))
							Next j
						Else
							Dim row As INDArray = indices_Conflict.slice(i)
							For j As Integer = 0 To row.length() - 1
								Dim put As INDArray = arrList(j).slice(row.getInt(j))
								put = put.reshape(Longs.concat(New Long(){1},put.shape()))
								arrList(j) = put
							Next j
						End If

						i += 1
					Loop
				ElseIf indices_Conflict.RowVector Then
					For i As Integer = 0 To indices_Conflict.length() - 1
						Dim add As INDArray = slice(indices_Conflict.getInt(i))
						add = add.reshape(Longs.concat(New Long() {1},add.shape()))
						arrList.Add(add)
					Next i
				End If

				Return concat(0,CType(arrList, List(Of INDArray)).ToArray())

			End If


		End Function

'JAVA TO VB CONVERTER NOTE: The parameter indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function put(ByVal indices_Conflict As INDArray, ByVal element As INDArray) As INDArray Implements INDArray.put
			If indices_Conflict.rank() > 2 Then
				Throw New ND4JIllegalArgumentException("Indices must be a vector or matrix.")
			End If

			If indices_Conflict.rows() = rank() Then
				Dim ndIndexIterator As New NdIndexIterator(element.shape())
				Dim i As Integer = 0
				Do While i < indices_Conflict.columns()
'JAVA TO VB CONVERTER NOTE: The variable specifiedIndex was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
					Dim specifiedIndex_Conflict() As Integer = indices_Conflict.getColumn(i).dup().data().asInt()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					putScalar(specifiedIndex_Conflict,element.getDouble(ndIndexIterator.next()))
					i += 1
				Loop
			Else
				Dim arrList As IList(Of INDArray) = New List(Of INDArray)()

				If indices_Conflict.Matrix OrElse indices_Conflict.ColumnVector Then
					Dim i As Integer = 0
					Do While i < indices_Conflict.rows()
						Dim row As INDArray = indices_Conflict.getRow(i)
						For j As Integer = 0 To row.length() - 1
							Dim slice As INDArray = Me.slice(row.getInt(j))
							Nd4j.Executioner.execAndReturn(New Assign(New INDArray(){slice, element},New INDArray(){slice}))
							arrList.Add(Me.slice(row.getInt(j)))
						Next j
						i += 1
					Loop
				ElseIf indices_Conflict.RowVector Then
					For i As Integer = 0 To indices_Conflict.length() - 1
						arrList.Add(slice(indices_Conflict.getInt(i)))
					Next i
				End If
			End If
			Return Me
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function put(ByVal indices_Conflict() As INDArrayIndex, ByVal element As INDArray) As INDArray
			Nd4j.Compressor.autoDecompress(Me)
			Dim isSpecifiedIndex As Boolean = False
			For Each idx As INDArrayIndex In indices_Conflict
				If TypeOf idx Is SpecifiedIndex Then
					isSpecifiedIndex = True
					Exit For
				End If
			Next idx

			If Not isSpecifiedIndex Then
				Return get(indices_Conflict).assign(element)
			Else
				'Can't get a view, so we'll do it in subsets instead
				' This is inefficient, but it is correct...
				Dim numSpecified As Integer = 0
				Dim specifiedIdxs As IList(Of Long()) = New List(Of Long())()
				Dim specifiedIdxDims As IList(Of Integer) = New List(Of Integer)()

				Dim destinationIndices() As INDArrayIndex = CType(indices_Conflict.Clone(), org.nd4j.linalg.indexing.INDArrayIndex()) 'Shallow clone
				Dim sourceIndices() As INDArrayIndex = CType(indices_Conflict.Clone(), org.nd4j.linalg.indexing.INDArrayIndex())
				For i As Integer = 0 To indices_Conflict.Length - 1
					Dim idx As INDArrayIndex = indices_Conflict(i)
					If TypeOf idx Is SpecifiedIndex Then
						numSpecified += 1
						Dim idxs() As Long = DirectCast(idx, SpecifiedIndex).getIndexes()
						specifiedIdxs.Add(idxs)
						specifiedIdxDims.Add(i)
					ElseIf TypeOf idx Is PointIndex Then
						'Example: [2,3,3].put(point(1), ..., [1,x,y]) -> can't use point(1) on [1,x,y]
						sourceIndices(i) = NDArrayIndex.point(0)
					End If
				Next i
				Dim counts(specifiedIdxs.Count - 1) As Integer
				Dim dims(specifiedIdxDims.Count - 1) As Integer
				For i As Integer = 0 To specifiedIdxs.Count - 1
					counts(i) = specifiedIdxs(i).Length
					dims(i) = specifiedIdxDims(i)
				Next i

				Dim iter As New NdIndexIterator(counts)
				Do While iter.MoveNext()
					Dim iterationIdxs() As Long = iter.Current
					For i As Integer = 0 To iterationIdxs.Length - 1
						Dim indicesForDim() As Long = specifiedIdxs(i)
						destinationIndices(dims(i)) = NDArrayIndex.point(indicesForDim(CInt(iterationIdxs(i))))
						sourceIndices(dims(i)) = NDArrayIndex.point(iterationIdxs(i))
					Next i

					Dim sourceView As INDArray = element.get(sourceIndices)
					Dim destinationView As INDArray = Me.get(destinationIndices)
					destinationView.assign(sourceView)
				Loop
			End If
			Return Me
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function put(ByVal indices_Conflict() As INDArrayIndex, ByVal element As Number) As INDArray
			Nd4j.Compressor.autoDecompress(Me)
			Dim get As INDArray = Me.get(indices_Conflict)
			Dim i As Integer = 0
			Do While i < get.length()
				get.putScalar(i, element.doubleValue())
				i += 1
			Loop
			Return Me
		End Function

		Public Overridable Function swapAxes(ByVal dimension As Integer, ByVal [with] As Integer) As INDArray Implements INDArray.swapAxes
			Dim shape() As Integer = ArrayUtil.range(0, Me.shape().Length)
			shape(dimension) = [with]
			shape([with]) = dimension
			Return permute(shape)
		End Function


		Public Overridable ReadOnly Property View As Boolean Implements INDArray.isView
			Get
		'        
		'            We don't really use Shape offset value anywhere
		'            And it's possible to be not a view, and have non-empty originalBuffer
		'         
				' length/data.length can be different in case of Threshold conversion
				If Empty OrElse S Then
					Return False
				End If
    
				Dim c2 As val = (length() < data().length() AndAlso data_Conflict.dataType() <> DataType.INT)
				Dim c3 As val = (data().originalDataBuffer() IsNot Nothing AndAlso data_Conflict IsNot data_Conflict.originalDataBuffer())
    
				Return c2 OrElse c3
			End Get
		End Property

		Public Overridable ReadOnly Property Sparse As Boolean Implements INDArray.isSparse
			Get
				Return False
			End Get
		End Property

		Public Overridable Function data() As DataBuffer
			Return data_Conflict
		End Function

		Public Overridable WriteOnly Property Data As DataBuffer
			Set(ByVal data As DataBuffer)
				Me.data_Conflict = data
			End Set
		End Property

		Public Overridable Function slices() As Long Implements INDArray.slices
			Return size(0)
		End Function

		Protected Friend Overridable Function create(ByVal buffer As DataBuffer) As INDArray
			Return create(buffer)
		End Function

		Public Overridable Function cond(ByVal condition As Condition) As INDArray Implements INDArray.cond
			If Empty Then
				Return empty(DataType.BOOL)
			End If
			Dim ret As INDArray = createUninitialized(DataType.BOOL, Me.shape())
			Nd4j.Executioner.exec(New MatchConditionTransform(Me,ret, condition))
			Return ret
		End Function

		Protected Friend Overridable Sub init(ByVal shape() As Integer, ByVal stride() As Integer)
			'null character
			If shapeInformation_Conflict Is Nothing OrElse jvmShapeInfo Is Nothing OrElse ordering() = ChrW(&H0000) Then
				'Shape.setOrder(shapeInfo(), Nd4j.order());
				Dim si As val = Nd4j.ShapeInfoProvider.createShapeInformation(ArrayUtil.toLongArray(shape), ArrayUtil.toLongArray(stride), 1, order(), Me.dataType(), False)
				ShapeInformation = si
			End If

		End Sub

		Protected Friend Overridable Sub init(ByVal shape() As Long, ByVal stride() As Long)
			'null character
			If shapeInformation_Conflict Is Nothing OrElse jvmShapeInfo Is Nothing OrElse ordering() = ChrW(&H0000) Then
				Dim si As val = Nd4j.ShapeInfoProvider.createShapeInformation(shape,stride, 1, order(), Me.dataType(), False)
				ShapeInformation = si
			End If

		End Sub

		Public Overridable Function getScalar(ByVal i As Long) As INDArray Implements INDArray.getScalar
			If i >= Me.length() Then
				Throw New ND4JIllegalStateException("Index can't be greater then array length")
			End If

			If i < 0 Then
				i += Me.length()
			End If

			Dim idx As Long = If(Me.Scalar, 0, Shape.getOffset(jvmShapeInfo.javaShapeInformation, Shape.ind2subC(Me.shape(), i)))
			Dim buffer As val = createBuffer(Me.data(), Me.data().originalOffset() + idx, 1)
			Dim shape As val = Nd4j.ShapeInfoProvider.createShapeInformation(New Long(){}, New Long(){},1,"c"c, dataType(), False)
			Return createArrayFromShapeBuffer(buffer, shape)
		End Function

		''' <summary>
		''' Do a row wise op (a,s,m,d)
		''' a : add
		''' s : subtract
		''' m : multiply
		''' d : divide
		''' h : reverse subtraction
		''' t : reverse division
		''' </summary>
		''' <param name="columnVector"> the column  vector </param>
		''' <param name="operation">    the operation
		''' @return </param>
		Protected Friend Overridable Function doColumnWise(ByVal columnVector As INDArray, ByVal operation As Char) As INDArray
			Nd4j.Compressor.autoDecompress(Me)
		   If columnVector.Scalar Then
			   Select Case operation
				   Case "a"c
					   addi(columnVector.getDouble(0))
				   Case "p"c
					   assign(columnVector.getDouble(0))
				   Case "s"c
					   subi(columnVector.getDouble(0))
				   Case "m"c
					   muli(columnVector.getDouble(0))
				   Case "d"c
					   divi(columnVector.getDouble(0))
				   Case "h"c
					   rsubi(columnVector.getDouble(0))
				   Case "t"c
					   rdivi(columnVector.getDouble(0))

			   End Select

			   Return Me

		   ElseIf Scalar Then
			   Select Case operation
				   Case "a"c
					   Return columnVector.addi(getDouble(0))
				   Case "p"c
					   Return columnVector.assign(getDouble(0))
				   Case "s"c
					   Return columnVector.subi(getDouble(0))
				   Case "m"c
					   Return columnVector.muli(getDouble(0))
				   Case "d"c
					   Return columnVector.divi(getDouble(0))
				   Case "h"c
					   Return columnVector.rsubi(getDouble(0))
				   Case "t"c
					   Return columnVector.rdivi(getDouble(0))

			   End Select
		   End If

			'Input validation: require (a) columnVector to actually be a column vector, and (b) this.size(0) to match columnVector.size(0)
			'Or, simply require it to be a rank 1 vector
			If (Not columnVector.ColumnVector AndAlso columnVector.rank() > 1) OrElse Me.size(0) <> columnVector.size(0) OrElse columnVector.length() <= 1 Then
				Throw New System.InvalidOperationException("Mismatched shapes (shape = " & java.util.Arrays.toString(shape()) & ", column vector shape =" & java.util.Arrays.toString(columnVector.shape()) & ")")
			End If

			If columnVector.data().sameUnderlyingData(data()) Then
				Return doColumnWise(columnVector.dup(), operation)
			End If
			If equalShapes(columnVector) Then
				Select Case operation
					Case "a"c
						addi(columnVector)
					Case "p"c
						assign(columnVector)
					Case "s"c
						subi(columnVector)
					Case "m"c
						muli(columnVector)
					Case "d"c
						divi(columnVector)
					Case "h"c
						rsubi(columnVector)
					Case "t"c
						rdivi(columnVector)
				End Select

				Return Me
			End If
			If rows() = 1 AndAlso columnVector.Scalar Then
				applyScalarOp(columnVector, operation)
			Else
				' special optimization case, broadcast turns into ScalarOp Along Dimension
				If rank() = 2 AndAlso elementWiseStride() = 1 AndAlso ordering() = "c"c AndAlso columnVector.elementWiseStride() = 1 Then
					Select Case operation
						Case "a"c
							Dim op As New ScalarAdd(Me, columnVector, Me, 0.0)
							op.Dimension = 1
							Nd4j.Executioner.exec(op)
						Case "p"c
							Dim op As New ScalarSet(Me, columnVector, Me, 0.0)
							op.Dimension = 1
							Nd4j.Executioner.exec(op)
						Case "s"c
							Dim op As New ScalarSubtraction(Me, columnVector, Me, 0.0)
							op.Dimension = 1
							Nd4j.Executioner.exec(op)
						Case "m"c
							Dim op As New ScalarMultiplication(Me, columnVector, Me, 0.0)
							op.Dimension = 1
							Nd4j.Executioner.exec(op)
						Case "d"c
							Dim op As New ScalarDivision(Me, columnVector, Me, 0.0)
							op.Dimension = 1
							Nd4j.Executioner.exec(op)
						Case "h"c
							Dim op As New ScalarReverseSubtraction(Me, columnVector, Me, 0.0)
							op.Dimension = 1
							Nd4j.Executioner.exec(op)
						Case "t"c
							Dim op As New ScalarReverseDivision(Me, columnVector, Me, 0.0)
							op.Dimension = 1
							Nd4j.Executioner.exec(op)
					End Select
				Else
					applyBroadcastOp(columnVector, operation)
				End If

			End If

			Return Me

		End Function

		''' <summary>
		''' Do a row wise op (a,s,m,d)
		''' a : add
		''' s : subtract
		''' m : multiply
		''' d : divide
		''' h : reverse subtraction
		''' t : reverse division
		''' </summary>
		''' <param name="rowVector"> the row vector </param>
		''' <param name="operation"> the operation
		''' @return </param>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: protected INDArray doRowWise(INDArray rowVector, final char operation)
		Protected Friend Overridable Function doRowWise(ByVal rowVector As INDArray, ByVal operation As Char) As INDArray
			Nd4j.Compressor.autoDecompress(Me)


			If rowVector.Scalar Then
				Select Case operation
					Case "a"c
						addi(rowVector.getDouble(0))
					Case "p"c
						assign(rowVector.getDouble(0))
					Case "s"c
						subi(rowVector.getDouble(0))
					Case "m"c
						muli(rowVector.getDouble(0))
					Case "d"c
						divi(rowVector.getDouble(0))
					Case "h"c
						rsubi(rowVector.getDouble(0))
					Case "t"c
						rdivi(rowVector.getDouble(0))

				End Select

				Return Me
			ElseIf Scalar Then
				Select Case operation
					Case "a"c
						Return rowVector.addi(getDouble(0))
					Case "p"c
						Return rowVector.assign(getDouble(0))
					Case "s"c
						Return rowVector.subi(getDouble(0))
					Case "m"c
						Return rowVector.muli(getDouble(0))
					Case "d"c
						Return rowVector.divi(getDouble(0))
					Case "h"c
						Return rowVector.rsubi(getDouble(0))
					Case "t"c
						Return rowVector.rdivi(getDouble(0))

				End Select
			End If

			'Input validation: require (a) rowVector to actually be a row vector, and (b) this.size(1) to match rowVector.size(1)
			If Not rowVector.RowVector OrElse Me.rank() > 1 AndAlso rowVector.rank() > 1 AndAlso Me.size(1) <> rowVector.size(1) OrElse rowVector.length() <= 1 Then
				Throw New System.InvalidOperationException("Mismatched shapes (shape = " & java.util.Arrays.toString(shape()) & ", row vector shape =" & java.util.Arrays.toString(rowVector.shape()) & ")")
			End If

			If rowVector.data().sameUnderlyingData(data()) Then
				Return doRowWise(rowVector.dup(), operation)
			End If

			If ArrayList Then
				Select Case operation
					Case "a"c
						addi(rowVector)
					Case "p"c
						assign(rowVector)
					Case "s"c
						subi(rowVector)
					Case "m"c
						muli(rowVector)
					Case "d"c
						divi(rowVector)
					Case "h"c
						rsubi(rowVector)
					Case "t"c
						rdivi(rowVector)
				End Select

				Return Me
			End If

			If rank() = 2 AndAlso columns() = 1 AndAlso rowVector.Scalar Then
				applyScalarOp(rowVector, operation)
			Else
				' special optimization case, broadcast turns into ScalarOp Along Dimension
				If rank() = 2 AndAlso elementWiseStride() = 1 AndAlso ordering() = "f"c AndAlso rowVector.elementWiseStride() = 1 Then
					Select Case operation
						Case "a"c
							Dim op As New ScalarAdd(Me, rowVector, Me, 0.0)
							op.Dimension = 0
							Nd4j.Executioner.exec(op)
						Case "p"c
							Dim op As New ScalarSet(Me, rowVector, Me, 0.0)
							op.Dimension = 0
							Nd4j.Executioner.exec(op)
						Case "s"c
							Dim op As New ScalarSubtraction(Me, rowVector, Me, 0.0)
							op.Dimension = 0
							Nd4j.Executioner.exec(op)
						Case "m"c
							Dim op As New ScalarMultiplication(Me, rowVector, Me, 0.0)
							op.Dimension = 0
							Nd4j.Executioner.exec(op)
						Case "d"c
							Dim op As New ScalarDivision(Me, rowVector, Me, 0.0)
							op.Dimension = 0
							Nd4j.Executioner.exec(op)
						Case "h"c
							Dim op As New ScalarReverseSubtraction(Me, rowVector, Me, 0.0)
							op.Dimension = 0
							Nd4j.Executioner.exec(op)
						Case "t"c
							Dim op As New ScalarReverseDivision(Me, rowVector, Me, 0.0)
							op.Dimension = 0
							Nd4j.Executioner.exec(op)

					End Select
				Else
					applyBroadcastOp(rowVector, operation)
				End If
			End If

			Return Me
		End Function


'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: private void applyBroadcastOp(INDArray vector, final char operation)
		Private Sub applyBroadcastOp(ByVal vector As INDArray, ByVal operation As Char)
			Nd4j.Compressor.autoDecompress(Me)
			Dim alongDimension As Integer = If(Shape.isRowVectorShape(vector.shape()), 1, 0)

			' FIXME: probably this is wrong, because strict equality is always false in current DataBuffer mechanics
			If Me.data() Is vector.data() Then
				vector = vector.dup()
			End If
			Select Case operation
				Case "a"c
					Nd4j.Executioner.exec(New BroadcastAddOp(Me, vector, Me, alongDimension))
					Return
				Case "s"c
					Nd4j.Executioner.exec(New BroadcastSubOp(Me, vector, Me, alongDimension))
					Return
				Case "m"c
					Nd4j.Executioner.exec(New BroadcastMulOp(Me, vector, Me, alongDimension))
					Return
				Case "d"c
					Nd4j.Executioner.exec(New BroadcastDivOp(Me, vector, Me, alongDimension))
					Return
				Case "h"c
					Nd4j.Executioner.exec(New BroadcastRSubOp(Me, vector, Me, alongDimension))
					Return
				Case "t"c
					Nd4j.Executioner.exec(New BroadcastRDivOp(Me, vector, Me, alongDimension))
					Return
				Case "p"c
					Nd4j.Executioner.exec(New BroadcastCopyOp(Me, vector, Me, alongDimension))
					Return
				Case Else
					Throw New System.NotSupportedException("Unknown operation: " & operation)
			End Select
		End Sub

		Private Sub applyScalarOp(ByVal vector As INDArray, ByVal operation As Char)
			Nd4j.Compressor.autoDecompress(Me)
			Select Case operation
				Case "a"c
					addi(vector.getDouble(0))
				Case "s"c
					subi(vector.getDouble(0))
				Case "m"c
					muli(vector.getDouble(0))
				Case "d"c
					divi(vector.getDouble(0))
				Case "h"c
					rsubi(vector.getDouble(0))
				Case "t"c
					rdivi(vector.getDouble(0))
			End Select
		End Sub

		Protected Friend Overridable Function shapeOf() As DataBuffer
			'        if (shape == null)
			'            shape = Shape.shapeOf(shapeInfoDataBuffer());
			'        return shape;

			Return Shape.shapeOf(shapeInfoDataBuffer())
		End Function

		Protected Friend Overridable Function strideOf() As DataBuffer
			'        if (stride == null)
			'            stride = Shape.stride(shapeInfoDataBuffer());
			'        return stride;
			Return Shape.stride(shapeInfoDataBuffer())
		End Function

		Public Overridable Function stride(ByVal dimension As Integer) As Integer Implements INDArray.stride
			Dim rank As Integer = jvmShapeInfo.rank
			Preconditions.checkArgument(dimension < rank, "Cannot get stride for dimension %s from rank %s array: " & "dimension indices must be in range -rank <= dimension < rank", dimension, rank)
			If dimension < 0 Then
				Return CInt(stride()(dimension + rank))
			End If
			Return CInt(stride()(dimension))
		End Function

		Public Overridable Function rdiviColumnVector(ByVal columnVector As INDArray) As INDArray Implements INDArray.rdiviColumnVector
			validateNumericalArray("rdiviColumnVector", False)
			Return doColumnWise(columnVector, "t"c)
		End Function

		Public Overridable Function rdivColumnVector(ByVal columnVector As INDArray) As INDArray Implements INDArray.rdivColumnVector
			validateNumericalArray("rdivColumnVector", False)
			Return dup().rdiviColumnVector(columnVector)
		End Function

		Public Overridable Function rdiviRowVector(ByVal rowVector As INDArray) As INDArray Implements INDArray.rdiviRowVector
			validateNumericalArray("rdiviRowVector", False)
			Return doRowWise(rowVector, "t"c)
		End Function

		Public Overridable Function rdivRowVector(ByVal rowVector As INDArray) As INDArray Implements INDArray.rdivRowVector
			validateNumericalArray("rdivRowVector", False)
			Return dup().rdiviRowVector(rowVector)
		End Function

		Public Overridable Function rsubiColumnVector(ByVal columnVector As INDArray) As INDArray Implements INDArray.rsubiColumnVector
			validateNumericalArray("rsubiColumnVector", False)
			Return doColumnWise(columnVector, "h"c)
		End Function

		Public Overridable Function rsubColumnVector(ByVal columnVector As INDArray) As INDArray Implements INDArray.rsubColumnVector
			validateNumericalArray("rsubColumnVector", False)
			Return dup().rsubiColumnVector(columnVector)
		End Function

		Public Overridable Function rsubiRowVector(ByVal rowVector As INDArray) As INDArray Implements INDArray.rsubiRowVector
			validateNumericalArray("rsubiRowVector", False)
			Return doRowWise(rowVector, "h"c)
		End Function

		Public Overridable Function rsubRowVector(ByVal rowVector As INDArray) As INDArray Implements INDArray.rsubRowVector
			validateNumericalArray("rsubRowVector", False)
			Return dup().rsubiRowVector(rowVector)
		End Function

		Public Overridable Function put(ByVal i As Integer, ByVal element As INDArray) As INDArray Implements INDArray.put
			Preconditions.checkArgument(element.Scalar, "Element must be a scalar: element has shape %ndShape", element)
			Return putScalar(i, element.getDouble(0))
		End Function

		Public Overridable Function diviColumnVector(ByVal columnVector As INDArray) As INDArray Implements INDArray.diviColumnVector
			validateNumericalArray("diviColumnVector", False)
			Return doColumnWise(columnVector, "d"c)
		End Function

		Public Overridable Function divColumnVector(ByVal columnVector As INDArray) As INDArray Implements INDArray.divColumnVector
			validateNumericalArray("divColumnVector", False)
			Return dup().diviColumnVector(columnVector)
		End Function

		Public Overridable Function diviRowVector(ByVal rowVector As INDArray) As INDArray Implements INDArray.diviRowVector
			validateNumericalArray("diviRowVector", False)
			Return doRowWise(rowVector, "d"c)
		End Function

		Public Overridable Function divRowVector(ByVal rowVector As INDArray) As INDArray Implements INDArray.divRowVector
			validateNumericalArray("divRowVector", False)
			Return dup().diviRowVector(rowVector)
		End Function

		Public Overridable Function muliColumnVector(ByVal columnVector As INDArray) As INDArray Implements INDArray.muliColumnVector
			validateNumericalArray("muliColumnVector", False)
			Return doColumnWise(columnVector, "m"c)
		End Function

		Public Overridable Function mulColumnVector(ByVal columnVector As INDArray) As INDArray Implements INDArray.mulColumnVector
			validateNumericalArray("mulColumnVector", False)
			Return dup().muliColumnVector(columnVector)
		End Function

		Public Overridable Function muliRowVector(ByVal rowVector As INDArray) As INDArray Implements INDArray.muliRowVector
			validateNumericalArray("muliRowVector", False)
			Return doRowWise(rowVector, "m"c)
		End Function

		Public Overridable Function mulRowVector(ByVal rowVector As INDArray) As INDArray Implements INDArray.mulRowVector
			validateNumericalArray("mulRowVector", False)
			Return dup().muliRowVector(rowVector)
		End Function

		Public Overridable Function subiColumnVector(ByVal columnVector As INDArray) As INDArray Implements INDArray.subiColumnVector
			validateNumericalArray("subiColumnVector", False)
			Return doColumnWise(columnVector, "s"c)
		End Function

		Public Overridable Function subColumnVector(ByVal columnVector As INDArray) As INDArray Implements INDArray.subColumnVector
			validateNumericalArray("subColumnVector", False)
			Return dup().subiColumnVector(columnVector)
		End Function

		Public Overridable Function subiRowVector(ByVal rowVector As INDArray) As INDArray Implements INDArray.subiRowVector
			validateNumericalArray("subiRowVector", False)
			Return doRowWise(rowVector, "s"c)
		End Function

		Public Overridable Function subRowVector(ByVal rowVector As INDArray) As INDArray Implements INDArray.subRowVector
			validateNumericalArray("subRowVector", False)
			Return dup().subiRowVector(rowVector)
		End Function

		Public Overridable Function addiColumnVector(ByVal columnVector As INDArray) As INDArray Implements INDArray.addiColumnVector
			validateNumericalArray("addiColumnVector", False)
			Return doColumnWise(columnVector, "a"c)
		End Function

		Public Overridable Function putiColumnVector(ByVal columnVector As INDArray) As INDArray Implements INDArray.putiColumnVector
			Return doColumnWise(columnVector, "p"c)
		End Function

		Public Overridable Function addColumnVector(ByVal columnVector As INDArray) As INDArray Implements INDArray.addColumnVector
			validateNumericalArray("addColumnVector", False)
			Return dup().addiColumnVector(columnVector)
		End Function

		Public Overridable Function addiRowVector(ByVal rowVector As INDArray) As INDArray Implements INDArray.addiRowVector
			validateNumericalArray("addiRowVector", False)
			Return doRowWise(rowVector, "a"c)
		End Function

		Public Overridable Function putiRowVector(ByVal rowVector As INDArray) As INDArray Implements INDArray.putiRowVector
			validateNumericalArray("putiRowVector", False)
			Return doRowWise(rowVector, "p"c)
		End Function

		Public Overridable Function addRowVector(ByVal rowVector As INDArray) As INDArray Implements INDArray.addRowVector
			validateNumericalArray("addRowVector", False)
			Return dup().addiRowVector(rowVector)
		End Function

		Public Overridable Function mmul(ByVal other As INDArray, ByVal result As INDArray, ByVal mMulTranspose As MMulTranspose) As INDArray Implements INDArray.mmul
			Return mMulTranspose.exec(Me, other, result)
		End Function

		Public Overridable Function mmul(ByVal other As INDArray, ByVal mMulTranspose As MMulTranspose) As INDArray Implements INDArray.mmul
			Return mMulTranspose.exec(Me, other, Nothing)
		End Function

		Public Overridable Function mmul(ByVal other As INDArray, ByVal resultOrder As Char) As INDArray Implements INDArray.mmul
			Preconditions.checkArgument(resultOrder = "c"c OrElse resultOrder = "f"c, "Order must be either 'c' or 'f', but [" & resultOrder & "] was given")
			Preconditions.checkState(Me.dataType() = other.dataType(), "Matrix multiplication: arrays must have same dtype: %s vs. %s", Me.dataType(), other.dataType())
			' FIXME: add support for 3D+ here?
			Dim shape() As Long = If(other.rank() = 1, New Long(){rows()}, New Long()){rows(), other.columns()}
			Dim result As INDArray = createUninitialized(Me.dataType(), shape, resultOrder)
			If result.Scalar Then
				Return scalar(Me.dataType(), Nd4j.BlasWrapper.dot(Me, other)).reshape(ChrW(1), 1)
			End If
			Return mmuli(other, result)
		End Function

		Public Overridable Function mmul(ByVal other As INDArray) As INDArray Implements INDArray.mmul
			Return mmul(other,If(Me.ordering() = "f"c AndAlso other.ordering() = "f"c AndAlso other.rank() <> 1, "f"c, "c"c))
		End Function

		Protected Friend Overridable Function create(ByVal shape() As Integer, ByVal ordering As Char) As INDArray
			Return create(shape, ordering)
		End Function

		Public Overridable Function toDoubleMatrix() As Double()()
			If Not Matrix Then
				Throw New ND4JIllegalStateException("Unable to create a 2d array from a non matrix! Shape: " & Shape.shapeToStringShort(Me))
			End If

			If Me.size(0) > Integer.MaxValue OrElse Me.size(1) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret[][] As Double = new Double[rows()][columns()]
			Dim ret()() As Double = RectangularArrays.RectangularDoubleArray(rows(), columns())
			For i As Integer = 0 To ret.Length - 1
				ret(i) = getRow(i).dup().data().asDouble()
			Next i

			Return ret
		End Function

		Public Overridable Function toDoubleVector() As Double() Implements INDArray.toDoubleVector
			If Not VectorOrScalar Then
				Throw New ND4JIllegalStateException("Unable to create a 1d array from a non vector! Shape: " & Shape.shapeToStringShort(Me))
			End If
			Return dup().data().asDouble()
		End Function

		Public Overridable Function toFloatVector() As Single() Implements INDArray.toFloatVector
			If Not VectorOrScalar Then
				Throw New ND4JIllegalStateException("Unable to create a 1d array from a non vector! Shape: " & Shape.shapeToStringShort(Me))
			End If
			Return dup().data().asFloat()
		End Function

		Public Overridable Function toFloatMatrix() As Single()()
			If Not Matrix Then
				Throw New ND4JIllegalStateException("Unable to create a 2d array from a non matrix! Shape: " & Shape.shapeToStringShort(Me))
			End If

			If Me.rows() > Integer.MaxValue OrElse Me.columns() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret[][] As Single = new Single[CInt(rows())][CInt(columns())]
			Dim ret()() As Single = RectangularArrays.RectangularSingleArray(CInt(rows()), CInt(columns()))
			For i As Integer = 0 To ret.Length - 1
				ret(i) = getRow(i).dup().data().asFloat()
			Next i

			Return ret
		End Function

		Public Overridable Function toIntVector() As Integer() Implements INDArray.toIntVector
			If Empty Then
				Return New Integer(){}
			End If

			If Not VectorOrScalar Then
				Throw New ND4JIllegalStateException("Unable to create a 1d array from a non vector! Shape: " & Shape.shapeToStringShort(Me))
			End If
			If View OrElse elementWiseStride() <> 1 Then
				Return dup().data().asInt()
			End If
			Return data().asInt()
		End Function

		Public Overridable Function toLongVector() As Long() Implements INDArray.toLongVector
			If Not VectorOrScalar Then
				Throw New ND4JIllegalStateException("Unable to create a 1d array from a non vector! Shape: " & Shape.shapeToStringShort(Me))
			End If
			If View OrElse elementWiseStride() <> 1 Then
				Return dup().data().asLong()
			End If
			Return data().asLong()
		End Function

		Public Overridable Function toLongMatrix() As Long()()
			If Not Matrix Then
				Throw New ND4JIllegalStateException("Unable to create a 2d array from a non matrix! Shape: " & Shape.shapeToStringShort(Me))
			End If

			If Me.rows() > Integer.MaxValue OrElse Me.columns() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret[][] As Long = new Long[CInt(rows())][CInt(columns())]
			Dim ret()() As Long = RectangularArrays.RectangularLongArray(CInt(rows()), CInt(columns()))
			For i As Integer = 0 To ret.Length - 1
				ret(i) = getRow(i).dup().data().asLong()
			Next i

			Return ret
		End Function

		Public Overridable Function toIntMatrix() As Integer()()
			If Not Matrix Then
				Throw New ND4JIllegalStateException("Unable to create a 2d array from a non matrix! Shape: " & Shape.shapeToStringShort(Me))
			End If

			If Me.rows() > Integer.MaxValue OrElse Me.columns() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ret[][] As Integer = new Integer[CInt(rows())][CInt(columns())]
			Dim ret()() As Integer = RectangularArrays.RectangularIntegerArray(CInt(rows()), CInt(columns()))
			For i As Integer = 0 To ret.Length - 1
				ret(i) = getRow(i).dup().data().asInt()
			Next i

			Return ret
		End Function

		''' <summary>
		''' Perform an copy matrix multiplication
		''' </summary>
		''' <param name="other">  the other matrix to perform matrix multiply with </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result of the matrix multiplication </returns>
		Public Overridable Function mmul(ByVal other As INDArray, ByVal result As INDArray) As INDArray Implements INDArray.mmul
			Return mmuli(other, result)
		End Function

		Public Overridable Function div(ByVal other As INDArray) As INDArray Implements INDArray.div
			If Shape.areShapesBroadcastable(Me.shape(), other.shape()) Then
				Return divi(other, createUninitialized(Shape.pickPairwiseDataType(Me.dataType(), other.dataType()), Shape.broadcastOutputShape(Me.shape(), other.shape()), Me.ordering()))
			Else
				Return divi(other, createUninitialized(Shape.pickPairwiseDataType(Me.dataType(), other.dataType()), Me.shape(), Me.ordering()))
			End If
		End Function

		Public Overridable Function div(ByVal other As INDArray, ByVal result As INDArray) As INDArray Implements INDArray.div
			validateNumericalArray("div", True)
			Return divi(other, result)
		End Function

		Public Overridable Function mul(ByVal other As INDArray) As INDArray Implements INDArray.mul
			validateNumericalArray("mul", False)
			If Shape.areShapesBroadcastable(Me.shape(), other.shape()) Then
				Return muli(other, createUninitialized(Shape.pickPairwiseDataType(Me.dataType(), other.dataType()), Shape.broadcastOutputShape(Me.shape(), other.shape()), Me.ordering()))
			Else
				Dim z As val = createUninitialized(Shape.pickPairwiseDataType(Me.dataType(), other.dataType()), Me.shape(), Me.ordering())
				Return muli(other, z)
			End If
		End Function

		Public Overridable Function mul(ByVal other As INDArray, ByVal result As INDArray) As INDArray Implements INDArray.mul
			Return muli(other, result)
		End Function

		Public Overridable Function [sub](ByVal other As INDArray) As INDArray Implements INDArray.sub
			validateNumericalArray("sub", False)
			If Shape.areShapesBroadcastable(Me.shape(), other.shape()) Then
				Return subi(other, createUninitialized(Shape.pickPairwiseDataType(Me.dataType(), other.dataType()), Shape.broadcastOutputShape(Me.shape(), other.shape()), Me.ordering()))
			Else
				Return subi(other, createUninitialized(Shape.pickPairwiseDataType(Me.dataType(), other.dataType()), Me.shape(), Me.ordering()))
			End If
		End Function

		Public Overridable Function [sub](ByVal other As INDArray, ByVal result As INDArray) As INDArray Implements INDArray.sub
			Return subi(other, result)
		End Function

		Public Overridable Function add(ByVal other As INDArray) As INDArray Implements INDArray.add
			validateNumericalArray("add", False)
			If Shape.areShapesBroadcastable(Me.shape(), other.shape()) Then
				Return addi(other, createUninitialized(Shape.pickPairwiseDataType(Me.dataType(), other.dataType()), Shape.broadcastOutputShape(Me.shape(), other.shape()), Me.ordering()))
			Else
				Return addi(other, createUninitialized(Shape.pickPairwiseDataType(Me.dataType(), other.dataType()), Me.shape(), Me.ordering()))
			End If
		End Function

		Public Overridable Function add(ByVal other As INDArray, ByVal result As INDArray) As INDArray Implements INDArray.add
			validateNumericalArray("add", False)
			Return addi(other, result)
		End Function

		Public Overridable Function mmuli(ByVal other As INDArray, ByVal transpose As MMulTranspose) As INDArray Implements INDArray.mmuli
			validateNumericalArray("mmuli", False)
			Return dup().mmuli(other, Me,transpose)
		End Function

		Public Overridable Function mmuli(ByVal other As INDArray) As INDArray Implements INDArray.mmuli
			validateNumericalArray("mmuli", False)
			Return dup().mmuli(other, Me)
		End Function

		Public Overridable Function mmuli(ByVal other As INDArray, ByVal result As INDArray, ByVal transpose As MMulTranspose) As INDArray Implements INDArray.mmuli
			Return transpose.exec(Me, other, result)
		End Function

		Public Overridable Function mmuli(ByVal other As INDArray, ByVal result As INDArray) As INDArray Implements INDArray.mmuli
			validateNumericalArray("mmuli", False)
			LinAlgExceptions.assertMultiplies(Me, other)
			If other.rank() = 1 Then
				'GEMV edge case
				Preconditions.checkState(result.length() = Me.size(0) AndAlso Me.size(1) = other.size(0), "Invalid matrix multiplication: %ndShape x %ndShape with result shape %ndShape", Me, other, result)
			Else
				'Standard case
				Preconditions.checkState(result.rank() = 2 AndAlso result.size(0) = Me.size(0) AndAlso result.size(1) = other.size(1), "Invalid result array shape: expected shape [%s,%s], got shape %ndShape result array for %ndShape x %ndShape", Me.size(0), other.size(1), result, Me, other)
			End If

			If other.Scalar Then
				Return muli(other.getDouble(0), result)
			End If
			If Scalar Then
				Return other.muli(getDouble(0), result)
			End If

			' check sizes and resize if necessary 


			If result Is Me OrElse result Is other Then
	'             actually, blas cannot do multiplications in-place. Therefore, we will fake by
	'             * allocating a temporary object on the side and copy the result later.
	'             
				Dim temp As INDArray = create(result.dataType(), result.shape(), getStrides(result.shape(), "f"c), "f"c)

				If other.columns() = 1 OrElse other.rank() = 1 Then
					Nd4j.BlasWrapper.level2().gemv(BlasBufferUtil.getCharForTranspose(result), BlasBufferUtil.getCharForTranspose(Me), 1.0, Me, other, 0.0, temp)

				Else
					Nd4j.BlasWrapper.level3().gemm(BlasBufferUtil.getCharForTranspose(result), BlasBufferUtil.getCharForTranspose(Me), BlasBufferUtil.getCharForTranspose(temp), 1.0, Me, other, 0.0, temp)
				End If

				result.assign(temp)


			Else

				'We require that the result array is 'f' (fortran) order
				' However, user might have called mmuli with a c order array for the result
				' In which case, we need to allocate a temporary f order array, and later do an assign to the real result array

				Dim requiresTemp As Boolean = result.ordering() <> "f"c OrElse result.View OrElse Not Shape.hasDefaultStridesForShape(result)
				Dim gemmResultArr As INDArray
				If requiresTemp Then
					'Can use createUninitialized due to beta==0.0 parameter in gemm
					gemmResultArr = createUninitialized(result.dataType(), result.shape(), "f"c)
				Else
					gemmResultArr = result
				End If

				If other.columns() = 1 OrElse other.rank() = 1 Then
					Nd4j.BlasWrapper.level2().gemv(ordering(), BlasBufferUtil.getCharForTranspose(other), 1.0, Me, other, 0.0, gemmResultArr)
				Else
					'gemm doesn't support strides so vectors and views
					'don't work
					Nd4j.BlasWrapper.level3().gemm(ordering(), BlasBufferUtil.getCharForTranspose(other), BlasBufferUtil.getCharForTranspose(gemmResultArr), 1.0, Me, other, 0.0, gemmResultArr)
				End If

				If requiresTemp Then
					result.assign(gemmResultArr)
				End If
			End If

			' 1D edge case: reshape back to vector
			If other.rank() = 1 Then
				result = result.reshape(ChrW(result.length()))
			End If
			Return result
		End Function

		Private Function create(ByVal shape() As Integer, ByVal stride() As Integer) As INDArray
			Return create(shape, stride)
		End Function

		Public Overridable Function divi(ByVal other As INDArray) As INDArray Implements INDArray.divi
			Return divi(other, Me)
		End Function

		Public Overridable Function divi(ByVal other As INDArray, ByVal result As INDArray) As INDArray Implements INDArray.divi
			validateNumericalArray("divi", False)
			Shape.assertBroadcastable("divi", Me, other, result)
			exec(New DivOp(Me, other, result))
			Return result
		End Function

		Public Overridable Function muli(ByVal other As INDArray) As INDArray Implements INDArray.muli
			Return muli(other, Me)
		End Function

		Public Overridable Function muli(ByVal other As INDArray, ByVal result As INDArray) As INDArray Implements INDArray.muli
			validateNumericalArray("muli", False)
			Shape.assertBroadcastable("muli", Me, other, result)
			exec(New MulOp(Me, other, result))
			Return result
		End Function

		Public Overridable Function subi(ByVal other As INDArray) As INDArray Implements INDArray.subi
			Return subi(other, Me)
		End Function

		''' <summary>
		''' in place subtraction of two matrices
		''' </summary>
		''' <param name="other">  the second ndarray to subtract </param>
		''' <param name="result"> the result ndarray </param>
		''' <returns> the result of the subtraction </returns>
		Public Overridable Function subi(ByVal other As INDArray, ByVal result As INDArray) As INDArray Implements INDArray.subi
			validateNumericalArray("subi", False)
			Shape.assertBroadcastable("subi", Me, other, result)
			exec(New SubOp(Me, other, result))
			Return result
		End Function

		Public Overridable Function addi(ByVal other As INDArray) As INDArray Implements INDArray.addi
			Return addi(other, Me)
		End Function

		Public Overridable Function addi(ByVal other As INDArray, ByVal result As INDArray) As INDArray Implements INDArray.addi
			validateNumericalArray("addi", False)
			Shape.assertBroadcastable("addi", Me, other, result)
			exec(New AddOp(Me, other, result))
			Return result
		End Function

		Public Overridable Function normmax(ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.normmax
			validateNumericalArray("normmax", False)
			Return Nd4j.Executioner.exec(New NormMax(Me, keepDims, dimension))
		End Function

		Public Overridable Function normmax(ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.normmax
			Return normmax(False, dimension)
		End Function

		Public Overridable Function rdiv(ByVal other As INDArray) As INDArray Implements INDArray.rdiv
			validateNumericalArray("rdiv", False)
			If Shape.areShapesBroadcastable(Me.shape(), other.shape()) Then
				Return rdivi(other, createUninitialized(Shape.pickPairwiseDataType(Me.dataType(), other.dataType()), Shape.broadcastOutputShape(Me.shape(), other.shape()), Me.ordering()))
			Else
				Return rdivi(other, Me.ulike())
			End If
		End Function

		Public Overridable Function rdivi(ByVal other As INDArray) As INDArray Implements INDArray.rdivi
			Return rdivi(other, Me)
		End Function

		Public Overridable Function rdiv(ByVal other As INDArray, ByVal result As INDArray) As INDArray Implements INDArray.rdiv
			validateNumericalArray("rdiv", False)
			Return dup().rdivi(other, result)
		End Function

		Public Overridable Function rdivi(ByVal other As INDArray, ByVal result As INDArray) As INDArray Implements INDArray.rdivi
			validateNumericalArray("rdivi", False)
			Shape.assertBroadcastable("rdivi", Me, other, result)
			exec(New RDivOp(Me, other, result))
			Return result
		End Function

		Public Overridable Function rsub(ByVal other As INDArray, ByVal result As INDArray) As INDArray Implements INDArray.rsub
			validateNumericalArray("rsub", False)
			Return rsubi(other, result)
		End Function

		Public Overridable Function rsub(ByVal other As INDArray) As INDArray Implements INDArray.rsub
			validateNumericalArray("rsub", False)
			If Shape.areShapesBroadcastable(Me.shape(), other.shape()) Then
				Return rsubi(other, createUninitialized(Shape.pickPairwiseDataType(Me.dataType(), other.dataType()), Shape.broadcastOutputShape(Me.shape(), other.shape()), Me.ordering()))
			Else
				Return rsubi(other, Me.ulike())
			End If
		End Function

		Public Overridable Function rsubi(ByVal other As INDArray) As INDArray Implements INDArray.rsubi
			Return rsubi(other, Me)
		End Function

		Public Overridable Function rsubi(ByVal other As INDArray, ByVal result As INDArray) As INDArray Implements INDArray.rsubi
			validateNumericalArray("rsubi", False)
			Shape.assertBroadcastable("rsubi", Me, other, result)
			exec(New RSubOp(Me, other, result))
			Return result
		End Function

		Public Overridable Function assign(ByVal value As Number) As INDArray Implements INDArray.assign
			Preconditions.checkState(dataType() <> DataType.BOOL OrElse value.doubleValue() = 0.0 OrElse value.doubleValue() = 1.0, "Only values 0 or 1 are allowed for scalar " & "assign on boolean arrays: got value %s on to assign to boolean array with shape %ndShape", value, Me)
			Nd4j.Executioner.exec(New ScalarSet(Me, value))
			Return Me
		End Function

		Public Overridable Function assign(ByVal value As Boolean) As INDArray Implements INDArray.assign
			Return assign(If(value, 1, 0))
		End Function

		Public Overridable Function assignIf(ByVal arr As INDArray, ByVal condition As Condition) As INDArray Implements INDArray.assignIf
			BooleanIndexing.assignIf(Me, arr, condition)
			Return Me
		End Function

		Public Overridable Function replaceWhere(ByVal arr As INDArray, ByVal condition As Condition) As INDArray Implements INDArray.replaceWhere
			Nd4j.Compressor.autoDecompress(Me)
			BooleanIndexing.replaceWhere(Me, arr, condition)
			Return Me
		End Function

		<Obsolete>
		Public Overridable Function linearIndex(ByVal i As Long) As Long Implements INDArray.linearIndex
			Dim idx As Long = i
			Dim j As Integer = 0
			Do While j < jvmShapeInfo.rank - 1
				If size(CInt(i)) = 1 Then
					j += 1
					Continue Do
				End If
				idx += i * stride(j)
				j += 1
			Loop
			Return Shape.offset(jvmShapeInfo.javaShapeInformation) + (idx)
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter slice was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function slice(ByVal slice_Conflict As Long) As INDArray Implements INDArray.slice
			Nd4j.Compressor.autoDecompress(Me)


			Dim slices As Long = Me.slices()
			If slice_Conflict >= slices Then
				Throw New System.ArgumentException("Illegal slice " & slice_Conflict)
			End If

			If jvmShapeInfo.rank = 0 Then
				Throw New System.ArgumentException("Can't slice a 0-d NDArray")
			End If


			If slice_Conflict < 0 Then
				slice_Conflict += rank()
			End If
			Dim indexes(rank() - 1) As INDArrayIndex
			indexes(0) = NDArrayIndex.point(slice_Conflict)
			Dim i As Integer = 1
			Do While i < rank()
				indexes(i) = NDArrayIndex.all()
				i += 1
			Loop
			Return get(indexes)
		End Function



		Protected Friend Overridable Function createScalarForIndex(ByVal i As Long, ByVal applyOffset As Boolean) As INDArray
			If ArrayList Then
				Return getScalar(i)
			End If
			Return create(data(), New Long() {1, 1}, New Long() {1, 1}, i)
		End Function

		Protected Friend Overridable Function createScalar(ByVal d As Double) As INDArray
			Return scalar(d)
		End Function

		Public Overridable ReadOnly Property TrailingOnes As Integer Implements INDArray.getTrailingOnes
			Get
				Dim numLeadingOnes As Integer = 0
				For i As Integer = rank() - 1 To 1 Step -1
					If size(i) = 1 Then
						numLeadingOnes += 1
					End If
				Next i
    
				Return numLeadingOnes
			End Get
		End Property

		Public Overridable ReadOnly Property LeadingOnes As Integer Implements INDArray.getLeadingOnes
			Get
				Dim numLeadingOnes As Integer = 0
				Dim i As Integer = 0
				Do While i < rank()
					If size(i) = 1 Then
						numLeadingOnes += 1
					End If
					i += 1
				Loop
    
				Return numLeadingOnes
			End Get
		End Property

'JAVA TO VB CONVERTER NOTE: The parameter slice was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function slice(ByVal slice_Conflict As Long, ByVal dimension As Integer) As INDArray Implements INDArray.slice
			Nd4j.Compressor.autoDecompress(Me)

			Dim slices As Long = size(dimension)
			If slice_Conflict >= slices Then
				Throw New System.ArgumentException("Illegal slice " & slice_Conflict)
			End If

			If jvmShapeInfo.rank = 0 Then
				If slice_Conflict = 0 Then
					Return createScalarForIndex(slice_Conflict, True)
				Else
					Throw New System.ArgumentException("Can't slice a 0-d NDArray")
				End If

			End If


			If slice_Conflict < 0 Then
				slice_Conflict += rank()
			End If
			Dim indexes(rank() - 1) As INDArrayIndex
			indexes(dimension) = NDArrayIndex.point(slice_Conflict)
			Dim i As Integer = 0
			Do While i < rank()
				If i <> dimension Then
					indexes(i) = NDArrayIndex.all()
				End If
				i += 1
			Loop
			Return get(indexes)

		End Function

		Public Overridable Function getScalar(ByVal indexes() As Integer) As INDArray Implements INDArray.getScalar
			If indexes.Length > rank() Then
				Throw New ND4JIllegalStateException("Indexes can't be longer then array rank")
			End If

			For i As Integer = 0 To indexes.Length - 1
				If indexes(i) < 0 Then
					indexes(i) += Me.size(i)
				End If
			Next i
			Dim idx As Long = Shape.getOffset(jvmShapeInfo.javaShapeInformation, indexes)
			Dim buffer As val = createBuffer(Me.data(), idx, 1)
			Dim shape As val = Nd4j.ShapeInfoProvider.createShapeInformation(New Long(){}, New Long(){},1, "c"c, Me.dataType(), False)
			Return createArrayFromShapeBuffer(buffer, shape)
		End Function

		Public Overridable Function getScalar(ParamArray ByVal indexes() As Long) As INDArray Implements INDArray.getScalar
			If indexes.Length > rank() Then
				Throw New ND4JIllegalStateException("Indexes can't be longer then array rank")
			End If

			For i As Integer = 0 To indexes.Length - 1
				If indexes(i) < 0 Then
					indexes(i) += Me.size(i)
				End If
			Next i

			Dim idx As Long = Shape.getOffset(jvmShapeInfo.javaShapeInformation, indexes)
			Dim buffer As val = createBuffer(Me.data(), idx, 1)
			Dim shape As val = Nd4j.ShapeInfoProvider.createShapeInformation(New Long(){}, New Long(){},1,"c"c, Me.dataType(), False)
			Return createArrayFromShapeBuffer(buffer, shape)
		End Function

		Public Overridable Function rdiv(ByVal n As Number) As INDArray Implements INDArray.rdiv
			'return dup().rdivi(n);
			Return rdivi(n, createUninitialized(Shape.pickPairwiseDataType(Me.dataType(), n), Me.shape(), Me.ordering()))
		End Function

		Public Overridable Function rdivi(ByVal n As Number) As INDArray Implements INDArray.rdivi
			Return rdivi(n, Me)
		End Function

		Public Overridable Function rsub(ByVal n As Number) As INDArray Implements INDArray.rsub
			validateNumericalArray("rsub", False)
			Return rsubi(n, createUninitialized(Shape.pickPairwiseDataType(Me.dataType(), n),Me.shape(), Me.ordering()))
		End Function

		Public Overridable Function rsubi(ByVal n As Number) As INDArray Implements INDArray.rsubi
			Return rsubi(n, Me)
		End Function

		Public Overridable Function div(ByVal n As Number) As INDArray Implements INDArray.div
			validateNumericalArray("div", False)
			Return divi(n, createUninitialized(Shape.pickPairwiseDataType(Me.dataType(), n),Me.shape(), Me.ordering()))
		End Function

		Public Overridable Function divi(ByVal n As Number) As INDArray Implements INDArray.divi
			Return divi(n, Me)
		End Function

		Public Overridable Function mul(ByVal n As Number) As INDArray Implements INDArray.mul
			validateNumericalArray("mul", False)
			Return muli(n, createUninitialized(Shape.pickPairwiseDataType(Me.dataType(), n), Me.shape(), Me.ordering()))
		End Function

		Public Overridable Function muli(ByVal n As Number) As INDArray Implements INDArray.muli
			Return muli(n, Me)
		End Function

		Public Overridable Function [sub](ByVal n As Number) As INDArray Implements INDArray.sub
			validateNumericalArray("sub", False)
			Return subi(n, createUninitialized(Me.dataType(), Me.shape(), Me.ordering()))
		End Function

		Public Overridable Function subi(ByVal n As Number) As INDArray Implements INDArray.subi
			Return subi(n, Me)
		End Function

		Public Overridable Function add(ByVal n As Number) As INDArray Implements INDArray.add
			validateNumericalArray("add", False)
			Return addi(n, createUninitialized(Shape.pickPairwiseDataType(Me.dataType(), n),Me.shape(), Me.ordering()))
		End Function

		Public Overridable Function addi(ByVal n As Number) As INDArray Implements INDArray.addi
			Return addi(n, Me)
		End Function

		Public Overridable Function repmat(ByVal shape() As Long) As INDArray Implements INDArray.repmat
			Nd4j.Compressor.autoDecompress(Me)
			Dim rows As Long = Me.rows() * shape(0)
			Dim cols As Long = columns() * shape(1)
			Dim ret As INDArray = reshape(ChrW(1), length()).repeat(0, shape(0)).reshape(ChrW(rows), columns()).repeat(0, shape(1))
			Return ret.reshape(ChrW(rows), cols)
		End Function

		<Obsolete>
		Public Overridable Function repmat(ByVal shape() As Integer) As INDArray Implements INDArray.repmat
			Dim longShape() As Long = ArrayUtil.toLongArray(shape)
			Return repmat(longShape)
		End Function

		Public Overridable Function repeat(ByVal dimension As Integer, ParamArray ByVal repeats() As Long) As INDArray Implements INDArray.repeat
			Nd4j.Compressor.autoDecompress(Me)
			Dim op As CustomOp = DynamicCustomOp.builder("repeat").addInputs(Me).addIntegerArguments(ArrayUtil.toInts(repeats)).build()
			op.addIArgument(dimension) 'Native op: last iarg is dimension

			Dim l As LongShapeDescriptor = op.calculateOutputShape()(0)
			Dim [out] As INDArray = create(l)
			op.addOutputArgument([out])
			exec(op)
			Return [out]
		End Function

		Public Overridable Function putRow(ByVal row As Long, ByVal toPut As INDArray) As INDArray Implements INDArray.putRow
			If RowVector AndAlso toPut.Vector Then
				Return assign(toPut)
			End If
			Return put(New INDArrayIndex() {NDArrayIndex.point(row), NDArrayIndex.all()}, toPut)
		End Function

		Public Overridable Function putColumn(ByVal column As Integer, ByVal toPut As INDArray) As INDArray Implements INDArray.putColumn
			Nd4j.Compressor.autoDecompress(Me)

			If ColumnVector AndAlso toPut.Vector Then
				Return assign(toPut)
			End If
			Return put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.point(column)}, toPut)
		End Function

		Public Overridable Function getNumber(ByVal i As Long) As Number Implements INDArray.getNumber
			Select Case dataType()
				Case [DOUBLE], FLOAT, HALF, BFLOAT16
					Return getDouble(i)
				Case [LONG], INT, [SHORT], UBYTE, [BYTE], BOOL, UINT64, UINT32, UINT16
					Return getLong(i)
				Case Else
					Throw New System.NotSupportedException("Cannot get number from array of datatype: " & dataType())
			End Select
		End Function

		Public Overridable Function getNumber(ParamArray ByVal idx() As Long) As Number Implements INDArray.getNumber
			Select Case dataType()
				Case [DOUBLE], FLOAT, HALF
					Return getDouble(idx)
				Case [LONG], INT, [SHORT], UBYTE, [BYTE], BOOL
					Return getLong(idx)
				Case Else
					Throw New System.NotSupportedException("Cannot get number from array of datatype: " & dataType())
			End Select
		End Function

		Public Overridable Function getDouble(ByVal i As Long) As Double Implements INDArray.getDouble
			Nd4j.Compressor.autoDecompress(Me)
			Preconditions.checkState(Not Empty, "Unable to get value from empty array")

			If i >= length() Then
				Throw New System.ArgumentException("Unable to get linear index " & i & ": values is greater than length (" & length() & ")")
			End If

			autoProcessScalarCall()

			If i = 0 Then
				Return data().getDouble(i)
			End If

			Dim dimensions() As Long = If(ordering() = "c"c, Shape.ind2subC(Me, i), Shape.ind2sub(Me, i))
			Shape.assertShapeLessThan(dimensions, shape())
			Return getDouble(dimensions)

		End Function

		Public Overridable Function getDouble(ByVal i As Long, ByVal j As Long) As Double Implements INDArray.getDouble
			Return getDouble(New Long() {i, j})
		End Function

		Public Overridable Function getFloat(ByVal i As Long) As Single Implements INDArray.getFloat
			Return CSng(getDouble(i))
		End Function

		Public Overridable Function getFloat(ByVal i As Long, ByVal j As Long) As Single Implements INDArray.getFloat
			Return CSng(getDouble(i, j))
		End Function

		Public Overridable Function transpose() As INDArray Implements INDArray.transpose
			Preconditions.checkState(rank() >= 2, "Can't transpose array with rank < 2: array shape %ndShape", Me)

			Return permute(ArrayUtil.reverseCopy(ArrayUtil.range(0, rank())))
		End Function

		''' 
		''' <summary>
		''' Return transposed version of this matrix.
		''' 
		''' PLEASE NOTE: This method is NOT in place, it will return transposed copy instead.
		''' </summary>
		Public Overridable Function transposei() As INDArray Implements INDArray.transposei
			Preconditions.checkState(rank() >= 2, "Can't transpose array with rank < 2: array shape %ndShape", Me)

			Return permutei(ArrayUtil.reverseCopy(ArrayUtil.range(0, rank())))
		End Function

		Protected Friend Overridable Function create(ByVal data As DataBuffer, ByVal shape() As Integer, ByVal strides() As Integer) As INDArray
			Return create(data, shape, strides, 0, ordering())
		End Function

		<Obsolete>
		Public Overridable Function reshape(ByVal order As Char, ParamArray ByVal newShape() As Integer) As INDArray Implements INDArray.reshape
			Return reshape(order, ArrayUtil.toLongArray(newShape))
		End Function

		Public Overridable Function reshape(ByVal order As Char, ParamArray ByVal newShape() As Long) As INDArray Implements INDArray.reshape
			Return reshape(order, False, newShape)
		End Function

		Public Overridable Function reshape(ByVal order As Char, ByVal enforceView As Boolean, ParamArray ByVal newShape() As Long) As INDArray Implements INDArray.reshape
			Nd4j.Compressor.autoDecompress(Me)

			' special case for empty reshape
			If Me.length() = 1 AndAlso (newShape Is Nothing OrElse newShape.Length = 0) AndAlso Me.elementWiseStride() = 1 Then
				Return create(Me.data(), New Integer(){}, New Integer(){}, 0)
			End If

			If newShape Is Nothing OrElse newShape.Length < 1 Then
				Throw New ND4JIllegalStateException("Can't reshape(long...) without shape arguments. Got empty shape instead.")
			End If

			' TODO: maybe toFlatten() makes more sense here?
			' reshape(-1) special case
			If newShape.Length = 1 AndAlso newShape(0) = -1 Then
				newShape(0) = Me.length()
			End If

			Dim numberNegativesOnes As Integer = 0
			Dim shape() As Long = ArrayUtil.copy(newShape)


			Dim i As Integer = 0
			Do While i < shape.Length
				If shape(i) < 0 Then
					If numberNegativesOnes >= 1 Then
						Throw New System.ArgumentException("Only one dimension can be negative ones. Got shape " & java.util.Arrays.toString(newShape))
					End If

					numberNegativesOnes += 1

					Dim shapeLength As Integer = 1
					For j As Integer = 0 To shape.Length - 1
						If shape(j) >= 1 Then
							shapeLength *= shape(j)
						End If
					Next j
					Dim realShape As Long = Math.Abs(length() \ shapeLength)
					Dim thisNewShape(shape.Length - 1) As Long
					For j As Integer = 0 To shape.Length - 1
						If i <> j Then
							thisNewShape(j) = shape(j)
						Else
							thisNewShape(j) = realShape
						End If
					Next j

					shape = thisNewShape
					Exit Do

				End If
				i += 1
			Loop

			Dim prod As Long = ArrayUtil.prodLong(shape)

			If prod <> Me.length() Then
				Throw New ND4JIllegalStateException("New shape length doesn't match original length: [" & prod & "] vs [" & Me.length() & "]. Original shape: " & java.util.Arrays.toString(Me.shape()) & " New Shape: " & java.util.Arrays.toString(newShape))
			End If





			Dim reshapeAttempt As INDArray = Shape.newShapeNoCopy(Me, shape, order = "f"c)
			If reshapeAttempt IsNot Nothing Then
				' kinda strange get/set usage
				'  reshapeAttempt.setOrder(Shape.getOrder(reshapeAttempt));
				Return reshapeAttempt
			End If

			If enforceView Then
				Throw New ND4JIllegalStateException("Unable to reshape array as view, called with enforceView=true. " & "Use enforceView=false to return a copy instead, or call reshape on a non-strided array. Array shape info: " & Me.shapeInfoToString().replaceAll(vbLf,""))
			End If


			If order <> ordering() Then
				Dim ret As INDArray = createUninitialized(Me.dataType(), shape, order)
				ret.Data = dup(order).data()
				Return ret
			ElseIf Me.Empty Then
				Return create(Me.dataType(), shape)
			Else
				Dim ret As INDArray = Me.dup(order)
				Return create(ret.data(), shape)
			End If
		End Function

		Public Overridable Function getDoubleUnsafe(ByVal offset As Long) As Double Implements INDArray.getDoubleUnsafe
			Return data().getDouble(offset)
		End Function

		Public Overridable Function putScalarUnsafe(ByVal offset As Long, ByVal value As Double) As INDArray Implements INDArray.putScalarUnsafe
			autoProcessScalarCall()
			data().put(offset, value)
			Return Me
		End Function

		Public Overridable Function reshape(ByVal order As Char, ByVal rows As Integer, ByVal columns As Integer) As INDArray Implements INDArray.reshape
			Return reshape(order, New Long() {rows, columns})
		End Function

		''' <summary>
		''' Reshape the ndarray in to the specified dimensions,
		''' possible errors being thrown for invalid shapes
		''' 
		''' Note here that one dimension can be -1.
		''' The dimension that is -1 will be inferred from the shape and
		''' the length of the ndarray
		''' </summary>
		''' <param name="shape"> the shape of the ndarray. </param>
		''' <returns> the new reshaped nd array </returns>

		Public Overridable Function reshape(ByVal shape() As Integer) As INDArray Implements INDArray.reshape
			Return reshape(order(), shape)
		End Function

		Public Overridable Function reshape(ParamArray ByVal shape() As Long) As INDArray Implements INDArray.reshape
			Return reshape(order(), shape)
		End Function

		Public Overridable Function prod(ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.prod
			validateNumericalArray("prod", False)
			Return Nd4j.Executioner.exec(New Prod(Me, keepDims, dimension))
		End Function

		Public Overridable Function prod(ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.prod
			Return prod(False, dimension)
		End Function

		Public Overridable Function mean(ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.mean
			validateNumericalArray("mean", False)
			Return Nd4j.Executioner.exec(New Mean(Me, keepDims, dimension))
		End Function

		Public Overridable Function mean(ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.mean
			Return mean(False, dimension)
		End Function

		Public Overridable Function amean(ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.amean
			validateNumericalArray("amean", False)
			Return Nd4j.Executioner.exec(New AMean(Me, dimension))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public INDArray mean(@NonNull INDArray result, boolean keepDims, int... dimension)
		Public Overridable Function mean(ByVal result As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.mean
			validateNumericalArray("mean", False)
			Return Nd4j.Executioner.exec(New Mean(Me, result, keepDims, dimension))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public INDArray mean(@NonNull INDArray result, int... dimension)
		Public Overridable Function mean(ByVal result As INDArray, ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.mean
			Return mean(result, False, dimension)
		End Function

		Public Overridable Function var(ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.var
			validateNumericalArray("var", False)
			Return Nd4j.Executioner.exec(New Variance(Me, dimension))
		End Function

		Public Overridable Function var(ByVal biasCorrected As Boolean, ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.var
			validateNumericalArray("var", False)
			Return Nd4j.Executioner.exec(New Variance(Me, biasCorrected, dimension))
		End Function

		Public Overridable Function max(ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.max
			validateNumericalArray("max", False)
			Return Nd4j.Executioner.exec(New Max(Me, keepDims, dimension))
		End Function

		Public Overridable Function max(ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.max
			Return max(False, dimension)
		End Function

		Public Overridable Function amax(ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.amax
			validateNumericalArray("amax", False)
			Return Nd4j.Executioner.exec(New AMax(Me, dimension))
		End Function

		Public Overridable Function min(ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.min
			validateNumericalArray("min", False)
			Return Nd4j.Executioner.exec(New Min(Me, keepDims, dimension))
		End Function

		Public Overridable Function min(ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.min
			Return min(False, dimension)
		End Function

		Public Overridable Function amin(ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.amin
			validateNumericalArray("amin", False)
			Return Nd4j.Executioner.exec(New AMin(Me, dimension))
		End Function

		Public Overridable Function sum(ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.sum
			validateNumericalArray("sum", True)
			Return Nd4j.Executioner.exec(New Sum(Me, dimension))
		End Function

		Public Overridable Function sum(ByVal keepDim As Boolean, ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.sum
			validateNumericalArray("sum", True)
			Return Nd4j.Executioner.exec(New Sum(Me, Nothing, keepDim, dimension))
		End Function

		Public Overridable Function entropy(ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.entropy
			validateNumericalArray("entropy", False)
			Return Nd4j.Executioner.exec(New Entropy(Me, dimension))
		End Function

		Public Overridable Function shannonEntropy(ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.shannonEntropy
			validateNumericalArray("shannonEntropy", False)
			Return Nd4j.Executioner.exec(New ShannonEntropy(Me, dimension))
		End Function

		Public Overridable Function logEntropy(ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.logEntropy
			validateNumericalArray("logEntropy", False)
			Return Nd4j.Executioner.exec(New LogEntropy(Me, dimension))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public INDArray sum(@NonNull INDArray result, boolean keepDims, int... dimension)
		Public Overridable Function sum(ByVal result As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.sum
			validateNumericalArray("sum", True)
			Return Nd4j.Executioner.exec(New Sum(Me, result, keepDims, dimension))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public INDArray sum(@NonNull INDArray result, int... dimension)
		Public Overridable Function sum(ByVal result As INDArray, ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.sum
			Return sum(result, False, dimension)
		End Function

		Public Overridable Function norm1(ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.norm1
			Return norm1(False, dimension)
		End Function

		Public Overridable Function norm1(ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.norm1
			validateNumericalArray("norm1", False)
			Return Nd4j.Executioner.exec(New Norm1(Me, keepDims, dimension))
		End Function

		Public Overridable Function std(ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.std
			Return std(True, dimension)
		End Function

		Public Overridable Function std(ByVal biasCorrected As Boolean, ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.std
			Return std(biasCorrected, False, dimension)
		End Function

		Public Overridable Function std(ByVal biasCorrected As Boolean, ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.std
			validateNumericalArray("std", False)
			Return Nd4j.Executioner.exec(New StandardDeviation(Me, biasCorrected, keepDims, dimension))
		End Function

		Public Overridable Function stdNumber(ByVal biasCorrected As Boolean) As Number Implements INDArray.stdNumber
			validateNumericalArray("stdNumber", False)
			Return Nd4j.Executioner.exec(New StandardDeviation(Me, biasCorrected)).getDouble(0)
		End Function

		Public Overridable Function norm2(ByVal keepDims As Boolean, ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.norm2
			validateNumericalArray("norm2", False)
			Return Nd4j.Executioner.exec(New Norm2(Me, keepDims, dimension))
		End Function

		Public Overridable Function norm2(ParamArray ByVal dimension() As Integer) As INDArray Implements INDArray.norm2
			Return norm2(False, dimension)
		End Function

		Public Overridable Function columns() As Integer Implements INDArray.columns
			If Matrix Then
				Return CInt(size(1))
			ElseIf Shape.isColumnVectorShape(shape()) Then
				Return 1
			ElseIf Shape.isRowVectorShape(shape()) Then
				Return CInt(length())
			End If
			Throw New System.InvalidOperationException("Rank is [" & rank() & "]; columns() call is not valid")


		End Function

		Public Overridable Function rows() As Integer Implements INDArray.rows
			If Matrix Then
				Return CInt(size(0))
			ElseIf Shape.isRowVectorShape(shape()) Then
				Return 1
			ElseIf Shape.isColumnVectorShape(shape()) Then
				Return CInt(length())
			End If

			Throw New System.InvalidOperationException("Rank is " & rank() & " rows() call is not valid")
		End Function

		Public Overridable Function ravel(ByVal ordering As Char) As INDArray Implements INDArray.ravel
			Nd4j.Compressor.autoDecompress(Me)
			If ordering = Me.ordering() AndAlso Shape.hasDefaultStridesForShape(Me) Then
				Return reshape(ordering, length())
			End If
			Return dup(ordering).reshape(ordering, length())
		End Function

		Public Overridable Function ravel() As INDArray Implements INDArray.ravel
			Return reshape(ChrW(length()))
		End Function

		Public Overridable Sub sliceVectors(ByVal list As IList(Of INDArray))
			If ArrayList Then
				list.Add(Me)
			Else
				Dim i As Integer = 0
				Do While i < slices()
					slice(i).sliceVectors(list)
					i += 1
				Loop
			End If
		End Sub

		Public Overridable Function reshape(ByVal newRows As Long, ByVal newColumns As Long) As INDArray Implements INDArray.reshape
			Return reshape(New Long() {newRows, newColumns})
		End Function

		Public Overridable Function getColumn(ByVal c As Long) As INDArray Implements INDArray.getColumn
			Nd4j.Compressor.autoDecompress(Me)

			If ColumnVector AndAlso c = 0 Then
				Return Me
			ElseIf ColumnVector AndAlso c > 0 Then
				Throw New System.ArgumentException("Illegal index for column")
			End If
			Preconditions.checkArgument(Me.rank() = 2, "getColumn() can be called on 2D arrays only")
			Return tensorAlongDimension(c, 0)
		End Function

		Public Overridable Function getColumn(ByVal c As Long, ByVal keepDim As Boolean) As INDArray Implements INDArray.getColumn
			Dim col As INDArray = getColumn(c)
			If Not keepDim Then
				Return col
			End If
			Return col.reshape(ChrW(col.length()), 1)
		End Function

		Public Overridable Function getRows(ByVal rindices() As Integer) As INDArray Implements INDArray.getRows
			Nd4j.Compressor.autoDecompress(Me)

			If Not Matrix AndAlso Not ArrayList Then
				Throw New System.ArgumentException("Unable to get columns from a non matrix or vector")
			End If
			If ArrayList Then
				Return pullRows(Me, 1, rindices)
			Else
				Dim ret As INDArray = createUninitialized(Me.dataType(), New Long() {rindices.Length, columns()})
				For i As Integer = 0 To rindices.Length - 1
					ret.putRow(i, getRow(rindices(i)))
				Next i
				Return ret
			End If
		End Function

		Public Overridable Function get(ParamArray ByVal indexes() As INDArrayIndex) As INDArray
			Nd4j.Compressor.autoDecompress(Me)

			Dim numPoint As Integer = 0
			Dim numInterval As Integer = 0
			Dim numAll As Integer = 0
			Dim numNewAxis As Integer = 0
			Dim numSpecified As Integer = 0
			For Each i As INDArrayIndex In indexes
				If TypeOf i Is PointIndex Then
					numPoint += 1
				ElseIf TypeOf i Is NDArrayIndexAll Then
					numAll += 1
				ElseIf TypeOf i Is IntervalIndex Then
					numInterval += 1
				ElseIf TypeOf i Is NewAxis Then
					numNewAxis += 1
				ElseIf TypeOf i Is SpecifiedIndex Then
					numSpecified += 1
				Else
					Throw New System.InvalidOperationException("Unknown index: " & i)
				End If
			Next i

			' Padding remaining dimensions with all() index if too few indices provided
			If indexes.Length - numNewAxis < Me.rank() Then
				Dim newIndexes As val = New INDArrayIndex((Me.rank() + numNewAxis) - 1){}
				For e As Integer = 0 To indexes.Length - 1
					newIndexes(e) = indexes(e)
				Next e

				For e As Integer = indexes.Length To newIndexes.length - 1
					numAll += 1
					newIndexes(e) = NDArrayIndex.all()
				Next e

				indexes = newIndexes
			End If

			Preconditions.checkState((numPoint + numInterval + numAll + numSpecified) = rank(), "Illegal set of indices for array: need at least" & " %s point/interval/all/specified indices for rank %s array (%ndShape), got indices %s", rank(), rank(), Me, indexes)

			Dim outRank As Integer = rank() + numNewAxis - numPoint
			Preconditions.checkState(outRank >= 0, "Illegal set of indices for array: %ndShape, %s", Me, indexes)


			'To work out sub-array, we need to work out 3 things: offset, shape and strides. We calculate all of these
			Dim outShape(outRank - 1) As Long
			Dim outStrides(outRank - 1) As Long
			Dim offset As Long = Me.offset() 'Start with existing offset if view

			Dim outIdx As Integer = 0 'Axis number counter for output array
			Dim inIdx As Integer = 0 'Axis number counter for input array
			For i As Integer = 0 To indexes.Length - 1
				If TypeOf indexes(i) Is PointIndex Then
					'Point indexes don't appear in output
					Dim pi As PointIndex = DirectCast(indexes(i), PointIndex)
					offset += pi.offset() * stride(inIdx)
					inIdx += 1
				ElseIf TypeOf indexes(i) Is NDArrayIndexAll Then
					'All index: doesn't change offset. Axis is in both in and output arrays
					outShape(outIdx) = size(inIdx)
					outStrides(outIdx) = stride(inIdx)
					inIdx += 1
					outIdx += 1
				ElseIf TypeOf indexes(i) Is IntervalIndex Then
					'Interval index: Axis is in both in and output arrays, but output might be smaller
					Dim ii As IntervalIndex = DirectCast(indexes(i), IntervalIndex)
					Dim start As Long = ii.offset()
					Dim endInc As Long = ii.end() - (If(ii.isInclusive(), 0, 1))
					If endInc >= size(inIdx) Then
						Throw New System.InvalidOperationException("Indices are out of range: Cannot get interval index " & indexes(i) & " on array with size(" & inIdx & ")=" & size(inIdx) & ". Array shape: " & java.util.Arrays.toString(shape()) & ", indices: " & java.util.Arrays.toString(indexes))
					End If
					Dim stride As Long = ii.stride()
					Dim length As Long = (endInc - start)\stride + 1

					offset += ii.offset() * Me.stride(inIdx)
					outShape(outIdx) = length
					outStrides(outIdx) = ii.stride() * Me.stride(inIdx)
					inIdx += 1
					outIdx += 1
				ElseIf TypeOf indexes(i) Is NewAxis Then
					'New axis: appends a 1 in shape. Axis not present in input, but is present in output
					outShape(outIdx) = 1
					If outIdx > 0 Then 'Stride doesn't matter for 1 size axis anyway...
						outStrides(outIdx) = outStrides(outIdx - 1)
					Else
						outStrides(outIdx) = 1
					End If
					outIdx += 1
				ElseIf TypeOf indexes(i) Is SpecifiedIndex Then
					'Specified index: axis present in both input and output
					Dim si As SpecifiedIndex = DirectCast(indexes(i), SpecifiedIndex)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: outShape[outIdx++] = si.length();
					outShape(outIdx) = si.length()
						outIdx += 1
					inIdx += 1
					'Don't care about strides for specified index, as result won't be a view
				Else
					Throw New System.InvalidOperationException("Unknown index type: " & i) 'Should never happen
				End If
			Next i


			'Note: If we have specified indices, we can't return a view. Instead, we copy the specified sub-arrays from
			' the input array to the output array.
			'How? Create the output array, then do loop over the specified indices only, and copy sub-arrays for all other axes
			If numSpecified > 0 Then
				Dim [out] As INDArray = create(dataType(), outShape)

				'Need to copy subsets here
				Dim specifiedSizes(numSpecified - 1) As Long
				Dim si(numSpecified - 1) As SpecifiedIndex
				Dim j As Integer=0
				For i As Integer = 0 To indexes.Length - 1
					If TypeOf indexes(i) Is SpecifiedIndex Then
						specifiedSizes(j) = indexes(i).length()
						si(j) = DirectCast(indexes(i), SpecifiedIndex)
						j += 1
					End If
				Next i
				Dim iter As New NdIndexIterator(specifiedSizes)

				'What we need to do here: Iterate over sub-arrays for both input and output
				'(1) Get from input: requested indices, except for:
				'    i. specified indices -> replace with loop + point
				'    ii. new axis indices -> ignore/exclude (don't appear in input)
				'    iii. interval indices -> replace with all
				'(2) Get from output: requested indices, except for:
				'    i. point indices -> ignore/exclude (don't appear in output)
				'    ii. new axis indices -> replace with point(0)


				Dim pointIdxsIn((indexes.Length - numNewAxis) - 1) As INDArrayIndex 'Indices for source (this array)
				Dim specifiedAxisIn(numSpecified - 1) As Integer
				Dim specCount As Integer = 0
				j = 0
				For i As Integer = 0 To indexes.Length - 1
					If TypeOf indexes(i) Is NewAxis Then
						Continue For 'Skip new axis in source dims
					End If
					If TypeOf indexes(i) Is SpecifiedIndex Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: specifiedAxisIn[specCount++] = j;
						specifiedAxisIn(specCount) = j
							specCount += 1
					End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: pointIdxsIn[j++] = indexes[i];
					pointIdxsIn(j) = indexes(i)
						j += 1
				Next i

				Dim pointIdxsOut((indexes.Length-numPoint) - 1) As INDArrayIndex 'Indices for destination (output array)
				j = 0
				specCount = 0
				Dim specifiedAxisOut(numSpecified - 1) As Integer
				For i As Integer = 0 To indexes.Length - 1
					If TypeOf indexes(i) Is NewAxis Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: pointIdxsOut[j++] = NDArrayIndex.point(0);
						pointIdxsOut(j) = NDArrayIndex.point(0)
							j += 1
						Continue For
					ElseIf TypeOf indexes(i) Is PointIndex Then
						Continue For
					ElseIf TypeOf indexes(i) Is SpecifiedIndex Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: specifiedAxisOut[specCount++] = j;
						specifiedAxisOut(specCount) = j
							specCount += 1
					ElseIf TypeOf indexes(i) Is IntervalIndex Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: pointIdxsOut[j++] = NDArrayIndex.all();
						pointIdxsOut(j) = NDArrayIndex.all()
							j += 1
						Continue For
					End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: pointIdxsOut[j++] = indexes[i];
					pointIdxsOut(j) = indexes(i)
						j += 1
				Next i


				'Iterate over sub-arrays; copy from source to destination
				Do While iter.MoveNext()
					Dim specifiedIdxs() As Long = iter.Current
					For i As Integer = 0 To specifiedIdxs.Length - 1
						Dim sourceIdx As Long = si(i).getIndexes()(CInt(specifiedIdxs(i)))
						pointIdxsIn(specifiedAxisIn(i)) = NDArrayIndex.point(sourceIdx)
						Dim outI As Integer = CInt(specifiedIdxs(i))
						pointIdxsOut(specifiedAxisOut(i)) = NDArrayIndex.point(outI)
					Next i

					[out].get(pointIdxsOut).assign(get(pointIdxsIn))
				Loop

				Return [out]
			End If


			Dim order As Char = Shape.getOrder(outShape, outStrides, -1)
			Dim [out] As INDArray = create(data_Conflict, outShape, outStrides, offset, order)
			Return [out]
		End Function

		Public Overridable Function getColumns(ParamArray ByVal cindices() As Integer) As INDArray Implements INDArray.getColumns
			If Not Matrix AndAlso Not ArrayList Then
				Throw New System.ArgumentException("Unable to get columns from a non matrix or vector")
			End If
			If ArrayList Then
				Return pullRows(Me, 0, cindices, Me.ordering())
			Else
				Dim ret As INDArray = createUninitialized(Me.dataType(), New Long(){rows(), cindices.Length})
				For i As Integer = 0 To cindices.Length - 1
					ret.putColumn(i, getColumn(cindices(i)))
				Next i
				Return ret
			End If

		End Function

		Protected Friend Overridable Function create(ByVal rows As Integer, ByVal length As Integer) As INDArray
			Return create(New Integer() {rows, length})
		End Function

		Public Overridable Function getRow(ByVal r As Long) As INDArray Implements INDArray.getRow
			If RowVector AndAlso r = 0 Then
				Return Me
			ElseIf RowVector AndAlso r > 0 Then
				Throw New System.ArgumentException("Illegal index for row: requested row " & r & " but this.size(0)=" & Me.size(0))
			End If

			Preconditions.checkArgument(rank() = 2, "getRow() can be called on 2D arrays only")
			Preconditions.checkArgument(r < rows(), "Row index must be smaller than total number of rows")

			Return tensorAlongDimension(r, 1)
		End Function

		Public Overridable Function getRow(ByVal r As Long, ByVal keepDim As Boolean) As INDArray Implements INDArray.getRow
			Dim row As INDArray = getRow(r)
			If Not keepDim Then
				Return row
			End If
			Return row.reshape(ChrW(1), row.length())
		End Function

		Public Overridable Function equalsWithEps(ByVal o As Object, ByVal eps As Double) As Boolean Implements INDArray.equalsWithEps
			Nd4j.Compressor.autoDecompress(Me)


			If o Is Nothing Then
				Return False
			End If

			If Not (TypeOf o Is INDArray) Then
				Return False
			End If

			Dim n As INDArray = DirectCast(o, INDArray)
			Nd4j.Compressor.autoDecompress(n)

			If n Is Me Then
				Return True
			End If

			If Me.rank() <> n.rank() Then
				Return False
			End If

			If Me.length() <> n.length() Then
				Return False
			End If

			If Me.Empty <> n.Empty Then
				Return False
			End If

			If Me.Empty AndAlso n.Empty Then
				Return Shape.shapeEquals(Me.shape(), n.shape())
			End If

			If Me.dataType() <> n.dataType() Then
				Return False
			End If

			' meh
			If Me.dataType() = DataType.UTF8 AndAlso n.dataType() = DataType.UTF8 Then
				For e As Long = 0 To Me.length() - 1
					Dim str1 As val = Me.getString(e)
					Dim str2 As val = n.getString(e)

					If Not str1.Equals(str2) Then
						Return False
					End If
				Next e

				Return True
			End If

			'epsilon equals
			If Scalar AndAlso n.Scalar Then
				If Z Then
					Dim val As val = getLong(0)
					Dim val2 As val = n.getLong(0)

					Return val = val2
				ElseIf R Then
					Dim val As val = getDouble(0)
					Dim val2 As val = n.getDouble(0)

					If Double.IsNaN(val) <> Double.IsNaN(val2) Then
						Return False
					End If

					Return Math.Abs(val - val2) < eps
				ElseIf B Then
					Dim val As val = getInt(0)
					Dim val2 As val = n.getInt(0)

					Return val = val2
				End If

			ElseIf ArrayList AndAlso n.Vector Then
				Dim op As val = New EqualsWithEps(Me, n, eps)
				exec(op)
				Dim diff As val = op.z().getDouble(0)

				Return diff < 0.5
			End If

			If Not Me.shape().SequenceEqual(n.shape()) Then
				Return False
			End If


			If Not Shape.shapeEquals(shape(), n.shape()) Then
				Return False
			End If


			If slices() <> n.slices() Then
				Return False
			End If

			If n.ordering() = ordering() Then
				Dim op As New EqualsWithEps(Me, n, eps)
				Nd4j.Executioner.exec(op)
				Dim diff As Double = op.z().getDouble(0)

				Return diff < 0.5
			Else
				Dim op As New EqualsWithEps(Me, n, eps)
				Nd4j.Executioner.exec(op)
				Dim diff As Double = op.z().getDouble(0)

				Return diff < 0.5
			End If
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public boolean equalShapes(@NonNull INDArray other)
		Public Overridable Function equalShapes(ByVal other As INDArray) As Boolean Implements INDArray.equalShapes
			If Empty <> other.Empty Then
				Return False
			End If
			If rank() <> other.rank() Then
				Return False
			End If
			Dim i As Integer=0
			Do While i<rank()
				If size(i) <> other.size(i) Then
					Return False
				End If
				i += 1
			Loop
			Return True
		End Function

		''' <summary>
		''' Compare two matrices. Returns true if and only if other is also a
		''' DoubleMatrix which has the same size and the maximal absolute
		''' difference in matrix elements is smaller than 1e-5.
		''' </summary>
		''' <param name="o"> </param>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			Return equalsWithEps(o, EPS_THRESHOLD)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim longHash As val = exec(New HashCode(Me))(0).getLong(0)
			Return If(Math.Abs(longHash) <= Integer.MaxValue, CInt(longHash), CInt(longHash Mod Integer.MaxValue))
		End Function

		Public Overridable Function shapeInfoDataBuffer() As DataBuffer
			Return shapeInformation_Conflict
		End Function

		Public Overridable Function shapeInfo() As LongBuffer Implements INDArray.shapeInfo
			Return shapeInformation_Conflict.asNioLong()
		End Function

		Public Overridable Function shape() As Long() Implements INDArray.shape
			Return jvmShapeInfo.shape
		End Function

		Public Overridable Function shapeInfoToString() As String Implements INDArray.shapeInfoToString
			Return Shape.shapeToString(Me)
		End Function

		Public Overridable Function stride() As Long() Implements INDArray.stride
			Return jvmShapeInfo.stride
		End Function


		Public Overridable Function offset() As Long Implements INDArray.offset
			Return data().offset()
		End Function

		Public Overridable Function ordering() As Char Implements INDArray.ordering
			Return jvmShapeInfo.order
		End Function

		Public Overridable Function size(ByVal dimension As Integer) As Long Implements INDArray.size
			If dimension < 0 Then
				dimension += jvmShapeInfo.rank
			End If

			If Scalar Then
				If dimension = 0 OrElse dimension = 1 OrElse dimension < 0 Then
					Return length()
				Else
					Throw New System.ArgumentException("Illegal dimension for scalar " & dimension)
				End If
			End If

			If dimension >= rank() Then
				Throw New System.ArgumentException("Invalid size: cannot get size of dimension " & dimension & " for rank " & rank() & " NDArray (array shape: " & java.util.Arrays.toString(Me.shape()) & ")")
			End If

			Return jvmShapeInfo.shape(dimension)
		End Function

		Public Overridable Function rank() As Integer Implements INDArray.rank
			Return jvmShapeInfo.rank
		End Function

		Public Overridable Function length() As Long Implements INDArray.length
			If Empty Then
				Return 0
			End If
			Return jvmShapeInfo.length
		End Function

		Public Overridable Function broadcast(ByVal result As INDArray) As INDArray Implements INDArray.broadcast
			Nd4j.Compressor.autoDecompress(Me)

			Dim shape As val = result.shape()

			If Shape.shapeEquals(shape, Me.shape()) Then
				Return Me
			End If

			' if we're on scalar, we can just create new array
			If Me.Scalar Then
				Return createUninitialized(Me.dataType(), shape).assign(Me.getDouble(0))
			End If




			Dim compatible As Boolean = True
			Dim count As Integer = shape.length - 1
			Dim thisCount As Integer = jvmShapeInfo.rank - 1
			For i As Integer = shape.length - 1 To 1 Step -1
				If count < 0 OrElse thisCount < 0 Then
					Exit For
				End If
				If shape(count) <> Me.shape()(thisCount) AndAlso shape(count) <> 1 AndAlso Me.shape()(thisCount) <> 1 Then
					compatible = False
					Exit For
				End If

				count -= 1
				thisCount -= 1
			Next i

			If Not compatible Then
				Throw New System.ArgumentException("Incompatible broadcast from " & java.util.Arrays.toString(Me.shape()) & " to " & java.util.Arrays.toString(shape))
			End If



			Dim retShape(shape.length - 1) As Long
			Dim broadCastDimensions As IList(Of Integer) = New List(Of Integer)()
			Dim nonBroadCastDimensions As IList(Of Integer) = New List(Of Integer)()
			For i As Integer = 0 To retShape.Length - 1
				If Me.shape().Length = 1 Then
					If i = 0 Then
						If i < Me.shape().Length Then
							retShape(i) = Math.Max(1, shape(i))
						Else
							retShape(i) = shape(i)
						End If
					Else
						If i < Me.shape().Length Then
							retShape(i) = Math.Max(shape(i), size(i))
						Else
							retShape(i) = shape(i)
						End If
					End If
				Else
					If i < rank() AndAlso size(i) = 1 Then
						broadCastDimensions.Add(i)
					Else
						nonBroadCastDimensions.Add(i)
					End If
					If i < Me.shape().Length Then
						retShape(i) = Math.Max(shape(i), size(i))
					Else
						retShape(i) = shape(i)
					End If
				End If

			Next i


			If RowVector Then
				'number of times to repeat each value
				Dim i As Integer = 0
				Do While i < result.slices()
					result.putSlice(i, Me)
					i += 1
				Loop
			ElseIf ColumnVector Then
				Dim i As Integer = 0
				Do While i < result.columns()
					result.putColumn(i, Me)
					i += 1
				Loop

			Else
				Dim repeat(shape.length - 1) As Integer
				For i As Integer = 0 To shape.length - 1
					If i < rank() Then
						If size(i) = 1 Then
							repeat(i) = CInt(Math.Truncate(shape(i)))
						Else
							repeat(i) = 1
						End If

					Else
						repeat(i) = CInt(Math.Truncate(shape(i)))
					End If
				Next i

				If Me.View Then
					Nd4j.Executioner.execAndReturn(New Tile(New INDArray(){Me.dup(Me.ordering())},New INDArray(){result},repeat))
				Else
					Nd4j.Executioner.execAndReturn(New Tile(New INDArray(){Me},New INDArray(){result},repeat))
				End If
			End If
			Return result

		End Function

		Public Overridable Function broadcast(ParamArray ByVal shape() As Long) As INDArray Implements INDArray.broadcast
		  Return broadcast(createUninitialized(Me.dataType(), shape, Me.ordering()))
		End Function

		<Obsolete>
		Public Overridable Function dimShuffle(ByVal rearrange() As Object, ByVal newOrder() As Integer, ByVal broadCastable() As Boolean) As INDArray Implements INDArray.dimShuffle
			Return dimShuffle(rearrange, ArrayUtil.toLongArray(newOrder), broadCastable)
		End Function

		''' <summary>
		''' Dimshuffle: an extension of permute that adds the ability
		''' to broadcast various dimensions.
		''' <p/>
		''' See theano for more examples.
		''' This will only accept integers and xs.
		''' <p/>
		''' An x indicates a dimension should be broadcasted rather than permuted.
		''' </summary>
		''' <param name="rearrange"> the dimensions to swap to </param>
		''' <returns> the newly permuted array </returns>
		Public Overridable Function dimShuffle(ByVal rearrange() As Object, ByVal newOrder() As Long, ByVal broadCastable() As Boolean) As INDArray Implements INDArray.dimShuffle
			Nd4j.Compressor.autoDecompress(Me)

			If broadCastable.Length <> jvmShapeInfo.rank Then
				Throw New System.ArgumentException("The broadcastable dimensions must be the same length as the current shape")
			End If

'JAVA TO VB CONVERTER NOTE: The variable broadcast was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim broadcast_Conflict As Boolean = False
			Dim set As [Set](Of Object) = New HashSet(Of Object)()
			For i As Integer = 0 To rearrange.Length - 1
				set.add(rearrange(i))
				If TypeOf rearrange(i) Is Integer? Then
					Dim j As Integer? = DirectCast(rearrange(i), Integer?)
					If j >= broadCastable.Length Then
						Throw New System.ArgumentException("Illegal dimension, dimension must be < broadcastable.length (aka the real dimensions")
					End If
				ElseIf TypeOf rearrange(i) Is Char? Then
					Dim c As Char? = DirectCast(rearrange(i), Char?)
					If c <> "x"c Then
						Throw New System.ArgumentException("Illegal input: Must be x")
					End If
					broadcast_Conflict = True

				Else
					Throw New System.ArgumentException("Only characters and integers allowed")
				End If
			Next i

			'just do permute
			If Not broadcast_Conflict Then
				Dim ret(rearrange.Length - 1) As Integer
				For i As Integer = 0 To ret.Length - 1
					ret(i) = DirectCast(rearrange(i), Integer?)
				Next i
				Return permute(ret)
			Else
				Dim drop As IList(Of Integer) = New List(Of Integer)()
				For i As Integer = 0 To broadCastable.Length - 1
					If Not set.contains(i) Then
						If broadCastable(i) Then
							drop.Add(i)
						Else
							Throw New System.ArgumentException("We can't drop the given dimension because its not broadcastable")
						End If
					End If

				Next i


				'list of dimensions to keep
				Dim shuffle(broadCastable.Length - 1) As Integer
				Dim count As Integer = 0
				For i As Integer = 0 To rearrange.Length - 1
					If TypeOf rearrange(i) Is Integer? Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: shuffle[count++] = (System.Nullable<Integer>) rearrange[i];
						shuffle(count) = DirectCast(rearrange(i), Integer?)
							count += 1
					End If
				Next i


				Dim augment As IList(Of Integer) = New List(Of Integer)()
				For i As Integer = 0 To rearrange.Length - 1
					If TypeOf rearrange(i) Is Char? Then
						augment.Add(i)
					End If
				Next i

				Dim augmentDims() As Integer? = CType(augment, List(Of Integer)).ToArray()

				count = 0

				Dim dropIdx As Integer = 0
				Dim newShape((shuffle.Length + drop.Count) - 1) As Integer
				For i As Integer = 0 To newShape.Length - 1
					If i < shuffle.Length Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: newShape[count++] = shuffle[i];
						newShape(count) = shuffle(i)
							count += 1
					Else
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: newShape[count++] = drop.get(dropIdx++);
						newShape(count) = drop(dropIdx)
							dropIdx += 1
							count += 1
					End If
				Next i

				Dim ret As INDArray 'TODO is this correct? This was old behaviour before adding permute input check
				If newShape.Length = Me.rank() Then
					ret = permute(newShape)
				Else
					ret = dup()
				End If
				Dim newDims As IList(Of Long) = New List(Of Long)()
				Dim shape() As Long = Arrays.CopyOfRange(ret.shape(), 0, shuffle.Length)
				For i As Integer = 0 To shape.Length - 1
					newDims.Add(shape(i))
				Next i

				For i As Integer = 0 To augmentDims.Length - 1
					newDims.Insert(augmentDims(i), 1L)
				Next i

				Dim toReshape() As Long = ArrayUtil.toArrayLong(newDims)


				ret = ret.reshape(toReshape)
				Return ret

			End If


		End Function

		Public Overridable Function permute(ParamArray ByVal rearrange() As Integer) As INDArray Implements INDArray.permute
			Preconditions.checkArgument(rearrange.Length = rank(), "Incorrect number of arguments for permute function:" & " got arguments %s for rank %s array. Number of arguments must equal array rank", rearrange, rank())
			Nd4j.Compressor.autoDecompress(Me)
			Dim alreadyInOrder As Boolean = True
			'IntBuffer shapeInfo = shapeInfo();
			Dim rank As Integer = jvmShapeInfo.rank
			For i As Integer = 0 To rank - 1
				If rearrange(i) <> i Then
					alreadyInOrder = False
					Exit For
				End If
			Next i

			If alreadyInOrder Then
				Return Me
			End If

			checkArrangeArray(rearrange)
			Dim newShape As val = doPermuteSwap(shape(), rearrange)
			Dim newStride As val = doPermuteSwap(stride(), rearrange)

			Dim newOrder As Char = Shape.getOrder(newShape, newStride, 1)

			Dim value As INDArray = create(data(), newShape, newStride, offset(), newOrder)
			Return value
		End Function

		Public Overridable Function permutei(ParamArray ByVal rearrange() As Integer) As INDArray Implements INDArray.permutei
			Preconditions.checkArgument(rearrange.Length = rank(), "Incorrect number of arguments for permute function:" & " got arguments %s for rank %s array. Number of arguments must equal array rank", rearrange, rank())
			Dim alreadyInOrder As Boolean = True
			Dim shapeInfo As val = Me.shapeInfo()
			Dim rank As Integer = jvmShapeInfo.rank
			For i As Integer = 0 To rank - 1
				If rearrange(i) <> i Then
					alreadyInOrder = False
					Exit For
				End If
			Next i

			If alreadyInOrder Then
				Return Me
			End If

			checkArrangeArray(rearrange)
			Dim newShape As val = doPermuteSwap(shape(), rearrange)
			Dim newStride As val = doPermuteSwap(stride(), rearrange)
			Dim newOrder As Char = Shape.getOrder(newShape, newStride, 1)

			Dim ews As val = shapeInfo.get(2 * rank + 2)

			Dim si As val = Nd4j.ShapeInfoProvider.createShapeInformation(newShape, newStride, ews, newOrder, dataType(), Empty)
			ShapeInformation = si


			If shapeInfo.get(2 * rank + 2) > 0 Then
				'for the backend to work - no ews for permutei
				'^^ not true anymore? Not sure here. Marking this for raver
				ShapeInformation = Nd4j.ShapeInfoProvider.createShapeInformation(newShape, newStride, 0, newOrder, dataType(), Empty)
			End If

			'this.shape = null;
			'this.stride = null;


			Return Me
		End Function


		<Obsolete>
		Protected Friend Overridable Function doPermuteSwap(ByVal shape As LongBuffer, ByVal rearrange() As Integer) As Long()
			Dim ret As val = New Long(rearrange.Length - 1){}
			For i As Integer = 0 To rearrange.Length - 1
				ret(i) = shape.get(rearrange(i))
			Next i
			Return ret
		End Function

		<Obsolete>
		Protected Friend Overridable Function doPermuteSwap(ByVal shape As IntBuffer, ByVal rearrange() As Integer) As Integer()
			Dim ret(rearrange.Length - 1) As Integer
			For i As Integer = 0 To rearrange.Length - 1
				ret(i) = shape.get(rearrange(i))
			Next i
			Return ret
		End Function

		<Obsolete>
		Protected Friend Overridable Function doPermuteSwap(ByVal shape As DataBuffer, ByVal rearrange() As Integer) As Integer()
			Dim ret(rearrange.Length - 1) As Integer
			For i As Integer = 0 To rearrange.Length - 1
				ret(i) = shape.getInt(rearrange(i))
			Next i
			Return ret
		End Function

		Protected Friend Overridable Function doPermuteSwap(ByVal shape() As Long, ByVal rearrange() As Integer) As Long()
			Dim ret As val = New Long(rearrange.Length - 1){}
			For i As Integer = 0 To rearrange.Length - 1
				ret(i) = shape(rearrange(i))
			Next i

			Return ret
		End Function


		Protected Friend Overridable Sub checkArrangeArray(ByVal arr() As Integer)
			Preconditions.checkArgument(arr.Length = jvmShapeInfo.rank, "Invalid rearrangement: number of arrangement (%s) != rank (%s)", arr.Length, jvmShapeInfo.rank)
			For i As Integer = 0 To arr.Length - 1
				If arr(i) >= arr.Length Then
					Throw New System.ArgumentException("The specified dimensions can't be swapped. Given element " & i & " was >= number of dimensions")
				End If
				If arr(i) < 0 Then
					Throw New System.ArgumentException("Invalid dimension: " & i & " : negative value")
				End If


			Next i

			For i As Integer = 0 To arr.Length - 1
				For j As Integer = 0 To arr.Length - 1
					If i <> j AndAlso arr(i) = arr(j) Then
						Throw New System.ArgumentException("Permute array must have unique elements")
					End If
				Next j
			Next i

		End Sub

		Protected Friend Overridable Sub autoProcessScalarCall()
	'        if (Nd4j.getExecutioner().getProfilingMode() != OpExecutioner.ProfilingMode.DISABLED && Nd4j.getExecutioner().getProfilingMode() != OpExecutioner.ProfilingMode.SCOPE_PANIC)
	'            OpProfiler.getInstance().processScalarCall();
		End Sub

		''' <summary>
		''' Checks whether the matrix is a vector.
		''' </summary>
		Public Overridable ReadOnly Property Vector As Boolean Implements INDArray.isVector
			Get
				If jvmShapeInfo.rank = 1 Then
					Return True
				End If
    
				Return RowVector OrElse ColumnVector
			End Get
		End Property

		Public Overridable ReadOnly Property VectorOrScalar As Boolean Implements INDArray.isVectorOrScalar
			Get
				Return ArrayList OrElse Scalar
			End Get
		End Property

		Public Overridable ReadOnly Property Square As Boolean Implements INDArray.isSquare
			Get
				Return Matrix AndAlso rows() = columns()
			End Get
		End Property

		Public Overridable ReadOnly Property RowVector As Boolean Implements INDArray.isRowVector
			Get
				Return (rank() = 2 AndAlso rows() = 1) AndAlso length() > 1 OrElse rank() = 1 AndAlso length() > 1
			End Get
		End Property

		Public Overridable ReadOnly Property ColumnVector As Boolean Implements INDArray.isColumnVector
			Get
				Return rank() = 2 AndAlso columns() = 1 AndAlso length() > 1
			End Get
		End Property

		Public Overridable ReadOnly Property ColumnVectorOrScalar As Boolean Implements INDArray.isColumnVectorOrScalar
			Get
				Return ColumnVector OrElse Scalar
			End Get
		End Property

		Public Overridable ReadOnly Property RowVectorOrScalar As Boolean Implements INDArray.isRowVectorOrScalar
			Get
				Return RowVector OrElse Scalar
			End Get
		End Property

		''' <summary>
		''' Generate string representation of the matrix.
		''' Printing will switch to scientific notation on a per element basis
		'''      - when abs value is greater than or equal to 10000
		'''      - when abs value is less than or equal to 0.0001 and not zero
		''' 
		'''  If the number of elements in the array is greater than 1000 (by default) only the first and last three elements
		'''  in a dimension are included. This can be changed globally using <seealso cref="NDArrayStrings.setMaxPrintElements(Long)"/>
		''' 
		''' 
		''' </summary>
		Public Overrides Function ToString() As String
			Return toString(New NDArrayStrings())
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public String toString(@NonNull NDArrayStrings options)
		Public Overridable Function toString(ByVal options As NDArrayStrings) As String Implements INDArray.toString
			If wasClosed() Then
				Return "<Closed NDArray, id=" & Id & ", dtype=" & dataType() & ", shape=" & java.util.Arrays.toString(shape()) & ">"
			End If
			If Not Compressed AndAlso Not preventUnpack Then
				Return options.format(Me)
			ElseIf Compressed AndAlso compressDebug Then
				Return "COMPRESSED ARRAY. SYSTEM PROPERTY compressdebug is true. This is to prevent auto decompression from being triggered."
			ElseIf preventUnpack Then
				Return "Array string unpacking is disabled."
			End If
			Return options.format(Me)
		End Function

		Public Overridable Function toString(ByVal maxElements As Long, ByVal forceSummarize As Boolean, ByVal precision As Integer) As String Implements INDArray.toString
			Return toString(New NDArrayStrings(maxElements, forceSummarize, precision))
		End Function


		Public Overridable Function toStringFull() As String Implements INDArray.toStringFull
			Return toString(Long.MaxValue, False, -1 * dataType().precision())
		End Function

		Public Overridable Function element() As Object Implements INDArray.element

			If Not Scalar Then
				Throw New System.InvalidOperationException("Unable to retrieve element from non scalar matrix")
			End If
			If data_Conflict.dataType() = DataType.FLOAT Then
				Return data_Conflict.getFloat(0)
			End If
			Return data_Conflict.getDouble(0)
		End Function

		Public Overridable Function remainder(ByVal denominator As INDArray) As INDArray Implements INDArray.remainder
			If Shape.areShapesBroadcastable(Me.shape(), denominator.shape()) Then
				Return remainder(denominator, createUninitialized(Me.dataType(), Shape.broadcastOutputShape(Me.shape(), denominator.shape())))
			Else
				Return remainder(denominator, Me.ulike())
			End If
		End Function

		Public Overridable Function remainder(ByVal denominator As INDArray, ByVal result As INDArray) As INDArray Implements INDArray.remainder
			validateNumericalArray("remainder", False)
			Preconditions.checkArgument(Shape.areShapesBroadcastable(Me.shape(), denominator.shape()),"Shapes must be broadcastable")

			Dim op As val = New RemainderOp(Me, denominator, result)
			Nd4j.Executioner.exec(op)
			Return result
		End Function

		Public Overridable Function remainder(ByVal denominator As Number) As INDArray Implements INDArray.remainder
			Return remainder(denominator, createUninitialized(Me.dataType(), Me.shape()))
		End Function

		Public Overridable Function remainder(ByVal denominator As Number, ByVal result As INDArray) As INDArray Implements INDArray.remainder
			validateNumericalArray("remainder", False)

			Dim op As New ScalarRemainder(Me, Nothing, result, denominator)
			Nd4j.Executioner.exec(op)
			Return result
		End Function

		Public Overridable Function remainderi(ByVal denominator As INDArray) As INDArray Implements INDArray.remainderi
			validateNumericalArray("remainderi", False)
			Dim op As New RemainderOp(Me, denominator, Me)
			Nd4j.Executioner.exec(op)
			Return Me
		End Function

		Public Overridable Function remainderi(ByVal denominator As Number) As INDArray Implements INDArray.remainderi
			validateNumericalArray("remainderi", False)
			Dim op As New ScalarRemainder(Me, Nothing, Me, denominator)
			Nd4j.Executioner.exec(op)
			Return Me
		End Function

		Public Overridable Function fmod(ByVal denominator As INDArray) As INDArray Implements INDArray.fmod
			validateNumericalArray("fmod", False)
			If Shape.areShapesBroadcastable(Me.shape(), denominator.shape()) Then
				Return fmod(denominator, createUninitialized(defaultFloatingPointType(), Shape.broadcastOutputShape(Me.shape(), denominator.shape())))
			Else
				Return fmod(denominator, Me.ulike())
			End If
		End Function

		Public Overridable Function fmod(ByVal denominator As INDArray, ByVal result As INDArray) As INDArray Implements INDArray.fmod
			validateNumericalArray("fmod", False)
			If Shape.areShapesBroadcastable(Me.shape(), denominator.shape()) Then
				Dim outShape As val = Shape.broadcastOutputShape(Me.shape(), denominator.shape())
				Preconditions.checkArgument(Shape.shapeEquals(outShape, result.shape()), "Result shape doesn't match expectations: " & java.util.Arrays.toString(result.shape()))

				exec(New FloorModOp(New INDArray(){Me, denominator}, New INDArray(){result}))

				Return result
			Else
				Dim op As New FModOp(Me, denominator, result)
				Nd4j.Executioner.exec(op)
				Return result
			End If
		End Function

		Public Overridable Function fmod(ByVal denominator As Number) As INDArray Implements INDArray.fmod
			Return fmod(denominator, createUninitialized(Me.dataType(), Me.shape()))
		End Function

		Public Overridable Function fmod(ByVal denominator As Number, ByVal result As INDArray) As INDArray Implements INDArray.fmod
			validateNumericalArray("fmod", False)
			Dim op As New ScalarFMod(Me, Nothing, result, denominator)
			Nd4j.Executioner.exec(op)
			Return result
		End Function

		Public Overridable Function fmodi(ByVal denominator As INDArray) As INDArray Implements INDArray.fmodi
			validateNumericalArray("fmodi", False)
			Dim op As New FModOp(Me, denominator, Me)
			Nd4j.Executioner.exec(op)
			Return Me
		End Function

		Public Overridable Function fmodi(ByVal denominator As Number) As INDArray Implements INDArray.fmodi
			validateNumericalArray("fmodi", False)
			Dim op As New ScalarFMod(Me, Nothing, Me, denominator)
			Nd4j.Executioner.exec(op)
			Return Me
		End Function

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		@Override public Iterator<Object> iterator()
		If True Then
			Return New FirstAxisIterator(Me)
		End If

		public Long originalOffset()
		If True Then
			If data().originalOffset() >= Integer.MaxValue Then
				Throw New System.ArgumentException("Original offset of buffer can not be >= Integer.MAX_VALUE")
			End If

			Return data().originalOffset()
		End If

		private void readObject(ObjectInputStream s)
		If True Then
			Try
				s.defaultReadObject()
				read(s)
			Catch e As Exception
				Throw New Exception(e)
			End Try

		End If

		private void writeObject(ObjectOutputStream [out]) throws IOException
		If True Then
			[out].defaultWriteObject()
			write([out])
		End If

		'Custom serialization for Java serialization
		protected void write(ObjectOutputStream [out]) throws IOException
		If True Then
			If Me.View Then
				'As per Nd4j.write, duplicate before writing to the output stream
				'BaseDataBuffer.write(...) doesn't know about strides etc, so dup (or equiv. strategy) is necessary here
				'Furthermore, because we only want to save the *actual* data for a view (not the full data), the shape info
				' (mainly strides, offset, element-wise stride) may be different in the duped array vs. the view array
				Dim copy As INDArray = Me.dup()
				copy.shapeInfoDataBuffer().write([out])
				copy.data().write([out])
			Else
				shapeInformation_Conflict.write([out])
				data().write([out])
			End If
		End If

		'Custom deserialization for Java serialization
		protected void read(ObjectInputStream s)
		If True Then
			Dim headerShape As val = BaseDataBuffer.readHeader(s)

			shapeInformation_Conflict = createBuffer(New Integer(Shape.shapeInfoLength(rank()) - 1){})
			shapeInformation_Conflict.read(s, headerShape.getLeft(), headerShape.getMiddle(), headerShape.getRight())

			ShapeInformation = Pair.create(shapeInformation_Conflict, shapeInformation_Conflict.asLong())

			Dim headerData As val = BaseDataBuffer.readHeader(s)
			data_Conflict = createBuffer(headerData.getRight(), headerData.getMiddle(), False)
			data().read(s, headerData.getLeft(), headerData.getMiddle(), headerData.getRight())
		End If

		public INDArray argMax(Integer... dimension)
		If True Then
			Return argMax(Me, dimension)
		End If

		public Boolean Attached
		If True Then
			If Empty Then
				Return False
			End If

			Preconditions.checkArgument(Not (data_Conflict Is Nothing AndAlso Not Empty), "Array has no buffer!")

			Return data_Conflict.Attached OrElse (data_Conflict.underlyingDataBuffer() IsNot Nothing AndAlso data_Conflict.underlyingDataBuffer().Attached) OrElse (data_Conflict.originalDataBuffer() IsNot Nothing AndAlso data_Conflict.originalDataBuffer().Attached)
		End If

		public Boolean InScope
		If True Then
			If Not Attached Then
				Return True
			End If

			Return data_Conflict.InScope
		End If

		public INDArray detach()
		If True Then
			If Not Attached Then
				Return Me
			End If

			WorkspaceUtils.assertValidArray(Me, "Cannot detach INDArray")

			Nd4j.Executioner.commit()

	'        
	'         two options here
	'         1) we're within some workspace
	'         2) we're out of any workspace
	'        
			If Nd4j.MemoryManager.CurrentWorkspace Is Nothing Then
				If Not View Then
					Nd4j.Executioner.commit()
					Dim buffer As DataBuffer = createBuffer(Me.dataType(), Me.length(), False)

					Nd4j.MemoryManager.memcpy(buffer, Me.data())

					Return createArrayFromShapeBuffer(buffer, Me.shapeInfoDataBuffer())
				Else
					Dim copy As INDArray = createUninitialized(Me.dataType(), Me.shape(), Me.ordering())
					copy.assign(Me)
					Nd4j.Executioner.commit()

					Return copy
				End If
			Else
				Dim workspace As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace
				Nd4j.MemoryManager.CurrentWorkspace = Nothing
				Dim copy As INDArray = Nothing

				If Not View Then
					Nd4j.Executioner.commit()
					Dim buffer As DataBuffer = createBuffer(Me.dataType(), Me.length(), False)

					'Pointer.memcpy(buffer.pointer(), this.data.pointer(), this.lengthLong() * Nd4j.sizeOfDataType(this.data.dataType()));
					Nd4j.MemoryManager.memcpy(buffer, Me.data())

					copy = createArrayFromShapeBuffer(buffer, Me.shapeInfoDataBuffer()) 'this.dup(this.ordering());


				Else
					copy = createUninitialized(Me.dataType(), Me.shape(), Me.ordering())
					copy.assign(Me)
					Nd4j.Executioner.commit()
				End If

				Nd4j.MemoryManager.CurrentWorkspace = workspace

				Return copy
			End If
		End If

		public INDArray leverage()
		If True Then
			WorkspaceUtils.assertValidArray(Me, "Cannot leverage INDArray to new workspace")
			If Not Attached Then
				Return Me
			End If

			Dim workspace As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace
			If workspace Is Nothing Then
				Return Me.detach()
			End If

			Dim parentWorkspace As MemoryWorkspace = workspace.ParentWorkspace

			If Me.data_Conflict.ParentWorkspace Is parentWorkspace Then
				Return Me
			End If

			' if there's no parent ws - just detach
			If parentWorkspace Is Nothing Then
				Return Me.detach()
			Else
				Nd4j.Executioner.commit()

				' temporary set parent ws as current ws
				Nd4j.MemoryManager.CurrentWorkspace = parentWorkspace

				Dim copy As INDArray = Nothing
				If Not Me.View Then
					Nd4j.Executioner.commit()
					Dim buffer As DataBuffer = createBuffer(Me.length(), False)
					Nd4j.MemoryManager.memcpy(buffer, Me.data())

					copy = createArrayFromShapeBuffer(buffer, Me.shapeInfoDataBuffer())
				Else
					copy = Me.dup(Me.ordering())
					Nd4j.Executioner.commit()
				End If

				' restore current ws
				Nd4j.MemoryManager.CurrentWorkspace = workspace
				Return copy
			End If
		End If

		public INDArray leverageTo(String id)
		If True Then
			Return leverageTo(id, False)
		End If

		public INDArray leverageTo(String id, Boolean enforceExistence) throws Nd4jNoSuchWorkspaceException
		If True Then
			WorkspaceUtils.assertValidArray(Me, "Cannot leverage INDArray to new workspace")
			If Not Attached Then
				Return Me
			End If

			If Not Nd4j.WorkspaceManager.checkIfWorkspaceExists(id) Then
				If enforceExistence Then
					Throw New Nd4jNoSuchWorkspaceException(id)
				Else
					Return Me
				End If
			End If

			Dim current As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace
			Dim target As MemoryWorkspace = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(id)

			If Me.data_Conflict.ParentWorkspace Is target Then
				Return Me
			End If

			Nd4j.MemoryManager.CurrentWorkspace = target
			Dim copy As INDArray = Nothing
			If Not Me.View Then
				Nd4j.Executioner.commit()
				Dim buffer As DataBuffer = createBuffer(Me.dataType(), Me.length(), False)
				Nd4j.MemoryManager.memcpy(buffer, Me.data())

				copy = createArrayFromShapeBuffer(buffer, Me.shapeInfoDataBuffer())
			Else
				copy = Me.dup(Me.ordering())
				Nd4j.Executioner.commit()
			End If

			Nd4j.MemoryManager.CurrentWorkspace = current

			Return copy
		End If

		public INDArray leverageOrDetach(String id)
		If True Then
			If Not Attached Then
				Return Me
			End If

			If Not Nd4j.WorkspaceManager.checkIfWorkspaceExistsAndActive(id) Then
				Return detach()
			End If
			Return leverageTo(id)
		End If

		public INDArray migrate()
		If True Then
			Return migrate(False)
		End If

		public INDArray migrate(Boolean detachOnNoWs)
		If True Then
			WorkspaceUtils.assertValidArray(Me, "Cannot leverage INDArray to new workspace")

			Dim current As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace

			If current Is Nothing Then
				If detachOnNoWs Then
					Return detach()
				Else
					Return Me
				End If
			End If

			Dim copy As INDArray = Nothing

			If Not Me.View Then
				Nd4j.Executioner.commit()
				Dim buffer As DataBuffer = createBuffer(Me.dataType(), Me.length(), False)
				Nd4j.MemoryManager.memcpy(buffer, Me.data())

				copy = createArrayFromShapeBuffer(buffer, Me.shapeInfoDataBuffer())
			Else
				copy = Me.dup(Me.ordering())
				Nd4j.Executioner.commit()
			End If

			Return copy
		End If

		public Number percentileNumber(Number quantile)
		If True Then
			validateNumericalArray("percentileNumber", False)
			If quantile.intValue() < 0 OrElse quantile.intValue() > 100 Then
				Throw New ND4JIllegalStateException("Percentile value should be in 0...100 range")
			End If

			If Scalar Then
				Return Me.getDouble(0)
			End If

			Dim sorted As INDArray = sort(Me.dup(Me.ordering()), True)

			Return getPercentile(quantile, sorted)
		End If

		public Number medianNumber()
		If True Then
			validateNumericalArray("medianNumber", False)
			If Scalar Then
				Return getNumber(0)
			End If
			Return percentileNumber(50)
		End If

		public INDArray median(Integer... dimension)
		If True Then
			validateNumericalArray("median", False)
			'Check edge case: size 1 element. No dimension == full array
			If dimension.length = 0 Then
				Return scalar(dataType(), medianNumber().doubleValue())
			End If
			Dim shapeProd As Long = 1
			For Each d As Integer In dimension
				shapeProd *= size(d)
			Next d
			If shapeProd = 1 Then
				Dim newShape() As Long = ArrayUtil.removeIndex(shape(), dimension)
				Return dup("c"c).reshape("c"c, newShape)
			End If
			Return percentile(50, dimension)
		End If

		protected Double getPercentile(Number quantile, INDArray sorted)
		If True Then
			validateNumericalArray("getPercentile", False)
			If quantile.intValue() = 0 Then
				Return sorted.getDouble(0)
			ElseIf quantile.intValue() = 100 Then
				Return sorted.getDouble(sorted.length() - 1)
			End If

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim pos As Double = (quantile.doubleValue() / 100.0) * CDbl(sorted.length() + 1)
			If pos < 1 Then
				Return sorted.getDouble(0)
			ElseIf pos >= sorted.length() Then
				Return sorted.getDouble(sorted.length() - 1)
			End If

			Dim fposition As Double = FastMath.floor(pos)
			Dim position As Integer = CInt(Math.Truncate(fposition))

			Dim diff As Double = pos - fposition

			Dim lower As Double = sorted.getDouble(position-1)
			Dim upper As Double = sorted.getDouble(position)

			Return lower + diff * (upper - lower)
		End If

		public INDArray percentile(Number quantile, Integer... dimension)
		If True Then
			validateNumericalArray("percentile", False)
			If quantile.doubleValue() < 0 OrElse quantile.doubleValue() > 100 Then
				Throw New ND4JIllegalStateException("Percentile value should be in 0...100 range")
			End If

			If Scalar Then
				Return scalar(Me.getDouble(0))
			End If

			Dim sorted As INDArray = Nd4j.NDArrayFactory.sort(Me.dup(Me.ordering()), False, dimension)

			' there's no practical sense doing this on GPU, stride will be just size of TAD.
			Dim ret As INDArray = createUninitialized(defaultFloatingPointType(), sorted.tensorsAlongDimension(dimension))
			Dim i As Integer = 0
			Do While i < ret.length()
				ret.putScalar(i, getPercentile(quantile, sorted.tensorAlongDimension(i, dimension)))
				i += 1
			Loop

			Return ret

		End If

		protected abstract Integer stringBuffer(FlatBufferBuilder builder, DataBuffer buffer)

		public Integer toFlatArray(FlatBufferBuilder builder)
		If True Then
			If View Then
				Return dup(Me.ordering()).toFlatArray(builder)
			End If
			Dim shape As Integer = FlatArray.createShapeVector(builder, Me.shapeInfoDataBuffer().asLong())
			Dim buffer As Integer = If(Me.Empty, 0, If(Me.dataType() = DataType.UTF8, stringBuffer(builder, Me.data()), FlatArray.createBufferVector(builder, Me.data().asBytes())))
			Dim type As val = If(Me.Empty, FlatBuffersMapper.getDataTypeAsByte(Me.dataType()), FlatBuffersMapper.getDataTypeAsByte(Me.data().dataType()))
			Dim array As Integer = FlatArray.createFlatArray(builder, shape, buffer, type, ByteOrder.BE)

			Return array
		End If

		protected static DataTypeEx convertType(DataType type)
		If True Then
			If type = DataType.HALF Then
				Return DataTypeEx.FLOAT16
			ElseIf type = DataType.FLOAT Then
				Return DataTypeEx.FLOAT
			ElseIf type = DataType.DOUBLE Then
				Return DataTypeEx.DOUBLE

			ElseIf type = DataType.INT Then
				Return DataTypeEx.INT8
			ElseIf type = DataType.LONG Then
				Return DataTypeEx.INT16

			Else
				Throw New System.InvalidOperationException("Unknown dataType: [" & type & "]")
			End If
		End If

		public Boolean Empty
		If True Then
			Return Shape.isEmpty(jvmShapeInfo.javaShapeInformation)
		End If

		public Long() shapeInfoJava()
		If True Then
			Return jvmShapeInfo.javaShapeInformation
		End If

		public DataType dataType()
		If True Then
			If data_Conflict IsNot Nothing Then
				Return data_Conflict.dataType()
			End If

			Dim e As val = Shape.extras(jvmShapeInfo.javaShapeInformation)

			If e <> 0 Then
				Dim t As val = ArrayOptionsHelper.dataType(jvmShapeInfo.javaShapeInformation)
				If t <> DataType.UNKNOWN Then
					Return t
				End If
			End If

			Return DataType.UNKNOWN
		End If

		public Boolean R
		If True Then
			Dim dtype As val = dataType()
			Return dtype = DataType.FLOAT OrElse dtype = DataType.DOUBLE OrElse dtype = DataType.HALF OrElse dtype = DataType.BFLOAT16
		End If

		public Boolean Z
		If True Then
			Return Not R AndAlso Not B AndAlso Not S
		End If

		public Boolean B
		If True Then
			Return dataType() = DataType.BOOL
		End If

		public Boolean S
		If True Then
			Return dataType() = DataType.UTF8
		End If

		public INDArray castTo(DataType dataType)
		If True Then
			If dataType = dataType() Then 'No-op if correct datatype
				Return Me
			End If
			If Empty AndAlso rank() = 0 Then
				Return empty(dataType)
			End If
			Dim result As val = createUninitialized(dataType, Me.shape(), Me.ordering())
			result.assign(Me)
			Return result
		End If

		public Boolean all()
		If True Then
			Dim r As val = Nd4j.Executioner.exec(New All(Me))
			Return r.getDouble(0) <> 0.0
		End If

		public Boolean any()
		If True Then
			Dim r As val = Nd4j.Executioner.exec(New Any(Me))
			Return r.getDouble(0) <> 0.0
		End If

		public Boolean none()
		If True Then
			Return Not any()
		End If


		''' <summary>
		''' Validate that the operation is being applied on a numerical array (not boolean or utf8).
		''' Some operations (such as sum, norm2, add(Number) etc don't make sense when applied to boolean/utf8 arrays </summary>
		''' <param name="opName"> Operation name to print in the exception </param>
		protected void validateNumericalArray(String opName, Boolean allowEmpty)
		If True Then
			If dataType() = DataType.BOOL OrElse dataType() = DataType.UTF8 Then
				Throw New System.InvalidOperationException("Cannot apply operation " & opName & " to array with " & dataType() & " datatype. Array shape: " & java.util.Arrays.toString(shape()))
			End If
			If Not allowEmpty AndAlso Empty Then
				Throw New System.InvalidOperationException("Cannot perform operation " & opName & " on empty array with datatype " & dataType())
			End If
		End If

		public Boolean closeable()
		If True Then
			If released OrElse Attached Then
				Return False
			End If

			' empty arrays have no buffer at all
			If Empty Then
				Return True
			End If

			If View Then
				Return False
			End If

			Return data_Conflict.closeable()
		End If

		public void close()
		If True Then
			' empty arrays have no buffer at all
			If released OrElse Empty Then
				Return
			End If

			Nd4j.Executioner.commit()

			If Not closeable() Then
				Throw New ND4JIllegalStateException("Can't release this INDArray")
			End If

			data_Conflict.close()

			released = True
		End If

		public INDArray [like]()
		If True Then
			Return create(Me.dataType(), Me.shape(), getStrides(Me.shape(), Me.ordering()), Me.ordering())
		End If

		public INDArray ulike()
		If True Then
			Return createUninitialized(Me.dataType(), Me.shape(), Me.ordering())
		End If

		public Boolean wasClosed()
		If True Then
			' data can be null if that's empty array
			If released OrElse (data() IsNot Nothing AndAlso data().wasClosed()) Then
				Return True
			End If

			Return False
		End If

		public Long Id
		If True Then
			Return arrayId
		End If
	End Class

End Namespace