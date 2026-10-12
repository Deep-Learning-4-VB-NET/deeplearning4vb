Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MnistFetcher = org.deeplearning4j.datasets.base.MnistFetcher
Imports Cifar10Fetcher = org.deeplearning4j.datasets.fetchers.Cifar10Fetcher
Imports TinyImageNetFetcher = org.deeplearning4j.datasets.fetchers.TinyImageNetFetcher
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertNull

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
'ORIGINAL LINE: @NativeTag @Tag(TagNames.FILE_IO) public class ConcurrentDownloaderTest extends org.deeplearning4j.BaseDL4JTest
	Public Class ConcurrentDownloaderTest
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 180000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConcurrentDownloadingOfSameDataSet() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testConcurrentDownloadingOfSameDataSet()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ExecutorService executorService = Executors.newFixedThreadPool(24);
			Dim executorService As ExecutorService = Executors.newFixedThreadPool(24)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ExecutorCompletionService<Object> completionService = new ExecutorCompletionService<>(executorService);
			Dim completionService As New ExecutorCompletionService(Of Object)(executorService)

			Const expected As Integer = 24
			For i As Integer = 0 To expected - 1
				completionService.submit(Function() (New MnistFetcher()).downloadAndUntar())
			Next i

			Dim received As Integer = 0
			Dim e As Exception = Nothing
			Do While received < expected AndAlso e Is Nothing
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Future<Object> result = completionService.take();
				Dim result As Future(Of Object) = completionService.take()
				Try
					result.get()
					received += 1
				Catch executionException As ExecutionException
					e = executionException.InnerException
				End Try
			Loop

			executorService.shutdownNow()
			assertNull(e)
		End Sub
	End Class

End Namespace