Imports System
Imports System.Collections.Generic
Imports System.Linq

Namespace JavaCompat

    ''' <summary>A cell of a <see cref="Table(Of R, C, V)"/>: row key, column key and value.</summary>
    Public Class Cell(Of R, C, V)
        Public ReadOnly Property RowKey As R
        Public ReadOnly Property ColumnKey As C
        Public ReadOnly Property Value As V

        Public Sub New(ByVal rowKey As R, ByVal columnKey As C, ByVal value As V)
            Me.RowKey = rowKey
            Me.ColumnKey = columnKey
            Me.Value = value
        End Sub

        Public Function getRowKey() As R
            Return RowKey
        End Function

        Public Function getColumnKey() As C
            Return ColumnKey
        End Function

        Public Function getValue() As V
            Return Value
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim other = TryCast(obj, Cell(Of R, C, V))
            If other Is Nothing Then Return False
            Return Object.Equals(RowKey, other.RowKey) AndAlso Object.Equals(ColumnKey, other.ColumnKey) AndAlso Object.Equals(Value, other.Value)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(RowKey, ColumnKey, Value)
        End Function

        Public Overrides Function ToString() As String
            Return "(" & Arrays.FormatElement(RowKey) & "," & Arrays.FormatElement(ColumnKey) & ")=" & Arrays.FormatElement(Value)
        End Function
    End Class

    ''' <summary>A two-dimensional map keyed by row and column, with the Guava <c>Table</c> API.</summary>
    Public Interface Table(Of R, C, V)
        Function contains(ByVal rowKey As R, ByVal columnKey As C) As Boolean
        Function containsRow(ByVal rowKey As R) As Boolean
        Function containsColumn(ByVal columnKey As C) As Boolean
        Function containsValue(ByVal value As V) As Boolean
        ''' <summary>The value at (row, column), or the default value if absent.</summary>
        Function [get](ByVal rowKey As R, ByVal columnKey As C) As V
        Function isEmpty() As Boolean
        Function size() As Integer
        Sub clear()
        ''' <summary>Stores the value and returns the previous one (default value if none).</summary>
        Function put(ByVal rowKey As R, ByVal columnKey As C, ByVal value As V) As V
        Sub putAll(ByVal table As Table(Of R, C, V))
        ''' <summary>Removes the value and returns it (default value if none).</summary>
        Function remove(ByVal rowKey As R, ByVal columnKey As C) As V
        ''' <summary>A snapshot of the row: column key to value.</summary>
        Function row(ByVal rowKey As R) As IDictionary(Of C, V)
        ''' <summary>A snapshot of the column: row key to value.</summary>
        Function column(ByVal columnKey As C) As IDictionary(Of R, V)
        Function cellSet() As ISet(Of Cell(Of R, C, V))
        Function rowKeySet() As ISet(Of R)
        Function columnKeySet() As ISet(Of C)
        Function values() As ICollection(Of V)
        Function rowMap() As IDictionary(Of R, IDictionary(Of C, V))
        Function columnMap() As IDictionary(Of C, IDictionary(Of R, V))
    End Interface

    ''' <summary>A <see cref="Table(Of R, C, V)"/> backed by nested insertion-ordered dictionaries (Guava <c>HashBasedTable</c>).</summary>
    Public Class HashBasedTable(Of R, C, V)
        Implements Table(Of R, C, V)

        Private ReadOnly rows As New LinkedHashMap(Of R, LinkedHashMap(Of C, V))()

        Public Shared Function create() As HashBasedTable(Of R, C, V)
            Return New HashBasedTable(Of R, C, V)()
        End Function

        Public Function contains(ByVal rowKey As R, ByVal columnKey As C) As Boolean Implements Table(Of R, C, V).contains
            Dim r = rows.get(rowKey)
            Return r IsNot Nothing AndAlso r.ContainsKey(columnKey)
        End Function

        Public Function containsRow(ByVal rowKey As R) As Boolean Implements Table(Of R, C, V).containsRow
            Return rows.ContainsKey(rowKey)
        End Function

        Public Function containsColumn(ByVal columnKey As C) As Boolean Implements Table(Of R, C, V).containsColumn
            Return rows.Values.Any(Function(r) r.ContainsKey(columnKey))
        End Function

        Public Function containsValue(ByVal value As V) As Boolean Implements Table(Of R, C, V).containsValue
            Return rows.Values.Any(Function(r) r.containsValue(value))
        End Function

        Public Function [get](ByVal rowKey As R, ByVal columnKey As C) As V Implements Table(Of R, C, V).get
            Dim r = rows.get(rowKey)
            If r Is Nothing Then Return Nothing
            Return r.get(columnKey)
        End Function

        Public Function isEmpty() As Boolean Implements Table(Of R, C, V).isEmpty
            Return size() = 0
        End Function

        Public Function size() As Integer Implements Table(Of R, C, V).size
            Return rows.Values.Sum(Function(r) r.Count)
        End Function

        Public Sub clear() Implements Table(Of R, C, V).clear
            rows.Clear()
        End Sub

        Public Function put(ByVal rowKey As R, ByVal columnKey As C, ByVal value As V) As V Implements Table(Of R, C, V).put
            Dim r = rows.get(rowKey)
            If r Is Nothing Then
                r = New LinkedHashMap(Of C, V)()
                rows.put(rowKey, r)
            End If
            Return r.put(columnKey, value)
        End Function

        Public Sub putAll(ByVal table As Table(Of R, C, V)) Implements Table(Of R, C, V).putAll
            If table Is Nothing Then Throw New ArgumentNullException(NameOf(table))
            For Each cell In table.cellSet()
                put(cell.RowKey, cell.ColumnKey, cell.Value)
            Next cell
        End Sub

        Public Function remove(ByVal rowKey As R, ByVal columnKey As C) As V Implements Table(Of R, C, V).remove
            Dim r = rows.get(rowKey)
            If r Is Nothing Then Return Nothing
            Dim previous As V = r.removeAndGet(columnKey)
            If r.Count = 0 Then rows.Remove(rowKey)
            Return previous
        End Function

        Public Function row(ByVal rowKey As R) As IDictionary(Of C, V) Implements Table(Of R, C, V).row
            Dim r = rows.get(rowKey)
            Return If(r Is Nothing, New LinkedHashMap(Of C, V)(), New LinkedHashMap(Of C, V)(r))
        End Function

        Public Function column(ByVal columnKey As C) As IDictionary(Of R, V) Implements Table(Of R, C, V).column
            Dim result As New LinkedHashMap(Of R, V)()
            For Each kv In rows
                Dim v As V
                If kv.Value.TryGetValue(columnKey, v) Then result.put(kv.Key, v)
            Next kv
            Return result
        End Function

        Public Function cellSet() As ISet(Of Cell(Of R, C, V)) Implements Table(Of R, C, V).cellSet
            Dim result As New LinkedHashSet(Of Cell(Of R, C, V))()
            For Each kv In rows
                For Each inner In kv.Value
                    result.Add(New Cell(Of R, C, V)(kv.Key, inner.Key, inner.Value))
                Next inner
            Next kv
            Return result
        End Function

        Public Function rowKeySet() As ISet(Of R) Implements Table(Of R, C, V).rowKeySet
            Return rows.keySet()
        End Function

        Public Function columnKeySet() As ISet(Of C) Implements Table(Of R, C, V).columnKeySet
            Dim result As New LinkedHashSet(Of C)()
            For Each r In rows.Values
                For Each c In r.Keys
                    result.Add(c)
                Next c
            Next r
            Return result
        End Function

        Public Function values() As ICollection(Of V) Implements Table(Of R, C, V).values
            Dim result As New List(Of V)()
            For Each r In rows.Values
                result.AddRange(r.Values)
            Next r
            Return result
        End Function

        Public Function rowMap() As IDictionary(Of R, IDictionary(Of C, V)) Implements Table(Of R, C, V).rowMap
            Dim result As New LinkedHashMap(Of R, IDictionary(Of C, V))()
            For Each kv In rows
                result.put(kv.Key, New LinkedHashMap(Of C, V)(kv.Value))
            Next kv
            Return result
        End Function

        Public Function columnMap() As IDictionary(Of C, IDictionary(Of R, V)) Implements Table(Of R, C, V).columnMap
            Dim result As New LinkedHashMap(Of C, IDictionary(Of R, V))()
            For Each c In columnKeySet()
                result.put(c, column(c))
            Next c
            Return result
        End Function

        Public Overrides Function ToString() As String
            Return "{" & String.Join(", ", cellSet().Select(Function(c) c.ToString())) & "}"
        End Function
    End Class

End Namespace
