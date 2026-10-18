Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray

Namespace org.nd4vb.linalg.api.iter

    ''' <summary>
    ''' Iterates over the slices along the first axis of an array; scalar slices are returned as their
    ''' <c>Double</c> value. Java's <c>Iterator</c> methods (<see cref="hasNext"/>, <see cref="[next]"/>,
    ''' <see cref="remove"/>) are kept, and the class is also an <see cref="IEnumerator(Of Object)"/>.
    ''' </summary>
    Public Class FirstAxisIterator
        Implements IEnumerator(Of Object)

        Private iterateOver As INDArray
        Private i As Integer = 0
        Private currentValue As Object

        Public Sub New(ByVal iterateOver As INDArray)
            Me.iterateOver = iterateOver
        End Sub

        Public Overridable Function hasNext() As Boolean
            Return i < iterateOver.slices()
        End Function

        Public Overridable Sub remove()
        End Sub

        Public Overridable Function [next]() As Object
            Dim s As INDArray = iterateOver.slice(i)
            i += 1
            If s.Scalar Then
                Return s.getDouble(0L)
            Else
                Return s
            End If
        End Function

        Public ReadOnly Property Current As Object Implements IEnumerator(Of Object).Current
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
            currentValue = Nothing
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
        End Sub

    End Class

End Namespace
