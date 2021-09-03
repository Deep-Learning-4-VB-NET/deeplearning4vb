Imports System
Imports RandomPathFilter = org.datavec.api.io.filters.RandomPathFilter
Imports ParentPathLabelGenerator = org.datavec.api.io.labels.ParentPathLabelGenerator
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports InputSplit = org.datavec.api.split.InputSplit
Imports BaseImageLoader = org.datavec.image.loader.BaseImageLoader
Imports ImageRecordReader = org.datavec.image.recordreader.ImageRecordReader
Imports ImageTransform = org.datavec.image.transform.ImageTransform
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports Preconditions = org.nd4j.common.base.Preconditions

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


	Public Class TinyImageNetFetcher
		Inherits CacheableExtractableDataSetFetcher

		Public Const WORDS_FILENAME As String = "words.txt"
		Public Const LOCAL_CACHE_NAME As String = "TINYIMAGENET_200"

		Public Shared INPUT_WIDTH As Integer = 64
		Public Shared INPUT_HEIGHT As Integer = 64
		Public Shared INPUT_CHANNELS As Integer = 3
		Public Shared NUM_LABELS As Integer = 200
		Public Shared NUM_EXAMPLES As Integer = NUM_LABELS*500

		Public Overrides Function remoteDataUrl(ByVal set As DataSetType) As String
			Return DL4JResources.getURLString("datasets/tinyimagenet_200_dl4j.v1.zip")
		End Function
		Public Overrides Function localCacheName() As String
			Return LOCAL_CACHE_NAME
		End Function
		Public Overrides Function expectedChecksum(ByVal set As DataSetType) As Long
			Return 33822361L
		End Function
		Public Overrides Function getRecordReader(ByVal rngSeed As Long, ByVal imgDim() As Integer, ByVal set As DataSetType, ByVal imageTransform As ImageTransform) As RecordReader
			Preconditions.checkState(imgDim Is Nothing OrElse imgDim.Length = 2, "Invalid image dimensions: must be null or lenth 2. Got: %s", imgDim)
			' check empty cache
			Dim localCache As File = LocalCacheDir
			deleteIfEmpty(localCache)

			Try
				If Not localCache.exists() Then
					downloadAndExtract()
				End If
			Catch e As Exception
				Throw New Exception("Could not download TinyImageNet", e)
			End Try

			Dim rng As New Random(rngSeed)
			Dim datasetPath As File
			Select Case set
				Case org.deeplearning4j.datasets.fetchers.DataSetType.TRAIN
					datasetPath = New File(localCache, "/train/")
				Case org.deeplearning4j.datasets.fetchers.DataSetType.TEST
					datasetPath = New File(localCache, "/test/")
				Case org.deeplearning4j.datasets.fetchers.DataSetType.VALIDATION
					Throw New System.ArgumentException("You will need to manually iterate the /validation/images/ directory, TinyImageNet does not provide labels")

				Case Else
					datasetPath = New File(localCache, "/train/")
			End Select

			' set up file paths
			Dim pathFilter As New RandomPathFilter(rng, BaseImageLoader.ALLOWED_FORMATS)
			Dim filesInDir As New org.datavec.api.Split.FileSplit(datasetPath, BaseImageLoader.ALLOWED_FORMATS, rng)
			Dim filesInDirSplit() As org.datavec.api.Split.InputSplit = filesInDir.sample(pathFilter, 1)

			Dim h As Integer = (If(imgDim Is Nothing, TinyImageNetFetcher.INPUT_HEIGHT, imgDim(0)))
			Dim w As Integer = (If(imgDim Is Nothing, TinyImageNetFetcher.INPUT_WIDTH, imgDim(1)))
			Dim rr As New ImageRecordReader(h, w,TinyImageNetFetcher.INPUT_CHANNELS, New ParentPathLabelGenerator(), imageTransform)

			Try
				rr.initialize(filesInDirSplit(0))
			Catch e As Exception
				Throw New Exception(e)
			End Try

			Return rr
		End Function


	End Class
End Namespace