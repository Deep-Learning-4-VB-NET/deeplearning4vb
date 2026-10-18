Imports System

Namespace org.nd4vb.linalg.api.ndarray

    ''' <summary>
    ''' Summary statistics of an array (Java <c>INDArrayStatistics</c>, used by <c>OpExecutioner.inspectArray</c> and the
    ''' op profiler). Lombok's <c>@Builder @AllArgsConstructor @NoArgsConstructor @Data</c> members are written out.
    ''' </summary>
    Public Class INDArrayStatistics

        Private minValue As Double
        Private maxValue As Double
        Private meanValue As Double
        Private stdDevValue As Double
        Private countPositive As Long
        Private countNegative As Long
        Private countZero As Long
        Private countInf As Long
        Private countNaN As Long

        Public Sub New()
        End Sub

        Public Sub New(ByVal minValue As Double, ByVal maxValue As Double, ByVal meanValue As Double, ByVal stdDevValue As Double, ByVal countPositive As Long, ByVal countNegative As Long, ByVal countZero As Long, ByVal countInf As Long, ByVal countNaN As Long)
            Me.minValue = minValue
            Me.maxValue = maxValue
            Me.meanValue = meanValue
            Me.stdDevValue = stdDevValue
            Me.countPositive = countPositive
            Me.countNegative = countNegative
            Me.countZero = countZero
            Me.countInf = countInf
            Me.countNaN = countNaN
        End Sub

        Public Shared Function builder() As INDArrayStatisticsBuilder
            Return New INDArrayStatisticsBuilder()
        End Function

        Public Overridable Function getMinValue() As Double
            Return minValue
        End Function

        Public Overridable Sub setMinValue(ByVal minValue As Double)
            Me.minValue = minValue
        End Sub

        Public Overridable Function getMaxValue() As Double
            Return maxValue
        End Function

        Public Overridable Sub setMaxValue(ByVal maxValue As Double)
            Me.maxValue = maxValue
        End Sub

        Public Overridable Function getMeanValue() As Double
            Return meanValue
        End Function

        Public Overridable Sub setMeanValue(ByVal meanValue As Double)
            Me.meanValue = meanValue
        End Sub

        Public Overridable Function getStdDevValue() As Double
            Return stdDevValue
        End Function

        Public Overridable Sub setStdDevValue(ByVal stdDevValue As Double)
            Me.stdDevValue = stdDevValue
        End Sub

        Public Overridable Function getCountPositive() As Long
            Return countPositive
        End Function

        Public Overridable Sub setCountPositive(ByVal countPositive As Long)
            Me.countPositive = countPositive
        End Sub

        Public Overridable Function getCountNegative() As Long
            Return countNegative
        End Function

        Public Overridable Sub setCountNegative(ByVal countNegative As Long)
            Me.countNegative = countNegative
        End Sub

        Public Overridable Function getCountZero() As Long
            Return countZero
        End Function

        Public Overridable Sub setCountZero(ByVal countZero As Long)
            Me.countZero = countZero
        End Sub

        Public Overridable Function getCountInf() As Long
            Return countInf
        End Function

        Public Overridable Sub setCountInf(ByVal countInf As Long)
            Me.countInf = countInf
        End Sub

        Public Overridable Function getCountNaN() As Long
            Return countNaN
        End Function

        Public Overridable Sub setCountNaN(ByVal countNaN As Long)
            Me.countNaN = countNaN
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim o As INDArrayStatistics = TryCast(obj, INDArrayStatistics)
            If o Is Nothing OrElse CObj(o).GetType() IsNot CObj(Me).GetType() Then Return False
            Return minValue.Equals(o.minValue) AndAlso maxValue.Equals(o.maxValue) AndAlso meanValue.Equals(o.meanValue) AndAlso stdDevValue.Equals(o.stdDevValue) AndAlso countPositive.Equals(o.countPositive) AndAlso countNegative.Equals(o.countNegative) AndAlso countZero.Equals(o.countZero) AndAlso countInf.Equals(o.countInf) AndAlso countNaN.Equals(o.countNaN)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            h = h * 59 + minValue.GetHashCode()
            h = h * 59 + maxValue.GetHashCode()
            h = h * 59 + meanValue.GetHashCode()
            h = h * 59 + stdDevValue.GetHashCode()
            h = h * 59 + countPositive.GetHashCode()
            h = h * 59 + countNegative.GetHashCode()
            h = h * 59 + countZero.GetHashCode()
            h = h * 59 + countInf.GetHashCode()
            h = h * 59 + countNaN.GetHashCode()
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "INDArrayStatistics(" & "minValue=" & Arrays.JavaDoubleToString(minValue) & ", maxValue=" & Arrays.JavaDoubleToString(maxValue) & ", meanValue=" & Arrays.JavaDoubleToString(meanValue) & ", stdDevValue=" & Arrays.JavaDoubleToString(stdDevValue) & ", countPositive=" & countPositive.ToString() & ", countNegative=" & countNegative.ToString() & ", countZero=" & countZero.ToString() & ", countInf=" & countInf.ToString() & ", countNaN=" & countNaN.ToString() & ")"
        End Function

        ''' <summary>Lombok's builder for <see cref="INDArrayStatistics"/>.</summary>
        Public Class INDArrayStatisticsBuilder

            Private minValueValue As Double
            Private maxValueValue As Double
            Private meanValueValue As Double
            Private stdDevValueValue As Double
            Private countPositiveValue As Long
            Private countNegativeValue As Long
            Private countZeroValue As Long
            Private countInfValue As Long
            Private countNaNValue As Long

            Public Function minValue(ByVal value As Double) As INDArrayStatisticsBuilder
                Me.minValueValue = value
                Return Me
            End Function

            Public Function maxValue(ByVal value As Double) As INDArrayStatisticsBuilder
                Me.maxValueValue = value
                Return Me
            End Function

            Public Function meanValue(ByVal value As Double) As INDArrayStatisticsBuilder
                Me.meanValueValue = value
                Return Me
            End Function

            Public Function stdDevValue(ByVal value As Double) As INDArrayStatisticsBuilder
                Me.stdDevValueValue = value
                Return Me
            End Function

            Public Function countPositive(ByVal value As Long) As INDArrayStatisticsBuilder
                Me.countPositiveValue = value
                Return Me
            End Function

            Public Function countNegative(ByVal value As Long) As INDArrayStatisticsBuilder
                Me.countNegativeValue = value
                Return Me
            End Function

            Public Function countZero(ByVal value As Long) As INDArrayStatisticsBuilder
                Me.countZeroValue = value
                Return Me
            End Function

            Public Function countInf(ByVal value As Long) As INDArrayStatisticsBuilder
                Me.countInfValue = value
                Return Me
            End Function

            Public Function countNaN(ByVal value As Long) As INDArrayStatisticsBuilder
                Me.countNaNValue = value
                Return Me
            End Function

            Public Function build() As INDArrayStatistics
                Return New INDArrayStatistics(minValueValue, maxValueValue, meanValueValue, stdDevValueValue, countPositiveValue, countNegativeValue, countZeroValue, countInfValue, countNaNValue)
            End Function

        End Class

    End Class

End Namespace
