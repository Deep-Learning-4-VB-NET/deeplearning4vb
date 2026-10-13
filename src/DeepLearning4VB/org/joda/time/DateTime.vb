Imports System
Imports System.Globalization

' DeepLearning4VB shim for the part of the Joda-Time API that DataVec uses (org.joda.time.DateTime).
' Joda-Time is Apache-2.0 licensed (https://www.joda.org/joda-time/); this file is an independent
' re-implementation over System.DateTime/TimeZoneInfo, not a translation of the Joda source.
' Inside this namespace the .NET System.DateTime is referred to as Date.

Namespace org.joda.time

    ''' <summary>
    ''' An immutable instant (milliseconds since 1970-01-01T00:00:00Z) paired with a
    ''' <see cref="DateTimeZone"/> in which the calendar fields are computed (ISO calendar only).
    ''' </summary>
    <Serializable>
    Public NotInheritable Class DateTime
        Implements IEquatable(Of DateTime), IComparable(Of DateTime)

        Private ReadOnly millis_ As Long
        Private ReadOnly zone_ As DateTimeZone

        ''' <summary>The current time in the default zone.</summary>
        Public Sub New()
            Me.New(currentTimeMillis(), DateTimeZone.getDefault())
        End Sub

        Public Sub New(ByVal instant As Long)
            Me.New(instant, DateTimeZone.getDefault())
        End Sub

        Public Sub New(ByVal instant As Long, ByVal zone As DateTimeZone)
            Me.millis_ = instant
            Me.zone_ = If(zone, DateTimeZone.getDefault())
        End Sub

        ''' <summary>From a .NET date; Local and Utc kinds are converted to UTC, Unspecified is taken as UTC.</summary>
        Public Sub New(ByVal instant As Date)
            Me.New(ToMillis(instant), DateTimeZone.getDefault())
        End Sub

        Public Sub New(ByVal instant As Date, ByVal zone As DateTimeZone)
            Me.New(ToMillis(instant), zone)
        End Sub

        Public Sub New(ByVal year As Integer, ByVal monthOfYear As Integer, ByVal dayOfMonth As Integer, ByVal hourOfDay As Integer, ByVal minuteOfHour As Integer)
            Me.New(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, 0, 0, DateTimeZone.getDefault())
        End Sub

        Public Sub New(ByVal year As Integer, ByVal monthOfYear As Integer, ByVal dayOfMonth As Integer, ByVal hourOfDay As Integer, ByVal minuteOfHour As Integer, ByVal zone As DateTimeZone)
            Me.New(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, 0, 0, zone)
        End Sub

        Public Sub New(ByVal year As Integer, ByVal monthOfYear As Integer, ByVal dayOfMonth As Integer, ByVal hourOfDay As Integer, ByVal minuteOfHour As Integer, ByVal secondOfMinute As Integer)
            Me.New(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute, 0, DateTimeZone.getDefault())
        End Sub

        Public Sub New(ByVal year As Integer, ByVal monthOfYear As Integer, ByVal dayOfMonth As Integer, ByVal hourOfDay As Integer, ByVal minuteOfHour As Integer, ByVal secondOfMinute As Integer, ByVal zone As DateTimeZone)
            Me.New(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute, 0, zone)
        End Sub

        Public Sub New(ByVal year As Integer, ByVal monthOfYear As Integer, ByVal dayOfMonth As Integer, ByVal hourOfDay As Integer, ByVal minuteOfHour As Integer, ByVal secondOfMinute As Integer, ByVal millisOfSecond As Integer)
            Me.New(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute, millisOfSecond, DateTimeZone.getDefault())
        End Sub

        ''' <summary>From wall-clock fields in the given zone.</summary>
        Public Sub New(ByVal year As Integer, ByVal monthOfYear As Integer, ByVal dayOfMonth As Integer, ByVal hourOfDay As Integer, ByVal minuteOfHour As Integer, ByVal secondOfMinute As Integer, ByVal millisOfSecond As Integer, ByVal zone As DateTimeZone)
            Dim z As DateTimeZone = If(zone, DateTimeZone.getDefault())
            Dim local As New Date(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute, millisOfSecond, DateTimeKind.Unspecified)
            Me.millis_ = z.FromLocal(local)
            Me.zone_ = z
        End Sub

        Public Shared Function now() As DateTime
            Return New DateTime()
        End Function

        Public Shared Function now(ByVal zone As DateTimeZone) As DateTime
            Return New DateTime(currentTimeMillis(), zone)
        End Function

        ''' <summary>Parses an ISO-8601 date/time (for example <c>2016-01-01T12:30:45.000Z</c> or <c>2016-01-01T12:30:45+05:30</c>).</summary>
        Public Shared Function parse(ByVal text As String) As DateTime
            If text Is Nothing Then Throw New ArgumentNullException("text")
            Dim dto As DateTimeOffset
            If Not DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.RoundtripKind, dto) Then
                Throw New ArgumentException("Invalid format: """ & text & """")
            End If
            Return New DateTime(dto.ToUnixTimeMilliseconds(), DateTimeZone.forOffsetMillis(CInt(dto.Offset.TotalMilliseconds)))
        End Function

        Friend Shared Function currentTimeMillis() As Long
            Return (Date.UtcNow.Ticks - DateTimeZone.Epoch.Ticks) \ TimeSpan.TicksPerMillisecond
        End Function

        Private Shared Function ToMillis(ByVal d As Date) As Long
            Dim utc As Date = If(d.Kind = DateTimeKind.Local, d.ToUniversalTime(), d)
            Return (utc.Ticks - DateTimeZone.Epoch.Ticks) \ TimeSpan.TicksPerMillisecond
        End Function

        ''' <summary>Milliseconds since 1970-01-01T00:00:00Z.</summary>
        Public Function getMillis() As Long
            Return millis_
        End Function

        Public Function getZone() As DateTimeZone
            Return zone_
        End Function

        ''' <summary>Wall-clock time in this instant's zone (Kind is Unspecified).</summary>
        Friend Function ToLocal() As Date
            Return zone_.ToLocal(millis_)
        End Function

        ''' <summary>Reads a calendar field, for example <c>dt.[get](DateTimeFieldType.hourOfDay())</c>.</summary>
        Public Function [get](ByVal type As DateTimeFieldType) As Integer
            If type Is Nothing Then Throw New ArgumentException("The DateTimeFieldType must not be null")
            Return type.GetValue(ToLocal())
        End Function

        Public Function getEra() As Integer
            Return 1
        End Function

        Public Function getCenturyOfEra() As Integer
            Return ToLocal().Year \ 100
        End Function

        Public Function getYearOfEra() As Integer
            Return ToLocal().Year
        End Function

        Public Function getYearOfCentury() As Integer
            Return ToLocal().Year Mod 100
        End Function

        Public Function getYear() As Integer
            Return ToLocal().Year
        End Function

        Public Function getWeekyear() As Integer
            Return DateTimeFieldType.IsoWeekyear(ToLocal())
        End Function

        Public Function getWeekOfWeekyear() As Integer
            Return DateTimeFieldType.IsoWeek(ToLocal())
        End Function

        Public Function getMonthOfYear() As Integer
            Return ToLocal().Month
        End Function

        Public Function getDayOfYear() As Integer
            Return ToLocal().DayOfYear
        End Function

        Public Function getDayOfMonth() As Integer
            Return ToLocal().Day
        End Function

        ''' <summary>ISO day of week: Monday = 1 ... Sunday = 7.</summary>
        Public Function getDayOfWeek() As Integer
            Return DateTimeFieldType.IsoDayOfWeek(ToLocal())
        End Function

        Public Function getHourOfDay() As Integer
            Return ToLocal().Hour
        End Function

        Public Function getMinuteOfDay() As Integer
            Dim l As Date = ToLocal()
            Return l.Hour * 60 + l.Minute
        End Function

        Public Function getMinuteOfHour() As Integer
            Return ToLocal().Minute
        End Function

        Public Function getSecondOfDay() As Integer
            Dim l As Date = ToLocal()
            Return (l.Hour * 60 + l.Minute) * 60 + l.Second
        End Function

        Public Function getSecondOfMinute() As Integer
            Return ToLocal().Second
        End Function

        Public Function getMillisOfDay() As Integer
            Return CInt(ToLocal().TimeOfDay.Ticks \ TimeSpan.TicksPerMillisecond)
        End Function

        Public Function getMillisOfSecond() As Integer
            Return ToLocal().Millisecond
        End Function

        ''' <summary>Same instant, fields computed in another zone.</summary>
        Public Function withZone(ByVal zone As DateTimeZone) As DateTime
            Return New DateTime(millis_, zone)
        End Function

        ''' <summary>Same wall-clock fields, re-interpreted in another zone.</summary>
        Public Function withZoneRetainFields(ByVal zone As DateTimeZone) As DateTime
            Dim z As DateTimeZone = If(zone, DateTimeZone.getDefault())
            Return New DateTime(z.FromLocal(ToLocal()), z)
        End Function

        Public Function withMillis(ByVal newMillis As Long) As DateTime
            Return If(newMillis = millis_, Me, New DateTime(newMillis, zone_))
        End Function

        Public Function plus(ByVal durationMillis As Long) As DateTime
            Return withMillis(millis_ + durationMillis)
        End Function

        Public Function minus(ByVal durationMillis As Long) As DateTime
            Return withMillis(millis_ - durationMillis)
        End Function

        Public Function plusMillis(ByVal millis As Integer) As DateTime
            Return withMillis(millis_ + millis)
        End Function

        Public Function plusSeconds(ByVal seconds As Integer) As DateTime
            Return withMillis(millis_ + seconds * 1000L)
        End Function

        Public Function plusMinutes(ByVal minutes As Integer) As DateTime
            Return withMillis(millis_ + minutes * 60000L)
        End Function

        Public Function plusHours(ByVal hours As Integer) As DateTime
            Return withMillis(millis_ + hours * 3600000L)
        End Function

        ''' <summary>Adds calendar days in the local zone (the wall-clock time is kept across DST changes).</summary>
        Public Function plusDays(ByVal days As Integer) As DateTime
            Return WithLocal(ToLocal().AddDays(days))
        End Function

        Public Function plusWeeks(ByVal weeks As Integer) As DateTime
            Return WithLocal(ToLocal().AddDays(7L * weeks))
        End Function

        Public Function plusMonths(ByVal months As Integer) As DateTime
            Return WithLocal(ToLocal().AddMonths(months))
        End Function

        Public Function plusYears(ByVal years As Integer) As DateTime
            Return WithLocal(ToLocal().AddYears(years))
        End Function

        Public Function minusMillis(ByVal millis As Integer) As DateTime
            Return plusMillis(-millis)
        End Function

        Public Function minusSeconds(ByVal seconds As Integer) As DateTime
            Return plusSeconds(-seconds)
        End Function

        Public Function minusMinutes(ByVal minutes As Integer) As DateTime
            Return plusMinutes(-minutes)
        End Function

        Public Function minusHours(ByVal hours As Integer) As DateTime
            Return plusHours(-hours)
        End Function

        Public Function minusDays(ByVal days As Integer) As DateTime
            Return plusDays(-days)
        End Function

        Public Function minusWeeks(ByVal weeks As Integer) As DateTime
            Return plusWeeks(-weeks)
        End Function

        Public Function minusMonths(ByVal months As Integer) As DateTime
            Return plusMonths(-months)
        End Function

        Public Function minusYears(ByVal years As Integer) As DateTime
            Return plusYears(-years)
        End Function

        Private Function WithLocal(ByVal local As Date) As DateTime
            Return New DateTime(zone_.FromLocal(local), zone_)
        End Function

        Public Function isBefore(ByVal instant As Long) As Boolean
            Return millis_ < instant
        End Function

        Public Function isBefore(ByVal other As DateTime) As Boolean
            Return millis_ < If(other Is Nothing, currentTimeMillis(), other.millis_)
        End Function

        Public Function isAfter(ByVal instant As Long) As Boolean
            Return millis_ > instant
        End Function

        Public Function isAfter(ByVal other As DateTime) As Boolean
            Return millis_ > If(other Is Nothing, currentTimeMillis(), other.millis_)
        End Function

        Public Function isEqual(ByVal other As DateTime) As Boolean
            Return millis_ = If(other Is Nothing, currentTimeMillis(), other.millis_)
        End Function

        ''' <summary>The instant as a UTC <see cref="Date"/> (<c>toDate()</c> in Joda).</summary>
        Public Function toDate() As Date
            Return DateTimeZone.Epoch.AddMilliseconds(millis_)
        End Function

        ''' <summary>The instant with the offset of its zone.</summary>
        Public Function toDateTimeOffset() As DateTimeOffset
            Return New DateTimeOffset(ToLocal(), TimeSpan.FromMilliseconds(zone_.getOffset(millis_)))
        End Function

        Public Function CompareTo(ByVal other As DateTime) As Integer Implements IComparable(Of DateTime).CompareTo
            If other Is Nothing Then Return 1
            Return millis_.CompareTo(other.millis_)
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return Equals(TryCast(obj, DateTime))
        End Function

        Public Overloads Function Equals(ByVal other As DateTime) As Boolean Implements IEquatable(Of DateTime).Equals
            Return other IsNot Nothing AndAlso other.millis_ = millis_ AndAlso other.zone_.Equals(zone_)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return millis_.GetHashCode() Xor zone_.GetHashCode()
        End Function

        ''' <summary>ISO-8601 with milliseconds and offset, for example <c>2016-01-01T12:30:45.000Z</c> or <c>2016-01-01T18:00:45.000+05:30</c>.</summary>
        Public Overrides Function ToString() As String
            Dim local As Date = ToLocal()
            Dim offset As Integer = zone_.getOffset(millis_)
            Dim offsetText As String = If(offset = 0, "Z", DateTimeZone.FormatOffset(TimeSpan.FromMilliseconds(offset), True))
            Return local.ToString("yyyy-MM-dd'T'HH:mm:ss.fff", CultureInfo.InvariantCulture) & offsetText
        End Function

    End Class

End Namespace
