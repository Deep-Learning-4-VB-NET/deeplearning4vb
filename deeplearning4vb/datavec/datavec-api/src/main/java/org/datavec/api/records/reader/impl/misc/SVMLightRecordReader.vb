Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Record = org.datavec.api.records.Record
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataLine = org.datavec.api.records.metadata.RecordMetaDataLine
Imports LineRecordReader = org.datavec.api.records.reader.impl.LineRecordReader
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports InputSplit = org.datavec.api.split.InputSplit
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports Writable = org.datavec.api.writable.Writable
Imports Configuration = org.datavec.api.conf.Configuration

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SVMLightRecordReader extends org.datavec.api.records.reader.impl.LineRecordReader
	<Serializable>
	Public Class SVMLightRecordReader
		Inherits LineRecordReader

		' Configuration options. 
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Public Shared ReadOnly NAME_SPACE As String = GetType(SVMLightRecordReader).FullName
		Public Shared ReadOnly NUM_FEATURES As String = NAME_SPACE & ".numfeatures"
		Public Shared ReadOnly ZERO_BASED_INDEXING As String = NAME_SPACE & ".zeroBasedIndexing"
		Public Shared ReadOnly ZERO_BASED_LABEL_INDEXING As String = NAME_SPACE & ".zeroBasedLabelIndexing"
		Public Shared ReadOnly MULTILABEL As String = NAME_SPACE & ".multilabel"
		Public Shared ReadOnly NUM_LABELS As String = NAME_SPACE & ".numLabels"

		' Constants. 
		Public Const COMMENT_CHAR As String = "#"
		Public Shared ReadOnly ALLOWED_DELIMITERS As String = "[ " & vbTab & "]"
		Public Const PREFERRED_DELIMITER As String = " "
		Public Const FEATURE_DELIMITER As String = ":"
		Public Const LABEL_DELIMITER As String = ","
		Public Const QID_PREFIX As String = "qid"

		' For convenience 
		Public Shared ReadOnly ZERO As Writable = New DoubleWritable(0)
		Public Shared ReadOnly ONE As Writable = New DoubleWritable(1)
		Public Shared ReadOnly LABEL_ZERO As Writable = New IntWritable(0)
		Public Shared ReadOnly LABEL_ONE As Writable = New IntWritable(1)

		Protected Friend numFeatures As Integer = -1 ' number of features
		Protected Friend zeroBasedIndexing As Boolean = True ' whether to use zero-based indexing, true is safest
	'                                                 * but adds extraneous column if data is not zero indexed
	'                                                 
		Protected Friend zeroBasedLabelIndexing As Boolean = False ' whether to use zero-based label indexing (NONSTANDARD!)
		Protected Friend appendLabel As Boolean = True ' whether to append labels to output
'JAVA TO VB CONVERTER NOTE: The field multilabel was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend multilabel_Conflict As Boolean = False ' whether targets are multilabel
		Protected Friend numLabels As Integer = -1 ' number of labels (required for multilabel targets)
		Protected Friend recordLookahead As Writable = Nothing

		' for backwards compatibility
		Public Shared ReadOnly NUM_ATTRIBUTES As String = NAME_SPACE & ".numattributes"

		Public Sub New()
		End Sub

		''' <summary>
		''' Must be called before attempting to read records.
		''' </summary>
		''' <param name="conf">          DataVec configuration </param>
		''' <param name="split">         FileSplit </param>
		''' <exception cref="IOException"> </exception>
		''' <exception cref="InterruptedException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.conf.Configuration conf, org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
			MyBase.initialize(conf, split)
			Me.Conf = conf
		End Sub

		''' <summary>
		''' Set configuration.
		''' </summary>
		''' <param name="conf">          DataVec configuration </param>
		''' <exception cref="IOException"> </exception>
		''' <exception cref="InterruptedException"> </exception>
		Public Overrides WriteOnly Property Conf As Configuration
			Set(ByVal conf As Configuration)
				MyBase.Conf = conf
				numFeatures = conf.getInt(NUM_FEATURES, -1)
				If numFeatures < 0 Then
					numFeatures = conf.getInt(NUM_ATTRIBUTES, -1)
				End If
				If numFeatures < 0 Then
					Throw New System.NotSupportedException("numFeatures must be set in configuration")
				End If
				appendLabel = conf.getBoolean(APPEND_LABEL, True)
				multilabel_Conflict = conf.getBoolean(MULTILABEL, False)
				zeroBasedIndexing = conf.getBoolean(ZERO_BASED_INDEXING, True)
				zeroBasedLabelIndexing = conf.getBoolean(ZERO_BASED_LABEL_INDEXING, False)
				numLabels = conf.getInt(NUM_LABELS, -1)
				If multilabel_Conflict AndAlso numLabels < 0 Then
					Throw New System.NotSupportedException("numLabels must be set in confirmation for multilabel problems")
				End If
			End Set
		End Property

		''' <summary>
		''' Helper function to help detect lines that are
		''' commented out. May read ahead and cache a line.
		''' 
		''' @return
		''' </summary>
		Protected Friend Overridable ReadOnly Property NextRecord As Writable
			Get
				Dim w As Writable = Nothing
				If recordLookahead IsNot Nothing Then
					w = recordLookahead
					recordLookahead = Nothing
				End If
				Do While w Is Nothing AndAlso MyBase.hasNext()
					w = MyBase.next().GetEnumerator().next()
					If Not w.ToString().StartsWith(COMMENT_CHAR, StringComparison.Ordinal) Then
						Exit Do
					End If
					w = Nothing
				Loop
				Return w
			End Get
		End Property

		Public Overrides Function hasNext() As Boolean
			recordLookahead = NextRecord
			Return (recordLookahead IsNot Nothing)
		End Function

		''' <summary>
		''' Return next record as list of Writables.
		''' 
		''' @return
		''' </summary>
		Public Overrides Function [next]() As IList(Of Writable)
			If numFeatures < 0 AndAlso numLabels < 0 Then
				Throw New System.InvalidOperationException("Cannot get record: setConf(Configuration) has not been called. A setConf " & "call is rquired to specify the number of features and/or labels in the source dataset")
			End If


			Dim w As Writable = NextRecord
			If w Is Nothing Then
				Throw New NoSuchElementException("No next element found!")
			End If
			Dim line As String = w.ToString()
			Dim record As IList(Of Writable) = New List(Of Writable)(Collections.nCopies(numFeatures, ZERO))

			' Remove trailing comments
			Dim commentRegex As String = ALLOWED_DELIMITERS & "*" & COMMENT_CHAR & ".*$"
			Dim tokens() As String = line.replaceFirst(commentRegex, "").Split(ALLOWED_DELIMITERS)

			' Iterate over feature tokens
			For i As Integer = 1 To tokens.Length - 1
				Dim token As String = tokens(i)
				' Split into feature index and value
				Dim featureTokens() As String = token.Split(FEATURE_DELIMITER, True)
				If featureTokens(0).StartsWith(QID_PREFIX, StringComparison.Ordinal) Then
					' Ignore QID entry for now
				Else
					' Parse feature index -- enforce that it's a positive integer
					Dim index As Integer = -1
					Try
						index = Integer.Parse(featureTokens(0))
						If index < 0 Then
							Throw New System.FormatException("")
						End If
					Catch e As System.FormatException
						Dim msg As String = String.Format("Feature index must be positive integer (found {0})", featureTokens(i).ToString())
						Throw New System.FormatException(msg)
					End Try

					' If not using zero-based indexing, shift all indeces to left by one
					If Not zeroBasedIndexing Then
						If index = 0 Then
							Throw New System.IndexOutOfRangeException("Found feature with index " & index & " but not using zero-based indexing")
						End If
						index -= 1
					End If

					' Check whether feature index exceeds number of features
					If numFeatures >= 0 AndAlso index >= numFeatures Then
						Throw New System.IndexOutOfRangeException("Found " & (index+1) & " features in record, expected " & numFeatures)
					End If

					' Add feature
					record(index) = New DoubleWritable(Double.Parse(featureTokens(1)))
				End If
			Next i

			' If labels should be appended
			If appendLabel Then
				Dim labels As IList(Of Writable) = New List(Of Writable)()

				' Treat labels as indeces for multilabel binary classification
				If multilabel_Conflict Then
					labels = New List(Of Writable)(Collections.nCopies(numLabels, LABEL_ZERO))
					If Not tokens(0).Equals("") Then
						Dim labelTokens() As String = tokens(0).Split(LABEL_DELIMITER, True)
						For i As Integer = 0 To labelTokens.Length - 1
							' Parse label index -- enforce that it's a positive integer
							Dim index As Integer = -1
							Try
								index = Integer.Parse(labelTokens(i))
								If index < 0 Then
									Throw New System.FormatException("")
								End If
							Catch e As System.FormatException
								Dim msg As String = String.Format("Multilabel index must be positive integer (found {0})", labelTokens(i).ToString())
								Throw New System.FormatException(msg)
							End Try

							' If not using zero-based indexing for labels, shift all indeces to left by one
							If Not zeroBasedLabelIndexing Then
								If index = 0 Then
									Throw New System.IndexOutOfRangeException("Found label with index " & index & " but not using zero-based indexing")
								End If
								index -= 1
							End If

							' Check whether label index exceeds number of labels
							If numLabels >= 0 AndAlso index >= numLabels Then
								Throw New System.IndexOutOfRangeException("Found " & (index + 1) & " labels in record, expected " & numLabels)
							End If

							' Add label
							labels(index) = LABEL_ONE
						Next i
					End If
				Else
					Dim labelTokens() As String = tokens(0).Split(LABEL_DELIMITER, True)
					Dim numLabelsFound As Integer = If(labelTokens(0).Equals(""), 0, labelTokens.Length)
					If numLabels < 0 Then
						numLabels = numLabelsFound
					End If
					If numLabelsFound <> numLabels Then
						Throw New System.IndexOutOfRangeException("Found " & labelTokens.Length & " labels in record, expected " & numLabels)
					End If
					For i As Integer = 0 To numLabelsFound - 1
						Try ' Encode label as integer, if possible
							labels.Add(New IntWritable(Integer.Parse(labelTokens(i))))
						Catch e As System.FormatException
							labels.Add(New DoubleWritable(Double.Parse(labelTokens(i))))
						End Try
					Next i
				End If

				' Append labels to record
				CType(record, List(Of Writable)).AddRange(labels)
			End If

			Return record
		End Function

		''' <summary>
		''' Return next Record.
		''' 
		''' @return
		''' </summary>
		Public Overrides Function nextRecord() As Record
			Dim [next] As IList(Of Writable) = Me.next()
			Dim uri As URI = (If(locations Is Nothing OrElse locations.Length < 1, Nothing, locations(splitIndex)))
			Dim meta As RecordMetaData = New RecordMetaDataLine(Me.lineIndex - 1, uri, GetType(SVMLightRecordReader)) '-1 as line number has been incremented already...
			Return New org.datavec.api.records.impl.Record([next], meta)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.writable.Writable> record(java.net.URI uri, java.io.DataInputStream dataInputStream) throws java.io.IOException
		Public Overrides Function record(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of Writable)
			'Here: we are reading a single line from the DataInputStream. How to handle headers?
			Throw New System.NotSupportedException("Reading SVMLightRecordReader data from DataInputStream not yet implemented")
		End Function

		Public Overrides Sub reset()
			MyBase.reset()
			recordLookahead = Nothing
		End Sub

		Protected Friend Overrides Sub onLocationOpen(ByVal location As URI)
			MyBase.onLocationOpen(location)
			recordLookahead = Nothing
		End Sub
	End Class

End Namespace