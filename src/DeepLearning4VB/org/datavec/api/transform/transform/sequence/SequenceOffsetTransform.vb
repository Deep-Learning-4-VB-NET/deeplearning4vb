Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.sequence

    ''' <summary>
    ''' Sequence offset transform takes a sequence, and shifts The values in one or more columns by a specified number of
    ''' times steps. It has 2 modes of operation (OperationType enum), with respect to the columns it operates on:<br/>
    ''' InPlace: operations may be performed in-place, modifying the values in the specified columns<br/>
    ''' NewColumn: operations may produce new columns, with the original (source) columns remaining unmodified<br/>
    ''' <br/>
    ''' Additionally, there are 2 modes for handling values outside the original sequence (EdgeHandling enum):
    ''' TrimSequence: the entire sequence is trimmed (start or end) by a specified number of steps<br/>
    ''' SpecifiedValue: for any values outside of the original sequence, they are given a specified value<br/>
    ''' <br/>
    ''' Note 1: When specifying offsets, they are done as follows:
    ''' Positive offsets: move the values in the specified columns to a later time. Earlier time steps are either be trimmed
    ''' or Given specified values; the last values in these columns will be truncated/removed.
    ''' <br/>
    ''' Note 2: Care must be taken when using TrimSequence: for example, if we chain multiple sequence offset transforms on the
    ''' one dataset, we may end up trimming much more than we want. In this case, it may be better to use SpecifiedValue,
    ''' (with, for example, NullWritable) and then do a single trim operation (via SequenceTrimTransform) at the end.
    ''' <para>Equality (Lombok, excluding columnsToOffsetSet and inputSchema) compares the class, the columns, the
    ''' offset, both modes and the edge case value. With <c>OperationType.InPlace</c> the output schema keeps the
    ''' (renamed) offset columns; Java dropped them from the schema while still emitting their values.</para>
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema", "columnsToOffsetSet"}) @JsonInclude(NON_NULL) @Data
    '       @EqualsAndHashCode(exclude = {"columnsToOffsetSet", "inputSchema"})
    <JsonIgnoreProperties("inputSchema", "columnsToOffsetSet")>
    <JsonIncludeNonNull>
    <Serializable>
    Public Class SequenceOffsetTransform
        Implements TransformT

        <JsonConverter(GetType(JsonStringEnumConverter))>
        Public Enum OperationType
            InPlace
            NewColumn
        End Enum

        <JsonConverter(GetType(JsonStringEnumConverter))>
        Public Enum EdgeHandling
            TrimSequence
            SpecifiedValue
        End Enum

        Private columnsToOffset As IList(Of String)
        Private offsetAmount As Integer
        Private operationTypeField As OperationType
        Private edgeHandlingField As EdgeHandling
        Private edgeCaseValue As WritableT

        Private columnsToOffsetSet As ISet(Of String)
        Private inputSchemaField As SchemaT

        <JsonConstructor>
        Public Sub New(ByVal columnsToOffset As IList(Of String), ByVal offsetAmount As Integer, ByVal operationType As OperationType, ByVal edgeHandling As EdgeHandling, ByVal edgeCaseValue As WritableT)
            If edgeCaseValue IsNot Nothing AndAlso edgeHandling <> EdgeHandling.SpecifiedValue Then
                Throw New NotSupportedException("edgeCaseValue was non-null, but EdgeHandling was not set to SpecifiedValue. " & "edgeCaseValue can only be used with SpecifiedValue mode")
            End If

            Me.columnsToOffset = columnsToOffset
            Me.offsetAmount = offsetAmount
            Me.operationTypeField = operationType
            Me.edgeHandlingField = edgeHandling
            Me.edgeCaseValue = edgeCaseValue

            Me.columnsToOffsetSet = New HashSet(Of String)(columnsToOffset)
        End Sub

        ''' <summary>Java <c>getColumnsToOffset()</c> (Lombok).</summary>
        Public Function getColumnsToOffset() As IList(Of String)
            Return columnsToOffset
        End Function

        ''' <summary>Java <c>setColumnsToOffset()</c> (Lombok; does not update the lookup set, as in Java).</summary>
        Public Sub setColumnsToOffset(ByVal columnsToOffset As IList(Of String))
            Me.columnsToOffset = columnsToOffset
        End Sub

        ''' <summary>Java <c>getOffsetAmount()</c> (Lombok).</summary>
        Public Function getOffsetAmount() As Integer
            Return offsetAmount
        End Function

        ''' <summary>Java <c>setOffsetAmount()</c> (Lombok).</summary>
        Public Sub setOffsetAmount(ByVal offsetAmount As Integer)
            Me.offsetAmount = offsetAmount
        End Sub

        ''' <summary>Java <c>getOperationType()</c> (Lombok).</summary>
        Public Function getOperationType() As OperationType
            Return operationTypeField
        End Function

        ''' <summary>Java <c>setOperationType()</c> (Lombok).</summary>
        Public Sub setOperationType(ByVal operationType As OperationType)
            Me.operationTypeField = operationType
        End Sub

        ''' <summary>Java <c>getEdgeHandling()</c> (Lombok).</summary>
        Public Function getEdgeHandling() As EdgeHandling
            Return edgeHandlingField
        End Function

        ''' <summary>Java <c>setEdgeHandling()</c> (Lombok).</summary>
        Public Sub setEdgeHandling(ByVal edgeHandling As EdgeHandling)
            Me.edgeHandlingField = edgeHandling
        End Sub

        ''' <summary>Java <c>getEdgeCaseValue()</c> (Lombok).</summary>
        Public Function getEdgeCaseValue() As WritableT
            Return edgeCaseValue
        End Function

        ''' <summary>Java <c>setEdgeCaseValue()</c> (Lombok).</summary>
        Public Sub setEdgeCaseValue(ByVal edgeCaseValue As WritableT)
            Me.edgeCaseValue = edgeCaseValue
        End Sub

        ''' <summary>Java <c>getColumnsToOffsetSet()</c> (Lombok).</summary>
        Public Function getColumnsToOffsetSet() As ISet(Of String)
            Return columnsToOffsetSet
        End Function

        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            For Each s As String In columnsToOffset
                If Not inputSchema.hasColumn(s) Then
                    Throw New InvalidOperationException("Column """ & s & """ is not found in input schema")
                End If
            Next s

            Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)()
            For Each m As ColumnMetaData In inputSchema.getColumnMetaData()
                If columnsToOffsetSet.Contains(m.Name) Then
                    If operationTypeField = OperationType.InPlace Then
                        'Only change is to the name
                        Dim mNew As ColumnMetaData = m.clone()
                        mNew.Name = getNewColumnName(m)
                        newMeta.Add(mNew)
                    Else
                        'Original is unmodified, new column is added
                        newMeta.Add(m)

                        Dim mNew As ColumnMetaData = m.clone()
                        mNew.Name = getNewColumnName(m)
                        newMeta.Add(mNew)
                    End If
                Else
                    'No change to this column
                    newMeta.Add(m)
                End If
            Next m

            Return inputSchema.newSchema(newMeta)
        End Function

        Private Function getNewColumnName(ByVal m As ColumnMetaData) As String
            Return "sequenceOffset(" & offsetAmount & "," & m.Name & ")"
        End Function

        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Set(ByVal value As SchemaT)
                Me.inputSchemaField = value
            End Set
            Get
                Return inputSchemaField
            End Get
        End Property

        Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
            Return outputColumnNames()(0)
        End Function

        Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
            Return New List(Of String)(inputSchemaField.getColumnNames()).ToArray()
        End Function

        Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
            Return outputColumnNames()
        End Function

        Public Overridable Function columnName() As String Implements ColumnOp.columnName
            Return outputColumnName()
        End Function

        Public Overridable Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT) Implements TransformT.map
            Throw New NotSupportedException("SequenceOffsetTransform cannot be applied to non-sequence data")
        End Function

        Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of WritableT)) Implements TransformT.mapSequence
            'Edge case
            If offsetAmount >= sequence.Count AndAlso edgeHandlingField = EdgeHandling.TrimSequence Then
                'No output
                Return New List(Of IList(Of WritableT))()
            End If

            Dim colNames As IList(Of String) = inputSchemaField.getColumnNames()
            Dim nIn As Integer = inputSchemaField.numColumns()
            Dim nOut As Integer = nIn + (If(operationTypeField = OperationType.InPlace, 0, columnsToOffset.Count))

            'Depending on settings, the original sequence might be smaller than the input
            Dim firstOutputStepInclusive As Integer
            Dim lastOutputStepInclusive As Integer
            If edgeHandlingField = EdgeHandling.TrimSequence Then
                If offsetAmount >= 0 Then
                    'Values in the specified columns are shifted later -> trim the start of the sequence
                    firstOutputStepInclusive = offsetAmount
                    lastOutputStepInclusive = sequence.Count - 1
                Else
                    'Values in the specified columns are shifted earlier -> trim the end of the sequence
                    firstOutputStepInclusive = 0
                    lastOutputStepInclusive = sequence.Count - 1 + offsetAmount
                End If
            Else
                'Specified value -> same output size
                firstOutputStepInclusive = 0
                lastOutputStepInclusive = sequence.Count - 1
            End If

            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            For [step] As Integer = firstOutputStepInclusive To lastOutputStepInclusive
                Dim thisStepIn As IList(Of WritableT) = sequence([step]) 'Input for the *non-shifted* values
                Dim thisStepOut As IList(Of WritableT) = New List(Of WritableT)(nOut)

                For j As Integer = 0 To nIn - 1
                    If columnsToOffsetSet.Contains(colNames(j)) Then
                        If (edgeHandlingField = EdgeHandling.SpecifiedValue AndAlso [step] - offsetAmount < 0) OrElse [step] - offsetAmount >= sequence.Count Then
                            If operationTypeField = OperationType.NewColumn Then
                                'Keep the original value
                                thisStepOut.Add(thisStepIn(j))
                            End If
                            thisStepOut.Add(edgeCaseValue)
                        Else
                            'Trim case, or specified but within range
                            Dim shifted As WritableT = sequence([step] - offsetAmount)(j)
                            If operationTypeField = OperationType.InPlace Then
                                'Shift by the specified amount and output
                                thisStepOut.Add(shifted)
                            Else
                                'Add the old value and the new (offset) value
                                thisStepOut.Add(thisStepIn(j))
                                thisStepOut.Add(shifted)
                            End If
                        End If
                    Else
                        'Value is unmodified in this column
                        thisStepOut.Add(thisStepIn(j))
                    End If
                Next j

                [out].Add(thisStepOut)
            Next [step]

            Return [out]
        End Function

        Public Overridable Function map(ByVal input As Object) As Object Implements TransformT.map
            Throw New NotSupportedException("SequenceOffsetTransform cannot be applied to non-sequence data")
        End Function

        Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements TransformT.mapSequence
            Throw New NotSupportedException("Not yet implemented/supported")
        End Function

        ''' <summary>Lombok <c>toString()</c>.</summary>
        Public Overrides Function ToString() As String
            Return "SequenceOffsetTransform(columnsToOffset=" & javaToString(columnsToOffset) & ", offsetAmount=" & offsetAmount &
                ", operationType=" & operationTypeField.ToString() & ", edgeHandling=" & edgeHandlingField.ToString() &
                ", edgeCaseValue=" & javaToString(edgeCaseValue) & ", columnsToOffsetSet=" & javaToString(columnsToOffsetSet) &
                ", inputSchema=" & javaToString(inputSchemaField) & ")"
        End Function

        ''' <summary>Lombok equality (excluding columnsToOffsetSet and inputSchema).</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim o As SequenceOffsetTransform = DirectCast(obj, SequenceOffsetTransform)
            Return javaEquals(columnsToOffset, o.columnsToOffset) AndAlso offsetAmount = o.offsetAmount AndAlso
                operationTypeField = o.operationTypeField AndAlso edgeHandlingField = o.edgeHandlingField AndAlso
                Objects.equals(edgeCaseValue, o.edgeCaseValue)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(columnsToOffset, offsetAmount, operationTypeField, edgeHandlingField, edgeCaseValue)
        End Function

    End Class

End Namespace
