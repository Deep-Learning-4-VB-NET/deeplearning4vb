Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil

Namespace org.nd4vb.linalg.api.iter

    ''' <summary>
    ''' Java's coordinate iterator over a shape (Java <c>FlatIterator</c>), kept as it is: it counts
    ''' <c>prod(shape)</c> steps but only advances the last coordinate (the Java implementation never carries into
    ''' the other dimensions). Java's <c>Iterator</c> methods are kept and the class is also an
    ''' <see cref="IEnumerator(Of T)"/>; the returned array is the iterator's own coordinate buffer.
    ''' </summary>
    Public Class FlatIterator
        Implements IEnumerator(Of Integer())

        Private shape() As Integer
        Private runningDimension As Integer
        Private currentCoord() As Integer
        Private length As Integer
        Private currentIndex As Integer = 0
        Private currentValue() As Integer

        Public Sub New(ByVal shape() As Integer)
            Me.shape = shape
            Me.currentCoord = New Integer(shape.Length - 1) {}
            length = ArrayUtil.prod(shape)
        End Sub

        Public Overridable Sub remove()
        End Sub

        Public Overridable Function hasNext() As Boolean
            Return currentIndex < length
        End Function

        Public Overridable Function [next]() As Integer()
            If currentCoord(runningDimension) = shape(runningDimension) Then
                runningDimension -= 1
                currentCoord(runningDimension) = 0
            Else
                'bump to the next coordinate
                currentCoord(runningDimension) += 1
            End If
            currentIndex += 1
            Return currentCoord
        End Function

        Public ReadOnly Property Current As Integer() Implements IEnumerator(Of Integer()).Current
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
            Throw New NotSupportedException("FlatIterator cannot be reset")
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
        End Sub

    End Class

End Namespace
