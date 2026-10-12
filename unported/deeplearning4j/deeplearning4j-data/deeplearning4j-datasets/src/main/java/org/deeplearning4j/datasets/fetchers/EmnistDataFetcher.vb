Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports EmnistFetcher = org.deeplearning4j.datasets.base.EmnistFetcher
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports ResourceType = org.deeplearning4j.common.resources.ResourceType
Imports EmnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator
Imports MnistManager = org.deeplearning4j.datasets.mnist.MnistManager
Imports DataSetFetcher = org.nd4j.linalg.dataset.api.iterator.fetcher.DataSetFetcher

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
'ORIGINAL LINE: @Slf4j public class EmnistDataFetcher extends MnistDataFetcher implements org.nd4j.linalg.dataset.api.iterator.fetcher.DataSetFetcher
	<Serializable>
	Public Class EmnistDataFetcher
		Inherits MnistDataFetcher
		Implements DataSetFetcher

		Protected Friend fetcher As EmnistFetcher

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public EmnistDataFetcher(org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.@Set dataSet, boolean binarize, boolean train, boolean shuffle, long rngSeed) throws java.io.IOException
		Public Sub New(ByVal dataSet As EmnistDataSetIterator.Set, ByVal binarize As Boolean, ByVal train As Boolean, ByVal shuffle As Boolean, ByVal rngSeed As Long)
			fetcher = New EmnistFetcher(dataSet)
			If Not emnistExists(fetcher) Then
				fetcher.downloadAndUntar()
			End If


			Dim EMNIST_ROOT As String = DL4JResources.getDirectory(ResourceType.DATASET, "EMNIST").getAbsolutePath()
			If train Then
				images = FilenameUtils.concat(EMNIST_ROOT, fetcher.TrainingFilesFilename_unzipped)
				labels = FilenameUtils.concat(EMNIST_ROOT, fetcher.TrainingFileLabelsFilename_unzipped)
				totalExamples_Conflict = EmnistDataSetIterator.numExamplesTrain(dataSet)
			Else
				images = FilenameUtils.concat(EMNIST_ROOT, fetcher.TestFilesFilename_unzipped)
				labels = FilenameUtils.concat(EMNIST_ROOT, fetcher.TestFileLabelsFilename_unzipped)
				totalExamples_Conflict = EmnistDataSetIterator.numExamplesTest(dataSet)
			End If
			Dim man As MnistManager
			Try
				man = New MnistManager(images, labels, totalExamples_Conflict)
			Catch e As Exception
				log.error("",e)
				FileUtils.deleteDirectory(New File(EMNIST_ROOT))
				Call (New EmnistFetcher(dataSet)).downloadAndUntar()
				man = New MnistManager(images, labels, totalExamples_Conflict)
			End Try

			numOutcomes = EmnistDataSetIterator.numLabels(dataSet)
			Me.binarize = binarize
			cursor_Conflict = 0
			man.setCurrent(cursor_Conflict)
			inputColumns_Conflict = man.Images.EntryLength
			Me.train = train
			Me.shuffle = shuffle

			order = New Integer(totalExamples_Conflict - 1){}
			For i As Integer = 0 To order.Length - 1
				order(i) = i
			Next i
			rng = New Random(rngSeed)
			reset() 'Shuffle order


			'For some inexplicable reason, EMNIST LETTERS set is indexed 1 to 26 (i.e., 1 to nClasses), while everything else
			' is indexed (0 to nClasses-1) :/
			If dataSet = EmnistDataSetIterator.Set.LETTERS Then
				oneIndexed = True
			Else
				oneIndexed = False
			End If
			Me.fOrder = True 'MNIST is C order, EMNIST is F order
			man.close()
		End Sub

		Private Function emnistExists(ByVal e As EmnistFetcher) As Boolean
			'Check 4 files:
			Dim EMNIST_ROOT As String = DL4JResources.getDirectory(ResourceType.DATASET, "EMNIST").getAbsolutePath()
			Dim f As New File(EMNIST_ROOT, e.TrainingFilesFilename_unzipped)
			If Not f.exists() Then
				Return False
			End If
			f = New File(EMNIST_ROOT, e.TrainingFileLabelsFilename_unzipped)
			If Not f.exists() Then
				Return False
			End If
			f = New File(EMNIST_ROOT, e.TestFilesFilename_unzipped)
			If Not f.exists() Then
				Return False
			End If
			f = New File(EMNIST_ROOT, e.TestFileLabelsFilename_unzipped)
			If Not f.exists() Then
				Return False
			End If
			Return True
		End Function
	End Class

End Namespace