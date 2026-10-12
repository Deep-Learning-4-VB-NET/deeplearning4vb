Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Text
Imports DeepLearning4VB.JavaCompat

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

    ''' <summary>
    ''' String utilities, a copy of Spring's <c>org.springframework.util.StringUtils</c>.
    ''' <para>Differences from the Java original: the <c>CharSequence</c> overloads are merged into the
    ''' <c>String</c> ones; <c>java.util.Locale</c> is <see cref="CultureInfo"/> (the locale variant has
    ''' no .NET equivalent and is dropped); <c>java.util.Enumeration</c> is <see cref="IEnumerator(Of T)"/>;
    ''' <c>java.util.Properties</c> is <see cref="Properties"/>; Java's <c>Collection&lt;?&gt;</c> is
    ''' <see cref="IEnumerable"/>; sorting and the sorted sets use ordinal string order like Java's
    ''' <c>String.compareTo</c>; case conversions are culture invariant.</para>
    ''' </summary>
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

        Public Shared Function hasLength(ByVal str As String) As Boolean
            Return str IsNot Nothing AndAlso str.Length > 0
        End Function

        Public Shared Function hasText(ByVal str As String) As Boolean
            If Not hasLength(str) Then
                Return False
            End If
            For i As Integer = 0 To str.Length - 1
                If Not Char.IsWhiteSpace(str.Chars(i)) Then
                    Return True
                End If
            Next i
            Return False
        End Function

        Public Shared Function containsWhitespace(ByVal str As String) As Boolean
            If Not hasLength(str) Then
                Return False
            End If
            For i As Integer = 0 To str.Length - 1
                If Char.IsWhiteSpace(str.Chars(i)) Then
                    Return True
                End If
            Next i
            Return False
        End Function

        Public Shared Function trimWhitespace(ByVal str As String) As String
            If Not hasLength(str) Then
                Return str
            End If
            Dim sb As New StringBuilder(str)
            Do While sb.Length > 0 AndAlso Char.IsWhiteSpace(sb.Chars(0))
                sb.Remove(0, 1)
            Loop
            Do While sb.Length > 0 AndAlso Char.IsWhiteSpace(sb.Chars(sb.Length - 1))
                sb.Remove(sb.Length - 1, 1)
            Loop
            Return sb.ToString()
        End Function

        Public Shared Function trimAllWhitespace(ByVal str As String) As String
            If Not hasLength(str) Then
                Return str
            End If
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
        End Function

        Public Shared Function trimLeadingWhitespace(ByVal str As String) As String
            If Not hasLength(str) Then
                Return str
            End If
            Dim sb As New StringBuilder(str)
            Do While sb.Length > 0 AndAlso Char.IsWhiteSpace(sb.Chars(0))
                sb.Remove(0, 1)
            Loop
            Return sb.ToString()
        End Function

        Public Shared Function trimTrailingWhitespace(ByVal str As String) As String
            If Not hasLength(str) Then
                Return str
            End If
            Dim sb As New StringBuilder(str)
            Do While sb.Length > 0 AndAlso Char.IsWhiteSpace(sb.Chars(sb.Length - 1))
                sb.Remove(sb.Length - 1, 1)
            Loop
            Return sb.ToString()
        End Function

        Public Shared Function trimLeadingCharacter(ByVal str As String, ByVal leadingCharacter As Char) As String
            If Not hasLength(str) Then
                Return str
            End If
            Dim sb As New StringBuilder(str)
            Do While sb.Length > 0 AndAlso sb.Chars(0) = leadingCharacter
                sb.Remove(0, 1)
            Loop
            Return sb.ToString()
        End Function

        Public Shared Function trimTrailingCharacter(ByVal str As String, ByVal trailingCharacter As Char) As String
            If Not hasLength(str) Then
                Return str
            End If
            Dim sb As New StringBuilder(str)
            Do While sb.Length > 0 AndAlso sb.Chars(sb.Length - 1) = trailingCharacter
                sb.Remove(sb.Length - 1, 1)
            Loop
            Return sb.ToString()
        End Function

        Public Shared Function startsWithIgnoreCase(ByVal str As String, ByVal prefix As String) As Boolean
            If str Is Nothing OrElse prefix Is Nothing Then
                Return False
            End If
            If str.StartsWith(prefix, StringComparison.Ordinal) Then
                Return True
            End If
            If str.Length < prefix.Length Then
                Return False
            End If
            Dim lcStr As String = str.Substring(0, prefix.Length).ToLowerInvariant()
            Dim lcPrefix As String = prefix.ToLowerInvariant()
            Return lcStr.Equals(lcPrefix)
        End Function

        Public Shared Function endsWithIgnoreCase(ByVal str As String, ByVal suffix As String) As Boolean
            If str Is Nothing OrElse suffix Is Nothing Then
                Return False
            End If
            If str.EndsWith(suffix, StringComparison.Ordinal) Then
                Return True
            End If
            If str.Length < suffix.Length Then
                Return False
            End If
            Dim lcStr As String = str.Substring(str.Length - suffix.Length).ToLowerInvariant()
            Dim lcSuffix As String = suffix.ToLowerInvariant()
            Return lcStr.Equals(lcSuffix)
        End Function

        Public Shared Function substringMatch(ByVal str As String, ByVal index As Integer, ByVal substring As String) As Boolean
            For j As Integer = 0 To substring.Length - 1
                Dim i As Integer = index + j
                If i >= str.Length OrElse str.Chars(i) <> substring.Chars(j) Then
                    Return False
                End If
            Next j
            Return True
        End Function

        Public Shared Function countOccurrencesOf(ByVal str As String, ByVal [sub] As String) As Integer
            If str Is Nothing OrElse [sub] Is Nothing OrElse str.Length = 0 OrElse [sub].Length = 0 Then
                Return 0
            End If
            Dim count As Integer = 0
            Dim pos As Integer = 0
            Dim idx As Integer = str.IndexOf([sub], pos, StringComparison.Ordinal)
            Do While idx <> -1
                count += 1
                pos = idx + [sub].Length
                idx = str.IndexOf([sub], pos, StringComparison.Ordinal)
            Loop
            Return count
        End Function

        Public Shared Function replace(ByVal inString As String, ByVal oldPattern As String, ByVal newPattern As String) As String
            If Not hasLength(inString) OrElse Not hasLength(oldPattern) OrElse newPattern Is Nothing Then
                Return inString
            End If
            Dim sb As New StringBuilder()
            Dim pos As Integer = 0
            Dim index As Integer = inString.IndexOf(oldPattern, StringComparison.Ordinal)
            Dim patLen As Integer = oldPattern.Length
            Do While index >= 0
                sb.Append(inString, pos, index - pos)
                sb.Append(newPattern)
                pos = index + patLen
                index = inString.IndexOf(oldPattern, pos, StringComparison.Ordinal)
            Loop
            sb.Append(inString.Substring(pos))
            Return sb.ToString()
        End Function

        Public Shared Function delete(ByVal inString As String, ByVal pattern As String) As String
            Return replace(inString, pattern, "")
        End Function

        Public Shared Function deleteAny(ByVal inString As String, ByVal charsToDelete As String) As String
            If Not hasLength(inString) OrElse Not hasLength(charsToDelete) Then
                Return inString
            End If
            Dim sb As New StringBuilder()
            For i As Integer = 0 To inString.Length - 1
                Dim c As Char = inString.Chars(i)
                If charsToDelete.IndexOf(c) = -1 Then
                    sb.Append(c)
                End If
            Next i
            Return sb.ToString()
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

        Private Shared Function changeFirstCharacterCase(ByVal str As String, ByVal upperCase As Boolean) As String
            If str Is Nothing OrElse str.Length = 0 Then
                Return str
            End If
            Dim sb As New StringBuilder(str.Length)
            If upperCase Then
                sb.Append(Char.ToUpperInvariant(str.Chars(0)))
            Else
                sb.Append(Char.ToLowerInvariant(str.Chars(0)))
            End If
            sb.Append(str.Substring(1))
            Return sb.ToString()
        End Function

        Public Shared Function getFilename(ByVal path As String) As String
            If path Is Nothing Then
                Return Nothing
            End If
            Dim separatorIndex As Integer = path.LastIndexOf(FOLDER_SEPARATOR, StringComparison.Ordinal)
            Return If(separatorIndex <> -1, path.Substring(separatorIndex + 1), path)
        End Function

        Public Shared Function getFilenameExtension(ByVal path As String) As String
            If path Is Nothing Then
                Return Nothing
            End If
            Dim extIndex As Integer = path.LastIndexOf(EXTENSION_SEPARATOR)
            If extIndex = -1 Then
                Return Nothing
            End If
            Dim folderIndex As Integer = path.LastIndexOf(FOLDER_SEPARATOR, StringComparison.Ordinal)
            Return If(folderIndex > extIndex, Nothing, path.Substring(extIndex + 1))
        End Function

        Public Shared Function stripFilenameExtension(ByVal path As String) As String
            If path Is Nothing Then
                Return Nothing
            End If
            Dim extIndex As Integer = path.LastIndexOf(EXTENSION_SEPARATOR)
            If extIndex = -1 Then
                Return path
            End If
            Dim folderIndex As Integer = path.LastIndexOf(FOLDER_SEPARATOR, StringComparison.Ordinal)
            Return If(folderIndex > extIndex, path, path.Substring(0, extIndex))
        End Function

        Public Shared Function applyRelativePath(ByVal path As String, ByVal relativePath As String) As String
            Dim separatorIndex As Integer = path.LastIndexOf(FOLDER_SEPARATOR, StringComparison.Ordinal)
            If separatorIndex <> -1 Then
                Dim newPath As String = path.Substring(0, separatorIndex)
                If Not relativePath.StartsWith(FOLDER_SEPARATOR, StringComparison.Ordinal) Then
                    newPath &= FOLDER_SEPARATOR
                End If
                Return newPath & relativePath
            End If
            Return relativePath
        End Function

        ''' <summary>Normalises the path: backslashes become slashes and <c>.</c> / <c>..</c> elements are resolved.</summary>
        Public Shared Function cleanPath(ByVal path As String) As String
            If path Is Nothing Then
                Return Nothing
            End If
            Dim pathToUse As String = replace(path, WINDOWS_FOLDER_SEPARATOR, FOLDER_SEPARATOR)
            Dim prefixIndex As Integer = pathToUse.IndexOf(":"c)
            Dim prefix As String = ""
            If prefixIndex <> -1 Then
                prefix = pathToUse.Substring(0, prefixIndex + 1)
                pathToUse = pathToUse.Substring(prefixIndex + 1)
            End If
            If pathToUse.StartsWith(FOLDER_SEPARATOR, StringComparison.Ordinal) Then
                prefix &= FOLDER_SEPARATOR
                pathToUse = pathToUse.Substring(1)
            End If
            Dim pathArray() As String = delimitedListToStringArray(pathToUse, FOLDER_SEPARATOR)
            Dim pathElements As New List(Of String)()
            Dim tops As Integer = 0
            For i As Integer = pathArray.Length - 1 To 0 Step -1
                Dim element As String = pathArray(i)
                If CURRENT_PATH.Equals(element) Then
                    ' points to the current directory - drop it
                ElseIf TOP_PATH.Equals(element) Then
                    tops += 1
                ElseIf tops > 0 Then
                    tops -= 1
                Else
                    pathElements.Insert(0, element)
                End If
            Next i
            For i As Integer = 0 To tops - 1
                pathElements.Insert(0, TOP_PATH)
            Next i
            Return prefix & collectionToDelimitedString(pathElements, FOLDER_SEPARATOR)
        End Function

        Public Shared Function pathEquals(ByVal path1 As String, ByVal path2 As String) As Boolean
            Return cleanPath(path1).Equals(cleanPath(path2))
        End Function

        ''' <summary>
        ''' Parses a Java locale string such as <c>en</c>, <c>en_GB</c> or <c>en_GB_POSIX</c> into a
        ''' <see cref="CultureInfo"/> (<c>en-GB</c>). The variant part is validated as in Java but
        ''' dropped, because <see cref="CultureInfo"/> has no equivalent of <c>Locale.getVariant()</c>.
        ''' Returns Nothing when there is no language part.
        ''' </summary>
        Public Shared Function parseLocaleString(ByVal localeString As String) As CultureInfo
            Dim parts() As String = tokenizeToStringArray(localeString, "_ ", False, False)
            Dim language As String = If(parts.Length > 0, parts(0), "")
            Dim country As String = If(parts.Length > 1, parts(1), "")
            validateLocalePart(language)
            validateLocalePart(country)
            Dim variantPart As String = ""
            If parts.Length >= 2 Then
                Dim endIndexOfCountryCode As Integer = localeString.LastIndexOf(country, StringComparison.Ordinal) + country.Length
                variantPart = trimLeadingWhitespace(localeString.Substring(endIndexOfCountryCode))
                If variantPart.StartsWith("_", StringComparison.Ordinal) Then
                    variantPart = trimLeadingCharacter(variantPart, "_"c)
                End If
            End If
            If language.Length = 0 Then
                Return Nothing
            End If
            Return New CultureInfo(If(country.Length > 0, language & "-" & country, language))
        End Function

        Private Shared Sub validateLocalePart(ByVal localePart As String)
            For i As Integer = 0 To localePart.Length - 1
                Dim ch As Char = localePart.Chars(i)
                If ch <> "_"c AndAlso ch <> " "c AndAlso Not Char.IsLetterOrDigit(ch) Then
                    Throw New ArgumentException("Locale part """ & localePart & """ contains invalid characters")
                End If
            Next i
        End Sub

        ''' <summary>The culture name as a language tag, e.g. <c>en-GB</c> (Java: language plus <c>-country</c> when present).</summary>
        Public Shared Function toLanguageTag(ByVal locale As CultureInfo) As String
            Return locale.Name
        End Function

        Public Shared Function addStringToArray(ByVal array() As String, ByVal str As String) As String()
            If ObjectUtils.isEmpty(array) Then
                Return New String() {str}
            End If
            Dim newArr(array.Length) As String
            System.Array.Copy(array, 0, newArr, 0, array.Length)
            newArr(array.Length) = str
            Return newArr
        End Function

        Public Shared Function concatenateStringArrays(ByVal array1() As String, ByVal array2() As String) As String()
            If ObjectUtils.isEmpty(array1) Then
                Return array2
            End If
            If ObjectUtils.isEmpty(array2) Then
                Return array1
            End If
            Dim newArr(array1.Length + array2.Length - 1) As String
            System.Array.Copy(array1, 0, newArr, 0, array1.Length)
            System.Array.Copy(array2, 0, newArr, array1.Length, array2.Length)
            Return newArr
        End Function

        Public Shared Function mergeStringArrays(ByVal array1() As String, ByVal array2() As String) As String()
            If ObjectUtils.isEmpty(array1) Then
                Return array2
            End If
            If ObjectUtils.isEmpty(array2) Then
                Return array1
            End If
            Dim result As New List(Of String)(array1)
            For Each str As String In array2
                If Not result.Contains(str) Then
                    result.Add(str)
                End If
            Next str
            Return toStringArray(result)
        End Function

        ''' <summary>Sorts the array in place in ordinal order (Java's natural <c>String</c> order) and returns it.</summary>
        Public Shared Function sortStringArray(ByVal array() As String) As String()
            If ObjectUtils.isEmpty(array) Then
                Return New String() {}
            End If
            System.Array.Sort(array, StringComparer.Ordinal)
            Return array
        End Function

        Public Shared Function toStringArray(ByVal collection As ICollection(Of String)) As String()
            If collection Is Nothing Then
                Return Nothing
            End If
            Dim result(collection.Count - 1) As String
            collection.CopyTo(result, 0)
            Return result
        End Function

        ''' <summary>Java: <c>toStringArray(Enumeration&lt;String&gt;)</c>; drains the enumerator.</summary>
        Public Shared Function toStringArray(ByVal enumeration As IEnumerator(Of String)) As String()
            If enumeration Is Nothing Then
                Return Nothing
            End If
            Dim list As New List(Of String)()
            Do While enumeration.MoveNext()
                list.Add(enumeration.Current)
            Loop
            Return list.ToArray()
        End Function

        Public Shared Function trimArrayElements(ByVal array() As String) As String()
            If ObjectUtils.isEmpty(array) Then
                Return New String() {}
            End If
            Dim result(array.Length - 1) As String
            For i As Integer = 0 To array.Length - 1
                Dim element As String = array(i)
                result(i) = If(element IsNot Nothing, element.Trim(), Nothing)
            Next i
            Return result
        End Function

        ''' <summary>Removes duplicates and sorts the strings (Java used a <c>TreeSet</c>: ordinal order).</summary>
        Public Shared Function removeDuplicateStrings(ByVal array() As String) As String()
            If ObjectUtils.isEmpty(array) Then
                Return array
            End If
            Dim [set] As New SortedSet(Of String)(array, StringComparer.Ordinal)
            Return toStringArray([set])
        End Function

        Public Shared Function split(ByVal toSplit As String, ByVal delimiter As String) As String()
            If Not hasLength(toSplit) OrElse Not hasLength(delimiter) Then
                Return Nothing
            End If
            Dim offset As Integer = toSplit.IndexOf(delimiter, StringComparison.Ordinal)
            If offset < 0 Then
                Return Nothing
            End If
            Dim beforeDelimiter As String = toSplit.Substring(0, offset)
            Dim afterDelimiter As String = toSplit.Substring(offset + delimiter.Length)
            Return New String() {beforeDelimiter, afterDelimiter}
        End Function

        Public Shared Function splitArrayElementsIntoProperties(ByVal array() As String, ByVal delimiter As String) As Properties
            Return splitArrayElementsIntoProperties(array, delimiter, Nothing)
        End Function

        Public Shared Function splitArrayElementsIntoProperties(ByVal array() As String, ByVal delimiter As String, ByVal charsToDelete As String) As Properties
            If ObjectUtils.isEmpty(array) Then
                Return Nothing
            End If
            Dim result As New Properties()
            For Each element As String In array
                Dim current As String = element
                If charsToDelete IsNot Nothing Then
                    current = deleteAny(current, charsToDelete)
                End If
                Dim splittedElement() As String = split(current, delimiter)
                If splittedElement IsNot Nothing Then
                    result.setProperty(splittedElement(0).Trim(), splittedElement(1).Trim())
                End If
            Next element
            Return result
        End Function

        Public Shared Function tokenizeToStringArray(ByVal str As String, ByVal delimiters As String) As String()
            Return tokenizeToStringArray(str, delimiters, True, True)
        End Function

        ''' <summary>
        ''' Tokenizes like <c>java.util.StringTokenizer</c>: every character of <paramref name="delimiters"/>
        ''' separates tokens and the tokenizer itself never produces empty tokens (trimming can).
        ''' </summary>
        Public Shared Function tokenizeToStringArray(ByVal str As String, ByVal delimiters As String, ByVal trimTokens As Boolean, ByVal ignoreEmptyTokens As Boolean) As String()
            If str Is Nothing Then
                Return Nothing
            End If
            Dim tokens As New List(Of String)()
            For Each rawToken As String In str.Split(delimiters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                Dim token As String = rawToken
                If trimTokens Then
                    token = token.Trim()
                End If
                If Not ignoreEmptyTokens OrElse token.Length > 0 Then
                    tokens.Add(token)
                End If
            Next rawToken
            Return toStringArray(tokens)
        End Function

        Public Shared Function delimitedListToStringArray(ByVal str As String, ByVal delimiter As String) As String()
            Return delimitedListToStringArray(str, delimiter, Nothing)
        End Function

        Public Shared Function delimitedListToStringArray(ByVal str As String, ByVal delimiter As String, ByVal charsToDelete As String) As String()
            If str Is Nothing Then
                Return New String() {}
            End If
            If delimiter Is Nothing Then
                Return New String() {str}
            End If
            Dim result As New List(Of String)()
            If "".Equals(delimiter) Then
                For i As Integer = 0 To str.Length - 1
                    result.Add(deleteAny(str.Substring(i, 1), charsToDelete))
                Next i
            Else
                Dim pos As Integer = 0
                Dim delPos As Integer = str.IndexOf(delimiter, pos, StringComparison.Ordinal)
                Do While delPos <> -1
                    result.Add(deleteAny(str.Substring(pos, delPos - pos), charsToDelete))
                    pos = delPos + delimiter.Length
                    delPos = str.IndexOf(delimiter, pos, StringComparison.Ordinal)
                Loop
                If str.Length > 0 AndAlso pos <= str.Length Then
                    result.Add(deleteAny(str.Substring(pos), charsToDelete))
                End If
            End If
            Return toStringArray(result)
        End Function

        Public Shared Function commaDelimitedListToStringArray(ByVal str As String) As String()
            Return delimitedListToStringArray(str, ",")
        End Function

        ''' <summary>Java returned a <c>TreeSet</c>: the set is sorted in ordinal order.</summary>
        Public Shared Function commaDelimitedListToSet(ByVal str As String) As ISet(Of String)
            Dim [set] As New SortedSet(Of String)(StringComparer.Ordinal)
            For Each token As String In commaDelimitedListToStringArray(str)
                [set].Add(token)
            Next token
            Return [set]
        End Function

        ''' <summary>
        ''' Java: <c>collectionToDelimitedString(Collection&lt;?&gt;, ...)</c>. Any enumerable is accepted;
        ''' elements are rendered like Java's <c>String.valueOf</c> (<c>null</c>, <c>true</c>, <c>1.0</c>).
        ''' </summary>
        Public Shared Function collectionToDelimitedString(ByVal coll As IEnumerable, ByVal delim As String, ByVal prefix As String, ByVal suffix As String) As String
            If coll Is Nothing Then
                Return ""
            End If
            Dim sb As New StringBuilder()
            Dim first As Boolean = True
            For Each item As Object In coll
                If Not first Then
                    sb.Append(delim)
                End If
                first = False
                sb.Append(prefix).Append(Arrays.FormatElement(item)).Append(suffix)
            Next item
            Return sb.ToString()
        End Function

        Public Shared Function collectionToDelimitedString(ByVal coll As IEnumerable, ByVal delim As String) As String
            Return collectionToDelimitedString(coll, delim, "", "")
        End Function

        Public Shared Function collectionToCommaDelimitedString(ByVal coll As IEnumerable) As String
            Return collectionToDelimitedString(coll, ",")
        End Function

        Public Shared Function arrayToDelimitedString(ByVal arr() As Object, ByVal delim As String) As String
            If ObjectUtils.isEmpty(arr) Then
                Return ""
            End If
            If arr.Length = 1 Then
                Return ObjectUtils.nullSafeToString(arr(0))
            End If
            Dim sb As New StringBuilder()
            For i As Integer = 0 To arr.Length - 1
                If i > 0 Then
                    sb.Append(delim)
                End If
                sb.Append(Arrays.FormatElement(arr(i)))
            Next i
            Return sb.ToString()
        End Function

        Public Shared Function arrayToCommaDelimitedString(ByVal arr() As Object) As String
            Return arrayToDelimitedString(arr, ",")
        End Function
    End Class

End Namespace
