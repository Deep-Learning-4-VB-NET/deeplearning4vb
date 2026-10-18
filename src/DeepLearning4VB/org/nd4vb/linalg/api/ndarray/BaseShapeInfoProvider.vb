Imports System
Imports System.Threading
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

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

Namespace org.nd4vb.linalg.api.ndarray

    ''' <summary>
    ''' Base implementation of <see cref="ShapeInfoProvider"/>: builds shape information buffers through
    ''' <c>Shape.createShapeInformation</c> (which asks <c>Nd4vb.Executioner</c>) and marks them constant.
    ''' <para>
    ''' Java's <c>AtomicLong bytes</c> is the <see cref="Long"/> field <see cref="bytes"/>, updated with
    ''' <see cref="Interlocked"/>; Java's getter <c>getCachedBytes()</c> is the property <see cref="CachedBytes"/>.
    ''' </para>
    ''' </summary>
    ''' @author raver119@gmail.com
    Public MustInherit Class BaseShapeInfoProvider
        Implements ShapeInfoProvider

        ''' <summary>The bytes held by the cache of a subclass (update with <see cref="Interlocked"/>).</summary>
        Protected Friend bytes As Long = 0

        ''' <summary>Forgets the cached buffers, if the implementation caches them.</summary>
        Public MustOverride Sub purgeCache() Implements ShapeInfoProvider.purgeCache

        ''' <summary>
        ''' This method creates shapeInformation buffer, based on shape being passed in, in the default order
        ''' (<c>Nd4vb.order()</c>).
        ''' </summary>
        ''' <param name="shape"> the shape </param>
        ''' <param name="dataType"> the data type </param>
        Public Overridable Function createShapeInformation(ByVal shape() As Long, ByVal dataType As DataType) As Pair(Of DataBuffer, Long()) Implements ShapeInfoProvider.createShapeInformation
            Dim order As Char = Nd4vbT.order()

            Return createShapeInformation(shape, order, dataType)
        End Function

        ''' <summary>
        ''' This method creates shapeInformation buffer, based on shape and order being passed in: dense strides
        ''' (<c>Nd4vb.getStrides</c>) and element-wise stride 1.
        ''' </summary>
        ''' <param name="shape"> the shape </param>
        ''' <param name="order"> "c"c or "f"c </param>
        ''' <param name="dataType"> the data type </param>
        Public Overridable Function createShapeInformation(ByVal shape() As Long, ByVal order As Char, ByVal dataType As DataType) As Pair(Of DataBuffer, Long()) Implements ShapeInfoProvider.createShapeInformation
            Dim stride() As Long = Nd4vbT.getStrides(shape, order)

            ' this won't be view, so ews is 1
            Dim ews As Integer = 1

            Return createShapeInformation(shape, stride, ews, order, dataType, False)
        End Function

        ''' <summary>
        ''' This method creates shapeInformation buffer, based on detailed shape info being passed in.
        ''' </summary>
        Public Overridable Function createShapeInformation(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dataType As DataType, ByVal empty As Boolean) As Pair(Of DataBuffer, Long()) Implements ShapeInfoProvider.createShapeInformation
            Dim buffer As DataBuffer = ShapeT.createShapeInformation(shape, stride, elementWiseStride, order, dataType, empty)
            buffer.Constant = True
            Return Pair.create(buffer, buffer.asLong())
        End Function

        ''' <summary>
        ''' This method creates shapeInformation buffer, based on detailed shape info and the extras (data type and array
        ''' type flags) being passed in.
        ''' </summary>
        Public Overridable Function createShapeInformation(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal extras As Long) As Pair(Of DataBuffer, Long()) Implements ShapeInfoProvider.createShapeInformation
            Dim buffer As DataBuffer = ShapeT.createShapeInformation(shape, stride, elementWiseStride, order, extras)
            buffer.Constant = True
            Return Pair.create(buffer, buffer.asLong())
        End Function

        ''' <summary>The memory used by the cache, in bytes.</summary>
        Public Overridable ReadOnly Property CachedBytes As Long Implements ShapeInfoProvider.CachedBytes
            Get
                Return Interlocked.Read(bytes)
            End Get
        End Property

    End Class

End Namespace
