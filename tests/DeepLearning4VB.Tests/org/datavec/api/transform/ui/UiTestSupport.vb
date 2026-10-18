Imports System
Imports System.Text.RegularExpressions

Namespace org.datavec.api.transform.ui

    ''' <summary>Helpers of the HTML report tests.</summary>
    Friend Module UiTestSupport

        ''' <summary>Number of non-overlapping occurrences of <paramref name="needle"/> in <paramref name="text"/> (ordinal).</summary>
        Public Function countOccurrences(ByVal text As String, ByVal needle As String) As Integer
            Dim count As Integer = 0
            Dim index As Integer = text.IndexOf(needle, StringComparison.Ordinal)
            While index >= 0
                count += 1
                index = text.IndexOf(needle, index + needle.Length, StringComparison.Ordinal)
            End While
            Return count
        End Function

        ''' <summary>Asserts that a statistics table row with this label and (already HTML-encoded) value exists.</summary>
        Public Sub assertStat(ByVal html As String, ByVal label As String, ByVal encodedValue As String)
            Dim pattern As String = "<tr><td>" & Regex.Escape(label) & "</td><td[^>]*>" & Regex.Escape(encodedValue) & "</td></tr>"
            assertTrue(Regex.IsMatch(html, pattern), "statistic " & label & " = " & encodedValue)
        End Sub

        ''' <summary>Asserts that the page loads nothing from outside and runs no script.</summary>
        Public Sub assertSelfContained(ByVal html As String)
            assertTrue(html.StartsWith("<!DOCTYPE html>", StringComparison.Ordinal))
            assertFalse(html.IndexOf("<script", StringComparison.OrdinalIgnoreCase) >= 0, "no script")
            assertFalse(html.IndexOf("<link", StringComparison.OrdinalIgnoreCase) >= 0, "no stylesheet link")
            assertFalse(html.IndexOf("http://", StringComparison.OrdinalIgnoreCase) >= 0, "no http URL")
            assertFalse(html.IndexOf("https://", StringComparison.OrdinalIgnoreCase) >= 0, "no https URL")
            assertFalse(html.IndexOf("@import", StringComparison.OrdinalIgnoreCase) >= 0, "no CSS import")
            assertFalse(html.IndexOf("url(", StringComparison.OrdinalIgnoreCase) >= 0, "no CSS url()")
            assertTrue(html.Contains("@media (prefers-color-scheme: dark)"), "dark mode")
        End Sub

        ''' <summary>A new empty temporary directory.</summary>
        Public Function newTempDir() As String
            Dim dir As String = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "dl4vb-ui-" & Guid.NewGuid().ToString("N"))
            System.IO.Directory.CreateDirectory(dir)
            Return dir
        End Function

    End Module

End Namespace
