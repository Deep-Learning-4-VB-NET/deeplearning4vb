Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.executors

' Tests of org.nd4vb.linalg.executors (kept in the env test folder, which the buffers agent owns; the namespace mirrors
' the source).
Namespace org.nd4vb.linalg.executors

    ''' <summary>Tests for <see cref="ExecutorServiceProvider"/>.</summary>
    <TestClass>
    Public Class ExecutorServiceProviderTests

        <TestMethod>
        Public Sub testPools()
            assertTrue(ExecutorServiceProvider.NumberOfThreads >= 1)
            Dim factory As TaskFactory = ExecutorServiceProvider.ExecutorService
            Assert.AreSame(factory, ExecutorServiceProvider.ExecutorService)
            Dim counter As Integer = 0
            Dim tasks As New List(Of Task)()
            For i As Integer = 1 To 100
                tasks.Add(factory.StartNew(Sub() Interlocked.Increment(counter)))
            Next
            assertTrue(Task.WaitAll(tasks.ToArray(), TimeSpan.FromMinutes(1)))
            assertEquals(100, counter)

            Dim options As ParallelOptions = ExecutorServiceProvider.ForkJoinPool
            Assert.AreSame(options, ExecutorServiceProvider.ForkJoinPool)
            assertEquals(ExecutorServiceProvider.NumberOfThreads, options.MaxDegreeOfParallelism)
            Dim sum As Long = 0
            Parallel.For(0, 1000, options, Sub(i) Interlocked.Add(sum, i))
            assertEquals(499500L, sum)
            assertEquals("org.nd4vb.parallel.threads", ExecutorServiceProvider.EXEC_THREADS)
            assertEquals("org.nd4vb.parallel.enabled", ExecutorServiceProvider.ENABLED)
        End Sub

    End Class

End Namespace
