Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape

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

Namespace org.nd4vb.linalg.shape

    ''' <summary>
    ''' Upstream <c>org.nd4j.linalg.shape.ShapeBufferTests</c>. Java built the shape information through the executioner
    ''' and read it as a <c>java.nio.LongBuffer</c> (<c>asNioLong()</c>, native-only); here the same shape information is
    ''' the <c>Long()</c> from <c>Shape.createShapeInfoArray</c> and is read with the <c>Long()</c> overloads.
    ''' <c>testArrCreationShape</c> needs arrays and moves with the array tests (phase 2 wave 3).
    ''' </summary>
    <TestClass>
    Public Class ShapeBufferTests

        <TestMethod>
        Public Sub testRank()
            Dim shape() As Long = {2, 4}
            Dim stride() As Long = {1, 2}
            Dim buff() As Long = ShapeT.createShapeInfoArray(shape, stride, 1, "c"c, DataType.DOUBLE, False)
            assertEquals(2, ShapeT.rank(buff))
        End Sub

        <TestMethod>
        Public Sub testShape()
            Dim shape() As Long = {2, 4}
            Dim stride() As Long = {1, 2}
            Dim buff() As Long = ShapeT.createShapeInfoArray(shape, stride, 1, "c"c, DataType.DOUBLE, False)
            Dim shapeView() As Long = ShapeT.shapeOf(buff)
            assertTrue(ShapeT.contentEquals(shape, shapeView))
            Dim strideView() As Long = ShapeT.stride(buff)
            assertTrue(ShapeT.contentEquals(stride, strideView))
            assertEquals("c"c, ShapeT.order(buff))
            assertEquals(1L, ShapeT.elementWiseStride(buff))
            assertFalse(ShapeT.isVector(ShapeT.shape(buff)))
            assertTrue(ShapeT.contentEquals(shape, ShapeT.shapeOf(buff)))
            assertTrue(ShapeT.contentEquals(stride, ShapeT.stride(buff)))
        End Sub

        <TestMethod>
        Public Sub testBuff()
            Dim shape() As Long = {1, 2}
            Dim stride() As Long = {1, 2}
            Dim buff() As Long = ShapeT.createShapeInfoArray(shape, stride, 1, "c"c, DataType.DOUBLE, False)
            assertTrue(ShapeT.isVector(ShapeT.shape(buff)))
        End Sub

    End Class

End Namespace
