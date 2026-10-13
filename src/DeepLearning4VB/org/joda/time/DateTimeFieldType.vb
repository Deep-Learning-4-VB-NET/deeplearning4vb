Imports System
Imports System.Text.Json.Serialization

' DeepLearning4VB shim for the part of the Joda-Time API that DataVec uses (org.joda.time.DateTimeFieldType).
' Joda-Time is Apache-2.0 licensed (https://www.joda.org/joda-time/); this file is an independent
' re-implementation, not a translation of the Joda source.

Namespace org.joda.time

    ''' <summary>
    ''' Identifies a date/time field (hour of day, day of month, ...) that can be read from a
    ''' <see cref="DateTime"/> with <c>[get]</c>. Instances are singletons obtained from the shared
    ''' factory methods; <see cref="forName"/> looks one up by its Joda name. Serializes with
    ''' System.Text.Json as <c>{"Name":"hourOfDay"}</c>.
    ''' </summary>
    <Serializable>
    Public NotInheritable Class DateTimeFieldType
        Implements IEquatable(Of DateTimeFieldType)

        Friend Enum Field
            Era = 0
            CenturyOfEra = 1
            YearOfCentury = 2
            YearOfEra = 3
            Year = 4
            WeekyearOfCentury = 5
            Weekyear = 6
            WeekOfWeekyear = 7
            DayOfYear = 8
            MonthOfYear = 9
            DayOfMonth = 10
            DayOfWeek = 11
            HalfdayOfDay = 12
            HourOfHalfday = 13
            ClockhourOfHalfday = 14
            ClockhourOfDay = 15
            HourOfDay = 16
            MinuteOfDay = 17
            MinuteOfHour = 18
            SecondOfDay = 19
            SecondOfMinute = 20
            MillisOfDay = 21
            MillisOfSecond = 22
        End Enum

        Private Shared ReadOnly Names As String() = {
            "era", "centuryOfEra", "yearOfCentury", "yearOfEra", "year", "weekyearOfCentury", "weekyear",
            "weekOfWeekyear", "dayOfYear", "monthOfYear", "dayOfMonth", "dayOfWeek", "halfdayOfDay",
            "hourOfHalfday", "clockhourOfHalfday", "clockhourOfDay", "hourOfDay", "minuteOfDay",
            "minuteOfHour", "secondOfDay", "secondOfMinute", "millisOfDay", "millisOfSecond"}

        Private Shared ReadOnly Instances As DateTimeFieldType() = CreateInstances()

        Private ReadOnly field_ As Field
        Private ReadOnly name_ As String

        Private Sub New(ByVal field As Field)
            Me.field_ = field
            Me.name_ = Names(CInt(field))
        End Sub

        ''' <summary>Creates the field type with the given Joda name (used by System.Text.Json); prefer <see cref="forName"/>.</summary>
        <JsonConstructor>
        Public Sub New(ByVal name As String)
            Dim canonical As DateTimeFieldType = forName(name)
            Me.field_ = canonical.field_
            Me.name_ = canonical.name_
        End Sub

        Private Shared Function CreateInstances() As DateTimeFieldType()
            Dim ret(Names.Length - 1) As DateTimeFieldType
            For i As Integer = 0 To Names.Length - 1
                ret(i) = New DateTimeFieldType(CType(i, Field))
            Next
            Return ret
        End Function

        ''' <summary>The Joda field name, for example <c>hourOfDay</c>.</summary>
        Public ReadOnly Property Name As String
            Get
                Return name_
            End Get
        End Property

        Public Function getName() As String
            Return name_
        End Function

        ''' <summary>Looks a field type up by its Joda name.</summary>
        Public Shared Function forName(ByVal name As String) As DateTimeFieldType
            If name Is Nothing Then Throw New ArgumentNullException("name")
            For Each t As DateTimeFieldType In Instances
                If String.Equals(t.name_, name, StringComparison.Ordinal) Then Return t
            Next
            Throw New ArgumentException("Unknown DateTimeFieldType name: " & name)
        End Function

        ''' <summary>All field types, in Joda order.</summary>
        Public Shared Function values() As DateTimeFieldType()
            Return DirectCast(Instances.Clone(), DateTimeFieldType())
        End Function

        Public Shared Function era() As DateTimeFieldType
            Return Instances(CInt(Field.Era))
        End Function

        Public Shared Function centuryOfEra() As DateTimeFieldType
            Return Instances(CInt(Field.CenturyOfEra))
        End Function

        Public Shared Function yearOfCentury() As DateTimeFieldType
            Return Instances(CInt(Field.YearOfCentury))
        End Function

        Public Shared Function yearOfEra() As DateTimeFieldType
            Return Instances(CInt(Field.YearOfEra))
        End Function

        Public Shared Function year() As DateTimeFieldType
            Return Instances(CInt(Field.Year))
        End Function

        Public Shared Function weekyearOfCentury() As DateTimeFieldType
            Return Instances(CInt(Field.WeekyearOfCentury))
        End Function

        Public Shared Function weekyear() As DateTimeFieldType
            Return Instances(CInt(Field.Weekyear))
        End Function

        Public Shared Function weekOfWeekyear() As DateTimeFieldType
            Return Instances(CInt(Field.WeekOfWeekyear))
        End Function

        Public Shared Function dayOfYear() As DateTimeFieldType
            Return Instances(CInt(Field.DayOfYear))
        End Function

        Public Shared Function monthOfYear() As DateTimeFieldType
            Return Instances(CInt(Field.MonthOfYear))
        End Function

        Public Shared Function dayOfMonth() As DateTimeFieldType
            Return Instances(CInt(Field.DayOfMonth))
        End Function

        Public Shared Function dayOfWeek() As DateTimeFieldType
            Return Instances(CInt(Field.DayOfWeek))
        End Function

        Public Shared Function halfdayOfDay() As DateTimeFieldType
            Return Instances(CInt(Field.HalfdayOfDay))
        End Function

        Public Shared Function hourOfHalfday() As DateTimeFieldType
            Return Instances(CInt(Field.HourOfHalfday))
        End Function

        Public Shared Function clockhourOfHalfday() As DateTimeFieldType
            Return Instances(CInt(Field.ClockhourOfHalfday))
        End Function

        Public Shared Function clockhourOfDay() As DateTimeFieldType
            Return Instances(CInt(Field.ClockhourOfDay))
        End Function

        Public Shared Function hourOfDay() As DateTimeFieldType
            Return Instances(CInt(Field.HourOfDay))
        End Function

        Public Shared Function minuteOfDay() As DateTimeFieldType
            Return Instances(CInt(Field.MinuteOfDay))
        End Function

        Public Shared Function minuteOfHour() As DateTimeFieldType
            Return Instances(CInt(Field.MinuteOfHour))
        End Function

        Public Shared Function secondOfDay() As DateTimeFieldType
            Return Instances(CInt(Field.SecondOfDay))
        End Function

        Public Shared Function secondOfMinute() As DateTimeFieldType
            Return Instances(CInt(Field.SecondOfMinute))
        End Function

        Public Shared Function millisOfDay() As DateTimeFieldType
            Return Instances(CInt(Field.MillisOfDay))
        End Function

        Public Shared Function millisOfSecond() As DateTimeFieldType
            Return Instances(CInt(Field.MillisOfSecond))
        End Function

        ''' <summary>Reads this field from a wall-clock time (ISO week rules, Monday = 1).</summary>
        Friend Function GetValue(ByVal local As Date) As Integer
            Select Case field_
                Case Field.Era
                    Return 1
                Case Field.CenturyOfEra
                    Return local.Year \ 100
                Case Field.YearOfCentury
                    Return local.Year Mod 100
                Case Field.YearOfEra, Field.Year
                    Return local.Year
                Case Field.WeekyearOfCentury
                    Return IsoWeekyear(local) Mod 100
                Case Field.Weekyear
                    Return IsoWeekyear(local)
                Case Field.WeekOfWeekyear
                    Return IsoWeek(local)
                Case Field.DayOfYear
                    Return local.DayOfYear
                Case Field.MonthOfYear
                    Return local.Month
                Case Field.DayOfMonth
                    Return local.Day
                Case Field.DayOfWeek
                    Return IsoDayOfWeek(local)
                Case Field.HalfdayOfDay
                    Return If(local.Hour < 12, 0, 1)
                Case Field.HourOfHalfday
                    Return local.Hour Mod 12
                Case Field.ClockhourOfHalfday
                    Return If(local.Hour Mod 12 = 0, 12, local.Hour Mod 12)
                Case Field.ClockhourOfDay
                    Return If(local.Hour = 0, 24, local.Hour)
                Case Field.HourOfDay
                    Return local.Hour
                Case Field.MinuteOfDay
                    Return local.Hour * 60 + local.Minute
                Case Field.MinuteOfHour
                    Return local.Minute
                Case Field.SecondOfDay
                    Return (local.Hour * 60 + local.Minute) * 60 + local.Second
                Case Field.SecondOfMinute
                    Return local.Second
                Case Field.MillisOfDay
                    Return CInt(local.TimeOfDay.Ticks \ TimeSpan.TicksPerMillisecond)
                Case Field.MillisOfSecond
                    Return local.Millisecond
            End Select
            Throw New InvalidOperationException("Unknown field: " & name_)
        End Function

        ''' <summary>ISO day of week: Monday = 1 ... Sunday = 7.</summary>
        Friend Shared Function IsoDayOfWeek(ByVal d As Date) As Integer
            Return (CInt(d.DayOfWeek) + 6) Mod 7 + 1
        End Function

        ''' <summary>ISO-8601 week-based year (the year of the Thursday in the same week).</summary>
        Friend Shared Function IsoWeekyear(ByVal d As Date) As Integer
            Return d.Date.AddDays(4 - IsoDayOfWeek(d)).Year
        End Function

        ''' <summary>ISO-8601 week of the week-based year (1..53).</summary>
        Friend Shared Function IsoWeek(ByVal d As Date) As Integer
            Dim thursday As Date = d.Date.AddDays(4 - IsoDayOfWeek(d))
            Return (thursday.DayOfYear - 1) \ 7 + 1
        End Function

        ''' <summary>The date of the given ISO week-based year, week and ISO day of week.</summary>
        Friend Shared Function FromIsoWeekDate(ByVal weekyear As Integer, ByVal week As Integer, ByVal dayOfWeek As Integer) As Date
            Dim jan4 As New Date(weekyear, 1, 4)
            Dim mondayOfWeek1 As Date = jan4.AddDays(1 - IsoDayOfWeek(jan4))
            Return mondayOfWeek1.AddDays((week - 1) * 7 + (dayOfWeek - 1))
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return Equals(TryCast(obj, DateTimeFieldType))
        End Function

        Public Overloads Function Equals(ByVal other As DateTimeFieldType) As Boolean Implements IEquatable(Of DateTimeFieldType).Equals
            Return other IsNot Nothing AndAlso other.field_ = field_
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return CInt(field_)
        End Function

        ''' <summary>The Joda field name.</summary>
        Public Overrides Function ToString() As String
            Return name_
        End Function

    End Class

End Namespace
