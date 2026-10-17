Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Runtime.Serialization
Imports System.Text
Imports System.Text.Json.Serialization
Imports System.Text.RegularExpressions
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports DateTimeFormat = DeepLearning4VB.org.joda.time.format.DateTimeFormat
Imports DateTimeFormatter = DeepLearning4VB.org.joda.time.format.DateTimeFormatter
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports TimeMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.TimeMetaData
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.time

    ''' <summary>
    ''' Convert a String column
    ''' to a time column by parsing the date/time String, using a JodaTime.
    ''' <p>
    ''' Time format is specified as per http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html
    ''' </p>
    ''' <para>Without a time format, a list of common formats is tried in turn (fractional seconds are removed first).
    ''' The formatters are not serialized; they are rebuilt after deserialization (Java <c>readObject</c>), with the
    ''' locale in both modes (Java dropped the locale for the list of common formats).</para>
    ''' <para>Equality (Lombok, excluding the formatters) compares the class, the column name, the format, the zone, the
    ''' locale and the valid time range.</para>
    ''' </summary>
    ' Java: @Data @EqualsAndHashCode(exclude = {"formatter", "formatters"}) @JsonIgnoreProperties({"formatters", "formatter"})
    <Serializable>
    Public Class StringToTimeTransform
        Inherits BaseColumnTransform

        Private ReadOnly timeFormat As String
        Private ReadOnly timeZone As DateTimeZone
        Private ReadOnly locale As CultureInfo
        Private ReadOnly minValidTime As Long?
        Private ReadOnly maxValidTime As Long?

        ' formats from:
        ' http://www.java2s.com/Tutorials/Java/Data_Type_How_to/Legacy_Date_Format/Guess_the_format_pattern_based_on_date_value.htm
        ' 2017-09-21T17:06:29.064687
        ' 12/1/2010 11:21
        Private Shared ReadOnly formats() As String = {"YYYY-MM-dd'T'HH:mm:ss", "YYYY-MM-dd", "YYYY-MM-dd'T'HH:mm:ss'Z'", "YYYY-MM-dd'T'HH:mm:ssZ", "YYYY-MM-dd'T'HH:mm:ss.SSS'Z'", "YYYY-MM-dd'T'HH:mm:ss.SSSZ", "YYYY-MM-dd HH:mm:ss", "MM/dd/YYYY HH:mm:ss", "MM/dd/YYYY'T'HH:mm:ss.SSS'Z'", "MM/dd/YYYY'T'HH:mm:ss.SSSZ", "MM/dd/YYYY'T'HH:mm:ss.SSS", "MM/dd/YYYY'T'HH:mm:ssZ", "MM/dd/YYYY'T'HH:mm:ss", "YYYY:MM:dd HH:mm:ss", "YYYYMMdd", "YYYY-MM-dd HH:mm:ss", "MM/dd/YYYY HH:mm"}

        <NonSerialized>
        Private formatters() As DateTimeFormatter
        <NonSerialized>
        Private formatter As DateTimeFormatter

        ''' <summary>
        ''' Instantiate this without a time format specified. If this constructor is
        ''' used, this transform will be allowed to handle several common transforms as
        ''' defined in the static formats array.
        ''' </summary>
        ''' <param name="columnName"> Name of the String column </param>
        ''' <param name="timeZone">   Timezone for time parsing </param>
        Public Sub New(ByVal columnName As String, ByVal timeZone As TimeZoneInfo)
            Me.New(columnName, Nothing, timeZone, Nothing, Nothing, Nothing)
        End Sub

        ''' <param name="columnName"> Name of the String column </param>
        ''' <param name="timeZone">   Timezone for time parsing </param>
        ''' <param name="locale">     Locale for i18n </param>
        Public Sub New(ByVal columnName As String, ByVal timeZone As TimeZoneInfo, ByVal locale As CultureInfo)
            Me.New(columnName, Nothing, timeZone, locale, Nothing, Nothing)
        End Sub

        ''' <param name="columnName"> Name of the String column </param>
        ''' <param name="timeFormat"> Time format, as per <a href=
        '''                   "http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html">http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html</a> </param>
        ''' <param name="timeZone">   Timezone for time parsing </param>
        Public Sub New(ByVal columnName As String, ByVal timeFormat As String, ByVal timeZone As TimeZoneInfo)
            Me.New(columnName, timeFormat, timeZone, Nothing, Nothing, Nothing)
        End Sub

        ''' <param name="columnName"> Name of the String column </param>
        ''' <param name="timeFormat"> Time format, as per <a href=
        '''                   "http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html">http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html</a> </param>
        ''' <param name="timeZone">   Timezone for time parsing </param>
        ''' <param name="locale">     Locale for i18n </param>
        Public Sub New(ByVal columnName As String, ByVal timeFormat As String, ByVal timeZone As TimeZoneInfo, ByVal locale As CultureInfo)
            Me.New(columnName, timeFormat, timeZone, locale, Nothing, Nothing)
        End Sub

        ''' <summary>
        ''' Instantiate this without a time format specified. If this constructor is
        ''' used, this transform will be allowed to handle several common transforms as
        ''' defined in the static formats array.
        ''' </summary>
        ''' <param name="columnName"> Name of the String column </param>
        ''' <param name="timeZone">   Timezone for time parsing </param>
        ''' <param name="locale">     Locale for i18n </param>
        Public Sub New(ByVal columnName As String, ByVal timeZone As DateTimeZone, ByVal locale As CultureInfo)
            Me.New(columnName, Nothing, timeZone, locale, Nothing, Nothing)
        End Sub

        ''' <param name="columnName"> Name of the String column </param>
        ''' <param name="timeFormat"> Time format, as per <a href=
        '''                   "http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html">http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html</a> </param>
        ''' <param name="timeZone">   Timezone for time parsing </param>
        Public Sub New(ByVal columnName As String, ByVal timeFormat As String, ByVal timeZone As DateTimeZone)
            Me.New(columnName, timeFormat, timeZone, Nothing, Nothing, Nothing)
        End Sub

        ''' <param name="columnName"> Name of the String column </param>
        ''' <param name="timeFormat"> Time format, as per <a href=
        '''                   "http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html">http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html</a> </param>
        ''' <param name="timeZone">   Timezone for time parsing </param>
        ''' <param name="locale">     Locale for i18n </param>
        Public Sub New(ByVal columnName As String, ByVal timeFormat As String, ByVal timeZone As DateTimeZone, ByVal locale As CultureInfo)
            Me.New(columnName, timeFormat, timeZone, locale, Nothing, Nothing)
        End Sub

        ''' <param name="columnName">   Name of the String column </param>
        ''' <param name="timeFormat">   Time format, as per <a href=
        '''                     "http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html">http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html</a> </param>
        ''' <param name="timeZone">     Timezone for time parsing </param>
        ''' <param name="locale">       Locale for i18n </param>
        ''' <param name="minValidTime"> Min valid time (epoch millisecond format). If null: no
        '''                     restriction in min valid time </param>
        ''' <param name="maxValidTime"> Max valid time (epoch millisecond format). If null: no
        '''                     restriction in max valid time </param>
        Public Sub New(ByVal columnName As String, ByVal timeFormat As String, ByVal timeZone As TimeZoneInfo, ByVal locale As CultureInfo, ByVal minValidTime As Long?, ByVal maxValidTime As Long?)
            Me.New(columnName, timeFormat, DateTimeZone.forTimeZone(timeZone), locale, minValidTime, maxValidTime)
        End Sub

        ''' <param name="columnName">   Name of the String column </param>
        ''' <param name="timeFormat">   Time format, as per <a href=
        '''                     "http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html">http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html</a> </param>
        ''' <param name="timeZone">     Timezone for time parsing </param>
        ''' <param name="locale">       Locale for i18n </param>
        ''' <param name="minValidTime"> Min valid time (epoch millisecond format). If null: no
        '''                     restriction in min valid time </param>
        ''' <param name="maxValidTime"> Max valid time (epoch millisecond format). If null: no
        '''                     restriction in max valid time </param>
        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal timeFormat As String, ByVal timeZone As DateTimeZone, ByVal locale As CultureInfo, ByVal minValidTime As Long?, ByVal maxValidTime As Long?)
            MyBase.New(columnName)
            Me.timeFormat = timeFormat
            Me.timeZone = timeZone
            Me.locale = locale
            Me.minValidTime = minValidTime
            Me.maxValidTime = maxValidTime
            initFormatters()
        End Sub

        ''' <summary>Builds the formatter (or the list of common formatters when no format is given).</summary>
        Private Sub initFormatters()
            If timeFormat IsNot Nothing Then
                formatter = newFormatter(timeFormat)
            Else
                Dim dateFormatList As New List(Of DateTimeFormatter)()
                For i As Integer = 0 To formats.Length - 1
                    dateFormatList.Add(newFormatter(formats(i)))
                Next i
                formatters = dateFormatList.ToArray()
            End If
        End Sub

        Private Function newFormatter(ByVal pattern As String) As DateTimeFormatter
            If locale IsNot Nothing Then
                Return DateTimeFormat.forPattern(pattern).withZone(timeZone).withLocale(locale)
            Else
                Return DateTimeFormat.forPattern(pattern).withZone(timeZone)
            End If
        End Function

        ''' <summary>Java <c>readObject</c>: the formatters are not serialized and are rebuilt here.</summary>
        <OnDeserialized>
        Private Sub onDeserialized(ByVal context As StreamingContext)
            initFormatters()
        End Sub

        ''' <summary>Java <c>getTimeFormat()</c> (Lombok).</summary>
        Public Function getTimeFormat() As String
            Return timeFormat
        End Function

        ''' <summary>Java <c>getTimeZone()</c> (Lombok).</summary>
        Public Function getTimeZone() As DateTimeZone
            Return timeZone
        End Function

        ''' <summary>Java <c>getLocale()</c> (Lombok).</summary>
        Public Function getLocale() As CultureInfo
            Return locale
        End Function

        ''' <summary>Java <c>getMinValidTime()</c> (Lombok).</summary>
        Public Function getMinValidTime() As Long?
            Return minValidTime
        End Function

        ''' <summary>Java <c>getMaxValidTime()</c> (Lombok).</summary>
        Public Function getMaxValidTime() As Long?
            Return maxValidTime
        End Function

        ''' <summary>Java <c>getFormatter()</c> (Lombok): <c>Nothing</c> when no time format was given.</summary>
        Public Function getFormatter() As DateTimeFormatter
            Return formatter
        End Function

        ''' <summary>Java <c>getFormatters()</c> (Lombok): the common formats, when no time format was given.</summary>
        Public Function getFormatters() As DateTimeFormatter()
            Return formatters
        End Function

        Public Overrides Function getNewColumnMetaData(ByVal newName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
            Return New TimeMetaData(newName, timeZone, minValidTime, maxValidTime)
        End Function

        Public Overrides Function map(ByVal columnWritable As WritableT) As WritableT
            Dim str As String = javaTrim(columnWritable.ToString())
            Dim tIdx As Integer = str.IndexOf("'T'", StringComparison.Ordinal)
            If tIdx >= 0 Then
                str = str.Substring(0, tIdx) & "T" & str.Substring(tIdx + 3)
            End If

            If formatter Is Nothing Then
                Dim result As Long = -1
                If Regex.IsMatch(str, "\.[0-9]+") Then
                    str = Regex.Replace(str, "\.[0-9]+", "")
                End If

                For Each f As DateTimeFormatter In formatters
                    Try
                        result = f.parseMillis(str)
                        Return New LongWritable(result)
                    Catch e As Exception
                        'Try the next format
                    End Try
                Next f

                If result < 0 Then
                    Throw New InvalidOperationException("Unable to parse date time " & str)
                End If
            Else
                Dim time As Long = formatter.parseMillis(str)
                Return New LongWritable(time)
            End If

            Throw New InvalidOperationException("Unable to parse date time " & str)
        End Function

        ''' <summary>Java <c>String.trim()</c>: removes leading and trailing characters up to U+0020.</summary>
        Private Shared Function javaTrim(ByVal s As String) As String
            Dim st As Integer = 0
            Dim len As Integer = s.Length
            While st < len AndAlso s(st) <= " "c
                st += 1
            End While
            While st < len AndAlso s(len - 1) <= " "c
                len -= 1
            End While
            Return s.Substring(st, len - st)
        End Function

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.Append("StringToTimeTransform(timeZone=").Append(javaToString(timeZone))
            If minValidTime.HasValue Then
                sb.Append(",minValidTime=").Append(minValidTime.Value)
            End If
            If maxValidTime.HasValue Then
                If minValidTime.HasValue Then
                    sb.Append(",")
                End If
                sb.Append("maxValidTime=").Append(maxValidTime.Value)
            End If
            sb.Append(")")
            Return sb.ToString()
        End Function

        ''' <summary>
        ''' Transform an object in to another object (as in Java: not supported, returns <c>Nothing</c>)
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Return Nothing
        End Function

        ''' <summary>Equal when the class, the column name, the format, the zone, the locale and the valid range are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim o As StringToTimeTransform = DirectCast(obj, StringToTimeTransform)
            Return String.Equals(timeFormat, o.timeFormat) AndAlso Objects.equals(timeZone, o.timeZone) AndAlso Objects.equals(locale, o.locale) AndAlso
                Nullable.Equals(minValidTime, o.minValidTime) AndAlso Nullable.Equals(maxValidTime, o.maxValidTime)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), timeFormat, timeZone, locale, minValidTime, maxValidTime)
        End Function

    End Class

End Namespace
