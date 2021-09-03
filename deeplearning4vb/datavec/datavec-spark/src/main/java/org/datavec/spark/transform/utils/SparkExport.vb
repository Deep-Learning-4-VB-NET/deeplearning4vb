Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports FileUtils = org.apache.commons.io.FileUtils
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Writable = org.datavec.api.writable.Writable
Imports WritablesToStringFunction = org.datavec.spark.transform.misc.WritablesToStringFunction

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

Namespace org.datavec.spark.transform.utils


	Public Class SparkExport

		'Quick and dirty CSV export (using Spark). Eventually, rework this to use DataVec record writers on Spark
		Public Shared Sub exportCSVSpark(ByVal directory As String, ByVal delimiter As String, ByVal outputSplits As Integer, ByVal data As JavaRDD(Of IList(Of Writable)))
			exportCSVSpark(directory, delimiter, Nothing, outputSplits, data)
		End Sub

		Public Shared Sub exportCSVSpark(ByVal directory As String, ByVal delimiter As String, ByVal quote As String, ByVal outputSplits As Integer, ByVal data As JavaRDD(Of IList(Of Writable)))

			'NOTE: Order is probably not random here...
			Dim lines As JavaRDD(Of String) = data.map(New WritablesToStringFunction(delimiter, quote))
			lines.coalesce(outputSplits)

			lines.saveAsTextFile(directory)
		End Sub

		'Another quick and dirty CSV export (local). Dumps all values into a single file
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exportCSVLocal(java.io.File outputFile, String delimiter, org.apache.spark.api.java.JavaRDD<java.util.List<org.datavec.api.writable.Writable>> data, int rngSeed) throws Exception
		Public Shared Sub exportCSVLocal(ByVal outputFile As File, ByVal delimiter As String, ByVal data As JavaRDD(Of IList(Of Writable)), ByVal rngSeed As Integer)
			exportCSVLocal(outputFile, delimiter, Nothing, data, rngSeed)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exportCSVLocal(java.io.File outputFile, String delimiter, String quote, org.apache.spark.api.java.JavaRDD<java.util.List<org.datavec.api.writable.Writable>> data, int rngSeed) throws Exception
		Public Shared Sub exportCSVLocal(ByVal outputFile As File, ByVal delimiter As String, ByVal quote As String, ByVal data As JavaRDD(Of IList(Of Writable)), ByVal rngSeed As Integer)

			Dim lines As JavaRDD(Of String) = data.map(New WritablesToStringFunction(delimiter, quote))
			Dim linesList As IList(Of String) = lines.collect() 'Requires all data in memory
			If Not (TypeOf linesList Is ArrayList) Then
				linesList = New List(Of String)(linesList)
			End If
			Collections.shuffle(linesList, New Random(rngSeed))

			FileUtils.writeLines(outputFile, linesList)
		End Sub

		'Another quick and dirty CSV export (local). Dumps all values into multiple files (specified number of files)
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exportCSVLocal(String outputDir, String baseFileName, int numFiles, String delimiter, org.apache.spark.api.java.JavaRDD<java.util.List<org.datavec.api.writable.Writable>> data, int rngSeed) throws Exception
		Public Shared Sub exportCSVLocal(ByVal outputDir As String, ByVal baseFileName As String, ByVal numFiles As Integer, ByVal delimiter As String, ByVal data As JavaRDD(Of IList(Of Writable)), ByVal rngSeed As Integer)
			exportCSVLocal(outputDir, baseFileName, numFiles, delimiter, Nothing, data, rngSeed)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exportCSVLocal(String outputDir, String baseFileName, int numFiles, String delimiter, String quote, org.apache.spark.api.java.JavaRDD<java.util.List<org.datavec.api.writable.Writable>> data, int rngSeed) throws Exception
		Public Shared Sub exportCSVLocal(ByVal outputDir As String, ByVal baseFileName As String, ByVal numFiles As Integer, ByVal delimiter As String, ByVal quote As String, ByVal data As JavaRDD(Of IList(Of Writable)), ByVal rngSeed As Integer)

			Dim lines As JavaRDD(Of String) = data.map(New WritablesToStringFunction(delimiter, quote))
			Dim split(numFiles - 1) As Double
			For i As Integer = 0 To split.Length - 1
				split(i) = 1.0 / numFiles
			Next i
			Dim splitData() As JavaRDD(Of String) = lines.randomSplit(split)

			Dim count As Integer = 0
			Dim r As New Random(rngSeed)
			For Each subset As JavaRDD(Of String) In splitData
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: String path = org.apache.commons.io.FilenameUtils.concat(outputDir, baseFileName + (count++) + ".csv");
				Dim path As String = FilenameUtils.concat(outputDir, baseFileName & (count) & ".csv")
					count += 1
				Dim linesList As IList(Of String) = subset.collect()
				If Not (TypeOf linesList Is ArrayList) Then
					linesList = New List(Of String)(linesList)
				End If
				Collections.shuffle(linesList, r)
				FileUtils.writeLines(New File(path), linesList)
			Next subset
		End Sub

		' No shuffling
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exportCSVLocal(String outputDir, String baseFileName, int numFiles, String delimiter, org.apache.spark.api.java.JavaRDD<java.util.List<org.datavec.api.writable.Writable>> data) throws Exception
		Public Shared Sub exportCSVLocal(ByVal outputDir As String, ByVal baseFileName As String, ByVal numFiles As Integer, ByVal delimiter As String, ByVal data As JavaRDD(Of IList(Of Writable)))
			exportCSVLocal(outputDir, baseFileName, numFiles, delimiter, Nothing, data)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exportCSVLocal(String outputDir, String baseFileName, int numFiles, String delimiter, String quote, org.apache.spark.api.java.JavaRDD<java.util.List<org.datavec.api.writable.Writable>> data) throws Exception
		Public Shared Sub exportCSVLocal(ByVal outputDir As String, ByVal baseFileName As String, ByVal numFiles As Integer, ByVal delimiter As String, ByVal quote As String, ByVal data As JavaRDD(Of IList(Of Writable)))

			Dim lines As JavaRDD(Of String) = data.map(New WritablesToStringFunction(delimiter, quote))
			Dim split(numFiles - 1) As Double
			For i As Integer = 0 To split.Length - 1
				split(i) = 1.0 / numFiles
			Next i
			Dim splitData() As JavaRDD(Of String) = lines.randomSplit(split)

			Dim count As Integer = 0
			For Each subset As JavaRDD(Of String) In splitData
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: String path = org.apache.commons.io.FilenameUtils.concat(outputDir, baseFileName + (count++) + ".csv");
				Dim path As String = FilenameUtils.concat(outputDir, baseFileName & (count) & ".csv")
					count += 1
				'            subset.saveAsTextFile(path);
				Dim linesList As IList(Of String) = subset.collect()
				FileUtils.writeLines(New File(path), linesList)
			Next subset
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor private static class SequenceToStringFunction implements org.apache.spark.api.java.function.@Function<java.util.List<java.util.List<org.datavec.api.writable.Writable>>, String>
		Private Class SequenceToStringFunction
			Implements [Function](Of IList(Of IList(Of Writable)), String)

			Friend ReadOnly delim As String

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public String call(java.util.List<java.util.List<org.datavec.api.writable.Writable>> sequence) throws Exception
			Public Overrides Function [call](ByVal sequence As IList(Of IList(Of Writable))) As String

				Dim sb As New StringBuilder()
				Dim firstTimeStep As Boolean = True
				For Each c As IList(Of Writable) In sequence
					If Not firstTimeStep Then
						sb.Append(vbLf)
					End If
					Dim first As Boolean = True
					For Each w As Writable In c
						If Not first Then
							sb.Append(delim)
						End If
						sb.Append(w.ToString())
						first = False
					Next w
					firstTimeStep = False
				Next c

				Return sb.ToString()
			End Function
		End Class



		'Another quick and dirty CSV export (local). Dumps all values into a single file
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exportStringLocal(java.io.File outputFile, org.apache.spark.api.java.JavaRDD<String> data, int rngSeed) throws Exception
		Public Shared Sub exportStringLocal(ByVal outputFile As File, ByVal data As JavaRDD(Of String), ByVal rngSeed As Integer)
			Dim linesList As IList(Of String) = data.collect() 'Requires all data in memory
			If Not (TypeOf linesList Is ArrayList) Then
				linesList = New List(Of String)(linesList)
			End If
			Collections.shuffle(linesList, New Random(rngSeed))

			FileUtils.writeLines(outputFile, linesList)
		End Sub

		'Quick and dirty CSV export: one file per sequence, with shuffling of the order of sequences
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exportCSVSequenceLocal(java.io.File baseDir, org.apache.spark.api.java.JavaRDD<java.util.List<java.util.List<org.datavec.api.writable.Writable>>> sequences, long seed) throws Exception
		Public Shared Sub exportCSVSequenceLocal(ByVal baseDir As File, ByVal sequences As JavaRDD(Of IList(Of IList(Of Writable))), ByVal seed As Long)
			baseDir.mkdirs()
			If Not baseDir.isDirectory() Then
				Throw New System.ArgumentException("File is not a directory: " & baseDir.ToString())
			End If
			Dim baseDirStr As String = baseDir.ToString()

			Dim fileContents As IList(Of String) = sequences.map(New SequenceToStringFunction(",")).collect()
			If Not (TypeOf fileContents Is ArrayList) Then
				fileContents = New List(Of String)(fileContents)
			End If
			Collections.shuffle(fileContents, New Random(seed))

			Dim i As Integer = 0
			For Each s As String In fileContents
				Dim path As String = FilenameUtils.concat(baseDirStr, i & ".csv")
				Dim f As New File(path)
				FileUtils.writeStringToFile(f, s)
				i += 1
			Next s
		End Sub

		'Quick and dirty CSV export: one file per sequence, without shuffling
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exportCSVSequenceLocalNoShuffling(java.io.File baseDir, org.apache.spark.api.java.JavaRDD<java.util.List<java.util.List<org.datavec.api.writable.Writable>>> sequences) throws Exception
		Public Shared Sub exportCSVSequenceLocalNoShuffling(ByVal baseDir As File, ByVal sequences As JavaRDD(Of IList(Of IList(Of Writable))))
			exportCSVSequenceLocalNoShuffling(baseDir, sequences, "", ",", "csv")
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exportCSVSequenceLocalNoShuffling(java.io.File baseDir, org.apache.spark.api.java.JavaRDD<java.util.List<java.util.List<org.datavec.api.writable.Writable>>> sequences, String delimiter, String filePrefix, String fileExtension) throws Exception
		Public Shared Sub exportCSVSequenceLocalNoShuffling(ByVal baseDir As File, ByVal sequences As JavaRDD(Of IList(Of IList(Of Writable))), ByVal delimiter As String, ByVal filePrefix As String, ByVal fileExtension As String)
			baseDir.mkdirs()
			If Not baseDir.isDirectory() Then
				Throw New System.ArgumentException("File is not a directory: " & baseDir.ToString())
			End If
			Dim baseDirStr As String = baseDir.ToString()

			Dim fileContents As IList(Of String) = sequences.map(New SequenceToStringFunction(delimiter)).collect()
			If Not (TypeOf fileContents Is ArrayList) Then
				fileContents = New List(Of String)(fileContents)
			End If

			Dim i As Integer = 0
			For Each s As String In fileContents
				Dim path As String = FilenameUtils.concat(baseDirStr, filePrefix & "_" & i & "." & fileExtension)
				Dim f As New File(path)
				FileUtils.writeStringToFile(f, s)
				i += 1
			Next s
		End Sub
	End Class

End Namespace