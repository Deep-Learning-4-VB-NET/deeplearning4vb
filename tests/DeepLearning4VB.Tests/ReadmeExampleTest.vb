Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.records.reader.impl.csv
Imports DeepLearning4VB.org.datavec.api.split
Imports DeepLearning4VB.org.datavec.api.transform
Imports DeepLearning4VB.org.datavec.api.transform.condition
Imports DeepLearning4VB.org.datavec.api.transform.condition.column
Imports DeepLearning4VB.org.datavec.api.transform.schema
Imports DeepLearning4VB.org.datavec.api.writable
Imports DeepLearning4VB.org.datavec.local.transforms

' The README's usage example, compiled outside the library's namespaces exactly as a user's project would be.
Namespace Global.ReadmeExample

    <TestClass>
    Public Class ReadmeExampleTest

        <TestMethod>
        Public Sub ReadmeExample_RunsAsDocumented()
            Dim dir As String = Path.Combine(Path.GetTempPath(), "dl4vb-tests", Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(dir)
            Dim csvPath As String = Path.Combine(dir, "people.csv")
            File.WriteAllText(csvPath, "name,age,city" & vbLf & "Ann,34,London" & vbLf & "Ben,12,Paris" & vbLf & "Cleo,52,Berlin" & vbLf)
            Try
                ' ---- README example starts here ----
                Dim schema = New Schema.Builder().
                    addColumnString("name").
                    addColumnInteger("age").
                    addColumnCategorical("city", "London", "Paris", "Berlin").
                    build()

                Dim tp = New TransformProcess.Builder(schema).
                    removeColumns("name").
                    filter(New IntegerColumnCondition("age", ConditionOp.LessThan, 18)).
                    categoricalToInteger("city").
                    build()

                Dim records As New List(Of IList(Of Writable))()
                Using reader As New CSVRecordReader(1)
                    reader.initialize(New FileSplit(csvPath))
                    While reader.hasNext()
                        records.Add(reader.next())
                    End While
                End Using

                Dim result = LocalTransformExecutor.execute(records, tp)
                Dim json As String = tp.toJson()
                Dim sameProcess = TransformProcess.fromJson(json)
                ' ---- README example ends here ----

                Assert.AreEqual(2, result.Count)
                Assert.AreEqual("34", result(0)(0).ToString())
                Assert.AreEqual("0", result(0)(1).ToString())
                Assert.AreEqual("52", result(1)(0).ToString())
                Assert.AreEqual("2", result(1)(1).ToString())
                Assert.IsTrue(json.Contains("""@class"" : ""org.datavec.api.transform.transform.categorical.CategoricalToIntegerTransform"""), json)
                Assert.AreEqual(json, sameProcess.toJson())
            Finally
                Directory.Delete(dir, True)
            End Try
        End Sub

    End Class

End Namespace
