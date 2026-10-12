Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports EmnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator

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
'ORIGINAL LINE: @Slf4j public class EmnistFetcher extends MnistFetcher
	Public Class EmnistFetcher
		Inherits MnistFetcher

		Private ReadOnly ds As EmnistDataSetIterator.Set

		Public Sub New(ByVal ds As EmnistDataSetIterator.Set)
			Me.ds = ds
		End Sub

		Private Shared Function getImagesFileName(ByVal ds As EmnistDataSetIterator.Set, ByVal train As Boolean) As String
			Return "emnist-" & name(ds) & "-" & (If(train, "train", "test")) & "-images-idx3-ubyte.gz"
		End Function

		Private Shared Function getImagesFileNameUnzipped(ByVal ds As EmnistDataSetIterator.Set, ByVal train As Boolean) As String
			Return "emnist-" & name(ds) & "-" & (If(train, "train", "test")) & "-images-idx3-ubyte"
		End Function

		Private Shared Function getLabelsFileName(ByVal ds As EmnistDataSetIterator.Set, ByVal train As Boolean) As String
			Return "emnist-" & name(ds) & "-" & (If(train, "train", "test")) & "-labels-idx1-ubyte.gz"
		End Function

		Private Shared Function getLabelsFileNameUnzipped(ByVal ds As EmnistDataSetIterator.Set, ByVal train As Boolean) As String
			Return "emnist-" & name(ds) & "-" & (If(train, "train", "test")) & "-labels-idx1-ubyte"
		End Function

		Private Shared Function getMappingFileName(ByVal ds As EmnistDataSetIterator.Set, ByVal train As Boolean) As String
			Return "emnist-" & name(ds) & "-mapping.txt"
		End Function

		Private Shared Function name(ByVal ds As EmnistDataSetIterator.Set) As String
			Select Case ds
				Case EmnistDataSetIterator.Set.COMPLETE
					Return "byclass"
				Case EmnistDataSetIterator.Set.MERGE
					Return "bymerge"
				Case EmnistDataSetIterator.Set.BALANCED
					Return "balanced"
				Case EmnistDataSetIterator.Set.LETTERS
					Return "letters"
				Case EmnistDataSetIterator.Set.DIGITS
					Return "digits"
				Case EmnistDataSetIterator.Set.MNIST
					Return "mnist"
				Case Else
					Throw New System.NotSupportedException("Unknown DataSet: " & ds)
			End Select
		End Function

		Public Overrides ReadOnly Property Name As String
			Get
				Return "EMNIST"
			End Get
		End Property

		' --- Train files ---
		Public Overrides ReadOnly Property TrainingFilesURL As String
			Get
				Return DL4JResources.getURLString("datasets/emnist/" & getImagesFileName(ds, True))
			End Get
		End Property

		Public Overrides ReadOnly Property TrainingFilesMD5 As String
			Get
				Select Case ds
					Case EmnistDataSetIterator.Set.COMPLETE
						'byclass-train-images
						Return "712dda0bd6f00690f32236ae4325c377"
					Case EmnistDataSetIterator.Set.MERGE
						'bymerge-train-images
						Return "4a792d4df261d7e1ba27979573bf53f3"
					Case EmnistDataSetIterator.Set.BALANCED
						'balanced-train-images
						Return "4041b0d6f15785d3fa35263901b5496b"
					Case EmnistDataSetIterator.Set.LETTERS
						'letters-train-images
						Return "8795078f199c478165fe18db82625747"
					Case EmnistDataSetIterator.Set.DIGITS
						'digits-train-images
						Return "d2662ecdc47895a6bbfce25de9e9a677"
					Case EmnistDataSetIterator.Set.MNIST
						'mnist-train-images
						Return "3663598a39195d030895b6304abb5065"
					Case Else
						Throw New System.NotSupportedException("Unknown DataSet: " & ds)
				End Select
			End Get
		End Property

		Public Overrides ReadOnly Property TrainingFilesFilename As String
			Get
				Return getImagesFileName(ds, True)
			End Get
		End Property

		Public Overrides ReadOnly Property TrainingFilesFilename_unzipped As String
			Get
				Return getImagesFileNameUnzipped(ds, True)
			End Get
		End Property

		Public Overrides ReadOnly Property TrainingFileLabelsURL As String
			Get
				Return DL4JResources.getURLString("datasets/emnist/" & getLabelsFileName(ds, True))
			End Get
		End Property

		Public Overrides ReadOnly Property TrainingFileLabelsMD5 As String
			Get
				Select Case ds
					Case EmnistDataSetIterator.Set.COMPLETE
						'byclass-train-labels
						Return "ee299a3ee5faf5c31e9406763eae7e43"
					Case EmnistDataSetIterator.Set.MERGE
						'bymerge-train-labels
						Return "491be69ef99e1ab1f5b7f9ccc908bb26"
					Case EmnistDataSetIterator.Set.BALANCED
						'balanced-train-labels
						Return "7a35cc7b2b7ee7671eddf028570fbd20"
					Case EmnistDataSetIterator.Set.LETTERS
						'letters-train-labels
						Return "c16de4f1848ddcdddd39ab65d2a7be52"
					Case EmnistDataSetIterator.Set.DIGITS
						'digits-train-labels
						Return "2223fcfee618ac9c89ef20b6e48bcf9e"
					Case EmnistDataSetIterator.Set.MNIST
						'mnist-train-labels
						Return "6c092f03c9bb63e678f80f8bc605fe37"
					Case Else
						Throw New System.NotSupportedException("Unknown DataSet: " & ds)
				End Select
			End Get
		End Property

		Public Overrides ReadOnly Property TrainingFileLabelsFilename As String
			Get
				Return getLabelsFileName(ds, True)
			End Get
		End Property

		Public Overrides ReadOnly Property TrainingFileLabelsFilename_unzipped As String
			Get
				Return getLabelsFileNameUnzipped(ds, True)
			End Get
		End Property


		' --- Test files ---

		Public Overrides ReadOnly Property TestFilesURL As String
			Get
				Return DL4JResources.getURLString("datasets/emnist/" & getImagesFileName(ds, False))
			End Get
		End Property

		Public Overrides ReadOnly Property TestFilesMD5 As String
			Get
				Select Case ds
					Case EmnistDataSetIterator.Set.COMPLETE
						'byclass-test-images
						Return "1435209e34070a9002867a9ab50160d7"
					Case EmnistDataSetIterator.Set.MERGE
						'bymerge-test-images
						Return "8eb5d34c91f1759a55831c37ec2a283f"
					Case EmnistDataSetIterator.Set.BALANCED
						'balanced-test-images
						Return "6818d20fe2ce1880476f747bbc80b22b"
					Case EmnistDataSetIterator.Set.LETTERS
						'letters-test-images
						Return "382093a19703f68edac6d01b8dfdfcad"
					Case EmnistDataSetIterator.Set.DIGITS
						'digits-test-images
						Return "a159b8b3bd6ab4ed4793c1cb71a2f5cc"
					Case EmnistDataSetIterator.Set.MNIST
						'mnist-test-images
						Return "fb51b6430fc4dd67deaada1bf25d4524"
					Case Else
						Throw New System.NotSupportedException("Unknown DataSet: " & ds)
				End Select
			End Get
		End Property

		Public Overrides ReadOnly Property TestFilesFilename As String
			Get
				Return getImagesFileName(ds, False)
			End Get
		End Property

		Public Overrides ReadOnly Property TestFilesFilename_unzipped As String
			Get
				Return getImagesFileNameUnzipped(ds, False)
			End Get
		End Property

		Public Overrides ReadOnly Property TestFileLabelsURL As String
			Get
				Return DL4JResources.getURLString("datasets/emnist/" & getLabelsFileName(ds, False))
			End Get
		End Property

		Public Overrides ReadOnly Property TestFileLabelsMD5 As String
			Get
				Select Case ds
					Case EmnistDataSetIterator.Set.COMPLETE
						'byclass-test-labels
						Return "7a0f934bd176c798ecba96b36fda6657"
					Case EmnistDataSetIterator.Set.MERGE
						'bymerge-test-labels
						Return "c13f4cd5211cdba1b8fa992dae2be992"
					Case EmnistDataSetIterator.Set.BALANCED
						'balanced-test-labels
						Return "acd3694070dcbf620e36670519d4b32f"
					Case EmnistDataSetIterator.Set.LETTERS
						'letters-test-labels
						Return "d4108920cd86601ec7689a97f2de7f59"
					Case EmnistDataSetIterator.Set.DIGITS
						'digits-test-labels
						Return "8afde66ea51d865689083ba6bb779fac"
					Case EmnistDataSetIterator.Set.MNIST
						'mnist-test-labels
						Return "ae7f6be798a9a5d5f2bd32e078a402dd"
					Case Else
						Throw New System.NotSupportedException("Unknown DataSet: " & ds)
				End Select
			End Get
		End Property

		Public Overrides ReadOnly Property TestFileLabelsFilename As String
			Get
				Return getLabelsFileName(ds, False)
			End Get
		End Property

		Public Overrides ReadOnly Property TestFileLabelsFilename_unzipped As String
			Get
				Return getLabelsFileNameUnzipped(ds, False)
			End Get
		End Property

		Public Shared Function numLabels(ByVal dataSet As EmnistDataSetIterator.Set) As Integer
			Select Case dataSet
				Case EmnistDataSetIterator.Set.COMPLETE
					Return 62
				Case EmnistDataSetIterator.Set.MERGE
					Return 47
				Case EmnistDataSetIterator.Set.BALANCED
					Return 47
				Case EmnistDataSetIterator.Set.LETTERS
					Return 26
				Case EmnistDataSetIterator.Set.DIGITS
					Return 10
				Case EmnistDataSetIterator.Set.MNIST
					Return 10
				Case Else
					Throw New System.NotSupportedException("Unknown Set: " & dataSet)
			End Select
		End Function
	End Class

End Namespace