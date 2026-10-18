Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape

Namespace org.nd4vb.linalg.api.iter

    ''' <summary>
    ''' Iterates over the elements of an array in its own order (C or Fortran), as doubles. Java's <c>Iterator</c>
    ''' methods are kept and the class is also an <see cref="IEnumerator(Of Double)"/>. The subscripts come from
    ''' <c>Shape.ind2sub</c>/<c>Shape.ind2subC</c>.
    ''' </summary>
    Public Class INDArrayIterator
        Implements IEnumerator(Of Double)

        Private iterateOver As INDArray
        Private i As Integer = 0
        Private currentValue As Double

        Public Sub New(ByVal iterateOver As INDArray)
            Me.iterateOver = iterateOver
        End Sub

        Public Overridable Function hasNext() As Boolean
            Return i < iterateOver.length()
        End Function

        Public Overridable Sub remove()
        End Sub

        Public Overridable Function [next]() As Double
            Dim index As Long = i
            i += 1
            Return iterateOver.getDouble(If(iterateOver.ordering() = "c"c, ShapeT.ind2subC(iterateOver, index), ShapeT.ind2sub(iterateOver, index)))
        End Function

        Public ReadOnly Property Current As Double Implements IEnumerator(Of Double).Current
            Get
                Return currentValue
            End Get
        End Property

        Private ReadOnly Property CurrentObject As Object Implements IEnumerator.Current
            Get
                Return currentValue
            End Get
        End Property

        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
            If Not hasNext() Then Return False
            currentValue = [next]()
            Return True
        End Function

        Public Sub Reset() Implements IEnumerator.Reset
            i = 0
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
        End Sub

    End Class

End Namespace
