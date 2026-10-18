Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape

Namespace org.nd4vb.linalg.api.iter

    ''' <summary>
    ''' Iterates over all the subscripts of a shape, in C ('c') or Fortran ('f') order: for shape [2, 2] in C order
    ''' the subscripts are [0,0], [0,1], [1,0], [1,1]. With <c>cache</c> = True the subscripts are precomputed once per
    ''' shape and order and shared (Java keeps the cache in a static map; here it is a synchronized dictionary keyed by
    ''' the shape contents and the order).
    ''' Java's <c>Iterator</c> methods are kept and the class is also an <see cref="IEnumerator(Of T)"/>.
    ''' </summary>
    Public Class NdIndexIterator
        Implements IEnumerator(Of Long())

        Private length As Integer = -1
        Private i As Integer = 0
        Private shape() As Long
        Private order As Char = "c"c
        Private cache As Boolean = False
        Private Shared ReadOnly lookupMap As New Dictionary(Of String, LinearIndexLookup)()
        Private lookup As LinearIndexLookup
        Private currentValue() As Long

        ''' <summary>
        ''' Pass in the shape to iterate over.
        ''' Defaults to c ordering </summary>
        ''' <param name="shape"> the shape to iterate over </param>
        Public Sub New(ParamArray ByVal shape() As Integer)
            Me.New("c"c, shape)
            Me.cache = False
        End Sub

        ''' <summary>
        ''' Pass in the shape to iterate over.
        ''' Defaults to c ordering </summary>
        ''' <param name="shape"> the shape to iterate over </param>
        Public Sub New(ParamArray ByVal shape() As Long)
            Me.New("c"c, False, shape)
            Me.cache = False
        End Sub

        ''' <summary>
        ''' Pass in the shape to iterate over </summary>
        ''' <param name="shape"> the shape to iterate over </param>
        Public Sub New(ByVal order As Char, ByVal cache As Boolean, ParamArray ByVal shape() As Long)
            Me.shape = CType(shape.Clone(), Long())
            Me.length = CInt(ArrayUtil.prodLong(shape))
            Me.order = order
            Me.cache = cache
            If Me.cache Then
                Dim key As String = order & ":" & String.Join(",", Array.ConvertAll(shape, Function(v) v.ToString(Globalization.CultureInfo.InvariantCulture)))
                SyncLock lookupMap
                    Dim found As LinearIndexLookup = Nothing
                    If Not lookupMap.TryGetValue(key, found) Then
                        found = New LinearIndexLookup(shape, order)
                        'warm up the cache
                        For n As Integer = 0 To length - 1
                            found.lookup(n)
                        Next n
                        lookupMap(key) = found
                    End If
                    Me.lookup = found
                End SyncLock
            End If
        End Sub

        Public Sub New(ByVal order As Char, ParamArray ByVal shape() As Integer)
            Me.New(order, False, ArrayUtil.toLongArray(shape))
        End Sub

        Public Sub New(ByVal order As Char, ParamArray ByVal shape() As Long)
            Me.New(order, False, shape)
        End Sub

        Public Overridable Function hasNext() As Boolean
            Return i < length
        End Function

        Public Overridable Function [next]() As Long()
            If lookup IsNot Nothing Then
                Dim cached As Long() = lookup.lookup(i)
                i += 1
                Return cached
            End If

            Select Case order
                Case "c"c
                    Dim c As Long() = ShapeT.ind2subC(shape, i)
                    i += 1
                    Return c
                Case "f"c
                    Dim f As Long() = ShapeT.ind2sub(shape, i)
                    i += 1
                    Return f
                Case Else
                    Throw New ArgumentException("Illegal ordering " & order)
            End Select
        End Function

        Public Overridable Sub remove()
        End Sub

        Public ReadOnly Property Current As Long() Implements IEnumerator(Of Long()).Current
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
