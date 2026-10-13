Imports System
Imports System.Collections.Generic
Imports System.IO
Imports DeepLearning4VB.org.datavec.api.io.filters
Imports DeepLearning4VB.org.datavec.api.util.files

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
    ''' Base input split
    '''
    ''' @author Adam Gibson
    ''' </summary>
    Public MustInherit Class BaseInputSplit
        Implements InputSplit

        ''' <summary>URIs, as a String, via <see cref="URIUtil.toUriString"/> (which includes file:/ etc)</summary>
        Protected Friend uriStrings As IList(Of String)
        Protected Friend iterationOrder() As Integer
        ''' <summary>The split length (the field is called <c>length</c> in Java; VB does not allow a field and a method of the same name).</summary>
        Protected Friend splitLength As Long = 0

        Public MustOverride Function resetSupported() As Boolean Implements InputSplit.resetSupported

        Public MustOverride Sub reset() Implements InputSplit.reset

        Public MustOverride Function openInputStreamFor(ByVal location As String) As Stream Implements InputSplit.openInputStreamFor

        Public MustOverride Function openOutputStreamFor(ByVal location As String) As Stream Implements InputSplit.openOutputStreamFor

        Public MustOverride Sub bootStrapForWrite() Implements InputSplit.bootStrapForWrite

        Public MustOverride Function needsBootstrapForWrite() As Boolean Implements InputSplit.needsBootstrapForWrite

        Public MustOverride Sub updateSplitLocations(ByVal reset As Boolean) Implements InputSplit.updateSplitLocations

        Public Overridable Function canWriteToLocation(ByVal location As Uri) As Boolean Implements InputSplit.canWriteToLocation
            Return location.IsAbsoluteUri
        End Function

        Public Overridable Function addNewLocation() As String Implements InputSplit.addNewLocation
            Throw New NotSupportedException("Unable to add new location.")
        End Function

        Public Overridable Function addNewLocation(ByVal location As String) As String Implements InputSplit.addNewLocation
            Throw New NotSupportedException("Unable to add new location.")
        End Function

        Public Overridable Function locations() As Uri() Implements InputSplit.locations
            If uriStrings Is Nothing Then
                uriStrings = New List(Of String)()
            End If
            Dim uris(uriStrings.Count - 1) As Uri
            Dim i As Integer = 0
            For Each s As String In uriStrings
                uris(i) = URIUtil.createURI(s)
                i += 1
            Next s
            Return uris
        End Function

        Public Overridable Function locationsIterator() As IEnumerator(Of Uri) Implements InputSplit.locationsIterator
            Return New UriFromPathIterator(locationsPathIterator())
        End Function

        Public Overridable Function locationsPathIterator() As IEnumerator(Of String) Implements InputSplit.locationsPathIterator
            If iterationOrder Is Nothing Then
                Return uriStrings.GetEnumerator()
            End If
            Return New ShuffledListIterator(Of String)(uriStrings, iterationOrder)
        End Function

        Public Overridable Function length() As Long Implements InputSplit.length
            Return 0
        End Function

        ''' <summary>
        ''' Samples the locations based on the PathFilter and splits the result into
        ''' an array of InputSplit objects, with sizes proportional to the weights.
        ''' </summary>
        ''' <param name="pathFilter"> to modify the locations in some way (null == as is) </param>
        ''' <param name="weights">    to split the locations into multiple InputSplit </param>
        ''' <returns>           the sampled locations </returns>
        Public Overridable Function sample(ByVal pathFilter As PathFilter, ByVal ParamArray weights() As Double) As InputSplit()
            Dim paths() As Uri = If(pathFilter IsNot Nothing, pathFilter.filter(locations()), locations())
            If weights IsNot Nothing AndAlso weights.Length > 0 AndAlso weights(0) <> 1.0 Then
                Dim splits(weights.Length - 1) As InputSplit
                Dim totalWeight As Double = 0
                For i As Integer = 0 To weights.Length - 1
                    totalWeight += weights(i)
                Next i
                Dim cumulWeight As Double = 0
                Dim partitions(weights.Length) As Integer
                For i As Integer = 0 To weights.Length - 1
                    ' Java Math.round(double): floor(x + 0.5)
                    partitions(i) = CInt(Math.Floor(cumulWeight * paths.Length / totalWeight + 0.5))
                    cumulWeight += weights(i)
                Next i
                partitions(weights.Length) = paths.Length
                For i As Integer = 0 To weights.Length - 1
                    Dim uris As IList(Of Uri) = New List(Of Uri)()
                    Dim j As Integer = partitions(i)
                    Do While j < partitions(i + 1)
                        uris.Add(paths(j))
                        j += 1
                    Loop
                    splits(i) = New CollectionInputSplit(uris)
                Next i
                Return splits
            Else
                Return New InputSplit() {New CollectionInputSplit(paths)}
            End If
        End Function

    End Class

End Namespace
