Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports DoubleMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.DoubleMetaData
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.doubletransform

    ''' <summary>
    ''' Add a new double column, calculated from one or more other columns. A new column (with the specified name) is added
    ''' as the final column of the output. No other columns are modified.<br/>
    ''' For example, if newColumnName=="newCol", mathOp==MathOp.Add, and columns=={"col1","col2"}, then the output column
    ''' with name "newCol" has value col1+col2.<br/>
    ''' <b>NOTE</b>: Division here is using double precision division.
    ''' Equality is the base class's (new column name, op and columns).
    ''' </summary>
    ''' <seealso cref="DoubleMathOpTransform"/>
    ' Java: @Data
    <Serializable>
    Public Class DoubleColumnsMathOpTransform
        Inherits BaseColumnsMathOpTransform

        <JsonConstructor>
        Public Sub New(ByVal newColumnName As String, ByVal mathOp As MathOp, ByVal columns As IList(Of String))
            Me.New(newColumnName, mathOp, New List(Of String)(columns).ToArray())
        End Sub

        ''' <param name="newColumnName"> Name of the new column (output column) </param>
        ''' <param name="mathOp">        Mathematical operation. Only Add/Subtract/Multiply/Divide/Modulus is allowed here </param>
        ''' <param name="columns">       Columns to use in the mathematical operation </param>
        Public Sub New(ByVal newColumnName As String, ByVal mathOp As MathOp, ByVal ParamArray columns() As String)
            MyBase.New(newColumnName, mathOp, columns)
        End Sub

        Protected Friend Overrides Function derivedColumnMetaData(ByVal newColumnName As String, ByVal inputSchema As SchemaT) As ColumnMetaData
            Return New DoubleMetaData(newColumnName)
        End Function

        Protected Friend Overrides Function doOp(ByVal ParamArray input() As WritableT) As WritableT
            Select Case mathOpField
                Case MathOp.Add
                    Dim sum As Double = 0
                    For Each w As WritableT In input
                        sum += w.toDouble()
                    Next w
                    Return New DoubleWritable(sum)
                Case MathOp.Subtract
                    Return New DoubleWritable(input(0).toDouble() - input(1).toDouble())
                Case MathOp.Multiply
                    Dim product As Double = 1.0
                    For Each w As WritableT In input
                        product *= w.toDouble()
                    Next w
                    Return New DoubleWritable(product)
                Case MathOp.Divide
                    Return New DoubleWritable(input(0).toDouble() / input(1).toDouble())
                Case MathOp.Modulus
                    Return New DoubleWritable(input(0).toDouble() Mod input(1).toDouble())
                Case Else
                    Throw New Exception("Invalid mathOp: " & mathOpField.ToString()) 'Should never happen
            End Select
        End Function

        Public Overrides Function ToString() As String
            Return "DoubleColumnsMathOpTransform(newColumnName=""" & newColumnName & """,mathOp=" & mathOpField.ToString() & ",columns=" & Arrays.toString(columns) & ")"
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object: the op applied to a list of (boxed) doubles
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Dim row As IList(Of Double) = toDoubles(input)
            Select Case mathOpField
                Case MathOp.Add
                    Dim sum As Double = 0
                    For Each w As Double In row
                        sum += w
                    Next w
                    Return sum
                Case MathOp.Subtract
                    Return row(0) - row(1)
                Case MathOp.Multiply
                    Dim product As Double = 1.0
                    For Each w As Double In row
                        product *= w
                    Next w
                    Return product
                Case MathOp.Divide
                    Return row(0) / row(1)
                Case MathOp.Modulus
                    Return row(0) Mod row(1)
                Case Else
                    Throw New Exception("Invalid mathOp: " & mathOpField.ToString()) 'Should never happen
            End Select
        End Function

        ''' <summary>
        ''' Transform a sequence: a list of rows (lists of doubles) becomes the list of results
        ''' </summary>
        ''' <param name="sequence"> </param>
        Public Overrides Function mapSequence(ByVal sequence As Object) As Object
            Dim seq As System.Collections.IList = DirectCast(sequence, System.Collections.IList)
            Dim ret As IList(Of Double) = New List(Of Double)()
            For Each [step] As Object In seq
                ret.Add(DirectCast(map([step]), Double))
            Next [step]
            Return ret
        End Function

        ''' <summary>Java <c>(List&lt;Double&gt;) input</c>: every element must be a boxed Double.</summary>
        Private Shared Function toDoubles(ByVal input As Object) As IList(Of Double)
            Dim list As System.Collections.IList = DirectCast(input, System.Collections.IList)
            Dim ret As New List(Of Double)(list.Count)
            For Each o As Object In list
                ret.Add(DirectCast(o, Double))
            Next o
            Return ret
        End Function

    End Class

End Namespace
