Imports System
Imports BlasBufferUtil = DeepLearning4VB.org.nd4vb.linalg.api.blas.BlasBufferUtil
Imports Lapack = DeepLearning4VB.org.nd4vb.linalg.api.blas.Lapack
Imports Level1 = DeepLearning4VB.org.nd4vb.linalg.api.blas.Level1
Imports Level2 = DeepLearning4VB.org.nd4vb.linalg.api.blas.Level2
Imports Level3 = DeepLearning4VB.org.nd4vb.linalg.api.blas.Level3
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports LinAlgExceptions = DeepLearning4VB.org.nd4vb.linalg.util.LinAlgExceptions

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

Namespace org.nd4vb.linalg.factory

    ''' <summary>
    ''' Base implementation of a blas wrapper that
    ''' delegates things to the underlying level.
    ''' This is a migration tool to preserve the old api
    ''' while allowing for migration to the newer one (Java <c>BaseBlasWrapper</c>).
    ''' <para>
    ''' The levels and LAPACK come from the array factory (<c>Nd4vb.factory()</c>); a backend may override
    ''' <see cref="level1"/>, <see cref="level2"/>, <see cref="level3"/> and <see cref="lapack"/>. Java's <c>Number</c>
    ''' parameters are <see cref="Object"/> holding a boxed number (read with <see cref="Convert.ToDouble(Object)"/>).
    ''' </para>
    ''' </summary>
    Public MustInherit Class BaseBlasWrapper
        Implements BlasWrapper

        ''' <summary>LAPACK of the array factory.</summary>
        Public Overridable Function lapack() As Lapack Implements BlasWrapper.lapack
            Return Nd4vb.factory().lapack()
        End Function

        ''' <summary>BLAS level 1 of the array factory.</summary>
        Public Overridable Function level1() As Level1 Implements BlasWrapper.level1
            Return Nd4vb.factory().level1()
        End Function

        ''' <summary>BLAS level 2 of the array factory.</summary>
        Public Overridable Function level2() As Level2 Implements BlasWrapper.level2
            Return Nd4vb.factory().level2()
        End Function

        ''' <summary>BLAS level 3 of the array factory.</summary>
        Public Overridable Function level3() As Level3 Implements BlasWrapper.level3
            Return Nd4vb.factory().level3()
        End Function

        ''' <summary>dy = da * dx + dy (any arrays of the same length).</summary>
        Public Overridable Function axpy(ByVal da As Object, ByVal dx As INDArray, ByVal dy As INDArray) As INDArray Implements BlasWrapper.axpy
            '        if(!dx.isVector())
            '            throw new IllegalArgumentException("Unable to use axpy on a non vector");
            LinAlgExceptions.assertSameLength(dx, dy)
            level1().axpy(dx.length(), Convert.ToDouble(da), dx, dy)
            Return dy
        End Function

        ''' <summary>y = alpha * a * x + beta * y.</summary>
        Public Overridable Function gemv(ByVal alpha As Object, ByVal a As INDArray, ByVal x As INDArray, ByVal beta As Double, ByVal y As INDArray) As INDArray Implements BlasWrapper.gemv
            LinAlgExceptions.assertVector(x, y)
            LinAlgExceptions.assertMatrix(a)
            level2().gemv(BlasBufferUtil.getCharForTranspose(a), BlasBufferUtil.getCharForTranspose(x), Convert.ToDouble(alpha), a, x, beta, y)
            Return y
        End Function

        ''' <summary>a = alpha * x * y' + a.</summary>
        Public Overridable Function ger(ByVal alpha As Object, ByVal x As INDArray, ByVal y As INDArray, ByVal a As INDArray) As INDArray Implements BlasWrapper.ger
            level2().ger(BlasBufferUtil.getCharForTranspose(x), Convert.ToDouble(alpha), x, y, a)
            Return a
        End Function

        Public Overridable Function syevr(ByVal jobz As Char, ByVal range As Char, ByVal uplo As Char, ByVal a As INDArray, ByVal vl As Single, ByVal vu As Single, ByVal il As Integer, ByVal iu As Integer, ByVal abstol As Object, ByVal w As INDArray, ByVal z As INDArray, ByVal isuppz() As Integer) As Integer Implements BlasWrapper.syevr
            Throw New NotSupportedException()
        End Function

        ''' <summary>Exchanges x and y; returns y.</summary>
        Public Overridable Function swap(ByVal x As INDArray, ByVal y As INDArray) As INDArray Implements BlasWrapper.swap
            level1().swap(x, y)
            Return y
        End Function

        ''' <summary>x = alpha * x (a vector).</summary>
        Public Overridable Function scal(ByVal alpha As Double, ByVal x As INDArray) As INDArray Implements BlasWrapper.scal
            LinAlgExceptions.assertVector(x)

            If x.data().dataType() = DataType.FLOAT Then
                Return scal(CSng(alpha), x)
            End If
            level1().scal(x.length(), alpha, x)
            Return x
        End Function

        ''' <summary>x = alpha * x (a vector).</summary>
        Public Overridable Function scal(ByVal alpha As Single, ByVal x As INDArray) As INDArray Implements BlasWrapper.scal
            LinAlgExceptions.assertVector(x)

            If x.data().dataType() = DataType.DOUBLE Then
                Return scal(CDbl(alpha), x)
            End If
            level1().scal(x.length(), alpha, x)
            Return x
        End Function

        ''' <summary>y = x (vectors); returns y.</summary>
        Public Overridable Function copy(ByVal x As INDArray, ByVal y As INDArray) As INDArray Implements BlasWrapper.copy
            LinAlgExceptions.assertVector(x, y)
            level1().copy(x, y)
            Return y
        End Function

        ''' <summary>dy = da * dx + dy (vectors).</summary>
        Public Overridable Function axpy(ByVal da As Double, ByVal dx As INDArray, ByVal dy As INDArray) As INDArray Implements BlasWrapper.axpy
            LinAlgExceptions.assertVector(dx, dy)

            If dx.data().dataType() = DataType.FLOAT Then
                Return axpy(CSng(da), dx, dy)
            End If
            level1().axpy(dx.length(), da, dx, dy)
            Return dy
        End Function

        ''' <summary>dy = da * dx + dy (vectors).</summary>
        Public Overridable Function axpy(ByVal da As Single, ByVal dx As INDArray, ByVal dy As INDArray) As INDArray Implements BlasWrapper.axpy
            LinAlgExceptions.assertVector(dx, dy)

            If dx.data().dataType() = DataType.DOUBLE Then
                Return axpy(CDbl(da), dx, dy)
            End If
            level1().axpy(dx.length(), da, dx, dy)
            Return dy
        End Function

        ''' <summary>The dot product of x and y.</summary>
        Public Overridable Function dot(ByVal x As INDArray, ByVal y As INDArray) As Double Implements BlasWrapper.dot
            Return level1().dot(x.length(), 1, x, y)
        End Function

        ''' <summary>The Euclidean norm of the vector x.</summary>
        Public Overridable Function nrm2(ByVal x As INDArray) As Double Implements BlasWrapper.nrm2
            LinAlgExceptions.assertVector(x)
            Return level1().nrm2(x)
        End Function

        ''' <summary>The sum of the magnitudes of the vector x.</summary>
        Public Overridable Function asum(ByVal x As INDArray) As Double Implements BlasWrapper.asum
            LinAlgExceptions.assertVector(x)
            Return level1().asum(x)
        End Function

        ''' <summary>The 0-based index of the element of x with the largest magnitude.</summary>
        Public Overridable Function iamax(ByVal x As INDArray) As Integer Implements BlasWrapper.iamax
            Return level1().iamax(x)
        End Function

        ''' <summary>y = alpha * a * x + beta * y (a matrix, x and y vectors).</summary>
        Public Overridable Function gemv(ByVal alpha As Double, ByVal a As INDArray, ByVal x As INDArray, ByVal beta As Double, ByVal y As INDArray) As INDArray Implements BlasWrapper.gemv
            LinAlgExceptions.assertVector(x, y)
            LinAlgExceptions.assertMatrix(a)

            If a.data().dataType() = DataType.FLOAT Then
                '            DefaultOpExecutioner.validateDataType(DataType.FLOAT, a, x, y);
                Return gemv(CSng(alpha), a, x, CSng(beta), y)
            Else
                level2().gemv("N"c, "N"c, alpha, a, x, beta, y)
            End If
            Return y
        End Function

        ''' <summary>y = alpha * a * x + beta * y (a matrix, x and y vectors).</summary>
        Public Overridable Function gemv(ByVal alpha As Single, ByVal a As INDArray, ByVal x As INDArray, ByVal beta As Single, ByVal y As INDArray) As INDArray Implements BlasWrapper.gemv
            LinAlgExceptions.assertVector(x, y)
            LinAlgExceptions.assertMatrix(a)

            If a.data().dataType() = DataType.DOUBLE Then
                Return gemv(CDbl(alpha), a, x, CDbl(beta), y)
            End If
            level2().gemv("N"c, "N"c, alpha, a, x, beta, y)
            Return y
        End Function

        ''' <summary>a = alpha * x * y' + a (x and y vectors, a matrix).</summary>
        Public Overridable Function ger(ByVal alpha As Double, ByVal x As INDArray, ByVal y As INDArray, ByVal a As INDArray) As INDArray Implements BlasWrapper.ger
            LinAlgExceptions.assertVector(x, y)
            LinAlgExceptions.assertMatrix(a)

            If x.data().dataType() = DataType.FLOAT Then
                Return ger(CSng(alpha), x, y, a)
            End If

            level2().ger("N"c, alpha, x, y, a)
            Return a
        End Function

        ''' <summary>a = alpha * x * y' + a (x and y vectors, a matrix).</summary>
        Public Overridable Function ger(ByVal alpha As Single, ByVal x As INDArray, ByVal y As INDArray, ByVal a As INDArray) As INDArray Implements BlasWrapper.ger
            LinAlgExceptions.assertVector(x, y)
            LinAlgExceptions.assertMatrix(a)

            If x.data().dataType() = DataType.DOUBLE Then
                Return ger(CDbl(alpha), x, y, a)
            End If

            level2().ger("N"c, alpha, x, y, a)
            Return a
        End Function

        Public Overridable Function gesv(ByVal a As INDArray, ByVal ipiv() As Integer, ByVal b As INDArray) As INDArray Implements BlasWrapper.gesv
            Throw New NotSupportedException()
        End Function

        Public Overridable Sub checkInfo(ByVal name As String, ByVal info As Integer) Implements BlasWrapper.checkInfo

        End Sub

        Public Overridable Function sysv(ByVal uplo As Char, ByVal a As INDArray, ByVal ipiv() As Integer, ByVal b As INDArray) As INDArray Implements BlasWrapper.sysv
            Throw New NotSupportedException()
        End Function

        ''' <summary>Eigenvalues (and vectors) of a symmetric matrix through <see cref="lapack"/>.</summary>
        Public Overridable Function syev(ByVal jobz As Char, ByVal uplo As Char, ByVal a As INDArray, ByVal w As INDArray) As Integer Implements BlasWrapper.syev
            Return lapack().syev(jobz, uplo, a, w)
        End Function

        Public Overridable Function syevx(ByVal jobz As Char, ByVal range As Char, ByVal uplo As Char, ByVal a As INDArray, ByVal vl As Double, ByVal vu As Double, ByVal il As Integer, ByVal iu As Integer, ByVal abstol As Double, ByVal w As INDArray, ByVal z As INDArray) As Integer Implements BlasWrapper.syevx
            Throw New NotSupportedException()
        End Function

        Public Overridable Function syevx(ByVal jobz As Char, ByVal range As Char, ByVal uplo As Char, ByVal a As INDArray, ByVal vl As Single, ByVal vu As Single, ByVal il As Integer, ByVal iu As Integer, ByVal abstol As Single, ByVal w As INDArray, ByVal z As INDArray) As Integer Implements BlasWrapper.syevx
            Throw New NotSupportedException()
        End Function

        Public Overridable Function syevd(ByVal jobz As Char, ByVal uplo As Char, ByVal A As INDArray, ByVal w As INDArray) As Integer Implements BlasWrapper.syevd
            Throw New NotSupportedException()
        End Function

        Public Overridable Function syevr(ByVal jobz As Char, ByVal range As Char, ByVal uplo As Char, ByVal a As INDArray, ByVal vl As Double, ByVal vu As Double, ByVal il As Integer, ByVal iu As Integer, ByVal abstol As Double, ByVal w As INDArray, ByVal z As INDArray, ByVal isuppz() As Integer) As Integer Implements BlasWrapper.syevr
            Throw New NotSupportedException()
        End Function

        Public Overridable Function syevr(ByVal jobz As Char, ByVal range As Char, ByVal uplo As Char, ByVal a As INDArray, ByVal vl As Single, ByVal vu As Single, ByVal il As Integer, ByVal iu As Integer, ByVal abstol As Single, ByVal w As INDArray, ByVal z As INDArray, ByVal isuppz() As Integer) As Integer Implements BlasWrapper.syevr
            Throw New NotSupportedException()
        End Function

        Public Overridable Sub posv(ByVal uplo As Char, ByVal A As INDArray, ByVal B As INDArray) Implements BlasWrapper.posv
            Throw New NotSupportedException()
        End Sub

        Public Overridable Function geev(ByVal jobvl As Char, ByVal jobvr As Char, ByVal A As INDArray, ByVal WR As INDArray, ByVal WI As INDArray, ByVal VL As INDArray, ByVal VR As INDArray) As Integer Implements BlasWrapper.geev
            Throw New NotSupportedException()
        End Function

        Public Overridable Function sygvd(ByVal itype As Integer, ByVal jobz As Char, ByVal uplo As Char, ByVal A As INDArray, ByVal B As INDArray, ByVal W As INDArray) As Integer Implements BlasWrapper.sygvd
            Throw New NotSupportedException()
        End Function

        Public Overridable Sub gelsd(ByVal A As INDArray, ByVal B As INDArray) Implements BlasWrapper.gelsd
            Throw New NotSupportedException()
        End Sub

        Public Overridable Sub geqrf(ByVal A As INDArray, ByVal tau As INDArray) Implements BlasWrapper.geqrf
            Throw New NotSupportedException()
        End Sub

        Public Overridable Sub ormqr(ByVal side As Char, ByVal trans As Char, ByVal A As INDArray, ByVal tau As INDArray, ByVal C As INDArray) Implements BlasWrapper.ormqr
            Throw New NotSupportedException()
        End Sub

        ''' <summary>y = alpha * x + y (deprecated in Java).</summary>
        Public Overridable Sub saxpy(ByVal alpha As Double, ByVal x As INDArray, ByVal y As INDArray) Implements BlasWrapper.saxpy
            axpy(alpha, x, y)
        End Sub

        ''' <summary>y = alpha * x + y (deprecated in Java).</summary>
        Public Overridable Sub saxpy(ByVal alpha As Single, ByVal x As INDArray, ByVal y As INDArray) Implements BlasWrapper.saxpy
            axpy(alpha, x, y)
        End Sub
    End Class

End Namespace
