Imports System.Collections.Generic
Imports Platform = com.sun.jna.Platform
Imports SneakyThrows = lombok.SneakyThrows
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BeforeAll = org.junit.jupiter.api.BeforeAll
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Downloader = org.nd4j.common.resources.Downloader
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.deeplearning4j.spark.parameterserver.iterators


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag @Slf4j public class VirtualDataSetIteratorTest
	Public Class VirtualDataSetIteratorTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll @SneakyThrows public static void beforeAll()
		Public Shared Sub beforeAll()
			If Platform.isWindows() Then
				Dim hadoopHome As New File(System.getProperty("java.io.tmpdir"),"hadoop-tmp")
				Dim binDir As New File(hadoopHome,"bin")
				If Not binDir.exists() Then
					binDir.mkdirs()
				End If
				Dim outputFile As New File(binDir,"winutils.exe")
				If Not outputFile.exists() Then
					log.info("Fixing spark for windows")
					Downloader.download("winutils.exe", URI.create("https://github.com/cdarlint/winutils/blob/master/hadoop-2.6.5/bin/winutils.exe?raw=true").toURL(), outputFile,"db24b404d2331a1bec7443336a5171f1",3)
				End If

				System.setProperty("hadoop.home.dir", hadoopHome.getAbsolutePath())
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSimple1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSimple1()
			Dim iterators As IList(Of IEnumerator(Of DataSet)) = New List(Of IEnumerator(Of DataSet))()

			Dim first As IList(Of DataSet) = New List(Of DataSet)()
			Dim second As IList(Of DataSet) = New List(Of DataSet)()

			For i As Integer = 0 To 99
				Dim features As INDArray = Nd4j.create(100).assign(i)
				Dim labels As INDArray = Nd4j.create(10).assign(i)
				Dim ds As New DataSet(features, labels)

				If i < 25 Then
					first.Add(ds)
				Else
					second.Add(ds)
				End If
			Next i

			iterators.Add(first.GetEnumerator())
			iterators.Add(second.GetEnumerator())

			Dim vdsi As New VirtualDataSetIterator(iterators)
			Dim cnt As Integer = 0
			Do While vdsi.MoveNext()
				Dim ds As DataSet = vdsi.Current

				assertEquals(CDbl(cnt), ds.Features.meanNumber().doubleValue(), 0.0001)
				assertEquals(CDbl(cnt), ds.Labels.meanNumber().doubleValue(), 0.0001)

				cnt += 1
			Loop

			assertEquals(100, cnt)
		End Sub
	End Class

End Namespace