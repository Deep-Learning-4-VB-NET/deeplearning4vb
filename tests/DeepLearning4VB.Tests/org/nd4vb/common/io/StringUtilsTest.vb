Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.common.io

Namespace org.nd4vb.common.io

    <TestClass>
    Public Class StringUtilsTest

        <TestMethod>
        Public Sub HasLengthAndHasText()
            assertFalse(StringUtils.hasLength(Nothing))
            assertFalse(StringUtils.hasLength(""))
            assertTrue(StringUtils.hasLength(" "))
            assertFalse(StringUtils.hasText("   "))
            assertTrue(StringUtils.hasText(" a "))
            assertTrue(StringUtils.containsWhitespace("a b"))
            assertFalse(StringUtils.containsWhitespace("ab"))
            assertTrue(StringUtils.isEmpty(""))
            assertTrue(StringUtils.isEmpty(Nothing))
            assertFalse(StringUtils.isEmpty("x"))
        End Sub

        <TestMethod>
        Public Sub Trimming()
            assertEquals("a b", StringUtils.trimWhitespace("  a b  "))
            assertEquals("ab", StringUtils.trimAllWhitespace(" a b "))
            assertEquals("a ", StringUtils.trimLeadingWhitespace(" a "))
            assertEquals(" a", StringUtils.trimTrailingWhitespace(" a "))
            assertEquals("a__", StringUtils.trimLeadingCharacter("__a__", "_"c))
            assertEquals("__a", StringUtils.trimTrailingCharacter("__a__", "_"c))
            assertEquals(New String() {"a", "b", Nothing}, StringUtils.trimArrayElements(New String() {" a", "b ", Nothing}))
        End Sub

        <TestMethod>
        Public Sub ReplaceDeleteAndCount()
            assertEquals("x-y-z", StringUtils.replace("x.y.z", ".", "-"))
            assertEquals("abc", StringUtils.replace("abc", "", "-"))
            assertEquals("ac", StringUtils.delete("abc", "b"))
            assertEquals("bd", StringUtils.deleteAny("abcd", "ac"))
            assertEquals(3, StringUtils.countOccurrencesOf("abababa", "ab"))
            assertEquals(2, StringUtils.countOccurrencesOf("abababa", "aba"))
            assertEquals(0, StringUtils.countOccurrencesOf("abc", "d"))
            assertEquals(0, StringUtils.countOccurrencesOf("abc", ""))
            assertEquals("'a'", StringUtils.quote("a"))
            assertNull(StringUtils.quote(Nothing))
            assertEquals("'a'", StringUtils.quoteIfString("a"))
            assertEquals(5, StringUtils.quoteIfString(5))
        End Sub

        <TestMethod>
        Public Sub CaseAndPrefixHelpers()
            assertEquals("Hello", StringUtils.capitalize("hello"))
            assertEquals("hello", StringUtils.uncapitalize("Hello"))
            assertEquals("", StringUtils.capitalize(""))
            assertNull(StringUtils.capitalize(Nothing))
            assertTrue(StringUtils.startsWithIgnoreCase("Hello", "he"))
            assertFalse(StringUtils.startsWithIgnoreCase("Hello", "hello world"))
            assertTrue(StringUtils.endsWithIgnoreCase("Hello", "LLO"))
            assertFalse(StringUtils.endsWithIgnoreCase(Nothing, "x"))
            assertTrue(StringUtils.substringMatch("hello", 1, "ell"))
            assertFalse(StringUtils.substringMatch("hello", 3, "lol"))
            assertEquals("Baz", StringUtils.unqualify("foo.bar.Baz"))
            assertEquals("Baz", StringUtils.unqualify("foo/bar/Baz", "/"c))
        End Sub

        <TestMethod>
        Public Sub PathHelpers()
            assertEquals("file.txt", StringUtils.getFilename("a/b/file.txt"))
            assertEquals("file.txt", StringUtils.getFilename("file.txt"))
            assertNull(StringUtils.getFilename(Nothing))
            assertEquals("txt", StringUtils.getFilenameExtension("a/b/file.txt"))
            assertNull(StringUtils.getFilenameExtension("a.b/file"))
            assertEquals("a/b/file", StringUtils.stripFilenameExtension("a/b/file.txt"))
            assertEquals("a.b/file", StringUtils.stripFilenameExtension("a.b/file"))
            assertEquals("a/b/c.txt", StringUtils.applyRelativePath("a/b/x.txt", "c.txt"))
            assertEquals("c.txt", StringUtils.applyRelativePath("x.txt", "c.txt"))
            assertEquals("a/c/d.txt", StringUtils.cleanPath("a\b\..\c\.\d.txt"))
            assertEquals("file:/x/y", StringUtils.cleanPath("file:/x/./y"))
            assertEquals("../a", StringUtils.cleanPath("../a"))
            assertNull(StringUtils.cleanPath(Nothing))
            assertTrue(StringUtils.pathEquals("a/b/../c", "a/c"))
            assertFalse(StringUtils.pathEquals("a/b", "a/c"))
        End Sub

        <TestMethod>
        Public Sub ArraysAndCollections()
            assertEquals(New String() {"a", "b"}, StringUtils.addStringToArray(New String() {"a"}, "b"))
            assertEquals(New String() {"b"}, StringUtils.addStringToArray(Nothing, "b"))
            assertEquals(New String() {"a", "b", "c"}, StringUtils.concatenateStringArrays(New String() {"a", "b"}, New String() {"c"}))
            assertEquals(New String() {"a", "b", "c"}, StringUtils.mergeStringArrays(New String() {"a", "b"}, New String() {"b", "c"}))
            assertEquals(New String() {"a", "b", "c"}, StringUtils.sortStringArray(New String() {"c", "a", "b"}))
            assertEquals(New String() {"a", "b"}, StringUtils.removeDuplicateStrings(New String() {"b", "a", "b"}))
            Dim list As New List(Of String) From {"x", "y"}
            assertEquals(New String() {"x", "y"}, StringUtils.toStringArray(list))
            Dim enumerator As IEnumerator(Of String) = list.GetEnumerator()
            assertEquals(New String() {"x", "y"}, StringUtils.toStringArray(enumerator))
            assertEquals("a,b", StringUtils.collectionToCommaDelimitedString(New List(Of String) From {"a", "b"}))
            assertEquals("[a];[b]", StringUtils.collectionToDelimitedString(New List(Of String) From {"a", "b"}, ";", "[", "]"))
            assertEquals("", StringUtils.collectionToDelimitedString(Nothing, ","))
            assertEquals("1,null", StringUtils.arrayToCommaDelimitedString(New Object() {1, Nothing}))
            assertEquals("x", StringUtils.arrayToDelimitedString(New Object() {"x"}, ","))
            assertEquals("", StringUtils.arrayToDelimitedString(Nothing, ","))
        End Sub

        <TestMethod>
        Public Sub SplittingAndTokenizing()
            assertEquals(New String() {"a", "b=c"}, StringUtils.split("a=b=c", "="))
            assertNull(StringUtils.split("abc", "="))
            assertEquals(New String() {"a", "b", "c"}, StringUtils.tokenizeToStringArray(" a , b ,, c ", ","))
            assertEquals(New String() {"a", "", "c"}, StringUtils.tokenizeToStringArray("a, ,c", ",", True, False))
            assertEquals(New String() {"a", "b", ""}, StringUtils.delimitedListToStringArray("a,b,", ","))
            assertEquals(New String() {"a", "b"}, StringUtils.delimitedListToStringArray("a|b", "|"))
            assertEquals(New String() {"a", "b", "c"}, StringUtils.delimitedListToStringArray("abc", ""))
            assertEquals(New String() {}, StringUtils.delimitedListToStringArray(Nothing, ","))
            assertEquals(New String() {"a,b"}, StringUtils.delimitedListToStringArray("a,b", Nothing))
            assertEquals(New String() {"x", "y"}, StringUtils.commaDelimitedListToStringArray("x,y"))
            assertEquals(New String() {"a", "b", "c"}, StringUtils.commaDelimitedListToSet("c,a,b,a").ToArray())
            Dim props = StringUtils.splitArrayElementsIntoProperties(New String() {"a=1", " b = 2 ", "c"}, "=")
            assertEquals("1", props.getProperty("a"))
            assertEquals("2", props.getProperty("b"))
            assertNull(props.getProperty("c"))
            assertEquals(2, props.size())
            Dim stripped = StringUtils.splitArrayElementsIntoProperties(New String() {"a=[1]"}, "=", "[]")
            assertEquals("1", stripped.getProperty("a"))
            assertNull(StringUtils.splitArrayElementsIntoProperties(New String() {}, "="))
        End Sub

        <TestMethod>
        Public Sub LocaleParsing()
            Dim culture = StringUtils.parseLocaleString("en_GB")
            assertEquals("en-GB", culture.Name)
            assertEquals("en-GB", StringUtils.toLanguageTag(culture))
            assertEquals("en", StringUtils.parseLocaleString("en").Name)
            assertEquals("en-GB", StringUtils.parseLocaleString("en_GB_POSIX").Name)
            assertNull(StringUtils.parseLocaleString(""))
            assertThrows(Of ArgumentException)(Function() StringUtils.parseLocaleString("en-GB"))
        End Sub

    End Class

End Namespace
