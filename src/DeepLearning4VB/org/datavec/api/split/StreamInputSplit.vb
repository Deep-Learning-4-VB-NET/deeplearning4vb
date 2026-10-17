Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports DeepLearning4VB.org.datavec.api.util.files
Imports DeepLearning4VB.org.nd4vb.common.function
Imports DeepLearning4VB.org.nd4vb.common.util

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

Namespace org.datavec.api.split

    ''' <summary>
    ''' An input split over a list of URIs whose streams are opened by a user-supplied function
    ''' (Lombok <c>@Data</c> in Java: accessors, Equals, GetHashCode and ToString are written out).
    ''' </summary>
    Public Class StreamInputSplit
        Implements InputSplit

        Protected Friend uris As IList(Of Uri)
        Protected Friend streamCreatorFn As [Function](Of Uri, Stream)
        Protected Friend rng As Random
        Protected Friend order() As Integer

        ''' <summary>
        ''' Create a StreamInputSplit with no randomization
        ''' </summary>
        ''' <param name="uris">            The list of URIs to load </param>
        ''' <param name="streamCreatorFn"> The function to be used to create InputStream objects for a given URI. </param>
        Public Sub New(ByVal uris As IList(Of Uri), ByVal streamCreatorFn As [Function](Of Uri, Stream))
            Me.New(uris, streamCreatorFn, Nothing)
        End Sub

        ''' <summary>
        ''' Create a StreamInputSplit with optional randomization
        ''' </summary>
        ''' <param name="uris">            The list of URIs to load </param>
        ''' <param name="streamCreatorFn"> The function to be used to create InputStream objects for a given URI </param>
        ''' <param name="rng">             Random number generator instance. If non-null: streams will be iterated over in a random
        '''                        order. If null: no randomization (iteration order is according to the URIs list) </param>
        Public Sub New(ByVal uris As IList(Of Uri), ByVal streamCreatorFn As [Function](Of Uri, Stream), ByVal rng As Random)
            If uris Is Nothing Then Throw New ArgumentNullException(NameOf(uris), "uris is marked non-null but is null")
            If streamCreatorFn Is Nothing Then Throw New ArgumentNullException(NameOf(streamCreatorFn), "streamCreatorFn is marked non-null but is null")
            Me.uris = uris
            Me.streamCreatorFn = streamCreatorFn
            Me.rng = rng
        End Sub

        Public Overridable Function canWriteToLocation(ByVal location As Uri) As Boolean Implements InputSplit.canWriteToLocation
            Throw New NotSupportedException()
        End Function

        Public Overridable Function addNewLocation() As String Implements InputSplit.addNewLocation
            Throw New NotSupportedException()
        End Function

        Public Overridable Function addNewLocation(ByVal location As String) As String Implements InputSplit.addNewLocation
            Throw New NotSupportedException()
        End Function

        Public Overridable Sub updateSplitLocations(ByVal reset As Boolean) Implements InputSplit.updateSplitLocations
            Throw New NotSupportedException()
        End Sub

        Public Overridable Function needsBootstrapForWrite() As Boolean Implements InputSplit.needsBootstrapForWrite
            Throw New NotSupportedException()
        End Function

        Public Overridable Sub bootStrapForWrite() Implements InputSplit.bootStrapForWrite
            Throw New NotSupportedException()
        End Sub

        Public Overridable Function openOutputStreamFor(ByVal location As String) As Stream Implements InputSplit.openOutputStreamFor
            Throw New NotSupportedException()
        End Function

        Public Overridable Function openInputStreamFor(ByVal location As String) As Stream Implements InputSplit.openInputStreamFor
            Throw New NotSupportedException()
        End Function

        Public Overridable Function length() As Long Implements InputSplit.length
            Return uris.Count
        End Function

        Public Overridable Function locations() As Uri() Implements InputSplit.locations
            Return uris.ToArray()
        End Function

        Public Overridable Function locationsIterator() As IEnumerator(Of Uri) Implements InputSplit.locationsIterator
            If rng Is Nothing Then
                Return uris.GetEnumerator()
            Else
                If order Is Nothing Then
                    order = New Integer(uris.Count - 1) {}
                    For i As Integer = 0 To order.Length - 1
                        order(i) = i
                    Next i
                End If
                MathUtils.shuffleArray(order, rng)
                Return New ShuffledListIterator(Of Uri)(uris, order)
            End If
        End Function

        Public Overridable Function locationsPathIterator() As IEnumerator(Of String) Implements InputSplit.locationsPathIterator
            Throw New NotSupportedException()
        End Function

        Public Overridable Sub reset() Implements InputSplit.reset
            'No op
        End Sub

        Public Overridable Function resetSupported() As Boolean Implements InputSplit.resetSupported
            Return True
        End Function

        Public Function getUris() As IList(Of Uri)
            Return uris
        End Function

        Public Sub setUris(ByVal uris As IList(Of Uri))
            Me.uris = uris
        End Sub

        Public Function getStreamCreatorFn() As [Function](Of Uri, Stream)
            Return streamCreatorFn
        End Function

        Public Sub setStreamCreatorFn(ByVal streamCreatorFn As [Function](Of Uri, Stream))
            Me.streamCreatorFn = streamCreatorFn
        End Sub

        Public Function getRng() As Random
            Return rng
        End Function

        Public Sub setRng(ByVal rng As Random)
            Me.rng = rng
        End Sub

        Public Function getOrder() As Integer()
            Return order
        End Function

        Public Sub setOrder(ByVal order() As Integer)
            Me.order = order
        End Sub

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then Return True
            Dim other As StreamInputSplit = TryCast(o, StreamInputSplit)
            If other Is Nothing Then Return False
            If Not other.canEqual(Me) Then Return False
            Return listEquals(uris, other.uris) AndAlso Objects.equals(streamCreatorFn, other.streamCreatorFn) AndAlso
                Objects.equals(rng, other.rng) AndAlso Arrays.equals(order, other.order)
        End Function

        Private Shared Function listEquals(ByVal a As IList(Of Uri), ByVal b As IList(Of Uri)) As Boolean
            If a Is b Then Return True
            If a Is Nothing OrElse b Is Nothing Then Return False
            Return a.SequenceEqual(b)
        End Function

        Protected Overridable Function canEqual(ByVal other As Object) As Boolean
            Return TypeOf other Is StreamInputSplit
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            Dim urisHash As Integer = 43
            If uris IsNot Nothing Then
                urisHash = 1
                For Each u As Uri In uris
                    urisHash = 31 * urisHash + Objects.hashCode(u)
                Next u
            End If
            result = result * 59 + urisHash
            result = result * 59 + If(streamCreatorFn Is Nothing, 43, streamCreatorFn.GetHashCode())
            result = result * 59 + If(rng Is Nothing, 43, rng.GetHashCode())
            result = result * 59 + Arrays.hashCode(order)
            Return result
        End Function

        Public Overrides Function ToString() As String
            Dim urisText As String = If(uris Is Nothing, "null", "[" & String.Join(", ", uris.Select(Function(u) Objects.toString(u))) & "]")
            Return "StreamInputSplit(uris=" & urisText & ", streamCreatorFn=" & Objects.toString(streamCreatorFn) &
                ", rng=" & Objects.toString(rng) & ", order=" & Arrays.toString(order) & ")"
        End Function

    End Class

End Namespace
