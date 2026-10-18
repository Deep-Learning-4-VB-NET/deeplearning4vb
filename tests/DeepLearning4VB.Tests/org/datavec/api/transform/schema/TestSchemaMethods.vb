Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema

Namespace org.datavec.api.transform.schema

    ''' <summary>Port of the DataVec <c>TestSchemaMethods</c> JUnit class.</summary>
    <TestClass>
    Public Class TestSchemaMethods

        <TestMethod>
        Public Sub testNumberedColumnAdding()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnsDouble("doubleCol_%d", 0, 2).addColumnsLong("longCol_%d", 3, 5).addColumnsString("stringCol_%d", 6, 8).build()

            assertEquals(9, schema.numColumns())

            For i As Integer = 0 To 8
                If i <= 2 Then
                    assertEquals("doubleCol_" & i, schema.getName(i))
                    assertEquals(ColumnType.[Double], schema.getType(i))
                ElseIf i <= 5 Then
                    assertEquals("longCol_" & i, schema.getName(i))
                    assertEquals(ColumnType.[Long], schema.getType(i))
                Else
                    assertEquals("stringCol_" & i, schema.getName(i))
                    assertEquals(ColumnType.[String], schema.getType(i))
                End If
            Next i
        End Sub

        ' testToFromJsonYaml / testMissingPrimitives are ported in TestJsonYaml (JSON only).

    End Class

End Namespace
