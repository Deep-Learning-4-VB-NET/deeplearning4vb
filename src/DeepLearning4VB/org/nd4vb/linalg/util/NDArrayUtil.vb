Imports System
Imports SysMath = System.Math
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

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

Namespace org.nd4vb.linalg.util

    ''' <summary>
    ''' Deprecated conversions between Java arrays and row-vector or matrix INDArrays (in the default data type:
    ''' DOUBLE when <c>Nd4vb.dataType()</c> is DOUBLE, FLOAT otherwise), and back to integers (truncated toward zero).
    ''' </summary>
    <Obsolete>
    Public Class NDArrayUtil

        Private Sub New()
        End Sub

        <Obsolete>
        Public Shared Function toNDArray(ByVal nums()() As Integer) As INDArray
            If Nd4vbT.dataType() = DataType.DOUBLE Then
                Dim doubles() As Double = ArrayUtil.toDoubles(nums)
                Dim create As INDArray = Nd4vbT.create(doubles, New Integer() {nums(0).Length, nums.Length})
                Return create
            Else
                Dim doubles() As Single = ArrayUtil.toFloats(nums)
                Dim create As INDArray = Nd4vbT.create(doubles, New Integer() {nums(0).Length, nums.Length})
                Return create
            End If

        End Function

        <Obsolete>
        Public Shared Function toNDArray(ByVal nums() As Integer) As INDArray
            If Nd4vbT.dataType() = DataType.DOUBLE Then
                Dim doubles() As Double = ArrayUtil.toDoubles(nums)
                Dim create As INDArray = Nd4vbT.create(doubles, New Integer() {1, nums.Length})
                Return create
            Else
                Dim doubles() As Single = ArrayUtil.toFloats(nums)
                Dim create As INDArray = Nd4vbT.create(doubles, New Integer() {1, nums.Length})
                Return create
            End If
        End Function

        <Obsolete>
        Public Shared Function toNDArray(ByVal nums() As Long) As INDArray
            If Nd4vbT.dataType() = DataType.DOUBLE Then
                Dim doubles() As Double = ArrayUtil.toDoubles(nums)
                Dim create As INDArray = Nd4vbT.create(doubles, New Integer() {1, nums.Length})
                Return create
            Else
                Dim doubles() As Single = ArrayUtil.toFloats(nums)
                Dim create As INDArray = Nd4vbT.create(doubles, New Integer() {1, nums.Length})
                Return create
            End If
        End Function

        <Obsolete>
        Public Shared Function toInts(ByVal n As INDArray) As Integer()
            If n.length() > Integer.MaxValue Then
                Throw New ND4VBIllegalStateException("Can't convert INDArray with length > Integer.MAX_VALUE")
            End If

            n = n.reshape(-1L)
            Dim ret(CInt(n.length()) - 1) As Integer
            For i As Integer = 0 To n.length() - 1
                ret(i) = CInt(SysMath.Truncate(n.getFloat(i)))
            Next i
            Return ret
        End Function

        <Obsolete>
        Public Shared Function toLongs(ByVal n As INDArray) As Long()
            If n.length() > Integer.MaxValue Then
                Throw New ND4VBIllegalStateException("Can't convert INDArray with length > Integer.MAX_VALUE")
            End If

            n = n.reshape(-1L)

            Dim ret(CInt(n.length()) - 1) As Long
            For i As Integer = 0 To n.length() - 1
                ret(i) = CLng(SysMath.Truncate(n.getFloat(i)))
            Next i

            Return ret
        End Function

    End Class

End Namespace