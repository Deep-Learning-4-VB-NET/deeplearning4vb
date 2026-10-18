Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ConditionOp = DeepLearning4VB.org.datavec.api.transform.condition.ConditionOp
Imports DoubleColumnCondition = DeepLearning4VB.org.datavec.api.transform.condition.column.DoubleColumnCondition
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports FloatWritable = DeepLearning4VB.org.datavec.api.writable.FloatWritable
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports LocalTransformExecutorT = DeepLearning4VB.org.datavec.local.transforms.LocalTransformExecutor
Imports MathOp = DeepLearning4VB.org.datavec.api.transform.MathOp
Imports ReduceOp = DeepLearning4VB.org.datavec.api.transform.ReduceOp
Imports ReducerT = DeepLearning4VB.org.datavec.api.transform.reduce.Reducer
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.transform

    ''' <summary>Port of the DataVec (datavec-local) <c>ExecutionTest</c> JUnit class.</summary>
    <TestClass>
    Public Class ExecutionTest

        Private Shared Function row(ByVal ParamArray values() As WritableT) As IList(Of WritableT)
            Return New List(Of WritableT)(values)
        End Function

        ' Java: testExecutionNdarray() is not ported: NDArray columns and ndArrayMathFunctionTransform need the INDArray tensor API.

        <TestMethod>
        Public Sub testExecutionSimple()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnInteger("col0").addColumnCategorical("col1", "state0", "state1", "state2").addColumnDouble("col2").addColumnFloat("col3").build()

            Dim tp As TransformProcessT = (New TransformProcessT.Builder(schema)).categoricalToInteger("col1").doubleMathOp("col2", MathOp.Add, 10.0).floatMathOp("col3", MathOp.Add, 5.0F).build()

            Dim inputData As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            inputData.Add(row(New IntWritable(0), New Text("state2"), New DoubleWritable(0.1), New FloatWritable(0.3F)))
            inputData.Add(row(New IntWritable(1), New Text("state1"), New DoubleWritable(1.1), New FloatWritable(1.7F)))
            inputData.Add(row(New IntWritable(2), New Text("state0"), New DoubleWritable(2.1), New FloatWritable(3.6F)))

            Dim rdd As IList(Of IList(Of WritableT)) = inputData

            Dim [out] As IList(Of IList(Of WritableT)) = LocalTransformExecutorT.execute(rdd, tp).OrderBy(Function(o) o(0).toInt()).ToList()

            Dim expected As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            expected.Add(row(New IntWritable(0), New IntWritable(2), New DoubleWritable(10.1), New FloatWritable(5.3F)))
            expected.Add(row(New IntWritable(1), New IntWritable(1), New DoubleWritable(11.1), New FloatWritable(6.7F)))
            expected.Add(row(New IntWritable(2), New IntWritable(0), New DoubleWritable(12.1), New FloatWritable(8.6F)))

            assertEquals(expected, [out])
        End Sub

        <TestMethod>
        Public Sub testFilter()
            Dim filterSchema As SchemaT = (New SchemaT.Builder()).addColumnDouble("col1").addColumnDouble("col2").addColumnDouble("col3").build()
            Dim inputData As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            inputData.Add(row(New IntWritable(0), New DoubleWritable(1), New DoubleWritable(0.1)))
            inputData.Add(row(New IntWritable(1), New DoubleWritable(3), New DoubleWritable(1.1)))
            inputData.Add(row(New IntWritable(2), New DoubleWritable(3), New DoubleWritable(2.1)))
            Dim transformProcess As TransformProcessT = (New TransformProcessT.Builder(filterSchema)).filter(New DoubleColumnCondition("col1", ConditionOp.LessThan, 1)).build()
            Dim execute As IList(Of IList(Of WritableT)) = LocalTransformExecutorT.execute(inputData, transformProcess)
            assertEquals(2, execute.Count)
        End Sub

        <TestMethod>
        Public Sub testExecutionSequence()

            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnInteger("col0").addColumnCategorical("col1", "state0", "state1", "state2").addColumnDouble("col2").build()

            Dim tp As TransformProcessT = (New TransformProcessT.Builder(schema)).categoricalToInteger("col1").doubleMathOp("col2", MathOp.Add, 10.0).build()

            Dim inputSequences As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()
            Dim seq1 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            seq1.Add(row(New IntWritable(0), New Text("state2"), New DoubleWritable(0.1)))
            seq1.Add(row(New IntWritable(1), New Text("state1"), New DoubleWritable(1.1)))
            seq1.Add(row(New IntWritable(2), New Text("state0"), New DoubleWritable(2.1)))
            Dim seq2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            seq2.Add(row(New IntWritable(3), New Text("state0"), New DoubleWritable(3.1)))
            seq2.Add(row(New IntWritable(4), New Text("state1"), New DoubleWritable(4.1)))

            inputSequences.Add(seq1)
            inputSequences.Add(seq2)

            Dim rdd As IList(Of IList(Of IList(Of WritableT))) = inputSequences

            'Java sorted by descending sequence length
            Dim [out] As IList(Of IList(Of IList(Of WritableT))) = LocalTransformExecutorT.executeSequenceToSequence(rdd, tp).OrderByDescending(Function(o) o.Count).ToList()

            Dim expectedSequence As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()
            Dim seq1e As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            seq1e.Add(row(New IntWritable(0), New IntWritable(2), New DoubleWritable(10.1)))
            seq1e.Add(row(New IntWritable(1), New IntWritable(1), New DoubleWritable(11.1)))
            seq1e.Add(row(New IntWritable(2), New IntWritable(0), New DoubleWritable(12.1)))
            Dim seq2e As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            seq2e.Add(row(New IntWritable(3), New IntWritable(0), New DoubleWritable(13.1)))
            seq2e.Add(row(New IntWritable(4), New IntWritable(1), New DoubleWritable(14.1)))

            expectedSequence.Add(seq1e)
            expectedSequence.Add(seq2e)

            assertEquals(expectedSequence, [out])
        End Sub

        <TestMethod>
        Public Sub testReductionGlobal()

            Dim [in] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New Text("first"), New DoubleWritable(3.0)),
                row(New Text("second"), New DoubleWritable(5.0))
            }

            Dim inData As IList(Of IList(Of WritableT)) = [in]

            Dim s As SchemaT = (New SchemaT.Builder()).addColumnString("textCol").addColumnDouble("doubleCol").build()

            Dim tp As TransformProcessT = (New TransformProcessT.Builder(s)).reduce((New ReducerT.Builder(ReduceOp.TakeFirst)).takeFirstColumns("textCol").meanColumns("doubleCol").build()).build()

            Dim outRdd As IList(Of IList(Of WritableT)) = LocalTransformExecutorT.execute(inData, tp)

            Dim [out] As IList(Of IList(Of WritableT)) = outRdd

            Dim expOut As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(New Text("first"), New DoubleWritable(4.0))}

            assertEquals(expOut, [out])
        End Sub

        <TestMethod>
        Public Sub testReductionByKey()

            Dim [in] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New IntWritable(0), New Text("first"), New DoubleWritable(3.0)),
                row(New IntWritable(0), New Text("second"), New DoubleWritable(5.0)),
                row(New IntWritable(1), New Text("f"), New DoubleWritable(30.0)),
                row(New IntWritable(1), New Text("s"), New DoubleWritable(50.0))
            }

            Dim inData As IList(Of IList(Of WritableT)) = [in]

            Dim s As SchemaT = (New SchemaT.Builder()).addColumnInteger("intCol").addColumnString("textCol").addColumnDouble("doubleCol").build()

            Dim tp As TransformProcessT = (New TransformProcessT.Builder(s)).reduce((New ReducerT.Builder(ReduceOp.TakeFirst)).keyColumns("intCol").takeFirstColumns("textCol").meanColumns("doubleCol").build()).build()

            Dim outRdd As IList(Of IList(Of WritableT)) = LocalTransformExecutorT.execute(inData, tp)

            Dim [out] As IList(Of IList(Of WritableT)) = outRdd

            Dim expOut As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New IntWritable(0), New Text("first"), New DoubleWritable(4.0)),
                row(New IntWritable(1), New Text("f"), New DoubleWritable(40.0))
            }

            [out] = [out].OrderBy(Function(o) o(0).toInt()).ToList()

            assertEquals(expOut, [out])
        End Sub

    End Class

End Namespace
