' DeepLearning4VB: Java's java.lang.Number conversions for boxed .NET numbers (phase 2, see docs/PHASE2_DESIGN.md).

Imports System
Imports System.Globalization
Imports NumericCasts = DeepLearning4VB.org.nd4vb.linalg.managed.storage.NumericCasts

Namespace org.nd4vb.linalg.api.buffer

    ''' <summary>
    ''' The <c>intValue()</c>, <c>longValue()</c>, <c>floatValue()</c> and <c>doubleValue()</c> methods of Java's
    ''' <c>Number</c> for a boxed .NET number (Java <c>Number</c> parameters are <c>Object</c> in the port): floating-point
    ''' values convert with Java's saturating casts (NaN gives 0), longs narrow by wrapping, booleans are 1 and 0, and
    ''' strings are parsed in invariant culture.
    ''' </summary>
    Friend NotInheritable Class JavaNumber

        Private Sub New()
        End Sub

        Private Shared Function isFloating(ByVal value As Object) As Boolean
            Return TypeOf value Is Double OrElse TypeOf value Is Single OrElse TypeOf value Is Decimal
        End Function

        ''' <summary>Java <c>Number.doubleValue()</c>.</summary>
        Public Shared Function doubleValue(ByVal value As Object) As Double
            If value Is Nothing Then
                Throw New NullReferenceException("Number is null")
            End If
            If TypeOf value Is Boolean Then
                Return If(DirectCast(value, Boolean), 1.0, 0.0)
            End If
            If TypeOf value Is ULong Then
                Return NumericCasts.ULongToDouble(DirectCast(value, ULong))
            End If
            Return Convert.ToDouble(value, CultureInfo.InvariantCulture)
        End Function

        ''' <summary>Java <c>Number.floatValue()</c>.</summary>
        Public Shared Function floatValue(ByVal value As Object) As Single
            If TypeOf value Is Single Then
                Return DirectCast(value, Single)
            End If
            If TypeOf value Is Long Then
                Return CSng(DirectCast(value, Long))
            End If
            If TypeOf value Is ULong Then
                Return NumericCasts.ULongToSingle(DirectCast(value, ULong))
            End If
            Return CSng(doubleValue(value))
        End Function

        ''' <summary>Java <c>Number.longValue()</c>.</summary>
        Public Shared Function longValue(ByVal value As Object) As Long
            If value Is Nothing Then
                Throw New NullReferenceException("Number is null")
            End If
            If TypeOf value Is Long Then
                Return DirectCast(value, Long)
            End If
            If TypeOf value Is ULong Then
                Return NumericCasts.ULongToLong(DirectCast(value, ULong))
            End If
            If TypeOf value Is Boolean Then
                Return If(DirectCast(value, Boolean), 1L, 0L)
            End If
            If isFloating(value) OrElse TypeOf value Is String Then
                Return NumericCasts.DoubleToLong(doubleValue(value))
            End If
            Return Convert.ToInt64(value, CultureInfo.InvariantCulture)
        End Function

        ''' <summary>Java <c>Number.intValue()</c>.</summary>
        Public Shared Function intValue(ByVal value As Object) As Integer
            If TypeOf value Is Integer Then
                Return DirectCast(value, Integer)
            End If
            If isFloating(value) OrElse TypeOf value Is String Then
                Return NumericCasts.DoubleToInt(doubleValue(value))
            End If
            Return NumericCasts.LongToInt(longValue(value))
        End Function

    End Class

End Namespace
