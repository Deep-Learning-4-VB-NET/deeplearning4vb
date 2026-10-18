Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports DeepLearning4VB.org.nd4vb.common.function
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

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

Namespace org.nd4vb.linalg.util

    ''' <summary>
    ''' Test helpers (Java <c>ND4JTestUtils</c>): compare the binary array files (<c>Nd4vb.readBinary</c>) of two directories.
    ''' <para>
    ''' Differences from Java: files are path strings (<c>java.io.File</c>); the relative path of a file is the part after
    ''' the directory, with '/' separators (Java's <c>URI.relativize</c>); results are sorted by ordinal path comparison.
    ''' Fixed Java bug: files found only in the second directory are reported with their own path (Java looked them up in
    ''' the first directory and reported <c>null</c>).
    ''' </para>
    ''' </summary>
    Public Class ND4VBTestUtils

        Private Sub New()
        End Sub

        ''' <summary>The outcome of <see cref="validateSerializedArrays(String, String, Boolean)"/> (Lombok <c>@Data</c>).</summary>
        Public Class ComparisonResult

            Friend allResults As IList(Of Triple(Of String, String, Boolean))
            Friend passed As IList(Of Triple(Of String, String, Boolean))
            Friend failed As IList(Of Triple(Of String, String, Boolean))
            Friend skippedDir1 As IList(Of String)
            Friend skippedDir2 As IList(Of String)

            Public Sub New(ByVal allResults As IList(Of Triple(Of String, String, Boolean)), ByVal passed As IList(Of Triple(Of String, String, Boolean)), ByVal failed As IList(Of Triple(Of String, String, Boolean)), ByVal skippedDir1 As IList(Of String), ByVal skippedDir2 As IList(Of String))
                Me.allResults = allResults
                Me.passed = passed
                Me.failed = failed
                Me.skippedDir1 = skippedDir1
                Me.skippedDir2 = skippedDir2
            End Sub

            ''' <summary>Every compared pair (file 1, file 2, equal).</summary>
            Public Overridable Function getAllResults() As IList(Of Triple(Of String, String, Boolean))
                Return allResults
            End Function

            ''' <summary>The pairs that compared equal.</summary>
            Public Overridable Function getPassed() As IList(Of Triple(Of String, String, Boolean))
                Return passed
            End Function

            ''' <summary>The pairs that compared different.</summary>
            Public Overridable Function getFailed() As IList(Of Triple(Of String, String, Boolean))
                Return failed
            End Function

            ''' <summary>The files of directory 1 with no counterpart in directory 2.</summary>
            Public Overridable Function getSkippedDir1() As IList(Of String)
                Return skippedDir1
            End Function

            ''' <summary>The files of directory 2 with no counterpart in directory 1.</summary>
            Public Overridable Function getSkippedDir2() As IList(Of String)
                Return skippedDir2
            End Function

        End Class

        ''' <summary>Compares two arrays with <c>Equals</c>.</summary>
        Public Class EqualsFn
            Implements BiFunction(Of INDArray, INDArray, Boolean)

            Public Overridable Function apply(ByVal i1 As INDArray, ByVal i2 As INDArray) As Boolean Implements BiFunction(Of INDArray, INDArray, Boolean).apply
                Return i1.Equals(i2)
            End Function
        End Class

        ''' <summary>Compares two arrays with <c>equalsWithEps</c>.</summary>
        Public Class EqualsWithEpsFn
            Implements BiFunction(Of INDArray, INDArray, Boolean)

            Friend ReadOnly eps As Double

            ''' <param name="eps"> the tolerance </param>
            Public Sub New(ByVal eps As Double)
                Me.eps = eps
            End Sub

            Public Overridable Function apply(ByVal i1 As INDArray, ByVal i2 As INDArray) As Boolean Implements BiFunction(Of INDArray, INDArray, Boolean).apply
                Return i1.equalsWithEps(i2, eps)
            End Function
        End Class

        ''' <summary>
        ''' Scan the specified directories for matching files (i.e., same path relative to their respective root directories)
        ''' and compare the contents using <c>INDArray.Equals(Object)</c>
        ''' </summary>
        Public Shared Function validateSerializedArrays(ByVal dir1 As String, ByVal dir2 As String, ByVal recursive As Boolean) As ComparisonResult
            Return validateSerializedArrays(dir1, dir2, recursive, New EqualsFn())
        End Function

        ''' <summary>
        ''' Scan the specified directories for matching files (i.e., same path relative to their respective root directories)
        ''' and compare the contents using a provided function.<br/>
        ''' Assumes the saved files represent INDArrays saved with <c>Nd4vb.saveBinary</c>.
        ''' </summary>
        Public Shared Function validateSerializedArrays(ByVal dir1 As String, ByVal dir2 As String, ByVal recursive As Boolean, ByVal evalFn As BiFunction(Of INDArray, INDArray, Boolean)) As ComparisonResult
            Dim opt As SearchOption = If(recursive, SearchOption.AllDirectories, SearchOption.TopDirectoryOnly)
            Dim f1() As String = If(Directory.Exists(dir1), Directory.GetFiles(dir1, "*", opt), New String() {})
            Dim f2() As String = If(Directory.Exists(dir2), Directory.GetFiles(dir2, "*", opt), New String() {})
            Preconditions.checkState(f1.Length > 0, "No files found for directory 1: %s", Path.GetFullPath(dir1))
            Preconditions.checkState(f2.Length > 0, "No files found for directory 2: %s", Path.GetFullPath(dir2))

            Dim relativized1 As IDictionary(Of String, String) = relativize(dir1, f1)
            Dim relativized2 As IDictionary(Of String, String) = relativize(dir2, f2)

            Dim skipped1 As New List(Of String)()
            For Each s As String In relativized1.Keys
                If Not relativized2.ContainsKey(s) Then
                    skipped1.Add(relativized1(s))
                End If
            Next s

            Dim skipped2 As New List(Of String)()
            For Each s As String In relativized2.Keys
                If Not relativized1.ContainsKey(s) Then
                    skipped2.Add(relativized2(s))
                End If
            Next s

            Dim allResults As New List(Of Triple(Of String, String, Boolean))()
            Dim passed As New List(Of Triple(Of String, String, Boolean))()
            Dim failed As New List(Of Triple(Of String, String, Boolean))()
            For Each e As KeyValuePair(Of String, String) In relativized1
                Dim file1 As String = e.Value
                Dim file2 As String = Nothing
                If Not relativized2.TryGetValue(e.Key, file2) Then
                    Continue For
                End If

                Dim i1 As INDArray = Nd4vbT.readBinary(file1)
                Dim i2 As INDArray = Nd4vbT.readBinary(file2)
                Dim b As Boolean = evalFn.apply(i1, i2)
                Dim t As New Triple(Of String, String, Boolean)(file1, file2, b)
                allResults.Add(t)
                If b Then
                    passed.Add(t)
                Else
                    failed.Add(t)
                End If
            Next e

            Dim byFirst As Comparison(Of Triple(Of String, String, Boolean)) = Function(o1, o2) String.CompareOrdinal(o1.getFirst(), o2.getFirst())
            allResults.Sort(byFirst)
            passed.Sort(byFirst)
            failed.Sort(byFirst)
            skipped1.Sort(StringComparer.Ordinal)
            skipped2.Sort(StringComparer.Ordinal)

            Return New ComparisonResult(allResults, passed, failed, skipped1, skipped2)
        End Function

        ''' <summary>Maps each file's path relative to <paramref name="dir"/> ('/' separators) to the file.</summary>
        Private Shared Function relativize(ByVal dir As String, ByVal files() As String) As IDictionary(Of String, String)
            Dim root As String = Path.GetFullPath(dir).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) & Path.DirectorySeparatorChar
            Dim result As New Dictionary(Of String, String)()
            For Each f As String In files
                Dim full As String = Path.GetFullPath(f)
                Dim relative As String = If(full.StartsWith(root, StringComparison.OrdinalIgnoreCase), full.Substring(root.Length), Path.GetFileName(full))
                result(relative.Replace(Path.DirectorySeparatorChar, "/"c)) = f
            Next f
            Return result
        End Function

    End Class

End Namespace
