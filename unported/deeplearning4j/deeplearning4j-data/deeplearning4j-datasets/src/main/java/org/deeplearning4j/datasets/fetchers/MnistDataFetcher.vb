Imports System
Imports SneakyThrows = lombok.SneakyThrows
Imports FileUtils = org.apache.commons.io.FileUtils
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports MnistFetcher = org.deeplearning4j.datasets.base.MnistFetcher
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports ResourceType = org.deeplearning4j.common.resources.ResourceType
Imports MnistManager = org.deeplearning4j.datasets.mnist.MnistManager
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports BaseDataFetcher = org.nd4j.linalg.dataset.api.iterator.fetcher.BaseDataFetcher
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports MathUtils = org.nd4j.common.util.MathUtils

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



	<Serializable>
	Public Class MnistDataFetcher
		Inherits BaseDataFetcher

		Public Const NUM_EXAMPLES As Integer = 60000
		Public Const NUM_EXAMPLES_TEST As Integer = 10000

		Protected Friend Const CHECKSUM_TRAIN_FEATURES As Long = 2094436111L
		Protected Friend Const CHECKSUM_TRAIN_LABELS As Long = 4008842612L
		Protected Friend Const CHECKSUM_TEST_FEATURES As Long = 2165396896L
		Protected Friend Const CHECKSUM_TEST_LABELS As Long = 2212998611L

		Protected Friend Shared ReadOnly CHECKSUMS_TRAIN() As Long = {CHECKSUM_TRAIN_FEATURES, CHECKSUM_TRAIN_LABELS}
		Protected Friend Shared ReadOnly CHECKSUMS_TEST() As Long = {CHECKSUM_TEST_FEATURES, CHECKSUM_TEST_LABELS}

		Protected Friend binarize As Boolean = True
		Protected Friend train As Boolean
		Protected Friend order() As Integer
		Protected Friend rng As Random
		Protected Friend shuffle As Boolean
		Protected Friend oneIndexed As Boolean = False
		Protected Friend fOrder As Boolean = False 'MNIST is C order, EMNIST is F order

		Protected Friend firstShuffle As Boolean = True
		Protected Friend ReadOnly numExamples As Integer
		Protected Friend images, labels As String
		'note: we default to zero here on purpose, otherwise when first initializes an error is thrown.
		Private lastCursor As Long = 0


		''' <summary>
		''' Constructor telling whether to binarize the dataset or not </summary>
		''' <param name="binarize"> whether to binarize the dataset or not </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public MnistDataFetcher(boolean binarize) throws java.io.IOException
		Public Sub New(ByVal binarize As Boolean)
			Me.New(binarize, True, True, DateTimeHelper.CurrentUnixTimeMillis(), NUM_EXAMPLES)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public MnistDataFetcher(boolean binarize, boolean train, boolean shuffle, long rngSeed, int numExamples) throws java.io.IOException
		Public Sub New(ByVal binarize As Boolean, ByVal train As Boolean, ByVal shuffle As Boolean, ByVal rngSeed As Long, ByVal numExamples As Integer)
			If Not mnistExists() Then
				Call (New MnistFetcher()).downloadAndUntar()
			End If

			Dim MNIST_ROOT As String = DL4JResources.getDirectory(ResourceType.DATASET, "MNIST").getAbsolutePath()
			Dim checksums() As Long
			If train Then
				images = FilenameUtils.concat(MNIST_ROOT, MnistFetcher.TRAINING_FILES_FILENAME_UNZIPPED)
				labels = FilenameUtils.concat(MNIST_ROOT, MnistFetcher.TRAINING_FILE_LABELS_FILENAME_UNZIPPED)
				totalExamples_Conflict = NUM_EXAMPLES
				checksums = CHECKSUMS_TRAIN
			Else
				images = FilenameUtils.concat(MNIST_ROOT, MnistFetcher.TEST_FILES_FILENAME_UNZIPPED)
				labels = FilenameUtils.concat(MNIST_ROOT, MnistFetcher.TEST_FILE_LABELS_FILENAME_UNZIPPED)
				totalExamples_Conflict = NUM_EXAMPLES_TEST
				checksums = CHECKSUMS_TEST
			End If
			Dim files() As String = {images, labels}

			Try
				Dim man As New MnistManager(images, labels, train)
				validateFiles(files, checksums)
				man.close()
			Catch e As Exception
				Try
					FileUtils.deleteDirectory(New File(MNIST_ROOT))
				Catch e2 As Exception
				End Try
				Call (New MnistFetcher()).downloadAndUntar()
				Dim man As New MnistManager(images, labels, train)
				lastCursor = man.getCurrent()
				validateFiles(files, checksums)
				man.close()
			End Try

			Dim man As New MnistManager(images, labels, train)

			numOutcomes = 10
			Me.binarize = binarize
			cursor_Conflict = 0
			inputColumns_Conflict = man.Images.EntryLength
			Me.train = train
			Me.shuffle = shuffle

			If train Then
				order = New Integer(NUM_EXAMPLES - 1){}
			Else
				order = New Integer(NUM_EXAMPLES_TEST - 1){}
			End If
			For i As Integer = 0 To order.Length - 1
				order(i) = i
			Next i
			rng = New Random(rngSeed)
			Me.numExamples = numExamples
			reset() 'Shuffle order
			man.close()
		End Sub

		Private Function mnistExists() As Boolean
			Dim MNIST_ROOT As String = DL4JResources.getDirectory(ResourceType.DATASET, "MNIST").getAbsolutePath()
			'Check 4 files:
			Dim f As New File(MNIST_ROOT, MnistFetcher.TRAINING_FILES_FILENAME_UNZIPPED)
			If Not f.exists() Then
				Return False
			End If
			f = New File(MNIST_ROOT, MnistFetcher.TRAINING_FILE_LABELS_FILENAME_UNZIPPED)
			If Not f.exists() Then
				Return False
			End If
			f = New File(MNIST_ROOT, MnistFetcher.TEST_FILES_FILENAME_UNZIPPED)
			If Not f.exists() Then
				Return False
			End If
			f = New File(MNIST_ROOT, MnistFetcher.TEST_FILE_LABELS_FILENAME_UNZIPPED)
			If Not f.exists() Then
				Return False
			End If
			Return True
		End Function

		Private Sub validateFiles(ByVal files() As String, ByVal checksums() As Long)
			'Validate files:
			Try
				For i As Integer = 0 To files.Length - 1
					Dim f As New File(files(i))
					Dim adler As Checksum = New Adler32()
					Dim checksum As Long = If(f.exists(), FileUtils.checksum(f, adler).getValue(), -1)
					If Not f.exists() OrElse checksum <> checksums(i) Then
						Throw New System.InvalidOperationException("Failed checksum: expected " & checksums(i) & ", got " & checksum & " for file: " & f)
					End If
				Next i
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public MnistDataFetcher() throws java.io.IOException
		Public Sub New()
			Me.New(True)
		End Sub

		Private featureData()() As Single = Nothing

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SneakyThrows @Override public void fetch(int numExamples)
		Public Overrides Sub fetch(ByVal numExamples As Integer)
			If Not hasMore() Then
				Throw New System.InvalidOperationException("Unable to get more; there are no more images")
			End If

			Dim man As New MnistManager(images, labels, totalExamples_Conflict)
			man.setCurrent(CInt(lastCursor))
			Dim labels As INDArray = Nd4j.zeros(DataType.FLOAT, numExamples, numOutcomes)

			If featureData Is Nothing OrElse featureData.Length < numExamples Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: featureData = new Single[numExamples][28 * 28]
				featureData = RectangularArrays.RectangularSingleArray(numExamples, 28 * 28)
			End If

			Dim actualExamples As Integer = 0
			Dim working() As SByte = Nothing
			Dim i As Integer = 0
			Do While i < numExamples
				If Not hasMore() Then
					Exit Do
				End If

				man.setCurrent(cursor_Conflict)
				lastCursor = cursor_Conflict
				Dim img() As SByte = man.readImageUnsafe(order(cursor_Conflict))

				If fOrder Then
					'EMNIST requires F order to C order
					If working Is Nothing Then
						working = New SByte((28 * 28) - 1){}
					End If
					For j As Integer = 0 To (28 * 28) - 1
						working(j) = img(28 * (j Mod 28) + j \ 28)
					Next j
					img = working
				End If

				Dim label As Integer = man.readLabel(order(cursor_Conflict))
				If oneIndexed Then
					'For some inexplicable reason, Emnist LETTERS set is indexed 1 to 26 (i.e., 1 to nClasses), while everything else
					' is indexed (0 to nClasses-1) :/
					label -= 1
				End If

				labels.put(actualExamples, label, 1.0f)

				For j As Integer = 0 To img.Length - 1
					featureData(actualExamples)(j) = (CInt(img(j))) And &HFF
				Next j

				actualExamples += 1
				i += 1
				cursor_Conflict += 1
			Loop

			Dim features As INDArray

			If featureData.Length = actualExamples Then
				features = Nd4j.create(featureData)
			Else
				features = Nd4j.create(Arrays.CopyOfRange(featureData, 0, actualExamples))
			End If

			If actualExamples < numExamples Then
				labels = labels.get(NDArrayIndex.interval(0, actualExamples), NDArrayIndex.all())
			End If

			If binarize Then
				features = features.gt(30.0).castTo(DataType.FLOAT)
			Else
				features.divi(255.0)
			End If

			curr = New DataSet(features, labels)
			man.close()
		End Sub

		Public Overrides Sub reset()
			cursor_Conflict = 0
			curr = Nothing
			If shuffle Then
				If (train AndAlso numExamples < NUM_EXAMPLES) OrElse (Not train AndAlso numExamples < NUM_EXAMPLES_TEST) Then
					'Shuffle only first N elements
					If firstShuffle Then
						MathUtils.shuffleArray(order, rng)
						firstShuffle = False
					Else
						MathUtils.shuffleArraySubset(order, numExamples, rng)
					End If
				Else
					MathUtils.shuffleArray(order, rng)
				End If
			End If
		End Sub

		Public Overrides Function [next]() As DataSet
'JAVA TO VB CONVERTER NOTE: The local variable next was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim next_Conflict As DataSet = MyBase.next()
			Return next_Conflict
		End Function

		Public Overridable Sub close()
		End Sub

	End Class

End Namespace