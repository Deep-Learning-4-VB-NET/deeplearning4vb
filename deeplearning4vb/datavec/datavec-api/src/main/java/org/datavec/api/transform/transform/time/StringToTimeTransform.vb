Imports System
Imports System.Collections.Generic
Imports System.Text
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports TimeMetaData = org.datavec.api.transform.metadata.TimeMetaData
Imports BaseColumnTransform = org.datavec.api.transform.transform.BaseColumnTransform
Imports LongWritable = org.datavec.api.writable.LongWritable
Imports Writable = org.datavec.api.writable.Writable
Imports DateTimeZone = org.joda.time.DateTimeZone
Imports DateTimeFormat = org.joda.time.format.DateTimeFormat
Imports DateTimeFormatter = org.joda.time.format.DateTimeFormatter
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.datavec.api.transform.transform.time


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(exclude = { "formatter", "formatters" }) @JsonIgnoreProperties({ "formatters", "formatter" }) public class StringToTimeTransform extends org.datavec.api.transform.transform.BaseColumnTransform
	<Serializable>
	Public Class StringToTimeTransform
		Inherits BaseColumnTransform

		Private ReadOnly timeFormat As String
		Private ReadOnly timeZone As DateTimeZone
		Private ReadOnly locale As Locale
		Private ReadOnly minValidTime As Long?
		Private ReadOnly maxValidTime As Long?
		' formats from:
		' http://www.java2s.com/Tutorials/Java/Data_Type_How_to/Legacy_Date_Format/Guess_the_format_pattern_based_on_date_value.htm
		' 2017-09-21T17:06:29.064687
		' 12/1/2010 11:21
		Private Shared ReadOnly formats() As String = { "YYYY-MM-dd'T'HH:mm:ss", "YYYY-MM-dd", "YYYY-MM-dd'T'HH:mm:ss'Z'", "YYYY-MM-dd'T'HH:mm:ssZ", "YYYY-MM-dd'T'HH:mm:ss.SSS'Z'", "YYYY-MM-dd'T'HH:mm:ss.SSSZ", "YYYY-MM-dd HH:mm:ss", "MM/dd/YYYY HH:mm:ss", "MM/dd/YYYY'T'HH:mm:ss.SSS'Z'", "MM/dd/YYYY'T'HH:mm:ss.SSSZ", "MM/dd/YYYY'T'HH:mm:ss.SSS", "MM/dd/YYYY'T'HH:mm:ssZ", "MM/dd/YYYY'T'HH:mm:ss", "YYYY:MM:dd HH:mm:ss", "YYYYMMdd", "YYYY-MM-dd HH:mm:ss", "MM/dd/YYYY HH:mm"}
		<NonSerialized>
		Private formatters() As DateTimeFormatter

		<NonSerialized>
		Private formatter As DateTimeFormatter

		''' <summary>
		''' Instantiate this without a time format specified. If this constructor is
		''' used, this transform will be allowed to handle several common transforms as
		''' defined in the static formats array.
		''' 
		''' </summary>
		''' <param name="columnName"> Name of the String column </param>
		''' <param name="timeZone">   Timezone for time parsing </param>
		Public Sub New(ByVal columnName As String, ByVal timeZone As TimeZone)
			Me.New(columnName, Nothing, timeZone, Nothing, Nothing, Nothing)
		End Sub

		''' <param name="columnName"> Name of the String column </param>
		''' <param name="timeZone">   Timezone for time parsing </param>
		''' <param name="locale">     Locale for i18n </param>
		Public Sub New(ByVal columnName As String, ByVal timeZone As TimeZone, ByVal locale As Locale)
			Me.New(columnName, Nothing, timeZone, locale, Nothing, Nothing)
		End Sub

		''' <param name="columnName"> Name of the String column </param>
		''' <param name="timeFormat"> Time format, as per <a href=
		'''                   "http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html">http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html</a> </param>
		''' <param name="timeZone">   Timezone for time parsing </param>
		Public Sub New(ByVal columnName As String, ByVal timeFormat As String, ByVal timeZone As TimeZone)
			Me.New(columnName, timeFormat, timeZone, Nothing, Nothing, Nothing)
		End Sub

		''' <param name="columnName"> Name of the String column </param>
		''' <param name="timeFormat"> Time format, as per <a href=
		'''                   "http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html">http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html</a> </param>
		''' <param name="timeZone">   Timezone for time parsing </param>
		''' <param name="locale">     Locale for i18n </param>
		Public Sub New(ByVal columnName As String, ByVal timeFormat As String, ByVal timeZone As TimeZone, ByVal locale As Locale)
			Me.New(columnName, timeFormat, timeZone, locale, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Instantiate this without a time format specified. If this constructor is
		''' used, this transform will be allowed to handle several common transforms as
		''' defined in the static formats array.
		''' 
		''' </summary>
		''' <param name="columnName"> Name of the String column </param>
		''' <param name="timeZone">   Timezone for time parsing </param>
		''' <param name="locale">     Locale for i18n </param>
		Public Sub New(ByVal columnName As String, ByVal timeZone As DateTimeZone, ByVal locale As Locale)
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
		Public Sub New(ByVal columnName As String, ByVal timeFormat As String, ByVal timeZone As DateTimeZone, ByVal locale As Locale)
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
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public StringToTimeTransform(@JsonProperty("columnName") String columnName, @JsonProperty("timeFormat") String timeFormat, @JsonProperty("timeZone") java.util.TimeZone timeZone, @JsonProperty("locale") java.util.Locale locale, @JsonProperty("minValidTime") System.Nullable<Long> minValidTime, @JsonProperty("maxValidTime") System.Nullable<Long> maxValidTime)
		Public Sub New(ByVal columnName As String, ByVal timeFormat As String, ByVal timeZone As TimeZone, ByVal locale As Locale, ByVal minValidTime As Long?, ByVal maxValidTime As Long?)
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
		Public Sub New(ByVal columnName As String, ByVal timeFormat As String, ByVal timeZone As DateTimeZone, ByVal locale As Locale, ByVal minValidTime As Long?, ByVal maxValidTime As Long?)
			MyBase.New(columnName)
			Me.timeFormat = timeFormat
			Me.timeZone = timeZone
			Me.locale = locale
			Me.minValidTime = minValidTime
			Me.maxValidTime = maxValidTime
			If timeFormat IsNot Nothing Then
				If locale IsNot Nothing Then
					Me.formatter = DateTimeFormat.forPattern(timeFormat).withZone(timeZone).withLocale(locale)
				Else
					Me.formatter = DateTimeFormat.forPattern(timeFormat).withZone(timeZone)
				End If
			Else
				Dim dateFormatList As IList(Of DateTimeFormatter) = New List(Of DateTimeFormatter)()
				formatters = New DateTimeFormatter(formats.Length - 1){}
				For i As Integer = 0 To formatters.Length - 1
					If locale IsNot Nothing Then
						dateFormatList.Add(DateTimeFormat.forPattern(formats(i)).withZone(timeZone).withLocale(locale))
					Else
						dateFormatList.Add(DateTimeFormat.forPattern(formats(i)).withZone(timeZone))
					End If
				Next i
				formatters = CType(dateFormatList, List(Of DateTimeFormatter)).ToArray()
			End If
		End Sub

		Public Overrides Function getNewColumnMetaData(ByVal newName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
			Return New TimeMetaData(newName, timeZone, minValidTime, maxValidTime)
		End Function

		Public Overrides Function map(ByVal columnWritable As Writable) As Writable
			Dim str As String = columnWritable.ToString().Trim()
			If str.Contains("'T'") Then
				str = str.replaceFirst("'T'", "T")
			End If

			If formatter Is Nothing Then
				Dim result As Long = -1
				If Pattern.compile("\.[0-9]+").matcher(str).find() Then
					str = str.replaceAll("\.[0-9]+", "")
				End If

				For Each formatter As DateTimeFormatter In formatters
					Try
						result = formatter.parseMillis(str)
						Return New LongWritable(result)
					Catch e As Exception

					End Try

				Next formatter

				If result < 0 Then
					Throw New System.InvalidOperationException("Unable to parse date time " & str)
				End If
			Else
				Dim time As Long = formatter.parseMillis(str)
				Return New LongWritable(time)
			End If

			Throw New System.InvalidOperationException("Unable to parse date time " & str)

		End Function

		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			sb.Append("StringToTimeTransform(timeZone=").Append(timeZone)
			If minValidTime IsNot Nothing Then
				sb.Append(",minValidTime=").Append(minValidTime)
			End If
			If maxValidTime IsNot Nothing Then
				If minValidTime IsNot Nothing Then
					sb.Append(",")
				End If
				sb.Append("maxValidTime=").Append(maxValidTime)
			End If
			sb.Append(")")
			Return sb.ToString()
		End Function

		' Custom serialization methods, because Joda Time doesn't allow
		' DateTimeFormatter objects to be serialized :(
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream out) throws java.io.IOException
		Private Sub writeObject(ByVal [out] As ObjectOutputStream)
			[out].defaultWriteObject()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws IOException, ClassNotFoundException
		Private Sub readObject(ByVal [in] As ObjectInputStream)
			[in].defaultReadObject()
			If timeFormat IsNot Nothing Then
				If locale IsNot Nothing Then
					formatter = DateTimeFormat.forPattern(timeFormat).withZone(timeZone).withLocale(locale)
				Else
					formatter = DateTimeFormat.forPattern(timeFormat).withZone(timeZone)
				End If
			Else
				Dim dateFormatList As IList(Of DateTimeFormatter) = New List(Of DateTimeFormatter)()
				formatters = New DateTimeFormatter(formats.Length - 1){}
				For i As Integer = 0 To formatters.Length - 1
					dateFormatList.Add(DateTimeFormat.forPattern(formats(i)).withZone(timeZone))
				Next i

				formatters = CType(dateFormatList, List(Of DateTimeFormatter)).ToArray()
			End If
		End Sub

		''' <summary>
		''' Transform an object in to another object
		''' </summary>
		''' <param name="input"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Public Overridable Overloads Function map(ByVal input As Object) As Object
			Return Nothing
		End Function
	End Class

End Namespace