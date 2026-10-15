Imports System
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone

Namespace org.datavec.api.transform

    ''' <summary>
    ''' Shared helpers of the transform-core tests. <see cref="getSchema"/> is the Java
    ''' <c>TestTransforms.getSchema</c> helper (without its accidental fall-through from the Float case into the
    ''' Categorical case, which added a second, state-less categorical column named "column").
    ''' <para>Not ported in this wave, because every one of their test methods needs code that is not ported yet:
    ''' <c>TestTransforms</c>, <c>transform/TestJsonYaml</c>, <c>transform/RegressionTestJson</c> and
    ''' <c>transform/parse/*</c> (the transform implementations), <c>TestTransformProcess</c>, <c>TestJoin</c>,
    ''' <c>serde/*</c> and <c>schema/TestJsonYaml</c> (TransformProcess and the JSON/YAML serde), <c>reduce/*</c>
    ''' and <c>stringreduce/*</c> (the reducers), <c>sequence/*</c> (sequence operations), <c>transform/ndarray/*</c>
    ''' (INDArray) and <c>ui/*</c> (the HTML templates).</para>
    ''' </summary>
    Friend Module TransformTestSupport

        ''' <summary>A one-column schema named "column" of the given type (categorical: with the given state names).</summary>
        Public Function getSchema(ByVal type As ColumnType, ByVal ParamArray colNames() As String) As SchemaT
            Dim builder As New SchemaT.Builder()
            Select Case type
                Case ColumnType.[String]
                    builder.addColumnString("column")
                Case ColumnType.[Integer]
                    builder.addColumnInteger("column")
                Case ColumnType.[Long]
                    builder.addColumnLong("column")
                Case ColumnType.[Double]
                    builder.addColumnDouble("column")
                Case ColumnType.Float
                    builder.addColumnFloat("column")
                Case ColumnType.Categorical
                    builder.addColumnCategorical("column", colNames)
                Case ColumnType.Time
                    builder.addColumnTime("column", DateTimeZone.UTC)
                Case Else
                    Throw New Exception()
            End Select
            Return builder.build()
        End Function

    End Module

End Namespace
