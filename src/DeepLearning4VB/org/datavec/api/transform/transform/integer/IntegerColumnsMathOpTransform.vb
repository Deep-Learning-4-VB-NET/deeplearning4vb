Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports IntegerMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.IntegerMetaData
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.integer

    ''' <summary>
    ''' Add a new integer column, calculated from one or more other columns. A new column (with the specified name) is added
    ''' as the final column of the output. No other columns are modified.<br/>
    ''' For example, if newColumnName=="newCol", mathOp==MathOp.Add, and columns=={"col1","col2"}, then the output column
    ''' with name "newCol" has value col1+col2.<br/>
    ''' <b>NOTE</b>: Division here is using integer division (integer output). Use DoubleColumnsMathOpTransform
    ''' if a decimal output value is required.
    ''' Equality is the base class's (new column name, op and columns).
    ''' </summary>
    ''' <seealso cref="IntegerMathOpTransform"/>
    ' Java: @Data
    <Serializable>
    Public Class IntegerColumnsMathOpTransform
        Inherits BaseColumnsMathOpTransform

        ''' <param name="newColumnName"> Name of the new column (output column) </param>
        ''' <param name="mathOp">        Mathematical operation. Only Add/Subtract/Multiply/Divide/Modulus is allowed here </param>
        ''' <param name="columns">       Columns to use in the mathematical operation </param>
        <JsonConstructor>
        Public Sub New(ByVal newColumnName As String, ByVal mathOp As MathOp, ByVal ParamArray columns() As String)
            MyBase.New(newColumnName, mathOp, columns)
        End Sub

        Protected Friend Overrides Function derivedColumnMetaData(ByVal newColumnName As String, ByVal inputSchema As SchemaT) As ColumnMetaData
            Return New IntegerMetaData(newColumnName)
        End Function

        Protected Friend Overrides Function doOp(ByVal ParamArray input() As WritableT) As WritableT
            Select Case mathOpField
                Case MathOp.Add
                    Dim sum As Integer = 0
                    For Each w As WritableT In input
                        sum += w.toInt()
                    Next w
                    Return New IntWritable(sum)
                Case MathOp.Subtract
                    Return New IntWritable(input(0).toInt() - input(1).toInt())
                Case MathOp.Multiply
                    Dim product As Integer = 1
                    For Each w As WritableT In input
                        product *= w.toInt()
                    Next w
                    Return New IntWritable(product)
                Case MathOp.Divide
                    Return New IntWritable(input(0).toInt() \ input(1).toInt())
                Case MathOp.Modulus
                    Return New IntWritable(input(0).toInt() Mod input(1).toInt())
                Case Else
                    Throw New Exception("Invalid mathOp: " & mathOpField.ToString()) 'Should never happen
            End Select
        End Function

        Public Overrides Function ToString() As String
            Return "IntegerColumnsMathOpTransform(newColumnName=""" & newColumnName & """,mathOp=" & mathOpField.ToString() & ",columns=" & Arrays.toString(columns) & ")"
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object: the op applied to a list of (boxed) integers. As in Java, Subtract returns an
        ''' <c>IntWritable</c> and the other ops an Integer.
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Dim list As IList(Of Integer) = toInts(input)
            Select Case mathOpField
                Case MathOp.Add
                    Dim sum As Integer = 0
                    For Each w As Integer In list
                        sum += w
                    Next w
                    Return sum
                Case MathOp.Subtract
                    Return New IntWritable(list(0) - list(1))
                Case MathOp.Multiply
                    Dim product As Integer = 1
                    For Each w As Integer In list
                        product *= w
                    Next w
                    Return product
                Case MathOp.Divide
                    Return list(0) \ list(1)
                Case MathOp.Modulus
                    Return list(0) Mod list(1)
                Case Else
                    Throw New Exception("Invalid mathOp: " & mathOpField.ToString()) 'Should never happen
            End Select
        End Function

        ''' <summary>
        ''' Transform a sequence: a list of rows (lists of integers) becomes the list of results
        ''' </summary>
        ''' <param name="sequence"> </param>
        Public Overrides Function mapSequence(ByVal sequence As Object) As Object
            Dim seq As System.Collections.IList = DirectCast(sequence, System.Collections.IList)
            Dim ret As IList(Of Integer) = New List(Of Integer)()
            For Each [step] As Object In seq
                ret.Add(DirectCast(map([step]), Integer))
            Next [step]
            Return ret
        End Function

        ''' <summary>Java <c>(List&lt;Integer&gt;) input</c>: every element must be a boxed Integer.</summary>
        Private Shared Function toInts(ByVal input As Object) As IList(Of Integer)
            Dim list As System.Collections.IList = DirectCast(input, System.Collections.IList)
            Dim ret As New List(Of Integer)(list.Count)
            For Each o As Object In list
                ret.Add(DirectCast(o, Integer))
            Next o
            Return ret
        End Function

    End Class

End Namespace
