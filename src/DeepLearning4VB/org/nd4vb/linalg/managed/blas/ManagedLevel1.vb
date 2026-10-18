' DeepLearning4VB: BLAS level 1 of the managed ND4VB backend (phase 2, see docs/PHASE2_DESIGN.md).
' Structural model: nd4j-native org.nd4j.linalg.cpu.nativecpu.blas.CpuLevel1.

Imports System
Imports BaseLevel1 = DeepLearning4VB.org.nd4vb.linalg.api.blas.impl.BaseLevel1
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports Dot = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.reduce3.Dot
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
Imports ManagedBlas = DeepLearning4VB.org.nd4vb.linalg.managed.kernels.ManagedBlas

Namespace org.nd4vb.linalg.managed.blas

    ''' <summary>
    ''' BLAS level 1 of the managed backend (Java <c>CpuLevel1</c>): the FLOAT and DOUBLE routines run the managed kernels of
    ''' <see cref="ManagedBlas"/> on the arrays' storage, from the first element of each array (or element
    ''' <c>offset</c> of a buffer) with the given increments.
    ''' </summary>
    ''' <remarks>
    ''' <para>As in Java, a dot product with a non-positive increment (an array without an element-wise stride) runs a
    ''' <see cref="Dot"/> op through <c>Nd4vb.Executioner</c>; the HALF routines (<c>hdot</c>, <c>hasum</c>,
    ''' <c>haxpy</c>) and <c>srotg</c>/<c>drotg</c> (whose results Java could not return) throw
    ''' <see cref="NotSupportedException"/>; <c>srotm</c>, <c>drotm</c>, <c>srotmg</c> and <c>drotmg</c> throw
    ''' <see cref="NotSupportedException"/> naming milestone M2.</para>
    ''' <para>Difference from Java: the <see cref="DataBuffer"/> forms of <c>sdot</c>, <c>ddot</c>, <c>sasum</c>,
    ''' <c>dasum</c>, <c>isamax</c>, <c>idamax</c>, <c>scopy</c>, <c>dcopy</c>, <c>saxpy</c> and <c>daxpy</c> work (Java's
    ''' CPU backend threw <c>UnsupportedOperationException</c>). <c>sdsdot</c> and <c>dsdot</c> accumulate in double.</para>
    ''' </remarks>
    Public Class ManagedLevel1
        Inherits BaseLevel1

        Private Shared Function dsdotCore(ByVal n As Integer, ByVal x As Single(), ByVal xOffset As Long, ByVal incX As Integer, ByVal y As Single(), ByVal yOffset As Long, ByVal incY As Integer) As Double
            If n <= 0 Then
                Return 0.0
            End If
            ManagedBlas.CheckVector(x.Length, n, xOffset, incX, NameOf(x))
            ManagedBlas.CheckVector(y.Length, n, yOffset, incY, NameOf(y))
            Dim xi As Long = ManagedBlas.VectorStart(n, xOffset, incX)
            Dim yi As Long = ManagedBlas.VectorStart(n, yOffset, incY)
            Dim acc As Double = 0.0
            For i As Integer = 0 To n - 1
                acc += CDbl(x(CInt(xi))) * CDbl(y(CInt(yi)))
                xi += incX
                yi += incY
            Next i
            Return acc
        End Function

        Protected Friend Overrides Function sdsdot(ByVal N As Long, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer) As Single
            Dim xo, yo As Long
            Dim xs As Single() = BlasOperands.floats(X, "X", xo)
            Dim ys As Single() = BlasOperands.floats(Y, "Y", yo)
            Return CSng(alpha + dsdotCore(BlasOperands.count(N), xs, xo, incX, ys, yo, incY))
        End Function

        Protected Friend Overrides Function dsdot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer) As Double
            Dim xo, yo As Long
            Dim xs As Single() = BlasOperands.floats(X, "X", xo)
            Dim ys As Single() = BlasOperands.floats(Y, "Y", yo)
            Return dsdotCore(BlasOperands.count(N), xs, xo, incX, ys, yo, incY)
        End Function

        Protected Friend Overrides Function hdot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer) As Single
            Throw BlasOperands.notSupported("hdot")
        End Function

        Protected Friend Overrides Function hdot(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer, ByVal Y As DataBuffer, ByVal offsetY As Integer, ByVal incY As Integer) As Single
            Throw BlasOperands.notSupported("hdot")
        End Function

        Protected Friend Overrides Function sdot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer) As Single
            If incX >= 1 AndAlso incY >= 1 Then
                Dim xo, yo As Long
                Dim xs As Single() = BlasOperands.floats(X, "X", xo)
                Dim ys As Single() = BlasOperands.floats(Y, "Y", yo)
                Return ManagedBlas.Dot(BlasOperands.count(N), xs, xo, incX, ys, yo, incY)
            Else
                ' non-EWS dot variant
                Dim dot As New Dot(X, Y)
                Nd4vbT.Executioner.exec(dot)
                Return Convert.ToSingle(dot.FinalResult)
            End If
        End Function

        Protected Friend Overrides Function sdot(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer, ByVal Y As DataBuffer, ByVal offsetY As Integer, ByVal incY As Integer) As Single
            Dim xo, yo As Long
            Dim xs As Single() = BlasOperands.floats(X, offsetX, "X", xo)
            Dim ys As Single() = BlasOperands.floats(Y, offsetY, "Y", yo)
            Return ManagedBlas.Dot(BlasOperands.count(N), xs, xo, incX, ys, yo, incY)
        End Function

        Protected Friend Overrides Function ddot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer) As Double
            If incX >= 1 AndAlso incY >= 1 Then
                Dim xo, yo As Long
                Dim xs As Double() = BlasOperands.doubles(X, "X", xo)
                Dim ys As Double() = BlasOperands.doubles(Y, "Y", yo)
                Return ManagedBlas.Dot(BlasOperands.count(N), xs, xo, incX, ys, yo, incY)
            Else
                ' non-EWS dot variant
                Dim dot As New Dot(X, Y)
                Nd4vbT.Executioner.exec(dot)
                Return Convert.ToDouble(dot.FinalResult)
            End If
        End Function

        Protected Friend Overrides Function ddot(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer, ByVal Y As DataBuffer, ByVal offsetY As Integer, ByVal incY As Integer) As Double
            Dim xo, yo As Long
            Dim xs As Double() = BlasOperands.doubles(X, offsetX, "X", xo)
            Dim ys As Double() = BlasOperands.doubles(Y, offsetY, "Y", yo)
            Return ManagedBlas.Dot(BlasOperands.count(N), xs, xo, incX, ys, yo, incY)
        End Function

        Protected Friend Overrides Function snrm2(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Single
            Dim xo As Long
            Dim xs As Single() = BlasOperands.floats(X, "X", xo)
            Return ManagedBlas.Nrm2(BlasOperands.count(N), xs, xo, incX)
        End Function

        Protected Friend Overrides Function sasum(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Single
            Dim xo As Long
            Dim xs As Single() = BlasOperands.floats(X, "X", xo)
            Return ManagedBlas.Asum(BlasOperands.count(N), xs, xo, incX)
        End Function

        Protected Friend Overrides Function sasum(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer) As Single
            Dim xo As Long
            Dim xs As Single() = BlasOperands.floats(X, offsetX, "X", xo)
            Return ManagedBlas.Asum(BlasOperands.count(N), xs, xo, incX)
        End Function

        Protected Friend Overrides Function dnrm2(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Double
            Dim xo As Long
            Dim xs As Double() = BlasOperands.doubles(X, "X", xo)
            Return ManagedBlas.Nrm2(BlasOperands.count(N), xs, xo, incX)
        End Function

        Protected Friend Overrides Function dasum(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Double
            Dim xo As Long
            Dim xs As Double() = BlasOperands.doubles(X, "X", xo)
            Return ManagedBlas.Asum(BlasOperands.count(N), xs, xo, incX)
        End Function

        Protected Friend Overrides Function dasum(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer) As Double
            Dim xo As Long
            Dim xs As Double() = BlasOperands.doubles(X, offsetX, "X", xo)
            Return ManagedBlas.Asum(BlasOperands.count(N), xs, xo, incX)
        End Function

        Protected Friend Overrides Function isamax(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Integer
            Dim xo As Long
            Dim xs As Single() = BlasOperands.floats(X, "X", xo)
            Return ManagedBlas.Iamax(BlasOperands.count(N), xs, xo, incX)
        End Function

        Protected Friend Overrides Function isamax(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer) As Integer
            Dim xo As Long
            Dim xs As Single() = BlasOperands.floats(X, offsetX, "X", xo)
            Return ManagedBlas.Iamax(BlasOperands.count(N), xs, xo, incX)
        End Function

        Protected Friend Overrides Function idamax(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Integer
            Dim xo As Long
            Dim xs As Double() = BlasOperands.doubles(X, "X", xo)
            Return ManagedBlas.Iamax(BlasOperands.count(N), xs, xo, incX)
        End Function

        Protected Friend Overrides Function idamax(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer) As Integer
            Dim xo As Long
            Dim xs As Double() = BlasOperands.doubles(X, offsetX, "X", xo)
            Return ManagedBlas.Iamax(BlasOperands.count(N), xs, xo, incX)
        End Function

        Protected Friend Overrides Sub sswap(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)
            Dim xo, yo As Long
            Dim xs As Single() = BlasOperands.floats(X, "X", xo)
            Dim ys As Single() = BlasOperands.floats(Y, "Y", yo)
            ManagedBlas.Swap(BlasOperands.count(N), xs, xo, incX, ys, yo, incY)
        End Sub

        Protected Friend Overrides Sub scopy(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)
            Dim xo, yo As Long
            Dim xs As Single() = BlasOperands.floats(X, "X", xo)
            Dim ys As Single() = BlasOperands.floats(Y, "Y", yo)
            ManagedBlas.Copy(BlasOperands.count(N), xs, xo, incX, ys, yo, incY)
        End Sub

        Protected Friend Overrides Sub scopy(ByVal n As Long, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)
            Dim xo, yo As Long
            Dim xs As Single() = BlasOperands.floats(x, offsetX, "x", xo)
            Dim ys As Single() = BlasOperands.floats(y, offsetY, "y", yo)
            ManagedBlas.Copy(BlasOperands.count(n), xs, xo, incrX, ys, yo, incrY)
        End Sub

        Protected Friend Overrides Sub haxpy(ByVal N As Long, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)
            Throw BlasOperands.notSupported("haxpy")
        End Sub

        Protected Friend Overrides Sub saxpy(ByVal N As Long, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)
            Dim xo, yo As Long
            Dim xs As Single() = BlasOperands.floats(X, "X", xo)
            Dim ys As Single() = BlasOperands.floats(Y, "Y", yo)
            ManagedBlas.Axpy(BlasOperands.count(N), alpha, xs, xo, incX, ys, yo, incY)
        End Sub

        Protected Friend Overrides Sub haxpy(ByVal N As Long, ByVal alpha As Single, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)
            Throw BlasOperands.notSupported("haxpy")
        End Sub

        Protected Friend Overrides Sub saxpy(ByVal N As Long, ByVal alpha As Single, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)
            Dim xo, yo As Long
            Dim xs As Single() = BlasOperands.floats(x, offsetX, "x", xo)
            Dim ys As Single() = BlasOperands.floats(y, offsetY, "y", yo)
            ManagedBlas.Axpy(BlasOperands.count(N), alpha, xs, xo, incrX, ys, yo, incrY)
        End Sub

        Protected Friend Overrides Sub dswap(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)
            Dim xo, yo As Long
            Dim xs As Double() = BlasOperands.doubles(X, "X", xo)
            Dim ys As Double() = BlasOperands.doubles(Y, "Y", yo)
            ManagedBlas.Swap(BlasOperands.count(N), xs, xo, incX, ys, yo, incY)
        End Sub

        Protected Friend Overrides Sub dcopy(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)
            Dim xo, yo As Long
            Dim xs As Double() = BlasOperands.doubles(X, "X", xo)
            Dim ys As Double() = BlasOperands.doubles(Y, "Y", yo)
            ManagedBlas.Copy(BlasOperands.count(N), xs, xo, incX, ys, yo, incY)
        End Sub

        Protected Friend Overrides Sub dcopy(ByVal n As Long, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)
            Dim xo, yo As Long
            Dim xs As Double() = BlasOperands.doubles(x, offsetX, "x", xo)
            Dim ys As Double() = BlasOperands.doubles(y, offsetY, "y", yo)
            ManagedBlas.Copy(BlasOperands.count(n), xs, xo, incrX, ys, yo, incrY)
        End Sub

        Protected Friend Overrides Sub daxpy(ByVal N As Long, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)
            Dim xo, yo As Long
            Dim xs As Double() = BlasOperands.doubles(X, "X", xo)
            Dim ys As Double() = BlasOperands.doubles(Y, "Y", yo)
            ManagedBlas.Axpy(BlasOperands.count(N), alpha, xs, xo, incX, ys, yo, incY)
        End Sub

        Protected Friend Overrides Sub daxpy(ByVal N As Long, ByVal alpha As Double, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)
            Dim xo, yo As Long
            Dim xs As Double() = BlasOperands.doubles(x, offsetX, "x", xo)
            Dim ys As Double() = BlasOperands.doubles(y, offsetY, "y", yo)
            ManagedBlas.Axpy(BlasOperands.count(N), alpha, xs, xo, incrX, ys, yo, incrY)
        End Sub

        Protected Friend Overrides Sub srotg(ByVal a As Single, ByVal b As Single, ByVal c As Single, ByVal s As Single)
            Throw BlasOperands.notSupported("srotg")
        End Sub

        Protected Friend Overrides Sub srotmg(ByVal d1 As Single, ByVal d2 As Single, ByVal b1 As Single, ByVal b2 As Single, ByVal P As INDArray)
            Throw BlasOperands.notImplemented("srotmg")
        End Sub

        Protected Friend Overrides Sub srot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal c As Single, ByVal s As Single)
            Dim xo, yo As Long
            Dim xs As Single() = BlasOperands.floats(X, "X", xo)
            Dim ys As Single() = BlasOperands.floats(Y, "Y", yo)
            ManagedBlas.Rot(BlasOperands.count(N), xs, xo, incX, ys, yo, incY, c, s)
        End Sub

        Protected Friend Overrides Sub srotm(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal P As INDArray)
            Throw BlasOperands.notImplemented("srotm")
        End Sub

        Protected Friend Overrides Sub drotg(ByVal a As Double, ByVal b As Double, ByVal c As Double, ByVal s As Double)
            Throw BlasOperands.notSupported("drotg")
        End Sub

        Protected Friend Overrides Sub drotmg(ByVal d1 As Double, ByVal d2 As Double, ByVal b1 As Double, ByVal b2 As Double, ByVal P As INDArray)
            Throw BlasOperands.notImplemented("drotmg")
        End Sub

        Protected Friend Overrides Sub drot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal c As Double, ByVal s As Double)
            Dim xo, yo As Long
            Dim xs As Double() = BlasOperands.doubles(X, "X", xo)
            Dim ys As Double() = BlasOperands.doubles(Y, "Y", yo)
            ManagedBlas.Rot(BlasOperands.count(N), xs, xo, incX, ys, yo, incY, c, s)
        End Sub

        Protected Friend Overrides Sub drotm(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal P As INDArray)
            Throw BlasOperands.notImplemented("drotm")
        End Sub

        Protected Friend Overrides Sub sscal(ByVal N As Long, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer)
            Dim xo As Long
            Dim xs As Single() = BlasOperands.floats(X, "X", xo)
            ManagedBlas.Scal(BlasOperands.count(N), alpha, xs, xo, incX)
        End Sub

        Protected Friend Overrides Sub dscal(ByVal N As Long, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer)
            Dim xo As Long
            Dim xs As Double() = BlasOperands.doubles(X, "X", xo)
            ManagedBlas.Scal(BlasOperands.count(N), alpha, xs, xo, incX)
        End Sub

        Protected Friend Overrides Function hasum(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Single
            Throw BlasOperands.notSupported("hasum")
        End Function

        Protected Friend Overrides Function hasum(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer) As Single
            Throw BlasOperands.notSupported("hasum")
        End Function

    End Class

End Namespace
