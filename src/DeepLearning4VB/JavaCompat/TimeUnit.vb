Imports System
Imports System.Runtime.CompilerServices
Imports System.Text.Json.Serialization
Imports System.Threading

Namespace JavaCompat

    ''' <summary>
    ''' <c>java.util.concurrent.TimeUnit</c>. The conversion methods are extension methods in
    ''' <see cref="TimeUnitExtensions"/>, so <c>TimeUnit.HOURS.toMillis(12)</c> and
    ''' <c>TimeUnit.MILLISECONDS.convert(5, TimeUnit.MINUTES)</c> read like the Java code
    ''' (import <c>DeepLearning4VB.JavaCompat</c>). Serializes as its name with System.Text.Json.
    ''' </summary>
    <JsonConverter(GetType(JsonStringEnumConverter))>
    Public Enum TimeUnit
        NANOSECONDS = 0
        MICROSECONDS = 1
        MILLISECONDS = 2
        SECONDS = 3
        MINUTES = 4
        HOURS = 5
        DAYS = 6
    End Enum

    Public Module TimeUnitExtensions

        Private ReadOnly NanosPerUnit As Long() = {1L, 1000L, 1000000L, 1000000000L, 60000000000L, 3600000000000L, 86400000000000L}

        Private Function Scale(ByVal unit As TimeUnit) As Long
            Dim i As Integer = CInt(unit)
            If i < 0 OrElse i >= NanosPerUnit.Length Then Throw New ArgumentException("Unknown TimeUnit: " & unit)
            Return NanosPerUnit(i)
        End Function

        ''' <summary>Converts a duration in <paramref name="sourceUnit"/> to this unit, truncating; saturates at Long.MinValue/MaxValue on overflow like Java.</summary>
        <Extension>
        Public Function convert(ByVal unit As TimeUnit, ByVal sourceDuration As Long, ByVal sourceUnit As TimeUnit) As Long
            Dim src As Long = Scale(sourceUnit)
            Dim dst As Long = Scale(unit)
            If src = dst Then Return sourceDuration
            If src > dst Then
                Dim ratio As Long = src \ dst
                Dim limit As Long = Long.MaxValue \ ratio
                If sourceDuration > limit Then Return Long.MaxValue
                If sourceDuration < -limit Then Return Long.MinValue
                Return sourceDuration * ratio
            End If
            Return sourceDuration \ (dst \ src)
        End Function

        <Extension>
        Public Function toNanos(ByVal unit As TimeUnit, ByVal duration As Long) As Long
            Return convert(TimeUnit.NANOSECONDS, duration, unit)
        End Function

        <Extension>
        Public Function toMicros(ByVal unit As TimeUnit, ByVal duration As Long) As Long
            Return convert(TimeUnit.MICROSECONDS, duration, unit)
        End Function

        <Extension>
        Public Function toMillis(ByVal unit As TimeUnit, ByVal duration As Long) As Long
            Return convert(TimeUnit.MILLISECONDS, duration, unit)
        End Function

        <Extension>
        Public Function toSeconds(ByVal unit As TimeUnit, ByVal duration As Long) As Long
            Return convert(TimeUnit.SECONDS, duration, unit)
        End Function

        <Extension>
        Public Function toMinutes(ByVal unit As TimeUnit, ByVal duration As Long) As Long
            Return convert(TimeUnit.MINUTES, duration, unit)
        End Function

        <Extension>
        Public Function toHours(ByVal unit As TimeUnit, ByVal duration As Long) As Long
            Return convert(TimeUnit.HOURS, duration, unit)
        End Function

        <Extension>
        Public Function toDays(ByVal unit As TimeUnit, ByVal duration As Long) As Long
            Return convert(TimeUnit.DAYS, duration, unit)
        End Function

        ''' <summary>The duration as a <see cref="TimeSpan"/> (nanoseconds are rounded to ticks).</summary>
        <Extension>
        Public Function toTimeSpan(ByVal unit As TimeUnit, ByVal duration As Long) As TimeSpan
            Return TimeSpan.FromTicks(convert(TimeUnit.NANOSECONDS, duration, unit) \ 100L)
        End Function

        ''' <summary><c>TimeUnit.sleep</c>: blocks the current thread for the duration.</summary>
        <Extension>
        Public Sub sleep(ByVal unit As TimeUnit, ByVal timeout As Long)
            Dim ms As Long = toMillis(unit, timeout)
            If ms > 0 Then Thread.Sleep(TimeSpan.FromMilliseconds(ms))
        End Sub

        ''' <summary><c>TimeUnit.valueOf(name)</c>; throws <see cref="ArgumentException"/> for an unknown name.</summary>
        Public Function TimeUnitValueOf(ByVal name As String) As TimeUnit
            If name Is Nothing Then Throw New ArgumentNullException("name")
            Dim result As TimeUnit
            ' Enum.TryParse also accepts numeric strings ("5"); Java's valueOf only accepts the constant names.
            If name.Length > 0 AndAlso Char.IsLetter(name(0)) AndAlso [Enum].TryParse(Of TimeUnit)(name, False, result) AndAlso [Enum].IsDefined(GetType(TimeUnit), result) Then Return result
            Throw New ArgumentException("No enum constant java.util.concurrent.TimeUnit." & name)
        End Function

    End Module

End Namespace
