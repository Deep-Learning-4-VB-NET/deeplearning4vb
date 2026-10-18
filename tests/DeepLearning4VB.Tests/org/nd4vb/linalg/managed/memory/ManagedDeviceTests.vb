Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.managed
Imports AffinityManager = DeepLearning4VB.org.nd4vb.linalg.api.concurrency.AffinityManager
Imports AllocationsTracker = DeepLearning4VB.org.nd4vb.linalg.api.memory.AllocationsTracker
Imports EnvironmentT = DeepLearning4VB.org.nd4vb.linalg.factory.Environment
Imports ManagedDataBuffer = DeepLearning4VB.org.nd4vb.linalg.managed.buffer.ManagedDataBuffer

' Tests of org.nd4vb.linalg.managed.ManagedAffinityManager and ManagedEnvironment (kept in the managed\memory test folder,
' which the buffers agent owns; the namespace mirrors the source).
Namespace org.nd4vb.linalg.managed

    ''' <summary>Tests for <see cref="ManagedAffinityManager"/> and <see cref="ManagedEnvironment"/>.</summary>
    <TestClass>
    Public Class ManagedDeviceTests

        <TestMethod>
        Public Sub testAffinityManagerHasOneDevice()
            Dim am As AffinityManager = New ManagedAffinityManager()
            assertEquals(0, am.DeviceForCurrentThread.Value)
            assertEquals(0, am.getDeviceForThread(12345).Value)
            assertEquals(1, am.NumberOfDevices)
            assertTrue(am.CrossDeviceAccessSupported)
            Dim b As New ManagedDataBuffer(Global.DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType.FLOAT, 2)
            am.touch(b)
            am.tagLocation(b, AffinityManager.Location.HOST)
            assertNull(am.replicateToDevice(0, b))
            am.unsafeSetDevice(0)
            am.allowCrossDeviceAccess(False)
        End Sub

        <TestMethod>
        Public Sub testEnvironmentDefaultsAndSettings()
            Dim env As EnvironmentT = ManagedEnvironment.Instance
            Assert.AreSame(env, ManagedEnvironment.Instance)
            assertTrue(env.CPU)
            assertEquals(0, env.blasMajorVersion())
            assertTrue(env.maxThreads() > 0)
            assertTrue(env.maxMasterThreads() > 0)

            Dim e As New ManagedEnvironment()
            assertFalse(e.Verbose)
            assertFalse(e.Debug)
            assertFalse(e.Profiling)
            assertFalse(e.DetectingLeaks)
            assertEquals(1, e.tadThreshold())
            assertEquals(1024, e.elementwiseThreshold())
            e.Verbose = True
            assertFalse(e.DebugAndVerbose)
            e.Debug = True
            assertTrue(e.DebugAndVerbose)
            e.Profiling = True
            assertTrue(e.Profiling)
            e.LeaksDetector = True
            assertTrue(e.DetectingLeaks)
            e.allowHelpers(False)
            assertFalse(e.helpersAllowed())
            e.setTadThreshold(8)
            e.setElementwiseThreshold(4096)
            e.setMaxThreads(3)
            e.setMaxMasterThreads(2)
            assertEquals(8, e.tadThreshold())
            assertEquals(4096, e.elementwiseThreshold())
            assertEquals(3, e.maxThreads())
            assertEquals(2, e.maxMasterThreads())
            e.MaxPrimaryMemory = 100
            e.MaxSpecialMemory = 200
            e.MaxDeviceMemory = 300
            assertArrayEquals(New Long() {100, 200, 300}, e.getMemoryLimits())
            e.setGroupLimit(1, 50)
            e.setDeviceLimit(0, 60)
            assertEquals(50L, e.getGroupLimit(1))
            assertEquals(0L, e.getGroupLimit(2))
            assertEquals(60L, e.getDeviceLimit(0))
            assertEquals(AllocationsTracker.Instance.bytesOnDevice(0), e.getDeviceCouner(0))
        End Sub

    End Class

End Namespace
