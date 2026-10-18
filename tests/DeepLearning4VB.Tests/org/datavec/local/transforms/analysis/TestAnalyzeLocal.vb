Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports AnalyzeLocalT = DeepLearning4VB.org.datavec.local.transforms.AnalyzeLocal
Imports CategoricalAnalysis = DeepLearning4VB.org.datavec.api.transform.analysis.columns.CategoricalAnalysis
Imports ClassPathResource = DeepLearning4VB.org.nd4vb.common.io.ClassPathResource
Imports CollectionRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.collection.CollectionRecordReader
Imports CSVRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports DataAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.DataAnalysis
Imports DataQualityAnalysisT = DeepLearning4VB.org.datavec.api.transform.quality.DataQualityAnalysis
Imports DoubleAnalysis = DeepLearning4VB.org.datavec.api.transform.analysis.columns.DoubleAnalysis
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports FloatWritable = DeepLearning4VB.org.datavec.api.writable.FloatWritable
Imports InMemorySequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.inmemory.InMemorySequenceRecordReader
Imports IntegerAnalysis = DeepLearning4VB.org.datavec.api.transform.analysis.columns.IntegerAnalysis
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports NullWritable = DeepLearning4VB.org.datavec.api.writable.NullWritable
Imports NumericalColumnAnalysis = DeepLearning4VB.org.datavec.api.transform.analysis.columns.NumericalColumnAnalysis
Imports RecordReaderT = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports StringAnalysis = DeepLearning4VB.org.datavec.api.transform.analysis.columns.StringAnalysis
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.analysis

    ''' <summary>
    ''' Port of the DataVec <c>TestAnalyzeLocal</c> JUnit class, plus tests of the histograms, the quality analysis and
    ''' the unique values. The Java test compared with the column means and standard deviations computed by ND4J
    ''' (<c>RecordConverter.toMatrix</c>, <c>mean(0)</c>, <c>std(0)</c>); here they are computed directly.
    ''' </summary>
    <TestClass>
    Public Class TestAnalyzeLocal

        Private Shared Function row(ByVal ParamArray values() As WritableT) As IList(Of WritableT)
            Return New List(Of WritableT)(values)
        End Function

        Private Shared Function irisReader() As RecordReaderT
            Dim rr As RecordReaderT = New CSVRecordReader()
            rr.initialize(New FileSplit((New ClassPathResource("datavec-api/iris.dat")).File))
            Return rr
        End Function

        <TestMethod>
        Public Sub testAnalysisBasic()
            Dim rr As RecordReaderT = irisReader()

            Dim s As SchemaT = (New SchemaT.Builder()).addColumnsDouble("0", "1", "2", "3").addColumnInteger("label").build()

            Dim da As DataAnalysisT = AnalyzeLocalT.analyze(s, rr)

            Console.WriteLine(da)

            'Compare:
            Dim list As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            rr.reset()
            Do While rr.hasNext()
                list.Add(rr.next())
            Loop
            assertEquals(150, list.Count)

            For i As Integer = 0 To 4
                Dim column As Integer = i
                Dim values() As Double = list.Select(Function(r) r(column).toDouble()).ToArray()
                Dim mean As Double = values.Average()
                'ND4J's std is bias corrected (n - 1), like the sample standard deviation
                Dim std As Double = Math.Sqrt(values.Sum(Function(v) (v - mean) * (v - mean)) / (values.Length - 1))

                Dim m As Double = CType(da.getColumnAnalysis()(i), NumericalColumnAnalysis).getMean()
                Dim stddev As Double = CType(da.getColumnAnalysis()(i), NumericalColumnAnalysis).getSampleStdev()
                assertEquals(mean, m, 1e-3)
                assertEquals(std, stddev, 1e-3)
            Next i
        End Sub

        ''' <summary>Not in the Java test: minimum, maximum, counts and the histograms of the second pass.</summary>
        <TestMethod>
        Public Sub testAnalysisHistogramsAndCounts()
            Dim s As SchemaT = (New SchemaT.Builder()).addColumnsDouble("0", "1", "2", "3").addColumnInteger("label").build()
            Dim da As DataAnalysisT = AnalyzeLocalT.analyze(s, irisReader(), 10)

            Dim label As IntegerAnalysis = DirectCast(da.getColumnAnalysis("label"), IntegerAnalysis)
            assertEquals(0, label.getMin())
            assertEquals(2, label.getMax())
            assertEquals(150L, label.getCountTotal())
            assertEquals(50L, label.getCountZero())
            assertEquals(50L, label.getCountMinValue())
            assertEquals(50L, label.getCountMaxValue())
            assertEquals(10, label.getHistogramBucketCounts().Length)
            assertEquals(150L, label.getHistogramBucketCounts().Sum())

            Dim sepalLength As DoubleAnalysis = DirectCast(da.getColumnAnalysis("0"), DoubleAnalysis)
            assertEquals(4.3, sepalLength.getMin(), 1e-12)
            assertEquals(7.9, sepalLength.getMax(), 1e-12)
            assertEquals(11, sepalLength.getHistogramBuckets().Length)
            assertEquals(4.3, sepalLength.getHistogramBuckets()(0), 1e-12)
            assertEquals(7.9, sepalLength.getHistogramBuckets()(10), 1e-12)
            assertEquals(150L, sepalLength.getHistogramBucketCounts().Sum())
        End Sub

        ''' <summary>Not in the Java test: string and categorical columns.</summary>
        <TestMethod>
        Public Sub testAnalysisStringAndCategorical()
            Dim data As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New Text("a"), New Text("cat0")),
                row(New Text("abc"), New Text("cat1")),
                row(New Text("ab"), New Text("cat0"))
            }
            Dim s As SchemaT = (New SchemaT.Builder()).addColumnString("str").addColumnCategorical("cat", "cat0", "cat1").build()
            Dim da As DataAnalysisT = AnalyzeLocalT.analyze(s, New CollectionRecordReader(data))

            Dim str As StringAnalysis = DirectCast(da.getColumnAnalysis("str"), StringAnalysis)
            assertEquals(1, str.getMinLength())
            assertEquals(3, str.getMaxLength())
            assertEquals(2.0, str.getMeanLength(), 1e-12)
            assertEquals(3L, str.getHistogramBucketCounts().Sum())

            Dim cat As CategoricalAnalysis = DirectCast(da.getColumnAnalysis("cat"), CategoricalAnalysis)
            assertEquals(2L, cat.getMapOfCounts()("cat0"))
            assertEquals(1L, cat.getMapOfCounts()("cat1"))
        End Sub

        ''' <summary>As in Java, float columns are not supported by the local analysis.</summary>
        <TestMethod>
        Public Sub testAnalysisRejectsFloatColumns()
            Dim data As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(New FloatWritable(1.5F))}
            Dim s As SchemaT = (New SchemaT.Builder()).addColumnFloat("f").build()
            assertThrows(Of ArgumentException)(Sub() AnalyzeLocalT.analyze(s, New CollectionRecordReader(data)))
        End Sub

        <TestMethod>
        Public Sub testAnalysisWithoutData()
            Dim s As SchemaT = (New SchemaT.Builder()).addColumnDouble("d").build()
            assertThrows(Of InvalidOperationException)(Sub() AnalyzeLocalT.analyze(s, New CollectionRecordReader(New List(Of IList(Of WritableT))())))
        End Sub

        <TestMethod>
        Public Sub testAnalyzeQuality()
            Dim data As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New IntWritable(1), New DoubleWritable(1.0)),
                row(New Text("notAnInt"), New DoubleWritable(2.0)),
                row(NullWritable.INSTANCE, New DoubleWritable(Double.NaN)),
                row(New IntWritable(4), New Text(""))
            }
            Dim s As SchemaT = (New SchemaT.Builder()).addColumnInteger("int").addColumnDouble("double").build()
            Dim dqa As DataQualityAnalysisT = AnalyzeLocalT.analyzeQuality(s, New CollectionRecordReader(data))

            Dim intQuality = dqa.getColumnQualityList()(0)
            assertEquals(4L, intQuality.getCountTotal())
            assertEquals(2L, intQuality.getCountValid())
            assertEquals(1L, intQuality.getCountInvalid())
            assertEquals(1L, intQuality.getCountMissing())

            'As in Java, NaN passes DoubleMetaData.isValid when the column has no minimum or maximum
            Dim doubleQuality = dqa.getColumnQualityList()(1)
            assertEquals(4L, doubleQuality.getCountTotal())
            assertEquals(3L, doubleQuality.getCountValid())
            assertEquals(0L, doubleQuality.getCountInvalid())
            assertEquals(1L, doubleQuality.getCountMissing())
        End Sub

        <TestMethod>
        Public Sub testAnalyzeQualityWithoutData()
            Dim s As SchemaT = (New SchemaT.Builder()).addColumnInteger("int").build()
            Dim dqa As DataQualityAnalysisT = AnalyzeLocalT.analyzeQuality(s, New CollectionRecordReader(New List(Of IList(Of WritableT))()))
            assertEquals(0, dqa.getColumnQualityList().Count)
        End Sub

        ''' <summary>The Java version started from an empty state list and failed on the first time step.</summary>
        <TestMethod>
        Public Sub testAnalyzeQualitySequence()
            Dim seq1 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(New IntWritable(1)), row(New Text("x"))}
            Dim seq2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(New IntWritable(3)), row(NullWritable.INSTANCE), row(New IntWritable(5))}
            Dim s As SchemaT = (New SequenceSchemaT.Builder()).addColumnInteger("int").build()
            Dim reader As New InMemorySequenceRecordReader(New List(Of IList(Of IList(Of WritableT))) From {seq1, seq2})

            Dim dqa As DataQualityAnalysisT = AnalyzeLocalT.analyzeQualitySequence(s, reader)
            Dim q = dqa.getColumnQualityList()(0)
            assertEquals(5L, q.getCountTotal())
            assertEquals(3L, q.getCountValid())
            assertEquals(1L, q.getCountInvalid())
            assertEquals(1L, q.getCountMissing())
        End Sub

        <TestMethod>
        Public Sub testGetUnique()
            Dim data As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New Text("a"), New IntWritable(1)),
                row(New Text("b"), New IntWritable(1)),
                row(New Text("a"), New IntWritable(2))
            }
            Dim s As SchemaT = (New SchemaT.Builder()).addColumnString("str").addColumnInteger("int").build()

            Dim unique As ISet(Of WritableT) = AnalyzeLocalT.getUnique("str", s, New CollectionRecordReader(data))
            assertEquals(New HashSet(Of WritableT) From {New Text("a"), New Text("b")}, unique)

            Dim uniqueMap As IDictionary(Of String, ISet(Of WritableT)) = AnalyzeLocalT.getUnique(New List(Of String) From {"str", "int"}, s, New CollectionRecordReader(data))
            assertEquals(2, uniqueMap.Count)
            assertEquals(New HashSet(Of WritableT) From {New Text("a"), New Text("b")}, uniqueMap("str"))
            assertEquals(New HashSet(Of WritableT) From {New IntWritable(1), New IntWritable(2)}, uniqueMap("int"))
        End Sub

        <TestMethod>
        Public Sub testGetUniqueSequence()
            Dim seq1 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(New Text("a"), New IntWritable(1)), row(New Text("b"), New IntWritable(1))}
            Dim seq2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(New Text("c"), New IntWritable(3))}
            Dim s As SchemaT = (New SequenceSchemaT.Builder()).addColumnString("str").addColumnInteger("int").build()
            Dim sequences As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT))) From {seq1, seq2}

            Dim unique As ISet(Of WritableT) = AnalyzeLocalT.getUniqueSequence("str", s, New InMemorySequenceRecordReader(sequences))
            assertEquals(New HashSet(Of WritableT) From {New Text("a"), New Text("b"), New Text("c")}, unique)

            Dim uniqueMap As IDictionary(Of String, ISet(Of WritableT)) = AnalyzeLocalT.getUniqueSequence(New List(Of String) From {"str", "int"}, s, New InMemorySequenceRecordReader(sequences))
            assertEquals(New HashSet(Of WritableT) From {New IntWritable(1), New IntWritable(3)}, uniqueMap("int"))
            assertEquals(3, uniqueMap("str").Count)
        End Sub

    End Class

End Namespace
