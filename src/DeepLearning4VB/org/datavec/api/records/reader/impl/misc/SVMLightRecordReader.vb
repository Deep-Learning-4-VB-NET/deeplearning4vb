Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports System.Text.RegularExpressions
Imports DeepLearning4VB.JavaCompat
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports RecordImpl = DeepLearning4VB.org.datavec.api.records.impl.Record
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataLine = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataLine
Imports LineRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.LineRecordReader
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable

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

Namespace org.datavec.api.records.reader.impl.misc

    ''' <summary>
    ''' Reads SVMLight/LibSVM formatted text: <c>label[,label...] [qid:n] index:value index:value ... [# comment]</c>.
    ''' Each record is <c>numFeatures</c> doubles (zero where no value is given), optionally followed by the
    ''' labels. The configuration keys are the Java class name based ones. Java's
    ''' <c>NumberFormatException</c> is <see cref="NumberFormatException"/>, <c>IndexOutOfBoundsException</c> is
    ''' <see cref="IndexOutOfRangeException"/>, <c>UnsupportedOperationException</c> is
    ''' <see cref="NotSupportedException"/> and <c>NoSuchElementException</c> is <see cref="InvalidOperationException"/>.
    ''' </summary>
    <Serializable>
    Public Class SVMLightRecordReader
        Inherits LineRecordReader

        ' Configuration options.
        ''' <summary>Java: <c>SVMLightRecordReader.class.getName()</c>; kept as the Java name so configuration keys match.</summary>
        Public Shadows Const NAME_SPACE As String = "org.datavec.api.records.reader.impl.misc.SVMLightRecordReader"
        Public Const NUM_FEATURES As String = NAME_SPACE & ".numfeatures"
        Public Const ZERO_BASED_INDEXING As String = NAME_SPACE & ".zeroBasedIndexing"
        Public Const ZERO_BASED_LABEL_INDEXING As String = NAME_SPACE & ".zeroBasedLabelIndexing"
        Public Const MULTILABEL As String = NAME_SPACE & ".multilabel"
        Public Const NUM_LABELS As String = NAME_SPACE & ".numLabels"

        ' Constants.
        Public Const COMMENT_CHAR As String = "#"
        Public Const ALLOWED_DELIMITERS As String = "[ " & vbTab & "]"
        Public Const PREFERRED_DELIMITER As String = " "
        Public Const FEATURE_DELIMITER As String = ":"
        Public Const LABEL_DELIMITER As String = ","
        Public Const QID_PREFIX As String = "qid"

        ' For convenience
        Public Shared ReadOnly ZERO As writable.Writable = New DoubleWritable(0)
        Public Shared ReadOnly ONE As writable.Writable = New DoubleWritable(1)
        Public Shared ReadOnly LABEL_ZERO As writable.Writable = New IntWritable(0)
        Public Shared ReadOnly LABEL_ONE As writable.Writable = New IntWritable(1)

        Protected Friend numFeatures As Integer = -1 ' number of features
        Protected Friend zeroBasedIndexing As Boolean = True ' whether to use zero-based indexing, true is safest
        '                                                 * but adds extraneous column if data is not zero indexed
        Protected Friend zeroBasedLabelIndexing As Boolean = False ' whether to use zero-based label indexing (NONSTANDARD!)
        Protected Friend appendLabel As Boolean = True ' whether to append labels to output
        Protected Friend multilabel As Boolean = False ' whether targets are multilabel
        Protected Friend numLabels As Integer = -1 ' number of labels (required for multilabel targets)
        Protected Friend recordLookahead As writable.Writable = Nothing

        ' for backwards compatibility
        Public Const NUM_ATTRIBUTES As String = NAME_SPACE & ".numattributes"

        Public Sub New()
        End Sub

        ''' <summary>
        ''' Must be called before attempting to read records.
        ''' </summary>
        ''' <param name="conf">          DataVec configuration </param>
        ''' <param name="split">         FileSplit </param>
        Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
            MyBase.initialize(conf, split)
            Me.Conf = conf
        End Sub

        ''' <summary>
        ''' Set configuration (Java <c>setConf</c>; setting the property reads the options).
        ''' </summary>
        Public Overrides Property Conf As Configuration
            Set(ByVal value As Configuration)
                MyBase.Conf = value
                numFeatures = value.getInt(NUM_FEATURES, -1)
                If numFeatures < 0 Then
                    numFeatures = value.getInt(NUM_ATTRIBUTES, -1)
                End If
                If numFeatures < 0 Then
                    Throw New NotSupportedException("numFeatures must be set in configuration")
                End If
                appendLabel = value.getBoolean(APPEND_LABEL, True)
                multilabel = value.getBoolean(MULTILABEL, False)
                zeroBasedIndexing = value.getBoolean(ZERO_BASED_INDEXING, True)
                zeroBasedLabelIndexing = value.getBoolean(ZERO_BASED_LABEL_INDEXING, False)
                numLabels = value.getInt(NUM_LABELS, -1)
                If multilabel AndAlso numLabels < 0 Then
                    Throw New NotSupportedException("numLabels must be set in confirmation for multilabel problems")
                End If
            End Set
            Get
                Return MyBase.Conf
            End Get
        End Property

        ''' <summary>
        ''' Helper function to help detect lines that are
        ''' commented out. May read ahead and cache a line.
        ''' </summary>
        Protected Friend Overridable Function getNextRecord() As writable.Writable
            Dim w As writable.Writable = Nothing
            If recordLookahead IsNot Nothing Then
                w = recordLookahead
                recordLookahead = Nothing
            End If
            Do While w Is Nothing AndAlso MyBase.hasNext()
                w = MyBase.next()(0)
                If Not w.ToString().StartsWith(COMMENT_CHAR, StringComparison.Ordinal) Then
                    Exit Do
                End If
                w = Nothing
            Loop
            Return w
        End Function

        Public Overrides Function hasNext() As Boolean
            recordLookahead = getNextRecord()
            Return (recordLookahead IsNot Nothing)
        End Function

        ''' <summary>
        ''' Return next record as list of Writables.
        ''' </summary>
        Public Overrides Function [next]() As IList(Of writable.Writable)
            If numFeatures < 0 AndAlso numLabels < 0 Then
                Throw New InvalidOperationException("Cannot get record: setConf(Configuration) has not been called. A setConf " & "call is rquired to specify the number of features and/or labels in the source dataset")
            End If
            Dim w As writable.Writable = getNextRecord()
            If w Is Nothing Then
                Throw New InvalidOperationException("No next element found!")
            End If
            Dim line As String = w.ToString()
            Dim record As IList(Of writable.Writable) = New List(Of writable.Writable)(Enumerable.Repeat(ZERO, numFeatures))

            ' Remove trailing comments
            Dim commentRegex As String = ALLOWED_DELIMITERS & "*" & COMMENT_CHAR & ".*$"
            Dim tokens() As String = StringHelper.Split(New Regex(commentRegex).Replace(line, "", 1), ALLOWED_DELIMITERS, True)

            ' Iterate over feature tokens
            For i As Integer = 1 To tokens.Length - 1
                Dim token As String = tokens(i)
                ' Split into feature index and value
                Dim featureTokens() As String = StringHelper.Split(token, FEATURE_DELIMITER, True)
                If featureTokens(0).StartsWith(QID_PREFIX, StringComparison.Ordinal) Then
                    ' Ignore QID entry for now
                Else
                    ' Parse feature index -- enforce that it's a positive integer
                    Dim index As Integer = -1
                    Try
                        index = parseInt(featureTokens(0))
                        If index < 0 Then
                            Throw New NumberFormatException("")
                        End If
                    Catch e As NumberFormatException
                        ' (Java used featureTokens[i] here, which is out of range for every token but the first two)
                        Dim msg As String = String.Format("Feature index must be positive integer (found {0})", featureTokens(0))
                        Throw New NumberFormatException(msg)
                    End Try
                    ' If not using zero-based indexing, shift all indeces to left by one
                    If Not zeroBasedIndexing Then
                        If index = 0 Then
                            Throw New IndexOutOfRangeException("Found feature with index " & index & " but not using zero-based indexing")
                        End If
                        index -= 1
                    End If
                    ' Check whether feature index exceeds number of features
                    If numFeatures >= 0 AndAlso index >= numFeatures Then
                        Throw New IndexOutOfRangeException("Found " & (index + 1) & " features in record, expected " & numFeatures)
                    End If
                    ' Add feature
                    record(index) = New DoubleWritable(parseDouble(featureTokens(1)))
                End If
            Next i

            ' If labels should be appended
            If appendLabel Then
                Dim labels As IList(Of writable.Writable) = New List(Of writable.Writable)()
                ' Treat labels as indeces for multilabel binary classification
                If multilabel Then
                    labels = New List(Of writable.Writable)(Enumerable.Repeat(LABEL_ZERO, numLabels))
                    If Not tokens(0).Equals("") Then
                        Dim labelTokens() As String = StringHelper.Split(tokens(0), LABEL_DELIMITER, True)
                        For i As Integer = 0 To labelTokens.Length - 1
                            ' Parse label index -- enforce that it's a positive integer
                            Dim index As Integer = -1
                            Try
                                index = parseInt(labelTokens(i))
                                If index < 0 Then
                                    Throw New NumberFormatException("")
                                End If
                            Catch e As NumberFormatException
                                Dim msg As String = String.Format("Multilabel index must be positive integer (found {0})", labelTokens(i))
                                Throw New NumberFormatException(msg)
                            End Try
                            ' If not using zero-based indexing for labels, shift all indeces to left by one
                            If Not zeroBasedLabelIndexing Then
                                If index = 0 Then
                                    Throw New IndexOutOfRangeException("Found label with index " & index & " but not using zero-based indexing")
                                End If
                                index -= 1
                            End If
                            ' Check whether label index exceeds number of labels
                            If numLabels >= 0 AndAlso index >= numLabels Then
                                Throw New IndexOutOfRangeException("Found " & (index + 1) & " labels in record, expected " & numLabels)
                            End If
                            ' Add label
                            labels(index) = LABEL_ONE
                        Next i
                    End If
                Else
                    Dim labelTokens() As String = StringHelper.Split(tokens(0), LABEL_DELIMITER, True)
                    Dim numLabelsFound As Integer = If(labelTokens(0).Equals(""), 0, labelTokens.Length)
                    If numLabels < 0 Then
                        numLabels = numLabelsFound
                    End If
                    If numLabelsFound <> numLabels Then
                        Throw New IndexOutOfRangeException("Found " & labelTokens.Length & " labels in record, expected " & numLabels)
                    End If
                    For i As Integer = 0 To numLabelsFound - 1
                        Dim intLabel As Integer
                        If Integer.TryParse(labelTokens(i), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, intLabel) Then
                            ' Encode label as integer, if possible
                            labels.Add(New IntWritable(intLabel))
                        Else
                            labels.Add(New DoubleWritable(parseDouble(labelTokens(i))))
                        End If
                    Next i
                End If
                ' Append labels to record
                For Each l As writable.Writable In labels
                    record.Add(l)
                Next l
            End If

            Return record
        End Function

        ''' <summary>
        ''' Return next Record.
        ''' </summary>
        Public Overrides Function nextRecord() As Record
            Dim [next] As IList(Of writable.Writable) = Me.next()
            Dim uri As Uri = (If(locations Is Nothing OrElse locations.Length < 1, Nothing, locations(splitIndex)))
            Dim meta As RecordMetaData = New RecordMetaDataLine(Me.lineIndex - 1, uri, GetType(SVMLightRecordReader)) '-1 as line number has been incremented already...
            Return New RecordImpl([next], meta)
        End Function

        Public Overrides Function record(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of writable.Writable)
            'Here: we are reading a single line from the DataInputStream. How to handle headers?
            Throw New NotSupportedException("Reading SVMLightRecordReader data from DataInputStream not yet implemented")
        End Function

        Public Overrides Sub reset()
            MyBase.reset()
            recordLookahead = Nothing
        End Sub

        Protected Friend Overrides Sub onLocationOpen(ByVal location As Uri)
            MyBase.onLocationOpen(location)
            recordLookahead = Nothing
        End Sub

        ''' <summary>Java <c>Integer.parseInt</c>: culture invariant, no whitespace, throws <see cref="NumberFormatException"/>.</summary>
        Private Shared Function parseInt(ByVal s As String) As Integer
            Dim i As Integer
            If s IsNot Nothing AndAlso Integer.TryParse(s, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, i) Then
                Return i
            End If
            Throw New NumberFormatException("For input string: """ & s & """")
        End Function

        ''' <summary>Java <c>Double.parseDouble</c>: culture invariant, throws <see cref="NumberFormatException"/>.</summary>
        Private Shared Function parseDouble(ByVal s As String) As Double
            Dim d As Double
            If s IsNot Nothing AndAlso Double.TryParse(s.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, d) Then
                Return d
            End If
            Throw New NumberFormatException("For input string: """ & s & """")
        End Function

    End Class

End Namespace
