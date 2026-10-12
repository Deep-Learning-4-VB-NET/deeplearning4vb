Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Text

Namespace JavaCompat

    ''' <summary>
    ''' A string key/value store with the <c>java.util.Properties</c> API, including reading and
    ''' writing the Java <c>.properties</c> file format (comments, <c>key=value</c> /
    ''' <c>key:value</c> / <c>key value</c> separators, backslash escapes and line continuations).
    ''' </summary>
    <Serializable>
    Public Class Properties

        Private ReadOnly map As New Dictionary(Of String, String)(StringComparer.Ordinal)
        Private ReadOnly defaults As Properties

        Public Sub New()
        End Sub

        Public Sub New(ByVal defaults As Properties)
            Me.defaults = defaults
        End Sub

        ''' <summary>Returns the value for the key, consulting the defaults, or Nothing if absent.</summary>
        Public Function getProperty(ByVal key As String) As String
            If key Is Nothing Then Throw New ArgumentNullException(NameOf(key))
            Dim value As String = Nothing
            If map.TryGetValue(key, value) Then Return value
            If defaults IsNot Nothing Then Return defaults.getProperty(key)
            Return Nothing
        End Function

        ''' <summary>Returns the value for the key, or <paramref name="defaultValue"/> if absent.</summary>
        Public Function getProperty(ByVal key As String, ByVal defaultValue As String) As String
            Dim value As String = getProperty(key)
            Return If(value, defaultValue)
        End Function

        ''' <summary>Sets the value and returns the previous value (Nothing if there was none).</summary>
        Public Function setProperty(ByVal key As String, ByVal value As String) As String
            If key Is Nothing Then Throw New ArgumentNullException(NameOf(key))
            If value Is Nothing Then Throw New ArgumentNullException(NameOf(value))
            Dim previous As String = Nothing
            map.TryGetValue(key, previous)
            map(key) = value
            Return previous
        End Function

        ''' <summary>Alias of <see cref="setProperty"/> (java.util.Properties inherits put from Hashtable).</summary>
        Public Function put(ByVal key As String, ByVal value As String) As String
            Return setProperty(key, value)
        End Function

        Public Function remove(ByVal key As String) As String
            Dim previous As String = Nothing
            If map.TryGetValue(key, previous) Then
                map.Remove(key)
                Return previous
            End If
            Return Nothing
        End Function

        Public Function containsKey(ByVal key As String) As Boolean
            Return map.ContainsKey(key)
        End Function

        Public Sub clear()
            map.Clear()
        End Sub

        Public ReadOnly Property Count As Integer
            Get
                Return map.Count
            End Get
        End Property

        Public Function size() As Integer
            Return map.Count
        End Function

        Public Function isEmpty() As Boolean
            Return map.Count = 0
        End Function

        ''' <summary>All keys, including those only present in the defaults.</summary>
        Public Function stringPropertyNames() As ISet(Of String)
            Dim names As New LinkedHashSet(Of String)()
            If defaults IsNot Nothing Then
                For Each n In defaults.stringPropertyNames()
                    names.Add(n)
                Next
            End If
            For Each k In map.Keys
                names.Add(k)
            Next
            Return names
        End Function

        Public Sub putAll(ByVal other As Properties)
            If other Is Nothing Then Return
            For Each kv In other.map
                map(kv.Key) = kv.Value
            Next
        End Sub

        Public Sub putAll(ByVal other As IDictionary(Of String, String))
            If other Is Nothing Then Return
            For Each kv In other
                map(kv.Key) = kv.Value
            Next
        End Sub

        ''' <summary>Reads properties in the Java <c>.properties</c> format. Java reads streams as ISO-8859-1.</summary>
        Public Sub load(ByVal stream As Stream)
            load(New StreamReader(stream, Encoding.GetEncoding("ISO-8859-1")))
        End Sub

        ''' <summary>Reads properties in the Java <c>.properties</c> format.</summary>
        Public Sub load(ByVal reader As TextReader)
            If reader Is Nothing Then Throw New ArgumentNullException(NameOf(reader))
            Dim logical As New StringBuilder()
            Dim line As String = reader.ReadLine()
            While line IsNot Nothing
                Dim trimmed As String = line.TrimStart(" "c, ControlChars.Tab, ControlChars.FormFeed)
                If logical.Length = 0 AndAlso (trimmed.Length = 0 OrElse trimmed(0) = "#"c OrElse trimmed(0) = "!"c) Then
                    line = reader.ReadLine()
                    Continue While
                End If
                If logical.Length > 0 Then
                    ' continuation lines have their leading whitespace dropped
                    line = trimmed
                End If
                If EndsWithOddBackslashes(line) Then
                    logical.Append(line, 0, line.Length - 1)
                    line = reader.ReadLine()
                    If line Is Nothing Then
                        ParseLogicalLine(logical.ToString())
                        logical.Length = 0
                    End If
                    Continue While
                End If
                logical.Append(line)
                ParseLogicalLine(logical.ToString())
                logical.Length = 0
                line = reader.ReadLine()
            End While
        End Sub

        Private Shared Function EndsWithOddBackslashes(ByVal line As String) As Boolean
            Dim count As Integer = 0
            Dim i As Integer = line.Length - 1
            While i >= 0 AndAlso line(i) = "\"c
                count += 1
                i -= 1
            End While
            Return (count Mod 2) = 1
        End Function

        Private Sub ParseLogicalLine(ByVal line As String)
            Dim s As String = line.TrimStart(" "c, ControlChars.Tab, ControlChars.FormFeed)
            If s.Length = 0 OrElse s(0) = "#"c OrElse s(0) = "!"c Then Return
            ' find the end of the key: first unescaped '=', ':' or whitespace
            Dim keyEnd As Integer = 0
            Dim escaped As Boolean = False
            While keyEnd < s.Length
                Dim c As Char = s(keyEnd)
                If escaped Then
                    escaped = False
                ElseIf c = "\"c Then
                    escaped = True
                ElseIf c = "="c OrElse c = ":"c OrElse c = " "c OrElse c = ControlChars.Tab OrElse c = ControlChars.FormFeed Then
                    Exit While
                End If
                keyEnd += 1
            End While
            Dim valueStart As Integer = keyEnd
            Dim separatorSeen As Boolean = False
            While valueStart < s.Length
                Dim c As Char = s(valueStart)
                If c = " "c OrElse c = ControlChars.Tab OrElse c = ControlChars.FormFeed Then
                    valueStart += 1
                ElseIf (c = "="c OrElse c = ":"c) AndAlso Not separatorSeen Then
                    separatorSeen = True
                    valueStart += 1
                Else
                    Exit While
                End If
            End While
            Dim key As String = Unescape(s.Substring(0, keyEnd))
            Dim value As String = Unescape(s.Substring(valueStart))
            map(key) = value
        End Sub

        Private Shared Function Unescape(ByVal s As String) As String
            If s.IndexOf("\"c) < 0 Then Return s
            Dim sb As New StringBuilder(s.Length)
            Dim i As Integer = 0
            While i < s.Length
                Dim c As Char = s(i)
                If c = "\"c AndAlso i + 1 < s.Length Then
                    i += 1
                    Dim e As Char = s(i)
                    Select Case e
                        Case "t"c : sb.Append(ControlChars.Tab)
                        Case "n"c : sb.Append(ControlChars.Lf)
                        Case "r"c : sb.Append(ControlChars.Cr)
                        Case "f"c : sb.Append(ControlChars.FormFeed)
                        Case "u"c
                            If i + 4 < s.Length + 0 AndAlso i + 4 <= s.Length - 1 + 1 Then
                                Dim hex As String = s.Substring(i + 1, Math.Min(4, s.Length - i - 1))
                                Dim code As Integer
                                If hex.Length = 4 AndAlso Integer.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, code) Then
                                    sb.Append(ChrW(code))
                                    i += 4
                                Else
                                    Throw New FormatException("Malformed \uxxxx encoding.")
                                End If
                            Else
                                Throw New FormatException("Malformed \uxxxx encoding.")
                            End If
                        Case Else : sb.Append(e)
                    End Select
                Else
                    sb.Append(c)
                End If
                i += 1
            End While
            Return sb.ToString()
        End Function

        ''' <summary>Writes the properties in the Java <c>.properties</c> format.</summary>
        Public Sub store(ByVal writer As TextWriter, ByVal comments As String)
            If writer Is Nothing Then Throw New ArgumentNullException(NameOf(writer))
            If comments IsNot Nothing Then
                writer.Write("#" & comments & Environment.NewLine)
            End If
            writer.Write("#" & DateTime.Now.ToString("ddd MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture) & Environment.NewLine)
            For Each kv In map
                writer.Write(Escape(kv.Key, True) & "=" & Escape(kv.Value, False) & Environment.NewLine)
            Next
            writer.Flush()
        End Sub

        Public Sub store(ByVal stream As Stream, ByVal comments As String)
            Dim writer As New StreamWriter(stream, Encoding.GetEncoding("ISO-8859-1"))
            store(writer, comments)
        End Sub

        Private Shared Function Escape(ByVal s As String, ByVal isKey As Boolean) As String
            Dim sb As New StringBuilder(s.Length * 2)
            For i As Integer = 0 To s.Length - 1
                Dim c As Char = s(i)
                Select Case c
                    Case "\"c : sb.Append("\\")
                    Case ControlChars.Tab : sb.Append("\t")
                    Case ControlChars.Lf : sb.Append("\n")
                    Case ControlChars.Cr : sb.Append("\r")
                    Case ControlChars.FormFeed : sb.Append("\f")
                    Case "="c, ":"c, "#"c, "!"c : sb.Append("\"c).Append(c)
                    Case " "c
                        If isKey OrElse i = 0 Then
                            sb.Append("\ ")
                        Else
                            sb.Append(c)
                        End If
                    Case Else
                        If AscW(c) < 32 OrElse AscW(c) > 126 Then
                            sb.Append("\u").Append(AscW(c).ToString("x4", CultureInfo.InvariantCulture))
                        Else
                            sb.Append(c)
                        End If
                End Select
            Next
            Return sb.ToString()
        End Function

        Public Function toDictionary() As IDictionary(Of String, String)
            Return New Dictionary(Of String, String)(map, StringComparer.Ordinal)
        End Function

        Public Overrides Function ToString() As String
            Dim parts As New List(Of String)(map.Count)
            For Each kv In map
                parts.Add(kv.Key & "=" & kv.Value)
            Next
            Return "{" & String.Join(", ", parts) & "}"
        End Function
    End Class

End Namespace
