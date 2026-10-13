Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports NotImplementedException = org.apache.commons.lang3.NotImplementedException
Imports Configuration = org.datavec.api.conf.Configuration
Imports SVMLightRecordReader = org.datavec.api.records.reader.impl.misc.SVMLightRecordReader
Imports FileRecordWriter = org.datavec.api.records.writer.impl.FileRecordWriter
Imports PartitionMetaData = org.datavec.api.split.partition.PartitionMetaData
Imports ArrayWritable = org.datavec.api.writable.ArrayWritable
Imports Writable = org.datavec.api.writable.Writable

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SVMLightRecordWriter extends org.datavec.api.records.writer.impl.FileRecordWriter
	Public Class SVMLightRecordWriter
		Inherits FileRecordWriter

		' Configuration options. 
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Public Shared ReadOnly NAME_SPACE As String = GetType(SVMLightRecordWriter).FullName
		Public Shared ReadOnly FEATURE_FIRST_COLUMN As String = NAME_SPACE & ".featureStartColumn"
		Public Shared ReadOnly FEATURE_LAST_COLUMN As String = NAME_SPACE & ".featureEndColumn"
		Public Shared ReadOnly ZERO_BASED_INDEXING As String = NAME_SPACE & ".zeroBasedIndexing"
		Public Shared ReadOnly ZERO_BASED_LABEL_INDEXING As String = NAME_SPACE & ".zeroBasedLabelIndexing"
		Public Shared ReadOnly HAS_LABELS As String = NAME_SPACE & ".hasLabel"
		Public Shared ReadOnly MULTILABEL As String = NAME_SPACE & ".multilabel"
		Public Shared ReadOnly LABEL_FIRST_COLUMN As String = NAME_SPACE & ".labelStartColumn"
		Public Shared ReadOnly LABEL_LAST_COLUMN As String = NAME_SPACE & ".labelEndColumn"

		' Constants. 
		Public Const UNLABELED As String = ""

		Protected Friend featureFirstColumn As Integer = 0 ' First column with feature
		Protected Friend featureLastColumn As Integer = -1 ' Last column with feature
		Protected Friend zeroBasedIndexing As Boolean = False ' whether to use zero-based indexing, false is safest
		Protected Friend zeroBasedLabelIndexing As Boolean = False ' whether to use zero-based label indexing (NONSTANDARD!)
		Protected Friend hasLabel As Boolean = True ' Whether record has label
'JAVA TO VB CONVERTER NOTE: The field multilabel was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend multilabel_Conflict As Boolean = False ' Whether labels are for multilabel binary classification
		Protected Friend labelFirstColumn As Integer = -1 ' First column with label
		Protected Friend labelLastColumn As Integer = -1 ' Last column with label

		Public Sub New()
		End Sub



		''' <summary>
		''' Set DataVec configuration
		''' </summary>
		''' <param name="conf"> </param>
		Public Overrides WriteOnly Property Conf As Configuration
			Set(ByVal conf As Configuration)
				MyBase.Conf = conf
				featureFirstColumn = conf.getInt(FEATURE_FIRST_COLUMN, 0)
				hasLabel = conf.getBoolean(HAS_LABELS, True)
				multilabel_Conflict = conf.getBoolean(MULTILABEL, False)
				labelFirstColumn = conf.getInt(LABEL_FIRST_COLUMN, -1)
				labelLastColumn = conf.getInt(LABEL_LAST_COLUMN, -1)
				featureLastColumn = conf.getInt(FEATURE_LAST_COLUMN,If(labelFirstColumn > 0, labelFirstColumn-1, -1))
				zeroBasedIndexing = conf.getBoolean(ZERO_BASED_INDEXING, False)
				zeroBasedLabelIndexing = conf.getBoolean(ZERO_BASED_LABEL_INDEXING, False)
			End Set
		End Property

		Public Overrides Function supportsBatch() As Boolean
			Return False
		End Function

		''' <summary>
		''' Write next record.
		''' </summary>
		''' <param name="record"> </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.split.partition.PartitionMetaData write(java.util.List<org.datavec.api.writable.Writable> record) throws java.io.IOException
		Public Overridable Overloads Function write(ByVal record As IList(Of Writable)) As PartitionMetaData
			If record.Count > 0 Then
				Dim recordList As IList(Of Writable) = If(TypeOf record Is System.Collections.IList, CType(record, IList(Of Writable)), New List(Of Writable)(record))

	'             Infer label columns, if necessary. The default is
	'             * to assume that last column is a label and that the
	'             * first label column immediately follows the
	'             * last feature column.
	'             
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
	'             
				If featureLastColumn < 0 Then
					If labelFirstColumn > 0 Then
						featureLastColumn = labelFirstColumn - 1
					Else
						featureLastColumn = recordList.Count - 1
					End If
				End If

				Dim result As New StringBuilder()
				' Process labels
				If hasLabel Then
					' Track label indeces
					Dim labelIndex As Integer = If(zeroBasedLabelIndexing, 0, 1)
					For i As Integer = labelFirstColumn To labelLastColumn
						Dim w As Writable = record(i)
						' Handle array-structured Writables, which themselves have multiple columns
						If TypeOf w Is ArrayWritable Then
							Dim arr As ArrayWritable = DirectCast(w, ArrayWritable)
							For j As Integer = 0 To arr.length() - 1
								Dim val As Double = arr.getDouble(j)
								' If multilabel, only store indeces of non-zero labels
								If multilabel_Conflict Then
									If val = 1.0 Then
										result.Append(SVMLightRecordReader.LABEL_DELIMITER & labelIndex)
									ElseIf val <> 0.0 AndAlso val <> -1.0 Then
										Throw New System.FormatException("Expect value -1, 0, or 1 for multilabel targets (found " & val & ")")
									End If
								Else ' Store value of standard label
									result.Append(SVMLightRecordReader.LABEL_DELIMITER & val)
								End If
								labelIndex += 1 ' Increment label index for each entry in array
							Next j
						Else ' Handle scalar Writables
							' If multilabel, only store indeces of non-zero labels
							If multilabel_Conflict Then
								Dim val As Double = Convert.ToDouble(w.ToString())
								If val = 1.0 Then
									result.Append(SVMLightRecordReader.LABEL_DELIMITER & labelIndex)
								ElseIf val <> 0.0 AndAlso val <> -1.0 Then
									Throw New System.FormatException("Expect value -1, 0, or 1 for multilabel targets (found " & val & ")")
								End If
							Else ' Store value of standard label
								Try ' Encode label as integer, if possible
									Dim val As Integer = Convert.ToInt32(w.ToString())
									result.Append(SVMLightRecordReader.LABEL_DELIMITER & val)
								Catch e As Exception
									Dim val As Double = Convert.ToDouble(w.ToString())
									result.Append(SVMLightRecordReader.LABEL_DELIMITER & val)
								End Try
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
					Dim w As Writable = record(i)
					' Handle array-structured Writables, which themselves have multiple columns
					If TypeOf w Is ArrayWritable Then
						Dim arr As ArrayWritable = DirectCast(w, ArrayWritable)
						For j As Integer = 0 To arr.length() - 1
							Dim val As Double = arr.getDouble(j)
							If val <> 0 Then
								result.Append(SVMLightRecordReader.PREFERRED_DELIMITER & featureIndex)
								result.Append(SVMLightRecordReader.FEATURE_DELIMITER & val)
							End If
							featureIndex += 1 ' Increment feature index for each entry in array
						Next j
					Else
						Dim val As Double = w.toDouble()
						If val <> 0 Then
							result.Append(SVMLightRecordReader.PREFERRED_DELIMITER & featureIndex)
							result.Append(SVMLightRecordReader.FEATURE_DELIMITER & val)
						End If
						featureIndex += 1 ' Increment feature index once per scalar Writable
					End If
				Next i

				' Remove extra label delimiter at beginning
				Dim line As String = result.Substring(1).ToString()
				[out].write(line.GetBytes())
				[out].write(NEW_LINE.GetBytes())

			End If

			Return org.datavec.api.Split.partition.PartitionMetaData.builder().numRecordsUpdated(1).build()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.split.partition.PartitionMetaData writeBatch(java.util.List<java.util.List<org.datavec.api.writable.Writable>> batch) throws java.io.IOException
		Public Overridable Overloads Function writeBatch(ByVal batch As IList(Of IList(Of Writable))) As PartitionMetaData
			Throw New NotImplementedException("writeBatch is not supported on " & Me.GetType().Name)
		End Function
	End Class

End Namespace