Imports System
Imports System.Globalization
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Text.Json
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB
Imports DeepLearning4VB.org.joda.time.format
Imports DeepLearning4VB.org.nd4vb.common.util
Imports JodaDateTime = DeepLearning4VB.org.joda.time.DateTime
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports DateTimeFieldType = DeepLearning4VB.org.joda.time.DateTimeFieldType

Namespace org.joda.time

    ''' <summary>
    ''' Tests for the Joda-Time shim (DateTimeZone, DateTimeFieldType, DateTime, DateTimeFormat).
    ''' The reference instant 1451651445000 is 2016-01-01T12:30:45.000Z, a Friday in ISO week 53 of 2015.
    ''' </summary>
    <TestClass>
    Public Class JodaTimeShimTest

        Private Const NewYear2016 As Long = 1451651445000L   ' 2016-01-01T12:30:45Z
        Private Const Midsummer2015 As Long = 1435708799000L ' 2015-06-30T23:59:59Z
        Private Const Midnight2016 As Long = 1451606400000L  ' 2016-01-01T00:00:00Z
        Private Const July2016 As Long = 1467376245000L      ' 2016-07-01T12:30:45Z
        Private Const IndiaOffset As Integer = 19800000      ' +05:30 in millis

        ' ------------------------------------------------------------------ DateTimeZone

        <TestMethod>
        Public Sub DateTimeZone_Utc_IsASingleton()
            assertEquals("UTC", DateTimeZone.UTC.getID())
            assertEquals("UTC", DateTimeZone.UTC.ID)
            assertTrue(DateTimeZone.forID("UTC") Is DateTimeZone.UTC)
            assertTrue(DateTimeZone.forTimeZone(TimeZoneInfo.Utc) Is DateTimeZone.UTC)
            assertTrue(DateTimeZone.forOffsetMillis(0) Is DateTimeZone.UTC)
            assertTrue(DateTimeZone.forOffsetHours(0) Is DateTimeZone.UTC)
            assertTrue(DateTimeZone.UTC.toTimeZone() Is TimeZoneInfo.Utc)
            assertEquals(0, DateTimeZone.UTC.getOffset(NewYear2016))
            assertEquals(0, DateTimeZone.UTC.getStandardOffset(NewYear2016))
            assertTrue(DateTimeZone.UTC.isFixed())
            assertTrue(DateTimeZone.UTC.isStandardOffset(NewYear2016))
            assertEquals("UTC", DateTimeZone.UTC.getShortName(NewYear2016))
            assertEquals("Coordinated Universal Time", DateTimeZone.UTC.getName(NewYear2016))
            assertEquals("UTC", DateTimeZone.UTC.ToString())
            assertEquals(NewYear2016, DateTimeZone.UTC.convertUTCToLocal(NewYear2016))
            assertEquals(NewYear2016, DateTimeZone.UTC.convertLocalToUTC(NewYear2016, True))
        End Sub

        <TestMethod>
        Public Sub DateTimeZone_UtcAliases_ResolveToUtcRules()
            For Each aliasId As String In New String() {"Z", "GMT", "Etc/UTC", "Etc/GMT", "Zulu"}
                Dim z = DateTimeZone.forID(aliasId)
                assertEquals(aliasId, z.getID())
                assertEquals(0, z.getOffset(NewYear2016))
                assertEquals(0, z.getOffset(July2016))
                assertTrue(z.isFixed())
            Next
        End Sub

        <TestMethod>
        Public Sub DateTimeZone_FixedOffset_FromId()
            Dim india = DateTimeZone.forID("+05:30")
            assertEquals("+05:30", india.getID())
            assertEquals(IndiaOffset, india.getOffset(NewYear2016))
            assertEquals(IndiaOffset, india.getOffset(July2016))
            assertEquals(IndiaOffset, india.getStandardOffset(NewYear2016))
            assertTrue(india.isFixed())
            assertEquals(NewYear2016 + IndiaOffset, india.convertUTCToLocal(NewYear2016))
            assertEquals(NewYear2016, india.convertLocalToUTC(NewYear2016 + IndiaOffset, True))

            Dim minus8 = DateTimeZone.forID("-08:00")
            assertEquals(-8 * 3600000, minus8.getOffset(NewYear2016))
            assertEquals(-8 * 3600000, DateTimeZone.forID("-0800").getOffset(NewYear2016))
            assertEquals(2 * 3600000, DateTimeZone.forID("+02").getOffset(NewYear2016))
        End Sub

        <TestMethod>
        Public Sub DateTimeZone_FixedOffset_Factories()
            assertEquals("+05:30", DateTimeZone.forOffsetHoursMinutes(5, 30).getID())
            assertEquals(IndiaOffset, DateTimeZone.forOffsetHoursMinutes(5, 30).getOffset(0))
            assertEquals("-08:00", DateTimeZone.forOffsetHours(-8).getID())
            assertEquals(-8 * 3600000, DateTimeZone.forOffsetHours(-8).getOffset(0))
            assertEquals("-03:30", DateTimeZone.forOffsetHoursMinutes(-3, 30).getID())
            assertEquals(-(3 * 3600000 + 30 * 60000), DateTimeZone.forOffsetHoursMinutes(-3, 30).getOffset(0))
            assertEquals("+01:00", DateTimeZone.forOffsetMillis(3600000).getID())
            assertEquals(DateTimeZone.forID("+05:30"), DateTimeZone.forOffsetHoursMinutes(5, 30))
            assertThrows(Of ArgumentException)(Sub() DateTimeZone.forOffsetHours(24))
            assertThrows(Of ArgumentException)(Sub() DateTimeZone.forOffsetHoursMinutes(1, 60))
            assertThrows(Of ArgumentException)(Sub() DateTimeZone.forOffsetHoursMinutes(1, -30))
        End Sub

        <TestMethod>
        Public Sub DateTimeZone_NewYork_ObservesDaylightSaving()
            Dim ny = DateTimeZone.forID("America/New_York")
            assertEquals("America/New_York", ny.getID())
            assertEquals(-5 * 3600000, ny.getOffset(NewYear2016))
            assertEquals(-4 * 3600000, ny.getOffset(July2016))
            assertEquals(-5 * 3600000, ny.getStandardOffset(July2016))
            assertTrue(ny.isStandardOffset(NewYear2016))
            assertFalse(ny.isStandardOffset(July2016))
            assertFalse(ny.isFixed())
            assertFalse(String.IsNullOrEmpty(ny.getName(NewYear2016)))
            assertFalse(String.IsNullOrEmpty(ny.getShortName(NewYear2016)))
            assertNotEquals(ny.getName(NewYear2016), ny.getName(July2016))
            assertEquals("Eastern Standard Time", ny.toTimeZone().Id)
        End Sub

        <TestMethod>
        Public Sub DateTimeZone_LocalUtcConversion_RoundTrips()
            Dim ny = DateTimeZone.forID("America/New_York")
            For Each utc As Long In New Long() {NewYear2016, July2016, Midsummer2015, Midnight2016}
                Dim local As Long = ny.convertUTCToLocal(utc)
                assertEquals(utc + ny.getOffset(utc), local)
                assertEquals(utc, ny.convertLocalToUTC(local, True))
                assertEquals(utc, ny.convertLocalToUTC(local, False))
            Next
            Dim india = DateTimeZone.forID("+05:30")
            assertEquals(July2016, india.convertLocalToUTC(india.convertUTCToLocal(July2016), True))
        End Sub

        <TestMethod>
        Public Sub DateTimeZone_LocalUtcConversion_HandlesGapAndOverlap()
            Dim ny = DateTimeZone.forID("America/New_York")
            ' 2016-03-13T02:30 does not exist in New York (clocks jump from 02:00 to 03:00).
            Dim gapLocal As Long = 1457836200000L ' 2016-03-13T02:30:00 taken as a UTC wall-clock instant
            assertThrows(Of ArgumentException)(Sub() ny.convertLocalToUTC(gapLocal, True))
            ' Non-strict: the offset in force before the gap (-05:00) is used.
            assertEquals(gapLocal + 5 * 3600000, ny.convertLocalToUTC(gapLocal, False))
            ' 2016-11-06T01:30 happens twice; the earlier instant (daylight offset -04:00) is used.
            Dim overlapLocal As Long = 1478395800000L
            assertEquals(overlapLocal + 4 * 3600000, ny.convertLocalToUTC(overlapLocal, True))
            assertEquals(overlapLocal + 4 * 3600000, ny.convertLocalToUTC(overlapLocal, False))
        End Sub

        <TestMethod>
        Public Sub DateTimeZone_ForTimeZone_WrapsWindowsZones()
            Dim tokyo = DateTimeZone.forTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"))
            assertEquals("Tokyo Standard Time", tokyo.getID())
            assertEquals(9 * 3600000, tokyo.getOffset(NewYear2016))
            assertEquals(tokyo, DateTimeZone.forID("Tokyo Standard Time"))
            assertEquals(9 * 3600000, DateTimeZone.forID("Asia/Tokyo").getOffset(NewYear2016))
            assertNotNull(DateTimeZone.getDefault())
            assertEquals(DateTimeZone.getDefault(), DateTimeZone.forID(Nothing))
            assertEquals(DateTimeZone.getDefault(), DateTimeZone.forTimeZone(Nothing))
        End Sub

        <TestMethod>
        Public Sub DateTimeZone_UnknownId_ThrowsArgumentException()
            assertThrows(Of ArgumentException)(Sub() DateTimeZone.forID("Not/AZone"))
            assertThrows(Of ArgumentException)(Sub() DateTimeZone.forID("+25:00"))
            assertThrows(Of ArgumentException)(Sub() DateTimeZone.forID(""))
            assertThrows(Of ArgumentException)(Sub() DateTimeZone.forID("+5"))
        End Sub

        <TestMethod>
        Public Sub DateTimeZone_EqualityAndHashCode_FollowTheId()
            Dim a = DateTimeZone.forID("America/New_York")
            Dim b = DateTimeZone.forID("America/New_York")
            assertFalse(a Is b)
            assertTrue(a.Equals(b))
            assertTrue(a.Equals(DirectCast(b, Object)))
            assertEquals(a.GetHashCode(), b.GetHashCode())
            assertEquals(a, b)
            ' Same rules, different id: not equal (Joda compares ids).
            assertFalse(a.Equals(DateTimeZone.forID("Eastern Standard Time")))
            assertFalse(a.Equals(DateTimeZone.UTC))
            assertFalse(a.Equals(Nothing))
            assertFalse(a.Equals("America/New_York"))
            assertTrue(DateTimeZone.forID("+05:30").Equals(DateTimeZone.forID("+05:30")))
            assertEquals(DateTimeZone.forID("+05:30").GetHashCode(), DateTimeZone.forID("+05:30").GetHashCode())
        End Sub

        <TestMethod>
        Public Sub DateTimeZone_AvailableIds_IncludeUtcAndIana()
            Dim ids = DateTimeZone.getAvailableIDs()
            assertTrue(ids.Contains("UTC"))
            assertTrue(ids.Contains("America/New_York"))
            assertTrue(ids.Contains("Europe/London"))
            assertTrue(ids.Contains("Eastern Standard Time"))
        End Sub

        <TestMethod>
        Public Sub DateTimeZone_AddIanaMapping_RegistersNewIds()
            assertThrows(Of ArgumentException)(Sub() DateTimeZone.forID("Test/Shim_Zone"))
            DateTimeZone.addIanaMapping("Test/Shim_Zone", "Tokyo Standard Time")
            assertEquals(9 * 3600000, DateTimeZone.forID("Test/Shim_Zone").getOffset(NewYear2016))
            assertTrue(DateTimeZone.getAvailableIDs().Contains("Test/Shim_Zone"))
            assertThrows(Of ArgumentNullException)(Sub() DateTimeZone.addIanaMapping(Nothing, "x"))
        End Sub

        <TestMethod>
        Public Sub DateTimeZone_Json_RoundTrips()
            assertEquals("{""ID"":""UTC""}", JsonSerializer.Serialize(DateTimeZone.UTC))
            Dim back = JsonSerializer.Deserialize(Of DateTimeZone)("{""ID"":""UTC""}")
            assertEquals(DateTimeZone.UTC, back)
            assertEquals(0, back.getOffset(NewYear2016))

            Dim ny = DateTimeZone.forID("America/New_York")
            Dim json = JsonSerializer.Serialize(ny)
            assertEquals("{""ID"":""America/New_York""}", json)
            Dim ny2 = JsonSerializer.Deserialize(Of DateTimeZone)(json)
            assertEquals(ny, ny2)
            assertEquals(-5 * 3600000, ny2.getOffset(NewYear2016))

            Dim india = JsonSerializer.Deserialize(Of DateTimeZone)(JsonSerializer.Serialize(DateTimeZone.forID("+05:30")))
            assertEquals("+05:30", india.getID())
            assertEquals(IndiaOffset, india.getOffset(NewYear2016))

            assertThrows(Of Exception)(Sub() JsonSerializer.Deserialize(Of DateTimeZone)("{""ID"":""Not/AZone""}"))
        End Sub

        <TestMethod>
        Public Sub DateTimeZone_BinarySerialization_RoundTrips()
            Dim ny = DateTimeZone.forID("America/New_York")
            Dim back = SerializationUtils.fromByteArray(Of DateTimeZone)(SerializationUtils.toByteArray(ny))
            assertEquals(ny, back)
            assertEquals(-5 * 3600000, back.getOffset(NewYear2016))
            assertEquals(-4 * 3600000, back.getOffset(July2016))
            Dim utc = SerializationUtils.fromByteArray(Of DateTimeZone)(SerializationUtils.toByteArray(DateTimeZone.UTC))
            assertEquals(DateTimeZone.UTC, utc)
            assertEquals(0, utc.getOffset(July2016))
            assertEquals("UTC", utc.getShortName(July2016))
        End Sub

        ' ------------------------------------------------------------------ DateTimeFieldType

        <TestMethod>
        Public Sub DateTimeFieldType_Names_MatchJoda()
            assertEquals("hourOfDay", DateTimeFieldType.hourOfDay().getName())
            assertEquals("hourOfDay", DateTimeFieldType.hourOfDay().Name)
            assertEquals("hourOfDay", DateTimeFieldType.hourOfDay().ToString())
            assertEquals("dayOfMonth", DateTimeFieldType.dayOfMonth().getName())
            assertEquals("secondOfMinute", DateTimeFieldType.secondOfMinute().getName())
            assertEquals("monthOfYear", DateTimeFieldType.monthOfYear().getName())
            assertEquals("year", DateTimeFieldType.year().getName())
            assertEquals("dayOfWeek", DateTimeFieldType.dayOfWeek().getName())
            assertEquals("weekOfWeekyear", DateTimeFieldType.weekOfWeekyear().getName())
            assertEquals("weekyear", DateTimeFieldType.weekyear().getName())
            assertEquals("millisOfSecond", DateTimeFieldType.millisOfSecond().getName())
            assertEquals("era", DateTimeFieldType.era().getName())
            assertEquals("clockhourOfHalfday", DateTimeFieldType.clockhourOfHalfday().getName())
            assertEquals(23, DateTimeFieldType.values().Length)
        End Sub

        <TestMethod>
        Public Sub DateTimeFieldType_ForName_ReturnsTheSingletons()
            For Each t As DateTimeFieldType In DateTimeFieldType.values()
                assertTrue(DateTimeFieldType.forName(t.getName()) Is t, t.getName())
            Next
            assertTrue(DateTimeFieldType.forName("hourOfDay") Is DateTimeFieldType.hourOfDay())
            assertTrue(DateTimeFieldType.hourOfDay() Is DateTimeFieldType.hourOfDay())
            assertTrue(DateTimeFieldType.minuteOfHour() Is DateTimeFieldType.forName("minuteOfHour"))
            assertFalse(DateTimeFieldType.hourOfDay().Equals(DateTimeFieldType.hourOfHalfday()))
            assertThrows(Of ArgumentException)(Sub() DateTimeFieldType.forName("HourOfDay"))
            assertThrows(Of ArgumentException)(Sub() DateTimeFieldType.forName("bogus"))
            assertThrows(Of ArgumentNullException)(Sub() DateTimeFieldType.forName(Nothing))
        End Sub

        <TestMethod>
        Public Sub DateTimeFieldType_Json_RoundTrips()
            assertEquals("{""Name"":""hourOfDay""}", JsonSerializer.Serialize(DateTimeFieldType.hourOfDay()))
            Dim back = JsonSerializer.Deserialize(Of DateTimeFieldType)("{""Name"":""hourOfDay""}")
            assertEquals(DateTimeFieldType.hourOfDay(), back)
            assertEquals(DateTimeFieldType.hourOfDay().GetHashCode(), back.GetHashCode())
            assertEquals("hourOfDay", back.getName())
            Dim dt As New JodaDateTime(NewYear2016, DateTimeZone.UTC)
            assertEquals(12, dt.[get](back))
            For Each t As DateTimeFieldType In DateTimeFieldType.values()
                assertEquals(t, JsonSerializer.Deserialize(Of DateTimeFieldType)(JsonSerializer.Serialize(t)))
            Next
            assertThrows(Of Exception)(Sub() JsonSerializer.Deserialize(Of DateTimeFieldType)("{""Name"":""bogus""}"))
        End Sub

        <TestMethod>
        Public Sub DateTimeFieldType_BinarySerialization_RoundTrips()
            Dim back = SerializationUtils.fromByteArray(Of DateTimeFieldType)(SerializationUtils.toByteArray(DateTimeFieldType.dayOfWeek()))
            assertEquals(DateTimeFieldType.dayOfWeek(), back)
            assertEquals("dayOfWeek", back.getName())
        End Sub

        ' ------------------------------------------------------------------ DateTime

        <TestMethod>
        Public Sub DateTime_Get_ReadsFieldsInUtc()
            Dim dt As New JodaDateTime(NewYear2016, DateTimeZone.UTC)
            assertEquals(NewYear2016, dt.getMillis())
            assertTrue(dt.getZone() Is DateTimeZone.UTC)
            assertEquals(12, dt.[get](DateTimeFieldType.hourOfDay()))
            assertEquals(1, dt.[get](DateTimeFieldType.dayOfMonth()))
            assertEquals(45, dt.[get](DateTimeFieldType.secondOfMinute()))
            assertEquals(1, dt.[get](DateTimeFieldType.monthOfYear()))
            assertEquals(2016, dt.[get](DateTimeFieldType.year()))
            assertEquals(30, dt.[get](DateTimeFieldType.minuteOfHour()))
            assertEquals(0, dt.[get](DateTimeFieldType.millisOfSecond()))
            assertEquals(5, dt.[get](DateTimeFieldType.dayOfWeek()))
            assertEquals(53, dt.[get](DateTimeFieldType.weekOfWeekyear()))
            assertEquals(2015, dt.[get](DateTimeFieldType.weekyear()))
            assertEquals(1, dt.[get](DateTimeFieldType.dayOfYear()))
            assertEquals(1, dt.[get](DateTimeFieldType.era()))
            assertEquals(20, dt.[get](DateTimeFieldType.centuryOfEra()))
            assertEquals(16, dt.[get](DateTimeFieldType.yearOfCentury()))
            assertEquals(2016, dt.[get](DateTimeFieldType.yearOfEra()))
            assertEquals(15, dt.[get](DateTimeFieldType.weekyearOfCentury()))
            assertEquals(1, dt.[get](DateTimeFieldType.halfdayOfDay()))
            assertEquals(0, dt.[get](DateTimeFieldType.hourOfHalfday()))
            assertEquals(12, dt.[get](DateTimeFieldType.clockhourOfHalfday()))
            assertEquals(12, dt.[get](DateTimeFieldType.clockhourOfDay()))
            assertEquals(12 * 60 + 30, dt.[get](DateTimeFieldType.minuteOfDay()))
            assertEquals(12 * 3600 + 30 * 60 + 45, dt.[get](DateTimeFieldType.secondOfDay()))
            assertEquals((12 * 3600 + 30 * 60 + 45) * 1000, dt.[get](DateTimeFieldType.millisOfDay()))
            assertThrows(Of ArgumentException)(Sub() dt.[get](Nothing))
        End Sub

        <TestMethod>
        Public Sub DateTime_TypedGetters_MatchGet()
            Dim dt As New JodaDateTime(NewYear2016, DateTimeZone.UTC)
            assertEquals(2016, dt.getYear())
            assertEquals(1, dt.getMonthOfYear())
            assertEquals(1, dt.getDayOfMonth())
            assertEquals(12, dt.getHourOfDay())
            assertEquals(30, dt.getMinuteOfHour())
            assertEquals(45, dt.getSecondOfMinute())
            assertEquals(0, dt.getMillisOfSecond())
            assertEquals(5, dt.getDayOfWeek())
            assertEquals(53, dt.getWeekOfWeekyear())
            assertEquals(2015, dt.getWeekyear())
            assertEquals(1, dt.getDayOfYear())
            assertEquals(1, dt.getEra())
            assertEquals(20, dt.getCenturyOfEra())
            assertEquals(16, dt.getYearOfCentury())
            assertEquals(2016, dt.getYearOfEra())
            assertEquals(750, dt.getMinuteOfDay())
            assertEquals(45045, dt.getSecondOfDay())
            assertEquals(45045000, dt.getMillisOfDay())
            For Each t As DateTimeFieldType In DateTimeFieldType.values()
                assertTrue(dt.[get](t) >= 0, t.getName())
            Next
        End Sub

        <TestMethod>
        Public Sub DateTime_ToString_IsIso8601WithOffset()
            assertEquals("2016-01-01T12:30:45.000Z", New JodaDateTime(NewYear2016, DateTimeZone.UTC).ToString())
            assertEquals("2015-06-30T23:59:59.000Z", New JodaDateTime(Midsummer2015, DateTimeZone.UTC).ToString())
            assertEquals("2016-01-01T12:30:45.123Z", New JodaDateTime(NewYear2016 + 123, DateTimeZone.UTC).ToString())
            assertEquals("2016-01-01T18:00:45.000+05:30", New JodaDateTime(NewYear2016, DateTimeZone.forID("+05:30")).ToString())
            assertEquals("2016-01-01T07:30:45.000-05:00", New JodaDateTime(NewYear2016, DateTimeZone.forID("America/New_York")).ToString())
            assertEquals("2016-07-01T08:30:45.000-04:00", New JodaDateTime(July2016, DateTimeZone.forID("America/New_York")).ToString())
        End Sub

        <TestMethod>
        Public Sub DateTime_NonUtcZone_ShiftsTheWallClock()
            Dim ny As New JodaDateTime(NewYear2016, DateTimeZone.forID("America/New_York"))
            assertEquals(NewYear2016, ny.getMillis())
            assertEquals(7, ny.getHourOfDay())
            assertEquals(7, ny.[get](DateTimeFieldType.hourOfDay()))
            assertEquals(30, ny.getMinuteOfHour())
            assertEquals(1, ny.getDayOfMonth())
            Dim india As New JodaDateTime(NewYear2016, DateTimeZone.forID("+05:30"))
            assertEquals(18, india.getHourOfDay())
            assertEquals(0, india.getMinuteOfHour())
            assertEquals(45, india.getSecondOfMinute())
            ' Crossing midnight changes the date, the day of week and the week fields.
            Dim tokyo As New JodaDateTime(NewYear2016, DateTimeZone.forID("Asia/Tokyo"))
            assertEquals(21, tokyo.getHourOfDay())
            assertEquals(1, tokyo.getDayOfMonth())
            Dim lateUtc As New JodaDateTime(Midsummer2015, DateTimeZone.forID("Asia/Tokyo"))
            assertEquals(7, lateUtc.getMonthOfYear())
            assertEquals(1, lateUtc.getDayOfMonth())
            assertEquals(8, lateUtc.getHourOfDay())
            assertEquals(3, lateUtc.getDayOfWeek())
            assertEquals(182, lateUtc.getDayOfYear())
        End Sub

        <TestMethod>
        Public Sub DateTime_WithZone_KeepsTheInstant()
            Dim utc As New JodaDateTime(NewYear2016, DateTimeZone.UTC)
            Dim india = utc.withZone(DateTimeZone.forID("+05:30"))
            assertEquals(NewYear2016, india.getMillis())
            assertEquals("+05:30", india.getZone().getID())
            assertEquals(18, india.getHourOfDay())
            assertTrue(utc.isEqual(india))
            assertEquals(0, utc.CompareTo(india))
            assertFalse(utc.Equals(india))
            assertEquals(utc, india.withZone(DateTimeZone.UTC))
            assertEquals(utc, india.withZone(Nothing).withZone(DateTimeZone.UTC))

            Dim retained = utc.withZoneRetainFields(DateTimeZone.forID("+05:30"))
            assertEquals(NewYear2016 - IndiaOffset, retained.getMillis())
            assertEquals(12, retained.getHourOfDay())
            assertEquals(30, retained.getMinuteOfHour())
            assertEquals("2016-01-01T12:30:45.000+05:30", retained.ToString())
        End Sub

        <TestMethod>
        Public Sub DateTime_PlusMinus_Arithmetic()
            Dim dt As New JodaDateTime(NewYear2016, DateTimeZone.UTC)
            Dim next_ = dt.plusDays(1)
            assertEquals(NewYear2016 + 86400000L, next_.getMillis())
            assertEquals(2, next_.getDayOfMonth())
            assertEquals(6, next_.getDayOfWeek())
            assertTrue(next_.getZone() Is DateTimeZone.UTC)
            assertEquals(dt, next_.minusDays(1))
            Dim prev = dt.minusDays(1)
            assertEquals(2015, prev.getYear())
            assertEquals(12, prev.getMonthOfYear())
            assertEquals(31, prev.getDayOfMonth())
            assertEquals(365, prev.getDayOfYear())
            assertEquals(53, prev.getWeekOfWeekyear())
            assertEquals(2015, prev.getWeekyear())
            assertEquals(2, dt.plusMonths(1).getMonthOfYear())
            assertEquals(2017, dt.plusYears(1).getYear())
            assertEquals(8, dt.plusWeeks(1).getDayOfMonth())
            assertEquals(13, dt.plusHours(1).getHourOfDay())
            assertEquals(31, dt.plusMinutes(1).getMinuteOfHour())
            assertEquals(46, dt.plusSeconds(1).getSecondOfMinute())
            assertEquals(500, dt.plusMillis(500).getMillisOfSecond())
            assertEquals(NewYear2016 + 1234, dt.plus(1234).getMillis())
            assertEquals(NewYear2016 - 1234, dt.minus(1234).getMillis())
            assertEquals(dt, dt.plusHours(2).minusHours(2))
            assertEquals(dt, dt.plusMinutes(5).minusMinutes(5))
            assertEquals(dt, dt.plusSeconds(5).minusSeconds(5))
            assertEquals(dt, dt.plusMillis(5).minusMillis(5))
            assertEquals(dt, dt.plusWeeks(1).minusWeeks(1))
            assertEquals(dt, dt.plusMonths(3).minusMonths(3))
            assertEquals(dt, dt.plusYears(2).minusYears(2))
            assertTrue(dt.withMillis(NewYear2016) Is dt)
            assertEquals(Midnight2016, dt.withMillis(Midnight2016).getMillis())
        End Sub

        <TestMethod>
        Public Sub DateTime_PlusDays_KeepsWallClockAcrossDst()
            Dim ny = DateTimeZone.forID("America/New_York")
            ' 2016-03-12T12:00 EST -> plusDays(1) -> 2016-03-13T12:00 EDT (23 elapsed hours).
            Dim before As New JodaDateTime(2016, 3, 12, 12, 0, 0, 0, ny)
            Dim after = before.plusDays(1)
            assertEquals(12, after.getHourOfDay())
            assertEquals(13, after.getDayOfMonth())
            assertEquals(23L * 3600000L, after.getMillis() - before.getMillis())
            assertEquals(24L * 3600000L, before.plusHours(24).getMillis() - before.getMillis())
            assertEquals(13, before.plusHours(24).getHourOfDay())
        End Sub

        <TestMethod>
        Public Sub DateTime_Comparison_UsesTheInstant()
            Dim a As New JodaDateTime(NewYear2016, DateTimeZone.UTC)
            Dim b As New JodaDateTime(NewYear2016 + 1, DateTimeZone.UTC)
            Dim c As New JodaDateTime(NewYear2016, DateTimeZone.forID("Asia/Tokyo"))
            assertTrue(a.isBefore(b))
            assertTrue(a.isBefore(NewYear2016 + 1))
            assertFalse(a.isBefore(a))
            assertTrue(b.isAfter(a))
            assertTrue(b.isAfter(NewYear2016))
            assertFalse(a.isAfter(b))
            assertTrue(a.isEqual(c))
            assertFalse(a.isEqual(b))
            assertTrue(a.CompareTo(b) < 0)
            assertTrue(b.CompareTo(a) > 0)
            assertEquals(0, a.CompareTo(c))
            assertTrue(a.CompareTo(Nothing) > 0)
            assertTrue(a.Equals(New JodaDateTime(NewYear2016, DateTimeZone.UTC)))
            assertEquals(a.GetHashCode(), New JodaDateTime(NewYear2016, DateTimeZone.UTC).GetHashCode())
            assertFalse(a.Equals(b))
            assertFalse(a.Equals(c))
            assertFalse(a.Equals(Nothing))
            Dim list As New List(Of JodaDateTime) From {b, a}
            list.Sort()
            assertTrue(list(0) Is a)
            ' isBefore/isAfter(Nothing) compare with now
            Dim past As New JodaDateTime(NewYear2016, DateTimeZone.UTC)
            assertTrue(past.isBefore(DirectCast(Nothing, JodaDateTime)))
            assertFalse(past.isAfter(DirectCast(Nothing, JodaDateTime)))
        End Sub

        <TestMethod>
        Public Sub DateTime_FieldConstructors_AndParse()
            Dim fromFields As New JodaDateTime(2016, 1, 1, 12, 30, 45, 0, DateTimeZone.UTC)
            assertEquals(NewYear2016, fromFields.getMillis())
            assertEquals(NewYear2016, New JodaDateTime(2016, 1, 1, 12, 30, 45, DateTimeZone.UTC).getMillis())
            assertEquals(NewYear2016 - 45000, New JodaDateTime(2016, 1, 1, 12, 30, DateTimeZone.UTC).getMillis())
            assertEquals(NewYear2016, New JodaDateTime(2016, 1, 1, 18, 0, 45, 0, DateTimeZone.forID("+05:30")).getMillis())
            assertEquals(NewYear2016, New JodaDateTime(2016, 1, 1, 7, 30, 45, 0, DateTimeZone.forID("America/New_York")).getMillis())
            assertEquals(NewYear2016, New JodaDateTime(New Date(2016, 1, 1, 12, 30, 45, DateTimeKind.Utc), DateTimeZone.UTC).getMillis())
            assertEquals(NewYear2016, New JodaDateTime(New Date(2016, 1, 1, 12, 30, 45, DateTimeKind.Unspecified), DateTimeZone.UTC).getMillis())
            assertEquals(NewYear2016, New JodaDateTime(New Date(2016, 1, 1, 12, 30, 45, DateTimeKind.Utc).ToLocalTime(), DateTimeZone.UTC).getMillis())

            Dim parsed = JodaDateTime.parse("2016-01-01T12:30:45.000Z")
            assertEquals(NewYear2016, parsed.getMillis())
            assertTrue(parsed.getZone() Is DateTimeZone.UTC)
            Dim parsedIndia = JodaDateTime.parse("2016-01-01T18:00:45+05:30")
            assertEquals(NewYear2016, parsedIndia.getMillis())
            assertEquals("+05:30", parsedIndia.getZone().getID())
            assertEquals("2016-01-01T18:00:45.000+05:30", parsedIndia.ToString())
            assertThrows(Of ArgumentException)(Sub() JodaDateTime.parse("not a date"))
            assertThrows(Of ArgumentNullException)(Sub() JodaDateTime.parse(Nothing))

            Dim toDate = fromFields.toDate()
            assertEquals(DateTimeKind.Utc, toDate.Kind)
            assertEquals(New Date(2016, 1, 1, 12, 30, 45, DateTimeKind.Utc), toDate)
            Dim dto = fromFields.withZone(DateTimeZone.forID("+05:30")).toDateTimeOffset()
            assertEquals(TimeSpan.FromMinutes(330), dto.Offset)
            assertEquals(18, dto.Hour)
            assertEquals(NewYear2016, dto.ToUnixTimeMilliseconds())

            assertTrue(JodaDateTime.now(DateTimeZone.UTC).getMillis() > 1600000000000L)
            assertTrue(JodaDateTime.now().getMillis() > 1600000000000L)
            assertEquals(DateTimeZone.getDefault(), New JodaDateTime(NewYear2016).getZone())
            assertEquals(DateTimeZone.getDefault(), New JodaDateTime(NewYear2016, Nothing).getZone())
        End Sub

        <TestMethod>
        Public Sub DateTime_BinarySerialization_RoundTrips()
            Dim dt As New JodaDateTime(NewYear2016, DateTimeZone.forID("America/New_York"))
            Dim back = SerializationUtils.fromByteArray(Of JodaDateTime)(SerializationUtils.toByteArray(dt))
            assertEquals(dt, back)
            assertEquals(7, back.getHourOfDay())
            assertEquals(dt.ToString(), back.ToString())
        End Sub

        ' ------------------------------------------------------------------ DateTimeFormat

        <TestMethod>
        Public Sub DateTimeFormat_ParseMillis_BasicPattern()
            Dim fmt = DateTimeFormat.forPattern("YYYY-MM-dd HH:mm:ss").withZone(DateTimeZone.UTC)
            assertEquals(NewYear2016, fmt.parseMillis("2016-01-01 12:30:45"))
            assertEquals(Midsummer2015, fmt.parseMillis("2015-06-30 23:59:59"))
            assertEquals(Midnight2016, fmt.parseMillis("2016-01-01 00:00:00"))
            assertEquals("YYYY-MM-dd HH:mm:ss", fmt.getPattern())
            assertTrue(fmt.getZone() Is DateTimeZone.UTC)
            assertEquals("DateTimeFormatter[YYYY-MM-dd HH:mm:ss, UTC]", fmt.ToString())
            Dim dt = fmt.parseDateTime("2016-01-01 12:30:45")
            assertEquals(NewYear2016, dt.getMillis())
            assertTrue(dt.getZone() Is DateTimeZone.UTC)
            assertEquals(12, dt.getHourOfDay())
        End Sub

        <TestMethod>
        Public Sub DateTimeFormat_Print_RoundTrips()
            Dim fmt = DateTimeFormat.forPattern("YYYY-MM-dd HH:mm:ss").withZone(DateTimeZone.UTC)
            assertEquals("2016-01-01 12:30:45", fmt.print(NewYear2016))
            assertEquals("2015-06-30 23:59:59", fmt.print(Midsummer2015))
            For Each text As String In New String() {"2016-01-01 12:30:45", "2015-06-30 23:59:59", "1999-12-31 00:00:00", "2024-02-29 23:00:01"}
                assertEquals(text, fmt.print(fmt.parseMillis(text)))
            Next
            For Each instant As Long In New Long() {NewYear2016, Midsummer2015, Midnight2016, July2016, 0L}
                assertEquals(instant, fmt.parseMillis(fmt.print(instant)))
            Next
            ' print(DateTime) uses the value's zone when the formatter has none, the formatter's zone otherwise.
            Dim noZone = DateTimeFormat.forPattern("YYYY-MM-dd HH:mm:ss")
            assertNull(noZone.getZone())
            assertEquals("2016-01-01 12:30:45", noZone.print(New JodaDateTime(NewYear2016, DateTimeZone.UTC)))
            assertEquals("2016-01-01 18:00:45", noZone.print(New JodaDateTime(NewYear2016, DateTimeZone.forID("+05:30"))))
            assertEquals("2016-01-01 12:30:45", fmt.print(New JodaDateTime(NewYear2016, DateTimeZone.forID("+05:30"))))
            assertThrows(Of ArgumentNullException)(Sub() fmt.print(DirectCast(Nothing, JodaDateTime)))
        End Sub

        <TestMethod>
        Public Sub DateTimeFormat_ZoneName_Pattern()
            Dim fmt = DateTimeFormat.forPattern("YYYY-MM-dd HH:mm:ss.SSS zzz").withZone(DateTimeZone.UTC)
            assertEquals("2016-01-01 12:30:45.000 UTC", fmt.print(NewYear2016))
            assertEquals("2016-01-01 12:30:45.123 UTC", fmt.print(NewYear2016 + 123))
            assertEquals(NewYear2016, fmt.parseMillis("2016-01-01 12:30:45.000 UTC"))
            assertEquals(NewYear2016 + 123, fmt.parseMillis("2016-01-01 12:30:45.123 UTC"))
            ' A parseable zone name in the text is honoured.
            assertEquals(NewYear2016 + 5 * 3600000, fmt.parseMillis("2016-01-01 12:30:45.000 America/New_York"))
            assertEquals("2016-01-01 12:30:45.000 Coordinated Universal Time", DateTimeFormat.forPattern("YYYY-MM-dd HH:mm:ss.SSS zzzz").withZone(DateTimeZone.UTC).print(NewYear2016))
        End Sub

        <TestMethod>
        Public Sub DateTimeFormat_LiteralText()
            Dim fmt = DateTimeFormat.forPattern("yyyy-MM-dd'T'HH:mm:ss").withZone(DateTimeZone.UTC)
            assertEquals("2016-01-01T12:30:45", fmt.print(NewYear2016))
            assertEquals(NewYear2016, fmt.parseMillis("2016-01-01T12:30:45"))
            Dim quoted = DateTimeFormat.forPattern("'year' yyyy 'o''clock' HH").withZone(DateTimeZone.UTC)
            assertEquals("year 2016 o'clock 12", quoted.print(NewYear2016))
            assertEquals(Midnight2016 + 12 * 3600000L, quoted.parseMillis("year 2016 o'clock 12"))
            Dim doubleQuote = DateTimeFormat.forPattern("yyyy''MM").withZone(DateTimeZone.UTC)
            assertEquals("2016'01", doubleQuote.print(NewYear2016))
            assertThrows(Of ArgumentException)(Sub() DateTimeFormat.forPattern("yyyy 'unclosed"))
        End Sub

        <TestMethod>
        Public Sub DateTimeFormat_TwoDigitYear()
            Dim fmt = DateTimeFormat.forPattern("dd/MM/yy").withZone(DateTimeZone.UTC)
            assertEquals("01/01/16", fmt.print(NewYear2016))
            assertEquals("30/06/15", fmt.print(Midsummer2015))
            assertEquals(Midnight2016, fmt.parseMillis("01/01/16"))
            assertEquals(1435622400000L, fmt.parseMillis("30/06/15"))
            assertEquals(915148800000L, fmt.parseMillis("01/01/99"))
            assertEquals(2049, fmt.parseDateTime("01/01/49").getYear())
            assertEquals(1950, fmt.parseDateTime("01/01/50").getYear())
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("01/01/2016"))
        End Sub

        <TestMethod>
        Public Sub DateTimeFormat_MonthNames()
            Dim abbr = DateTimeFormat.forPattern("MMM d, yyyy").withZone(DateTimeZone.UTC)
            assertEquals("Jan 1, 2016", abbr.print(NewYear2016))
            assertEquals("Jun 30, 2015", abbr.print(Midsummer2015))
            assertEquals(Midnight2016, abbr.parseMillis("Jan 1, 2016"))
            assertEquals(1435622400000L, abbr.parseMillis("Jun 30, 2015"))
            assertEquals(1435622400000L, abbr.parseMillis("June 30, 2015"))
            assertEquals(Midnight2016, abbr.parseMillis("jan 1, 2016"))
            assertThrows(Of ArgumentException)(Sub() abbr.parseMillis("Foo 1, 2016"))

            Dim full = DateTimeFormat.forPattern("MMMM d, yyyy").withZone(DateTimeZone.UTC)
            assertEquals("January 1, 2016", full.print(NewYear2016))
            assertEquals("June 30, 2015", full.print(Midsummer2015))
            assertEquals(Midnight2016, full.parseMillis("January 1, 2016"))
            assertEquals("December", DateTimeFormat.forPattern("MMMM").withZone(DateTimeZone.UTC).print(Midnight2016 - 1))

            Dim dayNames = DateTimeFormat.forPattern("EEEE, EEE d MMMM yyyy").withZone(DateTimeZone.UTC)
            assertEquals("Friday, Fri 1 January 2016", dayNames.print(NewYear2016))
            assertEquals(Midnight2016, dayNames.parseMillis("Friday, Fri 1 January 2016"))
        End Sub

        <TestMethod>
        Public Sub DateTimeFormat_TwelveHourClock()
            Dim fmt = DateTimeFormat.forPattern("hh:mm a").withZone(DateTimeZone.UTC)
            assertEquals("12:30 PM", fmt.print(NewYear2016))
            assertEquals("12:00 AM", fmt.print(Midnight2016))
            assertEquals("11:59 PM", fmt.print(Midsummer2015))
            assertEquals("01:05 AM", fmt.print(Midnight2016 + 65 * 60000L))
            assertEquals("01:05 PM", fmt.print(Midnight2016 + (12 * 60 + 65) * 60000L))
            assertEquals(12 * 3600000L + 30 * 60000L, fmt.parseMillis("12:30 PM"))
            assertEquals(0L, fmt.parseMillis("12:00 AM"))
            assertEquals(13 * 3600000L + 5 * 60000L, fmt.parseMillis("01:05 PM"))
            assertEquals(65 * 60000L, fmt.parseMillis("01:05 AM"))
            assertEquals(65 * 60000L, fmt.parseMillis("1:05 am"))
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("13:05 PM"))
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("00:05 AM"))
            Dim k = DateTimeFormat.forPattern("KK:mm a").withZone(DateTimeZone.UTC)
            assertEquals("00:30 PM", k.print(NewYear2016))
            assertEquals(12 * 3600000L + 30 * 60000L, k.parseMillis("00:30 PM"))
            Dim clock24 = DateTimeFormat.forPattern("kk:mm").withZone(DateTimeZone.UTC)
            assertEquals("24:00", clock24.print(Midnight2016))
            assertEquals(0L, clock24.parseMillis("24:00"))
            assertEquals(Midnight2016 + 12 * 3600000L, DateTimeFormat.forPattern("yyyy-MM-dd HH a").withZone(DateTimeZone.UTC).parseMillis("2016-01-01 12 PM"))
        End Sub

        <TestMethod>
        Public Sub DateTimeFormat_DayOfYear()
            Dim fmt = DateTimeFormat.forPattern("yyyy-D").withZone(DateTimeZone.UTC)
            assertEquals("2016-1", fmt.print(NewYear2016))
            assertEquals("2015-181", fmt.print(Midsummer2015))
            assertEquals(Midnight2016, fmt.parseMillis("2016-1"))
            assertEquals(1435622400000L, fmt.parseMillis("2015-181"))
            assertEquals(1451520000000L, fmt.parseMillis("2015-365"))
            assertEquals(1483142400000L, fmt.parseMillis("2016-366"))
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("2015-366"))
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("2015-0"))
            assertEquals("2016-001", DateTimeFormat.forPattern("yyyy-DDD").withZone(DateTimeZone.UTC).print(NewYear2016))
            assertEquals("2015-181", DateTimeFormat.forPattern("yyyy-DDD").withZone(DateTimeZone.UTC).print(Midsummer2015))
        End Sub

        <TestMethod>
        Public Sub DateTimeFormat_AdjacentNumericFields()
            Dim fmt = DateTimeFormat.forPattern("yyyyMMddHHmmss").withZone(DateTimeZone.UTC)
            assertEquals("20160101123045", fmt.print(NewYear2016))
            assertEquals("20150630235959", fmt.print(Midsummer2015))
            assertEquals(NewYear2016, fmt.parseMillis("20160101123045"))
            assertEquals(Midsummer2015, fmt.parseMillis("20150630235959"))
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("201601011230"))
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("201601011230450"))
            Dim withFraction = DateTimeFormat.forPattern("yyyyMMddHHmmssSSS").withZone(DateTimeZone.UTC)
            assertEquals("20160101123045123", withFraction.print(NewYear2016 + 123))
            assertEquals(NewYear2016 + 123, withFraction.parseMillis("20160101123045123"))
            Dim compact = DateTimeFormat.forPattern("yyyyMMdd").withZone(DateTimeZone.UTC)
            assertEquals("20160101", compact.print(NewYear2016))
            assertEquals(Midnight2016, compact.parseMillis("20160101"))
        End Sub

        <TestMethod>
        Public Sub DateTimeFormat_Fractions()
            Dim utc = DateTimeZone.UTC
            assertEquals("45.1", DateTimeFormat.forPattern("ss.S").withZone(utc).print(NewYear2016 + 123))
            assertEquals("45.12", DateTimeFormat.forPattern("ss.SS").withZone(utc).print(NewYear2016 + 123))
            assertEquals("45.123", DateTimeFormat.forPattern("ss.SSS").withZone(utc).print(NewYear2016 + 123))
            assertEquals("45.123000", DateTimeFormat.forPattern("ss.SSSSSS").withZone(utc).print(NewYear2016 + 123))
            assertEquals(45120L, DateTimeFormat.forPattern("ss.SS").withZone(utc).parseMillis("45.12"))
            assertEquals(45123L, DateTimeFormat.forPattern("ss.SSS").withZone(utc).parseMillis("45.123"))
            assertEquals(45100L, DateTimeFormat.forPattern("ss.SSS").withZone(utc).parseMillis("45.1"))
        End Sub

        <TestMethod>
        Public Sub DateTimeFormat_ZoneOffset_Printing()
            Dim utc = DateTimeZone.UTC
            Dim india = DateTimeZone.forID("+05:30")
            assertEquals("2016-01-01T12:30:45+0000", DateTimeFormat.forPattern("yyyy-MM-dd'T'HH:mm:ssZ").withZone(utc).print(NewYear2016))
            assertEquals("2016-01-01T12:30:45+00:00", DateTimeFormat.forPattern("yyyy-MM-dd'T'HH:mm:ssZZ").withZone(utc).print(NewYear2016))
            assertEquals("2016-01-01T12:30:45UTC", DateTimeFormat.forPattern("yyyy-MM-dd'T'HH:mm:ssZZZ").withZone(utc).print(NewYear2016))
            assertEquals("2016-01-01T18:00:45+0530", DateTimeFormat.forPattern("yyyy-MM-dd'T'HH:mm:ssZ").withZone(india).print(NewYear2016))
            assertEquals("2016-01-01T18:00:45+05:30", DateTimeFormat.forPattern("yyyy-MM-dd'T'HH:mm:ssZZ").withZone(india).print(NewYear2016))
            assertEquals("2016-01-01T18:00:45+05:30", DateTimeFormat.forPattern("yyyy-MM-dd'T'HH:mm:ssZZZ").withZone(india).print(NewYear2016))
            Dim ny = DateTimeZone.forID("America/New_York")
            assertEquals("-0500", DateTimeFormat.forPattern("Z").withZone(ny).print(NewYear2016))
            assertEquals("-04:00", DateTimeFormat.forPattern("ZZ").withZone(ny).print(July2016))
            assertEquals("America/New_York", DateTimeFormat.forPattern("ZZZ").withZone(ny).print(July2016))
            assertEquals("-08:00", DateTimeFormat.forPattern("ZZ").withZone(DateTimeZone.forOffsetHours(-8)).print(NewYear2016))
        End Sub

        <TestMethod>
        Public Sub DateTimeFormat_ZoneOffset_Parsing()
            Dim fmt = DateTimeFormat.forPattern("yyyy-MM-dd'T'HH:mm:ssZZ").withZone(DateTimeZone.UTC)
            assertEquals(NewYear2016 - IndiaOffset, fmt.parseMillis("2016-01-01T12:30:45+05:30"))
            assertEquals(NewYear2016, fmt.parseMillis("2016-01-01T12:30:45+00:00"))
            assertEquals(NewYear2016, fmt.parseMillis("2016-01-01T12:30:45Z"))
            assertEquals(NewYear2016 + 5 * 3600000, fmt.parseMillis("2016-01-01T12:30:45-05:00"))
            assertEquals(NewYear2016 + 5 * 3600000, fmt.parseMillis("2016-01-01T12:30:45-0500"))
            assertEquals(NewYear2016 + 5 * 3600000, fmt.parseMillis("2016-01-01T12:30:45-05"))
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("2016-01-01T12:30:45"))
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("2016-01-01T12:30:45+25:00"))

            Dim singleZ = DateTimeFormat.forPattern("yyyy-MM-dd'T'HH:mm:ssZ").withZone(DateTimeZone.UTC)
            assertEquals(NewYear2016 - IndiaOffset, singleZ.parseMillis("2016-01-01T12:30:45+0530"))
            assertEquals(NewYear2016 - IndiaOffset, singleZ.parseMillis("2016-01-01T12:30:45+05:30"))

            ' The parsed zone is the formatter's unless withOffsetParsed() is used.
            Dim dt = fmt.parseDateTime("2016-01-01T12:30:45+05:30")
            assertTrue(dt.getZone() Is DateTimeZone.UTC)
            assertEquals(7, dt.getHourOfDay())
            Dim kept = fmt.withOffsetParsed().parseDateTime("2016-01-01T12:30:45+05:30")
            assertEquals(NewYear2016 - IndiaOffset, kept.getMillis())
            assertEquals("+05:30", kept.getZone().getID())
            assertEquals(12, kept.getHourOfDay())
            assertEquals("2016-01-01T12:30:45.000+05:30", kept.ToString())
            Dim offsetParsed = fmt.withOffsetParsed()
            assertTrue(offsetParsed.withOffsetParsed() Is offsetParsed)
            assertFalse(offsetParsed Is fmt)

            Dim zoneId = DateTimeFormat.forPattern("yyyy-MM-dd'T'HH:mm:ss ZZZ").withZone(DateTimeZone.UTC)
            assertEquals(NewYear2016, zoneId.parseMillis("2016-01-01T12:30:45 UTC"))
            assertEquals(NewYear2016 - IndiaOffset, zoneId.parseMillis("2016-01-01T12:30:45 +05:30"))
            assertEquals(NewYear2016 + 5 * 3600000, zoneId.parseMillis("2016-01-01T12:30:45 America/New_York"))
            assertEquals("America/New_York", zoneId.withOffsetParsed().parseDateTime("2016-01-01T12:30:45 America/New_York").getZone().getID())
            assertThrows(Of ArgumentException)(Sub() zoneId.parseMillis("2016-01-01T12:30:45 Not/AZone"))
        End Sub

        <TestMethod>
        Public Sub DateTimeFormat_FormatterZone_InterpretsLocalTime()
            Dim ny = DateTimeFormat.forPattern("yyyy-MM-dd HH:mm:ss").withZone(DateTimeZone.forID("America/New_York"))
            assertEquals(NewYear2016 + 5 * 3600000, ny.parseMillis("2016-01-01 12:30:45"))
            assertEquals(July2016 + 4 * 3600000, ny.parseMillis("2016-07-01 12:30:45"))
            assertEquals("2016-01-01 07:30:45", ny.print(NewYear2016))
            assertEquals("2016-07-01 08:30:45", ny.print(July2016))
            assertEquals("America/New_York", ny.parseDateTime("2016-01-01 12:30:45").getZone().getID())
            Dim india = ny.withZone(DateTimeZone.forID("+05:30"))
            assertEquals(NewYear2016 - IndiaOffset, india.parseMillis("2016-01-01 12:30:45"))
            assertTrue(ny.withZone(DateTimeZone.forID("America/New_York")) Is ny)
            assertTrue(ny.withZoneUTC().getZone() Is DateTimeZone.UTC)
        End Sub

        <TestMethod>
        Public Sub DateTimeFormat_WeekBasedFields()
            Dim utc = DateTimeZone.UTC
            assertEquals("2015-W53-5", DateTimeFormat.forPattern("xxxx-'W'ww-e").withZone(utc).print(NewYear2016))
            assertEquals("2015-W27-2", DateTimeFormat.forPattern("xxxx-'W'ww-e").withZone(utc).print(Midsummer2015))
            assertEquals(Midnight2016, DateTimeFormat.forPattern("xxxx-'W'ww-e").withZone(utc).parseMillis("2015-W53-5"))
            assertEquals(1435622400000L, DateTimeFormat.forPattern("xxxx-'W'ww-e").withZone(utc).parseMillis("2015-W27-2"))
            assertEquals(1451865600000L, DateTimeFormat.forPattern("xxxx-'W'ww").withZone(utc).parseMillis("2016-W01"))
            assertEquals("AD 20 16", DateTimeFormat.forPattern("G C YY").withZone(utc).print(NewYear2016))
        End Sub

        <TestMethod>
        Public Sub DateTimeFormat_InvalidInput_ThrowsArgumentException()
            Dim fmt = DateTimeFormat.forPattern("yyyy-MM-dd HH:mm:ss").withZone(DateTimeZone.UTC)
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("not a date"))
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis(""))
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("2016-01-01 12:30:45xyz"))
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("2016-01-01 12:30:45 "))
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis(" 2016-01-01 12:30:45"))
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("2016-13-01 12:30:45"))
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("2016-00-01 12:30:45"))
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("2016-02-30 12:30:45"))
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("2015-02-29 12:30:45"))
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("2016-01-01 24:30:45"))
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("2016-01-01 12:60:45"))
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("2016-01-01 12:30:60"))
            assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("2016-01-01"))
            assertThrows(Of ArgumentNullException)(Sub() fmt.parseMillis(Nothing))
            Dim ex = assertThrows(Of ArgumentException)(Sub() fmt.parseMillis("2016-13-01 12:30:45"))
            assertTrue(ex.Message.Contains("monthOfYear"), ex.Message)
            ' 2016-02-29 is a valid leap day.
            assertEquals(1456704000000L, fmt.parseMillis("2016-02-29 00:00:00"))
        End Sub

        <TestMethod>
        Public Sub DateTimeFormat_UnknownPatternLetter_Throws()
            assertThrows(Of ArgumentException)(Sub() DateTimeFormat.forPattern("yyyy-MM-dd Q"))
            assertThrows(Of ArgumentException)(Sub() DateTimeFormat.forPattern("yyyy-MM-dd j"))
            assertThrows(Of ArgumentException)(Sub() DateTimeFormat.forPattern("yyyy-MM-dd VV"))
            assertThrows(Of ArgumentException)(Sub() DateTimeFormat.forPattern(""))
            assertThrows(Of ArgumentException)(Sub() DateTimeFormat.forPattern(Nothing))
            ' Every documented letter is accepted.
            For Each letter As String In New String() {"G", "C", "Y", "x", "w", "e", "E", "y", "D", "M", "d", "a", "K", "h", "H", "k", "m", "s", "S", "z", "Z"}
                assertNotNull(DateTimeFormat.forPattern(letter))
            Next
        End Sub

        <TestMethod>
        Public Sub DateTimeFormat_WithLocale_UsesGermanNames()
            Dim de As New CultureInfo("de-DE")
            Dim fmt = DateTimeFormat.forPattern("d. MMMM yyyy").withZone(DateTimeZone.UTC).withLocale(de)
            assertEquals("1. Januar 2016", fmt.print(NewYear2016))
            assertEquals("30. Juni 2015", fmt.print(Midsummer2015))
            assertEquals(Midnight2016, fmt.parseMillis("1. Januar 2016"))
            assertEquals(1435622400000L, fmt.parseMillis("30. Juni 2015"))
            ' Invariant (English) names are still accepted when parsing.
            assertEquals(Midnight2016, fmt.parseMillis("1. January 2016"))
            assertEquals(de, fmt.getLocale())
            assertEquals("Dezember", DateTimeFormat.forPattern("MMMM").withZone(DateTimeZone.UTC).withLocale(de).print(Midnight2016 - 1))
            assertEquals("Freitag", DateTimeFormat.forPattern("EEEE").withZone(DateTimeZone.UTC).withLocale(de).print(NewYear2016))
            assertEquals("Freitag, 1. Januar 2016", DateTimeFormat.forPattern("EEEE, d. MMMM yyyy").withZone(DateTimeZone.UTC).withLocale(de).print(NewYear2016))
            assertEquals(Midnight2016, DateTimeFormat.forPattern("EEEE, d. MMMM yyyy").withZone(DateTimeZone.UTC).withLocale(de).parseMillis("Freitag, 1. Januar 2016"))
            ' Numbers are culture independent.
            assertEquals("2016-01-01 12:30:45", DateTimeFormat.forPattern("yyyy-MM-dd HH:mm:ss").withZone(DateTimeZone.UTC).withLocale(de).print(NewYear2016))
            ' The invariant culture is the default and withLocale(Nothing) restores it.
            Dim english = fmt.withLocale(Nothing)
            assertNull(english.getLocale())
            assertEquals("1. January 2016", english.print(NewYear2016))
            assertTrue(fmt.withLocale(de) Is fmt)
            Dim fr = DateTimeFormat.forPattern("MMMM").withZone(DateTimeZone.UTC).withLocale(New CultureInfo("fr-FR")).print(NewYear2016)
            assertEquals("janvier", fr)
        End Sub

        <TestMethod>
        Public Sub DateTimeFormat_BinarySerialization_RoundTrips()
            Dim fmt = DateTimeFormat.forPattern("yyyy-MM-dd HH:mm:ss").withZone(DateTimeZone.UTC)
            assertEquals(NewYear2016, fmt.parseMillis("2016-01-01 12:30:45")) ' build the cached parser first
            Dim back = SerializationUtils.fromByteArray(Of DateTimeFormatter)(SerializationUtils.toByteArray(fmt))
            assertNotNull(back)
            assertEquals("yyyy-MM-dd HH:mm:ss", back.getPattern())
            assertEquals(DateTimeZone.UTC, back.getZone())
            assertEquals(NewYear2016, back.parseMillis("2016-01-01 12:30:45"))
            assertEquals(Midsummer2015, back.parseMillis("2015-06-30 23:59:59"))
            assertEquals("2016-01-01 12:30:45", back.print(NewYear2016))
            assertThrows(Of ArgumentException)(Sub() back.parseMillis("2016-13-01 12:30:45"))

            ' Also with a locale, a fixed-offset zone and the offset-parsed flag, through a plain BinaryFormatter.
            Dim de = DateTimeFormat.forPattern("d. MMMM yyyy HH:mm ZZ").withZone(DateTimeZone.forID("+05:30")).withLocale(New CultureInfo("de-DE")).withOffsetParsed()
            Dim bf As New BinaryFormatter()
            Dim bytes As Byte()
            Using ms As New MemoryStream()
                bf.Serialize(ms, de)
                bytes = ms.ToArray()
            End Using
            Dim de2 As DateTimeFormatter
            Using ms As New MemoryStream(bytes)
                de2 = DirectCast(bf.Deserialize(ms), DateTimeFormatter)
            End Using
            assertEquals("de-DE", de2.getLocale().Name)
            assertEquals("+05:30", de2.getZone().getID())
            assertEquals("1. Januar 2016 18:00 +05:30", de2.print(NewYear2016))
            ' the pattern has no seconds, so the parsed instant is the top of the minute
            assertEquals(NewYear2016 - 45000, de2.parseMillis("1. Januar 2016 18:00 +05:30"))
            assertEquals("-05:00", de2.parseDateTime("1. Januar 2016 07:30 -05:00").getZone().getID())
            assertEquals(NewYear2016 - 45000, de2.parseMillis("1. Januar 2016 07:30 -05:00"))

            ' A New York zone survives too (adjustment rules included).
            Dim ny = DateTimeFormat.forPattern("yyyy-MM-dd HH:mm:ss").withZone(DateTimeZone.forID("America/New_York"))
            Dim ny2 = SerializationUtils.fromByteArray(Of DateTimeFormatter)(SerializationUtils.toByteArray(ny))
            assertEquals("2016-07-01 08:30:45", ny2.print(July2016))
            assertEquals(July2016, ny2.parseMillis("2016-07-01 08:30:45"))
        End Sub

    End Class

End Namespace
