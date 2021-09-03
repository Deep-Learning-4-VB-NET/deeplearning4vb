Imports System
Imports System.Threading
Imports Platform = com.sun.jna.Platform
Imports SneakyThrows = lombok.SneakyThrows
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SparkConf = org.apache.spark.SparkConf
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports Downloader = org.nd4j.common.resources.Downloader

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
Namespace org.datavec.spark


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Base Spark Test") public abstract class BaseSparkTest implements java.io.Serializable
	<Serializable>
	Public MustInherit Class BaseSparkTest

		Protected Friend Shared sc As JavaSparkContext

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
			sc = Context
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach synchronized void after()
		Friend Overridable Sub after()
			SyncLock Me
				sc.close()
				' Wait until it's stopped, to avoid race conditions during tests
				For i As Integer = 0 To 99
					If Not sc.sc().stopped().get() Then
						Try
							Thread.Sleep(100L)
						Catch e As InterruptedException
							log.error("", e)
						End Try
					Else
						Exit For
					End If
				Next i
				If Not sc.sc().stopped().get() Then
					Throw New Exception("Spark context is not stopped after 10s")
				End If
				sc = Nothing
			End SyncLock
		End Sub

		Public Overridable ReadOnly Property Context As JavaSparkContext
			Get
				SyncLock Me
					If sc IsNot Nothing Then
						Return sc
					End If
					Dim sparkConf As SparkConf = (New SparkConf()).setMaster("local[*]").set("spark.driver.host", "localhost").set("spark.driverEnv.SPARK_LOCAL_IP", "127.0.0.1").set("spark.executorEnv.SPARK_LOCAL_IP", "127.0.0.1").setAppName("sparktest")
					If useKryo() Then
						sparkConf.set("spark.serializer", "org.apache.spark.serializer.KryoSerializer")
					End If
					sc = New JavaSparkContext(sparkConf)
					Return sc
				End SyncLock
			End Get
		End Property

		Public Overridable Function useKryo() As Boolean
			Return False
		End Function
	End Class

End Namespace