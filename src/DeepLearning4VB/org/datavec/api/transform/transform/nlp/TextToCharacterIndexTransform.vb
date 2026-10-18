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
Imports System.Linq
Imports System.Text.Json.Serialization
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports IntegerMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.IntegerMetaData
Imports BaseSequenceExpansionTransform = DeepLearning4VB.org.datavec.api.transform.sequence.expansion.BaseSequenceExpansionTransform
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.nlp

    ''' <summary>
    ''' Convert each text value in a sequence to a longer integer sequence of character indices: one time step per
    ''' character, holding the index of the character in the given map. The other columns are copied to every new
    ''' time step. Characters that are not in the map throw (<c>exceptionOnUnknown</c>) or are skipped.
    ''' <para>Equality (Lombok <c>@EqualsAndHashCode(callSuper = true)</c>) adds the character index map and the
    ''' <c>exceptionOnUnknown</c> flag to the base class fields.</para>
    ''' </summary>
    <Serializable>
    Public Class TextToCharacterIndexTransform
        Inherits BaseSequenceExpansionTransform

        Private characterIndexMap As IDictionary(Of Char, Integer)
        Private exceptionOnUnknown As Boolean
        <NonSerialized>
        Private writableMap As IDictionary(Of Char, IList(Of WritableT))

        ''' <param name="columnName">         Name of the text column </param>
        ''' <param name="newColumnName">      Name of the column after expansion </param>
        ''' <param name="characterIndexMap">  Character to integer index map </param>
        ''' <param name="exceptionOnUnknown"> If true: throw an exception on unknown characters. False: skip unknown characters. </param>
        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal newColumnName As String, ByVal characterIndexMap As IDictionary(Of Char, Integer), ByVal exceptionOnUnknown As Boolean)
            MyBase.New(JavaCollections.singletonList(columnName), JavaCollections.singletonList(newColumnName))
            Me.characterIndexMap = characterIndexMap
            Me.exceptionOnUnknown = exceptionOnUnknown
        End Sub

        Protected Friend Overrides Function expandedColumnMetaDatas(ByVal origColumnMeta As IList(Of ColumnMetaData), ByVal expandedColumnNames As IList(Of String)) As IList(Of ColumnMetaData)
            Return JavaCollections.singletonList(Of ColumnMetaData)(New IntegerMetaData(expandedColumnNames(0), 0, characterIndexMap.Count - 1))
        End Function

        ''' <exception cref="InvalidOperationException">for an unknown character, when <c>exceptionOnUnknown</c> is set</exception>
        Protected Friend Overrides Function expandTimeStep(ByVal currentStepValues As IList(Of WritableT)) As IList(Of IList(Of WritableT))
            If writableMap Is Nothing Then
                Dim m As IDictionary(Of Char, IList(Of WritableT)) = New Dictionary(Of Char, IList(Of WritableT))()
                For Each entry As KeyValuePair(Of Char, Integer) In characterIndexMap
                    m(entry.Key) = JavaCollections.singletonList(Of WritableT)(New IntWritable(entry.Value))
                Next entry
                writableMap = m
            End If
            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            Dim cArr() As Char = currentStepValues(0).ToString().ToCharArray()
            For Each c As Char In cArr
                Dim w As IList(Of WritableT) = Nothing
                If Not writableMap.TryGetValue(c, w) Then
                    If exceptionOnUnknown Then
                        Throw New InvalidOperationException("Unknown character found in text: """ & c & """")
                    End If
                    Continue For
                End If

                [out].Add(w)
            Next c

            Return [out]
        End Function

        ''' <summary>Java <c>getCharacterIndexMap()</c> (Lombok).</summary>
        Public Function getCharacterIndexMap() As IDictionary(Of Char, Integer)
            Return characterIndexMap
        End Function

        ''' <summary>Java <c>isExceptionOnUnknown()</c> (Lombok).</summary>
        Public Function isExceptionOnUnknown() As Boolean
            Return exceptionOnUnknown
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim other As TextToCharacterIndexTransform = DirectCast(obj, TextToCharacterIndexTransform)
            Return exceptionOnUnknown = other.exceptionOnUnknown AndAlso javaEquals(characterIndexMap, other.characterIndexMap)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), characterIndexMap, exceptionOnUnknown)
        End Function

        Public Overrides Function ToString() As String
            Return "TextToCharacterIndexTransform(characterIndexMap=" & javaToString(characterIndexMap) & ", exceptionOnUnknown=" & exceptionOnUnknown.ToString().ToLowerInvariant() & ", writableMap=" & javaToString(writableMap) & ")"
        End Function

    End Class

End Namespace
