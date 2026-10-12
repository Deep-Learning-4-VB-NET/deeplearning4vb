Imports System
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports BaseImageLoader = org.datavec.image.loader.BaseImageLoader
Imports ObjectDetectionRecordReader = org.datavec.image.recordreader.objdetect.ObjectDetectionRecordReader
Imports ImageTransform = org.datavec.image.transform.ImageTransform

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


	Public Class SvhnDataFetcher
		Inherits CacheableExtractableDataSetFetcher

		Private Shared BASE_URL As String = "http://ufldl.stanford.edu/"

		Public Shared WriteOnly Property BaseUrl As String
			Set(ByVal baseUrl As String)
				BASE_URL = baseUrl
			End Set
		End Property

		Public Shared NUM_LABELS As Integer = 10

		Public Overrides Function remoteDataUrl(ByVal set As DataSetType) As String
			Select Case set
				Case org.deeplearning4j.datasets.fetchers.DataSetType.TRAIN
					Return BASE_URL & "housenumbers/train.tar.gz"
				Case org.deeplearning4j.datasets.fetchers.DataSetType.TEST
					Return BASE_URL & "housenumbers/test.tar.gz"
				Case org.deeplearning4j.datasets.fetchers.DataSetType.VALIDATION
					Return BASE_URL & "housenumbers/extra.tar.gz"
				Case Else
					 Throw New System.ArgumentException("Unknown DataSetType:" & set)
			End Select
		End Function

		Public Overrides Function localCacheName() As String
			Return "SVHN"
		End Function

		Public Overrides Function dataSetName(ByVal set As DataSetType) As String
			Select Case set
				Case org.deeplearning4j.datasets.fetchers.DataSetType.TRAIN
					Return "train"
				Case org.deeplearning4j.datasets.fetchers.DataSetType.TEST
					Return "test"
				Case org.deeplearning4j.datasets.fetchers.DataSetType.VALIDATION
					Return "extra"
				Case Else
					Throw New System.ArgumentException("Unknown DataSetType:" & set)
			End Select
		End Function

		Public Overrides Function expectedChecksum(ByVal set As DataSetType) As Long
			Select Case set
				Case org.deeplearning4j.datasets.fetchers.DataSetType.TRAIN
					Return 979655493L
				Case org.deeplearning4j.datasets.fetchers.DataSetType.TEST
					Return 1629515343L
				Case org.deeplearning4j.datasets.fetchers.DataSetType.VALIDATION
					Return 132781169L
				Case Else
					 Throw New System.ArgumentException("Unknown DataSetType:" & set)
			End Select
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public java.io.File getDataSetPath(DataSetType set) throws java.io.IOException
		Public Overridable Function getDataSetPath(ByVal set As DataSetType) As File
			Dim localCache As File = LocalCacheDir
			' check empty cache
			deleteIfEmpty(localCache)

			Dim datasetPath As File
			Select Case set
				Case org.deeplearning4j.datasets.fetchers.DataSetType.TRAIN
					datasetPath = New File(localCache, "/train/")
				Case org.deeplearning4j.datasets.fetchers.DataSetType.TEST
					datasetPath = New File(localCache, "/test/")
				Case org.deeplearning4j.datasets.fetchers.DataSetType.VALIDATION
					datasetPath = New File(localCache, "/extra/")
				Case Else
					datasetPath = Nothing
			End Select

			If Not datasetPath.exists() Then
				downloadAndExtract(set)
			End If
			Return datasetPath
		End Function

		Public Overrides Function getRecordReader(ByVal rngSeed As Long, ByVal imgDim() As Integer, ByVal set As DataSetType, ByVal imageTransform As ImageTransform) As RecordReader
			Try
				Dim rng As New Random(rngSeed)
				Dim datasetPath As File = getDataSetPath(set)

				Dim data As New org.datavec.api.Split.FileSplit(datasetPath, BaseImageLoader.ALLOWED_FORMATS, rng)
				Dim recordReader As New ObjectDetectionRecordReader(imgDim(1), imgDim(0), imgDim(2), imgDim(4), imgDim(3), Nothing)

				recordReader.initialize(data)
				Return recordReader
			Catch e As IOException
				Throw New Exception("Could not download SVHN", e)
			End Try
		End Function
	End Class
End Namespace