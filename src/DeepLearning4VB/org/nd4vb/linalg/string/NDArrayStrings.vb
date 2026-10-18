Imports System
Imports System.Globalization
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException

Namespace org.nd4vb.linalg.string

    ''' <summary>
    ''' Formats an <see cref="INDArray"/> as a string (Java <c>NDArrayStrings</c>).
    ''' <para>
    ''' Java's <c>java.text.DecimalFormat</c> patterns ("0.0000", "0.####", "0.####E0") are applied as .NET custom
    ''' numeric format strings with the invariant culture; the only difference is the rounding of exact decimal ties
    ''' (Java rounds half-even, .NET away from zero). The Lombok accessors are written out; the decimal format is
    ''' exposed as its pattern string.
    ''' </para>
    ''' </summary>
    Public Class NDArrayStrings

        Public Const EMPTY_ARRAY_STR As String = "[]"

        Private Shared ReadOnly OPEN_BRACKETS() As String = {"", "[", "[[", "[[[", "[[[[", "[[[[[", "[[[[[[", "[[[[[[[", "[[[[[[[["}
        Private Shared ReadOnly CLOSE_BRACKETS() As String = {"", "]", "]]", "]]]", "]]]]", "]]]]]", "]]]]]]", "]]]]]]]", "]]]]]]]]"}

        ''' <summary>
        ''' The default number of elements for printing INDArrays (via NDArrayStrings or INDArray.toString)
        ''' </summary>
        Public Const DEFAULT_MAX_PRINT_ELEMENTS As Long = 1000

        ''' <summary>
        ''' The maximum number of elements to print by default for INDArray.toString()
        ''' Default value is 1000 - given by <see cref="DEFAULT_MAX_PRINT_ELEMENTS"/>
        ''' </summary>
        Private Shared maxPrintElements As Long = DEFAULT_MAX_PRINT_ELEMENTS

        Private localMaxPrintElements As Long = maxPrintElements
        Private colSep As String = ","
        Private newLineSep As String = ","
        Private padding As Integer = 7
        Private precision As Integer = 4
        Private minToPrintWithoutSwitching As Double
        Private maxToPrintWithoutSwitching As Double
        Private scientificFormat As String = ""
        Private decimalFormat As String
        Private dontOverrideFormat As Boolean = False

        Public Sub New()
            Me.New(",", 4)
        End Sub

        Public Sub New(ByVal colSep As String)
            Me.New(colSep, 4)
        End Sub

        ''' <summary>
        ''' Specify the number of digits after the decimal point to include </summary>
        ''' <param name="precision"> </param>
        Public Sub New(ByVal precision As Integer)
            Me.New(",", precision)
        End Sub

        Public Sub New(ByVal maxElements As Long, ByVal precision As Integer)
            Me.New(",", precision)
            Me.localMaxPrintElements = maxElements
        End Sub

        Public Sub New(ByVal maxElements As Long)
            Me.New()
            Me.localMaxPrintElements = maxElements
        End Sub

        Public Sub New(ByVal maxElements As Long, ByVal forceSummarize As Boolean, ByVal precision As Integer)
            Me.New(",", precision)
            If forceSummarize Then
                localMaxPrintElements = 0
            Else
                localMaxPrintElements = maxElements
            End If
        End Sub

        Public Sub New(ByVal forceSummarize As Boolean, ByVal precision As Integer)
            Me.New(",", precision)
            If forceSummarize Then
                localMaxPrintElements = 0
            End If
        End Sub

        Public Sub New(ByVal forceSummarize As Boolean)
            Me.New(",", 4)
            If forceSummarize Then
                localMaxPrintElements = 0
            End If
        End Sub

        ''' <summary>
        ''' Specify a delimiter for elements in columns for 2d arrays (or in the rank-1th dimension in higher order arrays)
        ''' Separator in elements in remaining dimensions defaults to ",\n"
        ''' </summary>
        ''' <param name="colSep">    field separating columns; </param>
        ''' <param name="precision"> digits after decimal point </param>
        Public Sub New(ByVal colSep As String, ByVal precision As Integer)
            Me.colSep = colSep
            If Not Regex.Replace(colSep, "\s", "").Equals(",") Then
                Me.newLineSep = ""
            End If
            Dim decFormatNum As New StringBuilder("0.")
            Dim prec As Integer = Math.Abs(precision)
            Me.precision = prec
            Dim useHash As Boolean = precision < 0
            Do While prec > 0
                decFormatNum.Append(If(useHash, "#", "0"))
                prec -= 1
            Loop
            Me.decimalFormat = decFormatNum.ToString()
        End Sub

        ''' <summary>
        ''' Specify a col separator and a decimal format string </summary>
        ''' <param name="colSep"> </param>
        ''' <param name="decFormat"> </param>
        Public Sub New(ByVal colSep As String, ByVal decFormat As String)
            Me.colSep = colSep
            Me.decimalFormat = decFormat
            If decFormat.ToUpperInvariant().Contains("E") Then
                Me.padding = decFormat.Length + 3
            Else
                Me.padding = decFormat.Length + 1
            End If
            Me.dontOverrideFormat = True
        End Sub

        ''' <param name="arr"> </param>
        ''' <returns> String representation of the array adhering to options provided in the constructor </returns>
        Public Overridable Function format(ByVal arr As INDArray) As String
            Return format(arr, True)
        End Function

        ''' <summary>
        ''' Format the given ndarray as a string
        ''' </summary>
        ''' <param name="arr">       the array to format </param>
        ''' <param name="summarize"> If true and the number of elements in the array is greater than > 1000 only the first three and last elements in any dimension will print </param>
        ''' <returns> the formatted array </returns>
        Public Overridable Function format(ByVal arr As INDArray, ByVal summarize As Boolean) As String
            If arr.Empty Then
                Return EMPTY_ARRAY_STR
            End If
            Me.scientificFormat = "0."
            Dim addPrecision As Integer = Me.precision
            Do While addPrecision > 0
                Me.scientificFormat &= "#"
                addPrecision -= 1
            Loop
            Me.scientificFormat = Me.scientificFormat & "E0"
            If Me.scientificFormat.Length + 2 > Me.padding Then
                Me.padding = Me.scientificFormat.Length + 2
            End If
            Me.maxToPrintWithoutSwitching = Math.Pow(10, Me.precision)
            Me.minToPrintWithoutSwitching = 1.0 / (Me.maxToPrintWithoutSwitching)
            Return format(arr, 0, summarize AndAlso arr.length() > localMaxPrintElements)
        End Function

        Private Function format(ByVal arr As INDArray, ByVal offset As Integer, ByVal summarize As Boolean) As String
            Dim rank As Integer = arr.rank()
            If arr.Scalar OrElse arr.length() = 1 Then
                Dim fRank As Integer = Math.Min(rank, OPEN_BRACKETS.Length - 1)
                If arr.R Then
                    Dim arrElement As Double = arr.getDouble(0L)
                    If Not dontOverrideFormat AndAlso ((Math.Abs(arrElement) < Me.minToPrintWithoutSwitching AndAlso arrElement <> 0) OrElse (Math.Abs(arrElement) >= Me.maxToPrintWithoutSwitching)) Then
                        'switch to scientific notation
                        Dim asString As String = formatNumber(arrElement, scientificFormat)
                        'from E to small e
                        asString = asString.Replace("E"c, "e"c)
                        Return OPEN_BRACKETS(fRank) & asString & CLOSE_BRACKETS(fRank)
                    Else
                        If arr.getDouble(0L) = 0 Then
                            Return OPEN_BRACKETS(fRank) & "0" & CLOSE_BRACKETS(fRank)
                        End If
                        Return OPEN_BRACKETS(fRank) & formatNumber(arr.getDouble(0L), decimalFormat) & CLOSE_BRACKETS(fRank)
                    End If
                ElseIf arr.Z Then
                    Dim arrElement As Long = arr.getLong(0L)
                    Return OPEN_BRACKETS(fRank) & arrElement.ToString(CultureInfo.InvariantCulture) & CLOSE_BRACKETS(fRank)
                ElseIf arr.B Then
                    Dim arrElement As Long = arr.getLong(0L)
                    Return OPEN_BRACKETS(fRank) & (If(arrElement = 0, "false", "true")) & CLOSE_BRACKETS(fRank)
                ElseIf arr.dataType() = DataType.UTF8 Then
                    Dim s As String = arr.getString(0)
                    Return OPEN_BRACKETS(fRank) & """" & s.Replace(vbLf, "\n") & """" & CLOSE_BRACKETS(fRank)
                Else
                    Throw New ND4VBIllegalStateException()
                End If
            ElseIf rank = 1 Then
                'true vector
                Return vectorToString(arr, summarize)
            ElseIf arr.RowVector Then
                'a slice from a higher dim array
                If offset = 0 Then
                    Dim sb As New StringBuilder()
                    sb.Append("[")
                    sb.Append(vectorToString(arr, summarize))
                    sb.Append("]")
                    Return sb.ToString()
                End If
                Return vectorToString(arr, summarize)
            Else
                offset += 1
                Dim sb As New StringBuilder()
                sb.Append("[")
                Dim nSlices As Long = arr.slices()
                Dim i As Integer = 0
                Do While i < nSlices
                    If summarize AndAlso i > 2 AndAlso i < nSlices - 3 Then
                        sb.Append(" ...")
                        sb.Append(newLineSep).Append(" " & vbLf)
                        sb.Append(repeat(vbLf, rank - 2))
                        sb.Append(repeat(" ", offset))
                        ' immediately jump to the last slices so we only print ellipsis once
                        i = Math.Max(i, CInt(nSlices) - 4)
                    Else
                        If arr.rank() = 3 AndAlso arr.slice(i).RowVector Then
                            sb.Append("[")
                        End If
                        'hack fix for slice issue with 'f' order
                        If arr.ordering() = "f"c AndAlso arr.rank() > 2 AndAlso arr.size(arr.rank() - 1) = 1 Then
                            sb.Append(format(arr.dup("c"c).slice(i), offset, summarize))
                        Else
                            sb.Append(format(arr.slice(i), offset, summarize))
                        End If
                        If i <> nSlices - 1 Then
                            If arr.rank() = 3 AndAlso arr.slice(i).RowVector Then
                                sb.Append("]")
                            End If
                            sb.Append(newLineSep).Append(" " & vbLf)
                            sb.Append(repeat(vbLf, rank - 2))
                            sb.Append(repeat(" ", offset))
                        Else
                            If arr.rank() = 3 AndAlso arr.slice(i).RowVector Then
                                sb.Append("]")
                            End If
                        End If
                    End If
                    i += 1
                Loop
                sb.Append("]")
                Return sb.ToString()
            End If
        End Function

        Private Function vectorToString(ByVal arr As INDArray, ByVal summarize As Boolean) As String
            Dim sb As New StringBuilder()
            sb.Append("[")
            Dim l As Long = arr.length()
            Dim i As Integer = 0
            Do While i < l
                If summarize AndAlso i > 2 AndAlso i < l - 3 Then
                    sb.Append("  ...")
                    ' immediately jump to the last elements so we only print ellipsis once
                    i = Math.Max(i, CInt(l) - 4)
                Else
                    If arr.R Then
                        Dim arrElement As Double = arr.getDouble(CLng(i))
                        If Not dontOverrideFormat AndAlso ((Math.Abs(arrElement) < Me.minToPrintWithoutSwitching AndAlso arrElement <> 0) OrElse (Math.Abs(arrElement) >= Me.maxToPrintWithoutSwitching)) Then
                            'switch to scientific notation
                            Dim asString As String = formatNumber(arrElement, scientificFormat)
                            'from E to small e
                            asString = asString.Replace("E"c, "e"c)
                            sb.Append(asString.PadLeft(padding))
                        Else
                            If arrElement = 0 Then
                                sb.Append("0".PadLeft(padding))
                            Else
                                sb.Append(formatNumber(arrElement, decimalFormat).PadLeft(padding))
                            End If
                        End If
                    ElseIf arr.Z Then
                        Dim arrElement As Long = arr.getLong(CLng(i))
                        sb.Append(arrElement.ToString(CultureInfo.InvariantCulture).PadLeft(padding))
                    ElseIf arr.B Then
                        Dim arrElement As Long = arr.getLong(CLng(i))
                        sb.Append((If(arrElement = 0, "false", "true")).PadLeft(padding))
                    ElseIf arr.dataType() = DataType.UTF8 Then
                        Dim s As String = arr.getString(i)
                        s = """" & s.Replace(vbLf, "\n") & """"
                        sb.Append(s)
                    End If
                End If
                If i < l - 1 Then
                    If Not summarize OrElse i <= 2 OrElse i >= l - 3 OrElse (summarize AndAlso l = 6) Then
                        sb.Append(colSep)
                    End If
                End If
                i += 1
            Loop
            sb.Append("]")
            Return sb.ToString()
        End Function

        ''' <summary>Java <c>new DecimalFormat(pattern, DecimalFormatSymbols.getInstance(Locale.US)).format(value)</c>.</summary>
        Private Shared Function formatNumber(ByVal value As Double, ByVal pattern As String) As String
            If Double.IsNaN(value) Then Return "NaN"
            If Double.IsPositiveInfinity(value) Then Return "∞"
            If Double.IsNegativeInfinity(value) Then Return "-∞"
            Dim s As String = value.ToString(pattern, CultureInfo.InvariantCulture)
            ' DecimalFormat drops a trailing decimal point left by an all-optional ("0.###") fraction
            If s.EndsWith(".", StringComparison.Ordinal) Then s = s.Substring(0, s.Length - 1)
            Return s
        End Function

        ''' <summary>Apache Commons <c>StringUtils.repeat</c>: an empty string for a non-positive count.</summary>
        Private Shared Function repeat(ByVal s As String, ByVal count As Integer) As String
            If count <= 0 Then Return ""
            Dim sb As New StringBuilder(s.Length * count)
            For n As Integer = 1 To count
                sb.Append(s)
            Next
            Return sb.ToString()
        End Function

        Public Shared Function getMaxPrintElements() As Long
            Return maxPrintElements
        End Function

        Public Shared Sub setMaxPrintElements(ByVal value As Long)
            maxPrintElements = value
        End Sub

        Public Overridable Function getLocalMaxPrintElements() As Long
            Return localMaxPrintElements
        End Function

        Public Overridable Sub setLocalMaxPrintElements(ByVal value As Long)
            localMaxPrintElements = value
        End Sub

        Public Overridable Function getColSep() As String
            Return colSep
        End Function

        Public Overridable Sub setColSep(ByVal value As String)
            colSep = value
        End Sub

        Public Overridable Function getNewLineSep() As String
            Return newLineSep
        End Function

        Public Overridable Sub setNewLineSep(ByVal value As String)
            newLineSep = value
        End Sub

        Public Overridable Function getPadding() As Integer
            Return padding
        End Function

        Public Overridable Sub setPadding(ByVal value As Integer)
            padding = value
        End Sub

        Public Overridable Function getPrecision() As Integer
            Return precision
        End Function

        Public Overridable Sub setPrecision(ByVal value As Integer)
            precision = value
        End Sub

        Public Overridable Function getMinToPrintWithoutSwitching() As Double
            Return minToPrintWithoutSwitching
        End Function

        Public Overridable Sub setMinToPrintWithoutSwitching(ByVal value As Double)
            minToPrintWithoutSwitching = value
        End Sub

        Public Overridable Function getMaxToPrintWithoutSwitching() As Double
            Return maxToPrintWithoutSwitching
        End Function

        Public Overridable Sub setMaxToPrintWithoutSwitching(ByVal value As Double)
            maxToPrintWithoutSwitching = value
        End Sub

        Public Overridable Function getScientificFormat() As String
            Return scientificFormat
        End Function

        Public Overridable Sub setScientificFormat(ByVal value As String)
            scientificFormat = value
        End Sub

        ''' <summary>The decimal format pattern (Java returns the <c>DecimalFormat</c> object).</summary>
        Public Overridable Function getDecimalFormat() As String
            Return decimalFormat
        End Function

        Public Overridable Sub setDecimalFormat(ByVal value As String)
            decimalFormat = value
        End Sub

        Public Overridable Function isDontOverrideFormat() As Boolean
            Return dontOverrideFormat
        End Function

        Public Overridable Sub setDontOverrideFormat(ByVal value As Boolean)
            dontOverrideFormat = value
        End Sub

    End Class

End Namespace
