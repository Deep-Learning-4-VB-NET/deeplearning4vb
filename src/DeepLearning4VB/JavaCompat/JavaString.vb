Imports System
Imports System.Globalization
Imports System.Text
Imports System.Text.RegularExpressions

''' <summary>
''' Equivalent of <c>java.lang.String.format</c> / <c>java.util.Formatter</c> for the subset of
''' format specifiers the ported code uses: <c>%s %S %d %x %X %f %e %E %g %G %c %b %B %h %n %%</c>
''' with optional argument index (<c>1$</c>), flags (<c>- + 0 , ( space #</c>), width and precision.
''' Formatting is culture-invariant, as Java's default locale output is for these specifiers.
''' </summary>
Friend Module JavaString

    Private ReadOnly specifier As New Regex("%(?:(\d+)\$)?([-#+ 0,(]*)(\d+)?(?:\.(\d+))?([a-zA-Z%])", RegexOptions.Compiled)

    Public Function format(ByVal fmt As String, ByVal ParamArray args() As Object) As String
        If fmt Is Nothing Then Throw New ArgumentNullException(NameOf(fmt))
        If args Is Nothing Then args = New Object() {Nothing}
        Dim sb As New StringBuilder(fmt.Length + 16)
        Dim last As Integer = 0
        Dim nextArg As Integer = 0
        For Each m As Match In specifier.Matches(fmt)
            sb.Append(fmt, last, m.Index - last)
            last = m.Index + m.Length
            Dim conv As Char = m.Groups(5).Value(0)
            If conv = "n"c Then
                sb.Append(Environment.NewLine)
                Continue For
            ElseIf conv = "%"c Then
                sb.Append("%"c)
                Continue For
            End If
            Dim arg As Object
            If m.Groups(1).Success Then
                Dim idx As Integer = Integer.Parse(m.Groups(1).Value, CultureInfo.InvariantCulture) - 1
                If idx < 0 OrElse idx >= args.Length Then Throw New FormatException("Format specifier '" & m.Value & "' refers to a missing argument")
                arg = args(idx)
            Else
                If nextArg >= args.Length Then Throw New FormatException("Format specifier '" & m.Value & "' has no matching argument")
                arg = args(nextArg)
                nextArg += 1
            End If
            Dim flags As String = m.Groups(2).Value
            Dim width As Integer = If(m.Groups(3).Success, Integer.Parse(m.Groups(3).Value, CultureInfo.InvariantCulture), -1)
            Dim precision As Integer = If(m.Groups(4).Success, Integer.Parse(m.Groups(4).Value, CultureInfo.InvariantCulture), -1)
            sb.Append(Pad(Convert(conv, arg, flags, precision, width), width, flags))
        Next
        sb.Append(fmt, last, fmt.Length - last)
        Return sb.ToString()
    End Function

    Private Function Pad(ByVal s As String, ByVal width As Integer, ByVal flags As String) As String
        If width < 0 OrElse s.Length >= width Then Return s
        If flags.IndexOf("-"c) >= 0 Then Return s.PadRight(width)
        Return s.PadLeft(width)
    End Function

    Private Function Convert(ByVal conv As Char, ByVal arg As Object, ByVal flags As String, ByVal precision As Integer, ByVal width As Integer) As String
        Select Case conv
            Case "s"c, "S"c
                Dim s As String = Arrays.FormatElement(arg)
                If precision >= 0 AndAlso s.Length > precision Then s = s.Substring(0, precision)
                Return If(conv = "S"c, s.ToUpperInvariant(), s)
            Case "d"c
                If arg Is Nothing Then Return "null"
                Return FormatInteger(System.Convert.ToInt64(arg, CultureInfo.InvariantCulture), flags, width)
            Case "x"c, "X"c
                If arg Is Nothing Then Return "null"
                Dim hex As String
                If TypeOf arg Is Long Then
                    hex = CULng(BitConverter.ToUInt64(BitConverter.GetBytes(CLng(arg)), 0)).ToString("x", CultureInfo.InvariantCulture)
                ElseIf TypeOf arg Is Byte OrElse TypeOf arg Is SByte Then
                    hex = (System.Convert.ToInt32(arg) And &HFF).ToString("x", CultureInfo.InvariantCulture)
                ElseIf TypeOf arg Is Short Then
                    hex = (System.Convert.ToInt32(arg) And &HFFFF).ToString("x", CultureInfo.InvariantCulture)
                Else
                    hex = CUInt(BitConverter.ToUInt32(BitConverter.GetBytes(System.Convert.ToInt32(arg, CultureInfo.InvariantCulture)), 0)).ToString("x", CultureInfo.InvariantCulture)
                End If
                If flags.IndexOf("#"c) >= 0 Then hex = "0x" & hex
                If flags.IndexOf("0"c) >= 0 AndAlso width > hex.Length Then hex = hex.PadLeft(width, "0"c)
                Return If(conv = "X"c, hex.ToUpperInvariant(), hex)
            Case "f"c
                If arg Is Nothing Then Return "null"
                Dim d As Double = System.Convert.ToDouble(arg, CultureInfo.InvariantCulture)
                If Double.IsNaN(d) Then Return "NaN"
                If Double.IsInfinity(d) Then Return If(d > 0, If(flags.IndexOf("+"c) >= 0, "+Infinity", "Infinity"), "-Infinity")
                Dim p As Integer = If(precision < 0, 6, precision)
                Dim pattern As String = If(flags.IndexOf(","c) >= 0, "#,##0", "0") & If(p > 0, "." & New String("0"c, p), "")
                Dim body As String = Math.Abs(d).ToString(pattern, CultureInfo.InvariantCulture)
                Return ApplySign(body, BitConverter.DoubleToInt64Bits(d) < 0, flags, width)
            Case "e"c, "E"c
                If arg Is Nothing Then Return "null"
                Dim d As Double = System.Convert.ToDouble(arg, CultureInfo.InvariantCulture)
                If Double.IsNaN(d) Then Return "NaN"
                If Double.IsInfinity(d) Then Return If(d > 0, "Infinity", "-Infinity")
                Dim p As Integer = If(precision < 0, 6, precision)
                Dim body As String = Math.Abs(d).ToString("0" & If(p > 0, "." & New String("0"c, p), "") & "e+00", CultureInfo.InvariantCulture)
                If conv = "E"c Then body = body.ToUpperInvariant()
                Return ApplySign(body, d < 0, flags, width)
            Case "g"c, "G"c
                If arg Is Nothing Then Return "null"
                Dim d As Double = System.Convert.ToDouble(arg, CultureInfo.InvariantCulture)
                If Double.IsNaN(d) Then Return "NaN"
                If Double.IsInfinity(d) Then Return If(d > 0, "Infinity", "-Infinity")
                Dim p As Integer = If(precision < 0, 6, If(precision = 0, 1, precision))
                Dim absd As Double = Math.Abs(d)
                Dim body As String
                If absd <> 0 AndAlso (absd < 0.0001 OrElse absd >= Math.Pow(10, p)) Then
                    body = absd.ToString("0." & New String("0"c, Math.Max(0, p - 1)) & "e+00", CultureInfo.InvariantCulture)
                Else
                    Dim digitsBefore As Integer = If(absd = 0, 1, Math.Max(1, CInt(Math.Floor(Math.Log10(absd))) + 1))
                    Dim decimals As Integer = Math.Max(0, p - digitsBefore)
                    body = absd.ToString("0" & If(decimals > 0, "." & New String("0"c, decimals), ""), CultureInfo.InvariantCulture)
                End If
                If conv = "G"c Then body = body.ToUpperInvariant()
                Return ApplySign(body, d < 0, flags, width)
            Case "c"c, "C"c
                If arg Is Nothing Then Return "null"
                Dim s As String = If(TypeOf arg Is Char, CChar(arg).ToString(), ChrW(System.Convert.ToInt32(arg, CultureInfo.InvariantCulture)).ToString())
                Return If(conv = "C"c, s.ToUpperInvariant(), s)
            Case "b"c, "B"c
                Dim s As String
                If arg Is Nothing Then
                    s = "false"
                ElseIf TypeOf arg Is Boolean Then
                    s = If(CBool(arg), "true", "false")
                Else
                    s = "true"
                End If
                Return If(conv = "B"c, s.ToUpperInvariant(), s)
            Case "h"c, "H"c
                Dim s As String = If(arg Is Nothing, "null", arg.GetHashCode().ToString("x", CultureInfo.InvariantCulture))
                Return If(conv = "H"c, s.ToUpperInvariant(), s)
            Case Else
                Throw New FormatException("Unknown format conversion: '" & conv & "'")
        End Select
    End Function

    Private Function FormatInteger(ByVal value As Long, ByVal flags As String, ByVal width As Integer) As String
        Dim negative As Boolean = value < 0
        Dim magnitude As String
        If value = Long.MinValue Then
            magnitude = "9223372036854775808"
        Else
            magnitude = Math.Abs(value).ToString(CultureInfo.InvariantCulture)
        End If
        If flags.IndexOf(","c) >= 0 Then
            magnitude = GroupDigits(magnitude)
        End If
        Return ApplySign(magnitude, negative, flags, width)
    End Function

    Private Function GroupDigits(ByVal digits As String) As String
        Dim sb As New StringBuilder()
        Dim count As Integer = 0
        For i As Integer = digits.Length - 1 To 0 Step -1
            sb.Insert(0, digits(i))
            count += 1
            If count Mod 3 = 0 AndAlso i > 0 Then sb.Insert(0, ","c)
        Next
        Return sb.ToString()
    End Function

    Private Function ApplySign(ByVal magnitude As String, ByVal negative As Boolean, ByVal flags As String, ByVal width As Integer) As String
        Dim prefix As String = ""
        Dim suffix As String = ""
        If negative Then
            If flags.IndexOf("("c) >= 0 Then
                prefix = "("
                suffix = ")"
            Else
                prefix = "-"
            End If
        ElseIf flags.IndexOf("+"c) >= 0 Then
            prefix = "+"
        ElseIf flags.IndexOf(" "c) >= 0 Then
            prefix = " "
        End If
        If flags.IndexOf("0"c) >= 0 AndAlso width > 0 Then
            Dim padWidth As Integer = width - prefix.Length - suffix.Length
            If magnitude.Length < padWidth Then magnitude = magnitude.PadLeft(padWidth, "0"c)
        End If
        Return prefix & magnitude & suffix
    End Function

    ''' <summary>Java <c>String.valueOf</c>: "null" for Nothing, Java-style booleans and floating point.</summary>
    Public Function valueOf(ByVal o As Object) As String
        Return Arrays.FormatElement(o)
    End Function

    ''' <summary>Converts a Java <c>SimpleDateFormat</c>/<c>DateTimeFormatter</c> pattern to a .NET custom format string.</summary>
    Public Function ConvertJavaDatePattern(ByVal javaPattern As String) As String
        If javaPattern Is Nothing Then Return Nothing
        Dim sb As New StringBuilder(javaPattern.Length + 4)
        Dim inQuote As Boolean = False
        For Each c As Char In javaPattern
            If c = "'"c Then
                inQuote = Not inQuote
                sb.Append(c)
            ElseIf inQuote Then
                sb.Append(c)
            Else
                Select Case c
                    Case "S"c : sb.Append("f"c)
                    Case "a"c : sb.Append("tt")
                    Case "E"c : sb.Append("ddd")
                    Case "u"c : sb.Append("ddd")
                    Case ":"c, "."c, "-"c, "/"c, " "c, ","c, "_"c : sb.Append("\"c).Append(c)
                    Case Else : sb.Append(c)
                End Select
            End If
        Next
        Return sb.ToString()
    End Function

End Module
