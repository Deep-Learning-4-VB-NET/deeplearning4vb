Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text

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

Namespace org.nd4j.common.io



	Public MustInherit Class StringUtils
		Private Const FOLDER_SEPARATOR As String = "/"
		Private Const WINDOWS_FOLDER_SEPARATOR As String = "\"
		Private Const TOP_PATH As String = ".."
		Private Const CURRENT_PATH As String = "."
		Private Const EXTENSION_SEPARATOR As Char = "."c

		Public Sub New()
		End Sub

		Public Shared Function isEmpty(ByVal str As Object) As Boolean
			Return str Is Nothing OrElse "".Equals(str)
		End Function

		Public Shared Function hasLength(ByVal str As CharSequence) As Boolean
			Return str IsNot Nothing AndAlso str.length() > 0
		End Function

		Public Shared Function hasLength(ByVal str As String) As Boolean
			Return hasLength(CType(str, CharSequence))
		End Function

		Public Shared Function hasText(ByVal str As CharSequence) As Boolean
			If Not hasLength(str) Then
				Return False
			Else
				Dim strLen As Integer = str.length()

				For i As Integer = 0 To strLen - 1
					If Not Char.IsWhiteSpace(str.charAt(i)) Then
						Return True
					End If
				Next i

				Return False
			End If
		End Function

		Public Shared Function hasText(ByVal str As String) As Boolean
			Return hasText(CType(str, CharSequence))
		End Function

		Public Shared Function containsWhitespace(ByVal str As CharSequence) As Boolean
			If Not hasLength(str) Then
				Return False
			Else
				Dim strLen As Integer = str.length()

				For i As Integer = 0 To strLen - 1
					If Char.IsWhiteSpace(str.charAt(i)) Then
						Return True
					End If
				Next i

				Return False
			End If
		End Function

		Public Shared Function containsWhitespace(ByVal str As String) As Boolean
			Return containsWhitespace(CType(str, CharSequence))
		End Function

		Public Shared Function trimWhitespace(ByVal str As String) As String
			If Not hasLength(str) Then
				Return str
			Else
				Dim sb As New StringBuilder(str)

				Do While sb.Length > 0 AndAlso Char.IsWhiteSpace(sb.Chars(0))
					sb.Remove(0, 1)
				Loop

				Do While sb.Length > 0 AndAlso Char.IsWhiteSpace(sb.Chars(sb.Length - 1))
					sb.Remove(sb.Length - 1, 1)
				Loop

				Return sb.ToString()
			End If
		End Function

		Public Shared Function trimAllWhitespace(ByVal str As String) As String
			If Not hasLength(str) Then
				Return str
			Else
				Dim sb As New StringBuilder(str)
				Dim index As Integer = 0

				Do While sb.Length > index
					If Char.IsWhiteSpace(sb.Chars(index)) Then
						sb.Remove(index, 1)
					Else
						index += 1
					End If
				Loop

				Return sb.ToString()
			End If
		End Function

		Public Shared Function trimLeadingWhitespace(ByVal str As String) As String
			If Not hasLength(str) Then
				Return str
			Else
				Dim sb As New StringBuilder(str)

				Do While sb.Length > 0 AndAlso Char.IsWhiteSpace(sb.Chars(0))
					sb.Remove(0, 1)
				Loop

				Return sb.ToString()
			End If
		End Function

		Public Shared Function trimTrailingWhitespace(ByVal str As String) As String
			If Not hasLength(str) Then
				Return str
			Else
				Dim sb As New StringBuilder(str)

				Do While sb.Length > 0 AndAlso Char.IsWhiteSpace(sb.Chars(sb.Length - 1))
					sb.Remove(sb.Length - 1, 1)
				Loop

				Return sb.ToString()
			End If
		End Function

		Public Shared Function trimLeadingCharacter(ByVal str As String, ByVal leadingCharacter As Char) As String
			If Not hasLength(str) Then
				Return str
			Else
				Dim sb As New StringBuilder(str)

				Do While sb.Length > 0 AndAlso sb.Chars(0) = leadingCharacter
					sb.Remove(0, 1)
				Loop

				Return sb.ToString()
			End If
		End Function

		Public Shared Function trimTrailingCharacter(ByVal str As String, ByVal trailingCharacter As Char) As String
			If Not hasLength(str) Then
				Return str
			Else
				Dim sb As New StringBuilder(str)

				Do While sb.Length > 0 AndAlso sb.Chars(sb.Length - 1) = trailingCharacter
					sb.Remove(sb.Length - 1, 1)
				Loop

				Return sb.ToString()
			End If
		End Function

		Public Shared Function startsWithIgnoreCase(ByVal str As String, ByVal prefix As String) As Boolean
			If str IsNot Nothing AndAlso prefix IsNot Nothing Then
				If str.StartsWith(prefix, StringComparison.Ordinal) Then
					Return True
				ElseIf str.Length < prefix.Length Then
					Return False
				Else
					Dim lcStr As String = str.Substring(0, prefix.Length).ToLower()
					Dim lcPrefix As String = prefix.ToLower()
					Return lcStr.Equals(lcPrefix)
				End If
			Else
				Return False
			End If
		End Function

		Public Shared Function endsWithIgnoreCase(ByVal str As String, ByVal suffix As String) As Boolean
			If str IsNot Nothing AndAlso suffix IsNot Nothing Then
				If str.EndsWith(suffix, StringComparison.Ordinal) Then
					Return True
				ElseIf str.Length < suffix.Length Then
					Return False
				Else
					Dim lcStr As String = str.Substring(str.Length - suffix.Length).ToLower()
					Dim lcSuffix As String = suffix.ToLower()
					Return lcStr.Equals(lcSuffix)
				End If
			Else
				Return False
			End If
		End Function

		Public Shared Function substringMatch(ByVal str As CharSequence, ByVal index As Integer, ByVal substring As CharSequence) As Boolean
			For j As Integer = 0 To substring.length() - 1
				Dim i As Integer = index + j
				If i >= str.length() OrElse str.charAt(i) <> substring.charAt(j) Then
					Return False
				End If
			Next j

			Return True
		End Function

		Public Shared Function countOccurrencesOf(ByVal str As String, ByVal [sub] As String) As Integer
			If str IsNot Nothing AndAlso [sub] IsNot Nothing AndAlso str.Length <> 0 AndAlso [sub].Length <> 0 Then
				Dim count As Integer = 0

				Dim idx As Integer
				Dim pos As Integer = 0
				idx = str.IndexOf([sub], pos, System.StringComparison.Ordinal)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: for (int pos = 0; (idx = str.indexOf(sub, pos)) != -1; pos = idx + sub.length())
				Do While idx <> -1
					count += 1
						idx = str.IndexOf([sub], pos, StringComparison.Ordinal)
					pos = idx + [sub].Length
				Loop

				Return count
			Else
				Return 0
			End If
		End Function

		Public Shared Function replace(ByVal inString As String, ByVal oldPattern As String, ByVal newPattern As String) As String
			If hasLength(inString) AndAlso hasLength(oldPattern) AndAlso newPattern IsNot Nothing Then
				Dim sb As New StringBuilder()
				Dim pos As Integer = 0
				Dim index As Integer = inString.IndexOf(oldPattern, StringComparison.Ordinal)

				Dim patLen As Integer = oldPattern.Length
				Do While index >= 0
					sb.Append(inString.Substring(pos, index - pos))
					sb.Append(newPattern)
					pos = index + patLen
					index = inString.IndexOf(oldPattern, pos, System.StringComparison.Ordinal)
				Loop

				sb.Append(inString.Substring(pos))
				Return sb.ToString()
			Else
				Return inString
			End If
		End Function

		Public Shared Function delete(ByVal inString As String, ByVal pattern As String) As String
			Return replace(inString, pattern, "")
		End Function

		Public Shared Function deleteAny(ByVal inString As String, ByVal charsToDelete As String) As String
			If hasLength(inString) AndAlso hasLength(charsToDelete) Then
				Dim sb As New StringBuilder()

				For i As Integer = 0 To inString.Length - 1
					Dim c As Char = inString.Chars(i)
					If charsToDelete.IndexOf(c) = -1 Then
						sb.Append(c)
					End If
				Next i

				Return sb.ToString()
			Else
				Return inString
			End If
		End Function

		Public Shared Function quote(ByVal str As String) As String
			Return If(str IsNot Nothing, "'" & str & "'", Nothing)
		End Function

		Public Shared Function quoteIfString(ByVal obj As Object) As Object
			Return If(TypeOf obj Is String, quote(DirectCast(obj, String)), obj)
		End Function

		Public Shared Function unqualify(ByVal qualifiedName As String) As String
			Return unqualify(qualifiedName, "."c)
		End Function

		Public Shared Function unqualify(ByVal qualifiedName As String, ByVal separator As Char) As String
			Return qualifiedName.Substring(qualifiedName.LastIndexOf(separator) + 1)
		End Function

		Public Shared Function capitalize(ByVal str As String) As String
			Return changeFirstCharacterCase(str, True)
		End Function

		Public Shared Function uncapitalize(ByVal str As String) As String
			Return changeFirstCharacterCase(str, False)
		End Function

		Private Shared Function changeFirstCharacterCase(ByVal str As String, ByVal capitalize As Boolean) As String
			If str IsNot Nothing AndAlso str.Length <> 0 Then
				Dim sb As New StringBuilder(str.Length)
				If capitalize Then
					sb.Append(Char.ToUpper(str.Chars(0)))
				Else
					sb.Append(Char.ToLower(str.Chars(0)))
				End If

				sb.Append(str.Substring(1))
				Return sb.ToString()
			Else
				Return str
			End If
		End Function

		Public Shared Function getFilename(ByVal path As String) As String
			If path Is Nothing Then
				Return Nothing
			Else
				Dim separatorIndex As Integer = path.LastIndexOf("/", StringComparison.Ordinal)
				Return If(separatorIndex <> -1, path.Substring(separatorIndex + 1), path)
			End If
		End Function

		Public Shared Function getFilenameExtension(ByVal path As String) As String
			If path Is Nothing Then
				Return Nothing
			Else
				Dim extIndex As Integer = path.LastIndexOf(46)
				If extIndex = -1 Then
					Return Nothing
				Else
					Dim folderIndex As Integer = path.LastIndexOf("/", StringComparison.Ordinal)
					Return If(folderIndex > extIndex, Nothing, path.Substring(extIndex + 1))
				End If
			End If
		End Function

		Public Shared Function stripFilenameExtension(ByVal path As String) As String
			If path Is Nothing Then
				Return Nothing
			Else
				Dim extIndex As Integer = path.LastIndexOf(46)
				If extIndex = -1 Then
					Return path
				Else
					Dim folderIndex As Integer = path.LastIndexOf("/", StringComparison.Ordinal)
					Return If(folderIndex > extIndex, path, path.Substring(0, extIndex))
				End If
			End If
		End Function

		Public Shared Function applyRelativePath(ByVal path As String, ByVal relativePath As String) As String
			Dim separatorIndex As Integer = path.LastIndexOf("/", StringComparison.Ordinal)
			If separatorIndex <> -1 Then
				Dim newPath As String = path.Substring(0, separatorIndex)
				If Not relativePath.StartsWith("/", StringComparison.Ordinal) Then
					newPath = newPath & "/"
				End If

				Return newPath & relativePath
			Else
				Return relativePath
			End If
		End Function

		Public Shared Function cleanPath(ByVal path As String) As String
			If path Is Nothing Then
				Return Nothing
			Else
				Dim pathToUse As String = replace(path, "\", "/")
				Dim prefixIndex As Integer = pathToUse.IndexOf(":", StringComparison.Ordinal)
				Dim prefix As String = ""
				If prefixIndex <> -1 Then
					prefix = pathToUse.Substring(0, prefixIndex + 1)
					pathToUse = pathToUse.Substring(prefixIndex + 1)
				End If

				If pathToUse.StartsWith("/", StringComparison.Ordinal) Then
					prefix = prefix & "/"
					pathToUse = pathToUse.Substring(1)
				End If

				Dim pathArray() As String = delimitedListToStringArray(pathToUse, "/")
				Dim pathElements As New LinkedList()
				Dim tops As Integer = 0

				Dim i As Integer
				For i = pathArray.Length - 1 To 0 Step -1
					Dim element As String = pathArray(i)
					If Not ".".Equals(element) Then
						If "..".Equals(element) Then
							tops += 1
						ElseIf tops > 0 Then
							tops -= 1
						Else
'JAVA TO VB CONVERTER TODO TASK: There is no .NET LinkedList equivalent to the 2-parameter Java 'add' method:
							pathElements.Add(0, element)
						End If
					End If
				Next i

				For i = 0 To tops - 1
'JAVA TO VB CONVERTER TODO TASK: There is no .NET LinkedList equivalent to the 2-parameter Java 'add' method:
					pathElements.Add(0, "..")
				Next i

				Return prefix & collectionToDelimitedString(pathElements, "/")
			End If
		End Function

		Public Shared Function pathEquals(ByVal path1 As String, ByVal path2 As String) As Boolean
			Return cleanPath(path1).Equals(cleanPath(path2))
		End Function

		Public Shared Function parseLocaleString(ByVal localeString As String) As Locale
			Dim parts() As String = tokenizeToStringArray(localeString, "_ ", False, False)
			Dim language As String = If(parts.Length > 0, parts(0), "")
			Dim country As String = If(parts.Length > 1, parts(1), "")
			validateLocalePart(language)
			validateLocalePart(country)
			Dim [variant] As String = ""
			If parts.Length >= 2 Then
				Dim endIndexOfCountryCode As Integer = localeString.LastIndexOf(country, StringComparison.Ordinal) + country.Length
				[variant] = trimLeadingWhitespace(localeString.Substring(endIndexOfCountryCode))
				If [variant].StartsWith("_", StringComparison.Ordinal) Then
					[variant] = trimLeadingCharacter([variant], "_"c)
				End If
			End If

			Return If(language.Length > 0, New Locale(language, country, [variant]), Nothing)
		End Function

		Private Shared Sub validateLocalePart(ByVal localePart As String)
			For i As Integer = 0 To localePart.Length - 1
				Dim ch As Char = localePart.Chars(i)
				If AscW(ch) <> 95 AndAlso AscW(ch) <> 32 AndAlso Not Char.IsLetterOrDigit(ch) Then
					Throw New System.ArgumentException("Locale part """ & localePart & """ contains invalid characters")
				End If
			Next i

		End Sub

		Public Shared Function toLanguageTag(ByVal locale As Locale) As String
			Return locale.getLanguage() + (If(hasText(locale.getCountry()), "-" & locale.getCountry(), ""))
		End Function

		Public Shared Function addStringToArray(ByVal array() As String, ByVal str As String) As String()
			If ObjectUtils.isEmpty(array) Then
				Return New String() {str}
			Else
				Dim newArr(array.Length) As String
				Array.Copy(array, 0, newArr, 0, array.Length)
				newArr(array.Length) = str
				Return newArr
			End If
		End Function

		Public Shared Function concatenateStringArrays(ByVal array1() As String, ByVal array2() As String) As String()
			If ObjectUtils.isEmpty(array1) Then
				Return array2
			ElseIf ObjectUtils.isEmpty(array2) Then
				Return array1
			Else
				Dim newArr((array1.Length + array2.Length) - 1) As String
				Array.Copy(array1, 0, newArr, 0, array1.Length)
				Array.Copy(array2, 0, newArr, array1.Length, array2.Length)
				Return newArr
			End If
		End Function

		Public Shared Function mergeStringArrays(ByVal array1() As String, ByVal array2() As String) As String()
			If ObjectUtils.isEmpty(array1) Then
				Return array2
			ElseIf ObjectUtils.isEmpty(array2) Then
				Return array1
			Else
				Dim result As New ArrayList()
				result.AddRange(New ArrayList From {array1})
				Dim arr$() As String = array2
				Dim len$ As Integer = array2.Length

				For i$ As Integer = 0 To len$ - 1
					Dim str As String = arr$(i$)
					If Not result.Contains(str) Then
						result.Add(str)
					End If
				Next i$

				Return toStringArray(result)
			End If
		End Function

		Public Shared Function sortStringArray(ByVal array() As String) As String()
			If ObjectUtils.isEmpty(array) Then
				Return New String(){}
			Else
				Array.Sort(array)
				Return array
			End If
		End Function

		Public Shared Function toStringArray(ByVal collection As ICollection(Of String)) As String()
			Return If(collection Is Nothing, Nothing, collection.ToArray())
		End Function

		Public Shared Function toStringArray(ByVal enumeration As IEnumerator(Of String)) As String()
			If enumeration Is Nothing Then
				Return Nothing
			Else
				Dim list As ArrayList = Collections.list(enumeration)
				Return CType(list.ToArray(GetType(String)), String())
			End If
		End Function

		Public Shared Function trimArrayElements(ByVal array() As String) As String()
			If ObjectUtils.isEmpty(array) Then
				Return New String(){}
			Else
				Dim result(array.Length - 1) As String

				For i As Integer = 0 To array.Length - 1
					Dim element As String = array(i)
					result(i) = If(element IsNot Nothing, element.Trim(), Nothing)
				Next i

				Return result
			End If
		End Function

		Public Shared Function removeDuplicateStrings(ByVal array() As String) As String()
			If ObjectUtils.isEmpty(array) Then
				Return array
			Else
				Dim set As New SortedSet()
				Dim arr$() As String = array
				Dim len$ As Integer = array.Length

				For i$ As Integer = 0 To len$ - 1
					Dim element As String = arr$(i$)
					set.Add(element)
				Next i$

				Return toStringArray(set)
			End If
		End Function

		Public Shared Function split(ByVal toSplit As String, ByVal delimiter As String) As String()
			If hasLength(toSplit) AndAlso hasLength(delimiter) Then
				Dim offset As Integer = toSplit.IndexOf(delimiter, StringComparison.Ordinal)
				If offset < 0 Then
					Return Nothing
				Else
					Dim beforeDelimiter As String = toSplit.Substring(0, offset)
					Dim afterDelimiter As String = toSplit.Substring(offset + delimiter.Length)
					Return New String() {beforeDelimiter, afterDelimiter}
				End If
			Else
				Return Nothing
			End If
		End Function

		Public Shared Function splitArrayElementsIntoProperties(ByVal array() As String, ByVal delimiter As String) As Properties
			Return splitArrayElementsIntoProperties(array, delimiter, Nothing)
		End Function

		Public Shared Function splitArrayElementsIntoProperties(ByVal array() As String, ByVal delimiter As String, ByVal charsToDelete As String) As Properties
			If ObjectUtils.isEmpty(array) Then
				Return Nothing
			Else
				Dim result As New Properties()
				Dim arr$() As String = array
				Dim len$ As Integer = array.Length

				For i$ As Integer = 0 To len$ - 1
					Dim element As String = arr$(i$)
					If charsToDelete IsNot Nothing Then
						element = deleteAny(element, charsToDelete)
					End If

					Dim splittedElement() As String = split(element, delimiter)
					If splittedElement IsNot Nothing Then
						result.setProperty(splittedElement(0).Trim(), splittedElement(1).Trim())
					End If
				Next i$

				Return result
			End If
		End Function

		Public Shared Function tokenizeToStringArray(ByVal str As String, ByVal delimiters As String) As String()
			Return tokenizeToStringArray(str, delimiters, True, True)
		End Function

		Public Shared Function tokenizeToStringArray(ByVal str As String, ByVal delimiters As String, ByVal trimTokens As Boolean, ByVal ignoreEmptyTokens As Boolean) As String()
			If str Is Nothing Then
				Return Nothing
			Else
				Dim st As New StringTokenizer(str, delimiters)
				Dim tokens As New ArrayList()

				Do While st.hasMoreTokens()
					Dim token As String = st.nextToken()
					If trimTokens Then
						token = token.Trim()
					End If

					If Not ignoreEmptyTokens OrElse token.Length > 0 Then
						tokens.Add(token)
					End If
				Loop

				Return toStringArray(tokens)
			End If
		End Function

		Public Shared Function delimitedListToStringArray(ByVal str As String, ByVal delimiter As String) As String()
			Return delimitedListToStringArray(str, delimiter, Nothing)
		End Function

		Public Shared Function delimitedListToStringArray(ByVal str As String, ByVal delimiter As String, ByVal charsToDelete As String) As String()
			If str Is Nothing Then
				Return New String(){}
			ElseIf delimiter Is Nothing Then
				Return New String() {str}
			Else
				Dim result As New ArrayList()
				Dim pos As Integer
				If "".Equals(delimiter) Then
					For pos = 0 To str.Length - 1
						result.Add(deleteAny(str.Substring(pos, 1), charsToDelete))
					Next pos
				Else
					Dim delPos As Integer
					pos = 0
					delPos = str.IndexOf(delimiter, pos, System.StringComparison.Ordinal)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: for (pos = 0; (delPos = str.indexOf(delimiter, pos)) != -1; pos = delPos + delimiter.length())
					Do While delPos <> -1
						result.Add(deleteAny(str.Substring(pos, delPos - pos), charsToDelete))
							delPos = str.IndexOf(delimiter, pos, StringComparison.Ordinal)
						pos = delPos + delimiter.Length
					Loop

					If str.Length > 0 AndAlso pos <= str.Length Then
						result.Add(deleteAny(str.Substring(pos), charsToDelete))
					End If
				End If

				Return toStringArray(result)
			End If
		End Function

		Public Shared Function commaDelimitedListToStringArray(ByVal str As String) As String()
			Return delimitedListToStringArray(str, ",")
		End Function

		Public Shared Function commaDelimitedListToSet(ByVal str As String) As ISet(Of String)
			Dim set As New SortedSet()
			Dim tokens() As String = commaDelimitedListToStringArray(str)
			Dim arr$() As String = tokens
			Dim len$ As Integer = tokens.Length

			For i$ As Integer = 0 To len$ - 1
				Dim token As String = arr$(i$)
				set.Add(token)
			Next i$

			Return set
		End Function

		Public Shared Function collectionToDelimitedString(Of T1)(ByVal coll As ICollection(Of T1), ByVal delim As String, ByVal prefix As String, ByVal suffix As String) As String
			If CollectionUtils.isEmpty(coll) Then
				Return ""
			Else
				Dim sb As New StringBuilder()
				Dim it As System.Collections.IEnumerator = coll.GetEnumerator()

				Do While it.MoveNext()
					sb.Append(prefix).Append(it.Current).Append(suffix)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					If it.hasNext() Then
						sb.Append(delim)
					End If
				Loop

				Return sb.ToString()
			End If
		End Function

		Public Shared Function collectionToDelimitedString(Of T1)(ByVal coll As ICollection(Of T1), ByVal delim As String) As String
			Return collectionToDelimitedString(coll, delim, "", "")
		End Function

		Public Shared Function collectionToCommaDelimitedString(Of T1)(ByVal coll As ICollection(Of T1)) As String
			Return collectionToDelimitedString(coll, ",")
		End Function

		Public Shared Function arrayToDelimitedString(ByVal arr() As Object, ByVal delim As String) As String
			If ObjectUtils.isEmpty(arr) Then
				Return ""
			ElseIf arr.Length = 1 Then
				Return ObjectUtils.nullSafeToString(arr(0))
			Else
				Dim sb As New StringBuilder()

				For i As Integer = 0 To arr.Length - 1
					If i > 0 Then
						sb.Append(delim)
					End If

					sb.Append(arr(i))
				Next i

				Return sb.ToString()
			End If
		End Function

		Public Shared Function arrayToCommaDelimitedString(ByVal arr() As Object) As String
			Return arrayToDelimitedString(arr, ",")
		End Function

	End Class

End Namespace