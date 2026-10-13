Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Text
Imports System.Text.RegularExpressions
Imports JodaDateTime = DeepLearning4VB.org.joda.time.DateTime
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports DateTimeFieldType = DeepLearning4VB.org.joda.time.DateTimeFieldType

' DeepLearning4VB shim for the part of the Joda-Time API that DataVec uses
' (org.joda.time.format.DateTimeFormat / DateTimeFormatter). Joda-Time is Apache-2.0 licensed
' (https://www.joda.org/joda-time/); this file is an independent re-implementation of the pattern
' syntax, not a translation of the Joda source.

Namespace org.joda.time.format

    ''' <summary>
    ''' Factory for <see cref="DateTimeFormatter"/>s from Joda pattern strings. Pattern letters:
    ''' <c>G</c> era, <c>C</c> century of era, <c>Y</c> year of era, <c>x</c> weekyear, <c>w</c> week of weekyear,
    ''' <c>e</c>/<c>E</c> day of week (number/text), <c>y</c> year, <c>D</c> day of year, <c>M</c> month (number, or
    ''' text for 3+ letters), <c>d</c> day of month, <c>a</c> halfday, <c>K</c> hour of halfday (0-11), <c>h</c> clockhour
    ''' of halfday (1-12), <c>H</c> hour of day (0-23), <c>k</c> clockhour of day (1-24), <c>m</c> minute, <c>s</c>
    ''' second, <c>S</c> fraction of second, <c>z</c> zone name (print only), <c>Z</c> zone offset (<c>ZZ</c> with
    ''' colon, <c>ZZZ</c> zone id). Text in single quotes is literal; <c>''</c> is a quote.
    ''' </summary>
    Public NotInheritable Class DateTimeFormat

        Private Sub New()
        End Sub

        ''' <summary>Creates a formatter for the pattern (no zone, invariant culture until <c>withZone</c>/<c>withLocale</c>).</summary>
        Public Shared Function forPattern(ByVal pattern As String) As DateTimeFormatter
            If pattern Is Nothing OrElse pattern.Length = 0 Then Throw New ArgumentException("Invalid pattern specification")
            Return New DateTimeFormatter(pattern, Tokenize(pattern), Nothing, Nothing, False)
        End Function

        Friend Shared Function Tokenize(ByVal pattern As String) As List(Of PatternToken)
            Dim tokens As New List(Of PatternToken)()
            Dim i As Integer = 0
            Dim n As Integer = pattern.Length
            While i < n
                Dim c As Char = pattern(i)
                If c = "'"c Then
                    If i + 1 < n AndAlso pattern(i + 1) = "'"c Then
                        tokens.Add(PatternToken.Literal("'"))
                        i += 2
                    Else
                        Dim sb As New StringBuilder()
                        i += 1
                        Dim closed As Boolean = False
                        While i < n
                            If pattern(i) = "'"c Then
                                If i + 1 < n AndAlso pattern(i + 1) = "'"c Then
                                    sb.Append("'"c)
                                    i += 2
                                Else
                                    closed = True
                                    i += 1
                                    Exit While
                                End If
                            Else
                                sb.Append(pattern(i))
                                i += 1
                            End If
                        End While
                        If Not closed Then Throw New ArgumentException("Illegal pattern specification: " & pattern)
                        tokens.Add(PatternToken.Literal(sb.ToString()))
                    End If
                ElseIf (c >= "A"c AndAlso c <= "Z"c) OrElse (c >= "a"c AndAlso c <= "z"c) Then
                    Dim count As Integer = 1
                    While i + count < n AndAlso pattern(i + count) = c
                        count += 1
                    End While
                    If "GCYxwedEyDMaKhHkmsSzZ".IndexOf(c) < 0 Then
                        Throw New ArgumentException("Illegal pattern component: " & New String(c, count))
                    End If
                    tokens.Add(PatternToken.Field(c, count))
                    i += count
                Else
                    tokens.Add(PatternToken.Literal(c.ToString()))
                    i += 1
                End If
            End While
            Return tokens
        End Function

    End Class

    ''' <summary>One element of a parsed pattern: a field letter with its repeat count, or literal text.</summary>
    <Serializable>
    Friend NotInheritable Class PatternToken
        Public ReadOnly Kind As Char
        Public ReadOnly Count As Integer
        Public ReadOnly Text As String

        Private Sub New(ByVal kind As Char, ByVal count As Integer, ByVal text As String)
            Me.Kind = kind
            Me.Count = count
            Me.Text = text
        End Sub

        Public Shared Function Literal(ByVal text As String) As PatternToken
            Return New PatternToken(ControlChars.NullChar, 0, text)
        End Function

        Public Shared Function Field(ByVal kind As Char, ByVal count As Integer) As PatternToken
            Return New PatternToken(kind, count, Nothing)
        End Function

        Public ReadOnly Property IsLiteral As Boolean
            Get
                Return Kind = ControlChars.NullChar
            End Get
        End Property

        ''' <summary>True for fields that print as digits (adjacent numeric fields parse with fixed widths).</summary>
        Public ReadOnly Property IsNumeric As Boolean
            Get
                If IsLiteral Then Return False
                If Kind = "M"c Then Return Count <= 2
                Return "CYxwedyDKhHkmsS".IndexOf(Kind) >= 0
            End Get
        End Property
    End Class

    ''' <summary>
    ''' Prints instants and parses text according to a Joda pattern. Immutable; <c>withZone</c> and
    ''' <c>withLocale</c> return new formatters. Without a zone the default (system) zone is used, without a
    ''' locale the invariant culture (English month and day names, <c>AM</c>/<c>PM</c>).
    ''' </summary>
    <Serializable>
    Public NotInheritable Class DateTimeFormatter

        Private ReadOnly pattern_ As String
        Private ReadOnly tokens As List(Of PatternToken)
        Private ReadOnly zone_ As DateTimeZone
        Private ReadOnly locale_ As CultureInfo
        Private ReadOnly offsetParsed As Boolean
        <NonSerialized> Private parser As Regex

        Friend Sub New(ByVal pattern As String, ByVal tokens As List(Of PatternToken), ByVal zone As DateTimeZone, ByVal locale As CultureInfo, ByVal offsetParsed As Boolean)
            Me.pattern_ = pattern
            Me.tokens = tokens
            Me.zone_ = zone
            Me.locale_ = locale
            Me.offsetParsed = offsetParsed
        End Sub

        ''' <summary>A formatter that prints in, and interprets parsed wall-clock times in, the given zone.</summary>
        Public Function withZone(ByVal zone As DateTimeZone) As DateTimeFormatter
            If zone_ IsNot Nothing AndAlso zone_.Equals(zone) Then Return Me
            If zone_ Is Nothing AndAlso zone Is Nothing Then Return Me
            Return New DateTimeFormatter(pattern_, tokens, zone, locale_, offsetParsed)
        End Function

        Public Function withZoneUTC() As DateTimeFormatter
            Return withZone(DateTimeZone.UTC)
        End Function

        ''' <summary>A formatter using the culture's month, day and AM/PM names (<c>Nothing</c> = invariant).</summary>
        Public Function withLocale(ByVal locale As CultureInfo) As DateTimeFormatter
            If Object.Equals(locale, locale_) Then Return Me
            Return New DateTimeFormatter(pattern_, tokens, zone_, locale, offsetParsed)
        End Function

        ''' <summary>A formatter whose parsed results keep the offset found in the text as their zone.</summary>
        Public Function withOffsetParsed() As DateTimeFormatter
            If offsetParsed Then Return Me
            Return New DateTimeFormatter(pattern_, tokens, zone_, locale_, True)
        End Function

        Public Function getZone() As DateTimeZone
            Return zone_
        End Function

        Public Function getLocale() As CultureInfo
            Return locale_
        End Function

        ''' <summary>The pattern this formatter was built from.</summary>
        Public Function getPattern() As String
            Return pattern_
        End Function

        Private Function Culture() As CultureInfo
            Return If(locale_, CultureInfo.InvariantCulture)
        End Function

        ''' <summary>Formats an instant (milliseconds since the epoch) in the formatter's zone.</summary>
        Public Function print(ByVal instant As Long) As String
            Dim zone As DateTimeZone = If(zone_, DateTimeZone.getDefault())
            Return PrintLocal(zone.ToLocal(instant), zone, instant)
        End Function

        ''' <summary>Formats a date/time; the formatter's zone overrides the value's zone when set.</summary>
        Public Function print(ByVal dt As JodaDateTime) As String
            If dt Is Nothing Then Throw New ArgumentNullException("dt")
            Dim zone As DateTimeZone = If(zone_, dt.getZone())
            Return PrintLocal(zone.ToLocal(dt.getMillis()), zone, dt.getMillis())
        End Function

        Private Function PrintLocal(ByVal local As Date, ByVal zone As DateTimeZone, ByVal instant As Long) As String
            Dim dtf As DateTimeFormatInfo = Culture().DateTimeFormat
            Dim sb As New StringBuilder()
            For Each t As PatternToken In tokens
                If t.IsLiteral Then
                    sb.Append(t.Text)
                    Continue For
                End If
                Select Case t.Kind
                    Case "G"c
                        sb.Append(If(local.Year > 0, "AD", "BC"))
                    Case "C"c
                        AppendNumber(sb, local.Year \ 100, t.Count)
                    Case "Y"c, "y"c
                        AppendYear(sb, local.Year, t.Count)
                    Case "x"c
                        AppendYear(sb, DateTimeFieldType.IsoWeekyear(local), t.Count)
                    Case "w"c
                        AppendNumber(sb, DateTimeFieldType.IsoWeek(local), t.Count)
                    Case "e"c
                        AppendNumber(sb, DateTimeFieldType.IsoDayOfWeek(local), t.Count)
                    Case "E"c
                        sb.Append(If(t.Count >= 4, dtf.GetDayName(local.DayOfWeek), dtf.GetAbbreviatedDayName(local.DayOfWeek)))
                    Case "D"c
                        AppendNumber(sb, local.DayOfYear, t.Count)
                    Case "M"c
                        If t.Count >= 4 Then
                            sb.Append(dtf.GetMonthName(local.Month))
                        ElseIf t.Count = 3 Then
                            sb.Append(dtf.GetAbbreviatedMonthName(local.Month))
                        Else
                            AppendNumber(sb, local.Month, t.Count)
                        End If
                    Case "d"c
                        AppendNumber(sb, local.Day, t.Count)
                    Case "a"c
                        sb.Append(If(local.Hour < 12, dtf.AMDesignator, dtf.PMDesignator))
                    Case "K"c
                        AppendNumber(sb, local.Hour Mod 12, t.Count)
                    Case "h"c
                        AppendNumber(sb, If(local.Hour Mod 12 = 0, 12, local.Hour Mod 12), t.Count)
                    Case "H"c
                        AppendNumber(sb, local.Hour, t.Count)
                    Case "k"c
                        AppendNumber(sb, If(local.Hour = 0, 24, local.Hour), t.Count)
                    Case "m"c
                        AppendNumber(sb, local.Minute, t.Count)
                    Case "s"c
                        AppendNumber(sb, local.Second, t.Count)
                    Case "S"c
                        Dim frac As String = local.Millisecond.ToString("000", CultureInfo.InvariantCulture)
                        If t.Count < 3 Then
                            frac = frac.Substring(0, t.Count)
                        Else
                            frac = frac.PadRight(t.Count, "0"c)
                        End If
                        sb.Append(frac)
                    Case "z"c
                        sb.Append(If(t.Count >= 4, zone.getName(instant), zone.getShortName(instant)))
                    Case "Z"c
                        If t.Count >= 3 Then
                            sb.Append(zone.getID())
                        Else
                            sb.Append(DateTimeZone.FormatOffset(TimeSpan.FromMilliseconds(zone.getOffset(instant)), t.Count = 2))
                        End If
                End Select
            Next
            Return sb.ToString()
        End Function

        Private Shared Sub AppendNumber(ByVal sb As StringBuilder, ByVal value As Integer, ByVal minDigits As Integer)
            If value < 0 Then
                sb.Append("-"c)
                value = -value
            End If
            sb.Append(value.ToString(CultureInfo.InvariantCulture).PadLeft(minDigits, "0"c))
        End Sub

        Private Shared Sub AppendYear(ByVal sb As StringBuilder, ByVal year As Integer, ByVal count As Integer)
            If count = 2 Then
                sb.Append((Math.Abs(year) Mod 100).ToString("00", CultureInfo.InvariantCulture))
            Else
                AppendNumber(sb, year, count)
            End If
        End Sub

        ''' <summary>Parses text to an instant (milliseconds since the epoch).</summary>
        Public Function parseMillis(ByVal text As String) As Long
            Return parseDateTime(text).getMillis()
        End Function

        ''' <summary>
        ''' Parses text to a date/time. Missing fields default to 1970-01-01T00:00:00.000; a two-letter year
        ''' is in 1950..2049; the whole text must match (an <see cref="ArgumentException"/> is thrown otherwise).
        ''' </summary>
        Public Function parseDateTime(ByVal text As String) As JodaDateTime
            If text Is Nothing Then Throw New ArgumentNullException("text")
            Dim m As Match = GetParser().Match(text)
            If Not m.Success Then Throw New ArgumentException("Invalid format: """ & text & """")
            Dim dtf As DateTimeFormatInfo = Culture().DateTimeFormat

            Dim year As Integer = 1970
            Dim twoDigitYear As Boolean = False
            Dim bc As Boolean = False
            Dim century As Integer = Integer.MinValue
            Dim month As Integer = 1
            Dim day As Integer = 1
            Dim dayOfYear As Integer = -1
            Dim weekyear As Integer = Integer.MinValue
            Dim twoDigitWeekyear As Boolean = False
            Dim week As Integer = -1
            Dim dayOfWeek As Integer = -1
            Dim hour As Integer = 0
            Dim hourOfHalfday As Integer = -1
            Dim clockhourOfHalfday As Integer = -1
            Dim halfday As Integer = -1
            Dim minute As Integer = 0
            Dim second As Integer = 0
            Dim millis As Integer = 0
            Dim hasOffset As Boolean = False
            Dim parsedOffset As TimeSpan = TimeSpan.Zero
            Dim parsedZone As DateTimeZone = Nothing

            Dim g As Integer = 1
            For Each t As PatternToken In tokens
                If t.IsLiteral Then Continue For
                Dim s As String = m.Groups(g).Value
                g += 1
                Select Case t.Kind
                    Case "G"c
                        bc = s.StartsWith("B", StringComparison.OrdinalIgnoreCase)
                    Case "C"c
                        century = ParseInt(s, text)
                    Case "Y"c, "y"c
                        year = ParseInt(s, text)
                        twoDigitYear = (t.Count = 2)
                    Case "x"c
                        weekyear = ParseInt(s, text)
                        twoDigitWeekyear = (t.Count = 2)
                    Case "w"c
                        week = ParseInt(s, text)
                    Case "e"c
                        dayOfWeek = ParseInt(s, text)
                    Case "E"c
                        dayOfWeek = DayNameToIso(s, dtf, text)
                    Case "D"c
                        dayOfYear = ParseInt(s, text)
                    Case "M"c
                        month = If(t.Count <= 2, ParseInt(s, text), MonthNameToNumber(s, dtf, text))
                    Case "d"c
                        day = ParseInt(s, text)
                    Case "a"c
                        halfday = If(s.Equals(dtf.PMDesignator, StringComparison.OrdinalIgnoreCase) OrElse s.Equals("PM", StringComparison.OrdinalIgnoreCase), 1, 0)
                    Case "K"c
                        hourOfHalfday = ParseInt(s, text)
                    Case "h"c
                        clockhourOfHalfday = ParseInt(s, text)
                    Case "H"c
                        hour = ParseInt(s, text)
                    Case "k"c
                        hour = ParseInt(s, text)
                        If hour = 24 Then hour = 0
                    Case "m"c
                        minute = ParseInt(s, text)
                    Case "s"c
                        second = ParseInt(s, text)
                    Case "S"c
                        millis = Integer.Parse(s.PadRight(3, "0"c).Substring(0, 3), CultureInfo.InvariantCulture)
                    Case "z"c
                        Try
                            parsedZone = DateTimeZone.forID(s)
                        Catch ex As ArgumentException
                            ' zone names are not parseable in Joda either; ignore unknown names
                        End Try
                    Case "Z"c
                        If t.Count >= 3 Then
                            parsedZone = DateTimeZone.forID(s)
                        Else
                            If Not DateTimeZone.TryParseOffset(s, parsedOffset) Then Throw New ArgumentException("Invalid format: """ & text & """")
                            hasOffset = True
                        End If
                End Select
            Next

            If twoDigitYear AndAlso year >= 0 AndAlso year <= 99 Then year = If(year >= 50, 1900 + year, 2000 + year)
            If century <> Integer.MinValue AndAlso Not twoDigitYear Then year = century * 100 + (If(year = 1970, 0, year Mod 100))
            If bc Then year = 1 - year
            If twoDigitWeekyear AndAlso weekyear >= 0 AndAlso weekyear <= 99 Then weekyear = If(weekyear >= 50, 1900 + weekyear, 2000 + weekyear)

            If clockhourOfHalfday >= 0 Then
                CheckRange(clockhourOfHalfday, 1, 12, "clockhourOfHalfday", text)
                hour = (clockhourOfHalfday Mod 12) + If(halfday = 1, 12, 0)
            ElseIf hourOfHalfday >= 0 Then
                CheckRange(hourOfHalfday, 0, 11, "hourOfHalfday", text)
                hour = hourOfHalfday + If(halfday = 1, 12, 0)
            ElseIf halfday = 1 AndAlso hour < 12 Then
                hour += 12
            End If
            CheckRange(year, 1, 9999, "year", text)
            CheckRange(month, 1, 12, "monthOfYear", text)
            CheckRange(hour, 0, 23, "hourOfDay", text)
            CheckRange(minute, 0, 59, "minuteOfHour", text)
            CheckRange(second, 0, 59, "secondOfMinute", text)

            Dim local As Date
            If weekyear <> Integer.MinValue Then
                CheckRange(weekyear, 1, 9999, "weekyear", text)
                If week < 0 Then week = 1
                CheckRange(week, 1, 53, "weekOfWeekyear", text)
                If dayOfWeek < 0 Then dayOfWeek = 1
                CheckRange(dayOfWeek, 1, 7, "dayOfWeek", text)
                local = DateTimeFieldType.FromIsoWeekDate(weekyear, week, dayOfWeek)
            ElseIf dayOfYear >= 0 Then
                ' -1 means the pattern had no D field; a parsed 0 must be rejected, not ignored
                CheckRange(dayOfYear, 1, If(Date.IsLeapYear(year), 366, 365), "dayOfYear", text)
                local = New Date(year, 1, 1).AddDays(dayOfYear - 1)
            Else
                CheckRange(day, 1, Date.DaysInMonth(year, month), "dayOfMonth", text)
                local = New Date(year, month, day)
            End If
            local = local.AddHours(hour).AddMinutes(minute).AddSeconds(second).AddMilliseconds(millis)
            Dim localMillis As Long = (local.Ticks - DateTimeZone.Epoch.Ticks) \ TimeSpan.TicksPerMillisecond

            Dim instant As Long
            Dim resultZone As DateTimeZone
            If hasOffset Then
                instant = localMillis - CLng(parsedOffset.TotalMilliseconds)
                resultZone = If(offsetParsed, DateTimeZone.forOffsetMillis(CInt(parsedOffset.TotalMilliseconds)), If(zone_, DateTimeZone.getDefault()))
            ElseIf parsedZone IsNot Nothing Then
                instant = parsedZone.convertLocalToUTC(localMillis, False)
                resultZone = If(offsetParsed, parsedZone, If(zone_, DateTimeZone.getDefault()))
            Else
                resultZone = If(zone_, DateTimeZone.getDefault())
                instant = resultZone.convertLocalToUTC(localMillis, False)
            End If
            Return New JodaDateTime(instant, resultZone)
        End Function

        Private Shared Function ParseInt(ByVal s As String, ByVal text As String) As Integer
            Dim v As Integer
            If Integer.TryParse(s, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, v) Then Return v
            Throw New ArgumentException("Invalid format: """ & text & """ is malformed at """ & s & """")
        End Function

        Private Shared Sub CheckRange(ByVal value As Integer, ByVal min As Integer, ByVal max As Integer, ByVal fieldName As String, ByVal text As String)
            If value < min OrElse value > max Then
                Throw New ArgumentException("Cannot parse """ & text & """: Value " & value & " for " & fieldName & " must be in the range [" & min & "," & max & "]")
            End If
        End Sub

        Private Shared Function MonthNameToNumber(ByVal s As String, ByVal dtf As DateTimeFormatInfo, ByVal text As String) As Integer
            For i As Integer = 1 To 12
                If s.Equals(dtf.GetMonthName(i), StringComparison.OrdinalIgnoreCase) OrElse s.Equals(dtf.GetAbbreviatedMonthName(i), StringComparison.OrdinalIgnoreCase) Then Return i
            Next
            Dim inv As DateTimeFormatInfo = CultureInfo.InvariantCulture.DateTimeFormat
            For i As Integer = 1 To 12
                If s.Equals(inv.GetMonthName(i), StringComparison.OrdinalIgnoreCase) OrElse s.Equals(inv.GetAbbreviatedMonthName(i), StringComparison.OrdinalIgnoreCase) Then Return i
            Next
            Throw New ArgumentException("Invalid format: """ & text & """ is malformed at """ & s & """")
        End Function

        Private Shared Function DayNameToIso(ByVal s As String, ByVal dtf As DateTimeFormatInfo, ByVal text As String) As Integer
            For Each dtfInfo As DateTimeFormatInfo In New DateTimeFormatInfo() {dtf, CultureInfo.InvariantCulture.DateTimeFormat}
                For Each dow As DayOfWeek In [Enum].GetValues(GetType(DayOfWeek))
                    If s.Equals(dtfInfo.GetDayName(dow), StringComparison.OrdinalIgnoreCase) OrElse s.Equals(dtfInfo.GetAbbreviatedDayName(dow), StringComparison.OrdinalIgnoreCase) Then
                        Return (CInt(dow) + 6) Mod 7 + 1
                    End If
                Next
            Next
            Throw New ArgumentException("Invalid format: """ & text & """ is malformed at """ & s & """")
        End Function

        Private Function GetParser() As Regex
            Dim re As Regex = parser
            If re IsNot Nothing Then Return re
            Dim dtf As DateTimeFormatInfo = Culture().DateTimeFormat
            Dim sb As New StringBuilder("^")
            For i As Integer = 0 To tokens.Count - 1
                Dim t As PatternToken = tokens(i)
                If t.IsLiteral Then
                    sb.Append(Regex.Escape(t.Text))
                    Continue For
                End If
                Dim nextIsNumeric As Boolean = i + 1 < tokens.Count AndAlso tokens(i + 1).IsNumeric
                Dim fixedWidth As String = "(\d{" & t.Count & "})"
                Dim variable As String = "(\d{1,9})"
                Select Case t.Kind
                    Case "G"c
                        sb.Append("(AD|BC|CE|BCE)")
                    Case "Y"c, "y"c, "x"c
                        If t.Count = 2 Then
                            sb.Append("(\d{2})")
                        ElseIf nextIsNumeric Then
                            sb.Append("(-?\d{" & t.Count & "})")
                        Else
                            sb.Append("(-?\d{1,9})")
                        End If
                    Case "C"c, "w"c, "e"c, "D"c, "d"c, "K"c, "h"c, "H"c, "k"c, "m"c, "s"c, "S"c
                        sb.Append(If(nextIsNumeric, fixedWidth, variable))
                    Case "M"c
                        If t.Count <= 2 Then
                            sb.Append(If(nextIsNumeric, fixedWidth, variable))
                        Else
                            Dim names As New List(Of String)()
                            For k As Integer = 1 To 12
                                names.Add(dtf.GetMonthName(k))
                                names.Add(dtf.GetAbbreviatedMonthName(k))
                                names.Add(CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(k))
                                names.Add(CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(k))
                            Next
                            sb.Append("(" & Alternation(names) & ")")
                        End If
                    Case "E"c
                        Dim names As New List(Of String)()
                        For Each dow As DayOfWeek In [Enum].GetValues(GetType(DayOfWeek))
                            names.Add(dtf.GetDayName(dow))
                            names.Add(dtf.GetAbbreviatedDayName(dow))
                            names.Add(CultureInfo.InvariantCulture.DateTimeFormat.GetDayName(dow))
                            names.Add(CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedDayName(dow))
                        Next
                        sb.Append("(" & Alternation(names) & ")")
                    Case "a"c
                        sb.Append("(" & Alternation(New String() {dtf.AMDesignator, dtf.PMDesignator, "AM", "PM"}) & ")")
                    Case "z"c
                        sb.Append("([A-Za-z][A-Za-z0-9_/+\-]*)")
                    Case "Z"c
                        If t.Count >= 3 Then
                            sb.Append("([A-Za-z][A-Za-z0-9_/+\-]*|[+-]\d{2}:?\d{2})")
                        Else
                            sb.Append("(Z|[+-]\d{2}(?::?\d{2})?)")
                        End If
                End Select
            Next
            sb.Append("$")
            re = New Regex(sb.ToString(), RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant)
            parser = re
            Return re
        End Function

        Private Shared Function Alternation(ByVal names As IEnumerable(Of String)) As String
            Dim distinct As New List(Of String)()
            For Each n As String In names
                If Not String.IsNullOrEmpty(n) AndAlso Not distinct.Contains(n) Then distinct.Add(n)
            Next
            distinct.Sort(Function(a, b) b.Length.CompareTo(a.Length))
            Dim parts As New List(Of String)()
            For Each n As String In distinct
                parts.Add(Regex.Escape(n))
            Next
            Return String.Join("|", parts)
        End Function

        Public Overrides Function ToString() As String
            Return "DateTimeFormatter[" & pattern_ & If(zone_ Is Nothing, "", ", " & zone_.getID()) & "]"
        End Function

    End Class

End Namespace
