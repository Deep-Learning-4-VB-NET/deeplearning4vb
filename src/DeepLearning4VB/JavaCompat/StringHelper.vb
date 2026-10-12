Imports System
Imports System.Text
Imports System.Runtime.CompilerServices

Friend Module StringHelper
	<Extension>
	Public Function SubstringSpecial(ByVal self As String, ByVal start As Integer, ByVal _end As Integer) As String
		Return self.Substring(start, _end - start)
	End Function

	<Extension>
	Public Function StartsWith(ByVal self As String, ByVal prefix As String, ByVal toffset As Integer) As Boolean
		Return self.IndexOf(prefix, toffset, StringComparison.Ordinal) = toffset
	End Function

	<Extension>
	Public Function Split(ByVal self As String, ByVal regexDelimiter As String, ByVal trimTrailingEmptyStrings As Boolean) As String()
		Dim splitArray() As String = RegularExpressions.Regex.Split(self, regexDelimiter)

		If trimTrailingEmptyStrings Then
			If splitArray.Length > 1 Then
				For i As Integer = splitArray.Length To 1 Step -1
					If splitArray(i - 1).Length > 0 Then
						If i < splitArray.Length Then
							Array.Resize(splitArray, i)
						End If

						Exit For
					End If
				Next i
			End If
		End If

		Return splitArray
	End Function

	Public Function NewString(ByVal bytes() As SByte) As String
		Return NewString(bytes, 0, bytes.Length)
	End Function
	Public Function NewString(ByVal bytes() As SByte, ByVal index As Integer, ByVal count As Integer) As String
		Return Encoding.UTF8.GetString(CType(CObj(bytes), Byte()), index, count)
	End Function
	Public Function NewString(ByVal bytes() As SByte, ByVal encoding As String) As String
		Return NewString(bytes, 0, bytes.Length, encoding)
	End Function
	Public Function NewString(ByVal bytes() As SByte, ByVal index As Integer, ByVal count As Integer, ByVal encoding As String) As String
		Return NewString(bytes, index, count, Text.Encoding.GetEncoding(encoding))
	End Function
	Public Function NewString(ByVal bytes() As SByte, ByVal encoding As Encoding) As String
		Return NewString(bytes, 0, bytes.Length, encoding)
	End Function
	Public Function NewString(ByVal bytes() As SByte, ByVal index As Integer, ByVal count As Integer, ByVal encoding As Encoding) As String
		Return encoding.GetString(DirectCast(DirectCast(bytes, Object), Byte()), index, count)
	End Function

	<Extension>
	Public Function GetBytes(ByVal self As String) As SByte()
		Return GetSBytesForEncoding(Encoding.UTF8, self)
	End Function
	<Extension> _
	Public Function GetBytes(ByVal self As String, ByVal encoding As Encoding) As SByte()
		Return GetSBytesForEncoding(encoding, self)
	End Function
	<Extension> _
	Public Function GetBytes(ByVal self As String, ByVal encoding As String) As SByte()
		Return GetSBytesForEncoding(Text.Encoding.GetEncoding(encoding), self)
	End Function
	Private Function GetSBytesForEncoding(ByVal encoding As Encoding, ByVal s As String) As SByte()
		Dim sbytes(encoding.GetByteCount(s) - 1) As SByte
		encoding.GetBytes(s, 0, s.Length, CType(CObj(sbytes), Byte()), 0)
		Return sbytes
	End Function
End Module