Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Text
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.slf4j
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports SVMLightRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.misc.SVMLightRecordReader
Imports FileRecordWriter = DeepLearning4VB.org.datavec.api.records.writer.impl.FileRecordWriter
Imports PartitionMetaData = DeepLearning4VB.org.datavec.api.split.partition.PartitionMetaData
Imports ArrayWritable = DeepLearning4VB.org.datavec.api.writable.ArrayWritable

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

Namespace org.datavec.api.records.writer.impl.misc

    ''' <summary>
    ''' Writes records in SVMLight/LibSVM format: <c>label[,label...] index:value index:value ...</c>.
    ''' Numbers are formatted like Java's <c>Double.toString</c>, so files round-trip through
    ''' <see cref="SVMLightRecordReader"/>. The configuration keys are the Java class name based ones.
    ''' </summary>
    Public Class SVMLightRecordWriter
        Inherits FileRecordWriter

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(SVMLightRecordWriter))

        ' Configuration options.
        ''' <summary>Java: <c>SVMLightRecordWriter.class.getName()</c>; kept as the Java name so configuration keys match.</summary>
        Public Const NAME_SPACE As String = "org.datavec.api.records.writer.impl.misc.SVMLightRecordWriter"
        Public Const FEATURE_FIRST_COLUMN As String = NAME_SPACE & ".featureStartColumn"
        Public Const FEATURE_LAST_COLUMN As String = NAME_SPACE & ".featureEndColumn"
        Public Const ZERO_BASED_INDEXING As String = NAME_SPACE & ".zeroBasedIndexing"
        Public Const ZERO_BASED_LABEL_INDEXING As String = NAME_SPACE & ".zeroBasedLabelIndexing"
        Public Const HAS_LABELS As String = NAME_SPACE & ".hasLabel"
        Public Const MULTILABEL As String = NAME_SPACE & ".multilabel"
        Public Const LABEL_FIRST_COLUMN As String = NAME_SPACE & ".labelStartColumn"
        Public Const LABEL_LAST_COLUMN As String = NAME_SPACE & ".labelEndColumn"

        ' Constants.
        Public Const UNLABELED As String = ""

        Protected Friend featureFirstColumn As Integer = 0 ' First column with feature
        Protected Friend featureLastColumn As Integer = -1 ' Last column with feature
        Protected Friend zeroBasedIndexing As Boolean = False ' whether to use zero-based indexing, false is safest
        Protected Friend zeroBasedLabelIndexing As Boolean = False ' whether to use zero-based label indexing (NONSTANDARD!)
        Protected Friend hasLabel As Boolean = True ' Whether record has label
        Protected Friend multilabel As Boolean = False ' Whether labels are for multilabel binary classification
        Protected Friend labelFirstColumn As Integer = -1 ' First column with label
        Protected Friend labelLastColumn As Integer = -1 ' Last column with label

        Public Sub New()
        End Sub

        ''' <summary>
        ''' Set DataVec configuration (Java <c>setConf</c>; setting the property reads the options).
        ''' </summary>
        Public Overrides Property Conf As Configuration
            Set(ByVal value As Configuration)
                MyBase.Conf = value
                featureFirstColumn = value.getInt(FEATURE_FIRST_COLUMN, 0)
                hasLabel = value.getBoolean(HAS_LABELS, True)
                multilabel = value.getBoolean(MULTILABEL, False)
                labelFirstColumn = value.getInt(LABEL_FIRST_COLUMN, -1)
                labelLastColumn = value.getInt(LABEL_LAST_COLUMN, -1)
                featureLastColumn = value.getInt(FEATURE_LAST_COLUMN, If(labelFirstColumn > 0, labelFirstColumn - 1, -1))
                zeroBasedIndexing = value.getBoolean(ZERO_BASED_INDEXING, False)
                zeroBasedLabelIndexing = value.getBoolean(ZERO_BASED_LABEL_INDEXING, False)
            End Set
            Get
                Return MyBase.Conf
            End Get
        End Property

        Public Overrides Function supportsBatch() As Boolean
            Return False
        End Function

        ''' <summary>
        ''' Write next record.
        ''' </summary>
        ''' <param name="record"> </param>
        ''' <exception cref="System.IO.IOException"> </exception>
        Public Overrides Function write(ByVal record As IList(Of writable.Writable)) As PartitionMetaData
            If record.Count > 0 Then
                '             Infer label columns, if necessary. The default is
                '             * to assume that last column is a label and that the
                '             * first label column immediately follows the
                '             * last feature column.
                If hasLabel Then
                    If labelLastColumn < 0 Then
                        labelLastColumn = record.Count - 1
                    End If
                    If labelFirstColumn < 0 Then
                        If featureLastColumn > 0 Then
                            labelFirstColumn = featureLastColumn + 1
                        Else
                            labelFirstColumn = record.Count - 1
                        End If
                    End If
                End If

                '             Infer feature columns, if necessary. The default is
                '             * to assume that the first column is a feature and that
                '             * the last feature column immediately precedes the first
                '             * label column, if there are any.
                If featureLastColumn < 0 Then
                    If labelFirstColumn > 0 Then
                        featureLastColumn = labelFirstColumn - 1
                    Else
                        featureLastColumn = record.Count - 1
                    End If
                End If

                Dim result As New StringBuilder()
                ' Process labels
                If hasLabel Then
                    ' Track label indeces
                    Dim labelIndex As Integer = If(zeroBasedLabelIndexing, 0, 1)
                    For i As Integer = labelFirstColumn To labelLastColumn
                        Dim w As writable.Writable = record(i)
                        ' Handle array-structured Writables, which themselves have multiple columns
                        If TypeOf w Is ArrayWritable Then
                            Dim arr As ArrayWritable = DirectCast(w, ArrayWritable)
                            For j As Long = 0 To arr.length() - 1
                                Dim val As Double = arr.getDouble(j)
                                ' If multilabel, only store indeces of non-zero labels
                                If multilabel Then
                                    If val = 1.0 Then
                                        result.Append(SVMLightRecordReader.LABEL_DELIMITER & labelIndex)
                                    ElseIf val <> 0.0 AndAlso val <> -1.0 Then
                                        Throw New NumberFormatException("Expect value -1, 0, or 1 for multilabel targets (found " & Arrays.JavaDoubleToString(val) & ")")
                                    End If
                                Else ' Store value of standard label
                                    result.Append(SVMLightRecordReader.LABEL_DELIMITER & Arrays.JavaDoubleToString(val))
                                End If
                                labelIndex += 1 ' Increment label index for each entry in array
                            Next j
                        Else ' Handle scalar Writables
                            ' If multilabel, only store indeces of non-zero labels
                            If multilabel Then
                                Dim val As Double = parseDouble(w.ToString())
                                If val = 1.0 Then
                                    result.Append(SVMLightRecordReader.LABEL_DELIMITER & labelIndex)
                                ElseIf val <> 0.0 AndAlso val <> -1.0 Then
                                    Throw New NumberFormatException("Expect value -1, 0, or 1 for multilabel targets (found " & Arrays.JavaDoubleToString(val) & ")")
                                End If
                            Else ' Store value of standard label
                                Dim intVal As Integer
                                If Integer.TryParse(w.ToString(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, intVal) Then
                                    ' Encode label as integer, if possible
                                    result.Append(SVMLightRecordReader.LABEL_DELIMITER & intVal)
                                Else
                                    Dim val As Double = parseDouble(w.ToString())
                                    result.Append(SVMLightRecordReader.LABEL_DELIMITER & Arrays.JavaDoubleToString(val))
                                End If
                            End If
                            labelIndex += 1 ' Increment label index once per scalar Writable
                        End If
                    Next i
                End If
                If result.ToString().Equals("") Then ' Add "unlabeled" label if no labels found
                    result.Append(SVMLightRecordReader.LABEL_DELIMITER & UNLABELED)
                End If

                ' Track feature indeces
                Dim featureIndex As Integer = If(zeroBasedIndexing, 0, 1)
                For i As Integer = featureFirstColumn To featureLastColumn
                    Dim w As writable.Writable = record(i)
                    ' Handle array-structured Writables, which themselves have multiple columns
                    If TypeOf w Is ArrayWritable Then
                        Dim arr As ArrayWritable = DirectCast(w, ArrayWritable)
                        For j As Long = 0 To arr.length() - 1
                            Dim val As Double = arr.getDouble(j)
                            If val <> 0 Then
                                result.Append(SVMLightRecordReader.PREFERRED_DELIMITER & featureIndex)
                                result.Append(SVMLightRecordReader.FEATURE_DELIMITER & Arrays.JavaDoubleToString(val))
                            End If
                            featureIndex += 1 ' Increment feature index for each entry in array
                        Next j
                    Else
                        Dim val As Double = w.toDouble()
                        If val <> 0 Then
                            result.Append(SVMLightRecordReader.PREFERRED_DELIMITER & featureIndex)
                            result.Append(SVMLightRecordReader.FEATURE_DELIMITER & Arrays.JavaDoubleToString(val))
                        End If
                        featureIndex += 1 ' Increment feature index once per scalar Writable
                    End If
                Next i

                ' Remove extra label delimiter at beginning
                Dim line As String = result.ToString().Substring(1)
                [out].write(encoding.GetBytes(line))
                [out].write(encoding.GetBytes(NEW_LINE))
            End If

            Return PartitionMetaData.builder().numRecordsUpdated(1).build()
        End Function

        Public Overrides Function writeBatch(ByVal batch As IList(Of IList(Of writable.Writable))) As PartitionMetaData
            Throw New NotImplementedException("writeBatch is not supported on " & Me.GetType().Name)
        End Function

        ''' <summary>Java <c>Double.valueOf</c>: culture invariant, throws <see cref="NumberFormatException"/> on malformed input.</summary>
        Private Shared Function parseDouble(ByVal s As String) As Double
            Dim d As Double
            If Double.TryParse(If(s, "").Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, d) Then
                Return d
            End If
            Throw New NumberFormatException("For input string: """ & s & """")
        End Function

    End Class

End Namespace
