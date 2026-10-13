Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.Text
Imports System.Text.Encodings.Web
Imports System.Text.Json
Imports System.Text.RegularExpressions
Imports System.Xml
Imports System.Xml.Linq
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.nd4j.common.collections
Imports DeepLearning4VB.org.nd4j.common.config
Imports DeepLearning4VB.org.slf4j
Imports ClassPathResource = DeepLearning4VB.org.nd4j.common.io.ClassPathResource

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

Namespace org.datavec.api.conf

    ''' <summary>
    ''' Provides access to configuration parameters (the Hadoop-style <c>Configuration</c>).
    '''
    ''' <para>Configurations are specified by resources: XML files of <c>&lt;property&gt;</c> elements with
    ''' <c>&lt;name&gt;</c>, <c>&lt;value&gt;</c> and optional <c>&lt;final&gt;</c> children, below a
    ''' <c>&lt;configuration&gt;</c> root. A resource is named by a classpath resource name (looked up with
    ''' <see cref="ClassPathResource"/>), a <see cref="Uri"/> or a <see cref="Stream"/>. The default resources
    ''' <c>core-default.xml</c> and <c>core-site.xml</c> are loaded first, when they exist.</para>
    '''
    ''' <para>Values may contain <c>${name}</c> references to system properties (<see cref="JavaSystem"/>) or to
    ''' other properties, which are expanded by <see cref="[get]"/>. Properties marked <c>final</c> in a
    ''' resource cannot be overridden by later resources.</para>
    '''
    ''' <para>Differences from the Java original: the XML is parsed with <see cref="XmlReader"/>/<see cref="XDocument"/>
    ''' (comments are ignored, whitespace is kept, DTDs are ignored and XInclude is not supported); class names
    ''' are resolved through <see cref="ND4JClassLoading"/> and there is no class loader property; <c>main</c>
    ''' was dropped; file locations are path strings.</para>
    ''' </summary>
    Public Class Configuration
        Implements IEnumerable(Of KeyValuePair(Of String, String)), writable.Writable

        Private Shared ReadOnly LOG As Logger = LoggerFactory.getLogger(GetType(Configuration))

        Private quietmode As Boolean = True

        ''' <summary>List of configuration resources (String, Uri, Stream or XElement).</summary>
        Private resources As List(Of Object) = New List(Of Object)()

        ''' <summary>List of configuration parameters marked <b>final</b>.</summary>
        Private finalParameters As ISet(Of String) = New HashSet(Of String)()

        Private loadDefaults As Boolean = True

        ''' <summary>Configuration objects</summary>
        Private Shared ReadOnly REGISTRY As New WeakIdentityHashMap(Of Configuration, Object)()

        ''' <summary>List of default Resources. Resources are loaded in the order of the list entries (guarded by the class lock).</summary>
        Private Shared ReadOnly defaultResources As New List(Of String)()

        ''' <summary>Cache of classes loaded by name (the Java original kept one cache per class loader).</summary>
        Private Shared ReadOnly CACHE_CLASSES As New ConcurrentDictionary(Of String, Type)(StringComparer.Ordinal)

        ''' <summary>Flag to indicate if the storage of resource which updates a key needs to be stored for each key</summary>
        Private storeResource As Boolean

        ''' <summary>Stores the mapping of key to the resource which modifies or loads the key most recently</summary>
        Private updatingResource As Dictionary(Of String, String)

        Private properties As Properties
        Private overlay As Properties

        Shared Sub New()
            'print deprecation warning if hadoop-site.xml is found in classpath
            Try
                If New ClassPathResource("hadoop-site.xml").exists() Then
                    LOG.warn("DEPRECATED: hadoop-site.xml found in the classpath. " & "Usage of hadoop-site.xml is deprecated. Instead use core-site.xml, " & "mapred-site.xml and hdfs-site.xml to override properties of " & "core-default.xml, mapred-default.xml and hdfs-default.xml " & "respectively")
                End If
            Catch ex As Exception
                ' a failing resource lookup must not break the type initialisation
            End Try
            addDefaultResource("core-default.xml")
            addDefaultResource("core-site.xml")
        End Sub

        ''' <summary>A new configuration.</summary>
        Public Sub New()
            Me.New(True)
        End Sub

        ''' <summary>
        ''' A new configuration where the behavior of reading from the default
        ''' resources can be turned off.
        '''
        ''' If the parameter <paramref name="loadDefaults"/> is false, the new instance
        ''' will not load resources from the default files.
        ''' </summary>
        ''' <param name="loadDefaults"> specifies whether to load from the default files </param>
        Public Sub New(ByVal loadDefaults As Boolean)
            Me.loadDefaults = loadDefaults
            SyncLock GetType(Configuration)
                REGISTRY.put(Me, Nothing)
            End SyncLock
            Me.storeResource = False
        End Sub

        ''' <summary>
        ''' A new configuration with the same settings and additional facility for
        ''' storage of resource to each key which loads or updates
        ''' the key most recently
        ''' </summary>
        ''' <param name="other"> the configuration from which to clone settings </param>
        ''' <param name="storeResource"> flag to indicate if the storage of resource to each key is to be stored </param>
        Private Sub New(ByVal other As Configuration, ByVal storeResource As Boolean)
            Me.New(other)
            Me.loadDefaults = other.loadDefaults
            Me.storeResource = storeResource
            If storeResource Then
                updatingResource = New Dictionary(Of String, String)(StringComparer.Ordinal)
            End If
        End Sub

        ''' <summary>A new configuration with the same settings cloned from another.</summary>
        ''' <param name="other"> the configuration from which to clone settings. </param>
        Public Sub New(ByVal other As Configuration)
            Me.resources = New List(Of Object)(other.resources)
            SyncLock other
                If other.properties IsNot Nothing Then
                    Me.properties = cloneProperties(other.properties)
                End If
                If other.overlay IsNot Nothing Then
                    Me.overlay = cloneProperties(other.overlay)
                End If
            End SyncLock
            Me.finalParameters = New HashSet(Of String)(other.finalParameters)
            SyncLock GetType(Configuration)
                REGISTRY.put(Me, Nothing)
            End SyncLock
        End Sub

        Private Shared Function cloneProperties(ByVal source As Properties) As Properties
            Dim copy As New Properties()
            copy.putAll(source)
            Return copy
        End Function

        ''' <summary>Add a default resource. Resources are loaded in the order of the resources added.</summary>
        ''' <param name="name"> file name. File should be present in the classpath. </param>
        Public Shared Sub addDefaultResource(ByVal name As String)
            ' The lock hierarchy is that we must always lock
            ' instances before locking the class. Since reloadConfiguration
            ' is synchronized on the instance, we must not call conf.reloadConfiguration
            ' while holding a lock on Configuration.class. Otherwise we could deadlock
            ' if that conf is attempting to lock the Class
            Dim toReload As List(Of Configuration)
            SyncLock GetType(Configuration)
                If defaultResources.Contains(name) Then
                    Return
                End If
                defaultResources.Add(name)
                ' Make a copy so we don't iterate while not holding the lock
                toReload = New List(Of Configuration)(REGISTRY.keySet())
            End SyncLock
            For Each conf As Configuration In toReload
                If conf.loadDefaults Then
                    conf.reloadConfiguration()
                End If
            Next conf
        End Sub

        ''' <summary>
        ''' Add a configuration resource.
        '''
        ''' The properties of this resource will override properties of previously
        ''' added resources, unless they were marked final.
        ''' </summary>
        ''' <param name="name"> resource to be added, the classpath is examined for a file with that name. </param>
        Public Overridable Sub addResource(ByVal name As String)
            addResourceObject(name)
        End Sub

        ''' <summary>
        ''' Add a configuration resource.
        '''
        ''' The properties of this resource will override properties of previously
        ''' added resources, unless they were marked final.
        ''' </summary>
        ''' <param name="url"> url of the resource to be added, the local filesystem is
        '''            examined directly to find the resource, without referring to
        '''            the classpath. </param>
        Public Overridable Sub addResource(ByVal url As Uri)
            addResourceObject(url)
        End Sub

        ''' <summary>
        ''' Add a configuration resource.
        '''
        ''' The properties of this resource will override properties of previously
        ''' added resources, unless they were marked final.
        ''' </summary>
        ''' <param name="in"> stream to deserialize the object from (closed after it has been read). </param>
        Public Overridable Sub addResource(ByVal [in] As Stream)
            addResourceObject([in])
        End Sub

        ''' <summary>
        ''' Reload configuration from previously added resources.
        '''
        ''' This method will clear all the configuration read from the added
        ''' resources, and final parameters. This will make the resources to
        ''' be read again before accessing the values. Values that are added
        ''' via set methods will overlay values read from the resources.
        ''' </summary>
        Public Overridable Sub reloadConfiguration()
            SyncLock Me
                properties = Nothing ' trigger reload
                finalParameters.Clear() ' clear site-limits
            End SyncLock
        End Sub

        Private Sub addResourceObject(ByVal resource As Object)
            SyncLock Me
                resources.Add(resource) ' add to resources
                reloadConfiguration()
            End SyncLock
        End Sub

        Private Shared ReadOnly varPat As New Regex("\$\{[^\}\$ ]+\}", RegexOptions.Compiled)

        Private Function substituteVars(ByVal expr As String) As String
            If expr Is Nothing Then
                Return Nothing
            End If
            Dim eval As String = expr
            Const MAX_SUBST As Integer = 20
            For s As Integer = 0 To MAX_SUBST - 1
                Dim match As Match = varPat.Match(eval)
                If Not match.Success Then
                    Return eval
                End If
                Dim var As String = match.Value
                var = var.Substring(2, var.Length - 3) ' remove ${ .. }
                Dim val As String = Nothing
                Try
                    val = JavaSystem.getProperty(var)
                Catch se As Security.SecurityException
                    LOG.warn("Unexpected SecurityException in Configuration: " & se.Message)
                End Try
                If val Is Nothing Then
                    val = getRaw(var)
                End If
                If val Is Nothing Then
                    Return eval ' return literal ${var}: var is unbound
                End If
                ' substitute
                eval = eval.Substring(0, match.Index) & val & eval.Substring(match.Index + match.Length)
            Next s
            Throw New InvalidOperationException("Variable substitution depth too large: " & MAX_SUBST & " " & expr)
        End Function

        ''' <summary>
        ''' Get the value of the <paramref name="name"/> property, Nothing if no such property exists.
        '''
        ''' Values are processed for variable expansion before being returned.
        ''' </summary>
        ''' <param name="name"> the property name. </param>
        ''' <returns> the value of the <paramref name="name"/> property, or Nothing if no such property exists. </returns>
        Public Overridable Function [get](ByVal name As String) As String
            Return substituteVars(getProps().getProperty(name))
        End Function

        ''' <summary>Get the value of the <paramref name="name"/> property, without doing variable expansion.</summary>
        ''' <param name="name"> the property name. </param>
        ''' <returns> the value of the <paramref name="name"/> property, or Nothing if no such property exists. </returns>
        Public Overridable Function getRaw(ByVal name As String) As String
            Return getProps().getProperty(name)
        End Function

        ''' <summary>Get the first character of the <paramref name="name"/> property (throws if there is no such property).</summary>
        Public Overridable Function getChar(ByVal name As String) As Char
            Dim value As String = getProps().getProperty(name)
            Return value(0)
        End Function

        ''' <summary>Get the first character of the <paramref name="name"/> property, or <paramref name="defaultValue"/> if there is no such property.</summary>
        Public Overridable Function getChar(ByVal name As String, ByVal defaultValue As Char) As Char
            Dim value As String = getProps().getProperty(name, defaultValue.ToString())
            Return value(0)
        End Function

        ''' <summary>Set the <paramref name="value"/> of the <paramref name="name"/> property.</summary>
        ''' <param name="name"> property name. </param>
        ''' <param name="value"> property value. </param>
        Public Overridable Sub [set](ByVal name As String, ByVal value As String)
            getOverlay().setProperty(name, value)
            getProps().setProperty(name, value)
        End Sub

        ''' <summary>Sets a property if it is currently unset.</summary>
        ''' <param name="name"> the property name </param>
        ''' <param name="value"> the new value </param>
        Public Overridable Sub setIfUnset(ByVal name As String, ByVal value As String)
            If [get](name) Is Nothing Then
                [set](name, value)
            End If
        End Sub

        Private Function getOverlay() As Properties
            SyncLock Me
                If overlay Is Nothing Then
                    overlay = New Properties()
                End If
                Return overlay
            End SyncLock
        End Function

        ''' <summary>
        ''' Get the value of the <paramref name="name"/> property. If no such property
        ''' exists, then <paramref name="defaultValue"/> is returned.
        ''' </summary>
        ''' <param name="name"> property name. </param>
        ''' <param name="defaultValue"> default value. </param>
        ''' <returns> property value, or <paramref name="defaultValue"/> if the property doesn't exist. </returns>
        Public Overridable Function [get](ByVal name As String, ByVal defaultValue As String) As String
            Return substituteVars(getProps().getProperty(name, defaultValue))
        End Function

        ''' <summary>
        ''' Get the value of the <paramref name="name"/> property as an <c>int</c>.
        '''
        ''' If no such property exists, or if the specified value is not a valid
        ''' <c>int</c>, then <paramref name="defaultValue"/> is returned. Hexadecimal values
        ''' (<c>0x1F</c>, <c>-0x1F</c>) are accepted.
        ''' </summary>
        ''' <param name="name"> property name. </param>
        ''' <param name="defaultValue"> default value. </param>
        ''' <returns> property value as an <c>int</c>, or <paramref name="defaultValue"/>. </returns>
        Public Overridable Function getInt(ByVal name As String, ByVal defaultValue As Integer) As Integer
            Dim valueString As String = [get](name)
            If valueString Is Nothing Then
                Return defaultValue
            End If
            Try
                Dim hexString As String = getHexDigits(valueString)
                If hexString IsNot Nothing Then
                    Return parseJavaInt(hexString, 16)
                End If
                Return parseJavaInt(valueString, 10)
            Catch e As NumberFormatException
                Return defaultValue
            End Try
        End Function

        ''' <summary>Set the value of the <paramref name="name"/> property to an <c>int</c>.</summary>
        ''' <param name="name"> property name. </param>
        ''' <param name="value"> <c>int</c> value of the property. </param>
        Public Overridable Sub setInt(ByVal name As String, ByVal value As Integer)
            [set](name, value.ToString(CultureInfo.InvariantCulture))
        End Sub

        ''' <summary>
        ''' Get the value of the <paramref name="name"/> property as a <c>long</c>.
        ''' If no such property is specified, or if the specified value is not a valid
        ''' <c>long</c>, then <paramref name="defaultValue"/> is returned.
        ''' </summary>
        ''' <param name="name"> property name. </param>
        ''' <param name="defaultValue"> default value. </param>
        ''' <returns> property value as a <c>long</c>, or <paramref name="defaultValue"/>. </returns>
        Public Overridable Function getLong(ByVal name As String, ByVal defaultValue As Long) As Long
            Dim valueString As String = [get](name)
            If valueString Is Nothing Then
                Return defaultValue
            End If
            Try
                Dim hexString As String = getHexDigits(valueString)
                If hexString IsNot Nothing Then
                    Return parseJavaLong(hexString, 16)
                End If
                Return parseJavaLong(valueString, 10)
            Catch e As NumberFormatException
                Return defaultValue
            End Try
        End Function

        Private Function getHexDigits(ByVal value As String) As String
            Dim negative As Boolean = False
            Dim str As String = value
            Dim hexString As String
            If value.StartsWith("-", StringComparison.Ordinal) Then
                negative = True
                str = value.Substring(1)
            End If
            If str.StartsWith("0x", StringComparison.Ordinal) OrElse str.StartsWith("0X", StringComparison.Ordinal) Then
                hexString = str.Substring(2)
                If negative Then
                    hexString = "-" & hexString
                End If
                Return hexString
            End If
            Return Nothing
        End Function

        ''' <summary>Parses like <c>Long.parseLong(s, radix)</c>: optional sign, digits only, no whitespace; throws <see cref="NumberFormatException"/>.</summary>
        Private Shared Function parseJavaLong(ByVal s As String, ByVal radix As Integer) As Long
            If s Is Nothing Then Throw New NumberFormatException("null")
            Dim i As Integer = 0
            Dim negative As Boolean = False
            If s.Length > 0 AndAlso (s(0) = "-"c OrElse s(0) = "+"c) Then
                negative = (s(0) = "-"c)
                i = 1
            End If
            If i >= s.Length Then Throw New NumberFormatException("For input string: """ & s & """")
            Dim magnitude As Decimal = 0D
            For j As Integer = i To s.Length - 1
                Dim d As Integer = digitValue(s(j), radix)
                If d < 0 Then Throw New NumberFormatException("For input string: """ & s & """")
                magnitude = magnitude * radix + d
                If magnitude > 9223372036854775808D Then Throw New NumberFormatException("For input string: """ & s & """")
            Next j
            If negative Then Return CLng(-magnitude)
            If magnitude > Long.MaxValue Then Throw New NumberFormatException("For input string: """ & s & """")
            Return CLng(magnitude)
        End Function

        ''' <summary>Parses like <c>Integer.parseInt(s, radix)</c>.</summary>
        Private Shared Function parseJavaInt(ByVal s As String, ByVal radix As Integer) As Integer
            Dim l As Long = parseJavaLong(s, radix)
            If l < Integer.MinValue OrElse l > Integer.MaxValue Then Throw New NumberFormatException("For input string: """ & s & """")
            Return CInt(l)
        End Function

        Private Shared Function digitValue(ByVal c As Char, ByVal radix As Integer) As Integer
            Dim d As Integer = -1
            If c >= "0"c AndAlso c <= "9"c Then
                d = AscW(c) - AscW("0"c)
            ElseIf c >= "a"c AndAlso c <= "z"c Then
                d = AscW(c) - AscW("a"c) + 10
            ElseIf c >= "A"c AndAlso c <= "Z"c Then
                d = AscW(c) - AscW("A"c) + 10
            End If
            If d >= radix Then Return -1
            Return d
        End Function

        ''' <summary>Set the value of the <paramref name="name"/> property to a <c>long</c>.</summary>
        ''' <param name="name"> property name. </param>
        ''' <param name="value"> <c>long</c> value of the property. </param>
        Public Overridable Sub setLong(ByVal name As String, ByVal value As Long)
            [set](name, value.ToString(CultureInfo.InvariantCulture))
        End Sub

        ''' <summary>
        ''' Get the value of the <paramref name="name"/> property as a <c>float</c>.
        ''' If no such property is specified, or if the specified value is not a valid
        ''' <c>float</c>, then <paramref name="defaultValue"/> is returned.
        ''' </summary>
        ''' <param name="name"> property name. </param>
        ''' <param name="defaultValue"> default value. </param>
        ''' <returns> property value as a <c>float</c>, or <paramref name="defaultValue"/>. </returns>
        Public Overridable Function getFloat(ByVal name As String, ByVal defaultValue As Single) As Single
            Dim valueString As String = [get](name)
            If valueString Is Nothing Then
                Return defaultValue
            End If
            ' Float.parseFloat: surrounding whitespace is ignored and a type suffix (1.5f, 1.5d) is allowed
            Dim s As String = valueString.Trim()
            If s.Length > 1 AndAlso "fFdD".IndexOf(s(s.Length - 1)) >= 0 AndAlso Char.IsDigit(s(s.Length - 2)) Then
                s = s.Substring(0, s.Length - 1)
            End If
            Dim result As Single
            If Single.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, result) Then
                Return result
            End If
            Return defaultValue
        End Function

        ''' <summary>Set the value of the <paramref name="name"/> property to a <c>float</c> (formatted like <c>Float.toString</c>).</summary>
        ''' <param name="name"> property name. </param>
        ''' <param name="value"> property value. </param>
        Public Overridable Sub setFloat(ByVal name As String, ByVal value As Single)
            [set](name, Arrays.JavaFloatToString(value))
        End Sub

        ''' <summary>
        ''' Get the value of the <paramref name="name"/> property as a <c>boolean</c>.
        ''' If no such property is specified, or if the specified value is not a valid
        ''' <c>boolean</c>, then <paramref name="defaultValue"/> is returned.
        ''' </summary>
        ''' <param name="name"> property name. </param>
        ''' <param name="defaultValue"> default value. </param>
        ''' <returns> property value as a <c>boolean</c>, or <paramref name="defaultValue"/>. </returns>
        Public Overridable Function getBoolean(ByVal name As String, ByVal defaultValue As Boolean) As Boolean
            Dim valueString As String = [get](name)
            Return "true".Equals(valueString) OrElse (Not "false".Equals(valueString) AndAlso defaultValue)
        End Function

        ''' <summary>Set the value of the <paramref name="name"/> property to a <c>boolean</c> (stored as <c>true</c>/<c>false</c>).</summary>
        ''' <param name="name"> property name. </param>
        ''' <param name="value"> <c>boolean</c> value of the property. </param>
        Public Overridable Sub setBoolean(ByVal name As String, ByVal value As Boolean)
            [set](name, If(value, "true", "false"))
        End Sub

        ''' <summary>Set the given property, if it is currently unset.</summary>
        ''' <param name="name"> property name </param>
        ''' <param name="value"> new value </param>
        Public Overridable Sub setBooleanIfUnset(ByVal name As String, ByVal value As Boolean)
            setIfUnset(name, If(value, "true", "false"))
        End Sub

        ''' <summary>
        ''' Get the value of the <paramref name="name"/> property as a <see cref="Regex"/>.
        ''' If no such property is specified, or if the specified value is not a valid
        ''' pattern, then <paramref name="defaultValue"/> is returned.
        ''' </summary>
        ''' <param name="name"> property name </param>
        ''' <param name="defaultValue"> default value </param>
        ''' <returns> property value as a compiled pattern, or defaultValue </returns>
        Public Overridable Function getPattern(ByVal name As String, ByVal defaultValue As Regex) As Regex
            Dim valString As String = [get](name)
            If valString Is Nothing OrElse "".Equals(valString) Then
                Return defaultValue
            End If
            Try
                Return New Regex(valString)
            Catch pse As ArgumentException
                LOG.warn("Regular expression '" & valString & "' for property '" & name & "' not valid. Using default: " & pse.Message)
                Return defaultValue
            End Try
        End Function

        ''' <summary>
        ''' Set the given property to a <see cref="Regex"/>. Like the Java original, passing Nothing is an
        ''' error (the value of a property must not be Nothing).
        ''' </summary>
        ''' <param name="name"> property name </param>
        ''' <param name="pattern"> new value </param>
        Public Overridable Sub setPattern(ByVal name As String, ByVal pattern As Regex)
            If pattern Is Nothing Then
                [set](name, Nothing)
            Else
                [set](name, pattern.ToString())
            End If
        End Sub

        Public Overridable Sub write(ByVal [out] As DataOutput) Implements writable.Writable.write
        End Sub

        Public Overridable Sub readFields(ByVal [in] As DataInput) Implements writable.Writable.readFields
        End Sub

        ''' <summary>
        ''' A class that represents a set of positive integer ranges. It parses
        ''' strings of the form: "2-3,5,7-" where ranges are separated by comma and
        ''' the lower/upper bounds are separated by dash. Either the lower or upper
        ''' bound may be omitted meaning all values up to or over. So the string
        ''' above means 2, 3, 5, and 7, 8, 9, ...
        ''' </summary>
        Public Class IntegerRanges

            Friend Class Range
                Friend start As Integer
                Friend last As Integer
            End Class

            Friend ranges As IList(Of Range) = New List(Of Range)()

            Public Sub New()
            End Sub

            Public Sub New(ByVal newValue As String)
                For Each token As String In newValue.Split(","c)
                    If token.Length = 0 Then Continue For ' StringTokenizer skips empty tokens
                    Dim rng As String = token.Trim()
                    Dim parts() As String = rng.Split("-"c)
                    If parts.Length < 1 OrElse parts.Length > 2 Then
                        Throw New ArgumentException("integer range badly formed: " & rng)
                    End If
                    Dim r As New Range()
                    r.start = convertToInt(parts(0), 0)
                    If parts.Length = 2 Then
                        r.last = convertToInt(parts(1), Integer.MaxValue)
                    Else
                        r.last = r.start
                    End If
                    If r.start > r.last Then
                        Throw New ArgumentException("IntegerRange from " & r.start & " to " & r.last & " is invalid")
                    End If
                    ranges.Add(r)
                Next token
            End Sub

            ''' <summary>Convert a string to an int treating empty strings as the default value.</summary>
            ''' <param name="value"> the string value </param>
            ''' <param name="defaultValue"> the value for if the string is empty </param>
            ''' <returns> the desired integer </returns>
            Friend Shared Function convertToInt(ByVal value As String, ByVal defaultValue As Integer) As Integer
                Dim trim As String = value.Trim()
                If trim.Length = 0 Then
                    Return defaultValue
                End If
                Return parseJavaInt(trim, 10)
            End Function

            ''' <summary>Is the given value in the set of ranges</summary>
            ''' <param name="value"> the value to check </param>
            ''' <returns> is the value in the ranges? </returns>
            Public Overridable Function isIncluded(ByVal value As Integer) As Boolean
                For Each r As Range In ranges
                    If r.start <= value AndAlso value <= r.last Then
                        Return True
                    End If
                Next r
                Return False
            End Function

            Public Overrides Function ToString() As String
                Dim result As New StringBuilder()
                Dim first As Boolean = True
                For Each r As Range In ranges
                    If first Then
                        first = False
                    Else
                        result.Append(","c)
                    End If
                    result.Append(r.start)
                    result.Append("-"c)
                    result.Append(r.last)
                Next r
                Return result.ToString()
            End Function

        End Class

        ''' <summary>Parse the given attribute as a set of integer ranges</summary>
        ''' <param name="name"> the attribute name </param>
        ''' <param name="defaultValue"> the default value if it is not set </param>
        ''' <returns> a new set of ranges from the configured value </returns>
        Public Overridable Function getRange(ByVal name As String, ByVal defaultValue As String) As IntegerRanges
            Return New IntegerRanges([get](name, defaultValue))
        End Function

        ''' <summary>
        ''' Get the comma delimited values of the <paramref name="name"/> property as
        ''' a collection of <c>String</c>s. If no such property is specified then Nothing is returned.
        ''' </summary>
        ''' <param name="name"> property name. </param>
        ''' <returns> property value as a collection of <c>String</c>s. </returns>
        Public Overridable Function getStringCollection(ByVal name As String) As ICollection(Of String)
            Dim valueString As String = [get](name)
            If valueString Is Nothing Then
                Return Nothing
            End If
            Return Arrays.asList(splitCommas(valueString))
        End Function

        ''' <summary>
        ''' Get the comma delimited values of the <paramref name="name"/> property as
        ''' an array of <c>String</c>s. If no such property is specified then Nothing is returned.
        ''' </summary>
        ''' <param name="name"> property name. </param>
        ''' <returns> property value as an array of <c>String</c>s, or Nothing. </returns>
        Public Overridable Function getStrings(ByVal name As String) As String()
            Dim valueString As String = [get](name)
            Return splitCommas(valueString)
        End Function

        ''' <summary>
        ''' Get the comma delimited values of the <paramref name="name"/> property as
        ''' an array of <c>String</c>s. If no such property is specified then default value is returned.
        ''' </summary>
        ''' <param name="name"> property name. </param>
        ''' <param name="defaultValue"> The default value </param>
        ''' <returns> property value as an array of <c>String</c>s, or default value. </returns>
        Public Overridable Function getStrings(ByVal name As String, ByVal ParamArray defaultValue() As String) As String()
            Dim valueString As String = [get](name)
            If valueString Is Nothing Then
                Return defaultValue
            Else
                Return splitCommas(valueString)
            End If
        End Function

        ''' <summary>
        ''' Get the comma delimited values of the <paramref name="name"/> property as
        ''' a collection of <c>String</c>s, trimmed of the leading and trailing whitespace.
        ''' If no such property is specified then an empty collection is returned.
        ''' </summary>
        ''' <param name="name"> property name. </param>
        ''' <returns> property value as a collection of <c>String</c>s, or empty collection </returns>
        Public Overridable Function getTrimmedStringCollection(ByVal name As String) As ICollection(Of String)
            Dim valueString As String = [get](name)
            If valueString Is Nothing Then
                Return JavaCollections.emptyList(Of String)()
            End If
            Return Arrays.asList(stripAll(splitCommas(valueString)))
        End Function

        ''' <summary>
        ''' Get the comma delimited values of the <paramref name="name"/> property as
        ''' an array of <c>String</c>s, trimmed of the leading and trailing whitespace.
        ''' If no such property is specified then Nothing is returned (as the Java code did).
        ''' </summary>
        ''' <param name="name"> property name. </param>
        ''' <returns> property value as an array of trimmed <c>String</c>s, or Nothing. </returns>
        Public Overridable Function getTrimmedStrings(ByVal name As String) As String()
            Dim valueString As String = [get](name)
            Return stripAll(splitCommas(valueString))
        End Function

        ''' <summary>
        ''' Get the comma delimited values of the <paramref name="name"/> property as
        ''' an array of <c>String</c>s, trimmed of the leading and trailing whitespace.
        ''' If no such property is specified then default value is returned.
        ''' </summary>
        ''' <param name="name"> property name. </param>
        ''' <param name="defaultValue"> The default value </param>
        ''' <returns> property value as an array of trimmed <c>String</c>s, or default value. </returns>
        Public Overridable Function getTrimmedStrings(ByVal name As String, ByVal ParamArray defaultValue() As String) As String()
            Dim valueString As String = [get](name)
            If valueString Is Nothing Then
                Return defaultValue
            Else
                Return stripAll(splitCommas(valueString))
            End If
        End Function

        ''' <summary>Set the array of string values for the <paramref name="name"/> property as comma delimited values.</summary>
        ''' <param name="name"> property name. </param>
        ''' <param name="values"> The values </param>
        Public Overridable Sub setStrings(ByVal name As String, ByVal ParamArray values() As String)
            [set](name, If(values Is Nothing, Nothing, String.Join(",", values)))
        End Sub

        ''' <summary>commons-lang <c>StringUtils.split(str, ",")</c>: adjacent separators are treated as one, Nothing in gives Nothing out.</summary>
        Private Shared Function splitCommas(ByVal str As String) As String()
            If str Is Nothing Then Return Nothing
            Return str.Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries)
        End Function

        ''' <summary>commons-lang <c>StringUtils.stripAll</c>: trims every element, Nothing in gives Nothing out.</summary>
        Private Shared Function stripAll(ByVal strs() As String) As String()
            If strs Is Nothing Then Return Nothing
            Dim result(strs.Length - 1) As String
            For i As Integer = 0 To strs.Length - 1
                result(i) = If(strs(i) Is Nothing, Nothing, strs(i).Trim())
            Next i
            Return result
        End Function

        ''' <summary>Load a class by name, through <see cref="ND4JClassLoading"/>.</summary>
        ''' <param name="name"> the class name. </param>
        ''' <returns> the class object. </returns>
        ''' <exception cref="TypeLoadException"> if the class is not found (<c>ClassNotFoundException</c> in Java). </exception>
        Public Overridable Function getClassByName(ByVal name As String) As Type
            Dim clazz As Type = Nothing
            If CACHE_CLASSES.TryGetValue(name, clazz) Then
                Return clazz
            End If
            clazz = ND4JClassLoading.loadClassByName(name)
            If clazz Is Nothing Then
                Throw New TypeLoadException("Class not found: " & name)
            End If
            CACHE_CLASSES(name) = clazz
            Return clazz
        End Function

        ''' <summary>
        ''' Get the value of the <paramref name="name"/> property as an array of <see cref="Type"/>.
        ''' The value of the property specifies a list of comma separated class names.
        ''' If no such property is specified, then <paramref name="defaultValue"/> is returned.
        ''' </summary>
        ''' <param name="name"> the property name. </param>
        ''' <param name="defaultValue"> default value. </param>
        ''' <returns> property value as a <c>Type()</c>, or <paramref name="defaultValue"/>. </returns>
        Public Overridable Function getClasses(ByVal name As String, ByVal ParamArray defaultValue() As Type) As Type()
            Dim classnames() As String = getStrings(name)
            If classnames Is Nothing Then
                Return defaultValue
            End If
            Try
                Dim classes(classnames.Length - 1) As Type
                For i As Integer = 0 To classnames.Length - 1
                    classes(i) = getClassByName(classnames(i))
                Next i
                Return classes
            Catch e As TypeLoadException
                Throw New InvalidOperationException(e.Message, e)
            End Try
        End Function

        ''' <summary>
        ''' Get the value of the <paramref name="name"/> property as a <see cref="Type"/>.
        ''' If no such property is specified, then <paramref name="defaultValue"/> is returned.
        ''' </summary>
        ''' <param name="name"> the class name. </param>
        ''' <param name="defaultValue"> default value. </param>
        ''' <returns> property value as a <see cref="Type"/>, or <paramref name="defaultValue"/>. </returns>
        Public Overridable Function getClass(ByVal name As String, ByVal defaultValue As Type) As Type
            Dim valueString As String = [get](name)
            If valueString Is Nothing Then
                Return defaultValue
            End If
            Try
                Return getClassByName(valueString)
            Catch e As TypeLoadException
                Throw New InvalidOperationException(e.Message, e)
            End Try
        End Function

        ''' <summary>
        ''' Get the value of the <paramref name="name"/> property as a <see cref="Type"/>
        ''' implementing the interface specified by <paramref name="xface"/>.
        '''
        ''' If no such property is specified, then <paramref name="defaultValue"/> is returned.
        '''
        ''' An exception is thrown if the returned class does not implement the named interface.
        ''' </summary>
        ''' <param name="name"> the class name. </param>
        ''' <param name="defaultValue"> default value. </param>
        ''' <param name="xface"> the interface implemented by the named class. </param>
        ''' <returns> property value as a <see cref="Type"/>, or <paramref name="defaultValue"/>. </returns>
        Public Overridable Function getClass(ByVal name As String, ByVal defaultValue As Type, ByVal xface As Type) As Type
            Try
                Dim theClass As Type = getClass(name, defaultValue)
                If theClass IsNot Nothing AndAlso Not xface.IsAssignableFrom(theClass) Then
                    Throw New InvalidOperationException(theClass.ToString() & " not " & xface.FullName)
                ElseIf theClass IsNot Nothing Then
                    Return theClass
                Else
                    Return Nothing
                End If
            Catch e As InvalidOperationException
                Throw
            Catch e As Exception
                Throw New InvalidOperationException(e.Message, e)
            End Try
        End Function

        ''' <summary>
        ''' Get the value of the <paramref name="name"/> property as a list of objects implementing the
        ''' interface specified by <paramref name="xface"/>. Each class is instantiated with its parameterless
        ''' constructor and, when it is <see cref="Configurable"/>, configured with this configuration.
        '''
        ''' An exception is thrown if any of the classes does not exist, or if it does not implement the named interface.
        ''' </summary>
        ''' <param name="name"> the property name. </param>
        ''' <param name="xface"> the interface implemented by the classes named by <paramref name="name"/>. </param>
        ''' <returns> a list of objects implementing <paramref name="xface"/>. </returns>
        Public Overridable Function getInstances(Of U)(ByVal name As String, ByVal xface As Type) As IList(Of U)
            Dim ret As IList(Of U) = New List(Of U)()
            Dim classes() As Type = getClasses(name)
            For Each cl As Type In classes
                If Not xface.IsAssignableFrom(cl) Then
                    Throw New InvalidOperationException(cl.ToString() & " does not implement " & xface.ToString())
                End If
                ret.Add(CType(newInstance(cl), U))
            Next cl
            Return ret
        End Function

        ''' <summary>Replaces <c>ReflectionUtils.newInstance(theClass, conf)</c>: parameterless construction plus <see cref="Configurable.setConf"/>.</summary>
        Private Function newInstance(ByVal theClass As Type) As Object
            Dim result As Object = Activator.CreateInstance(theClass, True)
            If TypeOf result Is Configurable Then
                DirectCast(result, Configurable).setConf(Me)
            End If
            Return result
        End Function

        ''' <summary>
        ''' Set the value of the <paramref name="name"/> property to the name of a
        ''' <paramref name="theClass"/> implementing the given interface <paramref name="xface"/>.
        '''
        ''' An exception is thrown if <paramref name="theClass"/> does not implement the interface <paramref name="xface"/>.
        ''' </summary>
        ''' <param name="name"> property name. </param>
        ''' <param name="theClass"> property value. </param>
        ''' <param name="xface"> the interface implemented by the named class. </param>
        Public Overridable Sub setClass(ByVal name As String, ByVal theClass As Type, ByVal xface As Type)
            If Not xface.IsAssignableFrom(theClass) Then
                Throw New InvalidOperationException(theClass.ToString() & " not " & xface.FullName)
            End If
            [set](name, theClass.FullName)
        End Sub

        ''' <summary>
        ''' Get a local file name under a directory named in <paramref name="dirsProp"/> with
        ''' the given <paramref name="path"/>.  If <paramref name="dirsProp"/> contains multiple directories,
        ''' then one is chosen based on <paramref name="path"/>'s (Java) hash code.  If the selected
        ''' directory does not exist, an attempt is made to create it.
        ''' </summary>
        ''' <param name="dirsProp"> directory in which to locate the file. </param>
        ''' <param name="path"> file-path. </param>
        ''' <returns> local file under the directory with the given path. </returns>
        Public Overridable Function getFile(ByVal dirsProp As String, ByVal path As String) As String
            Dim dirs() As String = getStrings(dirsProp)
            Dim hashCode As Integer = javaStringHashCode(path)
            For i As Integer = 0 To dirs.Length - 1 ' try each local dir
                Dim index As Integer = ((hashCode + i) And Integer.MaxValue) Mod dirs.Length
                Dim file As String = System.IO.Path.Combine(dirs(index), path)
                Dim dir As String = System.IO.Path.GetDirectoryName(file)
                If Directory.Exists(dir) OrElse mkdirs(dir) Then
                    Return file
                End If
            Next i
            Throw New IOException("No valid local directories in property: " & dirsProp)
        End Function

        Private Shared Function mkdirs(ByVal dir As String) As Boolean
            Try
                Directory.CreateDirectory(dir)
                Return True
            Catch e As Exception
                Return False
            End Try
        End Function

        ''' <summary><c>String.hashCode()</c> as Java computes it (wrapping arithmetic).</summary>
        Private Shared Function javaStringHashCode(ByVal s As String) As Integer
            Dim h As Integer = 0
            For Each c As Char In s
                h = 31 * h + AscW(c)
            Next c
            Return h
        End Function

        ''' <summary>
        ''' Get the <see cref="Uri"/> for the named resource: a file found through <see cref="ClassPathResource"/>
        ''' (search directories, then embedded manifest resources extracted to a temporary file).
        ''' </summary>
        ''' <param name="name"> resource name. </param>
        ''' <returns> the url for the named resource, or Nothing when it does not exist. </returns>
        Public Overridable Function getResource(ByVal name As String) As Uri
            Try
                Dim res As New ClassPathResource(name)
                If Not res.exists() Then
                    Return Nothing
                End If
                Return New Uri(System.IO.Path.GetFullPath(res.File))
            Catch e As Exception
                Return Nothing
            End Try
        End Function

        ''' <summary>Get an input stream attached to the configuration resource with the given <paramref name="name"/>.</summary>
        ''' <param name="name"> configuration resource name. </param>
        ''' <returns> an input stream attached to the resource, or Nothing. </returns>
        Public Overridable Function getConfResourceAsInputStream(ByVal name As String) As Stream
            Try
                Dim url As Uri = getResource(name)
                If url Is Nothing Then
                    LOG.info(name & " not found")
                    Return Nothing
                Else
                    LOG.info("found resource " & name & " at " & url.ToString())
                End If
                Return openStream(url)
            Catch e As Exception
                Return Nothing
            End Try
        End Function

        ''' <summary>Get a <see cref="TextReader"/> attached to the configuration resource with the given <paramref name="name"/>.</summary>
        ''' <param name="name"> configuration resource name. </param>
        ''' <returns> a reader attached to the resource, or Nothing. </returns>
        Public Overridable Function getConfResourceAsReader(ByVal name As String) As TextReader
            Try
                Dim url As Uri = getResource(name)
                If url Is Nothing Then
                    LOG.info(name & " not found")
                    Return Nothing
                Else
                    LOG.info("found resource " & name & " at " & url.ToString())
                End If
                Return New StreamReader(openStream(url))
            Catch e As Exception
                Return Nothing
            End Try
        End Function

        Private Shared Function openStream(ByVal url As Uri) As Stream
            If url.IsFile Then
                Return New FileStream(url.LocalPath, FileMode.Open, FileAccess.Read)
            End If
            Return WebRequest.Create(url).GetResponse().GetResponseStream()
        End Function

        Private Function getProps() As Properties
            SyncLock Me
                If properties Is Nothing Then
                    properties = New Properties()
                    loadResources(properties, resources, quietmode)
                    If overlay IsNot Nothing Then
                        properties.putAll(overlay)
                        If storeResource Then
                            For Each key As String In overlay.stringPropertyNames()
                                updatingResource(key) = "Unknown"
                            Next key
                        End If
                    End If
                End If
                Return properties
            End SyncLock
        End Function

        ''' <summary>Return the number of keys in the configuration.</summary>
        ''' <returns> number of keys in the configuration. </returns>
        Public Overridable Function size() As Integer
            Return getProps().size()
        End Function

        ''' <summary>Clears all keys from the configuration.</summary>
        Public Overridable Sub clear()
            getProps().clear()
            getOverlay().clear()
        End Sub

        ''' <summary>
        ''' Get an enumerator over the <c>String</c> key-value pairs in the configuration (a snapshot).
        ''' </summary>
        ''' <returns> an enumerator over the entries. </returns>
        Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, String)) Implements IEnumerable(Of KeyValuePair(Of String, String)).GetEnumerator
            ' Get a copy of the string to string pairs.
            Dim props As Properties = getProps()
            Dim result As IDictionary(Of String, String) = New Dictionary(Of String, String)(StringComparer.Ordinal)
            For Each key As String In props.stringPropertyNames()
                result(key) = props.getProperty(key)
            Next key
            Return result.GetEnumerator()
        End Function

        Private Function GetEnumeratorNonGeneric() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function

        Private Sub loadResources(ByVal properties As Properties, ByVal resources As List(Of Object), ByVal quiet As Boolean)
            If loadDefaults Then
                ' To avoid addResource causing a ConcurrentModificationException
                Dim toLoad As List(Of String)
                SyncLock GetType(Configuration)
                    toLoad = New List(Of String)(defaultResources)
                End SyncLock
                For Each resource As String In toLoad
                    loadResource(properties, resource, quiet)
                Next resource

                'support the hadoop-site.xml as a deprecated case
                If getResource("hadoop-site.xml") IsNot Nothing Then
                    loadResource(properties, "hadoop-site.xml", quiet)
                End If
            End If

            For Each resource As Object In resources
                loadResource(properties, resource, quiet)
            Next resource
        End Sub

        Private Shared Function readerSettings() As XmlReaderSettings
            Dim settings As New XmlReaderSettings()
            'ignore all comments inside the xml file
            settings.IgnoreComments = True
            settings.IgnoreWhitespace = False
            settings.IgnoreProcessingInstructions = True
            settings.DtdProcessing = DtdProcessing.Ignore
            Return settings
        End Function

        Private Shared Function parseXml(ByVal url As Uri) As XDocument
            Using reader As XmlReader = XmlReader.Create(url.AbsoluteUri, readerSettings())
                Return XDocument.Load(reader, LoadOptions.PreserveWhitespace)
            End Using
        End Function

        Private Shared Function parseXml(ByVal stream As Stream) As XDocument
            Using reader As XmlReader = XmlReader.Create(stream, readerSettings())
                Return XDocument.Load(reader, LoadOptions.PreserveWhitespace)
            End Using
        End Function

        ''' <summary>Java DOM <c>hasChildNodes()</c> on a document parsed with comments ignored.</summary>
        Private Shared Function hasChildNodes(ByVal element As XElement) As Boolean
            Return element.Nodes().Any(Function(n) Not (TypeOf n Is XComment OrElse TypeOf n Is XProcessingInstruction))
        End Function

        ''' <summary>Java <c>((Text) element.getFirstChild()).getData()</c>.</summary>
        Private Shared Function firstTextData(ByVal element As XElement) As String
            Dim first As XNode = element.Nodes().First(Function(n) Not (TypeOf n Is XComment OrElse TypeOf n Is XProcessingInstruction))
            Dim text As XText = TryCast(first, XText)
            If text Is Nothing Then
                Throw New InvalidCastException("Element <" & element.Name.LocalName & "> does not start with text")
            End If
            Return text.Value
        End Function

        Private Sub loadResource(ByVal properties As Properties, ByVal name As Object, ByVal quiet As Boolean)
            Try
                Dim doc As XDocument = Nothing
                Dim root As XElement = Nothing

                If TypeOf name Is Uri Then ' an URL resource
                    Dim url As Uri = DirectCast(name, Uri)
                    If Not quiet Then
                        LOG.info("parsing " & url.ToString())
                    End If
                    doc = parseXml(url)
                ElseIf TypeOf name Is String Then ' a CLASSPATH resource
                    Dim url As Uri = getResource(DirectCast(name, String))
                    If url IsNot Nothing Then
                        If Not quiet Then
                            LOG.info("parsing " & url.ToString())
                        End If
                        doc = parseXml(url)
                    End If
                ElseIf TypeOf name Is Stream Then
                    Try
                        doc = parseXml(DirectCast(name, Stream))
                    Finally
                        DirectCast(name, Stream).Close()
                    End Try
                ElseIf TypeOf name Is XElement Then
                    root = DirectCast(name, XElement)
                End If

                If doc Is Nothing AndAlso root Is Nothing Then
                    If quiet Then
                        Return
                    End If
                    Throw New InvalidOperationException(Objects.toString(name) & " not found")
                End If

                If root Is Nothing Then
                    root = doc.Root
                End If
                If Not "configuration".Equals(root.Name.LocalName) Then
                    LOG.error("bad conf file: top-level element not <configuration>")
                End If
                For Each prop As XElement In root.Elements()
                    If "configuration".Equals(prop.Name.LocalName) Then
                        loadResource(properties, prop, quiet)
                        Continue For
                    End If
                    If Not "property".Equals(prop.Name.LocalName) Then
                        LOG.warn("bad conf file: element not <property>")
                    End If
                    Dim attr As String = Nothing
                    Dim value As String = Nothing
                    Dim finalParameter As Boolean = False
                    For Each field As XElement In prop.Elements()
                        Dim tag As String = field.Name.LocalName
                        If "name".Equals(tag) AndAlso hasChildNodes(field) Then
                            attr = firstTextData(field).Trim()
                        End If
                        If "value".Equals(tag) AndAlso hasChildNodes(field) Then
                            value = firstTextData(field)
                        End If
                        If "final".Equals(tag) AndAlso hasChildNodes(field) Then
                            finalParameter = "true".Equals(firstTextData(field))
                        End If
                    Next field

                    ' Ignore this parameter if it has already been marked as 'final'
                    If attr IsNot Nothing AndAlso value IsNot Nothing Then
                        If Not finalParameters.Contains(attr) Then
                            properties.setProperty(attr, value)
                            If storeResource Then
                                updatingResource(attr) = Objects.toString(name)
                            End If
                            If finalParameter Then
                                finalParameters.Add(attr)
                            End If
                        Else
                            LOG.warn(Objects.toString(name) & ":a attempt to override final parameter: " & attr & ";  Ignoring.")
                        End If
                    End If
                Next prop

            Catch e As Exception When TypeOf e Is IOException OrElse TypeOf e Is XmlException OrElse TypeOf e Is WebException OrElse TypeOf e Is UnauthorizedAccessException
                LOG.error("error parsing conf file: " & e.ToString())
                Throw New InvalidOperationException(e.Message, e)
            End Try
        End Sub

        ''' <summary>
        ''' Write out the non-default properties in this configuration to the given <see cref="Stream"/>
        ''' as an XML document (UTF-8, one <c>&lt;property&gt;</c> per line as the Java transformer wrote it).
        ''' </summary>
        ''' <param name="out"> the output stream to write to. </param>
        Public Overridable Sub writeXml(ByVal [out] As Stream)
            Dim props As Properties = getProps()
            Try
                Dim conf As New XElement("configuration")
                Dim doc As New XDocument(New XDeclaration("1.0", "UTF-8", "no"), conf)
                conf.Add(New XText(vbLf))
                For Each name As String In props.stringPropertyNames()
                    Dim value As String = props.getProperty(name)
                    If value Is Nothing Then
                        Continue For
                    End If
                    Dim propNode As New XElement("property")
                    conf.Add(propNode)

                    Dim nameNode As New XElement("name")
                    nameNode.Add(New XText(name))
                    propNode.Add(nameNode)

                    Dim valueNode As New XElement("value")
                    valueNode.Add(New XText(value))
                    propNode.Add(valueNode)

                    conf.Add(New XText(vbLf))
                Next name

                Dim settings As New XmlWriterSettings()
                settings.Encoding = New UTF8Encoding(False)
                settings.OmitXmlDeclaration = False
                settings.Indent = False
                settings.NewLineHandling = NewLineHandling.None
                settings.CloseOutput = False
                Using writer As XmlWriter = XmlWriter.Create([out], settings)
                    doc.Save(writer)
                End Using
                [out].Flush()
            Catch e As Exception
                Throw New InvalidOperationException(e.Message, e)
            End Try
        End Sub

        ''' <summary>
        '''  Writes out all the parameters and their properties (final and resource) to
        '''  the given <see cref="TextWriter"/>. The format of the output is JSON:
        '''  <c>{ "properties" : [ {key1,value1,key1.isFinal,key1.resource}, {key2,value2, key2.isFinal,key2.resource}... ] }</c>
        '''  It does not output the parameters of the configuration object which is loaded from an input stream.
        ''' </summary>
        ''' <param name="conf"> the configuration to dump </param>
        ''' <param name="out"> the writer to write to </param>
        Public Shared Sub dumpConfiguration(ByVal conf As Configuration, ByVal [out] As TextWriter)
            Dim config As New Configuration(conf, True)
            config.reloadConfiguration()
            Dim options As New JsonWriterOptions()
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            Using ms As New MemoryStream()
                Using dumpGenerator As New Utf8JsonWriter(ms, options)
                    dumpGenerator.WriteStartObject()
                    dumpGenerator.WritePropertyName("properties")
                    dumpGenerator.WriteStartArray()
                    Dim props As Properties = config.getProps()
                    For Each key As String In props.stringPropertyNames()
                        dumpGenerator.WriteStartObject()
                        dumpGenerator.WriteString("key", key)
                        dumpGenerator.WriteString("value", config.[get](key))
                        dumpGenerator.WriteBoolean("isFinal", config.finalParameters.Contains(key))
                        Dim resource As String = Nothing
                        config.updatingResource.TryGetValue(key, resource)
                        If resource Is Nothing Then
                            dumpGenerator.WriteNull("resource")
                        Else
                            dumpGenerator.WriteString("resource", resource)
                        End If
                        dumpGenerator.WriteEndObject()
                    Next key
                    dumpGenerator.WriteEndArray()
                    dumpGenerator.WriteEndObject()
                    dumpGenerator.Flush()
                End Using
                [out].Write(Encoding.UTF8.GetString(ms.ToArray()))
            End Using
            [out].Flush()
        End Sub

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.Append("Configuration: ")
            If loadDefaults Then
                SyncLock GetType(Configuration)
                    appendResources(defaultResources, sb)
                End SyncLock
                If resources.Count > 0 Then
                    sb.Append(", ")
                End If
            End If
            appendResources(resources, sb)
            Return sb.ToString()
        End Function

        Private Shared Sub appendResources(ByVal resources As IEnumerable, ByVal sb As StringBuilder)
            Dim first As Boolean = True
            For Each resource As Object In resources
                If Not first Then
                    sb.Append(", ")
                End If
                sb.Append(Objects.toString(resource))
                first = False
            Next resource
        End Sub

        ''' <summary>
        ''' Set the quietness-mode.
        '''
        ''' In the quiet-mode, error and informational messages might not be logged,
        ''' and resources that cannot be found are skipped instead of being an error.
        ''' </summary>
        ''' <param name="quietmode"> <c>true</c> to set quiet-mode on, <c>false</c> to turn it off. </param>
        Public Overridable Sub setQuietMode(ByVal quietmode As Boolean)
            SyncLock Me
                Me.quietmode = quietmode
            End SyncLock
        End Sub

        Public Overridable Function toDouble() As Double Implements writable.Writable.toDouble
            Throw New NotSupportedException()
        End Function

        Public Overridable Function toFloat() As Single Implements writable.Writable.toFloat
            Throw New NotSupportedException()
        End Function

        Public Overridable Function toInt() As Integer Implements writable.Writable.toInt
            Throw New NotSupportedException()
        End Function

        Public Overridable Function toLong() As Long Implements writable.Writable.toLong
            Throw New NotSupportedException()
        End Function

        ''' <summary><c>Writable.getType()</c>; implemented under another name so that it does not shadow <see cref="Object.GetType"/>.</summary>
        Private Function getWritableType() As writable.WritableType Implements writable.Writable.getType
            Throw New NotSupportedException()
        End Function

        Public Overridable Sub writeType(ByVal [out] As DataOutput) Implements writable.Writable.writeType
            Throw New NotSupportedException()
        End Sub

    End Class

End Namespace
