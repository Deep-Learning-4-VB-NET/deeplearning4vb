Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Cifar10Fetcher = org.deeplearning4j.datasets.fetchers.Cifar10Fetcher
Imports TinyImageNetFetcher = org.deeplearning4j.datasets.fetchers.TinyImageNetFetcher
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames

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

Namespace org.deeplearning4j.datasets

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.FILE_IO) @Tag(TagNames.NDARRAY_ETL) public class TestDataSets extends org.deeplearning4j.BaseDL4JTest
	Public Class TestDataSets
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 180000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTinyImageNetExists() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTinyImageNetExists()
			'Simple sanity check on extracting
			Dim f As New TinyImageNetFetcher()
			f.downloadAndExtract()
			f.downloadAndExtract()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCifar10Exists() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCifar10Exists()
			'Simple sanity check on extracting
			Dim f As New Cifar10Fetcher()
			f.downloadAndExtract()
			f.downloadAndExtract()
		End Sub

	End Class

End Namespace