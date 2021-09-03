Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
import static org.junit.jupiter.api.Assertions.assertTrue
import static org.junit.jupiter.api.Assumptions.assumeTrue
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
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
Namespace org.deeplearning4j.datasets.fetchers

	''' <summary>
	''' @author saudet
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Svhn Data Fetcher Test") @Tag(TagNames.FILE_IO) @NativeTag class SvhnDataFetcherTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class SvhnDataFetcherTest
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				' Shouldn't take this long but slow download or drive access on CI machines may need extra time.
				Return 480_000_000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Svhn Data Fetcher") void testSvhnDataFetcher() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSvhnDataFetcher()
			' Ignore unless integration tests - CI can get caught up on slow disk access
			assumeTrue(IntegrationTests)
			Dim fetch As New SvhnDataFetcher()
			Dim path As File = fetch.getDataSetPath(DataSetType.TRAIN)
			Dim path2 As File = fetch.getDataSetPath(DataSetType.TEST)
			Dim path3 As File = fetch.getDataSetPath(DataSetType.VALIDATION)
			assertTrue(path.isDirectory())
			assertTrue(path2.isDirectory())
			assertTrue(path3.isDirectory())
		End Sub
	End Class

End Namespace