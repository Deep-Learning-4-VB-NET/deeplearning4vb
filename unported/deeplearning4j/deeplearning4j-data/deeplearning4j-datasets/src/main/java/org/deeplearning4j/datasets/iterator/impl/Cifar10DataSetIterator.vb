Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Getter = lombok.Getter
Imports FileUtils = org.apache.commons.io.FileUtils
Imports ImageTransform = org.datavec.image.transform.ImageTransform
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports ResourceType = org.deeplearning4j.common.resources.ResourceType
Imports RecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderDataSetIterator
Imports Cifar10Fetcher = org.deeplearning4j.datasets.fetchers.Cifar10Fetcher
Imports DataSetType = org.deeplearning4j.datasets.fetchers.DataSetType
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor

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

Namespace org.deeplearning4j.datasets.iterator.impl


	<Serializable>
	Public Class Cifar10DataSetIterator
		Inherits RecordReaderDataSetIterator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.nd4j.linalg.dataset.api.DataSetPreProcessor preProcessor;
		Protected Friend Shadows preProcessor_Conflict As DataSetPreProcessor

		''' <summary>
		''' Create an iterator for the training set, with random iteration order (RNG seed fixed to 123)
		''' </summary>
		''' <param name="batchSize"> Minibatch size for the iterator </param>
		Public Sub New(ByVal batchSize As Integer)
			Me.New(batchSize, Nothing, DataSetType.TRAIN, Nothing, 123)
		End Sub

		''' <summary>
		''' * Create an iterator for the training or test set, with random iteration order (RNG seed fixed to 123)
		''' </summary>
		''' <param name="batchSize"> Minibatch size for the iterator </param>
		''' <param name="set">       The dataset (train or test) </param>
		Public Sub New(ByVal batchSize As Integer, ByVal set As DataSetType)
			Me.New(batchSize, Nothing, set, Nothing, 123)
		End Sub

		''' <summary>
		''' Get the Tiny ImageNet iterator with specified train/test set, with random iteration order (RNG seed fixed to 123)
		''' </summary>
		''' <param name="batchSize"> Size of each patch </param>
		''' <param name="imgDim"> Dimensions of desired output - for example, {64, 64} </param>
		''' <param name="set"> Train, test, or validation </param>
		Public Sub New(ByVal batchSize As Integer, ByVal imgDim() As Integer, ByVal set As DataSetType)
			Me.New(batchSize, imgDim, set, Nothing, 123)
		End Sub

		''' <summary>
		''' Get the Tiny ImageNet iterator with specified train/test set and (optional) custom transform.
		''' </summary>
		''' <param name="batchSize"> Size of each patch </param>
		''' <param name="imgDim"> Dimensions of desired output - for example, {64, 64} </param>
		''' <param name="set"> Train, test, or validation </param>
		''' <param name="imageTransform"> Additional image transform for output </param>
		''' <param name="rngSeed"> random number generator seed to use when shuffling examples </param>
		Public Sub New(ByVal batchSize As Integer, ByVal imgDim() As Integer, ByVal set As DataSetType, ByVal imageTransform As ImageTransform, ByVal rngSeed As Long)
			MyBase.New((New Cifar10Fetcher()).getRecordReader(rngSeed, imgDim, set, imageTransform), batchSize, 1, Cifar10Fetcher.NUM_LABELS)
		End Sub

		''' <summary>
		''' Get the labels - either in "categories" (imagenet synsets format, "n01910747" or similar) or human-readable format,
		''' such as "jellyfish" </summary>
		''' <param name="categories"> If true: return category/synset format; false: return "human readable" label format </param>
		''' <returns> Labels </returns>
		Public Shared Overloads Function getLabels(ByVal categories As Boolean) As IList(Of String)
			Dim rawLabels As IList(Of String) = (New Cifar10DataSetIterator(1)).getLabels()
			If categories Then
				Return rawLabels
			End If

			'Otherwise, convert to human-readable format, using 'words.txt' file
			Dim baseDir As File = DL4JResources.getDirectory(ResourceType.DATASET, Cifar10Fetcher.LOCAL_CACHE_NAME)
			Dim labelFile As New File(baseDir, Cifar10Fetcher.LABELS_FILENAME)
			Dim lines As IList(Of String)
			Try
				lines = FileUtils.readLines(labelFile, StandardCharsets.UTF_8)
			Catch e As IOException
				Throw New Exception("Error reading label file", e)
			End Try

			Dim map As IDictionary(Of String, String) = New Dictionary(Of String, String)()
			For Each line As String In lines
				Dim split() As String = line.Split(vbTab, True)
				map(split(0)) = split(1)
			Next line

			Dim outLabels As IList(Of String) = New List(Of String)(rawLabels.Count)
			For Each s As String In rawLabels
				Dim s2 As String = map(s)
				Preconditions.checkState(s2 IsNot Nothing, "Label ""%s"" not found in labels.txt file")
				outLabels.Add(s2)
			Next s
			Return outLabels
		End Function
	End Class

End Namespace