Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.column

    ''' <summary>
    ''' Rearrange the order of the columns.
    ''' Note: A partial list of columns can be used here. Any columns that are not explicitly mentioned
    ''' will be placed after those that are in the output, without changing their relative order.
    ''' <para>Equality compares the class, the new order and the computed output order. Unlike Java, setting the
    ''' input schema also stores it, so that <see cref="columnNames"/> works.</para>
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema", "outputOrder"}) @Data
    <JsonIgnoreProperties("inputSchema", "outputOrder")>
    <Serializable>
    Public Class ReorderColumnsTransform
        Implements TransformT

        Private ReadOnly newOrder As IList(Of String)
        Private inputSchemaField As SchemaT
        Private outputOrder() As Integer 'Mapping from in to out. so output[i] = input.get(outputOrder[i])

        ''' <param name="newOrder"> A partial or complete order of the columns in the output </param>
        Public Sub New(ByVal ParamArray newOrder() As String)
            Me.New(Arrays.asList(newOrder))
        End Sub

        ''' <param name="newOrder"> A partial or complete order of the columns in the output </param>
        <JsonConstructor>
        Public Sub New(ByVal newOrder As IList(Of String))
            Me.newOrder = newOrder
        End Sub

        ''' <summary>Java <c>getNewOrder()</c> (Lombok).</summary>
        Public Function getNewOrder() As IList(Of String)
            Return newOrder
        End Function

        ''' <summary>Java <c>getOutputOrder()</c> (Lombok).</summary>
        Public Function getOutputOrder() As Integer()
            Return outputOrder
        End Function

        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            For Each s As String In newOrder
                If Not inputSchema.hasColumn(s) Then
                    Throw New InvalidOperationException("Input schema does not contain column with name """ & s & """")
                End If
            Next s
            If inputSchema.numColumns() < newOrder.Count Then
                Throw New ArgumentException("Schema has " & inputSchema.numColumns() & " column but newOrder has " & newOrder.Count & " columns")
            End If

            Dim origNames As IList(Of String) = inputSchema.getColumnNames()
            Dim origMeta As IList(Of ColumnMetaData) = inputSchema.getColumnMetaData()
            Dim outMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)()

            Dim taken(origNames.Count - 1) As Boolean
            For Each s As String In newOrder
                Dim idx As Integer = inputSchema.getIndexOfColumn(s)
                outMeta.Add(origMeta(idx))
                taken(idx) = True
            Next s

            For i As Integer = 0 To taken.Length - 1
                If taken(i) Then
                    Continue For
                End If
                outMeta.Add(origMeta(i))
            Next i

            Return inputSchema.newSchema(outMeta)
        End Function

        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Set(ByVal value As SchemaT)
                For Each s As String In newOrder
                    If Not value.hasColumn(s) Then
                        Throw New InvalidOperationException("Input schema does not contain column with name """ & s & """")
                    End If
                Next s
                If value.numColumns() < newOrder.Count Then
                    Throw New ArgumentException("Schema has " & value.numColumns() & " columns but newOrder has " & newOrder.Count & " columns")
                End If

                Dim origNames As IList(Of String) = value.getColumnNames()
                outputOrder = New Integer(origNames.Count - 1) {}

                Dim taken(origNames.Count - 1) As Boolean
                Dim j As Integer = 0
                For Each s As String In newOrder
                    Dim idx As Integer = value.getIndexOfColumn(s)
                    taken(idx) = True
                    outputOrder(j) = idx
                    j += 1
                Next s

                For i As Integer = 0 To taken.Length - 1
                    If taken(i) Then
                        Continue For
                    End If
                    outputOrder(j) = i
                    j += 1
                Next i

                Me.inputSchemaField = value
            End Set
            Get
                Return inputSchemaField
            End Get
        End Property

        Public Overridable Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT) Implements TransformT.map
            Dim [out] As IList(Of WritableT) = New List(Of WritableT)()
            For Each i As Integer In outputOrder
                [out].Add(writables(i))
            Next i
            Return [out]
        End Function

        Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of WritableT)) Implements TransformT.mapSequence
            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            For Each [step] As IList(Of WritableT) In sequence
                [out].Add(map([step]))
            Next [step]
            Return [out]
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overridable Function map(ByVal input As Object) As Object Implements TransformT.map
            Throw New NotSupportedException("Unable to map. Please treat this as a special operation. This should be handled by your implementation.")
        End Function

        ''' <summary>
        ''' Transform a sequence
        ''' </summary>
        ''' <param name="sequence"> </param>
        Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements TransformT.mapSequence
            Throw New NotSupportedException("Unable to map. Please treat this as a special operation. This should be handled by your implementation.")
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then
                Return True
            End If
            If o Is Nothing OrElse Me.GetType() IsNot o.GetType() Then
                Return False
            End If

            Dim o2 As ReorderColumnsTransform = DirectCast(o, ReorderColumnsTransform)

            If Not javaEquals(newOrder, o2.newOrder) Then
                Return False
            End If
            Return javaEquals(outputOrder, o2.outputOrder)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = javaHashCode(newOrder)
            result = 31 * result + javaHashCode(outputOrder)
            Return result
        End Function

        Public Overrides Function ToString() As String
            Return "ReorderColumnsTransform(newOrder=" & javaToString(newOrder) & ")"
        End Function

        ''' <summary>
        ''' The output column name
        ''' after the operation has been applied
        ''' </summary>
        ''' <returns> the output column name </returns>
        Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
            Return outputColumnNames()(0)
        End Function

        ''' <summary>
        ''' The output column names
        ''' This will often be the same as the input
        ''' </summary>
        ''' <returns> the output column names </returns>
        Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
            Return New List(Of String)(newOrder).ToArray()
        End Function

        ''' <summary>
        ''' Returns column names
        ''' this op is meant to run on
        ''' </summary>
        Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
            Return New List(Of String)(InputSchema.getColumnNames()).ToArray()
        End Function

        ''' <summary>
        ''' Returns a singular column name
        ''' this op is meant to run on
        ''' </summary>
        Public Overridable Function columnName() As String Implements ColumnOp.columnName
            Return columnNames()(0)
        End Function

    End Class

End Namespace
