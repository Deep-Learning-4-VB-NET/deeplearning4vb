Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ClassPathResource = DeepLearning4VB.org.nd4vb.common.io.ClassPathResource
Imports CSVRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports LineRecordReaderFunctionT = DeepLearning4VB.org.datavec.local.transforms.functions.LineRecordReaderFunction
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.functions

    ''' <summary>Port of the DataVec <c>TestLineRecordReaderFunction</c> JUnit class (iris data: <c>datavec-api/iris.dat</c>).</summary>
    <TestClass>
    Public Class TestLineRecordReaderFunction

        <TestMethod>
        Public Sub testLineRecordReader()
            Dim dataFile As String = (New ClassPathResource("datavec-api/iris.dat")).File
            Dim lines As IList(Of String) = File.ReadAllLines(dataFile).Where(Function(l) l.Length > 0).ToList()

            Dim linesRdd As IList(Of String) = lines

            Dim rr As New CSVRecordReader(0, ","c)

            Dim [out] As IList(Of IList(Of WritableT)) = linesRdd.Select(Function(input) (New LineRecordReaderFunctionT(rr)).apply(input)).ToList()
            Dim outList As IList(Of IList(Of WritableT)) = [out]

            Dim rr2 As New CSVRecordReader(0, ","c)
            rr2.initialize(New FileSplit(dataFile))
            'Java used a HashSet of lists (content equality); the lists are compared by content here
            Dim expectedSet As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            Dim totalCount As Integer = 0
            Do While rr2.hasNext()
                expectedSet.Add(rr2.next())
                totalCount += 1
            Loop

            assertEquals(totalCount, outList.Count)

            For Each line As IList(Of WritableT) In outList
                Dim current As IList(Of WritableT) = line
                assertTrue(expectedSet.Any(Function(e) DeepEquals(e, current)))
            Next line

            'Not in the Java test: the records come out in line order
            assertEquals(expectedSet, outList)
        End Sub

    End Class

End Namespace
