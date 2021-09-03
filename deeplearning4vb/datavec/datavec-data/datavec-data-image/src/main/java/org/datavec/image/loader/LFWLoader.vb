Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BalancedPathFilter = org.datavec.api.io.filters.BalancedPathFilter
Imports PathLabelGenerator = org.datavec.api.io.labels.PathLabelGenerator
Imports PatternPathLabelGenerator = org.datavec.api.io.labels.PatternPathLabelGenerator
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports InputSplit = org.datavec.api.split.InputSplit
Imports Image = org.datavec.image.data.Image
Imports ImageRecordReader = org.datavec.image.recordreader.ImageRecordReader
Imports ImageTransform = org.datavec.image.transform.ImageTransform
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.datavec.image.loader



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class LFWLoader extends BaseImageLoader implements java.io.Serializable
	<Serializable>
	Public Class LFWLoader
		Inherits BaseImageLoader

		Public Const NUM_IMAGES As Integer = 13233
		Public Const NUM_LABELS As Integer = 5749
		Public Const SUB_NUM_IMAGES As Integer = 1054
		Public Const SUB_NUM_LABELS As Integer = 432
		Public Const HEIGHT As Integer = 250
		Public Const WIDTH As Integer = 250
		Public Const CHANNELS As Integer = 3
		Public Const DATA_URL As String = "http://vis-www.cs.umass.edu/lfw/lfw.tgz"
		Public Const LABEL_URL As String = "http://vis-www.cs.umass.edu/lfw/lfw-names.txt"
		Public Const SUBSET_URL As String = "http://vis-www.cs.umass.edu/lfw/lfw-a.tgz"
		Protected Friend Const REGEX_PATTERN As String = ".[0-9]+"
		Public Shared ReadOnly LABEL_PATTERN As PathLabelGenerator = New PatternPathLabelGenerator(REGEX_PATTERN)

		Public dataFile As String = "lfw"
		Public labelFile As String = "lfw-names.txt"
		Public subsetFile As String = "lfw-a"

		Public localDir As String = "lfw"
		Public localSubDir As String = "lfw-a/lfw"
		Protected Friend fullDir As File

		Protected Friend useSubset As Boolean = False
		Protected Friend inputSplit() As org.datavec.api.Split.InputSplit

		Public lfwData As IDictionary(Of String, String) = New Dictionary(Of String, String)()
		Public lfwLabel As IDictionary(Of String, String) = New Dictionary(Of String, String)()
		Public lfwSubsetData As IDictionary(Of String, String) = New Dictionary(Of String, String)()

		Public Sub New()
			Me.New(False)
		End Sub

		Public Sub New(ByVal useSubset As Boolean)
			Me.New(New Long() {HEIGHT, WIDTH, CHANNELS}, Nothing, useSubset)
		End Sub

		Public Sub New(ByVal imgDim() As Integer, ByVal useSubset As Boolean)
			Me.New(imgDim, Nothing, useSubset)
		End Sub

		Public Sub New(ByVal imgDim() As Long, ByVal useSubset As Boolean)
			Me.New(imgDim, Nothing, useSubset)
		End Sub

		Public Sub New(ByVal imgDim() As Integer, ByVal imgTransform As ImageTransform, ByVal useSubset As Boolean)
			Me.height = imgDim(0)
			Me.width = imgDim(1)
			Me.channels = imgDim(2)
			Me.imageTransform = imgTransform
			Me.useSubset = useSubset
			Me.localDir = If(useSubset, localSubDir, localDir)
			Me.fullDir = New File(BASE_DIR, localDir)
			generateLfwMaps()
		End Sub

		Public Sub New(ByVal imgDim() As Long, ByVal imgTransform As ImageTransform, ByVal useSubset As Boolean)
			Me.height = imgDim(0)
			Me.width = imgDim(1)
			Me.channels = imgDim(2)
			Me.imageTransform = imgTransform
			Me.useSubset = useSubset
			Me.localDir = If(useSubset, localSubDir, localDir)
			Me.fullDir = New File(BASE_DIR, localDir)
			generateLfwMaps()
		End Sub

		Public Overridable Sub generateLfwMaps()
			If useSubset Then
				' Subset of just faces with a name starting with A
				lfwSubsetData("filesFilename") = Path.GetFileName(SUBSET_URL)
				lfwSubsetData("filesURL") = SUBSET_URL
				lfwSubsetData("filesFilenameUnzipped") = subsetFile

			Else
				lfwData("filesFilename") = Path.GetFileName(DATA_URL)
				lfwData("filesURL") = DATA_URL
				lfwData("filesFilenameUnzipped") = dataFile

				lfwLabel("filesFilename") = labelFile
				lfwLabel("filesURL") = LABEL_URL
				lfwLabel("filesFilenameUnzipped") = labelFile
			End If

		End Sub

		Public Overridable Sub load()
			load(NUM_IMAGES, NUM_IMAGES, NUM_LABELS, LABEL_PATTERN, 1, rng)
		End Sub

		Public Overridable Sub load(ByVal batchSize As Long, ByVal numExamples As Long, ByVal numLabels As Long, ByVal labelGenerator As PathLabelGenerator, ByVal splitTrainTest As Double, ByVal rng As Random)
			If Not imageFilesExist() Then
				If Not fullDir.exists() OrElse fullDir.listFiles() Is Nothing OrElse fullDir.listFiles().length = 0 Then
					fullDir.mkdir()

					If useSubset Then
						log.info("Downloading {} subset...", localDir)
						downloadAndUntar(lfwSubsetData, fullDir)
					Else
						log.info("Downloading {}...", localDir)
						downloadAndUntar(lfwData, fullDir)
						downloadAndUntar(lfwLabel, fullDir)
					End If
				End If
			End If
			Dim fileSplit As New org.datavec.api.Split.FileSplit(fullDir, ALLOWED_FORMATS, rng)
			Dim pathFilter As New BalancedPathFilter(rng, ALLOWED_FORMATS, labelGenerator, numExamples, numLabels, 0, batchSize, Nothing)
			inputSplit = fileSplit.sample(pathFilter, numExamples * splitTrainTest, numExamples * (1 - splitTrainTest))
		End Sub

		Public Overridable Function imageFilesExist() As Boolean
			If useSubset Then
				Dim f As New File(BASE_DIR, lfwSubsetData("filesFilenameUnzipped"))
				If Not f.exists() Then
					Return False
				End If
			Else
				Dim f As New File(BASE_DIR, lfwData("filesFilenameUnzipped"))
				If Not f.exists() Then
					Return False
				End If
				f = New File(BASE_DIR, lfwLabel("filesFilenameUnzipped"))
				If Not f.exists() Then
					Return False
				End If
			End If
			Return True
		End Function


		Public Overridable Function getRecordReader(ByVal numExamples As Long) As RecordReader
			Return getRecordReader(numExamples, numExamples, New Long() {height, width, channels},If(useSubset, SUB_NUM_LABELS, NUM_LABELS), LABEL_PATTERN, True, 1, New Random(DateTimeHelper.CurrentUnixTimeMillis()))
		End Function

		Public Overridable Function getRecordReader(ByVal batchSize As Long, ByVal numExamples As Long, ByVal numLabels As Long, ByVal rng As Random) As RecordReader
			Return getRecordReader(numExamples, batchSize, New Long() {height, width, channels}, numLabels, LABEL_PATTERN, True, 1, rng)
		End Function

		Public Overridable Function getRecordReader(ByVal batchSize As Long, ByVal numExamples As Long, ByVal train As Boolean, ByVal splitTrainTest As Double) As RecordReader
			Return getRecordReader(numExamples, batchSize, New Long() {height, width, channels},If(useSubset, SUB_NUM_LABELS, NUM_LABELS), LABEL_PATTERN, train, splitTrainTest, New Random(DateTimeHelper.CurrentUnixTimeMillis()))
		End Function

		Public Overridable Function getRecordReader(ByVal batchSize As Long, ByVal numExamples As Long, ByVal imgDim() As Integer, ByVal train As Boolean, ByVal splitTrainTest As Double, ByVal rng As Random) As RecordReader
			Return getRecordReader(numExamples, batchSize, imgDim,If(useSubset, SUB_NUM_LABELS, NUM_LABELS), LABEL_PATTERN, train, splitTrainTest, rng)
		End Function

		Public Overridable Function getRecordReader(ByVal batchSize As Long, ByVal numExamples As Long, ByVal imgDim() As Long, ByVal train As Boolean, ByVal splitTrainTest As Double, ByVal rng As Random) As RecordReader
			Return getRecordReader(numExamples, batchSize, imgDim,If(useSubset, SUB_NUM_LABELS, NUM_LABELS), LABEL_PATTERN, train, splitTrainTest, rng)
		End Function

		Public Overridable Function getRecordReader(ByVal batchSize As Long, ByVal numExamples As Long, ByVal labelGenerator As PathLabelGenerator, ByVal train As Boolean, ByVal splitTrainTest As Double, ByVal rng As Random) As RecordReader
			Return getRecordReader(numExamples, batchSize, New Long() {height, width, channels},If(useSubset, SUB_NUM_LABELS, NUM_LABELS), labelGenerator, train, splitTrainTest, rng)
		End Function

		Public Overridable Function getRecordReader(ByVal batchSize As Long, ByVal numExamples As Long, ByVal imgDim() As Integer, ByVal labelGenerator As PathLabelGenerator, ByVal train As Boolean, ByVal splitTrainTest As Double, ByVal rng As Random) As RecordReader
			Return getRecordReader(numExamples, batchSize, imgDim,If(useSubset, SUB_NUM_LABELS, NUM_LABELS), labelGenerator, train, splitTrainTest, rng)
		End Function

		Public Overridable Function getRecordReader(ByVal batchSize As Long, ByVal numExamples As Long, ByVal imgDim() As Long, ByVal labelGenerator As PathLabelGenerator, ByVal train As Boolean, ByVal splitTrainTest As Double, ByVal rng As Random) As RecordReader
			Return getRecordReader(numExamples, batchSize, imgDim,If(useSubset, SUB_NUM_LABELS, NUM_LABELS), labelGenerator, train, splitTrainTest, rng)
		End Function

		Public Overridable Function getRecordReader(ByVal batchSize As Long, ByVal numExamples As Long, ByVal imgDim() As Integer, ByVal numLabels As Long, ByVal labelGenerator As PathLabelGenerator, ByVal train As Boolean, ByVal splitTrainTest As Double, ByVal rng As Random) As RecordReader
			load(batchSize, numExamples, numLabels, labelGenerator, splitTrainTest, rng)
			Dim recordReader As RecordReader = New ImageRecordReader(imgDim(0), imgDim(1), imgDim(2), labelGenerator, imageTransform)

			Try
				Dim data As org.datavec.api.Split.InputSplit = If(train, inputSplit(0), inputSplit(1))
				recordReader.initialize(data)
			Catch e As Exception When TypeOf e Is IOException OrElse TypeOf e Is InterruptedException
				log.error("",e)
			End Try
			Return recordReader
		End Function

		Public Overridable Function getRecordReader(ByVal batchSize As Long, ByVal numExamples As Long, ByVal imgDim() As Long, ByVal numLabels As Long, ByVal labelGenerator As PathLabelGenerator, ByVal train As Boolean, ByVal splitTrainTest As Double, ByVal rng As Random) As RecordReader
			load(batchSize, numExamples, numLabels, labelGenerator, splitTrainTest, rng)
			Dim recordReader As RecordReader = New ImageRecordReader(imgDim(0), imgDim(1), imgDim(2), labelGenerator, imageTransform)

			Try
				Dim data As org.datavec.api.Split.InputSplit = If(train, inputSplit(0), inputSplit(1))
				recordReader.initialize(data)
			Catch e As Exception When TypeOf e Is IOException OrElse TypeOf e Is InterruptedException
				log.error("",e)
			End Try
			Return recordReader
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray asRowVector(java.io.File f) throws java.io.IOException
		Public Overrides Function asRowVector(ByVal f As File) As INDArray
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray asRowVector(java.io.InputStream inputStream) throws java.io.IOException
		Public Overrides Function asRowVector(ByVal inputStream As Stream) As INDArray
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray asMatrix(java.io.File f) throws java.io.IOException
		Public Overrides Function asMatrix(ByVal f As File) As INDArray
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray asMatrix(java.io.File f, boolean nchw) throws java.io.IOException
		Public Overrides Function asMatrix(ByVal f As File, ByVal nchw As Boolean) As INDArray
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray asMatrix(java.io.InputStream inputStream) throws java.io.IOException
		Public Overrides Function asMatrix(ByVal inputStream As Stream) As INDArray
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray asMatrix(java.io.InputStream inputStream, boolean nchw) throws java.io.IOException
		Public Overrides Function asMatrix(ByVal inputStream As Stream, ByVal nchw As Boolean) As INDArray
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.image.data.Image asImageMatrix(java.io.File f) throws java.io.IOException
		Public Overrides Function asImageMatrix(ByVal f As File) As Image
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.image.data.Image asImageMatrix(java.io.File f, boolean nchw) throws java.io.IOException
		Public Overrides Function asImageMatrix(ByVal f As File, ByVal nchw As Boolean) As Image
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.image.data.Image asImageMatrix(java.io.InputStream inputStream) throws java.io.IOException
		Public Overrides Function asImageMatrix(ByVal inputStream As Stream) As Image
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.image.data.Image asImageMatrix(java.io.InputStream inputStream, boolean nchw) throws java.io.IOException
		Public Overrides Function asImageMatrix(ByVal inputStream As Stream, ByVal nchw As Boolean) As Image
			Throw New System.NotSupportedException()
		End Function

	End Class

End Namespace