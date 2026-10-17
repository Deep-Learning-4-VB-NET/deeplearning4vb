Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports ParseDoubleTransform = DeepLearning4VB.org.datavec.api.transform.transforms.parse.ParseDoubleTransform
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.parse

    ''' <summary>Port of the DataVec <c>ParseDoubleTransformTest</c> JUnit class ("Parse Double Transform Test").</summary>
    <TestClass>
    Public Class ParseDoubleTransformTest

        ''' <summary>Test Double Transform</summary>
        <TestMethod>
        Public Sub testDoubleTransform()
            Dim record As IList(Of WritableT) = New List(Of WritableT)()
            record.Add(New Text("0.0"))
            Dim transformed As IList(Of WritableT) = New List(Of WritableT) From {New DoubleWritable(0.0)}
            assertEquals(transformed, (New ParseDoubleTransform()).map(record))
        End Sub

    End Class

End Namespace
