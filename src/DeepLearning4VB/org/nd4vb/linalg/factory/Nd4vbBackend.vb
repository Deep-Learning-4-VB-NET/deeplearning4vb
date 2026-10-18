Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Reflection
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory
Imports ND4VBClassLoading = DeepLearning4VB.org.nd4vb.common.config.ND4VBClassLoading
Imports ND4VBEnvironmentVars = DeepLearning4VB.org.nd4vb.common.config.ND4VBEnvironmentVars
Imports ND4VBSystemProperties = DeepLearning4VB.org.nd4vb.common.config.ND4VBSystemProperties
Imports Resource = DeepLearning4VB.org.nd4vb.common.io.Resource
Imports PropertiesT = DeepLearning4VB.JavaCompat.Properties
Imports EnvironmentT = DeepLearning4VB.org.nd4vb.linalg.factory.Environment

Namespace org.nd4vb.linalg.factory

    ''' <summary>
    ''' An ND4VB backend (Java <c>Nd4jBackend</c>): it says whether it can run here, how it ranks, and where its
    ''' configuration and array class are.
    ''' <para>
    ''' A backend is selected by <see cref="load"/>. Java finds the backends with <c>ServiceLoader</c> (and loads
    ''' native libraries); ND4VB finds them with <see cref="ND4VBClassLoading.loadService(Of S)"/>: every concrete
    ''' subclass of <see cref="Nd4vbBackend"/> with a public parameterless constructor in the assemblies
    ''' <see cref="ND4VBClassLoading"/> searches (by default every assembly loaded in the application domain, so
    ''' the managed backend <c>DeepLearning4VB.org.nd4vb.linalg.managed.ManagedBackend</c> of this library is always
    ''' found), plus instances registered with <c>ND4VBClassLoading.registerService(Of Nd4vbBackend)</c>. The available
    ''' backend with the highest <see cref="Priority"/> wins. If none is available, the assemblies listed (separated by
    ''' ';') in the system property <see cref="ND4VBSystemProperties.DYNAMIC_LOAD_CLASSPATH_PROPERTY"/> or the
    ''' environment variable <see cref="ND4VBEnvironmentVars.BACKEND_DYNAMIC_LOAD_CLASSPATH"/> are loaded with
    ''' <see cref="loadLibrary"/> (Java: jar files added to the class path) and the search is repeated once.
    ''' </para>
    ''' <para>
    ''' Java's <c>getContext()</c> returned the process-wide <c>Nd4jContext</c>, which is not ported yet; instead
    ''' <see cref="Properties"/> holds the properties read from the backend's <see cref="ConfigurationResource"/>, and
    ''' <see cref="load"/> reads them when it selects the backend (Java merged them into <c>Nd4jContext</c>).
    ''' </para>
    ''' </summary>
    Public MustInherit Class Nd4vbBackend

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(Nd4vbBackend))

        ''' <summary>The priority of CPU backends: the environment variable <c>BACKEND_PRIORITY_CPU</c>, default 0.</summary>
        Public Shared ReadOnly BACKEND_PRIORITY_CPU As Integer

        ''' <summary>The priority of GPU backends: the environment variable <c>BACKEND_PRIORITY_GPU</c>, default 100.</summary>
        Public Shared ReadOnly BACKEND_PRIORITY_GPU As Integer

        ''' @deprecated Use <seealso cref="ND4VBEnvironmentVars.BACKEND_DYNAMIC_LOAD_CLASSPATH"/>
        <Obsolete("Use ND4VBEnvironmentVars.BACKEND_DYNAMIC_LOAD_CLASSPATH")>
        Public Const DYNAMIC_LOAD_CLASSPATH As String = ND4VBEnvironmentVars.BACKEND_DYNAMIC_LOAD_CLASSPATH

        ''' @deprecated Use <seealso cref="ND4VBSystemProperties.DYNAMIC_LOAD_CLASSPATH_PROPERTY"/>
        <Obsolete("Use ND4VBSystemProperties.DYNAMIC_LOAD_CLASSPATH_PROPERTY")>
        Public Const DYNAMIC_LOAD_CLASSPATH_PROPERTY As String = ND4VBSystemProperties.DYNAMIC_LOAD_CLASSPATH_PROPERTY

        Private Shared triedDynamicLoad As Boolean = False
        Private Shared ReadOnly loadGate As New Object()

        Private propertiesValue As PropertiesT

        Shared Sub New()
            BACKEND_PRIORITY_CPU = priorityFromEnvironment(ND4VBEnvironmentVars.BACKEND_PRIORITY_CPU, 0)
            BACKEND_PRIORITY_GPU = priorityFromEnvironment(ND4VBEnvironmentVars.BACKEND_PRIORITY_GPU, 100)
        End Sub

        Private Shared Function priorityFromEnvironment(ByVal variable As String, ByVal defaultValue As Integer) As Integer
            Dim s As String = System.Environment.GetEnvironmentVariable(variable)
            If s IsNot Nothing AndAlso s.Length > 0 Then
                Dim n As Integer
                If Not Integer.TryParse(s, Globalization.NumberStyles.AllowLeadingSign, Globalization.CultureInfo.InvariantCulture, n) Then
                    Throw New InvalidOperationException("Invalid value of the environment variable " & variable & ": [" & s & "]", New JavaCompat.NumberFormatException("For input string: """ & s & """"))
                End If
                Return n
            End If
            Return defaultValue
        End Function

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
        ''' </summary>
        Public MustOverride Function canRun() As Boolean

        ''' <summary>
        ''' Get the configuration resource (a properties file; may be Nothing when the backend has none)
        ''' </summary>
        Public MustOverride ReadOnly Property ConfigurationResource As Resource

        ''' <summary>
        '''  Get the actual (concrete/implementation) class for standard INDArrays for this backend
        ''' </summary>
        Public MustOverride ReadOnly Property NDArrayClass As Type

        ''' <summary>The backend environment (Java <c>getEnvironment()</c>).</summary>
        Public MustOverride ReadOnly Property Environment As EnvironmentT

        ''' <summary>
        ''' Get the build information of the backend
        ''' </summary>
        Public MustOverride Function buildInfo() As String

        Public MustOverride Sub logBackendInit()

        ''' <summary>
        ''' Loads the best available backend.
        ''' </summary>
        ''' <exception cref="NoAvailableBackendException"> if no backend is available </exception>
        Public Shared Function load() As Nd4vbBackend
            Dim logInitProperty As String = JavaSystem.getProperty(ND4VBSystemProperties.LOG_INITIALIZATION, "true")
            ' Java Boolean.parseBoolean: "true" in any case, anything else is false
            Dim logInit As Boolean = String.Equals(logInitProperty, "true", StringComparison.OrdinalIgnoreCase)

            Dim backends As List(Of Nd4vbBackend)
            Try
                backends = ND4VBClassLoading.loadService(Of Nd4vbBackend)().ToList()
            Catch serviceError As System.Exception
                ' a fatal error due to a provider construction error.
                ' backends mustn't throw an exception during construction.
                Throw New InvalidOperationException("failed to process available backends", serviceError)
            End Try

            ' high-priority first; the sort is stable, like Java's List.sort
            For Each backend As Nd4vbBackend In backends.OrderByDescending(Function(b) b.Priority).ToList()
                Dim available As Boolean = False
                Dim [error] As String = Nothing
                Try
                    available = backend.Available
                Catch e As System.Exception
                    [error] = e.Message
                End Try
                If Not available Then
                    If logInit Then
                        log.warn("Skipped [{}] backend (unavailable): {}", CObj(backend).GetType().Name, [error])
                    End If
                    Continue For
                End If

                Try
                    backend.loadConfiguration()
                Catch e As IOException
                    log.error("", e)
                End Try

                If logInit Then
                    log.info("Loaded [{}] backend", CObj(backend).GetType().Name)
                End If
                Return backend
            Next backend

            ' need to dynamically load assemblies and retry, note that we do this right before the backend loads.
            ' An existing backend should take precedence over ones being dynamically discovered.
            ' Note that we prioritize system properties first, followed by environment variables.
            Dim paths() As String
            SyncLock loadGate
                Dim fromProperty As String = JavaSystem.getProperty(ND4VBSystemProperties.DYNAMIC_LOAD_CLASSPATH_PROPERTY)
                Dim fromEnvironment As String = System.Environment.GetEnvironmentVariable(ND4VBEnvironmentVars.BACKEND_DYNAMIC_LOAD_CLASSPATH)
                If fromProperty IsNot Nothing AndAlso Not triedDynamicLoad Then
                    paths = fromProperty.Split(";"c)
                ElseIf fromEnvironment IsNot Nothing AndAlso Not triedDynamicLoad Then
                    paths = fromEnvironment.Split(";"c)
                Else
                    Throw New NoAvailableBackendException("Please ensure that an ND4VB backend is available: the DeepLearning4VB assembly contains the managed backend (org.nd4vb.linalg.managed.ManagedBackend), and other backends are found when their assembly is loaded. Please see: https://deeplearning4j.konduit.ai/nd4j/backend")
                End If
                triedDynamicLoad = True
            End SyncLock

            'load all the discoverable assemblies and try to load the backend again
            For Each path As String In paths
                If path.Trim().Length > 0 Then
                    loadLibrary(path.Trim())
                End If
            Next path

            Return load()
        End Function

        ''' <summary>
        ''' Loads the given .NET assembly so that the backends it contains can be found (Java: adds a jar file to the
        ''' class path). This is benign if the assembly is already loaded.
        ''' </summary>
        ''' <param name="jar"> the path of the assembly to load </param>
        ''' <exception cref="NoAvailableBackendException"> if the assembly cannot be loaded </exception>
        Public Shared Sub loadLibrary(ByVal jar As String)
            SyncLock loadGate
                Try
                    Dim fullPath As String = System.IO.Path.GetFullPath(jar)
                    'Disallow if already loaded
                    For Each loaded As Assembly In AppDomain.CurrentDomain.GetAssemblies()
                        If loaded.IsDynamic Then Continue For
                        Dim location As String = Nothing
                        Try
                            location = loaded.Location
                        Catch ex As NotSupportedException
                            location = Nothing
                        End Try
                        If Not String.IsNullOrEmpty(location) AndAlso String.Equals(System.IO.Path.GetFullPath(location), fullPath, StringComparison.OrdinalIgnoreCase) Then
                            Return
                        End If
                    Next loaded
                    Assembly.LoadFrom(fullPath)
                Catch e As System.Exception
                    Throw New NoAvailableBackendException(e)
                End Try
            End SyncLock
        End Sub

        ''' <summary>
        ''' The backend's configuration properties, read from <see cref="ConfigurationResource"/> on first use (Java
        ''' returned the <c>Nd4jContext</c> configuration, which also held the Java system properties).
        ''' </summary>
        ''' <exception cref="IOException"> if the configuration resource cannot be read </exception>
        Public Overridable ReadOnly Property Properties As PropertiesT
            Get
                loadConfiguration()
                Return propertiesValue
            End Get
        End Property

        ''' <summary>Java <c>getProperties()</c>.</summary>
        Public Overridable Function getProperties() As PropertiesT
            Return Properties
        End Function

        Private Sub loadConfiguration()
            If propertiesValue IsNot Nothing Then Return
            Dim props As New PropertiesT()
            Dim resource As Resource = ConfigurationResource
            If resource IsNot Nothing Then
                Using stream As Stream = resource.InputStream
                    If stream IsNot Nothing Then
                        props.load(stream)
                    End If
                End Using
            End If
            propertiesValue = props
        End Sub

        Public Overrides Function ToString() As String
            Return CObj(Me).GetType().FullName
        End Function

        ''' <summary>Thrown when no backend is available (Java <c>Nd4jBackend.NoAvailableBackendException</c>).</summary>
        Public Class NoAvailableBackendException
            Inherits System.Exception

            Public Sub New(ByVal s As String)
                MyBase.New(s)
            End Sub

            ''' <summary>
            ''' Constructs a new exception with the specified cause and a detail
            ''' message of <tt>(cause==null ? null : cause.toString())</tt> (which
            ''' typically contains the class and detail message of <tt>cause</tt>).
            ''' </summary>
            ''' <param name="cause"> the cause </param>
            Public Sub New(ByVal cause As System.Exception)
                MyBase.New(If(cause Is Nothing, Nothing, cause.GetType().FullName & ": " & cause.Message), cause)
            End Sub
        End Class

    End Class

End Namespace
