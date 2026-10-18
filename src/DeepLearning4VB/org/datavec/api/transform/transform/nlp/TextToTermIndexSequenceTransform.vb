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

Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports IntegerMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.IntegerMetaData
Imports BaseSequenceExpansionTransform = DeepLearning4VB.org.datavec.api.transform.sequence.expansion.BaseSequenceExpansionTransform
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.nlp

    ''' <summary>
    ''' Convert each text value in a sequence to a longer integer sequence of term indices: the text is split with the
    ''' delimiter (a regular expression, as Java's <c>String.split</c>) and each time step holds the index of the term in
    ''' the vocabulary. The other columns are copied to every new time step. Unknown terms throw
    ''' (<c>exceptionOnUnknown</c>) or are skipped.
    ''' <para>Equality (Lombok <c>@EqualsAndHashCode(callSuper = true, exclude = {"writableMap"})</c>) adds the word index
    ''' map, the delimiter and the <c>exceptionOnUnknown</c> flag to the base class fields.</para>
    ''' <para>As in Java, the class's own <c>@JsonIgnoreProperties({"writableMap"})</c> replaces the one of the base
    ''' class, so the JSON includes the input schema.</para>
    ''' </summary>
    <Serializable>
    <JsonIgnoreProperties("writableMap")>
    <JsonIncludeNonNull>
    Public Class TextToTermIndexSequenceTransform
        Inherits BaseSequenceExpansionTransform

        Private wordIndexMap As IDictionary(Of String, Integer)
        Private delimiter As String
        Private exceptionOnUnknown As Boolean
        <NonSerialized>
        Private writableMap As IDictionary(Of String, IList(Of WritableT))

        ''' <param name="columnName">         Name of the text column </param>
        ''' <param name="newColumnName">      Name of the column after expansion </param>
        ''' <param name="vocabulary">         Vocabulary </param>
        ''' <param name="delimiter">          Delimiter </param>
        ''' <param name="exceptionOnUnknown"> If true: throw an exception on unknown characters. False: skip unknown characters. </param>
        Public Sub New(ByVal columnName As String, ByVal newColumnName As String, ByVal vocabulary As IList(Of String), ByVal delimiter As String, ByVal exceptionOnUnknown As Boolean)
            MyBase.New(JavaCollections.singletonList(columnName), JavaCollections.singletonList(newColumnName))
            Me.wordIndexMap = New Dictionary(Of String, Integer)()
            For i As Integer = 0 To vocabulary.Count - 1
                Me.wordIndexMap(vocabulary(i)) = i
            Next i
            Me.delimiter = delimiter
            Me.exceptionOnUnknown = exceptionOnUnknown
        End Sub

        ''' <param name="columnName">         Name of the text column </param>
        ''' <param name="newColumnName">      Name of the column after expansion </param>
        ''' <param name="wordIndexMap">       Map from terms in vocabulary to indeces </param>
        ''' <param name="delimiter">          Delimiter </param>
        ''' <param name="exceptionOnUnknown"> If true: throw an exception on unknown characters. False: skip unknown characters. </param>
        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal newColumnName As String, ByVal wordIndexMap As IDictionary(Of String, Integer), ByVal delimiter As String, ByVal exceptionOnUnknown As Boolean)
            MyBase.New(JavaCollections.singletonList(columnName), JavaCollections.singletonList(newColumnName))
            Me.wordIndexMap = wordIndexMap
            Me.delimiter = delimiter
            Me.exceptionOnUnknown = exceptionOnUnknown
        End Sub

        Protected Friend Overrides Function expandedColumnMetaDatas(ByVal origColumnMeta As IList(Of ColumnMetaData), ByVal expandedColumnNames As IList(Of String)) As IList(Of ColumnMetaData)
            Return JavaCollections.singletonList(Of ColumnMetaData)(New IntegerMetaData(expandedColumnNames(0), 0, wordIndexMap.Count - 1))
        End Function

        ''' <exception cref="InvalidOperationException">for an unknown term, when <c>exceptionOnUnknown</c> is set</exception>
        Protected Friend Overrides Function expandTimeStep(ByVal currentStepValues As IList(Of WritableT)) As IList(Of IList(Of WritableT))
            If writableMap Is Nothing Then
                Dim m As IDictionary(Of String, IList(Of WritableT)) = New Dictionary(Of String, IList(Of WritableT))()
                For Each entry As KeyValuePair(Of String, Integer) In wordIndexMap
                    m(entry.Key) = JavaCollections.singletonList(Of WritableT)(New IntWritable(entry.Value))
                Next entry
                writableMap = m
            End If
            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            Dim text As String = currentStepValues(0).ToString()
            Dim tokens() As String = javaSplit(text, Me.delimiter)
            For Each token As String In tokens
                Dim w As IList(Of WritableT) = Nothing
                If Not writableMap.TryGetValue(token, w) Then
                    If exceptionOnUnknown Then
                        Throw New InvalidOperationException("Unknown token found in text: """ & token & """")
                    End If
                    Continue For
                End If
                [out].Add(w)
            Next token

            Return [out]
        End Function

        ''' <summary>Java <c>getWordIndexMap()</c> (Lombok).</summary>
        Public Function getWordIndexMap() As IDictionary(Of String, Integer)
            Return wordIndexMap
        End Function

        ''' <summary>Java <c>getDelimiter()</c> (Lombok).</summary>
        Public Function getDelimiter() As String
            Return delimiter
        End Function

        ''' <summary>Java <c>isExceptionOnUnknown()</c> (Lombok).</summary>
        Public Function isExceptionOnUnknown() As Boolean
            Return exceptionOnUnknown
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim other As TextToTermIndexSequenceTransform = DirectCast(obj, TextToTermIndexSequenceTransform)
            Return exceptionOnUnknown = other.exceptionOnUnknown AndAlso String.Equals(delimiter, other.delimiter) AndAlso javaEquals(wordIndexMap, other.wordIndexMap)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), wordIndexMap, delimiter, exceptionOnUnknown)
        End Function

        Public Overrides Function ToString() As String
            Return "TextToTermIndexSequenceTransform(wordIndexMap=" & javaToString(wordIndexMap) & ", delimiter=" & javaToString(delimiter) & ", exceptionOnUnknown=" & exceptionOnUnknown.ToString().ToLowerInvariant() & ", writableMap=" & javaToString(writableMap) & ")"
        End Function

    End Class

End Namespace
