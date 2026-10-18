Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports ConditionT = DeepLearning4VB.org.datavec.api.transform.condition.Condition
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.condition

    ''' <summary>
    ''' Replace the value in a specified column with a 'yes' value, if a condition is satisfied/true.<br/>
    ''' Replace the value of this same column with a 'no' value otherwise.
    ''' Note that the condition can be any generic condition, including on other column(s), different to the column
    ''' that will be modified.
    ''' <para>Equality (Lombok, excluding the column index) compares the class, the column, both values and the
    ''' condition.</para>
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"filterColIdx"}) @EqualsAndHashCode(exclude = {"filterColIdx"}) @Data
    <JsonIgnoreProperties("filterColIdx")>
    <Serializable>
    Public Class ConditionalReplaceValueTransformWithDefault
        Implements TransformT

        Protected Friend ReadOnly columnToReplace As String
        Protected Friend yesVal As WritableT
        Protected Friend noVal As WritableT
        Protected Friend filterColIdx As Integer
        Protected Friend ReadOnly condition As ConditionT

        ' Java names the JSON property of the condition "condiiton" (sic).
        <JsonConstructor>
        Public Sub New(ByVal columnToReplace As String, ByVal yesVal As WritableT, ByVal noVal As WritableT, <JsonProperty("condiiton")> ByVal condition As ConditionT)
            Me.columnToReplace = columnToReplace
            Me.yesVal = yesVal
            Me.noVal = noVal
            Me.condition = condition
        End Sub

        ''' <summary>Java <c>getColumnToReplace()</c> (Lombok).</summary>
        Public Function getColumnToReplace() As String
            Return columnToReplace
        End Function

        ''' <summary>Java <c>getYesVal()</c> (Lombok).</summary>
        Public Function getYesVal() As WritableT
            Return yesVal
        End Function

        ''' <summary>Java <c>setYesVal()</c> (Lombok).</summary>
        Public Sub setYesVal(ByVal yesVal As WritableT)
            Me.yesVal = yesVal
        End Sub

        ''' <summary>Java <c>getNoVal()</c> (Lombok).</summary>
        Public Function getNoVal() As WritableT
            Return noVal
        End Function

        ''' <summary>Java <c>setNoVal()</c> (Lombok).</summary>
        Public Sub setNoVal(ByVal noVal As WritableT)
            Me.noVal = noVal
        End Sub

        ''' <summary>Java <c>getFilterColIdx()</c> (Lombok).</summary>
        Public Function getFilterColIdx() As Integer
            Return filterColIdx
        End Function

        ''' <summary>Java <c>getCondition()</c> (Lombok).</summary>
        Public Function getCondition() As ConditionT
            Return condition
        End Function

        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            'Conditional replace should not change any of the metadata, under normal usage
            Return inputSchema
        End Function

        ''' <summary>The input schema is held by the condition.</summary>
        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Set(ByVal value As SchemaT)
                Me.filterColIdx = value.getColumnNames().IndexOf(columnToReplace)
                If Me.filterColIdx < 0 Then
                    Throw New InvalidOperationException("Column """ & columnToReplace & """ not found in input schema")
                End If
                condition.InputSchema = value
            End Set
            Get
                Return condition.InputSchema
            End Get
        End Property

        Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
            Return columnToReplace
        End Function

        Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
            Return columnNames()
        End Function

        Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
            Return New String() {columnToReplace}
        End Function

        Public Overridable Function columnName() As String Implements ColumnOp.columnName
            Return columnToReplace
        End Function

        Public Overrides Function ToString() As String
            Return "ConditionalReplaceValueTransformWithDefault(replaceColumn=""" & columnToReplace & """,yesValue=" & javaToString(yesVal) & """,noValue=" & javaToString(noVal) & ",condition=" & javaToString(condition) & ")"
        End Function

        Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of WritableT)) Implements TransformT.mapSequence
            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            For Each [step] As IList(Of WritableT) In sequence
                [out].Add(map([step]))
            Next [step]
            Return [out]
        End Function

        ''' <summary>The 'yes' value if the condition holds for the input, otherwise the 'no' value.</summary>
        Public Overridable Function map(ByVal input As Object) As Object Implements TransformT.map
            If condition.condition(input) Then
                Return yesVal
            Else
                Return noVal
            End If
        End Function

        Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements TransformT.mapSequence
            Dim seq As System.Collections.IList = DirectCast(sequence, System.Collections.IList)
            Dim [out] As IList(Of Object) = New List(Of Object)()
            For Each [step] As Object In seq
                [out].Add(map([step]))
            Next [step]
            Return [out]
        End Function

        Public Overridable Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT) Implements TransformT.map
            If condition.condition(writables) Then
                'Condition holds -> set yes value
                Dim newList As IList(Of WritableT) = New List(Of WritableT)(writables)
                newList(filterColIdx) = yesVal
                Return newList
            Else
                'Condition does not hold -> set no value
                Dim newList As IList(Of WritableT) = New List(Of WritableT)(writables)
                newList(filterColIdx) = noVal
                Return newList
            End If
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim o As ConditionalReplaceValueTransformWithDefault = DirectCast(obj, ConditionalReplaceValueTransformWithDefault)
            Return String.Equals(columnToReplace, o.columnToReplace) AndAlso Objects.equals(yesVal, o.yesVal) AndAlso
                Objects.equals(noVal, o.noVal) AndAlso Objects.equals(condition, o.condition)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(columnToReplace, yesVal, noVal, condition)
        End Function

    End Class

End Namespace
