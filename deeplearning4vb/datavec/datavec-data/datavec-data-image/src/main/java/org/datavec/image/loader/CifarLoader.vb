Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports OpenCVFrameConverter = org.bytedeco.javacv.OpenCVFrameConverter
Imports Sum = org.nd4j.linalg.api.ops.impl.reduce.same.Sum
Imports org.nd4j.common.primitives
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports ColorConversionTransform = org.datavec.image.transform.ColorConversionTransform
Imports EqualizeHistTransform = org.datavec.image.transform.EqualizeHistTransform
Imports ImageTransform = org.datavec.image.transform.ImageTransform
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports FeatureUtil = org.nd4j.linalg.util.FeatureUtil
Imports org.bytedeco.opencv.opencv_core
Imports org.bytedeco.opencv.global.opencv_core
Imports org.bytedeco.opencv.global.opencv_imgproc

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
'ORIGINAL LINE: @Slf4j public class CifarLoader extends NativeImageLoader implements Serializable
	<Serializable>
	Public Class CifarLoader
		Inherits NativeImageLoader

		Public Const NUM_TRAIN_IMAGES As Integer = 50000
		Public Const NUM_TEST_IMAGES As Integer = 10000
		Public Const NUM_LABELS As Integer = 10 ' Note 6000 imgs per class
		Public Const HEIGHT As Integer = 32
		Public Const WIDTH As Integer = 32
		Public Const CHANNELS As Integer = 3
		Public Const DEFAULT_USE_SPECIAL_PREPROC As Boolean = False
		Public Const DEFAULT_SHUFFLE As Boolean = True

		Private Const BYTEFILELEN As Integer = 3073
		Private Shared ReadOnly TRAINFILENAMES() As String = {"data_batch_1.bin", "data_batch_2.bin", "data_batch_3.bin", "data_batch_4.bin", "data_batch5.bin"}
		Private Const TESTFILENAME As String = "test_batch.bin"
		Private Const dataBinUrl As String = "https://www.cs.toronto.edu/~kriz/cifar-10-binary.tar.gz"
		Private Const localDir As String = "cifar"
		Private Const dataBinFile As String = "cifar-10-batches-bin"
		Private Const labelFileName As String = "batches.meta.txt"
		Private Const numToConvertDS As Integer = 10000 ' Each file is 10000 images, limiting for file preprocess load

		Protected Friend ReadOnly fullDir As File
		Protected Friend ReadOnly meanVarPath As File
		Protected Friend ReadOnly trainFilesSerialized As String
		Protected Friend ReadOnly testFilesSerialized As String

'JAVA TO VB CONVERTER NOTE: The field inputStream was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend inputStream_Conflict As Stream
		Protected Friend trainInputStream As Stream
		Protected Friend testInputStream As Stream
		Protected Friend labels As IList(Of String) = New List(Of String)()
		Public Shared cifarDataMap As IDictionary(Of String, String) = New Dictionary(Of String, String)()


		Protected Friend train As Boolean
		Protected Friend useSpecialPreProcessCifar As Boolean
		Protected Friend seed As Long
		Protected Friend shuffle As Boolean = True
		Protected Friend numExamples As Integer = 0
		Protected Friend uMean As Double = 0
		Protected Friend uStd As Double = 0
		Protected Friend vMean As Double = 0
		Protected Friend vStd As Double = 0
		Protected Friend meanStdStored As Boolean = False
		Protected Friend loadDSIndex As Integer = 0
		Protected Friend loadDS As New DataSet()
		Protected Friend fileNum As Integer = 0

		Private Shared ReadOnly Property DefaultDirectory As File
			Get
				Return New File(BASE_DIR, FilenameUtils.concat(localDir, dataBinFile))
			End Get
		End Property

		Public Sub New()
			Me.New(True)
		End Sub

		Public Sub New(ByVal train As Boolean)
			Me.New(train, Nothing)
		End Sub

		Public Sub New(ByVal train As Boolean, ByVal fullPath As File)
			Me.New(HEIGHT, WIDTH, CHANNELS, Nothing, train, DEFAULT_USE_SPECIAL_PREPROC, fullPath, DateTimeHelper.CurrentUnixTimeMillis(), DEFAULT_SHUFFLE)
		End Sub

		Public Sub New(ByVal height As Integer, ByVal width As Integer, ByVal channels As Integer, ByVal train As Boolean, ByVal useSpecialPreProcessCifar As Boolean)
			Me.New(height, width, channels, Nothing, train, useSpecialPreProcessCifar)
		End Sub

		Public Sub New(ByVal height As Integer, ByVal width As Integer, ByVal channels As Integer, ByVal imgTransform As ImageTransform, ByVal train As Boolean, ByVal useSpecialPreProcessCifar As Boolean)
			Me.New(height, width, channels, imgTransform, train, useSpecialPreProcessCifar, DEFAULT_SHUFFLE)
		End Sub

		Public Sub New(ByVal height As Integer, ByVal width As Integer, ByVal channels As Integer, ByVal imgTransform As ImageTransform, ByVal train As Boolean, ByVal useSpecialPreProcessCifar As Boolean, ByVal shuffle As Boolean)
			Me.New(height, width, channels, imgTransform, train, useSpecialPreProcessCifar, Nothing, DateTimeHelper.CurrentUnixTimeMillis(), shuffle)
		End Sub

		Public Sub New(ByVal height As Integer, ByVal width As Integer, ByVal channels As Integer, ByVal imgTransform As ImageTransform, ByVal train As Boolean, ByVal useSpecialPreProcessCifar As Boolean, ByVal fullDir As File, ByVal seed As Long, ByVal shuffle As Boolean)
			MyBase.New(height, width, channels, imgTransform)
			Me.height = height
			Me.width = width
			Me.channels = channels
			Me.train = train
			Me.useSpecialPreProcessCifar = useSpecialPreProcessCifar
			Me.seed = seed
			Me.shuffle = shuffle

			If fullDir Is Nothing Then
				Me.fullDir = DefaultDirectory
			Else
				Me.fullDir = fullDir
			End If
			meanVarPath = New File(Me.fullDir, "meanVarPath.txt")
			trainFilesSerialized = FilenameUtils.concat(Me.fullDir.ToString(), "cifar_train_serialized")
			testFilesSerialized = FilenameUtils.concat(Me.fullDir.ToString(), "cifar_test_serialized.ser")

			load()
		End Sub



'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray asRowVector(File f) throws IOException
		Public Overrides Function asRowVector(ByVal f As File) As INDArray
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray asRowVector(InputStream inputStream) throws IOException
		Public Overrides Function asRowVector(ByVal inputStream As Stream) As INDArray
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray asMatrix(File f) throws IOException
		Public Overrides Function asMatrix(ByVal f As File) As INDArray
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray asMatrix(InputStream inputStream) throws IOException
		Public Overrides Function asMatrix(ByVal inputStream As Stream) As INDArray
			Throw New System.NotSupportedException()
		End Function

		Protected Friend Overridable Sub generateMaps()
			cifarDataMap("filesFilename") = Path.GetFileName(dataBinUrl)
			cifarDataMap("filesURL") = dataBinUrl
			cifarDataMap("filesFilenameUnzipped") = dataBinFile
		End Sub

		Private Sub defineLabels()
			Try
				Dim path As New File(fullDir, labelFileName)
				Dim br As New StreamReader(path)
				Dim line As String

				line = br.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = br.readLine()) != null)
				Do While line IsNot Nothing
					labels.Add(line)
						line = br.ReadLine()
				Loop
			Catch e As IOException
				log.error("",e)
			End Try
		End Sub

		Protected Friend Overridable Sub load()
			If Not cifarRawFilesExist() AndAlso Not fullDir.exists() Then
				generateMaps()
				fullDir.mkdir()

				log.info("Downloading CIFAR data set")
				downloadAndUntar(cifarDataMap, New File(BASE_DIR, localDir))
			End If
			Try
				Dim subFiles As ICollection(Of File) = FileUtils.listFiles(fullDir, New String() {"bin"}, True)
				Dim trainIter As IEnumerator(Of File) = subFiles.GetEnumerator()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				trainInputStream = New SequenceInputStream(New FileStream(trainIter.next(), FileMode.Open, FileAccess.Read), New FileStream(trainIter.next(), FileMode.Open, FileAccess.Read))
				Do While trainIter.MoveNext()
					Dim nextFile As File = trainIter.Current
					If Not TESTFILENAME.Equals(nextFile.getName()) Then
						trainInputStream = New SequenceInputStream(trainInputStream, New FileStream(nextFile, FileMode.Open, FileAccess.Read))
					End If
				Loop
				testInputStream = New FileStream(fullDir, TESTFILENAME, FileMode.Open, FileAccess.Read)
			Catch e As Exception
				Throw New Exception(e)
			End Try

			If labels.Count = 0 Then
				defineLabels()
			End If

			If useSpecialPreProcessCifar AndAlso train AndAlso Not cifarProcessedFilesExists() Then
				For i As Integer = fileNum + 1 To (TRAINFILENAMES.Length)
					inputStream_Conflict = trainInputStream
					Dim result As DataSet = convertDataSet(numToConvertDS)
					result.save(New File(trainFilesSerialized & i & ".ser"))
				Next i
				'            for (int i = 1; i <= (TRAINFILENAMES.length); i++){
				'                normalizeCifar(new File(trainFilesSerialized + i + ".ser"));
				'            }
				inputStream_Conflict = testInputStream
				Dim result As DataSet = convertDataSet(numToConvertDS)
				result.save(New File(testFilesSerialized))
				'            normalizeCifar(new File(testFilesSerialized));
			End If
			setInputStream()
		End Sub

		Private Function cifarRawFilesExist() As Boolean
			Dim f As New File(fullDir, TESTFILENAME)
			If Not f.exists() Then
				Return False
			End If

			For Each name As String In TRAINFILENAMES
				f = New File(fullDir, name)
				If Not f.exists() Then
					Return False
				End If
			Next name
			Return True
		End Function

		Private Function cifarProcessedFilesExists() As Boolean
			Dim f As File
			If train Then
				f = New File(trainFilesSerialized & 1 & ".ser")
				If Not f.exists() Then
					Return False
				End If
			Else
				f = New File(testFilesSerialized)
				If Not f.exists() Then
					Return False
				End If
			End If
			Return True
		End Function

		''' <summary>
		''' Preprocess and store cifar based on successful Torch approach by Sergey Zagoruyko
		''' Reference: <a href="https://github.com/szagoruyko/cifar.torch">https://github.com/szagoruyko/cifar.torch</a>
		''' </summary>
		Public Overridable Function convertCifar(ByVal orgImage As Mat) As Mat
			numExamples += 1
			Dim resImage As New Mat()
			Dim converter As New OpenCVFrameConverter.ToMat()
			'        ImageTransform yuvTransform = new ColorConversionTransform(new Random(seed), COLOR_BGR2Luv);
			'        ImageTransform histEqualization = new EqualizeHistTransform(new Random(seed), COLOR_BGR2Luv);
			Dim yuvTransform As ImageTransform = New ColorConversionTransform(New Random(seed), COLOR_BGR2YCrCb)
			Dim histEqualization As ImageTransform = New EqualizeHistTransform(New Random(seed), COLOR_BGR2YCrCb)

			If converter IsNot Nothing Then
				Dim writable As New ImageWritable(converter.convert(orgImage))
				' TODO determine if need to normalize y before transform - opencv docs rec but currently doing after
				writable = yuvTransform.transform(writable) ' Converts to chrome color to help emphasize image objects
				writable = histEqualization.transform(writable) ' Normalizes values to further clarify object of interest
				resImage = converter.convert(writable.Frame)
			End If

			Return resImage
		End Function

		''' <summary>
		''' Normalize and store cifar based on successful Torch approach by Sergey Zagoruyko
		''' Reference: <a href="https://github.com/szagoruyko/cifar.torch">https://github.com/szagoruyko/cifar.torch</a>
		''' </summary>
		Public Overridable Sub normalizeCifar(ByVal fileName As File)
			Dim result As New DataSet()
			result.load(fileName)
			If Not meanStdStored AndAlso train Then
				uMean = Math.Abs(uMean / numExamples)
				uStd = Math.Sqrt(uStd)
				vMean = Math.Abs(vMean / numExamples)
				vStd = Math.Sqrt(vStd)
				' TODO find cleaner way to store and load (e.g. json or yaml)
				Try
					FileUtils.write(meanVarPath, uMean & "," & uStd & "," & vMean & "," & vStd)
				Catch e As IOException
					log.error("",e)
				End Try
				meanStdStored = True
			ElseIf uMean = 0 AndAlso meanStdStored Then
				Try
					Dim values() As String = FileUtils.readFileToString(meanVarPath).Split(",")
					uMean = Double.Parse(values(0))
					uStd = Double.Parse(values(1))
					vMean = Double.Parse(values(2))
					vStd = Double.Parse(values(3))

				Catch e As IOException
					log.error("",e)
				End Try
			End If
			Dim i As Integer = 0
			Do While i < result.numExamples()
				Dim newFeatures As INDArray = result.get(i).getFeatures()
				newFeatures.tensorAlongDimension(0, New Integer() {0, 2, 3}).divi(255)
				newFeatures.tensorAlongDimension(1, New Integer() {0, 2, 3}).subi(uMean).divi(uStd)
				newFeatures.tensorAlongDimension(2, New Integer() {0, 2, 3}).subi(vMean).divi(vStd)
				result.get(i).setFeatures(newFeatures)
				i += 1
			Loop
			result.save(fileName)
		End Sub

		Public Overridable Function convertMat(ByVal byteFeature() As SByte) As Pair(Of INDArray, Mat)
			Dim label As INDArray = FeatureUtil.toOutcomeVector(byteFeature(0), NUM_LABELS) ' first value in the 3073 byte array
			Dim image As New Mat(HEIGHT, WIDTH, CV_8UC(CHANNELS)) ' feature are 3072
			Dim imageData As ByteBuffer = image.createBuffer()

			Dim i As Integer = 0
			Do While i < HEIGHT * WIDTH
				imageData.put(3 * i, byteFeature(i + 1 + 2 * HEIGHT * WIDTH)) ' blue
				imageData.put(3 * i + 1, byteFeature(i + 1 + HEIGHT * WIDTH)) ' green
				imageData.put(3 * i + 2, byteFeature(i + 1)) ' red
				i += 1
			Loop
			'        if (useSpecialPreProcessCifar) {
			'            image = convertCifar(image);
			'        }

			Return New Pair(Of INDArray, Mat)(label, image)
		End Function

		Public Overridable Function convertDataSet(ByVal num As Integer) As DataSet
			Dim batchNumCount As Integer = 0
			Dim dataSets As IList(Of DataSet) = New List(Of DataSet)()
			Dim matConversion As Pair(Of INDArray, Mat)
			Dim byteFeature(BYTEFILELEN - 1) As SByte

			Try
	'            while (inputStream.read(byteFeature) != -1 && batchNumCount != num) {
				Do While batchNumCount <> num AndAlso inputStream_Conflict.Read(byteFeature, 0, byteFeature.Length) <> -1
					matConversion = convertMat(byteFeature)
					Try
						dataSets.Add(New DataSet(asMatrix(matConversion.Second), matConversion.First))
						batchNumCount += 1
					Catch e As Exception
						log.error("",e)
						Exit Do
					End Try
				Loop
			Catch e As IOException
				log.error("",e)
			End Try

			If dataSets.Count = 0 Then
				Return New DataSet()
			End If

			Dim result As DataSet = DataSet.merge(dataSets)

			Dim uTempMean, vTempMean As Double
			For Each data As DataSet In result
				Try
					If useSpecialPreProcessCifar Then
						Dim uChannel As INDArray = data.Features.tensorAlongDimension(1, New Integer() {0, 2, 3})
						Dim vChannel As INDArray = data.Features.tensorAlongDimension(2, New Integer() {0, 2, 3})
						uTempMean = uChannel.meanNumber().doubleValue()
						' TODO INDArray.var result is incorrect based on dimensions passed in thus using manual
						uStd += varManual(uChannel, uTempMean)
						uMean += uTempMean
						vTempMean = vChannel.meanNumber().doubleValue()
						vStd += varManual(vChannel, vTempMean)
						vMean += vTempMean
						data.Features = data.Features.div(255)
					Else
						' normalize if just input stream and not special preprocess
						data.Features = data.Features.div(255)
					End If
				Catch e As System.ArgumentException
					Throw New System.InvalidOperationException("The number of channels must be 3 to special preProcess Cifar with.")
				End Try
			Next data
			If shuffle AndAlso num > 1 Then
				result.shuffle(seed)
			End If
			Return result
		End Function

		Public Overridable Function varManual(ByVal x As INDArray, ByVal mean As Double) As Double
			Dim xSubMean As INDArray = x.sub(mean)
			Dim squared As INDArray = xSubMean.muli(xSubMean)
			Dim accum As Double = Nd4j.Executioner.execAndReturn(New Sum(squared)).getFinalResult().doubleValue()
			Return accum / x.ravel().length()
		End Function

		Public Overridable Function [next](ByVal batchSize As Integer) As DataSet
			Return [next](batchSize, 0)
		End Function

		Public Overridable Function [next](ByVal batchSize As Integer, ByVal exampleNum As Integer) As DataSet
			Dim temp As IList(Of DataSet) = New List(Of DataSet)()
			Dim result As DataSet
			If cifarProcessedFilesExists() AndAlso useSpecialPreProcessCifar Then
				If exampleNum = 0 OrElse ((exampleNum \ fileNum) = numToConvertDS AndAlso train) Then
					fileNum += 1
					If train Then
						loadDS.load(New File(trainFilesSerialized & fileNum & ".ser"))
					End If
					loadDS.load(New File(testFilesSerialized))
					' Shuffle all examples in file before batching happens also for each reset
					If shuffle AndAlso batchSize > 1 Then
						loadDS.shuffle(seed)
					End If
					loadDSIndex = 0
					'          inputBatched = loadDS.batchBy(batchSize);
				End If
				' TODO loading full train dataset when using cuda causes memory error - find way to load into list off gpu
				'            result = inputBatched.get(batchNum);
				For i As Integer = 0 To batchSize - 1
					If loadDS.get(loadDSIndex) IsNot Nothing Then
						temp.Add(loadDS.get(loadDSIndex))
					Else
						Exit For
					End If
					loadDSIndex += 1
				Next i
				If temp.Count > 1 Then
					result = DataSet.merge(temp)
				Else
					result = temp(0)
				End If
			Else
				result = convertDataSet(batchSize)
			End If
			Return result
		End Function

		Public Overridable ReadOnly Property InputStream As Stream
			Get
				Return inputStream_Conflict
			End Get
		End Property

		Public Overridable Sub setInputStream()
			If train Then
				inputStream_Conflict = trainInputStream
			Else
				inputStream_Conflict = testInputStream
			End If
		End Sub

		Public Overridable ReadOnly Property Labels As IList(Of String)
			Get
				Return labels
			End Get
		End Property

		Public Overridable Sub reset()
			numExamples = 0
			fileNum = 0
			load()
		End Sub

	End Class

End Namespace