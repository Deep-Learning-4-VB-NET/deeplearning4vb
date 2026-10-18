Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports SequenceWritablesToStringFunctionT = DeepLearning4VB.org.datavec.local.transforms.misc.SequenceWritablesToStringFunction
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritablesToStringFunctionT = DeepLearning4VB.org.datavec.local.transforms.misc.WritablesToStringFunction
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.functions

    ''' <summary>Port of the DataVec <c>TestWritablesToStringFunctions</c> JUnit class, plus quoting tests.</summary>
    <TestClass>
    Public Class TestWritablesToStringFunctions

        Private Shared Function row(ByVal ParamArray values() As WritableT) As IList(Of WritableT)
            Return New List(Of WritableT)(values)
        End Function

        <TestMethod>
        Public Sub testWritablesToString()
            Dim l As IList(Of WritableT) = row(New DoubleWritable(1.5), New Text("someValue"))
            Dim expected As String = l(0).ToString() & "," & l(1).ToString()

            assertEquals(expected, (New WritablesToStringFunctionT(",")).apply(l))
        End Sub

        <TestMethod>
        Public Sub testSequenceWritablesToString()
            Dim l As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New DoubleWritable(1.5), New Text("someValue")),
                row(New DoubleWritable(2.5), New Text("otherValue"))
            }

            Dim expected As String = l(0)(0).ToString() & "," & l(0)(1).ToString() & vbLf & l(1)(0).ToString() & "," & l(1)(1).ToString()

            assertEquals(expected, (New SequenceWritablesToStringFunctionT(",")).apply(l))
        End Sub

        ''' <summary>Not in the Java tests: values containing the delimiter are quoted, and quotes in them doubled.</summary>
        <TestMethod>
        Public Sub testQuoting()
            Dim l As IList(Of WritableT) = row(New Text("a,b"), New Text("say ""hi"""), New Text("x""y,z"))
            assertEquals("a,b,say ""hi"",x""y,z", (New WritablesToStringFunctionT(",")).apply(l))
            assertEquals("""a,b"",say ""hi"",""x""""y,z""", (New WritablesToStringFunctionT(",", """")).apply(l))

            Dim seq As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(New Text("a,b")), row(New Text("c"))}
            assertEquals("'a,b';c", (New SequenceWritablesToStringFunctionT(",", ";", "'")).apply(seq))
            assertEquals("a,b" & vbLf & "c", (New SequenceWritablesToStringFunctionT()).apply(seq))
        End Sub

    End Class

End Namespace
