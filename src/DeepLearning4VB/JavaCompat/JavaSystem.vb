Imports System
Imports System.Collections.Concurrent
Imports System.Diagnostics
Imports System.IO

''' <summary>
''' Equivalents of the <c>java.lang.System</c> members used by the ported code.
''' "System properties" are an in-process key/value store (Java's <c>-D</c> settings);
''' a few well-known Java property names are mapped onto their .NET equivalents.
''' </summary>
Friend Module JavaSystem

    Private ReadOnly properties As New ConcurrentDictionary(Of String, String)(StringComparer.Ordinal)

    ''' <summary>Returns the property value, or Nothing if it is not set.</summary>
    Public Function getProperty(ByVal name As String) As String
        If name Is Nothing Then Throw New ArgumentNullException(NameOf(name))
        Dim value As String = Nothing
        If properties.TryGetValue(name, value) Then Return value
        Select Case name
            Case "java.io.tmpdir" : Return Path.GetTempPath()
            Case "user.home" : Return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            Case "user.dir" : Return Directory.GetCurrentDirectory()
            Case "user.name" : Return Environment.UserName
            Case "line.separator" : Return Environment.NewLine
            Case "file.separator" : Return Path.DirectorySeparatorChar.ToString()
            Case "path.separator" : Return Path.PathSeparator.ToString()
            Case "os.name" : Return Environment.OSVersion.VersionString
            Case "os.arch" : Return If(Environment.Is64BitProcess, "amd64", "x86")
            Case "java.version" : Return Environment.Version.ToString()
        End Select
        Return Nothing
    End Function

    ''' <summary>Returns the property value, or <paramref name="defaultValue"/> if it is not set.</summary>
    Public Function getProperty(ByVal name As String, ByVal defaultValue As String) As String
        Dim value As String = getProperty(name)
        Return If(value, defaultValue)
    End Function

    ''' <summary>Sets a property and returns the previous value (Nothing if it was unset).</summary>
    Public Function setProperty(ByVal name As String, ByVal value As String) As String
        If name Is Nothing Then Throw New ArgumentNullException(NameOf(name))
        If value Is Nothing Then Throw New ArgumentNullException(NameOf(value))
        Dim previous As String = getProperty(name)
        properties(name) = value
        Return previous
    End Function

    ''' <summary>Removes a property and returns its previous value (Nothing if it was unset).</summary>
    Public Function clearProperty(ByVal name As String) As String
        Dim previous As String = Nothing
        properties.TryRemove(name, previous)
        Return previous
    End Function

    Public Function getenv(ByVal name As String) As String
        Return Environment.GetEnvironmentVariable(name)
    End Function

    Public Function currentTimeMillis() As Long
        Return DateTimeHelper.CurrentUnixTimeMillis()
    End Function

    ''' <summary>A monotonic high-resolution timestamp in nanoseconds (relative origin, like Java).</summary>
    Public Function nanoTime() As Long
        Return CLng(Stopwatch.GetTimestamp() * (1000000000.0 / Stopwatch.Frequency))
    End Function

    Public Function lineSeparator() As String
        Return Environment.NewLine
    End Function

End Module
