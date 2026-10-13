Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Text
Imports System.Text.Json.Serialization

' DeepLearning4VB shim for the part of the Joda-Time API that DataVec uses (org.joda.time.DateTimeZone).
' Joda-Time is Apache-2.0 licensed (https://www.joda.org/joda-time/); this file is an independent
' re-implementation over System.TimeZoneInfo, not a translation of the Joda source.

Namespace org.joda.time

    ''' <summary>
    ''' A time zone identified by a Joda/IANA style id, backed by a <see cref="TimeZoneInfo"/>.
    ''' Supported ids: <c>UTC</c> (and its aliases), fixed offsets such as <c>+05:30</c>, Windows time zone
    ''' ids, and the IANA ids in the built-in table (extend it with <see cref="addIanaMapping"/>).
    ''' Instants are milliseconds since 1970-01-01T00:00:00Z. Serializes with System.Text.Json as
    ''' <c>{"ID":"UTC"}</c> (Jackson's Joda module wrote the bare id string).
    ''' </summary>
    <Serializable>
    Public NotInheritable Class DateTimeZone
        Implements IEquatable(Of DateTimeZone)

        Friend Shared ReadOnly Epoch As New Date(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)

        Private Shared ReadOnly ianaToWindows As New Dictionary(Of String, String)(StringComparer.Ordinal) From {
            {"America/New_York", "Eastern Standard Time"}, {"America/Toronto", "Eastern Standard Time"},
            {"America/Detroit", "Eastern Standard Time"}, {"America/Chicago", "Central Standard Time"},
            {"America/Winnipeg", "Central Standard Time"}, {"America/Denver", "Mountain Standard Time"},
            {"America/Edmonton", "Mountain Standard Time"}, {"America/Phoenix", "US Mountain Standard Time"},
            {"America/Los_Angeles", "Pacific Standard Time"}, {"America/Vancouver", "Pacific Standard Time"},
            {"America/Anchorage", "Alaskan Standard Time"}, {"Pacific/Honolulu", "Hawaiian Standard Time"},
            {"America/Halifax", "Atlantic Standard Time"}, {"America/St_Johns", "Newfoundland Standard Time"},
            {"America/Mexico_City", "Central Standard Time (Mexico)"}, {"America/Bogota", "SA Pacific Standard Time"},
            {"America/Lima", "SA Pacific Standard Time"}, {"America/Caracas", "Venezuela Standard Time"},
            {"America/Santiago", "Pacific SA Standard Time"}, {"America/Sao_Paulo", "E. South America Standard Time"},
            {"America/Argentina/Buenos_Aires", "Argentina Standard Time"}, {"America/Buenos_Aires", "Argentina Standard Time"},
            {"Europe/London", "GMT Standard Time"}, {"Europe/Dublin", "GMT Standard Time"},
            {"Europe/Lisbon", "GMT Standard Time"}, {"Europe/Paris", "Romance Standard Time"},
            {"Europe/Madrid", "Romance Standard Time"}, {"Europe/Brussels", "Romance Standard Time"},
            {"Europe/Copenhagen", "Romance Standard Time"}, {"Europe/Amsterdam", "W. Europe Standard Time"},
            {"Europe/Berlin", "W. Europe Standard Time"}, {"Europe/Rome", "W. Europe Standard Time"},
            {"Europe/Vienna", "W. Europe Standard Time"}, {"Europe/Zurich", "W. Europe Standard Time"},
            {"Europe/Stockholm", "W. Europe Standard Time"}, {"Europe/Oslo", "W. Europe Standard Time"},
            {"Europe/Warsaw", "Central European Standard Time"}, {"Europe/Prague", "Central Europe Standard Time"},
            {"Europe/Budapest", "Central Europe Standard Time"}, {"Europe/Belgrade", "Central Europe Standard Time"},
            {"Europe/Athens", "GTB Standard Time"}, {"Europe/Bucharest", "GTB Standard Time"},
            {"Europe/Helsinki", "FLE Standard Time"}, {"Europe/Kiev", "FLE Standard Time"},
            {"Europe/Kyiv", "FLE Standard Time"}, {"Europe/Istanbul", "Turkey Standard Time"},
            {"Europe/Moscow", "Russian Standard Time"}, {"Africa/Cairo", "Egypt Standard Time"},
            {"Africa/Johannesburg", "South Africa Standard Time"}, {"Africa/Lagos", "W. Central Africa Standard Time"},
            {"Africa/Nairobi", "E. Africa Standard Time"}, {"Africa/Casablanca", "Morocco Standard Time"},
            {"Asia/Dubai", "Arabian Standard Time"}, {"Asia/Riyadh", "Arab Standard Time"},
            {"Asia/Jerusalem", "Israel Standard Time"}, {"Asia/Tehran", "Iran Standard Time"},
            {"Asia/Karachi", "Pakistan Standard Time"}, {"Asia/Kolkata", "India Standard Time"},
            {"Asia/Calcutta", "India Standard Time"}, {"Asia/Dhaka", "Bangladesh Standard Time"},
            {"Asia/Bangkok", "SE Asia Standard Time"}, {"Asia/Jakarta", "SE Asia Standard Time"},
            {"Asia/Ho_Chi_Minh", "SE Asia Standard Time"}, {"Asia/Shanghai", "China Standard Time"},
            {"Asia/Hong_Kong", "China Standard Time"}, {"Asia/Singapore", "Singapore Standard Time"},
            {"Asia/Kuala_Lumpur", "Singapore Standard Time"}, {"Asia/Manila", "Singapore Standard Time"},
            {"Asia/Taipei", "Taipei Standard Time"}, {"Asia/Seoul", "Korea Standard Time"},
            {"Asia/Tokyo", "Tokyo Standard Time"}, {"Australia/Perth", "W. Australia Standard Time"},
            {"Australia/Adelaide", "Cen. Australia Standard Time"}, {"Australia/Darwin", "AUS Central Standard Time"},
            {"Australia/Brisbane", "E. Australia Standard Time"}, {"Australia/Sydney", "AUS Eastern Standard Time"},
            {"Australia/Melbourne", "AUS Eastern Standard Time"}, {"Australia/Hobart", "Tasmania Standard Time"},
            {"Pacific/Auckland", "New Zealand Standard Time"}, {"Pacific/Fiji", "Fiji Standard Time"}
        }

        ''' <summary>The UTC zone.</summary>
        Public Shared ReadOnly UTC As New DateTimeZone("UTC", TimeZoneInfo.Utc)

        Private ReadOnly id_ As String
        Private ReadOnly tz As TimeZoneInfo

        ''' <summary>Creates the zone for an id; same as <see cref="forID"/> (used by System.Text.Json).</summary>
        <JsonConstructor>
        Public Sub New(ByVal id As String)
            Me.New(id, Resolve(id))
        End Sub

        Private Sub New(ByVal id As String, ByVal tz As TimeZoneInfo)
            If id Is Nothing Then Throw New ArgumentNullException("id")
            Me.id_ = id
            Me.tz = tz
        End Sub

        ''' <summary>The zone id, for example <c>UTC</c>, <c>Europe/London</c> or <c>+05:30</c>.</summary>
        Public ReadOnly Property ID As String
            Get
                Return id_
            End Get
        End Property

        Public Function getID() As String
            Return id_
        End Function

        ''' <summary>Gets a zone by id; <c>Nothing</c> gives the default (system) zone.</summary>
        Public Shared Function forID(ByVal id As String) As DateTimeZone
            If id Is Nothing Then Return getDefault()
            If id = "UTC" Then Return UTC
            Return New DateTimeZone(id)
        End Function

        ''' <summary>Wraps a .NET time zone (the <c>java.util.TimeZone</c> overload).</summary>
        Public Shared Function forTimeZone(ByVal zone As TimeZoneInfo) As DateTimeZone
            If zone Is Nothing Then Return getDefault()
            If zone.Id = TimeZoneInfo.Utc.Id OrElse zone.Equals(TimeZoneInfo.Utc) Then Return UTC
            Return New DateTimeZone(zone.Id, zone)
        End Function

        Public Shared Function forOffsetHours(ByVal hoursOffset As Integer) As DateTimeZone
            Return forOffsetHoursMinutes(hoursOffset, 0)
        End Function

        Public Shared Function forOffsetHoursMinutes(ByVal hoursOffset As Integer, ByVal minutesOffset As Integer) As DateTimeZone
            If hoursOffset = 0 AndAlso minutesOffset = 0 Then Return UTC
            If hoursOffset < -23 OrElse hoursOffset > 23 Then Throw New ArgumentException("Hours out of range: " & hoursOffset)
            If minutesOffset < -59 OrElse minutesOffset > 59 Then Throw New ArgumentException("Minutes out of range: " & minutesOffset)
            If hoursOffset > 0 AndAlso minutesOffset < 0 Then Throw New ArgumentException("Positive hours must not have negative minutes: " & minutesOffset)
            Dim totalMinutes As Integer = hoursOffset * 60
            If hoursOffset < 0 Then
                totalMinutes -= Math.Abs(minutesOffset)
            Else
                totalMinutes += minutesOffset
            End If
            Return forOffsetMillis(totalMinutes * 60000)
        End Function

        ''' <summary>A fixed-offset zone; the id is the offset formatted as <c>+HH:mm</c>.</summary>
        Public Shared Function forOffsetMillis(ByVal millisOffset As Integer) As DateTimeZone
            If millisOffset = 0 Then Return UTC
            Dim offset As TimeSpan = TimeSpan.FromMilliseconds(millisOffset)
            Dim id As String = FormatOffset(offset, True)
            Return New DateTimeZone(id, TimeZoneInfo.CreateCustomTimeZone(id, offset, id, id))
        End Function

        ''' <summary>The system (local) time zone.</summary>
        Public Shared Function getDefault() As DateTimeZone
            Return forTimeZone(TimeZoneInfo.Local)
        End Function

        ''' <summary>UTC, the known IANA ids and the Windows system zone ids.</summary>
        Public Shared Function getAvailableIDs() As ICollection(Of String)
            Dim ids As New SortedSet(Of String)(StringComparer.Ordinal)
            ids.Add("UTC")
            SyncLock ianaToWindows
                For Each k As String In ianaToWindows.Keys
                    ids.Add(k)
                Next
            End SyncLock
            For Each z As TimeZoneInfo In TimeZoneInfo.GetSystemTimeZones()
                ids.Add(z.Id)
            Next
            Return ids
        End Function

        ''' <summary>Registers (or replaces) the Windows zone id that an IANA id resolves to.</summary>
        Public Shared Sub addIanaMapping(ByVal ianaId As String, ByVal windowsId As String)
            If ianaId Is Nothing OrElse windowsId Is Nothing Then Throw New ArgumentNullException()
            SyncLock ianaToWindows
                ianaToWindows(ianaId) = windowsId
            End SyncLock
        End Sub

        Private Shared Function Resolve(ByVal id As String) As TimeZoneInfo
            If id Is Nothing Then Throw New ArgumentNullException("id")
            Select Case id
                Case "UTC", "Z", "GMT", "UCT", "Zulu", "Universal", "Etc/UTC", "Etc/GMT", "Etc/UCT", "Etc/Zulu", "Etc/Universal", "GMT0", "Etc/GMT0", "Etc/GMT+0", "Etc/GMT-0"
                    Return TimeZoneInfo.Utc
            End Select
            Dim offset As TimeSpan
            If TryParseOffset(id, offset) Then
                If offset = TimeSpan.Zero Then Return TimeZoneInfo.Utc
                Return TimeZoneInfo.CreateCustomTimeZone(id, offset, id, id)
            End If
            Dim windowsId As String = Nothing
            SyncLock ianaToWindows
                ianaToWindows.TryGetValue(id, windowsId)
            End SyncLock
            Try
                Return TimeZoneInfo.FindSystemTimeZoneById(If(windowsId, id))
            Catch ex As TimeZoneNotFoundException
                Throw New ArgumentException("The datetime zone id '" & id & "' is not recognised", ex)
            Catch ex As InvalidTimeZoneException
                Throw New ArgumentException("The datetime zone id '" & id & "' is not recognised", ex)
            End Try
        End Function

        ''' <summary>The underlying .NET time zone.</summary>
        Public Function toTimeZone() As TimeZoneInfo
            Return tz
        End Function

        Public Function isFixed() As Boolean
            Return Not tz.SupportsDaylightSavingTime
        End Function

        Private Function IsUtc() As Boolean
            Return tz Is TimeZoneInfo.Utc OrElse tz.Equals(TimeZoneInfo.Utc)
        End Function

        ''' <summary>Offset from UTC in milliseconds at the given instant (positive east of Greenwich).</summary>
        Public Function getOffset(ByVal instant As Long) As Integer
            If IsUtc() Then Return 0
            Return CInt(tz.GetUtcOffset(DateTimeOffset.FromUnixTimeMilliseconds(instant)).TotalMilliseconds)
        End Function

        Public Function getStandardOffset(ByVal instant As Long) As Integer
            Return CInt(tz.BaseUtcOffset.TotalMilliseconds)
        End Function

        Public Function isStandardOffset(ByVal instant As Long) As Boolean
            Return getOffset(instant) = getStandardOffset(instant)
        End Function

        ''' <summary>Long display name in force at the instant (standard or daylight name).</summary>
        Public Function getName(ByVal instant As Long) As String
            If IsUtc() Then Return "Coordinated Universal Time"
            If tz.IsDaylightSavingTime(DateTimeOffset.FromUnixTimeMilliseconds(instant)) Then Return tz.DaylightName
            Return tz.StandardName
        End Function

        ''' <summary>Short name in force at the instant: <c>UTC</c>, or the initials of the Windows display name (for example <c>PST</c> for "Pacific Standard Time").</summary>
        Public Function getShortName(ByVal instant As Long) As String
            If IsUtc() Then Return "UTC"
            Return Abbreviate(getName(instant))
        End Function

        Private Shared Function Abbreviate(ByVal name As String) As String
            If String.IsNullOrEmpty(name) OrElse name.IndexOf(" "c) < 0 Then Return name
            Dim sb As New StringBuilder()
            For Each word As String In name.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
                If Char.IsLetter(word(0)) Then sb.Append(Char.ToUpperInvariant(word(0)))
            Next
            Return sb.ToString()
        End Function

        ''' <summary>Converts a UTC instant to the local wall-clock instant (same millis arithmetic as Joda).</summary>
        Public Function convertUTCToLocal(ByVal instantUTC As Long) As Long
            Return instantUTC + getOffset(instantUTC)
        End Function

        ''' <summary>
        ''' Converts a local wall-clock instant to UTC. In a daylight-saving gap the non-strict conversion
        ''' uses the offset in force before the gap (strict throws); in an overlap the earlier instant is used.
        ''' </summary>
        Public Function convertLocalToUTC(ByVal instantLocal As Long, ByVal strict As Boolean) As Long
            If IsUtc() Then Return instantLocal
            Dim local As New Date(Epoch.Ticks + instantLocal * TimeSpan.TicksPerMillisecond, DateTimeKind.Unspecified)
            If tz.IsInvalidTime(local) Then
                If strict Then
                    Throw New ArgumentException("Illegal instant due to time zone offset transition (daylight savings time 'gap'): " &
                                                local.ToString("yyyy-MM-dd'T'HH:mm:ss.fff", CultureInfo.InvariantCulture) & " (" & id_ & ")")
                End If
                Dim before As TimeSpan = tz.GetUtcOffset(local.AddDays(-1))
                Return instantLocal - CLng(before.TotalMilliseconds)
            End If
            Dim offset As TimeSpan
            If tz.IsAmbiguousTime(local) Then
                offset = TimeSpan.MinValue
                For Each o As TimeSpan In tz.GetAmbiguousTimeOffsets(local)
                    If o > offset Then offset = o
                Next
            Else
                offset = tz.GetUtcOffset(local)
            End If
            Return instantLocal - CLng(offset.TotalMilliseconds)
        End Function

        ''' <summary>Wall-clock time in this zone for the instant (Kind is Unspecified).</summary>
        Friend Function ToLocal(ByVal instant As Long) As Date
            Dim utc As Date = Epoch.AddMilliseconds(instant)
            If IsUtc() Then Return Date.SpecifyKind(utc, DateTimeKind.Unspecified)
            Return Date.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(utc, tz), DateTimeKind.Unspecified)
        End Function

        ''' <summary>Instant for a wall-clock time in this zone (non-strict).</summary>
        Friend Function FromLocal(ByVal local As Date) As Long
            Dim localMillis As Long = (local.Ticks - Epoch.Ticks) \ TimeSpan.TicksPerMillisecond
            Return convertLocalToUTC(localMillis, False)
        End Function

        ''' <summary>Formats an offset as <c>+HHmm</c> or <c>+HH:mm</c> (seconds appended only when non-zero).</summary>
        Friend Shared Function FormatOffset(ByVal offset As TimeSpan, ByVal colon As Boolean) As String
            Dim sign As String = If(offset < TimeSpan.Zero, "-", "+")
            Dim a As TimeSpan = offset.Duration()
            Dim sep As String = If(colon, ":", "")
            Dim s As String = sign & a.Hours.ToString("00", CultureInfo.InvariantCulture) & sep & a.Minutes.ToString("00", CultureInfo.InvariantCulture)
            If a.Seconds <> 0 Then s &= sep & a.Seconds.ToString("00", CultureInfo.InvariantCulture)
            Return s
        End Function

        ''' <summary>Parses <c>Z</c>, <c>+HH</c>, <c>+HHmm</c>, <c>+HH:mm</c>, <c>+HHmmss</c> or <c>+HH:mm:ss</c>.</summary>
        Friend Shared Function TryParseOffset(ByVal text As String, ByRef offset As TimeSpan) As Boolean
            offset = TimeSpan.Zero
            If String.IsNullOrEmpty(text) Then Return False
            If text = "Z" Then Return True
            Dim c As Char = text(0)
            If c <> "+"c AndAlso c <> "-"c Then Return False
            Dim body As String = text.Substring(1).Replace(":", "")
            If body.Length <> 2 AndAlso body.Length <> 4 AndAlso body.Length <> 6 Then Return False
            For Each ch As Char In body
                If ch < "0"c OrElse ch > "9"c Then Return False
            Next
            Dim h As Integer = Integer.Parse(body.Substring(0, 2), CultureInfo.InvariantCulture)
            Dim m As Integer = If(body.Length >= 4, Integer.Parse(body.Substring(2, 2), CultureInfo.InvariantCulture), 0)
            Dim s As Integer = If(body.Length = 6, Integer.Parse(body.Substring(4, 2), CultureInfo.InvariantCulture), 0)
            If h > 23 OrElse m > 59 OrElse s > 59 Then Return False
            offset = New TimeSpan(h, m, s)
            If c = "-"c Then offset = offset.Negate()
            Return True
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return Equals(TryCast(obj, DateTimeZone))
        End Function

        Public Overloads Function Equals(ByVal other As DateTimeZone) As Boolean Implements IEquatable(Of DateTimeZone).Equals
            Return other IsNot Nothing AndAlso String.Equals(other.id_, id_, StringComparison.Ordinal)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return 57 + id_.GetHashCode()
        End Function

        ''' <summary>The zone id.</summary>
        Public Overrides Function ToString() As String
            Return id_
        End Function

    End Class

End Namespace
