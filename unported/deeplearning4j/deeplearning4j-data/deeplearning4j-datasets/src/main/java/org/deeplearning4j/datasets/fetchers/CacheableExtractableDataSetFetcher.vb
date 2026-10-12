Imports System
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports ResourceType = org.deeplearning4j.common.resources.ResourceType
Imports ArchiveUtils = org.nd4j.common.util.ArchiveUtils

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
'ORIGINAL LINE: @Slf4j public abstract class CacheableExtractableDataSetFetcher implements CacheableDataSet
	Public MustInherit Class CacheableExtractableDataSetFetcher
		Implements CacheableDataSet

		Public MustOverride Function getRecordReader(ByVal rngSeed As Long, ByVal imgDim() As Integer, ByVal set As DataSetType, ByVal imageTransform As org.datavec.image.transform.ImageTransform) As org.datavec.api.records.reader.RecordReader
		Public MustOverride Function expectedChecksum(ByVal set As DataSetType) As Long Implements CacheableDataSet.expectedChecksum
		Public MustOverride Function localCacheName() As String Implements CacheableDataSet.localCacheName
		Public MustOverride Function remoteDataUrl(ByVal set As DataSetType) As String Implements CacheableDataSet.remoteDataUrl
		Public Overridable Function dataSetName(ByVal set As DataSetType) As String Implements CacheableDataSet.dataSetName
			Return ""
		End Function
		Public Overridable Function remoteDataUrl() As String Implements CacheableDataSet.remoteDataUrl
			Return remoteDataUrl(DataSetType.TRAIN)
		End Function
		Public Overridable Function expectedChecksum() As Long Implements CacheableDataSet.expectedChecksum
			Return expectedChecksum(DataSetType.TRAIN)
		End Function
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void downloadAndExtract() throws java.io.IOException
		Public Overridable Sub downloadAndExtract()
			downloadAndExtract(DataSetType.TRAIN)
		End Sub

		''' <summary>
		''' Downloads and extracts the local dataset.
		''' </summary>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void downloadAndExtract(DataSetType set) throws java.io.IOException
		Public Overridable Sub downloadAndExtract(ByVal set As DataSetType)
			Dim localFilename As String = Path.GetFileName(remoteDataUrl(set))
			Dim tmpFile As New File(System.getProperty("java.io.tmpdir"), localFilename)
			Dim localCacheDir As File = Me.LocalCacheDir

			' check empty cache
			If localCacheDir.exists() Then
				Dim list() As File = localCacheDir.listFiles()
				If list Is Nothing OrElse list.Length = 0 Then
					localCacheDir.delete()
				End If
			End If

			Dim localDestinationDir As New File(localCacheDir, dataSetName(set))
			If Not localDestinationDir.exists() Then
				localCacheDir.mkdirs()
				tmpFile.delete()
				log.info("Downloading dataset to " & tmpFile.getAbsolutePath())
				FileUtils.copyURLToFile(New URL(remoteDataUrl(set)), tmpFile)
			Else
				'Directory exists and is non-empty - assume OK
				log.info("Using cached dataset at " & localCacheDir.getAbsolutePath())
				Return
			End If

			If expectedChecksum(set) <> 0L Then
				log.info("Verifying download...")
				Dim adler As Checksum = New Adler32()
				FileUtils.checksum(tmpFile, adler)
				Dim localChecksum As Long = adler.getValue()
				log.info("Checksum local is " & localChecksum & ", expecting " & expectedChecksum(set))

				If expectedChecksum(set) <> localChecksum Then
					log.error("Checksums do not match. Cleaning up files and failing...")
					tmpFile.delete()
					Throw New System.InvalidOperationException("Dataset file failed checksum: " & tmpFile & " - expected checksum " & expectedChecksum(set) & " vs. actual checksum " & localChecksum & ". If this error persists, please open an issue at https://github.com/eclipse/deeplearning4j.")
				End If
			End If

			Try
				ArchiveUtils.unzipFileTo(tmpFile.getAbsolutePath(), localCacheDir.getAbsolutePath(), False)
			Catch t As Exception
				'Catch any errors during extraction, and delete the directory to avoid leaving the dir in an invalid state
				If localCacheDir.exists() Then
					FileUtils.deleteDirectory(localCacheDir)
				End If
				Throw t
			End Try
		End Sub

		Protected Friend Overridable ReadOnly Property LocalCacheDir As File
			Get
				Return DL4JResources.getDirectory(ResourceType.DATASET, localCacheName())
			End Get
		End Property

		''' <summary>
		''' Returns a boolean indicating if the dataset is already cached locally.
		''' </summary>
		''' <returns> boolean </returns>
		Public Overridable ReadOnly Property Cached As Boolean Implements CacheableDataSet.isCached
			Get
				Return LocalCacheDir.exists()
			End Get
		End Property


		Protected Friend Shared Sub deleteIfEmpty(ByVal localCache As File)
			If localCache.exists() Then
				Dim files() As File = localCache.listFiles()
				If files Is Nothing OrElse files.Length < 1 Then
					Try
						FileUtils.deleteDirectory(localCache)
					Catch e As IOException
						'Ignore
						log.debug("Error deleting directory: {}", localCache)
					End Try
				End If
			End If
		End Sub
	End Class

End Namespace