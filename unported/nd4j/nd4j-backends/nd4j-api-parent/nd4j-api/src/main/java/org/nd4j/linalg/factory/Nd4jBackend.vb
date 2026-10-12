Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ND4JClassLoading = org.nd4j.common.config.ND4JClassLoading
Imports ND4JEnvironmentVars = org.nd4j.common.config.ND4JEnvironmentVars
Imports ND4JSystemProperties = org.nd4j.common.config.ND4JSystemProperties
Imports Nd4jContext = org.nd4j.context.Nd4jContext
Imports Resource = org.nd4j.common.io.Resource

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

Namespace org.nd4j.linalg.factory


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class Nd4jBackend
	Public MustInherit Class Nd4jBackend

		Public Shared ReadOnly BACKEND_PRIORITY_CPU As Integer

		Private Class ComparatorAnonymousInnerClass2
			Implements IComparer(Of Integer)

			Private ReadOnly outerInstance As Nd4j

			Public Sub New(ByVal outerInstance As Nd4j)
				Me.outerInstance = outerInstance
			End Sub

			Public Function Compare(ByVal o1 As Integer?, ByVal o2 As Integer?) As Integer Implements IComparer(Of Integer).Compare
				If ascending Then
					Return [in].getDouble(o1, colIdx).CompareTo([in].getDouble(o2, colIdx))
				Else
					Return -[in].getDouble(o1, colIdx).CompareTo([in].getDouble(o2, colIdx))
				End If
			End Function
		End Class

		Private Class ComparatorAnonymousInnerClass3
			Implements IComparer(Of Integer)

			Private ReadOnly outerInstance As Nd4j

			Public Sub New(ByVal outerInstance As Nd4j)
				Me.outerInstance = outerInstance
			End Sub

			Public Function Compare(ByVal o1 As Integer?, ByVal o2 As Integer?) As Integer Implements IComparer(Of Integer).Compare
				If ascending Then
					Return [in].getDouble(rowIdx, o1).CompareTo([in].getDouble(rowIdx, o2))
				Else
					Return -[in].getDouble(rowIdx, o1).CompareTo([in].getDouble(rowIdx, o2))
				End If
			End Function
		End Class
		Public Shared ReadOnly BACKEND_PRIORITY_GPU As Integer
		''' @deprecated Use <seealso cref="ND4JEnvironmentVars.BACKEND_DYNAMIC_LOAD_CLASSPATH"/> 
		<Obsolete("Use <seealso cref=""ND4JEnvironmentVars.BACKEND_DYNAMIC_LOAD_CLASSPATH""/>")>
		Public Const DYNAMIC_LOAD_CLASSPATH As String = ND4JEnvironmentVars.BACKEND_DYNAMIC_LOAD_CLASSPATH
		''' @deprecated Use <seealso cref="ND4JSystemProperties.DYNAMIC_LOAD_CLASSPATH_PROPERTY"/> 
		<Obsolete("Use <seealso cref=""ND4JSystemProperties.DYNAMIC_LOAD_CLASSPATH_PROPERTY""/>")>
		Public Const DYNAMIC_LOAD_CLASSPATH_PROPERTY As String = ND4JSystemProperties.DYNAMIC_LOAD_CLASSPATH_PROPERTY
		Private Shared triedDynamicLoad As Boolean = False

		Shared Sub New()
			Dim n As Integer = 0
			Dim s As String = System.Environment.GetEnvironmentVariable(ND4JEnvironmentVars.BACKEND_PRIORITY_CPU)
			If s IsNot Nothing AndAlso s.Length > 0 Then
				Try
					n = Integer.Parse(s)
				Catch e As System.FormatException
					Throw New Exception(e)
				End Try
			End If
			BACKEND_PRIORITY_CPU = n
			Dim n As Integer = 100
			Dim s As String = System.Environment.GetEnvironmentVariable(ND4JEnvironmentVars.BACKEND_PRIORITY_GPU)
			If s IsNot Nothing AndAlso s.Length > 0 Then
				Try
					n = Integer.Parse(s)
				Catch e As System.FormatException
					Throw New Exception(e)
				End Try
			End If
			BACKEND_PRIORITY_GPU = n
		End Sub


		''' <summary>
		''' Returns true if the
		''' backend allows order to be specified
		''' on blas operations (cblas) </summary>
		''' <returns> true if the backend allows
		''' order to be specified on blas operations </returns>
		Public MustOverride Function allowsOrder() As Boolean

		''' <summary>
		''' Gets a priority number for the backend.
		''' 
		''' Backends are loaded in priority order (highest first). </summary>
		''' <returns> a priority number. </returns>
		Public MustOverride ReadOnly Property Priority As Integer

		''' <summary>
		''' Determines whether a given backend is available in the current environment. </summary>
		''' <returns> true if the backend is available; false otherwise. </returns>
		Public MustOverride ReadOnly Property Available As Boolean

		''' <summary>
		''' Returns true if the backend can
		''' run on the os or not
		''' @return
		''' </summary>
		Public MustOverride Function canRun() As Boolean

		''' <summary>
		''' Get the configuration resource
		''' @return
		''' </summary>
		Public MustOverride ReadOnly Property ConfigurationResource As Resource

		''' <summary>
		'''  Get the actual (concrete/implementation) class for standard INDArrays for this backend
		''' </summary>
		Public MustOverride ReadOnly Property NDArrayClass As Type

		Public MustOverride ReadOnly Property Environment As Environment

		''' <summary>
		''' Get the build information of the backend
		''' </summary>
		Public MustOverride Function buildInfo() As String

		''' <summary>
		''' Loads the best available backend.
		''' @return
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static Nd4jBackend load() throws NoAvailableBackendException
		Public Shared Function load() As Nd4jBackend

			Dim logInitProperty As String = System.getProperty(ND4JSystemProperties.LOG_INITIALIZATION, "true")
			Dim logInit As Boolean = Boolean.Parse(logInitProperty)

			Dim backends As IList(Of Nd4jBackend) = New List(Of Nd4jBackend)()
			Dim loader As ServiceLoader(Of Nd4jBackend) = ND4JClassLoading.loadService(GetType(Nd4jBackend))
			Try
'JAVA TO VB CONVERTER NOTE: The variable nd4jBackend was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
				For Each nd4jBackend_Conflict As Nd4jBackend In loader
					backends.Add(nd4jBackend_Conflict)
				Next nd4jBackend_Conflict
			Catch serviceError As ServiceConfigurationError
				' a fatal error due to a syntax or provider construction error.
				' backends mustn't throw an exception during construction.
				Throw New Exception("failed to process available backends", serviceError)
			End Try

			backends.Sort(New ComparatorAnonymousInnerClass())

			For Each backend As Nd4jBackend In backends
				Dim available As Boolean = False
				Dim [error] As String = Nothing
				Try
					available = backend.Available
				Catch e As Exception
					[error] = e.Message
				End Try
				If Not available Then
					If logInit Then
						log.warn("Skipped [{}] backend (unavailable): {}", backend.GetType().Name, [error])
					End If
					Continue For
				End If

				Try
					Nd4jContext.Instance.updateProperties(backend.ConfigurationResource.InputStream)
				Catch e As IOException
					log.error("",e)
				End Try

				If logInit Then
					log.info("Loaded [{}] backend", backend.GetType().Name)
				End If
				Return backend
			Next backend

			'need to dynamically load jars and recall, note that we do this right before the backend loads.
			'An existing backend should take precedence over
			'ones being dynamically discovered.
			'Note that we prioritize jvm properties first, followed by environment variables.
			Dim jarUris() As String
			If System.getProperties().containsKey(ND4JSystemProperties.DYNAMIC_LOAD_CLASSPATH_PROPERTY) AndAlso Not triedDynamicLoad Then
				jarUris = System.getProperties().getProperty(ND4JSystemProperties.DYNAMIC_LOAD_CLASSPATH_PROPERTY).Split(";")
			' Do not call System.getenv(): Accessing all variables requires higher security privileges
			ElseIf System.Environment.GetEnvironmentVariable(ND4JEnvironmentVars.BACKEND_DYNAMIC_LOAD_CLASSPATH) IsNot Nothing AndAlso Not triedDynamicLoad Then
				jarUris = System.Environment.GetEnvironmentVariable(ND4JEnvironmentVars.BACKEND_DYNAMIC_LOAD_CLASSPATH).Split(";")

			Else
				Throw New NoAvailableBackendException("Please ensure that you have an nd4j backend on your classpath. Please see: https://deeplearning4j.konduit.ai/nd4j/backend")
			End If

			triedDynamicLoad = True
			'load all the discoverable uris and try to load the backend again
			For Each uri As String In jarUris
				loadLibrary(New File(uri))
			Next uri

			Return load()
		End Function

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of Nd4jBackend)

			Public Function Compare(ByVal o1 As Nd4jBackend, ByVal o2 As Nd4jBackend) As Integer Implements IComparer(Of Nd4jBackend).Compare
				' high-priority first
				Return o2.Priority - o1.Priority
			End Function
		End Class


		''' <summary>
		''' Adds the supplied Java Archive library to java.class.path. This is benign
		''' if the library is already loaded. </summary>
		''' <param name="jar"> the jar file to add </param>
		''' <exception cref="NoAvailableBackendException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static synchronized void loadLibrary(java.io.File jar) throws NoAvailableBackendException
		Public Shared Sub loadLibrary(ByVal jar As File)
			SyncLock GetType(Nd4jBackend)
				Try
					'We are using reflection here to circumvent encapsulation; addURL is not public
					Dim loader As URLClassLoader = CType(ND4JClassLoading.Nd4jClassloader, URLClassLoader)
					Dim url As java.net.URL = jar.toURI().toURL()
					'Disallow if already loaded
					For Each it As java.net.URL In java.util.Arrays.asList(loader.getURLs())
						If it.Equals(url) Then
							Return
						End If
					Next it
					Dim method As System.Reflection.MethodInfo = GetType(URLClassLoader).getDeclaredMethod("addURL", New Type() {GetType(java.net.URL)})
					method.setAccessible(True) 'promote the method to public access
					method.invoke(loader, New Object() {url})
'JAVA TO VB CONVERTER TODO TASK: The following Java 'multi-catch' could not be converted to a VB exception filter:
				Catch (final java.lang.NoSuchMethodException Or java.lang.IllegalAccessException Or java.net.MalformedURLException Or java.lang.reflect.InvocationTargetException e)
					Throw New NoAvailableBackendException(e)
				End Try
			End SyncLock
		End Sub

		''' 
		''' <summary>
		''' @return </summary>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public Properties getProperties() throws java.io.IOException
		Public Overridable ReadOnly Property Properties As Properties
			Get
				Return Context.Conf
			End Get
		End Property

		''' 
		''' <summary>
		''' @return </summary>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.context.Nd4jContext getContext() throws java.io.IOException
		Public Overridable ReadOnly Property Context As Nd4jContext
			Get
				Return Nd4jContext.Instance
			End Get
		End Property

		Public Overrides Function ToString() As String
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Return Me.GetType().FullName
		End Function

		Public MustOverride Sub logBackendInit()


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("serial") public static class NoAvailableBackendException extends Exception
		Public Class NoAvailableBackendException
			Inherits Exception

			Public Sub New(ByVal s As String)
				MyBase.New(s)
			End Sub

			''' <summary>
			''' Constructs a new exception with the specified cause and a detail
			''' message of <tt>(cause==null ? null : cause.toString())</tt> (which
			''' typically contains the class and detail message of <tt>cause</tt>).
			''' This constructor is useful for exceptions that are little more than
			''' wrappers for other throwables (for example, {@link
			''' PrivilegedActionException}).
			''' </summary>
			''' <param name="cause"> the cause (which is saved for later retrieval by the
			'''              <seealso cref="getCause()"/> method).  (A <tt>null</tt> value is
			'''              permitted, and indicates that the cause is nonexistent or
			'''              unknown.)
			''' @since 1.4 </param>
			Public Sub New(ByVal cause As Exception)
				MyBase.New(cause)
			End Sub
		End Class
	End Class

End Namespace