Imports System
Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.conf
Imports DeepLearning4VB.org.datavec.api.split
Imports DeepLearning4VB.org.datavec.api.split.partition

Namespace org.datavec.api.split.partition

    ''' <summary>Ported from the DataVec JUnit test of the same name (package <c>parittion</c> [sic] upstream).</summary>
    <TestClass>
    Public Class PartitionerTests

        Private tmpDir As String

        <TestInitialize>
        Public Sub CreateTempDir()
            tmpDir = Path.Combine(Path.GetTempPath(), "dl4vb-partitioner-" & Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(tmpDir)
        End Sub

        <TestCleanup>
        Public Sub DeleteTempDir()
            If Directory.Exists(tmpDir) Then
                Directory.Delete(tmpDir, True)
            End If
        End Sub

        <TestMethod>
        Public Sub testRecordsPerFilePartition()
            Dim partitioner As Partitioner = New NumberOfRecordsPartitioner()
            Dim fileSplit As New FileSplit(tmpDir)
            assertTrue(fileSplit.needsBootstrapForWrite())
            fileSplit.bootStrapForWrite()
            partitioner.init(fileSplit)
            assertEquals(1, partitioner.numPartitions())
        End Sub

        <TestMethod>
        Public Sub testInputAddFile()
            Dim partitioner As Partitioner = New NumberOfRecordsPartitioner()
            Dim fileSplit As New FileSplit(tmpDir)
            assertTrue(fileSplit.needsBootstrapForWrite())
            fileSplit.bootStrapForWrite()
            Dim configuration As New Configuration()
            configuration.[set](NumberOfRecordsPartitioner.RECORDS_PER_FILE_CONFIG, "5")
            partitioner.init(configuration, fileSplit)
            partitioner.updatePartitionInfo(PartitionMetaData.builder().numRecordsUpdated(5).build())
            assertTrue(partitioner.needsNewPartition())
            Dim os As Stream = partitioner.openNewStream()
            os.Close()
            assertNotNull(os)
            'run more than once to ensure output stream creation works properly
            partitioner.updatePartitionInfo(PartitionMetaData.builder().numRecordsUpdated(5).build())
            os = partitioner.openNewStream()
            os.Close()
            assertNotNull(os)

            'two files were added to the (initially empty) directory
            assertEquals(2, Directory.GetFiles(tmpDir).Length)
            assertEquals(2L, fileSplit.length())
            assertEquals(10, partitioner.totalRecordsWritten())
            assertEquals(0, partitioner.numRecordsWritten())
        End Sub

        <TestMethod>
        Public Sub testPartitionMetaData()
            Dim meta As PartitionMetaData = PartitionMetaData.builder().numRecordsUpdated(3).sizeUpdated(1024L).build()
            assertEquals(3, meta.getNumRecordsUpdated())
            assertEquals(1024L, meta.getSizeUpdated())
            assertEquals(3, meta.NumRecordsUpdated)
            assertEquals(1024L, meta.SizeUpdated)
            Dim empty As New PartitionMetaData()
            assertEquals(0, empty.getNumRecordsUpdated())
            assertEquals(0L, empty.getSizeUpdated())
            Dim full As New PartitionMetaData(7, 8L)
            assertEquals(7, full.getNumRecordsUpdated())
            assertEquals(8L, full.getSizeUpdated())
        End Sub

        <TestMethod>
        Public Sub testCurrentOutputStreamAndCounters()
            Dim partitioner As New NumberOfRecordsPartitioner()
            Dim fileSplit As New FileSplit(tmpDir)
            Dim configuration As New Configuration(False)
            configuration.setInt(NumberOfRecordsPartitioner.RECORDS_PER_FILE_CONFIG, 2)
            partitioner.init(configuration, fileSplit)
            assertFalse(partitioner.needsNewPartition())
            Dim first As Stream = partitioner.currentOutputStream()
            assertNotNull(first)
            assertTrue(partitioner.currentOutputStream() Is first)
            partitioner.updatePartitionInfo(PartitionMetaData.builder().numRecordsUpdated(1).build())
            assertFalse(partitioner.needsNewPartition())
            assertEquals(1, partitioner.numRecordsWritten())
            partitioner.updatePartitionInfo(PartitionMetaData.builder().numRecordsUpdated(1).build())
            assertTrue(partitioner.needsNewPartition())
            assertEquals(2, partitioner.numRecordsWritten())
            assertEquals(2, partitioner.totalRecordsWritten())
            first.Close()
            Dim second As Stream = partitioner.openNewStream()
            assertNotNull(second)
            assertFalse(second Is first)
            second.Close()
            assertEquals(0, partitioner.numRecordsWritten())
            assertEquals(2, partitioner.totalRecordsWritten())
        End Sub

    End Class

End Namespace
