Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports SysMath = System.Math
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports CoordinateFunction = DeepLearning4VB.org.nd4vb.linalg.api.shape.loop.coordinatefunction.CoordinateFunction
Imports ArrayOptionsHelper = DeepLearning4VB.org.nd4vb.linalg.api.shape.options.ArrayOptionsHelper
Imports ArrayType = DeepLearning4VB.org.nd4vb.linalg.api.shape.options.ArrayType
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
Imports INDArrayIndex = DeepLearning4VB.org.nd4vb.linalg.indexing.INDArrayIndex

'
' *  ******************************************************************************
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

Namespace org.nd4vb.linalg.api.shape

    ''' <summary>
    ''' Static helpers over shapes, strides and ND4J's shape information (Java <c>org.nd4j.linalg.api.shape.Shape</c>).
    ''' <para>
    ''' The shape information is a <c>Long()</c> (or a LONG <see cref="DataBuffer"/>) laid out as
    ''' <c>[rank, shape(0..rank-1), stride(0..rank-1), extras, elementWiseStride, order]</c>, of length
    ''' <c>2 * rank + 4</c> (<see cref="shapeInfoLength(Long)"/>): strides are in elements, <c>extras</c> holds the
    ''' data-type and array-type flags of <see cref="ArrayOptionsHelper"/>, and <c>order</c> is <c>AscW("c"c)</c> or
    ''' <c>AscW("f"c)</c>. A scalar (rank 0) is <c>[0, extras, elementWiseStride, order]</c>. This is the layout the
    ''' managed kernels read (<c>managed.kernels.ShapeInfoHelper</c>).
    ''' </para>
    ''' <para>
    ''' Differences from Java:
    ''' <list type="bullet">
    ''' <item>Native-only members not ported: every overload over <c>java.nio.IntBuffer</c>/<c>LongBuffer</c>
    ''' (<c>rank</c>, <c>size</c>, <c>shape</c>, <c>stride</c>, <c>shapeOf</c>, <c>length</c>, <c>isVector</c>,
    ''' <c>isMatrix</c>, <c>isRowVectorShape</c>, <c>getOffset</c>, <c>offset</c>, <c>order</c>, <c>elementWiseStride</c>,
    ''' <c>setElementWiseStride</c>, <c>setOrder</c>, <c>shapeToString</c>, <c>bufferToString</c>, <c>contentEquals</c>,
    ''' <c>toString</c>) and <c>toBuffer(int...)</c>, which allocated a direct <c>ByteBuffer</c>. Use the <c>Long()</c>
    ''' and <see cref="DataBuffer"/> overloads.</item>
    ''' <item>New: <see cref="createShapeInfoArray(Long(), Long(), Long, Char, Long)"/> and
    ''' <see cref="shapeToString(Long())"/>, the pure <c>Long()</c> counterparts of <c>createShapeInformation</c> and
    ''' Java's <c>shapeToString(LongBuffer)</c>; <see cref="shapeToString(INDArray)"/> uses the latter.</item>
    ''' <item>Java <c>Number</c> is <see cref="Object"/> (<see cref="pickPairwiseDataType(DataType, Object)"/>), and
    ''' Java's <c>RuntimeException</c>s for an invalid order are <see cref="InvalidOperationException"/>s.</item>
    ''' <item><see cref="createShapeInformation(Long(), Long(), Long, Char, DataType, Boolean)"/> delegates to
    ''' <c>Nd4vb.Executioner.createShapeInfo</c> as in Java; the managed backend builds the buffer from
    ''' <see cref="createShapeInfoArray(Long(), Long(), Long, Char, Long)"/>.</item>
    ''' </list>
    ''' </para>
    ''' </summary>
    Public Class Shape

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Return the shape of the largest length array
        ''' based on the input </summary>
        ''' <param name="inputs"> the inputs to get the max shape for </param>
        ''' <returns> the largest shape based on the inputs </returns>
        Public Shared Function getMaxShape(ParamArray ByVal inputs() As INDArray) As Long()
            If inputs Is Nothing Then
                Return Nothing
            ElseIf inputs.Length < 2 Then
                Return inputs(0).shape()
            Else
                Dim currMax() As Long = inputs(0).shape()
                For i As Integer = 1 To inputs.Length - 1
                    If inputs(i) Is Nothing Then
                        Continue For
                    End If
                    If ArrayUtil.prod(currMax) < inputs(i).length() Then
                        currMax = inputs(i).shape()
                    End If
                Next i

                Return currMax
            End If
        End Function

        ''' <summary>
        ''' Returns true if this shape is scalar </summary>
        ''' <param name="shape"> the shape that is scalar
        ''' @return </param>
        Public Shared Function shapeIsScalar(ByVal shapeArr() As Integer) As Boolean
            Return shapeArr.Length = 0 OrElse ArrayUtil.prodLong(shapeArr) = 1
        End Function

        Public Shared Function shapeIsScalar(ByVal shapeArr() As Long) As Boolean
            Return shapeArr.Length = 0 OrElse ArrayUtil.prodLong(shapeArr) = 1
        End Function

        ''' <summary>
        ''' Returns true if any shape has a -1
        ''' or a null or empty array is passed in </summary>
        ''' <param name="shape"> the input shape to validate </param>
        ''' <returns> true if the shape is null,empty, or contains a -1 element </returns>
        Public Shared Function isPlaceholderShape(ByVal shapeArr() As Integer) As Boolean
            If shapeArr Is Nothing Then
                Return True
            Else
                For i As Integer = 0 To shapeArr.Length - 1
                    If shapeArr(i) < 0 Then
                        Return True
                    End If
                Next i
            End If

            Return False
        End Function

        Public Shared Function isPlaceholderShape(ByVal shapeArr() As Long) As Boolean
            If shapeArr Is Nothing Then
                Return True
            Else
                If shapeArr.Length = 1 AndAlso shapeArr(0) = Long.MinValue Then
                    'Temporary sentinel for empty array
                    Return False
                End If
                For i As Integer = 0 To shapeArr.Length - 1
                    If shapeArr(i) < 0 Then
                        Return True
                    End If
                Next i
            End If

            Return False
        End Function

        ''' <summary>
        ''' Compute the broadcast rules according to:
        ''' https://docs.scipy.org/doc/numpy-1.10.1/user/basics.broadcasting.html
        '''
        ''' Note that the array can be null if the arrays are already equal
        ''' in shape.
        '''
        ''' This function should be used in conjunction with
        ''' the shape ops.
        ''' </summary>
        ''' <param name="left"> the left array </param>
        ''' <param name="right"> the right array (the array to be broadcasted </param>
        ''' <returns> the broadcast dimensions if any </returns>
        Public Shared Function getBroadcastDimensions(ByVal left() As Integer, ByVal right() As Integer) As Integer()
            If left.SequenceEqual(right) Then
                Return Nothing
            End If

            Dim n As Integer = SysMath.Min(left.Length,right.Length)
            Dim dims As New List(Of Integer)()
            Dim leftIdx As Integer = left.Length - 1
            Dim rightIdx As Integer = right.Length - 1
            For i As Integer = n - 1 To 0 Step -1
                If left(leftIdx) <> right(rightIdx) AndAlso right(rightIdx) = 1 OrElse left(leftIdx) = 1 Then
                    dims.Add(i)
                ElseIf left(leftIdx) <> right(rightIdx) Then
                    Throw New System.ArgumentException("Unable to broadcast dimension " & i & " due to shape mismatch. Right shape must be 1. " & "Left array shape: " & Arrays.toString(left) & ", right array shape: " & Arrays.toString(right))
                End If

                leftIdx -= 1
                rightIdx -= 1
            Next i

            dims.Reverse()
            Return Ints.toArray(dims)
        End Function

        Public Shared Function sizeAt(ByVal shapeArr() As Long, ByVal index As Integer) As Long
            If index < 0 Then
                index += shapeArr.Length
            End If

            Return shapeArr(index)
        End Function

        Public Shared Function getBroadcastDimensions(ByVal left() As Long, ByVal right() As Long) As Integer()
            If left.SequenceEqual(right) Then
                Return Nothing
            End If

            Dim n As Integer = SysMath.Min(left.Length,right.Length)
            Dim dims As New List(Of Integer)()
            Dim leftIdx As Integer = left.Length - 1
            Dim rightIdx As Integer = right.Length - 1
            For i As Integer = n - 1 To 0 Step -1
                If left(leftIdx) <> right(rightIdx) AndAlso right(rightIdx) = 1 OrElse left(leftIdx) = 1 Then
                    dims.Add(i)
                ElseIf left(leftIdx) <> right(rightIdx) Then
                    Throw New System.ArgumentException("Unable to broadcast dimension " & i & " due to shape mismatch. Right shape must be 1. " & "Left array shape: " & Arrays.toString(left) & ", right array shape: " & Arrays.toString(right))
                End If

                leftIdx -= 1
                rightIdx -= 1
            Next i

            dims.Reverse()
            Return Ints.toArray(dims)
        End Function

        ''' <summary>
        ''' Get the broadcast output shape
        ''' based on the 2 input shapes
        ''' Result output shape based on:
        ''' https://docs.scipy.org/doc/numpy-1.10.1/user/basics.broadcasting.html
        '''
        ''' </summary>
        ''' <param name="left"> the left shape </param>
        ''' <param name="right"> the right second
        ''' @return </param>
        Public Shared Function broadcastOutputShape(ByVal left() As Integer, ByVal right() As Integer) As Integer()
            assertBroadcastable(left, right)
            If left.SequenceEqual(right) Then
                Return left
            End If
            Dim n As Integer = SysMath.Max(left.Length,right.Length)
            Dim dims As New List(Of Integer)()
            Dim leftIdx As Integer = left.Length - 1
            Dim rightIdx As Integer = right.Length - 1
            For i As Integer = n - 1 To 0 Step -1
                If leftIdx < 0 Then
                    dims.Add(right(rightIdx))
                ElseIf rightIdx < 0 Then
                    dims.Add(left(leftIdx))
                ElseIf left(leftIdx) <> right(rightIdx) AndAlso right(rightIdx) = 1 OrElse left(leftIdx) = 1 Then
                    dims.Add(SysMath.Max(left(leftIdx),right(rightIdx)))
                ElseIf left(leftIdx) = right(rightIdx) Then
                    dims.Add(left(leftIdx))
                Else
                    Throw New System.ArgumentException("Unable to broadcast dimension " & i & " due to shape mismatch. Right shape must be 1.")
                End If

                leftIdx -= 1
                rightIdx -= 1
            Next i

            dims.Reverse()
            Return Ints.toArray(dims)
        End Function

        Public Shared Function containsZeros(ByVal shapeOnly() As Long) As Boolean
            For Each v In shapeOnly
                If v = 0 Then
                    Return True
                End If
            Next v

            Return False
        End Function

        ''' <summary>
        ''' Assert that the broadcast operation {@code result = first.op(second)} is valid, given the
        ''' shapes of first, second, and result.<br>
        ''' Throws an exception otherwise
        ''' </summary>
        ''' <param name="op">     Name of the operation </param>
        ''' <param name="first">  First array </param>
        ''' <param name="second"> Second array </param>
        ''' <param name="result"> Result arrray. </param>
        Public Shared Sub assertBroadcastable(ByVal op As String, ByVal first As INDArray, ByVal second As INDArray, ByVal result As INDArray)
            Dim fShape() As Long = first.shape()
            Dim sShape() As Long = second.shape()
            Preconditions.checkState(ShapeT.areShapesBroadcastable(fShape, sShape), "Cannot perform operation ""%s"" - shapes are not equal and are not broadcastable." & "first.shape=%s, second.shape=%s", op, fShape, sShape)

            Dim outShape() As Long = ShapeT.broadcastOutputShape(fShape, sShape)
            If Not outShape.SequenceEqual(result.shape()) Then
                'Two cases
                ' 1. x.addi(y)
                ' 2. x.addi(y, z)

                Dim extra As String = ""
                If first Is result Then
                    extra = "." & vbLf & "In-place operations like x." & op & "(y) can only be performed when x and y have the same shape," & " or x and y are broadcastable with x.shape() == broadcastShape(x,y)"
                End If

                Throw New System.InvalidOperationException("Cannot perform in-place operation """ & op & """: result array shape does" & " not match the broadcast operation output shape: " & Arrays.toString(fShape) & "." & op & "(" & Arrays.toString(sShape) & ") != " & Arrays.toString(result.shape()) & extra)
            End If
        End Sub

        Public Shared Function broadcastOutputShape(ByVal left() As Long, ByVal right() As Long) As Long()
            If containsZeros(left) Then
                Return left
            ElseIf containsZeros(right) Then
                Return right
            End If

            assertBroadcastable(left, right)
            If left.SequenceEqual(right) Then
                Return left
            End If
            Dim n As Integer = SysMath.Max(left.Length,right.Length)
            Dim dims As New List(Of Long)()
            Dim leftIdx As Integer = left.Length - 1
            Dim rightIdx As Integer = right.Length - 1
            For i As Integer = n - 1 To 0 Step -1
                If leftIdx < 0 Then
                    dims.Add(right(rightIdx))
                ElseIf rightIdx < 0 Then
                    dims.Add(left(leftIdx))
                ElseIf left(leftIdx) <> right(rightIdx) AndAlso right(rightIdx) = 1 OrElse left(leftIdx) = 1 Then
                    dims.Add(SysMath.Max(left(leftIdx),right(rightIdx)))
                ElseIf left(leftIdx) = right(rightIdx) Then
                    dims.Add(left(leftIdx))
                Else
                    Throw New System.ArgumentException("Unable to broadcast dimension " & i & " due to shape mismatch. Right shape must be 1.")
                End If

                leftIdx -= 1
                rightIdx -= 1
            Next i

            dims.Reverse()
            Return Longs.toArray(dims)
        End Function

        '''
        ''' <param name="newShape"> the new shape possibly
        '''                 containing a negative number </param>
        ''' <param name="shape"> the shape to calculate from
        ''' @return </param>
        Public Shared Function resolveNegativeShapeIfNeccessary(ByVal newShape() As Integer, ByVal shapeArr() As Integer) As Integer()
            Dim numberNegativesOnes As Integer = 0
            Dim i As Integer = 0
            Do While i < shapeArr.Length
                If shapeArr(i) < 0 Then
                    If numberNegativesOnes >= 1 Then
                        Throw New System.ArgumentException("Only one dimension can be negative ones")
                    End If

                    numberNegativesOnes += 1

                    Dim shapeLength As Integer = 1
                    For j As Integer = 0 To shapeArr.Length - 1
                        If shapeArr(j) >= 1 Then
                            shapeLength *= shapeArr(j)
                        End If
                    Next j
                    Dim realShape As Integer = SysMath.Abs(ArrayUtil.prod(newShape) \ shapeLength)
                    Dim thisNewShape(shapeArr.Length - 1) As Integer
                    For j As Integer = 0 To shapeArr.Length - 1
                        If i <> j Then
                            thisNewShape(j) = shapeArr(j)
                        Else
                            thisNewShape(j) = realShape
                        End If
                    Next j

                    shapeArr = thisNewShape
                    Exit Do

                End If

                i += 1
            Loop

            For k As Integer = 0 To shapeArr.Length - 1
                If shapeArr(k) = 0 Then
                    shapeArr(k) = 1
                End If
            Next k

            Return shapeArr

        End Function

        ''' <summary>
        ''' Returns true if the dimension is null
        ''' or the dimension length is 1 and the first entry
        ''' is <seealso cref="Integer.MAX_VALUE"/> </summary>
        ''' <param name="shape"> the shape of the input array </param>
        ''' <param name="dimension"> the dimensions specified
        ''' </param>
        ''' <returns> true if the dimension length is equal to the shape length
        ''' the dimension is null or the dimension length is 1 and the first entry is
        ''' <seealso cref="Integer.MAX_VALUE"/> </returns>
        Public Shared Function isWholeArray(ByVal shapeArr() As Integer, ParamArray ByVal dimension() As Integer) As Boolean
            Return isWholeArray(shapeArr.Length, dimension)
        End Function

        Public Shared Function isWholeArray(ByVal shapeArr() As Long, ParamArray ByVal dimension() As Integer) As Boolean
            Return isWholeArray(shapeArr.Length, dimension)
        End Function

        ''' <summary>
        ''' Returns true if the dimension is null
        ''' or the dimension length is 1 and the first entry
        ''' is <seealso cref="Integer.MAX_VALUE"/> </summary>
        ''' <param name="rank"> the rank of the input array </param>
        ''' <param name="dimension"> the dimensions specified
        ''' </param>
        ''' <returns> true if the dimension length is equal to the rank,
        ''' the dimension is null or the dimension length is 1 and the first entry is
        ''' <seealso cref="Integer.MAX_VALUE"/> </returns>
        Public Shared Function isWholeArray(ByVal rank As Integer, ParamArray ByVal dimension() As Integer) As Boolean
            Return rank = 0 OrElse dimension Is Nothing OrElse dimension.Length = 0 OrElse (dimension.Length = 1 AndAlso dimension(0) = Integer.MaxValue) OrElse dimension.Length = rank
        End Function

        ''' <summary>
        ''' Get the shape of the reduced array </summary>
        ''' <param name="wholeShape"> the shape of the array
        '''                   with the reduce op being performed </param>
        ''' <param name="dimensions"> the dimensions the reduce op is being performed on </param>
        ''' <returns> the shape of the result array as the result of the reduce </returns>
        Public Shared Function getReducedShape(ByVal wholeShape() As Integer, ByVal dimensions() As Integer) As Long()
            If isWholeArray(wholeShape, dimensions) Then
                Return New Long() {}
            ElseIf dimensions.Length = 1 AndAlso wholeShape.Length = 2 Then
                Dim ret = New Long(1){}
                If dimensions(0) = 1 Then
                    ret(0) = wholeShape(0)
                    ret(1) = 1
                ElseIf dimensions(0) = 0 Then
                    ret(0) = 1
                    ret(1) = wholeShape(1)
                End If
                Return ret
            End If

            Return ArrayUtil.toLongArray(ArrayUtil.removeIndex(wholeShape, dimensions))
        End Function

        Public Shared Function getReducedShape(ByVal wholeShape() As Long, ByVal dimensions() As Integer) As Long()
            If isWholeArray(wholeShape, dimensions) Then
                Return New Long() {}
            ElseIf dimensions.Length = 1 AndAlso wholeShape.Length = 2 Then
                Dim ret = New Long(1){}
                If dimensions(0) = 1 Then
                    ret(0) = wholeShape(0)
                    ret(1) = 1
                ElseIf dimensions(0) = 0 Then
                    ret(0) = 1
                    ret(1) = wholeShape(1)
                End If
                Return ret
            End If

            Return ArrayUtil.removeIndex(wholeShape, dimensions)
        End Function

        ''' <summary>
        ''' Get the shape of the reduced array
        ''' </summary>
        ''' <param name="wholeShape"> the shape of the array
        '''                   with the reduce op being performed </param>
        ''' <param name="dimensions"> the dimensions the reduce op is being performed on </param>
        ''' <param name="keepDims"> if set to true, corresponding dimensions will be set to 1 </param>
        ''' <returns> the shape of the result array as the result of the reduce </returns>
        Public Shared Function getReducedShape(ByVal wholeShape() As Integer, ByVal dimensions() As Integer, ByVal keepDims As Boolean, ByVal newFormat As Boolean) As Long()
            ' we need to normalize dimensions, in case they have negative values or unsorted, or whatever
            dimensions = ShapeT.normalizeAxis(wholeShape.Length, dimensions)

            ' strip leading keepDims argument
            'if (newFormat)
            '    dimensions = Arrays.copyOfRange(dimensions, 1, dimensions.length);

            If Not keepDims Then
                If Not newFormat Then
                    Return getReducedShape(wholeShape, dimensions)
                Else
                    If isWholeArray(wholeShape, dimensions) Then
                        Return New Long() {}
                    ElseIf dimensions.Length = 1 AndAlso wholeShape.Length = 2 Then
                        Dim ret = New Long(0){}
                        If dimensions(0) = 1 Then
                            ret(0) = wholeShape(0)
                        ElseIf dimensions(0) = 0 Then
                            ret(0) = wholeShape(1)
                        End If
                        Return ret
                    End If

                    Return ArrayUtil.toLongArray(ArrayUtil.removeIndex(wholeShape, dimensions))
                End If
            End If

            ' we'll return full array of 1 as shape
            If isWholeArray(wholeShape, dimensions) Then
                Dim ones = New Long(wholeShape.Length - 1){}

                Arrays.Fill(ones, 1L)
                Return ones
            End If

            Dim result = ArrayUtil.toLongArray(Arrays.CopyOf(wholeShape, wholeShape.Length))
            For Each [dim] In dimensions
                result([dim]) = 1
            Next [dim]

            Return result
        End Function

        Public Shared Function getReducedShape(ByVal wholeShape() As Long, ByVal dimensions() As Integer, ByVal keepDims As Boolean) As Long()
             Return getReducedShape(wholeShape, dimensions, keepDims, True)
        End Function

        Public Shared Function getReducedShape(ByVal wholeShape() As Long, ByVal dimensions() As Integer, ByVal keepDims As Boolean, ByVal newFormat As Boolean) As Long()
            ' we need to normalize dimensions, in case they have negative values or unsorted, or whatever
            dimensions = ShapeT.normalizeAxis(wholeShape.Length, dimensions)

            ' strip leading keepDims argument
            'if (newFormat)
            '    dimensions = Arrays.copyOfRange(dimensions, 1, dimensions.length);

            If Not keepDims Then
                If Not newFormat Then
                    Return getReducedShape(wholeShape, dimensions)
                Else
                    If isWholeArray(wholeShape, dimensions) Then
                        Return New Long() {}
                    ElseIf dimensions.Length = 1 AndAlso wholeShape.Length = 2 Then
                        Dim ret = New Long(0){}
                        If dimensions(0) = 1 Then
                            ret(0) = wholeShape(0)
                        ElseIf dimensions(0) = 0 Then
                            ret(0) = wholeShape(1)
                        End If
                        Return ret
                    End If

                    Return ArrayUtil.removeIndex(wholeShape, dimensions)
                End If
            End If

            ' we'll return full array of 1 as shape
            If isWholeArray(wholeShape, dimensions) Then
                Dim ones = New Long(wholeShape.Length - 1){}

                Arrays.Fill(ones, 1L)
                Return ones
            End If

            Dim result = Arrays.CopyOf(wholeShape, wholeShape.Length)
            For Each [dim] In dimensions
                result([dim]) = 1
            Next [dim]

            Return result
        End Function

        ''' <summary>
        ''' Get the output shape of a matrix multiply
        ''' </summary>
        ''' <param name="left"> the first matrix shape to multiply </param>
        ''' <param name="right"> the second matrix shape to multiply </param>
        ''' <returns> the shape of the output array (the left's rows and right's columns) </returns>
        Public Shared Function getMatrixMultiplyShape(ByVal left() As Integer, ByVal right() As Integer) As Integer()
            If ShapeT.shapeIsScalar(left) Then
                Return right
            End If

            If ShapeT.shapeIsScalar(right) Then
                Return left
            End If

            If left.Length <> 2 AndAlso right.Length <> 2 Then
                Throw New System.ArgumentException("Illegal shapes for matrix multiply. Must be of length 2. Left shape: " & Arrays.toString(left) & ", right shape: " & Arrays.toString(right))
            End If

            Dim i As Integer = 0
            Do While i < left.Length
                If left(i) < 1 Then
                    Throw New ND4VBIllegalStateException("Left shape contained value < 0 at index " & i & " - left shape " & Arrays.toString(left))
                End If
                i += 1
            Loop

            i = 0
            Do While i < right.Length
                If right(i) < 1 Then
                    Throw New ND4VBIllegalStateException("Right shape contained value < 0 at index " & i & " - right shape " & Arrays.toString(right))
                End If
                i += 1
            Loop

            If left.Length > 1 AndAlso left(1) <> right(0) Then
                Throw New System.ArgumentException("Columns of left not equal to rows of right: left shape " & Arrays.toString(left) & ", right shape " & Arrays.toString(right))
            End If

            If left.Length < right.Length Then
                If left(0) = right(0) Then
                    Return New Integer() {1, right(1)}
                End If
            End If

            Dim shapeArr() As Integer = {left(0), right(1)}
            Return shapeArr
        End Function

        Public Shared Function getMatrixMultiplyShape(ByVal left() As Long, ByVal right() As Long) As Long()
            If ShapeT.shapeIsScalar(left) Then
                Return right
            End If

            If ShapeT.shapeIsScalar(right) Then
                Return left
            End If

            If left.Length <> 2 AndAlso right.Length <>2 Then
                If left.Length <> 3 AndAlso right.Length <> 3 Then
                    Throw New System.ArgumentException("Illegal shapes for matrix multiply. Must be both of length 2 or both" & "of length 3 (batch-wise matrix multiply). Left shape: " & Arrays.toString(left) & ", right shape: " & Arrays.toString(right))
                End If
            End If

            Dim i As Integer = 0
            Do While i < left.Length
                If left(i) < 1 Then
                    Throw New ND4VBIllegalStateException("Left shape contained value < 0 at index " & i & " - left shape " & Arrays.toString(left))
                End If
                i += 1
            Loop

            i = 0
            Do While i < right.Length
                If right(i) < 1 Then
                    Throw New ND4VBIllegalStateException("Right shape contained value < 0 at index " & i & " - right shape " & Arrays.toString(right))
                End If
                i += 1
            Loop

            If left.Length = 2 AndAlso left(1) <> right(0) OrElse left.Length = 3 AndAlso left(2) <> right(1) Then
                Throw New System.ArgumentException("Columns of left not equal to rows of right: left shape " & Arrays.toString(left) & ", right shape " & Arrays.toString(right))
            End If

            If left.Length < right.Length Then
                If left(0) = right(0) Then
                    Return New Long() {1, right(1)}
                End If
            End If

            If left.Length = 3 AndAlso left(0) <> right(0) Then
                Throw New System.ArgumentException("For batch matrix multiplication the leading dimension of both arguments" & "has to match, got left leading dimension" & left(0) & "and right " & right(0))
            End If

            Dim shapeArr() As Long
            If left.Length = 2 Then
                shapeArr = New Long(){left(0), right(1)}
            Else
                shapeArr = New Long(){left(0), left(1), right(2)}
            End If
            Return shapeArr
        End Function

        ''' <summary>
        ''' Create a copy of the matrix
        ''' where the new offset is zero
        ''' </summary>
        ''' <param name="arr"> the array to copy to offset 0 </param>
        ''' <returns> the same array if offset is zero
        ''' otherwise a copy of the array with
        ''' elements set to zero </returns>
        Public Shared Function toOffsetZero(ByVal arr As INDArray) As INDArray
            If arr.offset() < 1 AndAlso arr.data().length() = arr.length() Then
                If arr.ordering() = "f"c AndAlso arr.stride(-1) <> 1 OrElse arr.ordering() = "c"c AndAlso arr.stride(0) <> 1 Then
                    Return arr
                End If
            End If

            If arr.RowVector Then
                Dim rowCopy As INDArray = Nd4vbT.create(arr.shape())
                Dim i As Integer = 0
                Do While i < rowCopy.length()
                    rowCopy.putScalar(i, arr.getDouble(i))
                    i += 1
                Loop
                Return rowCopy
            End If

            Dim ret As INDArray = Nd4vbT.create(arr.shape(), arr.ordering())
            ret.assign(arr)
            Return ret
        End Function

        ''' <summary>
        ''' Create a copy of the ndarray where the new offset is zero
        ''' </summary>
        ''' <param name="arr"> the array to copy to offset 0 </param>
        ''' <returns> a copy of the array with elements set to zero offset </returns>
        Public Shared Function toOffsetZeroCopy(ByVal arr As INDArray) As INDArray
            Return toOffsetZeroCopyHelper(arr, Nd4vbT.order(), False)
        End Function

        ''' <summary>
        '''Create a copy of the ndarray where the new offset is zero, and has specified order </summary>
        ''' <param name="arr"> the array to copy to offset 0 </param>
        ''' <param name="order"> the order of the returned array </param>
        ''' <returns> a copy of the array with elements set to zero offset, and with specified order </returns>
        Public Shared Function toOffsetZeroCopy(ByVal arr As INDArray, ByVal order As Char) As INDArray
            Return toOffsetZeroCopyHelper(arr, order, False)
        End Function

        ''' <summary>
        ''' Create a copy of the ndarray where the new offset is zero.
        ''' Unlike toOffsetZeroCopy(INDArray) (which always returns arrays of order Nd4j.order()),
        ''' and toOffsetZeroCopy(INDArray,char) (which always returns arrays of a specified order)
        ''' this method returns NDArrays of any order (sometimes c, sometimes f).<br>
        ''' This method may be faster than the other two toOffsetZeroCopyAnyOrder methods as a result,
        ''' however no performance benefit (or cost) relative to them will be observed in many cases.
        ''' If a copy is necessary, the output will have order Nd4j.order() </summary>
        ''' <param name="arr"> NDArray to duplicate </param>
        ''' <returns> Copy with offset 0, but order might be c, or might be f </returns>
        Public Shared Function toOffsetZeroCopyAnyOrder(ByVal arr As INDArray) As INDArray
            Return toOffsetZeroCopyHelper(arr, Nd4vbT.order(), True)
        End Function

        Private Shared Function toOffsetZeroCopyHelper(ByVal arr As INDArray, ByVal order As Char, ByVal anyOrder As Boolean) As INDArray
            If arr.Empty Then
                Return arr 'Empty arrays are immutable, return as-is
            End If

            'Use CopyOp:
            Dim outOrder As Char = (If(anyOrder, arr.ordering(), order))
            If outOrder = "a"c Then
                outOrder = Nd4vbT.order()
            End If
            Dim z As INDArray = Nd4vbT.createUninitialized(arr.dataType(), arr.shape(), outOrder)
            z.assign(arr)
            Return z
        End Function

        ''' <summary>
        ''' Get a double based on the array and given indices
        ''' </summary>
        ''' <param name="arr">     the array to retrieve the double from </param>
        ''' <param name="indices"> the indices to iterate over </param>
        ''' <returns> the double at the specified index </returns>
        Public Shared Function getDouble(ByVal arr As INDArray, ByVal indices() As Integer) As Double
            Dim offset As Long = getOffset(arr.shapeInfoJava(), ArrayUtil.toLongArray(indices))
            Return arr.data().getDouble(offset)
        End Function

        Public Shared Function getDouble(ByVal arr As INDArray, ParamArray ByVal indices() As Long) As Double
            Dim offset As Long = getOffset(arr.shapeInfoJava(), indices)
            Return arr.data().getDouble(offset)
        End Function

        Public Shared Function getLong(ByVal arr As INDArray, ParamArray ByVal indices() As Long) As Long
            Dim offset As Long = getOffset(arr.shapeInfoJava(), indices)
            Return arr.data().getLong(offset)
        End Function

        ''' <summary>
        ''' Iterate over 2
        ''' coordinate spaces given 2 arrays </summary>
        ''' <param name="arr"> the first array </param>
        ''' <param name="coordinateFunction"> the coordinate function to use
        '''  </param>
        Public Shared Sub iterate(ByVal arr As INDArray, ByVal coordinateFunction As CoordinateFunction)
            ShapeT.iterate(0, arr.rank(), arr.shape(), New Long(arr.rank() - 1){}, coordinateFunction)
        End Sub

        ''' <summary>
        ''' Iterate over 2
        ''' coordinate spaces given 2 arrays </summary>
        ''' <param name="arr"> the first array </param>
        ''' <param name="arr2"> the second array </param>
        ''' <param name="coordinateFunction"> the coordinate function to use
        '''  </param>
        Public Shared Sub iterate(ByVal arr As INDArray, ByVal arr2 As INDArray, ByVal coordinateFunction As CoordinateFunction)
            ShapeT.iterate(0, arr.rank(), arr.shape(), New Long(arr.rank() - 1){}, 0, arr2.rank(), arr2.shape(), New Long(arr2.rank() - 1){}, coordinateFunction)
        End Sub

        ''' <summary>
        ''' Iterate over a pair of coordinates </summary>
        ''' <param name="dimension"> </param>
        ''' <param name="n"> </param>
        ''' <param name="size"> </param>
        ''' <param name="res"> </param>
        ''' <param name="dimension2"> </param>
        ''' <param name="n2"> </param>
        ''' <param name="size2"> </param>
        ''' <param name="res2"> </param>
        ''' <param name="func"> </param>
        Public Shared Sub iterate(ByVal dimension As Integer, ByVal n As Integer, ByVal size() As Integer, ByVal res() As Integer, ByVal dimension2 As Integer, ByVal n2 As Integer, ByVal size2() As Integer, ByVal res2() As Integer, ByVal func As CoordinateFunction)
            If dimension >= n OrElse dimension2 >= n2 Then
                ' stop clause
                func.process(ArrayUtil.toLongArray(res), ArrayUtil.toLongArray(res2))
                Return
            End If

            If size2.Length <> size.Length Then
                If dimension >= size.Length Then
                    Return
                End If
                Dim i As Integer = 0
                Do While i < size(dimension)
                    If dimension2 >= size2.Length Then
                        Exit Do
                    End If
                    Dim j As Integer = 0
                    Do While j < size2(dimension2)
                        res(dimension) = i
                        res2(dimension2) = j
                        iterate(dimension + 1, n, size, res, dimension2 + 1, n2, size2, res2, func)
                        j += 1
                    Loop

                    i += 1
                Loop
            Else
                If dimension >= size.Length Then
                    Return
                End If

                Dim i As Integer = 0
                Do While i < size(dimension)
                    Dim j As Integer = 0
                    Do While j < size2(dimension2)
                        If dimension2 >= size2.Length Then
                            Exit Do
                        End If
                        res(dimension) = i
                        res2(dimension2) = j
                        iterate(dimension + 1, n, size, res, dimension2 + 1, n2, size2, res2, func)
                        j += 1
                    Loop

                    i += 1
                Loop
            End If
        End Sub

        Public Shared Sub iterate(ByVal dimension As Integer, ByVal n As Integer, ByVal size() As Long, ByVal res() As Long, ByVal dimension2 As Integer, ByVal n2 As Integer, ByVal size2() As Long, ByVal res2() As Long, ByVal func As CoordinateFunction)
            If dimension >= n OrElse dimension2 >= n2 Then
                ' stop clause
                func.process(res, res2)
                Return
            End If

            If size2.Length <> size.Length Then
                If dimension >= size.Length Then
                    Return
                End If
                Dim i As Integer = 0
                Do While i < size(dimension)
                    If dimension2 >= size2.Length Then
                        Exit Do
                    End If
                    Dim j As Integer = 0
                    Do While j < size2(dimension2)
                        res(dimension) = i
                        res2(dimension2) = j
                        iterate(dimension + 1, n, size, res, dimension2 + 1, n2, size2, res2, func)
                        j += 1
                    Loop

                    i += 1
                Loop
            Else
                If dimension >= size.Length Then
                    Return
                End If

                Dim i As Integer = 0
                Do While i < size(dimension)
                    Dim j As Integer = 0
                    Do While j < size2(dimension2)
                        If dimension2 >= size2.Length Then
                            Exit Do
                        End If
                        res(dimension) = i
                        res2(dimension2) = j
                        iterate(dimension + 1, n, size, res, dimension2 + 1, n2, size2, res2, func)
                        j += 1
                    Loop

                    i += 1
                Loop
            End If
        End Sub

        ''' <summary>
        ''' Iterate over a pair of coordinates </summary>
        ''' <param name="dimension"> </param>
        ''' <param name="n"> </param>
        ''' <param name="size"> </param>
        Public Shared Sub iterate(ByVal dimension As Integer, ByVal n As Integer, ByVal size() As Integer, ByVal res() As Integer, ByVal func As CoordinateFunction)
            If dimension >= n Then 'stop clause
                func.process(ArrayUtil.toLongArray(res))
                Return
            End If
            Dim i As Integer = 0
            Do While i < size(dimension)
                res(dimension) = i
                iterate(dimension + 1, n, ArrayUtil.toLongArray(size), ArrayUtil.toLongArray(res), func)
                i += 1
            Loop
        End Sub

        Public Shared Sub iterate(ByVal dimension As Integer, ByVal n As Integer, ByVal size() As Long, ByVal res() As Long, ByVal func As CoordinateFunction)
            If dimension >= n Then 'stop clause
                func.process(res)
                Return
            End If
            Dim i As Integer = 0
            Do While i < size(dimension)
                res(dimension) = i
                iterate(dimension + 1, n, size, res, func)
                i += 1
            Loop
        End Sub

        ''' <summary>
        ''' Get an offset for retrieval
        ''' from a data buffer
        ''' based on the given
        ''' shape stride and given indices </summary>
        ''' <param name="baseOffset"> the offset to start from </param>
        ''' <param name="shape"> the shape of the array </param>
        ''' <param name="stride"> the stride of the array </param>
        ''' <param name="indices"> the indices to iterate over </param>
        ''' <returns> the double at the specified index </returns>
        Public Shared Function getOffset(ByVal baseOffset As Long, ByVal shapeArr() As Integer, ByVal stride() As Integer, ParamArray ByVal indices() As Integer) As Long
            'int ret =  mappers[shape.length].getOffset(baseOffset, shape, stride, indices);
            If shapeArr.Length <> stride.Length OrElse indices.Length <> shapeArr.Length Then
                Throw New System.ArgumentException("Indexes, shape, and stride must be the same length")
            End If
            Dim offset As Long = baseOffset
            For i As Integer = 0 To shapeArr.Length - 1
                If indices(i) >= shapeArr(i) Then
                    Throw New System.ArgumentException(String.Format("J: Index [{0:D}] must not be >= shape[{1:D}]={2:D}.", i, i, shapeArr(i)))
                End If
                If shapeArr(i) <> 1 Then
                    offset += indices(i) * stride(i)
                End If
            Next i

            Return offset
        End Function

        ''' <summary>
        ''' Get the offset of the specified indices from the shape info buffer
        ''' </summary>
        ''' <param name="shapeInformation">    Shape information to get the offset for </param>
        ''' <param name="indices">             Indices array to get the offset for (must be same length as array rank) </param>
        ''' <returns>                    Buffer offset fo the specified indices </returns>
    '    public static long getOffset(IntBuffer shapeInformation, int[] indices) {
    '        return getOffset(shapeInformation, ArrayUtil.toLongArray(indices));
    '    }
    '
    '    public static long getOffset(LongBuffer shapeInformation, int[] indices) {
    '        return getOffset(shapeInformation, ArrayUtil.toLongArray(indices));
    '    }

        ''' <summary>
        ''' Get the offset of the specified indices from the shape info buffer
        ''' </summary>
        ''' <param name="shapeInformation">    Shape information to get the offset for </param>
        ''' <param name="indices">             Indices array to get the offset for (must be same length as array rank) </param>
        ''' <returns>                    Buffer offset fo the specified indices </returns>
        <Obsolete>
        Public Shared Function getOffset(ByVal shapeInformation As DataBuffer, ByVal indices() As Integer) As Long
            Return getOffset(shapeInformation, ArrayUtil.toLongArray(indices))
        End Function
        Public Shared Function getOffset(ByVal shapeInformation As DataBuffer, ParamArray ByVal indices() As Long) As Long
            Dim rank As Integer = ShapeT.rank(shapeInformation)
            If indices.Length <> rank Then
                Throw New System.ArgumentException("Indexes must be same length as array rank")
            End If
            Dim offset As Long = 0
            For i As Integer = 0 To rank - 1
                Dim size_dimi As Integer = size(shapeInformation, i)
                If indices(i) > size_dimi Then
                    Throw New System.ArgumentException(String.Format("J: Index [{0:D}] must not be >= shape[{1:D}]={2:D}.", i, i, size_dimi))
                End If
                If size_dimi <> 1 Then
                    offset += indices(i) * stride(shapeInformation, i)
                End If
            Next i
            Return offset
        End Function

        Public Shared Function getOffset(ByVal shapeInformation() As Integer, ParamArray ByVal indices() As Integer) As Long
            Dim rank As Integer = ShapeT.rank(shapeInformation)
             Dim offset As Long = 0
            For i As Integer = 0 To SysMath.Min(rank,indices.Length) - 1
                Dim size_dimi As Integer = size(shapeInformation, i)
                If indices(i) > size_dimi Then
                    Throw New System.ArgumentException(String.Format("J: Index [{0:D}] must not be >= shape[{1:D}]={2:D}.", i, i, size_dimi))
                End If
                If size_dimi <> 1 Then
                    offset += indices(i) * stride(shapeInformation, i)
                End If
            Next i
            Return offset
        End Function

        Public Shared Function getOffset(ByVal shapeInformation() As Long, ParamArray ByVal indices() As Integer) As Long
            Dim rank As Integer = ShapeT.rank(shapeInformation)
            Dim offset As Long = 0
            For i As Integer = 0 To SysMath.Min(rank,indices.Length) - 1
                Dim size_dimi As Long = size(shapeInformation, i)
                If indices(i) > size_dimi Then
                    Throw New System.ArgumentException(String.Format("J: Index [{0:D}] must not be >= shape[{1:D}]={2:D}.", i, i, size_dimi))
                End If
                If size_dimi <> 1 Then
                    offset += indices(i) * stride(shapeInformation, i)
                End If
            Next i
            Return offset
        End Function

        Public Shared Function getOffset(ByVal shapeInformation() As Long, ParamArray ByVal indices() As Long) As Long
            Dim rank As Integer = ShapeT.rank(shapeInformation)
            Dim offset As Long = 0
            For i As Integer = 0 To SysMath.Min(rank,indices.Length) - 1
                Dim size_dimi As Long = size(shapeInformation, i)
                If indices(i) > size_dimi Then
                    Throw New System.ArgumentException(String.Format("J: Index [{0:D}] must not be >= shape[{1:D}]={2:D}.", i, i, size_dimi))
                End If
                If size_dimi <> 1 Then
                    offset += indices(i) * stride(shapeInformation, i)
                End If
            Next i
            Return offset
        End Function

        ''' <summary>
        ''' Get the offset of the specified [row,col] for the 2d array
        ''' </summary>
        ''' <param name="shapeInformation">    Shape information </param>
        ''' <param name="row">                 Row index to get the offset for </param>
        ''' <param name="col">                 Column index to get the offset for </param>
        ''' <returns>                    Buffer offset </returns>
        Public Shared Function getOffset(ByVal shapeInformation As DataBuffer, ByVal row As Integer, ByVal col As Integer) As Long
            Dim rank As Integer = ShapeT.rank(shapeInformation)
            If rank <> 2 Then
                Throw New System.ArgumentException("Cannot use this getOffset method on arrays of rank != 2 (rank is: " & rank & ")")
            End If
            Return getOffsetUnsafe(shapeInformation, row, col)
        End Function

        ''' <summary>
        ''' Identical to <seealso cref="Shape.getOffset(DataBuffer, Integer, Integer)"/> but without input validation on array rank
        ''' </summary>
        Public Shared Function getOffsetUnsafe(ByVal shapeInformation As DataBuffer, ByVal row As Integer, ByVal col As Integer) As Long
            Dim offset As Long = 0
            Dim size_0 As Integer = sizeUnsafe(shapeInformation, 0)
            Dim size_1 As Integer = sizeUnsafe(shapeInformation, 1)
            If row >= size_0 OrElse col >= size_1 Then
                Throw New System.ArgumentException("Invalid indices: cannot get [" & row & "," & col & "] from a " & Arrays.toString(shape(shapeInformation)) & " NDArray")
            End If

            If size_0 <> 1 Then
                offset += row * strideUnsafe(shapeInformation, 0, 2)
            End If
            If size_1 <> 1 Then
                offset += col * strideUnsafe(shapeInformation, 1, 2)
            End If

            Return offset
        End Function

        Public Shared Function getOffsetUnsafe(ByVal shapeInformation() As Integer, ByVal row As Integer, ByVal col As Integer) As Long
            Dim offset As Long = 0
            Dim size_0 As Integer = sizeUnsafe(shapeInformation, 0)
            Dim size_1 As Integer = sizeUnsafe(shapeInformation, 1)
            If row >= size_0 OrElse col >= size_1 AndAlso Not ShapeT.isVector(ShapeT.shape(shapeInformation)) AndAlso Not ShapeT.shapeIsScalar(ShapeT.shape(shapeInformation)) Then
                Throw New System.ArgumentException("Invalid indices: cannot get [" & row & "," & col & "] from a " & Arrays.toString(shape(shapeInformation)) & " NDArray")
            End If

            If size_0 <> 1 Then
                offset += row * strideUnsafe(shapeInformation, 0, 2)
            End If
            If size_1 <> 1 Then
                offset += col * strideUnsafe(shapeInformation, 1, 2)
            End If

            Return offset
        End Function

        Public Shared Function getOffsetUnsafe(ByVal shapeInformation() As Long, ByVal row As Long, ByVal col As Long) As Long
            Dim offset As Long = 0
            Dim size_0 As Long = sizeUnsafe(shapeInformation, 0)
            Dim size_1 As Long = sizeUnsafe(shapeInformation, 1)
            If row >= size_0 OrElse col >= size_1 AndAlso Not ShapeT.isVector(ShapeT.shape(shapeInformation)) AndAlso Not ShapeT.shapeIsScalar(ShapeT.shape(shapeInformation)) Then
                Throw New System.ArgumentException("Invalid indices: cannot get [" & row & "," & col & "] from a " & Arrays.toString(shape(shapeInformation)) & " NDArray")
            End If

            If size_0 <> 1 Then
                offset += row * strideUnsafe(shapeInformation, 0, 2)
            End If
            If size_1 <> 1 Then
                offset += col * strideUnsafe(shapeInformation, 1, 2)
            End If

            Return offset
        End Function

        ''' <summary>
        ''' Get the offset of the specified [dim0,dim1,dim2] for the 3d array
        ''' </summary>
        ''' <param name="shapeInformation">    Shape information </param>
        ''' <param name="dim0">                Row index to get the offset for </param>
        ''' <param name="dim1">                Column index to get the offset for </param>
        ''' <param name="dim2">                dimension 2 index to get the offset for </param>
        ''' <returns>                    Buffer offset </returns>
        Public Shared Function getOffset(ByVal shapeInformation As DataBuffer, ByVal dim0 As Integer, ByVal dim1 As Integer, ByVal dim2 As Integer) As Long
            Dim rank As Integer = ShapeT.rank(shapeInformation)
            If rank <> 3 Then
                Throw New System.ArgumentException("Cannot use this getOffset method on arrays of rank != 3 (rank is: " & rank & ")")
            End If
            Return getOffsetUnsafe(shapeInformation, dim0, dim1, dim2)
        End Function

        ''' <summary>
        ''' Identical to <seealso cref="Shape.getOffset(DataBuffer, Integer, Integer, Integer)"/> but without input validation on array rank
        ''' </summary>
        Public Shared Function getOffsetUnsafe(ByVal shapeInformation As DataBuffer, ByVal dim0 As Integer, ByVal dim1 As Integer, ByVal dim2 As Integer) As Long
            Dim offset As Long = 0
            Dim size_0 As Integer = sizeUnsafe(shapeInformation, 0)
            Dim size_1 As Integer = sizeUnsafe(shapeInformation, 1)
            Dim size_2 As Integer = sizeUnsafe(shapeInformation, 2)
            If dim0 >= size_0 OrElse dim1 >= size_1 OrElse dim2 >= size_2 Then
                Throw New System.ArgumentException("Invalid indices: cannot get [" & dim0 & "," & dim1 & "," & dim2 & "] from a " & Arrays.toString(shape(shapeInformation)) & " NDArray")
            End If

            If size_0 <> 1 Then
                offset += dim0 * strideUnsafe(shapeInformation, 0, 3)
            End If
            If size_1 <> 1 Then
                offset += dim1 * strideUnsafe(shapeInformation, 1, 3)
            End If
            If size_2 <> 1 Then
                offset += dim2 * strideUnsafe(shapeInformation, 2, 3)
            End If

            Return offset
        End Function

        Public Shared Function getOffsetUnsafe(ByVal shapeInformation() As Integer, ByVal dim0 As Integer, ByVal dim1 As Integer, ByVal dim2 As Integer) As Long
            Dim offset As Integer = 0
            Dim size_0 As Integer = sizeUnsafe(shapeInformation, 0)
            Dim size_1 As Integer = sizeUnsafe(shapeInformation, 1)
            Dim size_2 As Integer = sizeUnsafe(shapeInformation, 2)
            If dim0 >= size_0 OrElse dim1 >= size_1 OrElse dim2 >= size_2 Then
                Throw New System.ArgumentException("Invalid indices: cannot get [" & dim0 & "," & dim1 & "," & dim2 & "] from a " & Arrays.toString(shapeInformation) & " NDArray")
            End If

            If size_0 <> 1 Then
                offset += dim0 * strideUnsafe(shapeInformation, 0, 3)
            End If
            If size_1 <> 1 Then
                offset += dim1 * strideUnsafe(shapeInformation, 1, 3)
            End If
            If size_2 <> 1 Then
                offset += dim2 * strideUnsafe(shapeInformation, 2, 3)
            End If

            Return offset
        End Function

        ''' <summary>
        ''' Get the offset of the specified [dim0,dim1,dim2,dim3] for the 4d array
        ''' </summary>
        ''' <param name="shapeInformation">    Shape information </param>
        ''' <param name="dim0">                Row index to get the offset for </param>
        ''' <param name="dim1">                Column index to get the offset for </param>
        ''' <param name="dim2">                dimension 2 index to get the offset for </param>
        ''' <param name="dim3">                dimension 3 index to get the offset for </param>
        ''' <returns>                    Buffer offset </returns>
        Public Shared Function getOffset(ByVal shapeInformation As DataBuffer, ByVal dim0 As Integer, ByVal dim1 As Integer, ByVal dim2 As Integer, ByVal dim3 As Integer) As Long
            Dim rank As Integer = ShapeT.rank(shapeInformation)
            If rank <> 4 Then
                Throw New System.ArgumentException("Cannot use this getOffset method on arrays of rank != 4 (rank is: " & rank & ")")
            End If
            Return getOffsetUnsafe(shapeInformation, dim0, dim1, dim2, dim3)
        End Function

        Public Shared Function getOffsetUnsafe(ByVal shapeInformation As DataBuffer, ByVal dim0 As Integer, ByVal dim1 As Integer, ByVal dim2 As Integer, ByVal dim3 As Integer) As Long
            Dim offset As Long = 0
            Dim size_0 As Integer = sizeUnsafe(shapeInformation, 0)
            Dim size_1 As Integer = sizeUnsafe(shapeInformation, 1)
            Dim size_2 As Integer = sizeUnsafe(shapeInformation, 2)
            Dim size_3 As Integer = sizeUnsafe(shapeInformation, 3)
            If dim0 >= size_0 OrElse dim1 >= size_1 OrElse dim2 >= size_2 OrElse dim3 >= size_3 Then
                Throw New System.ArgumentException("Invalid indices: cannot get [" & dim0 & "," & dim1 & "," & dim2 & "," & dim3 & "] from a " & Arrays.toString(shape(shapeInformation)) & " NDArray")
            End If

            If size_0 <> 1 Then
                offset += dim0 * strideUnsafe(shapeInformation, 0, 4)
            End If
            If size_1 <> 1 Then
                offset += dim1 * strideUnsafe(shapeInformation, 1, 4)
            End If
            If size_2 <> 1 Then
                offset += dim2 * strideUnsafe(shapeInformation, 2, 4)
            End If
            If size_3 <> 1 Then
                offset += dim3 * strideUnsafe(shapeInformation, 3, 4)
            End If

            Return offset
        End Function

        Public Shared Function getOffsetUnsafe(ByVal shapeInformation() As Integer, ByVal dim0 As Integer, ByVal dim1 As Integer, ByVal dim2 As Integer, ByVal dim3 As Integer) As Long
            Dim offset As Long = 0
            Dim size_0 As Integer = sizeUnsafe(shapeInformation, 0)
            Dim size_1 As Integer = sizeUnsafe(shapeInformation, 1)
            Dim size_2 As Integer = sizeUnsafe(shapeInformation, 2)
            Dim size_3 As Integer = sizeUnsafe(shapeInformation, 3)
            If dim0 >= size_0 OrElse dim1 >= size_1 OrElse dim2 >= size_2 OrElse dim3 >= size_3 Then
                Throw New System.ArgumentException("Invalid indices: cannot get [" & dim0 & "," & dim1 & "," & dim2 & "," & dim3 & "] from a " & Arrays.toString(shape(shapeInformation)) & " NDArray")
            End If

            If size_0 <> 1 Then
                offset += dim0 * strideUnsafe(shapeInformation, 0, 4)
            End If
            If size_1 <> 1 Then
                offset += dim1 * strideUnsafe(shapeInformation, 1, 4)
            End If
            If size_2 <> 1 Then
                offset += dim2 * strideUnsafe(shapeInformation, 2, 4)
            End If
            If size_3 <> 1 Then
                offset += dim3 * strideUnsafe(shapeInformation, 3, 4)
            End If

            Return offset
        End Function

        Public Shared Function getOffsetUnsafe(ByVal shapeInformation() As Long, ByVal dim0 As Long, ByVal dim1 As Long, ByVal dim2 As Long, ByVal dim3 As Long) As Long
            Dim offset As Long = 0
            Dim size_0 As Long = sizeUnsafe(shapeInformation, 0)
            Dim size_1 As Long = sizeUnsafe(shapeInformation, 1)
            Dim size_2 As Long = sizeUnsafe(shapeInformation, 2)
            Dim size_3 As Long = sizeUnsafe(shapeInformation, 3)
            If dim0 >= size_0 OrElse dim1 >= size_1 OrElse dim2 >= size_2 OrElse dim3 >= size_3 Then
                Throw New System.ArgumentException("Invalid indices: cannot get [" & dim0 & "," & dim1 & "," & dim2 & "," & dim3 & "] from a " & Arrays.toString(shape(shapeInformation)) & " NDArray")
            End If

            If size_0 <> 1 Then
                offset += dim0 * strideUnsafe(shapeInformation, 0, 4)
            End If
            If size_1 <> 1 Then
                offset += dim1 * strideUnsafe(shapeInformation, 1, 4)
            End If
            If size_2 <> 1 Then
                offset += dim2 * strideUnsafe(shapeInformation, 2, 4)
            End If
            If size_3 <> 1 Then
                offset += dim3 * strideUnsafe(shapeInformation, 3, 4)
            End If

            Return offset
        End Function

        ''' <summary>
        ''' Output an int array for a particular dimension </summary>
        ''' <param name="axes"> the axes </param>
        ''' <param name="shape"> the current shape
        ''' @return </param>
        Public Shared Function sizeForAxes(ByVal axes() As Integer, ByVal shapeArr() As Integer) As Integer()
            Dim ret(shapeArr.Length - 1) As Integer
            For i As Integer = 0 To axes.Length - 1
                ret(i) = shapeArr(axes(i))
            Next i
            Return ret
        End Function

        ''' <summary>
        ''' Returns whether the given shape is a vector
        ''' </summary>
        ''' <param name="shapeInfo"> the shapeinfo to test </param>
        ''' <returns> whether the given shape is a vector </returns>
        Public Shared Function isVector(ByVal shapeInfo As DataBuffer) As Boolean
            Dim rank As Integer = ShapeT.rank(shapeInfo)
            If rank > 2 OrElse rank < 1 Then
                Return False
            Else
                Dim len As Long = ShapeT.length(shapeInfo)
                Dim shapeArr As DataBuffer = ShapeT.shapeOf(shapeInfo)
                Return shapeArr.getInt(0) = len OrElse shapeArr.getInt(1) = len
            End If
        End Function

        ''' <summary>
        ''' Returns whether the given shape is a vector
        ''' </summary>
        ''' <param name="shape"> the shape to test </param>
        ''' <returns> whether the given shape is a vector </returns>
        Public Shared Function isVector(ByVal shapeArr() As Integer) As Boolean
            If shapeArr.Length > 2 OrElse shapeArr.Length < 1 Then
                Return False
            Else
                Dim len As Long = ArrayUtil.prodLong(shapeArr)
                Return shapeArr(0) = len OrElse shapeArr(1) = len
            End If
        End Function

        Public Shared Function isVector(ByVal shapeArr() As Long) As Boolean
            If shapeArr.Length > 2 OrElse shapeArr.Length < 1 Then
                Return False
            Else
                Dim len As Long = ArrayUtil.prodLong(shapeArr)
                Return shapeArr(0) = len OrElse shapeArr(1) = len
            End If
        End Function

        ''' <summary>
        ''' Returns whether the passed in shape is a matrix
        ''' </summary>
        ''' <param name="shapeInfo"> whether the passed in shape is a matrix </param>
        ''' <returns> true if the shape is a matrix false otherwise </returns>
        Public Shared Function isMatrix(ByVal shapeInfo As DataBuffer) As Boolean
            Dim rank As Integer = ShapeT.rank(shapeInfo)
            If rank <> 2 Then
                Return False
            End If
            Return Not isVector(shapeInfo)
        End Function

        ''' <summary>
        ''' Returns whether the passed in shape is a matrix
        ''' </summary>
        ''' <param name="shape"> whether the passed in shape is a matrix </param>
        ''' <returns> true if the shape is a matrix false otherwise </returns>
        Public Shared Function isMatrix(ByVal shapeArr() As Integer) As Boolean
            If shapeArr.Length <> 2 Then
                Return False
            End If
            Return Not isVector(shapeArr)
        End Function

        Public Shared Function isMatrix(ByVal shapeArr() As Long) As Boolean
            If shapeArr.Length <> 2 Then
                Return False
            End If
            Return Not isVector(shapeArr)
        End Function

        ''' <summary>
        ''' Gets rid of any singleton dimensions of the given array
        ''' </summary>
        ''' <param name="shape"> the shape to squeeze </param>
        ''' <returns> the array with all of the singleton dimensions removed </returns>
        Public Shared Function squeeze(ByVal shapeArr() As Integer) As Integer()
            If isColumnVectorShape(shapeArr) Then
                Return shapeArr
            End If

            Dim ret As IList(Of Integer) = New List(Of Integer)()

            'strip all but last dimension
            For i As Integer = 0 To shapeArr.Length - 1
                If shapeArr(i) <> 1 Then
                    ret.Add(shapeArr(i))
                End If
            Next i
            Return ArrayUtil.toArray(ret)
        End Function

        ''' <summary>
        ''' Gets rid of any singleton dimensions of the given array
        ''' </summary>
        ''' <param name="shape"> the shape to squeeze </param>
        ''' <returns> the array with all of the singleton dimensions removed </returns>
        Public Shared Function squeeze(ByVal shapeArr() As Long) As Long()
            If isColumnVectorShape(shapeArr) Then
                Return shapeArr
            End If

            Dim ret As IList(Of Long) = New List(Of Long)()

            'strip all but last dimension
            For i As Integer = 0 To shapeArr.Length - 1
                If shapeArr(i) <> 1 Then
                    ret.Add(shapeArr(i))
                End If
            Next i
            Return ArrayUtil.toArrayLong(ret)
        End Function

        ''' <summary>
        ''' Return true if the shapes are equal after removing any size 1 dimensions
        ''' For example, [1,3,4] and [3,4] are considered equal by this method.
        ''' Or [2,1,1] and [1,2] are considered equal. </summary>
        ''' <param name="shape1"> First shape </param>
        ''' <param name="shape2"> Second shape
        ''' @return </param>
        Public Shared Function shapeEqualWithSqueeze(ByVal shape1() As Long, ByVal shape2() As Long) As Boolean
            If shape1 Is Nothing Then
                Return shape2 Is Nothing
            End If
            If shape2 Is Nothing Then
                Return False 'Shape 1 must be non-null by this point
            End If
            If shape1.Length = 0 AndAlso shape2.Length = 0 Then
                Return True
            End If

            Dim pos1 As Integer = 0
            Dim pos2 As Integer = 0
            Do While pos1 < shape1.Length AndAlso pos2 < shape2.Length
                If shape1(pos1) = 1 Then
                    pos1 += 1
                    Continue Do
                End If
                If shape2(pos2) = 1 Then
                    pos2 += 1
                    Continue Do
                End If
                'Both are non-1 shape. Must be equal
                If shape1(pos1) <> shape2(pos2) Then
                    Return False
                End If
                pos1 += 1
                pos2 += 1
            Loop
            'Handle trailing 1s
            Do While pos1 < shape1.Length AndAlso shape1(pos1) = 1
                pos1 += 1
            Loop
            Do While pos2 < shape2.Length AndAlso shape2(pos2) = 1
                pos2 += 1
            Loop

            '2 possibilities: all entries consumed -> same shape. Or some remaining - something like [2] vs. [2,3,4,5]
            Return pos1 = shape1.Length AndAlso pos2 = shape2.Length
        End Function

        ''' <summary>
        ''' Returns whether 2 shapes are equals by checking for dimension semantics
        ''' as well as array equality
        ''' </summary>
        ''' <param name="shape1"> the first shape for comparison </param>
        ''' <param name="shape2"> the second shape for comparison </param>
        ''' <returns> whether the shapes are equivalent </returns>
        Public Shared Function shapeEquals(ByVal shape1() As Integer, ByVal shape2() As Integer) As Boolean
            If isColumnVectorShape(shape1) AndAlso isColumnVectorShape(shape2) Then
                Return shape1.SequenceEqual(shape2)
            End If

            If isRowVectorShape(shape1) AndAlso isRowVectorShape(shape2) Then
                Dim shape1Comp() As Integer = squeeze(shape1)
                Dim shape2Comp() As Integer = squeeze(shape2)
                Return shape1Comp.SequenceEqual(shape2Comp)
            End If

            'scalars
            If shape1.Length = 0 OrElse shape2.Length = 0 Then
                If shape1.Length = 0 AndAlso shapeIsScalar(shape2) Then
                    Return True
                End If

                If shape2.Length = 0 AndAlso shapeIsScalar(shape1) Then
                    Return True
                End If
            End If

            shape1 = squeeze(shape1)
            shape2 = squeeze(shape2)

            Return scalarEquals(shape1, shape2) OrElse shape1.SequenceEqual(shape2)
        End Function

        ''' <summary>
        ''' Returns whether 2 shapes are equals by checking for dimension semantics
        ''' as well as array equality
        ''' </summary>
        ''' <param name="shape1"> the first shape for comparison </param>
        ''' <param name="shape2"> the second shape for comparison </param>
        ''' <returns> whether the shapes are equivalent </returns>
        Public Shared Function shapeEquals(ByVal shape1() As Long, ByVal shape2() As Long) As Boolean
            If isColumnVectorShape(shape1) AndAlso isColumnVectorShape(shape2) Then
                Return shape1.SequenceEqual(shape2)
            End If

            If isRowVectorShape(shape1) AndAlso isRowVectorShape(shape2) Then
                Dim shape1Comp() As Long = squeeze(shape1)
                Dim shape2Comp() As Long = squeeze(shape2)
                Return shape1Comp.SequenceEqual(shape2Comp)
            End If

            'scalars
            If shape1.Length = 0 OrElse shape2.Length = 0 Then
                If shape1.Length = 0 AndAlso shapeIsScalar(shape2) Then
                    Return True
                End If

                If shape2.Length = 0 AndAlso shapeIsScalar(shape1) Then
                    Return True
                End If
            End If

            shape1 = squeeze(shape1)
            shape2 = squeeze(shape2)

            Return scalarEquals(shape1, shape2) OrElse shape1.SequenceEqual(shape2)
        End Function

        ''' <summary>
        ''' Returns true if the given shapes are both scalars (0 dimension or shape[0] == 1)
        ''' </summary>
        ''' <param name="shape1"> the first shape for comparison </param>
        ''' <param name="shape2"> the second shape for comparison </param>
        ''' <returns> whether the 2 shapes are equal based on scalar rules </returns>
        Public Shared Function scalarEquals(ByVal shape1() As Integer, ByVal shape2() As Integer) As Boolean
            If shape1.Length = 0 AndAlso shape2.Length = 1 AndAlso shape2(0) = 1 Then
                Return True
            ElseIf shape2.Length = 0 AndAlso shape1.Length = 1 AndAlso shape1(0) = 1 Then
                Return True
            End If

            Return False
        End Function

        Public Shared Function scalarEquals(ByVal shape1() As Long, ByVal shape2() As Long) As Boolean
            If shape1.Length = 0 AndAlso shape2.Length = 1 AndAlso shape2(0) = 1 Then
                Return True
            ElseIf shape2.Length = 0 AndAlso shape1.Length = 1 AndAlso shape1(0) = 1 Then
                Return True
            End If

            Return False
        End Function

        ''' <summary>
        ''' Returns true if the given shape is of length 1
        ''' or provided the shape length is 2:
        ''' element 0 is 1 </summary>
        ''' <param name="shapeInfo"> the shape info to check </param>
        ''' <returns> true if the above conditions hold,false otherwise </returns>
        Public Shared Function isRowVectorShape(ByVal shapeInfo As DataBuffer) As Boolean
            Dim rank As Integer = ShapeT.rank(shapeInfo)
            Dim shapeArr As DataBuffer = ShapeT.shapeOf(shapeInfo)
            Return (rank = 2 AndAlso shapeArr.getInt(0) = 1) OrElse rank = 1

        End Function

        ''' <summary>
        ''' Returns true if the given shape is of length 1
        ''' or provided the shape length is 2:
        ''' element 0 is 1 </summary>
        ''' <param name="shape"> the shape to check </param>
        ''' <returns> true if the above conditions hold,false otherwise </returns>
        Public Shared Function isRowVectorShape(ByVal shapeArr() As Integer) As Boolean
            Return (shapeArr.Length = 2 AndAlso shapeArr(0) = 1) OrElse shapeArr.Length = 1
        End Function

        Public Shared Function isRowVectorShape(ByVal shapeArr() As Long) As Boolean
            Return (shapeArr.Length = 2 AndAlso shapeArr(0) = 1) OrElse shapeArr.Length = 1
        End Function

        ''' <summary>
        ''' Returns true if the given shape is length 2 and
        ''' the size at element 1 is 1 </summary>
        ''' <param name="shape"> the shape to check </param>
        ''' <returns> true if the above listed conditions
        ''' hold false otherwise </returns>
        Public Shared Function isColumnVectorShape(ByVal shapeArr() As Integer) As Boolean
            Return (shapeArr.Length = 2 AndAlso shapeArr(1) = 1)
        End Function

        ''' <summary>
        ''' Returns true if the given shape length is 2
        ''' and the size at element 1 is 1 </summary>
        ''' <param name="shape">
        ''' @return </param>
        Public Shared Function isColumnVectorShape(ByVal shapeArr() As Long) As Boolean
            Return (shapeArr.Length = 2 AndAlso shapeArr(1) = 1)
        End Function

        ''' <summary>
        ''' If a shape array is ony 1 in length
        ''' it returns a row vector </summary>
        ''' <param name="shape"> the shape of the array </param>
        ''' <returns> the shape as is if its already >= 2 in length
        ''' otherwise a row vector shape </returns>
        Public Shared Function ensureAtMinRowVector(ParamArray ByVal shapeArr() As Integer) As Integer()
            If shapeArr.Length >= 2 Then
                Return shapeArr
            End If
            Return New Integer() {1, shapeArr(0)}
        End Function

        Public Shared Function getTADLength(ByVal shapeArr() As Integer, ParamArray ByVal dimensions() As Integer) As Long
            Dim tadLength As Integer = 1
            For i As Integer = 0 To dimensions.Length - 1
                tadLength *= shapeArr(dimensions(i))
            Next i

            Return tadLength
        End Function

        Public Shared Function getTADLength(ByVal shapeArr() As Long, ParamArray ByVal dimensions() As Integer) As Long
            Dim tadLength As Integer = 1
            For i As Integer = 0 To dimensions.Length - 1
                tadLength *= shapeArr(dimensions(i))
            Next i

            Return tadLength
        End Function

        '''
        ''' <param name="shape"> </param>
        ''' <param name="stride"> </param>
        ''' <param name="isFOrder">
        ''' @return </param>
        Public Shared Function elementWiseStride(ByVal shapeArr() As Integer, ByVal stride() As Integer, ByVal isFOrder As Boolean) As Integer
            ' 0D edge case
            If shapeArr.Length = 0 AndAlso stride.Length = 0 Then
                Return 1
            End If

            If shapeArr.Length = 1 AndAlso stride.Length = 1 Then
                Return 1
            End If

            Dim oldnd As Integer
            Dim olddims() As Integer = ArrayUtil.copy(shapeArr)
            Dim oldstrides() As Integer = ArrayUtil.copy(stride)
            Dim np, op, last_stride As Long
            Dim oi, oj, ok, ni, nj, nk As Integer
            Dim newStrides(stride.Length - 1) As Long
            oldnd = 0
            'set the shape to be 1 x length
            Dim newShapeRank As Integer = 2
            Dim newShape(shapeArr.Length - 1) As Long
            newShape(0) = 1
            newShape(1) = ArrayUtil.prodLong(shapeArr)

    '
    '         * Remove axes with dimension 1 from the old array. They have no effect
    '         * but would need special cases since their strides do not matter.
    '
            For oi = 0 To shapeArr.Length - 1
                If shapeArr(oi) <> 1 Then
                    olddims(oldnd) = shapeArr(oi)
                    oldstrides(oldnd) = stride(oi)
                    oldnd += 1
                End If
            Next oi

            np = 1
            For ni = 0 To newShapeRank - 1
                np *= newShape(ni)
            Next ni
            op = 1
            For oi = 0 To oldnd - 1
                op *= olddims(oi)
            Next oi
            If np <> op Then
                ' different total sizes; no hope
                Return 0
            End If

            If np = 0 Then
                ' the current code does not handle 0-sized arrays, so give up
                Return 0
            End If

            ' oi to oj and ni to nj give the axis ranges currently worked with
            oi = 0
            oj = 1
            ni = 0
            nj = 1
            Do While ni < newShapeRank AndAlso oi < oldnd
                np = newShape(ni)
                op = olddims(oi)

                Do While np <> op
                    If np < op Then
                        ' Misses trailing 1s, these are handled later
                        np *= newShape(nj)
                            nj += 1
                    Else
                        op *= olddims(oj)
                            oj += 1
                    End If
                Loop

                ' Check whether the original axes can be combined
                ok = oi
                Do While ok < oj - 1
                    If isFOrder Then
                        If oldstrides(ok + 1) <> olddims(ok) * oldstrides(ok) Then
                            ' not contiguous enough
                            Return 0
                        End If
                    Else
                        ' C order
                        If oldstrides(ok) <> olddims(ok + 1) * oldstrides(ok + 1) Then
                            ' not contiguous enough
                            Return 0
                        End If
                    End If
                    ok += 1
                Loop

                ' Calculate new strides for all axes currently worked with
                If isFOrder Then
                    newStrides(ni) = oldstrides(oi)
                    For nk = ni + 1 To nj - 1
                        newStrides(nk) = newStrides(nk - 1) * newShape(nk - 1)
                    Next nk
                Else
                    ' C order
                    newStrides(nj - 1) = oldstrides(oj - 1)
                    For nk = nj - 1 To ni + 1 Step -1
                        newStrides(nk - 1) = newStrides(nk) * newShape(nk)
                    Next nk
                End If
                ni = nj
                    nj += 1
                oi = oj
                    oj += 1
            Loop

    '
    '         * Set strides corresponding to trailing 1s of the new shape.
    '
            If ni >= 1 Then
                last_stride = newStrides(ni - 1)
            Else
                last_stride = stride(shapeArr.Length - 1)
            End If
            If isFOrder AndAlso ni >= 1 Then
                last_stride *= newShape(ni - 1)
            End If
            For nk = ni To newShapeRank - 1
                newStrides(nk) = last_stride
            Next nk
            If newStrides(newShapeRank - 1) >= Integer.MaxValue Then
                Throw New System.ArgumentException("Element size can not be >= Integer.MAX_VALUE")
            End If
            'returns the last element of the new stride array
            Return CInt(newStrides(newShapeRank - 1))
        End Function

        Public Shared Function elementWiseStride(ByVal shapeArr() As Long, ByVal stride() As Long, ByVal isFOrder As Boolean) As Long
            ' 0D edge case
            If shapeArr.Length = 0 AndAlso stride.Length = 0 Then
                Return 1
            End If

            If shapeArr.Length = 1 AndAlso stride.Length = 1 Then
                Return stride(0)
            End If

            Dim oldnd As Integer
            Dim olddims() As Long = ArrayUtil.copy(shapeArr)
            Dim oldstrides() As Long = ArrayUtil.copy(stride)
            Dim np, op, last_stride As Long
            Dim oi, oj, ok, ni, nj, nk As Integer
            Dim newStrides(stride.Length - 1) As Long
            oldnd = 0
            'set the shape to be 1 x length
            Dim newShapeRank As Integer = 2
            Dim newShape(shapeArr.Length - 1) As Long
            newShape(0) = 1
            newShape(1) = ArrayUtil.prodLong(shapeArr)

    '
    '         * Remove axes with dimension 1 from the old array. They have no effect
    '         * but would need special cases since their strides do not matter.
    '
            For oi = 0 To shapeArr.Length - 1
                If shapeArr(oi) <> 1 Then
                    olddims(oldnd) = shapeArr(oi)
                    oldstrides(oldnd) = stride(oi)
                    oldnd += 1
                End If
            Next oi

            np = 1
            For ni = 0 To newShapeRank - 1
                np *= newShape(ni)
            Next ni
            op = 1
            For oi = 0 To oldnd - 1
                op *= olddims(oi)
            Next oi
            If np <> op Then
                ' different total sizes; no hope
                Return 0
            End If

            If np = 0 Then
                ' the current code does not handle 0-sized arrays, so give up
                Return 0
            End If

            ' oi to oj and ni to nj give the axis ranges currently worked with
            oi = 0
            oj = 1
            ni = 0
            nj = 1
            Do While ni < newShapeRank AndAlso oi < oldnd
                np = newShape(ni)
                op = olddims(oi)

                Do While np <> op
                    If np < op Then
                        ' Misses trailing 1s, these are handled later
                        np *= newShape(nj)
                            nj += 1
                    Else
                        op *= olddims(oj)
                            oj += 1
                    End If
                Loop

                ' Check whether the original axes can be combined
                ok = oi
                Do While ok < oj - 1
                    If isFOrder Then
                        If oldstrides(ok + 1) <> olddims(ok) * oldstrides(ok) Then
                            ' not contiguous enough
                            Return 0
                        End If
                    Else
                        ' C order
                        If oldstrides(ok) <> olddims(ok + 1) * oldstrides(ok + 1) Then
                            ' not contiguous enough
                            Return 0
                        End If
                    End If
                    ok += 1
                Loop

                ' Calculate new strides for all axes currently worked with
                If isFOrder Then
                    newStrides(ni) = oldstrides(oi)
                    For nk = ni + 1 To nj - 1
                        newStrides(nk) = newStrides(nk - 1) * newShape(nk - 1)
                    Next nk
                Else
                    ' C order
                    newStrides(nj - 1) = oldstrides(oj - 1)
                    For nk = nj - 1 To ni + 1 Step -1
                        newStrides(nk - 1) = newStrides(nk) * newShape(nk)
                    Next nk
                End If
                ni = nj
                    nj += 1
                oi = oj
                    oj += 1
            Loop

    '
    '         * Set strides corresponding to trailing 1s of the new shape.
    '
            If ni >= 1 Then
                last_stride = newStrides(ni - 1)
            Else
                last_stride = stride(shapeArr.Length - 1)
            End If
            If isFOrder AndAlso ni >= 1 Then
                last_stride *= newShape(ni - 1)
            End If
            For nk = ni To newShapeRank - 1
                newStrides(nk) = last_stride
            Next nk
            If newStrides(newShapeRank - 1) >= Integer.MaxValue Then
                Throw New System.ArgumentException("Element size can not be >= Integer.MAX_VALUE")
            End If
            'returns the last element of the new stride array
            Return newStrides(newShapeRank - 1)
        End Function

        Public Shared Function newShapeNoCopy(ByVal arr As INDArray, ByVal newShape() As Integer, ByVal isFOrder As Boolean) As INDArray
            Return newShapeNoCopy(arr, ArrayUtil.toLongArray(newShape), isFOrder)
        End Function
        ''' <summary>
        ''' A port of numpy's reshaping algorithm that leverages
        ''' no copy where possible and returns
        ''' null if the reshape
        ''' couldn't happen without copying </summary>
        ''' <param name="arr">  the array to reshape </param>
        ''' <param name="newShape"> the new shape </param>
        ''' <param name="isFOrder"> whether the array will be fortran ordered or not </param>
        ''' <returns> null if a reshape isn't possible, or a new ndarray </returns>
        Public Shared Function newShapeNoCopy(ByVal arr As INDArray, ByVal newShape() As Long, ByVal isFOrder As Boolean) As INDArray
            Dim oldnd As Integer
            Dim olddims() As Long = ArrayUtil.copy(arr.shape())
            Dim oldstrides() As Long = ArrayUtil.copy(arr.stride())
            Dim np, op, last_stride As Long
            Dim oi, oj, ok, ni, nj, nk As Integer
            Dim newStrides(newShape.Length - 1) As Long
            oldnd = 0
    '
    '         * Remove axes with dimension 1 from the old array. They have no effect
    '         * but would need special cases since their strides do not matter.
    '
            oi = 0
            Do While oi < arr.rank()
                If arr.size(oi) <> 1 Then
                    olddims(oldnd) = arr.size(oi)
                    oldstrides(oldnd) = arr.stride(oi)
                    oldnd += 1
                End If
                oi += 1
            Loop

            np = 1
            For ni = 0 To newShape.Length - 1
                np *= newShape(ni)
            Next ni
            op = 1
            For oi = 0 To oldnd - 1
                op *= olddims(oi)
            Next oi
            If np <> op Then
                ' different total sizes; no hope
                Return Nothing
            End If

            If np = 0 Then
                ' the current code does not handle 0-sized arrays, so give up
                Return Nothing
            End If

            ' oi to oj and ni to nj give the axis ranges currently worked with
            oi = 0
            oj = 1
            ni = 0
            nj = 1
            Do While ni < newShape.Length AndAlso oi < oldnd
                np = newShape(ni)
                op = olddims(oi)

                Do While np <> op
                    If np < op Then
                        ' Misses trailing 1s, these are handled later
                        np *= newShape(nj)
                            nj += 1
                    Else
                        op *= olddims(oj)
                            oj += 1
                    End If
                Loop

                ' Check whether the original axes can be combined
                ok = oi
                Do While ok < oj - 1
                    If isFOrder Then
                        If oldstrides(ok + 1) <> olddims(ok) * oldstrides(ok) Then
                            ' not contiguous enough
                            Return Nothing
                        End If
                    Else
                        ' C order
                        If oldstrides(ok) <> olddims(ok + 1) * oldstrides(ok + 1) Then
                            ' not contiguous enough
                            Return Nothing
                        End If
                    End If
                    ok += 1
                Loop

                ' Calculate new strides for all axes currently worked with
                If isFOrder Then
                    newStrides(ni) = oldstrides(oi)
                    For nk = ni + 1 To nj - 1
                        newStrides(nk) = newStrides(nk - 1) * newShape(nk - 1)
                    Next nk
                Else
                    ' C order
                    newStrides(nj - 1) = oldstrides(oj - 1)
                    For nk = nj - 1 To ni + 1 Step -1
                        newStrides(nk - 1) = newStrides(nk) * newShape(nk)
                    Next nk
                End If
                ni = nj
                    nj += 1
                oi = oj
                    oj += 1
            Loop

    '
    '         * Set strides corresponding to trailing 1s of the new shape.
    '
            If ni >= 1 Then
                last_stride = newStrides(ni - 1)
            Else
                last_stride = 1
            End If
            If isFOrder AndAlso ni >= 1 Then
                last_stride *= newShape(ni - 1)
            End If
            For nk = ni To newShape.Length - 1
                newStrides(nk) = last_stride
            Next nk

            ' we need to wrap buffer of a current array, to make sure it's properly marked as a View
            Dim db As DataBuffer = arr.data()
            Dim buffer As DataBuffer = Nd4vbT.createBuffer(db, arr.offset(), arr.length())
            Dim ret As INDArray = Nd4vbT.create(buffer, newShape, newStrides, arr.offset(),If(isFOrder, "f"c, "c"c))
            Return ret
        End Function

        ''' <summary>
        ''' Infer order from </summary>
        ''' <param name="shape"> the shape to infer by </param>
        ''' <param name="stride"> the stride to infer by </param>
        ''' <param name="elementStride"> the element stride to start at </param>
        ''' <returns> the storage order given shape and element stride </returns>
        Public Shared Function cOrFortranOrder(ByVal shapeArr() As Long, ByVal stride() As Long, ByVal elementStride As Long) As Boolean
            Dim sd As Long
            Dim [dim] As Long
            Dim i As Integer
            Dim cContiguous As Boolean = True
            Dim isFortran As Boolean = True

            sd = 1
            For i = shapeArr.Length - 1 To 0 Step -1
                [dim] = shapeArr(i)

                If stride(i) <> sd Then
                    cContiguous = False
                    Exit For
                End If
                ' contiguous, if it got this far
                If [dim] = 0 Then
                    Exit For
                End If
                sd *= [dim]

            Next i

            ' check if fortran contiguous
            sd = elementStride
            For i = 0 To shapeArr.Length - 1
                [dim] = shapeArr(i)
                If stride(i) <> sd Then
                    isFortran = False
                End If
                If [dim] = 0 Then
                    Exit For
                End If
                sd *= [dim]

            Next i
            Return cContiguous OrElse isFortran
        End Function

        <Obsolete>
        Public Shared Function cOrFortranOrder(ByVal shapeArr() As Integer, ByVal stride() As Integer, ByVal elementStride As Integer) As Boolean
            Return cOrFortranOrder(ArrayUtil.toLongArray(shapeArr), ArrayUtil.toLongArray(stride), elementStride)
        End Function

        ''' <summary>
        ''' Infer order from </summary>
        ''' <param name="shape"> the shape to infer by </param>
        ''' <param name="stride"> the stride to infer by </param>
        ''' <param name="elementStride"> the element stride to start at </param>
        ''' <returns> the storage order given shape and element stride </returns>
        Public Shared Function getOrder(ByVal shapeArr() As Integer, ByVal stride() As Integer, ByVal elementStride As Integer) As Char
            Dim sd As Integer
            Dim [dim] As Integer
            Dim i As Integer
            Dim cContiguous As Boolean = True
            Dim isFortran As Boolean = True

            sd = 1
            For i = shapeArr.Length - 1 To 0 Step -1
                [dim] = shapeArr(i)

                If stride(i) <> sd Then
                    cContiguous = False
                    Exit For
                End If
                ' contiguous, if it got this far
                If [dim] = 0 Then
                    Exit For
                End If
                sd *= [dim]

            Next i

            ' check if fortran contiguous
            sd = elementStride
            For i = 0 To shapeArr.Length - 1
                [dim] = shapeArr(i)
                If stride(i) <> sd Then
                    isFortran = False
                End If
                If [dim] = 0 Then
                    Exit For
                End If
                sd *= [dim]

            Next i

            If isFortran AndAlso cContiguous Then
                Return "a"c
            ElseIf isFortran AndAlso Not cContiguous Then
                Return "f"c
            ElseIf Not isFortran AndAlso Not cContiguous Then
                Return "c"c
            Else
                Return "c"c
            End If

        End Function

        Public Shared Function getOrder(ByVal shapeArr() As Long, ByVal stride() As Long, ByVal elementStride As Long) As Char
            Dim sd As Long
            Dim [dim] As Long
            Dim i As Integer
            Dim cContiguous As Boolean = True
            Dim isFortran As Boolean = True

            sd = 1
            For i = shapeArr.Length - 1 To 0 Step -1
                [dim] = shapeArr(i)

                If stride(i) <> sd Then
                    cContiguous = False
                    Exit For
                End If
                ' contiguous, if it got this far
                If [dim] = 0 Then
                    Exit For
                End If
                sd *= [dim]

            Next i

            ' check if fortran contiguous
            sd = elementStride
            For i = 0 To shapeArr.Length - 1
                [dim] = shapeArr(i)
                If stride(i) <> sd Then
                    isFortran = False
                End If
                If [dim] = 0 Then
                    Exit For
                End If
                sd *= [dim]

            Next i

            If isFortran AndAlso cContiguous Then
                Return "a"c
            ElseIf isFortran AndAlso Not cContiguous Then
                Return "f"c
            End If

            'Check if ascending strides
            Dim stridesAscending As Boolean = True
            For j As Integer = 1 To stride.Length - 1
                stridesAscending = stridesAscending And (stride(j) >= stride(j-1))
            Next j
            If stridesAscending Then
                Return "f"c
            End If

            Return "c"c
        End Function

        ''' <summary>
        ''' Infer the order for the ndarray based on the
        ''' array's strides </summary>
        ''' <param name="arr"> the array to get the
        '''            ordering for </param>
        ''' <returns> the ordering for the given array </returns>
        Public Shared Function getOrder(ByVal arr As INDArray) As Char
            Return getOrder(arr.shape(), arr.stride(), 1)
        End Function

        ''' <summary>
        ''' Convert the given index (such as 1,1)
        ''' to a linear index </summary>
        ''' <param name="shape"> the shape of the indexes to convert </param>
        ''' <param name="indices"> the index to convert </param>
        ''' <returns> the linear index given the shape
        ''' and indices </returns>
        Public Shared Function sub2Ind(ByVal shapeArr() As Integer, ByVal indices() As Integer) As Long
            Dim index As Long = 0
            Dim shift As Integer = 1
            For i As Integer = 0 To shapeArr.Length - 1
                index += shift * indices(i)
                shift *= shapeArr(i)
            Next i
            Return index
        End Function

        ''' <summary>
        ''' Convert a linear index to
        ''' the equivalent nd index </summary>
        ''' <param name="shape"> the shape of the dimensions </param>
        ''' <param name="index"> the index to map </param>
        ''' <param name="numIndices"> the number of total indices (typically prod of shape( </param>
        ''' <returns> the mapped indexes along each dimension </returns>
        Public Shared Function ind2sub(ByVal shapeArr() As Integer, ByVal index As Long, ByVal numIndices As Long) As Integer()
            Dim denom As Long = numIndices
            Dim ret(shapeArr.Length - 1) As Integer
            For i As Integer = ret.Length - 1 To 0 Step -1
                denom \= shapeArr(i)
                If index \ denom >= Integer.MaxValue Then
                    Throw New System.ArgumentException("Dimension can not be >= Integer.MAX_VALUE")
                End If
                ret(i) = CInt(index \ denom)
                index = index Mod denom

            Next i
            Return ret
        End Function

        Public Shared Function ind2sub(ByVal shapeArr() As Long, ByVal index As Long, ByVal numIndices As Long) As Long()
            Dim denom As Long = numIndices
            Dim ret(shapeArr.Length - 1) As Long
            For i As Integer = ret.Length - 1 To 0 Step -1
                denom \= shapeArr(i)
                If index \ denom >= Integer.MaxValue Then
                    Throw New System.ArgumentException("Dimension can not be >= Integer.MAX_VALUE")
                End If
                ret(i) = (index \ denom)
                index = index Mod denom
            Next i
            Return ret
        End Function

        ''' <summary>
        ''' Convert a linear index to
        ''' the equivalent nd index.
        ''' Infers the number of indices from the specified shape.
        ''' </summary>
        ''' <param name="shape"> the shape of the dimensions </param>
        ''' <param name="index"> the index to map </param>
        ''' <returns> the mapped indexes along each dimension </returns>
        Public Shared Function ind2sub(ByVal shapeArr() As Integer, ByVal index As Long) As Integer()
            Return ind2sub(shapeArr, index, ArrayUtil.prodLong(shapeArr))
        End Function

        Public Shared Function ind2sub(ByVal shapeArr() As Long, ByVal index As Long) As Long()
            Return ind2sub(shapeArr, index, ArrayUtil.prodLong(shapeArr))
        End Function

        ''' <summary>
        ''' Convert a linear index to
        ''' the equivalent nd index based on the shape of the specified ndarray.
        ''' Infers the number of indices from the specified shape.
        ''' </summary>
        ''' <param name="arr"> the array to compute the indexes
        '''            based on </param>
        ''' <param name="index"> the index to map </param>
        ''' <returns> the mapped indexes along each dimension </returns>
        Public Shared Function ind2sub(ByVal arr As INDArray, ByVal index As Long) As Long()
            If arr.rank() = 1 Then
                Return New Long(){CInt(index)}
            End If
            Return ind2sub(arr.shape(), index, ArrayUtil.prodLong(arr.shape()))
        End Function

        ''' <summary>
        ''' Convert a linear index to
        ''' the equivalent nd index </summary>
        ''' <param name="shape"> the shape of the dimensions </param>
        ''' <param name="index"> the index to map </param>
        ''' <param name="numIndices"> the number of total indices (typically prod of shape( </param>
        ''' <returns> the mapped indexes along each dimension </returns>
        Public Shared Function ind2subC(ByVal shapeArr() As Integer, ByVal index As Long, ByVal numIndices As Long) As Integer()
            Dim denom As Long = numIndices
            Dim ret(shapeArr.Length - 1) As Integer
            For i As Integer = 0 To shapeArr.Length - 1
                denom \= shapeArr(i)
                If index \ denom >= Integer.MaxValue Then
                    Throw New System.ArgumentException("Dimension can not be >= Integer.MAX_VALUE")
                End If
                ret(i) = CInt(index \ denom)
                index = index Mod denom

            Next i
            Return ret
        End Function

        Public Shared Function ind2subC(ByVal shapeArr() As Long, ByVal index As Long, ByVal numIndices As Long) As Long()
            Dim denom As Long = numIndices
            Dim ret(shapeArr.Length - 1) As Long
            For i As Integer = 0 To shapeArr.Length - 1
                denom \= shapeArr(i)
                If index \ denom >= Integer.MaxValue Then
                    Throw New System.ArgumentException("Dimension can not be >= Integer.MAX_VALUE")
                End If
                ret(i) = index \ denom
                index = index Mod denom

            Next i
            Return ret
        End Function

        ''' <summary>
        ''' Convert a linear index to
        ''' the equivalent nd index.
        ''' Infers the number of indices from the specified shape.
        ''' </summary>
        ''' <param name="shape"> the shape of the dimensions </param>
        ''' <param name="index"> the index to map </param>
        ''' <returns> the mapped indexes along each dimension </returns>
        Public Shared Function ind2subC(ByVal shapeArr() As Integer, ByVal index As Long) As Integer()
            Return ind2subC(shapeArr, index, ArrayUtil.prodLong(shapeArr))
        End Function

        Public Shared Function ind2subC(ByVal shapeArr() As Long, ByVal index As Long) As Long()
            Return ind2subC(shapeArr, index, ArrayUtil.prodLong(shapeArr))
        End Function

        ''' <summary>
        ''' Convert a linear index to
        ''' the equivalent nd index based on the shape of the specified ndarray.
        ''' Infers the number of indices from the specified shape.
        ''' </summary>
        ''' <param name="arr"> the array to compute the indexes
        '''            based on </param>
        ''' <param name="index"> the index to map </param>
        ''' <returns> the mapped indexes along each dimension </returns>
        Public Shared Function ind2subC(ByVal arr As INDArray, ByVal index As Long) As Long()
            If arr.rank() = 1 Then
                Return New Long(){index}
            End If
            Return ind2subC(arr.shape(), index, ArrayUtil.prodLong(arr.shape()))
        End Function

        ''' <summary>
        ''' Assert the both shapes are the same length
        ''' and shape[i] < lessThan[i] </summary>
        ''' <param name="shape"> the shape to check </param>
        ''' <param name="lessThan"> the shape to assert against </param>
        Public Shared Sub assertShapeLessThan(ByVal shapeArr() As Integer, ByVal lessThan() As Integer)
            If shapeArr.Length <> lessThan.Length Then
                Throw New System.ArgumentException("Shape length must be == less than length")
            End If
            For i As Integer = 0 To shapeArr.Length - 1
                If shapeArr(i) >= lessThan(i) Then
                    Throw New System.InvalidOperationException("Shape[" & i & "] should be less than lessThan[" & i & "]")
                End If
            Next i
        End Sub

        Public Shared Sub assertShapeLessThan(ByVal shapeArr() As Long, ByVal lessThan() As Long)
            If shapeArr.Length <> lessThan.Length Then
                Throw New System.ArgumentException("Shape length must be == less than length")
            End If
            For i As Integer = 0 To shapeArr.Length - 1
                If shapeArr(i) >= lessThan(i) Then
                    Throw New System.InvalidOperationException("Shape[" & i & "] should be less than lessThan[" & i & "]")
                End If
            Next i
        End Sub

        Public Shared Function newStrides(ByVal strides() As Integer, ByVal newLength As Integer, ByVal indexes() As INDArrayIndex) As Integer()
            If strides.Length > newLength Then
                Dim newStridesArr(strides.Length - 2) As Integer
                Array.Copy(strides, 1, newStridesArr, 0, newStridesArr.Length)
                strides = newStridesArr
            End If

            Return strides
        End Function

        ''' <summary>
        ''' Check if strides are in order suitable for non-strided mmul etc.
        ''' Returns true if c order and strides are descending [100,10,1] etc
        ''' Returns true if f order and strides are ascending [1,10,100] etc
        ''' False otherwise. </summary>
        ''' <returns> true if c+descending, f+ascending, false otherwise </returns>
        Public Shared Function strideDescendingCAscendingF(ByVal array As INDArray) As Boolean
            If array.rank() <= 1 Then
                Return True
            End If
            Dim strides() As Long = array.stride()
            If array.Vector AndAlso strides(0) = 1 AndAlso strides(1) = 1 Then
                Return True
            End If
            Dim order As Char = array.ordering()

            If order = "c"c Then 'Expect descending. [100,10,1] etc
                For i As Integer = 1 To strides.Length - 1
                    If strides(i - 1) <= strides(i) Then
                        Return False
                    End If
                Next i
                Return True
            ElseIf order = "f"c Then 'Expect ascending. [1,10,100] etc
                For i As Integer = 1 To strides.Length - 1
                    If strides(i - 1) >= strides(i) Then
                        Return False
                    End If
                Next i
                Return True
            ElseIf order = "a"c Then
                Return True
            Else
                Throw New System.InvalidOperationException("Invalid order: not c or f (is: " & order & ")")
            End If
        End Function

        ''' <summary>
        ''' Gets the rank given the shape info buffer </summary>
        ''' <param name="buffer"> the buffer to get the rank for </param>
        ''' <returns> the rank for the shape buffer </returns>
        Public Shared Function length(ByVal buffer As DataBuffer) As Long
            Dim ret As Long = 1
            Dim shapeArr As DataBuffer = ShapeT.shapeOf(buffer)
            Dim rank As Integer = ShapeT.rank(buffer)
            For i As Integer = 0 To rank - 1
                ret *= shapeArr.getLong(i)
            Next i

            Return ret
        End Function

        Public Shared Function length(ByVal buffer() As Integer) As Long
            Dim ret As Long = 1
            Dim limit As Integer = ShapeT.rank(buffer) + 1
            For i As Integer = 1 To limit - 1
                ret *= buffer(i)
            Next i
            Return ret
        End Function

        Public Shared Function length(ByVal buffer() As Long) As Long
            Dim ret As Long = 1
            Dim limit As Integer = ShapeT.rank(buffer) + 1
            For i As Integer = 1 To limit - 1
                ret *= buffer(i)
            Next i
            Return ret
        End Function

        ''' <summary>
        ''' Gets the rank given the shape info buffer </summary>
        ''' <param name="buffer"> the buffer to get the rank for </param>
        ''' <returns> the rank for the shape buffer </returns>
        Public Shared Function rank(ByVal buffer As DataBuffer) As Integer
            Return buffer.getInt(0)
        End Function

        Public Shared Function rank(ByVal buffer() As Long) As Integer
            Return CInt(buffer(0))
        End Function

        Public Shared Function rank(ByVal buffer() As Integer) As Integer
            Return buffer(0)
        End Function

        ''' <summary>
        ''' Get the size of the specified dimension. Equivalent to shape()[dimension] </summary>
        ''' <param name="buffer">       The buffer to get the shape from </param>
        ''' <param name="dimension">    The dimension to get. </param>
        ''' <returns>             The size of the specified dimension </returns>
        Public Shared Function size(ByVal buffer As DataBuffer, ByVal dimension As Integer) As Integer
            Dim rank As Integer = ShapeT.rank(buffer)
            If dimension >= rank Then
                Throw New System.ArgumentException("Invalid dimension " & dimension & " for rank " & rank & " array")
            End If
            Return buffer.getInt(1 + dimension)
        End Function

        Public Shared Function size(ByVal buffer() As Integer, ByVal dimension As Integer) As Integer
            Dim rank As Integer = ShapeT.rank(buffer)
            If dimension >= rank Then
                Throw New System.ArgumentException("Invalid dimension " & dimension & " for rank " & rank & " array")
            End If
            Return buffer(1 + dimension)
        End Function

        Public Shared Function size(ByVal buffer() As Long, ByVal dimension As Integer) As Long
            Dim rank As Integer = ShapeT.rank(buffer)
            If dimension >= rank Then
                Throw New System.ArgumentException("Invalid dimension " & dimension & " for rank " & rank & " array")
            End If
            Return buffer(1 + dimension)
        End Function

        ''' <summary>
        ''' Get the size of the specified dimension. Identical to Shape.size(...), but does not perform any input validation </summary>
        ''' <param name="buffer">       The buffer to get the shape from </param>
        ''' <param name="dimension">    The dimension to get. </param>
        ''' <returns>             The size of the specified dimension </returns>
        Public Shared Function sizeUnsafe(ByVal buffer As DataBuffer, ByVal dimension As Integer) As Integer
            Return buffer.getInt(1 + dimension)
        End Function

        Public Shared Function sizeUnsafe(ByVal buffer() As Integer, ByVal dimension As Integer) As Integer
            Return buffer(1 + dimension)
        End Function

        Public Shared Function sizeUnsafe(ByVal buffer() As Long, ByVal dimension As Integer) As Long
            Return buffer(1 + dimension)
        End Function

        ''' <summary>
        ''' Get array shape from the buffer, as an int[] </summary>
        ''' <param name="buffer">    Buffer to get the shape from </param>
        ''' <returns>          Shape array </returns>
        Public Shared Function shape(ByVal buffer As DataBuffer) As Long()
            Dim ret = New Long(rank(buffer) - 1){}
            For i As Integer = 0 To ret.length - 1
                ret(i) = buffer.getInt(1 + i)
            Next i
            Return ret
        End Function

        ''' <summary>
        ''' Get array shape from an int[] </summary>
        ''' <param name="buffer">    Buffer to get the shape from </param>
        ''' <returns>          Shape array </returns>
        Public Shared Function shape(ByVal buffer() As Integer) As Integer()
            Dim ret(rank(buffer) - 1) As Integer
            Array.Copy(buffer, 1, ret, 0, ret.Length)
            Return ret
        End Function

        Public Shared Function shape(ByVal buffer() As Long) As Long()
            Dim ret(rank(buffer) - 1) As Long
            Array.Copy(buffer, 1, ret, 0, ret.Length)
            Return ret
        End Function

        ''' <summary>
        ''' Get the stride of the specified dimension </summary>
        ''' <param name="buffer">       The buffer to get the stride from </param>
        ''' <param name="dimension">    The dimension to get. </param>
        ''' <returns>             The stride of the specified dimension </returns>
        Public Shared Function stride(ByVal buffer As DataBuffer, ByVal dimension As Integer) As Integer
            Dim rank As Integer = ShapeT.rank(buffer)
            If dimension >= rank Then
                Throw New System.ArgumentException("Invalid dimension " & dimension & " for rank " & rank & " array")
            End If
            Return buffer.getInt(1 + rank + dimension)
        End Function

        Public Shared Function stride(ByVal buffer() As Integer, ByVal dimension As Integer) As Integer
            Dim rank As Integer = ShapeT.rank(buffer)
            If dimension >= rank Then
                Throw New System.ArgumentException("Invalid dimension " & dimension & " for rank " & rank & " array")
            End If
            Return buffer(1 + rank + dimension)
        End Function

        Public Shared Function stride(ByVal buffer() As Long, ByVal dimension As Integer) As Long
            Dim rank As Integer = ShapeT.rank(buffer)
            If dimension >= rank Then
                Throw New System.ArgumentException("Invalid dimension " & dimension & " for rank " & rank & " array")
            End If
            Return buffer(1 + rank + dimension)
        End Function

        ''' <summary>
        ''' Get array shape from the buffer, as an int[] </summary>
        ''' <param name="buffer">    Buffer to get the shape from </param>
        ''' <returns>          Shape array </returns>
        Public Shared Function strideArr(ByVal buffer As DataBuffer) As Long()
            Dim ret = New Long(rank(buffer) - 1){}
            Dim stride As DataBuffer = ShapeT.stride(buffer)
            For i As Integer = 0 To ret.length - 1
                ret(i) = stride.getInt(i)
            Next i
            Return ret
        End Function

        ''' <summary>
        ''' Get the stride of the specified dimension, without any input validation </summary>
        ''' <param name="buffer">       The buffer to get the stride from </param>
        ''' <param name="dimension">    The dimension to get. </param>
        ''' <param name="rank">         Rank of the array </param>
        ''' <returns>             The stride of the specified dimension </returns>
        Public Shared Function strideUnsafe(ByVal buffer As DataBuffer, ByVal dimension As Integer, ByVal rank As Integer) As Integer
            Return buffer.getInt(1 + rank + dimension)
        End Function

        Public Shared Function strideUnsafe(ByVal buffer() As Integer, ByVal dimension As Integer, ByVal rank As Integer) As Integer
            Return buffer(1 + rank + dimension)
        End Function

        Public Shared Function strideUnsafe(ByVal buffer() As Long, ByVal dimension As Integer, ByVal rank As Integer) As Long
            Return buffer(1 + rank + dimension)
        End Function

        ''' <summary>
        ''' Return the shape info length
        ''' given the rank </summary>
        ''' <param name="rank"> the rank to get the length for </param>
        ''' <returns> rank * 2 + 4 </returns>
        Public Shared Function shapeInfoLength(ByVal rank As Long) As Integer
            Return CInt(rank) * 2 + 4
        End Function

        Public Shared Function shapeInfoLength(ByVal shapeArr() As Long) As Integer
            Return shapeInfoLength(CInt(shapeArr(0)))
        End Function

        ''' <summary>
        ''' Get the shape from
        ''' the given int buffer </summary>
        ''' <param name="buffer"> the buffer to get the shape information for
        ''' @return </param>
        Public Shared Function stride(ByVal buffer As DataBuffer) As DataBuffer
            Dim rank As Integer = ShapeT.rank(buffer)
            Return Nd4vbT.createBuffer(buffer, 1 + rank, rank)
        End Function

        Public Shared Function stride(ByVal buffer() As Integer) As Integer()
            Dim rank As Integer = ShapeT.rank(buffer)
            Dim ret(rank - 1) As Integer
            For i As Integer = 0 To rank - 1
                ret(i) = buffer(1 + rank + i)
            Next i

            Return ret
        End Function

        Public Shared Function stride(ByVal buffer() As Long) As Long()
            Dim rank As Integer = ShapeT.rank(buffer)
            Dim ret(rank - 1) As Long
            For i As Integer = 0 To rank - 1
                ret(i) = buffer(1 + rank + i)
            Next i

            Return ret
        End Function

        ''' <summary>
        ''' Get the shape from
        ''' the given int buffer </summary>
        ''' <param name="buffer"> the buffer to get the shape information for
        ''' @return </param>
        Public Shared Function shapeOf(ByVal buffer As DataBuffer) As DataBuffer
            Dim rank As Integer = CInt(buffer.getLong(0))
            Return Nd4vbT.createBuffer(buffer, 1, rank)
        End Function

        Public Shared Function shapeOf(ByVal buffer() As Integer) As Integer()
            Dim rank = buffer(0)
            Return Arrays.CopyOfRange(buffer, 1, 1 + rank)
        End Function

        Public Shared Function shapeOf(ByVal buffer() As Long) As Long()
            Dim rank = CInt(buffer(0))
            Return Arrays.CopyOfRange(buffer, 1, 1 + rank)
        End Function

        Public Shared Function stridesOf(ByVal buffer() As Integer) As Integer()
            Dim rank = buffer(0)
            Return Arrays.CopyOfRange(buffer, 1+rank, 1 + (rank * 2))
        End Function

        Public Shared Function stridesOf(ByVal buffer() As Long) As Long()
            Dim rank = CInt(buffer(0))
            Return Arrays.CopyOfRange(buffer, 1+rank, 1 + (rank * 2))
        End Function

        Public Shared Function flags(ByVal buffer As DataBuffer) As Integer()
            Dim length As Integer = buffer.getInt(0)
            Dim ret(length - 1) As Integer
            For i As Integer = 0 To ret.Length - 1
                ret(i) = buffer.getInt(1 + i)
            Next i
            Return ret
        End Function

        Public Shared Function sparseOffsets(ByVal buffer As DataBuffer) As Integer()
            Dim flagsLength As Integer = buffer.getInt(0)
            Dim offLength As Integer = buffer.getInt(flagsLength + 1)
            Dim ret(offLength - 1) As Integer
            For i As Integer = 0 To offLength - 1
                ret(i) = buffer.getInt(i + flagsLength + 2)
            Next i
            Return ret
        End Function

        Public Shared Function hiddenDimension(ByVal buffer As DataBuffer) As Integer()
            Dim flagsLength As Integer = buffer.getInt(0)
            Dim offLength As Integer = buffer.getInt(flagsLength + 1)
            Dim hiddenDimLength As Integer = buffer.getInt(flagsLength + offLength + 2)

            Dim ret(hiddenDimLength - 1) As Integer
            For i As Integer = 0 To hiddenDimLength - 1
                ret(i) = buffer.getInt(i + flagsLength + offLength + 3)
            Next i
            Return ret
        End Function

        Public Shared Function underlyingRank(ByVal buffer As DataBuffer) As Integer
            Dim flagsLength As Integer = buffer.getInt(0)
            Dim offLength As Integer = buffer.getInt(flagsLength + 1)
            Dim hiddenDimLength As Integer = buffer.getInt(flagsLength + offLength + 2)

            Return buffer.getInt(flagsLength + offLength + hiddenDimLength + 3)
        End Function

        ''' <summary>
        ''' Prints the shape
        ''' for this shape information </summary>
        ''' <param name="arr"> the shape information to print </param>
        ''' <returns> the shape information to string </returns>
        Public Shared Function shapeToString(ByVal arr As INDArray) As String
            Return shapeToString(arr.shapeInfoJava())
        End Function

        ''' <summary>
        ''' Prints the rank, data type, offset, order, shape and strides of a shape information array (Java's
        ''' <c>shapeToString(LongBuffer)</c>; the offset is always 0).
        ''' </summary>
        ''' <param name="buffer"> the shape information to print </param>
        ''' <returns> the shape information to string </returns>
        Public Shared Function shapeToString(ByVal buffer() As Long) As String
            Dim options As Long = buffer(buffer.Length - 3)
            Dim rank As Integer = ShapeT.rank(buffer)
            Dim sb As New StringBuilder()
            sb.Append("Rank: ").Append(rank).Append(",").Append(" DataType: ").Append(ArrayOptionsHelper.dataType(options).ToString()).Append(",").Append(" Offset: ").Append(0L).Append(",").Append(" Order: ").Append(ShapeT.order(buffer)).Append(",").Append(" Shape: [")
            For i As Integer = 0 To rank - 1
                sb.Append(buffer(1 + i))
                If i < rank - 1 Then
                    sb.Append(",")
                End If
            Next i
            sb.Append("], ")

            sb.Append(" Stride: [")
            For i As Integer = 0 To rank - 1
                sb.Append(buffer(1 + rank + i))
                If i < rank - 1 Then
                    sb.Append(",")
                End If
            Next i
            sb.Append("]")
            Return sb.ToString()
        End Function

        ''' <summary>
        ''' A short description of an array for messages: data type, shape without spaces and order, for example
        ''' <c>FLOAT,[2,3],c</c>.
        ''' </summary>
        Public Shared Function shapeToStringShort(ByVal arr As INDArray) As String
            Dim s() As Long = arr.shape()
            Return arr.dataType().ToString() & "," & (If(s Is Nothing, "[]", Arrays.toString(s).Replace(" ", ""))) & "," & arr.ordering()
        End Function

        ''' <summary>
        ''' Get the offset for the buffer
        '''
        ''' PLEASE NOTE: Legacy method. Will return 0 ALWAYS </summary>
        ''' <param name="buffer"> the shape info buffer to get the offset for
        ''' @return </param>
        <Obsolete>
        Public Shared Function offset(ByVal buffer As DataBuffer) As Integer
            'throw new UnsupportedOperationException("offset() method should NOT be used");
            Return 0
        End Function

        Public Shared Function options(ByVal buffer() As Long) As Long
            Dim length As Integer = shapeInfoLength(rank(buffer))
            Dim ret As Long = buffer(length - 3)
            Return ret
        End Function

        Public Shared Function extras(ByVal buffer() As Long) As Long
            Return options(buffer)
        End Function

        ''' <summary>
        ''' Get the offset for the buffer
        '''
        ''' PLEASE NOTE: Legacy method. Will return 0 ALWAYS </summary>
        ''' <param name="buffer">
        ''' @return </param>
        <Obsolete>
        Public Shared Function offset(ByVal buffer() As Integer) As Integer
            'throw new UnsupportedOperationException("offset() method should NOT be used");
            Return 0
        End Function

        <Obsolete>
        Public Shared Function offset(ByVal buffer() As Long) As Integer
            'throw new UnsupportedOperationException("offset() method should NOT be used");
            Return 0
        End Function

        ''' <summary>
        ''' Get the element wise stride for the
        ''' shape info buffer </summary>
        ''' <param name="buffer"> the buffer to get the element
        '''               wise stride from </param>
        ''' <returns> the element wise stride for the buffer </returns>
        Public Shared Function elementWiseStride(ByVal buffer As DataBuffer) As Integer
            Dim length2 As Integer = shapeInfoLength(buffer.getInt(0))
            Return buffer.getInt(length2 - 2)
        End Function

        ''' <summary>
        ''' Get the element wise stride for the
        ''' shape info buffer </summary>
        ''' <param name="buffer"> the buffer to get the element
        '''               wise stride from </param>
        ''' <returns> the element wise stride for the buffer </returns>
        Public Shared Function elementWiseStride(ByVal buffer() As Long) As Long
            Dim length2 As Integer = shapeInfoLength(buffer)
            Return buffer(length2 - 2)
        End Function

        ''' <summary>
        ''' Get the element wise stride for the
        ''' shape info buffer </summary>
        ''' <param name="buffer"> the buffer to get the element
        '''               wise stride from </param>
        ''' <returns> the element wise stride for the buffer </returns>
        Public Shared Sub setElementWiseStride(ByVal buffer As DataBuffer, ByVal elementWiseStride As Integer)
            Dim length2 As Integer = shapeInfoLength(ShapeT.rank(buffer))
            'if (1 > 0) throw new RuntimeException("setElementWiseStride called: [" + elementWiseStride + "], buffer: " + buffer);
            buffer.put(length2 - 2, elementWiseStride)
        End Sub

        ''' <summary>
        ''' Returns the order given the shape information </summary>
        ''' <param name="buffer"> the buffer
        ''' @return </param>
        Public Shared Function order(ByVal buffer As DataBuffer) As Char
            Dim length As Integer = ShapeT.shapeInfoLength(ShapeT.rank(buffer))
            Return ChrW(buffer.getInt(length - 1))
        End Function

        Public Shared Function order(ByVal buffer() As Integer) As Char
            Dim length As Integer = ShapeT.shapeInfoLength(ShapeT.rank(buffer))
            Return ChrW(buffer(length - 1))
        End Function

        Public Shared Function order(ByVal buffer() As Long) As Char
            Dim length As Integer = ShapeT.shapeInfoLength(ShapeT.rank(buffer))
            Return ChrW(buffer(length - 1))
        End Function

        ''' <summary>
        ''' Creates the shape information buffer for the given shape, strides, element-wise stride, order and data type
        ''' through <c>Nd4vb.Executioner.createShapeInfo</c>. The array is marked empty when <paramref name="empty"/> is
        ''' set or any dimension is 0.
        ''' </summary>
        Public Shared Function createShapeInformation(ByVal shapeArr() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dataType As DataType, ByVal empty As Boolean) As DataBuffer
            Dim isEmpty As Boolean = empty
            If Not empty Then
                For Each v In shapeArr
                    If v = 0 Then
                        isEmpty = True
                        Exit For
                    End If
                Next v
            End If

            Return Nd4vbT.Executioner.createShapeInfo(shapeArr, stride, elementWiseStride, order, dataType, isEmpty)
        End Function

        ''' <summary>
        ''' Creates the shape information buffer for the given shape, strides, element-wise stride, order and extras
        ''' (data type and array type flags) through <c>Nd4vb.Executioner.createShapeInfo</c>.
        ''' </summary>
        Public Shared Function createShapeInformation(ByVal shapeArr() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal extras As Long) As DataBuffer
            Dim dtype = ArrayOptionsHelper.dataType(extras)
            'just propogate extra // it is the same value in the backend
            Return Nd4vbT.Executioner.createShapeInfo(shapeArr, stride, elementWiseStride, order, dtype, extras)
        End Function

        ''' <summary>
        ''' The shape information <c>Long()</c> for the given shape, strides, element-wise stride, order and extras:
        ''' <c>[rank, shape..., stride..., extras, elementWiseStride, order]</c>. The values are copied as given
        ''' (Java's commented-out pure version of <c>createShapeInformation</c>; the managed backend wraps the result in a
        ''' constant buffer).
        ''' </summary>
        ''' <exception cref="ND4VBIllegalStateException">The shape and the strides have different lengths.</exception>
        Public Shared Function createShapeInfoArray(ByVal shapeArr() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal extras As Long) As Long()
            If shapeArr Is Nothing Then
                Throw New ArgumentNullException(NameOf(shapeArr))
            End If
            If stride Is Nothing Then
                Throw New ArgumentNullException(NameOf(stride))
            End If
            If shapeArr.Length <> stride.Length Then
                Throw New ND4VBIllegalStateException("Shape and stride must be the same length")
            End If

            Dim rank As Integer = shapeArr.Length
            Dim shapeBuffer(shapeInfoLength(rank) - 1) As Long
            shapeBuffer(0) = rank
            Dim count As Integer = 1
            For e As Integer = 0 To rank - 1
                shapeBuffer(count) = shapeArr(e)
                count += 1
            Next e
            For e As Integer = 0 To rank - 1
                shapeBuffer(count) = stride(e)
                count += 1
            Next e
            shapeBuffer(count) = extras
            shapeBuffer(count + 1) = elementWiseStride
            shapeBuffer(count + 2) = AscW(order)
            Return shapeBuffer
        End Function

        ''' <summary>
        ''' The shape information <c>Long()</c> for the given shape, strides, element-wise stride, order and data type,
        ''' with the extras computed as <see cref="createShapeInformation(Long(), Long(), Long, Char, DataType, Boolean)"/>
        ''' does: the data type bits, plus the empty bit when <paramref name="empty"/> is set or any dimension is 0.
        ''' </summary>
        Public Shared Function createShapeInfoArray(ByVal shapeArr() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dataType As DataType, ByVal empty As Boolean) As Long()
            Dim extras As Long = ArrayOptionsHelper.setOptionBit(0L, dataType)
            If empty OrElse (shapeArr IsNot Nothing AndAlso containsZeros(shapeArr)) Then
                extras = ArrayOptionsHelper.setOptionBit(extras, ArrayType.EMPTY)
            End If
            Return createShapeInfoArray(shapeArr, stride, elementWiseStride, order, extras)
        End Function

        Public Shared Function createSparseInformation(ByVal flags() As Integer, ByVal sparseOffsets() As Long, ByVal hiddenDimensions() As Integer, ByVal underlyingRank As Integer) As DataBuffer
            Dim flagLength As Integer = flags.Length
            Dim offsetsLength As Integer = sparseOffsets.Length
            Dim hiddenDimLength As Integer = hiddenDimensions.Length
            Dim totalLength As Integer = flagLength + offsetsLength + hiddenDimLength + 4

            Dim accu As New List(Of Integer)(totalLength)
            accu.Add(flagLength)
            For Each flag As Integer In flags
                accu.Add(flag)
            Next flag
            accu.Add(offsetsLength)
            For Each off As Long In sparseOffsets
                accu.Add(CInt(off))
            Next off

            accu.Add(hiddenDimLength)

            For Each [dim] As Integer In hiddenDimensions
                accu.Add([dim])
            Next [dim]
            accu.Add(underlyingRank)

            Return Nd4vbT.createBuffer(Ints.toArray(accu))
        End Function

        ''' <summary>
        ''' To String for an int buffer </summary>
        ''' <param name="buffer">
        ''' @return </param>
        Public Overloads Shared Function toString(ByVal buffer As DataBuffer) As String
            Return buffer.ToString()
        End Function

        ''' <summary>
        ''' Returns true if the given array
        ''' is meant for the whole dimension </summary>
        ''' <param name="arr"> the array to test </param>
        ''' <returns> true if arr.length == 1 && arr[0] is Integer.MAX_VALUE </returns>
        Public Shared Function wholeArrayDimension(ParamArray ByVal arr() As Integer) As Boolean
            Return arr Is Nothing OrElse arr.Length = 0 OrElse (arr.Length = 1 AndAlso arr(0) = Integer.MaxValue)
        End Function

        Public Shared Function uniquify(ByVal array() As Integer) As Integer()
            If array.Length <= 1 Then
                Return array
            End If

            Dim unique As ISet(Of Integer) = New JavaCompat.LinkedHashSet(Of Integer)()

            For Each v In array
                unique.Add(v)
            Next v

            Return Ints.toArray(unique)
        End Function

        Public Shared Function normalizeAxis(ByVal rank As Integer, ParamArray ByVal axis() As Integer) As Integer()
            If axis Is Nothing OrElse axis.Length = 0 Then
                Return New Integer() {Integer.MaxValue}
            End If

            If rank = 0 Then
                If axis.Length <> 1 OrElse (axis(0) <> 0 AndAlso axis(0) <> Integer.MaxValue) Then
                    Throw New ND4VBIllegalStateException("Array axis for scalar (rank 0) array invalid: rank " & Arrays.toString(axis))
                End If
                If axis(0) = Integer.MaxValue Then
                    Return axis
                End If
                Return New Integer(){Integer.MaxValue}
            End If

            ' first we should get rid of all negative axis
            Dim tmp(axis.Length - 1) As Integer

            Dim cnt As Integer = 0
            For Each v In axis
                Dim t = If(v < 0, v + rank, v)

                If (t >= rank AndAlso t <> Integer.MaxValue) OrElse t < 0 Then
                    Throw New ND4VBIllegalStateException("Axis array " & Arrays.toString(axis) & " contains values above array rank (rank=" & rank & ")")
                End If

                tmp(cnt) = t
                    cnt += 1
            Next v

            ' now we're sorting array
            If tmp.Length > 1 Then
                Array.Sort(tmp)
            End If

            ' and getting rid of possible duplicates
            Return uniquify(tmp)
        End Function

        '''
        ''' <summary>
        ''' Compare the contents of a buffer and
        ''' an array for equals </summary>
        ''' <param name="arr"> the array </param>
        ''' <param name="other"> the buffer </param>
        ''' <returns> true if the content equals false otherwise </returns>
        Public Shared Function contentEquals(ByVal arr() As Integer, ByVal other As DataBuffer) As Boolean
            For i As Integer = 0 To arr.Length - 1
                If other.getInt(i) <> arr(i) Then
                    Return False
                End If
            Next i
            Return True
        End Function

        Public Shared Function contentEquals(ByVal arr() As Long, ByVal other() As Long) As Boolean
            For i As Integer = 0 To arr.Length - 1
                If other(i) <> arr(i) Then
                    Return False
                End If
            Next i
            Return True
        End Function

        Public Shared Function contentEquals(ByVal arr() As Long, ByVal other As DataBuffer) As Boolean
            For i As Integer = 0 To arr.Length - 1
                If other.getLong(i) <> arr(i) Then
                    Return False
                End If
            Next i
            Return True
        End Function

        ''' <summary>
        ''' Are the elements in the buffer contiguous for this NDArray? </summary>
        Public Shared Function isContiguousInBuffer(ByVal [in] As INDArray) As Boolean
            Dim length As Long = [in].length()
            Dim dLength As Long = [in].data().length()
            If length = dLength Then
                Return True 'full buffer, always contiguous
            End If

            Dim order As Char = [in].ordering()

            Dim shapeArr() As Long = [in].shape()
            Dim stridesIfContiguous() As Long
            If order = "f"c Then
                stridesIfContiguous = ArrayUtil.calcStridesFortran(shapeArr)
            ElseIf order = "c"c Then
                stridesIfContiguous = ArrayUtil.calcStrides(shapeArr)
            ElseIf order = "a"c Then
                stridesIfContiguous = New Long() {1, 1}
            Else
                Throw New System.InvalidOperationException("Invalid order: not c or f (is: " & order & ")")
            End If

            Return [in].stride().SequenceEqual(stridesIfContiguous)
        End Function

        ''' <summary>
        ''' This method is used in DL4J LSTM implementation </summary>
        ''' <param name="input">
        ''' @return </param>
        Public Shared Function toMmulCompatible(ByVal input As INDArray) As INDArray
            If input.rank() <> 2 Then
                Throw New System.ArgumentException("Input must be rank 2 (matrix)")
            End If
            'Same conditions as GemmParams.copyIfNecessary()
            Dim doCopy As Boolean = False
            If input.ordering() = "c"c AndAlso (input.stride(0) <> input.size(1) OrElse input.stride(1) <> 1) Then
                doCopy = True
            ElseIf input.ordering() = "f"c AndAlso (input.stride(0) <> 1 OrElse input.stride(1) <> input.size(0)) Then
                doCopy = True
            End If

            If doCopy Then
                Return ShapeT.toOffsetZeroCopyAnyOrder(input)
            Else
                Return input
            End If
        End Function

        ''' <summary>
        ''' Return the rank for the given shape
        ''' </summary>
        ''' <param name="shape"> Shape to get the rank for </param>
        ''' <returns> Rank, of the array given the shape </returns>
        ''' <exception cref="ND4JIllegalStateException"> If shape array is null </exception>
        Public Shared Function rankFromShape(ByVal shapeArr() As Integer) As Integer
            If shapeArr Is Nothing Then
                Throw New ND4VBIllegalStateException("Cannot get rank from null shape array")
            End If
            Return shapeArr.Length
        End Function

        Public Shared Function rankFromShape(ByVal shapeArr() As Long) As Integer
            If shapeArr Is Nothing Then
                Throw New ND4VBIllegalStateException("Cannot get rank from null shape array")
            End If
            Return shapeArr.Length
        End Function

        Public Shared Sub assertBroadcastable(ByVal x As INDArray, ByVal y As INDArray)
            assertBroadcastable(x.shape(), y.shape())
        End Sub

        Public Shared Sub assertBroadcastable(ByVal x() As Integer, ByVal y() As Integer)
            If Not areShapesBroadcastable(x, y) Then
                Throw New ND4VBIllegalStateException("Arrays are different shape and are not broadcastable." & " Array 1 shape = " & Arrays.toString(x) & ", array 2 shape = " & Arrays.toString(y))
            End If
        End Sub

        Public Shared Sub assertBroadcastable(ByVal x() As Long, ByVal y() As Long)
            assertBroadcastable(x, y, Nothing)
        End Sub

        Public Shared Sub assertBroadcastable(ByVal x() As Long, ByVal y() As Long, ByVal opClass As Type)
            If Not areShapesBroadcastable(x, y) Then
                Throw New ND4VBIllegalStateException("Arrays are different shape and are not broadcastable." & " Array 1 shape = " & Arrays.toString(x) & ", array 2 shape = " & Arrays.toString(y) & (If(opClass Is Nothing, "", " - op: " & opClass.FullName)))
            End If
        End Sub

        Public Shared Function areShapesBroadcastable(ByVal x() As Integer, ByVal y() As Integer) As Boolean
            'Ported from: https://github.com/eclipse/deeplearning4j/libnd4j/blob/master/include/helpers/impl/ShapeUtils.cpp

            Dim minRank As Integer = SysMath.Min(x.Length, y.Length)
            Dim i As Integer=-1
            Do While i>= -minRank
                If x(x.Length + i) <> y(y.Length + i) AndAlso x(x.Length + i) <> 1 AndAlso y(y.Length + i) <> 1 Then
                    Return False
                End If
                i -= 1
            Loop

            Return True
        End Function

        Public Shared Function areShapesBroadcastable(ByVal left() As Long, ByVal right() As Long) As Boolean
            'Ported from: https://github.com/eclipse/deeplearning4j/libnd4j/blob/master/include/helpers/impl/ShapeUtils.cpp

            Dim minRank As Integer = SysMath.Min(left.Length, right.Length)

            Dim i As Integer = -1
            Do While i >= -minRank
                If sizeAt(left, i) <> sizeAt(right, i) AndAlso sizeAt(left, i) <> 1 AndAlso sizeAt(right, i) <> 1 Then
                    Return False
                End If
                i -= 1
            Loop

            Return True
        End Function

        '''
        ''' <param name="shape">
        ''' @return </param>
        Public Shared Function lengthOf(ByVal shapeArr() As Long) As Long
            If shapeArr.Length = 0 Then
                Return 1L
            Else
                Return ArrayUtil.prodLong(shapeArr)
            End If
        End Function

        Public Shared Function lengthOf(ByVal shapeArr() As Integer) As Long
            If shapeArr.Length = 0 Then
                Return 1L
            Else
                Return ArrayUtil.prodLong(shapeArr)
            End If
        End Function

        ''' <summary>
        ''' Calculate the length of the buffer required to store the given shape with the given strides
        ''' </summary>
        ''' <param name="shape">  Shape of the array </param>
        ''' <param name="stride"> Strides </param>
        ''' <returns> Length of the buffer </returns>
        Public Shared Function lengthOfBuffer(ByVal shapeArr() As Long, ByVal stride() As Long) As Long
            Preconditions.checkArgument(shapeArr.Length = stride.Length, "Shape and strides must be same length: shape %s, stride %s", shapeArr, stride)
            'Length is simply 1 + the buffer index of the last element
            Dim length As Long = 1
            For i As Integer = 0 To shapeArr.Length - 1
                length += (shapeArr(i)-1) * stride(i)
            Next i
            Return length
        End Function

        ''' <summary>
        ''' Calculate the length of the buffer required to store the given shape with the given strides
        ''' </summary>
        ''' <param name="shape">  Shape of the array </param>
        ''' <param name="stride"> Strides </param>
        ''' <returns> Length of the buffer </returns>
        Public Shared Function lengthOfBuffer(ByVal shapeArr() As Integer, ByVal stride() As Integer) As Long
            Preconditions.checkArgument(shapeArr.Length = stride.Length, "Shape and strides must be same length: shape %s, stride %s", shapeArr, stride)
            'Length is simply 1 + the buffer index of the last element
            Dim length As Long = 1
            For i As Integer = 0 To shapeArr.Length - 1
                length += (shapeArr(i)-1) * stride(i)
            Next i
            Return length
        End Function

        Public Shared Function hasDefaultStridesForShape(ByVal input As INDArray) As Boolean
            If input.rank() = 0 Then
                Return True
            End If
            If Not strideDescendingCAscendingF(input) Then
                Return False
            End If
            Dim order As Char = input.ordering()
            Dim defaultStrides() As Long
            If order = "f"c Then
                defaultStrides = ArrayUtil.calcStridesFortran(input.shape())
            Else
                defaultStrides = ArrayUtil.calcStrides(input.shape())
            End If
            Return input.stride().SequenceEqual(defaultStrides)
        End Function

        Public Shared Function isS(ByVal x As DataType) As Boolean
            Return x = DataType.UTF8
        End Function

        Public Shared Function isB(ByVal x As DataType) As Boolean
            Return x = DataType.BOOL
        End Function

        Public Shared Function isZ(ByVal x As DataType) As Boolean
            Return Not isR(x) AndAlso Not isS(x) AndAlso Not isB(x)
        End Function

        Public Shared Function isR(ByVal x As DataType) As Boolean
            Return x = DataType.FLOAT OrElse x = DataType.HALF OrElse x = DataType.DOUBLE OrElse x = DataType.BFLOAT16
        End Function

        Private Shared Function max(ByVal typeX As DataType, ByVal typeY As DataType) As DataType
            Return CType(SysMath.Max(CInt(typeX), CInt(typeY)), DataType)
        End Function

        Public Shared Function pickPairwiseDataType(ByVal typeX As DataType, ByVal number As Object) As DataType
            If TypeOf number Is Double Then
                Return pickPairwiseDataType(typeX, DataType.DOUBLE)
            ElseIf TypeOf number Is Single Then
                Return pickPairwiseDataType(typeX, DataType.FLOAT)
            ElseIf TypeOf number Is Long Then
                Return pickPairwiseDataType(typeX, DataType.LONG)
            ElseIf TypeOf number Is Integer Then
                Return pickPairwiseDataType(typeX, DataType.INT)
            ElseIf TypeOf number Is Short Then
                Return pickPairwiseDataType(typeX, DataType.SHORT)
            ElseIf TypeOf number Is SByte Then
                Return pickPairwiseDataType(typeX, DataType.BYTE)
            Else
                Throw New System.NotSupportedException("Unknown Number used: [" & number.GetType().FullName & "]")
            End If
        End Function

        ''' <summary>
        ''' Return a data type to use for output
        ''' within a pair wise operation such as add or subtract.
        ''' Basically: favor float like data types
        ''' over ints since they're typically used for indexing. </summary>
        ''' <param name="typeX"> the first input data type </param>
        ''' <param name="typeY"> the second input data type </param>
        ''' <returns> the resolved data type </returns>
        Public Shared Function pickPairwiseDataType(ByVal typeX As DataType, ByVal typeY As DataType) As DataType
            If typeX = typeY Then
                Return typeX
            End If

            Dim rX = isR(typeX)
            Dim rY = isR(typeY)

            ' if X is float - use it
            If rX AndAlso Not rY Then
                Return typeX
            End If

            ' if Y is float - use it
            If Not rX AndAlso rY Then
                Return typeY
            End If

            ' if both data types are float - return biggest one
            If rX AndAlso rY Then
                ' if we allow precision boost, then we pick bigger data type
                If Nd4vbT.PrecisionBoostAllowed Then
                    Return max(typeX, typeY)
                Else
                    ' and we return first operand otherwise
                    Return typeX
                End If

            End If

            ' if that's not real type, we apply same rules
            If Not rX AndAlso Not rY Then
                If Nd4vbT.PrecisionBoostAllowed Then
                    Return max(typeX, typeY)
                Else
                    ' and we return first operand otherwise
                    Return typeX
                End If
            End If

            Return typeX
        End Function

        Public Shared Function isEmpty(ByVal shapeInfo() As Long) As Boolean
            Return ArrayOptionsHelper.arrayType(shapeInfo) = ArrayType.EMPTY
        End Function

        Public Shared Sub assertValidOrder(ByVal order As Char)
            If order <> "c"c AndAlso order <> "f"c AndAlso order <> "a"c Then
                Throw New System.ArgumentException("Invalid order arg: must be 'c' or 'f' (or 'a' for vectors), got '" & order & "'")
            End If
        End Sub

        ''' <summary>
        ''' Create an INDArray to represent the (possibly null) int[] dimensions.
        ''' If null or length 0, returns an empty INT array. Otherwise, returns a 1d INT NDArray </summary>
        ''' <param name="dimensions"> Dimensions to convert </param>
        ''' <returns> Dimensions as an INDArray </returns>
        Public Shared Function ndArrayDimFromInt(ParamArray ByVal dimensions() As Integer) As INDArray
            If dimensions Is Nothing OrElse dimensions.Length = 0 Then
                Return Nd4vbT.empty(DataType.INT)
            Else
                Return Nd4vbT.createFromArray(dimensions)
            End If
        End Function

        ''' <summary>
        ''' Calculate the shape of the returned array, for a reduction along dimension </summary>
        ''' <param name="x">            Input array to reduce </param>
        ''' <param name="dimension">    Dimensions/axis to reduce on </param>
        ''' <param name="newFormat">    If new format (almost always true; will be removed eventually) </param>
        ''' <param name="keepDims">     If reduced dimensions should be kept as size 1 dimensions </param>
        ''' <returns>             Shape of the output array for the reduction </returns>
        Public Shared Function reductionShape(ByVal x As INDArray, ByVal dimension() As Integer, ByVal newFormat As Boolean, ByVal keepDims As Boolean) As Long()
            Dim wholeArray As Boolean = ShapeT.wholeArrayDimension(dimension) OrElse dimension.Length = x.rank()
            Dim retShape() As Long
            If Not newFormat Then
                retShape = If(wholeArray, New Long() {1, 1}, ArrayUtil.removeIndex(x.shape(), dimension))

                'ensure vector is proper shape (if old format)
                If retShape.Length = 1 Then
                    If dimension(0) = 0 Then
                        retShape = New Long(){1, retShape(0)}
                    Else
                        retShape = New Long(){retShape(0), 1}
                    End If
                ElseIf retShape.Length = 0 Then
                    retShape = New Long(){1, 1}
                End If
            Else
                If keepDims Then
                    retShape = CType(x.shape().Clone(), Long())
                    If wholeArray Then
                        For i As Integer = 0 To retShape.Length - 1
                            retShape(i) = 1
                        Next i
                    Else
                        For Each d As Integer In dimension
                            retShape(d) = 1
                        Next d
                    End If
                Else
                    retShape = If(wholeArray, New Long(){}, ArrayUtil.removeIndex(x.shape(), dimension))
                End If
            End If
            Return retShape
        End Function

        ''' <summary>
        ''' Determine whether the placeholder shape and the specified shape are compatible.<br>
        ''' Shapes are compatible if:<br>
        ''' (a) They are both the same length (same array rank, or null)<br>
        ''' (b) At each position either phShape[i] == -1 or phShape[i] == arrShape[i]
        ''' </summary>
        ''' <param name="phShape">  Placeholder shape </param>
        ''' <param name="arrShape"> Array shape to check if it matches the placeholder shape </param>
        ''' <returns> True if the array shape is compatible with the placeholder shape </returns>
        Public Shared Function shapeMatchesPlaceholder(ByVal phShape() As Long, ByVal arrShape() As Long) As Boolean
            If phShape Is Nothing AndAlso arrShape Is Nothing Then
                Return True 'Rank 0?
            End If
            If phShape Is Nothing OrElse arrShape Is Nothing Then
                Return False
            End If
            If phShape.Length <> arrShape.Length Then
                Return False
            End If
            For i As Integer = 0 To phShape.Length - 1
                If phShape(i) > 0 Then 'for <0 case: Any value for this dimension is OK (i.e., -1s)
                    If phShape(i) <> arrShape(i) Then
                        Return False
                    End If
                End If
            Next i

            Return True
        End Function

    End Class

End Namespace