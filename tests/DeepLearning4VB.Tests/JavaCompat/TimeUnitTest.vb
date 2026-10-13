Imports System
Imports System.Diagnostics
Imports System.Text.Json
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB
Imports DeepLearning4VB.JavaCompat

Namespace JavaCompat

    ''' <summary>Tests for the java.util.concurrent.TimeUnit shim (enum plus extension methods).</summary>
    <TestClass>
    Public Class TimeUnitTest

        ''' <summary>A holder so the enum converter is also exercised as a property.</summary>
        Public Class Holder
            Public Property Unit As TimeUnit
            Public Property Amount As Long
        End Class

        <TestMethod>
        Public Sub Conversions_MatchJava()
            assertEquals(43200000L, TimeUnit.HOURS.toMillis(12))
            assertEquals(300000L, TimeUnit.MILLISECONDS.convert(5, TimeUnit.MINUTES))
            assertEquals(86400L, TimeUnit.DAYS.toSeconds(1))
            assertEquals(1L, TimeUnit.SECONDS.toMinutes(119))
            assertEquals(2L, TimeUnit.SECONDS.toMinutes(120))
            assertEquals(1L, TimeUnit.NANOSECONDS.toMillis(1500000))
            assertEquals(0L, TimeUnit.NANOSECONDS.toMillis(999999))
            assertEquals(1000L, TimeUnit.MICROSECONDS.toNanos(1))
            assertEquals(1000000L, TimeUnit.SECONDS.toMicros(1))
            assertEquals(1L, TimeUnit.HOURS.toDays(47))
            assertEquals(2L, TimeUnit.HOURS.toDays(48))
            assertEquals(2L, TimeUnit.MINUTES.toHours(120))
            assertEquals(60L, TimeUnit.HOURS.toMinutes(1))
            assertEquals(1000000000L, TimeUnit.SECONDS.toNanos(1))
            assertEquals(86400000000000L, TimeUnit.DAYS.toNanos(1))
            assertEquals(7L, TimeUnit.DAYS.convert(1, TimeUnit.DAYS) * 7)
            assertEquals(1L, TimeUnit.DAYS.convert(24, TimeUnit.HOURS))
            assertEquals(0L, TimeUnit.DAYS.convert(23, TimeUnit.HOURS))
            assertEquals(90000L, TimeUnit.MILLISECONDS.convert(90, TimeUnit.SECONDS))
            assertEquals(5000000L, TimeUnit.NANOSECONDS.convert(5, TimeUnit.MILLISECONDS))
        End Sub

        <TestMethod>
        Public Sub Conversions_TruncateTowardsZero()
            assertEquals(-1L, TimeUnit.MILLISECONDS.toSeconds(-1500))
            assertEquals(-1L, TimeUnit.SECONDS.toMinutes(-119))
            assertEquals(0L, TimeUnit.MILLISECONDS.toSeconds(-999))
            assertEquals(-43200000L, TimeUnit.HOURS.toMillis(-12))
            assertEquals(0L, TimeUnit.HOURS.toMillis(0))
        End Sub

        <TestMethod>
        Public Sub SameUnit_ReturnsTheDuration()
            For Each u As TimeUnit In [Enum].GetValues(GetType(TimeUnit))
                assertEquals(12345L, u.convert(12345, u), u.ToString())
                assertEquals(Long.MaxValue, u.convert(Long.MaxValue, u))
                assertEquals(Long.MinValue, u.convert(Long.MinValue, u))
            Next
            assertEquals(5L, TimeUnit.NANOSECONDS.toNanos(5))
            assertEquals(5L, TimeUnit.DAYS.toDays(5))
        End Sub

        <TestMethod>
        Public Sub Overflow_SaturatesLikeJava()
            assertEquals(Long.MaxValue, TimeUnit.DAYS.toNanos(Long.MaxValue))
            assertEquals(Long.MinValue, TimeUnit.DAYS.toNanos(Long.MinValue))
            assertEquals(Long.MaxValue, TimeUnit.DAYS.toNanos(106752))
            assertEquals(Long.MinValue, TimeUnit.DAYS.toNanos(-106752))
            assertEquals(9223286400000000000L, TimeUnit.DAYS.toNanos(106751))
            assertEquals(-9223286400000000000L, TimeUnit.DAYS.toNanos(-106751))
            assertEquals(Long.MaxValue, TimeUnit.HOURS.toMillis(Long.MaxValue))
            assertEquals(Long.MinValue, TimeUnit.HOURS.toMillis(Long.MinValue))
            assertEquals(Long.MaxValue, TimeUnit.NANOSECONDS.convert(Long.MaxValue, TimeUnit.SECONDS))
            assertEquals(Long.MinValue, TimeUnit.MILLISECONDS.convert(Long.MinValue, TimeUnit.DAYS))
            ' Going to a coarser unit never overflows.
            assertEquals(106751L, TimeUnit.NANOSECONDS.toDays(Long.MaxValue))
            assertEquals(-106751L, TimeUnit.NANOSECONDS.toDays(Long.MinValue))
            assertEquals(Long.MaxValue \ 1000L, TimeUnit.MILLISECONDS.toSeconds(Long.MaxValue))
        End Sub

        <TestMethod>
        Public Sub ToTimeSpan_ConvertsToTicks()
            assertEquals(TimeSpan.FromMinutes(90), TimeUnit.MINUTES.toTimeSpan(90))
            assertEquals(TimeSpan.FromDays(1), TimeUnit.DAYS.toTimeSpan(1))
            assertEquals(TimeSpan.FromHours(12), TimeUnit.HOURS.toTimeSpan(12))
            assertEquals(TimeSpan.FromSeconds(119), TimeUnit.SECONDS.toTimeSpan(119))
            assertEquals(TimeSpan.FromMilliseconds(1500), TimeUnit.MILLISECONDS.toTimeSpan(1500))
            assertEquals(TimeSpan.FromTicks(10), TimeUnit.MICROSECONDS.toTimeSpan(1))
            assertEquals(TimeSpan.FromTicks(1), TimeUnit.NANOSECONDS.toTimeSpan(150))
            assertEquals(TimeSpan.FromTicks(0), TimeUnit.NANOSECONDS.toTimeSpan(99))
            assertEquals(TimeSpan.Zero, TimeUnit.HOURS.toTimeSpan(0))
            assertEquals(TimeSpan.FromHours(-2), TimeUnit.HOURS.toTimeSpan(-2))
            assertEquals(1500.0, TimeUnit.MILLISECONDS.toTimeSpan(1500).TotalMilliseconds, 0.0)
        End Sub

        <TestMethod>
        Public Sub Sleep_BlocksForTheDuration()
            Dim sw As Stopwatch = Stopwatch.StartNew()
            TimeUnit.MILLISECONDS.sleep(30)
            sw.Stop()
            assertTrue(sw.ElapsedMilliseconds >= 20, "slept only " & sw.ElapsedMilliseconds & " ms")
            sw.Restart()
            TimeUnit.NANOSECONDS.sleep(1)
            TimeUnit.SECONDS.sleep(0)
            TimeUnit.SECONDS.sleep(-5)
            sw.Stop()
            assertTrue(sw.ElapsedMilliseconds < 1000)
        End Sub

        <TestMethod>
        Public Sub ValueOf_ParsesNames()
            assertEquals(TimeUnit.HOURS, TimeUnitValueOf("HOURS"))
            assertEquals(TimeUnit.NANOSECONDS, TimeUnitValueOf("NANOSECONDS"))
            assertEquals(TimeUnit.DAYS, TimeUnitValueOf("DAYS"))
            For Each u As TimeUnit In [Enum].GetValues(GetType(TimeUnit))
                assertEquals(u, TimeUnitValueOf(u.ToString()))
            Next
            assertEquals(7, [Enum].GetValues(GetType(TimeUnit)).Length)
            assertEquals("HOURS", TimeUnit.HOURS.ToString())
            Dim ex = assertThrows(Of ArgumentException)(Sub() TimeUnitValueOf("FORTNIGHTS"))
            assertTrue(ex.Message.Contains("FORTNIGHTS"), ex.Message)
            assertThrows(Of ArgumentException)(Sub() TimeUnitValueOf("hours"))
            assertThrows(Of ArgumentException)(Sub() TimeUnitValueOf(""))
            assertThrows(Of ArgumentNullException)(Sub() TimeUnitValueOf(Nothing))
        End Sub

        <TestMethod>
        Public Sub EnumValues_HaveJavaOrdinals()
            assertEquals(0, CInt(TimeUnit.NANOSECONDS))
            assertEquals(1, CInt(TimeUnit.MICROSECONDS))
            assertEquals(2, CInt(TimeUnit.MILLISECONDS))
            assertEquals(3, CInt(TimeUnit.SECONDS))
            assertEquals(4, CInt(TimeUnit.MINUTES))
            assertEquals(5, CInt(TimeUnit.HOURS))
            assertEquals(6, CInt(TimeUnit.DAYS))
            assertTrue(TimeUnit.NANOSECONDS < TimeUnit.DAYS)
            assertThrows(Of ArgumentException)(Sub() CType(7, TimeUnit).toMillis(1))
            assertThrows(Of ArgumentException)(Sub() TimeUnit.SECONDS.convert(1, CType(-1, TimeUnit)))
        End Sub

        <TestMethod>
        Public Sub Json_SerializesAsTheName()
            assertEquals("""HOURS""", JsonSerializer.Serialize(TimeUnit.HOURS))
            assertEquals("""NANOSECONDS""", JsonSerializer.Serialize(TimeUnit.NANOSECONDS))
            assertEquals(TimeUnit.HOURS, JsonSerializer.Deserialize(Of TimeUnit)("""HOURS"""))
            assertEquals(TimeUnit.DAYS, JsonSerializer.Deserialize(Of TimeUnit)("""DAYS"""))
            For Each u As TimeUnit In [Enum].GetValues(GetType(TimeUnit))
                assertEquals("""" & u.ToString() & """", JsonSerializer.Serialize(u))
                assertEquals(u, JsonSerializer.Deserialize(Of TimeUnit)(JsonSerializer.Serialize(u)))
            Next
            assertEquals("[""SECONDS"",""DAYS""]", JsonSerializer.Serialize(New TimeUnit() {TimeUnit.SECONDS, TimeUnit.DAYS}))
            assertArrayEquals(New TimeUnit() {TimeUnit.SECONDS, TimeUnit.DAYS}, JsonSerializer.Deserialize(Of TimeUnit())("[""SECONDS"",""DAYS""]"))

            Dim h As New Holder With {.Unit = TimeUnit.MINUTES, .Amount = 5}
            Dim json As String = JsonSerializer.Serialize(h)
            assertEquals("{""Unit"":""MINUTES"",""Amount"":5}", json)
            Dim back = JsonSerializer.Deserialize(Of Holder)(json)
            assertEquals(TimeUnit.MINUTES, back.Unit)
            assertEquals(5L, back.Amount)
            assertEquals(300000L, TimeUnit.MILLISECONDS.convert(back.Amount, back.Unit))
            assertThrows(Of JsonException)(Sub() JsonSerializer.Deserialize(Of TimeUnit)("""FORTNIGHTS"""))
        End Sub

    End Class

End Namespace
