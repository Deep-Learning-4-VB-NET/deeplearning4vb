Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports ResourceType = org.deeplearning4j.common.resources.ResourceType
Imports Downloader = org.nd4j.common.resources.Downloader

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

Namespace org.deeplearning4j.datasets.base


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @Slf4j public class MnistFetcher
	Public Class MnistFetcher

		Protected Friend Const LOCAL_DIR_NAME As String = "MNIST"

		Private fileDir As File
		Private Const TRAINING_FILES_URL_RELATIVE As String = "datasets/mnist/train-images-idx3-ubyte.gz"
		Private Const TRAINING_FILES_MD_5 As String = "f68b3c2dcbeaaa9fbdd348bbdeb94873"
		Private Const TRAINING_FILES_FILENAME As String = "train-images-idx3-ubyte.gz"
		Public Const TRAINING_FILES_FILENAME_UNZIPPED As String = "train-images-idx3-ubyte"
		Private Const TRAINING_FILE_LABELS_URL_RELATIVE As String = "datasets/mnist/train-labels-idx1-ubyte.gz"
		Private Const TRAINING_FILE_LABELS_MD_5 As String = "d53e105ee54ea40749a09fcbcd1e9432"
		Private Const TRAINING_FILE_LABELS_FILENAME As String = "train-labels-idx1-ubyte.gz"
		Public Const TRAINING_FILE_LABELS_FILENAME_UNZIPPED As String = "train-labels-idx1-ubyte"

		'Test data:
		Private Const TEST_FILES_URL_RELATIVE As String = "datasets/mnist/t10k-images-idx3-ubyte.gz"
		Private Const TEST_FILES_MD_5 As String = "9fb629c4189551a2d022fa330f9573f3"
		Private Const TEST_FILES_FILENAME As String = "t10k-images-idx3-ubyte.gz"
		Public Const TEST_FILES_FILENAME_UNZIPPED As String = "t10k-images-idx3-ubyte"
		Private Const TEST_FILE_LABELS_URL_RELATIVE As String = "datasets/mnist/t10k-labels-idx1-ubyte.gz"
		Private Const TEST_FILE_LABELS_MD_5 As String = "ec29112dd5afa0611ce80d1b7f02629c"
		Private Const TEST_FILE_LABELS_FILENAME As String = "t10k-labels-idx1-ubyte.gz"
		Public Const TEST_FILE_LABELS_FILENAME_UNZIPPED As String = "t10k-labels-idx1-ubyte"


		Public Overridable ReadOnly Property Name As String
			Get
				Return "MNIST"
			End Get
		End Property

		Public Overridable ReadOnly Property BaseDir As File
			Get
				Return DL4JResources.getDirectory(ResourceType.DATASET, Name)
			End Get
		End Property

		' --- Train files ---
		Public Overridable ReadOnly Property TrainingFilesURL As String
			Get
				Return DL4JResources.getURLString(TRAINING_FILES_URL_RELATIVE)
			End Get
		End Property

		Public Overridable ReadOnly Property TrainingFilesMD5 As String
			Get
				Return TRAINING_FILES_MD_5
			End Get
		End Property

		Public Overridable ReadOnly Property TrainingFilesFilename As String
			Get
				Return TRAINING_FILES_FILENAME
			End Get
		End Property

		Public Overridable ReadOnly Property TrainingFilesFilename_unzipped As String
			Get
				Return TRAINING_FILES_FILENAME_UNZIPPED
			End Get
		End Property

		Public Overridable ReadOnly Property TrainingFileLabelsURL As String
			Get
				Return DL4JResources.getURLString(TRAINING_FILE_LABELS_URL_RELATIVE)
			End Get
		End Property

		Public Overridable ReadOnly Property TrainingFileLabelsMD5 As String
			Get
				Return TRAINING_FILE_LABELS_MD_5
			End Get
		End Property

		Public Overridable ReadOnly Property TrainingFileLabelsFilename As String
			Get
				Return TRAINING_FILE_LABELS_FILENAME
			End Get
		End Property

		Public Overridable ReadOnly Property TrainingFileLabelsFilename_unzipped As String
			Get
				Return TRAINING_FILE_LABELS_FILENAME_UNZIPPED
			End Get
		End Property


		' --- Test files ---

		Public Overridable ReadOnly Property TestFilesURL As String
			Get
				Return DL4JResources.getURLString(TEST_FILES_URL_RELATIVE)
			End Get
		End Property

		Public Overridable ReadOnly Property TestFilesMD5 As String
			Get
				Return TEST_FILES_MD_5
			End Get
		End Property

		Public Overridable ReadOnly Property TestFilesFilename As String
			Get
				Return TEST_FILES_FILENAME
			End Get
		End Property

		Public Overridable ReadOnly Property TestFilesFilename_unzipped As String
			Get
				Return TEST_FILES_FILENAME_UNZIPPED
			End Get
		End Property

		Public Overridable ReadOnly Property TestFileLabelsURL As String
			Get
				Return DL4JResources.getURLString(TEST_FILE_LABELS_URL_RELATIVE)
			End Get
		End Property

		Public Overridable ReadOnly Property TestFileLabelsMD5 As String
			Get
				Return TEST_FILE_LABELS_MD_5
			End Get
		End Property

		Public Overridable ReadOnly Property TestFileLabelsFilename As String
			Get
				Return TEST_FILE_LABELS_FILENAME
			End Get
		End Property

		Public Overridable ReadOnly Property TestFileLabelsFilename_unzipped As String
			Get
				Return TEST_FILE_LABELS_FILENAME_UNZIPPED
			End Get
		End Property


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public File downloadAndUntar() throws IOException
		Public Overridable Function downloadAndUntar() As File
			If fileDir IsNot Nothing Then
				Return fileDir
			End If

			Dim baseDir As File = Me.BaseDir
			If Not (baseDir.isDirectory() OrElse baseDir.mkdir()) Then
				Throw New IOException("Could not mkdir " & baseDir)
			End If

			log.info("Downloading {}...", Name)
			' get features
			Dim trainFeatures As New File(baseDir, TrainingFilesFilename)
			Dim testFeatures As New File(baseDir, TestFilesFilename)

			Downloader.downloadAndExtract("MNIST", New URL(TrainingFilesURL), trainFeatures, baseDir, TrainingFilesMD5,3)
			Downloader.downloadAndExtract("MNIST", New URL(TestFilesURL), testFeatures, baseDir, TestFilesMD5, 3)

			' get labels
			Dim trainLabels As New File(baseDir, TrainingFileLabelsFilename)
			Dim testLabels As New File(baseDir, TestFileLabelsFilename)

			Downloader.downloadAndExtract("MNIST", New URL(TrainingFileLabelsURL), trainLabels, baseDir, TrainingFileLabelsMD5, 3)
			Downloader.downloadAndExtract("MNIST", New URL(TestFileLabelsURL), testLabels, baseDir, TestFileLabelsMD5, 3)

			fileDir = baseDir
			Return fileDir
		End Function
	End Class

End Namespace