Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Text

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

Namespace org.nd4vb.common.util

    ''' <summary>
    ''' A bidirectional index: assigns each distinct object a sequential integer and looks up in both directions.
    ''' </summary>
    <Serializable>
    Public Class Index

        Private Const serialVersionUID As Long = 1160629777026141078L
        Private ReadOnly objects As New ConcurrentDictionary(Of Integer, Object)()
        Private ReadOnly indexes As New ConcurrentDictionary(Of Object, Integer)()

        ''' <summary>Adds the object with an explicit index. Returns false if the object was already indexed.</summary>
        Public Overridable Function add(ByVal o As Object, ByVal idx As Integer) As Boolean
            SyncLock Me
                If TypeOf o Is String AndAlso o.ToString().Length = 0 Then
                    Throw New ArgumentException("Unable to add the empty string")
                End If
                Dim existing As Integer
                If indexes.TryGetValue(o, existing) Then
                    Return False
                End If
                objects(idx) = o
                indexes(o) = idx
                Return True
            End SyncLock
        End Function

        ''' <summary>Adds the object with the next sequential index. Returns false if the object was already indexed.</summary>
        Public Overridable Function add(ByVal o As Object) As Boolean
            SyncLock Me
                If TypeOf o Is String AndAlso o.ToString().Length = 0 Then
                    Throw New ArgumentException("Unable to add the empty string")
                End If
                Dim existing As Integer
                If indexes.TryGetValue(o, existing) Then
                    Return False
                End If
                Dim index As Integer = objects.Count
                objects(index) = o
                indexes(o) = index
                Return True
            End SyncLock
        End Function

        ''' <summary>Returns the index of the object, or -1 if it is not indexed.</summary>
        Public Overridable Function indexOf(ByVal o As Object) As Integer
            SyncLock Me
                Dim index As Integer
                If indexes.TryGetValue(o, index) Then
                    Return index
                End If
                Return -1
            End SyncLock
        End Function

        ''' <summary>Returns the object at the index, or Nothing if there is none.</summary>
        Public Overridable Function [get](ByVal i As Integer) As Object
            SyncLock Me
                Dim o As Object = Nothing
                objects.TryGetValue(i, o)
                Return o
            End SyncLock
        End Function

        Public Overridable Function size() As Integer
            Return objects.Count
        End Function

        Public Overrides Function ToString() As String
            SyncLock Me
                Dim buff As New StringBuilder("[")
                Dim sz As Integer = objects.Count
                For i As Integer = 0 To sz - 1
                    buff.Append(Arrays.FormatElement([get](i)))
                    If i < sz - 1 Then
                        buff.Append(" , ")
                    End If
                Next i
                buff.Append("]")
                Return buff.ToString()
            End SyncLock
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then
                Return True
            End If
            Dim other = TryCast(o, Index)
            If other Is Nothing OrElse other.objects.Count <> objects.Count Then
                Return False
            End If
            For Each kv As KeyValuePair(Of Integer, Object) In objects
                Dim v As Object = Nothing
                If Not other.objects.TryGetValue(kv.Key, v) OrElse Not Object.Equals(v, kv.Value) Then
                    Return False
                End If
            Next kv
            Return True
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 0
            For Each kv As KeyValuePair(Of Integer, Object) In objects
                h = h Xor (kv.Key Xor Global.DeepLearning4VB.Objects.hashCode(kv.Value))
            Next kv
            Return h
        End Function
    End Class

End Namespace
