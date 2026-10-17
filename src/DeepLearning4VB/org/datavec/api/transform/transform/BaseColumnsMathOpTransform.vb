Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms

    ''' <summary>
    ''' Base class of the transforms that add a new column computed from two or more columns with a
    ''' <see cref="MathOp"/> (Add, Subtract, Multiply, Divide, Modulus). Equality (Lombok, excluding columnIdxs and
    ''' inputSchema) compares the class, the new column name, the op and the columns.
    ''' </summary>
    ' Java: @JsonInclude(NON_NULL) @JsonIgnoreProperties({"columnIdxs", "inputSchema"})
    '       @EqualsAndHashCode(exclude = {"columnIdxs", "inputSchema"}) @Data
    <Serializable>
    Public MustInherit Class BaseColumnsMathOpTransform
        Implements TransformT

        Protected Friend ReadOnly newColumnName As String
        Protected Friend ReadOnly mathOpField As MathOp
        Protected Friend ReadOnly columns() As String
        Private columnIdxs() As Integer
        Private inputSchemaField As SchemaT

        Public Sub New(ByVal newColumnName As String, ByVal mathOp As MathOp, ByVal ParamArray columns() As String)
            If columns Is Nothing OrElse columns.Length = 0 Then
                Throw New ArgumentException("Invalid input: cannot have null/0 columns")
            End If
            Me.newColumnName = newColumnName
            Me.mathOpField = mathOp
            Me.columns = columns

            Select Case mathOp
                Case MathOp.Add
                    If columns.Length < 2 Then
                        Throw New ArgumentException("Need 2 or more columns for Add op. Got: " & Arrays.toString(columns))
                    End If
                Case MathOp.Subtract
                    If columns.Length <> 2 Then
                        Throw New ArgumentException("Need exactly 2 columns for Subtract op. Got: " & Arrays.toString(columns))
                    End If
                Case MathOp.Multiply
                    If columns.Length < 2 Then
                        Throw New ArgumentException("Need 2 or more columns for Multiply op. Got: " & Arrays.toString(columns))
                    End If
                Case MathOp.Divide
                    If columns.Length <> 2 Then
                        Throw New ArgumentException("Need exactly 2 columns for Divide op. Got: " & Arrays.toString(columns))
                    End If
                Case MathOp.Modulus
                    If columns.Length <> 2 Then
                        Throw New ArgumentException("Need exactly 2 columns for Modulus op. Got: " & Arrays.toString(columns))
                    End If
                Case MathOp.ReverseSubtract, MathOp.ReverseDivide, MathOp.ScalarMin, MathOp.ScalarMax
                    Throw New ArgumentException("Invalid MathOp: cannot use " & mathOp.ToString() & " with ...ColumnsMathOpTransform")
                Case Else
                    Throw New Exception("Unknown MathOp: " & mathOp.ToString())
            End Select
        End Sub

        ''' <summary>Java <c>getNewColumnName()</c> (Lombok).</summary>
        Public Function getNewColumnName() As String
            Return newColumnName
        End Function

        ''' <summary>Java <c>getMathOp()</c> (Lombok).</summary>
        Public Function getMathOp() As MathOp
            Return mathOpField
        End Function

        ''' <summary>Java <c>getColumns()</c> (Lombok).</summary>
        Public Function getColumns() As String()
            Return columns
        End Function

        ''' <summary>Java <c>getColumnIdxs()</c> (Lombok).</summary>
        Public Function getColumnIdxs() As Integer()
            Return columnIdxs
        End Function

        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            For Each name As String In columns
                If Not inputSchema.hasColumn(name) Then
                    Throw New InvalidOperationException("Input schema does not have column with name """ & name & """")
                End If
            Next name

            Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(inputSchema.getColumnMetaData())

            newMeta.Add(derivedColumnMetaData(newColumnName, inputSchema))

            Return inputSchema.newSchema(newMeta)
        End Function

        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Set(ByVal value As SchemaT)
                columnIdxs = New Integer(columns.Length - 1) {}
                Dim i As Integer = 0
                For Each name As String In columns
                    If Not value.hasColumn(name) Then
                        Throw New InvalidOperationException("Input schema does not have column with name """ & name & """")
                    End If
                    columnIdxs(i) = value.getIndexOfColumn(name)
                    i += 1
                Next name

                Me.inputSchemaField = value
            End Set
            Get
                Return inputSchemaField
            End Get
        End Property

        Public Overridable Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT) Implements TransformT.map
            If inputSchemaField Is Nothing Then
                Throw New InvalidOperationException("Input schema has not been set")
            End If
            Dim [out] As IList(Of WritableT) = New List(Of WritableT)(writables)

            Dim temp(columns.Length - 1) As WritableT
            For i As Integer = 0 To columnIdxs.Length - 1
                temp(i) = [out](columnIdxs(i))
            Next i

            [out].Add(doOp(temp))
            Return [out]
        End Function

        Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of WritableT)) Implements TransformT.mapSequence
            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            For Each [step] As IList(Of WritableT) In sequence
                [out].Add(map([step]))
            Next [step]
            Return [out]
        End Function

        Public MustOverride Function mapSequence(ByVal sequence As Object) As Object Implements TransformT.mapSequence

        Public MustOverride Function map(ByVal input As Object) As Object Implements TransformT.map

        ''' <summary>
        ''' The output column name
        ''' after the operation has been applied
        ''' </summary>
        ''' <returns> the output column name </returns>
        Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
            Return newColumnName
        End Function

        ''' <summary>
        ''' The output column names
        ''' This will often be the same as the input
        ''' </summary>
        ''' <returns> the output column names </returns>
        Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
            Return New String() {newColumnName}
        End Function

        ''' <summary>
        ''' Returns column names
        ''' this op is meant to run on
        ''' </summary>
        Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
            Return columns
        End Function

        ''' <summary>
        ''' Returns a singular column name
        ''' this op is meant to run on
        ''' </summary>
        Public Overridable Function columnName() As String Implements ColumnOp.columnName
            Return columnNames()(0)
        End Function

        Protected Friend MustOverride Function derivedColumnMetaData(ByVal newColumnName As String, ByVal inputSchema As SchemaT) As ColumnMetaData

        Protected Friend MustOverride Function doOp(ByVal ParamArray input() As WritableT) As WritableT

        Public MustOverride Overrides Function ToString() As String

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim o As BaseColumnsMathOpTransform = DirectCast(obj, BaseColumnsMathOpTransform)
            Return String.Equals(newColumnName, o.newColumnName) AndAlso mathOpField = o.mathOpField AndAlso javaEquals(columns, o.columns)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(newColumnName, mathOpField, columns)
        End Function

    End Class

End Namespace
