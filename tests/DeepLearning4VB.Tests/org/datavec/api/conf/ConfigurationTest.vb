Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.conf

Namespace org.datavec.api.conf

    ''' <summary>
    ''' The Hadoop-style <see cref="Configuration"/>: typed get/set with defaults, variable expansion,
    ''' resources (XML streams and files, final parameters), iteration, copying, XML/JSON output, and
    ''' the <see cref="Configurable"/>/<see cref="Configured"/> pair.
    ''' </summary>
    <TestClass>
    Public Class ConfigurationTest

        Private tmpDir As String

        <TestInitialize>
        Public Sub CreateTempDir()
            tmpDir = Path.Combine(Path.GetTempPath(), "dl4vb-tests", Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(tmpDir)
        End Sub

        <TestCleanup>
        Public Sub DeleteTempDir()
            If Directory.Exists(tmpDir) Then Directory.Delete(tmpDir, True)
        End Sub

        Private Shared Function xmlStream(ByVal ParamArray properties() As String) As Stream
            'properties are "name=value" or "name=value=final"
            Dim sb As New StringBuilder()
            sb.Append("<?xml version=""1.0""?>").Append(vbLf)
            sb.Append("<configuration>").Append(vbLf)
            For Each p As String In properties
                Dim parts() As String = p.Split("="c)
                sb.Append("  <property>").Append(vbLf)
                sb.Append("    <name>").Append(parts(0)).Append("</name>").Append(vbLf)
                sb.Append("    <value>").Append(parts(1)).Append("</value>").Append(vbLf)
                If parts.Length > 2 Then
                    sb.Append("    <final>").Append(parts(2)).Append("</final>").Append(vbLf)
                End If
                sb.Append("  </property>").Append(vbLf)
            Next p
            sb.Append("</configuration>").Append(vbLf)
            Return New MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()))
        End Function

        ''' <summary>Writes the same XML to a file in the temp directory and returns its URI (a resource that can be re-read).</summary>
        Private Function xmlFile(ByVal name As String, ByVal ParamArray properties() As String) As Uri
            Dim target As String = Path.Combine(tmpDir, name)
            Using s As Stream = xmlStream(properties)
                Using f As New FileStream(target, FileMode.Create, FileAccess.Write)
                    s.CopyTo(f)
                End Using
            End Using
            Return New Uri(target)
        End Function

        <TestMethod>
        Public Sub testGetSetAndDefaults()
            Dim conf As New Configuration(False)
            assertNull(conf.[get]("missing"))
            assertEquals("dflt", conf.[get]("missing", "dflt"))
            assertNull(conf.getRaw("missing"))

            conf.[set]("name", "value")
            assertEquals("value", conf.[get]("name"))
            assertEquals("value", conf.[get]("name", "dflt"))
            assertEquals("value", conf.getRaw("name"))
            conf.[set]("name", "other")
            assertEquals("other", conf.[get]("name"))

            'setIfUnset only writes when the property is absent
            conf.setIfUnset("name", "ignored")
            assertEquals("other", conf.[get]("name"))
            conf.setIfUnset("fresh", "set")
            assertEquals("set", conf.[get]("fresh"))

            'a property value must not be Nothing (java.util.Properties semantics)
            assertThrows(Of ArgumentNullException)(Sub() conf.[set]("name", Nothing))

            'getChar returns the first character
            conf.[set]("ch", "xyz")
            assertEquals("x"c, conf.getChar("ch"))
            assertEquals("x"c, conf.getChar("ch", "d"c))
            assertEquals("d"c, conf.getChar("missing", "d"c))
            assertThrows(Of NullReferenceException)(Sub() conf.getChar("missing"))
        End Sub

        <TestMethod>
        Public Sub testIntAndLong()
            Dim conf As New Configuration(False)
            assertEquals(5, conf.getInt("missing", 5))
            conf.setInt("i", 42)
            assertEquals("42", conf.[get]("i"))
            assertEquals(42, conf.getInt("i", 0))
            conf.[set]("neg", "-17")
            assertEquals(-17, conf.getInt("neg", 0))
            conf.[set]("plus", "+8")
            assertEquals(8, conf.getInt("plus", 0))
            'hexadecimal values are accepted
            conf.[set]("hex", "0x1F")
            assertEquals(31, conf.getInt("hex", 0))
            conf.[set]("hexUpper", "0XfF")
            assertEquals(255, conf.getInt("hexUpper", 0))
            conf.[set]("negHex", "-0x10")
            assertEquals(-16, conf.getInt("negHex", 0))
            'invalid values (including surrounding whitespace, as Integer.parseInt) give the default
            conf.[set]("bad", "abc")
            assertEquals(7, conf.getInt("bad", 7))
            conf.[set]("spaced", " 12")
            assertEquals(7, conf.getInt("spaced", 7))
            conf.[set]("float", "1.5")
            assertEquals(7, conf.getInt("float", 7))
            conf.[set]("empty", "")
            assertEquals(7, conf.getInt("empty", 7))
            'values outside the int range are not ints
            conf.[set]("big", "4294967296")
            assertEquals(7, conf.getInt("big", 7))
            assertEquals(4294967296L, conf.getLong("big", 7L))

            conf.setLong("l", Long.MaxValue)
            assertEquals(Long.MaxValue.ToString(), conf.[get]("l"))
            assertEquals(Long.MaxValue, conf.getLong("l", 0L))
            conf.[set]("lmin", Long.MinValue.ToString())
            assertEquals(Long.MinValue, conf.getLong("lmin", 0L))
            conf.[set]("ltoobig", "9223372036854775808")
            assertEquals(3L, conf.getLong("ltoobig", 3L))
            conf.[set]("lhex", "0x7fffffffffffffff")
            assertEquals(Long.MaxValue, conf.getLong("lhex", 0L))
            assertEquals(9L, conf.getLong("missing", 9L))
            assertEquals(9L, conf.getLong("bad", 9L))
        End Sub

        <TestMethod>
        Public Sub testFloatAndBoolean()
            Dim conf As New Configuration(False)
            assertEquals(1.25F, conf.getFloat("missing", 1.25F), 0F)
            conf.setFloat("f", 1.5F)
            'stored as Float.toString would format it
            assertEquals("1.5", conf.[get]("f"))
            assertEquals(1.5F, conf.getFloat("f", 0F), 0F)
            conf.setFloat("f2", 100F)
            assertEquals("100.0", conf.[get]("f2"))
            conf.setFloat("f3", 1.0E10F)
            assertEquals("1.0E10", conf.[get]("f3"))
            assertEquals(1.0E10F, conf.getFloat("f3", 0F), 0F)
            'Float.parseFloat accepts a type suffix and surrounding whitespace
            conf.[set]("suffix", "2.5f")
            assertEquals(2.5F, conf.getFloat("suffix", 0F), 0F)
            conf.[set]("suffixD", "3.5d")
            assertEquals(3.5F, conf.getFloat("suffixD", 0F), 0F)
            conf.[set]("padded", " 4.5 ")
            assertEquals(4.5F, conf.getFloat("padded", 0F), 0F)
            conf.[set]("sci", "1e3")
            assertEquals(1000F, conf.getFloat("sci", 0F), 0F)
            conf.[set]("badf", "x1")
            assertEquals(0.5F, conf.getFloat("badf", 0.5F), 0F)

            'booleans: only the exact strings "true" and "false" count
            assertTrue(conf.getBoolean("missing", True))
            assertFalse(conf.getBoolean("missing", False))
            conf.setBoolean("t", True)
            conf.setBoolean("f", False)
            assertEquals("true", conf.[get]("t"))
            assertEquals("false", conf.[get]("f"))
            assertTrue(conf.getBoolean("t", False))
            assertFalse(conf.getBoolean("f", True))
            conf.[set]("upper", "TRUE")
            assertFalse(conf.getBoolean("upper", False))
            assertTrue(conf.getBoolean("upper", True))
            conf.[set]("yes", "yes")
            assertFalse(conf.getBoolean("yes", False))
            conf.setBooleanIfUnset("t", False)
            assertTrue(conf.getBoolean("t", False))
            conf.setBooleanIfUnset("unset", False)
            assertEquals("false", conf.[get]("unset"))
        End Sub

        <TestMethod>
        Public Sub testStringsCollectionsAndTrimming()
            Dim conf As New Configuration(False)
            assertNull(conf.getStrings("missing"))
            assertNull(conf.getStringCollection("missing"))
            assertNull(conf.getTrimmedStrings("missing"))
            assertArrayEquals(New String() {"x", "y"}, conf.getStrings("missing", "x", "y"))
            assertArrayEquals(New String() {"x"}, conf.getTrimmedStrings("missing", "x"))
            assertEquals(0, conf.getTrimmedStringCollection("missing").Count)

            conf.setStrings("list", "a", "b", "c")
            assertEquals("a,b,c", conf.[get]("list"))
            assertArrayEquals(New String() {"a", "b", "c"}, conf.getStrings("list"))
            assertArrayEquals(New String() {"a", "b", "c"}, conf.getStrings("list", "ignored"))
            assertEquals(Arrays.asList("a", "b", "c"), New List(Of String)(conf.getStringCollection("list")))

            'adjacent separators are treated as one (commons-lang StringUtils.split)
            conf.[set]("gaps", "a,,b,")
            assertArrayEquals(New String() {"a", "b"}, conf.getStrings("gaps"))
            'the untrimmed forms keep the whitespace, the trimmed forms strip it
            conf.[set]("spaced", " a , b ,c")
            assertArrayEquals(New String() {" a ", " b ", "c"}, conf.getStrings("spaced"))
            assertArrayEquals(New String() {"a", "b", "c"}, conf.getTrimmedStrings("spaced"))
            assertArrayEquals(New String() {"a", "b", "c"}, conf.getTrimmedStrings("spaced", "ignored"))
            assertEquals(Arrays.asList("a", "b", "c"), New List(Of String)(conf.getTrimmedStringCollection("spaced")))

            conf.setStrings("single", "only")
            assertArrayEquals(New String() {"only"}, conf.getStrings("single"))
            conf.setStrings("empty")
            assertEquals("", conf.[get]("empty"))
            assertEquals(0, conf.getStrings("empty").Length)
        End Sub

        <TestMethod>
        Public Sub testPatternAndRange()
            Dim conf As New Configuration(False)
            Dim dflt As New Regex("d.*")
            assertTrue(conf.getPattern("missing", dflt) Is dflt)
            conf.setPattern("p", New Regex("a+b"))
            assertEquals("a+b", conf.[get]("p"))
            Dim p As Regex = conf.getPattern("p", dflt)
            assertEquals("a+b", p.ToString())
            assertTrue(p.IsMatch("xaab"))
            'an invalid or empty pattern falls back to the default
            conf.[set]("bad", "(")
            assertTrue(conf.getPattern("bad", dflt) Is dflt)
            conf.[set]("empty", "")
            assertTrue(conf.getPattern("empty", dflt) Is dflt)
            assertThrows(Of ArgumentNullException)(Sub() conf.setPattern("none", Nothing))

            'integer ranges: "2-3,5,7-" is 2, 3, 5 and everything from 7 up
            conf.[set]("r", "2-3,5,7-")
            Dim ranges As Configuration.IntegerRanges = conf.getRange("r", "")
            For Each included As Integer In New Integer() {2, 3, 5, 7, 8, 1000, Integer.MaxValue}
                assertTrue(ranges.isIncluded(included), included & " should be included")
            Next included
            For Each excluded As Integer In New Integer() {0, 1, 4, 6, -5}
                assertFalse(ranges.isIncluded(excluded), excluded & " should be excluded")
            Next excluded
            assertEquals("2-3,5-5,7-" & Integer.MaxValue, ranges.ToString())
            'an omitted lower bound means zero; the default string is used when the property is absent
            assertTrue(conf.getRange("missing", "-3").isIncluded(0))
            assertFalse(conf.getRange("missing", "-3").isIncluded(4))
            assertEquals("", conf.getRange("missing", "").ToString())
            assertFalse(conf.getRange("missing", "").isIncluded(0))
            assertEquals("1-1,4-4", New Configuration.IntegerRanges(" 1 , 4 ,").ToString())
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New Configuration.IntegerRanges("5-3")
                                               End Sub)
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New Configuration.IntegerRanges("1-2-3")
                                               End Sub)
            assertFalse(New Configuration.IntegerRanges().isIncluded(1))
        End Sub

        <TestMethod>
        Public Sub testVariableSubstitution()
            Dim conf As New Configuration(False)
            conf.[set]("base", "/data")
            conf.[set]("full", "${base}/x")
            conf.[set]("nested", "${full}/y")
            assertEquals("/data/x", conf.[get]("full"))
            assertEquals("/data/x/y", conf.[get]("nested"))
            'getRaw does not expand
            assertEquals("${base}/x", conf.getRaw("full"))
            'an unbound variable is left as it is
            conf.[set]("unbound", "${no.such.var}/z")
            assertEquals("${no.such.var}/z", conf.[get]("unbound"))
            'defaults are expanded too
            assertEquals("/data/d", conf.[get]("missing", "${base}/d"))
            'typed getters see the expanded value
            conf.[set]("n", "4")
            conf.[set]("ref", "${n}")
            assertEquals(4, conf.getInt("ref", 0))

            'system properties take precedence over configuration properties
            Dim key As String = "dl4vb.conf.test." & Guid.NewGuid().ToString("N")
            JavaSystem.setProperty(key, "sys")
            Try
                conf.[set](key, "conf")
                conf.[set]("sysref", "${" & key & "}")
                assertEquals("sys", conf.[get]("sysref"))
            Finally
                JavaSystem.clearProperty(key)
            End Try
            assertEquals("conf", conf.[get]("sysref"))

            'a cycle is detected after 20 substitutions
            conf.[set]("a", "${b}")
            conf.[set]("b", "${a}")
            assertThrows(Of InvalidOperationException)(Sub() conf.[get]("a"))
        End Sub

        <TestMethod>
        Public Sub testResourcesAndFinalParameters()
            Dim conf As New Configuration(False)
            conf.addResource(xmlFile("first.xml", "a=1=true", "b=2", "c= spaced "))
            assertEquals("1", conf.[get]("a"))
            assertEquals("2", conf.[get]("b"))
            'values keep their whitespace, names are trimmed
            assertEquals(" spaced ", conf.[get]("c"))
            assertEquals(3, conf.size())

            'a later resource overrides earlier values unless they are final
            conf.addResource(xmlFile("second.xml", "a=10", "b=20", "d=4"))
            assertEquals("1", conf.[get]("a"))
            assertEquals("20", conf.[get]("b"))
            assertEquals("4", conf.[get]("d"))
            assertEquals(4, conf.size())

            'set() overlays resources, even final ones, and survives a reload
            conf.[set]("a", "set")
            conf.[set]("e", "5")
            assertEquals("set", conf.[get]("a"))
            conf.reloadConfiguration()
            assertEquals("set", conf.[get]("a"))
            assertEquals("20", conf.[get]("b"))
            assertEquals("5", conf.[get]("e"))

            'a stream resource is read once and closed afterwards
            Dim stream As Stream = xmlStream("s=1")
            Dim fromStream As New Configuration(False)
            fromStream.addResource(stream)
            assertTrue(stream.CanRead)
            assertEquals("1", fromStream.[get]("s"))
            assertFalse(stream.CanRead)

            'a nested <configuration> element is read recursively
            Dim nested As New Configuration(False)
            nested.addResource(New MemoryStream(Encoding.UTF8.GetBytes("<configuration><configuration><property><name>inner</name><value>v</value></property></configuration></configuration>")))
            assertEquals("v", nested.[get]("inner"))

            'a resource with no value, or no name, contributes nothing
            Dim incomplete As New Configuration(False)
            incomplete.addResource(New MemoryStream(Encoding.UTF8.GetBytes("<configuration><property><name>onlyName</name></property><property><value>onlyValue</value></property></configuration>")))
            assertEquals(0, incomplete.size())

            'malformed XML is an error once the properties are read
            Dim broken As New Configuration(False)
            broken.addResource(New MemoryStream(Encoding.UTF8.GetBytes("<configuration><property>")))
            assertThrows(Of InvalidOperationException)(Sub() broken.[get]("x"))
        End Sub

        <TestMethod>
        Public Sub testFileResourceAndMissingClassPathResource()
            Dim site As Uri = xmlFile("my-site.xml", "from.file=yes")
            Dim conf As New Configuration(False)
            conf.addResource(site)
            assertEquals("yes", conf.[get]("from.file"))
            assertEquals("Configuration: " & site.ToString(), conf.ToString())

            'a class path resource that does not exist is skipped in quiet mode...
            Dim quiet As New Configuration(False)
            quiet.addResource("no-such-resource-" & Guid.NewGuid().ToString("N") & ".xml")
            assertNull(quiet.[get]("anything"))
            assertEquals(0, quiet.size())
            '...and is an error otherwise
            Dim loud As New Configuration(False)
            loud.setQuietMode(False)
            loud.addResource("no-such-resource-" & Guid.NewGuid().ToString("N") & ".xml")
            assertThrows(Of InvalidOperationException)(Sub() loud.[get]("anything"))

            'the resource lookup helpers return Nothing for unknown names
            assertNull(conf.getResource("no-such-resource.xml"))
            assertNull(conf.getConfResourceAsInputStream("no-such-resource.xml"))
            assertNull(conf.getConfResourceAsReader("no-such-resource.xml"))
            'and find files under the ClassPathResource search directories
            Dim res As Uri = conf.getResource("datavec/util/csvsequence_1.txt")
            assertNotNull(res)
            assertTrue(res.IsFile)
            Using r As TextReader = conf.getConfResourceAsReader("datavec/util/csvsequence_1.txt")
                assertEquals("0.0,1.0,2.0", r.ReadLine())
            End Using
            Using s As Stream = conf.getConfResourceAsInputStream("datavec/util/csvsequence_1.txt")
                assertEquals(AscW("0"c), s.ReadByte())
            End Using
        End Sub

        <TestMethod>
        Public Sub testIterationSizeClearAndCopy()
            Dim conf As New Configuration(False)
            assertEquals(0, conf.size())
            conf.[set]("k1", "v1")
            conf.[set]("k2", "v2")
            assertEquals(2, conf.size())
            Dim seen As New Dictionary(Of String, String)()
            For Each entry As KeyValuePair(Of String, String) In conf
                seen(entry.Key) = entry.Value
            Next entry
            Dim expected As New Dictionary(Of String, String)()
            expected("k1") = "v1"
            expected("k2") = "v2"
            assertEquals(expected, seen)

            'the copy has the same values but is independent afterwards
            Dim copy As New Configuration(conf)
            assertEquals("v1", copy.[get]("k1"))
            assertEquals(2, copy.size())
            copy.[set]("k3", "v3")
            conf.[set]("k1", "changed")
            assertNull(conf.[get]("k3"))
            assertEquals("v1", copy.[get]("k1"))
            assertEquals("changed", conf.[get]("k1"))

            'resources are copied as well
            Dim withResource As New Configuration(False)
            Dim copiedResource As Uri = xmlFile("copied.xml", "r=1=true")
            withResource.addResource(copiedResource)
            withResource.[set]("o", "2")
            Dim copy2 As New Configuration(withResource)
            assertEquals("1", copy2.[get]("r"))
            assertEquals("2", copy2.[get]("o"))
            'the copy reloads the default resources as well (its loadDefaults flag is not copied, as in Java)
            assertEquals("Configuration: core-default.xml, core-site.xml, " & copiedResource.ToString(), copy2.ToString())
            'the final parameters are copied too: a later resource cannot override them in the copy either
            copy2.addResource(xmlFile("override.xml", "r=2"))
            assertEquals("1", copy2.[get]("r"))

            conf.clear()
            assertEquals(0, conf.size())
            assertNull(conf.[get]("k1"))
            'after clear, a reload brings back nothing either (the overlay was cleared too)
            conf.reloadConfiguration()
            assertEquals(0, conf.size())
            assertNull(conf.[get]("k2"))
        End Sub

        <TestMethod>
        Public Sub testWriteXmlAndDump()
            Dim conf As New Configuration(False)
            Dim resource As Uri = xmlFile("dumped.xml", "fromRes=r=true")
            conf.addResource(resource)
            conf.[set]("key", "value <&> ""quoted""")
            conf.[set]("other", "2")
            Dim xml As String
            Using ms As New MemoryStream()
                conf.writeXml(ms)
                xml = Encoding.UTF8.GetString(ms.ToArray())
            End Using
            assertTrue(xml.StartsWith("<?xml version=""1.0"" encoding=""utf-8"" standalone=""no""?>", StringComparison.Ordinal), xml)
            assertTrue(xml.Contains("<property><name>other</name><value>2</value></property>"), xml)
            assertTrue(xml.Contains("<name>key</name><value>value &lt;&amp;&gt; ""quoted""</value>"), xml)
            assertTrue(xml.Contains("<name>fromRes</name><value>r</value>"), xml)
            assertTrue(xml.Trim().EndsWith("</configuration>", StringComparison.Ordinal), xml)

            'the XML can be read back as a resource
            Dim back As New Configuration(False)
            back.addResource(New MemoryStream(Encoding.UTF8.GetBytes(xml)))
            assertEquals("value <&> ""quoted""", back.[get]("key"))
            assertEquals("2", back.[get]("other"))
            assertEquals("r", back.[get]("fromRes"))
            assertEquals(3, back.size())

            'the JSON dump lists every property with its final flag and the resource that set it
            Dim sw As New StringWriter()
            Configuration.dumpConfiguration(conf, sw)
            Dim json As String = sw.ToString()
            assertTrue(json.StartsWith("{""properties"":[", StringComparison.Ordinal), json)
            assertTrue(json.Contains("{""key"":""other"",""value"":""2"",""isFinal"":false,""resource"":""Unknown""}"), json)
            assertTrue(json.Contains("{""key"":""fromRes"",""value"":""r"",""isFinal"":true,""resource"":""" & resource.ToString() & """}"), json)
            assertTrue(json.Contains("""value"":""value <&> \""quoted\"""""), json)
            assertTrue(json.EndsWith("]}", StringComparison.Ordinal), json)
        End Sub

        <TestMethod>
        Public Sub testClassesAndInstances()
            Dim conf As New Configuration(False)
            assertEquals(GetType(String), conf.getClass("missing", GetType(String)))
            assertNull(conf.getClass("missing", Nothing))
            assertNull(conf.getClass("missing", Nothing, GetType(Configurable)))
            assertArrayEquals(New Type() {GetType(Integer)}, conf.getClasses("missing", GetType(Integer)))

            conf.setClass("c", GetType(Configured), GetType(Configurable))
            assertEquals(GetType(Configured).FullName, conf.[get]("c"))
            assertEquals(GetType(Configured), conf.getClass("c", Nothing))
            assertEquals(GetType(Configured), conf.getClass("c", Nothing, GetType(Configurable)))
            'the class must implement the interface, both when set and when read
            assertThrows(Of InvalidOperationException)(Sub() conf.setClass("x", GetType(String), GetType(Configurable)))
            assertThrows(Of InvalidOperationException)(Sub() conf.getClass("c", Nothing, GetType(IDisposable)))
            'an unknown class name is an error
            conf.[set]("unknown", "no.such.Class")
            assertThrows(Of InvalidOperationException)(Sub() conf.getClass("unknown", Nothing))
            assertThrows(Of InvalidOperationException)(Sub() conf.getClasses("unknown"))
            assertThrows(Of TypeLoadException)(Sub() conf.getClassByName("no.such.Class"))
            'classes are found by full name in any loaded assembly, including this one
            assertEquals(GetType(ConfigurableThing), conf.getClassByName(GetType(ConfigurableThing).FullName))
            assertEquals(GetType(Configuration), conf.getClassByName(GetType(Configuration).FullName))

            conf.setStrings("cs", GetType(Configured).FullName, GetType(ConfigurableThing).FullName)
            Dim classes() As Type = conf.getClasses("cs")
            assertArrayEquals(New Type() {GetType(Configured), GetType(ConfigurableThing)}, classes)
            'instances are created and, being Configurable, configured with this configuration
            Dim instances As IList(Of Configurable) = conf.getInstances(Of Configurable)("cs", GetType(Configurable))
            assertEquals(2, instances.Count)
            assertTrue(TypeOf instances(0) Is Configured)
            assertTrue(TypeOf instances(1) Is ConfigurableThing)
            assertTrue(instances(0).getConf() Is conf)
            assertTrue(instances(1).getConf() Is conf)
            assertEquals(1, DirectCast(instances(1), ConfigurableThing).timesConfigured)
            conf.setStrings("notConfigurable", GetType(String).FullName)
            assertThrows(Of InvalidOperationException)(Sub() conf.getInstances(Of Configurable)("notConfigurable", GetType(Configurable)))
        End Sub

        <TestMethod>
        Public Sub testGetFile()
            Dim conf As New Configuration(False)
            Dim dir1 As String = Path.Combine(tmpDir, "one")
            Dim dir2 As String = Path.Combine(tmpDir, "two")
            conf.setStrings("dirs", dir1, dir2)
            'the file is placed under one of the directories, which is created if needed
            Dim located As String = conf.getFile("dirs", "sub/data.bin")
            assertEquals("data.bin", Path.GetFileName(located))
            Dim parent As String = Path.GetFullPath(Path.GetDirectoryName(located))
            assertTrue(parent = Path.Combine(dir1, "sub") OrElse parent = Path.Combine(dir2, "sub"), parent)
            assertTrue(Directory.Exists(parent))
            'the choice is deterministic for a given path (Java String.hashCode)
            assertEquals(located, conf.getFile("dirs", "sub/data.bin"))
            'a single directory is always used
            conf.[set]("single", dir2)
            assertEquals(Path.GetFullPath(Path.Combine(dir2, "x.txt")), Path.GetFullPath(conf.getFile("single", "x.txt")))
            'a directory that cannot be created (the path is an existing file) is an error
            Dim blocker As String = Path.Combine(tmpDir, "blocker")
            System.IO.File.WriteAllText(blocker, "")
            conf.[set]("blocked", Path.Combine(blocker, "inner"))
            assertThrows(Of IOException)(Sub() conf.getFile("blocked", "x.txt"))
        End Sub

        <TestMethod>
        Public Sub testDefaultsAndToString()
            'the default resources are core-default.xml and core-site.xml (not present here)
            Dim conf As New Configuration()
            assertTrue(conf.ToString().StartsWith("Configuration: core-default.xml, core-site.xml", StringComparison.Ordinal), conf.ToString())
            assertNull(conf.[get]("fs.default.name"))
            conf.addResource(xmlStream("x=1"))
            assertTrue(conf.ToString().EndsWith(", System.IO.MemoryStream", StringComparison.Ordinal), conf.ToString())
            assertEquals("1", conf.[get]("x"))

            Dim noDefaults As New Configuration(False)
            assertEquals("Configuration: ", noDefaults.ToString())
            noDefaults.addResource(xmlStream("x=1"))
            noDefaults.addResource(xmlStream("y=2"))
            assertEquals("Configuration: System.IO.MemoryStream, System.IO.MemoryStream", noDefaults.ToString())
        End Sub

        ''' <summary>Configuration is a Writable in Java, but none of the Writable operations are supported.</summary>
        <TestMethod>
        Public Sub testWritableOperationsAreUnsupported()
            Dim conf As New Configuration(False)
            Dim w As DeepLearning4VB.org.datavec.api.writable.Writable = conf
            assertThrows(Of NotSupportedException)(Sub() w.toDouble())
            assertThrows(Of NotSupportedException)(Sub() w.toFloat())
            assertThrows(Of NotSupportedException)(Sub() w.toInt())
            assertThrows(Of NotSupportedException)(Sub() w.toLong())
            assertThrows(Of NotSupportedException)(Sub() w.[getType]())
            assertThrows(Of NotSupportedException)(Sub() w.writeType(Nothing))
            'write and readFields are no-ops
            w.write(Nothing)
            w.readFields(Nothing)
        End Sub

        <TestMethod>
        Public Sub testConfigurableAndConfigured()
            Dim conf As New Configuration(False)
            Dim configured As New Configured()
            assertNull(configured.getConf())
            assertNull(configured.Conf)
            configured.setConf(conf)
            assertTrue(configured.getConf() Is conf)
            assertTrue(configured.Conf Is conf)
            Dim other As New Configuration(False)
            configured.Conf = other
            assertTrue(configured.getConf() Is other)
            assertTrue(New Configured(conf).getConf() Is conf)
            assertNull(New Configured(Nothing).getConf())

            'the interface exposes the same pair
            Dim asConfigurable As Configurable = New ConfigurableThing()
            assertNull(asConfigurable.getConf())
            asConfigurable.setConf(conf)
            assertTrue(asConfigurable.getConf() Is conf)
            assertTrue(asConfigurable.Conf Is conf)
            asConfigurable.Conf = other
            assertTrue(asConfigurable.getConf() Is other)
            assertEquals(2, DirectCast(asConfigurable, ConfigurableThing).timesConfigured)

            'ReflectionUtils.setConf configures Configurables only, and only with a non-null conf
#Disable Warning BC40000 ' ReflectionUtils is obsolete, as @Deprecated in Java
            Dim thing As New ConfigurableThing()
            DeepLearning4VB.org.datavec.api.util.ReflectionUtils.setConf(thing, Nothing)
            assertNull(thing.getConf())
            DeepLearning4VB.org.datavec.api.util.ReflectionUtils.setConf(thing, conf)
            assertTrue(thing.getConf() Is conf)
            DeepLearning4VB.org.datavec.api.util.ReflectionUtils.setConf("not configurable", conf)
            Dim created As ConfigurableThing = DeepLearning4VB.org.datavec.api.util.ReflectionUtils.newInstance(Of ConfigurableThing)(GetType(ConfigurableThing), conf)
            assertTrue(created.getConf() Is conf)
            assertEquals(1, created.timesConfigured)
#Enable Warning BC40000
        End Sub

    End Class

    ''' <summary>A Configurable that counts how often it is configured; created by name in the tests above.</summary>
    Public Class ConfigurableThing
        Inherits Configured

        Public timesConfigured As Integer

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides Sub setConf(ByVal conf As Configuration)
            MyBase.setConf(conf)
            If conf IsNot Nothing Then timesConfigured += 1
        End Sub

    End Class

End Namespace
