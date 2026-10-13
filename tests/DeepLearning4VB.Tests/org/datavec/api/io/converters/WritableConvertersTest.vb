Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.io
Imports DeepLearning4VB.org.datavec.api.io.converters
Imports DeepLearning4VB.org.datavec.api.writable
Imports DVW = DeepLearning4VB.org.datavec.api.writable

Namespace org.datavec.api.io.converters

    ''' <summary>The writable converters: numeric widening/narrowing, label indexing and the identity converter.</summary>
    <TestClass>
    Public Class WritableConvertersTest

        <TestMethod>
        Public Sub testDoubleWritableConverter()
            Dim c As WritableConverter = New DoubleWritableConverter()
            assertEquals(New DoubleWritable(1.5), c.convert(New Text("1.5")))
            assertEquals(New DoubleWritable(2), c.convert(New IntWritable(2)))
            assertEquals(New DoubleWritable(0.5), c.convert(New FloatWritable(0.5F)))
            assertEquals(New DoubleWritable(-3), c.convert(New DoubleWritable(-3)))
            assertTrue(TypeOf c.convert(New IntWritable(2)) Is DoubleWritable)
            assertThrows(Of WritableConverterException)(Sub() c.convert(New BooleanWritable(True)))
            assertThrows(Of WritableConverterException)(Sub() c.convert(New LongWritable(1)))
            assertThrows(Of WritableConverterException)(Sub() c.convert(NullWritable.INSTANCE))
            Dim ex As WritableConverterException = assertThrows(Of WritableConverterException)(Sub() c.convert(New BooleanWritable(True)))
            assertTrue(ex.Message.Contains("BooleanWritable"), ex.Message)
        End Sub

        <TestMethod>
        Public Sub testFloatWritableConverter()
            Dim c As WritableConverter = New FloatWritableConverter()
            assertEquals(New FloatWritable(1.5F), c.convert(New Text("1.5")))
            assertEquals(New FloatWritable(2), c.convert(New IntWritable(2)))
            assertEquals(New FloatWritable(0.5F), c.convert(New DoubleWritable(0.5)))
            assertEquals(New FloatWritable(-3), c.convert(New FloatWritable(-3)))
            assertTrue(TypeOf c.convert(New DoubleWritable(0.5)) Is FloatWritable)
            assertThrows(Of WritableConverterException)(Sub() c.convert(New LongWritable(1)))
            assertThrows(Of WritableConverterException)(Sub() c.convert(New ByteWritable(1)))
        End Sub

        <TestMethod>
        Public Sub testLabelWriterConverter()
            Dim c As WritableConverter = New LabelWriterConverter(New List(Of String) From {"cat", "dog", "fish"})
            assertEquals(New IntWritable(0), c.convert(New Text("cat")))
            assertEquals(New IntWritable(2), c.convert(New Text("fish")))
            assertEquals(New IntWritable(-1), c.convert(New Text("bird")))
            assertEquals(New IntWritable(1), c.convert(New Text("dog")))
            ' any writable is matched by its string form
            Dim numbers As WritableConverter = New LabelWriterConverter(New String() {"1", "2.0"})
            assertEquals(New IntWritable(0), numbers.convert(New IntWritable(1)))
            assertEquals(New IntWritable(1), numbers.convert(New DoubleWritable(2)))
        End Sub

        <TestMethod>
        Public Sub testSelfWritableConverter()
            Dim c As WritableConverter = New SelfWritableConverter()
            Dim w As DVW.Writable = New Text("unchanged")
            assertTrue(w Is c.convert(w))
            assertTrue(NullWritable.INSTANCE Is c.convert(NullWritable.INSTANCE))
        End Sub

        <TestMethod>
        Public Sub testWritableConverterException()
            Dim inner As New InvalidOperationException("inner")
            assertEquals("msg", New WritableConverterException("msg").Message)
            assertTrue(New WritableConverterException("msg", inner).InnerException Is inner)
            assertTrue(New WritableConverterException(inner).InnerException Is inner)
            assertNotNull(New WritableConverterException().Message)
        End Sub

    End Class

End Namespace
