Imports RecordReader = org.datavec.api.records.reader.RecordReader
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

	Friend Interface CacheableDataSet

		Function remoteDataUrl() As String
		Function remoteDataUrl(ByVal set As DataSetType) As String
		Function localCacheName() As String
		Function dataSetName(ByVal set As DataSetType) As String
		Function expectedChecksum() As Long
		Function expectedChecksum(ByVal set As DataSetType) As Long
		ReadOnly Property Cached As Boolean
		Function getRecordReader(ByVal rngSeed As Long, ByVal imgDim() As Integer, ByVal set As DataSetType, ByVal imageTransform As ImageTransform) As RecordReader

	End Interface

End Namespace