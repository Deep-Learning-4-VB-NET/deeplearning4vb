Imports System
Imports System.Collections
Imports System.Collections.Generic

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.common.collection

    ''' <summary>
    ''' An append-only list of strings that stores all characters in one large char array,
    ''' which greatly reduces per-string heap overhead for very large lists. Supported
    ''' operations: Add, Clear, indexed get, IndexOf/LastIndexOf/Contains and enumeration.
    ''' Insert, Remove and indexed set are not supported because of the packed storage.
    ''' </summary>
    Public Class CompactHeapStringList
        Implements IList(Of String)

        Public Const DEFAULT_REALLOCATION_BLOCK_SIZE_BYTES As Integer = 8 * 1024 * 1024 ' 8MB
        Public Const DEFAULT_INTEGER_REALLOCATION_BLOCK_SIZE_BYTES As Integer = 1024 * 1024 ' 1MB - 262144 ints, 131k entries

        Private ReadOnly reallocationBlockSizeBytes As Integer
        Private ReadOnly reallocationIntegerBlockSizeBytes As Integer
        Private usedCount As Integer = 0
        Private nextDataOffset As Integer = 0
        Private data() As Char
        Private offsetAndLength() As Integer

        Public Sub New()
            Me.New(DEFAULT_REALLOCATION_BLOCK_SIZE_BYTES, DEFAULT_INTEGER_REALLOCATION_BLOCK_SIZE_BYTES)
        End Sub

        ''' <param name="reallocationBlockSizeBytes">Growth step of the character storage, in bytes (2 bytes per char).</param>
        ''' <param name="intReallocationBlockSizeBytes">Growth step of the offset/length storage, in bytes (8 bytes per entry).</param>
        Public Sub New(ByVal reallocationBlockSizeBytes As Integer, ByVal intReallocationBlockSizeBytes As Integer)
            If reallocationBlockSizeBytes < 2 Then Throw New ArgumentOutOfRangeException(NameOf(reallocationBlockSizeBytes))
            If intReallocationBlockSizeBytes < 8 Then Throw New ArgumentOutOfRangeException(NameOf(intReallocationBlockSizeBytes))
            Me.reallocationBlockSizeBytes = reallocationBlockSizeBytes
            Me.reallocationIntegerBlockSizeBytes = intReallocationBlockSizeBytes
            data = New Char((reallocationBlockSizeBytes \ 2) - 1) {}
            offsetAndLength = New Integer((intReallocationBlockSizeBytes \ 4) - 1) {}
        End Sub

        Public ReadOnly Property Count As Integer Implements ICollection(Of String).Count
            Get
                Return usedCount
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of String).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Function size() As Integer
            Return usedCount
        End Function

        Public ReadOnly Property Empty As Boolean
            Get
                Return usedCount = 0
            End Get
        End Property

        Public Function isEmpty() As Boolean
            Return usedCount = 0
        End Function

        Default Public Property Item(ByVal index As Integer) As String Implements IList(Of String).Item
            Get
                Return [get](index)
            End Get
            Set(ByVal value As String)
                Throw New NotSupportedException("Set specified index: not supported due to serialized storage structure")
            End Set
        End Property

        Public Function [get](ByVal index As Integer) As String
            If index < 0 OrElse index >= usedCount Then
                Throw New ArgumentOutOfRangeException(NameOf(index), "Invalid index: " & index & ", size = " & usedCount)
            End If
            Dim offset As Integer = offsetAndLength(2 * index)
            Dim length As Integer = offsetAndLength(2 * index + 1)
            Return New String(data, offset, length)
        End Function

        ''' <summary>Appends the string. Always returns true (as in Java).</summary>
        Public Function add(ByVal s As String) As Boolean
            If s Is Nothing Then Throw New ArgumentNullException(NameOf(s))
            Dim length As Integer = s.Length

            If nextDataOffset + length > data.Length Then
                If nextDataOffset > Integer.MaxValue - length Then
                    Throw New NotSupportedException("Cannot allocate new data char[]: required array size exceeds Integer.MaxValue")
                End If
                Dim toAdd As Integer = Math.Max(reallocationBlockSizeBytes \ 2, length)
                Dim newLength As Integer = data.Length + Math.Min(toAdd, Integer.MaxValue - data.Length)
                data = Arrays.CopyOf(data, newLength)
            End If
            If 2 * (usedCount + 1) >= offsetAndLength.Length Then
                If offsetAndLength.Length >= Integer.MaxValue - 2 Then
                    Throw New NotSupportedException("Cannot allocate new offset int[]: required array size exceeds Integer.MaxValue")
                End If
                Dim newLength As Integer = offsetAndLength.Length + Math.Min(reallocationIntegerBlockSizeBytes \ 4, Integer.MaxValue - offsetAndLength.Length)
                offsetAndLength = Arrays.CopyOf(offsetAndLength, newLength)
            End If

            s.CopyTo(0, data, nextDataOffset, length)
            offsetAndLength(2 * usedCount) = nextDataOffset
            offsetAndLength(2 * usedCount + 1) = length
            nextDataOffset += length
            usedCount += 1
            Return True
        End Function

        Private Sub AddCollection(ByVal item As String) Implements ICollection(Of String).Add
            add(item)
        End Sub

        Public Function addAll(ByVal c As IEnumerable(Of String)) As Boolean
            Dim any As Boolean = False
            For Each s As String In c
                add(s)
                any = True
            Next s
            Return any
        End Function

        Public Sub Clear() Implements ICollection(Of String).Clear
            usedCount = 0
            nextDataOffset = 0
            data = New Char((reallocationBlockSizeBytes \ 2) - 1) {}
            offsetAndLength = New Integer((reallocationIntegerBlockSizeBytes \ 4) - 1) {}
        End Sub

        Public Function Contains(ByVal item As String) As Boolean Implements ICollection(Of String).Contains
            Return IndexOf(item) >= 0
        End Function

        Public Sub CopyTo(ByVal array() As String, ByVal arrayIndex As Integer) Implements ICollection(Of String).CopyTo
            For i As Integer = 0 To usedCount - 1
                array(arrayIndex + i) = [get](i)
            Next i
        End Sub

        Public Function toArray() As String()
            Dim str(usedCount - 1) As String
            CopyTo(str, 0)
            Return str
        End Function

        Private Function MatchesAt(ByVal i As Integer, ByVal ch() As Char) As Boolean
            If offsetAndLength(2 * i + 1) <> ch.Length Then
                Return False
            End If
            Dim offset As Integer = offsetAndLength(2 * i)
            For j As Integer = 0 To ch.Length - 1
                If data(offset + j) <> ch(j) Then
                    Return False
                End If
            Next j
            Return True
        End Function

        Public Function IndexOf(ByVal item As String) As Integer Implements IList(Of String).IndexOf
            If item Is Nothing Then Return -1
            Dim ch() As Char = item.ToCharArray()
            For i As Integer = 0 To usedCount - 1
                If MatchesAt(i, ch) Then Return i
            Next i
            Return -1
        End Function

        Public Function lastIndexOf(ByVal item As String) As Integer
            If item Is Nothing Then Return -1
            Dim ch() As Char = item.ToCharArray()
            For i As Integer = usedCount - 1 To 0 Step -1
                If MatchesAt(i, ch) Then Return i
            Next i
            Return -1
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As String) Implements IList(Of String).Insert
            Throw New NotSupportedException("Insert at specified index: not supported due to serialized storage structure")
        End Sub

        Public Function Remove(ByVal item As String) As Boolean Implements ICollection(Of String).Remove
            Throw New NotSupportedException("Remove not supported")
        End Function

        Public Sub RemoveAt(ByVal index As Integer) Implements IList(Of String).RemoveAt
            Throw New NotSupportedException("Remove not supported")
        End Sub

        Public Function subList(ByVal fromIndex As Integer, ByVal toIndex As Integer) As IList(Of String)
            If fromIndex < 0 OrElse toIndex > usedCount OrElse fromIndex > toIndex Then
                Throw New ArgumentOutOfRangeException("Invalid range: " & fromIndex & " to " & toIndex & ", size = " & usedCount)
            End If
            Dim ret As New List(Of String)(toIndex - fromIndex)
            For i As Integer = fromIndex To toIndex - 1
                ret.Add([get](i))
            Next i
            Return ret
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of String) Implements IEnumerable(Of String).GetEnumerator
            For i As Integer = 0 To usedCount - 1
                Yield [get](i)
            Next i
        End Function

        Private Function GetEnumeratorNonGeneric() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then Return True
            Dim other = TryCast(o, IList(Of String))
            If other Is Nothing OrElse other.Count <> usedCount Then Return False
            For i As Integer = 0 To usedCount - 1
                If Not String.Equals([get](i), other(i), StringComparison.Ordinal) Then Return False
            Next i
            Return True
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            For i As Integer = 0 To usedCount - 1
                h = Arrays.WrapToInt32(31L * h + [get](i).GetHashCode())
            Next i
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return Arrays.toString(toArray())
        End Function
    End Class

End Namespace
