Imports System
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.factory
Imports ND4VBClassLoading = DeepLearning4VB.org.nd4vb.common.config.ND4VBClassLoading
Imports ND4VBEnvironmentVars = DeepLearning4VB.org.nd4vb.common.config.ND4VBEnvironmentVars
Imports ND4VBSystemProperties = DeepLearning4VB.org.nd4vb.common.config.ND4VBSystemProperties
Imports Resource = DeepLearning4VB.org.nd4vb.common.io.Resource
Imports AbstractResource = DeepLearning4VB.org.nd4vb.common.io.AbstractResource
Imports EnvironmentT = DeepLearning4VB.org.nd4vb.linalg.factory.Environment

'
' *  ******************************************************************************
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.linalg.factory

    ''' <summary>
    ''' Tests for backend discovery (<c>Nd4vbBackend.load</c>): the available backend with the highest priority wins,
    ''' unavailable backends are skipped, the configuration resource is read, and a missing backend throws. The test
    ''' backends have no public parameterless constructor, so they are never found by the assembly scan; they are
    ''' registered with <c>ND4VBClassLoading.registerService</c> and the scan is switched off during the tests.
    ''' </summary>
    <TestClass>
    Public Class Nd4vbBackendTests

        Private Class TextResource
            Inherits AbstractResource

            Private ReadOnly text As String

            Public Sub New(ByVal text As String)
                Me.text = text
            End Sub

            Public Overrides ReadOnly Property InputStream As Stream
                Get
                    Return New MemoryStream(Encoding.UTF8.GetBytes(text))
                End Get
            End Property

            Public Overrides ReadOnly Property Description As String
                Get
                    Return "test properties"
                End Get
            End Property
        End Class

        Private Class TestBackend
            Inherits Nd4vbBackend

            Private ReadOnly priorityValue As Integer
            Private ReadOnly availableValue As Boolean
            Private ReadOnly config As Resource
            Public ReadOnly Name As String

            Public Sub New(ByVal name As String, ByVal priority As Integer, ByVal available As Boolean, Optional ByVal config As Resource = Nothing)
                Me.Name = name
                Me.priorityValue = priority
                Me.availableValue = available
                Me.config = config
            End Sub

            Public Overrides Function allowsOrder() As Boolean
                Return True
            End Function

            Public Overrides ReadOnly Property Priority As Integer
                Get
                    Return priorityValue
                End Get
            End Property

            Public Overrides ReadOnly Property Available As Boolean
                Get
                    If Name = "throws" Then Throw New InvalidOperationException("broken backend")
                    Return availableValue
                End Get
            End Property

            Public Overrides Function canRun() As Boolean
                Return availableValue
            End Function

            Public Overrides ReadOnly Property ConfigurationResource As Resource
                Get
                    Return config
                End Get
            End Property

            Public Overrides ReadOnly Property NDArrayClass As Type
                Get
                    Return GetType(Object)
                End Get
            End Property

            Public Overrides ReadOnly Property Environment As EnvironmentT
                Get
                    Return Nothing
                End Get
            End Property

            Public Overrides Function buildInfo() As String
                Return "test backend " & Name
            End Function

            Public Overrides Sub logBackendInit()
            End Sub
        End Class

        <TestInitialize>
        Public Sub setUp()
            ND4VBClassLoading.clearRegisteredServices()
            ' no assembly scan: only the registered test backends are candidates
            ND4VBClassLoading.Nd4vbAssemblies = Function() New Assembly() {}
        End Sub

        <TestCleanup>
        Public Sub tearDown()
            ND4VBClassLoading.clearRegisteredServices()
            ND4VBClassLoading.resetNd4vbClassloader()
        End Sub

        <TestMethod>
        Public Sub testHighestPriorityAvailableBackendWins()
            ND4VBClassLoading.registerService(Of Nd4vbBackend)(New TestBackend("low", 1, True))
            ND4VBClassLoading.registerService(Of Nd4vbBackend)(New TestBackend("unavailable", 500, False))
            ND4VBClassLoading.registerService(Of Nd4vbBackend)(New TestBackend("throws", 400, True))
            ND4VBClassLoading.registerService(Of Nd4vbBackend)(New TestBackend("high", 50, True))
            ND4VBClassLoading.registerService(Of Nd4vbBackend)(New TestBackend("high-second", 50, True))

            Dim loaded As Nd4vbBackend = Nd4vbBackend.load()
            ' equal priorities keep the registration order (Java's stable List.sort)
            assertEquals("high", DirectCast(loaded, TestBackend).Name)
            assertEquals(GetType(TestBackend).FullName, loaded.ToString())
        End Sub

        <TestMethod>
        Public Sub testConfigurationResourceIsRead()
            Dim backend As New TestBackend("configured", 1, True, New TextResource("blas.vendor=managed" & vbLf & "# comment" & vbLf & "threads = 4" & vbLf))
            ND4VBClassLoading.registerService(Of Nd4vbBackend)(backend)
            Dim loaded As Nd4vbBackend = Nd4vbBackend.load()
            Assert.AreSame(backend, loaded)
            assertEquals("managed", loaded.Properties.getProperty("blas.vendor"))
            assertEquals("4", loaded.getProperties().getProperty("threads"))
            assertNull(loaded.Properties.getProperty("missing"))

            ' a backend without a configuration resource has empty properties
            Dim plain As New TestBackend("plain", 1, True)
            assertNull(plain.Properties.getProperty("blas.vendor"))
        End Sub

        <TestMethod>
        Public Sub testNoAvailableBackend()
            If JavaSystem.getProperty(ND4VBSystemProperties.DYNAMIC_LOAD_CLASSPATH_PROPERTY) IsNot Nothing OrElse
               System.Environment.GetEnvironmentVariable(ND4VBEnvironmentVars.BACKEND_DYNAMIC_LOAD_CLASSPATH) IsNot Nothing Then
                Assert.Inconclusive("a dynamic backend path is configured on this machine")
            End If
            ND4VBClassLoading.registerService(Of Nd4vbBackend)(New TestBackend("unavailable", 10, False))
            Dim ex = assertThrows(Of Nd4vbBackend.NoAvailableBackendException)(Sub() Nd4vbBackend.load())
            assertTrue(ex.Message.StartsWith("Please ensure that an ND4VB backend is available"), ex.Message)
        End Sub

        <TestMethod>
        Public Sub testLoadLibrary()
            ' an assembly that is already loaded is accepted
            Nd4vbBackend.loadLibrary(GetType(Nd4vbBackend).Assembly.Location)
            Dim missing As String = Path.Combine(Path.GetTempPath(), "no-such-backend-" & Guid.NewGuid().ToString("N") & ".dll")
            Dim ex = assertThrows(Of Nd4vbBackend.NoAvailableBackendException)(Sub() Nd4vbBackend.loadLibrary(missing))
            assertNotNull(ex.InnerException)
        End Sub

        <TestMethod>
        Public Sub testPrioritiesFromEnvironment()
            If System.Environment.GetEnvironmentVariable(ND4VBEnvironmentVars.BACKEND_PRIORITY_CPU) Is Nothing Then
                assertEquals(0, Nd4vbBackend.BACKEND_PRIORITY_CPU)
            End If
            If System.Environment.GetEnvironmentVariable(ND4VBEnvironmentVars.BACKEND_PRIORITY_GPU) Is Nothing Then
                assertEquals(100, Nd4vbBackend.BACKEND_PRIORITY_GPU)
            End If
        End Sub

    End Class

End Namespace
