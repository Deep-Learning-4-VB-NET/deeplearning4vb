Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports System.Threading
Imports Microsoft.VisualBasic
Imports StringUtils = org.apache.commons.lang3.StringUtils
Imports ReflectionUtils = org.datavec.api.util.ReflectionUtils
Imports Writable = org.datavec.api.writable.Writable
Imports WritableType = org.datavec.api.writable.WritableType
Imports JsonFactory = org.nd4j.shade.jackson.core.JsonFactory
Imports JsonGenerator = org.nd4j.shade.jackson.core.JsonGenerator
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
Imports org.w3c.dom
Imports SAXException = org.xml.sax.SAXException

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


	<Serializable>
	Public Class Configuration
		Implements IEnumerable(Of KeyValuePair(Of String, String)), Writable

		Private InstanceFieldsInitialized As Boolean = False

		Private Sub InitializeInstanceFields()
			classLoader_Conflict = Thread.CurrentThread.getContextClassLoader()
			If classLoader_Conflict Is Nothing Then
			classLoader_Conflict = GetType(Configuration).getClassLoader()
			End If
		End Sub

		Private Shared ReadOnly LOG As Logger = LoggerFactory.getLogger(GetType(Configuration))

'JAVA TO VB CONVERTER NOTE: The field quietmode was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private quietmode_Conflict As Boolean = True

		''' <summary>
		''' List of configuration resources.
		''' </summary>
		Private resources As New List(Of Object)()

		''' <summary>
		''' List of configuration parameters marked <b>final</b>.
		''' </summary>
		Private finalParameters As ISet(Of String) = New HashSet(Of String)()

		Private loadDefaults As Boolean = True

		''' <summary>
		''' Configuration objects
		''' </summary>
		Private Shared ReadOnly REGISTRY As New WeakHashMap(Of Configuration, Object)()

		''' <summary>
		''' List of default Resources. Resources are loaded in the order of the list
		''' entries
		''' </summary>
		Private Shared ReadOnly defaultResources As New CopyOnWriteArrayList(Of String)()

		Private Shared ReadOnly CACHE_CLASSES As ConcurrentMap(Of ClassLoader, IDictionary(Of String, Type)) = New ConcurrentDictionary(Of ClassLoader, IDictionary(Of String, Type))()

		''' <summary>
		''' Flag to indicate if the storage of resource which updates a key needs
		''' to be stored for each key
		''' </summary>
		Private storeResource As Boolean

		''' <summary>
		''' Stores the mapping of key to the resource which modifies or loads
		''' the key most recently
		''' </summary>
		Private updatingResource As Dictionary(Of String, String)

		Shared Sub New()
			'print deprecation warning if hadoop-site.xml is found in classpath
			Dim cL As ClassLoader = Thread.CurrentThread.getContextClassLoader()
			If cL Is Nothing Then
				cL = GetType(Configuration).getClassLoader()
			End If
			If cL.getResource("hadoop-site.xml") IsNot Nothing Then
				LOG.warn("DEPRECATED: hadoop-site.xml found in the classpath. " & "Usage of hadoop-site.xml is deprecated. Instead use core-site.xml, " & "mapred-site.xml and hdfs-site.xml to override properties of " & "core-default.xml, mapred-default.xml and hdfs-default.xml " & "respectively")
			End If
			addDefaultResource("core-default.xml")
			addDefaultResource("core-site.xml")
		End Sub

		Private properties As Properties
'JAVA TO VB CONVERTER NOTE: The field overlay was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private overlay_Conflict As Properties
'JAVA TO VB CONVERTER NOTE: The field classLoader was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Private classLoader_Conflict As ClassLoader



		''' <summary>
		''' A new configuration. </summary>
		Public Sub New()
			Me.New(True)
			If Not InstanceFieldsInitialized Then
				InitializeInstanceFields()
				InstanceFieldsInitialized = True
			End If
		End Sub

		''' <summary>
		''' A new configuration where the behavior of reading from the default
		''' resources can be turned off.
		''' 
		''' If the parameter {@code loadDefaults} is false, the new instance
		''' will not load resources from the default files. </summary>
		''' <param name="loadDefaults"> specifies whether to load from the default files </param>
		Public Sub New(ByVal loadDefaults As Boolean)
			If Not InstanceFieldsInitialized Then
				InitializeInstanceFields()
				InstanceFieldsInitialized = True
			End If
			Me.loadDefaults = loadDefaults
			SyncLock GetType(Configuration)
				REGISTRY.put(Me, Nothing)
			End SyncLock
			Me.storeResource = False
		End Sub

		''' <summary>
		''' A new configuration with the same settings and additional facility for
		''' storage of resource to each key which loads or updates
		''' the key most recently </summary>
		''' <param name="other"> the configuration from which to clone settings </param>
		''' <param name="storeResource"> flag to indicate if the storage of resource to
		''' each key is to be stored </param>
		Private Sub New(ByVal other As Configuration, ByVal storeResource As Boolean)
			Me.New(other)
			If Not InstanceFieldsInitialized Then
				InitializeInstanceFields()
				InstanceFieldsInitialized = True
			End If
			Me.loadDefaults = other.loadDefaults
			Me.storeResource = storeResource
			If storeResource Then
				updatingResource = New Dictionary(Of String, String)()
			End If
		End Sub

		''' <summary>
		''' A new configuration with the same settings cloned from another.
		''' </summary>
		''' <param name="other"> the configuration from which to clone settings. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public Configuration(Configuration other)
		Public Sub New(ByVal other As Configuration)
			If Not InstanceFieldsInitialized Then
				InitializeInstanceFields()
				InstanceFieldsInitialized = True
			End If
			Me.resources = CType(other.resources.clone(), ArrayList)
			SyncLock other
				If other.properties IsNot Nothing Then
					Me.properties = CType(other.properties.clone(), Properties)
				End If

				If other.overlay_Conflict IsNot Nothing Then
					Me.overlay_Conflict = CType(other.overlay_Conflict.clone(), Properties)
				End If
			End SyncLock

			Me.finalParameters = New HashSet(Of String)(other.finalParameters)
			SyncLock GetType(Configuration)
				REGISTRY.put(Me, Nothing)
			End SyncLock
		End Sub

		''' <summary>
		''' Add a default resource. Resources are loaded in the order of the resources
		''' added. </summary>
		''' <param name="name"> file name. File should be present in the classpath. </param>
		Public Shared Sub addDefaultResource(ByVal name As String)
			' The lock hierarchy is that we must always lock
			' instances before locking the class. Since reloadConfiguration
			' is synchronized on the instance, we must not call conf.reloadConfiguration
			' while holding a lock on Configuration.class. Otherwise we could deadlock
			' if that conf is attempting to lock the Class
			Dim toReload As List(Of Configuration)
			SyncLock GetType(Configuration)
				If defaultResources.contains(name) Then
					Return
				End If
				defaultResources.add(name)
				' Make a copy so we don't iterate while not holding the lock
				toReload = New List(Of Configuration)(REGISTRY.size())
				toReload.AddRange(REGISTRY.keySet())
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
		''' added resources, unless they were marked <a href="#Final">final</a>.
		''' </summary>
		''' <param name="name"> resource to be added, the classpath is examined for a file
		'''             with that name. </param>
		Public Overridable Sub addResource(ByVal name As String)
			addResourceObject(name)
		End Sub

		''' <summary>
		''' Add a configuration resource.
		''' 
		''' The properties of this resource will override properties of previously
		''' added resources, unless they were marked <a href="#Final">final</a>.
		''' </summary>
		''' <param name="url"> url of the resource to be added, the local filesystem is
		'''            examined directly to find the resource, without referring to
		'''            the classpath. </param>
		Public Overridable Sub addResource(ByVal url As URL)
			addResourceObject(url)
		End Sub


		''' <summary>
		''' Add a configuration resource.
		''' 
		''' The properties of this resource will override properties of previously
		''' added resources, unless they were marked <a href="#Final">final</a>.
		''' </summary>
		''' <param name="in"> InputStream to deserialize the object from. </param>
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

		Private Shared varPat As Pattern = Pattern.compile("\$\{[^\}\$" & ChrW(&H0020).ToString() & "]+\}")

		Private Function substituteVars(ByVal expr As String) As String
			If expr Is Nothing Then
				Return Nothing
			End If
			Dim match As Matcher = varPat.matcher("")
			Dim eval As String = expr
			Dim MAX_SUBST As Integer = 20
			For s As Integer = 0 To MAX_SUBST - 1
				match.reset(eval)
				If Not match.find() Then
					Return eval
				End If
				Dim var As String = match.group()
				var = var.Substring(2, (var.Length - 1) - 2) ' remove ${ .. }
				Dim val As String = Nothing
				Try
					val = System.getProperty(var)
				Catch se As SecurityException
					LOG.warn("Unexpected SecurityException in Configuration", se)
				End Try
				If val Is Nothing Then
					val = getRaw(var)
				End If
				If val Is Nothing Then
					Return eval ' return literal ${var}: var is unbound
				End If
				' substitute
				eval = eval.Substring(0, match.start()) & val & eval.Substring(match.end())
			Next s
			Throw New System.InvalidOperationException("Variable substitution depth too large: " & MAX_SUBST & " " & expr)
		End Function

		''' <summary>
		''' Get the value of the <code>name</code> property, <code>null</code> if
		''' no such property exists.
		''' 
		''' Values are processed for <a href="#VariableExpansion">variable expansion</a>
		''' before being returned.
		''' </summary>
		''' <param name="name"> the property name. </param>
		''' <returns> the value of the <code>name</code> property,
		'''         or null if no such property exists. </returns>
		Public Overridable Function get(ByVal name As String) As String
			Return substituteVars(Props.getProperty(name))
		End Function

		''' <summary>
		''' Get the value of the <code>name</code> property, without doing
		''' <a href="#VariableExpansion">variable expansion</a>.
		''' </summary>
		''' <param name="name"> the property name. </param>
		''' <returns> the value of the <code>name</code> property,
		'''         or null if no such property exists. </returns>
		Public Overridable Function getRaw(ByVal name As String) As String
			Return Props.getProperty(name)
		End Function

		''' <summary>
		''' Get the char value of the <code>name</code> property, <code>null</code> if
		''' no such property exists.
		''' 
		''' Values are processed for <a href="#VariableExpansion">variable expansion</a>
		''' before being returned.
		''' </summary>
		''' <param name="name"> the property name. </param>
		''' <returns> the value of the <code>name</code> property,
		'''         or null if no such property exists. </returns>
		Public Overridable Function getChar(ByVal name As String) As Char
			Return Props.getProperty(name).charAt(0)
		End Function

		''' <summary>
		''' Get the char value of the <code>name</code> property, <code>null</code> if
		''' no such property exists.
		''' 
		''' Values are processed for <a href="#VariableExpansion">variable expansion</a>
		''' before being returned.
		''' </summary>
		''' <param name="name"> the property name. </param>
		''' <returns> the value of the <code>name</code> property,
		'''         or null if no such property exists. </returns>
		Public Overridable Function getChar(ByVal name As String, ByVal defaultValue As Char) As Char
			Return Props.getProperty(name, defaultValue.ToString()).charAt(0)
		End Function

		''' <summary>
		''' Set the <code>value</code> of the <code>name</code> property.
		''' </summary>
		''' <param name="name"> property name. </param>
		''' <param name="value"> property value. </param>
		Public Overridable Sub set(ByVal name As String, ByVal value As String)
			Overlay.setProperty(name, value)
			Props.setProperty(name, value)
		End Sub

		''' <summary>
		''' Sets a property if it is currently unset. </summary>
		''' <param name="name"> the property name </param>
		''' <param name="value"> the new value </param>
		Public Overridable Sub setIfUnset(ByVal name As String, ByVal value As String)
			If get(name) Is Nothing Then
				set(name, value)
			End If
		End Sub

		Private ReadOnly Property Overlay As Properties
			Get
				SyncLock Me
					If overlay_Conflict Is Nothing Then
						overlay_Conflict = New Properties()
					End If
					Return overlay_Conflict
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Get the value of the <code>name</code> property. If no such property
		''' exists, then <code>defaultValue</code> is returned.
		''' </summary>
		''' <param name="name"> property name. </param>
		''' <param name="defaultValue"> default value. </param>
		''' <returns> property value, or <code>defaultValue</code> if the property
		'''         doesn't exist. </returns>
		Public Overridable Function get(ByVal name As String, ByVal defaultValue As String) As String
			Return substituteVars(Props.getProperty(name, defaultValue))
		End Function

		''' <summary>
		''' Get the value of the <code>name</code> property as an <code>int</code>.
		''' 
		''' If no such property exists, or if the specified value is not a valid
		''' <code>int</code>, then <code>defaultValue</code> is returned.
		''' </summary>
		''' <param name="name"> property name. </param>
		''' <param name="defaultValue"> default value. </param>
		''' <returns> property value as an <code>int</code>,
		'''         or <code>defaultValue</code>. </returns>
		Public Overridable Function getInt(ByVal name As String, ByVal defaultValue As Integer) As Integer
			Dim valueString As String = get(name)
			If valueString Is Nothing Then
				Return defaultValue
			End If
			Try
				Dim hexString As String = getHexDigits(valueString)
				If hexString IsNot Nothing Then
					Return Convert.ToInt32(hexString, 16)
				End If
				Return Integer.Parse(valueString)
			Catch e As System.FormatException
				Return defaultValue
			End Try
		End Function

		''' <summary>
		''' Set the value of the <code>name</code> property to an <code>int</code>.
		''' </summary>
		''' <param name="name"> property name. </param>
		''' <param name="value"> <code>int</code> value of the property. </param>
		Public Overridable Sub setInt(ByVal name As String, ByVal value As Integer)
			set(name, Convert.ToString(value))
		End Sub


		''' <summary>
		''' Get the value of the <code>name</code> property as a <code>long</code>.
		''' If no such property is specified, or if the specified value is not a valid
		''' <code>long</code>, then <code>defaultValue</code> is returned.
		''' </summary>
		''' <param name="name"> property name. </param>
		''' <param name="defaultValue"> default value. </param>
		''' <returns> property value as a <code>long</code>,
		'''         or <code>defaultValue</code>. </returns>
		Public Overridable Function getLong(ByVal name As String, ByVal defaultValue As Long) As Long
			Dim valueString As String = get(name)
			If valueString Is Nothing Then
				Return defaultValue
			End If
			Try
				Dim hexString As String = getHexDigits(valueString)
				If hexString IsNot Nothing Then
					Return Convert.ToInt64(hexString, 16)
				End If
				Return Long.Parse(valueString)
			Catch e As System.FormatException
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

		''' <summary>
		''' Set the value of the <code>name</code> property to a <code>long</code>.
		''' </summary>
		''' <param name="name"> property name. </param>
		''' <param name="value"> <code>long</code> value of the property. </param>
		Public Overridable Sub setLong(ByVal name As String, ByVal value As Long)
			set(name, Convert.ToString(value))
		End Sub

		''' <summary>
		''' Get the value of the <code>name</code> property as a <code>float</code>.
		''' If no such property is specified, or if the specified value is not a valid
		''' <code>float</code>, then <code>defaultValue</code> is returned.
		''' </summary>
		''' <param name="name"> property name. </param>
		''' <param name="defaultValue"> default value. </param>
		''' <returns> property value as a <code>float</code>,
		'''         or <code>defaultValue</code>. </returns>
		Public Overridable Function getFloat(ByVal name As String, ByVal defaultValue As Single) As Single
			Dim valueString As String = get(name)
			If valueString Is Nothing Then
				Return defaultValue
			End If
			Try
				Return Single.Parse(valueString)
			Catch e As System.FormatException
				Return defaultValue
			End Try
		End Function

		''' <summary>
		''' Set the value of the <code>name</code> property to a <code>float</code>.
		''' </summary>
		''' <param name="name"> property name. </param>
		''' <param name="value"> property value. </param>
		Public Overridable Sub setFloat(ByVal name As String, ByVal value As Single)
			set(name, Convert.ToString(value))
		End Sub

		''' <summary>
		''' Get the value of the <code>name</code> property as a <code>boolean</code>.
		''' If no such property is specified, or if the specified value is not a valid
		''' <code>boolean</code>, then <code>defaultValue</code> is returned.
		''' </summary>
		''' <param name="name"> property name. </param>
		''' <param name="defaultValue"> default value. </param>
		''' <returns> property value as a <code>boolean</code>,
		'''         or <code>defaultValue</code>. </returns>
		Public Overridable Function getBoolean(ByVal name As String, ByVal defaultValue As Boolean) As Boolean
			Dim valueString As String = get(name)
			Return "true".Equals(valueString) OrElse Not "false".Equals(valueString) AndAlso defaultValue
		End Function

		''' <summary>
		''' Set the value of the <code>name</code> property to a <code>boolean</code>.
		''' </summary>
		''' <param name="name"> property name. </param>
		''' <param name="value"> <code>boolean</code> value of the property. </param>
		Public Overridable Sub setBoolean(ByVal name As String, ByVal value As Boolean)
			set(name, Convert.ToString(value))
		End Sub

		''' <summary>
		''' Set the given property, if it is currently unset. </summary>
		''' <param name="name"> property name </param>
		''' <param name="value"> new value </param>
		Public Overridable Sub setBooleanIfUnset(ByVal name As String, ByVal value As Boolean)
			setIfUnset(name, Convert.ToString(value))
		End Sub

		''' <summary>
		''' Get the value of the <code>name</code> property as a <ocde>Pattern</code>.
		''' If no such property is specified, or if the specified value is not a valid
		''' <code>Pattern</code>, then <code>DefaultValue</code> is returned.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <param name="defaultValue"> default value </param>
		''' <returns> property value as a compiled Pattern, or defaultValue </returns>
		Public Overridable Function getPattern(ByVal name As String, ByVal defaultValue As Pattern) As Pattern
			Dim valString As String = get(name)
			If Nothing Is valString OrElse "".Equals(valString) Then
				Return defaultValue
			End If
			Try
				Return Pattern.compile(valString)
			Catch pse As PatternSyntaxException
				LOG.warn("Regular expression '" & valString & "' for property '" & name & "' not valid. Using default", pse)
				Return defaultValue
			End Try
		End Function

		''' <summary>
		''' Set the given property to <code>Pattern</code>.
		''' If the pattern is passed as null, sets the empty pattern which results in
		''' further calls to getPattern(...) returning the default value.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <param name="pattern"> new value </param>
		Public Overridable Sub setPattern(ByVal name As String, ByVal pattern As Pattern)
			If Nothing Is pattern Then
				set(name, Nothing)
			Else
				set(name, pattern.pattern())
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void write(DataOutput out) throws IOException
		Public Overridable Sub write(ByVal [out] As DataOutput)

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void readFields(DataInput in) throws IOException
		Public Overridable Sub readFields(ByVal [in] As DataInput)

		End Sub

		''' <summary>
		''' A class that represents a set of positive integer ranges. It parses
		''' strings of the form: "2-3,5,7-" where ranges are separated by comma and
		''' the lower/upper bounds are separated by dash. Either the lower or upper
		''' bound may be omitted meaning all values up to or over. So the string
		''' above means 2, 3, 5, and 7, 8, 9, ...
		''' </summary>
		Public Class IntegerRanges
			Private Class Range
				Friend start As Integer
				Friend [end] As Integer
			End Class

			Friend ranges As IList(Of Range) = New List(Of Range)()

			Public Sub New()
			End Sub

			Public Sub New(ByVal newValue As String)
				Dim itr As New StringTokenizer(newValue, ",")
				Do While itr.hasMoreTokens()
					Dim rng As String = itr.nextToken().Trim()
					Dim parts() As String = rng.Split("-", 3)
					If parts.Length < 1 OrElse parts.Length > 2 Then
						Throw New System.ArgumentException("integer range badly formed: " & rng)
					End If
					Dim r As New Range()
					r.start = convertToInt(parts(0), 0)
					If parts.Length = 2 Then
						r.end = convertToInt(parts(1), Integer.MaxValue)
					Else
						r.end = r.start
					End If
					If r.start > r.end Then
						Throw New System.ArgumentException("IntegerRange from " & r.start & " to " & r.end & " is invalid")
					End If
					ranges.Add(r)
				Loop
			End Sub

			''' <summary>
			''' Convert a string to an int treating empty strings as the default value. </summary>
			''' <param name="value"> the string value </param>
			''' <param name="defaultValue"> the value for if the string is empty </param>
			''' <returns> the desired integer </returns>
			Friend Shared Function convertToInt(ByVal value As String, ByVal defaultValue As Integer) As Integer
				Dim trim As String = value.Trim()
				If trim.Length = 0 Then
					Return defaultValue
				End If
				Return Integer.Parse(trim)
			End Function

			''' <summary>
			''' Is the given value in the set of ranges </summary>
			''' <param name="value"> the value to check </param>
			''' <returns> is the value in the ranges? </returns>
			Public Overridable Function isIncluded(ByVal value As Integer) As Boolean
				For Each r As Range In ranges
					If r.start <= value AndAlso value <= r.end Then
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
					result.Append(r.end)
				Next r
				Return result.ToString()
			End Function
		End Class

		''' <summary>
		''' Parse the given attribute as a set of integer ranges </summary>
		''' <param name="name"> the attribute name </param>
		''' <param name="defaultValue"> the default value if it is not set </param>
		''' <returns> a new set of ranges from the configured value </returns>
		Public Overridable Function getRange(ByVal name As String, ByVal defaultValue As String) As IntegerRanges
			Return New IntegerRanges(get(name, defaultValue))
		End Function

		''' <summary>
		''' Get the comma delimited values of the <code>name</code> property as
		''' a collection of <code>String</code>s.
		''' If no such property is specified then empty collection is returned.
		''' <para>
		''' This is an optimized version of <seealso cref="getStrings(String)"/>
		''' 
		''' </para>
		''' </summary>
		''' <param name="name"> property name. </param>
		''' <returns> property value as a collection of <code>String</code>s. </returns>
		Public Overridable Function getStringCollection(ByVal name As String) As ICollection(Of String)
			Dim valueString As String = get(name)
			If valueString Is Nothing Then
				Return Nothing
			End If
			Return java.util.Arrays.asList(StringUtils.Split(valueString, ","))
		End Function

		''' <summary>
		''' Get the comma delimited values of the <code>name</code> property as
		''' an array of <code>String</code>s.
		''' If no such property is specified then <code>null</code> is returned.
		''' </summary>
		''' <param name="name"> property name. </param>
		''' <returns> property value as an array of <code>String</code>s,
		'''         or <code>null</code>. </returns>
		Public Overridable Function getStrings(ByVal name As String) As String()
			Dim valueString As String = get(name)
			Return StringUtils.Split(valueString, ",")
		End Function

		''' <summary>
		''' Get the comma delimited values of the <code>name</code> property as
		''' an array of <code>String</code>s.
		''' If no such property is specified then default value is returned.
		''' </summary>
		''' <param name="name"> property name. </param>
		''' <param name="defaultValue"> The default value </param>
		''' <returns> property value as an array of <code>String</code>s,
		'''         or default value. </returns>
		Public Overridable Function getStrings(ByVal name As String, ParamArray ByVal defaultValue() As String) As String()
			Dim valueString As String = get(name)
			If valueString Is Nothing Then
				Return defaultValue
			Else
				Return StringUtils.Split(valueString, ",")
			End If
		End Function

		''' <summary>
		''' Get the comma delimited values of the <code>name</code> property as
		''' a collection of <code>String</code>s, trimmed of the leading and trailing whitespace.
		''' If no such property is specified then empty <code>Collection</code> is returned.
		''' </summary>
		''' <param name="name"> property name. </param>
		''' <returns> property value as a collection of <code>String</code>s, or empty <code>Collection</code> </returns>
		Public Overridable Function getTrimmedStringCollection(ByVal name As String) As ICollection(Of String)
			Dim valueString As String = get(name)
			If Nothing Is valueString Then
				Return java.util.Collections.emptyList()
			End If
			Return java.util.Arrays.asList(StringUtils.stripAll(StringUtils.Split(valueString, ",")))
		End Function

		''' <summary>
		''' Get the comma delimited values of the <code>name</code> property as
		''' an array of <code>String</code>s, trimmed of the leading and trailing whitespace.
		''' If no such property is specified then an empty array is returned.
		''' </summary>
		''' <param name="name"> property name. </param>
		''' <returns> property value as an array of trimmed <code>String</code>s,
		'''         or empty array. </returns>
		Public Overridable Function getTrimmedStrings(ByVal name As String) As String()
			Dim valueString As String = get(name)
			Return StringUtils.stripAll(StringUtils.Split(valueString, ","))
		End Function

		''' <summary>
		''' Get the comma delimited values of the <code>name</code> property as
		''' an array of <code>String</code>s, trimmed of the leading and trailing whitespace.
		''' If no such property is specified then default value is returned.
		''' </summary>
		''' <param name="name"> property name. </param>
		''' <param name="defaultValue"> The default value </param>
		''' <returns> property value as an array of trimmed <code>String</code>s,
		'''         or default value. </returns>
		Public Overridable Function getTrimmedStrings(ByVal name As String, ParamArray ByVal defaultValue() As String) As String()
			Dim valueString As String = get(name)
			If Nothing Is valueString Then
				Return defaultValue
			Else
				Return StringUtils.stripAll(StringUtils.Split(valueString, ","))
			End If
		End Function

		''' <summary>
		''' Set the array of string values for the <code>name</code> property as
		''' as comma delimited values.
		''' </summary>
		''' <param name="name"> property name. </param>
		''' <param name="values"> The values </param>
		Public Overridable Sub setStrings(ByVal name As String, ParamArray ByVal values() As String)
			set(name, StringUtils.join(values, ","))
		End Sub

		''' <summary>
		''' Load a class by name.
		''' </summary>
		''' <param name="name"> the class name. </param>
		''' <returns> the class object. </returns>
		''' <exception cref="ClassNotFoundException"> if the class is not found. </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public @Class getClassByName(String name) throws ClassNotFoundException
		Public Overridable Function getClassByName(ByVal name As String) As Type
			Dim map As IDictionary(Of String, Type) = CACHE_CLASSES.get(classLoader_Conflict)
			If map Is Nothing Then
				Dim newMap As IDictionary(Of String, Type) = New ConcurrentDictionary(Of String, Type)()
				map = CACHE_CLASSES.putIfAbsent(classLoader_Conflict, newMap)
				If map Is Nothing Then
					map = newMap
				End If
			End If

			Dim clazz As Type = map(name)
			If clazz Is Nothing Then
				clazz = Type.GetType(name, True, classLoader_Conflict)
				If clazz IsNot Nothing Then
					map(name) = clazz
				End If
			End If

			Return clazz
		End Function

		''' <summary>
		''' Get the value of the <code>name</code> property
		''' as an array of <code>Class</code>.
		''' The value of the property specifies a list of comma separated class names.
		''' If no such property is specified, then <code>defaultValue</code> is
		''' returned.
		''' </summary>
		''' <param name="name"> the property name. </param>
		''' <param name="defaultValue"> default value. </param>
		''' <returns> property value as a <code>Class[]</code>,
		'''         or <code>defaultValue</code>. </returns>
		Public Overridable Function getClasses(ByVal name As String, ParamArray ByVal defaultValue() As Type) As Type()
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
			Catch e As ClassNotFoundException
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Get the value of the <code>name</code> property as a <code>Class</code>.
		''' If no such property is specified, then <code>defaultValue</code> is
		''' returned.
		''' </summary>
		''' <param name="name"> the class name. </param>
		''' <param name="defaultValue"> default value. </param>
		''' <returns> property value as a <code>Class</code>,
		'''         or <code>defaultValue</code>. </returns>
		Public Overridable Function getClass(ByVal name As String, ByVal defaultValue As Type) As Type
			Dim valueString As String = get(name)
			If valueString Is Nothing Then
				Return defaultValue
			End If
			Try
				Return getClassByName(valueString)
			Catch e As ClassNotFoundException
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Get the value of the <code>name</code> property as a <code>Class</code>
		''' implementing the interface specified by <code>xface</code>.
		''' 
		''' If no such property is specified, then <code>defaultValue</code> is
		''' returned.
		''' 
		''' An exception is thrown if the returned class does not implement the named
		''' interface.
		''' </summary>
		''' <param name="name"> the class name. </param>
		''' <param name="defaultValue"> default value. </param>
		''' <param name="xface"> the interface implemented by the named class. </param>
		''' <returns> property value as a <code>Class</code>,
		'''         or <code>defaultValue</code>. </returns>
		Public Overridable Function getClass(Of U)(ByVal name As String, ByVal defaultValue As Type, ByVal xface As Type(Of U)) As Type
			Try
				Dim theClass As Type = getClass(name, defaultValue)
				If theClass IsNot Nothing AndAlso Not xface.IsAssignableFrom(theClass) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New Exception(theClass & " not " & xface.FullName)
				ElseIf theClass IsNot Nothing Then
					Return theClass.asSubclass(xface)
				Else
					Return Nothing
				End If
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Get the value of the <code>name</code> property as a <code>List</code>
		''' of objects implementing the interface specified by <code>xface</code>.
		''' 
		''' An exception is thrown if any of the classes does not exist, or if it does
		''' not implement the named interface.
		''' </summary>
		''' <param name="name"> the property name. </param>
		''' <param name="xface"> the interface implemented by the classes named by
		'''        <code>name</code>. </param>
		''' <returns> a <code>List</code> of objects implementing <code>xface</code>. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public <U> List<U> getInstances(String name, @Class<U> xface)
		Public Overridable Function getInstances(Of U)(ByVal name As String, ByVal xface As Type(Of U)) As IList(Of U)
			Dim ret As IList(Of U) = New List(Of U)()
			Dim classes() As Type = getClasses(name)
			For Each cl As Type In classes
				If Not xface.IsAssignableFrom(cl) Then
					Throw New Exception(cl & " does not implement " & xface)
				End If
				ret.Add(CType(ReflectionUtils.newInstance(cl, Me), U))
			Next cl
			Return ret
		End Function

		''' <summary>
		''' Set the value of the <code>name</code> property to the name of a
		''' <code>theClass</code> implementing the given interface <code>xface</code>.
		''' 
		''' An exception is thrown if <code>theClass</code> does not implement the
		''' interface <code>xface</code>.
		''' </summary>
		''' <param name="name"> property name. </param>
		''' <param name="theClass"> property value. </param>
		''' <param name="xface"> the interface implemented by the named class. </param>
		Public Overridable Sub setClass(ByVal name As String, ByVal theClass As Type, ByVal xface As Type)
			If Not xface.IsAssignableFrom(theClass) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New Exception(theClass & " not " & xface.FullName)
			End If
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			set(name, theClass.FullName)
		End Sub



		''' <summary>
		''' Get a local file name under a directory named in <i>dirsProp</i> with
		''' the given <i>path</i>.  If <i>dirsProp</i> contains multiple directories,
		''' then one is chosen based on <i>path</i>'s hash code.  If the selected
		''' directory does not exist, an attempt is made to create it.
		''' </summary>
		''' <param name="dirsProp"> directory in which to locate the file. </param>
		''' <param name="path"> file-path. </param>
		''' <returns> local file under the directory with the given path. </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public File getFile(String dirsProp, String path) throws IOException
		Public Overridable Function getFile(ByVal dirsProp As String, ByVal path As String) As File
			Dim dirs() As String = getStrings(dirsProp)
			Dim hashCode As Integer = path.GetHashCode()
			For i As Integer = 0 To dirs.Length - 1 ' try each local dir
				Dim index As Integer = (hashCode + i And Integer.MaxValue) Mod dirs.Length
				Dim file As New File(dirs(index), path)
				Dim dir As File = file.getParentFile()
				If dir.exists() OrElse dir.mkdirs() Then
					Return file
				End If
			Next i
			Throw New IOException("No valid local directories in property: " & dirsProp)
		End Function

		''' <summary>
		''' Get the <seealso cref="URL"/> for the named resource.
		''' </summary>
		''' <param name="name"> resource name. </param>
		''' <returns> the url for the named resource. </returns>
		Public Overridable Function getResource(ByVal name As String) As URL
			Return classLoader_Conflict.getResource(name)
		End Function

		''' <summary>
		''' Get an input stream attached to the configuration resource with the
		''' given <code>name</code>.
		''' </summary>
		''' <param name="name"> configuration resource name. </param>
		''' <returns> an input stream attached to the resource. </returns>
		Public Overridable Function getConfResourceAsInputStream(ByVal name As String) As Stream
			Try
				Dim url As URL = getResource(name)

				If url Is Nothing Then
					LOG.info(name & " not found")
					Return Nothing
				Else
					LOG.info("found resource " & name & " at " & url)
				End If

				Return url.openStream()
			Catch e As Exception
				Return Nothing
			End Try
		End Function

		''' <summary>
		''' Get a <seealso cref="Reader"/> attached to the configuration resource with the
		''' given <code>name</code>.
		''' </summary>
		''' <param name="name"> configuration resource name. </param>
		''' <returns> a reader attached to the resource. </returns>
		Public Overridable Function getConfResourceAsReader(ByVal name As String) As Reader
			Try
				Dim url As URL = getResource(name)

				If url Is Nothing Then
					LOG.info(name & " not found")
					Return Nothing
				Else
					LOG.info("found resource " & name & " at " & url)
				End If

				Return New StreamReader(url.openStream())
			Catch e As Exception
				Return Nothing
			End Try
		End Function

		Private ReadOnly Property Props As Properties
			Get
				SyncLock Me
					If properties Is Nothing Then
						properties = New Properties()
						loadResources(properties, resources, quietmode_Conflict)
						If overlay_Conflict IsNot Nothing Then
							properties.putAll(overlay_Conflict)
							If storeResource Then
								For Each item As KeyValuePair(Of Object, Object) In overlay_Conflict.entrySet()
									updatingResource(CStr(item.Key)) = "Unknown"
								Next item
							End If
						End If
					End If
					Return properties
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Return the number of keys in the configuration.
		''' </summary>
		''' <returns> number of keys in the configuration. </returns>
		Public Overridable Function size() As Integer
			Return Props.size()
		End Function

		''' <summary>
		''' Clears all keys from the configuration.
		''' </summary>
		Public Overridable Sub clear()
			Props.clear()
			Overlay.clear()
		End Sub

		''' <summary>
		''' Get an <seealso cref="System.Collections.IEnumerator"/> to go through the list of <code>String</code>
		''' key-value pairs in the configuration.
		''' </summary>
		''' <returns> an iterator over the entries. </returns>
		Public Overridable Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, String)) Implements IEnumerator(Of KeyValuePair(Of String, String)).GetEnumerator
			' Get a copy of just the string to string pairs. After the old object
			' methods that allow non-strings to be put into configurations are removed,
			' we could replace properties with a Map<String,String> and get rid of this
			' code.
			Dim result As IDictionary(Of String, String) = New Dictionary(Of String, String)()
			For Each item As KeyValuePair(Of Object, Object) In Props.entrySet()
				If TypeOf item.Key Is String AndAlso TypeOf item.Value Is String Then
					result(CStr(item.Key)) = CStr(item.Value)
				End If
			Next item
			Return result.SetOfKeyValuePairs().GetEnumerator()
		End Function

		Private Sub loadResources(ByVal properties As Properties, ByVal resources As ArrayList, ByVal quiet As Boolean)
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

		Private Sub loadResource(ByVal properties As Properties, ByVal name As Object, ByVal quiet As Boolean)
			Try
				Dim docBuilderFactory As DocumentBuilderFactory = DocumentBuilderFactory.newInstance()
				'ignore all comments inside the xml file
				docBuilderFactory.setIgnoringComments(True)

				'allow includes in the xml file
				docBuilderFactory.setNamespaceAware(True)
				Try
					docBuilderFactory.setXIncludeAware(True)
				Catch e As System.NotSupportedException
					LOG.error("Failed to set setXIncludeAware(true) for parser " & docBuilderFactory & ":" & e, e)
				End Try
				Dim builder As DocumentBuilder = docBuilderFactory.newDocumentBuilder()
				Dim doc As Document = Nothing
				Dim root As Element = Nothing

				If TypeOf name Is URL Then ' an URL resource
					Dim url As URL = DirectCast(name, URL)
					If url IsNot Nothing Then
						If Not quiet Then
							LOG.info("parsing " & url)
						End If
						doc = builder.parse(url.ToString())
					End If
				ElseIf TypeOf name Is String Then ' a CLASSPATH resource
					Dim url As URL = getResource(DirectCast(name, String))
					If url IsNot Nothing Then
						If Not quiet Then
							LOG.info("parsing " & url)
						End If
						doc = builder.parse(url.ToString())
					End If
				ElseIf TypeOf name Is Stream Then
					Try
						doc = builder.parse(DirectCast(name, Stream))
					Finally
						DirectCast(name, Stream).Close()
					End Try
				ElseIf TypeOf name Is Element Then
					root = DirectCast(name, Element)
				End If

				If doc Is Nothing AndAlso root Is Nothing Then
					If quiet Then
						Return
					End If
					Throw New Exception(name & " not found")
				End If

				If root Is Nothing Then
					root = doc.getDocumentElement()
				End If
				If Not "configuration".Equals(root.getTagName()) Then
					LOG.error("bad conf file: top-level element not <configuration>")
				End If
				Dim props As NodeList = root.getChildNodes()
				For i As Integer = 0 To props.getLength() - 1
					Dim propNode As Node = props.item(i)
					If Not (TypeOf propNode Is Element) Then
						Continue For
					End If
					Dim prop As Element = CType(propNode, Element)
					If "configuration".Equals(prop.getTagName()) Then
						loadResource(properties, prop, quiet)
						Continue For
					End If
					If Not "property".Equals(prop.getTagName()) Then
						LOG.warn("bad conf file: element not <property>")
					End If
					Dim fields As NodeList = prop.getChildNodes()
					Dim attr As String = Nothing
					Dim value As String = Nothing
					Dim finalParameter As Boolean = False
					For j As Integer = 0 To fields.getLength() - 1
						Dim fieldNode As Node = fields.item(j)
						If Not (TypeOf fieldNode Is Element) Then
							Continue For
						End If
						Dim field As Element = CType(fieldNode, Element)
						If "name".Equals(field.getTagName()) AndAlso field.hasChildNodes() Then
							attr = CType(field.getFirstChild(), Text).getData().Trim()
						End If
						If "value".Equals(field.getTagName()) AndAlso field.hasChildNodes() Then
							value = CType(field.getFirstChild(), Text).getData()
						End If
						If "final".Equals(field.getTagName()) AndAlso field.hasChildNodes() Then
							finalParameter = "true".Equals(CType(field.getFirstChild(), Text).getData())
						End If
					Next j

					' Ignore this parameter if it has already been marked as 'final'
					If attr IsNot Nothing AndAlso value IsNot Nothing Then
						If Not finalParameters.Contains(attr) Then
							properties.setProperty(attr, value)
							If storeResource Then
								updatingResource(attr) = name.ToString()
							End If
							If finalParameter Then
								finalParameters.Add(attr)
							End If
						Else
							LOG.warn(name & ":a attempt to override final parameter: " & attr & ";  Ignoring.")
						End If
					End If
				Next i

			Catch e As Exception When TypeOf e Is IOException OrElse TypeOf e Is ParserConfigurationException OrElse TypeOf e Is SAXException OrElse TypeOf e Is DOMException
				LOG.error("error parsing conf file: " & e)
				Throw New Exception(e)
			End Try
		End Sub

		''' <summary>
		''' Write out the non-default properties in this configuration to the give
		''' <seealso cref="System.IO.Stream_Output"/>.
		''' </summary>
		''' <param name="out"> the output stream to write to. </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void writeXml(OutputStream out) throws IOException
		Public Overridable Sub writeXml(ByVal [out] As Stream)
			Dim properties As Properties = Props
			Try
				Dim doc As Document = DocumentBuilderFactory.newInstance().newDocumentBuilder().newDocument()
				Dim conf As Element = doc.createElement("configuration")
				doc.appendChild(conf)
				conf.appendChild(doc.createTextNode(vbLf))
				Dim e As System.Collections.IEnumerator = properties.keys()
				Do While e.MoveNext()
					Dim name As String = CStr(e.Current)
					Dim [object] As Object = properties.get(name)
					Dim value As String
					If TypeOf [object] Is String Then
						value = DirectCast([object], String)
					Else
						Continue Do
					End If
					Dim propNode As Element = doc.createElement("property")
					conf.appendChild(propNode)

					Dim nameNode As Element = doc.createElement("name")
					nameNode.appendChild(doc.createTextNode(name))
					propNode.appendChild(nameNode)

					Dim valueNode As Element = doc.createElement("value")
					valueNode.appendChild(doc.createTextNode(value))
					propNode.appendChild(valueNode)

					conf.appendChild(doc.createTextNode(vbLf))
				Loop

				Dim source As New DOMSource(doc)
				Dim result As New StreamResult([out])
				Dim transFactory As TransformerFactory = TransformerFactory.newInstance()
				Dim transformer As Transformer = transFactory.newTransformer()
				transformer.transform(source, result)
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub

		''' <summary>
		'''  Writes out all the parameters and their properties (final and resource) to
		'''  the given <seealso cref="Writer"/>
		'''  The format of the output would be
		'''  { "properties" : [ {key1,value1,key1.isFinal,key1.resource}, {key2,value2,
		'''  key2.isFinal,key2.resource}... ] }
		'''  It does not output the parameters of the configuration object which is
		'''  loaded from an input stream. </summary>
		''' <param name="out"> the Writer to write to </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void dumpConfiguration(Configuration conf, Writer out) throws IOException
		Public Shared Sub dumpConfiguration(ByVal conf As Configuration, ByVal [out] As Writer)
			Dim config As New Configuration(conf, True)
			config.reloadConfiguration()
			Dim dumpFactory As New JsonFactory()
			Dim dumpGenerator As JsonGenerator = dumpFactory.createGenerator([out])
			dumpGenerator.writeStartObject()
			dumpGenerator.writeFieldName("properties")
			dumpGenerator.writeStartArray()
			dumpGenerator.flush()
			For Each item As KeyValuePair(Of Object, Object) In config.Props.entrySet()
				dumpGenerator.writeStartObject()
				dumpGenerator.writeStringField("key", CStr(item.Key))
				dumpGenerator.writeStringField("value", config.get(CStr(item.Key)))
				dumpGenerator.writeBooleanField("isFinal", config.finalParameters.Contains(item.Key))
				dumpGenerator.writeStringField("resource", config.updatingResource(item.Key))
				dumpGenerator.writeEndObject()
			Next item
			dumpGenerator.writeEndArray()
			dumpGenerator.writeEndObject()
			dumpGenerator.flush()
		End Sub

		''' <summary>
		''' Get the <seealso cref="ClassLoader"/> for this job.
		''' </summary>
		''' <returns> the correct class loader. </returns>
		Public Overridable Property ClassLoader As ClassLoader
			Get
				Return classLoader_Conflict
			End Get
			Set(ByVal classLoader As ClassLoader)
				Me.classLoader_Conflict = classLoader
			End Set
		End Property


		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			sb.Append("Configuration: ")
			If loadDefaults Then
				SyncLock GetType(Configuration)
					toString(defaultResources, sb)
				End SyncLock
				If resources.Count > 0 Then
					sb.Append(", ")
				End If
			End If
			toString(resources, sb)
			Return sb.ToString()
		End Function

		Private Sub toString(ByVal resources As System.Collections.IList, ByVal sb As StringBuilder)
'JAVA TO VB CONVERTER WARNING: Unlike Java's ListIterator, enumerators in .NET do not allow altering the collection:
			Dim i As System.Collections.IEnumerator = resources.GetEnumerator()
			Do While i.MoveNext()
				If i.nextIndex() <> 0 Then
					sb.Append(", ")
				End If
				sb.Append(i.Current)
			Loop
		End Sub

		''' <summary>
		''' Set the quietness-mode.
		''' 
		''' In the quiet-mode, error and informational messages might not be logged.
		''' </summary>
		''' <param name="quietmode"> <code>true</code> to set quiet-mode on, <code>false</code>
		'''              to turn it off. </param>
		Public Overridable WriteOnly Property QuietMode As Boolean
			Set(ByVal quietmode As Boolean)
				SyncLock Me
					Me.quietmode_Conflict = quietmode
				End SyncLock
			End Set
		End Property

		''' <summary>
		''' For debugging.  List non-default properties to the terminal and exit. </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void main(String[] args) throws Exception
		Public Shared Sub Main(ByVal args() As String)
			Call (New Configuration()).writeXml(System.out)
		End Sub


		Public Overridable Function toDouble() As Double Implements Writable.toDouble
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function toFloat() As Single Implements Writable.toFloat
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function toInt() As Integer Implements Writable.toInt
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function toLong() As Long Implements Writable.toLong
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function [getType]() As WritableType
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void writeType(DataOutput out) throws IOException
		Public Overridable Sub writeType(ByVal [out] As DataOutput)
			Throw New System.NotSupportedException()
		End Sub
	End Class

End Namespace