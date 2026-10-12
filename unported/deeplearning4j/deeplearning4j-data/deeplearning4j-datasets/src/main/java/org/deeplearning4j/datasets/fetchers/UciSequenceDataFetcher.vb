Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports CSVSequenceRecordReader = org.datavec.api.records.reader.impl.csv.CSVSequenceRecordReader
Imports NumberedFileInputSplit = org.datavec.api.split.NumberedFileInputSplit
Imports ImageTransform = org.datavec.image.transform.ImageTransform
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.deeplearning4j.datasets.fetchers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class UciSequenceDataFetcher extends CacheableExtractableDataSetFetcher
	Public Class UciSequenceDataFetcher
		Inherits CacheableExtractableDataSetFetcher

		Public Shared NUM_LABELS As Integer = 6
		Public Shared NUM_EXAMPLES As Integer = NUM_LABELS * 100
'JAVA TO VB CONVERTER NOTE: The field url was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared url_Conflict As String = "https://archive.ics.uci.edu/ml/machine-learning-databases/synthetic_control-mld/synthetic_control.data"

		Public Shared WriteOnly Property URL As String
			Set(ByVal url As String)
				UciSequenceDataFetcher.url_Conflict = url
			End Set
		End Property

		Public Overrides Function remoteDataUrl() As String
			Return url_Conflict
		End Function

		Public Overrides Function remoteDataUrl(ByVal type As DataSetType) As String
			Return remoteDataUrl()
		End Function

		Public Overrides Function localCacheName() As String
			Return "UCISequence_6"
		End Function

		Public Overrides Function expectedChecksum() As Long
			Return 104392751L
		End Function

		Public Overrides Function expectedChecksum(ByVal type As DataSetType) As Long
			Return expectedChecksum()
		End Function

		Public Overrides Function getRecordReader(ByVal rngSeed As Long, ByVal shape() As Integer, ByVal set As DataSetType, ByVal transform As ImageTransform) As CSVSequenceRecordReader
			Return getRecordReader(rngSeed, set)
		End Function

		Public Overridable Overloads Function getRecordReader(ByVal rngSeed As Long, ByVal set As DataSetType) As CSVSequenceRecordReader

			' check empty cache
			Dim localCache As File = LocalCacheDir
			deleteIfEmpty(localCache)

			Try
				If Not localCache.exists() Then
					downloadAndExtract()
				End If
			Catch e As Exception
				Throw New Exception("Could not download UCI Sequence data", e)
			End Try

			Dim dataPath As File

			Select Case set
				Case org.deeplearning4j.datasets.fetchers.DataSetType.TRAIN
					dataPath = New File(localCache, "/train")
				Case org.deeplearning4j.datasets.fetchers.DataSetType.TEST
					dataPath = New File(localCache, "/test")
				Case org.deeplearning4j.datasets.fetchers.DataSetType.VALIDATION
					Throw New System.ArgumentException("You will need to manually iterate the directory, UCISequence data does not provide labels")

				Case Else
					dataPath = New File(localCache, "/train")
			End Select

			Try
				downloadUCIData(dataPath)
				Dim data As CSVSequenceRecordReader
				Select Case set
					Case org.deeplearning4j.datasets.fetchers.DataSetType.TRAIN
						data = New CSVSequenceRecordReader(0, ", ")
						data.initialize(New org.datavec.api.Split.NumberedFileInputSplit(dataPath.getAbsolutePath() & "/%d.csv", 0, 449))
					Case org.deeplearning4j.datasets.fetchers.DataSetType.TEST
						data = New CSVSequenceRecordReader(0, ", ")
						data.initialize(New org.datavec.api.Split.NumberedFileInputSplit(dataPath.getAbsolutePath() & "/%d.csv", 450, 599))
					Case Else
						data = New CSVSequenceRecordReader(0, ", ")
						data.initialize(New org.datavec.api.Split.NumberedFileInputSplit(dataPath.getAbsolutePath() & "/%d.csv", 0, 449))
				End Select

				Return data
			Catch e As Exception
				Throw New Exception("Could not process UCI data", e)
			End Try
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void downloadUCIData(java.io.File dataPath) throws Exception
		Private Shared Sub downloadUCIData(ByVal dataPath As File)
			'if (dataPath.exists()) return;

			Dim data As String = IOUtils.toString(New URL(url_Conflict), Charset.defaultCharset())
			Dim lines() As String = data.Split(vbLf, True)

			Dim lineCount As Integer = 0
			Dim index As Integer = 0

			Dim linesList As New List(Of String)()

			For Each line As String In lines

				' label value
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: int count = lineCount++ / 100;
				Dim count As Integer = lineCount \ 100
					lineCount += 1

				' replace white space with commas and label value + new line
				line = line.replaceAll("\s+", ", " & count & vbLf)

				' add label to last number
				line = line & ", " & count
				linesList.Add(line)
			Next line

			' randomly shuffle data
			Collections.shuffle(linesList, New Random(12345))

			For Each line As String In linesList
				Dim outPath As New File(dataPath, index & ".csv")
				FileUtils.writeStringToFile(outPath, line, Charset.defaultCharset())
				index += 1
			Next line
		End Sub
	End Class

End Namespace