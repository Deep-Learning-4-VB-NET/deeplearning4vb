Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports LongMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.LongMetaData
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.longtransform

    ''' <summary>
    ''' Add a new long column, calculated from one or more other columns. A new column (with the specified name) is added
    ''' as the final column of the output. No other columns are modified.<br/>
    ''' For example, if newColumnName=="newCol", mathOp==MathOp.Add, and columns=={"col1","col2"}, then the output column
    ''' with name "newCol" has value col1+col2.<br/>
    ''' <b>NOTE</b>: Division here is using long division (long output). Use DoubleColumnsMathOpTransform
    ''' if a decimal output value is required.
    ''' Equality is the base class's (new column name, op and columns).
    ''' </summary>
    ''' <seealso cref="LongMathOpTransform"/>
    ' Java: @Data
    <Serializable>
    Public Class LongColumnsMathOpTransform
        Inherits BaseColumnsMathOpTransform

        ''' <param name="newColumnName"> Name of the new column (output column) </param>
        ''' <param name="mathOp">        Mathematical operation. Only Add/Subtract/Multiply/Divide/Modulus is allowed here </param>
        ''' <param name="columns">       Columns to use in the mathematical operation </param>
        <JsonConstructor>
        Public Sub New(ByVal newColumnName As String, ByVal mathOp As MathOp, ByVal ParamArray columns() As String)
            MyBase.New(newColumnName, mathOp, columns)
        End Sub

        Protected Friend Overrides Function derivedColumnMetaData(ByVal newColumnName As String, ByVal inputSchema As SchemaT) As ColumnMetaData
            Return New LongMetaData(newColumnName)
        End Function

        Protected Friend Overrides Function doOp(ByVal ParamArray input() As WritableT) As WritableT
            Select Case mathOpField
                Case MathOp.Add
                    Dim sum As Long = 0
                    For Each w As WritableT In input
                        sum += w.toLong()
                    Next w
                    Return New LongWritable(sum)
                Case MathOp.Subtract
                    Return New LongWritable(input(0).toLong() - input(1).toLong())
                Case MathOp.Multiply
                    Dim product As Long = 1
                    For Each w As WritableT In input
                        product *= w.toLong()
                    Next w
                    Return New LongWritable(product)
                Case MathOp.Divide
                    Return New LongWritable(input(0).toLong() \ input(1).toLong())
                Case MathOp.Modulus
                    Return New LongWritable(input(0).toLong() Mod input(1).toLong())
                Case Else
                    Throw New Exception("Invalid mathOp: " & mathOpField.ToString()) 'Should never happen
            End Select
        End Function

        Public Overrides Function ToString() As String
            Return "LongColumnsMathOpTransform(newColumnName=""" & newColumnName & """,mathOp=" & mathOpField.ToString() & ",columns=" & Arrays.toString(columns) & ")"
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object: the op applied to a list of (boxed) longs. As in Java, Add returns a
        ''' <c>LongWritable</c> and the other ops a Long.
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Dim list As IList(Of Long) = toLongs(input)
            Select Case mathOpField
                Case MathOp.Add
                    Dim sum As Long = 0
                    For Each w As Long In list
                        sum += w
                    Next w
                    Return New LongWritable(sum)
                Case MathOp.Subtract
                    Return list(0) - list(1)
                Case MathOp.Multiply
                    Dim product As Long = 1
                    For Each w As Long In list
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
        ''' Transform a sequence: a list of rows (lists of longs) becomes the list of results
        ''' </summary>
        ''' <param name="sequence"> </param>
        Public Overrides Function mapSequence(ByVal sequence As Object) As Object
            Dim seq As System.Collections.IList = DirectCast(sequence, System.Collections.IList)
            Dim ret As IList(Of Long) = New List(Of Long)()
            For Each l As Object In seq
                ret.Add(DirectCast(map(l), Long))
            Next l
            Return ret
        End Function

        ''' <summary>Java <c>(List&lt;Long&gt;) input</c>: every element must be a boxed Long.</summary>
        Private Shared Function toLongs(ByVal input As Object) As IList(Of Long)
            Dim list As System.Collections.IList = DirectCast(input, System.Collections.IList)
            Dim ret As New List(Of Long)(list.Count)
            For Each o As Object In list
                ret.Add(DirectCast(o, Long))
            Next o
            Return ret
        End Function

    End Class

End Namespace
