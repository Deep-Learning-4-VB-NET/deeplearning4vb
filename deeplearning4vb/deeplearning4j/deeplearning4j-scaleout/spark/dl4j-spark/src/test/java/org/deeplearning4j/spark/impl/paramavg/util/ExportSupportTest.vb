Imports Platform = com.sun.jna.Platform
Imports SneakyThrows = lombok.SneakyThrows
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Configuration = org.apache.hadoop.conf.Configuration
Imports FileSystem = org.apache.hadoop.fs.FileSystem
Imports SparkConf = org.apache.spark.SparkConf
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports Downloader = org.nd4j.common.resources.Downloader
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.deeplearning4j.spark.impl.paramavg.util


	''' <summary>
	''' @author Ede Meijer
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ExportSupportTest
	Public Class ExportSupportTest
		Private Const FS_CONF As String = "spark.hadoop.fs.defaultFS"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SneakyThrows @BeforeEach void before()
		Friend Overridable Sub before()
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
'ORIGINAL LINE: @Test public void testLocalSupported() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLocalSupported()
			assertSupported((New SparkConf()).setMaster("local").set(FS_CONF, "file:///"))
			assertSupported((New SparkConf()).setMaster("local[2]").set(FS_CONF, "file:///"))
			assertSupported((New SparkConf()).setMaster("local[64]").set(FS_CONF, "file:///"))
			assertSupported((New SparkConf()).setMaster("local[*]").set(FS_CONF, "file:///"))

			assertSupported((New SparkConf()).setMaster("local").set(FS_CONF, "hdfs://localhost:9000"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testClusterWithRemoteFSSupported() throws IOException, java.net.URISyntaxException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testClusterWithRemoteFSSupported()
			assertSupported("spark://localhost:7077", FileSystem.get(New URI("hdfs://localhost:9000"), New Configuration()), True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testClusterWithLocalFSNotSupported() throws IOException, java.net.URISyntaxException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testClusterWithLocalFSNotSupported()
			assertSupported("spark://localhost:7077", FileSystem.get(New URI("file:///home/test"), New Configuration()), False)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void assertSupported(org.apache.spark.SparkConf conf) throws java.io.IOException
		Private Sub assertSupported(ByVal conf As SparkConf)
			Dim sc As New JavaSparkContext(conf.setAppName("Test").set("spark.driver.host", "localhost"))
			Try
				assertTrue(ExportSupport.exportSupported(sc))
			Finally
				sc.stop()
			End Try
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void assertSupported(String master, org.apache.hadoop.fs.FileSystem fs, boolean supported) throws java.io.IOException
		Private Sub assertSupported(ByVal master As String, ByVal fs As FileSystem, ByVal supported As Boolean)
			assertEquals(supported, ExportSupport.exportSupported(master, fs))
		End Sub
	End Class

End Namespace